// connective_find.inl : implementation file
//

#include "stdafx.h"
#include "connective_find.h"
#include "G2ClientView.h"

#include <sampler/cpp/g2client_admin.h>
#include <include/g2_define_admin.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

namespace client {

//////////////////////////////////////////////////////////////////////////

int find_mode_from_host_id(short hostId)
{
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, connect_mode_::UNDEFINED);

    client::connective_host_list* hostList = runner->get_host_list();
    G2RETURN_VAL_IF_FAIL(hostList != NULL, connect_mode_::UNDEFINED);

    return hostList->find_mode_from_host_id(hostId);
}

int find_mode_from_camera(short camera)
{
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, connect_mode_::UNDEFINED);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, invalid_::CAMERA_NUMBER);

    return cameramap->get_connect_mode(camera);
}

short find_channel_from_host_id(short hostId)
{
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, invalid_::CHANNEL);

    client::connective_host_list* hostList = runner->get_host_list();
    G2RETURN_VAL_IF_FAIL(hostList != NULL, invalid_::CHANNEL);

    return hostList->find_channel_from_host_id(hostId);
}

short find_cameras_from_host_channel_ext(short hostId, int channelext, unsigned __int64& refcameras)
{
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, invalid_::CAMERA_NUMBER);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, invalid_::CAMERA_NUMBER);

    return cameramap->reference_camera(hostId, channelext, refcameras);
}

unsigned __int64 find_cameras_from_host_id(short hostId)
{
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, invalid_::CHANNEL);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, invalid_::CAMERA_NUMBER);

    return cameramap->get_cameras_by_host_id(hostId);
}

short find_channel_from_camera(short camera)
{
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, invalid_::CHANNEL);

    client::connective_host_list* hostList = runner->get_host_list();
    G2RETURN_VAL_IF_FAIL(hostList != NULL, invalid_::CHANNEL);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, invalid_::CAMERA_NUMBER);

    return hostList->find_channel_from_host_id(cameramap->get_host_id(camera));
}

short find_host_id_from_camera(short camera)
{
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, invalid_::CHANNEL);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, invalid_::CAMERA_NUMBER);

    return cameramap->get_host_id(camera);
}

short find_host_camera_from_camera(short camera)
{
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, invalid_::CHANNEL);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, invalid_::CAMERA_NUMBER);

    return cameramap->get_host_camera(camera);
}

int find_channel_ext_from_camera(short camera)
{
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, invalid_::CHANNEL_EXT);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, invalid_::CHANNEL_EXT);

    return cameramap->get_channel_ext(camera);
}

G2SPOT find_last_spot_from_host_id(short hostId)
{
    G2SPOT spot = { 0 };
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, spot);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, spot);

    unsigned __int64 cameras = cameramap->get_cameras_by_host_id(hostId); 
    client::screen_actuator& actuator = runner->actuator_ref();
    int camera = actuator.get_last_displayed_spot(cameras, spot);

    return spot;
}

short find_last_camera_from_host_id(short hostId)
{
    G2SPOT spot = { 0 };
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, client::invalid_::CAMERA_NUMBER);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, client::invalid_::CAMERA_NUMBER);

    unsigned __int64 cameras = cameramap->get_cameras_by_host_id(hostId);
    client::screen_actuator& actuator = runner->actuator_ref();
    return actuator.get_last_displayed_spot(cameras, spot);
}

short find_cameras_from_host_camera(short hostId, short hostcamera, unsigned __int64& refcameras)
{
    short basecamera = client::invalid_::CAMERA_NUMBER;
    refcameras = 0ui64;

    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, basecamera);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, basecamera);
    
    basecamera = cameramap->reference_camera(hostId, hostcamera, refcameras);
    
    return basecamera;
}

short find_host_camera_from_screen_camera(short camera)
{
    short hostcamera = client::invalid_::CAMERA_NUMBER;
    client::runner* runner = runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, hostcamera);

    client::camera_map* cameramap = runner->get_camera_map();
    G2RETURN_VAL_IF_FAIL(cameramap != NULL, hostcamera);

    hostcamera = cameramap->get_host_camera(camera);

    return hostcamera;
}

//////////////////////////////////////////////////////////////////////////

} // !_namespcae_client
