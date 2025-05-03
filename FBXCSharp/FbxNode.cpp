#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxNode* FbxNode_Create(FbxManager* manager, const char* name) {
        return FbxNode::Create(manager, name);
    }

    __declspec(dllexport) void FbxNode_SetLclTranslation(FbxNode* node, double pData0, double pData1, double pData2) {
        node->LclTranslation.Set(FbxVectorTemplate3<double>(pData0, pData1, pData2));
    }

    __declspec(dllexport) void FbxNode_AddChild(FbxNode* node, FbxNode* child) {
        node->AddChild(child);
    }

    __declspec(dllexport) void FbxNode_AddMaterial(FbxNode* node, FbxSurfacePhong* material) {
        node->AddMaterial(material);
    }

    __declspec(dllexport) void FbxNode_SetNodeAttribute(FbxNode* node, FbxNodeAttribute* attribute) {
        node->SetNodeAttribute(attribute);
    }

    __declspec(dllexport) void FbxNode_SetStuff(FbxNode* node, double X, double Y, double Z, int branch) {
        switch (branch) {
        case 0:
            node->LclTranslation.Set(FbxDouble3(X, Y, Z));
            break;
        case 1:
            node->LclRotation.Set(FbxDouble3(X, Y, Z));
            break;
        case 2:
            node->LclScaling.Set(FbxDouble3(X, Y, Z));
            break;
        }
    }

    __declspec(dllexport) void FbxNode_SetPostTargetRotation(FbxNode* node) {
        node->SetPostTargetRotation(FbxVector4(90, 0, 0, 0));
    }
}