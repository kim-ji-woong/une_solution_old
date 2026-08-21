using System;
using System.Collections.Generic;
using System.Text;

namespace GDK
{
    public struct G2RTP_CALLBACK
    {
        public enum TYPE
        {
            on_rtsp_connected_device = 0,
            on_rtsp_connected = 1,
            on_rtsp_disconnected = 2,
            on_rtp_connected = 3,
            on_receive_frame_data = 4,
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

            CALLBACK_COUNT = 15
        }
    }
}
