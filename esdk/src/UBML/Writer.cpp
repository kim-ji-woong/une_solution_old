#include "StdAfx.h"
#include "Writer.h"
//#include <algorithm>
#include "UData.h"
#include <stdarg.h>
#include "XMLWriter.h"
#include "UBMLWriter.h"

BEGIN_NS(UnE)
BEGIN_NS(UBML)

static const unsigned char ELEMENT_TYPE_TAG = 10;

void SetErrorMessage(std::wstring& rString, wchar_t* strFormat, ...)
{
	wchar_t str[512];

	va_list marker;
	va_start(marker,strFormat);

	vswprintf(str, 512, strFormat, marker);
	va_end(marker);

	rString = str;
}

Writer::Writer(void)
{
}

Writer::~Writer(void)
{
	RemoveAll(true);
}

Writer::Writer(const Writer&)
{
}

void Writer::operator =(const Writer&)
{
}

void Writer::AddElement(Element* pElement)
{
	if (pElement == 0) return;

	// 정렬해선 안된다. 데이터 표기 순서가 헝클어지기 때문이다.
	/*if (std::find(m_vecElement.begin(), m_vecElement.end(), pData) == m_vecElement.end())
	{
		m_vecElement.push_back(pData);
		std::sort(m_vecElement.begin(), m_vecElement.end());
	}*/
	unsigned int nElementCount = GetElementCount();

	for (unsigned int i=0;i<nElementCount;i++)
	{
		if (m_vecElement[i] == pElement)
			return;
	}

	m_vecElement.push_back(pElement);
}

unsigned int Writer::GetElementCount() const
{
	return (unsigned int)m_vecElement.size();
}

const Element* Writer::GetElement(unsigned int nIndex) const
{
	if (nIndex >= GetElementCount())
		return 0;

	return m_vecElement[nIndex];
}

void Writer::RemoveElement(unsigned int nBeginIndex, unsigned int nEndIndex, bool freeMemory)
{
	if (nBeginIndex > nEndIndex) return;

	unsigned int nCount = GetElementCount();
	if (nEndIndex > nCount) return;

	for (unsigned int i=nBeginIndex;i<=nEndIndex;i++)
	{
		std::vector<Element*>::iterator iter = m_vecElement.begin() + nBeginIndex;
		if (freeMemory) *iter;
		m_vecElement.erase(iter);
	}
}

void Writer::RemoveElement(unsigned int nIndex, bool freeMemory)
{
	if (nIndex >= GetElementCount())
		return;

	std::vector<Element*>::iterator iter = m_vecElement.begin() + nIndex;
	if (freeMemory) delete *iter;
	m_vecElement.erase(iter);
}

void Writer::RemoveElement(Element* pElement, bool freeMemory)
{
	if (pElement == 0) return;

	// Data가 정렬되지 않았으므로 사용할 수 없다.
	/*std::vector<Element*>::iterator iter = std::find(m_vecElement.begin(), m_vecElement.end(), pData);
	if (iter == m_vecElement.end())
		return;
	
	delete *iter;
	m_vecElement.erase(iter);*/

	unsigned int nElementCount = GetElementCount();

	for (unsigned int i=0;i<nElementCount;i++)
	{
		if (m_vecElement[i] == pElement)
		{
			if (freeMemory) delete pElement;
			m_vecElement.erase(m_vecElement.begin() + i);
			return;
		}
	}
}

void Writer::RemoveFirstElement(bool freeMemory)
{
	if (GetElementCount() == 0) return;

	std::vector<Element*>::iterator iter = m_vecElement.begin();
	if (freeMemory) delete *iter;
	m_vecElement.erase(iter);
}

void Writer::RemoveLastElement(bool freeMemory)
{
	unsigned int nElementCount = GetElementCount();
	if (nElementCount == 0) return;

	if (freeMemory) delete m_vecElement[nElementCount - 1];
	m_vecElement.pop_back();
}

void Writer::RemoveAll(bool freeMemory)
{
	unsigned int nElementCount = GetElementCount();

	for (unsigned int i=0;i<nElementCount;i++)
	{
		if (freeMemory) delete m_vecElement[i];
	}

	m_vecElement.clear();
}

bool Writer::WriteFileA(const char* strFilePath)
{
	if (strFilePath == 0) return false;

	FILE* fp;
	errno_t err = fopen_s(&fp, strFilePath, "wb");
	if (err != 0) return false;

	return WriteFile(fp);
}

bool Writer::WriteFile(const wchar_t* strFilePath)
{
	if (strFilePath == 0) return false;

	FILE* fp;
	errno_t err = _wfopen_s(&fp, strFilePath, L"wb");
	if (err != 0) return false;

	return WriteFile(fp);
}

