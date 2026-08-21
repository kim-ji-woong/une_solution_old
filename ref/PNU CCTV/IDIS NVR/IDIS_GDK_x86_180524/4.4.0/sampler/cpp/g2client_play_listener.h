// g2client_play_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_PLAY_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_PLAY_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_play.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2play_listener
{
public:
    virtual void on_g2play_connected(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_disconnected(G2HPLAY handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2play_receive_hard_disk_info(G2HPLAY handle, int channel, const std::vector<G2PLAY_HARD_DISK_INFO>& dis) {}
    virtual void on_g2play_receive_record_channels(G2HPLAY handle, int channel, const std::vector<G2PLAY_CHANNEL_INFO>& channels) = 0;
    virtual void on_g2play_receive_record_time_info_load(G2HPLAY handle, int channel, const G2RECORD_TIME_INFO& rti) = 0;
    virtual void on_g2play_receive_record_time_info_load_end(G2HPLAY handle, int channel, G2RECORD_TIME_INFO::RESOLUTION resolution, G2RECORD_TIME_INFO::COMMAND command) = 0;
    virtual void on_g2play_receive_frame_data(G2HPLAY handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2play_receive_text_in(G2HPLAY handle, int channel, const G2TEXT_IN& in) = 0;
    virtual void on_g2play_receive_notify_command_begin(G2HPLAY handle, int channel, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2play_receive_notify_command_end(G2HPLAY handle, int channel, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2play_receive_notify_play_speed_changed(G2HPLAY handle, int channel, G2PLAYER::COMMAND_AND_SPEED speed) = 0;
    virtual void on_g2play_receive_notify_frame_not_found(G2HPLAY handle, int channel, const G2SPOT& spot, G2PLAYER::PRECISION::TYPE precision) = 0;
    virtual void on_g2play_receive_notify_out_of_scope(G2HPLAY handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE playtype) = 0;
    virtual void on_g2play_receive_notify_player_error(G2HPLAY handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE errorcode) = 0;
    virtual void on_g2play_receive_scope_list(G2HPLAY handle, int channel, const std::vector<G2SCOPE>& scopes, G2PLAY_SCOPE_TYPE::TYPE type) = 0;
    virtual void on_g2play_receive_spot_list(G2HPLAY handle, int channel, const std::vector<G2SPOT>& spots) = 0;
    virtual void on_g2play_receive_no_recorded_data(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_record_failover_scope(G2HPLAY handle, int channel, const std::vector<G2FAILOVER_SCOPE_INFO>& infos) = 0;
    virtual void on_g2play_receive_event_log_load(G2HPLAY handle, int channel, const G2EVENT_LOG& log) = 0;
    virtual void on_g2play_receive_event_log_load_end(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_event_log_load_fail(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_event_log_load_stop(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_text_in_log_load(G2HPLAY handle, int channel, const G2EVENT& log) = 0;
    virtual void on_g2play_receive_text_in_log_load_end(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_text_in_log_load_fail(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_text_in_log_load_stop(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_device_log_load(G2HPLAY handle, int channel, const G2SYSTEM_LOG& log) = 0;
    virtual void on_g2play_receive_device_log_load_end(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_device_log_load_fail(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_device_log_load_stop(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_service_log_load(G2HPLAY handle, int channel, const G2SYSTEM_LOG& log) = 0;
    virtual void on_g2play_receive_service_log_load_end(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_service_log_load_fail(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_receive_service_log_load_stop(G2HPLAY handle, int channel) = 0;
    virtual void on_g2play_require_prepare_rollback(G2HPLAY handle, int channel, bool prepare) = 0;
    virtual void on_g2play_probe_session_profile(G2HPLAY handle, int channel, const G2PROBE_SESSION_PROFILE& probe) = 0;
	virtual void on_g2play_get_frame_buf_status(G2HPLAY handle, int channel, int tag, int* avail) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_PLAY_LISTENER_H_
