#include "stdafx.h"


BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Body::Body(void)
{
	Init();
}

Body::~Body(void)
{
}

void Body::Init()
{
	m_strSubClassName = L"AcDbModelerGeometry";
	m_strEntityType	  = L"BODY";
}

END_NS
END_NS
