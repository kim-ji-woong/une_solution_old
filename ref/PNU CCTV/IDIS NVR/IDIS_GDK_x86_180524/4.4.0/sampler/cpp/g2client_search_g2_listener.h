// g2client_search_g2_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_SEARCH_G2_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_SEARCH_G2_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_search_g2.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2search_g2_listener
{
public:
    virtual void on_g2search_g2_connected(G2HSEARCH_G2 handle, int channel) = 0;
    virtual void on_g2search_g2_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2search_g2_query_options_search_base(G2HSEARCH_G2 handle, int channel, G2SEARCH_G2_OPTIONS_SEARCH_BASE& options) {}
    virtual void on_g2search_g2_query_options_player(G2HSEARCH_G2 handle, int channel, G2SEARCH_G2_OPTIONS_PLAYER& options) {}
    virtual void on_g2search_g2_receive_record_time_info_load(G2HSEARCH_G2 handle, int channel, const G2RECORD_TIME_INFO& rti) = 0;
    virtual void on_g2search_g2_receive_record_time_info_load_end(G2HSEARCH_G2 handle, int channel, G2RECORD_TIME_INFO::RESOLUTION resolution, G2RECORD_TIME_INFO::COMMAND command) = 0;
    virtual void on_g2search_g2_receive_frame_data(G2HSEARCH_G2 handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2search_g2_receive_text_in(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei) = 0;
    virtual void on_g2search_g2_receive_event(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei) = 0;
    virtual void on_g2search_g2_receive_notify_command_begin(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2search_g2_receive_notify_command_end(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command) = 0;
    virtual void on_g2search_g2_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED speed) = 0;
    virtual void on_g2search_g2_receive_notify_frame_not_found(G2HSEARCH_G2 handle, int channel, G2SPOT& spot, G2PLAYER::PRECISION::TYPE precision) = 0;
    virtual void on_g2search_g2_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE status) = 0;
    virtual void on_g2search_g2_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, G2ROLLBACK_INFO& rbi) = 0;
    virtual void on_g2search_g2_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE error) = 0;
    virtual void on_g2search_g2_receive_event_log_load_end(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list) = 0;
    virtual void on_g2search_g2_receive_event_log_load_stop(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list) = 0;
    virtual void on_g2search_g2_receive_text_in_log_load_end(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list) = 0;
    virtual void on_g2search_g2_receive_text_in_log_load_stop(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list) = 0;
    virtual void on_g2search_g2_receive_scope_list(G2HSEARCH_G2 handle, int channel,  const std::vector<G2SCOPE>& scopes, G2SEARCH_G2_SCOPE_TYPE::TYPE type) = 0;
    virtual void on_g2search_g2_receive_spot_list(G2HSEARCH_G2 handle, int channel, const std::vector<G2SPOT>& spots) = 0;
    virtual void on_g2search_g2_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel) = 0;
    virtual void on_g2search_g2_receive_db_info(G2HSEARCH_G2 handle, int channel, const G2SEARCH_G2_REMOTE_DB& di) = 0;
    virtual void on_g2search_g2_receive_db_info_external(G2HSEARCH_G2 handle, int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& dis) = 0;
    virtual void on_g2search_g2_receive_db_selected(G2HSEARCH_G2 handle, int channel, unsigned int id, G2SEARCH_G2_REMOTE_DB::DB_SELECT_RESULT result) = 0;
    virtual void on_g2search_g2_receive_virtual_channelmap(G2HSEARCH_G2 handle, int channel) = 0;
    virtual void on_g2search_g2_require_prepare_rollback(G2HSEARCH_G2 handle, int channel, bool prepare) = 0;
    virtual void on_g2search_g2_probe_session_profile(G2HSEARCH_G2 handle, int channel, const G2PROBE_SESSION_PROFILE& probe) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_SEARCH_G2_LISTENER_H_
