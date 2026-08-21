// camera_view.cpp : implementation file
//

#include "stdafx.h"
#include "camera_view.h"
#include "screen_view.h"
#include "painter/painter.h"
#include "screen_common.h"
#include "screen/text_in/text_in_queue.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

camera_view::camera_view(void)
    : _screen(NULL)
    , _modePTZ(G2LIVE_CAMERA_STATUS::PTZ_NONE)
{
    g2_spot_make_invalid(&_spot);
    g2_time_make_invalid(&_time);
}

camera_view::camera_view(short index, screen_view* screen)
    : _screen(screen)
{
    initialize(index, screen);
}

camera_view::~camera_view(void)
{

}

//////////////////////////////////////////////////////////////////////////

void camera_view::initialize(short index, screen_view* screen)
{
    if (screen == NULL) return;

    _screen     = screen;
    _index      = index;
    _selected   = (_index == 0) ? true : false; // select camera initially
    _stub       = false;
    _visible    = false;
    _needDraw   = true;
    _useDualAudio    = false;
    _usePlayAudio    = false;
    _useSoundCapture = false;
    _status     = screen::camera_status_::DISABLE;
    _modePTZ    = G2LIVE_CAMERA_STATUS::PTZ_NONE;
    _modeCamera = screen::camera_::UNDEFINED;
    _ratioFact  = screen::img_ratio_::CONSTANT;

    g2_spot_make_invalid(&_spot);
    g2_time_make_invalid(&_time);

    _resImage.SetSize(320, 240);
    _resImageCrop.SetSize(320, 240);

    set_rect(CRect(0, 0, 320, 240));
}

void camera_view::reset(bool mode /*= true*/, bool title /*= true*/)
{
    _status = screen::camera_status_::DISABLE;
    _modePTZ = G2LIVE_CAMERA_STATUS::PTZ_NONE;
    _useDualAudio    = false;
    _usePlayAudio    = false;
    _useSoundCapture = false;
    _timeUpdate      = true;

    g2_spot_make_invalid(&_spot);
    g2_time_make_invalid(&_time);

    if (mode) {
        _modeCamera = screen::camera_::UNDEFINED;
    }
    if (title) {
        _title.clear();
        _title_ext.clear();
    }

    g2_time_make_invalid(&_time);
}

//////////////////////////////////////////////////////////////////////////

void camera_view::set_selected(bool selected)
{
    if (_selected != selected) {
        _selected = selected;
        _needDraw = true;
    }
}

void camera_view::set_rect(const CRect& rect)
{
    _rect = rect;
    _size = rect.Size();
}

void camera_view::set_status(short status)
{
    _status = status;
}

void camera_view::draw_image(painter* painter,
                             const unsigned char* image,
                             int cx, int cy)
{
    G2RETURN_IF_FAIL(painter != NULL);
    if (image == NULL || (cx <= 0 || cy <= 0)) return;

    CRect srcRect(0, 0, cx, cy);
    CRect dstRect = _rect;

    zoom_ratio(dstRect, dstRect, cx, cy, _ratioFact);

    fill_rect(painter);

    painter->draw_image(image, cx, cy, dstRect, srcRect);
}

bool camera_view::draw_status(painter* painter)
{
    if (painter == NULL) return false;

    short status = _status;
    bool  result = false;

    // TODO: draw no-video image code here

    return result;
}

void camera_view::draw_border(painter* painter)
{
    G2RETURN_IF_FAIL(painter != NULL);

    COLORREF color = (_screen->is_layout_1x1()) ? RGB(80, 80, 80) :
                     (is_selected()) ? RGB(0, 255, 0) : RGB(100, 100, 100);

    painter->draw_border(_rect, color, is_selected() ? 2 : 1);
}

