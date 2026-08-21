// g2client_callback.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_CALLBACK_H_
#define _G2_CLIENT_DLL_SAMPLER_CALLBACK_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_callback.h>
#include <utility>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2callback
{
public:
    g2callback(void);
    virtual ~g2callback(void);

private:
    G2HCALLBACK _handle;

public:
    void startup(int connections);
    void cleanup(void);

    G2HCALLBACK safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    void set_connection_info(int i, const wchar_t* address, unsigned short port);
    void set_dvrns_name(const wchar_t* name);
    void set_retry_count(int retry);
    void set_limit_count(int limit);

public:
    void notify_event(const G2EVENT_INFO& ei, const wchar_t* id, int num);
    void notify_event(int i, const G2EVENT_INFO& ei, const wchar_t* id, int num);

public:
    bool is_enable_notify(int i) const;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_CALLBACK_H_
