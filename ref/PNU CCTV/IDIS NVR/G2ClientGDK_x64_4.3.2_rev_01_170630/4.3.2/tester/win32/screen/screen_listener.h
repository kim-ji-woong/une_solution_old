// screen_listener.h : header file
//

#ifndef _SCREEN_LISTENER_H_
#define _SCREEN_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class screen_listener
{
public:
    screen_listener(void)
    {

    }

    virtual ~screen_listener(void)
    {

    }

protected:
    virtual void on_screen_camera_changed(short camera) = 0;
    virtual void on_screen_layout_changed(int layout, int changed = screen::layout_change_::CHANGE) = 0;
    virtual void on_screen_image_drew(short camera, const G2SPOT& Spot) = 0;

protected:
    virtual void on_screen_lbutton_down(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_lbutton_up(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_lbutton_dblclk(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_rbutton_down(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_rbutton_up(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_mbutton_down(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_mbutton_up(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_mouse_move(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_mouse_wheel(unsigned int flags, short zdelta, const CPoint& point) {}
    virtual bool on_screen_set_cursor(CWnd* pWnd, unsigned int hittest, unsigned int message, int camera, const CRect& rect) { return false; }
    virtual void on_screen_key_down(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags) {}
    virtual void on_screen_key_up(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags) {}
    virtual void on_screen_set_foucs(CWnd* pOldWnd) {}
    virtual void on_screen_resized(unsigned int type, int cx, int cy) {}

public:
    friend class screen_view;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif  // !_SCREEN_LISTENER_H_
