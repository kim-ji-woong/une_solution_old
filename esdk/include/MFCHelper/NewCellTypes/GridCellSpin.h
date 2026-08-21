// GridCellSpin.h: interface for the CGridCellSpin class.
//
// Written by ±èÁö¿õ [kjw@vbuilders.co.kr]
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_GRIDSPINCELL_H__3479ED0D_B57D_4940_B83D_9E2296ED75B6__INCLUDED_)
#define AFX_GRIDSPINCELL_H__3479ED0D_B57D_4940_B83D_9E2296ED75B6__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "GridCellNumeric.h"
#include "GridSpinButtonCtrl.h"

class __declspec(dllexport) CGridCellSpin : public CGridCellNumeric  
{
    DECLARE_DYNCREATE(CGridCellSpin)

public:
	CGridCellSpin();

public:
    virtual BOOL Edit(int nRow, int nCol, CRect rect, CPoint point, UINT nID, UINT nChar);
	virtual BOOL Draw(CDC* pDC, int nRow, int nCol, CRect rect, BOOL bEraseBkgnd = TRUE);
	virtual void NonDraw();

public:
	void CreateSpinCtrl(CGridCtrl& rGrid);
	void OnDeltapos(int nDeltaPos);
	void SetRange(int nMin, int nMax);
	bool GetRange(int& rMin, int& rMax) const;

protected:
	CGridSpinButtonCtrl m_btnSpin;
	CRect m_rectCell, m_rectSpin;
	bool m_isSpinCreated;
	CGridCtrl* m_pGrid;
	bool m_setRange;
	int m_nMinData, m_nMaxData;
};

#endif // !defined(AFX_GRIDSPINCELL_H__3479ED0D_B57D_4940_B83D_9E2296ED75B6__INCLUDED_)
