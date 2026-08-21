// g2_define_product.h : header file
//

#ifndef _G2_DEFINE_PRODUCT_H_
#define _G2_DEFINE_PRODUCT_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2_PRODUCT_INFO_CAPS {
    struct IP_CAMERA {
        enum IP_MODE_TYPE {
            IP_MODE_NOT_CHANGEABLE = 0,
            IP_MODE_CHANGABLE_OPEN_DIRECT = 1
        };
        enum DEWARPING_TYPE {
            DEWARPING_NOT_SUPPORT = 0,
            DEWARPING_1_0_0 = 1
        };

        bool is;
        bool auto_iris;
        unsigned char ip_mode;
        unsigned char dewarping;
    };

    struct REMOTE_ACCESSABLE {
        enum PUSH_NOTIFICATION_TYPE {
            TYPE_NONE = 0,
            TYPE_CALLBACK = 1
        };
        enum FEN_SERVICE_VERSION {
            VERSION_NONE  = 0,
            VERSION_1_0_0 = 1
        };
        enum PORT_TYPE {
            PORT_TYPE_SEPERATE = 0,
            PORT_TYPE_UNITY = 1
        };

        bool encrypt_password;
        bool support_RTP;
        bool support_RTSP;
        bool support_status;
        bool support_watch;
        bool support_search;
        bool support_audio;
        bool support_record;
        bool support_dvrns;
        bool support_ddns;
        bool support_onvif;
        bool support_vnc;
        unsigned char support_push;
        unsigned char support_fen;
        unsigned char port_type;
    };

    struct REMOTE_ACCESS {
        unsigned char count_status;
        unsigned char count_watch;
        unsigned char count_search;
        unsigned char count_record;
        unsigned char count_audio_in;
        unsigned char count_audio_out;
        unsigned int  authorities;
    };

    struct REMOTE_STATUS {
        enum BEEP_CONTROL_TYPE {
            BEEP_CONTROL_NONE = 0,
            BEEP_CONTROL_1_0_0 = 1
        };
        enum INSTANT_RECORDING_TYPE {
            INSTANT_RECORDING_TYPE_NOT_SUPPORT = 0,
            INSTANT_RECORDING_TYPE_1_0_0
        };

        bool remote_panic_recording;
        bool log_event;
        bool log_debug;
        unsigned char beep_control;
        unsigned char instant_recording;
    };

    struct REMOTE_WATCH {
        enum TICK_TYPE {
            TICK_TYPE_TICK        = 0,
            TICK_TYPE_SYSTEM_MSEC = 1,
            TICK_TYPE_LEGACY_MSEC = 2,
            TICK_TYPE_SEC         = 1
        };
        enum COMMAND_TYPE {
            COMMAND_CONTROL_COLOR = 0,
            COMMAND_CONTROL_COLOR_RELAY,
            COMMAND_CONTROL_PTZ,
            COMMAND_CONTROL_PTZ_RELATIVE,
            COMMAND_RELAY
        };

        unsigned char tick_type;
        unsigned char stream_count;
        bool transfer_multi_stream;
        bool motion_disp;
        bool audio_sync;
        bool xframe;
        unsigned short command_types[32];
        unsigned int command_types_len;
    };

    struct REMOTE_SEARCH {
        enum SEARCH_VERSION {
            VEERION_UNKNOWN    = 0,
            VERSION_LEGACY     = 1,
            VERSION_SAMBA_LIKE = 2,
            VERSION_SAMBA      = 3,
            VERSION_G2         = 4
        };
        enum TICK_TYPE {
            TICK_TYPE_TICK        = 0,
            TICK_TYPE_SYSTEM_MSEC = 1,
            TICK_TYPE_LEGACY_MSEC = 2
        };
        enum REC_INFO_TYPE {
            REC_INFO_TYPE_UNKNOWN    = 0,
            REC_INFO_TYPE_HOUR       = 1,
            REC_INFO_TYPE_MINUTE     = 2,
            REC_INFO_TYPE_IDR_MINUTE = 3,
            REC_INFO_TYPE_SECOND     = 4
        };
        unsigned int  version;
        unsigned int  tick_type;
        unsigned int  rec_info_type;
        unsigned char stream_count;
        unsigned char base_channel;
        bool archive;
        bool external;
        bool audio_play;
        int  events[256];
        unsigned int events_len;
    };

    struct REMOTE_CLIP_COPY {
        enum VERSION {
            VERSION_UNKNOWN   = 0,
            VERSION_MINIBANK  = 1,
            VERSION_CLIP_COPY = 2
        };
        enum INCLUDE_DATA {
            INCLUDE_TEXT_IN = 1,
            INCLUDE_GPS     = 2
        };
        enum SLICE_LARGE_FILE_SUPPORT {
            SLICE_LARGE_FILE_SUPPORT_NONE  = 0,
            SLICE_LARGE_FILE_SUPPORT_1_0_0 = 1
        };

        unsigned int version;
        bool password_save;
        bool password_encrypt;
        bool available_channels;
        bool player_exclude;
        bool player_exist;
        bool partial_clip;
        unsigned int include_data;
        unsigned char support_slice_large_file;
    };

    struct RECORD_DB {
        enum TYPE {
            NOT_SUPPORT = 0,
            IBANK_1_0 = 1,
            IBANK_2_0 = 2,
            IBANK_3_0 = 3
        };

        unsigned char type;
    };

    struct DEVICE {
        unsigned char count_camera;
        unsigned char count_audio_in;
        unsigned char count_audio_out;
        unsigned char count_alarm_in;
        unsigned char count_alarm_out;
        unsigned char count_text_in;
        unsigned char count_alarm_in_network;
        unsigned char count_alarm_out_network;
    };

    struct DEVICE_HYBRID {
        unsigned char count_camera;
        unsigned char count_alarm_in;
        unsigned char count_alarm_out;
        unsigned char count_audio_in;
        unsigned char count_audio_out;
        unsigned char count_network_camera;
        unsigned char count_network_alarm_in;
        unsigned char count_network_alarm_out;
        unsigned char count_network_audio_in;
        unsigned char count_network_audio_out;
    };

    struct TEXT_IN_SEARCH {
        enum VERSION {
            NONE = 0,
            IDR = 1,
            ADVANCED = 2
        };

        unsigned char version;
        bool match_whole_word;
        bool transaction_wise;
    };

    struct CODEC_INFO_VIDEO {
        enum TYPE {
            UNKNOWN = 0,
            MJPEG = 1,
            MLJPEG = 2,
            MPEG4 = 3,
            H264 = 4,
            BITMAP = 5,
            MXPEG = 6,
            HEVC = 7,
            UNRESTRICTED = 255
        };

        unsigned char codecs[32];
    };

    struct CODEC_INFO_AUDIO {
        enum TYPE {
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
        };

        unsigned char codecs[32];
    };
};

