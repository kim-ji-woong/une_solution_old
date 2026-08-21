// g2client_admin.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_USER_H_
#define _G2_CLIENT_DLL_SAMPLER_USER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_user.h>
#include <vector>
#include <string>

namespace client {
    class g2user_listener;

//////////////////////////////////////////////////////////////////////////

class g2user
{
protected:
    g2user(void);

public:
    static g2user& get(void);

private:
    g2user_listener* _listener;

public:
    void set_listener(g2user_listener* listener);

public:
    bool is_connected(void);
    bool is_login(void);
    bool is_logout(void);
    bool is_administrator(void);
    bool is_authority(G2USER_AUTHORITY::TYPE type);

    bool network_info(G2NETWORK_INFO& info);
    std::wstring id(void);
    std::wstring name(void);
    std::wstring description(void);

    std::wstring string_authority(G2USER_AUTHORITY::TYPE type);
    std::wstring string_authority_desc(G2USER_AUTHORITY::TYPE type);

protected:
    static G2RESULT G2CALLBACK on_connected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnect_entered(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_login_cancelled(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_login(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_logout(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_login_failed(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_login_failed_from_dvrns(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_set_authority(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_modify(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_USER_H_
