#include "stdafx.h"
#include "FileManager.h"
#include <stdio.h>

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

#define GetFilePointer(hFile) SetFilePointer(hFile, 0, NULL, FILE_CURRENT)

BEGIN_NS(UnE)
BEGIN_NS(Utility)

FileManager::FileManager()
{
	m_encodingType = UNKOWN;
}

FileManager::~FileManager()
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

static FileManager::EncodingType CheckEncodingType(char* strPage, int nStringLength)
{
	BYTE btBuf[8];
	memset(btBuf, 0, 8);

	// BOM 정의
	BYTE btBOM_UnicodeBE[] = {0xFE, 0xFF};
	BYTE btBOM_UnicodeLE[] = {0xFF, 0xFE};
	BYTE btBOM_UTF8[] = {0xEF, 0xBB, 0xBF};

	// 3바이트 이상
	if (nStringLength < 3)
		return FileManager::UNKOWN;

	memcpy(btBuf, strPage, 3);

	if (memcmp(btBuf, btBOM_UnicodeLE, 2) == 0)			// Unicode Little Endian
		return FileManager::UNICODE_LE;
	else if (memcmp(btBuf, btBOM_UnicodeBE, 2) == 0)	// Unicode Big Endian
		return FileManager::UNICODE_BE;
	else if (memcmp(btBuf, btBOM_UTF8, 3) == 0)			// UTF-8
		return FileManager::UTF8_BOM;

	return AnalyzeFormatUtf8(strPage, nStringLength) ? FileManager::UTF8_NO_BOM : FileManager::ANSI;
}

static FileManager::EncodingType CheckEncodingFileType(char* strPath)
{
	FILE* fp = 0;
	fopen_s(&fp, strPath, "rb");

	if (fp == 0)
		return FileManager::UNKOWN;

	fseek(fp, 0, SEEK_END);
	long nFileSize = ftell(fp);
	fseek(fp, 0, SEEK_SET);

	char* strPage = new char[nFileSize + 1];
	fread(strPage, 1, nFileSize, fp);
	fclose(fp);

	strPage[nFileSize] = 0;

	FileManager::EncodingType nResult = CheckEncodingType(strPage, nFileSize);
	delete [] strPage;

	return nResult;
}

bool FileManager::Open(wchar_t* path, DWORD dwMode)
{
	int nWLen = wcslen(path);
	int nLen = nWLen * 2 + 1;
	char* strPath = new char[nLen];

	BOOL bUsedDefaultChar;	
	WideCharToMultiByte(CP_ACP, 0, path, -1, strPath, nLen, 0, &bUsedDefaultChar);

	bool isSuccess = OpenA(strPath, dwMode);
	delete [] strPath;
	return isSuccess;
}

bool FileManager::OpenA(char* path, DWORD dwMode)
{
	DWORD dwShare = 0, dwCreateMode = 0;
	if ((dwMode&GENERIC_READ) == GENERIC_READ) 
	{
		dwShare |= FILE_SHARE_READ;
		dwCreateMode = OPEN_EXISTING;
	}
	if ((dwMode&GENERIC_WRITE) == GENERIC_WRITE) 
	{		
		dwShare |= FILE_SHARE_WRITE;
		if (dwCreateMode == 0) dwCreateMode = CREATE_ALWAYS;
		else dwCreateMode = OPEN_ALWAYS;
	}

	m_hFile = ::CreateFileA(path,
						dwMode,
						dwShare,
						0,
						dwCreateMode,
						FILE_ATTRIBUTE_NORMAL,
						0);

	if (m_hFile == INVALID_HANDLE_VALUE) return false;

	if ((dwMode&GENERIC_READ) == GENERIC_READ)
	{
		m_encodingType = CheckEncodingFileType(path);

		// File Header는 읽을 필요없음
		if (m_encodingType == FileManager::UNICODE_LE ||
			m_encodingType == FileManager::UNICODE_BE)
			Seek(2);
		else if (m_encodingType == FileManager::UTF8_BOM)
			Seek(3);
	}

	return true;
}

bool FileManager::Seek(LONG lOff, UINT nFrom)
{
	DWORD dwNew = ::SetFilePointer((HANDLE)m_hFile, lOff, NULL, (DWORD)nFrom);
	if (dwNew  == (DWORD)-1) return false;

	return true;
}

void FileManager::Close()
{
	::CloseHandle(m_hFile);
	m_hFile = 0;
}

BYTE* FileManager::Read(int nByte)
{
	BYTE* str = new BYTE[nByte+1];
	DWORD num;
	::ReadFile(m_hFile,str,nByte,&num,0);
	str[num] = 0;

	return str;
}

bool FileManager::Write(wchar_t *wstr, LONG lOff, UINT nFrom, int nMode)
{
	int nWLen = wcslen(wstr);
	int nLen = nWLen * 2 + 1;
	char* str = new char[nLen];

	BOOL bUsedDefaultChar;	
	WideCharToMultiByte(CP_ACP, 0, wstr, -1, str, nLen, 0, &bUsedDefaultChar);

	bool isSuccess = WriteA(str, lOff, nFrom, nMode);
	delete [] str;
	return isSuccess;
}

