#include "stdafx.h"
#include "StringManager.h"
#include <string.h>
#include <math.h>

BEGIN_NS(UnE)
BEGIN_NS(Utility)

StringManager::StringManager(void)
{
}

StringManager::~StringManager(void)
{
}

// 문자열 앞, 뒤에 있는 빈 칸 및 탭 문자등을 모두 없앤다.
template <class T, class STRING>
static STRING _ParseString(const T* str)
{
	T strNull[1];
	strNull[0] = 0;

	if (str == 0) return strNull;

	int nBeginIndex = -1, nLastIndex = -1;

	for (int i=0;str[i];i++)
	{
		if (str[i] != ' ' && str[i] != '\t' && str[i] != '\r' && str[i] != '\n')
		{
			if (nBeginIndex < 0) nBeginIndex = i;
			nLastIndex = i;
		}
	}

	if (nBeginIndex < 0 || nLastIndex < nBeginIndex)
		return strNull;

	STRING strResult = &str[nBeginIndex];
	strResult.erase(strResult.begin() + (nLastIndex - nBeginIndex) + 1, strResult.end());
	return strResult;

	//str[nLastIndex+1] = 0;
	//return &str[nBeginIndex];

	/*int len = (int)strlen(str);
	int i;

	for (i=len-1;i>=0;i--)
	{
		if (str[i] != ' ' && str[i] != '\t' && str[i] != '\r' && str[i] != '\n' && str[i] != 0)
		{
			len = i + 1;
			str[len] = 0;
			break;
		}
	}

	if (i < 0) return "";

	for (i=0;i<len;i++)
	{
		if (str[i] != ' ' && str[i] != '\t' && str[i] != '\r' && str[i] != '\n' && str[i] != 0)
		{
			return &str[i];
		}
	}

	return "";*/
}

// 문자열 앞, 뒤에 있는 빈 칸 및 탭 문자등을 모두 없앤다.
std::string StringManager::ParseStringA(const char* str)
{
	return _ParseString<char, std::string>(str);
}

// 문자열 앞, 뒤에 있는 빈 칸 및 탭 문자등을 모두 없앤다.
std::wstring StringManager::ParseString(const wchar_t* str)
{
	return _ParseString<wchar_t, std::wstring>(str);
}

// strSrc의 첫번째 Token을 얻어온다.
// Return 값 : strSrc에서 첫번째 Token이 제거된 문자열
// strSrc을 이루는 첫번째 문자는 반드시 Token의 구성원이어야 한다.(빈문자, 탭문자등으로 시작하면 안된다.)
// 만일, 그렇지 않다면 ParseString을 통하여 앞부분의 빈칸들을 제거하여야 한다.
template <class T, class STRING>
static STRING _GetToken(const T* strSrc, T* strToken, T* NULL_STRING)
{
	int i;

	for (i=0;strSrc[i];i++)
	{
		if (strSrc[i] == ' ' || strSrc[i] == '\t' || strSrc[i] == '\r' || strSrc[i] == '\n')
		{
			int j;
			for (j=0;j<i;j++) strToken[j] = strSrc[j];
			strToken[j] = 0;
			return _ParseString<T, STRING>(&strSrc[i]);
		}
	}

	memcpy(strToken, strSrc, sizeof(T) * i);
	strToken[i] = 0;
	return NULL_STRING;
}

// strSrc의 첫번째 Token을 얻어온다.
// Return 값 : strSrc에서 첫번째 Token이 제거된 문자열
// strSrc을 이루는 첫번째 문자는 반드시 Token의 구성원이어야 한다.(빈문자, 탭문자등으로 시작하면 안된다.)
// 만일, 그렇지 않다면 ParseString을 통하여 앞부분의 빈칸들을 제거하여야 한다.
std::string StringManager::GetTokenA(const char* strSrc, char* strToken)
{
	return _GetToken<char, std::string>(strSrc, strToken, "");
}

std::wstring StringManager::GetToken(const wchar_t* strSrc, wchar_t* strToken)
{
	return _GetToken<wchar_t, std::wstring>(strSrc, strToken, L"");
}

template <class T>
static int StringLength(const T* str)
{
	if (str == 0) return 0;

	for (int i=0;;i++)
	{
		if (str[i] == 0) return i;
	}

	return 0;
}

template <class T>
static bool StrToUInt(const T* str, int nStringLength, unsigned int* pData, unsigned int nFrom, unsigned int nTo)
{
	if (nFrom >= nTo) return false;
	if (nTo > (unsigned int)nStringLength) return false;

	bool bPositive = true;
	unsigned int num = 0;

	if (nStringLength <= 0) return false;
	//else if (nStringLength == 1 && (str[0] < '0' || str[0] > '9')) return false;
	else if ((nTo - nFrom) == 0 && (str[nFrom] < '0' || str[nFrom] > '9')) return false;

	for (unsigned int i=nFrom;i<nTo;i++)
	{
		if (str[i] < '0' || str[i] > '9') 
		{
			if (i == 0)
			{
				if (str[i] == '-') bPositive = false;
				else if (str[i] == '+') bPositive = true;
				else return false;

				continue;
			}
			else return false;
		}

		num = num * 10 + str[i] - '0';
	}

	*pData = num;
	return true;
}

