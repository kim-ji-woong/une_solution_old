
#include "stdafx.h"
#include "SettingBar.h"
#include "ColladaApp.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace AssimpView;

namespace AssimpView
{
	extern HWND g_hDlg;
}

const int nBorderSize = 10;

/////////////////////////////////////////////////////////////////////////////
// CCalendarBar

CSettingBar::CSettingBar()
{
}

CSettingBar::~CSettingBar()
{
}

BEGIN_MESSAGE_MAP(CSettingBar, CWnd)
	ON_WM_CREATE()
	ON_WM_SIZE()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCalendarBar 메시지 처리기

int CSettingBar::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CWnd::OnCreate(lpCreateStruct) == -1)
		return -1;

	CRect rectDummy(0, 0, 10, 10);
	m_wndSetting.Create(CSettingDialog::IDD, this);
	m_wndSetting.InitUI();
	m_wndSetting.ShowWindow(TRUE);
	g_hDlg = m_wndSetting.GetSafeHwnd();

	return 0;
}

void CSettingBar::OnSize(UINT nType, int cx, int cy)
{
	CWnd::OnSize(nType, cx, cy);

	int nMyCalendarsHeight = 70;

	if( cx != 0 && cy != 0)
	{	
		if (m_wndSetting.GetSafeHwnd() != NULL)
		{
			m_wndSetting.SetWindowPos(NULL, 0, 0, cx, cy, 0);
		}
	}
}

BOOL CSettingBar::Create(const RECT& rect, CWnd* pParentWnd, UINT nID)
{
	return CWnd::Create(NULL, _T(""), WS_CHILD | WS_VISIBLE, rect, pParentWnd, nID);
}

