// connective_search_g2.cpp : implementation file
//

#include "stdafx.h"
#include "MainFrm.h"
#include "G2ClientDoc.h"
#include "G2ClientView.h"
#include "search/search_data_manager.h"

#include "panel/controller/controller_search_g2.h"
#include <sampler/cpp/g2client_search_g2.h>
#include <boost/lexical_cast.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::on_g2search_g2_connected(G2HSEARCH_G2 handle, int channel)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    connective_host_info_ptr hi = _hostList.get_host_info(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(hi != NULL);

    const short hostId = hi->host_id();
    if (client::valid_host_id(hostId)) {
        // success connection
    }
    else {
        _search_g2->disconnect(channel);
        assert(!"occur broken artifact channel and hostId");
        return;
    }

    screen_actuator& actuator = actuator_ref();
    actuator.set_activate_screen(true);

    G2_PRODUCT_INFO pi;
    _search_g2->get_product_info(channel, pi);

    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);
    unsigned __int64 visible = actuator_ref().visible_camera();
    int maxcamera = pi.basic_info.hybrid ? pi.device_hybird.count_camera + pi.device_hybird.count_network_camera :
                                           pi.device.count_camera;
    std::wstring site = _hostList.site_name_from_host_id(hostId);

    //////////////////////////////////////////////////////////////////////////
    // set camera map

    if (_cameraMap.count_cameras(hostId) == 1) {
        short scrcamera = g2::find_first_biton(cameras);
        if (_cameraMap.get_host_camera(scrcamera) == client::invalid_::CAMERA_NUMBER) {
            for (int i = 0; i < maxcamera; ++i) {
                CString title;
                title.Format(L"Camera %d", i + 1);
                foundation_ptr()->add_site_child(site.c_str(), title, i, true);

                if (g2::contains_bit(visible, scrcamera)) {
                    camera_map_element element(hostId, i,
                                               client::connect_mode_::SEARCH_G2,
                                               client::invalid_::CHANNEL_EXT);
                    _cameraMap.set_camera_element(scrcamera++, element);
                }
            }
        }
    }
    cameras = _cameraMap.get_cameras_by_host_id(hostId);
    actuator.set_camera_mode(cameras, client::screen::camera_::SEARCH);

    //////////////////////////////////////////////////////////////////////////
    // update camera status

    for (short i = 0; i < _cameraMap.size(); ++i) {
        if (_cameraMap.get_host_id(i) == hostId) {
            actuator.set_camera_status(i, screen::camera_status_::ENABLE, false);
        }
    }
    set_camera_list(hostId, false, true);

    //////////////////////////////////////////////////////////////////////////
    
    frame_buffer::PLAYTIME typetime = frame_buffer::TIME_MSEC;
    if (_search_g2->is_support(channel, G2SEARCH_SUPPORT::BUFFER_USE_TICK)) {
        typetime = frame_buffer::TIME_TICK;
    }
    else if (_search_g2->is_support(channel, G2SEARCH_SUPPORT::BUFFER_USE_SYSTEMMSEC)) {
        typetime = frame_buffer::TIME_SYSMSEC;
    }
    actuator.setup_buffer(hostId, frame_buffer::TYPE_SEARCH_G2, typetime);

    search_data_ptr data = search_data_manager::get().new_search_data(search::SEARCH_LOCAL_G2, channel);
    search_data_manager::get().append_search_data(data);

    PostMessage(um_runner_::UM_CONNECTED_SEARCH_G2, WPARAM(hostId), LPARAM(channel));
}

