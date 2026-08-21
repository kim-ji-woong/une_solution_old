// g2client_user.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_user.h"
#include "g2client_user_listener.h"

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
    g2_user_register_callback(G2USER_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2user_listener* ptr = ((g2user*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2user::g2user(void)
    : _listener(NULL)
{
    g2_user_set_UPARAM((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnect_entered);
    REGISTER_CALLBACK(on_login_cancelled);
    REGISTER_CALLBACK(on_login);
    REGISTER_CALLBACK(on_logout);
    REGISTER_CALLBACK(on_login_failed);
    REGISTER_CALLBACK(on_login_failed_from_dvrns);
    REGISTER_CALLBACK(on_set_authority);
    REGISTER_CALLBACK(on_modify);
}

g2user& g2user::get(void)
{
    static g2user singleton;
    return singleton;
}

//////////////////////////////////////////////////////////////////////////

void g2user::set_listener(g2user_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

bool g2user::is_connected(void)
{
    return g2_user_is_connected();
}

bool g2user::is_login(void)
{
    return g2_user_is_login();
}

bool g2user::is_logout(void)
{
    return g2_user_is_logout();
}

bool g2user::is_administrator(void)
{
    return g2_user_is_administrator();
}

bool g2user::is_authority(G2USER_AUTHORITY::TYPE type)
{
    return g2_user_is_authority(type);
}

//////////////////////////////////////////////////////////////////////////

bool g2user::network_info(G2NETWORK_INFO& info)
{
    return g2_user_get_network_info(&info);
}

std::wstring g2user::id(void)
{
    G2STRING_32 s = { 0 };
    return (g2_user_get_id(&s) && s._len != 0) ? s._string : L"";
}

std::wstring g2user::name(void)
{
    G2STRING_64 s = { 0 } ;
    return (g2_user_get_name(&s) && s._len != 0) ? s._string : L"";
}

std::wstring g2user::description(void)
{
    G2STRING_128 s = { 0 };
    return (g2_user_get_description(&s) && s._len != 0) ? s._string : L"";
}

//////////////////////////////////////////////////////////////////////////

std::wstring g2user::string_authority(G2USER_AUTHORITY::TYPE type)
{
    G2STRING_128 s = { 0 };
    g2_user_get_string_authority(type, &s);
    return (s._len != 0) ? s._string : L"";
}

std::wstring g2user::string_authority_desc(G2USER_AUTHORITY::TYPE type)
{
    G2STRING_256 s = { 0 };
    g2_user_get_string_authority_desc(type, &s);
    return (s._len != 0) ? s._string : L"";
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2user::on_connected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2user_connected());
    return 1L;
}

G2RESULT g2user::on_disconnect_entered(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2user_disconnect_entered((G2DISCONNECT_REASON::TYPE)wparam, (G2SERVICE_LOGIN_FAIL_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2user::on_login_cancelled(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2user_login_cancelled());
    return 1L;
}

G2RESULT g2user::on_login(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2user_login());
    return 1L;
}

G2RESULT g2user::on_logout(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2user_logout((G2DISCONNECT_REASON::TYPE)wparam, (G2SERVICE_LOGIN_FAIL_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2user::on_login_failed(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2NETWORK_INFO* ni = (G2NETWORK_INFO*)(wparam);
    G2LPARAM_LIST* list = (G2LPARAM_LIST*)(lparam);
    int* p = (int*)(list->_params);
    CALL_LISTENER(on_g2user_login_failed(*ni, (G2DISCONNECT_REASON::TYPE)p[0], (G2SERVICE_LOGIN_FAIL_REASON::TYPE)p[1]));
    return 1L;
}

G2RESULT g2user::on_login_failed_from_dvrns(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2NETWORK_INFO* ni = (G2NETWORK_INFO*)(wparam);
    CALL_LISTENER(on_g2user_login_failed_from_dvrns(*ni, (G2FEN_RESULT::TYPE)lparam));
    return 1L;
}

G2RESULT g2user::on_set_authority(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2user_set_authority());
    return 1L;
}

G2RESULT g2user::on_modify(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2user_modify());
    return 1L;
}
