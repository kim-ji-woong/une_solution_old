using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class form_watch : screen_listener
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
        public void screen_channel_set_update(int channel)
        {
            if (_transfer_multi_stream)
            {
                for (int i = 0; i < _count_stream; ++i)
                {
                    camera_pane cp = _screen.get_pane(i);
                    if (cp != null)
                    {
                        cp._channelext = 0;
                        cp._stream_id = i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _count_camera; ++i)
                {
                    camera_pane cp = _screen.get_pane(i);
                    if (cp != null)
                    {
                        cp._channelext = i;
                        cp._stream_id = _adaptor.get_stream_id(channel, i);
                    }
                }
            }
        }
        public void screen_set_device_status(int channel, ref G2DEVICE_STATUS status)
        {
            screen_set_device_status(channel, ref status, true);
        }
        public void screen_set_device_status(int channel, ref G2DEVICE_STATUS status, bool update)
        {
            for (int i = 0; i < _count_camera; ++i)
            {
                camera_pane.STATUS camera_status = camera_pane.STATUS.UNDEFINED;
                if (_adaptor.is_authority(channel, G2RAS_AUTHORITY.TYPE.AUTHORITY_COVERT_CAMERA_VIEW) != true)
                {
                    G2DEVICE_STATUS.COVERT covert = status.convert(i);
                    if (covert == G2DEVICE_STATUS.COVERT.COVERT1)
                    {
                        camera_status = camera_pane.STATUS.COVERT_L1;
                    }
                    else if (covert == G2DEVICE_STATUS.COVERT.COVERT2)
                    {
                        camera_status = camera_pane.STATUS.COVERT_L2;
                    }
                }

                if (camera_status == camera_pane.STATUS.UNDEFINED)
                {
                    G2DEVICE_STATUS.CAMERA fact = status.camera(i);
                    camera_status = (fact == G2DEVICE_STATUS.CAMERA.ACTIVE ||
                                     fact == G2DEVICE_STATUS.CAMERA.MULTISTREAM) ? camera_pane.STATUS.ENABLE :
                                    (fact == G2DEVICE_STATUS.CAMERA.INACTIVE) ? camera_pane.STATUS.INACTIVATE :
                                    (fact == G2DEVICE_STATUS.CAMERA.VIDEOLOSS) ? camera_pane.STATUS.NO_VIDEO :
                                    (fact == G2DEVICE_STATUS.CAMERA.NOTCONNECTED) ? camera_pane.STATUS.NOT_CONNECTED : camera_pane.STATUS.DISABLE;
                }

                if (_transfer_multi_stream)
                {
                    G2DEVICE_STATUS_STREAM_INFO si;
                    camera_pane.STATUS pane_status;
                    string title;

                    for (int j = 0; j < _count_stream; ++j)
                    {
                        title = status._camera_desc[i];
                        pane_status = camera_status;
                        if (_adaptor.get_status_stream_info(channel, i, j, out si))
                        {
                            title = si._title;
                            if (title.Length == 0) title = string.Format("Stream {0}", j + 1);
                            if (si.is_on != true)
                            {
                                pane_status = camera_pane.STATUS.STREAM_OFF;
                            }
                        }
                        _screen.set_pane_status(j, pane_status, false);
                        _screen.set_pane_title(j, title, false);
                    }
                }
                else
                {
                    _screen.set_pane_status(i, camera_status, false);
                    _screen.set_pane_title(i, status._camera_desc[i], false);
                }
            }

            if (update)
            {
                _screen.update();
            }
        }

        public void on_screen_changed_pane(int pane)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_screen_changed_pane(pane); });
                return;
            }

            feature_control_color_screen_changed_pane(pane);
            feature_control_PTZ_screen_changed_pane(pane);
            feature_control_audio_screen_changed_pane(pane);
            feature_control_multi_stream_screen_changed_pane(pane);
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

            int channel = _channel;
            if (valid_channel(channel) && _adaptor.is_connected(channel))
            {
                g2channel_set pane_set = new g2channel_set();
                g2channel_stream_set stream_set = new g2channel_stream_set();
                g2channel_stream_set remove_set = new g2channel_stream_set();

                int pane_count = _transfer_multi_stream ? _count_stream : _count_camera;

                for (int i = 0; i < pane_count; ++i)
                {
                    G2CHANNEL_STREAM cs = _screen.get_pane_channel_stream(i);
                    if (cs.valid)
                    {
                        if (_screen.is_visible(i))
                        {
                            stream_set.insert(cs);
                            pane_set.insert(i);
                        }
                        else
                        {
                            remove_set.insert(cs);
                        }
                    }
                }

                if (_transfer_multi_stream)
                {
                    _adaptor.set_camera_stream_set(channel, stream_set, false);
                }
                else
                {
                    _adaptor.set_camera_list(channel, stream_set.to_channel_set(), false);
                }

                if (remove_set.empty() != true)
                {
                    frame_buf buf = _screen.buf_manager().get(channel);
                    if (buf != null)
                    {
                        buf.clear(remove_set);
                    }
                }

                g2decoder decoder = _screen.decoder();
                lock (decoder)
                {
                    for (int i = 0; i < pane_count; ++i)
                    {
                        if (pane_set.contains(i) != true)
                        {
                            decoder.close(i);
                        }
                    }
                }
            }
        }

        public void on_screen_image_disp(int pane, ref G2FRAME frame) { }
        public void on_screen_search_stopped(int channel, int pane, G2SPOT spot) { }
        public void on_screen_play_end_loaded(int channel) { }

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
