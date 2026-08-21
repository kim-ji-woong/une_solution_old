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

inline bool valid_channel(short channel)
{
    return (channel > client::invalid_::CHANNEL &&
            channel < client::MAX_CONNECTIVE_CHANNEL);
}

inline bool valid_channel_ext(int channelext)
{
    return (channelext > client::invalid_::CHANNEL_EXT);
}

inline bool valid_camera(short camera)
{
    return (camera > client::invalid_::CAMERA_NUMBER &&
            camera < client::MAX_SCREEN_CAMERA_COUNT);
}

inline bool is_valid(HWND handle)
{
    return (handle != NULL &&
            ::IsWindow(handle));
}

inline bool is_valid(HBRUSH handle) {
    return (handle != NULL &&
            ::GetObjectType(handle) == OBJ_BRUSH);
}

inline bool is_valid(const CWnd* wnd)
{
    return (wnd != NULL &&
            is_valid(wnd->GetSafeHwnd()));
}

inline bool is_valid(const CBrush* pobject)
{
    return (pobject != NULL &&
            is_valid((HBRUSH)pobject->GetSafeHandle()));
}

inline bool is_valid(const CGdiObject* pobject)
{
    return (pobject != NULL &&
            pobject->GetSafeHandle());
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_COMMON_CLIENT_VALIDATE_H_
