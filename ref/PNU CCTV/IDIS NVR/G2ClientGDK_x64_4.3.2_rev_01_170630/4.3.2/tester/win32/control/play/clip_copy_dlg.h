// clip_copy_dlg.h : header file
//

#ifndef _CONTROL_CLIP_COPY_DLG_H_
#define _CONTROL_CLIP_COPY_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <sampler/cpp/g2client_search_listener.h>
#include <sampler/cpp/g2client_search_g2_listener_saver.h>
#include <boost/shared_ptr.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class clip_copy_dlg : public CDialog
                    , protected client::g2search_listener
                    , protected client::g2search_g2_listener_saver
{
public:
    typedef enum {
        CONNECT_SEARCH,
        CONNECT_SEARCH_G2
    }connect_t;

public:
	clip_copy_dlg(short hostId, CWnd* pParent = NULL);
	clip_copy_dlg(short hostId, client::g2search_g2* search_g2, CWnd* parent = NULL);
	virtual ~clip_copy_dlg(void);

protected:
    enum {
        STANBY = 0,
        CLIPCOPY,
        CANCELED
    };

protected:
    CWnd*           _centerBase;
    CWnd*           _parent;

    CButton         _btnStart;
    CButton         _btnCancel;
    CButton         _chkFirst;
    CButton         _chkLast;
    CButton         _chkPassword;
    CDateTimeCtrl   _dtcFromDate;
    CDateTimeCtrl   _dtcFromTime;
    CDateTimeCtrl   _dtcToDate;
    CDateTimeCtrl   _dtcToTime;
    CProgressCtrl   _progressBar;

    G2TIME          _begin;
    G2TIME          _end;

    CString         _password;
    CString         _filePath;
    CString         _dirPath;

    int             _channelext;
    int             _channel;
    int             _progress;
    int             _clipcopyStatus;
    short           _hostId;

    bool            _initialized;
    bool            _started;
    bool            _usePassword;

    CFile           _file;
    connect_t       _mode;

    boost::shared_ptr<client::g2search>     _search;
    client::g2search_g2*  _search_g2;

protected:
    void initialize_search(void);
    void finalize_search(void);

    bool is_initialize(void) { return _initialized; }
    bool make_time_condition(G2TIME& begin, G2TIME& end);
    bool create_video_file(const CString& path);
    bool remove_video_file(void);
    bool is_exit_file(const CString& path);
    CString open_file_path(void);
    unsigned int get_dir_free_size(const CString& path, bool expandable = false);
    void enable_controls(bool enable = true);

/// search listener //////////////////////////////////////////////////////////////////
    virtual void on_g2search_connected(G2HSEARCH handle, int channel);
    virtual void on_g2search_disconnected(G2HSEARCH handle, int channel, G2DISCONNECT_REASON::TYPE reason);
    virtual void on_g2search_receive_recorded_date(G2HSEARCH handle, int channel, const std::vector<G2TIME>& dates) {}
    virtual void on_g2search_receive_recorded_time_hour(G2HSEARCH handle, int channel, const bool hour[][24], int segcount) {}
    virtual void on_g2search_receive_recorded_time_minute(G2HSEARCH handle, int channel, const unsigned char minute[][24 * 60]) {}
    virtual void on_g2search_receive_recorded_rechour_minute(G2HSEARCH handle, int channel, const std::vector<G2RECORD_TIME_INFO>& rti) {}
    virtual void on_g2search_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame) {}
    virtual void on_g2search_receive_no_frame(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_receive_no_recorded_data(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_receive_no_recorded_data_from_search_target(G2HSEARCH handle, int channel, G2SEARCH_TARGET::TYPE target) {}
    virtual void on_g2search_receive_find_idr_event_time(G2HSEARCH handle, int channel, const G2TIME& time) {}
    virtual void on_g2search_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND speed) {}
    virtual void on_g2search_receive_notify_play_stop_post(G2HSEARCH handle, int channel, G2SEARCH_DRIVE::MODE mode) {}
    virtual void on_g2search_receive_notify_end_of_play(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_receive_notify_segment_changed(G2HSEARCH handle, int channel, int segment) {}
    virtual void on_g2search_receive_notify_search_mode_changed(G2HSEARCH handle, int channel, G2SEARCH_MODE::TYPE mode) {}
    virtual void on_g2search_receive_notify_command_end(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND command) {}
    virtual void on_g2search_receive_query_result_event(G2HSEARCH handle, int channel, const std::vector<G2SEARCH_LOG_INFO>& info) {}
    virtual void on_g2search_receive_query_result_text_in(G2HSEARCH handle, int channel, const std::vector<G2TEXT_IN>& data) {}
    virtual void on_g2search_receive_recorded_date_scope(G2HSEARCH handle, int channel, const std::vector<G2SCOPE>& scopes) {}
    virtual void on_g2search_receive_segment_spot(G2HSEARCH handle, int channel, const std::vector<G2SPOT>& spots) {}
    virtual void on_g2search_receive_error(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_receive_text_in(G2HSEARCH handle, int channel, const G2TEXT_IN_ELEMENT& data) {}
    virtual void on_g2search_receive_external_tango_info(G2HSEARCH handle, int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& info) {}
    virtual void on_g2search_receive_gps_data_start(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_receive_gps_data(G2HSEARCH handle, int channel, const G2TEXT_IN& data) {}
    virtual void on_g2search_receive_gps_data_list(G2HSEARCH handle, int channel, const std::vector<G2TEXT_IN>& data) {}
    virtual void on_g2search_receive_gps_data_end(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_receive_gps_data_end_count(G2HSEARCH handle, int channel, int total) {}
    virtual void on_g2search_receive_gps_data_export_cancel_result(G2HSEARCH handle, int channel, int result) {}
    virtual void on_g2search_receive_gps_data_measure_result(G2HSEARCH handle, int channel, int count, signed char done) {}
    virtual void on_g2search_require_prepare_playback(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND command, const G2SPOT& spot) {}
    virtual bool on_g2search_require_prepare_load_event_image(G2HSEARCH handle, int channel, int selected, bool last) { return false; }
    virtual bool on_g2search_require_prepare_reload(G2HSEARCH handle, int channel) { return false; }
    virtual void on_g2search_probe_session_profile(G2HSEARCH handle, int channel, const G2PROBE_SESSION_PROFILE& probe) {}

    virtual void on_g2search_saver_disconnected(G2HSEARCH handle, int channel, unsigned int reason);
    virtual void on_g2search_saver_receive_recorded_date(G2HSEARCH handle, int channel, const std::vector<G2TIME>& dates) {}
    virtual void on_g2search_saver_receive_frame_data(G2HSEARCH handle, int channel, const G2FRAME& frame) {}
    virtual void on_g2search_saver_receive_no_frame(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_no_recorded_data(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, int speed) {}
    virtual void on_g2search_saver_receive_notify_play_stop_spot(G2HSEARCH handle, int channel, G2SEARCH_DRIVE::MODE mode) {}
    virtual void on_g2search_saver_receive_notify_end_of_play(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_notify_command_end(G2HSEARCH handle, int channel, int command) {}
    virtual void on_g2search_saver_receive_clipcopy_scope(G2HSEARCH handle, int channel, const std::vector<G2SCOPE>& scopes);
    virtual void on_g2search_saver_receive_clipcopy_cancel(G2HSEARCH handle, int channel);
    virtual void on_g2search_saver_receive_clipcopy_measure_size(G2HSEARCH handle, int channel);
    virtual void on_g2search_saver_receive_clipcopy_size(G2HSEARCH handle, int channel, signed char status, unsigned __int64 size, const G2TIME& begin, const G2TIME& end);
    virtual void on_g2search_saver_receive_clipcopy_data(G2HSEARCH handle, int channel, bool completed, int progress, int count, int length, const unsigned char* data);
    virtual void on_g2search_saver_receive_clipcopy_password(G2HSEARCH handle, int channel);
    virtual void on_g2search_saver_receive_clipcopy_enable_channels(G2HSEARCH handle, int channel, unsigned int cameras);
    virtual void on_g2search_saver_receive_bank_space(G2HSEARCH handle, int channel, int image_index_num, int audio_index_num, const G2TIME& start, int start_msec, unsigned __int64 image_size, unsigned __int64 audio_size) {}
    virtual void on_g2search_saver_receive_bank_image(G2HSEARCH handle, int channel, const unsigned char* data) {}
    virtual void on_g2search_saver_receive_bank_audio(G2HSEARCH handle, int channel, const unsigned char* data) {}
    virtual void on_g2search_saver_receive_bank_no_image(G2HSEARCH handle, int channel) {}
    virtual void on_g2search_saver_receive_bank_no_audio(G2HSEARCH handle, int channel) {}

/// search_g2 listener //////////////////////////////////////////////////////////////////
protected:
    virtual void on_g2search_g2_saver_connected(G2HSEARCH_G2 handle, int channel) {}
    virtual void on_g2search_g2_saver_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON::TYPE reason);
    virtual void on_g2search_g2_saver_receive_frame_data(G2HSEARCH_G2 handle, int channel, const G2FRAME& frame) {}
    virtual void on_g2search_g2_saver_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE status) {}
    virtual void on_g2search_g2_saver_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, const G2ROLLBACK_INFO& rbi) {}
    virtual void on_g2search_g2_saver_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE error) {}
    virtual void on_g2search_g2_saver_receive_scope_list(G2HSEARCH_G2 handle, int channel, const std::vector<G2SCOPE>& scopes, G2SEARCH_G2_SCOPE_TYPE::TYPE type);
    virtual void on_g2search_g2_saver_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel) {}
    virtual void on_g2search_g2_saver_receive_clipcopy_size(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_STATUS::TYPE status, const G2CLIPCOPY_SIZE_INFO& csi, unsigned int progress);
    virtual void on_g2search_g2_saver_receive_clipcopy_data(G2HSEARCH_G2 handle, int channel, unsigned __int64 offset, unsigned int size, const unsigned char* data, unsigned int progress);
    virtual void on_g2search_g2_saver_receive_clipcopy_set_password(G2HSEARCH_G2 handle, int channel, unsigned int result);
    virtual void on_g2search_g2_saver_receive_clipcopy_canceled(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_ERROR::TYPE error);
    virtual void on_g2search_g2_saver_receive_clipcopy_job_started(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB::TYPE job, unsigned int num, unsigned int total);
    virtual void on_g2search_g2_saver_receive_clipcopy_job_finished(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB::TYPE job, unsigned int num, unsigned int total);
    virtual void on_g2search_g2_saver_receive_clipcopy_section_begin(G2HSEARCH_G2 handle, int channel, unsigned int num, unsigned int total) {}
    virtual void on_g2search_g2_saver_receive_clipcopy_section_end(G2HSEARCH_G2 handle, int channel, unsigned int num, unsigned int total) {}

//////////////////////////////////////////////////////////////////////////

protected:
	virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual void OnCancel();

	DECLARE_MESSAGE_MAP()

    afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnTimer(UINT_PTR nIDEvent);

    afx_msg void on_btn_start_stop(void);
    afx_msg void on_chk_first(void);
    afx_msg void on_chk_last(void);

    LRESULT on_post_disconnected(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_start_save_video(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_stop_save_video(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_canceled_clipcopy(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_complete_clipcopy(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_update_clipcopy(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_CLIP_COPY_DLG_H_