template <class T, class IntegerType>
static bool StrToInt(const T* str, int nStringLength, IntegerType* pData, unsigned int nFrom, unsigned int nTo)
{
	if (nFrom >= nTo) return false;
	if (nTo > (unsigned int)nStringLength) return false;

	bool bPositive = true;
	IntegerType num = 0;

	if (nStringLength <= 0) return false;
	//else if (nStringLength == 1 && (str[0] < '0' || str[0] > '9')) return false;
	else if ((nTo - nFrom) == 0 && (str[nFrom] < '0' || str[nFrom] > '9')) return false;

	for (unsigned int i=nFrom;i<nTo;i++)
	{
		if (str[i] < '0' || str[i] > '9') 
		{
			if (i == 0)
			{
				if (str[i] == '-') bPositive = false;
				else if (str[i] == '+') bPositive = true;
				else return false;

				continue;
			}
			else return false;
		}

		num = num * 10 + str[i] - '0';
	}

	if (bPositive) *pData = num;
	else *pData = -num;
	return true;
}

template <class T>
static bool StrToDouble(const T* str, int nStringLength, double* pData, unsigned int nFrom, unsigned int nTo)
{
	if (nFrom >= nTo) return false;
	if (nTo > (unsigned int)nStringLength) return false;

	double num = 0;
	bool bPositive = true;
	bool bPoint = false;
	double dPosData = 10.0;
	double dExponent = 1.0;

	if (nStringLength <= 0) return false;
	//else if (nStringLength == 1 && (str[0] < '0' || str[0] > '9')) return false;
	else if ((nTo - nFrom) == 0 && (str[nFrom] < '0' || str[nFrom] > '9')) return false;

	for (unsigned int i=nFrom;i<nTo;i++)
	{
		if (str[i] < '0' || str[i] > '9')
		{
			if (i == 0)
			{
				if (str[i] == '-') bPositive = false;
				else if (str[i] == '+') bPositive = true;
				else if (str[i] == '.') bPoint = true;
				else return false;
			}
			else if (str[i] == '.')
			{
				if (bPoint) return false;
				else bPoint = true;
			}
			else if (str[i] == 'e' || str[i] == 'E')
			{
				int nEx;
				int len = StringLength<T>(&str[i+1]);
				if (!StrToInt<T,int>(&str[i+1], len, &nEx, 0, (unsigned int)len)) return false;
				dExponent = pow(10.0,nEx);
				break;
			}
			else return false;
		}
		else
		{
			if (!bPoint)
			{
				num = num * 10 + str[i] - '0';
			}
			else
			{
				num = num + (str[i] - '0') / dPosData;
				dPosData *= 10.0;
			}
		}
	}

	if (bPositive) *pData = num * dExponent;
	else *pData = -num * dExponent;
	return true;
}

bool StringManager::StrToUIntA(const char* str, unsigned int* pData)
{
	if (str == 0) return false;
	int len = (int)strlen(str);
	return Utility::StrToUInt<char>(str, len, pData, 0, (unsigned int)len);
}

bool StringManager::StrToUIntA(const char* str, unsigned int* pData, unsigned int nFrom, unsigned int nTo)
{
	if (str == 0) return false;
	int len = (int)strlen(str);
	return Utility::StrToUInt<char>(str, len, pData, nFrom, nTo + 1);
}

bool StringManager::StrToIntA(const char* str, int* pData)
{
	if (str == 0) return false;
	int len = (int)strlen(str);
	return Utility::StrToInt<char,int>(str, len, pData, 0, (unsigned int)len);
}

bool StringManager::StrToIntA(const char* str, int* pData, unsigned int nFrom, unsigned int nTo)
{
	if (str == 0) return false;
	int len = (int)strlen(str);
	return Utility::StrToInt<char,int>(str, len, pData, nFrom, nTo + 1);
}

bool StringManager::StrToInt64A(const char* str, __int64* pData)
{
	if (str == 0) return false;
	int len = (int)strlen(str);
	return Utility::StrToInt<char,__int64>(str, len, pData, 0, (unsigned int)len);
}

bool StringManager::StrToInt64A(const char* str, __int64* pData, unsigned int nFrom, unsigned int nTo)
{
	if (str == 0) return false;
	int len = (int)strlen(str);
	return Utility::StrToInt<char,__int64>(str, len, pData, nFrom, nTo + 1);
}

bool StringManager::StrToDoubleA(const char* str, double* pData)
{
	if (str == 0) return false;
	int len = (int)strlen(str);
	return Utility::StrToDouble<char>(str, len, pData, 0, (unsigned int)len);
}

bool StringManager::StrToDoubleA(const char* str, double* pData, unsigned int nFrom, unsigned int nTo)
{
	if (str == 0) return false;
	int len = (int)strlen(str);
	return Utility::StrToDouble<char>(str, len, pData, nFrom, nTo);
}

