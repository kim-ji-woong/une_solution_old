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

    public interface g2fen_listener
    {
        void on_g2fen_nat_type_discovered(G2NAT_INFO.TYPE type, ref G2NAT_INFO ni);
    }

    public class g2fen
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_fen_set_UPARAM(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_fen_register_callback(uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_fen_startup(uint connections, uint count_direct, uint count_hole_punching, uint count_relay, string path_history);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_fen_cleanup();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern void g2_fen_set_gateway(string address, ushort port);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_fen_set_secure_local();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_fen_set_external([MarshalAs(UnmanagedType.U1)] bool flag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern void g2_fen_set_external_proxy(string address, ushort port);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern void g2_fen_set_external_gateway(string address, ushort port);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_fen_set_external_nat(ref G2NAT_INFO ni);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_fen_set_external_activate([MarshalAs(UnmanagedType.U1)] bool activate);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_fen_service_join(uint elapse, [MarshalAs(UnmanagedType.U1)] bool pump_message_waitable);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_fen_get_nat_type();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_fen_is_startup();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_fen_is_activate();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_fen_is_failed_secure_nat_info();
        #endregion

        private g2fen()
        {
            _listener = null;

            #region GDK Callback Registration
            _p2_on_nat_type_discovered = new G2FUN_LISTENER(on_nat_type_discovered);

            register_callback(G2FEN_CALLBACK.TYPE.on_nat_type_discovered, _p2_on_nat_type_discovered);
            #endregion
        }
        ~g2fen()
        {

        }

        public static g2fen get()
        {
            return s_singleton;
        }

        public static g2fen s_singleton = new g2fen();
        private g2fen_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2FEN_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_fen_register_callback((uint)id, fn); }
        private G2FUN_LISTENER _p2_on_nat_type_discovered;
        #endregion

        public void startup(uint connections, uint count_direct, uint count_hole_punching, uint count_relay, string path_history)
        {
            g2_fen_startup(connections, count_direct, count_hole_punching, count_relay, path_history);
        }
        public void cleanup()
        {
            g2_fen_cleanup();
        }
        public void set_listener(g2fen_listener listener)
        {
            _listener = listener;
        }
        public void set_gateway(string address, ushort port)
        {
            g2_fen_set_gateway(address, port);
        }
        public void set_secure_local()
        {
            g2_fen_set_secure_local();
        }
        public void set_external(bool flag)
        {
            g2_fen_set_external(flag);
        }
        public void set_external_proxy(string address, ushort port)
        {
            g2_fen_set_external_proxy(address, port);
        }
        public void set_external_gateway(string address, ushort port)
        {
            g2_fen_set_external_gateway(address, port);
        }
        public void set_external_nat(ref G2NAT_INFO ni)
        {
            g2_fen_set_external_nat(ref ni);
        }
        public void set_external_activate(bool activate)
        {
            g2_fen_set_external_activate(activate);
        }
        public void join_service(uint elapse, bool pump_message_waitable)
        {
            g2_fen_service_join(elapse, pump_message_waitable);
        }

        public int get_nat_type()
        {
            return g2_fen_get_nat_type();
        }

        public bool is_startup()
        {
            return g2_fen_is_startup();
        }
        public bool is_activate()
        {
            return g2_fen_is_activate();
        }
        public bool is_failed_secure_nat_info()
        {
            return g2_fen_is_failed_secure_nat_info();
        }

        #region GDK Callback Handler
        private G2RESULT on_nat_type_discovered(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2NAT_INFO p = (G2NAT_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2NAT_INFO));
            if (_listener != null)
            {
                _listener.on_g2fen_nat_type_discovered(p.type, ref p);
            }
            return 1;
        }
        #endregion
    }
}
