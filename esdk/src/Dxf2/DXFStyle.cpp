#include "stdafx.h"
#include "UnEUtility/StringManager.h"

BEGIN_NS(DXF)
BEGIN_NS(TABLES)

Style::Entity::Entity(Style* pTable, const wchar_t* strStyleName, const wchar_t* strFontName)
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
	wcscpy_s(m_strStyleName, 256,strStyleName);
	//m_nHandle = Get32BitHandle();
	m_nFlag = 0;
	m_dFixedHeight = 0.0;
	m_dWidthFactor = 1.0;
	m_dObliqueAngle = 0.0;
	m_nGenerationFlag = 0;
	m_dLastHeight = 2.5;

	//if (!stricmp(strStyleName,"Standard"))
	if (strFontName == 0)
	{
		wcscpy_s(m_strPrimaryFontFile, 256, L"txt.shx");
		wcscpy_s(m_strBigFontFile, 256, L"whgtxt.shx");
		wcscpy_s(m_strFontName, 256, L"");
	}
	else
	{
		swprintf_s(m_strPrimaryFontFile, 256, L"%s.ttf",strFontName);
		wcscpy_s(m_strBigFontFile, 256, L"");
		wcscpy_s(m_strFontName, 256, strFontName);
	}
}

void Style::Entity::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"%s",(wchar_t*)m_pParent->m_strEntityName.data());
	AddLine(pMgr,5,L"%X",m_nHandle);
	AddLine(pMgr,330,L"%X",m_pParent->m_nHandle);
	AddLine(pMgr,100,L"%s",m_pParent->m_strDefSubClassName);
	AddLine(pMgr,100,L"%s",m_pParent->m_strSubClassName);
	AddLine(pMgr,2,L"%s",m_strStyleName);
	AddLine(pMgr,70,L"%d",m_nFlag);
	AddLine(pMgr,40,L"%lf",m_dFixedHeight);
	AddLine(pMgr,41,L"%lf",m_dWidthFactor);
	AddLine(pMgr,50,L"%lf",m_dObliqueAngle);
	AddLine(pMgr,71,L"%d",m_nGenerationFlag);
	AddLine(pMgr,42,L"%lf",m_dLastHeight);
	AddLine(pMgr,3,L"%s",m_strPrimaryFontFile);
	AddLine(pMgr,4,L"%s",m_strBigFontFile);

	if (_wcsicmp(m_strStyleName,L"Standard"))
	{
		AddLine(pMgr,1001,L"ACAD");
		AddLine(pMgr,1000,L"%s",m_strFontName);
		AddLine(pMgr,1071,L"33073");
	}
}

wchar_t* Style::Entity::GetStyleName()
{
	return m_strStyleName;
}

int Style::Entity::GetHandle()
{
	return m_nHandle;
}

const wchar_t* Style::Entity::GetFontName()
{
	if (wcslen(m_strFontName) == 0)
	{
		int len = (int)wcslen(m_strBigFontFile);

		for (int i=len-1;i>=0;i--)
		{
			if (m_strBigFontFile[i] == '.')
			{
				wcscpy_s(m_strFontName,m_strBigFontFile);
				m_strFontName[i] = 0;
				return m_strFontName;
			}
		}

		return m_strBigFontFile;
	}

	return m_strFontName;
}

Style::Style(TableManager* pMgr)
	: Table(pMgr)
{
	Init();
}

Style::~Style(void)
{
}

void Style::Init()
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
	m_strEntityName = L"STYLE";
	m_strSubClassName = L"AcDbTextStyleTableRecord";
	m_nEntitySize = 1;

	m_list.push_back(Entity(this,L"Standard"));
	m_pEntity = 0;
}

void Style::AddEntity(const wchar_t* strStyleName, const wchar_t* strFontName)
{
	m_list.push_back(Entity(this,strStyleName,strFontName));
	m_nEntitySize++;
}

void Style::Write(Utility::FileManager* pMgr)
{
	Table::Write(pMgr);

	std::list<Entity>::iterator p = m_list.begin();
	while (p != m_list.end())
	{
		Entity& rEntity = *p;
		rEntity.Write(pMgr);
		++p;
	}

	AddLine(pMgr,0,L"ENDTAB");
}

Style::Entity* Style::GetEntity(const wchar_t* strStyleName)
{
	std::list<Entity>::iterator p = m_list.begin();
	std::list<Entity>::iterator pEnd = m_list.end();

	while (p != pEnd)
	{
		Entity& rEntity = *p;
		if (!wcscmp(rEntity.GetStyleName(),strStyleName)) return &rEntity;
		++p;
	}

	return 0;
}

void Style::ReadDatai(int nCode, int nData)
{}

void Style::ReadDatad(int nCode, double dData)
{}

void Style::ReadDatas(int nCode, wchar_t* strData)
{
	switch (nCode)
	{
	case 0:
		if (!_wcsicmp(strData,L"STYLE"))
		{
			m_nEntitySize++;
			m_list.push_back(Entity(this,L"Temp"));
			std::list<Entity>::iterator pIter = m_list.end();pIter--;
			m_pEntity = &(*pIter);
		}
		break;

	case 5:
		if (m_pEntity)
		{
			int nHandle;
			if (UnE::Utility::StringManager::HexToInt(strData,&nHandle))
			{
				m_pEntity->m_nHandle = nHandle;
			}
		}
		break;

	case 2:
		if (m_pEntity)
		{
			wcscpy_s(m_pEntity->m_strStyleName, 256, strData);
		}
		break;

	case 3:
		if (m_pEntity)
		{
			wcscpy_s(m_pEntity->m_strBigFontFile, 256, strData);
		}
		break;

	case 4:
		if (m_pEntity)
		{
			wcscpy_s(m_pEntity->m_strPrimaryFontFile, 256, strData);
		}
		break;

	case 1000:
		if (m_pEntity)
		{
			wcscpy_s(m_pEntity->m_strFontName, 256, strData);
		}
		break;
	}
}

END_NS
END_NS
