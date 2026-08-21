// PaneLayer.cpp : 구현 파일입니다.
//

#include "stdafx.h"
#include "PlantViewer.h"
#include "PaneLayer.h"
#include "NewCellTypes/GridCellCheck.h"
#include "afxdialogex.h"
#include <vector>


// PaneLayer 대화 상자입니다.
using namespace std;
IMPLEMENT_DYNAMIC(PaneLayer, CPaneDialog)

PaneLayer::PaneLayer(CWnd* pParent /*=NULL*/)
	: CPaneDialog()
{

}

PaneLayer::~PaneLayer()
{
}

void PaneLayer::DoDataExchange(CDataExchange* pDX)
{
	CPaneDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_STATIC_GRID, m_staticGrid);
}


BEGIN_MESSAGE_MAP(PaneLayer, CPaneDialog)
	ON_WM_SIZE()
	ON_WM_CREATE()
END_MESSAGE_MAP()


// PaneLayer 메시지 처리기입니다.


void PaneLayer::OnSize(UINT nType, int cx, int cy)
{
	CPaneDialog::OnSize(nType, cx, cy);

	CRect rect;
	GetClientRect(rect);

	static bool bInit = false;
	if(m_staticGrid.GetSafeHwnd() && !bInit)
	{
		bInit = true;
		InitGrid();
	}
}


int PaneLayer::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CPaneDialog::OnCreate(lpCreateStruct) == -1)
		return -1;

	if(!m_imageList)
		m_imageList.Create(MAKEINTRESOURCE(IDB_BMP_BULB), 16, 1, RGB(255,255,255));
	m_gridLayer.SetImageList(&m_imageList);

	GetSystemTime(&m_time);
	
	return 0;
}

static void AddGridRow(CGridCtrl& rGrid, int nRow, bool bEnable,/*int nImage,*/ CString strName)
{
	CString strTemp;

	if (nRow >= rGrid.GetRowCount())
	{
		rGrid.SetRowCount(nRow + 1);
	}

	// ON
	GV_ITEM Item;
// 	Item.mask = GVIF_IMAGE | GVIF_FORMAT;
// 	Item.row = nRow;
// 	Item.col = 0;
// 	Item.iImage = nImage;
// 	Item.nFormat = DT_CENTER | DT_VCENTER | DT_SINGLELINE;
// 	rGrid.SetItem(&Item);
	
// checkbox
	rGrid.SetCellType(nRow, 0, RUNTIME_CLASS(CGridCellCheck));
	CGridCellCheck* pCell = (CGridCellCheck*)rGrid.GetCell(nRow, 0);
	DWORD dwState = pCell->GetState();
	pCell->SetState(0);
	pCell->SetCheck(bEnable ? TRUE : FALSE);
	pCell->SetState(dwState);
	pCell->SetPosition(1);

	// Name
	Item.mask = GVIF_TEXT | GVIF_FORMAT;
	Item.row = nRow;
	Item.col = 1;
	Item.strText = strName;
	Item.nFormat = DT_LEFT | DT_VCENTER | DT_SINGLELINE;
	rGrid.SetItem(&Item);

	rGrid.SetRedraw(TRUE);
}

void PaneLayer::InitGrid()
{
	WINDOWPLACEMENT pl;
	CWnd* pWndStatic = GetDlgItem(IDC_STATIC_GRID);
	pWndStatic->GetWindowPlacement(&pl);

	pWndStatic->ShowWindow(SW_HIDE);
	m_gridLayer.Create(pl.rcNormalPosition,this, (int)(__int64)&m_gridLayer, WS_BORDER | WS_TABSTOP | WS_VISIBLE | WS_CHILD);

	m_gridLayer.SetListMode(false);
	m_gridLayer.SetEditable(false); // ReadOnly
	m_gridLayer.SetFixedColumnSelection(FALSE);
	m_gridLayer.SetColumnResize(FALSE);

	m_gridLayer.SetRowCount(1);
	m_gridLayer.SetFixedRowCount(1); // 고정행
	m_gridLayer.SetColumnCount(2);
 
	int nColumnCount = m_gridLayer.GetColumnCount();

	GV_ITEM Item;
	Item.mask = GVIF_TEXT | GVIF_FORMAT;

	for (int i = 0; i < nColumnCount; i++)
	{
		Item.col = i;
		m_gridLayer.SetItemFormat(0, i, DT_CENTER | DT_VCENTER | DT_SINGLELINE);

		if (i == 0)
		{
			m_gridLayer.SetColumnWidth(i, 45);
			m_gridLayer.SetColumnType(CGridCtrlEx::CHECK_BOX, i);
			m_gridLayer.SetItemText(0, i, _T("ON"));
		}
		else if (i == 1)
		{
			m_gridLayer.SetColumnWidth(i, 135);
			m_gridLayer.SetItemText(0, i, _T("Name"));
			m_gridLayer.SetItemFormat(0, i, DT_CENTER | DT_VCENTER| DT_SINGLELINE);
		}

		m_gridLayer.SetItem(&Item);
	}

	m_gridLayer.SetRowHeight(0, 30);

	int nHeight = m_gridLayer.GetRowHeight(0);
	//int nRowCount = (pl.rcNormalPosition.bottom - pl.rcNormalPosition.top) / nHeight;
	//m_gridLayer.SetRowCount(nRowCount);

	for(int nRow = 1; nRow < m_gridLayer.GetRowCount(); nRow++)
	{
		m_gridLayer.GetCell(nRow, 0)->SetState(GVIS_READONLY);
		//m_gridLayer.SetItemState(nRow,0, m_gridLayer.GetItemState(nRow, 0) | GVIS_READONLY);
	}
}

