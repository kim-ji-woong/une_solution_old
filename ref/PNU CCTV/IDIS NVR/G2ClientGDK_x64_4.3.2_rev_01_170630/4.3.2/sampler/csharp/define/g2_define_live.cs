using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2LIVE_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_receive_stream_channels = 2,
            on_receive_frame_data = 3,
            on_receive_text_in = 4,
            on_receive_camera_status = 5,
            on_receive_event = 6,
            on_receive_ptz_menu = 7,
            on_receive_ptz_preset = 8,
            on_receive_service_log_load = 9,
            on_receive_service_log_load_end = 10,
            on_receive_service_log_load_fail = 11,
            on_receive_service_log_load_stop = 12,
            on_receive_audio_out_not_available = 13,
            on_receive_notify_append_device = 14,
            on_receive_notify_remove_device = 15,
            on_receive_network_alarm_result = 16,
            on_audio_streaming_started = 17,
            on_audio_streaming_stopped = 18,
            on_audio_capturing_started = 19,
            on_audio_capturing_stopped = 20,
            on_probe_session_profile = 21,
            on_receive_debug_log_load = 22,
            on_receive_debug_log_load_end = 23,
            on_receive_debug_log_load_fail = 24,
            on_receive_debug_log_load_stop = 25,

            CALLBACK_COUNT = 26
        }
    }

    public struct G2LIVE_SUPPORT
    {
        public enum QUERY
        {
            DRAW_MOTION = 1,
            STATUS_IDR = 3,
            IMAGE_CONFIG = 4,
            MULTI_STREAM = 5,
            AUDIO_STREAM_IN_WATCH_PORT = 6,
            BEEP_CONTROL = 8,
            HYBRID_STREAMING_VIDEO = 9,
            HYBRID_STREAMING_AUDIO_IN = 10,
            HYBRID_STREAMING_AUDIO_OUT = 11,
            PTZ_PRESET_G2 = 12,
            INSTANT_RECORDING = 129,
            NETWORK_ALARM_G2 = 516,
            SI_ELEVATOR_STATUS_INFO = 1000
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_CHANNEL_INFO
    {
        public G2GUID _guid;
        public int _channelext;
        public byte _stream_count;
        public uint _stream_remote; // multi-stream's mask
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_CAMERA_STATUS
    {
        public enum COMMON
        {
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
            PTZ_FUNC_RELATIVE_MOVE = 4096,
            PTZ_FUNC_ABSOLUTE_MOVE = 8192
        }
        public enum COVERT
        {
            COVERT_NONE = 0,
            COVERT_LEVEL1 = 1,
            COVERT_LEVEL2 = 2
        }
        public enum AUDIO_IN
        {
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

        public G2GUID _guid;
        public int _channelext;
        public sbyte _status;
        public sbyte _ptz;
        public uint _ptz_function;
        public sbyte _audio_in;
        public sbyte _alarm_in;
        public sbyte _covert;
        public uint _stream_remote;

        public CAMERA camera { get { return (CAMERA)_status; } }
        public PTZ ptz { get { return (PTZ)_ptz; } }
        public COVERT convert { get { return (COVERT)_covert; } }
        public AUDIO_IN audio_in { get { return (AUDIO_IN)_audio_in; } }
        public ALARM_IN alarm_in { get { return (ALARM_IN)_alarm_in; } }
        public uint stream_remote { get { return _stream_remote; } }
        public PTZ_FUNCTION ptz_function { get { return (PTZ_FUNCTION)_ptz_function; } }
    }

    public struct G2LIVE_CAMERA_COLOR
    {
        public enum TYPE
        {
            REVERT = -1,
            BRIGHTNESS = 0,
            CONTRAST,
            SATURATION,
            HUE
        }
        public enum VALUE
        {
            DEFAULT = 0,
            UP = 1,
            DOWN = -1
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_PTZ_COMMAND
    {
        public enum COMMAND
        {
            PTZ_MOVE_N = 0,
            PTZ_MOVE_NE,
            PTZ_MOVE_E,
            PTZ_MOVE_SE,
            PTZ_MOVE_S,
            PTZ_MOVE_SW,
            PTZ_MOVE_W,
            PTZ_MOVE_NW,
            PTZ_ZOOM_OUT,
            PTZ_ZOOM_IN,
            PTZ_FOCUS_NEAR,
            PTZ_FOCUS_FAR,
            PTZ_IRIS_CLOSE,
            PTZ_IRIS_OPEN,
            PTZ_MOVE_PRESET,
            PTZ_SET_PRESET,
            PTZ_STOP_MOVE,
            PTZ_STOP_ZOOM,
            PTZ_STOP_FOCUS,
            PTZ_STOP_IRIS,

            PTZ_SET_SPEED,      // [0:16)
            PTZ_MOVETO_ORIGIN,
            PTZ_AUTO_PAN_ON,
            PTZ_AUTO_PAN_OFF,
            PTZ_SCAN_ON,        // [1:16]
            PTZ_SCAN_OFF,
            PTZ_PATTERN_ON,     // [1:16]
            PTZ_PATTERN_OFF,
            PTZ_TOUR_ON,        // [1:16]
            PTZ_TOUR_OFF,

            PTZ_MENU_ON,
            PTZ_MENU_OFF,
            PTZ_MENU_UP,
            PTZ_MENU_DOWN,
            PTZ_MENU_RIGHT,
            PTZ_MENU_LEFT,
            PTZ_MENU_ENTER,
            PTZ_MENU_ESC,
            PTZ_NUMBERKEY,
            PTZ_SETCTRL_ON,

            PTZ_LIGHT_TURNON,
            PTZ_LIGHT_TURNOFF,
            PTZ_POWER_TURNON,
            PTZ_POWER_TURNOFF,
            PTZ_PUMP_TURNON,
            PTZ_PUMP_TURNOFF,
            PTZ_WIPER_TURNON,
            PTZ_WIPER_TURNOFF,
            PTZ_AUX_TURNON,
            PTZ_AUX_TURNOFF,

            PTZ_PATTERN_RECORD_ON,
            PTZ_PATTERN_RECORD_OFF,

            PTZ_FOCUS_ONEPUSH
        }

        G2LIVE_PTZ_COMMAND(COMMAND command)
        {
            _camera = G2GUID.INVALID_GUID;
            _command = (int)command;
            _argument = 0;
            _stream_id = -1;
            _reserved = 0;
        }

        public G2GUID _camera;
        public int _command;   // PTZ_MOVE_N, PTZ_MOVE_NE,...
        public int _argument;  // 0 : step, 1 : continuing & require stop command
        public int _stream_id;
        public int _reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_PTZ_MENU
    {
        public enum FUNCTION : uint
        {
            SPEED = 1,
            AUTOPAN = 2,
            TOUR = 4,
            PATTERN = 8,
            SCAN = 16,
            OSDMENU = 32,
            LIGHT = 64,
            PUMP = 128,
            WIPER = 256,
            POWER = 512,
            AUX = 1024,
            ORIGIN = 2048,
            OUTMONITOR = 4096,
            CTRL = 8192,
            ENTER = 16384,
            ESC = 32768,
            REC_PATTERN = 65536,
            OSDMENU_OFF_NONE = 131072,
        }

        public G2GUID _guid;
        public uint _function;
        public uint _function_ex;
        [MarshalAs(UnmanagedType.U1)]
        public bool _expanded;             // true : may use _function_ex
        [MarshalAs(UnmanagedType.U1)]
        public bool _active_CTRL;          // true : CTRL is active
        [MarshalAs(UnmanagedType.U1)]
        public bool _recording_pattern;    // true : pattern is recording

        public bool is_function(FUNCTION fn) { return (_function & (uint)fn) != 0; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_PTZ_PRESET
    {
        public enum VERSION
        {
            IDR = -1,
            G1 = 0,
            G2 = 1,
        }
        public enum CONST
        {
            MAX_NUM_PRESET_G2 = 256,
            MAX_NUM_PRESET_G1 = 16,
            MAX_NUM_PRESET_IDR = 128,
            MAX_NUM_PRESET = 128,
            MAX_LEN_PRESET_G2 = 32,
            MAX_LEN_PRESET = 16
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PRESET
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.MAX_LEN_PRESET + 1)]
            public byte[] _data;

            public string title
            {
                get { return (_data != null) ? Encoding.Default.GetString(_data) : ""; }
                set
                {
                    if (_data == null)
                    {
                        _data = new byte[(int)G2LIVE_PTZ_PRESET.CONST.MAX_LEN_PRESET + 1];
                    }

                    Array.Clear(_data, 0, _data.Length);
                    byte[] s = Encoding.Default.GetBytes(value);
                    Array.Copy(s, _data, Math.Min((int)G2LIVE_PTZ_PRESET.CONST.MAX_LEN_PRESET, s.Length));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct G1
        {
            public uint _count; // readonly
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.MAX_NUM_PRESET)]
            public PRESET[] _preset;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct G2
        {
            public uint _count; // readonly
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CONST.MAX_NUM_PRESET_G2)]
            public G2STRING_32[] _preset;
        }

        public int _version;
        public G2GUID _guid;
        public int _port;
        public int _model;
        public int _id;
        public int _number;
        public int _select;
        public G1 _g1;
        public G2 _g2;

        public bool is_G2 { get { return _version == (int)VERSION.G2; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_NETWORK_ALARM_INFO
    {
        public uint _seq_number;
        public G2FIXED_BUF_SBYTE_2 _version;
        public uint _id;    // channel of G2EVENT
        public int _level;
        public int _event;  // user defined type
        [MarshalAs(UnmanagedType.U1)]
        public bool _on;
        public G2STRING_128 _data;
        public G2TIME _time;
        public long _msec;

        public G2EVENT.LEVEL level
        {
            get { return (G2EVENT.LEVEL)_level; }
            set { _level = (int)value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_NETWORK_ALARM_RESULT
    {
        public enum TYPE
        {
            RESULT_OK = 0,
            FAIL_UNKNOWN,
            FAIL_NO_OPERATION,
            FAIL_DEVICE_INACTIVE
        }

        public uint _seq_number;
        public int _result;
        public G2EVENT _event;

        public TYPE result { get { return (TYPE)_result; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_NETWORK_ALARM_RESULT_PARAM
    {
        public G2GUID _guid;
        public G2LIVE_NETWORK_ALARM_RESULT _result;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_ELEVATOR_STATUS_INFO
    {
        public uint _seq_number;
        public G2CODEC_INFO_SI_ELEVATOR_STATUS _result;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_COMMAND_RESULT
    {
        public enum TYPE
        {
            UNKNOWN = 0,
            SUCCESS = 1,
            FAIL = 2
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_COMMAND_CONTROL_COLOR
    {
        public enum CHANGE
        {
            TO_DEFAULT = 0,
            BRIGHTNESS, CONTRAST, HUE, SATURATION,
            ALL
        }

        public ushort _brightness;
        public ushort _contrast;
        public ushort _hue;
        public ushort _saturation;
        public byte _change;
        public short _change_value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_COMMAND_CONTROL_COLOR_RANGE
    {
        public ushort _min_brightness;
        public ushort _max_brightness;
        public ushort _min_contrast;
        public ushort _max_contrast;
        public ushort _min_hue;
        public ushort _max_hue;
        public ushort _min_saturation;
        public ushort _max_saturation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_COMMAND_CONTROL_COLOR_STATUS
    {
        public G2LIVE_COMMAND_CONTROL_COLOR _control;
        public G2LIVE_COMMAND_CONTROL_COLOR_RANGE _range;
        public int _camera;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_COMMAND_CONTROL_COLOR_RESULT
    {
        public G2LIVE_COMMAND_CONTROL_COLOR _control;
        public int _camera;
        public int _result;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_COMMAND_CONTROL_PTZ
    {
        public enum TYPE : uint
        {
            ALL = 0xFFFFFFFF,
            PAN = 1,
            TILT = 2,
            ZOOM = 4,
            FOCUS = 8,
            IRIS = 16
        }
        public enum MOVE
        {
            ABSOLUTE = 0,
            VELOCITY = 1,   // for P/T: 0 < val clockwise, val < 0 counterclocksize
            // for Z  : 0 < val zoom-in, val < 0 zoom-out
            // for P/T/Z: abs(val) is speed(velocity)
            VELOCITY_BASED_ON_MIN_MAX = 2,
            POINT_BASED_ON_IMAGE = 3
        }

        public uint _types;
        public int _move;
        public float _pan;
        public float _tilt;
        public float _zoom;
        public float _focus;
        public float _iris;

        public TYPE types
        {
            get { return (TYPE)_types; }
            set { _types = (uint)value; }
        }
        public MOVE move
        {
            get { return (MOVE)_move; }
            set { _move = (int)value; }
        }
        public bool contains(TYPE type) { return (_types & (uint)type) != 0; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_COMMAND_CONTROL_PTZ_RANGE
    {
        public uint _types;
        public float _min_pan;
        public float _max_pan;
        public float _min_tilt;
        public float _max_tilt;
        public float _min_zoom;
        public float _max_zoom;
        public float _min_focus;
        public float _max_focus;
        public float _min_iris;
        public float _max_iris;

        public bool contains(G2LIVE_COMMAND_CONTROL_PTZ.TYPE type) { return (_types & (uint)type) != 0; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_COMMAND_CONTROL_PTZ_STATUS
    {
        public G2LIVE_COMMAND_CONTROL_PTZ _control;
        public G2LIVE_COMMAND_CONTROL_PTZ_RANGE _range;
        public int _camera;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_COMMAND_CONTROL_PTZ_RESULT
    {
        public int _camera;
        public int _result;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LIVE_PARAM_PTZ_PRESET
    {
        public G2LIVE_PTZ_PRESET _data;
        public int _camera;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct G2LIVE_PARAM_PTZ_MENU
    {
        public G2LIVE_PTZ_MENU _data;
        public int _camera;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct G2LIVE_PARAM_LIVE_AUDIO_OUT
    {
        public G2GUID _root;
        public int _camera;
    }
}
