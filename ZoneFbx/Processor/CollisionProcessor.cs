using Lumina.Data.Files;
using Lumina.Data.Files.Pcb;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ZoneFbx.Fbx;
using static Lumina.Models.Models.Model;

namespace ZoneFbx.Processor
{
    internal class CollisionProcessor(Lumina.GameData data, IntPtr contextManager, ZoneExporter.Options options, ContextManager ContextManager, string zonePath) : Processor(data, contextManager, options)
    {
        private readonly string zonePath = zonePath;

        public void ProcessList()
        {
            string listPath = $"bg/{zonePath[..zonePath.LastIndexOf("/level")]}/collision/";
            string listFilePath = $"{listPath}list.pcb";
            if (!data.FileExists(listFilePath)) return;

            var listFile = data.GetFile<PcbListFile>(listFilePath);
            if (listFile == null || listFile.Nodes.Children == null) return;

            var listNode = Node.Create(contextManager, "collision_list");

            foreach (var node in listFile.Nodes.Children)
            {
                var objectFilename = $"tr{node.Id:D4}.pcb";
                var objectFilepath = $"{listPath}{objectFilename}";

                var collision = data.GetFile<PcbResourceFile>(objectFilepath);
                if (collision == null)
                {
                    Console.WriteLine("sadge");
                    continue;
                }

                var pcbMesh = createCollisionMesh(collision, Path.GetFileNameWithoutExtension(objectFilename));
                if (pcbMesh == IntPtr.Zero) continue;

                var meshNode = Node.Create(contextManager, Path.GetFileNameWithoutExtension(objectFilename));
                Node.SetNodeAttribute(meshNode, pcbMesh);
                Node.AddChild(listNode, meshNode);
            }

            var rootNode = Scene.GetRootNode(contextManager);
            Node.AddChild(rootNode, listNode);
        }

        public bool ProcessCollisionAsset(string collisionAssetPath, IntPtr node)
        {
            if (string.IsNullOrEmpty(collisionAssetPath)) return false;
            var collision = data.GetFile<PcbResourceFile>(collisionAssetPath);
            if (collision == null) return false;

            var mesh = createCollisionMesh(collision, Path.GetFileNameWithoutExtension(collisionAssetPath));
            if (mesh == IntPtr.Zero) return false;

            Node.SetNodeAttribute(node, mesh);

            if (options.enableCollisionVariants)
            {
                var variant = GetVariantIndex(collisionAssetPath);
                if (variant != -1)
                {
                    while (true)
                    {
                        var variantAssetPath = GetVariant(collisionAssetPath, ++variant);
                        if (string.IsNullOrEmpty(variantAssetPath)) break;
                        var collisionVariant = data.GetFile<PcbResourceFile>(variantAssetPath);
                        if (collisionVariant == null) break;
                        var variantMesh = createCollisionMesh(collisionVariant, Path.GetFileNameWithoutExtension(variantAssetPath));
                        if (variantMesh == IntPtr.Zero) break;

                        var variantNode = Node.Create(contextManager, Path.GetFileNameWithoutExtension(variantAssetPath));
                        Node.SetNodeAttribute(variantNode, variantMesh);
                        Node.AddChild(node, variantNode);
                    }
                }
            }

            return true;
            
        }

        public int GetVariantIndex(string collisionAssetPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(collisionAssetPath);
            var match = Regex.Match(fileName, @"\d+$");

            if (match.Success)
            {
                if (int.TryParse(match.Value, out int result))
                {
                    return result;
                }
            }

            return -1;
        }

        public string GetVariant(string collisionAssetPath, int variant)
        {
            string directory = Path.GetDirectoryName(collisionAssetPath);
            string fileName = Path.GetFileNameWithoutExtension(collisionAssetPath);
            string extension = Path.GetExtension(collisionAssetPath);
            var regex = new Regex(@"\d+$");
            if (!regex.IsMatch(fileName)) return "";

            fileName = regex.Replace(fileName, variant.ToString()) + ".pcb";
            return Path.Combine(directory ?? "", fileName).Replace("\\", "/");
            
        }

        public IntPtr createCollisionMesh(PcbResourceFile collisionFile, string name)
        {
            var mesh = Fbx.Mesh.Create(contextManager, name);
            var totalVertices = 0;
            //Console.WriteLine($"{collisionFile.Nodes.TotalNodes.ToString()}, {collisionFile.Nodes.TotalPolygons.ToString()}");

            //return IntPtr.Zero;
            if (collisionFile == null) return IntPtr.Zero;
            foreach (var resourceNode in collisionFile.Nodes.Children)
            {
                totalVertices += recursiveGetTotalVertices(resourceNode);
            }

            Fbx.Mesh.InitControlPoints(mesh, totalVertices);

            int meshIndex = 0;
            foreach (var resourceNode in collisionFile.Nodes.Children)
            {
                recursiveFormCollisionMesh(resourceNode, mesh, ref meshIndex);
            }

            return mesh;
        }

        private int recursiveGetTotalVertices(PcbResourceFile.ResourceNode resourceNode)
        {
            int totalVertices = 0;

            if (resourceNode.Children != null)
            {
                foreach (var childResourceNode in resourceNode.Children)
                {
                    totalVertices += recursiveGetTotalVertices(childResourceNode);
                }
            }

            totalVertices += resourceNode.Vertices.Length;

            return totalVertices;
        }

        private void recursiveFormCollisionMesh(PcbResourceFile.ResourceNode resourceNode, IntPtr mesh, ref int meshIndex)
        {
            var baseIndex = meshIndex;
            foreach (var v in resourceNode.Vertices)
            {
                Fbx.Mesh.SetControlPointAt(mesh, v.X, v.Y, v.Z, 0, meshIndex++);
            }

            foreach (var p in resourceNode.Polygons)
            {
                Fbx.Mesh.BeginPolygon(mesh);
                Fbx.Mesh.AddPolygon(mesh, baseIndex + p.VertexIndex[0]);
                Fbx.Mesh.AddPolygon(mesh, baseIndex + p.VertexIndex[1]);
                Fbx.Mesh.AddPolygon(mesh, baseIndex + p.VertexIndex[2]);
                Fbx.Mesh.EndPolygon(mesh);
            }

            if (resourceNode.Children != null)
            {
                foreach (var childResourceNode in resourceNode.Children) recursiveFormCollisionMesh(childResourceNode, mesh, ref meshIndex);
            }
        }
    }
}
