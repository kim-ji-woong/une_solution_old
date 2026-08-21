// g2app_frame_relay.cpp : implementation file
//

#include "stdafx.h"
#include "g2app_frame_relay.h"
#include "g2app_frame_relay_listener.h"

#include <assert.h>

#if defined(_WIN32)
    #if defined(_WIN64)
        #pragma comment(lib, "G2ClientGDKx64.lib")
    #else
        #pragma comment(lib, "G2ClientGDK.lib")
    #endif
    #pragma warning(disable : 4312)
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

#define REGISTER_CALLBACK(function) \
    g2_app_frame_relay_register_callback(_handle, G2APP_FRAME_RELAY_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2app_frame_relay_listener* ptr = ((g2app_frame_relay*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2app_frame_relay::g2app_frame_relay(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_app_frame_relay_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_receive_request_site_product_info);
}

g2app_frame_relay::~g2app_frame_relay(void)
{
    g2_app_frame_relay_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2app_frame_relay::startup(int connections, unsigned int send_queue_size_limit /*= 0*/)
{
    assert(_handle != G2HNULL && "g2app_frame_relay is not initialized");
    g2_app_frame_relay_startup(_handle, connections, send_queue_size_limit);
}

void g2app_frame_relay::cleanup(void)
{
    assert(_handle != G2HNULL && "g2app_frame_relay is not initialized");
    g2_app_frame_relay_cleanup(_handle);
}

void g2app_frame_relay::set_listener(g2app_frame_relay_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2app_frame_relay::connect(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2app_frame_relay is not initialized");
    return g2_app_frame_relay_connect(_handle, &ni, options, res);
}

void g2app_frame_relay::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2app_frame_relay is not initialized");
    return g2_app_frame_relay_disconnect(_handle, channel);
}

bool g2app_frame_relay::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2app_frame_relay is not initialized");
    return g2_app_frame_relay_is_connecting(_handle, channel);
}

bool g2app_frame_relay::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2app_frame_relay is not initialized");
    return g2_app_frame_relay_is_connected(_handle, channel);
}

bool g2app_frame_relay::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2app_frame_relay is not initialized");
    return g2_app_frame_relay_is_disconnecting(_handle, channel);
}

bool g2app_frame_relay::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2app_frame_relay is not initialized");
    return g2_app_frame_relay_is_disconnected(_handle, channel);
}

bool g2app_frame_relay::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2app_frame_relay is not initialized");
    return g2_app_frame_relay_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2app_frame_relay::request_alive_check(int channel, int val)
{
    return g2_app_frame_relay_request_alive_check(_handle, channel, val);
}

bool g2app_frame_relay::send_site_connected(int channel, const G2STRING_64& site)
{
    return g2_app_frame_relay_send_site_connected(_handle, channel, &site);
}

bool g2app_frame_relay::send_site_disconnected(int channel, const G2STRING_64& site, int reason)
{
    return g2_app_frame_relay_send_site_disconnected(_handle, channel, &site, reason);
}

bool g2app_frame_relay::send_site_product_info(int channel, const G2STRING_64& site, bool(G2API *get_adaptor)(G2HANDLE, void*), G2HANDLE from_handle, int from_channel)
{
    return g2_app_frame_relay_send_site_product_info(_handle, channel, &site, get_adaptor, from_handle, from_channel);
}

bool g2app_frame_relay::send_site_frame_data(int channel, const G2STRING_64& site, const G2FRAME& frame)
{
    return g2_app_frame_relay_send_site_frame_data(_handle, channel, &site, &frame);
}

//////////////////////////////////////////////////////////////////////////

bool g2app_frame_relay::is_able_to_send(int channel, unsigned int size) const
{
    return g2_app_frame_relay_is_able_to_send(_handle, channel, size);
}

//////////////////////////////////////////////////////////////////////////

bool g2app_frame_relay::search_controller_option_read_file(const std::wstring& path, G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION& option)
{
    return g2_app_frame_relay_search_controller_option_read_file(path.c_str(), &option);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2app_frame_relay::on_connected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2app_frame_relay_connected(handle, (int)wparam));
    return 1L;
}

G2RESULT g2app_frame_relay::on_disconnected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2app_frame_relay_disconnected(handle, (int)wparam, (int)lparam));
    return 1L;
}

G2RESULT g2app_frame_relay::on_receive_request_site_product_info(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2app_frame_relay_receive_request_site_product_info(handle, (int)wparam));
    return 1L;
}
