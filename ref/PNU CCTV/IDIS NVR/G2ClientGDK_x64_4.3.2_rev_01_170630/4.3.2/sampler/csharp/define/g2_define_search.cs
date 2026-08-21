using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2SEARCH_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_receive_recorded_date = 2,
            on_receive_recorded_time_hour = 3,
            on_receive_recorded_time_minute = 4,
            on_receive_recorded_rechour_minute = 5,
            on_receive_frame_data = 6,
            on_receive_no_frame = 7,
            on_receive_no_recorded_data = 8,
            on_receive_no_recorded_data_from_search_target = 9,
            on_receive_find_idr_event_time = 10,
            on_receive_notify_play_speed_changed = 11,
            on_receive_notify_play_stop_post = 12,
            on_receive_notify_end_of_play = 13,
            on_receive_notify_segment_changed = 14,
            on_receive_notify_search_mode_changed = 15,
            on_receive_notify_command_end = 16,
            on_receive_query_result_event = 17,
            on_receive_query_result_text_in = 18,
            on_receive_recorded_date_scope = 19,
            on_receive_segment_spot = 20,
            on_receive_error = 21,
            on_receive_text_in = 22,
            on_receive_external_tango_info = 23,
            on_receive_gps_data_start = 24,
            on_receive_gps_data = 25,
            on_receive_gps_data_list = 26,
            on_receive_gps_data_end = 27,
            on_receive_gps_data_end_count = 28,
            on_receive_gps_data_export_cancel_result = 29,
            on_receive_gps_data_measure_result = 30,
            on_require_prepare_playback = 31,
            on_require_prepare_load_event_image = 32,
            on_require_prepare_reload = 33,
            on_probe_session_profile = 34,

            on_saver_connected = 35,
            on_saver_disconnected = 36,
            on_saver_receive_recorded_date = 37,
            on_saver_receive_frame_data = 38,
            on_saver_receive_no_frame = 39,
            on_saver_receive_no_recorded_data = 40,
            on_saver_receive_notify_play_speed_changed = 41,
            on_saver_receive_notify_play_stop_spot = 42,
            on_saver_receive_notify_end_of_play = 43,
            on_saver_receive_notify_command_end = 44,
            on_saver_receive_clipcopy_scope = 45,
            on_saver_receive_clipcopy_measure_size = 46,
            on_saver_receive_clipcopy_size = 47,
            on_saver_receive_clipcopy_data = 48,
            on_saver_receive_clipcopy_set_password = 49,
            on_saver_receive_clipcopy_enable_channels = 50,
            on_saver_receive_clipcopy_canceled = 51,
            on_saver_receive_clipcopy_job_started = 52,
            on_saver_receive_clipcopy_job_finished = 53,
            on_saver_receive_clipcopy_section_begin = 54,
            on_saver_receive_clipcopy_section_end = 55,
            on_saver_receive_bank_space = 56,
            on_saver_receive_bank_image = 57,
            on_saver_receive_bank_audio = 58,
            on_saver_receive_bank_no_image = 59,
            on_saver_receive_bank_no_audio = 60,

            CALLBACK_COUNT = 61
        }
    }

    public struct G2SEARCH_PLAYBACK
    {
        public enum COMMAND
        {
            STOP = 0,
            PLAY,
            REW,
            FF,
            PREV,
            NEXT,
            FIRST,
            LAST,
            GOTO_HOUR,
            GOTO_SEC,
            GOTO_MSEC,
            GOTO_EVENT = 16,
            GOTO_SPOT = 110
        }
    }

    public struct G2SEARCH_DRIVE
    {
        public enum MODE
        {
            UNDEFINED = -1,
            G2 = 0,
            SAMBA,
            HOUR,
            IDR,
            LEGACY
        }
    }

    public struct G2SEARCH_QUERY
    {
        public enum MODE
        {
            UNDEFINED = -1,
            EVENT = 0,
            TEXT_IN
        }
    }

    public struct G2SEARCH_MODE
    {
        public enum TYPE
        {
            UNDEFINED = -1,
            TIMELAPSE = 0,
            EVENT_SEARCH
        }
    }

    public struct G2SEARCH_TIMELAPSE
    {
        public enum MODE
        {
            UNDEFINED = -1,
            MINUTE = 0,
            HOUR,
            MINUTE_IDR
        }
    }

    public struct G2SEARCH_TARGET
    {
        public enum TYPE
        {
            HDD = 0,
            ARCHIVE,
            EXTERNAL
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_EXTERNAL_DISK
    {
        public enum DISK
        {
            UNKNOWN = 0,
            IDE_HDD,
            SCSI_HDD,
            USB_HDD,
            IDE_CDRW,
            IDE_DVDRW,
            SW_RAID_DISK,
            FLASH_MEMORY,
            ESATA_HDD,
            ISCSI_HDD,
        }
        public enum STORAGE
        {
            NOT_USING = 0,
            RECORD,
            ARCHIVE,
            CLIP_COPY,
            EXTERNAL
        }

        public uint _disk_type;
        public uint _number;
        public ulong _capacity;
        public byte _storage_type;
    }

    public struct G2SEARCH_SUPPORT
    {
        public enum QUERY
        {
            SAMBA_SEARCH = 0,
            BUFFER_USE_TICK,
            BUFFER_USE_SYSTEMMSEC,
            BUFFER_USE_LEGACYMSEC,
            PLAY_AUDIO,
            CLIP_PLAYER,
            CLIP_SAVE_PASSWORD,
            CLIP_AVAIL_CHANNEL,
            CLIP_INCLUDE_TEXT_IN,
            CLIP_EXCLUDE_PLAYER,
            MINIBANK_PLAYER,
            MINIBANK_AUDIO,
            EXTERNAL_TANGO_SEARCH,
            ARCHIVE,
            ARCHIVE_ADT,
            VIRTUAL_CHANNEL = 18,
            CLIP_INCLUDE_GPS_DATA = 21,
            CLIP_SLICE = 22
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2EVENT_QUERY_CONDITION
    {
        public enum SYSTEM_EVENT_TYPE
        {
            SYSTEM_EVENT_TANGO_FULL = 0,
            SYSTEM_EVENT_RECORDER_BAD,
            SYSTEM_EVENT_ALARM_IN_BAD,
            SYSTEM_EVENT_DISK_BAD,
            SYSTEM_EVENT_DISK_TEMPERATURE,
            SYSTEM_EVENT_DISK_SMART,
            SYSTEM_EVENT_SYSTEM_ALIVE,
            SYSTEM_EVENT_PANIC,
            SYSTEM_EVENT_TANGO_ALMOST_FULL,
            SYSTEM_EVENT_DC_FAN_ERROR,
            SYSTEM_EVENT_DISK_CONFIG_CHANGE,
            SYSTEM_EVENT_SYSTEM_BOOT_UP,
            SYSTEM_EVENT_SYSTEM_RESTART,
            SYSTEM_EVENT_SYSTEM_SHUTDOWN,
            SYSTEM_EVENT_COVER_OPEN,
            SYSTEM_EVENT_DISK_OFF,
            SYSTEM_EVENT_NO_DISK,
            SYSTEM_EVENT_TEXT_IN_BAD,
            SYSTEM_EVENT_GPS_RECEIVE_ERROR,
            SYSTEM_EVENT_LOGIN_FAILED_SEVERAL_TIMES,
            SYSTEM_EVENT_USER_LOGIN,
            SYSTEM_EVENT_USER_LOGOUT,
            SYSTEM_EVENT_SETUP_CHANGED,
            SYSTEM_EVENT_DISK_ON = 23,

            MAX_SYSTEM_EVENT_COUNT
        }
        public enum CARD_EVENT
        {
            MAX_FS_CARD_NUMBER = 18
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DWELLTIME_TYPE
        {
            public int _motion;
            public int _object;
            public int _video_loss;
            public int _video_analytics;
            public int _alarm_in;
        }

        public int _seq_number;
        public G2TIME _begin;
        public G2TIME _end;
        public uint _motion;
        public uint _object;
        public uint _video_loss;
        public uint _video_blind;
        public uint _video_analytics;
        public uint _trip_zone;
        public uint _tamper;
        public uint _face_detect;
        public uint _instant_record;
        public uint _record;
        public uint _alarm_in;
        public uint _alarm_in_network;
        public uint _audio_in;
        public uint _text_in;
        public uint _system_events;
        [MarshalAs(UnmanagedType.U1)]
        public bool _reload;
        public DWELLTIME_TYPE _dwell;
        public uint _fs_loop_id;
        public sbyte _fs_panic;
        public sbyte _fs_card;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CARD_EVENT.MAX_FS_CARD_NUMBER + 1)]
        public sbyte[] _fs_card_number;

        public void clear_channels()
        {
            _motion = 0;
            _object = 0;
            _video_loss = 0;
            _video_blind = 0;
            _video_analytics = 0;
            _trip_zone = 0;
            _tamper = 0;
            _face_detect = 0;
            _instant_record = 0;
            _record = 0;
            _alarm_in = 0;
            _alarm_in_network = 0;
            _audio_in = 0;
            _text_in = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2TEXT_IN_QUERY_CONDITION
    {
        public enum TEXTIN_EVENT
        {
            MAX_ITEM_COUNT = 5,
            MAX_NAME_VALUE_LENGTH = 79
        }
        public enum CONDITION
        {
            COND_NONE = 0,
            COND_AND,
            COND_OR
        }
        public enum COMPARATOR
        {
            COMP_NONE = 0,
            COMP_LESS,
            COMP_LESS_SAME,
            COMP_SAME,
            COMP_MORE_SAME,
            COMP_MORE
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ITEM_TYPE
        {
            public int _condition;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] _name;
            public int _comparator;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] _value;
            public int _column;
            public int _line;

            public string name
            {
                get { return (_name != null) ? Encoding.Default.GetString(_name) : ""; }
                set
                {
                    if (_name == null) _name = new byte[80];

                    Array.Clear(_name, 0, _name.Length);
                    byte[] s = Encoding.Default.GetBytes(value);
                    Array.Copy(s, _name, Math.Min(80, s.Length));
                }
            }
            public string value
            {
                get { return (_value != null) ? Encoding.Default.GetString(_value) : ""; }
                set
                {
                    if (_value == null) _value = new byte[80];

                    Array.Clear(_value, 0, _value.Length);
                    byte[] s = Encoding.Default.GetBytes(value);
                    Array.Copy(s, _value, Math.Min(80, s.Length));
                }
            }
        }

        [MarshalAs(UnmanagedType.U1)]
        public bool _reload;
        public G2TIME _begin;
        public G2TIME _end;
        public uint _channels;
        public int _data_type;
        public int _item_count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public ITEM_TYPE[] _item;
        [MarshalAs(UnmanagedType.U1)]
        public bool _case_sensitive;
        [MarshalAs(UnmanagedType.U1)]
        public bool _match_whole_word;
        [MarshalAs(UnmanagedType.U1)]
        public bool _transaction_wise;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2QUERY_TIME_RANGE
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool _day_first;
        public G2TIME _begin;
        public G2TIME _end;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_LOG_INFO
    {
        public enum LOG_INFO
        {
            MAX_TRANS_NUMBER = 16,
            MAX_CAMERA = 16
        }

        public int _seq_number;
        public int _rdn;            // relative duplicated(time) number
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public sbyte[] _version;
        public int _event_level1;
        public int _event_level2;
        public sbyte _event_id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)LOG_INFO.MAX_TRANS_NUMBER + 1)]
        public sbyte[] _trans_number;
        public sbyte _level;
        public G2TIME _time;
        public int _msec;
        public uint _cameras;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)LOG_INFO.MAX_CAMERA)]
        public ushort[] _pre_dwell;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)LOG_INFO.MAX_CAMERA)]
        public ushort[] _pst_dwell;
        public G2STRING_64 _label;

        public G2EVENT_INFO.TYPE_LEVEL1 event_level1 { get { return (G2EVENT_INFO.TYPE_LEVEL1)_event_level1; } }
        public G2EVENT_INFO.TYPE_LEVEL2 event_level2 { get { return (G2EVENT_INFO.TYPE_LEVEL2)_event_level2; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2HARD_DISK_INFO
    {
        public enum FILE_SYSTEM
        {
            NATIVE = 0,
            GENERAL,
            RAW
        }
        public enum DISK_TYPE
        {
            UNKNOWN = 0x00,
            SCSI,
            ATAPI,
            ATA,
            TYPE_1394,
            SSA,
            FIBER,
            USB,
            RAID,
            ISCSI,
            SAS,
            SATA,
            SD,
            MMC,
            VIRTUAL,
            FILEBACKEDVIRTUAL,
            MAX,
            NETWORK = 0x7D,
            ESATA = 0x7E,
            MAX_RESERVED = 0x7F
        }

        public int _index;
        public int _disk_type;
        public int _file_system;
        [MarshalAs(UnmanagedType.U1)]
        public bool _enabled;
        [MarshalAs(UnmanagedType.U1)]
        public bool _removable;
        public ulong _capacity;
        public ulong _free_space;
        public G2SCOPE _scope;
        public G2STRING_256 _path;
        public G2STRING_256 _model;
        public G2STRING_256 _serial;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_PARAM_GPS_DATA_MEASURE_RESULT
    {
        public int _count;
        public sbyte _done;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_PARAM_PREPARE_PLAYBACK
    {
        public int _command;
        public G2SPOT _spot;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_PARAM_PREPARE_LOAD_EVENT_IMAGE
    {
        public int _selected;
        [MarshalAs(UnmanagedType.U1)]
        public bool _last;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_PARAM_CLIPCOPY_SIZE
    {
        public int _status;
        public ulong _size;
        public G2TIME _begin;
        public G2TIME _end;
        public G2CLIPCOPY_SIZE_INFO _info;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_PARAM_CLIPCOPY_DATA
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool _completed;
        public int _progress;
        public int _offset;
        public int _size;
        public IntPtr _data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEARCH_PARAM_BANK_SPACE
    {
        public int _image_index_number;
        public int _audio_index_number;
        public G2TIME _start_time;
        public int _start_msec;
        public ulong _image_size;
        public ulong _audio_size;
    }
}
