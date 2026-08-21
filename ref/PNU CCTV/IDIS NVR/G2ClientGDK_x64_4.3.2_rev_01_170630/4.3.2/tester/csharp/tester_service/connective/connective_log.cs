using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    public partial class form_log : g2play_listener,
                                    g2live_listener,
                                    g2backup_listener
    {
        private g2live _adaptor_live;
        private g2play _adaptor_play;
        private g2backup _adaptor_backup;
        delegate void ctrl_invoke();

        public void main(ref G2GUID guid)
        {
            init_connective_log(ref guid);
            adaptor_connect(ref guid);
        }

        public void init_connective_log(ref G2GUID guid)
        {
            if (guid.is_service_streaming)
            {
                _adaptor_live = new g2live();
                _adaptor_live.set_listener(this);
                _adaptor_live.startup(2, Handle);
            }
            else if (guid.is_service_recording || guid.is_service_recording_failover)
            {
                _adaptor_play = new g2play();
                _adaptor_play.set_listener(this);
                _adaptor_play.startup(1);
            }
            else if (guid.is_service_backup)
            {
                _adaptor_backup = new g2backup();
                _adaptor_backup.set_listener(this);
                _adaptor_backup.startup(1);
            }

            if (guid.is_service_recording == false)
            {
                CMB_LOG.Items.RemoveAt(2);
            }
        }

        public void adaptor_connect(ref G2GUID guid)
        {
            G2CONNECT_RES res;
            g2admin admin = g2admin.get();
            g2monitor monitor = g2monitor.get();
            if (_is_admin)
            {
                if (admin.is_connected() == true)
                {
                    admin.request_system_log_search(soption);
                    admin.request_debug_log_search(doption);
                }
            }
            else if (guid.is_service_streaming)
            {
                _adaptor_live.connect(ref guid, out res);
            }
            else if (guid.is_service_recording || guid.is_service_recording_failover)
            {
                _adaptor_play.connect(ref guid, out res);
            }
            else if (guid.is_service_monitoring)
            {
                if (monitor.is_connected() == true)
                {
                    monitor.request_system_log_search(soption);
                    monitor.request_debug_log_search(doption);
                }
            }
            else if (guid.is_service_backup)
            {
                G2GUID temp = new G2GUID();
                _adaptor_backup.connect(ref guid, ref temp, out res);
            }
        }

        public void add_system_log(G2SYSTEM_LOG log)
        {
            _syslog_list.Add(log);
        }

        public void add_debug_log(G2DEBUG_LOG log)
        {
            _debuglog_list.Add(log);
        }
        public void add_device_log(G2SYSTEM_LOG log)
        {
            _devicelog_list.Add(log);
        }

        public string service_name(G2GUID guid)
        {
            string str = "";
            if (guid.is_service_administration)
            {
                str = "<Admin>";
            }
            else if (guid.is_service_monitoring)
            {
                str = "<Monitoring>";
            }
            else if (guid.is_service_streaming)
            {
                str = "<Streaming>";
            }
            else if (guid.is_service_recording)
            {
                str = "<Recording>";
            }
            else if (guid.is_service_backup)
            {
                str = "<Backup>";
            }
            else if (guid.is_service_recording_failover)
            {
                str = "<Recording Failover>";
            }
            return str;
        }

        public void log_load_end_update_list()
        {
            if (this.InvokeRequired)
            {
                ctrl_invoke del = new ctrl_invoke(log_load_end_update_list);
                Invoke(del);
            }
            else
            {
                if (num == CMB_LOG.SelectedIndex)
                {
                    update_log_list(null, null);
                }
            }
        }

        #region g2live_listener 멤버

        public void on_g2live_connected(int handle, int channel)                                                                
        {
           
            _adaptor_live.request_system_log_search(channel, soption);
            _adaptor_live.request_debug_log_search(channel, doption);
            _channel = channel;
        }

        public void on_g2live_receive_stream_channels(int handle, int channel, Dictionary<G2GUID, G2LIVE_CHANNEL_INFO> chs){}
        public void on_g2live_disconnected(int handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            if (reason == G2DISCONNECT_REASON.TYPE.NO_SERVER)
            {
                str[0] = "No Server running.";
                str[1] = "No Server running.";
                log_load_end_update_list();
            }
            _channel = -1;
        }
        public void on_g2live_probe_session_profile(int handle, int channel, ref G2PROBE_SESSION_PROFILE probe) {}
        public void on_g2live_receive_audio_out_not_available(int handle, int channel, ref G2GUID root) {}
        public void on_g2live_receive_camera_status(int handle, int channel, G2LIVE_CAMERA_STATUS[] bunch) {}
        public void on_g2live_receive_event(int handle, int channel, ref G2EVENT_INFO ei) {}
        public void on_g2live_receive_frame_data(int handle, int channel, ref G2FRAME frame) {}
        public void on_g2live_receive_network_alarm_result(int handle, int channel, ref G2GUID root, ref G2LIVE_NETWORK_ALARM_RESULT result) {}
        public void on_g2live_receive_notify_append_device(int handle, int channel, ref G2GUID root) {}
        public void on_g2live_receive_notify_remove_device(int handle, int channel, ref G2GUID root) {}
        public void on_g2live_receive_ptz_menu(int handle, int channel, ref G2LIVE_PTZ_MENU menu) {}
        public void on_g2live_receive_ptz_preset(int handle, int channel, ref G2LIVE_PTZ_PRESET preset) {}
        public void on_g2live_receive_service_log_load(int handle, int channel, ref G2SYSTEM_LOG log)
        {
            add_system_log(log);
            _s_row_id = log._row_id;
        }
        public void on_g2live_receive_service_log_load_end(int handle, int channel)
        {
            str[0] = "Complete!";
            log_load_end_update_list();
        }
        public void on_g2live_receive_service_log_load_fail(int handle, int channel) {}
        public void on_g2live_receive_service_log_load_stop(int handle, int channel) {}
        public void on_g2live_receive_debug_log_load(int handle, int channel, ref G2DEBUG_LOG log)
        {
            add_debug_log(log);
            _d_row_id = log._row_id;
        }
        public void on_g2live_receive_debug_log_load_end(int handle, int channel)
        {
            str[1] = "Complete!";
            log_load_end_update_list();
        }
        public void on_g2live_receive_debug_log_load_fail(int handle, int channel) {}
        public void on_g2live_receive_debug_log_load_stop(int handle, int channel) {}
        public void on_g2live_receive_text_in(int handle, int channel, ref G2TEXT_IN data) {}
        public void on_g2live_sound_capturing_started(int handle, int channel, ref G2GUID root, int camera) {}
        public void on_g2live_sound_capturing_stopped(int handle, int channel, ref G2GUID root, int camera) {}
        public void on_g2live_sound_streaming_started(int handle, int channel, int channelext) {}
        public void on_g2live_sound_streaming_stopped(int handle, int channel, int channelext) {}

        #endregion


        #region g2play_listener 멤버

        public void on_g2play_connected(int handle, int channel)
        {
            _adaptor_play.request_system_log_search(channel, soption);
            _adaptor_play.request_debug_log_search(channel, doption);
            _adaptor_play.request_device_log_search(channel, dev_option);
            _channel = channel;
        }
        public void on_g2play_disconnected(int handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            if (reason == G2DISCONNECT_REASON.TYPE.NO_SERVER)
            {
                str[0] = "No Server running.";
                str[1] = "No Server running.";
                str[2] = "No Server running.";
                log_load_end_update_list();
            }
            _channel = -1;
        }
        public void on_g2play_probe_session_profile(int handle, int channel, ref G2PROBE_SESSION_PROFILE probe) {}
        public void on_g2play_query_options_search_base(int handle, int channel, ref G2PLAY_OPTIONS_SEARCH_BASE options) {}
        public void on_g2play_receive_device_log_load(int handle, int channel, ref G2SYSTEM_LOG log)
        { 
            add_device_log(log);
            _dev_row_id = log._row_id;
        }
        public void on_g2play_receive_device_log_load_end(int handle, int channel)
        {
            str[2] = "Complete!";
            log_load_end_update_list();
        }
        public void on_g2play_receive_device_log_load_fail(int handle, int channel) { }
        public void on_g2play_receive_device_log_load_stop(int handle, int channel) { }
        public void on_g2play_receive_record_failover_scope(int handle, int channel, G2FAILOVER_SCOPE_INFO[] infos) {}
        public void on_g2play_receive_event_log_load(int handle, int channel, ref G2EVENT_LOG log) {}
        public void on_g2play_receive_event_log_load_end(int handle, int channel) {}
        public void on_g2play_receive_event_log_load_fail(int handle, int channel) {}
        public void on_g2play_receive_event_log_load_stop(int handle, int channel) {}
        public void on_g2play_receive_frame_data(int handle, int channel, ref G2FRAME frame) {}
        public void on_g2play_receive_hard_disk_info(int handle, int channel, G2PLAY_HARD_DISK_INFO[] dis) {}
        public void on_g2play_receive_no_recorded_data(int handle, int channel) {}
        public void on_g2play_receive_notify_command_begin(int handle, int channel, G2PLAYER.COMMAND_AND_SPEED command) {}
        public void on_g2play_receive_notify_command_end(int handle, int channel, G2PLAYER.COMMAND_AND_SPEED command) {}
        public void on_g2play_receive_notify_frame_not_found(int handle, int channel, ref G2SPOT spot, G2PLAYER.PRECISION precision) {}
        public void on_g2play_receive_notify_out_of_scope(int handle, int channel, G2PLAYER.OUT_OF_SCOPE status) {}
        public void on_g2play_receive_notify_play_speed_changed(int handle, int channel, G2PLAYER.COMMAND_AND_SPEED speed) {}
        public void on_g2play_receive_notify_player_error(int handle, int channel, G2PLAYER.PLAYER_ERROR error) {}
        public void on_g2play_receive_record_channels(int handle, int channel, G2PLAY_CHANNEL_INFO[] channels) {}
        public void on_g2play_receive_record_time_info_load(int handle, int channel, ref G2RECORD_TIME_INFO rti) {}
        public void on_g2play_receive_record_time_info_load_end(int handle, int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.COMMAND command) {}
        public void on_g2play_receive_scope_list(int handle, int channel, G2SCOPE[] scopes, int type) {}
        public void on_g2play_receive_service_log_load(int handle, int channel, ref G2SYSTEM_LOG log)
        {
            add_system_log(log);
            _s_row_id = log._row_id;
        }
        public void on_g2play_receive_service_log_load_end(int handle, int channel)
        {
            str[0] = "Complete!";
            log_load_end_update_list();
        }
        public void on_g2play_receive_service_log_load_fail(int handle, int channel) {}
        public void on_g2play_receive_service_log_load_stop(int handle, int channel) {}
        public void on_g2play_receive_debug_log_load(int handle, int channel, ref G2DEBUG_LOG log)
        {
            add_debug_log(log);
            _d_row_id = log._row_id;
        }
        public void on_g2play_receive_debug_log_load_end(int handle, int channel)
        {
            str[1] = "Complete!";
            log_load_end_update_list();
        }
        public void on_g2play_receive_debug_log_load_fail(int handle, int channel) {}
        public void on_g2play_receive_debug_log_load_stop(int handle, int channel) {}
        public void on_g2play_receive_spot_list(int handle, int channel, G2SPOT[] spots) {}
        public void on_g2play_receive_text_in(int handle, int channel, ref G2TEXT_IN textIn) {}
        public void on_g2play_receive_text_in_log_load(int handle, int channel, ref G2EVENT log) {}
        public void on_g2play_receive_text_in_log_load_end(int handle, int channel) {}
        public void on_g2play_receive_text_in_log_load_fail(int handle, int channel) {}
        public void on_g2play_receive_text_in_log_load_stop(int handle, int channel) {}
        public void on_g2play_require_prepare_rollback(int handle, int channel, bool prepare) {}

        #endregion


        #region g2backup_listener 멤버

        public void on_g2backup_connected(int handle, int channel, ref G2GUID site) 
        {
            _adaptor_backup.request_system_log_search(channel, soption);
            _adaptor_backup.request_debug_log_search(channel, doption);
            _channel = channel;
        }
        public void on_g2backup_disconnected(int handle, int channel, G2DISCONNECT_REASON.TYPE reason) 
        {
            if (reason == G2DISCONNECT_REASON.TYPE.NO_SERVER)
            {
                str[0] = "No Server running.";
                str[1] = "No Server running.";
                log_load_end_update_list();
            }
            _channel = -1;
        }
        public void on_g2backup_receive_backup_site_result(int handle, int channel, ref G2BACKUP_SITE_RESULT result) { }
        public void on_g2backup_receive_record_channels(int handle, int channel, G2BACKUP_CHANNEL_INFO[] channels) { }
        public void on_g2backup_receive_record_time_info_load(int handle, int channel, ref G2RECORD_TIME_INFO rti) { }
        public void on_g2backup_receive_record_time_info_load_end(int handle, int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.COMMAND command) { }
        public void on_g2backup_receive_response_no_recorded_data(int handle, int channel, G2BACKUP_CHANNEL_INFO[] channels) { }
        public void on_g2backup_receive_frame_data(int handle, int channel, ref G2FRAME frame) { }
        public void on_g2backup_receive_text_in(int handle, int channel, ref G2TEXT_IN textIn) { }
        public void on_g2backup_receive_notify_command_begin(int handle, int channel, G2PLAYER.COMMAND_AND_SPEED command) { }
        public void on_g2backup_receive_notify_command_end(int handle, int channel, G2PLAYER.COMMAND_AND_SPEED command) { }
        public void on_g2backup_receive_notify_play_speed_changed(int handle, int channel, G2PLAYER.COMMAND_AND_SPEED speed) { }
        public void on_g2backup_receive_notify_frame_not_found(int handle, int channel, ref G2SPOT spot, G2PLAYER.PRECISION precision) { }
        public void on_g2backup_receive_notify_out_of_scope(int handle, int channel, G2PLAYER.OUT_OF_SCOPE playtype) { }
        public void on_g2backup_receive_notify_player_error(int handle, int channel, G2PLAYER.PLAYER_ERROR errorcode) { }
        public void on_g2backup_receive_scope_list(int handle, int channel, G2SCOPE[] scopes, G2PLAY_SCOPE_TYPE.TYPE type) { }
        public void on_g2backup_receive_spot_list(int handle, int channel, G2SPOT[] spots) { }
        public void on_g2backup_receive_no_recorded_data(int handle, int channel) { }
        public void on_g2backup_receive_event_log_load(int handle, int channel, ref G2EVENT_LOG log) { }
        public void on_g2backup_receive_event_log_load_end(int handle, int channel) { }
        public void on_g2backup_receive_event_log_load_fail(int handle, int channel) { }
        public void on_g2backup_receive_event_log_load_stop(int handle, int channel) { }
        public void on_g2backup_receive_text_in_log_load(int handle, int channel, ref G2EVENT log) { }
        public void on_g2backup_receive_text_in_log_load_end(int handle, int channel) { }
        public void on_g2backup_receive_text_in_log_load_fail(int handle, int channel) { }
        public void on_g2backup_receive_text_in_log_load_stop(int handle, int channel) { }
        public void on_g2backup_receive_service_log_load(int handle, int channel, ref G2SYSTEM_LOG log)
        {
            add_system_log(log);
            _s_row_id = log._row_id;
        }
        public void on_g2backup_receive_service_log_load_end(int handle, int channel) 
        {
            str[0] = "Complete!";
            log_load_end_update_list();
        }
        public void on_g2backup_receive_service_log_load_fail(int handle, int channel) { }
        public void on_g2backup_receive_service_log_load_stop(int handle, int channel) { }
        public void on_g2backup_require_prepare_rollback(int handle, int channel, bool prepare) { }
        public void on_g2backup_probe_session_profile(int handle, int channel, ref G2PROBE_SESSION_PROFILE probe) { }
        public void on_g2backup_receive_debug_log_load(int handle, int channel, ref G2DEBUG_LOG log) 
        {
            add_debug_log(log);
            _d_row_id = log._row_id;
        }
        public void on_g2backup_receive_debug_log_load_end(int handle, int channel) 
        {
            str[1] = "Complete!";
            log_load_end_update_list();
        }
        public void on_g2backup_receive_debug_log_load_fail(int handle, int channel) { }
        public void on_g2backup_receive_debug_log_load_stop(int handle, int channel) { }

        #endregion
    }   
}        
       
