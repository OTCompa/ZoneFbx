using JeremyAnsel.BcnSharp;
using Lumina.Data.Files;
using Lumina.Models.Materials;
using System.Numerics;
using ZoneFbx.Fbx;

namespace ZoneFbx.Processor
{
    internal class TextureProcessor
    {
        private readonly Lumina.GameData data;
        private readonly string outputPath;
        private readonly string zoneCode;
        private readonly IntPtr scene;
        public TextureProcessor(Lumina.GameData data, string outputPath, string zoneCode, IntPtr scene)
        {
            this.data = data;
            this.outputPath = outputPath;
            this.zoneCode = zoneCode;
            this.scene = scene;
        }

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
                        color = materialInfo?.DiffuseColor;
                        break;
                    }

                    // if processing emissives instead
                    if (suffix.Equals("_e"))
                    {
                        color = materialInfo?.EmissiveColor;
                        if (color == null) return IntPtr.Zero;
                    }
                    if (suffix.Equals("_blend"))
                    {
                        color = materialInfo?.Diffuse2Color;
                        //if (materialInfo?.DiffuseColor != null && materialInfo?.DiffuseColor != Vector3.Zero) color *= materialInfo.DiffuseColor;
                        if (color == null) return IntPtr.Zero;
                    }
                    if (suffix.Equals("_e_blend"))
                    {
                        color = materialInfo?.Emissive2Color;
                        if (color == null) return IntPtr.Zero;
                    }
                    break;
                case Texture.Usage.Specular:
                    color = materialInfo?.SpecularColor; break;
            }

            filename = Util.GetTexturePath(outputPath, zoneCode, tex.TexturePath, material.MaterialPath, color, suffix);
            extractTexture(tex, color, filename);

            if (!string.IsNullOrEmpty(suffix) && !suffix.Equals("_e")) return IntPtr.Zero;
            return initializeFileTexture(tex.TexturePath, filename, suffix);
        }

        private IntPtr initializeFileTexture(string texfilePath, string textureOutputPath, string suffix = "")
        {
            string textureObjectName = $"{Path.GetFileNameWithoutExtension(texfilePath)}{suffix}";
            var texture = FileTexture.Create(scene, textureObjectName);
            FileTexture.SetStuff(texture, textureOutputPath);
            return texture;
        }

        private void extractTexture(Texture tex, Vector3? color, string outputPath)
        {
            if (File.Exists(outputPath)) return;

            TexFile? texfile = loadTexture(tex);
            if (texfile == null) return;

            try
            {
                byte[] imageDataCopy = new byte[texfile.ImageData.Length];
                texfile.ImageData.CopyTo(imageDataCopy, 0);
                Util.SaveAsBitmap(outputPath, imageDataCopy, texfile.Header.Width, texfile.Header.Height, color);
            } catch (NotSupportedException)
            {
                decodeTexture(texfile, outputPath, color, out var decodedData);
                Util.SaveAsBitmap(outputPath, decodedData, texfile.Header.Width, texfile.Header.Height, color);
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

        private void decodeTexture(TexFile texfile, string outputPath, Vector3? color, out byte[] decodedData)
        {
            decodedData = new byte[texfile.Header.Width * texfile.Header.Height * 4];
            switch (texfile.Header.Format)
            {
                case TexFile.TextureFormat.BC5:
                    Bc5Sharp.Decode(texfile.Data, decodedData, texfile.Header.Width, texfile.Header.Height);
                    break;
                case TexFile.TextureFormat.BC7:
                    Bc7Sharp.Decode(texfile.Data, decodedData, texfile.Header.Width, texfile.Header.Height);
                    break;
                default:
                    Console.WriteLine($"Format {texfile.Header.Format} not supported: {texfile.FilePath}");
                    return;
            }
        }
    }
}
