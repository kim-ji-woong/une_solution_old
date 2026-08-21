// SettingDialog.cpp : 구현 파일입니다.
//

#include "stdafx.h"
#include "SettingDialog.h"
#include "afxdialogex.h"

#include "AssimpView.h"
#include "RenderOptions.h"

using namespace AssimpView;
namespace AssimpView
{
	extern bool g_bFPSView;
	extern RenderOptions g_sOptions;
	
	void LoadCheckerPatternColors();
	void SetupPPUIState();
	void PopulateExportMenu();	
}

void UpdateEdit(HWND hWnd)
{
	if( hWnd != NULL)
	{
		CSettingDialog * pDlg = (CSettingDialog*)CWnd::FromHandle(hWnd);
		if( pDlg != NULL)
		{
			pDlg->OnEnChangeEdit();
		}
	}
}

// CSettingDialog 대화 상자입니다.

IMPLEMENT_DYNAMIC(CSettingDialog, CDlgGradient )

CSettingDialog::CSettingDialog(CWnd* pParent /*=NULL*/)
	: CDlgGradient(CSettingDialog::IDD, pParent)
{
	AddStatic(IDC_STATIC);
	AddStatic(IDC_NUMVERTS);
	AddStatic(IDC_NUMNODES);
	AddStatic(IDC_NUMSHADERS);
	AddStatic(IDC_LOADTIME);
	AddStatic(IDC_NUMFACES);
	AddStatic(IDC_NUMMATS);
	AddStatic(IDC_NUMMESHES);
	AddStatic(IDC_FPS);

	AddCheck(IDC_TOGGLEMS);
	AddCheck(IDC_TOGGLEWIRE);
	AddCheck(IDC_TOGGLEMAT);
	AddCheck(IDC_TOGGLENORMALS);
	AddCheck(IDC_AUTOROTATE);
	AddCheck(IDC_LOWQUALITY);
	AddCheck(IDC_NOSPECULAR);
	AddCheck(IDC_SHOWSKELETON);
	AddCheck(IDC_ZOOM);
	AddCheck(IDC_LIGHTROTATE);
	AddCheck(IDC_3LIGHTS);
	AddCheck(IDC_BFCULL);
	AddCheck(IDC_NOAB);

	AddStatic(IDC_EVERT);
	AddStatic(IDC_ENODEWND);
	AddStatic(IDC_ESHADER);
	AddStatic(IDC_ELOAD);

	AddStatic(IDC_EFACE);
	AddStatic(IDC_EMAT);
	AddStatic(IDC_EMESH);
	AddStatic(IDC_EFPS);
}

CSettingDialog::~CSettingDialog()
{
}

void CSettingDialog::DoDataExchange(CDataExchange* pDX)
{
	CDlgGradient::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_SLIDERANIM, m_sliderAni);
}


BEGIN_MESSAGE_MAP(CSettingDialog, CDlgGradient )
	ON_BN_CLICKED(IDC_TOGGLEMS, &CSettingDialog::OnToggleMS)
	ON_BN_CLICKED(IDC_TOGGLEWIRE, &CSettingDialog::OnToggleWireFrame)
	ON_BN_CLICKED(IDC_TOGGLEMAT, &CSettingDialog::OnToggleMats)
	ON_BN_CLICKED(IDC_TOGGLENORMALS, &CSettingDialog::OnToggleNormals)
	ON_BN_CLICKED(IDC_AUTOROTATE, &CSettingDialog::OnToggleAutoRotate)
	ON_BN_CLICKED(IDC_LOWQUALITY, &CSettingDialog::OnToggleLowQuality)
	ON_BN_CLICKED(IDC_NOSPECULAR, &CSettingDialog::OnToggleSpecular)
	ON_BN_CLICKED(IDC_SHOWSKELETON, &CSettingDialog::OnToggleSkeleton)
	ON_BN_CLICKED(IDC_ZOOM, &CSettingDialog::OnToggleFPSView)
	ON_BN_CLICKED(IDC_LIGHTROTATE, &CSettingDialog::OnToggleLightRotate)
	ON_BN_CLICKED(IDC_3LIGHTS, &CSettingDialog::OnToggleMultipleLights)
	ON_BN_CLICKED(IDC_BFCULL, &CSettingDialog::OnToggleCulling)
	ON_BN_CLICKED(IDC_NOAB, &CSettingDialog::OnToggleTransparency)
	ON_EN_CHANGE(IDC_EVERT, &CSettingDialog::OnEnChangeEdit)
	ON_EN_CHANGE(IDC_ENODEWND, &CSettingDialog::OnEnChangeEdit)
	ON_EN_CHANGE(IDC_ESHADER, &CSettingDialog::OnEnChangeEdit)
	ON_EN_CHANGE(IDC_ELOAD, &CSettingDialog::OnEnChangeEdit)
	ON_EN_CHANGE(IDC_EFACE, &CSettingDialog::OnEnChangeEdit)
	ON_EN_CHANGE(IDC_EMAT, &CSettingDialog::OnEnChangeEdit)
	ON_EN_CHANGE(IDC_EMESH, &CSettingDialog::OnEnChangeEdit)
	ON_EN_CHANGE(IDC_EFPS, &CSettingDialog::OnEnChangeEdit)

