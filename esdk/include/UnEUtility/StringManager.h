#pragma once

#include <string>

#ifdef StrToInt
#undef StrToInt
#endif

namespace UnE
{
	namespace Utility
	{
		class StringManager
		{
		public:
			StringManager(void);
			virtual ~StringManager(void);

		public:
			// 문자열 앞, 뒤에 있는 빈 칸 및 탭 문자등을 모두 없앤다.
			static std::string ParseStringA(const char* str);
			static std::wstring ParseString(const wchar_t* str);
			// strSrc의 첫번째 Token을 얻어온다.
			// Return 값 : strSrc에서 첫번째 Token이 제거된 문자열
			// strSrc을 이루는 첫번째 문자는 반드시 Token의 구성원이어야 한다.(빈문자, 탭문자등으로 시작하면 안된다.)
			// 만일, 그렇지 않다면 ParseString을 통하여 앞부분의 빈칸들을 제거하여야 한다.
			static std::string GetTokenA(const char* strSrc, char* strToken);
			static std::wstring GetToken(const wchar_t* strSrc, wchar_t* strToken);
			static bool StrToUIntA(const char* str, unsigned int* pData);
			static bool StrToUIntA(const char* str, unsigned int* pData, unsigned int nFrom, unsigned int nTo);
			static bool StrToIntA(const char* str, int* pData);
			static bool StrToIntA(const char* str, int* pData, unsigned int nFrom, unsigned int nTo);
			static bool StrToInt64A(const char* str, __int64* pData);
			static bool StrToInt64A(const char* str, __int64* pData, unsigned int nFrom, unsigned int nTo);
			static bool StrToDoubleA(const char* str, double* pData);
			static bool StrToDoubleA(const char* str, double* pData, unsigned int nFrom, unsigned int nTo);
			static bool StrToUInt(const wchar_t* wstr, unsigned int* pData);
			static bool StrToUInt(const wchar_t* wstr, unsigned int* pData, unsigned int nFrom, unsigned int nTo);
			static bool StrToInt(const wchar_t* wstr, int* pData);
			static bool StrToInt(const wchar_t* wstr, int* pData, unsigned int nFrom, unsigned int nTo);
			static bool StrToInt64(const wchar_t* wstr, __int64* pData);
			static bool StrToInt64(const wchar_t* wstr, __int64* pData, unsigned int nFrom, unsigned int nTo);
			static bool StrToDouble(const wchar_t* wstr, double* pData);
			static bool StrToDouble(const wchar_t* wstr, double* pData, unsigned int nFrom, unsigned int nTo);
			// 16진수 형태의 문자열을 int 값으로 바꾼다.
			static bool HexToIntA(const char* str, int* pData);
			static bool HexToInt64A(const char* str, __int64* pData);
			static bool HexToInt(const wchar_t* str, int* pData);
			static bool HexToInt64(const wchar_t* str, __int64* pData);
		};
	}
}
