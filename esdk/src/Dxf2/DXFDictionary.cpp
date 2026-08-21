#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(OBJECTS)

std::wstring Dictionary::m_strSubClassName = L"AcDbDictionary";

Dictionary::Dictionary(ObjectManager* pMgr)
	: Object(pMgr)
{
	Init();
}

Dictionary::~Dictionary(void)
{
	std::list<Entry>::iterator pIter = m_listEntry.begin();
	std::list<Entry>::iterator pEnd = m_listEntry.end();

	while (pIter != pEnd)
	{
		delete pIter->m_pObj;
		pIter->m_pObj = 0;
		pIter++;
	}
}

void Dictionary::Init()
{
	m_strEntityType = L"DICTIONARY";

	AddData(0,(wchar_t*)m_strEntityType.data());
	AddData(5,&m_nHandle);
}

wchar_t* Dictionary::GetSubClassName()
{
	return (wchar_t*)m_strSubClassName.data();
}

// nHandleCode : 350 or 360
void Dictionary::AddEntry(Object* pObj, wchar_t* strEntryName, int nHandleCode)
{
	Entry ent;

	pObj->SetDictionaryHandle(m_nHandle);
	ent.m_pObj = pObj;
	ent.m_strEntryName = strEntryName;
	ent.m_nHandleCode  = nHandleCode;

	m_listEntry.push_back(ent);
}

// nBlockHandle과 연관된 Layout 객체의 핸들을 리턴한다.
int Dictionary::GetLayoutHandle(int nBlockHandle)
{
	std::list<Entry>::iterator p = m_listEntry.begin();

	while (p != m_listEntry.end())
	{
		Entry& rEnt = *p;

		if (rEnt.m_pObj)
		{
			if (!wcscmp(rEnt.m_strEntryName.data(),L"ACAD_LAYOUT") && !wcscmp(rEnt.m_pObj->GetEntityType(),L"DICTIONARY"))
			{
				Dictionary* pDictionary = (Dictionary*)rEnt.m_pObj;

				std::list<Entry>::iterator pIter = pDictionary->m_listEntry.begin();

				while (pIter != pDictionary->m_listEntry.end())
				{
					Entry& rEnt1 = *pIter;

					if (rEnt1.m_pObj)
					{
						if (!wcscmp(rEnt1.m_pObj->GetEntityType(),L"LAYOUT"))
						{
							Layout* pLayout = (Layout*)rEnt1.m_pObj;
							if (pLayout->GetBlockHandle() == nBlockHandle) return pLayout->GetHandle();
						}
					}

					pIter++;
				}
			}
		}

		++p;
	}

	return 0;
}

void Dictionary::Write(Utility::FileManager* pMgr)
{
	Object::Write(pMgr);

	std::list<Entry>::const_iterator p = m_listEntry.begin();
	while (p != m_listEntry.end())
	{
		Entry ent = *p;
		AddLine(pMgr,3,L"%s",(wchar_t*)ent.m_strEntryName.data());
		AddLine(pMgr,ent.m_nHandleCode,L"%X",ent.m_pObj->GetHandle());
		++p;
	}

	p = m_listEntry.begin();
	while (p != m_listEntry.end())
	{
		Entry ent = *p;
		ent.m_pObj->Write(pMgr);
		++p;
	}
}

/*void Dictionary::AddApplication(char* strAppName, int nHandleCode, int nAppHandle)
{
	std::list<DXFData>::iterator p = m_list.begin();
	bool bFlag = false, b102Finish = false;
	int n102Find = 0;

	while (p != m_list.end())
	{
		DXFData data = *p;

		if (data.nCode == 5) bFlag = true;
		else
		{
			if (bFlag)
			{
				if (data.nCode == 102) n102Find++;
				else
				{
					if ((n102Find % 2) == 0) b102Finish = true;
				}
			}
		}

		if (bFlag && b102Finish)
		{
			AddApplication(p,strAppName,nHandleCode,nAppHandle);
			return;
		}

		++p;
	}

	AddApplication(m_list.end(),strAppName,nHandleCode,nAppHandle);
}

void Dictionary::AddApplication(std::list<DXFData>::iterator p, char* strAppName, int nHandleCode, int nAppHandle)
{
	DXFData arrData[3];
	arrData[0].nCode = 102;
	arrData[0].str	 = "{";
	arrData[0].str	+= strAppName;
	arrData[1].nCode = nHandleCode;
	arrData[1].nData = nAppHandle;
	arrData[2].nCode = 102;
	arrData[2].str	 = "}";

	for (int i=2;i>=0;i--) p = m_list.insert(p,arrData[i]);
}*/

END_NS
END_NS
