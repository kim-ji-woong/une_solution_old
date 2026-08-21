// connective_search.cpp : implementation file
//

#include "stdafx.h"
#include "MainFrm.h"
#include "G2ClientDoc.h"
#include "G2ClientView.h"

#include "search/search_data_manager.h"
#include "panel/controller/controller_search.h"
#include <sampler/cpp/g2client_search.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::on_g2search_connected(G2HSEARCH handle, int channel)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    connective_host_info_ptr hi = _hostList.get_host_info(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(hi != NULL);

    const short hostId = hi->host_id();
    if (client::valid_host_id(hostId)) {
        // success connection
    }
    else {
        _search->disconnect(channel);
        assert(!"occur broken artifact channel and hostId");
        return;
    }

    reset_screen_view(hostId, true, true);
    set_disconnect_by_me(hostId, false);

    screen_actuator& actuator = actuator_ref();
    actuator.set_activate_screen(true);

    G2_PRODUCT_INFO pi;
    _search->get_product_info(channel, pi);

    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);
    unsigned __int64 visible = actuator_ref().visible_camera();
    int maxcamera = pi.device.count_camera;
    std::wstring site = _hostList.site_name_from_host_id(hostId);

    //////////////////////////////////////////////////////////////////////////
    
    if (_cameraMap.count_cameras(hostId) == 1) {
        short scrcamera = g2::find_first_biton(cameras);
        if (_cameraMap.get_host_camera(scrcamera) == client::invalid_::CAMERA_NUMBER) {
            for (int i = 0; i < maxcamera; ++i) {
                CString title;
                title.Format(L"Camera %d", i + 1);
                foundation_ptr()->add_site_child(site.c_str(), title, i, true);

                if (g2::contains_bit(visible, scrcamera)) {
                    camera_map_element element(hostId, i,
                                               client::connect_mode_::SEARCH,
                                               client::invalid_::CHANNEL_EXT);
                    _cameraMap.set_camera_element(scrcamera++, element);
                }
            }
        }
    }
    cameras = _cameraMap.get_cameras_by_host_id(hostId);
    actuator.set_camera_mode(cameras, client::screen::camera_::SEARCH);

    //////////////////////////////////////////////////////////////////////////
    
    if (g2::countof_bits(cameras) > 0) {
        if (_search->get_search_target(channel) == G2SEARCH_TARGET::ARCHIVE) {
            if (_search->is_support(channel, G2SEARCH_SUPPORT::ARCHIVE)) {
                show_screen_message(_T("Remote Site does not support archive search."));
                _search->disconnect(channel);
                return;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////

    for (short i = 0; i < _cameraMap.size(); ++i) {
        if (_cameraMap.get_host_id(i) == hostId) {
            actuator.set_camera_status(i, screen::camera_status_::ENABLE, false);
        }
    }
    set_camera_list(hostId, false, true);

    //////////////////////////////////////////////////////////////////////////
    
    frame_buffer::PLAYTIME typetime = frame_buffer::TIME_MSEC;
    if (_search->is_support(channel, G2SEARCH_SUPPORT::BUFFER_USE_TICK)) {
        typetime = frame_buffer::TIME_TICK;
    }
    else if (_search->is_support(channel, G2SEARCH_SUPPORT::BUFFER_USE_SYSTEMMSEC)) {
        typetime = frame_buffer::TIME_SYSMSEC;
    }
    int driveMode = _search->get_drive_mode(channel);
    frame_buffer::TYPE typebuf = (driveMode == G2SEARCH_DRIVE::SAMBA) ? frame_buffer::TYPE_SEARCH_SAMBA :
                                 (driveMode == G2SEARCH_DRIVE::IDR  ) ? frame_buffer::TYPE_SEARCH_IDR :
                                                                        frame_buffer::TYPE_SEARCH;
    actuator.setup_buffer(hostId, typebuf, typetime);

    search_data_ptr data = search_data_manager::get().new_search_data(search::SEARCH_LOCAL, channel);
    search_data_manager::get().append_search_data(data);

    PostMessage(um_runner_::UM_CONNECTED_SEARCH, WPARAM(hostId), LPARAM(channel));
}

void CG2ClientView::on_g2search_disconnected(G2HSEARCH handle, int channel, G2DISCONNECT_REASON::TYPE reason)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    connective_host_info_ptr hi = _hostList.get_host_info(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(hi != NULL && client::valid_host_id(hi->host_id()));
    const short hostId = hi->host_id();

    //////////////////////////////////////////////////////////////////////////
    
    if (reason != G2DISCONNECT_REASON::LOGOUT) {
        G2STRING_128 sreason = { 0 };
        g2_get_string_disconnect_reason(reason, &sreason);

        std::wstring message = _hostList.site_name_from_host_id(hostId);
        message += L" : ";
        message += sreason._string;
        show_screen_message(L"Device disconnected.", message);
    }
    
    //////////////////////////////////////////////////////////////////////////
        
    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);
    screen_actuator& actuator = actuator_ref();

    //////////////////////////////////////////////////////////////////////////

    release_host_id(hostId, true, false);

    actuator.fire_disconnected();

    client::search_data_manager::get().remove_search_data(client::search::SEARCH_LOCAL, channel);

    PostMessage(um_runner_::UM_DISCONNECTED_SEARCH, WPARAM(hostId), LPARAM(channel));
}

void CG2ClientView::on_g2search_receive_recorded_date(G2HSEARCH handle, int channel, const std::vector<G2TIME>& dates)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    if (search_data_ptr data = search_data_manager::get().find_search_data_search(channel)) {
        data->set_date_info_dvr(dates);
    }

    PostMessage(um_runner_::UM_RECEIVE_RECORDED_DATE_SEARCH, WPARAM(channel));
}

