// g2client_search_g2_listener_saver.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_SEARCH_G2_LISTENER_SAVER_H_
#define _G2_CLIENT_DLL_SAMPLER_SEARCH_G2_LISTENER_SAVER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_search_g2.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2search_g2_listener_saver
{
public:
    virtual void on_g2search_g2_saver_connected(G2HSEARCH_G2 handle, int channel) = 0;
    virtual void on_g2search_g2_saver_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2search_g2_saver_receive_frame_data(G2HSEARCH_G2 handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2search_g2_saver_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE status) = 0;
    virtual void on_g2search_g2_saver_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, const G2ROLLBACK_INFO& rbi) = 0;
    virtual void on_g2search_g2_saver_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE error) = 0;
    virtual void on_g2search_g2_saver_receive_scope_list(G2HSEARCH_G2 handle, int channel, const std::vector<G2SCOPE>& scopes, G2SEARCH_G2_SCOPE_TYPE::TYPE type) = 0;
    virtual void on_g2search_g2_saver_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel) = 0;
    virtual void on_g2search_g2_saver_receive_clipcopy_size(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_STATUS::TYPE status, const G2CLIPCOPY_SIZE_INFO& csi, unsigned int progress) = 0;
    virtual void on_g2search_g2_saver_receive_clipcopy_data(G2HSEARCH_G2 handle, int channel, unsigned __int64 offset, unsigned int size, const unsigned char* data, unsigned int progress) = 0;
    virtual void on_g2search_g2_saver_receive_clipcopy_set_password(G2HSEARCH_G2 handle, int channel, unsigned int result) = 0;
    virtual void on_g2search_g2_saver_receive_clipcopy_canceled(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_ERROR::TYPE error) = 0;
    virtual void on_g2search_g2_saver_receive_clipcopy_job_started(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB::TYPE job, unsigned int num, unsigned int total) = 0;
    virtual void on_g2search_g2_saver_receive_clipcopy_job_finished(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB::TYPE job, unsigned int num, unsigned int total) = 0;
    virtual void on_g2search_g2_saver_receive_clipcopy_section_begin(G2HSEARCH_G2 handle, int channel, unsigned int num, unsigned int total) = 0;
    virtual void on_g2search_g2_saver_receive_clipcopy_section_end(G2HSEARCH_G2 handle, int channel, unsigned int num, unsigned int total) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_SEARCH_G2_LISTENER_SAVER_H_
