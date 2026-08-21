// g2client_callback.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_callback.h"

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

g2callback::g2callback(void)
    : _handle(G2HNULL)
{
    _handle = g2_callback_initialize((G2UPARAM)(this));
}

g2callback::~g2callback(void)
{
    g2_callback_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2callback::startup(int connections)
{
    assert(_handle != G2HNULL && "g2client_callback is not initialized");
    g2_callback_startup(_handle, connections);
}

void g2callback::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_callback is not initialized");
    g2_callback_cleanup(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2callback::set_connection_info(int i, const wchar_t* address, unsigned short port)
{
    g2_callback_set_connection_info(_handle, i, address, port);
}

void g2callback::set_dvrns_name(const wchar_t* name)
{
    g2_callback_set_dvrns_name(_handle, name);
}

void g2callback::set_retry_count(int retry)
{
    g2_callback_set_retry_count(_handle, retry);
}

void g2callback::set_limit_count(int limit)
{
    g2_callback_set_limit_count(_handle, limit);
}

//////////////////////////////////////////////////////////////////////////

void g2callback::notify_event(const G2EVENT_INFO& ei, const wchar_t* id, int num)
{
    g2_callback_notify_event_all(_handle, &ei, id, num);
}

void g2callback::notify_event(int i, const G2EVENT_INFO& ei, const wchar_t* id, int num)
{
    g2_callback_notify_event(_handle, i, &ei, id, num);
}

//////////////////////////////////////////////////////////////////////////

bool g2callback::is_enable_notify(int i) const
{
    return g2_callback_is_enable_notify(_handle, i);
}
