using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace GDK
{
    using G2HVIOLET = System.Int32;
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

    public interface g2violet_listener
    {
        void on_g2violet_connected(G2HVIOLET handle, int channel);
        void on_g2violet_disconnected(G2HVIOLET handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2violet_receive_service_log_load(G2HVIOLET handle, int channel, ref G2SYSTEM_LOG log);
        void on_g2violet_receive_service_log_load_end(G2HVIOLET handle, int channel);
        void on_g2violet_receive_service_log_load_fail(G2HVIOLET handle, int channel);
        void on_g2violet_receive_service_log_load_stop(G2HVIOLET handle, int channel);
        void on_g2violet_receive_agent_audio_connected(G2HVIOLET handle, int channel, int monitor);
        void on_g2violet_receive_agent_audio_disconnected(G2HVIOLET handle, int channel, int monitor);
        void on_g2violet_receive_agent_layout_attached(G2HVIOLET handle, int channel, ref G2GUID agent, int monitor, ref G2GUID layout);
        void on_g2violet_receive_cameras_on_monitor(G2HVIOLET handle, int channel, ref G2GUID agent, int monitor, G2GUID[] cameras);
    }

    public class g2violet
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_violet_register_callback(G2HVIOLET handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HVIOLET g2_violet_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_violet_finalize(G2HVIOLET handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_violet_startup(G2HVIOLET handle, int connections);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_violet_cleanup(G2HVIOLET handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_violet_connect(G2HVIOLET handle, ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_violet_disconnect(G2HVIOLET handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_is_connecting(G2HVIOLET handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_is_connected(G2HVIOLET handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_is_disconnecting(G2HVIOLET handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_is_disconnected(G2HVIOLET handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_is_disconnectable(G2HVIOLET handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_request_set_layout_monitor_live(G2HVIOLET handle, int channel, int monitor, ref G2GUID layout);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_request_set_screen_format(G2HVIOLET handle, int channel, int monitor, int format);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_request_set_screen_camera_format(G2HVIOLET handle, int channel, int monitor, int camera, int format);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_request_get_cameras_on_monitor(G2HVIOLET handle, int channel, int monitor);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_request_system_log_search(G2HVIOLET handle, int channel, ref G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_request_system_log_search_stop(G2HVIOLET handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_stream_channel_control_via_id(G2HVIOLET handle, int channel, int seq, int id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_stream_channel_control_ptz_joystick(G2HVIOLET handle, int channel, int seq, int id, int speed);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_stream_channel_control(G2HVIOLET handle, int channel, int seq, int len, IntPtr buf);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_mouse_input(G2HVIOLET handle, int channel, ref G2VIOLET_MOUSE_INPUT mi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_monitor_message(G2HVIOLET handle, int channel, int monitor, string message, uint time_out, ushort size, int position);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_monitor_message_hide(G2HVIOLET handle, int channel, int monitor);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_screen_message(G2HVIOLET handle, int channel, int monitor, ref G2VIOLET_MONITOR_MESSAGE message);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_screen_message_hide(G2HVIOLET handle, int channel, int monitor);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_agent_audio_connect(G2HVIOLET handle, int channel, int monitor, string address, ushort port, string id, string password, [MarshalAs(UnmanagedType.U1)] bool encrypted);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_violet_send_agent_audio_disconnect(G2HVIOLET handle, int channel, int monitor);
        #endregion

        public g2violet()
        {
            this._handle = g2_violet_initialize(_param);
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_receive_service_log_load = new G2FUN_LISTENER(on_receive_service_log_load);
            this._p2_on_receive_service_log_load_end = new G2FUN_LISTENER(on_receive_service_log_load_end);
            this._p2_on_receive_service_log_load_fail = new G2FUN_LISTENER(on_receive_service_log_load_fail);
            this._p2_on_receive_service_log_load_stop = new G2FUN_LISTENER(on_receive_service_log_load_stop);
            this._p2_on_receive_agent_audio_connected = new G2FUN_LISTENER(on_receive_agent_audio_connected);
            this._p2_on_receive_agent_audio_disconnected = new G2FUN_LISTENER(on_receive_agent_audio_disconnected);
            this._p2_on_receive_agent_layout_attached = new G2FUN_LISTENER(on_receive_agent_layout_attached);
            this._p2_on_receive_cameras_on_monitor = new G2FUN_LISTENER(on_receive_cameras_on_monitor);
        }
        ~g2violet()
        {
            cleanup();
        }

        private G2HVIOLET _handle;
        private G2UPARAM _param;
        private g2violet_listener _listener;

        #region GDK Callback Deleagte
        private void register_callback(G2VIOLET_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_violet_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_service_log_load;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_agent_audio_connected;
        private G2FUN_LISTENER _p2_on_receive_agent_audio_disconnected;
        private G2FUN_LISTENER _p2_on_receive_agent_layout_attached;
        private G2FUN_LISTENER _p2_on_receive_cameras_on_monitor;
        #endregion

        public G2HVIOLET safe_handle() { return _handle; }

        public void startup(int connections)
        {
            cleanup();

            _handle = g2_violet_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2VIOLET_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2VIOLET_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2VIOLET_CALLBACK.TYPE.on_receive_service_log_load, _p2_on_receive_service_log_load);
            register_callback(G2VIOLET_CALLBACK.TYPE.on_receive_service_log_load_end, _p2_on_receive_service_log_load_end);
            register_callback(G2VIOLET_CALLBACK.TYPE.on_receive_service_log_load_fail, _p2_on_receive_service_log_load_fail);
            register_callback(G2VIOLET_CALLBACK.TYPE.on_receive_service_log_load_stop, _p2_on_receive_service_log_load_stop);
            register_callback(G2VIOLET_CALLBACK.TYPE.on_receive_agent_audio_connected, _p2_on_receive_agent_audio_connected);
            register_callback(G2VIOLET_CALLBACK.TYPE.on_receive_agent_audio_disconnected, _p2_on_receive_agent_audio_disconnected);
            register_callback(G2VIOLET_CALLBACK.TYPE.on_receive_agent_layout_attached, _p2_on_receive_agent_layout_attached);
            register_callback(G2VIOLET_CALLBACK.TYPE.on_receive_cameras_on_monitor, _p2_on_receive_cameras_on_monitor);
            #endregion

            g2_violet_startup(_handle, connections);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HVIOLET handle = _handle; _handle = 0;
                g2_violet_cleanup(handle);
                g2_violet_finalize(handle);
            }
        }
        public void set_listener(g2violet_listener listener)
        {
            _listener = listener;
        }

        public int connect(ref G2GUID service, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_violet_connect(_handle, ref service, ref options, out res);
        }
        public int connect(ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_violet_connect(_handle, ref service, ref options, out res);
        }
        public void disconnect(int channel)
        {
            g2_violet_disconnect(_handle, channel);
        }
        public bool is_connecting(int channel)
        {
            return g2_violet_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_violet_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_violet_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_violet_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_violet_is_disconnectable(_handle, channel);
        }

        public bool request_set_layout_monitor_live(int channel, int monitor, ref G2GUID layout)
        {
            return g2_violet_request_set_layout_monitor_live(_handle, channel, monitor, ref layout);
        }
        public bool request_set_screen_format(int channel, int monitor, int format)
        {
            return g2_violet_request_set_screen_format(_handle, channel, monitor, format);
        }
        public bool request_set_screen_camera_format(int channel, int monitor, int camera, int format)
        {
            return g2_violet_request_set_screen_camera_format(_handle, channel, monitor, camera, format);
        }
        public bool request_get_cameras_on_monitor(int channel, int monitor)
        {
            return g2_violet_request_get_cameras_on_monitor(_handle, channel, monitor);
        }
        public bool request_system_log_search(int channel, ref G2SERVICE_SEARCH_OPTION_SYSTEM_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_violet_request_system_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }
        public bool request_system_log_search_stop(int channel)
        {
            return g2_violet_request_system_log_search_stop(_handle, channel);
        }

        public bool send_stream_channel_control_via_id(int channel, int seq, int id)
        {
            return g2_violet_send_stream_channel_control_via_id(_handle, channel, seq, id);
        }
        public bool send_stream_channel_control_ptz_joystick(int channel, int seq, int id, int speed)
        {
            return g2_violet_send_stream_channel_control_ptz_joystick(_handle, channel, seq, id, speed);
        }
        public bool send_stream_channel_control(int channel, int seq, int len, IntPtr buf)
        {
            return g2_violet_send_stream_channel_control(_handle, channel, seq, len, buf);
        }
        public bool send_mouse_input(int channel, ref G2VIOLET_MOUSE_INPUT mi)
        {
            return g2_violet_send_mouse_input(_handle, channel, ref mi);
        }
        public bool send_monitor_message(int channel, int monitor, string message, uint time_out, ushort size, int position)
        {
            return g2_violet_send_monitor_message(_handle, channel, monitor, message, time_out, size, position);
        }
        public bool send_monitor_message_hide(int channel, int monitor)
        {
            return g2_violet_send_monitor_message_hide(_handle, channel, monitor);
        }
        public bool send_screen_message(int channel, int monitor, ref G2VIOLET_MONITOR_MESSAGE message)
        {
            return g2_violet_send_screen_message(_handle, channel, monitor, ref message);
        }
        public bool send_screen_message_hide(int channel, int monitor)
        {
            return g2_violet_send_screen_message_hide(_handle, channel, monitor);
        }
        public bool send_agent_audio_connect(int channel, int monitor, string address, ushort port, string id, string password, bool encrypted)
        {
            return g2_violet_send_agent_audio_connect(_handle, channel, monitor, address, port, id, password, encrypted);
        }
        public bool send_agent_audio_disconnect(int channel, int monitor)
        {
            return g2_violet_send_agent_audio_disconnect(_handle, channel, monitor);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2violet_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2violet_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SYSTEM_LOG log = (G2SYSTEM_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SYSTEM_LOG));
            _listener.on_g2violet_receive_service_log_load(handle, (int)wparam, ref log);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_end(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2violet_receive_service_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_fail(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2violet_receive_service_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_stop(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2violet_receive_service_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_agent_audio_connected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2violet_receive_agent_audio_connected(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_receive_agent_audio_disconnected(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2violet_receive_agent_audio_disconnected(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_receive_agent_layout_attached(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2VIOLET_PARAM_LAYOUT data = (G2VIOLET_PARAM_LAYOUT)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2VIOLET_PARAM_LAYOUT));
            _listener.on_g2violet_receive_agent_layout_attached(handle, (int)wparam, ref data._agent, data._monitor, ref data._layout);
            return 1;
        }
        private G2RESULT on_receive_cameras_on_monitor(G2HVIOLET handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2VIOLET_PARAM_CAMERASET data = (G2VIOLET_PARAM_CAMERASET)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2VIOLET_PARAM_CAMERASET));
            G2GUID[] guids = new G2GUID[data._camera_GUIDs._len];
            for (int i = 0; i < data._camera_GUIDs._len; ++i)
            {
                IntPtr ptr = new IntPtr(data._camera_GUIDs._guid.ToInt64() + i * Marshal.SizeOf(typeof(G2GUID)));
                guids[i] = (G2GUID)Marshal.PtrToStructure(ptr, typeof(G2GUID));
            }
            _listener.on_g2violet_receive_cameras_on_monitor(handle, (int)wparam, ref data._agent, data._monitor, guids);
            return 1;
        }
        #endregion
    }
}
