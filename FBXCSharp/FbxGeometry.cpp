#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"

extern "C" {
    FBXCSHARP_API FbxGeometryElementVertexColor* FbxGeometryElementVertexColor_Create(FbxMesh* mesh) {
        return mesh->CreateElementVertexColor();
    }

    FBXCSHARP_API void FbxGeometryElementVertexColor_SetMappingNode(FbxGeometryElementVertexColor* element) {
        element->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
    }

    FBXCSHARP_API FbxLayerElementArrayTemplate<FbxColor>* FbxGeometryElementVertexColor_GetDirectArray(FbxGeometryElementVertexColor* element) {
        return &(element->GetDirectArray());
    }


    FBXCSHARP_API FbxGeometryElementUV* FbxGeometryElementUV_Create(FbxMesh* mesh, const char* name) {
        return mesh->CreateElementUV(name);
    }

    FBXCSHARP_API void FbxGeometryElementUV_SetMappingNode(FbxGeometryElementUV* element) {
        element->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
    }

    FBXCSHARP_API FbxLayerElementArrayTemplate<FbxVector2>* FbxGeometryElementUV_GetDirectArray(FbxGeometryElementUV* element) {
        return &(element->GetDirectArray());
    }


    FBXCSHARP_API FbxGeometryElementTangent* FbxGeometryElementTangent_Create(FbxMesh* mesh) {
        return mesh->CreateElementTangent();
    }

    FBXCSHARP_API void FbxGeometryElementTangent_SetMappingNode(FbxGeometryElementTangent* element) {
        element->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
    }

    FBXCSHARP_API FbxLayerElementArrayTemplate<FbxVector4>* FbxGeometryElementTangent_GetDirectArray(FbxGeometryElementTangent* element) {
        return &(element->GetDirectArray());
    }
}