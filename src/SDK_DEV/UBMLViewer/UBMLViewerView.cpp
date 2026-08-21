
#include "stdafx.h"
#ifndef SHARED_HANDLERS
#include "UBMLViewer.h"
#endif

#include "UBMLViewerDoc.h"
#include "UBMLViewerView.h"


// Core API 

#include "UMathAPI.h"
#include "UDB.h"
#include "UBaseDriver.h"
#include "UBaseModel.h"
#include "UBaseView.h"
#include "UEntity.h"
#include "UAnimation.h"
#include "afxdlgs.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif

using namespace UnE::Core;

// CUBMLViewerView

IMPLEMENT_DYNCREATE(CUBMLViewerView, CView)

BEGIN_MESSAGE_MAP(CUBMLViewerView, CView)
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CUBMLViewerView::OnFilePrintPreview)
	ON_WM_ERASEBKGND()
	ON_WM_TIMER()
	ON_WM_SIZE()
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_RBUTTONDOWN()
	ON_WM_RBUTTONUP()
	ON_WM_MBUTTONDOWN()
	ON_WM_MOUSEMOVE()
	ON_WM_MOUSEWHEEL()
	ON_COMMAND(ID_ADD_CUBE, &CUBMLViewerView::OnAddCube)
	ON_COMMAND(ID_REMOVE_CUBE, &CUBMLViewerView::OnRemoveCube)
	ON_COMMAND(ID_SAVE_CUBE, &CUBMLViewerView::OnSaveCube)
	ON_COMMAND(ID_LOAD_CUBE, &CUBMLViewerView::OnLoadCube)
	ON_COMMAND(ID_SELECT_ENTITY, &CUBMLViewerView::OnSelectEntity)
	ON_COMMAND(ID_CLEAR_SELECT, &CUBMLViewerView::OnClearSelect)
	ON_COMMAND(ID_VIEW_WIREFRAME, &CUBMLViewerView::OnViewWireframe)
	ON_UPDATE_COMMAND_UI(ID_VIEW_WIREFRAME, &CUBMLViewerView::OnUpdateViewWireframe)
	ON_COMMAND(ID_TEXTURE_SET, &CUBMLViewerView::OnTextureSet)
	ON_COMMAND(ID_VIEW_HIDDENLINE, &CUBMLViewerView::OnViewHiddenline)
	ON_UPDATE_COMMAND_UI(ID_VIEW_HIDDENLINE, &CUBMLViewerView::OnUpdateViewHiddenline)
	ON_UPDATE_COMMAND_UI(ID_VIEW_SHADED, &CUBMLViewerView::OnUpdateViewPolygon)
	ON_COMMAND(ID_VIEW_SHADED, &CUBMLViewerView::OnViewPolygon)
	ON_COMMAND(ID_VIEW_OCTREE, &CUBMLViewerView::OnViewOctree)
	ON_UPDATE_COMMAND_UI(ID_VIEW_OCTREE, &CUBMLViewerView::OnUpdateViewOctree)
	ON_COMMAND(ID_VIEW_TEXTURED, &CUBMLViewerView::OnViewTextured)
	ON_UPDATE_COMMAND_UI(ID_VIEW_TEXTURED, &CUBMLViewerView::OnUpdateViewTextured)
	ON_UPDATE_COMMAND_UI(ID_TEXTURE_SET, &CUBMLViewerView::OnUpdateTextureSet)
	ON_COMMAND(ID_TEXTURE_REMOVE, &CUBMLViewerView::OnTextureRemove)
	ON_UPDATE_COMMAND_UI(ID_TEXTURE_REMOVE, &CUBMLViewerView::OnUpdateTextureRemove)
	ON_COMMAND(ID_IMPORT_DAE, &CUBMLViewerView::OnImportDae)
	ON_COMMAND(ID_DELETE_ENTITY, &CUBMLViewerView::OnDeleteEntity)
	ON_COMMAND(ID_REMOVE_POI, &CUBMLViewerView::OnRemovePoi)
	ON_COMMAND(ID_ADD_POI_TYPE1, &CUBMLViewerView::OnAddPoiType1)
	ON_COMMAND(ID_ADD_POI_TYPE2, &CUBMLViewerView::On32815)
	//ON_COMMAND(ID_32815, &CUBMLViewerView::On32815)
END_MESSAGE_MAP()


CUBMLViewerView::CUBMLViewerView()
{
	bInit = false;
	m_bViewWireFrame = FALSE;
	m_bViewHiddenLine = FALSE;
	m_bViewPolygon = TRUE;
	m_bShowOctree = FALSE;

	m_nViewMode = 4;
	m_p3DView = NULL;
}

CUBMLViewerView::~CUBMLViewerView()
{
}

BOOL CUBMLViewerView::PreCreateWindow(CREATESTRUCT& cs)
{
	return CView::PreCreateWindow(cs);
}

void CUBMLViewerView::OnDraw(CDC* /*pDC*/)
{
	CUBMLViewerDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;
	if( m_p3DView != NULL)
		m_p3DView->RenderOneFrame();
}

