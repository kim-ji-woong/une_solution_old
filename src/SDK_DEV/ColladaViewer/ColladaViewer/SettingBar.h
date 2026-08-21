#pragma once

/////////////////////////////////////////////////////////////////////////////
// CCalendarBar 창
#include "SettingDialog.h"

class CSettingBar : public CWnd
{
// 생성입니다.
public:
	CSettingBar();

// 특성입니다.
protected:
	CSettingDialog m_wndSetting;

// 재정의입니다.
public:
	virtual BOOL Create(const RECT& rect, CWnd* pParentWnd, UINT nID = (UINT)-1);

// 구현입니다.
public:
	virtual ~CSettingBar();

protected:
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSize(UINT nType, int cx, int cy);

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnShowWindow(BOOL bShow, UINT nStatus);

};
