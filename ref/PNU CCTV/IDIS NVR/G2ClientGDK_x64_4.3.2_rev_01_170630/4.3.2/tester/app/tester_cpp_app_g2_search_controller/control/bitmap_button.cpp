// KissButton.cpp : implementation file
//

#include "stdafx.h"
#include "bitmap_button.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

bitmap_button::bitmap_button(void)
{
	_is_tracking   = FALSE;
	_is_hover      = FALSE;
	_is_pressed    = FALSE;

    _color_backgnd  = ::GetSysColor(COLOR_BTNFACE);
    for (int i = 0; i < image_::COUNT; ++i) {
        _color_text[i] = RGB(0, 0, 0);
    }
    _color_text_outline = RGB(255, 0, 0);
	_tooltip.Empty();
    _text.Empty();
    _use_outline_text = false;
    _string_format = DT_CENTER | DT_VCENTER | DT_NOPREFIX | DT_SINGLELINE;
    _rect_margin.SetRectEmpty();
}

bitmap_button::~bitmap_button(void)
{
	if (_image_list.GetSafeHandle()) {
		_image_list.DeleteImageList();
	}
}

BEGIN_MESSAGE_MAP(bitmap_button, CButton)
	ON_WM_MOUSEMOVE()
	ON_WM_SIZE()

	ON_MESSAGE(WM_MOUSELEAVE, &bitmap_button::OnMouseLeave)
	ON_MESSAGE(WM_MOUSEHOVER, &bitmap_button::OnMouseHover)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

void bitmap_button::PreSubclassWindow()
{
	ModifyStyle(0, BS_OWNERDRAW);
	GetClientRect(&_rect_client);
	CButton::PreSubclassWindow();
}

BOOL bitmap_button::PreTranslateMessage(MSG* pMsg)
{
    return CButton::PreTranslateMessage(pMsg);
}

void bitmap_button::DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct)
{
	static const POINT ptTemp = { 0, 0 };

	CDC* pDC = CDC::FromHandle(lpDrawItemStruct->hDC);

	int nImage = image_::NORMAL;

	if (lpDrawItemStruct->itemState & ODS_DISABLED) {
		nImage = image_::DISABLE;
	}
	else if (lpDrawItemStruct->itemState & ODS_SELECTED) {
		nImage = image_::PRESSED;
	}
	else {
		if (_is_pressed) {
			nImage = image_::PRESSED;
		}
		else if (_is_hover) {
			nImage = image_::HOVER;
		}
		else {
			nImage = image_::NORMAL;
		}
	}
	_image_list.Draw(pDC, nImage, ptTemp, ILD_NORMAL);

    draw_text_impl(pDC, nImage);
}


LRESULT bitmap_button::WindowProc(UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message) {
    case WM_LBUTTONDOWN:
    case WM_RBUTTONDOWN:
    case WM_MBUTTONDOWN:
    case WM_LBUTTONUP:
    case WM_RBUTTONUP:
    case WM_MBUTTONUP:
    case WM_LBUTTONDBLCLK:
    case WM_RBUTTONDBLCLK:
    case WM_MOUSEMOVE:
        if (_tooltip.IsEmpty() == FALSE) {
            const MSG* lpMsg = GetCurrentMessage();
            if (lpMsg != NULL) {
                MSG msg = *lpMsg;
                _tooltip_ctrl.RelayEvent(&msg);
            }
        }
        break;
    }

    return CButton::WindowProc(message, wParam, lParam);
}

//////////////////////////////////////////////////////////////////////////

void bitmap_button::OnSize(UINT nType, int cx, int cy)
{
    CButton::OnSize(nType, cx, cy);
    GetClientRect(_rect_client);
}

void bitmap_button::OnMouseMove(UINT nFlags, CPoint point)
{
	if (_is_tracking  == FALSE) {
		TRACKMOUSEEVENT tmEvt;
		tmEvt.cbSize     = sizeof(TRACKMOUSEEVENT);
		tmEvt.hwndTrack  = GetSafeHwnd();
		tmEvt.dwFlags    = TME_LEAVE | TME_HOVER;
		tmEvt.dwHoverTime= 1;

		_is_tracking = _TrackMouseEvent(&tmEvt);
	}

	CPoint ptTemp = point;
	ClientToScreen(&ptTemp);

	CRect rctWindow;
	GetWindowRect(rctWindow);

	if (rctWindow.PtInRect(ptTemp) == FALSE) {
		_is_hover = FALSE;
	}
	else {
		_is_hover = TRUE;
	}

	CButton::OnMouseMove(nFlags, point);
}

LRESULT bitmap_button::OnMouseHover(WPARAM wparam, LPARAM lparam)
{
	_is_hover = TRUE;
	RedrawWindow();
	return 0L;
}

LRESULT bitmap_button::OnMouseLeave(WPARAM wparam, LPARAM lparam)
{
	_is_tracking = FALSE;
	_is_hover	   = FALSE;
	_is_pressed  = FALSE;
	RedrawWindow();

	if (_tooltip_ctrl.GetSafeHwnd()) {
		_tooltip_ctrl.Pop();
	}

	return 0L;
}

//////////////////////////////////////////////////////////////////////////

bool bitmap_button::create(CWnd* pParentWnd, const button_info& rbtnInfo)
{
	ASSERT(client::is_valid(pParentWnd));

	_is_tracking   = FALSE;
	_is_hover      = FALSE;
	_is_pressed    = FALSE;

	CRect rctButton(rbtnInfo._rect);

	DWORD dwStyle = WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | WS_VISIBLE;
	dwStyle |= BS_OWNERDRAW;

	bool freturn = false;
	BOOL bReturn = CButton::Create(_T(""), dwStyle, rctButton, pParentWnd, rbtnInfo._button_id);
	if (bReturn) {
		freturn = set_bitmap_button(rbtnInfo);
	}
	else {
		ASSERT(FALSE);
	}

	return freturn;
}

