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
    MAX_CONNECTIVE_CHANNEL = 64,
};

namespace invalid_ {
    enum TYPE {
        CHANNEL = -1,
        CHANNEL_RECONNECT = -2,
        CHANNEL_EXT = -1,
        HOST_MODE = -1,
        CAMERA_NUMBER = -1,
        DECODER_KEY = -1
    };
};

namespace play_menu_ {
    enum ID {
        CONTEXT_MOVE = 0,
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
    MAX_HOUR_DATA   = 512,//48,
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

        BACK_FASTEST_INFINITE   = -13,
        BACK_FASTEST_MORE       = -12,
        BACK_FASTEST            = -11,
        BACK_FASTER_MORE        = -10,
        BACK_FASTER             = -9,
        BACK_FAST_MORE          = -8,
        BACK_FAST               = -7,
        BACK_NORMAL_TRIPLE      = -6,
        BACK_NORMAL_TWICE_HALF  = -5,
        BACK_NORMAL_TWICE       = -4,
        BACK_NORMAL_HFAST       = -3,
        BACK_NORMAL             = -2,
        BACK_SLOW               = -1,
        PLAY_STOP               =  0,
        PLAY_SLOW               = +1,
        PLAY_NORMAL             = +2,
        PLAY_NORMAL_HFAST       = +3,
        PLAY_NORMAL_TWICE       = +4,
        PLAY_NORMAL_TWICE_HALF  = +5,
        PLAY_NORMAL_TRIPLE      = +6,
        PLAY_FAST               = +7,
        PLAY_FAST_MORE          = +8,
        PLAY_FASTER             = +9,
        PLAY_FASTER_MORE        = +10,
        PLAY_FASTEST            = +11,
        PLAY_FASTEST_MORE       = +12,
        PLAY_FASTEST_INFINITE   = +13,
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
