using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace GDK
{
    using G2HPLAY = System.Int32;
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

    public interface g2play_listener
    {
        /// <summary>
        /// @ on_g2play_connected
        /// <para>callback when connected to the recording service</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_connected(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_disconnected
        /// <para>callback when disconnected from the recording service</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="reason">reason for disconnection</param>
        void on_g2play_disconnected(G2HPLAY handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        /// <summary>
        /// @ on_g2play_query_options_search_base
        /// <para>callback for setting search base option to the recording service</para>
        /// </summary>
        /// <param name="handle">g2_play control handler</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="options">structure to set to search base option</param>
        void on_g2play_query_options_search_base(G2HPLAY handle, int channel, ref G2PLAY_OPTIONS_SEARCH_BASE options);
        /// <summary>
        /// @ on_g2play_receive_hard_disk_info
        /// <para>callback for the hard disk information used to the recording service</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="dis">hard disk information</param>
        void on_g2play_receive_hard_disk_info(G2HPLAY handle, int channel, G2PLAY_HARD_DISK_INFO[] dis);
        /// <summary>
        /// @ on_g2play_receive_record_channels
        /// <para>callback for the device channel information registered to the recording service</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="channels">device channel information</param>
        void on_g2play_receive_record_channels(G2HPLAY handle, int channel, G2PLAY_CHANNEL_INFO[] channels);
        /// <summary>
        /// @ on_g2play_receive_record_time_info_load
        /// <para>recording time information callback</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="rti">recording time information</param>
        void on_g2play_receive_record_time_info_load(G2HPLAY handle, int channel, ref G2RECORD_TIME_INFO rti);
        /// <summary>
        /// @ on_g2play_receive_record_time_info_load_end
        /// <para>callback for the end of the recording time information callback</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="resolution">resolution</param>
        /// <param name="command">command</param>
        void on_g2play_receive_record_time_info_load_end(G2HPLAY handle, int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.COMMAND command);
        /// <summary>
        /// @ on_g2play_receive_frame_data
        /// <para>recorded image data callback</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="frame">recorded image data</param>
        void on_g2play_receive_frame_data(G2HPLAY handle, int channel, ref G2FRAME frame);
        /// <summary>
        /// @ on_g2play_receive_text_in
        /// <para>Text-in data callback</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="textIn">Text-in data</param>
        void on_g2play_receive_text_in(G2HPLAY handle, int channel, ref G2TEXT_IN textIn);
        /// <summary>
        /// @ on_g2play_receive_notify_command_begin
        /// <para>callback for the start of running a playback-related command for recorded images </para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="command">playback-related command for recorded images </param>
        void on_g2play_receive_notify_command_begin(G2HPLAY handle, int channel, G2PLAYER.COMMAND_AND_SPEED command);
        /// <summary>
        /// @ on_g2play_receive_notify_command_end
        /// <para>callback for the end of running a playback-related command for recorded images </para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="command">playback-related command for recorded images </param>
        void on_g2play_receive_notify_command_end(G2HPLAY handle, int channel, G2PLAYER.COMMAND_AND_SPEED command);
        /// <summary>
        /// @ on_g2play_receive_notify_play_speed_changed
        /// <para>callback when the playback speed of a recorded image has changed</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="speed">playback speed of a recorded image</param>
        void on_g2play_receive_notify_play_speed_changed(G2HPLAY handle, int channel, G2PLAYER.COMMAND_AND_SPEED speed);
        /// <summary>
        /// @ on_g2play_receive_notify_frame_not_found
        /// <para>callback when recorded image data on the requested time is not found</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="spot">requested time information</param>
        /// <param name="precision">precision</param>
        void on_g2play_receive_notify_frame_not_found(G2HPLAY handle, int channel, ref G2SPOT spot, G2PLAYER.PRECISION precision);
        /// <summary>
        /// @ on_g2play_receive_notify_out_of_scope
        /// <para>callback when the requested time is beyond the recording time scope </para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="status">type</param>
        void on_g2play_receive_notify_out_of_scope(G2HPLAY handle, int channel, G2PLAYER.OUT_OF_SCOPE status);
        /// <summary>
        /// @ on_g2play_receive_notify_player_error
        /// <para>callback for an error in recorded image playback</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="error">type</param>
        void on_g2play_receive_notify_player_error(G2HPLAY handle, int channel, G2PLAYER.PLAYER_ERROR error);
        /// <summary>
        /// @ on_g2play_receive_scope_list
        /// <para>callback for the list of recorded image time scopes</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="scopes">recording time scope list</param>
        /// <param name="type">G2PLAY_SCOPE_TYPE enum type</param> 
        void on_g2play_receive_scope_list(G2HPLAY handle, int channel, G2SCOPE[] scopes, int type);
        /// <summary>
        /// @ on_g2play_receive_spot_list
        /// <para>callback for the recording time information list for the specified time</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="spots">recording time information</param>
        void on_g2play_receive_spot_list(G2HPLAY handle, int channel, G2SPOT[] spots);
        /// <summary>
        /// @ on_g2play_receive_no_recorded_data
        /// <para>callback when there is no recorded image</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_no_recorded_data(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_record_failover_scope
        /// <para>callback for the search for the recording failover scope infos</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="infos">record failover scope information array</param>
        void on_g2play_receive_record_failover_scope(G2HPLAY handle, int channel, G2FAILOVER_SCOPE_INFO[] infos);
        /// <summary>
        /// @ on_g2play_receive_event_log_load
        /// <para>callback for the search for the recording service event log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="log">event log</param>
        void on_g2play_receive_event_log_load(G2HPLAY handle, int channel, ref G2EVENT_LOG log);
        /// <summary>
        /// @ on_g2play_receive_event_log_load_end
        /// <para> callback for the end of the search for the recording service event log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_event_log_load_end(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_event_log_load_fail
        /// <para> callback for the failure of the search for the recording service event log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_event_log_load_fail(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_event_log_load_stop
        /// <para>callback for the stop of the search for the recording service event log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_event_log_load_stop(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_text_in_log_load
        /// <para>callback for the search for the service text in event log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="log">text in log</param>
        void on_g2play_receive_text_in_log_load(G2HPLAY handle, int channel, ref G2EVENT log);
        /// <summary>
        /// @ on_g2play_receive_text_in_log_load_end
        /// <para>callback for the end of the search for the service text in event log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_text_in_log_load_end(G2HPLAY handle, int channel);     
        /// <summary>
        /// @ on_g2play_receive_text_in_log_load_fail
        /// <para>callback for the failure of the search for the service text in event log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_text_in_log_load_fail(G2HPLAY handle, int channel);    
        /// <summary>
        /// @ on_g2play_receive_text_in_log_load_stop
        /// <para>callback for the stop of the search for the service text in event log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_text_in_log_load_stop(G2HPLAY handle, int channel);        
        /// <summary>
        /// @ on_g2play_receive_device_log_load
        /// <para>callback for the search for the recording service device status log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="log">device status log</param>
        void on_g2play_receive_device_log_load(G2HPLAY handle, int channel, ref G2SYSTEM_LOG log);
        /// <summary>
        /// @ on_g2play_receive_device_log_load_end
        /// <para>callback for the end of the search for the recording service device status log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_device_log_load_end(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_device_log_load_fail
        /// <para>callback for the failure of the search for the recording service device status log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_device_log_load_fail(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_device_log_load_stop
        /// <para>callback for the stop of the search for the recording service device status log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_device_log_load_stop(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_service_log_load
        /// <para>callback for the search for the recording service system log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="log">system log</param>
        void on_g2play_receive_service_log_load(G2HPLAY handle, int channel, ref G2SYSTEM_LOG log);
        /// <summary>
        /// @ on_g2play_receive_service_log_load_end
        /// <para>callback for the end of the search for the recording service system log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_service_log_load_end(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_service_log_load_fail
        /// <para> callback for the failure of the search for the recording service system log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_service_log_load_fail(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_service_log_load_stop
        /// <para>callback for the stop of the search for the recording service system log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_service_log_load_stop(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_debug_log_load
        /// <para>callback for the search for the recording service debug log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="log">debug log</param>
        void on_g2play_receive_debug_log_load(G2HPLAY handle, int channel, ref G2DEBUG_LOG log);
        /// <summary>
        /// @ on_g2play_receive_debug_log_load_end
        /// <para>callback for the end of the search for the recording service debug log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_debug_log_load_end(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_debug_log_load_fail
        /// <para>callback for the failure of the search for the recording service debug log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_debug_log_load_fail(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_receive_debug_log_load_stop
        /// <para>callback for the stop of the search for the recording service debug log</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_receive_debug_log_load_stop(G2HPLAY handle, int channel);
        /// <summary>
        /// @ on_g2play_require_prepare_rollback
        /// <para>callback when the recording service needs to do rollback</para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="prepare">If set to TRUE, do not receive image data as rollback is being made.</param>
        void on_g2play_require_prepare_rollback(G2HPLAY handle, int channel, bool prepare);
        /// <summary>
        /// @ on_g2play_probe_session_profile
        /// <para>callback for request probe session profile </para>        
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="probe">structure to save the prove session status for profiling</param>
        void on_g2play_probe_session_profile(G2HPLAY handle, int channel, ref G2PROBE_SESSION_PROFILE probe);
        /// <summary>
        /// @ on_g2play_get_frame_buf_status
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="avail">avail</param>
        void on_g2play_get_frame_buf_status(G2HPLAY handle, int channel, int tag, ref int avail);
    }

    public class g2play
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_register_callback(G2HPLAY handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HPLAY g2_play_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_finalize(G2HPLAY handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_startup(G2HPLAY handle, int connections);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_cleanup(G2HPLAY handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_play_connect(G2HPLAY handle, ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res, int usage);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_disconnect(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_is_connecting(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_is_connected(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_is_disconnecting(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_is_disconnected(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_is_disconnectable(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_set_camera_list(G2HPLAY handle, int channel, ref G2CHANNEL_SET channels, ref G2ROLLBACK_INFO rbi, [MarshalAs(UnmanagedType.U1)] bool prepare_rollback, [MarshalAs(UnmanagedType.U1)] out bool preparing);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_set_camera_list_interest(G2HPLAY handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_set_play_control_command(G2HPLAY handle, int channel, int command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_set_probe_session_profile(G2HPLAY handle, [MarshalAs(UnmanagedType.U1)] bool active);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_alive_check(G2HPLAY handle, int channel, int check);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_hard_disk_info(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_record_channels(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_record_channels_each(G2HPLAY handle, int channel, G2GUID[] camera, uint count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_record_time_info(G2HPLAY handle, int channel, int resolution, int direction, ref G2SCOPE scope, int count, int command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_record_time_info_load_stop(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_record_failover_scope_info(G2HPLAY handle, int channel, ref G2SCOPE scope, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_reload_current(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_play(G2HPLAY handle, int channel, ref G2PLAYBACK_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_pause(G2HPLAY handle, int channel, [MarshalAs(UnmanagedType.U1)] bool rollback, ref G2ROLLBACK_INFO rbi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_move_to_first(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_move_to_last(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_move_to_spot(G2HPLAY handle, int channel, ref G2SPOT spot, int precision, [MarshalAs(UnmanagedType.U1)] bool forward);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_prev_step(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_next_step(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_notify_end_of_play(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_scope_list(G2HPLAY handle, int channel, ref G2TIME from, ref G2TIME to, ref G2CHANNEL_SET channels, int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_spot_list(G2HPLAY handle, int channel, ref G2TIME time, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_event_log_search(G2HPLAY handle, int channel, ref G2SERVICE_SEARCH_OPTION_EVENT_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_event_log_search_stop(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_text_in_log_search(G2HPLAY handle, int channel, ref G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_text_in_log_search_next(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_text_in_log_search_stop(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_device_log_search(G2HPLAY handle, int channel, ref G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_device_log_search_stop(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_system_log_search(G2HPLAY handle, int channel, ref G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_system_log_search_stop(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_debug_log_search(G2HPLAY handle, int channel, ref G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_request_debug_log_search_stop(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_get_camera_list(G2HPLAY handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_get_camera_list_interest(G2HPLAY handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_play_get_play_speed(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_play_get_play_control_command(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_play_get_current_command(G2HPLAY handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_is_stopped(G2HPLAY handle, int channel);
        #endregion

        public g2play()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_query_options_search_base = new G2FUN_LISTENER(on_query_options_search_base);
            this._p2_on_receive_hard_disk_info = new G2FUN_LISTENER(on_receive_hard_disk_info);
            this._p2_on_receive_record_channels = new G2FUN_LISTENER(on_receive_record_channels);
            this._p2_on_receive_record_time_info_load = new G2FUN_LISTENER(on_receive_record_time_info_load);
            this._p2_on_receive_record_time_info_load_end = new G2FUN_LISTENER(on_receive_record_time_info_load_end);
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
            this._p2_on_receive_record_failover_scope = new G2FUN_LISTENER(on_receive_record_failover_scope);
            this._p2_on_receive_event_log_load = new G2FUN_LISTENER(on_receive_event_log_load);
            this._p2_on_receive_event_log_load_end = new G2FUN_LISTENER(on_receive_event_log_load_end);
            this._p2_on_receive_event_log_load_fail = new G2FUN_LISTENER(on_receive_event_log_load_fail);
            this._p2_on_receive_event_log_load_stop = new G2FUN_LISTENER(on_receive_event_log_load_stop);
            this._p2_on_receive_text_in_log_load = new G2FUN_LISTENER(on_receive_text_in_log_load);
            this._p2_on_receive_text_in_log_load_end = new G2FUN_LISTENER(on_receive_text_in_log_load_end);
            this._p2_on_receive_text_in_log_load_fail = new G2FUN_LISTENER(on_receive_text_in_log_load_fail);
            this._p2_on_receive_text_in_log_load_stop = new G2FUN_LISTENER(on_receive_text_in_log_load_stop);
            this._p2_on_receive_device_log_load = new G2FUN_LISTENER(on_receive_device_log_load);
            this._p2_on_receive_device_log_load_end = new G2FUN_LISTENER(on_receive_device_log_load_end);
            this._p2_on_receive_device_log_load_fail = new G2FUN_LISTENER(on_receive_device_log_load_fail);
            this._p2_on_receive_device_log_load_stop = new G2FUN_LISTENER(on_receive_device_log_load_stop);
            this._p2_on_receive_service_log_load = new G2FUN_LISTENER(on_receive_service_log_load);
            this._p2_on_receive_service_log_load_end = new G2FUN_LISTENER(on_receive_service_log_load_end);
            this._p2_on_receive_service_log_load_fail = new G2FUN_LISTENER(on_receive_service_log_load_fail);
            this._p2_on_receive_service_log_load_stop = new G2FUN_LISTENER(on_receive_service_log_load_stop);
            this._p2_on_require_prepare_rollback = new G2FUN_LISTENER(on_require_prepare_rollback);
            this._p2_on_probe_session_profile = new G2FUN_LISTENER(on_probe_session_profile);
            this._p2_on_receive_debug_log_load = new G2FUN_LISTENER(on_receive_debug_log_load);
            this._p2_on_receive_debug_log_load_end = new G2FUN_LISTENER(on_receive_debug_log_load_end);
            this._p2_on_receive_debug_log_load_fail = new G2FUN_LISTENER(on_receive_debug_log_load_fail);
            this._p2_on_receive_debug_log_load_stop = new G2FUN_LISTENER(on_receive_debug_log_load_stop);
            this._p2_on_get_frame_buf_status = new G2FUN_LISTENER(on_get_frame_buf_status);

        }
        ~g2play()
        {
            cleanup();
        }

        private G2HPLAY _handle;
        private G2UPARAM _param;
        private g2play_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2PLAY_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_play_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_query_options_search_base;
        private G2FUN_LISTENER _p2_on_receive_hard_disk_info;
        private G2FUN_LISTENER _p2_on_receive_record_channels;
        private G2FUN_LISTENER _p2_on_receive_record_time_info_load;
        private G2FUN_LISTENER _p2_on_receive_record_time_info_load_end;
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
        private G2FUN_LISTENER _p2_on_receive_record_failover_scope;
        private G2FUN_LISTENER _p2_on_receive_event_log_load;
        private G2FUN_LISTENER _p2_on_receive_event_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_event_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_event_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_text_in_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_device_log_load;
        private G2FUN_LISTENER _p2_on_receive_device_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_device_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_device_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_service_log_load;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_stop;
        private G2FUN_LISTENER _p2_on_require_prepare_rollback;
        private G2FUN_LISTENER _p2_on_probe_session_profile;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_stop;
        private G2FUN_LISTENER _p2_on_get_frame_buf_status;
        #endregion

        public G2HPLAY safe_handle() { return _handle; }

        /// <summary>
        /// @ startup
        /// <para>This method creates g2play object, and registers g2play callback functions.</para>
        /// </summary>
        /// <param name="connections">no. of channels to be connected simultaneously to the recording service</param>
        public void startup(int connections)
        {
            cleanup();

            _handle = g2_play_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2PLAY_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2PLAY_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2PLAY_CALLBACK.TYPE.on_query_options_search_base, _p2_on_query_options_search_base);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_hard_disk_info, _p2_on_receive_hard_disk_info);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_record_channels, _p2_on_receive_record_channels);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_record_time_info_load, _p2_on_receive_record_time_info_load);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_record_time_info_load_end, _p2_on_receive_record_time_info_load_end);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_frame_data, _p2_on_receive_frame_data);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_text_in, _p2_on_receive_text_in);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_notify_command_begin, _p2_on_receive_notify_command_begin);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_notify_command_end, _p2_on_receive_notify_command_end);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_notify_play_speed_changed, _p2_on_receive_notify_play_speed_changed);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_notify_frame_not_found, _p2_on_receive_notify_frame_not_found);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_notify_out_of_scope, _p2_on_receive_notify_out_of_scope);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_notify_player_error, _p2_on_receive_notify_player_error);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_scope_list, _p2_on_receive_scope_list);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_spot_list, _p2_on_receive_spot_list);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_no_recorded_data, _p2_on_receive_no_recorded_data);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_record_failover_scope, _p2_on_receive_record_failover_scope);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_event_log_load, _p2_on_receive_event_log_load);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_event_log_load_end, _p2_on_receive_event_log_load_end);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_event_log_load_fail, _p2_on_receive_event_log_load_fail);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_event_log_load_stop, _p2_on_receive_device_log_load_stop);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_text_in_log_load, _p2_on_receive_text_in_log_load);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_text_in_log_load_end, _p2_on_receive_text_in_log_load_end);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_text_in_log_load_fail, _p2_on_receive_text_in_log_load_fail);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_text_in_log_load_stop, _p2_on_receive_device_log_load_stop);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_device_log_load, _p2_on_receive_device_log_load);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_device_log_load_end, _p2_on_receive_device_log_load_end);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_device_log_load_fail, _p2_on_receive_device_log_load_fail);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_device_log_load_stop, _p2_on_receive_device_log_load_stop);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_service_log_load, _p2_on_receive_service_log_load);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_service_log_load_end, _p2_on_receive_service_log_load_end);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_service_log_load_fail, _p2_on_receive_service_log_load_fail);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_service_log_load_stop, _p2_on_receive_service_log_load_stop);
            register_callback(G2PLAY_CALLBACK.TYPE.on_require_prepare_rollback, _p2_on_require_prepare_rollback);
            register_callback(G2PLAY_CALLBACK.TYPE.on_probe_session_profile, _p2_on_probe_session_profile);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_debug_log_load, _p2_on_receive_debug_log_load);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_debug_log_load_end, _p2_on_receive_debug_log_load_end);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_debug_log_load_fail, _p2_on_receive_debug_log_load_fail);
            register_callback(G2PLAY_CALLBACK.TYPE.on_receive_debug_log_load_stop, _p2_on_receive_debug_log_load_stop);
            register_callback(G2PLAY_CALLBACK.TYPE.on_get_frame_buf_status, _p2_on_get_frame_buf_status);
            #endregion

            g2_play_startup(_handle, connections);
        }

        /// <summary>
        /// @ cleanup
        /// <para>This method deletes the memory and resources created by g2_play_startup(). </para>
        /// </summary>
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HPLAY handle = _handle; _handle = 0;
                g2_play_cleanup(handle);
                g2_play_finalize(handle);
            }
        }
        
        /// <summary>
        /// @ set_listener
        /// <para>This method registers g2play_listener function.</para>
        /// </summary>
        /// <param name="listener">listener function</param>
        public void set_listener(g2play_listener listener)
        {
            _listener = listener;
        }

        /// <summary>
        /// @ connect
        /// <para>This method connects to the recording service.</para>
        /// </summary>
        /// <param name="service">recording service unique id</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection fails, it returns -1; otherwise, it returns channel no. (other than -1).</returns>
        public int connect(ref G2GUID service, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_play_connect(_handle, ref service, ref options, out res, (int)G2PLAY_USAGE.TYPE.PLAYBACK);
        }

        /// <summary>
        /// @ connect
        /// <para>This method connects to the recording service.</para>
        /// </summary>
        /// <param name="service">recording service unique id</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <param name="usage">connection type</param>
        /// <returns>If a connection fails, it returns -1; otherwise, it returns channel no. (other than -1).</returns>
        public int connect(ref G2GUID service, out G2CONNECT_RES res, G2PLAY_USAGE.TYPE usage)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_play_connect(_handle, ref service, ref options, out res, (int)usage);
        }

        /// <summary>
        /// @ connect
        /// <para>This method connects to the recording service.</para>
        /// </summary>
        /// <param name="service">This method connects to the recording service.</param>
        /// <param name="options">The structure wherein the options for service connection is saved. It has information about connection time out.</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <param name="usage">connection type</param>
        /// <returns>If a connection fails, it returns -1; otherwise, it returns channel no. (other than -1).</returns>
        public int connect(ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res, G2PLAY_USAGE.TYPE usage)
        {
            return g2_play_connect(_handle, ref service, ref options, out res, (int)usage);
        }

        /// <summary>
        /// @ disconnect
        /// <para>This method disconnects the recording service for the corresponding channel.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        public void disconnect(int channel)
        {
            g2_play_disconnect(_handle, channel);
        }

        /// <summary>
        /// @ is_connecting
        /// <para>This method checks whether the corresponding channel is connecting to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connecting(int channel)
        {
            return g2_play_is_connecting(_handle, channel);
        }

        /// <summary>
        /// @ is_connected
        /// <para>This method checks whether the corresponding channel is connected to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If connected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connected(int channel)
        {
            return g2_play_is_connected(_handle, channel);
        }

        /// <summary>
        /// @ is_disconnecting
        /// <para>This method checks whether the corresponding channel is disconnecting from the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If in the process of disconnecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnecting(int channel)
        {
            return g2_play_is_disconnecting(_handle, channel);
        }
        
        /// <summary>
        /// @ is_disconnected
        /// <para>This method checks whether the corresponding channel is disconnected from the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If disconnected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnected(int channel)
        {
            return g2_play_is_disconnected(_handle, channel);
        }

        /// <summary>
        /// @ is_disconnectable
        /// <para>This method checks whether the corresponding channel is connected or in the process of connecting to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If connected or in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnectable(int channel)
        {
            return g2_play_is_disconnectable(_handle, channel);
        }

        /// <summary>
        /// @ set_camera_list
        /// <para>This method requests recorded images for the specified camera channel to the recording service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channelset">camera channel list</param>
        /// <param name="rbi">structure wherein rollback information is saved</param>
        /// <param name="prepare_rollback">whether to do rollback</param> 
        /// <param name="preparing">return parameter whether do or not rollback</param> 
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_camera_list(int channel, g2channel_set channelset, ref G2ROLLBACK_INFO rbi, bool prepare_rollback, out bool preparing)
        {
            G2CHANNEL_SET chs = channelset;
            return g2_play_set_camera_list(_handle, channel, ref chs, ref rbi, prepare_rollback, out preparing);
        }
        /// <summary>
        /// @ set_camera_list_interest
        /// <para>This method sets the channel of interest on the recording service.</para>
        /// <para>To receive a recorded image, you need to register it as the channel of interest.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="channelset">channel list of interest</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_camera_list_interest(int channel, g2channel_set channelset)
        {
            G2CHANNEL_SET chs = channelset;
            return g2_play_set_camera_list_interest(_handle, channel, ref chs);
        }

        /// <summary>
        /// @ set_play_control_command
        /// <para>This method sets a playback-related command for recorded images on the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="command">playback-related command</param>
        public void set_play_control_command(int channel, int command)
        {
            g2_play_set_play_control_command(_handle, channel, command);
        }
        /// <summary>
        /// @ set_probe_session_profile
        /// <para>This method sets probe session status for profiling.</para>
        /// </summary>
        /// <param name="active">probe session active status</param>
        public void set_probe_session_profile(bool active)
        {
            g2_play_set_probe_session_profile(_handle, active);
        }

        /// <summary>
        /// @ request_alive_check
        /// <para> This method sends packets periodically to maintain the connection to the recording service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="check">Set 0x1010</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_alive_check(int channel, int check)
        {
            return g2_play_request_alive_check(_handle, channel, check);
        }

        /// <summary>
        /// @ request_hard_disk_info
        /// <para> This method requests the hard disk information of the recording service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_hard_disk_info(int channel)
        {
            return g2_play_request_hard_disk_info(_handle, channel);
        }

        /// <summary>
        /// @ request_record_channels
        /// <para>This method requests the channel information for all devices on the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_record_channels(int channel)
        {
            return g2_play_request_record_channels(_handle, channel);
        }

        /// <summary>
        /// @ request_record_channels_each
        /// <para>This method requests the channel information for one or more specified cameras to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="cameras">camera unique id list</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_record_channels_each(int channel, G2GUIDSET cameras)
        {
            G2GUID[] param = cameras.to_array();
            return g2_play_request_record_channels_each(_handle, channel, param, (uint)param.Length);
        }

        /// <summary>
        /// @  request_record_time_info
        /// <para>This method requests recording time information to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="resolution">time information resolution</param>
        /// <param name="direction">search direction
        /// : For backward (to the left) search, -1; for forward search (to the right), 1 </param>
        /// <param name="scope">search scope</param>
        /// <param name="count">maximum length of time for search</param>
        /// <param name="command">command type</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_record_time_info(int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.DIRECTION direction, ref G2SCOPE scope, int count, G2RECORD_TIME_INFO.COMMAND command)
        {
            return g2_play_request_record_time_info(_handle, channel, (int)resolution, (int)direction, ref scope, count, (int)command);
        }

        /// <summary>
        /// @ request_record_time_info_load_stop
        /// <para>This method cancels the request for time information to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_record_time_info_load_stop(int channel)
        {
            return g2_play_request_record_time_info_load_stop(_handle, channel);
        }

        /// <summary>
        /// @ request_record_failover_scope_info
        /// <para>This method requests recording failover scope information to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="scope">search scope. if invalid scope, then receive entire scope</param>
        /// <param name="tag">for distinction, mainly 0</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_record_failover_scope_info(int channel, ref G2SCOPE scope, int tag)
        {
            return g2_play_request_record_failover_scope_info(_handle, channel, ref scope, tag);
        }

        /// <summary>
        /// @ request_reload_current
        /// <para>For the second time, This method requests the frame of the current search location to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_reload_current(int channel)
        {
            return g2_play_request_reload_current(_handle, channel);
        }

        /// <summary>
        /// @ request_play
        /// <para>This method requests recorded image playback to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="command">structure wherein playback command information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_play(int channel, ref G2PLAYBACK_COMMAND command)
        {
            return g2_play_request_play(_handle, channel, ref command);
        }

        /// <summary>
        /// @ request_pause
        /// <para>This method makes a request of pausing the playback of the recorded image to remote device.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="rollback">whether to do rollback</param>
        /// <param name="rbi">structure wherein rollback information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_pause(int channel, bool rollback, ref G2ROLLBACK_INFO rbi)
        {
            return g2_play_request_pause(_handle, channel, rollback, ref rbi);
        }

        /// <summary>
        /// @ request_move_to_first
        /// <para>This method requests the first recorded frame to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_move_to_first(int channel)
        {
            return g2_play_request_move_to_first(_handle, channel);
        }

        /// <summary>
        /// @ request_move_to_last
        /// <para>This method requests the last recorded frame to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_move_to_last(int channel)
        {
            return g2_play_request_move_to_last(_handle, channel);
        }

        /// <summary>
        /// @ request_move_to_spot
        /// <para>This method requests the frame at the specified point of time to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="spot">time information</param>
        /// <param name="precision">precision</param>
        /// <param name="forward">TRUE for forward (to the right) search; FALSE for backward (to the left) search</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_move_to_spot(int channel, ref G2SPOT spot, int precision, bool forward)
        {
            return g2_play_request_move_to_spot(_handle, channel, ref spot, precision, forward);
        }

        /// <summary>
        /// @ request_prev_step
        /// <para>This method requests the previous frame to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_prev_step(int channel)
        {
            return g2_play_request_prev_step(_handle, channel);
        }

        /// <summary>
        /// @ request_next_step
        /// <para>This method requests the next frame to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_next_step(int channel)
        {
            return g2_play_request_next_step(_handle, channel);
        }

        /// <summary>
        /// @ request_notify_end_of_play
        /// <para>This method notifies the recording service that there is no more frame received.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_notify_end_of_play(int channel)
        {
            return g2_play_request_notify_end_of_play(_handle, channel);
        }

        /// <summary>
        /// @ request_scope_list
        /// <para>This method requests the time scope list of recorded images for the specified device channel to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="from">start time</param>
        /// <param name="to">end time</param>
        /// <param name="channels">structure wherein device channel information is saved</param>
        /// <param name="type">time scope type</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_scope_list(int channel, ref G2TIME from, ref G2TIME to, g2channel_set channels, int type)
        {
            G2CHANNEL_SET chs = channels;
            return g2_play_request_scope_list(_handle, channel, ref from, ref to, ref chs, type);
        }

        /// <summary>
        /// @ request_spot_list
        /// <para>This method requests the recording time information on a particular point of time for the specified device channel to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="time">time</param>
        /// <param name="channels">structure wherein device channel information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_spot_list(int channel, ref G2TIME time, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_play_request_spot_list(_handle, channel, ref time, ref chs);
        }

        /// <summary>
        /// @ request_event_log_search
        /// <para>This method requests the event log to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="option">log search condition</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_event_log_search(int channel, G2SERVICE_SEARCH_OPTION_EVENT_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_EVENT_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_EVENT_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_play_request_event_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @ request_event_log_search_stop
        /// <para>This method cancels the request of event log to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_event_log_search_stop(int channel)
        {
            return g2_play_request_event_log_search_stop(_handle, channel);
        }

        /// <summary>
        /// @ request_text_in_log_search
        /// <para>This method requests the recording service text_in log.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="option">text_in log search condition </param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_text_in_log_search(int channel, G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG option) 
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_play_request_text_in_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }
        /// <summary>
        /// @ request_text_in_log_search_next
        /// <para>This method requests the recording service next text in log.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_text_in_log_search_next(int channel)            
        {
            return g2_play_request_text_in_log_search_next(_handle, channel);
        }
        /// <summary>
        /// @ request_text_in_log_search_stop
        /// <para>This method request to stop search the recording service text in log</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_text_in_log_search_stop(int channel)   
        {
            return g2_play_request_text_in_log_search_stop(_handle, channel);
        }

        /// <summary>
        /// @ request_device_log_search
        /// <para>This method requests the operation status log to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="option">log search condition</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_device_log_search(int channel, G2SERVICE_SEARCH_OPTION_SYSTEM_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_play_request_device_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @ request_device_log_search_stop
        /// <para>This method cancels the request for operation status log to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_device_log_search_stop(int channel)
        {
            return g2_play_request_device_log_search_stop(_handle, channel);
        }

        /// <summary>
        /// @ request_system_log_search
        /// <para>This method requests system log to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="option">log search condition</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_system_log_search(int channel, G2SERVICE_SEARCH_OPTION_SYSTEM_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_play_request_system_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @ request_system_log_search_stop
        /// <para>This method cancels the request for system log to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_system_log_search_stop(int channel)
        {
            return g2_play_request_system_log_search_stop(_handle, channel);
        }

        /// <summary>
        /// @ request_debug_log_search
        /// <para>This method requests debug log to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="option">debug log search condition</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_debug_log_search(int channel, G2SERVICE_SEARCH_OPTION_DEBUG_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_play_request_debug_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @ request_debug_log_search_stop
        /// <para>This method cancels the request for debug log to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_debug_log_search_stop(int channel)
        {
            return g2_play_request_debug_log_search_stop(_handle, channel);
        }

        /// <summary>
        /// @ get_camera_list
        /// <para>This method obtains the information of the channel that has requested an image to the recording service</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="channels">structure to save the information of the channel that has requested an image</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_camera_list(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_play_get_camera_list(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        
        /// <summary>
        /// @ get_camera_list_interest
        /// <para>This method obtains the channel information of interest that has been configured to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="channels">structure to save the channel information of interest</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_camera_list_interest(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_play_get_camera_list_interest(_handle, channel, out chs);
            channels = chs;
            return ret;
        }

        /// <summary>
        /// @ get_play_speed
        /// <para>This method obtains the current playback speed of a recorded image.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>It returns the playback speed of a recorded image.</returns>
        public int get_play_speed(int channel)
        {
            return g2_play_get_play_speed(_handle, channel);
        }

        /// <summary>
        /// @ get_play_control_command
        /// <para>This obtains the playback control command that has been configured.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>It returns the value of the playback control command.</returns>
        public int get_play_control_command(int channel)
        {
            return g2_play_get_play_control_command(_handle, channel);
        }

        /// <summary>
        /// @ get_current_command
        /// <para>This method obtains the playback-related command that has been configured to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>It returns the playback-related command that has been configured.</returns>
        public int get_current_command(int channel)
        {
            return g2_play_get_current_command(_handle, channel);
        }

        /// <summary>
        /// @ is_stopped
        /// <para>This method checks whether playback has stopped.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If in playback stop status, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_stopped(int channel)
        {
            return g2_play_is_stopped(_handle, channel);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_query_options_search_base(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PLAY_OPTIONS_SEARCH_BASE options = (G2PLAY_OPTIONS_SEARCH_BASE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PLAY_OPTIONS_SEARCH_BASE));
            _listener.on_g2play_query_options_search_base(handle, (int)wparam, ref options);
            Marshal.StructureToPtr(options, (IntPtr)lparam, true);
            return 1;
        }
        private G2RESULT on_receive_hard_disk_info(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH bunch = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2PLAY_HARD_DISK_INFO[] hdi = new G2PLAY_HARD_DISK_INFO[bunch._len];
            for (int i = 0; i < bunch._len; ++i)
            {
                IntPtr ptr = new IntPtr(bunch._params.ToInt64() + i * Marshal.SizeOf(typeof(G2PLAY_HARD_DISK_INFO)));
                hdi[i] = (G2PLAY_HARD_DISK_INFO)Marshal.PtrToStructure(ptr, typeof(G2PLAY_HARD_DISK_INFO));
            }
            _listener.on_g2play_receive_hard_disk_info(handle, (int)wparam, hdi);
            return 1;
        }
        private G2RESULT on_receive_record_channels(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH bunch = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2PLAY_CHANNEL_INFO[] chs = new G2PLAY_CHANNEL_INFO[bunch._len];
            for (int i = 0; i < bunch._len; ++i)
            {
                IntPtr ptr = new IntPtr(bunch._params.ToInt64() + i * Marshal.SizeOf(typeof(G2PLAY_CHANNEL_INFO)));
                chs[i] = (G2PLAY_CHANNEL_INFO)Marshal.PtrToStructure(ptr, typeof(G2PLAY_CHANNEL_INFO));
            }
            _listener.on_g2play_receive_record_channels(handle, (int)wparam, chs);
            return 1;
        }
        private G2RESULT on_receive_record_time_info_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2RECORD_TIME_INFO rti = (G2RECORD_TIME_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2RECORD_TIME_INFO));
            _listener.on_g2play_receive_record_time_info_load(handle, (int)wparam, ref rti);
            return 1;
        }
        private G2RESULT on_receive_record_time_info_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_record_time_info_load_end(handle, (int)wparam, (G2RECORD_TIME_INFO.RESOLUTION)G2PARAM_.LOWORD((uint)lparam), (G2RECORD_TIME_INFO.COMMAND)G2PARAM_.HIWORD((uint)lparam));
            return 1;
        }
        private G2RESULT on_receive_frame_data(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2play_receive_frame_data(handle, (int)wparam, ref frame);
            return 1;
        }
        private G2RESULT on_receive_text_in(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2TEXT_IN textIn = (G2TEXT_IN)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2TEXT_IN));
            _listener.on_g2play_receive_text_in(handle, (int)wparam, ref textIn);
            return 1;
        }
        private G2RESULT on_receive_notify_command_begin(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_notify_command_begin(handle, (int)wparam, (G2PLAYER.COMMAND_AND_SPEED)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_command_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_notify_command_end(handle, (int)wparam, (G2PLAYER.COMMAND_AND_SPEED)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_play_speed_changed(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_notify_play_speed_changed(handle, (int)wparam, (G2PLAYER.COMMAND_AND_SPEED)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_frame_not_found(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SPOT_PRECISION data = (G2SPOT_PRECISION)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_PRECISION));
            _listener.on_g2play_receive_notify_frame_not_found(handle, (int)wparam, ref data._spot, (G2PLAYER.PRECISION)data._precision);
            return 1;
        }
        private G2RESULT on_receive_notify_out_of_scope(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_notify_out_of_scope(handle, (int)wparam, (G2PLAYER.OUT_OF_SCOPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_player_error(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_notify_player_error(handle, (int)wparam, (G2PLAYER.PLAYER_ERROR)lparam);
            return 1;
        }
        private G2RESULT on_receive_scope_list(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PLAY_SCOPE_LIST scope = (G2PLAY_SCOPE_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PLAY_SCOPE_LIST));
            G2SCOPE[] scopes = scope._list.to();
            _listener.on_g2play_receive_scope_list(handle, (int)wparam, scopes, scope._type);
            return 1;
        }
        private G2RESULT on_receive_spot_list(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SPOT_LIST data = (G2SPOT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_LIST));
            G2SPOT[] bunch = data.to();
            _listener.on_g2play_receive_spot_list(handle, (int)wparam, bunch);
            return 1;
        }
        private G2RESULT on_receive_no_recorded_data(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_no_recorded_data(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_record_failover_scope(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FAILOVER_SCOPE_INFO_LIST data = (G2FAILOVER_SCOPE_INFO_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FAILOVER_SCOPE_INFO_LIST));
            G2FAILOVER_SCOPE_INFO[] bunch = data.to();
            _listener.on_g2play_receive_record_failover_scope(handle, (int)wparam, bunch);
            return 1;
        }
        private G2RESULT on_receive_event_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_LOG log = (G2EVENT_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_LOG));
            _listener.on_g2play_receive_event_log_load(handle, (int)wparam, ref log);
            return 1;
        }
        private G2RESULT on_receive_event_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_event_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_event_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_event_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_event_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_event_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT log = (G2EVENT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT));
            _listener.on_g2play_receive_text_in_log_load(handle, (int)wparam, ref log);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_text_in_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_text_in_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_text_in_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_text_in_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_device_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SYSTEM_LOG data = (G2SYSTEM_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SYSTEM_LOG));
            _listener.on_g2play_receive_device_log_load(handle, (int)wparam, ref data);
            return 1;
        }
        private G2RESULT on_receive_device_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_device_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_device_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_device_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_device_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_device_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SYSTEM_LOG data = (G2SYSTEM_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SYSTEM_LOG));
            _listener.on_g2play_receive_service_log_load(handle, (int)wparam, ref data);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_service_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_service_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_service_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_require_prepare_rollback(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_require_prepare_rollback(handle, (int)wparam, Convert.ToBoolean(lparam));
            return 1;
        }
        private G2RESULT on_probe_session_profile(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PROBE_SESSION_PROFILE p = (G2PROBE_SESSION_PROFILE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PROBE_SESSION_PROFILE));
            _listener.on_g2play_probe_session_profile(handle, (int)wparam, ref p);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEBUG_LOG data = (G2DEBUG_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2DEBUG_LOG));
            _listener.on_g2play_receive_debug_log_load(handle, (int)wparam, ref data);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_debug_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_debug_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_receive_debug_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_get_frame_buf_status(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            int avail = (int)Marshal.PtrToStructure((IntPtr)lparam, typeof(int));
            if (_listener != null) _listener.on_g2play_get_frame_buf_status(handle, w._channel, w._tag, ref avail);
            Marshal.StructureToPtr(avail, (IntPtr)lparam, true);
            return 1;
        }
        #endregion
    }
}
