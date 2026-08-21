// g2_define_search.h : header file
//

#ifndef _G2_DEFINE_SEARCH_H_
#define _G2_DEFINE_SEARCH_H_

#include "g2_define.h"
#include "g2_define_product.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2SEARCH_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_receive_recorded_date = 2,
        on_receive_recorded_time_hour = 3,
        on_receive_recorded_time_minute = 4,
        on_receive_recorded_rechour_minute = 5,
        on_receive_frame_data = 6,
        on_receive_no_frame = 7,
        on_receive_no_recorded_data = 8,
        on_receive_no_recorded_data_from_search_target = 9,
        on_receive_find_idr_event_time = 10,
        on_receive_notify_play_speed_changed = 11,
        on_receive_notify_play_stop_post = 12,
        on_receive_notify_end_of_play = 13,
        on_receive_notify_segment_changed = 14,
        on_receive_notify_search_mode_changed = 15,
        on_receive_notify_command_end = 16,
        on_receive_query_result_event = 17,
        on_receive_query_result_text_in = 18,
        on_receive_recorded_date_scope = 19,
        on_receive_segment_spot = 20,
        on_receive_error = 21,
        on_receive_text_in = 22,
        on_receive_external_tango_info = 23,
        on_receive_gps_data_start = 24,
        on_receive_gps_data = 25,
        on_receive_gps_data_list = 26,
        on_receive_gps_data_end = 27,
        on_receive_gps_data_end_count = 28,
        on_receive_gps_data_export_cancel_result = 29,
        on_receive_gps_data_measure_result = 30,
        on_require_prepare_playback = 31,
        on_require_prepare_load_event_image = 32,
        on_require_prepare_reload = 33,
        on_probe_session_profile = 34,

        on_saver_connected = 35,
        on_saver_disconnected = 36,
        on_saver_receive_recorded_date = 37,
        on_saver_receive_frame_data = 38,
        on_saver_receive_no_frame = 39,
        on_saver_receive_no_recorded_data = 40,
        on_saver_receive_notify_play_speed_changed = 41,
        on_saver_receive_notify_play_stop_spot = 42,
        on_saver_receive_notify_end_of_play = 43,
        on_saver_receive_notify_command_end = 44,
        on_saver_receive_clipcopy_scope = 45,
        on_saver_receive_clipcopy_measure_size = 46,
        on_saver_receive_clipcopy_size = 47,
        on_saver_receive_clipcopy_data = 48,
        on_saver_receive_clipcopy_set_password = 49,
        on_saver_receive_clipcopy_enable_channels = 50,
        on_saver_receive_clipcopy_canceled = 51,
        on_saver_receive_clipcopy_job_started = 52,
        on_saver_receive_clipcopy_job_finished = 53,
        on_saver_receive_clipcopy_section_begin = 54,
        on_saver_receive_clipcopy_section_end = 55,
        on_saver_receive_bank_space = 56,
        on_saver_receive_bank_image = 57,
        on_saver_receive_bank_audio = 58,
        on_saver_receive_bank_no_image = 59,
        on_saver_receive_bank_no_audio = 60,

        CALLBACK_COUNT = 61
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2SEARCH_PLAYBACK {
    enum COMMAND {
        STOP = 0,
        PLAY,
        REW,
        FF,
        PREV,
        NEXT,
        FIRST,
        LAST,
        GOTO_HOUR,
        GOTO_SEC,
        GOTO_MSEC,
        GOTO_SPOT = 110
    };
};

struct G2SEARCH_DRIVE {
    enum MODE {
        UNDEFINED = -1,
        G2 = 0,
        SAMBA,
        HOUR,
        IDR,
        LEGACY
    };
};

struct G2SEARCH_QUERY {
    enum MODE {
        UNDEFINED = -1,
        EVENT = 0,
        TEXT_IN
    };
};

