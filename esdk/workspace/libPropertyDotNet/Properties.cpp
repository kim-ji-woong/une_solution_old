// 기본 DLL 파일입니다.

#include "stdafx.h"
#include <map>
#include <utility>

#include <atlcoll.h>
using namespace ATL;
#include "Properties.h"

using namespace UnE::Utility;

using namespace System;
using namespace System::Collections;

UNE::Properties* inProperties = new UNE::Properties();
//////////////////////////////////////////////////////////////////////////
// Local function

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

int WideToMulti(char* pszDst, const wchar_t* pwzIn, UINT uCodepage)
{
	int nReqLen = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), NULL, 0, NULL, NULL);
	int nLen    = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), pszDst, nReqLen, NULL, NULL); 
	if(nLen)
		pszDst[nLen] = 0;
	return nLen;
} 

//////////////////////////////////////////////////////////////////////////


void UnE::Utility::Properties::SetProperty( System::String^ strKey, int nValue )
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strKey);
	WideToMulti(buf, t, CP_ACP);
	std::string szKey = std::string(buf);
	delete[] t;	

	inProperties->SetValue(szKey, nValue);
}

void UnE::Utility::Properties::SetProperty( System::String^ strKey, float nValue )
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strKey);
	WideToMulti(buf, t, CP_ACP);
	std::string szKey = std::string(buf);
	delete[] t;	

	inProperties->SetValue(szKey, nValue);
}

void UnE::Utility::Properties::SetProperty( System::String^ strKey, double nValue )
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strKey);
	WideToMulti(buf, t, CP_ACP);
	std::string szKey = std::string(buf);
	delete[] t;	

	inProperties->SetValue(szKey, nValue);
}

void UnE::Utility::Properties::SetProperty( System::String^ strKey, System::String^ nValue )
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strKey);
	WideToMulti(buf, t, CP_ACP);
	std::string szKey = std::string(buf);
	delete[] t;	

	wchar_t * t2 = ToWcharArray(nValue);
	WideToMulti(buf, t2, CP_ACP);
	std::string szValue = std::string(buf);
	delete[] t2;	

	inProperties->SetValue(szKey, szValue);
}

bool UnE::Utility::Properties::GetProperty( System::String^ strKey, System::String^ %value )
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strKey);
	WideToMulti(buf, t, CP_ACP);
	std::string szKey = std::string(buf);
	delete[] t;	

	std::string retValue = "";
	if( inProperties->GetValue(szKey, retValue))
	{
		USES_CONVERSION;
		value = ToSystemString(A2W(retValue.c_str()));
		return true;
	}
	return false;
}

bool UnE::Utility::Properties::GetProperty( System::String^ strKey, int %value )
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strKey);
	WideToMulti(buf, t, CP_ACP);
	std::string szKey = std::string(buf);
	delete[] t;	

	int retValue = 0;
	if( inProperties->GetValue(szKey, retValue))
	{
		value = retValue;
		return true;
	}
	return false;
}

bool UnE::Utility::Properties::GetProperty( System::String^ strKey, float %value )
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strKey);
	WideToMulti(buf, t, CP_ACP);
	std::string szKey = std::string(buf);
	delete[] t;	

	float retValue = 0;
	if( inProperties->GetValue(szKey, retValue))
	{
		value = retValue;
		return true;
	}
	return false;
}

bool UnE::Utility::Properties::GetProperty( System::String^ strKey, double %value )
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strKey);
	WideToMulti(buf, t, CP_ACP);
	std::string szKey = std::string(buf);
	delete[] t;	

	double retValue = 0;
	if( inProperties->GetValue(szKey, retValue))
	{
		value = retValue;
		return true;
	}
	return false;
}
