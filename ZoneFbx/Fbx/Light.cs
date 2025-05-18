using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZoneFbx.Fbx;

namespace ZoneFbx.Fbx
{
    internal static partial class Light
    {
        public enum EType
        {
            ePoint,
            eDirectional,
            eSpot,
            eArea,
            eVolume
        };

        public enum EDecayType
        {
            eNone,
            eLinear,
            eQuadratic,
            eCubic
        };

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create(IntPtr contextManager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetColor")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetColor(IntPtr light, double X, double Y, double Z);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetIntensity")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetIntensity(IntPtr light, double intensity);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetLightType")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetLightType(IntPtr light, EType type);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetDecay")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetDecay(IntPtr light, EDecayType decay);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_CastShadows")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void CastShadows(IntPtr light);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetAngle")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetAngle(IntPtr light, double falloff, double hotspot);

    }
}
