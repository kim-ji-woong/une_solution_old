#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

LType::Entity::Entity(LType* pTable, wchar_t* strLineType, wchar_t* strAnnotation)
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
	m_strLineType = strLineType;
	//m_nHandle = Get32BitHandle();
	m_nFlag = 0;
	wcscpy_s(m_strAnnotation, 256, strAnnotation);
	m_nAlignCode = 65;
	m_nLineTypeSize = 0;
	m_dPatternLength = 0.0;
}

void LType::Entity::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"%s",(wchar_t*)m_pParent->m_strEntityName.data());
	AddLine(pMgr,5,L"%X",m_nHandle);
	AddLine(pMgr,330,L"%X",m_pParent->m_nHandle);
	AddLine(pMgr,100,L"%s",m_pParent->m_strDefSubClassName);
	AddLine(pMgr,100,L"%s",m_pParent->m_strSubClassName);
	AddLine(pMgr,2,L"%s",(wchar_t*)m_strLineType.data());
	AddLine(pMgr,70,L"%d",m_nFlag);
	AddLine(pMgr,3,L"%s",m_strAnnotation);
	AddLine(pMgr,72,L"%d",m_nAlignCode);
	AddLine(pMgr,73,L"%d",m_nLineTypeSize);
	AddLine(pMgr,40,L"%lf",m_dPatternLength);

	std::list<Data>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		AddLine(pMgr,49,L"%lf",p->m_dLength);
		AddLine(pMgr,74,L"%d",p->m_nType);
		p++;
	}
}

void LType::Entity::AddData(double dLength, short nType)
{
	if (m_nLineTypeSize < MAX_LINE_TYPE_DATA) m_nLineTypeSize++;
	else return;

	Data data;
	data.m_dLength = dLength;
	data.m_nType   = nType;
	m_list.push_back(data);

	if (dLength > 0) m_dPatternLength += dLength;
	else m_dPatternLength -= dLength;
}

bool LType::Entity::GetData(void*& pID, double* pLength, short* pType)
{
	//static std::list<Data>::iterator p;
	std::list<Data>::iterator& p = m_dataIter;

	if (pID == 0) p = m_list.begin();
	else
	{
		p = *(std::list<Data>::iterator*)pID;
	}

	if (p != m_list.end())
	{
		Data& rData = *p;
		p++;
		pID = &p;
		*pLength = rData.m_dLength;
		*pType	 = rData.m_nType;
		return true;
	}

	return false;
}

wchar_t* LType::Entity::GetTypeName()
{
	return (wchar_t*)m_strLineType.data();
}

int LType::Entity::GetHandle()
{
	return m_nHandle;
}

LType::LType(TableManager* pMgr)
	: Table(pMgr)
{
	Init();
}

LType::~LType(void)
{
}

void LType::Clear()
{
	m_nEntitySize = 0;
	m_list.clear();
}

void AddCentr01(std::list<LType::Entity>& rList, LType* pLType, int& rEntitySize)
{
	rEntitySize++;

	LType::Entity entity(pLType,L"A-CENTR01",L"중심선(일반) Center ____ _ ____ _ ____ _ ____ _");

	entity.AddData(1.25,0);
	entity.AddData(-0.25,0);
	entity.AddData(0.25,0);
	entity.AddData(-0.25,0);

	rList.push_back(entity);
}

void AddCentr01H(std::list<LType::Entity>& rList, LType* pLType, int& rEntitySize)
{
	rEntitySize++;

	LType::Entity entity(pLType,L"A-CENTR01H",L"중심선(반배수) Center (.5x) ___ _ ___ _ ___ _ _");

	entity.AddData(0.75,0);
	entity.AddData(-0.125,0);
	entity.AddData(0.125,0);
	entity.AddData(-0.125,0);

	rList.push_back(entity);
}

