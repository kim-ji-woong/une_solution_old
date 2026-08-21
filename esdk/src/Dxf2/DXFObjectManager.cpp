#include "stdafx.h"

#ifdef GetObject
#undef GetObject
#endif

BEGIN_NS(DXF)
BEGIN_NS(OBJECTS)

ObjectManager::ObjectManager(BLOCKS::BlockManager* pBlkMgr)
{
	m_pMgr = 0;
	SetBlockManager(pBlkMgr);
	Init();
}

ObjectManager::~ObjectManager(void)
{
	Clear();
	m_bDeleted = true;
}

void ObjectManager::Clear()
{
	if (m_bDeleted) return;
	if (m_pOwner)
	{
		if (m_pOwner->GetObjectManager() != this) return;
	}
	std::list<Object*>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		Object* pObj = *p;
		p++;
		delete pObj;
	}

	m_list.clear();
	m_pObject = 0;
	m_hasLayout = false;
}

void ObjectManager::SetBlockManager(BLOCKS::BlockManager* pBlkMgr)
{
	m_pBlkMgr = pBlkMgr;
}

template <class T>
T* ObjectManager::GetGroupDictionary(int nDictionaryHandle)
{
	int nDuplicateFlag = 1;
	T* pDic = new T(this);

	pDic->AddData(102,L"{ACAD_REACTORS");
	pDic->AddData(330,&nDictionaryHandle);
	pDic->AddData(102,L"}");
	pDic->AddData(330,&nDictionaryHandle);
	pDic->AddData(100,Dictionary::GetSubClassName());
	pDic->AddData(281,&nDuplicateFlag);

	return pDic;
}

Layout* ObjectManager::GetDefaultLayout(int nDictionaryHandle, wchar_t* strLayoutName, int nOrder, bool bPrimary)
{
	wchar_t strBlockName[256];
	int nBlockHandle;
	m_pBlkMgr->GetBlockInfo(strBlockName,&nBlockHandle,nOrder);

	Layout* pLayout = new Layout(strLayoutName,nOrder,nBlockHandle, this);
	pLayout->SetDictionaryHandle(nDictionaryHandle);

	PlotSettings plot(this);

	if (bPrimary)
	{
		plot.SetDevicePath(L"none_device");
		plot.SetPaperSize(210.0,297.0);
		plot.SetMargin(7.5,7.5,20.0,20.0);
		plot.SetOrigin(11.54999923706054,-13.65000009536743);
		plot.SetPrintScale(1.0,8.704084754739808);
		plot.SetLayoutFlag(UseStandardScale | PlotPlotStyles | PrintLineweights | DrawViewportsFirst | ModelType | UpdatePaper | Initializing);
		plot.SetPaperUnits(MILLIMETERS);
		plot.SetPlotType(LastScreenDisplay);
		plot.SetScaleType(0);
		plot.SetFloatingPointScale(0.1148885871608098);
	}
	else
	{
		plot.SetDevicePath(L"");
	}

	plot.SetData();
	pLayout->AddPlotSettings(&plot);
	pLayout->SetData();

	return pLayout;
}

Dictionary* ObjectManager::GetLayoutDictionary(int nDictionaryHandle)
{
	Dictionary* pDic = GetGroupDictionary<Dictionary>(nDictionaryHandle);
	int nHandle = pDic->GetHandle();

	Layout* pLayout1 = GetDefaultLayout(nHandle,L"Model",0,true);
	Layout* pLayout2 = GetDefaultLayout(nHandle,L"배치1",1);
	Layout* pLayout3 = GetDefaultLayout(nHandle,L"배치2",2);

	pDic->AddEntry(pLayout1,pLayout1->GetLayoutName(),350);
	pDic->AddEntry(pLayout2,pLayout2->GetLayoutName(),350);
	pDic->AddEntry(pLayout3,pLayout3->GetLayoutName(),350);

	return pDic;
}

Dictionary* ObjectManager::GetMLineStyleDictionary(int nDictionaryHandle)
{
	Dictionary* pDic = GetGroupDictionary<Dictionary>(nDictionaryHandle);
	MLineStyle* pMLine = new MLineStyle(L"Standard", this);

	pMLine->SetDictionaryHandle(pDic->GetHandle());
	pMLine->SetData();
	pDic->AddEntry(pMLine,pMLine->GetStyleName(),350);

	return pDic;
}

Dictionary* ObjectManager::GetPlotSettingDictionary(int nDictionaryHandle)
{
	return GetGroupDictionary<Dictionary>(nDictionaryHandle);
}

