
#pragma once

#include "CoreAPI.h"
#include "UCore3DDoc.h"





class CORE_API UCore3DView : public CView
{
protected: 
	UCore3DView();
	DECLARE_DYNCREATE(UCore3DView)

public:

#ifdef _DEBUG
	virtual void		AssertValid() const;
	virtual void		Dump(CDumpContext& dc) const;
#endif

	virtual				~UCore3DView();
	virtual void		OnDraw(CDC* pDC);  
	virtual void		OnInitialUpdate();

	UCore3DDoc*			GetDocument() const;



protected:
	BOOL m_bMouseOrbitMode;
	BOOL m_bMousePanMode;
	BOOL m_bMouseZoomMode;

	BOOL m_bViewWorldAABB;
	BOOL m_bUseTrackBall;

	CPoint mPtStart;
	CPoint mPtCur;
	CPoint mPtPrev;


protected:
	DECLARE_MESSAGE_MAP()

public:	
	afx_msg BOOL		OnEraseBkgnd(CDC* pDC);
	afx_msg void		OnSize(UINT nType, int cx, int cy);
	afx_msg void		OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg void		OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void		OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void		OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void		OnMButtonDown(UINT nFlags, CPoint point);
	afx_msg void		OnMButtonUp(UINT nFlags, CPoint point);
	afx_msg void		OnMouseMove(UINT nFlags, CPoint point);
	afx_msg BOOL		OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);

	// SHOW/HIDE ROOT SCENE NODE AXISALIGNBOX
	afx_msg void		OnViewWorldAabb();
	afx_msg void		OnUpdateViewWorldAabb(CCmdUI *pCmdUI);

	// ENABLE/DISABLE TRACKBALL ROTATION
	afx_msg void		OnUseTrackball();
	afx_msg void		OnUpdateUseTrackball(CCmdUI *pCmdUI);



};

#ifndef _DEBUG  
inline UCore3DDoc* UCore3DView::GetDocument() const
{ return reinterpret_cast<UCore3DDoc*>(m_pDocument); }
#endif

