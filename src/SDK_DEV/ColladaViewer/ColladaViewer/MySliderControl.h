#if !defined(AFX_MYSLIDERCONTROL_H__C76FA857_51CC_4EB6_A8E2_8323BBEF10BD__INCLUDED_)
#define AFX_MYSLIDERCONTROL_H__C76FA857_51CC_4EB6_A8E2_8323BBEF10BD__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MySliderControl.h : header file
//
//
//
// Copyright (C) 2001 by Nic Wilson
// All rights reserved
//
// This is free software.
// This code may be used in compiled form in any way you desire. This  
// file may be redistributed unmodified by any means PROVIDING it is   
// not sold for profit without the authors written consent, and   
// providing that this notice and the authors name and all copyright   
// notices remains intact. If the source code in this file is used in   
// any  commercial application then a statement along the lines of   
// "Portions Copyright ?2001 Nic Wilson" must be included in   
// the startup banner, "About" box and printed documentation. An email   
// letting me know that you are using it would be nice.
//
// No warrantee of any kind, expressed or implied, is included with this
// software; use at your own risk, responsibility for damages (if any) to
// anyone resulting from the use of this software rests entirely with the
// user.
//
// Version: 1.0
// Release: 1 (www.codeguru.com)
// -----------------------------------------------------------------------
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
// CMySliderControl window

class CMySliderControl : public CSliderCtrl
{
	CDC			m_dcBk;
	CBitmap		m_bmpBk;
	CBitmap     *m_bmpBkOld;
	//CBitmap*	m_pbmpOldBk;
	CPen m_penThumb;
	CPen m_penThumbLight;
	CPen m_penThumbLighter;
	CPen m_penThumbDark;
	CPen m_penThumbDarker;
	COLORREF m_crThumb;
	COLORREF m_crThumbLight;
	COLORREF m_crThumbLighter;
	COLORREF m_crThumbDark;
	COLORREF m_crThumbDarker;


// Construction
public:
	CMySliderControl();

// Attributes
public:

// Operations
public:
	void ChangeSytle();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMySliderControl)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CMySliderControl();

	// Generated message map functions
protected:
	//{{AFX_MSG(CMySliderControl)
		afx_msg void OnCustomDraw(NMHDR* pNMHDR, LRESULT* pResult);
		afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MYSLIDERCONTROL_H__C76FA857_51CC_4EB6_A8E2_8323BBEF10BD__INCLUDED_)
