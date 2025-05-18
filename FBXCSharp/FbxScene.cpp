#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"
#include "ContextManager.h"

extern "C" {
    FBXCSHARP_API FbxNode* FbxScene_GetRootNode(ContextManager* contextManager) {
        return contextManager->pScene->GetRootNode();
    }
}