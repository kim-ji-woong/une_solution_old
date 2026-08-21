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
    public partial class form_event_search_IDR : Form
    {
        public form_event_search_IDR(G2EVENT_QUERY_CONDITION options, g2channel_set cameras)
        {
            InitializeComponent();

            this._options = options;
            this._cameras = cameras;
        }

        public void set_product_info(ref G2_PRODUCT_INFO pi)
        {
            _pi = pi;
            _count_camera = _pi.device.count_camera;

            imp_set_device();
        }
        public void set_time_range(G2TIME from, G2TIME to)
        {
            _range_from = from;
            _range_to = to;
        }
        public void load()
        {
            imp_load();

            if (CHK_FIRST.Checked &&
                CHK_LAST.Checked)
            {
                if (_range_from.valid &&
                    _range_to.valid)
                {
                    DTP_FROM.Value = _range_from;
                    DTP_TO.Value = _range_to;
                }
            }
        }

        protected void imp_load()
        {
            imp_set_dwell_time(SLD_ALARM_IN, _options._dwell._alarm_in);
            imp_set_dwell_time(SLD_MOTION, _options._dwell._motion);
            imp_set_dwell_time(SLD_OBJECT, _options._dwell._object);
            imp_set_dwell_time(SLD_VIDEO_LOSS, _options._dwell._video_loss);
            imp_set_dwell_time(SLD_VIDEO_ANALYTICS, _options._dwell._video_analytics);

            CHK_ALARM_IN.Checked = (_options._alarm_in != 0);
            CHK_MOTION.Checked = (_options._motion != 0);
            CHK_OBJECT.Checked = (_options._object != 0);
            CHK_VIDEO_LOSS.Checked = (_options._video_loss != 0);
            CHK_VIDEO_ANALYTICS.Checked = (_options._video_analytics != 0);

            bool check_all = true;
            foreach (TreeNode node in TRV_DEVICES.Nodes[0].Nodes)
            {
                node.Checked = _cameras.contains((int)node.Tag);
                if (node.Checked != true)
                {
                    check_all = false;
                }
            }
            TRV_DEVICES.Nodes[0].Checked = check_all;

            if (_options._begin.valid) DTP_FROM.Value = _options._begin;
            if (_options._end.valid) DTP_TO.Value = _options._end;

            CHK_FIRST.Checked = (_options._begin.valid != true);
            DTP_FROM.Enabled = CHK_FIRST.Checked != true;
            CHK_LAST.Checked = (_options._end.valid != true);
            DTP_TO.Enabled = CHK_LAST.Checked != true;
        }
        protected bool imp_save()
        {
            g2channel_set cameras = new g2channel_set();
            foreach (TreeNode node in TRV_DEVICES.Nodes[0].Nodes)
            {
                if (node.Checked)
                {
                    cameras.insert((int)node.Tag);
                }
            }

            if (cameras.empty())
            {
                MessageBox.Show(this, "There is no selected device.", "Event Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            uint chs = cameras.to_uint32();

            _cameras.assign(cameras);
            _options._dwell._alarm_in = SLD_ALARM_IN.Value;
            _options._dwell._motion = SLD_MOTION.Value;
            _options._dwell._object = SLD_OBJECT.Value;
            _options._dwell._video_loss = SLD_VIDEO_LOSS.Value;
            _options._dwell._video_analytics = SLD_VIDEO_ANALYTICS.Value;
            _options._alarm_in = CHK_ALARM_IN.Checked ? chs : 0;
            _options._motion = CHK_MOTION.Checked ? chs : 0;
            _options._object = CHK_OBJECT.Checked ? chs : 0;
            _options._video_loss = CHK_VIDEO_LOSS.Checked ? chs : 0;
            _options._video_analytics = CHK_VIDEO_ANALYTICS.Checked ? chs : 0;

            return true;
        }

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
        protected void imp_set_dwell_time(object sender, int time)
        {
            int min = time / 60;
            int sec = time % 60;
            string val = string.Format("{0}:{1}", min.ToString("00"), sec.ToString("00"));

            if (sender == SLD_ALARM_IN) STC_DWELL_ALARM_IN.Text = val;
            if (sender == SLD_MOTION) STC_DWELL_MOTION.Text = val;
            if (sender == SLD_OBJECT) STC_DWELL_OBJECT.Text = val;
            if (sender == SLD_VIDEO_LOSS) STC_DWELL_VIDEO_LOSS.Text = val;
            if (sender == SLD_VIDEO_ANALYTICS) STC_DWELL_VIDEO_ANALYTICS.Text = val;
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
        private void on_dwell_time_value_changed(object sender, EventArgs e)
        {
            TrackBar c = sender as TrackBar;
            imp_set_dwell_time(sender, c.Value);
        }
        private void on_dwell_time_value_reset(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TrackBar c = sender as TrackBar;
                c.Value = 10;
            }
        }
        private void on_btn_OK(object sender, EventArgs e)
        {
            if (CHK_FIRST.Checked) _options._begin.reset();
            else _options._begin = DTP_FROM.Value;
            if (CHK_LAST.Checked) _options._end.reset();
            else _options._end = DTP_TO.Value;

            bool valid_range = (_options._begin.valid && _options._end.valid &&
                               (_options._begin >= _options._end)) ? false : true;
            if (valid_range != true)
            {
                MessageBox.Show(this, "Invalid recording range", "Event search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (imp_save())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private G2_PRODUCT_INFO _pi;
        private G2TIME _range_from;
        private G2TIME _range_to;
        private g2channel_set _cameras;
        private int _count_camera;
        public G2EVENT_QUERY_CONDITION _options;
    }
}
