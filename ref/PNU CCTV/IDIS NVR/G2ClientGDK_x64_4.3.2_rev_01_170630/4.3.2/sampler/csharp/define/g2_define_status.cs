using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2STATUS_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_receive_instant_recording_start = 7,
            on_receive_instant_recording_stop = 8,
            on_receive_instant_recording_status = 9,
            on_receive_status = 2,
            on_receive_callback_event = 3,
            on_receive_log_system = 4,
            on_receive_log_event = 5,
            on_receive_log_debug = 6,

            CALLBACK_COUNT = 10
        }
    }

    public struct G2STATUS_SUPPORT
    {
        public enum QUERY
        {
            SUPPORT_LOG_SYSTEM = 1,
            SUPPORT_LOG_EVENT = 2,
            SUPPORT_LOG_DEBUG = 3,
            SUPPORT_LOG_G2 = 4,
            SUPPORT_INSTANT_RECORDING = 63
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2STATUS_LOG_SEARCH_OPTIONS
    {
        public enum TYPE
        {
            UNDEFINED = 0,
            SYSTEM = 1,
            EVENT,
            DEBUG
        }

        public G2TIME _from;
        public G2TIME _to;
        public TYPE _type;
        [MarshalAs(UnmanagedType.U1)]
        public bool _reload;
    }

    public struct G2STATUS_LOG
    {
        public enum G1_SYSTEM_LOG
        {
            UNKNOWN = 0,
            BOOT_UP = 1,
            SHUTDOWN,
            RESTART,
            LOCAL_UPGRADE,
            LOCAL_UPGRADE_FAIL,
            REMOTE_UPGRADE,
            REMOTE_UPGRADE_FAIL,
            POWER_FAILURE,
            LOCAL_TIME_CHANGE,
            LOCAL_TIME_ZONE_CHANGE,
            REMOTE_TIME_CHANGE,
            REMOTE_TIME_ZONE_CHANGE,
            TIME_SYNCED,
            TIME_SYNC_FAILED,
            DISK_BAD,
            DISK_CONFIG_CHANGE,
            DISK_ON,
            DISK_OFF,
            FACTORY_RESET,
            HEATER_ON,
            ACC_ON_OFF,
            DISK_INFO,
            DISK_REMOVED,
            NO_STORAGE,
            NOT_FORMATTED_STORAGE,
            AUTO_FORMAT,
            KEY_ON_OFF,
            SHUTOFF,
            LOCAL_LOGIN = 100,
            LOCAL_LOGOUT,
            REMOTE_LOGIN,
            REMOTE_LOGOUT,
            PASSWORD_MISMATCH,
            PASSWORD_CHANGED_BY_AUTHENTICATION,
            LOCAL_SETUP_BEGIN = 200,
            LOCAL_SETUP_END,
            REMOTE_SETUP_CHANGE,
            REMOTE_SETUP_FAIL,
            SETUP_IMPORTED,
            SETUP_IMPORT_FAIL,
            SETUP_EXPORTED,
            SETUP_EXPORT_FAIL,
            SETUP_EXPORT_CANCEL,
            LOCAL_SETUP_CHANGE,
            SCHEDULE_ON = 300,
            SCHEDULE_OFF,
            PANIC_ON,
            PANIC_OFF,
            LOCAL_CLEAR_ALL_DATA,
            REMOTE_CLEAR_ALL_DATA,
            CLEAR_DISK,
            FORMAT_DISK,
            TANGO_FULL,
            AUTO_DELETION,
            ARCHIVE_ON,
            ARCHIVE_OFF,
            AUTO_DELETION_ARCHIVE,
            REMOTE_FORMAT_DISK,
            LOCAL_SEARCH_BEGIN = 400,
            LOCAL_SEARCH_END,
            REMOTE_SEARCH_BEGIN,
            REMOTE_SEARCH_END,
            LOCAL_CLIP_COPY_BEGIN = 500,
            LOCAL_CLIP_COPY_END,
            LOCAL_CLIP_COPY_CANCEL,
            LOCAL_CLIP_COPY_FAIL,
            REMOTE_CLIP_COPY_BEGIN,
            REMOTE_CLIP_COPY_END,
            REMOTE_CLIP_COPY_CANCEL,
            REMOTE_CLIP_COPY_FAIL,
            LOCAL_CLIP_COPY_INFO_USER,
            LOCAL_CLIP_COPY_INFO_FROM_TIME,
            LOCAL_CLIP_COPY_INFO_TO_TIME,
            LOCAL_CLIP_COPY_INFO_DURATION,
            LOCAL_CLIP_COPY_INFO_EXPORT_TIME,
            LOCAL_CLIP_COPY_INFO_CAMERA_TITLE,
            REMOTE_CLIP_COPY_INFO_USER,
            REMOTE_CLIP_COPY_INFO_FROM_TIME,
            REMOTE_CLIP_COPY_INFO_TO_TIME,
            REMOTE_CLIP_COPY_INFO_DURATION,
            REMOTE_CLIP_COPY_INFO_EXPORT_TIME,
            REMOTE_CLIP_COPY_INFO_CAMERA_TITLE,
            LOCAL_CLIP_COPY_INFO_SITE_NAME,
            REMOTE_CLIP_COPY_INFO_SITE_NAME,
            CLIP_COPY_ARCHIVE_EXPORTED,
            CLIP_COPY_ARCHIVE_EXPORT_FAIL,
            CLIP_COPY_ARCHIVE_EXPORT_CANCEL,
            CALLBACK_FAIL = 600,
            WATCH_CLIENT_CONNECTED,
            WATCH_CLIENT_DISCONNECTED,
            WATCH_CLIENT_CONNECT_FAILED,
            AUDIO_CLIENT_CONNECTED,
            AUDIO_CLIENT_DISCONNECTED,
            AUDIO_CLIENT_CONNECT_FAILED,
            FTP_UPLOAD_FAIL,
            FTP_UPLOAD_OK,
            UPNP_OK = 610,
            UPNP_PORT_CHANGED,
            UPNP_ERROR_NO_IGD,
            UPNP_ERROR_NETWORK,
            UPNP_ERROR_FULL,
            UPNP_ERROR_CONFLICT,
            PUSH_NOTIFICATION_FAIL = 620,
            NETFS_CONNECTED,
            NETFS_DISCONNECTED,
            LOCAL_OTP_SESSION_START = 630,
            LOCAL_OTP_SESSION_CANCEL,
            LOCAL_OTP_SESSION_FINISH,
            REMOTE_OTP_SESSION_START,
            REMOTE_OTP_SESSION_CANCEL,
            REMOTE_OTP_SESSION_FINISH,
            NETWORK_CAMERA_REBOOT = 640,
            PRINT_BEGIN = 700,
            PRINT_END,
            PRINT_CANCEL,
            MIRROR_1_START = 800,
            MIRROR_1_START_FAIL,
            MIRROR_1_STOP,
            MIRROR_1_STOP_FAIL,
            MIRROR_2_START,
            MIRROR_2_START_FAIL,
            MIRROR_2_STOP,
            MIRROR_2_STOP_FAIL,
            RESCHEDULE_START = 900,
            RESCHEDULE_STOP,
            SEQUENCE_ON = 1000,
            SEQUENCE_OFF,
            EVENT_MONITORING_ENABLED,
            EVENT_MONITORING_DISABLED,
            ALARM_AUDIO_IMPORT_FAIL = 1100,
            ALARM_AUDIO_IMPORTED,
            ALARM_AUDIO_EXPORT_FAIL,
            ALARM_AUDIO_EXPORTED,
            ALARM_AUDIO_EXPORT_CANCEL,
            PARTIAL_DATA_ERASE_BEGIN = 1200,
            PARTIAL_DATA_ERASE_END,
            PARTIAL_DATA_ERASE_CANCEL,
            PARTIAL_DATA_ERASE_FAIL,
            PARTIAL_DATA_ERASE_INFO_INFO_USER,
            PARTIAL_DATA_ERASE_INFO_FROM_TIME,
            PARTIAL_DATA_ERASE_INFO_TO_TIME,
            PARTIAL_DATA_ERASE_INFO_DURATION,
            PARTIAL_DATA_ERASE_INFO_CAMERA_TITLE,
            SYSTEM_LOG_EXPORTED = 1300,
            SYSTEM_LOG_EXPORT_FAIL,
            SYSTEM_LOG_EXPORT_CANCEL,
            NETWORK_CAMERA_UPGRADE_BEGIN = 1400,
            NETWORK_CAMERA_UPGRADE_END,
            NETWORK_CAMERA_UPGRADE_FAIL,
            NETWORK_CAMERA_UPGRADE_INFO_USER,
            NETWORK_CAMERA_UPGRADE_INFO_TITLE,
            NETWORK_CAMERA_REGISTER_BEGIN,
            NETWORK_CAMERA_REGISTER_END,
            NETWORK_CAMERA_REGISTER_FAIL,
            NETWORK_CAMERA_REGISTER_CANCEL,
            EQUALIZER_INITIALIZE_ALL_CHANNEL = 1500,
            EQUALIZER_INITIALIZE_EACH_CHANNEL,
            SECOM_FS_CONNECTED = 11000,
            SECOM_FS_DISCONNECTED,
            SECOM_HOST_CONNECTED = 11100,
            SECOM_HOST_DISCONNECTED
        }
        public enum G2_LOG_TYPE_SYSTEM
        {
            GENERAL = 10100000,
            GENERAL_SYSTEM_BOOT_UP = 10100001,
            GENERAL_SYSTEM_SHUT_DOWN = 10100002,
            GENERAL_SYSTEM_RESTART = 10100003,
            GENERAL_POWER_FAILURE = 10100004,
            GENERAL_FACTORY_RESET = 10100005,
            GENERAL_SHUTOFF = 10100006,
            GENERAL_FACTORY_RESET_WONET = 10100007,
            GENERAL_RESET_ADMIN_PASSWORD = 10100008,
            USER = 10200000,
            USER_LOCAL_LOGIN = 10200001,
            USER_LOCAL_LOGOUT = 10200002,
            USER_REMOTE_LOGIN = 10200003,
            USER_REMOTE_LOGOUT = 10200004,
            USER_PASSWORD_MISMATCH = 10200005,
            USER_PASSWORD_CHANGED_BY_AUTHENTICATION = 10200006,
            UPGRADE = 10300000,
            LOCAL_UPGRADE = 10300001,
            LOCAL_UPGRADE_FAILED = 10300002,
            REMOTE_UPGRADE = 10300003,
            REMOTE_UPGRADE_FAILED = 10300004,
            TIME = 10400000,
            LOCAL_TIME_CHANGE = 10400001,
            LOCAL_TIME_ZONE_CHANGE = 10400002,
            REMOTE_TIME_CHANGE = 10400003,
            REMOTE_TIME_ZONE_CHANGE = 10400004,
            TIME_SYNCED = 10400005,
            TIME_SYNC_FAILED = 10400006,
            SETUP = 10500000,
            LOCAL_SETUP_BEGIN = 10500001,
            LOCAL_SETUP_END = 10500002,
            LOCAL_SETUP_CHANGE = 10500003,
            REMOTE_SETUP_CHANGE = 10500004,
            REMOTE_SETUP_FAILED = 10500005,
            SETUP_IMPORTED = 10500006,
            SETUP_IMPORT_FAILED = 10500007,
            SETUP_EXPORTED = 10500008,
            SETUP_EXPORT_FAILED = 10500009,
            SETUP_EXPORT_CANCELED = 10500010,
            NETWORK = 10600000,
            NETWORK_FEN = 10601000,
            NETWORK_FEN_NOTIFY_OK = 10601001,
            NETWORK_FEN_NOTIFY_FAILED = 10601002,
            NETWORK_UPNP = 10602000,
            NETWORK_UPNP_REGISTER_OK = 10602001,
            NETWORK_UPNP_REGISTER_NEW_PORT = 10602002,
            NETWORK_UPNP_REGISTER_FAILED = 10602003,
            NETWORK_DIRECTIP = 10603000,
            NETWORK_DIRECTIP_RESET_REGISTRATION = 10603001,
            NETWORK_DIRECTIP_SWITCHED_TO_DIRECTIP = 10603002,
            NETWORK_DIRECTIP_SWITCHED_TO_OPENIP = 10603003,
            NETWORK_TCP_CALLBACK = 10604000,
            NETWORK_TCP_CALLBACK_OK = 10604001,
            NETWORK_TCP_CALLBACK_FAILED = 10604002,
            NETWORK_EMAIL = 10605000,
            NETWORK_EMAIL_SEND_OK = 10605001,
            NETWORK_EMAIL_SEND_FAILED = 10605002,
            NETWORK_FTP = 10606000,
            NETWORK_FTP_UPLOAD_OK = 10606001,
            NETWORK_FTP_UPLOAD_FAILED = 10606002,
            NETWORK_CLOUD = 10607000,
            NETWORK_CLOUD_UPLOAD_OK = 10607001,
            NETWORK_CLOUD_UPLOAD_FAILED = 10607002
        }
        public enum G2_LOG_TYPE_DEBUG
        {
            GENERAL = 20100000
        }

        public enum RESULT
        {
            SUCCESS = 0,
            FAIL,
            NO_MORE,
            LOADING,
            NOT_CONNECTED,
            NOT_SUPPORT
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2STATUS_LOG_SYSTEM
    {
        public enum VERSION
        {
            LEGACY = 0,
            IDR,
            G1,
            G2
        }

        public VERSION _version;
        public uint _type;
        public G2TIME _time;
        public G2STRING_128 _label;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public sbyte[] _data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2STATUS_LOG_EVENT
    {
        public enum VERSION
        {
            LEGACY = 0,
            IDR,
            G1,
            G2
        }

        public VERSION _version;
        public int _level1;
        public int _level2;
        public G2TIME _time;
        public G2STRING_64 _event;
        public G2STRING_128 _label;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public sbyte[] _data;
    }
}
