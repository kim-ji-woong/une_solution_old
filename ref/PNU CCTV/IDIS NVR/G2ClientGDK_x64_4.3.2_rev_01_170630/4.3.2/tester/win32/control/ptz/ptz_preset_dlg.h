// ptz_preset_dlg.h : header file
//

#ifndef _CONTROL_PTZ_PRESET_DLG_H_
#define _CONTROL_PTZ_PRESET_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define_live.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class ptz_preset_dlg : public CDialog
{
public:
    ptz_preset_dlg(CWnd* parent = NULL, int mode = MODE_PRESET);
    virtual ~ptz_preset_dlg(void);

public:
    enum {
        MODE_UNDEFIND = -1,
        MODE_PRESET = 0,
        MODE_PREVIEW
    };

protected:
    enum {
        COLUMN_NUMBER = 0,
        COLUMN_TITLE
    };

    struct timer_ {
        struct id_ {
            enum ID {
                CHECK_VALID = 100
            };
        };
        struct elapse_ {
            enum VAL {
                CHECK_VALID = 100
            };
        };
    };

private:
    CWnd* _parent;

    int _mode;
    int _selected;

    G2LIVE_PTZ_PRESET _preset;

    CListCtrl _list;

public:
    int selected(void) const { return _selected; }
    void set_preset(const G2LIVE_PTZ_PRESET& preset) { _preset = preset; }
    G2LIVE_PTZ_PRESET preset(void) const { return _preset; }

protected:
    void init_preset(void);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()

    afx_msg void OnTimer(UINT_PTR nIDEvent);
    afx_msg void OnClickList(NMHDR *pNMHDR, LRESULT *pResult);
    afx_msg void OnOK();
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_PTZ_PRESET_DLG_H_