bool Writer::WriteFile(FILE* fp)
{
	if (fp == 0) return false;

	/*bool isArray;
	size_t nDataByteSize;*/
	unsigned int nElementCount = GetElementCount();

	for (unsigned int i=0;i<nElementCount;i++)
	{
		Element* pElement = m_vecElement[i];
		if (pElement == 0) continue;

		if (!WriteElement(fp, pElement))
		{
			fclose(fp);
			return false;
		}
		//int nElementTag = pElement->GetTag();
		//fwrite(&nElementTag, SIZE_LONG, 1, fp);

		//unsigned int nDataCount = pElement->GetDataCount();
		//fwrite(&nDataCount, SIZE_LONG, 1, fp);

		//for (unsigned int j=0;j<nDataCount;j++)
		//{
		//	const UData* pData = pElement->GetData(j);
		//	if (pData == 0) continue;

		//	if (pData->GetClassType() == ELEMENT)
		//	{
		//		WriteElement(fp, (const Element*)pData);
		//	}
		//	else
		//	{
		//		WriteSegment(fp, (const Segment*)pData);
		//	}
		//	/*const Segment* pSegment = pData->GetSegment(j);
		//	if (pSegment == 0) continue;

		//	unsigned char typeTag = pSegment->GetTypeTag(isArray, nDataByteSize);
		//	fwrite(&typeTag, SIZE_BYTE, 1, fp);

		//	unsigned int nDataCount = pSegment->GetDataCount();

		//	if (isArray)
		//	{
		//		if (nDataCount == 0)
		//		{
		//			SetErrorMessage(m_strErrorMessage, L"%d번째 Tag %d에서 %d번째 Data의 배열 개수가 0입니다.", i, nDataTag, j);
		//			fclose(fp);
		//			return false;
		//		}

		//		fwrite(&nDataCount, SIZE_LONG, 1, fp);

		//		for (unsigned int k=0;k<nDataCount;k++)
		//		{
		//			fwrite(pSegment->GetData(k), nDataByteSize, 1, fp);
		//		}
		//	}
		//	else
		//	{
		//		if (nDataCount == 0)
		//		{
		//			SetErrorMessage(m_strErrorMessage, L"%d번째 Tag %d에서 %d번째 Data가 존재하지 않습니다.", i, nDataTag, j);
		//			fclose(fp);
		//			return false;
		//		}

		//		fwrite(pSegment->GetData(0), nDataByteSize, 1, fp);
		//	}*/
		//}
	}

	fclose(fp);
	return true;
}

bool Writer::WriteElement(FILE* fp, const Element* pElement)
{
	int nElementTag = pElement->GetTag();
	fwrite(&nElementTag, SIZE_LONG, 1, fp);

	unsigned int nDataCount = pElement->GetDataCount();
	fwrite(&nDataCount, SIZE_LONG, 1, fp);

	for (unsigned int j=0;j<nDataCount;j++)
	{
		const UData* pData = pElement->GetData(j);
		if (pData == 0) continue;

		if (pData->GetClassType() == UData::ELEMENT)
		{
			// 제일 상위 Element는 Type Tag를 붙일 필요가 없지만, 하위 Element는 다른 Segment와의 구별을 위하여 Tag를 붙인다.
			fwrite(&ELEMENT_TYPE_TAG, SIZE_BYTE, 1, fp);
			if (!WriteElement(fp, (const Element*)pData))
				return false;
		}
		else
		{
			if (!WriteSegment(fp, (const Segment*)pData, nElementTag))
				return false;
		}
	}

	return true;
}

bool Writer::WriteSegment(FILE* fp, const Segment* pSegment, int nElementTag)
{
	bool isArray;
	size_t nDataByteSize;

	unsigned char typeTag = pSegment->GetTypeTag(isArray, nDataByteSize);
	fwrite(&typeTag, SIZE_BYTE, 1, fp);

	unsigned int nDataCount = pSegment->GetDataCount();

	if (isArray)
	{
		// 빈문자열의 경우 배열의 개수가 0일 수 있음
		/*if (nDataCount == 0)
		{
			SetErrorMessage(m_strErrorMessage, L"Tag [%d]에서 배열 개수가 0인 데이터가 존재합니다.", nElementTag);
			fclose(fp);
			return false;
		}*/

		fwrite(&nDataCount, SIZE_LONG, 1, fp);

		for (unsigned int k=0;k<nDataCount;k++)
		{
			fwrite(pSegment->GetData(k), nDataByteSize, 1, fp);
		}
	}
	else
	{
		if (nDataCount == 0)
		{
			SetErrorMessage(m_strErrorMessage, L"Tag [%d]에서 Data 개수가 0인 데이터가 존재합니다.", nElementTag);
			fclose(fp);
			return false;
		}

		fwrite(pSegment->GetData(0), nDataByteSize, 1, fp);
	}

	return true;
}

const std::wstring& Writer::GetErrorMessage() const
{
	return m_strErrorMessage;
}

bool Writer::ToXMLA(const char* strFilePath)
{
	XMLWriter xml;
	bool isSuccess = xml.WriteFileA(m_vecElement, strFilePath);

	if (!isSuccess)
		m_strErrorMessage = xml.GetErrorMessage();

	return isSuccess;
}

bool Writer::ToXML(const wchar_t* strFilePath)
{
	XMLWriter xml;
	bool isSuccess = xml.WriteFile(m_vecElement, strFilePath);

	if (!isSuccess)
		m_strErrorMessage = xml.GetErrorMessage();

	return isSuccess;
}


bool Writer::ToPrettyXMLA(const char* strFilePath)
{
	UBMLWriter xml;
	bool isSuccess = xml.WriteFileA(m_vecElement, strFilePath);

	if (!isSuccess)
		m_strErrorMessage = xml.GetErrorMessage();

	return isSuccess;
}

bool Writer::ToPrettyXML(const wchar_t* strFilePath)
{
	UBMLWriter xml;
	bool isSuccess = xml.WriteFile(m_vecElement, strFilePath);

	if (!isSuccess)
		m_strErrorMessage = xml.GetErrorMessage();

	return isSuccess;
}

END_NS
END_NS
