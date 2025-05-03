using System.Runtime.InteropServices;

namespace ZoneFbx
{
    internal static partial class Fbx
    {
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxManager_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Manager_Create();

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxManager_Destroy")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Manager_Destroy(IntPtr manager);

        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxManager_Initialize")]
        public static extern bool Manager_Initialize(IntPtr manager);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxScene_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Scene_Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxScene_GetRootNode")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Scene_GetRootNode(IntPtr scene);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Node_Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_SetLclTranslation")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Node_SetLclTranslation(IntPtr node, double pData0, double pData1, double pData2);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_AddChild")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Node_AddChild(IntPtr node, IntPtr child);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Mesh_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_Init")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Mesh_Init(IntPtr mesh, int length);


        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementVertexColor_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr GeometryElementVertexColor_Create(IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementVertexColor_SetMappingNode")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void GeometryElementVertexColor_SetMappingNode(IntPtr element);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementVertexColor_GetDirectArray")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr GeometryElementVertexColor_GetDirectArray(IntPtr element);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerColor_Add")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void LayerColor_Add(IntPtr directArray, IntPtr color);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementUV_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr GeometryElementUV_Create(IntPtr mesh, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementUV_SetMappingNode")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void GeometryElementUV_SetMappingNode(IntPtr element);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementUV_GetDirectArray")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr GeometryElementUV_GetDirectArray(IntPtr element);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerUV_Add")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void LayerUV_Add(IntPtr directArray, IntPtr vector);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementTangent_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr GeometryElementTangent_Create(IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementTangent_SetMappingNode")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void GeometryElementTangent_SetMappingNode(IntPtr element);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxGeometryElementTangent_GetDirectArray")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr GeometryElementTangent_GetDirectArray(IntPtr element);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLayerTangent_Add")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void LayerTangent_Add(IntPtr directArray, IntPtr vector);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxVector4_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Vector4_Create(double X, double Y, double Z, double W);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxVector2_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Vector2_Create(double X, double Y);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_SetControlPointAt")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Mesh_SetControlPointAt(IntPtr mesh, IntPtr a, IntPtr b, int i);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_BeginPolygon")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Mesh_BeginPolygon(IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_AddPolygon")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Mesh_AddPolygon(IntPtr mesh, int i);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxMesh_EndPolygon")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Mesh_EndPolygon(IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxSurfacePhong_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr SurfacePhong_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxSurfacePhong_SetFactor")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SurfacePhong_SetFactor(IntPtr surfacePhong);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxFileTexture_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr FileTexture_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxFileTexture_SetStuff")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void FileTexture_SetStuff(IntPtr texture, [MarshalAs(UnmanagedType.LPStr)] string pathname);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxSurfacePhong_ConnectSrcObject")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void SurfacePhong_ConnectSrcObject(IntPtr outsurface, IntPtr texture, int branch);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_AddMaterial")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Node_AddMaterial(IntPtr node, IntPtr material);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_SetNodeAttribute")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Node_SetNodeAttribute(IntPtr node, IntPtr mesh);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_SetStuff")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Node_SetStuff(IntPtr node, double X, double Y, double Z, int branch);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxExporter_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Exporter_Create(IntPtr manager, [MarshalAs(UnmanagedType.LPStr)] string name);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxExporter_Initialize")]
        public static extern bool Exporter_Initialize(IntPtr exporter, [MarshalAs(UnmanagedType.LPStr)] string out_path, IntPtr manager);
        [DllImport("FBXCSharp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "FbxExporter_Export")]
        public static extern bool Exporter_Export(IntPtr exporter, IntPtr scene);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxExporter_Destroy")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Exporter_Destroy(IntPtr exporter);


        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_Create")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr Light_Create(IntPtr scene, [MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetColor")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Light_SetColor(IntPtr light, double X, double Y, double Z);

        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetIntensity")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Light_SetIntensity(IntPtr light, double intensity);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetLightType")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Light_SetLightType(IntPtr light, int type);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetDecay")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Light_SetDecay(IntPtr light, int decay);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_CastShadows")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Light_CastShadows(IntPtr light);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxLight_SetAngle")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Light_SetAngle(IntPtr light, double falloff, double hotspot);
        [LibraryImport("FBXCSharp.dll", EntryPoint = "FbxNode_SetPostTargetRotation")]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void Node_SetPostTargetRotation(IntPtr node);
    }
}