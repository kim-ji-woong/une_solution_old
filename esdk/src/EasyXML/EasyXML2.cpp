#include "StdAfx.h"
//#include "TCHAR.h"
#include "EasyXML2.h"
#include <list>
#include <fstream>

#pragma warning (disable:4311)
#pragma warning (disable:4312)

#define         MASKBITS                0x3F
#define         MASKBYTE                0x80
#define         MASK2BYTES              0xC0
#define         MASK3BYTES              0xE0
#define         MASK4BYTES              0xF0
#define         MASK5BYTES              0xF8
#define         MASK6BYTES              0xFC

#define CAN_NOT_INSERT_DATA		L"데이터를 삽입할 수 없습니다."
#define ATTRIBUTE_TEXT_ERROR	L"Attribute의 경우 속성의 이름과 값이 반드시 포함되어야 하며, 그 구분자로 \'|\'가 있어야만 합니다."
#define CAN_NOT_FIND_NODE		L"해당 노드를 찾을 수 없습니다."
#define ELEMENT_INSERT_ERROR	L"Element는 Element 아래에서만 추가될 수 있습니다."
#define TEXT_INSERT_ERROR		L"Text는 Element 아래에서만 추가될 수 있습니다."
#define ATTRIBUTE_INSERT_ERROR	L"Attribute은 Element 아래에서만 추가될 수 있습니다."
#define CAN_NOT_USE_TYPE		L"이 Type은 사용할 수 없습니다."
#define ROOT_ELEMENT_ONLY_ONE	L"Root Element 앞, 뒤에 데이터를 추가할 수 없습니다."
#define CAN_NOT_REMOVE_ATTR_TEXT	L"Attribute의 데이터는 삭제할 수 없습니다."
#define CAN_NOT_REMVOE_ROOT		L"Root Element는 삭제할 수 없습니다."
#define CAN_NOT_OPEN_FILE		L"File을 열 수 없습니다."

#define ERROR_LENGTH 256

BEGIN_NS(UnE)

wchar_t g_strError[ERROR_LENGTH];

static wchar_t* INDENT_XML = L"<?xml version=\"1.0\"?>\r\n<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">\r\n\t<xsl:output method=\"xml\" encoding=\"utf-8\" indent=\"yes\"/>\r\n\t<xsl:template match=\"*\">\r\n\t\t<xsl:element name=\"{name(.)}\">\r\n\t\t\t<xsl:apply-templates select=\"@*\"/>\r\n\t\t\t<xsl:if test=\"child::* or child::text()\">\r\n\t\t\t\t<xsl:apply-templates/>\r\n\t\t\t</xsl:if>\r\n\t\t</xsl:element>\r\n\t</xsl:template>\r\n\t<xsl:template match=\"@*\">\r\n\t\t<xsl:attribute name=\"{name(.)}\">\r\n\t\t\t<xsl:value-of select=\".\"/>\r\n\t\t</xsl:attribute>\r\n\t</xsl:template>\r\n\t<xsl:template match=\"processing-instruction()\">\r\n\t\t<xsl:copy-of select=\".\"/>\r\n\t</xsl:template>\r\n\t<xsl:template match=\"comment()\">\r\n\t\t<xsl:comment>\r\n\t\t\t<xsl:value-of select=\".\"/>\r\n\t\t</xsl:comment>\r\n\t</xsl:template>\r\n</xsl:stylesheet>\r\n";

TreeData::TreeData()
{
	m_strData = L"";
}

TreeData::TreeData(const TreeData& rhs)
{
	nDataType = rhs.nDataType;
	m_strData = rhs.m_strData;
}

TreeData::TreeData(const wchar_t* str)
{
	m_strData = str;
}

void TreeData::operator =(const TreeData& rhs)
{
	nDataType = rhs.nDataType;
	m_strData = rhs.m_strData;
}

TreeData::~TreeData(void)
{
}

void TreeData::operator =(const wchar_t* str)
{
	if (str != 0) m_strData = str;
}

const wchar_t* TreeData::GetStrData()
{
	return m_strData.data();
}

EasyXML2::EasyXML2(void)
{
	m_strStyleSheet[0] = 0;
	m_bIndent = true;
	wcscpy_s(m_strLocale, 32, L"korean");

	m_writeMode = false;
}

EasyXML2::~EasyXML2(void)
{
	CloseXMLFile();
}

// 읽기 전용 함수
// 파일이 아닌 메모리에서 읽는다.
bool EasyXML2::OpenXMLString(const wchar_t* strXML)
{
	ClearError();

	try
	{
		bool flag = IsOpen();
		if (flag) CloseXMLFile();

		m_pDoc.CreateInstance(L"Msxml2.DOMDocument.3.0");

		if (1)//bRead)
		{
			_variant_t varOut((bool)TRUE);
			varOut = m_pDoc->loadXML(strXML);
			if ((bool)varOut == FALSE)
			{
				//_stprintf_s(m_strError,"%s : 파일이 존재하지 않거나 잘못된 형식입니다.",strPath);
				return false;
			}

			MSXML2::IXMLDOMNodePtr pNod = m_pDoc->documentElement;

			while (pNod)
			{
				if (pNod->nodeType == MSXML2::NODE_ELEMENT)
				{
					ReadElement(pNod,0);
				}
				else if (pNod->nodeType == MSXML2::NODE_COMMENT)
				{
					//ReadComment(pNod);
				}

				pNod = pNod->GetnextSibling();
			}
		}
		else
		{
			MSXML2::IXMLDOMProcessingInstructionPtr pPI = m_pDoc->createProcessingInstruction(L"xml",L"version=\"1.0\" encoding=\"utf-8\"");
			//MSXML2::IXMLDOMProcessingInstructionPtr pPI = m_pDoc->createProcessingInstruction(L"xml",L"version=\"1.0\" encoding=\"euc-kr\"");
			MSXML2::IXMLDOMElementPtr pRootElement = m_pDoc->createElement(L"Temp");

			m_pDoc->appendChild(pPI);

			int nLenStyleSheet = (int)wcslen(m_strStyleSheet);

			if (nLenStyleSheet > 0)
			{
				wchar_t wstr[256];
				//wsprintf(wstr,L"type=\"text/xsl\" href=\"%s\"",m_strStyleSheet);
				wsprintfW(wstr,L"type=\"text/xsl\" href=\"");

				int nIndex = (int)wcslen(wstr);
				for (int i=0;i<nLenStyleSheet;i++,nIndex++)
				{
					wstr[nIndex] = m_strStyleSheet[i];
				}

				wstr[nIndex++] = '\"';
				wstr[nIndex] = 0;

				MSXML2::IXMLDOMProcessingInstructionPtr pPI2 = m_pDoc->createProcessingInstruction(L"xml:stylesheet",wstr);
				m_pDoc->appendChild(pPI2);
			}

			m_pDoc->appendChild(pRootElement);

			TreeData data = L"NoName";
			data.nDataType = ROOT;
			m_tree.Insert(data);
		}

//		_tcscpy_s(m_strPath,strPath);
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, 256, strError);
		return false;
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}
	catch (...)
	{
		wcscpy_s(m_strError, 256, CAN_NOT_OPEN_FILE);
		return false;
	}

	return true;
}

// strPath : 파일 경로
// bRead : 읽기용으로 여는 것인가?
//         이 값이 False일 경우 Root Element가 <Temp>인 XML 파일이 생성됨
bool EasyXML2::OpenXMLFile(const wchar_t* strPath, bool bRead)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	ClearError();

	try
	{
		bool flag = IsOpen();
		if (flag) CloseXMLFile();

		m_pDoc.CreateInstance(L"Msxml2.DOMDocument.3.0");

		if (bRead)
		{
			_variant_t varOut((bool)TRUE);
			varOut = m_pDoc->load(strPath);
			if ((bool)varOut == FALSE)
			{
				swprintf(m_strError, 256, L"%s : 파일이 존재하지 않거나 잘못된 형식입니다.", strPath);
				return false;
			}

			MSXML2::IXMLDOMNodePtr pNod = m_pDoc->documentElement;

			while (pNod)
			{
				if (pNod->nodeType == MSXML2::NODE_ELEMENT)
				{
					ReadElement(pNod,0);
				}
				else if (pNod->nodeType == MSXML2::NODE_COMMENT)
				{
					//ReadComment(pNod);
				}

				pNod = pNod->GetnextSibling();
			}
		}
		else
		{
			MSXML2::IXMLDOMProcessingInstructionPtr pPI = m_pDoc->createProcessingInstruction(L"xml",L"version=\"1.0\" encoding=\"utf-8\"");
			//MSXML2::IXMLDOMProcessingInstructionPtr pPI = m_pDoc->createProcessingInstruction(L"xml",L"version=\"1.0\" encoding=\"euc-kr\"");
			MSXML2::IXMLDOMElementPtr pRootElement = m_pDoc->createElement(L"NoName");

			m_pDoc->appendChild(pPI);

			int nLenStyleSheet = (int)wcslen(m_strStyleSheet);

			if (nLenStyleSheet > 0)
			{
				wchar_t wstr[256];
				//wsprintf(wstr,L"type=\"text/xsl\" href=\"%s\"",m_strStyleSheet);
				wsprintfW(wstr,L"type=\"text/xsl\" href=\"");

				int nIndex = (int)wcslen(wstr);
				for (int i=0;i<nLenStyleSheet;i++,nIndex++)
				{
					wstr[nIndex] = m_strStyleSheet[i];
				}

				wstr[nIndex++] = '\"';
				wstr[nIndex] = 0;

				MSXML2::IXMLDOMProcessingInstructionPtr pPI2 = m_pDoc->createProcessingInstruction(L"xml:stylesheet",wstr);
				m_pDoc->appendChild(pPI2);
			}

			m_pDoc->appendChild(pRootElement);

			TreeData data = L"NoName";
			data.nDataType = ROOT;
			m_tree.Insert(data);

			m_writeMode = true;
		}

		wcscpy_s(m_strPath, _MAX_PATH, strPath);
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, 256, strError);
		return false;
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}
	catch (...)
	{
		wcscpy_s(m_strError, 256, CAN_NOT_OPEN_FILE);
		return false;
	}

	return true;
}

