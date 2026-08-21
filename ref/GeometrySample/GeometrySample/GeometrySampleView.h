
// GeometrySampleView.h : CGeometrySampleView 클래스의 인터페이스
//

#pragma once


class CGeometrySampleView : public CView
{
protected: // serialization에서만 만들어집니다.
	CGeometrySampleView();
	DECLARE_DYNCREATE(CGeometrySampleView)

// 특성입니다.
public:
	CGeometrySampleDoc* GetDocument() const;

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
	virtual ~CGeometrySampleView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:
	void LineDraw(); 
	
	void OnMenuDMJoin();
	void OnMenuGMEndcap();
	void OnMenuPSINSIDE();

// 생성된 메시지 맵 함수
protected:
	afx_msg void OnFilePrintPreview();
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG  // GeometrySampleView.cpp의 디버그 버전
inline CGeometrySampleDoc* CGeometrySampleView::GetDocument() const
   { return reinterpret_cast<CGeometrySampleDoc*>(m_pDocument); }
#endif

