#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"
#include "ContextManager.h"
extern "C" {
    FBXCSHARP_API FbxExporter* FbxExporter_Create(ContextManager* contextManager, const char* name) {
        return FbxExporter::Create(contextManager->pManager, name);
    }

    FBXCSHARP_API bool FbxExporter_Initialize(FbxExporter* exporter, const char* out_path, ContextManager* contextManager) {
        return exporter->Initialize(out_path, -1, contextManager->pManager->GetIOSettings());
    }

    FBXCSHARP_API bool FbxExporter_Export(FbxExporter* exporter, ContextManager* contextManager) {
        return exporter->Export(contextManager->pScene);
    }

    FBXCSHARP_API void FbxExporter_Destroy(FbxExporter* exporter) {
        exporter->Destroy();
    }
}