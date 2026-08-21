#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Line::Line(void)
{
	Init();
}

Line::Line(double dArrCoordBegin[3], double dArrCoordEnd[3])
{
	Init();
	SetLine(dArrCoordBegin,dArrCoordEnd);
}

Line::~Line(void)
{
}

void Line::Init()
{
	m_strSubClassName = L"AcDbLine";
	m_strEntityType	  = L"LINE";
}

void Line::SetLine(double dArrCoordBegin[3], double dArrCoordEnd[3])
{
	memcpy(m_dArrCoordBegin,dArrCoordBegin,sizeof(double)*3);
	memcpy(m_dArrCoordEnd,dArrCoordEnd,sizeof(double)*3);
}

void Line::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	// Line Data »ðÀÔ
	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	if (m_fLineWidth != 0.0f) AddLine(pMgr,39,L"%d",(int)m_fLineWidth);
	AddLine(pMgr,10,L"%lf",m_dArrCoordBegin[0]);
	AddLine(pMgr,20,L"%lf",m_dArrCoordBegin[1]);
	AddLine(pMgr,30,L"%lf",m_dArrCoordBegin[2]);
	AddLine(pMgr,11,L"%lf",m_dArrCoordEnd[0]);
	AddLine(pMgr,21,L"%lf",m_dArrCoordEnd[1]);
	AddLine(pMgr,31,L"%lf",m_dArrCoordEnd[2]);
}

void Line::GetCoord(double dArrCoordBegin[3], double dArrCoordEnd[3])
{
	int nSize = sizeof(double) * 3;
	memcpy(dArrCoordBegin,m_dArrCoordBegin,nSize);
	memcpy(dArrCoordEnd,m_dArrCoordEnd,nSize);
}

bool Line::ReadDatai(int nCode, int nData)
{
	return __super::ReadDatai(nCode,nData);
}

bool Line::ReadDatad(int nCode, double dData)
{
	bool bResult = __super::ReadDatad(nCode,dData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 10:
		m_dArrCoordBegin[0] = dData;
		return true;

	case 20:
		m_dArrCoordBegin[1] = dData;
		return true;

	case 30:
		m_dArrCoordBegin[2] = dData;
		return true;

	case 11:
		m_dArrCoordEnd[0] = dData;
		return true;

	case 21:
		m_dArrCoordEnd[1] = dData;
		return true;

	case 31:
		m_dArrCoordEnd[2] = dData;
		return true;
	}

	return false;
}

bool Line::ReadDatas(int nCode, wchar_t* strData)
{
	bool bResult = __super::ReadDatas(nCode,strData);
	if (bResult) return bResult;

	return false;
}

END_NS
END_NS
