using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace GDK
{
    using G2HBACKUP = System.Int32;
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

    public interface g2backup_listener
    {
        void on_g2backup_connected(G2HBACKUP handle, int channel, ref G2GUID site);
        void on_g2backup_disconnected(G2HBACKUP handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2backup_receive_backup_site_result(G2HBACKUP handle, int channel, ref G2BACKUP_SITE_RESULT result);
        void on_g2backup_receive_record_channels(G2HBACKUP handle, int channel, G2BACKUP_CHANNEL_INFO[] channels);
        void on_g2backup_receive_record_time_info_load(G2HBACKUP handle, int channel, ref G2RECORD_TIME_INFO rti);
        void on_g2backup_receive_record_time_info_load_end(G2HBACKUP handle, int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.COMMAND command);
        void on_g2backup_receive_response_no_recorded_data(G2HBACKUP handle, int channel, G2BACKUP_CHANNEL_INFO[] channels);
        void on_g2backup_receive_frame_data(G2HBACKUP handle, int channel, ref G2FRAME frame);
        void on_g2backup_receive_text_in(G2HBACKUP handle, int channel, ref G2TEXT_IN textIn);
        void on_g2backup_receive_notify_command_begin(G2HBACKUP handle, int channel, G2PLAYER.COMMAND_AND_SPEED command);
        void on_g2backup_receive_notify_command_end(G2HBACKUP handle, int channel, G2PLAYER.COMMAND_AND_SPEED command);
        void on_g2backup_receive_notify_play_speed_changed(G2HBACKUP handle, int channel, G2PLAYER.COMMAND_AND_SPEED speed);
        void on_g2backup_receive_notify_frame_not_found(G2HBACKUP handle, int channel, ref G2SPOT spot, G2PLAYER.PRECISION precision);
        void on_g2backup_receive_notify_out_of_scope(G2HBACKUP handle, int channel, G2PLAYER.OUT_OF_SCOPE playtype);
        void on_g2backup_receive_notify_player_error(G2HBACKUP handle, int channel, G2PLAYER.PLAYER_ERROR errorcode);
        void on_g2backup_receive_scope_list(G2HBACKUP handle, int channel, G2SCOPE[] scopes, G2PLAY_SCOPE_TYPE.TYPE type);
        void on_g2backup_receive_spot_list(G2HBACKUP handle, int channel, G2SPOT[] spots);
        void on_g2backup_receive_no_recorded_data(G2HBACKUP handle, int channel);
        void on_g2backup_receive_event_log_load(G2HBACKUP handle, int channel, ref G2EVENT_LOG log);
        void on_g2backup_receive_event_log_load_end(G2HBACKUP handle, int channel);
        void on_g2backup_receive_event_log_load_fail(G2HBACKUP handle, int channel);
        void on_g2backup_receive_event_log_load_stop(G2HBACKUP handle, int channel);
        void on_g2backup_receive_text_in_log_load(G2HBACKUP handle, int channel, ref G2EVENT log);
        void on_g2backup_receive_text_in_log_load_end(G2HBACKUP handle, int channel);
        void on_g2backup_receive_text_in_log_load_fail(G2HBACKUP handle, int channel);
        void on_g2backup_receive_text_in_log_load_stop(G2HBACKUP handle, int channel);
        void on_g2backup_receive_service_log_load(G2HBACKUP handle, int channel, ref G2SYSTEM_LOG log);
        void on_g2backup_receive_service_log_load_end(G2HBACKUP handle, int channel);
        void on_g2backup_receive_service_log_load_fail(G2HBACKUP handle, int channel);
        void on_g2backup_receive_service_log_load_stop(G2HBACKUP handle, int channel);
        void on_g2backup_receive_debug_log_load(G2HBACKUP handle, int channel, ref G2DEBUG_LOG log);
        void on_g2backup_receive_debug_log_load_end(G2HBACKUP handle, int channel);
        void on_g2backup_receive_debug_log_load_fail(G2HBACKUP handle, int channel);
        void on_g2backup_receive_debug_log_load_stop(G2HBACKUP handle, int channel);
        void on_g2backup_require_prepare_rollback(G2HBACKUP handle, int channel, bool prepare);
        void on_g2backup_probe_session_profile(G2HBACKUP handle, int channel, ref G2PROBE_SESSION_PROFILE probe);
    }

    public class g2backup
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_register_callback(G2HBACKUP handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HBACKUP g2_backup_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_finalize(G2HBACKUP handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_startup(G2HBACKUP handle, int connections);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_cleanup(G2HBACKUP handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_backup_connect(G2HBACKUP handle, ref G2GUID service, ref G2GUID site, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_disconnect(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_is_connecting(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_is_connected(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_is_disconnecting(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_is_disconnected(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_is_disconnectable(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_set_camera_list(G2HBACKUP handle, int channel, ref G2CHANNEL_SET channels, ref G2ROLLBACK_INFO rbi, [MarshalAs(UnmanagedType.U1)] bool prepare_rollback, [MarshalAs(UnmanagedType.U1)] out bool preparing);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_set_camera_list_interest(G2HBACKUP handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_set_play_control_command(G2HBACKUP handle, int channel, int command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_set_event_query_mode(G2HBACKUP handle, int channel, int mode);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_backup_set_probe_session_profile(G2HBACKUP handle, [MarshalAs(UnmanagedType.U1)] bool active);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_service_info(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_alive_check(G2HBACKUP handle, int channel, int check);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_backup_site(G2HBACKUP handle, int channel, ref G2GUID site);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_record_channels(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_record_channels_each(G2HBACKUP handle, int channel, G2GUID[] camera, uint count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_record_time_info(G2HBACKUP handle, int channel, int resolution, int direction, ref G2SCOPE scope, int count, int command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_record_time_info_load_stop(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_query_no_recorded_data(G2HBACKUP handle, int channel, G2GUID[] camera, uint count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_reload_current(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_play(G2HBACKUP handle, int channel, ref G2PLAYBACK_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_pause(G2HBACKUP handle, int channel, [MarshalAs(UnmanagedType.U1)] bool rollback, ref G2ROLLBACK_INFO rbi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_move_to_first(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_move_to_last(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_move_to_spot(G2HBACKUP handle, int channel, ref G2SPOT spot, int precision, [MarshalAs(UnmanagedType.U1)] bool forward);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_prev_step(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_next_step(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_notify_end_of_play(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_scope_list(G2HBACKUP handle, int channel, ref G2TIME from, ref G2TIME to, ref G2CHANNEL_SET channels, int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_spot_list(G2HBACKUP handle, int channel, ref G2TIME time, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_event_log_search(G2HBACKUP handle, int channel, ref G2SERVICE_SEARCH_OPTION_EVENT_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_event_log_search_stop(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_text_in_log_search(G2HBACKUP handle, int channel, ref G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_text_in_log_search_next(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_text_in_log_search_stop(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_text_in_search(G2HBACKUP handle, int channel, ref G2TEXT_IN_QUERY_CONDITION option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_text_in_search_stop(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_system_log_search(G2HBACKUP handle, int channel, ref G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_system_log_search_stop(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_debug_log_search(G2HBACKUP handle, int channel, ref G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_request_debug_log_search_stop(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_get_camera_list(G2HBACKUP handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_get_camera_list_interest(G2HBACKUP handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_backup_get_play_speed(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_backup_get_play_control_command(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_backup_get_current_command(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_backup_get_event_query_mode(G2HBACKUP handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_backup_is_stopped(G2HBACKUP handle, int channel);
        #endregion

        public g2backup()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_receive_backup_site_result = new G2FUN_LISTENER(on_receive_backup_site_result);
            this._p2_on_receive_record_channels = new G2FUN_LISTENER(on_receive_record_channels);
            this._p2_on_receive_record_time_info_load = new G2FUN_LISTENER(on_receive_record_time_info_load);
            this._p2_on_receive_record_time_info_load_end = new G2FUN_LISTENER(on_receive_record_time_info_load_end);
            this._p2_on_receive_response_no_recorded_data = new G2FUN_LISTENER(on_receive_response_no_recorded_data);
            this._p2_on_receive_frame_data = new G2FUN_LISTENER(on_receive_frame_data);
            this._p2_on_receive_text_in = new G2FUN_LISTENER(on_receive_text_in);
            this._p2_on_receive_notify_command_begin = new G2FUN_LISTENER(on_receive_notify_command_begin);
            this._p2_on_receive_notify_command_end = new G2FUN_LISTENER(on_receive_notify_command_end);
            this._p2_on_receive_notify_play_speed_changed = new G2FUN_LISTENER(on_receive_notify_play_speed_changed);
            this._p2_on_receive_notify_frame_not_found = new G2FUN_LISTENER(on_receive_notify_frame_not_found);
            this._p2_on_receive_notify_out_of_scope = new G2FUN_LISTENER(on_receive_notify_out_of_scope);
            this._p2_on_receive_notify_player_error = new G2FUN_LISTENER(on_receive_notify_player_error);
            this._p2_on_receive_scope_list = new G2FUN_LISTENER(on_receive_scope_list);
            this._p2_on_receive_spot_list = new G2FUN_LISTENER(on_receive_spot_list);
            this._p2_on_receive_no_recorded_data = new G2FUN_LISTENER(on_receive_no_recorded_data);
            this._p2_on_receive_event_log_load = new G2FUN_LISTENER(on_receive_event_log_load);
            this._p2_on_receive_event_log_load_end = new G2FUN_LISTENER(on_receive_event_log_load_end);
            this._p2_on_receive_event_log_load_fail = new G2FUN_LISTENER(on_receive_event_log_load_fail);
            this._p2_on_receive_event_log_load_stop = new G2FUN_LISTENER(on_receive_event_log_load_stop);
            this._p2_on_receive_text_in_log_load = new G2FUN_LISTENER(on_receive_text_in_log_load);
            this._p2_on_receive_text_in_log_load_end = new G2FUN_LISTENER(on_receive_text_in_log_load_end);
            this._p2_on_receive_text_in_log_load_fail = new G2FUN_LISTENER(on_receive_text_in_log_load_fail);
            this._p2_on_receive_text_in_log_load_stop = new G2FUN_LISTENER(on_receive_text_in_log_load_stop);
            this._p2_on_receive_service_log_load = new G2FUN_LISTENER(on_receive_service_log_load);
            this._p2_on_receive_service_log_load_end = new G2FUN_LISTENER(on_receive_service_log_load_end);
            this._p2_on_receive_service_log_load_fail = new G2FUN_LISTENER(on_receive_service_log_load_fail);
            this._p2_on_receive_service_log_load_stop = new G2FUN_LISTENER(on_receive_service_log_load_stop);
            this._p2_on_receive_debug_log_load = new G2FUN_LISTENER(on_receive_debug_log_load);
            this._p2_on_receive_debug_log_load_end = new G2FUN_LISTENER(on_receive_debug_log_load_end);
            this._p2_on_receive_debug_log_load_fail = new G2FUN_LISTENER(on_receive_debug_log_load_fail);
            this._p2_on_receive_debug_log_load_stop = new G2FUN_LISTENER(on_receive_debug_log_load_stop);
            this._p2_on_require_prepare_rollback = new G2FUN_LISTENER(on_require_prepare_rollback);
            this._p2_on_probe_session_profile = new G2FUN_LISTENER(on_probe_session_profile);
        }
        ~g2backup()
        {
            cleanup();
        }

        private G2HBACKUP _handle;
        private G2UPARAM _param;
        private g2backup_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2BACKUP_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_backup_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_backup_site_result;
        private G2FUN_LISTENER _p2_on_receive_record_channels;
        private G2FUN_LISTENER _p2_on_receive_record_time_info_load;
        private G2FUN_LISTENER _p2_on_receive_record_time_info_load_end;
        private G2FUN_LISTENER _p2_on_receive_response_no_recorded_data;
        private G2FUN_LISTENER _p2_on_receive_frame_data;
        private G2FUN_LISTENER _p2_on_receive_text_in;
        private G2FUN_LISTENER _p2_on_receive_notify_command_begin;
        private G2FUN_LISTENER _p2_on_receive_notify_command_end;
        private G2FUN_LISTENER _p2_on_receive_notify_play_speed_changed;
        private G2FUN_LISTENER _p2_on_receive_notify_frame_not_found;
        private G2FUN_LISTENER _p2_on_receive_notify_out_of_scope;
        private G2FUN_LISTENER _p2_on_receive_notify_player_error;
        private G2FUN_LISTENER _p2_on_receive_scope_list;
        private G2FUN_LISTENER _p2_on_receive_spot_list;
        private G2FUN_LISTENER _p2_on_receive_no_recorded_data;
        private G2FUN_LISTENER _p2_on_receive_event_log_load;
        private G2FUN_LISTENER _p2_on_receive_event_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_event_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_event_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_service_log_load;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_stop;
        private G2FUN_LISTENER _p2_on_require_prepare_rollback;
        private G2FUN_LISTENER _p2_on_probe_session_profile;

        #endregion

        public G2HBACKUP safe_handle() { return _handle; }

        public void startup(int connections)
        {
            cleanup();

            _handle = g2_backup_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2BACKUP_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_backup_site_result, _p2_on_receive_backup_site_result);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_record_channels, _p2_on_receive_record_channels);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_record_time_info_load, _p2_on_receive_record_time_info_load);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_record_time_info_load_end, _p2_on_receive_record_time_info_load_end);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_response_no_recorded_data, _p2_on_receive_response_no_recorded_data);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_frame_data, _p2_on_receive_frame_data);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_text_in, _p2_on_receive_text_in);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_notify_command_begin, _p2_on_receive_notify_command_begin);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_notify_command_end, _p2_on_receive_notify_command_end);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_notify_play_speed_changed, _p2_on_receive_notify_play_speed_changed);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_notify_frame_not_found, _p2_on_receive_notify_frame_not_found);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_notify_out_of_scope, _p2_on_receive_notify_out_of_scope);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_notify_player_error, _p2_on_receive_notify_player_error);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_scope_list, _p2_on_receive_scope_list);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_spot_list, _p2_on_receive_spot_list);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_no_recorded_data, _p2_on_receive_no_recorded_data);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_event_log_load, _p2_on_receive_event_log_load);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_event_log_load_end, _p2_on_receive_event_log_load_end);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_event_log_load_fail, _p2_on_receive_event_log_load_fail);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_event_log_load_stop, _p2_on_receive_event_log_load_stop);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_text_in_log_load, _p2_on_receive_text_in_log_load);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_text_in_log_load_end, _p2_on_receive_text_in_log_load_end);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_text_in_log_load_fail, _p2_on_receive_text_in_log_load_fail);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_text_in_log_load_stop, _p2_on_receive_text_in_log_load_stop);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_service_log_load, _p2_on_receive_service_log_load);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_service_log_load_end, _p2_on_receive_service_log_load_end);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_service_log_load_fail, _p2_on_receive_service_log_load_fail);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_service_log_load_stop, _p2_on_receive_service_log_load_stop);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_debug_log_load, _p2_on_receive_debug_log_load);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_debug_log_load_end, _p2_on_receive_debug_log_load_end);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_debug_log_load_fail, _p2_on_receive_debug_log_load_fail);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_receive_debug_log_load_stop, _p2_on_receive_debug_log_load_stop);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_require_prepare_rollback, _p2_on_require_prepare_rollback);
            register_callback(G2BACKUP_CALLBACK.TYPE.on_probe_session_profile, _p2_on_probe_session_profile);
            #endregion

            g2_backup_startup(_handle, connections);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HBACKUP handle = _handle; _handle = 0;
                g2_backup_cleanup(handle);
                g2_backup_finalize(handle);
            }
        }
        public void set_listener(g2backup_listener listener)
        {
            _listener = listener;
        }

        public int connect(ref G2GUID service, ref G2GUID site, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_backup_connect(_handle, ref service, ref site, ref options, out res);
        }
        public int connect(ref G2GUID service, ref G2GUID site, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_backup_connect(_handle, ref service, ref site, ref options, out res);
        }
        public void disconnect(int channel)
        {
            g2_backup_disconnect(_handle, channel);
        }
        public bool is_connecting(int channel)
        {
            return g2_backup_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_backup_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_backup_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_backup_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_backup_is_disconnectable(_handle, channel);
        }

        public bool set_camera_list(int channel, g2channel_set channelset, ref G2ROLLBACK_INFO rbi, bool prepare_rollback, out bool preparing)
        {
            G2CHANNEL_SET chs = channelset;
            return g2_backup_set_camera_list(_handle, channel, ref chs, ref rbi, prepare_rollback, out preparing);
        }
        public bool set_camera_list_interest(int channel, g2channel_set channelset)
        {
            G2CHANNEL_SET chs = channelset;
            return g2_backup_set_camera_list_interest(_handle, channel, ref chs);
        }
        public void set_play_control_command(int channel, G2PLAYER.COMMAND_AND_SPEED command)
        {
            g2_backup_set_play_control_command(_handle, channel, (int)command);
        }
        public void set_event_query_mode(int channel, G2BACKUP_QUERY.MODE mode)
        {
            g2_backup_set_event_query_mode(_handle, channel, (int)mode);
        }
        public void set_probe_session_profile(bool active)
        {
            g2_backup_set_probe_session_profile(_handle, active);
        }

        public bool request_alive_check(int channel, int check)
        {
            return g2_backup_request_alive_check(_handle, channel, check);
        }
        public bool request_backup_site(int channel, ref G2GUID site)
        {
            return g2_backup_request_backup_site(_handle, channel, ref site);
        }
        public bool request_record_channels(int channel)
        {
            return g2_backup_request_record_channels(_handle, channel);
        }
        public bool request_record_channels_each(int channel, G2GUIDSET cameras)
        {
            G2GUID[] param = cameras.to_array();
            return g2_backup_request_record_channels_each(_handle, channel, param, (uint)param.Length);
        }
        public bool request_record_time_info(int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.DIRECTION direction, ref G2SCOPE scope, int count, G2RECORD_TIME_INFO.COMMAND command)
        {
            return g2_backup_request_record_time_info(_handle, channel, (int)resolution, (int)direction, ref scope, count, (int)command);
        }
        public bool request_record_time_info_load_stop(int channel)
        {
            return g2_backup_request_record_time_info_load_stop(_handle, channel);
        }
        public bool request_query_no_recorded_data(int channel, G2GUIDSET cameras)
        {
            G2GUID[] param = cameras.to_array();
            return g2_backup_request_query_no_recorded_data(_handle, channel, param, (uint)param.Length);
        }
        public bool request_reload_current(int channel)
        {
            return g2_backup_request_reload_current(_handle, channel);
        }
        public bool request_play(int channel, ref G2PLAYBACK_COMMAND command)
        {
            return g2_backup_request_play(_handle, channel, ref command);
        }
        public bool request_pause(int channel, bool rollback, ref G2ROLLBACK_INFO rbi)
        {
            return g2_backup_request_pause(_handle, channel, rollback, ref rbi);
        }
        public bool request_move_to_first(int channel)
        {
            return g2_backup_request_move_to_first(_handle, channel);
        }
        public bool request_move_to_last(int channel)
        {
            return g2_backup_request_move_to_last(_handle, channel);
        }
        public bool request_move_to_spot(int channel, ref G2SPOT spot, G2PLAYER.PRECISION precision, bool forward)
        {
            return g2_backup_request_move_to_spot(_handle, channel, ref spot, (int)precision, forward);
        }
        public bool request_prev_step(int channel)
        {
            return g2_backup_request_prev_step(_handle, channel);
        }
        public bool request_next_step(int channel)
        {
            return g2_backup_request_next_step(_handle, channel);
        }
        public bool request_notify_end_of_play(int channel)
        {
            return g2_backup_request_notify_end_of_play(_handle, channel);
        }
        public bool request_scope_list(int channel, ref G2TIME from, ref G2TIME to, g2channel_set channels, int type)
        {
            G2CHANNEL_SET chs = channels;
            return g2_backup_request_scope_list(_handle, channel, ref from, ref to, ref chs, type);
        }
        public bool request_spot_list(int channel, ref G2TIME time, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_backup_request_spot_list(_handle, channel, ref time, ref chs);
        }
        public bool request_event_log_search(int channel, G2SERVICE_SEARCH_OPTION_EVENT_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_EVENT_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_EVENT_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_backup_request_event_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }
        public bool request_event_log_search_stop(int channel)
        {
            return g2_backup_request_event_log_search_stop(_handle, channel);
        }
        public bool request_text_in_log_search(int channel, G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_backup_request_text_in_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }
        public bool request_text_in_log_search_next(int channel)
        {
            return g2_backup_request_text_in_log_search_next(_handle, channel);
        }
        public bool request_text_in_log_search_stop(int channel)
        {
            return g2_backup_request_text_in_log_search_stop(_handle, channel);
        }
        public bool request_text_in_search(int channel, ref G2TEXT_IN_QUERY_CONDITION option)
        {
            return g2_backup_request_text_in_search(_handle, channel, ref option);
        }
        public bool request_text_in_search_stop(int channel)
        {
            return g2_backup_request_text_in_search_stop(_handle, channel);
        }
        public bool request_system_log_search(int channel, G2SERVICE_SEARCH_OPTION_SYSTEM_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_backup_request_system_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }
        public bool request_system_log_search_stop(int channel)
        {
            return g2_backup_request_system_log_search_stop(_handle, channel);
        }
        public bool request_debug_log_search(int channel, G2SERVICE_SEARCH_OPTION_DEBUG_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_backup_request_debug_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }
        public bool request_debug_log_search_stop(int channel)
        {
            return g2_backup_request_debug_log_search_stop(_handle, channel);
        }

        public bool get_camera_list(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_backup_get_camera_list(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public bool get_camera_list_interest(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_backup_get_camera_list_interest(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public G2PLAYER.COMMAND_AND_SPEED get_play_speed(int channel)
        {
            return (G2PLAYER.COMMAND_AND_SPEED)g2_backup_get_play_speed(_handle, channel);
        }
        public G2PLAYER.COMMAND_AND_SPEED get_play_control_command(int channel)
        {
            return (G2PLAYER.COMMAND_AND_SPEED)g2_backup_get_play_control_command(_handle, channel);
        }
        public G2PLAYER.COMMAND_AND_SPEED get_current_command(int channel)
        {
            return (G2PLAYER.COMMAND_AND_SPEED)g2_backup_get_current_command(_handle, channel);
        }
        public G2BACKUP_QUERY.MODE get_event_query_mode(int channel)
        {
            return (G2BACKUP_QUERY.MODE)g2_backup_get_event_query_mode(_handle, channel);
        }

        public bool is_stopped(int channel)
        {
            return g2_backup_is_stopped(_handle, channel);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID guid = (G2GUID)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2GUID));
            _listener.on_g2backup_connected(handle, (int)wparam, ref guid);
            return 1;
        }
        private G2RESULT on_disconnected(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_backup_site_result(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2BACKUP_SITE_RESULT result = (G2BACKUP_SITE_RESULT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2BACKUP_SITE_RESULT));
            _listener.on_g2backup_receive_backup_site_result(handle, (int)wparam, ref result);
            return 1;
        }
        private G2RESULT on_receive_record_channels(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH bunch = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2BACKUP_CHANNEL_INFO[] chs = new G2BACKUP_CHANNEL_INFO[bunch._len];
            for (int i = 0; i < bunch._len; ++i)
            {
                IntPtr ptr = new IntPtr(bunch._params.ToInt64() + i * Marshal.SizeOf(typeof(G2BACKUP_CHANNEL_INFO)));
                chs[i] = (G2BACKUP_CHANNEL_INFO)Marshal.PtrToStructure(ptr, typeof(G2BACKUP_CHANNEL_INFO));
            }
            _listener.on_g2backup_receive_record_channels(handle, (int)wparam, chs);
            return 1;
        }
        private G2RESULT on_receive_record_time_info_load(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2RECORD_TIME_INFO info = (G2RECORD_TIME_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2RECORD_TIME_INFO));
            _listener.on_g2backup_receive_record_time_info_load(handle, (int)wparam, ref info);

            return 1;
        }
        private G2RESULT on_receive_record_time_info_load_end(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_record_time_info_load_end(handle, (int)wparam, (G2RECORD_TIME_INFO.RESOLUTION)G2PARAM_.LOWORD((uint)lparam), (G2RECORD_TIME_INFO.COMMAND)G2PARAM_.HIWORD((uint)lparam));
            return 1;
        }
        private G2RESULT on_receive_response_no_recorded_data(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH bunch = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2BACKUP_CHANNEL_INFO[] chs = new G2BACKUP_CHANNEL_INFO[bunch._len];
            for (int i = 0; i < bunch._len; ++i)
            {
                IntPtr ptr = new IntPtr(bunch._params.ToInt64() + i * Marshal.SizeOf(typeof(G2BACKUP_CHANNEL_INFO)));
                chs[i] = (G2BACKUP_CHANNEL_INFO)Marshal.PtrToStructure(ptr, typeof(G2BACKUP_CHANNEL_INFO));
            }
            _listener.on_g2backup_receive_response_no_recorded_data(handle, (int)wparam, chs);
            return 1;
        }
        private G2RESULT on_receive_frame_data(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2backup_receive_frame_data(handle, (int)wparam, ref frame);
            return 1;
        }
        private G2RESULT on_receive_text_in(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2TEXT_IN textIn = (G2TEXT_IN)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2TEXT_IN));
            _listener.on_g2backup_receive_text_in(handle, (int)wparam, ref textIn);
            return 1;
        }
        private G2RESULT on_receive_notify_command_begin(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_notify_command_begin(handle, (int)wparam, (G2PLAYER.COMMAND_AND_SPEED)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_command_end(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_notify_command_end(handle, (int)wparam, (G2PLAYER.COMMAND_AND_SPEED)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_play_speed_changed(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_notify_play_speed_changed(handle, (int)wparam, (G2PLAYER.COMMAND_AND_SPEED)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_frame_not_found(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SPOT_PRECISION data = (G2SPOT_PRECISION)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_PRECISION));
            _listener.on_g2backup_receive_notify_frame_not_found(handle, (int)wparam, ref data._spot, (G2PLAYER.PRECISION)data._precision);
            return 1;
        }
        private G2RESULT on_receive_notify_out_of_scope(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_notify_out_of_scope(handle, (int)wparam, (G2PLAYER.OUT_OF_SCOPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_player_error(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_notify_player_error(handle, (int)wparam, (G2PLAYER.PLAYER_ERROR)lparam);
            return 1;
        }
        private G2RESULT on_receive_scope_list(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PLAY_SCOPE_LIST scope = (G2PLAY_SCOPE_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PLAY_SCOPE_LIST));
            G2SCOPE[] scopes = scope._list.to();
            _listener.on_g2backup_receive_scope_list(handle, (int)wparam, scopes, (G2PLAY_SCOPE_TYPE.TYPE)scope._type);
            return 1;
        }
        private G2RESULT on_receive_spot_list(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SPOT_LIST data = (G2SPOT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_LIST));
            G2SPOT[] bunch = data.to();
            _listener.on_g2backup_receive_spot_list(handle, (int)wparam, bunch);
            return 1;
        }
        private G2RESULT on_receive_no_recorded_data(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_no_recorded_data(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_event_log_load(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_LOG log = (G2EVENT_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_LOG));
            _listener.on_g2backup_receive_event_log_load(handle, (int)wparam, ref log);
            return 1;
        }
        private G2RESULT on_receive_event_log_load_end(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_event_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_event_log_load_fail(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_event_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_event_log_load_stop(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_event_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT textIn = (G2EVENT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT));
            _listener.on_g2backup_receive_text_in_log_load(handle, (int)wparam, ref textIn);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load_end(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_text_in_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load_fail(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_text_in_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load_stop(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_text_in_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SYSTEM_LOG log = (G2SYSTEM_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SYSTEM_LOG));
            _listener.on_g2backup_receive_service_log_load(handle, (int)wparam, ref log);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_end(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_service_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_fail(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_service_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_stop(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_service_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEBUG_LOG log = (G2DEBUG_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2DEBUG_LOG));
            _listener.on_g2backup_receive_debug_log_load(handle, (int)wparam, ref log);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_end(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_debug_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_fail(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_debug_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_stop(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_receive_debug_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_require_prepare_rollback(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2backup_require_prepare_rollback(handle, (int)wparam, Convert.ToBoolean(lparam));
            return 1;
        }
        private G2RESULT on_probe_session_profile(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PROBE_SESSION_PROFILE p = (G2PROBE_SESSION_PROFILE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PROBE_SESSION_PROFILE));
            _listener.on_g2backup_probe_session_profile(handle, (int)wparam, ref p);
            return 1;
        }
        #endregion
    }
}
