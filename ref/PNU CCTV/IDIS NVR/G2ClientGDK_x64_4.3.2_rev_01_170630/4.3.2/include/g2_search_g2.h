// g2_search_g2.h : header file
//

#ifndef _G2_CLIENT_DLL_SEARCH_G2_H_
#define _G2_CLIENT_DLL_SEARCH_G2_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_search.h"
#include "g2_define_search_g2.h"
#include "g2_define_play.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_search_g2_register_callback(G2HSEARCH_G2 handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HSEARCH_G2 G2API g2_search_g2_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_search_g2_finalize(G2HSEARCH_G2 handle);
G2_DLLFUNC void G2API g2_search_g2_startup(G2HSEARCH_G2 handle, int connections);
G2_DLLFUNC void G2API g2_search_g2_cleanup(G2HSEARCH_G2 handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_search_g2_connect(G2HSEARCH_G2 handle, const G2GUID* root, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC int  G2API g2_search_g2_connect_ras(G2HSEARCH_G2 handle, const G2NETWORK_INFO* ni, bool port_unity, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_search_g2_disconnect(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_is_connecting(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_is_connected(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_is_disconnecting(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_is_disconnected(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_is_disconnectable(G2HSEARCH_G2 handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_search_g2_set_invoke_saver(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC void G2API g2_search_g2_set_revoke_saver(G2HSEARCH_G2 handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_g2_set_search_target(G2HSEARCH_G2 handle, int channel, int target);
G2_DLLFUNC bool G2API g2_search_g2_set_camera_list(G2HSEARCH_G2 handle, int channel, const G2CHANNEL_SET* channels, const G2ROLLBACK_INFO* rbi, bool prepare_rollback, bool* preparing);
G2_DLLFUNC bool G2API g2_search_g2_set_camera_list_interest(G2HSEARCH_G2 handle, int channel, const G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_search_g2_set_player_scope(G2HSEARCH_G2 handle, int channel, const G2SCOPE* scope);
G2_DLLFUNC bool G2API g2_search_g2_set_player_scope_reset(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_set_player_audio_play(G2HSEARCH_G2 handle, int channel, int audio);
G2_DLLFUNC void G2API g2_search_g2_set_play_control_command(G2HSEARCH_G2 handle, int channel, int command);
G2_DLLFUNC void G2API g2_search_g2_set_event_query_mode(G2HSEARCH_G2 handle, int channel, int mode);
G2_DLLFUNC void G2API g2_search_g2_set_event_query_cameras(G2HSEARCH_G2 handle, int channel, const G2CHANNEL_SET* cameras);
G2_DLLFUNC void G2API g2_search_g2_set_probe_session_profile(G2HSEARCH_G2 handle, bool active);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_g2_request_alive_check(G2HSEARCH_G2 handle, int channel, int check);
G2_DLLFUNC bool G2API g2_search_g2_request_db_info(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_db_info_external(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_db_select(G2HSEARCH_G2 handle, int channel, int id, int external_type, int external_num);
G2_DLLFUNC bool G2API g2_search_g2_request_virtual_channelmap(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_record_time_info(G2HSEARCH_G2 handle, int channel, int resolution, int direction, const G2SCOPE* scope, int count, int command);
G2_DLLFUNC bool G2API g2_search_g2_request_record_time_info_load_stop(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_record_time_info_on_time(G2HSEARCH_G2 handle, int channel, int resolution, int direction, const G2TIME* from, const G2TIME* to, int count, int command, G2SCOPE* res_scope);
G2_DLLFUNC bool G2API g2_search_g2_request_reload_current(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_reload_recent(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_play(G2HSEARCH_G2 handle, int channel, const G2PLAYBACK_COMMAND* command);
G2_DLLFUNC bool G2API g2_search_g2_request_pause(G2HSEARCH_G2 handle, int channel, bool rollback, const G2ROLLBACK_INFO* rbi);
G2_DLLFUNC bool G2API g2_search_g2_request_stop(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_goto_time_first_of(G2HSEARCH_G2 handle, int channel, const G2TIME* time, bool load_adjacent_frame, bool forward, bool* found_spot);
G2_DLLFUNC bool G2API g2_search_g2_request_move_to_first(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_move_to_last(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_move_to_spot(G2HSEARCH_G2 handle, int channel, const G2SPOT* spot, int precision, bool forward);
G2_DLLFUNC bool G2API g2_search_g2_request_move_to_play(G2HSEARCH_G2 handle, int channel, const G2PLAYBACK_COMMAND* command);
G2_DLLFUNC bool G2API g2_search_g2_request_prev_step(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_next_step(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_notify_end_of_play(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_scope_list(G2HSEARCH_G2 handle, int channel, const G2TIME* from, const G2TIME* to, const G2CHANNEL_SET* channels, int type);
G2_DLLFUNC bool G2API g2_search_g2_request_spot_list(G2HSEARCH_G2 handle, int channel, const G2TIME* time, const G2CHANNEL_SET* channels, bool load_adjacent_frame);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_measure_size(G2HSEARCH_G2 handle, int channel, const G2CHANNEL_SET* channels, const G2SCOPE* scope, unsigned __int64 free_space, const int ordered_set[], unsigned int ordered_set_len, bool slice, unsigned __int64 slice_size, bool exclude_player);
G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_info(G2HSEARCH_G2 handle, int channel, const G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_enable_channelset(G2HSEARCH_G2 handle, int channel, G2CHANNEL_SET* out);
G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_password(G2HSEARCH_G2 handle, int channel, const wchar_t* password);
G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_text_in(G2HSEARCH_G2 handle, int channel, bool include);
G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_gps_data(G2HSEARCH_G2 handle, int channel, bool include);
G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_event(G2HSEARCH_G2 handle, int channel, bool include);
G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_cancel(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_size(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_clipcopy_data(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_event_log_search(G2HSEARCH_G2 handle, int channel, const G2SEARCH_G2_EVENT_SEARCH_OPTIONS* option);
G2_DLLFUNC bool G2API g2_search_g2_request_event_log_search_next(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_event_log_search_stop(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_text_in_log_search(G2HSEARCH_G2 handle, int channel, const G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS* option);
G2_DLLFUNC bool G2API g2_search_g2_request_text_in_log_search_next(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_request_text_in_log_search_stop(G2HSEARCH_G2 handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_g2_get_adaptor(G2HSEARCH_G2 handle, void* ptr);
G2_DLLFUNC bool G2API g2_search_g2_get_server_network_info(G2HSEARCH_G2 handle, int channel, G2SERVER_NETWORK_INFO* ni);
G2_DLLFUNC bool G2API g2_search_g2_get_product_info(G2HSEARCH_G2 handle, int channel, G2_PRODUCT_INFO* pi);
G2_DLLFUNC bool G2API g2_search_g2_get_remote_search_caps(G2HSEARCH_G2 handle, int channel, G2_PRODUCT_INFO_CAPS::REMOTE_SEARCH* caps);
G2_DLLFUNC bool G2API g2_search_g2_get_remote_clipcopy_caps(G2HSEARCH handle, int channel, G2_PRODUCT_INFO_CAPS::REMOTE_CLIP_COPY* caps);
G2_DLLFUNC int  G2API g2_search_g2_get_remote_selected_db(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_get_remote_db_info(G2HSEARCH_G2 handle, int channel, G2SEARCH_G2_REMOTE_DB* info);
G2_DLLFUNC bool G2API g2_search_g2_get_text_in_search_caps(G2HSEARCH_G2 handle, int channel, G2_PRODUCT_INFO_CAPS::TEXT_IN_SEARCH* caps);
G2_DLLFUNC bool G2API g2_search_g2_get_authority(G2HSEARCH_G2 handle, int channel, G2RAS_AUTHORITY* auth);
G2_DLLFUNC bool G2API g2_search_g2_get_camera_list(G2HSEARCH_G2 handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_search_g2_get_camera_list_interest(G2HSEARCH_G2 handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC int  G2API g2_search_g2_get_play_speed(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC int  G2API g2_search_g2_get_play_control_command(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC int  G2API g2_search_g2_get_current_command(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC int  G2API g2_search_g2_get_event_query_mode(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_get_event_query_cameras(G2HSEARCH_G2 handle, int channel, G2CHANNEL_SET* cameras);
G2_DLLFUNC bool G2API g2_search_g2_get_option_query_event(G2HSEARCH_G2 handle, int channel, G2SEARCH_G2_EVENT_SEARCH_OPTIONS* options);
G2_DLLFUNC bool G2API g2_search_g2_get_option_query_text_in(G2HSEARCH_G2 handle, int channel, G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS* options);
G2_DLLFUNC bool G2API g2_search_g2_get_clipcopy_size_info(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_SIZE_INFO* info);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_g2_is_drive_mode(G2HSEARCH_G2 handle, int channel, int mode);
G2_DLLFUNC bool G2API g2_search_g2_is_event_query_mode(G2HSEARCH_G2 handle, int channel, int mode);
G2_DLLFUNC bool G2API g2_search_g2_is_loading_log_event(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_is_loading_record_time_info(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_is_stopped(G2HSEARCH_G2 handle, int channel);
G2_DLLFUNC bool G2API g2_search_g2_is_support(G2HSEARCH_G2 handle, int channel, int query);
G2_DLLFUNC bool G2API g2_search_g2_is_authority(G2HSEARCH_G2 handle, int channel, int authority);
G2_DLLFUNC bool G2API g2_search_g2_is_probe_session_profile(G2HSEARCH_G2 handle);
G2_DLLFUNC bool G2API g2_search_g2_text_in_search_options_condition_is_valid(const G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION* condition);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_g2_sole_set_camera_list(G2HSEARCH_G2 handle, int channel, const G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_search_g2_sole_set_player_scope(G2HSEARCH_G2 handle, int channel, int camera, const G2SCOPE* scope);
G2_DLLFUNC bool G2API g2_search_g2_sole_set_player_scope_reset(G2HSEARCH_G2 handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_search_g2_sole_set_player_audio_play(G2HSEARCH_G2 handle, int channel, int camera, int audio);
G2_DLLFUNC bool G2API g2_search_g2_sole_get_camera_list(G2HSEARCH_G2 handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_record_time_info(G2HSEARCH_G2 handle, int channel, int camera, int resolution, int direction, const G2SCOPE* scope, int count);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_record_time_info_load_stop(G2HSEARCH_G2 handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_record_time_info_on_time(G2HSEARCH_G2 handle, int channel, int camera, int resolution, int direction, const G2TIME* from, const G2TIME* to, int count, G2SCOPE* res_scope);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_play(G2HSEARCH_G2 handle, int channel, int camera, const G2PLAYBACK_COMMAND* command);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_pause(G2HSEARCH_G2 handle, int channel, int camera, bool rollback, const G2ROLLBACK_INFO* rbi);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_stop(G2HSEARCH_G2 handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_move_to_first(G2HSEARCH_G2 handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_move_to_last(G2HSEARCH_G2 handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_move_to_spot(G2HSEARCH_G2 handle, int channel, int camera, const G2SPOT* spot, int precision, bool forward);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_move_to_play(G2HSEARCH_G2 handle, int channel, int camera, const G2PLAYBACK_COMMAND* command);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_prev_step(G2HSEARCH_G2 handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_next_step(G2HSEARCH_G2 handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_notify_end_of_play(G2HSEARCH_G2 handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_scope_list(G2HSEARCH_G2 handle, int channel, int camera, const G2TIME* from, const G2TIME* to);
G2_DLLFUNC bool G2API g2_search_g2_sole_request_spot_list(G2HSEARCH_G2 handle, int channel, int camera, const G2TIME* time, bool load_adjacent_frame);
G2_DLLFUNC bool G2API g2_search_g2_sole_is_loading_record_time_info(G2HSEARCH_G2 handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_search_g2_sole_is_stopped(G2HSEARCH_G2 handle, int channel, int camera);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_SEARCH_G2_H_
