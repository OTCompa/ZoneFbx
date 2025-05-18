using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class SurfacePhong
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxSurfacePhong_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create(IntPtr contextManager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxSurfacePhong_SetFactor")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetFactor(IntPtr surfacePhong, double specularFactor, double normalFactor);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxSurfacePhong_ConnectSrcObject")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void ConnectSrcObject(IntPtr outsurface, IntPtr texture, int branch);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxSurfacePhong_PropertyExists")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool PropertyExists(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string propertyName);
    }
}
