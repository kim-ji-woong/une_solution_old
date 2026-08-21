#pragma once

#include <list>
#include "afxwin.h"

class CDlgGradient : public CDialog
{
public:
	CDlgGradient(UINT nID, CWnd* pParent);
	~CDlgGradient(void);

	static void SetStyle(UINT nStyle);
	static std::list<CDlgGradient*> DlgList;
	static void RedrawAllDialog();
	
private:
	static COLORREF m_colorTop;
	static COLORREF m_colorBottom;
	static COLORREF GetTopColor();
	static COLORREF GetBottomColor();
	
	std::list<UINT> m_listID;
	std::list<UINT> m_CheckIdList;
	BOOL m_bShow;
	
protected:
	void AddStatic(UINT nID);
	void AddCheck(UINT nID);
	BOOL IsShow() { return m_bShow; }
	DECLARE_MESSAGE_MAP()
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	afx_msg void OnShowWindow(BOOL bShow, UINT nStatus);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
public:
	virtual BOOL OnInitDialog();
	virtual void ChangeSytle();
};
