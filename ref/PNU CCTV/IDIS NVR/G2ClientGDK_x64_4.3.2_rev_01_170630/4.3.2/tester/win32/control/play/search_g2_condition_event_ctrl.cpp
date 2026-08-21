// search_g2_condition_event_ctrl.cpp : implementation file
//

#include "stdafx.h"
#include "search_g2_condition_event_ctrl.h"
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

search_g2_condition_event_ctrl::search_g2_condition_event_ctrl(CWnd* parent /*=NULL*/)
    : CDialog(IDD_SEARCH_CONDITION_EVENT, parent)
    , _parent(parent)
{
    _element[event_::camera_::CAMERA_ALARM_IN           ].set(event_::camera_::CAMERA_ALARM_IN,             G2EVENT::ALARM_IN_BAD_ON,       _T("Alarm In"));
    _element[event_::camera_::CAMERA_MOTION             ].set(event_::camera_::CAMERA_MOTION,               G2EVENT::MOTION_ON,             _T("Motion Detection"));
    _element[event_::camera_::CAMERA_OBJECT             ].set(event_::camera_::CAMERA_OBJECT,               G2EVENT::OBJECT_TRACK_ON,       _T("Object Detection"));
    _element[event_::camera_::CAMERA_VLOSS              ].set(event_::camera_::CAMERA_VLOSS,                G2EVENT::VIDEO_LOSS_ON,         _T("Video Loss"));
    _element[event_::camera_::CAMERA_TEXT_IN            ].set(event_::camera_::CAMERA_TEXT_IN,              G2EVENT::TEXT_IN_BAD_ON,        _T("Text In"));
    _element[event_::camera_::CAMERA_AUDIO              ].set(event_::camera_::CAMERA_AUDIO,                G2EVENT::AUDIO_ON,              _T("Audio Detection"));
    _element[event_::camera_::CAMERA_TRIPZONE           ].set(event_::camera_::CAMERA_TRIPZONE,             G2EVENT::TRIPZONE_ON,           _T("TripZone"));
    _element[event_::camera_::CAMERA_TAMPER             ].set(event_::camera_::CAMERA_TAMPER,               G2EVENT::TAMPER_ON,             _T("Tamper"));
    _element[event_::camera_::CAMERA_VANALYTICS         ].set(event_::camera_::CAMERA_VANALYTICS,           G2EVENT::VIDEO_ANALYTICS_ON,    _T("Video Analytics"));

    _element[event_::system_::SYSTEM_RECORDER_HEALTH    ].set(event_::system_::SYSTEM_RECORDER_HEALTH,      G2EVENT::RECORDER_BAD_ON,       _T("Record Bad"));
    _element[event_::system_::SYSTEM_ALARM_IN_HEALTH    ].set(event_::system_::SYSTEM_ALARM_IN_HEALTH,      G2EVENT::ALARM_IN_BAD_ON,       _T("Alarm In Bad"));
    _element[event_::system_::SYSTEM_DISK_FULL          ].set(event_::system_::SYSTEM_DISK_FULL,            G2EVENT::TANGO_FULL_ON,         _T("Disk Full"));
    _element[event_::system_::SYSTEM_DISK_BAD           ].set(event_::system_::SYSTEM_DISK_BAD,             G2EVENT::DISK_BAD,              _T("Disk Bad"));
    _element[event_::system_::SYSTEM_DISK_TEMPERATURE   ].set(event_::system_::SYSTEM_DISK_TEMPERATURE,     G2EVENT::DISK_TEMPERATURE_ON,   _T("Disk Temperature"));
    _element[event_::system_::SYSTEM_DISK_SMART         ].set(event_::system_::SYSTEM_DISK_SMART,           G2EVENT::DISK_SMART_ON,         _T("Disk Smart"));
    _element[event_::system_::SYSTEM_DISK_ALMOST_FULL   ].set(event_::system_::SYSTEM_DISK_ALMOST_FULL,     G2EVENT::TANGO_ALMOST_FULL_ON,  _T("Disk Almost Full"));
    _element[event_::system_::SYSTEM_DISK_OFF           ].set(event_::system_::SYSTEM_DISK_OFF,             G2EVENT::DISK_OFF,              _T("eSATA/iSCSI disconnected"));
    _element[event_::system_::SYSTEM_DISK_CONFIG_CHANGED].set(event_::system_::SYSTEM_DISK_CONFIG_CHANGED,  G2EVENT::DISK_CONFIG_CHANGE,    _T("Disk Config Change"));
    _element[event_::system_::SYSTEM_PANIC_RECORD       ].set(event_::system_::SYSTEM_PANIC_RECORD,         G2EVENT::PANIC_ON,              _T("Panic Record"));
    _element[event_::system_::SYSTEM_FAN_ERROR          ].set(event_::system_::SYSTEM_FAN_ERROR,            G2EVENT::FAN_ERROR_ON,          _T("Fan Error"));
}

