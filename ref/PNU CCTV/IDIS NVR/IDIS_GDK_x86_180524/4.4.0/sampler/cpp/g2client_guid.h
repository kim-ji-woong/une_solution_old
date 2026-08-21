// g2client_guid.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_GUID_H_
#define _G2_CLIENT_DLL_SAMPLER_GUID_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_guid.h>
#include <string>
#include <functional>
#include <boost/unordered_set.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2guid : public G2GUID
{
public:
    g2guid(void);
    g2guid(const G2GUID& guid);
    explicit g2guid(const std::wstring& string);
    ~g2guid(void);

public:
    static G2GUID INVALID_GUID;
    static G2GUID create(void);

public:
    void set(const G2GUID& guid) { *this = guid; }
    void make_invalid(void);
    void from_string(const std::wstring& string);
    std::wstring to_string(void) const;
    size_t hash_value(void) const;

    bool is_valid(void) const;
    bool is_user(void) const;
    bool is_user_group(void) const;
    bool is_user_group_administrator(void) const;

public:
    const G2GUID& c_ref(void) const { return *this; }
    const G2GUID* c_ptr(void) const { return this;  }
    G2GUID& ref(void)  { return *this; }
    G2GUID* ptr(void)  { return this;  }

    operator G2GUID*() { return this;  }
    operator const G2GUID*()  const { return this;  }

    g2guid& operator=(const G2GUID& guid);
    bool operator< (const G2GUID& right) const;
    bool operator> (const G2GUID& right) const;
    bool operator<=(const G2GUID& right) const;
    bool operator>=(const G2GUID& right) const;
    bool operator==(const G2GUID& right) const;
    bool operator!=(const G2GUID& right) const;

public:
    struct fun_less : public std::less<G2GUID> {
        bool operator()(const G2GUID& left, const G2GUID& right) const;
    };
};

//////////////////////////////////////////////////////////////////////////

std::size_t hash_value(const G2GUID& guid);

//////////////////////////////////////////////////////////////////////////

bool is_service(const G2GUID& guid);
bool is_service_aministration(const G2GUID& guid);
bool is_service_federation(const G2GUID& guid);
bool is_service_recording(const G2GUID& guid);
bool is_service_recording_failover(const G2GUID& guid);
bool is_service_recording_redundant(const G2GUID& guid);
bool is_service_backup(const G2GUID& guid);
bool is_service_streaming(const G2GUID& guid);
bool is_service_analytics(const G2GUID& guid);
bool is_service_monitoring(const G2GUID& guid);
bool is_service_videowall(const G2GUID& guid);
bool is_service_videowall_failover(const G2GUID& guid);
bool is_device(const G2GUID& guid);
bool is_device_root(const G2GUID& guid);
bool is_device_parent(const G2GUID& guid);
bool is_device_leaf(const G2GUID& guid);
bool is_device_group(const G2GUID& guid);
bool is_device_camera(const G2GUID& guid);
bool is_device_stream(const G2GUID& guid);
bool is_device_alarm_in(const G2GUID& guid);
bool is_device_alarm_in_network(const G2GUID& guid);
bool is_device_alarm_out(const G2GUID& guid);
bool is_device_text_in(const G2GUID& guid);
bool is_device_text_in_network(const G2GUID& guid);
bool is_device_sdcard(const G2GUID& guid);
bool is_device_audio_in(const G2GUID& guid);
bool is_device_audio_out(const G2GUID& guid);
bool is_device_hybrided(const G2GUID& guid);
bool is_device_hybrid_parent(const G2GUID& guid);
bool is_device_hybrid_leaf(const G2GUID& guid);
bool is_device_hybrid_camera(const G2GUID& guid);
bool is_device_violet(const G2GUID& guid);
bool is_device_violet_agent(const G2GUID& guid);
bool is_device_violet_monitor(const G2GUID& guid);
bool is_device_violet_monitor_primary(const G2GUID& guid);
bool is_device_network_keyboard(const G2GUID& guid);
bool is_layout(const G2GUID& guid);
bool is_sequence(const G2GUID& guid);
bool is_sequence_camera(const G2GUID& guid);
bool is_sequence_layout(const G2GUID& guid);
bool is_map(const G2GUID& guid);
bool is_map_device(const G2GUID& guid);
bool is_map_shortcut(const G2GUID& guid);
bool is_map_camera(const G2GUID& guid);
bool is_map_alarm_in(const G2GUID& guid);
bool is_map_alarm_in_network(const G2GUID& guid);
bool is_map_alarm_out(const G2GUID& guid);
bool is_map_audio_in(const G2GUID& guid);
bool is_map_path_sequence(const G2GUID& guid);

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_GUID_H_
