// g2_define_rtp.h : header file
//

#ifndef _G2_DEFINE_RTP_H_
#define _G2_DEFINE_RTP_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2RTP_CALLBACK {
    enum TYPE {
        on_rtsp_connected_device = 0,
        on_rtsp_connected = 1,
        on_rtsp_disconnected = 2,
        on_rtp_connected = 3,
        on_receive_frame_data = 4,
        on_receive_audio_data = 15,
        on_receive_event = 5,
        on_receive_device_status = 6,
        on_receive_ptz_preset = 7,
        on_receive_ptz_menu = 8,
        on_receive_audio_out_not_available = 9,
        on_audio_streaming_started = 10,
        on_audio_streaming_stopped = 11,
        on_audio_capturing_started = 12,
        on_audio_capturing_stopped = 13,
        on_probe_session_profile = 14,

        CALLBACK_COUNT = 16
    };
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_RTP_H_
