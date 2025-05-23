﻿using Lumina.Data.Files;
using ZoneFbx.Fbx;
using Lumina.Data.Parsing.Layer;
using static Lumina.Data.Parsing.Layer.LayerCommon;
using Lumina.Excel.Sheets;
using Lumina.Excel;

namespace ZoneFbx.Processor
{
    internal class LayerProcessor : Processor
    {
        private readonly InstanceObjectProcessor instanceObjectProcessor;
        private readonly string zone_path;
        private readonly string output_path;

        private readonly ExcelSheet<EObj> EObjSheet;

        public LayerProcessor(Lumina.GameData data, IntPtr contextManager, ZoneExporter.Options options, InstanceObjectProcessor instanceObjectProcessor, string zone_path, string output_path) : base(data, contextManager, options)
        {
            this.instanceObjectProcessor = instanceObjectProcessor;
            this.zone_path = zone_path;
            this.output_path = output_path;

            try
            {
                EObjSheet = data.GetExcelSheet<EObj>()!;
            } catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error: Unable to get EObj sheet!\n");
                throw new Exception("unable to get EObj sheet");
            }
        }

        public bool ProcessLayerGroupBinaries(bool canIgnoreBg = false)
        {
            string[] lgbs = { "bg", "planevent", "planlive", "planmap" }; //, "planner"};

            foreach (var lgbName in lgbs)
            {
                var path = GetLgbPath(lgbName, zone_path);
                if (!ProcessLayerGroupBinary(path))
                {
                    Console.WriteLine($"LGB \"{lgbName}.lgb\" skipped.");
                    if (!canIgnoreBg && lgbName == "bg") return false;
                }
            }

            return true;
        }

        public static string GetLgbPath(string lgbName, string zonePath) => $"bg/{Util.TrimLevelPath(zonePath)}/{lgbName}.lgb";

        public bool ProcessLayerGroupBinary(string lgbPath)
        {
            var file = data.GetFile<LgbFile>(lgbPath);
            if (file == null) return false;

            var root_node = Scene.GetRootNode(contextManager);

            var ret = processLayers(file.Layers, root_node);

            if (options.enableJsonExport)
            {
                Util.SaveJson(Path.GetFileNameWithoutExtension(lgbPath), file.Layers, output_path);
            }

            return ret;
        }

        private IntPtr processInstanceObject(LayerCommon.InstanceObject obj)
        {
            string sgbPath;
            IntPtr objNode;

            switch (obj.AssetType)
            {
                case LayerEntryType.SharedGroup:
                    var sharedGroupObj = (SharedGroupInstanceObject)obj.Object;
                    sgbPath = sharedGroupObj.AssetPath;

                    objNode = Node.Create(contextManager, Path.GetFileNameWithoutExtension(sgbPath));
                    Util.InitChildNode(obj, objNode);

                    if (processSharedGroupBinary(sgbPath, objNode)) return objNode;

                    break;
                case LayerEntryType.EventObject:
                    var eventObj = (EventInstanceObject)obj.Object;
                    if (!EObjSheet.TryGetRow(eventObj.ParentData.BaseId, out var row)) return IntPtr.Zero;
                    if (row.SgbPath.ValueNullable == null) return IntPtr.Zero;
                    sgbPath = row.SgbPath.Value.SgbPath.ToString();
                    if (!sgbPath.EndsWith("sgb")) return IntPtr.Zero;  // 1 more sanity check

                    objNode = Node.Create(contextManager, Path.GetFileName(sgbPath));
                    Util.InitChildNode(obj, objNode);

                    if (processSharedGroupBinary(sgbPath, objNode)) return objNode;

                    break;
                case LayerEntryType.LayLight:
                    if (options.mode == ZoneExporter.Mode.Default)
                        return instanceObjectProcessor.ProcessInstanceObjectLayLight(obj);
                    break;
                case LayerEntryType.BG:
                    return instanceObjectProcessor.ProcessInstanceObjectBG(obj);
            }
            return IntPtr.Zero;
        }

        private bool processSharedGroupBinary(string sgbPath, IntPtr parentNode)
        {
            var hasChild = false;
            var sgb = data.GetFile<SgbFile>(sgbPath);
            if (sgb == null) return false;

            for (int i = 0; i < sgb.LayerGroups.Length; i++)
            {
                var layerGroup = sgb.LayerGroups[i];
                var layerGroupNode = Node.Create(contextManager, $"LayerGroup{i}");  // this is probably redundant, i've only seen sgbs with 1 layer group

                if (processLayers(layerGroup.Layers, layerGroupNode))
                {
                    Node.AddChild(parentNode, layerGroupNode);
                    hasChild = true;
                }

                if (options.enableJsonExport)
                {
                    Util.SaveJson(Path.GetFileNameWithoutExtension(sgbPath), layerGroup.Layers, output_path);
                }

            }
            return hasChild;
        }

        private bool processLayers(LayerCommon.Layer[] layers, IntPtr parentNode)
        {
            var groupHasChild = false;
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];

                if (options.mode == ZoneExporter.Mode.Festival && layer.FestivalID == 0) continue;
                if (options.mode != ZoneExporter.Mode.Festival && layer.FestivalID != 0)
                {
                    if (!options.enableFestivals) Console.WriteLine($"Skipping festival {layer.FestivalID}");
                    continue;
                }

                var layerNode = Node.Create(contextManager, layer.Name);
                var layerHasChild = false;

                for (int j = 0; j < layer.InstanceObjects.Length; j++)
                {
                    var childNode = processInstanceObject(layer.InstanceObjects[j]);
                    if (childNode != IntPtr.Zero)
                    {
                        Node.AddChild(layerNode, childNode);
                        layerHasChild = true;
                    }
                }

                if (layerHasChild)
                {
                    Node.AddChild(parentNode, layerNode);
                    groupHasChild = true;
                }
            }
            return groupHasChild;
        }
    }
}
