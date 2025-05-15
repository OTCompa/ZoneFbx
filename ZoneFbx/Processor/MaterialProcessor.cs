using Lumina.Models.Materials;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ZoneFbx.Fbx;

namespace ZoneFbx.Processor
{
    internal class MaterialProcessor
    {
        private readonly Lumina.GameData data;
        private readonly TextureProcessor textureProcessor;
        private readonly ZoneExporter.Options options;
        private readonly IntPtr scene;

        private readonly string outputPath;

        private readonly Dictionary<ulong, IntPtr> materialCache = [];

        private readonly Dictionary<string, Dictionary<string, string>> materialTextureDict = [];
        public MaterialProcessor(Lumina.GameData data, TextureProcessor textureProcessor, IntPtr scene, ZoneExporter.Options options, string outputPath)
        {
            this.data = data;
            this.textureProcessor = textureProcessor;
            this.scene = scene;
            this.options = options;
            this.outputPath = outputPath;
        }

        public IntPtr CreateMaterial(Material material)
        {
            if (!options.enableLightshaftModels && material.ShaderPack == "lightshaft.shpk") return IntPtr.Zero;

            if (material.File == null) return IntPtr.Zero;
            var hash = material.File.FilePath.IndexHash;
            if (materialCache.TryGetValue(hash, out var res)) return res;

            var materialInfo = options.disableBaking ? null : new MaterialInfo(material, outputPath, options);
            var outputSurface = SurfacePhong.Create(scene, Path.GetFileNameWithoutExtension(material.MaterialPath));
            SurfacePhong.SetFactor(outputSurface, options.specularFactor, options.normalFactor);

            HashSet<Texture.Usage> alreadySet = new HashSet<Texture.Usage>();
            string outputFilePath;
            for (int i = 0; i < material.Textures.Length; i++)
            {
                var texture = material.Textures[i];
                IntPtr textureObject;
                if (texture == null || texture.TexturePath.Contains("dummy")) continue;
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
                alreadySet.Add(texture.TextureUsageSimple);
                textureObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out outputFilePath);
                if (textureObject == IntPtr.Zero) return IntPtr.Zero;

                addToMaterialTextureDict(Path.GetFileNameWithoutExtension(outputFilePath), material, texture.TextureUsageSimple.ToString());
                if (texture.TextureUsageSimple == Texture.Usage.Diffuse)
                {
                    var emissiveObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out var emissiveFilename, "_e");
                    if (emissiveObject != IntPtr.Zero)
                    {
                        SurfacePhong.ConnectSrcObject(outputSurface, emissiveObject, 3);
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

        public void ExportJsonTextureMap()
        {
            var jsonExport = JsonConvert.SerializeObject(materialTextureDict, Formatting.Indented);
            var jsonFolder = Path.Combine(outputPath, "json");
            Directory.CreateDirectory(jsonFolder);
            File.WriteAllText(Path.Combine(jsonFolder, "materialTextureMap.json"), jsonExport);
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
                incrementAndTryAdd(subdict, $"Unused{usage}", filename);
            }
        }

        private void incrementAndTryAdd(Dictionary<string, string> dict, string usage, string filename, int num = 0)
        {
            if (!dict.TryAdd($"{usage}num", filename))
            {
                incrementAndTryAdd(dict, usage, filename, num + 1);
            }
        }

        private void connectSrcObjects(Texture.Usage type, IntPtr outputSurface, IntPtr texture)
        {
            switch (type)
            {
                case Texture.Usage.Diffuse:
                    SurfacePhong.ConnectSrcObject(outputSurface, texture, 0);
                    break;
                case Texture.Usage.Specular:
                    SurfacePhong.ConnectSrcObject(outputSurface, texture, 1);
                    break;
                case Texture.Usage.Normal:
                    SurfacePhong.ConnectSrcObject(outputSurface, texture, 2);
                    break;
            }
        }

    }
}
