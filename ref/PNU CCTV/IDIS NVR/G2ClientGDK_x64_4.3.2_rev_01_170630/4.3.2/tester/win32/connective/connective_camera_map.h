// connective_camera_map.h : header file
//

#ifndef _CONNECTIVE_CAMERA_MAP_H_
#define _CONNECTIVE_CAMERA_MAP_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <sampler/cpp/g2client_guid.h>
#include <utility/g2lock.h>

#include <set>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class camera_map_base
{
public:
    camera_map_base(void)
        : _size(0)
    {

    }
    explicit camera_map_base(int countelement)
    {
        create(countelement);
    }
    virtual ~camera_map_base(void)
    {

    }

public:
    virtual void initialize(void) = 0;
    virtual bool create(int countelement)
    {
        assert(countelement > 0);
        _size = countelement;
        return (_size > 0);
    }
    int size(void) const { return _size; }

protected:
    bool valid_element(short index) const
    {
        return (index >= 0 && index < _size);
    }

protected:
    int _size;
};

//////////////////////////////////////////////////////////////////////////

class camera_map_element
{
public:
    camera_map_element(void);

    camera_map_element(short hostId,
                    short hostCamera,
                    int connectMode,
                    int channelext);

    ~camera_map_element(void);

protected:
    short _hostId;
    short _hostcamera;
    short _connectMode;
    int   _channelext;

public:
    void initialize(void);
    bool is_valid(void);

public:
    short host_id(void)                    const   { return _hostId;         }
    short host_camera(void)                const   { return _hostcamera;     }
    short connect_mode(void)               const   { return _connectMode;    }
    int channel_ext(void)                  const   { return _channelext;     }

    void set_host_id(short hostId)                 { _hostId = hostId;       }
    void set_host_camera(short camera)             { _hostcamera = camera;   }
    void set_connect_mode(short mode)              { _connectMode = mode;    }
    void set_channel_ext(int channelext)           { _channelext = channelext;  }
};

//////////////////////////////////////////////////////////////////////////

class camera_map : public camera_map_base
{
public:
    camera_map(void);
    virtual ~camera_map(void);

protected:
    unsigned __int64 _cameras;
    std::vector<camera_map_element> _elements;

    g2::critical_section _lock_exec;

public:
    virtual void initialize(void);
    virtual bool create(short countelement);

    void cleanup(void);

    bool set_camera_element(short scrcamera, const camera_map_element& info);

    void set_host_id(short scrcamera, short hostId);
    void set_host_camera(short scrcamera, short hostcamera);
    void set_connect_mode(short scrcamera, int connectMode);
    void set_decoder_key(short scrcamera, int decoderKey);
    void set_channel_ext(short scrcamera, int channelext);

    /////////////////////////////////////////////

    short get_host_id(short scrcamera);
    short get_host_camera(short scrcamera);
    short get_camera_by_channel_ext(int channelext);
    short get_camera_by_host_camera(short hostId, short hostcamera);
    int get_connect_mode(short scrcamera);
    void  decoder_key(const unsigned __int64& cameras, unsigned __int64& out) const;
    void  decoder_key_invisible(const unsigned __int64& visible, short hostId, unsigned __int64& out) const;
    g2::channels decoder_key_to_channels(const unsigned __int64& cameras) const;
    g2::channels decoder_key_invisible_to_channels(const unsigned __int64& visible, short hostId) const;

    int get_channel_ext(short scrcamera);
    unsigned __int64 get_cameras_by_host_id(short hostId);
    unsigned __int64 get_cameras_by_channel_ext(int channelext);
    unsigned __int64 get_cameras(void);

    short reference_camera(short hostId, int channelext, unsigned __int64& refcameras);
    short reference_camera(short hostId, short hostcamera, unsigned __int64& refcameras);

    std::set<int> get_channel_ext_by_host_id(short hostId, int connectMode = connect_mode_::UNDEFINED);

    /////////////////////////////////////////////

    bool remove_by_host_id(short hostId);
    bool remove_camera(short camera);

    /////////////////////////////////////////////

    int count_cameras(void);
    int count_cameras(short hostId);
    int count_cameras_from_channel_ext(short hostId, int channelext);
    int size(void) const { return static_cast<int>(_elements.size()); }
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#include "connective_camera_map.inl"

#endif // !_CONNECTIVE_CAMERA_MAP_H_
