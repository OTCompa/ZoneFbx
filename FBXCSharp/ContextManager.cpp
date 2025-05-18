#include "pch.h"
#include "ContextManager.h"
#include <fbxsdk.h>
#include "FBXCSharp.h"

ContextManager::ContextManager() {
	pManager = nullptr;
	pScene = nullptr;
}

ContextManager::~ContextManager() {
	if (pManager) pManager->Destroy();
	pScene = nullptr;
	pManager = nullptr;
}

extern "C" {
	FBXCSHARP_API ContextManager* ContextManager_Create() {
		auto contextManager = new ContextManager();
		return contextManager;
	}
	FBXCSHARP_API void ContextManager_Destroy(ContextManager* contextManager) {
		contextManager->~ContextManager();
		delete contextManager;
	}

	FBXCSHARP_API void ContextManager_CreateManager(ContextManager* contextManager) {
		if (!contextManager->pManager) {
			contextManager->pManager = FbxManager::Create();
		}
		else {
			printf("FBXCSharp: Manager already exists!\n");
		}
	}

	FBXCSHARP_API void ContextManager_DestroyManager(ContextManager* contextManager) {
		contextManager->pManager->Destroy();
		contextManager->pManager = nullptr;
	}

	FBXCSHARP_API void ContextManager_CreateScene(ContextManager* contextManager, const char* name) {
		if (contextManager->pManager) {
			if (contextManager->pScene) {
				printf("FBXCSharp: Scene already exists!\n");
			}
			else {
				contextManager->pScene = FbxScene::Create(contextManager->pManager, name);
				contextManager->pScene->GetGlobalSettings().SetSystemUnit(FbxSystemUnit::m);
			}
		}
		else {
			printf("FBXCSharp: Manager not initialized!\n");
		}
	}

	FBXCSHARP_API void ContextManager_DestroyScene(ContextManager* contextManager) {
		if (contextManager->pScene) {
			contextManager->pScene->Destroy();
			contextManager->pScene = nullptr;
		}
	}
}