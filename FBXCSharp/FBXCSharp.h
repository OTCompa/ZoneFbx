// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the FBXCSHARP_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// FBXCSHARP_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef FBXCSHARP_EXPORTS
#define FBXCSHARP_API __declspec(dllexport)
#else
#define FBXCSHARP_API __declspec(dllimport)
#endif

#include <fbxsdk.h>

// This class is exported from the dll
class FBXCSHARP_API CFBXCSharp {
public:
	CFBXCSharp(void);
	// TODO: add your methods here.
};

extern FBXCSHARP_API int nFBXCSharp;

FBXCSHARP_API int fnFBXCSharp(void);
