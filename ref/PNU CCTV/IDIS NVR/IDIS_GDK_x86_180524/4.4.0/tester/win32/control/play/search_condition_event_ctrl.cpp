// search_condition_event_ctrl.cpp : implementation file
//

#include "stdafx.h"
#include "search_condition_event_ctrl.h"
#include "common/client_functional.h"

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

search_condition_event_ctrl::search_condition_event_ctrl(CWnd* parent /*=NULL*/)
    : CDialog(IDD_SEARCH_CONDITION_EVENT, parent)
    , _parent(parent)
{
    _element[event_::camera_::CAMERA_ALARM_IN].set(event_::camera_::CAMERA_ALARM_IN,     -1, _T("Alarm In"));
    _element[event_::camera_::CAMERA_MOTION].set(event_::camera_::CAMERA_MOTION,         -1, _T("Motion Detection"));
    _element[event_::camera_::CAMERA_OBJECT].set(event_::camera_::CAMERA_OBJECT,         -1, _T("Object Detection"));
    _element[event_::camera_::CAMERA_VLOSS].set(event_::camera_::CAMERA_VLOSS,           -1, _T("Video Loss"));
    _element[event_::camera_::CAMERA_BLIND].set(event_::camera_::CAMERA_BLIND,           -1, _T("Video Blind"));
    _element[event_::camera_::CAMERA_TRIPZONE].set(event_::camera_::CAMERA_TRIPZONE,     -1, _T("TripZone"));
    _element[event_::camera_::CAMERA_TAMPER].set(event_::camera_::CAMERA_TAMPER,         -1, _T("Tamper"));
    _element[event_::camera_::CAMERA_VANALYTICS].set(event_::camera_::CAMERA_VANALYTICS, -1, _T("Video Analytics"));
    _element[event_::camera_::CAMERA_RECORD].set(event_::camera_::CAMERA_RECORD,         -1, _T("Record"));
    _element[event_::camera_::CAMERA_TEXT_IN].set(event_::camera_::CAMERA_TEXT_IN,       -1, _T("Text In"));
    _element[event_::camera_::CAMERA_AUDIO].set(event_::camera_::CAMERA_AUDIO,           -1, _T("Audio Detection"));

    _element[event_::system_::SYSTEM_RECORDER_HEALTH].set(event_::system_::SYSTEM_RECORDER_HEALTH,          G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_RECORDER_BAD,         _T("Record Bad"));
    _element[event_::system_::SYSTEM_ALARM_IN_HEALTH].set(event_::system_::SYSTEM_ALARM_IN_HEALTH,          G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_ALARM_IN_BAD,         _T("Alarm In Bad"));
    _element[event_::system_::SYSTEM_DISK_FULL].set(event_::system_::SYSTEM_DISK_FULL,                      G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_TANGO_FULL,           _T("Disk Full"));
    _element[event_::system_::SYSTEM_DISK_BAD].set(event_::system_::SYSTEM_DISK_BAD,                        G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_DISK_BAD,             _T("Disk Bad"));
    _element[event_::system_::SYSTEM_DISK_TEMPERATURE].set(event_::system_::SYSTEM_DISK_TEMPERATURE,        G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_DISK_TEMPERATURE,     _T("Disk Temperature"));
    _element[event_::system_::SYSTEM_DISK_SMART].set(event_::system_::SYSTEM_DISK_SMART,                    G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_DISK_SMART,           _T("Disk Smart"));
    _element[event_::system_::SYSTEM_DISK_ALMOST_FULL].set(event_::system_::SYSTEM_DISK_ALMOST_FULL,        G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_TANGO_ALMOST_FULL,    _T("Disk Almost Full"));
    _element[event_::system_::SYSTEM_DISK_OFF].set(event_::system_::SYSTEM_DISK_OFF,                        G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_DISK_OFF,             _T("eSATA/iSCSI disconnected"));
    _element[event_::system_::SYSTEM_DISK_CONFIG_CHANGED].set(event_::system_::SYSTEM_DISK_CONFIG_CHANGED,  G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_DISK_CONFIG_CHANGE,   _T("Disk Config Change"));
    _element[event_::system_::SYSTEM_PANIC_RECORD].set(event_::system_::SYSTEM_PANIC_RECORD,                G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_PANIC,                _T("Panic Record"));
    _element[event_::system_::SYSTEM_FAN_ERROR].set(event_::system_::SYSTEM_FAN_ERROR,                      G2EVENT_QUERY_CONDITION::SYSTEM_EVENT_DC_FAN_ERROR,         _T("Fan Error"));
}

search_condition_event_ctrl::~search_condition_event_ctrl(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

void search_condition_event_ctrl::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    for (int i = 0; i < 5; ++i) {

    }
}

BEGIN_MESSAGE_MAP(search_condition_event_ctrl, CDialog)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL search_condition_event_ctrl::PreTranslateMessage(MSG* pMsg)
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

BOOL search_condition_event_ctrl::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialize();

    return TRUE;
}

//////////////////////////////////////////////////////////////////////////

bool search_condition_event_ctrl::create(CWnd* parent, const CRect& rect, unsigned int id)
{
    assert(parent != NULL);

    bool result = false;
    if (result = (CDialog::Create(IDD_SEARCH_CONDITION_EVENT, parent) == TRUE)) {
        MoveWindow(rect);
        SetDlgCtrlID(id);
    }
    return result;
}

