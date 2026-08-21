#include "stdafx.h"
#include "TextReader.h"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

#define GetFilePointer(hFile) SetFilePointer(hFile, 0, NULL, FILE_CURRENT)

BEGIN_NS(UnE)
BEGIN_NS(Utility)

TextReader::TextReader()
: m_memFile(L"")
{
	m_encodingType = UNKOWN;
}

TextReader::~TextReader()
{
	::CloseHandle(m_hFile);
}

// Return 값 : true이면 BOM이 없는 UTF-8
//             false이면 ANSI
static bool AnalyzeFormatUtf8(char *strPage, int nStringLength)
{
     char *p;
     bool bFind = false;
 
     for (p=strPage;p-strPage < nStringLength;p++) {
          if ((*p & 0x80) == 0x80) {
              bFind = true;
 
              // 상위 비트가 110이고 다음 문자의 상위 비트가 10이면 UTF8맞음
              // p가 문서 끝을 넘거나 중간에 하나라도 규칙에 맞지 않으면 UTF8이 아님
              if ((*p & 0xe0) == 0xc0) {
                   p++;if (p-strPage >= nStringLength) return false;
                   if ((*p & 0xc0) != 0x80) return false;
                   continue;
              }
 
              // 상위 비트가 1110일 때는 다음 두 문자의 상위 비트가 10이어야 한다.
              if ((*p & 0xf0) == 0xe0) {
                   p++;if (p-strPage >= nStringLength) return false;
                   if ((*p & 0xc0) != 0x80) return false;
                   p++;if (p-strPage >= nStringLength) return false;
                   if ((*p & 0xc0) != 0x80) return false;
                   continue;
              }
 
              // 상위 비트가 11110일 때는 다음 세 문자의 상위 비트가 10이어야 한다.
              if ((*p & 0xf8) == 0xf0) {
                   p++;if (p-strPage >= nStringLength) return false;
                   if ((*p & 0xc0) != 0x80) return false;
                   p++;if (p-strPage >= nStringLength) return false;
                   if ((*p & 0xc0) != 0x80) return false;
                   p++;if (p-strPage >= nStringLength) return false;
                   if ((*p & 0xc0) != 0x80) return false;
                   continue;
              }
 
              // 0x80을 넘었는데 상위 비트가 110, 1110, 11110 중 하나가 아니면
              // UTF-8 문서가 아니다.
              return false;
          }
     }
    
     // 0x80 넘는 값이 하나도 없으면 ANSI로 취급한다.
     if (bFind == false) {
          return false;
     }
 
     // 0x80을 넘는 모든 값이 UTF-8의 조건을 만족하면 UTF-8문서이다.
     return true;
}

static TextReader::EncodingType CheckEncodingType(char* strPage, int nStringLength)
{
	BYTE btBuf[8];
	memset(btBuf, 0, 8);

	// BOM 정의
	BYTE btBOM_UnicodeBE[] = {0xFE, 0xFF};
	BYTE btBOM_UnicodeLE[] = {0xFF, 0xFE};
	BYTE btBOM_UTF8[] = {0xEF, 0xBB, 0xBF};

	// 3바이트 이상
	if (nStringLength < 3)
		return TextReader::UNKOWN;

	memcpy(btBuf, strPage, 3);

	if (memcmp(btBuf, btBOM_UnicodeLE, 2) == 0)			// Unicode Little Endian
		return TextReader::UNICODE_LE;
	else if (memcmp(btBuf, btBOM_UnicodeBE, 2) == 0)	// Unicode Big Endian
		return TextReader::UNICODE_BE;
	else if (memcmp(btBuf, btBOM_UTF8, 3) == 0)			// UTF-8
		return TextReader::UTF8_BOM;

	return AnalyzeFormatUtf8(strPage, nStringLength) ? TextReader::UTF8_NO_BOM : TextReader::ANSI;
}

TextReader::EncodingType TextReader::CheckEncodingFileType()
{
	int nFileSize = ::GetFileSize(m_hFile, 0);

	char* strPage = new char[nFileSize + 1];

	DWORD num;
	::ReadFile(m_hFile, strPage, nFileSize, &num, 0);
	strPage[num] = 0;

	::SetFilePointer((HANDLE)m_hFile, 0, NULL, (DWORD)FILE_BEGIN);

	TextReader::EncodingType nResult = CheckEncodingType(strPage, (int)num);
	delete [] strPage;

	return nResult;
}

bool TextReader::Open(wchar_t* path)
{
	DWORD dwShare = FILE_SHARE_READ;
	DWORD dwCreateMode = OPEN_EXISTING;
	DWORD dwMode = GENERIC_READ;

	m_hFile = ::CreateFileW(path,
						dwMode,
						dwShare,
						0,
						dwCreateMode,
						FILE_ATTRIBUTE_NORMAL,
						0);

	if (m_hFile == INVALID_HANDLE_VALUE) return false;

	m_encodingType = CheckEncodingFileType();
	ReadAll();

	return true;
}

