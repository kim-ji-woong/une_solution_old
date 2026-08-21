using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CCTVViewer
{
    public partial class FormMain : Form
    {
        private const short LAYOUT_1X1 = 0;
        private const short LAYOUT_2X2 = 1;
        private const short LAYOUT_3X3 = 2;
        private const short LAYOUT_4X4 = 3;
        private const short LAYOUT_5X5 = 4;
        private const short LAYOUT_6X6 = 5;
        private const short LAYOUT_7X7 = 6;
        private const short LAYOUT_8X8 = 7;
        private const short LAYOUT_8X1 = 8;
        private const short LAYOUT_12X1 = 9;
        private const short LAYOUT_32X1 = 10;

        private int m_nCCTVID = 0, m_nPort = 0;
        private string m_strIP = "", m_strUserID = "", m_strPW = "", m_strCameraName = "";
        private IntPtr m_parentWindowHandle = IntPtr.Zero;

        const int WM_COPYDATA = 0x4A;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref COPYDATASTRUCT lParam);

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        public FormMain(int nCCTVID, string strIP, int nPort, string strUserID, string strPW, string strCameraName, IntPtr parentWindowHandle)
        {
            m_nCCTVID = nCCTVID;
            m_strIP = strIP;
            m_nPort = nPort;
            m_strUserID = strUserID;
            m_strPW = strPW;
            m_strCameraName = strCameraName;
            m_parentWindowHandle = parentWindowHandle;

            InitializeComponent();

            this.labelIP.Text = string.Format("{0} - {1}", m_nCCTVID, m_strIP);
            this.labelCCTVName.Text = string.Format("{0}", m_strCameraName);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //this.DesktopLocation = new Point(this.Size.Width * m_nColumnIndex, this.Size.Height * m_nRowIndex);
            axRASplus_WatSear1.initialize();

            axRASplus_WatSear1.setLayout(LAYOUT_1X1);
            axRASplus_WatSear1.setupOSD(false, false, false, false, false, false);
            // 접속 시도시 메시지박스가 나타나지 않도록 한다.
            axRASplus_WatSear1.setHiddenMessageBox(true);
            // Mouse 오른쪽 버튼 Click시 팝업메뉴가 나타나지 않도록 한다.
            axRASplus_WatSear1.setProperty(0, 0, 0, 0, "", "");

            axRASplus_WatSear1.setCameraMap(0, 0, m_strCameraName, m_strIP, 0, m_strUserID, m_strPW, m_nPort, false, false, false, "", 0, 0);
            axRASplus_WatSear1.connect();

            //SendMessage(m_parentWindowHandle, WM_COPYDATA, 0, COPYDATASTRUCT)
        }

        private void btnTestAlarm_Click(object sender, EventArgs e)
        {
            SendAlarm(1);
        }

        private void SendAlarm(int nData)
        {
            if (m_parentWindowHandle != IntPtr.Zero)
            {
                COPYDATASTRUCT data = new COPYDATASTRUCT();
                data.dwData = new IntPtr(nData);
                data.cbData = m_nCCTVID;

                SendMessage(m_parentWindowHandle, WM_COPYDATA, 0, ref data);
            }
        }

        const int RSC_EVNET_TYPE_UNKNOWN = -1;
        const int RSC_EVENT_TYPE_ALMOST_FULL = 0;
        const int RSC_EVENT_TYPE_ALARM_IN = 1;
        const int RSC_EVENT_TYPE_TEXT_IN = 2;
        const int RSC_EVENT_TYPE_MOTION = 3;
        const int RSC_EVENT_TYPE_VIDEOLOSS = 4;
        const int RSC_EVENT_TYPE_RECORDER_BAD = 9;
        const int RSC_EVENT_TYPE_ALARM_IN_BAD = 10;
        const int RSC_EVENT_TYPE_DISK_BAD = 11;
        const int RSC_EVENT_TYPE_TEMPERATURE = 12;
        const int RSC_EVENT_TYPE_DISK_SMART = 13;
        const int RSC_EVENT_SYSTEM_ALIVE = 14;
        const int RSC_EVENT_TYPE_PANIC_ON = 15;
        const int RSC_EVENT_SIPASS_REC_ON = 16;
        const int RSC_EVENT_SIPASS_REC_OFF = 17;
        const int RSC_EVENT_OBJECT_TRACKER = 18;
        const int RSC_EVENT_VIDEO_BLIND = 19;
        const int RSC_EVENT_TYPE_PANIC_OFF = 20;
        const int RSC_EVENT_TYPE_ALARM_IN_OFF = 21;
        const int RSC_EVENT_TYPE_FAN_ON = 23;
        const int RSC_EVENT_TYPE_FAN_OFF = 24;
        const int RSC_EVENT_TYPE_SECOM_FS_LOOP = 25;
        const int RSC_EVENT_TYPE_SECOM_FS_PANIC = 26;
        const int RSC_EVENT_TYPE_SECOM_FS_CARD = 27;
        const int RSC_EVENT_TYPE_MOTION_OFF = 29;
        const int RSC_EVENT_TYPE_VIDEO_BLIND_OFF = 30;
        const int RSC_EVENT_TYPE_NETWORK_ALARM_ON = 34;
        const int RSC_EVENT_TYPE_NETWORK_ALARM_OFF = 35;
        const int RSC_EVENT_TYPE_DISK_CONFIG_CHANGE = 36;
        const int RSC_EVENT_TYPE_STORAGE_ON = 40;
        const int RSC_EVENT_TYPE_STORAGE_OFF = 41;
        const int RSC_EVENT_TYPE_VIDEO_ANALYTICS = 42;
        const int RSC_EVENT_TYPE_COVER_OPEN = 45;
        const int RSC_EVENT_TYPE_COVER_CLOSE = 46;
        const int RSC_EVENT_TYPE_AUDIO_ON = 61;
        const int RSC_EVENT_TYPE_TRIPZONE_ON = 64;
        const int RSC_EVENT_TYPE_TAMPER_ON = 67;
        const int RSC_EVENT_TYPE_TEXT_IN_BAD_ON = 71;
        const int RSC_EVENT_TYPE_TEXT_IN_BAD_OFF = 72;
        const int RSC_EVENT_TYPE_USER_DEFINED_ALARM_ON = 73;
        const int RSC_EVENT_TYPE_USER_DEFINED_ALARM_OFF = 74;
        const int RSC_EVENT_TYPE_LOGIN_FAILED_SEVERAL_TIMES = 77;
        const int RSC_EVENT_TYPE_FACE_DETECTION_ON = 82;
        const int RSC_EVENT_TYPE_IGNORED_FACE_DETECTION_ON = 83;
        const int RSC_EVENT_TYPE_FACE_DETECTION_OFF = 84;
        const int RSC_EVENT_TYPE_CAMERA_RECORD_BAD_ON = 85;
        const int RSC_EVENT_TYPE_CAMERA_RECORD_BAD_OFF = 86;
        const int RSC_EVENT_TYPE_CAMERA_FAN_ERROR_ON = 87;
        const int RSC_EVENT_TYPE_CAMERA_FAN_ERROR_OFF = 88;

        public string getEventTypeIconName(int eventType)
        {
            string getEventTypeIconName = "";
            switch (eventType)
            {
                case RSC_EVENT_TYPE_ALMOST_FULL:
                    getEventTypeIconName = "event_almost_full";
                    break;
                case RSC_EVENT_TYPE_ALARM_IN:
                    getEventTypeIconName = "event_alarm_in";
                    break;
                case RSC_EVENT_TYPE_TEXT_IN:
                    getEventTypeIconName = "event_text_in";
                    break;
                case RSC_EVENT_TYPE_MOTION:
                    getEventTypeIconName = "event_motion";
                    break;
                case RSC_EVENT_TYPE_VIDEOLOSS:
                    getEventTypeIconName = "event_video_loss";
                    break;
                case RSC_EVENT_TYPE_TEMPERATURE:
                    getEventTypeIconName = "event_temperature";
                    break;
                case RSC_EVENT_TYPE_PANIC_OFF:
                    getEventTypeIconName = "event_panic_off";
                    break;
                case RSC_EVENT_TYPE_PANIC_ON:
                    getEventTypeIconName = "event_panic_on";
                    break;
                case RSC_EVENT_TYPE_RECORDER_BAD:
                    getEventTypeIconName = "event_recorder_bad";
                    break;
                case RSC_EVENT_TYPE_ALARM_IN_BAD:
                    getEventTypeIconName = "event_alarm_in_bad";
                    break;
                case RSC_EVENT_TYPE_DISK_BAD:
                    getEventTypeIconName = "event_disk_bad";
                    break;
                case RSC_EVENT_TYPE_DISK_SMART:
                    getEventTypeIconName = "event_disk_smart";
                    break;
                case RSC_EVENT_OBJECT_TRACKER:
                    getEventTypeIconName = "event_object_tracker";
                    break;
                case RSC_EVENT_VIDEO_BLIND:
                    getEventTypeIconName = "event_video_blind";
                    break;
                case RSC_EVENT_TYPE_ALARM_IN_OFF:
                    getEventTypeIconName = "event_alarm_in_off";
                    break;
                case RSC_EVENT_TYPE_FAN_OFF:
                    getEventTypeIconName = "event_fan_error_off";
                    break;
                case RSC_EVENT_TYPE_FAN_ON:
                    getEventTypeIconName = "event_fan_error_on";
                    break;
                case RSC_EVENT_TYPE_SECOM_FS_LOOP:
                    getEventTypeIconName = "event_alarm_in";
                    break;
                case RSC_EVENT_TYPE_SECOM_FS_PANIC:
                    getEventTypeIconName = "event_panic_on";
                    break;
                case RSC_EVENT_TYPE_SECOM_FS_CARD:
                    getEventTypeIconName = "event_card";
                    break;
                case RSC_EVENT_SIPASS_REC_ON:
                    getEventTypeIconName = "sipass_panic_on";
                    break;
                case RSC_EVENT_SIPASS_REC_OFF:
                    getEventTypeIconName = "sipass_panic_off";
                    break;
                case RSC_EVENT_TYPE_MOTION_OFF:
                    getEventTypeIconName = "event_motion_off";
                    break;
                case RSC_EVENT_TYPE_VIDEO_BLIND_OFF:
                    getEventTypeIconName = "event_video_blind_off";
                    break;
                case RSC_EVENT_TYPE_NETWORK_ALARM_ON:
                    getEventTypeIconName = "event_network_alarm_on";
                    break;
                case RSC_EVENT_TYPE_NETWORK_ALARM_OFF:
                    getEventTypeIconName = "event_network_alarm_off";
                    break;
                case RSC_EVENT_TYPE_STORAGE_ON:
                    getEventTypeIconName = "event_storage_on";
                    break;
                case RSC_EVENT_TYPE_STORAGE_OFF:
                    getEventTypeIconName = "event_storage_off";
                    break;
                case RSC_EVENT_TYPE_VIDEO_ANALYTICS:
                    getEventTypeIconName = "event_video_analytics";
                    break;
                case RSC_EVENT_TYPE_COVER_OPEN:
                    getEventTypeIconName = "event_cover_open";
                    break;
                case RSC_EVENT_TYPE_COVER_CLOSE:
                    getEventTypeIconName = "event_cover_close";
                    break;
                case RSC_EVENT_TYPE_AUDIO_ON:
                    getEventTypeIconName = "event_audio_on";
                    break;
                case RSC_EVENT_TYPE_DISK_CONFIG_CHANGE:
                    getEventTypeIconName = "event_disk_config_change";
                    break;
                case RSC_EVENT_TYPE_TRIPZONE_ON:
                    getEventTypeIconName = "event_tripzone_on";
                    break;
                case RSC_EVENT_TYPE_TAMPER_ON:
                    getEventTypeIconName = "event_tamper_on";
                    break;
                case RSC_EVENT_TYPE_TEXT_IN_BAD_ON:
                    getEventTypeIconName = "event_text_in_bad";
                    break;
                case RSC_EVENT_TYPE_LOGIN_FAILED_SEVERAL_TIMES:
                    getEventTypeIconName = "event_login_failed";
                    break;
                case RSC_EVENT_TYPE_FACE_DETECTION_ON:
                    getEventTypeIconName = "event_facedetection_on";
                    break;
                case RSC_EVENT_TYPE_IGNORED_FACE_DETECTION_ON:
                    getEventTypeIconName = "event_facedetection_on";
                    break;
                case RSC_EVENT_TYPE_FACE_DETECTION_OFF:
                    getEventTypeIconName = "event_facedetection_off";
                    break;
                case RSC_EVENT_TYPE_USER_DEFINED_ALARM_ON:
                    getEventTypeIconName = "event_alarm_in";
                    break;
                case RSC_EVENT_TYPE_USER_DEFINED_ALARM_OFF:
                    getEventTypeIconName = "event_alarm_in_off";
                    break;
                case RSC_EVENT_TYPE_CAMERA_RECORD_BAD_ON:
                    getEventTypeIconName = "event_recordfail_on";
                    break;
                case RSC_EVENT_TYPE_CAMERA_RECORD_BAD_OFF:
                    getEventTypeIconName = "event_recordfail_off";
                    break;
                case RSC_EVENT_TYPE_CAMERA_FAN_ERROR_ON:
                    getEventTypeIconName = "event_fan_error_on";
                    break;
                case RSC_EVENT_TYPE_CAMERA_FAN_ERROR_OFF:
                    getEventTypeIconName = "event_fan_error_off";
                    break;
                default:
                    getEventTypeIconName = "event_unknown";
                    break;
            }
            return getEventTypeIconName;
        }

        private void axRASplus_WatSear1_EventLoaded(object sender, AxRASplus_WatSearLib._DRASplus_WatSearEvents_EventLoadedEvent e)
        {
            try
            {
                dynamic obj = (dynamic)e.eventLog;
                int nEventType = (int)obj.EventType;

                if (nEventType == RSC_EVENT_TYPE_ALARM_IN)
                {
                    string szEvent = getEventTypeIconName(nEventType);
                    System.Diagnostics.Trace.WriteLine(nEventType + " " + szEvent);
                    SendAlarm(1);
                }
                else if (nEventType == RSC_EVENT_TYPE_ALARM_IN_OFF)
                {
                    string szEvent = getEventTypeIconName(nEventType);
                    System.Diagnostics.Trace.WriteLine(nEventType + " " + szEvent);
                    SendAlarm(0);
                }

                //System.Diagnostics.Trace.WriteLine(nEventType);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
    }
}
