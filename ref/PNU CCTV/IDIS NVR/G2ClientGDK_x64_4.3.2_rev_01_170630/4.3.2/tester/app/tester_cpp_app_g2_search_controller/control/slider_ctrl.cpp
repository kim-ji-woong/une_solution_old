// slider_ctrl.cpp : implementation file
//

#include "stdafx.h"
#include "slider_ctrl.h"
#include "common/client_functional.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

slider_ctrl::slider_ctrl(void)
{
	_clrFocusRect = GetSysColor(COLOR_BTNFACE);

    _use_default_ctrl = TRUE;
    _is_hover = FALSE;
    _is_pressed = FALSE;

    CBitmap bmpBkgnd, bmpImgList;

    bool loading_slider_ctrl_image = _sliderBackgnd.LoadBitmap(_T("BMP_SLD_SPEED")) &&
                                     bmpBkgnd.LoadBitmap(_T("BMP_DLG_BKGND")) &&
                                     bmpImgList.LoadBitmap(_T("BMP_SLD_THUMB"));

    G2RETURN_IF_ASSERT(loading_slider_ctrl_image);

    _brBackgnd.CreatePatternBrush(&bmpBkgnd);
    _image_list.Create(image_::WIDTH, image_::HEIGHT, ILC_COLOR24, image_::COUNT, image_::COUNT);
    _image_list.Add(&bmpImgList, RGB(255,0,255));
}

slider_ctrl::~slider_ctrl(void)
{
	SAFE_DELETE_GDIOBJ(_brBackgnd);
    SAFE_DELETE_GDIOBJ(_sliderBackgnd);

    if (_image_list.GetSafeHandle()) {
        _image_list.DeleteImageList();
    }
}

BEGIN_MESSAGE_MAP(slider_ctrl, CSliderCtrl)
	ON_NOTIFY_REFLECT(NM_CUSTOMDRAW, OnCustomDraw)
	ON_WM_ERASEBKGND()
	ON_WM_SIZE()
    ON_WM_MOUSEMOVE()
    ON_WM_LBUTTONDOWN()
    ON_WM_LBUTTONUP()
    ON_WM_RBUTTONDOWN()
    ON_WM_RBUTTONUP()

    ON_MESSAGE(WM_MOUSELEAVE, &slider_ctrl::OnMouseLeave)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////

void slider_ctrl::PreSubclassWindow()
{
	CSliderCtrl::PreSubclassWindow();
	GetClientRect(_rctClient);
}

void slider_ctrl::OnSize(UINT nType, int cx, int cy)
{
	CSliderCtrl::OnSize(nType, cx, cy);
	GetClientRect(_rctClient);
}

BOOL slider_ctrl::OnEraseBkgnd(CDC* pDC)
{
	return TRUE;
}

void slider_ctrl::OnLButtonDown(UINT nFlags, CPoint point)
{
    if (_rctThumb.PtInRect(point)) {
        _is_pressed = TRUE;
        ClearSel(TRUE);
    }

    CSliderCtrl::OnLButtonDown(nFlags, point);
}

void slider_ctrl::OnLButtonUp(UINT nFlags, CPoint point)
{
    _is_pressed = FALSE;

    CSliderCtrl::OnLButtonUp(nFlags, point);
}

void slider_ctrl::OnRButtonDown(UINT nFlags, CPoint point)
{
    if (GetPos() != 0) {
        SetPos(0);

        CWnd* parent = GetParent();
        if (parent) {
            parent->PostMessage(um_controller_::UM_SLIDER_CHANGED);
        }
    }
    
    CSliderCtrl::OnRButtonDown(nFlags, point);
}

void slider_ctrl::OnRButtonUp(UINT nFlags, CPoint point)
{
    CSliderCtrl::OnRButtonUp(nFlags, point);
}

void slider_ctrl::OnMouseMove(UINT nFlags, CPoint point)
{
    BOOL isPtInRect = _rctThumb.PtInRect(point);

    if (isPtInRect != _is_hover) {
        _is_hover = isPtInRect;
        ClearSel(TRUE);
    }

    CSliderCtrl::OnMouseMove(nFlags, point);
}

LRESULT slider_ctrl::OnMouseLeave(WPARAM wparam, LPARAM lparam)
{
    _is_hover = FALSE;
    _is_pressed = FALSE;
    
    ClearSel(TRUE);

    return 0L;
}

