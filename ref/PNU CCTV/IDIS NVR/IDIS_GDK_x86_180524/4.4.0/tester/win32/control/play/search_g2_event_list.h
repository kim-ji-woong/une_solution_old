// search_g2_event_list.h : header file
//

#ifndef _CONTROL_SEARCH_G2_EVENT_LIST_H_
#define _CONTROL_SEARCH_G2_EVENT_LIST_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "search/search_common.h"
#include <include/g2_define_search.h>
#include <include/g2_define_search_g2.h>

namespace client {
    class listener_search_g2_event_list;

//////////////////////////////////////////////////////////////////////////

class search_g2_event_list : public CWnd
{
    DECLARE_DYNAMIC(search_g2_event_list)

public:
    search_g2_event_list(void);
    virtual ~search_g2_event_list(void);

protected:
    struct control_ {
        enum ID {
            EVENT_LIST_CTRL = 1200,
            BTN_NEXT
        };
    };

    struct timer_ {
        struct id_ {
            enum ID {
                LOAD_EVENT_IMAGE = 100
            };
        };
    };

    struct column_ {
        enum COL {
            EVENT = 0,
            CONTENT,
            TIME
        };
    };

    struct message_ {
        enum ID {
            UM_RECEIVE_RESULT_QUERY = (WM_USER + 100),
            UM_LIST_CLEAR
        };
    };

    struct item_data {
        int     _index;
        G2SPOT  _spot;
        item_data(int index, G2SPOT spot) 
            : _index(index)
            , _spot(spot) {
        }
    };

protected:
    CWnd* _parent;
    listener_search_g2_event_list* _listener;

    bool    _initialized;
    bool    _enable;
    short   _hostId;
    CFont   _font;

    CListCtrl   _list;;
    CButton     _btnNext;

    std::vector<G2EVENT>    _g2eventLog;

    g2::critical_section    _syncEvent;
    g2::critical_section    _syncTextIn;

public:
    bool create(CWnd* parent, const CRect& rect, unsigned int id);
    void set_listenr(listener_search_g2_event_list* listener);
    void set_host_id(short hostId);
    void set_enable(bool enable);
    void show(bool show);
    void focus(void);
    bool is_initialized(void) const { return _initialized; }
    bool is_show(void) const { return IsWindowVisible() == TRUE; }

    void insert(const std::vector<G2EVENT>& info, int mode = G2SEARCH_G2_QUERY::EVENT);
    void clear(bool post = false);

private:
    void initialize(void);
    void finalize(void);


protected:
    virtual void DoDataExchange(CDataExchange* pDX);

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnSize(UINT nType, int cx, int cy);
    afx_msg void OnEnable(BOOL bEnable);
    afx_msg void OnTimer(UINT_PTR nIDEvent);

    afx_msg void OnBtnNext(void);
    afx_msg void OnLstItemChanged(NMHDR* pNMHDR, LRESULT* pResult);

    LRESULT on_receive_result_query(WPARAM wParam, LPARAM lParam);
    LRESULT on_clear_list(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_SEARCH_G2_EVENT_LIST_H_
