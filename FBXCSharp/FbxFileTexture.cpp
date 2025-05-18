#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"
#include "ContextManager.h"

extern "C" {
    FBXCSHARP_API FbxFileTexture* FbxFileTexture_Create(ContextManager* contextManager, const char* name) {
        return FbxFileTexture::Create(contextManager->pScene, name);
    }

    FBXCSHARP_API void FbxFileTexture_SetStuff(FbxFileTexture* texture, const char* pathname) {
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