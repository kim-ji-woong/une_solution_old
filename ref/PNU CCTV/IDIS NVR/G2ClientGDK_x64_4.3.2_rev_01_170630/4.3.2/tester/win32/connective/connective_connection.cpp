// connective_connection.cpp : implementation file
//

#include "stdafx.h"
#include "G2ClientDoc.h"
#include "G2ClientView.h"
#include "MainFrm.h"

#include <include/g2_guid.h>
#include <sampler/cpp/g2client_admin.h>
#include <sampler/cpp/g2client_watch.h>
#include <sampler/cpp/g2client_search.h>
#include <sampler/cpp/g2client_search_g2.h>
#include <sampler/cpp/g2client_status.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::connect_watch(client::drop_info& di)
{
    di._connectMode = connect_mode_::WATCH;

    const short prevHostId = _cameraMap.get_host_id(di._dropcamera);
    const int prevMode = _cameraMap.get_connect_mode(di._dropcamera);

    if (invoke_drop_info_device(di)) {
        connective_host_info hi((LPCTSTR)di._sitename,
                                di._network,
                                di._connectMode);

        int findId = _hostList.find_host_id(hi);
        int hostId = client::invalid_::HOST_ID;

        if (findId < 0) {
            if (_hostList.count() >= client::MAX_CONNECTIVE_CHANNEL) {
                wchar_t buf[4] = { 0 };
                _itow_s(client::MAX_CONNECTIVE_CHANNEL, buf, 10);

                std::wstring message = L"The number of sites that can be connected is limited to ";
                message += buf;
                show_screen_message(message.c_str());
                return;
            }

            hostId = _hostList.find_free_host_id();
            if (client::valid_host_id(hostId) != true) {
                return;
            }

            hi.set_host_id(hostId);
            if (_hostList.append_host(hi) < 0) {
                return;
            }
        }
        else {
            hostId = findId;

            set_camera_map(hostId, di);
            disconnect_by_host_id(prevHostId);
            set_camera_list(hostId);
            return;
        }

        set_camera_map(hostId, di);
        set_disconnect_by_me(hostId, true);

        int channel = _watch->connect_ras(di._network, NULL);

        if (client::valid_channel(channel)) {
            _hostList.set_host_channel(hostId, channel);
        }
        else {
            remove_camera_map(hostId);
        }

        disconnect_by_host_id(prevHostId);
    }
    else {
        assert(!"failed the make drop info");
    }
}

void CG2ClientView::connect_search(client::drop_info& di)
{
    di._connectMode = connect_mode_::SEARCH_G2;
    
    const short prevHostId = _cameraMap.get_host_id(di._dropcamera);

    if (invoke_drop_info_device(di)) {
        connective_host_info hi((LPCTSTR)di._sitename,
                                di._network,
                                di._connectMode);

        int findId = _hostList.find_host_id(hi);
        int hostId = client::invalid_::HOST_ID;

        if (findId < 0) {
            if (_hostList.count() >= client::MAX_CONNECTIVE_CHANNEL) {
                wchar_t buf[4] = { 0 };
                _itow_s(client::MAX_CONNECTIVE_CHANNEL, buf, 10);

                std::wstring message = L"The number of sites that can be connected is limited to ";
                message += buf;
                show_screen_message(message.c_str());
                return;
            }

            hostId = _hostList.find_free_host_id();
            if (client::valid_host_id(hostId) != true) {
                return;
            }

            hi.set_host_id(hostId);
            if (_hostList.append_host(hi) < 0) {
                return;
            }
        }
        else {
            hostId = findId;

            set_camera_map(hostId, di);
            disconnect_by_host_id(prevHostId);
            set_camera_list(hostId);
            return;
        }

        set_camera_map(hostId, di);
        set_disconnect_by_me(hostId, true);

        int channel = _search_g2->connect_ras(hi.network(), false);
        
        if (client::valid_channel(channel)) {
            _hostList.set_host_channel(hostId, channel);
        }
        else {
            remove_camera_map(hostId);
        }

        disconnect_by_host_id(prevHostId);

        _dropInfoEvent.insert(std::make_pair(hostId, di));
    }
    else {
        assert(!"failed the make drop info");
    }
}

void CG2ClientView::connect_status(void)
{
    client::drop_info di = _dropInfo;
    di._connectMode = connect_mode_::STATUS;
    if(invoke_drop_info_device(di) == false) return;

    const short prevHostId = _cameraMap.get_host_id(di._dropcamera);

    G2CONNECT_RES res;
    _status->connect_ras(di._network, false, NULL, &res);
}

