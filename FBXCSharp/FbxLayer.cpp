#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"

extern "C" {
    FBXCSHARP_API void FbxLayerColor_Add(FbxLayerElementArrayTemplate<FbxColor>* dArray, FbxColor* color) {
        dArray->Add(*color);
    }
    FBXCSHARP_API void FbxLayerUV_Add(FbxLayerElementArrayTemplate<FbxVector2>* dArray, FbxVector2* vector) {
        dArray->Add(*vector);
    }

    FBXCSHARP_API void FbxLayerTangent_Add(FbxLayerElementArrayTemplate<FbxVector4>* dArray, FbxVector4* vector) {
        dArray->Add(*vector);
    }

    FBXCSHARP_API void FbxLayerMaterial_Add(FbxLayerElementArrayTemplate<int>* dArray, int num) {
        dArray->Add(num);
    }

    FBXCSHARP_API void FbxLayer_SetMaterials(FbxLayer* layer, FbxLayerElementMaterial* material) {
        layer->SetMaterials(material);
    }

    FBXCSHARP_API FbxLayerElementMaterial* FbxLayerElementMaterial_Create(FbxMesh* mesh, const char* name) {
        return FbxLayerElementMaterial::Create(mesh, name);
    }

    FBXCSHARP_API void FbxLayerElementMaterial_SetMappingMode(FbxLayerElementMaterial* element) {
        element->SetMappingMode(FbxLayerElement::eAllSame);
    }

    FBXCSHARP_API void FbxLayerElementMaterial_SetReferenceMode(FbxLayerElementMaterial* element) {
        element->SetReferenceMode(FbxLayerElement::eDirect);
    }

    FBXCSHARP_API FbxLayerElementArrayTemplate<int>* FbxLayerElementMaterial_GetIndexArray(FbxLayerElementMaterial* element) {
        return &(element->GetIndexArray());
    }
}