void CUBMLViewerView::OnFilePrintPreview()
{
#ifndef SHARED_HANDLERS
	AFXPrintPreview(this);
#endif
}

BOOL CUBMLViewerView::OnPreparePrinting(CPrintInfo* pInfo)
{
	return DoPreparePrinting(pInfo);
}

void CUBMLViewerView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
}

void CUBMLViewerView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
}

void CUBMLViewerView::OnContextMenu(CWnd* /* pWnd */, CPoint point)
{
#ifndef SHARED_HANDLERS
	theApp.GetContextMenuManager()->ShowPopupMenu(IDR_POPUP_EDIT, point.x, point.y, this, TRUE);
#endif
}


#ifdef _DEBUG
void CUBMLViewerView::AssertValid() const
{
	CView::AssertValid();
}

void CUBMLViewerView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CUBMLViewerDoc* CUBMLViewerView::GetDocument() const 
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CUBMLViewerDoc)));
	return (CUBMLViewerDoc*)m_pDocument;
}
#endif //_DEBUG


void CUBMLViewerView::OnInitialUpdate()
{
	CView::OnInitialUpdate();

	m_p3DView = new UnE::Core::UBaseView(m_hWnd);

	CRect rect;
	GetWindowRect(&rect);

	m_p3DView->AddOperator(&mouseOperator);	
	UBaseDriver::Instance().SetDisplayMode(rect.Width()+1, rect.Height()+1, 32);
	if( m_p3DView->CreateRenderWindow( rect.Width()+1, rect.Height()+1, "main", "maincam") == true)
	{		
		mouseOperator.SetHWnd(m_hWnd);

		UDB::AddBaseView(m_p3DView);
		UDB::AddOperator(&mouseOperator);

		m_p3DView->LoadDefultResource();

		bInit = true;
	}	
}



LRESULT CUBMLViewerView::WindowProc( UINT message, WPARAM wParam, LPARAM lParam )
{
	return CView::WindowProc(message, wParam, lParam);
}


BOOL CUBMLViewerView::OnEraseBkgnd(CDC* pDC)
{
	return TRUE;
}

BOOL CUBMLViewerView::DestroyWindow()
{
	KillTimer(2);

	BOOL bResult =  CView::DestroyWindow();

	return bResult;
}


void CUBMLViewerView::OnTimer(UINT_PTR nIDEvent)
{
	if( nIDEvent == 2)
	{
		//UBaseView::Instance().RenderOneFrame(m_hWnd);
	}	
	CView::OnTimer(nIDEvent);
}


void CUBMLViewerView::OnSize(UINT nType, int cx, int cy)
{
	CView::OnSize(nType, cx, cy);

	if( cx != 0 && cy != 0)
	{
		if( m_p3DView != NULL)
		{
			m_p3DView->ChangeDisplaySize(cx,  cy);
			m_p3DView->RenderOneFrame();
		}
		
	}
}


void CUBMLViewerView::OnLButtonDown(UINT nFlags, CPoint point)
{
	mouseOperator.OnLButtonDown(nFlags, point);

	UnE::Core::UObject * pObj = mouseOperator.SelectedObject();

	if( pObj != NULL)
	{
		std::string szName = pObj->GetName();

	}

	CView::OnLButtonDown(nFlags, point);
}


void CUBMLViewerView::OnLButtonUp(UINT nFlags, CPoint point)
{
	mouseOperator.OnLButtonUp(nFlags, point);
	CView::OnLButtonUp(nFlags, point);
}

void CUBMLViewerView::OnRButtonDown(UINT nFlags, CPoint point)
{
	mouseOperator.OnRButtonDown(nFlags, point);
	CView::OnRButtonDown(nFlags, point);
}

void CUBMLViewerView::OnRButtonUp(UINT nFlags, CPoint point)
{
	mouseOperator.SavePoint(nFlags, point);
	//ouseOperator.OnRButtonUp(nFlags, point);
	ClientToScreen(&point);
	OnContextMenu(this, point);
}


void CUBMLViewerView::OnMouseMove(UINT nFlags, CPoint point)
{
	DWORD nCountPre = GetTickCount();

	mouseOperator.OnMouseMove(nFlags, point);

	DWORD nCountPost = GetTickCount();
	DWORD nCurCnt = nCountPost - nCountPre;
	TRACE1("MOVE : %d\n", nCurCnt);
	CView::OnMouseMove(nFlags, point);	
}


void CUBMLViewerView::OnMButtonDown(UINT nFlags, CPoint point)
{
	mouseOperator.OnMButtonDown(nFlags, point);
	CView::OnMButtonDown(nFlags, point);
}

void CUBMLViewerView::OnAddCube()
{
	//m_p3DView->CreateBox("MyCube");
}

void CUBMLViewerView::OnRemoveCube()
{
	//m_p3DView->RemoveCube("MyCube");
	//UnE::Core::UBaseView::Instance().RenderOneFrame(m_hWnd);
}

void CUBMLViewerView::OnSaveCube()
{
	//m_p3DView->SaveCube("MyCube");
	//m_p3DView->CreateCircle();
}

void CUBMLViewerView::OnLoadCube()
{
}

