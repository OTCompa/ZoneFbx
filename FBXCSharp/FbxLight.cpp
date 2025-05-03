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

    __declspec(dllexport) void FbxLight_SetLightType(FbxLight* light, int type) {
        // based on Lumina.Data.Parsing.Layer.LayerEnums.LightType
        switch (type) {
        case 1:
            light->LightType.Set(FbxLight::eDirectional); break;
        case 2:
            light->LightType.Set(FbxLight::ePoint); break;
        case 3:
            light->LightType.Set(FbxLight::eSpot); break;
        }
    }

    __declspec(dllexport) void FbxLight_SetDecay(FbxLight* light, int decayType) {
        switch (decayType) {
        case 1:
            light->DecayType.Set(FbxLight::eLinear); break;
        case 2:
            light->DecayType.Set(FbxLight::eQuadratic); break;
        case 3:
            light->DecayType.Set(FbxLight::eCubic); break;
        }
    }

    __declspec(dllexport) void FbxLight_CastShadows(FbxLight* light) {
        light->CastShadows.Set(true);
    }

    __declspec(dllexport) void FbxLight_SetAngle(FbxLight* light, double falloff, double hotspot) {
        light->OuterAngle.Set(falloff);
        light->InnerAngle.Set(hotspot);
    }
}