#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    //FBXCSHARP_API void FbxLayerColor_Add(FbxLayerElementArrayTemplate<FbxColor>* dArray, FbxColor* color) {
    //    dArray->Add(*color);
    //}

    FBXCSHARP_API void FbxObject_ConnectSrcObject(FbxObject* parent, FbxObject* child) {
        parent->ConnectSrcObject(child);
    }

}