struct G2SEARCH_MODE {
    enum TYPE {
        UNDEFINED = -1,
        TIMELAPSE = 0,
        EVENT_SEARCH
    };
};

struct G2SEARCH_TIMELAPSE {
    enum MODE {
        UNDEFINED = -1,
        MINUTE = 0,
        HOUR,
        MINUTE_IDR
    };
};

struct G2SEARCH_TARGET {
    enum TYPE {
        HDD = 0,
        ARCHIVE,
        EXTERNAL
    };
};

struct G2SEARCH_EXTERNAL_DISK {
    enum DISK {
        UNKNOWN = 0,
        IDE_HDD,
        SCSI_HDD,
        USB_HDD,
        IDE_CDRW,
        IDE_DVDRW,
        SW_RAID_DISK,
        FLASH_MEMORY,
        ESATA_HDD,
        ISCSI_HDD,
    };
    enum STORAGE {
        NOT_USING = 0,
        RECORD,
        ARCHIVE,
        CLIP_COPY
    };

    unsigned int  _disk_type;
    unsigned int  _number;
    unsigned __int64 _capacity;
    unsigned char _storage_type;
};

struct G2SEARCH_SUPPORT {
    enum QUERY {
        SAMBA_SEARCH = 0,
        BUFFER_USE_TICK,
        BUFFER_USE_SYSTEMMSEC,
        BUFFER_USE_LEGACYMSEC,
        PLAY_AUDIO,
        CLIP_PLAYER,
        CLIP_SAVE_PASSWORD,
        CLIP_AVAIL_CHANNEL,
        CLIP_INCLUDE_TEXT_IN,
        CLIP_COPY_STRUCTURE,
        MINIBANK_PLAYER,
        MINIBANK_AUDIO,
        EXTERNAL_TANGO_SEARCH,
        ARCHIVE,
        ARCHIVE_ADT,
        VIRTUAL_CHANNEL = 18,
        CLIP_INCLUDE_GPS_DATA = 21,
        CLIP_SLICE = 22
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2EVENT_QUERY_CONDITION {
    enum {
        SYSTEM_EVENT_TANGO_FULL     = 0,
        SYSTEM_EVENT_RECORDER_BAD,
        SYSTEM_EVENT_ALARM_IN_BAD,
        SYSTEM_EVENT_DISK_BAD,
        SYSTEM_EVENT_DISK_TEMPERATURE,
        SYSTEM_EVENT_DISK_SMART,
        SYSTEM_EVENT_SYSTEM_ALIVE,
        SYSTEM_EVENT_PANIC,
        SYSTEM_EVENT_TANGO_ALMOST_FULL,
        SYSTEM_EVENT_DC_FAN_ERROR,
        SYSTEM_EVENT_DISK_CONFIG_CHANGE,
        SYSTEM_EVENT_SYSTEM_BOOT_UP,
        SYSTEM_EVENT_SYSTEM_RESTART,
        SYSTEM_EVENT_SYSTEM_SHUTDOWN,
        SYSTEM_EVENT_COVER_OPEN,
        SYSTEM_EVENT_DISK_OFF,
        SYSTEM_EVENT_NO_DISK,

        MAX_SYSTEM_EVENT_COUNT
    };

    enum {
        MAX_FS_CARD_NUMBER = 18
    };

    struct DWELLTIME_TYPE {
        int _motion;
        int _object;
        int _video_loss;
        int _video_analytics;
        int _alarm_in;
    };

    int     _seq_number;
    G2TIME  _begin;
    G2TIME  _end;
    unsigned int _motion;
    unsigned int _object;
    unsigned int _video_loss;
    unsigned int _video_blind;
    unsigned int _video_analytics;
    unsigned int _trip_zone;
    unsigned int _tamper;
    unsigned int _face_detect;
    unsigned int _instant_record;
    unsigned int _record;
    unsigned int _alarm_in;
    unsigned int _alarm_in_network;
    unsigned int _audio_in;
    unsigned int _text_in;
    unsigned int _system_events;
    bool         _reload;
    DWELLTIME_TYPE _dwell;
    unsigned int _fs_loop_id;
    signed char  _fs_panic;
    signed char  _fs_card;
    signed char  _fs_card_number[MAX_FS_CARD_NUMBER + 1];
};

struct G2TEXT_IN_QUERY_CONDITION {
    enum {
        MAX_ITEM_COUNT = 5,
        MAX_NAME_VALUE_LENGTH = 79
    };
    enum CONDITION {
        COND_NONE = 0,
        COND_AND,
        COND_OR
    };
    enum COMPARATOR {
        COMP_NONE = 0,
        COMP_LESS,
        COMP_LESS_SAME,
        COMP_SAME,
        COMP_MORE_SAME,
        COMP_MORE,
    };

