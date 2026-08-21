// search_g2_condition_dlg.h : header file
//

#ifndef _CONTROL_SEARCH_G2_CONDITION_DLG_H_
#define _CONTROL_SEARCH_G2_CONDITION_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "search_g2_condition_event_ctrl.h"
#include "search_g2_condition_text_in_ctrl.h"

#include <include/g2_define_search_g2.h>
#include <boost/shared_ptr.hpp>
#include <map>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class search_g2_condition_dlg : public CDialog
{
public:
    search_g2_condition_dlg(std::vector<G2EVENT::TYPE>& supported, CWnd* parent = NULL);
    virtual ~search_g2_condition_dlg(void);

private:
    struct control_ {
        enum ID {
            QUERY_CONTROL_EVENT = 1200,
            QUERY_CONTROL_TEXT_IN
        };
    };

    struct message_ {
        enum ID {
            TREE_CHANGE_CHECK = (WM_USER + 100)
        };
    };

    struct timer_ {
        struct id_ {
            enum ID {
                CHANGE_QUERY_MODE = 100
            };
        };
    };

    struct item_ {
        enum TYPE {
            ROOT = 0,
            CAMERA,
            ALARMIN,
            ALARMOUT,
            AUDIOIN,
            AUDIOOUT,
            TEXTIN
        };
    };
    
    typedef struct _treeItemData{
        unsigned char   _channel;
        item_::TYPE     _type;

        _treeItemData(unsigned char channel, item_::TYPE type)
            : _channel(channel)
            , _type(type)
        {
        }
    }treeItemData;

private:
    CWnd*                               _parent;

    G2SEARCH_G2_QUERY::MODE             _mode;
    G2CHANNEL_SET                       _cameras;
    G2TIME                              _begin;
    G2TIME                              _end;

    CButton                             _chkFirst;
    CButton                             _chkLast;
    CDateTimeCtrl                       _dtcFromDate;
    CDateTimeCtrl                       _dtcFromTime;
    CDateTimeCtrl                       _dtcToDate;
    CDateTimeCtrl                       _dtcToTime;
    CButton                             _rdoEvent;
    CButton                             _rdoTextIn;
    CTreeCtrl                           _tree;
    CStatic                             _stcCotnrol;

    search_g2_condition_event_ctrl      _eventCtrl;
    search_g2_condition_text_in_ctrl    _textInCtrl;

    
    G2SEARCH_G2_EVENT_SEARCH_OPTIONS    _eventOptions;
    G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS  _textInOptions;

    CString                             _root;
    G2_PRODUCT_INFO                     _productInfo;

    std::map<HTREEITEM, treeItemData>   _treeItems;
    std::vector<G2EVENT::TYPE>          _supported;

public:
    void set_condition_event(const G2SEARCH_G2_EVENT_SEARCH_OPTIONS& options);
    void set_condition_text_in(const G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& options);
    void set_query_mode(G2SEARCH_G2_QUERY::MODE mode);
    void set_query_cameras(G2CHANNEL_SET cameras);
    void set_device_info(const CString& root, const G2_PRODUCT_INFO& pi);

    int get_query_mode(void) const { return _mode; }
    G2CHANNEL_SET get_query_cameras(void) const { return _cameras; }
    G2TIME get_time_begin(void) const { return _begin; }
    G2TIME get_time_end(void) const { return _end; }
    void get_event_condition(G2SEARCH_G2_EVENT_SEARCH_OPTIONS& options);
    void get_text_in_condition(G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& options);
    
private:
    void change_condition_mode(G2SEARCH_G2_QUERY::MODE mode);
    void change_device_tree(G2SEARCH_G2_QUERY::MODE mode);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual void OnTimer(UINT_PTR nIDEvent);
    virtual BOOL OnInitDialog();
    virtual void OnOK();

    DECLARE_MESSAGE_MAP()

    afx_msg void OnNMClickTree(NMHDR* pNMHDR, LRESULT* pResult);

    afx_msg void on_chk_first(void);
    afx_msg void on_chk_last(void);
    afx_msg void on_select_mode(UINT nID);

    LRESULT on_tree_change_check(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_SEARCH_G2_CONDITION_DLG_H_
