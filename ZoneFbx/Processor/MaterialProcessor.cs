using Lumina.Excel.Sheets;
using Lumina.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ZoneFbx.Fbx;
using static System.Net.Mime.MediaTypeNames;

namespace ZoneFbx.Processor
{
    internal class MaterialProcessor
    {
        private readonly Lumina.GameData data;
        private readonly TextureProcessor textureProcessor;
        private readonly ZoneExporter.Flags flags;
        private readonly IntPtr scene;

        private readonly Dictionary<ulong, IntPtr> materialCache = [];

        public MaterialProcessor(Lumina.GameData data, TextureProcessor textureProcessor, IntPtr scene, ZoneExporter.Flags flags)
        {
            this.data = data;
            this.textureProcessor = textureProcessor;
            this.scene = scene;
            this.flags = flags;
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
                    continue;
                }
                alreadySet.Add(texture.TextureUsageSimple);

                var textureObject = textureProcessor.PrepareTexture(material, texture, materialInfo);
                var emissiveObject = textureProcessor.PrepareTexture(material, texture, materialInfo, "_e");
                ConnectSrcObjects(texture.TextureUsageSimple, outputSurface, textureObject, emissiveObject);
            }

            materialCache[hash] = outputSurface;
            return outputSurface;
        }

        private void ConnectSrcObjects(Texture.Usage type, IntPtr outputSurface, IntPtr texture, IntPtr emissive)
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
