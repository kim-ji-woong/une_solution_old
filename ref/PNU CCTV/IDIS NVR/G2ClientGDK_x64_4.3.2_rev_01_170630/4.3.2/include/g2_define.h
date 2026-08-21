// g2_define.h : header file
//

#ifndef _G2_DEFINE_H_
#define _G2_DEFINE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

#if defined(_WIN32)
#ifdef _G2_DLL_EXPORT
#define G2_DLLFUNC extern "C" __declspec(dllexport)
#else
#define G2_DLLFUNC extern "C" __declspec(dllimport)
#endif
#else
#define G2_DLLFUNC
#endif

#if defined(_WIN32)
#ifndef G2API
#define G2API __stdcall
#endif
#ifndef G2CALLBACK
#define G2CALLBACK __stdcall
#endif
#elif defined(__APPLE__) || defined(ANDROID)
#ifndef G2API
#define G2API
#endif
#ifndef G2CALLBACK
#define G2CALLBACK
#endif
#else
#ifndef G2API
#define G2API __attribute__((stdcall))
#endif
#ifndef G2CALLBACK
#define G2CALLBACK __attribute__((stdcall))
#endif
#endif

#if !defined(_WIN32)
#define __int64 long long
#endif

#if defined(_WIN32)
#define G2W64 _W64
#else
#define G2W64
#endif

typedef void* G2HANDLE_PTR;
typedef long  G2HANDLE;
typedef long  G2HBURNER;
typedef long  G2HADMIN;
typedef long  G2HLIVE;
typedef long  G2HPLAY;
typedef long  G2HPLAY_SAVER;
typedef long  G2HPLAY_SOLE;
typedef long  G2HMONITOR;
typedef long  G2HVIOLET;
typedef long  G2HWATCH;
typedef long  G2HRTP;
typedef long  G2HSEARCH;
typedef long  G2HSEARCH_G2;
typedef long  G2HSTATUS;
typedef long  G2HDEVICE_INFO;
typedef long  G2HBACKUP;
typedef long  G2HBACKUP_SAVER;
typedef long  G2HDUALAUDIO;
typedef long  G2HCALLBACK;
typedef long  G2HDECODER;
typedef void* G2HBURNER_DEVICE;
typedef long  G2HLOOKER;
typedef long  G2HDPSERVER;
typedef long  G2HDPTANGO;
typedef long  G2HPANIC;
typedef void* G2HGPSGRAPH;
typedef void* G2HGPSGMAP;

#if defined(_WIN32)
typedef HWND  G2HWND;
#else
typedef void* G2HWND;
#endif

#ifndef G2SEALED
#if defined(_WIN32)
#define G2SEALED sealed
#else
#define G2SEALED
#endif
#endif

#ifndef G2HNULL
#define G2HNULL 0
#endif

typedef unsigned int G2BOOL;

#ifndef G2TRUE
#define G2TRUE 1
#endif

#ifndef G2FALSE
#define G2FALSE 0
#endif

#if defined(_WIN64) || (__APPLE__)
    typedef unsigned __int64 G2WPARAM;
    typedef __int64  G2LPARAM;
    typedef __int64* G2UPARAM;
    typedef __int64  G2RESULT;
#else
    typedef G2W64 unsigned int G2WPARAM;
    typedef G2W64 long  G2LPARAM;
    typedef G2W64 long* G2UPARAM;
    typedef G2W64 long  G2RESULT;
#endif

typedef G2RESULT(G2CALLBACK *G2FUN_LISTENER)(G2HANDLE, G2WPARAM, G2LPARAM, G2UPARAM);
typedef G2RESULT(G2CALLBACK *G2FUN_LISTENER_PTR)(G2HANDLE_PTR, G2WPARAM, G2LPARAM, G2UPARAM);
typedef long long(G2CALLBACK *J2FUN_LISTENER)(long long, long long, long long, void*, int);
typedef bool (G2API *G2FUN_GET_ADAPTOR)(G2HANDLE, void*);

struct G2LISTENER_INFO {
    int _type;
    G2FUN_LISTENER _func;
};

struct G2PARAM_BUNCH {
    void* _bunch;
    unsigned int _len;
};

struct G2WPARAM_LIST {
    G2WPARAM* _params;
    unsigned int _len;
};

struct G2LPARAM_LIST {
    G2LPARAM* _params;
    unsigned int _len;
};

struct G2VARIANT {
    union {
        void*           PV;
        const wchar_t*  STR;
        bool            B;
        char            I1;
        unsigned char   UI1;
        short           I2;
        unsigned short  UI2;
        int             I4;
        unsigned int    UI4;
        long            L;
        unsigned long   UL;
        long long       LL;
        unsigned long long ULL;
        __int64         I8;
        unsigned __int64 UI8;
        float           F4;
        double          F8;
    };
};

struct G2VARIANT_PARAMS {
    G2VARIANT* _arg;
    unsigned int _count;
};

//////////////////////////////////////////////////////////////////////////

struct G2SIZE {
    int cx;
    int cy;
};

struct G2POINT {
    int x;
    int y;
};

struct G2RECT {
    int left;
    int top;
    int right;
    int bottom;
};

struct G2DT {
    enum FORMAT {
        TOP             = 0x00000000,
        LEFT            = 0x00000000,
        CENTER          = 0x00000001,
        RIGHT           = 0x00000002,
        VCENTER         = 0x00000004,
        BOTTOM          = 0x00000008,
        WORDBREAK       = 0x00000010,
        SINGLELINE      = 0x00000020,
        EXPANDTABS      = 0x00000040,
        TABSTOP         = 0x00000080,
        NOCLIP          = 0x00000100,
        EXTERNALLEADING = 0x00000200,
        CALCRECT        = 0x00000400,
        NOPREFIX        = 0x00000800,
        INTERNAL        = 0x00001000
    };
};

//////////////////////////////////////////////////////////////////////////

struct G2DISCONNECT_REASON {
    enum TYPE {
        UNKNOWN                         = 0,    // unknown case
        LOGOUT                          = 1,    // normally logout (base->post)
        FULL_CHANNEL                    = 2,    // deny connection because all of server channels are used (base<-post)
        INVALID_VERSION                 = 3,    // invalid product version (base->post)
        LOGIN_FAIL                      = 4,    // invalid user or passwd (base<-post)
        ADMIN_CLOSE                     = 5,    // admin close the current connection forcibly (base<-post)
        ADMIN_TIMEOUT                   = 6,    // timeout (base<-post)
        SYS_SHUTDOWN                    = 7,    // post system shutdown (base<-post)
        NO_CHANNEL                      = 8,    // can't connect - all of my network channels are used
        NO_SERVER                       = 10,   // can't connect - no server module (sock. err=10061)
        NET_DOWN                        = 11,   // network is down (sock. err=10050)
        NET_UNREACHABLE	                = 12,   // network is unreachable (sock. err=10051)
        CONN_TIMEOUT                    = 13,   // connection time out (sock. err=10060)
        CONN_RESET                      = 14,   // connection reset by peer (sock. err=10054)
        HOST_DOWN                       = 15,   // host is down (sock. err=10064)
        HOST_UNREACHABLE                = 16,   // no route th host (sock. err=10065)
        CONN_ABORTED                    = 17,   // connection aborted (sock. err=10053)
        CONN_CANCEL                     = 20,   // connection has been canceled by user.
        NET_NORESPONSE                  = 21,   // the peer host does not respond.
        NET_NOISY                       = 22,   // network is too noisy.
        SEND_OVERFLOW                   = 23,   // sending queue overflow.
        NO_AUTHORITY                    = 25,   // You have no authority for search.
        PORT_USED                       = 26,   // the port is already in use.
        SSL_CONNECTION_FAILED           = 27,   // SSL connection failed.
        NET_TIMEOUT                     = 28,   // network is timed out
        HOST_TIMEOUT                    = 29,   // host is timed out
        NOT_SUPPORT_RTP_TCP             = 30,   // host cannot support RTP over TCP
        SOCKET_ERROR_OCCURRED           = 31,   // socket error occurred
        FEN_RENDEZ_CONN_FAILED          = 32,   // rendezvous service is not available
        FEN_RENDEZ_NO_ELEMENT           = 33,   // rendezvous Element Not Found
        FEN_RELAY_CONN_FAILED           = 34,   // relay Connection Failed
        FEN_RELAY_NOT_AVAILABLE         = 35,   // relay Service is not available
        FEN_DIRECT_CONN_DOWN            = 36,   // Fen Direct connection is closed
        FEN_UDT_CONN_DOWN               = 37,   // Fen UDT connection is closed
        FEN_RELAY_CONN_DOWN             = 38,   // Fen Relay Connection is closed
        INVALID_RECEIVE_PACKET_BUFFER   = 1001, // invalid receive packet buffer
        INVALID_SEND_PACKET_BUFFER      = 1002, // invalid send packet buffer
        ALIVE_CHECK_TICKOUT             = 1003, // alive check tick out
        RTSP_START_FAILED               = 2001, // RTSP session start failed
        RTSP_STOP_FAILED                = 2002, // RTSP session stop failed
        RTSP_IMAGE_NOT_RECEIVED         = 2003, // image is not received by rtsp
        RTSP_TEARDOWN_DISCONNECT        = 2004, // disconnected when you requests teardown
        RTSP_TUNNELING_DISCONNECT       = 2005, // request disconnect of http tunneling channel
        RTSP_SESSION_ALREADY_FINISHED   = 2006, // RTSP session is already finished
        RTSP_ALIVE_CHECK_ERROR          = 2007, // RTSP alive check error occurred
        RTSP_OVER_ALIVE_CHECK_INTERVAL  = 2008, // RTSP over alive check interval
        MISMATCH_ADAPTOR                = 10000,
        MISMATCH_PORT_UNITY             = 10001,
        NOT_SUPPORT_PRODUCT             = 10002
    };
};

