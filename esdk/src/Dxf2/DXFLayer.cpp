#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

Layer::Entity::Entity(int nHandle1, int nHandle2)
{
	m_pParent = 0;
	m_strLayerName  = L"";
	m_nHandle = nHandle1;
	//m_nHandle = Get32BitHandle();
	m_nFlag = 0;
	m_nColor = 7;
	m_strLineType = L"Continuous";
	m_nLineWeight = -3;
	m_nHardPointer = nHandle2;
	//m_nHardPointer = Get32BitHandle();
}

Layer::Entity::Entity(Layer* pTable, wchar_t* strLayerName)
{
	m_nHandle = m_nHardPointer = 0;

	if (pTable != 0)
	{
		TableManager* pTblMgr = pTable->GetManager();

		if (pTblMgr != 0)
		{
			DXFManager* pDXFMgr = pTblMgr->GetOwner();

			if (pDXFMgr != 0)
			{
				m_nHandle = pDXFMgr->Get32BitHandle();
				m_nHardPointer = pDXFMgr->Get32BitHandle();
			}
		}
	}

	m_pParent = pTable;
	m_strLayerName = strLayerName;
	//m_nHandle = Get32BitHandle();
	m_nFlag = 0;
	m_nColor = 7;
	m_strLineType = L"Continuous";
	m_nLineWeight = -3;
	//m_nHardPointer = Get32BitHandle();
}

void Layer::Entity::SetLayerName(wchar_t* strLayerName)
{
	m_strLayerName = strLayerName;
}

void Layer::Entity::SetOwner(Layer* pTable)
{
	m_pParent = pTable;
}

void Layer::Entity::SetColor(int nColor)
{
	m_nColor = nColor;
}

void Layer::Entity::SetLineType(wchar_t* strLineType)
{
	m_strLineType = strLineType;
}

void Layer::Entity::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"%s",(wchar_t*)m_pParent->m_strEntityName.data());
	AddLine(pMgr,5,L"%X",m_nHandle);
	AddLine(pMgr,330,L"%X",m_pParent->m_nHandle);
	AddLine(pMgr,100,L"%s",m_pParent->m_strDefSubClassName);
	AddLine(pMgr,100,L"%s",m_pParent->m_strSubClassName);
	AddLine(pMgr,2,L"%s",(wchar_t*)m_strLayerName.data());
	AddLine(pMgr,70,L"%d",m_nFlag);
	AddLine(pMgr,62,L"%d",m_nColor);
	AddLine(pMgr,6,L"%s",(wchar_t*)m_strLineType.data());
	AddLine(pMgr,370,L"%d",m_nLineWeight);
	AddLine(pMgr,390,L"%X",m_nHardPointer);
}

void Layer::Entity::SetFlag(int nFlag)
{
	m_nFlag = nFlag;
}

int Layer::Entity::GetFlag()
{
	return m_nFlag;
}

wchar_t* Layer::Entity::GetLayerName()
{
	return (wchar_t*)m_strLayerName.data();
}

// Return 값 : ACI(AutoCAD Color Index)
int Layer::Entity::GetColor()
{
	return m_nColor;
}

int Layer::Entity::GetHandle()
{
	return m_nHandle;
}

wchar_t* Layer::Entity::GetLineType()
{
	return (wchar_t*)m_strLineType.data();
}

bool Layer::Entity::IsFrozen() const
{
	return (m_nFlag & FROZEN) == FROZEN;
}

bool Layer::Entity::IsLocked() const
{
	return (m_nFlag & LOCK) == LOCK;
}

bool Layer::Entity::IsHidden() const
{
	return m_nColor < 0;
}

Layer::Layer(TableManager* pMgr)
	: Table(pMgr)
{
	Init();
}

Layer::~Layer(void)
{
}

void Layer::Clear()
{
	m_nEntitySize = 0;
	m_list.clear();
}

void Layer::Init()
{
	if (m_pMgr != 0)
	{
		DXFManager* pDXFMgr = m_pMgr->GetOwner();

		if (pDXFMgr != 0)
		{
			m_nHandle = pDXFMgr->Get32BitHandle();
		}
	}

	m_pEntity = 0;
	//m_nHandle = Get32BitHandle();
	m_nSoftPointer = 0;
	m_strEntityName = L"LAYER";
	m_strSubClassName = L"AcDbLayerTableRecord";
	m_nEntitySize = 1;

	m_list.push_back(Entity(this,L"0"));
}

Layer::Entity* Layer::AddEntity(wchar_t* strLayerName)
{
	m_nEntitySize++;
	m_list.push_back(Entity(this,strLayerName));

	std::list<Entity>::iterator& p = m_list.end();
	p--;
	Entity& rEntity = *p;
	return &rEntity;
}

void Layer::Write(Utility::FileManager* pMgr)
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

void Layer::ReadDatai(int nCode, int nData) 
{
	if (m_pEntity)
	{
		if (nCode == 62)
			m_pEntity->SetColor(nData);
		else if (nCode == 70)
			m_pEntity->SetFlag(nData);
	}
}

void Layer::ReadDatad(int nCode, double dData) 
{
}

void Layer::ReadDatas(int nCode, wchar_t* strData) 
{
	if (nCode == 0 && !wcscmp(strData,L"LAYER"))
	{
		int nHandle1 = 0, nHandle2 = 0;

		if (m_pMgr != 0)
		{
			DXFManager* pDXFMgr = m_pMgr->GetOwner();

			if (pDXFMgr != 0)
			{
				nHandle1 = pDXFMgr->Get32BitHandle();
				nHandle2 = pDXFMgr->Get32BitHandle();
			}
		}

		m_list.push_back(Entity(nHandle1, nHandle2));
		m_nEntitySize++;

		std::list<Entity>::iterator p = m_list.end();
		p--;
		Entity& rEntity = *p;
		m_pEntity = &rEntity;
	}
	else
	{
		if (m_pEntity)
		{
			if (nCode == 2) m_pEntity->SetLayerName(strData);
			else if (nCode == 6) m_pEntity->SetLineType(strData);
		}
	}
}

// pID : Layer 정보를 담고 있는 링크드 리스트 노드의 포인터
Layer::Entity* Layer::GetEntity(void*& pID)
{
	//static std::list<Entity>::iterator p;
	std::list<Entity>::iterator& p = m_entIter;

	if (pID == 0) p = m_list.begin();
	else
	{
		p = *(std::list<Entity>::iterator*)pID;
	}

	if (p != m_list.end())
	{
		Entity& rEntity = *p;
		p++;
		pID = &p;

		if (rEntity.GetHandle() < 0) return 0;
		return &rEntity;
	}

	return 0;
}

END_NS
END_NS
