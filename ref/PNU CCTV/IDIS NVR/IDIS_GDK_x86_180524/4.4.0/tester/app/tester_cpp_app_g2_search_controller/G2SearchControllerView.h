// G2SearchControllerView.h : header file
//

#ifndef _G2SEARCH_CONTROLLER_VIEW_H_
#define _G2SEARCH_CONTROLLER_VIEW_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "stdafx.h"
#include "actuator/screen_actuator_listener.h"

#include <sampler/cpp/g2client_search_g2_listener.h>
#include <sampler/cpp/app/g2app_frame_relay_listener.h>

namespace client {
    class g2search_g2;
    class g2app_frame_relay;
    class G2SearchControllerDlg;
    class screen_actuator;
}

//////////////////////////////////////////////////////////////////////////

class G2SearchControllerView : public CWnd
                             , protected client::g2search_g2_listener
                             , protected client::g2app_frame_relay_listener
                             , protected client::screen_actuator_listener
{
public:
    G2SearchControllerView(G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION option);
    virtual ~G2SearchControllerView(void);

public:
    enum TIMER_ID {
        TIMER_ID_ALIVE_CHECK = 1024,
        TIMER_ID_DISCONNECT = 2048
    };

    enum CONST_DATA {
        ALIVE_CHECK_INTERVAL = 1000 * 30
    };

protected:
    boost::shared_ptr<client::g2search_g2> _search_g2;
    boost::shared_ptr<client::g2app_frame_relay> _relay;
    boost::shared_ptr<client::G2SearchControllerDlg> _controller;
    boost::shared_ptr<client::screen_actuator> _actuator;
    
    g2::critical_section _lock_cameralist;

    int _relay_channel;
    int _search_channel;
    std::set<short> _endofplayHost;
    G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION _option;

public:
    boost::shared_ptr<client::g2search_g2> search_g2_ptr(void) const { return _search_g2; }
    client::G2SearchControllerDlg& controller_ref(void) const { return *_controller; }
    client::screen_actuator& actuator_ref(void) { return *_actuator; }
    const G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION& option(void) { return _option; }

protected:
    HWND safe_hwnd(void) const { return GetSafeHwnd(); }

    void initialize(void);
    void finalize(void);
    void ctor_network(void);
    void dtor_network(void);
    void cleanup(void);

public:
    int set_play_speed(short channel, int command, int clientSpeed = client::search::speed_::PLAY_SPEED_UNDEFINED);

    void show_screen_message(const std::wstring& string, bool autohide = true, int elapse = 5000);
    void show_screen_message(const std::wstring& string, const std::wstring& message, bool autohide = true, int elapse = 5000);
    void hide_screen_message(void);

    //////////////////////////////////////////////////////////////////////////

protected:
    void set_camera_list(void);
    void set_camera_list_impl_search_g2(short channel, const std::set<int>& channelSetInterest, const std::set<int>& channelSetPlay);

    //////////////////////////////////////////////////////////////////////////

public:
    bool connect_relay(void);

protected:
    bool connect_search(void);
    void disconnect_search(void);

    bool is_connected(void);
    bool is_disconnected(void);
    bool is_disconnectable(void);
    
public:
    G2SPOT find_last_spot(void);

protected:
    virtual void on_g2app_frame_relay_connected(G2HANDLE handle, int channel);
    virtual void on_g2app_frame_relay_disconnected(G2HANDLE handle, int channel, int reason);
    virtual void on_g2app_frame_relay_receive_request_site_product_info(G2HANDLE handle, int channel);

protected:
    virtual void on_g2search_g2_connected(G2HSEARCH_G2 handle, int channel);
    virtual void on_g2search_g2_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON::TYPE reason);
    virtual void on_g2search_g2_receive_record_time_info_load(G2HSEARCH_G2 handle, int channel, const G2RECORD_TIME_INFO& rti);
    virtual void on_g2search_g2_receive_record_time_info_load_end(G2HSEARCH_G2 handle, int channel, G2RECORD_TIME_INFO::RESOLUTION resolution, G2RECORD_TIME_INFO::COMMAND command);
    virtual void on_g2search_g2_receive_frame_data(G2HSEARCH_G2 handle, int channel, const G2FRAME& frame);
    virtual void on_g2search_g2_receive_text_in(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei) {}
    virtual void on_g2search_g2_receive_event(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei) {}
    virtual void on_g2search_g2_receive_notify_command_begin(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command);
    virtual void on_g2search_g2_receive_notify_command_end(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command);
    virtual void on_g2search_g2_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED speed);
    virtual void on_g2search_g2_receive_notify_frame_not_found(G2HSEARCH_G2 handle, int channel, G2SPOT& spot, G2PLAYER::PRECISION::TYPE precision);
    virtual void on_g2search_g2_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE status);
    virtual void on_g2search_g2_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, G2ROLLBACK_INFO& rbi);
    virtual void on_g2search_g2_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE error);
    virtual void on_g2search_g2_receive_event_log_load_end(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list) {}
    virtual void on_g2search_g2_receive_event_log_load_stop(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list) {}
    virtual void on_g2search_g2_receive_text_in_log_load_end(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list) {}
    virtual void on_g2search_g2_receive_text_in_log_load_stop(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list) {}
    virtual void on_g2search_g2_receive_scope_list(G2HSEARCH_G2 handle, int channel,  const std::vector<G2SCOPE>& scopes, G2SEARCH_G2_SCOPE_TYPE::TYPE type);
    virtual void on_g2search_g2_receive_spot_list(G2HSEARCH_G2 handle, int channel, const std::vector<G2SPOT>& spots);
    virtual void on_g2search_g2_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel);
    virtual void on_g2search_g2_receive_db_info(G2HSEARCH_G2 handle, int channel, const G2SEARCH_G2_REMOTE_DB& di);
    virtual void on_g2search_g2_receive_db_info_external(G2HSEARCH_G2 handle, int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& dis);
    virtual void on_g2search_g2_receive_db_selected(G2HSEARCH_G2 handle, int channel, unsigned int id, G2SEARCH_G2_REMOTE_DB::DB_SELECT_RESULT reason);
    virtual void on_g2search_g2_receive_virtual_channelmap(G2HSEARCH_G2 handle, int channel);
    virtual void on_g2search_g2_require_prepare_rollback(G2HSEARCH_G2 handle, int channel, bool prepare);
    virtual void on_g2search_g2_probe_session_profile(G2HSEARCH_G2 handle, int channel, const G2PROBE_SESSION_PROFILE& probe) {}

protected:
    virtual void on_screen_image_loaded(const G2FRAME& frame);
    virtual void on_screen_no_image_loaded(short channel, short camera);

public:
    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnTimer(UINT_PTR nIDEvent);

    afx_msg LRESULT on_post_connect_relay(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_connected_relay(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_disconnected_relay(WPARAM wParam, LPARAM lParam);

    afx_msg LRESULT on_post_connect_search_g2(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_connected_search_g2(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_disconnected_search_g2(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_screen_no_image_loaded_search_g2(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_rectime_info_load_end_search_g2(WPARAM wParm, LPARAM lParm);
    afx_msg LRESULT on_post_no_recorded_data_search_g2(WPARAM wParm, LPARAM lParm);
    afx_msg LRESULT on_post_receive_command_begin_search_g2(WPARAM wParm, LPARAM lParm);

    //////////////////////////////////////////////////////////////////////////

public:
    static G2SearchControllerView* __self_pointer__;
};

//////////////////////////////////////////////////////////////////////////

namespace client {
    typedef G2SearchControllerView runner;
    client::runner* runner_ptr(void);
}

#endif // !_G2SEARCH_CONTROLLER_VIEW_H_
