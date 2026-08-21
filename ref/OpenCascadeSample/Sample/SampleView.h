// SampleView.h : CSampleView 클래스의 인터페이스
//


#pragma once
#include <V3d_View.hxx>

enum View3D_CurrentAction { 
  CurAction3d_Nothing,
  CurAction3d_DynamicZooming,
  CurAction3d_WindowZooming,
  CurAction3d_DynamicPanning,
  CurAction3d_GlobalPanning,
  CurAction3d_DynamicRotation,
  CurAction3d_BeginSpotLight,
  CurAction3d_TargetSpotLight,
  CurAction3d_EndSpotLight,
  CurAction3d_BeginPositionalLight,
  CurAction3d_BeginDirectionalLight,
  CurAction3d_EndDirectionalLight
};

class CSampleView : public CView
{
protected: // serialization에서만 만들어집니다.
	CSampleView();
	DECLARE_DYNCREATE(CSampleView)

// 특성입니다.
public:
	CSampleDoc* GetDocument() const;

// 작업입니다.
public:

// 재정의입니다.
public:
	virtual void OnDraw(CDC* pDC);  // 이 뷰를 그리기 위해 재정의되었습니다.
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// 구현입니다.
public:
	virtual ~CSampleView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

	void onMakeCube(int nCubeLen, int nRowCount, int nColCount, int nDepthCount);
	void onMakeBottle();
	void selectionChanged();

protected:
	Handle_V3d_View     myView;
	Standard_Boolean     myDegenerateModeIsOn;
	View3D_CurrentAction myCurrentMode;

	Standard_Integer     myXmin;
    Standard_Integer     myYmin;  
    Standard_Integer     myXmax;
    Standard_Integer	 myYmax;
	Standard_Integer myWidth;
	Standard_Integer myHeight;

	Quantity_Factor viewScale;

	enum VisMode { VIS_WIREFRAME, VIS_SHADE, VIS_HLR };
	VisMode              myVisMode;

protected:
	void CubeTest();
	void TrimmedCurve();
	void EArcTest();

// 생성된 메시지 맵 함수
protected:
	DECLARE_MESSAGE_MAP()
public:
	virtual void OnInitialUpdate();
	void RedrawVisMode();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnWireMode();
	afx_msg void OnUpdateWireMode(CCmdUI *pCmdUI);
	afx_msg void OnShadeMode();
	afx_msg void OnUpdateShadeMode(CCmdUI *pCmdUI);
	afx_msg void OnHlrMode();
	afx_msg void OnUpdateHlrMode(CCmdUI *pCmdUI);
};

#ifndef _DEBUG  // SampleView.cpp의 디버그 버전
inline CSampleDoc* CSampleView::GetDocument() const
   { return reinterpret_cast<CSampleDoc*>(m_pDocument); }
#endif

