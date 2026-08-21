// g2client_dualaudio.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_dualaudio.h"
#include "g2client_dualaudio_listener.h"

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
    g2_dualaudio_register_callback(_handle, G2DUALAUDIO_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2dualaudio_listener* ptr = ((g2dualaudio*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2dualaudio::g2dualaudio(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_dualaudio_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected)
    REGISTER_CALLBACK(on_disconnected)
    REGISTER_CALLBACK(on_receive_audio_in_opened)
    REGISTER_CALLBACK(on_receive_audio_in_closed)
    REGISTER_CALLBACK(on_receive_audio_out_opened)
    REGISTER_CALLBACK(on_receive_audio_out_closed)
    REGISTER_CALLBACK(on_require_select_channel)
    REGISTER_CALLBACK(on_require_reconnect_imp)
}

g2dualaudio::~g2dualaudio(void)
{
    g2_dualaudio_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2dualaudio::startup(int connections, G2HWND focus)
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    g2_dualaudio_startup(_handle, connections, focus);
}

void g2dualaudio::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    g2_dualaudio_cleanup(_handle);
}

void g2dualaudio::set_listener(g2dualaudio_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2dualaudio::connect(const G2GUID& site, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    return g2_dualaudio_connect(_handle, &site, options, res);
}

int g2dualaudio::connect_ras(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    return g2_dualaudio_connect_ras(_handle, &ni, options, res);
}

void g2dualaudio::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    return g2_dualaudio_disconnect(_handle, channel);
}

bool g2dualaudio::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    return g2_dualaudio_is_connecting(_handle, channel);
}

bool g2dualaudio::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    return g2_dualaudio_is_connected(_handle, channel);
}

bool g2dualaudio::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    return g2_dualaudio_is_disconnecting(_handle, channel);
}

bool g2dualaudio::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    return g2_dualaudio_is_disconnected(_handle, channel);
}

bool g2dualaudio::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_dualaudio is not initialized");
    return g2_dualaudio_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2dualaudio::set_audio_in_enable(int channel, int number, bool enable)
{
    return g2_dualaudio_set_audio_in_enable(_handle, channel, number, enable);
}

bool g2dualaudio::set_audio_out_enable(int channel, bool enable)
{
    return g2_dualaudio_set_audio_out_enable(_handle, channel, enable);
}

void g2dualaudio::set_audio_in_disable(int channel)
{
    g2_dualaudio_set_audio_in_disable(_handle, channel);
}

void g2dualaudio::set_audio_out_disable(int channel)
{
    g2_dualaudio_set_audio_out_disable(_handle, channel);
}

void g2dualaudio::set_audio_in_disable_all(G2HDUALAUDIO handle)
{
    g2_dualaudio_set_audio_in_disable_all(handle);
}

void g2dualaudio::set_audio_out_disable_all(G2HDUALAUDIO handle)
{
    g2_dualaudio_set_audio_out_disable_all(handle);
}

void g2dualaudio::append_reserve(const G2GUID& site, G2DUALAUDIO_OPEN_MODE::MODE mode, const G2DUALAUDIO_RESERVE& param)
{
    g2_dualaudio_append_reserve(_handle, &site, mode, &param);
}

void g2dualaudio::remove_reserve(const G2GUID& site)
{
    g2_dualaudio_remove_reserve(_handle, &site);
}

//////////////////////////////////////////////////////////////////////////

bool g2dualaudio::get_reserved_parameter(const G2GUID& site, G2DUALAUDIO_OPEN_MODE::MODE mode, G2DUALAUDIO_RESERVE& param) const
{
    return g2_dualaudio_get_reserved_parameter(_handle, &site, mode, &param);
}

G2GUID g2dualaudio::get_site_guid(int channel) const
{
    return g2_dualaudio_get_site_guid(_handle, channel);
}

int g2dualaudio::get_channel_from_site_guid(const G2GUID& site) const
{
    return g2_dualaudio_get_channel_from_site_guid(_handle, &site);
}

//////////////////////////////////////////////////////////////////////////

bool g2dualaudio::is_drive_mode(int channel, G2DUALAUDIO_DRIVE::MODE mode) const
{
    return g2_dualaudio_is_drive_mode(_handle, channel, mode);
}

bool g2dualaudio::is_server_full_channel(const G2GUID& site) const
{
    return g2_dualaudio_is_server_full_channel(_handle, &site);
}

bool g2dualaudio::is_reserved(const G2GUID& site, G2DUALAUDIO_OPEN_MODE::MODE mode) const
{
    return g2_dualaudio_is_reserved(_handle, &site, mode);
}

bool g2dualaudio::is_contains_site_guid(const G2GUID& site) const
{
    return g2_dualaudio_is_contains_site_guid(_handle, &site);
}

bool g2dualaudio::is_require_reconnect(int channel) const
{
    return g2_dualaudio_is_require_reconnect(_handle, channel);
}

bool g2dualaudio::is_last_use_audio_in(const G2GUID& site) const
{
    return g2_dualaudio_is_last_use_audio_in(_handle, &site);
}

bool g2dualaudio::is_last_use_audio_out(const G2GUID& site) const
{
    return g2_dualaudio_is_last_use_audio_out(_handle, &site);
}

bool g2dualaudio::is_activate_audio_in(const G2GUID& site) const
{
    return g2_dualaudio_is_activate_audio_in(_handle, &site);
}

bool g2dualaudio::is_activate_audio_out(const G2GUID& site) const
{
    return g2_dualaudio_is_activate_audio_out(_handle, &site);
}

bool g2dualaudio::is_activate_dualaudio(const G2GUID& site) const
{
    return g2_dualaudio_is_activate_dualaudio(_handle, &site);
}

bool g2dualaudio::is_opening_audio_out(int channel) const
{
    return g2_dualaudio_is_opening_audio_out(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2dualaudio::on_connected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2dualaudio_connected(handle, wparam));
    return 1L;
}

G2RESULT g2dualaudio::on_disconnected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2dualaudio_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2dualaudio::on_receive_audio_in_opened(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* site = (G2GUID*)(lparam);
    CALL_LISTENER(on_g2dualaudio_receive_audio_in_opened(handle, wparam, *site));
    return 1L;
}

G2RESULT g2dualaudio::on_receive_audio_in_closed(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* site = (G2GUID*)(lparam);
    CALL_LISTENER(on_g2dualaudio_receive_audio_in_closed(handle, wparam, *site));
    return 1L;
}

G2RESULT g2dualaudio::on_receive_audio_out_opened(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* site = (G2GUID*)(lparam);
    CALL_LISTENER(on_g2dualaudio_receive_audio_out_opened(handle, wparam, *site));
    return 1L;
}

G2RESULT g2dualaudio::on_receive_audio_out_closed(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* site = (G2GUID*)(lparam);
    CALL_LISTENER(on_g2dualaudio_receive_audio_out_closed(handle, wparam, *site));
    return 1L;
}

G2RESULT g2dualaudio::on_require_select_channel(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2DUALAUDIO_SELECT_CHANNEL* data = (G2DUALAUDIO_SELECT_CHANNEL*)(lparam);
    CALL_LISTENER(on_g2dualaudio_require_select_channel(handle, wparam, data->_site, data->_audio_count, *data->_number));
    return 1L;
}

G2RESULT g2dualaudio::on_require_reconnect_imp(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* site = (G2GUID*)(lparam);
    g2dualaudio_listener* ptr = ((g2dualaudio*)(uparam))->_listener;
    return (ptr != NULL) ? (ptr->on_g2dualaudio_require_reconnect_imp(handle, *site) ? 1L : 0L) : 0L;
}
