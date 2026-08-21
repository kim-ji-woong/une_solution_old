#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(HEADER)

CData::CData(void)
{
	m_strVariable.empty();
	m_nType = 0;
	m_nCode = 9;
	m_nCode1 = 0;	
	m_nCode2 = 0;
	m_nCode3 = 0;
	m_fValue1 = 0;
	m_fValue2 = 0;
	m_fValue3 = 0;
	m_strValue.empty();
}

CData::~CData(void)
{
}

// Type1(Data가 정수 또는 실수인 헤더변수)를 추가
void CData::SetData(wstring strVariable, int nCode1, double fValue)
{
	m_strVariable = strVariable;
	m_nCode1 = nCode1;

	if(m_nCode1 >= 62)
	{
		m_nType = 5;
		m_nValue1 = (int)(fValue + 0.1);
	}
	else
	{
		m_nType = 1;
		m_fValue1 = fValue;
	}
}

// Type2(Data가 문자열인 헤더변수)를 추가
void CData::SetData(wstring strVariable, int nCode1, wstring strValue)
{
	m_nType = 2;
	m_strVariable = strVariable;
	m_nCode1 = nCode1;
	m_strValue = strValue;
}

// Type3(code와 Data가 3개씩 존재하는 헤더변수)를 추가
void CData::SetData(wstring strVariable, int nCode1, double fValue1, int nCode2, double fValue2, int nCode3, double fValue3)
{
	m_nType = 3;
	m_strVariable = strVariable;
	m_nCode1 = nCode1;
	m_nCode2 = nCode2;
	m_nCode3 = nCode3;
	m_fValue1 = fValue1;
	m_fValue2 = fValue2;
	m_fValue3 = fValue3;
}

void CData::SetData(wstring strVariable, int nCode1, double fValue1, int nCode2, double fValue2)
{
	m_nType = 4;
	m_strVariable = strVariable;
	m_nCode1 = nCode1;
	m_nCode2 = nCode2;
	m_fValue1 = fValue1;
	m_fValue2 = fValue2;
}

int	CData::GetType()	// Type를 반환
{
	return m_nType;
}

// Type1을 반환
bool CData::GetData_Type1(wstring& strVariable, int& nCode, double& fValue)
{
	if(m_strVariable == L"" || m_nCode1 == 0)
		return false;

	strVariable = m_strVariable;
	nCode = m_nCode1;
	fValue = m_fValue1;

	return true;
}

// Type2를 반환
bool CData::GetData_Type2(wstring& strVariable, int& nCode, wstring& strValue)
{
	if(m_strVariable == L"" || m_nCode1 == 0)
		return false;
	
	strVariable = m_strVariable;
	nCode = m_nCode1;
	strValue = m_strValue;

	return true;
}

// Type3을 반환
bool CData::GetData_Type3(wstring& strVariable, int& nCode, int& nCode1, double& fValue1, int& nCode2, double& fValue2, int& nCode3, double& fValue3)
{
	if(m_strVariable == L"" || m_nCode1 == 0 || m_nCode2 == 0 || m_nCode3 == 0)
		return false;

	strVariable = m_strVariable;
	nCode = m_nCode;
	nCode1 = m_nCode1;
	nCode2 = m_nCode2;
	nCode3 = m_nCode3;
	fValue1 = m_fValue1;
	fValue2 = m_fValue2;
	fValue3 = m_fValue3;

	return true;
}

bool CData::GetData_Type4(wstring& strVariable, int& nCode, int& nCode1, double& fValue1, int& nCode2, double& fValue2)
{
	if(m_strVariable == L"" || m_nCode1 == 0 || m_nCode2 == 0)
		return false;

	strVariable = m_strVariable;
	nCode = m_nCode;
	nCode1 = m_nCode1;
	nCode2 = m_nCode2;
	fValue1 = m_fValue1;
	fValue2 = m_fValue2;

	return true;
}

// Update Data
bool CData::UpdateData(double fValue)
{
	if(m_strVariable == L"" || m_nCode1 == 0)
		return false;

	m_fValue1 = fValue;

	return true;
}

bool CData::UpdateData(wstring strValue)
{
	if(m_strVariable == L"" || m_nCode1 == 0)
		return false;
	
	m_strValue = strValue;

	return true;
}

bool CData::UpdateData(double fValue1, double fValue2)
{
	if(m_strVariable == L"" || m_nCode1 == 0 || m_nCode2 == 0)
		return false;

	m_fValue1 = fValue1;
	m_fValue2 = fValue2;

	return true;
}

bool CData::UpdateData(double fValue1, double fValue2, double fValue3)
{
	if(m_strVariable == L"" || m_nCode1 == 0 || m_nCode2 == 0 || m_nCode3 == 0)
		return false;

	m_fValue1 = fValue1;
	m_fValue2 = fValue2;
	m_fValue3 = fValue3;

	return true;
}

void CData::Write(Utility::FileManager* pMgr)
{
	wchar_t strBuff[256];
	memset(strBuff, 0, 256);

	if(m_nType == 1)
	{
		swprintf_s(strBuff, L"%3d\r\n$%s\r\n%3d\r\n%g\r\n", m_nCode, m_strVariable.c_str(), m_nCode1, m_fValue1);
	}
	else if(m_nType == 2)
	{
		swprintf_s(strBuff, L"%3d\r\n$%s\r\n%3d\r\n%s\r\n", m_nCode, m_strVariable.c_str(), m_nCode1, m_strValue.c_str());
	}
	else if(m_nType == 3)
	{
		swprintf_s(strBuff, L"%3d\r\n$%s\r\n%3d\r\n%g\r\n%3d\r\n%g\r\n%3d\r\n%g\r\n", m_nCode, m_strVariable.c_str(), m_nCode1, m_fValue1, m_nCode2, m_fValue2, m_nCode3, m_fValue3);
	}
	else if(m_nType == 4)
	{
		swprintf_s(strBuff, L"%3d\r\n$%s\r\n%3d\r\n%g\r\n%3d\r\n%g\r\n", m_nCode, m_strVariable.c_str(), m_nCode1, m_fValue1, m_nCode2, m_fValue2);
	}
	else
	{
		swprintf_s(strBuff, L"%3d\r\n$%s\r\n%3d\r\n%6d\r\n", m_nCode, m_strVariable.c_str(), m_nCode1, m_nValue1);
	}

	pMgr->Write(strBuff,0,FILE_CURRENT, Utility::FileManager::WRITE_REPLACE);
}

int CData::GetIntValue()
{
	return m_nValue1;
}

END_NS
END_NS