bool EasyXML2::OpenXMLFileA(const char* strPath, bool bRead)
{
	_bstr_t bstrPath(strPath);
	return OpenXMLFile((const wchar_t*)bstrPath, bRead);
}

void EasyXML2::CloseXMLFile(void)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	ClearError();

	try
	{
		bool flag = IsOpen();

		if (flag)
		{
			if (m_writeMode) Save();
			m_pDoc.Release();
			m_pDoc = NULL;
			m_tree.RemoveAll();
		}
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}
}

bool EasyXML2::IsOpen()
{
	// TODO: 여기에 구현 코드를 추가합니다.
	if (m_pDoc)
	{
		return true;
	}

	return false;
}

static HRESULT Transform(MSXML2::IXMLDOMDocumentPtr pXML, BSTR bsOutputXMLFile)
{
	MSXML2::IXMLDOMDocumentPtr pXSL=NULL;
	MSXML2::IXMLDOMDocumentPtr pResult=NULL;
	HRESULT hr=S_OK;

	try
	{
		hr=pXSL.CreateInstance(L"Msxml2.DOMDocument.3.0");

		// 스타일 시트 불러오기
		pXSL->put_async(VARIANT_FALSE);
		//pXSL->load((_variant_t)"indent.xsl");
		pXSL->loadXML(INDENT_XML);
		hr=pResult.CreateInstance(L"Msxml2.DOMDocument.3.0");

		if(FAILED(hr)) 
		{
			return E_FAIL;
		}

		VARIANT vObject;
		VariantInit(&vObject);
		vObject.vt=VT_DISPATCH;
		vObject.pdispVal=pResult;
		hr=pXML->transformNodeToObject(pXSL,vObject); // 변환하기~
		
		if(FAILED(hr))
		{
			return E_FAIL;
		}

		// 변환 완성 저장하기
		pResult->save((_variant_t)bsOutputXMLFile);
	}
	catch(_com_error& e)
	{
		throw e;
	}
	
	return S_OK;
}

// UTF-8 Encoding
//static void WriteXML(std::ofstream& fout, wchar_t* strXML, int nBeginIndex, int nEndIndex, int nDepth)
//{
//	TCHAR buf[3];
//	int i;
//	for (i=0;i<nDepth;i++) fout << "\t";
//
//   for(i=nBeginIndex; i <= nEndIndex; i++)
//   {
//      // 0xxxxxxx
//      if(strXML[i] < 0x80)
//      {
//		  buf[0] = (TCHAR)strXML[i];
//		  fout << buf[0];
//      }
//      // 110xxxxx 10xxxxxx
//      else if(strXML[i] < 0x800)
//      {
//		  buf[0] = (TCHAR)(MASK2BYTES | strXML[i] >> 6);
//		  buf[1] = (TCHAR)(MASKBYTE | strXML[i] & MASKBITS);
//		  fout.write(buf,2);
//      }
//      // 1110xxxx 10xxxxxx 10xxxxxx
//      else if(strXML[i] < 0x10000)
//      {
//		  buf[0] = (TCHAR)(MASK3BYTES | strXML[i] >> 12);
//		  buf[1] = (TCHAR)(MASKBYTE | strXML[i] >> 6 & MASKBITS);
//		  buf[2] = (TCHAR)(MASKBYTE | strXML[i] & MASKBITS);
//		  fout.write(buf,3);
//      }
//   }
//
//   fout << std::endl;
//}

// UTF-8 Encoding
static void WriteXML(FILE* fp, wchar_t* strXML, int nBeginIndex, int nEndIndex, int nDepth)
{
	char buf[3];
	int i;
	for (i=0;i<nDepth;i++) fprintf_s(fp,"\t");

   for(i=nBeginIndex; i <= nEndIndex; i++)
   {
      // 0xxxxxxx
      if(strXML[i] < 0x80)
      {
		  buf[0] = (char)strXML[i];
		  fwrite(buf,sizeof(char),1,fp);
      }
      // 110xxxxx 10xxxxxx
      else if(strXML[i] < 0x800)
      {
		  buf[0] = (char)(MASK2BYTES | strXML[i] >> 6);
		  buf[1] = (char)(MASKBYTE | strXML[i] & MASKBITS);
		  fwrite(buf,sizeof(char),2,fp);
      }
      // 1110xxxx 10xxxxxx 10xxxxxx
      else if(strXML[i] < 0x10000)
      {
		  buf[0] = (char)(MASK3BYTES | strXML[i] >> 12);
		  buf[1] = (char)(MASKBYTE | strXML[i] >> 6 & MASKBITS);
		  buf[2] = (char)(MASKBYTE | strXML[i] & MASKBITS);
		  fwrite(buf,sizeof(char),3,fp);
      }
   }

   fprintf_s(fp,"\r\n");
}

//static void WriteXML(std::wofstream& fout, wchar_t* strXML, int nBeginIndex, int nEndIndex, int nDepth)
//{
//	int i;
//	for (i=0;i<nDepth;i++) fout << L"\t";
//
//	fout.write(&strXML[nBeginIndex],nEndIndex - nBeginIndex + 1);
//	fout << std::endl;
//}

static int GetXMLText(/*std::wstring& wstrText, */wchar_t* wstrXML, int nBeginIndex)
{
	bool bFind = false;
	int nEndIndex = -1;

	for (int i=nBeginIndex;wstrXML[i];i++)
	{
		if (wstrXML[i] == L'<')
		{
			nEndIndex = i - 1;
			break;
		}
		else if (wstrXML[i] != L'\r' && wstrXML[i] != L'\n' && wstrXML[i] != L'\t' && wstrXML[i] != L' ') bFind = true;
	}

	if (!bFind) return -1;

	/*wstrText.clear();
	wstrText.append(&wstrXML[nBeginIndex],nEndIndex - nBeginIndex + 1);*/
	return nEndIndex;
}

//static int WriteHeader(std::wofstream& fout, wchar_t* wstrXML)
//{
//	int nBeginIndex = -1, nEndIndex, nQuestion = -1;
//	bool bFirst = true;
//
//	for (int i=0;wstrXML[i];i++)
//	{
//		if (wstrXML[i] == L'<') nBeginIndex = i;
//		else if (wstrXML[i] == L'>')
//		{
//			nEndIndex = i;
//			if (nQuestion != nEndIndex - 1) return nBeginIndex;
//
//			if (bFirst)
//			{
//				/*unsigned char utf8Header[3] = {0xEF, 0xBB, 0xBF};
//				fout.write((char*)utf8Header,3);*/
//				fout << L"<?xml version=\"1.0\" encoding=\"utf-8\"?>" << std::endl;
//				//fout << "<?xml version=" << strXMLVersion << " encoding=" << strXMLEncoding << "?>" << std::endl;
//				bFirst = false;
//			}
//			else WriteXML(fout,wstrXML,nBeginIndex,nEndIndex,0);
//		}
//		else if (wstrXML[i] == L'?') nQuestion = i;
//	}
//
//	return -1;
//}

