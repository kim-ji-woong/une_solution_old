// client_functional.h : header file
//

#ifndef _COMMON_CLIENT_FUNCTIONAL_H_
#define _COMMON_CLIENT_FUNCTIONAL_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define.h>
#include <include/g2_define_play.h>
#include <algorithm>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

inline LOGFONT* create_log_font(LOGFONT& font,
    int size = 9,
    int weight = FW_NORMAL,
    unsigned char quality = CLEARTYPE_NATURAL_QUALITY,
    unsigned char pitch_and_family = DEFAULT_PITCH | FF_DONTCARE,
    const wchar_t* face = L"MS Shell Dlg 2")
{
    memset(&font, 0x00, sizeof(font));
    font.lfHeight         = -::MulDiv(size, 96, 72);
    font.lfWidth          = 0;
    font.lfEscapement     = 0;
    font.lfOrientation    = 0;
    font.lfWeight         = weight;
    font.lfItalic         = FALSE;
    font.lfUnderline      = FALSE;
    font.lfStrikeOut      = FALSE;
    font.lfCharSet        = DEFAULT_CHARSET;
    font.lfOutPrecision   = OUT_DEFAULT_PRECIS;
    font.lfClipPrecision  = CLIP_DEFAULT_PRECIS;
    font.lfQuality        = quality;
    font.lfPitchAndFamily = pitch_and_family;
    wcscpy_s(font.lfFaceName, face);

    return &font;
}

inline bool create_font(CFont& font,
                        int size = 9,
                        int weight = FW_NORMAL,
                        unsigned char quality = CLEARTYPE_NATURAL_QUALITY,
                        unsigned char pitch_and_family = DEFAULT_PITCH | FF_DONTCARE,
                        const wchar_t* face = L"MS Shell Dlg 2")
{
    if (font.GetSafeHandle()) {
        font.DeleteObject();
    }

    LOGFONT lf = { 0 };
    return (font.CreateFontIndirect(create_log_font(lf, size, weight, quality, pitch_and_family, face)) == TRUE);
}

inline HMONITOR get_monitor_handle(CPoint pos)
{
    return ::MonitorFromPoint(pos, MONITOR_DEFAULTTONEAREST);
}

inline bool get_monitor_info_ex(HMONITOR hmonitor, MONITORINFOEX& info)
{
    memset(&info, 0x00, sizeof(info));
    info.cbSize = sizeof(info);

    return (hmonitor) ? (::GetMonitorInfo(hmonitor, &info) == TRUE) : false;
}

inline bool get_monitor_info_ex(CPoint pos, MONITORINFOEX& info)
{
    HMONITOR hmonitor = client::get_monitor_handle(pos);
    memset(&info, 0x00, sizeof(info));
    info.cbSize = sizeof(info);

    return (hmonitor) ? (::GetMonitorInfo(hmonitor, &info) == TRUE) : false;
}

inline CRect confine_to_dest_rect(CRect src, const CRect& dst)
{
    if (src.IsRectEmpty() ||
        dst.IsRectEmpty() ||
        src == dst) {
            // nothing
    }
    else {
        if (src.left < dst.left) {
            src.OffsetRect(dst.left - src.left, 0);
        }
        if (dst.right < src.right) {
            src.OffsetRect(dst.right - src.right, 0);
        }
        if (src.top < dst.top) {
            src.OffsetRect(0, dst.top - src.top);
        }
        if (dst.bottom < src.bottom) {
            src.OffsetRect(0, dst.bottom - src.bottom);
        }
    }
    return src;
}

template <typename T> inline
T abs_diff(const T& a, const T& b)
{
    return (a < b) ? (b - a) : (a - b);
}

inline bool is_same_date(const CTime& left, const CTime& right)
{
    return (left.GetYear() == right.GetYear() &&
            left.GetMonth() == right.GetMonth() &&
            left.GetDay() == right.GetDay());
}

inline bool SAFE_CLOSE_HANDLE(HANDLE& handle)
{
    bool ret = handle && ::CloseHandle(handle);
    if (ret) { handle = NULL; }
    return ret;
}

inline bool SAFE_DELETE_GDIOBJ(CGdiObject& object)
{
    return (object.GetSafeHandle()) ?
           (object.DeleteObject() == TRUE) : false;
}

inline bool SAFE_DELETE_GDIOBJ(CGdiObject* object)
{
    return (object && object->GetSafeHandle()) ?
           (object->DeleteObject() == TRUE) : false;
}

inline bool SAFE_WINDOW_DESTROY(CWnd& object)
{
    return (object.GetSafeHwnd()) ?
           (object.DestroyWindow() == TRUE) : false;
}

inline bool SAFE_WINDOW_DESTROY(CWnd* object)
{
    return (object && object->GetSafeHwnd()) ?
           (object->DestroyWindow() == TRUE) : false;
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_COMMON_CLIENT_FUNCTIONAL_H_
