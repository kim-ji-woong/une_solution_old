#include "StdAfx.h"
#include "Reader.h"
#include "UData.h"
#include "XMLWriter.h"

BEGIN_NS(UnE)
BEGIN_NS(UBML)

void SetErrorMessage(std::wstring& rString, wchar_t* strFormat, ...);

Reader::Reader(void)
{
}

Reader::~Reader(void)
{
	RemoveAll(true);
}

Reader::Reader(const Reader&)
{
}

void Reader::operator= (const Reader&)
{
}

void Reader::RemoveAll(bool freeMemory)
{
	if (freeMemory)
	{
		size_t elementCount = m_vecElement.size();

		for (size_t i=0;i<elementCount;i++)
		{
			if (freeMemory) delete m_vecElement[i];
		}
	}

	m_vecElement.clear();
}

unsigned int Reader::GetElementCount() const
{
	return (unsigned int)m_vecElement.size();
}

const Element* Reader::GetElement(unsigned int nIndex) const
{
	return m_vecElement[nIndex];
}

bool Reader::ReadFileA(const char* strFilePath)
{
	if (strFilePath == 0) return false;

	FILE* fp;
	errno_t err = fopen_s(&fp, strFilePath, "rb");
	if (err != 0) return false;

	return ReadFile(fp);
}

bool Reader::ReadFile(const wchar_t* strFilePath)
{
	if (strFilePath == 0) return false;

	FILE* fp;
	errno_t err = _wfopen_s(&fp, strFilePath, L"rb");
	if (err != 0) return false;

	return ReadFile(fp);
}

bool Reader::ReadFile(FILE* fp)
{
	bool isSuccess = true;
	int nTag = 0;
	//unsigned int nDataCount = 0;

	while (ReadTag(fp, nTag, isSuccess))
	{
		/*Element* pElement = new Element(nTag);
		
		for (unsigned int i=0;i<nDataCount;i++)
		{
			Segment seg;
			
			if (!ReadSegment(fp, seg))
			{
				fclose(fp);
				return false;
			}

			pElement->AddSegment(seg);
		}*/
		Element* pElement = ReadElement(fp, &nTag);
		if (pElement == 0)
			return false;

		m_vecElement.push_back(pElement);
	}

	fclose(fp);
	return isSuccess;
}

bool Reader::ReadTag(FILE* fp, int& rTag/*, unsigned int& rDataCount*/, bool& isSuccess)
{
	/*if (feof(fp))
		return false;*/

	long nCurrent = ftell(fp);
	fseek(fp, 0, SEEK_END);
	long nFileSize = ftell(fp);
	fseek(fp, nCurrent, SEEK_SET);

	if (nCurrent >= nFileSize)
		return false;

	size_t readCount = fread(&rTag, SIZE_LONG, 1, fp);

	if (readCount != 1)
	{
		SetErrorMessage(m_strErrorMessage, L"Tag 위치가 잘못되었거나 파일에 오류가 있습니다.");
		//fclose(fp);
		isSuccess = false;
		return false;
	}

	//readCount = fread(&rDataCount, SIZE_LONG, 1, fp);

	//if (readCount != 1)
	//{
	//	SetErrorMessage(m_strErrorMessage, L"Tag내에 Data 개수 정보를 찾을 수 없습니다.");
	//	//fclose(fp);
	//	isSuccess = false;
	//	return false;
	//}

	return true;
}

template <class T>
static bool _ReadSegment(FILE* fp, Segment& rSegment, unsigned int nArrSize, size_t dataByteSize, std::wstring& strErrorMessage)
{
	unsigned char data[8];

	for (unsigned int i=0;i<nArrSize;i++)
	{
		if (fread(data, dataByteSize, 1, fp) != 1)
		{
			SetErrorMessage(strErrorMessage, L"잘못된 Data Block입니다.");
			//fclose(fp);
			return false;
		}

		rSegment.AddData(*(T*)data);
	}

	return true;
}

