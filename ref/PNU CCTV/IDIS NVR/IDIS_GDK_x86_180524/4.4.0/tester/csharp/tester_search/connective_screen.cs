using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class form_search : screen_listener
    {
        public void init_connective_screen()
        {
            _screen = SCREEN_PANE;
            _screen.option_use_scaler_G2 = true;
            _screen.create(2, 36);
            _screen.set_listener(this);
            _screen.set_pane_select(0, false);
            _screen.set_format(16, true);
        }

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
            if (mode == screen_format.CHANGED.LAYOUT)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate()
                    {
                        BTN_SCREEN_FORMAT_PREV.Enabled =
                        BTN_SCREEN_FORMAT_NEXT.Enabled = _screen.fomatter().is_enable_group();
                    });
                }
                else
                {
                    BTN_SCREEN_FORMAT_PREV.Enabled =
                    BTN_SCREEN_FORMAT_NEXT.Enabled = _screen.fomatter().is_enable_group();
                }
            }

            if (_controller != null)
            {
                _controller.on_screen_changed_format(format, mode);

                int channel = _channel;

                if (_adaptor.is_connected(channel))
                {
                    set_camera_list(channel);

                    if (_adaptor.is_stopped(channel) != true)
                    {
                        int pane = _screen.selected_pane();

                        List<int> buf = new List<int>();
                        for (int i = 0; i < _count_camera; ++i)
                        {
                            if (i != pane)
                            {
                                buf.Add(i);
                            }
                        }

                        int[] panes = buf.ToArray();

                        _screen.clear_last_image(panes, true);
                        _screen.clear_pane(panes, true);
                    }
                }
            }
        }
        public void on_screen_image_disp(int pane, ref G2FRAME frame)
        {
            G2SPOT spot = frame.spot;
            G2SPOT spot_table = _table.get_spot();

            _table.on_screen_image_disp(ref frame);
            _controller.on_screen_image_disp(ref frame);
            _data._spot_disp = spot;
            _data._spot_disp_channelext = frame.channel;

            if (spot_table._segment != spot._segment ||
                spot_table._time.is_same_date(spot._time) != true)
            {
                _controller.request_record_time(spot);
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

        public void on_mouse_click(camera_pane cp, MouseEventArgs e) { }
        public void on_mouse_doubleclick(camera_pane cp, MouseEventArgs e) { }

        private void on_control_screen_format_menu(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            screen_format.FORMAT format = (screen_format.FORMAT)mi.Tag;

            _screen.set_format(format, true);
        }
        private void on_control_screen_format(object sender, EventArgs e)
        {
            ContextMenu pop = new ContextMenu();
            EventHandler handler = new EventHandler(on_control_screen_format_menu);
            MenuItem[] mis = new MenuItem[(int)screen_format.FORMAT.COUNT];

            for (int i = 0; i < mis.Length; ++i)
            {
                screen_format.FORMAT format = (screen_format.FORMAT)i;
                string name = (format == screen_format.FORMAT.LAYOUT32P) ?
                               "Layout 32" : string.Format("Layout {0}x{1}", screen_format.get_col(format), screen_format.get_row(format));
                MenuItem mi = new MenuItem(name, handler);
                mi.Tag = format;
                mi.Checked = (format == _screen.fomatter()._format);
                mi.Enabled = (_screen.fomatter()._panes >= screen_format.panes_for_format(format));
                mis[i] = mi;
            }

            pop.MenuItems.AddRange(mis);
            pop.Show(this, new Point(BTN_SCREEN_FORMAT.Location.X, BTN_SCREEN_FORMAT.Bottom));
        }
        private void on_control_screen_format_prev(object sender, EventArgs e)
        {
            _screen.set_format_prev(true);
        }
        private void on_control_screen_format_next(object sender, EventArgs e)
        {
            _screen.set_format_next(true);
        }

        private screen_pane _screen;
    }
}
