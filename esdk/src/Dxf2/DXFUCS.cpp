#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

UCS::Entity::Entity(UCS* pTable, wchar_t* strUCSName, double dCoordOrigin[3], const Utility::Vertex3D& vDirectionX, const Utility::Vertex3D& vDirectionY)
{
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

	m_pParent = pTable;
	wcscpy_s(m_strUCSName, 256,strUCSName);
	//m_nHandle = Get32BitHandle();
	m_nFlag = 0;
	memcpy(m_dOriginCoord,dCoordOrigin,sizeof(double)*3);
	m_vDirection[0] = vDirectionX;
	m_vDirection[1] = vDirectionY;
	m_nConstant = 0;
	m_dElevation = 0.0;
}

void UCS::Entity::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"%s",(wchar_t*)m_pParent->m_strEntityName.data());
	AddLine(pMgr,5,L"%X",m_nHandle);
	AddLine(pMgr,330,L"%X",m_pParent->m_nHandle);
	AddLine(pMgr,100,L"%s",m_pParent->m_strDefSubClassName);
	AddLine(pMgr,100,L"%s",m_pParent->m_strSubClassName);
	AddLine(pMgr,2,L"%s",m_strUCSName);
	AddLine(pMgr,70,L"%d",m_nFlag);
	AddLine(pMgr,10,L"%lf",m_dOriginCoord[0]);
	AddLine(pMgr,20,L"%lf",m_dOriginCoord[0]);
	AddLine(pMgr,30,L"%lf",m_dOriginCoord[0]);

	for (int i=0;i<2;i++)
	{
		for (int j=1;j<=3;j++)
		{
			AddLine(pMgr,j*10+i+1,L"%d",m_vDirection[i].m_pt[j-1]);
		}
	}

	AddLine(pMgr,79,L"%d",m_nConstant);
	AddLine(pMgr,146,L"%lf",m_dElevation);
}

UCS::UCS(TableManager* pMgr)
	: Table(pMgr)
{
	Init();
}

UCS::~UCS(void)
{
}

void UCS::Init()
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
	m_strEntityName = L"UCS";
	m_strSubClassName = L"AcDbUCSTableRecord";
	m_nEntitySize = 0;
}

void UCS::Write(Utility::FileManager* pMgr)
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