struct G2SERVICE_LOGIN_FAIL_REASON {
    enum TYPE {
        REASON_NO_ERROR                 = 0,
        INVALID_VERSION                 = 1,
        FULL_CHANNEL                    = 2,
        FAILOVER_INACTIVE               = 3,    // failover inactive
        INVALID_LICENSE                 = 10,   // Invalid License(License Expired)
        INVALID_ID                      = 11,   // invalid id
        INVALID_PASSWORD                = 12,   // invalid password
        BLOCK_USER                      = 13,   // temporary disable
        CONNECTION_BY_SAME_ID           = 14,   // it is a user connecting by same ID
        DISABLE                         = 15,   // ID is disable.
        UNACCESSIBLE_IP                 = 16,   // not allowed client ip.
        INVALID_TIME                    = 17,   // because of disallowed time.
        DIRECTORY_UNABLE_CLIENT         = 18,   // client doesn't join in ActiveDirectory.
        DIRECTORY_UNABLE_SERVER         = 19,   // server doesn't join in ActiveDirectory.
        DIRECTORY_UNABLE_USER           = 20,   // user doesn't allow to ActiveDirectory Connection.
        DIRECTORY_DIFFERENT_DC          = 21,   // different DomainController.
        APP_FAIL_SEND_RETRY_OUT         = 100   // send retry too many for "guaranteed send".
    };
};

struct G2GUID {
    struct GUID_TYPE {
        unsigned int   _data1;
        unsigned short _data2;
        unsigned short _data3;
        unsigned char  _data4[8];
    };

    GUID_TYPE _guid;
    unsigned int _type;
};

struct G2GUID_LIST {
    const G2GUID* _guid;
    unsigned int _len;
};

struct G2GUID_8   { G2GUID _guid[8  ]; unsigned int _len; };
struct G2GUID_16  { G2GUID _guid[16 ]; unsigned int _len; };
struct G2GUID_32  { G2GUID _guid[32 ]; unsigned int _len; };
struct G2GUID_64  { G2GUID _guid[64 ]; unsigned int _len; };
struct G2GUID_128 { G2GUID _guid[128]; unsigned int _len; };
struct G2GUID_256 { G2GUID _guid[256]; unsigned int _len; };
struct G2GUID_512 { G2GUID _guid[512]; unsigned int _len; };

typedef G2BOOL (G2CALLBACK* G2GUIDENUMPROC)(const G2GUID* guid, G2UPARAM param);
typedef G2BOOL (G2CALLBACK* G2SERVICE_ITEM_ENUMPROC)(const G2GUID* service, const G2GUID* site, G2UPARAM param);

//////////////////////////////////////////////////////////////////////////

struct G2STRING_16  { wchar_t _string[16 ]; unsigned int _len; };
struct G2STRING_32  { wchar_t _string[32 ]; unsigned int _len; };
struct G2STRING_46  { wchar_t _string[46 ]; unsigned int _len; };
struct G2STRING_64  { wchar_t _string[64 ]; unsigned int _len; };
struct G2STRING_128 { wchar_t _string[128]; unsigned int _len; };
struct G2STRING_256 { wchar_t _string[256]; unsigned int _len; };
struct G2STRING_512 { wchar_t _string[512]; unsigned int _len; };

struct G2TIME {
    unsigned int _time; // time_t is not
};

struct G2TIME_SPAN {
    int _days;
    int _hours;
    int _minutes;
    int _seconds;
};

struct G2SPOT {
    unsigned int _segment;
    unsigned int _tick;
    G2TIME _time;
};

struct G2SPOT_PRECISION {
    G2SPOT _spot;
    int _precision;
};

struct G2SCOPE {
    G2SPOT _begin;
    G2SPOT _end;
};

struct G2AUDIO_CODEC_INFO {
    unsigned short _codec;
    unsigned short _sampling;
    unsigned short _channel;
    unsigned short _segment;
    unsigned int _bitrate;
    unsigned short _bits_per_sample;
    signed char _reserved[10];
};

struct G2AUDIO_DEVICE_INFO {
    G2AUDIO_CODEC_INFO _internal;
    int _len_output;
    int _len_sample;
    int _count_per_sec;
};

struct G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION {
    struct PARAM_ARCHIVED {
        const void* _ptr;
        int _len;
    };
    struct COORDINATE_TYPE {
        unsigned short _left;
        unsigned short _top;
        unsigned short _right;
        unsigned short _bottom;
    };
};

struct G2CODEC_INFO_SI_ELEVATOR_STATUS {
    struct PARAM_ARCHIVED {
        const void* _ptr;
        int _len;
    };

    enum DOOR_STATUS {
        STATUS_UNKNOWN = 0,
        CLOSING,
        CLOSE,
        OPENING,
        OPEN
    };
    enum DIRECTION {
        DIRECTION_UNKNOWN = 0,
        DOWN,
        STOP,
        UP
    };
    enum MODE {
        MODE_UNKNOWN = 0,
        MANUAL,
        AUTO
    };

    bool _has;
    unsigned short _id;
    float _floor;
    unsigned char _door_status;
    unsigned char _direction;
    unsigned char _mode;
    G2STRING_128  _additional;
};

struct G2FRAME {
    enum {
        FINGERPRINT_SIZE = 32,
        MAX_TITLE_LENGTH = 63
    };
    enum FROM {
        FROM_UNDEFINED = 0x0000,
        FROM_WATCH     = 0x0001,    // RAS watch
        FROM_SEARCH    = 0x0002,    // RAS search
        FROM_SEARCH_G2 = 0x0004,    // local search based G2Search
        FROM_RTP       = 0x0008,    // RTP, ONVIF(or 3rd party) protocol connect directly
        FROM_LIVE      = 0x0010,    // service streaming
        FROM_PLAY      = 0x0020,    // service recording
        FROM_BACKUP    = 0x0040,    // service backup
        FROM_LIVE_LINE = FROM_WATCH | FROM_RTP | FROM_LIVE,
        FROM_PLAY_LINE = FROM_SEARCH | FROM_SEARCH_G2 | FROM_PLAY | FROM_BACKUP
    };
    enum TYPE {
        I_FRAME = 0,
        P_FRAME,
        AUDIO,
        X_FRAME,
        B_FRAME,
        BUILT_IN_TYPE_COUNT,
        DUMMY_FRAME = 0x10,
        INVALID_TYPE = 0xFF
    };
    enum FLAG {  // bit-mask
        TIME_LAPSE  = 0x01,
        EVENT       = 0x02,
        PRE_EVENT   = 0x04,
        PANIC       = 0x08,
        FINGERPRINT = 0x10,
        IRREGULAR   = 0x20,
        BROKEN_DATA_HEADER = 0x40,
        BROKEN_DATA_BODY = 0x80,
    };

