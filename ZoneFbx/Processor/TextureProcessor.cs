using JeremyAnsel.BcnSharp;
using Lumina.Data.Files;
using Lumina.Models.Materials;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
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

        public IntPtr PrepareTexture(Material material, Texture tex, MaterialInfo? materialInfo, string suffix = "")
        {
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
                    color = materialInfo?.EmissiveColor;
                    if (color == null) return IntPtr.Zero;
                    break;
                case Texture.Usage.Specular:
                    color = materialInfo?.SpecularColor; break;
            }

            var textureOutputPath = Util.get_texture_path(outputPath, zoneCode, tex.TexturePath, material.MaterialPath, color, type: suffix);
            ExtractTexture(tex, color, textureOutputPath);
            return InitializeFileTexture(tex.TexturePath, textureOutputPath, suffix);
        }

        private IntPtr InitializeFileTexture(string texfilePath, string textureOutputPath, string suffix = "")
        {
            string textureObjectName = $"{Path.GetFileNameWithoutExtension(texfilePath)}{suffix}";
            var texture = FileTexture.Create(scene, textureObjectName);
            FileTexture.SetStuff(texture, textureOutputPath);
            return texture;
        }

        private void ExtractTexture(Texture tex, Vector3? color, string outputPath)
        {
            if (File.Exists(outputPath)) return;

            TexFile? texfile = LoadTexture(tex);
            if (texfile == null) return;

            try
            {
                SaveTexture(texfile, outputPath, color);
            } catch (NotSupportedException)
            {
                DecodeAndSaveTexture(texfile, outputPath, color);
            }
        }

        private TexFile? LoadTexture(Texture tex)
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

        private void SaveTexture(TexFile texfile, string outputPath, Vector3? color)
        {
            byte[] imageDataCopy = new byte[texfile.ImageData.Length];
            texfile.ImageData.CopyTo(imageDataCopy, 0);
            Util.SaveAsBitmap(outputPath, imageDataCopy, texfile.Header.Width, texfile.Header.Height, color);
        }

        private void DecodeAndSaveTexture(TexFile texfile, string outputPath, Vector3? color)
        {
            var decodedData = new byte[texfile.Header.Width * texfile.Header.Height * 4];

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

            Util.SaveAsBitmap(outputPath, decodedData, texfile.Header.Width, texfile.Header.Height, color);
        }
    }
}
