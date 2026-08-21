#include "Stdafx.h"

BEGIN_NS(DXFDotNet)

wchar_t* ToWcharArray(System::String^ str)
{
	if (str == nullptr)
		return 0;

	int nLen = str->Length;
	wchar_t* wstr = new wchar_t[nLen + 1];

	array<wchar_t>^ arr = str->ToCharArray();

	for (int i=0;i<nLen;i++)
		wstr[i] = arr[i];
	wstr[nLen] = 0;

	return wstr;
}

System::String^ ToSystemString(wchar_t* str)
{
	if (str == 0)
		return nullptr;

	System::String^ _str = gcnew System::String(L"");

	for (int i=0;str[i] != 0;i++)
	{
		_str += str[i];
	}

	return _str;
}

END_NS
