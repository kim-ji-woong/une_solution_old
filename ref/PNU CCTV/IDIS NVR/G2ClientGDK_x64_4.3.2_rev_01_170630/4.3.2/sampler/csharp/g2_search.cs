using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
    using G2HSEARCH = System.Int32;
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

    public interface g2search_listener
    {
        void on_g2search_connected(G2HSEARCH handle, int channel);
        void on_g2search_disconnected(G2HSEARCH handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2search_receive_recorded_date(G2HSEARCH handle, int channel, G2TIME[] dates);
        void on_g2search_receive_recorded_time_hour(G2HSEARCH handle, int channel, bool[][] hour, uint segcount);
        void on_g2search_receive_recorded_time_minute(G2HSEARCH handle, int channel, List<byte[]> minute);
        void on_g2search_receive_recorded_rechour_minute(G2HSEARCH handle, int channel, G2RECORD_TIME_INFO[] rti);
        void on_g2search_receive_frame_data(G2HSEARCH handle, int channel, ref G2FRAME frame);
        void on_g2search_receive_no_frame(G2HSEARCH handle, int channel);
        void on_g2search_receive_no_recorded_data(G2HSEARCH handle, int channel);
        void on_g2search_receive_no_recorded_data_from_search_target(G2HSEARCH handle, int channel, G2SEARCH_TARGET.TYPE target);
        void on_g2search_receive_find_idr_event_time(G2HSEARCH handle, int channel, G2TIME time);
        void on_g2search_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, int speed);
        void on_g2search_receive_notify_play_stop_post(G2HSEARCH handle, int channel, G2SEARCH_DRIVE.MODE mode);
        void on_g2search_receive_notify_end_of_play(G2HSEARCH handle, int channel);
        void on_g2search_receive_notify_segment_changed(G2HSEARCH handle, int channel, int segment);
        void on_g2search_receive_notify_search_mode_changed(G2HSEARCH handle, int channel, G2SEARCH_MODE.TYPE mode);
        void on_g2search_receive_notify_command_end(G2HSEARCH handle, int channel, int command);
        void on_g2search_receive_query_result_event(G2HSEARCH handle, int channel, G2SEARCH_LOG_INFO[] info);
        void on_g2search_receive_query_result_text_in(G2HSEARCH handle, int channel, G2TEXT_IN[] data);
        void on_g2search_receive_recorded_date_scope(G2HSEARCH handle, int channel, G2SCOPE[] scopes);
        void on_g2search_receive_segment_spot(G2HSEARCH handle, int channel, G2SPOT[] spots);
        void on_g2search_receive_error(G2HSEARCH handle, int channel);
        void on_g2search_receive_text_in(G2HSEARCH handle, int channel, ref G2TEXT_IN_ELEMENT data);
        void on_g2search_receive_external_tango_info(G2HSEARCH handle, int channel, G2SEARCH_EXTERNAL_DISK[] info);
        void on_g2search_receive_gps_data_start(G2HSEARCH handle, int channel);
        void on_g2search_receive_gps_data(G2HSEARCH handle, int channel, ref G2TEXT_IN data);
        void on_g2search_receive_gps_data_list(G2HSEARCH handle, int channel, G2TEXT_IN[] data);
        void on_g2search_receive_gps_data_end(G2HSEARCH handle, int channel);
        void on_g2search_receive_gps_data_end_count(G2HSEARCH handle, int channel, int total);
        void on_g2search_receive_gps_data_export_cancel_result(G2HSEARCH handle, int channel, int result);
        void on_g2search_receive_gps_data_measure_result(G2HSEARCH handle, int channel, int count, sbyte done);
        void on_g2search_require_prepare_playback(G2HSEARCH handle, int channel, int command, G2SPOT spot);
        bool on_g2search_require_prepare_load_event_image(G2HSEARCH handle, int channel, int selected, bool last); // default return true
        bool on_g2search_require_prepare_reload(G2HSEARCH handle, int channel); // default return true
        void on_g2search_probe_session_profile(G2HSEARCH handle, int channel, ref G2PROBE_SESSION_PROFILE probe);
    }

    public interface g2search_listener_saver
    {
        void on_g2search_saver_connected(G2HSEARCH handle, int channel);
        void on_g2search_saver_disconnected(G2HSEARCH handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        void on_g2search_saver_receive_recorded_date(G2HSEARCH handle, int channel, G2TIME[] dates);
        void on_g2search_saver_receive_frame_data(G2HSEARCH handle, int channel, ref G2FRAME frame);
        void on_g2search_saver_receive_no_frame(G2HSEARCH handle, int channel);
        void on_g2search_saver_receive_no_recorded_data(G2HSEARCH handle, int channel);
        void on_g2search_saver_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, int speed);
        void on_g2search_saver_receive_notify_play_stop_spot(G2HSEARCH handle, int channel, G2SEARCH_DRIVE.MODE mode);
        void on_g2search_saver_receive_notify_end_of_play(G2HSEARCH handle, int channel);
        void on_g2search_saver_receive_notify_command_end(G2HSEARCH handle, int channel, int command);
        void on_g2search_saver_receive_clipcopy_scope(G2HSEARCH handle, int channel, G2SCOPE[] scopes);
        void on_g2search_saver_receive_clipcopy_measure_size(G2HSEARCH handle, int channel);
        void on_g2search_saver_receive_clipcopy_size(G2HSEARCH handle, int channel, G2CLIPCOPY_STATUS.TYPE status, ulong size, G2TIME begin, G2TIME end, ref G2CLIPCOPY_SIZE_INFO info);
        void on_g2search_saver_receive_clipcopy_data(G2HSEARCH handle, int channel, int offset, int size, IntPtr data, int progress, bool completed);
        void on_g2search_saver_receive_clipcopy_set_password(G2HSEARCH handle, int channel);
        void on_g2search_saver_receive_clipcopy_enable_channels(G2HSEARCH handle, int channel, uint cameras);
        void on_g2search_saver_receive_clipcopy_canceled(G2HSEARCH handle, int channel);
        void on_g2search_saver_receive_clipcopy_job_started(G2HSEARCH handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total);
        void on_g2search_saver_receive_clipcopy_job_finished(G2HSEARCH handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total);
        void on_g2search_saver_receive_clipcopy_section_begin(G2HSEARCH handle, int channel, uint num, uint total);
        void on_g2search_saver_receive_clipcopy_section_end(G2HSEARCH handle, int channel, uint num, uint total);
        void on_g2search_saver_receive_bank_space(G2HSEARCH handle, int channel, int image_index_num, int audio_index_num, G2TIME start, int start_msec, ulong image_size, ulong audio_size);
        void on_g2search_saver_receive_bank_image(G2HSEARCH handle, int channel, IntPtr data);
        void on_g2search_saver_receive_bank_audio(G2HSEARCH handle, int channel, IntPtr data);
        void on_g2search_saver_receive_bank_no_image(G2HSEARCH handle, int channel);
        void on_g2search_saver_receive_bank_no_audio(G2HSEARCH handle, int channel);
    }

    public class g2search
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_register_callback(G2HSEARCH handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HSEARCH g2_search_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_finalize(G2HSEARCH handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_startup(G2HSEARCH handle, int connections);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_cleanup(G2HSEARCH handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_connect(G2HSEARCH handle, ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_connect_ras(G2HSEARCH handle, ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_disconnect(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_connecting(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_connected(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_disconnecting(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_disconnected(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_disconnectable(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_set_probe_session_profile(G2HSEARCH handle, [MarshalAs(UnmanagedType.U1)] bool active);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_set_invoke_saver(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_set_revoke_saver(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_set_camera_list(G2HSEARCH handle, int channel, uint cameras);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_set_search_target(G2HSEARCH handle, int channel, int target);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_set_change_segment(G2HSEARCH handle, int channel, int id);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_set_change_search_mode(G2HSEARCH handle, int channel, [MarshalAs(UnmanagedType.U1)] bool event_search);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_set_external_tango(G2HSEARCH handle, int channel, int type, int number);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_set_query_mode(G2HSEARCH handle, int channel, int mode);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_set_query_cameras(G2HSEARCH handle, int channel, uint cameras);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_set_invoke_on_play(G2HSEARCH handle, int channel, ref G2SPOT spot);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_search_set_revoke_on_play(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_record_date(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_record_time(G2HSEARCH handle, int channel, ref G2TIME date);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_record_hour(G2HSEARCH handle, int channel, ref G2SPOT spot, int length, [MarshalAs(UnmanagedType.U1)] bool direction, [MarshalAs(UnmanagedType.U1)] bool check_diff_prev_req);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_record_hour_command(G2HSEARCH handle, int channel, int command, ref G2SPOT spot);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_playback(G2HSEARCH handle, int channel, int command, ref G2SPOT spot);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_pause(G2HSEARCH handle, int channel, ref G2ROLLBACK_INFO rbi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_reload_current(G2HSEARCH handle, int channel, int camera, ref G2SPOT spot);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_notify_end_of_play(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_query_specific_event(G2HSEARCH handle, int channel, int seq_number, ref G2TIME begin, ref G2TIME end, int event_type, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_query_event(G2HSEARCH handle, int channel, ref G2EVENT_QUERY_CONDITION condition);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_query_text_in(G2HSEARCH handle, int channel, ref G2TEXT_IN_QUERY_CONDITION condition);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_event_image(G2HSEARCH handle, int channel, int selected, [MarshalAs(UnmanagedType.U1)] bool last);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_event_search_stop_idr(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy_enable_channelset(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy_scope(G2HSEARCH handle, int channel, ref G2TIME from, ref G2TIME to, uint cameras);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy_measure_size(G2HSEARCH handle, int channel, ref G2SCOPE scope, ulong free_space, bool slice);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy_size(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy(G2HSEARCH handle, int channel, [MarshalAs(UnmanagedType.U1)] bool start);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy_password(G2HSEARCH handle, int channel, [MarshalAs(UnmanagedType.U1)] bool use, ref G2STRING_32 password);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy_text_in(G2HSEARCH handle, int channel, [MarshalAs(UnmanagedType.U1)] bool include);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy_gps_data(G2HSEARCH handle, int channel, [MarshalAs(UnmanagedType.U1)] bool include);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy_structure(G2HSEARCH handle, int channel, int version, [MarshalAs(UnmanagedType.U1)] bool exclude_player, [MarshalAs(UnmanagedType.U1)] bool avoid_seek);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_clipcopy_cancel(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_mini_bank_begin(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_mini_bank_end(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_mini_bank_space(G2HSEARCH handle, int channel, ref G2TIME from, ref G2TIME to, uint cameras, [MarshalAs(UnmanagedType.U1)] bool audio);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_mini_bank(G2HSEARCH handle, int channel, int command, ref G2TIME time, int msec);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_gps_data_measure(G2HSEARCH handle, int channel, ref G2TIME from, ref G2TIME to);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_gps_data_measure_result(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_gps_data(G2HSEARCH handle, int channel, ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_gps_data_next(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_request_gps_data_export_cancel(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_adaptor(G2HSEARCH handle, IntPtr ptr);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_server_network_info(G2HSEARCH handle, int channel, out G2SERVER_NETWORK_INFO ni);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_product_info(G2HSEARCH handle, int channel, out G2_PRODUCT_INFO pi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_remote_search_caps(G2HSEARCH handle, int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH caps);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_remote_clipcopy_caps(G2HSEARCH handle, int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_CLIP_COPY caps);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_text_in_search_caps(G2HSEARCH handle, int channel, out G2_PRODUCT_INFO_CAPS.TEXT_IN_SEARCH caps);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_authority(G2HSEARCH handle, int channel, out G2RAS_AUTHORITY auth);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_camera_list(G2HSEARCH handle, int channel, out uint cameras);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_time_last(G2HSEARCH handle, int channel, out G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_get_drive_mode(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_get_query_mode(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_get_timelapse_mode(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_get_play_speed(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_get_current_command(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_search_get_search_target(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_query_cameras(G2HSEARCH handle, int channel, out uint cameras);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_query_condition_event(G2HSEARCH handle, int channel, out G2EVENT_QUERY_CONDITION condition);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_query_condition_text_in(G2HSEARCH handle, int channel, out G2TEXT_IN_QUERY_CONDITION condition);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_query_result_text_in(G2HSEARCH handle, int channel, int count, G2TEXT_IN[] data);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_query_result_event(G2HSEARCH handle, int channel, int selected, out G2SEARCH_LOG_INFO data);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_get_clipcopy_size_info(G2HSEARCH handle, int channel, out G2CLIPCOPY_SIZE_INFO info);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_probe_session_profile(G2HSEARCH handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_rec_info_type(G2HSEARCH handle, int channel, int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_drive_mode(G2HSEARCH handle, int channel, int mode);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_query_mode(G2HSEARCH handle, int channel, int mode);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_timelapse_mode(G2HSEARCH handle, int channel, int mode);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_event_search_mode(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_stopped(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_loading(G2HSEARCH handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_support(G2HSEARCH handle, int channel, int query);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_search_is_authority(G2HSEARCH handle, int channel, int authority);
        #endregion

        public g2search()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._listener_saver = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_receive_recorded_date = new G2FUN_LISTENER(on_receive_recorded_date);
            this._p2_on_receive_recorded_time_hour = new G2FUN_LISTENER(on_receive_recorded_time_hour);
            this._p2_on_receive_recorded_time_minute = new G2FUN_LISTENER(on_receive_recorded_time_minute);
            this._p2_on_receive_recorded_rechour_minute = new G2FUN_LISTENER(on_receive_recorded_rechour_minute);
            this._p2_on_receive_frame_data = new G2FUN_LISTENER(on_receive_frame_data);
            this._p2_on_receive_no_frame = new G2FUN_LISTENER(on_receive_no_frame);
            this._p2_on_receive_no_recorded_data = new G2FUN_LISTENER(on_receive_no_recorded_data);
            this._p2_on_receive_no_recorded_data_from_search_target = new G2FUN_LISTENER(on_receive_no_recorded_data_from_search_target);
            this._p2_on_receive_find_idr_event_time = new G2FUN_LISTENER(on_receive_find_idr_event_time);
            this._p2_on_receive_notify_play_speed_changed = new G2FUN_LISTENER(on_receive_notify_play_speed_changed);
            this._p2_on_receive_notify_play_stop_post = new G2FUN_LISTENER(on_receive_notify_play_stop_post);
            this._p2_on_receive_notify_end_of_play = new G2FUN_LISTENER(on_receive_notify_end_of_play);
            this._p2_on_receive_notify_segment_changed = new G2FUN_LISTENER(on_receive_notify_segment_changed);
            this._p2_on_receive_notify_search_mode_changed = new G2FUN_LISTENER(on_receive_notify_search_mode_changed);
            this._p2_on_receive_notify_command_end = new G2FUN_LISTENER(on_receive_notify_command_end);
            this._p2_on_receive_query_result_event = new G2FUN_LISTENER(on_receive_query_result_event);
            this._p2_on_receive_query_result_text_in = new G2FUN_LISTENER(on_receive_query_result_text_in);
            this._p2_on_receive_recorded_date_scope = new G2FUN_LISTENER(on_receive_recorded_date_scope);
            this._p2_on_receive_segment_spot = new G2FUN_LISTENER(on_receive_segment_spot);
            this._p2_on_receive_error = new G2FUN_LISTENER(on_receive_error);
            this._p2_on_receive_text_in = new G2FUN_LISTENER(on_receive_text_in);
            this._p2_on_receive_external_tango_info = new G2FUN_LISTENER(on_receive_external_tango_info);
            this._p2_on_receive_gps_data_start = new G2FUN_LISTENER(on_receive_gps_data_start);
            this._p2_on_receive_gps_data = new G2FUN_LISTENER(on_receive_gps_data);
            this._p2_on_receive_gps_data_list = new G2FUN_LISTENER(on_receive_gps_data_list);
            this._p2_on_receive_gps_data_end = new G2FUN_LISTENER(on_receive_gps_data_end);
            this._p2_on_receive_gps_data_end_count = new G2FUN_LISTENER(on_receive_gps_data_end_count);
            this._p2_on_receive_gps_data_export_cancel_result = new G2FUN_LISTENER(on_receive_gps_data_export_cancel_result);
            this._p2_on_receive_gps_data_measure_result = new G2FUN_LISTENER(on_receive_gps_data_measure_result);
            this._p2_on_require_prepare_playback = new G2FUN_LISTENER(on_require_prepare_playback);
            this._p2_on_require_prepare_load_event_image = new G2FUN_LISTENER(on_require_prepare_load_event_image);
            this._p2_on_require_prepare_reload = new G2FUN_LISTENER(on_require_prepare_reload);
            this._p2_on_probe_session_profile = new G2FUN_LISTENER(on_probe_session_profile);
            this._p2_on_saver_connected = new G2FUN_LISTENER(on_saver_connected);
            this._p2_on_saver_disconnected = new G2FUN_LISTENER(on_saver_disconnected);
            this._p2_on_saver_receive_recorded_date = new G2FUN_LISTENER(on_saver_receive_recorded_date);
            this._p2_on_saver_receive_frame_data = new G2FUN_LISTENER(on_saver_receive_frame_data);
            this._p2_on_saver_receive_no_frame = new G2FUN_LISTENER(on_saver_receive_no_frame);
            this._p2_on_saver_receive_no_recorded_data = new G2FUN_LISTENER(on_saver_receive_no_recorded_data);
            this._p2_on_saver_receive_notify_play_speed_changed = new G2FUN_LISTENER(on_saver_receive_notify_play_speed_changed);
            this._p2_on_saver_receive_notify_play_stop_spot = new G2FUN_LISTENER(on_saver_receive_notify_play_stop_spot);
            this._p2_on_saver_receive_notify_end_of_play = new G2FUN_LISTENER(on_saver_receive_notify_end_of_play);
            this._p2_on_saver_receive_notify_command_end = new G2FUN_LISTENER(on_saver_receive_notify_command_end);
            this._p2_on_saver_receive_clipcopy_scope = new G2FUN_LISTENER(on_saver_receive_clipcopy_scope);
            this._p2_on_saver_receive_clipcopy_measure_size = new G2FUN_LISTENER(on_saver_receive_clipcopy_measure_size);
            this._p2_on_saver_receive_clipcopy_size = new G2FUN_LISTENER(on_saver_receive_clipcopy_size);
            this._p2_on_saver_receive_clipcopy_data = new G2FUN_LISTENER(on_saver_receive_clipcopy_data);
            this._p2_on_saver_receive_clipcopy_set_password = new G2FUN_LISTENER(on_saver_receive_clipcopy_set_password);
            this._p2_on_saver_receive_clipcopy_enable_channels = new G2FUN_LISTENER(on_saver_receive_clipcopy_enable_channels);
            this._p2_on_saver_receive_clipcopy_canceled = new G2FUN_LISTENER(on_saver_receive_clipcopy_canceled);
            this._p2_on_saver_receive_clipcopy_job_started = new G2FUN_LISTENER(on_saver_receive_clipcopy_job_started);
            this._p2_on_saver_receive_clipcopy_job_finished = new G2FUN_LISTENER(on_saver_receive_clipcopy_job_finished);
            this._p2_on_saver_receive_clipcopy_section_begin = new G2FUN_LISTENER(on_saver_receive_clipcopy_section_begin);
            this._p2_on_saver_receive_clipcopy_section_end = new G2FUN_LISTENER(on_saver_receive_clipcopy_section_end);
            this._p2_on_saver_receive_bank_space = new G2FUN_LISTENER(on_saver_receive_bank_space);
            this._p2_on_saver_receive_bank_image = new G2FUN_LISTENER(on_saver_receive_bank_image);
            this._p2_on_saver_receive_bank_audio = new G2FUN_LISTENER(on_saver_receive_bank_audio);
            this._p2_on_saver_receive_bank_no_image = new G2FUN_LISTENER(on_saver_receive_bank_no_image);
            this._p2_on_saver_receive_bank_no_audio = new G2FUN_LISTENER(on_saver_receive_bank_no_audio);
        }
        ~g2search()
        {
            cleanup();
        }

        private G2HSEARCH _handle;
        private G2UPARAM _param;
        private g2search_listener _listener;
        private g2search_listener_saver _listener_saver;

        #region GDK Callback Delegate
        private void register_callback(G2SEARCH_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_search_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_recorded_date;
        private G2FUN_LISTENER _p2_on_receive_recorded_time_hour;
        private G2FUN_LISTENER _p2_on_receive_recorded_time_minute;
        private G2FUN_LISTENER _p2_on_receive_recorded_rechour_minute;
        private G2FUN_LISTENER _p2_on_receive_frame_data;
        private G2FUN_LISTENER _p2_on_receive_no_frame;
        private G2FUN_LISTENER _p2_on_receive_no_recorded_data;
        private G2FUN_LISTENER _p2_on_receive_no_recorded_data_from_search_target;
        private G2FUN_LISTENER _p2_on_receive_find_idr_event_time;
        private G2FUN_LISTENER _p2_on_receive_notify_play_speed_changed;
        private G2FUN_LISTENER _p2_on_receive_notify_play_stop_post;
        private G2FUN_LISTENER _p2_on_receive_notify_end_of_play;
        private G2FUN_LISTENER _p2_on_receive_notify_segment_changed;
        private G2FUN_LISTENER _p2_on_receive_notify_search_mode_changed;
        private G2FUN_LISTENER _p2_on_receive_notify_command_end;
        private G2FUN_LISTENER _p2_on_receive_query_result_event;
        private G2FUN_LISTENER _p2_on_receive_query_result_text_in;
        private G2FUN_LISTENER _p2_on_receive_recorded_date_scope;
        private G2FUN_LISTENER _p2_on_receive_segment_spot;
        private G2FUN_LISTENER _p2_on_receive_error;
        private G2FUN_LISTENER _p2_on_receive_text_in;
        private G2FUN_LISTENER _p2_on_receive_external_tango_info;
        private G2FUN_LISTENER _p2_on_receive_gps_data_start;
        private G2FUN_LISTENER _p2_on_receive_gps_data;
        private G2FUN_LISTENER _p2_on_receive_gps_data_list;
        private G2FUN_LISTENER _p2_on_receive_gps_data_end;
        private G2FUN_LISTENER _p2_on_receive_gps_data_end_count;
        private G2FUN_LISTENER _p2_on_receive_gps_data_export_cancel_result;
        private G2FUN_LISTENER _p2_on_receive_gps_data_measure_result;
        private G2FUN_LISTENER _p2_on_require_prepare_playback;
        private G2FUN_LISTENER _p2_on_require_prepare_load_event_image;
        private G2FUN_LISTENER _p2_on_require_prepare_reload;
        private G2FUN_LISTENER _p2_on_probe_session_profile;
        private G2FUN_LISTENER _p2_on_saver_connected;
        private G2FUN_LISTENER _p2_on_saver_disconnected;
        private G2FUN_LISTENER _p2_on_saver_receive_recorded_date;
        private G2FUN_LISTENER _p2_on_saver_receive_frame_data;
        private G2FUN_LISTENER _p2_on_saver_receive_no_frame;
        private G2FUN_LISTENER _p2_on_saver_receive_no_recorded_data;
        private G2FUN_LISTENER _p2_on_saver_receive_notify_play_speed_changed;
        private G2FUN_LISTENER _p2_on_saver_receive_notify_play_stop_spot;
        private G2FUN_LISTENER _p2_on_saver_receive_notify_end_of_play;
        private G2FUN_LISTENER _p2_on_saver_receive_notify_command_end;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_scope;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_measure_size;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_size;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_data;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_set_password;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_enable_channels;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_canceled;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_job_started;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_job_finished;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_section_begin;
        private G2FUN_LISTENER _p2_on_saver_receive_clipcopy_section_end;
        private G2FUN_LISTENER _p2_on_saver_receive_bank_space;
        private G2FUN_LISTENER _p2_on_saver_receive_bank_image;
        private G2FUN_LISTENER _p2_on_saver_receive_bank_audio;
        private G2FUN_LISTENER _p2_on_saver_receive_bank_no_image;
        private G2FUN_LISTENER _p2_on_saver_receive_bank_no_audio;
        #endregion

        public G2HSEARCH safe_handle() { return _handle; }

        public void startup(int connections)
        {
            cleanup();

            _handle = g2_search_initialize(_param);

            #region GDK Callback Registration
            register_callback(G2SEARCH_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_recorded_date, _p2_on_receive_recorded_date);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_recorded_time_hour, _p2_on_receive_recorded_time_hour);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_recorded_time_minute, _p2_on_receive_recorded_time_minute);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_recorded_rechour_minute, _p2_on_receive_recorded_rechour_minute);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_frame_data, _p2_on_receive_frame_data);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_no_frame, _p2_on_receive_no_frame);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_no_recorded_data, _p2_on_receive_no_recorded_data);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_no_recorded_data_from_search_target, _p2_on_receive_no_recorded_data_from_search_target);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_find_idr_event_time, _p2_on_receive_find_idr_event_time);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_notify_play_speed_changed, _p2_on_receive_notify_play_speed_changed);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_notify_play_stop_post, _p2_on_receive_notify_play_stop_post);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_notify_end_of_play, _p2_on_receive_notify_end_of_play);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_notify_segment_changed, _p2_on_receive_notify_segment_changed);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_notify_search_mode_changed, _p2_on_receive_notify_search_mode_changed);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_notify_command_end, _p2_on_receive_notify_command_end);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_query_result_event, _p2_on_receive_query_result_event);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_query_result_text_in, _p2_on_receive_query_result_text_in);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_recorded_date_scope, _p2_on_receive_recorded_date_scope);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_segment_spot, _p2_on_receive_segment_spot);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_error, _p2_on_receive_error);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_text_in, _p2_on_receive_text_in);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_external_tango_info, _p2_on_receive_external_tango_info);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_gps_data_start, _p2_on_receive_gps_data_start);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_gps_data, _p2_on_receive_gps_data);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_gps_data_list, _p2_on_receive_gps_data_list);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_gps_data_end, _p2_on_receive_gps_data_end);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_gps_data_end_count, _p2_on_receive_gps_data_end_count);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_gps_data_export_cancel_result, _p2_on_receive_gps_data_export_cancel_result);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_receive_gps_data_measure_result, _p2_on_receive_gps_data_measure_result);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_require_prepare_playback, _p2_on_require_prepare_playback);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_require_prepare_load_event_image, _p2_on_require_prepare_load_event_image);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_require_prepare_reload, _p2_on_require_prepare_reload);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_probe_session_profile, _p2_on_probe_session_profile);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_connected, _p2_on_saver_connected);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_disconnected, _p2_on_saver_disconnected);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_recorded_date, _p2_on_saver_receive_recorded_date);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_frame_data, _p2_on_saver_receive_frame_data);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_no_frame, _p2_on_saver_receive_no_frame);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_no_recorded_data, _p2_on_saver_receive_no_recorded_data);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_notify_play_speed_changed, _p2_on_saver_receive_notify_play_speed_changed);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_notify_play_stop_spot, _p2_on_saver_receive_notify_play_stop_spot);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_notify_end_of_play, _p2_on_saver_receive_notify_end_of_play);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_notify_command_end, _p2_on_saver_receive_notify_command_end);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_scope, _p2_on_saver_receive_clipcopy_scope);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_measure_size, _p2_on_saver_receive_clipcopy_measure_size);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_size, _p2_on_saver_receive_clipcopy_size);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_data, _p2_on_saver_receive_clipcopy_data);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_set_password, _p2_on_saver_receive_clipcopy_set_password);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_enable_channels, _p2_on_saver_receive_clipcopy_enable_channels);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_canceled, _p2_on_saver_receive_clipcopy_canceled);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_job_started, _p2_on_saver_receive_clipcopy_job_started);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_job_finished, _p2_on_saver_receive_clipcopy_job_finished);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_section_begin, _p2_on_saver_receive_clipcopy_section_begin);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_clipcopy_section_end, _p2_on_saver_receive_clipcopy_section_end);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_bank_space, _p2_on_saver_receive_bank_space);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_bank_image, _p2_on_saver_receive_bank_image);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_bank_audio, _p2_on_saver_receive_bank_audio);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_bank_no_image, _p2_on_saver_receive_bank_no_image);
            register_callback(G2SEARCH_CALLBACK.TYPE.on_saver_receive_bank_no_audio, _p2_on_saver_receive_bank_no_audio);
            #endregion

            g2_search_startup(_handle, connections);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HSEARCH handle = _handle; _handle = 0;
                g2_search_cleanup(handle);
                g2_search_finalize(handle);
            }
        }
        public void set_listener(g2search_listener listener)
        {
            _listener = listener;
        }

        public int connect(ref G2GUID root, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_search_connect(_handle, ref root, ref options, out res);
        }
        public int connect(ref G2GUID root, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_search_connect(_handle, ref root, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, out G2CONNECT_RES res)
        {
            G2CONNECT_OPTIONS options = new G2CONNECT_OPTIONS();
            options.set_default();
            return g2_search_connect_ras(_handle, ref ni, ref options, out res);
        }
        public int connect_ras(ref G2NETWORK_INFO ni, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_search_connect_ras(_handle, ref ni, ref options, out res);
        }
        public void disconnect(int channel)
        {
            g2_search_disconnect(_handle, channel);
        }
        public bool is_connecting(int channel)
        {
            return g2_search_is_connecting(_handle, channel);
        }
        public bool is_connected(int channel)
        {
            return g2_search_is_connected(_handle, channel);
        }
        public bool is_disconnecting(int channel)
        {
            return g2_search_is_disconnecting(_handle, channel);
        }
        public bool is_disconnected(int channel)
        {
            return g2_search_is_disconnected(_handle, channel);
        }
        public bool is_disconnectable(int channel)
        {
            return g2_search_is_disconnectable(_handle, channel);
        }

        public void set_invoke_saver(int channel, g2search_listener_saver listener)
        {
            this._listener_saver = listener;
            g2_search_set_invoke_saver(_handle, channel);
        }
        public void set_revoke_saver(int channel)
        {
            g2_search_set_revoke_saver(_handle, channel);
            this._listener_saver = null;
        }
        public bool set_camera_list(int channel, uint cameras)
        {
            return g2_search_set_camera_list(_handle, channel, cameras);
        }
        public bool set_search_target(int channel, int target)
        {
            return g2_search_set_search_target(_handle, channel, target);
        }
        public bool set_change_segment(int channel, int id)
        {
            return g2_search_set_change_segment(_handle, channel, id);
        }
        public bool set_change_search_mode(int channel, bool event_search)
        {
            return g2_search_set_change_search_mode(_handle, channel, event_search);
        }
        public bool set_external_tango(int channel, int type, int number)
        {
            return g2_search_set_external_tango(_handle, channel, type, number);
        }
        public void set_query_mode(int channel, G2SEARCH_QUERY.MODE mode)
        {
            g2_search_set_query_mode(_handle, channel, (int)mode);
        }
        public void set_query_cameras(int channel, uint cameras)
        {
            g2_search_set_query_cameras(_handle, channel, cameras);
        }
        public void set_invoke_on_play(int channel, G2SPOT spot)
        {
            g2_search_set_invoke_on_play(_handle, channel, ref spot);
        }
        public void set_revoke_on_play(int channel)
        {
            g2_search_set_revoke_on_play(_handle, channel);
        }
        public void set_probe_session_profile(bool active)
        {
            g2_search_set_probe_session_profile(_handle, active);
        }

        public bool request_record_date(int channel)
        {
            return g2_search_request_record_date(_handle, channel);
        }
        public bool request_record_time(int channel, G2TIME date)
        {
            return g2_search_request_record_time(_handle, channel, ref date);
        }
        public bool request_record_hour(int channel, G2SPOT spot, int length, bool direction, bool check_diff_prev_req)
        {
            return g2_search_request_record_hour(_handle, channel, ref spot, length, direction, check_diff_prev_req);
        }
        public bool request_record_hour_command(int channel, G2SEARCH_PLAYBACK.COMMAND command, G2SPOT spot)
        {
            return g2_search_request_record_hour_command(_handle, channel, (int)command, ref spot);
        }
        public bool request_playback(int channel, G2SEARCH_PLAYBACK.COMMAND command)
        {
            return request_playback(channel, command, G2SPOT.INVALID_SPOT);
        }
        public bool request_playback(int channel, G2SEARCH_PLAYBACK.COMMAND command, G2SPOT spot)
        {
            return g2_search_request_playback(_handle, channel, (int)command, ref spot);
        }
        public bool request_pause(int channel, G2ROLLBACK_INFO rbi)
        {
            return g2_search_request_pause(_handle, channel, ref rbi);
        }
        public bool request_reload_current(int channel, int camera, G2SPOT spot)
        {
            return g2_search_request_reload_current(_handle, channel, camera, ref spot);
        }
        public bool request_notify_end_of_play(int channel)
        {
            return g2_search_request_notify_end_of_play(_handle, channel);
        }
        public bool request_query_specific_event(int channel, int seq_number, G2TIME begin, G2TIME end, int event_type, int camera)
        {
            return g2_search_request_query_specific_event(_handle, channel, seq_number, ref begin, ref end, event_type, camera);
        }
        public bool request_query_event(int channel, ref G2EVENT_QUERY_CONDITION condition)
        {
            return g2_search_request_query_event(_handle, channel, ref condition);
        }
        public bool request_query_text_in(int channel, ref G2TEXT_IN_QUERY_CONDITION condition)
        {
            return g2_search_request_query_text_in(_handle, channel, ref condition);
        }
        public bool request_event_image(int channel, int selected, bool last)
        {
            return g2_search_request_event_image(_handle, channel, selected, last);
        }
        public bool request_event_search_stop_idr(int channel)
        {
            return g2_search_request_event_search_stop_idr(_handle, channel);
        }
        public bool request_clipcopy_enable_channelset(int channel)
        {
            return g2_search_request_clipcopy_enable_channelset(_handle, channel);
        }
        public bool request_clipcopy_scope(int channel, G2TIME from, G2TIME to, uint cameras)
        {
            return g2_search_request_clipcopy_scope(_handle, channel, ref from, ref to, cameras);
        }
        public bool request_clipcopy_measure_size(int channel, G2SCOPE scope, ulong free_space, bool slice)
        {
            return g2_search_request_clipcopy_measure_size(_handle, channel, ref scope, free_space, slice);
        }
        public bool request_clipcopy_size(int channel)
        {
            return g2_search_request_clipcopy_size(_handle, channel);
        }
        public bool request_clipcopy(int channel, bool start)
        {
            return g2_search_request_clipcopy(_handle, channel, start);
        }
        public bool request_clipcopy_password(int channel, bool use, G2STRING_32 password)
        {
            return g2_search_request_clipcopy_password(_handle, channel, use, ref password);
        }
        public bool request_clipcopy_text_in(int channel, bool include)
        {
            return g2_search_request_clipcopy_text_in(_handle, channel, include);
        }
        public bool request_clipcopy_gps_data(int channel, bool include)
        {
            return g2_search_request_clipcopy_gps_data(_handle, channel, include);
        }
        public bool request_clipcopy_structure(int channel, int version, bool exclude_player, bool avoid_seek)
        {
            return g2_search_request_clipcopy_structure(_handle, channel, version, exclude_player, avoid_seek);
        }
        public bool request_clipcopy_cancel(int channel)
        {
            return g2_search_request_clipcopy_cancel(_handle, channel);
        }
        public bool request_mini_bank_begin(int channel)
        {
            return g2_search_request_mini_bank_begin(_handle, channel);
        }
        public bool request_mini_bank_end(int channel)
        {
            return g2_search_request_mini_bank_end(_handle, channel);
        }
        public bool request_mini_bank_space(int channel, G2TIME from, G2TIME to, uint cameras, bool audio)
        {
            return g2_search_request_mini_bank_space(_handle, channel, ref from, ref to, cameras, audio);
        }
        public bool request_mini_bank(int channel, int command, G2TIME time, int msec)
        {
            return g2_search_request_mini_bank(_handle, channel, command, ref time, msec);
        }
        public bool request_gps_data_measure(int channel, G2TIME from, G2TIME to)
        {
            return g2_search_request_gps_data_measure(_handle, channel, ref from, ref to);
        }
        public bool request_gps_data_measure_result(int channel)
        {
            return g2_search_request_gps_data_measure_result(_handle, channel);
        }
        public bool request_gps_data(int channel, G2TIME time)
        {
            return g2_search_request_gps_data(_handle, channel, ref time);
        }
        public bool request_gps_data_next(int channel)
        {
            return g2_search_request_gps_data_next(_handle, channel);
        }
        public bool request_gps_data_export_cancel(int channel)
        {
            return g2_search_request_gps_data_export_cancel(_handle, channel);
        }

        public G2FUN_GET_ADAPTOR get_adaptor()
        {
            return new G2FUN_GET_ADAPTOR(g2_search_get_adaptor);
        }
        public bool get_server_network_info(int channel, out G2SERVER_NETWORK_INFO ni)
        {
            return g2_search_get_server_network_info(_handle, channel, out ni);
        }
        public bool get_product_info(int channel, out G2_PRODUCT_INFO pi)
        {
            return g2_search_get_product_info(_handle, channel, out pi);
        }
        public bool get_remote_search_caps(int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH caps)
        {
            return g2_search_get_remote_search_caps(_handle, channel, out caps);
        }
        public bool get_remote_clipcopy_caps(int channel, out G2_PRODUCT_INFO_CAPS.REMOTE_CLIP_COPY caps)
        {
            return g2_search_get_remote_clipcopy_caps(_handle, channel, out caps);
        }
        public bool get_text_in_search_caps(int channel, out G2_PRODUCT_INFO_CAPS.TEXT_IN_SEARCH caps)
        {
            return g2_search_get_text_in_search_caps(_handle, channel, out caps);
        }
        public bool get_authority(int channel, out G2RAS_AUTHORITY auth)
        {
            return g2_search_get_authority(_handle, channel, out auth);
        }
        public bool get_camera_list(int channel, out uint cameras)
        {
            return g2_search_get_camera_list(_handle, channel, out cameras);
        }
        public bool get_time_last(int channel, out G2TIME time)
        {
            return g2_search_get_time_last(_handle, channel, out time);
        }
        public int get_drive_mode(int channel)
        {
            return g2_search_get_drive_mode(_handle, channel);
        }
        public int get_query_mode(int channel)
        {
            return g2_search_get_query_mode(_handle, channel);
        }
        public int get_timelapse_mode(int channel)
        {
            return g2_search_get_timelapse_mode(_handle, channel);
        }
        public int get_play_speed(int channel)
        {
            return g2_search_get_play_speed(_handle, channel);
        }
        public G2SEARCH_PLAYBACK.COMMAND get_current_command(int channel)
        {
            return (G2SEARCH_PLAYBACK.COMMAND)g2_search_get_current_command(_handle, channel);
        }
        public int get_search_target(int channel)
        {
            return g2_search_get_search_target(_handle, channel);
        }
        public bool get_query_cameras(int channel, out uint cameras)
        {
            return g2_search_get_query_cameras(_handle, channel, out cameras);
        }
        public bool get_query_condition_event(int channel, out G2EVENT_QUERY_CONDITION condition)
        {
            return g2_search_get_query_condition_event(_handle, channel, out condition);
        }
        public bool get_query_condition_text_in(int channel, out G2TEXT_IN_QUERY_CONDITION condition)
        {
            return g2_search_get_query_condition_text_in(_handle, channel, out condition);
        }
        public bool get_query_result_text_in(int channel, int count, G2TEXT_IN[] data)
        {
            return g2_search_get_query_result_text_in(_handle, channel, count, data);
        }
        public bool get_query_result_event(int channel, int selected, out G2SEARCH_LOG_INFO data)
        {
            return g2_search_get_query_result_event(_handle, channel, selected, out data);
        }
        public bool get_clipcopy_size_info(int channel, out G2CLIPCOPY_SIZE_INFO info)
        {
            return g2_search_get_clipcopy_size_info(_handle, channel, out info);
        }

        public bool is_rec_info_type(int channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE type)
        {
            return g2_search_is_rec_info_type(_handle, channel, (int)type);
        }
        public bool is_drive_mode(int channel, G2SEARCH_DRIVE.MODE mode)
        {
            return g2_search_is_drive_mode(_handle, channel, (int)mode);
        }
        public bool is_query_mode(int channel, G2SEARCH_QUERY.MODE mode)
        {
            return g2_search_is_query_mode(_handle, channel, (int)mode);
        }
        public bool is_timelapse_mode(int channel, int mode)
        {
            return g2_search_is_timelapse_mode(_handle, channel, mode);
        }
        public bool is_event_search_mode(int channel)
        {
            return g2_search_is_event_search_mode(_handle, channel);
        }
        public bool is_stopped(int channel)
        {
            return g2_search_is_stopped(_handle, channel);
        }
        public bool is_loading(int channel)
        {
            return g2_search_is_loading(_handle, channel);
        }
        public bool is_support(int channel, G2SEARCH_SUPPORT.QUERY query)
        {
            return g2_search_is_support(_handle, channel, (int)query);
        }
        public bool is_authority(int channel, G2RAS_AUTHORITY.TYPE authority)
        {
            return g2_search_is_authority(_handle, channel, (int)authority);
        }
        public bool is_probe_session_profile()
        {
            return g2_search_is_probe_session_profile(_handle);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_disconnected(_handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_recorded_date(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2TIME[] dates = new G2TIME[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2TIME)));
                dates[i] = (G2TIME)Marshal.PtrToStructure(ptr, typeof(G2TIME));
            }
            _listener.on_g2search_receive_recorded_date(handle, (int)wparam, dates);
            return 1;
        }
        private G2RESULT on_receive_recorded_time_hour(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            bool[][] hours = new bool[p._len][];

            for (int i = 0; i < p._len; ++i)
            {
                hours[i] = new bool[24];

                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * 24);
                byte[] buf = new byte[24];
                Marshal.Copy(ptr, buf, 0, 24);
                for (int j = 0; j < 24; ++j)
                {
                    hours[i][j] = (buf[j] != 0);
                }
            }
            _listener.on_g2search_receive_recorded_time_hour(handle, (int)wparam, hours, p._len);
            return 1;
        }
        private G2RESULT on_receive_recorded_time_minute(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            IntPtr p = (IntPtr)lparam;
            List<byte[]> minutes = new List<byte[]>(32);
            for (int i = 0; i < 32; ++i)
            {
                IntPtr ptr = new IntPtr(p.ToInt64() + i * 24 * 60);
                byte[] buf = new byte[24 * 60];
                Marshal.Copy(ptr, buf, 0, 24 * 60);
                minutes.Add(buf);
            }
            _listener.on_g2search_receive_recorded_time_minute(handle, (int)wparam, minutes);
            return 1;
        }
        private G2RESULT on_receive_recorded_rechour_minute(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2RECORD_TIME_INFO[] rti = new G2RECORD_TIME_INFO[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2RECORD_TIME_INFO)));
                rti[i] = (G2RECORD_TIME_INFO)Marshal.PtrToStructure(ptr, typeof(G2RECORD_TIME_INFO));
            }
            _listener.on_g2search_receive_recorded_rechour_minute(handle, (int)wparam, rti);
            return 1;
        }
        private G2RESULT on_receive_frame_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2search_receive_frame_data(handle, (int)wparam, ref frame);
            return 1;
        }
        private G2RESULT on_receive_no_frame(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_no_frame(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_no_recorded_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_no_recorded_data(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_no_recorded_data_from_search_target(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_TARGET.TYPE target = (G2SEARCH_TARGET.TYPE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_TARGET.TYPE));
            _listener.on_g2search_receive_no_recorded_data_from_search_target(handle, (int)wparam, target);
            return 1;
        }
        private G2RESULT on_receive_find_idr_event_time(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2TIME time = (G2TIME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2TIME));
            _listener.on_g2search_receive_find_idr_event_time(handle, (int)wparam, time);
            return 1;
        }
        private G2RESULT on_receive_notify_play_speed_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_notify_play_speed_changed(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_play_stop_post(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_DRIVE.MODE mode = (G2SEARCH_DRIVE.MODE)lparam;
            _listener.on_g2search_receive_notify_play_stop_post(handle, (int)wparam, mode);
            return 1;
        }
        private G2RESULT on_receive_notify_end_of_play(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_notify_end_of_play(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_notify_segment_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_notify_segment_changed(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_search_mode_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_notify_search_mode_changed(handle, (int)wparam, (G2SEARCH_MODE.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_command_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_notify_command_end(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_receive_query_result_event(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2SEARCH_LOG_INFO[] li = new G2SEARCH_LOG_INFO[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2SEARCH_LOG_INFO)));
                li[i] = (G2SEARCH_LOG_INFO)Marshal.PtrToStructure(ptr, typeof(G2SEARCH_LOG_INFO));
            }
            _listener.on_g2search_receive_query_result_event(handle, (int)wparam, li);
            return 1;
        }
        private G2RESULT on_receive_query_result_text_in(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2TEXT_IN[] data = new G2TEXT_IN[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2TEXT_IN)));
                data[i] = (G2TEXT_IN)Marshal.PtrToStructure(ptr, typeof(G2TEXT_IN));
            }
            _listener.on_g2search_receive_query_result_text_in(handle, (int)wparam, data);

            return 1;
        }
        private G2RESULT on_receive_recorded_date_scope(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2SCOPE[] scopes = new G2SCOPE[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2SCOPE)));
                scopes[i] = (G2SCOPE)Marshal.PtrToStructure(ptr, typeof(G2SCOPE));
            }
            _listener.on_g2search_receive_recorded_date_scope(handle, (int)wparam, scopes);
            return 1;
        }
        private G2RESULT on_receive_segment_spot(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2SPOT[] spots = new G2SPOT[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2SPOT)));
                spots[i] = (G2SPOT)Marshal.PtrToStructure(ptr, typeof(G2SPOT));
            }
            _listener.on_g2search_receive_segment_spot(handle, (int)wparam, spots);
            return 1;
        }
        private G2RESULT on_receive_error(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_error(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_text_in(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2TEXT_IN_ELEMENT data = (G2TEXT_IN_ELEMENT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2TEXT_IN_ELEMENT));
            _listener.on_g2search_receive_text_in(handle, (int)wparam, ref data);
            return 1;
        }
        private G2RESULT on_receive_external_tango_info(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2SEARCH_EXTERNAL_DISK[] edi = new G2SEARCH_EXTERNAL_DISK[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2SEARCH_EXTERNAL_DISK)));
                edi[i] = (G2SEARCH_EXTERNAL_DISK)Marshal.PtrToStructure(ptr, typeof(G2SEARCH_EXTERNAL_DISK));
            }
            _listener.on_g2search_receive_external_tango_info(handle, (int)wparam, edi);
            return 1;
        }
        private G2RESULT on_receive_gps_data_start(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_gps_data_start(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_gps_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2TEXT_IN data = (G2TEXT_IN)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2TEXT_IN));
            _listener.on_g2search_receive_gps_data(handle, (int)wparam, ref data);
            return 1;
        }
        private G2RESULT on_receive_gps_data_list(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2TEXT_IN[] data = new G2TEXT_IN[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2TEXT_IN)));
                data[i] = (G2TEXT_IN)Marshal.PtrToStructure(ptr, typeof(G2TEXT_IN));
            }
            _listener.on_g2search_receive_gps_data_list(handle, (int)wparam, data);
            return 1;
        }
        private G2RESULT on_receive_gps_data_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_gps_data_end(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_gps_data_end_count(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_gps_data_end_count(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_receive_gps_data_export_cancel_result(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2search_receive_gps_data_export_cancel_result(handle, (int)wparam, (int)lparam);
            return 1;
        }
        private G2RESULT on_receive_gps_data_measure_result(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_PARAM_GPS_DATA_MEASURE_RESULT p = (G2SEARCH_PARAM_GPS_DATA_MEASURE_RESULT)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_PARAM_GPS_DATA_MEASURE_RESULT));
            _listener.on_g2search_receive_gps_data_measure_result(handle, (int)wparam, p._count, p._done);
            return 1;
        }
        private G2RESULT on_require_prepare_playback(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_PARAM_PREPARE_PLAYBACK p = (G2SEARCH_PARAM_PREPARE_PLAYBACK)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_PARAM_PREPARE_PLAYBACK));
            _listener.on_g2search_require_prepare_playback(handle, (int)wparam, p._command, p._spot);
            return 1;
        }
        private G2RESULT on_require_prepare_load_event_image(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2SEARCH_PARAM_PREPARE_LOAD_EVENT_IMAGE p = (G2SEARCH_PARAM_PREPARE_LOAD_EVENT_IMAGE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_PARAM_PREPARE_LOAD_EVENT_IMAGE));
            return _listener.on_g2search_require_prepare_load_event_image(handle, (int)wparam, p._selected, p._last) ? G2BOOLEAN.G2TRUE : G2BOOLEAN.G2FALSE;
        }
        private G2RESULT on_require_prepare_reload(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            return _listener.on_g2search_require_prepare_reload(handle, (int)wparam) ? G2BOOLEAN.G2TRUE : G2BOOLEAN.G2FALSE;
        }
        private G2RESULT on_probe_session_profile(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PROBE_SESSION_PROFILE p = (G2PROBE_SESSION_PROFILE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PROBE_SESSION_PROFILE));
            _listener.on_g2search_probe_session_profile(handle, (int)wparam, ref p);
            return 1;
        }
        #endregion
        #region GDK Callback Handler Saver
        private G2RESULT on_saver_connected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_connected(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_disconnected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_recorded_date(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
                G2TIME[] dates = new G2TIME[p._len];
                for (int i = 0; i < p._len; ++i)
                {
                    IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2TIME)));
                    dates[i] = (G2TIME)Marshal.PtrToStructure(ptr, typeof(G2TIME));
                }
                _listener_saver.on_g2search_saver_receive_recorded_date(handle, (int)wparam, dates);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_frame_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
                _listener_saver.on_g2search_saver_receive_frame_data(handle, (int)wparam, ref frame);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_no_frame(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_no_frame(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_no_recorded_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_no_recorded_data(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_notify_play_speed_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_notify_play_speed_changed(handle, (int)wparam, (int)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_notify_play_stop_spot(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_notify_play_stop_spot(handle, (int)wparam, (G2SEARCH_DRIVE.MODE)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_notify_end_of_play(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_notify_end_of_play(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_notify_command_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_notify_command_end(handle, (int)wparam, (int)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_scope(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
                G2SCOPE[] scopes = new G2SCOPE[p._len];
                for (int i = 0; i < p._len; ++i)
                {
                    IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2SCOPE)));
                    scopes[i] = (G2SCOPE)Marshal.PtrToStructure(ptr, typeof(G2SCOPE));
                }
                _listener_saver.on_g2search_saver_receive_clipcopy_scope(handle, (int)wparam, scopes);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_measure_size(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_clipcopy_measure_size(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_size(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2SEARCH_PARAM_CLIPCOPY_SIZE p = (G2SEARCH_PARAM_CLIPCOPY_SIZE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_PARAM_CLIPCOPY_SIZE));
                _listener_saver.on_g2search_saver_receive_clipcopy_size(handle, (int)wparam, (G2CLIPCOPY_STATUS.TYPE)p._status, p._size, p._begin, p._end, ref p._info);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2SEARCH_PARAM_CLIPCOPY_DATA p = (G2SEARCH_PARAM_CLIPCOPY_DATA)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_PARAM_CLIPCOPY_DATA));
                _listener_saver.on_g2search_saver_receive_clipcopy_data(handle, (int)wparam, p._offset, p._size, p._data, p._progress, p._completed);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_set_password(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_clipcopy_set_password(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_enable_channels(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_clipcopy_enable_channels(handle, (int)wparam, (uint)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_canceled(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_clipcopy_canceled(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_job_started(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2CLIPCOPY_JOB param = (G2CLIPCOPY_JOB)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_JOB));
                _listener_saver.on_g2search_saver_receive_clipcopy_job_started(handle, (int)wparam, (G2CLIPCOPY_JOB.TYPE)param._job, param._num, param._total);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_job_finished(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2CLIPCOPY_JOB param = (G2CLIPCOPY_JOB)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_JOB));
                _listener_saver.on_g2search_saver_receive_clipcopy_job_finished(handle, (int)wparam, (G2CLIPCOPY_JOB.TYPE)param._job, param._num, param._total);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_section_begin(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
                uint[] vals = new uint[p._len];
                for (int i = 0; i < p._len; ++i)
                {
                    vals[i] = (uint)Marshal.ReadInt32((IntPtr)lparam, i * sizeof(uint));
                }
                _listener_saver.on_g2search_saver_receive_clipcopy_section_begin(handle, (int)wparam, vals[0], vals[1]);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_clipcopy_section_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
                uint[] vals = new uint[p._len];
                for (int i = 0; i < p._len; ++i)
                {
                    vals[i] = (uint)Marshal.ReadInt32((IntPtr)lparam, i * sizeof(uint));
                }
                _listener_saver.on_g2search_saver_receive_clipcopy_section_end(handle, (int)wparam, vals[0], vals[1]);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_bank_space(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                G2SEARCH_PARAM_BANK_SPACE p = (G2SEARCH_PARAM_BANK_SPACE)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2SEARCH_PARAM_BANK_SPACE));
                _listener_saver.on_g2search_saver_receive_bank_space(handle, (int)wparam, p._image_index_number, p._audio_index_number, p._start_time, p._start_msec, p._image_size, p._audio_size);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_bank_image(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_bank_image(handle, (int)wparam, (IntPtr)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_bank_audio(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_bank_audio(handle, (int)wparam, (IntPtr)lparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_bank_no_image(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_bank_no_audio(handle, (int)wparam);
            }
            return 1;
        }
        private G2RESULT on_saver_receive_bank_no_audio(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener_saver != null)
            {
                _listener_saver.on_g2search_saver_receive_bank_no_audio(handle, (int)wparam);
            }
            return 1;
        }
        #endregion
    }
}
