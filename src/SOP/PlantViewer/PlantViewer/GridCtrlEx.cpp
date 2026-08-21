// GridCtrlEx.cpp : 구현 파일입니다.
//

#include "stdafx.h"
#include "GridCtrlEx.h"
#include "NewCellTypes/GridCellCheck.h"

// CGridCtrlEx

IMPLEMENT_DYNAMIC(CGridCtrlEx, CGridCtrl)

CGridCtrlEx::CGridCtrlEx()
{
}

CGridCtrlEx::~CGridCtrlEx()
{
}


BEGIN_MESSAGE_MAP(CGridCtrlEx, CGridCtrl)
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONDBLCLK()
	ON_WM_MOUSEMOVE()
END_MESSAGE_MAP()

void CGridCtrlEx::SetColumnType(ColumnType nType, int nCol)
{
	m_mapColumnType[nCol] = nType;
}

CGridCtrlEx::ColumnType CGridCtrlEx::GetColumnType(int nCol)
{
	if (m_mapColumnType.find(nCol) == m_mapColumnType.end())
	{
		return NORMAL;
	}

	return m_mapColumnType[nCol];
}

// CGridCtrlEx 메시지 처리기입니다.



void CGridCtrlEx::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	CCellID id = GetCellFromPt(point);
	ColumnType type = GetColumnType(id.col);

	CGridCellBase* pCellBase = GetCell(id.row,id.col);
	if (pCellBase == 0) 
		return CGridCtrl::OnLButtonDown(nFlags, point);

	DWORD dwState = pCellBase->GetState();

	if (id.row >= GetFixedRowCount() && type != NORMAL && (dwState & GVIS_READONLY) != GVIS_READONLY)
	{
		if (type == CHECK_BOX)
		{
			//CGridCellCheck* pCell = (CGridCellCheck*)GetCell(id.row,id.col);
			CGridCellCheck* pCell = (CGridCellCheck*)pCellBase;
			pCell->SetCheck(!pCell->GetCheck());
			Invalidate();
		}
		else if (type == IMAGE_CIRCLE)
		{
			int nImageCount = GetImageList()->GetImageCount();
			int nImage = pCellBase->GetImage() + 1;

			if (nImage >= nImageCount) nImage = 0;
			pCellBase->SetImage(nImage);
			Invalidate();

			m_bLMouseButtonDown   = TRUE;
			m_LeftClickDownPoint = point;
			m_LeftClickDownCell  = id;
		}
	}
	else CGridCtrl::OnLButtonDown(nFlags, point);
}

void CGridCtrlEx::OnEditCell(int nRow, int nCol, CPoint point, UINT nChar)
{
	CGridCellBase* pCell = GetCell(nRow, nCol);

	if ((pCell->GetState() & GVIS_NOTEXT) == GVIS_NOTEXT)
	{
		return;
	}

	CGridCtrl::OnEditCell(nRow,nCol,point,nChar);
}

void CGridCtrlEx::OnLButtonUp(UINT nFlags, CPoint point)
{
	CGridCtrl::OnLButtonUp(nFlags,point);
}

void CGridCtrlEx::OnLButtonDblClk(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	CCellID id = GetCellFromPt(point);
	ColumnType type = GetColumnType(id.col);

	CGridCellBase* pCellBase = GetCell(id.row,id.col);
	if (pCellBase == 0) return;

	DWORD dwState = pCellBase->GetState();

	if (id.row >= GetFixedRowCount() && type != NORMAL && (dwState & GVIS_READONLY) != GVIS_READONLY)
	{
		if (type == COLOR)
		{
			CColorDialog dlg;
			if (dlg.DoModal() == IDOK)
			{
				SetItemBkColour(id.row,id.col,dlg.GetColor());
				Invalidate();
			}
		}
	}
	else CGridCtrl::OnLButtonDblClk(nFlags, point);
}

const CCellID& CGridCtrlEx::GetClickedCell() const
{
	return m_LeftClickDownCell;
}

void CGridCtrlEx::OnMouseMove(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	// KYJ
	// 다이얼로그가 두개이상 떠있고 두개의 다이얼로그가 그리드를 가지고 있을때
	// 하나의 그리드에 포커스가 가있는 상태에서 다른 그리드에 마우스무브가 일어나면 뻗는다.
	// 그래서 막았다
	return;
	//CGridCtrl::OnMouseMove(nFlags, point);
}
