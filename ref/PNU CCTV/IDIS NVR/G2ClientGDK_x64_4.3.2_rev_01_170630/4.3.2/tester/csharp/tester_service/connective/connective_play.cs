using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    public partial class form_play : g2play_listener
    {
        private G2GUID _service;
        private int _channel;
        private search_data _s_data;
        private g2play _adaptor;
        private g2user _user_adaptor;
        private int _total_camera_count;
        private int _panel_num;
        public play_info _play_info;
        public on_rec_failover_start _on_rec_failover_start;
        public check_exist_failover_service _check_exist_failover_service;

        public void init_connective_play()
        {
            _adaptor = new g2play();
            _adaptor.set_listener(this);
            _adaptor.startup(2);
        }

        public bool connect_play()
        {
            bool connect_res = false;

            G2CONNECT_RES res;
            lock (_adaptor)
            {
                _channel = _adaptor.connect(ref _service, out res);
            }

            if (_channel >= 0) 
            {
                connect_res = true;
                _screen.buf_setup(_channel, _total_camera_count, frame_buf.TYPE.PLAY);
            }

            return connect_res;
        }

        public void on_post_receive_record_channels(int channel, G2PLAY_CHANNEL_INFO[] channels)
        {
            g2channel_set channelset = new g2channel_set();
            for (int j = 0; j < channels.Length; ++j)
            {
                if (_play_info.camera_list.Contains(channels[j]._guid))
                {
                    int index = _play_info.index_from_camera(channels[j]._guid);
                    _play_info.channelexts[index] = channels[j]._channelext;
                    _play_info.panel_numbers[index] = _panel_num++;

                    channelset.insert(channels[j]._channelext);
                }
            }

            _s_data = new search_data(search_data.MODE.PLAY, channel);

            G2SPOT spot;
            int pane = _screen.get_last_disp_spot(out spot);
            int channelext = _play_info.channelext_from_panel(pane);
            G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(channelext, spot);
            bool preparing = false;
            _adaptor.set_camera_list_interest(channel, channelset);
            _adaptor.set_camera_list(channel, channelset, ref rbi, true, out preparing);

            _table = _table_minute;
            _screen.resume();
            _screen.resume_search_audio();
            _controller.set_channel(channel, _s_data);
            _controller.set_table(_table_minute);
            _controller.Visible = true;
            _table.Visible = true;

            foreach (time_table_base p in _table_set)
            {
                if (p != _table)
                {
                    p.Visible = false;
                }
            }
        }

        public void on_post_receive_record_time_info_load_end(int channel, G2RECORD_TIME_INFO.RESOLUTION res, G2RECORD_TIME_INFO.COMMAND command)
        {
            if (res == G2RECORD_TIME_INFO.RESOLUTION.DAY)
            {
                if (_s_data._spot_disp.valid == false)
                {
                    _adaptor.request_move_to_last(channel);
                    _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.MOVE_TO_LAST);
                }

                _controller.set_record_date(_s_data);
            }
            else if (res == G2RECORD_TIME_INFO.RESOLUTION.MINUTE)
            {
                if (command == G2RECORD_TIME_INFO.COMMAND.INIT)
                {
                    load_time_table(channel);
                }
            }
        }

        #region g2play_listener 멤버

        public void on_g2play_connected(int handle, int channel)
        {
            _adaptor.request_record_channels(channel);
            _screen.message().hide(STRING.NIS_CONNECTING);
        }

        public void on_g2play_disconnected(int handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            _channel = -1;
            if (reason != G2DISCONNECT_REASON.TYPE.LOGOUT)
            {
                string error = g2foundation.get_string_disconnect_reason(reason);
                _screen.message().disp(error, 60 * 1000, false);
            }
            else
            {
                _screen.message().hide(STRING.NIS_CONNECTING);
                _screen.message().hide(STRING.NIS_LOADING_RECORD_TIME);
            }
        }

        public void on_g2play_probe_session_profile(int handle, int channel, ref G2PROBE_SESSION_PROFILE probe)
        {
            Debug.WriteLine("callback [on_g2play_probe_session_profile] is not implemented.");
        }

        public void on_g2play_query_options_search_base(int handle, int channel, ref G2PLAY_OPTIONS_SEARCH_BASE options)
        {
            Debug.WriteLine("callback [on_g2play_query_options_search_base] is not implemented.");
        }

        public void on_g2play_receive_device_log_load(int handle, int channel, ref G2SYSTEM_LOG log)
        {
            Debug.WriteLine("callback [on_g2play_receive_device_log_load] is not implemented.");
        }

        public void on_g2play_receive_device_log_load_end(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_device_log_load_end] is not implemented.");
        }

        public void on_g2play_receive_device_log_load_fail(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_device_log_load_fail] is not implemented.");
        }

        public void on_g2play_receive_device_log_load_stop(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_device_log_load_stop] is not implemented.");
        }

        public void on_g2play_receive_record_failover_scope(int handle, int channel, G2FAILOVER_SCOPE_INFO[] infos)
        {
            this.BeginInvoke((MethodInvoker)delegate() { _controller.on_receive_record_failover_scope(infos); });
        }

        public void on_g2play_receive_event_log_load(int handle, int channel, ref G2EVENT_LOG log)
        {
            _controller.on_receive_event_log(channel, log);
        }

        public void on_g2play_receive_event_log_load_end(int handle, int channel)
        {
            _controller.on_receive_event_log_load_end(channel);
        }

        public void on_g2play_receive_event_log_load_fail(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_event_log_load_fail] is not implemented.");
        }

        public void on_g2play_receive_event_log_load_stop(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_event_log_load_stop] is not implemented.");
        }

        public void on_g2play_receive_frame_data(int handle, int channel, ref G2FRAME frame)
        {
            int pane = _play_info.panel_from_channelext(frame.channel);
            _screen.put_frame(ref frame, _channel, pane);
        }

        public void on_g2play_receive_hard_disk_info(int handle, int channel, G2PLAY_HARD_DISK_INFO[] dis)
        {
            Debug.WriteLine("callback [on_g2play_receive_hard_disk_info] is not implemented.");
        }

        public void on_g2play_receive_no_recorded_data(int handle, int channel)
        {
            _screen.message().disp(STRING.NIS_NO_RECORDED_IMAGE, STRING.get(STRING.NIS_NO_RECORDED_IMAGE), 5000, false);
        }

        public void on_g2play_receive_notify_command_begin(int handle, int channel, G2PLAYER.COMMAND_AND_SPEED command)
        {
            if (command != G2PLAYER.COMMAND_AND_SPEED.NEXT_STEP)
            {
                if (command != G2PLAYER.COMMAND_AND_SPEED.PLAY_SLOW &&
                    command != G2PLAYER.COMMAND_AND_SPEED.PLAY_NORMAL)
                {
                    frame_buf buf = _screen.buf_manager().get(channel);
                    if (buf != null)
                    {
                        buf.remove();
                    }
                }
            }
        }

        public void on_g2play_receive_notify_command_end(int handle, int channel, G2PLAYER.COMMAND_AND_SPEED command)
        {
            if (G2PLAYER.is_command_move(command) != true)
            {
                frame_buf buf = _screen.buf_manager().get(channel);
                if (buf != null)
                {
                    buf.remove();
                }
            }
        }

        public void on_g2play_receive_notify_frame_not_found(int handle, int channel, ref G2SPOT spot, G2PLAYER.PRECISION precision)
        {
            _controller.on_receive_frame_not_found(channel, spot, precision);
        }

        public void on_g2play_receive_notify_out_of_scope(int handle, int channel, G2PLAYER.OUT_OF_SCOPE status)
        {
            _screen.search_play_end(channel);
        }

        public void on_g2play_receive_notify_play_speed_changed(int handle, int channel, G2PLAYER.COMMAND_AND_SPEED speed)
        {
            _controller.on_receive_play_speed_changed(channel, speed);
        }

        public void on_g2play_receive_notify_player_error(int handle, int channel, G2PLAYER.PLAYER_ERROR error)
        {
            Debug.WriteLine("callback [on_g2play_receive_notify_player_error] is not implemented.");
        }

        public void on_g2play_receive_record_channels(int handle, int channel, G2PLAY_CHANNEL_INFO[] channels)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_record_channels(channel, channels); });
        }

        public void on_g2play_receive_record_time_info_load(int handle, int channel, ref G2RECORD_TIME_INFO rti)
        {
            if (rti._resolution == G2RECORD_TIME_INFO.RESOLUTION.DAY)
            {
                _s_data.set_date(ref rti);
            }
            else
            {
                G2TIME selected_date = new G2TIME(_controller.CAL_MONTH.SelectionStart);
                G2TIME receive_date = new G2TIME(rti._spot._time.year, rti._spot._time.month, rti._spot._time.day);
                if (selected_date == receive_date)
                {
                    _s_data.set_minute(ref rti);
                }
            }
        }

        public void on_g2play_receive_record_time_info_load_end(int handle, int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.COMMAND command)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_receive_record_time_info_load_end(channel, resolution, command); });
        }

        public void on_g2play_receive_scope_list(int handle, int channel, G2SCOPE[] scopes, int type)
        {
            _controller.on_receive_scope_list(channel, scopes, type);
        }

        public void on_g2play_receive_service_log_load(int handle, int channel, ref G2SYSTEM_LOG log)
        {
            Debug.WriteLine("callback [on_g2play_receive_service_log_load] is not implemented.");
        }

        public void on_g2play_receive_service_log_load_end(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_service_log_load_end] is not implemented.");
        }

        public void on_g2play_receive_service_log_load_fail(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_service_log_load_fail] is not implemented.");
        }

        public void on_g2play_receive_service_log_load_stop(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_service_log_load_stop] is not implemented.");
        }

        public void on_g2play_receive_spot_list(int handle, int channel, G2SPOT[] spots)
        {
            _controller.on_receive_spot_list(channel, spots);
        }

        public void on_g2play_receive_text_in(int handle, int channel, ref G2TEXT_IN textIn)
        {
            Debug.WriteLine("callback [on_g2play_receive_text_in] is not implemented.");
        }

        public void on_g2play_receive_text_in_log_load(int handle, int channel, ref G2EVENT log)
        {
            _controller.on_receive_text_in_log(channel, log);
        }

        public void on_g2play_receive_text_in_log_load_end(int handle, int channel)
        {
            _controller.on_receive_text_in_log_end(channel);
        }

        public void on_g2play_receive_text_in_log_load_fail(int handle, int channel)
        {
            _controller.on_receive_text_in_log_fail(channel);
        }

        public void on_g2play_receive_text_in_log_load_stop(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_text_in_log_load_stop] is not implemented.");
        }

        public void on_g2play_require_prepare_rollback(int handle, int channel, bool prepare)
        {
            Debug.WriteLine("callback [on_g2play_require_prepare_rollback] is not implemented.");
        }

        public void on_g2play_receive_debug_log_load(int handle, int channel, ref G2DEBUG_LOG log)
        {
            Debug.WriteLine("callback [on_g2play_receive_debug_log_load] is not implemented.");
        }

        public void on_g2play_receive_debug_log_load_end(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_debug_log_load_end] is not implemented.");
        }

        public void on_g2play_receive_debug_log_load_fail(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_debug_log_load_fail] is not implemented.");
        }

        public void on_g2play_receive_debug_log_load_stop(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_receive_debug_log_load_stop] is not implemented.");
        }

        #endregion
    }
}
