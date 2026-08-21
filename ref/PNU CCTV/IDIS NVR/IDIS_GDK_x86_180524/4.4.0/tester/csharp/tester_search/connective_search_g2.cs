using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    using G2HSEARCH_G2 = System.Int32;

    public partial class form_search : g2search_g2_listener
    {
        public void init_connective_search_g2()
        {
            _adaptor_g2 = new adaptor_g2();
            _adaptor_g2.set_listener(this);
            _adaptor_g2.startup(2);
            _adaptor = _adaptor_g2; // default adaptor is G2SEARCH_G2
        }

        public void on_g2search_g2_connected(G2HSEARCH_G2 handle, int channel)
        {
            lock (_adaptor_g2)
            {
                if (_channel != channel)
                {
                    _adaptor_g2.disconnect(channel);
                    return;
                }
            }

            G2_PRODUCT_INFO pi;
            if (_adaptor_g2.get_product_info(channel, out pi))
            {
                if (pi.remote_search.base_channel > 0 &&
                    pi.remote_search.stream_count > 0)
                {
                    _count_camera = pi.remote_search.base_channel * pi.remote_search.stream_count;
                }
                else
                {
                    _count_camera = pi.device.count_camera;
                }
            }

            int[] panes = new int[_count_camera];
            for (int i = 0; i < _count_camera; ++i)
            {
                panes[i] = i;
            }

            _table = _table_minute;
            _data = new search_data(search_data.MODE.LOCAL_G2, channel);
            _screen.reset(false);
            _screen.buf_setup(_channel, 64, frame_buf.TYPE.PLAY);
            _screen.set_pane_select(0, false);
            _screen.set_format(_count_camera, true);
            _screen.set_pane_mode(panes, camera_pane.MODE.PLAY);
            _screen.set_pane_status(panes, camera_pane.STATUS.ENABLE, true);
            _screen.resume();
            _screen.resume_search_audio();
            _controller_g2.set_channel(channel, _data);
            _controller_g2.set_table(_table_minute);
            _controller = _controller_g2;

            if (_adaptor_g2.is_support(channel, G2SEARCH_SUPPORT.QUERY.VIRTUAL_CHANNEL))
            {
                _adaptor_g2.request_virtual_channelmap(channel);
            }
            else
            {
                set_camera_list(channel);
            }

            if (_finalize != true)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_search_g2_connected(channel); });
            }
        }
        public void on_g2search_g2_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            lock (_adaptor_g2)
            {
                if (_channel != channel)
                {
                    return;
                }
            }

            _screen.buf_reset(channel);
            _screen.set_pane_select(0);
            _screen.set_format(16, false);
            _screen.reset(true);
            _screen.suspend();
            _screen.suspend_search_audio();
            _screen.decoder().close();
            //_screen.search_audio().close();
            _count_camera = 0;
            _channel = -1;
            _table.reset();
            _data = null;

            if (_finalize != true)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_search_g2_disconnected(channel, reason); });
            }
        }
        public void on_g2search_g2_query_options_search_base(G2HSEARCH_G2 handle, int channel, ref G2SEARCH_G2_OPTIONS_SEARCH_BASE options) { }
        public void on_g2search_g2_query_options_player(G2HSEARCH_G2 handle, int channel, ref G2SEARCH_G2_OPTIONS_PLAYER options) { }
        public void on_g2search_g2_receive_record_time_info_load(G2HSEARCH_G2 handle, int channel, ref G2RECORD_TIME_INFO rti)
        {
            if (rti._resolution == G2RECORD_TIME_INFO.RESOLUTION.DAY)
            {
                _data.set_date(ref rti);
            }
            else
            {
                _data.set_minute(ref rti);
            }
        }
        public void on_g2search_g2_receive_record_time_info_load_end(G2HSEARCH_G2 handle, int channel, G2RECORD_TIME_INFO.RESOLUTION resolution, G2RECORD_TIME_INFO.COMMAND command)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_search_g2_receive_record_time_info_load_end(channel, resolution, command); });
        }
        public void on_g2search_g2_receive_frame_data(G2HSEARCH_G2 handle, int channel, ref G2FRAME frame)
        {
            int pane = frame.channel;
            _screen.put_frame(ref frame, channel, pane);
        }
        public void on_g2search_g2_receive_text_in(G2HSEARCH_G2 handle, int channel, ref G2EVENT ei) { }
        public void on_g2search_g2_receive_event(G2HSEARCH_G2 handle, int channel, ref G2EVENT ei) { }
        public void on_g2search_g2_receive_notify_command_begin(G2HSEARCH_G2 handle, int channel, G2PLAYER.COMMAND_AND_SPEED command)
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
        public void on_g2search_g2_receive_notify_command_end(G2HSEARCH_G2 handle, int channel, G2PLAYER.COMMAND_AND_SPEED command)
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
        public void on_g2search_g2_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, int channel, G2PLAYER.COMMAND_AND_SPEED speed)
        {
            _controller_g2.on_receive_play_speed_changed(channel, speed);
        }
        public void on_g2search_g2_receive_notify_frame_not_found(G2HSEARCH_G2 handle, int channel, G2SPOT spot, G2PLAYER.PRECISION precision)
        {
            _controller_g2.on_receive_frame_not_found(channel, spot, precision);
        }
        public void on_g2search_g2_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER.OUT_OF_SCOPE status)
        {
            _screen.search_play_end(channel);
        }
        public void on_g2search_g2_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, ref G2ROLLBACK_INFO rbi)
        {
            G2SPOT spot;
            int pane = _screen.get_last_disp_spot(out spot);
            rbi._channelext = pane;
            rbi._spot = spot;
            rbi.precision = G2PLAYER.PRECISION.TICK_SECOND;

            if (rbi._spot.valid != true)
            {
                if (_adaptor_g2.is_stopped(channel) != true)
                {
                    g2channel_set channelset;
                    if (_adaptor_g2.get_camera_list(channel, out channelset))
                    {
                        if (channelset.empty() != true)
                        {
                            if (channelset.contains(_data._spot_disp_channelext))
                            {
                                rbi._channelext = _data._spot_disp_channelext;
                                rbi._spot = _data._spot_disp;
                                rbi.precision = G2PLAYER.PRECISION.TICK_SECOND;
                            }
                            else
                            {
                                int channelext = channelset[0];
                                if (channelext >= 0)
                                {
                                    rbi._channelext = channelext;
                                    rbi._spot = _data._spot_disp;
                                    rbi.precision = G2PLAYER.PRECISION.SECOND;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void on_g2search_g2_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER.PLAYER_ERROR error) { }
        public void on_g2search_g2_receive_event_log_load_end(G2HSEARCH_G2 handle, int channel, G2EVENT[] list)
        {
            _controller_g2.on_receive_event_log(channel, list, false);
        }
        public void on_g2search_g2_receive_event_log_load_stop(G2HSEARCH_G2 handle, int channel, G2EVENT[] list)
        {
            _controller_g2.on_receive_event_log(channel, list, true);
        }
        public void on_g2search_g2_receive_text_in_log_load_end(G2HSEARCH_G2 handle, int channel, G2EVENT[] list)
        {
            _controller_g2.on_receive_text_in_log(channel, list, false);
        }
        public void on_g2search_g2_receive_text_in_log_load_stop(G2HSEARCH_G2 handle, int channel, G2EVENT[] list)
        {
            _controller_g2.on_receive_text_in_log(channel, list, true);
        }
        public void on_g2search_g2_receive_scope_list(G2HSEARCH_G2 handle, int channel, G2SCOPE[] scopes, int type)
        {
            _controller_g2.on_receive_scope_list(channel, scopes, type);
        }
        public void on_g2search_g2_receive_spot_list(G2HSEARCH_G2 handle, int channel, G2SPOT[] spots)
        {
            _controller_g2.on_receive_spot_list(channel, spots);
        }
        public void on_g2search_g2_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel)
        {
            _screen.message().disp(STRING.NIS_NO_RECORDED_IMAGE, "there is no recorded image.", 5000, false);
        }
        public void on_g2search_g2_receive_db_info(G2HSEARCH_G2 handle, int channel, ref G2SEARCH_G2_REMOTE_DB di) { }
        public void on_g2search_g2_receive_db_info_external(G2HSEARCH_G2 handle, int channel, G2SEARCH_EXTERNAL_DISK[] dis) { }
        public void on_g2search_g2_receive_db_selected(G2HSEARCH_G2 handle, int channel, uint id, G2SEARCH_G2_REMOTE_DB.DB_SELECT_RESULT result) { }
        public void on_g2search_g2_receive_virtual_channelmap(G2HSEARCH_G2 handle, int channel)
        {
            set_camera_list(channel);
        }
        public void on_g2search_g2_require_prepare_rollback(G2HSEARCH_G2 handle, int channel, bool prepare) { }
        public void on_g2search_g2_probe_session_profile(G2HSEARCH_G2 handle, int channel, ref G2PROBE_SESSION_PROFILE probe)
        {
            if (_probe_perf)
            {
                string p = probe.ToString();
                this.BeginInvoke((MethodInvoker)delegate() { STC_PROBE_PERF.Text = p; });
            }
        }

        private void on_post_search_g2_connected(int channel)
        {
            BTN_DISCONNECT.Enabled = true;
            STC_PROBE_PERF.Text = "";

            _screen.message().hide(STRING.NIS_CONNECTING);
            _controller_g2.Visible = true;
            _controller_g1.Visible = false;
            _table.Visible = true;

            foreach (time_table_base p in _table_set)
            {
                if (p != _table)
                {
                    p.Visible = false;
                }
            }
        }
        private void on_post_search_g2_disconnected(int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            if (reason != G2DISCONNECT_REASON.TYPE.LOGOUT)
            {
                string error = g2foundation.get_string_disconnect_reason(reason);
                _screen.message().disp(error, 5 * 1000, false);
            }
            else
            {
                _screen.message().hide(STRING.NIS_CONNECTING);
                _screen.message().hide(STRING.NIS_LOADING_RECORD_TIME);
            }

            _table.update();
            _controller_g2.reset();
            _controller_g2.set_enable(false);

            STC_PROBE_PERF.Text = "";
            BTN_CONNECT.Enabled = true;
            BTN_DISCONNECT.Enabled = false;
        }
        private void on_post_search_g2_receive_record_time_info_load_end(int channel, G2RECORD_TIME_INFO.RESOLUTION res, G2RECORD_TIME_INFO.COMMAND command)
        {
            if (res == G2RECORD_TIME_INFO.RESOLUTION.DAY)
            {
                if (_data._spot_disp.valid != true)
                {
                    _adaptor_g2.request_move_to_last(channel);
                    _adaptor_g2.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.MOVE_TO_LAST);
                }

                _controller_g2.set_record_date(channel, _data);
            }
            else if (res == G2RECORD_TIME_INFO.RESOLUTION.MINUTE)
            {
                if (command == G2RECORD_TIME_INFO.COMMAND.INIT)
                {
                    imp_load_time_table_g2(channel);
                }
            }
        }

        public void imp_set_camera_list_g2(int channel, g2channel_set channelset_interest, g2channel_set channelset)
        {
            if (channelset_interest.empty()) return;

            G2SPOT spot;
            int pane = _screen.get_last_disp_spot(out spot);
            bool preparing = false;

            frame_buf buf = _screen.buf_manager().get(channel);
            if (buf != null)
            {
                buf.remove();
                buf.set_channelset(channelset_interest, channelset);
            }

            G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(pane, spot);
            _adaptor_g2.set_camera_list_interest(channel, channelset_interest);
            _adaptor_g2.set_camera_list(channel, channelset, rbi, true, out preparing);
        }
        public void imp_load_time_table_g2(int channel)
        {
            if (_adaptor_g2.is_connected(channel) != true)
            {
                return;
            }

            G2SPOT spot;
            _table.set_data(_data, _screen.selected_pane());
            _table.set_spot_standard(_data._spot_standard);
            _table.update();
            _data._request_record_time = false;
            _screen.message().hide(STRING.NIS_LOADING_RECORD_TIME);
            _controller_g2.set_enable(true);
            _screen.get_last_disp_spot(out spot);

            if (spot.valid)
            {
                if (_table.is_contains_date(_data._spot_disp) != true)
                {
                    _controller_g2.request_record_time(_data._spot_disp);
                }
            }
        }

        private adaptor_g2 _adaptor_g2;
    }
}
