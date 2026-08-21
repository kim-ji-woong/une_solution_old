using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace GDK
{
    using G2HPLAY_SAVER = System.Int32;
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

    public interface g2play_saver_listener
    {
        /// <summary>
        /// @ on_g2play_saver_connected
        /// <para>callback when connected to the recording service</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_saver_connected(G2HPLAY_SAVER handle, int channel);
        /// <summary>
        /// @ on_g2play_saver_disconnected
        /// <para>callback when disconnected from the recording service</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="reason">reason for disconnection</param>
        void on_g2play_saver_disconnected(G2HPLAY_SAVER handle, int channel, G2DISCONNECT_REASON.TYPE reason);
        /// <summary>
        /// @ on_g2play_saver_receive_record_channels
        /// <para>callback for the device channel information registered to the recording service</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="channels">device channel information</param>
        void on_g2play_saver_receive_record_channels(G2HPLAY_SAVER handle, int channel, G2PLAY_CHANNEL_INFO[] channels);
        /// <summary>
        /// @ on_g2play_saver_receive_frame_data
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="frame">recorded image data</param>
        void on_g2play_saver_receive_frame_data(G2HPLAY_SAVER handle, int channel, ref G2FRAME frame);
        /// <summary>
        /// @ on_g2play_saver_receive_notify_out_of_scope
        /// <para>callback when the requested time is beyond the recording time scope</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="status">type</param>
        void on_g2play_saver_receive_notify_out_of_scope(G2HPLAY_SAVER handle, int channel, G2PLAYER.OUT_OF_SCOPE status);
        /// <summary>
        /// @ on_g2play_saver_receive_notify_player_error
        /// <para>callback for an error in recorded image playback</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="error">type</param>
        void on_g2play_saver_receive_notify_player_error(G2HPLAY_SAVER handle, int channel, G2PLAYER.PLAYER_ERROR error);
        /// <summary>
        /// @ on_g2play_saver_receive_scope_list
        /// <para>callback for the list of recorded image time scopes</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="scopes">recording time scope list</param>
        void on_g2play_saver_receive_scope_list(G2HPLAY_SAVER handle, int channel, G2SCOPE[] scopes);
        /// <summary>
        /// @ on_g2play_saver_receive_no_recorded_data
        /// <para>callback when there is no recorded image</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_saver_receive_no_recorded_data(G2HPLAY_SAVER handle, int channel);
        /// <summary>
        /// @ on_g2play_saver_receive_clipcopy_size
        /// <para>callback for clipcopy file size information</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="status">clipcopy status</param> 
        /// <param name="csi">clipcopy file size information</param>
        void on_g2play_saver_receive_clipcopy_size(G2HPLAY_SAVER handle, int channel, G2CLIPCOPY_STATUS.TYPE status, ref G2CLIPCOPY_SIZE_INFO csi);
        /// <summary>
        /// @ on_g2play_saver_receive_clipcopy_data
        /// <para>callback for clipcopy file data</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="offset">saved data offset from first position of clipcopy data</param> 
        /// <param name="size">total clipcopy size</param>
        /// <param name="data">clipcopy data</param>
        /// <param name="progress">progress value</param>
        void on_g2play_saver_receive_clipcopy_data(G2HPLAY_SAVER handle, int channel, ulong offset, uint size, IntPtr data, uint progress);
        /// <summary>
        /// @ on_g2play_saver_receive_clipcopy_canceled
        /// <para>callback when the saving of clipcopy file has been canceled</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        void on_g2play_saver_receive_clipcopy_canceled(G2HPLAY_SAVER handle, int channel);
        /// <summary>
        /// @ on_g2play_saver_receive_clipcopy_set_password
        /// <para>callback when a password has been set on clipcopy file</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="result">TRUE for successful password setting; FALSE for failed setting</param>
        void on_g2play_saver_receive_clipcopy_set_password(G2HPLAY_SAVER handle, int channel, uint result);
        
        /// <summary>
        /// @ on_g2play_saver_receive_clipcopy_job_started
        /// <para>callback when the saving of clipcopy file has started</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="job">clipcopy job type</param> 
        /// <param name="num">current clipcopy file number</param>
        /// <param name="total">clipcopy file total count</param>
        void on_g2play_saver_receive_clipcopy_job_started(G2HPLAY_SAVER handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total);
        
        /// <summary>
        /// @ on_g2play_saver_receive_clipcopy_job_finished
        /// <para>callback when the saving of clipcopy file has finished</para>
        /// </summary>
        /// <param name="handle">g2_play_saver control handle</param>
        /// <param name="channel">service connection channel</param>
        /// <param name="job">clipcopy job type</param> 
        /// <param name="num">current clipcopy file number</param> 
        /// <param name="total">clipcopy file total count</param>
        void on_g2play_saver_receive_clipcopy_job_finished(G2HPLAY_SAVER handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total);
    }

    public class g2play_saver
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_saver_register_callback(G2HPLAY_SAVER handle, uint type, G2FUN_LISTENER func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HPLAY_SAVER g2_play_saver_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_saver_finalize(G2HPLAY_SAVER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_saver_startup(G2HPLAY_SAVER handle, int connections);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_saver_cleanup(G2HPLAY_SAVER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_play_saver_connect(G2HPLAY_SAVER handle, ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_play_saver_disconnect(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_is_connecting(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_is_connected(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_is_disconnecting(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_is_disconnected(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_is_disconnectable(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_set_camera_list(G2HPLAY_SAVER handle, int channel, ref G2CHANNEL_SET channels, ref G2ROLLBACK_INFO rbi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_set_camera_list_interest(G2HPLAY_SAVER handle, int channel, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_record_channels(G2HPLAY_SAVER handle, int channel, G2GUID[] camera, uint count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_play(G2HPLAY_SAVER handle, int channel, ref G2PLAYBACK_COMMAND command);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_pause(G2HPLAY_SAVER handle, int channel, [MarshalAs(UnmanagedType.U1)] bool rollback, ref G2ROLLBACK_INFO rbi);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_move_to_spot(G2HPLAY_SAVER handle, int channel, ref G2SPOT spot, int precision, [MarshalAs(UnmanagedType.U1)] bool forward);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_notify_end_of_play(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_scope_list(G2HPLAY_SAVER handle, int channel, ref G2TIME from, ref G2TIME to, ref G2CHANNEL_SET channels);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_clipcopy_measure_size(G2HPLAY_SAVER handle, int channel, ref G2CHANNEL_SET channels, ref G2SCOPE scope, ulong free_space);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_clipcopy_password(G2HPLAY_SAVER handle, int channel, string password);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_clipcopy_text_in(G2HPLAY_SAVER handle, int channel, [MarshalAs(UnmanagedType.U1)] bool include);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_clipcopy_cancel(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_clipcopy_size(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_request_clipcopy_data(G2HPLAY_SAVER handle, int channel);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_play_saver_get_clipcopy_size_info(G2HPLAY_SAVER handle, int channel, out G2CLIPCOPY_SIZE_INFO csi);
        #endregion

        public g2play_saver()
        {
            this._handle = 0;
            this._param = new G2UPARAM(0);
            this._listener = null;
            this._p2_on_connected = new G2FUN_LISTENER(on_connected);
            this._p2_on_disconnected = new G2FUN_LISTENER(on_disconnected);
            this._p2_on_receive_record_channels = new G2FUN_LISTENER(on_receive_record_channels);
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
        ~g2play_saver()
        {
            cleanup();
        }

        private G2HPLAY_SAVER _handle;
        private G2UPARAM _param;
        private g2play_saver_listener _listener;

        #region GDK Callback Delegate
        private void register_callback(G2PLAY_SAVER_CALLBACK.TYPE id, G2FUN_LISTENER fn) { g2_play_saver_register_callback(_handle, (uint)id, fn); }
        private G2FUN_LISTENER _p2_on_connected;
        private G2FUN_LISTENER _p2_on_disconnected;
        private G2FUN_LISTENER _p2_on_receive_record_channels;
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

        public G2HPLAY_SAVER safe_handle() { return _handle; }

        /// <summary>
        /// @ startup
        /// <para>This method creates g2play_saver object, and registers g2play_saver callback functions.</para>
        /// </summary>
        /// <param name="connections">no. of channels to be connected simultaneously to the recording service</param>
        public void startup(int connections)
        {
            cleanup();

            _handle = g2_play_saver_initialize(_param);

            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_connected, _p2_on_connected);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_disconnected, _p2_on_disconnected);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_record_channels, _p2_on_receive_record_channels);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_frame_data, _p2_on_receive_frame_data);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_notify_out_of_scope, _p2_on_receive_notify_out_of_scope);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_notify_player_error, _p2_on_receive_notify_player_error);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_scope_list, _p2_on_receive_scope_list);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_no_recorded_data, _p2_on_receive_no_recorded_data);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_clipcopy_size, _p2_on_receive_clipcopy_size);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_clipcopy_data, _p2_on_receive_clipcopy_data);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_clipcopy_canceled, _p2_on_receive_clipcopy_canceled);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_clipcopy_set_password, _p2_on_receive_clipcopy_set_password);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_clipcopy_job_started, _p2_on_receive_clipcopy_job_started);
            register_callback(G2PLAY_SAVER_CALLBACK.TYPE.on_receive_clipcopy_job_finished, _p2_on_receive_clipcopy_job_finished);

            g2_play_saver_startup(_handle, connections);
        }
        /// <summary>
        /// @ cleanup
        /// <para>This method deletes the memory and resources created by g2_play_saver startup().</para>
        /// </summary>
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HPLAY_SAVER handle = _handle; _handle = 0;
                g2_play_saver_cleanup(handle);
                g2_play_saver_finalize(handle);
            }
        }
        
        /// <summary>
        /// @ set_listener
        /// <para>This method registers g2play_listener function.</para>
        /// </summary>
        /// <param name="listener">listener function</param>
        public void set_listener(g2play_saver_listener listener)
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
            return g2_play_saver_connect(_handle, ref service, ref options, out res);
        }

        /// <summary>
        /// @ connect
        /// <para>This method connects to the recording service.</para>
        /// </summary>
        /// <param name="service">recording service unique id</param>
        /// <param name="options">The structure wherein the options for service connection is saved. It has information about connection time out.</param>
        /// <param name="res">n this out parameter, connection result information will be stored.
        /// This parameter will contain error type id, and session id information after the method call.
        /// The session id will only be valid when the connection has been established successfully.
        /// User can check error type id to figure out what was the reason for disconnection.</param>
        /// <returns>If a connection fails, it returns -1; otherwise, it returns channel no. (other than -1).</returns>
        public int connect(ref G2GUID service, ref G2CONNECT_OPTIONS options, out G2CONNECT_RES res)
        {
            return g2_play_saver_connect(_handle, ref service, ref options, out res);
        }
        
        /// <summary>
        /// @ disconnect
        /// <para>This method disconnects the recording service for the corresponding channel.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        public void disconnect(int channel)
        {
            g2_play_saver_disconnect(_handle, channel);
        }

        /// <summary>
        /// @ is_connecting
        /// <para>This method checks whether the corresponding channel is connecting to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connecting(int channel)
        {
            return g2_play_saver_is_connecting(_handle, channel);
        }

        /// <summary>
        /// @ is_connected
        /// <para>This method checks whether the corresponding channel is connected to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If connected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_connected(int channel)
        {
            return g2_play_saver_is_connected(_handle, channel);
        }

        /// <summary>
        /// @ is_disconnecting
        /// <para>This method checks whether the corresponding channel is disconnecting from the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If in the process of disconnecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnecting(int channel)
        {
            return g2_play_saver_is_disconnecting(_handle, channel);
        }

        /// <summary>
        /// @ is_disconnected
        /// <para>This method checks whether the corresponding channel is disconnected from the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If connected, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnected(int channel)
        {
            return g2_play_saver_is_disconnected(_handle, channel);
        }
        /// <summary>
        /// @ is_disconnectable
        /// <para>This method checks whether the corresponding channel is connected or in the process of connecting to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If connected or in the process of connecting, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_disconnectable(int channel)
        {
            return g2_play_saver_is_disconnectable(_handle, channel);
        }

        /// <summary>
        /// @ set_camera_list
        /// <para>This method requests recorded images for the specified camera channel to the recording service.</para>
        /// </summary>
        /// <param name="channel">streaming service channel</param>
        /// <param name="channels">camera channel list</param>
        /// <param name="rbi">structure wherein rollback information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_camera_list(int channel, g2channel_set channels, ref G2ROLLBACK_INFO rbi)
        {
            G2CHANNEL_SET chs = channels;
            return g2_play_saver_set_camera_list(_handle, channel, ref chs, ref rbi);
        }

        /// <summary>
        /// @ set_camera_list_interest
        /// <para>This method sets the channel of interest on the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="channels">channel list of interest</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool set_camera_list_interest(int channel, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_play_saver_set_camera_list_interest(_handle, channel, ref chs);
        }

        /// <summary>
        /// @ request_record_channels
        /// <para>This method requests the channel information for all devices on the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="GUIDs">recording service guid list</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_record_channels(int channel, G2GUIDSET GUIDs)
        {
            G2GUID[] param = GUIDs.to_array();
            return g2_play_saver_request_record_channels(_handle, channel, param, (uint)param.Length);
        }

        /// <summary>
        /// @ request_play
        /// <para>This method requests recorded image playback to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="command">structure wherein playback command information is saved</param>
        /// <returns></returns>
        public bool request_play(int channel, ref G2PLAYBACK_COMMAND command)
        {
            return g2_play_saver_request_play(_handle, channel, ref command);
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
            return g2_play_saver_request_pause(_handle, channel, rollback, ref rbi);
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
        public bool request_move_to_spot(int channel, ref G2SPOT spot, G2PLAYER.PRECISION precision, bool forward)
        {
            return g2_play_saver_request_move_to_spot(_handle, channel, ref spot, (int)precision, forward);
        }

        /// <summary>
        /// @ request_notify_end_of_play
        /// <para>This method notifies the recording service that there is no more frame received.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_notify_end_of_play(int channel)
        {
            return g2_play_saver_request_notify_end_of_play(_handle, channel);
        }

        /// <summary>
        /// @ request_scope_list
        /// <para>This method requests the time scope list of recorded images for the specified device channel to the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="from">start time</param>
        /// <param name="to">end time</param>
        /// <param name="channels">structure wherein device channel information is saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_scope_list(int channel, ref G2TIME from, ref G2TIME to, g2channel_set channels)
        {
            G2CHANNEL_SET chs = channels;
            return g2_play_saver_request_scope_list(_handle, channel, ref from, ref to, ref chs);
        }

        /// <summary>
        /// @ request_clipcopy_measure_size
        /// <para>This method sets the maximum size of clipcopy file on the recording service.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="channels">structure wherein device channel information is saved</param>
        /// <param name="scope">time scope</param>
        /// <param name="free_space">available disk space of the drive where clipcopy file is to be saved</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_clipcopy_measure_size(int channel, g2channel_set channels, ref G2SCOPE scope, ulong free_space)
        {
            G2CHANNEL_SET chs = channels;
            return g2_play_saver_request_clipcopy_measure_size(_handle, channel, ref chs, ref scope, free_space);
        }

        /// <summary>
        /// @ request_clipcopy_password
        /// <para>This method sets the password on clipcopy file.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="password">password</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_clipcopy_password(int channel, string password)
        {
            return g2_play_saver_request_clipcopy_password(_handle, channel, password);
        }
        /// <summary>
        /// @ request_clipcopy_text_in
        /// <para>This method set whether or not include text in data</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="include">whether or not include text in data</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_clipcopy_text_in(int channel, bool include)
        {
            return g2_play_saver_request_clipcopy_text_in(_handle, channel, include);
        }
        /// <summary>
        /// @ request_clipcopy_cancel
        /// <para>This method cancels the saving of clipcopy.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_clipcopy_cancel(int channel)
        {
            return g2_play_saver_request_clipcopy_cancel(_handle, channel);
        }

        /// <summary>
        /// @ request_clipcopy_size
        /// <para>This method requests the size of the clipcopy file that is currently being saved via a callback.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_clipcopy_size(int channel)
        {
            return g2_play_saver_request_clipcopy_size(_handle, channel);
        }

        /// <summary>
        /// @ request_clipcopy_data
        /// <para>This method requests clipcopy file data via a callback.</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool request_clipcopy_data(int channel)
        {
            return g2_play_saver_request_clipcopy_data(_handle, channel);
        }

        /// <summary>
        /// @ get_clipcopy_size_info
        /// <para>This method obtains clipcopy size information</para>
        /// </summary>
        /// <param name="channel">recording service channel</param>
        /// <param name="csi">structure to save clipcopy size information</param>
        /// <returns>If a request succeeds, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool get_clipcopy_size_info(int channel, out G2CLIPCOPY_SIZE_INFO csi) 
        {
            return g2_play_saver_get_clipcopy_size_info(_handle, channel, out csi);
        }

        #region GDK Callback Handler
        private G2RESULT on_connected(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_saver_connected(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_disconnected(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_saver_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON.TYPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_record_channels(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PARAM_BUNCH p = (G2PARAM_BUNCH)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PARAM_BUNCH));
            G2PLAY_CHANNEL_INFO[] channels = new G2PLAY_CHANNEL_INFO[p._len];
            for (int i = 0; i < p._len; ++i)
            {
                IntPtr ptr = new IntPtr(p._params.ToInt64() + i * Marshal.SizeOf(typeof(G2PLAY_CHANNEL_INFO)));
                channels[i] = (G2PLAY_CHANNEL_INFO)Marshal.PtrToStructure(ptr, typeof(G2PLAY_CHANNEL_INFO));
            }
            _listener.on_g2play_saver_receive_record_channels(handle, (int)wparam, channels);
            return 1;
        }

        private G2RESULT on_receive_frame_data(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2FRAME frame = (G2FRAME)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2FRAME));
            _listener.on_g2play_saver_receive_frame_data(handle, (int)wparam, ref frame);
            return 1;
        }
        private G2RESULT on_receive_notify_out_of_scope(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_saver_receive_notify_out_of_scope(handle, (int)wparam, (G2PLAYER.OUT_OF_SCOPE)lparam);
            return 1;
        }
        private G2RESULT on_receive_notify_player_error(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_saver_receive_notify_player_error(handle, (int)wparam, (G2PLAYER.PLAYER_ERROR)lparam);
            return 1;
        }
        private G2RESULT on_receive_scope_list(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PLAY_SCOPE_LIST scope = (G2PLAY_SCOPE_LIST)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PLAY_SCOPE_LIST));
            G2SCOPE[] scopes = scope._list.to();
            _listener.on_g2play_saver_receive_scope_list(handle, (int)wparam, scopes);
            return 1;
        }
        private G2RESULT on_receive_no_recorded_data(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_saver_receive_no_recorded_data(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_size(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO param = (G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO));
            _listener.on_g2play_saver_receive_clipcopy_size(handle, (int)wparam, (G2CLIPCOPY_STATUS.TYPE)param._status, ref param._info);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_data(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            G2CLIPCOPY_DATA data = (G2CLIPCOPY_DATA)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_DATA));
            _listener.on_g2play_saver_receive_clipcopy_data(handle, (int)wparam, data._offset, data._size, data._data, data._progress);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_canceled(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_saver_receive_clipcopy_canceled(handle, (int)wparam);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_set_password(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            _listener.on_g2play_saver_receive_clipcopy_set_password(handle, (int)wparam, (uint)lparam);
            return 1;
        }
        private G2RESULT on_receive_clipcopy_job_started(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener != null)
            {
                G2CLIPCOPY_JOB param = (G2CLIPCOPY_JOB)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_JOB));
                _listener.on_g2play_saver_receive_clipcopy_job_started(handle, (int)wparam,  (G2CLIPCOPY_JOB.TYPE)param._job, param._num, param._total);
            }
            return 1;
        }
        private G2RESULT on_receive_clipcopy_job_finished(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
        {
            if (_listener != null)
            {
                G2CLIPCOPY_JOB param = (G2CLIPCOPY_JOB)Marshal.PtrToStructure((IntPtr)lparam, typeof(G2CLIPCOPY_JOB));
                _listener.on_g2play_saver_receive_clipcopy_job_finished(handle, (int)wparam, (G2CLIPCOPY_JOB.TYPE)param._job, param._num, param._total);
            }
            return 1;
        }
        #endregion
    }
}
