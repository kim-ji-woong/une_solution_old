using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2BACKUP_SAVER_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_receive_backup_site_result = 2,
            on_receive_record_channels = 3,
            on_receive_response_no_recorded_data = 4,
            on_receive_frame_data = 5,
            on_receive_notify_out_of_scope = 6,
            on_receive_notify_player_error = 7,
            on_receive_scope_list = 8,
            on_receive_no_recorded_data = 9,
            on_receive_clipcopy_size = 10,
            on_receive_clipcopy_data = 11,
            on_receive_clipcopy_canceled = 12,
            on_receive_clipcopy_set_password = 13,
            on_receive_clipcopy_job_started = 14,
            on_receive_clipcopy_job_finished = 15,

            CALLBACK_COUNT = 16
        }
    }
}
