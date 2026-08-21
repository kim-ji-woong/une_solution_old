// g2_define_user.h : header file
//

#ifndef _G2_DEFINE_USER_H_
#define _G2_DEFINE_USER_H_

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2USER_CALLBACK {
    enum TYPE {
        on_connected = 0,   // administration service channel
        on_disconnect_entered = 1,
        on_login_cancelled = 2,
        on_login = 3,
        on_logout = 4,
        on_login_failed = 5,
        on_login_failed_from_dvrns = 6,
        on_set_authority = 7,
        on_modify = 8,

        CALLBACK_COUNT = 9
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2USER_AUTHORITY {
    enum TYPE {
        AUTHORITY_INVALID = -1,
        AUTHORITY_SETUP_STORAGE = 1,
        AUTHORITY_SETUP_CONNECT = 10,
        AUTHORITY_SERVICE_SETUP = 50,
        AUTHORITY_DEVICE_SETUP = 100,
        AUTHORITY_DEVICE_REMOTE_SETUP = 110,
        AUTHORITY_DEVICE_REMOTE_UPGRADE = 120,
        AUTHORITY_SCHEDULE_SETUP = 300,
        AUTHORITY_LIVE = 400,
        AUTHORITY_PTZ,
        AUTHORITY_AUDIO_IN,
        AUTHORITY_AUDIO_OUT,
        AUTHORITY_INSTANT_RECORDING,
        AUTHORITY_LIVE_IMAGE_PROCESSING,
        AUTHORITY_LIVE_IMAGE_EXPORT,
        AUTHORITY_LIVE_COLOR_CONTROL = 410,
        AUTHORITY_SEARCH = 500,
        AUTHORITY_SAVE_AVI,
        AUTHORITY_SAVE_CLIPCOPY,
        AUTHORITY_SEARCH_IMAGE_PROCESSING,
        AUTHORITY_SEARCH_IMAGE_EXPORT,
        AUTHORITY_SEARCH_VIDEO_ANALYSIS,
        AUTHORITY_EVENT_SEARCH = 600,
        AUTHORITY_EVENT_SEARCH_EXPORT,
        AUTHORITY_DEVICE_HEALTH_MONITORING = 700,
        AUTHORITY_ALARM_OUT_CONTROL = 800,
        AUTHORITY_LOG_VIEW = 900,
        AUTHORITY_LOG_EXPORT,
        AUTHORITY_EVENT_HISTORY_VIEW = 1000,
        AUTHORITY_EVENT_HISTORY_EXPORT,
        AUTHORITY_DEVICE_STATUS_MONITORING = 1100,
        AUTHORITY_SHOW_DEVICE_AND_GROUP = 1300,
        AUTHORITY_CREATE_USER_LAYOUT = 1400,
        AUTHORITY_TEXT_IN = 1500,
        AUTHORITY_MAX
    };
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_USER_H_
