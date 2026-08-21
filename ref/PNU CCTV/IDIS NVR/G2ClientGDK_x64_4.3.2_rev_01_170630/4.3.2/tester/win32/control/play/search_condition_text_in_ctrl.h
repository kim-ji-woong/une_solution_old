// search_condition_text_in_ctrl.h : header file
//

#ifndef _CONTROL_SEARCH_CONDITION_TEXT_IN_CTRL_H_
#define _CONTROL_SEARCH_CONDITION_TEXT_IN_CTRL_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define_search.h>
#include <boost/shared_ptr.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class search_condition_text_in_ctrl : public CDialog
{
public:
    search_condition_text_in_ctrl(CWnd* parent = NULL);
    virtual ~search_condition_text_in_ctrl(void);

public:
    struct const_ {
        struct max_ {
            enum DATA {
                COLUMN_VALUE = 80,
                LINE_VALUE = 5,
                NAME_VALUE_LENGTH = G2TEXT_IN_QUERY_CONDITION::MAX_NAME_VALUE_LENGTH,
                COLUMN_LENGTH = 2,
                LINE_LENGTH = 1,
                QUERY_ITEM_COUNT = 5,
            };
        };
    };

    class CFloatEdit : public CEdit {
    protected:
        LRESULT WindowProc(UINT message, WPARAM wParam, LPARAM lParam) {
            bool succeeded = true;
            if (message == WM_CHAR) {
                unsigned int code = (unsigned int)wParam;
                if (code == 8  /* Back space */ ||
                    code == 46 /* . */ ||
                    (code >= 48 && code <= 57)) {
                }
                else {
                    succeeded = false;
                }
            }
            return succeeded ? CEdit::WindowProc(message, wParam, lParam) : 0L;
        }
    };

private:
    CWnd* _parent;

    bool _idr;
    bool _matchWholeWord;
    bool _transationWise;

    int _columns[const_::max_::QUERY_ITEM_COUNT];
    int _lines[const_::max_::QUERY_ITEM_COUNT];
    CFloatEdit _value[const_::max_::QUERY_ITEM_COUNT];

private:
    void initialzie(void);
    void finalize(void);
    void change_operator(UINT nID);
    void to_query(G2TEXT_IN_QUERY_CONDITION& query) const;
    void from_query(const G2TEXT_IN_QUERY_CONDITION& query);

    bool valid_index(int index) const;

public:
    bool create(CWnd* parent, const CRect& rect, unsigned int id);
    void show(bool show);
    void set_device_idr(bool idr);
    void set_query_condition(const G2TEXT_IN_QUERY_CONDITION& condition);
    void get_query_condition(G2TEXT_IN_QUERY_CONDITION& condition);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL PreTranslateMessage(MSG* pMsg);
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()

    afx_msg void OnChangeOperator1(void);
    afx_msg void OnChangeOperator2(void);
    afx_msg void OnChangeOperator3(void);
    afx_msg void OnChangeOperator4(void);
    afx_msg void OnChangeColumn(void);
    afx_msg void OnChangeLine(void);
};

//////////////////////////////////////////////////////////////////////////

inline bool search_condition_text_in_ctrl::valid_index(int index) const
{
    return (index >= 0 &&
            index < const_::max_::QUERY_ITEM_COUNT);
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_SEARCH_CONDITION_TEXT_IN_CTRL_H_
