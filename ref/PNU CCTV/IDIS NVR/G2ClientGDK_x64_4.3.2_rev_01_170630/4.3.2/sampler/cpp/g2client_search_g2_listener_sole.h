// g2client_search_g2_listener_sole.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_SEARCH_G2_LISTENER_SOLE_H_
#define _G2_CLIENT_DLL_SAMPLER_SEARCH_G2_LISTENER_SOLE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_search_g2.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2search_g2_listener_sole
{
public:
    virtual void on_g2search_g2_sole_connected(G2HSEARCH_G2 handle, int channel) = 0;
    virtual void on_g2search_g2_sole_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2search_g2_sole_query_options_player(G2HSEARCH_G2 handle, int channel, int camera, G2SEARCH_G2_OPTIONS_PLAYER& options) = 0;
    virtual void on_g2search_g2_sole_receive_record_time_info_load(G2HSEARCH_G2 handle, int channel, int camera, const G2RECORD_TIME_INFO& rti) = 0;
    virtual void on_g2search_g2_sole_receive_record_time_info_load_end(G2HSEARCH_G2 handle, int channel, int camera, G2RECORD_TIME_INFO::RESOLUTION resolution) = 0;
    virtual void on_g2search_g2_sole_receive_frame_data(G2HSEARCH_G2 handle, int channel, int camera, const G2FRAME& frame) = 0;
    virtual void on_g2search_g2_sole_receive_text_in(G2HSEARCH_G2 handle, int channel, int camera, const G2EVENT& in) = 0;
    virtual void on_g2search_g2_sole_receive_event(G2HSEARCH_G2 handle, int channel, int camera, const G2EVENT& evt) = 0;
    virtual void on_g2search_g2_sole_receive_notify_command_begin(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2search_g2_sole_receive_notify_command_end(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2search_g2_sole_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER::COMMAND_AND_SPEED speed) = 0;
    virtual void on_g2search_g2_sole_receive_notify_frame_not_found(G2HSEARCH_G2 handle, int channel, int camera, const G2SPOT& spot, G2PLAYER::PRECISION::TYPE precision) = 0;
    virtual void on_g2search_g2_sole_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER::OUT_OF_SCOPE::TYPE status) = 0;
    virtual void on_g2search_g2_sole_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, int camera, G2ROLLBACK_INFO& rbi) = 0;
    virtual void on_g2search_g2_sole_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER::PLAYER_ERROR::TYPE error) = 0;
    virtual void on_g2search_g2_sole_receive_scope_list(G2HSEARCH_G2 handle, int channel, int camera, const std::vector<G2SCOPE>& scopes, G2SEARCH_G2_SCOPE_TYPE::TYPE type) = 0;
    virtual void on_g2search_g2_sole_receive_spot_list(G2HSEARCH_G2 handle, int channel, int camera, const std::vector<G2SPOT>& spots) = 0;
    virtual void on_g2search_g2_sole_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel, int camera) = 0;
    virtual void on_g2search_g2_sole_require_prepare_rollback(G2HSEARCH_G2 handle, int channel, int camera, bool prepare) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_SEARCH_G2_LISTENER_SOLE_H_
