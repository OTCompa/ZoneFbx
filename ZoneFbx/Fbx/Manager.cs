using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class Manager
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxManager_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create();

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxManager_Destroy")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Destroy(IntPtr manager);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxManager_Initialize")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Initialize(IntPtr manager);
    }
}
