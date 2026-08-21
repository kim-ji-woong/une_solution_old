#include "stdafx.h"

#ifndef SHARED_HANDLERS
#include "ColladaApp.h"
#endif

#include "ColladaDoc.h"
#include "ColladaView.h"

#include "AssimpView.h"
#include "Display.h"


using namespace AssimpView;



#ifdef _DEBUG
#define new DEBUG_NEW
#endif

IMPLEMENT_DYNCREATE(ColladaView, CView)

BEGIN_MESSAGE_MAP(ColladaView, CView)
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &ColladaView::OnFilePrintPreview)
	ON_WM_CONTEXTMENU()
	ON_WM_RBUTTONUP()
	ON_WM_ERASEBKGND()
	ON_WM_TIMER()
	ON_COMMAND(ID_BACKGROUND_SETCOLOR, &ColladaView::OnBackgroundSetColor)
	ON_WM_SIZE()
END_MESSAGE_MAP()

ColladaView::ColladaView()
{
	m_bInit= false;
}

ColladaView::~ColladaView()
{
}

BOOL ColladaView::PreCreateWindow(CREATESTRUCT& cs)
{
	if (!CWnd::PreCreateWindow(cs))
		return FALSE;
	
	cs.dwExStyle |= WS_EX_CLIENTEDGE;
	cs.style &= ~WS_BORDER;
	cs.lpszClass = AfxRegisterWndClass(CS_HREDRAW|CS_VREDRAW|CS_DBLCLKS,
	::LoadCursor(NULL, IDC_ARROW), NULL, NULL);
	return TRUE;
}

int iCurrent = 0;
double g_dCurTime = 0;
double g_dLastTime = 0;

void ColladaView::OnDraw(CDC* /*pDC*/)
{
	ColladaDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	if( m_bInit == true)
	{
		CDisplay::Instance().OnRender();

		// measure FPS, average it out
		g_dCurTime     = timeGetTime();
		g_fElpasedTime = (float)((g_dCurTime - g_dLastTime) * 0.001);
		g_dLastTime    = g_dCurTime;
		double dFPS = 1.0f / g_fElpasedTime;

		if (30 == iCurrent++)
		{
			iCurrent = 0;
			if (dFPS != g_fFPS)
			{
				g_fFPS = dFPS;
				char szOut[256];	
				sprintf(szOut,"%i",(int)floorf((float)dFPS+0.5f));
				::SetDlgItemText(g_hDlg, IDC_EFPS, szOut);
				UpdateEdit(g_hDlg);
			    //TRACE(szOut);
			}
		}
	}

}

void ColladaView::OnFilePrintPreview()
{
#ifndef SHARED_HANDLERS
	AFXPrintPreview(this);
#endif
}

BOOL ColladaView::OnPreparePrinting(CPrintInfo* pInfo)
{
	return DoPreparePrinting(pInfo);
}

void ColladaView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	
}

void ColladaView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	
}

void ColladaView::OnRButtonUp(UINT /* nFlags */, CPoint point)
{
	ClientToScreen(&point);
	OnContextMenu(this, point);
}

void ColladaView::OnContextMenu(CWnd* /* pWnd */, CPoint point)
{
#ifndef SHARED_HANDLERS
	theApp.GetContextMenuManager()->ShowPopupMenu(IDR_POPUP_EDIT, point.x, point.y, this, TRUE);
#endif
}


// ColladaView 진단

#ifdef _DEBUG
void ColladaView::AssertValid() const
{
	CView::AssertValid();
}

void ColladaView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

ColladaDoc* ColladaView::GetDocument() const // 디버그되지 않은 버전은 인라인으로 지정됩니다.
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(ColladaDoc)));
	return (ColladaDoc*)m_pDocument;
}
#endif //_DEBUG

