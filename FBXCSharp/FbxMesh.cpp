#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxMesh* FbxMesh_Create(FbxScene* scene, const char* name) {
        return FbxMesh::Create(scene, name);
    }

    __declspec(dllexport) void FbxMesh_Init(FbxMesh* mesh, int length) {
        mesh->InitControlPoints(length);
        mesh->InitNormals(length);
    }

    __declspec(dllexport) void FbxMesh_SetControlPointAt(FbxMesh* mesh, FbxVector4* a, FbxVector4* b, int i) {
        mesh->SetControlPointAt(*a, *b, i);
    }

    __declspec(dllexport) void FbxMesh_BeginPolygon(FbxMesh* mesh) {
        mesh->BeginPolygon();
    }

    __declspec(dllexport) void FbxMesh_AddPolygon(FbxMesh* mesh, int i) {
        mesh->AddPolygon(i);
    }

    __declspec(dllexport) void FbxMesh_EndPolygon(FbxMesh* mesh) {
        mesh->EndPolygon();
    }
}