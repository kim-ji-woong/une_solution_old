using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class form_export_clip : Form
    {
        public enum CANCEL_STATUS
        {
            NOT_CANCELED = 0,
            CANCELED = 1,
            SEND_CANCELED = 2
        }

        public form_export_clip(adaptor_playable adaptor, int channel)
        {
            InitializeComponent();

            this._adaptor = adaptor;
            this._adaptor_g1 = adaptor as adaptor_g1;
            this._adaptor_g2 = adaptor as adaptor_g2;
            this._channel = channel;
            this._timer_cancel = new Timer();
            this._timer_cancel.Interval = 20 * 1000;
            this._timer_cancel.Tick += new EventHandler(on_timer);
            this._timer_measure_size = new Timer();
            this._timer_measure_size.Interval = 250;
            this._timer_measure_size.Tick += new EventHandler(on_timer);
            this._channel_set = new g2channel_set();
            this._channel_set_enable = new g2channel_set();
            this._filename = "";
            this._filedest = "";
            this._cancel_status = CANCEL_STATUS.NOT_CANCELED;
            this._started = false;
            this._channel_set_enable_loaded = false;
            this._buf = new byte[64 * 1024];

            if (_adaptor_g1 != null)
            {
                this.EDT_PASSWORD.MaxLength = 15;
            }
            else
            {
                this.EDT_PASSWORD.MaxLength = 16;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            Array.Resize(ref _buf, 0);
            _buf = null;
        }

        public void set_product_info(ref G2_PRODUCT_INFO pi)
        {
            _pi = pi;
            _count_camera = (_pi.remote_search.base_channel > 0 &&
                             _pi.remote_search.stream_count > 0) ? pi.remote_search.base_channel * _pi.remote_search.stream_count :
                             _pi.device.count_camera;

            g2pair<Control, bool>[] options = new g2pair<Control, bool>[] {
                new g2pair<Control, bool>(CHK_SAVE_PASSWORD, _pi.remote_clip_copy.password_save),
                new g2pair<Control, bool>(CHK_INCLUDE_TEXT_IN, _pi.remote_clip_copy.contains_data(G2_PRODUCT_INFO_CAPS.REMOTE_CLIP_COPY.INCLUDE_DATA.TEXT_IN)),
                new g2pair<Control, bool>(CHK_INCLUDE_GPS, _pi.remote_clip_copy.contains_data(G2_PRODUCT_INFO_CAPS.REMOTE_CLIP_COPY.INCLUDE_DATA.GPS)),
                new g2pair<Control, bool>(CHK_INCLUDE_EVENT, _pi.remote_search.version == (uint)G2_PRODUCT_INFO_CAPS.REMOTE_SEARCH.VERSION.G2),
                new g2pair<Control, bool>(CHK_EXCLUDE_PLAYER, _pi.remote_clip_copy.player_exclude)
            };

            Point[] locs = new Point[options.Length];
            int pos = 0;
            for (int i = 0; i < options.Length; ++i)
            {
                locs[i] = options[i].first.Location;
            }

            foreach (g2pair<Control, bool> o in options)
            {
                o.first.Visible = o.second;
                if (o.second)
                {
                    o.first.Location = locs[pos++];
                }
            }

            imp_set_device();
        }
        public void set_enable(bool enable)
        {
            CHK_FIRST.Enabled =
            CHK_LAST.Enabled = enable;
            TRV_DEVICES.Enabled =
            EDT_PASSWORD.Enabled =
            CHK_SAVE_PASSWORD.Enabled =
            CHK_INCLUDE_TEXT_IN.Enabled =
            CHK_INCLUDE_GPS.Enabled =
            CHK_EXCLUDE_PLAYER.Enabled =
            BTN_CLOSE.Enabled = enable;

            if (CHK_FIRST.Checked != true)
            {
                DTP_FROM.Enabled = enable;
            }
            if (CHK_LAST.Checked != true)
            {
                DTP_TO.Enabled = enable;
            }

            if (enable)
            {
                BTN_START.Text = "Start";
                _started = false;
            }
            else
            {
                BTN_START.Text = "Stop";
                _started = true;
            }

            PRG_STATUS.Value = 0;
            STC_STATUS.Text = "";
        }
        public void set_time_range(G2TIME from, G2TIME to)
        {
            _range_from = from;
            _range_to = to;
        }
        public void set_cameras(g2channel_set cameras)
        {
            _cameras = cameras;
        }
        public void load()
        {
            if (_range_from.valid &&
                _range_to.valid)
            {
                DTP_FROM.Value = _range_from;
                DTP_TO.Value = _range_to;
            }

            if (_adaptor.is_support(_channel, G2SEARCH_SUPPORT.QUERY.CLIP_AVAIL_CHANNEL))
            {
                if (_adaptor_g2 != null)
                {
                    g2channel_set channelset;
                    _adaptor_g2.request_clipcopy_enable_channelset(_channel, out channelset);
                    _channel_set_enable.assign(channelset);
                    _channel_set_enable_loaded = true;
                }
                else
                {
                    _adaptor_g1.request_clipcopy_enable_channelset(_channel);
                }

                for (; _channel_set_enable_loaded != true; )
                {
                    g2looper.sleep(1);
                }
            }
            else
            {
                _channel_set_enable.assign(new g2channel_set(0, _count_camera));
            }

            TreeNodeCollection nodes = TRV_DEVICES.Nodes[0].Nodes;
            for (int i = nodes.Count - 1; i >= 0; --i)
            {
                TreeNode node = nodes[i];
                if (_channel_set_enable.contains((int)node.Tag) != true)
                {
                    node.Remove();
                }
            }

            bool check_all = true;
            foreach (TreeNode node in TRV_DEVICES.Nodes[0].Nodes)
            {
                int num = (int)node.Tag;
                node.Checked = _cameras.contains((int)node.Tag);
                if (node.Checked != true)
                {
                    check_all = false;
                }
            }
            TRV_DEVICES.Nodes[0].Checked = check_all;
        }

        public bool is_started() { return _started; }

        protected void imp_set_device()
        {
            TRV_DEVICES.Nodes.Clear();
            TRV_DEVICES.AfterCheck += new TreeViewEventHandler(on_tree_device_after_check);

            if (_count_camera > 0) TRV_DEVICES.Nodes.Add(imp_build_node("Camera", _count_camera));

            foreach (TreeNode node in TRV_DEVICES.Nodes)
            {
                node.ExpandAll();
            }
        }
        protected TreeNode imp_build_node(string name, int count)
        {
            TreeNode node = new TreeNode(name);
            for (int i = 0; i < count; ++i)
            {
                node.Nodes.Add(name + ((int)(i + 1)).ToString()).Tag = i;
            }
            return node;
        }
        protected bool imp_get_time_range(out G2SCOPE scope)
        {
            scope = new G2SCOPE();

            if (CHK_FIRST.Checked) scope._begin._time.reset();
            else scope._begin._time = DTP_FROM.Value;
            if (CHK_LAST.Checked) scope._end._time.reset();
            else scope._end._time = DTP_TO.Value;

            return (scope._begin._time.valid && scope._end._time.valid &&
                   (scope._begin._time >= scope._end._time)) ? false : true;
        }

        protected bool imp_clip_create(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch { }
            }

            if (_file != null)
            {
                _file.Close();
            }

            try
            {
                FileStream fs = File.Create(path);
                _file = fs;
                return true;
            }
            catch
            {
                return false;
            }
        }
        protected void imp_clip_remove()
        {
            if (_file != null)
            {
                _file.Close();
            }

            try
            {
                File.Delete(_filedest);
            }
            catch { }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }
        private void on_form_closing(object sender, FormClosingEventArgs e)
        {
            if (is_started())
            {
                MessageBox.Show(this, "Clip is being exported.\nRetry when clip has been exported.", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (is_started())
                {
                    e.Cancel = true;
                }
            }
        }
        private void on_chk_first(object sender, EventArgs e)
        {
            bool enable = (CHK_FIRST.Checked != true);
            DTP_FROM.Enabled = enable;
        }
        private void on_chk_last(object sender, EventArgs e)
        {
            bool enable = (CHK_LAST.Checked != true);
            DTP_TO.Enabled = enable;
        }
        private void on_dtp_mouse_wheel(object sender, MouseEventArgs e)
        {
            SendKeys.Send(e.Delta > 0 ? "{UP}" : "{DOWN}");
        }
        private void on_tree_device_after_check(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode node in e.Node.Nodes)
                    {
                        node.Checked = e.Node.Checked;
                    }
                }
            }
        }
        private void on_btn_start(object sender, EventArgs e)
        {
            if (is_started())
            {
                if (MessageBox.Show(this, "Do you want to cancel export clip?", "Export Clip", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _cancel_status = CANCEL_STATUS.CANCELED;

                    if (_timer_cancel.Enabled != true)
                    {
                        _timer_cancel.Start();
                    }
                }
                return;
            }

            _timer_cancel.Stop();
            _timer_measure_size.Stop();

            G2SCOPE scope;
            if (imp_get_time_range(out scope) != true)
            {
                MessageBox.Show(this, "Invalid recording range", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            g2channel_set channel_set = new g2channel_set();
            List<int> ordered_set = new List<int>();
            foreach (TreeNode node in TRV_DEVICES.Nodes[0].Nodes)
            {
                if (node.Checked)
                {
                    int ch = (int)node.Tag;
                    channel_set.insert(ch);
                    ordered_set.Add(ch);
                }
            }

            if (channel_set.empty())
            {
                MessageBox.Show(this, "There is no selected device.", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool exclude = CHK_EXCLUDE_PLAYER.Checked;
            string filter = "";
            if (exclude)
            {
                if (_pi.record_db.type == (byte)G2_PRODUCT_INFO_CAPS.RECORD_DB.TYPE.IBANK_3_0)
                {
                    filter = "Self-Player Files (*.cbf)|*.cbf||";
                }
                else
                {
                    filter = "Self-Player Files (*.ccf)|*.ccf||";
                }
            }
            else
            {
                filter = "Self-Player Files (*.exe)|*.exe||";
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = filter;
            dialog.OverwritePrompt = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _filename = dialog.FileName;
            }

            if (_filename.Length == 0) return;

            _filedest = _filename;
            _progress = 0;
            _measured_size = 0UL;

            PRG_STATUS.Value = 0;
            STC_STATUS.Text = "";

            if (_adaptor is adaptor_g2)
            {
                imp_start_g2(channel_set, ordered_set, scope);
            }
            else
            {
                imp_start_g1(channel_set, scope);
            }

            set_enable(false);
        }
        private void on_btn_close(object sender, EventArgs e)
        {

        }
        private void on_chk_save_password(object sender, EventArgs e)
        {
            EDT_PASSWORD.Visible = CHK_SAVE_PASSWORD.Checked;
        }
        private void on_timer(object sender, EventArgs e)
        {
            if (_adaptor is adaptor_g2)
            {
                on_timer_g2(sender, e);
            }
            else
            {
                on_timer_g1(sender, e);
            }
        }

        public void post_completed()
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_completed(); });
        }
        public void post_canceled(bool no_message)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_canceled(no_message); });
        }
        public void post_progress(uint progress)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_progress(progress); });
        }
        public void post_message(string message, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_message(message, buttons, icon); });
        }

        private void on_post_completed()
        {
            if (_file != null)
            {
                _file.Close();
            }

            set_enable(true);
            MessageBox.Show(this, "Exporting completed", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void on_post_canceled(bool no_message)
        {
            if (_file != null)
            {
                _file.Close();
            }

            try
            {
                File.Delete(_filedest);
            }
            catch { }

            if (_timer_cancel.Enabled)
            {
                _timer_cancel.Stop();
            }
            if (_timer_measure_size.Enabled)
            {
                _timer_measure_size.Stop();
            }

            set_enable(true);

            if (no_message != true)
            {
                MessageBox.Show(this, "Exporting clip is canceled", "Export clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void on_post_progress(uint progress)
        {
            PRG_STATUS.Value = (int)progress;
        }
        private void on_post_message(string message, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(this, message, "Export Clip", buttons, icon);
        }

        public static string string_from_bytes(long value)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (value == 0) return "0" + suf[0];
            long bytes = Math.Abs(value);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(value) * num).ToString() + suf[place];
        }

        private G2_PRODUCT_INFO _pi;
        private G2TIME _range_from;
        private G2TIME _range_to;
        private adaptor_playable _adaptor;
        private adaptor_g1 _adaptor_g1;
        private adaptor_g2 _adaptor_g2;
        private int _channel;
        private int _count_camera;
        private Timer _timer_cancel;
        private Timer _timer_measure_size;
        private g2channel_set _channel_set;
        private g2channel_set _channel_set_enable;
        private g2channel_set _cameras;
        private List<int> _ordered_set;
        private string _filename;
        private string _filedest;
        private FileStream _file;
        private CANCEL_STATUS _cancel_status;
        private bool _started;
        private bool _channel_set_enable_loaded;
        private uint _progress;
        private ulong _measured_size;
        private byte[] _buf;
    }
}
