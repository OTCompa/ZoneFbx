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
    }
}
