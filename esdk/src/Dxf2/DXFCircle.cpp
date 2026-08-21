#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Circle::Circle(void)
{
	Init();
}

Circle::~Circle(void)
{
}

void Circle::Init()
{
	m_strSubClassName = L"AcDbCircle";
	m_strEntityType	  = L"CIRCLE";
	m_vNormal.m_pt[0] = 0.0;
	m_vNormal.m_pt[1] = 0.0;
	m_vNormal.m_pt[2] = 1.0;
}

void Circle::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	if (m_fLineWidth != 0.0f) AddLine(pMgr,39,L"%d",(int)m_fLineWidth);
	AddLine(pMgr,10,L"%lf",m_dArrCoordCenter[0]);
	AddLine(pMgr,20,L"%lf",m_dArrCoordCenter[1]);
	AddLine(pMgr,30,L"%lf",m_dArrCoordCenter[2]);
	AddLine(pMgr,40,L"%lf",m_dRadius);

	if (m_vNormal.m_pt[0] != 0.0 || m_vNormal.m_pt[1] != 0.0 || m_vNormal.m_pt[2] != 1.0)
	{
		AddLine(pMgr,210,L"%lf",m_vNormal.m_pt[0]);
		AddLine(pMgr,220,L"%lf",m_vNormal.m_pt[1]);
		AddLine(pMgr,230,L"%lf",m_vNormal.m_pt[2]);
	}
}


void Circle::SetCircle(double dArrCoordCenter[3], double dRadius)
{
	memcpy(m_dArrCoordCenter,dArrCoordCenter,sizeof(double)*3);
	m_dRadius = dRadius;
}

double Circle::GetRadius()
{
	return m_dRadius;
}

bool Circle::ReadDatai(int nCode, int nData)
{
	return __super::ReadDatai(nCode,nData);
}

bool Circle::ReadDatad(int nCode, double dData)
{
	bool bResult = __super::ReadDatad(nCode,dData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 10:
		m_dArrCoordCenter[0] = dData;
		return true;

	case 20:
		m_dArrCoordCenter[1] = dData;
		return true;

	case 30:
		m_dArrCoordCenter[2] = dData;
		return true;

	case 40:
		m_dRadius = dData;
		return true;

	case 210:
		m_vNormal.m_pt[0] = dData;
		return true;

	case 220:
		m_vNormal.m_pt[1] = dData;
		return true;

	case 230:
		m_vNormal.m_pt[2] = dData;
		return true;
	}

	return false;
}

bool Circle::ReadDatas(int nCode, wchar_t* strData)
{
	bool bResult = __super::ReadDatas(nCode,strData);
	if (bResult) return bResult;

	return false;
}

END_NS
END_NS
