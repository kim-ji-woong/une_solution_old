#ifndef StrUtil_h__
#define StrUtil_h__

#pragma once

int __declspec(dllexport) WideToMulti(char* pszDst, const wchar_t* pwzIn, unsigned int uCodepage);
int __declspec(dllexport) MultiToWide(wchar_t* pwzDst, const char* pszIn, unsigned int uCodepage);

int __declspec(dllexport) UTF8toACP(char* pszAcp,  const char* pszUTF8);
int __declspec(dllexport) ACPtoUTF8(char* pszUTF8, const char* pszAcp);

int __declspec(dllexport) UnicodeToUTF8(char* pszUTF8, const wchar_t* pszUnicode);

#endif // StrUtil_h__