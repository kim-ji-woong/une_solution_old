using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2BACKUP_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_receive_backup_site_result = 2,
            on_receive_record_channels = 3,
            on_receive_record_time_info_load = 4,
            on_receive_record_time_info_load_end = 5,
            on_receive_response_no_recorded_data = 6,
            on_receive_frame_data = 7,
            on_receive_text_in = 8,
            on_receive_notify_command_begin = 9,
            on_receive_notify_command_end = 10,
            on_receive_notify_play_speed_changed = 11,
            on_receive_notify_frame_not_found = 12,
            on_receive_notify_out_of_scope = 13,
            on_receive_notify_player_error = 14,
            on_receive_scope_list = 15,
            on_receive_spot_list = 16,
            on_receive_no_recorded_data = 17,
            on_receive_event_log_load = 18,
            on_receive_event_log_load_end = 19,
            on_receive_event_log_load_fail = 20,
            on_receive_event_log_load_stop = 21,
            on_receive_text_in_log_load = 22,
            on_receive_text_in_log_load_end = 23,
            on_receive_text_in_log_load_fail = 24,
            on_receive_text_in_log_load_stop = 25,
            on_receive_service_log_load = 26,
            on_receive_service_log_load_end = 27,
            on_receive_service_log_load_fail = 28,
            on_receive_service_log_load_stop = 29,
            on_require_prepare_rollback = 30,
            on_probe_session_profile = 31,

            on_receive_debug_log_load = 32,
            on_receive_debug_log_load_end = 33,
            on_receive_debug_log_load_fail = 34,
            on_receive_debug_log_load_stop = 35,

            CALLBACK_COUNT = 36
        }
    }

    public struct G2BACKUP_QUERY
    {
        public enum MODE
        {
            UNDEFINED = -1,
            EVENT = 0,
            TEXT_IN
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2BACKUP_CHANNEL_INFO
    {
        public G2GUID _guid;
        public int _channelext;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2BACKUP_SITE_RESULT
    {
        public enum RESULT
        {
            UNKNOWN_RESULT = 0,
            SITE_OK,
            FAIL_INIT_TANGO,
            FAIL_CLIPCOPY
        }

        public G2GUID _service;
        public G2GUID _site;
        public uint _result;

        public RESULT result { get { return (RESULT)_result; } }
    }
}
