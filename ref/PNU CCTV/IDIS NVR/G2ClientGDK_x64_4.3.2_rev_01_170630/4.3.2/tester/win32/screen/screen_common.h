// screen_common.h : header file
//

#ifndef _SCREEN_COMMON_H_
#define _SCREEN_COMMON_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {
    namespace screen {

//////////////////////////////////////////////////////////////////////////

struct margin_ {
    static const RECT CAMERA_RECT;
};

struct osd_size_ {
    static const SIZE SCALE_BASE;
    static const SIZE TEXT_IN;
    static const SIZE TITLE;
    static const SIZE DATETIME;
};

struct frame_max_ {
    enum {
        WIDTH = 1920,
        HEIGHT = 1200,
        SIZE = 1024 * 1024 * 1,
    };
};

struct count_ {
    enum {
        HOST_CAMERA = 32,
        CAMERA = 36,
        LIVE_STREAM = 36
    };
};

struct layout_reckon_ {
    enum MODE {
        CHANGE,
        PREV,
        NEXT
    };
};

struct layout_change_ {
    enum MODE {
        CHANGE,
        PREV,
        NEXT
    };
};

struct stream_buf_ {
    enum MODE {
        OFF = 0,
        ON,
        AUTO
    };
};

struct play_speed_ {
    enum {
        STOP = 0,
        NORMAL = 10
    };
};

struct camera_ {
    enum MODE {
        UNDEFINED = -1,
        WATCH = 0,
        SEARCH = 1
    };
};

struct looper_ {
    enum MODE {
        UNDEFINED = -1,
        LIVE = 0,
        PLAY
    };
};

struct looper_id_ {
    enum TYPE {
        PLAY_BEGIN = 0,
        PLAY_END = 3,
        LIVE_BEGIN = 4,
        LIVE_END = 7
    };
};

struct looper_count_ {
    enum TYPE {
        LIVE = 4,
        PLAY = 4,
        SCREEN = LIVE + PLAY
    };
};

struct layout_base_ {
    enum MODE {
        UNDEFINED = -1,
        PAGE = 0,
        CAMERA
    };
};

struct camera_status_ {
    enum TYPE {
        UNDEFINED = -1,
        DISABLE = 0,
        INACTIVATE,
        ENABLE,         // status that can draw image
        NO_VIDEO,
        COVERT_L1,
        COVERT_L2,
        NOT_CONNECTED,
        RECONNECTING,
        COUNT
    };
};

struct img_ratio_ {
    enum FACTOR {
        HALF = 0,
        X1,
        X2,
        X4,
        NORMAL,
        CONSTANT
    };
};

struct mode_ {
    enum TYPE {
        UNDEFINED = 0x0000,
        LIVE   = 0x0001,
        PLAY   = 0x0002,
    };
};

//////////////////////////////////////////////////////////////////////////

    } // !_namespace_screen
} // !_namespace_client

#endif // !_SCREEN_COMMON_H_