void camera_view::draw_osd(painter* painter)
{
    G2RETURN_IF_FAIL(painter != NULL);

    if (is_stub()) return;

    std::wstring string;

    if (_status == screen::camera_status_::RECONNECTING) {
        string = L"Reconnecting ...";
        painter->draw_message(string, _rect, RGB(255, 255, 255));
    }
    else if (_status == screen::camera_status_::NO_VIDEO) {
        string = L"No video";
        painter->draw_message(string, _rect, RGB(255, 255, 255));
        painter->draw_background(_rect, RGB(0, 0, 255));
    }
    else if (_status == screen::camera_status_::ENABLE) {
        // ...
    }
    else {
        switch (_status) {
            case screen::camera_status_::DISABLE:
            case screen::camera_status_::ENABLE:
                // nothing
                break;
            case screen::camera_status_::INACTIVATE:
                string = L"Not use";
                break;
            case screen::camera_status_::COVERT_L1:
                string = L"Covert";
                break;
            case screen::camera_status_::NOT_CONNECTED:
                string = L"Not connected";
                break;
            default:
                assert(!"undefined camera status type");
                break;
        }
        if (string.empty() != true) {
            painter->draw_message(string, _rect, RGB(255, 255, 255));
        }
    }

    string = (_title_ext.empty()) ?_title : _title_ext;

    if (is_selected() &&
        string.empty()) {
        wchar_t buf[4] = { 0 };
        _itow_s(index() + 1, buf, 10);
        string = buf;
    }

    painter->draw_title(string, _rect, RGB(255, 255, 255), DT_SINGLELINE | DT_TOP);

    /////////////////////////////////////////////

    if (_status == screen::camera_status_::ENABLE &&
        g2_time_is_valid(&_time)) {
        CTime time(g2_time_to_time32_t(&_time));
        painter->draw_time(time.Format(_T("%Y-%m-%d %H:%M:%S")).operator LPCTSTR(),
                           _rect,
                           RGB(255, 255, 255),
                           DT_SINGLELINE | DT_BOTTOM | DT_CENTER);
    }
}

void camera_view::draw_text_in_live(painter* painter, text_in_queue* queue)
{
    G2RETURN_IF_FAIL(painter != NULL);
    G2RETURN_IF_FAIL(queue != NULL);

    if (is_mode_none()) return;

    const int TEXT_IN_LINE_HEIGHT      = 20;
    const int TEXT_IN_START_X          = 0;
    const int TEXT_IN_END_X            = 512;
    const int TEXT_IN_ID_END_X         = TEXT_IN_START_X + 25;
    const int TEXT_IN_START_Y          = 0;
    const int TEXT_IN_END_Y            = 480;

    g2::scoped_sectionlock_<text_in_queue> section(*queue);

    CRect rcdest, rect = _rect;
    rect.DeflateRect(10, 10, 10, 20);
    constant_ratio(rcdest, rect, TEXT_IN_END_X, TEXT_IN_END_Y);
    if (rcdest.bottom < rect.bottom) {
        rcdest.OffsetRect(0, rect.bottom - rcdest.bottom);
    }

    int count = queue->size();
    int height = 0;
    int y = rcdest.bottom - 20;
    text_in_queue::element_sp element;

    painter->prepare_text_in(rcdest);

    bool begin = false;
    for (int i = 0; i < count; ++i) {
        element = queue->pop_back();
        if (element.get() == NULL) continue;
        if (y <= TEXT_IN_START_Y ||
            begin) {
            continue;
        }
        queue->push_front(element);

        if (element->_from != G2TEXT_IN_ELEMENT::FROM_LIVE) continue;

        if (element->_type == G2TEXT_IN_ELEMENT::TYPE_END &&
            element->_data._len == 0) {
            continue;
        }

        painter->push_text_in_text(element->_data._string, 
                                   CRect(rcdest.left + 3, y - TEXT_IN_LINE_HEIGHT, rcdest.right - 3, y), 
                                   RGB(255, 255, 255), 
                                   DT_LEFT | DT_SINGLELINE | DT_VCENTER, true);
        y -= TEXT_IN_LINE_HEIGHT;

        if (element->_type == G2TEXT_IN_ELEMENT::TYPE_START) {
            begin = true;
            y -= 5;
        }
    }
}

void camera_view::draw_text_in_play(painter* painter, text_in_queue* queue, const G2SPOT& spot)
{
    G2RETURN_IF_FAIL(painter != NULL);
    G2RETURN_IF_FAIL(queue != NULL);

    if (is_mode_none()) return;

    const int TEXT_IN_LINE_HEIGHT      = 20;
    const int TEXT_IN_START_X          = 0;
    const int TEXT_IN_END_X            = 512;
    const int TEXT_IN_ID_END_X         = TEXT_IN_START_X + 25;
    const int TEXT_IN_START_Y          = 0;
    const int TEXT_IN_END_Y            = 480;

    g2::scoped_sectionlock_<text_in_queue> section(*queue);

    POSITION from, to;
    queue->find_text_in_range(from, to, spot);

    if (from == NULL ||
        to == NULL) return;

    CRect rcdest, rect = _rect;
    rect.DeflateRect(10, 10, 10, 20);
    constant_ratio(rcdest, rect, TEXT_IN_END_X, TEXT_IN_END_Y);
    if (rcdest.bottom < rect.bottom) {
        rcdest.OffsetRect(0, rect.bottom - rcdest.bottom);
    }

    int count = queue->size();
    int height = 0;
    int y = rcdest.bottom - 20;
    text_in_queue::element_sp element;

    painter->prepare_text_in(rcdest);

    for ( ; from != to && to != NULL; ) {
        if (element = queue->at_prev(to)) {
            if (y <= TEXT_IN_START_Y) continue;
            if (g2_time_compare(&_time, &element->_spot._time) < 0) continue;

            if (element->_type == G2TEXT_IN_ELEMENT::TYPE_END) {
                y -= TEXT_IN_LINE_HEIGHT;
            }

            if (element->_type == G2TEXT_IN_ELEMENT::TYPE_END &&
                element->_data._len == 0) {
                continue;
            }

            painter->push_text_in_text(element->_data._string, 
                                       CRect(rcdest.left + 3, y - TEXT_IN_LINE_HEIGHT, rcdest.right - 3, y), 
                                       RGB(255, 255, 255), 
                                       DT_LEFT | DT_SINGLELINE | DT_VCENTER, true);

            y -= TEXT_IN_LINE_HEIGHT;
            
            if (element->_type == G2TEXT_IN_ELEMENT::TYPE_START) {
                y -= 5;
            }
        }
    }
}

