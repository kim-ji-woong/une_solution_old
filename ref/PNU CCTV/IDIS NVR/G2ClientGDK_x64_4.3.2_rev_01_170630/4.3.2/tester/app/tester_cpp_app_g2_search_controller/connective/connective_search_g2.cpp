// connective_search_g2.cpp : implementation file
//

#include "stdafx.h"
#include "G2SearchControllerView.h"
#include "G2SearchControllerDlg.h"
#include "actuator/screen_actuator.h"
#include "control/message_box.h"
#include "search/search_data_manager.h"

#include <sampler/cpp/g2client_search_g2.h>
#include <boost/lexical_cast.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerView::on_g2search_g2_connected(G2HSEARCH_G2 handle, int channel)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    set_camera_list();

    //////////////////////////////////////////////////////////////////////////
    
    frame_buffer::PLAYTIME typetime = frame_buffer::TIME_MSEC;
    if (_search_g2->is_support(channel, G2SEARCH_SUPPORT::BUFFER_USE_TICK)) {
        typetime = frame_buffer::TIME_TICK;
    }
    else if (_search_g2->is_support(channel, G2SEARCH_SUPPORT::BUFFER_USE_SYSTEMMSEC)) {
        typetime = frame_buffer::TIME_SYSMSEC;
    }

    if (actuator_ref().is_prepare_drive(channel) == false)
        actuator_ref().setup_buffer(channel, frame_buffer::TYPE_SEARCH_G2, typetime);

    search_data_ptr data = search_data_manager::get().new_search_data(search::SEARCH_LOCAL_G2, channel);
    search_data_manager::get().append_search_data(data);

    PostMessage(um_runner_::UM_CONNECTED_SEARCH_G2, WPARAM(channel));
}

void G2SearchControllerView::on_g2search_g2_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON::TYPE reason)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    //////////////////////////////////////////////////////////////////////////
    
    G2STRING_128 sreason = { 0 };
    g2_get_string_disconnect_reason(reason, &sreason);

    std::wstring message = sreason._string;
    int duration = _option._message_duration > 0 ? _option._message_duration : 0;
    show_screen_message(L"Device disconnected.", message, true, duration);

    std::wostringstream stream;
    stream << L"Connect failed : " << _option._device_ni._address 
           << L" ( " << _option._device_ni._user_id << L" )";

    if (_controller.get())
        _controller->set_init_message(stream.str());

    //////////////////////////////////////////////////////////////////////////
    
    client::search_data_manager::get().remove_search_data(search::SEARCH_LOCAL_G2, channel);

    PostMessage(um_runner_::UM_DISCONNECTED_SEARCH_G2, WPARAM(channel), LPARAM(reason));
}

void G2SearchControllerView::on_g2search_g2_receive_record_time_info_load(G2HSEARCH_G2 handle, int channel, const G2RECORD_TIME_INFO& rti)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    if (search_data_ptr data = search_data_manager::get().find_search_data_search_g2(channel)) {
        if (rti._resolution == G2RECORD_TIME_INFO::DAY) {
            data->set_date_info(rti);
        }
        else {
            data->set_minute_info(rti);
        }
    }
}

void G2SearchControllerView::on_g2search_g2_receive_record_time_info_load_end(G2HSEARCH_G2 handle, int channel, G2RECORD_TIME_INFO::RESOLUTION resolution, G2RECORD_TIME_INFO::COMMAND command)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    PostMessage(um_runner_::UM_RECTIME_LOADED_SEARCH_G2, WPARAM(channel), MAKELONG(resolution, command));
}

void G2SearchControllerView::on_g2search_g2_receive_frame_data(G2HSEARCH_G2 handle, int channel, const G2FRAME& frame)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    const short camera = frame._index._channel;
    G2RETURN_IF_FAIL(client::valid_camera(camera));
    
    unsigned __int64 refcameras = 0x01ui64 << camera;
    actuator_ref().put_frame(G2FRAME::FROM_SEARCH_G2, frame, channel, camera, refcameras);
}

void G2SearchControllerView::on_g2search_g2_receive_notify_command_begin(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    PostMessage(um_runner_::UM_RECEIVE_COMMAND_BEGIN_SEARCH_G2, WPARAM(channel), LPARAM(command));
}

