
// TestAlrimiSwitchDlg.cpp : 구현 파일
//

#include "stdafx.h"
#include "TestAlrimiSwitch.h"
#include "TestAlrimiSwitchDlg.h"
#include "AlrimiSwitchProtect.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

#define	TIMER_PROTECT		0


// 응용 프로그램 정보에 사용되는 CAboutDlg 대화 상자입니다.

class CAboutDlg : public CDialog
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

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CTestAlrimiSwitchDlg 대화 상자

CTestAlrimiSwitchDlg::CTestAlrimiSwitchDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CTestAlrimiSwitchDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CTestAlrimiSwitchDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CTestAlrimiSwitchDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_TIMER()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDOK, &CTestAlrimiSwitchDlg::OnBnClickedOk)
	ON_BN_CLICKED(IDC_BTN_START, &CTestAlrimiSwitchDlg::OnBnClickedBtnSTART)
	ON_BN_CLICKED(IDC_BTN_STOP, &CTestAlrimiSwitchDlg::OnBnClickedBtnSTOP)
END_MESSAGE_MAP()


// CTestAlrimiSwitchDlg 메시지 처리기
void CTestAlrimiSwitchDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// 대화 상자에 최소화 단추를 추가할 경우 아이콘을 그리려면
//  아래 코드가 필요합니다. 문서/뷰 모델을 사용하는 MFC 응용 프로그램의 경우에는
//  프레임워크에서 이 작업을 자동으로 수행합니다.

void CTestAlrimiSwitchDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 그리기를 위한 디바이스 컨텍스트

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
		CDialog::OnPaint();
	}
}

// 사용자가 최소화된 창을 끄는 동안에 커서가 표시되도록 시스템에서
//  이 함수를 호출합니다.
HCURSOR CTestAlrimiSwitchDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

BOOL CTestAlrimiSwitchDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

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
	////////////////////////////////////////////////////////////////////////
	// 1) 오디오 초기화
	if(InitAudioOut() != AUD_OK)
		return false;

	// 2) 응용프로그램이 실행된 경우에 외부 감시를 중단하도록 명령한다.
	FILE *fp;
	fp = fopen("./AlrimiSwitchProtect.cfg", "r"); 
	if(fp != NULL)
	{
		fscanf(fp, "%s", szSwitchProtectFileName);
		fclose(fp);
		//
		fp = fopen(szSwitchProtectFileName, "w"); // "AlrimiSwitchProtect.exe에서 인식할 수 있는 파일이름을 만든다
		fclose(fp);
	}

	// 3) 알리미 스위치를 응용프로그램 내부에서 직접 감시한다.  2초간격(변경 가능함)
	SetTimer(TIMER_PROTECT, 2000, NULL);
	////////////////////////////////////////////////////////////////////////
	
	return TRUE;  // 포커스를 컨트롤에 설정하지 않으면 TRUE를 반환합니다.
}

void CTestAlrimiSwitchDlg::OnTimer(UINT nIDEvent)
{
	if(nIDEvent == TIMER_PROTECT)
	{
		// 알리미 스위치 내부 감시
		AlrimiSwitchProtect();


	}
}

void CTestAlrimiSwitchDlg::OnBnClickedBtnSTART()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	// 방송하기
	if( SwitchControl(1) == false) // 1 : start
	{
		OnBnClickedOk();

		if(InitAudioOut() != AUD_OK)
			return;

		// 2) 응용프로그램이 실행된 경우에 외부 감시를 중단하도록 명령한다.
		FILE *fp;
		fp = fopen("./AlrimiSwitchProtect.cfg", "r"); 
		if(fp != NULL)
		{
			fscanf(fp, "%s", szSwitchProtectFileName);
			fclose(fp);
			//
			fp = fopen(szSwitchProtectFileName, "w"); // "AlrimiSwitchProtect.exe에서 인식할 수 있는 파일이름을 만든다
			fclose(fp);
		}

		// 3) 알리미 스위치를 응용프로그램 내부에서 직접 감시한다.  2초간격(변경 가능함)
		SetTimer(TIMER_PROTECT, 2000, NULL);

		SwitchControl(1);
	}
}

void CTestAlrimiSwitchDlg::OnBnClickedBtnSTOP()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	// 방송 중지
	if( SwitchControl(0) == false)// ; // 0 : stop
	{
		OnBnClickedOk();

		if(InitAudioOut() != AUD_OK)
			return;

		// 2) 응용프로그램이 실행된 경우에 외부 감시를 중단하도록 명령한다.
		FILE *fp;
		fp = fopen("./AlrimiSwitchProtect.cfg", "r"); 
		if(fp != NULL)
		{
			fscanf(fp, "%s", szSwitchProtectFileName);
			fclose(fp);
			//
			fp = fopen(szSwitchProtectFileName, "w"); // "AlrimiSwitchProtect.exe에서 인식할 수 있는 파일이름을 만든다
			fclose(fp);
		}

		// 3) 알리미 스위치를 응용프로그램 내부에서 직접 감시한다.  2초간격(변경 가능함)
		SetTimer(TIMER_PROTECT, 2000, NULL);

		SwitchControl(0);
	}
}

void CTestAlrimiSwitchDlg::OnBnClickedOk()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	// 1) 오디오 해제
	UninitAudioOut();

	// 2) 내부 감시를 중단한다
	KillTimer(TIMER_PROTECT);

	// 3) 외부 감시 시작. 감시 중지 파일을 삭제하여 외부 감시가 시작되도록 한다
	DeleteFileA((LPCSTR)szSwitchProtectFileName);

	OnOK();
}