//////////////////////////////////////////////////////////////////////////

void slider_ctrl::OnCustomDraw(NMHDR* pNMHDR, LRESULT* pResult) 
{
	LPNMCUSTOMDRAW lpDraw = reinterpret_cast<LPNMCUSTOMDRAW>(pNMHDR);
	CDC* lpDC = CDC::FromHandle(lpDraw->hdc);

	switch(lpDraw->dwDrawStage) {
	case CDDS_PREPAINT:
		*pResult = (_use_default_ctrl == FALSE) ? 
					CDRF_NOTIFYITEMDRAW | CDRF_NOTIFYPOSTPAINT :
					CDRF_NOTIFYPOSTPAINT;
		break;

	case CDDS_ITEMPREPAINT:
		switch(lpDraw->dwItemSpec) {
		case TBCD_TICS :
			*pResult = CDRF_DODEFAULT;
			break;
		case TBCD_THUMB :
			DrawThumb(lpDC, lpDraw);
			*pResult = CDRF_SKIPDEFAULT;
			break;
		case TBCD_CHANNEL :
			DrawChannel(lpDC, lpDraw);
			*pResult = CDRF_SKIPDEFAULT;
			break;
		}
        Invalidate();
		break;
	}
}

/////////////////////////////////////////////////////////////////////////////

void slider_ctrl::DrawChannel(CDC* lpDC, LPNMCUSTOMDRAW lpDraw)
{
    if (client::is_valid(_brBackgnd)) {
        lpDC->FillRect(_rctClient, &_brBackgnd);
    }

	CRect rctTraker(lpDraw->rc);
    rctTraker.InflateRect(0, 1, 0, -1);

    CRect rctClient;
    GetClientRect(rctClient);

    CDC dc;
    dc.CreateCompatibleDC(lpDC);
    CBitmap backbufferBitmap;
    backbufferBitmap.CreateCompatibleBitmap(lpDC, rctClient.Width(), rctClient.Height());
    CBitmap* backbufferoldbitmap = reinterpret_cast<CBitmap*>(dc.SelectObject(&backbufferBitmap));

    CDC memdc;
    memdc.CreateCompatibleDC(lpDC);

    CBitmap* oldbitmap = memdc.SelectObject(&_sliderBackgnd);

    dc.StretchBlt(0, 0, channel_::BORDER, rctTraker.Height(),
                  &memdc,
                  0, 0, channel_::BORDER, channel_::HEIGHT,
                  SRCCOPY);

    dc.StretchBlt(channel_::BORDER, 0, rctTraker.Width() - (channel_::BORDER * 2), rctTraker.Height(),
                  &memdc,
                  channel_::BORDER, 0, channel_::WIDTH - (channel_::BORDER * 2), channel_::HEIGHT,
                  SRCCOPY);

    dc.StretchBlt(rctTraker.Width() - channel_::BORDER, 0, channel_::BORDER, rctTraker.Height(),
                  &memdc,
                  channel_::WIDTH - channel_::BORDER, 0, channel_::BORDER, channel_::HEIGHT,
                  SRCCOPY);

    lpDC->BitBlt(rctTraker.left, rctTraker.top,
                 rctTraker.Width(), rctTraker.Height(),
                 &dc,
                 0, 0,
                 SRCCOPY);

    // restore //////////////////////////////////
    memdc.SelectObject(oldbitmap);
    memdc.DeleteDC();
    dc.SelectObject(backbufferoldbitmap);
    dc.DeleteDC();
}

void slider_ctrl::DrawThumb(CDC* lpDC, LPNMCUSTOMDRAW lpDraw)
{
	GetThumbRect(_rctThumb);
    _rctThumb.InflateRect(0, 0, 0, -1);

    int nImage = image_::NORMAL;

    if (lpDraw->uItemState & ODS_DISABLED) {
        nImage = image_::DISABLE;
    }
    else if (_is_pressed) {
        nImage = image_::PRESSED;
    }
    else if (_is_hover) {
        nImage = image_::HOVER;
    }
    else {
        nImage = image_::NORMAL;
    }

    POINT ptTemp = {_rctThumb.left, _rctThumb.top};
    _image_list.Draw(lpDC, nImage, ptTemp, ILD_NORMAL);
}
