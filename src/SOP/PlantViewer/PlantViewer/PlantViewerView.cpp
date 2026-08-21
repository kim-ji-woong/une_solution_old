// 이 MFC 샘플 소스 코드는 MFC Microsoft Office Fluent 사용자 인터페이스("Fluent UI")를 
// 사용하는 방법을 보여 주며, MFC C++ 라이브러리 소프트웨어에 포함된 
// Microsoft Foundation Classes Reference 및 관련 전자 문서에 대해 
// 추가적으로 제공되는 내용입니다.  
// Fluent UI를 복사, 사용 또는 배포하는 데 대한 사용 약관은 별도로 제공됩니다.  
// Fluent UI 라이선싱 프로그램에 대한 자세한 내용은 
// http://msdn.microsoft.com/officeui를 참조하십시오.
//
// Copyright (C) Microsoft Corporation
// 모든 권리 보유.

// PlantViewerView.cpp : CPlantViewerView 클래스의 구현
//

#include "stdafx.h"
// SHARED_HANDLERS는 미리 보기, 축소판 그림 및 검색 필터 처리기를 구현하는 ATL 프로젝트에서 정의할 수 있으며
// 해당 프로젝트와 문서 코드를 공유하도록 해 줍니다.
#ifndef SHARED_HANDLERS
#include "PlantViewer.h"
#endif

#include "PlantViewerDoc.h"
#include "PlantViewerView.h"
#include "MainFrm.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CPlantViewerView

IMPLEMENT_DYNCREATE(CPlantViewerView, CView)

BEGIN_MESSAGE_MAP(CPlantViewerView, CView)
	// 표준 인쇄 명령입니다.
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CPlantViewerView::OnFilePrintPreview)
	ON_WM_CONTEXTMENU()
	ON_WM_RBUTTONUP()
	ON_WM_SETFOCUS()
	ON_WM_KILLFOCUS()
	
END_MESSAGE_MAP()

// CPlantViewerView 생성/소멸

CPlantViewerView::CPlantViewerView()
{
	// TODO: 여기에 생성 코드를 추가합니다.

}

CPlantViewerView::~CPlantViewerView()
{
}

BOOL CPlantViewerView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: CREATESTRUCT cs를 수정하여 여기에서
	//  Window 클래스 또는 스타일을 수정합니다.

	return CView::PreCreateWindow(cs);
}

// CPlantViewerView 그리기

void CPlantViewerView::OnDraw(CDC* /*pDC*/)
{
	CPlantViewerDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: 여기에 원시 데이터에 대한 그리기 코드를 추가합니다.
}


// CPlantViewerView 인쇄


void CPlantViewerView::OnFilePrintPreview()
{
#ifndef SHARED_HANDLERS
	AFXPrintPreview(this);
#endif
}

BOOL CPlantViewerView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// 기본적인 준비
	return DoPreparePrinting(pInfo);
}

void CPlantViewerView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 인쇄하기 전에 추가 초기화 작업을 추가합니다.
}

void CPlantViewerView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 인쇄 후 정리 작업을 추가합니다.
}

void CPlantViewerView::OnRButtonUp(UINT /* nFlags */, CPoint point)
{
	ClientToScreen(&point);
	OnContextMenu(this, point);
}

void CPlantViewerView::OnContextMenu(CWnd* /* pWnd */, CPoint point)
{
#ifndef SHARED_HANDLERS
	theApp.GetContextMenuManager()->ShowPopupMenu(IDR_POPUP_EDIT, point.x, point.y, this, TRUE);
#endif
}


// CPlantViewerView 진단

#ifdef _DEBUG
void CPlantViewerView::AssertValid() const
{
	CView::AssertValid();
}

void CPlantViewerView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CPlantViewerDoc* CPlantViewerView::GetDocument() const // 디버그되지 않은 버전은 인라인으로 지정됩니다.
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CPlantViewerDoc)));
	return (CPlantViewerDoc*)m_pDocument;
}
#endif //_DEBUG


// CPlantViewerView 메시지 처리기




void CPlantViewerView::OnSetFocus(CWnd* pOldWnd)
{
	CView::OnSetFocus(pOldWnd);

	// TODO: 여기에 메시지 처리기 코드를 추가합니다.
	// 마우스가 윈도우 영역으로 돌아오면 마우스 Input을 계속 받도록 한다.
	if (thePlayer)
		thePlayer->PauseInput(false);
}


void CPlantViewerView::OnKillFocus(CWnd* pNewWnd)
{
	CView::OnKillFocus(pNewWnd);

	// TODO: 여기에 메시지 처리기 코드를 추가합니다.
	// 마우스가 윈도우 영역을 벗어나면 더 이상 Virtools 윈도우에 영향을 미치지 않도록 수정
	if (thePlayer)
		thePlayer->PauseInput(true);
}


void CPlantViewerView::OnInitialUpdate()
{
	CView::OnInitialUpdate();

	CMainFrame* pFrame = (CMainFrame*) AfxGetApp()->GetMainWnd();
	pFrame->GetPane().DeleteItem();
}

