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

namespace ZoneFbx
{
    internal partial class ZoneExporter
    {
        private string game_path;
        private string zone_path;
        private string output_path;
        private string zone_code;
        private readonly Lumina.GameData data;
        private readonly ExcelSheet<EObj> EObjSheet;
        private Flags flags;

        IntPtr manager = IntPtr.Zero;
        IntPtr scene = IntPtr.Zero;

        Dictionary<ulong, IntPtr> material_cache = new();
        Dictionary<string, IntPtr> mesh_cache = new();

        public ZoneExporter(string game_path, string zone_path, string output_path, Flags flags)
        {
            this.game_path = game_path;
            this.zone_path = zone_path;

            zone_code = zone_path.Substring(zone_path.LastIndexOf("/level") - 4, 4);

            this.output_path = Path.Combine(output_path, zone_code) + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(this.output_path);

            this.flags = flags;

            Console.WriteLine("Initializing...");

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

            Console.WriteLine($"Done! Map exported to {Path.Combine(output_path, zone_code, $"{Path.GetFileName(zone_code)}.fbx")}");

        }

        private bool init()
        {
            string name = zone_path.Substring(zone_path.LastIndexOf("/level") - 4, 4);

            manager = Fbx.FbxManager_Create();
            if (manager == IntPtr.Zero)
            {
                Console.WriteLine("Failed to create FbxManager");
                return false;
            }
            Fbx.FbxManager_Initialize(manager);
            scene = Fbx.FbxScene_Create(manager, name);
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

                Fbx.FbxNode_AddChild(terrain_node, plate_node);
            }

            var rootNode = Fbx.FbxScene_GetRootNode(scene);
            Fbx.FbxNode_AddChild(rootNode, terrain_node);

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
                    if (material == IntPtr.Zero) continue;