void CG2ClientView::on_g2search_receive_recorded_time_hour(G2HSEARCH handle, int channel, const bool hour[][24], int segcount)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::search_data_ptr data = client::search_data_manager::get().find_search_data_search(channel)) {
        data->set_hour_info_dvr(hour, segcount);
    }

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->load_site(channel);
        controller->set_enable(true);
    }
}

void CG2ClientView::on_g2search_receive_recorded_time_minute(G2HSEARCH handle, int channel, const unsigned char minute[][24 * 60])
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::search_data_ptr data = client::search_data_manager::get().find_search_data_search(channel)) {
        data->set_minute_info_idr(minute);
    }

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->load_site(channel);
        controller->set_enable(true);
    }
}

void CG2ClientView::on_g2search_receive_recorded_rechour_minute(G2HSEARCH handle, int channel, const std::vector<G2RECORD_TIME_INFO>& rti)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::search_data_ptr data = client::search_data_manager::get().find_search_data_search(channel)) {
        for (std::vector<G2RECORD_TIME_INFO>::const_iterator itr(rti.begin());
             itr != rti.end();
             ++itr) {
            data->set_minute_info(*itr);
        }
    }

    /////////////////////////////////////////////

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->load_site(channel);
        controller->set_enable(true);
    }
}

void CG2ClientView::on_g2search_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    const int hostcamera = frame._index._channel;

    G2RETURN_IF_FAIL(client::valid_host_id(hostId));
    G2RETURN_IF_FAIL(client::valid_host_camera(hostcamera));

    screen_actuator& actuator = actuator_ref();
    if (frame._index._type == G2FRAME::AUDIO) {
        if (actuator.is_layout_1x1() != true) {
            return;
        }
    }

    unsigned __int64 refcameras = 0ui64;
    short basecamera = client::find_cameras_from_host_camera(hostId, hostcamera, refcameras);

    if (client::valid_camera(basecamera) != true) {
        return;
    }

    actuator.put_frame(G2FRAME::FROM_SEARCH, frame, hostId, basecamera, refcameras);
}

void CG2ClientView::on_g2search_receive_no_frame(G2HSEARCH handle, int channel)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_no_frame_loaded(channel);
    }

    screen_actuator& actuator = actuator_ref();
    actuator.set_search_end_play(G2FRAME::FROM_SEARCH, hostId);

    _endofplayHost.insert(hostId);
}

