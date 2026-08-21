// connective_status.cpp : implementation file
//

#include "stdafx.h"
#include "MainFrm.h"
#include "G2ClientDoc.h"
#include "G2ClientView.h"
#include "control/status/status_dlg.h"

#include <sampler/cpp/g2client_status.h>
#include <boost/lexical_cast.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::on_g2status_connected(G2HSTATUS handle, int channel)
{
    
    _status->request_status(channel);
    //_status->request_alive_check(channel);
    
    if( foundation_ptr()->get_status_viewer() != NULL) {
        client::StatusViewer* status_view = foundation_ptr()->get_status_viewer();
        status_view->_channel = channel;
        status_view->_reconnect = false;
        G2_PRODUCT_INFO& ref_pi = status_view->_productInfo;
        _status->get_product_info(channel, ref_pi);
    }
    
    G2RETURN_IF_FAIL(_status->safe_handle() == handle);

 
}

void CG2ClientView::on_g2status_disconnected(G2HSTATUS handle, int channel, G2DISCONNECT_REASON::TYPE reason)
{
    // TODO
    if( foundation_ptr()->get_status_viewer() != NULL) {
        client::StatusViewer* status_view = foundation_ptr()->get_status_viewer();
        status_view->_reconnect = true;
    }
}

void CG2ClientView::on_g2status_receive_status(G2HSTATUS handle, int channel, const G2MONITORING_DEVICE_STATUS& status)
{
    // TODO
    
    client::StatusViewer* status_view = foundation_ptr()->get_status_viewer();
    G2RETURN_IF_FAIL(status_view != NULL);

    COleDateTime begin((time_t)g2_time_to_time32_t(&status._record._begin));
    COleDateTime end((time_t)g2_time_to_time32_t(&status._record._end));
    
    
    CString bTime = _T("");
    CString eTime = _T("");

    bTime.Format(_T("%02d/%02d/%02d %02d:%02d:%02d")
        , begin.GetYear(),begin.GetMonth(),begin.GetDay()
        , begin.GetHour(),begin.GetMinute(),begin.GetSecond());
    eTime.Format(_T("%02d/%02d/%02d %02d:%02d:%02d")
        , end.GetYear(),end.GetMonth(),end.GetDay()
        , end.GetHour(),end.GetMinute(),end.GetSecond());
    
    CEdit* edit = (CEdit*)status_view->GetDlgItem(IDC_STATUS_EDT);
    edit->Clear();
    
    CString result(_T(""));
    result = parse_status(status);
    
    // Below adds status info to be printed on the static control
    // such as site info including site name, HW version, and SW version,
    // connection state info, the record time range, 
    // and the state of Remote Panic Recording support
    CString site_name(_T("Site Name : "));
    site_name += status_view->_productInfo.name._string;
    site_name += _T("    ");
    CString site_hw_version(_T("HW v"));
    site_hw_version += status_view->_productInfo.version_hw._string;
    site_hw_version += _T("    ");
    CString site_sw_version(_T("SW v"));
    site_sw_version += status_view->_productInfo.version_sw._string;
    site_sw_version += _T("    ");
    CString site_info(_T(""));
    site_info = site_name + site_hw_version + site_sw_version;
    CString connection(_T("Connection Status : "));
    switch(status._connection) {
        case G2MONITORING_DEVICE_STATUS::SESSION_STARTING :
            connection += _T("SESSION STARTING");
            break;

        case G2MONITORING_DEVICE_STATUS::SESSION_DISCONNECTED :
            connection += _T("SESSION DISCONNECTED");
                break;

        case G2MONITORING_DEVICE_STATUS::SESSION_CONNECTING :
            connection += _T("SESSION CONNECTING");
                break;

        case G2MONITORING_DEVICE_STATUS::SESSION_CONNECTED :
            connection += _T("SESSION CONNECTED");
                break;

        case G2MONITORING_DEVICE_STATUS::SESSION_DISCONNECTING :
            connection += _T("SESSION DISCONNECTING");
                break;
        default:
            break;
    }
    
    
    CString rec_time = _T("Recorded Time : ") + bTime + _T(" ~ ") + eTime;
    CString remote_pannic_support = (status._support_remote_panic_recording) 
                                  ? _T("Supports Remote Panic Recording")
                                  : _T("Not Support Remote Panic Recording");
    
    
    if( status_view != NULL) {
        
        status_view->SetDlgItemText(IDC_STATUS_EDT, result);
        status_view->SetDlgItemText(IDC_STATUS_STC_SITE
                                   , site_info + _T("\r\n")
                                   + connection + _T("\r\n")
                                   + rec_time + _T("\r\n")
                                   + remote_pannic_support);
    }

}

