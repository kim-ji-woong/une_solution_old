using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GDK
{
    using G2HBACKUP_SAVER = System.Int32;
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

    public interface g2backup_saver_listener
    {
        void on_g2backup_saver_connected(G2HBACKUP_SAVER handle, int channel);
        void on_g2backup_saver_disconnected(G2HBACKUP_SAVER handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2backup_saver_receive_backup_site_result(G2HBACKUP_SAVER handle, int channel, ref G2BACKUP_SITE_RESULT result);
        void on_g2backup_saver_receive_record_channels(G2HBACKUP_SAVER handle, int channel, G2BACKUP_CHANNEL_INFO[] channels);
        void on_g2backup_saver_receive_response_no_recorded_data(G2HBACKUP_SAVER handle, int channel, G2BACKUP_CHANNEL_INFO[] channels);
        void on_g2backup_saver_receive_frame_data(G2HBACKUP_SAVER handle, int channel, ref G2FRAME frame);
        void on_g2backup_saver_receive_notify_out_of_scope(G2HBACKUP_SAVER handle, int channel, G2PLAYER.OUT_OF_SCOPE playtype);
        void on_g2backup_saver_receive_notify_player_error(G2HBACKUP_SAVER handle, int channel, G2PLAYER.PLAYER_ERROR errorcode);
        void on_g2backup_saver_receive_scope_list(G2HBACKUP_SAVER handle, int channel, G2SCOPE[] scopes);
        void on_g2backup_saver_receive_no_recorded_data(G2HBACKUP_SAVER handle, int channel);
        void on_g2backup_saver_receive_clipcopy_size(G2HBACKUP_SAVER handle, int channel, G2CLIPCOPY_STATUS.TYPE status, ref G2CLIPCOPY_SIZE_INFO csi);
        void on_g2backup_saver_receive_clipcopy_data(G2HBACKUP_SAVER handle, int channel, ulong offset, uint size, IntPtr data, uint progress);
        void on_g2backup_saver_receive_clipcopy_canceled(G2HBACKUP_SAVER handle, int channel);
        void on_g2backup_saver_receive_clipcopy_set_password(G2HBACKUP_SAVER handle, int channel, uint result);
        void on_g2backup_saver_receive_clipcopy_job_started(G2HBACKUP_SAVER handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total);
        void on_g2backup_saver_receive_clipcopy_job_finished(G2HBACKUP_SAVER handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total);
    }

    public class g2backup_saver
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_saver_register_callback(G2HBACKUP_SAVER handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HBACKUP_SAVER g2_backup_saver_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_saver_finalize(G2HBACKUP_SAVER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_saver_startup(G2HBACKUP_SAVER handle, int connections);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_saver_cleanup(G2HBACKUP_SAVER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_backup_saver_connect(G2HBACKUP_SAVER handle, ref G2GUID service, ref G2GUID site, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_saver_disconnect(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_is_connecting(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_is_connected(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_is_disconnecting(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_is_disconnected(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_is_disconnectable(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_set_camera_list(G2HBACKUP_SAVER handle, int channel, ref G2CHANNEL_SET channels, ref G2ROLLBACK_INFO rbi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_set_camera_list_interest(G2HBACKUP_SAVER handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_backup_site(G2HBACKUP_SAVER handle, int channel, ref G2GUID siteGUID);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_record_channels(G2HBACKUP_SAVER handle, int channel, G2GUID[] camera, uint count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_query_no_recorded_data(G2HBACKUP_SAVER handle, int channel, G2GUID[] camera, uint count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_play(G2HBACKUP_SAVER handle, int channel, ref G2PLAYBACK_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_pause(G2HBACKUP_SAVER handle, int channel, [MarshalAs(UnmanagedType.U1)] bool rollback, ref G2ROLLBACK_INFO rbi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_move_to_spot(G2HBACKUP_SAVER handle, int channel, ref G2SPOT spot, int precision, [MarshalAs(UnmanagedType.U1)] bool forward);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_notify_end_of_play(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_scope_list(G2HBACKUP_SAVER handle, int channel, ref G2TIME from, ref G2TIME to, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_clipcopy_measure_size(G2HBACKUP_SAVER handle, int channel, ref G2CHANNEL_SET channels, ref G2SCOPE scope, ulong free_space);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_clipcopy_password(G2HBACKUP_SAVER handle, int channel, string password);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_clipcopy_text_in(G2HBACKUP_SAVER handle, int channel, [MarshalAs(UnmanagedType.U1)] bool include);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_clipcopy_cancel(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_clipcopy_size(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_request_clipcopy_data(G2HBACKUP_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_saver_get_clipcopy_size_info(G2HBACKUP_SAVER handle, int channel, out G2CLIPCOPY_SIZE_INFO csi);
        #endregion

        public g2backup_saver()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_receive_backup_site_result = new G2FUN_LISTENER(on_receive_backup_site_result);
            this._p2_on_receive_record_channels = new G2FUN_LISTENER(on_receive_record_channels);
            this._p2_on_receive_response_no_recorded_data = new G2FUN_LISTENER(on_receive_response_no_recorded_data);
            this._p2_on_receive_frame_data = new G2FUN_LISTENER(on_receive_frame_data);
            this._p2_on_receive_notify_out_of_scope = new G2FUN_LISTENER(on_receive_notify_out_of_scope);
            this._p2_on_receive_notify_player_error = new G2FUN_LISTENER(on_receive_notify_player_error);
            this._p2_on_receive_scope_list = new G2FUN_LISTENER(on_receive_scope_list);
            this._p2_on_receive_no_recorded_data = new G2FUN_LISTENER(on_receive_no_recorded_data);
            this._p2_on_receive_clipcopy_size = new G2FUN_LISTENER(on_receive_clipcopy_size);
            this._p2_on_receive_clipcopy_data = new G2FUN_LISTENER(on_receive_clipcopy_data);
            this._p2_on_receive_clipcopy_canceled = new G2FUN_LISTENER(on_receive_clipcopy_canceled);
            this._p2_on_receive_clipcopy_set_password = new G2FUN_LISTENER(on_receive_clipcopy_set_password);
            this._p2_on_receive_clipcopy_job_started = new G2FUN_LISTENER(on_receive_clipcopy_job_started);
            this._p2_on_receive_clipcopy_job_finished = new G2FUN_LISTENER(on_receive_clipcopy_job_finished);
        }
        ~g2backup_saver()
        {
            cleanup();
        }

        private G2HBACKUP_SAVER _handle;
        private G2UPARAM _param;
        private g2backup_saver_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2BACKUP_SAVER_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_backup_saver_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_backup_site_result;
        private G2FUN_LISTENER _p2_on_receive_record_channels;
        private G2FUN_LISTENER _p2_on_receive_response_no_recorded_data;
        private G2FUN_LISTENER _p2_on_receive_frame_data;
        private G2FUN_LISTENER _p2_on_receive_notify_out_of_scope;
        private G2FUN_LISTENER _p2_on_receive_notify_player_error;
        private G2FUN_LISTENER _p2_on_receive_scope_list;
        private G2FUN_LISTENER _p2_on_receive_no_recorded_data;
        private G2FUN_LISTENER _p2_on_receive_clipcopy_size;
        private G2FUN_LISTENER _p2_on_receive_clipcopy_data;
        private G2FUN_LISTENER _p2_on_receive_clipcopy_canceled;
        private G2FUN_LISTENER _p2_on_receive_clipcopy_set_password;
        private G2FUN_LISTENER _p2_on_receive_clipcopy_job_started;
        private G2FUN_LISTENER _p2_on_receive_clipcopy_job_finished;
        #endregion

        public G2HBACKUP_SAVER safe_handle() { return _handle; }

        public void startup(int connections)
        {
            cleanup();

            _handle = g2_backup_saver_initialize(_param);

            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_backup_site_result, _p2_on_receive_backup_site_result);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_record_channels, _p2_on_receive_record_channels);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_response_no_recorded_data, _p2_on_receive_response_no_recorded_data);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_frame_data, _p2_on_receive_frame_data);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_notify_out_of_scope, _p2_on_receive_notify_out_of_scope);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_notify_player_error, _p2_on_receive_notify_player_error);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_scope_list, _p2_on_receive_scope_list);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_no_recorded_data, _p2_on_receive_no_recorded_data);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_clipcopy_size, _p2_on_receive_clipcopy_size);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_clipcopy_data, _p2_on_receive_clipcopy_data);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_clipcopy_canceled, _p2_on_receive_clipcopy_canceled);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_clipcopy_set_password, _p2_on_receive_clipcopy_set_password);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_clipcopy_job_started, _p2_on_receive_clipcopy_job_started);
            register_callback(G2BACKUP_SAVER_CALLBACK.TYPE.on_receive_clipcopy_job_finished, _p2_on_receive_clipcopy_job_finished);

            g2_backup_saver_startup(_handle, connections);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HBACKUP_SAVER handle = _handle; _handle = 0;
                g2_backup_saver_cleanup(handle);
                g2_backup_saver_finalize(handle);
            }
        }

        public void set_listener(g2backup_saver_listener listener)
        {
            _listener = listener;
        }

        public int connect(ref G2GUID service, ref G2GUID site, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_backup_saver_connect(_handle, ref service, ref site, ref options, out res);
        }
        public int connect(ref G2GUID service, ref G2GUID site, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_backup_saver_connect(_handle, ref service, ref site, ref options, out res);
        }
        public void disconnect(int channel)
        {
            g2_backup_saver_disconnect(_handle, channel);
        }
        public bool is_connecting(int channel)
        {
            return g2_backup_saver_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_backup_saver_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_backup_saver_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_backup_saver_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_backup_saver_is_disconnectable(_handle, channel);
        }

        public bool set_camera_list(int channel, g2channel_set channelset, ref G2ROLLBACK_INFO rbi)
        {
            G2CHANNEL_SET chs = channelset;
            return g2_backup_saver_set_camera_list(_handle, channel, ref chs, ref rbi);
        }
        public bool set_camera_list_interest(int channel, g2channel_set channelset)
        {
            G2CHANNEL_SET chs = channelset;
            return g2_backup_saver_set_camera_list_interest(_handle, channel, ref chs);
        }

        public bool request_backup_site(int channel, ref G2GUID siteGUID)
        {
            return g2_backup_saver_request_backup_site(_handle, channel, ref siteGUID);
        }
        public bool request_record_channels_each(int channel, G2GUIDSET cameras)
        {
            G2GUID[] param = cameras.to_array();
            return g2_backup_saver_request_record_channels(_handle, channel, param, (uint)param.Length);
        }
        public bool request_query_no_recorded_data(int channel, G2GUIDSET cameras)
        {
            G2GUID[] param = cameras.to_array();
            return g2_backup_saver_request_query_no_recorded_data(_handle, channel, param, (uint)param.Length);
        }
        public bool request_play(int channel, ref G2PLAYBACK_COMMAND command)
        {
            return g2_backup_saver_request_play(_handle, channel, ref command);
        }
        public bool request_pause(int channel, bool rollback, ref G2ROLLBACK_INFO rbi)
        {
            return g2_backup_saver_request_pause(_handle, channel, rollback, ref rbi);
        }
        public bool request_move_to_spot(int channel, ref G2SPOT spot, G2PLAYER.PRECISION precision, bool forward)
        {
            return g2_backup_saver_request_move_to_spot(_handle, channel, ref spot, (int)precision, forward);
        }
        public bool request_notify_end_of_play(int channel)
        {
            return g2_backup_saver_request_notify_end_of_play(_handle, channel);
        }
        public bool request_scope_list(int channel, ref G2TIME from, ref G2TIME to, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_backup_saver_request_scope_list(_handle, channel, ref from, ref to, ref chs);
        }
        public bool request_clipcopy_measure_size(int channel, ref G2CHANNEL_SET channels, ref G2SCOPE scope, uint free_space)
        {
            return g2_backup_saver_request_clipcopy_measure_size(_handle, channel, ref channels, ref scope, free_space);
        }
        public bool request_clipcopy_password(int channel, string password)
        {
            return g2_backup_saver_request_clipcopy_password(_handle, channel, password);
        }
        public bool request_clipcopy_text_in(int channel, bool include)
        {
            return g2_backup_saver_request_clipcopy_text_in(_handle, channel, include);
        }
        public bool request_clipcopy_cancel(int channel)
        {
            return g2_backup_saver_request_clipcopy_cancel(_handle, channel);
        }
        public bool request_clipcopy_size(int channel)
        {
            return g2_backup_saver_request_clipcopy_size(_handle, channel);
        }
        public bool request_clipcopy_data(int channel)
        {
            return g2_backup_saver_request_clipcopy_data(_handle, channel);
        }
        public bool get_clipcopy_size_info(int channel, out G2CLIPCOPY_SIZE_INFO csi)
        {
            return g2_backup_saver_get_clipcopy_size_info(_handle, channel, out csi);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_saver_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_saver_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_backup_site_result(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2BACKUP_SITE_RESULT result = (G2BACKUP_SITE_RESULT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2BACKUP_SITE_RESULT));
            _listener.on_g2backup_saver_receive_backup_site_result(handle, (int)wparam, ref result);
            return 1;
        }
        private G2RESULT on_receive_record_channels(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH bunch = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2BACKUP_CHANNEL_INFO[] chs = new G2BACKUP_CHANNEL_INFO[bunch._len];
            for (int i = 0; i < bunch._len; ++i)
            {
                IntPtr ptr = new IntPtr(bunch._params.ToInt64() + i * Marshal.SizeOf(typeof(G2BACKUP_CHANNEL_INFO)));
                chs[i] = (G2BACKUP_CHANNEL_INFO)Marshal.PtrToStructure(ptr, typeof(G2BACKUP_CHANNEL_INFO));
            }
            _listener.on_g2backup_saver_receive_record_channels(handle, (int)wparam, chs);
            return 1;
        }
        private G2RESULT on_receive_response_no_recorded_data(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH bunch = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2BACKUP_CHANNEL_INFO[] chs = new G2BACKUP_CHANNEL_INFO[bunch._len];
            for (int i = 0; i < bunch._len; ++i)
            {
                IntPtr ptr = new IntPtr(bunch._params.ToInt64() + i * Marshal.SizeOf(typeof(G2BACKUP_CHANNEL_INFO)));
                chs[i] = (G2BACKUP_CHANNEL_INFO)Marshal.PtrToStructure(ptr, typeof(G2BACKUP_CHANNEL_INFO));
            }
            _listener.on_g2backup_saver_receive_response_no_recorded_data(handle, (int)wparam, chs);
            return 1;
        }
        private G2RESULT on_receive_frame_data(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2backup_saver_receive_frame_data(handle, (int)wparam, ref frame);
            return 1;
        }
        private G2RESULT on_receive_notify_out_of_scope(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_saver_receive_notify_out_of_scope(handle, (int)wparam, (G2PLAYER.OUT_OF_SCOPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_player_error(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_saver_receive_notify_player_error(handle, (int)wparam, (G2PLAYER.PLAYER_ERROR)lparam);
            return 1;
        }
        private G2RESULT on_receive_scope_list(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PLAY_SCOPE_LIST scope = (G2PLAY_SCOPE_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PLAY_SCOPE_LIST));
            G2SCOPE[] scopes = scope._list.to();
            _listener.on_g2backup_saver_receive_scope_list(handle, (int)wparam, scopes);
            return 1;
        }
        private G2RESULT on_receive_no_recorded_data(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_saver_receive_no_recorded_data(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_size(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO param = (G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO));
            _listener.on_g2backup_saver_receive_clipcopy_size(handle, (int)wparam, (G2CLIPCOPY_STATUS.TYPE)param._status, ref param._info);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_data(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2CLIPCOPY_DATA data = (G2CLIPCOPY_DATA)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_DATA));
            _listener.on_g2backup_saver_receive_clipcopy_data(handle, (int)wparam, data._offset, data._size, data._data, data._progress);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_canceled(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_saver_receive_clipcopy_canceled(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_set_password(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_saver_receive_clipcopy_set_password(handle, (int)wparam, (uint)lparam);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_job_started(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener != null)
            {
                G2CLIPCOPY_JOB param = (G2CLIPCOPY_JOB)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_JOB));
                _listener.on_g2backup_saver_receive_clipcopy_job_started(handle, (int)wparam, (G2CLIPCOPY_JOB.TYPE)param._job, param._num, param._total);
            }
            return 1;
        }
        private G2RESULT on_receive_clipcopy_job_finished(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener != null)
            {
                G2CLIPCOPY_JOB param = (G2CLIPCOPY_JOB)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_JOB));
                _listener.on_g2backup_saver_receive_clipcopy_job_finished(handle, (int)wparam, (G2CLIPCOPY_JOB.TYPE)param._job, param._num, param._total);
            }
            return 1;
        }
        #endregion
    }
}
