using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
    using G2HWATCH = System.Int32;
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

    public interface g2watch_listener
    {
        void on_g2watch_connected(G2HWATCH handle, int channel);
        void on_g2watch_disconnected(G2HWATCH handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2watch_receive_frame_data(G2HWATCH handle, int channel, ref G2FRAME frame);
        void on_g2watch_receive_audio_data(G2HWATCH handle, int channel, ref G2FRAME frame);
        void on_g2watch_receive_event(G2HWATCH handle, int channel, ref G2EVENT_INFO ei);
        void on_g2watch_receive_device_status(G2HWATCH handle, int channel, ref G2DEVICE_STATUS status);
        void on_g2watch_receive_ptz_preset(G2HWATCH handle, int channel, int camera, ref G2LIVE_PTZ_PRESET preset);
        void on_g2watch_receive_ptz_menu(G2HWATCH handle, int channel, int camera, ref G2LIVE_PTZ_MENU menu);
        void on_g2watch_receive_camera_title_idr(G2HWATCH handle, int channel, int camera, string title);
        void on_g2watch_receive_text_in(G2HWATCH handle, int channel, ref G2TEXT_IN data);
        void on_g2watch_receive_network_camera_information(G2HWATCH handle, int channel);
        void on_g2watch_receive_audio_out_not_available(G2HWATCH handle, int channel);
        void on_g2watch_receive_command_result_control_color_status(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_COLOR control, ref G2LIVE_COMMAND_CONTROL_COLOR_RANGE range);
        void on_g2watch_receive_command_result_control_color(G2HANDLE handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_COLOR control, G2LIVE_COMMAND_RESULT.TYPE result);
        void on_g2watch_receive_command_result_control_ptz_status(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_PTZ control, ref G2LIVE_COMMAND_CONTROL_PTZ_RANGE range);
        void on_g2watch_receive_command_result_control_ptz(G2HWATCH handle, int channel, int camera, G2LIVE_COMMAND_RESULT.TYPE result);
        void on_g2watch_receive_network_alarm_result(G2HWATCH handle, int channel, ref G2LIVE_NETWORK_ALARM_RESULT result);
        void on_g2watch_receive_elevator_status_info_response(G2HWATCH handle, int channel, uint seq_number);
        void on_g2watch_receive_instant_recording_start(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status);
        void on_g2watch_receive_instant_recording_stop(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result);
        void on_g2watch_receive_instant_recording_status(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status);
        void on_g2watch_audio_streaming_started(G2HWATCH handle, int channel, int camera);
        void on_g2watch_audio_streaming_stopped(G2HWATCH handle, int channel, int camera);
        void on_g2watch_audio_capturing_started(G2HWATCH handle, int channel, int camera);
        void on_g2watch_audio_capturing_stopped(G2HWATCH handle, int channel, int camera);
        void on_g2watch_probe_session_profile(G2HWATCH handle, int channel, ref G2PROBE_SESSION_PROFILE probe);
    }

    public class g2watch
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_watch_register_callback(G2HWATCH handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HWATCH g2_watch_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_watch_finalize(G2HWATCH handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_watch_startup(G2HWATCH handle, int connections, G2HWND focus);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_watch_cleanup(G2HWATCH handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_watch_connect(G2HWATCH handle, ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_watch_connect_ras(G2HWATCH handle, ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_watch_connect_ras_event(G2HWATCH handle, ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_watch_disconnect(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_connecting(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_connected(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_disconnecting(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_disconnected(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_disconnectable(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_focus_G2HWND(G2HWATCH handle, G2HWND focus);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_camera_channelset(G2HWATCH handle, int channel, ref G2CHANNEL_SET channels, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_camera_stream_set(G2HWATCH handle, int channel, ref G2CHANNEL_STREAM_SET streams, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_camera_stream_set_onetime(G2HWATCH handle, int channel, ref G2CHANNEL_STREAM_SET streams, ref G2CHANNEL_STREAM_SET onetime_streams);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_audio_channelset(G2HWATCH handle, int channel, ref G2CHANNEL_SET channels, [MarshalAs(UnmanagedType.U1)] bool force);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_stream_id(G2HWATCH handle, int channel, int camera, int stream_id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_alarm_out(G2HWATCH handle, int channel, int num, [MarshalAs(UnmanagedType.U1)] bool on);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_beep_control(G2HWATCH handle, int channel, [MarshalAs(UnmanagedType.U1)] bool on);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_camera_color(G2HWATCH handle, int channel, int camera, int type, int value);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_ptz_command(G2HWATCH handle, int channel, int camera, ref G2LIVE_PTZ_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_ptz_preset(G2HWATCH handle, int channel, int camera, ref G2LIVE_PTZ_PRESET preset);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_network_alarm(G2HWATCH handle, int channel, ref G2LIVE_NETWORK_ALARM_INFO info);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_enable_audio_streaming(G2HWATCH handle, int channel, int camera, [MarshalAs(UnmanagedType.U1)] bool enable);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_enable_audio_capturing(G2HWATCH handle, int channel, int camera, [MarshalAs(UnmanagedType.U1)] bool enable);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_disable_audio_streaming(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_disable_audio_capturing(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_set_disable_audio(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_watch_set_probe_session_profile(G2HWATCH handle, [MarshalAs(UnmanagedType.U1)] bool active);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_request_alive_check(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_request_device_status(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_request_ptz_menu(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_request_ptz_preset(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_request_stream_channel_control(G2HWATCH handle, int channel, int camera, int seq, IntPtr buf, int len);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_request_instant_recording_start(G2HWATCH handle, int channel, ref G2INSTANT_RECORD_SETTING_LIST setting);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_request_instant_recording_stop(G2HWATCH handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_request_instant_recording_status(G2HWATCH handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_command_control_color_status(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_command_control_color(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_COLOR control);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_command_control_ptz_status(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_command_control_ptz_status_relative(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_command_control_ptz_status_relative_IDIS(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_command_control_ptz(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_PTZ control);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_command_control_ptz_IDIS(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_PTZ_STATUS status, [MarshalAs(UnmanagedType.U1)] bool req_result);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_fish_eye_control(G2HWATCH handle, int channel, ref G2FISHEYE_CONTROL status);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_network_alarm_info(G2HWATCH handle, int channel, ref G2LIVE_NETWORK_ALARM_INFO info);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_send_elevator_status_info(G2HWATCH handle, int channel, ref G2LIVE_ELEVATOR_STATUS_INFO info);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_adaptor(G2HWATCH handle, IntPtr ptr);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_server_network_info(G2HWATCH handle, int channel, out G2SERVER_NETWORK_INFO ni);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_product_info(G2HWATCH handle, int channel, out G2_PRODUCT_INFO pi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_remote_watch_caps(G2HWATCH handle, int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_WATCH caps);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_remote_setup_info_file(G2HWATCH handle, int channel, string path, string sitename, G2HWND parent);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_authority(G2HWATCH handle, int channel, out G2RAS_AUTHORITY auth);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_camera_channelset(G2HWATCH handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_camera_stream_set(G2HWATCH handle, int channel, out G2CHANNEL_STREAM_SET streams);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_camera_stream(G2HWATCH handle, int channel, int camera, out G2CHANNEL_STREAM_SET streams);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_audio_channelset(G2HWATCH handle, int channel, out G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_watch_get_stream_id(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern uint g2_watch_get_stream_remote(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_status(G2HWATCH handle, int channel, out G2DEVICE_STATUS status);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_status_stream_info(G2HWATCH handle, int channel, int camera, int stream_id, out G2DEVICE_STATUS_STREAM_INFO status);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_status_ip_camera_info(G2HWATCH handle, int channel, int camera, out G2DEVICE_STATUS_IP_CAMERA_INFO status);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_status_ptz_advanced_info(G2HWATCH handle, int channel, int camera, out G2DEVICE_STATUS_PTZ_ADVANCED_INFO status);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_watch_get_status_ptz(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern uint g2_watch_get_status_ptz_function(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_watch_get_status_stream_eptz(G2HWATCH handle, int channel, int camera, int stream_id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern uint g2_watch_get_status_stream_eptz_function(G2HWATCH handle, int channel, int camera, int stream_id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_get_status_command_control_color(G2HWATCH handle, int channel, int camera, out G2LIVE_COMMAND_CONTROL_COLOR control, out G2LIVE_COMMAND_CONTROL_COLOR_RANGE range);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_ras_event_session(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_multi_stream(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_command_control_color(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_command_control_ptz(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_command_control_ptz_relative(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_command_control_ptz_relative_IDIS(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_command_control_ptz_relative_IDIS_one_click_move(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_audio_in(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_audio_out(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_contains_audio_streaming(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_contains_audio_capturing(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_contains_audio_capturing_any(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_contains_audio(G2HWATCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_audio_out_opening(G2HWATCH handle, int channel, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_support(G2HWATCH handle, int channel, int query);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_authority(G2HWATCH handle, int channel, int query);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_probe_session_profile(G2HWATCH handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_audio_device_play(G2HWND focus);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_watch_is_enable_audio_device_record();
        #endregion

        public g2watch()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_receive_frame_data = new G2FUN_LISTENER(on_receive_frame_data);
            this._p2_on_receive_audio_data = new G2FUN_LISTENER(on_receive_audio_data);
            this._p2_on_receive_event = new G2FUN_LISTENER(on_receive_event);
            this._p2_on_receive_device_status = new G2FUN_LISTENER(on_receive_device_status);
            this._p2_on_receive_ptz_preset = new G2FUN_LISTENER(on_receive_ptz_preset);
            this._p2_on_receive_ptz_menu = new G2FUN_LISTENER(on_receive_ptz_menu);
            this._p2_on_receive_camera_title_idr = new G2FUN_LISTENER(on_receive_camera_title_idr);
            this._p2_on_receive_text_in = new G2FUN_LISTENER(on_receive_text_in);
            this._p2_on_receive_network_camera_information = new G2FUN_LISTENER(on_receive_network_camera_information);
            this._p2_on_receive_audio_out_not_available = new G2FUN_LISTENER(on_receive_audio_out_not_available);
            this._p2_on_receive_command_result_control_color_status = new G2FUN_LISTENER(on_receive_command_result_control_color_status);
            this._p2_on_receive_command_result_control_color = new G2FUN_LISTENER(on_receive_command_result_control_color);
            this._p2_on_receive_command_result_control_ptz_status = new G2FUN_LISTENER(on_receive_command_result_control_ptz_status);
            this._p2_on_receive_command_result_control_ptz = new G2FUN_LISTENER(on_receive_command_result_control_ptz);
            this._p2_on_receive_network_alarm_result = new G2FUN_LISTENER(on_receive_network_alarm_result);
            this._p2_on_receive_elevator_status_info_response = new G2FUN_LISTENER(on_receive_elevator_status_info_response);
            this._p2_on_receive_instant_recording_start = new G2FUN_LISTENER(on_receive_instant_recording_start);
            this._p2_on_receive_instant_recording_stop = new G2FUN_LISTENER(on_receive_instant_recording_stop);
            this._p2_on_receive_instant_recording_status = new G2FUN_LISTENER(on_receive_instant_recording_status);
            this._p2_on_audio_streaming_started = new G2FUN_LISTENER(on_audio_streaming_started);
            this._p2_on_audio_streaming_stopped = new G2FUN_LISTENER(on_audio_streaming_stopped);
            this._p2_on_audio_capturing_started = new G2FUN_LISTENER(on_audio_capturing_started);
            this._p2_on_audio_capturing_stopped = new G2FUN_LISTENER(on_audio_capturing_stopped);
            this._p2_on_probe_session_profile = new G2FUN_LISTENER(on_probe_session_profile);
        }
        ~g2watch()
        {
            cleanup();
        }

        private G2HWATCH _handle;
        private G2UPARAM _param;
        private g2watch_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2WATCH_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_watch_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_frame_data;
        private G2FUN_LISTENER _p2_on_receive_audio_data;
        private G2FUN_LISTENER _p2_on_receive_event;
        private G2FUN_LISTENER _p2_on_receive_device_status;
        private G2FUN_LISTENER _p2_on_receive_ptz_preset;
        private G2FUN_LISTENER _p2_on_receive_ptz_menu;
        private G2FUN_LISTENER _p2_on_receive_camera_title_idr;
        private G2FUN_LISTENER _p2_on_receive_text_in;
        private G2FUN_LISTENER _p2_on_receive_network_camera_information;
        private G2FUN_LISTENER _p2_on_receive_audio_out_not_available;
        private G2FUN_LISTENER _p2_on_receive_command_result_control_color_status;
        private G2FUN_LISTENER _p2_on_receive_command_result_control_color;
        private G2FUN_LISTENER _p2_on_receive_command_result_control_ptz_status;
        private G2FUN_LISTENER _p2_on_receive_command_result_control_ptz;
        private G2FUN_LISTENER _p2_on_receive_network_alarm_result;
        private G2FUN_LISTENER _p2_on_receive_elevator_status_info_response;
        private G2FUN_LISTENER _p2_on_receive_instant_recording_start;
        private G2FUN_LISTENER _p2_on_receive_instant_recording_stop;
        private G2FUN_LISTENER _p2_on_receive_instant_recording_status;
        private G2FUN_LISTENER _p2_on_audio_streaming_started;
        private G2FUN_LISTENER _p2_on_audio_streaming_stopped;
        private G2FUN_LISTENER _p2_on_audio_capturing_started;
        private G2FUN_LISTENER _p2_on_audio_capturing_stopped;
        private G2FUN_LISTENER _p2_on_probe_session_profile;
        #endregion

        public G2HWATCH safe_handle() { return _handle; }

        public void startup(int connections, G2HWND focus)
        {
            cleanup();

            _handle = g2_watch_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2WATCH_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2WATCH_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_frame_data, _p2_on_receive_frame_data);
            register_callback(G2WATCH_CALLBACK.TYPE.on_recieve_audio_data, _p2_on_receive_audio_data);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_event, _p2_on_receive_event);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_device_status, _p2_on_receive_device_status);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_ptz_preset, _p2_on_receive_ptz_preset);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_ptz_menu, _p2_on_receive_ptz_menu);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_camera_title_idr, _p2_on_receive_camera_title_idr);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_text_in, _p2_on_receive_text_in);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_network_camera_information, _p2_on_receive_network_camera_information);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_audio_out_not_available, _p2_on_receive_audio_out_not_available);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_command_result_control_color_status, _p2_on_receive_command_result_control_color_status);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_command_result_control_color, _p2_on_receive_command_result_control_color);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_command_result_control_ptz_status, _p2_on_receive_command_result_control_ptz_status);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_command_result_control_ptz, _p2_on_receive_command_result_control_ptz);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_network_alarm_result, _p2_on_receive_network_alarm_result);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_elevator_status_info_response, _p2_on_receive_elevator_status_info_response);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_instant_recording_start, _p2_on_receive_instant_recording_start);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_instant_recording_stop, _p2_on_receive_instant_recording_stop);
            register_callback(G2WATCH_CALLBACK.TYPE.on_receive_instant_recording_status, _p2_on_receive_instant_recording_status);
            register_callback(G2WATCH_CALLBACK.TYPE.on_audio_streaming_started, _p2_on_audio_streaming_started);
            register_callback(G2WATCH_CALLBACK.TYPE.on_audio_streaming_stopped, _p2_on_audio_streaming_stopped);
            register_callback(G2WATCH_CALLBACK.TYPE.on_audio_capturing_started, _p2_on_audio_capturing_started);
            register_callback(G2WATCH_CALLBACK.TYPE.on_audio_capturing_stopped, _p2_on_audio_capturing_stopped);
            register_callback(G2WATCH_CALLBACK.TYPE.on_probe_session_profile, _p2_on_probe_session_profile);
            #endregion

            g2_watch_startup(_handle, connections, focus);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HWATCH handle = _handle; _handle = 0;
                g2_watch_cleanup(handle);
                g2_watch_finalize(handle);
            }
        }

        public void set_listener(g2watch_listener listener)
        {
            _listener = listener;
        }

        public int connect(ref G2GUID root, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_watch_connect(_handle, ref root, ref options, out res);
        }
        public int connect(ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_watch_connect(_handle, ref root, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_watch_connect_ras(_handle, ref ni, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_watch_connect_ras(_handle, ref ni, ref options, out res);
        }
        public int connect_ras_event(ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_watch_connect_ras_event(_handle, ref ni, ref options, out res);
        }
        public void disconnect(int channel)
        {
            g2_watch_disconnect(_handle, channel);
        }

        public bool is_connecting(int channel)
        {
            return g2_watch_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_watch_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_watch_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_watch_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_watch_is_disconnectable(_handle, channel);
        }

        public bool set_focus_G2HWND(G2HWND focus)
        {
            return g2_watch_set_focus_G2HWND(_handle, focus);
        }
        public bool set_camera_list(int channel, g2channel_set channels, bool force)
        {
            G2CHANNEL_SET chs = channels;
            return g2_watch_set_camera_channelset(_handle, channel, ref chs, force);
        }
        public bool set_camera_stream_set(int channel, g2channel_stream_set streams, bool force)
        {
            G2CHANNEL_STREAM_SET chs = streams;
            return g2_watch_set_camera_stream_set(_handle, channel, ref chs, force);
        }
        public bool set_camera_stream_set(int channel, g2channel_stream_set streams, g2channel_stream_set onetime_streams)
        {
            G2CHANNEL_STREAM_SET chs = streams;
            G2CHANNEL_STREAM_SET ochs = onetime_streams;
            return g2_watch_set_camera_stream_set_onetime(_handle, channel, ref chs, ref ochs);
        }
        public bool set_audio_channelset(int channel, g2channel_set channels, bool force)
        {
            G2CHANNEL_SET chs = channels;
            return g2_watch_set_audio_channelset(_handle, channel, ref chs, force);
        }
        public bool set_stream_id(int channel, int camera, int stream_id)
        {
            return g2_watch_set_stream_id(_handle, channel, camera, stream_id);
        }
        public bool set_alarm_out(int channel, int num, bool on)
        {
            return g2_watch_set_alarm_out(_handle, channel, num, on);
        }
        public bool set_beep_control(int channel, bool on)
        {
            return g2_watch_set_beep_control(_handle, channel, on);
        }
        public bool set_camera_color(int channel, int camera, G2LIVE_CAMERA_COLOR.TYPE type, G2LIVE_CAMERA_COLOR.VALUE value)
        {
            return g2_watch_set_camera_color(_handle, channel, camera, (int)type, (int)value);
        }
        public bool set_ptz_command(int channel, int camera, ref G2LIVE_PTZ_COMMAND command)
        {
            return g2_watch_set_ptz_command(_handle, channel, camera, ref command);
        }
        public bool set_ptz_preset(int channel, int camera, ref G2LIVE_PTZ_PRESET preset)
        {
            return g2_watch_set_ptz_preset(_handle, channel, camera, ref preset);
        }
        public bool set_network_alarm(int channel, ref G2LIVE_NETWORK_ALARM_INFO info)
        {
            return g2_watch_set_network_alarm(_handle, channel, ref info);
        }
        public bool set_enable_audio_streaming(int channel, int camera, bool enable)
        {
            return g2_watch_set_enable_audio_streaming(_handle, channel, camera, enable);
        }
        public bool set_enable_audio_capturing(int channel, int camera, bool enable)
        {
            return g2_watch_set_enable_audio_capturing(_handle, channel, camera, enable);
        }
        public bool set_disable_audio_streaming(int channel)
        {
            return g2_watch_set_disable_audio_streaming(_handle, channel);
        }
        public bool set_disable_audio(int channel)
        {
            return g2_watch_set_disable_audio(_handle, channel);
        }
        public void set_probe_session_profile(bool active)
        {
            g2_watch_set_probe_session_profile(_handle, active);
        }

        public bool request_alive_check(int channel)
        {
            return g2_watch_request_alive_check(_handle, channel);
        }
        public bool request_device_status(int channel)
        {
            return g2_watch_request_device_status(_handle, channel);
        }
        public bool request_ptz_menu(int channel, int camera)
        {
            return g2_watch_request_ptz_menu(_handle, channel, camera);
        }
        public bool request_ptz_preset(int channel, int camera)
        {
            return g2_watch_request_ptz_preset(_handle, channel, camera);
        }
        public bool request_stream_channel_control(int channel, int camera, int seq, sbyte[] buf, int len)
        {
            GCHandle gch = GCHandle.Alloc(buf, GCHandleType.Pinned);
            bool ret = g2_watch_request_stream_channel_control(_handle, channel, camera, seq, gch.AddrOfPinnedObject(), len);
            gch.Free();
            return ret;
        }
        public bool request_instant_recording_start(int channel, G2INSTANT_RECORD_SETTING[] setting)
        {
            GCHandle gch = GCHandle.Alloc(setting, GCHandleType.Pinned);
            G2INSTANT_RECORD_SETTING_LIST param = new G2INSTANT_RECORD_SETTING_LIST();
            param._list = gch.AddrOfPinnedObject();
            param._len = (uint)setting.Length;
            g2_watch_request_instant_recording_start(_handle, channel, ref param);
            gch.Free();
            return true;
        }
        public bool request_instant_recording_stop(int channel, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_watch_request_instant_recording_stop(_handle, channel, ref chs);
        }
        public bool request_instant_recording_status(int channel, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_watch_request_instant_recording_status(_handle, channel, ref chs);
        }

        public bool send_command_control_color_status(int channel, int camera)
        {
            return g2_watch_send_command_control_color_status(_handle, channel, camera);
        }
        public bool send_command_control_color(int channel, int camera, ref G2LIVE_COMMAND_CONTROL_COLOR control)
        {
            return g2_watch_send_command_control_color(_handle, channel, camera, ref control);
        }
        public bool send_command_control_ptz_status(int channel, int camera)
        {
            return g2_watch_send_command_control_ptz_status(_handle, channel, camera);
        }
        public bool send_command_control_ptz_status_relative(int channel, int camera)
        {
            return g2_watch_send_command_control_ptz_status_relative(_handle, channel, camera);
        }
        public bool send_command_control_ptz_status_relative_IDIS(int channel, int camera)
        {
            return g2_watch_send_command_control_ptz_status_relative_IDIS(_handle, channel, camera);
        }
        public bool send_command_control_ptz(int channel, int camera, ref G2LIVE_COMMAND_CONTROL_PTZ control)
        {
            return g2_watch_send_command_control_ptz(_handle, channel, camera, ref control);
        }
        public bool send_command_control_ptz_IDIS(int channel, int camera, ref G2LIVE_COMMAND_CONTROL_PTZ_STATUS status, bool req_result)
        {
            return g2_watch_send_command_control_ptz_IDIS(_handle, channel, camera, ref status, req_result);
        }
        public bool send_fish_eye_control(int channel, ref G2FISHEYE_CONTROL control)
        {
            return g2_watch_send_fish_eye_control(_handle, channel, ref control);
        }
        public bool send_network_alarm_info(int channel, ref G2LIVE_NETWORK_ALARM_INFO info)
        {
            return g2_watch_send_network_alarm_info(_handle, channel, ref info);
        }
        public bool send_elevator_status_info(int channel, ref G2LIVE_ELEVATOR_STATUS_INFO info)
        {
            return g2_watch_send_elevator_status_info(_handle, channel, ref info);
        }

        public G2FUN_GET_ADAPTOR get_adaptor()
        {
            return new G2FUN_GET_ADAPTOR(g2_watch_get_adaptor);
        }
        public bool get_server_network_info(int channel, out G2SERVER_NETWORK_INFO ni)
        {
            return g2_watch_get_server_network_info(_handle, channel, out ni);
        }
        public bool get_product_info(int channel, out G2_PRODUCT_INFO pi)
        {
            return g2_watch_get_product_info(_handle, channel, out pi);
        }
        public bool get_remote_watch_caps(int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_WATCH caps)
        {
            return g2_watch_get_remote_watch_caps(_handle, channel, out caps);
        }
        public bool get_remote_setup_info_file(int channel, string path, string sitename, G2HWND focus)
        {
            return g2_watch_get_remote_setup_info_file(_handle, channel, path, sitename, focus);
        }
        public bool get_authority(int channel, out G2RAS_AUTHORITY auth)
        {
            return g2_watch_get_authority(_handle, channel, out auth);
        }
        public bool get_camera_channelset(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_watch_get_camera_channelset(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public bool get_camera_stream_set(int channel, out g2channel_stream_set streams)
        {
            G2CHANNEL_STREAM_SET chs;
            bool ret = g2_watch_get_camera_stream_set(_handle, channel, out chs);
            streams = chs;
            return ret;
        }
        public bool get_camera_stream_set(int channel, int camera, out g2channel_stream_set streams)
        {
            G2CHANNEL_STREAM_SET chs;
            bool ret = g2_watch_get_camera_stream(_handle, channel, camera, out chs);
            streams = chs;
            return ret;
        }
        public bool get_audio_channelset(int channel, out g2channel_set channels)
        {
            G2CHANNEL_SET chs;
            bool ret = g2_watch_get_audio_channelset(_handle, channel, out chs);
            channels = chs;
            return ret;
        }
        public int get_stream_id(int channel, int camera)
        {
            return g2_watch_get_stream_id(_handle, channel, camera);
        }
        public uint get_stream_remote(int channel, int camera)
        {
            return g2_watch_get_stream_remote(_handle, channel, camera);
        }
        public bool get_status(int channel, out G2DEVICE_STATUS status)
        {
            return g2_watch_get_status(_handle, channel, out status);
        }
        public bool get_status_stream_info(int channel, int camera, int stream_id, out G2DEVICE_STATUS_STREAM_INFO status)
        {
            return g2_watch_get_status_stream_info(_handle, channel, camera, stream_id, out status);
        }
        public bool get_status_ip_camera_info(int channel, int camera, out G2DEVICE_STATUS_IP_CAMERA_INFO status)
        {
            return g2_watch_get_status_ip_camera_info(_handle, channel, camera, out status);
        }
        public bool get_status_ptz_advanced_info(int channel, int camera, out G2DEVICE_STATUS_PTZ_ADVANCED_INFO status)
        {
            return g2_watch_get_status_ptz_advanced_info(_handle, channel, camera, out status);
        }
        public G2DEVICE_STATUS.PTZ get_status_ptz(int channel, int camera)
        {
            return (G2DEVICE_STATUS.PTZ)g2_watch_get_status_ptz(_handle, channel, camera);
        }
        public G2DEVICE_STATUS.PTZ_FUNCTION get_status_ptz_function(int channel, int camera)
        {
            return (G2DEVICE_STATUS.PTZ_FUNCTION)g2_watch_get_status_ptz_function(_handle, channel, camera);
        }
        public G2DEVICE_STATUS.PTZ get_status_stream_eptz(int channel, int camera, int stream_id)
        {
            return (G2DEVICE_STATUS.PTZ)g2_watch_get_status_stream_eptz(_handle, channel, camera, stream_id);
        }
        public G2DEVICE_STATUS.PTZ_FUNCTION get_status_stream_eptz_function(int channel, int camera, int stream_id)
        {
            return (G2DEVICE_STATUS.PTZ_FUNCTION)g2_watch_get_status_stream_eptz_function(_handle, channel, camera, stream_id);
        }
        public bool get_status_command_control_color(int channel, int camera, out G2LIVE_COMMAND_CONTROL_COLOR control, out G2LIVE_COMMAND_CONTROL_COLOR_RANGE range)
        {
            return g2_watch_get_status_command_control_color(_handle, channel, camera, out control, out range);
        }

        public bool is_ras_event_session(int channel)
        {
            return g2_watch_is_ras_event_session(_handle, channel);
        }
        public bool is_enable_multi_stream(int channel, int camera)
        {
            return g2_watch_is_enable_multi_stream(_handle, channel, camera);
        }
        public bool is_enable_command_control_color(int channel, int camera)
        {
            return g2_watch_is_enable_command_control_color(_handle, channel, camera);
        }
        public bool is_enable_command_control_ptz(int channel, int camera)
        {
            return g2_watch_is_enable_command_control_ptz(_handle, channel, camera);
        }
        public bool is_enable_command_control_ptz_relative(int channel, int camera)
        {
            return g2_watch_is_enable_command_control_ptz_relative(_handle, channel, camera);
        }
        public bool is_enable_command_control_ptz_relative_IDIS(int channel, int camera)
        {
            return g2_watch_is_enable_command_control_ptz_relative_IDIS(_handle, channel, camera);
        }
        public bool is_enable_command_control_ptz_relative_IDIS_one_click_move(int channel, int camera)
        {
            return g2_watch_is_enable_command_control_ptz_relative_IDIS_one_click_move(_handle, channel, camera);
        }
        public bool is_enable_audio_in(int channel, int camera)
        {
            return g2_watch_is_enable_audio_in(_handle, channel, camera);
        }
        public bool is_enable_audio_out(int channel, int camera)
        {
            return g2_watch_is_enable_audio_out(_handle, channel, camera);
        }
        public bool is_contains_audio_streaming(int channel, int camera)
        {
            return g2_watch_is_contains_audio_streaming(_handle, channel, camera);
        }
        public bool is_contains_audio_capturing(int channel, int camera)
        {
            return g2_watch_is_contains_audio_capturing(_handle, channel, camera);
        }
        public bool is_contains_audio_capturing(int channel)
        {
            return g2_watch_is_contains_audio_capturing_any(_handle, channel);
        }
        public bool is_contains_audio(int channel)
        {
            return g2_watch_is_contains_audio(_handle, channel);
        }
        public bool is_audio_out_opening(int channel, int camera)
        {
            return g2_watch_is_audio_out_opening(_handle, channel, camera);
        }
        public bool is_support(int channel, G2LIVE_SUPPORT.QUERY query)
        {
            return g2_watch_is_support(_handle, channel, (int)query);
        }
        public bool is_authority(int channel, G2RAS_AUTHORITY.TYPE authority)
        {
            return g2_watch_is_authority(_handle, channel, (int)authority);
        }
        public bool is_probe_perofmance()
        {
            return g2_watch_is_probe_session_profile(_handle);
        }
        public bool is_enable_audio_device_play(G2HWND focus)
        {
            return g2_watch_is_enable_audio_device_play(focus);
        }
        public bool is_enable_audio_device_record()
        {
            return g2_watch_is_enable_audio_device_record();
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_frame_data(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME p = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2watch_receive_frame_data(handle, (int)wparam, ref p);
            return 1;
        }
        private G2RESULT on_receive_audio_data(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME p = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2watch_receive_audio_data(handle, (int)wparam, ref p);
            return 1;
        }
        private G2RESULT on_receive_event(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2EVENT_INFO p = (G2EVENT_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2EVENT_INFO));
            _listener.on_g2watch_receive_event(handle, (int)wparam, ref p);
            return 1;
        }
        private G2RESULT on_receive_device_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEVICE_STATUS p = (G2DEVICE_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2DEVICE_STATUS));
            _listener.on_g2watch_receive_device_status(handle, (int)wparam, ref p);
            return 1;
        }
        private G2RESULT on_receive_ptz_preset(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_PARAM_PTZ_PRESET p = (G2LIVE_PARAM_PTZ_PRESET)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_PARAM_PTZ_PRESET));
            _listener.on_g2watch_receive_ptz_preset(handle, (int)wparam, p._camera, ref p._data);
            return 1;
        }
        private G2RESULT on_receive_ptz_menu(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_PARAM_PTZ_MENU p = (G2LIVE_PARAM_PTZ_MENU)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_PARAM_PTZ_MENU));
            _listener.on_g2watch_receive_ptz_menu(handle, (int)wparam, p._camera, ref p._data);
            return 1;
        }
        private G2RESULT on_receive_camera_title_idr(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2DEVICE_NAME p = (G2DEVICE_NAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2DEVICE_NAME));
            _listener.on_g2watch_receive_camera_title_idr(handle, (int)wparam, p._number, p._name);
            return 1;
        }
        private G2RESULT on_receive_text_in(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2TEXT_IN p = (G2TEXT_IN)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2TEXT_IN));
            _listener.on_g2watch_receive_text_in(handle, (int)wparam, ref p);
            return 1;
        }
        private G2RESULT on_receive_network_camera_information(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_receive_network_camera_information(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_audio_out_not_available(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_receive_audio_out_not_available(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_command_result_control_color_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_COMMAND_CONTROL_COLOR_STATUS p = (G2LIVE_COMMAND_CONTROL_COLOR_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_COMMAND_CONTROL_COLOR_STATUS));
            _listener.on_g2watch_receive_command_result_control_color_status(handle, (int)wparam, p._camera, ref p._control, ref p._range);
            return 1;
        }
        private G2RESULT on_receive_command_result_control_color(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_COMMAND_CONTROL_COLOR_RESULT p = (G2LIVE_COMMAND_CONTROL_COLOR_RESULT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_COMMAND_CONTROL_COLOR_RESULT));
            _listener.on_g2watch_receive_command_result_control_color(handle, (int)wparam, p._camera, ref p._control, (G2LIVE_COMMAND_RESULT.TYPE)p._result);
            return 1;
        }
        private G2RESULT on_receive_command_result_control_ptz_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_COMMAND_CONTROL_PTZ_STATUS p = (G2LIVE_COMMAND_CONTROL_PTZ_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_COMMAND_CONTROL_PTZ_STATUS));
            _listener.on_g2watch_receive_command_result_control_ptz_status(handle, (int)wparam, p._camera, ref p._control, ref p._range);
            return 1;
        }
        private G2RESULT on_receive_command_result_control_ptz(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_COMMAND_CONTROL_PTZ_RESULT p = (G2LIVE_COMMAND_CONTROL_PTZ_RESULT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_COMMAND_CONTROL_PTZ_RESULT));
            _listener.on_g2watch_receive_command_result_control_ptz(handle, (int)wparam, p._camera, (G2LIVE_COMMAND_RESULT.TYPE)p._result);
            return 1;
        }
        private G2RESULT on_receive_network_alarm_result(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2LIVE_NETWORK_ALARM_RESULT p = (G2LIVE_NETWORK_ALARM_RESULT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2LIVE_NETWORK_ALARM_RESULT));
            _listener.on_g2watch_receive_network_alarm_result(handle, (int)wparam, ref p);
            return 1;
        }
        private G2RESULT on_receive_elevator_status_info_response(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_receive_elevator_status_info_response(handle, (int)wparam, (uint)lparam);
            return 1;
        }
        private G2RESULT on_receive_instant_recording_start(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS p = (G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS));
            G2INSTANT_RECORDING_CHANNEL_STATUS[] status = new G2INSTANT_RECORDING_CHANNEL_STATUS[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._list.ToInt64() + i * Marshal.SizeOf(typeof(G2INSTANT_RECORDING_CHANNEL_STATUS)));
                status[i] = (G2INSTANT_RECORDING_CHANNEL_STATUS)Marshal.PtrToStructure(ptr, typeof(G2INSTANT_RECORDING_CHANNEL_STATUS));
            }
            _listener.on_g2watch_receive_instant_recording_start(handle, (int)wparam, (G2INSTANT_RECORDING_RESULT.TYPE)p._result, status);
            return 1;
        }
        private G2RESULT on_receive_instant_recording_stop(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_receive_instant_recording_stop(handle, (int)wparam, (G2INSTANT_RECORDING_RESULT.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_instant_recording_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS p = (G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS));
            G2INSTANT_RECORDING_CHANNEL_STATUS[] status = new G2INSTANT_RECORDING_CHANNEL_STATUS[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._list.ToInt64() + i * Marshal.SizeOf(typeof(G2INSTANT_RECORDING_CHANNEL_STATUS)));
                status[i] = (G2INSTANT_RECORDING_CHANNEL_STATUS)Marshal.PtrToStructure(ptr, typeof(G2INSTANT_RECORDING_CHANNEL_STATUS));
            }
            _listener.on_g2watch_receive_instant_recording_status(handle, (int)wparam, (G2INSTANT_RECORDING_RESULT.TYPE)p._result, status);
            return 1;
        }
        private G2RESULT on_audio_streaming_started(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_audio_streaming_started(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_audio_streaming_stopped(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_audio_streaming_stopped(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_audio_capturing_started(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_audio_capturing_started(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_audio_capturing_stopped(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2watch_audio_capturing_stopped(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_probe_session_profile(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PROBE_SESSION_PROFILE p = (G2PROBE_SESSION_PROFILE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PROBE_SESSION_PROFILE));
            _listener.on_g2watch_probe_session_profile(handle, (int)wparam, ref p);
            return 1;
        }
        #endregion
    }
}
