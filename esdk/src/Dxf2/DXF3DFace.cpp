#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

_3DFace::_3DFace(void)
{
	Init();
}

_3DFace::~_3DFace(void)
{
}

_3DFace::_3DFace(double dArrCoord1[3], double dArrCoord2[3], double dArrCoord3[3])
{
	Init();
	Set3DFace(dArrCoord1,dArrCoord2,dArrCoord3,dArrCoord3);
}

_3DFace::_3DFace(double dArrCoord1[3], double dArrCoord2[3], double dArrCoord3[3], double dArrCoord4[3])
{
	Init();
	Set3DFace(dArrCoord1,dArrCoord2,dArrCoord3,dArrCoord4);
}

void _3DFace::Init()
{
	m_strSubClassName = L"AcDbFace";
	m_strEntityType	  = L"3DFACE";
}

void _3DFace::Set3DFace(double dArrCoord1[3], double dArrCoord2[3], double dArrCoord3[3], double dArrCoord4[3])
{
	int nSize = sizeof(double) * 3;
	memcpy(m_dArrCoord[0],dArrCoord1,nSize);
	memcpy(m_dArrCoord[1],dArrCoord2,nSize);
	memcpy(m_dArrCoord[2],dArrCoord3,nSize);
	memcpy(m_dArrCoord[3],dArrCoord4,nSize);
}

END_NS
END_NS
