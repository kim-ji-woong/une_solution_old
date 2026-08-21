#pragma once

#include "UMouseOperator.h"

class CUBMLViewerView : public CView
{
protected:
	CUBMLViewerView();
	DECLARE_DYNCREATE(CUBMLViewerView)


public:
	CUBMLViewerDoc* GetDocument() const;

	bool bInit;

public:
	virtual void OnDraw(CDC* pDC);  
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

public:
	virtual ~CUBMLViewerView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:


protected:
	virtual void OnInitialUpdate();
	virtual BOOL DestroyWindow();
	virtual LRESULT WindowProc(UINT message, WPARAM wParam, LPARAM lParam);


	afx_msg void OnFilePrintPreview();
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg void OnSize(UINT nType, int cx, int cy);

	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnMButtonDown(UINT nFlags, CPoint point);

	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnAddCube();
	afx_msg void OnRemoveCube();
	afx_msg void OnSaveCube();
	afx_msg void OnLoadCube();


private:
	UnE::Core::UBaseView *   m_p3DView;
	UnE::Core::MouseOperator mouseOperator;

	BOOL	m_bViewWireFrame;
	BOOL	m_bViewHiddenLine;
	BOOL	m_bViewPolygon;
	BOOL	m_bViewTextured;

	int		m_nViewMode; // 1 : WireFrame , 2 : Hidden Line , 3 : Shaded , 4 : Texutred


	BOOL	m_bShowOctree;


public:
	afx_msg void OnSelectEntity();
	afx_msg void OnClearSelect();
	afx_msg void OnViewWireframe();
	afx_msg void OnUpdateViewWireframe(CCmdUI *pCmdUI);
	afx_msg void OnTextureSet();
	afx_msg void OnViewHiddenline();
	afx_msg void OnUpdateViewHiddenline(CCmdUI *pCmdUI);
	afx_msg void OnUpdateViewPolygon(CCmdUI *pCmdUI);
	afx_msg void OnViewPolygon();
	afx_msg void OnViewOctree();
	afx_msg void OnUpdateViewOctree(CCmdUI *pCmdUI);
	afx_msg void OnViewTextured();
	afx_msg void OnUpdateViewTextured(CCmdUI *pCmdUI);
	afx_msg void OnUpdateTextureSet(CCmdUI *pCmdUI);
	afx_msg void OnTextureRemove();
	afx_msg void OnUpdateTextureRemove(CCmdUI *pCmdUI);
	afx_msg void OnImportDae();
	afx_msg void OnDeleteEntity();
	afx_msg void OnRemovePoi();
	afx_msg void OnAddPoiType1();
	afx_msg void On32815();
};

#ifndef _DEBUG
inline CUBMLViewerDoc* CUBMLViewerView::GetDocument() const
   { return reinterpret_cast<CUBMLViewerDoc*>(m_pDocument); }
#endif

