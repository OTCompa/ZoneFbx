#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxLight* FbxLight_Create(FbxScene* scene, const char* name) {
        return FbxLight::Create(scene, name);
    }

    __declspec(dllexport) void FbxLight_SetColor(FbxLight* light, double X, double Y, double Z) {
        light->Color.Set(FbxDouble3(X, Y, Z));
    }

    __declspec(dllexport) void FbxLight_SetIntensity(FbxLight* light, double intensity) {
        light->Intensity.Set(intensity);
    }

    __declspec(dllexport) void FbxLight_SetLightType(FbxLight* light, FbxLight::EType type) {
        light->LightType.Set(type);
    }

    __declspec(dllexport) void FbxLight_SetDecay(FbxLight* light, FbxLight::EDecayType decayType) {
        light->DecayType.Set(decayType);
    }

    __declspec(dllexport) void FbxLight_CastShadows(FbxLight* light) {
        light->CastShadows.Set(true);
    }

    __declspec(dllexport) void FbxLight_SetAngle(FbxLight* light, double falloff, double hotspot) {
        light->OuterAngle.Set(falloff);
        light->InnerAngle.Set(hotspot);
    }
}