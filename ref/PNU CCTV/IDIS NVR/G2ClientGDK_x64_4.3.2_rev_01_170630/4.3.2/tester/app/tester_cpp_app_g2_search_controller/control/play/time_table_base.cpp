// time_table_base.cpp : implementation file
//

#include "stdafx.h"
#include "time_table_base.h"

#include <common/client_functional.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

time_table_base::time_table_base(void)
    : _parent(NULL)
    , _listener(NULL)
    , _beginOffset(0)
    , _selectPos(-1)
    , _initialized(false)
    , _enable(false)
    , _created(false)
    , _surface_size(0, 0)
{

}

time_table_base::~time_table_base(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

IMPLEMENT_DYNAMIC(time_table_base, CWnd)

void time_table_base::DoDataExchange(CDataExchange* pDX)
{
    CWnd::DoDataExchange(pDX);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(time_table_base, CWnd)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_SIZE()
    ON_WM_ERASEBKGND()
    ON_WM_LBUTTONDOWN()
    ON_WM_LBUTTONUP()
    ON_WM_MOUSEMOVE()
    ON_WM_SETCURSOR()
    ON_WM_VSCROLL()
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

int time_table_base::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CWnd::OnCreate(lpCreateStruct) == -1) {
        return -1;
    }

    ModifyStyle(0, WS_CLIPCHILDREN | WS_CLIPSIBLINGS);
    initialize();

    return 0;
}

void time_table_base::OnDestroy()
{
    CWnd::OnDestroy();
}

void time_table_base::OnSize(UINT nType, int cx, int cy)
{
    CWnd::OnSize(nType, cx, cy);

    if (_created == false) return;

    CRect rect;
    GetClientRect(&rect);
    rect.DeflateRect(default_::TIMETABLE_BORDER, default_::TIMETABLE_BORDER, 
                     default_::TIMETABLE_BORDER, default_::TIMETABLE_BORDER);

    if (_rect != rect) {
        _rect = rect;
        _rectHead = _rect;
        _rectHead.bottom = default_::TIMETABLE_HEAD_HEIGHT + default_::TIMETABLE_TIME_BOUNDARY_HEIGHT;

        _rectBody = _rect;
        _rectBody.top = _rectHead.bottom;

        int width = _surface_size.cx > 0 ? _surface_size.cx : default_::TIMETABLE_WIDTH;
        if (_rect.Width() > width) {
            _beginOffset = 0;
        }

        initialize_surface(width, _rect.Height());
    }
}

BOOL time_table_base::OnEraseBkgnd(CDC* pDC)
{
    render();
    present(pDC);

    return TRUE;
}

void time_table_base::OnLButtonDown(UINT nFlags, CPoint point)
{
    CWnd::OnLButtonDown(nFlags, point);

    if (_enable != true) {
        return;
    }

    SetFocus();

    if (_rectHead.PtInRect(point)) {
        SetCursor(_cursor[cursor_::GRIP]);
        _dragMode = drag_::DATETIME;
        _beginPoint = point;
    }
    else if (_rectBody.PtInRect(point)) {
        SetCursor(_cursor[cursor_::FINGER]);
        _dragMode = drag_::TIMEBAR;

        point.x -= default_::TIMETABLE_BORDER;
        point.y -= default_::TIMETABLE_BORDER;
        set_select_pos(point.x + _beginOffset);
    }
    else {
        return;
    }

    SetCapture();
    Invalidate();
}

void time_table_base::OnLButtonUp(UINT nFlags, CPoint point)
{
    CWnd::OnLButtonUp(nFlags, point);

    if (_enable != true) {
        return;
    }

    if (_dragMode == drag_::TIMEBAR) {
        point.x -= default_::TIMETABLE_BORDER;
        point.y -= default_::TIMETABLE_BORDER;
        set_select_pos(point.x + _beginOffset);
    }

    if (GetCapture() == this) {
        ReleaseCapture();
    }
    _dragMode = drag_::OFF;
}

