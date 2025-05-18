using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class Vector4
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxVector4_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create(double X, double Y, double Z, double W);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxVector4_Destroy")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Destroy(IntPtr v);

    }
    internal static partial class Vector2
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxVector2_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create(double X, double Y);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxVector2_Destroy")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Destroy(IntPtr v);
    }
}