bool Reader::ReadData(FILE* fp, Element* pParentElement)
{
	unsigned char typeTag;
	size_t readCount = fread(&typeTag, SIZE_BYTE, 1, fp);

	if (readCount != 1)
	{
		SetErrorMessage(m_strErrorMessage, L"Data Type 정보를 얻어올 수 없습니다.");
		//fclose(fp);
		return 0;
	}

	unsigned char dataType = typeTag & 0x3f;
	return dataType == 10 ? ReadElement(fp, typeTag, pParentElement) : ReadSegment(fp, typeTag, pParentElement);
}

bool Reader::ReadElement(FILE* fp, unsigned char typeTag, Element* pParentElement)
{
	bool isArray  = (typeTag & 0x80) ? true : false;
	unsigned int nArrayCount = 1;

	if (isArray)
	{
		if (fread(&nArrayCount, SIZE_LONG, 1, fp) != 1)
			return false;
	}

	for (unsigned int i=0;i<nArrayCount;i++)
	{
		Element* pElement = ReadElement(fp);
		if (pElement == 0) return false;

		pParentElement->AddData(pElement);
	}

	return true;
}

Element* Reader::ReadElement(FILE* fp, int* pElementTag)
{
	int nElementTag;

	if (pElementTag) nElementTag = *pElementTag;
	else
	{
		if (fread(&nElementTag, SIZE_LONG, 1, fp) != 1)
			return 0;
	}

	Element* pElement = new Element(nElementTag);

	unsigned int nDataCount;
	if (fread(&nDataCount, SIZE_LONG, 1, fp) != 1)
	{
		delete pElement;
		return 0;
	}

	unsigned char typeTag;

	for (unsigned int i=0;i<nDataCount;i++)
	{
		if (fread(&typeTag, SIZE_BYTE, 1, fp) != 1)
		{
			delete pElement;
			return 0;
		}

		unsigned char dataType = typeTag & 0x3f;

		if (dataType == 10)
		{
			if (!ReadElement(fp, typeTag, pElement))
			{
				delete pElement;
				return 0;
			}
		}
		else
		{
			if (!ReadSegment(fp, typeTag, pElement))
			{
				delete pElement;
				return 0;
			}
		}
	}

	return pElement;
}

bool Reader::ReadSegment(FILE* fp, unsigned char typeTag, Element* pParentElement)
{
	bool isArray;
	unsigned int nArrSize = 1;
	size_t dataByteSize;

	Segment* pSegment = new Segment;

	if (!pSegment->SetTypeTag(typeTag, isArray, dataByteSize))
	{
		SetErrorMessage(m_strErrorMessage, L"잘못된 Data Type Tag(%d)가 존재합니다.", typeTag);
		delete pSegment;
		//fclose(fp);
		return false;
	}

	if (isArray)
	{
		if (fread(&nArrSize, SIZE_LONG, 1, fp) != 1)
		{
			SetErrorMessage(m_strErrorMessage, L"배열의 크기를 읽을 수 없습니다.");
			delete pSegment;
			//fclose(fp);
			return false;
		}

		// 빈문자열의 경우 배열의 크기가 0일 수 있음
		/*if (nArrSize == 0)
		{
			SetErrorMessage(m_strErrorMessage, L"배열의 크기가 0일 수 없습니다.");
			delete pSegment;
			//fclose(fp);
			return false;
		}*/
	}

	unsigned int nDataType = typeTag & 0x3f;

	switch (nDataType)
	{
	case 1:
		if (!_ReadSegment<unsigned char>(fp, *pSegment, nArrSize, dataByteSize, m_strErrorMessage))
			goto FAILURE;
		break;

	case 2:
		if (!_ReadSegment<short>(fp, *pSegment, nArrSize, dataByteSize, m_strErrorMessage))
			goto FAILURE;
		break;

	case 3:
		if (!_ReadSegment<int>(fp, *pSegment, nArrSize, dataByteSize, m_strErrorMessage))
			goto FAILURE;
		break;

	case 4:
		if (!_ReadSegment<__int64>(fp, *pSegment, nArrSize, dataByteSize, m_strErrorMessage))
			goto FAILURE;
		break;

	case 5:
		if (!_ReadSegment<float>(fp, *pSegment, nArrSize, dataByteSize, m_strErrorMessage))
			goto FAILURE;
		break;

	case 6:
		if (!_ReadSegment<double>(fp, *pSegment, nArrSize, dataByteSize, m_strErrorMessage))
			goto FAILURE;
		break;

	case 7:
		if (!_ReadSegment<bool>(fp, *pSegment, nArrSize, dataByteSize, m_strErrorMessage))
			goto FAILURE;
		break;

	case 8:
		if (!_ReadSegment<char>(fp, *pSegment, nArrSize, dataByteSize, m_strErrorMessage))
			goto FAILURE;
		break;

	case 9:
		if (!_ReadSegment<wchar_t>(fp, *pSegment, nArrSize, dataByteSize, m_strErrorMessage))
			goto FAILURE;
		break;

	default:
		goto FAILURE;
	}

	pParentElement->AddData(pSegment);
	return true;

FAILURE:
	delete pSegment;
	return false;
}

