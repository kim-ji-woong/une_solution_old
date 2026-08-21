#include "stdafx.h"
#include "ColladaApp.h"

#include "AssimpLogWnd.h"
#include "Resource.h"
#include "MainFrm.h"

#include "LogDisplay.h"

#include "RichEdit.h"
#include "Assimp/types.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace AssimpView;

CAssimpLogWnd* CAssimpLogWnd::pInstance = NULL;

// header for the RTF log file
static const char* AI_VIEW_RTF_LOG_HEADER =
	"{\\rtf1" 
	"\\ansi" 
	"\\deff0"
	"{" 
	"\\fonttbl{\\f0 Courier New;}"
	"}" 
	"{\\colortbl;" 
	"\\red255\\green0\\blue0;" 	  // red for errors
	"\\red255\\green120\\blue0;"  // orange for warnings
	"\\red0\\green150\\blue0;" 	  // green for infos
	"\\red0\\green0\\blue180;" 	  // blue for debug messages
	"\\red0\\green0\\blue0;" 	  // black for everything else
	"}}";


/////////////////////////////////////////////////////////////////////////////
// CMyLogStream

void CMyLogStream::write(const char* message)
{
	CAssimpLogWnd::Instance().WriteLine(message);
}



/////////////////////////////////////////////////////////////////////////////
// CLogEdit

CLogEdit::CLogEdit()
{

}

CLogEdit::~CLogEdit()
{
}

BEGIN_MESSAGE_MAP(CLogEdit, CListBox)
	ON_WM_CONTEXTMENU()
	ON_COMMAND(ID_EDIT_COPY, OnEditCopy)
	ON_COMMAND(ID_EDIT_CLEAR, OnEditClear)
	ON_COMMAND(ID_VIEW_OUTPUTWND, OnViewOutput)
	ON_WM_WINDOWPOSCHANGING()
END_MESSAGE_MAP()
/////////////////////////////////////////////////////////////////////////////
// COutputList 메시지 처리기

void CLogEdit::OnContextMenu(CWnd* /*pWnd*/, CPoint point)
{
	CMenu menu;
	menu.LoadMenu(IDR_OUTPUT_POPUP);

	CMenu* pSumMenu = menu.GetSubMenu(0);

	if (AfxGetMainWnd()->IsKindOf(RUNTIME_CLASS(CMDIFrameWndEx)))
	{
		CMFCPopupMenu* pPopupMenu = new CMFCPopupMenu;

		if (!pPopupMenu->Create(this, point.x, point.y, (HMENU)pSumMenu->m_hMenu, FALSE, TRUE))
			return;

		((CMDIFrameWndEx*)AfxGetMainWnd())->OnShowPopupMenu(pPopupMenu);
		UpdateDialogControls(this, FALSE);
	}

	SetFocus();
}

void CLogEdit::OnEditCopy()
{
	MessageBox(_T("출력 복사"));
}

void CLogEdit::OnEditClear()
{
	MessageBox(_T("출력 지우기"));
}

void CLogEdit::OnViewOutput()
{
	CDockablePane* pParentBar = DYNAMIC_DOWNCAST(CDockablePane, GetOwner());
	CMDIFrameWndEx* pMainFrame = DYNAMIC_DOWNCAST(CMDIFrameWndEx, GetTopLevelFrame());

	if (pMainFrame != NULL && pParentBar != NULL)
	{
		pMainFrame->SetFocus();
		pMainFrame->ShowPane(pParentBar, FALSE, FALSE, FALSE);
		pMainFrame->RecalcLayout();

	}
}
//////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
// COutputBar

CAssimpLogWnd::CAssimpLogWnd()
{
	pInstance = this;
	this->szText = AI_VIEW_RTF_LOG_HEADER;
	this->szPlainText = "";
}

CAssimpLogWnd::~CAssimpLogWnd()
{
	if( pcStream != NULL)
		delete pcStream;
}

BEGIN_MESSAGE_MAP(CAssimpLogWnd, CDockablePane)
	ON_WM_CREATE()
	ON_WM_SIZE()
