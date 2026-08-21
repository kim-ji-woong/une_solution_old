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
    , _surfaceSize(0, 0)
{
    reset_data_list();
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

    CRect rect;
    GetClientRect(&rect);

    if (_rect != rect) {
        _rect = rect;
        _rectHead = _rect;
        _rectHead.bottom = default_::TIMETABLE_HEAD_HEIGHT + default_::TIMETABLE_TIME_BOUNDARY_HEIGHT;
        _rectBody = _rect;
        _rectBody.top = _rectHead.bottom;

        int width = _surfaceSize.cx > 0 ? _surfaceSize.cx : default_::TIMETABLE_WIDTH;
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

        int width = _surfaceSize.cx > 0 ? _surfaceSize.cx : default_::TIMETABLE_WIDTH;
        if (_beginOffset < 0 ||
            _rect.Width() > width) {
            _beginOffset = 0;
        }
        else if (_beginOffset + _rect.Width() > width) {
            _beginOffset = width - _rect.Width();
        }
    }
    else if (_dragMode == drag_::TIMEBAR) {
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

    BOOL created = CWnd::Create(NULL,
                                _T("g2client play time table"),
                                WS_VISIBLE | WS_CHILD,
                                rect,
                                parent,
                                id);
    if (created) {

    }
    else {
        assert(!"failed to create timetable");
    }
    return (created == TRUE);
}

void time_table_base::set_listenr(listener_time_table* listener)
{
    _listener = listener;
}

void time_table_base::clear(void)
{
    _beginOffset = 0;
    _selectPos = -1;
    reset_data_list();
}

void time_table_base::show(bool show)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    ShowWindow(show ? SW_SHOW : SW_HIDE);
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
    _surfaceSize.SetSize(cx, cy);

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
    _surfaceSize.SetSize(0, 0);
    _surface.DeleteDC();
    _dc.DeleteDC();
    DeleteObject(_surfaceDIB);
}

void time_table_base::prepare_bitmap_info(int cx, int cy, int bpp, BITMAPINFO& bi)
{
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

void time_table_base::draw_backgnd(CDC* pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }

    const int width = _surfaceSize.cx > 0 ? _surfaceSize.cx : __max(default_::TIMETABLE_WIDTH, _rect.Width());
    const int height = _rect.Height();

    pDC->FillSolidRect(0, 0, width, height, RGB(230, 230, 230));
    pDC->FillSolidRect(0, 0, width, default_::TIMETABLE_HEAD_HEIGHT, RGB(210, 210, 210));
    pDC->FillSolidRect(0, default_::TIMETABLE_HEAD_HEIGHT, width, default_::TIMETABLE_TIME_BOUNDARY_HEIGHT, RGB(180, 180, 180));
}

void time_table_base::draw_time(CDC *pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
            return;
    }

    int oldMode = pDC->SetBkMode(TRANSPARENT);
    CFont* oldFont = pDC->SelectObject(&_font);

    if (time_list().size() > 0) {
        CString string;
        CTime prev_date;
        int prev_segment = -1;
        int hour = 0;
        unsigned int index = 0;

        for (int pos = 0; pos < _surfaceSize.cx; pos += 60) {
            if (index < time_list().size()) {
                CTime time = time_list().at(index)._time;
                CTime date = CTime(time.GetYear(), time.GetMonth(), time.GetDay(), 0, 0, 0);
                int segment = time_list().at(index)._segment;

                if (date != prev_date || segment != prev_segment) {
                    prev_date = date;
                    prev_segment = segment;

                    CRect rect(_rectHead);
                    rect.left += pos;
                    rect.right += pos;

                    string = time.Format(_T("%Y-%m-%d"));
                    pDC->DrawText(string, string.GetLength(), rect, DT_SINGLELINE | DT_LEFT | DT_TOP);
                }
            }

            if (pos >= time_list().front()._pos &&
                pos <= time_list().back()._pos) {
                    hour = time_list().at(index)._time.GetHour();
                    index += 60;
            }
            string.Format(L"%02d", hour++);

            pDC->DrawText(string, string.GetLength(), CRect(pos, (default_::TIMETABLE_HEAD_HEIGHT / 2), pos + 12, default_::TIMETABLE_HEAD_HEIGHT), 0);
            pDC->FillSolidRect(pos, default_::TIMETABLE_HEAD_HEIGHT + (default_::TIMETABLE_TIME_BOUNDARY_HEIGHT / 2), 1, (default_::TIMETABLE_TIME_BOUNDARY_HEIGHT / 2), RGB(90, 90, 90));
        }
    }

    pDC->SelectObject(oldFont);
    pDC->SetBkMode(oldMode);  
}

