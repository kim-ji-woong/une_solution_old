#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

RadialDimension::RadialDimension(bool bRadial)
{
	m_bRadial = bRadial;
	Init();
}

RadialDimension::~RadialDimension(void)
{
}

void RadialDimension::Init()
{
	if (m_bRadial) m_strSubClassName = L"AcDbRadialDimension";
	else m_strSubClassName = L"AcDbDiametricDimension";
}

void RadialDimension::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,15,L"%lf",m_ptDefinition.m_pt[0]);
	AddLine(pMgr,25,L"%lf",m_ptDefinition.m_pt[1]);
	AddLine(pMgr,35,L"%lf",m_ptDefinition.m_pt[2]);
	AddLine(pMgr,40,L"%lf",m_dLength);

	AddLine(pMgr,1001,L"ACAD");
	AddLine(pMgr,1000,L"DSTYLE");
	AddLine(pMgr,1002,L"{");
	AddLine(pMgr,1070,L"40");
	AddLine(pMgr,1040,L"100.0");
	AddLine(pMgr,1070,L"288");
	AddLine(pMgr,1070,L"1");
	AddLine(pMgr,1002,L"}");
}

void RadialDimension::SetData(const Utility::Vertex3D& ptObj, double dLength)
{
	m_ptDefinition = ptObj;
	m_dLength = dLength;
}

AngularDimension::AngularDimension()
{
	Init();
}

AngularDimension::~AngularDimension()
{
}

void AngularDimension::Init()
{
	m_strSubClassName = L"AcDb2LineAngularDimension";
}

void AngularDimension::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,13,L"%lf",m_ptRightLineBegin.m_pt[0]);
	AddLine(pMgr,23,L"%lf",m_ptRightLineBegin.m_pt[1]);
	AddLine(pMgr,33,L"%lf",m_ptRightLineBegin.m_pt[2]);
	AddLine(pMgr,14,L"%lf",m_ptRightLineEnd.m_pt[0]);
	AddLine(pMgr,24,L"%lf",m_ptRightLineEnd.m_pt[1]);
	AddLine(pMgr,34,L"%lf",m_ptRightLineEnd.m_pt[2]);
	AddLine(pMgr,15,L"%lf",m_ptLeftLineBegin.m_pt[0]);
	AddLine(pMgr,25,L"%lf",m_ptLeftLineBegin.m_pt[1]);
	AddLine(pMgr,35,L"%lf",m_ptLeftLineBegin.m_pt[2]);
	AddLine(pMgr,16,L"%lf",m_ptArcBegin.m_pt[0]);
	AddLine(pMgr,26,L"%lf",m_ptArcBegin.m_pt[1]);
	AddLine(pMgr,36,L"%lf",m_ptArcBegin.m_pt[2]);

	AddLine(pMgr,1001,L"ACAD");
	AddLine(pMgr,1000,L"DSTYLE");
	AddLine(pMgr,1002,L"{");
	AddLine(pMgr,1070,L"40");
	AddLine(pMgr,1040,L"100.0");
	AddLine(pMgr,1002,L"}");
}

// ptRightLineBegin : 오른쪽 선의 양 끝점 가운데 원의 중점과 가까운 점
// ptRightLineEnd : 오른쪽 선의 양 끝점 가운데 원의 중점과 먼 점
// ptLeftLineBegin : 왼쪽 선의 양 끝점 가운데 원의 중점과 가까운 점
// ptArcBegin : 호의 양 끝점 가운데 아무점이나 상관없음
void AngularDimension::SetData(const Utility::Vertex3D& ptRightLineBegin, const Utility::Vertex3D& ptRightLineEnd, const Utility::Vertex3D& ptLeftLineBegin, const Utility::Vertex3D& ptArcBegin)
{
	m_ptRightLineBegin = ptRightLineBegin;
	m_ptRightLineEnd   = ptRightLineEnd;
	m_ptLeftLineBegin  = ptLeftLineBegin;
	m_ptArcBegin	   = ptArcBegin;
}

