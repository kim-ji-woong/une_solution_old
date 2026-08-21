// stdafx.h : 자주 사용하지만 자주 변경되지는 않는
// 표준 시스템 포함 파일 및 프로젝트 관련 포함 파일이
// 들어 있는 포함 파일입니다.

#pragma once


#ifndef BEGIN_NS
	#define BEGIN_NS(ns) namespace ns {
	#define END_NS }
#endif

#include "Common.h"

/*#define _CRT_TERMINATE_DEFINED
#define RC_INVOKED

typedef unsigned char BYTE;
struct HWND__
{
	int unused; 
};

typedef struct HWND__ *HWND;

struct HDC__
{
	int unused;
};

typedef struct HDC__ *HDC;

struct HGLRC__
{
	int unused;
};

typedef struct HGLRC__ *HGLRC;
typedef char *LPSTR, *LPCSTR;
typedef wchar_t *LPWSTR, *LPCWSTR;*/