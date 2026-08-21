// time_table.cpp : implementation file
//

#include "stdafx.h"
#include "time_table.h"

#include <common/client_functional.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

enum CONST_DATA {
    TIMETABLE_WIDTH = (60 * 24),
    TIMETABLE_HEAD_HEIGHT = 30,
    TIMETABLE_CAMERA_TITLE_WIDTH = 150,
    TIMETABLE_TIME_BOUNDARY_HEIGHT = 10,
    TIMETABLE_BAR_HEIGHT = 10,
    TIMETABLE_ROW_HEIGHT = TIMETABLE_BAR_HEIGHT * 2,
};

//////////////////////////////////////////////////////////////////////////

time_table::time_table(void)
    : _parent(NULL)
    , _listener(NULL)
    , _beginOffset(0)
    , _selectPos(-1)
    , _initialized(false)
    , _enable(false)
{

}

time_table::~time_table(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

IMPLEMENT_DYNAMIC(time_table, CWnd)

void time_table::DoDataExchange(CDataExchange* pDX)
{
    CWnd::DoDataExchange(pDX);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(time_table, CWnd)
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

int time_table::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CWnd::OnCreate(lpCreateStruct) == -1) {
        return -1;
    }

    initialize();

    return 0;
}

void time_table::OnDestroy()
{
    CWnd::OnDestroy();
}

void time_table::OnSize(UINT nType, int cx, int cy)
{
    CWnd::OnSize(nType, cx, cy);

    CRect rect;
    GetClientRect(&rect);

    if (_rect != rect) {
        _rect = rect;
        _rectHead = _rect;
        _rectHead.bottom = TIMETABLE_HEAD_HEIGHT + TIMETABLE_TIME_BOUNDARY_HEIGHT;

        _rectBody = _rect;
        _rectBody.top = _rectHead.bottom;

        initialize_surface(TIMETABLE_WIDTH, _rect.Height());
    }
}

BOOL time_table::OnEraseBkgnd(CDC* pDC)
{
    render();
    present(pDC);

    return TRUE;
}

void time_table::OnLButtonDown(UINT nFlags, CPoint point)
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
        set_select_spot(point.x + _beginOffset);
    }
    else {
        return;
    }

    SetCapture();
    Invalidate();
}

void time_table::OnLButtonUp(UINT nFlags, CPoint point)
{
    CWnd::OnLButtonUp(nFlags, point);

    if (_enable != true) {
        return;
    }

    if (_dragMode == drag_::TIMEBAR) {
        set_select_spot(point.x + _beginOffset);
    }

    if (GetCapture() == this) {
        ReleaseCapture();
    }
    _dragMode = drag_::OFF;
}

void time_table::OnMouseMove(UINT nFlags, CPoint point)
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

        if (_beginOffset < 0) {
            _beginOffset = 0;
        }
        else if (_beginOffset + _rect.Width() > TIMETABLE_WIDTH) {
            _beginOffset = TIMETABLE_WIDTH - _rect.Width();
        }
    }
    else if (_dragMode == drag_::TIMEBAR) {
        set_select_spot(point.x + _beginOffset);
    }

    if ((nFlags & MK_LBUTTON) == MK_LBUTTON &&
        (_dragMode == drag_::DATETIME || _dragMode == drag_::TIMEBAR)) {
        Invalidate();
    }
}

BOOL time_table::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
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

void time_table::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
    CWnd::OnVScroll(nSBCode, nPos, pScrollBar);
}

//////////////////////////////////////////////////////////////////////////

bool time_table::create(CWnd* parent, const CRect& rect, unsigned int id)
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

void time_table::set_listenr(listener_time_table* listener)
{
    _listener = listener;
}

