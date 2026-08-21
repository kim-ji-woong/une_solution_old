// G2SearchControllerDlg.h : header file
//

#ifndef _G2SEARCH_CONTROLLER_DLG_H_
#define _G2SEARCH_CONTROLLER_DLG_H_

#if _MSC_VER > 1000
#pragma  once
#endif // _MSC_VER > 1000

#include "control/bitmap_button.h"
#include "control/slider_ctrl.h"
#include "control/play/listener/listener_time_table.h"

#include <include/g2_define_search_g2.h>
#include <boost/shared_ptr.hpp>
#include <vector>

namespace client {
    class g2search_g2;
    class time_table_minute;
    class message_box;

//////////////////////////////////////////////////////////////////////////

class G2SearchControllerDlg : public CDialog
                            , public listener_time_table
{
public:
	G2SearchControllerDlg(CWnd* parent = NULL);
    virtual ~G2SearchControllerDlg(void);

protected:
    struct move_    { enum STEP  { FRAME_1 = 0, SEC_10, MIN_01, MIN_05, MIN_10, MIN_15, MIN_30, HOUR_1, COUNT };};
    struct control_ { enum ID    { TIME_TABLE = 1024 };};
    struct timer_   { struct id_ { enum ID  { BALANCE_AUTO = 1200 };};
    struct elapse_  { enum VAL   { BALANCE_AUTO = 30 * 1000 };};};

    struct const_   { 
        enum TYPE  { 
            PLAYBACK_HEIGHT = 51, 
            PLAYBACK_TOP = 15, 
            CLIENT_WIDTH = 728, 
            CLIENT_HEIGHT = 128,
            CONTROLLER_BORDER = 3,
            CONTROLLOER_CAPTION = 22,
            CONTROLLER_WIDTH = CLIENT_WIDTH + (2 * CONTROLLER_BORDER),
            CONTROLLER_HEIGHT = CLIENT_HEIGHT + CONTROLLOER_CAPTION + CONTROLLER_BORDER,
        };
    };

    struct button_  { 
        enum ID { 
            PREV = 0, 
            MOVE,
            NEXT, 
            PLAY, 
            STOP, 
            CALENDAR,
            COUNT 
        };
        enum SIZE {
            MOVE_STEP_WIDTH = 34,
            MOVE_STEP_HEIGHT = 25,
            MOVE_BTN_WIDTH = 17,
            MOVE_BTN_HEIGHT = 25,
            PLAYSTOP_WIDTH = 27,
            PLAYSTOP_HEIGHT = 25,
            CALENDAR_WIDTH = 74,
            CALENDAR_HEIGHT = 25,
        };
        enum POS {
            TOP = const_::PLAYBACK_TOP,
            CALENDAR_LEFT = 185,
            PLAY_LEFT = 300,
            STOP_LEFT = 300,
            PREV_LEFT = 620,
            MOVE_LEFT = 638,
            NEXT_LEFT = 673,
        };
    };

    struct slider_ {
        enum RECT {
            LEFT = 330,
            TOP = const_::PLAYBACK_TOP,
            WIDTH = 220,
            HEIGHT = 25,
        };
    };

    struct text_ {
        enum RECT {
            DATE_WIDTH = 150,
            DATE_HEIGHT = 25,
            DATE_LEFT = 17,
            DATE_TOP = const_::PLAYBACK_TOP,

            SPEED_WIDTH = 40,
            SPEED_HEIGHT = 25,
            SPEED_LEFT = 550,
            SPEED_TOP = const_::PLAYBACK_TOP,

            INIT_WIDTH = 300,
            INIT_HEIGHT = 25,
            INIT_LEFT = (const_::CONTROLLER_WIDTH - INIT_WIDTH) / 2,
            INIT_TOP = (const_::CONTROLLER_HEIGHT / 2) - INIT_HEIGHT,
        };

        enum FONT {
            FONT_DATE_SIZE = 9,
            FONT_SPEED_SIZE = 8,
        };
    };


    struct controller_info {
        controller_info(void)
            : _channel(client::invalid_::CHANNEL)
            , _camera(client::invalid_::CHANNEL_EXT) 
        { 
            memset(&_playback, 0, sizeof(_playback)); 
        }

        void reset(void)
        {
            _channel    = client::invalid_::CHANNEL;
            _camera     = client::invalid_::CHANNEL_EXT;
            memset(&_playback, 0, sizeof(_playback));
        }

        int _channel;
        int _camera;
        G2PLAYBACK_COMMAND _playback;
    };

protected:
    boost::shared_ptr<client::g2search_g2> _adaptor;
    boost::shared_ptr<time_table_minute> _time_table;
    boost::shared_ptr<client::message_box> _message;

    controller_info _ci;
    int _move_step;
    bool _enable;
    bool _is_play;
    bool _is_top_most;
    bool _use_start_pos;
    CPoint _start_pos;

    g2::critical_section _cs_data;

protected:
    CStatic _stc_speed;
    CStatic _stc_date;
    CStatic _stc_init;
    CFont _font_data;
    CFont _font_speed;
    CBrush _brush;
    CBrush _brush_bkgnd;
    CBitmap _backgnd;
    CImageList _move_image_list[move_::COUNT];

    bitmap_button _btns[button_::COUNT];
    slider_ctrl _sld_speed;

public:
    client::message_box* message_box_ptr(void);
    void set_top_most(bool top_most) { _is_top_most = top_most; }
    void set_start_pos(const CPoint& start_pos) { _use_start_pos = true; _start_pos = start_pos; }

protected:
    void initialize(void);
    void finalize(void);

    void stop_impl(void);
    void enable_control(bool enable);
     void reset_speed(void);
    int  set_play_speed(int command, int client_speed = client::search::speed_::PLAY_SPEED_UNDEFINED);
    void request_speed_shuttle(void);
    void load_time_table(int channel);
    bool balance_time_table(void);
    bool button_show(button_::ID id, bool show);

public:
    virtual void on_screen_image_drew(const G2SPOT& spot, int channelext);
    virtual void on_changed_select_time_table(const G2SPOT& spot);

public:
    void on_receive_notify_command_begin_play(int channel, int command);
    void on_receive_notify_command_end_play(int channel, int command);
    void on_receive_notify_play_speed_changed_play(int channel, int speed);
    void on_receive_response_scope_list(int channel, const std::vector<G2SCOPE>& scopes, int type);
    void on_receive_response_spot_list(int channel, const std::vector<G2SPOT>& spots);

public:
    void request_stop_sync(void);
    void request_first(void);
    void request_last(void);
    void request_stop(void);

    void screen_end_of_play(int channel);
    bool is_origin_pos_speed(void);
    bool is_enable(void) const  { return _enable; }
    void set_enable(bool enble) { _enable = enble; }

    void load_site(int channel, int camera);
    void update_site(int channel);

    void set_init_message(const std::wstring& message);
    void show_screen_message(const std::wstring& string, bool autohide = true, int elapse = 5000);
    void show_screen_message(const std::wstring& string, const std::wstring& message, bool autohide = true, int elapse = 5000);
    void hide_screen_message(void);

    //////////////////////////////////////////////////////////////////////////

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy(void);
    afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
    afx_msg void OnTimer(UINT_PTR nIDEvent);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
    afx_msg void OnSize(UINT nType, int cx, int cy);

    afx_msg void on_btn_prev(void);
    afx_msg void on_btn_move_step(void);
    afx_msg void on_btn_next(void);
    afx_msg void on_btn_play(void);
    afx_msg void on_btn_stop(void);
    afx_msg void on_btn_calendar(void);
    afx_msg void on_sld_speed_release_capture(NMHDR* pNMHDR, LRESULT* pResult);
    afx_msg void on_move_step(UINT nID);
    
protected:
    LRESULT on_post_goto_first(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_goto_last(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_receive_scope_list(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_receive_spot_list(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_play_speed_changed(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_connected(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_disconnected(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_load_time_table(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_update_time_table(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_slider_changed(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2SEARCH_CONTROLLER_DLG_H_