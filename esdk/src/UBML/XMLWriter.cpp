#include "StdAfx.h"
#include <BaseTsd.h>
// POCO XML DOM
#include <Poco/DOM/Document.h>
#include <Poco/DOM/Element.h>
#include <Poco/DOM/Text.h>
#include <Poco/DOM/AutoPtr.h>
#include <Poco/DOM/DOMWriter.h>
#include <Poco/XML/XMLWriter.h>
#include <fstream>

#include "StrUtil.h"
#include "XMLWriter.h"
#include "UData.h"

// USING POCO XML NAMESPACE
using Poco::XML::Element;
using Poco::XML::Document;
using Poco::XML::Text;
using Poco::XML::AutoPtr;
using Poco::XML::DOMWriter;
using Poco::XML::XMLWriter;
using namespace std;

BEGIN_NS(UnE)
BEGIN_NS(UBML)

XMLWriter::XMLWriter(void)
{
	m_pDoc = new Document();
}


XMLWriter::~XMLWriter(void)
{
	// smart pointer
	m_pDoc = NULL;
}
bool XMLWriter::WriteFileA(const std::vector<Element*>& rVecData, const char* strFilePath)
{
	if (strFilePath == 0 || m_pDoc == NULL)
		return false;

	if(WriteFile(rVecData, m_pDoc) == false)
		return false;

	DOMWriter writer;

	writer.setNewLine("\n");
	writer.setOptions(Poco::XML::XMLWriter::PRETTY_PRINT|Poco::XML::XMLWriter::WRITE_XML_DECLARATION);
	writer.setIndent("\t");
	std::fstream file;
	file.open(strFilePath, ios_base::out);
	writer.writeNode(file, m_pDoc);
	file.close();
	return true;
}

bool XMLWriter::WriteFile(const std::vector<Element*>& rVecData, const wchar_t* strFilePath)
{
	if (strFilePath == 0 || m_pDoc == NULL)
		return false;

	if(WriteFile(rVecData, m_pDoc) == false)
		return false;

	DOMWriter writer;
	writer.setNewLine("\n");
	writer.setIndent("\t");
	writer.setOptions(Poco::XML::XMLWriter::PRETTY_PRINT|Poco::XML::XMLWriter::WRITE_XML_DECLARATION);


	std::fstream file;
	file.open(strFilePath, ios_base::out);
	writer.writeNode(file, m_pDoc);
	file.close();
	return true;
}


void XMLWriter::IntToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, *(int*)pData);
}

void XMLWriter::UIntToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, *(unsigned int*)pData);
}

void XMLWriter::ShortToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, *(short*)pData);
}

void XMLWriter::UShortToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, *(unsigned short*)pData);
}

void XMLWriter::ByteToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, (int)*(char*)pData);
}

void XMLWriter::UByteToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, (int)*(unsigned char*)pData);
}

void XMLWriter::LONGLONGToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, *(__int64*)pData);
}

void XMLWriter::ULONGLONGToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, *(unsigned int*)pData);
}

void XMLWriter::FloatToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, *(float*)pData);
}

void XMLWriter::DoubleToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, *(double*)pData);
}

void XMLWriter::BoolToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat)
{
	swprintf(str, nBufSize, strFormat, *(bool*)pData);
}

