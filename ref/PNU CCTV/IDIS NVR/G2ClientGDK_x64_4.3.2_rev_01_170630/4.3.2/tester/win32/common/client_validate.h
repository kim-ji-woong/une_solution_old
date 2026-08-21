// client_validate.h : header file
//

#ifndef _COMMON_CLIENT_VALIDATE_H_
#define _COMMON_CLIENT_VALIDATE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_guid.h>
#include "client_define.h"

namespace client {

//////////////////////////////////////////////////////////////////////////

inline bool valid_guid(const G2GUID& guid)
{
    return g2_guid_is_valid(&guid);
}

inline bool valid_host_id(short hostId)
{
    return (hostId > client::invalid_::HOST_ID &&
            hostId < client::MAX_CONNECTIVE_CHANNEL);
}

inline bool valid_channel(short channel)
{
    return (channel > client::invalid_::CHANNEL &&
            channel < client::MAX_CONNECTIVE_CHANNEL);
}

inline bool valid_channel_ext(int channelext)
{
    return (channelext > client::invalid_::CHANNEL_EXT);
}

inline bool valid_host_camera(short camera)
{
    return (camera >= 0 &&
            camera < client::max_::HOST_CAMERA_RAS);
}

inline bool valid_host_camera(short hostId, short camera)
{
    return (valid_host_id(hostId) &&
            valid_host_camera(camera));
}

inline bool valid_camera(short camera)
{
    return (camera > client::invalid_::CAMERA_NUMBER &&
            camera < client::MAX_SCREEN_CAMERA_COUNT);
}

inline bool valid_net_mode(int mode)
{
    return (mode >= 0 &&
            mode < client::connect_mode_::COUNT);
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_COMMON_CLIENT_VALIDATE_H_
