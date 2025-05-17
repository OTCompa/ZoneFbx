using JeremyAnsel.BcnSharp;
using Lumina.Data.Files;
using Lumina.Data.Parsing.Tex.Buffers;
using Lumina.Models.Materials;
using System.Numerics;
using ZoneFbx.Fbx;

namespace ZoneFbx.Processor
{
    internal class TextureProcessor(Lumina.GameData data, IntPtr manager, IntPtr scene, ZoneExporter.Options options, string outputPath, string zoneCode) : Processor(data, manager, scene, options)
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
            extractTexture(tex, color, filename);

            if (!string.IsNullOrEmpty(suffix) && !suffix.Equals("_e")) return IntPtr.Zero;
            return initializeFileTexture(tex.TexturePath, filename, suffix);
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
            var rawData = TextureBuffer.FromStream(texfile.Header, texfile.Reader).RawData;
            switch (texfile.Header.Format)
            {
                case TexFile.TextureFormat.BC5:
                    Bc5Sharp.Decode(rawData, decodedData, texfile.Header.Width, texfile.Header.Height);
                    break;
                case TexFile.TextureFormat.BC7:
                    Bc7Sharp.Decode(rawData, decodedData, texfile.Header.Width, texfile.Header.Height);
                    break;
                default:
                    Console.WriteLine($"Format {texfile.Header.Format} not supported: {texfile.FilePath}");
                    return;
            }
        }
    }
}
