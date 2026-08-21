using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HLIVE = System.Int32;
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

    public interface g2live_listener
    {
        /// <summary>
        /// @ on_g2live_connected
        /// <para>callback when connected to the streaming service</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2live_connected(G2HLIVE handle, int channel);
        /// <summary>
        /// @ on_g2live_disconnected
        /// <para>callback when disconnected from the streaming service</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="reason">reason for disconnection</param>
        void on_g2live_disconnected(G2HLIVE handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        /// <summary>
        /// @ on_g2live_receive_stream_channels
        /// <para>callback for the channel information of the device registered to the streaming service</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="chs">device channel information list</param>
        void on_g2live_receive_stream_channels(G2HLIVE handle, int channel, Dictionary<G2GUID, G2LIVE_CHANNEL_INFO> chs);
        /// <summary>
        /// @ on_g2live_receive_frame_data
        /// <para>real-time image data callback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="frame">image data</param>
        void on_g2live_receive_frame_data(G2HLIVE handle, int channel, ref G2FRAME frame);
        /// <summary>
        /// @ on_g2live_receive_text_in
        /// <para>text-in data callback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="data">text-in data </param>
        void on_g2live_receive_text_in(G2HLIVE handle, int channel, ref G2TEXT_IN data);
        /// <summary>
        /// @ on_g2live_receive_camera_status
        /// <para>camera status information callback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="bunch">camera status information list</param>
        void on_g2live_receive_camera_status(G2HLIVE handle, int channel, G2LIVE_CAMERA_STATUS[] bunch);
        /// <summary>
        /// @ on_g2live_receive_event
        /// <para>real-time event callback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="ei">event information</param>
        void on_g2live_receive_event(G2HLIVE handle, int channel, ref G2EVENT_INFO ei);
        /// <summary>
        /// @ on_g2live_receive_ptz_menu
        /// <para>device PTZ menu information callback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="menu">PTZ menu information</param>
        void on_g2live_receive_ptz_menu(G2HLIVE handle, int channel, ref G2LIVE_PTZ_MENU menu);
        /// <summary>
        /// @ on_g2live_receive_ptz_preset
        /// <para>device PTZ preset information callback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="preset">PTZ preset information</param>
        void on_g2live_receive_ptz_preset(G2HLIVE handle, int channel, ref G2LIVE_PTZ_PRESET preset);
        /// <summary>
        /// @ on_g2live_receive_service_log_load
        /// <para>callback for the search for the streaming service system log</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="log">system log information</param>
        void on_g2live_receive_service_log_load(G2HLIVE handle, int channel, ref G2SYSTEM_LOG log);
        /// <summary>
        /// @ on_g2live_receive_service_log_load_end
        /// <para>callback for the end of the search for the streaming service system log</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2live_receive_service_log_load_end(G2HLIVE handle, int channel);
        /// <summary>
        /// @ on_g2live_receive_service_log_load_fail
        /// <para>callback for the failure of the search for the streaming service system log</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2live_receive_service_log_load_fail(G2HLIVE handle, int channel);
        /// <summary>
        /// @ on_g2live_receive_service_log_load_stop
        /// <para>callback for the stop of the search for the streaming service system log</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2live_receive_service_log_load_stop(G2HLIVE handle, int channel);
        /// <summary>
        /// @ on_g2live_receive_audio_out_not_available
        /// <para>callback when the mic input feature is not available</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="root">device unique id</param>
        void on_g2live_receive_audio_out_not_available(G2HLIVE handle, int channel, ref G2GUID root);
        /// <summary>
        /// @ on_g2live_receive_notify_append_device
        /// <para>callback when a device is appended to the streaming service</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="root">device unique id</param>
        void on_g2live_receive_notify_append_device(G2HLIVE handle, int channel, ref G2GUID root);
        /// <summary>
        /// @ on_g2live_receive_notify_remove_device
        /// <para>callback when a device is deleted from the streaming service</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="root">device unique id</param>
        void on_g2live_receive_notify_remove_device(G2HLIVE handle, int channel, ref G2GUID root);

        /// <summary>
        /// @ on_g2live_receive_network_alarm_result
        /// <para></para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channe</param>
        /// <param name="root">device unique id</param>
        /// <param name="result">structure to save the network alarm result</param>
        void on_g2live_receive_network_alarm_result(G2HLIVE handle, int channel, ref G2GUID root, ref G2LIVE_NETWORK_ALARM_RESULT result);
        /// <summary>
        /// @ on_g2live_sound_streaming_started
        /// <para>audio play start callback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="channelext">device channel</param>
        void on_g2live_sound_streaming_started(G2HLIVE handle, int channel, int channelext);
        /// <summary>
        /// @ on_g2live_sound_streaming_stopped
        /// <para>callback for the stop of audio playback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="channelext">device channel</param>
        void on_g2live_sound_streaming_stopped(G2HLIVE handle, int channel, int channelext);
        /// <summary>
        /// @ on_g2live_sound_capturing_started
        /// <para>mic input feature start callback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="root">device unique id</param>
        /// <param name="camera">camera number</param>               
        void on_g2live_sound_capturing_started(G2HLIVE handle, int channel, ref G2GUID root, int camera);
        /// <summary>
        /// @ on_g2live_sound_capturing_stopped
        /// <para>mic input feature stop callback</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="root">device unique id</param>
        /// <param name="camera">camera number</param>               
        void on_g2live_sound_capturing_stopped(G2HLIVE handle, int channel, ref G2GUID root, int camera);
        /// <summary>
        /// @ on_g2live_probe_session_profile
        /// <para>callback for request probe session profile</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="probe">structure to save the prove session status for profiling</param>
        void on_g2live_probe_session_profile(G2HLIVE handle, int channel, ref G2PROBE_SESSION_PROFILE probe);
        /// <summary>
        /// @ on_g2live_receive_debug_log_load
        /// <para>callback for the search for the streaming service debug log</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="log">debug log information</param>
        void on_g2live_receive_debug_log_load(G2HLIVE handle, int channel, ref G2DEBUG_LOG log);
        /// <summary>
        /// @ on_g2live_receive_debug_log_load_end
        /// <para>callback for the end of the search for the streaming service debug log</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2live_receive_debug_log_load_end(G2HLIVE handle, int channel);
        /// <summary>
        /// @ on_g2live_receive_debug_log_load_fail
        /// <para> callback for the failure of the search for the streaming service debug log</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2live_receive_debug_log_load_fail(G2HLIVE handle, int channel);
        /// <summary>
        /// @ on_g2live_receive_debug_log_load_stop
        /// <para>callback for the stop of the search for the streaming service debug log</para>
        /// </summary>
        /// <param name="handle">g2_live control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2live_receive_debug_log_load_stop(G2HLIVE handle, int channel);
    }

    public class g2live
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_live_register_callback(G2HLIVE handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HLIVE g2_live_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_live_finalize(G2HLIVE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_live_startup(G2HLIVE handle, int connections, G2HWND focus);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_live_cleanup(G2HLIVE handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_live_connect(G2HLIVE handle, ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_live_disconnect(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_connecting(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_connected(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_disconnecting(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_disconnected(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_disconnectable(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_camera_list(G2HLIVE handle, int channel, ref G2CHANNEL_SET channels, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_camera_list_interest(G2HLIVE handle, int channel, ref G2CHANNEL_SET channels, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_camera_stream_set(G2HLIVE handle, int channel, ref G2CHANNEL_STREAM_SET streams, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_camera_stream_channel(G2HLIVE handle, int channel, int channelext, int stream_id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_audio_list(G2HLIVE handle, int channel, ref G2CHANNEL_SET channels, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_stream_id(G2HLIVE handle, int channel, ref G2GUID camera, int id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_alarm_out(G2HLIVE handle, int channel, ref G2GUID alarm, [MarshalAs(UnmanagedType.U1)] bool on);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_beep_control(G2HLIVE handle, int channel, ref G2GUID root, [MarshalAs(UnmanagedType.U1)] bool on);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_camera_color(G2HLIVE handle, int channel, ref G2GUID camera, int type, int value);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_ptz_command(G2HLIVE handle, int channel, ref G2GUID camera, ref G2LIVE_PTZ_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_ptz_preset(G2HLIVE handle, int channel, ref G2GUID camera, ref G2LIVE_PTZ_PRESET preset);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_enable_audio_streaming(G2HLIVE handle, int channel, ref G2GUID camera, [MarshalAs(UnmanagedType.U1)] bool enable);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_enable_audio_capturing(G2HLIVE handle, int channel, ref G2GUID root, int camera, [MarshalAs(UnmanagedType.U1)]bool enable);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_disable_audio_streaming(G2HLIVE handle, int channel, ref G2GUID root);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_disable_audio_capturing(G2HLIVE handle, int channel, ref G2GUID root);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_disable_audio(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_set_disable_audio_root_guid(G2HLIVE handle, int channel, ref G2GUID root);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_live_set_probe_session_profile(G2HLIVE handle, [MarshalAs(UnmanagedType.U1)] bool active);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_request_alive_check(G2HLIVE handle, int channel, int check);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_request_stream_channels(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_request_stream_channels_each(G2HLIVE handle, int channel, G2GUID[] camera, uint count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_request_ptz_menu(G2HLIVE handle, int channel, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_request_ptz_preset(G2HLIVE handle, int channel, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_request_system_log_search(G2HLIVE handle, int channel, ref G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_request_system_log_search_stop(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_request_debug_log_search(G2HLIVE handle, int channel, ref G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE option);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_request_debug_log_search_stop(G2HLIVE handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_get_camera_list(G2HLIVE handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_get_camera_list_interest(G2HLIVE handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_get_camera_stream_set(G2HLIVE handle, int channel, out G2CHANNEL_STREAM_SET streams);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_get_camera_stream(G2HLIVE handle, int channel, int channelext, out G2CHANNEL_STREAM_SET streams);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2GUID g2_live_get_guid_from_channelext(G2HLIVE handle, int channel, int channelext);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_live_get_channelext_from_guid(G2HLIVE handle, int channel, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_live_get_stream_id(G2HLIVE handle, int channel, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_live_get_stream_count(G2HLIVE handle, int channel, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern uint g2_live_get_stream_remote(G2HLIVE handle, int channel, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_get_camera_status(G2HLIVE handle, int channel, ref G2GUID camera, out G2LIVE_CAMERA_STATUS status);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_get_camera_status_stream_info(G2HLIVE handle, int channel, ref G2GUID camera, int stream_id, out G2DEVICE_STATUS_STREAM_INFO status);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_enable_multi_stream(G2HLIVE handle, int channel, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_enable_audio_in(G2HLIVE handle, int channel, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_enable_audio_out(G2HLIVE handle, int channel, ref G2GUID root, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_contains_audio_streaming(G2HLIVE handle, int channel, ref G2GUID camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_contains_audio_capturing(G2HLIVE handle, int channel, ref G2GUID root, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_contains_audio_capturing_root(G2HLIVE handle, int channel, ref G2GUID root);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_contains_audio(G2HLIVE handle, int channel, ref G2GUID root);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_audio_out_opening(G2HLIVE handle, int channel, ref G2GUID root, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_live_is_support(G2HLIVE handle, int channel, ref G2GUID camera, int query);
        #endregion

        public g2live()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_receive_stream_channels = new G2FUN_LISTENER(on_receive_stream_channels);
            this._p2_on_receive_frame_data = new G2FUN_LISTENER(on_receive_frame_data);
            this._p2_on_receive_text_in = new G2FUN_LISTENER(on_receive_text_in);
            this._p2_on_receive_camera_status = new G2FUN_LISTENER(on_receive_camera_status);
            this._p2_on_receive_event = new G2FUN_LISTENER(on_receive_event);
            this._p2_on_receive_ptz_menu = new G2FUN_LISTENER(on_receive_ptz_menu);
            this._p2_on_receive_ptz_preset = new G2FUN_LISTENER(on_receive_ptz_preset);
            this._p2_on_receive_service_log_load = new G2FUN_LISTENER(on_receive_service_log_load);
            this._p2_on_receive_service_log_load_end = new G2FUN_LISTENER(on_receive_service_log_load_end);
            this._p2_on_receive_service_log_load_fail = new G2FUN_LISTENER(on_receive_service_log_load_fail);
            this._p2_on_receive_service_log_load_stop = new G2FUN_LISTENER(on_receive_service_log_load_stop);
            this._p2_on_receive_audio_out_not_available = new G2FUN_LISTENER(on_receive_audio_out_not_available);
            this._p2_on_receive_notify_append_device = new G2FUN_LISTENER(on_receive_notify_append_device);
            this._p2_on_receive_notify_remove_device = new G2FUN_LISTENER(on_receive_notify_remove_device);
            this._p2_on_receive_network_alarm_result = new G2FUN_LISTENER(on_receive_network_alarm_result);
            this._p2_on_audio_streaming_started = new G2FUN_LISTENER(on_audio_streaming_started);
            this._p2_on_audio_streaming_stopped = new G2FUN_LISTENER(on_audio_streaming_stopped);
            this._p2_on_audio_capturing_started = new G2FUN_LISTENER(on_audio_capturing_started);
            this._p2_on_audio_capturing_stopped = new G2FUN_LISTENER(on_audio_capturing_stopped);
            this._p2_on_probe_session_profile = new G2FUN_LISTENER(on_probe_session_profile);
            this._p2_on_receive_debug_log_load = new G2FUN_LISTENER(on_receive_debug_log_load);
            this._p2_on_receive_debug_log_load_end = new G2FUN_LISTENER(on_receive_debug_log_load_end);
            this._p2_on_receive_debug_log_load_fail = new G2FUN_LISTENER(on_receive_debug_log_load_fail);
            this._p2_on_receive_debug_log_load_stop = new G2FUN_LISTENER(on_receive_debug_log_load_stop);
        }
        ~g2live()
        {
            cleanup();
        }

        private G2HLIVE _handle;
        private G2UPARAM _param;
        private g2live_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2LIVE_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_live_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_stream_channels;
        private G2FUN_LISTENER _p2_on_receive_frame_data;
        private G2FUN_LISTENER _p2_on_receive_text_in;
        private G2FUN_LISTENER _p2_on_receive_camera_status;
        private G2FUN_LISTENER _p2_on_receive_event;
        private G2FUN_LISTENER _p2_on_receive_ptz_menu;
        private G2FUN_LISTENER _p2_on_receive_ptz_preset;
        private G2FUN_LISTENER _p2_on_receive_service_log_load;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_service_log_load_stop;
        private G2FUN_LISTENER _p2_on_receive_audio_out_not_available;
        private G2FUN_LISTENER _p2_on_receive_notify_append_device;
        private G2FUN_LISTENER _p2_on_receive_notify_remove_device;
        private G2FUN_LISTENER _p2_on_receive_network_alarm_result;
        private G2FUN_LISTENER _p2_on_audio_streaming_started;
        private G2FUN_LISTENER _p2_on_audio_streaming_stopped;
        private G2FUN_LISTENER _p2_on_audio_capturing_started;
        private G2FUN_LISTENER _p2_on_audio_capturing_stopped;
        private G2FUN_LISTENER _p2_on_probe_session_profile;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_end;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_fail;
        private G2FUN_LISTENER _p2_on_receive_debug_log_load_stop;
        #endregion

        public G2HLIVE safe_handle() { return _handle; }

        public void startup(int connections, G2HWND focus)
        {
            cleanup();

            _handle = g2_live_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2LIVE_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2LIVE_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_stream_channels, _p2_on_receive_stream_channels);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_frame_data, _p2_on_receive_frame_data);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_text_in, _p2_on_receive_text_in);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_camera_status, _p2_on_receive_camera_status);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_event, _p2_on_receive_event);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_ptz_menu, _p2_on_receive_ptz_menu);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_ptz_preset, _p2_on_receive_ptz_preset);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_service_log_load, _p2_on_receive_service_log_load);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_service_log_load_end, _p2_on_receive_service_log_load_end);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_service_log_load_fail, _p2_on_receive_service_log_load_fail);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_service_log_load_stop, _p2_on_receive_service_log_load_stop);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_audio_out_not_available, _p2_on_receive_audio_out_not_available);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_notify_append_device, _p2_on_receive_notify_append_device);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_notify_remove_device, _p2_on_receive_notify_remove_device);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_network_alarm_result, _p2_on_receive_network_alarm_result);
            register_callback(G2LIVE_CALLBACK.TYPE.on_audio_streaming_started, _p2_on_audio_streaming_started);
            register_callback(G2LIVE_CALLBACK.TYPE.on_audio_streaming_stopped, _p2_on_audio_streaming_stopped);
            register_callback(G2LIVE_CALLBACK.TYPE.on_audio_capturing_started, _p2_on_audio_capturing_started);
            register_callback(G2LIVE_CALLBACK.TYPE.on_audio_capturing_stopped, _p2_on_audio_capturing_stopped);
            register_callback(G2LIVE_CALLBACK.TYPE.on_probe_session_profile, _p2_on_probe_session_profile);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_debug_log_load, _p2_on_receive_debug_log_load);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_debug_log_load_end, _p2_on_receive_debug_log_load_end);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_debug_log_load_fail, _p2_on_receive_debug_log_load_fail);
            register_callback(G2LIVE_CALLBACK.TYPE.on_receive_debug_log_load_stop, _p2_on_receive_debug_log_load_stop);
            #endregion

            g2_live_startup(_handle, connections, focus);
        }

        /// <summary>
        /// @ cleanup
        /// <para>This method deletes the memory and resources created by g2_live_startup(). </para>
        /// </summary>
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HLIVE handle = _handle; _handle = 0;
                g2_live_cleanup(handle);
                g2_live_finalize(handle);
            }
        }

        /// <summary>
        /// @ set_listener
        /// <para>This method registers g2live_listener function</para>
        /// </summary>
        /// <param name="listener">listner function</param>
        public void set_listener(g2live_listener listener)
        {
            _listener = listener;
        }

        /// <summary>
        /// @ connect
        /// <para>This method connects to streaming service.</para>
        /// </summary>
        /// <param name="service">streaming service unique id</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection fails, it returns -1; otherwise, it returns channel no. (other than -1).</returns>
        public int connect(ref G2GUID service, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = G2CONNECT_OPTIONS.create();
            return g2_live_connect(_handle, ref service, ref options, out res);
        }

        /// <summary>
        /// @ connect
        /// <para>This method connects to streaming service.</para>
        /// </summary>
        /// <param name="service">streaming service unique id</param>
        /// <param name="options">The structure wherein the options for service connection is saved. It has information about connection time out.</param>
        /// <param name="res">In this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.
        /// </param>
        /// <returns>If a connection fails, it returns -1; otherwise, it returns channel no. (other than -1).</returns>
        public int connect(ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_live_connect(_handle, ref service, ref options, out res);
        }

        /// <summary>
        /// @ disconnect
        /// <para>This method disconnects the streaming service for the corresponding channel.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        public void disconnect(int channel)
        {
            g2_live_disconnect(_handle, channel);
        }

        /// <summary>
        /// @ is_connecting
        /// <para>This method checks whether the corresponding channel is connecting to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connecting(int channel)
        {
            return g2_live_is_connecting(_handle, channel);
        }

        /// <summary>
        /// @ is_connected
        /// <para>This method checks whether the corresponding channel is connected to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If connected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connected(int channel)
        {
            return g2_live_is_connected(_handle, channel);
        }
        
        /// <summary>
        /// @ is_disconnecting
        /// <para>This method checks whether the corresponding channel is disconnecting from the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If in the process of disconnecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnecting(int channel)
        {
            return g2_live_is_disconnecting(_handle, channel);
        }

        /// <summary>
        /// @ is_disconnected
        /// <para>This method checks whether the corresponding channel is disconnected from the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If connected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnected(int channel)
        {
            return g2_live_is_disconnected(_handle, channel);
        }

        /// <summary>
        /// @ is_disconnectable
        /// <para>This method checks whether the corresponding channel is connected or in the process of connecting to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If connected or in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnectable(int channel)
        {
            return g2_live_is_disconnectable(_handle, channel);
        }

        /// <summary>
        /// @ set_camera_list
        /// <para>This method requests images of the specified camera channel to the streaming service</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channels">camera channel list</param>
        /// <param name="force">whether to compare with the existing camera channel information
        /// : If set to TRUE, it does not send a request again when it is consistent with the existing camera channel information. If set to FALSE, it always sends a request to service.
        /// </param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_camera_list(int channel, g2channel_set channels, bool force)
        {
            G2CHANNEL_SET chs = channels;
            return g2_live_set_camera_list(_handle, channel, ref chs, force);
        }

        /// <summary>
        /// @ set_camera_list_interest
        /// <para>This method sets the channel of interest for the streaming service.</para>
        /// <para>To receive images and events, you need to register it as the channel of interest.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channels">channel list of interest</param>
        /// <param name="force">whether to compare with the existing channel information of interest
        /// : If set to TRUE, it does not send a request again when it is consistent with the existing channel information of interest. If set to FALSE, it always sends a request to service.
        /// </param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_camera_list_interest(int channel, g2channel_set channels, bool force)
        {
            G2CHANNEL_SET chs = channels;
            return g2_live_set_camera_list_interest(_handle, channel, ref chs, force);
        }
        /// <summary>
        /// @ set_camera_stream_set
        /// <para>This method sets streams for camera</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="streams">camera streams set</param>
        /// <param name="force">whether to compare with the existing channel information of interest
        /// : If set to TRUE, it does not send a request again when it is consistent with the existing channel information of interest. If set to FALSE, it always sends a request to service.</param>
        /// <returns></returns>
        public bool set_camera_stream_set(int channel, g2channel_stream_set streams, bool force)
        {
            G2CHANNEL_STREAM_SET chs = streams;
            return g2_live_set_camera_stream_set(_handle, channel, ref chs, force);
        }

        /// <summary>
        /// @ set_camera_stream_channel
        /// <para>This method selects multi-stream channel for the specified camera channel for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channelext">device channel</param>
        /// <param name="stream_id">multi-stream channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_camera_stream_channel(int channel, int channelext, int stream_id)
        {
            return g2_live_set_camera_stream_channel(_handle, channel, channelext, stream_id);
        }

        /// <summary>
        /// @ set_audio_list
        /// <para>This method requests audio data for the specified camera channel to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channels">camera channel list</param>
        /// <param name="force">whether to compare with the existing camera channel information
        /// : If set to TRUE, it does not send a request again when it is consistent with the existing camera channel information. If set to FALSE, it always sends a request to service.
        /// </param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_audio_list(int channel, g2channel_set channels, bool force)
        {
            G2CHANNEL_SET chs = channels;
            return g2_live_set_audio_list(_handle, channel, ref chs, force);
        }

        /// <summary>
        /// @ set_stream_id
        /// <para>This method selects multi-stream channel of the camera that corresponds to the unique id for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <param name="id">multi-stream channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_stream_id(int channel, ref G2GUID camera, int id)
        {
            return g2_live_set_stream_id(_handle, channel, ref camera, id);
        }

        /// <summary>
        /// @ set_alarm_out
        /// <para>This method sets On/Off of alarm-out corresponding to the unique id for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="alarm">alarm-out device unique id</param>
        /// <param name="on">On/Off</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_alarm_out(int channel, ref G2GUID alarm, bool on)
        {
            return g2_live_set_alarm_out(_handle, channel, ref alarm, on);
        }
        
        /// <summary>
        /// @ set_beep_control
        /// <para>This method sets on/off status of device's beep control</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">device guid</param>
        /// <param name="on">beep control on/off status</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_beep_control(int channel, ref G2GUID root, bool on)
        {
            return g2_live_set_beep_control(_handle, channel, ref root, on);
        }

        /// <summary>
        /// @ set_camera_color
        /// <para>This method applies color filter to the camera that corresponds to the unique id for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <param name="type">filter type</param>
        /// <param name="value">filter value</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_camera_color(int channel, ref G2GUID camera, G2LIVE_CAMERA_COLOR.TYPE type, G2LIVE_CAMERA_COLOR.VALUE value)
        {
            return g2_live_set_camera_color(_handle, channel, ref camera, (int)type, (int)value);
        }

        /// <summary>
        /// @ set_ptz_command
        /// <para>This method sends the PTZ control command for the specified camera to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <param name="command">structure wherein camera PTZ control information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_ptz_command(int channel, ref G2GUID camera, ref G2LIVE_PTZ_COMMAND command)
        {
            return g2_live_set_ptz_command(_handle, channel, ref camera, ref command);
        }

        /// <summary>
        /// @ set_ptz_preset
        /// <para>This method sets PTZ preset for the specified camera for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <param name="preset">structure wherein camera PTZ preset information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_ptz_preset(int channel, ref G2GUID camera, ref G2LIVE_PTZ_PRESET preset)
        {
            return g2_live_set_ptz_preset(_handle, channel, ref camera, ref preset);
        }

        /// <summary>
        /// @ set_enable_audio_streaming
        /// <para>This method sets whether to enable the audio playback of the specified camera for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <param name="enable">whether to enable the audio playback</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_enable_audio_streaming(int channel, ref G2GUID camera, bool enable)
        {
            return g2_live_set_enable_audio_streaming(_handle, channel, ref camera, enable);
        }

        /// <summary>
        /// @ set_enable_audio_capturing
        /// <para>This method sets whether to enable the mic input of the specified root device for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">root device unique id</param>
        /// <param name="camera">camera number</param>
        /// <param name="enable">whether to use mic input</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_enable_audio_capturing(int channel, ref G2GUID root, int camera, bool enable)
        {
            return g2_live_set_enable_audio_capturing(_handle, channel, ref root, camera, enable);
        }

        /// <summary>
        /// @ set_disable_audio_streaming
        /// <para>This method disables the audio playback of the specified root device for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">root device unique id</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_disable_audio_streaming(int channel, ref G2GUID root)
        {
            return g2_live_set_disable_audio_streaming(_handle, channel, ref root);
        }

        /// <summary>
        /// @ set_disable_audio_capturing
        /// <para>This method disables the use of mic input by the specified root device for the streaming service. </para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">root device unique id</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_disable_audio_capturing(int channel, ref G2GUID root)
        {
            return g2_live_set_disable_audio_capturing(_handle, channel, ref root);
        }

        /// <summary>
        /// @ set_disable_audio
        /// <para>This method disables the audio playback and use of mic input by all devices for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_disable_audio(int channel)
        {
            return g2_live_set_disable_audio(_handle, channel);
        }

        /// <summary>
        /// @ set_disable_audio
        /// <para>This method disables the audio playback and use of mic input by all devices for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">root device unique id</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_disable_audio(int channel, ref G2GUID root)
        {
            return g2_live_set_disable_audio_root_guid(_handle, channel, ref root);
        }

        /// <summary>
        /// @ set_probe_session_profile
        /// <para>This method sets probe session status for profiling</para>
        /// </summary>
        /// <param name="active">probe session active status</param>
        public void set_probe_session_profile(bool active)
        {
            g2_live_set_probe_session_profile(_handle, active);
        }

        /// <summary>
        /// @ request_alive_check
        /// <para> This method sends packets periodically to maintain the connection to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="check">Set 0x1010.</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_alive_check(int channel, int check)
        {
            return g2_live_request_alive_check(_handle, channel, check);
        }
        
        /// <summary>
        /// @ request_stream_channels
        /// <para>This method requests the channel information of all devices of the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_stream_channels(int channel)
        {
            return g2_live_request_stream_channels(_handle, channel);
        }

        /// <summary>
        /// @ request_stream_channels_each
        /// <para>This method requests the channel information of one or more cameras to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="GUIDs">camera unique id list</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_stream_channels_each(int channel, G2GUIDSET GUIDs)
        {
            G2GUID[] param = GUIDs.to_array();
            return g2_live_request_stream_channels_each(_handle, channel, param, (uint)param.Length);
        }

        /// <summary>
        /// @ request_ptz_menu
        /// <para>This method requests the PTZ menu information of the specified camera to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel </param>
        /// <param name="camera">camera unique id</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_ptz_menu(int channel, ref G2GUID camera)
        {
            return g2_live_request_ptz_menu(_handle, channel, ref camera);
        }

        /// <summary>
        /// @ request_ptz_preset
        /// <para>This method requests the PTZ preset information of the specified camera to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_ptz_preset(int channel, ref G2GUID camera)
        {
            return g2_live_request_ptz_preset(_handle, channel, ref camera);
        }

        /// <summary>
        /// @ request_system_log_search
        /// <para>This method requests the streaming service system log.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="option">system log search option</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_system_log_search(int channel, G2SERVICE_SEARCH_OPTION_SYSTEM_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_live_request_system_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @ request_system_log_search_stop
        /// <para>This method stops the search for the streaming service system log.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_system_log_search_stop(int channel)
        {
            return g2_live_request_system_log_search_stop(_handle, channel);
        }

        /// <summary>
        /// @ request_debug_log_search
        /// <para>This method requests the streaming service debug log.</para>
        /// </summary>
        /// <param name="channel">treaming service channel</param>
        /// <param name="option">debug log search option</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_debug_log_search(int channel, G2SERVICE_SEARCH_OPTION_DEBUG_LOG option)
        {
            bool res = false;
            using (MemoryStream stream = new MemoryStream())
            {
                G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE param = new G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE();
                GCHandle gch;
                option.to_param(ref param, stream, out gch);
                res = g2_live_request_debug_log_search(_handle, channel, ref param);
                gch.Free();
            }
            return res;
        }

        /// <summary>
        /// @request_debug_log_search_stop
        /// <para>This method stops the search for the streaming service debug log.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_debug_log_search_stop(int channel)
        {
            return g2_live_request_debug_log_search_stop(_handle, channel);
        }

        /// <summary>
        /// @ get_camera_list
        /// <para>This method obtains the information of the channel that has requested an image to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channels">structure to save the information of the channel that has requested an image</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_camera_list(int channel, out G2CHANNEL_SET channels)
        {
            return g2_live_get_camera_list(_handle, channel, out channels);
        }

        /// <summary>
        /// @ get_camera_list_interest
        /// <para>This method obtains the channel information of interest that has been set to the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channels">structure to save the channel information of interest</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_camera_list_interest(int channel, out G2CHANNEL_SET channels)
        {
            return g2_live_get_camera_list_interest(_handle, channel, out channels);
        }
        /// <summary>
        /// @ get_camera_stream_set
        /// <para>This method obtains all streams set of all devices for streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="streams">stream set</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_camera_stream_set(int channel, out g2channel_stream_set streams)  
        {
            G2CHANNEL_STREAM_SET chs;
            bool ret = g2_live_get_camera_stream_set(_handle, channel, out chs);
            streams = chs;
            return ret;
        }
        /// <summary>
        /// @ get_camera_stream_set
        /// <para>This method obtains streams set of a device that corresponds to the device channel for streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channelext">device channels</param>
        /// <param name="streams">stream set</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_camera_stream_set(int channel, int channelext, out g2channel_stream_set streams)
        {
            G2CHANNEL_STREAM_SET chs;
            bool ret = g2_live_get_camera_stream(_handle, channelext, channel, out chs);
            streams = chs;
            return ret;
        }

        /// <summary>
        /// @ get_guid_from_channelext
        /// <para>This method obtains the unique id of a device that corresponds to the device channel for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channelext">device channel</param>
        /// <returns>It returns device unique id.</returns>
        public G2GUID get_guid_from_channelext(int channel, int channelext)
        {
            return g2_live_get_guid_from_channelext(_handle, channel, channelext);
        }

        /// <summary>
        /// @ get_channelext_from_guid
        /// <para>This method obtains the device channel that corresponds to the camera unique id for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <returns>It returns device channel.</returns>
        public int get_channelext_from_guid(int channel, ref G2GUID camera)
        {
            return g2_live_get_channelext_from_guid(_handle, channel, ref camera);
        }

        /// <summary>
        /// @ get_steam_id
        /// <para>This method obtains the multi-stream id of the camera specified for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <returns>It returns the multi-stream id of camera.</returns>
        public int get_stream_id(int channel, ref G2GUID camera)
        {
            return g2_live_get_stream_id(_handle, channel, ref camera);
        }

        /// <summary>
        /// @ get_stream_count
        /// <para>This method obtains the no. of multi-streams of the camera specified for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <returns>It returns the no. of multi-streams of the camera.</returns>
        public int get_stream_count(int channel, ref G2GUID camera)
        {
            return g2_live_get_stream_count(_handle, channel, ref camera);
        }

        /// <summary>
        /// @ get_stream_remote
        /// <para>This method obtains multi-stream On/Off information of the camera specified for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <returns>It returns the multi-stream status of camera in bitmask.
        /// If bit value is 1, the multi-stream corresponding to the bit order is being used; if 0, it is not being used.</returns>
        public uint get_stream_remote(int channel, ref G2GUID camera)
        {
            return g2_live_get_stream_remote(_handle, channel, ref camera);
        }

        /// <summary>
        /// @ get_camera_status
        /// <para>This method obtains camera status information that corresponds to the camera unique id for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <param name="status">structure to save camera status information</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_camera_status(int channel, ref G2GUID camera, out G2LIVE_CAMERA_STATUS status)
        {
            return g2_live_get_camera_status(_handle, channel, ref camera, out status);
        }

        /// <summary>
        /// @ get_camera_status_stream_info
        /// <para>This method obtains stream information that corresponds to the camera unique id and stream id for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming serivce channel</param>
        /// <param name="camera">camera unique id</param>
        /// <param name="stream_id">streaming id</param>
        /// <param name="status">structure to save stream infomation</param>
        /// <returns></returns>
        public bool get_camera_status_stream_info(int channel, ref G2GUID camera, int stream_id, out G2DEVICE_STATUS_STREAM_INFO status)
        {
            return g2_live_get_camera_status_stream_info(_handle, channel, ref camera, stream_id, out status);
        }

        /// <summary>
        /// @ is_enable_multi_stream
        /// <para>This method checks whether the multi-stream of the camera that corresponds to the camera unique id for the streaming service is enabled.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <returns>If multi-stream is being used, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_enable_multi_stream(int channel, ref G2GUID camera)
        {
            return g2_live_is_enable_multi_stream(_handle, channel, ref camera);
        }
        
        /// <summary>
        /// @ is_enable_audio_in
        /// <para>This method checks whether the audio-in of the camera that corresponds to the camera unique id for the streaming service is enabled.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <returns>If audio-in is being used, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_enable_audio_in(int channel, ref G2GUID camera)
        {
            return g2_live_is_enable_audio_in(_handle, channel, ref camera);
        }

        /// <summary>
        /// @ is_enable_audio_out
        /// <para>This method checks whether the audio-out of the camera that corresponds to the camera unique id for the streaming service is is enabled.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">root device unique id</param>
        /// <param name="camera">camera channel</param>
        /// <returns>If audio-out is being used, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_enable_audio_out(int channel, ref G2GUID root, int camera)
        {
            return g2_live_is_enable_audio_out(_handle, channel, ref root, camera);
        }

        /// <summary>
        /// @ is_contains_audio_streaming
        /// <para>This method checks whether the camera that corresponds to the camera unique id for the streaming service is playing a sound.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <returns>If it is playing a sound, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_contains_audio_streaming(int channel, ref G2GUID camera)
        {
            return g2_live_is_contains_audio_streaming(_handle, channel, ref camera);
        }

        /// <summary>
        /// @ is_contains_audio_capturing
        /// <para> This method checks whether the mic input of the camera that corresponds to the camera unique id for the streaming service is being used.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">root unique id</param>
        /// <param name="camera">camera channel</param>
        /// <returns>If mic input is being used, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_contains_audio_capturing(int channel, ref G2GUID root, int camera)
        {
            return g2_live_is_contains_audio_capturing(_handle, channel, ref root, camera);
        }

        /// <summary>
        /// @ is_contains_audio_capturing_root
        /// <para>This method checks whether the mic input feature of root device is being used.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">root device unique id</param>
        /// <returns>If mic input is being used, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_contains_audio_capturing_root(int channel, ref G2GUID root)
        {
            return g2_live_is_contains_audio_capturing_root(_handle, channel, ref root);
        }

        /// <summary>
        /// @ is_contains_audio
        /// <para>This method checks whether any camera among those of the root device is playing a sound.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">root device unique id</param>
        /// <returns>If any camera is playing a sound, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_contains_audio(int channel, ref G2GUID root)
        {
            return g2_live_is_contains_audio(_handle, channel, ref root);
        }

        /// <summary>
        /// @ is_audio_out_opening
        /// <para>This method checks if selected camera of root device is preparing to use the mic input feature for the streaming service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="root">root device unique id</param>
        /// <param name="camera">camera number of root device that want to use the mic input</param> 
        /// <returns>If any device is preparing to use the mic input feature, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_audio_out_opening(int channel, ref G2GUID root, int camera)
        {
            return g2_live_is_audio_out_opening(_handle, channel, ref root, camera);
        }

        /// <summary>
        /// @ is_support
        /// <para>This method checks whether the camera that corresponds to the camera unique id for the streaming service supports a particular feature.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="camera">camera unique id</param>
        /// <param name="query">feature type : for feature type, refer to G2LIVE_SUPPORT of g2_define.cs </param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_support(int channel, ref G2GUID camera, G2LIVE_SUPPORT.QUERY query)
        {
            return g2_live_is_support(_handle, channel, ref camera, (int)query);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_stream_channels(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH bunch = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            Dictionary<G2GUID, G2LIVE_CHANNEL_INFO> chs = new Dictionary<G2GUID, G2LIVE_CHANNEL_INFO>();
            for (int i = 0; i < bunch._len; ++i)
            {
                IntPtr ptr = new IntPtr(bunch._params.ToInt64() + i * Marshal.SizeOf(typeof(G2LIVE_CHANNEL_INFO)));
                G2LIVE_CHANNEL_INFO ci = (G2LIVE_CHANNEL_INFO)Marshal.PtrToStructure(ptr, typeof(G2LIVE_CHANNEL_INFO));
                chs.Add(ci._guid, ci);
            }
            _listener.on_g2live_receive_stream_channels(handle, (int)wparam, chs);
            return 1;
        }
        private G2RESULT on_receive_frame_data(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2live_receive_frame_data(handle, (int)wparam, ref frame);
            return 1;
        }
        private G2RESULT on_receive_text_in(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2TEXT_IN data = (G2TEXT_IN)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2TEXT_IN));
            _listener.on_g2live_receive_text_in(handle, (int)wparam, ref data);
            return 1;
        }
        private G2RESULT on_receive_camera_status(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH bunch = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2LIVE_CAMERA_STATUS[] status = new G2LIVE_CAMERA_STATUS[bunch._len];
            for (int i = 0; i < bunch._len; ++i)
            {
                IntPtr ptr = new IntPtr(bunch._params.ToInt64() + i * Marshal.SizeOf(typeof(G2LIVE_CAMERA_STATUS)));
                status[i] = (G2LIVE_CAMERA_STATUS)Marshal.PtrToStructure(ptr, typeof(G2LIVE_CAMERA_STATUS));
            }
            _listener.on_g2live_receive_camera_status(handle, (int)wparam, status);
            return 1;
        }
        private G2RESULT on_receive_event(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_INFO info = (G2EVENT_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_INFO));
            _listener.on_g2live_receive_event(handle, (int)wparam, ref info);
            return 1;
        }
        private G2RESULT on_receive_ptz_menu(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_PTZ_MENU menu = (G2LIVE_PTZ_MENU)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_PTZ_MENU));
            _listener.on_g2live_receive_ptz_menu(handle, (int)wparam, ref menu);
            return 1;
        }
        private G2RESULT on_receive_ptz_preset(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_PTZ_PRESET preset = (G2LIVE_PTZ_PRESET)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_PTZ_PRESET));
            _listener.on_g2live_receive_ptz_preset(handle, (int)wparam, ref preset);
            return 1;
        }
        private G2RESULT on_receive_service_log_load(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SYSTEM_LOG log = (G2SYSTEM_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SYSTEM_LOG));
            _listener.on_g2live_receive_service_log_load(handle, (int)wparam, ref log);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_end(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_receive_service_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_fail(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_receive_service_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_service_log_load_stop(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_receive_service_log_load_stop(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_audio_out_not_available(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID root = (G2GUID)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2GUID));
            _listener.on_g2live_receive_audio_out_not_available(handle, (int)wparam, ref root);
            return 1;
        }
        private G2RESULT on_receive_notify_append_device(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID root = (G2GUID)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2GUID));
            _listener.on_g2live_receive_notify_append_device(handle, (int)wparam, ref root);
            return 1;
        }
        private G2RESULT on_receive_notify_remove_device(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2GUID root = (G2GUID)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2GUID));
            _listener.on_g2live_receive_notify_remove_device(handle, (int)wparam, ref root);
            return 1;
        }
        private G2RESULT on_receive_network_alarm_result(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_NETWORK_ALARM_RESULT_PARAM p = (G2LIVE_NETWORK_ALARM_RESULT_PARAM)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_NETWORK_ALARM_RESULT_PARAM));
            _listener.on_g2live_receive_network_alarm_result(handle, (int)wparam, ref p._guid, ref p._result);
            return 1;
        }
        private G2RESULT on_audio_streaming_started(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_sound_streaming_started(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_audio_streaming_stopped(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_sound_streaming_stopped(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_audio_capturing_started(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_PARAM_LIVE_AUDIO_OUT p = (G2LIVE_PARAM_LIVE_AUDIO_OUT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_PARAM_LIVE_AUDIO_OUT));
            _listener.on_g2live_sound_capturing_started(handle, (int)wparam, ref p._root, p._camera);
            return 1;
        }
        private G2RESULT on_audio_capturing_stopped(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_PARAM_LIVE_AUDIO_OUT p = (G2LIVE_PARAM_LIVE_AUDIO_OUT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_PARAM_LIVE_AUDIO_OUT));
            _listener.on_g2live_sound_capturing_stopped(handle, (int)wparam, ref p._root, p._camera);
            return 1;
        }
        private G2RESULT on_probe_session_profile(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PROBE_SESSION_PROFILE p = (G2PROBE_SESSION_PROFILE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PROBE_SESSION_PROFILE));
            _listener.on_g2live_probe_session_profile(handle, (int)wparam, ref p);
            return 1;
        }

        private G2RESULT on_receive_debug_log_load(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEBUG_LOG log = (G2DEBUG_LOG)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2DEBUG_LOG));
            _listener.on_g2live_receive_debug_log_load(handle, (int)wparam, ref log);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_end(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_receive_debug_log_load_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_fail(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_receive_debug_log_load_fail(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_debug_log_load_stop(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2live_receive_debug_log_load_stop(handle, (int)wparam);
            return 1;
        }
        #endregion
    }
}
