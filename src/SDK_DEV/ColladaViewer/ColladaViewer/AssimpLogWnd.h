#pragma once

#include "Assimp/LogStream.hpp"


class CMyLogStream : public Assimp::LogStream
{
public:
	/**	@brief	Implementation of the abstract method	*/
	void write(const char* message);
};

/////////////////////////////////////////////////////////////////////////////
// CLogEdit 창

class CLogEdit : public CRichEditCtrl
{
// 생성입니다.
public:
	CLogEdit();

// 구현입니다.
public:
	virtual ~CLogEdit();

protected:
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	afx_msg void OnEditCopy();
	afx_msg void OnEditClear();
	afx_msg void OnViewOutput();

	DECLARE_MESSAGE_MAP()
};



//////////////////////////////////////////////////////////////////////////
// CAssimpLogWnd
class CAssimpLogWnd : public CDockablePane
{
// 생성입니다.
public:
	CAssimpLogWnd();

	void UpdateFonts();

// 특성입니다.
protected:
	CMFCTabCtrl	m_wndTabs;

	CLogEdit m_wndOutputLog;
	//COutputList m_wndOutputDebug;
	//COutputList m_wndOutputFind;

protected:
	void FillBuildWindow();

	void AdjustHorzScroll(CListBox& wndListBox);

// 구현입니다.
public:
	virtual ~CAssimpLogWnd();

protected:
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSize(UINT nType, int cx, int cy);

	DECLARE_MESSAGE_MAP()

private:

	friend class CMyLogStream;
	friend INT_PTR CALLBACK LogDialogProc(HWND hwndDlg,UINT uMsg,
		WPARAM wParam,LPARAM lParam);

public:
	static CAssimpLogWnd* pInstance;
	inline static CAssimpLogWnd& Instance ()
	{
		return (*pInstance);
	}

	static void Delete();
	

	// initializes the log window
	void Init ();

	// Shows the log window
	void Show();

	// Clears the log window
	void Clear();

	// Save the log window to an user-defined file
	void Save();

	// write a line to the log window
	void WriteLine(const char* message);

	// Set the bUpdate member
	inline void SetAutoUpdate(bool b)
	{
		this->bUpdate = b;
	}

	// updates the log file
	void Update();

	
private:

	// Window handle
	HWND hwnd;

	// current text of the window (contains RTF tags)
	std::string szText;
	std::string szPlainText;

	// is the log window currently visible?
	bool bIsVisible;

	// Specified whether each new log message updates the log automatically
	bool bUpdate;


public:
	// associated log stream
	CMyLogStream* pcStream;
};

