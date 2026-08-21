
// GeometrySampleView.cpp : CGeometrySampleView 클래스의 구현
//

#include "stdafx.h"
// SHARED_HANDLERS는 미리 보기, 축소판 그림 및 검색 필터 처리기를 구현하는 ATL 프로젝트에서 정의할 수 있으며
// 해당 프로젝트와 문서 코드를 공유하도록 해 줍니다.
#ifndef SHARED_HANDLERS
#include "GeometrySample.h"
#endif

#include "GeometrySampleDoc.h"
#include "GeometrySampleView.h"

#include "Geometry/Vertex.h"
#include "Geometry/Math.h"
#include "Geometry/Line.h"

//#include <GdiPlusPen.h>



//using namespace Gdiplus;
using namespace UnE::Geometry;

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CGeometrySampleView

IMPLEMENT_DYNCREATE(CGeometrySampleView, CView)

BEGIN_MESSAGE_MAP(CGeometrySampleView, CView)
	// 표준 인쇄 명령입니다.
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CGeometrySampleView::OnFilePrintPreview)
	ON_WM_CONTEXTMENU()
	ON_WM_RBUTTONUP()
END_MESSAGE_MAP()

// CGeometrySampleView 생성/소멸

CGeometrySampleView::CGeometrySampleView()
{
	// TODO: 여기에 생성 코드를 추가합니다.

}

CGeometrySampleView::~CGeometrySampleView()
{
}

BOOL CGeometrySampleView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: CREATESTRUCT cs를 수정하여 여기에서
	//  Window 클래스 또는 스타일을 수정합니다.

	return CView::PreCreateWindow(cs);
}

// CGeometrySampleView 그리기
static void DrawLine(CDC* pDC, Line2D& rLine, CWnd* pWnd)
{
	Line2D::LineType type = rLine.GetLineType();
	Vertex2D vBegin = rLine.GetVertex(true);
	Vertex2D vEnd = rLine.GetVertex(false);



	RECT rect;
	pWnd->GetClientRect(&rect);

	int nHeight = rect.bottom - rect.top;
	int nWidth  = rect.right - rect.left;

	Vertex2D vTL(0.0, nHeight), vBL(0.0, 0.0), vBR(nWidth, 0.0), vTR(nWidth, nHeight);

	Line2D lineLeft(vTL, vBL, Line2D::SEGMENT), lineTop(vTL, vTR, Line2D::SEGMENT);
	Line2D lineRight(vTR, vBR, Line2D::SEGMENT), lineBottom(vBL, vBR, Line2D::SEGMENT);

	Vertex2D v1, v2;
	Line2D::LineType typeResult;



	if (type == Line2D::LINE)
	{
		//vBegin = Math::GetLinearVertex(vBegin, vEnd, -10000000.0);
		//vEnd = Math::GetLinearVertex(vEnd, vBegin, -100000000.0);

		Vertex2D arr[4];
		int nIndex = 0;

		if (rLine.IntersectLine(lineLeft, v1, v2, typeResult) > 0)
		{
			arr[nIndex++] = v1;
		}
		
		if (rLine.IntersectLine(lineRight, v1, v2, typeResult) > 0)
		{
			arr[nIndex++] = v1;
		}

		if (rLine.IntersectLine(lineTop, v1, v2, typeResult) > 0)
		{
			arr[nIndex++] = v1;
		}

		if (rLine.IntersectLine(lineBottom, v1, v2, typeResult) > 0)
		{
			arr[nIndex++] = v1;
		}

		if (nIndex < 2) return;

		vBegin = arr[0];
		vEnd = arr[1];
	}
	else if (type == Line2D::HALF_LINE_BEGIN_2_END)
	{
		//vEnd = Math::GetLinearVertex(vEnd, vBegin, -100000000.0);

		if (rLine.IntersectLine(lineLeft, v1, v2, typeResult) > 0)
		{
			vEnd = v1;
		}
		else if (rLine.IntersectLine(lineRight, v1, v2, typeResult) > 0)
		{
			vEnd = v1;
		}
		else if (rLine.IntersectLine(lineTop, v1, v2, typeResult) > 0)
		{
			vEnd = v1;
		}
		else if (rLine.IntersectLine(lineBottom, v1, v2, typeResult) > 0)
		{
			vEnd = v1;
		}
	}
	else if (type == Line2D::HALF_LINE_END_2_BEGIN)
	{
		//vBegin = Math::GetLinearVertex(vBegin, vEnd, -10000000.0);

		if (rLine.IntersectLine(lineLeft, v1, v2, typeResult) > 0)
		{
			vBegin = v1;
		}
		else if (rLine.IntersectLine(lineRight, v1, v2, typeResult) > 0)
		{
			vBegin = v1;
		}
		else if (rLine.IntersectLine(lineTop, v1, v2, typeResult) > 0)
		{
			vBegin = v1;
		}
		else if (rLine.IntersectLine(lineBottom, v1, v2, typeResult) > 0)
		{
			vBegin = v1;
		}
	}

	pDC->MoveTo((int)vBegin.x, (int)vBegin.y);
	pDC->LineTo((int)vEnd.x, (int)vEnd.y);
}

