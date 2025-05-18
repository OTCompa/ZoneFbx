#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"

extern "C" {
    FBXCSHARP_API FbxScene* FbxScene_Create(FbxManager* manager, const char* name) {
        return FbxScene::Create(manager, name);
    }

    FBXCSHARP_API FbxNode* FbxScene_GetRootNode(FbxScene* scene) {
        return scene->GetRootNode();
    }

    FBXCSHARP_API void FbxScene_SetSystemUnit(FbxScene* scene) {
        scene->GetGlobalSettings().SetSystemUnit(FbxSystemUnit::m);
    }
}