#include "stdafx.h"

BEGIN_NS(DXF)

SectionManager::SectionManager(void)
{
	m_pOwner = 0;
	m_bDeleted = false;
}

SectionManager::~SectionManager(void)
{
}

void SectionManager::SetOwner(DXFManager* pOwner)
{
	m_pOwner = pOwner;
}

DXFManager* SectionManager::GetOwner()
{
	return m_pOwner;
}

// Handle Code(5)가 정수가 아닌 문자열일 경우에도 읽을수 있는가?
bool SectionManager::ReadStringHandle()
{
	return false;
}

END_NS
