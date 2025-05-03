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
