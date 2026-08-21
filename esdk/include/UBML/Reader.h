#pragma once
#pragma once
#include <vector>
#include <string>

namespace UnE
{
	namespace UBML
	{
		class UData;
		class Element;
		class Segment;

		class __declspec(dllexport) Reader
		{
		private:
			Reader(const Reader&);
			void operator =(const Reader&);

		public:
			Reader(void);
			virtual ~Reader(void);

		public:
			unsigned int GetElementCount() const;
			const Element* GetElement(unsigned int nIndex) const;

			bool ReadFileA(const char* strFilePath);
			bool ReadFile(const wchar_t* strFilePath);
			bool ReadFile(FILE* fp);

			void RemoveAll(bool freeMemory = true);

			const std::wstring& GetErrorMessage() const;

			bool ToXMLA(const char* strFilePath);
			bool ToXML(const wchar_t* strFilePath);

		protected:
			bool ReadTag(FILE* fp, int& rTag, bool& isSuccess);
			bool ReadData(FILE* fp, Element* pParentElement);			
			bool ReadElement(FILE* fp, unsigned char typeTag, Element* pParentElement);
			Element* ReadElement(FILE* fp, int* pElementTag = 0);
			bool ReadSegment(FILE* fp, unsigned char typeTag, Element* pParentElement);

		private:
			std::vector<Element*> m_vecElement;
			std::wstring m_strErrorMessage;
		};
	}
}
