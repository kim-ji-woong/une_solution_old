// g2client_search_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_SEARCH_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_SEARCH_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_search.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2search_listener
{
public:
    virtual void on_g2search_connected(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_disconnected(G2HSEARCH handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2search_receive_recorded_date(G2HSEARCH handle, int channel, const std::vector<G2TIME>& dates) = 0;
    virtual void on_g2search_receive_recorded_time_hour(G2HSEARCH handle, int channel, const bool hour[][24], int segcount) = 0;
    virtual void on_g2search_receive_recorded_time_minute(G2HSEARCH handle, int channel, const unsigned char minute[][24 * 60]) = 0;
    virtual void on_g2search_receive_recorded_rechour_minute(G2HSEARCH handle, int channel, const std::vector<G2RECORD_TIME_INFO>& rti) = 0;
    virtual void on_g2search_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2search_receive_no_frame(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_receive_no_recorded_data(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_receive_no_recorded_data_from_search_target(G2HSEARCH handle, int channel, G2SEARCH_TARGET::TYPE target) = 0;
    virtual void on_g2search_receive_find_idr_event_time(G2HSEARCH handle, int channel, const G2TIME& time) = 0;
    virtual void on_g2search_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND speed) = 0;
    virtual void on_g2search_receive_notify_play_stop_post(G2HSEARCH handle, int channel, G2SEARCH_DRIVE::MODE mode) = 0;
    virtual void on_g2search_receive_notify_end_of_play(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_receive_notify_segment_changed(G2HSEARCH handle, int channel, int segment) = 0;
    virtual void on_g2search_receive_notify_search_mode_changed(G2HSEARCH handle, int channel, G2SEARCH_MODE::TYPE mode) = 0;
    virtual void on_g2search_receive_notify_command_end(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND command) = 0;
    virtual void on_g2search_receive_query_result_event(G2HSEARCH handle, int channel, const std::vector<G2SEARCH_LOG_INFO>& info) = 0;
    virtual void on_g2search_receive_query_result_text_in(G2HSEARCH handle, int channel, const std::vector<G2TEXT_IN>& data) = 0;
    virtual void on_g2search_receive_recorded_date_scope(G2HSEARCH handle, int channel, const std::vector<G2SCOPE>& scopes) = 0;
    virtual void on_g2search_receive_segment_spot(G2HSEARCH handle, int channel, const std::vector<G2SPOT>& spots) = 0;
    virtual void on_g2search_receive_error(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_receive_text_in(G2HSEARCH handle, int channel, const G2TEXT_IN_ELEMENT& data) = 0;
    virtual void on_g2search_receive_external_tango_info(G2HSEARCH handle, int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& info) = 0;
    virtual void on_g2search_receive_gps_data_start(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_receive_gps_data(G2HSEARCH handle, int channel, const G2TEXT_IN& data) = 0;
    virtual void on_g2search_receive_gps_data_list(G2HSEARCH handle, int channel, const std::vector<G2TEXT_IN>& data) = 0;
    virtual void on_g2search_receive_gps_data_end(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_receive_gps_data_end_count(G2HSEARCH handle, int channel, int total) = 0;
    virtual void on_g2search_receive_gps_data_export_cancel_result(G2HSEARCH handle, int channel, int result) = 0;
    virtual void on_g2search_receive_gps_data_measure_result(G2HSEARCH handle, int channel, int count, signed char done) = 0;
    virtual void on_g2search_require_prepare_playback(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND command, const G2SPOT& spot) = 0;
    virtual bool on_g2search_require_prepare_load_event_image(G2HSEARCH handle, int channel, int selected, bool last) = 0;
    virtual bool on_g2search_require_prepare_reload(G2HSEARCH handle, int channel) = 0;
    virtual void on_g2search_probe_session_profile(G2HSEARCH handle, int channel, const G2PROBE_SESSION_PROFILE& probe) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_SEARCH_LISTENER_H_
