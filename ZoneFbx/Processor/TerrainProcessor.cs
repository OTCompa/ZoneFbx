using Lumina.Data.Files;
using ZoneFbx.Fbx;

namespace ZoneFbx.Processor
{
    internal class TerrainProcessor(Lumina.GameData data, IntPtr contextManager, ZoneExporter.Options options, ModelProcessor modelProcessor, string zone_path) : Processor(data, contextManager, options)
    {
        private readonly ModelProcessor modelProcessor = modelProcessor;
        private readonly string zonePath = zone_path;

        public bool ProcessTerrain()
        {
            string terrainPath = $"bg/{zonePath[..zonePath.LastIndexOf("/level")]}/bgplate/";
            string teraFilePath = $"{terrainPath}terrain.tera";
            if (!data.FileExists(teraFilePath)) return true;

            var terafile = data.GetFile<TeraFile>(teraFilePath);
            if (terafile == null) return false;

            var terrainNode = Node.Create(contextManager, "terrain");

            for (int i = 0; i < terafile?.PlateCount; i++)
            {
                string modelPath = Path.Combine(terrainPath, $"{i:D4}.mdl");
                var model = modelProcessor.LoadModel(modelPath);
                if (model == null) continue;

                var plateNode = Node.Create(contextManager, $"bgplate_{i}");
                var pos = terafile.GetPlatePosition(i);
                Node.SetLclTranslation(plateNode, pos.X, 0, pos.Y);

                modelProcessor.ProcessModel(model, plateNode);

                Node.AddChild(terrainNode, plateNode);
            }

            var rootNode = Scene.GetRootNode(contextManager);
            Node.AddChild(rootNode, terrainNode);

            return true;
        }
    }
}
