using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    public class screen_pane_service : screen_pane
    {
        new public void create(int connections, int panes)
        {
            _message.Owner = this.ParentForm;
            _screen = (PictureBox)SCREEN_FRAME;
            _graphics = _screen.CreateGraphics();
            _pane_count = panes;
            _pane_selected = -1;
            _format = new screen_format(panes);
            _panes = new camera_pane_service[_pane_count * 2];

            for (int i = 0; i < _panes.Length; ++i)
            {
                camera_pane cp = new camera_pane_service(this, i);
                cp._stub = (i >= panes);
                _panes[i] = cp;
            }

            _buf_manager.create(connections, this);
            _decoder.startup(_pane_count);
            _looper.create(looper_proc);
            _looper.name = "GDK_tester::screen_looper";
            _search_audio.create(_buf_manager);
        }

        new public void looper_proc()
        {
            frame_element frame = null;
            int pane = _buf_manager.pop(out frame);

            if (valid_pane(pane))
            {
                if (frame != null &&
                    frame.valid)
                {
                    if (is_format1x1())
                    {
                        frame_buf buf = _buf_manager.get(frame.channel);
                        if (buf != null &&
                            buf.is_live() != true)
                        {
                            if (buf.speed() == 10)
                            {
                                if (_buf_manager.get_audio_count() > 0)
                                {
                                    search_audio_play(frame);
                                }
                            }
                        }
                    }

                    video_play(ref frame);
                }
            }
            else if (pane == (int)frame_element.LOADED.END_OF)
            {
                // in case that occur no-frame loaded as there is not inputed frame,
                // so is not called stop method, should stop audio play by compulsion.
                search_audio_stop();

                if (_listener != null)
                {
                    _listener.on_screen_play_end_loaded(frame.channel);
                }
            }
            else
            {
                Thread.Sleep(1);
            }

            // flush first element to prevent buffer full infinite loop
            // by input audio frames without video frame.
            if (_search_audio.is_suspended())
            {
                if (_buf_manager.get_audio_count() >= frame_buf.BUF_SIZE.AUDIO - 1)
                {
                    int channel_audio = _buf_manager.get_audio_channel();
                    if (channel_audio >= 0)
                    {
                        if (_buf_manager.get_frame_count(channel_audio) == 0)
                        {
                            _buf_manager.pop_audio();
                        }
                    }
                }
            }
        }

        new public void video_play(ref frame_element element)
        {
            int pane = element.pane;

            if (element.valid != true) return;
            if (is_visible(pane) != true)
            {
                camera_pane cp = get_pane(pane);
                if (cp != null && cp._mode == camera_pane.MODE.LIVE)
                {
                    _decoder.close(pane);

                    frame_buf buf = _buf_manager.get(element.channel);
                    if (buf != null)
                    {
                        buf.clear(element.channelext, element.stream_id);
                    }
                    return;
                }
            }

            G2FRAME frame = element.frame;
            G2DECODER_VIDEO_PARAM param = new G2DECODER_VIDEO_PARAM();
            G2DECODER_VIDEO_RESULT res;

            Size res_buf = new Size(Math.Max(720, (int)frame._width),
                                    Math.Max(576, (int)frame._height));

            if (res_buf.Width >= 0xFFFF ||
                res_buf.Height >= 0xFFFF)
            {
                res_buf.Width = 1920;
                res_buf.Height = 1088;
            }

            int len = res_buf.Width * res_buf.Height * 4;
            if (_buf_decoded == null)
            {
                _buf_decoded = new byte[len];
                _buf_filter = new byte[len];
            }
            else
            {
                if (_buf_decoded.Length < len)
                {
                    Array.Resize(ref _buf_decoded, len);
                    Array.Resize(ref _buf_filter, len);
                }
            }

            param._frame = frame;
            param._format = G2DECODER_VIDEO_PIX_FORMAT.TYPE.RGB32;
            param._decoder_id = pane;
            param._buf_data = element.data;
            param._buf_decompress = _buf_decoded;
            param._buf_filter = _buf_filter;

            bool ret = false;
            lock (_decoder)
            {
                ret = _decoder.decompress(ref param, out res);
            }

            if (element.sleep > 0)
            {
                int sleep = element.sleep - res._elapse;
                if (sleep > 0)
                {
                    g2looper.sleep(sleep);
                }
            }

            if (element.frame._extra._display)
            {
                if (options.probe_perf)
                {
                    set_probe_frame(element.pane, ref frame, ref res);
                }

                if (ret)
                {
                    display_frame(element.pane, ref element, param._buf_decompress, res._width, res._height);
                }
                else
                {
                    display_frame_error(element.pane, ref frame);
                }
            }

#if DEBUG
            if (res._result == (int)G2DECODER_VIDEO_RESULT.RESULT_TYPE.NOT_ENOUGH_BUF)
            {
                if (ret)
                {
                    Debug.WriteLine("screen_pane::decompress buf is not enough > failover via downscaling");
                }
                else
                {
                    Debug.WriteLine("screen_pane::decompress buf is not enough");
                }
            }
#endif
        }

        new public void display_frame(int pane, ref frame_element element, byte[] image, int width, int height)
        {
            if (valid_pane(pane))
            {
                bool noti = false;

                lock (this)
                {
                    camera_pane_service cp = (camera_pane_service)_panes[pane];
                    if (cp._visible)
                    {
                        cp._disp = false;
                        cp.disp_image(ref element.frame, image, width, height);
                        cp.set_spot(element.frame.spot);
                        cp.set_content_analytics_face_detection(ref element.face_detection);
                        cp.set_si_elevator_status(ref element.si_elevator_status);
                        cp.render();
                        cp.present(_graphics);

                        noti = (cp._mode == camera_pane.MODE.PLAY);
                    }
                }

                if (noti)
                {
                    if (_listener != null)
                    {
                        _listener.on_screen_image_disp(pane, ref element.frame);
                    }
                }
            }
        }
    }

    public class camera_pane_service : camera_pane
    {
        private bool is_rec;

        public bool IS_REC
        {
            get { return is_rec; }
            set { is_rec = value; }
        }


        public camera_pane_service(screen_pane parent, int num) : base(parent, num)
        {
            is_rec = false;
        }

        new public void render()
        {
            if (_surf_scene != null)
            {
                using (Graphics g = Graphics.FromImage(_surf_scene))
                {
                    g.DrawImageUnscaled(_surf_image, 0, 0);
                    disp_OSD(g);
                    disp_border(g);
                }
            }
        }

        new public void disp_OSD(Graphics g)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            Rectangle rc = _rect;
            rc.X = rc.Y = 0;

            if (_status != STATUS.ENABLE)
            {
                Font font = _parent.is_format1x1() && _rect.Height >= 480 ? _parent.options.font_1x1 : _parent.options.font;
                SolidBrush br = new SolidBrush(Color.White);
                br.Color = (_status == STATUS.NO_VIDEO) ? screen_options.color.pane.back_no_video :
                           (_status == STATUS.STREAM_OFF) ? screen_options.color.pane.back_stream_off :
                           (_stub) ? screen_options.color.pane.back_stub : Color.Transparent;
                SmoothingMode pre = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.None;
                g.FillRectangle(br, rc);
                g.SmoothingMode = pre;
                string s = (_status == STATUS.NO_VIDEO) ? "no video" :
                           (_status == STATUS.STREAM_OFF) ? "stream off" :
                           (_status == STATUS.NOT_CONNECTED) ? "not connected" :
                           (_status == STATUS.COVERT_L1) ? "covert" :
                           (_status == STATUS.COVERT_L2) ? "" : "";

                StringFormat fmt = new StringFormat();
                fmt.LineAlignment = StringAlignment.Center;
                fmt.Alignment = StringAlignment.Center;
                br.Color = Color.White;
                g.DrawString(s, font, br, rc, fmt);
            }

            if (_status == STATUS.UNDEFINED)
            {
                if (rc.Width > s_ci.Width && rc.Height > s_ci.Height)
                {
                    g.DrawImage(s_ci, constant_ratio.center_rect(ref rc, s_ci.Width, s_ci.Height));
                }
                else
                {
                    if (_stub)
                    {
                        InterpolationMode pre = g.InterpolationMode;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(s_ci, constant_ratio.imp(ref rc, s_ci.Width, s_ci.Height));
                        g.InterpolationMode = pre;
                    }
                }
            }

            disp_title(g);
            disp_time(g);
            disp_content_analytics(g);
            disp_si_elevator_status(g);
            disp_probe(g);

            disp_rec_dot(g);
        }

        public void disp_rec_dot(Graphics g)
        {
            if (is_rec)
            {
                RectangleF rc = _rect;
                InterpolationMode pre_interpolation_mode = g.InterpolationMode;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(GDK_tester.Properties.Resources.recording_dot,
                            new Rectangle(new Point((int)rc.Width - 20, 5), new Size(15, 15)));
                g.InterpolationMode = pre_interpolation_mode;
            }
        }
    }
}