/*bool Reader::ReadSegment(FILE* fp, Segment& rSegment)
{
	unsigned char dataType;
	size_t readCount = fread(&dataType, SIZE_BYTE, 1, fp);

	if (readCount != 1)
	{
		SetErrorMessage(m_strErrorMessage, L"Data Type 정보를 얻어올 수 없습니다.");
		fclose(fp);
		return false;
	}

	bool isArray;
	unsigned int nArrSize = 1;
	size_t dataByteSize;

	if (!rSegment.SetTypeTag(dataType, isArray, dataByteSize))
	{
		SetErrorMessage(m_strErrorMessage, L"잘못된 Data Type Tag(%d)가 존재합니다.", dataType);
		fclose(fp);
		return false;
	}

	if (isArray)
	{
		if (fread(&nArrSize, SIZE_LONG, 1, fp) != 1)
		{
			SetErrorMessage(m_strErrorMessage, L"배열의 크기를 읽을 수 없습니다.");
			fclose(fp);
			return false;
		}

		if (nArrSize == 0)
		{
			SetErrorMessage(m_strErrorMessage, L"배열의 크기가 0일 수 없습니다.");
			fclose(fp);
			return false;
		}
	}

	unsigned int nDataType = dataType & 0x3f;

	switch (nDataType)
	{
	case 1:
		if (!_ReadSegment<unsigned char>(fp, rSegment, nArrSize, dataByteSize, m_strErrorMessage))
			return false;
		break;

	case 2:
		if (!_ReadSegment<short>(fp, rSegment, nArrSize, dataByteSize, m_strErrorMessage))
			return false;
		break;

	case 3:
		if (!_ReadSegment<int>(fp, rSegment, nArrSize, dataByteSize, m_strErrorMessage))
			return false;
		break;

	case 4:
		if (!_ReadSegment<__int64>(fp, rSegment, nArrSize, dataByteSize, m_strErrorMessage))
			return false;
		break;

	case 5:
		if (!_ReadSegment<float>(fp, rSegment, nArrSize, dataByteSize, m_strErrorMessage))
			return false;
		break;

	case 6:
		if (!_ReadSegment<double>(fp, rSegment, nArrSize, dataByteSize, m_strErrorMessage))
			return false;
		break;

	case 7:
		if (!_ReadSegment<bool>(fp, rSegment, nArrSize, dataByteSize, m_strErrorMessage))
			return false;
		break;

	case 8:
		if (!_ReadSegment<char>(fp, rSegment, nArrSize, dataByteSize, m_strErrorMessage))
			return false;
		break;

	case 9:
		if (!_ReadSegment<wchar_t>(fp, rSegment, nArrSize, dataByteSize, m_strErrorMessage))
			return false;
		break;
	}

	return true;
}*/

const std::wstring& Reader::GetErrorMessage() const
{
	return m_strErrorMessage;
}

bool Reader::ToXMLA(const char* strFilePath)
{
	XMLWriter xml;
	bool isSuccess = xml.WriteFileA(m_vecElement, strFilePath);

	if (!isSuccess)
		m_strErrorMessage = xml.GetErrorMessage();

	return isSuccess;
}

bool Reader::ToXML(const wchar_t* strFilePath)
{
	XMLWriter xml;
	bool isSuccess = xml.WriteFile(m_vecElement, strFilePath);

	if (!isSuccess)
		m_strErrorMessage = xml.GetErrorMessage();

	return isSuccess;
}

END_NS
END_NS
