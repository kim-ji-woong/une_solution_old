// g2_play.h : header file
//

#ifndef _G2_CLIENT_DLL_PLAY_H_
#define _G2_CLIENT_DLL_PLAY_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_play.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_play_register_callback(G2HPLAY handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HPLAY G2API g2_play_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_play_finalize(G2HPLAY handle);
G2_DLLFUNC void G2API g2_play_startup(G2HPLAY handle, int connections);
G2_DLLFUNC void G2API g2_play_cleanup(G2HPLAY handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_play_connect(G2HPLAY handle, const G2GUID* service, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res, int usage);
G2_DLLFUNC void G2API g2_play_disconnect(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_is_connecting(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_is_connected(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_is_disconnecting(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_is_disconnected(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_is_disconnectable(G2HPLAY handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_play_set_camera_list(G2HPLAY handle, int channel, const G2CHANNEL_SET* channels, const G2ROLLBACK_INFO* rbi, bool prepare_rollback, bool* preparing);
G2_DLLFUNC bool G2API g2_play_set_camera_list_interest(G2HPLAY handle, int channel, const G2CHANNEL_SET* channels);
G2_DLLFUNC void G2API g2_play_set_play_control_command(G2HPLAY handle, int channel, int command);
G2_DLLFUNC void G2API g2_play_set_probe_session_profile(G2HPLAY handle, bool active);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_play_request_alive_check(G2HPLAY handle, int channel, int check);
G2_DLLFUNC bool G2API g2_play_request_hard_disk_info(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_record_channels(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_record_channels_each(G2HPLAY handle, int channel, const G2GUID camera[], unsigned int count);
G2_DLLFUNC bool G2API g2_play_request_record_time_info(G2HPLAY handle, int channel, int resolution, int direction, const G2SCOPE* scope, int count, int command);
G2_DLLFUNC bool G2API g2_play_request_record_time_info_load_stop(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_reload_current(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_play(G2HPLAY handle, int channel, const G2PLAYBACK_COMMAND* command);
G2_DLLFUNC bool G2API g2_play_request_pause(G2HPLAY handle, int channel, bool rollback, const G2ROLLBACK_INFO* rbi);
G2_DLLFUNC bool G2API g2_play_request_move_to_first(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_move_to_last(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_move_to_spot(G2HPLAY handle, int channel, const G2SPOT* spot, int precision, bool forward);
G2_DLLFUNC bool G2API g2_play_request_prev_step(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_next_step(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_notify_end_of_play(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_scope_list(G2HPLAY handle, int channel, const G2TIME* from, const G2TIME* to, const G2CHANNEL_SET* channels, int type);
G2_DLLFUNC bool G2API g2_play_request_spot_list(G2HPLAY handle, int channel, const G2TIME* time, const G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_play_request_event_log_search(G2HPLAY handle, int channel, const G2SERVICE_SEARCH_OPTION_EVENT_LOG* option);
G2_DLLFUNC bool G2API g2_play_request_event_log_search_stop(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_text_in_log_search(G2HPLAY handle, int channel, const G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG* option);
G2_DLLFUNC bool G2API g2_play_request_text_in_log_search_next(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_text_in_log_search_stop(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_device_log_search(G2HPLAY handle, int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG* option);
G2_DLLFUNC bool G2API g2_play_request_device_log_search_stop(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_system_log_search(G2HPLAY handle, int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG* option);
G2_DLLFUNC bool G2API g2_play_request_system_log_search_stop(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_request_debug_log_search(G2HPLAY handle, int channel, const G2SERVICE_SEARCH_OPTION_DEBUG_LOG* option);
G2_DLLFUNC bool G2API g2_play_request_debug_log_search_stop(G2HPLAY handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_play_get_camera_list(G2HPLAY handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_play_get_camera_list_interest(G2HPLAY handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC int  G2API g2_play_get_play_speed(G2HPLAY handle, int channel);
G2_DLLFUNC int  G2API g2_play_get_play_control_command(G2HPLAY handle, int channel);
G2_DLLFUNC int  G2API g2_play_get_current_command(G2HPLAY handle, int channel);
G2_DLLFUNC bool G2API g2_play_is_stopped(G2HPLAY handle, int channel);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_PLAY_H_
