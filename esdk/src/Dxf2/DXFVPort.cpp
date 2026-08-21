#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

VPort::Entity::Entity(int nHandle)
{
	m_pParent = 0;
	m_strVPortName = L"";
	m_nHandle = nHandle;
	//m_nHandle = Get32BitHandle();
	m_nFlag = 0;
	m_dBL[0] = 0.0;
	m_dBL[1] = 0.0;
	m_dTR[0] = 1.0;
	m_dTR[1] = 1.0;
	m_dCenter[0] = 2066.783982387438;
	m_dCenter[1] = 1143.184652058582;
	m_dSnapBasePoint[0] = 0.0;
	m_dSnapBasePoint[1] = 0.0;
	m_dSnapSpace[0] = 10.0;
	m_dSnapSpace[1] = 10.0;
	m_dGridSpace[0] = 10.0;
	m_dGridSpace[1] = 10.0;
	m_dViewDirection[0] = 0.0;
	m_dViewDirection[1] = 0.0;
	m_dViewDirection[2] = 1.0;
	m_dTargetPoint[0] = 0.0;
	m_dTargetPoint[1] = 0.0;
	m_dTargetPoint[2] = 0.0;
	m_dViewHeight = 2286.369304117165;
	m_dAspect	  = 1.80791788856305;
	m_dLensLength = 50.0;
	m_dFrontPlane = 0.0;
	m_dBackPlane  = 0.0;
	m_dSnapAngle  = 0.0;
	m_dTwistAngle = 0.0;
	m_nViewMode = 0;
	m_nCircleZoomPercent = 1000;
	m_nFastZoomSetting = 1;
	m_nIconSetting = 3;
	m_bSnapOnOff = false;
	m_bGridOnOff = false;
	m_nSnapStyle = 0;
	m_nSnapIsopair =0; 
	m_nRenderMode = 0;
	m_nUCSVP = 1;
	m_dUCSOrigin[0] = 0.0;
	m_dUCSOrigin[1] = 0.0;
	m_dUCSOrigin[2] = 0.0;
	m_vUCSAxis[0] = Utility::Vertex3D(1,0,0);
	m_vUCSAxis[1] = Utility::Vertex3D(0,1,0);
	m_nUCSHandle = 0;
	m_nOrthographicType = 0;
	m_dElevation = 0.0;
}

VPort::Entity::Entity(VPort* pTable, wchar_t* strVPortName, int nUCSHandle, Utility::Vertex3D vUCSOrigin, Utility::Vertex3D vAxisX, Utility::Vertex3D vAxisY)
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
	m_strVPortName = strVPortName;
	//m_nHandle = Get32BitHandle();

	m_nFlag = 0;
	m_dBL[0] = 0.0;
	m_dBL[1] = 0.0;
	m_dTR[0] = 1.0;
	m_dTR[1] = 1.0;
	m_dCenter[0] = 2066.783982387438;
	m_dCenter[1] = 1143.184652058582;
	m_dSnapBasePoint[0] = 0.0;
	m_dSnapBasePoint[1] = 0.0;
	m_dSnapSpace[0] = 10.0;
	m_dSnapSpace[1] = 10.0;
	m_dGridSpace[0] = 10.0;
	m_dGridSpace[1] = 10.0;
	m_dViewDirection[0] = 0.0;
	m_dViewDirection[1] = 0.0;
	m_dViewDirection[2] = 1.0;
	m_dTargetPoint[0] = 0.0;
	m_dTargetPoint[1] = 0.0;
	m_dTargetPoint[2] = 0.0;
	m_dViewHeight = 2286.369304117165;
	m_dAspect	  = 1.80791788856305;
	m_dLensLength = 50.0;
	m_dFrontPlane = 0.0;
	m_dBackPlane  = 0.0;
	m_dSnapAngle  = 0.0;
	m_dTwistAngle = 0.0;
	m_nViewMode = 0;
	m_nCircleZoomPercent = 1000;
	m_nFastZoomSetting = 1;
	m_nIconSetting = 3;
	m_bSnapOnOff = false;
	m_bGridOnOff = false;
	m_nSnapStyle = 0;
	m_nSnapIsopair =0; 
	m_nRenderMode = 0;
	m_nUCSVP = 1;
	memcpy(m_dUCSOrigin,vUCSOrigin.m_pt,sizeof(double)*3);
	m_vUCSAxis[0] = vAxisX;
	m_vUCSAxis[1] = vAxisY;
	m_nUCSHandle = nUCSHandle;
	m_nOrthographicType = 0;
	m_dElevation = 0.0;
}

