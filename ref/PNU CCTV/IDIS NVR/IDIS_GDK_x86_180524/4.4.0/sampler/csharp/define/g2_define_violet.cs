using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2VIOLET_CALLBACK
    {
        public enum TYPE
        {
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
        }
    }

    public struct G2VIOLET_KEY_ID
    {
        public enum ID
        {
            ID_NULL = 0,
            ID_SETUP,
            ID_QUIT,
            ID_PTZ_SET_PRESET,
            ID_PTZ_VIEW_PRESET,
            ID_PTZ_FOCUS_IN,
            ID_PTZ_FOCUS_IN_CONT,
            ID_PTZ_FOCUS_IN_STOP,
            ID_PTZ_FOCUS_OUT,
            ID_PTZ_FOCUS_OUT_CONT,
            ID_PTZ_FOCUS_OUT_STOP,
            ID_PTZ_IRIS_OPEN,
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
            ID_PTZ_ZOOM_OUT,
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
            ID_F1,
            ID_F2,
            ID_F3,
            ID_F4,
            ID_F5,
            ID_F6,
            ID_F7,
            ID_F8,
            ID_LOCK,
            ID_UNIT,
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
            ID_DVR_SPOT,
            ID_DVR_PLAY,
            ID_DVR_TRIPLEX,
            ID_DVR_PLAY_RW,
            ID_DVR_PLAY_PREV,
            ID_DVR_PLAY_NORMAL,
            ID_DVR_PLAY_FF,
            ID_DVR_PLAY_NEXT,
            ID_DVR_CLIP,
            ID_DVR_CLIP_2,
            ID_DVR_BOOK,
            ID_DVR_AUDIO,
            ID_DVR_MENU,
            ID_DVR_UP,
            ID_DVR_LEFT,
            ID_DVR_RIGHT,
            ID_DVR_DOWN,
            ID_DVR_ENTER,
            ID_DVR_SHUTTLE_CENTER,
            ID_DVR_SHUTTLE_LEFT1,
            ID_DVR_SHUTTLE_LEFT2,
            ID_DVR_SHUTTLE_LEFT3,
            ID_DVR_SHUTTLE_LEFT4,
            ID_DVR_SHUTTLE_LEFT5,
            ID_DVR_SHUTTLE_LEFT6,
            ID_DVR_SHUTTLE_LEFT7,
            ID_DVR_SHUTTLE_LEFT8,
            ID_DVR_SHUTTLE_RIGHT1,
            ID_DVR_SHUTTLE_RIGHT2,
            ID_DVR_SHUTTLE_RIGHT3,
            ID_DVR_SHUTTLE_RIGHT4,
            ID_DVR_SHUTTLE_RIGHT5,
            ID_DVR_SHUTTLE_RIGHT6,
            ID_DVR_SHUTTLE_RIGHT7,
            ID_DVR_SHUTTLE_RIGHT8,
            ID_DVR_MIC,
            ID_DVR_JOG_LEFT,
            ID_DVR_JOG_RIGHT,
            ID_DVR_MENU_OFF,
            ID_DVR_SPOT_OFF,

            ID_SPEAKER = ID_DVR_AUDIO,
            ID_MIC = ID_DVR_MIC,
            ID_PTZ_AUTO_SCAN_STOP
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2VIOLET_MOUSE_INPUT
    {
        public byte _button_l;
        public byte _button_r;
        public byte _button_m;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] _button_x;
        public byte _x;
        public byte _y;
        public byte _wheel;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2VIOLET_MONITOR_MESSAGE
    {
        public enum POSITION
        {
            LT = 0,
            CT,
            RT,
            RC,
            RB,
            CB,
            LB,
            LC,
            CC,
            POSITION_COUNT
        }

        public G2STRING_512 _message;
        public G2STRING_64 _font_face;
        public G2RECT _margin;
        public uint _duration;
        public uint _format;
        public uint _color;
        public uint _color_border;
        public int _font_height;
        public byte _alpha;
        [MarshalAs(UnmanagedType.U1)]
        public bool _bold;
        [MarshalAs(UnmanagedType.U1)]
        public bool _outline;

        void set_position(POSITION pos)
        {
            _format |= (uint)(
                (pos == POSITION.LT) ? (G2DT.FORMAT.LEFT | G2DT.FORMAT.TOP) :
                (pos == POSITION.CT) ? (G2DT.FORMAT.CENTER | G2DT.FORMAT.TOP) :
                (pos == POSITION.RT) ? (G2DT.FORMAT.RIGHT | G2DT.FORMAT.TOP) :
                (pos == POSITION.RC) ? (G2DT.FORMAT.RIGHT | G2DT.FORMAT.VCENTER) :
                (pos == POSITION.RB) ? (G2DT.FORMAT.RIGHT | G2DT.FORMAT.BOTTOM) :
                (pos == POSITION.CB) ? (G2DT.FORMAT.CENTER | G2DT.FORMAT.BOTTOM) :
                (pos == POSITION.LB) ? (G2DT.FORMAT.LEFT | G2DT.FORMAT.BOTTOM) :
                (pos == POSITION.LC) ? (G2DT.FORMAT.LEFT | G2DT.FORMAT.VCENTER) :
                (pos == POSITION.CC) ? (G2DT.FORMAT.CENTER | G2DT.FORMAT.VCENTER) : 0);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2VIOLET_PARAM_LAYOUT
    {
        public G2GUID _agent;
        public G2GUID _layout;
        public int _monitor;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2VIOLET_PARAM_CAMERASET
    {
        public G2GUID _agent;
        public G2PARAM_GUID_LIST _camera_GUIDs;
        public int _monitor;
    }
}
