// connective_find.h : header file
//

#ifndef _CONNECTIVE_FIND_H_
#define _CONNECTIVE_FIND_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <sampler/cpp/g2client_guid.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

int find_mode_from_host_id(short hostId);
int find_mode_from_camera(short camera);
short find_channel_from_host_id(short hostId);
short find_cameras_from_host_channel_ext(short hostId, int channelext, unsigned __int64& cameras);
unsigned __int64 find_cameras_from_host_id(short hostId);
short find_channel_from_camera(short camera);
short find_host_id_from_camera(short camera);
short find_host_camera_from_camera(short camera);
int find_channel_ext_from_camera(short camera);
G2SPOT find_last_spot_from_host_id(short hostId);
short find_last_camera_from_host_id(short hostId);
short find_cameras_from_host_camera(short hostId, short hostcamera, unsigned __int64& refcameras);
short find_host_camera_from_screen_camera(short camera);

template<typename OutputIterator>
OutputIterator find_root_guid_from_host_id(short hostId, OutputIterator out);

template<typename OutputIterator>
OutputIterator find_camera_guid_from_host_id(short hostId, OutputIterator out);

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONNECTIVE_FIND_H_
