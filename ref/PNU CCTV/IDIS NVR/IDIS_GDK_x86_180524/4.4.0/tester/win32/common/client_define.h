// client_define.h : header file
//

#ifndef _COMMON_CLIENT_DEFINE_H_
#define _COMMON_CLIENT_DEFINE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

enum {
    MAX_SCREEN_CAMERA_COUNT = 64,
    MAX_SCREEN_DECODER_COUNT = 64,
    MAX_CONNECTIVE_CHANNEL = 64,

    MIN_FRAME_SIZE_CX = 800,
    MIN_FRAME_SIZE_CY = 600,

    REQUEST_LOG_COUNT = 100,
};

namespace drag_id_ {
    enum ID {
        DRAG_FROM_DEVICE_TREE = 127,
        DRAG_FROM_INSTANT_EVENT = 130
    };
};

namespace connect_type_ {
    enum TYPE {
        UNDEFINED = -1,
        RAS = 0,
        NEX
    };
}

namespace connect_mode_ {
    enum MODE {
        UNDEFINED = -1,
        LIVE = 0,   // streaming service
        WATCH,
        RTP,
        PLAY,       // recording service
        SEARCH,
        SEARCH_G2,
        STATUS,
        COUNT
    };
};

namespace screen_mode_ {
    enum MODE {
        UNDEFINED = -1,
        LIVE = 0,
        PLAY,
        SEARCH,
    };
};

namespace invalid_ {
    enum TYPE {
        CHANNEL = -1,
        CHANNEL_RECONNECT = -2,
        CHANNEL_EXT = -1,
        HOST_ID = -1,
        HOST_MODE = -1,
        CAMERA_NUMBER = -1,
        DECODER_KEY = -1
    };
};

namespace max_ {
    enum TYPE {
        HOST_CAMERA_RAS = 32,
        MULTI_STREAM = 8,
    };
    enum LEN {
        SITE_NAME = 32,
        USER_ID_RAS = 16,
        USER_ID  = 32,
        PASSWORD_RAS = 8,
        PASSWORD = 32,
        PASSWORD_CLIPCOPY = 16,
    };
    enum VAL {
        REQUIRED_SPACE_4 = 4096000000 // 4gbyte
    };
};

namespace screen_formatter_ {
    enum LAYOUT {
        LAYOUT_UNDEFINED = -1,
        LAYOUT_1x1 = 0,
        LAYOUT_2x2,
        LAYOUT_3x3,
        LAYOUT_4x4,
        LAYOUT_5x5,
        LAYOUT_6x6,

        LAYOUT_COUNT
    };
};

namespace screen_menu_ {
    enum ID {
        CONTEXT_CONNECTIVE = 0,
        CONTEXT_CAMERA
    };
};

namespace play_menu_ {
    enum ID {
        CONTEXT_GOTO = 0,
        CONTEXT_MOVE,
        CONTEXT_DATA_SOURCE
    };
};

namespace view_ {
    enum TYPE {
        VIEW_UNDEFINED = -1,
        VIEW_SCREEN = 0,
        VIEW_LOG
    };
};

namespace event_condition_ {
    enum TYPE {
        UNDEFINED = -1,

        BEGIN_EVENT = 0,
        ALARM_IN = BEGIN_EVENT,
        MOTION_DETECTION,
        VIDEO_LOSS,
        VIDEO_BLIND,
        AUDIO_DETECTION,
        TRIPZONE,
        TAMPER,
        END_EVENT,

        ///////////////////////////////

        OBJECT_DETECTION = END_EVENT,
        TEXT_IN,
        NETWORK_ALARM_IN,

        ///////////////////////////////

        EVENT_COUNT
    };
};

////////////////////////////////////////////////////////////

namespace search {
enum {
    ALL_CHANNEL = -1
};

enum {
    MAX_DAY_DATA    = -1,
    MAX_TIME_SCOPE  = 100,
    MAX_HOUR_DATA   = 48,
    MIN_REMAIN_HOUR_DATA = 16,
    MIN_REQUEST_REC_HOUR = 2,
};

namespace direction_ {
    enum TYPE {
        DIRECTION_LEFT = -1,
        DIRECTION_RIGHT = 1
    };
};

namespace request_ {
    enum TYPE {
        REQUEST_UNKNOWN = 0,
        REQUEST_INIT,
        REQUEST_BALANCE_LEFT,
        REQUEST_BALANCE_RIGHT,
        REQUEST_OVERWRITE_LEFT,
        REQUEST_OVERWRITE_RIGHT,
    };
};

namespace speed_ {
    enum FACTOR {
        PLAY_SPEED_UNDEFINED = _I16_MAX,

        BACK_FASTEST_INFINITE = -13,
        BACK_FASTEST_MORE = -12,
        BACK_FASTEST      = -11,
        BACK_FASTER_MORE  = -10,
        BACK_FASTER       = -9,
        BACK_FAST_MORE    = -8,
        BACK_FAST         = -7,
        BACK_NORMAL_INFINITE = -6,
        BACK_NORMAL_TRIPLE = -5,
        BACK_NORMAL_TWICE = -4,
        BACK_NORMAL_HFAST = -3,
        BACK_NORMAL       = -2,
        BACK_SLOW         = -1,
        PLAY_STOP         =  0,
        PLAY_SLOW         = +1,
        PLAY_NORMAL       = +2,
        PLAY_NORMAL_HFAST = +3,
        PLAY_NORMAL_TWICE = +4,
        PLAY_NORMAL_TRIPLE = +5,
        PLAY_NORMAL_INFINITE = +6,
        PLAY_FAST         = +7,
        PLAY_FAST_MORE    = +8,
        PLAY_FASTER       = +9,
        PLAY_FASTER_MORE  = +10,
        PLAY_FASTEST      = +11,
        PLAY_FASTEST_MORE = +12,
        PLAY_FASTEST_INFINITE = +13,
    };

    enum BOUNDARY {
        BOUND_LOWER = BACK_FASTEST_INFINITE,
        BOUND_UPPER = PLAY_FASTEST_INFINITE,
    };
};

}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_COMMON_CLIENT_DEFINE_H_
