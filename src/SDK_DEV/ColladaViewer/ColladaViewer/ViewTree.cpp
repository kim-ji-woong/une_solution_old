
#include "stdafx.h"
#include "ViewTree.h"

#include "Display.h"

using namespace AssimpView;

namespace AssimpView
{
	extern HWND g_hDlg							/*= NULL*/;	
}

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CViewTree

CViewTree::CViewTree()
{
}

CViewTree::~CViewTree()
{
}

BEGIN_MESSAGE_MAP(CViewTree, CTreeCtrl)
	ON_NOTIFY_REFLECT(NM_RCLICK, &CViewTree::OnNMRClick)
	ON_NOTIFY_REFLECT(TVN_SELCHANGED, &CViewTree::OnTvnSelchanged)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CViewTree 메시지 처리기

BOOL CViewTree::OnNotify(WPARAM wParam, LPARAM lParam, LRESULT* pResult)
{
	BOOL bRes = CTreeCtrl::OnNotify(wParam, lParam, pResult);
	
	return TRUE;
}



void CViewTree::OnNMRClick(NMHDR *pNMHDR, LRESULT *pResult)
{
	HTREEITEM hItem = GetSelectedItem();
	if( hItem != NULL)
	{
		UpdateEdit(g_hDlg);
		CDisplay::Instance().ShowTreeViewContextMenu(hItem);
		CWnd * pWnd = AfxGetApp()->GetMainWnd()->GetActiveWindow();
		::UpdateWindow(pWnd->m_hWnd);		
	}
	*pResult = 0;
}


void CViewTree::OnTvnSelchanged(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMTREEVIEW pNMTreeView = reinterpret_cast<LPNMTREEVIEW>(pNMHDR);
	HTREEITEM hItem = GetSelectedItem();
	if( hItem != NULL)
	{
		UpdateEdit(g_hDlg);
		CDisplay::Instance().OnSetup( hItem );
		CWnd * pWnd = AfxGetApp()->GetMainWnd()->GetActiveWindow();
		::UpdateWindow(pWnd->m_hWnd);		
	}

	*pResult = 0;
}
