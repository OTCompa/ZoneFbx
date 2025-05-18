using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class Exporter
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxExporter_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create(IntPtr contextManager, [MarshalAs(UnmanagedType.LPStr)] string name);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxExporter_Initialize")]
        public static extern bool Initialize(IntPtr exporter, [MarshalAs(UnmanagedType.LPStr)] string out_path, IntPtr manager);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxExporter_Export")]
        public static extern bool Export(IntPtr exporter, IntPtr scene);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxExporter_Destroy")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Destroy(IntPtr exporter);

    }
}
