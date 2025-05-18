#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"

extern "C" {
    FBXCSHARP_API FbxExporter* FbxExporter_Create(FbxManager* manager, const char* name) {
        return FbxExporter::Create(manager, name);
    }

    FBXCSHARP_API bool FbxExporter_Initialize(FbxExporter* exporter, const char* out_path, FbxManager* manager) {
        return exporter->Initialize(out_path, -1, manager->GetIOSettings());
    }

    FBXCSHARP_API bool FbxExporter_Export(FbxExporter* exporter, FbxScene* scene) {
        return exporter->Export(scene);
    }

    FBXCSHARP_API void FbxExporter_Destroy(FbxExporter* exporter) {
        exporter->Destroy();
    }
}