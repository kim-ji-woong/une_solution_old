#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

TableManager::TableManager(void)
{
	Init();
}

TableManager::~TableManager(void)
{
	Clear();
	m_bDeleted = true;
}

void TableManager::Clear()
{
	if (m_bDeleted) return;
	if (m_pOwner)
	{
		if (m_pOwner->GetTableManager() != this) return;
	}
	std::list<Table*>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Table* pTable = *p;
		p++;
		delete pTable;
	}

	m_list.clear();
}

void TableManager::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"SECTION");
	AddLine(pMgr,2,L"TABLES");

	DimStyle* pDimStyle = GetDimStyle();
	if (pDimStyle) pDimStyle->SetBlockRecord(GetBlockRecord());

	std::list<Table*>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		Table* pTable = *p;
		pTable->Write(pMgr);
		++p;
	}

	AddLine(pMgr,0,L"ENDSEC");
}

void TableManager::Init()
{
	m_isNewTable = false;

	m_pTable = 0;
	VPort* pVPort = new VPort(this);
	LType* pLType = new LType(this);
	Layer* pLayer = new Layer(this);
	Style* pStyle = new Style(this);
	View* pView = new View(this);
	UCS* pUCS = new UCS(this);
	AppID* pApp = new AppID(this);
	DimStyle* pDim = new DimStyle(this);

	m_list.push_back(pApp);
	m_list.push_back(pDim);
	m_list.push_back(pLayer);
	m_list.push_back(pLType);
	m_list.push_back(pStyle);
	m_list.push_back(pUCS);
	m_list.push_back(pView);
	m_list.push_back(pVPort);

	m_nDimBlockIndex = 0;
}

void TableManager::AddTable(Table* pTable)
{
	m_list.push_back(pTable);
}

// nIndex번째 Layout 객체의 Handle을 얻어온다.
int TableManager::GetLayoutHandle(int nIndex)
{
	std::list<Table*>::const_iterator p = m_list.begin();
	int nCount = 0;

	while (p != m_list.end())
	{
		Table* pTable = *p;
		if (!wcscmp(pTable->GetEntityName(),L"BLOCK_RECORD"))
		{
			if (nCount++ == nIndex)
			{
				return pTable->GetHandle();
			}
		}

		++p;
	}

	return 0;
}

void TableManager::ReadDatai(int nCode, int nData)
{
	if (m_pTable) m_pTable->ReadDatai(nCode,nData);
}

void TableManager::ReadDatad(int nCode, double dData) 
{
	if (m_pTable) m_pTable->ReadDatad(nCode,dData);
}

void TableManager::ReadDatas(int nCode, wchar_t* strData) 
{
	if (nCode == 0 && !wcscmp(strData,L"TABLE")) m_isNewTable = true;
	else
	{
		if (m_isNewTable && nCode == 2)
		{
			m_isNewTable = false;

			if (!wcscmp(strData,L"APPID")) m_pTable = new AppID(this);
			else if (!wcscmp(strData,L"DIMSTYLE")) m_pTable = new DimStyle(this);
			else if (!wcscmp(strData,L"LAYER")) 
			{
				m_pTable = new Layer(this);
				m_pTable->Clear();
			}
			else if (!wcscmp(strData,L"LTYPE")) m_pTable = new LType(this);
			else if (!wcscmp(strData,L"STYLE")) m_pTable = new Style(this);
			else if (!wcscmp(strData,L"UCS")) m_pTable = new UCS(this);
			else if (!wcscmp(strData,L"VIEW")) m_pTable = new AppID(this);
			else if (!wcscmp(strData,L"VPORT")) 
			{
				m_pTable = new VPort(this);
				m_pTable->Clear();
			}
			else if (!wcscmp(strData,L"BLOCK_RECORD")) 
			{
				if (m_pOwner)
				{
					m_pTable = new BlockRecord(m_pOwner->GetBlockManager(),m_pOwner->GetObjectManager(), this);
				}
				else 
				{
					m_pTable = 0;
					return;
				}
			}
			else 
			{
				m_pTable = 0;
				return;
			}

			m_list.push_back(m_pTable);
		}
		else m_pTable->ReadDatas(nCode,strData);
	}
}

LType* TableManager::GetLType()
{
	std::list<Table*>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Table* pTable = *p;
		if (!wcscmp(pTable->GetEntityName(),L"LTYPE")) 
		{
			return (LType*)pTable;
		}

		p++;
	}

	return 0;
}

DimStyle* TableManager::GetDimStyle()
{
	std::list<Table*>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Table* pTable = *p;
		if (!wcscmp(pTable->GetEntityName(),L"DIMSTYLE")) 
		{
			return (DimStyle*)pTable;
		}

		p++;
	}

	return 0;
}

Style* TableManager::GetStyle()
{
	std::list<Table*>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Table* pTable = *p;
		if (!wcscmp(pTable->GetEntityName(),L"STYLE")) 
		{
			return (Style*)pTable;
		}

		p++;
	}

	return 0;
}

Layer* TableManager::GetLayer()
{
	std::list<Table*>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Table* pTable = *p;
		if (!wcscmp(pTable->GetEntityName(),L"LAYER")) 
		{
			return (Layer*)pTable;
		}

		p++;
	}

	return 0;
}

VPort* TableManager::GetVPort()
{
	std::list<Table*>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Table* pTable = *p;
		if (!wcscmp(pTable->GetEntityName(),L"VPORT")) 
		{
			return (VPort*)pTable;
		}

		p++;
	}

	return 0;
}

BlockRecord* TableManager::GetBlockRecord()
{
	std::list<Table*>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Table* pTable = *p;
		if (!wcscmp(pTable->GetEntityName(),L"BLOCK_RECORD")) 
		{
			return (BlockRecord*)pTable;
		}

		p++;
	}

	return 0;
}

wchar_t* TableManager::GetDimBlockName()
{
	swprintf_s(m_strDimBlockName, L"*D%d", m_nDimBlockIndex++);
	return m_strDimBlockName;
}

// Handle Code(5)가 정수가 아닌 문자열일 경우에도 읽을수 있는가?
bool TableManager::ReadStringHandle()
{
	if (m_pTable != 0 && !wcscmp(m_pTable->GetEntityName(), L"DIMSTYLE"))
		return true;

	return false;
}

END_NS
END_NS
