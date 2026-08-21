// search_g2_condition_text_in_ctrl.cpp : implementation file
//

#include "stdafx.h"
#include "search_g2_condition_text_in_ctrl.h"
#include "include/g2_search_g2.h"

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

search_g2_condition_text_in_ctrl::search_g2_condition_text_in_ctrl(CWnd* parent /*=NULL*/)
    : CDialog(IDD_SEARCH_CONDITION_TEXT_IN, parent)
    , _parent(parent)
    , _matchWholeWord(false)
    , _transationWise(false)
{
    for (int i = 0; i < const_::max_::QUERY_ITEM_COUNT; ++i) {
        memset(&_columns[i], 0, sizeof(_columns[i]));
        memset(&_lines[i], 0, sizeof(_lines[i]));
    }
}

search_g2_condition_text_in_ctrl::~search_g2_condition_text_in_ctrl(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

void search_g2_condition_text_in_ctrl::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    for (int i = 0; i < const_::max_::QUERY_ITEM_COUNT; ++i) {
        DDX_Text(pDX, IDC_EDT_COLUMN_0 + i, _columns[i]);
        DDV_MinMaxInt(pDX, _columns[i], 0, const_::max_::COLUMN_VALUE);
        DDX_Text(pDX, IDC_EDT_LINE_0 + i, _lines[i]);
        DDV_MinMaxInt(pDX, _lines[i], 0, const_::max_::LINE_VALUE);
        DDX_Control(pDX, IDC_EDT_VALUE_0 + i, _value[i]);
    }
}

BEGIN_MESSAGE_MAP(search_g2_condition_text_in_ctrl, CDialog)
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

BOOL search_g2_condition_text_in_ctrl::PreTranslateMessage(MSG* pMsg)
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

BOOL search_g2_condition_text_in_ctrl::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialzie();

    return TRUE;
}

//////////////////////////////////////////////////////////////////////////

void search_g2_condition_text_in_ctrl::OnChangeOperator1(void)
{
    change_operator(IDC_CBX_OPERATOR_1);
}

void search_g2_condition_text_in_ctrl::OnChangeOperator2(void)
{
    change_operator(IDC_CBX_OPERATOR_2);
}

void search_g2_condition_text_in_ctrl::OnChangeOperator3(void)
{
    change_operator(IDC_CBX_OPERATOR_3);
}

void search_g2_condition_text_in_ctrl::OnChangeOperator4(void)
{
    change_operator(IDC_CBX_OPERATOR_4);
}

void search_g2_condition_text_in_ctrl::OnChangeColumn()
{
    UpdateData(TRUE);
    for (int i = 0; i < const_::max_::QUERY_ITEM_COUNT; i++) {
        if (_columns[i] > const_::max_::COLUMN_VALUE) {
            _columns[i] = const_::max_::COLUMN_VALUE;
            UpdateData(FALSE);
            break;
        }
    }
}

void search_g2_condition_text_in_ctrl::OnChangeLine()
{
    UpdateData(TRUE);
    for (int i = 0; i < const_::max_::QUERY_ITEM_COUNT; i++) {
        if (_lines[i] > const_::max_::LINE_VALUE) {
            _lines[i] = const_::max_::LINE_VALUE;
            UpdateData(FALSE);
            break;
        }
    }
}

//////////////////////////////////////////////////////////////////////////

bool search_g2_condition_text_in_ctrl::create(CWnd* parent, const CRect& rect, unsigned int id)
{
    assert(parent != NULL);

    bool result = false;
    if (result = (CDialog::Create(IDD_SEARCH_CONDITION_TEXT_IN, parent) == TRUE)) {
        MoveWindow(rect);
        SetDlgCtrlID(id);
    }
    return result;
}

void search_g2_condition_text_in_ctrl::show(bool show)
{
    ShowWindow(show ? SW_SHOW : SW_HIDE);
}

void search_g2_condition_text_in_ctrl::set_query_condition(const G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& condition)
{
    from_query(condition);
}

void search_g2_condition_text_in_ctrl::get_query_condition(G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& condition)
{
    to_query(condition);
}

//////////////////////////////////////////////////////////////////////////

