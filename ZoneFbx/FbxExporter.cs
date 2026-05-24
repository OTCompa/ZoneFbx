using ZoneFbx.Fbx;

namespace ZoneFbx
{
    internal class FbxExporter
    {
        private IntPtr contextManager;

        public FbxExporter(IntPtr contextManager) {
            this.contextManager = contextManager;
        }

        public bool Export(string outputPathWithFilename)
        {
            var exporter = Exporter.Create(contextManager, "exporter");
            try
            {
                var out_fbx = $"{outputPathWithFilename}{(outputPathWithFilename.EndsWith(".fbx") ? "" : ".fbx")}";

                if (!Exporter.Initialize(exporter, out_fbx, contextManager))
                {
                    return false;
                }
                return Exporter.Export(exporter, contextManager);
            }
            finally
            {
                Exporter.Destroy(exporter);
            }
        }
    }
}