static int WriteHeader(FILE* fp, wchar_t* wstrXML)
{
	int nBeginIndex = -1, nEndIndex, nQuestion = -1;
	bool bFirst = true;

	for (int i=0;wstrXML[i];i++)
	{
		if (wstrXML[i] == L'<') nBeginIndex = i;
		else if (wstrXML[i] == L'>')
		{
			nEndIndex = i;
			if (nQuestion != nEndIndex - 1) return nBeginIndex;

			if (bFirst)
			{
				unsigned char utf8Header[3] = {0xEF, 0xBB, 0xBF};
				fwrite((char*)utf8Header,sizeof(char),3,fp);
				fprintf_s(fp,"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
				bFirst = false;
			}
			else WriteXML(fp,wstrXML,nBeginIndex,nEndIndex,0);
		}
		else if (wstrXML[i] == L'?') nQuestion = i;
	}

	return -1;
}

static HRESULT IndentXML(MSXML2::IXMLDOMDocumentPtr pXML, _bstr_t bstrPath)
{
	//std::ofstream fout((char*)bstrPath);
	FILE* fp;
	fopen_s(&fp,(char*)bstrPath,"wb");
	if (fp == 0) return S_FALSE;

	_bstr_t bstrXML = pXML->Getxml();
	wchar_t* wstrXML = bstrXML;
	//char* strXML = bstrXML;

	int nDepth = 0, nBeginIndex = 0, nSegmentIndex, nEndIndex, nSlashIndex = -1;
	//std::wstring wstrText;
	bool bTextMode = false;

	//nBeginIndex = WriteHeader(fout,wstrXML);
	nBeginIndex = WriteHeader(fp,wstrXML);
	if (nBeginIndex < 0)
	{
		fclose(fp);
		return S_FALSE;
	}

	for (int i=nBeginIndex;wstrXML[i];i++)
	{
		if (wstrXML[i] == L'<')
		{
			if (!bTextMode) nBeginIndex = i;
			nSegmentIndex = i;
		}
		else if (wstrXML[i] == L'>')
		{
			nEndIndex = i;

			int nNextIndex = GetXMLText(/*wstrText,*/wstrXML,i+1);

			if (nNextIndex >= 0)
			{
				bTextMode = true;
				i = nNextIndex;
				continue;
			}

			if (nSlashIndex == nSegmentIndex + 1)		// </ ...>
			{
				if (!bTextMode) nDepth--;
				//WriteXML(fout,wstrXML,nBeginIndex,nEndIndex,nDepth);
				WriteXML(fp,wstrXML,nBeginIndex,nEndIndex,nDepth);
			}
			else if (nSlashIndex == nEndIndex -1)	// <.../>
			{
				//WriteXML(fout,wstrXML,nBeginIndex,nEndIndex,nDepth);
				WriteXML(fp,wstrXML,nBeginIndex,nEndIndex,nDepth);
			}
			else	// < ... >
			{
				//WriteXML(fout,wstrXML,nBeginIndex,nEndIndex,nDepth);
				WriteXML(fp,wstrXML,nBeginIndex,nEndIndex,nDepth);
				nDepth++;
			}

			bTextMode = false;
		}
		else if (wstrXML[i] == L'/' && (wstrXML[i-1] == L'<' || wstrXML[i+1] == L'>')) nSlashIndex = i;
	}

	fclose(fp);
	return S_OK;
}

//static HRESULT IndentXML(MSXML2::IXMLDOMDocumentPtr pXML, _bstr_t bstrPath, char* strLocale)
//{
//	std::wofstream fout((wchar_t*)bstrPath);
//	if (!fout.is_open()) return S_FALSE;
//	fout.imbue(std::locale(strLocale));
//	/*FILE* fp;
//	_tfopen_s(&fp,(TCHAR*)bstrPath,_T("wb"));
//	if (fp == 0) return S_FALSE;*/
//
//	_bstr_t bstrXML = pXML->Getxml();
//	wchar_t* wstrXML = bstrXML;
//	//TCHAR* strXML = bstrXML;
//
//	int nDepth = 0, nBeginIndex = 0, nSegmentIndex, nEndIndex, nSlashIndex = -1;
//	//std::wstring wstrText;
//	bool bTextMode = false;
//
//	nBeginIndex = WriteHeader(fout,wstrXML);
//	if (nBeginIndex < 0) return S_FALSE;
//
//	int nLen = wcslen(wstrXML);
//
//	for (int i=nBeginIndex;wstrXML[i];i++)
//	{
//		if (wstrXML[i] == L'<')
//		{
//			if (!bTextMode) nBeginIndex = i;
//			nSegmentIndex = i;
//		}
//		else if (wstrXML[i] == L'>')
//		{
//			nEndIndex = i;
//
//			int nNextIndex = GetXMLText(/*wstrText,*/wstrXML,i+1);
//
//			if (nNextIndex >= 0)
//			{
//				bTextMode = true;
//				i = nNextIndex;
//				continue;
//			}
//
//			if (nSlashIndex == nSegmentIndex + 1)		// </ ...>
//			{
//				if (!bTextMode) nDepth--;
//				WriteXML(fout,wstrXML,nBeginIndex,nEndIndex,nDepth);
//				//WriteXML(fp,wstrXML,nBeginIndex,nEndIndex,nDepth);
//			}
//			else if (nSlashIndex == nEndIndex -1)	// <.../>
//			{
//				WriteXML(fout,wstrXML,nBeginIndex,nEndIndex,nDepth);
//				//WriteXML(fp,wstrXML,nBeginIndex,nEndIndex,nDepth);
//			}
//			else	// < ... >
//			{
//				WriteXML(fout,wstrXML,nBeginIndex,nEndIndex,nDepth);
//				//WriteXML(fp,wstrXML,nBeginIndex,nEndIndex,nDepth);
//				nDepth++;
//			}
//
//			bTextMode = false;
//		}
//		else if (wstrXML[i] == L'/' && (wstrXML[i-1] == L'<' || wstrXML[i+1] == L'>')) nSlashIndex = i;
//	}
//
//	//fclose(fp);
//	fout.close();
//	return S_OK;
//}

size_t EasyXML2::GetXMLString(std::wstring& strXML) const
{
	strXML = (const wchar_t*)m_pDoc->Getxml();
	return strXML.length();
}

size_t EasyXML2::GetXMLStringA(std::string& strXML) const
{
	strXML = (const char*)m_pDoc->Getxml();
	return strXML.length();
}

bool EasyXML2::Save()
{
	// TODO: 여기에 구현 코드를 추가합니다.
	return SaveAs(m_strPath);
}

bool EasyXML2::SaveAs(const wchar_t* strPath)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	ClearError();

	try
	{
		bool flag = IsOpen();

		if (flag)
		{
			//HRESULT hr = m_bIndent ? Transform(m_pDoc,bstr_t(strPath)) : m_pDoc->save(strPath);
			HRESULT hr = m_bIndent ? IndentXML(m_pDoc,bstr_t(strPath)) : m_pDoc->save(strPath);
			if (hr == 0) 
			{
				return true;
			}

			return false;
		}
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, 256, strError);
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}

	return false;
}

bool EasyXML2::SaveAsA(const char* strPath)
{
	_bstr_t bstrPath(strPath);
	return SaveAs((const wchar_t*)bstrPath);
}

DWORD_PTR EasyXML2::GetRootNode()
{
	// TODO: 여기에 구현 코드를 추가합니다.
	return (DWORD_PTR)m_tree.root;
}

DWORD_PTR EasyXML2::GetChildNode(DWORD_PTR nodeID)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	if (nodeID == 0) return 0;

	Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;
	return (DWORD_PTR)pNod->GetChild();
}

bool EasyXML2::GetChildNodeData(DWORD_PTR nodeID, wchar_t* strData)
{
	Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;
	DWORD_PTR child = (DWORD_PTR)pNod->GetChild();
	DataType nType = ATTRIBUTE_TEXT;
	return GetNodeData(child, strData, &nType);
}

bool EasyXML2::GetChildNodeDataA(DWORD_PTR nodeID, char* strData)
{
	Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;
	DWORD_PTR child = (DWORD_PTR)pNod->GetChild();
	DataType nType = ATTRIBUTE_TEXT;
	return GetNodeDataA(child, strData, &nType);
}

DWORD_PTR EasyXML2::GetParentNode(DWORD_PTR nodeID)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	if (nodeID == 0) return 0;

	Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;
	return (DWORD_PTR)pNod->GetParent();
}

DWORD_PTR EasyXML2::GetNextNode(DWORD_PTR nodeID)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	if (nodeID == 0) return 0;

	Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;
	return (DWORD_PTR)pNod->GetNext();
}

DWORD_PTR EasyXML2::GetPrevNode(DWORD_PTR nodeID)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	if (nodeID == 0) return 0;

	Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;
	return (DWORD_PTR)pNod->GetPrev();
}

// 자식 노드에 데이터를 추가합니다.
// Return Value : 추가된 노드의 ID
// nodeID : 삽입할 부모 노드
// nNodeType : 추가될 자식 노드의 타입
// strData : 추가될 자식 노드의 텍스트
// nNodeType이 ATTRIBUTE일 경우 InsertData() 대신 InsertAttributeData()를 사용합니다.
// '|'는 Attribute의 이름과 내용을 구분해 주는 구분자입니다.
DWORD_PTR EasyXML2::InsertData(DWORD_PTR nodeID, LONG nodeType, const wchar_t* strData)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	if (!IsOpen())
		return 0;
	if (nodeID == 0) return 0;

	ClearError();

	try
	{
		Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;
		TreeData data;

		if (nodeType == ELEMENT || nodeType == TEXT)
		{
			data = pNod->GetData();

			if (data.nDataType != ROOT && data.nDataType != ELEMENT)
			{
				if (nodeType == ELEMENT) swprintf(m_strError, 256, ELEMENT_INSERT_ERROR);
				else swprintf(m_strError, 256, TEXT_INSERT_ERROR);
				return 0;
			}

			data.nDataType = nodeType;
			data = strData;

			if (!InsertNodeData(pNod,data))
			{
				return 0;
			}
			else
			{
				return (DWORD_PTR)m_tree.Insert(data,pNod);
			}
		}
		/*else if (nodeType == ATTRIBUTE)
		{
			data = pNod->GetData();

			if (data.nDataType != ROOT && data.nDataType != ELEMENT)
			{
				swprintf(m_strError, 256, ATTRIBUTE_INSERT_ERROR);
				return 0;
			}

			data = strData;
			data.nDataType = nodeType;

			if (!InsertNodeData(pNod,data)) 
			{
				return 0;
			}

			wchar_t strName[256], strValue[256];
			if (!GetAttributeNameAndValue(data.GetStrData(),strName,strValue)) 
			{
				return 0;
			}

			data = strName;
			data.nDataType = ATTRIBUTE;
			pNod = m_tree.Insert(data,pNod);

			if (pNod == 0)
			{
				return 0;
			}
			else
			{
				data = strValue;
				data.nDataType = TEXT;
				if (m_tree.Insert(data,pNod))
				{
					return (DWORD_PTR)pNod;
				}
				else
				{
					return 0;
				}
			}
		}*/
		else
		{
			swprintf(m_strError, 256, CAN_NOT_USE_TYPE);
			return 0;
		}
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, 256, strError);
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}

	return 0;
}

DWORD_PTR EasyXML2::InsertDataA(DWORD_PTR nodeID, LONG nodeType, const char* strData)
{
	_bstr_t bstrData(strData);
	return InsertData(nodeID, nodeType, (const wchar_t*)bstrData);
}

DWORD_PTR EasyXML2::InsertAttributeData(DWORD_PTR nodeID, const wchar_t* strAttrName, const wchar_t* strAttrData)
{
	if (!IsOpen())
		return 0;
	if (nodeID == 0) return 0;

	ClearError();

	try
	{
		Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;
		TreeData data;

		data = pNod->GetData();

		if (data.nDataType != ROOT && data.nDataType != ELEMENT)
		{
			swprintf(m_strError, 256, ATTRIBUTE_INSERT_ERROR);
			return 0;
		}

		data = strAttrName;
		data.nDataType = ATTRIBUTE;

		if (!InsertAttributeNodeData(pNod, data, strAttrData)) 
		{
			return 0;
		}

		/*wchar_t strName[256], strValue[256];
		if (!GetAttributeNameAndValue(data.GetStrData(),strName,strValue)) 
		{
			return 0;
		}*/

		data = strAttrName;
		data.nDataType = ATTRIBUTE;
		pNod = m_tree.Insert(data,pNod);

		if (pNod == 0)
		{
			return 0;
		}
		else
		{
			data = strAttrData;
			data.nDataType = TEXT;
			if (m_tree.Insert(data,pNod))
			{
				return (DWORD_PTR)pNod;
			}
			else
			{
				return 0;
			}
		}
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, 256, strError);
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}

	return 0;
}

