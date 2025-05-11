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

        internal class MaterialTextureHelper
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum EffectiveTextureUsage
            {
                Unused,
                Diffuse,
                Specular,
                Normal,
                Emissive,
                Unknown,
            }

            public string filename;
            public EffectiveTextureUsage effectiveUse;

            public MaterialTextureHelper(string filename, EffectiveTextureUsage effectiveUse)
            {
                this.filename = filename;
                this.effectiveUse = effectiveUse;
            }
        }

        private readonly Dictionary<string, List<MaterialTextureHelper>> materialTextureDict = new();
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

            var materialInfo = flags.disableBaking ? null : new MaterialInfo(material);
            var outputSurface = SurfacePhong.Create(scene, Path.GetFileName(material.MaterialPath));
            SurfacePhong.SetFactor(outputSurface);

            HashSet<Texture.Usage> alreadySet = new HashSet<Texture.Usage>();
            for (int i = 0; i < material.Textures.Length; i++)
            {
                var texture = material.Textures[i];
                if (texture == null ||
                    texture.TexturePath.Contains("dummy") ||
                    alreadySet.Contains(texture.TextureUsageSimple))
                {
                    if (texture != null)
                    {
                        textureProcessor.PrepareTexture(material, texture, materialInfo, out var unusedFilename, $"_u{i}");
                        AddToMaterialTextureDict(unusedFilename, material, MaterialTextureHelper.EffectiveTextureUsage.Unused);
                        
                    }
                    continue;
                }
                alreadySet.Add(texture.TextureUsageSimple);

                var textureObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out var filename);
                var emissiveObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out var emissiveFilename, "_e");
                if (textureObject != IntPtr.Zero) AddToMaterialTextureDict(filename, material, getUsage(texture.TextureUsageSimple));
                if (emissiveObject != IntPtr.Zero) AddToMaterialTextureDict(emissiveFilename, material, MaterialTextureHelper.EffectiveTextureUsage.Emissive);
                connectSrcObjects(texture.TextureUsageSimple, outputSurface, textureObject, emissiveObject);
            }

            materialCache[hash] = outputSurface;
            return outputSurface;
        }

        public void ExportTextureJson()
        {
            var jsonExport = JsonConvert.SerializeObject(materialTextureDict, Formatting.Indented);
            File.WriteAllText(Path.Combine(outputPath, "materialTextureMap.json"), jsonExport);
        }

        private void AddToMaterialTextureDict(string filename, Material material, MaterialTextureHelper.EffectiveTextureUsage usage)
        {
            var mtrlFileName = Path.GetFileName(material.MaterialPath);
            if (!materialTextureDict.TryGetValue(mtrlFileName, out var arr))
            {
                arr = new();
                materialTextureDict.Add(mtrlFileName, arr);
            }
            arr.Add(new(filename, usage));
        }

        private MaterialTextureHelper.EffectiveTextureUsage getUsage(Texture.Usage usage)
        {
            switch (usage)
            {
                case Texture.Usage.Diffuse:
                    return MaterialTextureHelper.EffectiveTextureUsage.Diffuse;
                case Texture.Usage.Specular:
                    return MaterialTextureHelper.EffectiveTextureUsage.Specular;
                case Texture.Usage.Normal:
                    return MaterialTextureHelper.EffectiveTextureUsage.Normal;
                default:
                    return MaterialTextureHelper.EffectiveTextureUsage.Unknown;
            }
        }
        private void connectSrcObjects(Texture.Usage type, IntPtr outputSurface, IntPtr texture, IntPtr emissive)
        {
            switch (type)
            {
                case Texture.Usage.Diffuse:
                    if (emissive != IntPtr.Zero) SurfacePhong.ConnectSrcObject(outputSurface, emissive, 3);
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
