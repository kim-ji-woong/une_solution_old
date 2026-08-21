using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2WATCH_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_receive_frame_data = 2,
            on_recieve_audio_data = 25,
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

            CALLBACK_COUNT = 26
        }
    }
}
