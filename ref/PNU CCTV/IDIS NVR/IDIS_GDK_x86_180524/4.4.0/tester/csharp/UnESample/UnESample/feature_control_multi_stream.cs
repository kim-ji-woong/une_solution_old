using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDK;

namespace UnESample
{
    public partial class FormMain
    {
        public void feature_control_multi_stream_screen_changed_pane(int pane)
        {
            bool enable = false;
            if (_screen.contains_pane(pane))
            {
                GDK_tester.camera_pane cp = _screen.get_pane(pane);
                if (_transfer_multi_stream)
                {
                    enable = cp.is_enable() || cp.is_stream_off();
                }
                else
                {
                    enable = cp.is_enable();
                    enable = enable && _adaptor.is_enable_multi_stream(_channel, cp._channelext);
                }
            }

            feature_control_multi_stream_set_enable(enable);
        }

        public void feature_control_multi_stream_set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_multi_stream_set_enable(enable); });
                return;
            }

            if (enable)
            {
                G2_PRODUCT_INFO pi;
                if (_adaptor.get_product_info(_channel, out pi))
                {
                    if (pi.remote_watch.transfer_multi_stream)
                    {
                        /*CONTROL_MULTI_STREAM_CHK_TRANSFER.Enabled = (pi.ip_camera.is_ip_camera) &&
                                                                    (_count_stream != 0 && _count_camera == 1);*/
                    }
                }
            }

            /*if (enable)
            {
                CONTROL_MULTI_STREAM.Enabled = (_transfer_multi_stream != true);
                CONTROL_MULTI_STREAM_CHK_TRANSFER.Checked = _transfer_multi_stream;
            }
            else
            {
                CONTROL_MULTI_STREAM.Enabled = false;
                CONTROL_MULTI_STREAM_CHK_TRANSFER.Checked =
                CONTROL_MULTI_STREAM_CHK_TRANSFER.Enabled = false;
            }*/
        }

        public void feature_control_multi_stream_status_update()
        {
            if (_transfer_multi_stream) return;

            int channel = _channel;
            for (int i = 0; i < _count_camera; ++i)
            {
                int camera = i;
                int stream_id = _adaptor.get_stream_id(channel, camera);
                if (stream_id > 0)
                {
                    g2channel_set remote_stream = new g2channel_set();
                    remote_stream.from(_adaptor.get_stream_remote(channel, camera));

                    if (remote_stream.contains(stream_id) != true)
                    {
                        GDK_tester.camera_pane cp = _screen.get_pane(camera);
                        int pre_stream_id = stream_id;

                        stream_id = remote_stream[0];
                        cp._stream_id = stream_id;

                        g2channel_stream_set cs;
                        if (_adaptor.get_camera_stream_set(channel, out cs))
                        {
                            cs.erase(camera);
                            cs.insert(camera, stream_id);
                            _adaptor.set_stream_id(channel, camera, stream_id);
                            _adaptor.set_camera_stream_set(channel, cs, false);
                        }

                        GDK_tester.frame_buf buf = _screen.buf_manager().get(channel);
                        if (buf != null)
                        {
                            buf.clear(camera, pre_stream_id);
                        }

                        g2decoder decoder = _screen.decoder();
                        lock (decoder)
                        {
                            decoder.close(camera);
                        }
                    }
                }
            }
        }

        private void SetMultiStream()
        {
            int channel = _channel;
            int camera = _screen.selected_pane_channelext();
            g2channel_set remote_stream = new g2channel_set();
            remote_stream.from(_adaptor.get_stream_remote(channel, camera));

            if (remote_stream.empty())
            {
                return;
            }

            G2_PRODUCT_INFO_CAPS.REMOTE_WATCH caps;
            if (_adaptor.get_remote_watch_caps(channel, out caps) != true)
            {
                return;
            }

            //ContextMenu pop = new ContextMenu();
            //MenuItem[] mis = new MenuItem[caps.stream_count];

            int nSelectedStreamID = -1;
            float fMinFrame = 0.0f;

            for (int i = 0; i < caps.stream_count; ++i)
            {
                //int stream_id = i;
                //string name;
                //string title;
                G2DEVICE_STATUS_STREAM_INFO si;
                if (remote_stream.contains(i) && _adaptor.get_status_stream_info(channel, camera, i, out si))
                {
                    if (si._width <= 0 || si._height <= 0 || si._ips <= 0)
                        continue;

                    float frame = si._width * si._height * si._ips;

                    if (nSelectedStreamID < 0)
                    {
                        nSelectedStreamID = i;
                        fMinFrame = frame;
                    }
                    else if (frame < fMinFrame)
                    {
                        nSelectedStreamID = i;
                        fMinFrame = frame;
                    }

                    /*name = string.Format("stream {0} ({1}) {2}x{3}@{4}ips {5}", stream_id + 1, si.codec, si._width, si._height, si._ips.ToString("0.00"), si.bitrate_control_type);
                    title = si._title;
                    if (title.Length != 0)
                    {
                        name += " / ";
                        name += title;
                    }*/
                }
                else
                {
                    continue;
                    //name = string.Format("stream {0}", stream_id + 1);
                }

                /*MenuItem mi = new MenuItem(name, new EventHandler(on_control_multi_stream_select_stream));
                mi.Enabled = remote_stream.contains(i);
                mi.Tag = stream_id;
                mis[i] = mi;*/
            }

            /*int selected = _adaptor.get_stream_id(channel, camera);
            if (selected >= 0 &&
                selected < caps.stream_count)
            {
                mis[selected].Checked = true;
            }

            pop.MenuItems.AddRange(mis);
            pop.Show(this, new Point(CONTROL_MULTI_STREAM.Location.X, CONTROL_MULTI_STREAM.Bottom));*/

            if (nSelectedStreamID >= 0)
                SelectStream(nSelectedStreamID);
        }

        private void SelectStream(int stream_id)
        {
            int channel = _channel;
            int camera = _screen.selected_pane_channelext();

            if (_adaptor.get_stream_id(channel, camera) != stream_id &&
                _screen.contains_pane(camera))
            {
                G2CHANNEL_STREAM stream_pre = _screen.get_pane_channel_stream(camera);

                _screen.get_pane(camera)._stream_id = stream_id;
                g2channel_stream_set cs;
                if (_adaptor.get_camera_stream_set(channel, out cs))
                {
                    cs.erase(camera);
                    cs.insert(camera, stream_id);
                    _adaptor.set_stream_id(channel, camera, stream_id);
                    _adaptor.set_camera_stream_set(channel, cs, false);
                }
                _screen.fire_changed_pane(false);

                GDK_tester.frame_buf buf = _screen.buf_manager().get(channel);
                if (buf != null)
                {
                    buf.clear(stream_pre._channel, stream_pre._stream);
                }

                g2decoder decoder = _screen.decoder();
                lock (decoder)
                {
                    decoder.close(camera);
                }
            }
        }
    }
}
