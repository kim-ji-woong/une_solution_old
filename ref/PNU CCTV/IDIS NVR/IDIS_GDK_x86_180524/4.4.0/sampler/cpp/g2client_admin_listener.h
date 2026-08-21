// g2client_admin_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_ADMIN_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_ADMIN_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_admin.h>
#include <include/g2_define_fen.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2admin_listener
{
public:
    virtual void on_g2admin_connected(G2HADMIN handle) = 0;
    virtual void on_g2admin_disconnected(G2HADMIN handle, G2DISCONNECT_REASON::TYPE reason, G2SERVICE_LOGIN_FAIL_REASON::TYPE reason_user) = 0;
    virtual void on_g2admin_login_failed_from_dvrns(G2HADMIN handle, const G2NETWORK_INFO& ni, G2FEN_RESULT::TYPE error) = 0;
    virtual void on_g2admin_login_completed(G2HADMIN handle) = 0;
    virtual void on_g2admin_failover_prepare(G2HADMIN handle, G2DISCONNECT_REASON::TYPE reason, G2SERVICE_LOGIN_FAIL_REASON::TYPE reason_user) = 0;
    virtual void on_g2admin_failover_connected(G2HADMIN handle) = 0;
    virtual void on_g2admin_failover_failed(G2HADMIN handle, bool canceled) = 0;
    virtual void on_g2admin_notify_connectable_service(G2HADMIN handle, const G2GUID& service, const G2NETWORK_INFO& ni) = 0;
    virtual void on_g2admin_receive_empty_device(G2HADMIN handle) = 0;
    virtual void on_g2admin_receive_device_append(G2HADMIN handle, const std::vector<G2GROUP_MEMBER>& bunch) = 0;
    virtual void on_g2admin_receive_device_modify(G2HADMIN handle, const G2GUID& guid, bool reconnect) = 0;
    virtual void on_g2admin_receive_device_remove(G2HADMIN handle, const G2GUID& guid) = 0;
    virtual void on_g2admin_receive_device_remove_list(G2HADMIN handle, const std::vector<G2GUID>& bunch) = 0;
    virtual void on_g2admin_receive_device_group_list(G2HADMIN handle, const std::vector<G2DEVICE_GROUP>& bunch) = 0;
    virtual void on_g2admin_receive_device_group_append(G2HADMIN handle, const G2DEVICE_GROUP& group) = 0;
    virtual void on_g2admin_receive_device_group_modify(G2HADMIN handle, const G2DEVICE_GROUP& group) = 0;
    virtual void on_g2admin_receive_device_group_remove(G2HADMIN handle, const G2GUID& group) = 0;
    virtual void on_g2admin_receive_device_to_group_map(G2HADMIN handle, const std::vector<G2GROUP_MEMBER>& bunch) = 0;
    virtual void on_g2admin_receive_device_append_to_group(G2HADMIN handle, const G2GUID& device, const G2GUID& group) = 0;
    virtual void on_g2admin_receive_device_remove_to_group(G2HADMIN handle, const G2GUID& device, const G2GUID& group) = 0;
    virtual void on_g2admin_receive_device_list(G2HADMIN handle, const std::vector<G2GUID>& bunch) = 0;
    virtual void on_g2admin_receive_device_list_append_to_group(G2HADMIN handle, const std::vector<G2GROUP_MEMBER>& bunch) = 0;
    virtual void on_g2admin_receive_device_list_remove_to_group(G2HADMIN handle, const std::vector<G2GROUP_MEMBER>& bunch) = 0;
    virtual void on_g2admin_receive_layout_list(G2HADMIN handle, const std::vector<G2GUID>& bunch) = 0;
    virtual void on_g2admin_receive_layout_append(G2HADMIN handle, const G2GUID& layout) = 0;
    virtual void on_g2admin_receive_layout_modify(G2HADMIN handle, const G2GUID& layout) = 0;
    virtual void on_g2admin_receive_layout_remove(G2HADMIN handle, const G2GUID& layout) = 0;
    virtual void on_g2admin_receive_sequence_list(G2HADMIN handle, const std::vector<G2GUID>& bunch) = 0;
    virtual void on_g2admin_receive_sequence_append(G2HADMIN handle, const G2GUID& sequence) = 0;
    virtual void on_g2admin_receive_sequence_modify(G2HADMIN handle, const G2GUID& sequence) = 0;
    virtual void on_g2admin_receive_sequence_remove(G2HADMIN handle, const G2GUID& sequence) = 0;
    virtual void on_g2admin_receive_recording_device_status(G2HADMIN handle, const G2GUID& service, const G2RECORDING_DEVICE_STATUS& status) = 0;
    virtual void on_g2admin_receive_service_log_load(G2HADMIN handle, const G2SYSTEM_LOG& log) = 0;
    virtual void on_g2admin_receive_service_log_load_end(G2HADMIN handle) = 0;
    virtual void on_g2admin_receive_service_log_load_fail(G2HADMIN handle) = 0;
    virtual void on_g2admin_receive_service_log_load_stop(G2HADMIN handle) = 0;
    virtual void on_g2admin_modify_device_enable(G2HADMIN handle, const G2GUID& guid, bool enable) = 0;
	virtual void on_g2admin_receive_device_modify_list(G2HADMIN handle, const std::vector<G2GUID>& bunch) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_ADMIN_LISTENER_H_