bool CG2ClientView::reconnect_device(short hostId, unsigned int reason /*= G2DISCONNECT_REASON::UNKNOWN*/)
{
    bool ignore = false;
    switch (reason) {
        case G2DISCONNECT_REASON::INVALID_VERSION:
        case G2DISCONNECT_REASON::LOGIN_FAIL:
        case G2DISCONNECT_REASON::NO_AUTHORITY:
            ignore = true;
            break;
    }

    if (ignore) return false;

    g2::scoped_criticalsection lock(_lock_connection);

    if(is_disconnect_by_me(hostId)) return false;

    _hostList.set_host_channel(hostId, client::invalid_::CHANNEL_RECONNECT);

    SetTimer(TIMER_ID_RECONNECT + hostId, 1000, NULL);

    return true;
}

bool CG2ClientView::invoke_drop_info_device(drop_info& di)
{
    bool cameraLevel = false;
    if (di._connectMode == connect_mode_::SEARCH) {
        /*
        cameraLevel = (g2_guid_is_device_leaf(&deviceGUID) ||
                       g2_guid_is_device_hybrided(&deviceGUID));

        if (g2_guid_is_device_hybrid_parent(&hostGUID)) {
            hostGUID = client::g2admin::get().get_root_guid_from_leaf_guid(deviceGUID);
        }
        */
    }
    else {
        cameraLevel = di._hostcamera >= 0;
    }
    if (di._dragFromId == drag_id_::DRAG_FROM_INSTANT_EVENT) {
        cameraLevel = false;
    }
    di._cameraLevel = cameraLevel;
    
    client::foundation::valueType data;
    foundation_ptr()->get_site_info(di._sitename, data);

    memset(&di._network, 0, sizeof(G2NETWORK_INFO));
    memcpy(di._network._address,    data._address.GetBuffer(),  min(sizeof(di._network._address), data._address.GetLength() * 2));
    memcpy(di._network._user_id,    data._id.GetBuffer(),       min(sizeof(di._network._user_id), data._id.GetLength() * 2));
    memcpy(di._network._password,   data._password.GetBuffer(), min(sizeof(di._network._password), data._password.GetLength() * 2));
    di._network._port[G2NETWORK_INFO::WATCH_PORT]    = data._watchPort;
    di._network._port[G2NETWORK_INFO::SEARCH_PORT]   = data._searchPort;
    di._network._port[G2NETWORK_INFO::ADMIN_PORT]    = data._adminPort;
    di._network._port[G2NETWORK_INFO::AUDIO_PORT]    = data._audioPort;

    return true;
}

void CG2ClientView::set_camera_map(short hostId, const client::drop_info& di)
{
    connective_host_info_ptr hi = _hostList.get_host_info(hostId);
    client::screen_actuator& actuator = actuator_ref();

    const short channel = (hi != NULL) ? hi->channel() : client::invalid_::CHANNEL;

  
    camera_map_element element(hostId,
                               di._hostcamera,
                               di._connectMode,
                               di._channelext);

    screen::camera_status_::TYPE curstatus = screen::camera_status_::UNDEFINED;

    if (di._cameraLevel) {
        const short hostcamera = di._hostcamera;
        const short scrcamera = di._dropcamera;
        actuator.reset_camera(scrcamera, true);

        if (client::valid_channel(channel)) {
            curstatus = camera_status_by_direct_mode(element.connect_mode(), channel, hostcamera);
            actuator.set_camera_status(scrcamera, curstatus);

            if (scrcamera == actuator.selected_camera()) {
                actuator.fire_connected(scrcamera);
            }

            actuator.set_camera_mode(scrcamera, (di._connectMode == client::connect_mode_::SEARCH ||
                                                 di._connectMode == client::connect_mode_::SEARCH_G2) ? client::screen::camera_::SEARCH : 
                                                                                                        client::screen::camera_::WATCH);
        }
        _cameraMap.set_camera_element(scrcamera, element);
    }
    else {
        G2DEVICE_LEAF device = { 0 };
        G2LIVE_CAMERA_STATUS status = { 0 };
        short scrcamera = di._dropcamera;

        element.set_host_camera(di._hostcamera);
        element.set_channel_ext(di._channelext);
        _cameraMap.set_camera_element(scrcamera, element);
    }
}

