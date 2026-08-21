// SampleView.cpp : CSampleView 클래스의 구현
//

#include "stdafx.h"
#include "Sample.h"

#include "SampleDoc.h"
#include "SampleView.h"

#include <Handle_WNT_Window.hxx>
#include <WNT_Window.hxx>

//#include <QApplication>
#include <AIS_Shape.hxx>
#include <BRepBuilderAPI_Copy.hxx>

//#include <Geom2d_TrimmedCurve.hxx>
//#include <GCE2d_MakeArcOfCircle.hxx>
#include <Geom2d_Line.hxx>
#include <GCE2d_MakeLine.hxx>
#include <Geom2dAPI_InterCurveCurve.hxx>
#include <IntRes2d_IntersectionPoint.hxx>
#include "OCCGeometry/Arc.h"
#include "OCCGeometry/Circle.h"
#include "OCCGeometry/EArc.h"
#include "Geometry/Math.h"

//#ifdef _DEBUG
//#define new DEBUG_NEW
//#endif



// CSampleView

IMPLEMENT_DYNCREATE(CSampleView, CView)

BEGIN_MESSAGE_MAP(CSampleView, CView)
	// 표준 인쇄 명령입니다.
	ON_COMMAND(ID_FILE_PRINT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, &CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, &CView::OnFilePrintPreview)
	ON_WM_SIZE()
	ON_WM_ERASEBKGND()
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONDOWN()
	ON_WM_MOUSEWHEEL()
	ON_WM_RBUTTONDOWN()
	ON_COMMAND(IDM_WIRE_MODE, &CSampleView::OnWireMode)
	ON_UPDATE_COMMAND_UI(IDM_WIRE_MODE, &CSampleView::OnUpdateWireMode)
	ON_COMMAND(IDM_SHADE_MODE, &CSampleView::OnShadeMode)
	ON_UPDATE_COMMAND_UI(IDM_SHADE_MODE, &CSampleView::OnUpdateShadeMode)
	ON_COMMAND(IDM_HLR_MODE, &CSampleView::OnHlrMode)
	ON_UPDATE_COMMAND_UI(IDM_HLR_MODE, &CSampleView::OnUpdateHlrMode)
END_MESSAGE_MAP()

// CSampleView 생성/소멸

CSampleView::CSampleView()
{
	// TODO: 여기에 생성 코드를 추가합니다.
	myDegenerateModeIsOn=Standard_True;

	myVisMode = VIS_SHADE;

    // will be set in OnInitial update, but, for more security :
    myCurrentMode = CurAction3d_Nothing;

	myXmin=0;
    myYmin=0;  
    myXmax=0;
    myYmax=0;
	myWidth=0;
	myHeight=0;

	viewScale = 1.0;
}

CSampleView::~CSampleView()
{
}

BOOL CSampleView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: CREATESTRUCT cs를 수정하여 여기에서
	//  Window 클래스 또는 스타일을 수정합니다.

	return CView::PreCreateWindow(cs);
}

// CSampleView 그리기

void CSampleView::OnDraw(CDC* /*pDC*/)
{
	CSampleDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);
	if (!pDoc)
		return;

	// TODO: 여기에 원시 데이터에 대한 그리기 코드를 추가합니다.
	CRect aRect;
	GetWindowRect(aRect);
	if(myWidth != aRect.Width() || myHeight != aRect.Height()) {
		myWidth = aRect.Width();
		myHeight = aRect.Height();
		::PostMessage ( GetSafeHwnd () , WM_SIZE , SW_SHOW , myWidth + myHeight*65536 );
	}
	myView->Redraw();
}


// CSampleView 인쇄

BOOL CSampleView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// 기본적인 준비
	return DoPreparePrinting(pInfo);
}

void CSampleView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 인쇄하기 전에 추가 초기화 작업을 추가합니다.
}

void CSampleView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: 인쇄 후 정리 작업을 추가합니다.
}


