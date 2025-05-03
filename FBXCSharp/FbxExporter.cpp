#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxExporter* FbxExporter_Create(FbxManager* manager, const char* name) {
        return FbxExporter::Create(manager, name);
    }

    __declspec(dllexport) bool FbxExporter_Initialize(FbxExporter* exporter, const char* out_path, FbxManager* manager) {
        return exporter->Initialize(out_path, -1, manager->GetIOSettings());
    }

    __declspec(dllexport) bool FbxExporter_Export(FbxExporter* exporter, FbxScene* scene) {
        return exporter->Export(scene);
    }

    __declspec(dllexport) void FbxExporter_Destroy(FbxExporter* exporter) {
        exporter->Destroy();
    }
}