#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxManager* FbxManager_Create() {
        return FbxManager::Create();
    }

    __declspec(dllexport) void FbxManager_Destroy(FbxManager* manager) {
        if (manager) {
            manager->Destroy();
        }
    }

    __declspec(dllexport) bool FbxManager_Initialize(FbxManager* manager) {
        if (manager) {
            FbxIOSettings* io = FbxIOSettings::Create(manager, "IOSRoot");
            manager->SetIOSettings(io);

            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_MATERIAL, true);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_TEXTURE, true);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_EMBEDDED, false);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_SHAPE, true);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_GOBO, true);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_ANIMATION, false);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_GLOBAL_SETTINGS, true);

            return true;
        }
        return false;
    }
}