// g2client_violet.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_VIOLET_H_
#define _G2_CLIENT_DLL_SAMPLER_VIOLET_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_violet.h>
#include <vector>
#include <set>
#include <string>

namespace client {
    class g2violet_listener;

//////////////////////////////////////////////////////////////////////////

class g2violet
{
public:
    g2violet(void);
    virtual ~g2violet(void);

private:
    G2HVIOLET _handle;
    g2violet_listener* _listener;

public:
    void startup(int connections);
    void cleanup(void);
    void set_listener(g2violet_listener* listener);

    G2HVIOLET safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& service, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool request_set_layout_monitor_live(int channel, int number, const G2GUID& layoutGUID);
    bool request_set_screen_format(int channel, int monitor, int format);
    bool request_set_screen_camera_format(int channel, int monitor, int camera, int format);
    bool request_get_cameras_on_monitor(int channel, int monitor);
    bool request_system_log_search(int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option);
    bool request_system_log_search_stop(int channel);

public:
    bool send_stream_channel_control(int channel, int seq, const std::string& buf);
    bool send_stream_channel_control(int channel, int seq, int id);
    bool send_stream_channel_control_ptz_joystick(int channel, int seq, int id, int speed);
    bool send_mouse_input(int channel, const G2VIOLET_MOUSE_INPUT& mi);
    bool send_monitor_message(int channel, int monitor, const wchar_t* message, unsigned int time_out, unsigned short size, int position);
    bool send_monitor_message_hide(int channel, int monitor);
    bool send_screen_message(int channel, int monitor, const G2VIOLET_MONITOR_MESSAGE& message);
    bool send_screen_message_hide(int channel, int monitor);
    bool send_agent_audio_connect(int channel, int monitor, const wchar_t* address, unsigned short port, const wchar_t* id, const wchar_t* password, bool encrypted);
    bool send_agent_audio_disconnect(int channel, int monitor);

protected:
    static G2RESULT G2CALLBACK on_connected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_end(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_fail(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_stop(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_agent_audio_connected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_agent_audio_disconnected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_agent_layout_attached(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_cameras_on_monitor(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_VIOLET_H_
