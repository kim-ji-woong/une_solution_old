#ifndef UBMLWriter_h__
#define UBMLWriter_h__

#pragma once
#include <vector>
#include <string>

#include <Poco/DOM/DOMWriter.h>
#include <Poco/DOM/AutoPtr.h>

namespace Poco
{	
	namespace XML
	{
		class Document;	
		class Element;
	}
}
using namespace Poco::XML;

namespace UnE
{

	namespace UBML
	{
		class Element;
		class Segment;

		typedef void (*ToStringFunc)(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);

		class __declspec(dllexport) UBMLWriter : public Poco::XML::DOMWriter		
		{
		public:
			UBMLWriter(void);
			virtual ~UBMLWriter(void);

			Document * m_pDoc;

		public:
			bool WriteFileA(const std::vector<Element*>& rVecElement, const char* strFilePath);
			bool WriteFile(const std::vector<Element*>& rVecElement, const wchar_t* strFilePath);

			const std::wstring& GetErrorMessage() const;

		protected:
			bool WriteFile(const std::vector<Element*>& rVecElement, Document * m_pDoc , const wchar_t* strRootElementName = 0);
			bool WriteElement(Document * m_pDoc, Poco::XML::Element *pNode, const Element* pElement);
			bool WriteSegment(Document * m_pDoc, Poco::XML::Element *pNode, const Segment* pSegment);

			bool WriteXML(const std::vector<Element*>& rVecElement, Document * m_pDoc , const wchar_t* strRootElementName = 0);
			bool AddElement(Document * m_pDoc, Poco::XML::Element *pNode, const Element* pElement);
			bool AddSegment(Document * m_pDoc, Poco::XML::Element *pNode, const Segment* pSegment);

		protected:
			static void IntToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void UIntToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void ShortToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void UShortToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void ByteToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void UByteToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void LONGLONGToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void ULONGLONGToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void FloatToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void DoubleToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);
			static void BoolToString(wchar_t* str, int nBufSize, const void* pData, const wchar_t* strFormat);

			// isString : 0이면 문자열이 아님. 1이면 char*, 2이면 wchar_t*
			static ToStringFunc ToStringFunction(int nDataType, wchar_t* strFormat, int& isString);

		private:
			std::wstring m_strErrorMessage;
		};
	}
}

#endif // UBMLWriter_h__
