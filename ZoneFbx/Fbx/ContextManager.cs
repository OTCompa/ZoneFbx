using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class ContextManager
    {

        [LibraryImport("FBXCSharp.dll", EntryPoint = "ContextManager_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create();

        [LibraryImport("FBXCSharp.dll", EntryPoint = "ContextManager_Destroy")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Destroy(IntPtr contextManager);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "ContextManager_CreateManager")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void CreateManager(IntPtr contextManager);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "ContextManager_DestroyManager")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void DestroyManager(IntPtr contextManager);


        [LibraryImport("FBXCSharp.dll", EntryPoint = "ContextManager_CreateScene")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void CreateScene(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "ContextManager_DestroyScene")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void DestroyScene(IntPtr contextManager);

    }
}
