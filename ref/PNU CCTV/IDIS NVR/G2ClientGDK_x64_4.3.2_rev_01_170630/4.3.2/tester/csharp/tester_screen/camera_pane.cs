using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using GDK;

namespace GDK_tester
{
    public partial class camera_pane
    {
        public enum MODE
        {
            UNDEFINED = -1,
            LIVE,
            PLAY
        }
        public enum STATUS
        {
            UNDEFINED = -1,
            DISABLE = 0,
            INACTIVATE,
            ENABLE,
            NO_VIDEO,
            STREAM_OFF,
            COVERT_L1,
            COVERT_L2,
            NOT_CONNECTED,
            RECONNECTING,
            COUNT
        }

        public class last_image_type
        {
            public last_image_type() { reset(); }
            public void reset()
            {
                pts = 0;
                image = null;
                image_res = new Size();
                frame = new G2FRAME();
                frame._index._spot.reset();
            }

            public void reset_image()
            {
                image = null;
                image_res = new Size();
            }

            public void set_image(byte[] image, int width, int height)
            {
                if (this.image == null ||
                    this.image.Length != image.Length)
                {
                    this.image = (byte[])image.Clone();
                }
                else
                {
                    Array.Copy(image, this.image, image.Length);
                }

                this.image_res.Width = width;
                this.image_res.Height = height;
            }

            public long pts;
            public byte[] image;
            public Size image_res;
            public G2FRAME frame;
        }

        public class probe_frame
        {
            public void reset()
            {
                this.channelext = -1;
                this.stream_id = -1;
                this.size = 0;
                this.spot = G2SPOT.INVALID_SPOT;
                this.type = G2FRAME.TYPE.INVALID_TYPE;
                this.flags = 0;
                this.res_coded = new Size();
                this.res = new Size();
                this.dec = G2DECODER_CODEC.TYPE.UNDEFINED;
                this.dec_elpase = 0;
                this.empty = true;
            }

            public void set(ref G2FRAME frame, ref G2DECODER_VIDEO_RESULT dec)
            {
                this.channelext = frame.channel;
                this.stream_id = frame.stream_id;
                this.size = frame.data_size;
                this.spot = frame.spot;
                this.type = frame.type;
                this.flags = frame.flags;
                this.res_coded.Width = frame._width;
                this.res_coded.Height = frame._height;
                this.res.Width = dec._width;
                this.res.Height = dec._height;
                this.dec = frame.decoder;
                this.dec_elpase = dec._elapse;
                this.empty = false;
            }

            public override string ToString()
            {
                string t = type == G2FRAME.TYPE.I_FRAME ? "I" :
                           type == G2FRAME.TYPE.P_FRAME ? "P" :
                           type == G2FRAME.TYPE.X_FRAME ? "X" :
                           type == G2FRAME.TYPE.B_FRAME ? "B" : "";
                string f = "";
                if (flags != 0)
                {
                    if ((flags & G2FRAME.FLAG.PANIC) != 0) f += "P";
                    if ((flags & G2FRAME.FLAG.IRREGULAR) != 0) f += (f.Length == 0) ? "I" : "/I";
                    if ((flags & G2FRAME.FLAG.TIME_LAPSE) != 0) f += (f.Length == 0) ? "T" : "/T";
                    if ((flags & G2FRAME.FLAG.EVENT) != 0) f += (f.Length == 0) ? "E" : "/E";
                    if ((flags & G2FRAME.FLAG.PRE_EVENT) != 0) f += (f.Length == 0) ? "PE" : "/PE";
                    if ((flags & G2FRAME.FLAG.FINGERPRINT) != 0) f += (f.Length == 0) ? "F" : "/F";
                    if (f.Length != 0)
                    {
                        f = "Flags: " + f;
                    }
                }

                return string.Format("{0}x{1} (coded {2}x{3}) {4}\nCH: {5} ({6}) {7}\nSize: {8:D6} Tick: {{{9}}}{10}\nCodec: {11} Perf: {12:D2}",
                                        res.Width, res.Height, res_coded.Width, res_coded.Height, t,
                                        channelext, stream_id, f,
                                        size, spot._segment, spot._tick,
                                        dec, dec_elpase);
            }