    struct ITEM_TYPE {
        int      _condition;
        char     _name[80];
        int      _comparator;
        char     _value[80];
        int      _column;
        int      _line;
    };

    bool         _reload;
    G2TIME       _begin;
    G2TIME       _end;
    unsigned int _channels;
    int          _data_type;
    int          _item_count;
    ITEM_TYPE    _item[5];
    bool         _case_sensitive;
    bool         _match_whole_word;
    bool         _transaction_wise;
};

struct G2QUERY_TIME_RANGE {
    bool    _day_first;
    G2TIME  _begin;
    G2TIME  _end;
};

struct G2SEARCH_LOG_INFO {
    enum {
        MAX_TRANS_NUMBER = 16,
        MAX_CAMERA = 16
    };

    int             _seq_number;
    int             _rdn;           // relative duplicated(time) number
    signed char     _version[2];
    int             _event_level1;
    int             _event_level2;
    signed char     _event_id;
    signed char     _trans_number[MAX_TRANS_NUMBER + 1];
    signed char     _level;
    G2TIME          _time;
    int             _msec;
    unsigned int    _cameras;
    unsigned short  _pre_dwell[MAX_CAMERA];
    unsigned short  _pst_dwell[MAX_CAMERA];
    G2STRING_64     _label;
};

//////////////////////////////////////////////////////////////////////////

struct G2HARD_DISK_INFO {
    enum FILE_SYSTEM {
        FS_NATIVE = 0,
        FS_GENERAL,
        FS_RAW
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

    int     _index;
    int     _disk_type;
    int     _file_system;
    bool    _enabled;
    bool    _removable;
    unsigned __int64 _capacity;
    unsigned __int64 _free_space;
    G2SCOPE  _scope;
    G2STRING_256 _path;
    G2STRING_256 _model;
    G2STRING_256 _serial;
};

//////////////////////////////////////////////////////////////////////////

struct G2SEARCH_PARAM_GPS_DATA_MEASURE_RESULT {
    int         _count;
    signed char _done;
};

struct G2SEARCH_PARAM_PREPARE_PLAYBACK {
    int     _command;
    G2SPOT  _spot;
};

struct G2SEARCH_PARAM_PREPARE_LOAD_EVENT_IMAGE {
    int     _selected;
    bool    _last;
};

struct G2SEARCH_PARAM_CLIPCOPY_SIZE {
    int     _status;
    unsigned __int64 _size;
    G2TIME  _begin;
    G2TIME  _end;
    G2CLIPCOPY_SIZE_INFO _info;
};

struct G2SEARCH_PARAM_CLIPCOPY_DATA {
    bool _completed;
    int  _progress;
    int  _offset;
    int  _size;
    const unsigned char* _data;
};

struct G2SEARCH_PARAM_BANK_SPACE {
    int     _image_index_number;
    int     _audio_index_number;
    G2TIME  _start_time;
    int     _start_msec;
    unsigned __int64 _image_size;
    unsigned __int64 _audio_size;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_SEARCH_H_
