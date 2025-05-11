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
        private readonly Flags flags;

        private readonly TextureProcessor textureProcessor;
        private readonly MaterialProcessor materialProcessor;
        private readonly ModelProcessor modelProcessor;
        private readonly InstanceObjectProcessor instanceObjectProcessor;
        private readonly LayerProcessor layerProcessor;
        private readonly TerrainProcessor terrainProcessor;
        private readonly FbxExporter fbxExporter;

        IntPtr manager;
        IntPtr scene;

        public ZoneExporter(string gamePath, string zonePath, string outputPath, Flags flags)
        {
            this.zonePath = zonePath;
            zoneCode = Path.GetFileName(zonePath);
            this.outputPath = Path.Combine(outputPath, zoneCode) + Path.DirectorySeparatorChar;
            this.flags = flags;

            Directory.CreateDirectory(this.outputPath);

            Console.WriteLine("Initializing...");

            manager = Manager.Create();
            scene = Scene.Create(manager, zoneCode);
            Scene.SetSystemUnit(scene);

            fbxExporter = new(manager, scene);

            try
            {
                data = new Lumina.GameData(gamePath);
            } catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error: Game path directory is not valid!\n");
                throw new Exception("game path directory is not valid");
            }

            textureProcessor = new(data, this.outputPath, zoneCode, scene);
            materialProcessor = new(data, textureProcessor, scene, flags, outputPath);
            modelProcessor = new(data, materialProcessor, manager, scene);
            instanceObjectProcessor = new(data, modelProcessor, scene, flags);
            layerProcessor = new(data, instanceObjectProcessor, scene, zonePath, this.outputPath, flags);
            terrainProcessor = new(data, modelProcessor, this.zonePath, manager, scene);

            Console.WriteLine("Processing models and textures...");
            Console.WriteLine("Processing zone terrain");
            if (!terrainProcessor.ProcessTerrain())
            {
                Console.WriteLine("Failed to process zone terrain.");
                return;
            }

            Console.WriteLine("Processing lgb files...");
            if (!layerProcessor.ProcessLayerGroupBinaries())
            {
                Console.WriteLine("Failed to process bg.lgb.");
                return;
            }

            Console.WriteLine("Saving scene...");
            if (!fbxExporter.Export($"{this.outputPath}{zoneCode}.fbx"))
            {
                Console.WriteLine("Failed to save scene.");
                return;
            }

            materialProcessor.ExportTextureJson();

            Console.WriteLine($"Done! Map exported to {Path.Combine(this.outputPath, $"{Path.GetFileName(zoneCode)}.fbx")}");

        }

        ~ZoneExporter()
        {
            if (manager != IntPtr.Zero) Manager.Destroy(manager);
        }
    }
}
