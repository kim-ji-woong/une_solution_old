// event_condition_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "event_condition_dlg.h"

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

enum { IDD = IDD_EVENT_CONDITION };

enum RESOURCE_ID {
    IDCC_CHK_EVENT = 1024
};

//////////////////////////////////////////////////////////////////////////

event_condition_dlg::event_condition_dlg(CWnd* parent /*=NULL*/)
    : CDialog(IDD, parent)
    , _parent(parent)
{

}

event_condition_dlg::~event_condition_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void event_condition_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(event_condition_dlg, CDialog)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

namespace {
    bool pred_equal(int left, int right) { return (left == right); }
}

//////////////////////////////////////////////////////////////////////////

BOOL event_condition_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    CRect rect;
    GetClientRect(&rect);
    rect.DeflateRect(10, 10);
    rect.bottom = rect.top + 25;

    CFont* font = GetFont();

    bool checked = true;
    client::condition_event_info cei;
    for (int i = 0; i < event_condition_::END_EVENT; ++i) {
        if (client::secure_condition_event_info(i, cei)) {
            checked = (_eventList.empty() ||
                       std::find_if(_eventList.begin(), _eventList.end(),
                                    boost::bind(&pred_equal, _1, cei._event)) != _eventList.end()) ? true : false;

            _chkEvent[i].Create(cei._string.c_str(),
                                WS_VISIBLE | WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | BS_CHECKBOX | BS_AUTOCHECKBOX,
                                rect,
                                this,
                                IDCC_CHK_EVENT + i);

            _chkEvent[i].SetFont(font);
            _chkEvent[i].SetCheck(checked ? BST_CHECKED : BST_UNCHECKED);

            rect.OffsetRect(0, 25);
        }
    }

    if (_parent != NULL &&
        ::IsWindow(_parent->GetSafeHwnd())) {
        CenterWindow(_parent);
    }

    return TRUE;
}

void event_condition_dlg::OnOK()
{
    std::vector<int>().swap(_eventList);

    for (int i = 0; i < event_condition_::END_EVENT; ++i) {
        if (_chkEvent[i].GetCheck() == BST_CHECKED) {
            _eventList.push_back(client::secure_event_from_event_condition(i));
        }
    }

    if (_eventList.empty()) {
        MessageBox(L"No event condition selected", NULL, MB_OK);
        return;
    }

    CDialog::OnOK();
}

//////////////////////////////////////////////////////////////////////////

void event_condition_dlg::set_event(const std::vector<int>& eventList)
{
    _eventList.clear();
    _eventList = eventList;
}
