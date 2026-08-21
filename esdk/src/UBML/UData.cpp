#include "StdAfx.h"
#include "UData.h"
#include <UnEUtility/StringManager.h>

BEGIN_NS(UnE)
BEGIN_NS(UBML)

Segment::Segment()
{
	m_strDescription=L"Text";
	m_strName = L"";
}

Segment::Segment(DataType type)
{
	SetType(type);
}

void Segment::SetType(DataType type)
{
	m_type = type;
}

void Segment::SetTagDescription(std::wstring strDesc)
{
	m_strDescription = strDesc;
}

const std::wstring& Segment::GetTagDescription() const
{
	return m_strDescription;
}

void Segment::SetTagName( std::wstring strName )
{
	m_strName = strName;
}

const std::wstring& Segment::GetTagName() const
{
	return m_strName;
}

unsigned char Segment::GetTypeTag(bool& isArray, size_t& rDataByteSize) const
{
	isArray = m_type >= __BYTE_ARR;

	int nByteSizeTag = (m_type % 100) % 30;

	if (nByteSizeTag <= 2)
		rDataByteSize = SIZE_BYTE;
	else if (nByteSizeTag < 4)
		rDataByteSize = SIZE_SHORT;
	else if (nByteSizeTag < 6)
		rDataByteSize = SIZE_LONG;
	else if (nByteSizeTag < 8)
		rDataByteSize = SIZE_LONGLONG;
	else if (nByteSizeTag == 9)
		rDataByteSize = SIZE_FLOAT;
	else if (nByteSizeTag == 11)
		rDataByteSize = SIZE_DOUBLE;
	else if (nByteSizeTag == 13)
		rDataByteSize = SIZE_BYTE;
	else if (nByteSizeTag == 15)
		rDataByteSize = SIZE_CHAR;
	else if (nByteSizeTag == 17)
		rDataByteSize = SIZE_WCHAR;
	else if (nByteSizeTag == 19)
		rDataByteSize = SIZE_LONG;

	// 첫번째 Bit는 배열인지 여부
	unsigned char tag = isArray ? 1 << 7 : 0;

	// 두 번째 Bit는 부호 여부(1이면 signed)
	if (m_type % 2 == 1)
	{
		tag = tag | (1 << 6);
	}

	int num = (m_type / 100) % 10;
	tag = tag | (unsigned char)num;

	return tag;
}

std::wstring Segment::GetTypeTagString(bool& isArray) const
{
	isArray = m_type >= __BYTE_ARR;

	int nDataType = (m_type % 100) % 30;

	switch (nDataType)
	{
	case 1:
		return L"BYTE";

	case 2:
		return L"UBYTE";

	case 3:
		return L"SHORT";

	case 4:
		return L"USHORT";

	case 5:
		return L"LONG";

	case 6:
		return L"ULONG";

	case 7:
		return L"LONGLONG";

	case 8:
		return L"ULONGLONG";

	case 9:
		return L"FLOAT";

	case 11:
		return L"DOUBLE";

	case 13:
		return L"BOOLEAN";

	case 15:
		return L"CHAR";

	case 17:
		return L"WCHAR";

	case 19:
		return L"ELEMENT";
	}

	return L"";
}

