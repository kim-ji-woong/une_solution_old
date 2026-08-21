// g2_define_live.h : header file
//

#ifndef _G2_DEFINE_LIVE_H_
#define _G2_DEFINE_LIVE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2LIVE_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_receive_stream_channels = 2,
        on_receive_frame_data = 3,
        on_receive_audio_data = 30,
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
        on_receive_command_result_control_color_status = 26,
        on_receive_command_result_control_color = 27,
        on_receive_command_result_control_ptz_status = 28,
        on_receive_command_result_control_ptz = 29,

        CALLBACK_COUNT = 31
    };
};

struct G2LIVE_SUPPORT {
    enum QUERY {
        DRAW_MOTION  = 1,
        STATUS_IDR   = 3,
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
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2LIVE_CHANNEL_INFO {
    G2GUID          _guid;
    int             _channelext;
    unsigned char   _stream_count;
    unsigned long   _stream_remote; // multi-stream's mask
};

struct G2LIVE_CAMERA_STATUS {
    enum STATUS {
        STATE_UNKNOWN  = -1,
        STATE_INACTIVE = 0,
        STATE_ACTIVE   = 1,
    };
    enum CAMERA {
        STATE_VIDEOLOSS    = -1,
        STATE_NOTCONNECTED = 2,
        STATE_MULTISTREAM  = 3
    };
    enum PTZ {
        PTZ_NONE     = 0,
        PTZ_NORMAL   = 1,
        PTZ_ADVANCED = 2
    };
    enum PTZ_FUNCTION {
        PTZ_FUNC_PAN_TILT = 0x0001,
        PTZ_FUNC_ZOOM     = 0x0002,
        PTZ_FUNC_FOCUS    = 0x0004,
        PTZ_FUNC_IRIS     = 0x0008,
        PTZ_FUNC_PRESET_SETUP = 0x0010,
        PTZ_FUNC_PRESET_MOVE = 0x0020,
        PTZ_FUNC_OSD_MENU = 0x0040,
        PTZ_FUNC_ADVANCED = 0x0080,
        PTZ_FUNC_FOCUS_ONEPUSH = 0x0100,
        PTZ_FUNC_OSD_MENU_OFF_NONE = 0x0200,
        PTZ_FUNC_RELATIVE_MOVE = 0x1000,
        PTZ_FUNC_ABSOLUTE_MOVE = 0x2000,
		PTZ_FUNC_RELATIVE_MOVE_IDIS = 0x4000
    };
    enum COVERT {
        COVERT_NONE   = 0,
        COVERT_LEVEL1 = 1,
        COVERT_LEVEL2 = 2
    };
    enum AUDIO_IN {
        MICROPHONE = 1,
        LINEIN
    };
    enum ALARM_IN {
        NORMALLY_CLOSED = 0x0001,
        NORMALLY_OPEN   = 0x0002,
        SENSOR_ON = 0x0004
    };

    G2GUID        _guid;
    int           _channelext;
    signed char   _status;
    signed char   _ptz;
    unsigned int  _ptz_function;
    signed char   _audio_in;
    signed char   _alarm_in;
    signed char   _covert;
    unsigned long _stream_remote;
};

struct G2LIVE_CAMERA_COLOR {
    enum TYPE {
        REVERT = -1,
        BRIGHTNESS = 0,
        CONTRAST,
        SATURATION,
        HUE
    };
    enum VALUE {
        DEFAULT = 0,
        UP = 1,
        DOWN = -1
    };
};

struct G2LIVE_PTZ_COMMAND {
    enum COMMAND {
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

        PTZ_SET_SPEED,
        PTZ_MOVETO_ORIGIN,
        PTZ_AUTO_PAN_ON,
        PTZ_AUTO_PAN_OFF,
        PTZ_SCAN_ON,
        PTZ_SCAN_OFF,
        PTZ_PATTERN_ON,
        PTZ_PATTERN_OFF,
        PTZ_TOUR_ON,
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
    };

