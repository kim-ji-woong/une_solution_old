using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    using LayoutType = KeyValuePair<G2LAYOUT.FORMAT, string>;
    using LayoutList = List<KeyValuePair<G2LAYOUT.FORMAT, string>>;

    public partial class form_violet : Form
    {
        const int MAX_SCREEN_CAMERA_COUNT = 64;
        const int PTZ_SPEED_DEFAULT = 10;
        const int PTZ_SPEED_STOP = 0;

        public enum IDC
        {
            IDC_NULL = 0,
            IDC_BTN_VIOLET_KEY_PTZ_TILT_UP,
            IDC_BTN_VIOLET_KEY_PTZ_TILT_DOWN,
            IDC_BTN_VIOLET_KEY_PTZ_PAN_LEFT,
            IDC_BTN_VIOLET_KEY_PTZ_PAN_LEFT_UP,
            IDC_BTN_VIOLET_KEY_PTZ_PAN_LEFT_DOWN,
            IDC_BTN_VIOLET_KEY_PTZ_PAN_RIGHT,
            IDC_BTN_VIOLET_KEY_PTZ_PAN_RIGHT_UP,
            IDC_BTN_VIOLET_KEY_PTZ_PAN_RIGHT_DOWN,
            IDC_BTN_VIOLET_KEY_PTZ_ZOOM_IN,
            IDC_BTN_VIOLET_KEY_PTZ_ZOOM_OUT,
            IDC_BTN_VIOLET_KEY_PTZ_SET_PRESET,
            IDC_BTN_VIOLET_KEY_PTZ_VIEW_PRESET,
            IDC_BTN_VIOLET_KEY_PTZ_FOCUS_IN,
            IDC_BTN_VIOLET_KEY_PTZ_FOCUS_OUT,
            IDC_BTN_VIOLET_KEY_PTZ_IRIS_OPEN,
            IDC_BTN_VIOLET_KEY_PTZ_IRIS_CLOSE,
            IDC_BTN_VIOLET_KEY_PTZ_AUTOPAN,
            IDC_BTN_VIOLET_KEY_PTZ_TOUR,
            IDC_BTN_VIOLET_KEY_PTZ_PATTERN,
            IDC_BTN_VIOLET_KEY_PTZ_HOME,
            IDC_BTN_VIOLET_KEY_PTZ_MENU,
            IDC_BTN_VIOLET_KEY_PTZ_LIGHT,
            IDC_BTN_VIOLET_KEY_PTZ_AUX,
            IDC_BTN_VIOLET_KEY_PTZ_ALARM,
            IDC_BTN_VIOLET_KEY_F1,
            IDC_BTN_VIOLET_KEY_F2,
            IDC_BTN_VIOLET_KEY_F3,
            IDC_BTN_VIOLET_KEY_F4,
            IDC_BTN_VIOLET_KEY_0,
            IDC_BTN_VIOLET_KEY_1,
            IDC_BTN_VIOLET_KEY_2,
            IDC_BTN_VIOLET_KEY_3,
            IDC_BTN_VIOLET_KEY_4,
            IDC_BTN_VIOLET_KEY_5,
            IDC_BTN_VIOLET_KEY_6,
            IDC_BTN_VIOLET_KEY_7,
            IDC_BTN_VIOLET_KEY_8,
            IDC_BTN_VIOLET_KEY_9,
            IDC_BTN_VIOLET_KEY_ESC,
            IDC_BTN_VIOLET_KEY_PLUS,
            IDC_BTN_VIOLET_KEY_MINUS,
            IDC_BTN_VIOLET_KEY_DEV,
            IDC_BTN_VIOLET_KEY_MON,
            IDC_BTN_VIOLET_KEY_PANE,
            IDC_BTN_VIOLET_KEY_CAM,
            IDC_BTN_VIOLET_KEY_MACRO,
            IDC_BTN_VIOLET_KEY_ENTER,
            IDC_BTN_VIOLET_KEY_DVR_PANIC,
            IDC_BTN_VIOLET_KEY_DVR_ALARM,
            IDC_BTN_VIOLET_KEY_DVR_DISP,
            IDC_BTN_VIOLET_KEY_DVR_GROUP,
            IDC_BTN_VIOLET_KEY_DVR_FREEZE,
            IDC_BTN_VIOLET_KEY_FULLSCREEN,
            IDC_BTN_VIOLET_KEY_DVR_PLAY,
            IDC_BTN_VIOLET_KEY_DVR_PLAY_RW,
            IDC_BTN_VIOLET_KEY_DVR_PLAY_NORMAL,
            IDC_BTN_VIOLET_KEY_DVR_PLAY_FF,
            IDC_BTN_VIOLET_KEY_DVR_MENU,
            IDC_BTN_VIOLET_KEY_DVR_RESERVED1,
            IDC_BTN_VIOLET_KEY_DVR_RESERVED2,
            IDC_BTN_VIOLET_KEY_DVR_RESERVED3,
            IDC_BTN_VIOLET_KEY_DVR_UP,
            IDC_BTN_VIOLET_KEY_DVR_DOWN,
            IDC_BTN_VIOLET_KEY_DVR_LEFT,
            IDC_BTN_VIOLET_KEY_DVR_RIGHT,
        }

        public form_violet()
        {
            InitializeComponent();

            _channel = -1;
            _last_ptz_idc = IDC.IDC_NULL;

            _layout_info = new LayoutList();
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_1x1, "LAYOUT1x1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_2x2, "LAYOUT_2x2"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_3x3, "LAYOUT_3x3"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_4x4, "LAYOUT_4x4"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_5x5, "LAYOUT_5x5"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_6x6, "LAYOUT_6x6"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_7x7, "LAYOUT_7x7"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_8x8, "LAYOUT_8x8"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_5p1, "LAYOUT_5p1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_7p1, "LAYOUT_7p1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_9p1, "LAYOUT_9p1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_11p1, "LAYOUT_11p1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_12p1, "LAYOUT_12p1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_27p1, "LAYOUT_27p1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_8p2, "LAYOUT_8p2"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_18p2, "LAYOUT_18p2"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_4p3, "LAYOUT_4p3"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_9p3, "LAYOUT_9p3"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_3x2w, "LAYOUT_3x2w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_4x3w, "LAYOUT_4x3w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_5x4w, "LAYOUT_5x4w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_6x5w, "LAYOUT_6x5w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_2p1w, "LAYOUT_2p1w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_3p1w, "LAYOUT_3p1w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_4p1w, "LAYOUT_4p1w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_5p1w, "LAYOUT_5p1w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_6p1w, "LAYOUT_6p1w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_8p1w, "LAYOUT_8p1w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_10p1w, "LAYOUT_10p1w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_4p2w, "LAYOUT_4p2w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_12p2w, "LAYOUT_12p2w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_12h1, "LAYOUT_12h1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_32h1, "LAYOUT_32h1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_28p4, "LAYOUT_28p4"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_12p4w, "LAYOUT_12p4w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_16p12, "LAYOUT_16p12"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_24p12w, "LAYOUT_24p12w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_7x6w, "LAYOUT_7x6w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_8x7w, "LAYOUT_8x7w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_5x3w, "LAYOUT_5x3w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_6x3w, "LAYOUT_6x3w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_8x3w, "LAYOUT_8x3w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_8x5w, "LAYOUT_8x5w"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_2x1, "LAYOUT_2x1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_3x1, "LAYOUT_3x1"));
            _layout_info.Add(new LayoutType(G2LAYOUT.FORMAT.LAYOUT_4x1, "LAYOUT_4x1"));

            _violet = new g2violet();
            _violet.set_listener(this);
            _violet.startup(1);

            initialize_control();
            set_status_msg("Disconnected");
            enable_controls(false);
        }
        ~form_violet()
        {
            cleanup();
        }

        public void cleanup()
        {
            if (_last_ptz_idc > IDC.IDC_NULL)
            {
                on_post_violet_key_ptz(_last_ptz_idc, PTZ_SPEED_STOP);
            }

            _layout_info.Clear();
            _violet.cleanup();
        }

        protected void initialize_control()
        {
            #region set tag
            BTN_VIOLET_KEY_PTZ_TILT_UP.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_TILT_UP;
            BTN_VIOLET_KEY_PTZ_TILT_DOWN.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_TILT_DOWN;
            BTN_VIOLET_KEY_PTZ_PAN_LEFT.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_LEFT;
            BTN_VIOLET_KEY_PTZ_PAN_LEFT_UP.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_LEFT_UP;
            BTN_VIOLET_KEY_PTZ_PAN_LEFT_DOWN.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_LEFT_DOWN;
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_RIGHT;
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT_UP.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_RIGHT_UP;
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT_DOWN.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_RIGHT_DOWN;
            BTN_VIOLET_KEY_PTZ_ZOOM_IN.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_ZOOM_IN;
            BTN_VIOLET_KEY_PTZ_ZOOM_OUT.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_ZOOM_OUT;
            BTN_VIOLET_KEY_PTZ_SET_PRESET.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_SET_PRESET;
            BTN_VIOLET_KEY_PTZ_VIEW_PRESET.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_VIEW_PRESET;
            BTN_VIOLET_KEY_PTZ_FOCUS_IN.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_FOCUS_IN;
            BTN_VIOLET_KEY_PTZ_FOCUS_OUT.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_FOCUS_OUT;
            BTN_VIOLET_KEY_PTZ_IRIS_OPEN.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_IRIS_OPEN;
            BTN_VIOLET_KEY_PTZ_IRIS_CLOSE.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_IRIS_CLOSE;
            BTN_VIOLET_KEY_PTZ_AUTOPAN.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_AUTOPAN;
            BTN_VIOLET_KEY_PTZ_TOUR.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_TOUR;
            BTN_VIOLET_KEY_PTZ_PATTERN.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_PATTERN;
            BTN_VIOLET_KEY_PTZ_HOME.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_HOME;
            BTN_VIOLET_KEY_PTZ_MENU.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_MENU;
            BTN_VIOLET_KEY_PTZ_LIGHT.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_LIGHT;
            BTN_VIOLET_KEY_PTZ_AUX.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_AUX;
            BTN_VIOLET_KEY_PTZ_ALARM.Tag = IDC.IDC_BTN_VIOLET_KEY_PTZ_ALARM;
            BTN_VIOLET_KEY_F1.Tag = IDC.IDC_BTN_VIOLET_KEY_F1;
            BTN_VIOLET_KEY_F2.Tag = IDC.IDC_BTN_VIOLET_KEY_F2;
            BTN_VIOLET_KEY_F3.Tag = IDC.IDC_BTN_VIOLET_KEY_F3;
            BTN_VIOLET_KEY_F4.Tag = IDC.IDC_BTN_VIOLET_KEY_F4;
            BTN_VIOLET_KEY_0.Tag = IDC.IDC_BTN_VIOLET_KEY_0;
            BTN_VIOLET_KEY_1.Tag = IDC.IDC_BTN_VIOLET_KEY_1;
            BTN_VIOLET_KEY_2.Tag = IDC.IDC_BTN_VIOLET_KEY_2;
            BTN_VIOLET_KEY_3.Tag = IDC.IDC_BTN_VIOLET_KEY_3;
            BTN_VIOLET_KEY_4.Tag = IDC.IDC_BTN_VIOLET_KEY_4;
            BTN_VIOLET_KEY_5.Tag = IDC.IDC_BTN_VIOLET_KEY_5;
            BTN_VIOLET_KEY_6.Tag = IDC.IDC_BTN_VIOLET_KEY_6;
            BTN_VIOLET_KEY_7.Tag = IDC.IDC_BTN_VIOLET_KEY_7;
            BTN_VIOLET_KEY_8.Tag = IDC.IDC_BTN_VIOLET_KEY_8;
            BTN_VIOLET_KEY_9.Tag = IDC.IDC_BTN_VIOLET_KEY_9;
            BTN_VIOLET_KEY_ESC.Tag = IDC.IDC_BTN_VIOLET_KEY_ESC;
            BTN_VIOLET_KEY_PLUS.Tag = IDC.IDC_BTN_VIOLET_KEY_PLUS;
            BTN_VIOLET_KEY_MINUS.Tag = IDC.IDC_BTN_VIOLET_KEY_MINUS;
            BTN_VIOLET_KEY_DEV.Tag = IDC.IDC_BTN_VIOLET_KEY_DEV;
            BTN_VIOLET_KEY_MON.Tag = IDC.IDC_BTN_VIOLET_KEY_MON;
            BTN_VIOLET_KEY_PANE.Tag = IDC.IDC_BTN_VIOLET_KEY_PANE;
            BTN_VIOLET_KEY_CAM.Tag = IDC.IDC_BTN_VIOLET_KEY_CAM;
            BTN_VIOLET_KEY_ENTER.Tag = IDC.IDC_BTN_VIOLET_KEY_ENTER;
            BTN_VIOLET_KEY_DVR_PANIC.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_PANIC;
            BTN_VIOLET_KEY_DVR_ALARM.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_ALARM;
            BTN_VIOLET_KEY_DVR_DISP.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_DISP;
            BTN_VIOLET_KEY_DVR_GROUP.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_GROUP;
            BTN_VIOLET_KEY_DVR_FREEZE.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_FREEZE;
            BTN_VIOLET_KEY_FULLSCREEN.Tag = IDC.IDC_BTN_VIOLET_KEY_FULLSCREEN;
            BTN_VIOLET_KEY_DVR_PLAY.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_PLAY;
            BTN_VIOLET_KEY_DVR_PLAY_RW.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_PLAY_RW;
            BTN_VIOLET_KEY_DVR_PLAY_NORMAL.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_PLAY_NORMAL;
            BTN_VIOLET_KEY_DVR_PLAY_FF.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_PLAY_FF;
            BTN_VIOLET_KEY_DVR_MENU.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_MENU;
            BTN_VIOLET_KEY_DVR_UP.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_UP;
            BTN_VIOLET_KEY_DVR_DOWN.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_DOWN;
            BTN_VIOLET_KEY_DVR_LEFT.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_LEFT;
            BTN_VIOLET_KEY_DVR_RIGHT.Tag = IDC.IDC_BTN_VIOLET_KEY_DVR_RIGHT;
            #endregion

            #region set event

            BTN_VIOLET_KEY_PTZ_TILT_UP.MouseDown += on_btn_mouse_down;
            BTN_VIOLET_KEY_PTZ_TILT_UP.MouseUp += on_btn_mouse_up;
            BTN_VIOLET_KEY_PTZ_TILT_DOWN.MouseDown += on_btn_mouse_down;    
            BTN_VIOLET_KEY_PTZ_TILT_DOWN.MouseUp += on_btn_mouse_up;    
            BTN_VIOLET_KEY_PTZ_PAN_LEFT.MouseDown += on_btn_mouse_down;     
            BTN_VIOLET_KEY_PTZ_PAN_LEFT.MouseUp += on_btn_mouse_up;     
            BTN_VIOLET_KEY_PTZ_PAN_LEFT_UP.MouseDown += on_btn_mouse_down;  
            BTN_VIOLET_KEY_PTZ_PAN_LEFT_UP.MouseUp += on_btn_mouse_up;  
            BTN_VIOLET_KEY_PTZ_PAN_LEFT_DOWN.MouseDown += on_btn_mouse_down;
            BTN_VIOLET_KEY_PTZ_PAN_LEFT_DOWN.MouseUp += on_btn_mouse_up;
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT.MouseDown += on_btn_mouse_down;    
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT.MouseUp += on_btn_mouse_up;    
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT_UP.MouseDown += on_btn_mouse_down; 
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT_UP.MouseUp += on_btn_mouse_up;
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT_DOWN.MouseDown += on_btn_mouse_down;
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT_DOWN.MouseUp += on_btn_mouse_up;
            BTN_VIOLET_KEY_PTZ_ZOOM_IN.MouseDown += on_btn_mouse_down;
            BTN_VIOLET_KEY_PTZ_ZOOM_IN.MouseUp += on_btn_mouse_up;
            BTN_VIOLET_KEY_PTZ_ZOOM_OUT.MouseDown += on_btn_mouse_down;
            BTN_VIOLET_KEY_PTZ_ZOOM_OUT.MouseUp += on_btn_mouse_up;    
            BTN_VIOLET_CONNECT.Click += this.on_btn_violet_connect;
            BTN_VIOLET_SET_LAYOUT_MONITOR_LIVE.Click += this.on_btn_violet_set_layout_monitor_live;
            BTN_VIOLET_AUDIO_CONNECT.Click += this.on_btn_violet_audio_connect;
            BTN_VIOLET_AUDIO_DISCONNECT.Click += this.on_btn_violet_audio_disconnect;
            BTN_VIOLET_LAYOUT_CHANGE.Click += this.on_btn_violet_layout_change;
            BTN_VIOLET_KEY_PTZ_SET_PRESET.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_VIEW_PRESET.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_FOCUS_IN.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_FOCUS_OUT.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_IRIS_OPEN.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_IRIS_CLOSE.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_AUTOPAN.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_TOUR.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_PATTERN.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_HOME.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_MENU.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_LIGHT.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_AUX.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PTZ_ALARM.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_F1.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_F2.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_F3.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_F4.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_0.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_1.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_2.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_3.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_4.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_5.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_6.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_7.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_8.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_9.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_ESC.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PLUS.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_MINUS.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DEV.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_MON.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_PANE.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_CAM.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_ENTER.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_PANIC.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_ALARM.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_DISP.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_GROUP.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_FREEZE.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_FULLSCREEN.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_PLAY.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_PLAY_RW.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_PLAY_NORMAL.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_PLAY_FF.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_MENU.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_UP.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_LEFT.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_RIGHT.Click += this.on_btn_violet_key;
            BTN_VIOLET_KEY_DVR_DOWN.Click += this.on_btn_violet_key;
            RDO_VIOLET_MONITOR_MESSAGE_SHOW.Click += this.on_btn_violet_message_show;
            RDO_VIOLET_MONITOR_MESSAGE_HIDE.Click += this.on_btn_violet_message_hide;
            BTN_VIOLET_MESSAGE_SEND.Click += this.on_btn_violet_message_send;
            CBX_VIOLET_MESSAGE_POSITION.SelectedIndexChanged += this.on_cbx_selchange_message_position;
            BTN_VIOLET_MESSAGE_SCREEN_SEND.Click += this.on_btn_violet_screen_send;
            #endregion

            CBX_VIOLET_MESSAGE_TYPE.Items.Add("Screen Message");
            CBX_VIOLET_MESSAGE_TYPE.Items.Add("Monitor Message");
            CBX_VIOLET_MESSAGE_TYPE.SelectedIndex = 0;

            EDT_VIOLET_LAYOUT_MONITOR.Text = "1";
            EDT_VIOLET_MONITOR_NUMBER.Text = "1";
            EDT_VIOLET_MESSAGE_TIMEOUT.Text = "5000";
            EDT_VIOLET_MESSAGE_FONT_SIZE.Text = "50";

            CBX_VIOLET_MESSAGE_POSITION.Items.Add("Left & Top");
            CBX_VIOLET_MESSAGE_POSITION.Items.Add("Center & Top");
            CBX_VIOLET_MESSAGE_POSITION.Items.Add("Right & Top");
            CBX_VIOLET_MESSAGE_POSITION.Items.Add("Right & Center");
            CBX_VIOLET_MESSAGE_POSITION.Items.Add("Right & Bottom");
            CBX_VIOLET_MESSAGE_POSITION.Items.Add("Center & Bottom");
            CBX_VIOLET_MESSAGE_POSITION.Items.Add("Left & Bottom");
            CBX_VIOLET_MESSAGE_POSITION.Items.Add("Left & Center");
            CBX_VIOLET_MESSAGE_POSITION.Items.Add("Center");
            CBX_VIOLET_MESSAGE_POSITION.SelectedIndex = 0;

            CBX_VIOLET_LAYOUT_CAMERA.Items.Add("None");
            for (int i = 1; i < MAX_SCREEN_CAMERA_COUNT; ++i)
            {
                CBX_VIOLET_LAYOUT_CAMERA.Items.Add(string.Format("{0}", i));
            }
            CBX_VIOLET_LAYOUT_CAMERA.SelectedIndex = 0;

            foreach (LayoutType t in _layout_info)
            {
                CBX_VIOLET_LAYOUT_FORMAT.Items.Add(t.Value);
            }
            CBX_VIOLET_LAYOUT_FORMAT.SelectedIndex = 0;

            EDT_VIOLET_MONITOR_NUMBER_AUDIO.Text = "1";
            EDT_VIOLET_DEVICE_ADDRESS.Text = "127.0.0.1";
            EDT_VIOLET_DEVICE_PORT.Text = "8116";
            BTN_VIOLET_AUDIO_DISCONNECT.Enabled = false;

            RDO_VIOLET_MONITOR_MESSAGE_SHOW.Checked = true;
            RDO_VIOLET_MONITOR_MESSAGE_HIDE.Checked = false;

            EDT_VIOLET_MONITOR_NUMBER_SCREEN.Text = "1";
        }

        public bool connect()
        {
            if (!valid_channel(_channel))
            {
                G2GUID guid = new G2GUID();
                G2GUIDLIST services = new G2GUIDLIST();
                g2admin.get().get_service_guid(services);
                bool failover = CHK_USE_VIOLET_FAILOVER_CONNECT.Checked;

                foreach (G2GUID g in services)
                {
                    G2GUID temp = g;
                    if (g.is_service_videowall != true) continue;
                    if (g.is_service_videowall_failover != failover) continue;
                    if (g2admin.get().is_service_registered(ref temp) != true) continue;
                    guid = temp;
                    break;
                }

                if (guid.is_service_videowall)
                {
                    G2CONNECT_RES res;
                    _channel = _violet.connect(ref guid, out res);
                    set_status_msg("Connecting...");
                    BTN_VIOLET_CONNECT.Enabled = false;
                }
            }
            return valid_channel(_channel);
        }

        public void disconnect()
        {
            int channel = _channel;
            if (!valid_channel(channel)) { return; }
            if (_violet.is_disconnectable(channel))
            {
                _violet.disconnect(channel);
                set_status_msg("Disconnecting...");
                BTN_VIOLET_CONNECT.Enabled = false;

                while (_violet.is_disconnecting(channel))
                {
                    g2looper.sleep(5);
                }
            }
        }

        public void enable_controls(bool enable)
        {
            BTN_VIOLET_KEY_PTZ_TILT_UP.Enabled = enable;  
            BTN_VIOLET_KEY_PTZ_TILT_DOWN.Enabled = enable;
            BTN_VIOLET_KEY_PTZ_PAN_LEFT.Enabled = enable; 
            BTN_VIOLET_KEY_PTZ_PAN_LEFT_UP.Enabled = enable; 
            BTN_VIOLET_KEY_PTZ_PAN_LEFT_DOWN.Enabled = enable;
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT.Enabled = enable; 
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT_UP.Enabled = enable; 
            BTN_VIOLET_KEY_PTZ_PAN_RIGHT_DOWN.Enabled = enable; 
            BTN_VIOLET_KEY_PTZ_ZOOM_IN.Enabled = enable;  
            BTN_VIOLET_KEY_PTZ_ZOOM_OUT.Enabled = enable; 
            BTN_VIOLET_KEY_PTZ_SET_PRESET.Enabled = enable;    
            BTN_VIOLET_KEY_PTZ_VIEW_PRESET.Enabled = enable;   
            BTN_VIOLET_KEY_PTZ_FOCUS_IN.Enabled = enable;      
            BTN_VIOLET_KEY_PTZ_FOCUS_OUT.Enabled = enable;     
            BTN_VIOLET_KEY_PTZ_IRIS_OPEN.Enabled = enable;     
            BTN_VIOLET_KEY_PTZ_IRIS_CLOSE.Enabled = enable;    
            BTN_VIOLET_KEY_PTZ_AUTOPAN.Enabled = enable;   
            BTN_VIOLET_KEY_PTZ_TOUR.Enabled = enable;      
            BTN_VIOLET_KEY_PTZ_PATTERN.Enabled = enable;   
            BTN_VIOLET_KEY_PTZ_HOME.Enabled = enable;      
            BTN_VIOLET_KEY_PTZ_MENU.Enabled = enable;      
            BTN_VIOLET_KEY_PTZ_LIGHT.Enabled = enable;
            BTN_VIOLET_KEY_PTZ_AUX.Enabled = enable;
            BTN_VIOLET_KEY_PTZ_ALARM.Enabled = enable;
            BTN_VIOLET_KEY_F1.Enabled = enable;
            BTN_VIOLET_KEY_F2.Enabled = enable;
            BTN_VIOLET_KEY_F3.Enabled = enable;
            BTN_VIOLET_KEY_F4.Enabled = enable;
            BTN_VIOLET_KEY_0.Enabled = enable;
            BTN_VIOLET_KEY_1.Enabled = enable;
            BTN_VIOLET_KEY_2.Enabled = enable;
            BTN_VIOLET_KEY_3.Enabled = enable;
            BTN_VIOLET_KEY_4.Enabled = enable;
            BTN_VIOLET_KEY_5.Enabled = enable;
            BTN_VIOLET_KEY_6.Enabled = enable;
            BTN_VIOLET_KEY_7.Enabled = enable;
            BTN_VIOLET_KEY_8.Enabled = enable;
            BTN_VIOLET_KEY_9.Enabled = enable;
            BTN_VIOLET_KEY_ESC.Enabled = enable;
            BTN_VIOLET_KEY_PLUS.Enabled = enable;
            BTN_VIOLET_KEY_MINUS.Enabled = enable;
            BTN_VIOLET_KEY_DEV.Enabled = enable;
            BTN_VIOLET_KEY_MON.Enabled = enable;
            BTN_VIOLET_KEY_PANE.Enabled = enable;
            BTN_VIOLET_KEY_CAM.Enabled = enable;
            CHK_VIOLET_KEY_SHIFT.Enabled = enable;
            BTN_VIOLET_KEY_ENTER.Enabled = enable;
            BTN_VIOLET_KEY_DVR_PANIC.Enabled = enable;
            BTN_VIOLET_KEY_DVR_ALARM.Enabled = enable;
            BTN_VIOLET_KEY_DVR_DISP.Enabled = enable;
            BTN_VIOLET_KEY_DVR_GROUP.Enabled = enable;
            BTN_VIOLET_KEY_DVR_FREEZE.Enabled = enable;
            BTN_VIOLET_KEY_FULLSCREEN.Enabled = enable;
            BTN_VIOLET_KEY_DVR_PLAY.Enabled = enable;
            BTN_VIOLET_KEY_DVR_PLAY_RW.Enabled = enable;
            BTN_VIOLET_KEY_DVR_PLAY_NORMAL.Enabled = enable;
            BTN_VIOLET_KEY_DVR_PLAY_FF.Enabled = enable;
            BTN_VIOLET_KEY_DVR_MENU.Enabled = enable;
            BTN_VIOLET_KEY_DVR_UP.Enabled = enable;
            BTN_VIOLET_KEY_DVR_LEFT.Enabled = enable;
            BTN_VIOLET_KEY_DVR_RIGHT.Enabled = enable;
            BTN_VIOLET_KEY_DVR_DOWN.Enabled = enable;
            BTN_VIOLET_SET_LAYOUT_MONITOR_LIVE.Enabled = enable;
            EDT_VIOLET_LAYOUT_MONITOR.Enabled = enable;
            CBX_VIOLET_LAYOUT_CAMERA.Enabled = enable;
            CBX_VIOLET_LAYOUT_FORMAT.Enabled = enable;
            BTN_VIOLET_LAYOUT_CHANGE.Enabled = enable;
        }

        public void set_status_msg(string status)
        {
            TB_STATUS.Text = status;
        }

        protected bool is_shift_clicked()
        {
            return CHK_VIOLET_KEY_SHIFT.Checked;
        }

        protected G2VIOLET_KEY_ID.ID get_key_id(IDC idc)
        {
            bool shifted = is_shift_clicked();
            switch (idc) {
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_TILT_UP:        return G2VIOLET_KEY_ID.ID.ID_PTZ_TILT_UP;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_TILT_DOWN:      return G2VIOLET_KEY_ID.ID.ID_PTZ_TILT_DOWN;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_LEFT:       return G2VIOLET_KEY_ID.ID.ID_PTZ_PAN_LEFT;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_LEFT_UP:    return G2VIOLET_KEY_ID.ID.ID_PTZ_PAN_LEFT_UP;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_LEFT_DOWN:  return G2VIOLET_KEY_ID.ID.ID_PTZ_PAN_LEFT_DOWN;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_RIGHT:      return G2VIOLET_KEY_ID.ID.ID_PTZ_PAN_RIGHT;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_RIGHT_UP:   return G2VIOLET_KEY_ID.ID.ID_PTZ_PAN_RIGHT_UP;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_PAN_RIGHT_DOWN: return G2VIOLET_KEY_ID.ID.ID_PTZ_PAN_RIGHT_DOWN;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_ZOOM_IN:        return G2VIOLET_KEY_ID.ID.ID_PTZ_ZOOM_IN;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_ZOOM_OUT:       return G2VIOLET_KEY_ID.ID.ID_PTZ_ZOOM_OUT;

                case IDC.IDC_BTN_VIOLET_KEY_PTZ_SET_PRESET:     return G2VIOLET_KEY_ID.ID.ID_PTZ_SET_PRESET;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_VIEW_PRESET:    return G2VIOLET_KEY_ID.ID.ID_PTZ_VIEW_PRESET;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_FOCUS_IN:       return G2VIOLET_KEY_ID.ID.ID_PTZ_FOCUS_IN;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_FOCUS_OUT:      return G2VIOLET_KEY_ID.ID.ID_PTZ_FOCUS_OUT;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_IRIS_OPEN:      return G2VIOLET_KEY_ID.ID.ID_PTZ_IRIS_OPEN;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_IRIS_CLOSE:     return G2VIOLET_KEY_ID.ID.ID_PTZ_IRIS_CLOSE;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_AUTOPAN:        return shifted ? G2VIOLET_KEY_ID.ID.ID_PTZ_AUTO_PAN_STOP : G2VIOLET_KEY_ID.ID.ID_PTZ_AUTO_PAN;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_TOUR:           return shifted ? G2VIOLET_KEY_ID.ID.ID_PTZ_TOUR_STOP     : G2VIOLET_KEY_ID.ID.ID_PTZ_TOUR;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_PATTERN:        return shifted ? G2VIOLET_KEY_ID.ID.ID_PTZ_PATTERN_STOP  : G2VIOLET_KEY_ID.ID.ID_PTZ_PATTERN;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_HOME:           return G2VIOLET_KEY_ID.ID.ID_PTZ_HOME;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_MENU:           return shifted ? G2VIOLET_KEY_ID.ID.ID_PTZ_MENU_OFF      : G2VIOLET_KEY_ID.ID.ID_PTZ_MENU;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_LIGHT:          return shifted ? G2VIOLET_KEY_ID.ID.ID_PTZ_LIGHT_OFF     : G2VIOLET_KEY_ID.ID.ID_PTZ_LIGHT;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_AUX:            return shifted ? G2VIOLET_KEY_ID.ID.ID_PTZ_AUX_OFF       : G2VIOLET_KEY_ID.ID.ID_PTZ_AUX;
                case IDC.IDC_BTN_VIOLET_KEY_PTZ_ALARM:          return shifted ? G2VIOLET_KEY_ID.ID.ID_PTZ_ALARM_ALL     : G2VIOLET_KEY_ID.ID.ID_PTZ_ALARM;

                case IDC.IDC_BTN_VIOLET_KEY_F1:                 return shifted ? G2VIOLET_KEY_ID.ID.ID_F5 : G2VIOLET_KEY_ID.ID.ID_F1;
                case IDC.IDC_BTN_VIOLET_KEY_F2:                 return shifted ? G2VIOLET_KEY_ID.ID.ID_F6 : G2VIOLET_KEY_ID.ID.ID_F2;
                case IDC.IDC_BTN_VIOLET_KEY_F3:                 return shifted ? G2VIOLET_KEY_ID.ID.ID_F7 : G2VIOLET_KEY_ID.ID.ID_F3;
                case IDC.IDC_BTN_VIOLET_KEY_F4:                 return shifted ? G2VIOLET_KEY_ID.ID.ID_F8 : G2VIOLET_KEY_ID.ID.ID_F4;
                case IDC.IDC_BTN_VIOLET_KEY_0:                  return G2VIOLET_KEY_ID.ID.ID_0;
                case IDC.IDC_BTN_VIOLET_KEY_1:                  return G2VIOLET_KEY_ID.ID.ID_1;
                case IDC.IDC_BTN_VIOLET_KEY_2:                  return G2VIOLET_KEY_ID.ID.ID_2;
                case IDC.IDC_BTN_VIOLET_KEY_3:                  return G2VIOLET_KEY_ID.ID.ID_3;
                case IDC.IDC_BTN_VIOLET_KEY_4:                  return G2VIOLET_KEY_ID.ID.ID_4;
                case IDC.IDC_BTN_VIOLET_KEY_5:                  return G2VIOLET_KEY_ID.ID.ID_5;
                case IDC.IDC_BTN_VIOLET_KEY_6:                  return G2VIOLET_KEY_ID.ID.ID_6;
                case IDC.IDC_BTN_VIOLET_KEY_7:                  return G2VIOLET_KEY_ID.ID.ID_7;
                case IDC.IDC_BTN_VIOLET_KEY_8:                  return G2VIOLET_KEY_ID.ID.ID_8;
                case IDC.IDC_BTN_VIOLET_KEY_9:                  return G2VIOLET_KEY_ID.ID.ID_9;
                case IDC.IDC_BTN_VIOLET_KEY_ESC:                return G2VIOLET_KEY_ID.ID.ID_ESC;
                case IDC.IDC_BTN_VIOLET_KEY_PLUS:               return G2VIOLET_KEY_ID.ID.ID_PLUS;
                case IDC.IDC_BTN_VIOLET_KEY_MINUS:              return G2VIOLET_KEY_ID.ID.ID_MINUS;
                case IDC.IDC_BTN_VIOLET_KEY_DEV:                return G2VIOLET_KEY_ID.ID.ID_DEV;
                case IDC.IDC_BTN_VIOLET_KEY_MON:                return G2VIOLET_KEY_ID.ID.ID_MON;
                case IDC.IDC_BTN_VIOLET_KEY_PANE:               return G2VIOLET_KEY_ID.ID.ID_PANE;
                case IDC.IDC_BTN_VIOLET_KEY_CAM:                return G2VIOLET_KEY_ID.ID.ID_CAM;
                case IDC.IDC_BTN_VIOLET_KEY_MACRO:              return G2VIOLET_KEY_ID.ID.ID_MACRO;
                case IDC.IDC_BTN_VIOLET_KEY_ENTER:              return G2VIOLET_KEY_ID.ID.ID_DVR_ENTER;

                case IDC.IDC_BTN_VIOLET_KEY_DVR_PANIC:          return shifted ? G2VIOLET_KEY_ID.ID.ID_DVR_PANIC_OFF    : G2VIOLET_KEY_ID.ID.ID_DVR_PANIC;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_ALARM:          return shifted ? G2VIOLET_KEY_ID.ID.ID_DVR_ALARM_OFF    : G2VIOLET_KEY_ID.ID.ID_DVR_ALARM;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_DISP:           return shifted ? G2VIOLET_KEY_ID.ID.ID_DVR_DISP_ASSIGN  : G2VIOLET_KEY_ID.ID.ID_DVR_DISP;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_GROUP:          return shifted ? G2VIOLET_KEY_ID.ID.ID_DVR_GROUP_SEQ    : G2VIOLET_KEY_ID.ID.ID_DVR_GROUP;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_FREEZE:         return shifted ? G2VIOLET_KEY_ID.ID.ID_DVR_ZOOM         : G2VIOLET_KEY_ID.ID.ID_DVR_FREEZE;
                case IDC.IDC_BTN_VIOLET_KEY_FULLSCREEN:         return G2VIOLET_KEY_ID.ID.ID_DVR_SPOT;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_PLAY:           return shifted ? G2VIOLET_KEY_ID.ID.ID_DVR_TRIPLEX      : G2VIOLET_KEY_ID.ID.ID_DVR_PLAY;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_PLAY_RW:        return shifted ? G2VIOLET_KEY_ID.ID.ID_DVR_PLAY_PREV    : G2VIOLET_KEY_ID.ID.ID_DVR_PLAY_RW;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_PLAY_NORMAL:    return G2VIOLET_KEY_ID.ID.ID_DVR_PLAY_NORMAL;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_PLAY_FF:        return shifted ? G2VIOLET_KEY_ID.ID.ID_DVR_PLAY_NEXT    : G2VIOLET_KEY_ID.ID.ID_DVR_PLAY_FF;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_MENU:           return G2VIOLET_KEY_ID.ID.ID_DVR_MENU;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_RESERVED1:      return G2VIOLET_KEY_ID.ID.ID_NULL;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_RESERVED2:      return G2VIOLET_KEY_ID.ID.ID_NULL;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_RESERVED3:      return G2VIOLET_KEY_ID.ID.ID_NULL;

                case IDC.IDC_BTN_VIOLET_KEY_DVR_UP:             return G2VIOLET_KEY_ID.ID.ID_DVR_UP;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_DOWN:           return G2VIOLET_KEY_ID.ID.ID_DVR_DOWN;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_LEFT:           return G2VIOLET_KEY_ID.ID.ID_DVR_LEFT;
                case IDC.IDC_BTN_VIOLET_KEY_DVR_RIGHT:          return G2VIOLET_KEY_ID.ID.ID_DVR_RIGHT;

                default:
                    return G2VIOLET_KEY_ID.ID.ID_NULL;
            }
        }
        
        protected void on_btn_violet_connect(object sender, EventArgs e)
        {
            if (_channel == -1)
            {
                connect();
            }
            else
            {
                disconnect();
            }
        }

        protected void on_btn_violet_key(object sender, EventArgs e)
        {
            if (!valid_channel(_channel)) { return; };

            G2VIOLET_KEY_ID.ID id = G2VIOLET_KEY_ID.ID.ID_NULL;
            if (sender is Control)
            {
                Control c = sender as Control;
                if (c.Tag is IDC)
                {
                    IDC idc = (IDC)c.Tag;
                    id = get_key_id(idc);
                }
            }
            if (id != G2VIOLET_KEY_ID.ID.ID_NULL)
            {
                trace("[SEND] key = {0}\n", id);
                _violet.send_stream_channel_control_via_id(_channel, 0, (int)id);
            }
        }

        protected void on_btn_violet_set_layout_monitor_live(object sender, EventArgs e)
        {
            G2GUIDLIST guid_list = new G2GUIDLIST();
            List<G2LAYOUT> layout_list = new List<G2LAYOUT>();
            if (g2admin.get().get_layout_guid(guid_list))
            {
                foreach (G2GUID g in guid_list)
                {
                    G2GUID guid = g;
                    G2LAYOUT layout;
                    if (g2admin.get().get_layout_info(ref guid, out layout))
                    {
                        layout_list.Add(layout);
                    }
                }
            }

            trace("[LAYOUT] layout list size = {0}\n", layout_list.Count);
            if (layout_list.Count == 0) { return; }

            form_violet_layout form = new form_violet_layout(layout_list);
            if (form.ShowDialog() == DialogResult.OK)
            {
                G2GUID guid = form.get_selected_layout();
                int monitor = form.get_selected_monitor();

                if (!valid_channel(_channel)) { return; }
                trace("[SEND] monitor = {0}, layout = {1}\n", monitor, guid);
                _violet.request_set_layout_monitor_live(_channel, monitor, ref guid);
            }
        }

        protected void on_cbx_selchange_message_position(object sender, EventArgs e)
        {
            // ...
        }

        protected void on_btn_violet_audio_connect(object sender, EventArgs e)
        {
            EDT_VIOLET_MONITOR_NUMBER_AUDIO.Enabled = false;
            EDT_VIOLET_DEVICE_ADDRESS.Enabled = false;
            EDT_VIOLET_DEVICE_PORT.Enabled = false;
            EDT_VIOLET_DEVICE_USER_ID.Enabled = false;
            EDT_VIOLET_DEVICE_USER_PW.Enabled = false;
            BTN_VIOLET_AUDIO_CONNECT.Enabled = false;
            BTN_VIOLET_AUDIO_DISCONNECT.Enabled = false;

            string address = EDT_VIOLET_DEVICE_ADDRESS.Text;
            string id = EDT_VIOLET_DEVICE_USER_ID.Text;
            string password = EDT_VIOLET_DEVICE_USER_PW.Text;

            int monitor = 0;
            bool result = true;
            if (!int.TryParse(EDT_VIOLET_MONITOR_NUMBER_AUDIO.Text, out monitor))
            {
                trace("Failed parse audio monitor number!\n");
                result = false;
            }
            ushort port = 0;
            if (!ushort.TryParse(EDT_VIOLET_DEVICE_PORT.Text, out port))
            {
                trace("Failed parse device port!\n");
                result = false;
            }

            if (monitor < 1 || address.Length == 0 || port < 1 || id.Length == 0)
            {
                trace("audio connect invalid argument - monitor: {0}, address: {1}, port: {2}, id: {3}\n", monitor, address, port, id);
                result = false;
            }

            if (result)
            {
                result = _violet.send_agent_audio_connect(_channel, monitor, address, port, id, password, false);
            }
            if (!result)
            {
                EDT_VIOLET_MONITOR_NUMBER_AUDIO.Enabled = true;
                EDT_VIOLET_DEVICE_ADDRESS.Enabled = true;
                EDT_VIOLET_DEVICE_PORT.Enabled = true;
                EDT_VIOLET_DEVICE_USER_ID.Enabled = true;
                EDT_VIOLET_DEVICE_USER_PW.Enabled = true;
                BTN_VIOLET_AUDIO_CONNECT.Enabled = true;
                BTN_VIOLET_AUDIO_DISCONNECT.Enabled = false;
            }
        }

        protected void on_btn_violet_audio_disconnect(object sender, EventArgs e)
        {
            int monitor = 0;
            if (!int.TryParse(EDT_VIOLET_MONITOR_NUMBER_AUDIO.Text, out monitor))
            {
                trace("Failed parse audio monitor number!\n");
                return;
            }
            _violet.send_agent_audio_disconnect(_channel, monitor);
        }

        protected void on_btn_violet_layout_change(object sender, EventArgs e)
        {
            int monitor = 0;
            if (!int.TryParse(EDT_VIOLET_LAYOUT_MONITOR.Text, out monitor))
            {
                trace("Failed parse layout monitor!\n");
                return;
            }
            int camera = CBX_VIOLET_LAYOUT_CAMERA.SelectedIndex;
            int format = (int)G2LAYOUT.FORMAT.LAYOUT_UNDEFINED;
            int index = CBX_VIOLET_LAYOUT_FORMAT.SelectedIndex;
            if (index < _layout_info.Count)
            {
                format = (int)_layout_info[index].Key;
            }

            trace("[SEND] monitor = {0}, camera = {1}, format = {2}\n", monitor, camera, format);
            _violet.request_set_screen_camera_format(_channel, monitor, camera, format);
        }

        protected void on_btn_mouse_down(object sender, MouseEventArgs e)
        {
            if (!valid_channel(_channel)) { return; }

            if (!valid_channel(_channel))
            {
                return;
            }

            IDC idc = IDC.IDC_NULL;
            if (sender is Control)
            {
                Control c = sender as Control;
                if (c.Tag is IDC)
                {
                    idc = (IDC)c.Tag;
                }
            }
            on_post_violet_key_ptz(idc, PTZ_SPEED_DEFAULT);
        }

        protected void on_btn_mouse_up(object sender, MouseEventArgs e)
        {
            if (!valid_channel(_channel)) { return; }

            IDC idc = IDC.IDC_NULL;
            if (sender is Control)
            {
                Control c = sender as Control;
                if (c.Tag is IDC)
                {
                    idc = (IDC)c.Tag;
                }
            }
            on_post_violet_key_ptz(idc, PTZ_SPEED_STOP);
        }        

        protected void on_post_violet_key_ptz(IDC idc, int speed)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { on_post_violet_key_ptz(idc, speed); });
            }
            if (!valid_channel(_channel)) { return; }

            G2VIOLET_KEY_ID.ID id = get_key_id(idc);
            if (id < G2VIOLET_KEY_ID.ID.ID_PTZ_PAN_LEFT ||
                id > G2VIOLET_KEY_ID.ID.ID_PTZ_ZOOM_OUT)
            {
                return;
            }
            trace("[SEND] key = {0}, speed = {1}\n", id, speed);
            _violet.send_stream_channel_control_ptz_joystick(_channel, 0, (int)id, speed);
            _last_ptz_idc = ((speed == PTZ_SPEED_STOP) ? IDC.IDC_NULL : idc);
        }

        protected void on_btn_violet_message_show(object sender, EventArgs e)
        {
            EDT_VIOLET_MONITOR_MESSAGE.Enabled = true;
            EDT_VIOLET_MESSAGE_TIMEOUT.Enabled = true;
            EDT_VIOLET_MESSAGE_FONT_SIZE.Enabled = true;
            CBX_VIOLET_MESSAGE_POSITION.Enabled = true;
        }

        protected void on_btn_violet_message_hide(object sender, EventArgs e)
        {
            EDT_VIOLET_MONITOR_MESSAGE.Enabled = false;
            EDT_VIOLET_MESSAGE_TIMEOUT.Enabled = false;
            EDT_VIOLET_MESSAGE_FONT_SIZE.Enabled = false;
            CBX_VIOLET_MESSAGE_POSITION.Enabled = false;
        }

        protected void on_btn_violet_message_send(object sender, EventArgs e)
        {
            bool screen_message = (CBX_VIOLET_MESSAGE_TYPE.SelectedIndex == 0);
            bool show = RDO_VIOLET_MONITOR_MESSAGE_SHOW.Checked;
            int monitor = 0;
            if (!int.TryParse(EDT_VIOLET_MONITOR_NUMBER.Text, out monitor))
            {
                trace("Failed parse monitor number!\n");
                return;
            }            
            if (show)
            {
                string message = EDT_VIOLET_MONITOR_MESSAGE.Text;                
                uint timeout = 0;
                if (!uint.TryParse(EDT_VIOLET_MESSAGE_TIMEOUT.Text, out timeout))
                {
                    trace("Failed parse message timeout!\n");
                    return;
                }
                ushort fontsize = 0;
                if (!ushort.TryParse(EDT_VIOLET_MESSAGE_FONT_SIZE.Text, out fontsize))
                {
                    trace("Failed parse message font size!\n");
                    return;
                }

                if (message.Length == 0 || monitor < 1 || fontsize < 1)
                {
                    trace("message send invalid argument - message: {0}, monitor: {1}, fontsize: {2}", message, monitor, fontsize);
                    return;
                }

                int position = CBX_VIOLET_MESSAGE_POSITION.SelectedIndex;
                
                if (screen_message)
                {
                    G2VIOLET_MONITOR_MESSAGE msg = new G2VIOLET_MONITOR_MESSAGE();
                    msg._message = message;
                    msg._duration = timeout;
                    msg._color = 0xffffff;
                    msg._color_border = 0;
                    msg._font_height = fontsize;
                    msg._alpha = 200;
                    msg._bold = true;
                    msg._outline = true;
                    msg._margin = new G2RECT();
                    msg._margin.left = 0;
                    msg._margin.top = 0;
                    msg._margin.right = 0;
                    msg._margin.bottom = 0;

                    switch (position)
                    {
                        case (int)G2VIOLET_MONITOR_MESSAGE.POSITION.LT: msg._format = (uint)(G2DT.FORMAT.LEFT | G2DT.FORMAT.TOP); break;
                        case (int)G2VIOLET_MONITOR_MESSAGE.POSITION.CT: msg._format = (uint)(G2DT.FORMAT.CENTER | G2DT.FORMAT.TOP); break;
                        case (int)G2VIOLET_MONITOR_MESSAGE.POSITION.RT: msg._format = (uint)(G2DT.FORMAT.RIGHT | G2DT.FORMAT.TOP); break;
                        case (int)G2VIOLET_MONITOR_MESSAGE.POSITION.RC: msg._format = (uint)(G2DT.FORMAT.RIGHT | G2DT.FORMAT.VCENTER); break;
                        case (int)G2VIOLET_MONITOR_MESSAGE.POSITION.RB: msg._format = (uint)(G2DT.FORMAT.RIGHT | G2DT.FORMAT.BOTTOM); break;
                        case (int)G2VIOLET_MONITOR_MESSAGE.POSITION.CB: msg._format = (uint)(G2DT.FORMAT.CENTER | G2DT.FORMAT.BOTTOM); break;
                        case (int)G2VIOLET_MONITOR_MESSAGE.POSITION.LB: msg._format = (uint)(G2DT.FORMAT.LEFT | G2DT.FORMAT.BOTTOM); break;
                        case (int)G2VIOLET_MONITOR_MESSAGE.POSITION.LC: msg._format = (uint)(G2DT.FORMAT.LEFT | G2DT.FORMAT.VCENTER); break;
                    case (int)G2VIOLET_MONITOR_MESSAGE.POSITION.CC:
                        msg._format = (uint)(G2DT.FORMAT.CENTER | G2DT.FORMAT.VCENTER);
                        break;
                    }

                    _violet.send_screen_message(_channel, monitor, ref msg);
                }
                else
                {
                    _violet.send_monitor_message(_channel, monitor, message, timeout, fontsize, position);
                }
            }
            else
            {
                if (screen_message)
                {
                    _violet.send_screen_message_hide(_channel, monitor);
                }
                else
                {
                    _violet.send_monitor_message_hide(_channel, monitor);
                }
            }
        }

        protected void on_btn_violet_screen_send(object sender, EventArgs e)
        {
            int monitor = 0;
            if (!int.TryParse(EDT_VIOLET_MONITOR_NUMBER_SCREEN.Text, out monitor))
            {
                trace("Failed parse monitor number!\n");
                return;
            }

            if (valid_channel(_channel))
            {
                _violet.request_get_cameras_on_monitor(_channel, monitor);
            }
        }

        protected bool valid_channel(int channel)
        {
            return channel >= 0;
        }

        protected void trace(string fmt, params object[] objs)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(fmt, objs));
        }
        
        protected int _channel;
        protected g2violet _violet;
        protected LayoutList _layout_info;
        protected IDC _last_ptz_idc;
    }
}