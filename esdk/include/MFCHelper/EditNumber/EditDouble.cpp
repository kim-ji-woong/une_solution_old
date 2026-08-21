// EditDouble.cpp : 구현 파일입니다.
//

#include "stdafx.h"
#include "EditDouble.h"

// CEditDouble

IMPLEMENT_DYNAMIC(CEditDouble, CEdit)

CEditDouble::CEditDouble()
{
	m_bPermitMinus = false;
	m_strPrev = "";
	m_bChanged = false;
	m_dData = 0.0;
	m_nClassType = 1;

	m_bMax = m_bMin = false;
	m_dDataMax = (long)(((unsigned long)2 << 30) - 1);
	m_dDataMin = (long)((unsigned long)2 << 30);
}

CEditDouble::~CEditDouble()
{
}


BEGIN_MESSAGE_MAP(CEditDouble, CEdit)
	ON_CONTROL_REFLECT(EN_CHANGE, &CEditDouble::OnEnChange)
END_MESSAGE_MAP()



// CEditDouble 메시지 처리기입니다.
int CEditDouble::GetClassType() const
{
	return m_nClassType;
}

void CEditDouble::OnEnChange()
{
	// TODO:  RICHEDIT 컨트롤인 경우, 이 컨트롤은
	// CEdit::OnInitDialog() 함수를 재지정하고  마스크에 OR 연산하여 설정된
	// ENM_CHANGE 플래그를 지정하여 CRichEditCtrl().SetEventMask()를 호출해야만
	// 해당 알림 메시지를 보냅니다.

	if (!m_bChanged)
	{
		CString str;
		GetWindowText(str);

		double data;
		bool bResult = Utility::StringManager::StrToDouble((const char*)(LPCTSTR)str,&data);
		if (bResult)
		{
			if (data < 0.0 && !m_bPermitMinus) bResult = false;
			else if (m_bMax && data > m_dDataMax) bResult = false;
			else if (m_bMin && data < m_dDataMin) bResult = false;
		}
		else
		{
			int nLen = str.GetLength();

			if (nLen == 0)
			{
SET_ZERO_DATA:
				data = 0.0;
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
			m_dData = data;
		}
		else
		{
			m_bChanged = true;
			SetWindowText(m_strPrev);
		}
	}
	else m_bChanged = false;
}

void CEditDouble::SetData(double dData)
{
	m_dData = dData;
}

double CEditDouble::GetData() const
{
	return m_dData;
}

// nSize : 소수점 몇자리까지 표시할 것인가?
void CEditDouble::InitText(unsigned int nSize)
{
	CString str, strFormat;
	strFormat.Format("%%.%dlf",nSize);
	str.Format((char*)(LPCTSTR)strFormat,m_dData);

	SetWindowText((LPCTSTR)str);
}

void CEditDouble::SetMaxUse(bool bPermit)
{
	m_bMax = bPermit;
}

void CEditDouble::SetMinUse(bool bPermit)
{
	m_bMin = bPermit;
}

bool CEditDouble::GetMaxUse() const
{
	return m_bMax;
}

bool CEditDouble::GetMinUse() const
{
	return m_bMin;
}

// bMax : true이면 최대값
//        false이면 최소값
void CEditDouble::SetData(double dData, bool bMax)
{
	if (bMax) m_dDataMax = dData;
	else m_dDataMin = dData;
}

double CEditDouble::GetData(bool bMax) const
{
	return bMax ? m_dDataMax : m_dDataMin;
}
