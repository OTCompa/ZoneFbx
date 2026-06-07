using Lumina.Models.Materials;
using Newtonsoft.Json;
using ZoneFbx.Fbx;

namespace ZoneFbx.Processor
{
    internal class MaterialProcessor(Lumina.GameData data, IntPtr contextManager, ZoneExporter.Options options, TextureProcessor textureProcessor, string outputPath) : Processor(data, contextManager, options)
    {
        private readonly TextureProcessor textureProcessor = textureProcessor;

        private readonly string outputPath = outputPath;

        private readonly Dictionary<ulong, IntPtr> materialCache = [];

        private readonly Dictionary<string, Dictionary<string, string>> materialTextureDict = [];

        public void ResetCache()
        {
            materialCache.Clear();
            materialTextureDict.Clear();
        }

        public IntPtr CreateMaterial(Material material)
        {
            if (material.ShaderPack == "lightshaft.shpk")
            {
                if (!options.enableLightshaftModels) return IntPtr.Zero;
                return handleLightshaft(material);
            }
            // the above isn't pretty but I don't really wannt touch anything below this block 
            // until I get around to cleaning this up

            if (material.File == null) return IntPtr.Zero;
            var hash = material.File.FilePath.IndexHash;
            if (materialCache.TryGetValue(hash, out var res)) return res;

            var materialInfo = options.disableBaking ? null : new MaterialInfo(material, outputPath, options);
            var outputSurface = SurfacePhong.Create(contextManager, Path.GetFileNameWithoutExtension(material.MaterialPath));
            SurfacePhong.SetFactor(outputSurface, options.specularFactor, options.normalFactor);

            // TODO: clean this up
            HashSet<Texture.Usage> alreadySet = new HashSet<Texture.Usage>();
            string outputFilePath;
            for (int i = 0; i < material.Textures.Length; i++)
            {
                var texture = material.Textures[i];
                IntPtr textureObject;
                if (texture == null || texture.TexturePath.Contains("dummy")) continue;

                // blending logic
                // for bgUvScroll this might be reversed where the second texture is displayed by default
                if (alreadySet.Contains(texture.TextureUsageSimple))
                {
                    if (options.enableBlend && (options.disableBaking || (materialInfo != null && materialInfo.DiffuseBlendEnabled)))
                    {
                        textureProcessor.PrepareTexture(material, texture, materialInfo, out outputFilePath, "_blend");
                        if (!string.IsNullOrEmpty(outputFilePath) && !SurfacePhong.PropertyExists(outputSurface, $"Blend{texture.TextureUsageSimple}"))
                        {
                            Property.CreateString(outputSurface, $"Blend{texture.TextureUsageSimple}", Path.GetFileName(outputFilePath));
                        }
                        addToMaterialTextureDict(Path.GetFileNameWithoutExtension(outputFilePath), material, $"Blend{texture.TextureUsageSimple}");
                    }
                    continue;
                }
                // there's an unhandled edge case here where the following fails and is supposed to be a blended texture. this hopefully shouldn't ever happen
                // if it does happen i'll need to refactor when i have the time
                textureObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out outputFilePath);
                if (textureObject == IntPtr.Zero) continue;
                alreadySet.Add(texture.TextureUsageSimple);

                addToMaterialTextureDict(Path.GetFileNameWithoutExtension(outputFilePath), material, texture.TextureUsageSimple.ToString());
                if (texture.TextureUsageSimple == Texture.Usage.Diffuse)
                {
                    var emissiveObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out var emissiveFilename, "_e");
                    if (emissiveObject != IntPtr.Zero)
                    {
                        SurfacePhong.ConnectEmissive(outputSurface, emissiveObject);
                        addToMaterialTextureDict(Path.GetFileNameWithoutExtension(emissiveFilename), material, "Emissive");

                        // for materials that blend textures, if they contain an emissive, create a dummy texture for the emissive to blend with
                        // this may need to be researched more but it produces what looks to be the correct result for the maps i've tested so far?
                        if (options.enableBlend && materialInfo != null && materialInfo.DiffuseBlendEnabled)
                        {
                            var emissiveDummyPath = textureProcessor.CreateEmissiveDummy();
                            if (!string.IsNullOrEmpty(emissiveDummyPath) && !SurfacePhong.PropertyExists(outputSurface, "BlendEmissive"))
                            {
                                Property.CreateString(outputSurface, "BlendEmissive", Path.GetFileName(emissiveDummyPath));
                                addToMaterialTextureDict(Path.GetFileNameWithoutExtension(emissiveDummyPath), material, "BlendEmissive");
                            }
                        }
                    }
                }

