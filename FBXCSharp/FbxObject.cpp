#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    //__declspec(dllexport) void FbxLayerColor_Add(FbxLayerElementArrayTemplate<FbxColor>* dArray, FbxColor* color) {
    //    dArray->Add(*color);
    //}

    __declspec (dllexport) void FbxObject_ConnectSrcObject(FbxObject* parent, FbxObject* child) {
        parent->ConnectSrcObject(child);
    }

}