// g2_device_info.h : header file
//

#ifndef _G2_CLIENT_DLL_DEVICE_INFO_H_
#define _G2_CLIENT_DLL_DEVICE_INFO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_device_info.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_device_info_register_callback(G2HDEVICE_INFO handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HDEVICE_INFO G2API g2_device_info_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_device_info_finalize(G2HDEVICE_INFO handle);
G2_DLLFUNC void G2API g2_device_info_startup(G2HDEVICE_INFO handle, int connections);
G2_DLLFUNC void G2API g2_device_info_cleanup(G2HDEVICE_INFO handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_device_info_connect(G2HDEVICE_INFO handle, const G2GUID* root, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC int  G2API g2_device_info_connect_ras(G2HDEVICE_INFO handle, const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_device_info_disconnect(G2HDEVICE_INFO handle, int channel);
G2_DLLFUNC bool G2API g2_device_info_is_connecting(G2HDEVICE_INFO handle, int channel);
G2_DLLFUNC bool G2API g2_device_info_is_connected(G2HDEVICE_INFO handle, int channel);
G2_DLLFUNC bool G2API g2_device_info_is_disconnecting(G2HDEVICE_INFO handle, int channel);
G2_DLLFUNC bool G2API g2_device_info_is_disconnected(G2HDEVICE_INFO handle, int channel);
G2_DLLFUNC bool G2API g2_device_info_is_disconnectable(G2HDEVICE_INFO handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_device_info_get_product_info(G2HDEVICE_INFO handle, int channel, G2_PRODUCT_INFO* pi);
G2_DLLFUNC bool G2API g2_device_info_get_authority(G2HDEVICE_INFO handle, int channel, G2RAS_AUTHORITY* auth);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_device_info_is_authenticated(G2HDEVICE_INFO handle, int channel);
G2_DLLFUNC bool G2API g2_device_info_is_IDR_X032(G2HDEVICE_INFO handle, int channel);
G2_DLLFUNC bool G2API g2_device_info_is_IDR(G2HDEVICE_INFO handle, int channel);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_DEVICE_INFO_H_
