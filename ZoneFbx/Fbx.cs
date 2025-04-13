using System.Runtime.InteropServices;

namespace ZoneFbx
{
    internal static class Fbx
    {
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxManager_Create();

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxManager_Destroy(IntPtr manager);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FbxManager_Initialize(IntPtr manager);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxScene_Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxScene_GetRootNode(IntPtr scene);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxNode_Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxNode_SetLclTranslation(IntPtr node, double pData0, double pData1, double pData2);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxNode_AddChild(IntPtr node, IntPtr child);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxMesh_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxMesh_Init(IntPtr mesh, int length);


        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxGeometryElementVertexColor_Create(IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxGeometryElementVertexColor_SetMappingNode(IntPtr element);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxGeometryElementVertexColor_GetDirectArray(IntPtr element);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxLayerColor_Add(IntPtr directArray, IntPtr color);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxGeometryElementUV_Create(IntPtr mesh, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxGeometryElementUV_SetMappingNode(IntPtr element);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxGeometryElementUV_GetDirectArray(IntPtr element);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxLayerUV_Add(IntPtr directArray, IntPtr vector);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxGeometryElementTangent_Create(IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxGeometryElementTangent_SetMappingNode(IntPtr element);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxGeometryElementTangent_GetDirectArray(IntPtr element);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxLayerTangent_Add(IntPtr directArray, IntPtr vector);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxVector4_Create(double X, double Y, double Z, double W);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxVector2_Create(double X, double Y);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxMesh_SetControlPointAt(IntPtr mesh, IntPtr a, IntPtr b, int i);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxMesh_BeginPolygon(IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxMesh_AddPolygon(IntPtr mesh, int i);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxMesh_EndPolygon(IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxSurfacePhong_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxSurfacePhong_SetFactor(IntPtr surfacePhong);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxFileTexture_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxFileTexture_SetStuff(IntPtr texture, [MarshalAs(UnmanagedType.LPStr)] string pathname);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxSurfacePhong_ConnectSrcObject(IntPtr outsurface, IntPtr texture, int branch);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxNode_AddMaterial(IntPtr node, IntPtr material);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxNode_SetNodeAttribute(IntPtr node, IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxNode_SetStuff(IntPtr node, double X, double Y, double Z, int branch);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxExporter_Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FbxExporter_Initialize(IntPtr exporter, [MarshalAs(UnmanagedType.LPStr)] string out_path, IntPtr manager);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FbxExporter_Export(IntPtr exporter, IntPtr scene);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxExporter_Destroy(IntPtr exporter);


        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FbxLight_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxLight_SetColor(IntPtr light, double X, double Y, double Z);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxLight_SetIntensity(IntPtr light, double intensity);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxLight_SetLightType(IntPtr light, int type);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxLight_SetDecay(IntPtr light, int decay);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxLight_CastShadows(IntPtr light);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxLight_SetAngle(IntPtr light, double falloff, double hotspot);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FbxNode_SetPostTargetRotation(IntPtr node);
    }
}