using Lumina.Data.Files;
using Lumina.Models.Materials;
using System.Numerics;
using ZoneFbx.Fbx;

namespace ZoneFbx.Processor
{
    internal enum TextureMode { Default, Normal, Lightshaft }

    internal class TextureProcessor(Lumina.GameData data, IntPtr contextManager, ZoneExporter.Options options, string outputPath, string zoneCode) : Processor(data, contextManager, options)
    {
        private readonly string outputPath = outputPath;
        private readonly string zoneCode = zoneCode;

        public IntPtr PrepareTexture(Material material, Texture tex, MaterialInfo? materialInfo, out string filename, string suffix = "")
        {
            filename = "";
            Vector3? color = null;
            switch (tex.TextureUsageSimple)
            {
                case Texture.Usage.Diffuse:
                    // if actually diffuse
                    if (string.IsNullOrEmpty(suffix))
                    {
                        color = materialInfo?.DiffuseFactor;
                        break;
                    }

                    // if processing emissives instead
                    if (suffix.Equals("_e"))
                    {
                        color = materialInfo?.EmissiveFactor;
                        if (color == null) return IntPtr.Zero;
                    }
                    if (suffix.Equals("_blend"))
                    {
                        color = materialInfo?.BlendDiffuseFactor;
                        //if (materialInfo?.DiffuseColor != null && materialInfo?.DiffuseColor != Vector3.Zero) color *= materialInfo.DiffuseColor;
                        //if (color == null) return IntPtr.Zero;
                    }
                    break;
                case Texture.Usage.Specular:
                    color = materialInfo?.SpecularFactor; break;
            }
            filename = Util.GetTexturePath(outputPath, zoneCode, tex.TexturePath, material.MaterialPath, color, suffix);

            var mode = material.ShaderPack == "lightshaft.shpk" ? TextureMode.Lightshaft
                     : tex.TextureUsageSimple == Texture.Usage.Normal ? TextureMode.Normal
                     : TextureMode.Default;
            extractTexture(tex, color, filename, mode);

            if (!string.IsNullOrEmpty(suffix) && !suffix.Equals("_e")) return IntPtr.Zero;
            return initializeFileTexture(tex.TexturePath, filename, suffix);
        }

        public IntPtr CreateDiffuseDummy()
        {
            var filename = "diffuse_dummy.png";
            var fileOutputPath = Path.Combine(this.outputPath, "textures", filename);
            if (!File.Exists(fileOutputPath))
            {
                byte[] dummyData = [0, 0, 0, 0];
                Util.SaveAsBitmap(fileOutputPath, dummyData, 1, 1);
            }

            var texture = FileTexture.Create(contextManager, "diffuse_dummy");
            FileTexture.SetStuff(texture, fileOutputPath);
            return texture;
        }

        public string CreateEmissiveDummy()
        {
            var filename = "emission_dummy.png";
            var fileOutputPath = Path.Combine(this.outputPath, "textures", filename);
            if (File.Exists(fileOutputPath)) return fileOutputPath;

            byte[] dummyData = [0, 0, 0, 0];
            Util.SaveAsBitmap(fileOutputPath, dummyData, 1, 1);

            return fileOutputPath;
        }

        public IntPtr PrepareLightshaftEmission(Material material, Texture tex, MaterialInfo? materialInfo, out string filename)
        {
            filename = Util.GetTexturePath(outputPath, zoneCode, tex.TexturePath, material.MaterialPath, Vector3.One, "_e");
            extractTexture(tex, materialInfo?.LightshaftFactor, filename, TextureMode.Default);
            return initializeFileTexture(tex.TexturePath, filename, "_e");
        }

        private IntPtr initializeFileTexture(string texfilePath, string textureOutputPath, string suffix = "")
        {
            string textureObjectName = $"{Path.GetFileNameWithoutExtension(texfilePath)}{suffix}";
            var texture = FileTexture.Create(contextManager, textureObjectName);
            FileTexture.SetStuff(texture, textureOutputPath);
            return texture;
        }

        private void extractTexture(Texture tex, Vector3? color, string outputPath, TextureMode mode = TextureMode.Default)
        {
            if (File.Exists(outputPath)) return;

            TexFile? texfile = loadTexture(tex);
            if (texfile == null) return;

            try
            {
                byte[] imageDataCopy = new byte[texfile.ImageData.Length];
                texfile.ImageData.CopyTo(imageDataCopy, 0);
                switch (mode)
                {
                    case TextureMode.Lightshaft:
                        Util.SaveLightshaftAsBitmap(outputPath, imageDataCopy, texfile.Header.Width, texfile.Header.Height);
                        break;
                    case TextureMode.Normal:
                        Util.SaveNormalAsBitmap(outputPath, imageDataCopy, texfile.Header.Width, texfile.Header.Height);
                        break;
                    default:
                        Util.SaveAsBitmap(outputPath, imageDataCopy, texfile.Header.Width, texfile.Header.Height, color);
                        break;
                }
            } catch (NotSupportedException)
            {
                Console.WriteLine($"Format {texfile.Header.Format} not supported: {texfile.FilePath}");
            }
        }

        private TexFile? loadTexture(Texture tex)
        {
            TexFile? texFile = null;
            try
            {
                texFile = tex.GetTextureNc(data);
                if (texFile == null) throw new Exception("Texfile is null");
            } catch (Exception e)
            {
                Console.WriteLine($"Failed to get texture {tex.TexturePath}: {e.Message}");
            }
            return texFile;
        }
    }
}
