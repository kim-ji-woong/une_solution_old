#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(BLOCKS)

void BlockData::Block::SetData(int nBlockHandle, int nHandle, wchar_t* strBlockName, wchar_t* strLayerName, double dBasePointX, double dBasePointY, double dBasePointZ, wchar_t* strRefPath)
{
	int nFlag = 0;

	DXFData data[12];
	
	SetDXFData(5,&nHandle,&data[0]);
	SetDXFData(330,&nBlockHandle,&data[1]);
	SetDXFData(100,L"AcDbEntity",&data[2]);
	SetDXFData(8,strLayerName,&data[3]);
	SetDXFData(100,L"AcDbBlockBegin",&data[4]);
	SetDXFData(2,strBlockName,&data[5]);
	SetDXFData(70,&nFlag,&data[6]);
	SetDXFData(10,&dBasePointX,&data[7]);
	SetDXFData(20,&dBasePointY,&data[8]);
	SetDXFData(30,&dBasePointZ,&data[9]);
	SetDXFData(3,strBlockName,&data[10]);
	SetDXFData(1,strRefPath,&data[11]);

	for (int i=0;i<12;i++) m_list.push_back(data[i]);
}

void BlockData::Block::SetSmallDotData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName)
{
	int nHandle = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	//int nHandle = Get32BitHandle();
	int nColor = 0, nFlag2 = 1;
	int nFlag1 = 2;
	double dFlag3 = 0.5, dFlag4 = 1.0;
	double x = 0.0625, _x = -0.0625, y = 0.0;

	DXFData data[17];

	SetDXFData(0,L"LWPOLYLINE",&data[0]);
	SetDXFData(5,&nHandle,&data[1]);
	SetDXFData(330,&nBlockHandle,&data[2]);
	SetDXFData(100,L"AcDbEntity",&data[3]);
	SetDXFData(8,strLayerName,&data[4]);
	SetDXFData(6,L"ByBlock",&data[5]);
	SetDXFData(62,&nColor,&data[6]);
	SetDXFData(100,L"AcDbPolyline",&data[7]);
	SetDXFData(90,&nFlag1,&data[8]);
	SetDXFData(70,&nFlag2,&data[9]);
	SetDXFData(43,&dFlag3,&data[10]);
	SetDXFData(10,&_x,&data[11]);
	SetDXFData(20,&y,&data[12]);
	SetDXFData(42,&dFlag4,&data[13]);
	SetDXFData(10,&x,&data[14]);
	SetDXFData(20,&y,&data[15]);
	SetDXFData(42,&dFlag4,&data[16]);

	for (int i=0;i<17;i++) rList.push_back(data[i]);
}

void BlockData::Block::SetSlashData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName)
{
	int nHandle = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	//int nHandle = Get32BitHandle();
	int nColor = 0;
	int nFlag1 = -2;
	double x1 = -0.5, y1 = -0.5, z1 = 0.0;
	double x2 = 0.5, y2 = 0.5, z2 = 0.0;

	DXFData data[15];

	SetDXFData(0,L"LINE",&data[0]);
	SetDXFData(5,&nHandle,&data[1]);
	SetDXFData(330,&nBlockHandle,&data[2]);
	SetDXFData(100,L"AcDbEntity",&data[3]);
	SetDXFData(8,strLayerName,&data[4]);
	SetDXFData(6,L"ByBlock",&data[5]);
	SetDXFData(62,&nColor,&data[6]);
	SetDXFData(370,&nFlag1,&data[7]);
	SetDXFData(100,L"AcDbLine",&data[8]);
	SetDXFData(10,&x1,&data[9]);
	SetDXFData(20,&y1,&data[10]);
	SetDXFData(30,&z1,&data[11]);
	SetDXFData(11,&x2,&data[12]);
	SetDXFData(21,&y2,&data[13]);
	SetDXFData(31,&z2,&data[14]);

	for (int i=0;i<15;i++) rList.push_back(data[i]);
}

