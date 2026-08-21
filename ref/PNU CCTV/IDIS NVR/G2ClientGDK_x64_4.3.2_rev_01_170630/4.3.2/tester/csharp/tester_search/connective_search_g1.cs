using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    using G2HSEARCH = System.Int32;

    public partial class form_search : g2search_listener
    {
        public void init_connective_search_g1()
        {
            _adaptor_g1 = new adaptor_g1();
            _adaptor_g1.set_listener(this);
            _adaptor_g1.startup(2);
        }

        public void on_g2search_connected(G2HSEARCH handle, int channel)
        {
            lock (_adaptor_g1)
            {
                if (_channel != channel)
                {
                    _adaptor_g1.disconnect(channel);
                    return;
                }
            }

            G2_PRODUCT_INFO pi;
            if (_adaptor_g1.get_product_info(channel, out pi))
            {
                _count_camera = pi.device.count_camera;
            }

            int[] panes = new int[_count_camera];
            for (int i = 0; i < _count_camera; ++i)
            {
                panes[i] = i;
            }

            if (_adaptor_g1.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.HOUR))
            {
                _table = _table_hour;
            }
            else if (_adaptor_g1.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.IDR_MINUTE))
            {
                _table = _table_IDR;
            }
            else
            {
                _table = _table_minute;
            }

            frame_buf.TYPE buf_type = (pi.remote_search.version == (uint)G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.VERSION.G2) ? frame_buf.TYPE.PLAY :
                                      (pi.remote_search.version == (uint)G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.VERSION.SAMBA_LIKE) ? frame_buf.TYPE.SEARCH_SAMBA :
                                      (pi.remote_search.version == (uint)G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.VERSION.SAMBA) ? frame_buf.TYPE.SEARCH_SAMBA :
                                      (pi.remote_search.rec_info_type == (uint)G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.IDR_MINUTE) ? frame_buf.TYPE.SEARCH_IDR : frame_buf.TYPE.SEARCH;

            _data = new search_data(search_data.MODE.LOCAL_G1, channel);
            _screen.reset(false);
            _screen.buf_setup(_channel, 64, buf_type);
            _screen.set_pane_select(0, false);
            _screen.set_format(_count_camera, true);
            _screen.set_pane_mode(panes, camera_pane.MODE.PLAY);
            _screen.set_pane_status(panes, camera_pane.STATUS.ENABLE, true);
            _screen.resume();
            _screen.resume_search_audio();
            _controller_g1.set_channel(_channel, _data);
            _controller_g1.set_table(_table);
            _controller = _controller_g1;

            set_camera_list(channel);

            if (_finalize != true)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_search_g1_connected(channel); });
            }
        }
        public void on_g2search_disconnected(G2HSEARCH handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            lock (_adaptor_g1)
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
            _screen.search_audio().close();
            _count_camera = 0;
            _channel = -1;
            _table.reset();
            _data = null;

            if (_finalize != true)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_search_g1_disconnected(channel, reason); });
            }
        }
        public void on_g2search_receive_recorded_date(G2HSEARCH handle, int channel, G2TIME[] dates)
        {
            _data.set_record_date(dates);

            this.BeginInvoke((MethodInvoker)delegate() { on_post_search_g1_receive_recorded_date(channel); });
        }
        public void on_g2search_receive_recorded_time_hour(G2HSEARCH handle, int channel, bool[][] hour, uint segcount)
        {
            if (_data._request_record_time)
            {
                _data.set_record_hour(hour);

                this.BeginInvoke((MethodInvoker)delegate() { on_post_search_g1_receive_recorded_hour(channel); });
            }
        }
        public void on_g2search_receive_recorded_time_minute(G2HSEARCH handle, int channel, List<byte[]> minute)
        {
            if (_data._request_record_time)
            {
                _data.set_record_minute_IDR(minute);

                this.BeginInvoke((MethodInvoker)delegate() { on_post_search_g1_receive_recorded_minute_IDR(channel); });
            }
        }
        public void on_g2search_receive_recorded_rechour_minute(G2HSEARCH handle, int channel, G2RECORD_TIME_INFO[] rti)
        {
            if (_data._request_record_time)
            {
                _data.set_record_minute(rti);

                this.BeginInvoke((MethodInvoker)delegate() { on_post_search_g1_receive_recorded_rechour_minute(channel); });
            }
        }
        public void on_g2search_receive_frame_data(G2HSEARCH handle, int channel, ref G2FRAME frame)
        {
            int pane = frame.channel;
            _screen.put_frame(ref frame, channel, pane);
        }
        public void on_g2search_receive_no_frame(G2HSEARCH handle, int channel)
        {
            _controller_g1.on_receive_no_frame(channel);
            _screen.search_play_end(channel);
        }
        public void on_g2search_receive_no_recorded_data(G2HSEARCH handle, int channel)
        {
            _screen.message().disp(STRING.NIS_NO_RECORDED_IMAGE, "there is no recorded image.", 5000, false);
        }
        public void on_g2search_receive_no_recorded_data_from_search_target(G2HSEARCH handle, int channel, G2SEARCH_TARGET.TYPE target) { }
        public void on_g2search_receive_find_idr_event_time(G2HSEARCH handle, int channel, G2TIME time) { }
        public void on_g2search_receive_notify_play_speed_changed(G2HSEARCH handle, int channel, int speed)
        {
            if (speed == (int)G2SEARCH_PLAYBACK.COMMAND.STOP)
            {
                frame_buf buf = _screen.buf_manager().get(channel);
                if (buf != null)
                {
                    buf.remove();
                }
            }

            _controller_g1.on_receive_play_speed_changed(channel, speed);
        }
        public void on_g2search_receive_notify_play_stop_post(G2HSEARCH handle, int channel, G2SEARCH_DRIVE.MODE mode)
        {
            _controller_g1.on_receive_play_stop_post(channel, mode);
        }
        public void on_g2search_receive_notify_end_of_play(G2HSEARCH handle, int channel)
        {
            _screen.search_play_end(channel);
        }
        public void on_g2search_receive_notify_segment_changed(G2HSEARCH handle, int channel, int segment) { }
        public void on_g2search_receive_notify_search_mode_changed(G2HSEARCH handle, int channel, G2SEARCH_MODE.TYPE mode) { }
        public void on_g2search_receive_notify_command_end(G2HSEARCH handle, int channel, int command) { }
        public void on_g2search_receive_query_result_event(G2HSEARCH handle, int channel, G2SEARCH_LOG_INFO[] info)
        {
            _controller_g1.on_receive_event_log(channel, info);
        }
        public void on_g2search_receive_query_result_text_in(G2HSEARCH handle, int channel, G2TEXT_IN[] data)
        {
            _controller_g1.on_receive_text_in_log(channel, data);
        }
        public void on_g2search_receive_recorded_date_scope(G2HSEARCH handle, int channel, G2SCOPE[] scopes)
        {
            if (_data._request_record_time)
            {
                if (scopes.Length != 0)
                {
                    G2SPOT spot = scopes[0]._begin;
                    G2SCOPE scope = new G2SCOPE(scopes[0]._begin, scopes[scopes.Length - 1]._end);
                    _data.set_record_scope(scope);
                    _adaptor_g1.request_record_hour(channel, spot, 512, false, false);
                }
            }
            else
            {
                _controller_g1.on_receive_scope_list(channel, scopes);
            }
        }
        public void on_g2search_receive_segment_spot(G2HSEARCH handle, int channel, G2SPOT[] spots)
        {
            _controller_g1.on_receive_spot_list(channel, spots);
        }
        public void on_g2search_receive_error(G2HSEARCH handle, int channel) { }
        public void on_g2search_receive_text_in(G2HSEARCH handle, int channel, ref G2TEXT_IN_ELEMENT data) { }
        public void on_g2search_receive_external_tango_info(G2HSEARCH handle, int channel, G2SEARCH_EXTERNAL_DISK[] info) { }
        public void on_g2search_receive_gps_data_start(G2HSEARCH handle, int channel) { }
        public void on_g2search_receive_gps_data(G2HSEARCH handle, int channel, ref G2TEXT_IN data) { }
        public void on_g2search_receive_gps_data_list(G2HSEARCH handle, int channel, G2TEXT_IN[] data) { }
        public void on_g2search_receive_gps_data_end(G2HSEARCH handle, int channel) { }
        public void on_g2search_receive_gps_data_end_count(G2HSEARCH handle, int channel, int total) { }
        public void on_g2search_receive_gps_data_export_cancel_result(G2HSEARCH handle, int channel, int result) { }
        public void on_g2search_receive_gps_data_measure_result(G2HSEARCH handle, int channel, int count, sbyte done) { }
        public void on_g2search_require_prepare_playback(G2HSEARCH handle, int channel, int command, G2SPOT spot) { }
        public bool on_g2search_require_prepare_load_event_image(G2HSEARCH handle, int channel, int selected, bool last) { return true; }
        public bool on_g2search_require_prepare_reload(G2HSEARCH handle, int channel) { return true; }
        public void on_g2search_probe_session_profile(G2HSEARCH handle, int channel, ref G2PROBE_SESSION_PROFILE probe)
        {
            if (_probe_perf)
            {
                string p = probe.ToString();
                this.BeginInvoke((MethodInvoker)delegate() { STC_PROBE_PERF.Text = p; });
            }
        }

        private void on_post_search_g1_connected(int channel)
        {
            BTN_DISCONNECT.Enabled = true;
            STC_PROBE_PERF.Text = "";

            _screen.message().hide(STRING.NIS_CONNECTING);
            _controller_g1.Visible = true;
            _controller_g2.Visible = false;
            _table.Visible = true;

            foreach (time_table_base p in _table_set)
            {
                if (p != _table)
                {
                    p.Visible = false;
                }
            }
        }
        private void on_post_search_g1_disconnected(int channel, G2DISCONNECT_REASON.TYPE reason)
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
            _controller_g1.reset();
            _controller_g1.set_enable(false);

            STC_PROBE_PERF.Text = "";
            BTN_CONNECT.Enabled = true;
            BTN_DISCONNECT.Enabled = false;
        }
        private void on_post_search_g1_receive_recorded_date(int channel)
        {
            if (_data._spot_standard.valid != true)
            {
                if (_adaptor_g1.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.HOUR) ||
                    _adaptor_g1.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.IDR_MINUTE))
                {
                    G2TIME time;
                    if (_data.get_record_date_last(out time))
                    {
                        _data._request_record_time = true;
                        _data._spot_standard = new G2SPOT(0, time);
                        _screen.message().disp(STRING.NIS_LOADING_RECORD_TIME, STRING.get(STRING.NIS_LOADING_RECORD_TIME), 0, true);
                        _controller.set_enable(false);
                        _controller.set_select_date(time);
                        _adaptor_g1.request_record_time(channel, time);
                    }
                    else
                    {
                        _adaptor_g1.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.LAST);
                    }
                }
                else
                {
                    _adaptor_g1.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.LAST);
                }
            }

            _controller_g1.set_record_date(_data);
        }
        private void on_post_search_g1_receive_recorded_hour(int channel)
        {
            imp_load_time_table_g1(channel);

            _controller_g1.request_goto_last();
        }
        private void on_post_search_g1_receive_recorded_minute_IDR(int channel)
        {
            imp_load_time_table_g1(channel);

            _controller_g1.request_goto_last();
        }
        private void on_post_search_g1_receive_recorded_rechour_minute(int channel)
        {
            imp_load_time_table_g1(channel);
        }

        public void imp_set_camera_list_g1(int channel, g2channel_set channelset_interest, g2channel_set channelset)
        {
            uint cameras = channelset.to_uint32();
            uint camras_pre = 0;
            if (_adaptor_g1.get_camera_list(channel, out camras_pre))
            {
                if (cameras == camras_pre)
                {
                    return; // same camera list
                }
            }

            G2SPOT spot;
            int pane = _screen.get_last_disp_spot(out spot);

            _adaptor_g1.set_camera_list(channel, cameras);
            _adaptor_g1.request_reload_current(channel, pane, spot);

            frame_buf buf = _screen.buf_manager().get(channel);
            if (buf != null)
            {
                buf.remove();
                buf.set_channelset(channelset, channelset);
            }
        }
        public void imp_load_time_table_g1(int channel)
        {
            if (_adaptor_g1.is_connected(channel) != true)
            {
                return;
            }

            G2SPOT spot;
            _table.set_data(_data, _screen.selected_pane());
            _table.set_spot_standard(_data._spot_standard);
            _table.update();
            _data._request_record_time = false;
            _screen.message().hide(STRING.NIS_LOADING_RECORD_TIME);
            _controller_g1.set_record_time(_data);
            _controller_g1.set_enable(true);
            _screen.get_last_disp_spot(out spot);

            if (_adaptor_g1.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.MINUTE))
            {
                if (spot._time.valid)
                {
                    _controller_g1.request_record_time(spot);
                }
            }
        }

        private adaptor_g1 _adaptor_g1;
    }
}
