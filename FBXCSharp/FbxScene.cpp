#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxScene* FbxScene_Create(FbxManager* manager, const char* name) {
        return FbxScene::Create(manager, name);
    }

    __declspec(dllexport) FbxNode* FbxScene_GetRootNode(FbxScene* scene) {
        return scene->GetRootNode();
    }

    __declspec(dllexport) void FbxScene_SetSystemUnit(FbxScene* scene) {
        scene->GetGlobalSettings().SetSystemUnit(FbxSystemUnit::m);
    }
}