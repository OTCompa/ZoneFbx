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
        internal static partial class VertexColor
        {
            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementVertexColor_Create")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial IntPtr Create(IntPtr mesh);

            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementVertexColor_SetMappingNode")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial void SetMappingNode(IntPtr element);

            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementVertexColor_GetDirectArray")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial IntPtr GetDirectArray(IntPtr element);
        }

        internal static partial class UV
        {
            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementUV_Create")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial IntPtr Create(IntPtr mesh, [MarshalAs(UnmanagedType.LPStr)] string name);

            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementUV_SetMappingNode")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial void SetMappingNode(IntPtr element);

            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementUV_GetDirectArray")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial IntPtr GetDirectArray(IntPtr element);

        }

        internal static partial class Tangent
        {
            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementTangent_Create")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial IntPtr Create(IntPtr mesh);

            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementTangent_SetMappingNode")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial void SetMappingNode(IntPtr element);

            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementTangent_GetDirectArray")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial IntPtr GetDirectArray(IntPtr element);
        }

    }
}
