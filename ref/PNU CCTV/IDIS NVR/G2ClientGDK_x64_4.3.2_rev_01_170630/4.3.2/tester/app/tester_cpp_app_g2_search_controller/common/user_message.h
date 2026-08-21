// user_message.h : header file
//

#ifndef _COMMON_USER_MESSAGE_H_
#define _COMMON_USER_MESSAGE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

struct um_controller_ {
    enum MESSAGE {
        UM_CONNECTED = (WM_USER + 100),
        UM_DISCONNECTED,
        UM_LOAD_TIME_TABLE,
        UM_UPDATE_TIME_TABLE,
        UM_POST_PLAY_STOPED,

        UM_PLAY_GOTO_FIRST,
        UM_PLAY_GOTO_LAST,
        UM_RECEIVE_SCOPE_LIST,
        UM_RECEIVE_SPOT_LIST,
        UM_PLAY_SPEED_CHANGED,
        UM_SLIDER_CHANGED,
    };
};

struct um_runner_ {
    enum MESSAGE {
        UM_CONNECT_RELAY = (WM_USER + 100),
        UM_CONNECTED_RELAY,
        UM_DISCONNECTED_RELAY,

        UM_CONNECT_SEARCH_G2,
        UM_CONNECTED_SEARCH_G2,
        UM_DISCONNECTED_SEARCH_G2,
        UM_SCREEN_NO_IMAGE_LOADED_SEARCH_G2,
        UM_RECTIME_LOADED_SEARCH_G2,
        UM_NO_RECORDED_DATA_SEARCH_G2,
        UM_RECEIVE_COMMAND_BEGIN_SEARCH_G2,
    };
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_COMMON_USER_MESSAGE_H_
