using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HMONITOR = System.Int32;
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

    public interface g2monitor_listener
    {
        /// <summary>
        /// @ on_g2monitor_connected
        /// <para>callback when connected to the monitoring service</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="service">monitoring service guid</param>
        void on_g2monitor_connected(G2HMONITOR handle, ref G2GUID service);
        /// <summary>
        /// @ on_g2monitor_disconnected
        /// <para>callback when disconnected from the monitoring service</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="service">service guid</param>
        /// <param name="reason">reason for disconnection</param>
        void on_g2monitor_disconnected(G2HMONITOR handle, ref G2GUID service, G2DISCONNECT_REASON.TYPE reason);
        /// <summary>
        /// @ on_g2monitor_require_reconnect
        /// <para>callback when reconnection to the monitoring service is required</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="service">service unique id</param>
        /// <param name="reconnect">whether do or not reconnect</param> 
        void on_g2monitor_require_reconnect(G2HMONITOR handle, ref G2GUID service, ref bool reconnect);
        /// <summary>
        /// @ on_g2monitor_receive_event
        /// <para>callback when an event has occurred</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="ei">event information</param>
        void on_g2monitor_receive_event(G2HMONITOR handle, ref G2EVENT_INFO ei);
        /// <summary>
        /// @ on_g2monitor_receive_service_log_load
        /// <para>callback for the search for the monitoring service system log </para>
        /// </summary>
        /// <param name="handle">g2_ monitor control handle</param>
        /// <param name="log">system log information</param>
        void on_g2monitor_receive_service_log_load(G2HMONITOR handle, ref G2SYSTEM_LOG log);
        /// <summary>
        /// @ on_g2monitor_receive_service_log_load_end
        /// <para>callback for the end of the search for the monitoring service system log</para>
        /// </summary>
        /// <param name="handle">g2_ monitor control handle</param>
        void on_g2monitor_receive_service_log_load_end(G2HMONITOR handle);
        /// <summary>
        /// @ on_g2monitor_receive_service_log_load_fail
        /// <para>callback for the failure of the search for the monitoring service system log</para>
        /// </summary>
        /// <param name="handle">g2_ monitor control handle</param>
        void on_g2monitor_receive_service_log_load_fail(G2HMONITOR handle);
        /// <summary>
        /// @ on_g2monitor_receive_service_log_load_stop
        /// <para>callback for the stop of the search for the monitoring service system log</para>
        /// </summary>
        /// <param name="handle">g2_ monitor control handle</param>
        void on_g2monitor_receive_service_log_load_stop(G2HMONITOR handle);
        /// <summary>
        /// @ on_g2monitor_status_connected
        /// <para>callback when successfully connected to the monitoring service for the purpose of looking up device status information</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="service">service guid</param>
        void on_g2monitor_status_connected(G2HMONITOR handle, ref G2GUID service);
        /// <summary>
        /// @ on_g2monitor_status_disconnected
        /// <para>callback when disconnected from the monitoring service</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="service">service guid</param>
        /// <param name="reason">reason for disconnection</param>
        void on_g2monitor_status_disconnected(G2HMONITOR handle, ref G2GUID service, G2DISCONNECT_REASON.TYPE reason);
        /// <summary>
        /// @ on_g2monitor_status_receive_device_status
        /// <para>device status information callback</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="status">status information</param>
        void on_g2monitor_status_receive_device_status(G2HMONITOR handle, G2MONITORING_DEVICE_STATUS[] status);
        /// <summary>
        /// @ on_g2monitor_status_receive_device_health
        /// <para>device health information callback</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="health">Health information</param>
        void on_g2monitor_status_receive_device_health(G2HMONITOR handle, G2MONITORING_DEVICE_HEALTH[] health);
        /// <summary>
        /// @ on_g2monitor_status_receive_event
        /// <para>callback in real time when an event occurs</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="ei">event information</param>
        void on_g2monitor_status_receive_event(G2HMONITOR handle, ref G2EVENT_INFO ei);
        /// <summary>
        /// @ on_g2monitor_receive_debug_log_load
        /// <para>callback for the search for the monitoring service debug log </para>
        /// </summary>
        /// <param name="handle">g2_ monitor control handle</param>
        /// <param name="log">debug log information</param>
        void on_g2monitor_receive_debug_log_load(G2HMONITOR handle, ref G2DEBUG_LOG log);
        /// <summary>
        /// @ on_g2monitor_receive_debug_log_load_end
        /// <para>callback for the end of the search for the monitoring service debug log</para>
        /// </summary>
        /// <param name="handle">g2_ monitor control handle</param>
        void on_g2monitor_receive_debug_log_load_end(G2HMONITOR handle);
        /// <summary>
        /// @ on_g2monitor_receive_debug_log_load_fail
        /// <para>callback for the failure of the search for the monitoring service debug log</para>
        /// </summary>
        /// <param name="handle">g2_ monitor control handle</param>
        void on_g2monitor_receive_debug_log_load_fail(G2HMONITOR handle);
        /// <summary>
        /// @ on_g2monitor_receive_debug_log_load_stop
        /// <para>callback for the stop of the search for the monitoring service debug log</para>
        /// </summary>
        /// <param name="handle">g2_ monitor control handle</param>
        void on_g2monitor_receive_debug_log_load_stop(G2HMONITOR handle);
    }

    public class g2monitor
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_monitor_register_callback(G2HMONITOR handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HMONITOR g2_monitor_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_monitor_finalize(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_monitor_startup(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_monitor_cleanup(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_connect(G2HMONITOR handle, ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_monitor_disconnect(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_is_connecting(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_is_connected(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_is_disconnecting(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_is_disconnected(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_is_disconnectable(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_monitor_set_activate(G2HMONITOR handle, [MarshalAs(UnmanagedType.U1)] bool activate);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_monitor_set_disconnect_by_me(G2HMONITOR handle, [MarshalAs(UnmanagedType.U1)] bool me);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_set_device_list_interest_for_live(G2HMONITOR handle, G2GUID[] devices, uint length, [MarshalAs(UnmanagedType.U1)] bool invoke);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_set_device_list_interest_for_status(G2HMONITOR handle, G2GUID[] devices, uint length);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_set_device_list_interest_for_health(G2HMONITOR handle, G2GUID[] devices, uint length);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_request_alive_check(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_request_update_device_list_interest_for_status(G2HMONITOR handle, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_request_update_device_list_interest_for_health(G2HMONITOR handle, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_monitor_request_delete_device_status(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_monitor_request_delete_device_status_and_health(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_request_user_alarm_in_command(G2HMONITOR handle, ref G2MONITORING_USER_ALARM_IN_REQUEST request);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_request_system_log_search(G2HMONITOR handle, ref G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_request_system_log_search_stop(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_get_service_guid(G2HMONITOR handle, out G2GUID service);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_is_activate(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_is_monitoring(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_is_disconnect_by_me(G2HMONITOR handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_request_debug_log_search(G2HMONITOR handle, ref G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_monitor_request_debug_log_search_stop(G2HMONITOR handle);
        #endregion

        private g2monitor()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_require_reconnect = new G2FUN_LISTENER(on_require_reconnect);
            this._p2_on_receive_event = new G2FUN_LISTENER(on_receive_event);
            this._p2_on_receive_service_log_load = new G2FUN_LISTENER(on_receive_service_log_load);
            this._p2_on_receive_service_log_load_end = new G2FUN_LISTENER(on_receive_service_log_load_end);
            this._p2_on_receive_service_log_load_fail = new G2FUN_LISTENER(on_receive_service_log_load_fail);
            this._p2_on_receive_service_log_load_stop = new G2FUN_LISTENER(on_receive_service_log_load_stop);
            this._p2_on_status_connected = new G2FUN_LISTENER(on_status_connected);
            this._p2_on_status_disconnected = new G2FUN_LISTENER(on_status_disconnected);
            this._p2_on_status_receive_device_status = new G2FUN_LISTENER(on_status_receive_device_status);
            this._p2_on_status_receive_device_health = new G2FUN_LISTENER(on_status_receive_device_health);
            this._p2_on_status_receive_event = new G2FUN_LISTENER(on_status_receive_event);
            this._p2_on_receive_debug_log_load = new G2FUN_LISTENER(on_receive_debug_log_load);
            this._p2_on_receive_debug_log_load_end = new G2FUN_LISTENER(on_receive_debug_log_load_end);
            this._p2_on_receive_debug_log_load_fail = new G2FUN_LISTENER(on_receive_debug_log_load_fail);
            this._p2_on_receive_debug_log_load_stop = new G2FUN_LISTENER(on_receive_debug_log_load_stop);
        }
        ~g2monitor()
        {
            cleanup();
        }

        public static g2monitor get()
        {
            return s_singleton;
        }

        public static g2monitor s_singleton = new g2monitor();
        private G2HMONITOR _handle;
        private G2UPARAM _param;
        private g2monitor_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2MONITOR_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_monitor_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_require_reconnect;
        private G2FUN_LISTENER _p2_on_receive_event;
        private G2FUN_LISTENER _p2_on_receive_service_log_load;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_stop;
        private G2FUN_LISTENER _p2_on_status_connected;
        private G2FUN_LISTENER _p2_on_status_disconnected;
        private G2FUN_LISTENER _p2_on_status_receive_device_status;
        private G2FUN_LISTENER _p2_on_status_receive_device_health;
        private G2FUN_LISTENER _p2_on_status_receive_event;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_stop;
        #endregion

        public G2HMONITOR safe_handle() { return _handle; }

        /// <summary>
        /// @ startup
        /// <para>This method creates g2monitor object, and registers g2monitor callback functions.</para>
        /// </summary>
        public void startup()
        {
            cleanup();

            _handle = g2_monitor_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2MONITOR_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_require_reconnect, _p2_on_require_reconnect);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_receive_event, _p2_on_receive_event);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_receive_service_log_load, _p2_on_receive_service_log_load);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_receive_service_log_load_end, _p2_on_receive_service_log_load_end);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_receive_service_log_load_fail, _p2_on_receive_service_log_load_fail);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_receive_service_log_load_stop, _p2_on_receive_service_log_load_stop);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_status_connected, _p2_on_status_connected);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_status_disconnected, _p2_on_status_disconnected);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_status_receive_device_status, _p2_on_status_receive_device_status);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_status_receive_device_health, _p2_on_status_receive_device_health);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_status_receive_event, _p2_on_status_receive_event);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_receive_debug_log_load, _p2_on_receive_debug_log_load);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_receive_debug_log_load_end, _p2_on_receive_debug_log_load_end);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_receive_debug_log_load_fail, _p2_on_receive_debug_log_load_fail);
            register_callback(G2MONITOR_CALLBACK.TYPE.on_receive_debug_log_load_stop, _p2_on_receive_debug_log_load_stop);
            #endregion

            g2_monitor_startup(_handle);
        }

        /// <summary>
        /// @ cleanup
        /// <para>This method deletes the memory and resources created by g2_monitor_startup(). </para>
        /// </summary>
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HMONITOR handle = _handle; _handle = 0;
                g2_monitor_cleanup(handle);
                g2_monitor_finalize(handle);
            }
        }

        /// <summary>
        /// @ set_listener
        /// <para>This method registers g2moniotr_listener function.</para>
        /// </summary>
        /// <param name="listener">listener function</param>
        public void set_listener(g2monitor_listener listener)
        {
            _listener = listener;
        }
        /// <summary>
        /// @ connect
        /// <para>This method connects to the monitoring service.</para>
        /// </summary>
        /// <param name="service">monitoring service unique id</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection fails, it returns -1; otherwise, it returns channel no. (other than -1).</returns>
        public bool connect(ref G2GUID service, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_monitor_connect(_handle, ref service, ref options, out res);
        }

        /// <summary>
        /// @ connect
        /// <para>This method connects to the monitoring service.</para>
        /// </summary>
        /// <param name="service">monitoring service unique id</param>
        /// <param name="options">The structure wherein the options for service connection is saved. It has information about connection time out.</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection fails, it returns -1; otherwise, it returns channel no. (other than -1).</returns>
        public bool connect(ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_monitor_connect(_handle, ref service, ref options, out res);
        }

        /// <summary>
        /// @ disconnect
        /// <para>This method disconnects the monitoring service for the corresponding channel.</para>
        /// </summary>
        public void disconnect()
        {
            g2_monitor_disconnect(_handle);
        }

        /// <summary>
        /// @ is_connecting
        /// <para>This method checks whether the corresponding channel is connecting to the monitoring service.</para>
        /// </summary>
        /// <returns>If in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connecting()
        {
            return g2_monitor_is_connecting(_handle);
        }

        /// <summary>
        /// @ is_connected
        /// <para>This method checks whether the corresponding channel is connected to the monitoring service.</para>
        /// </summary>
        /// <returns>If connected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connected()
        {
            return g2_monitor_is_connected(_handle);
        }

        /// <summary>
        /// @ is_disconnecting
        /// <para>This method checks whether the corresponding channel is disconnecting from the monitoring service.</para>
        /// </summary>
        /// <returns>If in the process of disconnecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnecting()
        {
            return g2_monitor_is_disconnecting(_handle);
        }

        /// <summary>
        /// @ is_disconnected
        /// <para>This method checks whether the corresponding channel is disconnected from the monitoring service.</para>
        /// </summary>
        /// <returns>If disconnected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnected()
        {
            return g2_monitor_is_disconnected(_handle);
        }

        /// <summary>
        /// @ is_disconnectable
        /// <para>This method checks whether the corresponding channel is connected or in the process of connecting to the monitoring service.</para>
        /// </summary>
        /// <returns>If connected or in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnectable()
        {
            return g2_monitor_is_disconnectable(_handle);
        }

        /// <summary>
        /// @ set_activate
        /// <para>This method activates the monitoring service.</para>
        /// </summary>
        /// <param name="activate">To activate, set to TRUE; to deactivate, set to FALSE.</param>
        public void set_activate(bool activate)
        {
            g2_monitor_set_activate(_handle, activate);
        }
        
        /// <summary>
        /// @ set_disconnect_by_me
        /// <para>This method sets a disconnection by user.</para>
        /// </summary>
        /// <param name="me">For a disconnection intended by the user, set to TRUE; otherwise, set to FALSE.</param>
        public void set_disconnect_by_me(bool me)
        {
            g2_monitor_set_disconnect_by_me(_handle, me);
        }

        /// <summary>
        /// @ set_device_list_interest_for_live
        /// <para>This method sets up the device of interest in the monitoring service.</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="GUIDs">device unique id list</param>
        /// <param name="invoke">whether do or not invoke to devices</param>
        /// <returns></returns>
        public bool set_device_list_interest_for_live(G2HMONITOR handle, G2GUIDSET GUIDs, bool invoke)
        {
            G2GUID[] param = GUIDs.to_array();
            return g2_monitor_set_device_list_interest_for_live(_handle, param, (uint)param.Length, invoke);
        }

        /// <summary>
        /// @ set_device_list_interest_for_status
        /// <para>This method sets up the device of interest in the monitoring service.</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="GUIDs">device unique id list</param>
        /// <returns></returns>
        public bool set_device_list_interest_for_status(G2HMONITOR handle, G2GUIDSET GUIDs)
        {
            G2GUID[] param = GUIDs.to_array();
            return g2_monitor_set_device_list_interest_for_status(handle, param, (uint)param.Length);
        }

        /// <summary>
        /// @ set_device_list_interest_for_health
        /// <para>This method sets up the device of interest in the monitoring service.</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <param name="GUIDs">device unique id list</param>
        /// <returns></returns>
        public bool set_device_list_interest_for_health(G2HMONITOR handle, G2GUIDSET GUIDs)
        {
            G2GUID[] param = GUIDs.to_array();
            return g2_monitor_set_device_list_interest_for_health(handle, param, (uint)param.Length);
        }

        /// <summary>
        /// @ request_alive_check
        /// <para>This method sends packets periodically to maintain the connection to the monitoring service.</para>
        /// </summary>
        /// <returns>If setting is successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_alive_check()
        {
            return g2_monitor_request_alive_check(_handle);
        }

        /// <summary>
        /// @ request_update_device_list_interest_for_status
        /// <para>This method updates the device list interest for status</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <returns></returns>
        public bool request_update_device_list_interest_for_status(G2HMONITOR handle)
        {
            return g2_monitor_request_update_device_list_interest_for_status(handle, false);
        }

        /// <summary>
        /// @ request_update_device_list_interest_for_health
        /// <para>This method updates the device list interest for health</para>
        /// </summary>
        /// <param name="handle">g2_monitor control handle</param>
        /// <returns></returns>
        public bool request_update_device_list_interest_for_health(G2HMONITOR handle)
        {
            return g2_monitor_request_update_device_list_interest_for_health(handle, false);
        }

        /// <summary>
	    /// @ request_delete_device_status_and_health
        /// <para>This method delete all request device status and health.</para>
        /// </summary>
        public void request_delete_device_status_and_health() 
        {
            g2_monitor_request_delete_device_status_and_health(_handle);
        }

	    /// <summary>
	    /// @request_user_alarm_in_request
	    /// <para>This method sends user alarm in data to Monitoring Service.</para>
	    /// </summary>
	    /// <param name="request">user alarm in data structure</param>
	    /// <returns>returns result of user alarm in sending.</returns>
        public bool request_user_alarm_in_request(G2MONITORING_USER_ALARM_IN_REQUEST request)
        {
            return g2_monitor_request_user_alarm_in_command(_handle, ref request);
        }

    	/// <summary>
        /// @ request_system_log_search
        /// <para>This method requests the monitoring service system log.</para>
        /// </summary>
        /// <param name="option">structure wherein search option information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_system_log_search(G2SERVICE_SEARCH_OPTION_SYSTEM_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_monitor_request_system_log_search(_handle, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @ request_system_log_search_stop
        /// <para>This method cancels the request for the monitoring service system log.</para>
        /// </summary>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_system_log_search_stop()
        {
            return g2_monitor_request_system_log_search_stop(_handle);
        }

        /// <summary>
        /// @ request_debug_log_search
        /// <para>This method requests the monitoring service debug log.</para>
        /// </summary>
        /// <param name="option">structure wherein search option information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_debug_log_search(G2SERVICE_SEARCH_OPTION_DEBUG_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_monitor_request_debug_log_search(_handle, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @ request_debug_log_search_stop
        /// <para>This method cancels the request for the monitoring service debug log.</para>
        /// </summary>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_debug_log_search_stop()
        {
            return g2_monitor_request_debug_log_search_stop(_handle);
        }

        /// <summary>
        /// <para>This method obtains monitoring service unique id</para>
        /// </summary>
        /// <param name="service">structure to save service unique id</param>
        /// <returns></returns>
        public bool get_service_guid(out G2GUID service)
        {
            return g2_monitor_get_service_guid(_handle, out service);
        }
        /// <summary>
        /// <para>This method obtains monitoring service unique id</para>
        /// </summary>
        /// <returns>If a request succeeds, it returns monitoring service unique id; otherwise, it returns invalid unique id.</returns>
        public G2GUID get_service_guid()            
        {
            G2GUID service;
            g2_monitor_get_service_guid(_handle, out service);
            return service;
        }

        /// <summary>
        /// @ is_activate
        /// <para>This method checks whether the monitoring service is activated.</para>
        /// </summary>
        /// <returns>If activated, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_activate()
        {
            return g2_monitor_is_activate(_handle);
        }

        /// <summary>
        /// @ is_monitoring
        /// <para>This method checks whether the monitoring service is monitoring a device.</para>
        /// </summary>
        /// <returns>If the service is monitoring a device, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_monitoring()
        {
            return g2_monitor_is_monitoring(_handle);
        }

        /// <summary>
        /// @ is_disconnect_by_me
        /// <para>This obtains the configuration to decide whether or not it has been disconnected by user.</para>
        /// </summary>
        /// <returns>If configured as disconnection by the user, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnect_by_me()
        {
            return g2_monitor_is_disconnect_by_me(_handle);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2monitor_connected(handle, ref guid);
            return 1;
        }
        private G2RESULT on_disconnected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2monitor_disconnected(handle, ref guid, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_require_reconnect(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            uint reconnect = (uint)Marshal.PtrToStructure((IntPtr)lparam, typeof(uint));
            bool val = (reconnect == G2BOOLEAN.G2TRUE);
            _listener.on_g2monitor_require_reconnect(handle, ref guid, ref val);
            reconnect = (uint)(val ? G2BOOLEAN.G2TRUE : G2BOOLEAN.G2FALSE);
            Marshal.StructureToPtr(reconnect, (IntPtr)lparam, true);
            return 1;
        }
        private G2RESULT on_receive_event(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_INFO ei = (G2EVENT_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_INFO));
            _listener.on_g2monitor_receive_event(handle, ref ei);
            return 1;
        }
        private G2RESULT on_receive_service_log_load(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SYSTEM_LOG log = (G2SYSTEM_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SYSTEM_LOG));
            _listener.on_g2monitor_receive_service_log_load(handle, ref log);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_end(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2monitor_receive_service_log_load_end(handle);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_fail(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2monitor_receive_service_log_load_fail(handle);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_stop(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2monitor_receive_service_log_load_stop(handle);
            return 1;
        }
        private G2RESULT on_status_connected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2monitor_status_connected(handle, ref guid);
            return 1;
        }
        private G2RESULT on_status_disconnected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2GUID));
            _listener.on_g2monitor_status_disconnected(handle, ref guid, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_status_receive_device_status(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2MONITORING_DEVICE_STATUS[] status = new G2MONITORING_DEVICE_STATUS[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2MONITORING_DEVICE_STATUS)));
                status[i] = (G2MONITORING_DEVICE_STATUS)Marshal.PtrToStructure(ptr, typeof(G2MONITORING_DEVICE_STATUS));
            }
            _listener.on_g2monitor_status_receive_device_status(handle, status);
            return 1;
        }
        private G2RESULT on_status_receive_device_health(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2MONITORING_DEVICE_HEALTH[] health = new G2MONITORING_DEVICE_HEALTH[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2MONITORING_DEVICE_HEALTH)));
                health[i] = (G2MONITORING_DEVICE_HEALTH)Marshal.PtrToStructure(ptr, typeof(G2MONITORING_DEVICE_HEALTH));
            }
            _listener.on_g2monitor_status_receive_device_health(handle, health);
            return 1;
        }
        private G2RESULT on_status_receive_event(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_INFO ei = (G2EVENT_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_INFO));
            _listener.on_g2monitor_status_receive_event(handle, ref ei);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEBUG_LOG log = (G2DEBUG_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2DEBUG_LOG));
            _listener.on_g2monitor_receive_debug_log_load(handle, ref log);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_end(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2monitor_receive_debug_log_load_end(handle);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_fail(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2monitor_receive_debug_log_load_fail(handle);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_stop(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2monitor_receive_debug_log_load_stop(handle);
            return 1;
        }
        #endregion
    }
}
