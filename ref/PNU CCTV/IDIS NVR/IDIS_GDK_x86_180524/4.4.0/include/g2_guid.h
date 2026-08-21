// g2_guid.h : header file
//

#ifndef _G2_GUID_H_
#define _G2_GUID_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2GUID G2API g2_guid_from_string(const wchar_t* string);
G2_DLLFUNC void G2API g2_guid_to_string(const G2GUID* guid, G2STRING_46* string);
G2_DLLFUNC unsigned int G2API g2_guid_get_hash_value(const G2GUID* guid);
G2_DLLFUNC void G2API g2_guid_make_invalid(G2GUID* guid);

G2_DLLFUNC bool G2API g2_guid_is_less(const G2GUID* left, const G2GUID* right);
G2_DLLFUNC bool G2API g2_guid_is_greater(const G2GUID* left, const G2GUID* right);
G2_DLLFUNC bool G2API g2_guid_is_equal(const G2GUID* left, const G2GUID* right);

G2_DLLFUNC bool G2API g2_guid_is_valid(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_user(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_user_group(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_user_group_administrator(const G2GUID* guid);

G2_DLLFUNC bool G2API g2_guid_is_service(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_aministration(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_federation(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_recording(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_recording_failover(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_recording_redundant(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_backup(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_streaming(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_analytics(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_monitoring(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_monitoring_failover(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_videowall(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_service_videowall_failover(const G2GUID* guid);

G2_DLLFUNC bool G2API g2_guid_is_device(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_root(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_parent(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_leaf(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_group(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_camera(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_stream(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_alarm_in(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_alarm_in_network(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_alarm_out(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_text_in(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_text_in_network(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_sdcard(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_audio_in(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_audio_out(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_hybrided(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_hybrid_parent(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_hybrid_leaf(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_hybrid_camera(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_violet(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_violet_agent(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_violet_monitor(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_violet_monitor_primary(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_device_network_keyboard(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_layout(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_sequence(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_sequence_camera(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_sequence_layout(const G2GUID* guid);

G2_DLLFUNC bool G2API g2_guid_is_map(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_map_device(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_map_shortcut(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_map_camera(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_map_alarm_in(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_map_alarm_in_network(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_map_alarm_out(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_map_audio_in(const G2GUID* guid);
G2_DLLFUNC bool G2API g2_guid_is_map_path_sequence(const G2GUID* guid);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_GUID_H
