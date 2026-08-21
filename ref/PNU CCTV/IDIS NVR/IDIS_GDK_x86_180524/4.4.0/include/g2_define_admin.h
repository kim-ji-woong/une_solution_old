// g2_define_admin.h : header file
//

#ifndef _G2_DEFINE_ADMIN_H_
#define _G2_DEFINE_ADMIN_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2ADMIN_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_login_failed_from_dvrns = 2,
        on_login_completed = 3,
        on_failover_prepare = 4,
        on_failover_connected = 5,
        on_failover_failed = 6,
        on_notify_connectable_service = 7,
        on_receive_device_empty = 8,
        on_receive_device_append = 9,
        on_receive_device_modify = 10,
        on_receive_device_remove = 11,
        on_receive_device_remove_list = 12,
        on_receive_device_group_list = 13,
        on_receive_device_group_append = 14,
        on_receive_device_group_modify = 15,
        on_receive_device_group_remove = 16,
        on_receive_device_to_group_map = 17,
        on_receive_device_append_to_group = 18,
        on_receive_device_remove_to_group = 19,
        on_receive_device_list = 20,
        on_receive_device_list_append_to_group = 21,
        on_receive_device_list_remove_to_group = 22,
        on_receive_layout_list = 23,
        on_receive_layout_append = 24,
        on_receive_layout_modify = 25,
        on_receive_layout_remove = 26,
        on_receive_sequence_list = 27,
        on_receive_sequence_append = 28,
        on_receive_sequence_modify = 29,
        on_receive_sequence_remove = 30,
        on_receive_recording_device_status = 31,
        on_receive_service_log_load = 32,
        on_receive_service_log_load_end = 33,
        on_receive_service_log_load_fail = 34,
        on_receive_service_log_load_stop = 35,
        on_receive_service_debug_log_load = 36,
        on_receive_service_debug_log_load_end = 37,
        on_receive_service_debug_log_load_fail = 38,
        on_receive_service_debug_log_load_stop = 39,
        on_modify_device_enable = 40,
        on_receive_test_failover_end = 41,
		on_receive_device_modify_list = 42,
		on_receive_streaming_device_in_charge = 43,

        CALLBACK_COUNT = 44
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2DEVICE_ROOT {
    G2VERSION _version;
    G2GUID    _guid;
    bool      _enable;
    int       _parent_id;   // root node : 0, parent not is not zero
    G2STRING_128 _name;
    G2STRING_256 _desc;
    G2NETWORK_INFO _ni;
};

struct G2DEVICE_LEAF {
    G2VERSION _version;
    G2GUID    _guid;
    G2GUID    _guid_parent;
    G2GUID    _guid_root;
    unsigned int _number;
    unsigned int _number_root_based;
    bool      _enable;
    G2STRING_128 _name;
    G2STRING_256 _desc;
};

struct G2DEVICE_GROUP {
    G2GUID  _guid;
    G2GUID  _guid_parent;
    G2STRING_128 _name;
    G2STRING_256 _desc;
};

struct G2GROUP_MEMBER {
    G2GUID  _group;
    G2GUID  _member;
};

