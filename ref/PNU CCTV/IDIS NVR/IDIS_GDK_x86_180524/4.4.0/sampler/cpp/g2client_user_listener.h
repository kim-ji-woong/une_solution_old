// g2client_user_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_USER_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_USER_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_user.h>
#include <include/g2_define_fen.h>
#include <string>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2user_listener
{
public:
    virtual void on_g2user_connected(void) = 0;
    virtual void on_g2user_disconnect_entered(G2DISCONNECT_REASON::TYPE reason, G2SERVICE_LOGIN_FAIL_REASON::TYPE reason_user) = 0;
    virtual void on_g2user_login_cancelled(void) = 0;
    virtual void on_g2user_login(void) = 0;
    virtual void on_g2user_logout(G2DISCONNECT_REASON::TYPE reason, G2SERVICE_LOGIN_FAIL_REASON::TYPE reason_user) = 0;
    virtual void on_g2user_login_failed(const G2NETWORK_INFO& ni, G2DISCONNECT_REASON::TYPE reason, G2SERVICE_LOGIN_FAIL_REASON::TYPE reason_user) = 0;
    virtual void on_g2user_login_failed_from_dvrns(const G2NETWORK_INFO& ni, G2FEN_RESULT::TYPE reason) = 0;
    virtual void on_g2user_set_authority(void) = 0;
    virtual void on_g2user_modify(void) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_USER_LISTENER_H_
