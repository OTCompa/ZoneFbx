using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoneFbx.Fbx
{
    internal static partial class Property
    {
        public enum EFlags
        {
            eNone = 0,
            eStatic = 1 << 0,
            eAnimatable = 1 << 1,
            eAnimated = 1 << 2,
            eImported = 1 << 3,
            eUserDefined = 1 << 4,
            eHidden = 1 << 5,
            eNotSavable = 1 << 6,

            eLockedMember0 = 1 << 7,
            eLockedMember1 = 1 << 8,
            eLockedMember2 = 1 << 9,
            eLockedMember3 = 1 << 10,
            eLockedAll = eLockedMember0 | eLockedMember1 | eLockedMember2 | eLockedMember3,
            eMutedMember0 = 1 << 11,
            eMutedMember1 = 1 << 12,
            eMutedMember2 = 1 << 13,
            eMutedMember3 = 1 << 14,
            eMutedAll = eMutedMember0 | eMutedMember1 | eMutedMember2 | eMutedMember3,
        }

        //[LibraryImport("FBXCSharp.dll", EntryPoint = "FbxProperty_Create")]
        //[UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        //public static partial IntPtr Create(IntPtr obj, int dataType, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxProperty_CreateString")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void CreateString(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string value);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxProperty_SetString")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SetString(IntPtr property, [MarshalAs(UnmanagedType.LPStr)] string value);


        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxProperty_ModifyFlag")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void ModifyFlag(IntPtr property, EFlags flag, [MarshalAs(UnmanagedType.I1)] bool value);


        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxProperty_Destroy")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Destroy(IntPtr property);
    }
}
