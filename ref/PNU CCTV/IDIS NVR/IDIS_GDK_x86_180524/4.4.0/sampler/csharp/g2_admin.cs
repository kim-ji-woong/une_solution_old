using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HADMIN = System.Int32;
#if _WIN64
    using G2WPARAM = System.UInt64;
    using G2LPARAM = System.Int64;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int64;
#else
    using G2WPARAM = System.UInt32;
    using G2LPARAM = System.Int32;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int32;
#endif

    public interface g2admin_listener
    {
        /// <summary>
        /// @ on_g2admin_connected
        /// <para>callback when connected to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_connected(G2HADMIN handle);
        /// <summary>
        /// @ on_g2admin_disconnected
        /// <para>callback when disconnected to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="reason">reason for disconnection</param>
        /// <param name="reason_user">reason for login failed</param>
        void on_g2admin_disconnected(G2HADMIN handle, G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user);
        /// <summary>
        /// @ on_g2admin_login_failed_from_dvrns
        /// <para>callback when login falied from dvrns</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="ni">network information</param>
        /// <param name="error">error code</param>
        void on_g2admin_login_failed_from_dvrns(G2HADMIN handle, ref G2NETWORK_INFO ni, G2FEN_RESULT.TYPE error);
        /// <summary>
        /// @ on_g2admin_login_completed
        /// <para>callback when login to the administration service is completed</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_login_completed(G2HADMIN handle);
        /// <summary>
        /// @ on_g2admin_failover_prepare
        /// <para>callback when connecting to the administration failover service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="reason">reason for disconnection from the administration service</param>
        /// <param name="reason_user">reason for login failed</param>
        void on_g2admin_failover_prepare(G2HADMIN handle, G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user);
        /// <summary>
        /// @ on_g2admin_failover_connected
        /// <para>callback when connected to the administration failover service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_failover_connected(G2HADMIN handle);
        /// <summary>
        /// @ on_g2amdin_failover_failed
        /// <para>callback when it has failed to connect to the administration failover service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="canceled">TRUE for cancellation by user; FALSE for connection failure</param>
        void on_g2admin_failover_failed(G2HADMIN handle, bool canceled);
        /// <summary>
        /// @ on_g2admin_notify_connectable_service
        /// <para>callback notifying information of currently connectable service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="service">guid of service</param>
        /// <param name="ni">network information of service</param>
        void on_g2admin_notify_connectable_service(G2HADMIN handle, ref G2GUID service, ref G2NETWORK_INFO ni);
        /// <summary>
        /// @ on_g2admin_receive_empty_device
        /// <para>callback when the device list received from the administration service is empty</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_receive_empty_device(G2HADMIN handle);
        /// <summary>
        /// @ on_g2admin_receive_device_append
        /// <para>callback when a device is appended to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">device information list</param>
        void on_g2admin_receive_device_append(G2HADMIN handle, G2GROUP_MEMBER[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_device_modify
        /// <para>callback when device information is modified in the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="device">device unique id</param>
        /// <param name="reconnect">If TRUE, disconnect the device and connect it again.</param>
        void on_g2admin_receive_device_modify(G2HADMIN handle, ref G2GUID device, bool reconnect);
        /// <summary>
        /// @ on_g2admin_receive_device_remove
        /// <para>callback when a device is deleted in the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="device">device unique id</param>
        void on_g2admin_receive_device_remove(G2HADMIN handle, ref G2GUID device);
        /// <summary>
        /// @ on_g2admin_receive_device_remove_list
        /// <para>callback when one or more devices are deleted in the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">device guids list</param>
        void on_g2admin_receive_device_remove_list(G2HADMIN handle, G2GUID[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_device_group_list
        /// <para>callback for the device group registered to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">device group list</param>
        void on_g2admin_receive_device_group_list(G2HADMIN handle, G2DEVICE_GROUP[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_device_group_append
        /// <para>callback when a device group is appended to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="group">device group information</param>
        void on_g2admin_receive_device_group_append(G2HADMIN handle, ref G2DEVICE_GROUP group);
        /// <summary>
        /// @ on_g2admin_receive_device_group_modify
        /// <para>callback when a device group information is modified in the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="group">device group information</param>
        void on_g2admin_receive_device_group_modify(G2HADMIN handle, ref G2DEVICE_GROUP group);
        /// <summary>
        /// @ on_g2admin_receive_device_group_remove
        /// <para>callback when a device group information is deleted in the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="group">device group unique id</param>
        void on_g2admin_receive_device_group_remove(G2HADMIN handle, ref G2GUID group);
        /// <summary>
        /// @ on_g2admin_receive_device_to_group_map
        /// <para>callback for the device information registered to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">device information list</param>
        void on_g2admin_receive_device_to_group_map(G2HADMIN handle, G2GROUP_MEMBER[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_device_append_to_group
        /// <para>callback when a device is appended to a device group</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="device">device unique id</param>
        /// <param name="group">device group unique id</param>
        void on_g2admin_receive_device_append_to_group(G2HADMIN handle, ref G2GUID device, ref G2GUID group);
        /// <summary>
        /// @ on_g2admin_receive_device_remove_to_group
        /// <para>callback when a device is deleted from a device group</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="device">device unique id</param>
        /// <param name="group">device group unique id</param>
        void on_g2admin_receive_device_remove_to_group(G2HADMIN handle, ref G2GUID device, ref G2GUID group);
        /// <summary>
        /// @ on_g2admin_receive_device_list
        /// <para>callback for the device list registered to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">device unique id list</param>
        void on_g2admin_receive_device_list(G2HADMIN handle, G2GUID[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_device_list_append_to_group
        /// <para>callback when one or more devices are appended to a group</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">device group information list</param>
        void on_g2admin_receive_device_list_append_to_group(G2HADMIN handle, G2GROUP_MEMBER[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_device_list_remove_to_group
        /// <para>callback when one or more devices are deleted from a device group</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">device group information list</param>
        void on_g2admin_receive_device_list_remove_to_group(G2HADMIN handle, G2GROUP_MEMBER[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_layout_list
        /// <para>callback for the layout list registered to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">layout unique id list</param>
        void on_g2admin_receive_layout_list(G2HADMIN handle, G2GUID[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_layout_append
        /// <para>callback when a layout is appended to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="layout">layout unique id</param>
        void on_g2admin_receive_layout_append(G2HADMIN handle, ref G2GUID layout);
        /// <summary>
        /// @ on_g2admin_receive_layout_modify
        /// <para>callback when a layout is modified int the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="layout">layout unique id</param>
        void on_g2admin_receive_layout_modify(G2HADMIN handle, ref G2GUID layout);
        /// <summary>
        /// @ on_g2admin_receive_layout_remove
        /// <para>callback when a layout is deleted int the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="layout">layout unique id</param>
        void on_g2admin_receive_layout_remove(G2HADMIN handle, ref G2GUID layout);
        /// <summary>
        /// @ on_g2admin_receive_sequence_list
        /// <para>callback for the sequence monitoring list registered to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">sequence monitoring unique id list</param>
        void on_g2admin_receive_sequence_list(G2HADMIN handle, G2GUID[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_sequence_append
        /// <para>callback when a sequence monitoring is appended to the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="sequence">sequence monitoring unique id</param>
        void on_g2admin_receive_sequence_append(G2HADMIN handle, ref G2GUID sequence);
        /// <summary>
        /// @ on_g2admin_receive_sequence_modify
        /// <para>callback when a sequence monitoring is modified int the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="sequence">sequence monitoring unique id</param>
        void on_g2admin_receive_sequence_modify(G2HADMIN handle, ref G2GUID sequence);
        /// <summary>
        /// @ on_g2admin_receive_sequence_remove
        /// <para>callback when a sequence monitoring is deleted int the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="sequence">sequence monitoring unique id</param>
        void on_g2admin_receive_sequence_remove(G2HADMIN handle, ref G2GUID sequence);
        /// <summary>
        /// @ on_g2admin_receive_recording_device_status
        /// <para>callback for device recording status information</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="service">recording service unique id</param>
        /// <param name="status">device recording status information</param>
        void on_g2admin_receive_recording_device_status(G2HADMIN handle, ref G2GUID service, ref G2RECORDING_DEVICE_STATUS status);
        /// <summary>
        /// @ on_g2admin_receive_service_log_load
        /// <para>callback for the search for the administration service system log</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="log">system log information</param>
        void on_g2admin_receive_service_log_load(G2HADMIN handle, ref G2SYSTEM_LOG log);
        /// <summary>
        /// @ on_g2admin_receive_service_log_load_end
        /// <para>callback for the end of the search for the administration service system log</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_receive_service_log_load_end(G2HADMIN handle);
        /// <summary>
        /// @ on_g2admin_receive_service_log_load_fail
        /// <para>callback for the failure of the search for the administration service system log</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_receive_service_log_load_fail(G2HADMIN handle);
        /// <summary>
        /// @ on_g2admin_receive_service_log_load_stop
        /// <para>callback for the stop of the search for the administration service system log</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_receive_service_log_load_stop(G2HADMIN handle);
        /// <summary>
        /// @ on_g2admin_receive_debug_log_load
        /// <para>callback for the search for the administration service debug log</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="log">debug log information</param>
        void on_g2admin_receive_debug_log_load(G2HADMIN handle, ref G2DEBUG_LOG log);
        /// <summary>
        /// @ on_g2admin_receive_debug_log_load_end
        /// <para>callback for the end of the search for the administration service debug log</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_receive_debug_log_load_end(G2HADMIN handle);
        /// <summary>
        /// @ on_g2admin_receive_debug_log_load_fail
        /// <para>callback for the failure of the search for the administration service debug log</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_receive_debug_log_load_fail(G2HADMIN handle);
        /// <summary>
        /// @ on_g2admin_receive_debug_log_load_stop
        /// <para>callback for the stop of the search for the administration service debug log</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        void on_g2admin_receive_debug_log_load_stop(G2HADMIN handle);
        /// <summary>
        /// @ on_g2admin_modify_device_enable
        /// <para>callback when device enable is changed</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="device">device unique id</param>
        /// <param name="enable">device enable state</param>
        void on_g2admin_modify_device_enable(G2HADMIN handle, ref G2GUID device, bool enable); 
        /// <summary>
        /// @ on_g2admin_receive_test_failover_end
        /// <para>callback when test failover finished</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="timeout">timeout</param>
        void on_g2admin_receive_test_failover_end(G2HADMIN handle, bool timeout);
        /// <summary>
        /// @ on_g2admin_receive_device_modify_list
        /// <para>callback when one or more devices are modified in the administration service</para>
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="bunch">device unique id list</param>
        void on_g2admin_receive_device_modify_list(G2HADMIN handle, G2GUID[] bunch);
        /// <summary>
        /// @ on_g2admin_receive_stream_device_in_charge
        /// </summary>
        /// <param name="handle">g2_admin control handle</param>
        /// <param name="serviceGUID">service guid</param>
        /// <param name="append">append devices on streaming service</param>
        /// <param name="remove">remove devices on streaming service</param>
        void on_g2admin_receive_stream_device_in_charge(G2HADMIN handle, G2GUID serviceGUID, G2GUID[] appends, G2GUID[] removes);

    }

    public class g2admin
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_admin_register_callback(G2HADMIN handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HADMIN g2_admin_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_admin_finalize(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_admin_startup(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_admin_cleanup(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_connect(G2HADMIN handle, ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_connect_failover(G2HADMIN handle, ref G2NETWORK_INFO ni, ref G2NETWORK_INFO fni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_connect_on_active_directory(G2HADMIN handle, ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_admin_disconnect(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_connecting(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_connected(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_disconnecting(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_disconnected(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_disconnectable(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_admin_set_disconnect_by_me(G2HADMIN handle, [MarshalAs(UnmanagedType.U1)] bool me);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_request_alive_check(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_request_test_failover(G2HADMIN handle, ref G2GUID service, ref G2SERVICE_NETWORK_INFO sni, ref G2NETWORK_INFO ni);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_request_recording_device_status(G2HADMIN handle, ref G2GUID root, [MarshalAs(UnmanagedType.U1)] bool invoke);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_request_recording_device_status_v2(G2HADMIN handle, G2GUID[] roots, uint count, [MarshalAs(UnmanagedType.U1)] bool invoke);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_request_recording_instant(G2HADMIN handle, ref G2GUID camera, [MarshalAs(UnmanagedType.U1)] bool enable);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_request_system_log_search(G2HADMIN handle, ref G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_request_system_log_search_stop(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_request_debug_log_search(G2HADMIN handle, ref G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_request_debug_log_search_stop(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2GUID g2_admin_get_root_guid_from_leaf_guid(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2GUID g2_admin_get_parent_guid_from_leaf_guid(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        protected static extern G2GUID g2_admin_get_parent_guid_from_address(G2HADMIN handle, string address);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        protected static extern G2GUID g2_admin_get_parent_guid_from_dvrns_name(G2HADMIN handle, string name);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2GUID g2_admin_get_parent_guid_from_mac(G2HADMIN handle, ref G2MAC_ADDRESS mac);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_root_device_info(G2HADMIN handle, ref G2GUID guid, out G2DEVICE_ROOT device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_parent_device_info(G2HADMIN handle, ref G2GUID guid, out G2DEVICE_ROOT device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_leaf_device_info(G2HADMIN handle, ref G2GUID guid, out G2DEVICE_LEAF device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_device_product_info(G2HADMIN handle, ref G2GUID device, out G2_PRODUCT_INFO pi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_device_group_info(G2HADMIN handle, ref G2GUID guid, out G2DEVICE_GROUP group);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_layout_info(G2HADMIN handle, ref G2GUID guid, out G2LAYOUT layout);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_sequence_info(G2HADMIN handle, ref G2GUID guid, out G2SEQUENCE sequence);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_ref_text_in_network_from_camera(G2HADMIN handle, ref G2GUID camera, out G2GUID_8 textIn);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_admin_get_service_version(G2HADMIN handle, out G2SERVICE_VERSION version);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_service_info(G2HADMIN handle, ref G2GUID guid, out G2SERVICE service);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_numeric_id_from_device(G2HADMIN handle, ref G2GUID device, out G2DEVICE_NUMERIC_ID id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2GUID g2_admin_get_recording_service_guid_from_device(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2GUID g2_admin_get_streaming_service_guid_from_device(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2GUID g2_admin_get_backup_service_guid_from_device(G2HADMIN handle, ref G2GUID device, ref G2GUID backup, ref G2GUID site);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_camera_guid_from_recording_service(G2HADMIN handle, ref G2GUID service, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_camera_guid_from_backup_site(G2HADMIN handle, ref G2GUID service, ref G2GUID site, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_backup_service_guid_from_device(G2HADMIN handle, ref G2GUID device, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_backup_service_guid_from_device_site(G2HADMIN handle, ref G2GUID device, ref G2GUID site, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_backup_service_site_guid_from_device(G2HADMIN handle, ref G2GUID device, [MarshalAs(UnmanagedType.U1)] bool sort, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_SERVICE_ITEM proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_violet_service_guid(G2HADMIN handle, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_service_guid(G2HADMIN handle, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_root_guid(G2HADMIN handle, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_parent_guid(G2HADMIN handle, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_leaf_guid_from_root_guid(G2HADMIN handle, ref G2GUID device, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_leaf_guid_from_root_guid_reposit(G2HADMIN handle, ref G2GUID device, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_camera_guid_from_root_guid(G2HADMIN handle, ref G2GUID device, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_layout_guid(G2HADMIN handle, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_enum_group_guid_from_device_guid(G2HADMIN handle, ref G2GUID device, [MarshalAs(UnmanagedType.FunctionPtr)] G2PROC_ENUM_GUID proc, G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_service_network_info(G2HADMIN handle, ref G2GUID service, out G2NETWORK_INFO ni);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_get_device_network_info(G2HADMIN handle, ref G2GUID device, out G2NETWORK_INFO ni);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_admin_get_channelext_from_device_guid(G2HADMIN handle, ref G2GUID service, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2GUID g2_admin_get_device_guid_from_channelext(G2HADMIN handle, ref G2GUID service, int channelext);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_disconnect_by_me(G2HADMIN handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_service_contains(G2HADMIN handle, ref G2GUID service);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_service_registered(G2HADMIN handle, ref G2GUID service);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_service_online(G2HADMIN handle, ref G2GUID service);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_service_offline(G2HADMIN handle, ref G2GUID service);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_support_backup_site(G2HADMIN handle, ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_support_local_search(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_support_local_search_g2(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_contains(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_enable(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_access_right(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_contains_group(G2HADMIN handle, ref G2GUID group, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_dvr(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_dvr_pc_based(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_ipcamera(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_onvif(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_password_encrypted(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_need_connect_rtp(G2HADMIN handle, ref G2GUID device);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_registered_request_recording_status(ref G2GUID root);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_activate_service_record(ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_activate_instant_record(ref G2GUID camera, [MarshalAs(UnmanagedType.U1)] bool check_recording);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_contains_instant_record(ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_contains_instant_record_failed(ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_admin_is_device_primary_recording_has_device(ref G2GUID root);
        #endregion

        private g2admin()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_login_failed_from_dvrns = new G2FUN_LISTENER(on_login_failed_from_dvrns);
            this._p2_on_login_completed = new G2FUN_LISTENER(on_login_completed);
            this._p2_on_failover_prepare = new G2FUN_LISTENER(on_failover_prepare);
            this._p2_on_failover_connected = new G2FUN_LISTENER(on_failover_connected);
            this._p2_on_failover_failed = new G2FUN_LISTENER(on_failover_failed);
            this._p2_on_notify_connectable_service = new G2FUN_LISTENER(on_notify_connectable_service);
            this._p2_on_receive_device_empty = new G2FUN_LISTENER(on_receive_device_empty);
            this._p2_on_receive_device_append = new G2FUN_LISTENER(on_receive_device_append);
            this._p2_on_receive_device_modify = new G2FUN_LISTENER(on_receive_device_modify);
            this._p2_on_receive_device_remove = new G2FUN_LISTENER(on_receive_device_remove);
            this._p2_on_receive_device_remove_list = new G2FUN_LISTENER(on_receive_device_remove_list);
            this._p2_on_receive_device_group_list = new G2FUN_LISTENER(on_receive_device_group_list);
            this._p2_on_receive_device_group_append = new G2FUN_LISTENER(on_receive_device_group_append);
            this._p2_on_receive_device_group_modify = new G2FUN_LISTENER(on_receive_device_group_modify);
            this._p2_on_receive_device_group_remove = new G2FUN_LISTENER(on_receive_device_group_remove);
            this._p2_on_receive_device_to_group_map = new G2FUN_LISTENER(on_receive_device_to_group_map);
            this._p2_on_receive_device_append_to_group = new G2FUN_LISTENER(on_receive_device_append_to_group);
            this._p2_on_receive_device_remove_to_group = new G2FUN_LISTENER(on_receive_device_remove_to_group);
            this._p2_on_receive_device_list = new G2FUN_LISTENER(on_receive_device_list);
            this._p2_on_receive_device_list_append_to_group = new G2FUN_LISTENER(on_receive_device_list_append_to_group);
            this._p2_on_receive_device_list_remove_to_group = new G2FUN_LISTENER(on_receive_device_list_remove_to_group);
            this._p2_on_receive_layout_list = new G2FUN_LISTENER(on_receive_layout_list);
            this._p2_on_receive_layout_append = new G2FUN_LISTENER(on_receive_layout_append);
            this._p2_on_receive_layout_modify = new G2FUN_LISTENER(on_receive_layout_modify);
            this._p2_on_receive_layout_remove = new G2FUN_LISTENER(on_receive_layout_remove);
            this._p2_on_receive_sequence_list = new G2FUN_LISTENER(on_receive_sequence_list);
            this._p2_on_receive_sequence_append = new G2FUN_LISTENER(on_receive_sequence_append);
            this._p2_on_receive_sequence_modify = new G2FUN_LISTENER(on_receive_sequence_modify);
            this._p2_on_receive_sequence_remove = new G2FUN_LISTENER(on_receive_sequence_remove);
            this._p2_on_receive_recording_device_status = new G2FUN_LISTENER(on_receive_recording_device_status);
            this._p2_on_receive_service_log_load = new G2FUN_LISTENER(on_receive_service_log_load);
            this._p2_on_receive_service_log_load_end = new G2FUN_LISTENER(on_receive_service_log_load_end);
            this._p2_on_receive_service_log_load_fail = new G2FUN_LISTENER(on_receive_service_log_load_fail);
            this._p2_on_receive_service_log_load_stop = new G2FUN_LISTENER(on_receive_service_log_load_stop);
            this._p2_on_receive_debug_log_load = new G2FUN_LISTENER(on_receive_debug_log_load);
            this._p2_on_receive_debug_log_load_end = new G2FUN_LISTENER(on_receive_debug_log_load_end);
            this._p2_on_receive_debug_log_load_fail = new G2FUN_LISTENER(on_receive_debug_log_load_fail);
            this._p2_on_receive_debug_log_load_stop = new G2FUN_LISTENER(on_receive_debug_log_load_stop);
            this._p2_on_modify_device_enable = new G2FUN_LISTENER(on_modify_device_enable);
            this._p2_on_receive_test_failover_end = new G2FUN_LISTENER(on_receive_test_failover_end);
            this._p2_on_receive_device_modify_list = new G2FUN_LISTENER(on_receive_device_modify_list);
            this._p2_on_receive_stream_device_in_charge = new G2FUN_LISTENER(on_receive_stream_device_in_charge);
        }
        ~g2admin()
        {
            cleanup();
        }

        public static g2admin get()
        {
            return s_singleton;
        }

        public static g2admin s_singleton = new g2admin();
        private G2HADMIN _handle;
        private G2UPARAM _param;
        private g2admin_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2ADMIN_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_admin_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_login_failed_from_dvrns;
        private G2FUN_LISTENER _p2_on_login_completed;
        private G2FUN_LISTENER _p2_on_failover_prepare;
        private G2FUN_LISTENER _p2_on_failover_connected;
        private G2FUN_LISTENER _p2_on_failover_failed;
        private G2FUN_LISTENER _p2_on_notify_connectable_service;
        private G2FUN_LISTENER _p2_on_receive_device_empty;
        private G2FUN_LISTENER _p2_on_receive_device_append;
        private G2FUN_LISTENER _p2_on_receive_device_modify;
        private G2FUN_LISTENER _p2_on_receive_device_remove;
        private G2FUN_LISTENER _p2_on_receive_device_remove_list;
        private G2FUN_LISTENER _p2_on_receive_device_group_list;
        private G2FUN_LISTENER _p2_on_receive_device_group_append;
        private G2FUN_LISTENER _p2_on_receive_device_group_modify;
        private G2FUN_LISTENER _p2_on_receive_device_group_remove;
        private G2FUN_LISTENER _p2_on_receive_device_to_group_map;
        private G2FUN_LISTENER _p2_on_receive_device_append_to_group;
        private G2FUN_LISTENER _p2_on_receive_device_remove_to_group;
        private G2FUN_LISTENER _p2_on_receive_device_list;
        private G2FUN_LISTENER _p2_on_receive_device_list_append_to_group;
        private G2FUN_LISTENER _p2_on_receive_device_list_remove_to_group;
        private G2FUN_LISTENER _p2_on_receive_layout_list;
        private G2FUN_LISTENER _p2_on_receive_layout_append;
        private G2FUN_LISTENER _p2_on_receive_layout_modify;
        private G2FUN_LISTENER _p2_on_receive_layout_remove;
        private G2FUN_LISTENER _p2_on_receive_sequence_list;
        private G2FUN_LISTENER _p2_on_receive_sequence_append;
        private G2FUN_LISTENER _p2_on_receive_sequence_modify;
        private G2FUN_LISTENER _p2_on_receive_sequence_remove;
        private G2FUN_LISTENER _p2_on_receive_recording_device_status;
        private G2FUN_LISTENER _p2_on_receive_service_log_load;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_stop;
        private G2FUN_LISTENER _p2_on_modify_device_enable;
        private G2FUN_LISTENER _p2_on_receive_test_failover_end;
        private G2FUN_LISTENER _p2_on_receive_device_modify_list;
        private G2FUN_LISTENER _p2_on_receive_stream_device_in_charge;
        #endregion

        public G2HADMIN safe_handle() { return _handle; }

        /// <summary>
        /// @ startup
        /// <para>This method creates g2admin object, and registers g2admin callback functions. </para>
        /// </summary>
        public void startup()
        {
            cleanup();

            _handle = g2_admin_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2ADMIN_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_login_failed_from_dvrns, _p2_on_login_failed_from_dvrns);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_login_completed, _p2_on_login_completed);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_failover_prepare, _p2_on_failover_prepare);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_failover_connected, _p2_on_failover_connected);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_failover_failed, _p2_on_failover_failed);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_notify_connectable_service, _p2_on_notify_connectable_service);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_empty, _p2_on_receive_device_empty);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_append, _p2_on_receive_device_append);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_modify, _p2_on_receive_device_modify);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_remove, _p2_on_receive_device_remove);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_remove_list, _p2_on_receive_device_remove_list);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_group_list, _p2_on_receive_device_group_list);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_group_append, _p2_on_receive_device_group_append);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_group_modify, _p2_on_receive_device_group_modify);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_group_remove, _p2_on_receive_device_group_remove);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_to_group_map, _p2_on_receive_device_to_group_map);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_append_to_group, _p2_on_receive_device_append_to_group);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_remove_to_group, _p2_on_receive_device_remove_to_group);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_list, _p2_on_receive_device_list);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_list_append_to_group, _p2_on_receive_device_list_append_to_group);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_list_remove_to_group, _p2_on_receive_device_list_remove_to_group);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_layout_list, _p2_on_receive_layout_list);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_layout_append, _p2_on_receive_layout_append);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_layout_modify, _p2_on_receive_layout_modify);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_layout_remove, _p2_on_receive_layout_remove);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_sequence_list, _p2_on_receive_sequence_list);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_sequence_append, _p2_on_receive_sequence_append);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_sequence_modify, _p2_on_receive_sequence_modify);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_sequence_remove, _p2_on_receive_sequence_remove);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_recording_device_status, _p2_on_receive_recording_device_status);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_service_log_load, _p2_on_receive_service_log_load);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_service_log_load_end, _p2_on_receive_service_log_load_end);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_service_log_load_fail, _p2_on_receive_service_log_load_fail);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_service_log_load_stop, _p2_on_receive_service_log_load_stop);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_debug_log_load, _p2_on_receive_debug_log_load);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_debug_log_load_end, _p2_on_receive_debug_log_load_end);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_debug_log_load_fail, _p2_on_receive_debug_log_load_fail);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_debug_log_load_stop, _p2_on_receive_debug_log_load_stop);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_modify_device_enable, _p2_on_modify_device_enable);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_test_failover_end, _p2_on_receive_test_failover_end);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_device_modify_list, _p2_on_receive_device_modify_list);
            register_callback(G2ADMIN_CALLBACK.TYPE.on_receive_stream_device_in_charge, _p2_on_receive_stream_device_in_charge);
            #endregion

            g2_admin_startup(_handle);
        }

        /// <summary>
        /// @ cleanup
        /// <para>Deletes memory, and resource allocated through g2admin startup function.</para>
        /// <para>This should be called before the startup function call of g2admin object.</para>
        /// </summary>
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HADMIN handle = _handle; _handle = 0;
                g2_admin_cleanup(handle);
                g2_admin_finalize(handle);
            }
        }

        /// <summary>
        /// @ set_listener
        /// <para>This method registers g2admin_listener function.</para>
        /// </summary>
        /// <param name="listener">listener function</param>
        public void set_listener(g2admin_listener listener)
        {
            _listener = listener;
        }

        /// <summary>
        /// @ connect
        /// <para>This method connects to administration service using network information which is passed as parameter.</para>
        /// </summary>
        /// <param name="ni">The structure wherein the administration service connection information is saved.</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool connect(ref G2NETWORK_INFO ni, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_admin_connect(_handle, ref ni, ref options, out res);
        }

        /// <summary>
        /// @ connect
        /// <para>This method connects to administration service using network information which is passed as parameter.</para>
        /// </summary>
        /// <param name="ni">The structure wherein the administration service connection information is saved.</param>
        /// <param name="options">The structure wherein the options for service connection is saved. It has information about connection time out.</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id inforamation after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool connect(ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_admin_connect(_handle, ref ni, ref options, out res);
        }

        /// <summary>
        /// @ connect_failover
        /// <para>This method connects to administration failover service using network information which is passed as parameter.</para>
        /// </summary>
        /// <param name="ni">The structure wherein the administration service connection information is saved.</param>
        /// <param name="fni">The structure wherein the administration failover service connection information is saved.</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id inforamation after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool connect_failover(ref G2NETWORK_INFO ni, ref G2NETWORK_INFO fni, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_admin_connect_failover(_handle, ref ni, ref fni, ref options, out res);
        }

        /// <summary>
        /// @ connect_failover
        /// <para>This method connects to administration failover service using network information which is passed as parameter.</para>
        /// </summary>
        /// <param name="ni">The structure wherein the administration service connection information is saved.</param>
        /// <param name="fni">The structure wherein the administration failover service connection information is saved.</param>
        /// <param name="options">The structure wherein the options for service connection is saved. It has information about connection time out.</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id inforamation after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool connect_failover(ref G2NETWORK_INFO ni, ref G2NETWORK_INFO fni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_admin_connect_failover(_handle, ref ni, ref fni, ref options, out res);
        }

        /// <summary>
        /// @ connect_on_active_directory
        /// <para>This method connects to administration service. It use the account information associated with Active Directory.</para>
        /// </summary>
        /// <param name="ni">The structure wherein the administration service connection information is saved.</param>
        /// <param name="res">In this out parameter, connection result information will be stored. 
        /// This parameter will contain error type id, and session id inforamation after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool connect_on_active_directory(ref G2NETWORK_INFO ni, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_admin_connect_on_active_directory(_handle, ref ni, ref options, out res);
        }
        
        /// <summary>
        /// @ connect_on_active_directory
        /// <para>This method connects to administration service. It use the account information associated with Active Directory.</para>
        /// </summary>
        /// <param name="ni">The structure wherein the administration service connection information is saved.</param>
        /// <param name="options">The structure wherein the options for service connection is saved. It has information about connection time out.</param>
        /// <param name="res">In this out parameter, connection result information will be stored. 
        /// This parameter will contain error type id, and session id inforamation after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool connect_on_active_directory(ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_admin_connect_on_active_directory(_handle, ref ni, ref options, out res);
        }

        /// <summary>
        /// @ disconnect
        /// <para>This method disconnects from administration service.</para>
        /// </summary>
        public void disconnect()
        {
            g2_admin_disconnect(_handle);
        }

        /// <summary>
        /// @ is_connecting
        /// <para>It checks whether it is connecting to the administration service.</para>
        /// </summary>
        /// <returns>If in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connecting()
        {
            return g2_admin_is_connecting(_handle);
        }

        /// <summary>
        /// @ is_connected
        /// <para>It checks whether it is connected to the administration service.</para>
        /// </summary>
        /// <returns>If connected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connected()
        {
            return g2_admin_is_connected(_handle);
        }

        /// <summary>
        /// @ is_disconnecting
        /// <para>It checks whether it is disconnecting from the administration service.</para>
        /// </summary>
        /// <returns>If in the process of disconnecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnecting()
        {
            return g2_admin_is_disconnecting(_handle);
        }

        /// <summary>
        /// @ is_disconnected
        /// <para>It checks whether it is disconnected from the administration service.</para>
        /// </summary>
        /// <returns>If disconnected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnected()
        {
            return g2_admin_is_disconnected(_handle);
        }

        /// <summary>
        /// @ is_disconnectable
        /// <para>It checks whether it is connected or in the process of connecting to the administration service.</para>
        /// </summary>
        /// <returns>If connected or in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnectable()
        {
            return g2_admin_is_disconnectable(_handle);
        }

        /// <summary>
        /// @ set_disconnect_by_me
        /// <para>It sets a disconnection by user. (When disconnected from the administration service, it can be determined whether of not it was a disconnection by the user.</para>
        /// </summary>
        /// <param name="me">For a disconnection intended by the user, set to TRUE; otherwise, set to FALSE.</param>
        public void set_disconnect_by_me(bool me)
        {
            g2_admin_set_disconnect_by_me(_handle, me);
        }

        /// <summary>
        /// @ request_alive_check
        /// <para>It sends packets periodically to maintain the connection to the administration service.</para>
        /// </summary>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_alive_check()
        {
            return g2_admin_request_alive_check(_handle);
        }

        /// <summary>
        /// @ request_test_failover
        /// <para>This method requests to receive failover connection information</para>
        /// </summary>
        /// <param name="service">service GUID, a unique device id</param>
        /// <param name="sni">service network info.</param>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>        
        public bool request_test_failover(ref G2GUID service, ref G2SERVICE_NETWORK_INFO sni, ref G2NETWORK_INFO ni)
        {
            return g2_admin_request_test_failover(_handle, ref service, ref sni, ref ni);
        }

        /// <summary>
        /// @ request_recording_device_status
        /// <para>This method requests to receive root device recording status information</para>
        /// </summary>
        /// <param name="root">root GUID, a unique device id</param>
        /// <param name="invoke">To receive recording status, set to TRUE; otherwise, set to FALSE.</param>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_recording_device_status(ref G2GUID root, bool invoke)
        {
            return g2_admin_request_recording_device_status(_handle, ref root, invoke);
        }

        /// <summary>
        /// @ request_recording_device_status
        /// <para>This method requests to receive recording status information of devices. User may pass multiple GUIDs in a GUID container.</para>
        /// </summary>
        /// <param name="GUIDs">a set of unique device ids</param> 
        /// <param name="invoke">To receive recording status, set to TRUE; otherwise, set to FALSE.</param>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_recording_device_status(G2GUIDSET GUIDs, bool invoke)
        {
            G2GUID[] roots = GUIDs.to_array();
            return g2_admin_request_recording_device_status_v2(_handle, roots, (uint)roots.Length, invoke);
        }

        /// <summary>
        /// @ request_recording_instant
        /// <para>This method requests the instant recording of camera images.</para>
        /// </summary>
        /// <param name="camera">camera device unique id</param>
        /// <param name="enable">Set to TRUE to start instant recording, FALSE to cancel it.</param>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_recording_instant(ref G2GUID camera, bool enable)
        {
            return g2_admin_request_recording_instant(_handle, ref camera, enable);
        }

        /// <summary>
        /// @ request_system_log_search
        /// <para>This method requests the administration service system log.</para>
        /// </summary>
        /// <param name="option">structure wherein log search option information is saved</param>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_system_log_search(G2SERVICE_SEARCH_OPTION_SYSTEM_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_admin_request_system_log_search(_handle, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @ request_system_log_search_stop
        /// <para>This method cancels the request for the administration service system log.</para>
        /// </summary>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_system_log_search_stop()
        {
            return g2_admin_request_system_log_search_stop(_handle);
        }

        /// <summary>
        /// @ request_debug_log_search
        /// <para>This method cancels the request for the administration service debug log.</para>
        /// </summary>
        /// <param name="option">structure wherein debug log search option information is saved</param>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_debug_log_search(G2SERVICE_SEARCH_OPTION_DEBUG_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_admin_request_debug_log_search(_handle, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @ request_debug_log_search_stop
        /// <para>This method cancels the request for the administration service debug log.</para>
        /// </summary>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_debug_log_search_stop()
        {
            return g2_admin_request_debug_log_search_stop(_handle);
        }

        /// <summary>
        /// @ get_root_guid_from_leaf_guid
        /// <para>This method obtains root device unique id from leaf device unique id.</para>
        /// </summary>
        /// <param name="device">leaf unique id</param>
        /// <returns>It returns root device unique id.</returns>
        public G2GUID get_root_guid_from_leaf_guid(ref G2GUID device)
        {
            return g2_admin_get_root_guid_from_leaf_guid(_handle, ref device);
        }

        /// <summary>
        /// @ get_parent_guid_from_leaf_guid
        /// <para>This method obtains parent device unique id from leaf device unique id.</para>
        /// </summary>
        /// <param name="device">leaf device unique id</param>
        /// <returns>It returns parent device unique id.</returns>
        public G2GUID get_parent_guid_from_leaf_guid(ref G2GUID device)
        {
            return g2_admin_get_parent_guid_from_leaf_guid(_handle, ref device);
        }

        /// <summary>
        /// @ get_parent_guid_from_address
        /// <para>This method obtains parent device unique id from device IP address.</para>
        /// </summary>
        /// <param name="address">device IP address</param>
        /// <returns>It returns parent device unique id.</returns>
        public G2GUID get_parent_guid_from_address(string address)
        {
            return g2_admin_get_parent_guid_from_address(_handle, address);
        }

        /// <summary>
        /// @ get_parent_guid_from_dvrns_name
        /// <para>This method obtains parent device unique id from dvrns name.</para>
        /// </summary>
        /// <param name="name"> dvrns name</param>
        /// <returns>It returns parent device unique id.</returns>
        public G2GUID get_parent_guid_from_dvrns_name(string name)
        {
            return g2_admin_get_parent_guid_from_dvrns_name(_handle, name);
        }

        /// <summary>
        /// @ g2_admin_get_parent_guid_from_mac
        /// <para>This method obtains parent device unique id from mac address.</para>
        /// </summary>
        /// <param name="mac">mac address</param>
        /// <returns>It returns parent device unique id.</returns>
        public G2GUID g2_admin_get_parent_guid_from_mac(ref G2MAC_ADDRESS mac)
        {
            return g2_admin_get_parent_guid_from_mac(_handle, ref mac);
        }

        /// <summary>
        /// @ get_root_deivce_info
        /// <para>This method obtains root device information from device unique id.</para>
        /// </summary>
        /// <param name="guid">device unique id</param>
        /// <param name="device">structure to save root device information</param>
        /// <returns>If successful in obtaining root device information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_root_device_info(ref G2GUID guid, out G2DEVICE_ROOT device)
        {
            return g2_admin_get_root_device_info(_handle, ref guid, out device);
        }

        /// <summary>
        /// @ get_parent_device_info
        /// <para>This method obtains parent device information from device unique id.</para>
        /// </summary>
        /// <param name="guid">device unique id</param>
        /// <param name="device">structure to save parent device information</param>
        /// <returns>If successful in obtaining parent device information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_parent_device_info(ref G2GUID guid, out G2DEVICE_ROOT device)
        {
            return g2_admin_get_parent_device_info(_handle, ref guid, out device);
        }

        /// <summary>
        /// @ get_leaf_device_info
        /// <para>This method obtains leaf device information from leaf device unique id.</para>
        /// </summary>
        /// <param name="guid">leaf device unique id</param>
        /// <param name="device">structure to save leaf device information</param>
        /// <returns>If successful in obtaining leaf device information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_leaf_device_info(ref G2GUID guid, out G2DEVICE_LEAF device)
        {
            return g2_admin_get_leaf_device_info(_handle, ref guid, out device);
        }

        /// <summary>
        /// @ get_device_product_info
        /// <para>This method obtains device product information from root device unique id.</para>
        /// </summary>
        /// <param name="device">root device unique id</param>
        /// <param name="pi">product information</param>
        /// <returns>If successful in obtaining device product inforamtion, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_device_product_info(ref G2GUID device, out G2_PRODUCT_INFO pi)
        {
            return g2_admin_get_device_product_info(_handle, ref device, out pi);
        }

        /// <summary>
        /// @ get_device_group_info
        /// <para>This method obtains device group information from device group unique id.</para>
        /// </summary>
        /// <param name="guid">device group unique id</param>
        /// <param name="group">structure to save device group information</param>
        /// <returns>If successful in obtaining device group information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_device_group_info(ref G2GUID guid, out G2DEVICE_GROUP group)
        {
            return g2_admin_get_device_group_info(_handle, ref guid, out group);
        }

        /// <summary>
        /// @ get_layout_info
        /// <para>This method obtains layout information from layout unique id.</para>
        /// </summary>
        /// <param name="guid">layout unique id</param>
        /// <param name="layout">structure to save layout information</param>
        /// <returns>If successful in obtaining layout information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_layout_info(ref G2GUID guid, out G2LAYOUT layout)
        {
            return g2_admin_get_layout_info(_handle, ref guid, out layout);
        }

        /// <summary>
        /// @ get_sequence_info
        /// <para>This method obtains sequence monitoring information from sequence monitoring unique id.</para>
        /// </summary>
        /// <param name="guid">sequence monitoring unique id</param>
        /// <param name="sequence">structure to save sequence monitoring information</param>
        /// <returns>If successful in obtaining sequence monitoring information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_sequence_info(ref G2GUID guid, out G2SEQUENCE sequence)
        {
            return g2_admin_get_sequence_info(_handle, ref guid, out sequence);
        }

        /// <summary>
        /// @ get_ref_text_in_network_from_camera
        /// <para>It obtains reference network text-in unique id list from camera unique id.</para>
        /// </summary>
        /// <param name="camera">camera unique id</param>
        /// <param name="textIn">structure to save reference network text-in list</param>
        /// <returns>If successful in obtaining reference network text-in list, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_ref_text_in_network_from_camera(ref G2GUID camera, out G2GUID_8 textIn)
        {
            return g2_admin_get_ref_text_in_network_from_camera(_handle, ref camera, out textIn);
        }

        /// <summary>
        /// @ get_service_version
        /// <para>This method obtains service version.</para>
        /// </summary>
        /// <param name="versopm">structure to save service version</param>
        public void get_service_version(out G2SERVICE_VERSION version)
        {
            g2_admin_get_service_version(_handle, out version);
        }

        /// <summary>
        /// @ get_service_info
        /// <para>This method obtains service information from service unique id.</para>
        /// </summary>
        /// <param name="guid">service unique id</param>
        /// <param name="service">structure to save service information</param>
        /// <returns>If successful in obtaining service information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_service_info(ref G2GUID guid, out G2SERVICE service)
        {
            return g2_admin_get_service_info(_handle, ref guid, out service);
        }

        /// <summary>
        /// @ get_numeric_id_from_device
        /// <para>This method obtains numeric id from device unique id.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <param name="id">structure to save numeric id information</param>
        /// <returns>If successful in obtaining numeric id information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_numeric_id_from_device(ref G2GUID device, out G2DEVICE_NUMERIC_ID id)
        {
            return g2_admin_get_numeric_id_from_device(_handle, ref device, out id);
        }

        /// <summary>
        /// @ get_recording_service_guid_from_device
        /// <para>This method obtains the corresponding recording service unique id from device unique id.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>It returns recording service unique id.</returns>
        public G2GUID get_recording_service_guid_from_device(ref G2GUID device)
        {
            return g2_admin_get_recording_service_guid_from_device(_handle, ref device);
        }

        /// <summary>
        /// @ get_streaming_service_guid_from_device
        /// <para>This method obtains the corresponding streaming service unique id from device unique id.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>It returns streaming service unique id.</returns>
        public G2GUID get_streaming_service_guid_from_device(ref G2GUID device)
        {
            return g2_admin_get_streaming_service_guid_from_device(_handle, ref device);
        }

        /// <summary>
        /// @ get_backup_service_guid_from_device
        /// <para>This method obtains the corresponding backup service unique id from device unique id.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <param name="backup">unique id(GUID) of the backup service</param>
        /// <param name="site">unique id(GUID) of the site</param>
        /// <returns>It returns backup service unique id.</returns>
        public G2GUID get_backup_service_guid_from_device(ref G2GUID device, ref G2GUID backup, ref G2GUID site)
        {
            return g2_admin_get_backup_service_guid_from_device(_handle, ref device, ref backup, ref site);
        }

        /// <summary>
        /// @ get_camera_guid_from_recording_service
        /// <para>This method obtains the camera unique id list registered to the recording service.</para>
        /// </summary>
        /// <param name="service">recording service unique id</param>
        /// <param name="GUIDs">a list for storing camera guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_camera_guid_from_recording_service(ref G2GUID service, G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_camera_guid_from_recording_service(_handle, ref service, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_camera_guid_from_backup_site
        /// <para>This method obtains the camera unique id list related with backup site of registered to the backup service. </para>
        /// </summary>
        /// <param name="service">backup service unique id</param>
        /// <param name="site">backup site unique id</param>
        /// <param name="GUIDs">a list for storing camera guid</param>
        /// <returns>If a request succeds, it returns TRUE; otherwise, if returns FALSE.</returns>
        public bool get_camera_guid_from_backup_site(ref G2GUID service, ref G2GUID site, G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_camera_guid_from_backup_site(_handle, ref service, ref site, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_backup_service_guid_from_service
        /// <para>This method obtains the backup service unique id list where a device is registered.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <param name="GUIDs">a list for stroing backup service guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_backup_service_guid_from_device(ref G2GUID device, G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_backup_service_guid_from_device(_handle, ref device, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_backup_service_guid_from_device_site
        /// <para>This method obtains the backup service unique id list where a particular site’s device is registered.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <param name="site">site unique id (recording service or DVR root device)</param>
        /// <param name="GUIDs">a list for stroing backup service guids where a particular site's device is registered.</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_backup_service_guid_from_device_site(ref G2GUID device, ref G2GUID site, G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_backup_service_guid_from_device_site(_handle, ref device, ref site, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_backup_service_site_guid_from_device
        /// <para>This method obtains the backup service site unique id list where a device is registered.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <param name="sort">If sorting is nececessary, set to TRUE; otherwise, set to FALSE.</param>
        /// <param name="ITEMs">a list for storing backup service guids where a device is registered.</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_backup_service_site_guid_from_device(ref G2GUID device, bool sort, List<G2SERVICE_ITEM> ITEMs)
        {
            G2PROC_ENUM_SERVICE_ITEM proc = new G2PROC_ENUM_SERVICE_ITEM(G2SERVICE_ITEM.proc_enum_ITEM_List);
            GCHandle gch = GCHandle.Alloc(ITEMs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_backup_service_site_guid_from_device(_handle, ref device, sort, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_violet_service_guid
        /// <para>This method obtains the videowall service site unique id list where a device is registered.</para>
        /// </summary>
        /// <param name="GUIDs">a list for storing videowall service site guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_violet_service_guid(G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_violet_service_guid(_handle, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_service_guid
        /// <para>This method obtains service unique id list.</para>
        /// </summary>
        /// <param name="GUIDs">a list for storing service guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_service_guid(G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_service_guid(_handle, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_root_guid
        /// <para>This method obtains root device unique id list.</para>
        /// </summary>
        /// <param name="GUIDs">a list for storing root deivce guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_root_guid(G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_root_guid(_handle, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_parent_guid
        /// <para>This method obtains parent device unique id list.</para>
        /// </summary>
        /// <param name="GUIDs">a list for storing parent device guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_parent_guid(G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_parent_guid(_handle, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_leaf_guid_from_root_guid
        /// <para>This method obtains leaf device unique id list.</para>
        /// </summary>
        /// <param name="root">root device unique id</param>
        /// <param name="GUIDs">a list for storing leaf device guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_leaf_guid_from_root_guid(ref G2GUID root, G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_leaf_guid_from_root_guid(_handle, ref root, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_leaf_guid_from_root_guid_reposit
        /// <para>This method obtains the unique id list of all leaf devices under the root device.</para>
        /// </summary>
        /// <param name="root">root device unique id</param>
        /// <param name="GUIDs">a list for storing guids of all leaf deivces under the root device</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_leaf_guid_from_root_guid_reposit(ref G2GUID root, G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_leaf_guid_from_root_guid_reposit(_handle, ref root, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_camera_guid_from_root_guid
        /// <para>This method obtains camera unique id list for those under the root device.</para>
        /// </summary>
        /// <param name="root">root device unique id</param>
        /// <param name="GUIDs">a list for storing camera guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_camera_guid_from_root_guid(ref G2GUID root, G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_camera_guid_from_root_guid(_handle, ref root, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_layout_guid
        /// <para>This method obtains layout unique id list</para>
        /// </summary>
        /// <param name="GUIDs">a list for storing layout guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_layout_guid(G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_layout_guid(_handle, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_group_guid_from_device_guid
        /// <para>This method obtains group unique id list where a particular device is included</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <param name="GUIDs">a list for storing group guids</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_group_guid_from_device_guid(ref G2GUID device, G2GUIDLIST GUIDs)
        {
            G2PROC_ENUM_GUID proc = new G2PROC_ENUM_GUID(G2GUID.proc_enum_GUIDLIST);
            GCHandle gch = GCHandle.Alloc(GUIDs);
            IntPtr param = GCHandle.ToIntPtr(gch);
            bool res = g2_admin_enum_group_guid_from_device_guid(_handle, ref device, proc, param);
            gch.Free();
            return res;
        }

        /// <summary>
        /// @ get_service_network_info
        /// <para>This method obtains service connection information.</para>
        /// </summary>
        /// <param name="service">service unique id</param>
        /// <param name="ni">structure to save service connection information</param>
        /// <returns>If successful in obtaining service connection information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_service_network_info(ref G2GUID service, out G2NETWORK_INFO ni)
        {
            return g2_admin_get_service_network_info(_handle, ref service, out ni);
        }

        /// <summary>
        /// @ get_device_network_info
        /// <para>This method obtains device connection information.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <param name="ni">structure to save device connection information</param>
        /// <returns>If successful in obtaining device connection information, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_device_network_info(ref G2GUID device, out G2NETWORK_INFO ni)
        {
            return g2_admin_get_device_network_info(_handle, ref device, out ni);
        }

        /// <summary>
        /// @ get_channelext_from_device_guid
        /// <para>This method obtains device service channel.</para>
        /// </summary>
        /// <param name="service">service unique id</param>
        /// <param name="device">device unique id</param>
        /// <returns>If successful, it returns service channel; otherwise, it returns -1.</returns>
        public int get_channelext_from_device_guid(ref G2GUID service, ref G2GUID device)
        {
            return g2_admin_get_channelext_from_device_guid(_handle, ref service, ref device);
        }

        /// <summary>
        /// @ get_device_guid_from_channelext
        /// <para>This method obtains the unique id of the device that corresponds to the service channel.</para>
        /// </summary>
        /// <param name="service">service unique id</param>
        /// <param name="channelext">device channel</param>
        /// <returns>It returns device unique id.</returns>
        public G2GUID get_device_guid_from_channelext(ref G2GUID service, int channelext)
        {
            return g2_admin_get_device_guid_from_channelext(_handle, ref service, channelext);
        }

        /// <summary>
        /// @ is_disconnect_by_me
        /// <para>This method obtains the configuration to decide whether or not it has been disconnected by user.</para>
        /// </summary>
        /// <returns>If configured as disconnection by the user, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnect_by_me()
        {
            return g2_admin_is_disconnect_by_me(_handle);
        }

        /// <summary>
        /// @ is_service_contains
        /// <para>This method checks whether a service is connected to the administration service.</para>
        /// </summary>
        /// <param name="service">service unique id</param>
        /// <returns>If connected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_contains(ref G2GUID service)
        {
            return g2_admin_is_service_contains(_handle, ref service);
        }

        /// <summary>
        /// @ is_service_registered
        /// <para>This method checks whether a service is registered to the administration service.</para>
        /// </summary>
        /// <param name="service">service unique id</param>
        /// <returns>If registered, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_registered(ref G2GUID service)
        {
            return g2_admin_is_service_registered(_handle, ref service);
        }

        /// <summary>
        /// @ is_service_online
        /// <para>This method checks whether a service is online.</para>
        /// </summary>
        /// <param name="service">service unique id</param>
        /// <returns>If a service is online, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_online(ref G2GUID service)
        {
            return g2_admin_is_service_online(_handle, ref service);
        }

        /// <summary>
        /// @ is_service_offline
        /// <para>This method checks whether a service is offline.</para>
        /// </summary>
        /// <param name="service">service unique id</param>
        /// <returns>If a service is offline, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_offline(ref G2GUID service)
        {
            return g2_admin_is_service_offline(_handle, ref service);
        }

        /// <summary>
        /// @ is_support_backup_site
        /// <para>This method checks whether a site supports the backup feature.</para>
        /// </summary>
        /// <param name="guid">site unique id (recording service or DVR device)</param>
        /// <returns>If supporting the backup feature, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_support_backup_site(ref G2GUID guid)
        {
            return g2_admin_is_support_backup_site(_handle, ref guid);
        }

        /// <summary>
        /// @ is_support_local_search
        /// <para>This method checks whether a device supports local search.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If supporting local search, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_support_local_search(ref G2GUID device)
        {
            return g2_admin_is_support_local_search(_handle, ref device);
        }

        /// <summary>
        /// @ is_support_local_search_g2
        /// <para>This method checks whether a device supports local g2search.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If supporting local g2search, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_support_local_search_g2(ref G2GUID device)
        {
            return g2_admin_is_support_local_search_g2(_handle, ref device);
        }

        /// <summary>
        /// @ is_device_contains
        /// <para>This method checks whether a device is registered to the administration service.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If registered, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_contains(ref G2GUID device)
        {
            return g2_admin_is_device_contains(_handle, ref device);
        }

        /// <summary>
        /// @ is_device_enable
        /// <para>This method checks whether a device is enabled</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If enabled, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_enable(ref G2GUID device)
        {
            return g2_admin_is_device_enable(_handle, ref device);
        }

        /// <summary>
        /// @ is_device_access_right
        /// <para>This method checks whether a device is allocated to the user.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If allocated, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_access_right(ref G2GUID device)
        {
            return g2_admin_is_device_access_right(_handle, ref device);
        }

        /// <summary>
        /// @ is_device_contains_group
        /// <para>This method checks whether a device belongs to a device group.</para>
        /// </summary>
        /// <param name="group">device group unique id</param>
        /// <param name="device">device unique id</param>
        /// <returns>If a device belongs to a group, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_contains_group(ref G2GUID group, ref G2GUID device)
        {
            return g2_admin_is_device_contains_group(_handle, ref group, ref device);
        }

        /// <summary>
        /// @ is_device_dvr
        /// <para>This method checks whether a device is DVR device.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If a device is DVR device, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_dvr(ref G2GUID device)
        {
            return g2_admin_is_device_dvr(_handle, ref device);
        }

        /// <summary>
        /// @ is_device_dvr_pc_based
        /// <para>This method checks whether a device is PC-based DVR device.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If a device is PC-based DVR device, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_dvr_pc_based(ref G2GUID device)
        {
            return g2_admin_is_device_dvr_pc_based(_handle, ref device);
        }

        /// <summary>
        /// @ is_device_ipcamera
        /// <para>This method checks whether a device is network camera device.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If a device is network camera device, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_ipcamera(ref G2GUID device)
        {
            return g2_admin_is_device_ipcamera(_handle, ref device);
        }

        /// <summary>
        /// @ is_device_onvif
        /// <para>This method checks whether a device uses ONVIF protocol.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If using ONVIF protocol, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_onvif(ref G2GUID device)
        {
            return g2_admin_is_device_onvif(_handle, ref device);
        }

        /// <summary>
        /// @ is_device_password_encrypted
        /// <para>This method checks whether a device uses encrypted user password.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If using encrypted password, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_password_encrypted(ref G2GUID device)
        {
            return g2_admin_is_device_password_encrypted(_handle, ref device);
        }

        /// <summary>
        /// @ is_device_need_connect_rtp
        /// <para>This method checks whether a device needs RTP connection.</para>
        /// </summary>
        /// <param name="device">device unique id</param>
        /// <returns>If a device needs RTP connection, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_need_connect_rtp(ref G2GUID device)
        {
            return g2_admin_is_device_need_connect_rtp(_handle, ref device);
        }

        /// <summary>
        /// @ is_registered_request_recording_status
        /// <para>This method requests the recording status of a device.</para>
        /// </summary>
        /// <param name="root">root device unique id</param>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_registered_request_recording_status(ref G2GUID root)
        {
            return g2_admin_is_registered_request_recording_status(ref root);
        }

        /// <summary>
        /// @ is_activate_service_record
        /// <para>This method checks whether a camera device is under recording status. (except for instant recording)</para>
        /// </summary>
        /// <param name="camera">camera device unique id</param>
        /// <returns>If under recording status, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_activate_service_record(ref G2GUID camera)
        {
            return g2_admin_is_activate_service_record(ref camera);
        }

        /// <summary>
        /// @ is_activate_instant_record
        /// <para>This method checks whether a device is under recording or instant recording.</para>
        /// </summary>
        /// <param name="camera">camera device unique id</param>
        /// <param name="check_recording">It sets whether to return general recording status information.
        /// True : returns general recording status
        /// False : returns only whether or not it is under instant recording.
        /// </param>
        /// <returns>If a camera device is under instant recording, it returns TRUE; otherwise, it return FALSE.
        /// When check_recording is set to TRUE: if under instant recording, it returns general recording status; otherwise, it returns instant recording status.</returns>
        public bool is_activate_instant_record(ref G2GUID camera, bool check_recording)
        {
            return g2_admin_is_activate_instant_record(ref camera, check_recording);
        }
       
        /// <summary>
        /// @ is_contains_instant_record
        /// <para>This method checks whether a camera device is under instant recording.</para>
        /// </summary>
        /// <param name="camera">camera device unique id</param>
        /// <returns>If a camera device is under instant recording, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_contains_instant_record(ref G2GUID camera)
        {
            return g2_admin_is_contains_instant_record(ref camera);
        }

        /// <summary>
        /// @ is_contains_instant_record_failed
        /// <para>This method checks whether a camera device has failed in instant recording</para>
        /// </summary>
        /// <param name="camera">camera device unique id</param>
        /// <returns>If a camera device has failed in instant recording, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_contains_instant_record_failed(ref G2GUID camera)
        {
            return g2_admin_is_contains_instant_record_failed(ref camera);
        }
        public bool is_device_primary_recording_has_device(ref G2GUID root)
        {
            return g2_admin_is_device_primary_recording_has_device(ref root);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_connected(handle);
            return 1;
        }
        private G2RESULT on_disconnected(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_disconnected(handle, (G2DISCONNECT_REASON.TYPE)wparam, (G2SERVICE_LOGIN_FAIL_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_login_failed_from_dvrns(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2NETWORK_INFO ni = (G2NETWORK_INFO)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2NETWORK_INFO));
            _listener.on_g2admin_login_failed_from_dvrns(handle, ref ni, (G2FEN_RESULT.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_login_completed(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_login_completed(handle);
            return 1;
        }
        private G2RESULT on_failover_prepare(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_failover_prepare(handle, (G2DISCONNECT_REASON.TYPE)wparam, (G2SERVICE_LOGIN_FAIL_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_failover_connected(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_failover_connected(handle);
            return 1;
        }
        private G2RESULT on_failover_failed(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_failover_failed(handle, Convert.ToBoolean(wparam));
            return 1;
        }
        private G2RESULT on_notify_connectable_service(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            G2NETWORK_INFO ni = (G2NETWORK_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2NETWORK_INFO));
            _listener.on_g2admin_notify_connectable_service(handle, ref guid, ref ni);
            return 1;
        }
        private G2RESULT on_receive_device_empty(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_receive_empty_device(handle);
            return 1;
        }
        private G2RESULT on_receive_device_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GROUP_MEMBER_LIST data = new G2GROUP_MEMBER_LIST();
            data._members = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2GROUP_MEMBER[] bunch = data.to();
            _listener.on_g2admin_receive_device_append(handle, bunch);
            return 1;
        }
        private G2RESULT on_receive_device_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_receive_device_modify(handle, ref guid, Convert.ToBoolean(lparam));
            return 1;
        }
        private G2RESULT on_receive_device_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_receive_device_remove(handle, ref guid);
            return 1;
        }
        private G2RESULT on_receive_device_remove_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_GUID_LIST data = new G2PARAM_GUID_LIST();
            data._guid = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2GUID[] bunch = data.to();
            _listener.on_g2admin_receive_device_remove_list(handle, bunch);
            return 1;
        }
        private G2RESULT on_receive_device_group_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEVICE_GROUP_LIST data = new G2DEVICE_GROUP_LIST();
            data._groups = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2DEVICE_GROUP[] bunch = data.to();
            _listener.on_g2admin_receive_device_group_list(handle, bunch);
            return 1;
        }
        private G2RESULT on_receive_device_group_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEVICE_GROUP group = (G2DEVICE_GROUP)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2DEVICE_GROUP));
            _listener.on_g2admin_receive_device_group_append(handle, ref group);
            return 1;
        }
        private G2RESULT on_receive_device_group_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEVICE_GROUP group = (G2DEVICE_GROUP)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2DEVICE_GROUP));
            _listener.on_g2admin_receive_device_group_modify(handle, ref group);
            return 1;
        }
        private G2RESULT on_receive_device_group_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_receive_device_group_remove(handle, ref guid);
            return 1;
        }
        private G2RESULT on_receive_device_to_group_map(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GROUP_MEMBER_LIST data = new G2GROUP_MEMBER_LIST();
            data._members = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2GROUP_MEMBER[] bunch = data.to();
            _listener.on_g2admin_receive_device_to_group_map(handle, bunch);
            return 1;
        }
        private G2RESULT on_receive_device_append_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID device = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            G2GUID group = (G2GUID)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2GUID));
            _listener.on_g2admin_receive_device_append_to_group(handle, ref device, ref group);
            return 1;
        }
        private G2RESULT on_receive_device_remove_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID device = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            G2GUID group = (G2GUID)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2GUID));
            _listener.on_g2admin_receive_device_remove_to_group(handle, ref device, ref group);
            return 1;
        }
        private G2RESULT on_receive_device_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_GUID_LIST data = new G2PARAM_GUID_LIST();
            data._guid = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2GUID[] bunch = data.to();
            _listener.on_g2admin_receive_device_list(handle, bunch);
            return 1;
        }
        private G2RESULT on_receive_device_list_append_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GROUP_MEMBER_LIST data = new G2GROUP_MEMBER_LIST();
            data._members = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2GROUP_MEMBER[] bunch = data.to();
            _listener.on_g2admin_receive_device_list_append_to_group(handle, bunch);
            return 1;
        }
        private G2RESULT on_receive_device_list_remove_to_group(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GROUP_MEMBER_LIST data = new G2GROUP_MEMBER_LIST();
            data._members = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2GROUP_MEMBER[] bunch = data.to();
            _listener.on_g2admin_receive_device_list_remove_to_group(handle, bunch);
            return 1;
        }
        private G2RESULT on_receive_layout_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_GUID_LIST data = new G2PARAM_GUID_LIST();
            data._guid = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2GUID[] bunch = data.to();
            _listener.on_g2admin_receive_layout_list(handle, bunch);
            return 1;
        }
        private G2RESULT on_receive_layout_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_receive_layout_append(handle, ref guid);
            return 1;
        }
        private G2RESULT on_receive_layout_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_receive_layout_modify(handle, ref guid);
            return 1;
        }
        private G2RESULT on_receive_layout_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_receive_layout_remove(handle, ref guid);
            return 1;
        }
        private G2RESULT on_receive_sequence_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_GUID_LIST data = new G2PARAM_GUID_LIST();
            data._guid = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2GUID[] bunch = data.to();
            _listener.on_g2admin_receive_sequence_list(handle, bunch);
            return 1;
        }
        private G2RESULT on_receive_sequence_append(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_receive_sequence_append(handle, ref guid);
            return 1;
        }
        private G2RESULT on_receive_sequence_modify(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_receive_sequence_modify(handle, ref guid);
            return 1;
        }
        private G2RESULT on_receive_sequence_remove(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_receive_sequence_remove(handle, ref guid);
            return 1;
        }
        private G2RESULT on_receive_recording_device_status(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID service = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            G2RECORDING_DEVICE_STATUS status = (G2RECORDING_DEVICE_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2RECORDING_DEVICE_STATUS));
            _listener.on_g2admin_receive_recording_device_status(handle, ref service, ref status);
            return 1;
        }
        private G2RESULT on_receive_service_log_load(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SYSTEM_LOG log = (G2SYSTEM_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SYSTEM_LOG));
            _listener.on_g2admin_receive_service_log_load(handle, ref log);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_end(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_receive_service_log_load_end(handle);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_fail(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_receive_service_log_load_fail(handle);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_stop(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_receive_service_log_load_stop(handle);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEBUG_LOG log = (G2DEBUG_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2DEBUG_LOG));
            _listener.on_g2admin_receive_debug_log_load(handle, ref log);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_end(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_receive_debug_log_load_end(handle);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_fail(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_receive_debug_log_load_fail(handle);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_stop(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_receive_debug_log_load_stop(handle);
            return 1;
        }
        private G2RESULT on_modify_device_enable(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID device = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2admin_modify_device_enable(handle, ref device, Convert.ToBoolean(lparam));
            return 1;
        }

        private G2RESULT on_receive_test_failover_end(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2admin_receive_test_failover_end(handle, Convert.ToBoolean(wparam));
            return 1;
        }

        private G2RESULT on_receive_device_modify_list(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_GUID_LIST data = new G2PARAM_GUID_LIST();
            data._guid = (IntPtr)wparam;
            data._len = (uint)lparam;
            G2GUID[] bunch = data.to();
            _listener.on_g2admin_receive_device_modify_list(handle, bunch);
            return 1;
        }

        private G2RESULT on_receive_stream_device_in_charge(G2HADMIN handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID service = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            G2SERVICE_CHARGE_INFO info = (G2SERVICE_CHARGE_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SERVICE_CHARGE_INFO));
            
            G2PARAM_GUID_LIST data = new G2PARAM_GUID_LIST();
            data._guid = info._appends._guid;
            data._len = info._appends._len;
            G2GUID[] append = data.to();

            G2PARAM_GUID_LIST data2 = new G2PARAM_GUID_LIST();
            data2._guid = info._removes._guid;
            data2._len = info._removes._len;
            G2GUID[] remove = data2.to();

            _listener.on_g2admin_receive_stream_device_in_charge(handle, service, append, remove);
            return 1;
        }

        #endregion
    }
}
