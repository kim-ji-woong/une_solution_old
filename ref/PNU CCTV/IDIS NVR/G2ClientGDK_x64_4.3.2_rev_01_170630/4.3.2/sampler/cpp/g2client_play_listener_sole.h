// g2client_play_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_PLAY_LISTENER_SOLE_H_
#define _G2_CLIENT_DLL_SAMPLER_PLAY_LISTENER_SOLE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_play.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2play_listener_sole
{
public:
    virtual void on_g2play_sole_get_options_search_base(G2HPLAY_SOLE handle, int channel, G2SEARCH_G2_SOLE_BASE_OPTIONS* options) = 0;
    virtual void on_g2play_sole_get_options_player(G2HPLAY_SOLE handle, int channel, G2SEARCH_G2_SOLE_PLAYER_OPTIONS* options) = 0;
    virtual void on_g2play_sole_connected(G2HPLAY_SOLE handle, int channel) = 0;
    virtual void on_g2play_sole_disconnected(G2HPLAY_SOLE handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2play_sole_probe_frameload(G2HPLAY_SOLE handle, int channel, int ips, int bps) = 0;
    virtual void on_g2play_sole_receive_notify_command_begin(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2play_sole_receive_notify_command_end(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2play_sole_receive_notify_play_speed_changed(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER::COMMAND_AND_SPEED speed) = 0;
    virtual void on_g2play_sole_receive_frame_data(G2HPLAY_SOLE handle, int channel, int tag, const G2FRAME& frame) = 0;
    virtual void on_g2play_sole_receive_notify_frame_not_found(G2HPLAY_SOLE handle, int channel, int tag, const G2SPOT& spot, G2PLAYER::PRECISION::TYPE precision) = 0;
    virtual void on_g2play_sole_receive_notify_out_of_scope(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER::OUT_OF_SCOPE::TYPE type) = 0;
    virtual void on_g2play_sole_receive_notify_player_set_scope(G2HPLAY_SOLE handle, int channel, int tag, const G2SCOPE& scope) = 0;
    virtual void on_g2play_sole_receive_notify_player_error(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER::PLAYER_ERROR::TYPE type) = 0;
    virtual void on_g2play_sole_require_prepare_rollback(G2HPLAY_SOLE handle, int channel, int tag, const G2ROLLBACK_INFO& rbi) = 0;
    virtual void on_g2play_sole_get_frame_buf_status(G2HPLAY_SOLE handle, int channel, int tag, int* avail) = 0;
    virtual void on_g2play_sole_receive_text_in(G2HPLAY_SOLE handle, int channel, int tag, const G2TEXT_IN& ei) = 0;
    virtual void on_g2play_sole_receive_snapshot_frame(G2HPLAY_SOLE handle, int channel, int tag, int id, const G2FRAME& frame) = 0;
    virtual void on_g2play_sole_receive_snapshot_begin(G2HPLAY_SOLE handle, int channel, int tag, int id) = 0;
    virtual void on_g2play_sole_receive_snapshot_end(G2HPLAY_SOLE handle, int channel, int tag, int id, G2SEARCH_G2_SOLE_SNAPSHOP_LOADER::FINISH_REASON reason) = 0;
    virtual void on_g2play_sole_receive_record_channel(G2HPLAY_SOLE handle, int channel, int tag, int ch) = 0;
    virtual void on_g2play_sole_receive_record_time_info_load(G2HPLAY_SOLE handle, int channel, int tag, int id, const G2RECORD_TIME_INFO& rti) = 0;
    virtual void on_g2play_sole_receive_record_time_info_load_end(G2HPLAY_SOLE handle, int channel, int tag, int id, int reason) = 0;
    virtual void on_g2play_sole_receive_frame_channelset(G2HPLAY_SOLE handle, int channel, int tag, int id, const G2CHANNEL_SET& chs) = 0;
    virtual void on_g2play_sole_receive_spot_list(G2HPLAY_SOLE handle, int channel, int tag, int id, const std::vector<G2SPOT>& spots) = 0;
    virtual void on_g2play_sole_receive_scope_list(G2HPLAY_SOLE handle, int channel, int tag, int id, const std::vector<G2SCOPE>& scopes) = 0;
    virtual void on_g2play_sole_receive_recorded_scope(G2HPLAY_SOLE handle, int channel, int tag, int id, const G2SEARCH_G2_SOLE_RECORDED_SCOPE& scope) = 0;
    virtual void on_g2play_sole_receive_frame_spot_list(G2HPLAY_SOLE handle, int channel, int tag, int id, const std::vector<G2SPOT>& spots) = 0;
    
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_PLAY_SAVER_LISTENER_SOLE_H_
