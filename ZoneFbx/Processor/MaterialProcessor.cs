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

        //private readonly Dictionary<string, List<MaterialTextureHelper>> materialTextureDict = [];
        private readonly Dictionary<string, Dictionary<string, MaterialTextureHelper.EffectiveTextureUsage>> materialTextureDict = [];
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
            HashSet<string> alreadyRecorded = new HashSet<string>();
            string filename;
            for (int i = 0; i < material.Textures.Length; i++)
            {
                var texture = material.Textures[i];
                IntPtr textureObject;
                if (texture == null || texture.TexturePath.Contains("dummy")) continue;
                if (alreadySet.Contains(texture.TextureUsageSimple))
                {
                    textureProcessor.PrepareTexture(material, texture, materialInfo, out filename, "_blend");
                    if (!string.IsNullOrEmpty(filename) && !SurfacePhong.PropertyExists(outputSurface, texture.TextureUsageSimple.ToString()))
                    {
                        Property.CreateString(outputSurface, $"Blend{texture.TextureUsageSimple.ToString()}", filename);
                        alreadyRecorded.Add(filename);
                    }
                    AddToMaterialTextureDict(filename, material, getUsage(texture.TextureUsageSimple));
                    continue;
                }
                alreadySet.Add(texture.TextureUsageSimple);

                textureObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out filename);
                var emissiveObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out var emissiveFilename, "_e");

                if (textureObject != IntPtr.Zero)
                {
                    AddToMaterialTextureDict(filename, material, getUsage(texture.TextureUsageSimple));
                    alreadyRecorded.Add(filename);

                    if (materialInfo != null && materialInfo.Diffuse2Color != null)
                    {
                        textureProcessor.PrepareTexture(material, texture, materialInfo, out var diffuse2Filename, "_blend");
                        AddToMaterialTextureDict(diffuse2Filename, material, MaterialTextureHelper.EffectiveTextureUsage.Diffuse);
                        Property.CreateString(outputSurface, $"Blend{texture.TextureUsageSimple.ToString()}", diffuse2Filename);
                    }
                }
                if (emissiveObject != IntPtr.Zero)
                {
                    alreadyRecorded.Add(filename);
                    AddToMaterialTextureDict(emissiveFilename, material, MaterialTextureHelper.EffectiveTextureUsage.Emissive);

                    if (materialInfo != null && materialInfo.Emissive2Color != null && texture.TextureUsageSimple == Texture.Usage.Diffuse)
                    {
                        textureProcessor.PrepareTexture(material, texture, materialInfo, out var emissive2Filename, "_e_blend");
                        AddToMaterialTextureDict(emissiveFilename, material, MaterialTextureHelper.EffectiveTextureUsage.Emissive);
                        Property.CreateString(outputSurface, $"BlendEmissive", emissive2Filename);
                    }
                }

                connectSrcObjects(texture.TextureUsageSimple, outputSurface, textureObject, emissiveObject);
            }

            materialCache.Add(hash, outputSurface);
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
            if (!materialTextureDict.TryGetValue(mtrlFileName, out var subdict))
            {
                subdict = new();
                materialTextureDict.Add(mtrlFileName, subdict);
            }
                subdict.TryAdd(filename, usage);
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
