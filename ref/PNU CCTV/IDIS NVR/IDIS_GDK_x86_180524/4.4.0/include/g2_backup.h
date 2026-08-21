// g2_backup.h : header file
//

#ifndef _G2_CLIENT_DLL_BACKUP_H_
#define _G2_CLIENT_DLL_BACKUP_H_

#if _MSC_VER > 1000
#pragma  once
#endif // _MSC_VER > 1000

#include "g2_define_backup.h"
#include "g2_define_search.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_backup_register_callback(G2HBACKUP handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HBACKUP G2API g2_backup_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_backup_finalize(G2HBACKUP handle);
G2_DLLFUNC void G2API g2_backup_startup(G2HBACKUP handle, int connections);
G2_DLLFUNC void G2API g2_backup_cleanup(G2HBACKUP handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_backup_connect(G2HBACKUP handle, const G2GUID* service, const G2GUID* site, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_backup_disconnect(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_is_connecting(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_is_connected(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_is_disconnecting(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_is_disconnected(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_is_disconnectable(G2HBACKUP handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_backup_set_camera_list(G2HBACKUP handle, int channel, const G2CHANNEL_SET* channels, const G2ROLLBACK_INFO* rbi, bool prepare_rollback, bool* preparing);
G2_DLLFUNC bool G2API g2_backup_set_camera_list_interest(G2HBACKUP handle, int channel, const G2CHANNEL_SET* channels);
G2_DLLFUNC void G2API g2_backup_set_play_control_command(G2HBACKUP handle, int channel, int command);
G2_DLLFUNC void G2API g2_backup_set_event_query_mode(G2HBACKUP handle, int channel, int mode);
G2_DLLFUNC void G2API g2_backup_set_probe_session_profile(G2HBACKUP handle, bool active);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_backup_request_service_info(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_alive_check(G2HBACKUP handle, int channel, int check);
G2_DLLFUNC bool G2API g2_backup_request_backup_site(G2HBACKUP handle, int channel, const G2GUID* site);
G2_DLLFUNC bool G2API g2_backup_request_record_channels(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_record_channels_each(G2HBACKUP handle, int channel, const G2GUID camera[], unsigned int count);
G2_DLLFUNC bool G2API g2_backup_request_record_time_info(G2HBACKUP handle, int channel, int resolution, int direction, const G2SCOPE* scope, int count, int command);
G2_DLLFUNC bool G2API g2_backup_request_record_time_info_load_stop(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_query_no_recorded_data(G2HBACKUP handle, int channel, const G2GUID camera[], unsigned int count);
G2_DLLFUNC bool G2API g2_backup_request_reload_current(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_play(G2HBACKUP handle, int channel, const G2PLAYBACK_COMMAND* command);
G2_DLLFUNC bool G2API g2_backup_request_pause(G2HBACKUP handle, int channel, bool rollback, const G2ROLLBACK_INFO* rbi);
G2_DLLFUNC bool G2API g2_backup_request_move_to_first(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_move_to_last(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_move_to_spot(G2HBACKUP handle, int channel, const G2SPOT* spot, int precision, bool forward);
G2_DLLFUNC bool G2API g2_backup_request_prev_step(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_next_step(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_notify_end_of_play(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_event_log_search(G2HBACKUP handle, int channel, const G2SERVICE_SEARCH_OPTION_EVENT_LOG* option);
G2_DLLFUNC bool G2API g2_backup_request_event_log_search_stop(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_text_in_log_search(G2HBACKUP handle, int channel, const G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG* option);
G2_DLLFUNC bool G2API g2_backup_request_text_in_log_search_next(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_text_in_log_search_stop(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_text_in_search(G2HBACKUP handle, int channel, const G2TEXT_IN_QUERY_CONDITION* option);
G2_DLLFUNC bool G2API g2_backup_request_text_in_search_stop(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_system_log_search(G2HBACKUP handle, int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG* option);
G2_DLLFUNC bool G2API g2_backup_request_system_log_search_stop(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_debug_log_search(G2HBACKUP handle, int channel, const G2SERVICE_SEARCH_OPTION_DEBUG_LOG* option);
G2_DLLFUNC bool G2API g2_backup_request_debug_log_search_stop(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_request_scope_list(G2HBACKUP handle, int channel, const G2TIME* from, const G2TIME* to, const G2CHANNEL_SET* channels, int type);
G2_DLLFUNC bool G2API g2_backup_request_spot_list(G2HBACKUP handle, int channel, const G2TIME* time, const G2CHANNEL_SET* channels);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_backup_get_camera_list(G2HBACKUP handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_backup_get_camera_list_interest(G2HBACKUP handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC int  G2API g2_backup_get_play_speed(G2HBACKUP handle, int channel);
G2_DLLFUNC int  G2API g2_backup_get_play_control_command(G2HBACKUP handle, int channel);
G2_DLLFUNC int  G2API g2_backup_get_current_command(G2HBACKUP handle, int channel);
G2_DLLFUNC int  G2API g2_backup_get_event_query_mode(G2HBACKUP handle, int channel);
G2_DLLFUNC bool G2API g2_backup_is_stopped(G2HBACKUP handle, int channel);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_BACKUP_H_