struct G2_PRODUCT_INFO {
    enum TYPE {
        TYPE_UNKNOWN           = 0,
        TYPE_REMOTE_ACCESSABLE = 1,
        TYPE_VIDEO_SERVER      = 2,
        TYPE_VIDEO_DECODER     = 3,
        TYPE_KEYBOARD          = 4,
        TYPE_MMX               = 5,
        TYPE_SWITCH            = 7,
        TYPE_MOBILE_DEVICE     = 8,
        TYPE_INEX_SERVICE      = 10000,
        TYPE_RAS               = 20000,
        TYPE_CAMERA            = 100000,
        TYPE_AUDIO_IN          = 100001,
        TYPE_AUDIO_OUT         = 100002,
        TYPE_ALARM_IN          = 100003,
        TYPE_ALARM_OUT         = 100004,
        TYPE_TEXT_IN           = 100005
    };

    struct BASIC_INFO {
        enum NVR_TYPE {
            NVR_TYPE_NONE = 0,
            NVR_TYPE_NORMAL = 1,
            NVR_TYPE_DIRECT_IP = 2
        };
        enum ANALOG_HD_TYPE {
            ANALOG_HD_TYPE_NONE = 0,
            ANALOG_HD_TYPE_TVI = 1
        };

        bool standalone;
        bool hybrid;
        unsigned char nvr_type;
        bool analog_HD;
        unsigned char analog_HD_type;
    };