bool FileManager::WriteA(char *str, LONG lOff, UINT nFrom, int nMode)
{
	if (nMode == WRITE_REPLACE)
	{
		if (Seek(lOff,nFrom))
		{
			DWORD num;
			BOOL flag = ::WriteFile(m_hFile,str,(DWORD)strlen(str),&num,0);
			if (flag) return true;
		}
	}
	else if (nMode == WRITE_INSERT)
	{
		DWORD len = ::GetFileSize(m_hFile,NULL);
		if (len == INVALID_FILE_SIZE) return false;
		if ((LONG)len <= lOff) return false;

		char* buffer = new char[len-lOff+1];
		DWORD num;

		if (Seek(lOff,nFrom))
		{
			if (::ReadFile(m_hFile,buffer,len-lOff,&num,0) == FALSE) goto FAILURE;
			buffer[len-lOff] = 0;
		}
		else return false;

		Seek(lOff,nFrom);
		if (::WriteFile(m_hFile,str,(DWORD)strlen(str),&num,0) == FALSE) goto FAILURE;
		if (::WriteFile(m_hFile,buffer,len-lOff,&num,0) == FALSE) goto FAILURE;

		delete [] buffer;
		return true;

FAILURE:
		delete [] buffer;
		return false;
	}

	return false;
}

HANDLE FileManager::GetFileHandle()
{
	return m_hFile;
}

/*
bool FileManager::ReadLine(char *buf, int nSizeToRead, int* pSizeOfBytesRead, char delim)
{
	DWORD lengthToRead = 1, lengthRead;
	char temp[2];
	int count = 0;

	for (int i=0;i<nSizeToRead;i++)
	{
		if (!::ReadFile(m_hFile,temp,lengthToRead,&lengthRead,0)) return false;
		buf[count++] = temp[0];
		if (temp[0] == delim) break;
	}

	buf[count] = 0;
	*pSizeOfBytesRead = count;
	return true;
}*/

// m_encodingType에 따라 strSrc를 strTrg으로 변환한다.
void FileManager::ConversionString(const char* strSrc, int nSrcLen, wchar_t* strTrg)
{
	if (m_encodingType == ANSI)
	{
		int nLen = MultiByteToWideChar(CP_ACP, 0, strSrc, nSrcLen, NULL, NULL);
		MultiByteToWideChar(CP_ACP, 0, strSrc, nSrcLen, strTrg, nLen);
		strTrg[nLen] = 0;
	}
	else if (m_encodingType == UTF8_BOM || m_encodingType == UTF8_NO_BOM)
	{
		int nLen = MultiByteToWideChar(CP_UTF8, 0, strSrc, nSrcLen, NULL, NULL);
		MultiByteToWideChar(CP_UTF8, 0, strSrc, nSrcLen, strTrg, nLen);
		strTrg[nLen] = 0;
	}
	else if (m_encodingType == UNICODE_LE)
	{
		memcpy(strTrg, strSrc, nSrcLen);
		strTrg[nSrcLen / 2] = 0;
	}
	else if (m_encodingType == UNICODE_BE)
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

bool FileManager::ReadLine(wchar_t *buf, int nSizeToRead, int *pSizeOfBytesRead)
{
	int nLen = nSizeToRead * 2 + 1;
	char* strLine = new char[nLen];

	bool isSuccess = ReadLineA(strLine, nLen, pSizeOfBytesRead);

	if (!isSuccess)
	{
		delete [] strLine;
		return false;
	}

	// 문서의 EncodingType에 맞게 변환된다.
	if (strLine[0] == 0)
		ConversionString(&strLine[1], *pSizeOfBytesRead, buf);
	else
		ConversionString(strLine, *pSizeOfBytesRead, buf);

	//MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, strLine, -1, buf, nSizeToRead);
	delete [] strLine;
	*pSizeOfBytesRead = wcslen(buf);

	return true;
}

bool FileManager::ReadLineA(char *buf, int nSizeToRead, int *pSizeOfBytesRead, char delim)
{
	// 파일의 끝인지 검사
	DWORD fileSize = ::GetFileSize(m_hFile,0);
	if (fileSize <= GetFilePointer(m_hFile)) return false;

	DWORD len;
	if (!::ReadFile(m_hFile,buf,nSizeToRead,&len,0)) return false;
	int size = (int)len;
	int i;

	for (i=0;i<size;i++)
	{
		if (buf[i] == delim)
		{
			i++;
			break;
		}
	}
	buf[i] = 0;

	Seek(i-len,FILE_CURRENT);
	*pSizeOfBytesRead = i;

	return true;
}

FileManager::EncodingType FileManager::GetEncodingType() const
{
	return m_encodingType;
}

END_NS
END_NS
