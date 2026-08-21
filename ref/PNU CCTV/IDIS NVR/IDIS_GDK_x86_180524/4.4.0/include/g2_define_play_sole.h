// g2_define_play_sole.h : header file
//

#ifndef _G2_DEFINE_PLAY_SOLE_H_
#define _G2_DEFINE_PLAY_SOLE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_play.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2PLAY_SOLE_CALLBACK {
    enum TYPE {
        on_get_options_search_base = 0,
        on_get_options_player = 1,
        on_connected = 2,
        on_disconnected = 3,
        on_probe_frameload = 4,
        on_command_begin = 5,
        on_command_end = 6,
        on_play_speed_changed = 7,
        on_frame_loaded = 8,
        on_frame_not_found = 9,
        on_out_of_scope = 10,
        on_player_set_scope = 11,
        on_player_error = 12,
        on_get_rollback_info = 13,
        on_get_frame_buf_status = 14,
        on_receive_text_in = 15,
        on_receive_snapshot_frame = 16,
        on_receive_snapshot_begin = 17,
        on_receive_snapshot_end = 18,
        on_receive_record_channel = 19,
        on_receive_record_time_info_loaded = 20,
        on_receive_record_time_info_load_end = 21,
        on_receive_frame_channelset = 22,
        on_receive_spot_list = 23,
        on_receive_scope_list = 24,
        on_receive_recorded_scope = 25,
        on_receive_frame_spot_list = 26,
        on_error_occur_socket = 27,

        CALLBACK_COUNT = 28
    };
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_PLAY_SOLE_H_