void time_table_base::draw_time_bar(CDC* pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
            return;
    }

    g2::scoped_criticalsection lock(_lock_data);

    int x = 0, cx = 0;
    int y = default_::TIMETABLE_HEAD_HEIGHT + default_::TIMETABLE_TIME_BOUNDARY_HEIGHT;
    int tableW = _surfaceSize.cx;    
    int row = 1;
    COLORREF color[2] = { RGB(170, 170, 170), RGB(200, 200, 200) };

    for (table_data_list::const_iterator itr = data_list().begin();
        itr != data_list().end();
        ++itr, ++row) {
        pDC->FillSolidRect(0, y, tableW, default_::TIMETABLE_ROW_HEIGHT, color[row % 2]);

        const time_data_list& data = itr->_data;
        if (data.size() > 0) {
            unsigned int type = 0;
            unsigned int prevType = data.front()._rec_type;
            x = data.front()._pos;

            for (unsigned int i = 0; i < data.size(); ++i) {
                type = data.at(i)._rec_type;

                if ((type != prevType) || (i + 1 == data.size())) {
                    COLORREF bar_color;
                    if (get_rec_type_color(prevType, bar_color)) {
                        pDC->FillSolidRect(x, y + 5, cx, default_::TIMETABLE_BAR_HEIGHT, bar_color);
                    }
                    prevType = type;
                    x += cx;
                    cx = 1;
                }
                else {
                    ++cx;
                }
            }
        }
        y += default_::TIMETABLE_ROW_HEIGHT;
    }
}

void time_table_base::draw_title(CDC* pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }
}

void time_table_base::draw_select_spot(CDC* pDC)
{
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
    g2::scoped_criticalsection lock(_lock_surface);

    draw_backgnd(&_surface);
    draw_time(&_surface);
    draw_time_bar(&_surface);
    draw_select_spot(&_surface);
    draw_title(&_surface);
}

void time_table_base::present(CDC* pDC)
{
    int width = __max(_surfaceSize.cx, _rect.Width());
    pDC->BitBlt(0, 0, width, _rect.Height(), &_surface, _beginOffset, 0, SRCCOPY);
}

//////////////////////////////////////////////////////////////////////////

int time_table_base::find_pos_by_spot(const G2SPOT& spot)
{
    int index = -1;
    if (g2_time_is_valid(&spot._time) != true) {
        return index;
    }

    CTime time(g2_time_to_time32_t(&spot._time));
    CTime date(time.GetYear(), time.GetMonth(), time.GetDay(), time.GetHour(), 0, 0);

    for (unsigned int i = 0; i < time_list().size(); i += 60) {
        const time_data& data = time_list().at(i);

        if (data._time == date && 
            data._segment == spot._segment) {
            index = data._pos + time.GetMinute();
            break;
        }
    }

    return index;
}

//////////////////////////////////////////////////////////////////////////

void time_table_base::screen_image_drew(const G2SPOT& spot)
{
    set_select_pos(find_pos_by_spot(spot), false);
}

void time_table_base::set_select_pos(int pos, bool request /*= true*/)
{
    if (_selectPos == pos) {
        return;
    }

    int width = _surfaceSize.cx > 0 ? _surfaceSize.cx : default_::TIMETABLE_WIDTH;
    if (pos < 0 ||
        pos > width) {
        return;
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

    if (request) {
        int index = pos - time_list().front()._pos;
        if (index >= 0) {
            const time_data& data = time_list().at(index);

            G2SPOT spot;
            spot._segment = data._segment;
            g2_time_from_time32_t(&spot._time, (unsigned long)data._time.GetTime());

            if (g2_spot_is_valid(&spot)) {
                if (_listener) _listener->on_changed_select_time_table(spot);
            }
        }
    }

    return;
}