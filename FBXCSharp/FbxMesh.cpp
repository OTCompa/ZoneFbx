#include "pch.h"
#include <fbxsdk.h>
#include "ContextManager.h"

extern "C" {
    FBXCSHARP_API FbxMesh* FbxMesh_Create(ContextManager* contextManager, const char* name) {
        return FbxMesh::Create(contextManager->pScene, name);
    }

    FBXCSHARP_API void FbxMesh_Delete(FbxMesh* mesh) {
        mesh->Destroy();
    }

    FBXCSHARP_API void FbxMesh_Init(FbxMesh* mesh, int length) {
        mesh->InitControlPoints(length);
        mesh->InitNormals(length);
    }

    FBXCSHARP_API void FbxMesh_InitControlPoints(FbxMesh* mesh, int length) {
        mesh->InitControlPoints(length);
    }

    FBXCSHARP_API void FbxMesh_SetControlPointAtNoNormals(FbxMesh* mesh, double x, double y, double z, double w, int i) {
        mesh->SetControlPointAt(FbxVector4(x, y, z, w), i);
    }

    FBXCSHARP_API void FbxMesh_SetControlPointAt(FbxMesh* mesh, double ax, double ay, double az, double aw, double bx, double by, double bz, double bw, int i) {
        mesh->SetControlPointAt(FbxVector4(ax, ay, az, aw), FbxVector4(bx, by, bz, bw), i);
    }

    FBXCSHARP_API void FbxMesh_BeginPolygon(FbxMesh* mesh) {
        mesh->BeginPolygon();
    }

    FBXCSHARP_API void FbxMesh_AddPolygon(FbxMesh* mesh, int i) {
        mesh->AddPolygon(i);
    }

    FBXCSHARP_API void FbxMesh_EndPolygon(FbxMesh* mesh) {
        mesh->EndPolygon();
    }

    FBXCSHARP_API FbxLayer* FbxMesh_GetLayer(FbxMesh* mesh, int num) {
        return mesh->GetLayer(num);
    }
}