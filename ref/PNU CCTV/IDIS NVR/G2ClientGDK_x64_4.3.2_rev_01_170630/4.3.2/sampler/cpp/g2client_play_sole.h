// g2client_play.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_PLAY_H_
#define _G2_CLIENT_DLL_SAMPLER_PLAY_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_play.h>
#include <vector>
#include <set>
#include <map>

namespace client {
    class g2play_listener_sole;

//////////////////////////////////////////////////////////////////////////

class g2play_sole
{
public:
    g2play_sole(void);
    virtual ~g2play_sole(void);

private:
    G2HPLAY_SOLE _handle;
    g2play_listener_sole* _listener;

public:
    void startup(int connections);
    void cleanup(void);
    void set_listener(g2play_listener_sole* listener);

    G2HPLAY_SOLE safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    bool connect(const G2GUID& site, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    bool disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool set_camera_list(int channel, const G2SEARCH_G2_SOLE_CHANNEL_MAP& chs);

public:
    bool request_search_base_cleanup(int channel, int tag);
    bool request_record_channel(int channel, int tag, const G2GUID& camera);
    bool request_record_time_info(int channel, int tag, int id, int resolution, const G2CHANNEL_SET& chs, const G2SCOPE& scope, int direction);
    bool request_record_time_info_load_stop(int channel, int tag);
    bool request_set_player_scope(int channel, int tag, const G2SCOPE& scope);
    bool request_play(int channel, int tag, int speed);
    bool request_play(int channel, int tag, const G2PLAYBACK_COMMAND& command);
    bool request_play_audio(int channel, int tag, int audio);
    bool request_pause(int channel, int tag, const G2ROLLBACK_INFO& rbi, bool rollback);
    bool request_stop(int channel, int tag);
    bool request_move_to_first(int channel, int tag);
    bool request_move_to_last(int channel, int tag);
    bool request_move_to_spot(int channel, int tag, const G2SPOT& spot, int precision, bool forward, bool discard);
    bool request_prev_step(int channel, int tag);
    bool request_next_step(int channel, int tag);
    bool request_notify_end_of_play(int channel, int tag);
    bool request_snapshot(int channel, int tag, const G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION_LIST& options);
    bool reuqest_snapshot_cancel(int channel, int tag);
    bool request_frame_channelset(int channel, int tag, int id);
    bool request_spot_list(int channel, int tag, int id, int play_channel, const G2TIME& time, bool adjacent);
    bool request_scope_list(int channel, int tag, int id, int play_channel, const G2TIME& from, const G2TIME& to);
    bool request_recorded_scope(int channel, int tag, int id, int play_channel);
    bool request_frame_spot_list(int channel, int tag, int id, int play_channel, const G2SCOPE& scope, bool fcontinue);
    
public:
    bool get_frame_from(int* from);
    bool is_player_stopped(int channel, int tag);
    bool is_player_stopped(int channel);
    bool is_started();
    bool channel_from_site(const G2GUID site, int* channel);

protected:
    static G2RESULT G2CALLBACK on_get_options_search_base(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_get_options_player(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_connected(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_probe_frameload(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_command_begin(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_command_end(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_play_speed_changed(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_frame_loaded(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_frame_not_found(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_out_of_scope(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_player_set_scope(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_player_error(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_get_rollback_info(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_get_frame_buf_status(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_snapshot_frame(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_snapshot_begin(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_snapshot_end(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_record_channel(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_record_time_info_loaded(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_record_time_info_load_end(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_frame_channelset(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_spot_list(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_scope_list(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_recorded_scope(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_frame_spot_list(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_PLAY_H_
