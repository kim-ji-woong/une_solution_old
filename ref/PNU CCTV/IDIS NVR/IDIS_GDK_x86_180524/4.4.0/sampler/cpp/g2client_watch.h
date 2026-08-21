// g2client_watch.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_WATCH_H_
#define _G2_CLIENT_DLL_SAMPLER_WATCH_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_watch.h>
#include <set>
#include <utility>
#include <vector>

namespace client {
    class g2watch_listener;

//////////////////////////////////////////////////////////////////////////

class g2watch
{
public:
    g2watch(void);
    virtual ~g2watch(void);

private:
    G2HWATCH _handle;
    g2watch_listener* _listener;

public:
    void startup(int connections, G2HWND focus);
    void cleanup(void);
    void set_listener(g2watch_listener* listener);
    bool set_focus_G2HWND(G2HWND focus);

    G2HWATCH safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& root, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    int  connect_ras(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    int  connect_ras_event(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool set_camera_list(int channel, const std::set<int>& channels, bool force);
    bool set_camera_stream_set(int channel, const std::set<std::pair<int, int> >& streams, bool force);
    bool set_camera_stream_set(int channel, const std::set<std::pair<int, int> >& streams, const std::set<std::pair<int, int> >& onetime_streams);
    bool set_audio_list(int channel, const std::set<int>& channels, bool force);
    bool set_stream_id(int channel, int camera, int id);
    bool set_alarm_out(int channel, int id, bool on);
    bool set_camera_color(int channel, int camera, int type, int value);
    bool set_ptz_command(int channel, int camera, const G2LIVE_PTZ_COMMAND& command);
    bool set_ptz_preset(int channel, int camera, const G2LIVE_PTZ_PRESET& preset);
    bool set_network_alarm(int channel, const G2LIVE_NETWORK_ALARM_INFO& info);
    bool set_enable_audio_streaming(int channel, int camera, bool enable);
    bool set_enable_audio_capturing(int channel, int camera, bool enable);
    bool set_disable_audio_streaming(int channel);
    bool set_disable_audio_capturing(int channel);
    bool set_disable_audio(int channel);
    void set_probe_session_profile(bool active);

public:
    bool request_alive_check(int channel);
    bool request_device_status(int channel);
    bool request_ptz_menu(int channel, int camera);
    bool request_ptz_preset(int channel, int camera);
    bool request_stream_channel_control(int channel, int camera, int seq, signed char* buf, int len);
    bool request_instant_recording_start(int channel, std::vector<G2INSTANT_RECORD_SETTING> setting);
    bool request_instant_recording_stop(int channel, std::set<int> channels);
    bool request_instant_recording_status(int channel, std::set<int> channels);

public:
    bool send_command_control_color_status(int channel, int camera);
    bool send_command_control_color(int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR* control);
    bool send_command_control_ptz_status(int channel, int camera);
    bool send_command_control_ptz_status_relative(int channel, int camera);
	bool send_command_control_ptz_status_relative_IDIS(int channel, int camera);
    bool send_command_control_ptz(int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ* control);
	bool send_command_control_ptz_IDIS(int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ_STATUS* status, bool req_result);
    bool send_network_alarm_info(int channel, const G2LIVE_NETWORK_ALARM_INFO* info);
    bool send_elevator_status_info(int channel, const G2LIVE_ELEVATOR_STATUS_INFO* info);

public:
    bool get_server_network_info(int channel, G2SERVER_NETWORK_INFO& ni) const;
    bool get_product_info(int channel, G2_PRODUCT_INFO& pi) const;
    bool get_remote_watch_caps(int channel, G2_PRODUCT_INFO_CAPS::REMOTE_WATCH& caps) const;
    bool get_authority(int channel, G2RAS_AUTHORITY& auth) const;
    bool get_camera_list(int channel, std::set<int>& channels) const;
    bool get_camera_stream_set(int channel, std::set<std::pair<int, int> >& streams) const;
    bool get_camera_stream_set(int channel, int camera, std::set<std::pair<int, int> >& streams) const;
    bool get_audio_list(int channel, std::set<int>& channels) const;
    int  get_stream_id(int channel, int camera) const;
    unsigned int get_stream_remote(int channel, int camera) const;
    bool get_status(int channel, G2DEVICE_STATUS& status) const;
    bool get_status_stream_info(int channel, int camera, int stream_id, G2DEVICE_STATUS_STREAM_INFO& status) const;
    bool get_status_ip_camera_info(int channel, int camera, G2DEVICE_STATUS_IP_CAMERA_INFO& status) const;
    bool get_status_ptz_advanced_info(int channel, int camera, G2DEVICE_STATUS_PTZ_ADVANCED_INFO& status) const;
    int get_status_ptz(int channel, int camera) const;
    unsigned int get_status_ptz_function(int channel, int camera) const;
    bool get_status_command_control_color(int channel, int camera, G2LIVE_COMMAND_CONTROL_COLOR* control, G2LIVE_COMMAND_CONTROL_COLOR_RANGE* range) const;

public:
    bool is_enable_multi_stream(int channel, int camera) const;
    bool is_enable_command_control_color(int channel, int camera) const;
    bool is_enable_command_control_ptz(int channel, int camera) const;
    bool is_enable_command_control_ptz_relative(int channel, int camera) const;
	bool is_enable_command_control_ptz_relative_IDIS(int channel, int camera) const;
	bool is_enable_command_control_ptz_relative_IDIS_one_click_move(int channel, int camera) const;
    bool is_enable_audio_in(int channel, int camera) const;
    bool is_enable_audio_out(int channel, int camera) const;
    bool is_contains_audio_streaming(int channel, int camera) const;
    bool is_contains_audio_capturing(int channel, int camera) const;
    bool is_contains_audio_capturing(int channel) const;
    bool is_contains_audio(int channel) const;
    bool is_audio_out_opening(int channel, int camera) const;
    bool is_support(int channel, G2LIVE_SUPPORT::QUERY query) const;
    bool is_enable_audio_device_record(void) const;
    bool is_enable_audio_device_play(G2HWND focus) const;
    bool is_probe_perofmance(void) const;

protected:
    static G2RESULT G2CALLBACK on_connected(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_frame_data(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_ptz_preset(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_ptz_menu(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_camera_title_idr(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_network_camera_information(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_audio_out_not_available(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_command_result_control_color_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_command_result_control_color(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_command_result_control_ptz_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_command_result_control_ptz(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_network_alarm_result(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_elevator_status_info_response(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_instant_recording_start(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_instant_recording_stop(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_instant_recording_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_streaming_started(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_streaming_stopped(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_capturing_started(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_capturing_stopped(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_probe_session_profile(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_WATCH_H_
