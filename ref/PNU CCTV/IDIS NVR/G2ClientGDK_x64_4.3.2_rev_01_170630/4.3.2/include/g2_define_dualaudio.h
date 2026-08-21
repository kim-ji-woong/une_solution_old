// g2_define_dualaudio.h : header file
//

#ifndef _G2_DEFINE_DUALAUDIO_H_
#define _G2_DEFINE_DUALAUDIO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2DUALAUDIO_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_receive_audio_in_opened = 2,
        on_receive_audio_in_closed = 3,
        on_receive_audio_out_opened = 4,
        on_receive_audio_out_closed = 5,
        on_require_select_channel = 6,
        on_require_reconnect_imp = 7,

        CALLBACK_COUNT = 8
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2DUALAUDIO_DRIVE {
    enum MODE {
        DEFAULT = 0,
        IAUD,
        G711,
        G726,
        IDR,
        NAIAD
    };
};

struct G2DUALAUDIO_SELECT_CHANNEL {
    G2GUID _site;
    int    _audio_count;
    int*   _number;
};

struct G2DUALAUDIO_OPEN_MODE {
    enum MODE {
        AUDIO_INVALID = -1,
        AUDIO_IN  = 0x0001,
        AUDIO_OUT = 0x0002
    };
};

struct G2DUALAUDIO_RESERVE {
    G2GUID   _site;
    int      _host_id;
    int      _host_camera;
    int      _pane;
    bool     _control_from_me;
    bool     _broadcast;
    G2UPARAM _data;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_DUALAUDIO_H_