void PaneLayer::AddItem()
{
	//m_GridList.clear();
	DeleteItem();

	std::vector<CString> vecMessage;
	int nMsgCount = thePlayer->GetMessageList(vecMessage);

	//char* str[] = {"a1_off", "a1_on", "a2_off", "a2_on", "a3_on", "a3_off", "lakj", "Level", "kkk"};
	//int nArrCount = sizeof(str) / sizeof(char*);
	int nRow = 1;
	int nImage = 0;
	bool bEnable = true;
	//Grid_info grid_info;

	for (int i = 0; i < nMsgCount-1; i++)
	{
		CString temp, strLeft, strRight;
		temp = vecMessage[i];

		int cnt = temp.ReverseFind('_');

		if(cnt > 0)
		{
			strLeft = temp.Left(cnt);
			strRight = temp.Right(temp.GetLength() - cnt - 1);
				
			if(strRight == _T("off") || strRight == _T("on"))
			{
				for (int j = i+1; j < nMsgCount; j++)
				{
					temp = vecMessage[j];
					int cnt1 = temp.ReverseFind('_');
					CString strLeft1 = temp.Left(cnt);
					CString strRight1 = temp.Right(temp.GetLength() - cnt - 1);
						
					if(strLeft == strLeft1  && (strRight == _T("off") || strRight == _T("on")) && (strRight1 == _T("off") || strRight1 == _T("on")))
					{
						AddGridRow(m_gridLayer, nRow++, bEnable,/*nImage,*/ strLeft);
						//grid_info.nImage = nImage;
						//grid_info.strName = strLeft;
						
						//m_GridList.push_back(grid_info);
					}
				}
			}
		}
	}
}


void PaneLayer::DeleteItem()
{
	int nRowCount = m_gridLayer.GetRowCount();
	for (int i = 1;i < nRowCount; i++)
	{
		m_gridLayer.DeleteRow(1);
	}

	m_gridLayer.Refresh();
}


// Return 값 : milli second
static int GetTimeDiff(SYSTEMTIME t1, SYSTEMTIME t2)
{
	if (t1.wHour != t2.wHour || t1.wMinute != t2.wMinute)
		return 10000;

	int nTime1 = t1.wSecond * 1000 + t1.wMilliseconds;
	int nTime2 = t2.wSecond * 1000 + t2.wMilliseconds;

	if (nTime2 > nTime1) nTime1 += 60000;
	return nTime1 - nTime2;
}

class CGridCtrlPane : CGridCtrl
{
public:
	CPoint GetClicked()
	{
		return m_LeftClickDownPoint;
	}
};

BOOL PaneLayer::OnNotify(WPARAM wParam, LPARAM lParam, LRESULT* pResult)
{
// 	if (wParam == (WPARAM)&m_gridLayer)
// 	{
// 		CGridCtrlPane& rGrid = (CGridCtrlPane&)m_gridLayer;
// 		CPoint pt = rGrid.GetClicked();
// 
// 		CRect rect;
// 		m_gridLayer.GetClientRect(&rect);
// 
//  		int nRowCount = m_gridLayer.GetRowCount();
// 		int nHeight = 0;
//
// 		for (int i=0;i<nRowCount;i++)
// 		{
// 			nHeight += m_gridLayer.GetRowHeight(i);
// 		}
// 
// 		if(nHeight >= pt.y)
// 		{
// 			NM_GRIDVIEW* pGridView = (NM_GRIDVIEW*)lParam;
// 
// 			if(pGridView->iColumn == 0)
// 			{
// 				if(pGridView->iRow <= 0 ) return FALSE;
// 
// 				int nImage = m_gridLayer.GetItemImage(pGridView->iRow, 0);
// 
// 				GV_ITEM Item;
// 				Item.mask = GVIF_IMAGE | GVIF_FORMAT;
// 				Item.row = pGridView->iRow;
// 				Item.col = 0;
// 
// 				CString strTag;
// 
// 				if(nImage == 0)
// 				{
// 					Item.iImage = 1;
// 					strTag = _T("_off");
// 				}
// 				else
// 				{
// 					Item.iImage = 0;
// 					strTag = _T("_on");
// 				}
// 
// 				Item.nFormat = DT_CENTER | DT_VCENTER | DT_SINGLELINE;
// 				m_gridLayer.SetItem(&Item);
// 
// 				SYSTEMTIME t;
// 				GetSystemTime(&t);
// 
// 				int nDiff = GetTimeDiff(t, m_time);
// 
// 				if (nDiff > 300)
// 				{
// 					CString strMessage = m_gridLayer.GetItemText(pGridView->iRow, 1) + strTag;
// 					thePlayer->SendMessage("Level", (char*)(LPCTSTR)strMessage);
// 					m_time = t;
// 				}
// 
// 				m_gridLayer.SetSelectedRange(pGridView->iRow, 1, pGridView->iRow, 0, FALSE);
// 				return TRUE;
// 			}
//		}
//	}

	int nRowCount = m_gridLayer.GetRowCount();
	CCellID id = m_gridLayer.GetClickedCell();

	for( int i = 1; i < nRowCount ; i++ )
	{
		CGridCellCheck* pCell = (CGridCellCheck*)m_gridLayer.GetCell(i, 0);
		BOOL bChecked = pCell->GetCheck();

		CString strTag = _T("_off");
		if( bChecked == TRUE)
		{				
			strTag = _T("_on");
		}

		CString strMessage = m_gridLayer.GetItemText(i, 1) + strTag;
		thePlayer->SendMessage("Level", (char*)(LPCTSTR)strMessage);
	}

	return CPaneDialog::OnNotify(wParam, lParam, pResult);
}
