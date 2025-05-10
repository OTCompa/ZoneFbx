using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoneFbx.Fbx;

namespace ZoneFbx
{
    internal class FbxExporter
    {
        private IntPtr scene;
        private IntPtr manager;

        public FbxExporter(string name)
        {
            manager = Manager.Create();
            scene = Scene.Create(manager, name);
            Scene.SetSystemUnit(scene);
        }

        ~FbxExporter()
        {
            if (manager != IntPtr.Zero) Manager.Destroy(manager);
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
        public IntPtr GetScene() => scene;
        public IntPtr GetManager() => manager;

    }
}
