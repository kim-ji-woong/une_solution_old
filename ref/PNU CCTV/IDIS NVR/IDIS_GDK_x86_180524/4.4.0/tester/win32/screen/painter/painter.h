// painter.h : header file
//

#ifndef _SCREEN_PAINTER_H_
#define _SCREEN_PAINTER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "layer_osd.h"
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class painter {
public:
    painter(HWND focusHwnd, const CSize& sizePort);
    ~painter(void);

public:
    enum STATUS_TYPE {
        UNDEFINED = -1,
        INITIALIZED = 0,
        FINALIZED,
        DEVICE_LOST
    };

protected:
    bool _initialized;

    int _status;

    HWND _focusHwnd;

    CSize _sizePort;

    CDC _painterDC;
    CDC _surfaceBackgnd;
    CDC _surfaceMediate;

    HBITMAP _backgndDIB;
    HBITMAP _mediateDIB;

    BITMAPINFO _backgndInfo;
    BITMAPINFO _mediateInfo;

    void* _backgndBuffer;
    void* _mediateBuffer;

    typedef std::vector<unsigned char> buffer_type;
    buffer_type _bufferZoom;
    buffer_type _bufferConv;

    layer_osd* _layerOSD;

private:
    g2::critical_section _lock_buffer;

private:
    void prepare_bitmap_info(int cx, int cy, int bpp, BITMAPINFO& bi);
    void resize_buffer(size_t sizeZoom, size_t sizeConv);

public:
    bool initialize(void);
    bool finalize(void);

    CDC* get_dc(void);
    HDC get_hdc(void);

    bool draw_image(const unsigned char* image, int cx, int cy, const CRect& dstRect, const CRect& srcRect);
    bool draw_bitmap(HBITMAP* bitmap, const CRect& dstRect, const CRect& srcRect);
    bool draw_border(const CRect& rect, COLORREF color, size_t border);
    bool draw_title(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline = true);
    bool draw_time(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline = true);
    bool draw_message(const std::wstring& string, const CRect& rect, COLORREF color);
    bool draw_background(const CRect& rect, COLORREF color);
    bool fill_rect(const CRect& rect, COLORREF color);
    bool clear_osd(const CRect& rect, COLORREF color);
    bool render(const CRect& rect);
    bool present(void);
    bool present(const CRect& rect);
    bool capture(CBitmap& bitmap, const CRect& rctDst, const CRect& rctSrc);

    void prepare_text_in(const CRect& rect);
    bool push_text_in_text(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline = false);

public:
    CSize size_port(void) const { return _sizePort; }

protected:
    friend class camera_view;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SCREEN_PAINTER_H_
