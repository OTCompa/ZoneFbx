#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"
#include "ContextManager.h"

extern "C" {
    FBXCSHARP_API FbxMesh* FbxMesh_Create(ContextManager* contextManager, const char* name) {
        return FbxMesh::Create(contextManager->pScene, name);
    }

    FBXCSHARP_API void FbxMesh_Init(FbxMesh* mesh, int length) {
        mesh->InitControlPoints(length);
        mesh->InitNormals(length);
    }

    FBXCSHARP_API void FbxMesh_InitControlPoints(FbxMesh* mesh, int length) {
        mesh->InitControlPoints(length);
    }

    FBXCSHARP_API void FbxMesh_SetControlPointAtNoNormals(FbxMesh* mesh, FbxVector4* a, int i) {
        mesh->SetControlPointAt(*a, i);
    }

    FBXCSHARP_API void FbxMesh_SetControlPointAt(FbxMesh* mesh, FbxVector4* a, FbxVector4* b, int i) {
        mesh->SetControlPointAt(*a, *b, i);
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