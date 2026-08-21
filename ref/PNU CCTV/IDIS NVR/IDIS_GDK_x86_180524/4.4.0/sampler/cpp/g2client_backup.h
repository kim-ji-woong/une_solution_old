// g2client_backup.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_BACKUP_H_
#define _G2_CLIENT_DLL_SAMPLER_BACKUP_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_backup.h>
#include <vector>
#include <set>

namespace client {
    class g2backup_listener;

//////////////////////////////////////////////////////////////////////////

class g2backup
{
public:
    g2backup(void);
    virtual ~g2backup(void);

private:
    G2HBACKUP _handle;
    g2backup_listener* _listener;

public:
    void startup(int connections);
    void cleanup(void);
    void set_listener(g2backup_listener* listener);

    G2HBACKUP safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& service, const G2GUID& site, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool set_camera_list(int channel, const std::set<int>& channels, const G2ROLLBACK_INFO& rbi, bool prepare_rollback, bool* preparing);
    bool set_camera_list_interest(int channel, const std::set<int>& channels);
    void set_play_control_command(int channel, int command);
    void set_event_query_mode(int channel, int mode);
    void set_probe_session_profile(bool active);

public:
    bool request_service_info(int channel);
    bool request_alive_check(int channel, int check);
    bool request_backup_site(int channel, const G2GUID& siteGUID);
    bool request_record_channels(int channel);
    bool request_record_channels_each(int channel, const std::vector<G2GUID>& cameras);
    bool request_record_time_info(int channel, int resolution, int direction, const G2SCOPE& scope, int count, int command);
    bool request_record_time_info_load_stop(int channel);
    bool request_query_no_recorded_data(int channel, const std::vector<G2GUID>& cameras);
    bool request_reload_current(int channel);
    bool request_play(int channel, const G2PLAYBACK_COMMAND& command);
    bool request_pause(int channel, bool rollback, const G2ROLLBACK_INFO& rbi);
    bool request_move_to_first(int channel);
    bool request_move_to_last(int channel);
    bool request_move_to_spot(int channel, const G2SPOT& spot, int precision, bool move_forward);
    bool request_prev_step(int channel);
    bool request_next_step(int channel);
    bool request_notify_end_of_play(int channel);
    bool request_scope_list(int channel, const G2TIME& from, const G2TIME& to, const std::set<int>& channels, int type);
    bool request_spot_list(int channel, const G2TIME& time, const std::set<int>& channels);
    bool request_event_log_search(int channel, const G2SERVICE_SEARCH_OPTION_EVENT_LOG& option);
    bool request_event_log_search_stop(int channel);
    bool request_text_in_log_search(int channel, const G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG* option);
    bool request_text_in_log_search_next(int channel);
    bool request_text_in_log_search_stop(int channel);
    bool request_text_in_search(int channel, const G2TEXT_IN_QUERY_CONDITION* option);
    bool request_text_in_search_stop(int channel);
    bool request_system_log_search(int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option);
    bool request_system_log_search_stop(int channel);

public:
    bool get_camera_list(int channel, std::set<int>& channels) const;
    bool get_camera_list_interest(int channel, std::set<int>& channels) const;
    int  get_play_speed(int channel) const;
    int  get_play_control_command(int channel) const;
    int  get_current_command(int channel) const;
    int  get_event_query_mode(int channel) const;
    bool is_stopped(int channel) const;

protected:
    static G2RESULT G2CALLBACK on_connected(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_backup_site_result(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_record_channels(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_record_time_info_load(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_record_time_info_load_end(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_response_no_recorded_data(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_frame_data(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_command_begin(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_command_end(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_play_speed_changed(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_frame_not_found(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_out_of_scope(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_player_error(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_scope_list(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_spot_list(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_no_recorded_data(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_end(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_fail(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_stop(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_require_prepare_rollback(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_probe_session_profile(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_BACKUP_H_
