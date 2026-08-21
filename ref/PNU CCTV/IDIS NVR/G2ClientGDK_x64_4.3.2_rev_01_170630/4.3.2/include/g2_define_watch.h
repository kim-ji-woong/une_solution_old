// g2_define_watch.h : header file
//

#ifndef _G2_DEFINE_WATCH_H_
#define _G2_DEFINE_WATCH_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"
#include "g2_define_product.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2WATCH_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_receive_frame_data = 2,
        on_receive_event = 3,
        on_receive_device_status = 4,
        on_receive_ptz_preset = 5,
        on_receive_ptz_menu = 6,
        on_receive_camera_title_idr = 7,
        on_receive_text_in = 8,
        on_receive_network_camera_information = 9,
        on_receive_audio_out_not_available = 10,
        on_receive_command_result_control_color_status = 11,
        on_receive_command_result_control_color = 12,
        on_receive_command_result_control_ptz_status = 13,
        on_receive_command_result_control_ptz = 14,
        on_receive_network_alarm_result = 15,
        on_receive_elevator_status_info_response = 24,
        on_receive_instant_recording_start = 21,
        on_receive_instant_recording_stop = 22,
        on_receive_instant_recording_status = 23,
        on_audio_streaming_started = 16,
        on_audio_streaming_stopped = 17,
        on_audio_capturing_started = 18,
        on_audio_capturing_stopped = 19,
        on_probe_session_profile = 20,

        CALLBACK_COUNT = 25
    };
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_WATCH_H_
