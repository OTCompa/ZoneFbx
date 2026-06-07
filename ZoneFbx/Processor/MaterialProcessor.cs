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
                return HandleLightshaft(material);
            }

            if (material.File == null) return IntPtr.Zero;
            var hash = material.File.FilePath.IndexHash;
            if (materialCache.TryGetValue(hash, out var res)) return res;

            var materialInfo = options.disableBaking ? null : new MaterialInfo(material, outputPath, options);
            var outputSurface = SurfacePhong.Create(contextManager, Path.GetFileNameWithoutExtension(material.MaterialPath));
            SurfacePhong.SetFactor(outputSurface, options.specularFactor, options.normalFactor);

            var primaryAssigned = new HashSet<Texture.Usage>();
            foreach (var texture in material.Textures)
            {
                if (texture == null || texture.TexturePath.Contains("dummy")) continue;

                if (!IsBlendSlot(texture))
                {
                    ApplyPrimaryTexture(material, texture, materialInfo, outputSurface, primaryAssigned);
                }
                else
                {
                    ApplyBlendTexture(material, texture, materialInfo, outputSurface);
                }
            }

            materialCache.Add(hash, outputSurface);
            return outputSurface;
        }

        private static bool IsBlendSlot(Texture texture) =>
            texture.TextureUsageRaw.ToString()[^1] == '1';

        private void ApplyPrimaryTexture(Material material, Texture texture, MaterialInfo? materialInfo, IntPtr outputSurface, HashSet<Texture.Usage> primaryAssigned)
        {
            var usage = texture.TextureUsageSimple;

            if (primaryAssigned.Contains(usage))
                Console.WriteLine($"[MaterialProcessor] Warning: overwriting primary {usage} for {material.MaterialPath} ({texture.TextureUsageRaw})");

            var textureObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out var filePath);
            if (textureObject == IntPtr.Zero) return;

            primaryAssigned.Add(usage);
            addToMaterialTextureDict(Path.GetFileNameWithoutExtension(filePath), material, usage.ToString());
            connectSrcObjects(usage, outputSurface, textureObject);

            if (usage == Texture.Usage.Diffuse)
                ApplyDiffuseExtras(material, texture, materialInfo, outputSurface);
        }

        private void ApplyBlendTexture(Material material, Texture texture, MaterialInfo? materialInfo, IntPtr outputSurface)
        {
            if (!options.enableBlend) return;
            if (!options.disableBaking && (materialInfo == null || !materialInfo.DiffuseBlendEnabled)) return;

            var usage = texture.TextureUsageSimple;
            textureProcessor.PrepareTexture(material, texture, materialInfo, out var filePath);
            if (string.IsNullOrEmpty(filePath)) return;

            if (!SurfacePhong.PropertyExists(outputSurface, $"Blend{usage}"))
                Property.CreateString(outputSurface, $"Blend{usage}", Path.GetFileName(filePath));
            addToMaterialTextureDict(Path.GetFileNameWithoutExtension(filePath), material, $"Blend{usage}");
        }

        private void ApplyDiffuseExtras(Material material, Texture texture, MaterialInfo? materialInfo, IntPtr outputSurface)
        {
            var emissiveObject = textureProcessor.PrepareTexture(material, texture, materialInfo, out var emissiveFilePath, "_e");
            if (emissiveObject == IntPtr.Zero) return;

            SurfacePhong.ConnectEmissive(outputSurface, emissiveObject);
            addToMaterialTextureDict(Path.GetFileNameWithoutExtension(emissiveFilePath), material, "Emissive");

            if (!options.enableBlend || materialInfo == null || !materialInfo.DiffuseBlendEnabled) return;

            var dummyPath = textureProcessor.CreateEmissiveDummy();
            if (!string.IsNullOrEmpty(dummyPath) && !SurfacePhong.PropertyExists(outputSurface, "BlendEmissive"))
            {
                Property.CreateString(outputSurface, "BlendEmissive", Path.GetFileName(dummyPath));
                addToMaterialTextureDict(Path.GetFileNameWithoutExtension(dummyPath), material, "BlendEmissive");
            }
        }

        private IntPtr HandleLightshaft(Material material)
        {
            if (material.File == null) return IntPtr.Zero;
            var hash = material.File.FilePath.IndexHash;
            if (materialCache.TryGetValue(hash, out var res)) return res;

            var materialInfo = options.disableBaking ? null : new MaterialInfo(material, outputPath, options);
            var outputSurface = SurfacePhong.Create(contextManager, Path.GetFileNameWithoutExtension(material.MaterialPath));
            SurfacePhong.SetFactor(outputSurface, options.specularFactor, options.normalFactor);

            var primaryTexture = material.Textures.FirstOrDefault(t => !IsBlendSlot(t));
            if (primaryTexture != null)
            {
                var textureObject = textureProcessor.PrepareTexture(material, primaryTexture, null, out _);
                if (textureObject != IntPtr.Zero)
                    SurfacePhong.ConnectDiffuse(outputSurface, textureObject);

                var emissiveObject = textureProcessor.PrepareLightshaftEmission(material, primaryTexture, materialInfo, out _);
                if (emissiveObject != IntPtr.Zero)
                    SurfacePhong.ConnectEmissive(outputSurface, emissiveObject);
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
