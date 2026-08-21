// G2ClientView.h : header file
//

#pragma once

#include "G2ClientDoc.h"
#include "control/drop_target.h"
#include "panel/splitter_wnd.h"
#include "panel/panel_screen.h"
#include "panel/panel_controller.h"
#include "screen/screen_actuator_listener.h"
#include "connective/connective_camera_map.h"
#include "connective/connective_host_list.h"

#include <common/client_define.h>
#include <sampler/cpp/g2client_watch_listener.h>
#include <sampler/cpp/g2client_search_listener.h>
#include <sampler/cpp/g2client_search_g2_listener.h>
#include <sampler/cpp/g2client_dualaudio_listener.h>
#include <sampler/cpp/g2client_status_listener.h>

namespace client {
    class g2watch;
    class g2search;
    class g2search_g2;
    class g2dualaudio;
    class g2status;
    class message_box;
    class controller_base;
}

//////////////////////////////////////////////////////////////////////////

class CG2ClientView : public CView
                    , protected client::g2watch_listener
                    , protected client::g2search_listener
                    , protected client::g2search_g2_listener
                    , protected client::g2dualaudio_listener
                    , protected client::g2status_listener
                    , protected client::drop_event
                    , protected client::screen_actuator_listener
{
protected:
	CG2ClientView(void);
	DECLARE_DYNCREATE(CG2ClientView)

public:
    virtual ~CG2ClientView(void);

public:
	CG2ClientDoc* GetDocument(void) const;

protected:
    client::g2watch*        _watch;
    client::g2search*       _search;
    client::g2search_g2*    _search_g2;
    client::g2dualaudio*    _audio;
    client::g2status*       _status;

public:
    client::g2watch& watch_ref(void) const;
    client::g2search& search_ref(void) const;
    client::g2search_g2& search_g2_ref(void) const;
    client::g2dualaudio& audio_ref(void) const;
    client::g2status& status_ref(void) const;

protected:
    virtual void on_g2watch_connected(G2HWATCH handle, int channel);
    virtual void on_g2watch_disconnected(G2HWATCH handle, int channel, G2DISCONNECT_REASON::TYPE reason);
    virtual void on_g2watch_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame);
    virtual void on_g2watch_receive_event(G2HWATCH handle, int channel, const G2EVENT_INFO& info);
    virtual void on_g2watch_receive_device_status(G2HWATCH handle, int channel, const G2DEVICE_STATUS& status);
    virtual void on_g2watch_receive_ptz_preset(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_PRESET& preset);
    virtual void on_g2watch_receive_ptz_menu(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_MENU& menu);
    virtual void on_g2watch_receive_camera_title_idr(G2HWATCH handle, int channel, int camera, const std::wstring& title);
    virtual void on_g2watch_receive_text_in(G2HWATCH handle, int channel, const G2TEXT_IN& data);
    virtual void on_g2watch_receive_network_camera_information(G2HWATCH handle, int channel) {}
    virtual void on_g2watch_receive_audio_out_not_available(G2HWATCH handle, int channel);
    virtual void on_g2watch_receive_command_result_control_color_status(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, const G2LIVE_COMMAND_CONTROL_COLOR_RANGE& range) {}
    virtual void on_g2watch_receive_command_result_control_color(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, G2LIVE_COMMAND_RESULT::TYPE result) {}
    virtual void on_g2watch_receive_command_result_control_ptz_status(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ& control, const G2LIVE_COMMAND_CONTROL_PTZ_RANGE& range) {}
    virtual void on_g2watch_receive_command_result_control_ptz(G2HWATCH handle, int channel, int camera, G2LIVE_COMMAND_RESULT::TYPE result) {}
    virtual void on_g2watch_receive_network_alarm_result(G2HWATCH handle, int channel, const G2LIVE_NETWORK_ALARM_RESULT& result) {}
    virtual void on_g2watch_receive_elevator_status_info_response(G2HWATCH handle, int channel, unsigned int seq_number) {}
    virtual void on_g2watch_receive_instant_recording_start(G2HWATCH handle, int channel, const G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS& status) {}
    virtual void on_g2watch_receive_instant_recording_stop(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT::TYPE result) {}
    virtual void on_g2watch_receive_instant_recording_status(G2HWATCH handle, int channel, const G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS& status) {}
    virtual void on_g2watch_audio_streaming_started(G2HWATCH handle, int channel, int camera);
    virtual void on_g2watch_audio_streaming_stopped(G2HWATCH handle, int channel, int camera);
    virtual void on_g2watch_audio_capturing_started(G2HWATCH handle, int channel, int camera);
    virtual void on_g2watch_audio_capturing_stopped(G2HWATCH handle, int channel, int camera);
    virtual void on_g2watch_probe_session_profile(G2HWATCH handle, int channel, const G2PROBE_SESSION_PROFILE& probe) {}

