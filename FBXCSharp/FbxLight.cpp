#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"
#include "ContextManager.h"

extern "C" {
    FBXCSHARP_API FbxLight* FbxLight_Create(ContextManager* contextManager, const char* name) {
        return FbxLight::Create(contextManager->pScene, name);
    }

    FBXCSHARP_API void FbxLight_SetColor(FbxLight* light, double X, double Y, double Z) {
        light->Color.Set(FbxDouble3(X, Y, Z));
    }

    FBXCSHARP_API void FbxLight_SetIntensity(FbxLight* light, double intensity) {
        light->Intensity.Set(intensity);
    }

    FBXCSHARP_API void FbxLight_SetLightType(FbxLight* light, FbxLight::EType type) {
        light->LightType.Set(type);
    }

    FBXCSHARP_API void FbxLight_SetDecay(FbxLight* light, FbxLight::EDecayType decayType) {
        light->DecayType.Set(decayType);
    }

    FBXCSHARP_API void FbxLight_CastShadows(FbxLight* light) {
        light->CastShadows.Set(true);
    }

    FBXCSHARP_API void FbxLight_SetAngle(FbxLight* light, double falloff, double hotspot) {
        light->OuterAngle.Set(falloff);
        light->InnerAngle.Set(hotspot);
    }
}