#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) void FbxProperty_CreateString(FbxObject* object, const char* name, const char* value) {
        FbxPropertyT<FbxString> property = FbxProperty::Create(object, FbxStringDT, name);
        property.ModifyFlag(FbxPropertyFlags::eUserDefined, true);
        property.Set(value);
    }

    __declspec(dllexport) void FbxProperty_SetString(FbxProperty* property, const char* value) {
        property->Set("hello");
    }

    __declspec(dllexport) void FbxProperty_ModifyFlag(FbxProperty* property, FbxPropertyFlags::EFlags flag, bool value) {
        property->ModifyFlag(flag, value);
    }

    // might be redundant if manager handles disposing this too
    __declspec(dllexport) void FbxProperty_Destroy(FbxProperty* property) {
        property->Destroy();
    }

}