// isString : 0이면 문자열이 아님. 1이면 char*, 2이면 wchar_t*
ToStringFunc XMLWriter::ToStringFunction(int nDataType, wchar_t* strFormat, int& isString)
{
	isString = 0;
	int nType = (nDataType % 100) % 30;

	switch (nType)
	{
	case 1:
		wcscpy_s(strFormat, 8, L"%d");
		return ByteToString;

	case 2:
		wcscpy_s(strFormat, 8, L"%d");
		return UByteToString;

	case 3:
		wcscpy_s(strFormat, 8, L"%d");
		return ShortToString;

	case 4:
		wcscpy_s(strFormat, 8, L"%d");
		return UShortToString;

	case 5:
		wcscpy_s(strFormat, 8, L"%d");
		return IntToString;

	case 6:
		wcscpy_s(strFormat, 8, L"%d");
		return UIntToString;

	case 7:
		wcscpy_s(strFormat, 8, L"%I64d");
		return LONGLONGToString;

	case 8:
		wcscpy_s(strFormat, 8, L"%I64d");
		return ULONGLONGToString;

	case 9:
		wcscpy_s(strFormat, 8, L"%f");
		return FloatToString;

	case 11:
		wcscpy_s(strFormat, 8, L"%lf");
		return DoubleToString;

	case 13:
		wcscpy_s(strFormat, 8, L"%d");
		return BoolToString;

	case 15:
		isString = 1;
		return 0;

	case 17:
		isString = 2;
		return 0;
	}

	return 0;
}