void BlockData::Block::SetTriangleData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName)
{
	int nHandle1 = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	int nHandle2 = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	int nHandle3 = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	//int nHandle1 = Get32BitHandle();
	//int nHandle2 = Get32BitHandle();
	//int nHandle3 = Get32BitHandle();
	int nColor = 0;
	int nFlag1 = -2;
	double x1 = -1.0, y1 = 0.1666666666666666, z1 = 0.0;
	double x2 = 0.0, y2 = 0.0, z2 = 0.0;
	double x3 = 0.0, y3 = 0.0, z3 = 0.0;
	double x4 = -1.0, y4 = -0.1666666666666666, z4 = 0.0;
	double x5 = -1.0, y5 = 0.1666666666666666, z5 = 0.0;
	double x6 = -1.0, y6 = -0.1666666666666666, z6 = 0.0;

	DXFData data[45];

	SetDXFData(0,L"LINE",&data[0]);
	SetDXFData(5,&nHandle1,&data[1]);
	SetDXFData(330,&nBlockHandle,&data[2]);
	SetDXFData(100,L"AcDbEntity",&data[3]);
	SetDXFData(8,strLayerName,&data[4]);
	SetDXFData(6,L"ByBlock",&data[5]);
	SetDXFData(62,&nColor,&data[6]);
	SetDXFData(370,&nFlag1,&data[7]);
	SetDXFData(100,L"AcDbLine",&data[8]);
	SetDXFData(10,&x1,&data[9]);
	SetDXFData(20,&y1,&data[10]);
	SetDXFData(30,&z1,&data[11]);
	SetDXFData(11,&x2,&data[12]);
	SetDXFData(21,&y2,&data[13]);
	SetDXFData(31,&z2,&data[14]);

	SetDXFData(0,L"LINE",&data[15]);
	SetDXFData(5,&nHandle2,&data[16]);
	SetDXFData(330,&nBlockHandle,&data[17]);
	SetDXFData(100,L"AcDbEntity",&data[18]);
	SetDXFData(8,strLayerName,&data[19]);
	SetDXFData(6,L"ByBlock",&data[20]);
	SetDXFData(62,&nColor,&data[21]);
	SetDXFData(370,&nFlag1,&data[22]);
	SetDXFData(100,L"AcDbLine",&data[23]);
	SetDXFData(10,&x3,&data[24]);
	SetDXFData(20,&y3,&data[25]);
	SetDXFData(30,&z3,&data[26]);
	SetDXFData(11,&x4,&data[27]);
	SetDXFData(21,&y4,&data[28]);
	SetDXFData(31,&z4,&data[29]);

	SetDXFData(0,L"LINE",&data[30]);
	SetDXFData(5,&nHandle3,&data[31]);
	SetDXFData(330,&nBlockHandle,&data[32]);
	SetDXFData(100,L"AcDbEntity",&data[33]);
	SetDXFData(8,strLayerName,&data[34]);
	SetDXFData(6,L"ByBlock",&data[35]);
	SetDXFData(62,&nColor,&data[36]);
	SetDXFData(370,&nFlag1,&data[37]);
	SetDXFData(100,L"AcDbLine",&data[38]);
	SetDXFData(10,&x5,&data[39]);
	SetDXFData(20,&y5,&data[40]);
	SetDXFData(30,&z5,&data[41]);
	SetDXFData(11,&x6,&data[42]);
	SetDXFData(21,&y6,&data[43]);
	SetDXFData(31,&z6,&data[44]);

	for (int i=0;i<45;i++) rList.push_back(data[i]);
}

