#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Point::Point(void)
{
	Init();
}

Point::Point(double x, double y, double z)
{
	Init();
	SetCoord(x, y, z);
}

Point::~Point(void)
{
}

void Point::Init()
{
	m_strSubClassName = L"AcDbPoint";
	m_strEntityType	  = L"POINT";
}

void Point::SetCoord(double x, double y, double z)
{
	m_dX = x;
	m_dY = y;
	m_dZ = z;
}

void Point::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	// Line Data »ðÀÔ
	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,10,L"%lf",m_dX);
	AddLine(pMgr,20,L"%lf",m_dY);
	AddLine(pMgr,30,L"%lf",m_dZ);
}

double Point::X()
{
	return m_dX;
}

double Point::Y()
{
	return m_dY;
}

double Point::Z()
{
	return m_dZ;
}

bool Point::ReadDatai(int nCode, int nData)
{
	return __super::ReadDatai(nCode,nData);
}

bool Point::ReadDatad(int nCode, double dData)
{
	bool bResult = __super::ReadDatad(nCode,dData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 10:
		m_dX = dData;
		return true;

	case 20:
		m_dY = dData;
		return true;

	case 30:
		m_dZ = dData;
		return true;
	}

	return false;
}

bool Point::ReadDatas(int nCode, wchar_t* strData)
{
	bool bResult = __super::ReadDatas(nCode,strData);
	if (bResult) return bResult;

	return false;
}

END_NS
END_NS
