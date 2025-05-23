﻿using Lumina.Data.Files.Pcb;
using ZoneFbx.Fbx;

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
            return true;
            
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
                var pos = Vector4.Create(v.X, v.Y, v.Z, 0);
                ContextManager.CppVector4ToFree.Add(pos);
                Fbx.Mesh.SetControlPointAt(mesh, pos, meshIndex++);
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
