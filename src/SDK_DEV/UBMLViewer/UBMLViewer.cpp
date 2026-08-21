
#include "stdafx.h"
#include "afxwinappex.h"
#include "afxdialogex.h"
#include "UBMLViewer.h"
#include "MainFrm.h"

#include "UBMLViewerDoc.h"
#include "UBMLViewerView.h"

// Core API 
#include "UDB.h"
#include "UBaseDriver.h"
#include "UBaseView.h"
using namespace UnE::Core;

#ifdef _DEBUG
#include <crtdbg.h>
#define new DEBUG_NEW
#endif

// CUBMLViewerApp

BEGIN_MESSAGE_MAP(CUBMLViewerApp, CWinAppEx)
	ON_COMMAND(ID_APP_ABOUT, &CUBMLViewerApp::OnAppAbout)	
	ON_COMMAND(ID_FILE_NEW, &CWinAppEx::OnFileNew)
	ON_COMMAND(ID_FILE_OPEN, &CWinAppEx::OnFileOpen)
	ON_COMMAND(ID_FILE_PRINT_SETUP, &CWinAppEx::OnFilePrintSetup)
END_MESSAGE_MAP()

CUBMLViewerApp::CUBMLViewerApp()
{
	m_bHiColorIcons = TRUE;
	m_dwRestartManagerSupportFlags = AFX_RESTART_MANAGER_SUPPORT_ALL_ASPECTS;

#ifdef _MANAGED	
	System::Windows::Forms::Application::SetUnhandledExceptionMode(System::Windows::Forms::UnhandledExceptionMode::ThrowException);
#endif
		
	SetAppID(_T("UBMLViewer.AppID.NoVersion"));
}

CUBMLViewerApp theApp;


BOOL CUBMLViewerApp::InitInstance()
{
// 	//_CrtSetBreakAlloc(913);
// 	_CrtMemDumpAllObjectsSince(0);

	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinAppEx::InitInstance();
	
	CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED | COINIT_SPEED_OVER_MEMORY);//COINIT_MULTITHREADED | COINIT_SPEED_OVER_MEMORY);

	if (!AfxOleInit())
	{
		AfxMessageBox(IDP_OLE_INIT_FAILED);
		return FALSE;
	}

	AfxEnableControlContainer();

	EnableTaskbarInteraction(FALSE);
	AfxInitRichEdit2();

	SetRegistryKey(_T("UnE\\UBMLViewer"));

	

	LoadStdProfileSettings(4);  

	InitContextMenuManager();
	InitKeyboardManager();
	InitTooltipManager();

	
	CMFCToolTipInfo ttParams;
	ttParams.m_bVislManagerTheme = TRUE;
	theApp.GetTooltipManager()->SetTooltipParams(AFX_TOOLTIP_TYPE_ALL,
		RUNTIME_CLASS(CMFCToolTipCtrl), &ttParams);

	CSingleDocTemplate* pDocTemplate;
	pDocTemplate = new CSingleDocTemplate(
		IDR_MAINFRAME,
		RUNTIME_CLASS(CUBMLViewerDoc),
		RUNTIME_CLASS(CMainFrame),       
		RUNTIME_CLASS(CUBMLViewerView));
	if (!pDocTemplate)
		return FALSE;
	AddDocTemplate(pDocTemplate);


	HINSTANCE hIstance = AfxGetInstanceHandle();
	HKEY key = AfxGetApp()->GetAppRegistryKey();

	
	UBaseDriver::Instance().SetClient(hIstance);	
	UBaseDriver::Instance().SetRegistry(key);
	//UBaseDriver::Instance().ChangeRenderer(eRS_OPENGL);
	UBaseDriver::Instance().ChangeRenderer(eRS_DIRECT9);

	std::string szPath = "";
	GetWorkDir(szPath);
	std::string szAppName = "UBMLViewer";
	if( UnE::Core::UBaseDriver::Instance().InitDriver(szPath, szAppName) == false)
		return FALSE;

	CCommandLineInfo cmdInfo;
	ParseCommandLine(cmdInfo);

	EnableShellOpen();
	RegisterShellFileTypes(TRUE);

	if (!ProcessShellCommand(cmdInfo))
		return FALSE;



	m_pMainWnd->ShowWindow(SW_SHOW);
	m_pMainWnd->UpdateWindow();	
	m_pMainWnd->DragAcceptFiles();

	return TRUE;
}

int CUBMLViewerApp::ExitInstance()
{
	// Delete All Engine Resouces
	UnE::Core::UBaseDriver::Instance().DisposeDriver();

	//UDB * pDB = UDB::GetUDB();
	//if( pDB != NULL)
	//	delete pDB;

	AfxOleTerm(FALSE);

	CoUninitialize();

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


//////////////////////////////////////////////////////////////////////////

void CUBMLViewerApp::OnAppAbout()
{
	CAboutDlg aboutDlg;
	aboutDlg.DoModal();
}

void CUBMLViewerApp::PreLoadState()
{
	BOOL bNameValid;
	CString strName;
	bNameValid = strName.LoadString(IDS_EDIT_MENU);
	ASSERT(bNameValid);
	GetContextMenuManager()->AddMenu(strName, IDR_POPUP_EDIT);
	bNameValid = strName.LoadString(IDS_EXPLORER);
	ASSERT(bNameValid);
	GetContextMenuManager()->AddMenu(strName, IDR_POPUP_EXPLORER);
}

void CUBMLViewerApp::LoadCustomState()
{
}

void CUBMLViewerApp::SaveCustomState()
{
}



BOOL CUBMLViewerApp::OnIdle(LONG lCount)
{
	//UBaseView::Instance().RenderScene();

	return CWinAppEx::OnIdle(lCount);
}



void CUBMLViewerApp::GetWorkDir( std::string& strAppPath )
{
	const size_t bufsz = MAX_PATH * 2;
	char szPath[bufsz] = {0,};
	BOOL bDirOk = FALSE;
	CString m_strAppDirectory;
	// get engine dll location
	if ( ::GetModuleFileNameA(NULL, szPath, bufsz) != 0 )
	{
		if ( GetLastError() != ERROR_INSUFFICIENT_BUFFER )
		{
			CString strPath(szPath);
			// add seperator
			m_strAppDirectory = strPath.Left(strPath.ReverseFind(_T('\\')) + 1);
			// save path
			strAppPath = std::string(m_strAppDirectory.GetBuffer());
			bDirOk = TRUE;
		}
	}
	if ( !bDirOk )
	{
		::memset(szPath, 0, sizeof(TCHAR) * bufsz);
		// get current dir
		if ( GetCurrentDirectoryA(bufsz, szPath) != 0 )
		{
			m_strAppDirectory.SetString(szPath);
			// add seperator
			m_strAppDirectory += _T('\\');
			// save path
			strAppPath = std::string(m_strAppDirectory.GetBuffer());
		}
	}
}