    struct INDEX_TYPE {
        G2SPOT _spot;
        G2TIME _local_time;
        unsigned int   _data_size;
        unsigned int   _plain_size;
        unsigned int   _key_id;
        unsigned short _channel;
        unsigned short _stream_id;
        unsigned char  _from;
        unsigned char  _type;
        unsigned char  _flag;
        unsigned int   _data_file_offset;
        bool           _valid;
        bool           _bad;
    };
    struct VIDEO_INFO {
        bool _byfield;
        bool _vol_header;
        bool _progressive;
        bool _no_half;
        bool _roi;
        bool _fish_eye;
        unsigned short _ing_AF;
        unsigned short _zoom;
        unsigned char _ips;
        unsigned char _ips_var;
        G2SIZE _res_override;
    };
    struct AUDIO_INFO {
        G2AUDIO_DEVICE_INFO _device;
    };
    struct FRAME_INFO_UNION {
        union {
            VIDEO_INFO _video;
            AUDIO_INFO _audio;
        };
    };
    struct EXTRA_TYPE {
        int  _decoder;
        bool _display;
        __int64 _pts;
        FRAME_INFO_UNION _info;
        void* _param_stream;
        int   _param_stream_len;
        int   _required_key_frame;  ///< mutable
    };
    struct ARCHIVED_DATA {
        G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION::PARAM_ARCHIVED _content_anlaytics_face_detection;
        G2CODEC_INFO_SI_ELEVATOR_STATUS::PARAM_ARCHIVED _si_elevator_status;
    };

    const unsigned char* _data;
    const unsigned char* _plain_ptr;
    unsigned short  _width;
    unsigned short  _height;
    wchar_t         _title[MAX_TITLE_LENGTH + 1];
    unsigned char   _fingerprint[FINGERPRINT_SIZE];
    bool            _bad;

    INDEX_TYPE _index;
    EXTRA_TYPE _extra;
    ARCHIVED_DATA _archived;
};

struct G2CHANNEL {
    enum TYPE {
        MIN = 0,
        MAX = 0xFFFF - 3,
        ANY = 0xFFFF - 2,
        ALL = 0xFFFF - 1,
        NONE = 0xFFFF
    };
};

struct G2CHANNEL_SET {
    enum {
        MAX_CHANNEL_COUNT = 256
    };
    int _channels[MAX_CHANNEL_COUNT];
    unsigned int _len;
};

struct G2CHANNEL_STREAM {
    int _channel;
    int _stream;
};

struct G2CHANNEL_STREAM_SET {
    enum {
        MAX_STREAM_COUNT = 256
    };
    G2CHANNEL_STREAM _streams[MAX_STREAM_COUNT];
    unsigned int _len;
};

struct G2TEXT_IN {
    enum TRANSACTION {
        TRANSACTION_BEGIN,
        TRANSACTION_CONTINUE,
        TRANSACTION_END,
        TRANSACTION_COMPLETE,
    };
    enum {
        RESERVED_SIZE = 4,
        MAX_DATA_SIZE = 50 * 1024,
    };

    struct INDEX_TYPE {
        unsigned short _channel;
        unsigned char  _transaction;
        unsigned char  _system_id;
        G2SPOT         _spot;
        unsigned int   _data_file_offset;
        unsigned int   _data_size;
        G2CHANNEL_SET  _record_channels;
        unsigned char  _reserved[RESERVED_SIZE];
        bool           _bad;
    };

    INDEX_TYPE _index;
    bool _bad;
    unsigned char _data[MAX_DATA_SIZE];
};

struct G2TEXT_IN_ELEMENT {
    enum FROM {
        FROM_PLAY = 0,
        FROM_LIVE
    };
    enum TYPE {
        TYPE_START = 0,
        TYPE_CONTENT,
        TYPE_END,
    };

    int     _camera;
    int     _type;
    int     _from;
    G2SPOT  _spot;
    G2STRING_256 _data;
};

struct G2SCOPE_LIST {
    const G2SCOPE* _scopes;
    unsigned int _len;
};

struct G2SPOT_LIST {
    const G2SPOT* _spots;
    unsigned int _len;
};

//////////////////////////////////////////////////////////////////////////

struct G2VERSION {
    enum TYPE {
        DEVICE_1_0 = 0x00000100,
        USER_1_0   = 0x00000100,
        DEVICE = DEVICE_1_0,
        USER = USER_1_0,
    };
    int _version;
};

struct G2VERSION_NETWORK {
    enum TYPE {
        NETWORKINFO_1_0 = 0x00000100,
        NETWORKINFO_2_0 = 0x00000200,
        NETWORKINFO_2_1 = 0x00000201,
        NETWORKINFO_3_0 = 0x00000300,
        NETWORKINFO = NETWORKINFO_3_0,
    };
    int _version;
};

struct G2SOCKET_ADDRESS {
    enum LEN {
        MAX_HOST_NAME = 50
    };
    G2STRING_128 _host_name;
    G2STRING_128 _host_address;
    G2STRING_128 _interface;
    bool         _unresolved;
    unsigned short _port;
};

struct G2MAC_ADDRESS {
    enum LEN {
        MAX_LEN = 6
    };
    unsigned char _mac[MAX_LEN];
};

struct G2SSL_STATE {
    enum TYPE {
        UNKNOWN = -1,
        NOT_USE = 0,
        PACKET_HEADER,
        PACKET_MULTIMEDIA_EXCLUDE,
        PACKET_FULL,
        PACKET_MULTIMEDIA_PARTIALLY
    };
};

struct G2NETWORK_INFO {
    enum ADDRESS_TYPE {
        ADDRESS_TYPE_UNKNOWN = -1,
        ADDRESS_TYPE_IPV4 = 0,
        ADDRESS_TYPE_DVRNS,
        ADDRESS_TYPE_DNS,
        ADDRESS_TYPE_MDNS,
        ADDRESS_TYPE_DDNS,
        ADDRESS_TYPE_IPV6,
        ADDRESS_TYPE_WSDISCOVERY
    };
    enum COMMAND_PROTOCOL_TYPE {
        COMMAND_PROTOCOL_TYPE_UNKNOWN = -1,
        COMMAND_PROTOCOL_TYPE_IDIS = 0,
        COMMAND_PROTOCOL_TYPE_ONVIF,
        COMMAND_PROTOCOL_TYPE_HTTP_TCP,
        COMMAND_PROTOCOL_TYPE_COMBINE_ONVIF_HTTP
    };
    enum MEDIA_STREAM_PROTOCOL_TYPE {
        MEDIA_STREAM_PROTOCOL_TYPE_UNKNOWN = -1,
        MEDIA_STREAM_PROTOCOL_TYPE_IDIS = 0,
        MEDIA_STREAM_PROTOCOL_TYPE_RTSP_RTP_UDP_UNICAST = 10,
        MEDIA_STREAM_PROTOCOL_TYPE_RTSP_RTP_UDP_MULTICAST,
        MEDIA_STREAM_PROTOCOL_TYPE_RTSP_RTP_TCP,
        MEDIA_STREAM_PROTOCOL_TYPE_HTTP_RTP_UDP_UNICAST = 20,
        MEDIA_STREAM_PROTOCOL_TYPE_HTTP_TCP,
        MEDIA_STREAM_PROTOCOL_TYPE_RTP_RTSP_HTTP_TCP = 30,
    };
    enum NETWORK_TYPE {
        NET_TYPE_IP = 0,
        NET_TYPE_DVRNS,
        NET_TYPE_DNS,
        NET_TYPE_RTP,
        NET_TYPE_RTSP,
        NET_TYPE_RTSP_RTP,
        NET_TYPE_HTTP_RTP,
        NET_TYPE_MDNS,
    };
    enum PORT_TYPE {
        MIN_PORT_INDEX = 0,
        SERVICE_PORT = 0,
        ADMIN_PORT,
        WATCH_PORT,
        SEARCH_PORT,
        RECORD_PORT,
        AUDIO_PORT,
        WEB_PORT,
        RTP_PORT,
        RTSP_PORT,
        EXTRA_SERVER_PORT,
        HTTPS_PORT,
        MAX_PORT_INDEX,
    };
    enum ONVIF_SERVICE_PATH_TYPE {
        DEVICE_SERVICE_PATH = 0,
        MEDIA_SERVICE_PATH,
        PTZ_SERVICE_PATH,
        IMAGING_SERVICE_PATH,
        MAX_SERVICE_PATH_INDEX,
    };
    enum ONVIF_SERVICE_PATH_EXTRA_TYPE {
        EVENT_SERVICE_PATH = 0,
        ANALYTICS_SERVICE_PATH,
        MAX_SERVICE_PATH_EXTRA_INDEX,
    };
    enum MEDIA_CONNECT_TYPE {
        MEDIA_CONNECT_TYPE_STREAM = 0,
        MEDIA_CONNECT_TYPE_RECORD,
        MEDIA_CONNECT_TYPE_SEARCH,
        MEDIA_CONNECT_TYPE_MONITORING,
        MAX_MEDIA_CONNECT_TYPE_INDEX,
    };