void BlockData::Block::SetTwoLineData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName)
{
	int nHandle1 = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	int nHandle2 = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	int nHandle3 = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	//int nHandle1 = Get32BitHandle();
	//int nHandle2 = Get32BitHandle();
	//int nHandle3 = Get32BitHandle();
	int nColor = 0;
	int nFlag1 = -2;
	double x1 = -0.5, y1 = 0.5, z1 = 0.0;
	double x2 = 0.0, y2 = 0.0, z2 = 0.0;
	double x3 = 0.0, y3 = 0.0, z3 = 0.0;
	double x4 = -0.5, y4 = -0.5, z4 = 0.0;
	double x5 = 0.0, y5 = 0.0, z5 = 0.0;
	double x6 = -1.0, y6 = 0.0, z6 = 0.0;

	DXFData data[45];

	SetDXFData(0,L"LINE",&data[0]);
	SetDXFData(5,&nHandle1,&data[1]);
	SetDXFData(330,&nBlockHandle,&data[2]);
	SetDXFData(100,L"AcDbEntity",&data[3]);
	SetDXFData(8,strLayerName,&data[4]);
	SetDXFData(6,L"ByBlock",&data[5]);
	SetDXFData(62,&nColor,&data[6]);
	SetDXFData(370,&nFlag1,&data[7]);
	SetDXFData(100,L"AcDbLine",&data[8]);
	SetDXFData(10,&x1,&data[9]);
	SetDXFData(20,&y1,&data[10]);
	SetDXFData(30,&z1,&data[11]);
	SetDXFData(11,&x2,&data[12]);
	SetDXFData(21,&y2,&data[13]);
	SetDXFData(31,&z2,&data[14]);

	SetDXFData(0,L"LINE",&data[15]);
	SetDXFData(5,&nHandle2,&data[16]);
	SetDXFData(330,&nBlockHandle,&data[17]);
	SetDXFData(100,L"AcDbEntity",&data[18]);
	SetDXFData(8,strLayerName,&data[19]);
	SetDXFData(6,L"ByBlock",&data[20]);
	SetDXFData(62,&nColor,&data[21]);
	SetDXFData(370,&nFlag1,&data[22]);
	SetDXFData(100,L"AcDbLine",&data[23]);
	SetDXFData(10,&x3,&data[24]);
	SetDXFData(20,&y3,&data[25]);
	SetDXFData(30,&z3,&data[26]);
	SetDXFData(11,&x4,&data[27]);
	SetDXFData(21,&y4,&data[28]);
	SetDXFData(31,&z4,&data[29]);

	SetDXFData(0,L"LINE",&data[30]);
	SetDXFData(5,&nHandle3,&data[31]);
	SetDXFData(330,&nBlockHandle,&data[32]);
	SetDXFData(100,L"AcDbEntity",&data[33]);
	SetDXFData(8,strLayerName,&data[34]);
	SetDXFData(6,L"ByBlock",&data[35]);
	SetDXFData(62,&nColor,&data[36]);
	SetDXFData(370,&nFlag1,&data[37]);
	SetDXFData(100,L"AcDbLine",&data[38]);
	SetDXFData(10,&x5,&data[39]);
	SetDXFData(20,&y5,&data[40]);
	SetDXFData(30,&z5,&data[41]);
	SetDXFData(11,&x6,&data[42]);
	SetDXFData(21,&y6,&data[43]);
	SetDXFData(31,&z6,&data[44]);

	for (int i=0;i<45;i++) rList.push_back(data[i]);
}

void BlockData::Block::SetCircleArrowData(DXFManager* pDXFMgr, std::list<DXFData>& rList, int nBlockHandle, wchar_t* strLayerName)
{
	int nHandle = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	//int nHandle = Get32BitHandle();
	int nColor = 0;
	int nFlag1 = -2;
	double x = 0.0, y = 0.0, z = 0.0;
	double dRadius = 0.25;

	DXFData data[13];

	SetDXFData(0,L"CIRCLE",&data[0]);
	SetDXFData(5,&nHandle,&data[1]);
	SetDXFData(330,&nBlockHandle,&data[2]);
	SetDXFData(100,L"AcDbEntity",&data[3]);
	SetDXFData(8,strLayerName,&data[4]);
	SetDXFData(6,L"ByBlock",&data[5]);
	SetDXFData(62,&nColor,&data[6]);
	SetDXFData(370,&nFlag1,&data[7]);
	SetDXFData(100,L"AcDbCircle",&data[8]);
	SetDXFData(10,&x,&data[9]);
	SetDXFData(20,&y,&data[10]);
	SetDXFData(30,&z,&data[11]);
	SetDXFData(40,&dRadius,&data[12]);

	for (int i=0;i<13;i++) rList.push_back(data[i]);
}

void BlockData::Block::SetDefinedData(DXFManager* pDXFMgr, ArrowType type, int nBlockHandle, wchar_t* strBlockName, wchar_t* strLayerName, double dBasePointX, double dBasePointY, double dBasePointZ, wchar_t* strRefPath)
{
	int nHandle = pDXFMgr == 0 ? 0 : pDXFMgr->Get32BitHandle();
	//int nHandle = Get32BitHandle();
	int nFlag = 0;

	DXFData data[12];
	
	SetDXFData(5,&nHandle,&data[0]);
	SetDXFData(330,&nBlockHandle,&data[1]);
	SetDXFData(100,L"AcDbEntity",&data[2]);
	SetDXFData(8,strLayerName,&data[3]);
	SetDXFData(100,L"AcDbBlockBegin",&data[4]);
	SetDXFData(2,strBlockName,&data[5]);
	SetDXFData(70,&nFlag,&data[6]);
	SetDXFData(10,&dBasePointX,&data[7]);
	SetDXFData(20,&dBasePointY,&data[8]);
	SetDXFData(30,&dBasePointZ,&data[9]);
	SetDXFData(3,strBlockName,&data[10]);
	SetDXFData(1,strRefPath,&data[11]);

	for (int i=0;i<12;i++) m_list.push_back(data[i]);

	switch (type)
	{
	case SMALL_DOT:
		SetSmallDotData(pDXFMgr, m_list,nBlockHandle,strLayerName);
		break;

	case SLASH:
		SetSlashData(pDXFMgr, m_list,nBlockHandle,strLayerName);
		break;

	case TRIANGLE:
		SetTriangleData(pDXFMgr, m_list,nBlockHandle,strLayerName);
		break;

	case TWO_LINE:
		SetTwoLineData(pDXFMgr, m_list,nBlockHandle,strLayerName);
		break;

	case CIRCLE_ARROW:
		SetCircleArrowData(pDXFMgr, m_list,nBlockHandle,strLayerName);
		break;
	}
}

