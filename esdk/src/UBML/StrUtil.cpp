#include "stdafx.h"
#include "StrUtil.h"

#include <windows.h>
#include <string.h>



int UnicodeToMbcs(char* pszDst, const wchar_t* pwzIn, unsigned int uCodepage)
{
	int nReqLen = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), NULL, 0, NULL, NULL);
	int nLen    = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), pszDst, nReqLen, NULL, NULL); 
	if(nLen)
		pszDst[nLen] = 0;
	return nLen;
} 

int MbcsToUnicode(wchar_t* pwzDst, const char* pszIn, unsigned int uCodepage)
{ 
	int nReqLen = (int)MultiByteToWideChar(uCodepage, 0, pszIn, (int)strlen(pszIn), 0, 0 ); 
	int nLen    = (int)MultiByteToWideChar(uCodepage, 0, pszIn, (int)strlen(pszIn), pwzDst, nReqLen );
	if(nLen)
		pwzDst[nLen] = 0;
	return nLen; 
}


int UTF8toAscii(char* pszAcp,  const char* pszUTF8)
{
	wchar_t wzStr[MAX_PATH];
	if(strlen(pszUTF8)>0) {
		MbcsToUnicode(wzStr, pszUTF8, CP_UTF8);	
		return UnicodeToMbcs(pszAcp, wzStr, CP_ACP);
	}
	pszAcp[0] = '\0';
	return 0;
}


int AsciiToUTF8(char* pszUTF8, const char* pszAcp)
{
	wchar_t wzStr[MAX_PATH];
	if(strlen(pszAcp)>0) {
		MbcsToUnicode(wzStr, pszAcp, CP_ACP);	
		return UnicodeToMbcs(pszUTF8, wzStr, CP_UTF8);
	}

	pszUTF8[0] = '\0';
	return 0;
}

int UnicodeToUTF8(char* pszUTF8, const wchar_t* pszUnicode)
{
	if(wcslen(pszUnicode)>0) {
		return UnicodeToMbcs(pszUTF8, pszUnicode, CP_UTF8);
	}
	pszUTF8[0] = '\0';
	return 0;
}
