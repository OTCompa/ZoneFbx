using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ZoneFbx.Fbx;

namespace ZoneFbx.Processor
{
    internal class TerrainProcessor
    {
        private readonly Lumina.GameData data;
        private readonly ModelProcessor modelProcessor;
        private readonly string zonePath;
        private readonly IntPtr scene;
        private readonly IntPtr manager;
        public TerrainProcessor(Lumina.GameData data, ModelProcessor modelProcessor, string zone_path, IntPtr manager, IntPtr scene) {
            this.data = data;
            this.modelProcessor = modelProcessor;
            this.zonePath = zone_path;
            this.scene = scene;
            this.manager = manager;
        }

        public bool ProcessTerrain()
        {
            string terrainPath = $"bg/{zonePath[..zonePath.LastIndexOf("/level")]}/bgplate/";
            string teraFilePath = $"{terrainPath}terrain.tera";
            if (!data.FileExists(teraFilePath)) return true;

            var terafile = data.GetFile<TeraFile>(teraFilePath);
            if (terafile == null) return false;

            var terrainNode = Node.Create(manager, "terrain");

            for (int i = 0; i < terafile?.PlateCount; i++)
            {
                string modelPath = Path.Combine(terrainPath, $"{i:D4}.mdl");
                var model = modelProcessor.LoadModel(modelPath);
                if (model == null) continue;

                var plateNode = Node.Create(manager, $"bgplate_{i}");
                var pos = terafile.GetPlatePosition(i);
                Node.SetLclTranslation(plateNode, pos.X, 0, pos.Y);

                modelProcessor.ProcessModel(model, plateNode);

                Node.AddChild(terrainNode, plateNode);
            }

            var rootNode = Scene.GetRootNode(scene);
            Node.AddChild(rootNode, terrainNode);

            return true;
        }
    }
}
