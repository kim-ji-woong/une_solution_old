#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

Table::Table(TableManager* pMgr)
{
	m_pMgr = pMgr;
	m_strEntityName = L"";
	m_nEntitySize   = 0;
	m_strDefSubClassName = L"AcDbSymbolTableRecord";
}

Table::~Table(void)
{
}

void Table::Write(Utility::FileManager* pMgr)
{
	wchar_t strDefault[256];
	swprintf_s(strDefault, 256,L"0\r\nTABLE\r\n2\r\n%s\r\n5\r\n%X\r\n330\r\n%X\r\n100\r\nAcDbSymbolTable\r\n70\r\n%d\r\n",
		(wchar_t*)m_strEntityName.data(),m_nHandle,m_nSoftPointer,m_nEntitySize);
	pMgr->Write(strDefault,0,FILE_CURRENT);
}

int Table::GetHandle()
{
	return m_nHandle;
}

wchar_t* Table::GetEntityName()
{
	return (wchar_t*)m_strEntityName.data();
}

TableManager* Table::GetManager()
{
	return m_pMgr;
}

END_NS
END_NS
