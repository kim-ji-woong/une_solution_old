// screen_formatter.h : header file
//

#ifndef _SCREEN_FORMATTER_H_
#define _SCREEN_FORMATTER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <common/client_define.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class screen_formatter
{
public:
    screen_formatter(void);
    explicit screen_formatter(int countcamera);
    ~screen_formatter(void);

public:
    struct element_t {
        element_t(void)
            : _rect(0, 0, 0, 0)
            , _visible(false) {
        }

        CRect _rect;
        bool  _visible;

        void set_visible(bool visible) { _visible = visible; }
        void set_rect(const CRect& rect) { _rect  = rect; }
        bool is_visible(void) const { return _visible; }
        const CRect& rect(void) const { return _rect; }
    };

    struct TemplateRect {
        CPoint _region;
    };

    typedef std::vector<element_t> container_t;

protected:
    container_t _container;
    int _countcamera;
    int _layout;
    int _layoutMax;

public:
    const container_t& container(void) const { return _container;   }
    int current_layout(void) const { return _layout; }
    bool is_layout_1x1(void)  const { return _layout == screen_formatter_::LAYOUT_1x1; }

public:
    void create(int countcamera);
    void cleanup(void);

    int secure_max_layout(int layout) const;
    int secure_layout_for_camera_count(int countcamera) const;
    int secure_layout_for_camera_range(short start, short end) const;
    int reckon_layout(int layout, short cameraLT, const CRect& rect);

    static int secure_layout_for_camera_count(int countcamera, int layoutMax);
    static int count_per_layout(int layout);
    static int rows(int layout);
    static int cols(int layout);
    static bool is_regular(int layout);
    static bool valid_layout(int layout);

protected:
    int make_screen_layout(int layout, short cameraLT, const CRect arrRect[], const TemplateRect arrTemple[]);
};

//////////////////////////////////////////////////////////////////////////

inline int screen_formatter::secure_max_layout(int layout) const
{
    if (layout > screen_formatter_::LAYOUT_6x6) {
        return layout;
    }
    return (_countcamera < rows(layout) * cols(layout)) ?
            secure_max_layout(layout - 1) : layout;
}

inline int screen_formatter::count_per_layout(int layout)
{
    int camera = 0;
    switch (layout) {
        case screen_formatter_::LAYOUT_1x1  : camera = 1;  break;
        case screen_formatter_::LAYOUT_2x2  : camera = 4;  break;
        case screen_formatter_::LAYOUT_3x3  : camera = 9;  break;
        case screen_formatter_::LAYOUT_4x4  : camera = 16; break;
        case screen_formatter_::LAYOUT_5x5  : camera = 25; break;
        case screen_formatter_::LAYOUT_6x6  : camera = 36; break;
        default:
            assert(false);
            break;
    }
    return camera;
}

inline int screen_formatter::rows(int layout)
{
    int row = 0;
    switch (layout) {
        case screen_formatter_::LAYOUT_1x1  : row = 1; break;
        case screen_formatter_::LAYOUT_2x2  : row = 2; break;
        case screen_formatter_::LAYOUT_3x3  : row = 3; break;
        case screen_formatter_::LAYOUT_4x4  : row = 4; break;
        case screen_formatter_::LAYOUT_5x5  : row = 5; break;
        case screen_formatter_::LAYOUT_6x6  : row = 6; break;
        default:
            assert(false);
            break;
    }
    return row;
}

inline int screen_formatter::cols(int layout)
{
    int col = 0;
    switch (layout) {
        case screen_formatter_::LAYOUT_1x1  : col = 1; break;
        case screen_formatter_::LAYOUT_2x2  : col = 2; break;
        case screen_formatter_::LAYOUT_3x3  : col = 3; break;
        case screen_formatter_::LAYOUT_4x4  : col = 4; break;
        case screen_formatter_::LAYOUT_5x5  : col = 5; break;
        case screen_formatter_::LAYOUT_6x6  : col = 6; break;
        default:
            assert(false);
            break;
    }
    return col;
}

inline bool screen_formatter::is_regular(int layout)
{
    bool regular = false;
    switch (layout) {
        case screen_formatter_::LAYOUT_1x1 :
        case screen_formatter_::LAYOUT_2x2 :
        case screen_formatter_::LAYOUT_3x3 :
        case screen_formatter_::LAYOUT_4x4 :
        case screen_formatter_::LAYOUT_5x5 :
        case screen_formatter_::LAYOUT_6x6 :
            regular = true;
            break;
    }
    return regular;
}

inline bool screen_formatter::valid_layout(int layout)
{
    return (layout >= screen_formatter_::LAYOUT_1x1 &&
            layout < screen_formatter_::LAYOUT_COUNT);
}

//////////////////////////////////////////////////////////////////////////

namespace screen {
    typedef screen_formatter formatter;
}

} // !_namespace_client

#endif // !_SCREEN_FORMATTER_H_
