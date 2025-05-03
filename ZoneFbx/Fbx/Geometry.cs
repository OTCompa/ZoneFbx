using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class GeometryElement
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementVertexColor_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr VertexColor_Create(IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementVertexColor_SetMappingNode")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void VertexColor_SetMappingNode(IntPtr element);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementVertexColor_GetDirectArray")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr VertexColor_GetDirectArray(IntPtr element);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementUV_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr UV_Create(IntPtr mesh, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementUV_SetMappingNode")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void UV_SetMappingNode(IntPtr element);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementUV_GetDirectArray")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr UV_GetDirectArray(IntPtr element);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementTangent_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Tangent_Create(IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementTangent_SetMappingNode")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Tangent_SetMappingNode(IntPtr element);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementTangent_GetDirectArray")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Tangent_GetDirectArray(IntPtr element);
    }
}