END_MESSAGE_MAP()

void CSettingDialog::ChangeSytle()
{
	if( m_sliderAni .GetSafeHwnd() )
	{
		m_sliderAni.ChangeSytle();
	}
	
}

void CSettingDialog::InitUI()
{
	SetDlgItemText(IDC_EVERT, "0");
	SetDlgItemText(IDC_EFACE, "0");
	SetDlgItemText(IDC_EMAT, "0");
	SetDlgItemText(IDC_ESHADER, "0");
	SetDlgItemText(IDC_ENODEWND, "0");
	SetDlgItemText(IDC_ETEX, "0");
	SetDlgItemText(IDC_EMESH, "0");
	
#ifndef ASSIMP_BUILD_NO_EXPORT
	PopulateExportMenu();
#endif

	// setup the default window title
	//SetWindowText(m_hWnd,AI_VIEW_CAPTION_BASE);

	// read some UI properties from the registry and apply them
	DWORD dwValue;
	DWORD dwTemp = sizeof( DWORD );

	// store the key in a global variable for later use
	RegCreateKeyEx(HKEY_CURRENT_USER,"Software\\UnE\\ColladaViewer", NULL,NULL,0,KEY_ALL_ACCESS, NULL, &GetRootReg(),NULL);

	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"LastUIState",NULL,NULL, (BYTE*)&dwValue, &dwTemp))
	{
		dwValue = 1;
	}
	if (0 == dwValue)
	{
		// collapse the viewer
		// adjust the size
		::SetWindowText(GetDlgItem(IDC_BLUBB)->GetSafeHwnd(), ">>" );
	}
	else
	{
		CheckDlgButton(IDC_BLUBB, BST_CHECKED);
	}

	// AutoRotate
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"AutoRotate",NULL,NULL, (BYTE*)&dwValue,&dwTemp))
		dwValue = 0;

	if (0 == dwValue)
	{
		g_sOptions.bRotate = false;
		CheckDlgButton(IDC_AUTOROTATE,BST_UNCHECKED);
	}
	else
	{
		g_sOptions.bRotate = true;
		CheckDlgButton(IDC_AUTOROTATE,BST_CHECKED);
	}

	// MultipleLights
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"MultipleLights",NULL,NULL, (BYTE*)&dwValue,&dwTemp))
		dwValue = 0;

	if (0 == dwValue)
	{
		g_sOptions.b3Lights = false;
		CheckDlgButton(IDC_3LIGHTS, BST_UNCHECKED);
	}
	else 
	{
		g_sOptions.b3Lights = true;
		CheckDlgButton(IDC_3LIGHTS, BST_CHECKED);
	}

	// Light rotate
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"LightRotate",NULL,NULL,(BYTE*)&dwValue,&dwTemp))
		dwValue = 0;

	if (0 == dwValue)
	{
		g_sOptions.bLightRotate = false;
		CheckDlgButton(IDC_LIGHTROTATE,BST_UNCHECKED);
	}
	else 
	{
		g_sOptions.bLightRotate = true;
		CheckDlgButton(IDC_LIGHTROTATE,BST_CHECKED);
	}

	// NoSpecular
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"NoSpecular",NULL,NULL,(BYTE*)&dwValue,&dwTemp))
		dwValue = 0;

	if (0 == dwValue)
	{
		g_sOptions.bNoSpecular = false;
		CheckDlgButton(IDC_NOSPECULAR,BST_UNCHECKED);
	}
	else 
	{
		g_sOptions.bNoSpecular = true;
		CheckDlgButton(IDC_NOSPECULAR,BST_CHECKED);
	}

	// LowQuality
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"LowQuality",NULL,NULL,(BYTE*)&dwValue,&dwTemp))
		dwValue = 0;

	if (0 == dwValue)
	{
		g_sOptions.bLowQuality = false;
		CheckDlgButton(IDC_LOWQUALITY,BST_UNCHECKED);
	}
	else 
	{
		g_sOptions.bLowQuality = true;
		CheckDlgButton(IDC_LOWQUALITY,BST_CHECKED);
	}

	// LowQuality
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"NoTransparency",NULL,NULL,(BYTE*)&dwValue,&dwTemp))
		dwValue = 0;

	if (0 == dwValue)
	{
		g_sOptions.bNoAlphaBlending = false;
		CheckDlgButton(IDC_NOAB, BST_UNCHECKED);
	}
	else 
	{
		g_sOptions.bNoAlphaBlending = true;
		CheckDlgButton(IDC_NOAB, BST_CHECKED);
	}

	// DisplayNormals
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"RenderNormals",NULL,NULL,	(BYTE*)&dwValue,&dwTemp))
		dwValue = 0;
	if (0 == dwValue)
	{
		g_sOptions.bRenderNormals = false;
		CheckDlgButton(IDC_TOGGLENORMALS,BST_UNCHECKED);
	}
	else 
	{
		g_sOptions.bRenderNormals = true;
		CheckDlgButton(IDC_TOGGLENORMALS,BST_CHECKED);
	}

	// NoMaterials
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"RenderMats",NULL,NULL,(BYTE*)&dwValue,&dwTemp))
		dwValue = 1;

	if (0 == dwValue)
	{
		g_sOptions.bRenderMats = false;
		CheckDlgButton(IDC_TOGGLEMAT, BST_CHECKED);
	}
	else 
	{
		g_sOptions.bRenderMats = true;
		CheckDlgButton(IDC_TOGGLEMAT, BST_UNCHECKED);
	}

	// MultiSampling
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"MultiSampling",NULL,NULL,	(BYTE*)&dwValue,&dwTemp))
		dwValue = 1;

	if (0 == dwValue)
	{
		g_sOptions.bMultiSample = false;
		CheckDlgButton(IDC_TOGGLEMS,BST_UNCHECKED);
	}
	else 
	{
		g_sOptions.bMultiSample = true;
		CheckDlgButton(IDC_TOGGLEMS,BST_CHECKED);
	}

	// FPS Mode
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"FPSView",NULL,NULL,(BYTE*)&dwValue,&dwTemp))
		dwValue = 0;

	if (0 == dwValue)
	{
		g_bFPSView = false;
		CheckDlgButton(IDC_ZOOM,BST_CHECKED);
	}
	else 
	{
		g_bFPSView = true;
		CheckDlgButton(IDC_ZOOM,BST_UNCHECKED);
	}

	// WireFrame
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"Wireframe",NULL,NULL,(BYTE*)&dwValue,&dwTemp))
		dwValue = 0;

	if (0 == dwValue)
	{
		g_sOptions.eDrawMode = RenderOptions::NORMAL;
		CheckDlgButton(IDC_TOGGLEWIRE,BST_UNCHECKED);
	}
	else 
	{
		g_sOptions.eDrawMode = RenderOptions::WIREFRAME;
		CheckDlgButton(IDC_TOGGLEWIRE,BST_CHECKED);
	}

	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),"PostProcessing",NULL,NULL,(BYTE*)&dwValue,&dwTemp))
		ppsteps = ppstepsdefault;
	else 
		ppsteps = dwValue;

	SetupPPUIState();
	LoadCheckerPatternColors();

	SendDlgItemMessage(IDC_SLIDERANIM,TBM_SETRANGEMIN,TRUE,0);
	SendDlgItemMessage(IDC_SLIDERANIM,TBM_SETRANGEMAX,TRUE,10000);

	UpdateEdit(m_hWnd);		
	return;
}

