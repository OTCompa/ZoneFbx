using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ZoneFbx
{
    internal class ZoneExporter
    {
        private string game_path;
        private string zone_path;
        private string output_path;
        private string zone_code;
        private Lumina.GameData data;
        IntPtr manager = IntPtr.Zero;
        IntPtr scene = IntPtr.Zero;
        Dictionary<ulong, IntPtr> material_cache = new Dictionary<ulong, IntPtr>();
        Dictionary<string, IntPtr> mesh_cache = new Dictionary<string, IntPtr>();

        public ZoneExporter(string game_path, string zone_path, string output_path)
        {
            this.game_path = game_path;
            this.zone_path = zone_path;
            this.output_path = output_path;

            zone_code = zone_path.Substring(zone_path.LastIndexOf("/level") - 4, 4);

            Console.WriteLine("Initializing...");
            if (!init())
            {
                Console.WriteLine("Error occurred during ZoneExporter initialization.");
                return;
            }

            Console.WriteLine("Processing models and textures...");
            Console.WriteLine("Processing zone terrain");
            if (!process_terrain())
            {
                Console.WriteLine("Failed to process zone terrain.");
                return;
            }

            Console.WriteLine("Processing bg.lgb...");
            if (!process_bg())
            {
                Console.WriteLine("Failed to process zone bg.");
                return;
            }

            Console.WriteLine("Saving scene...");
            if (!save_scene())
            {
                Console.WriteLine("Failed to save scene.");
                return;
            }

            Console.WriteLine("Saved scene...");

        }

        private bool init()
        {
            try
            {
                data = new Lumina.GameData(game_path);
            } catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Error: Game path directory is not valid!\n");
                return false;
            }

            string name = zone_path.Substring(zone_path.LastIndexOf("/level") - 4, 4);

            manager = Fbx.FbxManager_Create();
            if (manager == IntPtr.Zero)
            {
                Console.WriteLine(":(");
                return false;
            }
            Fbx.FbxManager_Initialize(manager);
            scene = Fbx.FbxScene_Create(manager, name);
            // Fbx.FbxManager_Destroy(manager);
            return true;
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

            var terrain_node = Fbx.FbxNode_Create(manager, "terrain");

            for (int i = 0; i < terafile?.PlateCount; i++)
            {
                var plate_node = Fbx.FbxNode_Create(manager, "bgplate_" + i.ToString());
                var pos = terafile.GetPlatePosition(i);
                Fbx.FbxNode_SetLclTranslation(plate_node, pos.X, 0, pos.Y);

                string model_filename = string.Format("{0:D4}.mdl", i);
                string model_path = Path.Combine(terrain_path, model_filename);
                var plate_model_file = data.GetFile<MdlFile>(model_path);
                var plate_model = new Lumina.Models.Models.Model(plate_model_file!);
                plate_model.Update(data);

                process_model(plate_model, plate_node);

                Fbx.FbxNode_AddChild(terrain_node, plate_node);
            }

            var rootNode = Fbx.FbxScene_GetRootNode(scene);
            Fbx.FbxNode_AddChild(rootNode, terrain_node);

            return true;
        }

        private void process_model(Lumina.Models.Models.Model model, IntPtr node)
        {
            var path = model!.File!.FilePath.Path;
            path = path.Substring(path.LastIndexOf('/') + 1);
            for (int i = 0; i < model.Meshes.Length; i++)
            {
                string mesh_name = path + "_" + i.ToString();
                var mesh_node = Fbx.FbxNode_Create(manager, mesh_name);

                var result = mesh_cache.TryGetValue(mesh_name, out var cached_mesh);
                IntPtr mesh;
                if (result)
                {
                    mesh = cached_mesh;
                } else
                {
                    mesh = create_mesh(model.Meshes[i], mesh_name);
                    IntPtr material;
                    material = create_material(model.Meshes[i].Material);
                    Fbx.FbxNode_AddMaterial(mesh_node, material);
                    mesh_cache[mesh_name] = mesh;
                }
                Fbx.FbxNode_SetNodeAttribute(mesh_node, mesh);
                Fbx.FbxNode_AddChild(node, mesh_node);
            }
        }

        private IntPtr create_mesh(Lumina.Models.Models.Mesh game_mesh, string mesh_name)
        {
            var mesh = Fbx.FbxMesh_Create(scene, mesh_name);
            Fbx.FbxMesh_Init(mesh, game_mesh.Vertices.Length);

            var colorElement = Fbx.FbxGeometryElementVertexColor_Create(mesh);
            Fbx.FbxGeometryElementVertexColor_SetMappingNode(colorElement);

            var uvElement1 = Fbx.FbxGeometryElementUV_Create(mesh, "uv1");
            Fbx.FbxGeometryElementUV_SetMappingNode(uvElement1);

            var uvElement2 = Fbx.FbxGeometryElementUV_Create(mesh, "uv2");
            Fbx.FbxGeometryElementUV_SetMappingNode(uvElement2);

            var tangentElem1 = Fbx.FbxGeometryElementTangent_Create(mesh);
            Fbx.FbxGeometryElementTangent_SetMappingNode(tangentElem1);

            var tangentElem2 = Fbx.FbxGeometryElementTangent_Create(mesh);
            Fbx.FbxGeometryElementTangent_SetMappingNode(tangentElem2);

            for (int i = 0; i < game_mesh.Vertices.Length; i++)
            {
                IntPtr pos = IntPtr.Zero, norm = IntPtr.Zero, uv = IntPtr.Zero, tangent1 = IntPtr.Zero, tangent2 = IntPtr.Zero, color = IntPtr.Zero;

                if (game_mesh.Vertices[i].Position.HasValue)
                {
                    pos = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Position.Value.X,
                             game_mesh.Vertices[i].Position.Value.Y,
                             game_mesh.Vertices[i].Position.Value.Z,
                             game_mesh.Vertices[i].Position.Value.W);
                }

                if (game_mesh.Vertices[i].Normal.HasValue)
                {
                    norm = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Normal.Value.X,
                              game_mesh.Vertices[i].Normal.Value.Y,
                              game_mesh.Vertices[i].Normal.Value.Z,
                              0);
                }

                if (game_mesh.Vertices[i].Color.HasValue)
                {
                    color = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Color.Value.X,
                             game_mesh.Vertices[i].Color.Value.Y,
                             game_mesh.Vertices[i].Color.Value.Z,
                             game_mesh.Vertices[i].Color.Value.W);
                }

                if (game_mesh.Vertices[i].Tangent1.HasValue)
                {
                    tangent1 = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Tangent1.Value.X,
                              game_mesh.Vertices[i].Tangent1.Value.Y,
                              game_mesh.Vertices[i].Tangent1.Value.Z,
                              0);
                }

                if (game_mesh.Vertices[i].Tangent2.HasValue)
                {
                    tangent2 = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Tangent2.Value.X,
                             game_mesh.Vertices[i].Tangent2.Value.Y,
                             game_mesh.Vertices[i].Tangent2.Value.Z,
                             game_mesh.Vertices[i].Tangent2.Value.W);
                }

                if (pos != IntPtr.Zero && norm != IntPtr.Zero)
                {
                    Fbx.FbxMesh_SetControlPointAt(mesh, pos, norm, i);
                }

                if (game_mesh.Vertices[i].UV.HasValue)
                {
                    var uv1Array = Fbx.FbxGeometryElementUV_GetDirectArray(uvElement1);
                    Fbx.FbxLayerUV_Add(uv1Array, Fbx.FbxVector2_Create(game_mesh.Vertices[i].UV.Value.X, game_mesh.Vertices[i].UV.Value.Y * -1));
                    var uv2Array = Fbx.FbxGeometryElementUV_GetDirectArray(uvElement2);
                    Fbx.FbxLayerUV_Add(uv2Array, Fbx.FbxVector2_Create(game_mesh.Vertices[i].UV.Value.Z, game_mesh.Vertices[i].UV.Value.W * -1));
                }

                var colorArray = Fbx.FbxGeometryElementVertexColor_GetDirectArray(colorElement);
                Fbx.FbxLayerColor_Add(colorArray, color);

                var tangent1Array = Fbx.FbxGeometryElementTangent_GetDirectArray(tangentElem1);
                Fbx.FbxLayerTangent_Add(tangent1Array, tangent1);
                var tangent2Array = Fbx.FbxGeometryElementTangent_GetDirectArray(tangentElem2);
                Fbx.FbxLayerTangent_Add(tangent2Array, tangent2);
            }

            for (int i = 0; i < game_mesh.Indices.Length; i+= 3)
            {
                Fbx.FbxMesh_BeginPolygon(mesh);
                Fbx.FbxMesh_AddPolygon(mesh, game_mesh.Indices[i]);
                Fbx.FbxMesh_AddPolygon(mesh, game_mesh.Indices[i+1]);
                Fbx.FbxMesh_AddPolygon(mesh, game_mesh.Indices[i+2]);
                Fbx.FbxMesh_EndPolygon(mesh);
            }

            return mesh;
        }

        private IntPtr create_material(Lumina.Models.Materials.Material mat)
        {
            IntPtr outsurface;
            var mat_path = mat.MaterialPath;
            var material_name = mat_path.Substring(mat_path.LastIndexOf('/') + 1);

            var hash_file = mat.File;
            if (hash_file == null) return IntPtr.Zero;
            var hash = hash_file.FilePath.IndexHash;
            var result = material_cache.TryGetValue(hash, out var res);

            if (result)
            {
                return res;
            }
            extract_textures(mat);
            outsurface = Fbx.FbxSurfacePhong_Create(scene, material_name);

            Fbx.FbxSurfacePhong_SetFactor(outsurface);

            for (int i = 0; i < mat.Textures.Length; i++)
            {
                if (mat.Textures[i].TexturePath.Contains("dummy")) continue;

                string usage_name = mat.Textures[i].TextureUsageSimple.ToString();

                var texture = Fbx.FbxFileTexture_Create(scene, usage_name);
                var rel = Util.get_texture_path(output_path, zone_code, mat.Textures[i].TexturePath);
                Fbx.FbxFileTexture_SetStuff(texture, rel);

                switch (mat.Textures[i].TextureUsageRaw)
                {
                    case Lumina.Data.Parsing.TextureUsage.SamplerColorMap0:
                        Fbx.FbxSurfacePhong_ConnectSrcObject(outsurface, texture, 0);
                        break;
                    case Lumina.Data.Parsing.TextureUsage.SamplerSpecularMap0:
                        Fbx.FbxSurfacePhong_ConnectSrcObject(outsurface, texture, 1);
                        break;
                    case Lumina.Data.Parsing.TextureUsage.SamplerNormalMap0:
                        Fbx.FbxSurfacePhong_ConnectSrcObject(outsurface, texture, 2);
                        break;
                }
                
            }

            material_cache[hash] = outsurface;


            return outsurface;
        }

        private void extract_textures(Lumina.Models.Materials.Material mat)
        {
            for (int i = 0; i < mat.Textures.Length; i++)
            {
                var tex_path = Util.get_texture_path(output_path, zone_code, mat.Textures[i].TexturePath);
                if (File.Exists(tex_path)) continue;

                Lumina.Data.Files.TexFile texfile;
                try {
                    texfile = mat.Textures[i].GetTextureNc(data);
                } catch(Exception)
                {
                    continue;
                }

                Image texture;
                try
                {
                    unsafe
                    {
                        byte[] buffer = new byte[texfile.ImageData.Length];
                        fixed (byte* p = texfile.ImageData)
                        {
                            IntPtr imageData = (IntPtr)p;
                            texture = new Bitmap(texfile.Header.Width, texfile.Header.Height, texfile.Header.Width * 4, PixelFormat.Format32bppArgb, imageData);
                        }
                    }
                } catch (NotSupportedException)
                {
                    Console.WriteLine("Not supported: " + tex_path);
                    continue;
                }
                Directory.CreateDirectory(Path.GetDirectoryName(tex_path));
                texture.Save(tex_path, ImageFormat.Png);
            }
        }

        private bool process_bg()
        {
            string bg_path = "bg/" + zone_path.Substring(0, zone_path.Length - 5) + "/bg.lgb";
            var bg = data.GetFile<Lumina.Data.Files.LgbFile>(bg_path);

            if (bg == null) return false;
            for (int i = 0; i < bg.Layers.Length; i++)
            {
                var layer = bg.Layers[i];
                var layer_node = Fbx.FbxNode_Create(scene, layer.Name);

                for (int j = 0; j < layer.InstanceObjects.Length; j++)
                {
                    var obj = layer.InstanceObjects[j];
                    var obj_node = Fbx.FbxNode_Create(scene, obj.Name);

                    Fbx.FbxNode_SetStuff(obj_node, obj.Transform.Translation.X, obj.Transform.Translation.Y, obj.Transform.Translation.Z, 0);
                    Fbx.FbxNode_SetStuff(obj_node, Util.degrees(obj.Transform.Rotation.X), Util.degrees(obj.Transform.Rotation.Y), Util.degrees(obj.Transform.Rotation.Z), 1);
                    Fbx.FbxNode_SetStuff(obj_node, obj.Transform.Scale.X, obj.Transform.Scale.Y, obj.Transform.Scale.Z, 2);

                    if (obj.AssetType == Lumina.Data.Parsing.Layer.LayerEntryType.BG)
                    {
                        var instance_object = (Lumina.Data.Parsing.Layer.LayerCommon.BGInstanceObject)obj.Object;
                        var object_path = instance_object.AssetPath;
                        var object_file = data.GetFile<MdlFile>(object_path);
                        var model = new Lumina.Models.Models.Model(object_file);
                        model.Update(data);

                        var model_node = Fbx.FbxNode_Create(scene, object_path.Substring(object_path.LastIndexOf("/") + 1));

                        process_model(model, model_node);

                        Fbx.FbxNode_AddChild(obj_node, model_node);
                        Fbx.FbxNode_AddChild(layer_node, obj_node);
                    }
                }
                var root_node = Fbx.FbxScene_GetRootNode(scene);
                Fbx.FbxNode_AddChild(root_node, layer_node);
            }

            return true;
        }

        private bool save_scene()
        {
            var exporter = Fbx.FbxExporter_Create(manager, "exporter");
            var out_fbx = output_path + zone_code + ".fbx";

            if (!Fbx.FbxExporter_Initialize(exporter, out_fbx, manager))
            {
                return false;
            }
            var result = Fbx.FbxExporter_Export(exporter, scene);

            Fbx.FbxExporter_Destroy(exporter);
            return result;
            
        }

        ~ZoneExporter()
        {
            if (manager != IntPtr.Zero) Fbx.FbxManager_Destroy(manager);
        }
    }
}
