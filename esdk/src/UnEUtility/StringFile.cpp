#include "stdAfx.h"
#include "StringFile.h"


StringFile::StringFile(std::wstring strData)
{
	m_wstrData = strData;
	m_nCurrentIndex = 0;
}

StringFile::StringFile(std::string strData)
{
	m_strData = strData;
	m_nCurrentIndex = 0;
}

StringFile::~StringFile(void)
{
}

void StringFile::SetData( std::wstring strData )
{
	m_wstrData = strData;
	m_nCurrentIndex = 0;
}

void StringFile::SetDataA( std::string strData )
{
	m_strData = strData;
	m_nCurrentIndex = 0;
}

std::string StringFile::ReadLineA( bool& isSuccess )
{
	int nLen = m_strData.length();
	if (m_nCurrentIndex >= nLen)
	{
		isSuccess = false;
		return "";
	}

	int nIndex = (int)m_strData.find(L'\n', m_nCurrentIndex);

	if (nIndex < 0)
	{
		std::string strResult = m_strData.substr(m_nCurrentIndex);
		m_nCurrentIndex = nLen;
		return strResult;
	}

	isSuccess = true;
	std::string strData = m_strData.substr(m_nCurrentIndex, nIndex - m_nCurrentIndex);
	m_nCurrentIndex = nIndex + 1;
	return strData;
}

std::wstring StringFile::ReadLine( bool& isSuccess )
{
	/*std::wstring strData;
	strData = m_wstrData;

	std::wstring::size_type stTmp;
	stTmp = strData.find(L'\n', 0);	// 문자열 중 해당 문자가 있는지 검색
	strData.erase(0, stTmp+1);      // 자르기 시작할 곳, 얼마만큼 자를 것인가?

	return strData;*/
	int nLen = m_wstrData.length();
	if (m_nCurrentIndex >= nLen)
	{
		isSuccess = false;
		return L"";
	}

	int nIndex = (int)m_wstrData.find(L'\n', m_nCurrentIndex);

	if (nIndex < 0)
	{
		std::wstring wstrResult = m_wstrData.substr(m_nCurrentIndex);
		m_nCurrentIndex = nLen;
		return wstrResult;
	}

	isSuccess = true;
	std::wstring strData = m_wstrData.substr(m_nCurrentIndex, nIndex - m_nCurrentIndex);
	m_nCurrentIndex = nIndex + 1;

	return strData;
}