END_MESSAGE_MAP()

void CAssimpLogWnd::Init()
{

	CAssimpLogWnd::Instance().pcStream = new CMyLogStream();	
}


int CAssimpLogWnd::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CDockablePane::OnCreate(lpCreateStruct) == -1)
		return -1;

	CRect rectDummy;
	rectDummy.SetRectEmpty();
	if (!m_wndTabs.Create(CMFCTabCtrl::STYLE_FLAT, rectDummy, this, 1))
	{
		TRACE0("출력 탭 창을 만들지 못했습니다.\n");
		return -1;   
	}

	const DWORD dwStyle = LBS_NOINTEGRALHEIGHT | WS_CHILD | WS_VISIBLE | WS_HSCROLL | WS_VSCROLL | ES_MULTILINE | ES_READONLY | ES_AUTOVSCROLL;
	if (!m_wndOutputLog.Create(dwStyle, rectDummy, &m_wndTabs, 2))// ||
	{
		TRACE0("출력 창을 만들지 못했습니다.\n");
		return -1;      
	}

	DWORD nCurStyle = m_wndOutputLog.GetExStyle();
	m_wndOutputLog.ModifyStyleEx(0, nCurStyle | ES_MULTILINE| ES_WANTRETURN | ES_READONLY, 0 );
	UpdateFonts();

	CString strTabName;
	BOOL bNameValid;	
	bNameValid = strTabName.LoadString(IDS_BUILD_TAB);
	ASSERT(bNameValid);
	m_wndTabs.AddTab(&m_wndOutputLog, _T("Log"), (UINT)0);



	return 0;
}

void CAssimpLogWnd::OnSize(UINT nType, int cx, int cy)
{
	CDockablePane::OnSize(nType, cx, cy);

	m_wndTabs.SetWindowPos (NULL, -1, -1, cx, cy, SWP_NOMOVE | SWP_NOACTIVATE | SWP_NOZORDER);
}

void CAssimpLogWnd::AdjustHorzScroll(CListBox& wndListBox)
{
	CClientDC dc(this);
	CFont* pOldFont = dc.SelectObject(&afxGlobalData.fontRegular);

	int cxExtentMax = 0;

	for (int i = 0; i < wndListBox.GetCount(); i ++)
	{
		CString strItem;
		wndListBox.GetText(i, strItem);

		cxExtentMax = std::max<int>(cxExtentMax, dc.GetTextExtent(strItem).cx);
	}

	wndListBox.SetHorizontalExtent(cxExtentMax);
	dc.SelectObject(pOldFont);
}

void CAssimpLogWnd::FillBuildWindow()
{
	SetAutoUpdate(true);
	WriteLine("info: abc");
	WriteLine("info: 한글");
	WriteLine("warn: abc");
	Update();

}

void CAssimpLogWnd::UpdateFonts()
{
	m_wndOutputLog.SetFont(&afxGlobalData.fontRegular);
	//m_wndOutputDebug.SetFont(&afxGlobalData.fontRegular);
	//m_wndOutputFind.SetFont(&afxGlobalData.fontRegular);
}



void CAssimpLogWnd::WriteLine(const char* message)
{
	this->szPlainText.append(message);
	this->szPlainText.append("\r\n");

	if (0 != this->szText.length())
	{
		this->szText.resize(this->szText.length()-1);
	}

	switch (message[0])
	{
	case 'e': 
	case 'E':
		this->szText.append("{\\pard \\cf1 \\b \\fs18 ");
		break;
	case 'w': 
	case 'W':
		this->szText.append("{\\pard \\cf2 \\b \\fs18 ");
		break;
	case 'i': 
	case 'I':
		this->szText.append("{\\pard \\cf3 \\b \\fs18 ");
		break;
	case 'd': 
	case 'D':
		this->szText.append("{\\pard \\cf4 \\b \\fs18 ");
		break;
	default:
		this->szText.append("{\\pard \\cf5 \\b \\fs18 ");
		break;
	}

	std::string _message = message;
	for (unsigned int i = 0; i < _message.length();++i)
	{
		if ('\\' == _message[i] ||
			'}'  == _message[i] ||
			'{'  == _message[i])
		{
			_message.insert(i++,"\\");
		}
	}

	this->szText.append(_message);
	this->szText.append("\\par}}");

	if (this->bIsVisible && this->bUpdate)
	{
		SETTEXTEX sInfo;
		sInfo.flags = ST_DEFAULT;
		sInfo.codepage = CP_ACP;

		m_wndOutputLog.SendMessage(EM_SETTEXTEX, (WPARAM)&sInfo,( LPARAM)this->szText.c_str());
		//SendDlgItemMessage(this->hwnd,IDC_EDIT1,
		//	EM_SETTEXTEX,(WPARAM)&sInfo,( LPARAM)this->szText.c_str());
	}
	return;
}


