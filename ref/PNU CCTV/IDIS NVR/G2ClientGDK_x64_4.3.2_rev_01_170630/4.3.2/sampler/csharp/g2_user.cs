using System;
using System.Collections.Generic;
using System.IO;
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

    public interface g2user_listener
    {
        void on_g2user_connected();
        void on_g2user_disconnect_entered(G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user);
        void on_g2user_login_cancelled();
        void on_g2user_login();
        void on_g2user_logout(G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user);
        void on_g2user_login_failed(ref G2NETWORK_INFO ni, G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user);
        void on_g2user_login_failed_from_dvrns(ref G2NETWORK_INFO ni, G2FEN_RESULT.TYPE reason);
        void on_g2user_set_authority();
        void on_g2user_modify();
    }

    public class g2user
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_user_set_UPARAM(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_user_register_callback(uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_is_connected();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_is_login();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_is_logout();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_is_administrator();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_is_authority(int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_is_federation();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_get_network_info(out G2NETWORK_INFO ni);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_get_id(out G2STRING_32 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_get_name(out G2STRING_64 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_user_get_description(out G2STRING_128 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_user_get_string_authority(int type, out G2STRING_128 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_user_get_string_authority_desc(int type, out G2STRING_256 str);
        #endregion

        private g2user()
        {
            _param = new G2UPARAM(0);
            _listener = null;
            g2_user_set_UPARAM(_param);

            #region GDK Callback Registration
            _p2_on_connected = new G2FUN_LISTENER(on_connected);
            _p2_on_disconnect_entered = new G2FUN_LISTENER(on_disconnect_entered);
            _p2_on_login_cancelled = new G2FUN_LISTENER(on_login_cancelled);
            _p2_on_login = new G2FUN_LISTENER(on_login);
            _p2_on_logout = new G2FUN_LISTENER(on_logout);
            _p2_on_login_failed = new G2FUN_LISTENER(on_login_failed);
            _p2_on_login_failed_from_dvrns = new G2FUN_LISTENER(on_login_failed_from_dvrns);
            _p2_on_set_authority = new G2FUN_LISTENER(on_set_authority);
            _p2_on_modify = new G2FUN_LISTENER(on_modify);

            register_callback(G2USER_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2USER_CALLBACK.TYPE.on_disconnect_entered, _p2_on_disconnect_entered);
            register_callback(G2USER_CALLBACK.TYPE.on_login_cancelled, _p2_on_login_cancelled);
            register_callback(G2USER_CALLBACK.TYPE.on_login, _p2_on_login);
            register_callback(G2USER_CALLBACK.TYPE.on_logout, _p2_on_logout);
            register_callback(G2USER_CALLBACK.TYPE.on_login_failed, _p2_on_login_failed);
            register_callback(G2USER_CALLBACK.TYPE.on_login_failed_from_dvrns, _p2_on_login_failed_from_dvrns);
            register_callback(G2USER_CALLBACK.TYPE.on_set_authority, _p2_on_set_authority);
            register_callback(G2USER_CALLBACK.TYPE.on_modify, _p2_on_modify);
            #endregion
        }
        ~g2user()
        {

        }

        public static g2user get()
        {
            return s_singleton;
        }

        public static g2user s_singleton = new g2user();
        private G2UPARAM _param;
        private g2user_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2USER_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_user_register_callback((uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnect_entered;
        private G2FUN_LISTENER _p2_on_login_cancelled;
        private G2FUN_LISTENER _p2_on_login;
        private G2FUN_LISTENER _p2_on_logout;
        private G2FUN_LISTENER _p2_on_login_failed;
        private G2FUN_LISTENER _p2_on_login_failed_from_dvrns;
        private G2FUN_LISTENER _p2_on_set_authority;
        private G2FUN_LISTENER _p2_on_modify;
        #endregion

        public void set_listener(g2user_listener listener)
        {
            _listener = listener;
        }
        public bool is_connected()
        {
            return g2_user_is_connected();
        }
        public bool is_login()
        {
            return g2_user_is_login();
        }
        public bool is_logout()
        {
            return g2_user_is_logout();
        }
        public bool is_administrator()
        {
            return g2_user_is_administrator();
        }
        public bool is_authority(G2USER_AUTHORITY.TYPE type)
        {
            return g2_user_is_authority((int)type);
        }
        public bool is_federation()
        {
            return g2_user_is_federation();
        }

        public bool get_network_info(out G2NETWORK_INFO ni)
        {
            return g2_user_get_network_info(out ni);
        }
        public string get_id()
        {
            G2STRING_32 s;
            g2_user_get_id(out s);
            return s;
        }
        public string get_name()
        {
            G2STRING_64 s;
            g2_user_get_name(out s);
            return s;
        }
        public string get_description()
        {
            G2STRING_64 s;
            g2_user_get_name(out s);
            return s;
        }
        public bool get_id(out string id)
        {
            G2STRING_32 s;
            bool res = g2_user_get_id(out s);
            id = s;
            return res;
        }
        public bool get_name(out string name)
        {
            G2STRING_64 s;
            bool res = g2_user_get_name(out s);
            name = s;
            return res;
        }
        public bool get_description(out string desc)
        {
            G2STRING_128 s;
            bool res = g2_user_get_description(out s);
            desc = s;
            return res;
        }
        public static string get_string_authority(G2USER_AUTHORITY.TYPE type)
        {
            G2STRING_128 s;
            g2_user_get_string_authority((int)type, out s);
            return s;
        }
        public static string get_string_authority_desc(G2USER_AUTHORITY.TYPE type)
        {
            G2STRING_256 s;
            g2_user_get_string_authority_desc((int)type, out s);
            return s;
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2RESULT res = 0;
            if (_listener != null)
            {
                _listener.on_g2user_connected();
                res = 1;
            }
            return res;
        }
        private G2RESULT on_disconnect_entered(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener != null)
            {
                _listener.on_g2user_disconnect_entered((G2DISCONNECT_REASON.TYPE)wparam, (G2SERVICE_LOGIN_FAIL_REASON.TYPE)lparam);
            }
            return 1;
        }
        private G2RESULT on_login_cancelled(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2user_login_cancelled();
            return 1;
        }
        private G2RESULT on_login(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2user_login();
            return 1;
        }
        private G2RESULT on_logout(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2user_logout((G2DISCONNECT_REASON.TYPE)wparam, (G2SERVICE_LOGIN_FAIL_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_login_failed(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2NETWORK_INFO ni = (G2NETWORK_INFO)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2NETWORK_INFO));
            G2LPARAM_LIST list = (G2LPARAM_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LPARAM_LIST));
            IntPtr[] p = new IntPtr[list._len];
            Marshal.Copy(list._params, p, 0, p.Length);
            _listener.on_g2user_login_failed(ref ni, (G2DISCONNECT_REASON.TYPE)p[0], (G2SERVICE_LOGIN_FAIL_REASON.TYPE)p[1]);
            return 1;
        }
        private G2RESULT on_login_failed_from_dvrns(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2NETWORK_INFO ni = (G2NETWORK_INFO)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2NETWORK_INFO));
            _listener.on_g2user_login_failed_from_dvrns(ref ni, (G2FEN_RESULT.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_set_authority(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2user_set_authority();
            return 1;
        }
        private G2RESULT on_modify(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2user_modify();
            return 1;
        }
        #endregion
    }
}
