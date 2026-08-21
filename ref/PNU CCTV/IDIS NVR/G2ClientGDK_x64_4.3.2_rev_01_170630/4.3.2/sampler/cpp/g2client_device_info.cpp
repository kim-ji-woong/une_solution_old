// g2client_device_info.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_device_info.h"
#include "g2client_device_info_listener.h"

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
    g2_device_info_register_callback(_handle, G2DEVICE_INFO_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2device_info_listener* ptr = ((g2device_info*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

#ifndef LOWORD
#define LOWORD(l) ((unsigned short)(((unsigned int)(l)) & 0xffff))
#endif
#ifndef HIWORD
#define HIWORD(l) ((unsigned short)((((unsigned int)(l)) >> 16) & 0xffff))
#endif

//////////////////////////////////////////////////////////////////////////


g2device_info::g2device_info(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_device_info_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_receive_product_info);
    REGISTER_CALLBACK(on_failed);
    REGISTER_CALLBACK(on_canceled);
}

g2device_info::~g2device_info(void)
{
    g2_device_info_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2device_info::startup(int connections)
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    g2_device_info_startup(_handle, connections);
}

void g2device_info::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    g2_device_info_cleanup(_handle);
}

void g2device_info::set_listener(g2device_info_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2device_info::connect_ras(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_connect_ras(_handle, &ni, options, res);
}

void g2device_info::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_disconnect(_handle, channel);
}

bool g2device_info::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_is_connecting(_handle, channel);
}

bool g2device_info::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_is_connected(_handle, channel);
}

bool g2device_info::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_is_disconnecting(_handle, channel);
}

bool g2device_info::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_is_disconnected(_handle, channel);
}

bool g2device_info::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////
bool g2device_info::get_product_info(int channel, G2_PRODUCT_INFO& pi) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_get_product_info(_handle, channel, &pi);
}

bool g2device_info::get_authority(int channel, G2RAS_AUTHORITY& auth) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_get_authority(_handle, channel, &auth);
}

//////////////////////////////////////////////////////////////////////////

bool g2device_info::is_authenticated(int channel) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_is_authenticated(_handle, channel);
}

bool g2device_info::is_IDR_X032(int channel) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_is_IDR_X032(_handle, channel);
}

bool g2device_info::is_IDR(int channel) const
{
    assert(_handle != G2HNULL && "g2client_device_info is not initialized");
    return g2_device_info_is_IDR(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2device_info::on_receive_product_info(G2HDEVICE_INFO handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2_PRODUCT_INFO* pi = (G2_PRODUCT_INFO*)(lparam);
    CALL_LISTENER(on_g2device_info_receive_product_info(handle, LOWORD(wparam), (G2DISCONNECT_REASON::TYPE)HIWORD(wparam), *pi));
    return 1L;
}

G2RESULT g2device_info::on_failed(G2HDEVICE_INFO handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2device_info_failed(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2device_info::on_canceled(G2HDEVICE_INFO handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2device_info_canceled(handle, wparam));
    return 1L;
}
