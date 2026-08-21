// g2client_admin.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_ADMIN_H_
#define _G2_CLIENT_DLL_SAMPLER_ADMIN_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define.h>
#include <include/g2_admin.h>
#include <boost/noncopyable.hpp>
#include <vector>

namespace client {
    class g2admin_listener;

//////////////////////////////////////////////////////////////////////////

class g2admin G2SEALED : private boost::noncopyable
{
protected:
    g2admin(void);

public:
    ~g2admin(void);

    static g2admin& get(void);
    static g2admin* from_handle(G2HADMIN handle);

private:
    G2HADMIN _handle;
    g2admin_listener* _listener;

public:
    void startup(void);
    void cleanup(void);
    void set_listener(g2admin_listener* listener);

    G2HADMIN safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    bool connect(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    bool connect_on_active_directory(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(void);
    bool is_connecting(void) const;
    bool is_connected(void) const;
    bool is_disconnecting(void) const;
    bool is_disconnected(void) const;
    bool is_disconnectable(void) const;

public:
    void set_disconnect_by_me(bool me);

public:
    bool request_recording_device_status(const G2GUID& root, bool invoke);
    bool request_recording_device_status(const std::vector<G2GUID>& roots, bool invoke);
    bool request_recording_instant(const G2GUID& camera, bool enable);
    bool request_system_log_search(const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option);
    bool request_system_log_search_stop(void);

public:
    G2GUID get_root_guid_from_leaf_guid(const G2GUID& device) const;
    G2GUID get_parent_guid_from_leaf_guid(const G2GUID& device) const;
    G2GUID get_parent_guid_from_address(const wchar_t* address) const;
    G2GUID get_parent_guid_from_dvrns_name(const wchar_t* name) const;
    G2GUID get_parent_guid_from_mac(const G2MAC_ADDRESS& mac) const;
    bool get_root_device_info(const G2GUID& guid, G2DEVICE_ROOT& device) const;
    bool get_parent_device_info(const G2GUID& guid, G2DEVICE_ROOT& device) const;
    bool get_leaf_device_info(const G2GUID& guid, G2DEVICE_LEAF& device) const;
    bool get_device_product_info(const G2GUID& device, G2_PRODUCT_INFO& pi) const;
    bool get_device_group_info(const G2GUID& guid, G2DEVICE_GROUP& group) const;
    bool get_layout_info(const G2GUID& guid, G2LAYOUT& layout) const;
    bool get_sequence_info(const G2GUID& guid, G2SEQUENCE& sequence) const;
    bool get_ref_network_text_in_from_camera(const G2GUID& camera, std::vector<G2GUID>& bunch) const;

    bool get_service_info(const G2GUID& guid, G2SERVICE& service) const;
    bool get_numeric_id_from_device(const G2GUID& device, G2DEVICE_NUMERIC_ID& numeric_id) const;
    G2GUID get_recording_service_guid_from_device(const G2GUID& device) const;
    G2GUID get_streaming_service_guid_from_device(const G2GUID& device) const;
    G2GUID get_backup_service_guid_from_device(const G2GUID& device, const G2GUID& backup, const G2GUID& site) const;
    bool get_camera_guid_from_recording_service(const G2GUID& service, std::vector<G2GUID>& bunch) const;
    bool get_camera_guid_from_backup_site(const G2GUID& service, const G2GUID& site, std::vector<G2GUID>& bunch) const;
    bool get_backup_service_guid_from_device(const G2GUID& device, std::vector<G2GUID>& bunch) const;
    bool get_backup_service_guid_from_device_site(const G2GUID& device, const G2GUID& site, std::vector<G2GUID>& bunch) const;
    bool get_backup_service_site_guid_from_device(const G2GUID& device, bool sort, std::vector<G2SERVICE_ITEM>& bunch) const;
    bool get_violet_service_guid_from_device(std::vector<G2GUID>& bunch) const;

    bool get_service_guid(std::vector<G2GUID>& bunch) const;
    bool get_root_guid(std::vector<G2GUID>& bunch) const;
    bool get_parent_guid(std::vector<G2GUID>& bunch) const;
    bool get_leaf_guid_from_root_guid(const G2GUID& root, std::vector<G2GUID>& bunch) const;
    bool get_leaf_guid_from_root_guid_reposit(const G2GUID& root, std::vector<G2GUID>& bunch) const;
    bool get_camera_guid_from_root_guid(const G2GUID& root, std::vector<G2GUID>& bunch) const;
    bool get_layout_guid(std::vector<G2GUID>& bunch) const;
	bool get_group_guid_from_device_guid(const G2GUID& device, std::vector<G2GUID>& bunch) const;
    bool get_service_network_info(const G2GUID& service, G2NETWORK_INFO& ni) const;
    bool get_device_network_info(const G2GUID& service, G2NETWORK_INFO& ni) const;

    int  get_channelext_from_device_guid(const G2GUID& service, const G2GUID& device) const;
    G2GUID get_device_guid_from_channelext(const G2GUID& service, int channelext) const;

public:
    bool is_disconnect_by_me(void) const;
    bool is_service_contains(const G2GUID& service) const;
    bool is_service_registered(const G2GUID& service) const;
    bool is_service_online(const G2GUID& service) const;
    bool is_service_offline(const G2GUID& service) const;
    bool is_support_backup_site(const G2GUID& guid) const;
    bool is_support_local_search(const G2GUID& device) const;
    bool is_support_local_search_g2(const G2GUID& device) const;
    bool is_device_contains(const G2GUID& device) const;
    bool is_device_enable(const G2GUID& device) const;
    bool is_device_access_right(const G2GUID& device) const;
    bool is_device_contains_group(const G2GUID& group, const G2GUID& device) const;
    bool is_device_dvr(const G2GUID& device) const;
    bool is_device_dvr_pc_based(const G2GUID& device) const;
    bool is_device_ipcamera(const G2GUID& device) const;
    bool is_device_onvif(const G2GUID& device) const;
    bool is_device_password_encrypted(const G2GUID& device) const;
    bool is_device_need_connect_rtp(const G2GUID& device) const;
    bool is_activate_service_record(const G2GUID& camera) const;
    bool is_activate_instant_record(const G2GUID& camera, bool check_recording = false) const;
    bool is_contains_instant_record(const G2GUID& camera) const;
    bool is_contains_instant_record_failed(const G2GUID& camera) const;

protected:
    static G2RESULT G2CALLBACK on_connected(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_login_failed_from_dvrns(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_login_completed(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_failover_prepare(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_failover_connected(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_failover_failed(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_notify_connectable_service(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_empty(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_remove_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_group_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_group_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_group_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_group_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_to_group_map(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_append_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_remove_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_list_append_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_list_remove_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_layout_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_layout_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_layout_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_layout_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_sequence_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_sequence_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_sequence_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_sequence_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_recording_device_status(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_end(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_fail(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_stop(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_modify_device_enable(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
	static G2RESULT G2CALLBACK on_receive_device_modify_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_ADMIN_H_
