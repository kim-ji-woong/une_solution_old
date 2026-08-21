#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Attrib::Attrib(void)
{
	Init();
}

Attrib::~Attrib(void)
{
}

void Attrib::Init()
{
	m_strSubClassName = L"AcDbText";
	m_strEntityType	  = L"ATTRIB";
}

END_NS
END_NS
