// g2client_admin.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_admin.h"
#include "g2client_admin_listener.h"

#include <assert.h>

#if defined(_WIN32)
#if defined(_WIN64)
#pragma comment(lib, "G2ClientGDKx64.lib")
#else
#pragma comment(lib, "G2ClientGDK.lib")
#endif
#pragma warning(disable : 4244 4267 4312 4800)
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

#define REGISTER_CALLBACK(function) \
    g2_admin_register_callback(_handle, G2ADMIN_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2admin_listener* ptr = ((g2admin*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

namespace {
    struct guid_enum_proc_impl {
        static G2BOOL G2CALLBACK proc(const G2GUID* guid, G2UPARAM param) {
            assert(guid != NULL);
            std::vector<G2GUID>* buff = (std::vector<G2GUID>*)(param);
            if (buff) buff->push_back(*guid);
            return G2TRUE;
        }
    };
    struct service_item_enum_proc_impl {
        static G2BOOL G2CALLBACK proc(const G2GUID* service, const G2GUID* site, G2UPARAM param) {
            assert(service != NULL && site != NULL);
            std::vector<G2SERVICE_ITEM>* buff = (std::vector<G2SERVICE_ITEM>*)(param);
            G2SERVICE_ITEM item;
            item._service = *service;
            item._site = *site;
            if (buff) buff->push_back(item);
            return G2TRUE;
        }
    };
}

//////////////////////////////////////////////////////////////////////////

g2admin::g2admin(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_admin_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_login_failed_from_dvrns);
    REGISTER_CALLBACK(on_login_completed);
    REGISTER_CALLBACK(on_failover_prepare);
    REGISTER_CALLBACK(on_failover_connected);
    REGISTER_CALLBACK(on_failover_failed);
    REGISTER_CALLBACK(on_notify_connectable_service)
    REGISTER_CALLBACK(on_receive_device_empty);
    REGISTER_CALLBACK(on_receive_device_append);
    REGISTER_CALLBACK(on_receive_device_modify);
    REGISTER_CALLBACK(on_receive_device_remove);
    REGISTER_CALLBACK(on_receive_device_remove_list);
    REGISTER_CALLBACK(on_receive_device_group_list);
    REGISTER_CALLBACK(on_receive_device_group_append);
    REGISTER_CALLBACK(on_receive_device_group_modify);
    REGISTER_CALLBACK(on_receive_device_group_remove);
    REGISTER_CALLBACK(on_receive_device_to_group_map);
    REGISTER_CALLBACK(on_receive_device_append_to_group);
    REGISTER_CALLBACK(on_receive_device_remove_to_group);
    REGISTER_CALLBACK(on_receive_device_list)
    REGISTER_CALLBACK(on_receive_device_list_append_to_group);
    REGISTER_CALLBACK(on_receive_device_list_remove_to_group);
    REGISTER_CALLBACK(on_receive_layout_list);
    REGISTER_CALLBACK(on_receive_layout_append);
    REGISTER_CALLBACK(on_receive_layout_modify);
    REGISTER_CALLBACK(on_receive_layout_remove);
    REGISTER_CALLBACK(on_receive_sequence_list);
    REGISTER_CALLBACK(on_receive_sequence_append);
    REGISTER_CALLBACK(on_receive_sequence_modify);
    REGISTER_CALLBACK(on_receive_sequence_remove);
    REGISTER_CALLBACK(on_receive_recording_device_status);
    REGISTER_CALLBACK(on_receive_service_log_load);
    REGISTER_CALLBACK(on_receive_service_log_load_end);
    REGISTER_CALLBACK(on_receive_service_log_load_fail);
    REGISTER_CALLBACK(on_receive_service_log_load_stop);
    REGISTER_CALLBACK(on_modify_device_enable);
	REGISTER_CALLBACK(on_receive_device_modify_list);
}

g2admin::~g2admin(void)
{
    g2_admin_finalize(_handle);
    _handle = G2HNULL;
}

g2admin& g2admin::get(void)
{
    static g2admin s_singleton;
    return s_singleton;
}

g2admin* g2admin::from_handle(G2HADMIN handle)
{
    return &get();
}

//////////////////////////////////////////////////////////////////////////

void g2admin::startup(void)
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    g2_admin_startup(_handle);
}

void g2admin::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    g2_admin_cleanup(_handle);
}