Dictionary* ObjectManager::GetPlotStyleNameDictionary(int nDictionaryHandle)
{
	ACDBDictionaryWDFLT* pDic = GetGroupDictionary<ACDBDictionaryWDFLT>(nDictionaryHandle);
	int nHandle = m_pOwner == 0 ? 0 : m_pOwner->Get32BitHandle();
	//int nHandle = Get32BitHandle();

	pDic->AddData(3,L"Normal");
	pDic->AddData(350,&nHandle);
	pDic->AddData(100,L"AcDbDictionaryWithDefault");
	pDic->AddData(340,&nHandle);

	return pDic;
}

void ObjectManager::Init()
{
	Dictionary* pDic = new Dictionary(this);
	int nHandle = pDic->GetHandle();
	int nDictionaryHandle = pDic->GetDictionaryHandle();
	int nDuplicateFlag = 1;

	pDic->AddData(330,&nDictionaryHandle);
	pDic->AddData(100,L"AcDbDictionary");
	pDic->AddData(281,&nDuplicateFlag);

	Dictionary* pDic1 = GetGroupDictionary<Dictionary>(nHandle);
	Dictionary* pDic2 = GetLayoutDictionary(nHandle);
	Dictionary* pDic3 = GetMLineStyleDictionary(nHandle);
	Dictionary* pDic4 = GetPlotSettingDictionary(nHandle);
	//Dictionary* pDic5 = GetPlotStyleNameDictionary(nHandle);

	pDic->AddEntry(pDic1,L"ACAD_GROUP",350);
	pDic->AddEntry(pDic2,L"ACAD_LAYOUT",350);
	pDic->AddEntry(pDic3,L"ACAD_MLINESTYLE",350);
	pDic->AddEntry(pDic4,L"ACAD_PLOTSETTINGS",350);
	//pDic->AddEntry(pDic5,"ACAD_PLOTSTYLENAME",350);

	m_list.push_back(pDic);

	m_pObject = 0;
	m_hasLayout = false;
}

void ObjectManager::Write(Utility::FileManager* pMgr)
{
	AddLine(pMgr,0,L"SECTION");
	AddLine(pMgr,2,L"OBJECTS");

	std::list<Object*>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		Object* pObj = *p;
		pObj->Write(pMgr);
		++p;
	}

	AddLine(pMgr,0,L"ENDSEC");
}

void ObjectManager::AddObject(Object* pObj)
{
	m_list.push_back(pObj);
}

// nBlockHandle과 연관된 Layout 객체의 핸들을 리턴한다.
int ObjectManager::GetLayoutHandle(int nBlockHandle)
{
	std::list<Object*>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		Object* pObj = *p;

		if (!wcscmp(pObj->GetEntityType(),L"DICTIONARY"))
		{
			Dictionary* pDictionary = (Dictionary*)pObj;
			int nHandle = pDictionary->GetLayoutHandle(nBlockHandle);
			if (nHandle) return nHandle;
		}
		else if (!wcscmp(pObj->GetEntityType(),L"LAYOUT"))
		{
			Layout* pLayout = (Layout*)pObj;
			if (pLayout->GetBlockHandle() == nBlockHandle)
			{
				return pLayout->GetHandle();
			}
		}

		++p;
	}

	return 0;
}

void ObjectManager::ReadDatai(int nCode, int nData)
{
	if (m_pObject)
		m_pObject->ReadDatai(nCode, nData);
}

void ObjectManager::ReadDatad(int nCode, double dData)
{
	if (m_pObject)
		m_pObject->ReadDatad(nCode, dData);
}

void ObjectManager::ReadDatas(int nCode, wchar_t* strData)
{
	if (nCode == 0)
	{
		if (!wcscmp(strData, L"LAYOUT"))
		{
			if (!m_hasLayout)
			{
				m_pObject = new Layout(this);
				m_hasLayout = true;
				AddObject(m_pObject);
			}
			else
				m_pObject = 0;
		}
		else
			m_pObject = 0;
	}
	else
	{
		if (m_pObject)
			m_pObject->ReadDatas(nCode, strData);
	}
}

// pID : Object 정보를 담고 있는 링크드 리스트 노드의 포인터
Object* ObjectManager::GetObject(void*& pID)
{
	//static std::list<Object*>::iterator p;
	std::list<Object*>::iterator& p = m_objIter;

	if (pID == 0) p = m_list.begin();
	else
	{
		p = *(std::list<Object*>::iterator*)pID;
	}

	if (p != m_list.end())
	{
		Object* pObject = *p;
		p++;
		pID = &p;

		if (pObject->GetHandle() < 0) return 0;
		return pObject;
	}

	return 0;
}

void ObjectManager::SetDXFManager(DXFManager* pMgr)
{
	m_pMgr = pMgr;
}

DXFManager* ObjectManager::GetManager()
{
	return m_pMgr;
}

END_NS
END_NS
