// g2_admin.h : header file
//

#ifndef _G2_CLIENT_DLL_ADMIN_H_
#define _G2_CLIENT_DLL_ADMIN_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_admin.h"
#include "g2_define_product.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_admin_register_callback(G2HADMIN handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HADMIN G2API g2_admin_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_admin_finalize(G2HADMIN handle);
G2_DLLFUNC void G2API g2_admin_startup(G2HADMIN handle);
G2_DLLFUNC void G2API g2_admin_cleanup(G2HADMIN handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_admin_connect(G2HADMIN handle, const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC bool G2API g2_admin_connect_failover(G2HADMIN handle, const G2NETWORK_INFO* ni, const G2NETWORK_INFO* fni, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC bool G2API g2_admin_connect_on_active_directory(G2HADMIN handle, const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_admin_disconnect(G2HADMIN handle);
G2_DLLFUNC bool G2API g2_admin_is_connecting(G2HADMIN handle);
G2_DLLFUNC bool G2API g2_admin_is_connected(G2HADMIN handle);
G2_DLLFUNC bool G2API g2_admin_is_disconnecting(G2HADMIN handle);
G2_DLLFUNC bool G2API g2_admin_is_disconnected(G2HADMIN handle);
G2_DLLFUNC bool G2API g2_admin_is_disconnectable(G2HADMIN handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_admin_set_disconnect_by_me(G2HADMIN handle, bool me);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_admin_request_alive_check(G2HADMIN handle);
G2_DLLFUNC bool G2API g2_admin_request_test_failover(G2HADMIN handle, const G2GUID* service, const G2SERVICE_NETWORK_INFO* sni, const G2NETWORK_INFO* ni);
G2_DLLFUNC bool G2API g2_admin_request_recording_device_status(G2HADMIN handle, const G2GUID* root, bool invoke);
G2_DLLFUNC bool G2API g2_admin_request_recording_device_status_v2(G2HADMIN handle, const G2GUID roots[], unsigned int count, bool invoke);
G2_DLLFUNC bool G2API g2_admin_request_recording_instant(G2HADMIN handle, const G2GUID* camera, bool enable);
G2_DLLFUNC bool G2API g2_admin_request_system_log_search(G2HADMIN handle, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG* option);
G2_DLLFUNC bool G2API g2_admin_request_system_log_search_stop(G2HADMIN handle);
G2_DLLFUNC bool G2API g2_admin_request_debug_log_search(G2HADMIN handle, const G2SERVICE_SEARCH_OPTION_DEBUG_LOG* option);
G2_DLLFUNC bool G2API g2_admin_request_debug_log_search_stop(G2HADMIN handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2GUID G2API g2_admin_get_root_guid_from_leaf_guid(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC G2GUID G2API g2_admin_get_parent_guid_from_leaf_guid(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC G2GUID G2API g2_admin_get_parent_guid_from_address(G2HADMIN handle, const wchar_t* address);
G2_DLLFUNC G2GUID G2API g2_admin_get_parent_guid_from_dvrns_name(G2HADMIN handle, const wchar_t* name);
G2_DLLFUNC G2GUID G2API g2_admin_get_parent_guid_from_mac(G2HADMIN handle, const G2MAC_ADDRESS* mac);
G2_DLLFUNC bool G2API g2_admin_get_root_device_info(G2HADMIN handle, const G2GUID* guid, G2DEVICE_ROOT* device);
G2_DLLFUNC bool G2API g2_admin_get_parent_device_info(G2HADMIN handle, const G2GUID* guid, G2DEVICE_ROOT* device);
G2_DLLFUNC bool G2API g2_admin_get_leaf_device_info(G2HADMIN handle, const G2GUID* guid, G2DEVICE_LEAF* device);
G2_DLLFUNC bool G2API g2_admin_get_device_product_info(G2HADMIN handle, const G2GUID* device, G2_PRODUCT_INFO* pi);
G2_DLLFUNC bool G2API g2_admin_get_device_group_info(G2HADMIN handle, const G2GUID* guid, G2DEVICE_GROUP* group);
G2_DLLFUNC bool G2API g2_admin_get_layout_info(G2HADMIN handle, const G2GUID* guid, G2LAYOUT* layout);
G2_DLLFUNC bool G2API g2_admin_get_sequence_info(G2HADMIN handle, const G2GUID* guid, G2SEQUENCE* sequence);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_admin_get_ref_text_in_network_from_camera(G2HADMIN handle, const G2GUID* camera, G2GUID_8* out);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_admin_get_service_version(G2HADMIN handle, G2SERVICE_VERSION* version);
G2_DLLFUNC bool G2API g2_admin_get_service_info(G2HADMIN handle, const G2GUID* guid, G2SERVICE* service);
G2_DLLFUNC bool G2API g2_admin_get_numeric_id_from_device(G2HADMIN handle, const G2GUID* device, G2DEVICE_NUMERIC_ID* id);
G2_DLLFUNC G2GUID G2API g2_admin_get_recording_service_guid_from_device(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC G2GUID G2API g2_admin_get_streaming_service_guid_from_device(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC G2GUID G2API g2_admin_get_backup_service_guid_from_device(G2HADMIN handle, const G2GUID* device, const G2GUID* backup, const G2GUID* site);
G2_DLLFUNC bool G2API g2_admin_enum_camera_guid_from_recording_service(G2HADMIN handle, const G2GUID* service, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_camera_guid_from_backup_site(G2HADMIN handle, const G2GUID* service, const G2GUID* site, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_backup_service_guid_from_device(G2HADMIN handle, const G2GUID* device, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_backup_service_guid_from_device_site(G2HADMIN handle, const G2GUID* device, const G2GUID* site, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_backup_service_site_guid_from_device(G2HADMIN handle, const G2GUID* device, bool sort, G2SERVICE_ITEM_ENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_violet_service_guid(G2HADMIN handle, G2GUIDENUMPROC proc, G2UPARAM param);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_admin_enum_service_guid(G2HADMIN handle, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_root_guid(G2HADMIN handle, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_parent_guid(G2HADMIN handle, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_leaf_guid_from_root_guid(G2HADMIN handle, const G2GUID* device, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_leaf_guid_from_root_guid_reposit(G2HADMIN handle, const G2GUID* device, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_camera_guid_from_root_guid(G2HADMIN handle, const G2GUID* device, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_layout_guid(G2HADMIN handle, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_enum_group_guid_from_device_guid(G2HADMIN handle, const G2GUID* device, G2GUIDENUMPROC proc, G2UPARAM param);
G2_DLLFUNC bool G2API g2_admin_get_service_network_info(G2HADMIN handle, const G2GUID* service, G2NETWORK_INFO* ni);
G2_DLLFUNC bool G2API g2_admin_get_device_network_info(G2HADMIN handle, const G2GUID* device, G2NETWORK_INFO* ni);
G2_DLLFUNC bool G2API g2_admin_get_spot_current(G2HADMIN handle, G2SPOT* spot);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int    G2API g2_admin_get_channelext_from_device_guid(G2HADMIN handle, const G2GUID* service, const G2GUID* device);
G2_DLLFUNC G2GUID G2API g2_admin_get_device_guid_from_channelext(G2HADMIN handle, const G2GUID* service, int channelext);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_admin_is_disconnect_by_me(G2HADMIN handle);
G2_DLLFUNC bool G2API g2_admin_is_service_contains(G2HADMIN handle, const G2GUID* service);
G2_DLLFUNC bool G2API g2_admin_is_service_registered(G2HADMIN handle, const G2GUID* service);
G2_DLLFUNC bool G2API g2_admin_is_service_online(G2HADMIN handle, const G2GUID* service);
G2_DLLFUNC bool G2API g2_admin_is_service_offline(G2HADMIN handle, const G2GUID* service);
G2_DLLFUNC bool G2API g2_admin_is_support_backup_site(G2HADMIN handle, const G2GUID* guid);
G2_DLLFUNC bool G2API g2_admin_is_support_local_search(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_support_local_search_g2(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_contains(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_enable(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_access_right(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_contains_group(G2HADMIN handle, const G2GUID* group, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_dvr(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_dvr_pc_based(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_ipcamera(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_onvif(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_password_encrypted(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_device_need_connect_rtp(G2HADMIN handle, const G2GUID* device);
G2_DLLFUNC bool G2API g2_admin_is_registered_request_recording_status(const G2GUID* root);
G2_DLLFUNC bool G2API g2_admin_is_activate_service_record(const G2GUID* camera);
G2_DLLFUNC bool G2API g2_admin_is_activate_instant_record(const G2GUID* camera, bool check_recording);
G2_DLLFUNC bool G2API g2_admin_is_contains_instant_record(const G2GUID* camera);
G2_DLLFUNC bool G2API g2_admin_is_contains_instant_record_failed(const G2GUID* camera);
G2_DLLFUNC bool G2API g2_admin_is_device_primary_recording_has_device(const G2GUID* root);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_ADMIN_H_
