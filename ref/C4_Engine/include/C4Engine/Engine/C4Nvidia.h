//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#ifndef C4Nvidia_h
#define C4Nvidia_h


#define NVAPI_DEFAULT_HANDLE				0
#define NVAPI_SHORT_STRING_MAX				64
#define NVAPI_INTERFACE						extern NvAPI_Status __cdecl
#define MAKE_NVAPI_VERSION(typeName, ver)	(NvU32) (sizeof(typeName) | ((ver) << 16))
#define NV_DISPLAY_DRIVER_VERSION_VER		MAKE_NVAPI_VERSION(NV_DISPLAY_DRIVER_VERSION, 1)
#define NV_DECLARE_HANDLE(name)				struct name##__ { int unused; }; typedef struct name##__ *name


NV_DECLARE_HANDLE(NvDisplayHandle); 


typedef unsigned long	NvU32;
typedef char			NvAPI_ShortString[NVAPI_SHORT_STRING_MAX];


enum NvAPI_Status
{
	NVAPI_OK	= 0
};


struct NV_DISPLAY_DRIVER_VERSION
{
	NvU32				version;
	NvU32				drvVersion;
	NvU32				bldChangeListNum;
	NvAPI_ShortString	szBuildBranchString;
	NvAPI_ShortString	szAdapterString;
};


extern "C"
{
	NVAPI_INTERFACE NvAPI_Initialize();
	NVAPI_INTERFACE NvAPI_Unload();
	NVAPI_INTERFACE NvAPI_GetDisplayDriverVersion(NvDisplayHandle, NV_DISPLAY_DRIVER_VERSION *);
}


#endif

// ZYURVUR
