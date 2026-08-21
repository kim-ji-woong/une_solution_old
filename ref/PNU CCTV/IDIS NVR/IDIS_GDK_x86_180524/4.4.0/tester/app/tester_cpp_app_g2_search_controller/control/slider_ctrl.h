// slider_ctrl.h : header file
//

#ifndef _SLIDER_CTRL_H_
#define _SLIDER_CTRL_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

/////////////////////////////////////////////////////////////////////////////

class slider_ctrl : public CSliderCtrl
{
public:
	slider_ctrl();
	virtual ~slider_ctrl();

public:
    struct image_   { enum TYPE { NORMAL = 0, HOVER, PRESSED, DISABLE, COUNT }; 
                      enum SIZE { WIDTH = 10, HEIGHT = 21 }; };
    struct channel_ { enum SIZE { WIDTH = 5, HEIGHT = 4, BORDER = 1 }; };

private:
	COLORREF	_clrFocusRect;
	CRect		_rctClient;
	CRect       _rctThumb;

	BOOL		_use_default_ctrl;
	BOOL        _is_hover;
	BOOL        _is_pressed;

	CBrush		_brBackgnd;
	CBitmap     _sliderBackgnd;
	CImageList  _image_list;

public:
	void SetDefaultCtrl(bool fDefault)   { _use_default_ctrl = fDefault; }
	void SetColorFocus(COLORREF clrRect) { _clrFocusRect = clrRect; }

protected:
	void DrawChannel(CDC* lpDC, LPNMCUSTOMDRAW lpDraw);
	void DrawThumb(CDC* lpDC, LPNMCUSTOMDRAW lpDraw);

protected:
	virtual void PreSubclassWindow();

	DECLARE_MESSAGE_MAP()

	afx_msg void OnCustomDraw(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg LRESULT OnMouseLeave(WPARAM wparam, LPARAM lparam);
};

/////////////////////////////////////////////////////////////////////////////

}; // !_namespace_client

#endif // !_SLIDER_CTRL_H_
