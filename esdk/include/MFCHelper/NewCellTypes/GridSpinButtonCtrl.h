#pragma once

class CGridCellSpin;

// CGridSpinButtonCtrl

class CGridSpinButtonCtrl : public CSpinButtonCtrl
{
	DECLARE_DYNAMIC(CGridSpinButtonCtrl)

public:
	CGridSpinButtonCtrl();
	virtual ~CGridSpinButtonCtrl();

public:
	int SetPos(int nPos);
	void SetParentCell(CGridCellSpin* pCell);
	CGridCellSpin* GetParentCell() const;

protected:
	//void OnDeltapos(int nDelta);

protected:
	UINT_PTR m_nTimerID;
	int m_nPrevPos;
	CGridCellSpin* m_pParentCell;

protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnTimer(UINT_PTR nIDEvent);
};