void AddCentr01X(std::list<LType::Entity>& rList, LType* pLType, int& rEntitySize)
{
	rEntitySize++;

	LType::Entity entity(pLType,L"A-CENTR01X",L"중심선(배수) Center (2x) ________  __  ________");

	entity.AddData(2.5,0);
	entity.AddData(-0.5,0);
	entity.AddData(0.5,0);
	entity.AddData(-0.5,0);

	rList.push_back(entity);
}

void AddDashD01(std::list<LType::Entity>& rList, LType* pLType, int& rEntitySize)
{
	rEntitySize++;

	LType::Entity entity(pLType,L"A-DASHD01",L"파선(일반) Dashed __ __ __ __ __ __ __ __ __ __");

	entity.AddData(0.5,0);
	entity.AddData(-0.25,0);

	rList.push_back(entity);
}

void AddDashD01H(std::list<LType::Entity>& rList, LType* pLType, int& rEntitySize)
{
	rEntitySize++;

	LType::Entity entity(pLType,L"A-DASHD01H",L"파선(반배수) Dashed (.5x) _ _ _ _ _ _ _ _ _ _ _");

	entity.AddData(0.25,0);
	entity.AddData(-0.125,0);

	rList.push_back(entity);
}

void AddDashD01X(std::list<LType::Entity>& rList, LType* pLType, int& rEntitySize)
{
	rEntitySize++;

	LType::Entity entity(pLType,L"A-DASHD01X",L"파선(배수) Dashed (2x) ____  ____  ____  ____  ");

	entity.AddData(1.0,0);
	entity.AddData(-0.5,0);

	rList.push_back(entity);
}

void AddHiddn01(std::list<LType::Entity>& rList, LType* pLType, int& rEntitySize)
{
	rEntitySize++;

	LType::Entity entity(pLType,L"A-HIDDN01",L"숨은선(일반) Hidden __ __ __ __ __ __ __ __ __ ");

	entity.AddData(0.25,0);
	entity.AddData(-0.125,0);

	rList.push_back(entity);
}

void AddHiddn01H(std::list<LType::Entity>& rList, LType* pLType, int& rEntitySize)
{
	rEntitySize++;

	LType::Entity entity(pLType,L"A-HIDDN01H",L"숨은선(반배수) Hidden (.5x) _ _ _ _ _ _ _ _ _ _");

	entity.AddData(0.125,0);
	entity.AddData(-0.0625,0);

	rList.push_back(entity);
}

void AddHiddn01X(std::list<LType::Entity>& rList, LType* pLType, int& rEntitySize)
{
	rEntitySize++;

	LType::Entity entity(pLType,L"A-HIDDN01X",L"숨은선(배수) Hidden (2x) ____ ____ ____ ____ __");

	entity.AddData(0.5,0);
	entity.AddData(-0.25,0);

	rList.push_back(entity);
}

void LType::AddEntity(LType::Entity& rEntity)
{
	// 기존에 같은 LineType이 존재하는지 검사
	std::list<Entity>::iterator pIter = m_list.begin();
	std::list<Entity>::iterator pEnd = m_list.end();

	while (pIter != pEnd)
	{
		if (!_wcsicmp(pIter->GetTypeName(),rEntity.GetTypeName())) return;
		pIter++;
	}

	m_list.push_back(rEntity);
	m_nEntitySize++;
}

void LType::Init()
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
	m_strEntityName = L"LTYPE";
	m_strSubClassName = L"AcDbLinetypeTableRecord";
	m_strTypeName = L"";

	m_nEntitySize = 3;

	m_list.push_back(Entity(this,L"ByBlock"));
	m_list.push_back(Entity(this,L"ByLayer"));
	m_list.push_back(Entity(this,L"Continuous",L"Solid line"));

	AddCentr01(m_list,this,m_nEntitySize);
	AddCentr01H(m_list,this,m_nEntitySize);
	AddCentr01X(m_list,this,m_nEntitySize);
	AddDashD01(m_list,this,m_nEntitySize);
	AddDashD01H(m_list,this,m_nEntitySize);
	AddDashD01X(m_list,this,m_nEntitySize);
	AddHiddn01(m_list,this,m_nEntitySize);
	AddHiddn01H(m_list,this,m_nEntitySize);
	AddHiddn01X(m_list,this,m_nEntitySize);
}

