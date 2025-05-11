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

namespace ZoneFbx
{
    internal partial class ZoneExporter
    {
        private readonly string zone_path;
        private readonly string output_path;
        private readonly string zone_code;
        private readonly Lumina.GameData data;
        private readonly ExcelSheet<EObj> EObjSheet;
        private readonly Flags flags;

        private readonly TextureProcessor textureProcessor;
        private readonly MaterialProcessor materialProcessor;
        private readonly ModelProcessor modelProcessor;
        private readonly InstanceObjectProcessor instanceObjectProcessor;
        private readonly LayerProcessor layerProcessor;
        private readonly FbxExporter fbxExporter;

        IntPtr manager;
        IntPtr scene;

        public ZoneExporter(string game_path, string zone_path, string output_path, Flags flags)
        {
            this.zone_path = zone_path;

            zone_code = zone_path.Substring(zone_path.LastIndexOf("/level") - 4, 4);

            this.output_path = Path.Combine(output_path, zone_code) + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(this.output_path);

            this.flags = flags;

            Console.WriteLine("Initializing...");
            this.fbxExporter = new(zone_code);
            manager = fbxExporter.GetManager();
            scene = fbxExporter.GetScene();
            try
            {
                data = new Lumina.GameData(game_path);
            } catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error: Game path directory is not valid!\n");
                throw new Exception("game path directory is not valid");
            }

            try
            {
                EObjSheet = data.GetExcelSheet<EObj>()!;
            } catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error: Unable to get EObj sheet!\n");
                throw new Exception("unable to get EObj sheet");
            }

            this.textureProcessor = new(data, this.output_path, zone_code, fbxExporter.GetScene());
            this.materialProcessor = new(data, textureProcessor, fbxExporter.GetScene(), flags);
            this.modelProcessor = new(materialProcessor, fbxExporter.GetManager(), fbxExporter.GetScene());
            this.instanceObjectProcessor = new(data, modelProcessor, scene, flags);
            this.layerProcessor = new(data, instanceObjectProcessor, scene, zone_path, this.output_path, flags);

            Console.WriteLine("Processing models and textures...");
            Console.WriteLine("Processing zone terrain");
            if (!process_terrain())
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
            if (!fbxExporter.Export($"{this.output_path}{zone_code}.fbx"))
            {
                Console.WriteLine("Failed to save scene.");
                return;
            }

            Console.WriteLine($"Done! Map exported to {Path.Combine(output_path, zone_code, $"{Path.GetFileName(zone_code)}.fbx")}");

        }

        private bool process_terrain()
        {
            string terrain_path = zone_path[..zone_path.LastIndexOf("/level")];
            terrain_path = "bg/" + terrain_path + "/bgplate/";
            string filename = "terrain.tera";
            string terafile_path = terrain_path + filename;

            if (!data.FileExists(terafile_path)) return true;
            var terafile = data.GetFile<TeraFile>(terafile_path);
            if (terafile == null) return false;

            var terrain_node = Node.Create(manager, "terrain");

            for (int i = 0; i < terafile?.PlateCount; i++)
            {
                var plate_node = Node.Create(manager, "bgplate_" + i.ToString());
                var pos = terafile.GetPlatePosition(i);
                Node.SetLclTranslation(plate_node, pos.X, 0, pos.Y);

                string model_filename = string.Format("{0:D4}.mdl", i);
                string model_path = Path.Combine(terrain_path, model_filename);
                var plate_model_file = data.GetFile<MdlFile>(model_path);
                if (plate_model_file == null) { continue; }

                var plate_model = new Lumina.Models.Models.Model(plate_model_file!);
                try
                {
                    plate_model.Update(data);
                } catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine("Object " + model_path + " could not be resolved from game data.");
                    Console.WriteLine(e.Message);
                    plate_model = new Lumina.Models.Models.Model(plate_model_file!);
                }


                modelProcessor.ProcessModel(plate_model, plate_node);

                Node.AddChild(terrain_node, plate_node);
            }

            var rootNode = Scene.GetRootNode(scene);
            Node.AddChild(rootNode, terrain_node);

            return true;
        }

        ~ZoneExporter()
        {
            if (manager != IntPtr.Zero) Manager.Destroy(manager);
        }
    }
}
