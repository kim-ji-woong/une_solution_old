#include "stdafx.h"

#include "afxwinappex.h"
#include "afxdialogex.h"
#include "ColladaApp.h"
#include "MainFrm.h"

#include "ColladaDoc.h"
#include "ColladaView.h"


#include "AssimpView.h"

#include "Assimp/types.h"
#include "Assimp/Logger.hpp"
#include "Assimp/LogStream.hpp"
#include "AssimpLogWnd.h"

#ifdef _DEBUG
//#include <crtdbg.h>
#define new DEBUG_NEW
#endif

using namespace AssimpView;

BEGIN_MESSAGE_MAP(ColladaApp, CWinAppEx)
	ON_COMMAND(ID_APP_ABOUT, &ColladaApp::OnAppAbout)
	ON_COMMAND(ID_FILE_NEW, &CWinAppEx::OnFileNew)
	//ON_COMMAND(ID_FILE_OPEN, &CWinAppEx::OnFileOpen)
	ON_COMMAND(ID_FILE_PRINT_SETUP, &CWinAppEx::OnFilePrintSetup)
END_MESSAGE_MAP()


ColladaApp::ColladaApp()
{

	//_CrtSetBreakAlloc(608284);  
	//_CrtMemDumpAllObjectsSince(0);

	m_bHiColorIcons = TRUE;

	m_dwRestartManagerSupportFlags = AFX_RESTART_MANAGER_SUPPORT_ALL_ASPECTS;
#ifdef _MANAGED
	System::Windows::Forms::Application::SetUnhandledExceptionMode(System::Windows::Forms::UnhandledExceptionMode::ThrowException);
#endif

	SetAppID(_T("ColladaViewer.1.0"));

}

ColladaApp theApp;

BOOL ColladaApp::InitInstance()
{
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinAppEx::InitInstance();

	if (!AfxOleInit())
	{
		AfxMessageBox(IDP_OLE_INIT_FAILED);
		return FALSE;
	}

	AfxEnableControlContainer();
	EnableTaskbarInteraction(FALSE);

	AfxInitRichEdit2();

	SetRegistryKey(_T("UNE"));	
	LoadStdProfileSettings(4);  

	InitContextMenuManager();
	InitShellManager();
	InitKeyboardManager();
	InitTooltipManager();

	extern HINSTANCE g_hInstance;
	g_hInstance = m_hInstance;

	if( InitD3D() == FALSE)
		return FALSE;

	CMFCToolTipInfo ttParams;
	ttParams.m_bVislManagerTheme = TRUE;
	theApp.GetTooltipManager()->SetTooltipParams(AFX_TOOLTIP_TYPE_ALL,
		RUNTIME_CLASS(CMFCToolTipCtrl), &ttParams);

	CSingleDocTemplate* pDocTemplate;
	pDocTemplate = new CSingleDocTemplate(
		IDR_MAINFRAME,
		RUNTIME_CLASS(ColladaDoc),
		RUNTIME_CLASS(CMainFrame),       
		RUNTIME_CLASS(ColladaView));
	if (!pDocTemplate)
		return FALSE;
	AddDocTemplate(pDocTemplate);

	CCommandLineInfo cmdInfo;
	ParseCommandLine(cmdInfo);

	EnableShellOpen();
	RegisterShellFileTypes(TRUE);


	LONG nResult = RegOpenKeyEx(HKEY_CURRENT_USER, _T("Software\\UnE\\ColladaViewer"), 0, KEY_ALL_ACCESS  , &GetRootReg());
	if( nResult == ERROR_FILE_NOT_FOUND )
	{
		RegCreateKeyEx(HKEY_CURRENT_USER,  _T("Software\\UnE\\ColladaViewer"), 0, 0, 0, KEY_ALL_ACCESS, NULL, &GetRootReg(), 0);
	}

	if (!ProcessShellCommand(cmdInfo))
		return FALSE;
	
	m_pMainWnd->ShowWindow(SW_SHOW);
	m_pMainWnd->UpdateWindow();
	m_pMainWnd->DragAcceptFiles();

	Assimp::DefaultLogger::create("", Assimp::Logger::VERBOSE);
	CAssimpLogWnd::Instance().Init();	
	Assimp::DefaultLogger::get()->attachStream(CAssimpLogWnd::Instance().pcStream,
			Assimp::DefaultLogger::Debugging | Assimp::DefaultLogger::Info |
			Assimp::DefaultLogger::Err | Assimp::DefaultLogger::Warn);
		
	
	
	return TRUE;
}

int ColladaApp::ExitInstance()
{
	if( GetRootReg() != NULL)
		RegCloseKey(GetRootReg());
	AfxOleTerm(FALSE);

	ShutdownD3D();


	return CWinAppEx::ExitInstance();
}


//////////////////////////////////////////////////////////////////////////
// CAboutDlg

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

	enum { IDD = IDD_ABOUTBOX };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    


protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()

void ColladaApp::OnAppAbout()
{
	CAboutDlg aboutDlg;
	aboutDlg.DoModal();
}

void ColladaApp::PreLoadState()
{
	BOOL bNameValid;
	CString strName;
	bNameValid = strName.LoadString(IDS_EDIT_MENU);
	ASSERT(bNameValid);
	GetContextMenuManager()->AddMenu(strName, IDR_POPUP_EDIT);
}

void ColladaApp::LoadCustomState()
{
}

void ColladaApp::SaveCustomState()
{
}


