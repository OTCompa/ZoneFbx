#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"
#include "ContextManager.h"
extern "C" {
    //__declspec(dllexport) void FbxLayerColor_Add(FbxLayerElementArrayTemplate<FbxColor>* dArray, FbxColor* color) {
    //    dArray->Add(*color);
    //}
    FBXCSHARP_API FbxLayeredTexture* FbxLayeredTexture_Create(ContextManager* contextManager, const char* name) {
        return FbxLayeredTexture::Create(contextManager->pScene, name);
    }

    FBXCSHARP_API void FbxLayeredTexture_ConnectSrcObject(FbxLayeredTexture* layeredTexture, FbxObject* object) {
        layeredTexture->ConnectSrcObject(object);
    }

    FBXCSHARP_API void FbxLayeredTexture_SetTextureBlendMode(FbxLayeredTexture* layeredTexture, int index, FbxLayeredTexture::EBlendMode blendMode) {
        layeredTexture->SetTextureBlendMode(index, blendMode);
    }
}