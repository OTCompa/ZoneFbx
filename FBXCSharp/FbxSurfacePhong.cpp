#include "pch.h"
#include <fbxsdk.h>
#include "ContextManager.h"


extern "C" {
    FBXCSHARP_API FbxSurfacePhong* FbxSurfacePhong_Create(ContextManager* contextManager, const char* name) {
        return FbxSurfacePhong::Create(contextManager->pScene, name);
    }

    FBXCSHARP_API void FbxSurfacePhong_SetFactor(FbxSurfacePhong* surfacePhong, double specularFactor, double bumpFactor) {
        surfacePhong->AmbientFactor.Set(1.);
        surfacePhong->DiffuseFactor.Set(1.);
        surfacePhong->SpecularFactor.Set(specularFactor);
        //surfacePhong->EmissiveFactor.Set(0.3);  // doesn't seem to do anything?
        surfacePhong->BumpFactor.Set(bumpFactor);  // global normal strength, unsure if mtrl files contain this info
        surfacePhong->ShadingModel.Set("Phong");
    }

    FBXCSHARP_API void FbxSurfacePhong_ConnectDiffuse(FbxSurfacePhong* outsurface, FbxFileTexture* texture) {
        outsurface->Diffuse.ConnectSrcObject(texture);
    }

    FBXCSHARP_API void FbxSurfacePhong_ConnectSpecular(FbxSurfacePhong* outsurface, FbxFileTexture* texture) {
        outsurface->Specular.ConnectSrcObject(texture);
    }

    FBXCSHARP_API void FbxSurfacePhong_ConnectNormalMap(FbxSurfacePhong* outsurface, FbxFileTexture* texture) {
        outsurface->NormalMap.ConnectSrcObject(texture);
    }

    FBXCSHARP_API void FbxSurfacePhong_ConnectEmissive(FbxSurfacePhong* outsurface, FbxFileTexture* texture) {
        outsurface->Emissive.ConnectSrcObject(texture);
    }

    FBXCSHARP_API void FbxSurfacePhong_ConnectTransparency(FbxSurfacePhong* outsurface, FbxFileTexture* texture) {
        outsurface->TransparentColor.ConnectSrcObject(texture);
        outsurface->TransparencyFactor.Set(1.0);
    }

    FBXCSHARP_API bool FbxSurfacePhong_PropertyExists(FbxSurfacePhong* outsurface, const char* propertyName) {
        auto property = outsurface->FindProperty(propertyName);
        return property.IsValid();
    }
}