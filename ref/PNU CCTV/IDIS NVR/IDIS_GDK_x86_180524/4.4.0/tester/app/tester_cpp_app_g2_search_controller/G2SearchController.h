// G2SearchController.h : header file
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// G2SearchControllerApp:
// See G2SearchController.cpp for the implementation of this class
//

class G2SearchControllerApp : public CWinApp
{
public:
	G2SearchControllerApp();

// Overrides
	public:
	virtual BOOL InitInstance();

// Implementation

	DECLARE_MESSAGE_MAP()

public:
	void WriteProcessID();

private:
	CString m_strResultFilePath;
};

extern G2SearchControllerApp theApp;