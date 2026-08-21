#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

BlockRecord::Entity::Entity(BlockRecord* pTable, wchar_t* strAppName, int nBlockHandle, int nLayoutHandle, ArrowType type)
{
	m_pParent = pTable;
	wcscpy_s(m_strAppName,strAppName);
	m_nBlockHandle = nBlockHandle;
	m_nLayoutHandle = nLayoutHandle;
	m_arrowType = type;
}

void BlockRecord::Entity::Write(Utility::FileManager* pMgr) const
{
	AddLine(pMgr,0,L"%s",(wchar_t*)m_pParent->m_strEntityName.data());
	AddLine(pMgr,5,L"%X",m_nBlockHandle);
	AddLine(pMgr,330,L"%X",m_pParent->m_nHandle);
	AddLine(pMgr,100,L"%s",m_pParent->m_strDefSubClassName);
	AddLine(pMgr,100,L"%s",m_pParent->m_strSubClassName);
	AddLine(pMgr,2,L"%s",m_strAppName);
	AddLine(pMgr,340,L"%X",m_nLayoutHandle);
}

void BlockRecord::Entity::SetArrowType(ArrowType type)
{
	m_arrowType = type;
}

ArrowType BlockRecord::Entity::GetArrowType() const
{
	return m_arrowType;
}

int BlockRecord::Entity::GetBlockHandle() const
{
	return m_nBlockHandle;
}

const wchar_t* BlockRecord::Entity::GetAppName() const
{
	return m_strAppName;
}

BlockRecord::BlockRecord(BLOCKS::BlockManager* pBlkMgr, OBJECTS::ObjectManager* pObjMgr, TableManager* pMgr)
	: Table(pMgr)
{
	m_pBlkMgr = pBlkMgr;
	m_pObjMgr = pObjMgr;
	Init();
}

BlockRecord::~BlockRecord(void)
{
}

void BlockRecord::Init()
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
	m_strEntityName = L"BLOCK_RECORD";
	m_strDefSubClassName = L"AcDbSymbolTableRecord";
	m_strSubClassName = L"AcDbBlockTableRecord";

	int nBlockHandle, nLayoutHandle;
	wchar_t strBlockName[256];

	while (m_pBlkMgr->GetBlockInfo(strBlockName,&nBlockHandle,m_nEntitySize))
	{
		m_nEntitySize++;
		nLayoutHandle = m_pObjMgr->GetLayoutHandle(nBlockHandle);

		ArrowType type = FILL_TRIANGLE;
		if (!wcscmp(strBlockName,L"_DotSmall")) type = SMALL_DOT;
		else if (!wcscmp(strBlockName,L"_Oblique")) type = SLASH;
		else if (!wcscmp(strBlockName,L"_ClosedBlank")) type = TRIANGLE;
		else if (!wcscmp(strBlockName,L"_Open90")) type = TWO_LINE;
		else if (!wcscmp(strBlockName,L"_Small")) type = CIRCLE_ARROW;
		else if (!wcscmp(strBlockName,L"_None")) type = NONE;

		m_list.push_back(Entity(this,strBlockName,nBlockHandle,nLayoutHandle,type));
	}
}

void BlockRecord::AddEntity(const Entity& rEntity)
{
	m_list.push_back(rEntity);
	m_nEntitySize = (int)m_list.size();
}

void BlockRecord::Write(Utility::FileManager* pMgr)
{
	Table::Write(pMgr);

	std::list<Entity>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		const Entity& e = *p;
		e.Write(pMgr);
		++p;
	}

	AddLine(pMgr,0,L"ENDTAB");
}

int BlockRecord::GetBlockRecordHandle(ArrowType type)
{
	std::list<Entity>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		const Entity& e = *p;
		if (e.GetArrowType() == type) return e.GetBlockHandle();
		++p;
	}

	return 0;
}

bool BlockRecord::GetBlockHandle(const wchar_t* strAppName, int* pBlockHandle)
{
	std::list<Entity>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		const Entity& e = *p;
		if (!wcscmp(strAppName,e.GetAppName()))
		{
			*pBlockHandle = e.GetBlockHandle();
			return true;
		}
		++p;
	}

	return false;
}

END_NS
END_NS
