using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
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

    public interface g2app_frame_relay_listener
    {
        void on_g2app_frame_relay_connected(G2HANDLE handle, int channel);
        void on_g2app_frame_relay_disconnected(G2HANDLE handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2app_frame_relay_receive_request_site_product_info(G2HANDLE handle, int channel);
    }

    public class g2app_frame_relay
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_register_callback(G2HANDLE handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HANDLE g2_app_frame_relay_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_finalize(G2HANDLE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_startup(G2HANDLE handle, int connections, uint send_queue_size_limit);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_cleanup(G2HANDLE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_app_frame_relay_connect(G2HANDLE handle, ref G2NETWORK_INFO ni, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_disconnect(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_is_connecting(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_is_connected(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_is_disconnecting(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_is_disconnected(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_is_disconnectable(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_request_alive_check(G2HANDLE handle, int channel, int val);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_send_site_connected(G2HANDLE handle, int channel, ref G2STRING_64 site);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_send_site_disconnected(G2HANDLE handle, int channel, ref G2STRING_64 site, int reason);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_send_site_product_info(G2HANDLE handle, int channel, ref G2STRING_64 site, IntPtr fn, G2HANDLE from_handle, int from_channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_send_site_frame_data(G2HANDLE handle, int channel, ref G2STRING_64 site, ref G2FRAME frame);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_is_able_to_send(G2HANDLE handle, int channel, uint size);
        #endregion

        public g2app_frame_relay()
        {
            _param = new G2UPARAM(0);
            _handle = g2_app_frame_relay_initialize(_param);
            _listener = null;

            #region GDK Callback Registration
            _p2_on_connected = new G2FUN_LISTENER(on_connected);
            _p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            _p2_on_receive_request_site_product_info = new G2FUN_LISTENER(on_receive_request_site_product_info);

            register_callback(G2APP_FRAME_RELAY_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2APP_FRAME_RELAY_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2APP_FRAME_RELAY_CALLBACK.TYPE.on_receive_request_site_product_info, _p2_on_receive_request_site_product_info);
            #endregion
        }
        ~g2app_frame_relay()
        {
            g2_app_frame_relay_finalize(_handle);
        }

        private G2HANDLE _handle;
        private G2UPARAM _param;
        private g2app_frame_relay_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2APP_FRAME_RELAY_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_app_frame_relay_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_request_site_product_info;
        #endregion

        public G2HANDLE safe_handle() { return _handle; }

        public void startup(int connections)
        {
            g2_app_frame_relay_startup(_handle, connections, 0);
        }
        public void startup(int connections, uint send_queue_size_limit)
        {
            g2_app_frame_relay_startup(_handle, connections, send_queue_size_limit);
        }
        public void cleanup()
        {
            g2_app_frame_relay_cleanup(_handle);
        }
        public void set_listener(g2app_frame_relay_listener listener)
        {
            _listener = listener;
        }

        public int connect(ref G2NETWORK_INFO ni, out G2CONNECT_RES res)
        {
            return g2_app_frame_relay_connect(_handle, ref ni, out res);
        }
        public void disconnect(int channel)
        {
            g2_app_frame_relay_disconnect(_handle, channel);
        }
        public bool is_connecting(int channel)
        {
            return g2_app_frame_relay_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_app_frame_relay_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_app_frame_relay_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_app_frame_relay_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_app_frame_relay_is_disconnectable(_handle, channel);
        }

        public bool request_alive_check(int channel, int val)
        {
            return g2_app_frame_relay_request_alive_check(_handle, channel, val);
        }

        public bool send_site_connected(int channel, ref G2STRING_64 site)
        {
            return g2_app_frame_relay_send_site_connected(_handle, channel, ref site);
        }
        public bool send_site_disconnected(int channel, ref G2STRING_64 site, int reason)
        {
            return g2_app_frame_relay_send_site_disconnected(_handle, channel, ref site, reason);
        }
        public bool send_site_product_info(int channel, ref G2STRING_64 site, G2FUN_GET_ADAPTOR get_adaptor, G2HANDLE from_handle, int from_channel)
        {
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(get_adaptor);
            return g2_app_frame_relay_send_site_product_info(_handle, channel, ref site, fn, from_handle, from_channel);
        }
        public bool send_site_frame_data(int channel, ref G2STRING_64 site, ref G2FRAME frame)
        {
            return g2_app_frame_relay_send_site_frame_data(_handle, channel, ref site, ref frame);
        }

        public bool is_able_to_send(int channel, uint size)
        {
            return g2_app_frame_relay_is_able_to_send(_handle, channel, size);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2app_frame_relay_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2app_frame_relay_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)uparam);
            return 1;
        }
        private G2RESULT on_receive_request_site_product_info(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2app_frame_relay_receive_request_site_product_info(handle, (int)wparam);
            return 1;
        }
        #endregion
    }
}