//-------------------------------------------------------------------------------
// Load the light colors from the registry
//-------------------------------------------------------------------------------
void LoadLightColors()
{
	DWORD dwTemp = 4;
	RegQueryValueEx(GetRootReg(),"LightColor0",NULL,NULL, (BYTE*)&g_avLightColors[0],&dwTemp);
	RegQueryValueEx(GetRootReg(),"LightColor1",NULL,NULL, (BYTE*)&g_avLightColors[1],&dwTemp);
	RegQueryValueEx(GetRootReg(),"LightColor2",NULL,NULL, (BYTE*)&g_avLightColors[2],&dwTemp);
	return;
}


void ColladaView::OnInitialUpdate()
{
	CView::OnInitialUpdate();

	// create the D3D device object
	if (0 == CreateDevice(m_hWnd))
	{
		MessageBox("Failed to initialize Direct3D 9 (2)", "Collada Viewer",MB_OK);
		return;
	}


	CLogDisplay::Instance().SetView(GetSafeHwnd());
	CLogDisplay::Instance().AddEntry("[OK] Here we go!");
	
	char szFileName[MAX_PATH];
	D3DCOLOR clrColor;	
	DWORD dwTemp = MAX_PATH;
	RegCreateKeyEx(HKEY_CURRENT_USER, "Software\\UNE\\ColladaViewer",NULL,NULL,0,KEY_ALL_ACCESS, NULL, &GetRootReg(),NULL);
	if(ERROR_SUCCESS == RegQueryValueEx(GetRootReg(),"LastSkyBoxSrc",NULL,NULL, (BYTE*)szFileName,&dwTemp) && '\0' != szFileName[0])
	{
		CBackgroundPainter::Instance().SetCubeMapBG(szFileName);
	}
	else if(ERROR_SUCCESS == RegQueryValueEx(GetRootReg(),"LastTextureSrc",NULL,NULL, (BYTE*)szFileName,&dwTemp) && '\0' != szFileName[0])
	{
		CBackgroundPainter::Instance().SetTextureBG(szFileName);
	}
	else if(ERROR_SUCCESS == RegQueryValueEx(GetRootReg(),"Color",NULL,NULL,	(BYTE*)&clrColor,&dwTemp))
	{
		CBackgroundPainter::Instance().SetColor(clrColor);
	}
		
	LoadLightColors();
	
	CDisplay::Instance().SetViewMode(CDisplay::VIEWMODE_FULL);

	SetTimer(100, 10, NULL);

	m_bInit = true;
}


BOOL ColladaView::OnEraseBkgnd(CDC* pDC)
{	
	return FALSE;
}


BOOL ColladaView::Create(LPCTSTR lpszClassName, LPCTSTR lpszWindowName, DWORD dwStyle, const RECT& rect, CWnd* pParentWnd, UINT nID, CCreateContext* pContext)
{
	
	return CView::Create(lpszClassName, lpszWindowName, dwStyle, rect, pParentWnd, nID, pContext);
}


BOOL ColladaView::DestroyWindow()
{
	KillTimer(100);

	DeleteAsset();
	Assimp::DefaultLogger::kill();
	ShutdownDevice();

	return CView::DestroyWindow();
}




void ColladaView::OnTimer(UINT_PTR nIDEvent)
{
	if( nIDEvent == 100)
	{

		Invalidate(FALSE);
	}

	CView::OnTimer(nIDEvent);
}


LRESULT ColladaView::WindowProc(UINT message, WPARAM wParam, LPARAM lParam)
{
	if( TRUE == MessageProc(m_hWnd, message, wParam, lParam))
		return TRUE;

	return CView::WindowProc(message, wParam, lParam);
}

void ColladaView::OnBackgroundSetColor()
{
	
	GetDocument()->ChooseBGColor();
	//LoadBGTexture();
	//LoadSkybox(); 
}


void ColladaView::OnSize(UINT nType, int cx, int cy)
{
	CView::OnSize(nType, cx, cy);

	if( cx != 0 && cy != 0)
	{
		if( m_bInit == true)
		{
			KillTimer(100);

			m_bInit = false;
			Sleep(10);

			//Reset(cx, cy);
			
			m_bInit = true;
			Sleep(10);
			SetTimer(100, 10, NULL);
		}
		
	}
}