protected:
    virtual void on_g2search_connected(G2HSEARCH handle, int channel);
    virtual void on_g2search_disconnected(G2HSEARCH handle, int channel, G2DISCONNECT_REASON::TYPE reason);
    virtual void on_g2search_receive_recorded_date(G2HSEARCH handle, int channel, const std::vector<G2TIME>& dates);
    virtual void on_g2search_receive_recorded_time_hour(G2HSEARCH handle, int channel, const bool hour[][24], int segcount);
    virtual void on_g2search_receive_recorded_time_minute(G2HSEARCH handle, int channel, const unsigned char minute[][24 * 60]);
    virtual void on_g2search_receive_recorded_rechour_minute(G2HSEARCH handle, int channel, const std::vector<G2RECORD_TIME_INFO>& rti);
    virtual void on_g2search_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame);
    virtual void on_g2search_receive_no_frame(G2HSEARCH handle, int channel);
    virtual void on_g2search_receive_no_recorded_data(G2HSEARCH handle, int channel);
    virtual void on_g2search_receive_no_recorded_data_from_search_target(G2HSEARCH handle, int channel, G2SEARCH_TARGET::TYPE target);
    virtual void on_g2search_receive_find_idr_event_time(G2HSEARCH handle, int channel, const G2TIME& time);
    virtual void on_g2search_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND speed);
    virtual void on_g2search_receive_notify_play_stop_post(G2HSEARCH handle, int channel, G2SEARCH_DRIVE::MODE mode);
    virtual void on_g2search_receive_notify_end_of_play(G2HSEARCH handle, int channel);
    virtual void on_g2search_receive_notify_segment_changed(G2HSEARCH handle, int channel, int segment);
    virtual void on_g2search_receive_notify_search_mode_changed(G2HSEARCH handle, int channel, G2SEARCH_MODE::TYPE mode);
    virtual void on_g2search_receive_notify_command_end(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND command);
    virtual void on_g2search_receive_query_result_event(G2HSEARCH handle, int channel, const std::vector<G2SEARCH_LOG_INFO>& info);
    virtual void on_g2search_receive_query_result_text_in(G2HSEARCH handle, int channel, const std::vector<G2TEXT_IN>& data);
    virtual void on_g2search_receive_recorded_date_scope(G2HSEARCH handle, int channel, const std::vector<G2SCOPE>& scopes);
    virtual void on_g2search_receive_segment_spot(G2HSEARCH handle, int channel, const std::vector<G2SPOT>& spots);
    virtual void on_g2search_receive_error(G2HSEARCH handle, int channel);
    virtual void on_g2search_receive_text_in(G2HSEARCH handle, int channel, const G2TEXT_IN_ELEMENT& data);
    virtual void on_g2search_receive_external_tango_info(G2HSEARCH handle, int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& info);
    virtual void on_g2search_receive_gps_data_start(G2HSEARCH handle, int channel);
    virtual void on_g2search_receive_gps_data(G2HSEARCH handle, int channel, const G2TEXT_IN& data);
    virtual void on_g2search_receive_gps_data_list(G2HSEARCH handle, int channel, const std::vector<G2TEXT_IN>& data);
    virtual void on_g2search_receive_gps_data_end(G2HSEARCH handle, int channel);
    virtual void on_g2search_receive_gps_data_end_count(G2HSEARCH handle, int channel, int total);
    virtual void on_g2search_receive_gps_data_export_cancel_result(G2HSEARCH handle, int channel, int result);
    virtual void on_g2search_receive_gps_data_measure_result(G2HSEARCH handle, int channel, int count, signed char done);
    virtual void on_g2search_require_prepare_playback(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND command, const G2SPOT& spot);
    virtual bool on_g2search_require_prepare_load_event_image(G2HSEARCH handle, int channel, int selected, bool last);
    virtual bool on_g2search_require_prepare_reload(G2HSEARCH handle, int channel);
    virtual void on_g2search_probe_session_profile(G2HSEARCH handle, int channel, const G2PROBE_SESSION_PROFILE& probe) {}

