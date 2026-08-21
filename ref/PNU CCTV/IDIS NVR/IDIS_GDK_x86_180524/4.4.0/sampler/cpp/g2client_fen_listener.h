// g2client_fen_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_FEN_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_FEN_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_fen.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2fen_listener
{
public:
    virtual void on_g2fen_nat_type_discovered(G2NAT_INFO::TYPE type, const G2NAT_INFO& ni) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_FEN_LISTENER_H_
