using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class controller_search_g2 : Form, controller
    {
        public enum MODE
        {
            TIMELAPSE = 0,
            EVENT
        }

        public controller_search_g2(Control parent, Rectangle rect)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Visible = true;
            this.Location = rect.Location;
            this.Size = rect.Size;
            this.Parent = parent;
            this.STC_SPEED.Text = "";
            this.CHK_USE_SEGMENT.Checked = true;
            this.CHK_GOTO_ADJACENT_FRAME.Checked = false;
            this.BTN_EVENT_SEARCH_MORE.Enabled =
            this.BTN_TEXT_IN_SEARCH_MORE.Enabled = false;

            this._channel = -1;
            this._step_interval = 0;
            this._support_text_in_search = true;
            this._support_export_clip = true;
            this._command = new G2PLAYBACK_COMMAND();
            this._timer_stopped = new Timer();
            this._timer_stopped.Tick += new EventHandler(on_timer);
            this._timer_event_image = new Timer();
            this._timer_event_image.Tick += new EventHandler(on_timer);
            this._timer_event_image.Interval = 200;
        }

        public void reset()
        {
            _channel = -1;
            _data = null;
            _command = new G2PLAYBACK_COMMAND();
            _reserved = null;
            _timer_stopped.Stop();
            _timer_event_image.Stop();
            _event_list.Items.Clear();

            set_controller_mode(MODE.TIMELAPSE);

            CAL_MONTH.RemoveAllBoldedDates();
            CAL_MONTH.UpdateBoldedDates();
            CAL_MONTH.MaxDate = new DateTime(9998, 12, 31);
            CAL_MONTH.MinDate = new DateTime(1753, 1, 1, 0, 0, 0);
            CAL_MONTH.SelectionStart = CAL_MONTH.TodayDate;
            BTN_EVENT_SEARCH_MORE.Enabled =
            BTN_TEXT_IN_SEARCH_MORE.Enabled = false;
            SLD_SPEED.Value = 0;
        }

        public void set_adaptor(g2search_g2 adaptor, screen_pane screen)
        {
            _adaptor = adaptor;
            _screen = screen;
        }
        public void set_event_list(ListView event_list)
        {
            _event_list = event_list;
        }
        public void set_table(time_table_minute table)
        {
            _table = table;
        }
        public void set_channel(int channel, search_data data)
        {
            G2_PRODUCT_INFO pi;
            _adaptor.get_product_info(channel, out pi);
            _channel = channel;
            _data = data;
            _support_text_in_search = (pi.text_in_search.version != (byte)G2_PRODUCT_INFO_CAPS.TEXT_IN_SEARCH.VERSION.NONE);
            _support_export_clip = (pi.remote_clip_copy.version == (uint)G2_PRODUCT_INFO_CAPS.REMOTE_CLIP_COPY.VERSION.CLIP_COPY);

            this.BeginInvoke((MethodInvoker)delegate() { imp_set_channel_update(); });
        }
        public void set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { set_enable(enable); });
                return;
            }

            bool stop = (_channel < 0) ||
                        (_adaptor == null || _adaptor.is_stopped(_channel));

            if (enable != true)
            {
                if (this.Enabled)
                {
                    foreach (Control c in this.Controls)
                    {
                        if (c.Focused)
                        {
                            _last_focused = c;
                            break;
                        }
                    }
                }
            }

            CHK_AUDIO_PLAY.Enabled = stop && _screen.is_format1x1();
            CHK_AUDIO_PLAY.Checked = _screen.is_audio_enable(_screen.selected_pane());
            this.Enabled = enable;
            this.CAL_MONTH.Update();
            BTN_PLAY.Visible = stop;
            BTN_STOP.Visible = stop != true;

            if (enable)
            {
                if (_last_focused != null)
                {
                    _last_focused.Focus();
                    _last_focused = null;
                }
            }
        }
        public void set_controller_mode(MODE mode)
        {
            if (_table == null) return;
            if (mode == MODE.TIMELAPSE)
            {
                BTN_MODE_EVENT.Visible = true;
                BTN_MODE_TIMELAPSE.Visible = false;

                _table.Visible = true;
                _event_list.Visible = false;
            }
            else
            {
                BTN_MODE_TIMELAPSE.Visible = true;
                BTN_MODE_EVENT.Visible = false;

                _event_list.Visible = true;
                _table.Visible = false;
            }
        }
        public void set_record_date(int channel, search_data data)
        {
            List<DateTime> dates = new List<DateTime>();
            if (data._date.get(-1, dates))
            {
                DateTime date_e = dates[dates.Count - 1];
                DateTime min = new DateTime(dates[0].Year, dates[0].Month, 1);
                DateTime max = new DateTime(date_e.Year, date_e.Month, DateTime.DaysInMonth(date_e.Year, date_e.Month), 23, 59, 59);
                CAL_MONTH.RemoveAllBoldedDates();
                CAL_MONTH.BoldedDates = dates.ToArray();
                CAL_MONTH.MinDate = min;
                CAL_MONTH.MaxDate = max;
            }
        }
        public void set_select_date(DateTime date)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { set_select_date(date); });
                return;
            }

            CAL_MONTH.SelectionStart = new DateTime(date.Year, date.Month, date.Day);
        }
        public void set_reserve(MethodInvoker method)
        {
            _reserved = method;
        }

        protected void imp_set_channel_update()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { imp_set_channel_update(); });
                return;
            }

            BTN_TEXT_IN_SEARCH.Enabled = _support_text_in_search;
            BTN_EXPORT_CLIP.Enabled = _support_export_clip;
        }
        protected void imp_request_record_time(G2SPOT spot)
        {
            G2TIME from = spot._time.date;
            G2TIME to = new G2TIME(from.year, from.month, from.day, 23, 59, 59);
            G2SCOPE scope;

            set_enable(false);
            set_select_date(from);

            _data._request_record_time = true;
            _data._spot_standard = spot;
            _data.clear_record_time_info();
            _table.set_enable(false);
            _table.set_spot_standard(spot);
            _screen.message().disp(STRING.NIS_LOADING_RECORD_TIME, STRING.get(STRING.NIS_LOADING_RECORD_TIME), 0, true);
            _adaptor.request_record_time_info_on_time(_channel, G2RECORD_TIME_INFO.RESOLUTION.MINUTE, G2RECORD_TIME_INFO.DIRECTION.FORWARD, from, to, 512, G2RECORD_TIME_INFO.COMMAND.INIT, out scope);
            _data.set_record_scope(scope);
        }
        protected void imp_stop(int channel)
        {
            if (_timer_stopped.Enabled) return; // stopping
            if (_adaptor.is_stopped(channel))
            {
                _screen.set_play_speed(channel, 0);

                if (_reserved != null)
                {
                    _timer_stopped.Interval = 5;
                    _timer_stopped.Start();
                }
                SLD_SPEED.Value = 0;
                return;
            }

            set_enable(false);

            _screen.search_stop_enter(channel);

            G2SPOT spot;
            int pane = _screen.get_last_disp_spot(out spot);
            G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(pane, spot);

            _adaptor.request_pause(channel, true, rbi);
            _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.NONE);
            _command.speed = G2PLAYER.COMMAND_AND_SPEED.NONE;
            _screen.set_play_speed(channel, 0);
            _timer_stopped.Interval = 5;
            _timer_stopped.Stop();
            _timer_stopped.Start();
        }
        protected void imp_select_spot(int channel, G2SPOT spot)
        {
            if (_adaptor.is_connected(channel))
            {
                _adaptor.request_move_to_spot(channel, spot, G2PLAYER.PRECISION.MINUTE, true);
                _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.MOVE_TO_SPOT);
            }
        }
        protected void imp_select_date(int channel, DateTime date)
        {
            if (_adaptor.is_connected(channel))
            {
                G2TIME from = date.Date;
                G2TIME to = from + new G2TIME_SPAN(0, 23, 59, 59);
                g2channel_set channelset;
                _adaptor.get_camera_list(channel, out channelset);
                _adaptor.request_scope_list(channel, from, to, channelset, (int)G2PLAY_SCOPE_TYPE.TYPE.GOTO);
            }
        }
        protected void imp_goto_first(int channel)
        {
            if (_adaptor.is_connected(channel))
            {
                _adaptor.request_move_to_first(channel);
                _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.MOVE_TO_FIRST);
            }
        }
        protected void imp_goto_last(int channel)
        {
            if (_adaptor.is_connected(channel))
            {
                _adaptor.request_move_to_last(channel);
                _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.MOVE_TO_LAST);
            }
        }
        protected bool imp_goto_time(int channel, DateTime time)
        {
            if (_adaptor.is_connected(channel))
            {
                g2channel_set channelset;
                if (_adaptor.get_camera_list(channel, out channelset))
                {
                    return _adaptor.request_spot_list(channel, time, channelset, CHK_GOTO_ADJACENT_FRAME.Checked);
                }
            }
            return false;
        }
        protected void imp_step_prev(int channel, int interval)
        {
            if (interval == 0)
            {
                _adaptor.request_prev_step(channel);
                _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.PREV_STEP);
            }
            else
            {
                int pos = _table.get_select_pos() - interval;
                G2SPOT spot = _table.spot_from_pos(pos);
                if (_table.pos_from_spot(spot) >= 0)
                {
                    spot._tick = G2SPOT.INVALID_SPOT_TICK;
                }
                else
                {
                    G2SPOT disp_spot = new G2SPOT();
                    _screen.get_last_disp_spot(out disp_spot);
                    spot._segment = disp_spot._segment;
                    spot._time = disp_spot._time - new G2TIME_SPAN(interval * 60);
                    spot._tick = G2SPOT.INVALID_SPOT_TICK;
                }

                _adaptor.request_move_to_spot(channel, spot, G2PLAYER.PRECISION.FRAME, false);
                _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.PREV_STEP);
            }
        }
        protected void imp_step_next(int channel, int interval)
        {
            if (interval == 0)
            {
                _adaptor.request_next_step(channel);
                _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.NEXT_STEP);
            }
            else
            {
                int pos = _table.get_select_pos() + interval;
                G2SPOT spot = _table.spot_from_pos(pos);
                if (_table.pos_from_spot(spot) < 0)
                {
                    G2SPOT disp_spot = new G2SPOT();
                    _screen.get_last_disp_spot(out disp_spot);
                    spot._segment = disp_spot._segment;
                    spot._time = disp_spot._time + new G2TIME_SPAN(interval * 60);
                    spot._tick = 0;
                }

                _adaptor.request_move_to_spot(channel, spot, G2PLAYER.PRECISION.FRAME, true);
                _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.NEXT_STEP);
            }
        }
        protected void imp_search_event(int channel)
        {
            G2SEARCH_G2_EVENT_SEARCH_OPTIONS options;
            G2_PRODUCT_INFO pi;
            g2channel_set cameras;
            g2channel_set cameras_record;

            _adaptor.get_camera_list_interest(channel, out cameras_record);
            _adaptor.get_option_query_event(channel, out options);
            _adaptor.get_event_query_cameras(channel, out cameras);
            _adaptor.get_product_info(_channel, out pi);

            using (form_event_search form = new form_event_search(options, cameras))
            {
                G2SPOT spot;
                if (_screen.get_last_disp_spot(out spot) >= 0)
                {
                    if (spot._time.valid)
                    {
                        G2TIME begin = spot._time - new G2TIME_SPAN(0, 1, 0, 0);
                        if (begin.day != spot._time.day)
                        {
                            begin = spot._time;
                        }

                        form.set_time_range(begin, spot._time);
                    }
                }

                form.set_product_info(ref pi);
                form.load();
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                options = form._options;
            }

            options._record = cameras_record;
            options._unit_count = 100;

            _event_list.Items.Clear();
            _adaptor.set_event_query_cameras(channel, cameras);
            _adaptor.set_event_query_mode(channel, G2SEARCH_G2_QUERY.MODE.EVENT);
            if (_adaptor.request_event_log_search(channel, ref options))
            {
                _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);

                set_enable(false);
            }
        }
        protected void imp_search_event_more(int channel)
        {
            if (_adaptor.get_event_query_mode(channel) == G2SEARCH_G2_QUERY.MODE.EVENT)
            {
                _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);
                _adaptor.request_event_log_search_next(channel);

                set_enable(false);
            }
        }
        protected void imp_search_text_in(int channel)
        {
            G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS options;
            G2_PRODUCT_INFO pi;

            _adaptor.get_option_query_text_in(channel, out options);
            _adaptor.get_product_info(_channel, out pi);

            using (form_text_in_search form = new form_text_in_search(options, false))
            {
                G2SPOT spot;
                if (_screen.get_last_disp_spot(out spot) >= 0)
                {
                    if (spot._time.valid)
                    {
                        G2TIME begin = spot._time - new G2TIME_SPAN(0, 1, 0, 0);
                        if (begin.day != spot._time.day)
                        {
                            begin = spot._time;
                        }

                        form.set_time_range(begin, spot._time);
                    }
                }

                form.set_product_info(ref pi);
                form.load();
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                options = form._options_G2;
            }

            options._unit_count = 100;

            _event_list.Items.Clear();
            _adaptor.set_event_query_mode(channel, G2SEARCH_G2_QUERY.MODE.TEXT_IN);
            if (_adaptor.request_text_in_log_search(channel, ref options))
            {
                _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);

                set_enable(false);
            }
        }
        protected void imp_search_text_in_more(int channel)
        {
            if (_adaptor.get_event_query_mode(channel) == G2SEARCH_G2_QUERY.MODE.TEXT_IN)
            {
                _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);
                _adaptor.request_text_in_log_search_next(channel);

                set_enable(false);
            }
        }
        protected void imp_event_move_to(int channel, G2EVENT evt)
        {
            if (evt._spot.valid)
            {
                if (_adaptor.is_connected(channel))
                {
                    g2channel_set channelset = evt._associated_channels;
                    if (channelset.empty())
                    {
                        _adaptor.get_camera_list_interest(channel, out channelset);
                    }

                    int[] panes = channelset.to_array();

                    _screen.clear_last_image(panes, true);
                    _screen.set_format_range(panes, true);
                    _adaptor.request_move_to_spot(channel, evt._spot, G2PLAYER.PRECISION.EVENT, true);
                    _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.COMMAND_MOVE);
                }
            }
        }
        protected void imp_export_clip(int channel)
        {
            form_export_clip form = new form_export_clip(_adaptor as adaptor_playable, _channel);
            G2SPOT spot;
            G2_PRODUCT_INFO pi;
            _adaptor.get_product_info(_channel, out pi);
            _adaptor.set_invoke_saver(_channel, form);
            _screen.get_last_disp_spot(out spot);

            form.set_product_info(ref pi);
            form.set_time_range(spot._time, spot._time);
            form.set_cameras(_screen.get_pane_visible());
            form.load();
            form.ShowDialog();

            _adaptor.set_revoke_saver(_channel);
        }

        public void request_record_time(G2SPOT spot)
        {
            if (_data._request_record_time) return;
            if (_adaptor.get_play_control_command(_channel) == G2PLAYER.COMMAND_AND_SPEED.COMMAND_MOVE ||
               (_table.is_contains_date(spot) != true))
            {
                imp_request_record_time(spot);
            }
            return;
        }

        public void on_receive_play_speed_changed(int channel, G2PLAYER.COMMAND_AND_SPEED speed)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_play_speed_changed(channel, speed); });
                return;
            }

            set_enable(true);
        }
        public void on_receive_frame_not_found(int channel, G2SPOT spot, G2PLAYER.PRECISION precision)
        {
            if (precision == G2PLAYER.PRECISION.EVENT)
            {
                _screen.message().disp(STRING.NIS_NO_RECORDED_IMAGE, STRING.get(STRING.NIS_NO_RECORDED_IMAGE), 5000, false);
            }
        }
        public void on_receive_scope_list(int channel, G2SCOPE[] scopes, int type)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_scope_list(channel, scopes, type); });
                return;
            }

            if (scopes.Length == 0)
            {
                _screen.message().disp(STRING.NIS_NO_RECORDED_DATA, STRING.get(STRING.NIS_NO_RECORDED_DATA), 5000, false);
            }
            else
            {
                G2SPOT spot = G2SPOT.INVALID_SPOT;

                if (scopes.Length > 1 && CHK_USE_SEGMENT.Checked)
                {
                    form_select_segment form = new form_select_segment();
                    form.set_data(scopes);
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        G2SCOPE scope = form.scope_selected;
                        spot = scope._begin;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    spot = scopes[0]._begin;
                }

                if (spot.valid)
                {
                    if (_adaptor.is_connected(channel))
                    {
                        _adaptor.request_move_to_spot(channel, spot, G2PLAYER.PRECISION.FRAME, true);
                        _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.COMMAND_MOVE);
                    }
                }
            }
        }
        public void on_receive_spot_list(int channel, G2SPOT[] spots)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_spot_list(channel, spots); });
                return;
            }

            if (spots.Length == 0)
            {
                _screen.message().disp(STRING.NIS_NO_RECORDED_DATA, STRING.get(STRING.NIS_NO_RECORDED_DATA), 5000, false);
            }
            else
            {
                G2SPOT spot = G2SPOT.INVALID_SPOT;

                if (spots.Length > 1 && CHK_USE_SEGMENT.Checked)
                {
                    form_select_segment form = new form_select_segment();
                    form.set_data(spots);
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        G2SCOPE scope = form.scope_selected;
                        spot = scope._begin;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    spot = spots[0];
                }

                if (spot.valid)
                {
                    if (_adaptor.is_connected(channel))
                    {
                        _adaptor.request_move_to_spot(channel, spot, G2PLAYER.PRECISION.SECOND, true);
                        _adaptor.set_play_control_command(channel, G2PLAYER.COMMAND_AND_SPEED.COMMAND_MOVE);
                    }
                }
            }
        }
        public void on_receive_event_log(int channel, G2EVENT[] logs, bool canceled)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_event_log(channel, logs, canceled); });
                return;
            }

            if (canceled)
            {
                set_enable(true);
                BTN_TEXT_IN_SEARCH_MORE.Enabled = BTN_EVENT_SEARCH_MORE.Enabled = false;
                return;
            }
            if (logs.Length == 0)
            {
                _screen.message().disp(STRING.NIS_NO_RESULT, STRING.get(STRING.NIS_NO_RESULT), 5000, false);
                set_enable(true);
                BTN_TEXT_IN_SEARCH_MORE.Enabled = BTN_EVENT_SEARCH_MORE.Enabled = false;
                return;
            }

            _screen.message().hide(STRING.NIS_SEARCHING);

            set_controller_mode(MODE.EVENT);
            set_enable(true);

            _event_list.BeginUpdate();
            int select = _event_list.Items.Count;

            foreach (G2EVENT e in logs)
            {
                G2EVENT_INFO.TYPE_LEVEL2 lv2 = e.to_G2EVENT_INFO_LEVEL2();
                string source = (G2EVENT_INFO.is_event_camera(lv2)) ? string.Format("Camera {0}", e._channel + 1) :
                                (G2EVENT_INFO.is_event_alarm(lv2)) ? string.Format("Alarm-In {0}", e._channel + 1) :
                                (G2EVENT_INFO.is_event_alarm_network(lv2)) ? string.Format("Network Alarm-In {0}", e._channel + 1) :
                                (G2EVENT_INFO.is_event_audio(lv2)) ? string.Format("Audio-In {0}", e._channel + 1) :
                                (G2EVENT_INFO.is_event_text_in(lv2)) ? string.Format("Text-In {0}", e._channel + 1) : "";
                string type = e.string_event_type();
                string time = e._spot._time.to_string_date_time();
                string data = e._data;

                ListViewItem lvi = new ListViewItem(type);
                lvi.Tag = e;
                lvi.SubItems.Add(source.ToString());
                lvi.SubItems.Add(data);
                lvi.SubItems.Add(time);

                _event_list.Items.Add(lvi);
            }

            _event_list.EnsureVisible(select);
            _event_list.EndUpdate();
            _event_list.Items[select].Selected = true;

            BTN_EVENT_SEARCH_MORE.Enabled = (logs.Length >= 100);
            BTN_TEXT_IN_SEARCH_MORE.Enabled = false;
        }
        public void on_receive_text_in_log(int channel, G2EVENT[] logs, bool canceled)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_text_in_log(channel, logs, canceled); });
                return;
            }

            if (canceled)
            {
                set_enable(true);
                BTN_TEXT_IN_SEARCH_MORE.Enabled = BTN_EVENT_SEARCH_MORE.Enabled = false;
                return;
            }
            if (logs.Length == 0)
            {
                _screen.message().disp(STRING.NIS_NO_RESULT, STRING.get(STRING.NIS_NO_RESULT), 5000, false);
                set_enable(true);
                BTN_TEXT_IN_SEARCH_MORE.Enabled = BTN_EVENT_SEARCH_MORE.Enabled = false;
                return;
            }

            _screen.message().hide(STRING.NIS_SEARCHING);

            set_controller_mode(MODE.EVENT);
            set_enable(true);

            _event_list.BeginUpdate();
            int select = _event_list.Items.Count;

            foreach (G2EVENT e in logs)
            {
                G2EVENT_INFO.TYPE_LEVEL2 lv2 = e.to_G2EVENT_INFO_LEVEL2();
                string source = G2EVENT_INFO.is_event_text_in(lv2) ? string.Format("Text-In {0}", e._channel + 1) : "";
                string type = e.string_event_type();
                string time = e._spot._time.to_string_date_time();
                string data = e._data;

                ListViewItem lvi = new ListViewItem(type);
                lvi.Tag = e;
                lvi.SubItems.Add(source.ToString());
                lvi.SubItems.Add(data);
                lvi.SubItems.Add(time);

                _event_list.Items.Add(lvi);
            }

            _event_list.EnsureVisible(select);
            _event_list.EndUpdate();
            _event_list.Items[select].Selected = true;

            BTN_TEXT_IN_SEARCH_MORE.Enabled = (logs.Length >= 100);
            BTN_EVENT_SEARCH_MORE.Enabled = false;
        }

        public void on_screen_changed_pane(int pane)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_screen_changed_pane(pane); });
                return;
            }

            CHK_AUDIO_PLAY.Checked = _screen.is_audio_enable(pane);
        }
        public void on_screen_changed_format(screen_format.FORMAT format, screen_format.CHANGED mode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_screen_changed_format(format, mode); });
                return;
            }

            int channel = _channel;
            CHK_AUDIO_PLAY.Enabled = (format == screen_format.FORMAT.LAYOUT1X1) &&
                                     (_adaptor.is_connected(channel) && _adaptor.is_stopped(channel));
        }
        public void on_screen_image_disp(ref G2FRAME frame)
        {

        }
        public void on_screen_play_end_loaded(int channel)
        {
            _adaptor.request_notify_end_of_play(channel);

            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    imp_stop(channel);
                });
            }
            else
            {
                imp_stop(channel);
            }
        }

        public void on_time_table_move_to_spot(G2SPOT spot)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel))
            {
                imp_select_spot(channel, spot);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_select_spot(channel, spot); });
                imp_stop(channel);
            }
        }

        public void on_event_list_selected(G2EVENT evt)
        {
            _timer_event_image.Stop();
            _timer_event_image.Start();
        }

        private void on_timer(object sender, EventArgs e)
        {
            int channel = _channel;

            if (sender == _timer_stopped)
            {
                if (_adaptor.is_stopped(channel))
                {
                    _timer_stopped.Stop();
                    _screen.search_stop_leave(channel);

                    SLD_SPEED.Value = 0;
                    set_enable(true);

                    if (_reserved != null)
                    {
                        this.Invoke(_reserved);
                        _reserved = null;
                    }
                }
            }
            else if (sender == _timer_event_image)
            {
                _timer_event_image.Stop();

                if (_adaptor.is_connected(channel))
                {
                    if (_event_list.SelectedItems.Count != 0)
                    {
                        G2EVENT selected = (G2EVENT)_event_list.SelectedItems[0].Tag;
                        imp_event_move_to(channel, selected);
                    }
                }
            }
        }
        private void on_btn_step_prev(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel))
            {
                imp_step_prev(channel, _step_interval);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_step_prev(channel, _step_interval); });
                imp_stop(channel);
            }
        }

        private void on_btn_step_next(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel))
            {
                imp_step_next(channel, _step_interval);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_step_next(channel, _step_interval); });
                imp_stop(channel);
            }
        }
        private void on_btn_step_move(object sender, EventArgs e)
        {
            ContextMenu pop = new ContextMenu();
            EventHandler handler = new EventHandler(on_menu_step_move_interval);
            pop.MenuItems.Add("1F", handler).Tag = 0;
            pop.MenuItems.Add("-");
            pop.MenuItems.Add("1M", handler).Tag = 1;
            pop.MenuItems.Add("5M", handler).Tag = 5;
            pop.MenuItems.Add("10M", handler).Tag = 10;
            pop.MenuItems.Add("15M", handler).Tag = 15;
            pop.MenuItems.Add("30M", handler).Tag = 30;
            pop.MenuItems.Add("-");
            pop.MenuItems.Add("1H", handler).Tag = 60;

            foreach (MenuItem mi in pop.MenuItems)
            {
                if (mi.Tag != null)
                {
                    if ((int)mi.Tag == _step_interval)
                    {
                        mi.Checked = true;
                        break;
                    }
                }
            }

            pop.Show(this, new Point(BTN_STEP_INTERVAL.Location.X, BTN_STEP_INTERVAL.Bottom));
        }
        private void on_btn_play(object sender, EventArgs e)
        {
            if (_adaptor.is_stopped(_channel) != true)
            {
                return;
            }

            set_enable(true);

            G2SPOT spot;
            int pane = _screen.get_last_disp_spot(out spot);
            G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(pane, spot);

            _command._rbi = rbi;
            _command.speed = G2PLAYER.COMMAND_AND_SPEED.PLAY_NORMAL;
            _adaptor.request_play(_channel, _command);
            _adaptor.set_play_control_command(_channel, _command.speed);
            _screen.set_play_speed(_channel, 10);

            SLD_SPEED.Value = 10;
        }
        private void on_btn_stop(object sender, EventArgs e)
        {
            imp_stop(_channel);
        }
        private void on_btn_goto(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel))
            {
                imp_goto_time(channel, DTP_GOTO.Value);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_goto_time(channel, DTP_GOTO.Value); });
                imp_stop(channel);
            }
        }
        private void on_btn_goto_first(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel))
            {
                imp_goto_first(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_goto_first(channel); });
                imp_stop(channel);
            }
        }
        private void on_btn_goto_last(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel))
            {
                imp_goto_last(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_goto_last(channel); });
                imp_stop(channel);
            }
        }
        private void on_chk_play_audio(object sender, EventArgs e)
        {
            if (_screen.have_CONFIG_SOUND != true)
            {
                _screen.message().disp(STRING.NIS_NO_ASSEMBLY_DSOUND, STRING.get(STRING.NIS_NO_ASSEMBLY_DSOUND), 5000, false);
            }
            _screen.set_pane_audio_enable(_screen.selected_pane(), CHK_AUDIO_PLAY.Checked);
        }
        private void on_speed_value_changed(object sender, EventArgs e)
        {
            int val = SLD_SPEED.Value;
            int speed = val;

            G2PLAYER.COMMAND_AND_SPEED speed_svr = G2PLAYER.COMMAND_AND_SPEED.NONE;

            if (val == 0 ||
               (val < 4 && val > -4))
            {
                speed = 0;
            }
            else if (val > 0)
            {
                if (val >= 40)
                {
                    if (val > 70)
                    {
                        speed_svr = G2PLAYER.COMMAND_AND_SPEED.PLAY_FASTEST;
                    }
                    else if (val > 60)
                    {
                        speed_svr = G2PLAYER.COMMAND_AND_SPEED.PLAY_FASTER;
                    }
                    else
                    {
                        speed_svr = G2PLAYER.COMMAND_AND_SPEED.PLAY_FAST;
                    }

                    speed = 40 + (speed - 40) * 3;
                }
                else
                {
                    speed_svr = G2PLAYER.COMMAND_AND_SPEED.PLAY_NORMAL;
                }
            }
            else if (val < 0)
            {
                if (val <= -40)
                {
                    if (val < -70)
                    {
                        speed_svr = G2PLAYER.COMMAND_AND_SPEED.BACK_FASTEST;
                    }
                    else if (val < -60)
                    {
                        speed_svr = G2PLAYER.COMMAND_AND_SPEED.BACK_FASTER;
                    }
                    else
                    {
                        speed_svr = G2PLAYER.COMMAND_AND_SPEED.BACK_FAST;
                    }

                    speed = -40 + (speed + 40) * 3;
                }
                else
                {
                    speed_svr = G2PLAYER.COMMAND_AND_SPEED.BACK_NORMAL;
                }
            }

            _screen.set_play_speed(_channel, speed);

            if (speed == 0)
            {
                STC_SPEED.Text = "";
            }
            else
            {
                float speedf = (float)Math.Abs(speed) / 10.0F;
                STC_SPEED.Text = speedf.ToString("x0.0" + (Math.Abs(val) >= 40 ? " key-frame" : ""));
            }

            if (_command.speed == speed_svr)
            {
                return;
            }

            // rely on on_get_rollback_info() callback method
            // to use the rollback spot of actual last image
            // see on_g2search_g2_receive_notify_get_rollback_info handler

            G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(-1);
            _command._rbi = rbi;
            _command.speed = speed_svr;

            if (speed_svr == G2PLAYER.COMMAND_AND_SPEED.NONE)
            {
                if (_adaptor.is_stopped(_channel))
                {
                    _adaptor.request_pause(_channel, true, rbi);    // for occur rollback
                }
                else
                {
                    imp_stop(_channel);
                    SLD_SPEED.Value = 0;
                }
            }
            else
            {
                _adaptor.request_play(_channel, _command);
                _adaptor.set_play_control_command(_channel, speed_svr);
            }
        }
        private void on_speed_mouse_up(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TrackBar c = sender as TrackBar;
                c.Value = 0;
            }
        }
        private void on_cal_date_selected(object sender, DateRangeEventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel))
            {
                imp_select_date(channel, e.Start);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_select_date(channel, e.Start); });
                imp_stop(channel);
            }
        }
        private void on_dtp_goto_mouse_wheel(object sender, MouseEventArgs e)
        {
            SendKeys.Send(e.Delta > 0 ? "{UP}" : "{DOWN}");
        }
        private void on_menu_step_move_interval(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi.Tag != null)
            {
                _step_interval = (int)mi.Tag;
            }
            BTN_STEP_INTERVAL.Text = mi.Text;
        }
        private void on_btn_mode_timelapse(object sender, EventArgs e)
        {
            set_controller_mode(MODE.TIMELAPSE);
        }
        private void on_btn_mode_event(object sender, EventArgs e)
        {
            if (_event_list.Items.Count == 0)
            {
                if (_adaptor.is_event_query_mode(_channel, G2SEARCH_G2_QUERY.MODE.TEXT_IN))
                {
                    on_btn_event_search(BTN_TEXT_IN_SEARCH, new EventArgs());
                }
                else
                {
                    on_btn_event_search(BTN_EVENT_SEARCH, new EventArgs());
                }
            }

            set_controller_mode(MODE.EVENT);
        }
        private void on_btn_event_search(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel) &&
                _timer_stopped.Enabled != true)
            {
                imp_search_event(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_search_event(channel); });
                imp_stop(channel);
            }
        }
        private void on_btn_event_search_more(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel) &&
                _timer_stopped.Enabled != true)
            {
                imp_search_event_more(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_search_event_more(channel); });
                imp_stop(channel);
            }
        }
        private void on_btn_text_in_search(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel) &&
                _timer_stopped.Enabled != true)
            {
                imp_search_text_in(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_search_text_in(channel); });
                imp_stop(channel);
            }
        }
        private void on_btn_text_in_search_more(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel) &&
                _timer_stopped.Enabled != true)
            {
                imp_search_text_in_more(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_search_text_in_more(channel); });
                imp_stop(channel);
            }
        }
        private void on_btn_export_clip(object sener, EventArgs e)
        {
            if (_adaptor.is_authority(_channel, G2RAS_AUTHORITY.TYPE.AUTHORITY_CLIP_COPY) != true)
            {
                MessageBox.Show("You don't have clip-export authority.", "Authority", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int channel = _channel;
            if (_adaptor.is_stopped(channel) &&
                _timer_stopped.Enabled != true)
            {
                imp_export_clip(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_export_clip(channel); });
                imp_stop(channel);
            }
        }

        private g2search_g2 _adaptor;
        private screen_pane _screen;
        private search_data _data;
        private time_table_minute _table;
        private ListView _event_list;
        private int _channel;
        private int _step_interval;
        private bool _support_text_in_search;
        private bool _support_export_clip;
        private G2PLAYBACK_COMMAND _command;
        private MethodInvoker _reserved;
        private Timer _timer_stopped;
        private Timer _timer_event_image;
        private Control _last_focused;
    }
}
