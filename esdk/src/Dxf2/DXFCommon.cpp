#include "stdafx.h"

BEGIN_NS(DXF)

void AddLine(Utility::FileManager* pMgr, int nCode, wchar_t* strFormat, ...)
{
	wchar_t str[3072];
	swprintf_s(str,L"%d\r\n",nCode);

	va_list marker;
	va_start(marker,strFormat);

	int nElementSize = 3072 - (int)wcslen(str);
	
	vswprintf_s(&str[(int)wcslen(str)], nElementSize, strFormat, marker);
	va_end(marker);

	nElementSize = 3072 - (int)wcslen(str);

	swprintf_s(&str[(int)wcslen(str)], nElementSize,L"\r\n");
	pMgr->Write(str,0,FILE_CURRENT);
}

/*short Get16BitHandle()
{
	static short nHandle = 1;
	return nHandle++;
}

int Get32BitHandle()
{
	static int nHandle = 1;// << 16;
	return nHandle++;
}*/

// 주어진 Code가 어떠한 범위에 해당하는지를 알아낸다.
// Return 값
//   -1 : nCode 값이 범위를 벗어났다.
//    0 : 10진수 형태의 정수값
//    1 : 16진수 형태의 정수값
//    2 : 실수값
//    3 : 문자열
int GetCodeRange(int nCode)
{
	if (nCode >= 0 && nCode <= 4) return 3;
	else if (nCode == 5) return 1;
	else if (nCode >= 6 && nCode <= 9) return 3;
	else if (nCode >= 10 && nCode <= 59) return 2;
	else if (nCode >= 60 && nCode <= 99) return 0;
	else if (nCode == 100 || nCode == 102) return 3;
	else if (nCode == 105) return 1;
	else if (nCode >= 110 && nCode <= 149) return 2;
	else if (nCode >= 170 && nCode <= 179) return 0;
	else if (nCode >= 210 && nCode <= 239) return 2;
	else if (nCode >= 270 && nCode <= 299) return 0;
	else if (nCode >= 300 && nCode <= 309) return 3;
	else if (nCode >= 310 && nCode <= 369) return 1;
	else if (nCode >= 370 && nCode <= 389) return 0;
	else if (nCode >= 390 && nCode <= 399) return 1;
	else if (nCode >= 400 && nCode <= 409) return 0;
	else if (nCode >= 410 && nCode <= 419) return 3;
	else if (nCode >= 420 && nCode <= 429) return 0;
	else if (nCode >= 430 && nCode <= 439) return 3;
	else if (nCode >= 440 && nCode <= 459) return 0;
	else if (nCode >= 460 && nCode <= 469) return 2;
	else if (nCode >= 470 && nCode <= 479) return 3;
	else if (nCode == 999) return 3;
	else if (nCode >= 1000 && nCode <= 1009) return 3;
	else if (nCode >= 1010 && nCode <= 1059) return 2;
	else if (nCode >= 1060 && nCode <= 1071) return 0;

	return -1;
}

// nCode와 pData가 주어졌을 경우 DXFData에 값을 할당한다.
// Return 값
//   -1 : nCode 값이 범위를 벗어났다.
//    0 : pType1에 값이 할당되었으며, 10진수 형태의 정수값을 가진다.
//    1 : pType1에 값이 할당되었으며, 16진수 형태의 정수값을 가진다.
//    2 : pType1에 값이 할당되었으며, 실수값을 가진다.
//    3 : pType2에 값이 할당되었다.
int SetDXFData(int nCode, void* pData, struct _DXFData* pType)
{
	int nResult = GetCodeRange(nCode);

	if (nResult == 0 || nResult == 1)
	{
		pType->nCode = nCode;
		pType->nData = *(int*)pData;
	}
	else if (nResult == 2)
	{
		pType->nCode = nCode;
		pType->dData = *(double*)pData;
	}
	else if (nResult == 3)
	{
		pType->nCode = nCode;
		pType->str = (wchar_t*)pData;
	}
	
	return nResult;
}

void WriteDXFData(Utility::FileManager* pMgr, struct _DXFData* pDXF)
{
	wchar_t str[3072];
	swprintf_s(str,L"%d\r\n",pDXF->nCode);

	int nResult = GetCodeRange(pDXF->nCode);
	int nElementSize = 3072 - (int)wcslen(str);

	if (nResult == 0)
	{
		swprintf_s(&str[(int)wcslen(str)], nElementSize, L"%d\r\n", pDXF->nData);
	}
	else if (nResult == 1)
	{
		swprintf_s(&str[(int)wcslen(str)], nElementSize, L"%X\r\n", pDXF->nData);
	}
	else if (nResult == 2)
	{
		swprintf_s(&str[(int)wcslen(str)], nElementSize, L"%lf\r\n", pDXF->dData);
	}
	else if (nResult == 3)
	{
		swprintf_s(&str[(int)wcslen(str)], nElementSize, L"%s\r\n", pDXF->str.data());
	}
	else return;

	pMgr->Write(str,0,FILE_CURRENT);
}

END_NS