                connectSrcObjects(texture.TextureUsageSimple, outputSurface, textureObject);
            }

            materialCache.Add(hash, outputSurface);
            return outputSurface;
        }

        private IntPtr handleLightshaft(Material material)
        {
            if (!options.enableLightshaftModels) return IntPtr.Zero;
            if (material.File == null) return IntPtr.Zero;
            var hash = material.File.FilePath.IndexHash;
            if (materialCache.TryGetValue(hash, out var res)) return res;

            var materialInfo = options.disableBaking ? null : new MaterialInfo(material, outputPath, options);

            var outputSurface = SurfacePhong.Create(contextManager, Path.GetFileNameWithoutExtension(material.MaterialPath));
            SurfacePhong.SetFactor(outputSurface, options.specularFactor, options.normalFactor);

            string outputFilePath;
            for (int i = 0; i < material.Textures.Length; i++)
            {
                var texture = material.Textures[i];
                var textureObject = textureProcessor.PrepareTexture(material, texture, null, out outputFilePath);
                if (textureObject == IntPtr.Zero) continue;
                SurfacePhong.ConnectDiffuse(outputSurface, textureObject);

                var emissiveObject = textureProcessor.PrepareLightshaftEmission(material, texture, materialInfo, out _);
                if (emissiveObject != IntPtr.Zero)
                    SurfacePhong.ConnectEmissive(outputSurface, emissiveObject);

                // TODO: add blend for lightshaft
                break;
            }

            materialCache.Add(hash, outputSurface);
            return outputSurface;
        }

        public void ExportJsonTextureMap()
        {
            var jsonExport = JsonConvert.SerializeObject(materialTextureDict, Formatting.Indented);
            var jsonFolder = Path.Combine(outputPath, "json");
            Directory.CreateDirectory(jsonFolder);
            var suffix = options.mode == ZoneExporter.Mode.Default ? "" : "_festival";
            File.WriteAllText(Path.Combine(jsonFolder, $"materialTextureMap{suffix}.json"), jsonExport);
        }

        private void addToMaterialTextureDict(string filename, Material material, string usage)
        {
            var materialPath = material.MaterialPath;
            if (options.enableMTMap)
            {
                materialPath = Path.GetFileNameWithoutExtension(materialPath);
            }

            if (!materialTextureDict.TryGetValue(materialPath, out var subdict))
            {
                subdict = new();
                materialTextureDict.Add(materialPath, subdict);
            }
            if (!subdict.TryAdd(usage, filename))
            {
                int num = 0;
                while (!subdict.TryAdd($"Unused{usage}{num}", filename)) num++;
            }
        }

        private void connectSrcObjects(Texture.Usage type, IntPtr outputSurface, IntPtr texture)
        {
            switch (type)
            {
                case Texture.Usage.Diffuse:
                    SurfacePhong.ConnectDiffuse(outputSurface, texture);
                    break;
                case Texture.Usage.Specular:
                    SurfacePhong.ConnectSpecular(outputSurface, texture);
                    break;
                case Texture.Usage.Normal:
                    SurfacePhong.ConnectNormalMap(outputSurface, texture);
                    break;
            }
        }

    }
}