bool TextReader::ReadAll()
{
	DWORD fileSize = ::GetFileSize(m_hFile, 0);
	if (fileSize == 0)
		return true;

	DWORD len;
	char* buf = new char[fileSize + 1];

	if (!::ReadFile(m_hFile, buf, fileSize, &len, 0))
		return false;

	buf[len] = 0;

	wchar_t* wstr = new wchar_t[fileSize + 1];
	memset(wstr, 0, fileSize + 1);

	ConversionString(buf, len, m_encodingType, wstr);

	std::wstring wstrData;
	wstrData.append(wstr);

	m_memFile.SetData(wstr);

	delete [] wstr;
	delete [] buf;
	
	return true;
}

/*bool TextReader::Seek(LONG lOff, UINT nFrom)
{
	DWORD dwNew = ::SetFilePointer((HANDLE)m_hFile, lOff, NULL, (DWORD)nFrom);
	if (dwNew  == (DWORD)-1) return false;

	return true;
}*/

void TextReader::Close()
{
	::CloseHandle(m_hFile);
	m_hFile = 0;
}

HANDLE TextReader::GetFileHandle()
{
	return m_hFile;
}

// srcEncodingType에 따라 strSrc를 strTrg으로 변환한다.
void TextReader::ConversionString(const char* strSrc, int nSrcLen, EncodingType srcEncodingType, wchar_t* strTrg)
{
	if (srcEncodingType == ANSI)
	{
		int nLen = MultiByteToWideChar(CP_ACP, 0, strSrc, nSrcLen, NULL, NULL);
		MultiByteToWideChar(CP_ACP, 0, strSrc, nSrcLen, strTrg, nLen);
		strTrg[nLen] = 0;
	}
	else if (srcEncodingType == UTF8_BOM || srcEncodingType == UTF8_NO_BOM)
	{
		int nLen = MultiByteToWideChar(CP_UTF8, 0, strSrc, nSrcLen, NULL, NULL);
		MultiByteToWideChar(CP_UTF8, 0, strSrc, nSrcLen, strTrg, nLen);
		strTrg[nLen] = 0;
	}
	else if (srcEncodingType == UNICODE_LE)
	{
		memcpy(strTrg, strSrc, nSrcLen);
		strTrg[nSrcLen / 2] = 0;
	}
	else if (srcEncodingType == UNICODE_BE)
	{
		BYTE* arrUnicode = (BYTE*)strTrg;

		for (int i=0, j=0;i<nSrcLen;i+=2)
		{
			arrUnicode[j++] = strSrc[i + 1];
			arrUnicode[j++] = strSrc[i];
		}

		strTrg[nSrcLen / 2] = 0;
	}
}

bool TextReader::ReadLine(wchar_t *buf, int nSizeToRead, int *pSizeOfBytesRead)
{
	bool isSuccess;
	std::wstring strLine = m_memFile.ReadLine(isSuccess);

	if (!isSuccess)
		return false;

	wcscpy_s(buf, nSizeToRead, strLine.c_str());
	*pSizeOfBytesRead = strLine.length();
	return true;
	/*int nLen = nSizeToRead * 2 + 1;
	char* strLine = new char[nLen];

	bool isSuccess = ReadLineA(strLine, nLen, pSizeOfBytesRead);

	if (!isSuccess)
	{
		delete [] strLine;
		return false;
	}

	// 문서의 EncodingType에 맞게 변환된다.
	ConversionString(strLine, *pSizeOfBytesRead, m_encodingType, buf);
	//MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, strLine, -1, buf, nSizeToRead);
	delete [] strLine;

	*pSizeOfBytesRead = wcslen(buf);

	return true;*/
}

/*bool TextReader::ReadLineA(char* buf, int nSizeToRead, int *pSizeOfBytesRead)
{
	// 파일의 끝인지 검사
	DWORD fileSize = ::GetFileSize(m_hFile,0);
	if (fileSize <= GetFilePointer(m_hFile)) return false;

	DWORD len;
	if (!::ReadFile(m_hFile,buf,nSizeToRead,&len,0)) return false;

	int size = (int)len;
	int i;
	bool findEnd = false;

	for (i=0;i<size;i++)
	{
		if (buf[i] == '\n')
		{
			i++;
			break;
		}
		else if (buf[i] == '\r')
		{
			buf[i] = 0;
			*pSizeOfBytesRead = i;
			findEnd = true;
		}
	}
	buf[i] = 0;

	Seek(i-len, FILE_CURRENT);

	if (!findEnd)
		*pSizeOfBytesRead = i;

	return true;
}*/

TextReader::EncodingType TextReader::GetEncodingType() const
{
	return m_encodingType;
}

END_NS
END_NS