void CG2ClientView::on_g2search_g2_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON::TYPE reason)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    connective_host_info_ptr hi = _hostList.get_host_info(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(hi != NULL && client::valid_host_id(hi->host_id()));
    const short hostId = hi->host_id();

    //////////////////////////////////////////////////////////////////////////
    
    if (reason != G2DISCONNECT_REASON::LOGOUT &&
        reason != G2DISCONNECT_REASON::MISMATCH_ADAPTOR) {
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

    if (reason == G2DISCONNECT_REASON::MISMATCH_ADAPTOR) {
        set_disconnect_by_me(hostId, false);
        if (reconnect_device(hostId, reason)) {
            actuator.set_camera_status(cameras, screen::camera_status_::RECONNECTING);
            return;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    
    release_host_id(hostId, true, false);

    actuator.fire_disconnected();

    client::search_data_manager::get().remove_search_data(search::SEARCH_LOCAL_G2, channel);

    PostMessage(um_runner_::UM_DISCONNECTED_SEARCH_G2, WPARAM(hostId), LPARAM(channel));
}

void CG2ClientView::on_g2search_g2_receive_record_time_info_load(G2HSEARCH_G2 handle, int channel, const G2RECORD_TIME_INFO& rti)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (search_data_ptr data = search_data_manager::get().find_search_data_search_g2(channel)) {
        if (rti._resolution == G2RECORD_TIME_INFO::DAY) {
            data->set_date_info(rti);
        }
        else {
            data->set_minute_info(rti);
        }
    }
}

void CG2ClientView::on_g2search_g2_receive_record_time_info_load_end(G2HSEARCH_G2 handle, int channel, G2RECORD_TIME_INFO::RESOLUTION resolution, G2RECORD_TIME_INFO::COMMAND command)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    PostMessage(um_runner_::UM_RECTIME_LOADED_SEARCH_G2, WPARAM(channel), MAKELONG(resolution, command));
}

void CG2ClientView::on_g2search_g2_receive_frame_data(G2HSEARCH_G2 handle, int channel, const G2FRAME& frame)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    const short camera = frame._index._channel;

    G2RETURN_IF_FAIL(client::valid_host_id(hostId));
    G2RETURN_IF_FAIL(client::valid_host_camera(camera));

    std::wstring site = _hostList.site_name_from_host_id(hostId);
    foundation_ptr()->add_site_child(site.c_str(), frame._title, frame._index._channel, true);

    screen_actuator& actuator = actuator_ref();
    if (frame._index._type == G2FRAME::AUDIO) {
        if (actuator->is_layout_1x1() != true) {
            return;
        }
    }

    unsigned __int64 refcameras = 0x00ui64;
    int scrcamera = client::find_cameras_from_host_camera(hostId, camera, refcameras);

    if (client::valid_camera(scrcamera) != true) {
        return;
    }

    actuator.put_frame(G2FRAME::FROM_SEARCH_G2, frame, hostId, scrcamera, refcameras);
}

void CG2ClientView::on_g2search_g2_receive_text_in(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    // overlay text-in
}

void CG2ClientView::on_g2search_g2_receive_event(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));
}

void CG2ClientView::on_g2search_g2_receive_gps_data(G2HSEARCH_G2 handle, int channel, const G2EVENT& ei)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));
}

void CG2ClientView::on_g2search_g2_receive_notify_command_begin(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    PostMessage(um_runner_::UM_RECEIVE_COMMAND_BEGIN_SEARCH_G2, WPARAM(channel), LPARAM(command));
}

void CG2ClientView::on_g2search_g2_receive_notify_command_end(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if ((G2PLAYER::COMMAND_MOVE < command && command < G2PLAYER::COMMAND_SEARCH) != true) {
        screen_actuator& actuator = actuator_ref();
        actuator.clear_frame_buffer_play(hostId);
    }

    if (command == G2PLAYER::MOVE_TO_LAST) {
        std::map<short, client::drop_info>::iterator itr = _dropInfoEvent.find(hostId);
        if (itr != _dropInfoEvent.end()) {
            client::drop_info di = itr->second;

            _dropInfoEvent.erase(hostId);

            G2SPOT spotRemote;
            g2_spot_make_invalid(&spotRemote);

            int precision = G2PLAYER::PRECISION::EVENT;

            switch (di._dragFromId) {
                case drag_id_::DRAG_FROM_INSTANT_EVENT:
                    spotRemote = di._eventInfo._spot;
                default:
                    break;
            }

            if (g2_spot_is_valid(&spotRemote))
            {
                _search_g2->request_move_to_spot(channel, spotRemote, precision, true);
            }
        }
    }
}

