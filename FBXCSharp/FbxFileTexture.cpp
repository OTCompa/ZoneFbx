#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxFileTexture* FbxFileTexture_Create(FbxScene* scene, const char* name) {
        return FbxFileTexture::Create(scene, name);
    }

    __declspec(dllexport) void FbxFileTexture_SetStuff(FbxFileTexture* texture, const char* pathname) {
        texture->SetFileName(pathname);
        texture->SetMappingType(FbxTexture::eUV);
        texture->SetTextureUse(FbxTexture::eStandard);
        texture->SetMaterialUse(FbxFileTexture::eModelMaterial);
        texture->SetSwapUV(false);
        texture->SetTranslation(0.0, 0.0);
        texture->SetScale(1.0, 1.0);
        texture->SetRotation(0.0, 0.0);
    }
}