    struct URI {
        G2STRING_128 _rtsp_tcp;
        G2STRING_128 _rtsp_http;
        G2STRING_64  _profile_token;
    };

    struct URI_vector {
        URI _uri[32];
        unsigned int _len;
    };

    G2VERSION_NETWORK _version;
    ADDRESS_TYPE _address_type;
    COMMAND_PROTOCOL_TYPE _command_protocol_type;
    MEDIA_STREAM_PROTOCOL_TYPE _media_stream_protocol_type;
    MEDIA_STREAM_PROTOCOL_TYPE _media_record_protocol_type;
    MEDIA_STREAM_PROTOCOL_TYPE _media_search_protocol_type;
    wchar_t _address[128];
    wchar_t _address_resolved[128];
    wchar_t _user_id[32];
    wchar_t _password[65];
    wchar_t _extra_server_address[128];
    G2MAC_ADDRESS _mac;
    unsigned short _port[MAX_PORT_INDEX];
    wchar_t _profile_token[64];
    wchar_t _rtsp_uri[256];
    wchar_t _rtsp_uri_http[256];
    wchar_t _onvif_service_path[MAX_SERVICE_PATH_INDEX][64];
    wchar_t _onvif_service_path_extra[MAX_SERVICE_PATH_EXTRA_INDEX][64];
    URI_vector _uri;
};

struct G2SERVER_NETWORK_INFO
{
    enum PORT_TYPE {
        MIN_PORT_INDEX = 0,
        ADMIN_PORT = 0,
        WATCH_PORT,
        SEARCH_PORT,
        RECORD_PORT,
        AUDIO_PORT,
        WEB_PORT,
        RTSP_PORT,
        VNC_PORT,
        INEX_ADMIN_PORT,
        MAX_PORT_INDEX
    };

    G2MAC_ADDRESS _mac;
    G2MAC_ADDRESS _mac_additional[8];
    int _SSL_state;
    unsigned short _port[MAX_PORT_INDEX];
};

struct G2CONNECT_OPTIONS {
    unsigned int _connection_timeout;
};

struct G2CONNECT_RES {
    enum ERROR_TYPE {
        ERROR_UNKNOWN = -1,
        ERROR_NO = 0,
        ERROR_NETWORK_INFO_FIND,
        ERROR_NETWORK_INFO_RESOLVE,
        ERROR_ACTIVE_DIRECTORY_UNABLE_CLIENT,
        ERROR_ADAPTOR_INCOMPATIABLE
    };

    int _channel;
    int _err;
    int _err_dvrns;
};

struct G2DEVICE_STATUS {
    enum CONST_DATA {
        NUM_UNITS_MAX = 64,
    };
    enum COMMON {
        NOT_VALUE = -100,
        UNKNOWN = -1,
        INACTIVE = 0,
        ACTIVE = 1,
    };
    enum CAMERA {
        VIDEOLOSS = -1,
        NOTCONNECTED = 2,
        MULTISTREAM = 3
    };
    enum PTZ {
        ADVANCED = 2
    };
    enum PTZ_FUNCTION {
        PTZ_FUNC_PAN_TILT = 1,
        PTZ_FUNC_ZOOM = 2,
        PTZ_FUNC_FOCUS = 4,
        PTZ_FUNC_IRIS = 8,
        PTZ_FUNC_PRESET_SETUP = 16,
        PTZ_FUNC_PRESET_MOVE = 32,
        PTZ_FUNC_OSD_MENU = 64,
        PTZ_FUNC_ADVANCED = 128,
        PTZ_FUNC_FOCUS_ONEPUSH = 256,
        PTZ_FUNC_OSD_MENU_OFF_NONE = 512,
        PTZ_FUNC_RELATIVE_MOVE = 4096,
        PTZ_FUNC_ABSOLUTE_MOVE = 8192
    };
    enum COVERT {
        NORMAL = 0,
        COVERT1,
        COVERT2
    };
    enum AUDIO_IN {
        MICROPHONE = 1,
        LINEIN
    };
    enum ALARM_IN {
        NC_ = 1,    // normally closed
        NO_ = 2,    // normally open
        SENSOR_ON = 4
    };
    enum RECORDING {
        NO_RECORD = 0,
        RECORD
    };

    G2STRING_64  _sitename;
    G2STRING_32  _camera_desc[NUM_UNITS_MAX];
    signed char  _camera[NUM_UNITS_MAX];
    signed char  _ptz[NUM_UNITS_MAX];
    signed char  _covert[NUM_UNITS_MAX];
    signed char  _audio_in[NUM_UNITS_MAX];
    signed char  _audio_out[NUM_UNITS_MAX];
    G2STRING_32  _alarm_in_desc[NUM_UNITS_MAX];
    signed char  _alarm_in[NUM_UNITS_MAX];
    G2STRING_32  _alarm_out_desc[NUM_UNITS_MAX];
    signed char  _alarm_out[NUM_UNITS_MAX];
    G2STRING_32  _alarm_in_network_desc[NUM_UNITS_MAX];
    signed char  _alarm_in_network[NUM_UNITS_MAX];
    G2STRING_32  _text_in_desc[NUM_UNITS_MAX];
    signed char  _text_in[NUM_UNITS_MAX];
    signed char  _recording[NUM_UNITS_MAX];
    unsigned int _stream_remote[NUM_UNITS_MAX];
    unsigned int _ptz_function[NUM_UNITS_MAX];
};

struct G2DEVICE_STATUS_STREAM_INFO {
    enum STREAM_TYPE {
        STREAM_TYPE_ON = 0,
        STREAM_TYPE_OFF
    };

    unsigned char _type;
    unsigned char _codec;
    unsigned int  _width;
    unsigned int  _height;
    unsigned char _quality;                 // G2_PRODUCT_COMMON.G2_IMAGE_QUALITY
    unsigned char _bitrate_control_type;    // G2_PRODUCT_COMMON.G2_BITRATE_CONTROL_TYPE
    float _ips;
    bool  _roi;                             // multi-view streaming
    G2STRING_32 _title;
};

struct G2DEVICE_STATUS_IP_CAMERA_INFO {
    enum DEWARPING_STATUS_TYPE {
        DEWARPING_STATUS_UNKNOWN = -1,
        DEWARPING_STATUS_OFF,
        DEWARPING_STATUS_ON
    };

    signed char _dewarping_status;
};

struct G2DEVICE_STATUS_PTZ_ADVANCED_INFO {
    int _speed;
};

