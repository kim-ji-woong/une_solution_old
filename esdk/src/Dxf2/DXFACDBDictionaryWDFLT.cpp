#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(OBJECTS)

ACDBDictionaryWDFLT::ACDBDictionaryWDFLT(ObjectManager* pMgr)
	: Dictionary(pMgr)
{
	Init();
}

ACDBDictionaryWDFLT::~ACDBDictionaryWDFLT(void)
{
}

void ACDBDictionaryWDFLT::Init()
{
	m_strEntityType = L"ACDBDICTIONARYWDFLT";
	m_list.clear();

	AddData(0,(wchar_t*)m_strEntityType.data());
	AddData(5,&m_nHandle);
}

END_NS
END_NS
