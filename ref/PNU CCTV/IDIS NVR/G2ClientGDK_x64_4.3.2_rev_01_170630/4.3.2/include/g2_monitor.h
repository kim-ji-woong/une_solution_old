// g2_monitor.h : header file
//

#ifndef _G2_CLIENT_DLL_MONITOR_H_
#define _G2_CLIENT_DLL_MONITOR_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_monitor.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_monitor_register_callback(G2HMONITOR handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HMONITOR G2API g2_monitor_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_monitor_finalize(G2HMONITOR handle);
G2_DLLFUNC void G2API g2_monitor_startup(G2HMONITOR handle);
G2_DLLFUNC void G2API g2_monitor_cleanup(G2HMONITOR handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_monitor_connect(G2HMONITOR handle, const G2GUID* service, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_monitor_disconnect(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_is_connecting(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_is_connected(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_is_disconnecting(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_is_disconnected(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_is_disconnectable(G2HMONITOR handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_monitor_set_activate(G2HMONITOR handle, bool activate);
G2_DLLFUNC void G2API g2_monitor_set_disconnect_by_me(G2HMONITOR handle, bool me);
G2_DLLFUNC bool G2API g2_monitor_set_device_list_interest_for_live(G2HMONITOR handle, const G2GUID* devices, unsigned int count, bool invoke);
G2_DLLFUNC bool G2API g2_monitor_set_device_list_interest_for_status(G2HMONITOR handle, const G2GUID* devices, unsigned int count);
G2_DLLFUNC bool G2API g2_monitor_set_device_list_interest_for_health(G2HMONITOR handle, const G2GUID* devices, unsigned int count);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_monitor_request_alive_check(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_request_update_device_list_interest_for_status(G2HMONITOR hnalde, bool force);
G2_DLLFUNC bool G2API g2_monitor_request_update_device_list_interest_for_health(G2HMONITOR handle, bool force);
G2_DLLFUNC void G2API g2_monitor_request_delete_device_status_and_health(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_request_user_alarm_in_command(G2HMONITOR handle, const G2MONITORING_USER_ALARM_IN_REQUEST* request);
G2_DLLFUNC bool G2API g2_monitor_request_system_log_search(G2HMONITOR handle, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG* option);
G2_DLLFUNC bool G2API g2_monitor_request_system_log_search_stop(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_request_debug_log_search(G2HMONITOR handle, const G2SERVICE_SEARCH_OPTION_DEBUG_LOG* option);
G2_DLLFUNC bool G2API g2_monitor_request_debug_log_search_stop(G2HMONITOR handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_monitor_get_service_guid(G2HMONITOR handle, G2GUID* service);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_monitor_is_activate(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_is_monitoring(G2HMONITOR handle);
G2_DLLFUNC bool G2API g2_monitor_is_disconnect_by_me(G2HMONITOR handle);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_MONITOR_H_