void time_table::update(const G2SPOT& spot, const std::vector<time_table::time_table_data>& list)
{
    g2::scoped_criticalsection lock(_lock_data);

    std::vector<time_table::time_table_data>().swap(_rtiList);
    _rtiList.insert(_rtiList.begin(), list.begin(), list.end());
    const int count = (int)_rtiList.size();

    _spot = spot;

    if (count == 0) {
        _selectPos = -1;
        g2_spot_make_invalid(&_spot);
    }
    else {
        _enable = true;
    }

    lock.free();

    int current = _selectPos;
    if (current + _rect.Width() > TIMETABLE_WIDTH) {
        current = TIMETABLE_WIDTH - _rect.Width();
    }

    current -= _rect.Width() / 2;
    if (current < 0) {
        current = 0;
    }
    _beginOffset = current;

    int height = (count * TIMETABLE_ROW_HEIGHT) + TIMETABLE_HEAD_HEIGHT + TIMETABLE_TIME_BOUNDARY_HEIGHT;
    height = __max(_rect.Height(), height);

    initialize_surface(TIMETABLE_WIDTH, height);
    Invalidate();
}

void time_table::screen_image_drew(const G2TIME& time)
{
    if (g2_time_is_valid(&time) != true) {
        return;
    }

    CTime drewTime((time_t)time._time);

    const int drewpos = (drewTime.GetHour() * 60) + drewTime.GetMinute();
    set_select_spot(drewpos, false);

    Invalidate();
}

void time_table::clear(void)
{
    _beginOffset = 0;
    _selectPos = -1;
}

//////////////////////////////////////////////////////////////////////////

void time_table::initialize(void)
{
    CWinApp* app = AfxGetApp();
    _cursor[cursor_::FINGER] = app->LoadCursor(IDC_CURSOR_FINGER);
    _cursor[cursor_::HAND]   = app->LoadCursor(IDC_CURSOR_HAND);
    _cursor[cursor_::GRIP]   = app->LoadCursor(IDC_CURSOR_GRIP);

    client::create_font(_font, 8);

    CRect rect;
    GetClientRect(&rect);

    initialize_surface(TIMETABLE_WIDTH, rect.Height());

    _initialized = true;
}

void time_table::finalize(void)
{
    finalize_surface();

    _initialized = false;
}

void time_table::initialize_surface(int cx, int cy)
{
    g2::scoped_criticalsection lock(_lock_surface);

    finalize_surface();

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

void time_table::finalize_surface(void)
{
    _surface.DeleteDC();
    _dc.DeleteDC();
    DeleteObject(_surfaceDIB);
}

void time_table::draw_backgnd(CDC* pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }

    const int width = __max((TIMETABLE_CAMERA_TITLE_WIDTH + TIMETABLE_WIDTH), _rect.Width());
    const int height = _rect.Height();

    pDC->FillSolidRect(0, 0, width, height, RGB(230, 230, 230));
    pDC->FillSolidRect(0, 0, width, TIMETABLE_HEAD_HEIGHT, RGB(210, 210, 210));
    pDC->FillSolidRect(0, TIMETABLE_HEAD_HEIGHT, width, TIMETABLE_TIME_BOUNDARY_HEIGHT, RGB(180, 180, 180));
}

void time_table::draw_time(CDC *pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }

    int oldMode = pDC->SetBkMode(TRANSPARENT);
    CFont* oldFont = pDC->SelectObject(&_font);

    int pos = 0;
    CString string;

    for (int i = 0; i < 25; ++i) {
        pos = (60 * i);
        string.Format(L"%02d", i);
        pDC->DrawText(string, string.GetLength(), CRect(pos - 6, (TIMETABLE_HEAD_HEIGHT / 2), pos + 6, TIMETABLE_HEAD_HEIGHT), 0);
        pDC->FillSolidRect(pos, TIMETABLE_HEAD_HEIGHT + (TIMETABLE_TIME_BOUNDARY_HEIGHT / 2), 1, (TIMETABLE_TIME_BOUNDARY_HEIGHT / 2), RGB(90, 90, 90));
    }

    if (g2_spot_is_valid(&_spot)) {
        string = CTime((time_t)_spot._time._time).Format(_T("%Y-%m-%d")).operator LPCTSTR();
        pDC->DrawText(string, string.GetLength(), _rectHead, DT_SINGLELINE | DT_CENTER | DT_TOP);
    }

    pDC->SelectObject(oldFont);
    pDC->SetBkMode(oldMode);
}

