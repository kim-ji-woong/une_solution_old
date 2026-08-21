// g2client_search_listener_saver.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_SEARCH_LISTENER_SAVER_H_
#define _G2_CLIENT_DLL_SAMPLER_SEARCH_LISTENER_SAVER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_search.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2search_listener_saver
{
public:
    virtual void on_g2search_saver_connected(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_saver_disconnected(G2HSEARCH handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2search_saver_receive_recorded_date(G2HSEARCH handle, int channel, const std::vector<G2TIME>& dates) = 0;
    virtual void on_g2search_saver_receive_frame_data(G2HSEARCH handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2search_saver_receive_no_frame(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_saver_receive_no_recorded_data(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_saver_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, int speed) = 0;
    virtual void on_g2search_saver_receive_notify_play_stop_spot(G2HSEARCH handle, int channel, G2SEARCH_DRIVE::MODE mode) = 0;
    virtual void on_g2search_saver_receive_notify_end_of_play(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_saver_receive_notify_command_end(G2HSEARCH handle, int channel, int command) = 0;
    virtual void on_g2search_saver_receive_clipcopy_scope(G2HSEARCH handle, int channel, const std::vector<G2SCOPE>& scopes) = 0;
    virtual void on_g2search_saver_receive_clipcopy_cancel(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_saver_receive_clipcopy_measure_size(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_saver_receive_clipcopy_size(G2HSEARCH handle, int channel, G2CLIPCOPY_STATUS::TYPE status, unsigned __int64 size, const G2TIME& begin, const G2TIME& end) = 0;
    virtual void on_g2search_saver_receive_clipcopy_data(G2HSEARCH handle, int channel, int offset, int size, const unsigned char* data, int progress, bool completed) = 0;
    virtual void on_g2search_saver_receive_clipcopy_password(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_saver_receive_clipcopy_enable_channels(G2HSEARCH handle, int channel, unsigned int cameras) = 0;
    virtual void on_g2search_saver_receive_bank_space(G2HSEARCH handle, int channel, int image_index_num, int audio_index_num, const G2TIME& start, int start_msec, unsigned __int64 image_size, unsigned __int64 audio_size) = 0;
    virtual void on_g2search_saver_receive_bank_image(G2HSEARCH handle, int channel, const unsigned char* data) = 0;
    virtual void on_g2search_saver_receive_bank_audio(G2HSEARCH handle, int channel, const unsigned char* data) = 0;
    virtual void on_g2search_saver_receive_bank_no_image(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_saver_receive_bank_no_audio(G2HSEARCH handle, int channel) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_SEARCH_LISTENER_SAVER_H_