// CSampleView 진단

#ifdef _DEBUG
void CSampleView::AssertValid() const
{
	CView::AssertValid();
}

void CSampleView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

CSampleDoc* CSampleView::GetDocument() const // 디버그되지 않은 버전은 인라인으로 지정됩니다.
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(CSampleDoc)));
	return (CSampleDoc*)m_pDocument;
}
#endif //_DEBUG


// CSampleView 메시지 처리기

void IntersectTest()
{
	UnE::Geometry::Vertex2D vLine1Begin(0, 0), vLine1End(100, 0), vLine2Begin(50, -50), vLine2End(50, 50);
	UnE::Geometry::Vertex2D vLine1Dir = vLine1End - vLine1Begin;
	UnE::Geometry::Vertex2D vLine2Dir = vLine2End - vLine2Begin;

	Handle(Geom2d_Line) rLine1 = GCE2d_MakeLine(gp_Pnt2d(vLine1Begin.x, vLine1Begin.y), gp_Pnt2d(vLine1End.x, vLine1End.y));
	Handle(Geom2d_Line) rLine2 = GCE2d_MakeLine(gp_Pnt2d(vLine2Begin.x, vLine2Begin.y), gp_Pnt2d(vLine2End.x, vLine2End.y));

	void* voidPtr = &rLine1;
	Handle(Geom2d_Line)* line1Ptr = (Handle(Geom2d_Line)*)voidPtr;
//	Handle(Geom2d_Line) rLineTemp = (Handle(Geom2d_Line))voidPtr;

	Handle(Geom2d_Line) aaa(rLine1);
	aaa.Nullify();

	delete rLine1;

	Geom2dAPI_InterCurveCurve inter(rLine1, rLine2);

	int nbPoints = inter.NbPoints();

	size_t sizeInter = sizeof(Geom2dAPI_InterCurveCurve);
	BYTE* temp = (BYTE*)&inter;

	size_t sizeMyInter = sizeof(Geom2dInt_GInter);
	Geom2dInt_GInter* pMyIntersector = (Geom2dInt_GInter*)&temp[sizeInter - sizeMyInter];
	temp = (BYTE*)pMyIntersector;

	size_t size1 = sizeof(IntRes2d_SequenceOfIntersectionPoint);
	size_t size2 = sizeof(IntRes2d_SequenceOfIntersectionSegment);

	IntRes2d_SequenceOfIntersectionPoint* lpnt = (IntRes2d_SequenceOfIntersectionPoint*)&temp[sizeMyInter - size1 - size2 - (0x418 - 0x288)];
	temp = (BYTE*)lpnt;

	int* pSize = (int*)&temp[size1 - sizeof(int)];

	CString str;

	for (int i=1;i<=nbPoints;i++)
	{
		/*if (i <= 0 || i > *pSize)
			throw "";*/
		gp_Pnt2d pt = inter.Point(i);
		str.Format(_T("(%.2lf, %.2lf)\n"), pt.X(), pt.Y());
		TRACE(str);
	}
}

void CSampleView::OnInitialUpdate()
{
	CView::OnInitialUpdate();

	// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.
	IntersectTest();
	//EArcTest();
	//TrimmedCurve();
	//CubeTest();
}

void CSampleView::EArcTest()
{
	using namespace UnE::Geometry;

	bool isClockWise = true;
	Vertex2D vTL(0.0, 100.0), vBL(0.0, 0.0), vBR(200.0, 0.0);
	Geo::EArc2D arc(vTL, vBL, vBR, Math::HALF_PI(), Math::_3HALF_PI(), isClockWise);

	int nSlice = 10;
	double dA  = arc.GetEArcAngle() / nSlice;
	double dBeginAngle = arc.GetBeginAngle();

	Vertex2D v;
	CString str;

	for (int i=0;i<=nSlice;i++)
	{
		double dAngle = isClockWise ? dBeginAngle - dA * i : dBeginAngle + dA * i;

		if (!arc.GetVertex(dAngle, v))
		{
			str.Format(_T("각도(Radian) %.2lf에 대한 EArc::GetVertex() 호출이 실패하였습니다.\n"), dAngle);
			TRACE(str);
			return;
		}

		str.Format(_T("각도(Radian) : %.2lf => (%.2lf, %.2lf)\n"), dAngle, v.x, v.y);
		TRACE(str);
	}
}