void bitmap_button::enable(bool fenable /*= true*/)
{
    if (client::is_valid(this)) {
        EnableWindow(fenable == true);
    }
}

void bitmap_button::show(bool fshow /*= true*/)
{
    if (client::is_valid(this)) {
        ShowWindow(fshow ? SW_SHOW : SW_HIDE);
    }
}

void bitmap_button::redraw(void)
{
    if (client::is_valid(this)) {
        RedrawWindow();
    }
}

bool bitmap_button::set_image_list(CImageList& rImgList)
{
    ASSERT(rImgList.GetImageCount() == image_::COUNT);
    
	if (_image_list.GetSafeHandle() != NULL) {
		_image_list.DeleteImageList();
	}
    bool ret = _image_list.Create(&rImgList) ? true : false;
    if (ret) Invalidate();
    return ret;
}

void bitmap_button::draw_text_impl(CDC* pDc, int nImage)
{
    if (_text.IsEmpty()) return;

    CRect rctText = _rect_client;
    rctText.DeflateRect(_rect_margin);
    COLORREF clrText = _color_text[nImage];

    int modeBack = pDc->SetBkMode(TRANSPARENT);
    COLORREF clrBack = pDc->SetTextColor(clrText);

    if (_use_outline_text) {
        pDc->SetTextColor(_color_text_outline);
        CRect rctTemp = rctText;
        rctTemp.OffsetRect(-1,-1); pDc->DrawText(_text, rctTemp, _string_format);
        rctTemp.OffsetRect( 2, 0); pDc->DrawText(_text, rctTemp, _string_format);
        rctTemp.OffsetRect( 0, 1); pDc->DrawText(_text, rctTemp, _string_format);
        rctTemp.OffsetRect( 0, 1); pDc->DrawText(_text, rctTemp, _string_format);
        rctTemp.OffsetRect(-2, 0); pDc->DrawText(_text, rctTemp, _string_format);
        rctTemp.OffsetRect( 0,-1); pDc->DrawText(_text, rctTemp, _string_format);
    }

    pDc->SetTextColor(clrText);
    pDc->DrawText(_text, -1, rctText, _string_format);

    pDc->SetTextColor(clrBack);
    pDc->SetBkMode(modeBack);
}

bool bitmap_button::set_bitmap_button(const button_info& rbtnInfo)
{
	const CSize& szBitmap = rbtnInfo._rect.Size();

	ASSERT(szBitmap.cx > 0 && szBitmap.cy > 0);

	bool freturn = false;

	if (rbtnInfo._back_color != -1) {
		_color_backgnd = rbtnInfo._back_color;
	}

    CImageList imgTemp;
    CBitmap    bmpTemp;


    if (bmpTemp.LoadBitmap(rbtnInfo._resource) != TRUE) return false;


    imgTemp.Create(szBitmap.cx, szBitmap.cy, rbtnInfo._flags, image_::COUNT, 1);
	imgTemp.Add(&bmpTemp, rbtnInfo._mask_color);

	freturn = set_image_list(imgTemp);

	bmpTemp.DeleteObject();
	imgTemp.DeleteImageList();

	if (szBitmap.cx > 0 &&
		szBitmap.cy > 0) {
		SetWindowPos(NULL, 0, 0, szBitmap.cx, szBitmap.cy, SWP_NOMOVE | SWP_NOZORDER | SWP_NOREDRAW);
		GetClientRect(&_rect_client);
	}

	if (!rbtnInfo._tooltip.IsEmpty()) {
		set_tooltip_text(rbtnInfo._tooltip);
	}

	return freturn;
}

void bitmap_button::set_tooltip_text(const CString& strToolTip)
{
	if (_tooltip_ctrl.GetSafeHwnd() == NULL) {
		_tooltip_ctrl.Create(this, TTS_ALWAYSTIP);
		_tooltip_ctrl.Activate(FALSE);
		_tooltip_ctrl.AddTool(this, _tooltip, _rect_client, 1);
		_tooltip_ctrl.Activate(TRUE);
	}
	_tooltip = strToolTip;
	_tooltip_ctrl.Activate(FALSE);
	_tooltip_ctrl.SetToolRect(this, 1, _rect_client);
	_tooltip_ctrl.UpdateTipText(_tooltip, this, 1);
	_tooltip_ctrl.Activate(TRUE);
}

void bitmap_button::set_text(const CString& strText)
{
    _text = strText;
}

void bitmap_button::set_text_format(DWORD format)
{
    _string_format = format;
}

void bitmap_button::set_text_color(COLORREF color)
{
    for (int i = 0; i < image_::COUNT; ++i) {
        _color_text[i] = color;
    }
}

void bitmap_button::set_text_margin(const CRect& rect)
{
    _rect_margin = rect;

    RedrawWindow();
}

void bitmap_button::set_text_margin(int left, int top, int right, int bottom)
{
    set_text_margin(CRect(left, top, right, bottom));
}

void bitmap_button::set_text_outline(bool useOutLine, COLORREF color /*= RGB(255, 255, 255)*/)
{
    _use_outline_text = useOutLine;
    if (useOutLine) {
        _color_text_outline = color;
    }
}

void bitmap_button::set_font(CFont* pfont, bool update /*= true*/)
{
    client::is_valid(pfont);

    SetFont(pfont, update ? TRUE : FALSE);
}