//bool XMLWriter::WriteFile(const std::vector<Element*>& rVecElement, EasyXML2& rXML, const wchar_t* strRootElementName)
//{
//	unsigned int nElementCount = (unsigned int)rVecElement.size();
//	if (nElementCount == 0) return false;
//
//	DWORD_PTR nRootID = rXML.GetRootNode();
//	if (strRootElementName) rXML.SetNodeData(nRootID, strRootElementName);
//	else rXML.SetNodeData(nRootID, L"UBML");
//
//	wchar_t str[256];
//	
//	for (unsigned int i=0;i<nElementCount;i++)
//	{
//		Element* pElement = rVecElement[i];
//		if (pElement == 0) continue;
//
//		DWORD_PTR nNodeID = rXML.MakeElement(nRootID, pElement->GetTagString().c_str());
//		if (nNodeID == 0) continue;
//
//		unsigned int nDataCount = pElement->GetDataCount();
//
//		swprintf(str, 256, L"%d", nDataCount);
//		rXML.MakeAttribute(nNodeID, L"dataCount", str);
//
//		const std::wstring& strDesc = pElement->GetDescription();
//		
//		if (strDesc.length() > 0)
//			rXML.MakeAttribute(nNodeID, L"desc", strDesc.c_str());
//
//		for (unsigned int j=0;j<nDataCount;j++)
//		{
//			const UData* pData = pElement->GetData(j);
//			if (pData == 0) continue;
//
//			if (pData->GetClassType() == UData::ELEMENT)
//			{
//				if (!WriteElement(rXML, nNodeID, (const Element*)pData))
//					return false;
//			}
//			else
//			{
//				if (!WriteSegment(rXML, nNodeID, (const Segment*)pData))
//					return false;
//			}
//			/*const Segment* pSegment = pData->GetSegment(j);
//			if (pSegment == 0) continue;
//
//			unsigned int nSegDataCount = pSegment->GetDataCount();
//			if (nSegDataCount == 0) continue;
//
//			DataType type = pSegment->GetType();
//			ToStringFunc toString = ToStringFunction(type, strFormat, isString);
//			if (toString == 0 && isString == 0) continue;
//
//			DWORD_PTR nDataID = rXML.MakeElement(nNodeID, L"Data");
//			if (nDataID == 0) continue;
//
//			std::wstring strTag = pSegment->GetTypeTagString(isArray);
//			rXML.MakeAttribute(nDataID, L"type", strTag.c_str());
//
//			if (isArray)
//			{
//				swprintf(str, L"%d", nSegDataCount);
//				rXML.MakeAttribute(nDataID, L"array", str);
//
//				std::wstring strSegmentData = L"";
//
//				if (toString)
//				{
//					toString(str, 256, pSegment->GetData(0), strFormat);
//					strSegmentData = str;
//
//					for (int k=1;k<nSegDataCount;k++)
//					{
//						strSegmentData.append(L" ");
//						toString(str, 256, pSegment->GetData(k), strFormat);
//						strSegmentData.append(str);
//					}
//				}
//				else if (isString == 1)	// char*
//				{
//					char ansi[2];
//					int l = 0;
//
//					for (int k=0;k<nSegDataCount;k++, l++)
//					{
//						ansi[0] = *(char*)pSegment->GetData(k);
//
//						if (IsDBCSLeadByte(ansi[0]))	// 2Byte 문자
//						{
//							ansi[1] = *(char*)pSegment->GetData(++k);
//							MultiByteToWideChar(CP_ACP, 0, ansi, 2, &str[l], 256); 
//						}
//						else							// 1Byte 문자
//						{
//							MultiByteToWideChar(CP_ACP, 0, ansi, 1, &str[l], 256); 
//						}
//					}
//
//					strSegmentData.append(str, l);
//				}
//				else if (isString == 2)	// wchar_t*
//				{
//					for (int k=0;k<nSegDataCount;k++)
//					{
//						str[k] = *(wchar_t*)pSegment->GetData(k);
//					}
//
//					strSegmentData.append(str, nSegDataCount);
//				}
//
//				rXML.MakeText(nDataID, strSegmentData.c_str());
//			}
//			else
//			{
//				toString(str, 256, pSegment->GetData(0), strFormat);
//				rXML.MakeText(nDataID, str);
//			}*/
//		}
//	}
//
//	bool isSuccess = rXML.Save();
//
//	if (!isSuccess)
//	{
//		wchar_t strError[256];
//		rXML.GetErrorMessage(strError);
//		m_strErrorMessage = strError;
//	}
//
//	return isSuccess;
//}
//
//bool XMLWriter::WriteElement(EasyXML2& rXML, DWORD_PTR nNodeID, const Element* pElement)
//{
//	nNodeID = rXML.MakeElement(nNodeID, pElement->GetTagString().c_str());
//	if (nNodeID == 0) false;
//
//	unsigned int nDataCount = pElement->GetDataCount();
//
//	wchar_t str[256];
//
//	swprintf(str, 256, L"%d", nDataCount);
//	rXML.MakeAttribute(nNodeID, L"dataCount", str);
//
//	const std::wstring& strDesc = pElement->GetDescription();
//	if (strDesc.length() > 0) rXML.MakeAttribute(nNodeID, L"desc", strDesc.c_str());
//
//	for (unsigned int j=0;j<nDataCount;j++)
//	{
//		const UData* pData = pElement->GetData(j);
//		if (pData == 0) continue;
//
//		if (pData->GetClassType() == UData::ELEMENT)
//		{
//			if (!WriteElement(rXML, nNodeID, (const Element*)pData))
//				return false;
//		}
//		else
//		{
//			if (!WriteSegment(rXML, nNodeID, (const Segment*)pData))
//				return false;
//		}
//	}
//
//	return true;
//}
//
//bool XMLWriter::WriteSegment(EasyXML2& rXML, DWORD_PTR nNodeID, const Segment* pSegment)
//{
//	unsigned int nSegDataCount = pSegment->GetDataCount();
//	if (nSegDataCount == 0) false;
//
//	bool isArray;
//	int isString;
//	wchar_t str[256], strFormat[8];
//
//	DataType type = pSegment->GetType();
//	ToStringFunc toString = ToStringFunction(type, strFormat, isString);
//	if (toString == 0 && isString == 0) false;
//
//	DWORD_PTR nDataID = rXML.MakeElement(nNodeID, L"Data");
//	if (nDataID == 0) false;
//
//	std::wstring strTag = pSegment->GetTypeTagString(isArray);
//	rXML.MakeAttribute(nDataID, L"type", strTag.c_str());
//
//	if (isArray)
//	{
//		swprintf(str, 256, L"%d", nSegDataCount);
//		rXML.MakeAttribute(nDataID, L"array", str);
//
//		std::wstring strSegmentData = L"";
//
//		if (toString)
//		{
//			toString(str, 256, pSegment->GetData(0), strFormat);
//			strSegmentData = str;
//
//			for (unsigned int k=1;k<nSegDataCount;k++)
//			{
//				strSegmentData.append(L" ");
//				toString(str, 256, pSegment->GetData(k), strFormat);
//				strSegmentData.append(str);
//			}
//		}
//		else if (isString == 1)	// char*
//		{
//			char ansi[2];
//			int l = 0;
//
//			for (unsigned int k=0;k<nSegDataCount;k++, l++)
//			{
//				ansi[0] = *(char*)pSegment->GetData(k);
//
//				if (IsDBCSLeadByte(ansi[0]))	// 2Byte 문자
//				{
//					ansi[1] = *(char*)pSegment->GetData(++k);
//					MultiByteToWideChar(CP_ACP, 0, ansi, 2, &str[l], 256); 
//				}
//				else							// 1Byte 문자
//				{
//					MultiByteToWideChar(CP_ACP, 0, ansi, 1, &str[l], 256); 
//				}
//			}
//
//			strSegmentData.append(str, l);
//		}
//		else if (isString == 2)	// wchar_t*
//		{
//			for (unsigned int k=0;k<nSegDataCount;k++)
//			{
//				str[k] = *(wchar_t*)pSegment->GetData(k);
//			}
//
//			strSegmentData.append(str, nSegDataCount);
//		}
//
//		rXML.MakeText(nDataID, strSegmentData.c_str());
//	}
//	else
//	{
//		toString(str, 256, pSegment->GetData(0), strFormat);
//		rXML.MakeText(nDataID, str);
//	}
//
//	return true;
//}

