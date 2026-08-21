
#pragma once

#include "ViewTree.h"

class CSceneGraphViewToolBar : public CMFCToolBar
{
	virtual void OnUpdateCmdUI(CFrameWnd* /*pTarget*/, BOOL bDisableIfNoHndler)
	{
		CMFCToolBar::OnUpdateCmdUI((CFrameWnd*) GetOwner(), bDisableIfNoHndler);
	}

	virtual BOOL AllowShowOnList() const { return FALSE; }
};

class CSceneGraphView : public CDockablePane
{
// 생성입니다.
public:
	CSceneGraphView();

	void AdjustLayout();
	void OnChangeVisualStyle();

// 특성입니다.
protected:

	CViewTree m_wndFileView;
	CImageList m_FileViewImages;
	CSceneGraphViewToolBar m_wndToolBar;

protected:
	void FillFileView();

// 구현입니다.
public:
	virtual ~CSceneGraphView();

protected:
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	afx_msg void OnProperties();
	afx_msg void OnFileOpen();
	afx_msg void OnFileOpenWith();
	afx_msg void OnDummyCompile();
	afx_msg void OnEditCut();
	afx_msg void OnEditCopy();
	afx_msg void OnEditClear();
	afx_msg void OnPaint();
	afx_msg void OnSetFocus(CWnd* pOldWnd);

	DECLARE_MESSAGE_MAP()
};

