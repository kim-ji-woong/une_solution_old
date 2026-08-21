
#include "stdafx.h"
#include "UCore3DDoc.h"
#include "UCore3DView.h"



IMPLEMENT_DYNCREATE(UCore3DView, CView)

BEGIN_MESSAGE_MAP(UCore3DView, CView)
	ON_COMMAND(WM_OGRE3D_VIEW_AABB, &UCore3DView::OnViewWorldAabb)
	ON_UPDATE_COMMAND_UI(WM_OGRE3D_VIEW_AABB, &UCore3DView::OnUpdateViewWorldAabb)
	ON_COMMAND(WM_OGRE3D_USE_TRACKBALL, &UCore3DView::OnUseTrackball)
	ON_UPDATE_COMMAND_UI(WM_OGRE3D_USE_TRACKBALL, &UCore3DView::OnUpdateUseTrackball)
END_MESSAGE_MAP()


UCore3DView::UCore3DView()
{
	m_bMouseOrbitMode = FALSE;
	m_bMousePanMode = FALSE;
	m_bMouseZoomMode = FALSE;

	m_bViewWorldAABB = FALSE;
	m_bUseTrackBall = FALSE;
}

UCore3DView::~UCore3DView()
{
}

void UCore3DView::OnInitialUpdate()
{	
	CView::OnInitialUpdate();
	// Get Client rect
	CRect   rect;
	GetClientRect(&rect);
	

}


void UCore3DView::OnDraw(CDC* pDC)
{
	static BOOL bFirst = TRUE;
	
	UCore3DDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;


}


void UCore3DView::OnRButtonUp(UINT nFlags, CPoint point)

{
	TRACE0("R-UP ");
	TRACE1("X: %d", point.x);
	TRACE1("Y: %d\n", point.y);


	ClientToScreen(&point);
	OnContextMenu(this, point);

	//CView::OnRButtonDown(nFlags, point);
}


// COgre3D_SDIView 진단

#ifdef _DEBUG
void UCore3DView::AssertValid() const
{
	CView::AssertValid();
}

void UCore3DView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

UCore3DDoc* UCore3DView::GetDocument() const 
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(UCore3DDoc)));
	return (UCore3DDoc*)m_pDocument;
}
#endif //_DEBUG


// COgre3D_SDIView 메시지 처리기



BOOL UCore3DView::OnEraseBkgnd(CDC* pDC)
{
	return FALSE;
}

void UCore3DView::OnSize(UINT nType, int cx, int cy)
{
	CView::OnSize(nType, cx, cy);
}

void UCore3DView::OnRButtonDown(UINT nFlags, CPoint point)
{
	TRACE0("R-DOWN ");
	TRACE1("X: %d", point.x);
	TRACE1("Y: %d\n", point.y);
	CView::OnRButtonDown(nFlags, point);
}

void UCore3DView::OnLButtonDown(UINT nFlags, CPoint point)
{

	SetCapture();

	m_bMouseOrbitMode = TRUE;
	CView::OnLButtonDown(nFlags, point);

}

void UCore3DView::OnLButtonUp(UINT nFlags, CPoint point)
{
	m_bMouseOrbitMode = FALSE;
	ReleaseCapture();	
	CView::OnLButtonUp(nFlags, point);
}


void UCore3DView::OnMButtonDown(UINT nFlags, CPoint point)
{
	m_bMousePanMode = TRUE;
	SetCapture();

	CView::OnMButtonDown(nFlags, point);
}

void UCore3DView::OnMButtonUp(UINT nFlags, CPoint point)
{
	m_bMousePanMode = FALSE;
	ReleaseCapture();
	CView::OnMButtonUp(nFlags, point);
}


void UCore3DView::OnMouseMove(UINT nFlags, CPoint point)
{


	CView::OnMouseMove(nFlags, point);
}


BOOL UCore3DView::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	TRACE1("DELTA : %d \n" , zDelta);


	return CView::OnMouseWheel(nFlags, zDelta, pt);
}




void UCore3DView::OnViewWorldAabb()
{
		
}

void UCore3DView::OnUpdateViewWorldAabb(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_bViewWorldAABB);

}

void UCore3DView::OnUseTrackball()
{
	m_bUseTrackBall = !m_bUseTrackBall;
}

void UCore3DView::OnUpdateUseTrackball(CCmdUI *pCmdUI)
{
	pCmdUI->SetCheck(m_bUseTrackBall);
}
