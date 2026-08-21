// connective_camera_map.inl : inline implementation file
//

#ifndef _CONNECTIVE_CAMERA_MAP_INL_
#define _CONNECTIVE_CAMERA_MAP_INL_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <utility/g2retention_bit.h>
#include <algorithm>

namespace client {

//////////////////////////////////////////////////////////////////////////

inline camera_map_element::camera_map_element(void)
{
    initialize();
}

inline camera_map_element::camera_map_element(short hostId,
                                        short hostCamera,
                                        int connectMode,
                                        int channelext)
{
    _hostId = hostId;
    _hostcamera = hostCamera;
    _connectMode = connectMode;
    _channelext = channelext;
}

inline camera_map_element::~camera_map_element(void)
{

}

//////////////////////////////////////////////////////////////////////////

inline void camera_map_element::initialize(void)
{
    _hostId      = client::invalid_::HOST_ID;
    _hostcamera  = client::invalid_::CAMERA_NUMBER;
    _connectMode = client::connect_mode_::UNDEFINED;
    _channelext  = client::invalid_::CHANNEL;
}

inline bool camera_map_element::is_valid(void)
{
    return (_hostId != client::invalid_::HOST_ID &&
            _hostcamera != client::invalid_::CAMERA_NUMBER &&
            _connectMode != client::connect_mode_::UNDEFINED);
}

//////////////////////////////////////////////////////////////////////////

inline camera_map::camera_map(void)
{
    initialize();
}

inline camera_map::~camera_map(void)
{

}

//////////////////////////////////////////////////////////////////////////

inline bool camera_map::create(short countelement)
{
    std::vector<camera_map_element>(client::MAX_SCREEN_CAMERA_COUNT + 1).swap(_elements);

    assert(countelement < static_cast<int>(_elements.size()));
    return camera_map_base::create(countelement);
}

inline void camera_map::initialize(void)
{
    _cameras = 0ui64;
    std::for_each(_elements.begin(), _elements.end(), std::mem_fun_ref(&camera_map_element::initialize));
}

inline void camera_map::cleanup(void)
{
    initialize();
}

//////////////////////////////////////////////////////////////////////////

inline bool camera_map::set_camera_element(short scrcamera, const camera_map_element& info)
{
    G2RETURN_VAL_IF_FAIL(valid_element(scrcamera), false);

    g2::scoped_criticalsection lock(_lock_exec);

    _elements[scrcamera] = info;
    g2::set_bit(_cameras, scrcamera);

    return true;
}

inline void camera_map::set_host_id(short scrcamera, short hostId)
{
    G2RETURN_IF_FAIL(valid_element(scrcamera));

    g2::scoped_criticalsection lock(_lock_exec);

    _elements[scrcamera].set_host_id(hostId);
}

inline void camera_map::set_host_camera(short scrcamera, short hostcamera)
{
    G2RETURN_IF_FAIL(valid_element(scrcamera));

    g2::scoped_criticalsection lock(_lock_exec);

    _elements[scrcamera].set_host_camera(hostcamera);
}

inline void camera_map::set_connect_mode(short scrcamera, int connectMode)
{
    G2RETURN_IF_FAIL(valid_element(scrcamera));

    g2::scoped_criticalsection lock(_lock_exec);

    _elements[scrcamera].set_connect_mode(connectMode);
}

inline void camera_map::set_channel_ext(short scrcamera, int channelext)
{
    G2RETURN_IF_FAIL(valid_element(scrcamera));

    g2::scoped_criticalsection lock(_lock_exec);

    _elements[scrcamera].set_channel_ext(channelext);
}

inline short camera_map::get_host_id(short scrcamera)
{
    G2RETURN_VAL_IF_FAIL(valid_element(scrcamera), invalid_::HOST_ID);

    return _elements[scrcamera].host_id();
}

inline short camera_map::get_host_camera(short scrcamera)
{
    G2RETURN_VAL_IF_FAIL(valid_element(scrcamera), invalid_::CAMERA_NUMBER);

    return _elements[scrcamera].host_camera();
}

inline short camera_map::get_camera_by_channel_ext(int channelext)
{
    G2RETURN_VAL_IF_FAIL(client::valid_channel_ext(channelext), invalid_::CAMERA_NUMBER);

    short camera = invalid_::CAMERA_NUMBER;
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].channel_ext() == channelext) {
            camera = i;
            break;
        }
    }
    return camera;
}

inline short camera_map::get_camera_by_host_camera(short hostId, short hostcamera)
{
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), invalid_::CAMERA_NUMBER);
    G2RETURN_VAL_IF_FAIL(valid_element(hostcamera), invalid_::CAMERA_NUMBER);

    short camera = invalid_::CAMERA_NUMBER;
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].host_id() == hostId &&
            _elements[i].host_camera() == hostcamera) {
            camera = i;
            break;
        }
    }
    return camera;
}

