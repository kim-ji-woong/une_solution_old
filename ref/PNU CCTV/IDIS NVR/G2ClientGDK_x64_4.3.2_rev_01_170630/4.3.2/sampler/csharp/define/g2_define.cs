using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
    using G2HANDLE_PTR = System.IntPtr;
    using G2HWATCH = System.Int32;
    using G2HWND = System.IntPtr;
#if _WIN64
    using G2WPARAM = System.UInt64;
    using G2LPARAM = System.Int64;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int64;
#else
    using G2WPARAM = System.UInt32;
    using G2LPARAM = System.Int32;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int32;
#endif

    public class G2PLATFORM
    {
#if _WIN64
        public const string DLL_name = "G2ClientGDKx64.dll";
#else
        public const string DLL_name = "G2ClientGDK.dll";
#endif
    }

    public delegate G2RESULT G2FUN_LISTENER(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    public delegate G2RESULT G2FUN_LISTENER_PTR(G2HANDLE_PTR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    public delegate bool G2FUN_GET_ADAPTOR(G2HANDLE handle, IntPtr ptr);

    public class G2BOOLEAN
    {
        public const int G2FALSE = 0;
        public const int G2TRUE = 1;
    }

    public class G2CONST
    {
        public const int G2HNULL = 0;
        public const int GINFINITE = int.MaxValue;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PARAM_BUNCH
    {
        public IntPtr _params;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2WPARAM_LIST
    {
        public IntPtr _params;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LPARAM_LIST
    {
        public IntPtr _params;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2PARAM_LIST_int32
    {
        public IntPtr _val;
        public uint _len;
    }

    public struct G2DISCONNECT_REASON
    {
        public enum TYPE
        {
            UNKNOWN = 0,                            // unknown case
            LOGOUT = 1,                             // normally logout (base->post)
            FULL_CHANNEL = 2,                       // deny connection because all of server channels are used (base<-post)
            INVALID_VERSION = 3,                    // invalid product version (base->post)
            LOGIN_FAIL = 4,                         // invalid user or passwd (base<-post)
            ADMIN_CLOSE = 5,                        // admin close the current connection forcibly (base<-post)
            ADMIN_TIMEOUT = 6,                      // timeout (base<-post)
            SYS_SHUTDOWN = 7,                       // post system shutdown (base<-post)
            NO_CHANNEL = 8,                         // can't connect - all of my network channels are used
            NO_SERVER = 10,                         // can't connect - no server module (sock. err=10061)
            NET_DOWN = 11,                          // network is down (sock. err=10050)
            NET_UNREACHABLE = 12,                   // network is unreachable (sock. err=10051)
            CONN_TIMEOUT = 13,                      // connection time out (sock. err=10060)
            CONN_RESET = 14,                        // connection reset by peer (sock. err=10054)
            HOST_DOWN = 15,                         // host is down (sock. err=10064)
            HOST_UNREACHABLE = 16,                  // no route th host (sock. err=10065)
            CONN_ABORTED = 17,                      // connection aborted (sock. err=10053)
            CONN_CANCEL = 20,                       // connection has been canceled by user.
            NET_NORESPONSE = 21,                    // the peer host does not respond.
            NET_NOISY = 22,                         // network is too noisy.
            SEND_OVERFLOW = 23,                     // sending queue overflow.
            NO_AUTHORITY = 25,                      // You have no authority for search.
            PORT_USED = 26,                         // the port is already in use.
            SSL_CONNECTION_FAILED = 27,             // SSL connection failed.
            NET_TIMEOUT = 28,                       // network is timed out
            HOST_TIMEOUT = 29,                      // host is timed out
            NOT_SUPPORT_RTP_TCP = 30,               // host cannot support RTP over TCP
            SOCKET_ERROR_OCCURRED = 31,             // socket error occurred
            FEN_RENDEZ_CONN_FAILED = 32,            // rendezvous service is not available
            FEN_RENDEZ_NO_ELEMENT = 33,             // rendezvous Element Not Found
            FEN_RELAY_CONN_FAILED = 34,             // relay Connection Failed
            FEN_RELAY_NOT_AVAILABLE = 35,           // relay Service is not available
            FEN_DIRECT_CONN_DOWN = 36,              // Fen Direct connection is closed
            FEN_UDT_CONN_DOWN = 37,                 // Fen UDT connection is closed
            FEN_RELAY_CONN_DOWN = 38,               // Fen Relay Connection is closed
            INVALID_RECEIVE_PACKET_BUFFER = 1001,   // invalid receive packet buffer
            INVALID_SEND_PACKET_BUFFER = 1002,      // invalid send packet buffer
            ALIVE_CHECK_TICKOUT = 1003,             // alive check tick out
            RTSP_START_FAILED = 2001,               // RTSP session start failed
            RTSP_STOP_FAILED = 2002,                // RTSP session stop failed
            RTSP_IMAGE_NOT_RECEIVED = 2003,         // image is not received by rtsp
            RTSP_TEARDOWN_DISCONNECT = 2004,        // disconnected when you requests teardown
            RTSP_TUNNELING_DISCONNECT = 2005,       // request disconnect of http tunneling channel
            RTSP_SESSION_ALREADY_FINISHED = 2006,   // RTSP session is already finished
            RTSP_ALIVE_CHECK_ERROR = 2007,          // RTSP alive check error occurred
            RTSP_OVER_ALIVE_CHECK_INTERVAL = 2008,  // RTSP over alive check interval
            MISMATCH_ADAPTOR = 10000,
            MISMATCH_PORT_UNITY = 10001,
            NOT_SUPPORT_PRODUCT = 10002
        }
    }
    public struct G2SERVICE_LOGIN_FAIL_REASON
    {
        public enum TYPE
        {
            NO_ERROR = 0,
            INVALID_VERSION = 1,
            FULL_CHANNEL = 2,
            FAILOVER_INACTIVE = 3,                  // failover inactive
            INVALID_LICENSE = 10,                   // Invalid License(License Expired)
            INVALID_ID = 11,                        // invalid id
            INVALID_PASSWORD = 12,                  // invalid password
            BLOCK_USER = 13,                        // temporary disable
            CONNECTION_BY_SAME_ID = 14,             // it is a user connecting by same ID
            DISABLE = 15,                           // ID is disable.
            UNACCESSIBLE_IP = 16,                   // not allowed client ip.
            INVALID_TIME = 17,                      // because of disallowed time.
            DIRECTORY_UNABLE_CLIENT = 18,           // client doesn't join in ActiveDirectory.
            DIRECTORY_UNABLE_SERVER = 19,           // server doesn't join in ActiveDirectory.
            DIRECTORY_UNABLE_USER = 20,             // user doesn't allow to ActiveDirectory Connection.
            DIRECTORY_DIFFERENT_DC = 21,            // different DomainController.
            APP_FAIL_SEND_RETRY_OUT = 100           // send retry too many for "guaranteed send".
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2GUID
    {
        [StructLayout(LayoutKind.Sequential)]
        public partial struct GUID_TYPE
        {
            public uint _data1;
            public ushort _data2;
            public ushort _data3;
            public G2FIXED_BUF_8 _data4;
        }
        public GUID_TYPE _GUID;
        public uint _type;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2PARAM_GUID_LIST
    {
        public IntPtr _guid;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2GUID_8
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public G2GUID[] _guid;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct G2GUID_16
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public G2GUID[] _guid;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct G2GUID_32
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public G2GUID[] _guid;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct G2GUID_64
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public G2GUID[] _guid;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct G2GUID_128
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public G2GUID[] _guid;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct G2GUID_256
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public G2GUID[] _guid;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct G2GUID_512
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public G2GUID[] _guid;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct G2STRING_16
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string _string;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct G2STRING_32
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string _string;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct G2STRING_46
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 46)]
        public string _string;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct G2STRING_64
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string _string;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct G2STRING_128
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string _string;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct G2STRING_256
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string _string;
        public uint _len;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct G2STRING_512
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string _string;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2TIME
    {
        public uint _time;  // time_t is not

        public G2TIME date { get { return new G2TIME(year, month, day); } }
        public int year { get { return g2foundation.g2_time_get_year(ref this); } }
        public int month { get { return g2foundation.g2_time_get_month(ref this); } }
        public int day { get { return g2foundation.g2_time_get_day(ref this); } }
        public int hour { get { return g2foundation.g2_time_get_hour(ref this); } }
        public int minute { get { return g2foundation.g2_time_get_minute(ref this); } }
        public int second { get { return g2foundation.g2_time_get_second(ref this); } }
        public bool valid { get { return g2foundation.g2_time_is_valid(ref this); } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2TIME_SPAN
    {
        public int _days;
        public int _hours;
        public int _minutes;
        public int _seconds;

        public int total_hours { get { return _days * 24 + _hours; } }
        public int total_minutes { get { return total_hours * 60 + _minutes; } }
        public int total_seconds { get { return total_minutes * 60 + _seconds; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2SPOT
    {
        public uint _segment;
        public uint _tick;
        public G2TIME _time;

        public bool valid { get { return (g2foundation.g2_spot_is_valid(ref this)); } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SPOT_PRECISION
    {
        public G2SPOT _spot;
        public int _precision;

        public G2PLAYER.PRECISION precision
        {
            get { return (G2PLAYER.PRECISION)_precision; }
            set { _precision = (int)value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2SCOPE : ICloneable, IEquatable<G2SCOPE>
    {
        public G2SPOT _begin;
        public G2SPOT _end;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2AUDIO_CODEC_INFO
    {
        public ushort codec;
        public ushort sampling;
        public ushort channel;
        public ushort segment;
        public uint bitrate;
        public ushort bits_per_sample;
        public G2FIXED_RESERVED_10_SBYTE reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2AUDIO_DEVICE_INFO
    {
        public G2AUDIO_CODEC_INFO _internal;
        public int _len_output;
        public int _len_sample;
        public int _count_per_sec;
    }

    public partial struct G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PARAM_ARCHIVED
        {
            public IntPtr _ptr;
            public int _len;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORDINATE_TYPE
        {
            public ushort _left;
            public ushort _top;
            public ushort _right;
            public ushort _bottom;

            public Rectangle to() { return new Rectangle(_left, _top, _right - _left, _bottom - _top); }
        }

        public bool _has;
        public Size _resolution;
        public RectangleF[] _rects;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2CODEC_INFO_SI_ELEVATOR_STATUS
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PARAM_ARCHIVED
        {
            public IntPtr _ptr;
            public int _len;
        }

        public enum DOOR_STATUS
        {
            UNKNOWN = 0,
            CLOSING,
            CLOSE,
            OPENING,
            OPEN
        }
        public enum DIRECTION
        {
            UNKNOWN = 0,
            DOWN,
            STOP,
            UP
        }
        public enum MODE
        {
            UNKNOWN = 0,
            MANUAL,
            AUTO
        }

        [MarshalAs(UnmanagedType.U1)]
        public bool _has;
        public ushort _id;
        public float _floor;
        public sbyte _door_status;
        public sbyte _direction;
        public sbyte _mode;
        public G2STRING_128 _additional;

        public DOOR_STATUS door_status
        {
            get { return (DOOR_STATUS)_door_status; }
            set { _door_status = (sbyte)value; }
        }
        public DIRECTION direction
        {
            get { return (DIRECTION)_direction; }
            set { _direction = (sbyte)value; }
        }
        public MODE mode
        {
            get { return (MODE)_mode; }
            set { _mode = (sbyte)value; }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public partial struct G2FRAME
    {
        public enum CONST
        {
            FINGERPRINT_SIZE = 32,
            MAX_TITLE_LENGTH = 63
        }
        public enum FROM
        {
            UNDEFINED = 0,
            WATCH = 1,      // RAS watch
            SEARCH = 2,     // RAS search
            SEARCH_G2 = 4,  // local search based G2Search
            RTP = 8,        // RTP, ONVIF(or 3rd party) protocol connect directly
            LIVE = 16,      // service streaming
            PLAY = 32,      // service recording
            BACKUP = 64,    // service backup
            LIVE_LINE = WATCH | RTP | LIVE,
            PLAY_LINE = SEARCH | SEARCH_G2 | PLAY | BACKUP
        }
        public enum TYPE
        {
            I_FRAME = 0,
            P_FRAME,
            AUDIO,
            X_FRAME,
            B_FRAME,
            BUILT_IN_TYPE_COUNT,
            DUMMY_FRAME = 0x10,
            INVALID_TYPE = 0xFF
        }
        public enum FLAG
        {   // bit-mask
            UNDEFINED = 0,
            TIME_LAPSE = 1,
            EVENT = 2,
            PRE_EVENT = 4,
            PANIC = 8,
            FINGERPRINT = 16,
            IRREGULAR = 32,
            BROKEN_DATA_HEADER = 64,
            BROKEN_DATA_BODY = 128,
        }

        public const ushort INVALID_CHANNEL = 0xFFFF;
        public const ushort COMMON_CHANNEL = 0xFFFF;

        [StructLayout(LayoutKind.Sequential)]
        public struct INDEX_TYPE
        {
            public G2SPOT _spot;
            public G2TIME _local_time;
            public uint _data_size;
            public uint _plain_size;
            public uint _key_id;
            public ushort _channel;
            public ushort _stream_id;
            public byte _from;
            public byte _type;
            public byte _flag;
            public uint _data_file_offset;
            [MarshalAs(UnmanagedType.U1)]
            public bool _valid;
            [MarshalAs(UnmanagedType.U1)]
            public bool _bad;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VIDEO_INFO
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool _byfield;
            [MarshalAs(UnmanagedType.U1)]
            public bool _vol_header;
            [MarshalAs(UnmanagedType.U1)]
            public bool _progressive;
            [MarshalAs(UnmanagedType.U1)]
            public bool _no_half;
            [MarshalAs(UnmanagedType.U1)]
            public bool _roi;
            [MarshalAs(UnmanagedType.U1)]
            public bool _fish_eye;
            public ushort _ing_AF;
            public ushort _zoom;
            public byte _ips;
            public byte _ips_var;
            public G2SIZE _res_override;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct AUDIO_INFO
        {
            public G2AUDIO_DEVICE_INFO _device;
        }
        [StructLayout(LayoutKind.Explicit)]
        public struct FRAME_INFO_UNION
        {
            [FieldOffset(0)]
            public VIDEO_INFO _video;
            [FieldOffset(0)]
            public AUDIO_INFO _audio;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct EXTRA_TYPE
        {
            public int _decoder;
            [MarshalAs(UnmanagedType.U1)]
            public bool _display;
            public long _pts;
            public FRAME_INFO_UNION _info;
            public IntPtr _param_stream;
            public int _param_stream_len;
            public int _required_key_frame;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct ARCHIVED_DATA
        {
            public G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION.PARAM_ARCHIVED _content_analytics_face_detection;
            public G2CODEC_INFO_SI_ELEVATOR_STATUS.PARAM_ARCHIVED _si_elevator_status;
        }

        public IntPtr _data;
        public IntPtr _plain_ptr;   // raw stream data begin pointer
        public ushort _width;
        public ushort _height;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)CONST.MAX_TITLE_LENGTH + 1)]
        public string _title;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.FINGERPRINT_SIZE)]
        public byte[] _fingerprint;
        [MarshalAs(UnmanagedType.U1)]
        public bool _bad;
        public INDEX_TYPE _index;
        public EXTRA_TYPE _extra;
        public ARCHIVED_DATA _archived;

        public G2SPOT spot { get { return _index._spot; } }
        public G2TIME local_time { get { return _index._local_time; } }
        public uint key_frame_id { get { return _index._key_id; } }
        public uint data_size { get { return _index._data_size; } }
        public FROM from { get { return (FROM)_index._from; } }
        public TYPE type { get { return (TYPE)_index._type; } }
        public int channel { get { return (int)_index._channel; } }
        public int stream_id { get { return (int)_index._stream_id; } }
        public G2DECODER_CODEC.TYPE decoder { get { return (G2DECODER_CODEC.TYPE)_extra._decoder; } }
        public FLAG flags { get { return (FLAG)_index._flag; } }
        public FLAG record_type
        {
            get
            {
                return ((_index._flag & (byte)FLAG.PANIC) != 0 ? FLAG.PANIC :
                        (_index._flag & (byte)FLAG.IRREGULAR) != 0 ? FLAG.IRREGULAR :
                        (_index._flag & (byte)FLAG.TIME_LAPSE) != 0 ? FLAG.TIME_LAPSE :
                        (_index._flag & (byte)FLAG.EVENT) != 0 ? FLAG.EVENT : FLAG.PRE_EVENT);
            }
        }
        public string title { get { return _title; } }
        public float local_zoom { get { return _extra._info._video._zoom / 10.0F; } }

        public bool valid { get { return _index._valid; } }
        public bool is_bad { get { return _index._bad || _bad; } }
        public bool is_bad_index { get { return _index._bad; } }
        public bool is_bad_data { get { return _bad; } }
        public bool is_broken
        {
            get
            {
                return (_index._flag & (byte)FLAG.BROKEN_DATA_HEADER |
                        _index._flag & (byte)FLAG.BROKEN_DATA_BODY) != 0;
            }
        }
        public bool is_broken_data_header { get { return (_index._flag & (byte)FLAG.BROKEN_DATA_HEADER) != 0; } }
        public bool is_broken_data_body { get { return (_index._flag & (byte)FLAG.BROKEN_DATA_BODY) != 0; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2CHANNEL
    {
        public const int MIN = 0;
        public const int MAX = 0xFFFF - 3;
        public const int ANY = 0xFFFF - 2;
        public const int ALL = 0xFFFF - 1;
        public const int NONE = 0xFFFF;

        public static bool is_unique_channel(int channel)
        {
            return (channel >= MIN && channel <= MAX);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2CHANNEL_SET
    {
        public static readonly int G2_CHANNEL_MIN = 0;
        public static readonly int G2_CHANNEL_MAX = 0xFFFF - 3;
        public static readonly int G2_CHANNEL_ANY = 0xFFFF - 2;
        public static readonly int G2_CHANNEL_ALL = 0xFFFF - 1;
        public static readonly int G2_CHANNEL_NONE = 0xFFFF;

        public enum CONST
        {
            MAX_CHANNEL_COUNT = 256
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.MAX_CHANNEL_COUNT)]
        public int[] _channels;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2CHANNEL_STREAM
    {
        public enum CONST
        {
            STREAM_LIMIT = 8
        }
        public int _channel;
        public int _stream;

        public bool valid { get { return _channel >= 0 && _stream >= 0; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2CHANNEL_STREAM_SET
    {
        public enum CONST
        {
            MAX_STREAM_COUNT = 256
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.MAX_STREAM_COUNT)]
        public G2CHANNEL_STREAM[] _streams;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2TEXT_IN
    {
        public enum TRANSACTION
        {
            TRANSACTION_BEGIN,
            TRANSACTION_CONTINUE,
            TRANSACTION_END,
            TRANSACTION_COMPLETE,
        }
        public enum CONST
        {
            RESERVED_SIZE = 4,
            MAX_DATA_SIZE = 50 * 1024,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INDEX_TYPE
        {
            public ushort _channel;
            public byte _transaction;
            public byte _system_id;
            public G2SPOT _spot;
            public uint _data_file_offset;
            public uint _data_size;
            public G2CHANNEL_SET _record_channels;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.RESERVED_SIZE)]
            public byte[] _reserved;
            [MarshalAs(UnmanagedType.U1)]
            public bool _bad;
        }

        public INDEX_TYPE _index;
        [MarshalAs(UnmanagedType.U1)]
        public bool _bad;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.MAX_DATA_SIZE)]
        public byte[] _data;

        public G2SPOT spot { get { return _index._spot; } }
        public G2TIME time { get { return _index._spot._time; } }
        public int channel { get { return (int)_index._channel; } }
        public int system_id { get { return (int)_index._system_id; } }
        public TRANSACTION transaction { get { return (TRANSACTION)_index._transaction; } }
        public g2channel_set record_channels { get { return _index._record_channels; } }
        public bool is_bad { get { return _index._bad || _bad; } }
        public bool is_bad_index { get { return _index._bad; } }
        public bool is_bad_data { get { return _bad; } }
        public string data { get { return (_data != null) ? Encoding.Default.GetString(_data) : ""; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2TEXT_IN_ELEMENT
    {
        public enum TYPE
        {
            TYPE_START = 0,
            TYPE_CONTENT,
            TYPE_END
        }
        public enum FROM
        {
            FROM_PLAY = 0,
            FROM_LIVE
        }

        public int _camera;
        public int _type;
        public int _from;
        public G2SPOT _spot;
        public G2STRING_256 _data;

        public TYPE type { get { return (TYPE)_type; } }
        public FROM from { get { return (FROM)_from; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SCOPE_LIST
    {
        public IntPtr _scopes;
        public uint _len;
        public G2SCOPE[] to()
        {
            G2SCOPE[] list = new G2SCOPE[_len];
            for (int i = 0; i < list.Length; ++i)
            {
                IntPtr ptr = new IntPtr(_scopes.ToInt64() + i * Marshal.SizeOf(typeof(G2SCOPE)));
                list[i] = (G2SCOPE)Marshal.PtrToStructure(ptr, typeof(G2SCOPE));
            }
            return list;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SPOT_LIST
    {
        public IntPtr _spots;
        public uint _len;
        public G2SPOT[] to()
        {
            G2SPOT[] list = new G2SPOT[_len];
            for (int i = 0; i < list.Length; ++i)
            {
                IntPtr ptr = new IntPtr(_spots.ToInt64() + i * Marshal.SizeOf(typeof(G2SPOT)));
                list[i] = (G2SPOT)Marshal.PtrToStructure(ptr, typeof(G2SPOT));
            }
            return list;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2FAILOVER_SCOPE_INFO
    {
        public uint _scope_id;
        public G2GUID _service_key;
        public G2GUID _failover_service_key;
        public G2SCOPE _scope;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2VERSION
    {
        public enum TYPE
        {
            DEVICE_1_0 = 0x00000100,
            USER_1_0 = 0x00000100,
            DEVICE = DEVICE_1_0,
            USER = USER_1_0
        }
        public int _version;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2VERSION_NETWORK
    {
        public enum TYPE
        {
            NETWORKINFO_1_0 = 0x00000100,
            NETWORKINFO_2_0 = 0x00000200,
            NETWORKINFO_2_1 = 0x00000201,
            NETWORKINFO_3_0 = 0x00000300,
            NETWORKINFO = NETWORKINFO_3_0,
        }
        public int _version;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SOCKET_ADDRESS
    {
        public enum LEN
        {
            MAX_HOST_NAME = 50
        }
        G2STRING_128 _host_name;
        G2STRING_128 _host_address;
        G2STRING_128 _interface;
        [MarshalAs(UnmanagedType.U1)]
        bool _unresolved;
        ushort _port;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2MAC_ADDRESS
    {
        public enum LEN
        {
            MAX_LEN = 6
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)LEN.MAX_LEN)]
        public byte[] _mac;
    }

    public struct G2SSL_STATE
    {
        public enum TYPE
        {
            UNKNOWN = -1,
            NOT_USE = 0,
            PACKET_HEADER,
            PACKET_MULTIMEDIA_EXCLUDE,
            PACKET_FULL,
            PACKET_MULTIMEDIA_PARTIALLY
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct G2NETWORK_INFO
    {
        public enum ADDRESS_TYPE : int
        {
            UNKNOWN = -1,
            IPV4 = 0,
            DVRNS,
            DNS,
            MDNS,
            DDNS,
            IPV6,
            WSDISCOVERY
        }
        public enum COMMAND_PROTOCOL_TYPE : int
        {
            UNKNOWN = -1,
            IDIS = 0,
            ONVIF,
            HTTP_TCP,
            COMBINE_ONVIF_HTTP
        }
        public enum MEDIA_STREAM_PROTOCOL_TYPE : int
        {
            UNKNOWN = -1,
            IDIS = 0,
            RTSP_RTP_UDP_UNICAST = 10,
            RTSP_RTP_UDP_MULTICAST,
            RTSP_RTP_TCP,
            HTTP_RTP_UDP_UNICAST = 20,
            HTTP_TCP,
            RTP_RTSP_HTTP_TCP = 30,
        }
        public enum NETWORK_TYPE
        {
            IP = 0,
            DVRNS,
            DNS,
            RTP,
            RTSP,
            RTSP_RTP,
            HTTP_RTP,
            MDNS,
        }
        public enum PORT_TYPE
        {
            MIN_PORT_INDEX = 0,
            SERVICE_PORT = 0,
            ADMIN_PORT,
            WATCH_PORT,
            SEARCH_PORT,
            RECORD_PORT,
            AUDIO_PORT,
            WEB_PORT,
            RTP_PORT,
            RTSP_PORT,
            EXTRA_SERVER_PORT,
            HTTPS_PORT,
            MAX_PORT_INDEX,
        }
        public enum ONVIF_SERVICE_PATH_TYPE
        {
            DEVICE_SERVICE_PATH = 0,
            MEDIA_SERVICE_PATH,
            PTZ_SERVICE_PATH,
            IMAGING_SERVICE_PATH,
            MAX_SERVICE_PATH_INDEX,
        }
        public enum ONVIF_SERVICE_PATH_EXTRA_TYPE
        {
            EVENT_SERVICE_PATH = 0,
            ANALYTICS_SERVICE_PATH,
            MAX_SERVICE_PATH_EXTRA_INDEX,
        }
        public enum MEDIA_CONNECT_TYPE
        {
            MEDIA_CONNECT_TYPE_STREAM = 0,
            MEDIA_CONNECT_TYPE_RECORD,
            MEDIA_CONNECT_TYPE_SEARCH,
            MEDIA_CONNECT_TYPE_MONITORING,
            MAX_MEDIA_CONNECT_TYPE_INDEX,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct URI
        {
            public G2STRING_128 _rtsp_tcp;
            public G2STRING_128 _rtsp_http;
            public G2STRING_64 _profile_token;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct URI_vector
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public URI[] _uri;
            public uint _len;
        }

        public G2VERSION_NETWORK _version;
        public ADDRESS_TYPE _address_type;
        public COMMAND_PROTOCOL_TYPE _command_protocol_type;
        public MEDIA_STREAM_PROTOCOL_TYPE _media_stream_protocol_type;
        public MEDIA_STREAM_PROTOCOL_TYPE _media_record_protocol_type;
        public MEDIA_STREAM_PROTOCOL_TYPE _media_search_protocol_type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string _address;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string _address_resolved;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string _user_id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
        public string _password;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string _extra_server_address;
        public G2MAC_ADDRESS _mac;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)PORT_TYPE.MAX_PORT_INDEX)]
        public ushort[] _port;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string _profile_token;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string _rtsp_uri;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string _rtsp_uri_http;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ONVIF_SERVICE_PATH
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string _path;
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ONVIF_SERVICE_PATH_TYPE.MAX_SERVICE_PATH_INDEX)]
        public ONVIF_SERVICE_PATH[] _onvif_service_path;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ONVIF_SERVICE_PATH_EXTRA
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string _path_extra;
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ONVIF_SERVICE_PATH_EXTRA_TYPE.MAX_SERVICE_PATH_EXTRA_INDEX)]
        public ONVIF_SERVICE_PATH_EXTRA[] _onvif_service_path_extra;
        public URI_vector _uri;

        public void set_port(PORT_TYPE type, ushort port)
        {
            if (_port == null)
            {
                _port = new ushort[(int)PORT_TYPE.MAX_PORT_INDEX];
            }
            _port[(int)type] = port;
        }
        public ushort get_port(PORT_TYPE type) { return (_port != null) ? _port[(int)type] : (ushort)0; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SERVER_NETWORK_INFO
    {
        public enum PORT_TYPE
        {
            MIN_PORT_INDEX = 0,
            ADMIN_PORT = 0,
            WATCH_PORT,
            SEARCH_PORT,
            RECORD_PORT,
            AUDIO_PORT,
            WEB_PORT,
            RTSP_PORT,
            VNC_PORT,
            INEX_ADMIN_PORT,
            MAX_PORT_INDEX
        }
        public G2MAC_ADDRESS _mac;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public G2MAC_ADDRESS[] _mac_additional;
        public int _SSL_state;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)PORT_TYPE.MAX_PORT_INDEX)]
        public ushort[] _port;

        public G2SSL_STATE.TYPE SSL_state { get { return (G2SSL_STATE.TYPE)_SSL_state; } }
        public ushort get_port(PORT_TYPE type) { return (_port != null) ? _port[(int)type] : (ushort)0; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2CONNECT_OPTIONS
    {
        public static G2CONNECT_OPTIONS create()
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return options;
        }
        public uint _connection_timeout;
        public void set_default()
        {
            _connection_timeout = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2CONNECT_RES
    {
        public enum ERROR
        {
            UNKNOWN = -1,
            NO = 0,
            NETWORK_INFO_FIND,      // there is no device or no access right for device
            NETWORK_INFO_RESOLVE,   // FEN(or DVRNS), mDNS query fail
            ACTIVE_DIRECTORY_UNABLE_CLIENT,
            ADAPTOR_INCOMPATIABLE
        }
        public int _channel;
        public int _err;
        public int _err_dvrns;

        public ERROR err { get { return (ERROR)_err; } }
        public G2FEN_RESULT.TYPE err_dvrns { get { return (G2FEN_RESULT.TYPE)_err_dvrns; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2DEVICE_STATUS
    {
        public enum CONST
        {
            NUM_UNITS_MAX = 64,
        }
        public enum COMMON
        {
            NOT_VALUE = -100,
            UNKNOWN = -1,
            INACTIVE = 0,
            ACTIVE = 1,
        }
        public enum CAMERA
        {
            VIDEOLOSS = -1,
            INACTIVE = 0,
            ACTIVE = 1,
            NOTCONNECTED = 2,
            MULTISTREAM = 3
        }
        public enum PTZ
        {
            UNKNOWN = -1,
            INACTIVE = 0,
            ACTIVE = 1,
            ADVANCED = 2
        }
        public enum PTZ_FUNCTION
        {
            PAN_TILT = 1,
            ZOOM = 2,
            FOCUS = 4,
            IRIS = 8,
            PRESET_SETUP = 16,
            PRESET_MOVE = 32,
            OSD_MENU = 64,
            ADVANCED = 128,
            FOCUS_ONEPUSH = 256,
            OSD_MENU_OFF_NONE = 512,
            RELATIVE_MOVE = 4096,
            ABSOLUTE_MOVE = 8192
        }
        public enum COVERT
        {
            UNKNOWN = -1,
            NORMAL = 0,
            COVERT1 = 1,
            COVERT2
        }
        public enum AUDIO_IN
        {
            UNKNOWN = -1,
            INACTIVE = 0,
            MICROPHONE = 1,
            LINEIN
        }
        public enum ALARM_IN
        {
            UNKNOWN = -1,
            INACTIVE = 0,
            NC = 1, // normally closed
            NO = 2, // normally open
            SENSOR_ON = 4
        }
        public enum RECORDING
        {
            UNKNOWN = -1,
            NO_RECORD = 0,
            RECORD
        }

        public G2STRING_64 _sitename;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public G2STRING_32[] _camera_desc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _camera;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _ptz;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _covert;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _audio_in;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _audio_out;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public G2STRING_32[] _alarm_in_desc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _alarm_in;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public G2STRING_32[] _alarm_out_desc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _alarm_out;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public G2STRING_32[] _alarm_in_network_desc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _alarm_in_network;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public G2STRING_32[] _text_in_desc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _text_in;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public sbyte[] _recording;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public uint[] _stream_remote;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.NUM_UNITS_MAX)]
        public uint[] _ptz_function;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DEVICE_STATUS_STREAM_INFO
    {
        public enum STREAM_TYPE
        {
            ON = 0,
            OFF
        }

        public byte _type;
        public byte _codec;
        public uint _width;
        public uint _height;
        public byte _quality;
        public byte _bitrate_control_type;
        public float _ips;
        [MarshalAs(UnmanagedType.U1)]
        public bool _roi;   // multi-view streaming
        public G2STRING_32 _title;

        public STREAM_TYPE type { get { return (STREAM_TYPE)_type; } }
        public G2_PRODUCT_INFO_CAPS.CODEC_INFO_VIDEO.TYPE codec { get { return (G2_PRODUCT_INFO_CAPS.CODEC_INFO_VIDEO.TYPE)_codec; } }
        public G2_PRODUCT_COMMON.G2_IMAGE_QUALITY quality { get { return (G2_PRODUCT_COMMON.G2_IMAGE_QUALITY)_quality; } }
        public G2_PRODUCT_COMMON.G2_BITRATE_CONTROL_TYPE bitrate_control_type { get { return (G2_PRODUCT_COMMON.G2_BITRATE_CONTROL_TYPE)_bitrate_control_type; } }
        public bool is_on { get { return (STREAM_TYPE)_type == STREAM_TYPE.ON; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DEVICE_STATUS_IP_CAMERA_INFO
    {
        public enum DEWARPING_STATUS_TYPE
        {
            UNKNOWN = -1,
            OFF,
            ON
        }

        public sbyte _dewarping_status;

        public DEWARPING_STATUS_TYPE dewarping_status { get { return (DEWARPING_STATUS_TYPE)_dewarping_status; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DEVICE_STATUS_PTZ_ADVANCED_INFO
    {
        public int _speed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DEVICE_NAME
    {
        public int _number;
        public G2STRING_128 _name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2EVENT_INFO
    {
        public enum TYPE_LEVEL1
        {
            L1_UNKNOWN_TYPE = 0,
            L1_TIME = 1 << 16,
            L1_SYSTEM = 2 << 16,
            L1_DEVICE = 3 << 16,
            L1_USER = 4 << 16,
            L1_PREEVENT = 5 << 16,
        }
        public enum TYPE_LEVEL2
        {
            L2_UNKNOWN_TYPE = 0,

            // L1_TIME
            L2_CONTINUOUS = 1000,
            L2_PERIODIC,
            L2_ONE_TIME,

            // L1_SYSTEM
            L2_SERVICE_ADDED = 2000,
            L2_SERVICE_CONNECTED,
            L2_SERVICE_DISCONNECTED,

            L2_DEVICE_CONNECTED = 2500,
            L2_DEVICE_DISCONNECTED,
            L2_DEVICE_CONNECT_FAIL,
            L2_DEVICE_ADDED,
            L2_DEVICE_MODIFIED,
            L2_DEVICE_REMOVED,
            L2_NETWORK_CAMREA_CONNECTED,
            L2_NETWORK_CAMERA_DISCONNECTED,

            // camera conditions
            L2_CAMERA_EVENT_BEGIN = 3099,
            L2_CAMERA_MOTION_DETECTION_ON = 3100,
            L2_CAMERA_MOTION_DETECTION_OFF,
            L2_CAMERA_OBJECT_DETECTION_ON,
            L2_CAMERA_OBJECT_DETECTION_OFF,
            L2_CAMERA_VIDEO_LOSS_ON,
            L2_CAMERA_VIDEO_LOSS_OFF,
            L2_CAMERA_VIDEO_BLIND_ON,
            L2_CAMERA_VIDEO_BLIND_OFF,
            L2_CAMERA_IGNORED_MOTION_ON,
            L2_CAMERA_VIDEO_ANALYTICS_ON,
            L2_CAMERA_VIDEO_ANALYTICS_OFF,
            L2_CAMERA_TRIPZONE_ON,
            L2_CAMERA_TRIPZONE_OFF,
            L2_CAMERA_TAMPER_ON,
            L2_CAMERA_TAMPER_OFF,
            L2_CAMERA_IGNORED_TRIPZONE_ON,
            L2_CAMERA_IGNORED_VIDEO_ANALYTICS_ON,
            L2_CAMERA_FENCE_DETECTION,              // deprecated
            L2_CAMERA_LOITERING,                    // deprecated
            L2_CAMERA_ABANDONED_OBJECT_DETECTION,   // deprecated
            L2_CAMERA_REMOVED_OBJECT_DETECTION,     // deprecated
            L2_CAMERA_TRAFFIC,                      // deprecated
            L2_CAMERA_DIRECTION_DETECTION,          // deprecated
            L2_CAMERA_FACE_DETECTION,               // deprecated
            L2_CAMERA_FACE_DETECTION_ON,
            L2_CAMERA_IGNORED_FACE_DETECTION_ON,
            L2_CAMERA_FACE_DETECTION_OFF,
            L2_CAMERA_INSTANT_RECORD_ON,
            L2_CAMERA_INSTANT_RECORD_OFF,
            L2_CAMERA_RECORD_BAD_ON,
            L2_CAMERA_RECORD_BAD_OFF,
            L2_CAMERA_FAN_ERROR_ON,
            L2_CAMERA_FAN_ERROR_OFF,
            L2_CAMERA_EVENT_END,

            // alarm in conditions
            L2_ALARM_EVENT_BEGIN = 3199,
            L2_ALARMIN_ON = 3200,
            L2_ALARMIN_OFF,
            L2_NETWORK_ALARMIN_ON,
            L2_NETWORK_ALARMIN_OFF,
            L2_ALARMIN_BAD,
            L2_ALARM_RESET_IN,
            L2_USER_DEFINED_ALARMIN_ON,
            L2_USER_DEFINED_ALARMIN_OFF,
            L2_ALARM_EVENT_END,

            // audio conditions
            L2_AUDIO_EVENT_BEGIN = 3299,
            L2_AUDIO_ON = 3300,
            L2_AUDIO_OFF,
            L2_AUDIO_IGNORED_AUDIO_ON,
            L2_AUDIO_EVENT_END,

            L2_TEXT_IN_EVENT_BEGIN = 3399,
            L2_TEXT_IN_ON = 3400,
            L2_TEXT_IN_OFF,
            L2_TEXT_IN_BAD_ON,
            L2_TEXT_IN_BAD_OFF,
            L2_TEXT_IN_EVENT_END,

            // L1_USER
            L2_USER_EVENT_BEGIN = 3999,
            L2_UNKNOWN_USER_TYPE = 4000,
            L2_USER_LOGIN,
            L2_USER_LOGOUT,
            L2_USER_AUTO_LOGOUT,
            L2_USER_AWAY,
            L2_USER_EVENT_END,

            // L1_DEVICE (DVR)
            L2_DVR_EVENT_BEGIN = 4999,
            L2_DVR_TANGO_ALMOST_FULL_ON = 5000,
            L2_DVR_TANGO_ALMOST_FULL_OFF,
            L2_DVR_PANIC_RECORD_ON,
            L2_DVR_PANIC_RECORD_OFF,
            L2_DVR_RECORDER_BAD_ON,
            L2_DVR_RECORDER_BAD_OFF,
            L2_DVR_FAN_ERROR_ON,
            L2_DVR_FAN_ERROR_OFF,
            L2_DVR_SYSTEM_BOOTUP,
            L2_DVR_SYSTEM_RESTART,
            L2_DVR_SYSTEM_SHUTDOWN,
            L2_DVR_DISK_FULL_ON,
            L2_DVR_DISK_FULL_OFF,
            L2_DVR_DISK_BAD_ON,
            L2_DVR_DISK_BAD_OFF,
            L2_DVR_DISK_TEMPERATURE_ON,
            L2_DVR_DISK_TEMPERATURE_OFF,
            L2_DVR_DISK_SMART_ON,
            L2_DVR_DISK_SMART_OFF,
            L2_DVR_DISK_ON,
            L2_DVR_DISK_OFF,
            L2_DVR_DISK_CONFIG_CHANGE,
            L2_DVR_COVER_OPEN,
            L2_DVR_COVER_CLOSE,
            L2_DVR_CAR_OVERSPEED_ON,
            L2_DVR_CAR_OVERSPEED_OFF,
            L2_DVR_CAR_SUDDEN_ACCLERATION,
            L2_DVR_CAR_SUDDEN_STOP,
            L2_DVR_CAR_STARTING_WITH_DOORS_OPEN,
            L2_DVR_SYSTEM_ALIVE,
            L2_DVR_GPS_RECEIVE_ERROR_ON,
            L2_DVR_GPS_RECEIVE_ERROR_OFF,
            L2_DVR_LOGIN_FAILED_SEVERAL_TIMES,
            L2_DVR_NO_DISK,
            L2_DVR_STOP_RECORD_ON,
            L2_DVR_EVENT_END,

            L2_SECOM_EVENT_BEGIN = 6999,
            L2_SECOM_FS_LOOP = 7000,
            L2_SECOM_FS_PANIC,
            L2_SECOM_FS_CARD,
            L2_SECOM_FS_MACHINE,
            L2_SECOM_EVENT_END,

            L2_SIPASS_EVENT_BEGIN = 7499,
            L2_SIPASS_RECORD_ON = 7500,
            L2_SIPASS_RECORD_OFF,
            L2_SIPASS_EVENT_END
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CALLBACK_SITE
        {
            public G2STRING_64 _site;
            public G2MAC_ADDRESS _mac;
            [MarshalAs(UnmanagedType.U1)]
            public bool _is_name;
            [MarshalAs(UnmanagedType.U1)]
            public bool _case_insensitive;
        }

        [StructLayout(LayoutKind.Sequential)]
        public partial struct G2EVENT_RAS
        {
            public int _seq_number;
            public int _level;
            public int _channel;
            public uint _cameras;
            public G2SPOT _spot;
            public G2STRING_128 _label;
            public CALLBACK_SITE _site;
            [MarshalAs(UnmanagedType.U1)]
            public bool _g2;
            public G2EVENT _g2event;
            public G2EVENT.LEVEL level { get { return (G2EVENT.LEVEL)_level; } }
        }

        public int _level1;
        public int _level2;
        public G2GUID _source;
        public G2GUID _target;
        public G2SPOT _spot;
        public G2TIME _local_time;
        public G2STRING_64 _label;
        public G2EVENT_RAS _event_ras;
        [MarshalAs(UnmanagedType.U1)]
        public bool _is_evt_onetime;
        [MarshalAs(UnmanagedType.U1)]
        public bool _is_evt_dvr;
        [MarshalAs(UnmanagedType.U1)]
        public bool _is_evt_dvr_system;
        [MarshalAs(UnmanagedType.U1)]
        public bool _is_evt_monitoring; // event made by monitoring service

        public TYPE_LEVEL1 level1
        {
            get { return (TYPE_LEVEL1)_level1; }
            set { _level1 = (int)value; }
        }
        public TYPE_LEVEL2 level2
        {
            get { return (TYPE_LEVEL2)_level2; }
            set { _level2 = (int)value; }
        }
        public bool has_g2event { get { return _event_ras._g2; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2EVENT_LOG
    {
        long _row_id;
        public G2EVENT_INFO.TYPE_LEVEL1 _level1;
        public G2EVENT_INFO.TYPE_LEVEL2 _level2;
        public G2GUID _source;
        public G2GUID _service;
        public uint _action_type;
        public G2CHANNEL_SET _target;
        public G2CHANNEL_SET _target_working;
        public G2STRING_128 _data;
        public G2SPOT _spot_server;
        public G2TIME _time_source;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SYSTEM_LOG
    {
        public long _row_id;
        public int _id;
        public G2GUID _guid1;
        public G2GUID _guid2;
        public G2TIME _time;
        public G2STRING_256 _data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DEBUG_LOG
    {
        public long _row_id;
        public G2TIME _time;
        public G2STRING_256 _data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2EVENT_LOG_LIST
    {
        public IntPtr _events;
        public uint _len;
        public G2EVENT_LOG[] to()
        {
            G2EVENT_LOG[] list = new G2EVENT_LOG[_len];
            for (int i = 0; i < list.Length; ++i)
            {
                IntPtr ptr = new IntPtr(_events.ToInt64() + i * Marshal.SizeOf(typeof(G2EVENT_LOG)));
                list[i] = (G2EVENT_LOG)Marshal.PtrToStructure(ptr, typeof(G2EVENT_LOG));
            }
            return list;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_SYSTEM_LOG
    {
        public G2GUIDSET _GUIDs;
        public G2GUIDSET _GUIDs_reserved;
        public g2set<int> _IDs;
        public long _row_id;
        public uint _request_count;

        [StructLayout(LayoutKind.Sequential)]
        public struct PARAM_TYPE
        {
            public G2PARAM_GUID_LIST _GUIDs;
            public G2PARAM_GUID_LIST _GUIDs_reserved;
            public G2PARAM_LIST_int32 _IDs;
            public long _row_id;
            public uint _request_count;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_DEBUG_LOG
    {
        public long _row_id;
        public uint _request_count;

        [StructLayout(LayoutKind.Sequential)]
        public struct PARAM_TYPE
        {
            public long _row_id;
            public uint _request_count;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_EVENT_LOG
    {
        public enum DIRECTION
        {
            BACKWARD = -1,
            FORWARD = 1,
            NO_DIRECTION
        }
        public enum TYPE
        {
            RECORDING = 0,
            BACKUP
        }

        public G2GUIDSET _source;
        public g2set<int> _event;
        public g2set<int> _action;
        public g2channel_set _channels;
        public G2SCOPE _scope;
        public DIRECTION _direction;
        public TYPE _type;
        public long _row_id;
        public uint _request_count;
        public bool _retrieve_removed_device;
        public bool _union_result;

        [StructLayout(LayoutKind.Sequential)]
        public struct PARAM_TYPE
        {
            public G2PARAM_GUID_LIST _source;
            public G2PARAM_LIST_int32 _event;
            public G2PARAM_LIST_int32 _action;
            public G2CHANNEL_SET _channels;
            public G2SCOPE _scope;
            public int _direction;
            public int _type;
            public long _row_id;
            public uint _request_count;
            [MarshalAs(UnmanagedType.U1)]
            public bool _retrieve_removed_device;
            [MarshalAs(UnmanagedType.U1)]
            public bool _uion_result;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG
    {
        public G2GUIDSET _source;
        public G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS _options;

        [StructLayout(LayoutKind.Sequential)]
        public struct PARAM_TYPE
        {
            public G2PARAM_GUID_LIST _source;
            public G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS _options;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_ACTION_ACK_LOG
    {
        public G2GUIDSET _source;
        public g2set<int> _event;
        public G2SCOPE _scope;
        public G2GUID _sender;
        public G2GUID _receiver;
        public long _row_id;
        public uint _request_count;
        public int _action;
        public int _ack;

        [StructLayout(LayoutKind.Sequential)]
        public struct PARAM_TYPE
        {
            public G2PARAM_GUID_LIST _source;
            public G2PARAM_LIST_int32 _event;
            public G2SCOPE _scope;
            public G2GUID _sender;
            public G2GUID _receiver;
            public long _row_id;
            public uint _request_count;
            public int _action;
            public int _ack;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2CLIPCOPY_JOB
    {
        public enum TYPE
        {
            READY = 0,
            MEASURE_SIZE = 1,
            FORMAT_STORAGE = 3,
            COPY_DATA = 4,
            COPY_PLAYER = 5,
            FINALIZE = 6
        }

        public uint _job;
        public uint _num;
        public uint _total;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2CLIPCOPY_STATUS
    {
        public enum TYPE
        {
            NOT_COMPLETED = -1,
            NOT_ENOUGH_SPACE = 0,
            PARTIAL_COPIABLE,
            FULL_COPIABLE,
            NO_RECORDED_DATA
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2CLIPCOPY_ERROR
    {
        public enum TYPE
        {
            NONE = 0,
            CANCELED,
            BANK_UPDATED,
            STORAGE_FORMAT_FAILED,
            STORAGE_OPEN_FAILED,
            CLIP_COPY_EXE_NOT_FOUND,
            CLIP_COPY_EXE_COPY_FAILED,
            DATA_COPY_FAILED,
            NO_DATA_IN_SCOPE,
            FINALIZE_FAILED,
            CLIP_COPY_STORAGE_FULL,
            LESS_FREE_SPACE_THAN_MIN_SIZE,
            INVALID_FINGER_PRINT_FOUND,
            INVALID_SKIP_INTERVAL,
            FILE_IO_FAILED
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2CLIPCOPY_SIZE_INFO
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct INFO
        {
            public ulong _size;
            public G2SCOPE _scope;
        }

        [MarshalAs(UnmanagedType.U1)]
        public bool _use_peer_player;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public INFO[] _info;
        public uint _info_len;

        public bool empty() { return _info_len == 0; }
        public INFO[] get_info()
        {
            INFO[] ret = new INFO[_info_len];
            Array.Copy(_info, ret, ret.Length);
            return ret;
        }
        public G2SCOPE get_total_scope() { return (_info_len == 0) ? G2SCOPE.INVALID_SCOPE : new G2SCOPE(_info[0]._scope._begin, _info[_info_len - 1]._scope._end); }
        public ulong get_total_size()
        {
            ulong ret = 0UL;
            for (uint i = 0; i < _info_len; ++i)
            {
                ret += _info[i]._size;
            }
            return ret;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2CLIPCOPY_DATA
    {
        public ulong _offset;
        public uint _size;
        public uint _progress;
        public IntPtr _data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2ROLLBACK_INFO
    {
        public int _channelext;
        public int _precision;
        public G2SPOT _spot;

        public G2ROLLBACK_INFO(int channelext, G2SPOT spot, G2PLAYER.PRECISION precision)
        {
            _channelext = channelext;
            _precision = (int)precision;
            _spot = spot;
        }
        public G2ROLLBACK_INFO(int channelext, G2SPOT spot)
        {
            _channelext = channelext;
            _precision = (int)G2PLAYER.PRECISION.TICK_SECOND;
            _spot = spot;
        }
        public G2ROLLBACK_INFO(int channelext)
        {
            _channelext = channelext;
            _precision = (int)G2PLAYER.PRECISION.TICK_SECOND;
            _spot = G2SPOT.INVALID_SPOT;
        }

        public G2PLAYER.PRECISION precision
        {
            get { return (G2PLAYER.PRECISION)_precision; }
            set { _precision = (int)value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PLAYBACK_COMMAND
    {
        public int _speed;
        public G2ROLLBACK_INFO _rbi;

        public G2PLAYER.COMMAND_AND_SPEED speed
        {
            get { return (G2PLAYER.COMMAND_AND_SPEED)_speed; }
            set { _speed = (int)value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2PLAYER
    {
        public enum COMMAND_AND_SPEED
        {
            NONE = 0,    // or stop

            // play-command
            BACK_FASTEST = -5,
            BACK_FASTER = -4,
            BACK_FAST = -3,
            BACK_NORMAL = -2,
            BACK_SLOW = -1,
            PLAY_SLOW = +1,
            PLAY_NORMAL = +2,
            PLAY_FAST = +3,
            PLAY_FASTER = +4,
            PLAY_FASTEST = +5,

            // move-command
            COMMAND_MOVE = 1000000,
            MOVE_TO_FIRST,
            MOVE_TO_LAST,
            MOVE_TO_SPOT,
            PREV_STEP,
            NEXT_STEP,
            RELOAD_CURRENT,
            RELOAD_RECENT,

            // search-command
            COMMAND_SEARCH = 2000000,
            MOTION_SEARCH,

            // special commands (not used manually by user)
            COMMAND_SPECIAL = 3000000,
            ROLLBACK,
        }

        public enum PRECISION
        {
            KEY = 0,
            FRAME,
            TEXT_IN,
            EVENT,

            HOUR,
            MINUTE,
            SECOND,
            TICK,
            TICK_SECOND
        }

        public enum PLAYER_ERROR
        {
            HANG_ON_FAILED = 0,
            BANK_UPDATED,
            CONNECTION_LOST,
            UNKNOWN
        }

        public enum OUT_OF_SCOPE
        {
            BEGIN_OF_SCOPE = 0,
            BEGIN_OF_SCOPE_PLAYED,
            END_OF_SCOPE_PLAYED,
            END_OF_SCOPE,
        }

        public enum AUDIO_PLAY
        {
            NO = 0,
            ALL,
            ASSOCIATED_ONLY
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2RECORD_TYPE_INFO
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ELEMENT_TYPE
        {
            public int _channelext;
            public int _rec_type;

            public G2FRAME.FLAG record_type { get { return (G2FRAME.FLAG)_rec_type; } }
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public ELEMENT_TYPE[] _elements;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2RECORD_TIME_INFO
    {
        public enum RESOLUTION
        {
            MINUTE = 0,
            HOUR = 1,
            DAY = 2,
            MONTH = 3,
            SECOND = 4
        }
        public enum DIRECTION
        {
            BACKWARD = -1,
            FORWARD = 1
        }
        public enum COMMAND
        {
            UNKNWON = 0,
            INIT,
            BALANCE_BACKWARD,
            BALANCE_FORWARD,
            OVERWRITE_BACKWARD,
            OVERWRITE_FORWARD
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public G2RECORD_TYPE_INFO[] _record_type;
        public RESOLUTION _resolution;
        public G2CHANNEL_SET _channels;
        public G2SPOT _spot;
        public int _time_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2EVENT_DISK
    {
        public enum DISK_TYPE
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

        public int _type;
        public int _number;
        public int _raid_index;

        public DISK_TYPE type { get { return (DISK_TYPE)_type; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2EVENT_GPS
    {
        public float _lat;
        public float _lon;
        public float _speed;
        public float _angle;
        public G2TIME _time;  // time on GPS
        public G2FIXED_BUF_256 _data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2EVENT_TEXT_IN
    {
        public enum TRANSACTION
        {
            TRANSACTION_BEGIN = 0,
            TRANSACTION_CONTINUE,
            TRANSACTION_END,
            TRANSACTION_COMPLETE,
        }

        public int _transaction;
        public int _system_id;
        [MarshalAs(UnmanagedType.U1)]
        public bool _bad;
        public G2FIXED_BUF_256 _data;

        TRANSACTION transaction { get { return (TRANSACTION)_transaction; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2EVENT_NETWORK_ALARM
    {
        public G2FIXED_BUF_SBYTE_2 _version;
        public int _event;
        public G2TIME _time;
        public long _msec;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2EVENT_USER_DEFINED
    {
        public ushort _event;   // user defined type
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct G2EVENT_UNION
    {
        [FieldOffset(0)]
        public G2EVENT_DISK _disk;
        [FieldOffset(0)]
        public G2EVENT_GPS _gps;
        [FieldOffset(0)]
        public G2EVENT_TEXT_IN _text_in;
        [FieldOffset(0)]
        public G2EVENT_NETWORK_ALARM _network_alarm;
        [FieldOffset(0)]
        public G2EVENT_USER_DEFINED _user_defined;
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2EVENT
    {
        public enum LEVEL
        {
            NORMAL = 0,
            EMERGENCY,
            ERROR
        }
        public enum TYPE
        {
            TYPE_NONE = 0,
            TYPE_UNKNOWN = 0x7FFFFFFF,
            ALARM_IN_ON = 1,
            ALARM_IN_OFF = 2,
            ALARM_IN_BAD_ON = 3,
            ALARM_IN_BAD_OFF = 4,
            USER_DEFINED_ALARM_ON = 5,
            USER_DEFINED_ALARM_OFF = 6,
            MOTION_ON = 100,
            MOTION_OFF = 101,
            OBJECT_TRACK_ON = 102,
            OBJECT_TRACK_OFF = 103,
            TRIPZONE_ON = 104,
            TRIPZONE_OFF = 105,
            VIDEO_ANALYTICS_ON = 120,
            VIDEO_ANALYTICS_OFF = 121,
            IGNORED_MOTION_ON = 122,
            IGNORED_TRIPZONE_ON = 123,
            VIDEO_INIT = 200,
            VIDEO_LOSS_ON = 201,
            VIDEO_LOSS_OFF = 202,
            VIDEO_BLIND_ON = 203,
            VIDEO_BLIND_OFF = 204,
            TAMPER_ON = 205,
            TAMPER_OFF = 206,
            TEXT_IN_ON = 300,
            TEXT_IN_OFF = 301,
            TEXT_IN_DATA = 302,
            TEXT_IN_BAD_ON = 303,
            TEXT_IN_BAD_OFF = 304,
            FACE_DETECTION_ON = 400,
            FACE_DETECTION_OFF = 401,
            IGNORED_FACE_DETECTION_ON = 402,
            TANGO_FULL_ON = 100000,
            TANGO_FULL_OFF = 100001,
            TANGO_ALMOST_FULL_ON = 100002,
            TANGO_ALMOST_FULL_OFF = 100003,
            DISK_BAD = 200000,
            DISK_TEMPERATURE_ON = 200001,
            DISK_TEMPERATURE_OFF = 200002,
            DISK_SMART_ON = 200003,
            DISK_SMART_OFF = 200004,
            DISK_CONFIG_CHANGE = 200005,
            DISK_ON = 200006,
            DISK_OFF = 200007,
            NO_DISK = 200008,
            SYSTEM_ALIVE = 300000,
            PANIC_ON = 300001,
            PANIC_OFF = 300002,
            FAN_ERROR_ON = 300003,
            FAN_ERROR_OFF = 300004,
            SYSTEM_BOOT_UP = 300005,
            SYSTEM_RESTART = 300006,
            SYSTEM_SHUTDOWN = 300007,
            COVER_OPEN = 300008,
            COVER_CLOSE = 300009,
            STOP_RECORD_ON = 300010,
            LOGIN_FAILED_SEVERAL_TIMES = 300011,
            USER_LOGIN = 300012,
            USER_LOGOUT = 300013,
            SETUP_CHANGED = 300014,
            RECORDER_BAD_ON = 400000,
            RECORDER_BAD_OFF = 400001,
            INSTANT_RECORDING_ON = 400002,
            INSTANT_RECORDING_OFF = 400003,
            CAMERA_RECORD_BAD_ON = 400004,
            CAMERA_RECORD_BAD_OFF = 400005,
            AUDIO_ON = 500000,
            AUDIO_OFF = 500001,
            IGNORED_AUDIO_ON = 500002,
            USER_DEFINED = 600000,
            CAMERA_FAN_ERROR_ON = 700000,
            CAMERA_FAN_ERROR_OFF = 700001,
            NETWORK_ALARM_ON = 1000000,
            NETWORK_ALARM_OFF = 1000001,
            NETWORK_CAMERA_CONNECTED = 1000010,
            NETWORK_CAMERA_DISCONNECTED = 1000011,
            CHANNEL_RELATED_SEPARATOR = 1100000000,
            CAR_OVERSPEED_ON = 1100000000,
            CAR_OVERSPEED_OFF = 1100000001,
            CAR_SUDDEN_ACCELERATION = 1100000002,
            CAR_SUDDEN_STOP = 1100000003,
            CAR_STARTING_WITH_DOORS_OPEN = 1100000004,
            GPS_RECEIVE_ERROR_ON = 1100000100,
            GPS_RECEIVE_ERROR_OFF = 1100000101,
            GPS_DATA = 1100000102,
            SIPASS_RECORD_ON = 1200000000,
            SIPASS_RECORD_OFF = 1200000001,
            SECOM_SMART_LOOP = 1210000000,
            SECOM_SMART_PANIC = 1210000001,
            SECOM_SMART_CARD = 1210000002,
            SECOM_SMART_MACHINE = 1210000003,
            FENCE_DETECTION = 1220000000
        }
        public enum INFO_TYPE
        {
            NONE = 0,
            DISK,
            GPS,
            TEXT_IN,
            NETWORK_ALARM,
            USER_DEFINED
        }

        public int _type;
        public int _level;
        public uint _channel;
        public G2SPOT _spot;
        public G2STRING_256 _data;
        public G2CHANNEL_SET _associated_channels;
        public int _info_type;
        public G2EVENT_UNION _info;

        public LEVEL level
        {
            get { return (LEVEL)_level; }
            set { _level = (int)value; }
        }
        public TYPE type
        {
            get { return (TYPE)_type; }
            set { _type = (int)value; }
        }
        public INFO_TYPE info_type
        {
            get { return (INFO_TYPE)_info_type; }
            set { _info_type = (int)value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2EVENT_LIST
    {
        public IntPtr _events;
        public uint _len;
        public G2EVENT[] to()
        {
            G2EVENT[] list = new G2EVENT[_len];
            for (int i = 0; i < list.Length; ++i)
            {
                IntPtr ptr = new IntPtr(_events.ToInt64() + i * Marshal.SizeOf(typeof(G2EVENT)));
                list[i] = (G2EVENT)Marshal.PtrToStructure(ptr, typeof(G2EVENT));
            }
            return list;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2INSTANT_RECORDING_SUB_SETTING_NVR
    {
        public enum RECORD_PROFILE
        {
            VERYHIGH = 0,
            HIGH = 1,
            STANDARD = 2,
            BASIC = 3
        }

        public uint _record_profile;

        public RECORD_PROFILE record_profile { get { return (RECORD_PROFILE)_record_profile; } }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct G2INSTANT_RECORDING_SUB_SETTING_HANA
    {
        public enum RECORD_PROFILE
        {
            VERYHIGH = 0,
            HIGH = 1,
            STANDARD = 2,
            BASIC = 3
        }
        public enum QUALITY
        {
            LOW = 0,
            NORMAL = 1,
            HIGH = 2,
            VERY_HIGH = 3
        }
        public enum RESOLUTION
        {
            STANDARD = 0,   // CIF
            HIGH = 1,       // 2CIF
            VERY_HIGH = 2   // 4CIF
        }

        public uint _record_profile;    // for IP camera
        public uint _ips;               // for analog camera
        public byte _quality;
        public byte _resolution;

        public RECORD_PROFILE record_profile { get { return (RECORD_PROFILE)_record_profile; } }
        public QUALITY quality { get { return (QUALITY)_quality; } }
        public RESOLUTION resolution { get { return (RESOLUTION)_resolution; } }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct G2INSTANT_RECORDING_SUB_SETTING_TVI
    {
        public enum QUALITY
        {
            LOW = 0,
            NORMAL = 1,
            HIGH = 2,
            VERY_HIGH = 3
        }
        public enum RESOLUTION
        {
            STANDARD = 0,   // CIF
            HIGH = 1,       // 2CIF
            VERY_HIGH = 2   // 4CIF
        }

        public uint _ips;
        public byte _quality;
        public byte _resolution;

        public QUALITY quality { get { return (QUALITY)_quality; } }
        public RESOLUTION resolution { get { return (RESOLUTION)_resolution; } }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct G2INSTANT_RECORDING_SUB_SETTING_UNION
    {
        [FieldOffset(0)]
        public G2INSTANT_RECORDING_SUB_SETTING_NVR _nvr;
        [FieldOffset(0)]
        public G2INSTANT_RECORDING_SUB_SETTING_HANA _hana;
        [FieldOffset(0)]
        public G2INSTANT_RECORDING_SUB_SETTING_TVI _tvi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2INSTANT_RECORD_SETTING
    {
        public enum PRE_DURATION
        {
            USE_EXISTING_SETUP = -1,
            STOP_PRE_EVENT = 0,
        }
        public enum SUB_SETTING_TYPE
        {
            UNKNOWN = 0,
            NVR = 1,
            HANA = 2,   // PC-based products
            TVI = 3
        }

        public int _channel;
        public long _duration_pre;
        public uint _duration_post;
        public byte _sub_setting_type;
        public G2INSTANT_RECORDING_SUB_SETTING_UNION _sub_setting;

        public SUB_SETTING_TYPE sub_setting_type
        {
            get { return (SUB_SETTING_TYPE)_sub_setting_type; }
            set { _sub_setting_type = (byte)value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2INSTANT_RECORD_SETTING_LIST
    {
        public IntPtr _list;
        public uint _len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2INSTANT_RECORDING_CHANNEL_STATUS
    {
        public enum FAIL_REASON
        {
            SUCCESS = 0,
            INVALID_PARAM_CHANNEL,
            INVALID_PARAM_POST_DURATION,
            INVALID_PARAM_PRE_DURATION,
            INVALID_PARAM_SUB_SETTING_TYPE,
            INVALID_PARAM_SUB_SETTING_VALUE,
            NO_AUTHORITY,
            INTERNAL_SYSTEM_ERROR,
            PANIC_ON,
            STORAGE_NOT_FOUND,
            STORAGE_FULL,
            CAMERA_NOT_REGISTERED,
            CAMERA_DEACTIVATED,
            CAMERA_VIDEO_LOSS,
            NOT_SUPPORTED,
            UNKNOWN,
            STORAGE_PARTIALLY_FULL
        }

        public int _channel;
        public long _duration_pre;
        public uint _duration_post;
        [MarshalAs(UnmanagedType.U1)]
        public bool _on_recording;
        [MarshalAs(UnmanagedType.U1)]
        public bool _on_recording_audio;
        public G2SCOPE _scope;
        public int _fail_reason;
        public int _storage_groupId;

        public FAIL_REASON fail_reason { get { return (FAIL_REASON)_fail_reason; } }
    }

    public struct G2INSTANT_RECORDING_RESULT
    {
        public enum TYPE
        {
            SUCCESS,
            PARTIAL_SUCCESS,
            FAIL_NO_AUTHORITY,
            FAIL_SYSTEM_BUSY,
            FAIL_STORAGE_NOT_FOUND,
            FAIL_STORAGE_FULL,
            FAIL_PANIC_ON,
            FAIL_UNKNOWN
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS
    {
        public IntPtr _list;
        public uint _len;
        public uint _result;

        public G2INSTANT_RECORDING_RESULT.TYPE result { get { return (G2INSTANT_RECORDING_RESULT.TYPE)_result; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2RAS_AUTHORITY
    {
        public enum TYPE
        {
            AUTHORITY_SHUTDOWN_RESTART = 1,
            AUTHORITY_UPGRADE = 2,
            AUTHORITY_SYSTEM_TIME_CHANGE = 4,
            AUTHORITY_DATA_CLEAR = 8,
            AUTHORITY_SETUP = 16,
            AUTHORITY_USER_MANAGE = 32,
            AUTHORITY_COLOR_CONTROL = 64,
            AUTHORITY_PTZ_CONTROL = 128,
            AUTHORITY_ALARM_OUT_CONTROL = 256,
            AUTHORITY_COVERT_CAMERA_VIEW = 512,
            AUTHORITY_SYSTEM_CHECK = 1024,
            AUTHORITY_RECORD_SETUP = 2048,
            AUTHORITY_SEARCH = 4096,
            AUTHORITY_CLIP_COPY = 8192,
            AUTHORITY_PTZ_SETUP = 16384,
            AUTHORITY_ALARM_OUT_SETUP = 32768,
            AUTHORITY_COVERT_CAMERA_SETUP = 65536,
            AUTHORITY_SETUP_IMPORT = 131072,
            AUTHORITY_SETUP_EXPORT = 262144,
            AUTHORITY_VNC_SETUP = 524288,
            AUTHORITY_ALARM_AUDIO_CONTROL = 1048576,
            AUTHORITY_ALARM_AUDIO_SETUP = 2097152,
            AUTHORITY_POWER_MANAGEMENT = 4194304,
            AUTHORITY_NETWORK_CAMERA_REGISTER = 8388608
        }
        public enum LEVEL
        {
            LEVEL_NONE = 0,
            LEVEL_USER = 1,
            LEVEL_ADMIN = 2,
            LEVEL_XDR = 3
        }

        public uint _authority;
        public uint _level;

        public bool is_authority(TYPE authority) { return (_authority & (uint)authority) != 0; }
        public LEVEL level
        {
            get { return (LEVEL)_level; }
            set { _level = (uint)value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public partial struct G2PROBE_SESSION_PROFILE
    {
        public int _channel;
        public int _SSL_state;
        public int _FEN_connection;
        public float _ips;
        public int _bps;    // kilo bits per second
        public int _bps_audio;

        public G2SSL_STATE.TYPE SSL_state { get { return (G2SSL_STATE.TYPE)_SSL_state; } }
        public G2FEN_CONNECTION.TYPE FEN_connection { get { return (G2FEN_CONNECTION.TYPE)_FEN_connection; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SIZE
    {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DT
    {
        public enum FORMAT
        {
            TOP = 0,
            LEFT = 0,
            CENTER = 1,
            RIGHT = 2,
            VCENTER = 4,
            BOTTOM = 8,
            WORDBREAK = 16,
            SINGLELINE = 32,
            EXPANDTABS = 64,
            TABSTOP = 128,
            NOCLIP = 256,
            EXTERNALLEADING = 512,
            CALCRECT = 1024,
            NOPREFIX = 2048,
            INTERNAL = 4096
        }
    }
}
