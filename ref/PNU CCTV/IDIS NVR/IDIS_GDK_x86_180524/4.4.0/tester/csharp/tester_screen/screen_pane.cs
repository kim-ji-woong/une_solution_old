using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class screen_pane : UserControl
    {
        public screen_pane()
        {
            InitializeComponent();

            this.BackColor = screen_options.color.pane.back;
            this.options = new screen_options();
            this.option_use_scaler_G2 = true;
            this._message = new screen_message(this);
            this._pane_selected = -1;
            this._decoder = new g2decoder();
            this._looper = new g2looper();
            //this._search_audio = new sound_looper();
            this._buf_manager = new frame_buf_manager();
            this._buf_image_disp_lock = new object();
        }
        ~screen_pane()
        {
            cleanup();
        }

        public void create(int connections, int panes)
        {
            _message.Owner = this.ParentForm;
            _screen = (PictureBox)SCREEN_FRAME;
            _graphics = _screen.CreateGraphics();
            _pane_count = panes;
            _pane_selected = -1;
            _format = new screen_format(panes);
            _panes = new camera_pane[_pane_count * 2];

            for (int i = 0; i < _panes.Length; ++i)
            {
                camera_pane cp = new camera_pane(this, i);
                cp._stub = (i >= panes);
                _panes[i] = cp;
            }

            _buf_manager.create(connections, this);
            _decoder.startup(_pane_count);
            _looper.create(looper_proc);
            _looper.name = "GDK_tester::screen_looper";
            //_search_audio.create(_buf_manager);
        }
        public void cleanup()
        {
            _message.hide();
            //_search_audio.cleanup();
            _looper.cleanup();
        }

        public void buf_setup(int channel, int stremas, frame_buf.TYPE type)
        {
            _buf_manager.setup(channel, stremas, type);
        }
        public void buf_reset(int channel)
        {
            _buf_manager.remove(channel);
        }
        public void put_frame(ref G2FRAME frame, int channel, int pane)
        {
            if (frame.type == G2FRAME.TYPE.AUDIO)
            {
                if (is_format1x1() != true) return;
                if (is_audio_enable(_pane_selected) != true) return;
                if (have_CONFIG_SOUND != true) return;
            }

            _buf_manager.push(ref frame, channel, pane);
        }

        public void set_listener(screen_listener listener)
        {
            _listener = listener;
        }

        public void set_format(screen_format.FORMAT format, bool update)
        {
            set_format_imp(format, screen_format.CHANGED.LAYOUT, update);
        }

        public void set_format2(screen_format.FORMAT format, bool update, int paneIndex)
        {
            set_format_imp2(format, screen_format.CHANGED.LAYOUT, update, paneIndex);
        }

        public void set_format(int panes, bool update)
        {
            screen_format.FORMAT format = _format.format_for_panes(panes);
            set_format(format, update);
        }
        public void set_format_prev(bool update)
        {
            set_format_imp(_format._format, screen_format.CHANGED.PREV, update);
        }
        public void set_format_next(bool update)
        {
            set_format_imp(_format._format, screen_format.CHANGED.NEXT, update);
        }
        public bool set_format_range(int[] panes, bool update)
        {
            int begin = int.MaxValue;
            int end = int.MinValue;
            foreach (int pane in panes)
            {
                if (begin > pane) begin = pane;
                if (end < pane) end = pane;
            }

            if (valid_pane(begin))
            {
                screen_format.FORMAT format = _format.format_for_range(begin, end);
                set_pane_select(begin, false);
                set_format(format, update);
                return true;
            }
            return false;
        }

        protected void set_format_imp(screen_format.FORMAT format, screen_format.CHANGED mode, bool update)
        {
            screen_format.FORMAT pre_format = _format._format;
            Rectangle pre_rect = _format._rect;
            int pre_start = _format._start;
            int select = _pane_selected;
            int start = _format.calc_start_camera(format == screen_format.FORMAT.LAYOUT1X1 ? screen_format.FORMAT.LAYOUT2X2 : format, mode, _pane_selected, out select);
            //int start = _format.calc_start_camera(format, mode, _pane_selected, out select);

            if (pre_format == format &&
                pre_start == start &&
                pre_rect == _screen.ClientRectangle)
            {
                return;
            }

            if (format != pre_format)
            {
                _format_pre = pre_format;
            }

            Rectangle[] rects;
            _format.rects_for_format(format, _screen.ClientRectangle, start, out rects);

            lock (this)
            {
                foreach (camera_pane cp in _panes)
                {
                    cp._visible = false;
                }

                for (int i = 0; i < rects.Length; ++i)
                {
                    int pane = i + start;
                    if (contains_pane(pane))
                    {
                        Rectangle r = rects[i];
                        camera_pane cp = _panes[pane];
                        cp.set_rect(ref r);
                        if (r.IsEmpty != true)
                        {
                            cp._visible = true;
                        }
                    }
                }

                set_disp();

                foreach (camera_pane cp in _panes)
                {
                    if (cp._visible != true)
                    {
                        cp.set_spot(G2SPOT.INVALID_SPOT);
                    }
                }
            }

            if (format != screen_format.FORMAT.LAYOUT1X1)
            {
                search_audio_stop();
            }
            else
            {
                if (mode != screen_format.CHANGED.LAYOUT)
                {
                    search_audio_stop();
                }
            }

            if (_listener != null)
            {
                _listener.on_screen_changed_format(format, mode);
            }

            set_pane_select(select, false);

            if (update)
            {
                this.update();
            }
        }

        protected void set_format_imp2(screen_format.FORMAT format, screen_format.CHANGED mode, bool update, int paneIndex)
        {
            screen_format.FORMAT pre_format = _format._format;
            Rectangle pre_rect = _format._rect;
            int pre_start = _format._start;
            int select = _pane_selected;
            int start = _format.calc_start_camera(format == screen_format.FORMAT.LAYOUT1X1 ? screen_format.FORMAT.LAYOUT2X2 : format, mode, _pane_selected, out select);
            //int start = _format.calc_start_camera(format, mode, _pane_selected, out select);

            if (pre_format == format &&
                pre_start == start &&
                pre_rect == _screen.ClientRectangle)
            {
                return;
            }

            if (format != pre_format)
            {
                _format_pre = pre_format;
            }

            Rectangle[] rects;
            _format.rects_for_format(format, _screen.ClientRectangle, start, out rects);

            lock (this)
            {
                foreach (camera_pane cp in _panes)
                {
                    cp._visible = false;
                }

                //for (int i = 0; i < rects.Length; ++i)
                {
                    int pane = paneIndex;//i + start;
                    if (contains_pane(pane))
                    {
                        Rectangle r = rects[0];
                        //Rectangle r = rects[i];
                        camera_pane cp = _panes[pane];
                        cp.set_rect(ref r);
                        if (r.IsEmpty != true)
                        {
                            cp._visible = true;
                        }
                    }
                }

                set_disp();

                foreach (camera_pane cp in _panes)
                {
                    if (cp._visible != true)
                    {
                        cp.set_spot(G2SPOT.INVALID_SPOT);
                    }
                }
            }

            if (format != screen_format.FORMAT.LAYOUT1X1)
            {
                search_audio_stop();
            }
            else
            {
                if (mode != screen_format.CHANGED.LAYOUT)
                {
                    search_audio_stop();
                }
            }

            if (_listener != null)
            {
                _listener.on_screen_changed_format(format, mode);
            }

            set_pane_select(select, false);

            if (update)
            {
                this.update();
            }
        }

        public void set_pane_mode(int pane, camera_pane.MODE mode)
        {
            camera_pane cp = get_pane(pane);
            if (cp != null)
            {
                cp.set_mode(mode);
            }
        }
        public void set_pane_mode(int[] panes, camera_pane.MODE mode)
        {
            foreach (int pane in panes)
            {
                set_pane_mode(pane, mode);
            }
        }
        public void set_pane_select(int pane)
        {
            set_pane_select(pane, true);
        }
        public void set_pane_select(int pane, bool update)
        {
            bool noti = false;
            lock (this)
            {
                if (_pane_selected != pane)
                {
                    noti = true;
                    int pane_pre = _pane_selected;

                    camera_pane cp = get_pane(pane_pre);
                    if (cp != null)
                    {
                        cp._disp = true;
                        cp._selected = false;
                    }

                    cp = get_pane(pane);
                    if (cp != null)
                    {
                        cp._selected = true;
                        cp._disp = true;
                    }

                    _pane_selected = pane;
                }
            }

            if (update)
            {
                render();
                present(_graphics);
            }

            if (noti)
            {
                if (_listener != null)
                {
                    _listener.on_screen_changed_pane(_pane_selected);
                }
            }
        }
        public void set_pane_status(int pane, camera_pane.STATUS status, bool update)
        {
            if (valid_pane(pane))
            {
                bool changed = false;
                lock (this)
                {
                    camera_pane cp = _panes[pane];
                    if (cp._status != status)
                    {
                        cp._status = status;
                        cp._disp = true;
                        changed = true;
                    }
                }
                if (changed &&
                    update)
                {
                    this.update(pane);
                }
            }
        }
        public void set_pane_status(int[] panes, camera_pane.STATUS status, bool update)
        {
            foreach (int pane in panes)
            {
                set_pane_status(pane, status, false);
            }

            if (update)
            {
                this.update();
            }
        }
        public void set_pane_title(int pane, string title, bool update)
        {
            camera_pane cp = get_pane(pane);
            if (cp == null) return;

            bool changed = false;
            lock (this)
            {
                if (cp._title != title)
                {
                    cp._title = title;
                    cp._disp = true;
                    changed = true;
                }
            }
            if (changed &&
                update)
            {
                this.update(pane);
            }
        }
        public void set_pane_audio_enable(int pane, bool enable)
        {
            camera_pane cp = get_pane(pane);
            if (cp != null)
            {
                cp._audio_enable = enable;
            }
        }
        public void set_pane_audio_enable(bool enable)
        {
            lock (this)
            {
                foreach (camera_pane cp in _panes)
                {
                    cp._audio_enable = enable;
                }
            }
        }
        public void set_disp()
        {
            lock (this)
            {
                foreach (camera_pane cp in _panes)
                {
                    if (cp._visible) cp._disp = true;
                }
            }
        }
        public void set_disp(int pane)
        {
            lock (this)
            {
                _panes[pane]._disp = true;
            }
        }
        public void set_play_speed(int channel, int speed)
        {
            if (speed != 10)
            {
                search_audio_stop();
            }

            _buf_manager.set_speed(channel, speed);
        }
        public void set_probe_frame(int pane, ref G2FRAME frame, ref G2DECODER_VIDEO_RESULT dec)
        {
            camera_pane cp = get_pane(pane);
            if (cp != null)
            {
                lock (cp._probe)
                {
                    cp._probe.set(ref frame, ref dec);
                }
            }
        }

        public void search_play_end(int channel)
        {
            G2FRAME frame = new G2FRAME();
            put_frame(ref frame, channel, (int)frame_element.LOADED.END_OF);
        }
        public void search_stop_enter(int channel)
        {
            frame_buf buf = _buf_manager.get(channel);
            if (buf != null) buf.stop_enter();

            _looper.suspend();

            while (_looper.is_suspending)
            {
                g2looper.sleep(0);
            }

            search_audio_stop();

            if (buf != null) buf.remove();
        }
        public void search_stop_leave(int channel)
        {
            G2SPOT spot;
            int pane = get_last_disp_spot(out spot);

            frame_buf buf = _buf_manager.get(channel);
            if (buf != null)
            {
                buf.remove();
                buf.stop_leave();
            }

            if (_listener != null)
            {
                _listener.on_screen_search_stopped(channel, pane, spot);
            }
        }
        public void search_shutdown(int channel)
        {
            frame_buf buf = _buf_manager.get(channel);
            if (buf != null)
            {
                buf.remove();
            }
        }
        public void search_audio_stop()
        {
            //if (_search_audio.is_running())
            //{
            //    _search_audio.stop(true);
            //}

            _buf_manager.reset_audio();
            _buf_manager.set_audio_play(false);
        }
        public void search_audio_play(frame_element element)
        {
            if (is_format1x1() != true) return;
            //if (_search_audio.is_suspended() != true) return;
            if (element.valid != true) return;
            if (element.pane != _pane_selected) return;

            frame_element audio = null;
            if (_buf_manager.top_audio(out audio) < 0 ||
               (audio == null || audio.valid != true)) return;

            bool faster = false;
            if (audio.spot._segment == element.spot._segment)
            {
                if (audio.spot._time > element.spot._time)
                {
                    faster = true;
                }
            }
            else if (audio.spot > element.spot)
            {
                faster = true;
            }

            if (faster)
            {

            }
            else
            {
                //_search_audio.play();
            }
        }

        public void fire_changed_pane(bool post)
        {
            if (post)
            {
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    if (_listener != null)
                    {
                        _listener.on_screen_changed_pane(_pane_selected);
                    }
                });
            }
            else
            {
                if (_listener != null)
                {
                    _listener.on_screen_changed_pane(_pane_selected);
                }
            }
        }

        public void video_play(ref frame_element element)
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
        public void display_frame(int pane, ref frame_element element, byte[] image, int width, int height)
        {
            if (valid_pane(pane))
            {
                bool noti = false;

                lock (this)
                {
                    camera_pane cp = _panes[pane];
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
        public void display_frame_error(int pane, ref G2FRAME frame)
        {
            if (valid_pane(pane))
            {
                bool noti = false;

                lock (this)
                {
                    camera_pane cp = _panes[pane];
                    if (cp._visible)
                    {
                        cp._disp = false;
                        cp.set_spot(frame.spot);
                        cp.render();
                        cp.present(_graphics);

                        noti = (cp._mode == camera_pane.MODE.PLAY);
                    }
                }

                if (noti)
                {
                    if (_listener != null)
                    {
                        _listener.on_screen_image_disp(pane, ref frame);
                    }
                }
            }
        }
        public void reset(bool update)
        {
            lock (this)
            {
                foreach (camera_pane cp in _panes)
                {
                    cp.reset();
                }
            }

            lock (_decoder)
            {
                _decoder.close();
            }

            lock (_buf_image_disp_lock)
            {
                if (_buf_image_disp != null)
                {
                    _buf_image_disp.Dispose();
                    _buf_image_disp = null;
                }
            }

            if (update)
            {
                this.update();
            }
        }
        public void reset(int pane, bool update)
        {
            if (contains_pane(pane))
            {
                lock (this)
                {
                    camera_pane cp = _panes[pane];
                    cp.reset();
                }

                if (update)
                {
                    this.update(pane);
                }

                lock (_decoder)
                {
                    _decoder.close(pane);
                }
            }
        }
        public void update(int pane)
        {
            if (contains_pane(pane))
            {
                render(pane);
                present(_graphics, pane);
            }
        }
        public void update()
        {
            render();
            present(_graphics);
        }
        public void render(int pane)
        {
            camera_pane cp = get_pane(pane);
            if (cp != null)
            {
                lock (this)
                {
                    if (cp._visible &&
                        cp._disp)
                    {
                        cp._disp = false;
                        cp.disp_last_image();
                        cp.render();
                    }
                }
            }
        }
        public void render()
        {
            lock (this)
            {
                foreach (camera_pane cp in _panes)
                {
                    if (cp._visible &&
                        cp._disp)
                    {
                        cp._disp = false;
                        cp.disp_last_image();
                        cp.render();
                    }
                }
            }
        }
        public void present(Graphics g, int pane)
        {
            if (contains_pane(pane))
            {
                lock (this)
                {
                    camera_pane cp = _panes[pane];
                    if (cp._visible)
                    {
                        cp.present(g);
                    }
                }
            }
        }
        public void present(Graphics g)
        {
            lock (this)
            {
                foreach (camera_pane cp in _panes)
                {
                    if (cp._visible) cp.present(g);
                }
            }
        }

        public void clear_last_image(int[] panes, bool image_only)
        {
            foreach (int pane in panes)
            {
                camera_pane cp = get_pane(pane);
                if (cp != null)
                {
                    cp.set_spot(G2SPOT.INVALID_SPOT);

                    if (image_only)
                    {
                        cp._last_image.reset_image();
                    }
                    else
                    {
                        cp._last_image.reset();
                    }
                }
            }
        }
        public void clear_pane(int[] panes, bool update)
        {
            lock (this)
            {
                foreach (int pane in panes)
                {
                    camera_pane cp = get_pane(pane);
                    if (cp != null)
                    {
                        if (cp._visible)
                        {
                            cp.clear();
                            cp._disp = true;
                        }
                    }
                }
            }

            if (update)
            {
                this.update();
            }
        }

        public camera_pane get_pane(int pane)
        {
            if (contains_pane(pane))
            {
                if (_panes != null)
                {
                    return _panes[pane];
                }
            }
            return null;
        }
        public int get_pane_channelext(int pane)
        {
            return (valid_pane(pane)) ? _panes[pane]._channelext : -1;
        }
        public int get_pane_stream_id(int pane)
        {
            return (valid_pane(pane)) ? _panes[pane]._stream_id : -1;
        }
        public G2CHANNEL_STREAM get_pane_channel_stream(int pane)
        {
            return (valid_pane(pane)) ? _panes[pane].channel_stream : G2CHANNEL_STREAM.INVALID_CHANNEL_STREAM;
        }
        public camera_pane.STATUS get_pane_status(int pane)
        {
            return (valid_pane(pane)) ? _panes[pane]._status : camera_pane.STATUS.UNDEFINED;
        }
        public g2channel_set get_pane_visible()
        {
            g2channel_set panes = new g2channel_set();
            lock (this)
            {
                foreach (camera_pane cp in _panes)
                {
                    if (cp._visible)
                    {
                        panes.insert(cp._num);
                    }
                }
            }
            return panes;
        }
        public camera_pane.probe_frame get_pane_probe(int pane)
        {
            return valid_pane(pane) ? _panes[pane]._probe : null;
        }
        public int get_last_disp_spot(out G2SPOT spot)
        {
            spot = new G2SPOT();

            lock (this)
            {
                long pts = -1;
                int pane = -1;
                int i = 0;
                foreach (camera_pane cp in _panes)
                {
                    if (cp._last_image.pts != 0)
                    {
                        if (pts == -1 ||
                            pts < cp._last_image.pts)
                        {
                            pts = cp._last_image.pts;
                            pane = i;
                        }
                    }
                    i++;
                }

                if (valid_pane(pane))
                {
                    spot = _panes[pane]._last_image.frame.spot;
                }
                return pane;
            }
        }

        public screen_message message() { return _message; }
        public screen_format fomatter() { return _format; }
        public frame_buf_manager buf_manager() { return _buf_manager; }
        public g2decoder decoder() { return _decoder; }
        public sound_looper search_audio() { return _search_audio; }
        public int selected_pane() { return _pane_selected; }
        public int selected_pane_channelext() { return get_pane_channelext(_pane_selected); }
        public int selected_pane_stream_id() { return get_pane_stream_id(_pane_selected); }
        public bool valid_pane(int pane) { return (pane >= 0 && pane < _pane_count); }
        public bool contains_pane(int pane) { return pane >= 0 && pane < _panes.Length; }
        public bool is_visible(int pane) { return contains_pane(pane) ? _panes[pane]._visible : false; }
        public bool is_enable(int pane) { return contains_pane(pane) ? _panes[pane].is_enable() : false; }
        public bool is_format1x1() { return _format.is_format1x1(); }
        public bool is_audio_enable(int pane) { return valid_pane(pane) ? _panes[pane]._audio_enable : false; }

        protected PictureBox _screen;
        protected Graphics _graphics;
        protected screen_message _message;
        protected screen_listener _listener;
        protected g2decoder _decoder;
        protected frame_buf_manager _buf_manager;
        protected screen_format _format;
        protected screen_format.FORMAT _format_pre;
        protected camera_pane[] _panes;
        protected int _pane_count;
        protected int _pane_selected;
        protected byte[] _buf_decoded;
        protected byte[] _buf_filter;
        public Bitmap _buf_image_disp;
        public object _buf_image_disp_lock;
        public screen_method.fun_disp_image _method_disp_image;
        public long _pts;

        public screen_options options;
        public bool option_use_probe_performance
        {
            get { return options.probe_perf; }
            set { options.probe_perf = value; }
        }
        public bool option_use_scaler_G2
        {
            set
            {
                if (value)
                {
                    _method_disp_image = new screen_method.fun_disp_image(screen_method.imp_disp_image_G2);
                }
                else
                {
                    _method_disp_image = new screen_method.fun_disp_image(screen_method.imp_disp_image_GDI);
                }
            }
        }
        public bool have_CONFIG_SOUND
        {
#if _CONFIG_SLIMDX_SOUND || _CONFIG_DIRECT_SOUND
            get { return true; }
#else
            get { return false; }
#endif
        }
        public Bitmap buf_image_disp(Size size)
        {
            lock (_buf_image_disp_lock)
            {
                if (_buf_image_disp == null)
                {
                    _buf_image_disp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppRgb);
                }
                else
                {
                    if (_buf_image_disp.Width < size.Width ||
                        _buf_image_disp.Height < size.Height)
                    {
                        _buf_image_disp = new Bitmap(Math.Max(_buf_image_disp.Width, size.Width), Math.Max(_buf_image_disp.Height, size.Height), PixelFormat.Format32bppRgb);
                    }
                }
                return _buf_image_disp;
            }
        }
    }
}