inline unsigned __int64 camera_map::get_cameras_by_host_id(short hostId)
{
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(hostId), 0x00ui64);

    unsigned __int64 cameras = 0x00ui64;
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].host_id() == hostId) {
            g2::set_bit(cameras, i);
        }
    }
    return cameras;
}

inline unsigned __int64 camera_map::get_cameras_by_channel_ext(int channelext)
{
    G2RETURN_VAL_IF_FAIL(client::valid_channel_ext(channelext), 0x00ui64);

    unsigned __int64 cameras = 0x00ui64;
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].channel_ext() == channelext) {
            g2::set_bit(cameras, i);
        }
    }
    return cameras;
}

inline unsigned __int64 camera_map::get_cameras(void)
{
    unsigned __int64 cameras = 0x00ui64;
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].is_valid()) {
            g2::set_bit(cameras, i);
        }
    }
    return cameras;
}

inline int camera_map::get_connect_mode(short scrcamera)
{
    G2RETURN_VAL_IF_FAIL(valid_element(scrcamera), connect_mode_::UNDEFINED);

    return _elements[scrcamera].connect_mode();
}

inline int camera_map::get_channel_ext(short scrcamera)
{
    G2RETURN_VAL_IF_FAIL(valid_element(scrcamera), invalid_::CHANNEL_EXT);

    return _elements[scrcamera].channel_ext();
}

inline short camera_map::reference_camera(short hostId, int channelext, unsigned __int64& refcameras)
{
    g2::scoped_criticalsection lock(_lock_exec);

    refcameras = 0x00ui64;
    short base = invalid_::CAMERA_NUMBER;

    for (int i = client::MAX_SCREEN_CAMERA_COUNT - 1; i >= 0 ; --i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].host_id() == hostId &&
            _elements[i].channel_ext() == channelext) {
            g2::set_bit(refcameras, i);
            base = i;
        }
    }
    return base;
}

inline short camera_map::reference_camera(short hostId, short hostcamera, unsigned __int64& refcameras)
{
    g2::scoped_criticalsection lock(_lock_exec);

    refcameras = 0x00ui64;
    short base = invalid_::CAMERA_NUMBER;

    for (int i = client::MAX_SCREEN_CAMERA_COUNT - 1; i >= 0 ; --i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].host_id() == hostId &&
            _elements[i].host_camera() == hostcamera) {
            g2::set_bit(refcameras, i);
            base = i;
        }
    }
    return base;
}

inline std::set<int> camera_map::get_channel_ext_by_host_id(short hostId, int connectMode /*= connect_mode_::UNDEFINED*/)
{
    std::set<int> list;

    for (int i = client::MAX_SCREEN_CAMERA_COUNT - 1; i >= 0 ; --i) {
        const camera_map_element& info = _elements[i];

        if (connectMode != connect_mode_::UNDEFINED &&
            info.connect_mode() != connectMode) {
            continue;
        }

        if (info.host_id() == hostId) {
            list.insert(info.channel_ext());
        }
    }

    return list;
}

inline bool camera_map::remove_by_host_id(short hostId)
{
    g2::scoped_criticalsection lock(_lock_exec);

    bool succeed = false;
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].host_id() == hostId) {
            _elements[i].initialize();
            g2::reset_bit(_cameras, i);
            succeed = true;
        }
    }
    return succeed;
}

inline bool camera_map::remove_camera(short camera)
{
    g2::scoped_criticalsection lock(_lock_exec);

    bool succeed = false;
    if (g2::contains_bit(_cameras, camera)) {
        g2::reset_bit(_cameras, camera);
        _elements[camera].initialize();
    }
    return succeed;
}


inline int camera_map::count_cameras(void)
{
    g2::scoped_criticalsection lock(_lock_exec);

    int count = 0;
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].is_valid()) {
            ++count;
        }
    }
    return count;
}

inline int camera_map::count_cameras(short hostId)
{
    g2::scoped_criticalsection lock(_lock_exec);

    int count = 0;
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].host_id() == hostId) {
            ++count;
        }
    }
    return count;
}

inline int camera_map::count_cameras_from_channel_ext(short hostId, int channelext)
{
    g2::scoped_criticalsection lock(_lock_exec);

    int count = 0;
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(_cameras, i) &&
            _elements[i].host_id() == hostId &&
            _elements[i].channel_ext() == channelext) {
            ++count;
        }
    }
    return count;
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONNECTIVE_CAMERA_MAP_INL_
