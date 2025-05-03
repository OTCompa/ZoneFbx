using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal partial class Node
    {
        // not really sure how to handle Fbx.Scene.GetRootNode()
        // i'll figure it out soon:tm: if i really care about doing this

        //internal IntPtr nodePointer { get; private set; }
        //internal IntPtr managerPointer { get; private set; }
        //internal string name { get; private set; }

        //public Node(IntPtr manager, string name)
        //{
        //    managerPointer = manager;
        //    this.name = name;
        //    nodePointer = _Create(manager, name);
        //}

        //public void SetLclTranslation(double pData0, double pData1, double pData2)
        //{
        //    _SetLclTranslation(nodePointer, pData0, pData1, pData2);
        //}

        //public void AddChild(IntPtr child)  // TODO: Change to Node
        //{
        //    _AddChild(nodePointer, child);
        //}

        //public void AddMaterial(IntPtr material)  // TODO: Change to SurfacePhong
        //{
        //    _AddMaterial(nodePointer, material);
        //}

        //public void SetNodeAttribute(IntPtr mesh)  // TODO: Change to Mesh
        //{
        //    _SetNodeAttribute(nodePointer, mesh);
        //}

        //public void SetStuff(double X, double Y, double Z, int branch)  // TODO: refactor out branch
        //{
        //    _SetStuff(nodePointer, X, Y, Z, branch);
        //}

        //public void SetPostTargetRotation()
        //{
        //    _SetPostTargetRotation(nodePointer);
        //}

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_SetLclTranslation")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetLclTranslation(IntPtr node, double pData0, double pData1, double pData2);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_AddChild")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void AddChild(IntPtr node, IntPtr child);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_AddMaterial")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void AddMaterial(IntPtr node, IntPtr material);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_SetNodeAttribute")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetNodeAttribute(IntPtr node, IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_SetStuff")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetStuff(IntPtr node, double X, double Y, double Z, int branch);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_SetPostTargetRotation")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetPostTargetRotation(IntPtr node);
    }
}
