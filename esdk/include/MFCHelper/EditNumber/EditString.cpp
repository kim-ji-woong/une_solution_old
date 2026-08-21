#include "StdAfx.h"
#include "EditString.h"

CEditString::CEditString(void)
{
}

CEditString::~CEditString(void)
{
}

BEGIN_MESSAGE_MAP(CEditString, CEdit)
	ON_WM_KILLFOCUS()
END_MESSAGE_MAP()

void CEditString::SetValue(CString str)
{
	m_strValue = str;
	SetWindowText(str);
}

CString CEditString::GetValue()
{
	return m_strValue;
}

void CEditString::OnKillFocus(CWnd* pNewWnd)
{
	CEdit::OnKillFocus(pNewWnd);

	CString strValue;
	GetWindowText(strValue);

	if(m_strValue == strValue)
		return;

	SetValue(strValue);

	CWnd* pOwner = GetOwner();
	if(pOwner && IsWindow(pOwner->m_hWnd))
		pOwner->SendMessage(WM_LABELEDIT, GetDlgCtrlID(), (LPARAM)&strValue);
}

BOOL CEditString::PreTranslateMessage(MSG* pMsg)
{
	if(pMsg->message == WM_KEYDOWN)
	{
		if(pMsg->wParam == VK_RETURN)
		{
			CString strValue;
			GetWindowText(strValue);

			if(m_strValue == strValue)
				return TRUE;

			SetValue(strValue);

			CWnd* pOwner = GetOwner();
			if(pOwner && IsWindow(pOwner->m_hWnd))
				pOwner->SendMessage(WM_LABELEDIT, GetDlgCtrlID(), (LPARAM)&strValue);
		}
	}

	return CEdit::PreTranslateMessage(pMsg);
}