            public int channelext;
            public int stream_id;
            public uint size;
            public G2SPOT spot;
            public G2FRAME.TYPE type;
            public G2FRAME.FLAG flags;
            public Size res_coded;
            public Size res;
            public G2DECODER_CODEC.TYPE dec;
            public int dec_elpase;
            public bool empty;
        }

        public camera_pane(screen_pane parent, int num)
        {
            this._parent = parent;
            this._num = num;
            this._channelext = -1;
            this._stream_id = -1;
            this._disp = true;
            this._visible = false;
            this._selected = false;
            this._last_image = new last_image_type();
            this._probe = new probe_frame();
            this._audio_enable = true;
            reset();
        }
        static camera_pane()
        {
            Stream res = Assembly.GetExecutingAssembly().GetManifestResourceStream("tester_screen.res.ci.png");
            s_ci = new Bitmap(res);
            res.Close();
        }

        public void reset()
        {
            _mode = MODE.UNDEFINED;
            _status = STATUS.UNDEFINED;
            _image_resolution = new Size();
            _rect_output = new Rectangle();
            _rect_source = new Rectangle();
            _spot.reset();
            _last_image.reset();
            _probe.reset();
            _title = "";
            _title_pre = "";
            _str_date = "";
            _str_time = "";
            _str_datetime_pre = "";
            _str_si_elevator_pre = "";
            _changed_spot = true;
            _changed_time = true;
            _content_analytics_face_detection.reset();
            _si_elevator_status.reset();

            if (_visible)
            {
                if (_surf_scene != null) Graphics.FromImage(_surf_scene).Clear(screen_options.color.pane.back);
                if (_surf_image != null) Graphics.FromImage(_surf_image).Clear(Color.Black);
                if (_surf_layer_title != null) Graphics.FromImage(_surf_layer_title).Clear(Color.Transparent);
                if (_surf_layer_datetime != null) Graphics.FromImage(_surf_layer_datetime).Clear(Color.Transparent);
                if (_surf_layer_si_elevator != null) Graphics.FromImage(_surf_layer_si_elevator).Clear(Color.Transparent);

                _disp = true;
                _rect_layer_title = new RectangleF();
                _rect_layer_datetime = new RectangleF();
                _rect_layer_si_elevator = new RectangleF();
            }
            else
            {
                if (_surf_scene != null) _surf_scene.Dispose();
                if (_surf_image != null) _surf_image.Dispose();
                if (_surf_layer_title != null) _surf_layer_title.Dispose();
                if (_surf_layer_datetime != null) _surf_layer_datetime.Dispose();
                if (_surf_layer_si_elevator != null) _surf_layer_si_elevator.Dispose();

                _surf_scene = null;
                _surf_image = null;
                _surf_layer_title = null;
                _surf_layer_datetime = null;
                _surf_layer_si_elevator = null;
            }
        }

        public void set_mode(MODE mode)
        {
            _mode = mode;
        }

        public void set_rect(ref Rectangle rect)
        {
            _rect = rect;
            if (rect.IsEmpty)
            {
                if (_surf_scene != null) _surf_scene.Dispose();
                if (_surf_image != null) _surf_image.Dispose();
                if (_surf_layer_title != null) _surf_layer_title.Dispose();
                if (_surf_layer_datetime != null) _surf_layer_datetime.Dispose();
                if (_surf_layer_si_elevator != null) _surf_layer_si_elevator.Dispose();

                _surf_scene = null;
                _surf_image = null;
                _surf_layer_title = null;
                _surf_layer_datetime = null;
                _surf_layer_si_elevator = null;
            }
            else
            {
                int aligned_cx = ((rect.Width + 31) >> 5) << 5;
                int aligned_cy = ((rect.Height + 31) >> 5) << 5;

                _surf_scene = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
                _surf_image = new Bitmap(aligned_cx, aligned_cy, PixelFormat.Format32bppRgb);
                _surf_layer_title = new Bitmap(rect.Width, 24, PixelFormat.Format32bppArgb);
                _surf_layer_datetime = new Bitmap(rect.Width, 24, PixelFormat.Format32bppArgb);
                _surf_layer_si_elevator = new Bitmap(rect.Width, 24, PixelFormat.Format32bppArgb);
                _rect_layer_title = new RectangleF();
                _rect_layer_datetime = new RectangleF();
                _rect_layer_si_elevator = new RectangleF();

                Graphics.FromImage(_surf_scene).Clear(screen_options.color.pane.back);
                Graphics.FromImage(_surf_image).Clear(Color.Black);
                Graphics.FromImage(_surf_layer_title).Clear(Color.Transparent);
                Graphics.FromImage(_surf_layer_datetime).Clear(Color.Transparent);
                Graphics.FromImage(_surf_layer_si_elevator).Clear(Color.Transparent);
            }
        }