void time_table_base::OnMouseMove(UINT nFlags, CPoint point)
{
    if (_enable != true) {
        return;
    }

    bool succeeded = true;
    switch (_dragMode) {
        case drag_::DATETIME:
        case drag_::TIMEBAR:
            if (GetCapture() != this) {
                _dragMode = drag_::OFF;
                succeeded = false;
            }
            break;
    }

    if (succeeded != true) return;

    CWnd::OnMouseMove(nFlags, point);

    if (_rect.PtInRect(point) != TRUE) {
        return;
    }

    if (_dragMode == drag_::DATETIME) {
        _beginOffset += (_beginPoint.x - point.x);
        _beginPoint = point;

        int width = _surface_size.cx > 0 ? _surface_size.cx : default_::TIMETABLE_WIDTH;
        if (_beginOffset < 0 ||
            _rect.Width() > width) {
            _beginOffset = 0;
        }
        else if (_beginOffset + _rect.Width() > width) {
            _beginOffset = width - _rect.Width();
        }
    }
    else if (_dragMode == drag_::TIMEBAR) {
        point.x -= default_::TIMETABLE_BORDER;
        point.y -= default_::TIMETABLE_BORDER;
        set_select_pos(point.x + _beginOffset);
    }

    if ((nFlags & MK_LBUTTON) == MK_LBUTTON &&
        (_dragMode == drag_::DATETIME || _dragMode == drag_::TIMEBAR)) {
        Invalidate();
    }
}

BOOL time_table_base::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
{
    if (this == NULL ||
        ::IsWindow(GetSafeHwnd()) != TRUE ||
        _enable != true) {
        return CWnd::OnSetCursor(pWnd, nHitTest, message);
    }
    CPoint point;
    GetCursorPos(&point);
    ScreenToClient(&point);

    int cursor = -1;
    if (_rectHead.PtInRect(point)) {
        cursor = cursor_::HAND;
    }
    else if (_rectBody.PtInRect(point)) {
        cursor = cursor_::FINGER;
    }
    if (cursor != -1) {
        SetCursor(_cursor[cursor]);
    }
    return (cursor != -1) ? TRUE : CWnd::OnSetCursor(pWnd, nHitTest, message);
}

void time_table_base::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
    CWnd::OnVScroll(nSBCode, nPos, pScrollBar);
}

//////////////////////////////////////////////////////////////////////////

bool time_table_base::create(CWnd* parent, const CRect& rect, unsigned int id)
{
    if (parent == NULL ||
        ::IsWindow(parent->GetSafeHwnd()) != TRUE) {
        return false;
    }

    _parent = parent;
    _rect = rect;
    _rect.DeflateRect(default_::TIMETABLE_BORDER, default_::TIMETABLE_BORDER, 
                      default_::TIMETABLE_BORDER, default_::TIMETABLE_BORDER);

    _created = CWnd::Create(NULL,
                                _T("g2client play time table"),
                                WS_VISIBLE | WS_CHILD,
                                rect,
                                parent,
                                id) ? true : false;

    

    if (_created == false) {
        assert(!"failed to create timetable");
    }
    return _created;
}

void time_table_base::set_listenr(listener_time_table* listener)
{
    _listener = listener;
}

void time_table_base::clear(void)
{
    _beginOffset = 0;
    _selectPos = -1;
}

void time_table_base::show(bool show)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    ShowWindow(show ? SW_SHOW : SW_HIDE);
}

void time_table_base::resize(const CRect& rect)
{
    int width = rect.Width();

    CRect temp(rect);

    if (temp.Width() > _surface_size.cx) {
        temp.right = temp.left + _surface_size.cx;
    }

    MoveWindow(&temp);
}

//////////////////////////////////////////////////////////////////////////

void time_table_base::initialize(void)
{
    CWinApp* app = AfxGetApp();
    _cursor[cursor_::FINGER] = app->LoadCursor(IDC_CURSOR_FINGER);
    _cursor[cursor_::HAND]   = app->LoadCursor(IDC_CURSOR_HAND);
    _cursor[cursor_::GRIP]   = app->LoadCursor(IDC_CURSOR_GRIP);

    client::create_font(_font, 8);

    CRect rect;
    GetClientRect(&rect);
    rect.DeflateRect(default_::TIMETABLE_BORDER, default_::TIMETABLE_BORDER, 
                     default_::TIMETABLE_BORDER, default_::TIMETABLE_BORDER);

    initialize_surface(rect.Width(), rect.Height());

    _initialized = true;
}

void time_table_base::finalize(void)
{
    finalize_surface();

    _initialized = false;
}

void time_table_base::initialize_surface(int cx, int cy)
{
    g2::scoped_criticalsection lock(_lock_surface);

    finalize_surface();
    
    _surface_size.cx = cx;
    _surface_size.cy = cy;

    try {
        _dc.CreateDC(_T("DISPLAY"), NULL, NULL, NULL);
        if (_dc.GetSafeHdc() == NULL) {
            throw false;
        }

        prepare_bitmap_info(cx, cy, 32, _surfaceInfo);
        _surfaceDIB = ::CreateDIBSection(_dc, &_surfaceInfo, DIB_RGB_COLORS, &_surfaceBuffer, NULL, 0);

        if (_surface.CreateCompatibleDC(&_dc)) {
            _surface.SelectObject(_surfaceDIB);
        }
    }
    catch (...) {
        assert(!"failed initialize surface");
    }
}