void VPort::Entity::Write(Utility::FileManager* pMgr)
{
	int i;

	AddLine(pMgr,0,L"%s",(wchar_t*)m_pParent->m_strEntityName.data());
	AddLine(pMgr,5,L"%X",m_nHandle);
	AddLine(pMgr,330,L"%X",m_pParent->m_nHandle);
	AddLine(pMgr,100,L"%s",m_pParent->m_strDefSubClassName);
	AddLine(pMgr,100,L"%s",m_pParent->m_strSubClassName);
	AddLine(pMgr,2,L"%s",(wchar_t*)m_strVPortName.data());
	AddLine(pMgr,70,L"%d",m_nFlag);
	AddLine(pMgr,10,L"%lf",m_dBL[0]);
	AddLine(pMgr,20,L"%lf",m_dBL[1]);
	AddLine(pMgr,11,L"%lf",m_dTR[0]);
	AddLine(pMgr,21,L"%lf",m_dTR[1]);
	AddLine(pMgr,12,L"%lf",m_dCenter[0]);
	AddLine(pMgr,22,L"%lf",m_dCenter[1]);
	AddLine(pMgr,13,L"%lf",m_dSnapBasePoint[0]);
	AddLine(pMgr,23,L"%lf",m_dSnapBasePoint[1]);
	AddLine(pMgr,14,L"%lf",m_dSnapSpace[0]);
	AddLine(pMgr,24,L"%lf",m_dSnapSpace[1]);
	AddLine(pMgr,15,L"%lf",m_dGridSpace[0]);
	AddLine(pMgr,25,L"%lf",m_dGridSpace[1]);
	for (i=0;i<3;i++) AddLine(pMgr,10*(i+1)+6,L"%lf",m_dViewDirection[i]);
	for (i=0;i<3;i++) AddLine(pMgr,10*(i+1)+7,L"%lf",m_dTargetPoint[i]);
	AddLine(pMgr,40,L"%lf",m_dViewHeight);
	AddLine(pMgr,41,L"%lf",m_dAspect);
	AddLine(pMgr,42,L"%lf",m_dLensLength);
	AddLine(pMgr,43,L"%lf",m_dFrontPlane);
	AddLine(pMgr,44,L"%lf",m_dBackPlane);
	AddLine(pMgr,50,L"%lf",m_dSnapAngle);
	AddLine(pMgr,51,L"%lf",m_dTwistAngle);
	AddLine(pMgr,71,L"%d",m_nViewMode);
	AddLine(pMgr,72,L"%d",m_nCircleZoomPercent);
	AddLine(pMgr,73,L"%d",m_nFastZoomSetting);
	AddLine(pMgr,74,L"%d",m_nIconSetting);
	AddLine(pMgr,75,L"%d",m_bSnapOnOff);
	AddLine(pMgr,76,L"%d",m_bGridOnOff);
	AddLine(pMgr,77,L"%d",m_nSnapStyle);
	AddLine(pMgr,78,L"%d",m_nSnapIsopair);
	AddLine(pMgr,281,L"%d",m_nRenderMode);
	AddLine(pMgr,65,L"%d",m_nUCSVP);
	for (i=0;i<3;i++) AddLine(pMgr,10*(i+1)+100,L"%lf",m_dUCSOrigin[i]);
	for (i=0;i<3;i++) AddLine(pMgr,10*(i+1)+101,L"%.16lf",m_vUCSAxis[0].m_pt[i]);
	for (i=0;i<3;i++) AddLine(pMgr,10*(i+1)+102,L"%.16lf",m_vUCSAxis[1].m_pt[i]);
	if (m_nUCSHandle > 0) AddLine(pMgr,345,L"%X",m_nUCSHandle);
	AddLine(pMgr,79,L"%d",m_nOrthographicType);
	AddLine(pMgr,146,L"%lf",m_dElevation);
}

void VPort::Entity::SetViewportHeight(double dHeight)
{
	m_dViewHeight = dHeight;
}

// dAspect : 뷰의 너비/ 뷰의 높이
void VPort::Entity::SetViewportAspect(double dAspect)
{
	m_dAspect = dAspect;
}

