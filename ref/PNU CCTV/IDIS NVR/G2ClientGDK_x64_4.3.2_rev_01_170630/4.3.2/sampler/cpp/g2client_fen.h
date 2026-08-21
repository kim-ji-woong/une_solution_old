// g2client_fen.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_FEN_H_
#define _G2_CLIENT_DLL_SAMPLER_FEN_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_fen.h>

namespace client {
    class g2fen_listener;

//////////////////////////////////////////////////////////////////////////

class g2fen
{
protected:
    g2fen(void);

public:
    static g2fen& get(void);

private:
    g2fen_listener* _listener;

public:
    void startup(unsigned int connections, unsigned int count_direct, unsigned int count_hole_punching, unsigned int count_relay, const wchar_t* path_history);
    void cleanup(void);
    void set_listener(g2fen_listener* listener);
    void set_gateway(const wchar_t* address, unsigned short port);
    void set_secure_local(void);
    void set_external(bool flag);
    void set_external_proxy(const wchar_t* address, unsigned short port);
    void set_external_gateway(const wchar_t* address, unsigned short port);
    void set_external_nat(const G2NAT_INFO& ni);
    void set_external_activate(bool activate);

public:
    void join_service(unsigned int elapse, bool pump_message_waitable = true);

public:
    bool is_startup(void) const;
    bool is_activate(void) const;
    bool is_failed_secure_nat_info(void) const;

public:
    int get_nat_type(void) const;

protected:
    static G2RESULT G2CALLBACK on_nat_type_discovered(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_FEN_H_
