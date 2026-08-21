// screen_actuator.h : header file
//

#ifndef _SCREEN_ACTUATOR_H_
#define _SCREEN_ACTUATOR_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "screen_view.h"
#include "screen_listener.h"
#include "screen_actuator_listener.h"
#include "frame_buffer/frame_buffer.h"
#include "frame_buffer_manager.h"

#include <utility/g2looper.h>
#include <sampler/cpp/g2client_decoder.h>
#include <boost/unordered_set.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class screen_actuator : public screen_listener
{
public:
    screen_actuator(void);
    ~screen_actuator(void);

protected:
    screen_view _screen;
    g2decoder _decoder;

    int _countcamera;
    int _countchannel;
    int _speedPlay;

    bool _cleanup;

    volatile bool _ignoreFrame;
    volatile bool _ignoreDisplay;
    volatile unsigned int _refsleep;

    G2SPOT _spotVideo;
    boost::unordered_set<short> _ignoreChannels;

    frame_element _videoFrame;
    frame_buffer_manager _bufferManager;

    typedef std::vector<unsigned char> buffer_type;
    buffer_type _bufferImage;
    buffer_type _bufferExtra;

    typedef g2::looper_<screen_actuator> looper_type;
    looper_type _looper;

    screen_actuator_listener* _listener;

    g2::critical_section _lock_decoder;

protected:
    void initialize(void);
    bool is_cleanup(void) const { _cleanup; }

public:
    bool create(CWnd* parent, const CRect& rect, unsigned int id,
                int countcamera = 36,
                int countchannel = 16,
                int layout = screen_formatter_::LAYOUT_6x6);

    bool cleanup(void);

    void set_listener(screen_actuator_listener* listener);
    void set_priority(int priority = THREAD_PRIORITY_NORMAL);
    void set_drag_drop(CWnd* dropParent, unsigned int invokeMessageId, unsigned int secureMessageId);
    void set_activate_screen(bool activate) { _screen.set_activate(activate); }
    void set_camera_mode(short camera, screen::camera_::MODE cameraMode);
    void set_camera_mode(unsigned __int64 cameras, screen::camera_::MODE cameraMode);
    void set_camera_status(short camera, screen::camera_status_::TYPE status, bool update = true);
    void set_camera_status(unsigned __int64 cameras, screen::camera_status_::TYPE status, bool update = true);
    void set_use_camera_ptz(short camera, char use = G2LIVE_CAMERA_STATUS::PTZ_NONE);
    void set_camera_visible(short camera, bool visible = true);
    void set_camera_use_dual_audio(unsigned __int64& cameras, bool use, bool update = true);
    void set_camera_use_sound_capturing(unsigned __int64& cameras, bool use, bool update = true);
    void set_camera_ratio(short camera, screen::img_ratio_::FACTOR factor);
    void set_camera_ratio(screen::img_ratio_::FACTOR factor);
    void set_camera(short camera) { _screen.set_camera(camera); }
    void set_camera_title(short camera, const std::wstring& title, bool update = true);
    void set_camera_title(unsigned __int64& cameras, const std::wstring& title, bool update = true);
    void set_camera_title_ext(short camera, const std::wstring& title, bool update = true);
    void set_camera_title_ext(unsigned __int64& cameras, const std::wstring& title, bool update = true);

    short selected_camera(void) const { return _screen.selcamera(); }

    int get_camera_mode(short camera) const { return _screen.camera_mode(camera); }
    int get_camera_status(short camera) const { return _screen.camera_status(camera); }
    G2SPOT get_camera_spot(short camera) const { return _screen.camera_spot(camera); }
    std::wstring get_camera_title(short camera) const { return _screen.camera_title(camera); }
    int get_last_displayed_spot(unsigned __int64& cameras, G2SPOT& spot);

    int count_camera(void) const { return _countcamera; }
    int count_camera_at_layout(void) const { return _screen.count_camera_at_layout(); }
    int count_camera_running(void) const { return _screen.count_camera_running(); }
    int count_camera_mode(int mode) const { return _screen.count_camera_mode(mode); }
    screen::img_ratio_::FACTOR camera_ratio(short camera) const { return _screen.camera_ratio(camera); }

    bool is_camera_status(short camera, screen::camera_status_::TYPE status) const;
    char is_use_camera_ptz(short camera)  const;
    bool is_camera_mode_none(short camera) const { return _screen.is_camera_mode_none(camera); }
    bool is_camera_visible(short camera) const { return _screen.is_camera_visible(camera); }
    bool is_camera_no_video(short camera) const { return _screen.is_camera_no_video(camera); }
    bool is_camera_contains(short camera) const { return _screen.is_camera_contains(camera); }
    bool is_activate_screen(void) const { return _screen.is_activate(); }

    void reset_screen(bool mode, bool title = true, bool update = true);
    void reset_camera(short camera, bool mode, bool title = true, bool update = true);
    void reset_camera(unsigned __int64 cameras, bool mode, bool title = true, bool update = true);
    void update_camera(short camera, bool fill = true) { _screen.update_camera(camera, fill); }
    void update_camera(unsigned __int64 cameras, bool fill = true) { _screen.update_camera(cameras, fill); }
    void update_screen(void) { _screen.update_screen(); }
    void present(unsigned int elpase = 0) { _screen.present(elpase); }

    void fire_connected(short camera = -1);
    void fire_disconnected(short camera = -1);
    void fire_camera_changed(short camera = -1, bool post = true);

public:
    const screen_view& screen_ref(void) const { return _screen; }
    screen_view& screen_ref(void) { return _screen; }
    const screen_view* screen_ptr(void) const { return &_screen; }
    screen_view* screen_ptr(void) { return &_screen; }

    HWND safe_hwnd(void) const { return _screen.safe_hwnd(); }
    HWND screen_hwnd(void) const { return _screen.safe_hwnd(); }

    operator screen_view*() { return &_screen; }
    operator screen_view&() { return _screen;  }
    screen_view* operator->() { return &_screen; }
    const screen_view* operator->() const { return &_screen; }

    void set_layout(screen_formatter_::LAYOUT layout);
    void set_layout_page_prev(void) { _screen.set_layout_page_prev(); }
    void set_layout_page_next(void) { _screen.set_layout_page_next(); }
    int current_layout(void) const { return _screen.curr_layout(); }
    bool is_layout(int layout) const { return _screen.is_layout(layout); }
    bool is_layout_1x1(void) const { return _screen.is_layout_1x1(); }

    bool enable_window(bool enable = true);
    bool show(int command = SW_SHOWNORMAL);
    bool set_window_pos(const CWnd* insertafter, const CRect& rect, unsigned int flags);
    void move_window(const CRect& rect, bool repaint = true);
    void set_focus(void);
    void set_focus_post(void);
    void set_active_window(void);

    unsigned __int64 visible_camera(void) const { return _screen.visible_camera(); }
    unsigned __int64 visible_camera_mode(int mode)  const { return _screen.visible_camera_mode(mode); }

public:
    void close_decoder(short camera) { return _decoder.close(camera); }
    void close_decoder(unsigned __int64 cameras);
    void close_decoder(void) { _decoder.close(); }

public:
    void prepare_looper(int priority);
    void release_looper(void);

    void run(void);
    void suspend(void);
    void resume(void);

    bool is_suspended(void) const;
    bool is_suspending(void) const;
    bool is_running(void) const;

    void __stdcall looper_func(void);

protected:
    void actuator_video(void);
    void actuator_live(const frame_element& element);
    void actuator_play(const frame_element& element);
    void video_play(screen::looper_::MODE mode, const frame_element& element);

    void play_search_audio(void) {}
    void stop_search_audio(void) {}
    void set_search_audio_spot(const G2SPOT& spot) {}

public:
    void put_frame(G2FRAME::FROM from, const G2FRAME& frame, short hostId, short basecamera, unsigned __int64 refcameras);

    void setup_buffer(short hostId, frame_buffer::TYPE type, frame_buffer::PLAYTIME time = frame_buffer::TIME_MSEC);

    void clear_frame_buffer(short hostId);
    void clear_frame_buffer(short hostId, int hostcamera);
    void clear_frame_buffer(short hostId, const g2::channels& channelext);
    void clear_frame_buffer(short hostId, const std::set<int>& channelexts);
    void clear_frame_buffer_play(short hostId);
    void clear_frame_buffer_play(short hostId, int channelext);

    void remove_buffer(short hostId);
    void remove_buffer(void);

    void set_prepare_drive(short hostId, bool prepare);
    bool is_prepare_drive(short hostId) const;

    bool is_ignore_frame(void) const { return _ignoreFrame; }
    void set_ignore_frame(bool ignore) { _ignoreFrame = ignore; }

    bool is_ignore_display(void) const { return _ignoreDisplay; }
    void set_ignore_display(bool ignore) { _ignoreDisplay = ignore; }

    void set_search_shutdown(short hostId, unsigned __int64 cameras);
    void set_search_play_speed(short hostId, int speed, bool audioIgnore = false);
    void set_search_end_play(G2FRAME::FROM from, int hostId);

    void stop_search_enter(short hostId);
    void stop_search_leave(short hostId);

public:
    void put_text_in(short hostId, short camera, const G2TEXT_IN_ELEMENT& element, bool tohead = false);
    void delete_text_in(const unsigned __int64& cameras);

public:
    void set_deinterlace_filter(short camera, G2DECODER_VIDEO_DEINTERLACE::FILTER filter);
    void set_deinterlace_filter_all(G2DECODER_VIDEO_DEINTERLACE::FILTER filter);

protected:
    virtual void on_screen_camera_changed(short camera);
    virtual void on_screen_layout_changed(int layout, int changed = client::screen::layout_change_::CHANGE);
    virtual void on_screen_image_drew(short camera, const G2SPOT& spot);

    virtual void on_screen_lbutton_down(unsigned int flags, const CPoint& point);
    virtual void on_screen_lbutton_up(unsigned int flags, const CPoint& point);
    virtual void on_screen_lbutton_dblclk(unsigned int flags, const CPoint& point);
    virtual void on_screen_rbutton_down(unsigned int flags, const CPoint& point);
    virtual void on_screen_rbutton_up(unsigned int flags, const CPoint& point);
    virtual void on_screen_mbutton_down(unsigned int flags, const CPoint& point);
    virtual void on_screen_mbutton_up(unsigned int flags, const CPoint& point);
    virtual void on_screen_mouse_move(unsigned int flags, const CPoint& point);
    virtual void on_screen_mouse_wheel(unsigned int flags, short zdelta, const CPoint& point);
    virtual bool on_screen_set_cursor(CWnd* pWnd, unsigned int hittest, unsigned int message, int camera, const CRect& rect);
    virtual void on_screen_key_down(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags);
    virtual void on_screen_key_up(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags);
    virtual void on_screen_set_foucs(CWnd* pOldWnd);
    virtual void on_screen_resized(unsigned int type, int cx, int cy);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SCREEN_ACTUATOR_H_