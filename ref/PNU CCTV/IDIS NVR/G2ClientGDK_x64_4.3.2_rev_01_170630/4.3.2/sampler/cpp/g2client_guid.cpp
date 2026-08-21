// g2client_guid.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_guid.h"

#if defined(_WIN32)
#if defined(_WIN64)
#pragma comment(lib, "G2ClientGDKx64.lib")
#else
#pragma comment(lib, "G2ClientGDK.lib")
#endif
#pragma warning(disable : 4244 4267 4312 4800)
#endif

namespace client {

//////////////////////////////////////////////////////////////////////////

g2guid::g2guid(void)
{
    set(INVALID_GUID);
}

g2guid::g2guid(const G2GUID& guid)
{
    set(guid);
}

g2guid::g2guid(const std::wstring& string)
{
    from_string(string);
}

g2guid::~g2guid(void)
{

}

//////////////////////////////////////////////////////////////////////////

G2GUID g2guid::INVALID_GUID = g2guid::create();
G2GUID g2guid::create(void)
{
    g2guid g;
    g.make_invalid();
    return g;
}

//////////////////////////////////////////////////////////////////////////

void g2guid::make_invalid(void)
{
    g2_guid_make_invalid(this);
}

void g2guid::from_string(const std::wstring& string)
{
    set(g2_guid_from_string(string.c_str()));
}

std::wstring g2guid::to_string(void) const
{
    G2STRING_46 s = { 0 };
    g2_guid_to_string(this, &s);
    return (s._len > 0) ? s._string : L"";
}

size_t g2guid::hash_value(void) const
{
    return static_cast<size_t>(g2_guid_get_hash_value(this));
}

//////////////////////////////////////////////////////////////////////////

bool g2guid::is_valid(void) const
{
    return g2_guid_is_valid(this);
}

bool g2guid::is_user(void) const
{
    return g2_guid_is_valid(this);
}

bool g2guid::is_user_group(void) const
{
    return g2_guid_is_valid(this);
}

bool g2guid::is_user_group_administrator(void) const
{
    return g2_guid_is_valid(this);
}

//////////////////////////////////////////////////////////////////////////

g2guid& g2guid::operator=(const G2GUID& guid)
{
    ref() = guid;
    return (*this);
}

bool g2guid::operator< (const G2GUID& right) const
{
    return g2_guid_is_less(this, &right);
}

bool g2guid::operator> (const G2GUID& right) const
{
    return g2_guid_is_greater(this, &right);
}

bool g2guid::operator<=(const G2GUID& right) const
{
    return !g2_guid_is_less(&right, this);
}

bool g2guid::operator>=(const G2GUID& right) const
{
    return !(operator<(right));
}

bool g2guid::operator==(const G2GUID& right) const
{
    return g2_guid_is_equal(this, &right);
}

bool g2guid::operator!=(const G2GUID& right) const
{
    return !(*this == right);
}

//////////////////////////////////////////////////////////////////////////

bool g2guid::fun_less::operator()(const G2GUID& left, const G2GUID& right) const
{
    return g2_guid_is_less(&left, &right);
}

//////////////////////////////////////////////////////////////////////////

std::size_t hash_value(const G2GUID& guid)
{
    return g2_guid_get_hash_value(&guid);
}

//////////////////////////////////////////////////////////////////////////

bool is_service(const G2GUID& guid)                         { return g2_guid_is_service(&guid); }
bool is_service_aministration(const G2GUID& guid)           { return g2_guid_is_service_aministration(&guid); }
bool is_service_federation(const G2GUID& guid)              { return g2_guid_is_service_federation(&guid); }
bool is_service_recording(const G2GUID& guid)               { return g2_guid_is_service_recording(&guid); }
bool is_service_recording_failover(const G2GUID& guid)      { return g2_guid_is_service_recording_failover(&guid); }
bool is_service_recording_redundant(const G2GUID& guid)     { return g2_guid_is_service_recording_redundant(&guid); }
bool is_service_backup(const G2GUID& guid)                  { return g2_guid_is_service_backup(&guid); }
bool is_service_streaming(const G2GUID& guid)               { return g2_guid_is_service_streaming(&guid); }
bool is_service_analytics(const G2GUID& guid)               { return g2_guid_is_service_analytics(&guid); }
bool is_service_monitoring(const G2GUID& guid)              { return g2_guid_is_service_monitoring(&guid); }
bool is_service_videowall(const G2GUID& guid)               { return g2_guid_is_service_videowall(&guid); }
bool is_service_videowall_failover(const G2GUID& guid)      { return g2_guid_is_service_videowall_failover(&guid); }
bool is_device(const G2GUID& guid)                          { return g2_guid_is_device(&guid); }
bool is_device_root(const G2GUID& guid)                     { return g2_guid_is_device_root(&guid); }
bool is_device_parent(const G2GUID& guid)                   { return g2_guid_is_device_parent(&guid); }
bool is_device_leaf(const G2GUID& guid)                     { return g2_guid_is_device_leaf(&guid); }
bool is_device_group(const G2GUID& guid)                    { return g2_guid_is_device_group(&guid); }
bool is_device_camera(const G2GUID& guid)                   { return g2_guid_is_device_camera(&guid); }
bool is_device_stream(const G2GUID& guid)                   { return g2_guid_is_device_stream(&guid); }
bool is_device_alarm_in(const G2GUID& guid)                 { return g2_guid_is_device_alarm_in(&guid); }
bool is_device_alarm_in_network(const G2GUID& guid)         { return g2_guid_is_device_alarm_in_network(&guid); }
bool is_device_alarm_out(const G2GUID& guid)                { return g2_guid_is_device_alarm_out(&guid); }
bool is_device_text_in(const G2GUID& guid)                  { return g2_guid_is_device_text_in(&guid); }
bool is_device_text_in_network(const G2GUID& guid)          { return g2_guid_is_device_text_in_network(&guid); }
bool is_device_sdcard(const G2GUID& guid)                   { return g2_guid_is_device_sdcard(&guid); }
bool is_device_audio_in(const G2GUID& guid)                 { return g2_guid_is_device_audio_in(&guid); }
bool is_device_audio_out(const G2GUID& guid)                { return g2_guid_is_device_audio_out(&guid); }
bool is_device_hybrided(const G2GUID& guid)                 { return g2_guid_is_device_hybrided(&guid); }
bool is_device_hybrid_parent(const G2GUID& guid)            { return g2_guid_is_device_hybrid_parent(&guid); }
bool is_device_hybrid_leaf(const G2GUID& guid)              { return g2_guid_is_device_hybrid_leaf(&guid); }
bool is_device_hybrid_camera(const G2GUID& guid)            { return g2_guid_is_device_hybrid_camera(&guid); }
bool is_device_violet(const G2GUID& guid)                   { return g2_guid_is_device_violet(&guid); }
bool is_device_violet_agent(const G2GUID& guid)             { return g2_guid_is_device_violet_agent(&guid); }
bool is_device_violet_monitor(const G2GUID& guid)           { return g2_guid_is_device_violet_monitor(&guid); }
bool is_device_violet_monitor_primary(const G2GUID& guid)   { return g2_guid_is_device_violet_monitor_primary(&guid); }
bool is_device_network_keyboard(const G2GUID& guid)         { return g2_guid_is_device_network_keyboard(&guid); }
bool is_layout(const G2GUID& guid)                          { return g2_guid_is_layout(&guid); }
bool is_sequence(const G2GUID& guid)                        { return g2_guid_is_sequence(&guid); }
bool is_sequence_camera(const G2GUID& guid)                 { return g2_guid_is_sequence_camera(&guid); }
bool is_sequence_layout(const G2GUID& guid)                 { return g2_guid_is_sequence_layout(&guid); }
bool is_map(const G2GUID& guid)                             { return g2_guid_is_map(&guid); }
bool is_map_device(const G2GUID& guid)                      { return g2_guid_is_map_device(&guid); }
bool is_map_shortcut(const G2GUID& guid)                    { return g2_guid_is_map_shortcut(&guid); }
bool is_map_camera(const G2GUID& guid)                      { return g2_guid_is_map_camera(&guid); }
bool is_map_alarm_in(const G2GUID& guid)                    { return g2_guid_is_map_alarm_in(&guid); }
bool is_map_alarm_in_network(const G2GUID& guid)            { return g2_guid_is_map_alarm_in_network(&guid); }
bool is_map_alarm_out(const G2GUID& guid)                   { return g2_guid_is_map_alarm_out(&guid); }
bool is_map_audio_in(const G2GUID& guid)                    { return g2_guid_is_map_audio_in(&guid); }
bool is_map_path_sequence(const G2GUID& guid)               { return g2_guid_is_map_path_sequence(&guid); }

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client