AlignedDimension::AlignedDimension(void)
{
	Init();
}

AlignedDimension::~AlignedDimension(void)
{
}

void AlignedDimension::Init()
{
	m_strSubClassName = L"AcDbAlignedDimension";
}

void AlignedDimension::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,13,L"%lf",m_ptDefinition1.m_pt[0]);
	AddLine(pMgr,23,L"%lf",m_ptDefinition1.m_pt[1]);
	AddLine(pMgr,33,L"%lf",m_ptDefinition1.m_pt[2]);
	AddLine(pMgr,14,L"%lf",m_ptDefinition2.m_pt[0]);
	AddLine(pMgr,24,L"%lf",m_ptDefinition2.m_pt[1]);
	AddLine(pMgr,34,L"%lf",m_ptDefinition2.m_pt[2]);

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

void AlignedDimension::SetData(const Utility::Vertex3D& ptObj1, const Utility::Vertex3D& ptObj2)
{
	m_ptDefinition1 = ptObj1;
	m_ptDefinition2 = ptObj2;
}


LinearAndRotatedDimension::LinearAndRotatedDimension(void)
{
	Init();
}

LinearAndRotatedDimension::~LinearAndRotatedDimension(void)
{
}

void LinearAndRotatedDimension::Init()
{
	m_strSubClassName  = L"AcDbAlignedDimension";
	m_strSubClassName2 = L"AcDbRotatedDimension";
	m_dAngle		   = 0.0;
}

void LinearAndRotatedDimension::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,13,L"%lf",m_ptDefinition1.m_pt[0]);
	AddLine(pMgr,23,L"%lf",m_ptDefinition1.m_pt[1]);
	AddLine(pMgr,33,L"%lf",m_ptDefinition1.m_pt[2]);
	AddLine(pMgr,14,L"%lf",m_ptDefinition2.m_pt[0]);
	AddLine(pMgr,24,L"%lf",m_ptDefinition2.m_pt[1]);
	AddLine(pMgr,34,L"%lf",m_ptDefinition2.m_pt[2]);

	if (m_dAngle != 0.0) AddLine(pMgr,50,L"%lf",m_dAngle);

	AddLine(pMgr,100,L"%s",m_strSubClassName2.data());
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

void LinearAndRotatedDimension::SetData(const Utility::Vertex3D& ptObj1, const Utility::Vertex3D& ptObj2, bool bVertical)
{
	int nIndex;

	if (bVertical)
	{
		m_dAngle = 270.0;
		nIndex = 1;
	}
	else
	{
		m_dAngle = 0.0;
		nIndex = 0;
	}

	if (ptObj1.m_pt[nIndex] > ptObj2.m_pt[nIndex])
	{
		m_ptDefinition1 = ptObj2;
		m_ptDefinition2 = ptObj1;
	}
	else
	{
		m_ptDefinition1 = ptObj1;
		m_ptDefinition2 = ptObj2;
	}
}

/*wchar_t* GetDimBlockName()
{
	static wchar_t strName[256];
	static int i = 0;

	swprintf_s(strName,L"*D%d",i++);
	return strName;
}*/

Dimension::Dimension(TABLES::TableManager* pTblMgr, BLOCKS::BlockManager* pBlkMgr, wchar_t* strLayerName)
{
	m_pRefCount  = new int;
	*m_pRefCount = 1;
	m_pDimItem	 = 0;
	m_pBlkData	 = 0;

	if (pTblMgr && pTblMgr->GetBlockRecord())
	{
		wchar_t* strBlockName = pTblMgr->GetDimBlockName();
		m_pBlkData = pBlkMgr->AddBlock(pTblMgr,strBlockName,strLayerName);

		SetBlockName(strBlockName);
		SetSoftPointer(pTblMgr->GetLayoutHandle(1));
	}

	Init();
}

