// event_functional.h : header file
//

#ifndef _COMMON_EVENT_FUNCTIONAL_H_
#define _COMMON_EVENT_FUNCTIONAL_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <common/client_define.h>
#include <include/g2_define.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

struct condition_event_info {
    int _type;
    int _event;
    std::wstring _string;

    condition_event_info(void)
        : _type(-1)
        , _event(-1) {
    }
    condition_event_info(int type, int evt, const std::wstring& string)
        : _type(type)
        , _event(evt)
        , _string(string) {
    }

    bool equal_type(int type) { return (_type == type); }
    bool equal_event(int event) { return (_event == event); }
};

//////////////////////////////////////////////////////////////////////////

inline bool secure_condition_event_info(int condition, condition_event_info& cei)
{
    std::swap(condition_event_info(), cei);

    if (condition <= event_condition_::UNDEFINED &&
        condition >= event_condition_::EVENT_COUNT) {
        return false;
    }
    struct { int _type; int _event; LPCTSTR _string; } event_element[] = {
        { event_condition_::ALARM_IN, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_ALARMIN_ON, L"Alarm In" },
        { event_condition_::MOTION_DETECTION, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_MOTION_DETECTION_ON, L"Motion Detection" },
        { event_condition_::VIDEO_LOSS, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_VIDEO_LOSS_ON, L"Video Loss" },
        { event_condition_::VIDEO_BLIND, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_VIDEO_BLIND_ON, L"Video Blind" },
        { event_condition_::OBJECT_DETECTION, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_OBJECT_DETECTION_ON, L"Object Detection" },
        { event_condition_::TRIPZONE, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_TRIPZONE_ON, L"TripZone"},
        { event_condition_::TAMPER, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_TAMPER_ON, L"Tamper" },
        { event_condition_::AUDIO_DETECTION, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_AUDIO_ON, L"Audio Detection" },
        { event_condition_::TEXT_IN, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_TEXT_IN_ON, L"Text In" },
        { event_condition_::NETWORK_ALARM_IN, G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_NETWORK_ALARMIN_ON, L"Network Alarm" }
    };

    for (int i = 0; i < _countof(event_element); ++i) {
        if (event_element[i]._type == condition) {
            cei._type   = event_element[i]._type;
            cei._event  = event_element[i]._event;
            cei._string = event_element[i]._string;
            break;
        }
    }

    return true;
}

inline int secure_event_from_event_condition(int condition)
{
    int evt = -1;
    switch (condition) {
        case event_condition_::ALARM_IN:         evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_ALARMIN_ON; break;
        case event_condition_::MOTION_DETECTION: evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_MOTION_DETECTION_ON; break;
        case event_condition_::VIDEO_LOSS:       evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_VIDEO_LOSS_ON; break;
        case event_condition_::VIDEO_BLIND:      evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_VIDEO_BLIND_ON; break;
        case event_condition_::OBJECT_DETECTION: evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_OBJECT_DETECTION_ON; break;
        case event_condition_::TRIPZONE:         evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_TRIPZONE_ON; break;
        case event_condition_::TAMPER:           evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_CAMERA_TAMPER_ON; break;
        case event_condition_::AUDIO_DETECTION:  evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_AUDIO_ON; break;
        case event_condition_::TEXT_IN:          evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_TEXT_IN_ON; break;
        case event_condition_::NETWORK_ALARM_IN: evt = G2EVENT_INFO::L1_DEVICE | G2EVENT_INFO::L2_NETWORK_ALARMIN_ON; break;
        default: assert(!"undefined event type"); break;
    }
    return evt;
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_COMMON_EVENT_FUNCTIONAL_H_
