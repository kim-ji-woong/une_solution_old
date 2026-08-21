// controller_search_g2.h : header file
//

#ifndef _CONTROLLER_SEARCH_G2_H_
#define _CONTROLLER_SEARCH_G2_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "controller_base.h"
#include "control/play/listener/listener_time_table.h"
#include "control/play/listener/listener_search_g2_event_list.h"

#include <include/g2_define_search_g2.h>
#include <boost/shared_ptr.hpp>

namespace client {
    class g2search_g2;
    class time_table_minute_g2;
    class search_g2_event_list;

//////////////////////////////////////////////////////////////////////////

class controller_search_g2 : public controller_base
                           , public listener_time_table
                           , public listener_search_g2_event_list
{
public:
    controller_search_g2(void);
    virtual ~controller_search_g2(void);

protected:
    struct move_    { enum STEP { FRAME_1 = 0, MIN_01, MIN_05, MIN_10, MIN_15, MIN_30, MIN_60 };};
    struct mode_    { enum TYPE { TIMELAPSE = 0, EVENT };};
    struct const_   { enum TYPE { PLAYBACK_HEIGHT = 50 };};
    struct control_ { enum ID   { TIME_TABLE = 1024, EVENT_LIST };};
    struct timer_   { struct id_     { enum ID  { BALANCE_AUTO = 1200 };};
                      struct elapse_ { enum VAL { BALANCE_AUTO = 30 * 1000 };};};

    struct controller_info {
        controller_info(void)
            : _host_id(client::invalid_::HOST_ID)
            , _channel(client::invalid_::CHANNEL)
            , _channelext(client::invalid_::CHANNEL_EXT)
            , _selcamera(client::invalid_::CAMERA_NUMBER) { memset(&_playback, 0, sizeof(_playback)); }
        void reset(void)
        {
            _host_id    = client::invalid_::HOST_ID;
            _channel    = client::invalid_::CHANNEL;
            _channelext = client::invalid_::CHANNEL_EXT;
            _selcamera  = client::invalid_::CAMERA_NUMBER;
            memset(&_playback, 0, sizeof(_playback));
        }

        int _host_id;
        int _channel;
        int _channelext;
        int _selcamera;
        G2PLAYBACK_COMMAND _playback;

    };

protected:
    client::g2search_g2* _adaptor;
    controller_info _ci;
    boost::shared_ptr<time_table_minute_g2> _time_table;
    boost::shared_ptr<search_g2_event_list> _event_list;
    int  _move_step;
    int  _mode;
    bool _enable;

    g2::critical_section _cs_data;

protected:
    CComboBox   _cbx_format;
    CSliderCtrl _sld_speed;
    CStatic     _stc_speed;
    CButton     _btn_mode;
    CButton     _btn_play;
    CButton     _btn_stop;
    CButton     _btn_clipcopy;

protected:
    virtual void initialize(void);
    virtual void finalize(void);

    void stop_impl(void);
    void enable_control(int channel);
    void reset_speed(void);
    int  set_play_speed(int command, int client_speed = client::search::speed_::PLAY_SPEED_UNDEFINED);
    void request_speed_shuttle(void);
    void load_time_table(int channel);
    bool balance_time_table(void);
    void change_search_mode(const int mode);

public:
    virtual bool create(CWnd* parent);

public:
    virtual void on_screen_layout_changed(int layout, int changed);
    virtual void on_screen_image_drew(short camera, const G2SPOT& spot);
    virtual void on_changed_select_time_table(const G2SPOT& spot);
    virtual void on_request_more_event_search_g2(void);
    virtual void on_request_load_event_image_search_g2(G2SPOT spot);

public:
    void on_receive_notify_command_begin_play(int channel, int command);
    void on_receive_notify_command_end_play(int channel, int command);
    void on_receive_notify_play_speed_changed_play(int channel, int speed);
    void on_receive_scope_list(int channel, const std::vector<G2SCOPE>& scopes, int type);
    void on_receive_spot_list(int channel, const std::vector<G2SPOT>& spots);
    void on_receive_event_query_result(int channel, const std::vector<G2EVENT>& events);
    void on_receive_text_in_query_result(int channel, const std::vector<G2EVENT>& events);

public:
    void request_stop_sync(void);
    void request_first(void);
    void request_last(void);
    void request_stop(void);

    void screen_end_of_play(int host_id);
    bool is_origin_pos_speed(void);
    bool is_enable(void) const  { return _enable; }
    void set_enable(bool enble) { _enable = enble; }

    void load_site(int channel);
    void update_site(int channel);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog(void);
    virtual BOOL PreTranslateMessage(MSG* pMsg);

    DECLARE_MESSAGE_MAP();

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy(void);
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
    afx_msg void OnTimer(UINT_PTR nIDEvent);

    afx_msg void on_cbx_selchange_format(void);
    afx_msg void on_btn_mode(void);
    afx_msg void on_btn_filter(void);
    afx_msg void on_btn_layout_prev(void);
    afx_msg void on_btn_layout_next(void);
    afx_msg void on_btn_go_to(void);
    afx_msg void on_move_step(UINT nID);
    afx_msg void on_btn_backward(void);
    afx_msg void on_btn_forward(void);
    afx_msg void on_btn_move_step(void);
    afx_msg void on_btn_play(void);
    afx_msg void on_btn_stop(void);
    afx_msg void on_goto_time(void);
    afx_msg void on_goto_first(void);
    afx_msg void on_goto_last(void);
    afx_msg void on_btn_calendar(void);
    afx_msg void on_btn_clipcopy(void);
    afx_msg void on_sld_speed_release_capture(NMHDR* pNMHDR, LRESULT* pResult);

protected:
    LRESULT on_post_screen_camera_changed(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_receive_scope_list(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_receive_spot_list(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_enable_controls(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_disconnected(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_load_time_table(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_update_time_table(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROLLER_SEARCH_G2_H_