protected:
    virtual void on_g2search_saver_disconnected(G2HSEARCH handle, int channel, unsigned int reason) {}
    virtual void on_g2search_saver_receive_recorded_date(G2HSEARCH handle, int channel, const std::vector<G2TIME>& dates) {}
    virtual void on_g2search_saver_receive_frame_data(G2HSEARCH handle, int channel, const G2FRAME& frame) {}
    virtual void on_g2search_saver_receive_no_frame(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_no_recorded_data(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, int speed) {}
    virtual void on_g2search_saver_receive_notify_play_stop_spot(G2HSEARCH handle, int channel, G2SEARCH_DRIVE::MODE mode) {}
    virtual void on_g2search_saver_receive_notify_end_of_play(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_notify_command_end(G2HSEARCH handle, int channel, int command) {}
    virtual void on_g2search_saver_receive_clipcopy_scope(G2HSEARCH handle, int channel, const std::vector<G2SCOPE>& scopes) {}
    virtual void on_g2search_saver_receive_clipcopy_cancel(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_clipcopy_measure_size(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_clipcopy_size(G2HSEARCH handle, int channel, signed char status, unsigned __int64 size, const G2TIME& begin, const G2TIME& end) {}
    virtual void on_g2search_saver_receive_clipcopy_data(G2HSEARCH handle, int channel, bool completed, int progress, int count, int length, const unsigned char* data) {}
    virtual void on_g2search_saver_receive_clipcopy_password(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_clipcopy_enable_channels(G2HSEARCH handle, int channel, unsigned int cameras) {}
    virtual void on_g2search_saver_receive_bank_space(G2HSEARCH handle, int channel, int image_index_num, int audio_index_num, const G2TIME& start, int start_msec, unsigned __int64 image_size, unsigned __int64 audio_size) {}
    virtual void on_g2search_saver_receive_bank_image(G2HSEARCH handle, int channel, const unsigned char* data) {}
    virtual void on_g2search_saver_receive_bank_audio(G2HSEARCH handle, int channel, const unsigned char* data) {}
    virtual void on_g2search_saver_receive_bank_no_image(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_bank_no_audio(G2HSEARCH handle, int channel) {}

protected:
    virtual void on_g2search_g2_connected(G2HSEARCH_G2 handle, int channel);
    virtual void on_g2search_g2_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON::TYPE reason);
    virtual void on_g2search_g2_receive_record_time_info_load(G2HSEARCH_G2 handle, int channel, const G2RECORD_TIME_INFO& rti);
    virtual void on_g2search_g2_receive_record_time_info_load_end(G2HSEARCH_G2 handle, int channel, G2RECORD_TIME_INFO::RESOLUTION resolution, G2RECORD_TIME_INFO::COMMAND command);
    virtual void on_g2search_g2_receive_frame_data(G2HSEARCH_G2 handle, int channel, const G2FRAME& frame);
    virtual void on_g2search_g2_receive_text_in(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei);
    virtual void on_g2search_g2_receive_event(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei);
    virtual void on_g2search_g2_receive_gps_data(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei);
    virtual void on_g2search_g2_receive_notify_command_begin(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command);
    virtual void on_g2search_g2_receive_notify_command_end(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command);
    virtual void on_g2search_g2_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED speed);
    virtual void on_g2search_g2_receive_notify_frame_not_found(G2HSEARCH_G2 handle, int channel, G2SPOT& spot, G2PLAYER::PRECISION::TYPE precision);
    virtual void on_g2search_g2_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE status);
    virtual void on_g2search_g2_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, G2ROLLBACK_INFO& rbi);
    virtual void on_g2search_g2_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE error);
    virtual void on_g2search_g2_receive_event_log_load_end(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list);
    virtual void on_g2search_g2_receive_event_log_load_stop(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list);
    virtual void on_g2search_g2_receive_text_in_log_load_end(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list);
    virtual void on_g2search_g2_receive_text_in_log_load_stop(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list);
    virtual void on_g2search_g2_receive_scope_list(G2HSEARCH_G2 handle, int channel,  const std::vector<G2SCOPE>& scopes, G2SEARCH_G2_SCOPE_TYPE::TYPE type);
    virtual void on_g2search_g2_receive_spot_list(G2HSEARCH_G2 handle, int channel, const std::vector<G2SPOT>& spots);
    virtual void on_g2search_g2_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel);
    virtual void on_g2search_g2_receive_db_info(G2HSEARCH_G2 handle, int channel, const G2SEARCH_G2_REMOTE_DB& di);
    virtual void on_g2search_g2_receive_db_info_external(G2HSEARCH_G2 handle, int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& dis);
    virtual void on_g2search_g2_receive_db_selected(G2HSEARCH_G2 handle, int channel, unsigned int id, G2SEARCH_G2_REMOTE_DB::DB_SELECT_RESULT result);
    virtual void on_g2search_g2_receive_virtual_channelmap(G2HSEARCH_G2 handle, int channel);
    virtual void on_g2search_g2_require_prepare_rollback(G2HSEARCH_G2 handle, int channel, bool prepare);
    virtual void on_g2search_g2_probe_session_profile(G2HSEARCH_G2 handle, int channel, const G2PROBE_SESSION_PROFILE& probe) {}

