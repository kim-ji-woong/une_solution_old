// screen_formatter.cpp : implementation file
//

#include "stdafx.h"
#include "screen_formatter.h"
#include "screen_common.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

screen_formatter::screen_formatter(void)
    : _countcamera(0)
    , _layoutMax(screen_formatter_::LAYOUT_6x6)
{

}

screen_formatter::screen_formatter(int countcamera)
{
    create(countcamera);
}

screen_formatter::~screen_formatter(void)
{

}

//////////////////////////////////////////////////////////////////////////

void screen_formatter::create(int countcamera)
{
    assert(countcamera > 0);

    cleanup();

    _countcamera = countcamera;
    _layoutMax   = secure_layout_for_camera_count(countcamera);
    _container.resize(countcamera);
}

void screen_formatter::cleanup(void)
{
    _container.clear();
}

//////////////////////////////////////////////////////////////////////////

int screen_formatter::secure_layout_for_camera_count(int countcamera) const
{
    countcamera = (countcamera < 1) ? 1 :
                  (countcamera > _countcamera) ? _countcamera : countcamera;

    return secure_layout_for_camera_count(countcamera, _layoutMax);
}

int screen_formatter::secure_layout_for_camera_range(short start, short end) const
{
    int layout = _layoutMax, count = 0;
    for (int i = 0; i < screen_formatter_::LAYOUT_COUNT; ++i) {
        count = count_per_layout(i);
        if ((start / count) == (end / count)) {
            layout = i;
            break;
        }
    }
    return (layout = (layout > _layoutMax) ? _layoutMax : layout);
}

int screen_formatter::reckon_layout(int layout, short cameraLT, const CRect& rect)
{
    G2RETURN_VAL_IF_FAIL(valid_layout(layout), screen_formatter_::LAYOUT_1x1);

    _layout = secure_max_layout(layout);

    int ROW = rows(_layout);
    int COL = cols(_layout);
    const int W = rect.Width()  / COL;
    const int H = rect.Height() / ROW;
    const int PAGE = ROW * COL;

    int row, col, right, bottom, index;

    CRect rectBuffer[screen::count_::CAMERA];

    for (row = 0; row < ROW; ++row) {
        for (col = 0; col < COL; ++col) {
            right  = (col < COL - 1) ? rect.left + (col + 1) * W : rect.right;
            bottom = (row < ROW - 1) ? rect.top  + (row + 1) * H : rect.bottom;
            index  = row * COL + col;
            rectBuffer[index].SetRect(rect.left + col * W, rect.top + row * H, right, bottom);
        }

        short startcamera = (_countcamera == count_per_layout(_layout)) ? 0 : cameraLT;

        const TemplateRect* templatePtr = NULL;

        if (is_regular(_layout)) {
            int cameracount = ROW * COL;

            index = 0;

            for (int i = startcamera; index < _countcamera; ++i) {
                if (i >= _countcamera) {
                    i = 0;
                }
                if (index >= cameracount) {
                    _container[i].set_visible(false);
                    index++;
                    continue;
                }
                element_t& result = _container[i];
                result.set_visible(true);
                result.set_rect(rectBuffer[index]);
                index++;
            }
        }
    }
    return _layout;
}

//////////////////////////////////////////////////////////////////////////

int screen_formatter::secure_layout_for_camera_count(int countcamera, int layoutMax)
{
    int layout = layoutMax;
    for (int i = 0; i < screen_formatter_::LAYOUT_COUNT; ++i) {
        if (countcamera <= count_per_layout(i)) {
            layout = i;
            break;
        }
    }
    return (layout = (layout > layoutMax) ? layoutMax : layout);
}

int screen_formatter::make_screen_layout(int layout, short cameraLT, const CRect arrRect[], const TemplateRect arrTemple[])
{
    assert(arrRect && arrTemple);

    const int cameracount = count_per_layout(layout);
    int index = 0;
    CRect rect;

    for (int i = cameraLT; index < _countcamera; ++i) {
        if (i >= _countcamera) {
            i = 0;
        }
        if (index >= cameracount) {
            _container[i].set_visible(false);
            index++;
            continue;
        }

        const TemplateRect& element = arrTemple[index];
        const CPoint& region = element._region;

        rect.left   = arrRect[region.x].left;
        rect.top    = arrRect[region.x].top;
        rect.right  = arrRect[region.y].right;
        rect.bottom = arrRect[region.y].bottom;

        element_t& result = _container[i];
        result.set_visible(true);
        result.set_rect(rect);
        index++;
    }

    return layout;
}