DWORD_PTR EasyXML2::InsertAttributeDataA(DWORD_PTR nodeID, const char* strAttrName, const char* strAttrData)
{
	_bstr_t bstrAttrName(strAttrName), bstrAttrData(strAttrData);
	return InsertAttributeData(nodeID, (const wchar_t*)bstrAttrName, (const wchar_t*)bstrAttrData);
}

// 이전 노드에 데이터를 추가합니다.
// Return Value : 추가된 노드의 ID
// nodeID : 추가될 노드 뒤에 놓여진 노드
// nNodeType : 추가될 노드의 타입
// strData : 추가될 노드의 텍스트
// nNodeType이 ATTRIBUTE일 경우, InsertBefore() 대신 InsertBeforeAttribute()를 사용한다.
DWORD_PTR EasyXML2::InsertBefore(DWORD_PTR nodeID, LONG nodeType, const wchar_t* strData)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	bool flag = IsOpen();
	if (flag == false || nodeID == 0) return 0;

	ClearError();

	try
	{
		TreeData data;
		Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;

		if (nodeType == ELEMENT || nodeType == TEXT)
		{
			data = pNod->GetData();

			if (data.nDataType == ROOT)
			{
				swprintf(m_strError, 256, ROOT_ELEMENT_ONLY_ONE);
				return 0;
			}

			if (data.nDataType != ELEMENT && data.nDataType != TEXT)
			{
				swprintf(m_strError, 256, CAN_NOT_INSERT_DATA);
				return 0;
			}

			data = strData;
			data.nDataType = nodeType;

			if (!InsertBeforeNodeData(pNod,data))
			{
				return 0;
			}

			/*data = strData;
			data.nDataType = nodeType;*/
			return (DWORD_PTR)m_tree.InsertBefore(data,pNod);
		}
		/*else if (nodeType == ATTRIBUTE)
		{
			data = pNod->GetData();

			if (data.nDataType != ATTRIBUTE)
			{
				swprintf_s(m_strError,CAN_NOT_INSERT_DATA);
				return 0;
			}

			data = strData;
			data.nDataType = nodeType;

			if (!InsertBeforeNodeData(pNod,data))
			{
				return 0;
			}

			wchar_t strName[256], strValue[256];
			if (!GetAttributeNameAndValue(strData,strName,strValue)) 
			{
				return 0;
			}

			data = strName;
			data.nDataType = ATTRIBUTE;
			pNod = m_tree.InsertBefore(data,pNod);

			if (pNod == 0)
			{
				return 0;
			}

			data = strValue;
			data.nDataType = TEXT;

			if (m_tree.Insert(data,pNod))
			{
				return (DWORD_PTR)pNod;
			}
			else
			{
				return 0;
			}
		}*/
		else
		{
			swprintf(m_strError, 256, L"이 Type은 사용할 수 없습니다.");
			return 0;
		}
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, 256, strError);
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}

	return 0;
}

DWORD_PTR EasyXML2::InsertBeforeA(DWORD_PTR nodeID, LONG nodeType, const char* strData)
{
	_bstr_t bstrData(strData);
	return InsertBefore(nodeID, nodeType, (const wchar_t*)bstrData);
}

DWORD_PTR EasyXML2::InsertBeforeAttribute(DWORD_PTR nodeID, const wchar_t* strAttrName, const wchar_t* strAttrData)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	bool isOpen = IsOpen();
	if (isOpen == false || nodeID == 0) return 0;

	ClearError();

	try
	{
		TreeData data;
		Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;

		data = pNod->GetData();

		if (data.nDataType != ATTRIBUTE)
		{
			swprintf(m_strError, 256, CAN_NOT_INSERT_DATA);
			return 0;
		}

		data = strAttrName;
		data.nDataType = ATTRIBUTE;

		if (!InsertBeforeAttributeNodeData(pNod, data, strAttrData))
		{
			return 0;
		}

		/*wchar_t strName[256], strValue[256];
		if (!GetAttributeNameAndValue(strData,strName,strValue)) 
		{
			return 0;
		}*/

		data = strAttrName;
		data.nDataType = ATTRIBUTE;
		pNod = m_tree.InsertBefore(data,pNod);

		if (pNod == 0)
		{
			return 0;
		}

		data = strAttrData;
		data.nDataType = TEXT;

		if (m_tree.Insert(data,pNod))
		{
			return (DWORD_PTR)pNod;
		}
		else
		{
			return 0;
		}
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, 256, strError);
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}

	return 0;
}

DWORD_PTR EasyXML2::InsertBeforeAttributeA(DWORD_PTR nodeID, const char* strAttrName, const char* strAttrData)
{
	_bstr_t bstrAttrName(strAttrName), bstrAttrData(strAttrData);
	return InsertBeforeAttribute(nodeID, (const wchar_t*)bstrAttrName, (const wchar_t*)bstrAttrData);
}

// 이후 노드에 데이터를 추가합니다.
// Return Value : 추가된 노드의 ID
// nodeID : 추가될 노드 앞에 놓여진 노드
// nNodeType : 추가될 노드의 타입
// strData : 추가될 노드의 텍스트
// nNodeType이 ATTRIBUTE일 경우, InsertAfter() 대신 InsertAfterAttribute()을 사용한다.
DWORD_PTR EasyXML2::InsertAfter(DWORD_PTR nodeID, LONG nodeType, const wchar_t* strData)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	bool flag = IsOpen();
	if (flag == false || nodeID == 0) return 0;

	ClearError();

	try
	{
		TreeData data;
		Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;

		if (nodeType == ELEMENT || nodeType == TEXT)
		{
			data = pNod->GetData();

			if (data.nDataType == ROOT)
			{
				swprintf(m_strError, 256, ROOT_ELEMENT_ONLY_ONE);
				return 0;
			}

			if (data.nDataType != ELEMENT && data.nDataType != TEXT)
			{
				swprintf(m_strError, 256, CAN_NOT_INSERT_DATA);
				return 0;
			}

			data = strData;
			data.nDataType = nodeType;

			if (!InsertAfterNodeData(pNod,data))
			{
				return 0;
			}

			/*data = strData;
			data.nDataType = nodeType;*/
			return (DWORD_PTR)m_tree.InsertAfter(data,pNod);
		}
		/*else if (nodeType == ATTRIBUTE)
		{
			data = pNod->GetData();

			if (data.nDataType != ATTRIBUTE)
			{
				_stprintf_s(m_strError,CAN_NOT_INSERT_DATA);
				return 0;
			}

			data = strData;
			data.nDataType = nodeType;

			if (!InsertAfterNodeData(pNod,data))
			{
				return 0;
			}

			wchar_t strName[256], strValue[256];
			if (!GetAttributeNameAndValue(strData,strName,strValue)) 
			{
				return 0;
			}

			data = strName;
			data.nDataType = ATTRIBUTE;
			pNod = m_tree.InsertAfter(data,pNod);

			if (pNod == 0)
			{
				return 0;
			}

			data = strValue;
			data.nDataType = TEXT;

			if (m_tree.Insert(data,pNod))
			{
				return (DWORD_PTR)pNod;
			}
			else
			{
				return 0;
			}
		}*/
		else
		{
			swprintf(m_strError, 256, L"이 Type은 사용할 수 없습니다.");
			return 0;
		}
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, 256, strError);
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}

	return 0;
}

DWORD_PTR EasyXML2::InsertAfterA(DWORD_PTR nodeID, LONG nodeType, const char* strData)
{
	_bstr_t bstrData(strData);
	return InsertAfter(nodeID, nodeType, (const wchar_t*)bstrData);
}

DWORD_PTR EasyXML2::InsertAfterAttribute(DWORD_PTR nodeID, const wchar_t* strAttrName, const wchar_t* strAttrData)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	bool flag = IsOpen();
	if (flag == false || nodeID == 0) return 0;

	ClearError();

	try
	{
		TreeData data;
		Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;

		data = pNod->GetData();

		if (data.nDataType != ATTRIBUTE)
		{
			swprintf(m_strError, 256, CAN_NOT_INSERT_DATA);
			return 0;
		}

		data = strAttrName;
		data.nDataType = ATTRIBUTE;

		if (!InsertAfterAttributeNodeData(pNod, data, strAttrData))
		{
			return 0;
		}

		/*wchar_t strName[256], strValue[256];
		if (!GetAttributeNameAndValue(strData,strName,strValue)) 
		{
			return 0;
		}*/

		data = strAttrName;
		data.nDataType = ATTRIBUTE;
		pNod = m_tree.InsertAfter(data,pNod);

		if (pNod == 0)
		{
			return 0;
		}

		data = strAttrData;
		data.nDataType = TEXT;

		if (m_tree.Insert(data,pNod))
		{
			return (DWORD_PTR)pNod;
		}
		else
		{
			return 0;
		}
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, 256, strError);
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, 256, e.ErrorMessage());
	}

	return 0;
}

DWORD_PTR EasyXML2::InsertAfterAttributeA(DWORD_PTR nodeID, const char* strAttrName, const char* strAttrData)
{
	_bstr_t bstrAttrName(strAttrName), bstrAttrData(strAttrData);
	return InsertAfterAttribute(nodeID, (const wchar_t*)bstrAttrName, (const wchar_t*)bstrAttrData);
}

bool EasyXML2::SetNodeData(DWORD_PTR nodeID, const wchar_t* strData)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	bool flag = IsOpen();

	if (flag == false || nodeID == 0) return false;

	Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;

	ClearError();

	if (!ChangeXMLData(pNod,strData)) 
	{
		return false;
	}

	TreeData data = pNod->GetData();
	data = strData;
	pNod->SetData(data);

	return true;
}

bool EasyXML2::SetNodeDataA(DWORD_PTR nodeID, const char* strData)
{
	_bstr_t bstrData(strData);
	return SetNodeData(nodeID, (const wchar_t*)bstrData);
}

