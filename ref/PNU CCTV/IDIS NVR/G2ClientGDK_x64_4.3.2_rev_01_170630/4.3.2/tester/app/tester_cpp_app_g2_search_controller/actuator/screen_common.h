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

struct play_speed_ {
    enum {
        STOP = 0,
        NORMAL = 10
    };
};

struct looper_ {
    enum MODE {
        UNDEFINED = -1,
        LIVE = 0,
        PLAY
    };
};

//////////////////////////////////////////////////////////////////////////

    } // !_namespace_screen
} // !_namespace_client

#endif // !_SCREEN_COMMON_H_
