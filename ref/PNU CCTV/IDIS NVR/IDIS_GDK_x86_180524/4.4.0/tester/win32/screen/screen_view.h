// screen_view.h : header file
//

#ifndef _CLIENT_SCREEN_VIEW_H_
#define _CLIENT_SCREEN_VIEW_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "screen_drop_target.h"
#include "screen_formatter.h"
#include "screen_common.h"

#include <include/g2_define_live.h>

namespace client {
    class screen_listener;
    class painter;
    class camera_view;
    class text_in_queue;

//////////////////////////////////////////////////////////////////////////

class screen_view : public CWnd
                  , protected screen::drop_event_base
{
    DECLARE_DYNAMIC(screen_view)

public:
    screen_view(void);
    virtual ~screen_view(void);

protected:
    struct command_ {
        enum ID {
            UNDEFINED = -1,
            SET_CAMERA = 0,
            SET_LAYOUT,
            PREV_LAYOUT,
            NEXT_LAYOUT,
            DBL_CLICK,
            UPDATE_RECT,
            UPDATE_ALL,
            UPDATE_BY_ONE,
            UPDATE_SCREEN,
            SET_RATIO_FACT,
            SET_CAMERA_TITLE,
            SET_CAMERA_TITLE_EXT,
            SET_CAMERA_STATUS,
            DUMMY
        };
    };

protected:
    CWnd* _parent;
    camera_view* _cameraView;
    client::painter* _painter;
    screen_listener* _listener;

    screen::layout_base_::MODE  _modeBaseLayout;
    screen_formatter _fmtScreen;
    CRect _rctScreen;

    int _currLayout;
    int _prevLayout;
    int _currCameraLT;
    int _prevCameraLT;
    int _status;
    int _totalcamera;

    short _selcamera;
    short _selcameraPrev;
    short _countcamera;

    bool _activate;
    bool _lockUpdate;

    struct range {
        int _begin;
        int _end;

        range(void)
            : _begin(0)
            , _end(0) {
        }
        range(int begin, int end)
            : _begin(begin)
            , _end(end) {
        }
    }
    _rangeStub;

    CWnd* _dropParent;
    screen::screen_drop_target* _dropTarget;
    unsigned int _invokeDropMessage;
    unsigned int _secureDropMessage;

    screen::img_ratio_::FACTOR _factImageRatio;

    g2::critical_section _lock_exec;

public:
    bool create(CWnd* parent,
                const CRect& rect,
                unsigned int id,
                short countcamera,
                screen_formatter_::LAYOUT layout = screen_formatter_::LAYOUT_6x6);

    bool cleanup(void);
    void set_listener(screen_listener* listener);
    void set_drag_drop(CWnd* dropParent, unsigned int invokeMessageId, unsigned int secureMessageId);

    HWND safe_hwnd(void) const { return GetSafeHwnd(); }
    CWnd* parent_wnd(void) const { return _parent; }

    bool enable_window(bool enable = true);
    bool show(int command = SW_SHOWNORMAL);
    void set_focus(void);
    void set_focus_post(void);
    CSize screen_size(void) const { return _rctScreen.Size(); }

    bool set_window_pos(const CWnd* insertafter, const CRect& rect, unsigned int flags);
    void move_window(const CRect& rect, bool repaint = true);

    LRESULT send_message(unsigned int message, WPARAM wParam = 0, LPARAM lParam = 0L);
    bool post_message(unsigned int message, WPARAM wParam = 0, LPARAM lParam = 0L);

public:
    void display_frame(const unsigned char* image,
                       short cx, short cy,
                       short scrcamera,
                       const G2FRAME& frame);

    short find_camera_by_point(const CPoint& point) const;

    void reset_screen(bool mode, bool title, bool update);
    void reset_camera(short camera, bool mode, bool title, bool update);
    void reset_camera(unsigned __int64 cameras, bool mode, bool title, bool update);

    void fire_connected(short camera);
    void fire_disconnected(short camera);
    void fire_camera_changed(short camera, bool post = true);

    void set_activate(bool activate = true);
    void set_layout_base(int base);
    void set_layout(int layout);
    void set_camera_layout(short camera, int layout);
    void set_camera_count_layout(short camera, int countcamera);
    void set_layout_page_prev(void);
    void set_layout_page_next(void);
    void set_camera(short camera, bool redraw = true);
    void set_camera_title(short camera, const std::wstring& title, bool update = true);
    void set_camera_title(unsigned __int64& cameras, const std::wstring& title, bool update = true);
    void set_camera_title_ext(short camera, const std::wstring& title, bool update = true);
    void set_camera_title_ext(unsigned __int64& cameras, const std::wstring& title, bool update = true);
    void set_camera_status(short camera, short status, bool update = true);
    void set_camera_status(unsigned __int64 cameras, short status, bool update = true);
    void set_use_camera_ptz(short camera, char use = G2LIVE_CAMERA_STATUS::PTZ_NONE);
    void set_camera_mode(short camera, screen::camera_::MODE cameraMode);
    void set_camera_mode(unsigned __int64 cameras, screen::camera_::MODE cameraMode);
    void set_camera_visible(short camera, bool visible = true);
    void set_camera_use_dual_audio(short camera, bool use, bool update = true);
    void set_camera_use_dual_audio(unsigned __int64& cameras, bool use, bool update = true);
    void set_camera_use_sound_capturing(short camera, bool use, bool update = true);
    void set_camera_use_sound_capturing(unsigned __int64& cameras, bool use, bool update = true);

    const std::wstring camera_title(short camera) const;
    const std::wstring camera_title_ext(short camera) const;

    bool is_activate(void) const { return _activate; }
    bool is_camera_mode_none(short camera) const;
    bool is_camra_mode(short camera, int mode) const;
    bool is_camera_status(short camera, screen::camera_status_::TYPE status) const;
    char is_use_camera_ptz(short camera) const;
    bool is_camera_visible(short camera) const;
    bool is_camera_enable(short camera) const;
    bool is_camera_no_video(short camera) const;
    bool is_camera_contains(short camera) const;
    bool is_camera_stub(short camera) const;
    bool is_layout_base_page(void) const { return _modeBaseLayout == screen::layout_base_::PAGE; }
    bool is_layout_base(int base) const { return _modeBaseLayout == base; }
    bool is_layout(int layout) const { return _currLayout == layout; }
    bool is_layout_1x1(void) const { return _currLayout == screen_formatter_::LAYOUT_1x1; }

    short camera_mode(short camera) const;
    short camera_status(short camera) const;
    bool  camera_rect(short camera, CRect& rect) const;
    CRect camera_rect(short camera) const;
    CSize camera_image_size(short camera) const;

    int left_top_camera(void) const { return _currCameraLT; }
    int prev_layout(void) const { return _prevLayout; }
    int curr_layout(void) const { return _currLayout; }
    short selcamera_prev(void) const { return _selcameraPrev; }
    short selcamera(void) const { return _selcamera; }

    int count_camera_at_layout(void) const { return _fmtScreen.count_per_layout(_currLayout); }
    int count_camera_running(void) const;
    int count_camera_mode(int mode) const;

    camera_view& camera_view_ref(short camera) const;

    G2SPOT camera_spot(short camera) const;

    unsigned __int64 visible_camera(void) const;
    unsigned __int64 visible_camera_mode(int mode) const;
    unsigned __int64 visible_camera_live(void) const;
    unsigned __int64 visible_camera_play(void) const;
    unsigned __int64 running_camera(void) const;

    void erase_image(unsigned __int64& scrcameras);

    void update_camera(int camera, bool fill = true);
    void update_camera(unsigned __int64 cameras, bool fill = true);
    void update_camera_all(void);
    void update_screen(void);
    void present(unsigned int elpase = 0);

    void set_camera_ratio(short camera, screen::img_ratio_::FACTOR ratiofact);
    void set_camera_ratio(screen::img_ratio_::FACTOR ratiofact);

    screen::img_ratio_::FACTOR camera_ratio(short camera) const;

    int last_display_spot(unsigned __int64& cameras, G2SPOT& spot);
    G2SPOT last_display_spot(short camera);


protected:
    struct {
        text_in_queue* _queue;
        G2SPOT _spotLast;
    }
    _textIn;

public:
    void put_text_in(short hostId, short camera, const G2TEXT_IN_ELEMENT& element, bool tohead = false);
    void delete_text_in(const unsigned __int64& cameras);
    void display_text_in(short camera, const G2SPOT& spot, bool rollback = false);

protected:
    LRESULT execute_command(command_::ID command, WPARAM wParam = 0, LPARAM lParam = 0L);

    LRESULT __stdcall exe_set_camera(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_set_layout(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_prev_layout(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_next_layout(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_dbl_click(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_update_rect(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_update_all(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_update_camera_by_one(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_update_screen(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_set_camera_title(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_set_camera_title_ext(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_set_camera_status(WPARAM wParam, LPARAM lParam, BOOL& proceed);
    LRESULT __stdcall exe_set_ratio_factor(WPARAM wParam, LPARAM lParam, BOOL& proceed);

private:
    void register_drag_drop(void);
    void unregister_drag_drop(void);

private:
    bool create_painter(const CSize& sizePort);

    void set_camera_exe(short camera);
    void set_layout_exe(int layout);
    void update_screen_rect(int layout);
    void update_screen_rect(const CRect& rect);
    void update_camera_osd(void);
    void update_camera_by_one(int camera, bool fill = true);

    void reckon_camera_layout(screen::layout_reckon_::MODE mode, int layout, short selcamera, short& rpreselect, short& rcamera);

private:
    void on_mouse_button_down_impl(UINT nFlags, const CPoint& point);

protected:
    virtual void on_screen_lbutton_down(unsigned int flags, const CPoint& point, bool* dragUsed = NULL) {}
    virtual void on_screen_lbutton_up(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_lbutton_dblclk(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_rbutton_down(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_rbutton_up(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_mbutton_down(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_mbutton_up(unsigned int flags, const CPoint& point) {}
    virtual void on_screen_mouse_move(unsigned int flags, const CPoint& point, bool* dragUsed = NULL) {}
    virtual void on_screen_mouse_wheel(unsigned int flags, short zdelta, const CPoint& point) {}
    virtual bool on_screen_set_cursor(CWnd* pWnd, unsigned int hittest, unsigned int message, int camera, const CRect& rect) { return false; }
    virtual void on_screen_key_down(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags) {}
    virtual void on_screen_key_up(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags) {}
    virtual void on_screen_set_foucs(CWnd* pOldWnd) {}
    virtual void on_screen_resized(unsigned int type, int cx, int cy) {}

protected:
    virtual void DoDataExchange(CDataExchange* pDX);

    virtual bool OnDrop(FORMATETC* format,
                        STGMEDIUM* medium,
                        DWORD* effect,
                        POINTL point,
                        LONG_PTR userData);

    virtual bool OnDragEnter(FORMATETC* format,
                        STGMEDIUM* medium,
                        DWORD* effect,
                        POINTL point,
                        LONG_PTR userData);

    virtual bool OnDragOver(const POINTL& point);
    virtual bool OnDragLeave(void);

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
    afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
    afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
    afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
    afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
    afx_msg void OnMButtonDown(UINT nFlags, CPoint point);
    afx_msg void OnMButtonUp(UINT nFlags, CPoint point);
    afx_msg void OnMouseMove(UINT nFlags, CPoint point);
    afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
    afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg void OnMove(int x, int y);
    afx_msg void OnMoving(UINT nSide, LPRECT lpRect);
    afx_msg void OnSize(UINT nType, int cx, int cy);
    afx_msg void OnSizing(UINT nSide, LPRECT lpRect);
    afx_msg void OnShowWindow(BOOL bShow, UINT nStatus);
    afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
    afx_msg void OnKeyUp(UINT nChar, UINT nRepCnt, UINT nFlags);
    afx_msg void OnSetFocus(CWnd* pOldWnd);
    afx_msg void OnWindowPosChanged(WINDOWPOS* lpwndpos);

    /////////////////////////////////////////////

    afx_msg LRESULT on_post_set_focus(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_fire_camera_changed(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CLIENT_SCREEN_VIEW_H_
