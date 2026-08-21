// EditDoubleEx.cpp : 구현 파일입니다.
//

#include "stdafx.h"
#include "EditDoubleEx.h"

// CEditDoubleEx

IMPLEMENT_DYNAMIC(CEditDoubleEx, CEdit)

CEditDoubleEx::CEditDoubleEx()
{
	m_dMin = 0.0;
}

CEditDoubleEx::~CEditDoubleEx()
{
}


BEGIN_MESSAGE_MAP(CEditDoubleEx, CEdit)
	ON_CONTROL_REFLECT(EN_CHANGE, &CEditDoubleEx::OnEnChange)
END_MESSAGE_MAP()



// CEditDoubleEx 메시지 처리기입니다.
void CEditDoubleEx::SetMinimum(double dMin)
{
	m_dMin = dMin;
}

double CEditDoubleEx::GetMinimum() const
{
	return m_dMin;
}

void CEditDoubleEx::OnEnChange()
{
	// TODO:  RICHEDIT 컨트롤인 경우, 이 컨트롤은
	// CEditDouble::OnInitDialog() 함수를 재지정하고  마스크에 OR 연산하여 설정된
	// ENM_CHANGE 플래그를 지정하여 CRichEditCtrl().SetEventMask()를 호출해야만
	// 해당 알림 메시지를 보냅니다.

	// TODO:  여기에 컨트롤 알림 처리기 코드를 추가합니다.
	if (!m_bChanged)
	{
		CString str;
		GetWindowText(str);

		double data;
		bool bResult = Utility::StringManager::StrToDouble((const char*)(LPCTSTR)str,&data);
		if (bResult)
		{
			if (data < 0.0 && !m_bPermitMinus) bResult = false;
			else if (data < m_dMin) bResult = false;
		}
		else
		{
			if (str.GetLength() == 0)
			{
				data = 0.0;
				bResult = true;
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