void CG2ClientView::remove_camera_map(short hostId)
{
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    screen_actuator& actuator = actuator_ref();
    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);
    actuator.reset_camera(cameras, true);

    _hostList.remove_host(hostId);
    _cameraMap.remove_by_host_id(hostId);
}

void CG2ClientView::set_disconnect_by_me(short hostId, bool byMe /*= true*/)
{
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    g2::scoped_criticalsection lock(_lock_disconnected);

    std::list<int>::iterator itr = std::find(_disconnectByMe.begin(), _disconnectByMe.end(), hostId);
    if (byMe) {
        if (itr == _disconnectByMe.end()) {
            _disconnectByMe.push_back(hostId);
        }
    }
    else {
        if (itr != _disconnectByMe.end()) {
            _disconnectByMe.erase(itr);
        }
    }
}

bool CG2ClientView::is_disconnect_by_me(short hostId)
{
    g2::scoped_criticalsection lock(_lock_disconnected);

    bool result = false;
    std::list<int>::const_iterator itr = std::find(_disconnectByMe.begin(), _disconnectByMe.end(), hostId);
    if (itr != _disconnectByMe.end()) {
        result = true;
    }
    return result;
}

void CG2ClientView::disconnect_all(void)
{
    g2::scoped_criticalsection lock(_lock_connection);

    actuator_ref().set_ignore_frame(true);

    unsigned __int64 cameras = _cameraMap.get_cameras();
    for (short i = 0; i < _cameraMap.size(); ++i) {
        if (g2::contains_bit(cameras, i)) {
            disconnect_device(i);
        }
    }

    while (_hostList.count() != 0) {
        MSG msg;
        if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE | PM_QS_PAINT) ||
            PeekMessage(&msg, NULL, WM_MOUSEFIRST, WM_MOUSELAST, PM_REMOVE)) {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
            g2::looper_<client::runner>::yield();
        }
        else {
            g2::looper_<client::runner>::sleep(1);
        }
    }

    actuator_ref().set_ignore_frame(false);
}

void CG2ClientView::reset_screen_view(short hostId, bool redraw /*= true*/, bool connect /*= true*/)
{
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);

    screen_actuator& actuator = actuator_ref();
    actuator.reset_camera(cameras, true, connect ? false : true, false);

    if (connect != true) {
        init_camera_title(cameras, false);
    }
    if (redraw) {
        actuator.update_camera(cameras);
    }
}

void CG2ClientView::init_camera_title(short camera, bool redraw /*= true*/)
{
    screen_actuator& actuator = actuator_ref();
    actuator.set_camera_title(camera, L"", redraw);
}

void CG2ClientView::init_camera_title(unsigned __int64 cameras, bool redraw /*= true*/)
{
    screen_actuator& actuator = actuator_ref();
    actuator.set_camera_title(cameras, L"", redraw);
}

void CG2ClientView::disconnect_each_camera(short selcamera)
{
    G2RETURN_IF_FAIL(valid_camera(selcamera));

    g2::scoped_criticalsection lock(_lock_connection);

    const short hostId = _cameraMap.get_host_id(selcamera);
    const int channelext = _cameraMap.get_channel_ext(selcamera);
    G2RETURN_IF_FAIL(valid_host_id(hostId));

    screen_actuator& actuator = actuator_ref();
    const int totalofcamera = _cameraMap.count_cameras(hostId);
    const int countofcamera = _cameraMap.count_cameras_from_channel_ext(hostId, channelext);

    if (totalofcamera > 0) {
        if (totalofcamera == 1) {
            disconnect_device(selcamera);
        }
        else {
            const int mode = _cameraMap.get_connect_mode(selcamera);
            _cameraMap.remove_camera(selcamera);
            actuator.reset_camera(selcamera, true, true, false);

            set_camera_list(hostId);

            init_camera_title(selcamera, false);
            actuator.update_camera(selcamera);
        }
    }

    if (_cameraMap.count_cameras(hostId) == 0) {
        actuator.remove_buffer(hostId);
    }

    if (selcamera == actuator.selected_camera()) {
        actuator.fire_connected(selcamera);
    }
}

void CG2ClientView::disconnect_each_device(short selcamera)
{
    G2RETURN_IF_FAIL(valid_camera(selcamera));

    g2::scoped_criticalsection lock(_lock_connection);

    const int connectMode = _cameraMap.get_connect_mode(selcamera);
    disconnect_device(selcamera);
}