void BlockData::Block::GetBlockName(wchar_t* strBlockName) const
{
	std::list<DXFData>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		DXFData data = *p;
		if (data.nCode == 2)
		{
			wcscpy_s(strBlockName, wcslen(data.str.data()) + 1, data.str.data());
			return;
		}
		++p;
	}
}

void BlockData::Block::Write(Utility::FileManager* pMgr)
{
	std::list<DXFData>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		DXFData data = *p;
		WriteDXFData(pMgr,&data);
		++p;
	}
}

void BlockData::AddEntity(ENTITIES::Entity* pEntity)
{
	if (pEntity) 
	{
		m_listEntity.push_back(pEntity);
	}
}

void BlockData::RemoveEntity(ENTITIES::Entity* pEntity)
{
	if (pEntity)
	{
		m_listEntity.remove(pEntity);
	}
}

void BlockData::EndBlock::SetData(int nBlockHandle, int nHandle, wchar_t* strLayerName)
{
	//int nHandle = Get32BitHandle();

	DXFData data[5];
	
	SetDXFData(5,&nHandle,&data[0]);
	SetDXFData(330,&nBlockHandle,&data[1]);
	SetDXFData(100,L"AcDbEntity",&data[2]);
	SetDXFData(8,strLayerName,&data[3]);
	SetDXFData(100,L"AcDbBlockEnd",&data[4]);

	for (int i=0;i<5;i++) m_list.push_back(data[i]);
}

void BlockData::EndBlock::Write(Utility::FileManager* pMgr)
{
	std::list<DXFData>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		DXFData data = *p;
		WriteDXFData(pMgr,&data);
		++p;
	}
}

BlockData::BlockData(BlockManager* pMgr)
{
	m_pMgr = pMgr;
	m_strLayerName = L"";
	m_strBlockName = L"";

	m_pRefCount = new int;
	*m_pRefCount = 1;
}

BlockData::BlockData(const BlockData& rhs)
{
	Copy(rhs);
	*m_pRefCount += 1;
}

void BlockData::operator= (const BlockData& rhs)
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
			Clear();
		}
	}

	Copy(rhs);
	if (!bSame) *m_pRefCount += 1;
}

BlockData::~BlockData(void)
{
	*m_pRefCount -= 1;
	if (*m_pRefCount <= 0)
	{
		delete m_pRefCount;
		Clear();
	}
}

void BlockData::Copy(const BlockData& rhs)
{
	m_blk		   = rhs.m_blk;
	m_endBlock	   = rhs.m_endBlock;
	m_listEntity   = rhs.m_listEntity;
	m_nBlockHandle = rhs.m_nBlockHandle;
	m_strBlockName = rhs.m_strBlockName;
	m_strLayerName = rhs.m_strLayerName;
	m_pMgr		   = rhs.m_pMgr;
	m_pRefCount	   = rhs.m_pRefCount;
}

void BlockData::Clear()
{
	std::list<ENTITIES::Entity*>::iterator pIter = m_listEntity.begin();
	std::list<ENTITIES::Entity*>::iterator pEnd = m_listEntity.end();

	while (pIter != pEnd)
	{
		ENTITIES::Entity* pEntity = *pIter;
		delete pEntity;
		pIter++;
	}

	m_listEntity.clear();
}

void BlockData::SetBlockHandle(int nBlockHandle)
{
	m_nBlockHandle = nBlockHandle;
}

void BlockData::SetData(wchar_t* strBlockName, wchar_t* strLayerName, double dBasePointX, double dBasePointY, double dBasePointZ, wchar_t* strRefPath)
{
	int nHandle1 = 0;
	int nHandle2 = 0;

	if (m_pMgr != 0)
	{
		DXFManager* pDXFMgr = m_pMgr->GetOwner();

		if (pDXFMgr != 0)
		{
			m_nBlockHandle = pDXFMgr->Get32BitHandle();
			nHandle1 = pDXFMgr->Get32BitHandle();
			nHandle2 = pDXFMgr->Get32BitHandle();
		}
	}

	//m_nBlockHandle = Get32BitHandle();
	m_blk.SetData(m_nBlockHandle, nHandle1, strBlockName,strLayerName,dBasePointX,dBasePointY,dBasePointZ,strRefPath);
	m_endBlock.SetData(m_nBlockHandle, nHandle2,strLayerName);
}

