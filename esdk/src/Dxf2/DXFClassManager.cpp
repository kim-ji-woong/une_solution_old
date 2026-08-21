#include "stdafx.h"

#define DEFAULT_CLASS_SIZE		22

BEGIN_NS(DXF)
BEGIN_NS(CLASSES)

ClassManager::DefaultClass g_defClass[DEFAULT_CLASS_SIZE] = 
{
	ClassManager::DefaultClass(L"ACDBDICTIONARYWDFLT",L"AcDbDictionaryWithDefault",0,0,0),
	ClassManager::DefaultClass(L"ACDBPLACEHOLDER",L"AcDbPlaceHolder",0,0,0),
	ClassManager::DefaultClass(L"ARCALIGNEDTEXT",L"AcDbArcAlignedText",0,0,1),
	ClassManager::DefaultClass(L"DICTIONARYVAR",L"AcDbDictionaryVar",0,0,0),
	ClassManager::DefaultClass(L"HATCH",L"AcDbHatch",0,0,1),
	ClassManager::DefaultClass(L"IDBUFFER",L"AcDbIdBuffer",0,0,0),
	ClassManager::DefaultClass(L"IMAGE",L"AcDbRasterImage",127,0,1),
	ClassManager::DefaultClass(L"IMAGEDEF",L"AcDbRasterImageDef",0,0,0),
	ClassManager::DefaultClass(L"IMAGEDEF_REACTOR",L"AcDbRasterImageDefReactor",1,0,0),
	ClassManager::DefaultClass(L"LAYER_INDEX",L"AcDbLayerIndex",0,0,0),
	ClassManager::DefaultClass(L"LAYOUT",L"AcDbLayout",0,0,0),
	ClassManager::DefaultClass(L"LWPOLYLINE",L"AcDbPolyline",0,0,1),
	ClassManager::DefaultClass(L"OBJECT_PTR",L"CAseDLPNTableRecord",1,0,0),
	ClassManager::DefaultClass(L"OLE2FRAME",L"AcDbOle2Frame",0,0,1),
	ClassManager::DefaultClass(L"PLOTSETTINGS",L"AcDbPlotSettings",0,0,0),
	ClassManager::DefaultClass(L"RASTERVARIABLES",L"AcDbRasterVariables",0,0,0),
	ClassManager::DefaultClass(L"RTEXT",L"RText",0,0,1),
	ClassManager::DefaultClass(L"SORTENTSTABLE",L"AcDbSortentsTable",0,0,0),
	ClassManager::DefaultClass(L"SPATIAL_INDEX",L"AcDbSpatialIndex",0,0,0),
	ClassManager::DefaultClass(L"SPATIAL_FILTER",L"AcDbSpatialFilter",0,0,0),
	ClassManager::DefaultClass(L"WIPEOUT",L"AcDbWipeout",127,0,1),
	ClassManager::DefaultClass(L"WIPEOUTVARIABLES",L"AcDbWipeoutVariables",0,0,0)
};

ClassManager::DefaultClass::DefaultClass(wchar_t* strDXFRecord, wchar_t* strCPPClass, int n90, int n280, int n281)
{
	wcscpy_s(m_strDXFRecord, 64, strDXFRecord);
	wcscpy_s(m_strCPPClass, 64, strCPPClass);
	m_n90 = n90;
	m_n280 = n280;
	m_n281 = n281;
}

ClassManager::ClassManager(void)
{
	Init();
}

ClassManager::~ClassManager(void)
{
}

void ClassManager::Init()
{
	m_list.push_back(g_defClass[0]);
	m_list.push_back(g_defClass[1]);
	m_list.push_back(g_defClass[10]);
	m_list.push_back(g_defClass[3]);
}

void ClassManager::Write(Utility::FileManager* pMgr)
{
	wchar_t strDefault[32];
	swprintf_s(strDefault,L"0\r\nSECTION\r\n2\r\nCLASSES\r\n");
	pMgr->Write(strDefault,0,FILE_CURRENT);

	std::list<DefaultClass>::const_iterator p = m_list.begin();

	while (p != m_list.end())
	{
		AddLine(pMgr,0,L"CLASS");
		AddLine(pMgr,1,L"%s",p->m_strDXFRecord);
		AddLine(pMgr,2,L"%s",p->m_strCPPClass);
		AddLine(pMgr,3,L"DXF Loader");
		AddLine(pMgr,90,L"%d",p->m_n90);
		AddLine(pMgr,280,L"%d",p->m_n280);
		AddLine(pMgr,281,L"%d",p->m_n281);
		p++;
	}

	AddLine(pMgr,0,L"ENDSEC");
}

void ClassManager::AddClassType(wchar_t* strClassName)
{
	int* pArrIndex = 0;
	int nArrSize = 0;

	if (!_wcsicmp(strClassName,L"Hatch"))
	{
		nArrSize  = 1;
		pArrIndex = new int;
		*pArrIndex = 17;
	}
	else if (!_wcsicmp(strClassName,L"Image"))
	{
		nArrSize  = 4;
		pArrIndex = new int[nArrSize];
		pArrIndex[0] = 15;
		pArrIndex[1] = 7;
		pArrIndex[2] = 8;
		pArrIndex[3] = 6;
	}
	else return;

	for (int i=0;i<nArrSize;i++)
	{
		bool bSame = false;
		std::list<DefaultClass>::const_iterator p = m_list.begin();

		while (p != m_list.end())
		{
			if (!wcscmp(p->m_strDXFRecord,g_defClass[pArrIndex[i]].m_strDXFRecord))
			{
				bSame = true;
				break;
			}
			++p;
		}

		if (!bSame) m_list.push_back(g_defClass[pArrIndex[i]]);
	}

	delete [] pArrIndex;
}

END_NS
END_NS
