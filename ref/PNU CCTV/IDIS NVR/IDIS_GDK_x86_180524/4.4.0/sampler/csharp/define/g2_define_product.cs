using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    [StructLayout(LayoutKind.Sequential)]
    public struct G2_PRODUCT_INFO_CAPS
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct IP_CAMERA
        {
            public enum IP_MODE_TYPE
            {
                NOT_CHANGEABLE = 0,
                CHANGABLE_OPEN_DIRECT = 1
            }
            public enum DEWARPING_TYPE
            {
                NOT_SUPPORT = 0,
                VER_1_0_0 = 1
            }

            [MarshalAs(UnmanagedType.U1)]
            public bool is_ip_camera;
            [MarshalAs(UnmanagedType.U1)]
            public bool auto_iris;
            public byte ip_mode;
            public byte dewarping;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct REMOTE_ACCESSABLE
        {
            public enum PUSH_NOTIFICATION_TYPE
            {
                NONE = 0,
                CALLBACK = 1
            }
            public enum FEN_SERVICE_VERSION
            {
                NONE = 0,
                VER_1_0_0 = 1
            }
            public enum PORT_TYPE
            {
                SEPERATE = 0,
                UNITY = 1
            }

            [MarshalAs(UnmanagedType.U1)]
            public bool encrypt_password;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_RTP;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_RTSP;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_status;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_watch;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_search;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_audio;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_record;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_dvrns;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_ddns;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_onvif;
            [MarshalAs(UnmanagedType.U1)]
            public bool support_vnc;
            public byte support_push;
            public byte support_fen;
            public byte port_type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct REMOTE_ACCESS
        {
            public byte count_status;
            public byte count_watch;
            public byte count_search;
            public byte count_record;
            public byte count_audio_in;
            public byte count_audio_out;
            public uint authorities;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct REMOTE_STATUS
        {
            public enum BEEP_CONTROL_TYPE
            {
                NONE = 0,
                VER_1_0_0 = 1
            }
            public enum INSTANT_RECORDING_TYPE
            {
                INSTANT_RECORDING_TYPE_NOT_SUPPORT = 0,
                INSTANT_RECORDING_TYPE_1_0_0
            }

            [MarshalAs(UnmanagedType.U1)]
            public bool remote_panic_recording;
            [MarshalAs(UnmanagedType.U1)]
            public bool log_event;
            [MarshalAs(UnmanagedType.U1)]
            public bool log_debug;
            [MarshalAs(UnmanagedType.U1)]
            public byte beep_control;
            [MarshalAs(UnmanagedType.U1)]
            public byte instant_recording;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct REMOTE_WATCH
        {
            public enum TICK_TYPE
            {
                TICK = 0,
                SYSTEM_MSEC = 1,
                LEGACY_MSEC = 2,
                SEC = 1
            }
            public enum COMMAND_TYPE
            {
                CONTROL_COLOR = 0,
                CONTROL_COLOR_RELAY,
                CONTROL_PTZ,
                CONTROL_PTZ_RELATIVE,
                COMMAND_RELAY
            }

            public byte tick_type;
            public byte stream_count;
            [MarshalAs(UnmanagedType.U1)]
            public bool transfer_multi_stream;
            [MarshalAs(UnmanagedType.U1)]
            public bool motion_disp;
            [MarshalAs(UnmanagedType.U1)]
            public bool audio_sync;
            [MarshalAs(UnmanagedType.U1)]
            public bool xframe;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            ushort[] command_types;
            uint command_types_len;

            public g2set<ushort> get_command_types()
            {
                return new g2set<ushort>(command_types, (int)command_types_len);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct REMOTE_SEARCH
        {
            public enum VERSION
            {
                UNKNOWN = 0,
                LEGACY = 1,
                SAMBA_LIKE = 2,
                SAMBA = 3,
                G2 = 4
            }
            public enum TICK_TYPE
            {
                TICK = 0,
                SYSTEM_MSEC = 1,
                LEGACY_MSEC = 2
            }
            public enum REC_INFO_TYPE
            {
                UNKNOWN = 0,
                HOUR = 1,
                MINUTE = 2,
                IDR_MINUTE = 3,
                SECOND = 4
            }

            public uint version;
            public uint tick_type;
            public uint rec_info_type;
            public byte stream_count;
            public byte base_channel;
            [MarshalAs(UnmanagedType.U1)]
            public bool archive;
            [MarshalAs(UnmanagedType.U1)]
            public bool external;
            [MarshalAs(UnmanagedType.U1)]
            public bool audio_play;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public int[] events;
            public uint events_len;

            public g2set<int> get_event_set()
            {
                return new g2set<int>(events, (int)events_len);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct REMOTE_CLIP_COPY
        {
            public enum VERSION
            {
                UNKNOWN = 0,
                MINIBANK = 1,
                CLIP_COPY = 2
            }
            public enum INCLUDE_DATA
            {
                TEXT_IN = 1,
                GPS = 2
            }
            public enum SLICE_LARGE_FILE
            {
                SUPPORT_NONE = 0,
                SUPPORT_1_0_0 = 1
            }

            public uint version;
            [MarshalAs(UnmanagedType.U1)]
            public bool password_save;
            [MarshalAs(UnmanagedType.U1)]
            public bool password_encrypt;
            [MarshalAs(UnmanagedType.U1)]
            public bool available_channels;
            [MarshalAs(UnmanagedType.U1)]
            public bool player_exclude;
            [MarshalAs(UnmanagedType.U1)]
            public bool player_exist;
            [MarshalAs(UnmanagedType.U1)]
            public bool partial_clip;
            public uint include_data;
            public byte support_slice_large_file;

            public bool contains_data(INCLUDE_DATA data) { return (include_data & (uint)data) != 0; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECORD_DB
        {
            public enum TYPE
            {
                NOT_SUPPORT = 0,
                IBANK_1_0 = 1,
                IBANK_2_0 = 2,
                IBANK_3_0 = 3
            }

            public byte type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVICE
        {
            public byte count_camera;
            public byte count_audio_in;
            public byte count_audio_out;
            public byte count_alarm_in;
            public byte count_alarm_out;
            public byte count_text_in;
            public byte count_alarm_in_network;
            public byte count_alarm_out_network;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVICE_HYBRID
        {
            public byte count_camera;
            public byte count_alarm_in;
            public byte count_alarm_out;
            public byte count_audio_in;
            public byte count_audio_out;
            public byte count_network_camera;
            public byte count_network_alarm_in;
            public byte count_network_alarm_out;
            public byte count_network_audio_in;
            public byte count_network_audio_out;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TEXT_IN_SEARCH
        {
            public enum VERSION
            {
                NONE = 0,
                IDR = 1,
                ADVANCED = 2
            }

            public byte version;
            [MarshalAs(UnmanagedType.U1)]
            public bool match_whole_word;
            [MarshalAs(UnmanagedType.U1)]
            public bool transaction_wise;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CODEC_INFO_VIDEO
        {
            public enum TYPE
            {
                UNKNOWN = 0,
                MJPEG = 1,
                MLJPEG = 2,
                MPEG4 = 3,
                H264 = 4,
                BITMAP = 5,
                MXPEG = 6,
                HEVC = 7,
                UNRESTRICTED = 255
            }

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] codecs;

            public g2set<byte> get_codec_set()
            {
                g2set<byte> set = new g2set<byte>();
                if (codecs != null)
                {
                    foreach (byte c in codecs)
                    {
                        if (c != 0) set.insert(c);
                    }
                }
                return set;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CODEC_INFO_AUDIO
        {
            public enum TYPE
            {
                UNKNOWN = 0,
                ADPCM = 1,
                L16 = 2,
                G723 = 3,
                G726 = 4,
                G711_ULAW = 5,
                G711_ALAW = 6,
                ISP1000 = 7,
                ADPCM4BIT = 8,
                ADPCMF = 9,
                G721 = 10,
                AAC = 11,
                AAC_INT = 12,
                PCM = 13,
                UNRESTRICTED = 255
            }

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] codecs;

            public g2set<byte> get_codec_set()
            {
                g2set<byte> set = new g2set<byte>();
                if (codecs != null)
                {
                    foreach (byte c in codecs)
                    {
                        if (c != 0) set.insert(c);
                    }
                }
                return set;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2_PRODUCT_INFO
    {
        public enum TYPE
        {
            UNKNOWN = 0,
            REMOTE_ACCESSABLE = 1,
            VIDEO_SERVER = 2,
            VIDEO_DECODER = 3,
            KEYBOARD = 4,
            MMX = 5,
            SWITCH = 7,
            MOBILE_DEVICE = 8,
            INEX_SERVICE = 10000,
            RAS = 20000,
            CAMERA = 100000,
            AUDIO_IN = 100001,
            AUDIO_OUT = 100002,
            ALARM_IN = 100003,
            ALARM_OUT = 100004,
            TEXT_IN = 100005
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BASIC_INFO
        {
            public enum NVR_TYPE
            {
                NONE = 0,
                NORMAL = 1,
                DIRECT_IP = 2
            }
            public enum ANALOG_HD_TYPE
            {
                ANALOG_HD_TYPE_NONE = 0,
                ANALOG_HD_TYPE_TVI = 1
            }

            [MarshalAs(UnmanagedType.U1)]
            public bool standalone;
            [MarshalAs(UnmanagedType.U1)]
            public bool hybrid;
            public byte nvr_type;
            [MarshalAs(UnmanagedType.U1)]
            public bool analog_HD;
            public byte analog_HD_type;
        }

        public uint type;
        public uint model;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public uint[] model_line;
        public G2STRING_64 name;
        public G2STRING_64 version_hw;
        public G2STRING_64 version_sw;
        public G2STRING_64 version_build;
        public BASIC_INFO basic_info;
        public G2_PRODUCT_INFO_CAPS.IP_CAMERA ip_camera;
        public G2_PRODUCT_INFO_CAPS.REMOTE_ACCESSABLE remote_accessable;
        public G2_PRODUCT_INFO_CAPS.REMOTE_ACCESS remote_access;
        public G2_PRODUCT_INFO_CAPS.REMOTE_STATUS remote_status;
        public G2_PRODUCT_INFO_CAPS.REMOTE_WATCH remote_watch;
        public G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH remote_search;
        public G2_PRODUCT_INFO_CAPS.REMOTE_CLIP_COPY remote_clip_copy;
        public G2_PRODUCT_INFO_CAPS.RECORD_DB record_db;
        public G2_PRODUCT_INFO_CAPS.DEVICE device;
        public G2_PRODUCT_INFO_CAPS.DEVICE_HYBRID device_hybird;
        public G2_PRODUCT_INFO_CAPS.TEXT_IN_SEARCH text_in_search;
        public G2_PRODUCT_INFO_CAPS.CODEC_INFO_VIDEO codec_video;
        public G2_PRODUCT_INFO_CAPS.CODEC_INFO_AUDIO codec_audio;

        public bool is_DIRECT_IP { get { return basic_info.nvr_type == (byte)BASIC_INFO.NVR_TYPE.DIRECT_IP; } }
        public bool is_NVR { get { return basic_info.nvr_type != (byte)BASIC_INFO.NVR_TYPE.NONE; } }
        public bool is_series_HANA { get { return Array.IndexOf<uint>(model_line, 280000) > 0; }}
        public bool is_port_unity { get { return (remote_accessable.port_type == (byte)G2_PRODUCT_INFO_CAPS.REMOTE_ACCESSABLE.PORT_TYPE.UNITY); } }
        public bool is_search_version_G2 { get { return (remote_search.version == (uint)G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.VERSION.G2); } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2_PRODUCT_COMMON
    {
        public enum G2_IMAGE_QUALITY
        {
            LOW = 0,
            NORMAL = 1,
            HIGH = 2,
            VERY_HIGH = 3,
        }
        public enum G2_IMAGE_RESOLUTION
        {
            G2_IMAGE_RESOLUTION_CIF    = 0, // 352x288(PAL), 352x240(NTSC) => Legacy : RESOLUTION_STANDARD
            G2_IMAGE_RESOLUTION_2CIF   = 1, // 702x288(PAL), 704x240(NTSC) => Legacy : RESOLUTION_HIGH
            G2_IMAGE_RESOLUTION_4CIF   = 2, // 704x576(PAL), 704x480(NTSC) => Legacy : RESOLUTION_VERYHIGH
            G2_IMAGE_RESOLUTION_720P   = 3, // HD
            G2_IMAGE_RESOLUTION_1080P  = 4, // Full HD
            G2_IMAGE_RESOLUTION_NHD    = 5, // 640x360
            G2_IMAGE_RESOLUTION_QVGA   = 6, // 320x240
            G2_IMAGE_RESOLUTION_VGA    = 7, // 640x480
            G2_IMAGE_RESOLUTION_3M     = 8, // 2304x1296
            G2_IMAGE_RESOLUTION_CUSTOM = 9
        }
        public enum G2_BITRATE_CONTROL_TYPE
        {
            CBR = 0,
            VBR = 1
        }
    }
}
