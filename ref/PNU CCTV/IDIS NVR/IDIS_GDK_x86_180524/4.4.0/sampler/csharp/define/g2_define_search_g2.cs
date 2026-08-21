using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2SEARCH_G2_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_query_options_search_base = 2,
            on_query_options_player = 3,
            on_receive_record_time_info_load = 4,
            on_receive_record_time_info_load_end = 5,
            on_receive_frame_data = 6,
            on_receive_text_in = 7,
            on_receive_event = 8,
            on_receive_notify_command_begin = 9,
            on_receive_notify_command_end = 10,
            on_receive_notify_play_speed_changed = 11,
            on_receive_notify_frame_not_found = 12,
            on_receive_notify_out_of_scope = 13,
            on_receive_notify_get_rollback_info = 14,
            on_receive_notify_player_error = 15,
            on_receive_event_log_load_end = 16,
            on_receive_event_log_load_stop = 17,
            on_receive_text_in_log_load_end = 18,
            on_receive_text_in_log_load_stop = 19,
            on_receive_scope_list = 20,
            on_receive_spot_list = 21,
            on_receive_no_recorded_data = 22,
            on_receive_db_info = 23,
            on_receive_db_info_external = 24,
            on_receive_db_selected = 25,
            on_receive_virtual_channelmap = 26,
            on_require_prepare_rollback = 27,
            on_probe_session_profile = 28,

            on_sole_connected = 29,
            on_sole_disconnected = 30,
            on_sole_query_options_player = 31,
            on_sole_receive_record_time_info_load = 32,
            on_sole_receive_record_time_info_load_end = 33,
            on_sole_receive_frame_data = 34,
            on_sole_receive_text_in = 35,
            on_sole_receive_event = 36,
            on_sole_receive_notify_command_begin = 37,
            on_sole_receive_notify_command_end = 38,
            on_sole_receive_notify_play_speed_changed = 39,
            on_sole_receive_notify_frame_not_found = 40,
            on_sole_receive_notify_out_of_scope = 41,
            on_sole_receive_notify_get_rollback_info = 42,
            on_sole_receive_notify_player_error = 43,
            on_sole_receive_scope_list = 44,
            on_sole_receive_spot_list = 45,
            on_sole_receive_no_recorded_data = 46,
            on_sole_require_prepare_rollback = 47,

            on_saver_connected = 48,
            on_saver_disconnected = 49,
            on_saver_receive_frame_data = 50,
            on_saver_receive_notify_out_of_scope = 51,
            on_saver_receive_notify_get_rollback_info = 52,
            on_saver_receive_notify_player_error = 53,
            on_saver_receive_scope_list = 54,
            on_saver_receive_no_recorded_data = 55,
            on_saver_receive_clipcopy_size = 56,
            on_saver_receive_clipcopy_data = 57,
            on_saver_receive_clipcopy_set_password = 58,
            on_saver_receive_clipcopy_canceled = 59,
            on_saver_receive_clipcopy_job_started = 60,
            on_saver_receive_clipcopy_job_finished = 61,
            on_saver_receive_clipcopy_section_begin = 62,
            on_saver_receive_clipcopy_section_end = 63,

            CALLBACK_COUNT = 64
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_OPTIONS_SEARCH_BASE
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool segment_less;
        [MarshalAs(UnmanagedType.U1)]
        public bool playable_sole;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_OPTIONS_PLAYER
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool _play_fast_all_frames;
        [MarshalAs(UnmanagedType.U1)]
        public bool _load_event_on_player;
        [MarshalAs(UnmanagedType.U1)]
        public bool _load_text_in_on_player;
        [MarshalAs(UnmanagedType.U1)]
        public bool _move_to_load_all_channels;
        [MarshalAs(UnmanagedType.U1)]
        public bool _move_to_load_all_channels_parallel;
        public int _precision_tick_tolerance;   // milliseconds (default 0)
        public int _precision_event_interval;   // seconds (default 2)
        public int _key_lookup_min_threshold_for_move_to;   // seconds
        public int _key_lookup_max_threshold_for_move_to;   // seconds
        public int _key_lookup_threshold_for_backward_play; // seconds
    }

    public struct G2SEARCH_G2_QUERY
    {
        public enum MODE
        {
            UNDEFINED = -1,
            EVENT = 0,
            TEXT_IN
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_REMOTE_DB
    {
        public enum STORAGE
        {
            NOT_USING = 0,
            RECORD,
            ARCHIVE,
            CLIP_COPY,
            EXTERNAL
        }
        public enum DB_SELECT_RESULT
        {
            SELECTED = 0,
            NOT_EXIST
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DB_INFO
        {
            public uint _id;
            public uint _type;
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public DB_INFO[] _info;
        public uint _info_size;
        public uint _current_id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_SCOPE_LIST
    {
        public G2SCOPE_LIST _list;
        public int _type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_EVENT_SEARCH_OPTIONS
    {
        public enum DIRECTION
        {
            BACKWARD = -1,
            FORWARD = 1
        }

        public int _unit_count;
        public int _direction;
        [MarshalAs(UnmanagedType.U1)]
        public bool _union_result;
        public G2SCOPE _scope;
        public G2CHANNEL_SET _motion;
        public G2CHANNEL_SET _object;
        public G2CHANNEL_SET _video_loss;
        public G2CHANNEL_SET _video_blind;
        public G2CHANNEL_SET _video_analytics;
        public G2CHANNEL_SET _trip_zone;
        public G2CHANNEL_SET _tamper;
        public G2CHANNEL_SET _face_detect;
        public G2CHANNEL_SET _instant_record;
        public G2CHANNEL_SET _camera_record_bad;
        public G2CHANNEL_SET _camera_fan_error;
        public G2CHANNEL_SET _record;   // associated record camera channels
        public G2CHANNEL_SET _alarm_in;
        public G2CHANNEL_SET _alarm_in_network;
        public G2CHANNEL_SET _audio_in;
        public G2CHANNEL_SET _text_in;
        public G2CHANNEL_SET _car_event;
        public G2CHANNEL_SET _system_events;
        public G2CHANNEL_SET _spc_motion_xor_trip_zone;

        public void clear_channels()
        {
            _motion._len = 0;
            _object._len = 0;
            _video_loss._len = 0;
            _video_blind._len = 0;
            _video_analytics._len = 0;
            _trip_zone._len = 0;
            _tamper._len = 0;
            _face_detect._len = 0;
            _instant_record._len = 0;
            _camera_record_bad._len = 0;
            _camera_fan_error._len = 0;
            _record._len = 0;
            _alarm_in._len = 0;
            _alarm_in_network._len = 0;
            _audio_in._len = 0;
            _text_in._len = 0;
            _car_event._len = 0;
            _system_events._len = 0;
            _spc_motion_xor_trip_zone._len = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct EXPRESSION
        {
            public enum COMPARE_TYPE
            {
                NONE = 0,
                LESS,
                LESS_EQUAL,
                EQUAL,
                EQUAL_MORE,
                MORE
            }

            public G2STRING_128 _name;
            public G2STRING_128 _value;
            public int _op;
            public int _column;
            public int _line;
        }

        [MarshalAs(UnmanagedType.U1)]
        public bool _combi_and;
        public EXPRESSION _exp;

        public bool valid { get { return g2search_g2.text_in_search_condition_is_valid(ref this); } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS
    {
        public int _unit_count;
        public int _system_id;
        public G2SCOPE _scope;
        public G2CHANNEL_SET _channels;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION[] _condition;
        public int _condition_count;
        [MarshalAs(UnmanagedType.U1)]
        public bool _transaction_wise;
        [MarshalAs(UnmanagedType.U1)]
        public bool _case_sensitive;
        [MarshalAs(UnmanagedType.U1)]
        public bool _match_whole_word;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_PARAM_CLIPCOPY_SIZE_INFO
    {
        public G2CLIPCOPY_SIZE_INFO _info;
        public int _status;
        public uint _progress;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_SOLE_CHANNEL_TAG
    {
        public int _channel;
        public int _tag;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_SOLE_CHANNEL_TAG_ID
    {
        public int _channel;
        public int _tag;
        public long _id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_SOLE_SNAPSHOP_LOADER
    {
        public enum FINISH_REASON
        {
            COMPLETED,
            CANCELED,
            ERROR_OCCURRED,
            NO_IMAGE,
        };
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION
    {
        public long _id;
        public G2CHANNEL_SET _channels;
        public G2SPOT _spot;
        public int _precision;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION_LIST
    {
        public IntPtr _options;
        public uint _len;
        public G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION[] to()
        {
            G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION[] list = new G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION[_len];
            for (int i = 0; i < list.Length; ++i)
            {
                IntPtr ptr = new IntPtr(_options.ToInt64() + i * Marshal.SizeOf(typeof(G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION)));
                list[i] = (G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION)Marshal.PtrToStructure(ptr, typeof(G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION));
            }
            return list;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_SOLE_RECORDED_SCOPE
    {
        public G2CHANNEL_SET _channels;
        public G2SCOPE _scope;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_SOLE_PROBE_FRAMELOAD
    {
        public int _ips;
        public int _bps;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_G2_SOLE_PLAYER_OPTIONS
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool _load_all_frames_for_next_step;
        [MarshalAs(UnmanagedType.U1)]
        public bool _load_all_frames_for_play;
        [MarshalAs(UnmanagedType.U1)]
        public bool _load_all_channels_for_move_to;
        [MarshalAs(UnmanagedType.U1)]
        public bool _load_all_channels_in_parallel_for_move_to;
        [MarshalAs(UnmanagedType.U1)]
        public bool _wait_move_before_play;
        [MarshalAs(UnmanagedType.U1)]
        public bool _reload_last;
        public uint _tick_precision_tolerance; // in milliseconds
        public uint _event_precision_interval; // in seconds
        public uint _fast_skip_interval;
        public uint _faster_skip_interval;
        public uint _fastest_skip_interval;
        public uint _text_in_load_mask;
        public uint _event_log_load_mask;
        public uint _inex_event_log_load_mask;
        public uint _gps_data_load_mask;
        public uint _anpr_load_mask;
        public uint _text_in_load_tolerance;    // in seconds
        public uint _event_log_load_tolerance;
        public uint _inex_event_log_load_tolerance;
        public uint _gps_data_load_tolerance;
        public uint _anpr_load_tolerance;
        [MarshalAs(UnmanagedType.U1)]
        public bool _segment_less_search;
        public uint _key_lookup_min_threshold_for_move_to;     // in seconds. for HOUR_PRECISION, MINUTE_PRECISION, SECOND_PRECISION
        public uint _key_lookup_max_threshold_for_move_to;     //                 EVENT_PRECISION, TEXT_IN_PRECISION, FRAME_PRECISION
        public uint _key_lookup_threshold_for_backward_play;   // in seconds.
        public uint _key_skip_interval;    // in miliseconds
        public sbyte _key_frame_only_type;
    }
}
