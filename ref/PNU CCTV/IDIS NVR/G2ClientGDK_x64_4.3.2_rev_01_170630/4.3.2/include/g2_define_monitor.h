// g2_define_monitor.h : header file
//

#ifndef _G2_DEFINE_MONITOR_H_
#define _G2_DEFINE_MONITOR_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2MONITOR_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_require_reconnect = 2,
        on_receive_event = 3,
        on_receive_service_log_load = 4,
        on_receive_service_log_load_end = 5,
        on_receive_service_log_load_fail = 6,
        on_receive_service_log_load_stop = 7,
        on_status_connected = 8,
        on_status_disconnected = 9,
        on_status_receive_device_status = 10,
        on_status_receive_device_health = 11,
        on_status_receive_event = 12,
        on_receive_debug_log_load = 13,
        on_receive_debug_log_load_end = 14,
        on_receive_debug_log_load_fail = 15,
        on_receive_debug_log_load_stop = 16,

        CALLBACK_COUNT = 17
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2MONITORING_DEVICE_STATUS {
    enum CONST_DATA {
        NOT_SUPPORT = 100,
        NOT_VALUE = 101,
        NUM_UNITS_MAX = 64,
    };
    enum CONNECTION {
        SESSION_STARTING = 0,
        SESSION_DISCONNECTED = 1,
        SESSION_CONNECTING,
        SESSION_CONNECTED,
        SESSION_DISCONNECTING
    };
    enum ONOFF {
        OFF = 0,
        ON
    };
    enum CHECK_SYSTEM {
        ABNORMAL = 0,
        NORMAL = 1,
        NOT_USE_SYSTEM = 2
    };
    enum CHECK_EVENT {
        NO_EVENT = 0,
        EVENT = 1,
        NOT_USE_EVENT = 2
    };
    enum RECORD_MODE {
        RECORD_MODE_ALL             = 0xFFFFFFFF,
        RECORD_MODE_NONE            = 0x00000000,
        RECORD_MODE_RECORDING       = 0x00000001,
        RECORD_MODE_PLAYBACK        = 0x00000002,
        RECORD_MODE_ARCHIVE         = 0x00000004,
        RECORD_MODE_CLIPCOPY        = 0x00000008,
        RECORD_MODE_PANIC_RECORDING = 0x00000010,
    };

    struct RECORD_TYPE {
        struct ON_TYPE {
            unsigned char _record;
            unsigned char _panic;
            unsigned char _archive;
            unsigned char _clip;
            unsigned char _play;
        };

        unsigned int _mode;
        G2TIME  _begin;
        G2TIME  _end;
        ON_TYPE _on;
    };
    struct HEALTH_TYPE {
        __int64 _record;
        unsigned char _cameras[NUM_UNITS_MAX];
        unsigned char _cameras_record[NUM_UNITS_MAX];
        unsigned char _sensors[NUM_UNITS_MAX];
        unsigned char _textins[NUM_UNITS_MAX];
        unsigned char _fans[NUM_UNITS_MAX];
    };

    G2GUID        _site;
    G2GUID        _admin;   // for federation
    G2STRING_64   _version;
    int           _connection;
    unsigned int  _reason_disconnect;
    int           _authority;
    bool          _support_remote_panic_recording;
    bool          _valid;
    G2MAC_ADDRESS _mac;
    RECORD_TYPE   _record;
    HEALTH_TYPE   _health;
    unsigned char _motion[NUM_UNITS_MAX];
    unsigned char _alarm_in[NUM_UNITS_MAX];
    unsigned char _alarm_out[NUM_UNITS_MAX];
    unsigned char _alarm_in_network[NUM_UNITS_MAX];
    unsigned char _text_in[NUM_UNITS_MAX];
    unsigned char _object[NUM_UNITS_MAX];
    unsigned char _video_blind[NUM_UNITS_MAX];
    unsigned char _video_analysis[NUM_UNITS_MAX];
    unsigned char _audio_detect[NUM_UNITS_MAX];
    unsigned char _trip_zone[NUM_UNITS_MAX];
    unsigned char _tamper[NUM_UNITS_MAX];
    unsigned char _face_detect[NUM_UNITS_MAX];
    unsigned char _instant_record[NUM_UNITS_MAX];
};

struct G2MONITORING_DEVICE_HEALTH {
    G2GUID        _site;
    G2GUID        _admin;
    G2STRING_64   _version;
    int           _connection;
    unsigned int  _reason_disconnect;
    bool          _authority;
    bool          _valid;
    G2MAC_ADDRESS _mac;
    G2MONITORING_DEVICE_STATUS::RECORD_TYPE _record;
    G2MONITORING_DEVICE_STATUS::HEALTH_TYPE _health;
};

struct G2ACTION {
    int           _type;
    G2GUID        _guid;
    G2STRING_64   _name;
    G2STRING_256  _desc;
};

struct G2ACTION_ACK {
    int           _type;
    __int64       _event_key;
    G2SPOT        _ack_spot;
    G2GUID        _sender;
    int           _receiver_type;
    G2GUID_128    _receivers;
    G2STRING_256  _message;
    G2STRING_64   _extra_info;
    G2STRING_64   _extra_address;
};

struct G2MONITORING_EVENT_ACTION {
    G2EVENT_INFO  _event;
    G2ACTION      _action;
    G2ACTION_ACK   _ack;
    G2GUID        _service;
};

struct G2MONITORING_USER_ALARM_IN_REQUEST {
    G2STRING_32   _sequence_id;
    G2STRING_256  _data;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_MONITOR_H_
