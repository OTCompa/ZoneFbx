#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    FBXCSHARP_API void FbxProperty_CreateString(FbxObject* object, const char* name, const char* value) {
        FbxPropertyT<FbxString> property = FbxProperty::Create(object, FbxStringDT, name);
        property.ModifyFlag(FbxPropertyFlags::eUserDefined, true);
        property.Set(value);
    }

    FBXCSHARP_API void FbxProperty_ModifyFlag(FbxProperty* property, FbxPropertyFlags::EFlags flag, bool value) {
        property->ModifyFlag(flag, value);
    }

    // might be redundant if manager handles disposing this too
    FBXCSHARP_API void FbxProperty_Destroy(FbxProperty* property) {
        property->Destroy();
    }

}