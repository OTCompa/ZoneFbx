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
        private readonly FbxExporter fbxExporter;
        IntPtr manager;
        IntPtr scene;

        private readonly Dictionary<ulong, IntPtr> material_cache = [];
        private readonly Dictionary<string, IntPtr> mesh_cache = [];

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

            Console.WriteLine("Processing models and textures...");
            Console.WriteLine("Processing zone terrain");
            if (!process_terrain())
            {
                Console.WriteLine("Failed to process zone terrain.");
                return;
            }

            Console.WriteLine("Processing lgb files...");
            if (!process_lgbs())
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

                process_model(plate_model, plate_node);

                Node.AddChild(terrain_node, plate_node);
            }

            var rootNode = Scene.GetRootNode(scene);
            Node.AddChild(rootNode, terrain_node);

            return true;
        }

        private bool process_model(Lumina.Models.Models.Model model, IntPtr node)
        {
            var has_children = false;
            var path = model!.File!.FilePath.Path;
            path = path.Substring(path.LastIndexOf('/') + 1);
            for (int i = 0; i < model.Meshes.Length; i++)
            {
                string mesh_name = path + "_" + i.ToString();
                var mesh_node = Node.Create(manager, mesh_name);

                var result = mesh_cache.TryGetValue(mesh_name, out var cached_mesh);
                IntPtr mesh;
                if (result)
                {
                    mesh = cached_mesh;
                } else
                {
                    mesh = create_mesh(model.Meshes[i], mesh_name);
                    IntPtr material = materialProcessor.CreateMaterial(model.Meshes[i].Material);
                    if (material == IntPtr.Zero) continue;

                    Node.AddMaterial(mesh_node, material);
                    mesh_cache[mesh_name] = mesh;
                }
                Node.SetNodeAttribute(mesh_node, mesh);
                Node.AddChild(node, mesh_node);
                has_children = true;
            }
            return has_children;
        }

        private IntPtr create_mesh(Lumina.Models.Models.Mesh game_mesh, string mesh_name)
        {
            var mesh = Mesh.Create(scene, mesh_name);
            Mesh.Init(mesh, game_mesh.Vertices.Length);

            var colorElement = GeometryElement.VertexColor.Create(mesh);
            GeometryElement.VertexColor.SetMappingNode(colorElement);

            var uvElement1 = GeometryElement.UV.Create(mesh, "uv1");
            GeometryElement.UV.SetMappingNode(uvElement1);

            var uvElement2 = GeometryElement.UV.Create(mesh, "uv2");
            GeometryElement.UV.SetMappingNode(uvElement2);

            var tangentElem1 = GeometryElement.Tangent.Create(mesh);
            GeometryElement.Tangent.SetMappingNode(tangentElem1);

            var tangentElem2 = GeometryElement.Tangent.Create(mesh);
            GeometryElement.Tangent.SetMappingNode(tangentElem2);

            for (int i = 0; i < game_mesh.Vertices.Length; i++)
            {
                IntPtr pos = IntPtr.Zero, norm = IntPtr.Zero, uv = IntPtr.Zero, tangent1 = IntPtr.Zero, tangent2 = IntPtr.Zero, color = IntPtr.Zero;


                if (game_mesh.Vertices[i].Position.HasValue)
                {
                    pos = Fbx.Vector4.Create(game_mesh.Vertices[i].Position!.Value.X,
                             game_mesh.Vertices[i].Position!.Value.Y,
                             game_mesh.Vertices[i].Position!.Value.Z,
                             game_mesh.Vertices[i].Position!.Value.W);
                }

                if (game_mesh.Vertices[i].Normal.HasValue)
                {
                    norm = Fbx.Vector4.Create(game_mesh.Vertices[i].Normal!.Value.X,
                              game_mesh.Vertices[i].Normal!.Value.Y,
                              game_mesh.Vertices[i].Normal!.Value.Z,
                              0);
                }

                if (game_mesh.Vertices[i].Color.HasValue)
                {
                    color = Fbx.Vector4.Create(game_mesh.Vertices[i].Color!.Value.X,
                             game_mesh.Vertices[i].Color!.Value.Y,
                             game_mesh.Vertices[i].Color!.Value.Z,
                             game_mesh.Vertices[i].Color!.Value.W);
                }

                if (game_mesh.Vertices[i].Tangent1.HasValue)
                {
                    tangent1 = Fbx.Vector4.Create(game_mesh.Vertices[i].Tangent1!.Value.X,
                              game_mesh.Vertices[i].Tangent1!.Value.Y,
                              game_mesh.Vertices[i].Tangent1!.Value.Z,
                              0);
                }

                if (game_mesh.Vertices[i].Tangent2.HasValue)
                {
                    tangent2 = Fbx.Vector4.Create(game_mesh.Vertices[i].Tangent2!.Value.X,
                             game_mesh.Vertices[i].Tangent2!.Value.Y,
                             game_mesh.Vertices[i].Tangent2!.Value.Z,
                             game_mesh.Vertices[i].Tangent2!.Value.W);
                }

                if (pos != IntPtr.Zero && norm != IntPtr.Zero)
                {
                    Mesh.SetControlPointAt(mesh, pos, norm, i);
                }

                if (game_mesh.Vertices[i].UV.HasValue)
                {
                    var uv1Array = GeometryElement.UV.GetDirectArray(uvElement1);
                    Fbx.Layer.UV_Add(uv1Array, Fbx.Vector2.Create(game_mesh.Vertices[i].UV!.Value.X, game_mesh.Vertices[i].UV!.Value.Y * -1));
                    var uv2Array = GeometryElement.UV.GetDirectArray(uvElement2);
                    Fbx.Layer.UV_Add(uv2Array, Fbx.Vector2.Create(game_mesh.Vertices[i].UV!.Value.Z, game_mesh.Vertices[i].UV!.Value.W * -1));
                }

                var colorArray = GeometryElement.VertexColor.GetDirectArray(colorElement);
                Fbx.Layer.Color_Add(colorArray, color);

                var tangent1Array = GeometryElement.Tangent.GetDirectArray(tangentElem1);
                Fbx.Layer.Tangent_Add(tangent1Array, tangent1);
                var tangent2Array = GeometryElement.Tangent.GetDirectArray(tangentElem2);
                Fbx.Layer.Tangent_Add(tangent2Array, tangent2);
            }

            for (int i = 0; i < game_mesh.Indices.Length; i+= 3)
            {
                Mesh.BeginPolygon(mesh);
                Mesh.AddPolygon(mesh, game_mesh.Indices[i]);
                Mesh.AddPolygon(mesh, game_mesh.Indices[i+1]);
                Mesh.AddPolygon(mesh, game_mesh.Indices[i+2]);
                Mesh.EndPolygon(mesh);
            }

            return mesh;
        }

        private void init_child_node(InstanceObject obj, IntPtr node)
        {
            Node.SetStuff(node, obj.Transform.Translation.X, obj.Transform.Translation.Y, obj.Transform.Translation.Z, 0);
            if (obj.AssetType == LayerEntryType.LayLight)
            {
                // rotate light nodes -90 degrees on the X axis since the light nodes point towards its negative Y axis
                Node.SetStuff(node, Util.degrees(obj.Transform.Rotation.X) - 90, Util.degrees(obj.Transform.Rotation.Y), Util.degrees(obj.Transform.Rotation.Z), 1);
            } else
            {
                Node.SetStuff(node, Util.degrees(obj.Transform.Rotation.X), Util.degrees(obj.Transform.Rotation.Y), Util.degrees(obj.Transform.Rotation.Z), 1);
            }
            Node.SetStuff(node, obj.Transform.Scale.X, obj.Transform.Scale.Y, obj.Transform.Scale.Z, 2);
        }

        private bool process_layers(LayerCommon.Layer[] layers, IntPtr parentNode)
        {
            var group_has_child = false;
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (!flags.enableFestivals && layer.FestivalID != 0)
                {
                    Console.WriteLine($"Skipping festival {layer.FestivalID}");
                    continue;
                }

                var layer_node = Node.Create(scene, layer.Name);
                var layer_has_child = false;

                for (int j = 0; j < layer.InstanceObjects.Length; j++)
                {
                    var childNode = process_asset(layer.InstanceObjects[j]);
                    if (childNode != IntPtr.Zero)
                    {
                        Node.AddChild(layer_node, childNode);
                        layer_has_child = true;
                    }
                }

                if (layer_has_child)
                {
                    Node.AddChild(parentNode, layer_node);
                    group_has_child = true;
                }
            }
            return group_has_child;
        }

        private IntPtr process_asset(InstanceObject obj)
        {
            IntPtr obj_node;
            string sgb_path;
            switch (obj.AssetType)
            {
                case LayerEntryType.BG:
                    var instance_object = (BGInstanceObject)obj.Object;
                    var object_path = instance_object.AssetPath;
                    var object_file = data.GetFile<MdlFile>(object_path);
                    if (object_file == null)
                    {
                        Console.WriteLine($"Unable to get {object_path} from game data.");
                        return IntPtr.Zero;
                    }
                    var model = new Lumina.Models.Models.Model(object_file);
                    try
                    {
                        model.Update(data);
                    } catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("Object " + object_path + " could not be resolved from game data.");
                        Console.WriteLine(e.Message);
                        model = new Lumina.Models.Models.Model(object_file);  
                        // this should still create a mesh without a material (?)
                    }

                    var model_node = Node.Create(scene, object_path.Substring(object_path.LastIndexOf("/") + 1));
                    init_child_node(obj, model_node);

                    if (process_model(model, model_node))
                    {
                        return model_node;
                    }
                    break;
                case LayerEntryType.SharedGroup:
                    var sharedGroupObj = (SharedGroupInstanceObject)obj.Object;
                    sgb_path = sharedGroupObj.AssetPath;

                    obj_node = Node.Create(scene, sgb_path.Substring(sgb_path.LastIndexOf("/") + 1));
                    init_child_node(obj, obj_node);

                    if (process_sgb(sgb_path, obj_node))
                    {
                        return obj_node;
                    }
                    break;
                case LayerEntryType.LayLight:
                    if (!flags.enableLighting) return IntPtr.Zero;

                    obj_node = Node.Create(scene, $"light_{obj.InstanceId}");
                    init_child_node(obj, obj_node);

                    var lightObj = (LightInstanceObject)obj.Object;
                    if (lightObj.DiffuseColorHDRI.Intensity == 0.0) return IntPtr.Zero;

                    var light = Light.Create(scene, $"light_{obj.InstanceId}");
                    Light.SetIntensity(light, lightObj.DiffuseColorHDRI.Intensity * 10000);  // arbitrary value constant, just what makes lighting look correct

                    switch (lightObj.LightType)
                    {
                        case LightType.Directional:
                            Light.SetLightType(light, Light.EType.eDirectional); break;
                        case LightType.Point:
                            Light.SetLightType(light, Light.EType.ePoint); break;
                        case LightType.Spot:
                            Light.SetLightType(light, Light.EType.eSpot); break;
                        case LightType.Plane:
                            Light.SetLightType(light, Light.EType.eArea); break;  // areaLightShape defaults to eRectangle
                        default:
                            Light.SetLightType(light, Light.EType.ePoint); break;
                    }

                    Light.SetColor(light, lightObj.DiffuseColorHDRI.Red / 255f, lightObj.DiffuseColorHDRI.Green / 255f, lightObj.DiffuseColorHDRI.Blue / 255f);

                    switch (lightObj.Attenuation)
                    {
                        case 1:
                            Light.SetDecay(light, Light.EDecayType.eLinear); break;
                        case 2:
                            Light.SetDecay(light, Light.EDecayType.eQuadratic); break;
                        case 3:
                            Light.SetDecay(light, Light.EDecayType.eCubic); break;
                    }

                    if (lightObj.BGShadowEnabled == 1) Light.CastShadows(light);
                    
                    if (lightObj.LightType == LightType.Spot)
                    {
                        Light.SetAngle(light, lightObj.ConeDegree + lightObj.AttenuationConeCoefficient, lightObj.ConeDegree); 
                    }

                    Node.SetNodeAttribute(obj_node, light);
                    return obj_node;
                case LayerEntryType.EventObject:
                    var eventObj = (EventInstanceObject)obj.Object;
                    var success = EObjSheet.TryGetRow(eventObj.ParentData.BaseId, out var row);
                    if (!success) return IntPtr.Zero;
                    if (row.SgbPath.ValueNullable == null) return IntPtr.Zero;
                    sgb_path = row.SgbPath.Value.SgbPath.ToString();
                    if (!sgb_path.EndsWith("sgb")) return IntPtr.Zero;  // 1 more sanity check

                    obj_node = Node.Create(scene, sgb_path.Substring(sgb_path.LastIndexOf("/") + 1));
                    init_child_node(obj, obj_node);
                    if (process_sgb(sgb_path, obj_node))
                    {
                        return obj_node;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private bool process_sgb(string sgb_path, IntPtr parentNode)
        {
            var has_child = false;
            var sgb = data.GetFile<SgbFile>(sgb_path);
            if (sgb == null) return false;

            for (int i = 0; i < sgb.LayerGroups.Length; i++)
            {
                var layer_group = sgb.LayerGroups[i];
                var layer_group_node = Node.Create(scene, $"LayerGroup{i}");  // this is probably redundant, i've only seen sgbs with 1 layer group

                if (process_layers(layer_group.Layers, layer_group_node))
                {
                    Node.AddChild(parentNode, layer_group_node);
                    has_child = true;
                }

                if (flags.enableJsonExport)
                {
                    Util.save_json(Path.GetFileNameWithoutExtension(sgb_path), layer_group.Layers, output_path);
                }

            }
            return has_child;
        }

        private bool process_lgbs()
        {
            string[] lgbs = { "bg", "planevent", "planlive", "planmap" }; //, "planner"};

            var root_node = Scene.GetRootNode(scene);
            foreach (var lgb_name in lgbs)
            {
                var path = $"bg/{zone_path.Substring(0, zone_path.Length - 5)}/{lgb_name}.lgb";
                var file = data.GetFile<LgbFile>(path);

                // skip if lgb doesn't exist (or fail if bg.lgb doesn't exist)
                if (file == null)
                {
                    if (lgb_name == "bg") return false;
                    continue;
                }

                process_layers(file.Layers, root_node);
                if (flags.enableJsonExport) Util.save_json(Path.GetFileNameWithoutExtension(path), file.Layers, output_path);
            }

            return true;
        }

        ~ZoneExporter()
        {
            if (manager != IntPtr.Zero) Manager.Destroy(manager);
        }
    }
}