struct G2LAYOUT {
    enum FORMAT {
        LAYOUT_UNDEFINED = -1,
        LAYOUT_1x1      = 0,
        LAYOUT_2x2      = 1,
        LAYOUT_3x3      = 2,
        LAYOUT_4x4      = 3,
        LAYOUT_5x5      = 4,
        LAYOUT_6x6      = 5,
        LAYOUT_7x7      = 6,
        LAYOUT_8x8      = 7,
        LAYOUT_OFBASE   = 8,    // sentinel; this is not layout
        LAYOUT_5p1      = 9,
        LAYOUT_7p1      = 10,
        LAYOUT_9p1      = 11,
        LAYOUT_11p1     = 12,
        LAYOUT_12p1     = 13,
        LAYOUT_27p1     = 14,
        LAYOUT_8p2      = 15,
        LAYOUT_18p2     = 16,
        LAYOUT_4p3      = 17,
        LAYOUT_9p3      = 18,
        LAYOUT_3x2w     = 19,
        LAYOUT_4x3w     = 20,
        LAYOUT_5x4w     = 21,
        LAYOUT_6x5w     = 22,
        LAYOUT_2p1w_v2  = 57,
        LAYOUT_2p1w     = 23,
        LAYOUT_3p1w     = 24,
        LAYOUT_4p1w     = 25,
        LAYOUT_5p1w     = 26,
        LAYOUT_6p1w     = 27,
        LAYOUT_8p1w     = 28,
        LAYOUT_10p1w    = 29,
        LAYOUT_4p2w     = 30,
        LAYOUT_12p2w    = 31,
        LAYOUT_12h1     = 32,
        LAYOUT_32h1     = 33,
        LAYOUT_28p4     = 34,
        LAYOUT_12p4w    = 35,
        LAYOUT_16p12    = 36,
        LAYOUT_24p12w   = 37,
        LAYOUT_7x6w     = 38,
        LAYOUT_8x7w     = 39,
        LAYOUT_8x1      = 40,
        LAYOUT_9x1      = 41,
        LAYOUT_10x1     = 42,
        LAYOUT_11x1     = 43,
        LAYOUT_12x1     = 44,
        LAYOUT_13x1     = 45,
        LAYOUT_14x1     = 46,
        LAYOUT_15x1     = 47,
        LAYOUT_16x1     = 48,
        LAYOUT_5x3w     = 49,
        LAYOUT_6x3w     = 50,
        LAYOUT_8x3w     = 51,
        LAYOUT_8x5w     = 52,
        LAYOUT_2x1      = 53,
        LAYOUT_3x1      = 54,
        LAYOUT_4x1      = 55,
        LAYOUT_32h1_v2  = 56,
        LAYOUT_COUNT    = 58,
    };
    enum SHARING_MODE {
        SHARING_PUBLIC = 0,
        SHARING_GROUP,
        SHARING_PRIVATE
    };

    G2GUID  _guid;
    G2GUID  _guid_admin_svr;
    G2STRING_128 _name;
    G2STRING_256 _desc;
    bool    _federated;
    int     _sharing;
    int     _format;
    G2GUID  _cameras[64];
    int     _spot_hot;
    bool    _spot_evt[64];
    bool    _spot_evt_map[64];
};

struct G2SEQUENCE {
    enum TYPE {
        UNKNOWN = -1,
        CAMERA = 0,
        LAYOUT
    };
    enum SHARING_MODE {
        SHARING_PUBLIC = 0,
        SHARING_GROUP,
        SHARING_PRIVATE
    };

    struct ITEM {
        G2GUID _item;
        int _duration;
    };

    G2GUID _guid;
    G2GUID _guid_admin_svr;
    G2STRING_128 _name;
    G2STRING_256 _desc;
    TYPE _type;
    bool _federated;
    int  _sharing;
    ITEM _list[64];
};

struct G2SERVICE_NETWORK_INFO {
    struct element_t {
        int     _type;  // G2NETWORK_INFO::ADDRESS_TYPE
        G2STRING_128 _address;
        G2MAC_ADDRESS _mac;
        unsigned short _port[G2NETWORK_INFO::MAX_PORT_INDEX];
    }
    _element[16];
    G2STRING_64  _user_id;
    G2STRING_128 _password;
    unsigned int _count;
};

struct G2SERVICE {
    G2GUID  _guid;
    G2STRING_128 _name;
    G2STRING_256 _desc;
    bool    _registered;
    G2SERVICE_NETWORK_INFO _ni;
    G2GUID  _adminKey;
};

struct G2SERVICE_ITEM {
    G2GUID _service;
    G2GUID _site;
};

struct G2RECORDING_CAMERA_STATUS {
    enum REC_TYPE {
        TIME_LAPSE = 0,
        EVENT,
        INSTANT,
        IDLE
    };
    enum REC_FAIL_REASON {
        VIDEO_LOSS = 0,
        DEACTIVATED,
        RECORD_FAIL,
        STORAGE_FULL,
        DISCONNECTED,
        NO_SCHEDULE,
        RECORD_SUCCESS
    };

    G2GUID _camera;
    bool   _on_recording;
    bool   _on_recording_audio;
    REC_TYPE _rec_type;
    REC_FAIL_REASON _rec_fail_reason;
    G2TIME _from;
    G2TIME _to;
};

struct G2RECORDING_DEVICE_STATUS {
    G2GUID _root;
    int    _connection;
    int    _disconnect_reason;
    G2PARAM_BUNCH _status;
};

struct G2DEVICE_NUMERIC_ID {
    unsigned int     _service;
    unsigned int     _device;
};

struct G2SERVICE_VERSION {
	G2STRING_32 _version;
	G2STRING_32 _number;
};

struct G2SERVICE_CHARGE_INFO {
	G2GUID_LIST     _appends;
	G2GUID_LIST     _removes;
	bool            _isLoadBalance;
	bool			_requestDirectConnect;
};

///////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_ADMIN_H_
