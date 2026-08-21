#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(BLOCKS)

BlockManager::BlockManager(void)
{
	Init();
}

BlockManager::~BlockManager(void)
{
}

void BlockManager::Init()
{
	wcscpy_s(m_strDefaultLayerName, 256, L"0");

	BlockData data[9] = { BlockData(this), BlockData(this), BlockData(this), BlockData(this), BlockData(this), BlockData(this), BlockData(this), BlockData(this), BlockData(this) };
	data[0].SetData(L"*Model_Space",m_strDefaultLayerName);
	data[1].SetData(L"*Paper_Space",m_strDefaultLayerName);
	data[2].SetData(L"*Paper_Space0",m_strDefaultLayerName);

	data[3].SetDefinedData(SMALL_DOT,m_strDefaultLayerName);
	data[4].SetDefinedData(SLASH,m_strDefaultLayerName);
	data[5].SetDefinedData(TRIANGLE,m_strDefaultLayerName);
	data[6].SetDefinedData(TWO_LINE,m_strDefaultLayerName);
	data[7].SetDefinedData(CIRCLE_ARROW,m_strDefaultLayerName);
	data[8].SetDefinedData(NONE,m_strDefaultLayerName);

	for (int i=0;i<9;i++) m_list.push_back(data[i]);

	m_pEntity = 0;
	m_pBlock = 0;
	m_bEntityRead = false;
}

BlockData* BlockManager::AddBlock(TABLES::TableManager* pTblMgr, wchar_t* strBlockName, wchar_t* strLayerName, double dBasePointX, double dBasePointY, double dBasePointZ, wchar_t* strRefPath)
{
	BlockData data(this);
	data.SetData(strBlockName,strLayerName,dBasePointX,dBasePointY,dBasePointZ,strRefPath);
	m_list.push_back(data);

	std::list<BlockData>::iterator pIter = m_list.end();
	--pIter;
	BlockData& rData = *pIter;

	TABLES::BlockRecord* pBlockRecord = pTblMgr->GetBlockRecord();

	if (pBlockRecord && pTblMgr)
	{
		OBJECTS::ObjectManager* pObjMgr = pBlockRecord->GetObjectManager();

		if (pObjMgr)
		{
			int nBlockHandle  = data.GetBlockHandle();
			int nLayoutHandle = pObjMgr->GetLayoutHandle(nBlockHandle);
			TABLES::BlockRecord::Entity entity(pBlockRecord,strBlockName,nBlockHandle,nLayoutHandle);
			pBlockRecord->AddEntity(entity);

			rData.SetBlockHandle(entity.GetBlockHandle());
		}
	}

	return &rData;
}

void BlockManager::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"SECTION");
	AddLine(pMgr,2,L"BLOCKS");

	std::list<BlockData>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		BlockData data = *p;
		data.Write(pMgr);
		++p;
	}

	AddLine(pMgr,0,L"ENDSEC");
}

// 특정 순서의 Block 정보를 알아낸다.
// 만일 해당 Index의 Block이 존재하지 않으면 false를 리턴한다.
// nIndex : 몇 번째 Block인가?
// pBlockHandle : 해당 Block의 핸들
// strBlockName : 해당 Block의 이름
bool BlockManager::GetBlockInfo(wchar_t* strBlockName, int* pBlockHandle, int nIndex)
{
	int nCount = 0;

	std::list<BlockData>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		if (nCount++ == nIndex)
		{
			BlockData data = *p;
			*pBlockHandle  = data.GetBlockHandle();
			data.GetBlockName(strBlockName);
			return true;
		}
		++p;
	}

	return false;
}

void BlockManager::ReadDatai(int nCode, int nData)
{
	if (m_bEntityRead)
	{
		if (m_pEntity) m_pEntity->ReadDatai(nCode,nData);
	}
	else
	{
		if (m_pBlock) m_pBlock->ReadDatai(nCode,nData);
	}
}

void BlockManager::ReadDatad(int nCode, double dData)
{
	if (m_bEntityRead)
	{
		if (m_pEntity) m_pEntity->ReadDatad(nCode,dData);
	}
	else
	{
		if (m_pBlock) m_pBlock->ReadDatad(nCode,dData);
	}
}