Dimension::~Dimension(void)
{
	*m_pRefCount -= 1;
	if (*m_pRefCount <= 0)
	{
		delete m_pRefCount;
		delete m_pDimItem;
	}
}

Dimension::Dimension(const Dimension& rhs)
{
	memcpy(this,&rhs,sizeof(Dimension));
	*m_pRefCount += 1;
}

void Dimension::operator =(const Dimension& rhs)
{
	// 같은 메모리를 공유하고 있는지 검사
	bool bSame = false;
	if (m_pRefCount == rhs.m_pRefCount) bSame = true;

	if (!bSame)
	{
		*m_pRefCount -= 1;
		if (*m_pRefCount <= 0) 
		{
			delete m_pRefCount;
			delete m_pDimItem;
		}
	}

	memcpy(this,&rhs,sizeof(Dimension));
	if (!bSame) *m_pRefCount += 1;
}

void Dimension::Init()
{
	m_strSubClassName = L"AcDbDimension";
	m_strEntityType	  = L"DIMENSION";
	m_strUserDefined  = L"";
	m_strDimLineStyle = L"ISO-25";
}

void Dimension::Write(Utility::FileManager* pMgr)
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
	AddLine(pMgr,70,L"%d",m_nDimType);

	if (wcscmp(m_strUserDefined.data(),L"")) AddLine(pMgr,1,L"%s",m_strUserDefined.data());

	AddLine(pMgr,71,L"%d",m_nAttachType);
	AddLine(pMgr,42,L"%lf",m_dActualMeasurement);
	AddLine(pMgr,3,L"%s",m_strDimLineStyle.data());

	if (m_pDimItem) m_pDimItem->Write(pMgr);
}

BLOCKS::BlockData* Dimension::GetBlockData()
{
	return m_pBlkData;
}

void Dimension::SetUserDefinedText(wchar_t* strUserDefined)
{
	m_strUserDefined = strUserDefined;
}

void Dimension::SetBlockName(wchar_t* strBlockName)
{
	m_strBlockName = strBlockName;
}

void Dimension::SetDefinitionPoint(double x, double y, double z)
{
	m_dDefPoint[0] = x;
	m_dDefPoint[1] = y;
	m_dDefPoint[2] = z;
}

void Dimension::SetTextMiddlePoint(double x, double y, double z)
{
	m_dTextMidPoint[0] = x;
	m_dTextMidPoint[1] = y;
	m_dTextMidPoint[2] = z;
}

void Dimension::SetDimensionType(int nType)
{
	m_nDimType = nType;

	/*if (nType >= USER_DEFINED) nType -= USER_DEFINED;		// 치수 문자의 위치가 사용자 정의 위치로 정해졌음
	if (nType >= ORDINATE_TYPE) nType -= ORDINATE_TYPE;		// 세로 좌표
	if (nType >= BLOCK_REF) nType -= BLOCK_REF;				// Block 참조

	delete m_pDimItem;

	switch (nType)
	{
	case NORMAL:
		m_pDimItem = new LinearAndRotatedDimension;
		break;

	case ALIGNED:
		m_pDimItem = new AlignedDimension;
		break;

	case DIAMETER:
		m_pDimItem = new RadialDimension(false);
		break;

	case RADIUS:
		m_pDimItem = new RadialDimension(true);
		break;

	case ANGULAR:
	case ANGULAR_3_POINT:
	case ORDINATE:
	default:
		m_pDimItem = 0;
		return;
	}*/
}

void Dimension::SetDimensionItem(DimensionItem* pItem)
{
	delete m_pDimItem;
	m_pDimItem = pItem;
}

void Dimension::SetAttachmentType(int nType)
{
	m_nAttachType = nType;
}

void Dimension::SetActualMeasurement(double dActualMeasurement)
{
	m_dActualMeasurement = dActualMeasurement;
}

void Dimension::SetDimLineStyle(wchar_t* strDimLineStyle)
{
	m_strDimLineStyle = strDimLineStyle;
}

END_NS
END_NS
