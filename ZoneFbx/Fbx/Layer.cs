using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class Layer
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerColor_Add")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Color_Add(IntPtr directArray, IntPtr color);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerUV_Add")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void UV_Add(IntPtr directArray, IntPtr vector);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerTangent_Add")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Tangent_Add(IntPtr directArray, IntPtr vector);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerMaterial_Add")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Material_Add(IntPtr directArray, int num);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayer_SetMaterials")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetMaterials(IntPtr layer, IntPtr material);

        internal static partial class ElementMaterial
        {
            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerElementMaterial_Create")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial IntPtr Create(IntPtr mesh, [MarshalAs(UnmanagedType.LPStr)] string name);

            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerElementMaterial_SetMappingMode")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial void SetMappingMode(IntPtr element);
            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerElementMaterial_SetReferenceMode")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial void SetReferenceMode(IntPtr element);
            [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerElementMaterial_GetIndexArray")]
            [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
            public static partial IntPtr GetIndexArray(IntPtr element);
        }
    }
}
