#include "StdAfx.h"
#include "EditEx.h"

CEditEx::CEditEx(int nDot)
{
	m_nType = 0;
	m_nDecimalPoint = nDot;
}

CEditEx::~CEditEx(void)
{
}

BEGIN_MESSAGE_MAP(CEditEx, CEdit)
	ON_WM_KILLFOCUS()
	ON_WM_KEYDOWN()
END_MESSAGE_MAP()

void CEditEx::SetValue(double dValue)
{
	m_nType = 2;
	m_dValue = dValue;

	CString str;
	if(m_nDecimalPoint == 1)
		str.Format("%.1f", dValue);
	else if(m_nDecimalPoint == 2)
		str.Format("%.2f", dValue);
	else if(m_nDecimalPoint == 3)
		str.Format("%.3f", dValue);
	else
		str.Format("%g", dValue);

	CEdit::SetWindowText(str);
}

void CEditEx::SetValue(int nValue)
{
	m_nType = 1;
	m_nValue = nValue;

	CString str;
	str.Format("%d", nValue);
	CEdit::SetWindowText(str);
}

void CEditEx::SetValue(CString strValue)
{
	m_nType = 0;
	m_strValue = strValue;

	CEdit::SetWindowText(strValue);
}

double CEditEx::GetValueD()
{
	return m_dValue;
}

int CEditEx::GetValueN()
{
	return m_nValue;
}

CString CEditEx::GetValue()
{
	CString str;
	GetWindowText(str);

	return str;
}

void CEditEx::OnKillFocus(CWnd* pNewWnd)
{
	CEdit::OnKillFocus(pNewWnd);

	double param = 0;
	CString strValue;
	GetWindowText(strValue);
	if(m_nType == 0)
	{
		if(m_strValue == strValue)
			return;

		SetValue(strValue);

		char* pChar = (char*)(LPCTSTR)strValue;

		CWnd* pOwner = GetOwner();
		if(pOwner && IsWindow(pOwner->m_hWnd))
			pOwner->SendMessage(WM_LABELEDIT, GetDlgCtrlID(), (LPARAM)pChar);
	}
	else if(m_nType == 1)
	{
		int nValue = atoi(strValue);
		if(nValue == m_nValue)
			return;

		SetValue(nValue);

		CWnd* pOwner = GetOwner();
		if(pOwner && IsWindow(pOwner->m_hWnd))
			pOwner->SendMessage(WM_LABELEDIT, GetDlgCtrlID(), (LPARAM)&nValue);
	}
	else
	{
		double dValue = atof(strValue);
		if(dValue == m_dValue)
			return;

		SetValue(dValue);

		CWnd* pOwner = GetOwner();
		if(pOwner && IsWindow(pOwner->m_hWnd))
			pOwner->SendMessage(WM_LABELEDIT, GetDlgCtrlID(), (LPARAM)&dValue);
	}
}

void CEditEx::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	CEdit::OnKeyDown(nChar, nRepCnt, nFlags);
}

BOOL CEditEx::PreTranslateMessage(MSG* pMsg)
{
	if(pMsg->message == WM_KEYDOWN)
	{
		if(pMsg->wParam == VK_RETURN)
		{
			double param = 0;
			CString strValue;
			GetWindowText(strValue);
			if(m_nType == 0)
			{
				if(m_strValue == strValue)
					return TRUE;

				SetValue(strValue);

				char* pChar = (char*)(LPCTSTR)strValue;

				CWnd* pOwner = GetOwner();
				if(pOwner && IsWindow(pOwner->m_hWnd))
					pOwner->SendMessage(WM_LABELEDIT, GetDlgCtrlID(), (LPARAM)pChar);
			}
			else if(m_nType == 1)
			{
				int nValue = atoi(strValue);
				if(nValue == m_nValue)
					return TRUE;

				SetValue(nValue);

				CWnd* pOwner = GetOwner();
				if(pOwner && IsWindow(pOwner->m_hWnd))
					pOwner->SendMessage(WM_LABELEDIT, GetDlgCtrlID(), (LPARAM)&nValue);
			}
			else
			{
				double dValue = atof(strValue);
				if(dValue == m_dValue)
					return TRUE;

				SetValue(dValue);

				CWnd* pOwner = GetOwner();
				if(pOwner && IsWindow(pOwner->m_hWnd))
					pOwner->SendMessage(WM_LABELEDIT, GetDlgCtrlID(), (LPARAM)&dValue);
			}
			return TRUE;
		}
	}

	return CEdit::PreTranslateMessage(pMsg);
}

void CEditEx::SetWindowText(LPCTSTR lpszString)
{
	m_dValue = atof(lpszString);
	m_nValue = atoi(lpszString);
	m_strValue = lpszString;

	CEdit::SetWindowText(lpszString);
}
