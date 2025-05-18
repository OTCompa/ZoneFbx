using Lumina.Data.Files;
using Lumina.Models.Models;
using ZoneFbx.Fbx;

namespace ZoneFbx.Processor
{
    internal class ModelProcessor(Lumina.GameData data, IntPtr manager, IntPtr scene, ZoneExporter.Options options, MaterialProcessor materialProcessor) : Processor(data, manager, scene, options)
    {
        private readonly MaterialProcessor materialProcessor = materialProcessor;
        private readonly Dictionary<string, IntPtr> meshCache = [];

        public void ResetCache() => meshCache.Clear();

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

                if (!meshCache.TryGetValue(name, out var mesh))
                {
                    mesh = createMesh(model.Meshes[i], name);

                    var layer = Fbx.Mesh.GetLayer(mesh, 0);
                    var layerElementMaterial = Layer.ElementMaterial.Create(mesh, "elementMaterial");
                    Layer.ElementMaterial.SetMappingMode(layerElementMaterial);
                    Layer.ElementMaterial.SetReferenceMode(layerElementMaterial);
                    Layer.SetMaterials(layer, layerElementMaterial);
                    var layerMaterialIndexArray = Layer.ElementMaterial.GetIndexArray(layerElementMaterial);
                    Layer.Material_Add(layerMaterialIndexArray, 0);

                    meshCache[name] = mesh;
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

        public bool ProcessModelWithoutTexture(Model model, IntPtr node)
        {
            if (model.File == null) return false;

            var path = Path.GetFileNameWithoutExtension(model.File.FilePath.Path);

            var hasChildren = false;
            for (int i = 0; i < model.Meshes.Length; i++)
            {
                string name = $"{path}_{i}";
                var meshNode = Node.Create(manager, name);

                if (!meshCache.TryGetValue(name, out var mesh))
                {
                    mesh = createMesh(model.Meshes[i], name);
                    meshCache[name] = mesh;
                }
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
                IntPtr pos = IntPtr.Zero;
                IntPtr norm = IntPtr.Zero;
                IntPtr tangent1 = IntPtr.Zero;
                IntPtr tangent2 = IntPtr.Zero;
                IntPtr color = IntPtr.Zero;

                var vertex = gameMesh.Vertices[i];

                pos = vertex.Position.HasValue ? Vector4.Create(vertex.Position.Value.X, vertex.Position.Value.Y, vertex.Position.Value.Z, vertex.Position.Value.Z) : pos;
                norm = vertex.Normal.HasValue ? Vector4.Create(vertex.Normal.Value.X, vertex.Normal.Value.Y, vertex.Normal.Value.Z, 0) : norm;
                color = vertex.Color.HasValue ? Vector4.Create(vertex.Color.Value.X, vertex.Color.Value.Y, vertex.Color.Value.Z, vertex.Color.Value.W) : color;
                tangent1 = vertex.Tangent1.HasValue ? Vector4.Create(vertex.Tangent1.Value.X, vertex.Tangent1.Value.Y, vertex.Tangent1.Value.Z, vertex.Tangent1.Value.W) : tangent1;
                tangent2 = vertex.Tangent2.HasValue ? Vector4.Create(vertex.Tangent2.Value.X, vertex.Tangent2.Value.Y, vertex.Tangent2.Value.Z, vertex.Tangent2.Value.W) : tangent1;

                if (pos != IntPtr.Zero && norm != IntPtr.Zero)
                {
                    Fbx.Mesh.SetControlPointAt(mesh, pos, norm, i);
                }

                if (gameMesh.Vertices[i].UV.HasValue)
                {
                    var uv1Array = GeometryElement.UV.GetDirectArray(uvElement1);
                    Layer.UV_Add(uv1Array, Vector2.Create(gameMesh.Vertices[i].UV!.Value.X, gameMesh.Vertices[i].UV!.Value.Y * -1));
                    var uv2Array = GeometryElement.UV.GetDirectArray(uvElement2);
                    Layer.UV_Add(uv2Array, Vector2.Create(gameMesh.Vertices[i].UV!.Value.Z, gameMesh.Vertices[i].UV!.Value.W * -1));
                }

                var colorArray = GeometryElement.VertexColor.GetDirectArray(colorElement);
                Layer.Color_Add(colorArray, color);

                var tangent1Array = GeometryElement.Tangent.GetDirectArray(tangentElem1);
                Layer.Tangent_Add(tangent1Array, tangent1);
                var tangent2Array = GeometryElement.Tangent.GetDirectArray(tangentElem2);
                Layer.Tangent_Add(tangent2Array, tangent2);
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
