using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class LayeredTexture
    {
        public enum EBlendMode
        {
            eTranslucent,
            eAdditive,
            eModulate,
            eModulate2,
            eOver,
            eNormal,
            eDissolve,
            eDarken,
            eColorBurn,
            eLinearBurn,
            eDarkerColor,
            eLighten,
            eScreen,
            eColorDodge,
            eLinearDodge,
            eLighterColor,
            eSoftLight,
            eHardLight,
            eVividLight,
            eLinearLight,
            ePinLight,
            eHardMix,
            eDifference,
            eExclusion,
            eSubtract,
            eDivide,
            eHue,
            eSaturation,
            eColor,
            eLuminosity,
            eOverlay,
            eBlendModeCount
        };

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayeredTexture_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Create(IntPtr contextManager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayeredTexture_ConnectSrcObject")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr ConnectSrcObject(IntPtr layeredTexture, IntPtr obj);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayeredTexture_SetTextureBlendMode")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr SetTextureBlendMode(IntPtr layeredTexture, int index, EBlendMode blendMode);
    }
}