template <class CHAR_TYPE, class INT_TYPE>
static bool _HexToInteger(const CHAR_TYPE* str, INT_TYPE* pData)
{
	if (str == 0) return false;

	INT_TYPE num = 0;

	for (int i=0;str[i];i++)
	{
		if (str[i] >= '0' && str[i] <= '9')
		{
			num = num * 16 + str[i] - '0';
		}
		else if (str[i] >= 'a' && str[i] <= 'f')
		{
			num = num * 16 + str[i] - 'a' + 10;
		}
		else if (str[i] >= 'A' && str[i] <= 'F')
		{
			num = num * 16 + str[i] - 'A' + 10;
		}
		else return false;
	}

	*pData = num;
	return true;
}

// 16진수 형태의 문자열을 int 값으로 바꾼다.
bool StringManager::HexToIntA(const char* str, int* pData)
{
	return _HexToInteger<char, int>(str, pData);
	/*if (str == 0) return false;

	int len = (int)strlen(str);
	int num = 0;

	for (int i=0;i<len;i++)
	{
		if (str[i] >= '0' && str[i] <= '9')
		{
			num = num * 16 + str[i] - '0';
		}
		else if (str[i] >= 'a' && str[i] <= 'f')
		{
			num = num * 16 + str[i] - 'a' + 10;
		}
		else if (str[i] >= 'A' && str[i] <= 'F')
		{
			num = num * 16 + str[i] - 'A' + 10;
		}
		else return false;
	}

	*pData = num;
	return true;*/
}

bool StringManager::HexToInt64A(const char* str, __int64* pData)
{
	return _HexToInteger<char, __int64>(str, pData);
	/*if (str == 0) return false;

	int len = (int)strlen(str);
	__int64 num = 0;

	for (int i=0;i<len;i++)
	{
		if (str[i] >= '0' && str[i] <= '9')
		{
			num = num * 16 + str[i] - '0';
		}
		else if (str[i] >= 'a' && str[i] <= 'f')
		{
			num = num * 16 + str[i] - 'a' + 10;
		}
		else if (str[i] >= 'A' && str[i] <= 'F')
		{
			num = num * 16 + str[i] - 'A' + 10;
		}
		else return false;
	}

	*pData = num;
	return true;*/
}

// 16진수 형태의 문자열을 int 값으로 바꾼다.
bool StringManager::HexToInt(const wchar_t* str, int* pData)
{
	return _HexToInteger<wchar_t, int>(str, pData);
}

bool StringManager::HexToInt64(const wchar_t* str, __int64* pData)
{
	return _HexToInteger<wchar_t, __int64>(str, pData);
}

bool StringManager::StrToUInt(const wchar_t* wstr, unsigned int* pData)
{
	if (wstr == 0) return false;
	int len = (int)wcslen(wstr);
	return Utility::StrToUInt<wchar_t>(wstr, len, pData, 0, (unsigned int)len);
}

bool StringManager::StrToUInt(const wchar_t* wstr, unsigned int* pData, unsigned int nFrom, unsigned int nTo)
{
	if (wstr == 0) return false;
	int len = (int)wcslen(wstr);
	return Utility::StrToUInt<wchar_t>(wstr, len, pData, nFrom, nTo);
}

bool StringManager::StrToInt(const wchar_t* wstr, int* pData)
{
	if (wstr == 0) return false;
	int len = (int)wcslen(wstr);
	return Utility::StrToInt<wchar_t,int>(wstr, len, pData, 0, (unsigned int)len);
}

bool StringManager::StrToInt(const wchar_t* wstr, int* pData, unsigned int nFrom, unsigned int nTo)
{
	if (wstr == 0) return false;
	int len = (int)wcslen(wstr);
	return Utility::StrToInt<wchar_t,int>(wstr, len, pData, nFrom, nTo);
}

bool StringManager::StrToInt64(const wchar_t* wstr, __int64* pData)
{
	if (wstr == 0) return false;
	int len = (int)wcslen(wstr);
	return Utility::StrToInt<wchar_t,__int64>(wstr, len, pData, 0, (unsigned int)len);
}

bool StringManager::StrToInt64(const wchar_t* wstr, __int64* pData, unsigned int nFrom, unsigned int nTo)
{
	if (wstr == 0) return false;
	int len = (int)wcslen(wstr);
	return Utility::StrToInt<wchar_t,__int64>(wstr, len, pData, nFrom, nTo);
}

bool StringManager::StrToDouble(const wchar_t* wstr, double* pData)
{
	if (wstr == 0) return false;
	int len = (int)wcslen(wstr);
	return Utility::StrToDouble<wchar_t>(wstr, len, pData, 0, (unsigned int)len);
}

bool StringManager::StrToDouble(const wchar_t* wstr, double* pData, unsigned int nFrom, unsigned int nTo)
{
	if (wstr == 0) return false;
	int len = (int)wcslen(wstr);
	return Utility::StrToDouble<wchar_t>(wstr, len, pData, nFrom, nTo);
}

END_NS
END_NS
