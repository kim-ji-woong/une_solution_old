// layer_osd.cpp : implementation file
//

#include "stdafx.h"
#include "layer_osd.h"

#include <common/client_functional.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

osd_element_base::osd_element_base(ELEMENT_TYPE type)
    : _type(type)
    , _rect(0, 0, 0, 0)
    , _color(RGB(0, 0, 0))
    , _draw(false)
    , _visible(true)
{

}

osd_element_base::~osd_element_base(void)
{

}

//////////////////////////////////////////////////////////////////////////

osd_element_text::osd_element_text(ELEMENT_TYPE type)
    : osd_element_base(type)
    , _outline(false)
{
    _color = RGB(255, 255, 255);
    create_font(_fntSmall, 8);
    create_font(_fntLarge, 9, FW_SEMIBOLD, FIXED_PITCH | FF_DONTCARE);
}

osd_element_text::~osd_element_text(void)
{
    if (_fntSmall.GetSafeHandle()) {
        _fntSmall.DeleteObject();
    }

    if (_fntLarge.GetSafeHandle()) {
        _fntLarge.DeleteObject();
    }
}

void osd_element_text::render(CDC* dc)
{
    assert(dc != NULL);
    if (dc == NULL) return;
    if (_rect.IsRectEmpty()) return;
    if (_string.empty()) return;

    CFont* oldfont = NULL;
    COLORREF oldcolor = dc->SetTextColor(_color);
    int oldmode = dc->SetBkMode(TRANSPARENT);

    CRect dstrect = _rect;
    if (dstrect.Width() > 320 &&
        dstrect.Height() > 280) {
        dstrect.DeflateRect(10, 5);
        oldfont = (CFont*)dc->SelectObject(&_fntLarge);
    }
    else {
        dstrect.DeflateRect(5, 3);
        oldfont = (CFont*)dc->SelectObject(&_fntSmall);
    }

    if (_outline) {
        draw_text_outline(dc, _string, dstrect, _format);
    }
    dc->DrawTextEx(_string.c_str(), &dstrect, _format, NULL);

    dc->SetBkMode(oldmode);
    dc->SetTextColor(oldcolor);
    dc->SelectObject(oldfont);

    revoke();
}

void osd_element_text::draw_text_outline(CDC* dc, const std::wstring& string, const CRect& rect, unsigned int format)
{
    assert(dc != NULL);
    if (dc == NULL) return;

    struct { int x; int y; } elements[] = {
        { -1, -1 },
        {  2,  0 },
        {  0,  2 },
        { -2,  0 }
    };

    CRect dstrect = rect;
    COLORREF oldcolor = dc->SetTextColor(RGB(0, 0, 0));

    for (int i = 0; i < 4; ++i) {
        dstrect.OffsetRect(elements[i].x, elements[i].y);
        dc->DrawTextEx(string.c_str(), dstrect, format, NULL);
    }

    dc->SetTextColor(oldcolor);
}

void osd_element_text::reset(void)
{
    osd_element_base::reset();

    _outline = false;
    memset(&_string, 0, sizeof(_string));
    _format = 0x00;
}

//////////////////////////////////////////////////////////////////////////

osd_element_text_in::osd_element_text_in(ELEMENT_TYPE type)
    : osd_element_base(type)
{
    _color = RGB(255, 255, 255);
    create_font(_font, 9, FW_NORMAL, NONANTIALIASED_QUALITY, FIXED_PITCH | FF_DONTCARE, L"Courier");
}

osd_element_text_in::~osd_element_text_in(void)
{
    if (_font.GetSafeHandle()) {
        _font.DeleteObject();
    }
}

void osd_element_text_in::render(CDC* dc)
{
    if (is_visible() != true ||
        is_draw() != true) { 
        return;
    }

    assert(dc != NULL);
    if (dc == NULL) return;
    if (_elements.empty()) return;

    CRect rect;
    CFont* oldfont = (CFont*)dc->SelectObject(&_font);
    COLORREF oldcolor = dc->SetTextColor(_color);
    int oldmode = dc->SetBkMode(TRANSPARENT);

    for (std::vector<text_in_element_>::const_iterator itr(_elements.begin());
        itr != _elements.end();
        ++itr) {
        const text_in_element_& element = *itr;

        rect = element._rect;
        dc->SetTextColor(element._color);

        if (element._outline) {
            draw_text_outline(dc, element._string, rect, element._format);
        }
        dc->DrawTextEx(element._string.c_str(), rect, element._format, NULL);
    }

    dc->SetBkMode(oldmode);
    dc->SetTextColor(oldcolor);
    dc->SelectObject(oldfont);

    std::vector<text_in_element_>().swap(_elements);
    revoke();
}

void osd_element_text_in::draw_text_outline(CDC* dc, const std::wstring& string, const CRect& rect, unsigned int format)
{
    assert(dc != NULL);
    if (dc == NULL) return;

    struct { int x; int y; } elements[] = {
        { -1, -1 },
        {  2,  0 },
        {  0,  2 },
        { -2,  0 }
    };

    CRect dstrect = rect;
    COLORREF oldcolor = dc->SetTextColor(RGB(0, 0, 0));

    for (int i = 0; i < 4; ++i) {
        dstrect.OffsetRect(elements[i].x, elements[i].y);
        dc->DrawTextEx(string.c_str(), dstrect, format, NULL);
    }

    dc->SetTextColor(oldcolor);
}