bool EasyXML2::GetNodeData(DWORD_PTR nodeID, wchar_t* pStrData, DataType* pNodeType)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	if (nodeID == 0)
	{
		return false;
	}

	Tree<TreeData>::Node* pNod = (Tree<TreeData>::Node*)nodeID;
	TreeData data = pNod->GetData();

	if (data.nDataType == TEXT)
	{
		pNod = pNod->GetParent();
		TreeData parentData = pNod->GetData();
		if (parentData.nDataType == ATTRIBUTE) *pNodeType = ATTRIBUTE_TEXT;
		else *pNodeType = TEXT;
	}
	else
	{
		*pNodeType = (DataType)data.nDataType;
	}

	const wchar_t* str = data.GetStrData();
	wcscpy(pStrData, str);

	return true;
}

bool EasyXML2::GetNodeDataA(DWORD_PTR nodeID, char* pStrData, DataType* pNodeType)
{
	wchar_t wstrData[256];
	bool isResult = GetNodeData(nodeID, wstrData, pNodeType);

	if (isResult)
	{
		_bstr_t bstrData(wstrData);
		strcpy(pStrData, (char*)bstrData);
	}

	return isResult;
}

bool EasyXML2::RemoveNode(DWORD_PTR nodeID)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	if (nodeID == 0) return false;
	return RemoveData((Tree<TreeData>::Node*)nodeID);
}

void EasyXML2::GetErrorMessage(wchar_t* pStrError)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	wcscpy(pStrError, m_strError);
}

void EasyXML2::GetErrorMessageA(char* pStrError)
{
	// TODO: 여기에 구현 코드를 추가합니다.
	_bstr_t bstrError(m_strError);
	strcpy(pStrError, (char*)bstrError);
}

void EasyXML2::ClearError()
{
	wcscpy_s(m_strError, ERROR_LENGTH, L"");
}

// pParent : Attribute의 텍스트일 경우 해당하는 노드가 없다.
//           이 경우에는 pParent가 true가 되고, 그 부모 노드인 Attribute이 리턴된다.
void EasyXML2::FindNode(MSXML2::IXMLDOMNodePtr& pNod, MSXML2::IXMLDOMNodePtr& pParentNode, Tree<TreeData>::Node* pTreeNode, bool* pParent)
{
	std::list< Tree<TreeData>::Node* > listTreeNode;
	*pParent = false;
	pParentNode = NULL;

	try
	{
		while (pTreeNode)
		{
			listTreeNode.push_back(pTreeNode);
			pTreeNode = pTreeNode->GetParent();
		}

		std::list< Tree<TreeData>::Node* >::const_iterator p = listTreeNode.end();
		int nIndex;
		pNod = (MSXML2::IXMLDOMNode*)m_pDoc;

		do
		{
			--p;
			pTreeNode = *p;
			GetIndex(pTreeNode,nIndex);

			TreeData& rData = pTreeNode->GetData();
			const wchar_t* strItem = rData.GetStrData();
			int nType		= rData.nDataType;

			if (nType == ELEMENT || nType == ROOT)
			{
				MSXML2::IXMLDOMNodeListPtr pList = pNod->selectNodes(strItem);
				pNod = pList->item[nIndex];
			}
			else if (nType == ATTRIBUTE)
			{
				if (p == listTreeNode.begin())
				{
					pParentNode = pNod;
					if (!FindAttribute(pNod,(wchar_t*)strItem)) throw CAN_NOT_FIND_NODE;
				}
				else
				{
					--p;
					if (p == listTreeNode.begin())
					{
						pParentNode = pNod;
						if (!FindAttribute(pNod,(wchar_t*)strItem)) throw CAN_NOT_FIND_NODE;
						*pParent = true;
					}
					else throw CAN_NOT_USE_TYPE;
				}
			}
			else if (nType == TEXT)
			{
				if (p == listTreeNode.begin())
				{					
					MSXML2::IXMLDOMNodeListPtr childList = pNod->GetchildNodes();
					long nItemCount = childList->Getlength();

					for (long i=0;i<nItemCount;i++)
					{
						MSXML2::IXMLDOMNodePtr child = childList->Getitem(i);
						if (child->GetnodeType() == MSXML2::NODE_TEXT)
						{
							_bstr_t strValue = child->GetnodeValue();
							if (strValue != _bstr_t(strItem)) throw CAN_NOT_FIND_NODE;
							pNod = child;
							break;
						}
					}

					//_bstr_t strValue = pNod->GetnodeValue();
					//if (strValue != _bstr_t(strItem)) throw CAN_NOT_FIND_NODE;
				}
				else throw CAN_NOT_USE_TYPE;
			}
			else throw CAN_NOT_USE_TYPE;
		}
		while (p != listTreeNode.begin());
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}
}

