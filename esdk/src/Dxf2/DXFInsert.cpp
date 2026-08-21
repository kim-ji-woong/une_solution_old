#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Insert::Insert(void)
{
	Init();
}

Insert::~Insert(void)
{
}

void Insert::Init()
{
	m_strSubClassName = L"AcDbBlockReference";
	m_strEntityType	  = L"INSERT";
	m_ptInsert.SetVertex(0.0,0.0,0.0);
	m_strBlockName = L"";
	m_dAngle = 0.0;
}

bool Insert::ReadDatai(int nCode, int nData)
{
	return __super::ReadDatai(nCode,nData);
}

bool Insert::ReadDatad(int nCode, double dData)
{
	bool bResult = __super::ReadDatad(nCode,dData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 10:
		m_ptInsert.m_pt[0] = dData;
		return true;

	case 20:
		m_ptInsert.m_pt[1] = dData;
		return true;

	case 30:
		m_ptInsert.m_pt[2] = dData;
		return true;

	case 50:
		m_dAngle = dData;
		return true;
	}

	return false;
}

bool Insert::ReadDatas(int nCode, wchar_t* strData)
{
	bool bResult = __super::ReadDatas(nCode,strData);
	if (bResult) return bResult;

	if (nCode == 2) m_strBlockName = strData;

	return false;
}

const wchar_t* Insert::GetBlockName() const
{
	return m_strBlockName.data();
}

const Utility::Vertex3D& Insert::GetInsertPoint() const
{
	return m_ptInsert;
}

void Insert::SetBlockName(const wchar_t* strBlockName)
{
	m_strBlockName = strBlockName;
}

void Insert::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,2,L"%s",m_strBlockName.c_str());
	AddLine(pMgr,10,L"%lf",m_ptInsert.m_pt[0]);
	AddLine(pMgr,20,L"%lf",m_ptInsert.m_pt[1]);
	AddLine(pMgr,30,L"%lf",m_ptInsert.m_pt[2]);
	AddLine(pMgr,50,L"%lf",m_dAngle);
}

double Insert::GetAngle() const
{
	return m_dAngle;
}

END_NS
END_NS
