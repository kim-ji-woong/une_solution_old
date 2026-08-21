// painter.cpp : implementation file
//

#include "stdafx.h"
#include "painter.h"

#include <sampler/cpp/g2client_decoder.h>
#include <screen/frame_buffer/frame_buffer.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

painter::painter(HWND focusHwnd, const CSize& sizePort)
    : _focusHwnd(focusHwnd)
    , _sizePort(sizePort)
    , _initialized(false)
    , _backgndDIB(NULL)
    , _mediateDIB(NULL)
    , _backgndBuffer(NULL)
    , _mediateBuffer(NULL)
    , _bufferZoom(NULL)
    , _bufferConv(NULL)
    , _layerOSD(NULL)
{

}

painter::~painter(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

bool painter::initialize(void)
{
    finalize();

    try {
        _painterDC.CreateDC(_T("DISPLAY"), NULL, NULL, NULL);
        if (!_painterDC.GetSafeHdc()) {
            throw false;
        }

        const CSize& sizePort = _sizePort;

        prepare_bitmap_info(sizePort.cx, sizePort.cy, 32, _backgndInfo);
        _backgndDIB = ::CreateDIBSection(_painterDC, &_backgndInfo, DIB_RGB_COLORS, &_backgndBuffer, NULL, 0);

        prepare_bitmap_info(sizePort.cx, sizePort.cy, 32, _mediateInfo);
        _mediateDIB = ::CreateDIBSection(_painterDC, &_mediateInfo, DIB_RGB_COLORS, &_mediateBuffer, NULL, 0);

        if (_surfaceBackgnd.CreateCompatibleDC(&_painterDC)) {
            _surfaceBackgnd.SelectObject(_backgndDIB);
        }

        if (_surfaceMediate.CreateCompatibleDC(&_painterDC)) {
            _surfaceMediate.SelectObject(_mediateDIB);
        }

        /////////////////////////////////////////

        _layerOSD = new layer_osd(_focusHwnd, &_surfaceBackgnd);
        _layerOSD->initialize();

        _layerOSD->append(new osd_element_text(osd_element_base::OSD_ELEMENT_TITLE));
        _layerOSD->append(new osd_element_text(osd_element_base::OSD_ELEMENT_MESSAGE));
        _layerOSD->append(new osd_element_text(osd_element_base::OSD_ELEMENT_TIME));
        _layerOSD->append(new osd_element_text_in(osd_element_base::OSD_ELEMENT_TEXT_IN));
        _layerOSD->append(new osd_element_background());
        _layerOSD->append(new osd_element_border());

        _initialized = true;
        _status = INITIALIZED;
    }
    catch(...) {
        assert(!"failed initialize painter");
    }

    return _initialized;
}

bool painter::finalize(void)
{
    _initialized = false;

    if (_layerOSD) {
        _layerOSD->finalize();
    }
    SAFE_DELETE(_layerOSD);

    _surfaceMediate.DeleteDC();
    _surfaceBackgnd.DeleteDC();
    _painterDC.DeleteDC();

    DeleteObject(_backgndDIB);
    DeleteObject(_mediateDIB);

    _status = FINALIZED;

    return true;
}

CDC* painter::get_dc(void)
{
    return (_surfaceMediate.GetSafeHdc()) ? &_surfaceMediate : NULL;
}

HDC painter::get_hdc(void)
{
    return (_surfaceMediate.GetSafeHdc()) ? _surfaceMediate.GetSafeHdc() : NULL;
}

bool painter::draw_image(const unsigned char* image, int cx, int cy, const CRect& dstRect, const CRect& srcRect)
{
    if (_surfaceMediate.GetSafeHdc() == NULL) return false;

    int  dst_cx = dstRect.Width(), dst_cy = dstRect.Height();
    const unsigned char* processData = NULL;

    if (dst_cx & 0x1) ++dst_cx;
    if (dst_cy & 0x1) ++dst_cy;

    int aligned_dstcx = (((dst_cx + 31) >> 5) << 5);
    int aligned_dstcy = (((dst_cy + 31) >> 5) << 5);
    if (dst_cx < cx || dst_cy < cy) {
        resize_buffer(frame_buffer::calc_len_YV12(aligned_dstcx, aligned_dstcy),
                      frame_buffer::calc_len_RGB(aligned_dstcx, aligned_dstcy, 32));

        CRect rect(srcRect);
        if ((rect.right  % 2) != 0) rect.right++;
        if ((rect.bottom % 2) != 0) rect.bottom++;

        if (_bufferZoom.size() == 0 || _bufferConv.size() == 0) { return false; }

        g2decoder::resize_YV12(&_bufferZoom[0],
                               dst_cx, dst_cy,
                               image,
                               cx, cy,
                               rect.left, rect.top, rect.Width(), rect.Height());

        g2decoder::YV12_to_RGB32(&_bufferConv[0],
                                 aligned_dstcx * 4,
                                 &_bufferZoom[0],
                                 dst_cx, dst_cy);

        processData = &_bufferConv[0];
    }
    else {
        int aligned_cx = (((cx + 31) >> 5) << 5);
        int aligned_cy = (((cy + 31) >> 5) << 5);

        resize_buffer(frame_buffer::calc_len_RGB(aligned_dstcx, aligned_dstcy, 32),
                      frame_buffer::calc_len_RGB(aligned_cx, aligned_cy, 32));

        if (_bufferZoom.size() == 0 || _bufferConv.size() == 0) { return false; }

        g2decoder::YV12_to_RGB32(&_bufferConv[0],
                                 aligned_cx * 4,
                                 image,
                                 cx, cy);

        g2decoder::resize_RGB32(&_bufferZoom[0],
                                aligned_dstcx, dst_cy,
                                0, 0, dst_cx, dst_cy,
                                &_bufferConv[0],
                                aligned_cx, cy,
                                srcRect.left, srcRect.top, srcRect.Width(), srcRect.Height());

        processData = &_bufferZoom[0];
    }

    BITMAPINFO bi;
    prepare_bitmap_info(aligned_dstcx, dst_cy, 32, bi);
    bi.bmiHeader.biHeight = -(bi.bmiHeader.biHeight);
    bool result = (::SetDIBitsToDevice(_surfaceMediate,
                                dstRect.left, dstRect.top, dstRect.Width(), dstRect.Height(),
                                0, 0, 0, dst_cy,
                                processData,
                                &bi,
                                DIB_RGB_COLORS) != GDI_ERROR);

    return result;
}

bool painter::draw_bitmap(HBITMAP* bitmap, const CRect& dstRect, const CRect& srcRect)
{
    if (bitmap == NULL) return false;
    if (dstRect.IsRectEmpty()) return false;

    bool result = false;
    if (CDC* pDC = get_dc()) {
        CDC memDC;
        memDC.CreateCompatibleDC(pDC);
        HBITMAP* oldBmp = (HBITMAP*)memDC.SelectObject(bitmap);
        int  oldBltMode = memDC.SetStretchBltMode(HALFTONE);

        pDC->StretchBlt(dstRect.left, dstRect.top, dstRect.Width(), dstRect.Height(),
                        &memDC, 0, 0, srcRect.Width(), srcRect.Height(),
                        SRCCOPY);

        memDC.SelectObject(oldBmp);
        memDC.DeleteDC();

        result = true;
    }
    return result;
}

bool painter::draw_border(const CRect& rect, COLORREF color, size_t border)
{
    if (_layerOSD == NULL) return false;

    _layerOSD->set_border(rect, color, border);

    return true;
}

bool painter::draw_title(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline /*= true*/)
{
    if (_layerOSD == NULL) return false;

    _layerOSD->set_title(string, rect, color, format, outline);

    return true;
}

bool painter::draw_time(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline /*= true*/)
{
    if (_layerOSD == NULL) return false;

    _layerOSD->set_time(string, rect, color, format, outline);

    return true;
}

bool painter::draw_message(const std::wstring& string, const CRect& rect, COLORREF color)
{
    if (_layerOSD == NULL) return false;

    _layerOSD->set_message(string, rect, color);

    return true;
}

bool painter::draw_background(const CRect& rect, COLORREF color)
{
    if (_layerOSD == NULL) return false;

    _layerOSD->set_background(rect, color);

    return true;
}

bool painter::fill_rect(const CRect& rect, COLORREF color)
{
    if (_surfaceMediate.GetSafeHdc() == NULL) return false;
    if (rect.IsRectEmpty()) return true;

    _surfaceMediate.FillSolidRect(rect, color);

    return true;
}

bool painter::clear_osd(const CRect& rect, COLORREF color)
{
    return fill_rect(rect, color);
}

bool painter::render(const CRect& rect)
{
    if (_surfaceBackgnd.GetSafeHdc() == NULL) return false;
    if (_surfaceMediate.GetSafeHdc() == NULL) return false;

    BOOL succeeded = _surfaceBackgnd.BitBlt(rect.left, rect.top,
                                            rect.Width(), rect.Height(),
                                            &_surfaceMediate,
                                            rect.left, rect.top,
                                            SRCCOPY);

    if (succeeded) {
        if (_layerOSD) {
            _layerOSD->render();
        }
    }

    return (succeeded == TRUE);
}

bool painter::present(void)
{
    if (_focusHwnd == NULL ||
        ::IsWindow(_focusHwnd) != TRUE) {
        return false;
    }

    CRect rect;
    ::GetClientRect(_focusHwnd, &rect);

    return present(rect);
}

bool painter::present(const CRect& rect)
{
    if (_surfaceBackgnd.GetSafeHdc() == NULL) return false;

    BOOL succeeded = FALSE;
    if (HDC hDC = ::GetDC(_focusHwnd)) {
        succeeded = ::BitBlt(hDC, rect.left, rect.top,
                             rect.Width(), rect.Height(),
                             _surfaceBackgnd,
                             rect.left, rect.top,
                             SRCCOPY);
        ::ReleaseDC(_focusHwnd, hDC);
    }
    return (succeeded == TRUE);
}

bool painter::capture(CBitmap& bitmap, const CRect& rctDst, const CRect& rctSrc)
{
    return true;
}

void painter::prepare_text_in(const CRect& rect)
{
    if (_layerOSD != NULL) _layerOSD->prepare_text_in(rect);
}

bool painter::push_text_in_text(const std::wstring& string, const CRect& rect, COLORREF color, unsigned int format, bool outline /*= false*/)
{
    if (_layerOSD == NULL) return false;

    _layerOSD->push_text_in_text(string, rect, color, format, outline);

    return true;
}

//////////////////////////////////////////////////////////////////////////

void painter::prepare_bitmap_info(int cx, int cy, int bpp, BITMAPINFO& bi)
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

void painter::resize_buffer(size_t sizeZoom, size_t sizeConv)
{
    g2::scoped_criticalsection lock(_lock_buffer);

    if (_bufferZoom.size() < sizeZoom) {
        _bufferZoom.resize(sizeZoom);
    }

    if (_bufferConv.size() < sizeConv) {
        _bufferConv.resize(sizeConv);
    }
}
