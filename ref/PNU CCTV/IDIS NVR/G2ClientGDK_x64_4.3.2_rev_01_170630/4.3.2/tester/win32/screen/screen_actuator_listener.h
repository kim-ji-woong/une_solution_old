// ScreenListener.h : header file
//

#ifndef _SCREEN_ACTUATOR_LISTENER_H_
#define _SCREEN_ACTUATOR_LISTENER_H_

#include "screen_common.h"
#include <vector>

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

class screen_actuator_listener
{
public:
    screen_actuator_listener(void)
    {

    }

    virtual ~screen_actuator_listener(void)
    {

    }

protected:
    virtual void on_screen_no_image_loaded(short hostId) = 0;
    virtual void on_screen_camera_changed(short camera) = 0;
    virtual void on_screen_layout_changed(int layout, int changed = client::screen::layout_change_::CHANGE) = 0;
    virtual void on_screen_image_drew(short camera, const G2SPOT& spot) = 0;

    virtual void on_screen_lbutton_down(unsigned int flags, const CPoint& point) = 0;
    virtual void on_screen_lbutton_up(unsigned int flags, const CPoint& point) = 0;
    virtual void on_screen_lbutton_dblclk(unsigned int flags, const CPoint& point) = 0;
    virtual void on_screen_rbutton_down(unsigned int flags, const CPoint& point) = 0;
    virtual void on_screen_rbutton_up(unsigned int flags, const CPoint& point) = 0;
    virtual void on_screen_mbutton_down(unsigned int flags, const CPoint& point) = 0;
    virtual void on_screen_mbutton_up(unsigned int flags, const CPoint& point) = 0;
    virtual void on_screen_mouse_move(unsigned int flags, const CPoint& point) = 0;
    virtual void on_screen_mouse_wheel(unsigned int flags, short zdelta, const CPoint& point) = 0;
    virtual bool on_screen_set_cursor(CWnd* pWnd, unsigned int hittest, unsigned int message, int camera, const CRect& rect) = 0;
    virtual void on_screen_key_down(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags) = 0;
    virtual void on_screen_key_up(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags) = 0;
    virtual void on_screen_set_foucs(CWnd* pOldWnd) = 0;
    virtual void on_screen_resized(unsigned int type, int cx, int cy) = 0;

private:
    friend class screen_actuator;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SCREEN_ACTUATOR_LISTENER_H_
