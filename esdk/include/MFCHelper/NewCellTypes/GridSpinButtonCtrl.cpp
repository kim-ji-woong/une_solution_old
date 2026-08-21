// SpinButtonCtrlEx.cpp : 구현 파일입니다.
//

#include "stdafx.h"
#include "GridSpinButtonCtrl.h"
#include "GridCellSpin.h"


// CGridSpinButtonCtrl

IMPLEMENT_DYNAMIC(CGridSpinButtonCtrl, CSpinButtonCtrl)

CGridSpinButtonCtrl::CGridSpinButtonCtrl()
{
	m_nTimerID = 0;
	m_nPrevPos = 0;
	m_pParentCell = 0;
}

CGridSpinButtonCtrl::~CGridSpinButtonCtrl()
{
}

int CGridSpinButtonCtrl::SetPos(int nPos)
{
	int nResult = CSpinButtonCtrl::SetPos(nPos);
	m_nPrevPos = GetPos();
	return nResult;
}

void CGridSpinButtonCtrl::SetParentCell(CGridCellSpin* pCell)
{
	m_pParentCell = pCell;
}

CGridCellSpin* CGridSpinButtonCtrl::GetParentCell() const
{
	return (CGridCellSpin*)m_pParentCell;
}

BEGIN_MESSAGE_MAP(CGridSpinButtonCtrl, CSpinButtonCtrl)
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_TIMER()
END_MESSAGE_MAP()



// CGridSpinButtonCtrl 메시지 처리기입니다.
//void CGridSpinButtonCtrl::OnDeltapos(int nDelta)
//{
//	CString strDelta;
//	strDelta.Format(_T("OnDeltaPos : %d\n"),nDelta);
//	TRACE(strDelta);
//}

void CGridSpinButtonCtrl::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO:
	if (m_nTimerID)
	{
		KillTimer(m_nTimerID);
	}

	CSpinButtonCtrl::OnLButtonDown(nFlags, point);

	int nCurrentPos = GetPos();
	if (m_pParentCell) m_pParentCell->OnDeltapos(nCurrentPos - m_nPrevPos);
	m_nPrevPos = nCurrentPos;

	m_nTimerID = SetTimer((UINT_PTR)this,100,0);
}

void CGridSpinButtonCtrl::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO:
	if (m_nTimerID)
	{
		KillTimer(m_nTimerID);
		m_nTimerID = 0;
	}

	CSpinButtonCtrl::OnLButtonDown(nFlags, point);
}

void CGridSpinButtonCtrl::OnTimer(UINT_PTR nIDEvent)
{
	// TODO:
	int nCurrentPos = GetPos();
	if (m_pParentCell) m_pParentCell->OnDeltapos(nCurrentPos - m_nPrevPos);
	m_nPrevPos = nCurrentPos;

	CSpinButtonCtrl::OnTimer(nIDEvent);
}