void BlockData::SetDefinedData(ArrowType type, wchar_t* strLayerName, double dBasePointX, double dBasePointY, double dBasePointZ, wchar_t* strRefPath)
{
	wchar_t* strBlockName;

	switch (type)
	{
	case SMALL_DOT:
		strBlockName = L"_DotSmall";
		break;

	case SLASH:
		strBlockName = L"_Oblique";
		break;

	case TRIANGLE:
		strBlockName = L"_ClosedBlank";
		break;

	case TWO_LINE:
		strBlockName = L"_Open90";
		break;

	case CIRCLE_ARROW:
		strBlockName = L"_Small";
		break;

	case NONE:
		strBlockName = L"_None";
		break;

	default:
		return;
	}
	
	DXFManager* pDXFMgr = 0;
	int nHandle = 0;

	if (m_pMgr != 0)
	{
		pDXFMgr = m_pMgr->GetOwner();

		if (pDXFMgr != 0)
		{
			m_nBlockHandle = pDXFMgr->Get32BitHandle();
			nHandle = pDXFMgr->Get32BitHandle();
		}
	}

	//m_nBlockHandle = Get32BitHandle();
	m_blk.SetDefinedData(pDXFMgr, type,m_nBlockHandle,strBlockName,strLayerName,dBasePointX,dBasePointY,dBasePointZ,strRefPath);
	m_endBlock.SetData(m_nBlockHandle, nHandle,strLayerName);
}

void BlockData::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"BLOCK");
	m_blk.Write(pMgr);

	std::list<ENTITIES::Entity*>::const_iterator p = m_listEntity.begin();
	std::list<ENTITIES::Entity*>::const_iterator pEnd = m_listEntity.end();

	while (p != pEnd)
	{
		ENTITIES::Entity* pEntity = *p;
		pEntity->Write(pMgr);
		++p;
	}

	AddLine(pMgr,0,L"ENDBLK");
	m_endBlock.Write(pMgr);
}

int BlockData::GetBlockHandle()
{
	return m_nBlockHandle;
}

void BlockData::GetBlockName(wchar_t* strBlockName) const
{
	m_blk.GetBlockName(strBlockName);
}

void BlockData::ReadDatai(int nCode, int nData)
{
}

void BlockData::ReadDatad(int nCode, double dData)
{
	switch (nCode)
	{
	case 10:
		m_dArrInsert[0] = dData;
		return;

	case 20:
		m_dArrInsert[1] = dData;
		return;

	case 30:
		{
			int nHandle = 0;

			if (m_pMgr != 0)
			{
				DXFManager* pDXFMgr = m_pMgr->GetOwner();

				if (pDXFMgr != 0)
				{
					m_nBlockHandle = pDXFMgr->Get32BitHandle();
					nHandle = pDXFMgr->Get32BitHandle();
				}
			}

			m_dArrInsert[2] = dData;
			//m_nBlockHandle = Get32BitHandle();
			m_blk.SetData(m_nBlockHandle, nHandle, (wchar_t*)m_strBlockName.data(), (wchar_t*)m_strLayerName.data(), m_dArrInsert[0], m_dArrInsert[1], m_dArrInsert[2]);
		}
		return;
	}
}

void BlockData::ReadDatas(int nCode, wchar_t* strData)
{
	switch (nCode)
	{
	case 8:
		m_strLayerName = strData;
		return;

	case 2:
		m_strBlockName = strData;
		return;
	}
}

// pID : Entity 정보가 담긴 링크드 리스트 노드의 포인터
ENTITIES::Entity* BlockData::GetEntity(void*& pID)
{
	//static std::list<ENTITIES::Entity*>::iterator p;
	std::list<ENTITIES::Entity*>::iterator& p = m_entIter;

	if (pID == 0) p = m_listEntity.begin();
	else 
	{
		p = *(std::list<ENTITIES::Entity*>::iterator*)pID;
	}

	if (p != m_listEntity.end())
	{
		ENTITIES::Entity* pEntity = *p;
		p++;
		pID = &p;

		return pEntity;
	}

	return 0;
}

void BlockData::GetInsertPoint(double& insertX, double& insertY, double& insertZ)
{
	insertX = m_dArrInsert[0];
	insertY = m_dArrInsert[1];
	insertZ = m_dArrInsert[2];
}

END_NS
END_NS