// pTreeNod : 삽입할 부모 노드
// data : 삽입될 자식 노드의 데이터
// data의 타입이 ATTRIBUTE일 경우, InsertNodeData() 대신 InsertAttributeNodeData()를 사용한다.
bool EasyXML2::InsertNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data)
{
	if (pTreeNod == 0) return false;
	int len = (int)wcslen(data.GetStrData());
	if (len <= 0) return false;

	try
	{
		TreeData nodeData = pTreeNod->GetData();

		if (nodeData.nDataType != ELEMENT && nodeData.nDataType != ROOT) throw CAN_NOT_INSERT_DATA;

		bool bParent;
		MSXML2::IXMLDOMNodePtr pNod, pParentNode;
		FindNode(pNod,pParentNode,pTreeNod,&bParent);

		if (pNod != NULL)
		{
			if (bParent) throw CAN_NOT_INSERT_DATA;
		}

		return InsertNodeData((MSXML2::IXMLDOMNode*)pNod,data);
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return false;
}

bool EasyXML2::InsertNodeData(MSXML2::IXMLDOMElementPtr pElement, TreeData data)
{
	_bstr_t str = data.GetStrData();

	try
	{
		if (data.nDataType == ELEMENT) 
		{
			MSXML2::IXMLDOMElementPtr pNod = m_pDoc->createElement(str.GetBSTR());
			pElement->appendChild(pNod);
		}
		else if (data.nDataType == TEXT)
		{
			MSXML2::IXMLDOMTextPtr pText = m_pDoc->createTextNode(str.GetBSTR());
			pElement->appendChild((MSXML2::IXMLDOMNode*)pText);
		}
		/*else if (data.nDataType == ATTRIBUTE)
		{
			wchar_t strName[256], strValue[256];
			if (!GetAttributeNameAndValue(data.GetStrData(),strName,strValue))
			{
				throw ATTRIBUTE_TEXT_ERROR;
			}

			pElement->setAttribute(_bstr_t(strName).GetBSTR(),_bstr_t(strValue).GetBSTR());
		}*/
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return true;
}

bool EasyXML2::InsertAttributeNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data, const wchar_t* strAttrData)
{
	if (pTreeNod == 0) return false;
	int len = (int)wcslen(data.GetStrData());
	if (len <= 0) return false;

	try
	{
		TreeData nodeData = pTreeNod->GetData();

		if (nodeData.nDataType != ELEMENT && nodeData.nDataType != ROOT) throw CAN_NOT_INSERT_DATA;

		bool bParent;
		MSXML2::IXMLDOMNodePtr pNod, pParentNode;
		FindNode(pNod,pParentNode,pTreeNod,&bParent);

		if (pNod != NULL)
		{
			if (bParent) throw CAN_NOT_INSERT_DATA;
		}

		return InsertAttributeNodeData((MSXML2::IXMLDOMNode*)pNod, data, strAttrData);
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return false;
}

bool EasyXML2::InsertAttributeNodeData(MSXML2::IXMLDOMElementPtr pElement, TreeData data, const wchar_t* strAttrData)
{
	try
	{
		if (data.nDataType == ATTRIBUTE)
		{
			pElement->setAttribute(data.GetStrData(), strAttrData);
		}
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return true;
}

bool EasyXML2::GetAttributeNameAndValue(const wchar_t* strSource, wchar_t* strName, wchar_t* strValue)
{
	if (strSource == 0 || strName == 0 || strValue == 0) return false;
	int len = (int)wcslen(strSource);
	if (len <= 0) return false;

	int nIndex = -1;

	for (int i=0;i<len;i++)
	{
		if (strSource[i] == '|') //**********************************************************
		{
			nIndex = i;
			break;
		}
	}

	if (nIndex <= 0 || nIndex == len-1)
	{
		return false;
	}

	wcscpy_s(strName, ERROR_LENGTH, strSource);
	strName[nIndex] = 0;
	wcscpy_s(strValue, ERROR_LENGTH, &strSource[nIndex+1]);

	return true;
}

// rIndex : 같은 이름을 가진 노드가 여러개 있을 경우 몇 번째 노드의 것인지 알려준다.
void EasyXML2::GetIndex(Tree<TreeData>::Node* pNod, int& rIndex)
{
	rIndex = 0;
	if (pNod == 0) return;

	TreeData& rData = pNod->GetData();
	const wchar_t* str = rData.GetStrData();
	int nType  = rData.nDataType;
	Tree<TreeData>::Node* nod = pNod->GetPrev();

	while (nod)
	{
		TreeData rData1 = nod->GetData();
		if (nType == rData1.nDataType && !wcscmp(str,rData1.GetStrData())) rIndex++;
		nod = nod->GetPrev();
	}
}

bool EasyXML2::FindAttribute(MSXML2::IXMLDOMNodePtr& pNod, const wchar_t* strItem)
{
	try
	{
		MSXML2::IXMLDOMNamedNodeMapPtr pNodeMap = pNod->Getattributes();
		int nAttrSize = pNodeMap->Getlength();

		for (int i=0;i<nAttrSize;i++)
		{
			MSXML2::IXMLDOMAttributePtr pAttr = pNodeMap->Getitem(i);

			_bstr_t strAttr = pAttr->GetnodeName();
			if (!wcscmp((const wchar_t*)strAttr,strItem))
			{
				pNod = pAttr;
				return true;
			}
		}
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return false;
}

bool EasyXML2::ReadElement(void* pData, Tree<TreeData>::Node* pTreeNod)
{
	if (pData == 0) return false;
	MSXML2::IXMLDOMNodePtr pNod = (MSXML2::IXMLDOMNode*)pData;
	TreeData data;

	try
	{
		if (pTreeNod == 0)	// Root Element
		{
			data.nDataType = ROOT;
			_bstr_t str = pNod->GetnodeName();
			data = (const wchar_t*)str;
			pTreeNod = m_tree.Insert(data);
		}
		else
		{
			data.nDataType = ELEMENT;
			_bstr_t str = pNod->GetnodeName();
			data = (const wchar_t*)str;
			pTreeNod = m_tree.Insert(data,pTreeNod);
		}

		if (!ReadAttribute(pNod,pTreeNod)) return false;
		pNod = pNod->firstChild;

		while (pNod)
		{
			if (pNod->nodeType == MSXML2::NODE_ELEMENT)
			{
				if (!ReadElement(pNod,pTreeNod)) return false;
			}
			else if (pNod->nodeType == MSXML2::NODE_TEXT)
			{
				if (!ReadText(pNod,pTreeNod)) return false;
			}
			else if (pNod->nodeType == MSXML2::NODE_COMMENT)
			{
				//ReadComment(pNod);
			}

			pNod = pNod->nextSibling;
		}
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return true;
}

bool EasyXML2::ReadAttribute(void* pData, Tree<TreeData>::Node* pTreeNod)
{
	if (pData == 0) return false;
	MSXML2::IXMLDOMNodePtr pNod = (MSXML2::IXMLDOMNode*)pData;

	try
	{
		MSXML2::IXMLDOMNamedNodeMapPtr pNodeMap = pNod->Getattributes();
		int nAttrSize = pNodeMap->Getlength();

		for (int i=0;i<nAttrSize;i++)
		{
			MSXML2::IXMLDOMNodePtr pAttr = pNodeMap->Getitem(i);

			TreeData dataAttr, dataAttrText;
			dataAttr.nDataType = ATTRIBUTE;
			_bstr_t str = pAttr->GetnodeName();
			dataAttr = (const wchar_t*)str;
			dataAttrText.nDataType = TEXT;
			str = pAttr->GetnodeValue();
			dataAttrText = (const wchar_t*)str;

			Tree<TreeData>::Node* pAttrNod = m_tree.Insert(dataAttr,pTreeNod);
			m_tree.Insert(dataAttrText,pAttrNod);
		}
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return true;
}

bool EasyXML2::ReadText(void* pData, Tree<TreeData>::Node* pTreeNod)
{
	if (pData == 0) return false;
	MSXML2::IXMLDOMNodePtr pNod = (MSXML2::IXMLDOMNode*)pData;

	try
	{
		TreeData data;
		data.nDataType = TEXT;
		_bstr_t str = pNod->Gettext();
		data = (const wchar_t*)str;
		m_tree.Insert(data,pTreeNod);
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return true;
}

// pTreeNod : 추가될 노드 뒤에 놓여진 노드
// data : 삽입될 자식 노드의 데이터
// data의 타입이 ATTRIBUTE일 경우, InsertBeforeNodeData() 대신 InsertBeforeAttributeNodeData()를 사용한다.
bool EasyXML2::InsertBeforeNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data)
{
	if (pTreeNod == 0) return false;
	int len = (int)wcslen(data.GetStrData());
	if (len <= 0) return false;

	try
	{
		TreeData nodeData = pTreeNod->GetData();

		if (data.nDataType == ELEMENT || data.nDataType == TEXT)// || data.nDataType == ATTRIBUTE)
		{
			bool bParent;
			MSXML2::IXMLDOMNodePtr pNod, pParentNode;
			FindNode(pNod,pParentNode,pTreeNod,&bParent);

			if (pNod != NULL)
			{
				if (bParent) throw CAN_NOT_INSERT_DATA;
			}

			return InsertBeforeNodeData((MSXML2::IXMLDOMNode*)pNod,(MSXML2::IXMLDOMNode*)pParentNode,data);
		}
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return false;
}

bool EasyXML2::InsertBeforeNodeData(MSXML2::IXMLDOMNode* pNod, MSXML2::IXMLDOMNode* pParentNode, TreeData data)
{
	try
	{
		if (data.nDataType == ELEMENT) 
		{
			MSXML2::IXMLDOMElementPtr pElement = pNod;
			MSXML2::IXMLDOMElementPtr pParent = pElement->GetparentNode();
			MSXML2::IXMLDOMNodePtr pChild = pParent->GetfirstChild();

			int nType = pElement->nodeType;
			_bstr_t strTarget;
			if (nType == MSXML2::NODE_TEXT) strTarget = pElement->GetnodeValue();
			else if (nType == MSXML2::NODE_ELEMENT) strTarget = pElement->GetnodeName();

			int nSize = 0, nIndex = -1;

			while (pChild)
			{
				if (nIndex < 0)
				{
					_bstr_t strNode = pChild->GetnodeName();
					if (pChild->nodeType == MSXML2::NODE_ELEMENT) strNode = pChild->GetnodeName();
					else if (pChild->nodeType == MSXML2::NODE_TEXT) strNode = pChild->GetnodeValue();

					if (pChild->nodeType == nType && strNode == strTarget)
					{
						nIndex = nSize;
					}
				}

				nSize++;
				pChild = pChild->GetnextSibling();
			}

			MSXML2::IXMLDOMElementPtr pNewNod = m_pDoc->createElement(_bstr_t(data.GetStrData()).GetBSTR());
			pParent->appendChild(pNewNod);

			pChild = pParent->GetfirstChild();

			for (int i=0;i<nSize;i++)
			{
				if (i >= nIndex)
				{
					MSXML2::IXMLDOMNodePtr pPrev = (MSXML2::IXMLDOMNode*)pChild;
					pChild = pChild->GetnextSibling();
					pParent->appendChild(pPrev);
				}
				else pChild = pChild->GetnextSibling();
			}
		}
		else if (data.nDataType == TEXT)
		{
			MSXML2::IXMLDOMElementPtr pElement = pNod;
			MSXML2::IXMLDOMElementPtr pParent = pElement->GetparentNode();
			MSXML2::IXMLDOMNodePtr pChild = pParent->GetfirstChild();

			int nType = pElement->nodeType;
			_bstr_t strTarget;
			if (nType == MSXML2::NODE_TEXT) strTarget = pElement->GetnodeValue();
			else if (nType == MSXML2::NODE_ELEMENT) strTarget = pElement->GetnodeName();

			int nSize = 0, nIndex = -1;

			while (pChild)
			{
				if (nIndex < 0)
				{
					_bstr_t strNode;
					if (pChild->nodeType == MSXML2::NODE_ELEMENT) strNode = pChild->GetnodeName();
					else if (pChild->nodeType == MSXML2::NODE_TEXT) strNode = pChild->GetnodeValue();

					if (pChild->nodeType == nType && strNode == strTarget)
					{
						nIndex = nSize;
					}
				}

				nSize++;
				pChild = pChild->GetnextSibling();
			}

			MSXML2::IXMLDOMTextPtr pText = m_pDoc->createTextNode(_bstr_t(data.GetStrData()).GetBSTR());
			pParent->appendChild(pText);

			pChild = pParent->GetfirstChild();

			for (int i=0;i<nSize;i++)
			{
				if (i >= nIndex)
				{
					MSXML2::IXMLDOMNodePtr pPrev = (MSXML2::IXMLDOMNode*)pChild;
					pChild = pChild->GetnextSibling();
					pParent->appendChild(pPrev);
				}
				else pChild = pChild->GetnextSibling();
			}
		}
		/*else if (data.nDataType == ATTRIBUTE)
		{
			wchar_t strName[256], strValue[256];
			if (!GetAttributeNameAndValue(data.GetStrData(),strName,strValue))
			{
				throw ATTRIBUTE_TEXT_ERROR;
			}

			MSXML2::IXMLDOMNodePtr pElement = pNod;
			MSXML2::IXMLDOMElementPtr pParent = pParentNode;
			MSXML2::IXMLDOMNamedNodeMapPtr pNodeMap = pParent->Getattributes();
			MSXML2::IXMLDOMAttributePtr pAttr;

			_bstr_t strTarget = pElement->GetnodeName();

			int nAttrSize = pNodeMap->Getlength();
			int nIndex = -1, i;

			for (i=0;i<nAttrSize;i++)
			{
				pAttr = pNodeMap->Getitem(i);

				_bstr_t strAttrName = pAttr->GetnodeName();
				if (strAttrName == strTarget)
				{
					nIndex = i;
					break;
				}
			}

			pParent->setAttribute(_bstr_t(strName).GetBSTR(),_bstr_t(strValue).GetBSTR());

			for (i=0;i<nAttrSize;i++)
			{
				if (i >= nIndex)
				{
					pAttr = pNodeMap->Getitem(nIndex);
					pNodeMap->removeNamedItem(pAttr->GetnodeName());
					pNodeMap->setNamedItem(pAttr);
				}
			}
		}*/
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return true;
}

bool EasyXML2::InsertBeforeAttributeNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data, const wchar_t* strAttrData)
{
	if (pTreeNod == 0) return false;
	int len = (int)wcslen(data.GetStrData());
	if (len <= 0) return false;

	try
	{
		TreeData nodeData = pTreeNod->GetData();

		if (data.nDataType == ATTRIBUTE)
		{
			bool bParent;
			MSXML2::IXMLDOMNodePtr pNod, pParentNode;
			FindNode(pNod,pParentNode,pTreeNod,&bParent);

			if (pNod != NULL)
			{
				if (bParent) throw CAN_NOT_INSERT_DATA;
			}

			return InsertBeforeAttributeNodeData((MSXML2::IXMLDOMNode*)pNod, (MSXML2::IXMLDOMNode*)pParentNode, data, strAttrData);
		}
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return false;
}

bool EasyXML2::InsertBeforeAttributeNodeData(MSXML2::IXMLDOMNode* pNod, MSXML2::IXMLDOMNode* pParentNode, TreeData data, const wchar_t* strAttrData)
{
	try
	{
		if (data.nDataType == ATTRIBUTE)
		{
			/*wchar_t strName[256], strValue[256];
			if (!GetAttributeNameAndValue(data.GetStrData(),strName,strValue))
			{
				throw ATTRIBUTE_TEXT_ERROR;
			}*/

			MSXML2::IXMLDOMNodePtr pElement = pNod;
			MSXML2::IXMLDOMElementPtr pParent = pParentNode;
			MSXML2::IXMLDOMNamedNodeMapPtr pNodeMap = pParent->Getattributes();
			MSXML2::IXMLDOMAttributePtr pAttr;

			_bstr_t strTarget = pElement->GetnodeName();

			int nAttrSize = pNodeMap->Getlength();
			int nIndex = -1, i;

			for (i=0;i<nAttrSize;i++)
			{
				pAttr = pNodeMap->Getitem(i);

				_bstr_t strAttrName = pAttr->GetnodeName();
				if (strAttrName == strTarget)
				{
					nIndex = i;
					break;
				}
			}

			pParent->setAttribute(data.GetStrData(), strAttrData);

			for (i=0;i<nAttrSize;i++)
			{
				if (i >= nIndex)
				{
					pAttr = pNodeMap->Getitem(nIndex);
					pNodeMap->removeNamedItem(pAttr->GetnodeName());
					pNodeMap->setNamedItem(pAttr);
				}
			}
		}
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return true;
}

// pTreeNod : 추가될 노드 앞에 놓여진 노드
// data : 삽입될 자식 노드의 데이터
// data의 타입이 ATTRIBUTE일 경우, InsertAfterNodeData() 대신 InsertAfterAttributeNodeData()를 사용한다.
bool EasyXML2::InsertAfterNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data)
{
	if (pTreeNod == 0) return false;
	int len = (int)wcslen(data.GetStrData());
	if (len <= 0) return false;

	try
	{
		TreeData nodeData = pTreeNod->GetData();

		if (data.nDataType == ELEMENT || data.nDataType == TEXT)// || data.nDataType == ATTRIBUTE)
		{
			bool bParent;
			MSXML2::IXMLDOMNodePtr pNod, pParentNode;
			FindNode(pNod,pParentNode,pTreeNod,&bParent);

			if (pNod != NULL)
			{
				if (bParent) throw CAN_NOT_INSERT_DATA;
			}

			return InsertAfterNodeData((MSXML2::IXMLDOMNode*)pNod,(MSXML2::IXMLDOMNode*)pParentNode,data);
		}
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return false;
}

bool EasyXML2::InsertAfterNodeData(MSXML2::IXMLDOMNode* pNod, MSXML2::IXMLDOMNode* pParentNode, TreeData data)
{
	try
	{
		if (data.nDataType == ELEMENT) 
		{
			MSXML2::IXMLDOMElementPtr pElement = pNod;
			MSXML2::IXMLDOMElementPtr pParent = pElement->GetparentNode();
			MSXML2::IXMLDOMNodePtr pChild = pParent->GetfirstChild();

			int nType = pElement->nodeType;
			_bstr_t strTarget;
			if (nType == MSXML2::NODE_TEXT) strTarget = pElement->GetnodeValue();
			else if (nType == MSXML2::NODE_ELEMENT) strTarget = pElement->GetnodeName();

			int nSize = 0, nIndex = -1;

			while (pChild)
			{
				if (nIndex < 0)
				{
					_bstr_t strNode = pChild->GetnodeName();
					if (pChild->nodeType == MSXML2::NODE_ELEMENT) strNode = pChild->GetnodeName();
					else if (pChild->nodeType == MSXML2::NODE_TEXT) strNode = pChild->GetnodeValue();

					if (pChild->nodeType == nType && strNode == strTarget)
					{
						nIndex = nSize + 1;
					}
				}

				nSize++;
				pChild = pChild->GetnextSibling();
			}

			MSXML2::IXMLDOMElementPtr pNewNod = m_pDoc->createElement(_bstr_t(data.GetStrData()));
			pParent->appendChild(pNewNod);

			pChild = pParent->GetfirstChild();

			for (int i=0;i<nSize;i++)
			{
				if (i >= nIndex)
				{
					MSXML2::IXMLDOMNodePtr pPrev = (MSXML2::IXMLDOMNode*)pChild;
					pChild = pChild->GetnextSibling();
					pParent->appendChild(pPrev);
				}
				else pChild = pChild->GetnextSibling();
			}
		}
		else if (data.nDataType == TEXT)
		{
			MSXML2::IXMLDOMElementPtr pElement = pNod;
			MSXML2::IXMLDOMElementPtr pParent = pElement->GetparentNode();
			MSXML2::IXMLDOMNodePtr pChild = pParent->GetfirstChild();

			int nType = pElement->nodeType;
			_bstr_t strTarget;
			if (nType == MSXML2::NODE_TEXT) strTarget = pElement->GetnodeValue();
			else if (nType == MSXML2::NODE_ELEMENT) strTarget = pElement->GetnodeName();

			int nSize = 0, nIndex = -1;

			while (pChild)
			{
				if (nIndex < 0)
				{
					_bstr_t strNode;
					if (pChild->nodeType == MSXML2::NODE_ELEMENT) strNode = pChild->GetnodeName();
					else if (pChild->nodeType == MSXML2::NODE_TEXT) strNode = pChild->GetnodeValue();

					if (pChild->nodeType == nType && strNode == strTarget)
					{
						nIndex = nSize + 1;
					}
				}

				nSize++;
				pChild = pChild->GetnextSibling();
			}

			MSXML2::IXMLDOMTextPtr pText = m_pDoc->createTextNode(_bstr_t(data.GetStrData()).GetBSTR());
			pParent->appendChild(pText);

			pChild = pParent->GetfirstChild();

			for (int i=0;i<nSize;i++)
			{
				if (i >= nIndex)
				{
					MSXML2::IXMLDOMNodePtr pPrev = (MSXML2::IXMLDOMNode*)pChild;
					pChild = pChild->GetnextSibling();
					pParent->appendChild(pPrev);
				}
				else pChild = pChild->GetnextSibling();
			}
		}
		/*else if (data.nDataType == ATTRIBUTE)
		{
			wchar_t strName[256], strValue[256];
			if (!GetAttributeNameAndValue((const TCHAR*)data.GetStrData(),strName,strValue))
			{
				throw ATTRIBUTE_TEXT_ERROR;
			}

			MSXML2::IXMLDOMNodePtr pElement = pNod;
			MSXML2::IXMLDOMElementPtr pParent = pParentNode;
			MSXML2::IXMLDOMNamedNodeMapPtr pNodeMap = pParent->Getattributes();
			MSXML2::IXMLDOMAttributePtr pAttr;

			_bstr_t strTarget = pElement->GetnodeName();

			int nAttrSize = pNodeMap->Getlength();
			int nIndex = -1, i;

			for (i=0;i<nAttrSize;i++)
			{
				pAttr = pNodeMap->Getitem(i);

				_bstr_t strAttrName = pAttr->GetnodeName();
				if (strAttrName == strTarget)
				{
					nIndex = i + 1;
					break;
				}
			}

			pParent->setAttribute(_bstr_t(strName).GetBSTR(),_bstr_t(strValue).GetBSTR());

			for (i=0;i<nAttrSize;i++)
			{
				if (i >= nIndex)
				{
					pAttr = pNodeMap->Getitem(nIndex);
					pNodeMap->removeNamedItem(pAttr->GetnodeName());
					pNodeMap->setNamedItem(pAttr);
				}
			}
		}*/
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return true;
}

bool EasyXML2::InsertAfterAttributeNodeData(Tree<TreeData>::Node* pTreeNod, TreeData data, const wchar_t* strAttrData)
{
	if (pTreeNod == 0) return false;
	int len = (int)wcslen(data.GetStrData());
	if (len <= 0) return false;

	try
	{
		TreeData nodeData = pTreeNod->GetData();

		if (data.nDataType == ATTRIBUTE)
		{
			bool bParent;
			MSXML2::IXMLDOMNodePtr pNod, pParentNode;
			FindNode(pNod,pParentNode,pTreeNod,&bParent);

			if (pNod != NULL)
			{
				if (bParent) throw CAN_NOT_INSERT_DATA;
			}

			return InsertAfterAttributeNodeData((MSXML2::IXMLDOMNode*)pNod, (MSXML2::IXMLDOMNode*)pParentNode, data, strAttrData);
		}
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return false;
}

bool EasyXML2::InsertAfterAttributeNodeData(MSXML2::IXMLDOMNode* pNod, MSXML2::IXMLDOMNode* pParentNode, TreeData data, const wchar_t* strAttrData)
{
	try
	{
		if (data.nDataType == ATTRIBUTE)
		{
			/*wchar_t strName[256], strValue[256];
			if (!GetAttributeNameAndValue((const TCHAR*)data.GetStrData(),strName,strValue))
			{
				throw ATTRIBUTE_TEXT_ERROR;
			}*/

			MSXML2::IXMLDOMNodePtr pElement = pNod;
			MSXML2::IXMLDOMElementPtr pParent = pParentNode;
			MSXML2::IXMLDOMNamedNodeMapPtr pNodeMap = pParent->Getattributes();
			MSXML2::IXMLDOMAttributePtr pAttr;

			_bstr_t strTarget = pElement->GetnodeName();

			int nAttrSize = pNodeMap->Getlength();
			int nIndex = -1, i;

			for (i=0;i<nAttrSize;i++)
			{
				pAttr = pNodeMap->Getitem(i);

				_bstr_t strAttrName = pAttr->GetnodeName();
				if (strAttrName == strTarget)
				{
					nIndex = i + 1;
					break;
				}
			}

			pParent->setAttribute(data.GetStrData(), strAttrData);

			for (i=0;i<nAttrSize;i++)
			{
				if (i >= nIndex)
				{
					pAttr = pNodeMap->Getitem(nIndex);
					pNodeMap->removeNamedItem(pAttr->GetnodeName());
					pNodeMap->setNamedItem(pAttr);
				}
			}
		}
		else return false;
	}
	catch (const wchar_t* strError)
	{
		throw strError;
	}
	catch (_com_error& e)
	{
		wcscpy_s(g_strError, ERROR_LENGTH, e.ErrorMessage());
		throw g_strError;
	}

	return true;
}

bool EasyXML2::ChangeXMLData(Tree<TreeData>::Node* pTreeNod, const wchar_t* strData)
{
	try
	{
		bool bParent;
		MSXML2::IXMLDOMNodePtr pNod, pParentNode;
		FindNode(pNod,pParentNode,pTreeNod,&bParent);

		if (bParent)
		{
			pNod->PutnodeValue(_bstr_t(strData).GetBSTR());
		}
		else
		{
			if (pNod->nodeType == MSXML2::NODE_ELEMENT)
			{
				MSXML2::IXMLDOMElementPtr pNewNod = m_pDoc->createElement(_bstr_t(strData).GetBSTR());

				MSXML2::IXMLDOMNamedNodeMapPtr pNodeMap = pNod->Getattributes();
				int nAttrSize = pNodeMap->Getlength();

				for (int i=0;i<nAttrSize;i++)
				{
					MSXML2::IXMLDOMNodePtr pAttr = pNodeMap->Getitem(i);
					pNewNod->setAttribute(pAttr->GetnodeName().GetBSTR(),pAttr->Gettext());
				}

				MSXML2::IXMLDOMNodePtr pChild = pNod->firstChild;
				while (pChild)
				{
					MSXML2::IXMLDOMNodePtr pPrev = pChild;
					pChild = pChild->nextSibling;
					pNewNod->appendChild(pPrev);
				}

				pParentNode = pNod->GetparentNode();
				pParentNode->replaceChild(pNewNod,pNod);
			}
			else if (pNod->nodeType == MSXML2::NODE_TEXT)
			{
				pNod->PutnodeValue(_bstr_t(strData).GetBSTR());
			}
			else if (pNod->nodeType == MSXML2::NODE_ATTRIBUTE)
			{
				_bstr_t strNode = pNod->GetnodeName();
				_bstr_t strValue = pNod->GetnodeValue();
				MSXML2::IXMLDOMElementPtr pElement = pParentNode;

				MSXML2::IXMLDOMNamedNodeMapPtr pNodeMap = pElement->Getattributes();
				int nAttrSize = pNodeMap->Getlength();
				int i, nIndex = -1;

				for (i=0;i<nAttrSize;i++)
				{
					MSXML2::IXMLDOMAttributePtr pAttr = pNodeMap->Getitem(i);
					if (pAttr->GetnodeName() == strNode)
					{
						nIndex = i;
						break;
					}
				}

				pElement->setAttribute(_bstr_t(strData),strValue);
				pElement->removeAttribute(strNode);

				for (i=0;i<nAttrSize-1;i++)
				{
					if (i >= nIndex)
					{
						MSXML2::IXMLDOMAttributePtr pAttr = pNodeMap->Getitem(nIndex);

						strNode = pAttr->GetnodeName();
						strValue = pAttr->GetnodeValue();

						pElement->removeAttribute(strNode);
						pElement->setAttribute(strNode,strValue);
					}
				}
			}
			else return false;
		}
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, ERROR_LENGTH, strError);
		return false;
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, ERROR_LENGTH, e.ErrorMessage());
		return false;
	}

	return true;
}

bool EasyXML2::RemoveData(Tree<TreeData>::Node* pTreeNod)
{
	try
	{
		bool bParent;
		MSXML2::IXMLDOMNodePtr pNod, pParentNode;
		FindNode(pNod,pParentNode,pTreeNod,&bParent);

		if (pNod)
		{
			if (bParent) throw CAN_NOT_REMOVE_ATTR_TEXT;
		}

		TreeData data = pTreeNod->GetData();

		if (data.nDataType == ROOT) throw CAN_NOT_REMVOE_ROOT;
		else if (data.nDataType == ELEMENT || data.nDataType == TEXT)
		{
			pParentNode = pNod->GetparentNode();
			pParentNode->removeChild(pNod);
		}
		else if (data.nDataType == ATTRIBUTE)
		{
			MSXML2::IXMLDOMNamedNodeMapPtr pNodeMap = pParentNode->Getattributes();
			pNodeMap->removeNamedItem(pNod->GetnodeName());
		}
		else throw CAN_NOT_USE_TYPE;

		m_tree.Remove(pTreeNod);
	}
	catch (const wchar_t* strError)
	{
		wcscpy_s(m_strError, ERROR_LENGTH, strError);
		return false;
	}
	catch (_com_error& e)
	{
		wcscpy_s(m_strError, ERROR_LENGTH, e.ErrorMessage());
		return false;
	}

	return true;
}

void EasyXML2::SetStyleSheet(const wchar_t* strStyleSheet)
{
	if (strStyleSheet)
	{
		wcscpy(m_strStyleSheet, strStyleSheet);
	}
}

void EasyXML2::SetStyleSheetA(const char* strStyleSheet)
{
	if (strStyleSheet)
	{
		_bstr_t bstrStyleSheet(strStyleSheet);
		wcscpy(m_strStyleSheet, (const wchar_t*)bstrStyleSheet);
	}
}

void EasyXML2::GetStyleSheet(wchar_t* strStyleSheet)
{
	if (strStyleSheet == 0) return;
	wcscpy(strStyleSheet, m_strStyleSheet);
}

void EasyXML2::GetStyleSheetA(char* strStyleSheet)
{
	if (strStyleSheet == 0) return;

	_bstr_t bstrStyleSheet(m_strStyleSheet);
	strcpy(strStyleSheet, (char*)bstrStyleSheet);
}

void EasyXML2::SetIndent(bool bIndent)
{
	m_bIndent = bIndent;
}

bool EasyXML2::GetIndent() const
{
	return m_bIndent;
}

DWORD_PTR EasyXML2::MakeElement(DWORD_PTR nodeID, const wchar_t* strElement, const wchar_t* strText)
{
	nodeID = InsertData(nodeID,ELEMENT,strElement);
	if (strText == 0) return nodeID;

	InsertData(nodeID,TEXT,strText);
	return nodeID;
}

DWORD_PTR EasyXML2::MakeElementA(DWORD_PTR nodeID, const char* strElement, const char* strText)
{
	_bstr_t bstrElement(strElement);
	nodeID = InsertData(nodeID, ELEMENT, (const wchar_t*)bstrElement);
	if (strText == 0) return nodeID;

	_bstr_t bstrText(strText);
	InsertData(nodeID, TEXT, (const wchar_t*)bstrText);
	return nodeID;
}

DWORD_PTR EasyXML2::MakeAttribute(DWORD_PTR nodeID, const wchar_t* strAttrName, const wchar_t* strAttrData)
{
	/*wchar_t strAttribute[256];
	swprintf_s(strAttribute, 256, L"%s|%s", strAttr, strText);*/
	return InsertAttributeData(nodeID, strAttrName, strAttrData);
}

DWORD_PTR EasyXML2::MakeAttributeA(DWORD_PTR nodeID, const char* strAttrName, const char* strAttrData)
{
	return InsertAttributeDataA(nodeID, strAttrName, strAttrData);
}

DWORD_PTR EasyXML2::MakeText(DWORD_PTR nodeID, const wchar_t* strText)
{
	return InsertData(nodeID,TEXT,strText);
}

DWORD_PTR EasyXML2::MakeTextA(DWORD_PTR nodeID, const char* strText)
{
	return InsertDataA(nodeID,TEXT,strText);
}

void EasyXML2::SetLocale(const wchar_t* strLocale)
{
	if (strLocale == 0) return;
	wcscpy_s(m_strLocale, strLocale);
}

void EasyXML2::SetLocaleA(const char* strLocale)
{
	if (strLocale == 0) return;

	_bstr_t bstrLocale(strLocale);
	wcscpy_s(m_strLocale, (const wchar_t*)bstrLocale);
}

void EasyXML2::GetLocale(wchar_t* strLocale) const
{
	if (strLocale == 0) return;
	wcscpy(strLocale, m_strLocale);
}

void EasyXML2::GetLocaleA(char* strLocale) const
{
	if (strLocale == 0) return;

	_bstr_t bstrLocale(m_strLocale);
	strcpy(strLocale, (char*)bstrLocale);
}

DWORD_PTR EasyXML2::GetChildNodeCount(DWORD_PTR nNodeID, DataType type)
{
	Tree<TreeData>::Node* pTreeNod = (Tree<TreeData>::Node*)nNodeID;
	if (pTreeNod == 0) return 0;

	DWORD_PTR nCount = 0;
	Tree<TreeData>::Node* pNode = pTreeNod->child;

	while (pNode)
	{
		if (pNode->data.nDataType == type)
		{
			nCount++;
		}

		pNode = pNode->next;
	}

	return nCount;
}

DWORD_PTR EasyXML2::FindNodeTree(DWORD_PTR nParentID, const wchar_t* strNodeName, DataType type)
{
	DataType nNodeType;
	wchar_t strData[256];

	DWORD_PTR nNodeID = GetChildNode(nParentID);

	// 먼저 자식노드들부터 검사
	while (nNodeID)
	{
		bool bResult = GetNodeData(nNodeID,strData,&nNodeType);
		if (!bResult) return 0;

		if (nNodeType == type)
		{
			if (!wcsicmp(strData,strNodeName)) return nNodeID;
		}

		nNodeID = GetNextNode(nNodeID);
	}

	nNodeID = GetChildNode(nParentID);

	// 발견되지 않을 경우 자식의 자식노드들을 검사
	while (nNodeID)
	{
		DWORD_PTR nID = FindNodeTree(nNodeID,strNodeName,type);
		if (nID) return nID;

		nNodeID = GetNextNode(nNodeID);
	}

	return 0;
}

DWORD_PTR EasyXML2::FindNodeTreeA(DWORD_PTR nParentID, const char* strNodeName, DataType type)
{
	_bstr_t bstrNodeName(strNodeName);
	return FindNodeTree(nParentID, (const wchar_t*)bstrNodeName, type);
}

END_NS
