using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2PLAY_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_query_options_search_base = 2,
            on_receive_hard_disk_info = 3,
            on_receive_record_channels = 4,
            on_receive_record_time_info_load = 5,
            on_receive_record_time_info_load_end = 6,
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
            on_receive_device_log_load = 26,
            on_receive_device_log_load_end = 27,
            on_receive_device_log_load_fail = 28,
            on_receive_device_log_load_stop = 29,
            on_receive_service_log_load = 30,
            on_receive_service_log_load_end = 31,
            on_receive_service_log_load_fail = 32,
            on_receive_service_log_load_stop = 33,
            on_require_prepare_rollback = 34,
            on_probe_session_profile = 35,

            on_receive_debug_log_load = 36,
            on_receive_debug_log_load_end = 37,
            on_receive_debug_log_load_fail = 38,
            on_receive_debug_log_load_stop = 39,

            on_receive_record_failover_scope = 40,
            on_get_frame_buf_status = 41,

            CALLBACK_COUNT = 42
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PLAY_OPTIONS_SEARCH_BASE
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool segment_less;
        public int sole_player_count;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PLAY_CHANNEL_INFO
    {
        public G2GUID _guid;
        public int _channelext;
    }

    public struct G2PLAY_SCOPE_TYPE
    {
        public enum TYPE
        {
            UNDEFINED = 0,
            CLIP_COPY = 1,
            GOTO
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PLAY_SCOPE_LIST
    {
        public G2SCOPE_LIST _list;
        public int _type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2FAILOVER_SCOPE_INFO_LIST
    {
        public IntPtr _infos;
        public uint _len;
        public G2FAILOVER_SCOPE_INFO[] to()
        {
            G2FAILOVER_SCOPE_INFO[] list = new G2FAILOVER_SCOPE_INFO[_len];
            for (int i = 0; i < list.Length; ++i)
            {
                IntPtr ptr = new IntPtr(_infos.ToInt64() + i * Marshal.SizeOf(typeof(G2FAILOVER_SCOPE_INFO)));
                list[i] = (G2FAILOVER_SCOPE_INFO)Marshal.PtrToStructure(ptr, typeof(G2FAILOVER_SCOPE_INFO));
            }
            return list;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PLAY_CLIPCOPY_STATUS
    {
        public enum STATUS
        {
            READY = 0,
            MEASURE_SIZE = 1,
            FORMAT_STORAGE = 3,
            COPY_DATA = 4,
            COPY_PLAYER = 5,
            FINALIZE = 6
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PLAY_HARD_DISK_INFO
    {
        public enum FILE_SYSTEM_TYPE
        {
            NATIVE = 0,
            GENERAL,
            RAW
        }
        public enum DISK_TYPE
        {
            INVALID = 0x00,
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

        public G2STRING_256 _path;
        public G2STRING_256 _path_storage_sub;
        public G2STRING_64 _volume_label;
        public G2STRING_64 _serial;
        public DISK_TYPE _disk_type;
        public FILE_SYSTEM_TYPE _fs;
        public G2SCOPE _frame_scope;
        [MarshalAs(UnmanagedType.U1)]
        public bool _enabled;
        [MarshalAs(UnmanagedType.U1)]
        public bool _system_volume;
        [MarshalAs(UnmanagedType.U1)]
        public bool _removable;
        public ulong _capacity;
        public ulong _free_space;
        public ulong _storage_capacity;
        public ulong _storage_free_space;
        public uint _bank_count;
        public uint _bank_count_max;
        public uint _bank_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PLAY_CLIPCOPY_SIZE
    {
        public int _status;
        public uint _size;
        public G2SCOPE _scope;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PLAY_USAGE
    {
        public enum TYPE
        {
            PLAYBACK = 0,
            LOG
        }
    }
}
