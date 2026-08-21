using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
    using G2HSEARCH_G2 = System.Int32;
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

    public interface g2search_g2_listener
    {
        void on_g2search_g2_connected(G2HSEARCH_G2 handle, int channel);
        void on_g2search_g2_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2search_g2_query_options_search_base(G2HSEARCH_G2 handle, int channel, ref G2SEARCH_G2_OPTIONS_SEARCH_BASE options);
        void on_g2search_g2_query_options_player(G2HSEARCH_G2 handle, int channel, ref G2SEARCH_G2_OPTIONS_PLAYER options);
        void on_g2search_g2_receive_record_time_info_load(G2HSEARCH_G2 handle, int channel, ref G2RECORD_TIME_INFO rti);
        void on_g2search_g2_receive_record_time_info_load_end(G2HSEARCH_G2 handle, int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.COMMAND command);
        void on_g2search_g2_receive_frame_data(G2HSEARCH_G2 handle, int channel, ref G2FRAME frame);
        void on_g2search_g2_receive_text_in(G2HSEARCH_G2 handle, int channel, ref G2EVENT ei);
        void on_g2search_g2_receive_event(G2HSEARCH_G2 handle, int channel, ref G2EVENT ei);
        void on_g2search_g2_receive_notify_command_begin(G2HSEARCH_G2 handle, int channel, G2PLAYER.COMMAND_AND_SPEED command);
        void on_g2search_g2_receive_notify_command_end(G2HSEARCH_G2 handle, int channel, G2PLAYER.COMMAND_AND_SPEED command);
        void on_g2search_g2_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, int channel, G2PLAYER.COMMAND_AND_SPEED speed);
        void on_g2search_g2_receive_notify_frame_not_found(G2HSEARCH_G2 handle, int channel, G2SPOT spot, G2PLAYER.PRECISION precision);
        void on_g2search_g2_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER.OUT_OF_SCOPE status);
        void on_g2search_g2_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, ref G2ROLLBACK_INFO rbi);
        void on_g2search_g2_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER.PLAYER_ERROR error);
        void on_g2search_g2_receive_event_log_load_end(G2HSEARCH_G2 handle, int channel, G2EVENT[] list);
        void on_g2search_g2_receive_event_log_load_stop(G2HSEARCH_G2 handle, int channel, G2EVENT[] list);
        void on_g2search_g2_receive_text_in_log_load_end(G2HSEARCH_G2 handle, int channel, G2EVENT[] list);
        void on_g2search_g2_receive_text_in_log_load_stop(G2HSEARCH_G2 handle, int channel, G2EVENT[] list);
        void on_g2search_g2_receive_scope_list(G2HSEARCH_G2 handle, int channel, G2SCOPE[] scopes, int type);
        void on_g2search_g2_receive_spot_list(G2HSEARCH_G2 handle, int channel, G2SPOT[] spots);
        void on_g2search_g2_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel);
        void on_g2search_g2_receive_db_info(G2HSEARCH_G2 handle, int channel, ref G2SEARCH_G2_REMOTE_DB di);
        void on_g2search_g2_receive_db_info_external(G2HSEARCH_G2 handle, int channel, G2SEARCH_EXTERNAL_DISK[] dis);
        void on_g2search_g2_receive_db_selected(G2HSEARCH_G2 handle, int channel, uint id, G2SEARCH_G2_REMOTE_DB.DB_SELECT_RESULT result);
        void on_g2search_g2_receive_virtual_channelmap(G2HSEARCH_G2 handle, int channel);
        void on_g2search_g2_require_prepare_rollback(G2HSEARCH_G2 handle, int channel, bool prepare);
        void on_g2search_g2_probe_session_profile(G2HSEARCH_G2 handle, int channel, ref G2PROBE_SESSION_PROFILE probe);
    }

    public interface g2search_g2_listener_sole
    {
        void on_g2search_g2_sole_connected(G2HSEARCH_G2 handle, int channel);
        void on_g2search_g2_sole_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2search_g2_sole_query_options_player(G2HSEARCH_G2 handle, int channel, int camera, ref G2SEARCH_G2_OPTIONS_PLAYER options);
        void on_g2search_g2_sole_receive_record_time_info_load(G2HSEARCH_G2 handle, int channel, int camera, ref G2RECORD_TIME_INFO rti);
        void on_g2search_g2_sole_receive_record_time_info_load_end(G2HSEARCH_G2 handle, int channel, int camera, G2RECORD_TIME_INFO.RESOLUTION resolution);
        void on_g2search_g2_sole_receive_frame_data(G2HSEARCH_G2 handle, int channel, int camera, ref G2FRAME frame);
        void on_g2search_g2_sole_receive_text_in(G2HSEARCH_G2 handle, int channel, int camera, ref G2EVENT ei);
        void on_g2search_g2_sole_receive_event(G2HSEARCH_G2 handle, int channel, int camera, ref G2EVENT ei);
        void on_g2search_g2_sole_receive_notify_command_begin(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER.COMMAND_AND_SPEED command);
        void on_g2search_g2_sole_receive_notify_command_end(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER.COMMAND_AND_SPEED command);
        void on_g2search_g2_sole_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER.COMMAND_AND_SPEED speed);
        void on_g2search_g2_sole_receive_notify_frame_not_found(G2HSEARCH_G2 handle, int channel, int camera, G2SPOT spot, G2PLAYER.PRECISION precision);
        void on_g2search_g2_sole_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER.OUT_OF_SCOPE status);
        void on_g2search_g2_sole_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, int camera, ref G2ROLLBACK_INFO rbi);
        void on_g2search_g2_sole_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, int camera, G2PLAYER.PLAYER_ERROR error);
        void on_g2search_g2_sole_receive_scope_list(G2HSEARCH_G2 handle, int channel, int camera, G2SCOPE[] scopes);
        void on_g2search_g2_sole_receive_spot_list(G2HSEARCH_G2 handle, int channel, int camera, G2SPOT[] spots);
        void on_g2search_g2_sole_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel, int camera);
        void on_g2search_g2_sole_require_prepare_rollback(G2HSEARCH_G2 handle, int channel, int camera, bool prepare);
    }

    public interface g2search_g2_listener_saver
    {
        void on_g2search_g2_saver_connected(G2HSEARCH_G2 handle, int channel);
        void on_g2search_g2_saver_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2search_g2_saver_receive_frame_data(G2HSEARCH_G2 handle, int channel, ref G2FRAME frame);
        void on_g2search_g2_saver_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER.OUT_OF_SCOPE status);
        void on_g2search_g2_saver_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, ref G2ROLLBACK_INFO rbi);
        void on_g2search_g2_saver_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER.PLAYER_ERROR error);
        void on_g2search_g2_saver_receive_scope_list(G2HSEARCH_G2 handle, int channel, G2SCOPE[] scopes, int type);
        void on_g2search_g2_saver_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel);
        void on_g2search_g2_saver_receive_clipcopy_size(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_STATUS.TYPE status, ref G2CLIPCOPY_SIZE_INFO csi, uint progress);
        void on_g2search_g2_saver_receive_clipcopy_data(G2HSEARCH_G2 handle, int channel, ulong offset, uint size, IntPtr data, uint progress);
        void on_g2search_g2_saver_receive_clipcopy_set_password(G2HSEARCH_G2 handle, int channel, uint result);
        void on_g2search_g2_saver_receive_clipcopy_canceled(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_ERROR.TYPE error);
        void on_g2search_g2_saver_receive_clipcopy_job_started(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total);
        void on_g2search_g2_saver_receive_clipcopy_job_finished(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total);
        void on_g2search_g2_saver_receive_clipcopy_section_begin(G2HSEARCH_G2 handle, int channel, uint num, uint total);
        void on_g2search_g2_saver_receive_clipcopy_section_end(G2HSEARCH_G2 handle, int channel, uint num, uint total);
    }

    public class g2search_g2
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_register_callback(G2HSEARCH_G2 handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HSEARCH_G2 g2_search_g2_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_finalize(G2HSEARCH_G2 handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_startup(G2HSEARCH_G2 handle, int connections);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_cleanup(G2HSEARCH_G2 handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_g2_connect(G2HSEARCH_G2 handle, ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_g2_connect_ras(G2HSEARCH_G2 handle, ref G2NETWORK_INFO ni, [MarshalAs(UnmanagedType.U1)] bool port_unity, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_disconnect(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_connecting(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_connected(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_disconnecting(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_disconnected(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_disconnectable(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_set_invoke_saver(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_set_revoke_saver(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_set_search_target(G2HSEARCH_G2 handle, int channel, int target);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_set_camera_list(G2HSEARCH_G2 handle, int channel, ref G2CHANNEL_SET channels, ref G2ROLLBACK_INFO rbi, [MarshalAs(UnmanagedType.U1)] bool prepare_rollback, [MarshalAs(UnmanagedType.U1)] out bool preparing);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_set_camera_list_interest(G2HSEARCH_G2 handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_set_play_control_command(G2HSEARCH_G2 handle, int channel, int command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_set_event_query_mode(G2HSEARCH_G2 handle, int channel, int mode);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_set_event_query_cameras(G2HSEARCH_G2 handle, int channel, ref G2CHANNEL_SET cameras);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_g2_set_probe_session_profile(G2HSEARCH_G2 handle, [MarshalAs(UnmanagedType.U1)] bool active);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_alive_check(G2HSEARCH_G2 handle, int channel, int check);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_db_info(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_db_select(G2HSEARCH_G2 handle, int channel, int id, int external_type, int external_num);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_virtual_channelmap(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_record_time_info(G2HSEARCH_G2 handle, int channel, int resolution, int direction, ref G2SCOPE scope, int count, int command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_record_time_info_load_stop(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_record_time_info_on_time(G2HSEARCH_G2 handle, int channel, int resolution, int direction, ref G2TIME from, ref G2TIME to, int count, int command, out G2SCOPE res_scope);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_reload_current(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_reload_recent(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_play(G2HSEARCH_G2 handle, int channel, ref G2PLAYBACK_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_pause(G2HSEARCH_G2 handle, int channel, [MarshalAs(UnmanagedType.U1)] bool rollback, ref G2ROLLBACK_INFO rbi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_stop(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_goto_time_first_of(G2HSEARCH_G2 handle, int channel, ref G2TIME time, [MarshalAs(UnmanagedType.U1)] bool load_adjacent_frame, [MarshalAs(UnmanagedType.U1)] bool forward, [MarshalAs(UnmanagedType.U1)] out bool found_spot);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_move_to_first(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_move_to_last(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_move_to_spot(G2HSEARCH_G2 handle, int channel, ref G2SPOT spot, int precision, [MarshalAs(UnmanagedType.U1)] bool forward);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_move_to_play(G2HSEARCH_G2 handle, int channel, ref G2PLAYBACK_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_prev_step(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_next_step(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_notify_end_of_play(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_scope_list(G2HSEARCH_G2 handle, int channel, ref G2TIME from, ref G2TIME to, ref G2CHANNEL_SET channels, int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_spot_list(G2HSEARCH_G2 handle, int channel, ref G2TIME time, ref G2CHANNEL_SET channels, [MarshalAs(UnmanagedType.U1)] bool load_adjacent_frame);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_measure_size(G2HSEARCH_G2 handle, int channel, ref G2CHANNEL_SET channels, ref G2SCOPE scope, ulong free_space, IntPtr ordered_set, uint ordered_set_len, [MarshalAs(UnmanagedType.U1)] bool slice, ulong slice_size, [MarshalAs(UnmanagedType.U1)] bool explcude_player);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_info(G2HSEARCH_G2 handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_enable_channelset(G2HSEARCH_G2 handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_password(G2HSEARCH_G2 handle, int channel, string password);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_text_in(G2HSEARCH_G2 handle, int channel, [MarshalAs(UnmanagedType.U1)] bool include);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_gps_data(G2HSEARCH_G2 handle, int channel, [MarshalAs(UnmanagedType.U1)] bool include);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_event(G2HSEARCH_G2 handle, int channel, [MarshalAs(UnmanagedType.U1)] bool include);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_cancel(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_size(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_clipcopy_data(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_event_log_search(G2HSEARCH_G2 handle, int channel, ref G2SEARCH_G2_EVENT_SEARCH_OPTIONS option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_event_log_search_next(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_event_log_search_stop(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_text_in_log_search(G2HSEARCH_G2 handle, int channel, ref G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_text_in_log_search_next(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_request_text_in_log_search_stop(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_adaptor(G2HSEARCH_G2 handle, IntPtr ptr);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_server_network_info(G2HSEARCH_G2 handle, int channel, out G2SERVER_NETWORK_INFO ni);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_product_info(G2HSEARCH_G2 handle, int channel, out G2_PRODUCT_INFO pi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_remote_search_caps(G2HSEARCH_G2 handle, int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH caps);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_remote_clipcopy_caps(G2HSEARCH_G2 handle, int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_CLIP_COPY caps);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_g2_get_remote_selected_db(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_remote_db_info(G2HSEARCH_G2 handle, int channel, out G2SEARCH_G2_REMOTE_DB info);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_text_in_search_caps(G2HSEARCH_G2 handle, int channel, out G2_PRODUCT_INFO_CAPS.TEXT_IN_SEARCH caps);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_authority(G2HSEARCH_G2 handle, int channel, out G2RAS_AUTHORITY auth);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_camera_list(G2HSEARCH_G2 handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_camera_list_interest(G2HSEARCH_G2 handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_set_player_scope(G2HSEARCH_G2 handle, int channel, ref G2SCOPE scope);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_set_player_scope_reset(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_set_player_audio_play(G2HSEARCH_G2 handle, int channel, int audio);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_g2_get_play_speed(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_g2_get_play_control_command(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_g2_get_current_command(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_g2_get_event_query_mode(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_event_query_cameras(G2HSEARCH_G2 handle, int channel, out G2CHANNEL_SET cameras);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_option_query_event(G2HSEARCH_G2 handle, int channel, out G2SEARCH_G2_EVENT_SEARCH_OPTIONS options);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_option_query_text_in(G2HSEARCH_G2 handle, int channel, out G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS options);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_get_clipcopy_size_info(G2HSEARCH_G2 handle, int channel, out G2CLIPCOPY_SIZE_INFO csi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_drive_mode(G2HSEARCH_G2 handle, int channel, int mode);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_event_query_mode(G2HSEARCH_G2 handle, int channel, int mode);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_loading_log_event(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_loading_record_time_info(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_stopped(G2HSEARCH_G2 handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_support(G2HSEARCH_G2 handle, int channel, int query);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_authority(G2HSEARCH_G2 handle, int channel, int authority);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_is_probe_session_profile(G2HSEARCH_G2 handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_text_in_search_options_condition_is_valid(ref G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION condition);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_set_camera_list(G2HSEARCH_G2 handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_set_player_scope(G2HSEARCH_G2 handle, int channel, int camera, ref G2SCOPE scope);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_set_player_scope_reset(G2HSEARCH_G2 handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_set_player_audio_play(G2HSEARCH_G2 handle, int channel, int camera, int audio);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_record_time_info(G2HSEARCH_G2 handle, int channel, int camera, int resolution, int direction, ref G2SCOPE scope, int count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_record_time_info_load_stop(G2HSEARCH_G2 handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_record_time_info_on_time(G2HSEARCH_G2 handle, int channel, int camera, int resolution, int direction, ref G2TIME from, ref G2TIME to, int count, out G2SCOPE res_scope);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_play(G2HSEARCH_G2 handle, int channel, int camera, ref G2PLAYBACK_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_pause(G2HSEARCH_G2 handle, int channel, int camera, [MarshalAs(UnmanagedType.U1)] bool rollback, ref G2ROLLBACK_INFO rbi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_stop(G2HSEARCH_G2 handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_move_to_first(G2HSEARCH_G2 handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_move_to_last(G2HSEARCH_G2 handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_move_to_spot(G2HSEARCH_G2 handle, int channel, int camera, ref G2SPOT spot, int precision, [MarshalAs(UnmanagedType.U1)] bool forward);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_move_to_play(G2HSEARCH_G2 handle, int channel, int camera, ref G2PLAYBACK_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_prev_step(G2HSEARCH_G2 handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_next_step(G2HSEARCH_G2 handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_notify_end_of_play(G2HSEARCH_G2 handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_scope_list(G2HSEARCH_G2 handle, int channel, int camera, ref G2TIME from, ref G2TIME to);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_request_spot_list(G2HSEARCH_G2 handle, int channel, int camera, ref G2TIME time, [MarshalAs(UnmanagedType.U1)] bool load_adjacent_frame);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_is_loading_record_time_info(G2HSEARCH_G2 handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_g2_sole_is_stopped(G2HSEARCH_G2 handle, int channel, int camera);
        #endregion

        public g2search_g2()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._listener_sole = null;
            this._listener_saver = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_query_options_search_base = new G2FUN_LISTENER(on_query_options_search_base);
            this._p2_on_query_options_player = new G2FUN_LISTENER(on_query_options_player);
            this._p2_on_receive_record_time_info_load = new G2FUN_LISTENER(on_receive_record_time_info_load);
            this._p2_on_receive_record_time_info_load_end = new G2FUN_LISTENER(on_receive_record_time_info_load_end);
            this._p2_on_receive_frame_data = new G2FUN_LISTENER(on_receive_frame_data);
            this._p2_on_receive_text_in = new G2FUN_LISTENER(on_receive_text_in);
            this._p2_on_receive_event = new G2FUN_LISTENER(on_receive_event);
            this._p2_on_receive_notify_command_begin = new G2FUN_LISTENER(on_receive_notify_command_begin);
            this._p2_on_receive_notify_command_end = new G2FUN_LISTENER(on_receive_notify_command_end);
            this._p2_on_receive_notify_play_speed_changed = new G2FUN_LISTENER(on_receive_notify_play_speed_changed);
            this._p2_on_receive_notify_frame_not_found = new G2FUN_LISTENER(on_receive_notify_frame_not_found);
            this._p2_on_receive_notify_out_of_scope = new G2FUN_LISTENER(on_receive_notify_out_of_scope);
            this._p2_on_receive_notify_get_rollback_info = new G2FUN_LISTENER(on_receive_notify_get_rollback_info);
            this._p2_on_receive_notify_player_error = new G2FUN_LISTENER(on_receive_notify_player_error);
            this._p2_on_receive_event_log_load_end = new G2FUN_LISTENER(on_receive_event_log_load_end);
            this._p2_on_receive_event_log_load_stop = new G2FUN_LISTENER(on_receive_event_log_load_stop);
            this._p2_on_receive_text_in_log_load_end = new G2FUN_LISTENER(on_receive_text_in_log_load_end);
            this._p2_on_receive_text_in_log_load_stop = new G2FUN_LISTENER(on_receive_text_in_log_load_stop);
            this._p2_on_receive_scope_list = new G2FUN_LISTENER(on_receive_scope_list);
            this._p2_on_receive_spot_list = new G2FUN_LISTENER(on_receive_spot_list);
            this._p2_on_receive_no_recorded_data = new G2FUN_LISTENER(on_receive_no_recorded_data);
            this._p2_on_receive_db_info = new G2FUN_LISTENER(on_receive_db_info);
            this._p2_on_receive_db_info_external = new G2FUN_LISTENER(on_receive_db_info_external);
            this._p2_on_receive_db_selected = new G2FUN_LISTENER(on_receive_db_selected);
            this._p2_on_receive_virtual_channelmap = new G2FUN_LISTENER(on_receive_virtual_channelmap);
            this._p2_on_require_prepare_rollback = new G2FUN_LISTENER(on_require_prepare_rollback);
            this._p2_on_probe_session_profile = new G2FUN_LISTENER(on_probe_session_profile);
            this._p2_on_sole_connected = new G2FUN_LISTENER(on_sole_connected);
            this._p2_on_sole_disconnected = new G2FUN_LISTENER(on_sole_disconnected);
            this._p2_on_sole_query_options_player = new G2FUN_LISTENER(on_sole_query_options_player);
            this._p2_on_sole_receive_record_time_info_load = new G2FUN_LISTENER(on_sole_receive_record_time_info_load);
            this._p2_on_sole_receive_record_time_info_load_end = new G2FUN_LISTENER(on_sole_receive_record_time_info_load_end);
            this._p2_on_sole_receive_frame_data = new G2FUN_LISTENER(on_sole_receive_frame_data);
            this._p2_on_sole_receive_text_in = new G2FUN_LISTENER(on_sole_receive_text_in);
            this._p2_on_sole_receive_event = new G2FUN_LISTENER(on_sole_receive_event);
            this._p2_on_sole_receive_notify_command_begin = new G2FUN_LISTENER(on_sole_receive_notify_command_begin);
            this._p2_on_sole_receive_notify_command_end = new G2FUN_LISTENER(on_sole_receive_notify_command_end);
            this._p2_on_sole_receive_notify_play_speed_changed = new G2FUN_LISTENER(on_sole_receive_notify_play_speed_changed);
            this._p2_on_sole_receive_notify_frame_not_found = new G2FUN_LISTENER(on_sole_receive_notify_frame_not_found);
            this._p2_on_sole_receive_notify_out_of_scope = new G2FUN_LISTENER(on_sole_receive_notify_out_of_scope);
            this._p2_on_sole_receive_notify_get_rollback_info = new G2FUN_LISTENER(on_sole_receive_notify_get_rollback_info);
            this._p2_on_sole_receive_notify_player_error = new G2FUN_LISTENER(on_sole_receive_notify_player_error);
            this._p2_on_sole_receive_scope_list = new G2FUN_LISTENER(on_sole_receive_scope_list);
            this._p2_on_sole_receive_spot_list = new G2FUN_LISTENER(on_sole_receive_spot_list);
            this._p2_on_sole_receive_no_recorded_data = new G2FUN_LISTENER(on_sole_receive_no_recorded_data);
            this._p2_on_sole_require_prepare_rollback = new G2FUN_LISTENER(on_sole_require_prepare_rollback);
            this._p2_on_saver_connected = new G2FUN_LISTENER(on_saver_connected);
            this._p2_on_saver_disconnected = new G2FUN_LISTENER(on_saver_disconnected);
            this._p2_on_saver_receive_frame_data = new G2FUN_LISTENER(on_saver_receive_frame_data);
            this._p2_on_saver_receive_notify_out_of_scope = new G2FUN_LISTENER(on_saver_receive_notify_out_of_scope);
            this._p2_on_saver_receive_notify_get_rollback_info = new G2FUN_LISTENER(on_saver_receive_notify_get_rollback_info);
            this._p2_on_saver_receive_notify_player_error = new G2FUN_LISTENER(on_saver_receive_notify_player_error);
            this._p2_on_saver_receive_scope_list = new G2FUN_LISTENER(on_saver_receive_scope_list);
            this._p2_on_saver_receive_no_recorded_data = new G2FUN_LISTENER(on_saver_receive_no_recorded_data);
            this._p2_on_saver_receive_clipcopy_size = new G2FUN_LISTENER(on_saver_receive_clipcopy_size);
            this._p2_on_saver_receive_clipcopy_data = new G2FUN_LISTENER(on_saver_receive_clipcopy_data);
            this._p2_on_saver_receive_clipcopy_set_password = new G2FUN_LISTENER(on_saver_receive_clipcopy_set_password);
            this._p2_on_saver_receive_clipcopy_canceled = new G2FUN_LISTENER(on_saver_receive_clipcopy_canceled);
            this._p2_on_saver_receive_clipcopy_job_started = new G2FUN_LISTENER(on_saver_receive_clipcopy_job_started);
            this._p2_on_saver_receive_clipcopy_job_finished = new G2FUN_LISTENER(on_saver_receive_clipcopy_job_finished);
            this._p2_on_saver_receive_clipcopy_section_begin = new G2FUN_LISTENER(on_saver_receive_clipcopy_section_begin);
            this._p2_on_saver_receive_clipcopy_section_end = new G2FUN_LISTENER(on_saver_receive_clipcopy_section_end);
        }
        ~g2search_g2()
        {
            cleanup();
        }

        private G2HSEARCH_G2 _handle;
        private G2UPARAM _param;
        private g2search_g2_listener _listener;
        private g2search_g2_listener_sole _listener_sole;
        private g2search_g2_listener_saver _listener_saver;

        #region GDK Callback Delegate
        private void register_callback(G2SEARCH_G2_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_search_g2_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_query_options_search_base;
        private G2FUN_LISTENER _p2_on_query_options_player;
        private G2FUN_LISTENER _p2_on_receive_record_time_info_load;
        private G2FUN_LISTENER _p2_on_receive_record_time_info_load_end;
        private G2FUN_LISTENER _p2_on_receive_frame_data;
        private G2FUN_LISTENER _p2_on_receive_text_in;
        private G2FUN_LISTENER _p2_on_receive_event;
        private G2FUN_LISTENER _p2_on_receive_notify_command_begin;
        private G2FUN_LISTENER _p2_on_receive_notify_command_end;
        private G2FUN_LISTENER _p2_on_receive_notify_play_speed_changed;
        private G2FUN_LISTENER _p2_on_receive_notify_frame_not_found;
        private G2FUN_LISTENER _p2_on_receive_notify_out_of_scope;
        private G2FUN_LISTENER _p2_on_receive_notify_get_rollback_info;
        private G2FUN_LISTENER _p2_on_receive_notify_player_error;
        private G2FUN_LISTENER _p2_on_receive_event_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_event_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_scope_list;
        private G2FUN_LISTENER _p2_on_receive_spot_list;
        private G2FUN_LISTENER _p2_on_receive_no_recorded_data;
        private G2FUN_LISTENER _p2_on_receive_db_info;
        private G2FUN_LISTENER _p2_on_receive_db_info_external;
        private G2FUN_LISTENER _p2_on_receive_db_selected;
        private G2FUN_LISTENER _p2_on_receive_virtual_channelmap;
        private G2FUN_LISTENER _p2_on_require_prepare_rollback;
        private G2FUN_LISTENER _p2_on_probe_session_profile;
        private G2FUN_LISTENER _p2_on_sole_connected;
        private G2FUN_LISTENER _p2_on_sole_disconnected;
        private G2FUN_LISTENER _p2_on_sole_query_options_player;
        private G2FUN_LISTENER _p2_on_sole_receive_record_time_info_load;
        private G2FUN_LISTENER _p2_on_sole_receive_record_time_info_load_end;
        private G2FUN_LISTENER _p2_on_sole_receive_frame_data;
        private G2FUN_LISTENER _p2_on_sole_receive_text_in;
        private G2FUN_LISTENER _p2_on_sole_receive_event;
        private G2FUN_LISTENER _p2_on_sole_receive_notify_command_begin;
        private G2FUN_LISTENER _p2_on_sole_receive_notify_command_end;
        private G2FUN_LISTENER _p2_on_sole_receive_notify_play_speed_changed;
        private G2FUN_LISTENER _p2_on_sole_receive_notify_frame_not_found;
        private G2FUN_LISTENER _p2_on_sole_receive_notify_out_of_scope;
        private G2FUN_LISTENER _p2_on_sole_receive_notify_get_rollback_info;
        private G2FUN_LISTENER _p2_on_sole_receive_notify_player_error;
        private G2FUN_LISTENER _p2_on_sole_receive_scope_list;
        private G2FUN_LISTENER _p2_on_sole_receive_spot_list;
        private G2FUN_LISTENER _p2_on_sole_receive_no_recorded_data;
        private G2FUN_LISTENER _p2_on_sole_require_prepare_rollback;
        private G2FUN_LISTENER _p2_on_saver_connected;
        private G2FUN_LISTENER _p2_on_saver_disconnected;
        private G2FUN_LISTENER _p2_on_saver_receive_frame_data;
        private G2FUN_LISTENER _p2_on_saver_receive_notify_out_of_scope;
        private G2FUN_LISTENER _p2_on_saver_receive_notify_get_rollback_info;
        private G2FUN_LISTENER _p2_on_saver_receive_notify_player_error;
        private G2FUN_LISTENER _p2_on_saver_receive_scope_list;
        private G2FUN_LISTENER _p2_on_saver_receive_no_recorded_data;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_size;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_data;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_set_password;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_canceled;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_job_started;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_job_finished;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_section_begin;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_section_end;
        #endregion

        public G2HSEARCH_G2 safe_handle() { return _handle; }

        public void startup(int connections)
        {
            cleanup();

            _handle = g2_search_g2_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_query_options_search_base, _p2_on_query_options_search_base);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_query_options_player, _p2_on_query_options_player);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_record_time_info_load, _p2_on_receive_record_time_info_load);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_record_time_info_load_end, _p2_on_receive_record_time_info_load_end);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_frame_data, _p2_on_receive_frame_data);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_text_in, _p2_on_receive_text_in);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_event, _p2_on_receive_event);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_notify_command_begin, _p2_on_receive_notify_command_begin);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_notify_command_end, _p2_on_receive_notify_command_end);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_notify_play_speed_changed, _p2_on_receive_notify_play_speed_changed);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_notify_frame_not_found, _p2_on_receive_notify_frame_not_found);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_notify_out_of_scope, _p2_on_receive_notify_out_of_scope);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_notify_get_rollback_info, _p2_on_receive_notify_get_rollback_info);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_notify_player_error, _p2_on_receive_notify_player_error);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_event_log_load_end, _p2_on_receive_event_log_load_end);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_event_log_load_stop, _p2_on_receive_event_log_load_stop);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_text_in_log_load_end, _p2_on_receive_text_in_log_load_end);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_text_in_log_load_stop, _p2_on_receive_text_in_log_load_stop);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_scope_list, _p2_on_receive_scope_list);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_spot_list, _p2_on_receive_spot_list);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_no_recorded_data, _p2_on_receive_no_recorded_data);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_db_info, _p2_on_receive_db_info);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_db_info_external, _p2_on_receive_db_info_external);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_db_selected, _p2_on_receive_db_selected);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_receive_virtual_channelmap, _p2_on_receive_virtual_channelmap);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_require_prepare_rollback, _p2_on_require_prepare_rollback);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_probe_session_profile, _p2_on_probe_session_profile);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_connected, _p2_on_sole_connected);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_disconnected, _p2_on_sole_disconnected);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_query_options_player, _p2_on_sole_query_options_player);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_record_time_info_load, _p2_on_sole_receive_record_time_info_load);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_record_time_info_load_end, _p2_on_sole_receive_record_time_info_load_end);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_frame_data, _p2_on_sole_receive_frame_data);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_text_in, _p2_on_sole_receive_text_in);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_event, _p2_on_sole_receive_event);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_notify_command_begin, _p2_on_sole_receive_notify_command_begin);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_notify_command_end, _p2_on_sole_receive_notify_command_end);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_notify_play_speed_changed, _p2_on_sole_receive_notify_play_speed_changed);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_notify_frame_not_found, _p2_on_sole_receive_notify_frame_not_found);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_notify_out_of_scope, _p2_on_sole_receive_notify_out_of_scope);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_notify_get_rollback_info, _p2_on_sole_receive_notify_get_rollback_info);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_notify_player_error, _p2_on_sole_receive_notify_player_error);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_scope_list, _p2_on_sole_receive_scope_list);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_spot_list, _p2_on_sole_receive_spot_list);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_receive_no_recorded_data, _p2_on_sole_receive_no_recorded_data);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_sole_require_prepare_rollback, _p2_on_sole_require_prepare_rollback);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_connected, _p2_on_saver_connected);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_disconnected, _p2_on_saver_disconnected);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_frame_data, _p2_on_saver_receive_frame_data);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_notify_out_of_scope, _p2_on_saver_receive_notify_out_of_scope);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_notify_get_rollback_info, _p2_on_saver_receive_notify_get_rollback_info);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_notify_player_error, _p2_on_saver_receive_notify_player_error);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_scope_list, _p2_on_saver_receive_scope_list);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_no_recorded_data, _p2_on_saver_receive_no_recorded_data);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_clipcopy_size, _p2_on_saver_receive_clipcopy_size);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_clipcopy_data, _p2_on_saver_receive_clipcopy_data);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_clipcopy_set_password, _p2_on_saver_receive_clipcopy_set_password);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_clipcopy_canceled, _p2_on_saver_receive_clipcopy_canceled);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_clipcopy_job_started, _p2_on_saver_receive_clipcopy_job_started);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_clipcopy_job_finished, _p2_on_saver_receive_clipcopy_job_finished);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_clipcopy_section_begin, _p2_on_saver_receive_clipcopy_section_begin);
            register_callback(G2SEARCH_G2_CALLBACK.TYPE.on_saver_receive_clipcopy_section_end, _p2_on_saver_receive_clipcopy_section_end);
            #endregion

            g2_search_g2_startup(_handle, connections);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HSEARCH_G2 handle = _handle; _handle = 0;
                g2_search_g2_cleanup(handle);
                g2_search_g2_finalize(handle);
            }
        }
        public void set_listener(g2search_g2_listener listener)
        {
            _listener = listener;
        }
        public void set_listener_sole(g2search_g2_listener_sole listener)
        {
            _listener_sole = listener;
        }

        public int connect(ref G2GUID root, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_search_g2_connect(_handle, ref root, ref options, out res);
        }
        public int connect(ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_search_g2_connect(_handle, ref root, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, bool port_unity, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_search_g2_connect_ras(_handle, ref ni, port_unity, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, bool port_unity, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_search_g2_connect_ras(_handle, ref ni, port_unity, ref options, out res);
        }
        public void disconnect(int channel)
        {
            g2_search_g2_disconnect(_handle, channel);
        }
        public bool is_connecting(int channel)
        {
            return g2_search_g2_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_search_g2_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_search_g2_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_search_g2_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_search_g2_is_disconnectable(_handle, channel);
        }

        public void set_invoke_saver(int channel, g2search_g2_listener_saver listener)
        {
            this._listener_saver = listener;
            g2_search_g2_set_invoke_saver(_handle, channel);
        }
        public void set_revoke_saver(int channel)
        {
            g2_search_g2_set_revoke_saver(_handle, channel);
            this._listener_saver = null;
        }

        public bool set_search_target(int channel, G2SEARCH_TARGET.TYPE target)
        {
            return g2_search_g2_set_search_target(_handle, channel, (int)target);
        }
        public bool set_camera_list(int channel, g2channel_set channels, G2ROLLBACK_INFO rbi, bool prepare_rollback, out bool preparing)
        {
            G2CHANNEL_SET chs = channels;
            return g2_search_g2_set_camera_list(_handle, channel, ref chs, ref rbi, prepare_rollback, out preparing);
        }
        public bool set_camera_list_interest(int channel, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_search_g2_set_camera_list_interest(_handle, channel, ref chs);
        }
        public bool set_player_scope(int channel, G2SCOPE scope)
        {
            return g2_search_g2_set_player_scope(_handle, channel, ref scope);
        }
        public bool set_player_scope_reset(int channel)
        {
            return g2_search_g2_set_player_scope_reset(_handle, channel);
        }
        public bool set_player_audio_play(int channel, G2PLAYER.AUDIO_PLAY audio)
        {
            return g2_search_g2_set_player_audio_play(_handle, channel, (int)audio);
        }
        public void set_play_control_command(int channel, G2PLAYER.COMMAND_AND_SPEED command)
        {
            g2_search_g2_set_play_control_command(_handle, channel, (int)command);
        }
        public void set_event_query_mode(int channel, G2SEARCH_G2_QUERY.MODE mode)
        {
            g2_search_g2_set_event_query_mode(_handle, channel, (int)mode);
        }
        public void set_event_query_cameras(int channel, g2channel_set cameras)
        {
            G2CHANNEL_SET chs = cameras;
            g2_search_g2_set_event_query_cameras(_handle, channel, ref chs);
        }
        public void set_probe_session_profile(bool active)
        {
            g2_search_g2_set_probe_session_profile(_handle, active);
        }

        public bool request_db_info(int channel)
        {
            return g2_search_g2_request_db_info(_handle, channel);
        }
        public bool request_db_select(int channel, G2SEARCH_G2_REMOTE_DB.STORAGE id)
        {
            return request_db_select(channel, id, -1, -1);
        }
        public bool request_db_select(int channel, G2SEARCH_G2_REMOTE_DB.STORAGE id, int external_type, int external_num)
        {
            return g2_search_g2_request_db_select(_handle, channel, (int)id, external_type, external_num);
        }
        public bool request_virtual_channelmap(int channel)
        {
            return g2_search_g2_request_virtual_channelmap(_handle, channel);
        }
        public bool request_record_time_info(int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.DIRECTION direction, G2SCOPE scope, int count, G2RECORD_TIME_INFO.COMMAND command)
        {
            return g2_search_g2_request_record_time_info(_handle, channel, (int)resolution, (int)direction, ref scope, count, (int)command);
        }
        public bool request_record_time_info_load_stop(int channel)
        {
            return g2_search_g2_request_record_time_info_load_stop(_handle, channel);
        }
        public bool request_record_time_info_on_time(int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.DIRECTION direction, G2TIME from, G2TIME to, int count, G2RECORD_TIME_INFO.COMMAND command, out G2SCOPE res_scope)
        {
            return g2_search_g2_request_record_time_info_on_time(_handle, channel, (int)resolution, (int)direction, ref from, ref to, count, (int)command, out res_scope);
        }
        public bool request_reload_current(int channel)
        {
            return g2_search_g2_request_reload_current(_handle, channel);
        }
        public bool request_reload_recent(int channel)
        {
            return g2_search_g2_request_reload_recent(_handle, channel);
        }
        public bool request_play(int channel, G2PLAYBACK_COMMAND command)
        {
            return g2_search_g2_request_play(_handle, channel, ref command);
        }
        public bool request_pause(int channel, bool rollback, G2ROLLBACK_INFO rbi)
        {
            return g2_search_g2_request_pause(_handle, channel, rollback, ref rbi);
        }
        public bool request_stop(int channel)
        {
            return g2_search_g2_request_stop(_handle, channel);
        }
        public bool request_goto_time_first_of(int channel, G2TIME time, bool load_adjacent_frame, bool forward, out bool found_spot)
        {
            return g2_search_g2_request_goto_time_first_of(_handle, channel, ref time, load_adjacent_frame, forward, out found_spot);
        }
        public bool request_move_to_first(int channel)
        {
            return g2_search_g2_request_move_to_first(_handle, channel);
        }
        public bool request_move_to_last(int channel)
        {
            return g2_search_g2_request_move_to_last(_handle, channel);
        }
        public bool request_move_to_spot(int channel, G2SPOT spot, G2PLAYER.PRECISION precision, bool forward)
        {
            return g2_search_g2_request_move_to_spot(_handle, channel, ref spot, (int)precision, forward);
        }
        public bool request_move_to_play(int channel, G2PLAYBACK_COMMAND command)
        {
            return g2_search_g2_request_move_to_play(_handle, channel, ref command);
        }
        public bool request_prev_step(int channel)
        {
            return g2_search_g2_request_prev_step(_handle, channel);
        }
        public bool request_next_step(int channel)
        {
            return g2_search_g2_request_next_step(_handle, channel);
        }
        public bool request_notify_end_of_play(int channel)
        {
            return g2_search_g2_request_notify_end_of_play(_handle, channel);
        }
        public bool request_scope_list(int channel, G2TIME from, G2TIME to, g2channel_set channels, int type)
        {
            G2CHANNEL_SET chs = channels;
            return g2_search_g2_request_scope_list(_handle, channel, ref from, ref to, ref chs, type);
        }
        public bool request_spot_list(int channel, G2TIME time, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_search_g2_request_spot_list(_handle, channel, ref time, ref chs, false);
        }
        public bool request_spot_list(int channel, G2TIME time, g2channel_set channels, bool load_adjacent_frame)
        {
            G2CHANNEL_SET chs = channels;
            return g2_search_g2_request_spot_list(_handle, channel, ref time, ref chs, load_adjacent_frame);
        }
        public bool request_clipcopy_measure_size(int channel, g2channel_set channels, G2SCOPE scope, ulong free_space, int[] ordered_set, bool slice, ulong slice_size, bool exclude_player)
        {
            GCHandle gch_ordered_set = GCHandle.Alloc(ordered_set, GCHandleType.Pinned);
            G2CHANNEL_SET chs = channels;
            bool ret = g2_search_g2_request_clipcopy_measure_size(_handle, channel, ref chs, ref scope, free_space, gch_ordered_set.AddrOfPinnedObject(), (uint)ordered_set.Length, slice, slice_size, exclude_player);
            gch_ordered_set.Free();
            return ret;
        }
        public bool request_clipcopy_info(int channel, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_search_g2_request_clipcopy_info(_handle, channel, ref chs);
        }
        public bool request_clipcopy_enable_channelset(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_search_g2_request_clipcopy_enable_channelset(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public bool request_clipcopy_password(int channel, string password)
        {
            return g2_search_g2_request_clipcopy_password(_handle, channel, password);
        }
        public bool request_clipcopy_text_in(int channel, bool include)
        {
            return g2_search_g2_request_clipcopy_text_in(_handle, channel, include);
        }
        public bool request_clipcopy_gps_data(int channel, bool include)
        {
            return g2_search_g2_request_clipcopy_gps_data(_handle, channel, include);
        }
        public bool request_clipcopy_event(int channel, bool include)
        {
            return g2_search_g2_request_clipcopy_event(_handle, channel, include);
        }
        public bool request_clipcopy_cancel(int channel)
        {
            return g2_search_g2_request_clipcopy_cancel(_handle, channel);
        }
        public bool request_clipcopy_size(int channel)
        {
            return g2_search_g2_request_clipcopy_size(_handle, channel);
        }
        public bool request_clipcopy_data(int channel)
        {
            return g2_search_g2_request_clipcopy_data(_handle, channel);
        }
        public bool request_event_log_search(int channel, ref G2SEARCH_G2_EVENT_SEARCH_OPTIONS option)
        {
            return g2_search_g2_request_event_log_search(_handle, channel, ref option);
        }
        public bool request_event_log_search_next(int channel)
        {
            return g2_search_g2_request_event_log_search_next(_handle, channel);
        }
        public bool request_event_log_search_stop(int channel)
        {
            return g2_search_g2_request_event_log_search_stop(_handle, channel);
        }
        public bool request_text_in_log_search(int channel, ref G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS option)
        {
            return g2_search_g2_request_text_in_log_search(_handle, channel, ref option);
        }
        public bool request_text_in_log_search_next(int channel)
        {
            return g2_search_g2_request_text_in_log_search_next(_handle, channel);
        }
        public bool request_text_in_log_search_stop(int channel)
        {
            return g2_search_g2_request_text_in_log_search_stop(_handle, channel);
        }

        public G2FUN_GET_ADAPTOR get_adaptor()
        {
            return new G2FUN_GET_ADAPTOR(g2_search_g2_get_adaptor);
        }
        public bool get_server_network_info(int channel, out G2SERVER_NETWORK_INFO ni)
        {
            return g2_search_g2_get_server_network_info(_handle, channel, out ni);
        }
        public bool get_product_info(int channel, out G2_PRODUCT_INFO pi)
        {
            return g2_search_g2_get_product_info(_handle, channel, out pi);
        }
        public bool get_remote_search_caps(int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH caps)
        {
            return g2_search_g2_get_remote_search_caps(_handle, channel, out caps);
        }
        public bool get_remote_clipcopy_caps(int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_CLIP_COPY caps)
        {
            return g2_search_g2_get_remote_clipcopy_caps(_handle, channel, out caps);
        }
        public int get_remote_selected_db(int channel)
        {
            return g2_search_g2_get_remote_selected_db(_handle, channel);
        }
        public bool get_remote_db_info(int channel, out G2SEARCH_G2_REMOTE_DB info)
        {
            return g2_search_g2_get_remote_db_info(_handle, channel, out info);
        }
        public bool get_text_in_search_caps(int channel, out G2_PRODUCT_INFO_CAPS.TEXT_IN_SEARCH caps)
        {
            return g2_search_g2_get_text_in_search_caps(_handle, channel, out caps);
        }
        public bool get_authority(int channel, out G2RAS_AUTHORITY auth)
        {
            return g2_search_g2_get_authority(_handle, channel, out auth);
        }
        public bool get_camera_list(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_search_g2_get_camera_list(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public bool get_camera_list_interest(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_search_g2_get_camera_list_interest(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public G2PLAYER.COMMAND_AND_SPEED get_play_speed(int channel)
        {
            return (G2PLAYER.COMMAND_AND_SPEED)g2_search_g2_get_play_speed(_handle, channel);
        }
        public G2PLAYER.COMMAND_AND_SPEED get_play_control_command(int channel)
        {
            return (G2PLAYER.COMMAND_AND_SPEED)g2_search_g2_get_play_control_command(_handle, channel);
        }
        public G2PLAYER.COMMAND_AND_SPEED get_current_command(int channel)
        {
            return (G2PLAYER.COMMAND_AND_SPEED)g2_search_g2_get_current_command(_handle, channel);
        }
        public G2SEARCH_G2_QUERY.MODE get_event_query_mode(int channel)
        {
            return (G2SEARCH_G2_QUERY.MODE)g2_search_g2_get_event_query_mode(_handle, channel);
        }
        public bool get_event_query_cameras(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_search_g2_get_event_query_cameras(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public bool get_option_query_event(int channel, out G2SEARCH_G2_EVENT_SEARCH_OPTIONS options)
        {
            return g2_search_g2_get_option_query_event(_handle, channel, out options);
        }
        public bool get_option_query_text_in(int channel, out G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS options)
        {
            return g2_search_g2_get_option_query_text_in(_handle, channel, out options);
        }
        public bool get_clipcopy_size_info(int channel, out G2CLIPCOPY_SIZE_INFO csi)
        {
            return g2_search_g2_get_clipcopy_size_info(_handle, channel, out csi);
        }

        public bool is_drive_mode(int channel, G2SEARCH_DRIVE.MODE mode)
        {
            return g2_search_g2_is_drive_mode(_handle, channel, (int)mode);
        }
        public bool is_event_query_mode(int channel, G2SEARCH_G2_QUERY.MODE mode)
        {
            return g2_search_g2_is_event_query_mode(_handle, channel, (int)mode);
        }
        public bool is_loading_record_time_info(int channel)
        {
            return g2_search_g2_is_loading_record_time_info(_handle, channel);
        }
        public bool is_stopped(int channel)
        {
            return g2_search_g2_is_stopped(_handle, channel);
        }
        public bool is_support(int channel, G2SEARCH_SUPPORT.QUERY query)
        {
            return g2_search_g2_is_support(_handle, channel, (int)query);
        }
        public bool is_authority(int channel, G2RAS_AUTHORITY.TYPE authority)
        {
            return g2_search_g2_is_authority(_handle, channel, (int)authority);
        }
        public bool is_probe_session_profile()
        {
            return g2_search_g2_is_probe_session_profile(_handle);
        }

        public static bool text_in_search_condition_is_valid(ref G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS_CONDITION condition)
        {
            return g2_search_g2_text_in_search_options_condition_is_valid(ref condition);
        }

        public bool sole_set_camera_list(int channel, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_search_g2_sole_set_camera_list(_handle, channel, ref chs);
        }
        public bool sole_set_player_scope(int channel, int camera, G2SCOPE scope)
        {
            return g2_search_g2_sole_set_player_scope(_handle, channel, camera, ref scope);
        }
        public bool sole_set_player_scope_reset(int channel, int camera)
        {
            return g2_search_g2_sole_set_player_scope_reset(_handle, channel, camera);
        }
        public bool sole_set_player_audio_play(int channel, int camera, G2PLAYER.AUDIO_PLAY audio)
        {
            return g2_search_g2_sole_set_player_audio_play(_handle, channel, camera, (int)audio);
        }
        public bool sole_request_record_time_info(int channel, int camera, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.DIRECTION direction, G2SCOPE scope, int count)
        {
            return g2_search_g2_sole_request_record_time_info(_handle, channel, camera, (int)resolution, (int)direction, ref scope, count);
        }
        public bool sole_request_record_time_info_load_stop(int channel, int camera)
        {
            return g2_search_g2_sole_request_record_time_info_load_stop(_handle, channel, camera);
        }
        public bool sole_request_record_time_info_on_time(int channel, int camera, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.DIRECTION direction, G2TIME from, G2TIME to, int count, out G2SCOPE res_scope)
        {
            return g2_search_g2_sole_request_record_time_info_on_time(_handle, channel, camera, (int)resolution, (int)direction, ref from, ref to, count, out res_scope);
        }
        public bool sole_request_play(int channel, int camera, G2PLAYBACK_COMMAND command)
        {
            return g2_search_g2_sole_request_play(_handle, channel, camera, ref command);
        }
        public bool sole_request_pause(int channel, int camera, bool rollback, G2ROLLBACK_INFO rbi)
        {
            return g2_search_g2_sole_request_pause(_handle, channel, camera, rollback, ref rbi);
        }
        public bool sole_request_stop(int channel, int camera)
        {
            return g2_search_g2_sole_request_stop(_handle, channel, camera);
        }
        public bool sole_request_move_to_first(int channel, int camera)
        {
            return g2_search_g2_sole_request_move_to_first(_handle, channel, camera);
        }
        public bool sole_request_move_to_last(int channel, int camera)
        {
            return g2_search_g2_sole_request_move_to_last(_handle, channel, camera);
        }
        public bool sole_request_move_to_spot(int channel, int camera, G2SPOT spot, G2PLAYER.PRECISION precision, bool forward)
        {
            return g2_search_g2_sole_request_move_to_spot(_handle, channel, camera, ref spot, (int)precision, forward);
        }
        public bool sole_request_move_to_play(int channel, int camera, G2PLAYBACK_COMMAND command)
        {
            return g2_search_g2_sole_request_move_to_play(_handle, channel, camera, ref command);
        }
        public bool sole_request_prev_step(int channel, int camera)
        {
            return g2_search_g2_sole_request_prev_step(_handle, channel, camera);
        }
        public bool sole_request_next_step(int channel, int camera)
        {
            return g2_search_g2_sole_request_next_step(_handle, channel, camera);
        }
        public bool sole_request_notify_end_of_play(int channel, int camera)
        {
            return g2_search_g2_sole_request_notify_end_of_play(_handle, channel, camera);
        }
        public bool sole_request_scope_list(int channel, int camera, G2TIME from, G2TIME to)
        {
            return g2_search_g2_sole_request_scope_list(_handle, channel, camera, ref from, ref to);
        }
        public bool sole_request_spot_list(int channel, int camera, G2TIME time)
        {
            return g2_search_g2_sole_request_spot_list(_handle, channel, camera, ref time, false);
        }
        public bool sole_request_spot_list(int channel, int camera, G2TIME time, bool load_adjacent_frame)
        {
            return g2_search_g2_sole_request_spot_list(_handle, channel, camera, ref time, load_adjacent_frame);
        }
        public bool sole_is_loading_record_time_info(int channel, int camera)
        {
            return g2_search_g2_sole_is_loading_record_time_info(_handle, channel, camera);
        }
        public bool sole_is_stopped(int channel, int camera)
        {
            return g2_search_g2_sole_is_stopped(_handle, channel, camera);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_query_options_search_base(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_OPTIONS_SEARCH_BASE options = (G2SEARCH_G2_OPTIONS_SEARCH_BASE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_OPTIONS_SEARCH_BASE));
            _listener.on_g2search_g2_query_options_search_base(handle, (int)wparam, ref options);
            Marshal.StructureToPtr(options, (IntPtr)lparam, true);
            return 1;
        }
        private G2RESULT on_query_options_player(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_OPTIONS_PLAYER options = (G2SEARCH_G2_OPTIONS_PLAYER)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_OPTIONS_PLAYER));
            _listener.on_g2search_g2_query_options_player(handle, (int)wparam, ref options);
            Marshal.StructureToPtr(options, (IntPtr)lparam, true);
            return 1;
        }
        private G2RESULT on_receive_record_time_info_load(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2RECORD_TIME_INFO rti = (G2RECORD_TIME_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2RECORD_TIME_INFO));
            _listener.on_g2search_g2_receive_record_time_info_load(handle, (int)wparam, ref rti);
            return 1;
        }
        private G2RESULT on_receive_record_time_info_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_receive_record_time_info_load_end(handle, (int)wparam, (G2RECORD_TIME_INFO.RESOLUTION)G2PARAM_.LOWORD((uint)lparam), (G2RECORD_TIME_INFO.COMMAND)G2PARAM_.HIWORD((uint)lparam));
            return 1;
        }
        private G2RESULT on_receive_frame_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2search_g2_receive_frame_data(handle, (int)wparam, ref frame);
            return 1;
        }
        private G2RESULT on_receive_text_in(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT ei = (G2EVENT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT));
            _listener.on_g2search_g2_receive_text_in(handle, (int)wparam, ref ei);
            return 1;
        }
        private G2RESULT on_receive_event(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT ei = (G2EVENT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT));
            _listener.on_g2search_g2_receive_event(handle, (int)wparam, ref ei);
            return 1;
        }
        private G2RESULT on_receive_notify_command_begin(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_receive_notify_command_begin(handle, (int)wparam, (G2PLAYER.COMMAND_AND_SPEED)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_command_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_receive_notify_command_end(handle, (int)wparam, (G2PLAYER.COMMAND_AND_SPEED)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_receive_notify_play_speed_changed(handle, (int)wparam, (G2PLAYER.COMMAND_AND_SPEED)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_frame_not_found(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SPOT_PRECISION pre = (G2SPOT_PRECISION)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_PRECISION));
            _listener.on_g2search_g2_receive_notify_frame_not_found(handle, (int)wparam, pre._spot, pre.precision);
            return 1;
        }
        private G2RESULT on_receive_notify_out_of_scope(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_receive_notify_out_of_scope(handle, (int)wparam, (G2PLAYER.OUT_OF_SCOPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2ROLLBACK_INFO rbi = (G2ROLLBACK_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2ROLLBACK_INFO));
            _listener.on_g2search_g2_receive_notify_get_rollback_info(handle, (int)wparam, ref rbi);
            Marshal.StructureToPtr(rbi, (IntPtr)lparam, true);
            return 1;
        }
        private G2RESULT on_receive_notify_player_error(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_receive_notify_player_error(handle, (int)wparam, (G2PLAYER.PLAYER_ERROR)lparam);
            return 1;
        }
        private G2RESULT on_receive_event_log_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_LIST buf = (G2EVENT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_LIST));
            G2EVENT[] list = buf.to();
            _listener.on_g2search_g2_receive_event_log_load_end(handle, (int)wparam, list);
            return 1;
        }
        private G2RESULT on_receive_event_log_load_stop(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_LIST buf = (G2EVENT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_LIST));
            G2EVENT[] list = buf.to();
            _listener.on_g2search_g2_receive_event_log_load_stop(handle, (int)wparam, list);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_LIST buf = (G2EVENT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_LIST));
            G2EVENT[] list = buf.to();
            _listener.on_g2search_g2_receive_text_in_log_load_end(handle, (int)wparam, list);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load_stop(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_LIST buf = (G2EVENT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_LIST));
            G2EVENT[] list = buf.to();
            _listener.on_g2search_g2_receive_text_in_log_load_end(handle, (int)wparam, list);
            return 1;
        }
        private G2RESULT on_receive_scope_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SCOPE_LIST data = (G2SEARCH_G2_SCOPE_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_SCOPE_LIST));
            G2SCOPE[] scopes = data._list.to();
            _listener.on_g2search_g2_receive_scope_list(handle, (int)wparam, scopes, data._type);
            return 1;
        }
        private G2RESULT on_receive_spot_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SPOT_LIST data = (G2SPOT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_LIST));
            G2SPOT[] spots = data.to();
            _listener.on_g2search_g2_receive_spot_list(handle, (int)wparam, spots);
            return 1;
        }
        private G2RESULT on_receive_no_recorded_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_receive_no_recorded_data(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_db_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_REMOTE_DB di = (G2SEARCH_G2_REMOTE_DB)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_REMOTE_DB));
            _listener.on_g2search_g2_receive_db_info(handle, (int)wparam, ref di);
            return 1;
        }
        private G2RESULT on_receive_db_info_external(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2SEARCH_EXTERNAL_DISK[] dis = new G2SEARCH_EXTERNAL_DISK[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt32() + i * Marshal.SizeOf(typeof(G2SEARCH_EXTERNAL_DISK)));
                dis[i] = (G2SEARCH_EXTERNAL_DISK)Marshal.PtrToStructure(ptr, typeof(G2SEARCH_EXTERNAL_DISK));
            }
            _listener.on_g2search_g2_receive_db_info_external(handle, (int)wparam, dis);
            return 1;
        }
        private G2RESULT on_receive_db_selected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_receive_db_selected(handle, (int)wparam, G2PARAM_.LOWORD((uint)lparam), (G2SEARCH_G2_REMOTE_DB.DB_SELECT_RESULT)G2PARAM_.HIWORD((uint)lparam));
            return 1;
        }
        private G2RESULT on_receive_virtual_channelmap(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_receive_virtual_channelmap(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_require_prepare_rollback(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_g2_require_prepare_rollback(handle, (int)wparam, Convert.ToBoolean(lparam));
            return 1;
        }
        private G2RESULT on_probe_session_profile(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PROBE_SESSION_PROFILE p = (G2PROBE_SESSION_PROFILE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PROBE_SESSION_PROFILE));
            _listener.on_g2search_g2_probe_session_profile(handle, (int)wparam, ref p);
            return 1;
        }
        #endregion
        #region GDK Callback Handler Sole Player
        private G2RESULT on_sole_connected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_connected(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_sole_disconnected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            }
            return 1;
        }
        private G2RESULT on_sole_query_options_player(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                G2SEARCH_G2_OPTIONS_PLAYER options = (G2SEARCH_G2_OPTIONS_PLAYER)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_OPTIONS_PLAYER));
                _listener_sole.on_g2search_g2_sole_query_options_player(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), ref options);
                Marshal.StructureToPtr(options, (IntPtr)lparam, true);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_record_time_info_load(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                G2RECORD_TIME_INFO rti = (G2RECORD_TIME_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2RECORD_TIME_INFO));
                _listener_sole.on_g2search_g2_sole_receive_record_time_info_load(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), ref rti);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_record_time_info_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_receive_record_time_info_load_end(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), (G2RECORD_TIME_INFO.RESOLUTION)lparam);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_frame_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
                _listener_sole.on_g2search_g2_sole_receive_frame_data(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), ref frame);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_text_in(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                G2EVENT ei = (G2EVENT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT));
                _listener_sole.on_g2search_g2_sole_receive_text_in(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), ref ei);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_event(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                G2EVENT ei = (G2EVENT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT));
                _listener_sole.on_g2search_g2_sole_receive_event(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), ref ei);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_notify_command_begin(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_receive_notify_command_begin(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), (G2PLAYER.COMMAND_AND_SPEED)lparam);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_notify_command_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_receive_notify_command_end(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), (G2PLAYER.COMMAND_AND_SPEED)lparam);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_receive_notify_play_speed_changed(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), (G2PLAYER.COMMAND_AND_SPEED)lparam);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_notify_frame_not_found(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                G2SPOT_PRECISION pre = (G2SPOT_PRECISION)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_PRECISION));
                _listener_sole.on_g2search_g2_sole_receive_notify_frame_not_found(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), pre._spot, pre.precision);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_notify_out_of_scope(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_receive_notify_out_of_scope(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), (G2PLAYER.OUT_OF_SCOPE)lparam);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                G2ROLLBACK_INFO rbi = (G2ROLLBACK_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2ROLLBACK_INFO));
                _listener_sole.on_g2search_g2_sole_receive_notify_get_rollback_info(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), ref rbi);
                Marshal.StructureToPtr(rbi, (IntPtr)lparam, true);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_notify_player_error(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_receive_notify_player_error(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), (G2PLAYER.PLAYER_ERROR)lparam);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_scope_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                G2SEARCH_G2_SCOPE_LIST data = (G2SEARCH_G2_SCOPE_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_SCOPE_LIST));
                G2SCOPE[] scopes = data._list.to();
                _listener_sole.on_g2search_g2_sole_receive_scope_list(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), scopes);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_spot_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                G2SPOT_LIST data = (G2SPOT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_LIST));
                G2SPOT[] spots = data.to();
                _listener_sole.on_g2search_g2_sole_receive_spot_list(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), spots);
            }
            return 1;
        }
        private G2RESULT on_sole_receive_no_recorded_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_receive_no_recorded_data(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam));
            }
            return 1;
        }
        private G2RESULT on_sole_require_prepare_rollback(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_sole != null)
            {
                _listener_sole.on_g2search_g2_sole_require_prepare_rollback(handle, G2PARAM_.LOWORD(wparam), G2PARAM_.HIWORD(wparam), Convert.ToBoolean(lparam));
            }
            return 1;
        }
        #endregion
        #region GDK Callback Handler Saver
        private G2RESULT on_saver_connected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_g2_saver_connected(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_disconnected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_g2_saver_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_frame_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
                _listener_saver.on_g2search_g2_saver_receive_frame_data(handle, (int)wparam, ref frame);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_notify_out_of_scope(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_g2_saver_receive_notify_out_of_scope(handle, (int)wparam, (G2PLAYER.OUT_OF_SCOPE)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2ROLLBACK_INFO rbi = (G2ROLLBACK_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2ROLLBACK_INFO));
                _listener_saver.on_g2search_g2_saver_receive_notify_get_rollback_info(handle, (int)wparam, ref rbi);
                Marshal.StructureToPtr(rbi, (IntPtr)lparam, true);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_notify_player_error(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_g2_saver_receive_notify_player_error(handle, (int)wparam, (G2PLAYER.PLAYER_ERROR)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_scope_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2SEARCH_G2_SCOPE_LIST data = (G2SEARCH_G2_SCOPE_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_SCOPE_LIST));
                G2SCOPE[] scopes = data._list.to();
                _listener_saver.on_g2search_g2_saver_receive_scope_list(handle, (int)wparam, scopes, data._type);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_no_recorded_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_g2_saver_receive_no_recorded_data(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_size(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2SEARCH_G2_PARAM_CLIPCOPY_SIZE_INFO param = (G2SEARCH_G2_PARAM_CLIPCOPY_SIZE_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_PARAM_CLIPCOPY_SIZE_INFO));
                _listener_saver.on_g2search_g2_saver_receive_clipcopy_size(handle, (int)wparam, (G2CLIPCOPY_STATUS.TYPE)param._status, ref param._info, param._progress);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2CLIPCOPY_DATA data = (G2CLIPCOPY_DATA)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_DATA));
                _listener_saver.on_g2search_g2_saver_receive_clipcopy_data(handle, (int)wparam, data._offset, data._size, data._data, data._progress);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_set_password(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_g2_saver_receive_clipcopy_set_password(handle, (int)wparam, (uint)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_canceled(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_g2_saver_receive_clipcopy_canceled(handle, (int)wparam, (G2CLIPCOPY_ERROR.TYPE)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_job_started(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2CLIPCOPY_JOB param = (G2CLIPCOPY_JOB)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_JOB));
                _listener_saver.on_g2search_g2_saver_receive_clipcopy_job_started(handle, (int)wparam, (G2CLIPCOPY_JOB.TYPE)param._job, param._num, param._total);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_job_finished(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2CLIPCOPY_JOB param = (G2CLIPCOPY_JOB)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_JOB));
                _listener_saver.on_g2search_g2_saver_receive_clipcopy_job_finished(handle, (int)wparam, (G2CLIPCOPY_JOB.TYPE)param._job, param._num, param._total);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_section_begin(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
                uint[] vals = new uint[p._len];
                for (int i = 0; i < p._len; ++i)
                {
                    vals[i] = (uint)Marshal.ReadInt32((IntPtr)lparam, i * sizeof(uint));
                }
                _listener_saver.on_g2search_g2_saver_receive_clipcopy_section_begin(handle, (int)wparam, vals[0], vals[1]);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_section_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
                uint[] vals = new uint[p._len];
                for (int i = 0; i < p._len; ++i)
                {
                    vals[i] = (uint)Marshal.ReadInt32((IntPtr)lparam, i * sizeof(uint));
                }
                _listener_saver.on_g2search_g2_saver_receive_clipcopy_section_end(handle, (int)wparam, vals[0], vals[1]);
            }
            return 1;
        }
        #endregion
    }
}
