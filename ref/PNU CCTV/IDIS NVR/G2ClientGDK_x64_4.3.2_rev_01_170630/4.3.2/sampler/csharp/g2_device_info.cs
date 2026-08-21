using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
    using G2HDEVICE_INFO = System.Int32;
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

    public interface g2device_info_listener
    {
        void on_g2device_info_receive_product_info(G2HDEVICE_INFO handle, int channel, G2DISCONNECT_REASON.TYPE reason, ref G2_PRODUCT_INFO pi);
        void on_g2device_info_failed(G2HDEVICE_INFO handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2device_info_canceled(G2HDEVICE_INFO handle, int channel);
    }

    public class g2device_info
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_device_info_register_callback(G2HDEVICE_INFO handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HDEVICE_INFO g2_device_info_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_device_info_finalize(G2HDEVICE_INFO handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_device_info_startup(G2HDEVICE_INFO handle, int connections);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_device_info_cleanup(G2HDEVICE_INFO handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_device_info_connect(G2HDEVICE_INFO handle, ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_device_info_connect_ras(G2HDEVICE_INFO handle, ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_device_info_disconnect(G2HDEVICE_INFO handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_is_connecting(G2HDEVICE_INFO handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_is_connected(G2HDEVICE_INFO handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_is_disconnecting(G2HDEVICE_INFO handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_is_disconnected(G2HDEVICE_INFO handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_is_disconnectable(G2HDEVICE_INFO handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_get_product_info(G2HDEVICE_INFO handle, int channel, out G2_PRODUCT_INFO pi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_get_authority(G2HDEVICE_INFO handle, int channel, out G2RAS_AUTHORITY auth);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_is_authenticated(G2HDEVICE_INFO handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_is_IDR_X032(G2HDEVICE_INFO handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_device_info_is_IDR(G2HDEVICE_INFO handle, int channel);
        #endregion

        public g2device_info()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_receive_product_info = new G2FUN_LISTENER(on_receive_product_info);
            this._p2_on_failed = new G2FUN_LISTENER(on_failed);
            this._p2_on_canceled = new G2FUN_LISTENER(on_canceled);
        }
        ~g2device_info()
        {
            cleanup();
        }

        private G2HDEVICE_INFO _handle;
        private G2UPARAM _param;
        private g2device_info_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2DEVICE_INFO_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_device_info_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_receive_product_info;
        private G2FUN_LISTENER _p2_on_failed;
        private G2FUN_LISTENER _p2_on_canceled;
        #endregion

        public G2HDEVICE_INFO safe_handle()
        {
            return _handle;
        }

        public void startup(int connections)
        {
            cleanup();

            _handle = g2_device_info_initialize(_param);

            register_callback(G2DEVICE_INFO_CALLBACK.TYPE.on_receive_product_info, _p2_on_receive_product_info);
            register_callback(G2DEVICE_INFO_CALLBACK.TYPE.on_failed, _p2_on_failed);
            register_callback(G2DEVICE_INFO_CALLBACK.TYPE.on_canceled, _p2_on_canceled);

            g2_device_info_startup(_handle, connections);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HDEVICE_INFO handle = _handle; _handle = 0;
                g2_device_info_cleanup(handle);
                g2_device_info_finalize(handle);
            }
        }
        public void set_listener(g2device_info_listener listener)
        {
            _listener = listener;
        }

        public int connect(ref G2GUID root, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_device_info_connect(_handle, ref root, ref options, out res);
        }
        public int connect(ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_device_info_connect(_handle, ref root, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_device_info_connect_ras(_handle, ref ni, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_device_info_connect_ras(_handle, ref ni, ref options, out res);
        }
        public void disconnect(int channel)
        {
            g2_device_info_disconnect(_handle, channel);
        }

        public bool is_connecting(int channel)
        {
            return g2_device_info_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_device_info_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_device_info_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_device_info_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_device_info_is_disconnectable(_handle, channel);
        }

        public bool get_product_info(int channel, out G2_PRODUCT_INFO pi)
        {
            return g2_device_info_get_product_info(_handle, channel, out pi);
        }
        public bool get_authority(int channel, out G2RAS_AUTHORITY auth)
        {
            return g2_device_info_get_authority(_handle, channel, out auth);
        }

        public bool is_authenticated(int channel)
        {
            return g2_device_info_is_authenticated(_handle, channel);
        }
        public bool is_IDR_X032(int channel)
        {
            return g2_device_info_is_IDR_X032(_handle, channel);
        }
        public bool is_IDR(int channel)
        {
            return g2_device_info_is_IDR(_handle, channel);
        }

        private G2RESULT on_receive_product_info(G2HDEVICE_INFO handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2_PRODUCT_INFO pi = (G2_PRODUCT_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2_PRODUCT_INFO));
            _listener.on_g2device_info_receive_product_info(handle, (int)G2PARAM_.LOWORD(wparam), (G2DISCONNECT_REASON.TYPE)G2PARAM_.HIWORD(wparam), ref pi);
            return 1;
        }
        private G2RESULT on_failed(G2HDEVICE_INFO handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2device_info_failed(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_canceled(G2HDEVICE_INFO handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2device_info_canceled(handle, (int)wparam);
            return 1;
        }
    }
}
