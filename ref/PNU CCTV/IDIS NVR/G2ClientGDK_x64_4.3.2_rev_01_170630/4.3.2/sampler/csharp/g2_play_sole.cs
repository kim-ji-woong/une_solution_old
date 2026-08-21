using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace GDK
{
    using G2HPLAY_SOLE = System.Int32;
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

    public interface g2play_sole_listener
    {
        /// <summary>
        /// @ on_g2play_sole_get_options_search_base
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="options">structure to set to sole base option</param>
        void on_g2play_sole_get_options_search_base(G2HPLAY_SOLE handle, int channel, ref PLAY_SOLE_BASE_OPTIONS options);
        /// <summary>
        /// @ on_g2play_sole_get_options_player
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="options">structure to set to sole player option</param>
        void on_g2play_sole_get_options_player(G2HPLAY_SOLE handle, int channel, ref G2SEARCH_G2_SOLE_PLAYER_OPTIONS options);
        /// <summary>
        /// @ on_g2play_sole_connected
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_sole_connected(G2HPLAY_SOLE handle, int channel);
        /// <summary>
        /// @ on_g2play_sole_disconnected
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="reason">reason for disconnection</param>
        void on_g2play_sole_disconnected(G2HPLAY_SOLE handle, int channel, uint reason);
        /// <summary>
        /// @ on_g2play_sole_probe_frameload
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="ips">ips</param>
        /// <param name="bps">bps</param>
        void on_g2play_sole_probe_frameload(G2HPLAY_SOLE handle, int channel, int ips, int bps);
        /// <summary>
        /// @ on_g2play_sole_command_begin
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="command">playback-related command for recorded images</param>
        void on_g2play_sole_command_begin(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER.COMMAND_AND_SPEED command);
        /// <summary>
        /// @ on_g2play_sole_command_end
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="command">playback-related command for recorded images</param>
        void on_g2play_sole_command_end(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER.COMMAND_AND_SPEED command);
        /// <summary>
        /// @ on_g2play_sole_play_speed_changed
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="speed">playback speed of a recorded image</param>
        void on_g2play_sole_play_speed_changed(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER.COMMAND_AND_SPEED speed);
        /// <summary>
        /// @ on_g2play_sole_frame_loaded
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="frame">recorded image data</param>
        void on_g2play_sole_frame_loaded(G2HPLAY_SOLE handle, int channel, int tag, ref G2FRAME frame);
        /// <summary>
        /// @ on_g2play_sole_frame_not_found
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="spot">requested time information</param>
        /// <param name="precision">precision</param>
        void on_g2play_sole_frame_not_found(G2HPLAY_SOLE handle, int channel, int tag, ref G2SPOT spot, G2PLAYER.PRECISION precision);
        /// <summary>
        /// @ on_g2play_sole_out_of_scope
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="status">type</param>
        void on_g2play_sole_out_of_scope(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER.OUT_OF_SCOPE status);
        /// <summary>
        /// @ on_g2play_sole_player_set_scope
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="scope">search scope</param>
        void on_g2play_sole_player_set_scope(G2HPLAY_SOLE handle, int channel, int tag, ref G2SCOPE scope);
        /// <summary>
        /// @ on_g2play_sole_player_error
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="error">type</param>
        void on_g2play_sole_player_error(G2HPLAY_SOLE handle, int channel, int tag, G2PLAYER.PLAYER_ERROR error);
        /// <summary>
        /// @ on_g2play_sole_get_rollback_info
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="rbi">structure wherein rollback information is saved</param>
        void on_g2play_sole_get_rollback_info(G2HPLAY_SOLE handle, int channel, int tag, ref G2ROLLBACK_INFO rbi);
        /// <summary>
        /// @ on_g2play_sole_get_frame_buf_status
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="avail">avail</param>
        void on_g2play_sole_get_frame_buf_status(G2HPLAY_SOLE handle, int channel, int tag, ref int avail);
        /// <summary>
        /// @ on_g2play_sole_receive_text_in
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="tie">text-in data</param>
        void on_g2play_sole_receive_text_in(G2HPLAY_SOLE handle, int channel, int tag, ref G2TEXT_IN tie);
        /// <summary>
        /// @ on_g2play_sole_receive_snapshot_frame
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="frame">recorded image data</param>
        void on_g2play_sole_receive_snapshot_frame(G2HPLAY_SOLE handle, int channel, int tag, long id, ref G2FRAME frame);
        /// <summary>
        /// @ on_g2play_sole_receive_snapshot_begin
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        void on_g2play_sole_receive_snapshot_begin(G2HPLAY_SOLE handle, int channel, int tag, long id);
        /// <summary>
        /// @ on_g2play_sole_receive_snapshot_end
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="reason">reason</param>
        void on_g2play_sole_receive_snapshot_end(G2HPLAY_SOLE handle, int channel, int tag, long id, G2SEARCH_G2_SOLE_SNAPSHOP_LOADER.FINISH_REASON reason);
        /// <summary>
        /// @ on_g2play_sole_receive_record_channel
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="ch">channel</param>
        void on_g2play_sole_receive_record_channel(G2HPLAY_SOLE handle, int channel, int tag, int ch);
        /// <summary>
        /// @ on_g2play_sole_receive_record_time_info_loaded
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="rti">recording time information</param>
        void on_g2play_sole_receive_record_time_info_loaded(G2HPLAY_SOLE handle, int channel, int tag, int id, ref G2RECORD_TIME_INFO rti);
        /// <summary>
        /// @ on_g2play_sole_receive_record_time_info_load_end
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="reason">reason</param>
        void on_g2play_sole_receive_record_time_info_load_end(G2HPLAY_SOLE handle, int channel, int tag, int id, int reason);
        /// <summary>
        /// @ on_g2play_sole_receive_frame_channelset
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="chs">channel set</param>
        void on_g2play_sole_receive_frame_channelset(G2HPLAY_SOLE handle, int channel, int tag, int id, ref g2channel_set chs);
        /// <summary>
        /// @ on_g2play_sole_receive_spot_list
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="spots">recording time information</param>
        void on_g2play_sole_receive_spot_list(G2HPLAY_SOLE handle, int channel, int tag, int id, G2SPOT[] spots);
        /// <summary>
        /// @ on_g2play_sole_receive_scope_list
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="scopes">recording time scope list</param>
        void on_g2play_sole_receive_scope_list(G2HPLAY_SOLE handle, int channel, int tag, int id, G2SCOPE[] scopes);
        /// <summary>
        /// @ on_g2play_sole_receive_recorded_scope
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="chs">channel set</param>
        /// <param name="scope">recording time scope</param>
        void on_g2play_sole_receive_recorded_scope(G2HPLAY_SOLE handle, int channel, int tag, int id, ref g2channel_set chs, ref G2SCOPE scope);
        /// <summary>
        /// @ on_g2play_sole_receive_frame_spot_list
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_play control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="spots">recording time information</param>
        void on_g2play_sole_receive_frame_spot_list(G2HPLAY_SOLE handle, int channel, int tag, int id, G2SPOT[] spots);
    }

    public class g2play_sole
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_sole_register_callback(G2HPLAY_SOLE handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HPLAY_SOLE g2_play_sole_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_sole_finalize(G2HPLAY_SOLE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_startup(G2HPLAY_SOLE handle, int panes);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern void g2_play_sole_cleanup(G2HPLAY_SOLE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_connect(G2HPLAY_SOLE handle, ref G2GUID site, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_disconnect_with_channel(G2HPLAY_SOLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_disconnect(G2HPLAY_SOLE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_is_connecting(G2HPLAY_SOLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_is_connected(G2HPLAY_SOLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_is_disconnecting(G2HPLAY_SOLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_is_disconnected(G2HPLAY_SOLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_is_disconnected_not_any(G2HPLAY_SOLE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_is_disconnectable(G2HPLAY_SOLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_search_base_cleanup(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_record_channel(G2HPLAY_SOLE handle, int channel, int tag, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_record_time_info(G2HPLAY_SOLE handle, int channel, int tag, int id, int resolution, ref G2CHANNEL_SET chs, ref G2SCOPE scope, int direction);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_record_time_info_load_stop(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_set_player_scope(G2HPLAY_SOLE handle, int channel, int tag, ref G2SCOPE scope);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_set_camera_list(G2HPLAY_SOLE handle, int channel, ref PLAY_SOLE_CHANNEL_MAP chs);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_play(G2HPLAY_SOLE handle, int channel, int tag, int speed);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_play_with_command(G2HPLAY_SOLE handle, int channel, int tag, ref G2PLAYBACK_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_play_audio(G2HPLAY_SOLE handle, int channel, int tag, int audio);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_pause(G2HPLAY_SOLE handle, int channel, int tag, ref G2ROLLBACK_INFO rbi, bool rollback);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_stop(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_move_to_first(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_move_to_last(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_move_to_spot(G2HPLAY_SOLE handle, int channel, int tag, ref G2SPOT spot, int precision, bool forward, bool discard_if_duplicated_command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_step_prev(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_step_next(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_notify_end_of_play(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_snapshot(G2HPLAY_SOLE handle, int channel, int tag, ref G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION_LIST options);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_snapshot_cancel(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_frame_channelset(G2HPLAY_SOLE handle, int channel, int tag, int id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_spot_list(G2HPLAY_SOLE handle, int channel, int tag, int id, int play_channel, ref G2TIME time, bool load_adjacent_frame);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_scope_list(G2HPLAY_SOLE handle, int channel, int tag, int id, int play_channel, ref G2TIME from, ref G2TIME to);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_recorded_scope(G2HPLAY_SOLE handle, int channel, int tag, int id, int play_channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_request_frame_spot_list(G2HPLAY_SOLE handle, int channel, int tag, int id, int play_channel, ref G2SCOPE scope, ref bool fcontinue);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_get_frame_from(G2HPLAY_SOLE handle, out int from);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_is_player_stopped_with_tag(G2HPLAY_SOLE handle, int channel, int tag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_is_player_stopped(G2HPLAY_SOLE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_is_started(G2HPLAY_SOLE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_sole_channel_from_site(G2HPLAY_SOLE handle, ref G2GUID site, out int channel);
        #endregion

        public g2play_sole()
        {
            _handle = 0;
            _param = new G2UPARAM(0);
            _listener = null;
            _p2_on_get_options_search_base = new G2FUN_LISTENER(on_get_options_search_base);
            _p2_on_get_options_player = new G2FUN_LISTENER(on_get_options_player);
            _p2_on_connected = new G2FUN_LISTENER(on_connected);
            _p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            _p2_on_probe_frameload = new G2FUN_LISTENER(on_probe_frameload);
            _p2_on_command_begin = new G2FUN_LISTENER(on_command_begin);
            _p2_on_command_end = new G2FUN_LISTENER(on_command_end);
            _p2_on_play_speed_changed = new G2FUN_LISTENER(on_play_speed_changed);
            _p2_on_frame_loaded = new G2FUN_LISTENER(on_frame_loaded);
            _p2_on_frame_not_found = new G2FUN_LISTENER(on_frame_not_found);
            _p2_on_out_of_scope = new G2FUN_LISTENER(on_out_of_scope);
            _p2_on_player_set_scope = new G2FUN_LISTENER(on_player_set_scope);
            _p2_on_player_error = new G2FUN_LISTENER(on_player_error);
            _p2_on_get_rollback_info = new G2FUN_LISTENER(on_get_rollback_info);
            _p2_on_get_frame_buf_status = new G2FUN_LISTENER(on_get_frame_buf_status);
            _p2_on_receive_text_in = new G2FUN_LISTENER(on_receive_text_in);
            _p2_on_receive_snapshot_frame = new G2FUN_LISTENER(on_receive_snapshot_frame);
            _p2_on_receive_snapshot_begin = new G2FUN_LISTENER(on_receive_snapshot_begin);
            _p2_on_receive_snapshot_end = new G2FUN_LISTENER(on_receive_snapshot_end);
            _p2_on_receive_record_channel = new G2FUN_LISTENER(on_receive_record_channel);
            _p2_on_receive_record_time_info_loaded = new G2FUN_LISTENER(on_receive_record_time_info_loaded);
            _p2_on_receive_record_time_info_load_end = new G2FUN_LISTENER(on_receive_record_time_info_load_end);
            _p2_on_receive_frame_channelset = new G2FUN_LISTENER(on_receive_frame_channelset);
            _p2_on_receive_spot_list = new G2FUN_LISTENER(on_receive_spot_list);
            _p2_on_receive_scope_list = new G2FUN_LISTENER(on_receive_scope_list);
            _p2_on_receive_recorded_scope = new G2FUN_LISTENER(on_receive_recorded_scope);
            _p2_on_receive_frame_spot_list = new G2FUN_LISTENER(on_receive_frame_spot_list);
        }
        ~g2play_sole()
        {
            cleanup();
        }

        private G2HPLAY_SOLE _handle;
        private G2UPARAM _param;
        private g2play_sole_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2PLAY_SOLE_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_play_sole_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_get_options_search_base;
        private G2FUN_LISTENER _p2_on_get_options_player;
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_probe_frameload;
        private G2FUN_LISTENER _p2_on_command_begin;
        private G2FUN_LISTENER _p2_on_command_end;
        private G2FUN_LISTENER _p2_on_play_speed_changed;
        private G2FUN_LISTENER _p2_on_frame_loaded;
        private G2FUN_LISTENER _p2_on_frame_not_found;
        private G2FUN_LISTENER _p2_on_out_of_scope;
        private G2FUN_LISTENER _p2_on_player_set_scope;
        private G2FUN_LISTENER _p2_on_player_error;
        private G2FUN_LISTENER _p2_on_get_rollback_info;
        private G2FUN_LISTENER _p2_on_get_frame_buf_status;
        private G2FUN_LISTENER _p2_on_receive_text_in;
        private G2FUN_LISTENER _p2_on_receive_snapshot_frame;
        private G2FUN_LISTENER _p2_on_receive_snapshot_begin;
        private G2FUN_LISTENER _p2_on_receive_snapshot_end;
        private G2FUN_LISTENER _p2_on_receive_record_channel;
        private G2FUN_LISTENER _p2_on_receive_record_time_info_loaded;
        private G2FUN_LISTENER _p2_on_receive_record_time_info_load_end;
        private G2FUN_LISTENER _p2_on_receive_frame_channelset;
        private G2FUN_LISTENER _p2_on_receive_spot_list;
        private G2FUN_LISTENER _p2_on_receive_scope_list;
        private G2FUN_LISTENER _p2_on_receive_recorded_scope;
        private G2FUN_LISTENER _p2_on_receive_frame_spot_list;
        #endregion

        public G2HPLAY_SOLE safe_handle() { return _handle; }

        /// <summary>
        /// @ startup
        /// <para></para>
        /// </summary>
        /// <param name="connections">no. of channels to be connected simultaneously to the recording service</param>
        /// <returns>If a startup succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool startup(int connections)
        {
            cleanup();

            _handle = g2_play_sole_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_get_options_search_base, _p2_on_get_options_search_base);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_get_options_player, _p2_on_get_options_player);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_probe_frameload, _p2_on_probe_frameload);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_command_begin, _p2_on_command_begin);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_command_end, _p2_on_command_end);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_play_speed_changed, _p2_on_play_speed_changed);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_frame_loaded, _p2_on_frame_loaded);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_frame_not_found, _p2_on_frame_not_found);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_out_of_scope, _p2_on_out_of_scope);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_player_set_scope, _p2_on_player_set_scope);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_player_error, _p2_on_player_error);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_get_rollback_info, _p2_on_get_rollback_info);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_get_frame_buf_status, _p2_on_get_frame_buf_status);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_text_in, _p2_on_receive_text_in);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_snapshot_frame, _p2_on_receive_snapshot_frame);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_snapshot_begin, _p2_on_receive_snapshot_begin);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_snapshot_end, _p2_on_receive_snapshot_end);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_record_channel, _p2_on_receive_record_channel);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_record_time_info_loaded, _p2_on_receive_record_time_info_loaded);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_record_time_info_load_end, _p2_on_receive_record_time_info_load_end);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_frame_channelset, _p2_on_receive_frame_channelset);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_spot_list, _p2_on_receive_spot_list);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_scope_list, _p2_on_receive_scope_list);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_recorded_scope, _p2_on_receive_recorded_scope);
            register_callback(G2PLAY_SOLE_CALLBACK.TYPE.on_receive_frame_spot_list, _p2_on_receive_frame_spot_list);
            #endregion

            return g2_play_sole_startup(_handle, connections);
        }

        /// <summary>
        /// @ cleanup
        /// <para></para>
        /// </summary>
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HPLAY_SOLE handle = _handle; _handle = 0;
                g2_play_sole_cleanup(handle);
                g2_play_sole_finalize(handle);
            }
        }

        /// <summary>
        /// @ set_listener
        /// <para></para>
        /// </summary>
        /// <param name="listener">listener function</param>
        public void set_listener(g2play_sole_listener listener)
        {
            _listener = listener;
        }

        /// <summary>
        /// @ connect
        /// <para></para>
        /// </summary>
        /// <param name="site">site</param>
        /// <param name="options">The structure wherein the options for service connection is saved. It has information about connection time out.</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connect succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool connect(ref G2GUID site, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_play_sole_connect(_handle, ref site, ref options, out res);
        }

        /// <summary>
        /// @ disconnect
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a disconnect succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool disconnect(int channel)
        {
            return g2_play_sole_disconnect_with_channel(_handle, channel);
        }

        /// <summary>
        /// @ disconnect
        /// <para></para>
        /// </summary>
        /// <returns>If a disconnect succeeds, it returns boolean TRUE; otherwise, it returns FALSE.</returns>
        public bool disconnect()
        {
            return g2_play_sole_disconnect(_handle);
        }

        /// <summary>
        /// @ is_connecting
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connecting(int channel)
        {
            return g2_play_sole_is_connecting(_handle, channel);
        }

        /// <summary>
        /// @ is_connected
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If connected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connected(int channel)
        {
            return g2_play_sole_is_connected(_handle, channel);
        }

        /// <summary>
        /// @ is_disconnecting
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If in the process of disconnecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnecting(int channel)
        {
            return g2_play_sole_is_disconnecting(_handle, channel);
        }

        /// <summary>
        /// @ is_disconnected
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If disconnected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnected(int channel)
        {
            return g2_play_sole_is_disconnected(_handle, channel);
        }

        /// <summary>
        /// @ is_disconnected_not_any
        /// <para></para>
        /// </summary>
        /// <returns>If disconnected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnected_not_any()
        {
            return g2_play_sole_is_disconnected_not_any(_handle);
        }

        /// <summary>
        /// @ is_disconnectable
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If connected or in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnectable(int channel)
        {
            return g2_play_sole_is_disconnectable(_handle, channel);
        }

        /// <summary>
        /// @ request_search_base_cleanup
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_search_base_cleanup(int channel, int tag)
        {
            return g2_play_sole_request_search_base_cleanup(_handle, channel, tag);
        }

        /// <summary>
        /// @ request_record_channel
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="camera">camera unique id</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_record_channel(int channel, int tag, ref G2GUID camera)
        {
            return g2_play_sole_request_record_channel(_handle, channel, tag, ref camera);
        }

        /// <summary>
        /// @ request_record_time_info
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="resolution">time information resolution</param>
        /// <param name="channelset">camera channel list</param>
        /// <param name="scope">search scope</param>
        /// <param name="direction">search direction
        /// : For backward (to the left) search, -1; for forward search (to the right), 1</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_record_time_info(int channel, int tag, int id, int resolution, g2channel_set channelset, ref G2SCOPE scope, int direction)
        {
            G2CHANNEL_SET chs = channelset;
            return g2_play_sole_request_record_time_info(_handle, channel, tag, id, resolution, ref chs, ref scope, direction);
        }

        /// <summary>
        /// @ request_record_time_info_load_stop
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_record_time_info_load_stop(int channel, int tag)
        {
            return g2_play_sole_request_record_time_info_load_stop(_handle, channel, tag);
        }

        /// <summary>
        /// @ request_set_player_scope
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="scope">search scope</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_set_player_scope(int channel, int tag, ref G2SCOPE scope)
        {
            return g2_play_sole_request_set_player_scope(_handle, channel, tag, ref scope);
        }

        /// <summary>
        /// @ request_set_camera_list
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="chs">channel set</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_set_camera_list(int channel, ref PLAY_SOLE_CHANNEL_MAP chs)
        {
            return g2_play_sole_request_set_camera_list(_handle, channel, ref chs);
        }

        /// <summary>
        /// @ request_play
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="speed">playback speed of a recorded image</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_play(int channel, int tag, int speed)
        {
            return g2_play_sole_request_play(_handle, channel, tag, speed);
        }

        /// <summary>
        /// @ request_play
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="command">structure wherein playback command information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_play(int channel, int tag, ref G2PLAYBACK_COMMAND command)
        {
            return g2_play_sole_request_play_with_command(_handle, channel, tag, ref command);
        }

        /// <summary>
        /// @ request_play_audio
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="audio">audio</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_play_audio(int channel, int tag, int audio)
        {
            return g2_play_sole_request_play_audio(_handle, channel, tag, audio);
        }

        /// <summary>
        /// @ request_pause
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="rbi"></param>
        /// <param name="rollback">whether to do rollback</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_pause(int channel, int tag, ref G2ROLLBACK_INFO rbi, bool rollback)
        {
            return g2_play_sole_request_pause(_handle, channel, tag, ref rbi, rollback);
        }

        /// <summary>
        /// @ request_stop
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_stop(int channel, int tag)
        {
            return g2_play_sole_request_stop(_handle, channel, tag);
        }

        /// <summary>
        /// @ request_move_to_first
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_move_to_first(int channel, int tag)
        {
            return g2_play_sole_request_move_to_first(_handle, channel, tag);
        }

        /// <summary>
        /// @ request_move_to_last
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_move_to_last(int channel, int tag)
        {
            return g2_play_sole_request_move_to_last(_handle, channel, tag);
        }

        /// <summary>
        /// @ request_move_to_spot
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="spot">requested time information</param>
        /// <param name="precision">requested time information</param>
        /// <param name="forward">TRUE for forward (to the right) search; FALSE for backward (to the left) search</param>
        /// <param name="discard_if_duplicated_command">discard if duplicated command</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_move_to_spot(int channel, int tag, ref G2SPOT spot, int precision, bool forward, bool discard_if_duplicated_command)
        {
            return g2_play_sole_request_move_to_spot(_handle, channel, tag, ref spot, precision, forward, discard_if_duplicated_command);
        }

        /// <summary>
        /// @ request_step_prev
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_step_prev(int channel, int tag)
        {
            return g2_play_sole_request_step_prev(_handle, channel, tag);
        }

        /// <summary>
        /// @ request_step_next
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_step_next(int channel, int tag)
        {
            return g2_play_sole_request_step_next(_handle, channel, tag);
        }

        /// <summary>
        /// @ request_notify_end_of_play
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_notify_end_of_play(int channel, int tag)
        {
            return g2_play_sole_request_notify_end_of_play(_handle, channel, tag);
        }

        /// <summary>
        /// @ request_snapshot
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="options">options</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_snapshot(int channel, int tag, ref G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION_LIST options)
        {
            return g2_play_sole_request_snapshot(_handle, channel, tag, ref options);
        }

        /// <summary>
        /// @ request_snapshot_cancel
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_snapshot_cancel(int channel, int tag)
        {
            return g2_play_sole_request_snapshot_cancel(_handle, channel, tag);
        }

        /// <summary>
        /// @ request_frame_channelset
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_frame_channelset(int channel, int tag, int id)
        {
            return g2_play_sole_request_frame_channelset(_handle, channel, tag, id);
        }

        /// <summary>
        /// @ request_spot_list
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="play_channel">play channel</param>
        /// <param name="time">time</param>
        /// <param name="load_adjacent_frame">load adjacent frame</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_spot_list(int channel, int tag, int id, int play_channel, ref G2TIME time, bool load_adjacent_frame)
        {
            return g2_play_sole_request_spot_list(_handle, channel, tag, id, play_channel, ref time, load_adjacent_frame);
        }

        /// <summary>
        /// @ request_scope_list
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="play_channel">play channel</param>
        /// <param name="from">from</param>
        /// <param name="to">to</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_scope_list(int channel, int tag, int id, int play_channel, ref G2TIME from, ref G2TIME to)
        {
            return g2_play_sole_request_scope_list(_handle, channel, tag, id, play_channel, ref from, ref to);
        }

        /// <summary>
        /// @ request_recorded_scope
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="play_channel">play channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_recorded_scope(int channel, int tag, int id, int play_channel)
        {
            return g2_play_sole_request_recorded_scope(_handle, channel, tag, id, play_channel);
        }

        /// <summary>
        /// @ request_frame_spot_list
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <param name="id">id</param>
        /// <param name="play_channel">play channel</param>
        /// <param name="scope">search scope</param>
        /// <param name="fcontinue">fcontinue</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_frame_spot_list(int channel, int tag, int id, int play_channel, ref G2SCOPE scope, ref bool fcontinue)
        {
            return g2_play_sole_request_frame_spot_list(_handle, channel, tag, id, play_channel, ref scope, ref fcontinue);
        }

        /// <summary>
        /// @ get_frame_from
        /// <para></para>
        /// </summary>
        /// <param name="from">from</param>
        /// <returns></returns>
        public bool get_frame_from(out int from)
        {
            return g2_play_sole_get_frame_from(_handle, out from);
        }

        /// <summary>
        /// @ is_player_stopped
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="tag">tag</param>
        /// <returns></returns>
        public bool is_player_stopped(int channel, int tag)
        {
            return g2_play_sole_is_player_stopped_with_tag(_handle, channel, tag);
        }

        /// <summary>
        /// @ is_player_stopped
        /// <para></para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If in playback stop status, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_player_stopped(int channel)
        {
            return g2_play_sole_is_player_stopped(_handle, channel);
        }

        /// <summary>
        /// @ is_started
        /// <para></para>
        /// </summary>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_started()
        {
            return g2_play_sole_is_started(_handle);
        }

        /// <summary>
        /// @ channel_from_site
        /// <para></para>
        /// </summary>
        /// <param name="site">site</param>
        /// <param name="channel">recording service channel</param>
        /// <returns>If successful, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool channel_from_site(ref G2GUID site, out int channel)
        {
            return g2_play_sole_channel_from_site(_handle, ref site, out channel);
        }

        #region GDK Callback Handler
        private G2RESULT on_get_options_search_base(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            PLAY_SOLE_BASE_OPTIONS option = (PLAY_SOLE_BASE_OPTIONS)Marshal.PtrToStructure((IntPtr)lparam, typeof(PLAY_SOLE_BASE_OPTIONS));
            if (_listener != null) _listener.on_g2play_sole_get_options_search_base(handle, (int)wparam, ref option);
            Marshal.StructureToPtr(option, (IntPtr)lparam, false);
            return 1; 
        }
        private G2RESULT on_get_options_player(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_PLAYER_OPTIONS options = (G2SEARCH_G2_SOLE_PLAYER_OPTIONS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_SOLE_PLAYER_OPTIONS));
            if (_listener != null) _listener.on_g2play_sole_get_options_player(handle, (int)wparam, ref options);
            Marshal.StructureToPtr(options, (IntPtr)lparam, true);
            return 1;
        }
        private G2RESULT on_connected(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener != null) _listener.on_g2play_sole_connected(handle, (int)wparam); 
            return 1;
        }
        private G2RESULT on_disconnected(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener != null) _listener.on_g2play_sole_disconnected(handle, (int)wparam, (uint)lparam); 
            return 1;
        }
        private G2RESULT on_probe_frameload(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_PROBE_FRAMELOAD l = (G2SEARCH_G2_SOLE_PROBE_FRAMELOAD)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_SOLE_PROBE_FRAMELOAD));
            if (_listener != null) _listener.on_g2play_sole_probe_frameload(handle, (int)wparam, l._ips, l._bps); 
            return 1;
        }
        private G2RESULT on_command_begin(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            if (_listener != null) _listener.on_g2play_sole_command_begin(handle, w._channel, w._tag, (G2PLAYER.COMMAND_AND_SPEED)lparam); 
            return 1;
        }
        private G2RESULT on_command_end(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            if (_listener != null) _listener.on_g2play_sole_command_end(handle, w._channel, w._tag, (G2PLAYER.COMMAND_AND_SPEED)lparam); 
            return 1;
        }
        private G2RESULT on_play_speed_changed(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            if (_listener != null) _listener.on_g2play_sole_play_speed_changed(handle, w._channel, w._tag, (G2PLAYER.COMMAND_AND_SPEED)lparam); 
            return 1;
        }
        private G2RESULT on_frame_loaded(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            if (_listener != null) _listener.on_g2play_sole_frame_loaded(handle, w._channel, w._tag, ref frame); 
            return 1;
        }
        private G2RESULT on_frame_not_found(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            G2SPOT_PRECISION l = (G2SPOT_PRECISION)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_PRECISION));
            if (_listener != null) _listener.on_g2play_sole_frame_not_found(handle, w._channel, w._tag, ref l._spot, (G2PLAYER.PRECISION)l._precision); 
            return 1;
        }
        private G2RESULT on_out_of_scope(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            if (_listener != null) _listener.on_g2play_sole_out_of_scope(handle, w._channel, w._tag, (G2PLAYER.OUT_OF_SCOPE)lparam); 
            return 1;
        }
        private G2RESULT on_player_set_scope(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            G2SCOPE scope = (G2SCOPE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SCOPE));
            if (_listener != null) _listener.on_g2play_sole_player_set_scope(handle, w._channel, w._tag, ref scope); 
            return 1;
        }
        private G2RESULT on_player_error(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            if (_listener != null) _listener.on_g2play_sole_player_error(handle, w._channel, w._tag, (G2PLAYER.PLAYER_ERROR)lparam); 
            return 1;
        }
        private G2RESULT on_get_rollback_info(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            G2ROLLBACK_INFO rbi = (G2ROLLBACK_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2ROLLBACK_INFO));
            if (_listener != null) _listener.on_g2play_sole_get_rollback_info(handle, w._channel, w._tag, ref rbi);
            Marshal.StructureToPtr(rbi, (IntPtr)lparam, true);
            return 1;
        }
        private G2RESULT on_get_frame_buf_status(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            int avail = (int)Marshal.PtrToStructure((IntPtr)lparam, typeof(int));
            if (_listener != null) _listener.on_g2play_sole_get_frame_buf_status(handle, w._channel, w._tag, ref avail);
            Marshal.StructureToPtr(avail, (IntPtr)lparam, true);
            return 1;
        }
        private G2RESULT on_receive_text_in(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            G2TEXT_IN tie = (G2TEXT_IN)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2TEXT_IN));
            if (_listener != null) _listener.on_g2play_sole_receive_text_in(handle, w._channel, w._tag, ref tie); 
            return 1;
        }
        private G2RESULT on_receive_snapshot_frame(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            if (_listener != null) _listener.on_g2play_sole_receive_snapshot_frame(handle, w._channel, w._tag, w._id, ref frame); 
            return 1;
        }
        private G2RESULT on_receive_snapshot_begin(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            if (_listener != null) _listener.on_g2play_sole_receive_snapshot_begin(handle, w._channel, w._tag, w._id); 
            return 1;
        }
        private G2RESULT on_receive_snapshot_end(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            if (_listener != null) _listener.on_g2play_sole_receive_snapshot_end(handle, w._channel, w._tag, w._id, (G2SEARCH_G2_SOLE_SNAPSHOP_LOADER.FINISH_REASON)lparam); 
            return 1; 
        }
        private G2RESULT on_receive_record_channel(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG w = (G2SEARCH_G2_SOLE_CHANNEL_TAG)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG));
            if (_listener != null) _listener.on_g2play_sole_receive_record_channel(handle, w._channel, w._tag, (int)lparam); 
            return 1;
        }
        private G2RESULT on_receive_record_time_info_loaded(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            G2RECORD_TIME_INFO rti = (G2RECORD_TIME_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2RECORD_TIME_INFO));
            if (_listener != null) _listener.on_g2play_sole_receive_record_time_info_loaded(handle, w._channel, w._tag, (int)w._id, ref rti); 
            return 1;
        }
        private G2RESULT on_receive_record_time_info_load_end(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            if (_listener != null) _listener.on_g2play_sole_receive_record_time_info_load_end(handle, w._channel, w._tag, (int)w._id, (int)lparam); 
            return 1;
        }
        private G2RESULT on_receive_frame_channelset(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            G2CHANNEL_SET channel_set = (G2CHANNEL_SET)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CHANNEL_SET));
            g2channel_set chs = channel_set;
            if (_listener != null) _listener.on_g2play_sole_receive_frame_channelset(handle, w._channel, w._tag, (int)w._id, ref chs); 
            return 1;
        }
        private G2RESULT on_receive_spot_list(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            G2SPOT_LIST data = (G2SPOT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_LIST));
            G2SPOT[] bunch = data.to();
            if (_listener != null) _listener.on_g2play_sole_receive_spot_list(handle, w._channel, w._tag, (int)w._id, bunch); 
            return 1;
        }
        private G2RESULT on_receive_scope_list(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            G2SCOPE_LIST data = (G2SCOPE_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SCOPE_LIST));
            G2SCOPE[] bunch = data.to();
            
            if (_listener != null) _listener.on_g2play_sole_receive_scope_list(handle, w._channel, w._tag, (int)w._id, bunch); 
            return 1;
        }
        private G2RESULT on_receive_recorded_scope(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            G2SEARCH_G2_SOLE_RECORDED_SCOPE l = (G2SEARCH_G2_SOLE_RECORDED_SCOPE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_G2_SOLE_RECORDED_SCOPE));
            g2channel_set chs = l._channels;
            if (_listener != null) _listener.on_g2play_sole_receive_recorded_scope(handle, w._channel, w._tag, (int)w._id, ref chs, ref l._scope); 
            return 1;
        }
        private G2RESULT on_receive_frame_spot_list(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_G2_SOLE_CHANNEL_TAG_ID w = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID)Marshal.PtrToStructure((IntPtr)wparam, typeof(G2SEARCH_G2_SOLE_CHANNEL_TAG_ID));
            G2SPOT_LIST data = (G2SPOT_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SPOT_LIST));
            G2SPOT[] bunch = data.to();
            if (_listener != null) _listener.on_g2play_sole_receive_frame_spot_list(handle, w._channel, w._tag, (int)w._id, bunch); 
            return 1;
        }
        #endregion
    }
}
