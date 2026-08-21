#pragma once

#include "Grid/GridCtrl.h"
#include <map>

// CGridCtrlEx

class __declspec(dllexport) CGridCtrlEx : public CGridCtrl
{
	DECLARE_DYNAMIC(CGridCtrlEx)

public:
	enum ColumnType {NORMAL = 0, COLOR, CHECK_BOX, IMAGE_CIRCLE};

public:
	CGridCtrlEx();
	virtual ~CGridCtrlEx();

public:
	void SetColumnType(ColumnType nType, int nCol);
	ColumnType GetColumnType(int nCol);
	const CCellID& GetClickedCell() const;

protected:
	virtual void OnEditCell(int nRow, int nCol, CPoint point, UINT nChar);

protected:
	std::map<int,ColumnType> m_mapColumnType;

protected:
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
};


