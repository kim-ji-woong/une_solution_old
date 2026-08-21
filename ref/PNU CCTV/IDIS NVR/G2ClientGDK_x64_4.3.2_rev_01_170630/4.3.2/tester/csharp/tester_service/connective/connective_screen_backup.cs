using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    public partial class form_backup : screen_listener
    {
        private screen_pane _screen;

        public void init_connective_screen()
        {
            _screen = SCREEN_PANE;
            _screen.option_use_scaler_G2 = true;
            int camera_count;
            if (_total_camera_count == 1) camera_count = 1;
            else if (_total_camera_count < 10) camera_count = _total_camera_count < 5 ? 4 : 9;
            else if (_total_camera_count < 17) camera_count = _total_camera_count < 13 ? 12 : 16;
            else if (_total_camera_count < 26) camera_count = _total_camera_count < 21 ? 20 : 25;
            else if (_total_camera_count < 37) camera_count = _total_camera_count < 34 ? 33 : 36;
            else if (_total_camera_count < 50) camera_count = 49;
            else camera_count = 64;
            _screen.create(2, camera_count);
            _screen.set_listener(this);
            _screen.set_pane_select(0, false);
            _screen.set_format(camera_count, true);

            int[] panes = new int[_total_camera_count];
            for (int i = 0; i < _total_camera_count; ++i)
            {
                panes[i] = i;
            }
            _screen.set_pane_mode(panes, camera_pane.MODE.PLAY);
            _screen.set_pane_status(panes, camera_pane.STATUS.ENABLE, true);
        }

        #region screen_listener 멤버

        public void on_screen_changed_pane(int pane)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_screen_changed_pane(pane); });
                return;
            }

            load_time_table(_channel);

            if (_controller != null)
            {
                _controller.on_screen_changed_pane(pane);
            }
        }

        public void on_screen_changed_format(screen_format.FORMAT format, screen_format.CHANGED mode)
        {
            if (_controller != null && _adaptor.is_connected(_channel))
            {
                _controller.on_screen_changed_format(format, mode);

                g2channel_set channelset = new g2channel_set();
                if (format == screen_format.FORMAT.LAYOUT1X1)
                {
                    int pane = _screen.selected_pane();
                    channelset.insert(_backup_info.channelext_from_panel(pane));
                }
                else
                {
                    foreach (int ch in _backup_info.channelexts)
                    {
                        channelset.insert(ch);
                    }
                }

                frame_buf buf = _screen.buf_manager().get(_channel);
                if (buf != null)
                {
                    buf.remove();
                    buf.set_channelset(channelset, channelset);
                }

                G2SPOT spot;
                int panel = _screen.get_last_disp_spot(out spot);
                int channelext = _backup_info.channelext_from_panel(panel);
                G2ROLLBACK_INFO rbi = new G2ROLLBACK_INFO(channelext, spot);
                bool preparing = false;
                _adaptor.set_camera_list_interest(_channel, channelset);
                _adaptor.set_camera_list(_channel, channelset, ref rbi, true, out preparing);

                if (_adaptor.is_stopped(_channel) != true)
                {
                    int pane = _screen.selected_pane();

                    List<int> bufs = new List<int>();
                    for (int i = 0; i < _total_camera_count; ++i)
                    {
                        if (i != pane)
                        {
                            bufs.Add(i);
                        }
                    }

                    int[] panes = bufs.ToArray();

                    _screen.clear_last_image(panes, true);
                    _screen.clear_pane(panes, true);
                }
            }
        }

        public void on_screen_image_disp(int pane, ref G2FRAME frame)
        {
            G2SPOT spot = frame.spot;
            G2SPOT spot_table = _table.get_spot();

            _table.on_screen_image_disp(ref frame);
            _controller.on_screen_image_disp(ref frame);

            _s_data._spot_disp = spot;
            _s_data._spot_disp_channelext = frame.channel;

            if (spot_table._time.is_same_date(spot._time) == false)
            {
                G2TIME from = spot._time.date;
                G2TIME to = from + new G2TIME_SPAN(0, 23, 59, 59);
                g2channel_set channelset;
                _adaptor.get_camera_list(_channel, out channelset);
                _adaptor.request_scope_list(_channel, ref from, ref to, channelset, (int)G2PLAY_SCOPE_TYPE.TYPE.GOTO);
            }
        }

        public void on_screen_search_stopped(int channel, int pane, G2SPOT spot)
        {
            _screen.resume();
        }

        public void on_screen_play_end_loaded(int channel)
        {
            _controller.on_screen_play_end_loaded(channel);
        }

        #endregion
    }
}
