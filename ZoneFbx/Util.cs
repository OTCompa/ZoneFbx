using Lumina.Data.Parsing.Layer;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using ZoneFbx.Fbx;
using static Lumina.Data.Parsing.Layer.LayerCommon;

namespace ZoneFbx
{
    internal class Util
    {
        public static void EnsureExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public static string EnsureExists(string path, string subpath)
        {
            var resPath = Path.Combine(path, subpath);
            if (!Directory.Exists(resPath)) Directory.CreateDirectory(resPath);
            return resPath;
        }

        public static string GetTextureFolder(string out_path, string zone_code)
        {
            return out_path + "textures" + Path.DirectorySeparatorChar;
        }

        public static string GetTexturePath(string out_path, string zone_code, string texture_path, string material_path, Vector3? v = null)
        {
            string tex_abs_path;

            if (v == null)
            {
                tex_abs_path = texture_path.Substring(texture_path.LastIndexOf('/') + 1).Replace(".tex", ".png");
            } else
            {
                tex_abs_path = $"{Path.GetFileName(material_path)}_{Path.GetFileNameWithoutExtension(texture_path)}.png";
            }

            tex_abs_path = GetTextureFolder(out_path, zone_code) + tex_abs_path;
            return tex_abs_path;
        }

        public static double Degrees(double radians)
        {
            return 180 / Math.PI * radians;
        }

        public static void SaveAsBitmap(string tex_path, byte[] data, int width, int height, Vector3? multiplier = null)
        {
            if (multiplier != null)
            {
                for (int i = 0; i < data.Length; i += 4)
                {
                    data[i] = Convert.ToByte(Math.Clamp(data[i] * multiplier.Value.Z, 0, 255));         // b
                    data[i + 1] = Convert.ToByte(Math.Clamp(data[i + 1] * multiplier.Value.Y, 0, 255)); // g
                    data[i + 2] = Convert.ToByte(Math.Clamp(data[i + 2] * multiplier.Value.X, 0, 255)); // r
                }
            }

            unsafe
            {
                fixed (byte* p = data)
                {
                    IntPtr imageData = (IntPtr)p;
                    var texture = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, imageData);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(tex_path)!);
                    texture.Save(tex_path, ImageFormat.Png);
                }
            }
        }

        public static void SaveJson(string filename, LayerCommon.Layer[] layers, string output_path)
        {
            var layerJson = JsonConvert.SerializeObject(layers, Formatting.Indented);
            var jsonFolder = Path.Combine(output_path, "json");
            Directory.CreateDirectory(jsonFolder);
            var filepath = Path.Combine(jsonFolder, $"{filename}.json");
            if (!File.Exists(filepath))
            {
                File.WriteAllText(Path.Combine(jsonFolder, $"{filename}.json"), layerJson);
            }
        }

        public static void InitChildNode(InstanceObject obj, IntPtr node)
        {
            Node.SetStuff(node, obj.Transform.Translation.X, obj.Transform.Translation.Y, obj.Transform.Translation.Z, 0);
            if (obj.AssetType == LayerEntryType.LayLight)
            {
                // rotate light nodes -90 degrees on the X axis since the light nodes point towards its negative Y axis
                Node.SetStuff(node, Util.Degrees(obj.Transform.Rotation.X) - 90, Util.Degrees(obj.Transform.Rotation.Y), Util.Degrees(obj.Transform.Rotation.Z), 1);
            } else
            {
                Node.SetStuff(node, Util.Degrees(obj.Transform.Rotation.X), Util.Degrees(obj.Transform.Rotation.Y), Util.Degrees(obj.Transform.Rotation.Z), 1);
            }
            Node.SetStuff(node, obj.Transform.Scale.X, obj.Transform.Scale.Y, obj.Transform.Scale.Z, 2);
        }

        public static string TrimLevelPath(string zonePath) => zonePath.Substring(0, zonePath.LastIndexOf("/"));
    }
}
