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
    public partial class controller_search_g1 : Form, controller
    {
        public enum MODE
        {
            TIMELAPSE = 0,
            EVENT
        }

        public controller_search_g1(Control parent, Rectangle rect)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Visible = true;
            this.Location = rect.Location;
            this.Size = rect.Size;
            this.Parent = parent;
            this.CHK_USE_SEGMENT.Checked = true;
            this.SLD_FAST_SPEED.Location = this.SLD_PLAY_SPEED.Location;
            this.STC_FAST_SPEED.Location = this.STC_PLAY_SPEED.Location;
            this.BTN_EVENT_SEARCH_NEXT.Enabled =
            this.BTN_TEXT_IN_SEARCH_NEXT.Enabled = false;

            this._channel = -1;
            this._step_interval = 0;
            this._speed_play = 10;
            this._speed_fast = 40;
            this._support_segment_loader = true;
            this._support_segment_select = false;
            this._support_goto_date = true;
            this._support_step_interval = true;
            this._support_audio = true;
            this._support_text_in_search = true;
            this._support_export_clip = true;
            this._enable_audio_control = false;
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
            SLD_PLAY_SPEED.Value = 10;
            SLD_FAST_SPEED.Value = 40;
            SLD_PLAY_SPEED.Visible =
            STC_PLAY_SPEED.Visible = true;
            SLD_FAST_SPEED.Visible =
            STC_FAST_SPEED.Visible = false;
            BTN_EVENT_SEARCH_NEXT.Enabled =
            BTN_TEXT_IN_SEARCH_NEXT.Enabled = false;
        }

        public void set_adaptor(g2search adaptor, screen_pane screen)
        {
            _adaptor = adaptor;
            _screen = screen;
        }
        public void set_event_list(ListView event_list)
        {
            _event_list = event_list;
        }
        public void set_table(time_table_base table)
        {
            _table = table;
        }
        public void set_channel(int channel, search_data data)
        {
            G2_PRODUCT_INFO pi;
            _adaptor.get_product_info(channel, out pi);
            _channel = channel;
            _data = data;
            _support_segment_loader = (_adaptor.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.MINUTE));
            _support_segment_select = (_adaptor.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.HOUR));
            _support_goto_date = (_adaptor.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.MINUTE));
            _support_step_interval = (_adaptor.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.HOUR) != true);
            _support_audio = (_adaptor.is_support(channel, G2SEARCH_SUPPORT.QUERY.PLAY_AUDIO));
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

            if (_channel >= 0)
            {
                if (_adaptor.is_connected(_channel))
                {
                    _adaptor.set_change_search_mode(_channel, mode == MODE.EVENT);
                }
            }
        }
        public void set_record_date(search_data data)
        {
            List<DateTime> dates = new List<DateTime>();
            if (data.get_record_date(dates))
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
        public void set_record_time(search_data data)
        {
            int count = _support_segment_select ? data._record_hour.Count : 1;
            if (count != CBX_SEGMENT.Items.Count)
            {
                CBX_SEGMENT.Items.Clear();

                for (int i = 1; i < count + 1; ++i)
                {
                    CBX_SEGMENT.Items.Add("segment " + i.ToString());
                }
            }
            if (CBX_SEGMENT.Items.Count > 0)
            {
                CBX_SEGMENT.SelectedIndex = 0;
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
        public void request_record_time(G2SPOT spot)
        {
            if (_data._request_record_time) return;
            if (_table.is_contains_date(spot) != true)
            {
                imp_request_record_time(spot);
            }
        }
        public void request_goto_last()
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel))
            {
                imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.LAST);
                imp_goto_last(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_goto_last(channel); });
                imp_stop(channel);
            }
        }

        protected void imp_set_channel_update()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { imp_set_channel_update(); });
                return;
            }

            DTP_GOTO.CustomFormat = _support_goto_date ? " yyyy-MM-dd HH:mm:ss" : " HH:mm:ss";
            DTP_GOTO.ShowUpDown = _support_goto_date ? false : true;
            BTN_STEP_INTERVAL.Enabled = _support_step_interval;
            CHK_USE_SEGMENT.Enabled = _support_segment_loader;
            CHK_AUDIO_PLAY.Enabled = _support_audio && _enable_audio_control;
            BTN_EXPORT_CLIP.Enabled = _support_export_clip;

            if (_support_step_interval != true)
            {
                _step_interval = 0;
                BTN_STEP_INTERVAL.Text = "1F";
            }

            BTN_TEXT_IN_SEARCH.Enabled = _support_text_in_search;
        }
        protected void imp_request_record_time(G2SPOT spot)
        {
            G2TIME from = spot._time.date;

            set_enable(false);
            set_select_date(from);

            _data._request_record_time = true;
            _data._spot_standard = spot;
            _data.clear_record_time_info();
            _table.set_enable(false);
            _table.set_spot_standard(spot);
            _screen.message().disp(STRING.NIS_LOADING_RECORD_TIME, STRING.get(STRING.NIS_LOADING_RECORD_TIME), 0, true);
            _adaptor.request_record_time(_channel, from);
        }
        protected void imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND command)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { imp_set_play_controls(command); });
                return;
            }

            if (command == G2SEARCH_PLAYBACK.COMMAND.PLAY ||
                command == G2SEARCH_PLAYBACK.COMMAND.REW ||
                command == G2SEARCH_PLAYBACK.COMMAND.FF ||
                command == G2SEARCH_PLAYBACK.COMMAND.FIRST ||
                command == G2SEARCH_PLAYBACK.COMMAND.LAST ||
                command == G2SEARCH_PLAYBACK.COMMAND.PREV ||
                command == G2SEARCH_PLAYBACK.COMMAND.NEXT)
            {
                BTN_STOP.Enabled = true;
                BTN_STEP_PREV.Enabled = false;
                BTN_STEP_NEXT.Enabled = false;
                BTN_STEP_INTERVAL.Enabled = false;
                BTN_GOTO_FIRST.Enabled = false;
                BTN_GOTO_LAST.Enabled = false;
                BTN_GOTO.Enabled = false;
                DTP_GOTO.Enabled = false;
                CBX_SEGMENT.Enabled = false;
                CHK_AUDIO_PLAY.Enabled = false;
                _event_list.Enabled = false;
            }

            if (command == G2SEARCH_PLAYBACK.COMMAND.PLAY)
            {
                BTN_PLAY.Enabled = false;
                BTN_REW.Enabled = false;
                BTN_FF.Enabled = false;
            }
            else if (command == G2SEARCH_PLAYBACK.COMMAND.REW ||
                     command == G2SEARCH_PLAYBACK.COMMAND.FF)
            {
                BTN_PLAY.Enabled = false;
                BTN_REW.Enabled = false;
                BTN_FF.Enabled = false;
            }
            else if (command == G2SEARCH_PLAYBACK.COMMAND.FIRST ||
                     command == G2SEARCH_PLAYBACK.COMMAND.LAST ||
                     command == G2SEARCH_PLAYBACK.COMMAND.PREV ||
                     command == G2SEARCH_PLAYBACK.COMMAND.NEXT)
            {
                BTN_STOP.Enabled = false;
                BTN_PLAY.Enabled = false;
                BTN_REW.Enabled = false;
                BTN_FF.Enabled = false;
                CAL_MONTH.Enabled = false;
            }
            else
            {
                BTN_PLAY.Enabled = true;
                BTN_STOP.Enabled = false;
                BTN_STEP_PREV.Enabled = true;
                BTN_STEP_NEXT.Enabled = true;
                BTN_STEP_INTERVAL.Enabled = _support_step_interval;
                BTN_REW.Enabled = true;
                BTN_FF.Enabled = true;
                BTN_GOTO_FIRST.Enabled = true;
                BTN_GOTO_LAST.Enabled = true;
                BTN_GOTO.Enabled = true;
                DTP_GOTO.Enabled = true;
                CAL_MONTH.Enabled = true;
                CBX_SEGMENT.Enabled = _support_segment_select;
                CHK_AUDIO_PLAY.Enabled = _support_audio && _enable_audio_control;
                _event_list.Enabled = true;
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
                    _timer_stopped.Interval = 500;
                    _timer_stopped.Start();
                }
                return;
            }

            set_enable(false);

            _screen.search_stop_enter(channel);

            G2SPOT spot;
            int pane = _screen.get_last_disp_spot(out spot);
            G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(pane, spot);

            _adaptor.request_pause(channel, rbi);
            _screen.set_play_speed(channel, 0);
            _timer_stopped.Interval = 500;
            _timer_stopped.Stop();
            _timer_stopped.Start();
        }
        protected void imp_select_spot(int channel, G2SPOT spot)
        {
            if (_adaptor.is_connected(channel))
            {
                if (_adaptor.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.HOUR))
                {
                    G2TIME time = _table.get_spot_selected()._time;
                    _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.GOTO_HOUR, new G2SPOT(0, G2TIME.create(time.year, time.month, time.day, spot._time.hour, 0, 0)));
                }
                else
                {
                    _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.GOTO_SPOT, spot);
                }
            }
        }
        protected void imp_select_date(int channel, DateTime date)
        {
            if (_adaptor.is_connected(channel))
            {
                G2TIME from = date.Date;
                if (from.valid)
                {
                    if (_adaptor.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.HOUR) ||
                        _adaptor.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.IDR_MINUTE))
                    {
                        if (_data.contains_record_date(date))
                        {
                            set_enable(false);

                            _data._request_record_time = true;
                            _data._spot_standard = new G2SPOT(0, from);
                            _adaptor.request_record_time(channel, from);
                        }
                        else
                        {
                            _screen.message().disp(STRING.NIS_NO_RECORDED_IMAGE, STRING.get(STRING.NIS_NO_RECORDED_IMAGE), 5000, false);
                        }
                    }
                    else
                    {
                        _adaptor.request_record_time(channel, from);
                    }
                }
            }
        }
        protected void imp_goto_first(int channel)
        {
            if (_adaptor.is_connected(channel))
            {
                _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.FIRST);
            }
        }
        protected void imp_goto_last(int channel)
        {
            if (_adaptor.is_connected(channel))
            {
                _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.LAST);
            }
        }
        protected bool imp_goto_time(int channel, DateTime time)
        {
            if (_adaptor.is_connected(channel))
            {
                G2SPOT spot = G2SPOT.INVALID_SPOT;
                if (_support_goto_date)
                {
                    spot.set(0, time);
                }
                else
                {
                    G2TIME time_date = _data._spot_standard._time;
                    spot.set(0, G2TIME.create(time_date.year, time_date.month, time_date.day, time.Hour, time.Minute, time.Second));
                }

                if (_adaptor.is_rec_info_type(channel, G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.REC_INFO_TYPE.MINUTE))
                {
                    return _adaptor.request_record_hour_command(channel, G2SEARCH_PLAYBACK.COMMAND.GOTO_SEC, spot);
                }
                else
                {
                    return _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.GOTO_SEC, spot);
                }
            }
            return false;
        }
        protected void imp_step_prev(int channel, int interval)
        {
            if (interval == 0)
            {
                _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.PREV);
            }
            else
            {
                int pos = _table.get_select_pos() - interval;
                G2SPOT spot = _table.spot_from_pos(pos);
                if (_table.pos_from_spot(spot) >= 0)
                {
                    _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.GOTO_SPOT, spot);
                }
            }
        }
        protected void imp_step_next(int channel, int interval)
        {
            if (interval == 0)
            {
                _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.NEXT);
            }
            else
            {
                int pos = _table.get_select_pos() + interval;
                G2SPOT spot = _table.spot_from_pos(pos);
                if (_table.pos_from_spot(spot) >= 0)
                {
                    _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.GOTO_SPOT, spot);
                }
            }
        }
        protected void imp_search_event(int channel)
        {
            G2EVENT_QUERY_CONDITION options;
            G2_PRODUCT_INFO pi;
            g2channel_set cameras = new g2channel_set();
            uint cameras_query = 0;

            _adaptor.get_query_condition_event(channel, out options);
            _adaptor.get_query_cameras(channel, out cameras_query);
            _adaptor.get_product_info(_channel, out pi);

            cameras.from(cameras_query);

            if (_adaptor.is_drive_mode(channel, G2SEARCH_DRIVE.MODE.IDR))
            {
                using (form_event_search_IDR form = new form_event_search_IDR(options, cameras))
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
                    if (options._begin.valid) options._begin.set(spot._time.year, spot._time.month, spot._time.day);
                    if (options._end.valid) options._end.set(spot._time.year, spot._time.month, spot._time.day);
                }
            }
            else
            {
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

                    options = form._options_G1;
                }
            }

        //  options._record = ;
            options._reload = true;

            _event_list.Items.Clear();
            _adaptor.set_query_cameras(channel, cameras.to_uint32());
            _adaptor.set_query_mode(channel, G2SEARCH_QUERY.MODE.EVENT);
            _adaptor.set_change_search_mode(channel, true);

            if (_adaptor.request_query_event(channel, ref options))
            {
                _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);

                set_enable(false);
            }
        }
        protected void imp_search_event_next(int channel)
        {
            if (_adaptor.is_query_mode(channel, G2SEARCH_QUERY.MODE.EVENT) != true)
            {
                return;
            }

            G2SEARCH_LOG_INFO log;
            bool proceed = (_adaptor.is_drive_mode(channel, G2SEARCH_DRIVE.MODE.IDR)) ?
                            _adaptor.get_query_result_event(channel, 99, out log) :
                            _adaptor.get_query_result_event(channel, 0, out log);

            if (proceed != true)
            {
                return;
            }

            G2EVENT_QUERY_CONDITION options;
            _adaptor.get_query_condition_event(channel, out options);

            options._reload = false;

            if (_adaptor.is_drive_mode(channel, G2SEARCH_DRIVE.MODE.IDR) != true)
            {
                options._end = log._time;

                if (options._end.valid != true ||
                   (options._end.valid && options._begin.valid && options._end < options._begin))
                {
                    return;
                }
            }

            set_enable(false);

            _adaptor.request_query_event(channel, ref options);
        }
        protected void imp_search_text_in(int channel)
        {
            G2TEXT_IN_QUERY_CONDITION options;
            G2_PRODUCT_INFO pi;

            _adaptor.get_query_condition_text_in(channel, out options);
            _adaptor.get_product_info(_channel, out pi);

            if (_adaptor.is_drive_mode(channel, G2SEARCH_DRIVE.MODE.IDR))
            {
                using (form_text_in_search form = new form_text_in_search(options, true))
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

                    options = form._options_G1;
                    if (options._begin.valid)
                    {
                        options._begin.set(spot._time.year, spot._time.month, spot._time.day);
                    }
                    else
                    {
                        options._begin.set(spot._time.year, spot._time.month, spot._time.day, 0, 0, 0);
                    }

                    if (options._end.valid)
                    {
                        options._end.set(spot._time.year, spot._time.month, spot._time.day);
                    }
                    else
                    {
                        options._end.set(spot._time.year, spot._time.month, spot._time.day, 23, 59, 59);
                    }
                }
            }
            else
            {
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

                    options = form._options_G1;
                }
            }

        //  options._record = ;
            options._reload = true;

            _event_list.Items.Clear();
            _adaptor.set_query_mode(channel, G2SEARCH_QUERY.MODE.TEXT_IN);
            _adaptor.set_change_search_mode(channel, true);

            if (_adaptor.request_query_text_in(channel, ref options))
            {
                _screen.message().disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 1 * 60 * 1000, false);

                set_enable(false);
            }
        }
        protected void imp_search_text_in_next(int channel)
        {
            if (_adaptor.is_query_mode(channel, G2SEARCH_QUERY.MODE.TEXT_IN) != true)
            {
                return;
            }

            G2SEARCH_LOG_INFO log;
            bool proceed = (_adaptor.is_drive_mode(channel, G2SEARCH_DRIVE.MODE.IDR)) ?
                            _adaptor.get_query_result_event(channel, 99, out log) :
                            _adaptor.get_query_result_event(channel, 0, out log);

            G2TEXT_IN_QUERY_CONDITION options;
            if (_adaptor.get_query_condition_text_in(channel, out options))
            {
                if (_adaptor.is_drive_mode(channel, G2SEARCH_DRIVE.MODE.IDR))
                {
                    options._reload = false;
                    options._begin = log._time;
                }
                else
                {
                    options._reload = false;
                    options._end = log._time;
                }

                set_enable(false);

                _adaptor.request_query_text_in(channel, ref options);
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
        protected void imp_event_move_to(int channel, int selected)
        {
            if (_adaptor.is_connected(channel))
            {
                G2SEARCH_LOG_INFO log;
                if (_adaptor.get_query_result_event(channel, selected, out log))
                {
                    g2channel_set channelset = new g2channel_set();
                    channelset.from(log._cameras);

                    if (channelset.empty())
                    {
                        _screen.message().disp(STRING.NIS_NO_RECORDED_IMAGE, STRING.get(STRING.NIS_NO_RECORDED_IMAGE), 5000, false);
                        return;
                    }
                    else
                    {
                        int[] panes = channelset.to_array();
                        _screen.clear_last_image(panes, true);
                        _screen.set_format_range(panes, true);
                        _adaptor.request_event_image(channel, selected, false);
                    }
                }
            }
        }

        public void on_receive_no_frame(int channel)
        {
            G2SEARCH_PLAYBACK.COMMAND command = _adaptor.get_current_command(channel);
            if (command == G2SEARCH_PLAYBACK.COMMAND.GOTO_SEC ||
                command == G2SEARCH_PLAYBACK.COMMAND.GOTO_EVENT)
            {
                _screen.message().disp(STRING.NIS_NO_RECORDED_IMAGE, STRING.get(STRING.NIS_NO_RECORDED_IMAGE), 5000, false);
            }
        }
        public void on_receive_play_speed_changed(int channel, int speed) { }
        public void on_receive_play_stop_post(int channel, G2SEARCH_DRIVE.MODE mode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_play_stop_post(channel, mode); });
                return;
            }

            _screen.set_play_speed(channel, 0);

            imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.STOP);
        }
        public void on_receive_scope_list(int channel, G2SCOPE[] scopes)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_scope_list(channel, scopes); });
                return;
            }

            if (scopes.Length == 0)
            {
                _screen.message().disp(STRING.NIS_NO_RECORDED_IMAGE, STRING.get(STRING.NIS_NO_RECORDED_IMAGE), 5000, false);
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
                        _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.GOTO_SPOT, spot);
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
                _screen.message().disp(STRING.NIS_NO_RECORDED_IMAGE, STRING.get(STRING.NIS_NO_RECORDED_IMAGE), 5000, false);
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
                        _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.GOTO_SPOT, spot);
                    }
                }
            }
        }
        public void on_receive_event_log(int channel, G2SEARCH_LOG_INFO[] logs)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_event_log(channel, logs); });
                return;
            }

            if (logs.Length == 0)
            {
                _screen.message().disp(STRING.NIS_NO_RESULT, STRING.get(STRING.NIS_NO_RESULT), 5000, false);
                set_enable(true);
                BTN_TEXT_IN_SEARCH_NEXT.Enabled = BTN_EVENT_SEARCH_NEXT.Enabled = false;
                return;
            }

            _screen.message().hide(STRING.NIS_SEARCHING);

            set_controller_mode(MODE.EVENT);
            set_enable(true);

            _event_list.BeginUpdate();
            _event_list.Items.Clear();

            int i = 0;
            foreach (G2SEARCH_LOG_INFO e in logs)
            {
                string source = (G2EVENT_INFO.is_event_camera(e.event_level2)) ? string.Format("Camera {0}", e._event_id + 1) :
                                (G2EVENT_INFO.is_event_alarm(e.event_level2)) ? string.Format("AlarmIn {0}", e._event_id + 1) :
                                (G2EVENT_INFO.is_event_audio(e.event_level2)) ? string.Format("AudioIn {0}", e._event_id + 1) :
                                (G2EVENT_INFO.is_event_text_in(e.event_level2)) ? string.Format("TextIn {0}", e._event_id + 1) : "";
                string type = G2EVENT_INFO.string_event_type(e.event_level1, e.event_level2);
                string time = e._time.to_string_date_time();
                string data = e._label;

                ListViewItem lvi = new ListViewItem(type);
                lvi.Tag = e;
                lvi.SubItems.Add(source.ToString());
                lvi.SubItems.Add(data);
                lvi.SubItems.Add(time);
                lvi.SubItems[0].Tag = i++;

                _event_list.Items.Insert(0, lvi);
            }

            _event_list.EndUpdate();
            _event_list.Items[0].Selected = true;

            BTN_EVENT_SEARCH_NEXT.Enabled = (logs.Length >= 100);
            BTN_TEXT_IN_SEARCH_NEXT.Enabled = false;
        }
        public void on_receive_text_in_log(int channel, G2TEXT_IN[] logs)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_receive_text_in_log(channel, logs); });
                return;
            }

            if (logs.Length == 0)
            {
                _screen.message().disp(STRING.NIS_NO_RESULT, STRING.get(STRING.NIS_NO_RESULT), 5000, false);
                set_enable(true);
                BTN_TEXT_IN_SEARCH_NEXT.Enabled = BTN_EVENT_SEARCH_NEXT.Enabled = false;
                return;
            }

            _screen.message().hide(STRING.NIS_SEARCHING);

            set_controller_mode(MODE.EVENT);
            set_enable(true);

            _event_list.BeginUpdate();
            _event_list.Items.Clear();

            int i = 0;
            foreach (G2TEXT_IN e in logs)
            {
                string source = string.Format("Text-In {0}", e.channel + 1);
                string type = g2foundation.get_string_event_type_g2(G2EVENT.TYPE.TEXT_IN_ON);
                string time = e.time.to_string_date_time();
                string data = e.data;

                ListViewItem lvi = new ListViewItem(type);
                lvi.Tag = e;
                lvi.SubItems.Add(source.ToString());
                lvi.SubItems.Add(data);
                lvi.SubItems.Add(time);
                lvi.SubItems[0].Tag = i++;

                _event_list.Items.Insert(0, lvi);
            }

            _event_list.EndUpdate();
            _event_list.Items[0].Selected = true;

            BTN_TEXT_IN_SEARCH_NEXT.Enabled = (logs.Length >= 100);
            BTN_EVENT_SEARCH_NEXT.Enabled = false;
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
            _enable_audio_control = (format == screen_format.FORMAT.LAYOUT1X1) &&
                                    (_adaptor.is_connected(channel) && _adaptor.is_stopped(channel));
            CHK_AUDIO_PLAY.Enabled = _support_audio && _enable_audio_control;
        }
        public void on_screen_image_disp(ref G2FRAME frame)
        {
            G2SEARCH_PLAYBACK.COMMAND command = _adaptor.get_current_command(_channel);

            if (command == G2SEARCH_PLAYBACK.COMMAND.PLAY ||
                command == G2SEARCH_PLAYBACK.COMMAND.REW ||
                command == G2SEARCH_PLAYBACK.COMMAND.FF)
            {
                return;
            }

            if (command == G2SEARCH_PLAYBACK.COMMAND.GOTO_SEC)
            {
                _screen.message().hide(STRING.NIS_NO_RECORDED_IMAGE);
            }
            else if (command == G2SEARCH_PLAYBACK.COMMAND.GOTO_EVENT)
            {
                _screen.message().hide(STRING.NIS_NO_RECORDED_IMAGE);
            }

            imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.STOP);
        }
        public void on_screen_play_end_loaded(int channel)
        {
            _adaptor.request_notify_end_of_play(channel);

            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    imp_stop(channel);
                    imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.STOP);
                });
            }
            else
            {
                imp_stop(channel);
                imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.STOP);
            }
        }

        public void on_time_table_move_to_spot(G2SPOT spot)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel) &&
                _timer_stopped.Enabled != true)
            {
                imp_select_spot(channel, spot);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_select_spot(channel, spot); });
                imp_stop(channel);
            }
        }

        public void on_event_list_selected(G2SEARCH_LOG_INFO log)
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
                        int selected = (int)_event_list.SelectedItems[0].SubItems[0].Tag;
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
                imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.PREV);
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
                imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.NEXT);
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
            int channel = _channel;
            if (_adaptor.is_stopped(channel) != true)
            {
                return;
            }

            set_enable(true);
            imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.PLAY);
            SLD_PLAY_SPEED.Visible =
            STC_PLAY_SPEED.Visible = true;
            SLD_FAST_SPEED.Visible =
            STC_FAST_SPEED.Visible = false;

            _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.PLAY);
            _screen.set_play_speed(channel, _speed_play);
        }
        private void on_btn_stop(object sender, EventArgs e)
        {
            imp_stop(_channel);
        }
        private void on_btn_rew(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel) != true)
            {
                return;
            }

            set_enable(true);
            imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.REW);
            SLD_FAST_SPEED.Visible =
            STC_FAST_SPEED.Visible = true;
            SLD_PLAY_SPEED.Visible =
            STC_PLAY_SPEED.Visible = false;

            _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.REW);
            _screen.set_play_speed(channel, -_speed_fast);
        }
        private void on_btn_ff(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel) != true)
            {
                return;
            }

            set_enable(true);
            imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.FF);
            SLD_FAST_SPEED.Visible =
            STC_FAST_SPEED.Visible = true;
            SLD_PLAY_SPEED.Visible =
            STC_PLAY_SPEED.Visible = false;

            _adaptor.request_playback(channel, G2SEARCH_PLAYBACK.COMMAND.FF);
            _screen.set_play_speed(channel, _speed_fast);
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
                imp_set_play_controls(G2SEARCH_PLAYBACK.COMMAND.FIRST);
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
            request_goto_last();
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
            int speed = 0;
            if (sender == SLD_PLAY_SPEED)
            {
                _speed_play = speed = SLD_PLAY_SPEED.Value;

                float speedf = _speed_play / 10.0F;
                STC_PLAY_SPEED.Text = speedf.ToString("x0.0");
            }
            else
            {
                _speed_fast = speed = SLD_FAST_SPEED.Value;
                speed = (_adaptor.get_current_command(_channel) == G2SEARCH_PLAYBACK.COMMAND.REW) ? -speed : speed;
                float speedf = _speed_fast / 10.0F;
                STC_FAST_SPEED.Text = speedf.ToString("x0.0");
            }

            if (_adaptor.is_stopped(_channel) != true)
            {
                _screen.set_play_speed(_channel, speed);
            }
        }
        private void on_speed_value_reset(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (sender == SLD_PLAY_SPEED)
                {
                    SLD_PLAY_SPEED.Value = 10;
                }
                else
                {
                    SLD_FAST_SPEED.Value = 40;
                }
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
        private void on_cbx_segment_selected_index_changed(object sender, EventArgs e)
        {
            int id = CBX_SEGMENT.SelectedIndex;

            time_table_hour table = _table as time_table_hour;
            if (table != null)
            {
                if (_adaptor.set_change_segment(_channel, id))
                {
                    table.select_segment_id(id);
                }
            }
        }
        private void on_btn_mode_timelapse(object sender, EventArgs e)
        {
            set_controller_mode(MODE.TIMELAPSE);
        }
        private void on_btn_mode_event(object sender, EventArgs e)
        {
            if (_event_list.Items.Count == 0)
            {
                if (_adaptor.is_query_mode(_channel, G2SEARCH_QUERY.MODE.TEXT_IN))
                {
                    on_btn_text_in_search(BTN_TEXT_IN_SEARCH, new EventArgs());
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
        private void on_btn_event_search_next(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel) &&
                _timer_stopped.Enabled != true)
            {
                imp_search_event_next(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_search_event_next(channel); });
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
        private void on_btn_text_in_search_next(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_stopped(channel) &&
                _timer_stopped.Enabled != true)
            {
                imp_search_text_in_next(channel);
            }
            else
            {
                set_reserve((MethodInvoker)delegate() { imp_search_text_in_next(channel); });
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

        private g2search _adaptor;
        private screen_pane _screen;
        private search_data _data;
        private time_table_base _table;
        private ListView _event_list;
        private int _channel;
        private int _step_interval;
        private int _speed_play;
        private int _speed_fast;
        private bool _support_segment_loader;
        private bool _support_segment_select;
        private bool _support_goto_date;
        private bool _support_step_interval;
        private bool _support_audio;
        private bool _support_text_in_search;
        private bool _support_export_clip;
        private bool _enable_audio_control;
        private MethodInvoker _reserved;
        private Timer _timer_stopped;
        private Timer _timer_event_image;
        private Control _last_focused;
    }
}
