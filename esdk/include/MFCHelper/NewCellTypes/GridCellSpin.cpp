// CGridCellSpin.cpp: implementation of the CGridCellSpin class.
//
// Written by ±èÁö¿õ [kjw@vbuilders.co.kr]
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "GridCellSpin.h"
#include "../Grid/inplaceedit.h"
#include "../Grid/GridCtrl.h"
// #include "../../VBUtil/VBUStringManager.h"
#include "UnEUtility/StringManager.h"

IMPLEMENT_DYNCREATE(CGridCellSpin, CGridCellNumeric)

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
CGridCellSpin::CGridCellSpin()
{
	m_isSpinCreated = false;
	m_pGrid = 0;
	m_setRange = false;
}

// Create a control to do the editing
BOOL CGridCellSpin::Edit(int nRow, int nCol, CRect rect, CPoint /* point */, UINT nID, UINT nChar)
{
    m_bEditing = TRUE;

	if (m_isSpinCreated)
	{
		rect.right -= m_rectSpin.Width();
	}
    
    // CInPlaceEdit auto-deletes itself
    m_pEditWnd = new CInPlaceEdit(GetGrid(), rect, /*GetStyle() |*/ ES_NUMBER, nID, nRow, nCol,
		GetText(), nChar);

    return TRUE;
}

BOOL CGridCellSpin::Draw(CDC* pDC, int nRow, int nCol, CRect rect, BOOL bEraseBkgnd)
{
	if (m_rectCell != rect)
	{
		m_rectSpin = rect;

		if (rect.Width() > 20)
		{
			m_rectSpin.left = m_rectSpin.right - 20;
		}

		m_btnSpin.MoveWindow(&m_rectSpin,TRUE);
		m_rectCell = rect;
	}

	if (m_isSpinCreated)
	{
		rect.right -= m_rectSpin.Width();
		m_btnSpin.ShowWindow(SW_SHOW);
	}

	return __super::Draw(pDC, nRow, nCol, rect, bEraseBkgnd);
}

void CGridCellSpin::NonDraw()
{
	m_btnSpin.ShowWindow(SW_HIDE);
}

void CGridCellSpin::CreateSpinCtrl(CGridCtrl& rGrid)
{
	m_isSpinCreated = true;
	m_rectCell.SetRect(0,0,0,0);
	m_btnSpin.Create(WS_CHILD | WS_VISIBLE, CRect(0, 0, 20, 20), &rGrid, (UINT)(INT_PTR)this);

	m_btnSpin.SetRange(0,30000);
	m_btnSpin.SetPos(15000);
	m_btnSpin.SetParentCell(this);
	m_btnSpin.ShowWindow(SW_HIDE);

	m_pGrid = &rGrid;
}

void CGridCellSpin::OnDeltapos(int nDeltaPos)
{
	int nData = 0;

#ifdef UNICODE
	UnE::Utility::StringManager::StrToInt((const wchar_t*)GetText(),&nData);
#else
	UnE::Utility::StringManager::StrToIntA((const char*)GetText(),&nData);
#endif

	int num = nData + nDeltaPos;

	if (m_setRange)
	{
		if (num < m_nMinData) num = m_nMinData;
		else if (num > m_nMaxData) num = m_nMaxData;
	}

	CString strText;
	strText.Format(_T("%d"), num);
	SetText(strText);

	if (m_pGrid)
	{
		CRect rect = m_rectCell;
		rect.right -= m_rectSpin.Width();
		m_pGrid->InvalidateRect(&rect,TRUE);
	}
}

void CGridCellSpin::SetRange(int nMin, int nMax)
{
	if (nMin > nMax) return;

	m_setRange = true;
	m_nMinData = nMin;
	m_nMaxData = nMax;
}

bool CGridCellSpin::GetRange(int& rMin, int& rMax) const
{
	if (!m_setRange) return false;

	rMin = m_nMinData;
	rMax = m_nMaxData;
	return true;
}
