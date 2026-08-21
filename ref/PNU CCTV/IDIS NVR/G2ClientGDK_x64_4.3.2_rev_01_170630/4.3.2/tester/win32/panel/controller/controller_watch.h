// controller_watch.h : header file
//

#ifndef _CONTROLLER_WATCH_H_
#define _CONTROLLER_WATCH_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "controller_base.h"

#include <sampler/cpp/g2client_guid.h>
#include <include/g2_define_live.h>

//////////////////////////////////////////////////////////////////////////

namespace client {
    class g2watch;

//////////////////////////////////////////////////////////////////////////

class controller_watch : public controller_base
{
public:
    controller_watch(void);
    ~controller_watch(void);

protected:
    CComboBox           _cbxFormat;
    CComboBox           _cbxColorEffect;
    CComboBox           _cbxAlarmout;

    short               _selcamera;
    short               _channel;
    short               _hostId;
    int                 _channelext;

    client::g2watch*    _watch;

    bool                _ptzMenuOn;
    int                 _presetMode;

protected:
    virtual void initialize(void);
    virtual void finalize(void);

public:
    virtual bool create(CWnd* parent);
    virtual void on_screen_layout_changed(int layout, int changed);

protected:
    void enable_ptz_control(char modePTZ);
    void enable_image_control(BOOL enabled);
    void enable_alarmout_control(BOOL enabled);
    unsigned int secure_command(unsigned int id);
    bool create_popup_menu(CMenu& menu, unsigned int id);

public:
    void receive_ptz_menu(short channel, const G2LIVE_PTZ_MENU& menu);
    void receive_ptz_preset(short channel, const G2LIVE_PTZ_PRESET& preset);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual BOOL PreTranslateMessage(MSG* pMsg) ;

    DECLARE_MESSAGE_MAP();

    afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);

    /////////////////////////////////////////////

    afx_msg void on_cbx_selchange_format(void);
    afx_msg void on_btn_prev_camera(void);
    afx_msg void on_btn_next_camera(void);
    afx_msg void on_btn_ptz_preset(UINT nID);
    afx_msg void on_btn_ptz_advanced(void);
    afx_msg void on_camera_ptz_direction(UINT nID);
    afx_msg void on_camera_ptz_control(UINT nID);
    afx_msg void on_cbx_selchange_color_effect_cmd(void);
    afx_msg void on_btn_color_effect_up(void);
    afx_msg void on_btn_color_effect_down(void);
    afx_msg void on_cbx_selchange_alarmout(void);
    afx_msg void on_btn_alarmout_on(void);
    afx_msg void on_btn_alarmout_off(void);

    /////////////////////////////////////////////

    LRESULT on_post_screen_camera_changed(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_receive_ptz_preset(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_receive_ptz_menu(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROLLER_WATCH_H_
