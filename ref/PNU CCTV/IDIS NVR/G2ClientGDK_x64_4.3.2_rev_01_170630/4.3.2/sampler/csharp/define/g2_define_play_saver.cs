using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2PLAY_SAVER_CALLBACK
    {
        public enum TYPE
        {
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
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO
    {
        public G2CLIPCOPY_SIZE_INFO _info;
        public int _status;
    };
}