void CGeometrySampleView::OnDraw(CDC* /*pDC*/)
{
	CGeometrySampleDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	CDC* pDC = GetDC();
	CPen* pNewPen = new CPen(PS_SOLID, 3, RGB(255, 0, 0));
	CPen* pOldPen = NULL;

	pOldPen = pDC->SelectObject(pNewPen);

	pDC->SetBkColor(TRANSPARENT);
	//pDC->Rectangle(CRect(100, 200, 200, 300));


	// TODO: 여기에 원시 데이터에 대한 그리기 코드를 추가합니다.

	Vertex2D v1(100, 200);
	Vertex2D v2(200, 300);

	Vertex2D v3(40, 180);
	Vertex2D v4(180, 40);
	
	Line2D line1(v3, v4, Line2D::LINE);
	Line2D halfLine1(v3, v4, Line2D::HALF_LINE_BEGIN_2_END);
	Line2D seg1(v3, v4, Line2D::SEGMENT);

	Vertex2D ver3(240, 280);
	Vertex2D ver4(280, 240);
	Line2D line2(ver3, ver4, Line2D::LINE);

	Line2D::LineType lineType;
	int nLine = line1.IntersectLine(line2, v3, v4, lineType); //line1과 line2의 교차점 개수 구하기

	bool isLine = line1.IsInclude(v3);

	Vertex2D ver1= Math::GetLinearVertex(v3, v4, 50);
	Vertex2D ver2 = Math::GetRightVertex(ver1, v3, 100);



	LOGBRUSH lb;
	lb.lbStyle = BS_SOLID;
	lb.lbColor = RGB( 192, 192, 192 );

	CPen arNewPen;

	// 각각의 팬의 속성을 지정해 준다.
	arNewPen.CreatePen( PS_SOLID , 20, &lb );

	// 화면에 출력
	// 팬을 지정해 주면서 이전 값을 받아서
	pOldPen = pDC->SelectObject( &arNewPen );

	// 화면에 선을 그리고
	//pDC->MoveTo( 70, 70);
	//pDC->LineTo( v3.x, v3.y);
	DrawLine(pDC, line1, this);

 	// 다시 이전 값으로 dc를 복원
 	pDC->SelectObject( pOldPen );

	// 그 위에 기본 속성의 선을 하나 더 그린다.
	//pDC->MoveTo( 80, 40 );
	//pDC->LineTo( v4.x, v4.y );
	DrawLine(pDC, line2, this);

	// 선을 그린 후 그 선의 속성에 대해서 DeleteObject.
	arNewPen.DeleteObject();

	pDC->SelectObject(pOldPen);
	delete pNewPen;
	ReleaseDC(pDC);

}


// CGeometrySampleiew 인쇄


void CGeometrySampleView::OnFilePrintPreview()
{
#ifndef SHARED_HANDLERS
	AFXPrintPreview(this);
#endif
}

BOOL CGeometrySampleView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// 기본적인 준비
	return DoPreparePrinting(pInfo);
}

void CGeometrySampleView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 인쇄하기 전에 추가 초기화 작업을 추가합니다.
}

void CGeometrySampleView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 인쇄 후 정리 작업을 추가합니다.
}

void CGeometrySampleView::OnRButtonUp(UINT /* nFlags */, CPoint point)
{
	ClientToScreen(&point);
	OnContextMenu(this, point);
}

void CGeometrySampleView::OnContextMenu(CWnd* /* pWnd */, CPoint point)
{
#ifndef SHARED_HANDLERS
	theApp.GetContextMenuManager()->ShowPopupMenu(IDR_POPUP_EDIT, point.x, point.y, this, TRUE);
#endif
}


// CGeometrySampleView 진단

#ifdef _DEBUG
void CGeometrySampleView::AssertValid() const
{
	CView::AssertValid();
}

void CGeometrySampleView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CGeometrySampleDoc* CGeometrySampleView::GetDocument() const // 디버그되지 않은 버전은 인라인으로 지정됩니다.
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CGeometrySampleDoc)));
	return (CGeometrySampleDoc*)m_pDocument;
}


#endif //_DEBUG

// CGeometrySampleView 메시지 처리기
