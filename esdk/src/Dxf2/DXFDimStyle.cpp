#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

DimStyle::Entity::Entity(DimStyle* pTable, int nStyleHandle)
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

	m_pParent	   = pTable;
	//m_nHandle	   = Get32BitHandle();
	m_nStyleHandle = nStyleHandle;

	Init();
}

void DimStyle::Entity::Init()
{
	m_strDimStyle = L"ISO-25";
	m_dArrowSize = 2.5;
	m_dSpaceFromObject = 0.625;
	m_dBaseLineSpace = 3.75;
	m_dExtendedLength = 1.25;
	m_nTIH = 0;
	m_nTOH = 0;
	m_nTAD = 1;
	m_nZIN = 8;
	m_dFontSize = 2.5;
	m_dCenter = 2.5;
	m_dAltf = 0.03937007874016;
	m_dGap = 0.625;
	m_nAltd = 3;
	m_nTofl = 1;
	m_nDec = 2;
	m_nTDec = 2;
	m_nAltTD = 3;
	m_nDSep = 44;
	m_nTolJ = 0;
	m_nTZin = 8;
	m_nTextColor = 0;		// ByBlock
	m_arrowType = FILL_TRIANGLE;
}

void DimStyle::Entity::Write(Utility::FileManager* pMgr, BlockRecord* pBlockRecord) const
{
	AddLine(pMgr,0,L"%s",(wchar_t*)m_pParent->m_strEntityName.data());
	AddLine(pMgr,105,L"%X",m_nHandle);
	AddLine(pMgr,330,L"%X",m_pParent->m_nHandle);
	AddLine(pMgr,100,L"%s",m_pParent->m_strDefSubClassName);
	AddLine(pMgr,100,L"%s",m_pParent->m_strSubClassName);
	AddLine(pMgr,2,L"%s",m_strDimStyle.data());
	AddLine(pMgr,70,L"0");

	AddLine(pMgr,41,L"%lf",m_dArrowSize);
	AddLine(pMgr,42,L"%lf",m_dSpaceFromObject);
	AddLine(pMgr,43,L"%lf",m_dBaseLineSpace);
	AddLine(pMgr,44,L"%lf",m_dExtendedLength);
	AddLine(pMgr,73,L"%d",m_nTIH);
	AddLine(pMgr,74,L"%d",m_nTOH);
	AddLine(pMgr,77,L"%d",m_nTAD);
	AddLine(pMgr,78,L"%d",m_nZIN);
	AddLine(pMgr,140,L"%lf",m_dFontSize);
	AddLine(pMgr,141,L"%lf",m_dCenter);
	AddLine(pMgr,143,L"%lf",m_dAltf);
	AddLine(pMgr,147,L"%lf",m_dGap);
	AddLine(pMgr,171,L"%d",m_nAltd);
	AddLine(pMgr,172,L"%d",m_nTofl);
	AddLine(pMgr,178,L"%d",m_nTextColor);
	AddLine(pMgr,271,L"%d",m_nDec);
	AddLine(pMgr,272,L"%d",m_nTDec);
	AddLine(pMgr,274,L"%d",m_nAltTD);
	AddLine(pMgr,278,L"%d",m_nDSep);
	AddLine(pMgr,283,L"%d",m_nTolJ);
	AddLine(pMgr,284,L"%d",m_nTZin);

	AddLine(pMgr,340,L"%X",m_nStyleHandle);

	if (m_arrowType != FILL_TRIANGLE && pBlockRecord)
	{
		int nBlockHandle = pBlockRecord->GetBlockRecordHandle(m_arrowType);
		if (nBlockHandle) AddLine(pMgr,342,L"%X",nBlockHandle);
	}
}

void DimStyle::Entity::SetDimStyleName(wchar_t* strDimStyle)
{
	if (strDimStyle) m_strDimStyle = strDimStyle;
}

void DimStyle::Entity::SetArrowSize(double dArrowSize)
{
	m_dArrowSize = dArrowSize;
}

void DimStyle::Entity::SetBaseLineSpace(double dSpace)
{
	m_dBaseLineSpace = dSpace;
}

void DimStyle::Entity::SetExtendedLength(double dLength)
{
	m_dExtendedLength = dLength;
}

void DimStyle::Entity::SetTIH(int nTIH)
{
	m_nTIH = nTIH;
}

void DimStyle::Entity::SetTOH(int nTOH)
{
	m_nTOH = nTOH;
}

void DimStyle::Entity::SetZIN(int nZIN)
{
	m_nZIN = nZIN;
}

void DimStyle::Entity::SetFontSize(double dFontSize)
{
	m_dFontSize = dFontSize;
}

void DimStyle::Entity::SetCenterMarkSize(double dSize)
{
	m_dCenter = dSize;
}

void DimStyle::Entity::SetAltF(double dAltf)
{
	m_dAltf = dAltf;
}

void DimStyle::Entity::SetTextSpace(double dSpace)
{
	m_dGap = dSpace;
}

void DimStyle::Entity::SetSignificant(int nSignificant)
{
	m_nDec = nSignificant;
}

void DimStyle::Entity::SetTextDistance(bool bFar)
{
	if (bFar) m_nTAD = 2;
	else m_nTAD = 1;
}