void CG2ClientView::on_g2status_receive_callback_event(G2HSTATUS handle, const G2EVENT_INFO& ei)
{
    // TODO
}

//////////////////////////////////////////////////////////////////////////

CString CG2ClientView::parse_status(const G2MONITORING_DEVICE_STATUS& status)
{
    client::StatusViewer* status_view = foundation_ptr()->get_status_viewer();
    G2RETURN_VAL_IF_FAIL(status_view != NULL, _T("ERROR OCCURRED"));
    
    // below are the count of supported devices
    int alarmInCount    = 0;
    int alarmOutCount   = 0;
    int cameraCount     = 0;
    int audioInCount    = 0;
    int audioOutCount   = 0;  
    int textInCount     = 0;

    // if hybrid use below
    int networkAlarmInCount     = 0;
    int networkAlarmOutCount    = 0;
    int networkCameraCount      = 0;
    int networkAudioInCount     = 0;
    int networkAudioOutCount    = 0;

    const G2_PRODUCT_INFO& pi = status_view->_productInfo;


    if (pi.basic_info.hybrid) {
        networkCameraCount = (int)pi.device_hybird.count_network_camera;
        networkAlarmInCount = (int)pi.device_hybird.count_network_alarm_in;
        networkAlarmOutCount = (int)pi.device_hybird.count_network_alarm_out;
        networkAudioInCount = (int)pi.device_hybird.count_network_audio_in;
        networkAudioOutCount = (int)pi.device_hybird.count_network_audio_out;

        cameraCount = (int)pi.device_hybird.count_camera;
        audioInCount = (int)pi.device_hybird.count_audio_in;
        audioOutCount = (int)pi.device_hybird.count_audio_out;
        alarmInCount = (int)pi.device_hybird.count_alarm_in;
        alarmOutCount = (int)pi.device_hybird.count_alarm_out;

    } else {
        cameraCount = (int)pi.device.count_camera;
        audioInCount = (int)pi.device.count_audio_in;
        audioOutCount = (int)pi.device.count_audio_out;
        alarmInCount = (int)pi.device.count_alarm_in;
        alarmOutCount = (int)pi.device.count_alarm_out;
        textInCount = (int)pi.device.count_text_in;
    }
    
    // Health - Cameras
    std::vector<CString> cameraHealth;  
    for (int i = 0; i < cameraCount + networkCameraCount; i++) {        
        switch((int)status._health._cameras[i]) {
            case G2MONITORING_DEVICE_STATUS::ABNORMAL :
                cameraHealth.push_back(_T("ABNORMAL"));
                break;
            case G2MONITORING_DEVICE_STATUS::NORMAL :
                cameraHealth.push_back(_T("NORMAL"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_SYSTEM:
                cameraHealth.push_back(_T("NOT_USE_SYSTEM"));
                break;
            default:
                continue;
                break;
        }
    }

    // Health - AlarmIn
    std::vector<CString> alarmInHealth;  
    for (int i = 0; i < cameraCount + networkCameraCount; i++) {        
        switch((int)status._health._sensors[i]) {
            case G2MONITORING_DEVICE_STATUS::ABNORMAL :
                alarmInHealth.push_back(_T("ABNORMAL"));
                break;
            case G2MONITORING_DEVICE_STATUS::NORMAL :
                alarmInHealth.push_back(_T("NORMAL"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_SYSTEM :
                alarmInHealth.push_back(_T("NOT_USE_SYSTEM"));
                break;
            default:
                continue;
                break;
        }
    }

    
    // Motion Event
    std::vector<CString> motion;  
    for (int i = 0; i < cameraCount + networkCameraCount; i++) {        
        switch((int)status._motion[i]) {
            case G2MONITORING_DEVICE_STATUS::EVENT :
                motion.push_back(_T("EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NO_EVENT :
                motion.push_back(_T("NO EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_EVENT :
                motion.push_back(_T("NOT USE EVENT"));
                break;
            default:
                continue;
                break;
        }
    }
    
    // AlarmIn Event
    std::vector<CString> alarmIn;  
    for (int i = 0; i < alarmInCount+networkAlarmInCount; i++) {        
        switch((int)status._alarm_in[i]) {
            case G2MONITORING_DEVICE_STATUS::EVENT :
                alarmIn.push_back(_T("EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NO_EVENT :
                alarmIn.push_back(_T("NO EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_EVENT :
                alarmIn.push_back(_T("NOT USE EVENT"));
                break;
            default:
                continue;
                break;
        }
    }
    // AlarmOut Event
    std::vector<CString> alarmOut;  
    for (int i = 0; i < alarmOutCount+networkAlarmOutCount; i++) {        
        switch((int)status._alarm_out[i]) {
            case G2MONITORING_DEVICE_STATUS::EVENT :
                alarmOut.push_back(_T("EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NO_EVENT :
                alarmOut.push_back(_T("NO EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_EVENT :
                alarmOut.push_back(_T("NOT USE EVENT"));
                break;
            default:
                continue;
                break;
        }
    }
    
    // TextIn Event
    std::vector<CString> textIn;  
    for (int i = 0; i < textInCount; i++) {        
        switch((int)status._text_in[i]) {
            case G2MONITORING_DEVICE_STATUS::EVENT :
                textIn.push_back(_T("EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NO_EVENT :
                textIn.push_back(_T("NO EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_EVENT :
                textIn.push_back(_T("NOT USE EVENT"));
                break;
            default:
                continue;
                break;
        }
    }
    
    // Object Event
    std::vector<CString> objectTrack;  
    for (int i = 0; i < cameraCount + networkCameraCount; i++) {        
        switch((int)status._object[i]) {
            case G2MONITORING_DEVICE_STATUS::EVENT :
                objectTrack.push_back(_T("EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NO_EVENT :
                objectTrack.push_back(_T("NO EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_EVENT :
                objectTrack.push_back(_T("NOT USE EVENT"));
                break;
            default:
                continue;
                break;
        }
    }
    
    // VideoBlind Event
    std::vector<CString> videoBlind;  
    for (int i = 0; i < cameraCount + networkCameraCount; i++) {        
        switch((int)status._video_blind[i]) {
            case G2MONITORING_DEVICE_STATUS::EVENT :
                videoBlind.push_back(_T("EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NO_EVENT :
                videoBlind.push_back(_T("NO EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_EVENT :
                videoBlind.push_back(_T("NOT USE EVENT"));
                break;
            default:
                continue;
                break;
        }
    }

    // VideoAnalysis Event
    std::vector<CString> videoAnalysis;  
    for (int i = 0; i < cameraCount + networkCameraCount; i++) {        
        switch((int)status._video_analysis[i]) {
            case G2MONITORING_DEVICE_STATUS::EVENT :
                videoAnalysis.push_back(_T("EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NO_EVENT :
                videoAnalysis.push_back(_T("NO EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_EVENT :
                videoAnalysis.push_back(_T("NOT USE EVENT"));
                break;
            default:
                continue;
                break;
        }
    }

    // Tripzone Event
    std::vector<CString> tripzone;  
    for (int i = 0; i < cameraCount + networkCameraCount; i++) {        
        switch((int)status._trip_zone[i]) {
            case G2MONITORING_DEVICE_STATUS::EVENT :
                tripzone.push_back(_T("EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NO_EVENT :
                tripzone.push_back(_T("NO EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_EVENT :
                tripzone.push_back(_T("NOT USE EVENT"));
                break;
            default:
                continue;
                break;
        }
    }
    
    // Tampering Event
    std::vector<CString> tamper;  
    for (int i = 0; i < cameraCount + networkCameraCount; i++) {        
        switch((int)status._tamper[i]) {
            case G2MONITORING_DEVICE_STATUS::EVENT :
                tamper.push_back(_T("EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NO_EVENT :
                tamper.push_back(_T("NO EVENT"));
                break;
            case G2MONITORING_DEVICE_STATUS::NOT_USE_EVENT :
                tamper.push_back(_T("NOT USE EVENT"));
                break;
            default:
                continue;
                break;
        }
    }
    
    // putting infos grouped with proper camera number,
    // and make camera number vector index
    std::vector<CString> cameras;
    for(int i = 0; i< cameraCount + networkCameraCount; i++) {
        CString temp = _T("CAM");
        CString str;
        str.Format(_T("%d"),i);
        temp += str + _T(" : ");
        temp += _T("Cam=") + cameraHealth[i] + _T(" / ")
              + _T("AlarmIn=") + alarmInHealth[i] + _T(" / ")
              + _T("Motion=") + motion[i] + _T(" / ");
        if(i < alarmInCount+networkAlarmInCount) { temp += _T("AlarmInEvt=") +alarmIn[i]; }
        else { temp += _T(" "); }
        temp += _T(" / ");
        if(i < alarmOutCount+networkAlarmOutCount) { temp += _T("AlarmOutEvt=") + alarmOut[i]; }
        else { temp += _T(" "); }
        temp += _T(" / ");
        if (i < textInCount ) { temp += _T("TextIn=") + textIn[i]; }
        else { temp += _T(" "); }
        temp += _T(" / ");
        temp += _T("ObjectTrack=") + objectTrack[i] + _T(" / ")
              + _T("VideoBlind=") + videoBlind[i] + _T(" / ")
              + _T("VideoAnalysis=") + videoAnalysis[i] + _T(" / ")
              + _T("Tripzone=") + tripzone[i] + _T(" / ")
              + _T("Tamper=") + tamper[i] + _T(" / ");
        cameras.push_back(temp);
    }

    // put grouped information into a CString
    std::vector<CString>::iterator itr = cameras.begin();
    std::vector<CString>::iterator itrLast = cameras.end();
    
    CString result(_T(""));
    for ( ; itr != itrLast; ++itr) {
        result += *itr;
        result += _T("\r\n");
    }

    // belows adds more status info to result

    // Record - On
    struct record_info{
        CString _record;
        CString _panic;
        CString _archive;
        CString _clip;
        CString _play;
        record_info() :
             _record(_T(""))
            ,_panic(_T(""))
            ,_archive(_T(""))
            ,_clip(_T(""))
            ,_play(_T(""))
        {
            
        }
    } record;
    
    switch(status._record._on._record) {
        case G2MONITORING_DEVICE_STATUS::OFF :
            record._record =_T("OFF");
            break;

        case G2MONITORING_DEVICE_STATUS::ON :
            record._record =_T("ON");
            break;

        case G2MONITORING_DEVICE_STATUS::NOT_SUPPORT :
            record._record =_T("NOT SUPPORT");
            break;

        default:
            break;
    
    }

    switch(status._record._on._panic) {
        case G2MONITORING_DEVICE_STATUS::OFF :
            record._panic =_T("OFF");
            break;

        case G2MONITORING_DEVICE_STATUS::ON :
            record._panic =_T("ON");
            break;

        case G2MONITORING_DEVICE_STATUS::NOT_SUPPORT :
            record._panic =_T("NOT SUPPORT");
            break;

        default:
            break;

    }

    switch(status._record._on._archive) {
        case G2MONITORING_DEVICE_STATUS::OFF :
            record._archive =_T("OFF");
            break;

        case G2MONITORING_DEVICE_STATUS::ON :
            record._archive =_T("ON");
            break;
        
        case G2MONITORING_DEVICE_STATUS::NOT_SUPPORT :
            record._archive =_T("NOT SUPPORT");
            break;

        default:
            break;

    }

    switch(status._record._on._clip) {
        case G2MONITORING_DEVICE_STATUS::OFF :
            record._clip =_T("OFF");
            break;

        case G2MONITORING_DEVICE_STATUS::ON :
            record._clip =_T("ON");
            break;

        case G2MONITORING_DEVICE_STATUS::NOT_SUPPORT :
            record._clip =_T("NOT SUPPORT");
            break;

        default:
            break;

    }
    switch(status._record._on._play) {
        case G2MONITORING_DEVICE_STATUS::OFF :
            record._play =_T("OFF");
            break;

        case G2MONITORING_DEVICE_STATUS::ON :
            record._play =_T("ON");
            break;
        
        case G2MONITORING_DEVICE_STATUS::NOT_SUPPORT :
            record._play =_T("NOT SUPPORT");
            break;
        
        default:
            break;

    }
    CString record_info = _T("");
    record_info += _T("Record On/Off : ") + record._record + _T("\r\n")
                 + _T("Panic On/Off : ") + record._panic + _T("\r\n")
                 + _T("Archive On/Off : ") + record._archive + _T("\r\n")
                 + _T("Clip On/Off : ") + record._clip + _T("\r\n")
                 + _T("Play On/Off : ") + record._play + _T("\r\n");
    
    // Record - Mode
    CString record_mode = _T("Record Mode : ");
    switch(status._record._mode) {
        case G2MONITORING_DEVICE_STATUS::RECORD_MODE_ALL :
            record_mode += _T("RECORD MODE ALL");
            break;
        case G2MONITORING_DEVICE_STATUS::RECORD_MODE_NONE :
            record_mode += _T("RECORD MODE NONE");
            break;
        case G2MONITORING_DEVICE_STATUS::RECORD_MODE_RECORDING :
            record_mode += _T("RECORD MODE RECORDING");
            break;
        case G2MONITORING_DEVICE_STATUS::RECORD_MODE_PLAYBACK :
            record_mode += _T("RECORD MODE PLAYBACK");
            break;
        case G2MONITORING_DEVICE_STATUS::RECORD_MODE_CLIPCOPY :
            record_mode += _T("RECORD MODE CLIPCOPY");
            break;
        case G2MONITORING_DEVICE_STATUS::RECORD_MODE_PANIC_RECORDING :
            record_mode += _T("RECORD MODE PANIC RECORDING");
            break;
        default:
            break;
    }
    record_mode += _T("\r\n");



    // Health - Record
    CString record_health = _T("Record Health : ");
    switch(status._health._record) {
        case G2MONITORING_DEVICE_STATUS::ABNORMAL :
            record_health += _T("ABNORMAL");
            break;
        case G2MONITORING_DEVICE_STATUS::NORMAL :
            record_health += _T("ABNORMAL");
            break;
        case G2MONITORING_DEVICE_STATUS::NOT_USE_SYSTEM :
            record_health += _T("NOT USE SYSTEM");
            break;
        default :
            break;
    }
    record_health += _T("\r\n");


    result += record_info + record_mode + record_health;
    
    // finally returns
    return result;
}