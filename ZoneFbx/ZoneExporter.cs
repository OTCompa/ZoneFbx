using Lumina.Data.Files;
using System.Drawing;
using System.Drawing.Imaging;
using JeremyAnsel.BcnSharp;
using Lumina.Data.Parsing.Layer;
using Newtonsoft.Json;
using static Lumina.Data.Parsing.Layer.LayerCommon;
using Lumina.Models.Materials;
using Lumina.Extensions;
using System.Numerics;
using System;
using Lumina.Excel.Sheets;
using Lumina.Excel;
using ZoneFbx.Fbx;
using System.Drawing.Text;
using ZoneFbx.Processor;
using System.Xml.Linq;

namespace ZoneFbx
{
    internal partial class ZoneExporter
    {
        private readonly string zonePath;
        private readonly string outputPath;
        private readonly string zoneCode;
        private readonly Lumina.GameData data;
        private readonly Options options;

        private readonly CollisionProcessor collisionProcessor;
        private readonly TextureProcessor textureProcessor;
        private readonly MaterialProcessor materialProcessor;
        private readonly ModelProcessor modelProcessor;
        private readonly InstanceObjectProcessor instanceObjectProcessor;
        private readonly LayerProcessor layerProcessor;
        private readonly TerrainProcessor terrainProcessor;
        private readonly FbxExporter fbxExporter;

        private IntPtr contextManager { get; set; }

        private readonly List<Processor.Processor> processors = [];

        public ZoneExporter(string gamePath, string zonePath, string outputPath, Options options)
        {
            this.zonePath = zonePath;
            zoneCode = Path.GetFileName(zonePath);
            this.outputPath = Path.Combine(outputPath, zoneCode) + Path.DirectorySeparatorChar;
            this.options = options;

            Directory.CreateDirectory(this.outputPath);

            Console.WriteLine("Initializing...");

            contextManager = ContextManager.Create();
            ContextManager.CreateManager(contextManager);
            ContextManager.CreateScene(contextManager, zoneCode);

            fbxExporter = new(contextManager);

            try
            {
                data = new Lumina.GameData(gamePath);
            } catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error: Game path directory is not valid!\n");
                throw new Exception("game path directory is not valid");
            }

            collisionProcessor = new(data, contextManager, options, zonePath);
            textureProcessor = new(data, contextManager, options, this.outputPath, zoneCode);
            materialProcessor = new(data, contextManager, options, textureProcessor, this.outputPath);
            modelProcessor = new(data, contextManager, options, materialProcessor);
            instanceObjectProcessor = new(data, contextManager, options, modelProcessor, collisionProcessor);
            layerProcessor = new(data, contextManager, options, instanceObjectProcessor, zonePath, this.outputPath);
            terrainProcessor = new(data, contextManager, options, modelProcessor, this.zonePath);

            // 0 idea how to structure this program atp
            processors.AddRange([collisionProcessor, textureProcessor, materialProcessor, modelProcessor, instanceObjectProcessor, layerProcessor, terrainProcessor]);

            if (!exportZone())
            {
                Console.WriteLine("ZoneFbx has run into an error. Please open an issue on the GitHub repo with details about this error.");
                return;
            }
            Console.WriteLine("Zone export finished.");

            // starting different modes from scratch because keeping track of all 3 at the same time would
            // add complexity that i probably wouldn't be able to reasonably manage
            if (options.enableCollisions)
            {
                Console.WriteLine("Beginning collision export...");
                options.mode = Mode.Collision;
                ReinitializeFbx($"{zoneCode}_collision");
                if (!exportCollision())
                {
                    Console.WriteLine("ZoneFbx has run into an error. Please open an issue on the GitHub repo with details about this error.");
                    return;
                }
                Console.WriteLine("Collision export finished.");
            }

            if (options.enableFestivals)
            {
                Console.WriteLine("Beginning festival export...");
                options.mode = Mode.Festival;
                ReinitializeFbx($"{zoneCode}_festival");
                if (!exportFestivals())
                {
                    Console.WriteLine("ZoneFbx has run into an error. Please open an issue on the GitHub repo with details about this error.");
                    return;
                }
                Console.WriteLine("Festival export finished.");
            }
        }

        private void ReinitializeFbx(string sceneName)
        {
            ContextManager.DestroyScene(contextManager);
            ContextManager.CreateScene(contextManager, sceneName);

            foreach (var processor in processors)
            {
                processor.UpdateOptions(options);
            }

            modelProcessor.ResetCache();
            materialProcessor.ResetCache();
            instanceObjectProcessor.ResetCache();

        }

        private bool exportZone()
        {
            Console.WriteLine("Processing models and textures...");
            Console.WriteLine("Processing zone terrain");
            if (!terrainProcessor.ProcessTerrain())
            {
                Console.WriteLine("Failed to process zone terrain.");
                return false;
            }

            Console.WriteLine("Processing lgb files...");
            if (!layerProcessor.ProcessLayerGroupBinaries())
            {
                Console.WriteLine("Failed to process bg.lgb.");
                return false;
            }

            Console.WriteLine("Saving scene...");
            var outputFilePath = $"{this.outputPath}{zoneCode}.fbx";
            if (!fbxExporter.Export(outputFilePath))
            {
                Console.WriteLine("Failed to save scene.");
                return false;
            }

            if (options.enableJsonExport || options.enableMTMap) materialProcessor.ExportJsonTextureMap();

            Console.WriteLine($"Done! Map exported to {outputFilePath}");
            return true;
        }

        private bool exportCollision()
        {
            Console.WriteLine("Processing list.pcb...");
            collisionProcessor.ProcessList();

            Console.WriteLine("Processing lgb files...");
            if (!layerProcessor.ProcessLayerGroupBinaries(true))
            {
                Console.WriteLine("Failed to process bg.lgb.");
                return false;
            }

            Console.WriteLine("Saving scene...");
            var outputFilePath = $"{this.outputPath}{zoneCode}_collision.fbx";
            if (!fbxExporter.Export(outputFilePath))
            {
                Console.WriteLine("Failed to save scene.");
                return false;
            }

            Console.WriteLine($"Done! Collision models exported to {outputFilePath}");
            return true;
        }

        ~ZoneExporter()
        {
            if (contextManager != IntPtr.Zero)
            {
                ContextManager.DestroyManager(contextManager);
                ContextManager.Destroy(contextManager);
            }
        }

        private bool exportFestivals()
        {
            Console.WriteLine("Processing lgb files...");
            layerProcessor.ProcessLayerGroupBinaries(true);

            Console.WriteLine("Saving scene...");
            var outputFilePath = $"{this.outputPath}{zoneCode}_festival.fbx";
            if (!fbxExporter.Export(outputFilePath))
            {
                Console.WriteLine("Failed to save scene.");
                return false;
            }

            if (options.enableJsonExport || options.enableMTMap) materialProcessor.ExportJsonTextureMap();

            Console.WriteLine($"Done! Festival models exported to {outputFilePath}");
            return true;
        }
    }
}
