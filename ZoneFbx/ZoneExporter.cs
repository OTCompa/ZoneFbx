using Lumina;
using System;
using System.Numerics;
using ZoneFbx.Fbx;
using ZoneFbx.Processor;

namespace ZoneFbx
{
    internal partial class ZoneExporter : IDisposable
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

        private IntPtr contextManager;

        public ZoneExporter(string gamePath, string zonePath, string outputPath, Options options)
        {
            this.zonePath = zonePath.Trim();
            zoneCode = Path.GetFileName(this.zonePath);
            this.outputPath = Path.Combine(outputPath, zoneCode) + Path.DirectorySeparatorChar;
            this.options = options;

            Directory.CreateDirectory(this.outputPath);

            Console.WriteLine("Initializing...");

            // Construct GameData BEFORE allocating native resources so a failure here doesn't leak the context manager.
            try
            {
                data = new GameData(gamePath, new LuminaOptions() { PanicOnSheetChecksumMismatch = false });
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error: Game path directory is not valid!\n");
                throw new Exception("game path directory is not valid");
            }

            contextManager = ContextManager.Create();
            try
            {
                ContextManager.CreateManager(contextManager);
                ContextManager.CreateScene(contextManager, zoneCode);

                fbxExporter = new(contextManager);

                collisionProcessor = new(data, contextManager, options, this.zonePath);
                textureProcessor = new(data, contextManager, options, this.outputPath, zoneCode);
                materialProcessor = new(data, contextManager, options, textureProcessor, this.outputPath);
                modelProcessor = new(data, contextManager, options, materialProcessor);
                instanceObjectProcessor = new(data, contextManager, options, modelProcessor, collisionProcessor);
                layerProcessor = new(data, contextManager, options, instanceObjectProcessor, this.zonePath, this.outputPath);
                terrainProcessor = new(data, contextManager, options, modelProcessor, this.zonePath);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (contextManager != IntPtr.Zero)
            {
                ContextManager.DestroyManager(contextManager);
                ContextManager.Destroy(contextManager);
                contextManager = IntPtr.Zero;
            }
        }

        public void Export()
        {
            if (options.enableMainMap && !RunPass("Zone", null, null, exportZone)) return;
            if (options.enableCollisions && !RunPass("Collision", Mode.Collision, $"{zoneCode}_collision", exportCollision)) return;
            if (options.enableFestivals && !RunPass("Festival", Mode.Festival, $"{zoneCode}_festival", exportFestivals)) return;
        }

        // Drives one export pass. For the initial zone pass, mode/sceneName are null and the scene
        // doesn't need to be re-initialized. Subsequent passes start fresh by resetting caches and
        // replacing the FBX scene so we don't have to track three exports' worth of state at once.
        private bool RunPass(string passName, Mode? mode, string? sceneName, Func<bool> work)
        {
            if (mode.HasValue)
            {
                Console.WriteLine($"Beginning {passName.ToLower()} export...");
                options.mode = mode.Value;
                ReinitializeFbx(sceneName!);
            }

            if (!work())
            {
                Console.WriteLine("ZoneFbx has run into an error. Please open an issue on the GitHub repo with details about this error.");
                return false;
            }

            Console.WriteLine($"{passName} export finished.");
            return true;
        }

        private void ReinitializeFbx(string sceneName)
        {
            ContextManager.DestroyScene(contextManager);
            ContextManager.CreateScene(contextManager, sceneName);

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

        private bool exportFestivals()
        {
            Console.WriteLine("Processing lgb files...");
            if (!layerProcessor.ProcessLayerGroupBinaries(true))
            {
                Console.WriteLine("Failed to process lgbs for festivals.");
                return false;
            }

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
