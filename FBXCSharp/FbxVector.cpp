#include "pch.h"
#include <fbxsdk.h>

extern "C" {
    __declspec(dllexport) FbxVector4* FbxVector4_Create(double X, double Y, double Z, double W) {
        return new FbxVector4(X, Y, Z, W);
    }
    __declspec(dllexport) FbxVector2* FbxVector2_Create(double X, double Y) {
        return new FbxVector2(X, Y);
    }
}