void CG2ClientView::on_g2search_receive_no_recorded_data(G2HSEARCH handle, int channel)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    show_screen_message(_T("There is no recorded image."));
}

void CG2ClientView::on_g2search_receive_no_recorded_data_from_search_target(G2HSEARCH handle, int channel, G2SEARCH_TARGET::TYPE target)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_no_recorded_data_of_search_target(channel, target);
    }
}

void CG2ClientView::on_g2search_receive_find_idr_event_time(G2HSEARCH handle, int channel, const G2TIME& time)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_finding_idr_event_time(channel, time);
    }
}

void CG2ClientView::on_g2search_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND speed)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (speed == G2PLAYER::NONE) {
        screen_actuator& actuator = actuator_ref();
        actuator.clear_frame_buffer_play(hostId);
    }

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_notify_play_speed_changed(channel, speed);
    }
}

void CG2ClientView::on_g2search_receive_notify_play_stop_post(G2HSEARCH handle, int channel, G2SEARCH_DRIVE::MODE mode)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_notify_play_stop_post(channel, mode);
    }
}

void CG2ClientView::on_g2search_receive_notify_end_of_play(G2HSEARCH handle, int channel)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    screen_actuator& actuator = actuator_ref();
    actuator.set_search_end_play(G2FRAME::FROM_SEARCH, hostId);

    _endofplayHost.insert(hostId);
}

void CG2ClientView::on_g2search_receive_notify_segment_changed(G2HSEARCH handle, int channel, int segment)
{

}

void CG2ClientView::on_g2search_receive_notify_search_mode_changed(G2HSEARCH handle, int channel, G2SEARCH_MODE::TYPE mode)
{

}

void CG2ClientView::on_g2search_receive_notify_command_end(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND command)
{

}

void CG2ClientView::on_g2search_receive_query_result_event(G2HSEARCH handle, int channel, const std::vector<G2SEARCH_LOG_INFO>& info)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_event_query_result(channel, info);
    }
}

void CG2ClientView::on_g2search_receive_query_result_text_in(G2HSEARCH handle, int channel, const std::vector<G2TEXT_IN>& data)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_text_in_query_result(channel, data);
    }
}

void CG2ClientView::on_g2search_receive_recorded_date_scope(G2HSEARCH handle, int channel, const std::vector<G2SCOPE>& scopes)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_recieve_recorded_data_scope(channel, scopes);
    }
}

void CG2ClientView::on_g2search_receive_segment_spot(G2HSEARCH handle, int channel, const std::vector<G2SPOT>& spots)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_segment_spot(channel, spots);
    }
}

void CG2ClientView::on_g2search_receive_error(G2HSEARCH handle, int channel)
{

}

void CG2ClientView::on_g2search_receive_text_in(G2HSEARCH handle, int channel, const G2TEXT_IN_ELEMENT& data)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    unsigned __int64 refcameras = 0ui64;
    int basecamera = client::find_cameras_from_host_camera(hostId, data._camera, refcameras);

    if (client::valid_camera(basecamera) != true) {
        return;
    }

    screen_actuator& actuator = actuator_ref();

    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(refcameras, i)) {
            actuator.put_text_in(hostId, i, data, _search->get_current_command(channel) == G2SEARCH_PLAYBACK::PREV);
        }
    }
}

void CG2ClientView::on_g2search_receive_external_tango_info(G2HSEARCH handle, int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& info)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_external_tango_info(channel, info);
    }
}

void CG2ClientView::on_g2search_receive_gps_data_start(G2HSEARCH handle, int channel)
{

}

void CG2ClientView::on_g2search_receive_gps_data(G2HSEARCH handle, int channel, const G2TEXT_IN& data)
{

}

void CG2ClientView::on_g2search_receive_gps_data_list(G2HSEARCH handle, int channel, const std::vector<G2TEXT_IN>& data)
{

}

void CG2ClientView::on_g2search_receive_gps_data_end(G2HSEARCH handle, int channel)
{

}

