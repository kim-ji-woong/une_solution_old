// g2client_fen.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_fen.h"
#include "g2client_fen_listener.h"

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
    g2_fen_register_callback(G2FEN_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2fen_listener* ptr = ((g2fen*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2fen::g2fen(void)
    : _listener(NULL)
{
    g2_fen_set_UPARAM((G2UPARAM)(this));

    REGISTER_CALLBACK(on_nat_type_discovered);
}

g2fen& g2fen::get(void)
{
    static g2fen s_singleton;
    return s_singleton;
}

//////////////////////////////////////////////////////////////////////////

void g2fen::startup(unsigned int connections, unsigned int count_direct, unsigned int count_hole_punching, unsigned int count_relay, const wchar_t* path_history)
{
    g2_fen_startup(connections, count_direct, count_hole_punching, count_relay, path_history);
}

void g2fen::cleanup(void)
{
    g2_fen_cleanup();
}

void g2fen::set_listener(g2fen_listener* listener)
{
    _listener = listener;
}

void g2fen::set_gateway(const wchar_t* address, unsigned short port)
{
    g2_fen_set_gateway(address, port);
}

void g2fen::set_external(bool flag)
{
    g2_fen_set_external(flag);
}

void g2fen::set_external_proxy(const wchar_t* address, unsigned short port)
{
    g2_fen_set_external_proxy(address, port);
}

void g2fen::set_external_gateway(const wchar_t* address, unsigned short port)
{
    g2_fen_set_external_gateway(address, port);
}

void g2fen::set_external_nat(const G2NAT_INFO& ni)
{
    g2_fen_set_external_nat(&ni);
}

void set_external_activate(bool activate)
{
    g2_fen_set_external_activate(activate);
}

void g2fen::set_secure_local(void)
{
    g2_fen_set_secure_local();
}

//////////////////////////////////////////////////////////////////////////

void g2fen::join_service(unsigned int elapse, bool pump_message_waitable /*= true*/)
{
    g2_fen_service_join(elapse, pump_message_waitable);
}

//////////////////////////////////////////////////////////////////////////

bool g2fen::is_startup(void) const
{
    return g2_fen_is_startup();
}

bool g2fen::is_activate(void) const
{
    return g2_fen_is_activate();
}

bool g2fen::is_failed_secure_nat_info(void) const
{
    return g2_fen_is_failed_secure_nat_info();
}

//////////////////////////////////////////////////////////////////////////

int g2fen::get_nat_type(void) const
{
    return g2_fen_get_nat_type();
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2fen::on_nat_type_discovered(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2NAT_INFO* p = (G2NAT_INFO*)(lparam);
    CALL_LISTENER(on_g2fen_nat_type_discovered((G2NAT_INFO::TYPE)p->_type, *p));
    return 1L;
}