void G2SearchControllerView::on_g2search_g2_receive_notify_command_end(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED command)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    if ((G2PLAYER::COMMAND_MOVE < command && command < G2PLAYER::COMMAND_SEARCH) != true) {
        actuator_ref().clear_frame_buffer_play(channel);
    }
}

void G2SearchControllerView::on_g2search_g2_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, int channel, G2PLAYER::COMMAND_AND_SPEED speed)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    if (speed == G2PLAYER::NONE) {
        actuator_ref().clear_frame_buffer_play(channel);
    }

    if (_controller.get())
        _controller->on_receive_notify_play_speed_changed_play(channel, speed);
}

void G2SearchControllerView::on_g2search_g2_receive_notify_frame_not_found(G2HSEARCH_G2 handle, int channel, G2SPOT& spot, G2PLAYER::PRECISION::TYPE precision)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    if (precision == G2PLAYER::PRECISION::EVENT) {
        int duration = _option._message_duration > 0 ? _option._message_duration : 0;
        show_screen_message(L"There is no recorded image.", true, duration);
        g2::looper_<client::runner>::sleep(1000);
        _search_g2->disconnect(channel);
    }
}

void G2SearchControllerView::on_g2search_g2_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER::OUT_OF_SCOPE::TYPE status)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    actuator_ref().set_search_end_play(G2FRAME::FROM_SEARCH_G2, channel);

    _endofplayHost.insert(channel);

    if (_controller.get())
        _controller->on_receive_notify_play_speed_changed_play(channel, 0);
}

void G2SearchControllerView::on_g2search_g2_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, G2ROLLBACK_INFO& rbi)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);
}

void G2SearchControllerView::on_g2search_g2_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER::PLAYER_ERROR::TYPE error)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    if (error == G2PLAYER::PLAYER_ERROR::BANK_UPDATE) {
        if (_controller.get()) {
            _controller->request_stop_sync();
            _controller->request_first();
            int duration = _option._message_duration > 0 ? _option._message_duration : 0;
            show_screen_message(L"Recorded data was overwritten.", true, duration);
        }
    }
    else if (error == G2PLAYER::PLAYER_ERROR::HANG_ON_FAILED ||
             error == G2PLAYER::PLAYER_ERROR::UNKNOWN) {
        _search_g2->disconnect(channel);
        std::wstring string;
        string += L"Recording service error : ";
        string += boost::lexical_cast<std::wstring>(error);

        int duration = _option._message_duration > 0 ? _option._message_duration : 0;
        show_screen_message(string, true, duration);
    }
}

void G2SearchControllerView::on_g2search_g2_receive_scope_list(G2HSEARCH_G2 handle, int channel,  const std::vector<G2SCOPE>& scopes, G2SEARCH_G2_SCOPE_TYPE::TYPE type)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    if (_controller.get())
        _controller->on_receive_response_scope_list(channel, scopes, type);
}

void G2SearchControllerView::on_g2search_g2_receive_spot_list(G2HSEARCH_G2 handle, int channel, const std::vector<G2SPOT>& spots)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    if (_controller.get())
        _controller->on_receive_response_spot_list(channel, spots);
}

void G2SearchControllerView::on_g2search_g2_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    PostMessage(um_runner_::UM_NO_RECORDED_DATA_SEARCH_G2, WPARAM(channel));
}

void G2SearchControllerView::on_g2search_g2_receive_db_info(G2HSEARCH_G2 handle, int channel, const G2SEARCH_G2_REMOTE_DB& di)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);
}

void G2SearchControllerView::on_g2search_g2_receive_db_info_external(G2HSEARCH_G2 handle, int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& dis)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);
}

void G2SearchControllerView::on_g2search_g2_receive_db_selected(G2HSEARCH_G2 handle, int channel, unsigned int id, G2SEARCH_G2_REMOTE_DB::DB_SELECT_RESULT result)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);
}

void G2SearchControllerView::on_g2search_g2_receive_virtual_channelmap(G2HSEARCH_G2 handle, int channel)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);
}

void G2SearchControllerView::on_g2search_g2_require_prepare_rollback(G2HSEARCH_G2 handle, int channel, bool prepare)
{
    G2RETURN_IF_FAIL(_search_g2->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _search_channel);

    actuator_ref().set_prepare_drive(channel, prepare);
}