// listener dual audio
protected:
            void on_g2dualaudio_connection_impl(const int channel);
    virtual void on_g2dualaudio_connected(G2HDUALAUDIO handle, int channel);
    virtual void on_g2dualaudio_disconnected(G2HDUALAUDIO handle, int channel, G2DISCONNECT_REASON::TYPE reason);
    virtual void on_g2dualaudio_receive_audio_in_opened(G2HDUALAUDIO handle, int channel, const G2GUID& root);
    virtual void on_g2dualaudio_receive_audio_in_closed(G2HDUALAUDIO handle, int channel, const G2GUID& root);
    virtual void on_g2dualaudio_receive_audio_out_opened(G2HDUALAUDIO handle, int channel, const G2GUID& root);
    virtual void on_g2dualaudio_receive_audio_out_closed(G2HDUALAUDIO handle, int channel, const G2GUID& root);
    virtual void on_g2dualaudio_require_select_channel(G2HDUALAUDIO handle, int channel, const G2GUID& root, int count, int& number);
    virtual bool on_g2dualaudio_require_reconnect_imp(G2HDUALAUDIO handle, const G2GUID& root);

// listener status
protected:
    virtual void on_g2status_connected(G2HSTATUS handle, int channel);
    virtual void on_g2status_disconnected(G2HSTATUS handle, int channel, G2DISCONNECT_REASON::TYPE reason);
    virtual void on_g2status_receive_status(G2HSTATUS handle, int channel, const G2MONITORING_DEVICE_STATUS& status);
    virtual void on_g2status_receive_callback_event(G2HSTATUS handle, const G2EVENT_INFO& ei);
    virtual void on_g2status_receive_log_system(G2HSTATUS handle, int channel, const std::list<G2STATUS_LOG_SYSTEM>& logs) {}
    virtual void on_g2status_receive_log_event(G2HSTATUS handle, int channel, const std::list<G2STATUS_LOG_EVENT>& logs) {}
    virtual void on_g2status_receive_log_debug(G2HSTATUS handle, int channel, const std::list<G2STATUS_LOG_SYSTEM>& logs) {}
    CString parse_status(const G2MONITORING_DEVICE_STATUS& status);

