// g2_define_search_g2.h : header file
//

#ifndef _G2_DEFINE_SEARCH_G2_H_
#define _G2_DEFINE_SEARCH_G2_H_

#include "g2_define_search.h"
#include "g2_define_product.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2SEARCH_G2_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_query_options_search_base = 2,
        on_query_options_player = 3,
        on_receive_record_time_info_load = 4,
        on_receive_record_time_info_load_end = 5,
        on_receive_frame_data = 6,
        on_receive_text_in = 7,
        on_receive_event = 8,
        on_receive_notify_command_begin = 9,
        on_receive_notify_command_end = 10,
        on_receive_notify_play_speed_changed = 11,
        on_receive_notify_frame_not_found = 12,
        on_receive_notify_out_of_scope = 13,
        on_receive_notify_get_rollback_info = 14,
        on_receive_notify_player_error = 15,
        on_receive_event_log_load_end = 16,
        on_receive_event_log_load_stop = 17,
        on_receive_text_in_log_load_end = 18,
        on_receive_text_in_log_load_stop = 19,
        on_receive_scope_list = 20,
        on_receive_spot_list = 21,
        on_receive_no_recorded_data = 22,
        on_receive_db_info = 23,
        on_receive_db_info_external = 24,
        on_receive_db_selected = 25,
        on_receive_virtual_channelmap = 26,
        on_require_prepare_rollback = 27,
        on_probe_session_profile = 28,

        on_sole_connected = 29,
        on_sole_disconnected = 30,
        on_sole_query_options_player = 31,
        on_sole_receive_record_time_info_load = 32,
        on_sole_receive_record_time_info_load_end = 33,
        on_sole_receive_frame_data = 34,
        on_sole_receive_text_in = 35,
        on_sole_receive_event = 36,
        on_sole_receive_notify_command_begin = 37,
        on_sole_receive_notify_command_end = 38,
        on_sole_receive_notify_play_speed_changed = 39,
        on_sole_receive_notify_frame_not_found = 40,
        on_sole_receive_notify_out_of_scope = 41,
        on_sole_receive_notify_get_rollback_info = 42,
        on_sole_receive_notify_player_error = 43,
        on_sole_receive_scope_list = 44,
        on_sole_receive_spot_list = 45,
        on_sole_receive_no_recorded_data = 46,
        on_sole_require_prepare_rollback = 47,

        on_saver_connected = 48,
        on_saver_disconnected = 49,
        on_saver_receive_frame_data = 50,
        on_saver_receive_notify_out_of_scope = 51,
        on_saver_receive_notify_get_rollback_info = 52,
        on_saver_receive_notify_player_error = 53,
        on_saver_receive_scope_list = 54,
        on_saver_receive_no_recorded_data = 55,
        on_saver_receive_clipcopy_size = 56,
        on_saver_receive_clipcopy_data = 57,
        on_saver_receive_clipcopy_set_password = 58,
        on_saver_receive_clipcopy_canceled = 59,
        on_saver_receive_clipcopy_job_started = 60,
        on_saver_receive_clipcopy_job_finished = 61,
        on_saver_receive_clipcopy_section_begin = 62,
        on_saver_receive_clipcopy_section_end = 63,

        CALLBACK_COUNT = 64
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2SEARCH_G2_OPTIONS_SEARCH_BASE {
    bool _segment_less;
    bool _playable_sole;
};

struct G2SEARCH_G2_OPTIONS_PLAYER
{
    bool _play_fast_all_frames;
    bool _load_event_on_player;
    bool _load_text_in_on_player;
    bool _move_to_load_all_channels;
    bool _move_to_load_all_channels_parallel;
    int  _precision_tick_tolerance; // milliseconds (default 0)
    int  _precision_event_interval; // seconds (default 2)
	int  _key_lookup_min_threshold_for_move_to; // seconds
    int  _key_lookup_max_threshold_for_move_to; // seconds
    int  _key_lookup_threshold_for_backward_play; // seconds
};

struct G2SEARCH_G2_QUERY {
    enum MODE {
        UNDEFINED = -1,
        EVENT = 0,
        TEXT_IN
    };
};

struct G2SEARCH_G2_REMOTE_DB {
    enum STORAGE {
        NOT_USING = 0,
        RECORD,
        ARCHIVE,
        CLIP_COPY,
        EXTERNAL
    };
    enum DB_SELECT_RESULT {
        DB_SELECTED = 0,
        DB_NOT_EXIST
    };

    struct DB_INFO {
        unsigned int _id;
        unsigned int _type;
    };

    DB_INFO _info[8];
    unsigned int _info_len;
    unsigned int _current_id;
};

struct G2SEARCH_G2_SCOPE_LIST {
    G2SCOPE_LIST _list;
    int _type;
};

struct G2SEARCH_G2_EVENT_SEARCH_OPTIONS {
    enum DIRECTION {
        BACKWARD = -1,
        FORWARD = 1
    };

