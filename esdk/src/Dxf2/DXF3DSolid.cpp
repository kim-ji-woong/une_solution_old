#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

_3DSolid::_3DSolid(void)
{
	Init();
}

_3DSolid::~_3DSolid(void)
{
}

void _3DSolid::Init()
{
	m_strSubClassName = L"AcDbModelerGeometry";
	m_strEntityType	  = L"3DSOLID";
}

END_NS
END_NS
