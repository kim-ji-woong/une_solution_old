#pragma once
#include <afx.h>
#include "afxwin.h"

#define WM_LABELEDIT	WM_USER+1013

class __declspec(dllexport) CEditEx : public CEdit
{
public:
	CEditEx(int nDot = 0);
	virtual ~CEditEx(void);

	void	SetValue(double dValue);
	void	SetValue(int nValue);
	void	SetValue(CString strValue);

	double GetValueD();
	int GetValueN();
	CString GetValue();

private:
	int m_nType;
	int m_nDecimalPoint;

	double m_dValue;
	int m_nValue;
	CString m_strValue;

protected:
	DECLARE_MESSAGE_MAP()
	afx_msg void OnKillFocus(CWnd* pNewWnd);
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);

	virtual BOOL PreTranslateMessage(MSG* pMsg);

public:
	virtual void SetWindowText(LPCTSTR lpszString);
};
