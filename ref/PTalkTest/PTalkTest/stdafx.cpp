
// stdafx.cpp : 표준 포함 파일만 들어 있는 소스 파일입니다.
// PTalkTest.pch는 미리 컴파일된 헤더가 됩니다.
// stdafx.obj에는 미리 컴파일된 형식 정보가 포함됩니다.

#include "stdafx.h"


wchar_t* ToWcharArray(System::String^ str)
{
	if (str == nullptr)
		return 0;

	int nLen = str->Length;
	wchar_t* wstr = new wchar_t[nLen + 1];

	array<wchar_t>^ arr = str->ToCharArray();

	for (int i = 0; i<nLen; i++)
		wstr[i] = arr[i];
	wstr[nLen] = 0;

	return wstr;
}

System::String^ ToSystemString(wchar_t* str)
{
	if (str == 0)
		return nullptr;

	System::String^ _str = gcnew System::String(L"");

	for (int i = 0; str[i] != 0; i++)
	{
		_str += str[i];
	}

	return _str;
}

int WideToMulti(char* pszDst, const wchar_t* pwzIn, UINT uCodepage)
{
	int nReqLen = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), NULL, 0, NULL, NULL);
	int nLen = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), pszDst, nReqLen, NULL, NULL);
	if (nLen)
		pszDst[nLen] = 0;
	return nLen;
}