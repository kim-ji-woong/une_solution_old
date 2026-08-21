// g2client_search_g2.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_SEARCH_G2_H_
#define _G2_CLIENT_DLL_SAMPLER_SEARCH_G2_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_search_g2.h>
#include <vector>
#include <set>

namespace client {
    class g2search_g2_listener;
    class g2search_g2_listener_sole;
    class g2search_g2_listener_saver;

//////////////////////////////////////////////////////////////////////////

class g2search_g2
{
public:
    g2search_g2(void);
    virtual ~g2search_g2(void);

private:
    G2HSEARCH_G2 _handle;
    g2search_g2_listener* _listener;
    g2search_g2_listener_sole* _listener_sole;
    g2search_g2_listener_saver* _listener_saver;

public:
    void startup(int connections);
    void cleanup(void);
    void set_listener(g2search_g2_listener* listener);
    void set_listener_sole(g2search_g2_listener_sole* listener);

    G2HSEARCH_G2 safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& root, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    int  connect_ras(const G2NETWORK_INFO& ni, bool port_unity, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    void set_invoke_saver(int channel, g2search_g2_listener_saver* listener);
    void set_revoke_saver(int channel);

public:
    bool set_search_target(int channel, G2SEARCH_TARGET::TYPE target);
    bool set_camera_list(int channel, const std::set<int>& channels, const G2ROLLBACK_INFO& rbi, bool prepare_rollback, bool* preparing);
    bool set_camera_list_interest(int channel, const std::set<int>& channels);
    bool set_player_scope(int channel, const G2SCOPE& scope);
    bool set_player_scope_reset(int channel);
    bool set_player_audio_play(int channel, G2PLAYER::AUDIO_PLAY::TYPE audio);
    void set_play_control_command(int channel, int command);
    void set_event_query_mode(int channel, int mode);
    void set_event_query_cameras(int channel, const std::set<int>& channels);
    void set_probe_session_profile(bool active);

public:
    bool get_server_network_info(int channel, G2SERVER_NETWORK_INFO& ni) const;
    bool get_product_info(int channel, G2_PRODUCT_INFO& pi) const;
    bool get_remote_search_caps(int channel, G2_PRODUCT_INFO_CAPS::REMOTE_SEARCH& caps) const;
    bool get_remote_db_info(int channel, G2SEARCH_G2_REMOTE_DB& info) const;
    bool get_authority(int channel, G2RAS_AUTHORITY& auth) const;
    bool get_camera_list(int channel, std::set<int>& channels) const;
    bool get_camera_list_interest(int channel, std::set<int>& channels) const;
    int  get_play_speed(int channel) const;
    int  get_play_control_command(int channel) const;
    int  get_remote_selected_db(int channel) const;
    int  get_current_command(int channel) const;
    int  get_event_query_mode(int channel) const;
    bool get_event_query_cameras(int channel, std::set<int>& channels) const;
    bool get_option_query_event(int channel, G2SEARCH_G2_EVENT_SEARCH_OPTIONS& options) const;
    bool get_option_query_text_in(int channel, G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& options) const;
    bool get_clipcopy_size_info(int channel, G2CLIPCOPY_SIZE_INFO& csi);

public:
    bool is_drive_mode(int channel, G2SEARCH_DRIVE::MODE mode) const;
    bool is_loading_record_time_info(int channel) const;
    bool is_stopped(int channel) const;
    bool is_support(int channel, G2SEARCH_SUPPORT::QUERY query) const;
    bool is_probe_session_profile(void) const;

public:
    bool request_db_info(int channel);
    bool request_db_select(int channel, G2SEARCH_G2_REMOTE_DB::STORAGE id);
    bool request_db_select(int channel, G2SEARCH_G2_REMOTE_DB::STORAGE id, int external_type, int external_num);
    bool request_record_time_info(int channel, int resolution, int direction, const G2SCOPE& scope, int count, int command);
    bool request_record_time_info_load_stop(int channel);
    bool request_record_time_info_on_time(int channel, int resolution, int direction, const G2TIME& from, const G2TIME& to, int count, int command, G2SCOPE* res_scope);
    bool request_reload_current(int channel);
    bool request_reload_recent(int channel);
    bool request_play(int channel, const G2PLAYBACK_COMMAND& command);
    bool request_pause(int channel, bool rollback, const G2ROLLBACK_INFO& rbi);
    bool request_stop(int channel);
    bool request_goto_time_first_of(int channel, const G2TIME& time, bool load_adjacent_frame = false, bool forward = true, bool* found_spot = NULL);
    bool request_move_to_first(int channel);
    bool request_move_to_last(int channel);
    bool request_move_to_spot(int channel, const G2SPOT& spot, int precision, bool forward = true);
    bool request_move_to_play(int channel, const G2PLAYBACK_COMMAND& command);
    bool request_prev_step(int channel);
    bool request_next_step(int channel);
    bool request_notify_end_of_play(int channel);
    bool request_scope_list(int channel, const G2TIME& from, const G2TIME& to, const std::set<int>& channels, int type);
    bool request_spot_list(int channel, const G2TIME& time, const std::set<int>& channels, bool load_adjacent_frame = false);
    bool request_clipcopy_measure_size(int channel, const std::set<int>& channels, const G2SCOPE& scope, unsigned __int64 free_space, const std::vector<int>& ordered_set, bool slice, unsigned __int64 slice_size, bool exlude_player);
    bool request_clipcopy_info(int channel, const std::set<int>& channels);
    bool request_clipcopy_password(int channel, const wchar_t* password);
    bool request_clipcopy_text_in(int channel, bool include);
    bool request_clipcopy_gps_data(int channel, bool include);
    bool request_clipcopy_event(int channel, bool include);
    bool request_clipcopy_cancel(int channel);
    bool request_clipcopy_size(int channel);
    bool request_clipcopy_data(int channel);
    bool request_event_log_search(int channel, const G2SEARCH_G2_EVENT_SEARCH_OPTIONS& option);
    bool request_event_log_search_next(int channel);
    bool request_event_log_search_stop(int channel);
    bool request_text_in_log_search(int channel, const G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& option);
    bool request_text_in_log_search_next(int channel);
    bool request_text_in_log_search_stop(int channel);

    bool sole_set_camera_list(int channel, const G2CHANNEL_SET* channels);
    bool sole_set_player_scope(int channel, int camera, const G2SCOPE* scope);
    bool sole_set_player_scope_reset(int channel, int camera);
    bool sole_set_player_audio_play(int channel, int camera, G2PLAYER::AUDIO_PLAY::TYPE audio);
    bool sole_get_camera_list(int channel, G2CHANNEL_SET* channels);
    bool sole_request_record_time_info(int channel, int camera, int resolution, int direction, const G2SCOPE* scope, int count);
    bool sole_request_record_time_info_load_stop(int channel, int camera);
    bool sole_request_record_time_info_on_time(int channel, int camera, int resolution, int direction, const G2TIME* from, const G2TIME* to, int count, G2SCOPE* res_scope);
    bool sole_request_play(int channel, int camera, const G2PLAYBACK_COMMAND* command);
    bool sole_request_pause(int channel, int camera, bool rollback, const G2ROLLBACK_INFO& rbi);
    bool sole_request_stop(int channel, int camera);
    bool sole_request_move_to_first(int channel, int camera);
    bool sole_request_move_to_last(int channel, int camera);
    bool sole_request_move_to_spot(int channel, int camera, const G2SPOT* spot, int precision, bool forward);
    bool sole_request_move_to_play(int channel, int camera, const G2PLAYBACK_COMMAND* command);
    bool sole_request_prev_step(int channel, int camera);
    bool sole_request_next_step(int channel, int camera);
    bool sole_request_notify_end_of_play(int channel, int camera);
    bool sole_request_scope_list(int channel, int camera, const G2TIME* from, const G2TIME* to);
    bool sole_request_spot_list(int channel, int camera, const G2TIME* time, bool load_adjacent_frame = false);
    bool sole_is_loading_record_time_info(int channel, int camera);
    bool sole_is_stopped(int channel, int camera);

protected:
    static G2RESULT G2CALLBACK on_connected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_query_options_search_base(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_query_options_player(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_record_time_info_load(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_record_time_info_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_frame_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_command_begin(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_command_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_frame_not_found(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_out_of_scope(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_player_error(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event_log_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event_log_load_stop(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in_log_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in_log_load_stop(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_scope_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_spot_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_no_recorded_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_db_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_db_info_external(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_db_selected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_virtual_channelmap(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_require_prepare_rollback(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_probe_session_profile(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);

    static G2RESULT G2CALLBACK on_sole_connected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_disconnected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_query_options_player(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_record_time_info_load(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_record_time_info_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_frame_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_text_in(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_event(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_notify_command_begin(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_notify_command_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_notify_frame_not_found(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_notify_out_of_scope(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_notify_player_error(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_scope_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_spot_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_receive_no_recorded_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_sole_require_prepare_rollback(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);

    static G2RESULT G2CALLBACK on_saver_connected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_disconnected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_frame_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_notify_out_of_scope(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_notify_player_error(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_scope_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_no_recorded_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_size(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_set_password(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_canceled(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_job_started(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_job_finished(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_section_begin(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_saver_receive_clipcopy_section_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};                             

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_SEARCH_G2_H_