protected:
    virtual void on_screen_no_image_loaded(short hostId);
    virtual void on_screen_camera_changed(short camera);
    virtual void on_screen_layout_changed(int layout, int changed = client::screen::layout_change_::CHANGE);
    virtual void on_screen_image_drew(short camera, const G2SPOT& spot);

    virtual void on_screen_lbutton_down(unsigned int flags, const CPoint& point);
    virtual void on_screen_lbutton_up(unsigned int flags, const CPoint& point);
    virtual void on_screen_lbutton_dblclk(unsigned int flags, const CPoint& point);
    virtual void on_screen_rbutton_down(unsigned int flags, const CPoint& point);
    virtual void on_screen_rbutton_up(unsigned int flags, const CPoint& point);
    virtual void on_screen_mbutton_down(unsigned int flags, const CPoint& point);
    virtual void on_screen_mbutton_up(unsigned int flags, const CPoint& point);
    virtual void on_screen_mouse_move(unsigned int flags, const CPoint& point);
    virtual void on_screen_mouse_wheel(unsigned int flags, short zdelta, const CPoint& point);
    virtual bool on_screen_set_cursor(CWnd* pWnd, unsigned int hittest, unsigned int message, int camera, const CRect& rect);
    virtual void on_screen_key_down(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags);
    virtual void on_screen_key_up(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags);
    virtual void on_screen_set_foucs(CWnd* pOldWnd);
    virtual void on_screen_resized(unsigned int type, int cx, int cy);

//////////////////////////////////////////////////////////////////////////

public:
    enum TIMER_ID {
        TIMER_ID_ALIVE_CHECK = 1024,
        TIMER_ID_RECONNECT = 2048
    };

protected:
    g2::critical_section _lock_connection;
    g2::critical_section _lock_cameralist;
    g2::critical_section _lock_disconnected;

    client::splitter_wnd _splitter;
    client::panel_screen _screenPannel;
    client::panel_controller _controllerPannel;

    client::message_box* _message;

    client::camera_map _cameraMap;
    client::connective_host_list _hostList;

    std::list<int> _disconnectByMe;
    std::set<short> _endofplayHost;

    client::CIDropTarget* _dropTarget;
    client::drop_info _dropInfo;
    std::map<short, client::drop_info> _dropInfoEvent;

    int _viewType;
    int _selSpkCamera;
    int _selMicCamera;
    
    typedef G2DUALAUDIO_OPEN_MODE::MODE audio_mode;
    typedef struct _audio_info{
        const client::g2guid    _guid;
        int                     _camera;
        audio_mode              _mode;
        bool                    _activate;

        _audio_info(client::g2guid guid, int camera = client::invalid_::CAMERA_NUMBER, audio_mode mode = G2DUALAUDIO_OPEN_MODE::AUDIO_INVALID, bool activate = false)
            : _guid(guid)
            , _camera(camera)
            , _mode(mode)
            , _activate(activate)
        {}
    } audio_info;
    typedef boost::shared_ptr<audio_info> audio_info_ptr;

    std::list<audio_info_ptr> _audio_info_list;

public:
    HWND safe_hwnd(void) const { return GetSafeHwnd(); }
    client::screen_actuator& actuator_ref(void);
    client::message_box* message_box_ptr(void);
    client::controller_watch* controller_live_ptr(void);
    client::controller_search* controller_search_ptr(void);
    client::controller_search_g2* controller_search_g2_ptr(void);
    
    std::vector<client::controller_base*> controller_bunch(void);

    void initialize(void);
    void finalize(void);
    void ctor_network(void);
    void dtor_network(void);
    void disconnect_all(void);
    void cleanup(void);

    void register_drag_drop(void);
    void unregister_drag_drop(void);

    void show_screen_message(const std::wstring& string, bool autohide = true, int elapse = 5000);
    void show_screen_message(const std::wstring& string, const std::wstring& message, bool autohide = true, int elapse = 5000);
    void hide_screen_message(void);

    client::camera_map* get_camera_map(void) { return &_cameraMap; }
    client::connective_host_list* get_host_list(void) { return &_hostList; }

    int set_play_speed(short hostId, int command, int clientSpeed = client::search::speed_::PLAY_SPEED_UNDEFINED);

    void set_view_type(int type);
    int get_view_type(void) const { return _viewType; }
    void connect_status(void);

