using System.Runtime.InteropServices;

namespace ZoneFbx
{
    internal static class Fbx
    {
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxManager_Create")]
        public static extern IntPtr Manager_Create();

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxManager_Destroy")]
        public static extern void Manager_Destroy(IntPtr manager);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxManager_Initialize")]
        public static extern bool Manager_Initialize(IntPtr manager);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxScene_Create")]
        public static extern IntPtr Scene_Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxScene_GetRootNode")]
        public static extern IntPtr Scene_GetRootNode(IntPtr scene);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxNode_Create")]
        public static extern IntPtr Node_Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxNode_SetLclTranslation")]
        public static extern void Node_SetLclTranslation(IntPtr node, double pData0, double pData1, double pData2);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxNode_AddChild")]
        public static extern void Node_AddChild(IntPtr node, IntPtr child);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxMesh_Create")]
        public static extern IntPtr Mesh_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxMesh_Init")]
        public static extern void Mesh_Init(IntPtr mesh, int length);


        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxGeometryElementVertexColor_Create")]
        public static extern IntPtr GeometryElementVertexColor_Create(IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxGeometryElementVertexColor_SetMappingNode")]
        public static extern void GeometryElementVertexColor_SetMappingNode(IntPtr element);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxGeometryElementVertexColor_GetDirectArray")]
        public static extern IntPtr GeometryElementVertexColor_GetDirectArray(IntPtr element);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLayerColor_Add")]
        public static extern void LayerColor_Add(IntPtr directArray, IntPtr color);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxGeometryElementUV_Create")]
        public static extern IntPtr GeometryElementUV_Create(IntPtr mesh, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxGeometryElementUV_SetMappingNode")]
        public static extern void GeometryElementUV_SetMappingNode(IntPtr element);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxGeometryElementUV_GetDirectArray")]
        public static extern IntPtr GeometryElementUV_GetDirectArray(IntPtr element);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLayerUV_Add")]
        public static extern void LayerUV_Add(IntPtr directArray, IntPtr vector);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxGeometryElementTangent_Create")]
        public static extern IntPtr GeometryElementTangent_Create(IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxGeometryElementTangent_SetMappingNode")]
        public static extern void GeometryElementTangent_SetMappingNode(IntPtr element);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxGeometryElementTangent_GetDirectArray")]
        public static extern IntPtr GeometryElementTangent_GetDirectArray(IntPtr element);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLayerTangent_Add")]
        public static extern void LayerTangent_Add(IntPtr directArray, IntPtr vector);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxVector4_Create")]
        public static extern IntPtr Vector4_Create(double X, double Y, double Z, double W);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxVector2_Create")]
        public static extern IntPtr Vector2_Create(double X, double Y);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxMesh_SetControlPointAt")]
        public static extern void Mesh_SetControlPointAt(IntPtr mesh, IntPtr a, IntPtr b, int i);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxMesh_BeginPolygon")]
        public static extern void Mesh_BeginPolygon(IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxMesh_AddPolygon")]
        public static extern void Mesh_AddPolygon(IntPtr mesh, int i);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxMesh_EndPolygon")]
        public static extern void Mesh_EndPolygon(IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxSurfacePhong_Create")]
        public static extern IntPtr SurfacePhong_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxSurfacePhong_SetFactor")]
        public static extern void SurfacePhong_SetFactor(IntPtr surfacePhong);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxFileTexture_Create")]
        public static extern IntPtr FileTexture_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxFileTexture_SetStuff")]
        public static extern void FileTexture_SetStuff(IntPtr texture, [MarshalAs(UnmanagedType.LPStr)] string pathname);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxSurfacePhong_ConnectSrcObject")]
        public static extern void SurfacePhong_ConnectSrcObject(IntPtr outsurface, IntPtr texture, int branch);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxNode_AddMaterial")]
        public static extern void Node_AddMaterial(IntPtr node, IntPtr material);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxNode_SetNodeAttribute")]
        public static extern void Node_SetNodeAttribute(IntPtr node, IntPtr mesh);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxNode_SetStuff")]
        public static extern void Node_SetStuff(IntPtr node, double X, double Y, double Z, int branch);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxExporter_Create")]
        public static extern IntPtr Exporter_Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxExporter_Initialize")]
        public static extern bool Exporter_Initialize(IntPtr exporter, [MarshalAs(UnmanagedType.LPStr)] string out_path, IntPtr manager);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxExporter_Export")]
        public static extern bool Exporter_Export(IntPtr exporter, IntPtr scene);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxExporter_Destroy")]
        public static extern void Exporter_Destroy(IntPtr exporter);


        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLight_Create")]
        public static extern IntPtr Light_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLight_SetColor")]
        public static extern void Light_SetColor(IntPtr light, double X, double Y, double Z);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLight_SetIntensity")]
        public static extern void Light_SetIntensity(IntPtr light, double intensity);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLight_SetLightType")]
        public static extern void Light_SetLightType(IntPtr light, int type);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLight_SetDecay")]
        public static extern void Light_SetDecay(IntPtr light, int decay);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLight_CastShadows")]
        public static extern void Light_CastShadows(IntPtr light);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxLight_SetAngle")]
        public static extern void Light_SetAngle(IntPtr light, double falloff, double hotspot);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxNode_SetPostTargetRotation")]
        public static extern void Node_SetPostTargetRotation(IntPtr node);
    }
}