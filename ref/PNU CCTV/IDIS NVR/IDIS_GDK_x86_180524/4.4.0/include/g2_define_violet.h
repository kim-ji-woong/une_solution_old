// g2_define_violet.h : header file
//

#ifndef _G2_DEFINE_VIOLET_H_
#define _G2_DEFINE_VIOLET_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2VIOLET_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_receive_service_log_load = 2,
        on_receive_service_log_load_end = 3,
        on_receive_service_log_load_fail = 4,
        on_receive_service_log_load_stop = 5,
        on_receive_agent_audio_connected = 6,
        on_receive_agent_audio_disconnected = 7,
        on_receive_agent_layout_attached = 8,
        on_receive_cameras_on_monitor = 9,

        CALLBACK_COUNT = 10
    };
};

struct G2VIOLET_KEY_ID {
    enum ID {
        ID_NULL = 0,
        ID_SETUP,            // 1
        ID_QUIT,
        ID_PTZ_SET_PRESET,
        ID_PTZ_VIEW_PRESET,
        ID_PTZ_FOCUS_IN,
        ID_PTZ_FOCUS_IN_CONT,
        ID_PTZ_FOCUS_IN_STOP,
        ID_PTZ_FOCUS_OUT,
        ID_PTZ_FOCUS_OUT_CONT,
        ID_PTZ_FOCUS_OUT_STOP,
        ID_PTZ_IRIS_OPEN,    // 11
        ID_PTZ_IRIS_OPEN_CONT,
        ID_PTZ_IRIS_OPEN_STOP,
        ID_PTZ_IRIS_CLOSE,
        ID_PTZ_IRIS_CLOSE_CONT,
        ID_PTZ_IRIS_CLOSE_STOP,
        ID_PTZ_AUTO_PAN,
        ID_PTZ_AUTO_PAN_STOP,
        ID_PTZ_TOUR,
        ID_PTZ_TOUR_STOP,
        ID_PTZ_PATTERN,
        ID_PTZ_PATTERN_STOP,
        ID_PTZ_HOME,
        ID_PTZ_MENU,
        ID_PTZ_MENU_OFF,
        ID_PTZ_LIGHT,
        ID_PTZ_LIGHT_OFF,
        ID_PTZ_AUX,
        ID_PTZ_AUX_OFF,
        ID_PTZ_ALARM,
        ID_PTZ_ALARM_ALL,
        ID_PTZ_PAN_LEFT,
        ID_PTZ_PAN_RIGHT,
        ID_PTZ_TILT_UP,
        ID_PTZ_TILT_DOWN,
        ID_PTZ_PAN_LEFT_UP,
        ID_PTZ_PAN_LEFT_DOWN,
        ID_PTZ_PAN_RIGHT_UP,
        ID_PTZ_PAN_RIGHT_DOWN,
        ID_PTZ_ZOOM_IN,
        ID_PTZ_ZOOM_OUT,         // 41
        ID_PTZ_CTRL_ON,
        ID_PTZ_CTRL_OFF,

        ID_0 = 51,
        ID_1,
        ID_2,
        ID_3,
        ID_4,
        ID_5,
        ID_6,
        ID_7,
        ID_8,
        ID_9,
        ID_ESC,
        ID_PLUS,
        ID_MINUS,
        ID_MON,
        ID_DVR,
        ID_DEV,
        ID_CAM,
        ID_ENTER,
        ID_SHIFT,
        ID_MACRO,
        ID_F1,                   // 71
        ID_F2,
        ID_F3,
        ID_F4,
        ID_F5,
        ID_F6,
        ID_F7,
        ID_F8,
        ID_LOCK,
        ID_UNIT,                 // 80
        ID_PANE,
        ID_RESET,
        ID_CAM2,

        ID_DVR_PANIC = 101,
        ID_DVR_PANIC_OFF,
        ID_DVR_ALARM,
        ID_DVR_ALARM_OFF,
        ID_DVR_DISP,
        ID_DVR_DISP_ASSIGN,
        ID_DVR_GROUP,
        ID_DVR_GROUP_SEQ,
        ID_DVR_FREEZE,
        ID_DVR_ZOOM,
        ID_DVR_SPOT,             // 111
        ID_DVR_PLAY,
        ID_DVR_TRIPLEX,
        ID_DVR_PLAY_RW,
        ID_DVR_PLAY_PREV,
        ID_DVR_PLAY_NORMAL,
        ID_DVR_PLAY_FF,
        ID_DVR_PLAY_NEXT,
        ID_DVR_CLIP,
        ID_DVR_CLIP_2,
        ID_DVR_BOOK,             // 121
        ID_DVR_AUDIO,
        ID_DVR_MENU,
        ID_DVR_UP,
        ID_DVR_LEFT,
        ID_DVR_RIGHT,
        ID_DVR_DOWN,
        ID_DVR_ENTER,
        ID_DVR_SHUTTLE_CENTER,
        ID_DVR_SHUTTLE_LEFT1,
        ID_DVR_SHUTTLE_LEFT2,    // 131
        ID_DVR_SHUTTLE_LEFT3,
        ID_DVR_SHUTTLE_LEFT4,
        ID_DVR_SHUTTLE_LEFT5,
        ID_DVR_SHUTTLE_LEFT6,
        ID_DVR_SHUTTLE_LEFT7,
        ID_DVR_SHUTTLE_LEFT8,
        ID_DVR_SHUTTLE_RIGHT1,
        ID_DVR_SHUTTLE_RIGHT2,
        ID_DVR_SHUTTLE_RIGHT3,
        ID_DVR_SHUTTLE_RIGHT4,   // 141
        ID_DVR_SHUTTLE_RIGHT5,
        ID_DVR_SHUTTLE_RIGHT6,
        ID_DVR_SHUTTLE_RIGHT7,
        ID_DVR_SHUTTLE_RIGHT8,
        ID_DVR_MIC,              // ignore
        ID_DVR_JOG_LEFT,
        ID_DVR_JOG_RIGHT,
        ID_DVR_MENU_OFF,         // 149
        ID_DVR_SPOT_OFF,

        ID_SPEAKER = ID_DVR_AUDIO,
        ID_MIC = ID_DVR_MIC,

        /////////////////////////////////////

        ID_PTZ_AUTO_SCAN_STOP
    };
};

struct G2VIOLET_MOUSE_INPUT {
    signed char _button_l;
    signed char _button_r;
    signed char _button_m;
    signed char _button_x[5];   // reserved

    signed char _x;
    signed char _y;
    signed char _wheel; // down : +1, up : -1
};

struct G2VIOLET_MONITOR_MESSAGE {
    enum POSITION {
        LT = 0, // left,   top
        CT,     // center, top
        RT,     // right,  top
        RC,     // right,  center
        RB,     // right,  bottom
        CB,     // center, bottom
        LB,     // left,   bottom
        LC,     // left,   center,
        CC,     // center, center
        POSITION_COUNT
    };

    G2STRING_512  _message;
    G2STRING_64   _font_face;
    G2RECT        _margin;
    unsigned int  _duration;
    unsigned int  _format;
    unsigned int  _color;   // BGR order
    unsigned int  _color_border;
    int           _font_height;
    unsigned char _alpha;
    bool          _bold;
    bool          _outline;
};

struct G2VIOLET_PARAM_LAYOUT {
    G2GUID _agent;
    G2GUID _layout;
    int    _monitor;
};

struct G2VIOLET_PARAM_CAMERASET {
    G2GUID _agent;
    G2GUID_LIST _camera_GUIDs;
    int    _monitor;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_VIOLET_H_
