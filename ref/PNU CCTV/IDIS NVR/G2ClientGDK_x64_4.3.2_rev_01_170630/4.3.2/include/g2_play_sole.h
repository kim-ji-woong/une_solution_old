// g2_play_sole.h : header file
//

#ifndef _G2_CLIENT_DLL_PLAY_SOLE_H_
#define _G2_CLIENT_DLL_PLAY_SOLE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_play_sole.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_play_sole_register_callback(G2HPLAY_SOLE handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HPLAY_SOLE G2API g2_play_sole_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_play_sole_finalize(G2HPLAY_SOLE handle);
G2_DLLFUNC bool G2API g2_play_sole_startup(G2HPLAY_SOLE handle, int connections);
G2_DLLFUNC void G2API g2_play_sole_cleanup(G2HPLAY_SOLE handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_play_sole_connect(G2HPLAY_SOLE handle, const G2GUID* site, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC bool G2API g2_play_sole_disconnect_with_channel(G2HPLAY_SOLE handle, int channel);
G2_DLLFUNC bool G2API g2_play_sole_disconnect(G2HPLAY_SOLE handle);
G2_DLLFUNC bool G2API g2_play_sole_is_connecting(G2HPLAY_SOLE handle, int channel);
G2_DLLFUNC bool G2API g2_play_sole_is_connected(G2HPLAY_SOLE handle, int channel);
G2_DLLFUNC bool G2API g2_play_sole_is_disconnecting(G2HPLAY_SOLE handle, int channel);
G2_DLLFUNC bool G2API g2_play_sole_is_disconnected(G2HPLAY_SOLE handle, int channel);
G2_DLLFUNC bool G2API g2_play_sole_is_disconnected_not_any(G2HPLAY_SOLE handle);
G2_DLLFUNC bool G2API g2_play_sole_is_disconnectable(G2HPLAY_SOLE handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_play_sole_request_search_base_cleanup(G2HPLAY_SOLE handle, int channel, int tag);
G2_DLLFUNC bool G2API g2_play_sole_request_record_channel(G2HPLAY_SOLE handle, int channel, int tag, const G2GUID* camera);
G2_DLLFUNC bool G2API g2_play_sole_request_record_time_info(G2HPLAY_SOLE handle, int channel, int tag, int id, int resolution, const G2CHANNEL_SET* chs, const G2SCOPE* scope, int direction);
G2_DLLFUNC bool G2API g2_play_sole_request_record_time_info_load_stop(G2HPLAY_SOLE handle, int channel, int tag);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_play_sole_request_set_player_scope(G2HPLAY_SOLE handle, int channel, int tag, const G2SCOPE* scope);
G2_DLLFUNC bool G2API g2_play_sole_request_set_camera_list(G2HPLAY_SOLE handle, int channel, const G2SEARCH_G2_SOLE_CHANNEL_MAP* chs);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_play_sole_request_play(G2HPLAY_SOLE handle, int channel, int tag, int speed);
G2_DLLFUNC bool G2API g2_play_sole_request_play_with_command(G2HPLAY_SOLE handle, int channel, int tag, const G2PLAYBACK_COMMAND* command);
G2_DLLFUNC bool G2API g2_play_sole_request_play_audio(G2HPLAY_SOLE handle, int channel, int tag, int audio);
G2_DLLFUNC bool G2API g2_play_sole_request_pause(G2HPLAY_SOLE handle, int channel, int tag, const G2ROLLBACK_INFO* rbi, bool rollback);
G2_DLLFUNC bool G2API g2_play_sole_request_stop(G2HPLAY_SOLE handle, int channel, int tag);
G2_DLLFUNC bool G2API g2_play_sole_request_move_to_first(G2HPLAY_SOLE handle, int channel, int tag);
G2_DLLFUNC bool G2API g2_play_sole_request_move_to_last(G2HPLAY_SOLE handle, int channel, int tag);
G2_DLLFUNC bool G2API g2_play_sole_request_move_to_spot(G2HPLAY_SOLE handle, int channel, int tag, const G2SPOT* spot, int precision, bool forward, bool discard_if_duplcated_command);
G2_DLLFUNC bool G2API g2_play_sole_request_step_prev(G2HPLAY_SOLE handle, int channel, int tag);
G2_DLLFUNC bool G2API g2_play_sole_request_step_next(G2HPLAY_SOLE handle, int channel, int tag);
G2_DLLFUNC bool G2API g2_play_sole_request_notify_end_of_play(G2HPLAY_SOLE handle, int channel, int tag);
G2_DLLFUNC bool G2API g2_play_sole_request_snapshot(G2HPLAY_SOLE handle, int channel, int tag, const G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION_LIST* options);
G2_DLLFUNC bool G2API g2_play_sole_request_snapshot_cancel(G2HPLAY_SOLE handle, int channel, int tag);
G2_DLLFUNC bool G2API g2_play_sole_request_frame_channelset(G2HPLAY_SOLE handle, int channel, int tag, int id);
G2_DLLFUNC bool G2API g2_play_sole_request_spot_list(G2HPLAY_SOLE handle, int channel, int tag, int id, int play_channel, const G2TIME* time, bool load_adjacent_frame);
G2_DLLFUNC bool G2API g2_play_sole_request_scope_list(G2HPLAY_SOLE handle, int channel, int tag, int id, int play_channel, const G2TIME* from, const G2TIME* to);
G2_DLLFUNC bool G2API g2_play_sole_request_recorded_scope(G2HPLAY_SOLE handle, int channel, int tag, int id, int play_channel);
G2_DLLFUNC bool G2API g2_play_sole_request_frame_spot_list(G2HPLAY_SOLE handle, int channel, int tag, int id, int play_channel, const G2SCOPE* scope, const bool* fcontinue);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_play_sole_get_frame_from(G2HPLAY_SOLE handle, int* from);
G2_DLLFUNC bool G2API g2_play_sole_is_player_stopped_with_tag(G2HPLAY_SOLE handle, int channel, int tag);
G2_DLLFUNC bool G2API g2_play_sole_is_player_stopped(G2HPLAY_SOLE handle, int channel);
G2_DLLFUNC bool G2API g2_play_sole_is_started(G2HPLAY_SOLE handle);
G2_DLLFUNC bool G2API g2_play_sole_channel_from_site(G2HPLAY_SOLE handle, const G2GUID* site, int* channel);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_PLAY_SOLE_H_