//-------------------------------------------------------------------------------
// Toggle the "Display Normals" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleNormals()
{
	g_sOptions.bRenderNormals = !g_sOptions.bRenderNormals; 

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bRenderNormals)dwValue = 1;
	RegSetValueExA(GetRootReg(),"RenderNormals",0,REG_DWORD,(const BYTE*)&dwValue,4);
}

//-------------------------------------------------------------------------------
// Toggle the "AutoRotate" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleAutoRotate()
{
	g_sOptions.bRotate = !g_sOptions.bRotate; 

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bRotate)dwValue = 1;
	RegSetValueExA(GetRootReg(),"AutoRotate",0,REG_DWORD,(const BYTE*)&dwValue,4);
	UpdateWindow();
	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "FPS" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleFPSView()
{
	g_bFPSView = !g_bFPSView;
	SetupFPSView();

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_bFPSView)dwValue = 1;
	RegSetValueExA(GetRootReg(),"FPSView",0,REG_DWORD,(const BYTE*)&dwValue,4);

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "2 Light sources" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleMultipleLights()
{
	g_sOptions.b3Lights = !g_sOptions.b3Lights; 

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.b3Lights)dwValue = 1;
	RegSetValueExA(GetRootReg(),"MultipleLights",0,REG_DWORD,(const BYTE*)&dwValue,4);

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "LightRotate" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleLightRotate()
{
	g_sOptions.bLightRotate = !g_sOptions.bLightRotate; 

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bLightRotate)dwValue = 1;
	RegSetValueExA(GetRootReg(),"LightRotate",0,REG_DWORD,(const BYTE*)&dwValue,4);

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "NoTransparency" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleTransparency()
{
	g_sOptions.bNoAlphaBlending = !g_sOptions.bNoAlphaBlending;

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bNoAlphaBlending)dwValue = 1;
	RegSetValueExA(GetRootReg(),"NoTransparency",0,REG_DWORD,(const BYTE*)&dwValue,4);

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "LowQuality" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleLowQuality()
{
	g_sOptions.bLowQuality = !g_sOptions.bLowQuality; 

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bLowQuality)dwValue = 1;
	RegSetValueExA(GetRootReg(),"LowQuality",0,REG_DWORD,(const BYTE*)&dwValue,4);

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "Specular" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleSpecular()
{
	g_sOptions.bNoSpecular = !g_sOptions.bNoSpecular; 

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bNoSpecular)dwValue = 1;
	RegSetValueExA(GetRootReg(),"NoSpecular",0,REG_DWORD,(const BYTE*)&dwValue,4);

	// update all specular materials
	CMaterialManager::Instance().UpdateSpecularMaterials();

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "RenderMats" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleMats()
{
	g_sOptions.bRenderMats = !g_sOptions.bRenderMats; 

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bRenderMats)dwValue = 1;
	RegSetValueExA(GetRootReg(),"RenderMats",0,REG_DWORD,(const BYTE*)&dwValue,4);

	// update all specular materials
	CMaterialManager::Instance().UpdateSpecularMaterials();

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "Culling" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleCulling()
{
	g_sOptions.bCulling = !g_sOptions.bCulling; 

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bCulling)dwValue = 1;
	RegSetValueExA(GetRootReg(),"Culling",0,REG_DWORD,(const BYTE*)&dwValue,4);

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "Skeleton" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleSkeleton()
{
	g_sOptions.bSkeleton = !g_sOptions.bSkeleton; 

	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bCulling)dwValue = 1;
	RegSetValueExA(GetRootReg(),"Skeleton",0,REG_DWORD,(const BYTE*)&dwValue,4);

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Toggle the "WireFrame" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleWireFrame()
{
	if (g_sOptions.eDrawMode == RenderOptions::WIREFRAME)
		g_sOptions.eDrawMode = RenderOptions::NORMAL;
	else
		g_sOptions.eDrawMode = RenderOptions::WIREFRAME;

	// store this in the registry, too
	DWORD dwValue = 0;
	if (RenderOptions::WIREFRAME == g_sOptions.eDrawMode)
		dwValue = 1;

	RegSetValueExA(GetRootReg(),"Wireframe",0,REG_DWORD,(const BYTE*)&dwValue,4);

	::UpdateWindow(g_hView);
}


