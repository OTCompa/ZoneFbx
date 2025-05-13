#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    //__declspec(dllexport) void FbxLayerColor_Add(FbxLayerElementArrayTemplate<FbxColor>* dArray, FbxColor* color) {
    //    dArray->Add(*color);
    //}
    __declspec(dllexport) FbxLayeredTexture* FbxLayeredTexture_Create(FbxScene* scene, const char* name) {
        return FbxLayeredTexture::Create(scene, name);
    }

    __declspec (dllexport) void FbxLayeredTexture_ConnectSrcObject(FbxLayeredTexture* layeredTexture, FbxObject* object) {
        layeredTexture->ConnectSrcObject(object);
    }

    __declspec (dllexport) void FbxLayeredTexture_SetTextureBlendMode(FbxLayeredTexture* layeredTexture, int index, FbxLayeredTexture::EBlendMode blendMode) {
        layeredTexture->SetTextureBlendMode(index, blendMode);
    }
}