void CG2ClientView::on_g2search_g2_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED speed)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (speed == G2PLAYER::NONE) {
        screen_actuator& actuator = actuator_ref();
        actuator.clear_frame_buffer_play(hostId);
    }

    if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
        controller->on_receive_notify_play_speed_changed_play(channel, speed);
    }
}

void CG2ClientView::on_g2search_g2_receive_notify_frame_not_found(G2HSEARCH_G2 handle, int channel, G2SPOT& spot, G2PLAYER::PRECISION::TYPE precision)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    if (precision == G2PLAYER::PRECISION::EVENT) {
        const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
        G2RETURN_IF_FAIL(client::valid_host_id(hostId));

        show_screen_message(L"There is no recorded image.");
        g2::looper_<client::runner>::sleep(1000);
        _search_g2->disconnect(channel);
    }
}

void CG2ClientView::on_g2search_g2_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE status)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    screen_actuator& actuator = actuator_ref();
    actuator.set_search_end_play(G2FRAME::FROM_SEARCH_G2, hostId);

    _endofplayHost.insert(hostId);
}

void CG2ClientView::on_g2search_g2_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, G2ROLLBACK_INFO& rbi)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
}

void CG2ClientView::on_g2search_g2_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE error)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (error == G2PLAYER::PLAYER_ERROR::BANK_UPDATE) {
        if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
            controller->request_stop_sync();
            controller->request_first();
            show_screen_message(L"Recorded data was overwritten.");
        }
    }
    else if (error == G2PLAYER::PLAYER_ERROR::HANG_ON_FAILED ||
             error == G2PLAYER::PLAYER_ERROR::UNKNOWN) {
        _search_g2->disconnect(channel);
        std::wstring string;
        string += L"Recording service error : ";
        string += boost::lexical_cast<std::wstring>(error);

        show_screen_message(string);
    }
}

void CG2ClientView::on_g2search_g2_receive_event_log_load_end(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
        controller->on_receive_event_query_result(channel, list);
    }
}

void CG2ClientView::on_g2search_g2_receive_event_log_load_stop(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
        controller->on_receive_event_query_result(channel, list);
    }
}

void CG2ClientView::on_g2search_g2_receive_text_in_log_load_end(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
        controller->on_receive_text_in_query_result(channel, list);
    }
}

void CG2ClientView::on_g2search_g2_receive_text_in_log_load_stop(G2HSEARCH_G2 handle, int channel, const std::vector<G2EVENT>& list)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
        controller->on_receive_text_in_query_result(channel, list);
    }
}

void CG2ClientView::on_g2search_g2_receive_scope_list(G2HSEARCH_G2 handle, int channel,  const std::vector<G2SCOPE>& scopes, G2SEARCH_G2_SCOPE_TYPE::TYPE type)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));
    if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
        controller->on_receive_scope_list(channel, scopes, type);
    }
}

void CG2ClientView::on_g2search_g2_receive_spot_list(G2HSEARCH_G2 handle, int channel, const std::vector<G2SPOT>& spots)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));
    if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
        controller->on_receive_spot_list(channel, spots);
    }
}

void CG2ClientView::on_g2search_g2_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    PostMessage(um_runner_::UM_NO_RECORDED_DATA_SEARCH_G2, WPARAM(channel));
}

void CG2ClientView::on_g2search_g2_receive_db_info(G2HSEARCH_G2 handle, int channel, const G2SEARCH_G2_REMOTE_DB& di)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
}

void CG2ClientView::on_g2search_g2_receive_db_info_external(G2HSEARCH_G2 handle, int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& dis)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
}

void CG2ClientView::on_g2search_g2_receive_db_selected(G2HSEARCH_G2 handle, int channel, unsigned int id, G2SEARCH_G2_REMOTE_DB::DB_SELECT_RESULT result)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
}

