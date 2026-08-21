using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
    using G2HSTATUS = System.Int32;
    using G2HWND = System.IntPtr;
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

    public interface g2status_listener
    {
        void on_g2status_connected(G2HSTATUS handle, int channel);
        void on_g2status_disconnected(G2HSTATUS handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2status_receive_instant_recording_start(G2HSTATUS handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status);
        void on_g2status_receive_instant_recording_stop(G2HSTATUS handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result);
        void on_g2status_receive_instant_recording_status(G2HSTATUS handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status);
        void on_g2status_receive_status(G2HSTATUS handle, int channel, ref G2MONITORING_DEVICE_STATUS status);
        void on_g2status_receive_callback_event(G2HSTATUS handle, ref G2EVENT_INFO info);
        void on_g2status_receive_log_system(G2HSTATUS handle, int channel, G2STATUS_LOG_SYSTEM[] logs);
        void on_g2status_receive_log_event(G2HSTATUS handle, int channel, G2STATUS_LOG_EVENT[] logs);
        void on_g2status_receive_log_debug(G2HSTATUS handle, int channel, G2STATUS_LOG_SYSTEM[] logs);
    }

    public class g2status
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_status_register_callback(G2HSTATUS handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HSTATUS g2_status_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_status_finalize(G2HSTATUS handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_status_startup(G2HSTATUS handle, int connections, int callback_connections);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_status_cleanup(G2HSTATUS handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_callback_server_restart(G2HSTATUS handle, ushort port);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_status_callback_server_shutdown(G2HSTATUS handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_callback_server_is_startup(G2HSTATUS handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_status_connect(G2HSTATUS handle, ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_status_connect_ras(G2HSTATUS handle, ref G2NETWORK_INFO ni, [MarshalAs(UnmanagedType.U1)] bool idr, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_status_disconnect(G2HSTATUS handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_is_connecting(G2HSTATUS handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_is_connected(G2HSTATUS handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_is_disconnecting(G2HSTATUS handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_is_disconnected(G2HSTATUS handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_is_disconnectable(G2HSTATUS handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_request_alive_check(G2HSTATUS handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_request_panic_record(G2HSTATUS handle, int channel, [MarshalAs(UnmanagedType.U1)] bool on);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_request_instant_recording_start(G2HSTATUS handle, int channel, ref G2INSTANT_RECORD_SETTING_LIST setting);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_request_instant_recording_stop(G2HSTATUS handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_request_instant_recording_status(G2HSTATUS handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_request_status(G2HSTATUS handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_request_log(G2HSTATUS handle, int channel, ref G2STATUS_LOG_SEARCH_OPTIONS options, out int res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_get_adaptor(G2HSTATUS handle, IntPtr ptr);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_get_server_network_info(G2HSTATUS handle, int channel, out G2SERVER_NETWORK_INFO ni);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_get_product_info(G2HSTATUS handle, int channel, out G2_PRODUCT_INFO pi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_get_remote_status_caps(G2HSTATUS handle, int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_STATUS caps);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_get_remote_setup_info_file(G2HSTATUS handle, int channel, string path, string sitename, G2HWND parent);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_get_authority(G2HSTATUS handle, int channel, out G2RAS_AUTHORITY auth);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_get_status(G2HSTATUS handle, int channel, out G2MONITORING_DEVICE_STATUS status);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_is_support(G2HSTATUS handle, int channel, int query);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_is_authority(G2HSTATUS handle, int channel, int query);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_status_is_IDR(G2HSTATUS handle, int channel);
        #endregion

        public g2status()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_receive_instant_recording_start = new G2FUN_LISTENER(on_receive_instant_recording_start);
            this._p2_on_receive_instant_recording_stop = new G2FUN_LISTENER(on_receive_instant_recording_stop);
            this._p2_on_receive_instant_recording_status = new G2FUN_LISTENER(on_receive_instant_recording_status);
            this._p2_on_receive_status = new G2FUN_LISTENER(on_receive_status);
            this._p2_on_receive_callback_event = new G2FUN_LISTENER(on_receive_callback_event);
            this._p2_on_receive_log_system = new G2FUN_LISTENER(on_receive_log_system);
            this._p2_on_receive_log_event = new G2FUN_LISTENER(on_receive_log_event);
            this._p2_on_receive_log_debug = new G2FUN_LISTENER(on_receive_log_debug);
        }
        ~g2status()
        {
            cleanup();
        }

        private G2HSTATUS _handle;
        private G2UPARAM _param;
        private g2status_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2STATUS_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_status_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_instant_recording_start;
        private G2FUN_LISTENER _p2_on_receive_instant_recording_stop;
        private G2FUN_LISTENER _p2_on_receive_instant_recording_status;
        private G2FUN_LISTENER _p2_on_receive_status;
        private G2FUN_LISTENER _p2_on_receive_callback_event;
        private G2FUN_LISTENER _p2_on_receive_log_system;
        private G2FUN_LISTENER _p2_on_receive_log_event;
        private G2FUN_LISTENER _p2_on_receive_log_debug;
        #endregion

        public G2HSTATUS safe_handle() { return _handle; }

        public void startup(int connections, int callback_connections)
        {
            cleanup();

            _handle = g2_status_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2STATUS_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2STATUS_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2STATUS_CALLBACK.TYPE.on_receive_instant_recording_start, _p2_on_receive_instant_recording_start);
            register_callback(G2STATUS_CALLBACK.TYPE.on_receive_instant_recording_stop, _p2_on_receive_instant_recording_stop);
            register_callback(G2STATUS_CALLBACK.TYPE.on_receive_instant_recording_status, _p2_on_receive_instant_recording_status);
            register_callback(G2STATUS_CALLBACK.TYPE.on_receive_status, _p2_on_receive_status);
            register_callback(G2STATUS_CALLBACK.TYPE.on_receive_callback_event, _p2_on_receive_callback_event);
            register_callback(G2STATUS_CALLBACK.TYPE.on_receive_log_system, _p2_on_receive_log_system);
            register_callback(G2STATUS_CALLBACK.TYPE.on_receive_log_event, _p2_on_receive_log_event);
            register_callback(G2STATUS_CALLBACK.TYPE.on_receive_log_debug, _p2_on_receive_log_debug);
            #endregion

            g2_status_startup(_handle, connections, callback_connections);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HSTATUS handle = _handle; _handle = 0;
                g2_status_cleanup(handle);
                g2_status_finalize(handle);
            }
        }
        public void set_listener(g2status_listener listener)
        {
            _listener = listener;
        }

        public bool callback_server_restart(ushort port)
        {
            return g2_status_callback_server_restart(_handle, port);
        }
        public void callback_server_shutdown()
        {
            g2_status_callback_server_shutdown(_handle);
        }
        public bool callback_server_is_startup()
        {
            return g2_status_callback_server_is_startup(_handle);
        }

        public int connect(ref G2GUID root, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_status_connect(_handle, ref root, ref options, out res);
        }
        public int connect(ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_status_connect(_handle, ref root, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, bool idr, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_status_connect_ras(_handle, ref ni, idr, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, bool idr, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_status_connect_ras(_handle, ref ni, idr, ref options, out res);
        }
        public void disconnect(int channel)
        {
            g2_status_disconnect(_handle, channel);
        }
        public bool is_connecting(int channel)
        {
            return g2_status_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_status_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_status_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_status_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_status_is_disconnectable(_handle, channel);
        }

        public bool request_alive_check(int channel)
        {
            return g2_status_request_alive_check(_handle, channel);
        }
        public bool request_panic_record(int channel, bool on)
        {
            return g2_status_request_panic_record(_handle, channel, on);
        }
        public bool request_instant_recording_start(int channel, G2INSTANT_RECORD_SETTING[] setting)
        {
            GCHandle gch = GCHandle.Alloc(setting, GCHandleType.Pinned);
            G2INSTANT_RECORD_SETTING_LIST param = new G2INSTANT_RECORD_SETTING_LIST();
            param._list = gch.AddrOfPinnedObject();
            param._len = (uint)setting.Length;
            g2_status_request_instant_recording_start(_handle, channel, ref param);
            gch.Free();
            return true;
        }
        public bool request_instant_recording_stop(int channel, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_status_request_instant_recording_stop(_handle, channel, ref chs);
        }
        public bool request_instant_recording_status(int channel, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_status_request_instant_recording_status(_handle, channel, ref chs);
        }
        public bool request_instant_recording_status(int channel)
        {
            G2CHANNEL_SET chs = G2CHANNEL_SET.G2CHANNEL_SET_ALL;
            return g2_status_request_instant_recording_status(_handle, channel, ref chs);
        }
        public bool request_status(int channel)
        {
            return g2_status_request_status(_handle, channel);
        }
        public bool request_log(int channel, ref G2STATUS_LOG_SEARCH_OPTIONS options, out G2STATUS_LOG.RESULT res)
        {
            int r = 0;
            bool ret = g2_status_request_log(_handle, channel, ref options, out r);
            res = (G2STATUS_LOG.RESULT)r;
            return ret;
        }

        public G2FUN_GET_ADAPTOR get_adaptor()
        {
            return new G2FUN_GET_ADAPTOR(g2_status_get_adaptor);
        }
        public bool get_server_network_info(int channel, out G2SERVER_NETWORK_INFO ni)
        {
            return g2_status_get_server_network_info(_handle, channel, out ni);
        }
        public bool get_product_info(int channel, out G2_PRODUCT_INFO pi)
        {
            return g2_status_get_product_info(_handle, channel, out pi);
        }
        public bool get_remote_status_caps(int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_STATUS caps)
        {
            return g2_status_get_remote_status_caps(_handle, channel, out caps);
        }
        public bool get_remote_setup_info_file(int channel, string path, string sitename, G2HWND focus)
        {
            return g2_status_get_remote_setup_info_file(_handle, channel, path, sitename, focus);
        }
        public bool get_authority(int channel, out G2RAS_AUTHORITY auth)
        {
            return g2_status_get_authority(_handle, channel, out auth);
        }
        public bool get_status(int channel, ref G2MONITORING_DEVICE_STATUS status)
        {
            return g2_status_get_status(_handle, channel, out status);
        }

        public bool is_support(int channel, G2STATUS_SUPPORT.QUERY query)
        {
            return g2_status_is_support(_handle, channel, (int)query);
        }
        public bool is_authority(int channel, G2RAS_AUTHORITY.TYPE authority)
        {
            return g2_status_is_authority(_handle, channel, (int)authority);
        }
        public bool is_IDR(int channel)
        {
            return g2_status_is_IDR(_handle, channel);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2status_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2status_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_instant_recording_start(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS p = (G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS));
            G2INSTANT_RECORDING_CHANNEL_STATUS[] status = new G2INSTANT_RECORDING_CHANNEL_STATUS[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._list.ToInt64() + i * Marshal.SizeOf(typeof(G2INSTANT_RECORDING_CHANNEL_STATUS)));
                status[i] = (G2INSTANT_RECORDING_CHANNEL_STATUS)Marshal.PtrToStructure(ptr, typeof(G2INSTANT_RECORDING_CHANNEL_STATUS));
            }
            _listener.on_g2status_receive_instant_recording_start(handle, (int)wparam, (G2INSTANT_RECORDING_RESULT.TYPE)p._result, status);
            return 1;
        }
        private G2RESULT on_receive_instant_recording_stop(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2status_receive_instant_recording_stop(handle, (int)wparam, (G2INSTANT_RECORDING_RESULT.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_instant_recording_status(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS p = (G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS));
            G2INSTANT_RECORDING_CHANNEL_STATUS[] status = new G2INSTANT_RECORDING_CHANNEL_STATUS[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._list.ToInt64() + i * Marshal.SizeOf(typeof(G2INSTANT_RECORDING_CHANNEL_STATUS)));
                status[i] = (G2INSTANT_RECORDING_CHANNEL_STATUS)Marshal.PtrToStructure(ptr, typeof(G2INSTANT_RECORDING_CHANNEL_STATUS));
            }
            _listener.on_g2status_receive_instant_recording_status(handle, (int)wparam, (G2INSTANT_RECORDING_RESULT.TYPE)p._result, status);
            return 1;
        }
        private G2RESULT on_receive_status(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2MONITORING_DEVICE_STATUS status = (G2MONITORING_DEVICE_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2MONITORING_DEVICE_STATUS));
            _listener.on_g2status_receive_status(handle, (int)wparam, ref status);
            return 1;
        }
        private G2RESULT on_receive_callback_event(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_INFO info = (G2EVENT_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_INFO));
            _listener.on_g2status_receive_callback_event(handle, ref info);
            return 1;
        }
        private G2RESULT on_receive_log_system(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH buf = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2STATUS_LOG_SYSTEM[] logs = new G2STATUS_LOG_SYSTEM[buf._len];
            for (int i = 0; i < buf._len; ++i)
            {
                IntPtr ptr = new IntPtr(buf._params.ToInt64() + i * Marshal.SizeOf(typeof(G2STATUS_LOG_SYSTEM)));
                logs[i] = (G2STATUS_LOG_SYSTEM)Marshal.PtrToStructure(ptr, typeof(G2STATUS_LOG_SYSTEM));
            }
            _listener.on_g2status_receive_log_system(handle, (int)wparam, logs);
            return 1;
        }
        private G2RESULT on_receive_log_event(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH buf = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2STATUS_LOG_EVENT[] logs = new G2STATUS_LOG_EVENT[buf._len];
            for (int i = 0; i < buf._len; ++i)
            {
                IntPtr ptr = new IntPtr(buf._params.ToInt64() + i * Marshal.SizeOf(typeof(G2STATUS_LOG_EVENT)));
                logs[i] = (G2STATUS_LOG_EVENT)Marshal.PtrToStructure(ptr, typeof(G2STATUS_LOG_EVENT));
            }
            _listener.on_g2status_receive_log_event(handle, (int)wparam, logs);
            return 1;
        }
        private G2RESULT on_receive_log_debug(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH buf = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2STATUS_LOG_SYSTEM[] logs = new G2STATUS_LOG_SYSTEM[buf._len];
            for (int i = 0; i < buf._len; ++i)
            {
                IntPtr ptr = new IntPtr(buf._params.ToInt64() + i * Marshal.SizeOf(typeof(G2STATUS_LOG_SYSTEM)));
                logs[i] = (G2STATUS_LOG_SYSTEM)Marshal.PtrToStructure(ptr, typeof(G2STATUS_LOG_SYSTEM));
            }
            _listener.on_g2status_receive_log_debug(handle, (int)wparam, logs);
            return 1;
        }
        #endregion
    }
}
