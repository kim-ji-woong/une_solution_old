// search_condition_text_in_ctrl.cpp : implementation file
//

#include "stdafx.h"
#include "search_condition_text_in_ctrl.h"

#include <algorithm>
#include <functional>
#include <boost/bind.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

search_condition_text_in_ctrl::search_condition_text_in_ctrl(CWnd* parent /*=NULL*/)
    : CDialog(IDD_SEARCH_CONDITION_TEXT_IN, parent)
    , _parent(parent)
    , _idr(false)
    , _matchWholeWord(false)
    , _transationWise(false)
{
    for (int i = 0; i < G2TEXT_IN_QUERY_CONDITION::MAX_ITEM_COUNT; ++i) {
        memset(&_columns[i], 0, sizeof(_columns[i]));
        memset(&_lines[i], 0, sizeof(_lines[i]));
    }
}

search_condition_text_in_ctrl::~search_condition_text_in_ctrl(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

void search_condition_text_in_ctrl::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    for (int i = 0; i < G2TEXT_IN_QUERY_CONDITION::MAX_ITEM_COUNT; ++i) {
        DDX_Text(pDX, IDC_EDT_COLUMN_0 + i, _columns[i]);
        DDV_MinMaxInt(pDX, _columns[i], 0, const_::max_::COLUMN_VALUE);
        DDX_Text(pDX, IDC_EDT_LINE_0 + i, _lines[i]);
        DDV_MinMaxInt(pDX, _lines[i], 0, const_::max_::LINE_VALUE);
        DDX_Control(pDX, IDC_EDT_VALUE_0 + i, _value[i]);
    }
}

BEGIN_MESSAGE_MAP(search_condition_text_in_ctrl, CDialog)
    ON_CBN_SELCHANGE(IDC_CBX_OPERATOR_1, OnChangeOperator1)
    ON_CBN_SELCHANGE(IDC_CBX_OPERATOR_2, OnChangeOperator2)
    ON_CBN_SELCHANGE(IDC_CBX_OPERATOR_3, OnChangeOperator3)
    ON_CBN_SELCHANGE(IDC_CBX_OPERATOR_4, OnChangeOperator4)
    ON_EN_CHANGE(IDC_EDT_COLUMN_0,       OnChangeColumn)
    ON_EN_CHANGE(IDC_EDT_COLUMN_1,       OnChangeColumn)
    ON_EN_CHANGE(IDC_EDT_COLUMN_2,       OnChangeColumn)
    ON_EN_CHANGE(IDC_EDT_COLUMN_3,       OnChangeColumn)
    ON_EN_CHANGE(IDC_EDT_COLUMN_4,       OnChangeColumn)
    ON_EN_CHANGE(IDC_EDT_LINE_0,         OnChangeLine)
    ON_EN_CHANGE(IDC_EDT_LINE_1,         OnChangeLine)
    ON_EN_CHANGE(IDC_EDT_LINE_2,         OnChangeLine)
    ON_EN_CHANGE(IDC_EDT_LINE_3,         OnChangeLine)
    ON_EN_CHANGE(IDC_EDT_LINE_4,         OnChangeLine)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL search_condition_text_in_ctrl::PreTranslateMessage(MSG* pMsg)
{
    bool result = false;
    if (pMsg->message == WM_KEYDOWN) {
        switch (pMsg->wParam) {
            case VK_ESCAPE:
            case VK_RETURN:
                result = true;
                break;
            default:
                break;
        }
    }
    return (result) ? TRUE : CDialog::PreTranslateMessage(pMsg);
}

BOOL search_condition_text_in_ctrl::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialzie();

    return TRUE;
}

//////////////////////////////////////////////////////////////////////////

void search_condition_text_in_ctrl::OnChangeOperator1(void)
{
    change_operator(IDC_CBX_OPERATOR_1);
}

void search_condition_text_in_ctrl::OnChangeOperator2(void)
{
    change_operator(IDC_CBX_OPERATOR_2);
}

void search_condition_text_in_ctrl::OnChangeOperator3(void)
{
    change_operator(IDC_CBX_OPERATOR_3);
}

void search_condition_text_in_ctrl::OnChangeOperator4(void)
{
    change_operator(IDC_CBX_OPERATOR_4);
}

