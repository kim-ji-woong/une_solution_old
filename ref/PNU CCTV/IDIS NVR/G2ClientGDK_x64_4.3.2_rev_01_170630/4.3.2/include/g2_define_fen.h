// g2_define_fen.h : header file
//

#ifndef _G2_DEFINE_FEN_H_
#define _G2_DEFINE_FEN_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2FEN_CALLBACK {
    enum TYPE {
        on_nat_type_discovered = 0,

        CALLBACK_COUNT = 1
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2NAT_INFO {
    enum VERSION {
        G2NATINFO_1_0 = 0x00000100,
        G2NATINFO = G2NATINFO_1_0,
    };
    enum TYPE {
        TYPE_UNKNOWN = 0,
        TYPE_OPEN_INTERNET,
        TYPE_FULL_CONE,
        TYPE_RESTRICTED_CONE,
        TYPE_PORT_RESTRICTED_CONE,
        TYPE_SYMMETRIC,
        TYPE_SYMMETRIC_UDP_FIREWALL,
        TYPE_UDP_BLOCKED,
        TYPE_DISCOVERY_FAILED,
        TYPE_MOBILE
    };
    enum PORT_MAPPING {
        PORT_MAPPING_NONE = 0x00000000,
        PORT_FORWARDING   = 0x00000001,
        UPNP              = 0x00000002,
        NAT_PMP           = 0x00000004,
    };

    unsigned int  _version;
    unsigned char _type;
    unsigned char _port_mapping;
};

struct G2FEN_CONNECTION {
    enum TYPE {
        UNKNOWN = 0,
        TCP_DIRECT_EXTERNAL = 1,
        TCP_DIRECT_INTERNAL = 2,
        UDP_HOLE_PUNCHING = 3,
        VIA_RELAY_SERVICE = 4
    };
};

struct G2FEN_RESULT {
    enum TYPE {
        NOT_ERROR = 0,
        NETWORK_ERROR = NOT_ERROR + 1,
        NETWORK_INITIALIZE_ERROR = NOT_ERROR + 2,
        NETWORK_PROTOCOL_ERROR = NOT_ERROR + 3,
        INVALID_PACKET = NOT_ERROR + 4,
        INVALID_PACKET_VERSION = NOT_ERROR + 5,
        NETWORK_INITIALIZE_ERROR_HELPDESK = NOT_ERROR + 6,
        NETWORK_INITIALIZE_ERROR_NOTIFY = NOT_ERROR + 7,
        NETWORK_INITIALIZE_ERROR_MYIPADDRESS = NOT_ERROR + 8,
        QUERY_ERROR = NOT_ERROR + 200,
        NO_NAME_ERROR = QUERY_ERROR + 1,
        NAME_TABLE_RETRIEVE_ERROR = QUERY_ERROR + 2,
        NEW_DDNS_RESPONSE = QUERY_ERROR + 3,
        REMOVE_FAIL = QUERY_ERROR + 4
    };
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_FEN_H_
