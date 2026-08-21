using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2MONITOR_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_require_reconnect = 2,
            on_receive_event = 3,
            on_receive_service_log_load = 4,
            on_receive_service_log_load_end = 5,
            on_receive_service_log_load_fail = 6,
            on_receive_service_log_load_stop = 7,
            on_status_connected = 8,
            on_status_disconnected = 9,
            on_status_receive_device_status = 10,
            on_status_receive_device_health = 11,
            on_status_receive_event = 12,
            on_receive_debug_log_load = 13,
            on_receive_debug_log_load_end = 14,
            on_receive_debug_log_load_fail = 15,
            on_receive_debug_log_load_stop = 16,

            CALLBACK_COUNT = 17
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2MONITORING_DEVICE_STATUS
    {
        public static readonly byte NOT_SUPPORT = 100;
        public static readonly byte NOT_VALUE = 101;

        public enum CONST
        {
            NUM_UNITS_MAX = 64,
        }
        public enum SESSION
        {
            STARTING = 0,
            DISCONNECTED = 1,
            CONNECTING,
            CONNECTED,
            DISCONNECTING
        }
        public enum ONOFF
        {
            OFF = 0,
            ON
        }
        public enum CHECK_SYSTEM
        {
            ABNORMAL = 0,
            NORMAL = 1,
            NOT_USE = 2,
            NOT_SUPPORT = 100
        }
        public enum CHECK_EVENT
        {
            NO_EVENT = 0,
            EVENT = 1,
            NOT_USE = 2,
            NOT_SUPPORT = 100
        }
        public enum RECORD_MODE : uint
        {
            ALL = 4294967295,
            NONE = 0,
            RECORDING = 1,
            PLAYBACK = 2,
            ARCHIVE = 4,
            CLIPCOPY = 8,
            PANIC_RECORDING = 16,
            PANIC_NOT_USE = 32
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECORD_TYPE
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct ON_TYPE
            {
                public byte _record;
                public byte _panic;
                public byte _archive;
                public byte _clip;
                public byte _play;
            }

            public uint _mode;
            public G2TIME _begin;
            public G2TIME _end;
            public ON_TYPE _on;

            public bool on_record { get { return _on._record == (byte)ONOFF.ON; } }
            public bool on_panic { get { return _on._panic == (byte)ONOFF.ON; } }
            public bool on_archive { get { return _on._archive == (byte)ONOFF.ON; } }
            public bool on_clip { get { return _on._clip == (byte)ONOFF.ON; } }
            public bool on_play { get { return _on._play == (byte)ONOFF.ON; } }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct HEALTH_TYPE
        {
            public long _record;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
            public byte[] _cameras;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
            public byte[] _cameras_record;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
            public byte[] _sensors;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
            public byte[] _textins;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
            public byte[] _fans;
        }

        public G2GUID _site;
        public G2GUID _admin;   // for federation
        public G2STRING_64 _version;
        public int _connection;
        public uint _reason_disconnect;
        public int _authority;
        [MarshalAs(UnmanagedType.U1)]
        public bool _support_remote_panic_recording;
        [MarshalAs(UnmanagedType.U1)]
        public bool _valid;
        public G2MAC_ADDRESS _mac;
        public RECORD_TYPE _record;
        public HEALTH_TYPE _health;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _motion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _alarm_in;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _alarm_out;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _alarm_in_network;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _text_in;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _object;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _video_blind;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _video_analysis;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _audio_in;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _trip_zone;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _tamper;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _face_detect;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public byte[] _instant_record;

        public bool is_authority(G2RAS_AUTHORITY.TYPE authority) { return (_authority & (int)authority) != 0; }
        public bool is_panic_record_not_use() { return (_record._mode & (uint)RECORD_MODE.PANIC_NOT_USE) != 0; }
        public bool is_panic_recording() { return (_record._mode & (uint)RECORD_MODE.PANIC_RECORDING) != 0; }
        public static bool is_empty_status(byte[] state) { return (state == null || state.Length == 0 || state[0] == NOT_VALUE); }
        public static int length_of_status(byte[] state)
        {
            int res = 0;
            if (state != null) foreach (byte val in state) if (val == NOT_VALUE) break; else res = res + 1;
            return res;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2MONITORING_DEVICE_HEALTH
    {
        public G2GUID _site;
        public G2GUID _admin;
        public G2STRING_64 _version;
        public int _connection;
        public uint _reason_disconnect;
        [MarshalAs(UnmanagedType.U1)]
        public bool _authority;
        [MarshalAs(UnmanagedType.U1)]
        public bool _valid;
        public G2MAC_ADDRESS _mac;
        public G2MONITORING_DEVICE_STATUS.RECORD_TYPE _record;
        public G2MONITORING_DEVICE_STATUS.HEALTH_TYPE _health;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2MONITORING_USER_ALARM_IN_REQUEST
    {
        public G2STRING_32 _sequence_id;
        public G2STRING_256 _data;
    }
}