    G2GUID  _camera;
    int     _command;   // PTZ_MOVE_N, PTZ_MOVE_NE,...
    int     _argument;  // 0 : step, 1 : continuing & require stop command
    int     _stream_id;
    int     _reserved;
};

struct G2LIVE_PTZ_MENU {
    enum FUNCTION {
        FUNCTION_SPEED       = 0x00000001,
        FUNCTION_AUTOPAN     = 0x00000002,
        FUNCTION_TOUR        = 0x00000004,
        FUNCTION_PATTERN     = 0x00000008,
        FUNCTION_SCAN        = 0x00000010,
        FUNCTION_OSDMENU     = 0x00000020,
        FUNCTION_LIGHT       = 0x00000040,
        FUNCTION_PUMP        = 0x00000080,
        FUNCTION_WIPER       = 0x00000100,
        FUNCTION_POWER       = 0x00000200,
        FUNCTION_AUX         = 0x00000400,
        FUNCTION_ORIGIN      = 0x00000800,
        FUNCTION_OUTMONITOR  = 0x00001000,
        FUNCTION_CTRL        = 0x00002000,
        FUNCTION_ENTER       = 0x00004000,
        FUNCTION_ESC         = 0x00008000,
        FUNCTION_REC_PATTERN = 0x00010000,
        FUNCTION_OSDMENU_OFF_NONE = 0x00020000,
    };

    G2GUID _guid;
    unsigned long _function;
    unsigned long _function_ex;
    bool _expanded;             // true : may use _function_ex
    bool _active_CTRL;          // true : CTRL is active
    bool _recording_pattern;    // true : pattern is recording
};

struct G2LIVE_PTZ_PRESET {
    enum VERSION {
        VERSION_IDR = -1,
        VERSION_G1 = 0,
        VERSION_G2 = 1,
    };
    enum {
        MAX_NUM_PRESET_G2 = 256,
        MAX_NUM_PRESET_G1 = 16,
        MAX_NUM_PRESET_IDR = 128,
        MAX_NUM_PRESET = 128,
        MAX_LEN_PRESET_G2 = 32,
        MAX_LEN_PRESET = 16
    };

    struct G1 {
        unsigned int _count;
        char _preset[MAX_NUM_PRESET][MAX_LEN_PRESET + 1];
    };
    struct G2 {
        unsigned int _count;
        G2STRING_32 _preset[MAX_NUM_PRESET_G2];
    };

    int     _version;
    G2GUID  _guid;
    int     _port;
    int     _model;
    int     _id;
    int     _number;
    int     _select;
    G1      _g1;
    G2      _g2;
};

struct G2LIVE_NETWORK_ALARM_INFO {
    unsigned int _seq_number;
    signed char _version[2];
    unsigned int _id;   // channel of G2EVENT
    int     _level;
    int     _event;     // user defined type
    bool    _on;
    G2STRING_128 _data;
    G2TIME  _time;
    __int64 _msec;
};

struct G2LIVE_NETWORK_ALARM_RESULT {
    enum TYPE {
        RESULT_OK = 0,
        FAIL_UNKNOWN,
        FAIL_NO_OPERATION,
        FAIL_DEVICE_INACTIVE
    };

    unsigned int _seq_number;
    int     _result;
    G2EVENT _event;
};

struct G2LIVE_NETWORK_ALARM_RESULT_PARAM {
    G2GUID _guid;
    G2LIVE_NETWORK_ALARM_RESULT _result;
};

struct G2LIVE_ELEVATOR_STATUS_INFO {
    unsigned int _seq_number;
    G2CODEC_INFO_SI_ELEVATOR_STATUS _status;
};

struct G2LIVE_COMMAND_RESULT {
    enum TYPE {
        UNKNOWN = 0,
        SUCCESS = 1,
        FAIL = 2
    };
};

struct G2LIVE_COMMAND_CONTROL_COLOR {
    enum CHANGE {
        CHANGE_TO_DEFAULT = 0,
        CHANGE_BRIGHTNESS, CHANGE_CONTRAST, CHANGE_HUE, CHANGE_SATURATION,
        CHANGE_ALL
    };

