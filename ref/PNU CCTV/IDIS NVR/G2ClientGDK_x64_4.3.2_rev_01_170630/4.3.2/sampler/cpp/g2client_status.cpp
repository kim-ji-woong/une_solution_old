// g2client_status.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_status.h"
#include "g2client_status_listener.h"

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
    g2_status_register_callback(_handle, G2STATUS_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2status_listener* ptr = ((g2status*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2status::g2status(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_status_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_receive_status);
    REGISTER_CALLBACK(on_receive_callback_event);
    REGISTER_CALLBACK(on_receive_log_system);
    REGISTER_CALLBACK(on_receive_log_event);
    REGISTER_CALLBACK(on_receive_log_debug);
}

g2status::~g2status(void)
{
    g2_status_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2status::startup(int connections, int callback_connections)
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    g2_status_startup(_handle, connections, callback_connections);
}

void g2status::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    g2_status_cleanup(_handle);
}

void g2status::set_listener(g2status_listener* listener)
{
    _listener = listener;
}

bool g2status::callback_server_restart(unsigned short port)
{
    return g2_status_callback_server_restart(_handle, port);
}

void g2status::callback_server_shutdown(void)
{
    g2_status_callback_server_shutdown(_handle);
}

bool g2status::callback_server_is_startup(void) const
{
    return g2_status_callback_server_is_startup(_handle);
}

//////////////////////////////////////////////////////////////////////////

int g2status::connect(const G2GUID& root, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    return g2_status_connect(_handle, &root, options, res);
}

int g2status::connect_ras(const G2NETWORK_INFO& ni, bool idr, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    return g2_status_connect_ras(_handle, &ni, idr, options, res);
}

void g2status::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    return g2_status_disconnect(_handle, channel);
}

bool g2status::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    return g2_status_is_connecting(_handle, channel);
}

bool g2status::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    return g2_status_is_connected(_handle, channel);
}

bool g2status::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    return g2_status_is_disconnecting(_handle, channel);
}

bool g2status::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    return g2_status_is_disconnected(_handle, channel);
}

bool g2status::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_status is not initialized");
    return g2_status_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2status::request_alive_check(int channel)
{
    return g2_status_request_alive_check(_handle, channel);
}

bool g2status::request_panic_record(int channel, bool on)
{
    return g2_status_request_panic_record(_handle, channel, on);
}

bool g2status::request_status(int channel)
{
    return g2_status_request_status(_handle, channel);
}

bool g2status::request_log(int channel, const G2STATUS_LOG_SEARCH_OPTIONS& options, G2STATUS_LOG::RESULT& res)
{
    int r = 0;
    bool ret = g2_status_request_log(_handle, channel, &options, &r);
    res = (G2STATUS_LOG::RESULT)r;
    return ret;
}

//////////////////////////////////////////////////////////////////////////

bool g2status::get_server_network_info(int channel, G2SERVER_NETWORK_INFO& ni) const
{
    return g2_status_get_server_network_info(_handle, channel, &ni);
}

bool g2status::get_product_info(int channel, G2_PRODUCT_INFO& pi) const
{
    return g2_status_get_product_info(_handle, channel, &pi);
}

bool g2status::get_remote_status_caps(int channel, G2_PRODUCT_INFO_CAPS::REMOTE_STATUS& caps) const
{
    return g2_status_get_remote_status_caps(_handle, channel, &caps);
}

bool g2status::get_authority(int channel, G2RAS_AUTHORITY& auth) const
{
    return g2_status_get_authority(_handle, channel, &auth);
}

bool g2status::get_status(int channel, G2MONITORING_DEVICE_STATUS& status) const
{
    return g2_status_get_status(_handle, channel, &status);
}

//////////////////////////////////////////////////////////////////////////

bool g2status::is_support(int channel, G2STATUS_SUPPORT::QUERY query) const
{
    return g2_status_is_support(_handle, channel, query);
}

bool g2status::is_authority(int channel, G2RAS_AUTHORITY::TYPE authority) const
{
    return g2_status_is_authority(_handle, channel, authority);
}

bool g2status::is_IDR(int channel) const
{
    return g2_status_is_IDR(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2status::on_connected(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2status_connected(handle, wparam));
    return 1L;
}

G2RESULT g2status::on_disconnected(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2status_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2status::on_receive_status(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2MONITORING_DEVICE_STATUS* status = (G2MONITORING_DEVICE_STATUS*)(lparam);
    CALL_LISTENER(on_g2status_receive_status(handle, wparam, *status));
    return 1L;
}

G2RESULT g2status::on_receive_callback_event(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_INFO* ei = (G2EVENT_INFO*)(lparam);
    CALL_LISTENER(on_g2status_receive_callback_event(handle, *ei));
    return 1L;
}

G2RESULT g2status::on_receive_log_system(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2STATUS_LOG_SYSTEM* buf = (G2STATUS_LOG_SYSTEM*)p->_bunch;
    std::list<G2STATUS_LOG_SYSTEM> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2status_receive_log_system(handle, wparam, data));
    return 1L;
}

G2RESULT g2status::on_receive_log_event(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2STATUS_LOG_EVENT* buf = (G2STATUS_LOG_EVENT*)p->_bunch;
    std::list<G2STATUS_LOG_EVENT> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2status_receive_log_event(handle, wparam, data));
    return 1L;
}

G2RESULT g2status::on_receive_log_debug(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2STATUS_LOG_SYSTEM* buf = (G2STATUS_LOG_SYSTEM*)p->_bunch;
    std::list<G2STATUS_LOG_SYSTEM> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2status_receive_log_debug(handle, wparam, data));
    return 1L;
}
