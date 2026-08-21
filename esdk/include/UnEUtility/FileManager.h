#pragma once
#include <Windows.h>

namespace UnE
{
	namespace Utility
	{
		class FileManager  
		{
		public:
			enum WriteType {WRITE_INSERT = 0, WRITE_REPLACE};
			enum EncodingType {ANSI = 0, UTF8_BOM, UTF8_NO_BOM, UNICODE_LE, UNICODE_BE, UNKOWN};

		public:
			bool ReadLine(wchar_t* buf, int nSizeToRead, int* pSizeOfBytesRead);
			bool ReadLineA(char* buf, int nSizeToRead, int* pSizeOfBytesRead, char delim = '\n');
			HANDLE GetFileHandle();
			bool Write(wchar_t* str, LONG lOff, UINT nFrom = FILE_BEGIN, int nMode = WRITE_REPLACE);
			bool WriteA(char* str, LONG lOff, UINT nFrom = FILE_BEGIN, int nMode = WRITE_REPLACE);
			BYTE* Read(int nByte);
			void Close();
			bool Seek(LONG lOff, UINT nFrom = FILE_BEGIN);
			virtual bool Open(wchar_t* path, DWORD dwMode = GENERIC_READ|GENERIC_WRITE);
			virtual bool OpenA(char* path, DWORD dwMode = GENERIC_READ|GENERIC_WRITE);
			FileManager();
			virtual ~FileManager();

			EncodingType GetEncodingType() const;

		protected:
			// m_encodingType에 따라 strSrc를 strTrg으로 변환한다.
			void ConversionString(const char* strSrc, int nSrcLen, wchar_t* strTrg);

		protected:
			HANDLE m_hFile;
			EncodingType m_encodingType;
		};
	}
}
