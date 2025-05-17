using Lumina.Data.Files;
using Lumina.Models.Materials;
using Lumina.Models.Models;
using ZoneFbx.Fbx;

namespace ZoneFbx.Processor
{
    internal class ModelProcessor
    {
        private readonly Lumina.GameData data;
        private readonly MaterialProcessor materialProcessor;
        private readonly Dictionary<string, IntPtr> mesh_cache = [];
        private readonly IntPtr manager;
        private readonly IntPtr scene;

        public ModelProcessor(Lumina.GameData data, MaterialProcessor materialProcessor, IntPtr manager, IntPtr scene)
        {
            this.data = data;
            this.materialProcessor = materialProcessor;
            this.manager = manager;
            this.scene = scene;
        }

        public Model? LoadModel(string modelPath)
        {
            var modelFile = data.GetFile<MdlFile>(modelPath);
            if (modelFile == null) return null;

            var model = new Model(modelFile);
            try
            {
                model.Update(data);
            } catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Object " + modelPath + " could not be resolved from game data.");
                Console.WriteLine(e.Message);
                model = new Model(modelFile);
            }
            return model;
        }

        public bool ProcessModel(Model model, IntPtr node)
        {
            if (model.File == null) return false;

            var path = Path.GetFileNameWithoutExtension(model.File.FilePath.Path);

            var hasChildren = false;
            for (int i = 0; i < model.Meshes.Length; i++)
            {
                string name = $"{path}_{i}";
                var meshNode = Node.Create(manager, name);

                if (!mesh_cache.TryGetValue(name, out var mesh))
                {
                    mesh = createMesh(model.Meshes[i], name);

                    var layer = Fbx.Mesh.GetLayer(mesh, 0);
                    var layerElementMaterial = Layer.ElementMaterial.Create(mesh, "elementMaterial");
                    Layer.ElementMaterial.SetMappingMode(layerElementMaterial);
                    Layer.ElementMaterial.SetReferenceMode(layerElementMaterial);
                    Layer.SetMaterials(layer, layerElementMaterial);
                    var layerMaterialIndexArray = Layer.ElementMaterial.GetIndexArray(layerElementMaterial);
                    Layer.Material_Add(layerMaterialIndexArray, 0);

                    mesh_cache[name] = mesh;
                }

                IntPtr material = materialProcessor.CreateMaterial(model.Meshes[i].Material);
                if (material == IntPtr.Zero) continue;

                Node.AddMaterial(meshNode, material);


                Node.SetNodeAttribute(meshNode, mesh);
                Node.AddChild(node, meshNode);
                hasChildren = true;
            }
            return hasChildren;
        }

        private IntPtr createMesh(Lumina.Models.Models.Mesh gameMesh, string name)
        {
            var mesh = Fbx.Mesh.Create(scene, name);
            Fbx.Mesh.Init(mesh, gameMesh.Vertices.Length);

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

            for (int i = 0; i < gameMesh.Vertices.Length; i++)
            {
                IntPtr pos = IntPtr.Zero, norm = IntPtr.Zero, tangent1 = IntPtr.Zero, tangent2 = IntPtr.Zero, color = IntPtr.Zero;


                if (gameMesh.Vertices[i].Position.HasValue)
                {
                    pos = Fbx.Vector4.Create(gameMesh.Vertices[i].Position!.Value.X,
                             gameMesh.Vertices[i].Position!.Value.Y,
                             gameMesh.Vertices[i].Position!.Value.Z,
                             gameMesh.Vertices[i].Position!.Value.W);
                }

                if (gameMesh.Vertices[i].Normal.HasValue)
                {
                    norm = Fbx.Vector4.Create(gameMesh.Vertices[i].Normal!.Value.X,
                              gameMesh.Vertices[i].Normal!.Value.Y,
                              gameMesh.Vertices[i].Normal!.Value.Z,
                              0);
                }

                if (gameMesh.Vertices[i].Color.HasValue)
                {
                    color = Fbx.Vector4.Create(gameMesh.Vertices[i].Color!.Value.X,
                             gameMesh.Vertices[i].Color!.Value.Y,
                             gameMesh.Vertices[i].Color!.Value.Z,
                             gameMesh.Vertices[i].Color!.Value.W);
                }

                if (gameMesh.Vertices[i].Tangent1.HasValue)
                {
                    tangent1 = Fbx.Vector4.Create(gameMesh.Vertices[i].Tangent1!.Value.X,
                              gameMesh.Vertices[i].Tangent1!.Value.Y,
                              gameMesh.Vertices[i].Tangent1!.Value.Z,
                              0);
                }

                if (gameMesh.Vertices[i].Tangent2.HasValue)
                {
                    tangent2 = Fbx.Vector4.Create(gameMesh.Vertices[i].Tangent2!.Value.X,
                             gameMesh.Vertices[i].Tangent2!.Value.Y,
                             gameMesh.Vertices[i].Tangent2!.Value.Z,
                             gameMesh.Vertices[i].Tangent2!.Value.W);
                }

                if (pos != IntPtr.Zero && norm != IntPtr.Zero)
                {
                    Fbx.Mesh.SetControlPointAt(mesh, pos, norm, i);
                }

                if (gameMesh.Vertices[i].UV.HasValue)
                {
                    var uv1Array = GeometryElement.UV.GetDirectArray(uvElement1);
                    Fbx.Layer.UV_Add(uv1Array, Fbx.Vector2.Create(gameMesh.Vertices[i].UV!.Value.X, gameMesh.Vertices[i].UV!.Value.Y * -1));
                    var uv2Array = GeometryElement.UV.GetDirectArray(uvElement2);
                    Fbx.Layer.UV_Add(uv2Array, Fbx.Vector2.Create(gameMesh.Vertices[i].UV!.Value.Z, gameMesh.Vertices[i].UV!.Value.W * -1));
                }

                var colorArray = GeometryElement.VertexColor.GetDirectArray(colorElement);
                Fbx.Layer.Color_Add(colorArray, color);

                var tangent1Array = GeometryElement.Tangent.GetDirectArray(tangentElem1);
                Fbx.Layer.Tangent_Add(tangent1Array, tangent1);
                var tangent2Array = GeometryElement.Tangent.GetDirectArray(tangentElem2);
                Fbx.Layer.Tangent_Add(tangent2Array, tangent2);
            }

            for (int i = 0; i < gameMesh.Indices.Length; i += 3)
            {
                Fbx.Mesh.BeginPolygon(mesh);
                Fbx.Mesh.AddPolygon(mesh, gameMesh.Indices[i]);
                Fbx.Mesh.AddPolygon(mesh, gameMesh.Indices[i + 1]);
                Fbx.Mesh.AddPolygon(mesh, gameMesh.Indices[i + 2]);
                Fbx.Mesh.EndPolygon(mesh);
            }

            return mesh;
        }
    }
}
