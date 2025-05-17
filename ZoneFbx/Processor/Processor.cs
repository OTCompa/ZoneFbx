using Lumina;

namespace ZoneFbx.Processor
{
    internal class Processor(GameData data, IntPtr manager, IntPtr scene, ZoneExporter.Options options)
    {
        internal GameData data { get; private set; } = data;
        internal IntPtr scene { get; private set; } = scene;
        internal IntPtr manager { get; private set; } = manager;

        internal ZoneExporter.Options options { get; private set; } = options;

        public void UpdateScene(IntPtr scene)
        {
            this.scene = scene;
        }

        public void UpdateManager(IntPtr manager)
        {
            this.manager = manager;
        }

        public void UpdateOptions(ZoneExporter.Options options)
        {
            this.options = options;
        }
    }
}