struct G2DEVICE_NAME {
    int _number;
    G2STRING_128 _name;
};

struct G2CLIPCOPY_JOB {
    enum TYPE {
        READY          = 0,
        MEASURE_SIZE   = 1,
        FORMAT_STORAGE = 3,
        COPY_DATA      = 4,
        COPY_PLAYER    = 5,
        FINALIZE       = 6
    };

    unsigned int _job;
    unsigned int _num;
    unsigned int _total;
};

struct G2CLIPCOPY_STATUS {
    enum TYPE {
        NOT_COMPLETED = -1,
        NOT_ENOUGH_SPACE = 0,
        PARTIAL_COPIABLE,
        FULL_COPIABLE,
        NO_RECORDED_DATA
    };
};

struct G2CLIPCOPY_ERROR {
    enum TYPE {
        NONE = 0,
        CANCELED,
        BANK_UPDATED,
        STORAGE_FORMAT_FAILED,
        STORAGE_OPEN_FAILED,
        CLIP_COPY_EXE_NOT_FOUND,
        CLIP_COPY_EXE_COPY_FAILED,
        DATA_COPY_FAILED,
        NO_DATA_IN_SCOPE,
        FINALIZE_FAILED,
        CLIP_COPY_STORAGE_FULL,
        LESS_FREE_SPACE_THAN_MIN_SIZE,
        INVALID_FINGER_PRINT_FOUND,
        INVALID_SKIP_INTERVAL,
        FILE_IO_FAILED
    };
};

struct G2CLIPCOPY_SIZE_INFO {
    struct INFO {
        unsigned __int64 _size;
        G2SCOPE _scope;
    };
    bool         _use_peer_player;
    INFO         _info[128];
    unsigned int _info_len;
};

struct G2CLIPCOPY_DATA {
    unsigned __int64 _offset;
    unsigned int _size;
    unsigned int _progress;
    const unsigned char*  _data;
};

struct G2ROLLBACK_INFO {
    int _channelext;
    int _precision;
    G2SPOT _spot;
};

struct G2PLAYBACK_COMMAND {
    int _speed;
    G2ROLLBACK_INFO _rbi;
};

struct G2PLAYER {
    enum COMMAND_AND_SPEED {
        NONE            = 0,    // or stop

        // play-command
        BACK_FASTEST    = -5,
        BACK_FASTER     = -4,
        BACK_FAST       = -3,
        BACK_NORMAL     = -2,
        BACK_SLOW       = -1,
        PLAY_SLOW       = +1,
        PLAY_NORMAL     = +2,
        PLAY_FAST       = +3,
        PLAY_FASTER     = +4,
        PLAY_FASTEST    = +5,

        // move-command
        COMMAND_MOVE    = 1000000,
        MOVE_TO_FIRST,
        MOVE_TO_LAST,
        MOVE_TO_SPOT,
        PREV_STEP,
        NEXT_STEP,
        RELOAD_CURRENT,
        RELOAD_RECENT,

        // search-command
        COMMAND_SEARCH  = 2000000,
        MOTION_SEARCH,

        // special commands (not used manually by user)
        COMMAND_SPECIAL = 3000000,
        ROLLBACK
    };

    struct PRECISION {
        enum TYPE {
            KEY = 0,
            FRAME,
            TEXT_IN,
            EVENT,

            HOUR,
            MINUTE,
            SECOND,
            TICK,
            TICK_SECOND
        };
    };

    struct PLAYER_ERROR {
        enum TYPE {
            HANG_ON_FAILED = 0,
            BANK_UPDATE,
            CONNECTION_LOST,
            UNKNOWN
        };
    };

    struct OUT_OF_SCOPE {
        enum TYPE {
            BEGIN_OF_SCOPE = 0,
            BEGIN_OF_SCOPE_PLAYED,
            END_OF_SCOPE_PLAYED,
            END_OF_SCOPE
        };
    };

    struct AUDIO_PLAY {
        enum TYPE {
            AUDIO_NO = 0,
            AUDIO_ALL = 1,
            ASSOCIATED_ONLY = 2
        };
    };
};

struct G2RECORD_TYPE_INFO {
    struct ELEMENT_TYPE {
        int _channelext;
        int _rec_type;
    };
    ELEMENT_TYPE _elements[64];
};

struct G2RECORD_TIME_INFO {
    enum RESOLUTION {
        MINUTE  = 0,
        HOUR    = 1,
        DAY     = 2,
        MONTH   = 3,
        SECOND  = 4
    };
    enum DIRECTION {
        BACKWARD = -1,
        FORWARD = 1
    };
    enum COMMAND {
        UNKNOWN = 0,
        INIT,
        BALANCE_BACKWARD,
        BALANCE_FORWARD,
        OVERWRITE_BACKWARD,
        OVERWRITE_FORWARD
    };

    G2RECORD_TYPE_INFO  _record_type[64];
    RESOLUTION          _resolution;
    G2CHANNEL_SET       _channels;
    G2SPOT              _spot;
    int                 _time_size;
};

struct G2FAILOVER_SCOPE_INFO {
    unsigned int _scope_id;
    G2GUID _service_key;
    G2GUID _failover_service_key;
    G2SCOPE _scope;
};

//////////////////////////////////////////////////////////////////////////

struct G2EVENT_DISK {
    enum DISK_TYPE {
        DISK_TYPE_UNKNOWN = 0,
        DISK_TYPE_IDE_HDD,
        DISK_TYPE_SCSI_HDD,
        DISK_TYPE_USB_HDD,
        DISK_TYPE_IDE_CDRW,
        DISK_TYPE_IDE_DVDRW,
        DISK_TYPE_SW_RAID_DISK,
        DISK_TYPE_FLASH_MEMORY,
        DISK_TYPE_ESATA_HDD,
        DISK_TYPE_ISCSI_HDD,
        DISK_TYPE_CAMERA_SD_CARD
    };

    int _type;
    int _number;
    int _raid_index;
};

struct G2EVENT_GPS {
    float   _lat;
    float   _lon;
    float   _speed;
    float   _angle;
    G2TIME  _time;  // time on GPS
    unsigned char _data[256];
};

struct G2EVENT_TEXT_IN {
    enum TRANSACTION {
        TRANSACTION_BEGIN = 0,
        TRANSACTION_CONTINUE,
        TRANSACTION_END,
        TRANSACTION_COMPLETE,
    };

    int  _transaction;
    int  _system_id;
    bool _bad;
    unsigned char _data[256];
};

struct G2EVENT_NETWORK_ALARM {
    char   _version[2];
    int    _event;  // user defined type
    G2TIME _time;
    __int64 _msec;
};

struct G2EVENT_USER_DEFINED {
    unsigned short _event;  // user defined type
};

