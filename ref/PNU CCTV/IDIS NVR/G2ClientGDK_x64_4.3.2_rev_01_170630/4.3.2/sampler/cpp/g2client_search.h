// g2client_search.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_SEARCH_H_
#define _G2_CLIENT_DLL_SAMPLER_SEARCH_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_search.h>

namespace client {
    class g2search_listener;
    class g2search_listener_saver;

//////////////////////////////////////////////////////////////////////////

class g2search
{
public:
    g2search(void);
    virtual ~g2search(void);

private:
    G2HSEARCH _handle;
    g2search_listener* _listener;
    g2search_listener_saver* _listener_saver;

public:
    void startup(int connections);
    void cleanup(void);
    void set_listener(g2search_listener* listener);

    G2HSEARCH safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& root, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    int  connect_ras(const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    void set_invoke_saver(int channel, g2search_listener_saver* listener);
    void set_revoke_saver(int channel);
    void set_probe_session_profile(bool active);

public:
    bool set_camera_list(int channel, unsigned int cameras);
    bool set_current_channel(int channel, const G2SPOT& spot);
    bool set_search_target(int channel, G2SEARCH_TARGET::TYPE target);
    bool set_change_segment(int channel, int id);
    bool set_change_search_mode(int channel, bool event_search);
    bool set_external_tango(int channel, int type, int number);
    void set_query_mode(int channel, G2SEARCH_QUERY::MODE mode);
    void set_query_cameras(int channel, unsigned int cameras);
    void set_invoke_on_play(int channel, const G2SPOT& spot);
    void set_revoke_on_play(int channel);

public:
    bool get_server_network_info(int channel, G2SERVER_NETWORK_INFO& ni) const;
    bool get_product_info(int channel, G2_PRODUCT_INFO& pi) const;
    bool get_remote_search_caps(int channel, G2_PRODUCT_INFO_CAPS::REMOTE_SEARCH& caps) const;
    bool get_authority(int channel, G2RAS_AUTHORITY& auth) const;
    bool get_camera_list(int channel, unsigned int& cameras) const;
    bool get_time_last(int channel, G2TIME& time) const;
    G2SEARCH_DRIVE::MODE get_drive_mode(int channel) const;
    G2SEARCH_QUERY::MODE get_query_mode(int channel) const;
    G2SEARCH_TIMELAPSE::MODE get_timelapse_mode(int channel) const;
    int  get_play_speed(int channel) const;
    int  get_current_command(int channel) const;
    G2SEARCH_TARGET::TYPE get_search_target(int channel) const;
    bool get_query_cameras(int channel, unsigned int& cameras) const;
    bool get_query_condition_event(int channel, G2EVENT_QUERY_CONDITION& condition) const;
    bool get_query_condition_text_in(int channel, G2TEXT_IN_QUERY_CONDITION& condition) const;
    bool get_query_result_text_in(int channel, int count, G2TEXT_IN data[]) const;
    bool get_query_result_event(int channel, int selected, G2SEARCH_LOG_INFO& data) const;
    bool is_drive_mode(int channel, G2SEARCH_DRIVE::MODE mode) const;
    bool is_query_mode(int channel, G2SEARCH_QUERY::MODE mode) const;
    bool is_timelapse_mode(int channel, G2SEARCH_TIMELAPSE::MODE mode) const;
    bool is_event_search_mode(int channel) const;
    bool is_stopped(int channel) const;
    bool is_loading(int channel) const;
    bool is_support(int channel, G2SEARCH_SUPPORT::QUERY query) const;

public:
    bool request_record_date(int channel);
    bool request_record_time(int channel, const G2TIME& date);
    bool request_record_hour(int channel, const G2SPOT& spot, int length, bool direction, bool check_diff_prev_req = true);
    bool request_record_hour_command(int channel, int command, const G2SPOT& spot);
    bool request_playback(int channel, int command, const G2SPOT& spot);
    bool request_reload_current(int channel, int camera, const G2SPOT& spot);
    bool request_goto_displayed_spot(int channel, const G2SPOT& spot);
    bool request_notify_end_of_play(int channel);
    bool request_query_specific_event(int channel, int seq_number, const G2TIME& begin, const G2TIME& end, int event_type, int camera);
    bool request_query_event(int channel, const G2EVENT_QUERY_CONDITION& condition);
    bool request_query_text_in(int channel, const G2TEXT_IN_QUERY_CONDITION& condition);
    bool request_event_image(int channel, int selected, bool last);
    bool request_event_search_stop_idr(int channel);

public:
    bool request_clipcopy_get_enable_channel(int channel);
    bool request_clipcopy_scope(int channel, const G2TIME& from, const G2TIME& to, unsigned int cameras);
    bool request_clipcopy_measure_size(int channel, const G2SCOPE& scope, unsigned __int64 freespace, bool slice);
    bool request_clipcopy_size(int channel);
    bool request_clipcopy(int channel, bool start);
    bool request_clipcopy_password(int channel, bool use, const G2STRING_32& password);
    bool request_clipcopy_text_in(int channel, bool include);
    bool request_clipcopy_structure(int channel, int version, bool exclude_player, bool avoid_seek);
    bool request_clipcopy_cancel(int channel);
    bool request_mini_bank_begin(int channel);
    bool request_mini_bank_end(int channel);
    bool request_mini_bank_space(int channel, const G2TIME& from, const G2TIME& to, unsigned int cameras, bool audio);
    bool request_mini_bank(int channel, int command, const G2TIME& time, int msec);
    bool request_gps_data_measure(int channel, const G2TIME& from, const G2TIME& to);
    bool request_gps_data_measuer_result(int channel);
    bool request_gps_data(int channel, const G2TIME& time);
    bool request_gps_data_next(int channel);
    bool request_gps_data_export_cancel(int channel);

protected:
    static G2RESULT G2CALLBACK on_connected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_recorded_date(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_recorded_time_hour(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_recorded_time_minute(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_recorded_rechour_minute(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_frame_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_no_frame(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_no_recorded_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_no_recorded_data_from_search_target(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_find_idr_event_time(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_play_speed_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_play_stop_post(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_end_of_play(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_segment_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_search_mode_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_command_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_query_result_event(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_query_result_text_in(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_recorded_date_scope(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_segment_spot(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_error(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_external_tango_info(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_gps_data_start(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_gps_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_gps_data_list(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_gps_data_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_gps_data_end_count(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_gps_data_export_cancel_result(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_gps_data_measure_result(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_require_prepare_playback(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_require_prepare_load_event_image(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_require_prepare_reload(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_probe_session_profile(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);

    static G2RESULT G2CALLBACK on_saver_connected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_disconnected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_recorded_date(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_frame_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_no_frame(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_no_recorded_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_notify_play_speed_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_notify_play_stop_spot(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_notify_end_of_play(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_notify_command_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_scope(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_measure_size(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_size(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_set_password(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_enable_channels(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_bank_space(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_bank_image(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_bank_audio(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_bank_no_image(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_bank_no_audio(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_SEARCH_H_