void g2admin::set_listener(g2admin_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

bool g2admin::connect(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    return g2_admin_connect(_handle, &ni, options, res);
}

bool g2admin::connect_on_active_directory(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    return g2_admin_connect_on_active_directory(_handle, &ni, options, res);
}

void g2admin::disconnect(void)
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    g2_admin_disconnect(_handle);
}

bool g2admin::is_connecting(void) const
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    return g2_admin_is_connecting(_handle);
}

bool g2admin::is_connected(void) const
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    return g2_admin_is_connected(_handle);
}

bool g2admin::is_disconnecting(void) const
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    return g2_admin_is_disconnecting(_handle);
}

bool g2admin::is_disconnected(void) const
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    return g2_admin_is_disconnected(_handle);
}

bool g2admin::is_disconnectable(void) const
{
    assert(_handle != G2HNULL && "g2client_amdin is not initialized");
    return g2_admin_is_disconnectable(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2admin::set_disconnect_by_me(bool me)
{
    g2_admin_set_disconnect_by_me(_handle, me);
}

//////////////////////////////////////////////////////////////////////////

bool g2admin::request_recording_device_status(const G2GUID& root, bool invoke)
{
    return g2_admin_request_recording_device_status(_handle, &root, invoke);
}

bool g2admin::request_recording_device_status(const std::vector<G2GUID>& roots, bool invoke)
{
    return g2_admin_request_recording_device_status_v2(_handle, roots.empty() ? NULL : &roots[0], roots.size(), invoke);
}

bool g2admin::request_recording_instant(const G2GUID& camera, bool enable)
{
    return g2_admin_request_recording_instant(_handle, &camera, enable);
}

bool g2admin::request_system_log_search(const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option)
{
    return g2_admin_request_system_log_search(_handle, &option);
}

bool g2admin::request_system_log_search_stop(void)
{
    return g2_admin_request_system_log_search_stop(_handle);
}

//////////////////////////////////////////////////////////////////////////

G2GUID g2admin::get_root_guid_from_leaf_guid(const G2GUID& device) const
{
    return g2_admin_get_root_guid_from_leaf_guid(_handle, &device);
}

G2GUID g2admin::get_parent_guid_from_leaf_guid(const G2GUID& device) const
{
    return g2_admin_get_parent_guid_from_leaf_guid(_handle, &device);
}

G2GUID g2admin::get_parent_guid_from_address(const wchar_t* address) const
{
    return g2_admin_get_parent_guid_from_address(_handle, address);
}

G2GUID g2admin::get_parent_guid_from_dvrns_name(const wchar_t* name) const
{
    return g2_admin_get_parent_guid_from_dvrns_name(_handle, name);
}

G2GUID g2admin::get_parent_guid_from_mac(const G2MAC_ADDRESS& mac) const
{
    return g2_admin_get_parent_guid_from_mac(_handle, &mac);
}

bool g2admin::get_root_device_info(const G2GUID& guid, G2DEVICE_ROOT& device) const
{
    return g2_admin_get_root_device_info(_handle, &guid, &device);
}

bool g2admin::get_parent_device_info(const G2GUID& guid, G2DEVICE_ROOT& device) const
{
    return g2_admin_get_parent_device_info(_handle, &guid, &device);
}

bool g2admin::get_leaf_device_info(const G2GUID& guid, G2DEVICE_LEAF& device) const
{
    return g2_admin_get_leaf_device_info(_handle, &guid, &device);
}

bool g2admin::get_device_product_info(const G2GUID& device, G2_PRODUCT_INFO& pi) const
{
    return g2_admin_get_device_product_info(_handle, &device, &pi);
}

bool g2admin::get_device_group_info(const G2GUID& guid, G2DEVICE_GROUP& group) const
{
    return g2_admin_get_device_group_info(_handle, &guid, &group);
}

bool g2admin::get_layout_info(const G2GUID& guid, G2LAYOUT& layout) const
{
    return g2_admin_get_layout_info(_handle, &guid, &layout);
}

bool g2admin::get_sequence_info(const G2GUID& guid, G2SEQUENCE& sequence) const
{
    return g2_admin_get_sequence_info(_handle, &guid, &sequence);
}

bool g2admin::get_ref_network_text_in_from_camera(const G2GUID& camera, std::vector<G2GUID>& bunch) const
{
    G2GUID_8 list;
    if (g2_admin_get_ref_text_in_network_from_camera(_handle, &camera, &list)) {
        std::vector<G2GUID>().swap(bunch);
        bunch.assign(list._guid, list._guid + list._len);
        return true;
    }
    return false;
}

//////////////////////////////////////////////////////////////////////////

bool g2admin::get_service_info(const G2GUID& guid, G2SERVICE& service) const
{
    return g2_admin_get_service_info(_handle, &guid, &service);
}

bool g2admin::get_numeric_id_from_device(const G2GUID& device, G2DEVICE_NUMERIC_ID& numeric_id) const
{
    return g2_admin_get_numeric_id_from_device(_handle, &device, &numeric_id);
}

G2GUID g2admin::get_recording_service_guid_from_device(const G2GUID& device) const
{
    return g2_admin_get_recording_service_guid_from_device(_handle, &device);
}

G2GUID g2admin::get_streaming_service_guid_from_device(const G2GUID& device) const
{
    return g2_admin_get_streaming_service_guid_from_device(_handle, &device);
}

G2GUID g2admin::get_backup_service_guid_from_device(const G2GUID& device, const G2GUID& backup, const G2GUID& site) const
{
    return g2_admin_get_backup_service_guid_from_device(_handle, &device, &backup, &site);
}

bool g2admin::get_camera_guid_from_recording_service(const G2GUID& service, std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_camera_guid_from_recording_service(_handle, &service, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_camera_guid_from_backup_site(const G2GUID& service, const G2GUID& site, std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_camera_guid_from_backup_site(_handle, &service, &site, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_backup_service_guid_from_device(const G2GUID& device, std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_backup_service_guid_from_device(_handle, &device, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_backup_service_guid_from_device_site(const G2GUID& device, const G2GUID& site, std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_backup_service_guid_from_device_site(_handle, &device, &site, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_backup_service_site_guid_from_device(const G2GUID& device, bool sort, std::vector<G2SERVICE_ITEM>& bunch) const
{
    std::vector<G2SERVICE_ITEM>().swap(bunch);
    return g2_admin_enum_backup_service_site_guid_from_device(_handle, &device, sort, service_item_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_violet_service_guid_from_device(std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_violet_service_guid(_handle, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

//////////////////////////////////////////////////////////////////////////

bool g2admin::get_service_guid(std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_service_guid(_handle, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_root_guid(std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_root_guid(_handle, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_parent_guid(std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_parent_guid(_handle, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_leaf_guid_from_root_guid(const G2GUID& root, std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_leaf_guid_from_root_guid(_handle, &root, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_leaf_guid_from_root_guid_reposit(const G2GUID& root, std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_leaf_guid_from_root_guid_reposit(_handle, &root, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_camera_guid_from_root_guid(const G2GUID& root, std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_camera_guid_from_root_guid(_handle, &root, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_layout_guid(std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_layout_guid(_handle, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_group_guid_from_device_guid(const G2GUID& device, std::vector<G2GUID>& bunch) const
{
    std::vector<G2GUID>().swap(bunch);
    return g2_admin_enum_group_guid_from_device_guid(_handle, &device, guid_enum_proc_impl::proc, (G2UPARAM)(&bunch));
}

bool g2admin::get_service_network_info(const G2GUID& service, G2NETWORK_INFO& ni) const
{
    return g2_admin_get_service_network_info(_handle, &service, &ni);
}

bool g2admin::get_device_network_info(const G2GUID& device, G2NETWORK_INFO& ni) const
{
    return g2_admin_get_device_network_info(_handle, &device, &ni);
}

//////////////////////////////////////////////////////////////////////////

int g2admin::get_channelext_from_device_guid(const G2GUID& service, const G2GUID& device) const
{
    return g2_admin_get_channelext_from_device_guid(_handle, &service, &device);
}

G2GUID g2admin::get_device_guid_from_channelext(const G2GUID& service, int channelext) const
{
    return g2_admin_get_device_guid_from_channelext(_handle, &service, channelext);
}

//////////////////////////////////////////////////////////////////////////

bool g2admin::is_disconnect_by_me(void) const
{
    return g2_admin_is_disconnect_by_me(_handle);
}

bool g2admin::is_service_contains(const G2GUID& service) const
{
    return g2_admin_is_service_contains(_handle, &service);
}

bool g2admin::is_service_registered(const G2GUID& service) const
{
    return g2_admin_is_service_registered(_handle, &service);
}

bool g2admin::is_service_online(const G2GUID& service) const
{
    return g2_admin_is_service_online(_handle, &service);
}

bool g2admin::is_service_offline(const G2GUID& service) const
{
    return g2_admin_is_service_offline(_handle, &service);
}

bool g2admin::is_support_backup_site(const G2GUID& guid) const
{
    return g2_admin_is_support_backup_site(_handle, &guid);
}

bool g2admin::is_support_local_search(const G2GUID& device) const
{
    return g2_admin_is_support_local_search(_handle, &device);
}

bool g2admin::is_support_local_search_g2(const G2GUID& device) const
{
    return g2_admin_is_support_local_search_g2(_handle, &device);
}

bool g2admin::is_device_contains(const G2GUID& device) const
{
    return g2_admin_is_device_contains(_handle, &device);
}

bool g2admin::is_device_enable(const G2GUID& device) const
{
    return g2_admin_is_device_enable(_handle, &device);
}

bool g2admin::is_device_access_right(const G2GUID& device) const
{
    return g2_admin_is_device_access_right(_handle, &device);
}

bool g2admin::is_device_contains_group(const G2GUID& group, const G2GUID& device) const
{
    return g2_admin_is_device_contains_group(_handle, &group, &device);
}

bool g2admin::is_device_dvr(const G2GUID& device) const
{
    return g2_admin_is_device_dvr(_handle, &device);
}

bool g2admin::is_device_dvr_pc_based(const G2GUID& device) const
{
    return g2_admin_is_device_dvr_pc_based(_handle, &device);
}

bool g2admin::is_device_ipcamera(const G2GUID& device) const
{
    return g2_admin_is_device_ipcamera(_handle, &device);
}

bool g2admin::is_device_onvif(const G2GUID& device) const
{
    return g2_admin_is_device_onvif(_handle, &device);
}

bool g2admin::is_device_password_encrypted(const G2GUID& device) const
{
    return g2_admin_is_device_password_encrypted(_handle, &device);
}

bool g2admin::is_device_need_connect_rtp(const G2GUID& device) const
{
    return g2_admin_is_device_need_connect_rtp(_handle, &device);
}

bool g2admin::is_activate_service_record(const G2GUID& camera) const
{
    return g2_admin_is_activate_service_record(&camera);
}

bool g2admin::is_activate_instant_record(const G2GUID& camera, bool check_recording /*= false*/) const
{
    return g2_admin_is_activate_instant_record(&camera, check_recording);
}

bool g2admin::is_contains_instant_record(const G2GUID& camera) const
{
    return g2_admin_is_contains_instant_record(&camera);
}

bool g2admin::is_contains_instant_record_failed(const G2GUID& camera) const
{
    return g2_admin_is_contains_instant_record_failed(&camera);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2admin::on_connected(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2admin_connected(handle));
    return 1L;
}

G2RESULT g2admin::on_disconnected(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2admin_disconnected(handle, (G2DISCONNECT_REASON::TYPE)wparam, (G2SERVICE_LOGIN_FAIL_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2admin::on_login_failed_from_dvrns(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2NETWORK_INFO* ni = (G2NETWORK_INFO*)(wparam);
    int error = wparam;
    CALL_LISTENER(on_g2admin_login_failed_from_dvrns(handle, *ni, (G2FEN_RESULT::TYPE)error));
    return 1L;
}

G2RESULT g2admin::on_login_completed(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2admin_login_completed(handle));
    return 1L;
}

G2RESULT g2admin::on_failover_prepare(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2admin_failover_prepare(handle, (G2DISCONNECT_REASON::TYPE)wparam, (G2SERVICE_LOGIN_FAIL_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2admin::on_failover_connected(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2admin_failover_connected(handle));
    return 1L;
}

G2RESULT g2admin::on_failover_failed(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
#if defined(_WIN32)
#pragma warning(push)
#pragma warning(disable : 4800)
#endif
    CALL_LISTENER(on_g2admin_failover_failed(handle, wparam));
    return 1L;
#if defined(_WIN32)
#pragma warning(pop)
#endif
}

G2RESULT g2admin::on_notify_connectable_service(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    G2NETWORK_INFO* ni = (G2NETWORK_INFO*)(lparam);
    CALL_LISTENER(on_g2admin_notify_connectable_service(handle, *guid, *ni));
    return 1L;
}

G2RESULT g2admin::on_receive_device_empty(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2admin_receive_empty_device(handle));
    return 1L;
}

G2RESULT g2admin::on_receive_device_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GROUP_MEMBER* data = (G2GROUP_MEMBER*)(wparam);
    size_t size = lparam;
    std::vector<G2GROUP_MEMBER> bunch(data, data + size);
    CALL_LISTENER(on_g2admin_receive_device_append(handle, bunch));
    return 1L;
}

G2RESULT g2admin::on_receive_device_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    bool reconnect = (bool)(lparam);
    assert(guid != NULL);
    CALL_LISTENER(on_g2admin_receive_device_modify(handle, *guid, reconnect));
    return 1L;
}

G2RESULT g2admin::on_receive_device_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    assert(guid != NULL);
    CALL_LISTENER(on_g2admin_receive_device_remove(handle, *guid));
    return 1L;
}

G2RESULT g2admin::on_receive_device_remove_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* data = (G2GUID*)(wparam);
    size_t size = lparam;
    std::vector<G2GUID> bunch(data, data + size);
    CALL_LISTENER(on_g2admin_receive_device_remove_list(handle, bunch));
    return 1L;
}

G2RESULT g2admin::on_receive_device_group_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2DEVICE_GROUP* data = (G2DEVICE_GROUP*)(wparam);
    size_t size = lparam;
    std::vector<G2DEVICE_GROUP> bunch(data, data + size);
    CALL_LISTENER(on_g2admin_receive_device_group_list(handle, bunch));
    return 1L;
}

G2RESULT g2admin::on_receive_device_group_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2DEVICE_GROUP* group = (G2DEVICE_GROUP*)(wparam);
    assert(group != NULL);
    CALL_LISTENER(on_g2admin_receive_device_group_append(handle, *group));
    return 1L;
}

G2RESULT g2admin::on_receive_device_group_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2DEVICE_GROUP* group = (G2DEVICE_GROUP*)(wparam);
    assert(group != NULL);
    CALL_LISTENER(on_g2admin_receive_device_group_modify(handle, *group));
    return 1L;
}

G2RESULT g2admin::on_receive_device_group_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    assert(guid != NULL);
    CALL_LISTENER(on_g2admin_receive_device_group_remove(handle, *guid));
    return 1L;
}

G2RESULT g2admin::on_receive_device_to_group_map(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GROUP_MEMBER* data = (G2GROUP_MEMBER*)(wparam);
    size_t size = lparam;
    std::vector<G2GROUP_MEMBER> bunch(data, data + size);
    CALL_LISTENER(on_g2admin_receive_device_to_group_map(handle, bunch));
    return 1L;
}

G2RESULT g2admin::on_receive_device_append_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* device = (G2GUID*)(wparam);
    G2GUID* group = (G2GUID*)(lparam);
    assert(device != NULL && group != NULL);
    CALL_LISTENER(on_g2admin_receive_device_append_to_group(handle, *device, *group));
    return 1L;
}

G2RESULT g2admin::on_receive_device_remove_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* device = (G2GUID*)(wparam);
    G2GUID* group = (G2GUID*)(lparam);
    assert(device != NULL && group != NULL);
    CALL_LISTENER(on_g2admin_receive_device_remove_to_group(handle, *device, *group));
    return 1L;
}

G2RESULT g2admin::on_receive_device_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* data = (G2GUID*)(wparam);
    size_t size = lparam;
    std::vector<G2GUID> bunch(data, data + size);
    CALL_LISTENER(on_g2admin_receive_device_list(handle, bunch));
    return 1L;
}

G2RESULT g2admin::on_receive_device_list_append_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GROUP_MEMBER* data = (G2GROUP_MEMBER*)(wparam);
    size_t size = lparam;
    std::vector<G2GROUP_MEMBER> bunch(data, data + size);
    CALL_LISTENER(on_g2admin_receive_device_list_append_to_group(handle, bunch));
    return 1L;
}

G2RESULT g2admin::on_receive_device_list_remove_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GROUP_MEMBER* data = (G2GROUP_MEMBER*)(wparam);
    size_t size = lparam;
    std::vector<G2GROUP_MEMBER> bunch(data, data + size);
    CALL_LISTENER(on_g2admin_receive_device_list_remove_to_group(handle, bunch));
    return 1L;
}

G2RESULT g2admin::on_receive_layout_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* data = (G2GUID*)(wparam);
    size_t size = lparam;
    std::vector<G2GUID> bunch(data, data + size);
    CALL_LISTENER(on_g2admin_receive_layout_list(handle, bunch));
    return 1L;
}

G2RESULT g2admin::on_receive_layout_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    assert(guid != NULL);
    CALL_LISTENER(on_g2admin_receive_layout_append(handle, *guid));
    return 1L;
}

G2RESULT g2admin::on_receive_layout_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    assert(guid != NULL);
    CALL_LISTENER(on_g2admin_receive_layout_modify(handle, *guid));
    return 1L;
}

G2RESULT g2admin::on_receive_layout_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    assert(guid != NULL);
    CALL_LISTENER(on_g2admin_receive_layout_remove(handle, *guid));
    return 1L;
}

G2RESULT g2admin::on_receive_sequence_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* data = (G2GUID*)(wparam);
    size_t size = lparam;
    std::vector<G2GUID> bunch(data, data + size);
    CALL_LISTENER(on_g2admin_receive_sequence_list(handle, bunch));
    return 1L;
}

G2RESULT g2admin::on_receive_sequence_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    assert(guid != NULL);
    CALL_LISTENER(on_g2admin_receive_sequence_append(handle, *guid));
    return 1L;
}

G2RESULT g2admin::on_receive_sequence_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    assert(guid != NULL);
    CALL_LISTENER(on_g2admin_receive_sequence_modify(handle, *guid));
    return 1L;
}

G2RESULT g2admin::on_receive_sequence_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* guid = (G2GUID*)(wparam);
    assert(guid != NULL);
    CALL_LISTENER(on_g2admin_receive_sequence_remove(handle, *guid));
    return 1L;
}

G2RESULT g2admin::on_receive_recording_device_status(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* service = (G2GUID*)(wparam);
    G2RECORDING_DEVICE_STATUS* status = (G2RECORDING_DEVICE_STATUS*)(lparam);
    CALL_LISTENER(on_g2admin_receive_recording_device_status(handle, *service, *status));
    return 1L;
}

G2RESULT g2admin::on_receive_service_log_load(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SYSTEM_LOG* data = (G2SYSTEM_LOG*)(lparam);
    CALL_LISTENER(on_g2admin_receive_service_log_load(handle, *data));
    return 1L;
}

G2RESULT g2admin::on_receive_service_log_load_end(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2admin_receive_service_log_load_end(handle));
    return 1L;
}

G2RESULT g2admin::on_receive_service_log_load_fail(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2admin_receive_service_log_load_fail(handle));
    return 1L;
}

G2RESULT g2admin::on_receive_service_log_load_stop(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2admin_receive_service_log_load_stop(handle));
    return 1L;
}

G2RESULT g2admin::on_modify_device_enable(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* device = (G2GUID*)(wparam);
    bool enable = (bool)(lparam);
    CALL_LISTENER(on_g2admin_modify_device_enable(handle, *device, enable));
    return 1L;
}

G2RESULT g2admin::on_receive_device_modify_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
	G2GUID* data = (G2GUID*)(wparam);
    size_t size = lparam;
    std::vector<G2GUID> bunch(data, data + size);
	CALL_LISTENER(on_g2admin_receive_device_modify_list(handle, bunch));
	return 1L;
}