//-------------------------------------------------------------------------------
// Toggle the "MultiSample" state
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleMS()
{
	g_sOptions.bMultiSample = !g_sOptions.bMultiSample; 
	DeleteAssetData();
	ShutdownDevice();
	if (0 == CreateDevice(g_hView))
	{
		CLogDisplay::Instance().AddEntry("[ERROR] Failed to toggle MultiSampling mode");
		g_sOptions.bMultiSample = !g_sOptions.bMultiSample;
		CreateDevice(g_hView);
	}
	CreateAssetData();
	if (g_sOptions.bMultiSample)
	{
		CLogDisplay::Instance().AddEntry("[OK] Changed MultiSampling mode to the maximum value for this device");
	}
	else
	{
		CLogDisplay::Instance().AddEntry("[OK] MultiSampling has been disabled");
	}
	// store this in the registry, too
	DWORD dwValue = 0;
	if (g_sOptions.bMultiSample)
		dwValue = 1;
	RegSetValueExA(GetRootReg(),"MultiSampling",0,REG_DWORD,(const BYTE*)&dwValue,4);

	::UpdateWindow(g_hView);
}

//-------------------------------------------------------------------------------
// Expand or collapse the UI
//-------------------------------------------------------------------------------
void CSettingDialog::OnToggleUIState()
{

	DWORD dwValue;
	if (BST_UNCHECKED == IsDlgButtonChecked(IDC_BLUBB))
	{
		dwValue = 0;
		::SetWindowText(::GetDlgItem(m_hWnd,IDC_BLUBB),">>");
		RegSetValueExA(GetRootReg(),"LastUIState",0,REG_DWORD,(const BYTE*)&dwValue,4);
	}
	else
	{
		dwValue = 1;
		::SetWindowText(::GetDlgItem(m_hWnd,IDC_BLUBB),"<<");
		RegSetValueExA(GetRootReg(),"LastUIState",0,REG_DWORD,(const BYTE*)&dwValue,4);
	}
	::UpdateWindow(m_hWnd);
	::UpdateWindow(g_hView);
	return;
}