void LType::Write(Utility::FileManager* pMgr)
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

LType::Entity* LType::GetEntity(wchar_t* strTypeName)
{
	std::list<Entity>::iterator& p = m_list.begin();

	while (p != m_list.end())
	{
		Entity& rEntity = *p;
		if (!wcscmp(rEntity.GetTypeName(),strTypeName)) return &rEntity;
		++p;
	}

	return 0;
}

void LType::AddEntity(int nFactor, unsigned short nStyle, wchar_t* strTypeName, wchar_t* strAnnotation)
{
	static unsigned short nFlag[16] = {1,2,4,8,16,32,64,128,256,512,1024,2048,4096,8192,16384,32768};
	bool bArr[16];
	int nArr[16], nArrSize = 0, i;

	for (i=0;i<16;i++)
	{
		if ((nStyle & nFlag[i]) == nFlag[i]) bArr[i] = true;
		else bArr[i] = false;
	}

	// 같은 Pattern이 반복하는지 검사
	////////////////////////////////////////////
	unsigned short nArrCompact[3] = {0,0,0};
	for (i=0;i<3;i++)
	{
		int nData = 16 - (2 << i);
		for (int j=15;j>=nData;j--)
		{
			if (bArr[j]) nArrCompact[i] = nArrCompact[i] * 2 + 1;
			else nArrCompact[i] *= 2;
		}
	}

	int nTargetSize = 16;

	for (i=0;i<3;i++)
	{
		int nSize = 2 << i;
		int nData = 16 / nSize;
		unsigned short num = nStyle;
		int nShift = 16 - nSize;

		for (int j=0;j<nData;j++)
		{
			unsigned short temp = num << nShift;
			temp = temp >> nShift;

			if (temp != nArrCompact[i]) break;
			num = num >> nSize;
		}

		if (num == 0)
		{
			nTargetSize = nSize;
			break;
		}
	}
	////////////////////////////////////////////

	bool bPrev = bArr[0];
	if (bPrev) nArr[0] = 1;
	else nArr[0] = -1;

	for (i=1;i<nTargetSize;i++)
	{
		if (bArr[i] == bPrev)
		{
			if (bArr[i]) nArr[nArrSize]++;
			else nArr[nArrSize]--;
		}
		else
		{
			if (bArr[i]) nArr[++nArrSize] = 1;
			else nArr[++nArrSize] = -1;
		}

		bPrev = bArr[i];
	}

	nArrSize++;
	int nLen = 0;

	Entity entity(this,strTypeName,strAnnotation);

	for (i=0;i<nArrSize;i++) 
	{
		nArr[i] *= nFactor;
		if (nArr[i] > 0) nArr[i]--;
		entity.AddData(nArr[i]);
	}

	m_nEntitySize++;
	m_list.push_back(entity);
}

void LType::ReadDatai(int nCode, int nData)
{
	switch (nCode)
	{
	case 74:
		m_pEntity->AddData(m_dTempData,nData);
		break;
	}
}

void LType::ReadDatad(int nCode, double dData)
{
	switch (nCode)
	{
	case 49:
		m_dTempData = dData;
		break;
	}
}

void LType::ReadDatas(int nCode, wchar_t* strData)
{
	//static std::wstring strTypeName;

	switch (nCode)
	{
	case 2:
		m_strTypeName = strData;
		break;

	case 3:
		m_nEntitySize++;
		m_list.push_back(Entity(this,(wchar_t*)m_strTypeName.data(),strData));
		{
			std::list<Entity>::iterator& rIter = m_list.end();
			rIter--;
			Entity& rEntity = *rIter;
			m_pEntity = &rEntity;
		}
		break;
	}
}

LType::Entity* LType::GetEntityFromID(void*& pID)
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