                    Fbx.FbxNode_AddMaterial(mesh_node, material);
                    mesh_cache[mesh_name] = mesh;
                }
                Fbx.FbxNode_SetNodeAttribute(mesh_node, mesh);
                Fbx.FbxNode_AddChild(node, mesh_node);
                has_children = true;
            }
            return has_children;
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
                    pos = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Position!.Value.X,
                             game_mesh.Vertices[i].Position!.Value.Y,
                             game_mesh.Vertices[i].Position!.Value.Z,
                             game_mesh.Vertices[i].Position!.Value.W);
                }

                if (game_mesh.Vertices[i].Normal.HasValue)
                {
                    norm = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Normal!.Value.X,
                              game_mesh.Vertices[i].Normal!.Value.Y,
                              game_mesh.Vertices[i].Normal!.Value.Z,
                              0);
                }

                if (game_mesh.Vertices[i].Color.HasValue)
                {
                    color = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Color!.Value.X,
                             game_mesh.Vertices[i].Color!.Value.Y,
                             game_mesh.Vertices[i].Color!.Value.Z,
                             game_mesh.Vertices[i].Color!.Value.W);
                }

                if (game_mesh.Vertices[i].Tangent1.HasValue)
                {
                    tangent1 = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Tangent1!.Value.X,
                              game_mesh.Vertices[i].Tangent1!.Value.Y,
                              game_mesh.Vertices[i].Tangent1!.Value.Z,
                              0);
                }

                if (game_mesh.Vertices[i].Tangent2.HasValue)
                {
                    tangent2 = Fbx.FbxVector4_Create(game_mesh.Vertices[i].Tangent2!.Value.X,
                             game_mesh.Vertices[i].Tangent2!.Value.Y,
                             game_mesh.Vertices[i].Tangent2!.Value.Z,
                             game_mesh.Vertices[i].Tangent2!.Value.W);
                }

                if (pos != IntPtr.Zero && norm != IntPtr.Zero)
                {
                    Fbx.FbxMesh_SetControlPointAt(mesh, pos, norm, i);
                }

                if (game_mesh.Vertices[i].UV.HasValue)
                {
                    var uv1Array = Fbx.FbxGeometryElementUV_GetDirectArray(uvElement1);
                    Fbx.FbxLayerUV_Add(uv1Array, Fbx.FbxVector2_Create(game_mesh.Vertices[i].UV!.Value.X, game_mesh.Vertices[i].UV!.Value.Y * -1));
                    var uv2Array = Fbx.FbxGeometryElementUV_GetDirectArray(uvElement2);
                    Fbx.FbxLayerUV_Add(uv2Array, Fbx.FbxVector2_Create(game_mesh.Vertices[i].UV!.Value.Z, game_mesh.Vertices[i].UV!.Value.W * -1));
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
            if (!flags.enableLightshaftModels && mat.ShaderPack == "lightshaft.shpk") return IntPtr.Zero;

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

            var materialInfo = flags.disableBaking ? null : get_shader(mat);
            outsurface = Fbx.FbxSurfacePhong_Create(scene, material_name);

            Fbx.FbxSurfacePhong_SetFactor(outsurface);
            HashSet<Texture.Usage> alreadySet = new HashSet<Texture.Usage>();
            for (int i = 0; i < mat.Textures.Length; i++)
            {
                var tex = mat.Textures[i];
                if (tex == null) continue;
                if (tex.TexturePath.Contains("dummy")) continue;

                if (alreadySet.Contains(tex.TextureUsageSimple)) continue;
                alreadySet.Add(tex.TextureUsageSimple);

                Vector3? v = null;

                switch (tex.TextureUsageSimple)
                {
                    case Texture.Usage.Diffuse:
                        v = materialInfo?.DiffuseColor ?? v; break;
                    case Texture.Usage.Specular:
                        v = materialInfo?.SpecularColor ?? v; break;
                }

                var tex_path = Util.get_texture_path(output_path, zone_code, tex.TexturePath, mat.MaterialPath, v);
                var emissive_path = "";
                extract_texture(tex, v, tex_path);
                
                if (tex.TextureUsageSimple == Texture.Usage.Diffuse && materialInfo != null && materialInfo.EmissiveColor.HasValue)
                {
                    emissive_path = Util.get_texture_path(output_path, zone_code, tex.TexturePath, mat.MaterialPath, materialInfo.EmissiveColor, type: "_e");
                    extract_texture(tex, materialInfo.EmissiveColor, emissive_path);
                }

                string tex_name = Path.GetFileNameWithoutExtension(tex.TexturePath);

                var texture = Fbx.FbxFileTexture_Create(scene, tex_name);
                Fbx.FbxFileTexture_SetStuff(texture, tex_path);

                switch (tex.TextureUsageSimple)
                {
                    case Texture.Usage.Diffuse:
                        Fbx.FbxSurfacePhong_ConnectSrcObject(outsurface, texture, 0);
                        if (emissive_path.Length > 0)
                        {
                            string emissive_name = Path.GetFileNameWithoutExtension(tex.TexturePath) + "_e";
                            var emissive = Fbx.FbxFileTexture_Create(scene, emissive_name);
                            Fbx.FbxFileTexture_SetStuff(emissive, emissive_path);
                            Fbx.FbxSurfacePhong_ConnectSrcObject(outsurface, emissive, 3);
                        }
                        break;
                    case Texture.Usage.Specular:
                        Fbx.FbxSurfacePhong_ConnectSrcObject(outsurface, texture, 1);
                        break;
                    case Texture.Usage.Normal:
                        Fbx.FbxSurfacePhong_ConnectSrcObject(outsurface, texture, 2);
                        break;
                }
            }

            material_cache[hash] = outsurface;

            return outsurface;
        }

        private void extract_texture(Texture tex, Vector3? v, string tex_path)
        {
            if (File.Exists(tex_path)) return;
            TexFile? texfile;
            try
            {
                texfile = tex.GetTextureNc(data);
                if (texfile == null) throw new Exception();
            } catch (Exception)
            {
                Console.WriteLine("Failed to get texture: " + tex.TexturePath);
                return;
            }

            try
            {
                Util.SaveAsBitmap(tex_path, texfile.ImageData, texfile.Header.Width, texfile.Header.Height, v);
            } catch (NotSupportedException)
            {
                var decoded = new byte[texfile.Header.Width * texfile.Header.Height * 4];
                if (texfile.Header.Format == TexFile.TextureFormat.BC7)
                {
                    Console.WriteLine("Processing BC7 texture: " + tex_path);
                    Bc7Sharp.Decode(texfile.Data, decoded, texfile.Header.Width, texfile.Header.Height);

                    Util.SaveAsBitmap(tex_path, decoded, texfile.Header.Width, texfile.Header.Height, v);
                } else if (texfile.Header.Format == TexFile.TextureFormat.BC5)
                {
                    Console.WriteLine("Processing BC5 texture: " + tex_path);
                    Bc5Sharp.Decode(texfile.Data, decoded, texfile.Header.Width, texfile.Header.Height);

                    Util.SaveAsBitmap(tex_path, decoded, texfile.Header.Width, texfile.Header.Height, v);
                } else
                {
                    Console.WriteLine("Not supported: " + tex_path);
                    return;
                }
            }
        }

        private MaterialInfo? get_shader(Material mat)
        {
            var diffuseOffset = -1;
            var specularOffset = -1;
            var emissiveOffset = -1;

            if (mat.File == null) { return null; }

            for (int i = 0; i < mat.File.Constants.Length; i++)
            {
                var constant = mat.File.Constants[i];
                switch(constant.ConstantId)
                {
                    case 0x2C2A34DD:
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for diffuse color. May cause unexpected results.");
                        }
                        diffuseOffset = constant.ValueOffset;
                        break;
                    case 0x141722D5:
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for specular color. May cause unexpected results.");
                        }
                        specularOffset = constant.ValueOffset;
                        break;
                    case 0x38A64362:
                        if (constant.ValueSize != 12)
                        {
                            Console.WriteLine("Unexpected size for emmisive color. May cause unexpected results.");
                        }
                        emissiveOffset = constant.ValueOffset;
                        break;
                }
            }

            if (diffuseOffset == -1 && specularOffset == -1) return null;

            long cursor = 16; // mtrl header size
            var br = mat.File.Reader;

            int colorsetBlockSize;
            int stringBlockSize;
            int additionalDataSize;

            // get string block size
            br.Seek(6);
            colorsetBlockSize = br.ReadUInt16();
            stringBlockSize = br.ReadUInt16();

            // get tex/map/colorset numbers to figure out where string block is
            br.Seek(12);
            var numStrings = 0;

            for (int i = 0; i < 3; i++)
            {
                numStrings += br.ReadByte();
            }

            additionalDataSize = br.ReadByte();

            // put cursor at beginning of shader area
            cursor += numStrings * 4 + additionalDataSize + stringBlockSize + colorsetBlockSize + 2;  // skip shader constants data size
            br.Seek(cursor);

            var numShaderKeys = br.ReadUInt16();
            var numShaderConstants = br.ReadUInt16();
            var numTextureSampler = br.ReadUInt16();
            br.Seek(br.Position + 4 + numShaderKeys * 8);  // position at the start of shader constants ids/offsets/sizes
            cursor = br.Position + numShaderConstants * 8 + numTextureSampler * 12;

            // get all the relevant information
            var ret = new MaterialInfo();
            if (diffuseOffset >= -1)
            {
                br.Seek(cursor + diffuseOffset);
                var v = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                if (v != Vector3.One)
                    ret.DiffuseColor = v;
            }
            if (specularOffset >= -1)
            {
                br.Seek(cursor + specularOffset);
                var v = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                if (v != Vector3.One)
                    ret.SpecularColor = v;
            }
            if (emissiveOffset >= -1)
            {
                br.Seek(cursor + emissiveOffset);  // multiply by 0.2 to simulate emissiveFactor = 0.2
                var v = new Vector3(br.ReadSingle() * .2f, br.ReadSingle() * .2f, br.ReadSingle() * .2f);
                if (v != Vector3.Zero)
                    ret.EmissiveColor = v;
            }

            return ret;
        }

        private IntPtr init_child_node(LayerCommon.InstanceObject obj)
        {
            var obj_node = Fbx.FbxNode_Create(scene, obj.Name);
            Fbx.FbxNode_SetStuff(obj_node, obj.Transform.Translation.X, obj.Transform.Translation.Y, obj.Transform.Translation.Z, 0);
            if (obj.AssetType == LayerEntryType.LayLight)
            {
                // rotate light nodes -90 degrees on the X axis since the light nodes point towards its negative Y axis
                Fbx.FbxNode_SetStuff(obj_node, Util.degrees(obj.Transform.Rotation.X) - 90, Util.degrees(obj.Transform.Rotation.Y), Util.degrees(obj.Transform.Rotation.Z), 1);
            } else
            {
                Fbx.FbxNode_SetStuff(obj_node, Util.degrees(obj.Transform.Rotation.X), Util.degrees(obj.Transform.Rotation.Y), Util.degrees(obj.Transform.Rotation.Z), 1);
            }
            Fbx.FbxNode_SetStuff(obj_node, obj.Transform.Scale.X, obj.Transform.Scale.Y, obj.Transform.Scale.Z, 2);
            return obj_node;
        }

        private bool process_layers(LayerCommon.Layer[] layers, IntPtr parentNode)
        {
            var group_has_child = false;
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (!flags.enableFestivals && layer.FestivalID != 0) continue;

                var layer_node = Fbx.FbxNode_Create(scene, layer.Name);
                var layer_has_child = false;

                for (int j = 0; j < layer.InstanceObjects.Length; j++)
                {
                    var childNode = process_asset(layer.InstanceObjects[j]);
                    if (childNode != IntPtr.Zero)
                    {
                        Fbx.FbxNode_AddChild(layer_node, childNode);
                        layer_has_child = true;
                    }
                }

                if (layer_has_child)
                {
                    Fbx.FbxNode_AddChild(parentNode, layer_node);
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
                    obj_node = init_child_node(obj);
                    var instance_object = (LayerCommon.BGInstanceObject)obj.Object;
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

                    var model_node = Fbx.FbxNode_Create(scene, object_path.Substring(object_path.LastIndexOf("/") + 1));

                    if (process_model(model, model_node))
                    {
                        Fbx.FbxNode_AddChild(obj_node, model_node);
                        return obj_node;
                    }
                    break;
                case LayerEntryType.SharedGroup:
                    obj_node = init_child_node(obj);
                    var sharedGroupObj = (LayerCommon.SharedGroupInstanceObject)obj.Object;
                    sgb_path = sharedGroupObj.AssetPath;

                    if (process_sgb(sgb_path, obj_node))
                    {
                        return obj_node;
                    }
                    break;
                case LayerEntryType.LayLight:
                    if (!flags.enableLighting) return IntPtr.Zero;

                    obj_node = init_child_node(obj);
                    var lightObj = (LayerCommon.LightInstanceObject)obj.Object;
                    if (lightObj.DiffuseColorHDRI.Intensity == 0.0) return IntPtr.Zero;

                    var light = Fbx.FbxLight_Create(scene, $"light_{obj.InstanceId}");

                    Fbx.FbxLight_SetLightType(light, (int)lightObj.LightType);
                    Fbx.FbxLight_SetColor(light, lightObj.DiffuseColorHDRI.Red, lightObj.DiffuseColorHDRI.Green, lightObj.DiffuseColorHDRI.Blue);
                    Fbx.FbxLight_SetIntensity(light, lightObj.DiffuseColorHDRI.Intensity * .1);  // arbitrarily chosen to make it look more natural
                    Fbx.FbxLight_SetDecay(light, (int)lightObj.Attenuation);
                    if (lightObj.BGShadowEnabled == 1) Fbx.FbxLight_CastShadows(light);
                    
                    if (lightObj.LightType == LightType.Spot)
                    {
                        Fbx.FbxLight_SetAngle(light, lightObj.AttenuationConeCoefficient, lightObj.ConeDegree); 
                    }

                    Fbx.FbxNode_SetNodeAttribute(obj_node, light);
                    return obj_node;
                case LayerEntryType.EventObject:
                    var eventObj = (LayerCommon.EventInstanceObject)obj.Object;
                    var success = EObjSheet.TryGetRow(eventObj.ParentData.BaseId, out var row);
                    if (!success) return IntPtr.Zero;
                    if (row.SgbPath.ValueNullable == null) return IntPtr.Zero;
                    sgb_path = row.SgbPath.Value.SgbPath.ToString();
                    if (!sgb_path.EndsWith("sgb")) return IntPtr.Zero;  // 1 more sanity check

                    obj_node = init_child_node(obj);
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
                var layer_group_node = Fbx.FbxNode_Create(scene, layer_group.Name);

                if (process_layers(layer_group.Layers, layer_group_node))
                {
                    Fbx.FbxNode_AddChild(parentNode, layer_group_node);
                    has_child = true;
                }

                if (flags.enableJsonExport)
                {
                    Util.save_json(Path.GetFileNameWithoutExtension(sgb_path), layer_group.Layers, output_path);
                }

            }
            return has_child;
        }

        private bool process_bg()
        {
            string bg_path = "bg/" + zone_path.Substring(0, zone_path.Length - 5) + "/bg.lgb";
            var bg = data.GetFile<Lumina.Data.Files.LgbFile>(bg_path);

            string planmap_path = "bg/" + zone_path.Substring(0, zone_path.Length - 5) + "/planmap.lgb";
            var planmap = data.GetFile<Lumina.Data.Files.LgbFile>(planmap_path);

            if (bg == null) return false;
            var root_node = Fbx.FbxScene_GetRootNode(scene);
            process_layers(bg.Layers, root_node);

            if (planmap != null) process_layers(planmap.Layers, root_node);

            if (flags.enableJsonExport)
            {
                Util.save_json(Path.GetFileNameWithoutExtension(bg_path), bg.Layers, output_path);
                if (planmap != null) Util.save_json(Path.GetFileNameWithoutExtension(planmap_path), planmap.Layers, output_path);
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