void CSettingDialog::OnEnChangeEdit()
{	
	CRect rc;
	CWnd* pWnd = NULL;
	pWnd = GetDlgItem(IDC_EVERT);
	if( pWnd != NULL)
	{
		pWnd->GetWindowRect(rc);
		ScreenToClient(rc);
		InvalidateRect(rc);
	}
	
	pWnd = GetDlgItem(IDC_ENODEWND);
	if( pWnd != NULL)
	{
		pWnd->GetWindowRect(rc);
		ScreenToClient(rc);
		InvalidateRect(rc);
	}

	pWnd = GetDlgItem(IDC_ESHADER);
	if( pWnd != NULL)
	{
		pWnd->GetWindowRect(rc);
		ScreenToClient(rc);
		InvalidateRect(rc);
	}

	pWnd = GetDlgItem(IDC_ELOAD);
	if( pWnd != NULL)
	{
		pWnd->GetWindowRect(rc);
		ScreenToClient(rc);
		InvalidateRect(rc);
	}
	
	pWnd = GetDlgItem(IDC_EFACE);
	if( pWnd != NULL)
	{
		pWnd->GetWindowRect(rc);
		ScreenToClient(rc);
		InvalidateRect(rc);
	}

	pWnd = GetDlgItem(IDC_EMAT);
	if( pWnd != NULL)
	{
		pWnd->GetWindowRect(rc);
		ScreenToClient(rc);
		InvalidateRect(rc);
	}

	pWnd = GetDlgItem(IDC_EMESH);
	if( pWnd != NULL)
	{
		pWnd->GetWindowRect(rc);
		ScreenToClient(rc);
		InvalidateRect(rc);
	}

	pWnd = GetDlgItem(IDC_EFPS);
	if( pWnd != NULL)
	{
		pWnd->GetWindowRect(rc);
		ScreenToClient(rc);
		InvalidateRect(rc);
	}
}

