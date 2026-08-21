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


inline connect_type_::TYPE connect_type_from_connect_mode(int mode)
{
    if (mode == client::connect_mode_::UNDEFINED) {
        return client::connect_type_::UNDEFINED;
    }
    connect_type_::TYPE connection = client::connect_type_::UNDEFINED;
    switch (mode) {
        case client::connect_mode_::WATCH:
        case client::connect_mode_::SEARCH:
        case client::connect_mode_::SEARCH_G2:
            connection = client::connect_type_::RAS;
            break;
        default:
            assert(!"undefined connection mode");
            break;
    }
    return connection;
}

inline G2NETWORK_INFO::PORT_TYPE port_type_from_connect_mode(int mode)
{
    G2NETWORK_INFO::PORT_TYPE port = G2NETWORK_INFO::MIN_PORT_INDEX;
    switch (mode) {
        case client::connect_mode_::WATCH:
            port = G2NETWORK_INFO::WATCH_PORT;
            break;
        case client::connect_mode_::SEARCH:
        case client::connect_mode_::SEARCH_G2:
            port = G2NETWORK_INFO::SEARCH_PORT;
            break;
        default:
            assert(!"not supported connection mode");
            break;
    }
    return port;
}

inline int revise_speed(int svrspeed, int cltspeed)
{
    int correct = cltspeed;

    switch (svrspeed) {
        case G2PLAYER::NONE:
            correct = search::speed_::PLAY_STOP;
            break;
        case G2PLAYER::BACK_FASTEST:
            switch (cltspeed) {
        case search::speed_::BACK_FASTEST:
        case search::speed_::BACK_FASTEST_MORE:
            break;
        default:
            correct = search::speed_::BACK_FASTEST;
            break;
            }
            break;
        case G2PLAYER::BACK_FASTER:
            switch (cltspeed) {
        case search::speed_::BACK_FASTER:
        case search::speed_::BACK_FASTER_MORE:
            break;
        default:
            correct = search::speed_::BACK_FASTER;
            break;
            }
            break;
        case G2PLAYER::BACK_FAST:
            switch (cltspeed) {
        case search::speed_::BACK_FAST:
        case search::speed_::BACK_FAST_MORE:
        case search::speed_::BACK_FASTEST_INFINITE:
            break;
        default:
            correct = search::speed_::BACK_FAST;
            break;
            }
            break;
        case G2PLAYER::BACK_NORMAL:
            switch (cltspeed) {
        case search::speed_::BACK_NORMAL_INFINITE:
        case search::speed_::BACK_NORMAL_TRIPLE:
        case search::speed_::BACK_NORMAL_TWICE:
        case search::speed_::BACK_NORMAL_HFAST:
        case search::speed_::BACK_NORMAL:
            break;
        default:
            correct = search::speed_::BACK_NORMAL;
            break;
            }
            break;
        case G2PLAYER::BACK_SLOW:
            switch (cltspeed) {
        case search::speed_::BACK_SLOW:
            break;
        default:
            correct = search::speed_::BACK_SLOW;
            break;
            }
            break;
        case G2PLAYER::PLAY_SLOW:
            switch (cltspeed) {
        case search::speed_::PLAY_SLOW:
            break;
        default:
            correct = search::speed_::PLAY_SLOW;
            break;
            }
            break;
        case G2PLAYER::PLAY_NORMAL:
            switch (cltspeed) {
        case search::speed_::PLAY_NORMAL:
        case search::speed_::PLAY_NORMAL_HFAST:
        case search::speed_::PLAY_NORMAL_TWICE:
        case search::speed_::PLAY_NORMAL_TRIPLE:
        case search::speed_::PLAY_NORMAL_INFINITE:
            break;
        default:
            correct = search::speed_::PLAY_NORMAL;
            break;
            }
            break;
        case G2PLAYER::PLAY_FAST:
            switch (cltspeed) {
        case search::speed_::PLAY_FAST:
        case search::speed_::PLAY_FAST_MORE:
        case search::speed_::PLAY_FASTEST_INFINITE:
            break;
        default:
            correct = search::speed_::PLAY_FAST;
            break;
            }
            break;
        case G2PLAYER::PLAY_FASTER:
            switch (cltspeed) {
        case search::speed_::PLAY_FASTER:
        case search::speed_::PLAY_FASTER_MORE:
            break;
        default:
            correct = search::speed_::PLAY_FASTER;
            break;
            }
            break;
        case G2PLAYER::PLAY_FASTEST:
            switch (cltspeed) {
        case search::speed_::PLAY_FASTEST:
        case search::speed_::PLAY_FASTEST_MORE:
            break;
        default:
            correct = search::speed_::PLAY_FASTEST;
            break;
            }
            break;
    }

    return correct;
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

inline const std::vector<CRect>& rect_division(std::vector<CRect>& container, const CRect& rect, int row, int col, const CRect& deflate = CRect(0, 0, 0, 0))
{
    int x = rect.left, y = rect.top;
    int w = rect.right - rect.left;
    int h = rect.bottom - rect.top;

    int cellw = w / col, cellh = h / row;
    int padcx = w % col, padcy = h % row;
    int i = 0, j;

    // random permutation ///////////////////////

    std::vector<int> cxpermute(col);
    std::vector<int> cypermute(row);

    for (i = 0; i < col; ++i) cxpermute[i] = i;
    for (i = 0; i < row; ++i) cypermute[i] = i;

    std::random_shuffle(cxpermute.begin(), cxpermute.end());
    std::random_shuffle(cypermute.begin(), cypermute.end());

    // padding margin value /////////////////////

    std::vector<int> cxcell(col);
    std::vector<int> cycell(row);

    container.resize(row* col);

    for (i = 0; i < col; ++i) cxcell[cxpermute[i]] = cellw + ((--padcx >= 0) ? 1 : 0);
    for (i = 0; i < row; ++i) cycell[cypermute[i]] = cellh + ((--padcy >= 0) ? 1 : 0);
    for (i = 0; i < row; ++i) {
        for (j = 0; j < col; ++j) {
            if (j != 0) x += cxcell[col - 1];
            RECT& cell  = container[(i * col) + j];
            cell.left   = x + deflate.left;
            cell.top    = y + deflate.top;
            cell.right  = x + cxcell[j] - deflate.right;
            cell.bottom = y + cycell[i] - deflate.bottom;
        }

        x = rect.left;
        y = y + cycell[i];
    }

    return container;
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_COMMON_CLIENT_FUNCTIONAL_H_