        public void set_spot(G2SPOT spot)
        {
            if (_spot != spot)
            {
                if (_spot._time != spot._time)
                {
                    _changed_time = true;
                }
                _changed_spot = true;
                _spot = spot;
            }
        }

        public void set_content_analytics_face_detection(ref G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION content)
        {
            _content_analytics_face_detection = content;
        }

        public void set_si_elevator_status(ref G2CODEC_INFO_SI_ELEVATOR_STATUS status)
        {
            _si_elevator_status = status;
        }

        public bool is_enable() { return (_status == STATUS.ENABLE); }
        public bool is_stream_off() { return (_status == STATUS.STREAM_OFF); }
        public bool pt_in_pane(Point pt) { return _rect.Contains(pt); }

        public void disp_image(ref G2FRAME frame, byte[] image, int width, int height)
        {
            imp_disp_image(ref frame, image, width, height);
            if (_mode == MODE.PLAY)
            {
                _title = frame._title;
                _last_image.set_image(image, width, height);
                _last_image.frame = frame;
                _last_image.pts = ++_parent._pts;
            }
        }

        public void disp_last_image()
        {
            if (_last_image.image != null)
            {
                imp_disp_image(ref _last_image.frame, _last_image.image, _last_image.image_res.Width, _last_image.image_res.Height);
            }
        }

        public void disp_border(Graphics g)
        {
            Color color = _selected ? screen_options.color.pane.border_select :
                                      screen_options.color.pane.border;
            Pen pen = null;
            Rectangle r;
            if (_selected)
            {
                r = new Rectangle(0, 0, _rect.Width, _rect.Height);
                pen = new Pen(color, 2);
            }
            else
            {
                r = new Rectangle(0, 0, _rect.Width - 1, _rect.Height - 1);
                pen = new Pen(color, 1);
            }
            pen.Alignment = PenAlignment.Inset;

            SmoothingMode pre = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;
            g.DrawRectangle(pen, r);
            g.SmoothingMode = pre;
        }

        public void disp_OSD(Graphics g)
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
        }

        public void disp_content_analytics(Graphics g)
        {
            if (_rect_output.IsEmpty) return;
            if (_rect_source.IsEmpty) return;
            if (_content_analytics_face_detection._has)
            {
                RectangleF rect_source = _rect_source;
                SizeF ratio = new SizeF(_rect_output.Width / rect_source.Width, _rect_output.Height / rect_source.Height);
                SizeF size_image = _image_resolution;
                SizeF size_base = new SizeF(_content_analytics_face_detection._resolution);
                PointF offset = new PointF(-rect_source.Left, -rect_source.Top);
                List<RectangleF> rects = new List<RectangleF>();
                foreach (RectangleF var in _content_analytics_face_detection._rects)
                {
                    if (var.IsEmpty != true)
                    {
                        RectangleF area = new RectangleF((var.Left / size_base.Width) * size_image.Width,
                                                         (var.Top / size_base.Height) * size_image.Height,
                                                         (var.Width / size_base.Width) * size_image.Width,
                                                         (var.Height / size_base.Height) * size_image.Height);
                        if (area.IntersectsWith(rect_source))
                        {
                            area.Offset(offset);
                            area.X *= ratio.Width;
                            area.Y *= ratio.Height;
                            area.Width *= ratio.Width;
                            area.Height *= ratio.Height;
                            area.Offset(_rect_output.Left, _rect_output.Top);
                            rects.Add(area);
                        }
                    }
                }

                if (rects.Count > 0)
                {
                    Pen pen = new Pen(Color.FromArgb(240, 255, 255, 255), _rect_output.Width > 352 ? 2.0f : 1.0f);
                    g.SetClip(_rect_output);
                    g.DrawRectangles(pen, rects.ToArray());
                    g.ResetClip();
                }
            }
        }

