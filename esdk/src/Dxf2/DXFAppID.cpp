#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

AppID::Entity::Entity(AppID* pTable, wchar_t* strAppName)
{
	m_pParent = pTable;
	//m_nHandle = Get32BitHandle();
	wcscpy_s(m_strAppName, 256, strAppName);

	if (pTable != 0)
	{
		TableManager* pTblMgr = pTable->GetManager();

		if (pTblMgr != 0)
		{
			DXFManager* pDXFMgr = pTblMgr->GetOwner();

			if (pDXFMgr != 0)
			{
				m_nHandle = pDXFMgr->Get32BitHandle();
			}
		}
	}
}

void AppID::Entity::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"%s",(char*)m_pParent->m_strEntityName.data());
	AddLine(pMgr,5,L"%X",m_nHandle);
	AddLine(pMgr,330,L"%X",m_pParent->m_nHandle);
	AddLine(pMgr,100,L"%s",m_pParent->m_strDefSubClassName);
	AddLine(pMgr,100,L"%s",m_pParent->m_strSubClassName);
	AddLine(pMgr,2,L"%s",m_strAppName);
	AddLine(pMgr,70,L"0");
}

AppID::AppID(TableManager* pMgr)
	: Table(pMgr)
{
	Init();
}

AppID::~AppID(void)
{
}

void AppID::Init()
{
	if (m_pMgr != 0)
	{
		DXFManager* pDXFMgr = m_pMgr->GetOwner();

		if (pDXFMgr != 0)
		{
			m_nHandle = pDXFMgr->Get32BitHandle();
		}
	}

	//m_nHandle = Get32BitHandle();
	m_nSoftPointer = 0;
	m_strEntityName = L"APPID";
	m_strSubClassName = L"AcDbRegAppTableRecord";
	m_nEntitySize = 5;

	m_list.push_back(Entity(this,L"ACAD"));
	m_list.push_back(Entity(this, L"ACAD_PSEXT"));
	m_list.push_back(Entity(this, L"ACAD_EXEMPT_FROM_CAD_STANDARDS"));
	m_list.push_back(Entity(this, L"DCO15"));
	m_list.push_back(Entity(this, L"ADE"));
}

void AppID::Write(Utility::FileManager* pMgr)
{
	Table::Write(pMgr);

	std::list<Entity>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		Entity e = *p;
		e.Write(pMgr);
		++p;
	}

	AddLine(pMgr,0,L"ENDTAB");
}

END_NS
END_NS
