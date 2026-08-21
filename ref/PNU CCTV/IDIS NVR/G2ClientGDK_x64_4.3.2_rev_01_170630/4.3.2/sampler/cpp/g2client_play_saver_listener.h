// g2client_play_saver_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_PLAY_SAVER_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_PLAY_SAVER_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_play_saver.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2play_saver_listener
{
public:
    virtual void on_g2play_saver_connected(G2HPLAY_SAVER handle, int channel) = 0;
    virtual void on_g2play_saver_disconnected(G2HPLAY_SAVER handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2play_saver_receive_record_channels(G2HPLAY_SAVER handle, int channel, const std::vector<G2PLAY_CHANNEL_INFO>& channels) = 0;
    virtual void on_g2play_saver_receive_frame_data(G2HPLAY_SAVER handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2play_saver_receive_notify_out_of_scope(G2HPLAY_SAVER handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE status) = 0;
    virtual void on_g2play_saver_receive_notify_player_error(G2HPLAY_SAVER handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE error) = 0;
    virtual void on_g2play_saver_receive_scope_list(G2HPLAY_SAVER handle, int channel, const std::vector<G2SCOPE>& scopes) = 0;
    virtual void on_g2play_saver_receive_no_recorded_data(G2HPLAY_SAVER handle, int channel) = 0;
    virtual void on_g2play_saver_receive_clipcopy_size(G2HPLAY_SAVER handle, int channel, G2CLIPCOPY_STATUS::TYPE status, const G2CLIPCOPY_SIZE_INFO& csi) = 0;
    virtual void on_g2play_saver_receive_clipcopy_data(G2HPLAY_SAVER handle, int channel, unsigned __int64 offset, unsigned int size, const unsigned char* data, unsigned int progress) = 0;
    virtual void on_g2play_saver_receive_clipcopy_canceled(G2HPLAY_SAVER handle, int channel) = 0;
    virtual void on_g2play_saver_receive_clipcopy_set_password(G2HPLAY_SAVER handle, int channel, unsigned int result) = 0;
    virtual void on_g2play_saver_receive_clipcopy_job_started(G2HPLAY_SAVER handle, int channel, G2CLIPCOPY_JOB::TYPE job) = 0;
    virtual void on_g2play_saver_receive_clipcopy_job_finished(G2HPLAY_SAVER handle, int channel, G2CLIPCOPY_JOB::TYPE job) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_PLAY_SAVER_LISTENER_H_
