#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxSurfacePhong* FbxSurfacePhong_Create(FbxScene* scene, const char* name) {
        return FbxSurfacePhong::Create(scene, name);
    }

    __declspec(dllexport) void FbxSurfacePhong_SetFactor(FbxSurfacePhong* surfacePhong, double specularFactor, double bumpFactor) {
        surfacePhong->AmbientFactor.Set(1.);
        surfacePhong->DiffuseFactor.Set(1.);
        surfacePhong->SpecularFactor.Set(specularFactor);
        //surfacePhong->EmissiveFactor.Set(0.3);  // doesn't seem to do anything?
        surfacePhong->BumpFactor.Set(bumpFactor);  // global normal strength, unsure if mtrl files contain this info
        surfacePhong->ShadingModel.Set("Phong");
    }

    __declspec(dllexport) void FbxSurfacePhong_ConnectSrcObject(FbxSurfacePhong* outsurface, FbxFileTexture* texture, int branch) {
        switch (branch) {
        case 0:
            outsurface->Diffuse.ConnectSrcObject(texture); break;
        case 1:
            outsurface->Specular.ConnectSrcObject(texture); break;
        case 2:
            outsurface->NormalMap.ConnectSrcObject(texture); break;
        case 3:
            outsurface->Emissive.ConnectSrcObject(texture); break;
        }
    }

    __declspec(dllexport) bool FbxSurfacePhong_PropertyExists(FbxSurfacePhong* outsurface, const char* propertyName) {
        auto property = outsurface->FindProperty(propertyName);
        return property.IsValid();
    }
}