struct G2EVENT {
    enum LEVEL {
        LEVEL_NORMAL = 0,
        LEVEL_EMERGENCY,
        LEVEL_ERROR
    };
    enum TYPE {
        TYPE_NONE                       = 0,
        TYPE_UNKNOWN                    = 0x7FFFFFFF,
        ALARM_IN_ON                     = 1,
        ALARM_IN_OFF                    = 2,
        ALARM_IN_BAD_ON                 = 3,
        ALARM_IN_BAD_OFF                = 4,
        USER_DEFINED_ALARM_ON           = 5,
        USER_DEFINED_ALARM_OFF          = 6,
        MOTION_ON                       = 100,
        MOTION_OFF                      = 101,
        OBJECT_TRACK_ON                 = 102,
        OBJECT_TRACK_OFF                = 103,
        TRIPZONE_ON                     = 104,
        TRIPZONE_OFF                    = 105,
        VIDEO_ANALYTICS_ON              = 120,
        VIDEO_ANALYTICS_OFF             = 121,
        IGNORED_MOTION_ON               = 122,
        IGNORED_TRIPZONE_ON             = 123,
        VIDEO_INIT                      = 200,
        VIDEO_LOSS_ON                   = 201,
        VIDEO_LOSS_OFF                  = 202,
        VIDEO_BLIND_ON                  = 203,
        VIDEO_BLIND_OFF                 = 204,
        TAMPER_ON                       = 205,
        TAMPER_OFF                      = 206,
        TEXT_IN_ON                      = 300,
        TEXT_IN_OFF                     = 301,
        TEXT_IN_DATA                    = 302,
        TEXT_IN_BAD_ON                  = 303,
        TEXT_IN_BAD_OFF                 = 304,
        FACE_DETECTION_ON               = 400,
        FACE_DETECTION_OFF              = 401,
        IGNORED_FACE_DETECTION_ON       = 402,
        TANGO_FULL_ON                   = 100000,
        TANGO_FULL_OFF                  = 100001,
        TANGO_ALMOST_FULL_ON            = 100002,
        TANGO_ALMOST_FULL_OFF           = 100003,
        TANGO_PARTIALLY_FULL_ON         = 100004,
        TANGO_PARTIALLY_FULL_OFF        = 100005,
        TANGO_PARTIALLY_ALMOST_FULL_ON  = 100006,
        TANGO_PARTIALLY_ALMOST_FULL_OFF = 100007,
        DISK_BAD                        = 200000,
        DISK_TEMPERATURE_ON             = 200001,
        DISK_TEMPERATURE_OFF            = 200002,
        DISK_SMART_ON                   = 200003,
        DISK_SMART_OFF                  = 200004,
        DISK_CONFIG_CHANGE              = 200005,
        DISK_ON                         = 200006,
        DISK_OFF                        = 200007,
        NO_DISK                         = 200008,
        SYSTEM_ALIVE                    = 300000,
        PANIC_ON                        = 300001,
        PANIC_OFF                       = 300002,
        FAN_ERROR_ON                    = 300003,
        FAN_ERROR_OFF                   = 300004,
        SYSTEM_BOOT_UP                  = 300005,
        SYSTEM_RESTART                  = 300006,
        SYSTEM_SHUTDOWN                 = 300007,
        COVER_OPEN                      = 300008,
        COVER_CLOSE                     = 300009,
        STOP_RECORD_ON                  = 300010,
        LOGIN_FAILED_SEVERAL_TIMES      = 300011,
        USER_LOGIN                      = 300012,
        USER_LOGOUT                     = 300013,
        SETUP_CHANGED                   = 300014,
        RECORDER_BAD_ON                 = 400000,
        RECORDER_BAD_OFF                = 400001,
        INSTANT_RECORDING_ON            = 400002,
        INSTANT_RECORDING_OFF           = 400003,
        CAMERA_RECORD_BAD_ON            = 400004,
        CAMERA_RECORD_BAD_OFF           = 400005,
        AUDIO_ON                        = 500000,
        AUDIO_OFF                       = 500001,
        IGNORED_AUDIO_ON                = 500002,
        USER_DEFINED                    = 600000,
        CAMERA_FAN_ERROR_ON             = 700000,
        CAMERA_FAN_ERROR_OFF            = 700001,
        NETWORK_ALARM_ON                = 1000000,
        NETWORK_ALARM_OFF               = 1000001,
        NETWORK_CAMERA_CONNECTED        = 1000010,
        NETWORK_CAMERA_DISCONNECTED     = 1000011,
        CHANNEL_RELATED_SEPARATOR       = 1100000000,
        CAR_OVERSPEED_ON                = 1100000000,
        CAR_OVERSPEED_OFF               = 1100000001,
        CAR_SUDDEN_ACCELERATION         = 1100000002,
        CAR_SUDDEN_STOP                 = 1100000003,
        CAR_STARTING_WITH_DOORS_OPEN    = 1100000004,
        GPS_RECEIVE_ERROR_ON            = 1100000100,
        GPS_RECEIVE_ERROR_OFF           = 1100000101,
        GPS_DATA                        = 1100000102,
        SIPASS_RECORD_ON                = 1200000000,
        SIPASS_RECORD_OFF               = 1200000001,
        SECOM_SMART_LOOP                = 1210000000,
        SECOM_SMART_PANIC               = 1210000001,
        SECOM_SMART_CARD                = 1210000002,
        SECOM_SMART_MACHINE             = 1210000003,
        SPC_MOTION_XOR_TRIPZONE_ON      = 1230000000,
        SPC_MOTION_XOR_TRIPZONE_OFF     = 1230000001
    };
    enum INFO_TYPE {
        INFO_TYPE_NONE = 0,
        INFO_TYPE_DISK,
        INFO_TYPE_GPS,
        INFO_TYPE_TEXT_IN,
        INFO_TYPE_NETWORK_ALARM,
        INFO_TYPE_USER_DEFINED
    };

    int           _type;
    int           _level;
    unsigned int  _channel;
    G2SPOT        _spot;
    G2STRING_256  _data;
    G2CHANNEL_SET _associated_channels;
    int           _info_type;

    union {
        G2EVENT_DISK    _disk;
        G2EVENT_GPS     _gps;
        G2EVENT_TEXT_IN _text_in;
        G2EVENT_NETWORK_ALARM _network_alarm;
        G2EVENT_USER_DEFINED _user_defined;
    };
};

struct G2EVENT_LIST {
    const G2EVENT* _events;
    unsigned int _len;
};

struct G2EVENT_INFO {
    enum TYPE_LEVEL1 {
        L1_UNKNOWN_TYPE = 0,
        L1_TIME     = 1 << 16,
        L1_SYSTEM   = 2 << 16,
        L1_DEVICE   = 3 << 16,
        L1_USER     = 4 << 16,
        L1_PREEVENT = 5 << 16,
    };
    enum TYPE_LEVEL2 {
        L2_UNKNOWN_TYPE = 0,

        // L1_TIME
        L2_CONTINUOUS = 1000,
        L2_PERIODIC,
        L2_ONE_TIME,

        // L1_SYSTEM
        L2_SERVICE_ADDED = 2000,
        L2_SERVICE_CONNECTED,
        L2_SERVICE_DISCONNECTED,

        L2_DEVICE_CONNECTED = 2500,
        L2_DEVICE_DISCONNECTED,
        L2_DEVICE_CONNECT_FAIL,
        L2_DEVICE_ADDED,
        L2_DEVICE_MODIFIED,
        L2_DEVICE_REMOVED,
        L2_NETWORK_CAMREA_CONNECTED,
        L2_NETWORK_CAMERA_DISCONNECTED,

        // camera conditions
        L2_CAMERA_EVENT_BEGIN = 3099,
        L2_CAMERA_MOTION_DETECTION_ON = 3100,
        L2_CAMERA_MOTION_DETECTION_OFF,
        L2_CAMERA_OBJECT_DETECTION_ON,
        L2_CAMERA_OBJECT_DETECTION_OFF,
        L2_CAMERA_VIDEO_LOSS_ON,
        L2_CAMERA_VIDEO_LOSS_OFF,
        L2_CAMERA_VIDEO_BLIND_ON,
        L2_CAMERA_VIDEO_BLIND_OFF,
        L2_CAMERA_IGNORED_MOTION_ON,
        L2_CAMERA_VIDEO_ANALYTICS_ON,
        L2_CAMERA_VIDEO_ANALYTICS_OFF,
        L2_CAMERA_TRIPZONE_ON,
        L2_CAMERA_TRIPZONE_OFF,
        L2_CAMERA_TAMPER_ON,
        L2_CAMERA_TAMPER_OFF,
        L2_CAMERA_IGNORED_TRIPZONE_ON,
        L2_CAMERA_IGNORED_VIDEO_ANALYTICS_ON,
        L2_CAMERA_FENCE_DETECTION,              // deprecated
        L2_CAMERA_LOITERING,                    // deprecated
        L2_CAMERA_ABANDONED_OBJECT_DETECTION,   // deprecated
        L2_CAMERA_REMOVED_OBJECT_DETECTION,     // deprecated
        L2_CAMERA_TRAFFIC,                      // deprecated
        L2_CAMERA_DIRECTION_DETECTION,          // deprecated
        L2_CAMERA_FACE_DETECTION,               // deprecated
        L2_CAMERA_FACE_DETECTION_ON,
        L2_CAMERA_IGNORED_FACE_DETECTION_ON,
        L2_CAMERA_FACE_DETECTION_OFF,
        L2_CAMERA_INSTANT_RECORD_ON,
        L2_CAMERA_INSTANT_RECORD_OFF,
        L2_CAMERA_RECORD_BAD_ON,
        L2_CAMERA_RECORD_BAD_OFF,
        L2_CAMERA_FAN_ERROR_ON,
        L2_CAMERA_FAN_ERROR_OFF,
        L2_CAMERA_EVENT_END,