bool XMLWriter::WriteFile(const std::vector<Element*>& rVecElement, Document * pDoc, const wchar_t* strRootElementName)
{
	unsigned int nElementCount = (unsigned int)rVecElement.size();
	if (nElementCount == 0)
		return true;

	char buf[512];	
	char str[256];

	if (strRootElementName)
	{
		UnicodeToUTF8(buf, strRootElementName);
	}
	else
	{
		strncpy(buf, "UBML", 5);
	}
	Poco::XML::Element * pRoot = pDoc->createElement(std::string(buf));;
	pDoc->appendChild(pRoot);	

	for (unsigned int i=0;i < nElementCount;i++)
	{
		Element* pElement = rVecElement[i];
		if (pElement == 0)
			continue;

		UnicodeToUTF8(buf, pElement->GetTagString().c_str());
		Poco::XML::Element * pEleNode = pDoc->createElement(std::string(buf));
		if (pEleNode == 0)
			continue;

		pRoot->appendChild(pEleNode);

		unsigned int nDataCount = pElement->GetDataCount();
		sprintf(str, "%d", nDataCount);

		pEleNode->setAttribute("dataCount", str);

		const std::wstring& strDesc = pElement->GetDescription();

		if (strDesc.length() > 0)
		{
			UnicodeToUTF8(buf, strDesc.c_str());
			pEleNode->setAttribute("desc", std::string(buf));	
		}

		for (unsigned int j=0;j<nDataCount;j++)
		{
			const UData* pData = pElement->GetData(j);
			if (pData == 0)
				continue;

			if (pData->GetClassType() == UData::ELEMENT)
			{
				if (!WriteElement(pDoc, pEleNode, (const Element*)pData))
					return false;
			}
			else
			{
				if (!WriteSegment(pDoc, pEleNode, (const Segment*)pData))
					return false;
			}	
		}
	}

	return true;
}



