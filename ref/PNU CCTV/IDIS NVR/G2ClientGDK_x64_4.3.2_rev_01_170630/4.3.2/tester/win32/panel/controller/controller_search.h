// controller_search.h : header file
//

#ifndef _CONTROLLER_SEARCH_H_
#define _CONTROLLER_SEARCH_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "controller_base.h"
#include "control/play/search_event_progress.h"
#include "control/play/listener/listener_time_table.h"
#include "control/play/listener/listener_search_event_list.h"
#include <include/g2_define_search.h>
#include <boost/shared_ptr.hpp>

namespace client {
    class g2search;
    class time_table_base;
    class search_event_list;

//////////////////////////////////////////////////////////////////////////

class controller_search : public controller_base
                        , public listener_time_table
                        , protected listener_search_event_list
{
public:
    controller_search(void);
    virtual ~controller_search(void);

protected:
    struct control_ {
        enum ID {
            TIME_TABLE_HOUR = 1200,
            TIME_TABLE_MINUTE,
            TIME_TABLE_MINUTE_IDR,

            TIME_TABLE_COUNT,

            EVENT_LIST = TIME_TABLE_COUNT
        };
    };

    struct default_ {
        struct playback_ {
            enum VAL {
                HEIGHT = 50
            };
        };

        struct speed_ {
            enum VAL {
                PLAY = 10,
                FAST = 40
            };
        };
    };

    struct mode_ {
        enum TYPE {
            TIMELAPSE = 0,
            EVENT
        };
    };

    struct buttons_ {
        enum TYPE {            
            MODE = 0,
            FILTER,
            CALENDAR,
            GO_TO,
            ADDITIONAL,
            SAVE,

            BEGIN_PLAYBACK,
            REW = BEGIN_PLAYBACK,
            PREV,
            STOP,
            PLAY,
            NEXT,
            FF,
            END_PLAYBACK,

            LAYOUT_PREV = END_PLAYBACK,
            LAYOUT_NEXT,

            COUNT
        };
    };

    struct shuttle_ {
        enum TYPE {
            PLAY = 0,
            FAST,
            COUNT
        };
    };

protected:
    CSliderCtrl _sldShuttle[shuttle_::COUNT];

    CStatic _stcSpeed;
    CComboBox _cbxFormat;

    CButton _buttons[buttons_::COUNT];

    search_event_progress _progress;

    bool _enable;

    client::g2search* _search;
    boost::shared_ptr<time_table_base> _timeTable[control_::TIME_TABLE_COUNT];
    boost::shared_ptr<search_event_list> _eventList;
    
    g2::critical_section _lock_data;

    struct controller_info {
        short   _selcamera;
        short   _hostId;
        short   _channel;
        int     _mode;
        int     _playSpeed;
        int     _fastSpeed;

        struct event {
            short _hostId;
        }
        _event;
        
        controller_info(void)
            : _selcamera(client::invalid_::CAMERA_NUMBER)
            , _hostId(client::invalid_::HOST_ID)
            , _channel(client::invalid_::CHANNEL)
            , _playSpeed(default_::speed_::PLAY)
            , _fastSpeed(default_::speed_::FAST) {
            _event._hostId = client::invalid_::HOST_ID;
        }

        void reset(void) {
            _selcamera = client::invalid_::CAMERA_NUMBER;
            _hostId = client::invalid_::HOST_ID;
            _channel = client::invalid_::CHANNEL;
            _playSpeed = default_::speed_::PLAY;
            _fastSpeed = default_::speed_::FAST;
            _event._hostId = client::invalid_::HOST_ID;
        }
    }
    _playbackInfo;

protected:
    void stop_impl(void);
    void enable_controls(int command);
    void change_slider_play_speed(int id);
    void set_play_speed(int command, int clientSpeed);
    void load_time_table(int channel);
    void change_shuttle_mode(int mode);
    void change_search_mode(int mode);
    void create_event_progress(short channel, const G2TIME& begin, const G2TIME& end);
    
public:
    void on_receive_no_frame_loaded(int channel);
    void on_receive_no_recorded_data_of_search_target(int channel, G2SEARCH_TARGET::TYPE target);
    void on_receive_finding_idr_event_time(int channel, const G2TIME& time);
    void on_receive_notify_play_speed_changed(int channel, int speed);
    void on_receive_notify_play_stop_post(int channel, G2SEARCH_DRIVE::MODE mode);
    void on_receive_event_query_result(int channel, const std::vector<G2SEARCH_LOG_INFO>& info);
    void on_receive_text_in_query_result(int channel, const std::vector<G2TEXT_IN>& data);
    void on_recieve_recorded_data_scope(int channel, const std::vector<G2SCOPE>& scopes);
    void on_receive_segment_spot(int channel, const std::vector<G2SPOT>& spots);
    void on_receive_external_tango_info(int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& info);
    void on_receive_prepare_reload(int channel);

    void screen_end_of_play(short hostId);
    bool is_enable(void) const { return _enable; }
    void set_enable(bool enable) { _enable = enable; }

    void load_site(int channel);
    void update_site(int channel);

    bool is_stopped(void);

public:
    virtual bool create(CWnd* parent);
    virtual void initialize(void);
    virtual void finalize(void);

public:
    virtual void on_screen_layout_changed(int layout, int changed);
    virtual void on_screen_image_drew(short camera, const G2SPOT& spot);
    virtual void on_changed_select_time_table(const G2SPOT& spot);

public:
    virtual void on_request_more_event_search(void);
    virtual void on_request_load_event_image_search(int selected, bool last);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual BOOL PreTranslateMessage(MSG* pMsg) ;

    DECLARE_MESSAGE_MAP();

    afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
    afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);

    afx_msg void on_btn_mode(void);
    afx_msg void on_btn_filter(void);
    afx_msg void on_btn_calendar(void);
    afx_msg void on_btn_go_to(void);
    afx_msg void on_btn_additional(void);
    afx_msg void on_btn_clipcopy(void);
    afx_msg void on_btn_rew(void);
    afx_msg void on_btn_prev(void);
    afx_msg void on_btn_stop(void);
    afx_msg void on_btn_play(void);
    afx_msg void on_btn_next(void);
    afx_msg void on_btn_ff(void);

    afx_msg void on_cbx_selchange_format(void);
    afx_msg void on_btn_layout_prev(void);
    afx_msg void on_btn_layout_next(void);

    afx_msg void on_goto_time(void);
    afx_msg void on_goto_first(void);
    afx_msg void on_goto_last(void);
    afx_msg void on_search_data_source(UINT nID);
    afx_msg void on_select_segment(void);

    /////////////////////////////////////////////

    LRESULT on_post_screen_camera_changed(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_receive_scope_list(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_receive_spot_list(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_receive_external_tango_list(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_disconnected(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_load_time_table(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_update_time_table(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROLLER_SEARCH_H_
