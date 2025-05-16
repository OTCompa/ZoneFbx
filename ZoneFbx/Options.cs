namespace ZoneFbx
{
    internal partial class ZoneExporter
    {
        public class Options
        {
            public bool enableLightshaftModels = false;
            public bool enableLighting = false;
            public bool enableFestivals = false;
            public bool enableJsonExport = false;
            public bool enableBlend = false;
            public bool enableMTMap = false;

            public bool disableBaking = false;

            public double specularFactor = 0.3;
            public double normalFactor = 0.2;
            public double lightIntensityFactor = 10000;
        }
    }
}
