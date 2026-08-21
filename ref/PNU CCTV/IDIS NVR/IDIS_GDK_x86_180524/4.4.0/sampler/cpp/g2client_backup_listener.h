// g2client_backup_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_BACKUP_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_BACKUP_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_backup.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2backup_listener
{
public:
    virtual void on_g2backup_connected(G2HBACKUP handle, int channel, const G2GUID& site) = 0;
    virtual void on_g2backup_disconnected(G2HBACKUP handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2backup_receive_backup_site_result(G2HBACKUP handle, int channel, const G2BACKUP_SITE_RESULT& result) = 0;
    virtual void on_g2backup_receive_record_channels(G2HBACKUP handle, int channel, const std::vector<G2BACKUP_CHANNEL_INFO>& channels) = 0;
    virtual void on_g2backup_receive_record_time_info_load(G2HBACKUP handle, int channel, const G2RECORD_TIME_INFO& rti) = 0;
    virtual void on_g2backup_receive_record_time_info_load_end(G2HBACKUP handle, int channel, G2RECORD_TIME_INFO::RESOLUTION resolution, G2RECORD_TIME_INFO::COMMAND command) = 0;
    virtual void on_g2backup_receive_response_no_recorded_data(G2HBACKUP handle, int channel, const std::vector<G2BACKUP_CHANNEL_INFO>& channels) = 0;
    virtual void on_g2backup_receive_frame_data(G2HBACKUP handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2backup_receive_text_in(G2HBACKUP handle, int channel, const G2TEXT_IN& in) = 0;
    virtual void on_g2backup_receive_notify_command_begin(G2HBACKUP handle, int channel, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2backup_receive_notify_command_end(G2HBACKUP handle, int channel, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2backup_receive_notify_play_speed_changed(G2HBACKUP handle, int channel, G2PLAYER::COMMAND_AND_SPEED speed) = 0;
    virtual void on_g2backup_receive_notify_frame_not_found(G2HBACKUP handle, int channel, G2SPOT& spot, G2PLAYER::PRECISION::TYPE precision) = 0;
    virtual void on_g2backup_receive_notify_out_of_scope(G2HBACKUP handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE playtype) = 0;
    virtual void on_g2backup_receive_notify_player_error(G2HBACKUP handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE errorcode) = 0;
    virtual void on_g2backup_receive_scope_list(G2HBACKUP handle, int channel,  const std::vector<G2SCOPE>& scopes, G2PLAY_SCOPE_TYPE::TYPE type) = 0;
    virtual void on_g2backup_receive_spot_list(G2HBACKUP handle, int channel, const std::vector<G2SPOT>& spots) = 0;
    virtual void on_g2backup_receive_no_recorded_data(G2HBACKUP handle, int channel) = 0;
    virtual void on_g2backup_receive_event_log_load(G2HPLAY handle, int channel, const G2EVENT_LOG& log) = 0;
    virtual void on_g2backup_receive_event_log_load_end(G2HPLAY handle, int channel) = 0;
    virtual void on_g2backup_receive_event_log_load_fail(G2HPLAY handle, int channel) = 0;
    virtual void on_g2backup_receive_event_log_load_stop(G2HPLAY handle, int channel) = 0;
    virtual void on_g2backup_receive_text_in_log_load(G2HPLAY handle, int channel, const G2EVENT& log) = 0;
    virtual void on_g2backup_receive_text_in_log_load_end(G2HPLAY handle, int channel) = 0;
    virtual void on_g2backup_receive_text_in_log_load_fail(G2HPLAY handle, int channel) = 0;
    virtual void on_g2backup_receive_text_in_log_load_stop(G2HPLAY handle, int channel) = 0;
    virtual void on_g2backup_receive_service_log_load(G2HBACKUP handle, int channel, const G2SYSTEM_LOG& log) = 0;
    virtual void on_g2backup_receive_service_log_load_end(G2HBACKUP handle, int channel) = 0;
    virtual void on_g2backup_receive_service_log_load_fail(G2HBACKUP handle, int channel) = 0;
    virtual void on_g2backup_receive_service_log_load_stop(G2HBACKUP handle, int channel) = 0;
    virtual void on_g2backup_require_prepare_rollback(G2HBACKUP handle, int channel, bool prepare) = 0;
    virtual void on_g2backup_probe_session_profile(G2HBACKUP handle, int channel, const G2PROBE_SESSION_PROFILE& probe) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_BACKUP_LISTENER_H_