void CSampleView::TrimmedCurve()
{
	using namespace UnE::Geometry;

	Vertex2D v1(0, 5), v2(5.5, 1), v3(-2, 2);
	Geo::Arc2D arc(v1, v2, v3);
	Geo::Circle2D circle(v1, v2, v3);

	double dRadius = arc.GetRadius();
	double dArcAngle = arc.GetArcAngle();
	const Vertex2D& vCenter = arc.GetCenter();
	const Vertex2D& vBegin = arc.GetBegin();
	const Vertex2D& vEnd = arc.GetEnd();

	printf("slj");

    /*DisplayCurve(aDoc,C);
    Handle(ISession_Direction) aDirection = new ISession_Direction(P,V);
    aDoc->GetISessionContext()->Display(aDirection, Standard_False);

    DisplayPoint(aDoc,P,"P",false,0.5);

    PostProcess(aDoc,ID_BUTTON_Test_7,TheDisplayType,Message.ToCString());*/
}

void CSampleView::CubeTest()
{
	myView = GetDocument()->GetViewer()->CreateView();

    // set the default mode in wireframe ( not hidden line ! )
    myView->SetDegenerateModeOn();
    // store for restore state after rotation (witch is in Degenerated mode)
    myDegenerateModeIsOn = Standard_True;


	Handle(Graphic3d_WNTGraphicDevice) theGraphicDevice = 
		((CSampleApp*)AfxGetApp())->GetGraphicDevice();
    
    Handle(WNT_Window) aWNTWindow = new WNT_Window(theGraphicDevice,GetSafeHwnd ());
    myView->SetWindow(aWNTWindow);
    if (!aWNTWindow->IsMapped()) aWNTWindow->Map();

//	Standard_Integer w=100 , h=100 ;   /* Debug Matrox                         */
//	aWNTWindow->Size (w,h) ;           /* Keeps me unsatisfied (rlb).....      */
	                                   /* Resize is not supposed to be done on */
	                                   /* Matrox                               */
	                                   /* I suspect another problem elsewhere  */
//	::PostMessage ( GetSafeHwnd () , WM_SIZE , SIZE_RESTORED , w + h*65536 ) ;

    // store the mode ( nothing , dynamic zooming, dynamic ... )
    myCurrentMode = CurAction3d_Nothing;
	myVisMode = VIS_SHADE;

	//onMakeBottle();
	onMakeCube(100, 5, 5, 5);
	RedrawVisMode();
}

TopoDS_Shape
MakeBottle(const Standard_Real myWidth , const Standard_Real myHeight , const Standard_Real myThickness);

void MoveShape(TopoDS_Shape& rShape, int nIndex)
{
	gp_Trsf transform;
	transform.SetTranslation(gp_Vec(0.0, nIndex * 300, 0.0));
	TopLoc_Location* pLoc = new TopLoc_Location(transform);

	rShape.Location(transform);
}

void MoveShape(TopoDS_Shape& rShape, int x, int y, int z)
{
	gp_Trsf transform;
	transform.SetTranslation(gp_Vec(x, y, z));
	TopLoc_Location* pLoc = new TopLoc_Location(transform);

	rShape.Location(transform);
}

