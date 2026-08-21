// g2client_violet.cpp : implementaton file
//

#include "stdafx.h"
#include "g2client_violet.h"
#include "g2client_violet_listener.h"

#include <assert.h>

#if defined(_WIN32)
#if defined(_WIN64)
#pragma comment(lib, "G2ClientGDKx64.lib")
#else
#pragma comment(lib, "G2ClientGDK.lib")
#endif
#pragma warning(disable : 4244 4267 4312 4800)
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

#define REGISTER_CALLBACK(function) \
    g2_violet_register_callback(_handle, G2VIOLET_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2violet_listener* ptr = ((g2violet*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2violet::g2violet(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_violet_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_receive_service_log_load);
    REGISTER_CALLBACK(on_receive_service_log_load_end);
    REGISTER_CALLBACK(on_receive_service_log_load_fail);
    REGISTER_CALLBACK(on_receive_service_log_load_stop);
    REGISTER_CALLBACK(on_receive_agent_audio_connected);
    REGISTER_CALLBACK(on_receive_agent_audio_disconnected);
    REGISTER_CALLBACK(on_receive_agent_layout_attached);
    REGISTER_CALLBACK(on_receive_cameras_on_monitor);
}

g2violet::~g2violet(void)
{
    g2_violet_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2violet::startup(int connections)
{
    assert(_handle != G2HNULL && "g2client_violet is not initialized");
    g2_violet_startup(_handle, connections);
}

void g2violet::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_violet is not initialized");
    g2_violet_cleanup(_handle);
}

void g2violet::set_listener(g2violet_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2violet::connect(const G2GUID& service, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_violet is not initialized");
    return g2_violet_connect(_handle, &service, options, res);
}

void g2violet::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_violet is not initialized");
    return g2_violet_disconnect(_handle, channel);
}

bool g2violet::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_violet is not initialized");
    return g2_violet_is_connecting(_handle, channel);
}

bool g2violet::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_violet is not initialized");
    return g2_violet_is_connected(_handle, channel);
}

bool g2violet::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_violet is not initialized");
    return g2_violet_is_disconnecting(_handle, channel);
}

bool g2violet::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_violet is not initialized");
    return g2_violet_is_disconnected(_handle, channel);
}

bool g2violet::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_violet is not initialized");
    return g2_violet_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2violet::request_set_layout_monitor_live(int channel, int number, const G2GUID& layoutGUID)
{
    return g2_violet_request_set_layout_monitor_live(_handle, channel, number, &layoutGUID);
}

bool g2violet::request_set_screen_format(int channel, int monitor, int format)
{
    return g2_violet_request_set_screen_format(_handle, channel, monitor, format);
}

bool g2violet::request_set_screen_camera_format(int channel, int monitor, int camera, int format)
{
    return g2_violet_request_set_screen_camera_format(_handle, channel, monitor, camera, format);
}

bool g2violet::request_get_cameras_on_monitor(int channel, int monitor)
{
    return g2_violet_request_get_cameras_on_monitor(_handle, channel, monitor);
}

bool g2violet::request_system_log_search(int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option)
{
    return g2_violet_request_system_log_search(_handle, channel, &option);
}

bool g2violet::request_system_log_search_stop(int channel)
{
    return g2_violet_request_system_log_search_stop(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2violet::send_stream_channel_control(int channel, int seq, const std::string& buf)
{
    return g2_violet_send_stream_channel_control(_handle, channel, seq, buf.length(), buf.c_str());
}

bool g2violet::send_stream_channel_control(int channel, int seq, int id)
{
    return g2_violet_send_stream_channel_control_via_id(_handle, channel, seq, id);
}

bool g2violet::send_stream_channel_control_ptz_joystick(int channel, int seq, int id, int speed)
{
    return g2_violet_send_stream_channel_control_ptz_joystick(_handle, channel, seq, id, speed);
}

bool g2violet::send_mouse_input(int channel, const G2VIOLET_MOUSE_INPUT& mi)
{
    return g2_violet_send_mouse_input(_handle, channel, &mi);
}

bool g2violet::send_monitor_message(int channel, int monitor, const wchar_t* message, unsigned int time_out, unsigned short size, int position)
{
    return g2_violet_send_monitor_message(_handle, channel, monitor, message, time_out, size, position);
}

bool g2violet::send_monitor_message_hide(int channel, int monitor)
{
    return g2_violet_send_monitor_message_hide(_handle, channel, monitor);
}

bool g2violet::send_screen_message(int channel, int monitor, const G2VIOLET_MONITOR_MESSAGE& message)
{
    return g2_violet_send_screen_message(_handle, channel, monitor, &message);
}

bool g2violet::send_screen_message_hide(int channel, int monitor)
{
    return g2_violet_send_screen_message_hide(_handle, channel, monitor);
}

bool g2violet::send_agent_audio_connect(int channel, int monitor, const wchar_t* address, unsigned short port, const wchar_t* id, const wchar_t* password, bool encrypted)
{
    return g2_violet_send_agent_audio_connect(_handle, channel, monitor, address, port, id, password, encrypted);
}

bool g2violet::send_agent_audio_disconnect(int channel, int monitor)
{
    return g2_violet_send_agent_audio_disconnect(_handle, channel, monitor);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2violet::on_connected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2violet_connected(handle, wparam));
    return 1L;
}

G2RESULT g2violet::on_disconnected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2violet_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2violet::on_receive_service_log_load(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SYSTEM_LOG* data = (G2SYSTEM_LOG*)(lparam);
    CALL_LISTENER(on_g2violet_receive_service_log_load(handle, wparam, *data));
    return 1L;
}

G2RESULT g2violet::on_receive_service_log_load_end(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2violet_receive_service_log_load_end(handle, wparam));
    return 1L;
}

G2RESULT g2violet::on_receive_service_log_load_fail(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2violet_receive_service_log_load_fail(handle, wparam));
    return 1L;
}

G2RESULT g2violet::on_receive_service_log_load_stop(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2violet_receive_service_log_load_stop(handle, wparam));
    return 1L;
}

G2RESULT G2CALLBACK g2violet::on_receive_agent_audio_connected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2violet_receive_agent_audio_connected(handle, wparam, lparam));
    return 1L;
}

G2RESULT G2CALLBACK g2violet::on_receive_agent_audio_disconnected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2violet_receive_agent_audio_disconnected(handle, wparam, lparam));
    return 1L;
}

G2RESULT G2CALLBACK g2violet::on_receive_agent_layout_attached(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2VIOLET_PARAM_LAYOUT* p = (G2VIOLET_PARAM_LAYOUT*)(lparam);
    CALL_LISTENER(on_g2violet_receive_agent_layout_attached(handle, wparam, p->_agent, p->_monitor, p->_layout));
    return 1L;
}

G2RESULT G2CALLBACK g2violet::on_receive_cameras_on_monitor(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2VIOLET_PARAM_CAMERASET* p = (G2VIOLET_PARAM_CAMERASET*)(lparam);
    std::vector<G2GUID> GUIDs;
    if (p->_camera_GUIDs._guid &&
        p->_camera_GUIDs._len) {
        GUIDs.assign(p->_camera_GUIDs._guid, p->_camera_GUIDs._guid + p->_camera_GUIDs._len);
    }
    CALL_LISTENER(on_g2violet_receive_cameras_on_monitor(handle, wparam, p->_agent, p->_monitor, GUIDs));
    return 1L;
}
