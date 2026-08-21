// g2_foundation.h : header file
//

#ifndef _G2_CLIENT_DLL_FOUNDATION_H_
#define _G2_CLIENT_DLL_FOUNDATION_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_time_make_invalid(G2TIME* time);
G2_DLLFUNC void G2API g2_time_get_current(G2TIME* time);
G2_DLLFUNC bool G2API g2_time_is_valid(const G2TIME* time);
G2_DLLFUNC bool G2API g2_time_is_same_date(const G2TIME* left, const G2TIME* right);
G2_DLLFUNC bool G2API g2_time_is_same_hour(const G2TIME* left, const G2TIME* right);
G2_DLLFUNC bool G2API g2_time_is_same_minute(const G2TIME* left, const G2TIME* right);
G2_DLLFUNC int  G2API g2_time_compare(const G2TIME* left, const G2TIME* right);
G2_DLLFUNC int  G2API g2_time_to_utc32(const G2TIME* time);
G2_DLLFUNC int  G2API g2_time_to_time32_t(const G2TIME* time);
G2_DLLFUNC int  G2API g2_time_get_year(const G2TIME* time);
G2_DLLFUNC int  G2API g2_time_get_month(const G2TIME* time);
G2_DLLFUNC int  G2API g2_time_get_day(const G2TIME* time);
G2_DLLFUNC int  G2API g2_time_get_hour(const G2TIME* time);
G2_DLLFUNC int  G2API g2_time_get_minute(const G2TIME* time);
G2_DLLFUNC int  G2API g2_time_get_second(const G2TIME* time);
G2_DLLFUNC bool G2API g2_time_to_elements(const G2TIME* time, int* year, int* month, int* day, int* hour, int* minute, int* second);
G2_DLLFUNC bool G2API g2_time_from_utc(G2TIME* time, int utc);
G2_DLLFUNC bool G2API g2_time_from_uint32(G2TIME* time, unsigned int val);
G2_DLLFUNC bool G2API g2_time_from_time32_t(G2TIME* time, int val);
G2_DLLFUNC bool G2API g2_time_from_elements(G2TIME* time, int year, int month, int day, int hour, int minute, int second);

G2_DLLFUNC int G2API g2_time_span_total_hours(const G2TIME_SPAN* ts);
G2_DLLFUNC int G2API g2_time_span_total_minutes(const G2TIME_SPAN* ts);
G2_DLLFUNC int G2API g2_time_span_total_seconds(const G2TIME_SPAN* ts);
G2_DLLFUNC void G2API g2_time_span_set_total_seconds(G2TIME_SPAN* ts, int seconds);
G2_DLLFUNC G2TIME_SPAN G2API g2_time_subtract_time(const G2TIME* left, const G2TIME* right);
G2_DLLFUNC G2TIME G2API g2_time_plus_span(const G2TIME* left, const G2TIME_SPAN* ts);
G2_DLLFUNC G2TIME G2API g2_time_subtract_span(const G2TIME* left, const G2TIME_SPAN* ts);
G2_DLLFUNC int G2API g2_time_span_compare(const G2TIME_SPAN* left, const G2TIME_SPAN* right);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_spot_make_invalid(G2SPOT* spot);
G2_DLLFUNC bool G2API g2_spot_is_valid(const G2SPOT* spot);
G2_DLLFUNC int  G2API g2_spot_compare(const G2SPOT* left, const G2SPOT* right);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_scope_make_invalid(G2SCOPE* scope);
G2_DLLFUNC bool G2API g2_scope_is_overlap(const G2SCOPE* left, const G2SCOPE* right);
G2_DLLFUNC bool G2API g2_scope_contains_spot(const G2SCOPE* scope, const G2SPOT* spot);
G2_DLLFUNC bool G2API g2_scope_contains_day(const G2SCOPE* scope, const G2TIME* time);
G2_DLLFUNC bool G2API g2_scope_contains_hour(const G2SCOPE* scope, const G2TIME* time);
G2_DLLFUNC bool G2API g2_scope_contains_min(const G2SCOPE* scope, const G2TIME* time);
G2_DLLFUNC G2SCOPE G2API g2_scope_get_overlapped_scope(const G2SCOPE* left, const G2SCOPE* right);
G2_DLLFUNC G2SCOPE G2API g2_scope_get_merged_scope(const G2SCOPE* left, const G2SCOPE* right);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_channelset_from_array(G2CHANNEL_SET* channelset, int channels[], unsigned int size);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_get_string_disconnect_reason(int reason, G2STRING_128* string);
G2_DLLFUNC void G2API g2_get_string_disconnect_reason_service(int reason, int reason_user, G2STRING_128* string);
G2_DLLFUNC void G2API g2_get_string_dvrns_error(int error, G2STRING_128* string);
G2_DLLFUNC void G2API g2_get_string_service_log(const G2GUID* service, const G2SYSTEM_LOG* log, G2STRING_128* string);
G2_DLLFUNC void G2API g2_get_string_event_type(int level1, int level2, G2STRING_64* string);
G2_DLLFUNC void G2API g2_get_string_event_type_network(int type, G2STRING_64* string);
G2_DLLFUNC void G2API g2_get_string_event_type_g2(int type, G2STRING_64* string);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_get_event_info_type_from_network_event(int evt, int* level1, int* level2);
G2_DLLFUNC bool G2API g2_get_event_info_type_from_g2event(int evt, int* level1, int* level2);
G2_DLLFUNC bool G2API g2_event_info_is_camera(int level2);
G2_DLLFUNC bool G2API g2_event_info_is_alarm(int level2);
G2_DLLFUNC bool G2API g2_event_info_is_alarm_network(int level2);
G2_DLLFUNC bool G2API g2_event_info_is_audio(int level2);
G2_DLLFUNC bool G2API g2_event_info_is_text_in(int level2);
G2_DLLFUNC bool G2API g2_event_info_is_dvr_system(int level2);
G2_DLLFUNC bool G2API g2_event_info_is_ignored(int level2);
G2_DLLFUNC bool G2API g2_event_info_is_off(int level2);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_event_is_disk(int type);
G2_DLLFUNC bool G2API g2_event_is_gps(int type);
G2_DLLFUNC bool G2API g2_event_is_text_in(int type);
G2_DLLFUNC bool G2API g2_event_is_secom(int type);
G2_DLLFUNC bool G2API g2_event_is_network_alarm(int type);
G2_DLLFUNC bool G2API g2_event_is_user_defined(int type);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_string_encrypt_by_sha256(const wchar_t* text, G2STRING_256* out);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_disk_get_free_space(const wchar_t* dir, unsigned __int64* free_bytes_available, unsigned __int64* total_number_of_bytes, unsigned __int64* total_number_of_free_bytes);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_FOUNDATION_H_
