// IniFile.cpp: implementation of the CIniFile class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "IniFile.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CIniFile::CIniFile(CString m_FName)
{
	m_Name = m_FName;
}

CIniFile::~CIniFile()
{
}

CString CIniFile::ReadString(CString m_Sec, CString m_Ident, CString m_Def)
{
	char Buffer[2048];

	GetPrivateProfileString(m_Sec, m_Ident,m_Def, Buffer, sizeof(Buffer), m_Name);
	return Buffer;
}

BOOL CIniFile::WriteString(CString m_Sec, CString m_Ident, CString m_Val)
{
	return WritePrivateProfileString(m_Sec, m_Ident, m_Val, m_Name);
}

BOOL CIniFile::ReadSections(CStringArray& m_Secs)
{
	LPVOID pvData = NULL;
	HGLOBAL hGlobal = GlobalAlloc(GMEM_MOVEABLE, 16385);
	_ASSERTE(NULL != hGlobal);

	pvData = GlobalLock(hGlobal);
	_ASSERTE(NULL != pvData);

	m_Secs.RemoveAll();
	
	if (GetPrivateProfileString(NULL, NULL, NULL, (char*) pvData, 16384, m_Name))
	{
        char *P = (char*) pvData;
        while (*P != 0)
		{
			m_Secs.Add(P);
			P += strlen(P) + 1;
		}
	}
	GlobalUnlock(hGlobal);
	GlobalFree(hGlobal);
	return m_Secs.GetSize() > 0;
}

BOOL CIniFile::ReadSection(CString m_Sec, CStringArray& m_Secs)
{
	LPVOID pvData = NULL;
	HGLOBAL hGlobal = GlobalAlloc(GMEM_MOVEABLE, 16385);
	_ASSERTE(NULL != hGlobal);

	pvData = GlobalLock(hGlobal);
	_ASSERTE(NULL != pvData);

	m_Secs.RemoveAll();
	
	if (GetPrivateProfileString(m_Sec, NULL, NULL, (char*) pvData, 16384, m_Name))
	{
        char *P = (char*) pvData;
        while (*P != 0)
		{
			m_Secs.Add(P);
			P += strlen(P) + 1;
		}
	}
	GlobalUnlock(hGlobal);
	GlobalFree(hGlobal);
	return m_Secs.GetSize() > 0;
}

BOOL CIniFile::DeleteSection(CString strSec, CString strKey)
{
	if(WritePrivateProfileString(strSec, strKey, NULL, m_Name))
		return TRUE;
	return FALSE;
}

CString CIniFile::GetFirstParam(CString &mName, CString m_Deli)
{
	int P = mName.Find(m_Deli);
	
	if (P == -1)
	{
		return mName;
	}
	
	CString szResult = mName.Mid(0, P);
	mName.Delete(0, P + 1);
	mName.TrimLeft();
	mName.TrimRight();
	
	return szResult;
}
