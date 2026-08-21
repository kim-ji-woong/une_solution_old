#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Solid::Solid(void)
{
	Init();
}

Solid::~Solid(void)
{
}

void Solid::Init()
{
	m_strSubClassName = L"AcDbTrace";
	m_strEntityType	  = L"SOLID";
}

void Solid::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,10,L"%lf",m_ptCorner[0].m_pt[0]);
	AddLine(pMgr,20,L"%lf",m_ptCorner[0].m_pt[1]);
	AddLine(pMgr,30,L"%lf",m_ptCorner[0].m_pt[2]);
	AddLine(pMgr,11,L"%lf",m_ptCorner[1].m_pt[0]);
	AddLine(pMgr,21,L"%lf",m_ptCorner[1].m_pt[1]);
	AddLine(pMgr,31,L"%lf",m_ptCorner[1].m_pt[2]);
	AddLine(pMgr,12,L"%lf",m_ptCorner[2].m_pt[0]);
	AddLine(pMgr,22,L"%lf",m_ptCorner[2].m_pt[1]);
	AddLine(pMgr,32,L"%lf",m_ptCorner[2].m_pt[2]);
	AddLine(pMgr,13,L"%lf",m_ptCorner[3].m_pt[0]);
	AddLine(pMgr,23,L"%lf",m_ptCorner[3].m_pt[1]);
	AddLine(pMgr,33,L"%lf",m_ptCorner[3].m_pt[2]);
}

void Solid::SetPoint(int nIndex, double x, double y, double z)
{
	if (nIndex < 0 || nIndex >= 4) return;

	m_ptCorner[nIndex].m_pt[0] = x;
	m_ptCorner[nIndex].m_pt[1] = y;
	m_ptCorner[nIndex].m_pt[2] = z;
}

void Solid::SetPoint(int nIndex, const Utility::Vertex3D& rPt)
{
	if (nIndex < 0 || nIndex >= 4) return;
	m_ptCorner[nIndex] = rPt;
}

bool Solid::GetPoint(int nIndex, Utility::Vertex3D* pt)
{
	if (nIndex < 0 || nIndex >= 4) return false;
	*pt = m_ptCorner[nIndex];
	return true;
}

bool Solid::ReadDatai(int nCode, int nData)
{
	return __super::ReadDatai(nCode,nData);
}

bool Solid::ReadDatad(int nCode, double dData)
{
	bool bResult = __super::ReadDatad(nCode,dData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 10:
	case 11:
	case 12:
	case 13:
		m_ptCorner[nCode%10].m_pt[0] = dData;
		return true;

	case 20:
	case 21:
	case 22:
	case 23:
		m_ptCorner[nCode%10].m_pt[1] = dData;
		return true;

	case 30:
	case 31:
	case 32:
	case 33:
		m_ptCorner[nCode%10].m_pt[2] = dData;
		return true;

	case 210:
	case 220:
	case 230:
	case 39:
		return true;
	}

	return false;
}

bool Solid::ReadDatas(int nCode, wchar_t* strData)
{
	return __super::ReadDatas(nCode,strData);
}

END_NS
END_NS