void DimStyle::Entity::SetTDec(int nTDec)
{
	m_nTDec = nTDec;
}

void DimStyle::Entity::SetAltTD(int nAltTD)
{
	m_nAltTD = nAltTD;
}

void DimStyle::Entity::SetDSep(int nDSep)
{
	m_nDSep = nDSep;
}

void DimStyle::Entity::SetTolJ(int nTolJ)
{
	m_nTolJ = nTolJ;
}

void DimStyle::Entity::SetTZin(int nTZin)
{
	m_nTZin = nTZin;
}

void DimStyle::Entity::SetTofl(int nTofl)
{
	m_nTofl = nTofl;
}

void DimStyle::Entity::SetSpaceFromObject(double dSpace)
{
	m_dSpaceFromObject = dSpace;
}

void DimStyle::Entity::SetTextColor(int nACI)
{
	m_nTextColor = nACI;
}

void DimStyle::Entity::SetArrowType(ArrowType type)
{
	m_arrowType = type;
}

wchar_t* DimStyle::Entity::GetDimStyleName()
{
	return (wchar_t*)m_strDimStyle.data();
}

double DimStyle::Entity::GetArrowSize()
{
	return m_dArrowSize;
}

double DimStyle::Entity::GetBaseLineSpace()
{
	return m_dBaseLineSpace;
}

int DimStyle::Entity::GetTIH()
{
	return m_nTIH;
}

int DimStyle::Entity::GetTOH()
{
	return m_nTOH;
}

int DimStyle::Entity::GetZIN()
{
	return m_nZIN;
}

double DimStyle::Entity::GetFontSize()
{
	return m_dFontSize;
}

double DimStyle::Entity::GetCenterMarkSize()
{
	return m_dCenter;
}

double DimStyle::Entity::GetAltF()
{
	return m_dAltf;
}

double DimStyle::Entity::GetTextSpace()
{
	return m_dGap;
}

int DimStyle::Entity::GetSignificant()
{
	return m_nDec;
}

// true : 객체와 치수선 바깥쪽에 문자열을 그린다.
// false : 객체와 치수선 사이에 문자열을 그린다.
bool DimStyle::Entity::GetTextDistance()
{
	if (m_nTAD == 2) return true;
	return false;
}

int DimStyle::Entity::GetTDec()
{
	return m_nTDec;
}

int DimStyle::Entity::GetAltTD()
{
	return m_nAltTD;
}

int DimStyle::Entity::GetDSep()
{
	return m_nDSep;
}

int DimStyle::Entity::GetTolJ()
{
	return m_nTolJ;
}

int DimStyle::Entity::GetTZin()
{
	return m_nTZin;
}

int DimStyle::Entity::GetHandle()
{
	return m_nHandle;
}

int DimStyle::Entity::GetStyleHandle()
{
	return m_nStyleHandle;
}

double DimStyle::Entity::GetSpaceFromObject()
{
	return m_dSpaceFromObject;
}

// ACI
int DimStyle::Entity::GetTextColor()
{
	return m_nTextColor;
}

ArrowType DimStyle::Entity::GetArrowType() const
{
	return m_arrowType;
}

DimStyle::DimStyle(TableManager* pMgr)
	: Table(pMgr)
{
	Init();
}

DimStyle::~DimStyle(void)
{
}

void DimStyle::Init()
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
	m_strEntityName = L"DIMSTYLE";
	m_strDefSubClassName = L"AcDbSymbolTableRecord";
	m_strSubClassName = L"AcDbDimStyleTableRecord";
	m_nEntitySize = 1;
	m_pBlockRecord = 0;
}

void DimStyle::Write(Utility::FileManager* pMgr)
{
	Table::Write(pMgr);
	AddLine(pMgr,100,L"AcDbDimStyleTable");
	AddLine(pMgr,71,L"%d",m_list.size());
//	AddLine(pMgr,340,"%X",m_nStyleHandle);

	std::list<Entity>::iterator p = m_list.begin();
	std::list<Entity>::iterator pEnd = m_list.end();

	while (p != pEnd)
	{
		Entity& rEntity = *p;
		AddLine(pMgr,340,L"%X",rEntity.GetHandle());
		++p;
	}

	p = m_list.begin();

	while (p != pEnd)
	{
		Entity& rEntity = *p;
		rEntity.Write(pMgr,m_pBlockRecord);
		++p;
	}

	AddLine(pMgr,0,L"ENDTAB");
}

void DimStyle::AddEntity(const Entity& rEntity)
{
	m_list.push_back(rEntity);
	m_nEntitySize = (int)m_list.size();
}

DimStyle::Entity* DimStyle::GetEntity(wchar_t* strStyleName)
{
	std::list<Entity>::iterator p = m_list.begin();
	std::list<Entity>::iterator pEnd = m_list.end();

	while (p != pEnd)
	{
		Entity& rEntity = *p;
		if (!wcscmp(rEntity.GetDimStyleName(),strStyleName)) return &rEntity;
		++p;
	}

	return 0;
}

void DimStyle::SetBlockRecord(BlockRecord* pBlockRecord)
{
	m_pBlockRecord = pBlockRecord;
}

END_NS
END_NS