search_g2_condition_event_ctrl::~search_g2_condition_event_ctrl(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

void search_g2_condition_event_ctrl::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(search_g2_condition_event_ctrl, CDialog)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL search_g2_condition_event_ctrl::PreTranslateMessage(MSG* pMsg)
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

BOOL search_g2_condition_event_ctrl::OnInitDialog()
{
    CDialog::OnInitDialog();
    initialize();

    unsigned int style = WS_VISIBLE | WS_CHILD | BS_AUTOCHECKBOX;
    unsigned int controlId = 0;
    int i, count, col = 2, row, eventId;
    row = _events.size() / 2 + _events.size() % 2;

    CRect rectClient;
    GetClientRect(&rectClient);

    std::vector<CRect> zones;
    client::rect_division(zones, rectClient, row, col, CRect(2, 2, 2, 2));

    for (i = 0, count = _events.size(); i < count; ++i) {
        eventId = _events[i];
        controlId = control_::CHK_BEGIN + eventId;
        CButton& button = _buttons[i];
        button.Create(_element[eventId]._string, style, zones[i], this, controlId);
        button.SetFont(&_font);
        button.SetCheck(BST_CHECKED);
        rectClient.OffsetRect(rectClient.Width(), 0);
    }
    return TRUE;
}

//////////////////////////////////////////////////////////////////////////

bool search_g2_condition_event_ctrl::create(CWnd* parent, const CRect& rect, unsigned int id, std::vector<G2EVENT::TYPE> supported)
{
    assert(parent != NULL);
    to_events(supported, _events);

    if ((CDialog::Create(IDD_SEARCH_CONDITION_EVENT, parent) == FALSE)) return false;
    
    MoveWindow(rect);
    SetDlgCtrlID(id);
    
    return true;
}

void search_g2_condition_event_ctrl::show(bool show)
{
    ShowWindow(show ? SW_SHOW : SW_HIDE);
}

void search_g2_condition_event_ctrl::set_query_condition(const G2SEARCH_G2_EVENT_SEARCH_OPTIONS& options)
{
}

void search_g2_condition_event_ctrl::get_query_condition(G2SEARCH_G2_EVENT_SEARCH_OPTIONS& options, const G2CHANNEL_SET& channels)
{
    int i, count, eventId;
    options._system_events._len = 0;

    for (i = 0, count = _events.size(); i < count; ++i) {
        eventId = _events[i];

        if (IsDlgButtonChecked(control_::CHK_BEGIN + eventId) == BST_CHECKED) {
            switch (eventId) {
                case event_::camera_::CAMERA_ALARM_IN:      options._alarm_in           = channels; break;
                case event_::camera_::CAMERA_MOTION:        options._motion             = channels; break;
                case event_::camera_::CAMERA_OBJECT:        options._object             = channels; break;
                case event_::camera_::CAMERA_VLOSS:         options._video_loss         = channels; break;
                case event_::camera_::CAMERA_TEXT_IN:       options._text_in            = channels; break;
                case event_::camera_::CAMERA_AUDIO:         options._audio_in           = channels; break;
                case event_::camera_::CAMERA_TRIPZONE:      options._trip_zone          = channels; break;
                case event_::camera_::CAMERA_TAMPER:        options._tamper             = channels; break;
                case event_::camera_::CAMERA_VANALYTICS:    options._video_analytics    = channels; break;

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
                    if (options._system_events._len < G2CHANNEL_SET::MAX_CHANNEL_COUNT)
                        options._system_events._channels[options._system_events._len++] = _element[eventId]._eventId;
                    break;
                default:
                    assert(!"undefined event id");
                    break;
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void search_g2_condition_event_ctrl::initialize(void)
{
    client::create_font(_font, 8);
}

void search_g2_condition_event_ctrl::finalize(void)
{
    if (_font.GetSafeHandle()) {
        _font.DeleteObject();
    }
}

void search_g2_condition_event_ctrl::to_events(const std::vector<G2EVENT::TYPE>& supported, std::vector<int>& target)
{
    std::vector<int>().swap(target);

    for (std::vector<G2EVENT::TYPE>::const_iterator itr = supported.begin();
        itr != supported.end();
        ++itr) {
        switch(*itr) {
        case G2EVENT::ALARM_IN_ON:             target.push_back(event_::camera_::CAMERA_ALARM_IN);              break;
        case G2EVENT::MOTION_ON:               target.push_back(event_::camera_::CAMERA_MOTION);                break;
        case G2EVENT::OBJECT_TRACK_ON:         target.push_back(event_::camera_::CAMERA_OBJECT);                break;
        case G2EVENT::VIDEO_LOSS_ON:           target.push_back(event_::camera_::CAMERA_VLOSS);                 break;
        case G2EVENT::TEXT_IN_BAD_ON:          target.push_back(event_::camera_::CAMERA_TEXT_IN);               break;
        case G2EVENT::AUDIO_ON:                target.push_back(event_::camera_::CAMERA_AUDIO);                 break;
        case G2EVENT::TRIPZONE_ON:             target.push_back(event_::camera_::CAMERA_TRIPZONE);              break;
        case G2EVENT::TAMPER_ON:               target.push_back(event_::camera_::CAMERA_TAMPER);                break;
        case G2EVENT::VIDEO_ANALYTICS_ON:      target.push_back(event_::camera_::CAMERA_VANALYTICS);            break;

        case G2EVENT::RECORDER_BAD_ON:         target.push_back(event_::system_::SYSTEM_RECORDER_HEALTH);       break;
        case G2EVENT::ALARM_IN_BAD_ON:         target.push_back(event_::system_::SYSTEM_ALARM_IN_HEALTH);       break;   
        case G2EVENT::TANGO_FULL_ON:           target.push_back(event_::system_::SYSTEM_DISK_FULL);             break;
        case G2EVENT::DISK_BAD:                target.push_back(event_::system_::SYSTEM_DISK_BAD);              break;
        case G2EVENT::DISK_TEMPERATURE_ON:     target.push_back(event_::system_::SYSTEM_DISK_TEMPERATURE);      break;
        case G2EVENT::DISK_SMART_ON:           target.push_back(event_::system_::SYSTEM_DISK_SMART);            break;
        case G2EVENT::TANGO_ALMOST_FULL_ON:    target.push_back(event_::system_::SYSTEM_DISK_ALMOST_FULL);      break;
        case G2EVENT::DISK_OFF:                target.push_back(event_::system_::SYSTEM_DISK_OFF);              break;
        case G2EVENT::DISK_CONFIG_CHANGE:      target.push_back(event_::system_::SYSTEM_DISK_CONFIG_CHANGED);   break;
        case G2EVENT::PANIC_ON:                target.push_back(event_::system_::SYSTEM_PANIC_RECORD);          break;
        case G2EVENT::FAN_ERROR_ON:            target.push_back(event_::system_::SYSTEM_FAN_ERROR);             break;
        }
    }
}
