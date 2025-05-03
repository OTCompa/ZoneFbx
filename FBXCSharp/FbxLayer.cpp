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

}