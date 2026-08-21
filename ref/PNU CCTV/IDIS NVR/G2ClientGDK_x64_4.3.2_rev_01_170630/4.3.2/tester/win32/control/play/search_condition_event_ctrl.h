// search_condition_event_ctrl.h : header file
//

#ifndef _CONTROL_SEARCH_CONDITION_EVENT_CTRL_H_
#define _CONTROL_SEARCH_CONDITION_EVENT_CTRL_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define_search.h>
#include <boost/shared_ptr.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class search_condition_event_ctrl : public CDialog
{
public:
    search_condition_event_ctrl(CWnd* parent = NULL);
    virtual ~search_condition_event_ctrl(void);

private:
    struct event_ {
        enum TYPE {
            TYPE_CAMERA = 0,
            TYPE_SYSTEM
        };

        struct camera_ {
            enum TYPE {
                CAMERA_ALARM_IN = 0,
                CAMERA_MOTION,
                CAMERA_OBJECT,
                CAMERA_VLOSS,
                CAMERA_BLIND,
                CAMERA_TRIPZONE,
                CAMERA_TAMPER,
                CAMERA_VANALYTICS,
                CAMERA_RECORD,
                CAMERA_TEXT_IN,
                CAMERA_AUDIO,

                CAMERA_ENDOF
            };
        };

        struct system_ {
            enum TYPE {
                SYSTEM_RECORDER_HEALTH = camera_::CAMERA_ENDOF,
                SYSTEM_ALARM_IN_HEALTH,
                SYSTEM_DISK_FULL,
                SYSTEM_DISK_BAD,
                SYSTEM_DISK_TEMPERATURE,
                SYSTEM_DISK_SMART,
                SYSTEM_DISK_ALMOST_FULL,
                SYSTEM_DISK_OFF,
                SYSTEM_DISK_CONFIG_CHANGED,
                SYSTEM_PANIC_RECORD,
                SYSTEM_FAN_ERROR,

                SYSTEM_ENDOF
            };
        };
    };

    struct control_ {
        enum ID {
            CHK_BEGIN = 1200
        };
    };

    struct element_ {
        int _id;
        int _systemId;
        CString _string;

        void set(int id, int systemId, const CString& string) {
            _id = id;
            _systemId = systemId;
            _string = string;
        }
    };

private:
    CWnd* _parent;

    CFont _font;
    CButton _buttons[255];

    G2EVENT_QUERY_CONDITION _condition;

    element_ _element[event_::system_::SYSTEM_ENDOF];

private:
    void initialize(void);
    void finalize(void);
    void to_events(std::vector<int>& target, const G2EVENT_QUERY_CONDITION& condition);
    
public:
    bool create(CWnd* parent, const CRect& rect, unsigned int id);
    void show(bool show);
    void set_query_condition(const G2EVENT_QUERY_CONDITION& condition);
    void get_query_condition(G2EVENT_QUERY_CONDITION& condition, unsigned int cameras);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL PreTranslateMessage(MSG* pMsg);
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_SEARCH_CONDITION_EVENT_CTRL_H_
