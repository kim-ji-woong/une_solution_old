using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace GDK
{
    public struct G2PLAY_SOLE_CALLBACK
    {
        public enum TYPE
        {
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
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PLAY_SOLE_CHANNEL_MAP
    {
        public enum CONST
        {
            MAX_CHANNEL_COUNT = 256
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.MAX_CHANNEL_COUNT)]
        public int[] _first;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.MAX_CHANNEL_COUNT)]
        public int[] _second;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PLAY_SOLE_OPTIONS_MAP_DATA
    {
        public uint _instance_id;
        [MarshalAs(UnmanagedType.U1)]
        public bool _use;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PLAY_SOLE_BASE_OPTIONS
    {
        public uint _use_player_count;
        public uint _use_rec_time_loader_count;
        public uint _use_text_in_loader_count;
        public uint _use_text_search_count;
        public uint _use_clip_copy_count;
        public uint _use_event_log_loader_count;
        public uint _use_segment_finder_count;
        public uint _use_channel_finder_count;
        public uint _use_snapshot_loader_count;
        public uint _use_gps_data_loader_count;
        public uint _use_gps_data_measurer_count;
        public uint _use_gps_data_exporter_count;
        public uint _use_gps_location_loader_count;
        public uint _use_backup_data_loader_count;
        public uint _use_scope_loader_count;
        [MarshalAs(UnmanagedType.U1)]
        public bool _force_segment_less_search;
    }
}