        public void disp_si_elevator_status(Graphics g)
        {
            if (_si_elevator_status._has)
            {
                Font font = _parent.is_format1x1() && _rect.Height >= 480 ? _parent.options.font_1x1 : _parent.options.font;
                Rectangle rc = _rect;
                rc.X = rc.Y = 0;
                rc.Inflate(-4, -8);

                string s = (_si_elevator_status._floor < 0.0f) ? "B" : "F";
                s += ((float)(int)_si_elevator_status._floor == _si_elevator_status._floor) ? ((int)_si_elevator_status._floor).ToString() : _si_elevator_status._floor.ToString("0.0");
                s += " ";
                s += _si_elevator_status.door_status == G2CODEC_INFO_SI_ELEVATOR_STATUS.DOOR_STATUS.CLOSING ? ">>|<<" :
                     _si_elevator_status.door_status == G2CODEC_INFO_SI_ELEVATOR_STATUS.DOOR_STATUS.CLOSE   ? " >|< " :
                     _si_elevator_status.door_status == G2CODEC_INFO_SI_ELEVATOR_STATUS.DOOR_STATUS.OPENING ? "<<|>>" :
                     _si_elevator_status.door_status == G2CODEC_INFO_SI_ELEVATOR_STATUS.DOOR_STATUS.OPEN    ? " <|> " : " ";
                s += " ";
                s += _si_elevator_status.direction == G2CODEC_INFO_SI_ELEVATOR_STATUS.DIRECTION.DOWN ? "▼" :
                     _si_elevator_status.direction == G2CODEC_INFO_SI_ELEVATOR_STATUS.DIRECTION.STOP ? "■" :
                     _si_elevator_status.direction == G2CODEC_INFO_SI_ELEVATOR_STATUS.DIRECTION.UP   ? "▲" : " ";
                s += " ";
                s += _si_elevator_status.mode == G2CODEC_INFO_SI_ELEVATOR_STATUS.MODE.MANUAL ? "MANUAL" :
                     _si_elevator_status.mode == G2CODEC_INFO_SI_ELEVATOR_STATUS.MODE.AUTO   ? "AUTO" : " ";

                if (_str_si_elevator_pre != s ||
                    _rect_layer_si_elevator.IsEmpty)
                {
                    _rect_layer_si_elevator = imp_disp_string_to_layer(_surf_layer_si_elevator, font, s);
                    _str_si_elevator_pre = s;
                }

                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = 165.0f / 255.0f;
                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                Rectangle rect_dest = new Rectangle();
                rect_dest.X = (int)(constant_ratio.center_horz(rc.Left, rc.Right, _rect_layer_si_elevator.Width));
                rect_dest.Y = (int)(rc.Bottom - _rect_layer_si_elevator.Height - _rect_layer_datetime.Height);
                rect_dest.Width = (int)_rect_layer_si_elevator.Width;
                rect_dest.Height = (int)_rect_layer_si_elevator.Height;

                InterpolationMode pre_interpolation_mode = g.InterpolationMode;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(_surf_layer_si_elevator, rect_dest,
                            _rect_layer_si_elevator.X, _rect_layer_si_elevator.Y, _rect_layer_si_elevator.Width, _rect_layer_si_elevator.Height,
                            GraphicsUnit.Pixel, ia);
                g.InterpolationMode = pre_interpolation_mode;
            }
        }

        public void disp_title(Graphics g)
        {
            if (_status == STATUS.COVERT_L2 ||
                _status == STATUS.DISABLE ||
                _status == STATUS.UNDEFINED) return;
            if (_title.Length == 0) return;

            Font font = _parent.is_format1x1() && _rect.Height >= 480 ? _parent.options.font_1x1 : _parent.options.font;
            Rectangle rc = _rect;
            rc.X = rc.Y = 0;
            rc.Inflate(-4, -4);

            if (_title_pre != _title ||
                _rect_layer_title.IsEmpty)
            {
                _rect_layer_title = imp_disp_string_to_layer(_surf_layer_title, font, _title);
                _title_pre = _title;
            }

            InterpolationMode pre_interpolation_mode = g.InterpolationMode;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(_surf_layer_title, rc.X, rc.Y, _rect_layer_title, GraphicsUnit.Pixel);
            g.InterpolationMode = pre_interpolation_mode;
        }

