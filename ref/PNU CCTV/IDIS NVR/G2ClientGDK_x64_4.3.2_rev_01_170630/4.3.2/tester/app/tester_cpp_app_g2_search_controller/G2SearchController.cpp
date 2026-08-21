// G2SearchController.cpp : implementation file
//

#include "stdafx.h"
#include "G2SearchController.h"
#include "G2SearchControllerView.h"
#include "G2SearchControllerDlg.h"

#include <sampler/cpp/app/g2app_frame_relay.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// G2SearchControllerApp

BEGIN_MESSAGE_MAP(G2SearchControllerApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// G2SearchControllerApp construction

G2SearchControllerApp::G2SearchControllerApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only G2SearchControllerApp object

G2SearchControllerApp theApp;


// G2SearchControllerApp initialization

BOOL G2SearchControllerApp::InitInstance()
{
	// InitCommonControlsEx() is required on Windows XP if an application
	// manifest specifies use of ComCtl32.dll version 6 or later to enable
	// visual styles.  Otherwise, any window creation will fail.
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// Set this to include all the common control classes you want to use
	// in your application.
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();

	AfxEnableControlContainer();

	// Standard initialization
	// If you are not using these features and wish to reduce the size
	// of your final executable, you should remove from the following
	// the specific initialization routines you do not need
	// Change the registry key under which our settings are stored
	// TODO: You should modify this string to be something appropriate
	// such as the name of your company or organization
	SetRegistryKey(_T("G2CLIENT\\GDK\\Tester"));

	G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION option = { 0 };
	if (__argc == 2 && client::g2app_frame_relay::search_controller_option_read_file(__targv[1], option)) {
	    G2SearchControllerView view(option);
	    view.CreateEx(NULL, AfxRegisterWndClass(0), _T("View"), WS_OVERLAPPEDWINDOW, CRect(), NULL, NULL);
    
	    if (view.connect_relay()) {
	        m_pMainWnd = &view.controller_ref();
	        INT_PTR nResponse = view.controller_ref().DoModal();
	    }
	    view.DestroyWindow();
	}

	// Since the dialog has been closed, return FALSE so that we exit the
	//  application, rather than start the application's message pump.
	return FALSE;
}
