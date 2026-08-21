#include "stdafx.h"
#include "EditInt.h"

IMPLEMENT_DYNAMIC(CEditInt, CEdit)

CEditInt::CEditInt()
{
	m_bPermitMinus = false;
	m_strPrev = "";
	m_bChanged = false;
	m_nData = 0;
	m_nClassType = 0;

	m_bMax = m_bMin = false;
	m_nDataMax = (long)(((unsigned long)2 << 30) - 1);
	m_nDataMin = (long)((unsigned long)2 << 30);
}

CEditInt::~CEditInt()
{
}

BEGIN_MESSAGE_MAP(CEditInt, CEdit)
	ON_CONTROL_REFLECT(EN_CHANGE, &CEditInt::OnEnChange)
END_MESSAGE_MAP()

int CEditInt::GetClassType() const
{
	return m_nClassType;
}

void CEditInt::OnEnChange()
{
	// TODO:  RICHEDIT 컨트롤인 경우, 이 컨트롤은
	// CEdit::OnInitDialog() 함수를 재지정하고  마스크에 OR 연산하여 설정된
	// ENM_CHANGE 플래그를 지정하여 CRichEditCtrl().SetEventMask()를 호출해야만
	// 해당 알림 메시지를 보냅니다.

	if (!m_bChanged)
	{
		CString str;
		GetWindowText(str);

		int data;
		bool bResult = Utility::StringManager::StrToInt((const char*)(LPCTSTR)str,&data);
		if (bResult)
		{
			if (data < 0 && !m_bPermitMinus) bResult = false;

			else if (m_bMax && data > m_nDataMax) bResult = false;
			else if (m_bMin && data < m_nDataMin) bResult = false;
		}
		else
		{
			int nLen = str.GetLength();

			if (nLen == 0)
			{
SET_ZERO_DATA:
				data = 0;
				bResult = true;
			}
			else if (nLen == 1)
			{
				char ch = str.GetAt(0);
				if (ch == '-' && m_bPermitMinus) goto SET_ZERO_DATA;
				else if (ch == '+') goto SET_ZERO_DATA;
			}
		}

		if (bResult)
		{
			m_strPrev = str;
			m_nData = data;
		}
		else
		{
			m_bChanged = true;
			SetWindowText(m_strPrev);
		}
	}
	else m_bChanged = false;
}

void CEditInt::SetData(int nData)
{
	m_nData = nData;
}

int CEditInt::GetData() const
{
	return m_nData;
}

void CEditInt::InitText()
{
	CString str;
	str.Format("%d",m_nData);

	SetWindowText((LPCTSTR)str);
}

void CEditInt::SetMaxUse(bool bPermit)
{
	m_bMax = bPermit;
}

void CEditInt::SetMinUse(bool bPermit)
{
	m_bMin = bPermit;
}

bool CEditInt::GetMaxUse() const
{
	return m_bMax;
}

bool CEditInt::GetMinUse() const
{
	return m_bMin;
}

// bMax : true이면 최대값
//        false이면 최소값
void CEditInt::SetData(int nData, bool bMax)
{
	if (bMax) m_nDataMax = nData;
	else m_nDataMin = nData;
}

int CEditInt::GetData(bool bMax) const
{
	return bMax ? m_nDataMax : m_nDataMin;
}

void CEditInt::SetPermitMinus(bool bPermit)
{
	m_bPermitMinus = bPermit;
}

bool CEditInt::GetPermitMinus() const
{
	return m_bPermitMinus;
}
