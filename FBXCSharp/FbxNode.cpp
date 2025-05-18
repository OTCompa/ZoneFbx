#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"
#include "ContextManager.h"

extern "C" {
    FBXCSHARP_API FbxNode* FbxNode_Create(ContextManager* contextManager, const char* name) {
        return FbxNode::Create(contextManager->pScene, name);
    }

    FBXCSHARP_API void FbxNode_SetLclTranslation(FbxNode* node, double pData0, double pData1, double pData2) {
        node->LclTranslation.Set(FbxVectorTemplate3<double>(pData0, pData1, pData2));
    }

    FBXCSHARP_API void FbxNode_AddChild(FbxNode* node, FbxNode* child) {
        node->AddChild(child);
    }

    FBXCSHARP_API void FbxNode_AddMaterial(FbxNode* node, FbxSurfacePhong* material) {
        node->AddMaterial(material);
    }

    FBXCSHARP_API void FbxNode_SetNodeAttribute(FbxNode* node, FbxNodeAttribute* attribute) {
        node->SetNodeAttribute(attribute);
    }

    FBXCSHARP_API void FbxNode_SetStuff(FbxNode* node, double X, double Y, double Z, int branch) {
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

    FBXCSHARP_API void FbxNode_SetPostTargetRotation(FbxNode* node) {
        node->SetPostTargetRotation(FbxVector4(90, 0, 0, 0));
    }
}