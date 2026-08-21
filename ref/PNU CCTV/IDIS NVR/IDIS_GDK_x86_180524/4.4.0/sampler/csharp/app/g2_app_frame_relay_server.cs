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

    public interface g2app_frame_relay_server_listener
    {
        void on_g2app_frame_relay_server_connected(G2HANDLE handle, int channel);
        void on_g2app_frame_relay_server_disconnected(G2HANDLE handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2app_frame_relay_server_receive_site_connected(G2HANDLE handle, int channel, ref G2STRING_64 site);
        void on_g2app_frame_relay_server_receive_site_disconnected(G2HANDLE handle, int channel, ref G2STRING_64 site, int reason);
        void on_g2app_frame_relay_server_receive_site_product_info(G2HANDLE handle, int channel, ref G2STRING_64 site, ref G2_PRODUCT_INFO pi);
        void on_g2app_frame_relay_server_receive_site_frame_data(G2HANDLE handle, int channel, ref G2STRING_64 site, ref G2FRAME frame);
    }

    public class g2app_frame_relay_server
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_server_register_callback(G2HANDLE handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HANDLE g2_app_frame_relay_server_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_server_finalize(G2HANDLE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_server_startup(G2HANDLE handle, ref G2APP_FRAME_RELAY_SERVER_OPTION option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_server_cleanup(G2HANDLE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_app_frame_relay_server_disconnect(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_server_is_connected(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_server_is_disconnecting(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_server_is_disconnected(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_server_is_disconnectable(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_server_request_site_product_info(G2HANDLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_server_is_usable_TCP_port(ushort port);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_server_search_controller_option_make_file(string path, ref G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_app_frame_relay_server_search_controller_run(string application, string option_file);
        #endregion

        public g2app_frame_relay_server()
        {
            _param = new G2UPARAM(0);
            _handle = g2_app_frame_relay_server_initialize(_param);
            _listener = null;

            #region GDK Callback Registration
            _p2_on_connected = new G2FUN_LISTENER(on_connected);
            _p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            _p2_on_receive_site_connected = new G2FUN_LISTENER(on_receive_site_connected);
            _p2_on_receive_site_disconnected = new G2FUN_LISTENER(on_receive_site_disconnected);
            _p2_on_receive_site_product_info = new G2FUN_LISTENER(on_receive_site_product_info);
            _p2_on_receive_site_frame_data = new G2FUN_LISTENER(on_receive_site_frame_data);

            register_callback(G2APP_FRAME_RELAY_SERVER_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2APP_FRAME_RELAY_SERVER_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2APP_FRAME_RELAY_SERVER_CALLBACK.TYPE.on_receive_site_connected, _p2_on_receive_site_connected);
            register_callback(G2APP_FRAME_RELAY_SERVER_CALLBACK.TYPE.on_receive_site_disconnected, _p2_on_receive_site_disconnected);
            register_callback(G2APP_FRAME_RELAY_SERVER_CALLBACK.TYPE.on_receive_site_product_info, _p2_on_receive_site_product_info);
            register_callback(G2APP_FRAME_RELAY_SERVER_CALLBACK.TYPE.on_receive_site_frame_data, _p2_on_receive_site_frame_data);
            #endregion
        }
        ~g2app_frame_relay_server()
        {
            g2_app_frame_relay_server_finalize(_handle);
        }

        private G2HANDLE _handle;
        private G2UPARAM _param;
        private g2app_frame_relay_server_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2APP_FRAME_RELAY_SERVER_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_app_frame_relay_server_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_site_connected;
        private G2FUN_LISTENER _p2_on_receive_site_disconnected;
        private G2FUN_LISTENER _p2_on_receive_site_product_info;
        private G2FUN_LISTENER _p2_on_receive_site_frame_data;
        #endregion

        public G2HANDLE safe_handle() { return _handle; }

        public void startup(ref G2APP_FRAME_RELAY_SERVER_OPTION option)
        {
            g2_app_frame_relay_server_startup(_handle, ref option);
        }
        public void cleanup()
        {
            g2_app_frame_relay_server_cleanup(_handle);
        }
        public void set_listener(g2app_frame_relay_server_listener listener)
        {
            _listener = listener;
        }

        public void disconnect(int channel)
        {
            g2_app_frame_relay_server_disconnect(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_app_frame_relay_server_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_app_frame_relay_server_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_app_frame_relay_server_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_app_frame_relay_server_is_disconnectable(_handle, channel);
        }

        public bool request_site_product_info(int channel)
        {
            return g2_app_frame_relay_server_request_site_product_info(_handle, channel);
        }

        public static bool is_usable_TCP_port(ushort port)
        {
            return g2_app_frame_relay_server_is_usable_TCP_port(port);
        }
        public static bool search_controller_option_make_file(string path, ref G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION option)
        {
            return g2_app_frame_relay_server_search_controller_option_make_file(path, ref option);
        }
        public static bool search_controller_run(string application, string option_file)
        {
            return g2_app_frame_relay_server_search_controller_run(application, option_file);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2app_frame_relay_server_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2app_frame_relay_server_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_site_connected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2STRING_64 site = (G2STRING_64)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2STRING_64));
            _listener.on_g2app_frame_relay_server_receive_site_connected(handle, (int)wparam, ref site);
            return 1;
        }
        private G2RESULT on_receive_site_disconnected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LPARAM_LIST list = (G2LPARAM_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LPARAM_LIST));
            IntPtr[] p = new IntPtr[list._len];
            Marshal.Copy(list._params, p, 0, p.Length);
            G2STRING_64 site = (G2STRING_64)Marshal.PtrToStructure(p[0], typeof(G2STRING_64));
            int reason = (int)p[1];
            _listener.on_g2app_frame_relay_server_receive_site_disconnected(handle, (int)wparam, ref site, reason);
            return 1;
        }
        private G2RESULT on_receive_site_product_info(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LPARAM_LIST list = (G2LPARAM_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LPARAM_LIST));
            IntPtr[] p = new IntPtr[list._len];
            Marshal.Copy(list._params, p, 0, p.Length);
            G2STRING_64 site = (G2STRING_64)Marshal.PtrToStructure(p[0], typeof(G2STRING_64));
            G2_PRODUCT_INFO pi = (G2_PRODUCT_INFO)Marshal.PtrToStructure(p[1], typeof(G2_PRODUCT_INFO));
            _listener.on_g2app_frame_relay_server_receive_site_product_info(handle, (int)wparam, ref site, ref pi);
            return 1;
        }
        private G2RESULT on_receive_site_frame_data(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LPARAM_LIST list = (G2LPARAM_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LPARAM_LIST));
            IntPtr[] p = new IntPtr[list._len];
            Marshal.Copy(list._params, p, 0, p.Length);
            G2STRING_64 site = (G2STRING_64)Marshal.PtrToStructure(p[0], typeof(G2STRING_64));
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure(p[1], typeof(G2FRAME));
            _listener.on_g2app_frame_relay_server_receive_site_frame_data(handle, (int)wparam, ref site, ref frame);
            return 1;
        }
        #endregion
    }
}
