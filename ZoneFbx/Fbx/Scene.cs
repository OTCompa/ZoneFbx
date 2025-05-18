using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class Scene
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxScene_GetRootNode")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr GetRootNode(IntPtr contextManager);
    }
}