void search_condition_text_in_ctrl::OnChangeColumn()
{
    UpdateData(TRUE);
    for (int i = 0; i < G2TEXT_IN_QUERY_CONDITION::MAX_ITEM_COUNT; i++) {
        if (_columns[i] > const_::max_::COLUMN_VALUE) {
            _columns[i] = const_::max_::COLUMN_VALUE;
            UpdateData(FALSE);
            break;
        }
    }
}

void search_condition_text_in_ctrl::OnChangeLine()
{
    UpdateData(TRUE);
    for (int i = 0; i < G2TEXT_IN_QUERY_CONDITION::MAX_ITEM_COUNT; i++) {
        if (_lines[i] > const_::max_::LINE_VALUE) {
            _lines[i] = const_::max_::LINE_VALUE;
            UpdateData(FALSE);
            break;
        }
    }
}

//////////////////////////////////////////////////////////////////////////

bool search_condition_text_in_ctrl::create(CWnd* parent, const CRect& rect, unsigned int id)
{
    assert(parent != NULL);

    bool result = false;
    if (result = (CDialog::Create(IDD_SEARCH_CONDITION_TEXT_IN, parent) == TRUE)) {
        MoveWindow(rect);
        SetDlgCtrlID(id);
    }
    return result;
}

void search_condition_text_in_ctrl::show(bool show)
{
    ShowWindow(show ? SW_SHOW : SW_HIDE);
}

void search_condition_text_in_ctrl::set_device_idr(bool idr)
{
    _idr = idr;
}

void search_condition_text_in_ctrl::set_query_condition(const G2TEXT_IN_QUERY_CONDITION& condition)
{
    from_query(condition);
}

void search_condition_text_in_ctrl::get_query_condition(G2TEXT_IN_QUERY_CONDITION& condition)
{
    to_query(condition);
}

//////////////////////////////////////////////////////////////////////////

void search_condition_text_in_ctrl::initialzie(void)
{
    int i, id;
    CComboBox* cbx = NULL;
    for (i = 0; i < G2TEXT_IN_QUERY_CONDITION::MAX_ITEM_COUNT - 1; ++i) {
        id = IDC_CBX_OPERATOR_1 + i;

        if (cbx = (CComboBox*)GetDlgItem(id)) {
            cbx->AddString(_T(""));
            cbx->AddString(_T("AND"));
            cbx->AddString(_T("OR"));
            cbx->SetCurSel(0);
        }
        change_operator(IDC_CBX_OPERATOR_1);
    }

    for (i = 0; i < G2TEXT_IN_QUERY_CONDITION::MAX_ITEM_COUNT; ++i) {
        id = IDC_CBX_COMP_0 + i;
        
        if (cbx = (CComboBox*)GetDlgItem(id)) {
            cbx->AddString(_T(""));
            cbx->AddString(_T("<"));
            cbx->AddString(_T("<="));
            cbx->AddString(_T("="));
            cbx->AddString(_T(">="));
            cbx->AddString(_T(">"));
        }

        GetDlgItem(IDC_EDT_NAME_0 + i)->SendMessage(EM_SETLIMITTEXT, const_::max_::NAME_VALUE_LENGTH);
        GetDlgItem(IDC_EDT_VALUE_0 + i)->SendMessage(EM_SETLIMITTEXT, const_::max_::NAME_VALUE_LENGTH);
        GetDlgItem(IDC_EDT_COLUMN_0 + i)->SendMessage(EM_SETLIMITTEXT, const_::max_::COLUMN_LENGTH);
        GetDlgItem(IDC_EDT_LINE_0 + i)->SendMessage(EM_SETLIMITTEXT, const_::max_::LINE_LENGTH);
    }

    if (_idr) {
        GetDlgItem(IDC_CHK_MATCH_WORD)->EnableWindow(FALSE);
        GetDlgItem(IDC_CHK_TRANSACTION_WISE)->EnableWindow(FALSE);
    }
    else {
        CheckDlgButton(IDC_CHK_MATCH_WORD, (_matchWholeWord) ? BST_CHECKED : BST_UNCHECKED);
        CheckDlgButton(IDC_CHK_TRANSACTION_WISE, (_transationWise) ? BST_CHECKED : BST_UNCHECKED);
    }
}

void search_condition_text_in_ctrl::finalize(void)
{

}