protected:
    void set_camera_map(short hostId, const client::drop_info& di);
    void remove_camera_map(short hostId);

    void set_camera_list(short hostId, bool clearLiveBuff = false, bool force = false);
    void set_camera_list_impl_watch(short hostId, short channel, const g2::channels& cameras, bool force = false);
    void set_camera_list_impl_rtp(short hostId, short channel, const g2::channels& cameras, bool force = false);
    void set_camera_list_impl_live(short hostId, short channel, const std::set<int>& channelSetInterest, const std::set<int>& channelSetLive, bool force = false);
    void set_camera_list_impl_play(short hostId, short channel, const std::set<int>& channelSetInterest, const std::set<int>& channelSetPlay);
    void set_camera_list_impl_search(short hostId, short channel, const g2::channels& cameras, bool force = false);
    void set_camera_list_impl_search_g2(short hostId, short channel, const std::set<int>& channelSetInterest, const std::set<int>& channelSetPlay);

    void release_host_id(short hostId, bool redraw = true, bool connect = true);

    bool is_connected(client::connect_mode_::MODE mode, short channel);
    bool is_disconnected(client::connect_mode_::MODE mode, short channel);
    bool is_disconnectable(client::connect_mode_::MODE mode, short channel);

    bool invoke_drop_info_device(client::drop_info& di);

    void connect_watch(client::drop_info& di);
    void connect_search(client::drop_info& di);


    void set_disconnect_by_me(short hostId, bool byMe = true);
    bool is_disconnect_by_me(short hostId);

    void reset_screen_view(short hostId, bool redraw = true, bool connect = true);
    void init_camera_title(short camera, bool redraw = true);
    void init_camera_title(unsigned __int64 cameras, bool redraw = true);

    void disconnect_each_camera(short selcamera);
    void disconnect_each_device(short selcamera);
    void disconnect_device(short selcamera);
    void disconnect_by_host_id(short hostId);

    bool reconnect_device(short hostId, unsigned int reason = G2DISCONNECT_REASON::UNKNOWN);
    bool reconnect_impl(short hostId);
    bool reconnect_kill(short hostId);

    client::screen::camera_status_::TYPE camera_status_by_device_status(int status);
    client::screen::camera_status_::TYPE camera_status_by_direct_mode(int connectMode, short channel = -1, short hostcamera = -1);

    bool is_audio_stream_in_live_port(int selcamera);
    bool is_activate_dual_audio_speaker(int selcamera);
    bool is_activate_dual_audio_microphone(int selcamera);
    bool is_activate_dual_audio(const G2GUID& root);

protected:
    virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
    virtual void OnDraw(CDC* pDC);  // overridden to draw this view
    virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
    virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
    virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

#ifdef _DEBUG
    virtual void AssertValid() const;
    virtual void Dump(CDumpContext& dc) const;
#endif

    virtual void OnDrop(client::drop_info& di);

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpcs);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
    afx_msg void OnTimer(UINT_PTR nIDEvent);

    /////////////////////////////////////////////

    afx_msg void on_drop_watch(void);
    afx_msg void on_drop_search(void);
    afx_msg void on_screen_camera_instant_record(void);
    afx_msg void on_screen_camera_disconnect(UINT nID);
    afx_msg void on_screen_camera_ratio(UINT nID);
    afx_msg void on_screen_camera_listen(void);
    afx_msg void on_screen_camera_talk(void);

    afx_msg LRESULT on_drop_from_screen(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_drop_info_secure(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_connected_play(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_connected_search(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_connected_search_g2(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_disconnected_live(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_disconnected_play(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_disconnected_search(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_disconnected_search_g2(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_receive_recorded_date_search(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_screen_no_image_loaded_play(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_screen_no_image_loaded_search(WPARAM wParam, LPARAM lParam);
    afx_msg LRESULT on_post_screen_no_image_loaded_search_g2(WPARAM wParam, LPARAM lParam);

    afx_msg LRESULT on_post_rectime_info_load_end_search_g2(WPARAM wParm, LPARAM lParm);
    afx_msg LRESULT on_post_no_recorded_data_search_g2(WPARAM wParm, LPARAM lParm);
    afx_msg LRESULT on_post_receive_command_begin_search_g2(WPARAM wParm, LPARAM lParm);

    afx_msg LRESULT on_post_connected_dual_audio_out(int selcamera, bool activate);
    afx_msg LRESULT on_post_connected_dual_audio_in(int selcamera, bool activate);    

public:
    static CG2ClientView* __self_pointer__;
};

#ifndef _DEBUG  // debug version in G2ClientView.cpp
inline CG2ClientDoc* CG2ClientView::GetDocument(void) const
   { return reinterpret_cast<CG2ClientDoc*>(m_pDocument); }
#endif

//////////////////////////////////////////////////////////////////////////

namespace client {
    typedef CG2ClientView runner;
    client::runner* runner_ptr(bool securing = false);
}