bool Segment::SetTypeTag(unsigned char typeTag, bool& isArray, size_t& rDataByteSize)
{
	isArray  = (typeTag & 0x80) ? true : false;
	unsigned char isSigned = typeTag & 0x40;
	unsigned char dataType = typeTag & 0x3f;

	switch (dataType)
	{
	case 1:
		if (isArray)
		{
			m_type = isSigned ? __BYTE_ARR : __UBYTE_ARR;
		}
		else
		{
			m_type = isSigned ? __BYTE : __UBYTE;
		}

		rDataByteSize = SIZE_BYTE;
		break;

	case 2:
		if (isArray)
		{
			m_type = isSigned ? __SHORT_ARR : __USHORT_ARR;
		}
		else
		{
			m_type = isSigned ? __SHORT : __USHORT;
		}

		rDataByteSize = SIZE_SHORT;
		break;

	case 3:
		if (isArray)
		{
			m_type = isSigned ? __LONG_ARR : __ULONG_ARR;
		}
		else
		{
			m_type = isSigned ? __LONG : __ULONG;
		}

		rDataByteSize = SIZE_LONG;
		break;

	case 4:
		if (isArray)
		{
			m_type = isSigned ? __LONGLONG_ARR : __ULONGLONG_ARR;
		}
		else
		{
			m_type = isSigned ? __LONGLONG : __ULONGLONG;
		}

		rDataByteSize = SIZE_LONGLONG;
		break;

	case 5:
		m_type = isArray ? __FLOAT_ARR : __FLOAT;
		rDataByteSize = SIZE_FLOAT;
		break;

	case 6:
		m_type = isArray ? __DOUBLE_ARR : __DOUBLE;
		rDataByteSize = SIZE_DOUBLE;
		break;

	case 7:
		m_type = isArray ? __BOOL_ARR : __BOOL;
		rDataByteSize = SIZE_BYTE;
		break;

	case 8:
		m_type = isArray ? __CHAR_ARR : __CHAR;
		rDataByteSize = SIZE_CHAR;
		break;

	case 9:
		m_type = isArray ? __WCHAR_ARR : __WCHAR;
		rDataByteSize = SIZE_WCHAR;
		break;

	// Segment는 Element가 될수 없음
	/*case 10:
		m_type = isArray ? __ELEMENT_ARR : __ELEMENT;
		rDataByteSize = SIZE_LONG;
		break;*/

	default:
		return false;
	}

	return true;
}

void Segment::AddData(bool data)
{
	__int64 nData;
	memcpy(&nData, &data, SIZE_BYTE);
	m_vecData.push_back(nData);
}

void Segment::AddData(char data)
{
	__int64 nData;
	memcpy(&nData, &data, SIZE_CHAR);
	m_vecData.push_back(nData);
}

void Segment::AddData(wchar_t data)
{
	__int64 nData;
	memcpy(&nData, &data, SIZE_WCHAR);
	m_vecData.push_back(nData);
}

void Segment::AddData(short data)
{
	__int64 nData;
	memcpy(&nData, &data, SIZE_SHORT);
	m_vecData.push_back(nData);
}

void Segment::AddData(int data)
{
	__int64 nData;
	memcpy(&nData, &data, SIZE_LONG);
	m_vecData.push_back(nData);
}

void Segment::AddData(__int64 data)
{
	__int64 nData;
	memcpy(&nData, &data, SIZE_LONGLONG);
	m_vecData.push_back(nData);
}

void Segment::AddData(float data)
{
	__int64 nData;
	memcpy(&nData, &data, SIZE_FLOAT);
	m_vecData.push_back(nData);
}

void Segment::AddData(double data)
{
	__int64 nData;
	memcpy(&nData, &data, SIZE_DOUBLE);
	m_vecData.push_back(nData);
}

UData::ClassType Segment::GetClassType() const
{
	return SEGMENT;
}



Element::Element(int nTag)
{
	m_nTag = nTag;
	m_strDescription = L"";
	m_vecData.reserve(50);
}

Element::~Element(void)
{
	RemoveAll(true);
}

template <class T>
static bool _MakeTag(const T* strHeader, int num, int& rData)
{
	rData = 0;
	if (strHeader == 0) return false;

	if (strHeader[0] == 0)
	{
		rData = num;
		return true;
	}
	else if (strHeader[1] == 0)
	{
		rData = (((int)strHeader[0]) << 24) | num;
		return true;
	}

	int n1 = (int)strHeader[0];
	int n2 = (int)strHeader[1];

	rData = (n1 << 24) | (n2 << 16) | num;
	return true;
}

template <class T>
static bool _MakeTag(const T* strTag, int& rData, size_t (*_StrLen)(const T*), bool (*_Str2Int)(const T*, int*))
{
	rData = 0;
	if (strTag == 0) return false;

	int nLen = _StrLen(strTag);
	if (nLen < 2) return false;

	int n3;
	if (!_Str2Int(&strTag[3], &n3))
		return false;

	int n1 = (int)strTag[0];
	int n2 = (int)strTag[1];

	rData = (n1 << 24) | (n2 << 16) | n3;
	return true;
}

bool Element::MakeTag(const char* strTag)
{
	return _MakeTag<char>(strTag, m_nTag, strlen, Utility::StringManager::StrToIntA);
}

bool Element::MakeTag(const wchar_t* strTag)
{
	return _MakeTag<wchar_t>(strTag, m_nTag, wcslen, Utility::StringManager::StrToInt);
}

bool Element::MakeTag(const char* strHeader, int num)
{
	return _MakeTag<char>(strHeader, num, m_nTag);
}

