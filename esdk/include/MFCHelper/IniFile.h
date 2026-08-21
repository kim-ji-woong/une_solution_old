#if !defined(AFX_INIFILE_H__026B6E63_7749_11D4_A80C_444553540000__INCLUDED_)
#define AFX_INIFILE_H__026B6E63_7749_11D4_A80C_444553540000__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CIniFile  
{
public:
	BOOL DeleteSection(CString strSec, CString strKey);
	CIniFile(CString m_FName);
	virtual ~CIniFile();

	CString m_Name;
	CString ReadString(CString m_Sec, CString m_Ident, CString m_Def);
	BOOL WriteString(CString m_Sec, CString m_Ident, CString m_Val);

	BOOL ReadSections(CStringArray& m_Secs);
	BOOL ReadSection(CString m_Sec, CStringArray& m_Secs);

	CString GetFirstParam(CString &mName, CString m_Deli = ",");
};

#endif // !defined(AFX_INIFILE_H__026B6E63_7749_11D4_A80C_444553540000__INCLUDED_)