void CG2ClientView::disconnect_device(short selcamera)
{
    G2RETURN_IF_FAIL(client::valid_camera(selcamera));

    g2::scoped_criticalsection lock(_lock_connection);

    const short hostId = _cameraMap.get_host_id(selcamera);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (connective_host_info_ptr info = _hostList.get_host_info(hostId)) {
        set_disconnect_by_me(hostId, true);

        if (client::valid_channel(info->channel()) != true ||
            is_disconnected(connect_mode_::MODE(info->mode()), info->channel())) {
            release_host_id(hostId, true, false);
        }
        else {
            switch (info->mode()) {
                case connect_mode_::WATCH:
                    _watch->disconnect(info->channel());
                    break;
                case connect_mode_::SEARCH:
                    _search->disconnect(info->channel());
                    break;
                case connect_mode_::SEARCH_G2:
                    _search_g2->disconnect(info->channel());
                    break;
                default:
                    assert(!"undefined connect type");
                    break;
            }
        }
        screen_actuator& actuator = actuator_ref();
        if (selcamera == actuator.selected_camera()) {
            actuator.fire_connected(selcamera);
        }
    }
}

void CG2ClientView::disconnect_by_host_id(short hostId)
{
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    g2::scoped_criticalsection lock(_lock_connection);

    connective_host_info_ptr hi = _hostList.get_host_info(hostId);
    G2RETURN_IF_FAIL(hi != NULL);

    set_disconnect_by_me(hostId, true);

    const int connectMode = _hostList.find_mode_from_host_id(hostId);

    if (connectMode == connect_mode_::SEARCH) {
        screen_actuator& actuator = actuator_ref();
        //actuator.set_search_shutdown(hostId, cameras);
    }

    const short channel = _hostList.find_channel_from_host_id(hostId);
    if (channel == client::invalid_::CHANNEL_RECONNECT) {
        reconnect_kill(hostId);
        return;
    }

    G2RETURN_IF_FAIL(client::valid_channel(channel));

    screen_actuator& actuator = actuator_ref();
    const int countofcamera = _cameraMap.count_cameras(hostId);

    if (countofcamera == 0) {
        switch (connectMode) {
            case connect_mode_::WATCH:
                _watch->disconnect(channel);
                break;
            case connect_mode_::SEARCH:
                _search->disconnect(channel);
                break;
            case connect_mode_::SEARCH_G2:
                _search_g2->disconnect(channel);
                break;
            default:
                assert(!"undefined connect type");
                break;
        }
    }
}

bool CG2ClientView::reconnect_impl(short hostId)
{
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), false);

    g2::scoped_criticalsection lock(_lock_connection);

    client::connective_host_info_ptr hi = _hostList.get_host_info(hostId);
    G2RETURN_VAL_IF_FAIL(hi != NULL, false);

    const short prevHostId = hi->host_id();
    const short prevChannel = hi->channel();

    client::connect_mode_::MODE connectMode = client::connect_mode_::MODE(hi->mode());
    //G2RETURN_VAL_IF_FAIL(connectMode == client::connect_mode_::WATCH, false);

    if (prevChannel != client::invalid_::CHANNEL_RECONNECT ||
        is_disconnectable(connectMode, prevChannel)) {
        return true;
    }

    /////////////////////////////////////////////

    if (is_disconnect_by_me(hostId)) {
        release_host_id(hostId);
        return true;
    }

    short rechannel = client::invalid_::CHANNEL;

    if (hi->mode() == connect_mode_::WATCH) {
        rechannel = _watch->connect_ras(hi->network());
    }
    else if (hi->mode() == connect_mode_::SEARCH_G2) {
        unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);
        short scrcamera = g2::find_first_biton(cameras);
        _cameraMap.set_connect_mode(scrcamera, connect_mode_::SEARCH);

        hi->set_mode(connect_mode_::SEARCH);
        set_disconnect_by_me(hostId, true);

        rechannel = _search->connect_ras(&hi->network());
        _dropInfoEvent.erase(hostId);
    }

    bool success = client::valid_channel(rechannel);
    if (success) {
        _hostList.set_host_channel(hostId, rechannel);
    }
    else {
        release_host_id(hostId);
    }
    return success;
}

