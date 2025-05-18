#include "pch.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"

extern "C" {
    FBXCSHARP_API FbxVector4* FbxVector4_Create(double X, double Y, double Z, double W) {
        return new FbxVector4(X, Y, Z, W);
    }

    FBXCSHARP_API void FbxVector4_Destroy(FbxVector4* v) {
        delete v;
    }

    FBXCSHARP_API FbxVector2* FbxVector2_Create(double X, double Y) {
        return new FbxVector2(X, Y);
    }

    FBXCSHARP_API void FbxVector2_Destroy(FbxVector2* v) {
        delete v;
    }
}