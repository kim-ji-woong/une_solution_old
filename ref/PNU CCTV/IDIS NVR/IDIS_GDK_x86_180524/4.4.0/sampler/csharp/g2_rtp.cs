using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HRTP = System.Int32;
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

    public interface g2rtp_listener
    {
        bool on_g2rtp_rtsp_connected_device(G2HRTP handle, int channel);
        bool on_g2rtp_rtsp_connected(G2HRTP handle, int channel);
        void on_g2rtp_rtsp_disconnected(G2HRTP handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2rtp_rtp_connected(G2HRTP handle, int channel);
        void on_g2rtp_receive_frame_data(G2HRTP handle, int channel, ref G2FRAME frame);
        void on_g2rtp_receive_event(G2HRTP handle, int channel, ref G2EVENT_INFO info);
        void on_g2rtp_receive_device_status(G2HRTP handle, int channel, ref G2DEVICE_STATUS status);
        void on_g2rtp_receive_ptz_preset(G2HRTP handle, int channel, ref G2LIVE_PTZ_PRESET preset);
        void on_g2rtp_receive_ptz_menu(G2HRTP handle, int channel, ref G2LIVE_PTZ_MENU menu);
        void on_g2rtp_receive_audio_out_not_available(G2HRTP handle, int channel);
        void on_g2rtp_audio_streaming_started(G2HRTP handle, int channel, int camera);
        void on_g2rtp_audio_streaming_stopped(G2HRTP handle, int channel, int camera);
        void on_g2rtp_audio_capturing_started(G2HRTP handle, int channel, int camera);
        void on_g2rtp_audio_capturing_stopped(G2HRTP handle, int channel, int camera);
        void on_g2rtp_probe_session_profile(G2HRTP handle, int channel, ref G2PROBE_SESSION_PROFILE probe);
    }

    public class g2rtp
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_rtp_register_callback(G2HRTP handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HRTP g2_rtp_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_rtp_finalize(G2HRTP handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_rtp_startup(G2HRTP handle, int connections, ushort port_range_min, ushort port_range_max, uint count_buffering, IntPtr focus);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_rtp_cleanup(G2HRTP handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_rtp_connect(G2HRTP handle, ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_rtp_disconnect(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_connecting(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_connected(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_disconnecting(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_disconnected(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_disconnectable(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_rtp_set_incomming_port(G2HRTP handle, ushort port_range_min, ushort port_range_max);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_rtp_set_buffering_count(G2HRTP handle, int count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_camera_channelset(G2HRTP handle, int channel, ref G2CHANNEL_SET channels, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_camera_list(G2HRTP handle, int channel, uint cameras, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_audio_channelset(G2HRTP handle, int channel, ref G2CHANNEL_SET channels, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_audio_list(G2HRTP handle, int channel, uint audios, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_stream_id(G2HRTP handle, int channel, int camera, int id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_alarm_out(G2HRTP handle, int channel, int id, [MarshalAs(UnmanagedType.U1)] bool on);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_camera_color(G2HRTP handle, int channel, int camera, int type, int value);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_ptz_command(G2HRTP handle, int channel, int camera, ref G2LIVE_PTZ_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_ptz_preset(G2HRTP handle, int channel, int camera, ref G2LIVE_PTZ_PRESET preset);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_enable_audio_streaming(G2HRTP handle, int channel, int camera, [MarshalAs(UnmanagedType.U1)] bool enable);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_enable_audio_capturing(G2HRTP handle, int channel, int camera, [MarshalAs(UnmanagedType.U1)] bool enable);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_disable_audio_streaming(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_disable_audio_capturing(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_set_disable_audio(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_rtp_set_probe_session_profile(G2HRTP handle, [MarshalAs(UnmanagedType.U1)] bool active);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_request_alive_check(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_request_ptz_menu(G2HRTP handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_request_ptz_preset(G2HRTP handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_get_camera_channelset(G2HRTP handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_get_camera_list(G2HRTP handle, int channel, out uint cameras);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_get_audio_channelset(G2HRTP handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_get_audio_list(G2HRTP handle, int channel, out uint audios);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_rtp_get_stream_id(G2HRTP handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern uint g2_rtp_get_stream_remote(G2HRTP handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_get_status(G2HRTP handle, int channel, out G2DEVICE_STATUS status);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_enable_multi_stream(G2HRTP handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_enable_audio_in(G2HRTP handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_enable_audio_out(G2HRTP handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_contains_audio_streaming(G2HRTP handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_contains_audio_capturing(G2HRTP handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_contains_audio(G2HRTP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_rtp_is_audio_out_opening(G2HRTP handle, int channel, int camera);
        #endregion

        public g2rtp()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_rtsp_connected_device = new G2FUN_LISTENER(on_rtsp_connected_device);
            this._p2_on_rtsp_connected = new G2FUN_LISTENER(on_rtsp_connected);
            this._p2_on_rtsp_disconnected = new G2FUN_LISTENER(on_rtsp_disconnected);
            this._p2_on_rtp_connected = new G2FUN_LISTENER(on_rtp_connected);
            this._p2_on_receive_frame_data = new G2FUN_LISTENER(on_receive_frame_data);
            this._p2_on_receive_event = new G2FUN_LISTENER(on_receive_event);
            this._p2_on_receive_device_status = new G2FUN_LISTENER(on_receive_device_status);
            this._p2_on_receive_ptz_preset = new G2FUN_LISTENER(on_receive_ptz_preset);
            this._p2_on_receive_ptz_menu = new G2FUN_LISTENER(on_receive_ptz_menu);
            this._p2_on_receive_audio_out_not_available = new G2FUN_LISTENER(on_receive_audio_out_not_available);
            this._p2_on_audio_streaming_started = new G2FUN_LISTENER(on_audio_streaming_started);
            this._p2_on_audio_streaming_stopped = new G2FUN_LISTENER(on_audio_streaming_stopped);
            this._p2_on_audio_capturing_started = new G2FUN_LISTENER(on_audio_capturing_started);
            this._p2_on_audio_capturing_stopped = new G2FUN_LISTENER(on_audio_capturing_stopped);
            this._p2_on_probe_session_profile = new G2FUN_LISTENER(on_probe_session_profile);
        }
        ~g2rtp()
        {
            cleanup();
        }

        private G2HRTP _handle;
        private G2UPARAM _param;
        private g2rtp_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2RTP_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_rtp_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_rtsp_connected_device;
        private G2FUN_LISTENER _p2_on_rtsp_connected;
        private G2FUN_LISTENER _p2_on_rtsp_disconnected;
        private G2FUN_LISTENER _p2_on_rtp_connected;
        private G2FUN_LISTENER _p2_on_receive_frame_data;
        private G2FUN_LISTENER _p2_on_receive_event;
        private G2FUN_LISTENER _p2_on_receive_device_status;
        private G2FUN_LISTENER _p2_on_receive_ptz_preset;
        private G2FUN_LISTENER _p2_on_receive_ptz_menu;
        private G2FUN_LISTENER _p2_on_receive_audio_out_not_available;
        private G2FUN_LISTENER _p2_on_audio_streaming_started;
        private G2FUN_LISTENER _p2_on_audio_streaming_stopped;
        private G2FUN_LISTENER _p2_on_audio_capturing_started;
        private G2FUN_LISTENER _p2_on_audio_capturing_stopped;
        private G2FUN_LISTENER _p2_on_probe_session_profile;
        #endregion

        public G2HRTP safe_handle() { return _handle; }

        public void startup(int connections, ushort port_range_min, ushort port_range_max, uint count_buffering, IntPtr focus)
        {
            cleanup();

            _handle = g2_rtp_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2RTP_CALLBACK.TYPE.on_rtsp_connected_device, _p2_on_rtsp_connected_device);
            register_callback(G2RTP_CALLBACK.TYPE.on_rtsp_connected, _p2_on_rtsp_connected);
            register_callback(G2RTP_CALLBACK.TYPE.on_rtsp_disconnected, _p2_on_rtsp_disconnected);
            register_callback(G2RTP_CALLBACK.TYPE.on_rtp_connected, _p2_on_rtp_connected);
            register_callback(G2RTP_CALLBACK.TYPE.on_receive_frame_data, _p2_on_receive_frame_data);
            register_callback(G2RTP_CALLBACK.TYPE.on_receive_event, _p2_on_receive_event);
            register_callback(G2RTP_CALLBACK.TYPE.on_receive_device_status, _p2_on_receive_device_status);
            register_callback(G2RTP_CALLBACK.TYPE.on_receive_ptz_preset, _p2_on_receive_ptz_preset);
            register_callback(G2RTP_CALLBACK.TYPE.on_receive_ptz_menu, _p2_on_receive_ptz_menu);
            register_callback(G2RTP_CALLBACK.TYPE.on_receive_audio_out_not_available, _p2_on_receive_audio_out_not_available);
            register_callback(G2RTP_CALLBACK.TYPE.on_audio_streaming_started, _p2_on_audio_streaming_started);
            register_callback(G2RTP_CALLBACK.TYPE.on_audio_streaming_stopped, _p2_on_audio_streaming_stopped);
            register_callback(G2RTP_CALLBACK.TYPE.on_audio_capturing_started, _p2_on_audio_capturing_started);
            register_callback(G2RTP_CALLBACK.TYPE.on_audio_capturing_stopped, _p2_on_audio_capturing_stopped);
            register_callback(G2RTP_CALLBACK.TYPE.on_probe_session_profile, _p2_on_probe_session_profile);
            #endregion

            g2_rtp_startup(_handle, connections, port_range_min, port_range_max, count_buffering, focus);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HRTP handle = _handle; _handle = 0;
                g2_rtp_cleanup(handle);
                g2_rtp_finalize(handle);
            }
        }
        public void set_listener(g2rtp_listener listener)
        {
            _listener = listener;
        }

        public int connect(ref G2GUID root, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_rtp_connect(_handle, ref root, ref options, out res);
        }
        public int connect(ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_rtp_connect(_handle, ref root, ref options, out res);
        }
        public void disconnect(int channel)
        {
            g2_rtp_disconnect(_handle, channel);
        }
        public bool is_connecting(int channel)
        {
            return g2_rtp_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_rtp_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_rtp_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_rtp_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_rtp_is_disconnectable(_handle, channel);
        }

        public void set_incomming_port(ushort port_range_min, ushort port_range_max)
        {
            g2_rtp_set_incomming_port(_handle, port_range_min, port_range_max);
        }
        public void set_buffering_count(int count)
        {
            g2_rtp_set_buffering_count(_handle, count);
        }
        public bool set_camera_list(int channel, g2channel_set channels, bool force)
        {
            G2CHANNEL_SET chs = channels;
            return g2_rtp_set_camera_channelset(_handle, channel, ref chs, force);
        }
        public bool set_camera_list(int channel, uint cameras, bool force)
        {
            return g2_rtp_set_camera_list(_handle, channel, cameras, force);
        }
        public bool set_audio_list(int channel, g2channel_set channels, bool force)
        {
            G2CHANNEL_SET chs = channels;
            return g2_rtp_set_audio_channelset(_handle, channel, ref chs, force);
        }
        public bool set_audio_list(int channel, uint audios, bool force)
        {
            return g2_rtp_set_audio_list(_handle, channel, audios, force);
        }
        public bool set_stream_id(int channel, int camera, int id)
        {
            return g2_rtp_set_stream_id(_handle, channel, camera, id);
        }
        public bool set_alarm_out(int channel, int id, bool on)
        {
            return g2_rtp_set_alarm_out(_handle, channel, id, on);
        }
        public bool set_camera_color(int channel, int camera, G2LIVE_CAMERA_COLOR.TYPE type, G2LIVE_CAMERA_COLOR.VALUE value)
        {
            return g2_rtp_set_camera_color(_handle, channel, camera, (int)type, (int)value);
        }
        public bool set_ptz_command(int channel, int camera, ref G2LIVE_PTZ_COMMAND command)
        {
            return g2_rtp_set_ptz_command(_handle, channel, camera, ref command);
        }
        public bool set_ptz_preset(int channel, int camera, ref G2LIVE_PTZ_PRESET preset)
        {
            return g2_rtp_set_ptz_preset(_handle, channel, camera, ref preset);
        }
        public bool set_enable_audio_streaming(int channel, int camera, bool enable)
        {
            return g2_rtp_set_enable_audio_streaming(_handle, channel, camera, enable);
        }
        public bool set_enable_audio_capturing(int channel, int camera, bool enable)
        {
            return g2_rtp_set_enable_audio_capturing(_handle, channel, camera, enable);
        }
        public bool set_disable_audio_streaming(int channel)
        {
            return g2_rtp_set_disable_audio_streaming(_handle, channel);
        }
        public bool set_disable_audio_capturing(int channel)
        {
            return g2_rtp_set_disable_audio_capturing(_handle, channel);
        }
        public bool set_disable_audio(int channel)
        {
            return g2_rtp_set_disable_audio(_handle, channel);
        }
        public void set_probe_session_profile(bool active)
        {
            g2_rtp_set_probe_session_profile(_handle, active);
        }

        public bool request_alive_check(int channel)
        {
            return g2_rtp_request_alive_check(_handle, channel);
        }
        public bool request_ptz_menu(int channel, int camera)
        {
            return g2_rtp_request_ptz_menu(_handle, channel, camera);
        }
        public bool request_ptz_preset(int channel, int camera)
        {
            return g2_rtp_request_ptz_preset(_handle, channel, camera);
        }

        public bool get_camera_list(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_rtp_get_camera_channelset(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public bool get_camera_list(int channel, out uint cameras)
        {
            return g2_rtp_get_camera_list(_handle, channel, out cameras);
        }
        public bool get_audio_list(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_rtp_get_audio_channelset(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public bool get_audio_list(int channel, out uint audios)
        {
            return g2_rtp_get_audio_list(_handle, channel, out audios);
        }
        public int get_stream_id(int channel, int camera)
        {
            return g2_rtp_get_stream_id(_handle, channel, camera);
        }
        public ulong get_stream_remote(int channel, int camera)
        {
            return g2_rtp_get_stream_remote(_handle, channel, camera);
        }
        public bool get_status(int channel, out G2DEVICE_STATUS status)
        {
            return g2_rtp_get_status(_handle, channel, out status);
        }

        public bool is_enable_multi_stream(int channel, int camera)
        {
            return g2_rtp_is_enable_multi_stream(_handle, channel, camera);
        }
        public bool is_enable_audio_in(int channel, int camera)
        {
            return g2_rtp_is_enable_audio_in(_handle, channel, camera);
        }
        public bool is_enable_audio_out(int channel, int camera)
        {
            return g2_rtp_is_enable_audio_out(_handle, channel, camera);
        }
        public bool is_contains_audio_streaming(int channel, int camera)
        {
            return g2_rtp_is_contains_audio_streaming(_handle, channel, camera);
        }
        public bool is_contains_audio_capturing(int channel, int camera)
        {
            return g2_rtp_is_contains_audio_capturing(_handle, channel, camera);
        }
        public bool is_contains_audio(int channel)
        {
            return g2_rtp_is_contains_audio(_handle, channel);
        }
        public bool is_audio_out_opening(int channel, int camera)
        {
            return g2_rtp_is_audio_out_opening(_handle, channel, camera);
        }

        #region GDK Callback Handler
        private G2RESULT on_rtsp_connected_device(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            return _listener.on_g2rtp_rtsp_connected_device(handle, (int)wparam) ? G2BOOLEAN.G2TRUE : G2BOOLEAN.G2FALSE;
        }
        private G2RESULT on_rtsp_connected(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            return _listener.on_g2rtp_rtsp_connected(handle, (int)wparam) ? G2BOOLEAN.G2TRUE : G2BOOLEAN.G2FALSE;
        }
        private G2RESULT on_rtsp_disconnected(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2rtp_rtsp_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_rtp_connected(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2rtp_rtp_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_frame_data(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2rtp_receive_frame_data(handle, (int)wparam, ref frame);
            return 1;
        }
        private G2RESULT on_receive_event(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_INFO info = (G2EVENT_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_INFO));
            _listener.on_g2rtp_receive_event(handle, (int)wparam, ref info);
            return 1;
        }
        private G2RESULT on_receive_device_status(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEVICE_STATUS status = (G2DEVICE_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2DEVICE_STATUS));
            _listener.on_g2rtp_receive_device_status(handle, (int)wparam, ref status);
            return 1;
        }
        private G2RESULT on_receive_ptz_preset(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_PTZ_PRESET preset = (G2LIVE_PTZ_PRESET)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_PTZ_PRESET));
            _listener.on_g2rtp_receive_ptz_preset(handle, (int)wparam, ref preset);
            return 1;
        }
        private G2RESULT on_receive_ptz_menu(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_PTZ_MENU menu = (G2LIVE_PTZ_MENU)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_PTZ_MENU));
            _listener.on_g2rtp_receive_ptz_menu(handle, (int)wparam, ref menu);
            return 1;
        }
        private G2RESULT on_receive_audio_out_not_available(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2rtp_receive_audio_out_not_available(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_audio_streaming_started(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2rtp_audio_streaming_started(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_audio_streaming_stopped(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2rtp_audio_streaming_stopped(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_audio_capturing_started(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2rtp_audio_capturing_started(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_audio_capturing_stopped(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2rtp_audio_capturing_stopped(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_probe_session_profile(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PROBE_SESSION_PROFILE p = (G2PROBE_SESSION_PROFILE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PROBE_SESSION_PROFILE));
            _listener.on_g2rtp_probe_session_profile(handle, (int)wparam, ref p);
            return 1;
        }
        #endregion
    }
}
