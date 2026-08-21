// g2_define_play_saver.h : header file
//

#ifndef _G2_DEFINE_PLAY_SAVER_H_
#define _G2_DEFINE_PLAY_SAVER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_play.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2PLAY_SAVER_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_receive_record_channels = 2,
        on_receive_frame_data = 3,
        on_receive_notify_out_of_scope = 4,
        on_receive_notify_player_error = 5,
        on_receive_scope_list = 6,
        on_receive_no_recorded_data = 7,
        on_receive_clipcopy_size = 8,
        on_receive_clipcopy_data = 9,
        on_receive_clipcopy_canceled = 10,
        on_receive_clipcopy_set_password = 11,
        on_receive_clipcopy_job_started = 12,
        on_receive_clipcopy_job_finished = 13,

        CALLBACK_COUNT = 14
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO {
    G2CLIPCOPY_SIZE_INFO _info;
    int _status;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_PLAY_SAVER_H_