void camera_view::fill_rect(painter* painter)
{
    G2RETURN_IF_FAIL(painter != NULL);

    COLORREF color = is_stub() ? RGB(20, 20, 20) : RGB(0, 0, 0);
    painter->fill_rect(_rect, color);

    draw_status(painter);
}

void camera_view::clear_osd(painter* painter)
{
    G2RETURN_IF_FAIL(painter != NULL);

    COLORREF color = is_no_video() ? RGB(48, 48, 48) : RGB(70, 70, 70);

    painter->clear_osd(_rect, color);
}

void camera_view::render(painter* painter)
{
    G2RETURN_IF_FAIL(painter != NULL);

    painter->render(_rect);
}

void camera_view::present(painter* painter)
{
    G2RETURN_IF_FAIL(painter != NULL);

    painter->present(_rect);
}

//////////////////////////////////////////////////////////////////////////

bool camera_view::is_draw(bool reset /*= true*/)
{
    bool need = _needDraw;
    if (reset) _needDraw = false;
    return need;
}

//////////////////////////////////////////////////////////////////////////

int camera_view::center_horz(int left, int right)
{
    return ((left + right) >> 1);
}

int camera_view::center_vert(int top, int bottom)
{
    return ((top + bottom) >> 1);
}

int camera_view::center_horz(int left, int right, int cx)
{
    return center_horz(left, right) - (cx >> 1);
}

int camera_view::center_vert(int top, int bottom, int cy)
{
    return center_vert(top, bottom) - (cy >> 1);
}

const RECT& camera_view::center_rect(RECT& result, const RECT& dest, int cx, int cy)
{
    result.left = center_horz(dest.left, dest.right, cx);
    result.top  = center_vert(dest.top, dest.bottom, cy);
    result.right = result.left + cx;
    result.bottom = result.top + cy;
    return result;
}

void camera_view::constant_ratio(RECT& out, const RECT& in, int cx, int cy)
{
    int w = in.right - in.left;
    int h = in.bottom - in.top;
    int cx_t = w, cy_t = h;

    float ratio_x = (float)cx / (float)w;
    float ratio_y = (float)cy / (float)h;

    cy_t = (int)((float)cy / ratio_x + 0.4F);

    if (cy_t > h) {
        cx_t = (int)((float)cx / ratio_y + 0.4F);
        cy_t = h;
    }
    else {
        cx_t = (cx >= w) ? w : (int)((float)cx / ratio_x + 0.4F);
    }

    center_rect(out, in, cx_t, cy_t);
}

void camera_view::zoom_ratio(CRect& out, const CRect& in, int cx, int cy, screen::img_ratio_::FACTOR factor)
{
    if (factor == screen::img_ratio_::NORMAL) {
        return;
    }
    if (factor == screen::img_ratio_::CONSTANT) {
        constant_ratio(out, in, cx, cy);
        return;
    }

    int imagew = 0, imageh = 0;

    switch (factor) {
        case screen::img_ratio_::HALF:
            imagew = cx >> 1;
            imageh = cy >> 1;
            break;
        case screen::img_ratio_::X1:
            imagew = cx;
            imageh = cy;
            break;
        case screen::img_ratio_::X2:
            imagew = cx << 1;
            imageh = cy << 1;
            break;
        case screen::img_ratio_::X4:
            imagew = cx << 2;
            imageh = cy << 2;
            break;
        default:
            imagew = cx;
            imageh = cy;
            break;
    }

    if (in.Width()  >= imagew &&
        in.Height() >= imageh) {
        center_rect(out, in, imagew, imageh);
    }
    else {
        constant_ratio(out, in, cx, cy);
    }
}
