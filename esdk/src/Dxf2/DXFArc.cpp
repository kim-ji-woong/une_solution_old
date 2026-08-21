#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Arc::Arc(void)
{
	Init();
}

Arc::~Arc(void)
{
}

void Arc::Init()
{
	m_strSubClassName = L"AcDbCircle";
	m_strEntityType	  = L"ARC";
	m_vNormal.m_pt[0] = 0.0;		// 값 확인할 것
	m_vNormal.m_pt[1] = 0.0;
	m_vNormal.m_pt[2] = 1.0;
}

void Arc::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	if (m_fLineWidth != 0.0f) AddLine(pMgr,39,L"%d",(int)m_fLineWidth);
	AddLine(pMgr,10,L"%lf",m_dArrCoordCenter[0]);
	AddLine(pMgr,20,L"%lf",m_dArrCoordCenter[1]);
	AddLine(pMgr,30,L"%lf",m_dArrCoordCenter[2]);
	AddLine(pMgr,40,L"%lf",m_dRadius);
	AddLine(pMgr,100,L"AcDbArc");
	AddLine(pMgr,50,L"%lf",m_dAngleBegin);
	AddLine(pMgr,51,L"%lf",m_dAngleEnd);

	if (m_vNormal.m_pt[0] != 0.0 || m_vNormal.m_pt[1] != 0.0 || m_vNormal.m_pt[2] != 1.0)
	{
		AddLine(pMgr,210,L"%lf",m_vNormal.m_pt[0]);
		AddLine(pMgr,220,L"%lf",m_vNormal.m_pt[1]);
		AddLine(pMgr,230,L"%lf",m_vNormal.m_pt[2]);
	}
}

void Arc::SetArc(double dArrCoordCenter[3], double dRadius, double dAngleBegin, double dAngleEnd)
{
	memcpy(m_dArrCoordCenter,dArrCoordCenter,sizeof(double)*3);
	m_dRadius = dRadius;
	m_dAngleBegin = dAngleBegin;
	m_dAngleEnd = dAngleEnd;
}

// Degree
void Arc::GetAngle(double* pBeginAngle, double* pEndAngle)
{
	*pBeginAngle = m_dAngleBegin;
	*pEndAngle	 = m_dAngleEnd;
}

bool Arc::ReadDatai(int nCode, int nData)
{
	return __super::ReadDatai(nCode,nData);
}

bool Arc::ReadDatad(int nCode, double dData)
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

	case 50:
		m_dAngleBegin = dData;
		return true;

	case 51:
		m_dAngleEnd = dData;
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

bool Arc::ReadDatas(int nCode, wchar_t* strData)
{
	bool bResult = __super::ReadDatas(nCode,strData);
	if (bResult) return bResult;

	return false;
}

END_NS
END_NS
