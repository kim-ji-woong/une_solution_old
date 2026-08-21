// layer_osd.h : haeder file
//

#ifndef _SCREEN_PAINTER_LAYER_OSD_H_
#define _SCREEN_PAINTER_LAYER_OSD_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <vector>
#include <string>

namespace client {

//////////////////////////////////////////////////////////////////////////

class osd_element_base
{
public:
    enum ELEMENT_TYPE {
        OSD_ELEMENT_BACKGROUND = 0,
        OSD_ELEMENT_TITLE,
        OSD_ELEMENT_MESSAGE,
        OSD_ELEMENT_TIME,
        OSD_ELEMENT_BORDER,
        OSD_ELEMENT_TEXT_IN,

        OSD_ELEMENT_COUNT
    };

public:
    osd_element_base(ELEMENT_TYPE type);
    virtual ~osd_element_base(void);

protected:
    ELEMENT_TYPE _type;
    CRect    _rect;
    COLORREF _color;

    bool     _draw;
    bool     _visible;

public:
    virtual void render(CDC* dc) = 0;
    virtual void reset(void) {
        _rect.SetRectEmpty();
        _draw = false;
        _visible = false;
    }

    bool is_visible(void) const { return _visible; }
    bool is_draw(void) const { return _draw; }

    void invoke(void) { _draw = true; }
    void revoke(void) { _draw = false; }

    void set_visible(bool visible) { _visible = visible; }
    void set_color(COLORREF color) { _color = color; }
    void set_rect(const CRect& rect) { _rect = rect; }

    ELEMENT_TYPE type(void) const { return _type; }
};

//////////////////////////////////////////////////////////////////////////

class osd_element_text : public osd_element_base
{
public:
    osd_element_text(ELEMENT_TYPE type);
    virtual ~osd_element_text(void);

protected:
    bool _outline;
    std::wstring _string;
    unsigned int _format;

    CFont _fntSmall;
    CFont _fntLarge;

private:
    void draw_text_outline(CDC* dc, const std::wstring& string, const CRect& rect, unsigned int format);

public:
    virtual void render(CDC* dc);
    virtual void reset(void);

    void set_text(const std::wstring& string) { _string = string; }
    void set_format(unsigned int format) { _format = format; }
    void set_outline(bool outline) { _outline = outline; }
};

class osd_element_text_in : public osd_element_base
{
public:
    osd_element_text_in(ELEMENT_TYPE type);
    virtual ~osd_element_text_in(void);

protected:
    struct text_in_element_ {
        bool _outline;
        std::wstring _string;
        unsigned int _format;
        CRect _rect;
        COLORREF _color;
    };

    std::vector<text_in_element_> _elements;

    CFont _font;

public:
    virtual void render(CDC* dc);

private:
    void draw_text_outline(CDC* dc, const std::wstring& string, const CRect& rect, unsigned int format);

public:
    void prepare(const CRect& rect);
    void push_string(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline);
};

class osd_element_border : public osd_element_base
{
public:
    osd_element_border(void);
    virtual ~osd_element_border(void);

private:
    size_t _border;

public:
    virtual void render(CDC* dc);

    void set_border(size_t border) { _border = border; }
};

class osd_element_background : public osd_element_text
{
public:
    osd_element_background(void);
    virtual ~osd_element_background(void);

public:
    virtual void render(CDC* dc);
};

//////////////////////////////////////////////////////////////////////////

class layer_osd
{
public:
    layer_osd(HWND focusHwnd, CDC* dc);
    ~layer_osd(void);

private:
    typedef std::vector<osd_element_base*> bunch_type;
    bunch_type _bunch;

private:
    CDC* _dc;
    HWND _focusHwnd;

public:
    bool initialize(void);
    bool finalize(void);

    void append(osd_element_base* element);
    void render(void);

    void reset(void);
    void prepare_text_in(const CRect& rect);
    void push_text_in_text(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline);

    void set_title(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline);
    void set_time(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline);
    void set_message(const std::wstring& string, const CRect& rect, COLORREF color);
    void set_background(const CRect& rect, COLORREF color);
    void set_border(const CRect& rect, COLORREF color, size_t border);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SCREEN_PAINTER_LAYER_OSD_H_
