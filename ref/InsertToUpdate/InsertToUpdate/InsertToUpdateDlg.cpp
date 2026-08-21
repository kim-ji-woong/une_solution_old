
// InsertToUpdateDlg.cpp : 구현 파일
//

#include "stdafx.h"
#include "InsertToUpdate.h"
#include "InsertToUpdateDlg.h"
#include "afxdialogex.h"
#include "UnEUtility/DBUtility.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

using namespace UnE::Utility;

// 응용 프로그램 정보에 사용되는 CAboutDlg 대화 상자입니다.

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// 대화 상자 데이터입니다.
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.

// 구현입니다.
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CInsertToUpdateDlg 대화 상자




CInsertToUpdateDlg::CInsertToUpdateDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CInsertToUpdateDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CInsertToUpdateDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CInsertToUpdateDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON_TEXT, &CInsertToUpdateDlg::OnBnClickedButtonText)
	ON_BN_CLICKED(IDC_BUTTON_OPEN_FILE, &CInsertToUpdateDlg::OnBnClickedButtonOpenFile)
	ON_BN_CLICKED(IDC_BUTTON_OPEN_FILE2, &CInsertToUpdateDlg::OnBnClickedButtonOpenFile2)
	ON_BN_CLICKED(IDC_BUTTON_FILE, &CInsertToUpdateDlg::OnBnClickedButtonFile)
END_MESSAGE_MAP()


// CInsertToUpdateDlg 메시지 처리기

BOOL CInsertToUpdateDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 시스템 메뉴에 "정보..." 메뉴 항목을 추가합니다.

	// IDM_ABOUTBOX는 시스템 명령 범위에 있어야 합니다.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// 이 대화 상자의 아이콘을 설정합니다. 응용 프로그램의 주 창이 대화 상자가 아닐 경우에는
	//  프레임워크가 이 작업을 자동으로 수행합니다.
	SetIcon(m_hIcon, TRUE);			// 큰 아이콘을 설정합니다.
	SetIcon(m_hIcon, FALSE);		// 작은 아이콘을 설정합니다.

	// TODO: 여기에 추가 초기화 작업을 추가합니다.

	return TRUE;  // 포커스를 컨트롤에 설정하지 않으면 TRUE를 반환합니다.
}

void CInsertToUpdateDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// 대화 상자에 최소화 단추를 추가할 경우 아이콘을 그리려면
//  아래 코드가 필요합니다. 문서/뷰 모델을 사용하는 MFC 응용 프로그램의 경우에는
//  프레임워크에서 이 작업을 자동으로 수행합니다.

void CInsertToUpdateDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 그리기를 위한 디바이스 컨텍스트입니다.

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 클라이언트 사각형에서 아이콘을 가운데에 맞춥니다.
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 아이콘을 그립니다.
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// 사용자가 최소화된 창을 끄는 동안에 커서가 표시되도록 시스템에서
//  이 함수를 호출합니다.
HCURSOR CInsertToUpdateDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



void CInsertToUpdateDlg::OnBnClickedButtonText()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	CString strInsertQuery;
	GetDlgItemText(IDC_EDIT_INSERT, strInsertQuery);

	if (strInsertQuery.GetLength() == 0)
	{
		AfxMessageBox(L"먼저 Insert Query를 입력하세요");
	}
	else
	{
		std::wstring strUpdateQuery;
		
		if (!DBUtility::InsertToUpdateString((wchar_t*)(LPCTSTR)strInsertQuery, strUpdateQuery, 0))
		{
			AfxMessageBox(DBUtility::GetErrorMessage().c_str());
		}
		else
		{
			SetDlgItemText(IDC_EDIT_Update, strUpdateQuery.c_str());
		}
	}
}


void CInsertToUpdateDlg::OnBnClickedButtonOpenFile()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	wchar_t szFilters[]= L"All Files (*.*)|*.*||";

   // Create an Open dialog; the default file name extension is ".my".
   CFileDialog dlg(TRUE, L"*", L"*.*",
      OFN_FILEMUSTEXIST | OFN_HIDEREADONLY, szFilters);

   if (dlg.DoModal() == IDOK)
   {
	   SetDlgItemText(IDC_EDIT_INSERT_FILE_PATH, dlg.GetPathName());
   }
}


void CInsertToUpdateDlg::OnBnClickedButtonOpenFile2()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	wchar_t szFilters[]= L"All Files (*.*)|*.*||";

   // Create an Open dialog; the default file name extension is ".my".
   CFileDialog dlg(TRUE, L"*", L"*.*",
      OFN_FILEMUSTEXIST | OFN_HIDEREADONLY, szFilters);

   if (dlg.DoModal() == IDOK)
   {
	   SetDlgItemText(IDC_EDIT_UPDATE_FILE_PATH, dlg.GetPathName());
   }
}


void CInsertToUpdateDlg::OnBnClickedButtonFile()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	CString strInsertPath, strUpdatePath;
	GetDlgItemText(IDC_EDIT_INSERT_FILE_PATH, strInsertPath);
	GetDlgItemText(IDC_EDIT_UPDATE_FILE_PATH, strUpdatePath);

	if (strInsertPath.GetLength() == 0)
	{
		AfxMessageBox(L"먼저 Insert Query File의 경로를 입력하세요");
	}
	else if (strUpdatePath.GetLength() == 0)
	{
		AfxMessageBox(L"먼저 Update Query File의 경로를 입력하세요");
	}
	else
	{
		if (!DBUtility::InsertToUpdateFile((wchar_t*)(LPCTSTR)strInsertPath, (wchar_t*)(LPCTSTR)strUpdatePath, 0))
			AfxMessageBox(DBUtility::GetErrorMessage().c_str());
		else
			AfxMessageBox(L"File 변환 완료");
	}
}
