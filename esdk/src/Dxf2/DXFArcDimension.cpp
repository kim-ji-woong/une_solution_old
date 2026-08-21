#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

ArcDimension::ArcDimension(TABLES::TableManager* pTblMgr, BLOCKS::BlockManager* pBlkMgr, wchar_t* strLayerName)
: Dimension(pTblMgr,pBlkMgr,strLayerName)
{
	Init();
}

ArcDimension::~ArcDimension(void)
{
}

void ArcDimension::Init()
{
	m_strSubClassName = L"AcDbDimension";
	m_strEntityType	  = L"ARC_DIMENSION";
	m_strUserDefined  = L"";
	m_strDimLineStyle = L"ISO-25";
}

// pt1 : 호의 한쪽 끝점
// pt2 : 호의 다른쪽 끝점
// ptCenter : 호의 중점
void ArcDimension::SetTargetPoint(const Utility::Vertex3D& pt1, const Utility::Vertex3D& pt2, const Utility::Vertex3D& ptCenter)
{
	m_ptTarget1 = pt1;
	m_ptTarget2 = pt2;
	m_ptTargetCenter = ptCenter;
}

void ArcDimension::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,2,L"%s",m_strBlockName.data());
	AddLine(pMgr,10,L"%lf",m_dDefPoint[0]);
	AddLine(pMgr,20,L"%lf",m_dDefPoint[1]);
	AddLine(pMgr,30,L"%lf",m_dDefPoint[2]);
	AddLine(pMgr,11,L"%lf",m_dTextMidPoint[0]);
	AddLine(pMgr,21,L"%lf",m_dTextMidPoint[1]);
	AddLine(pMgr,31,L"%lf",m_dTextMidPoint[2]);
	AddLine(pMgr,70,L"40");

	if (wcscmp(m_strUserDefined.data(), L"")) AddLine(pMgr,1,L"%s",m_strUserDefined.data());

	AddLine(pMgr,71,L"%d",m_nAttachType);
	AddLine(pMgr,42,L"%lf",m_dActualMeasurement);
	AddLine(pMgr,3,L"%s",m_strDimLineStyle.data());

	AddLine(pMgr,100,L"AcDbArcDimension");
	AddLine(pMgr,13,L"%lf",m_ptTarget1.m_pt[0]);
	AddLine(pMgr,23,L"%lf",m_ptTarget1.m_pt[1]);
	AddLine(pMgr,33,L"%lf",m_ptTarget1.m_pt[2]);
	AddLine(pMgr,14,L"%lf",m_ptTarget2.m_pt[0]);
	AddLine(pMgr,24,L"%lf",m_ptTarget2.m_pt[1]);
	AddLine(pMgr,34,L"%lf",m_ptTarget2.m_pt[2]);
	AddLine(pMgr,15,L"%lf",m_ptTargetCenter.m_pt[0]);
	AddLine(pMgr,25,L"%lf",m_ptTargetCenter.m_pt[1]);
	AddLine(pMgr,35,L"%lf",m_ptTargetCenter.m_pt[2]);
	AddLine(pMgr,70,L"0");
	AddLine(pMgr,71,L"0");
	AddLine(pMgr,16,L"%lf",m_ptTarget1.m_pt[0]);
	AddLine(pMgr,26,L"%lf",m_ptTarget1.m_pt[1]);
	AddLine(pMgr,36,L"%lf",m_ptTarget1.m_pt[2]);
	AddLine(pMgr,17,L"%lf",m_ptTarget2.m_pt[0]);
	AddLine(pMgr,27,L"%lf",m_ptTarget2.m_pt[1]);
	AddLine(pMgr,37,L"%lf",m_ptTarget2.m_pt[2]);

	AddLine(pMgr,1001,L"ACAD");
	AddLine(pMgr,1000,L"DSTYLE");
	AddLine(pMgr,1002,L"{");
	AddLine(pMgr,1070,L"40");
	AddLine(pMgr,1040,L"100.0");
	AddLine(pMgr,1070,L"41");
	AddLine(pMgr,1040,L"2.5");
	AddLine(pMgr,1070,L"140");
	AddLine(pMgr,1040,L"2.5");
	AddLine(pMgr,1070,L"143");
	AddLine(pMgr,1040,L"0.0393701");
	AddLine(pMgr,1070,L"147");
	AddLine(pMgr,1040,L"0.625");
	AddLine(pMgr,1070,L"172");
	AddLine(pMgr,1070,L"1");
	AddLine(pMgr,1070,L"271");
	AddLine(pMgr,1070,L"2");
	AddLine(pMgr,1002,L"}");
}

END_NS
END_NS