        // alarm in conditions
        L2_ALARM_EVENT_BEGIN = 3199,
        L2_ALARMIN_ON = 3200,
        L2_ALARMIN_OFF,
        L2_NETWORK_ALARMIN_ON,
        L2_NETWORK_ALARMIN_OFF,
        L2_ALARMIN_BAD,
        L2_ALARM_RESET_IN,
        L2_USER_DEFINED_ALARMIN_ON,
        L2_USER_DEFINED_ALARMIN_OFF,
        L2_ALARM_EVENT_END,

        // audio conditions
        L2_AUDIO_EVENT_BEGIN = 3299,
        L2_AUDIO_ON = 3300,
        L2_AUDIO_OFF,
        L2_AUDIO_IGNORED_AUDIO_ON,
        L2_AUDIO_EVENT_END,

        L2_TEXT_IN_EVENT_BEGIN = 3399,
        L2_TEXT_IN_ON = 3400,
        L2_TEXT_IN_OFF,
        L2_TEXT_IN_BAD_ON,
        L2_TEXT_IN_BAD_OFF,
        L2_TEXT_IN_EVENT_END,

        // L1_USER
        L2_USER_EVENT_BEGIN  = 3999,
        L2_UNKNOWN_USER_TYPE = 4000,
        L2_USER_LOGIN,
        L2_USER_LOGOUT,
        L2_USER_AUTO_LOGOUT,
        L2_USER_AWAY,
        L2_USER_EVENT_END,

        // L1_DEVICE (DVR)
        L2_DVR_EVENT_BEGIN          = 4999,
        L2_DVR_TANGO_ALMOST_FULL_ON = 5000,
        L2_DVR_TANGO_ALMOST_FULL_OFF,
        L2_DVR_PANIC_RECORD_ON,
        L2_DVR_PANIC_RECORD_OFF,
        L2_DVR_RECORDER_BAD_ON,
        L2_DVR_RECORDER_BAD_OFF,
        L2_DVR_FAN_ERROR_ON,
        L2_DVR_FAN_ERROR_OFF,
        L2_DVR_SYSTEM_BOOTUP,
        L2_DVR_SYSTEM_RESTART,
        L2_DVR_SYSTEM_SHUTDOWN,
        L2_DVR_DISK_FULL_ON,
        L2_DVR_DISK_FULL_OFF,
        L2_DVR_DISK_BAD_ON,
        L2_DVR_DISK_BAD_OFF,
        L2_DVR_DISK_TEMPERATURE_ON,
        L2_DVR_DISK_TEMPERATURE_OFF,
        L2_DVR_DISK_SMART_ON,
        L2_DVR_DISK_SMART_OFF,
        L2_DVR_DISK_ON,
        L2_DVR_DISK_OFF,
        L2_DVR_DISK_CONFIG_CHANGE,
        L2_DVR_COVER_OPEN,
        L2_DVR_COVER_CLOSE,
        L2_DVR_CAR_OVERSPEED_ON,
        L2_DVR_CAR_OVERSPEED_OFF,
        L2_DVR_CAR_SUDDEN_ACCLERATION,
        L2_DVR_CAR_SUDDEN_STOP,
        L2_DVR_CAR_STARTING_WITH_DOORS_OPEN,
        L2_DVR_SYSTEM_ALIVE,
        L2_DVR_GPS_RECEIVE_ERROR_ON,
        L2_DVR_GPS_RECEIVE_ERROR_OFF,
        L2_DVR_LOGIN_FAILED_SEVERAL_TIMES,
        L2_DVR_NO_DISK,
        L2_DVR_STOP_RECORD_ON,
        L2_DVR_TANGO_PARTIALLY_FULL_ON,
        L2_DVR_TANGO_PARTIALLY_FULL_OFF,
        L2_DVR_TANGO_PARTIALLY_ALMOST_FULL_ON,
        L2_DVR_TANGO_PARTIALLY_ALMOST_FULL_OFF,
        L2_DVR_EVENT_END,

        L2_SECOM_EVENT_BEGIN = 6999,
        L2_SECOM_FS_LOOP = 7000,
        L2_SECOM_FS_PANIC,
        L2_SECOM_FS_CARD,
        L2_SECOM_FS_MACHINE,
        L2_SECOM_EVENT_END,

        L2_SIPASS_EVENT_BEGIN = 7499,
        L2_SIPASS_RECORD_ON = 7500,
        L2_SIPASS_RECORD_OFF,
        L2_SIPASS_EVENT_END
    };

    struct CALLBACK_SITE {
        G2STRING_64 _site;
        G2MAC_ADDRESS _mac;
        bool _is_name;
        bool _case_insensitive;
    };

    struct G2EVENT_RAS {
        int _seq_number;
        int _level;
        int _channel;
        unsigned int  _cameras;
        G2SPOT        _spot;
        G2STRING_128  _label;
        CALLBACK_SITE _site;
        bool    _g2;
        G2EVENT _g2event;
    };

    int     _level1;
    int     _level2;
    G2GUID  _source;
    G2GUID  _target;
    G2SPOT  _spot;
    G2TIME  _local_time;
    G2STRING_64 _label;
    G2EVENT_RAS _event_ras;
    bool    _is_evt_onetime;
    bool    _is_evt_dvr;
    bool    _is_evt_dvr_system;
    bool    _is_evt_monitoring; // event made by monitoring service
};

struct G2EVENT_LOG {
    __int64         _row_id;
    G2EVENT_INFO::TYPE_LEVEL1 _level1;
    G2EVENT_INFO::TYPE_LEVEL2 _level2;
    G2GUID          _source;
    G2GUID          _service;
    unsigned int    _action_type;
    G2CHANNEL_SET   _target;
    G2CHANNEL_SET   _target_working;
    G2STRING_128    _data;
    G2SPOT          _spot_server;
    G2TIME          _time_source;
};

struct G2SYSTEM_LOG {
    __int64         _row_id;
    int             _id;
    G2GUID          _guid1;
    G2GUID          _guid2;
    G2TIME          _time;
    G2STRING_256    _data;
};

struct G2DEBUG_LOG {
    __int64         _row_id;
    G2TIME          _time;
    G2STRING_256    _data;
};

struct G2EVENT_LOG_LIST {
    const G2EVENT_LOG* _events;
    unsigned int _len;
};

struct G2EVENT_TYPE_LIST {
    const int* _types;
    unsigned int _len;
};

struct G2EVENT_ACTION_TYPE_LIST {
    const int* _types;
    unsigned int _len;
};

struct G2SERVICE_SYSTEM_LOG_ID_LIST {
    const int* _IDs;
    unsigned int _len;
};

struct G2SERVICE_SEARCH_OPTION_EVENT_LOG {
    enum DIRECTION {
        BACKWARD = -1,
        FORWARD = 1,
        NO_DIRECTION
    };
    enum TYPE {
        RECORDING = 0,
        BACKUP
    };

    G2GUID_LIST     _source;
    G2EVENT_TYPE_LIST _event;
    G2EVENT_ACTION_TYPE_LIST _action;
    G2CHANNEL_SET   _channels;
    G2SCOPE         _scope;
    int             _direction;
    int             _type;
    __int64         _row_id;
    unsigned int    _request_count;
    bool            _retrieve_removed_device;
    bool            _uinon_result;
};

struct G2SERVICE_SEARCH_OPTION_ACTION_ACK_LOG
{
    G2GUID_LIST     _source;
    G2EVENT_TYPE_LIST _event;
    G2SCOPE         _scope;
    G2GUID          _sender;
    G2GUID          _receiver;
    __int64         _row_id;
    unsigned int    _request_count;
    int             _action;
    int             _ack;
};