bool Element::MakeTag(const wchar_t* strHeader, int num)
{
	return _MakeTag<wchar_t>(strHeader, num, m_nTag);
}

void Element::SetTag(int nTag)
{
	m_nTag = nTag;
}

int Element::GetTag() const
{
	return m_nTag;
}

std::wstring Element::GetTagString() const
{
	char strHeader[4];
	memcpy(strHeader, &m_nTag, SIZE_LONG);

	wchar_t wstrTag[64];
	memset(wstrTag, 0, 64);

	if (((strHeader[3] >= 'a' && strHeader[3] <= 'z') || (strHeader[3] >= 'A' && strHeader[3] <= 'Z')) && 
		((strHeader[2] >= 'a' && strHeader[2] <= 'z') || (strHeader[2] >= 'A' && strHeader[2] <= 'Z')))
	{
		swprintf(wstrTag, 64, L"%c%c%d", strHeader[3], strHeader[2], *(short*)strHeader);
	}
	else
	{
		swprintf(wstrTag, 64, L"_%d_%d_%d", (unsigned char)strHeader[3], (unsigned char)strHeader[2], *(short*)strHeader);
	}

	/*if ((strHeader[3] >= 'a' && strHeader[3] <= 'z') || (strHeader[3] >= 'A' && strHeader[3] <= 'Z'))
		swprintf(wstrTag, 64, L"%c", strHeader[3]);
	else
		swprintf(wstrTag, 64, L"[%d]", strHeader[3]);

	int nLen = (int)wcslen(wstrTag);

	if ((strHeader[2] >= 'a' && strHeader[2] <= 'z') || (strHeader[2] >= 'A' && strHeader[2] <= 'Z'))
		swprintf(&wstrTag[nLen], 64 - nLen, L"%c", strHeader[2]);
	else
		swprintf(&wstrTag[nLen], 64 - nLen, L"[%d]", strHeader[2]);

	nLen = (int)wcslen(wstrTag);
	swprintf(&wstrTag[nLen], 64 - nLen, L"%d", *(short*)strHeader);*/

	return wstrTag;
}

void Element::SetDescription(std::wstring strDesc)
{
	m_strDescription = strDesc;
}

const std::wstring& Element::GetDescription() const
{
	return m_strDescription;
}

void Element::AddData(UData* pData)
{
	if (pData == 0) return;
	m_vecData.push_back(pData);
}

unsigned int Element::GetDataCount() const
{
	return (unsigned int)m_vecData.size();
}

const UData* Element::GetData(unsigned int nIndex) const
{
	if (nIndex >= GetDataCount())
		return 0;

	return m_vecData[nIndex];
}

void Element::RemoveData(unsigned int nBeginIndex, unsigned int nEndIndex, bool freeMemory)
{
	if (nBeginIndex > nEndIndex) return;

	unsigned int nCount = GetDataCount();
	if (nEndIndex > nCount) return;

	for (unsigned int i=nBeginIndex;i<=nEndIndex;i++)
	{
		std::vector<UData*>::iterator iter = m_vecData.begin() + nBeginIndex;
		if (freeMemory) delete *iter;
		m_vecData.erase(iter);
	}
}

void Element::RemoveData(unsigned int nIndex, bool freeMemory)
{
	if (nIndex >= GetDataCount())
		return;

	std::vector<UData*>::iterator iter = m_vecData.begin() + nIndex;
	if (freeMemory) delete *iter;
	m_vecData.erase(iter);
}

void Element::RemoveFirstData(bool freeMemory)
{
	if (GetDataCount() >= 0)
		return;

	if (freeMemory) delete *m_vecData.begin();
	m_vecData.erase(m_vecData.begin());
}

void Element::RemoveLastData(bool freeMemory)
{
	if (GetDataCount() >= 0)
		return;

	if (freeMemory)
	{
		std::vector<UData*>::iterator iter = m_vecData.end();
		iter--;
		delete *iter;
	}

	m_vecData.pop_back();
}

void Element::RemoveAll(bool freeMemory)
{
	if (freeMemory)
	{
		int nDataCount = (int)m_vecData.size();

		for (int i=0;i<nDataCount;i++)
		{
			delete m_vecData[i];
		}
	}

	m_vecData.clear();
}

UData::ClassType Element::GetClassType() const
{
	return ELEMENT;
}

END_NS
END_NS