void time_table_base::finalize_surface(void)
{
    _surface_size.cx = 0;
    _surface_size.cy = 0;

    _surface.DeleteDC();
    _dc.DeleteDC();
    DeleteObject(_surfaceDIB);
}

void time_table_base::draw_backgnd(CDC* pDC)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }

    const int width = _surface_size.cx > 0 ? _surface_size.cx :__max(default_::TIMETABLE_WIDTH, _rect.Width());
    const int height = _rect.Height();

    pDC->FillSolidRect(0, 0, width, height, RGB(230, 230, 230));
    pDC->FillSolidRect(0, 0, width, default_::TIMETABLE_HEAD_HEIGHT, RGB(210, 210, 210));
    pDC->FillSolidRect(0, default_::TIMETABLE_HEAD_HEIGHT, width, default_::TIMETABLE_TIME_BOUNDARY_HEIGHT, RGB(180, 180, 180));
}

void time_table_base::draw_title(CDC* pDC)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }
}

void time_table_base::draw_select_spot(CDC* pDC)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }

    if (_selectPos != -1) {
        pDC->FillSolidRect(_selectPos, _rectBody.top, 1, _rectBody.Height(), RGB(255, 0, 0));
    }
}

void time_table_base::render(void)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    g2::scoped_criticalsection lock(_lock_surface);

    draw_backgnd(&_surface);
    draw_time(&_surface);
    draw_time_bar(&_surface);
    draw_select_spot(&_surface);
    draw_title(&_surface);
}

void time_table_base::present(CDC* pDC)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    int width = _surface_size.cx > 0 ? _surface_size.cx :__max(default_::TIMETABLE_WIDTH, _rect.Width());

    CBrush brush(RGB(50, 50, 50));
    for (int i = 0; i < default_::TIMETABLE_BORDER; ++i) {
        CRect rect(CPoint(i, i), CSize(_rect.Width() + ((default_::TIMETABLE_BORDER - i) * 2), _rect.Height() + ((default_::TIMETABLE_BORDER - i) * 2)));
        pDC->FrameRect(&rect, &brush);
    }

    pDC->BitBlt(default_::TIMETABLE_BORDER, default_::TIMETABLE_BORDER, 
                _rect.Width(), _rect.Height(), 
                &_surface , 
                _beginOffset, 0, 
                SRCCOPY);
}

void time_table_base::prepare_bitmap_info(int cx, int cy, int bpp, BITMAPINFO& bi)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    bi.bmiHeader.biSize          = sizeof(BITMAPINFOHEADER);
    bi.bmiHeader.biPlanes        = 1;
    bi.bmiHeader.biBitCount      = bpp;
    bi.bmiHeader.biCompression   = BI_RGB;
    bi.bmiHeader.biXPelsPerMeter = 0;
    bi.bmiHeader.biYPelsPerMeter = 0;
    bi.bmiHeader.biClrUsed       = 0;
    bi.bmiHeader.biClrImportant  = 0;
    bi.bmiHeader.biWidth         = cx;
    bi.bmiHeader.biHeight        = cy;
    bi.bmiHeader.biSizeImage     = ((((cx * bpp) + 31) & ~31) >> 3) * cy;
}

bool time_table_base::set_select_pos(int pos, bool request /*= true*/)
{
    G2RETURN_VAL_IF_FAIL(GetSafeHwnd(), false);

    if (_selectPos == pos) {
        return false;
    }

    int width = _surface_size.cx > 0 ? _surface_size.cx : default_::TIMETABLE_WIDTH;
    if (pos < 0 ||
        pos > width) {
        return false;
    }
    
    if (_selectPos == -1) {
        _beginOffset = pos - _rect.Width() / 2;

        if (_beginOffset < 0) {
            _beginOffset = 0;
        }
        else if (_beginOffset + _rect.Width() > width) {
            _beginOffset = width - _rect.Width();
        }
    }
    else {
        if (pos < _beginOffset) {
            _beginOffset = pos - _rect.Width();
            if (_beginOffset < 0) {
                _beginOffset = 0;
            }
        }
        else if (pos > (_beginOffset + _rect.Width())) {
            if (pos > width - _rect.Width()) {
                _beginOffset = width - _rect.Width();
            }
            else {
                _beginOffset = pos;
            }
        }
    }

    _selectPos = pos;

    Invalidate();

    return true;
}