struct G2SERVICE_SEARCH_OPTION_SYSTEM_LOG {
    G2GUID_LIST     _GUIDs;
    G2GUID_LIST     _GUIDs_reserved;
    G2SERVICE_SYSTEM_LOG_ID_LIST _IDs;
    __int64         _row_id;
    unsigned int    _request_count;
};

struct G2SERVICE_SEARCH_OPTION_DEBUG_LOG {
    __int64         _row_id;
    unsigned int    _request_count;
};

//////////////////////////////////////////////////////////////////////////

struct G2INSTANT_RECORDING_SUB_SETTING_NVR {
    enum RECORD_PROFILE {
        RECORD_PROFILE_VERYHIGH = 0,
        RECORD_PROFILE_HIGH,
        RECORD_PROFILE_STANDARD,
        RECORD_PROFILE_BASIC
    };

    unsigned int _record_profile;
};

struct G2INSTANT_RECORDING_SUB_SETTING_HANA {
    enum RECORD_PROFILE {
        RECORD_PROFILE_VERYHIGH = 0,
        RECORD_PROFILE_HIGH = 1,
        RECORD_PROFILE_STANDARD = 2,
        RECORD_PROFILE_BASIC = 3
    };
    enum QUALITY {
        QUALITY_LOW = 0,
        QUALITY_NORMAL = 1,
        QUALITY_HIGH = 2,
        QUALITY_VERY_HIGH = 3
    };
    enum RESOLUTION {
        RESOLUTION_STANDARD = 0,    // CIF
        RESOLUTION_HIGH = 1,        // 2CIF
        RESOLUTION_VERY_HIGH = 2    // 4CIF
    };

    unsigned int  _record_profile;  // for IP camera
    unsigned int  _ips;             // for analog camera
    unsigned char _quality;
    unsigned char _resolution;
};

struct G2INSTANT_RECORDING_SUB_SETTING_TVI {
    enum QUALITY {
        QUALITY_LOW = 0,
        QUALITY_NORMAL = 1,
        QUALITY_HIGH = 2,
        QUALITY_VERY_HIGH = 3
    };
    enum RESOLUTION {
        RESOLUTION_STANDARD = 0,
        RESOLUTION_HIGH = 1,
        RESOLUTION_VERY_HIGH = 2
    };

    unsigned int  _ips;
    unsigned char _quality;
    unsigned char _resolution;
};

struct G2INSTANT_RECORD_SETTING {
    enum PRE_DURATION {
        PRE_DURATION_USE_EXISTING_SETUP = -1,
        PRE_DURATION_STOP_PRE_EVENT = 0
    };
    enum SUB_SETTING_TYPE {
        SUB_SETTING_TYPE_UNKNOWN = 0,
        SUB_SETTING_TYPE_NVR  = 1,
        SUB_SETTING_TYPE_HANA = 2,
        SUB_SETTING_TYPE_TVI  = 3
    };

    int           _channel;
    __int64       _duration_pre;
    unsigned int  _duration_post;
    unsigned char _sub_setting_type;

    union {
        G2INSTANT_RECORDING_SUB_SETTING_NVR  _sub_setting_nvr;
        G2INSTANT_RECORDING_SUB_SETTING_HANA _sub_setting_hana;
        G2INSTANT_RECORDING_SUB_SETTING_TVI  _sub_setting_tvi;
    };
};

struct G2INSTANT_RECORD_SETTING_LIST {
    G2INSTANT_RECORD_SETTING* _list;
    unsigned int _len;
};

struct G2INSTANT_RECORDING_RESULT {
    enum TYPE {
        RESULT_SUCCESS,
        RESULT_PARTIAL_SUCCESS,
        RESULT_FAIL_NO_AUTHORITY,
        RESULT_FAIL_SYSTEM_BUSY,
        RESULT_FAIL_STORAGE_NOT_FOUND,
        RESULT_FAIL_STORAGE_FULL,
        RESULT_FAIL_PANIC_ON,
        RESULT_FAIL_UNKNOWN
    };
};

struct G2INSTANT_RECORDING_CHANNEL_STATUS {
    enum FAIL_REASON {
        FAIL_REASON_SUCCESS = 0,
        FAIL_REASON_INVALID_PARAM_CHANNEL,
        FAIL_REASON_INVALID_PARAM_POST_DURATION,
        FAIL_REASON_INVALID_PARAM_PRE_DURATION,
        FAIL_REASON_INVALID_PARAM_SUB_SETTING_TYPE,
        FAIL_REASON_INVALID_PARAM_SUB_SETTING_VALUE,
        FAIL_REASON_NO_AUTHORITY,
        FAIL_REASON_INTERNAL_SYSTEM_ERROR,
        FAIL_REASON_PANIC_ON,
        FAIL_REASON_STORAGE_NOT_FOUND,
        FAIL_REASON_STORAGE_FULL,
        FAIL_REASON_CAMERA_NOT_REGISTERED,
        FAIL_REASON_CAMERA_DEACTIVATED,
        FAIL_REASON_CAMERA_VIDEO_LOSS,
        FAIL_REASON_NOT_SUPPORTED,
        FAIL_REASON_UNKNOWN,
        FAIL_REASON_STORAGE_PARTIALLY_FULL
    };

    int           _channel;
    __int64       _duration_pre;
    unsigned int  _duration_post;
    bool          _on_recording;
    bool          _on_recording_audio;
    G2SCOPE       _scope;
    int           _fail_reason;
    int           _storage_group_id;
};

struct G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS {
    G2INSTANT_RECORDING_CHANNEL_STATUS* _list;
    unsigned int _len;
    unsigned int _result;
};

//////////////////////////////////////////////////////////////////////////

struct G2RAS_AUTHORITY {
    enum TYPE {
        AUTHORITY_SHUTDOWN_RESTART        = 1,
        AUTHORITY_UPGRADE                 = 2,
        AUTHORITY_SYSTEM_TIME_CHANGE      = 4,
        AUTHORITY_DATA_CLEAR              = 8,
        AUTHORITY_SETUP                   = 16,
        AUTHORITY_USER_MANAGE             = 32,
        AUTHORITY_COLOR_CONTROL           = 64,
        AUTHORITY_PTZ_CONTROL             = 128,
        AUTHORITY_ALARM_OUT_CONTROL       = 256,
        AUTHORITY_COVERT_CAMERA_VIEW      = 512,
        AUTHORITY_SYSTEM_CHECK            = 1024,
        AUTHORITY_RECORD_SETUP            = 2048,
        AUTHORITY_SEARCH                  = 4096,
        AUTHORITY_CLIP_COPY               = 8192,
        AUTHORITY_PTZ_SETUP               = 16384,
        AUTHORITY_ALARM_OUT_SETUP         = 32768,
        AUTHORITY_COVERT_CAMERA_SETUP     = 65536,
        AUTHORITY_SETUP_IMPORT            = 131072,
        AUTHORITY_SETUP_EXPORT            = 262144,
        AUTHORITY_VNC_SETUP               = 524288,
        AUTHORITY_ALARM_AUDIO_CONTROL     = 1048576,
        AUTHORITY_ALARM_AUDIO_SETUP       = 2097152,
        AUTHORITY_POWER_MANAGEMENT        = 4194304,
        AUTHORITY_NETWORK_CAMERA_REGISTER = 8388608
    };
    enum LEVEL {
        LEVEL_NONE  = 0,
        LEVEL_USER  = 1,
        LEVEL_ADMIN = 2,
        LEVEL_XDR   = 3
    };

    unsigned int _authority;
    unsigned int _level;
};

//////////////////////////////////////////////////////////////////////////

struct G2PROBE_SESSION_PROFILE
{
    int   _channel;
    int   _SSL_state;
    int   _FEN_connection;
    float _ips;
    int   _bps;   // kilo bits per second
    int   _bps_audio;
};

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

//////////////////////////////////////////////////////////////////////////

#endif  // !_G2_DEFINE_H_
