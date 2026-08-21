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
    public partial class form_controller_play : Form
    {
        public enum MODE
        {
            TIMELAPSE = 0,
            EVENT
        }

        public form_controller_play(Control parent, Rectangle rect, form_play.play_info play_info, G2GUID service)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Visible = true;
            this.Location = rect.Location;
            this.Size = rect.Size;
            this.Parent = parent;
            this.STC_SPEED.Text = "";
            this.CHK_USE_SEGMENT.Checked = true;
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
            _by_date_select = false;
            _event_logs = new List<G2EVENT_LOG>();
            _play_info = play_info;
            _textin_logs = new List<G2EVENT>();
            _service = service;
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
            BTN_TEXT_IN_SEARCH_MORE.Enabled = false;
            SLD_SPEED.Value = 0;
        }

        public void set_adaptor(g2play adaptor, screen_pane screen, g2user user)
        {
            _adaptor = adaptor;
            _screen = screen;
            _user_adaptor = user;
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
            _channel = channel;
            _data = data;

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
        public void set_record_date(search_data data)
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

        public void imp_set_channel_update()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { imp_set_channel_update(); });
                return;
            }

            BTN_TEXT_IN_SEARCH.Enabled = _support_text_in_search;
            if (_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_SAVE_CLIPCOPY))
            {
                BTN_EXPORT_CLIP.Enabled = _support_export_clip;
            }
            else
            {
                BTN_EXPORT_CLIP.Enabled = false;
            }
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
            int channelext = _play_info.channelext_from_panel(pane);
            G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(channelext, spot);

            _adaptor.request_pause(channel, true, ref rbi);
            _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.NONE);
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
                _adaptor.request_move_to_spot(channel, ref spot, (int)G2PLAYER.PRECISION.MINUTE, true);
                _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.MOVE_TO_SPOT);
            }
        }
        protected void imp_select_date(int channel, DateTime date)
        {
            if (_adaptor.is_connected(channel))
            {
                _by_date_select = true;
                G2TIME from = date.Date;
                G2TIME to = from + new G2TIME_SPAN(0, 23, 59, 59);
                g2channel_set channelset;
                _adaptor.get_camera_list(channel, out channelset);
                _adaptor.request_scope_list(channel, ref from, ref to, channelset, (int)G2PLAY_SCOPE_TYPE.TYPE.GOTO);
            }
        }
        protected void imp_goto_first(int channel)
        {
            if (_adaptor.is_connected(channel))
            {
                _adaptor.request_move_to_first(channel);
                _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.MOVE_TO_FIRST);
            }
        }
        protected void imp_goto_last(int channel)
        {
            if (_adaptor.is_connected(channel))
            {
                _adaptor.request_move_to_last(channel);
                _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.MOVE_TO_LAST);
            }
        }
        protected bool imp_goto_time(int channel, DateTime time)
        {
            if (_adaptor.is_connected(channel))
            {
                g2channel_set channelset;
                if (_adaptor.get_camera_list(channel, out channelset))
                {
                    G2TIME time2 = new G2TIME(time);
                    return _adaptor.request_spot_list(channel, ref time2, channelset);
                }
            }
            return false;
        }
        protected void imp_step_prev(int channel, int interval)
        {
            if (interval == 0)
            {
                _adaptor.request_prev_step(channel);
                _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.PREV_STEP);
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

                _adaptor.request_move_to_spot(channel, ref spot, (int)G2PLAYER.PRECISION.FRAME, false);
                _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.PREV_STEP);
            }
        }
        protected void imp_step_next(int channel, int interval)
        {
            if (interval == 0)
            {
                _adaptor.request_next_step(channel);
                _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.NEXT_STEP);
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

                _adaptor.request_move_to_spot(channel, ref spot, (int)G2PLAYER.PRECISION.FRAME, true);
                _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.NEXT_STEP);
            }
        }
        protected void imp_search_event(int channel)
        {
            G2SERVICE_SEARCH_OPTION_EVENT_LOG options;

            using (form_event_search form = new form_event_search(_play_info.camera_infos))
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

                form.load();
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                options = form._option;
            }

            _event_list.Items.Clear();

            if (_adaptor.request_event_log_search(channel, options))
            {
                _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);

                set_enable(false);
            }

            _options = options;
        }
        protected void imp_search_event_more(int channel)
        {
            _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);
            _adaptor.request_event_log_search(channel, _options);
            set_enable(false);
        }
        protected void imp_search_text_in(int channel)
        {
            G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG option = new G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG();

            using (form_text_in_search form = new form_text_in_search(option._options))
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

                form.set_device(_play_info.textin_infos);
                form.load();
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                option._source = form._source;
                option._options = form._options;
            }

            option._options._unit_count = 100;

            _event_list.Items.Clear();
            if (_adaptor.request_text_in_log_search(channel, option))
            {
                _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);

                set_enable(false);
            }
        }
        protected void imp_search_text_in_more(int channel)
        {
            _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);
            _adaptor.request_text_in_log_search_next(channel);

            set_enable(false);
        }
        protected void imp_event_move_to(int channel, object evt)
        {
            if (evt is G2EVENT)
            {
                G2EVENT textin = (G2EVENT)evt;
                if (textin._spot.valid)
                {
                    if (_adaptor.is_connected(channel))
                    {
                        //g2channel_set channelset = evt._associated_channels;
                        //if (channelset.empty())
                        //{
                        //    _adaptor.get_camera_list_interest(channel, out channelset);
                        //}

                        //int[] panes = channelset.to_array();

                        //_screen.clear_last_image(panes, true);
                        //_screen.set_format_range(panes, true);
                        _adaptor.request_move_to_spot(channel, ref textin._spot, (int)G2PLAYER.PRECISION.EVENT, true);
                        _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.COMMAND_MOVE);
                    }
                }
            }
            else
            {
                G2EVENT_LOG event_log = (G2EVENT_LOG)evt;
                if (event_log._spot_server.valid)
                {
                    if (_adaptor.is_connected(channel))
                    {
                        //g2channel_set channelset = evt._associated_channels;
                        //if (channelset.empty())
                        //{
                        //    _adaptor.get_camera_list_interest(channel, out channelset);
                        //}

                        //int[] panes = channelset.to_array();

                        //_screen.clear_last_image(panes, true);
                        //_screen.set_format_range(panes, true);
                        _adaptor.request_move_to_spot(channel, ref event_log._spot_server, (int)G2PLAYER.PRECISION.EVENT, true);
                        _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.COMMAND_MOVE);
                    }
                }
            }
        }
        protected void imp_export_clip(int channel)
        {
            form_export_clip_play form = new form_export_clip_play();
            G2SPOT spot;
            _screen.get_last_disp_spot(out spot);
            form.set_invoke_saver(_service);
            form.set_time_range(spot._time, spot._time);
            form.set_device(_play_info.root_infos, _play_info.camera_infos, _play_info.channelexts);
            form.ShowDialog();
            form.set_revoke_saver();
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

                if (scopes.Length > 1 && CHK_USE_SEGMENT.Checked && _by_date_select)
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

                G2SPOT disp_spot = _data._spot_disp;
                if (_table.is_contains_date(disp_spot) == false)
                {
                    set_select_date(disp_spot._time.date);
                    _screen.message().disp(STRING.NIS_LOADING_RECORD_TIME, STRING.get(STRING.NIS_LOADING_RECORD_TIME), 0, true);
                    set_enable(false);
                    _data._request_record_time = true;
                    _data._spot_standard = disp_spot;
                    _data.clear_record_time_info();
                    _table.set_enable(false);
                    _table.set_spot_standard(disp_spot);

                    G2SCOPE day_scope = new G2SCOPE(scopes[0]._begin, scopes[scopes.Length - 1]._end);
                    _adaptor.request_record_time_info(channel, G2RECORD_TIME_INFO.RESOLUTION.MINUTE, G2RECORD_TIME_INFO.DIRECTION.FORWARD, ref day_scope, 24 * scopes.Length, G2RECORD_TIME_INFO.COMMAND.INIT);

                    _data.set_record_scope(day_scope);
                }

                if (spot.valid && _by_date_select)
                {
                    _by_date_select = false;
                    if (_adaptor.is_connected(channel))
                    {
                        _adaptor.request_move_to_spot(channel, ref spot, (int)G2PLAYER.PRECISION.FRAME, true);
                        _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.COMMAND_MOVE);
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
                        _adaptor.request_move_to_spot(channel, ref spot, (int)G2PLAYER.PRECISION.SECOND, true);
                        _adaptor.set_play_control_command(channel, (int)G2PLAYER.COMMAND_AND_SPEED.COMMAND_MOVE);
                    }
                }
            }
        }

        public void on_receive_event_log(int channel, G2EVENT_LOG log)
        {
            _event_logs.Add(log);
        }

        public void on_receive_event_log_load_end(int channel)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_event_log_load_end(channel); });
                return;
            }

            if (_event_logs.Count == 0)
            {
                _screen.message().disp(STRING.NIS_NO_RESULT, STRING.get(STRING.NIS_NO_RESULT), 5000, false);
                set_enable(true);
                BTN_TEXT_IN_SEARCH_MORE.Enabled = false;
                return;
            }

            _event_list.Columns[2].Text = "Device";

            if (_event_logs.Count == 100)
            {
                G2SPOT pre_last_spot = _event_logs[99]._spot_server;
                G2TIME next_time = pre_last_spot._time - new G2TIME_SPAN(1);
                G2SPOT next_spot = new G2SPOT(pre_last_spot._segment, next_time);
                _options._scope = new G2SCOPE(_options._scope._begin, next_spot);
                
                BTN_EVENT_SEARCH_MORE.Enabled = true;
            }
            else
            {
                BTN_EVENT_SEARCH_MORE.Enabled = false;
            }

            _screen.message().hide(STRING.NIS_SEARCHING);
            set_controller_mode(MODE.EVENT);
            set_enable(true);
            _event_list.BeginUpdate();

            int select_index = _event_list.Items.Count;
            foreach (G2EVENT_LOG event_log in _event_logs)
            {
                string event_type = G2EVENT_INFO.string_event_type(event_log._level1, event_log._level2);
                string triggered = _play_info.camera_name(event_log._source);
                string device = _play_info.root_name(event_log._source);
                string time = event_log._spot_server._time.to_string_date_time();

                ListViewItem lvi = new ListViewItem(event_type);
                lvi.Tag = event_log;
                lvi.SubItems.Add(triggered);
                lvi.SubItems.Add(device);
                lvi.SubItems.Add(time);

                _event_list.Items.Add(lvi);
            }
            _event_list.EnsureVisible(select_index);
            _event_list.EndUpdate();
            _event_list.Items[select_index].Selected = true;

            _event_logs.Clear();
        }


        public void on_receive_text_in_log(int channel, G2EVENT log)
        {
            _textin_logs.Add(log);
        }

        public void on_receive_text_in_log_end(int channel)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_text_in_log_end(channel); });
                return;
            }

            if (_textin_logs.Count == 0)
            {
                _screen.message().disp(STRING.NIS_NO_RESULT, STRING.get(STRING.NIS_NO_RESULT), 5000, false);
                set_enable(true);
                BTN_TEXT_IN_SEARCH_MORE.Enabled = false;
                return;
            }

            _event_list.Columns[2].Text = "Transaction";

            if (_textin_logs.Count == 100)
            {
                BTN_TEXT_IN_SEARCH_MORE.Enabled = true;
            }
            else
            {
                BTN_TEXT_IN_SEARCH_MORE.Enabled = false;
            }

            _screen.message().hide(STRING.NIS_SEARCHING);

            set_controller_mode(MODE.EVENT);
            set_enable(true);

            _event_list.BeginUpdate();
            int select = _event_list.Items.Count;

            foreach (G2EVENT e in _textin_logs)
            {
                G2EVENT_INFO.TYPE_LEVEL2 lv2 = e.to_G2EVENT_INFO_LEVEL2();
                int index = _play_info.index_from_channelext((int)e._channel);
                string source = index < 0 ? "" : _play_info.textin_infos[index]._name._string;
                string type = e.string_event_type();
                string time = e._spot._time.to_string_date_time();
                string data = System.Text.Encoding.UTF8.GetString(e._info._text_in._data.to());

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

            _textin_logs.Clear();
        }

        public void on_receive_text_in_log_fail(int channel)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_text_in_log_fail(channel); });
                return;
            }

            _screen.message().disp(STRING.NIS_SEARCHING_FAIL, STRING.get(STRING.NIS_NO_RESULT), 5000, false);
            set_enable(true);
            BTN_TEXT_IN_SEARCH_MORE.Enabled = false;
            return;
        }

        public void on_receive_record_failover_scope(G2FAILOVER_SCOPE_INFO[] infos)
        {
            if (infos.Length < 1)
            {
                _screen.message().disp(STRING.NIS_NO_RESULT, STRING.get(STRING.NIS_NO_RESULT), 5000, false);
                return;
            }

            List<string[]> rows = new List<string[]>(infos.Length);
            for (int i = 0; i< infos.Length; ++i)
            {
                string[] row = new string[2];
                row[0] = infos[i]._scope_id.ToString();
                row[1] = infos[i]._scope._begin._time.to_string_date_time() + " ~ " +
                    infos[i]._scope._end._time.to_string_date_time();
                rows.Add(row);
            }

            bool exist = _check_exist_failover_service();

            form_rec_failover form = new form_rec_failover(rows, exist);
            if (form.ShowDialog() == DialogResult.OK)
            {
                G2GUID failover_service = infos[0]._failover_service_key;
                _on_rec_failover_start(failover_service, _play_info);
            }
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

        public void on_event_list_selected()
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
                        object selected = _event_list.SelectedItems[0].Tag;
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
            int channelext = _play_info.channelext_from_panel(pane);
            G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(channelext, spot);

            _command._rbi = rbi;
            _command.speed = G2PLAYER.COMMAND_AND_SPEED.PLAY_NORMAL;
            _adaptor.request_play(_channel, ref _command);
            _adaptor.set_play_control_command(_channel, (int)_command.speed);
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

            G2SPOT spot;
            _screen.get_last_disp_spot(out spot);
            int pane = _screen.selected_pane();
            int channelext = _play_info.channelext_from_panel(pane);

            G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(channelext, spot);
            _command._rbi = rbi;
            _command.speed = speed_svr;

            if (speed_svr == G2PLAYER.COMMAND_AND_SPEED.NONE)
            {
                if (_adaptor.is_stopped(_channel))
                {
                    _adaptor.request_pause(_channel, true, ref rbi);    // for occur rollback
                }
                else
                {
                    imp_stop(_channel);
                    SLD_SPEED.Value = 0;
                }
            }
            else
            {
                _adaptor.request_play(_channel, ref _command);
                _adaptor.set_play_control_command(_channel, (int)speed_svr);
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

        private void on_btn_rec_failover(object sender, EventArgs e)
        {
            imp_stop(_channel);
            //G2SPOT a = new G2SPOT(824, new G2TIME(2016, 05, 25, 17, 47, 0));
            //G2SPOT b = new G2SPOT(824, new G2TIME(2016, 05, 25, 17, 48, 0));
            //G2SCOPE invalid = new G2SCOPE(a, b);
            G2SCOPE invalid = new G2SCOPE();
            _adaptor.request_record_failover_scope_info(_channel, ref invalid, 1);
        }

        public void failover_btn_invisible()
        {
            BTN_REC_FAILOVER.Visible = false;
        }

        public void set_func(form_play.on_rec_failover_start func1, form_play.check_exist_failover_service func2)
        {
            _on_rec_failover_start = func1;
            _check_exist_failover_service = func2;
        }

        private g2play _adaptor;
        private G2GUID _service;
        private g2user _user_adaptor;
        private form_play.play_info _play_info;
        private form_play.on_rec_failover_start _on_rec_failover_start;
        private form_play.check_exist_failover_service _check_exist_failover_service;
        private screen_pane _screen;
        private search_data _data;
        private time_table_minute _table;
        private ListView _event_list;
        private int _step_interval;
        private int _channel;
        private bool _support_text_in_search;
        private bool _support_export_clip;
        private G2PLAYBACK_COMMAND _command;
        private MethodInvoker _reserved;
        private Timer _timer_stopped;
        private Timer _timer_event_image;
        private Control _last_focused;
        private bool _by_date_select;
        private G2SERVICE_SEARCH_OPTION_EVENT_LOG _options;
        private List<G2EVENT_LOG> _event_logs;
        private List<G2EVENT> _textin_logs;
    }


    public class STRING
    {
        public const int NIS_CONNECTING = 1;
        public const int NIS_LOADING_RECORD_TIME = 2;
        public const int NIS_NO_RECORDED_IMAGE = 3;
        public const int NIS_NO_RECORDED_DATA = 4;
        public const int NIS_SEARCHING = 5;
        public const int NIS_NO_RESULT = 6;
        public const int NIS_NO_ASSEMBLY_DSOUND = 7;
        public const int NIS_SEARCHING_FAIL = 8;
        public const int NIS_ALREADY_SHOW_LOG = 9;
        public const int NIS_NOT_READY_BACKUP_SITE = 10;

        public static string get(int id)
        {
            if (id == NIS_CONNECTING) return "connecting...";
            if (id == NIS_LOADING_RECORD_TIME) return "loading record time information...";
            if (id == NIS_NO_RECORDED_IMAGE) return "there is no recorded image.";
            if (id == NIS_NO_RECORDED_DATA) return "there is no recorded data.";
            if (id == NIS_SEARCHING) return "searching...";
            if (id == NIS_NO_RESULT) return "no result";
            if (id == NIS_NO_ASSEMBLY_DSOUND) return "cannot load assembly \"Microsoft.DirectX.DirectSound\"";
            if (id == NIS_SEARCHING_FAIL) return "searching fail";
            if (id == NIS_ALREADY_SHOW_LOG) return "this log window has already seen";
            if (id == NIS_NOT_READY_BACKUP_SITE) return "not ready backup site";
            return "";
        }
    }
}
