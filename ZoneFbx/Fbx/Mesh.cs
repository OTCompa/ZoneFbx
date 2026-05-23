using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class Mesh
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create(IntPtr contextManager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_Init")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Init(IntPtr mesh, int length);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_InitControlPoints")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void InitControlPoints(IntPtr mesh, int length);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_SetControlPointAtNoNormals")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetControlPointAt(IntPtr mesh, double ax, double ay, double az, double aw, int i);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_SetControlPointAt")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetControlPointAt(IntPtr mesh, double ax, double ay, double az, double aw, double bx, double by, double bz, double bw, int i);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_BeginPolygon")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void BeginPolygon(IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_AddPolygon")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void AddPolygon(IntPtr mesh, int i);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_EndPolygon")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void EndPolygon(IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_GetLayer")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr GetLayer(IntPtr mesh, int num);
    }
}