void time_table::draw_time_bar(CDC* pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }

    g2::scoped_criticalsection lock(_lock_data);

    int i, x = 0, y = 0, cx = 0;
    unsigned int type = 0;
    unsigned int prevType = 0;

    x = 0;
    y = TIMETABLE_HEAD_HEIGHT + TIMETABLE_TIME_BOUNDARY_HEIGHT + (TIMETABLE_BAR_HEIGHT / 2);

    for (std::vector<time_table::time_table_data>::const_iterator itr(_rtiList.begin());
         itr != _rtiList.end();
         ++itr) {
        const time_table::time_table_data& data = *itr;

        x = 0;
        cx = 0;
        type = 0;
        prevType = 0;

        for (i = 0; i < 1440; ++i) {
            type = data._data[i];

            if (type != prevType &&
                cx > 0) {
                pDC->FillSolidRect(x, y, cx, TIMETABLE_BAR_HEIGHT, color_rec_time_info(prevType));

                x += cx;
                cx = 0;
            }

            if (type > 0) {
                ++cx;
            }
            else {
                ++x;
            }
            prevType = type;
        }

        if (cx > 0) {
            pDC->FillSolidRect(x, y, cx, TIMETABLE_BAR_HEIGHT, color_rec_time_info(prevType));

            x += cx;
            cx = 0;
        }

        y += TIMETABLE_ROW_HEIGHT;
    }
}

void time_table::draw_title(CDC* pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }
}

void time_table::draw_select_spot(CDC* pDC)
{
    if (pDC == NULL ||
        pDC->GetSafeHdc() == NULL) {
        return;
    }

    if (_selectPos != -1) {
        pDC->FillSolidRect(_selectPos, _rectBody.top, 1, _rectBody.Height(), RGB(255, 0, 0));
    }
}

void time_table::render(void)
{
    g2::scoped_criticalsection lock(_lock_surface);

    draw_backgnd(&_surface);
    draw_time(&_surface);
    draw_time_bar(&_surface);
    draw_select_spot(&_surface);
    draw_title(&_surface);
}

void time_table::present(CDC* pDC)
{
    pDC->BitBlt(-_beginOffset, 0, TIMETABLE_WIDTH, _rect.Height(), &_surface, 0, 0, SRCCOPY);
}

COLORREF time_table::color_rec_time_info(int type)
{
    COLORREF color;
    if (type & G2FRAME::PANIC) {
        color = RGB(41, 144, 229);
    }
    else if (type & G2FRAME::PRE_EVENT) {
        color = RGB(91, 41, 229);
    }
    else if (type & G2FRAME::EVENT) {
        color = RGB(203, 41, 229);
    }
    else if (type & G2FRAME::TIME_LAPSE) {
        color = RGB(50, 67, 89);
    }
    else if (type & G2FRAME::IRREGULAR) {
        color = RGB(222, 107, 4);
    }

    return color;
}

void time_table::prepare_bitmap_info(int cx, int cy, int bpp, BITMAPINFO& bi)
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

void time_table::set_select_spot(int pos, bool request /*= true*/)
{
    if (_selectPos == pos) {
        return;
    }

    if (pos < 0 ||
        pos > TIMETABLE_WIDTH) {
        return;
    }

    if (_selectPos == -1) {
        _beginOffset = pos - _rect.Width() / 2;

        if (_beginOffset < 0) {
            _beginOffset = 0;
        }
        else if (_beginOffset + _rect.Width() > TIMETABLE_WIDTH) {
            _beginOffset = TIMETABLE_WIDTH - _rect.Width();
        }
    }
    _selectPos = pos;

    int hour = pos / 60;
    int minute = pos % 60;

    G2SPOT spot = _spot;
    if (g2_spot_is_valid(&spot) &&
        request == true) {
        CTime date((time_t)spot._time._time);
        spot._time._time = (unsigned long)CTime(date.GetYear(), date.GetMonth(), date.GetDay(), hour, minute, 0).GetTime();

        if (request == true) {
            if (_listener) _listener->on_changed_select_time_table(spot);
        }
    }
}