        public void disp_time(Graphics g)
        {
            if (_status != STATUS.ENABLE) return;
            if (_spot._time.valid != true) return;

            Font font = _parent.is_format1x1() && _rect.Height >= 480 ? _parent.options.font_1x1 : _parent.options.font;
            RectangleF rc = _rect;
            rc.X = rc.Y = 0.0f;
            rc.Inflate(-4, -6);

            if (_changed_time)
            {
                _changed_time = false;
                _str_date = _spot._time.to_string_date();
                _str_time = _spot._time.to_string_time();
            }

            string s = (rc.Width < 160) ? _str_time : _str_date + " " + _str_time;
            if (s.Length != 0)
            {
                if (_str_datetime_pre != s ||
                    _rect_layer_datetime.IsEmpty)
                {
                    _str_datetime_pre = s;
                    _rect_layer_datetime = imp_disp_string_to_layer(_surf_layer_datetime, font, s);
                }

                InterpolationMode pre_interpolation_mode = g.InterpolationMode;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(_surf_layer_datetime,
                            constant_ratio.center_horz(rc.Left, rc.Right, _rect_layer_datetime.Width),
                            rc.Bottom - _rect_layer_datetime.Height,
                            _rect_layer_datetime,
                            GraphicsUnit.Pixel);
                g.InterpolationMode = pre_interpolation_mode;
            }
        }