void VPort::Entity::SetViewportName(wchar_t* strViewportName)
{
	m_strVPortName = strViewportName;
}

void VPort::Entity::SetViewportCenter(double dX, double dY)
{
	m_dCenter[0] = dX;
	m_dCenter[1] = dY;
}

void VPort::Entity::SetUCSAxis(const Utility::Vertex3D& vAxisX, const Utility::Vertex3D& vAxisY)
{
	m_vUCSAxis[0] = vAxisX;
	m_vUCSAxis[1] = vAxisY;
}

wchar_t* VPort::Entity::GetVPortName()
{
	return (wchar_t*)m_strVPortName.data();
}

void VPort::Entity::GetCenterPoint(double* pX, double* pY)
{
	*pX = m_dCenter[0];
	*pY = m_dCenter[1];
}

void VPort::Entity::GetUCSAxisX(double* pX, double* pY, double* pZ)
{
	*pX = m_vUCSAxis[0].m_pt[0];
	*pY = m_vUCSAxis[0].m_pt[1];
	*pZ = m_vUCSAxis[0].m_pt[2];
}

void VPort::Entity::GetUCSAxisY(double* pX, double* pY, double* pZ)
{
	*pX = m_vUCSAxis[1].m_pt[0];
	*pY = m_vUCSAxis[1].m_pt[1];
	*pZ = m_vUCSAxis[1].m_pt[2];
}

double VPort::Entity::GetViewportHeight()
{
	return m_dViewHeight;
}

double VPort::Entity::GetViewportAspect()
{
	return m_dAspect;
}

int VPort::Entity::GetHandle()
{
	return m_nHandle;
}

VPort::VPort(TableManager* pMgr)
	: Table(pMgr)
{
	Init();
}

VPort::~VPort(void)
{
}

void VPort::Clear()
{
	m_nEntitySize = 0;
	m_list.clear();
}

void VPort::Init()
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
	m_strEntityName = L"VPORT";
	m_strSubClassName = L"AcDbViewportTableRecord";
	m_nEntitySize = 1;

	m_list.push_back(Entity(this,L"*Active"));
}

void VPort::Write(Utility::FileManager* pMgr)
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

void VPort::ReadDatai(int nCode, int nData) 
{}

void VPort::ReadDatad(int nCode, double dData) 
{
	if (m_pEntity)
	{
		if (nCode == 12) m_dArrTemp[0] = dData;
		else if (nCode == 22) m_pEntity->SetViewportCenter(m_dArrTemp[0],dData);
		else if (nCode == 111) m_dArrTemp[0] = dData;
		else if (nCode == 121) m_dArrTemp[1] = dData;
		else if (nCode == 131) m_dArrTemp[2] = dData;
		else if (nCode == 112) m_dArrTemp[3] = dData;
		else if (nCode == 122) m_dArrTemp[4] = dData;
		else if (nCode == 132)
		{
			m_pEntity->SetUCSAxis(Utility::Vertex3D(m_dArrTemp[0],m_dArrTemp[1],m_dArrTemp[2]),Utility::Vertex3D(m_dArrTemp[3],m_dArrTemp[4],dData));
		}
		else if (nCode == 40) m_pEntity->SetViewportHeight(dData);
		else if (nCode == 41) m_pEntity->SetViewportAspect(dData);
	}
}

void VPort::ReadDatas(int nCode, wchar_t* strData) 
{
	if (nCode == 0 && !wcscmp(strData,L"VPORT"))
	{
		int nHandle = 0;

		if (m_pMgr != 0)
		{
			DXFManager* pDXFMgr = m_pMgr->GetOwner();

			if (pDXFMgr != 0)
			{
				m_nHandle = pDXFMgr->Get32BitHandle();
			}
		}

		m_list.push_back(Entity(nHandle));
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
			if (nCode == 2) m_pEntity->SetViewportName(strData);
		}
	}
}

VPort::Entity* VPort::GetActiveEntity()
{
	void* pID = 0;
	Entity* pEntity = GetEntity(pID);

	for (;pEntity;pEntity=GetEntity(pID))
	{
		if (!wcscmp(pEntity->GetVPortName(),L"*Active"))
		{
			return pEntity;
		}
	}

	return 0;
}

// pID : Viewport 정보를 담고 있는 링크드 리스트 노드의 포인터
VPort::Entity* VPort::GetEntity(void*& pID)
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