void CSampleView::onMakeBottle()
{
    //QApplication::setOverrideCursor( Qt::WaitCursor );
	int i=1, num = 1;
	bool newCreate = true;

	try
	{
		SYSTEMTIME t1, t2;
		::GetSystemTime(&t1);

		#pragma omp parallel
		{
			if (newCreate)
			{
				#pragma omp for
				for (i=1;i<=num;i++)
				{
					TopoDS_Shape aBottle=MakeBottle(50,70,30);MoveShape(aBottle,i);
					Handle(AIS_Shape) AISBottle=new AIS_Shape(aBottle);
					GetDocument()->GetAISContext()->SetMaterial(AISBottle,Graphic3d_NOM_GOLD);
					GetDocument()->GetAISContext()->SetDisplayMode(AISBottle,1,Standard_False);
					GetDocument()->GetAISContext()->Display(AISBottle, Standard_False);	
					GetDocument()->GetAISContext()->SetCurrentObject(AISBottle,Standard_False);

					CString str;
					str.Format(_T("%d processed\r\n"), i+1);
					TRACE(str);
				}
			}
			else
			{
				TopoDS_Shape aBottle=MakeBottle(50,70,30);
				Handle(AIS_Shape) AISBottle=new AIS_Shape(aBottle);
				GetDocument()->GetAISContext()->SetMaterial(AISBottle,Graphic3d_NOM_GOLD);
				GetDocument()->GetAISContext()->SetDisplayMode(AISBottle,1,Standard_False);
				GetDocument()->GetAISContext()->Display(AISBottle, Standard_False);	
				GetDocument()->GetAISContext()->SetCurrentObject(AISBottle,Standard_False);

				CString str;
				str.Format(_T("1 processed\r\n"));
				TRACE(str);

				#pragma omp for
				for (i=1;i<num;i++)
				{
					BRepBuilderAPI_Copy A;
					A.Perform(aBottle);
					TopoDS_Shape ShapeCopy;
					ShapeCopy=A.Shape();

					MoveShape(ShapeCopy,i);

					Handle(AIS_Shape) AISBottle=new AIS_Shape(ShapeCopy);
					GetDocument()->GetAISContext()->SetMaterial(AISBottle,Graphic3d_NOM_GOLD);
					GetDocument()->GetAISContext()->SetDisplayMode(AISBottle,1,Standard_False);
					GetDocument()->GetAISContext()->Display(AISBottle, Standard_False);	
					GetDocument()->GetAISContext()->SetCurrentObject(AISBottle,Standard_False);

					str.Format(_T("%d processed\r\n"), i+1);
					TRACE(str);
				}
			}
		}

		::GetSystemTime(&t2);

		int nMin = t2.wMinute - t1.wMinute;
		if (nMin < 0) nMin += 60;

		int nSec = t2.wSecond - t1.wSecond;
		if (nSec < 0) nSec += 60;

		CString strTime;
		strTime.Format(_T("%d:%d\r\n"), nMin, nSec);

#ifdef _DEBUG
		TRACE(strTime);
#else
		MessageBox(strTime);
#endif
	}
	catch (Standard_Failure e)
	{
		Standard_CString str = e.GetMessageString();
		printf("slj");
	}
    /*emit*/ selectionChanged();
    //fitAll();
    //QApplication::restoreOverrideCursor();
}

TopoDS_Shape MakeCube(int nCubeLen, int nRowCount, int nColCount, int nDepthCount);

