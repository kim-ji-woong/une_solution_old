#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

View::Entity::Entity(View* pTable, wchar_t* strViewName, double dViewHeight, double dViewWidth, double dViewCenterPoint[2], const Utility::Vertex3D& vViewDirection, double dTargetPoint[3])
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
				m_nDictionaryHandle = pDXFMgr->Get32BitHandle();
			}
		}
	}

	m_pParent = pTable;
	wcscpy_s(m_strViewName, 256,strViewName);
	//m_nHandle = Get32BitHandle();
	//m_nDictionaryHandle = Get32BitHandle();
	m_nFlag = 0;
	m_dViewHeight = dViewHeight;
	memcpy(m_dViewCenterPoint,dViewCenterPoint,sizeof(double)*2);
	m_dViewWidth = dViewWidth;
	m_vViewDirection = vViewDirection;
	memcpy(m_dTargetPoint,dTargetPoint,sizeof(double)*3);
	m_dLensLength = 50.0;
	m_dFrontPlane = 0.0;
	m_dBackPlane  = 0.0;
	m_dTwistAngle = 0.0;
	m_nViewMode   = 0;
	m_nRenderMode = 0;
	m_bAssociatedUCS = false;
}

void View::Entity::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"%s",(wchar_t*)m_pParent->m_strEntityName.data());
	AddLine(pMgr,5,L"%X",m_nHandle);
	AddLine(pMgr,102,L"{ACAD_XDICTIONARY\r\n360\r\n%X\r\n102\r\n}",m_nDictionaryHandle);
	AddLine(pMgr,330,L"%X",m_pParent->m_nHandle);
	AddLine(pMgr,100,L"%s",m_pParent->m_strDefSubClassName);
	AddLine(pMgr,100,L"%s",m_pParent->m_strSubClassName);
	AddLine(pMgr,2,L"%s",m_strViewName);
	AddLine(pMgr,70,L"%d",m_nFlag);
	AddLine(pMgr,40,L"%lf",m_dViewHeight);
	AddLine(pMgr,10,L"%lf",m_dViewCenterPoint[0]);
	AddLine(pMgr,20,L"%lf",m_dViewCenterPoint[1]);
	AddLine(pMgr,41,L"%lf",m_dViewWidth);
	AddLine(pMgr,11,L"%lf",m_vViewDirection.m_pt[0]);
	AddLine(pMgr,21,L"%lf",m_vViewDirection.m_pt[1]);
	AddLine(pMgr,31,L"%lf",m_vViewDirection.m_pt[2]);
	AddLine(pMgr,12,L"%lf",m_dTargetPoint[0]);
	AddLine(pMgr,22,L"%lf",m_dTargetPoint[1]);
	AddLine(pMgr,32,L"%lf",m_dTargetPoint[2]);
	AddLine(pMgr,42,L"%lf",m_dLensLength);
	AddLine(pMgr,43,L"%lf",m_dFrontPlane);
	AddLine(pMgr,44,L"%lf",m_dBackPlane);
	AddLine(pMgr,50,L"%lf",m_dTwistAngle);
	AddLine(pMgr,71,L"%d",m_nViewMode);
	AddLine(pMgr,281,L"%d",m_nRenderMode);
	AddLine(pMgr,72,L"%d",m_bAssociatedUCS);
}

View::View(TableManager* pMgr)
	: Table(pMgr)
{
	Init();
}

View::~View(void)
{
}

void View::Init()
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
	m_strEntityName = L"VIEW";
	m_strSubClassName = L"AcDbViewTableRecord";
	m_nEntitySize = 0;
}

void View::Write(Utility::FileManager* pMgr)
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
