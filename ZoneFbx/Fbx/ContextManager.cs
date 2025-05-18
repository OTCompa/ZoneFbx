using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal partial class ContextManager
    {
        public List<IntPtr> CppVector4ToFree = [];
        public List<IntPtr> CppVector2ToFree = [];

        public void CppVectorCleanup()
        {
            // there's probably some way to pass an array of pointers into c++ to do delete[] but this will do for now.
            if (CppVector4ToFree.Count > 0)
            {
                foreach(var v in CppVector4ToFree)
                {
                    Vector4.Destroy(v);
                }
                CppVector4ToFree.Clear();
            }

            if (CppVector2ToFree.Count > 0)
            {
                foreach(var v in CppVector2ToFree)
                {
                    Vector2.Destroy(v);
                }
                CppVector2ToFree.Clear();
            }
        }

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
