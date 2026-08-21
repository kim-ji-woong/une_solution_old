// g2_search.h : header file
//

#ifndef _G2_CLIENT_DLL_SEARCH_H_
#define _G2_CLIENT_DLL_SEARCH_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_search.h"
#include "g2_define_play.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_search_register_callback(G2HSEARCH handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HSEARCH G2API g2_search_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_search_finalize(G2HSEARCH handle);
G2_DLLFUNC void G2API g2_search_startup(G2HSEARCH handle, int connections);
G2_DLLFUNC void G2API g2_search_cleanup(G2HSEARCH handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_search_connect(G2HSEARCH handle, const G2GUID* root, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC int  G2API g2_search_connect_ras(G2HSEARCH handle, const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_search_disconnect(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_is_connecting(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_is_connected(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_is_disconnecting(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_is_disconnected(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_is_disconnectable(G2HSEARCH handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_search_set_invoke_saver(G2HSEARCH handle, int channel);
G2_DLLFUNC void G2API g2_search_set_revoke_saver(G2HSEARCH handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_set_camera_list(G2HSEARCH handle, int channel, unsigned int cameras);
G2_DLLFUNC bool G2API g2_search_set_search_target(G2HSEARCH handle, int channel, int target);
G2_DLLFUNC bool G2API g2_search_set_change_segment(G2HSEARCH handle, int channel, int id);
G2_DLLFUNC bool G2API g2_search_set_change_search_mode(G2HSEARCH handle, int channel, bool event_search);
G2_DLLFUNC bool G2API g2_search_set_external_tango(G2HSEARCH handle, int channel, int type, int number);
G2_DLLFUNC void G2API g2_search_set_query_mode(G2HSEARCH handle, int channel, int mode);
G2_DLLFUNC void G2API g2_search_set_query_cameras(G2HSEARCH handle, int channel, unsigned int cameras);
G2_DLLFUNC void G2API g2_search_set_invoke_on_play(G2HSEARCH handle, int channel, const G2SPOT* spot);
G2_DLLFUNC void G2API g2_search_set_revoke_on_play(G2HSEARCH handle, int channel);
G2_DLLFUNC void G2API g2_search_set_probe_session_profile(G2HSEARCH handle, bool active);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_request_record_date(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_request_record_time(G2HSEARCH handle, int channel, const G2TIME* date);
G2_DLLFUNC bool G2API g2_search_request_record_hour(G2HSEARCH handle, int channel, const G2SPOT* spot, int length, bool direction, bool check_diff_prev_req);
G2_DLLFUNC bool G2API g2_search_request_record_hour_command(G2HSEARCH handle, int channel, int command, const G2SPOT* spot);
G2_DLLFUNC bool G2API g2_search_request_playback(G2HSEARCH handle, int channel, int command, const G2SPOT* spot);
G2_DLLFUNC bool G2API g2_search_request_pause(G2HSEARCH handle, int channel, const G2ROLLBACK_INFO* rbi);
G2_DLLFUNC bool G2API g2_search_request_reload_current(G2HSEARCH handle, int channel, int camera, const G2SPOT* spot);
G2_DLLFUNC bool G2API g2_search_request_notify_end_of_play(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_request_query_specific_event(G2HSEARCH handle, int channel, int seq_number, const G2TIME* begin, const G2TIME* end, int event_type, int camera);
G2_DLLFUNC bool G2API g2_search_request_query_event(G2HSEARCH handle, int channel, const G2EVENT_QUERY_CONDITION* condition);
G2_DLLFUNC bool G2API g2_search_request_query_text_in(G2HSEARCH handle, int channel, const G2TEXT_IN_QUERY_CONDITION* condition);
G2_DLLFUNC bool G2API g2_search_request_event_image(G2HSEARCH handle, int channel, int selected, bool last);
G2_DLLFUNC bool G2API g2_search_request_event_search_stop_idr(G2HSEARCH handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_request_clipcopy_enable_channelset(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_request_clipcopy_scope(G2HSEARCH handle, int channel, const G2TIME* from, const G2TIME* to, unsigned int cameras);
G2_DLLFUNC bool G2API g2_search_request_clipcopy_measure_size(G2HSEARCH handle, int channel, const G2SCOPE* scope, unsigned __int64 free_space, bool slice);
G2_DLLFUNC bool G2API g2_search_request_clipcopy_size(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_request_clipcopy(G2HSEARCH handle, int channel, bool start);
G2_DLLFUNC bool G2API g2_search_request_clipcopy_password(G2HSEARCH handle, int channel, bool use, const G2STRING_32* password);
G2_DLLFUNC bool G2API g2_search_request_clipcopy_text_in(G2HSEARCH handle, int channel, bool include);
G2_DLLFUNC bool G2API g2_search_request_clipcopy_gps_data(G2HSEARCH handle, int channel, bool include);
G2_DLLFUNC bool G2API g2_search_request_clipcopy_structure(G2HSEARCH handle, int channel, int version, bool exclude_player, bool avoid_seek);
G2_DLLFUNC bool G2API g2_search_request_clipcopy_cancel(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_request_mini_bank_begin(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_request_mini_bank_end(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_request_mini_bank_space(G2HSEARCH handle, int channel, const G2TIME* from, const G2TIME* to, unsigned int cameras, bool audio);
G2_DLLFUNC bool G2API g2_search_request_mini_bank(G2HSEARCH handle, int channel, int command, const G2TIME* time, int msec);
G2_DLLFUNC bool G2API g2_search_request_gps_data_measure(G2HSEARCH handle, int channel, const G2TIME* from, const G2TIME* to);
G2_DLLFUNC bool G2API g2_search_request_gps_data_measure_result(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_request_gps_data(G2HSEARCH handle, int channel, const G2TIME* time);
G2_DLLFUNC bool G2API g2_search_request_gps_data_next(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_request_gps_data_export_cancel(G2HSEARCH handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_get_adaptor(G2HSEARCH handle, void* ptr);
G2_DLLFUNC bool G2API g2_search_get_server_network_info(G2HSEARCH handle, int channel, G2SERVER_NETWORK_INFO* ni);
G2_DLLFUNC bool G2API g2_search_get_product_info(G2HSEARCH handle, int channel, G2_PRODUCT_INFO* pi);
G2_DLLFUNC bool G2API g2_search_get_remote_search_caps(G2HSEARCH handle, int channel, G2_PRODUCT_INFO_CAPS::REMOTE_SEARCH* caps);
G2_DLLFUNC bool G2API g2_search_get_remote_clipcopy_caps(G2HSEARCH handle, int channel, G2_PRODUCT_INFO_CAPS::REMOTE_CLIP_COPY* caps);
G2_DLLFUNC bool G2API g2_search_get_text_in_search_caps(G2HSEARCH handle, int channel, G2_PRODUCT_INFO_CAPS::TEXT_IN_SEARCH* caps);
G2_DLLFUNC bool G2API g2_search_get_authority(G2HSEARCH handle, int channel, G2RAS_AUTHORITY* auth);
G2_DLLFUNC bool G2API g2_search_get_camera_list(G2HSEARCH handle, int channel, unsigned int* cameras);
G2_DLLFUNC bool G2API g2_search_get_time_last(G2HSEARCH handle, int channel, G2TIME* time);
G2_DLLFUNC int  G2API g2_search_get_drive_mode(G2HSEARCH handle, int channel);
G2_DLLFUNC int  G2API g2_search_get_query_mode(G2HSEARCH handle, int channel);
G2_DLLFUNC int  G2API g2_search_get_timelapse_mode(G2HSEARCH handle, int channel);
G2_DLLFUNC int  G2API g2_search_get_play_speed(G2HSEARCH handle, int channel);
G2_DLLFUNC int  G2API g2_search_get_current_command(G2HSEARCH handle, int channel);
G2_DLLFUNC int  G2API g2_search_get_search_target(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_get_query_cameras(G2HSEARCH handle, int channel, unsigned int* cameras);
G2_DLLFUNC bool G2API g2_search_get_query_condition_event(G2HSEARCH handle, int channel, G2EVENT_QUERY_CONDITION* condition);
G2_DLLFUNC bool G2API g2_search_get_query_condition_text_in(G2HSEARCH handle, int channel, G2TEXT_IN_QUERY_CONDITION* condition);
G2_DLLFUNC bool G2API g2_search_get_query_result_text_in(G2HSEARCH handle, int channel, int count, G2TEXT_IN data[]);
G2_DLLFUNC bool G2API g2_search_get_query_result_event(G2HSEARCH handle, int channel, int selected, G2SEARCH_LOG_INFO* data);
G2_DLLFUNC bool G2API g2_search_get_clipcopy_size_info(G2HSEARCH handle, int channel, G2CLIPCOPY_SIZE_INFO* info);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_search_is_rec_info_type(G2HSEARCH handle, int channel, int type);
G2_DLLFUNC bool G2API g2_search_is_drive_mode(G2HSEARCH handle, int channel, int mode);
G2_DLLFUNC bool G2API g2_search_is_query_mode(G2HSEARCH handle, int channel, int mode);
G2_DLLFUNC bool G2API g2_search_is_timelapse_mode(G2HSEARCH handle, int channel, int mode);
G2_DLLFUNC bool G2API g2_search_is_event_search_mode(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_is_stopped(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_is_loading(G2HSEARCH handle, int channel);
G2_DLLFUNC bool G2API g2_search_is_support(G2HSEARCH handle, int channel, int query);
G2_DLLFUNC bool G2API g2_search_is_authority(G2HSEARCH handle, int channel, int authority);
G2_DLLFUNC bool G2API g2_search_is_probe_session_profile(G2HSEARCH handle);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_SEARCH_H_