void search_condition_text_in_ctrl::to_query(G2TEXT_IN_QUERY_CONDITION& query) const
{
    memset(&query, 0, sizeof(query));

    query._case_sensitive   = (IsDlgButtonChecked(IDC_CHK_CASE_SENSTIVE) == BST_CHECKED);
    query._match_whole_word = (IsDlgButtonChecked(IDC_CHK_MATCH_WORD) == BST_CHECKED);
    query._transaction_wise = (IsDlgButtonChecked(IDC_CHK_TRANSACTION_WISE) == BST_CHECKED);

    int count = 1;
    for (int i = 0; i < G2TEXT_IN_QUERY_CONDITION::MAX_ITEM_COUNT - 1; ++i) {
        CComboBox* cbx = (CComboBox*)GetDlgItem(IDC_CBX_OPERATOR_1 + i);
        if (cbx && cbx->GetCurSel() > 0) {
            ++count;
        }
    }
    query._item_count = count;

    int condition, comparator, column, line;
    CString name, value;
    CComboBox* cbx = NULL;
    
    for (int i = 0; i < G2TEXT_IN_QUERY_CONDITION::MAX_ITEM_COUNT; ++i) {
        if (i > 0) {
            if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_OPERATOR_1 + i - 1)) {
                condition = cbx->GetCurSel();
            }
        }
        else {
            condition = 0;
        }

        if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_COMP_0 + i)) {
            comparator = cbx->GetCurSel();
        }

        column = GetDlgItemInt(IDC_EDT_COLUMN_0 + i);
        line = GetDlgItemInt(IDC_EDT_LINE_0 + i);

        GetDlgItemText(IDC_EDT_NAME_0 + i, name);
        GetDlgItemText(IDC_EDT_VALUE_0 + i, value);

        query._item[i]._condition = condition;
        query._item[i]._comparator = comparator;
        query._item[i]._column = column;
        query._item[i]._line = line;

        sprintf_s(query._item[i]._name, const_::max_::NAME_VALUE_LENGTH, "%S", name);
        sprintf_s(query._item[i]._value, const_::max_::NAME_VALUE_LENGTH, "%S", value);
    }
}

void search_condition_text_in_ctrl::from_query(const G2TEXT_IN_QUERY_CONDITION& query)
{
    CheckDlgButton(IDC_CHK_CASE_SENSTIVE, (query._case_sensitive != 0) ? BST_CHECKED : BST_UNCHECKED);

    _matchWholeWord = query._match_whole_word;
    _transationWise = query._transaction_wise;

    CComboBox* cbx = NULL;

    for (int i = 0; i < G2TEXT_IN_QUERY_CONDITION::MAX_ITEM_COUNT; ++i) {
        const G2TEXT_IN_QUERY_CONDITION::ITEM_TYPE& item = query._item[i];

        if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_OPERATOR_1 + i - 1)) {
            cbx->SetCurSel(item._condition);
        }

        if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_COMP_0 + i)) {
            cbx->SetCurSel(item._comparator);
        }

        SetDlgItemInt(IDC_EDT_COLUMN_0 + i, item._column, TRUE);
        SetDlgItemInt(IDC_EDT_LINE_0 + i, item._line, TRUE);
        SetDlgItemText(IDC_EDT_NAME_0 + i, CString(item._name));
        SetDlgItemText(IDC_EDT_VALUE_0 + i, CString(item._value));

        if (i > 0) {
            change_operator(IDC_CBX_OPERATOR_1 + i - 1);
        }
    }
}

void search_condition_text_in_ctrl::change_operator(UINT nID)
{
    CComboBox* cbx = (CComboBox*)GetDlgItem(nID);
    G2RETURN_IF_FAIL(cbx != NULL);

    BOOL enable = (cbx->GetCurSel() > 0) ? TRUE : FALSE;

    GetDlgItem(IDC_EDT_NAME_1   + nID - IDC_CBX_OPERATOR_1)->EnableWindow(enable);
    GetDlgItem(IDC_CBX_COMP_1   + nID - IDC_CBX_OPERATOR_1)->EnableWindow(enable);
    GetDlgItem(IDC_EDT_VALUE_1  + nID - IDC_CBX_OPERATOR_1)->EnableWindow(enable);
    GetDlgItem(IDC_EDT_COLUMN_1 + nID - IDC_CBX_OPERATOR_1)->EnableWindow(enable);
    GetDlgItem(IDC_EDT_LINE_1   + nID - IDC_CBX_OPERATOR_1)->EnableWindow(enable);
}