bool XMLWriter::WriteElement(Poco::XML::Document * pDoc, Poco::XML::Element *pNode, const Element* pElement)
{
	char buf[512];	
	char str[512];

	// CREATE ELEMENT
	UnicodeToUTF8(buf, pElement->GetTagString().c_str());
	Poco::XML::Element * pEleNode = pDoc->createElement(std::string(buf));
	if(pEleNode == 0)
		return false;

	// ADD DATACOUNT ATTR
	unsigned int nDataCount = pElement->GetDataCount();
	sprintf(str, "%d", nDataCount);
	pEleNode->setAttribute("dataCount", str);

	// ADD DESCRIPTION ATTR
	const std::wstring& strDesc = pElement->GetDescription();
	if (strDesc.length() > 0)
	{
		UnicodeToUTF8(buf, strDesc.c_str());
		pEleNode->setAttribute("desc", std::string(buf));	
	}
	// ADD ELEMENT AT CHILD
	pNode->appendChild(pEleNode);

	for (unsigned int j=0;j<nDataCount;j++)
	{
		const UData* pData = pElement->GetData(j);
		if (pData == 0)
			continue;

		if (pData->GetClassType() == UData::ELEMENT)
		{
			if (!WriteElement(pDoc, pEleNode, (const Element*)pData))
				return false;
		}
		else
		{
			if (!WriteSegment(pDoc, pEleNode, (const Segment*)pData))
				return false;
		}
	}

	return true;
}


bool XMLWriter::WriteSegment(Poco::XML::Document * pDoc, Poco::XML::Element *pNode, const Segment* pSegment)
{
	char buf[512];
	unsigned int nSegDataCount = pSegment->GetDataCount();
	if (nSegDataCount == 0)
		return true;

	bool isArray;
	int isString;
	wchar_t str[256], strFormat[8];

	DataType type = pSegment->GetType();
	ToStringFunc toString = ToStringFunction(type, strFormat, isString);
	if(toString == 0 && isString == 0)
		return false;

	// CREATE ELEMENT
	Poco::XML::Element * pEleNode = pDoc->createElement(std::string("Data"));
	if(pEleNode == 0)
		return false;

	// ADD DESCRIPTION ATTR
	const std::wstring& strTag = pSegment->GetTypeTagString(isArray);
	UnicodeToUTF8(buf, strTag.c_str());
	pEleNode->setAttribute("type", std::string(buf));	


	if (isArray)
	{
		sprintf(buf,  "%d", nSegDataCount);
		pEleNode->setAttribute("array", std::string(buf));	

		std::wstring strSegmentData = L"";

		if (toString)
		{
			toString(str, 256, pSegment->GetData(0), strFormat);
			strSegmentData = str;

			for (unsigned int k=1;k<nSegDataCount;k++)
			{
				strSegmentData.append(L" ");
				toString(str, 256, pSegment->GetData(k), strFormat);
				strSegmentData.append(str);
			}
		}
		else if (isString == 1)	// char*
		{
			char ansi[2];
			int l = 0;

			for (unsigned int k=0;k<nSegDataCount;k++, l++)
			{
				ansi[0] = *(char*)pSegment->GetData(k);

				if (IsDBCSLeadByte(ansi[0]))	// 2Byte 문자
				{
					ansi[1] = *(char*)pSegment->GetData(++k);
					MultiByteToWideChar(CP_ACP, 0, ansi, 2, &str[l], 256); 
				}
				else							// 1Byte 문자
				{
					MultiByteToWideChar(CP_ACP, 0, ansi, 1, &str[l], 256); 
				}
			}

			strSegmentData.append(str, l);
		}
		else if (isString == 2)	// wchar_t*
		{
			for (unsigned int k=0;k<nSegDataCount;k++)
			{
				str[k] = *(wchar_t*)pSegment->GetData(k);
			}

			strSegmentData.append(str, nSegDataCount);
		}

		UnicodeToUTF8(buf, strSegmentData.c_str());
		Poco::XML::Text* pText = pDoc->createTextNode(std::string(buf));
		pEleNode->appendChild(pText);
	}
	else
	{
		toString(str, 256, pSegment->GetData(0), strFormat);

		UnicodeToUTF8(buf, str);
		Poco::XML::Text* pText = pDoc->createTextNode(std::string(buf));
		pEleNode->appendChild(pText);

	}

	// ADD ELEMENT AT CHILD
	pNode->appendChild(pEleNode);

	return true;
}

const std::wstring& XMLWriter::GetErrorMessage() const
{
	return m_strErrorMessage;
}

END_NS
END_NS
