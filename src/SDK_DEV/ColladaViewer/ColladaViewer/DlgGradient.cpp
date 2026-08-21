#include "StdAfx.h"
#include "DlgGradient.h"
#include "resource.h"

#pragma comment(lib, "UxTheme.lib")

std::list<CDlgGradient*> CDlgGradient::DlgList;


COLORREF CDlgGradient::m_colorTop = 0;
COLORREF CDlgGradient::m_colorBottom = 0;

COLORREF RGB_BLUE = RGB(199,216,237);
COLORREF RGB_BLACK = RGB(180,187,197);
COLORREF RGB_SILVER = RGB(213,219,231);
COLORREF RGB_AQUA = RGB(195,202,217);
COLORREF RGB_BLUEBOTTOM = RGB(231,242,255);
COLORREF RGB_BLACKBOTTOM = RGB(230,240,241);
COLORREF RGB_SILVERBOTTOM = RGB(240,250,251);
COLORREF RGB_AQUABOTTOM = RGB(195,202,217);

CDlgGradient::CDlgGradient(UINT nID, CWnd* pParent)
	: CDialog(nID, pParent)
{
	m_bShow = FALSE;
	CDlgGradient::DlgList.push_back(this);
}

CDlgGradient::~CDlgGradient(void)
{
	m_listID.clear();
	m_CheckIdList.clear();
	if(CDlgGradient::DlgList.size() > 0)
	{
		DlgList.remove(this);
	}
}

BEGIN_MESSAGE_MAP(CDlgGradient, CDialog)
	ON_WM_ERASEBKGND()
	ON_WM_CTLCOLOR()
	ON_WM_SHOWWINDOW()
END_MESSAGE_MAP()

BOOL CDlgGradient::PreTranslateMessage(MSG* pMsg)
{
	if(pMsg->message == WM_KEYDOWN)
	{
		if(pMsg->wParam == VK_ESCAPE)
		{
			if( AfxGetApp()->GetMainWnd() != NULL)
				AfxGetApp()->GetMainWnd()->SendMessage(WM_KEYDOWN, (WPARAM)VK_ESCAPE, 0);
		}
		if(pMsg->wParam == VK_RETURN || pMsg->wParam == VK_ESCAPE)
			return TRUE;
	}	

	CWnd * pWnd = CWnd::FromHandle(pMsg->hwnd);		
	if( pWnd != NULL)
	{
		switch(pMsg->message)
		{
		case WM_LBUTTONDOWN:
		case WM_LBUTTONUP:
		case WM_KILLFOCUS:
			{
				CRect rc;
				std::list<UINT>::iterator it;
				for(it=m_CheckIdList.begin(); it!=m_CheckIdList.end(); ++it)
				{
					UINT nID = *it;
					CWnd * pWnd = GetDlgItem(nID);
					if(pWnd != NULL)		
					{					
						pWnd->GetWindowRect(rc);
						ScreenToClient(rc);
						InvalidateRect(rc);
					}		
				}
				for(it=m_listID.begin(); it!=m_listID.end(); ++it)
				{
					UINT nID = *it;
					CWnd * pWnd = GetDlgItem(nID);
					if(pWnd != NULL)		
					{					
						pWnd->GetWindowRect(rc);
						ScreenToClient(rc);
						InvalidateRect(rc);
					}		
				}
			}
		}
	}	

	return CDialog::PreTranslateMessage(pMsg);
}

void CDlgGradient::OnShowWindow(BOOL bShow, UINT nStatus)
{
	CDialog::OnShowWindow(bShow, nStatus);
	m_bShow = bShow;	
}

