using Lumina.Data.Files;
using Lumina.Models.Materials;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
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

        public static string get_texture_folder(string out_path, string zone_code)
        {
            return out_path + "textures" + Path.DirectorySeparatorChar;
        }

        public static string get_texture_path(string out_path, string zone_code, string texture_path, string material_path, Vector3? v = null, string type = "")
        {
            string tex_abs_path;

            if (v == null)
            {
                tex_abs_path = texture_path.Substring(texture_path.LastIndexOf('/') + 1).Replace(".tex", ".png");
            } else
            {
                if (type.Length == 0) type = texture_path.Substring(texture_path.LastIndexOf(".tex") - 2, 2);
                tex_abs_path = material_path.Substring(material_path.LastIndexOf('/') + 1).Replace(".mtrl", $"{type}.png");
            }

            tex_abs_path = get_texture_folder(out_path, zone_code) + tex_abs_path;
            return tex_abs_path;
        }

        public static double degrees(double radians)
        {
            return 180 / Math.PI * radians;
        }

        public static Bitmap toBitmap(byte[] data, int width, int height, Vector3? multiplier = null)
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
                    return new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, imageData);
                }
            }
        }

        public static void save_json(string filename, Layer[] layers, string output_path)
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
    }
}
