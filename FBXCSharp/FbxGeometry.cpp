#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxGeometryElementVertexColor* FbxGeometryElementVertexColor_Create(FbxMesh* mesh) {
        return mesh->CreateElementVertexColor();
    }

    __declspec(dllexport) void FbxGeometryElementVertexColor_SetMappingNode(FbxGeometryElementVertexColor* element) {
        element->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
    }

    __declspec(dllexport) FbxLayerElementArrayTemplate<FbxColor>* FbxGeometryElementVertexColor_GetDirectArray(FbxGeometryElementVertexColor* element) {
        return &(element->GetDirectArray());
    }


    __declspec(dllexport) FbxGeometryElementUV* FbxGeometryElementUV_Create(FbxMesh* mesh, const char* name) {
        return mesh->CreateElementUV(name);
    }

    __declspec(dllexport) void FbxGeometryElementUV_SetMappingNode(FbxGeometryElementUV* element) {
        element->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
    }

    __declspec(dllexport) FbxLayerElementArrayTemplate<FbxVector2>* FbxGeometryElementUV_GetDirectArray(FbxGeometryElementUV* element) {
        return &(element->GetDirectArray());
    }


    __declspec(dllexport) FbxGeometryElementTangent* FbxGeometryElementTangent_Create(FbxMesh* mesh) {
        return mesh->CreateElementTangent();
    }

    __declspec(dllexport) void FbxGeometryElementTangent_SetMappingNode(FbxGeometryElementTangent* element) {
        element->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
    }

    __declspec(dllexport) FbxLayerElementArrayTemplate<FbxVector4>* FbxGeometryElementTangent_GetDirectArray(FbxGeometryElementTangent* element) {
        return &(element->GetDirectArray());
    }
}