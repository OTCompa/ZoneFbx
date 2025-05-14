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
        private readonly ZoneExporter.Flags flags;
        private readonly IntPtr scene;

        private readonly string outputPath;

        private readonly Dictionary<ulong, IntPtr> materialCache = [];

        private readonly Dictionary<string, Dictionary<string, string>> materialTextureDict = [];
        public MaterialProcessor(Lumina.GameData data, TextureProcessor textureProcessor, IntPtr scene, ZoneExporter.Flags flags, string outputPath)
        {
            this.data = data;
            this.textureProcessor = textureProcessor;
            this.scene = scene;
            this.flags = flags;
            this.outputPath = outputPath;
        }

        public IntPtr CreateMaterial(Material material)
        {
            if (!flags.enableLightshaftModels && material.ShaderPack == "lightshaft.shpk") return IntPtr.Zero;

            if (material.File == null) return IntPtr.Zero;
            var hash = material.File.FilePath.IndexHash;
            if (materialCache.TryGetValue(hash, out var res)) return res;

            var materialInfo = flags.disableBaking ? null : new MaterialInfo(material, outputPath, flags);
            var outputSurface = SurfacePhong.Create(scene, Path.GetFileName(material.MaterialPath));
            SurfacePhong.SetFactor(outputSurface);

            HashSet<Texture.Usage> alreadySet = new HashSet<Texture.Usage>();
            string filename;
            for (int i = 0; i < material.Textures.Length; i++)
            {
                var texture = material.Textures[i];
                IntPtr textureObject;
                if (texture == null || texture.TexturePath.Contains("dummy")) continue;
                if (alreadySet.Contains(texture.TextureUsageSimple))
                {
                    if (flags.enableBlend)
                    {
                        if (flags.disableBaking || (materialInfo != null && materialInfo.DiffuseBlendEnabled))
                        {
                            textureProcessor.PrepareTexture(material, texture, materialInfo, out filename, "_blend");
                            if (!string.IsNullOrEmpty(filename) && !SurfacePhong.PropertyExists(outputSurface, $"Blend{texture.TextureUsageSimple}"))
                            {
                                Property.CreateString(outputSurface, $"Blend{texture.TextureUsageSimple}", filename);
                            }
                            AddToMaterialTextureDict(filename, material, texture.TextureUsageRaw.ToString());
                        }
                    }
                    continue;
                }
                alreadySet.Add(texture.TextureUsageSimple);
                textureObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out filename);
                if (textureObject == IntPtr.Zero) return IntPtr.Zero;

                AddToMaterialTextureDict(filename, material, texture.TextureUsageRaw.ToString());
                if (texture.TextureUsageSimple == Texture.Usage.Diffuse)
                {
                    var emissiveObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out var emissiveFilename, "_e");
                    if (emissiveObject != IntPtr.Zero)
                    {
                        SurfacePhong.ConnectSrcObject(outputSurface, emissiveObject, 3);
                        AddToMaterialTextureDict(emissiveFilename, material, "Emissive");

                        // for materials that blend textures, if they contain an emissive, create a dummy texture for the emissive to blend with
                        // this may need to be researched more but it produces what looks to be the correct result for the maps i've tested so far?
                        if (flags.enableBlend && materialInfo != null && materialInfo.DiffuseBlendEnabled)
                        {
                            var emissiveDummy = textureProcessor.CreateEmissiveDummy();
                            if (!string.IsNullOrEmpty(emissiveDummy) && !SurfacePhong.PropertyExists(outputSurface, "BlendEmissive"))
                            {
                                Property.CreateString(outputSurface, "BlendEmissive", emissiveDummy);
                                AddToMaterialTextureDict(emissiveDummy, material, "BlendEmissive");
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

        private void AddToMaterialTextureDict(string filename, Material material, string usage)
        {
            if (!materialTextureDict.TryGetValue(material.MaterialPath, out var subdict))
            {
                subdict = new();
                materialTextureDict.Add(material.MaterialPath, subdict);
            }
                subdict.TryAdd(filename, usage);
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
