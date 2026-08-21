#if !defined(AFX_VIFindReplaceDlg_H__DB4801F8_4CA4_4747_B6F9_31A85BAC3845__INCLUDED_)
#define AFX_VIFindReplaceDlg_H__DB4801F8_4CA4_4747_B6F9_31A85BAC3845__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// VIFindReplaceDlg.h : header file
//

#define WM_FINDREPLACECMD	WM_USER+33
#ifndef IDD_FIND_N_REPLACE_V2
#define IDD_FIND_N_REPLACE_V2 0
#endif


/////////////////////////////////////////////////////////////////////////////
// VIFindReplaceDlg dialog

class AFX_EXT_CLASS VIFindReplaceDlg : public VIDialog
{
public:
	enum EFindReplaceCmdCode	//wparam when WM_FINDREPLACECMD is received by parent
	{
		eFind,
		eReplace,
		eReplaceAll,
		eMarkAll,
		eClose,
	};
// Construction
public:
	VIFindReplaceDlg(CWnd* pParent = NULL);   // standard constructor

	BOOL ToggleReplaceMode(BOOL iReplace,BOOL iForce=FALSE);

// Dialog Data
	//{{AFX_DATA(VIFindReplaceDlg)
	enum { IDD = IDD_FIND_N_REPLACE_V2 };
	VICheckButton	m_matchCaseCheck;
	VICheckButton	m_matchWholeWordCheck;
	VICheckButton	m_searchUpCheck;
	VICheckButton	m_keepOpenCheck;

	VIRadioButton	m_currentDocumentRadio;
	VIRadioButton	m_currentProjectRadio;
	VIRadioButton	m_selectionOnlyRadio;

	VIEdit			m_findEdit;
	VIEdit			m_replaceEdit;

	VIButton		m_FindNextButton;
	VIButton		m_ReplaceButton;
	VIButton		m_ReplaceAllButton;
	VIButton		m_MarkAllButton;
	VIButton		m_CloseButton;

	VIStaticText	m_FindStatic;
	VIStaticText	m_ReplaceStatic;

	CStatic			m_SearchStatic;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(VIFindReplaceDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual LRESULT WindowProc(UINT message, WPARAM wParam, LPARAM lParam);
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	//}}AFX_VIRTUAL

// Implementation
public:

	// Generated message map functions
	//{{AFX_MSG(VIFindReplaceDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnDestroy();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	CString	m_findStr;
	CString	m_replaceStr;
	BOOL	m_matchCase;
	BOOL	m_matchWholeWord;
	BOOL	m_searchUp;
	BOOL	m_keepOpen;


	BOOL	m_currentDocument;
	BOOL	m_currentProject;
	BOOL	m_selectionOnly;

	enum EFindReplaceMode
	{
		EFindMode,
		EReplaceMode,
	};
	EFindReplaceMode m_Mode;

	VIRichEdit*	m_fullScreenRichEdit;

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_VIFindReplaceDlg_H__DB4801F8_4CA4_4747_B6F9_31A85BAC3845__INCLUDED_)