void search_g2_condition_text_in_ctrl::initialzie(void)
{
    int i, id;
    CComboBox* cbx = NULL;
    for (i = 0; i < const_::max_::QUERY_ITEM_COUNT - 1; ++i) {
        id = IDC_CBX_OPERATOR_1 + i;

        if (cbx = (CComboBox*)GetDlgItem(id)) {
            cbx->AddString(_T(""));
            cbx->AddString(_T("AND"));
            cbx->AddString(_T("OR"));
            cbx->SetCurSel(0);
        }
        change_operator(IDC_CBX_OPERATOR_1);
    }

    for (i = 0; i < const_::max_::QUERY_ITEM_COUNT; ++i) {
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

    CheckDlgButton(IDC_CHK_MATCH_WORD, (_matchWholeWord) ? BST_CHECKED : BST_UNCHECKED);
    CheckDlgButton(IDC_CHK_TRANSACTION_WISE, (_transationWise) ? BST_CHECKED : BST_UNCHECKED);
}

void search_g2_condition_text_in_ctrl::finalize(void)
{

}

void search_g2_condition_text_in_ctrl::to_query(G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& query) const
{
    memset(&query, 0, sizeof(query));

    query._case_sensitive   = (IsDlgButtonChecked(IDC_CHK_CASE_SENSTIVE) == BST_CHECKED);
    query._match_whole_word = (IsDlgButtonChecked(IDC_CHK_MATCH_WORD) == BST_CHECKED);
    query._transaction_wise  = (IsDlgButtonChecked(IDC_CHK_TRANSACTION_WISE) == BST_CHECKED);

    int comparator, column, line;
    bool combi_and = true;
    CString name, value;
    CComboBox* cbx = NULL;

    int& index = query._condition_count;
    index = 0;
    
    for (int i = 0; i < const_::max_::QUERY_ITEM_COUNT; ++i) {
        if (i > 0) {
            if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_OPERATOR_1 + i - 1)) {
                if (cbx->GetCurSel() == 0) continue;
                else {
                    combi_and = (cbx->GetCurSel() == 1);
                }
            }
        }
        if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_COMP_0 + i)) {
            comparator = cbx->GetCurSel();
        }
        column = GetDlgItemInt(IDC_EDT_COLUMN_0 + i);
        line = GetDlgItemInt(IDC_EDT_LINE_0 + i);
        GetDlgItemText(IDC_EDT_NAME_0 + i, name);
        GetDlgItemText(IDC_EDT_VALUE_0 + i, value);

        G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION cond = { 0 };
        cond._combi_and = combi_and;
        cond._exp._op = comparator;
        cond._exp._value_column = column;
        cond._exp._value_line = line;
        swprintf_s(cond._exp._name._string, const_::max_::NAME_VALUE_LENGTH, L"%s", name);
        swprintf_s(cond._exp._value._string, const_::max_::NAME_VALUE_LENGTH, L"%s", value);

        if (g2_search_g2_text_in_search_options_condition_is_valid(&cond)) {
            query._condition[index] = cond;
            ++index;
        }
    }
}

void search_g2_condition_text_in_ctrl::from_query(const G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& query)
{
    CheckDlgButton(IDC_CHK_CASE_SENSTIVE, (query._case_sensitive != 0) ? BST_CHECKED : BST_UNCHECKED);

    _matchWholeWord = query._match_whole_word;
    _transationWise = query._transaction_wise;

    CComboBox* cbx = NULL;

    for (int i = 0; i < const_::max_::QUERY_ITEM_COUNT; ++i) {
        if ( i < query._condition_count) {
            const G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION& item = query._condition[i];

            if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_OPERATOR_1 + i - 1)) {
                cbx->SetCurSel(item._combi_and ? 1 : 2);
            }

            if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_COMP_0 + i)) {
                cbx->SetCurSel(item._exp._op);
            }
            
            SetDlgItemInt(IDC_EDT_COLUMN_0 + i, item._exp._value_column, TRUE);
            SetDlgItemInt(IDC_EDT_LINE_0 + i, item._exp._value_line, TRUE);
            SetDlgItemText(IDC_EDT_NAME_0 + i, CString(item._exp._name._string));
            SetDlgItemText(IDC_EDT_VALUE_0 + i, CString(item._exp._value._string));

            if (i > 0) {
                change_operator(IDC_CBX_OPERATOR_1 + i - 1);
            }
        }
        else {
            if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_OPERATOR_1 + i - 1)) {
                cbx->SetCurSel(0);
            }

            if (cbx = (CComboBox*)GetDlgItem(IDC_CBX_COMP_0 + i)) {
                cbx->SetCurSel(0);
            }

            SetDlgItemInt(IDC_EDT_COLUMN_0 + i, 0, TRUE);
            SetDlgItemInt(IDC_EDT_LINE_0 + i, 0, TRUE);
            SetDlgItemText(IDC_EDT_NAME_0 + i, _T(""));
            SetDlgItemText(IDC_EDT_VALUE_0 + i, _T(""));
        }
    }
}

void search_g2_condition_text_in_ctrl::change_operator(UINT nID)
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