BOOL CDlgGradient::OnEraseBkgnd(CDC* pDC)
{
	CRect rect ;
	GetClientRect(&rect);

	COLORREF colorTop = m_colorTop;
	COLORREF colorBottom = m_colorBottom;

	int r1 = (int)(colorTop &0xff);
	int g1 = (int)((colorTop >> 8)&0xff);
	int b1 = (int)((colorTop >> 16)&0xff);

	int r2 = (int)(colorBottom &0xff);
	int g2 = (int)((colorBottom >> 8)&0xff);
	int b2 = (int)((colorBottom >> 16)&0xff);

	double dR = (double)(r2-r1) / 60.0;
	double dG = (double)(g2-g1) / 60.0;
	double dB = (double)(b2-b1) / 60.0;

	double red=r1, green=g1, blue=b1;

	CPen myPen[64];
	for (int i=0 ; i<=60 ; i++) 
	{
		red += dR;
		green += dG;
		blue += dB;

		BYTE r = (BYTE)red;
		BYTE g = (BYTE)green;
		BYTE b = (BYTE)blue;

		myPen[i].CreatePen(PS_SOLID, 1, RGB(r, g, b));
	}

	CPen *oldPen = pDC->SelectObject(&myPen[0]);

	for(int i=0 ; i<=rect.bottom; ++i)
	{
		pDC->MoveTo(0, i);
		pDC->LineTo(rect.right, i);
		pDC->SelectObject (&myPen[(i+1) * 64 / rect.bottom]);
	}
	pDC->SelectObject(oldPen);

	return FALSE;
}

HBRUSH CDlgGradient::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
		
	std::list<UINT>::iterator it;
	for(it=m_listID.begin(); it!=m_listID.end(); ++it)
	{
		UINT nID = *it;
		UINT nCtrlID = pWnd->GetDlgCtrlID();
		if(pWnd != NULL && pWnd->GetDlgCtrlID() == nID)		
		{
			if( CTLCOLOR_BTN == nCtlColor)
				pDC->SetTextColor(RGB(255,255,255));
			pDC->SetBkMode(TRANSPARENT);
			return (HBRUSH)GetStockObject(NULL_BRUSH);
		}		
	}

	std::list<UINT>::iterator it2;
	for( it2 = m_CheckIdList.begin(); it2 != m_CheckIdList.end(); ++it2)
	{
		UINT nID = *it2;
		UINT nCtrlID = pWnd->GetDlgCtrlID();
		if(pWnd != NULL && pWnd->GetDlgCtrlID() == nID)
		{
			pDC->SetBkMode(TRANSPARENT);
			return (HBRUSH)GetStockObject(NULL_BRUSH);
		}
	}
	return hbr;
}

void CDlgGradient::SetStyle(UINT nStyle)
{
	switch(nStyle)
	{
	case CMFCVisualManagerOffice2007::Office2007_LunaBlue:
		m_colorTop = RGB_BLUE;
		m_colorBottom = RGB_BLUEBOTTOM;
		break;
	case CMFCVisualManagerOffice2007::Office2007_ObsidianBlack:
		m_colorTop = RGB_BLACK;
		m_colorBottom = RGB_BLACKBOTTOM;
		break;
	case CMFCVisualManagerOffice2007::Office2007_Silver:
		m_colorTop = RGB_SILVER;
		m_colorBottom = RGB_SILVERBOTTOM;
		break;
	case CMFCVisualManagerOffice2007::Office2007_Aqua:
		m_colorTop = RGB_AQUA;
		m_colorBottom = RGB_AQUABOTTOM;
		break;
	default:
		m_colorTop = RGB_BLUE;
		m_colorBottom = RGB_BLUEBOTTOM;
		break;
	}
}

void CDlgGradient::AddStatic(UINT nID)
{
	m_listID.push_back(nID);
}

COLORREF CDlgGradient::GetTopColor()
{
	return m_colorTop;
}

COLORREF CDlgGradient::GetBottomColor()
{
	return m_colorBottom;
}

void CDlgGradient::RedrawAllDialog()
{
	std::list<CDlgGradient*>::iterator it;
	for(it = DlgList.begin(); it != DlgList.end(); ++it)
	{
		CDlgGradient * pDlg = *it;
		if( pDlg != NULL && pDlg->GetSafeHwnd())
		{
			pDlg->Invalidate(TRUE);
			pDlg->ChangeSytle();
		}
	}
}

BOOL CDlgGradient::OnInitDialog()
{
	CDialog::OnInitDialog();

	std::list<UINT>::iterator it;
	for(it = m_CheckIdList.begin(); it != m_CheckIdList.end(); ++it)
	{
		UINT nID = *it;
		CWnd * pWnd = GetDlgItem(nID);
		if( pWnd != NULL && nID == pWnd->GetDlgCtrlID())
		{
			SetWindowTheme(pWnd->m_hWnd, L"", L""); 						
		}
	}	
	return TRUE;  // return TRUE unless you set the focus to a control
}

void CDlgGradient::AddCheck( UINT nID )
{
	m_CheckIdList.push_back(nID);
}

void CDlgGradient::ChangeSytle()
{
}
