// g2_define_play.h : header file
//

#ifndef _G2_DEFINE_PLAY_H_
#define _G2_DEFINE_PLAY_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_search_g2.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2PLAY_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_query_options_search_base = 2,
        on_receive_hard_disk_info = 3,
        on_receive_record_channels = 4,
        on_receive_record_time_info_load = 5,
        on_receive_record_time_info_load_end = 6,
        on_receive_frame_data = 7,
        on_receive_text_in = 8,
        on_receive_notify_command_begin = 9,
        on_receive_notify_command_end = 10,
        on_receive_notify_play_speed_changed = 11,
        on_receive_notify_frame_not_found = 12,
        on_receive_notify_out_of_scope = 13,
        on_receive_notify_player_error = 14,
        on_receive_scope_list = 15,
        on_receive_spot_list = 16,
        on_receive_no_recorded_data = 17,
        on_receive_event_log_load = 18,
        on_receive_event_log_load_end = 19,
        on_receive_event_log_load_fail = 20,
        on_receive_event_log_load_stop = 21,
        on_receive_text_in_log_load = 22,
        on_receive_text_in_log_load_end = 23,
        on_receive_text_in_log_load_fail = 24,
        on_receive_text_in_log_load_stop = 25,
        on_receive_device_log_load = 26,
        on_receive_device_log_load_end = 27,
        on_receive_device_log_load_fail = 28,
        on_receive_device_log_load_stop = 29,
        on_receive_service_log_load = 30,
        on_receive_service_log_load_end = 31,
        on_receive_service_log_load_fail = 32,
        on_receive_service_log_load_stop = 33,
        on_require_prepare_rollback = 34,
        on_probe_session_profile = 35,

        on_receive_debug_log_load = 36,
        on_receive_debug_log_load_end = 37,
        on_receive_debug_log_load_fail = 38,
        on_receive_debug_log_load_stop = 39,

        on_receive_record_failover_scope = 40,

        CALLBACK_COUNT = 41
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2PLAY_OPTIONS_SEARCH_BASE {
    bool _segment_less;
    int  _sole_player_count;
};

struct G2PLAY_USAGE {
    enum TYPE {
        PLAYBACK = 0,
        LOG
    };
};

struct G2PLAY_CHANNEL_INFO {
    G2GUID  _guid;
    int     _channelext;
};

struct G2PLAY_SCOPE_TYPE {
    enum TYPE {
        UNDEFINED = 0,
        CLIP_COPY = 1,
        GOTO
    };
};

struct G2PLAY_SCOPE_LIST {
    G2SCOPE_LIST _list;
    int _type;
};

struct G2PLAY_CLIPCOPY_SIZE {
    int     _status;
    unsigned int _size;
    G2SCOPE _scope;
};

struct G2PLAY_CLIPCOPY_STATUS {
    enum STATUS {
        READY          = 0,
        MEASURE_SIZE   = 1,
        FORMAT_STORAGE = 3,
        COPY_DATA      = 4,
        COPY_PLAYER    = 5,
        FINALIZE       = 6
    };
};

struct G2PLAY_HARD_SMART_INFO {
    G2STRING_64 _name;
    G2STRING_64 _serial;
    G2STRING_64 _firmware;
    int  _drive_num;
    int  _status;
    int  _temperature;
    bool _enabled_smart;
};

struct G2PLAY_HARD_DISK_INFO {
    enum FILE_SYSTEM_TYPE {
        FILE_SYSTEM_NATIVE,
        FILE_SYSTEM_GENERAL,
        FILE_SYSTEM_RAW
    };
    enum DISK_TYPE {
        DISK_TYPE_UNKNOWN = 0x00,
        DISK_TYPE_SCSI,
        DISK_TYPE_ATAPI,
        DISK_TYPE_ATA,
        DISK_TYPE_1394,
        DISK_TYPE_SSA,
        DISK_TYPE_FIBER,
        DISK_TYPE_USB,
        DISK_TYPE_RAID,
        DISK_TYPE_ISCSI,
        DISK_TYPE_SAS,
        DISK_TYPE_SATA,
        DISK_TYPE_SD,
        DISK_TYPE_MMC,
        DISK_TYPE_VIRTUAL,
        DISK_TYPE_FILEBACKEDVIRTUAL,
        DISK_TYPE_MAX,
        DISK_TYPE_NETWORK = 0x7D,
        DISK_TYPE_ESATA = 0x7E,
        MAX_RESERVED = 0x7F
    };

    G2STRING_256 _path;                         // Recording service storage main path("C:", "\\.\PHISICALDRIVE1" ..)
    G2STRING_256 _path_storage_sub;             // Recording service storage sub path("\NexusSrorage\{XXX...}" ...)
    G2STRING_64 _volume_label;                  // do not use
    G2STRING_64 _serial;                        // disk serial number
    DISK_TYPE _disk_type;                       // enum DISK_TYPE
    FILE_SYSTEM_TYPE _fs;                       // enum FILE_SYSTEM_TYPE
    G2SCOPE _frame_scope;                       // Recording service storage recording scope
    bool _enabled;                              // whether to use the Recording service storage
    bool _system_volume;                        // system volume(whether or not the windows is installed on a volume)
    bool _removable;                            // do not use
    unsigned __int64 _capacity;                 // Total capacity of the disk (byte)
    unsigned __int64 _free_space;               // Available disk space
    unsigned __int64 _storage_capacity;         // Recording service storage total capacity (byte)
    unsigned __int64 _storage_free_space;       // Recording service storage available disk space
    unsigned int _bank_count;                   // Recording service storage bank count
    unsigned int _bank_count_max;               // do not use
    unsigned int _bank_size;                    // do not use
};

struct G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG {
    G2GUID_LIST _source;
    G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS _options;
};

struct G2FAILOVER_SCOPE_INFO_LIST {
    const G2FAILOVER_SCOPE_INFO* _infos;
    unsigned int _len;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_PLAY_H_
