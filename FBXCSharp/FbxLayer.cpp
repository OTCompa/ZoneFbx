#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) void FbxLayerColor_Add(FbxLayerElementArrayTemplate<FbxColor>* dArray, FbxColor* color) {
        dArray->Add(*color);
    }
    __declspec(dllexport) void FbxLayerUV_Add(FbxLayerElementArrayTemplate<FbxVector2>* dArray, FbxVector2* vector) {
        dArray->Add(*vector);
    }

    __declspec(dllexport) void FbxLayerTangent_Add(FbxLayerElementArrayTemplate<FbxVector4>* dArray, FbxVector4* vector) {
        dArray->Add(*vector);
    }

    __declspec(dllexport) void FbxLayerMaterial_Add(FbxLayerElementArrayTemplate<int>* dArray, int num) {
        dArray->Add(num);
    }

    __declspec(dllexport) void FbxLayer_SetMaterials(FbxLayer* layer, FbxLayerElementMaterial* material) {
        layer->SetMaterials(material);
    }

    __declspec(dllexport) FbxLayerElementMaterial* FbxLayerElementMaterial_Create(FbxMesh* mesh, const char* name) {
        return FbxLayerElementMaterial::Create(mesh, name);
    }

    __declspec(dllexport) void FbxLayerElementMaterial_SetMappingMode(FbxLayerElementMaterial* element) {
        element->SetMappingMode(FbxLayerElement::eAllSame);
    }

    __declspec(dllexport) void FbxLayerElementMaterial_SetReferenceMode(FbxLayerElementMaterial* element) {
        element->SetReferenceMode(FbxLayerElement::eDirect);
    }

    __declspec(dllexport) FbxLayerElementArrayTemplate<int>* FbxLayerElementMaterial_GetIndexArray(FbxLayerElementMaterial* element) {
        return &(element->GetIndexArray());
    }
}