void BlockManager::ReadDatas(int nCode, wchar_t* strData)
{
	if (nCode == 0)
	{
		ENTITIES::Entity* pEntity = 0;

		if (!wcscmp(strData,L"ARC"))
			pEntity = new ENTITIES::Arc;
		else if (!wcscmp(strData,L"CIRCLE"))
			pEntity = new ENTITIES::Circle;
		else if (!wcscmp(strData,L"ELLIPSE"))
			pEntity = new ENTITIES::Ellipse;
		else if (!wcscmp(strData,L"HATCH"))
			pEntity = new ENTITIES::Hatch;
		else if (!wcscmp(strData,L"LINE"))
			pEntity = new ENTITIES::Line;
		else if (!wcscmp(strData, L"POINT"))
			pEntity = new ENTITIES::Point;
		else if (!wcscmp(strData,L"LWPOLYLINE"))
			pEntity = new ENTITIES::PolyLine(true);
		else if (!wcscmp(strData,L"POLYLINE"))
			pEntity = new ENTITIES::PolyLine(false);
		else if (!wcscmp(strData,L"TEXT"))
			pEntity = new ENTITIES::Text;
		else if (!wcscmp(strData,L"MTEXT"))
			pEntity = new ENTITIES::MText;
		else if (!wcscmp(strData,L"INSERT"))
			pEntity = new ENTITIES::Insert;
		else if (!wcscmp(strData,L"VERTEX"))
		{
			if (m_pEntity && !wcscmp(m_pEntity->GetEntityType(), L"POLYLINE"))
			{
				ENTITIES::PolyLine* pPolyLine = (ENTITIES::PolyLine*)m_pEntity;
				pPolyLine->ReadVertex(true);
				return;
			}
			else
			{
				m_pEntity = 0;
				return;
			}
		}
		else if (!wcscmp(strData,L"BLOCK"))
		{
			m_bEntityRead = false;
			BlockData data(this);
			m_list.push_back(data);

			std::list<BlockData>::iterator pIter = m_list.end();
			pIter--;
			BlockData& rData = *pIter;
			m_pBlock = &rData;
			return;
		}
		else if (!wcscmp(strData,L"ENDBLK"))
		{
			m_pEntity = 0;
			m_pBlock = 0;
			return;
		}
		else
		{
			m_pEntity = 0;
			return;
		}

		if (m_pBlock)
		{
			m_pEntity = pEntity;
			m_pBlock->AddEntity(pEntity);
			m_bEntityRead = true;

			if (pEntity)
			{
				DXF::ENTITIES::EntityManager* pEntMgr = m_pOwner->GetEntityManager();
				pEntity->SetManager(pEntMgr);
				//pEntMgr->AddEntity(pEntity);
			}
		}
		else delete pEntity;
	}
	else
	{
		if (m_bEntityRead)
		{
			if (m_pEntity) m_pEntity->ReadDatas(nCode,strData);
		}
		else
		{
			if (m_pBlock) m_pBlock->ReadDatas(nCode,strData);
		}
	}
}

const BlockData* BlockManager::GetBlockData(const wchar_t* strBlockName)
{
	std::list<BlockData>::const_iterator pIter = m_list.begin();
	std::list<BlockData>::const_iterator pEnd = m_list.end();
	wchar_t _strBlockName[256];

	while (pIter != pEnd)
	{
		const BlockData& rData = *pIter;
		rData.GetBlockName(_strBlockName);
		if (!wcscmp(_strBlockName,strBlockName)) return &rData;
		pIter++;
	}

	return 0;
}

// MText 임시 객체들을 삭제한다.
void BlockManager::RemoveTempMText(std::list<ENTITIES::MText*>& tempMTextList)
{
	for (std::list<BlockData>::iterator iter = m_list.begin(); iter != m_list.end(); iter++)
	{
		BlockData& rBlock = *iter;

		for (std::list<ENTITIES::MText*>::iterator iter2 = tempMTextList.begin(); iter2 != tempMTextList.end(); iter2++)
		{
			rBlock.RemoveEntity(*iter2);
		}
	}
}

END_NS
END_NS