void CAssimpLogWnd::Show()
{
	if (this->hwnd)
	{
		//ShowWindow(this->hwnd,SW_SHOW);
		//this->bIsVisible = true;

		// contents aren't updated while the logger isn't displayed
		this->Update();
	}
}

//-------------------------------------------------------------------------------
void CAssimpLogWnd::Clear()
{
	this->szText = AI_VIEW_RTF_LOG_HEADER;;
	this->szPlainText = "";

	this->Update();
}
//-------------------------------------------------------------------------------
void CAssimpLogWnd::Update()
{
	if (this->bIsVisible)
	{
		SETTEXTEX sInfo;
		sInfo.flags = ST_DEFAULT;
		sInfo.codepage = CP_ACP;

		m_wndOutputLog.SendMessage(EM_SETTEXTEX,(WPARAM)&sInfo,( LPARAM)this->szText.c_str());
		//SendDlgItemMessage(this->hwnd,IDC_EDIT1,
		//	EM_SETTEXTEX,(WPARAM)&sInfo,( LPARAM)this->szText.c_str());
	}
}
//-------------------------------------------------------------------------------
void CAssimpLogWnd::Save()
{
#ifdef UNICODE
	wchar_t szFileName[MAX_PATH];
#else
	char szFileName[MAX_PATH];
#endif

	DWORD dwTemp = MAX_PATH;
	if(ERROR_SUCCESS != RegQueryValueEx(GetRootReg(),_T("LogDestination"),NULL,NULL,
		(BYTE*)szFileName,&dwTemp))
	{
		// Key was not found. Use C:
		_tcscpy(szFileName, _T(""));
	}
	else
	{

#ifdef UNICODE 
		// need to remove the file name
		wchar_t* sz = _tcsrchr(szFileName,'\\');
#else
		char* sz = _tcsrchr(szFileName,'\\');
#endif
		if (!sz)
			sz = _tcsrchr(szFileName,'/');
		if (!sz)
			*sz = 0;
	}


	OPENFILENAME sFilename1 = {
		sizeof(OPENFILENAME),
		theApp.GetMainWnd()->GetSafeHwnd() ,
		GetModuleHandle(NULL), 
		_T("Log files\0*.txt"), NULL, 0, 1, 
		szFileName, MAX_PATH, NULL, 0, NULL, 
		_T("Save log to file"),
		OFN_OVERWRITEPROMPT | OFN_HIDEREADONLY | OFN_NOCHANGEDIR, 
		0, 1, _T(".txt"), 0, NULL, NULL
	};
	if(GetSaveFileName(&sFilename1) == 0) return;

	// Now store the file in the registry
	RegSetValueExA(GetRootReg(),"LogDestination",0,REG_SZ,(const BYTE*)szFileName,MAX_PATH);

#ifdef UNICODE
	FILE* pFile = _wfopen(szFileName, _T("wt"));
#else
	FILE* pFile = fopen(szFileName, _T("wt"));
#endif
	fprintf(pFile, this->szPlainText.c_str());
	fclose(pFile);

	CLogDisplay::Instance().AddEntry("[INFO] The log file has been saved",
		D3DCOLOR_ARGB(0xFF,0xFF,0xFF,0));
}

void CAssimpLogWnd::Delete()
{
	if( pInstance != NULL)
		delete pInstance;
}

