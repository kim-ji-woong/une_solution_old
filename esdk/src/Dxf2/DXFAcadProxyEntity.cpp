#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

AcadProxyEntity::AcadProxyEntity(void)
{
	Init();
}

AcadProxyEntity::~AcadProxyEntity(void)
{
}

void AcadProxyEntity::Init()
{
	m_bNotSupported = true;
	m_strSubClassName = L"AcDbProxyEntity";
	m_strEntityType	  = L"ACAD_PROXY_ENTITY";
}

END_NS
END_NS
