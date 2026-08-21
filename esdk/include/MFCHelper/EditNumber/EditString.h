#pragma once
#include "afxwin.h"

#define WM_LABELEDIT	WM_USER+1013

class __declspec(dllexport) CEditString : public CEdit
{
public:
	CEditString(void);
	~CEditString(void);

	void	SetValue(CString str);
	CString GetValue();

protected:
	CString m_strValue;

protected:
	DECLARE_MESSAGE_MAP()
	afx_msg void OnKillFocus(CWnd* pNewWnd);
public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
};
