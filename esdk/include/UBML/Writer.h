#pragma once
#include <vector>
#include <string>

namespace UnE
{
	namespace UBML
	{
		class Element;
		class Segment;

		class __declspec(dllexport) Writer
		{
		private:
			Writer(const Writer&);
			void operator =(const Writer&);

		public:
			Writer(void);
			virtual ~Writer(void);

		public:
			void AddElement(Element* pElement);
			unsigned int GetElementCount() const;
			const Element* GetElement(unsigned int nIndex) const;

			void RemoveElement(unsigned int nBeginIndex, unsigned int nEndIndex, bool freeMemory = true);
			void RemoveElement(unsigned int nIndex, bool freeMemory = true);
			void RemoveElement(Element* pElement, bool freeMemory = true);
			void RemoveFirstElement(bool freeMemory = true);
			void RemoveLastElement(bool freeMemory = true);
			void RemoveAll(bool freeMemory = true);

			bool WriteFileA(const char* strFilePath);
			bool WriteFile(const wchar_t* strFilePath);
			bool WriteFile(FILE* fp);

			const std::wstring& GetErrorMessage() const;

			bool ToXMLA(const char* strFilePath);
			bool ToXML(const wchar_t* strFilePath);

			bool ToPrettyXMLA(const char* strFilePath);
			bool ToPrettyXML(const wchar_t* strFilePath);

		protected:
			bool WriteElement(FILE* fp, const Element* pElement);
			bool WriteSegment(FILE* fp, const Segment* pSegment, int nElementTag);

		private:
			std::vector<Element*> m_vecElement;
			std::wstring m_strErrorMessage;
		};
	}
}