bool CG2ClientView::reconnect_kill(short hostId)
{
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), false);

    g2::scoped_criticalsection lock(_lock_connection);

    /////////////////////////////////////////////

    const short channel = _hostList.find_channel_from_host_id(hostId);
    G2RETURN_VAL_IF_FAIL(channel == client::invalid_::CHANNEL_RECONNECT, false);

    KillTimer(TIMER_ID_RECONNECT + hostId);

    release_host_id(hostId, false, false);

    return true;
}

bool CG2ClientView::is_connected(client::connect_mode_::MODE mode, short channel)
{
    if (!client::valid_channel(channel)) return false;

    bool connected = false;
    switch(mode) {
        case connect_mode_::WATCH:
            connected = _watch->is_connected(channel);
            break;
        case connect_mode_::SEARCH:
            connected = _search->is_connected(channel);
            break;
        case connect_mode_::SEARCH_G2:
            connected = _search_g2->is_connected(channel);
            break;
        default:
            assert(!"undefined connect type");
            break;
    }
    return connected;
}

bool CG2ClientView::is_disconnected(client::connect_mode_::MODE mode, short channel)
{
    if (!client::valid_channel(channel)) return false;

    bool disconnected = false;
    switch(mode) {
        case connect_mode_::WATCH:
            disconnected = _watch->is_disconnected(channel);
            break;
        case connect_mode_::SEARCH:
            disconnected = _search->is_disconnected(channel);
            break;
        case connect_mode_::SEARCH_G2:
            disconnected = _search_g2->is_disconnected(channel);
            break;
        default:
            assert(!"undefined connect type");
            break;
    }
    return disconnected;
}

bool CG2ClientView::is_disconnectable(client::connect_mode_::MODE mode, short channel)
{
    if (!client::valid_channel(channel)) return false;

    bool disconnectable = false;
    switch(mode) {
        case connect_mode_::WATCH:
            disconnectable = _watch->is_disconnectable(channel);
            break;
        case connect_mode_::SEARCH:
            disconnectable = _search->is_disconnectable(channel);
            break;
        case connect_mode_::SEARCH_G2:
            disconnectable = _search_g2->is_disconnectable(channel);
            break;
        default:
            assert(!"undefined connect type");
            break;
    }
    return disconnectable;
}

client::screen::camera_status_::TYPE CG2ClientView::camera_status_by_device_status(int status)
{
    screen::camera_status_::TYPE camstatus = screen::camera_status_::UNDEFINED;
    switch (status) {
        case G2LIVE_CAMERA_STATUS::STATE_VIDEOLOSS:
            camstatus = screen::camera_status_::NO_VIDEO;
            break;
        case G2LIVE_CAMERA_STATUS::STATE_INACTIVE:
            camstatus = screen::camera_status_::INACTIVATE;
            break;
        case G2LIVE_CAMERA_STATUS::STATE_ACTIVE:
            camstatus = screen::camera_status_::ENABLE;
            break;
        case G2LIVE_CAMERA_STATUS::STATE_NOTCONNECTED:
            camstatus = screen::camera_status_::NOT_CONNECTED;
            break;
        case G2LIVE_CAMERA_STATUS::STATE_MULTISTREAM:
            camstatus = screen::camera_status_::ENABLE;
            break;
        default:
            camstatus = screen::camera_status_::UNDEFINED;
            break;
    }
    return camstatus;
}

client::screen::camera_status_::TYPE CG2ClientView::camera_status_by_direct_mode(int connectMode, short channel /*= -1*/, short hostcamera /*= -1*/)
{
    client::screen::camera_status_::TYPE camstatus = screen::camera_status_::UNDEFINED;

    G2RETURN_VAL_IF_FAIL(client::valid_channel(channel), camstatus);
    G2RETURN_VAL_IF_FAIL(client::valid_host_camera(hostcamera), camstatus);

    G2DEVICE_STATUS status = { 0 };
    switch (connectMode) {
        case client::connect_mode_::WATCH:
            if (_watch->get_status(channel, status)) {
                camstatus = camera_status_by_device_status(status._camera[hostcamera]);
            }
            break;
        case client::connect_mode_::SEARCH:
            camstatus = screen::camera_status_::ENABLE;
            break;
        case client::connect_mode_::SEARCH_G2:
            camstatus = screen::camera_status_::ENABLE;
            break;
        default:
            assert(!"undefined connect mode");
            break;
    }
    return camstatus;
}