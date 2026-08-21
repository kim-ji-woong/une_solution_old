// g2_define_app_frame_relay_server.h : header file
//

#ifndef _G2_DEFINE_APP_FRAME_RELAY_SERVER_H_
#define _G2_DEFINE_APP_FRAME_RELAY_SERVER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2APP_FRAME_RELAY_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected,
        on_receive_request_site_product_info,

        CALLBACK_COUNT
    };
};

struct G2APP_FRAME_RELAY_SERVER_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected,
        on_receive_site_connected,
        on_receive_site_disconnected,
        on_receive_site_product_info,
        on_receive_site_frame_data,

        CALLBACK_COUNT
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2APP_FRAME_RELAY_SERVER_OPTION {
    int _connections;
    int _threads;
    int _send_queue_size;
    unsigned int _tick_out;
    unsigned short _port;
    bool _iocp;
    bool _realloc;
};

struct G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION {
    G2NETWORK_INFO _server_ni;
    G2NETWORK_INFO _device_ni;
    G2CHANNEL_SET  _channels;
    G2STRING_64  _site;
    G2STRING_128 _title;
    G2POINT _startup_pos;
    bool _startup_pos_use;
    bool _no_top_most;
    bool _no_delete_file;
    bool _no_port_unity;
    int  _message_duration;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_APP_FRAME_RELAY_SERVER_H_
