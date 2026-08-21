#if _IDIS_NVR_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDK;

namespace UnE.Control.CCTVControl.IDIS_NVR
{
    public partial class IdisNvrSet : GDK_tester.screen_listener
    {
        private void init_connective_screen()
        {
            _screen.option_use_scaler_G2 = true;
            _screen.create(2, 36);
            //_screen.create(1, 1);
            _screen.set_listener(this);
            _screen.set_pane_select(0, false);
            _screen.set_format(16, true);
            //_screen.set_format(1, true);
        }

        public void screen_channel_set_update(int channel)
        {
            if (_transfer_multi_stream)
            {
                for (int i = 0; i < _count_stream; ++i)
                {
                    GDK_tester.camera_pane cp = _screen.get_pane(i);
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
                    GDK_tester.camera_pane cp = _screen.get_pane(i);
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
                GDK_tester.camera_pane.STATUS camera_status = GDK_tester.camera_pane.STATUS.UNDEFINED;
                if (_adaptor.is_authority(channel, G2RAS_AUTHORITY.TYPE.AUTHORITY_COVERT_CAMERA_VIEW) != true)
                {
                    G2DEVICE_STATUS.COVERT covert = status.convert(i);
                    if (covert == G2DEVICE_STATUS.COVERT.COVERT1)
                    {
                        camera_status = GDK_tester.camera_pane.STATUS.COVERT_L1;
                    }
                    else if (covert == G2DEVICE_STATUS.COVERT.COVERT2)
                    {
                        camera_status = GDK_tester.camera_pane.STATUS.COVERT_L2;
                    }
                }

                if (camera_status == GDK_tester.camera_pane.STATUS.UNDEFINED)
                {
                    G2DEVICE_STATUS.CAMERA fact = status.camera(i);
                    camera_status = (fact == G2DEVICE_STATUS.CAMERA.ACTIVE ||
                                     fact == G2DEVICE_STATUS.CAMERA.MULTISTREAM) ? GDK_tester.camera_pane.STATUS.ENABLE :
                                    (fact == G2DEVICE_STATUS.CAMERA.INACTIVE) ? GDK_tester.camera_pane.STATUS.INACTIVATE :
                                    (fact == G2DEVICE_STATUS.CAMERA.VIDEOLOSS) ? GDK_tester.camera_pane.STATUS.NO_VIDEO :
                                    (fact == G2DEVICE_STATUS.CAMERA.NOTCONNECTED) ? GDK_tester.camera_pane.STATUS.NOT_CONNECTED : GDK_tester.camera_pane.STATUS.DISABLE;
                }

                if (_transfer_multi_stream)
                {
                    G2DEVICE_STATUS_STREAM_INFO si;
                    GDK_tester.camera_pane.STATUS pane_status;
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
                                pane_status = GDK_tester.camera_pane.STATUS.STREAM_OFF;
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
            if (m_owner.InvokeRequired)
            {
                m_owner.BeginInvoke((MethodInvoker)delegate() { on_screen_changed_pane(pane); });
                return;
            }

            feature_control_color_screen_changed_pane(pane);
            feature_control_PTZ_screen_changed_pane(pane);
            feature_control_audio_screen_changed_pane(pane);
            feature_control_multi_stream_screen_changed_pane(pane);
        }

        public void on_screen_changed_format(GDK_tester.screen_format.FORMAT format, GDK_tester.screen_format.CHANGED mode)
            {
            if (mode == GDK_tester.screen_format.CHANGED.LAYOUT)
            {
                if (m_owner.InvokeRequired)
                {
                    //this.BeginInvoke((MethodInvoker)delegate()
                    //{
                    //    BTN_SCREEN_FORMAT_PREV.Enabled =
                    //    BTN_SCREEN_FORMAT_NEXT.Enabled = _screen.fomatter().is_enable_group();
                    //});
                }
                else
                {
                    //BTN_SCREEN_FORMAT_PREV.Enabled =
                    //BTN_SCREEN_FORMAT_NEXT.Enabled = _screen.fomatter().is_enable_group();
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
                    GDK_tester.frame_buf buf = _screen.buf_manager().get(channel);
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

        public void on_mouse_click(GDK_tester.camera_pane cp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_owner != null)
                    m_owner.OnMouseDown(this, MouseButtons.Left, e.X, e.Y);
            }
        }

        public void on_mouse_doubleclick(GDK_tester.camera_pane cp, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_owner != null)
                    m_owner.OnMouseDblClick(this, MouseButtons.Left, e.X, e.Y);
            }
        }
    }
}
#endif