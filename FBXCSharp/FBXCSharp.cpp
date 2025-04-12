#include "pch.h"
#include <fbxsdk.h>
// ngl i have 0 idea what i'm doing

extern "C" {
    __declspec(dllexport) FbxManager* FbxManager_Create() {
        return FbxManager::Create();
    }

    __declspec(dllexport) void FbxManager_Destroy(FbxManager* manager) {
        if (manager) {
            manager->Destroy();
        }
    }

    __declspec(dllexport) bool FbxManager_Initialize(FbxManager* manager) {
        if (manager) {
            FbxIOSettings* io = FbxIOSettings::Create(manager, "IOSRoot");
            manager->SetIOSettings(io);

            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_MATERIAL, true);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_TEXTURE, true);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_EMBEDDED, false);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_SHAPE, true);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_GOBO, true);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_ANIMATION, false);
            (*manager->GetIOSettings()).SetBoolProp(EXP_FBX_GLOBAL_SETTINGS, true);

            return true;
        }
        return false;
    }

    __declspec(dllexport) FbxScene* FbxScene_Create(FbxManager* manager, const char* name) {
        return FbxScene::Create(manager, name);
    }

    __declspec(dllexport) FbxNode* FbxScene_GetRootNode(FbxScene* scene) {
        return scene->GetRootNode();
    }

    __declspec(dllexport) FbxNode* FbxNode_Create(FbxManager* manager, const char* name) {
        return FbxNode::Create(manager, name);
    }

    __declspec(dllexport) void FbxNode_SetLclTranslation(FbxNode* node, double pData0, double pData1, double pData2) {
        node->LclTranslation.Set(FbxVectorTemplate3<double>(pData0, pData1, pData2));
    }

    __declspec(dllexport) void FbxNode_AddChild(FbxNode* node, FbxNode* child) {
        node->AddChild(child);
    }

    __declspec(dllexport) FbxMesh* FbxMesh_Create(FbxScene* scene, const char* name) {
        return FbxMesh::Create(scene, name);
    }

    __declspec(dllexport) void FbxMesh_Init(FbxMesh* mesh, int length) {
        mesh->InitControlPoints(length);
        mesh->InitNormals(length);
    }

    __declspec(dllexport) FbxGeometryElementVertexColor* FbxGeometryElementVertexColor_Create(FbxMesh* mesh) {
        return mesh->CreateElementVertexColor();
    }

    __declspec(dllexport) void FbxGeometryElementVertexColor_SetMappingNode(FbxGeometryElementVertexColor* element) {
        element->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
    }

    __declspec(dllexport) FbxLayerElementArrayTemplate<FbxColor>* FbxGeometryElementVertexColor_GetDirectArray(FbxGeometryElementVertexColor* element) {
        return &(element->GetDirectArray());
    }

    __declspec(dllexport) void FbxLayerColor_Add(FbxLayerElementArrayTemplate<FbxColor>* dArray, FbxColor* color) {
        dArray->Add(*color);
    }

    __declspec(dllexport) FbxGeometryElementUV* FbxGeometryElementUV_Create(FbxMesh* mesh, const char* name) {
        return mesh->CreateElementUV(name);
    }

    __declspec(dllexport) void FbxGeometryElementUV_SetMappingNode(FbxGeometryElementUV* element) {
        element->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
    }

    __declspec(dllexport) FbxLayerElementArrayTemplate<FbxVector2>* FbxGeometryElementUV_GetDirectArray(FbxGeometryElementUV* element) {
        return &(element->GetDirectArray());
    }

    __declspec(dllexport) void FbxLayerUV_Add(FbxLayerElementArrayTemplate<FbxVector2>* dArray, FbxVector2* vector) {
        dArray->Add(*vector);
    }

    __declspec(dllexport) FbxGeometryElementTangent* FbxGeometryElementTangent_Create(FbxMesh* mesh) {
        return mesh->CreateElementTangent();
    }

    __declspec(dllexport) void FbxGeometryElementTangent_SetMappingNode(FbxGeometryElementTangent* element) {
        element->SetMappingMode(FbxLayerElement::EMappingMode::eByControlPoint);
    }

    __declspec(dllexport) FbxLayerElementArrayTemplate<FbxVector4>* FbxGeometryElementTangent_GetDirectArray(FbxGeometryElementTangent* element) {
        return &(element->GetDirectArray());
    }

    __declspec(dllexport) void FbxLayerTangent_Add(FbxLayerElementArrayTemplate<FbxVector4>* dArray, FbxVector4* vector) {
        dArray->Add(*vector);
    }

    __declspec(dllexport) FbxVector4* FbxVector4_Create(double X, double Y, double Z, double W) {
        return new FbxVector4(X, Y, Z, W);
    }

    __declspec(dllexport) void FbxMesh_SetControlPointAt(FbxMesh* mesh, FbxVector4* a, FbxVector4* b, int i) {
        mesh->SetControlPointAt(*a, *b, i);
    }

    __declspec(dllexport) FbxVector2* FbxVector2_Create(double X, double Y) {
        return new FbxVector2(X, Y);
    }

    __declspec(dllexport) void FbxMesh_BeginPolygon(FbxMesh* mesh) {
        mesh->BeginPolygon();
    }

    __declspec(dllexport) void FbxMesh_AddPolygon(FbxMesh* mesh, int i) {
        mesh->AddPolygon(i);
    }

    __declspec(dllexport) void FbxMesh_EndPolygon(FbxMesh* mesh) {
        mesh->EndPolygon();
    }

    __declspec(dllexport) FbxSurfacePhong* FbxSurfacePhong_Create(FbxScene* scene, const char* name) {
        return FbxSurfacePhong::Create(scene, name);
    }

    __declspec(dllexport) void FbxSurfacePhong_SetFactor(FbxSurfacePhong* surfacePhong) {
        surfacePhong->AmbientFactor.Set(1.);
        surfacePhong->DiffuseFactor.Set(1.);
        surfacePhong->SpecularFactor.Set(0.3);
        //surfacePhong->EmissiveFactor.Set(0.3);  // doesn't seem to do anything?
        surfacePhong->BumpFactor.Set(0.2);  // global normal strength, unsure if mtrl files contain this info
        surfacePhong->ShadingModel.Set("Phong");
    }

    __declspec(dllexport) FbxFileTexture* FbxFileTexture_Create(FbxScene* scene, const char* name) {
        return FbxFileTexture::Create(scene, name);
    }

    __declspec(dllexport) void FbxFileTexture_SetStuff(FbxFileTexture* texture, const char* pathname) {
        texture->SetFileName(pathname);
        texture->SetMappingType(FbxTexture::eUV);
        texture->SetTextureUse(FbxTexture::eStandard);
        texture->SetMaterialUse(FbxFileTexture::eModelMaterial);
        texture->SetSwapUV(false);
        texture->SetTranslation(0.0, 0.0);
        texture->SetScale(1.0, 1.0);
        texture->SetRotation(0.0, 0.0);
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

    __declspec(dllexport) void FbxNode_AddMaterial(FbxNode* node, FbxSurfacePhong* material) {
        node->AddMaterial(material);
    }

    __declspec(dllexport) void FbxNode_SetNodeAttribute(FbxNode* node, FbxMesh* mesh) {
        node->SetNodeAttribute(mesh);
    }

    __declspec(dllexport) void FbxNode_SetStuff(FbxNode* node, double X, double Y, double Z, int branch) {
        switch (branch) {
        case 0:
            node->LclTranslation.Set(FbxDouble3(X, Y, Z));
            break;
        case 1:
            node->LclRotation.Set(FbxDouble3(X, Y, Z));
            break;
        case 2:
            node->LclScaling.Set(FbxDouble3(X, Y, Z));
            break;
        }
    }

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