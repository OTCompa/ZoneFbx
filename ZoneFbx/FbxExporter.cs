using ZoneFbx.Fbx;

namespace ZoneFbx
{
    internal class FbxExporter
    {
        private IntPtr scene;
        private IntPtr manager;

        public FbxExporter(IntPtr manager, IntPtr scene) {
            this.manager = manager;
            this.scene = scene;
        }

        public bool Export(string outputPathWithFilename)
        {
            var exporter = Exporter.Create(manager, "exporter");
            var out_fbx = $"{outputPathWithFilename}{(outputPathWithFilename.EndsWith(".fbx") ? "" : ".fbx")}";

            if (!Exporter.Initialize(exporter, out_fbx, manager))
            {
                return false;
            }
            var result = Exporter.Export(exporter, scene);

            Exporter.Destroy(exporter);
            return result;
        }
        
        public void UpdateScene(IntPtr scene)
        {
            this.scene = scene;
        }

        public void UpdateManager(IntPtr manager)
        {
            this.manager = manager;
        }
    }
}