void CG2ClientView::on_g2search_g2_receive_virtual_channelmap(G2HSEARCH_G2 handle, int channel)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
}

void CG2ClientView::on_g2search_g2_require_prepare_rollback(G2HSEARCH_G2 handle, int channel, bool prepare)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    screen_actuator& actuator = actuator_ref();
    actuator.set_prepare_drive(hostId, prepare);
}

//////////////////////////////////////////////////////////////////////////

LRESULT CG2ClientView::on_post_connected_search_g2(WPARAM wParam, LPARAM lParam)
{
    const short hostId = static_cast<short>(wParam);
    const int channel = static_cast<int>(lParam);

    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId) &&
        client::valid_channel(channel), 1L);

    screen_actuator& actuator = actuator_ref();
    actuator.fire_connected();

    return 0L;
}

LRESULT CG2ClientView::on_post_disconnected_search_g2(WPARAM wParam, LPARAM lParam)
{
    client::controller_search_g2* controller = controller_search_g2_ptr();
    G2RETURN_VAL_IF_FAIL(controller != NULL, 1L);

    controller->PostMessage(um_controller_::UM_DISCONNECTED, wParam, lParam);

    return 0L;
}

LRESULT CG2ClientView::on_post_screen_no_image_loaded_search_g2(WPARAM wParam, LPARAM lParam)
{
    const short hostId = static_cast<short>(wParam);
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), 1L);

    if (_endofplayHost.find(hostId) == _endofplayHost.end()) {
        return 1L;
    }

    _endofplayHost.insert(hostId);

    short channel = client::find_channel_from_host_id(hostId);
    _search_g2->request_notify_end_of_play(channel);

    if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
        controller->screen_end_of_play(hostId);
    }

    return 0L;
}

LRESULT CG2ClientView::on_post_rectime_info_load_end_search_g2(WPARAM wParm, LPARAM lParm)
{
    int channel     = static_cast<int>(wParm);
    int resolution  = static_cast<int>(LOWORD(lParm));
    int command     = static_cast<int>(HIWORD(lParm));

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), -1L);

    if (resolution == G2RECORD_TIME_INFO::DAY) {
        search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL_G2, channel);
        if (g2_spot_is_valid(&data->spot_selected()) == false) {
            _search_g2->request_move_to_last(channel);
            _search_g2->set_play_control_command(channel, G2PLAYER::MOVE_TO_SPOT);
        }
    }
    else if (resolution == G2RECORD_TIME_INFO::MINUTE) {
        if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
            if (client::search::request_::REQUEST_OVERWRITE_LEFT  == command ||
                client::search::request_::REQUEST_OVERWRITE_RIGHT == command) {
                controller->update_site(channel);
            }
            else {
                controller->load_site(channel);
                controller->set_enable(true);
            }
        }
    }

    return 0L;
}

LRESULT CG2ClientView::on_post_no_recorded_data_search_g2(WPARAM wParm, LPARAM lParm)
{
    int channel = static_cast<int>(wParm);

    show_screen_message(L"There is no recorded image.");
    g2::looper_<client::runner>::sleep(1000);
    _search_g2->disconnect(channel);

    return 0L;
}

LRESULT CG2ClientView::on_post_receive_command_begin_search_g2(WPARAM wParm, LPARAM lParm)
{
    int channel = static_cast<int>(wParm);
    int command = static_cast<int>(lParm);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), -1L);

    if (command != G2PLAYER::NEXT_STEP) {
        screen_actuator& actuator = actuator_ref();
        if (command != G2PLAYER::PLAY_SLOW &&
            command != G2PLAYER::PLAY_NORMAL) {
                actuator.clear_frame_buffer_play(hostId);
        }
    }

    if (client::controller_search_g2* controller = controller_search_g2_ptr()) {
        controller->on_receive_notify_command_begin_play(channel, command);
    }

    return 0L;
}
