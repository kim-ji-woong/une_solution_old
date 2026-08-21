#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

AttDef::AttDef(void)
{
	Init();
}

AttDef::~AttDef(void)
{
}

void AttDef::Init()
{
	m_strSubClassName = L"AcDbText";
	m_strEntityType	  = L"ATTDEF";
}

END_NS
END_NS