void osd_element_text_in::prepare(const CRect& rect) {
    _rect = rect;
    std::vector<text_in_element_>().swap(_elements);
}

void osd_element_text_in::push_string(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline)
{
    text_in_element_ element = { 0 };
    element._string  = string;
    element._format  = format;
    element._outline = outline;
    element._rect    = rect;
    element._color   = color;

    _elements.push_back(element);
}

//////////////////////////////////////////////////////////////////////////

osd_element_border::osd_element_border(void)
    : osd_element_base(OSD_ELEMENT_BORDER)
    , _border(1)
{
    _color = RGB(70, 70, 70);
}

osd_element_border::~osd_element_border(void)
{

}

void osd_element_border::render(CDC* dc)
{
    if (dc == NULL) return;
    if (_rect.IsRectEmpty()) return;

    CRect rect(_rect);
    for (size_t i = 0; i < _border; ++i) {
        dc->Draw3dRect(rect, _color, _color);
        rect.DeflateRect(1, 1);
    }

    revoke();
}

//////////////////////////////////////////////////////////////////////////

osd_element_background::osd_element_background(void)
    : osd_element_text(OSD_ELEMENT_BACKGROUND)
{

}

osd_element_background::~osd_element_background(void)
{

}

void osd_element_background::render(CDC* dc)
{
    assert(dc != NULL);
    if (dc == NULL) return;
    if (_rect.IsRectEmpty()) return;

    CRect dstrect = _rect;
    dc->FillSolidRect(dstrect, _color);

    revoke();
}

//////////////////////////////////////////////////////////////////////////

layer_osd::layer_osd(HWND focusHwnd, CDC* dc)
    : _focusHwnd(focusHwnd)
    , _dc(dc)
    , _bunch(osd_element_base::OSD_ELEMENT_COUNT, NULL)
{

}

layer_osd::~layer_osd(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

bool layer_osd::initialize(void)
{
    return true;
}

bool layer_osd::finalize(void)
{
    for (bunch_type::iterator itr(_bunch.begin());
         itr != _bunch.end();
         ++itr) {
        SAFE_DELETE(*itr);
    }
    return true;
}

void layer_osd::append(osd_element_base* element)
{
    if (element == NULL) return;

    osd_element_base::ELEMENT_TYPE type = element->type();
    assert(_bunch[type] == NULL);
    _bunch[type] = element;
}

void layer_osd::render(void)
{
    if (_dc == NULL ||
        _dc->GetSafeHdc() == NULL) {
        return;
    }

    for (bunch_type::iterator itr(_bunch.begin());
         itr != _bunch.end();
         ++itr) {
        osd_element_base* element = *itr;
        if (element != NULL &&
            element->is_draw() &&
            element->is_visible()) {
            element->render(_dc);
        }
    }
}

void layer_osd::reset(void)
{
    for (bunch_type::iterator itr(_bunch.begin());
         itr != _bunch.end();
         ++itr) {
        if (osd_element_base* element = *itr) {
            element->reset();
        }
    }
}

void layer_osd::prepare_text_in(const CRect& rect)
{
    if (osd_element_text_in* element = static_cast<osd_element_text_in*>(_bunch[osd_element_base::OSD_ELEMENT_TEXT_IN])) {
        element->prepare(rect);
        element->invoke();
    }
}

void layer_osd::push_text_in_text(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline)
{
    if (osd_element_text_in* element = static_cast<osd_element_text_in*>(_bunch[osd_element_base::OSD_ELEMENT_TEXT_IN])) {
        element->push_string(string, rect, color, format, outline);
    }
}

void layer_osd::set_title(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline)
{
    if (osd_element_text* element = static_cast<osd_element_text*>(_bunch[osd_element_base::OSD_ELEMENT_TITLE])) {
        element->set_text(string);
        element->set_rect(rect);
        element->set_color(color);
        element->set_format(format);
        element->set_outline(outline);
        element->invoke();
    }
}

void layer_osd::set_time(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline)
{
    if (osd_element_text* element = static_cast<osd_element_text*>(_bunch[osd_element_base::OSD_ELEMENT_TIME])) {
        element->set_text(string);
        element->set_rect(rect);
        element->set_color(color);
        element->set_format(format);
        element->set_outline(outline);
        element->invoke();
    }
}

void layer_osd::set_message(const std::wstring& string, const CRect& rect, COLORREF color)
{
    if (osd_element_text* element = static_cast<osd_element_text*>(_bunch[osd_element_base::OSD_ELEMENT_MESSAGE])) {
        element->set_text(string);
        element->set_rect(rect);
        element->set_color(color);
        element->set_format(DT_SINGLELINE | DT_VCENTER | DT_CENTER | DT_WORDBREAK);
        element->set_outline(true);
        element->invoke();
    }
}

void layer_osd::set_background(const CRect& rect, COLORREF color)
{
    if (osd_element_background* element = static_cast<osd_element_background*>(_bunch[osd_element_base::OSD_ELEMENT_BACKGROUND])) {
        element->set_rect(rect);
        element->set_color(color);
        element->invoke();
    }
}

void layer_osd::set_border(const CRect& rect, COLORREF color, size_t border)
{
    if (osd_element_border* element = static_cast<osd_element_border*>(_bunch[osd_element_base::OSD_ELEMENT_BORDER])) {
        element->set_rect(rect);
        element->set_color(color);
        element->set_border(border);
        element->invoke();
    }
}
