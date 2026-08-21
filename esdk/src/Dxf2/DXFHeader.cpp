#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(HEADER)

CHeader::CHeader()
{
	Init();
}

CHeader::~CHeader(void)
{
}

void CHeader::Clear()
{
	m_mapHeader.clear();
}

void CHeader::Init()
{
	m_mapHeader.clear();

	m_dAngBase = 0.0;
	m_nAngDir = 0;
	m_nAunits = 0;
	m_strCLayer = L"0";
	m_pVariable = 0;

	AddVariable(L"ACADVER",1,L"AC1015");
	AddVariable(L"ACADMAINTVER",70,20);
	AddVariable(L"DWGCODEPAGE",3,L"ANSI_949");
	AddVariable(L"INSBASE",10,0.0,20,0.0,30,0.0);
	AddVariable(L"EXTMIN",10,1240.791403982605,20,785.4249369202646,30,0.0);
	AddVariable(L"EXTMAX",10,2073.431948465598,20,1554.931978787252,30,0.0);
	AddVariable(L"LIMMIN",10,0.0,20,0.0);
	AddVariable(L"LIMMAX",10,420.0,20,297.0);
	AddVariable(L"ORTHOMODE",70,0);
	AddVariable(L"REGENMODE",70,1);
	AddVariable(L"FILLMODE",70,1);
	AddVariable(L"QTEXTMODE",70,0);
	AddVariable(L"MIRRTEXT",70,0);
	AddVariable(L"LTSCALE",40,1.0);
	AddVariable(L"ATTMODE",70,1);
	AddVariable(L"TEXTSIZE",40,2.5);
	AddVariable(L"TRACEWID",40,1.0);
	AddVariable(L"TEXTSTYLE",7,L"Standard");
	AddVariable(L"CLAYER",8,(wchar_t*)m_strCLayer.data());
	AddVariable(L"CELTYPE",6,L"ByLayer");
	AddVariable(L"CECOLOR",62,256);
	AddVariable(L"CELTSCALE",40,1.0);
	AddVariable(L"DISPSILH",70,0);
	AddVariable(L"DIMSCALE",40,100.0);
	AddVariable(L"DIMASZ",40,2.5);
	AddVariable(L"DIMEXO",40,0.625);
	AddVariable(L"DIMDLI",40,3.75);
	AddVariable(L"DIMRND",40,0.0);
	AddVariable(L"DIMDLE",40,0.0);
	AddVariable(L"DIMEXE",40,1.25);
	AddVariable(L"DIMTP",40,0.0);
	AddVariable(L"DIMTM",40,0.0);
	AddVariable(L"DIMTXT",40,2.5);
	AddVariable(L"DIMCEN",40,2.5);
	AddVariable(L"DIMTSZ",40,0.0);
	AddVariable(L"DIMTOL",70,0);
	AddVariable(L"DIMLIM",70,0);
	AddVariable(L"DIMTIH",70,0);
	AddVariable(L"DIMTOH",70,0);
	AddVariable(L"DIMSE1",70,0);
	AddVariable(L"DIMSE2",70,0);
	AddVariable(L"DIMTAD",70,1);
	AddVariable(L"DIMZIN",70,8);
	AddVariable(L"DIMBLK",1,L"");
	AddVariable(L"DIMASO",70,1);
	AddVariable(L"DIMSHO",70,1);
	AddVariable(L"DIMPOST",1,L"");
	AddVariable(L"DIMAPOST",1,L"");
	AddVariable(L"DIMALT",70,0);
	AddVariable(L"DIMALTD",70,3);
	AddVariable(L"DIMALTF",40,0.03937007874016);
	AddVariable(L"DIMLFAC",40,1.0);
	AddVariable(L"DIMTOFL",70,1);
	AddVariable(L"DIMTVP",40,0.0);
	AddVariable(L"DIMTIX",70,0);
	AddVariable(L"DIMSOXD",70,0);
	AddVariable(L"DIMSAH",70,0);
	AddVariable(L"DIMBLK1",1,L"");
	AddVariable(L"DIMBLK2",1,L"");
	AddVariable(L"DIMSTYLE",2,L"ISO-25");
	AddVariable(L"DIMCLRD",70,0);
	AddVariable(L"DIMCLRE",70,0);
	AddVariable(L"DIMCLRT",70,0);
	AddVariable(L"DIMTFAC",40,1.0);
	AddVariable(L"DIMGAP",40,0.625);
	AddVariable(L"DIMJUST",70,0);
	AddVariable(L"DIMSD1",70,0);
	AddVariable(L"DIMSD2",70,0);
	AddVariable(L"DIMTOLJ",70,0);
	AddVariable(L"DIMTZIN",70,8);
	AddVariable(L"DIMALTZ",70,0);
	AddVariable(L"DIMALTTZ",70,0);
	AddVariable(L"DIMUPT",70,0);
	AddVariable(L"DIMDEC",70,2);
	AddVariable(L"DIMTDEC",70,2);
	AddVariable(L"DIMALTU",70,2);
	AddVariable(L"DIMALTTD",70,3);
	AddVariable(L"DIMTXSTY",7,L"Standard");
	AddVariable(L"DIMAUNIT",70,0);
	AddVariable(L"DIMADEC",70,0);
	AddVariable(L"DIMALTRND",40,0.0);
	AddVariable(L"DIMAZIN",70,0);
	AddVariable(L"DIMDSEP",70,44);
	AddVariable(L"DIMATFIT",70,3);
	AddVariable(L"DIMFRAC",70,0);
	AddVariable(L"DIMLDRBLK",1,L"");
	AddVariable(L"DIMLUNIT",70,2);
	AddVariable(L"DIMLWD",70,-2);
	AddVariable(L"DIMLWE",70,-2);
	AddVariable(L"DIMTMOVE",70,0);
	AddVariable(L"LUNITS",70,2);
	AddVariable(L"LUPREC",70,4);
	AddVariable(L"SKETCHINC",40,1.0);
	AddVariable(L"FILLETRAD",40,0.0);
	AddVariable(L"AUNITS",70,m_nAunits);
	AddVariable(L"AUPREC",70,0);
	AddVariable(L"MENU",1,L".");
	AddVariable(L"ELEVATION",40,0.0);
	AddVariable(L"PELEVATION",40,0.0);
	AddVariable(L"THICKNESS",40,0.0);
	AddVariable(L"LIMCHECK",70,0);
	AddVariable(L"CHAMFERA",40,0.0);
	AddVariable(L"CHAMFERB",40,0.0);
	AddVariable(L"CHAMFERC",40,0.0);
	AddVariable(L"CHAMFERD",40,0.0);
	AddVariable(L"SKPOLY",70,0);
	AddVariable(L"TDCREATE",40,2453894.606139340);
	AddVariable(L"TDUCREATE",40,2453894.231139341);
	AddVariable(L"TDUPDATE",40,2453894.606535139);
	AddVariable(L"TDUUPDATE",40,2453894.231535139);
	AddVariable(L"TDINDWG",40,0.0004068287);
	AddVariable(L"TDUSRTIMER",40,0.0004066435);
	AddVariable(L"USRTIMER",70,1);
	AddVariable(L"ANGBASE",50,m_dAngBase);
	AddVariable(L"ANGDIR",70,m_nAngDir);
	AddVariable(L"PDMODE",70,0);
	AddVariable(L"PDSIZE",40,0.0);
	AddVariable(L"PLINEWID",40,0.0);
	AddVariable(L"SPLFRAME",70,0);
	AddVariable(L"SPLINETYPE",70,6);
	AddVariable(L"SPLINESEGS",70,8);
	AddVariable(L"HANDSEED",5,L"4FE");
	AddVariable(L"SURFTAB1",70,6);
	AddVariable(L"SURFTAB2",70,6);
	AddVariable(L"SURFTYPE",70,6);
	AddVariable(L"SURFU",70,6);
	AddVariable(L"SURFV",70,6);
	AddVariable(L"UCSBASE",2,L"");
	AddVariable(L"UCSNAME",2,L"");
	AddVariable(L"UCSORG",10,0.0,20,0.0,30,0.0);
	AddVariable(L"UCSXDIR",10,1.0,20,0.0,30,0.0);
	AddVariable(L"UCSYDIR",10,0.0,20,1.0,30,0.0);
	AddVariable(L"UCSORTHOREF",2,L"");
	AddVariable(L"UCSORTHOVIEW",70,0);
	AddVariable(L"UCSORGTOP",10,0.0,20,0.0,30,0.0);
	AddVariable(L"UCSORGBOTTOM",10,0.0,20,0.0,30,0.0);
	AddVariable(L"UCSORGLEFT",10,0.0,20,0.0,30,0.0);
	AddVariable(L"UCSORGRIGHT",10,0.0,20,0.0,30,0.0);
	AddVariable(L"UCSORGFRONT",10,0.0,20,0.0,30,0.0);
	AddVariable(L"UCSORGBACK",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PUCSBASE",2,L"");
	AddVariable(L"PUCSNAME",2,L"");
	AddVariable(L"PUCSORG",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PUCSXDIR",10,1.0,20,0.0,30,0.0);
	AddVariable(L"PUCSYDIR",10,0.0,20,1.0,30,0.0);
	AddVariable(L"PUCSORTHOREF",2,L"");
	AddVariable(L"PUCSORTHOVIEW",70,0);
	AddVariable(L"PUCSORGTOP",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PUCSORGBOTTOM",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PUCSORGLEFT",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PUCSORGRIGHT",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PUCSORGFRONT",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PUCSORGBACK",10,0.0,20,0.0,30,0.0);
	AddVariable(L"USERI1",70,0);
	AddVariable(L"USERI2",70,0);
	AddVariable(L"USERI3",70,0);
	AddVariable(L"USERI4",70,0);
	AddVariable(L"USERI5",70,0);
	AddVariable(L"USERR1",40,0.0);
	AddVariable(L"USERR2",40,0.0);
	AddVariable(L"USERR3",40,0.0);
	AddVariable(L"USERR4",40,0.0);
	AddVariable(L"USERR5",40,0.0);
	AddVariable(L"WORLDVIEW",70,1);
	AddVariable(L"SHADEDGE",70,3);
	AddVariable(L"SHADEDIF",70,70);
	AddVariable(L"TILEMODE",70,1);
	AddVariable(L"MAXACTVP",70,64);
	AddVariable(L"PINSBASE",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PLIMCHECK",70,0);
	AddVariable(L"PEXTMIN",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PEXTMAX",10,0.0,20,0.0,30,0.0);
	AddVariable(L"PLIMMIN",10,0.0,20,0.0);
	AddVariable(L"PLIMMAX",10,12.0,20,9.0);
	AddVariable(L"UNITMODE",70,0);
	AddVariable(L"VISRETAIN",70,1);
	AddVariable(L"PLINEGEN",70,0);
	AddVariable(L"PSLTSCALE",70,1);
	AddVariable(L"TREEDEPTH",70,3020);
	AddVariable(L"CMLSTYLE",2,L"Standard");
	AddVariable(L"CMLJUST",70,0);
	AddVariable(L"CMLSCALE",40,20.0);
	AddVariable(L"PROXYGRAPHICS",70,1);
	AddVariable(L"MEASUREMENT",70,1);
	AddVariable(L"CELWEIGHT",370,-1);
	AddVariable(L"ENDCAPS",280,0);
	AddVariable(L"JOINSTYLE",280,0);
	AddVariable(L"LWDISPLAY",290,0);
	AddVariable(L"INSUNITS",70,6);
	AddVariable(L"HYPERLINKBASE",1,L"");
	AddVariable(L"STYLESHEET",1,L"");
	AddVariable(L"XEDIT",290,1);
	AddVariable(L"CEPSNTYPE",380,0);
	AddVariable(L"PSTYLEMODE",290,1);
	AddVariable(L"FINGERPRINTGUID",2,L"{A55F154B-0DC0-4723-A30A-F061D2D89A62}");
	AddVariable(L"VERSIONGUID",2,L"{7DB39824-0F2B-4890-9004-F051DEEF5A37}");
	AddVariable(L"EXTNAMES",290,1);
	AddVariable(L"PSVPSCALE",40,0.0);
	AddVariable(L"OLESTARTUP",290,0);
}

void CHeader::AddVariable(wstring strVariable, int nCode1, double fValue)
{
	CData data;
	data.SetData(strVariable, nCode1, fValue);
	m_mapHeader.insert(make_pair(strVariable, data));
}

void CHeader::AddVariable(wstring strVariable, int nCode1, wstring strValue)
{
	CData data;
	data.SetData(strVariable, nCode1, strValue);
	m_mapHeader.insert(make_pair(strVariable, data));
}

void CHeader::AddVariable(wstring strVariable, int nCode1, double fValue1, int nCode2, double fValue2)
{
	CData data;
	data.SetData(strVariable, nCode1, fValue1, nCode2, fValue2);
	m_mapHeader.insert(make_pair(strVariable, data));
}

void CHeader::AddVariable(wstring strVariable, int nCode1, double fValue1, int nCode2, double fValue2, int nCode3, double fValue3)
{
	CData data;
	data.SetData(strVariable, nCode1, fValue1, nCode2, fValue2, nCode3, fValue3);
	m_mapHeader.insert(make_pair(strVariable, data));
}

bool CHeader::UpdateVariable(wstring strVariable, double fValue)
{
	bool result = false;
	map<wstring, CData>::iterator it;
	for(it = m_mapHeader.begin(); it != m_mapHeader.end(); it++)
	{
		if(strVariable == it->first)
		{
			result = it->second.UpdateData(fValue);
		}
	}

	return result;
}

bool CHeader::UpdateVariable(wstring strVariable, wstring strValue)
{
	bool result = false;
	map<wstring, CData>::iterator it;
	for(it = m_mapHeader.begin(); it != m_mapHeader.end(); it++)
	{
		if(strVariable == it->first)
		{
			result = it->second.UpdateData(strValue);
		}
	}

	return result;
}

bool CHeader::UpdateVariable(wstring strVariable, double fValue1, double fValue2)
{
	bool result = false;
	map<wstring, CData>::iterator it;
	for(it = m_mapHeader.begin(); it != m_mapHeader.end(); it++)
	{
		if(strVariable == it->first)
		{
			result = it->second.UpdateData(fValue1, fValue2);
		}
	}

	return result;
}

bool CHeader::UpdateVariable(wstring strVariable, double fValue1, double fValue2, double fValue3)
{
	bool result = false;
	map<wstring, CData>::iterator it;
	for(it = m_mapHeader.begin(); it != m_mapHeader.end(); it++)
	{
		if(strVariable == it->first)
		{
			result = it->second.UpdateData(fValue1, fValue2, fValue3);
		}
	}

	return result;
}

void CHeader::Write(Utility::FileManager* pMgr)
{
	wchar_t strBuff[256];
	swprintf_s(strBuff,L"%3d\r\nSECTION\r\n%3d\r\nHEADER\r\n", 0, 2);
	pMgr->Write(strBuff,0,FILE_CURRENT, Utility::FileManager::WRITE_REPLACE);

	map<wstring, CData>::iterator it;
	for(it = m_mapHeader.begin(); it != m_mapHeader.end(); it++)
	{
		CData data = it->second;
		data.Write(pMgr);
	}

	memset(strBuff, 0, 256);
	swprintf_s(strBuff,L"%3d\r\nENDSEC\r\n", 0);
	pMgr->Write(strBuff,0,FILE_CURRENT, Utility::FileManager::WRITE_REPLACE);

}

map<wstring, CData>& CHeader::GetHeader()
{
	return m_mapHeader;
}

void CHeader::ReadDatai(int nCode, int nData) 
{
	if (m_pVariable)
	{
		*(int*)m_pVariable = nData;

		if (m_pVariable == &m_nInsUnits)
			AddVariable(L"INSUNITS", 70, nData);
	}
}

void CHeader::ReadDatad(int nCode, double dData) 
{
	if (m_pVariable)
	{
		*(double*)m_pVariable = dData;
	}
}

void CHeader::ReadDatas(int nCode, wchar_t* strData) 
{
	if (nCode == 9)
	{
		if (!wcscmp(strData,L"$ANGBASE")) m_pVariable = &m_dAngBase;
		else if (!wcscmp(strData,L"$ANGDIR")) m_pVariable = &m_nAngDir;
		else if (!wcscmp(strData,L"$AUNITS")) m_pVariable = &m_nAunits;
		else if (!wcscmp(strData,L"$CLAYER")) m_pVariable = &m_strCLayer;
		else if (!wcscmp(strData,L"$INSUNITS")) m_pVariable = &m_nInsUnits;
		else m_pVariable = 0;
	}
	else
	{
		if (m_pVariable)
		{
			*(wstring*)m_pVariable = strData;
		}
	}
}

double CHeader::GetAngBase()
{
	return m_dAngBase;
}

int CHeader::GetAngDir()
{
	return m_nAngDir;
}

int CHeader::GetAunits()
{
	return m_nAunits;
}

wchar_t* CHeader::GetCurrentLayer()
{
	return (wchar_t*)m_strCLayer.data();
}

void CHeader::UpdateNextHandle()
{
	int nHandle = 0;

	if (m_pOwner != 0)
	{
		nHandle = m_pOwner->Get32BitHandle();
	}

	// 다음에 사용 가능한 핸들
	// 즉, 마지막으로 사용된 핸들 + 1이 된다.
	wchar_t strHandle[16];
	swprintf_s(strHandle, L"%X", nHandle);
	//swprintf_s(strHandle,L"%X", Get32BitHandle());
	UpdateVariable(L"HANDSEED",strHandle);
}

END_NS
END_NS