void CSampleView::onMakeCube(int nCubeLen, int nRowCount, int nColCount, int nDepthCount)
{
	TopoDS_Shape cube = MakeCube(nCubeLen, nRowCount, nColCount, nDepthCount);

	Handle(AIS_Shape) AISCube = new AIS_Shape(cube);
	GetDocument()->GetAISContext()->SetMaterial(AISCube,Graphic3d_NOM_GOLD);
	GetDocument()->GetAISContext()->SetDisplayMode(AISCube,1,Standard_False);
	GetDocument()->GetAISContext()->Display(AISCube, Standard_False);	
	GetDocument()->GetAISContext()->SetCurrentObject(AISCube,Standard_False);

	//int i=1, num = 1;
	bool newCreate = false;

	try
	{
		SYSTEMTIME t1, t2;
		::GetSystemTime(&t1);

		#pragma omp parallel
		{
			if (newCreate)
			{
				#pragma omp for
				for (int i=0;i<nRowCount;i++)
				{
					for (int j=0;j<nColCount;j++)
					{
						for (int k=0;k<nDepthCount;k++)
						{
							if (i == 0 && j == 0 && k == 0) continue;

							TopoDS_Shape aCube = MakeCube(nCubeLen, 1, 1, 1);MoveShape(aCube, i * nCubeLen * 2, j * nCubeLen * 2, k * nCubeLen * 2);
							Handle(AIS_Shape) AISCube = new AIS_Shape(aCube);
							GetDocument()->GetAISContext()->SetMaterial(AISCube,Graphic3d_NOM_GOLD);
							GetDocument()->GetAISContext()->SetDisplayMode(AISCube,1,Standard_False);
							GetDocument()->GetAISContext()->Display(AISCube, Standard_False);	
							GetDocument()->GetAISContext()->SetCurrentObject(AISCube,Standard_False);

							CString str;
							str.Format(_T("%d processed\r\n"), i+1);
							TRACE(str);
						}
					}
				}
			}
			else
			{
				/*Handle(AIS_Shape) AISCube = new AIS_Shape(cube);
				GetDocument()->GetAISContext()->SetMaterial(AISCube,Graphic3d_NOM_GOLD);
				GetDocument()->GetAISContext()->SetDisplayMode(AISCube,1,Standard_False);
				GetDocument()->GetAISContext()->Display(AISCube, Standard_False);	
				GetDocument()->GetAISContext()->SetCurrentObject(AISCube,Standard_False);*/

				CString str;
				str.Format(_T("1 processed\r\n"));
				TRACE(str);

				#pragma omp for
				for (int i=0;i<nRowCount;i++)
				{
					for (int j=0;j<nColCount;j++)
					{
						for (int k=0;k<nDepthCount;k++)
						{
							BRepBuilderAPI_Copy A;
							A.Perform(cube);
							TopoDS_Shape ShapeCopy;
							ShapeCopy=A.Shape();

							MoveShape(ShapeCopy, i * nCubeLen * 2, j * nCubeLen * 2, k * nCubeLen * 2);

							Handle(AIS_Shape) AISCube = new AIS_Shape(ShapeCopy);
							GetDocument()->GetAISContext()->SetMaterial(AISCube,Graphic3d_NOM_GOLD);
							GetDocument()->GetAISContext()->SetDisplayMode(AISCube,1,Standard_False);
							GetDocument()->GetAISContext()->Display(AISCube, Standard_False);	
							GetDocument()->GetAISContext()->SetCurrentObject(AISCube,Standard_False);

							str.Format(_T("%d processed\r\n"), i+1);
							TRACE(str);
						}
					}
				}
			}
		}

		::GetSystemTime(&t2);

		int nMin = t2.wMinute - t1.wMinute;
		if (nMin < 0) nMin += 60;

		int nSec = t2.wSecond - t1.wSecond;
		if (nSec < 0) nSec += 60;

		CString strTime;
		strTime.Format(_T("%d:%d\r\n"), nMin, nSec);

#ifdef _DEBUG
		TRACE(strTime);
#else
		MessageBox(strTime);
#endif
	}
	catch (Standard_Failure e)
	{
		Standard_CString str = e.GetMessageString();
		printf("slj");
	}
    /*emit*/ selectionChanged();
}

void CSampleView::selectionChanged()
{
    //QMetaObject::activate(this, &staticMetaObject, 0, 0);
}

