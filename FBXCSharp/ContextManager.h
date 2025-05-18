#pragma once
#include <fbxsdk.h>

struct ContextManager {
	FbxManager* pManager;
	FbxScene* pScene;

	ContextManager();
	~ContextManager();
};