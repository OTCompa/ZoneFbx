using Lumina;

namespace ZoneFbx.Processor
{
    internal class Processor(GameData data, IntPtr contextManager, ZoneExporter.Options options)
    {
        internal GameData data { get; private set; } = data;
        internal IntPtr contextManager { get; private set; } = contextManager;

        internal ZoneExporter.Options options { get; private set; } = options;

        public void UpdateOptions(ZoneExporter.Options options)
        {
            this.options = options;
        }
    }
}
