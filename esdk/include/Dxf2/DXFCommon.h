#pragma once

namespace DXF
{
	void AddLine(Utility::FileManager* pMgr, int nCode, wchar_t* strFormat, ...);
	//short Get16BitHandle();
	//int Get32BitHandle();
	
	// 주어진 Code가 어떠한 범위에 해당하는지를 알아낸다.
	// Return 값
	//   -1 : nCode 값이 범위를 벗어났다.
	//    0 : 10진수 형태의 정수값
	//    1 : 16진수 형태의 정수값
	//    2 : 실수값
	//    3 : 문자열
	int GetCodeRange(int nCode);
	
	// nCode와 pData가 주어졌을 경우 DXFData에 값을 할당한다.
	// Return 값
	//   -1 : nCode 값이 범위를 벗어났다.
	//    0 : pType1에 값이 할당되었으며, 10진수 형태의 정수값을 가진다.
	//    1 : pType1에 값이 할당되었으며, 16진수 형태의 정수값을 가진다.
	//    2 : pType1에 값이 할당되었으며, 실수값을 가진다.
	//    3 : pType2에 값이 할당되었다.
	int SetDXFData(int nCode, void* pData, struct _DXFData* pType);
	
	void WriteDXFData(Utility::FileManager* pMgr, struct _DXFData* pDXF);
}
