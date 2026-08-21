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
        UM_SCREEN_CAMERA_CHANGED = (WM_USER + 100),
        UM_RECEIVE_SCOPE_LIST,
        UM_RECEIVE_SPOT_LIST,
        UM_RECEIVE_EXTERNAL_TANGO_LIST,
        UM_ENABLE_CONTROLS,
        UM_DISCONNECTED,
        UM_LOAD_TIME_TABLE,
        UM_UPDATE_TIME_TABLE,
        UM_POST_PLAY_STOPED,

        UM_PTZ_RECEVIE_MENU,
        UM_PTZ_RECEVIE_PRESET,
    };
};

struct um_runner_ {
    enum MESSAGE {
        UM_DROP_FROM_SCREEN = (WM_USER + 100),
        UM_DROP_INFO_SECURE,
        UM_CONNECTED_LIVE,
        UM_CONNECTED_PLAY,
        UM_CONNECTED_BACKUP,
        UM_CONNECTED_SEARCH,
        UM_CONNECTED_SEARCH_G2,
        UM_DISCONNECTED_LIVE,
        UM_DISCONNECTED_PLAY,
        UM_DISCONNECTED_BACKUP,
        UM_DISCONNECTED_SEARCH,
        UM_DISCONNECTED_SEARCH_G2,

        UM_RECEIVE_RECORDED_DATE_SEARCH,

        UM_SCREEN_NO_IMAGE_LOADED_PLAY,
        UM_SCREEN_NO_IMAGE_LOADED_BACKUP,
        UM_SCREEN_NO_IMAGE_LOADED_SEARCH,
        UM_SCREEN_NO_IMAGE_LOADED_SEARCH_G2,

        UM_RECTIME_LOADED_SEARCH_G2,
        UM_NO_RECORDED_DATA_SEARCH_G2,
        UM_RECEIVE_COMMAND_BEGIN_SEARCH_G2,
    };
};

struct um_log_ {
    enum MESSAGE {
        UM_LOG_UPDATE = (WM_USER + 100),
        UM_LOG_LOAD_FAIL,
        UM_CONNECTED_SERVICE,
        UM_DISCONNECTED_SERVICE,
        UM_USER_LOGIN,
        UM_USER_LOGOUT
    };
};

struct um_clip_copy_ {
    enum MESSAGE {
        UM_CONNECTED = (WM_USER + 100),
        UM_DISCONNECTED,
        UM_START_SAVE_VIDEO,
        UM_STOP_SAVE_VIDEO,
        UM_CANCELED_CLIPCOPY,
        UM_COMPLETE_CLIPCOPY,
        UM_UPDATE_CLIPCOPY,
    };
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_COMMON_USER_MESSAGE_H_
