using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.GUI
{
    public class Config : ConfigurationSection
    {
        [ConfigurationProperty("gamePath", DefaultValue = "C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack")]
        public string GamePath
        {
            get => (string)this["gamePath"];
            set { this["gamePath"] = value; }
        }

        [ConfigurationProperty("outputPath", DefaultValue = "")]
        public string OutputPath
        {
            get => (string)this["outputPath"];
            set { this["outputPath"] = value; }
        }


        [ConfigurationProperty("enableLightshaft", DefaultValue = false)]
        public bool EnableLightshaft
        {
            get => (bool)this["enableLightshaft"];
            set { this["enableLightshaft"] = value; }
        }

        [ConfigurationProperty("enableLighting", DefaultValue = false)]
        public bool EnableLighting
        {
            get => (bool)this["enableLighting"];
            set { this["enableLighting"] = value; }
        }

        [ConfigurationProperty("enableFestival", DefaultValue = false)]
        public bool EnableFestival
        {
            get => (bool)this["enableFestival"];
            set { this["enableFestival"] = value; }
        }

        [ConfigurationProperty("enableJsonExport", DefaultValue = false)]
        public bool EnableJsonExport
        {
            get => (bool)this["enableJsonExport"];
            set { this["enableJsonExport"] = value; }
        }

        [ConfigurationProperty("enableBlend", DefaultValue = false)]
        public bool EnableBlend
        {
            get => (bool)this["enableBlend"];
            set { this["enableBlend"] = value; }
        }

        [ConfigurationProperty("enableMTMap", DefaultValue = false)]
        public bool EnableMTMap
        {
            get => (bool)this["enableMTMap"];
            set { this["enableMTMap"] = value; }
        }

        [ConfigurationProperty("enableCollision", DefaultValue = false)]
        public bool EnableCollision
        {
            get => (bool)this["enableCollision"];
            set { this["enableCollision"] = value; }
        }

        [ConfigurationProperty("disableBaking", DefaultValue = false)]
        public bool DisableBaking
        {
            get => (bool)this["disableBaking"];
            set { this["disableBaking"] = value; }
        }


        [ConfigurationProperty("specularFactor", DefaultValue = "0.3")]
        public string SpecularFactor
        {
            get => (string)this["specularFactor"];
            set { this["specularFactor"] = value; }
        }

        [ConfigurationProperty("normalFactor", DefaultValue = "0.2")]
        public string NormalFactor
        {
            get => (string)this["normalFactor"];
            set { this["normalFactor"] = value.ToString(); }
        }

        [ConfigurationProperty("lightIntensityFactor", DefaultValue = "10000")]
        public string LightIntensityFactor
        {
            get => (string)this["lightIntensityFactor"];
            set { this["lightIntensityFactor"] = value.ToString(); }
        }
    }
}