    unsigned int type;
    unsigned int model;
    unsigned int model_line[8];
    G2STRING_64  name;
    G2STRING_64  version_hw;
    G2STRING_64  version_sw;
    G2STRING_64  version_build;
    BASIC_INFO   basic_info;
    G2_PRODUCT_INFO_CAPS::IP_CAMERA ip_camera;
    G2_PRODUCT_INFO_CAPS::REMOTE_ACCESSABLE remote_accessable;
    G2_PRODUCT_INFO_CAPS::REMOTE_ACCESS     remote_access;
    G2_PRODUCT_INFO_CAPS::REMOTE_STATUS     remote_status;
    G2_PRODUCT_INFO_CAPS::REMOTE_WATCH      remote_watch;
    G2_PRODUCT_INFO_CAPS::REMOTE_SEARCH     remote_search;
    G2_PRODUCT_INFO_CAPS::REMOTE_CLIP_COPY  remote_clip_copy;
    G2_PRODUCT_INFO_CAPS::RECORD_DB         record_db;
    G2_PRODUCT_INFO_CAPS::DEVICE            device;
    G2_PRODUCT_INFO_CAPS::DEVICE_HYBRID     device_hybird;
    G2_PRODUCT_INFO_CAPS::TEXT_IN_SEARCH    text_in_search;
    G2_PRODUCT_INFO_CAPS::CODEC_INFO_VIDEO  codec_video;
    G2_PRODUCT_INFO_CAPS::CODEC_INFO_AUDIO  codec_audio;
};

//////////////////////////////////////////////////////////////////////////

struct G2_PRODUCT_COMMON {
    enum G2_IMAGE_QUALITY {
        G2_IMAGE_QUALITY_LOW        = 0,
        G2_IMAGE_QUALITY_NORMAL     = 1,
        G2_IMAGE_QUALITY_HIGH       = 2,
        G2_IMAGE_QUALITY_VERY_HIGH  = 3,
    };
    enum G2_IMAGE_RESOLUTION {
        G2_IMAGE_RESOLUTION_CIF     = 0,    // 352x288(PAL), 352x240(NTSC) => Legacy : RESOLUTION_STANDARD
        G2_IMAGE_RESOLUTION_2CIF    = 1,    // 702x288(PAL), 704x240(NTSC) => Legacy : RESOLUTION_HIGH
        G2_IMAGE_RESOLUTION_4CIF    = 2,    // 704x576(PAL), 704x480(NTSC) => Legacy : RESOLUTION_VERYHIGH
        G2_IMAGE_RESOLUTION_720P    = 3,    // HD
        G2_IMAGE_RESOLUTION_1080P   = 4,    // Full HD
        G2_IMAGE_RESOLUTION_NHD     = 5,    // 640x360
        G2_IMAGE_RESOLUTION_QVGA    = 6,    // 320x240
        G2_IMAGE_RESOLUTION_VGA     = 7,    // 640x480
        G2_IMAGE_RESOLUTION_3M      = 8,    // 2304x1296
        G2_IMAGE_RESOLUTION_CUSTOM  = 9
    };
    enum G2_BITRATE_CONTROL_TYPE {
        G2_BITRATE_CONTROL_TYPE_CBR = 0,
        G2_BITRATE_CONTROL_TYPE_VBR = 1,
    };
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_PRODUCT_H_
