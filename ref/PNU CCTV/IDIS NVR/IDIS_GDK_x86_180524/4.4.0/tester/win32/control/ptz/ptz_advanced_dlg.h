// ptz_advanced_dlg.h : header file
//

#ifndef _CONTROL_PTZ_ADVANCED_DLG_H_
#define _CONTROL_PTZ_ADVANCED_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define_live.h>

namespace client {
    class g2watch;

//////////////////////////////////////////////////////////////////////////

class ptz_advanced_dlg : public CDialog
{
public:
    ptz_advanced_dlg(CWnd* parent = NULL);
    virtual ~ptz_advanced_dlg(void);

protected:
    enum INDEXER_TYPE {
        INDEXER_TOUR = 0,
        INDEXER_PATTERN,
        INDEXER_SCAN,

        COUNTOF_INDEXER
    };

protected:
    CWnd*           _parent;
    g2watch*        _watch;
    short           _channel;
    short           _camera;

    bool            _activateCTRL;
    int             _commandId;

    G2LIVE_PTZ_MENU _menu;

    static BYTE _indexReposit[COUNTOF_INDEXER];

public:
    void set_menu(const G2LIVE_PTZ_MENU& menu);
    void set_connector(client::g2watch* watch, short channel, short camera);

protected:
    void init_menu(void);
    void apply_command(int cmd, int arg = 0);
    int secure_command(unsigned int id, bool activate = false);
    int secure_argument(unsigned int id);
    bool create_popup_menu(CMenu& menu, unsigned int id);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()

    afx_msg void on_menu_on_off(UINT nID);
    afx_msg void on_menu_index(UINT nID);
    afx_msg void on_btn_ptz(UINT nID);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_PTZ_ADVANCED_DLG_H_