void CSampleView::RedrawVisMode()
{
  switch (myVisMode)
  {
  case VIS_WIREFRAME:
    GetDocument()->GetAISContext()->SetDisplayMode(AIS_WireFrame);
    myView->SetComputedMode (Standard_False);
    break;
  case VIS_SHADE:
    GetDocument()->GetAISContext()->SetDisplayMode(AIS_Shaded);
    myView->SetComputedMode (Standard_False);
    break;
  case VIS_HLR:
    SetCursor(AfxGetApp()->LoadStandardCursor(IDC_WAIT));
    myView->SetComputedMode (Standard_True);
    SetCursor(AfxGetApp()->LoadStandardCursor(IDC_ARROW));
    GetDocument()->GetAISContext()->SetDisplayMode(AIS_WireFrame);
    break;
  }
}

void CSampleView::OnSize(UINT nType, int cx, int cy)
{
	CView::OnSize(nType, cx, cy);

	// TODO: 여기에 메시지 처리기 코드를 추가합니다.
	if (!myView.IsNull())
	myView->MustBeResized();
}

BOOL CSampleView::OnEraseBkgnd(CDC* pDC)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	return FALSE;
	//return CView::OnEraseBkgnd(pDC);
}

void CSampleView::OnMouseMove(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	if (nFlags & MK_LBUTTON)
	{
		myView->Pan(point.x-myXmax,myYmax-point.y); // Realize the panning
		myXmax = point.x; myYmax = point.y;	
	}
	else if (nFlags & MK_RBUTTON)
	{
		myView->Rotation(point.x,point.y);
	}

	//CView::OnMouseMove(nFlags, point);
}

void CSampleView::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	myXmin=point.x;  myYmin=point.y;
	myXmax=point.x;  myYmax=point.y;

	//CView::OnLButtonDown(nFlags, point);
}

BOOL CSampleView::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.
	if (zDelta < 0)
	{
		viewScale *= 0.9;
		if (viewScale < 0.1) viewScale = 0.1;
	}
	else
	{
		viewScale *= 1.1;
	}

	myView->SetScale(viewScale);

	return TRUE;
	//return CView::OnMouseWheel(nFlags, zDelta, pt);
}

void CSampleView::OnRButtonDown(UINT nFlags, CPoint point)
{
	// TODO: 여기에 메시지 처리기 코드를 추가 및/또는 기본값을 호출합니다.

	myView->StartRotation(point.x,point.y);
	//CView::OnRButtonDown(nFlags, point);
}

void CSampleView::OnWireMode()
{
	// TODO: 여기에 명령 처리기 코드를 추가합니다.
	myVisMode = VIS_WIREFRAME;
	RedrawVisMode();
}

void CSampleView::OnUpdateWireMode(CCmdUI *pCmdUI)
{
	// TODO: 여기에 명령 업데이트 UI 처리기 코드를 추가합니다.
	if (myVisMode == VIS_WIREFRAME) pCmdUI->SetCheck();
	else pCmdUI->SetCheck(0);
}

void CSampleView::OnShadeMode()
{
	// TODO: 여기에 명령 처리기 코드를 추가합니다.
	myVisMode = VIS_SHADE;
	RedrawVisMode();
}

void CSampleView::OnUpdateShadeMode(CCmdUI *pCmdUI)
{
	// TODO: 여기에 명령 업데이트 UI 처리기 코드를 추가합니다.
	if (myVisMode == VIS_SHADE) pCmdUI->SetCheck();
	else pCmdUI->SetCheck(0);
}

void CSampleView::OnHlrMode()
{
	// TODO: 여기에 명령 처리기 코드를 추가합니다.
	myVisMode = VIS_HLR;
	RedrawVisMode();
}

void CSampleView::OnUpdateHlrMode(CCmdUI *pCmdUI)
{
	// TODO: 여기에 명령 업데이트 UI 처리기 코드를 추가합니다.
	if (myVisMode == VIS_HLR) pCmdUI->SetCheck();
	else pCmdUI->SetCheck(0);
}
