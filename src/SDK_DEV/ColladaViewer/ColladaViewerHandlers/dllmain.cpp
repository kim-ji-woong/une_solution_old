// dllmain.cpp : DllMain의 구현입니다.

#include "stdafx.h"
#include "resource.h"
#include "ColladaViewerHandlers_i.h"
#include "dllmain.h"
#include "xdlldata.h"

CColladaViewerHandlersModule _AtlModule;

class CColladaViewerHandlersApp : public CWinApp
{
public:

// 재정의
	virtual BOOL InitInstance();
	virtual int ExitInstance();

	DECLARE_MESSAGE_MAP()
};

BEGIN_MESSAGE_MAP(CColladaViewerHandlersApp, CWinApp)
END_MESSAGE_MAP()

CColladaViewerHandlersApp theApp;

BOOL CColladaViewerHandlersApp::InitInstance()
{
	if (!PrxDllMain(m_hInstance, DLL_PROCESS_ATTACH, NULL))
		return FALSE;
	return CWinApp::InitInstance();
}

int CColladaViewerHandlersApp::ExitInstance()
{
	return CWinApp::ExitInstance();
}