        public void disp_probe(Graphics g)
        {
            if (_parent.option_use_probe_performance != true) return;
            if (_probe.empty) return;
            if (_status == STATUS.ENABLE)
            {
                Font font = _parent.is_format1x1() && _rect.Height >= 480 ? _parent.options.font_big :
                            _parent.fomatter().get_col() >= 4 ? _parent.options.font_small : _parent.options.font;
                Rectangle rc = _rect;
                rc.X = rc.Y = 0;
                rc.Inflate(-4, -4);

                string s = _probe.ToString();
                StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap);
                SolidBrush br = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
                fmt.LineAlignment = StringAlignment.Center;
                fmt.Alignment = StringAlignment.Near;
                Rectangle r = rc;
                r.Offset(-1, -1); g.DrawString(s, font, br, r, fmt);
                r.Offset(1, 0); g.DrawString(s, font, br, r, fmt);
                r.Offset(1, 0); g.DrawString(s, font, br, r, fmt);
                r.Offset(0, 1); g.DrawString(s, font, br, r, fmt);
                r.Offset(0, 1); g.DrawString(s, font, br, r, fmt);
                r.Offset(-1, 0); g.DrawString(s, font, br, r, fmt);
                r.Offset(-1, 0); g.DrawString(s, font, br, r, fmt);
                r.Offset(0, -1); g.DrawString(s, font, br, r, fmt);
                br.Color = Color.White;
                g.DrawString(s, font, br, rc, fmt);
            }
        }

        public void clear()
        {
            if (_surf_scene != null) Graphics.FromImage(_surf_scene).Clear(screen_options.color.pane.back);
            if (_surf_image != null) Graphics.FromImage(_surf_image).Clear(Color.Black);
            if (_surf_layer_title != null) Graphics.FromImage(_surf_layer_title).Clear(Color.Transparent);
            if (_surf_layer_datetime != null) Graphics.FromImage(_surf_layer_datetime).Clear(Color.Transparent);
            if (_surf_layer_si_elevator != null) Graphics.FromImage(_surf_layer_si_elevator).Clear(Color.Transparent);

            _rect_layer_title = new RectangleF();
            _rect_layer_datetime = new RectangleF();
            _rect_layer_si_elevator = new RectangleF();
            _image_resolution = new Size();
            _rect_output = new Rectangle();
            _rect_source = new Rectangle();
        }

        public void render()
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

        public bool present(Graphics g)
        {
            bool ret = false;
            if (_surf_scene != null)
            {
                try
                {
                    g.DrawImageUnscaled(_surf_scene, _rect);
                    ret = true;
                }
                catch (System.Exception) { }
            }
            return ret;
        }

        protected void imp_disp_image(ref G2FRAME frame, byte[] image, int width, int height)
        {
            Size res = new Size(width, height);
            Rectangle dst_rect = _rect;
            Rectangle src_rect = new Rectangle(0, 0, width, height);
            dst_rect.X = dst_rect.Y = 0;

            if (frame._extra._info._video._roi ||
                width < height)
            {
                // original ratio
                dst_rect = constant_ratio.get(ref dst_rect, src_rect.Width, src_rect.Height, frame._extra._info._video._no_half != true);
            }
            else
            {
                bool half = (frame._extra._info._video._no_half != true) &&
                            ((src_rect.Height << 1) < src_rect.Width); // ex. 2CIF
                if (half)
                {
                    src_rect.Height <<= 1;
                }

                // fit to screen(aspect ratio)
                src_rect = constant_ratio.imp(ref src_rect, _rect.Width, _rect.Height);

                if (half)
                {
                    src_rect.Y >>= 1;
                    src_rect.Height >>= 1;
                }
            }

            bool clear = false;

            _rect_source = src_rect;

            if (_image_resolution != res)
            {
                _image_resolution = res;
                clear = true;
            }
            if (_rect_output != dst_rect)
            {
                _rect_output = dst_rect;
                clear = true;
            }

            if (clear)
            {
                Graphics.FromImage(_surf_image).Clear(Color.Black);
            }

            set_spot(frame.spot);
            _parent._method_disp_image(_parent.buf_image_disp(res), _surf_image, ref dst_rect, ref src_rect, image, width, height, true);
        }

        public RectangleF imp_disp_string_to_layer(Bitmap buf, Font font, string s)
        {
            RectangleF res = new RectangleF();
            using (Graphics g = Graphics.FromImage(buf))
            {
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.Clear(Color.Transparent);
                SolidBrush br = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
                GraphicsUnit u = GraphicsUnit.Pixel;
                RectangleF rc = buf.GetBounds(ref u);
                rc.Inflate(-1, -1);
                RectangleF r = rc;
                r.Offset(-1, -1); g.DrawString(s, font, br, r);
                r.Offset(1, 0); g.DrawString(s, font, br, r);
                r.Offset(1, 0); g.DrawString(s, font, br, r);
                r.Offset(0, 1); g.DrawString(s, font, br, r);
                r.Offset(0, 1); g.DrawString(s, font, br, r);
                r.Offset(-1, 0); g.DrawString(s, font, br, r);
                r.Offset(-1, 0); g.DrawString(s, font, br, r);
                r.Offset(0, -1); g.DrawString(s, font, br, r);
                br.Color = Color.White;
                g.DrawString(s, font, br, rc);
                res.Size = g.MeasureString(s, font, rc.Size);
                res.Width += 1;
                res.Height += 1;
            }
            return res;
        }

        public static Bitmap s_ci;
        public screen_pane _parent;
        public int _num;
        public int _channelext;
        public int _stream_id;
        public MODE _mode;
        public STATUS _status;
        public Rectangle _rect;
        public Bitmap _surf_scene;
        public Bitmap _surf_image;
        public Bitmap _surf_layer_title;
        public Bitmap _surf_layer_datetime;
        public Bitmap _surf_layer_si_elevator;
        public RectangleF _rect_layer_title;
        public RectangleF _rect_layer_datetime;
        public RectangleF _rect_layer_si_elevator;
        public Size _image_resolution;
        public Rectangle _rect_output;
        public Rectangle _rect_source;
        public G2SPOT _spot;
        public last_image_type _last_image;
        public probe_frame _probe;
        public string _title;
        public string _title_pre;
        public string _str_date;
        public string _str_time;
        public string _str_datetime_pre;
        public string _str_si_elevator_pre;
        public bool _stub;
        public bool _visible;
        public bool _selected;
        public bool _disp;
        public bool _changed_spot;
        public bool _changed_time;
        public bool _audio_enable;
        public G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION _content_analytics_face_detection;
        public G2CODEC_INFO_SI_ELEVATOR_STATUS _si_elevator_status;

        public G2CHANNEL_STREAM channel_stream { get { return new G2CHANNEL_STREAM(_channelext, _stream_id); } }
    }
}