BOOL CUBMLViewerView::OnMouseWheel( UINT nFlags, short zDelta, CPoint pt )
{
	mouseOperator.OnMouseWheel(nFlags, zDelta, pt);
	return CView::OnMouseWheel(nFlags, zDelta, pt);	
}

void CUBMLViewerView::OnSelectEntity()
{
	CPoint pt = mouseOperator.GetSavedPoint();
	UINT   nFlag = mouseOperator.GetSavedFlags();

	mouseOperator.OnSelect(nFlag, pt);

	UEntity * pObj = mouseOperator.SelectedObject();
	if( pObj != NULL)
	{
		std::string name = "Walk";
		UAnimationState * pState = pObj->GetAnimationState(name);
		if( pState != NULL)
		{
			pState->SetLoop(true);
			pState->SetEnabled(true);
		}		
		//pState->AddTime(100.0f);
		TRACE1("Selected Object : % d\n", pObj->GetID());
		UDB::GetUDB()->GetAnimationManager()->SetEnabled(true);
		UDB::GetUDB()->GetAnimationManager()->AddAnimationState(pState);
	}	
}

void CUBMLViewerView::OnClearSelect()
{
	mouseOperator.ClearSelect();
}

void CUBMLViewerView::OnViewOctree()
{
	m_bShowOctree = ! m_bShowOctree;
	m_p3DView->ShowOctree(m_bShowOctree);
}

void CUBMLViewerView::OnUpdateViewOctree(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_bShowOctree);
}

void CUBMLViewerView::OnViewWireframe()
{
	m_nViewMode = 1;
	m_p3DView->ChangeViewMode(UnE::Core::UPolygonMode::ePM_WIREFRAME);	
}

void CUBMLViewerView::OnUpdateViewWireframe(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_nViewMode == 1);
}

void CUBMLViewerView::OnViewHiddenline()
{
	m_nViewMode = 2;
	m_p3DView->ChangeViewMode(UnE::Core::UPolygonMode::ePM_HIDDENLINE);
}

void CUBMLViewerView::OnUpdateViewHiddenline(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_nViewMode == 2);
}

void CUBMLViewerView::OnViewPolygon()
{
	m_nViewMode = 3;
	m_p3DView->ChangeViewMode(UnE::Core::UPolygonMode::ePM_SHADED);
}

void CUBMLViewerView::OnUpdateViewPolygon(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_nViewMode == 3);
}

void CUBMLViewerView::OnViewTextured()
{
	m_nViewMode = 4;
	m_p3DView->ChangeViewMode(UnE::Core::UPolygonMode::ePM_TEXTURED);
}

void CUBMLViewerView::OnUpdateViewTextured(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_nViewMode == 4);
}

void CUBMLViewerView::OnTextureSet()
{
	if( m_nViewMode != 4)
		return;

	CFileDialog * pDialog = new CFileDialog(TRUE);	
	if( pDialog->DoModal() == IDOK)
	{
		std::string szTextPath = pDialog->GetPathName().GetBuffer();
		mouseOperator.SetTexture(szTextPath);
	}
	delete pDialog;
}

void CUBMLViewerView::OnUpdateTextureSet(CCmdUI *pCmdUI)
{
	if(m_nViewMode == 4)
	{
		pCmdUI->Enable(TRUE);
	}
	else
	{
		pCmdUI->Enable(FALSE);
	}
}

void CUBMLViewerView::OnTextureRemove()
{
	mouseOperator.RemoveTexture();
}

void CUBMLViewerView::OnUpdateTextureRemove(CCmdUI *pCmdUI)
{
	if(m_nViewMode == 4)
	{
		pCmdUI->Enable(TRUE);
	}
	else
	{
		pCmdUI->Enable(FALSE);
	}
}

void CUBMLViewerView::OnImportDae()
{
	CFileDialog * pDialog = new CFileDialog(TRUE);	

	if( pDialog->DoModal() == IDOK)
	{
		std::string szTextPath = pDialog->GetPathName().GetBuffer();
		
		UBaseModel * pModel = UDB::GetBaseModel((int)m_hWnd);
		if( pModel != NULL)
		{
			pModel->Read(szTextPath);	
			Invalidate();
		}
	}
	delete pDialog;
}

void CUBMLViewerView::OnDeleteEntity()
{
	CPoint pt = mouseOperator.GetSavedPoint();
	UINT   nFlag = mouseOperator.GetSavedFlags();

	mouseOperator.OnDelete(nFlag, pt);
}

void CUBMLViewerView::OnRemovePoi()
{
	m_p3DView->RemovePOI();
	Invalidate();
}

void CUBMLViewerView::OnAddPoiType1()
{
	CFileDialog * pDialog = new CFileDialog(TRUE);
	if( pDialog->DoModal() == IDOK)
	{
		std::string szTextPath = pDialog->GetPathName().GetBuffer();
		int nID = m_p3DView->AddIconPOI(szTextPath);
		Invalidate();
	}
	delete pDialog;
}

void CUBMLViewerView::On32815()
{
	USES_CONVERSION;
	m_p3DView->AddTextPOI(W2A(L"AAA - 한글테스트 - BBB"));
	Invalidate();
}