void CG2ClientView::on_g2search_receive_gps_data_end_count(G2HSEARCH handle, int channel, int total)
{

}

void CG2ClientView::on_g2search_receive_gps_data_export_cancel_result(G2HSEARCH handle, int channel, int result)
{

}

void CG2ClientView::on_g2search_receive_gps_data_measure_result(G2HSEARCH handle, int channel, int count, signed char done)
{

}

void CG2ClientView::on_g2search_require_prepare_playback(G2HSEARCH handle, int channel, G2SEARCH_PLAYBACK::COMMAND command, const G2SPOT& spot)
{
    G2RETURN_IF_FAIL(_search->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (command != G2SEARCH_PLAYBACK::PLAY &&
        command != G2SEARCH_PLAYBACK::NEXT &&
        command != G2SEARCH_PLAYBACK::STOP &&
        command != G2SEARCH_PLAYBACK::PREV) {
        unsigned __int64 cameras = client::find_cameras_from_host_id(hostId);
        screen_actuator& actuator = actuator_ref();
        actuator.delete_text_in(cameras);
    }
}

bool CG2ClientView::on_g2search_require_prepare_load_event_image(G2HSEARCH handle, int channel, int selected, bool last)
{
    G2RETURN_VAL_IF_FAIL(_search->safe_handle() == handle, false);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), false);

    unsigned __int64 cameras = client::find_cameras_from_host_id(hostId);

    screen_actuator& actuator = actuator_ref();
    actuator.delete_text_in(cameras);

    return true;
}

bool CG2ClientView::on_g2search_require_prepare_reload(G2HSEARCH handle, int channel)
{
    G2RETURN_VAL_IF_FAIL(_search->safe_handle() == handle, false);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH);
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), false);

    if (client::controller_search* controller = controller_search_ptr()) {
        controller->on_receive_prepare_reload(channel);
    }

    return true;
}

//////////////////////////////////////////////////////////////////////////

LRESULT CG2ClientView::on_post_connected_search(WPARAM wParam, LPARAM lParam)
{
    const short hostId = static_cast<short>(wParam);
    const int channel = static_cast<int>(lParam);

    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId) &&
                         client::valid_channel(channel), 1L);

    screen_actuator& actuator = actuator_ref();
    actuator.fire_connected();

    return 0L;
}

LRESULT CG2ClientView::on_post_disconnected_search(WPARAM wParam, LPARAM lParam)
{
    client::controller_search* controller = controller_search_ptr();
    G2RETURN_VAL_IF_FAIL(controller != NULL, 1L);

    controller->PostMessage(um_controller_::UM_DISCONNECTED, wParam, lParam);

    return 0L;
}

LRESULT CG2ClientView::on_post_receive_recorded_date_search(WPARAM wParam, LPARAM lParam)
{
    const int channel = static_cast<short>(wParam);
    G2RETURN_VAL_IF_FAIL(client::valid_channel(channel), 1L);

    if (search_data_ptr data = search_data_manager::get().find_search_data_search(channel)) {
        if (_search->get_timelapse_mode(channel) != G2SEARCH_TIMELAPSE::MINUTE) {
            search::SEARCH_DATE_INFO_LIST list;
            data->get_date_info_dvr(list);
            if (list.size() > 0) {
                G2TIME time;
                g2_time_from_time32_t(&time, (unsigned long)list.back().GetTime());
                _search->request_record_time(channel, time);
            }
        }
    }

    return 0L;
}

LRESULT CG2ClientView::on_post_screen_no_image_loaded_search(WPARAM wParam, LPARAM lParam)
{
    const short hostId = static_cast<short>(wParam);
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), 1L);

    if (_endofplayHost.find(hostId) == _endofplayHost.end()) {
        return 1L;
    }

    _endofplayHost.insert(hostId);

    short channel = client::find_channel_from_host_id(hostId);
    _search->request_notify_end_of_play(channel);
    
    if (client::controller_search* controller = controller_search_ptr()) {
        controller->screen_end_of_play(hostId);
    }

    return 0L;
}