void search_condition_event_ctrl::show(bool show)
{
    ShowWindow(show ? SW_SHOW : SW_HIDE);
}

void search_condition_event_ctrl::set_query_condition(const G2EVENT_QUERY_CONDITION& condition)
{
    _condition = condition;

    std::vector<int> events;
    to_events(events, _condition);

    unsigned int style = WS_VISIBLE | WS_CHILD | BS_AUTOCHECKBOX;
    unsigned int controlId = 0;
    int i, count, col = 2, row, eventId;
    row = events.size() / 2 + events.size() % 2;

    CRect rect;
    GetClientRect(&rect);

    std::vector<CRect> zones;
    client::rect_division(zones, rect, row, col, CRect(2, 2, 2, 2));

    for (i = 0, count = events.size(); i < count; ++i) {
        eventId = events[i];
        controlId = control_::CHK_BEGIN + eventId;
        CButton& button = _buttons[i];
        button.Create(_element[eventId]._string, style, zones[i], this, controlId);
        button.SetFont(&_font);
        button.SetCheck(BST_CHECKED);
        rect.OffsetRect(rect.Width(), 0);
    }
}

void search_condition_event_ctrl::get_query_condition(G2EVENT_QUERY_CONDITION& condition, unsigned int cameras)
{
    std::vector<int> events;
    to_events(events, _condition);

    int i, count, eventId;

    for (i = 0, count = events.size(); i < count; ++i) {
        eventId = events[i];

        if (IsDlgButtonChecked(control_::CHK_BEGIN + eventId) == BST_CHECKED) {
            switch (eventId) {
                case event_::camera_::CAMERA_ALARM_IN:      condition._alarm_in = cameras; break;
                case event_::camera_::CAMERA_MOTION:        condition._motion = cameras; break;
                case event_::camera_::CAMERA_OBJECT:        condition._object = cameras; break;
                case event_::camera_::CAMERA_VLOSS:         condition._video_loss = cameras; break;
                case event_::camera_::CAMERA_BLIND:         condition._video_blind = cameras; break;
                case event_::camera_::CAMERA_RECORD:        condition._record = cameras; break;
                case event_::camera_::CAMERA_TRIPZONE:      condition._trip_zone = cameras; break;
                case event_::camera_::CAMERA_TAMPER:        condition._tamper = cameras; break;
                case event_::camera_::CAMERA_VANALYTICS:    condition._video_analytics = cameras; break;
                case event_::camera_::CAMERA_TEXT_IN:       condition._text_in = cameras; break;
                case event_::camera_::CAMERA_AUDIO:         condition._audio_in = cameras; break;

                case event_::system_::SYSTEM_RECORDER_HEALTH:
                case event_::system_::SYSTEM_ALARM_IN_HEALTH:
                case event_::system_::SYSTEM_DISK_FULL:
                case event_::system_::SYSTEM_DISK_BAD:
                case event_::system_::SYSTEM_DISK_TEMPERATURE:
                case event_::system_::SYSTEM_DISK_SMART:
                case event_::system_::SYSTEM_DISK_ALMOST_FULL:
                case event_::system_::SYSTEM_DISK_OFF:
                case event_::system_::SYSTEM_DISK_CONFIG_CHANGED:
                case event_::system_::SYSTEM_PANIC_RECORD:
                case event_::system_::SYSTEM_FAN_ERROR:
                    g2::contains_bit(condition._system_events, _element[eventId]._systemId);
                    break;
                default:
                    assert(!"undefined event id");
                    break;
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void search_condition_event_ctrl::initialize(void)
{
    client::create_font(_font, 8);
}

void search_condition_event_ctrl::finalize(void)
{
    if (_font.GetSafeHandle()) {
        _font.DeleteObject();
    }
}

void search_condition_event_ctrl::to_events(std::vector<int>& target, const G2EVENT_QUERY_CONDITION& condition)
{
    std::vector<int>().swap(target);

    if (_condition._alarm_in != 0) {
        target.push_back(event_::camera_::CAMERA_ALARM_IN);
    }
    if (_condition._motion != 0) {
        target.push_back(event_::camera_::CAMERA_MOTION);
    }
    if (_condition._object != 0) {
        target.push_back(event_::camera_::CAMERA_OBJECT);
    }
    if (_condition._video_loss != 0) {
        target.push_back(event_::camera_::CAMERA_VLOSS);
    }
    if (_condition._video_blind != 0) {
        target.push_back(event_::camera_::CAMERA_BLIND);
    }
    if (_condition._record != 0) {
        target.push_back(event_::camera_::CAMERA_RECORD);
    }
    if (_condition._trip_zone != 0) {
        target.push_back(event_::camera_::CAMERA_TRIPZONE);
    }
    if (_condition._tamper != 0) {
        target.push_back(event_::camera_::CAMERA_TAMPER);
    }
    if (_condition._video_analytics != 0) {
        target.push_back(event_::camera_::CAMERA_VANALYTICS);
    }
    if (_condition._text_in != 0) {
        target.push_back(event_::camera_::CAMERA_TEXT_IN);
    }
    if (_condition._audio_in != 0) {
        target.push_back(event_::camera_::CAMERA_AUDIO);
    }
    for (int i = event_::system_::SYSTEM_RECORDER_HEALTH; i < event_::system_::SYSTEM_ENDOF; ++i) {
        if (g2::contains_bit(_condition._system_events, _element[i]._systemId)) {
            target.push_back(i);
        }
    }
}
