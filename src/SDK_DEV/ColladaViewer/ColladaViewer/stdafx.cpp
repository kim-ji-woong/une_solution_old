
#include "stdafx.h"
#include "d3d9types.h"
#include "d3dx9.h"
#include "MainFrm.h"

#include "AssimpView.h"

using namespace AssimpView;

namespace AssimpView
{
	//-------------------------------------------------------------------------------
	// evil globals
	//-------------------------------------------------------------------------------
	HINSTANCE g_hInstance				= NULL;
	HWND g_hDlg							= NULL;
	HWND g_hView						= NULL;
	D3DPRESENT_PARAMETERS		g_3dPP;
	IDirect3D9* g_piD3D					= NULL;
	IDirect3DDevice9* g_piDevice		= NULL;
	IDirect3DVertexDeclaration9* gDefaultVertexDecl = NULL;
	
	double g_fFPS						= 0.0f;
	char g_szFileName[MAX_PATH];
	
	ID3DXEffect* g_piDefaultEffect		= NULL;
	ID3DXEffect* g_piNormalsEffect		= NULL;
	ID3DXEffect* g_piPassThroughEffect	= NULL;
	ID3DXEffect* g_piPatternEffect		= NULL;
	
	bool g_bMousePressed				= false;
	bool g_bMousePressedR				= false;
	bool g_bMousePressedM				= false;
	bool g_bMousePressedBoth			= false;
	
	float g_fElpasedTime				= 0.0f;
	
	D3DCAPS9 g_sCaps;
	bool g_bLoadingFinished				= false;
	HANDLE g_hThreadHandle				= NULL;
	float g_fWheelPos					= -10.0f;
	bool g_bLoadingCanceled				= false;
	IDirect3DTexture9* g_pcTexture		= NULL;
	bool g_bPlay						= false;
	double g_dCurrent = 0.;

	// default pp steps
	unsigned int ppsteps = aiProcess_CalcTangentSpace | // calculate tangents and bitangents if possible
		aiProcess_JoinIdenticalVertices    | // join identical vertices/ optimize indexing
		aiProcess_ValidateDataStructure    | // perform a full validation of the loader's output
		aiProcess_ImproveCacheLocality     | // improve the cache locality of the output vertices
		aiProcess_RemoveRedundantMaterials | // remove redundant materials
		aiProcess_FindDegenerates          | // remove degenerated polygons from the import
		aiProcess_FindInvalidData          | // detect invalid model data, such as invalid normal vectors
		aiProcess_GenUVCoords              | // convert spherical, cylindrical, box and planar mapping to proper UVs
		aiProcess_TransformUVCoords        | // preprocess UV transformations (scaling, translation ...)
		aiProcess_FindInstances            | // search for instanced meshes and remove them by references to one master
		aiProcess_LimitBoneWeights         | // limit bone weights to 4 per vertex
		aiProcess_OptimizeMeshes		   | // join small meshes, if possible;
		aiProcess_SplitByBoneCount         | // split meshes with too many bones. Necessary for our (limited) hardware skinning shader
		0;

	unsigned int ppstepsdefault = ppsteps;

	bool nopointslines = false;

	bool g_bWasFlipped = false;
	float g_smoothAngle = 80.f;

	aiMatrix4x4 g_mWorld;
	aiMatrix4x4 g_mWorldRotate;
	aiVector3D g_vRotateSpeed			= aiVector3D(0.5f,0.5f,0.5f);

	// NOTE: The second light direction is now computed from the first
	aiVector3D g_avLightDirs[1] = 
	{	aiVector3D(-0.5f,0.6f,0.2f)  };


	D3DCOLOR g_avLightColors[3] = 
	{
		D3DCOLOR_ARGB(0xFF,0xFF,0xFF,0xFF),
		D3DCOLOR_ARGB(0xFF,0xFF,0x00,0x00),
		D3DCOLOR_ARGB(0xFF,0x05,0x05,0x05),
	};

	POINT g_mousePos;
	POINT g_LastmousePos;
	bool g_bFPSView						= false;
	bool g_bInvert						= false;
	EClickPos g_eClick					= EClickPos_Circle;
	unsigned int g_iCurrentColor		= 0;

	float g_fLightIntensity				= 1.0f;
	float g_fLightColor					= 1.0f;

	RenderOptions g_sOptions;
	Camera g_sCamera;
	AssetHelper *g_pcAsset				= NULL;


	//
	// Contains the mask image for the HUD 
	// (used to determine the position of a click)
	//
	unsigned char* g_szImageMask		= NULL;

	float g_fLoadTime = 0.0f;

	// Static array to keep custom color values
	COLORREF g_aclCustomColors[16] = {0};
		
}

// Global registry key
HKEY g_hRegistry = NULL;

HKEY& GetRootReg()
{
	return g_hRegistry;
}

HTREEITEM GetRootItem()
{
	CMainFrame * pMainFrame = (CMainFrame*)AfxGetApp()->GetMainWnd();		
	HTREEITEM m_hRoot = (HTREEITEM)pMainFrame->m_wndClassView.m_wndClassView.GetRootItem(); 
	return m_hRoot;
}


HWND GetTreeHwnd()
{
	CMainFrame * pMainFrame = (CMainFrame*)AfxGetApp()->GetMainWnd();		
	return pMainFrame->m_wndClassView.m_wndClassView.m_hWnd;
}