    unsigned short _brightness;
    unsigned short _contrast;
    unsigned short _hue;
    unsigned short _saturation;
    unsigned char  _change;
    short          _change_value;
};

struct G2LIVE_COMMAND_CONTROL_COLOR_RANGE {
    unsigned short _min_brightness;
    unsigned short _max_brightness;
    unsigned short _min_contrast;
    unsigned short _max_contrast;
    unsigned short _min_hue;
    unsigned short _max_hue;
    unsigned short _min_saturation;
    unsigned short _max_saturation;
};

struct G2LIVE_COMMAND_CONTROL_COLOR_STATUS {
    G2LIVE_COMMAND_CONTROL_COLOR _control;
    G2LIVE_COMMAND_CONTROL_COLOR_RANGE _range;
    int _camera;
};

struct G2LIVE_COMMAND_CONTROL_COLOR_RESULT {
    G2LIVE_COMMAND_CONTROL_COLOR _control;
    int _camera;
    int _result;
};

struct G2LIVE_COMMAND_CONTROL_PTZ {
    enum TYPE { PAN = 1, TILT = 2, ZOOM = 4, FOCUS = 8, IRIS = 16 };
    enum MOVE_TYPE {
        MT_ABSOLUTE = 0,
        MT_VELOCITY = 1,    // for P/T: 0 < val clockwise, val < 0 counterclocksize
                            // for Z  : 0 < val zoom-in, val < 0 zoom-out
                            // for P/T/Z: abs(val) is speed(velocity)
        MT_VELOCITY_BASED_ON_MIN_MAX = 2,
        MT_POINT_BASED_ON_IMAGE = 3,
    };

    unsigned int _types;
    int   _move_type;
    float _pan;
    float _tilt;
    float _zoom;
    float _focus;
    float _iris;
};

struct G2LIVE_COMMAND_CONTROL_PTZ_RANGE
{
    unsigned int _types;
    float _min_pan;
    float _max_pan;
    float _min_tilt;
    float _max_tilt;
    float _min_zoom;
    float _max_zoom;
    float _min_focus;
    float _max_focus;
    float _min_iris;
    float _max_iris;
};

struct G2LIVE_COMMAND_CONTROL_PTZ_STATUS
{
    G2LIVE_COMMAND_CONTROL_PTZ _control;
    G2LIVE_COMMAND_CONTROL_PTZ_RANGE _range;
    int _camera;
};

struct G2LIVE_COMMAND_CONTROL_PTZ_RESULT
{
    int _camera;
    int _result;
};

//////////////////////////////////////////////////////////////////////////

struct G2LIVE_PARAM_PTZ_PRESET
{
    G2LIVE_PTZ_PRESET _data;
    int _camera;
};

struct G2LIVE_PARAM_PTZ_MENU
{
    G2LIVE_PTZ_MENU _data;
    int _camera;
};

struct G2LIVE_PARAM_LIVE_AUDIO_OUT
{
    G2GUID _root;
    int _camera;
};

struct G2LIVE_PARAM_COMMAND_CONTROL_COLOR_STATUS {
    G2LIVE_COMMAND_CONTROL_COLOR_STATUS _status;
    G2GUID _camera;
};

struct G2LIVE_PARAM_COMMAND_CONTROL_COLOR_RESULT {
    G2LIVE_COMMAND_CONTROL_COLOR_RESULT _result;
    G2GUID _camera;
};

struct G2LIVE_PARAM_COMMAND_CONTROL_PTZ_STATUS {
    G2LIVE_COMMAND_CONTROL_PTZ_STATUS _status;
    G2GUID _camera;
};

struct G2LIVE_PARAM_COMMAND_CONTROL_PTZ_RESULT {
    G2LIVE_COMMAND_CONTROL_PTZ_RESULT _result;
    G2GUID _camera;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_LIVE_H_