    int             _unit_count;
    int             _direction;
    bool            _union_result;
    G2SCOPE         _scope;
    G2CHANNEL_SET   _motion;
    G2CHANNEL_SET   _object;
    G2CHANNEL_SET   _video_loss;
    G2CHANNEL_SET   _video_blind;
    G2CHANNEL_SET   _video_analytics;
    G2CHANNEL_SET   _trip_zone;
    G2CHANNEL_SET   _tamper;
    G2CHANNEL_SET   _face_detect;
    G2CHANNEL_SET   _instant_record;
    G2CHANNEL_SET   _camera_record_bad;
    G2CHANNEL_SET   _camera_fan_error;
    G2CHANNEL_SET   _record;    // associated record camera channels
    G2CHANNEL_SET   _alarm_in;
    G2CHANNEL_SET   _alarm_in_network;
    G2CHANNEL_SET   _audio_in;
    G2CHANNEL_SET   _text_in;
    G2CHANNEL_SET   _car_event;
    G2CHANNEL_SET   _system_events;
    G2CHANNEL_SET   _spc_motion_xor_trip_zone;
};

struct G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION {
    struct EXPRESSION {
        enum {
            COMPARE_NONE = 0,
            COMPARE_LESS,
            COMPARE_LESS_EQUAL,
            COMPARE_EQUAL,
            COMPARE_EQUAL_MORE,
            COMPARE_MORE
        };

        G2STRING_128 _name;
        G2STRING_128 _value;
        int _op;
        int _value_column;
        int _value_line;
    };

    bool _combi_and;
    EXPRESSION _exp;
};

struct G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS {
    int             _unit_count;
    int             _system_id;
    G2SCOPE         _scope;
    G2CHANNEL_SET   _channels;
    G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION _condition[8];
    int             _condition_count;
    bool            _transaction_wise;
    bool            _case_sensitive;
    bool            _match_whole_word;
};

//////////////////////////////////////////////////////////////////////////

struct G2SEARCH_G2_PARAM_CLIPCOPY_SIZE_INFO {
    G2CLIPCOPY_SIZE_INFO _info;
    int _status;
    unsigned int _progress;
};

struct G2SEARCH_G2_SCOPE_TYPE {
    enum TYPE {
        UNDEFINED = 0,
        CLIP_COPY = 1,
        GOTO
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2SEARCH_G2_SOLE_CHANNEL_MAP {
    enum {
        MAX_CHANNEL_COUNT = 256
    };
    int _first[MAX_CHANNEL_COUNT];
    int _second[MAX_CHANNEL_COUNT];
    unsigned int _len;
};

struct G2SEARCH_G2_SOLE_CHANNEL_TAG {
    int _channel;
    int _tag;
};

struct G2SEARCH_G2_SOLE_CHANNEL_TAG_ID {
    int _channel;
    int _tag;
    __int64 _id;
};

struct G2SEARCH_G2_SOLE_SNAPSHOP_LOADER {
    enum FINISH_REASON {
        COMPLETED,
        CANCELED,
        ERROR_OCCURRED,
        NO_IMAGE,
    };
};

struct G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION {
    __int64 _id;
    G2CHANNEL_SET _channels;
    G2SPOT _spot;
    int _precision;
};

struct G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION_LIST {
    const G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION* _options;
    unsigned int _len;
};

struct G2SEARCH_G2_SOLE_RECORDED_SCOPE {
    G2CHANNEL_SET _channels;
    G2SCOPE _scope;
};

struct G2SEARCH_G2_SOLE_PROBE_FRAMELOAD {
    int _ips;
    int _bps;
};

struct G2SEARCH_G2_SOLE_BASE_OPTIONS {
    unsigned int _use_player_count;
    unsigned int _use_rec_time_loader_count;
    unsigned int _use_text_in_loader_count;
    unsigned int _use_text_search_count;
    unsigned int _use_clip_copy_count;
    unsigned int _use_event_log_loader_count;
    unsigned int _use_segment_finder_count;
    unsigned int _use_channel_finder_count;
    unsigned int _use_snapshot_loader_count;
    unsigned int _use_gps_data_loader_count;
    unsigned int _use_gps_data_measurer_count;
    unsigned int _use_gps_data_exporter_count;
    unsigned int _use_gps_location_loader_count;
    unsigned int _use_backup_data_loader_count;
    unsigned int _use_scope_loader_count;
    bool _force_segment_less_search;
};

struct G2SEARCH_G2_SOLE_PLAYER_OPTIONS {
    bool _load_all_frames_for_next_step;
    bool _load_all_frames_for_play;
    bool _load_all_channels_for_move_to;
    bool _load_all_channels_in_parallel_for_move_to;
    bool _wait_move_before_play;
    bool _reload_last;
    unsigned int _tick_precision_tolerance; // in milliseconds
    unsigned int _event_precision_interval; // in seconds
    unsigned int _fast_skip_interval;
    unsigned int _faster_skip_interval;
    unsigned int _fastest_skip_interval;
    unsigned int _text_in_load_mask;
    unsigned int _event_log_load_mask;
    unsigned int _inex_event_log_load_mask;
    unsigned int _gps_data_load_mask;
    unsigned int _text_in_load_tolerance;    // in seconds
    unsigned int _event_log_load_tolerance;
    unsigned int _inex_event_log_load_tolerance;
    unsigned int _gps_data_load_tolerance;
    bool _segment_less_search;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_SEARCH_G2_H_
