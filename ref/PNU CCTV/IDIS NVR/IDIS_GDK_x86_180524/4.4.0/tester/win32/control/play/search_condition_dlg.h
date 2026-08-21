// search_condition_dlg.h : header file
//

#ifndef _CONTROL_SEARCH_CONDITION_DLG_H_
#define _CONTROL_SEARCH_CONDITION_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "search_condition_event_ctrl.h"
#include "search_condition_text_in_ctrl.h"

#include <include/g2_define_search.h>
#include <boost/shared_ptr.hpp>
#include <map>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class search_condition_dlg : public CDialog
{
public:
    search_condition_dlg(CWnd* parent = NULL);
    virtual ~search_condition_dlg(void);

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
    CWnd* _parent;
    
    bool _idr;

    G2SEARCH_QUERY::MODE _mode;
    unsigned int _cameras;
    G2TIME _begin;
    G2TIME _end;

    CButton _chkFirst;
    CButton _chkLast;
    CDateTimeCtrl _dtcFromDate;
    CDateTimeCtrl _dtcFromTime;
    CDateTimeCtrl _dtcToDate;
    CDateTimeCtrl _dtcToTime;
    CButton _rdoEvent;
    CButton _rdoTextIn;
    CTreeCtrl _tree;
    CStatic _stcCotnrol;

    search_condition_event_ctrl _eventCtrl;
    search_condition_text_in_ctrl _textInCtrl;

    G2EVENT_QUERY_CONDITION _conditionEvent;
    G2TEXT_IN_QUERY_CONDITION _conditionTextIn;

    CString                     _root;
    G2_PRODUCT_INFO             _productInfo;

    std::map<HTREEITEM, treeItemData>  _treeItems;

public:
    void set_condition_event(const G2EVENT_QUERY_CONDITION& condition);
    void set_condition_text_in(const G2TEXT_IN_QUERY_CONDITION& condition);
    void set_query_mode(G2SEARCH_QUERY::MODE mode);
    void set_query_cameras(unsigned int cameras);
    void set_device_info(const CString& root, const G2_PRODUCT_INFO& pi);
    void set_device_idr(bool idr);

    int get_query_mode(void) const { return _mode; }
    unsigned int get_query_cameras(void) const { return _cameras; }
    G2TIME get_time_begin(void) const { return _begin; }
    G2TIME get_time_end(void) const { return _end; }
    G2TIME get_time_begin_idr(void) const { return _begin; }
    void get_event_condition(G2EVENT_QUERY_CONDITION& condition);
    void get_text_in_condition(G2TEXT_IN_QUERY_CONDITION& condition);
    
private:
    void change_condition_mode(G2SEARCH_QUERY::MODE mode);
    void change_device_tree(G2SEARCH_QUERY::MODE mode);

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

#endif // !_CONTROL_SEARCH_CONDITION_DLG_H_
