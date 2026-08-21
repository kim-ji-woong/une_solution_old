#pragma once
#include <Windows.h>
#include "StringFile.h"

// 문서의 Encoding Type을 알아내어 그에 맞게 Text를 읽어주는 Class
// [2013/02/15] 김지웅

namespace UnE
{
	namespace Utility
	{
		class TextReader
		{
		public:
			enum EncodingType {ANSI = 0, UTF8_BOM, UTF8_NO_BOM, UNICODE_LE, UNICODE_BE, UNKOWN};

		public:
			TextReader();
			virtual ~TextReader();

			bool ReadLine(wchar_t* buf, int nSizeToRead, int* pSizeOfBytesRead);
			HANDLE GetFileHandle();
			void Close();
			//bool Seek(LONG lOff, UINT nFrom = FILE_BEGIN);
			bool Open(wchar_t* path);

			EncodingType GetEncodingType() const;

		protected:
			EncodingType CheckEncodingFileType();
			//bool ReadLineA(char* buf, int nSizeToRead, int *pSizeOfBytesRead);
			bool ReadAll();

		public:
			// srcEncodingType에 따라 strSrc를 strTrg으로 변환한다.
			static void ConversionString(const char* strSrc, int nSrcLen, EncodingType srcEncodingType, wchar_t* strTrg);

		protected:
			HANDLE m_hFile;
			EncodingType m_encodingType;
			StringFile m_memFile;
		};
	}
}
