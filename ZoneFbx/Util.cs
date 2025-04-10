﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return out_path + zone_code + "_textures\\";
        }

        public static string get_texture_path(string out_path, string zone_code, string texture_path)
        {
            var tex_abs_path = texture_path.Substring(texture_path.LastIndexOf('/') + 1).Replace(".tex", ".png");
            tex_abs_path = get_texture_folder(out_path, zone_code) + tex_abs_path;
            return tex_abs_path;
        }

        public static double degrees(double radians)
        {
            return 180 / Math.PI * radians;
        }
    }
}
