using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    public partial class form_export_clip_play : Form, g2play_saver_listener
    {
        public enum CANCEL_STATUS
        {
            NOT_CANCELED = 0,
            CANCELED = 1,
            SEND_CANCELED = 2
        }

        public form_export_clip_play()
        {
            InitializeComponent();

            this._channel = -1;
            this._timer_cancel = new Timer();
            this._timer_cancel.Interval = 20 * 1000;
            this._timer_cancel.Tick += new EventHandler(on_timer);
            this._timer_measure_size = new Timer();
            this._timer_measure_size.Interval = 250;
            this._timer_measure_size.Tick += new EventHandler(on_timer);
            this._filename = "";
            this._filedest = "";
            this._cancel_status = CANCEL_STATUS.NOT_CANCELED;
            this._started = false;
            this._buf = new byte[64 * 1024];

            this.EDT_PASSWORD.MaxLength = 16;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            Array.Resize(ref _buf, 0);
            _buf = null;
        }

        public void set_invoke_saver(G2GUID service)
        {
            _adaptor = new g2play_saver();
            _adaptor.set_listener(this);
            _adaptor.startup(1);
            G2CONNECT_RES res;
            _channel = _adaptor.connect(ref service, out res);
        }

        public void set_revoke_saver()
        {
            if (_adaptor.is_disconnectable(_channel))
            {
                _adaptor.disconnect(_channel);

                while (_channel >= 0)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }

            _adaptor.cleanup();
        }

        public void set_enable(bool enable)
        {
            CHK_FIRST.Enabled =
            CHK_LAST.Enabled = enable;
            TRV_DEVICES.Enabled =
            EDT_PASSWORD.Enabled =
            CHK_SAVE_PASSWORD.Enabled =
            CHK_INCLUDE_TEXT_IN.Enabled =
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
            DTP_FROM.Value = from;
            DTP_TO.Value = to;
        }

        public bool is_started() { return _started; }

        public void set_device(List<G2DEVICE_ROOT> root_infos, List<G2DEVICE_LEAF> leaf_infos, List<int> channelexts)
        {
            TRV_DEVICES.Nodes.Clear();
            TRV_DEVICES.AfterCheck += new TreeViewEventHandler(on_tree_device_after_check);

            for (int i = 0; i < root_infos.Count; ++i)
            {
                TRV_DEVICES.Nodes.Add(imp_build_node(root_infos[i], leaf_infos[i], channelexts[i]));
            }

            foreach (TreeNode node in TRV_DEVICES.Nodes)
            {
                node.ExpandAll();
            }
        }
        protected TreeNode imp_build_node(G2DEVICE_ROOT root_info, G2DEVICE_LEAF leaf_info, int channelext)
        {
            TreeNode node = new TreeNode(root_info._name._string);
            node.Nodes.Add(leaf_info._name._string).Tag = channelext;
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
                    e.Node.Nodes[0].Checked = e.Node.Checked;
                }
                else
                {
                    e.Node.Parent.Checked = e.Node.Checked;
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

            g2channel_set checked_cameras = new g2channel_set();
            foreach (TreeNode node in TRV_DEVICES.Nodes)
            {
                if (node.Nodes[0].Checked)
                {
                    int channelext = (int)node.Nodes[0].Tag;
                    checked_cameras.insert(channelext);
                }
            }

            if (checked_cameras.size() == 0)
            {
                MessageBox.Show(this, "There is no selected device.", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string filter = "Self-Player Files (*.exe)|*.exe||";

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

            imp_start(checked_cameras, scope);

            set_enable(false);
        }
        private void imp_start(g2channel_set cameras, G2SCOPE scope)
        {
            _channel_set = cameras;
            _cancel_status = CANCEL_STATUS.NOT_CANCELED;

            if (imp_clip_create(_filedest) == false)
            {
                on_post_canceled(false);
                MessageBox.Show(this, "Failed create file", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _adaptor.request_scope_list(_channel, ref scope._begin._time, ref scope._end._time, cameras);
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
            if (sender == _timer_measure_size)
            {
                _timer_measure_size.Stop();

                string status = "";
                if (_measured_size != 0)
                {
                    status = string.Format("estimating file size {0}", string_from_bytes((long)_measured_size));
                }
                else
                {
                    status = "estimating file size";
                }

                STC_STATUS.Text = status;

                _adaptor.request_clipcopy_size(_channel);
            }
            else if (sender == _timer_cancel)
            {
                _timer_cancel.Stop();
                post_canceled(false);
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

        private g2play_saver _adaptor;
        private int _channel;
        private Timer _timer_cancel;
        private Timer _timer_measure_size;
        private string _filename;
        private string _filedest;
        private g2channel_set _channel_set;
        private FileStream _file;
        private CANCEL_STATUS _cancel_status;
        private bool _started;
        private uint _progress;
        private ulong _measured_size;
        private byte[] _buf;


        //////////////////////////////////////////////////////////////////////////////////////////////////

        public void imp_on_receive_scope_list(int channel, G2SCOPE[] scopes)
        {
            G2SCOPE scope;

            if (scopes.Length == 0)
            {
                imp_clip_remove();
                MessageBox.Show(this, "There is no search data", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                post_canceled(true);
                return;
            }
            else if (scopes.Length == 1)
            {
                scope = scopes[0];
            }
            else
            {
                form_select_segment form = new form_select_segment();
                form.set_data_clip(scopes);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    scope = form.scope_selected;
                }
                else
                {
                    imp_clip_remove();
                    post_canceled(true);
                    return;
                }
            }

            ulong free_space = 0UL;
            ulong slice_size = uint.MaxValue;

            string dir = Path.GetDirectoryName(_filename);
            if (dir.StartsWith(@"\\"))
            {
                free_space = g2foundation.get_disk_free_space(Path.GetDirectoryName(_filename));
            }
            else
            {
                DriveInfo di = new DriveInfo(dir);
                free_space = (ulong)di.AvailableFreeSpace;
                slice_size = (di.DriveFormat == "FAT") ? uint.MaxValue / 2 : uint.MaxValue;
            }

            ulong total = (ulong)Math.Min(free_space, 64L * 1024L * 1024L * 1024L);

            if (CHK_INCLUDE_TEXT_IN.Checked)
            {
                _adaptor.request_clipcopy_text_in(channel, true);
            }

            _adaptor.request_clipcopy_measure_size(channel, _channel_set, ref scope, total);
        }
        public void imp_on_receive_clipcopy_size(int channel, G2CLIPCOPY_STATUS.TYPE status, G2CLIPCOPY_SIZE_INFO info)
        {
            if (status == G2CLIPCOPY_STATUS.TYPE.NOT_ENOUGH_SPACE)
            {
                imp_clip_remove();
                set_enable(true);
                MessageBox.Show(this, "There is not sufficient space at storage media.", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (status == G2CLIPCOPY_STATUS.TYPE.NOT_COMPLETED)
            {
                _measured_size = info.get_total_size();
                _timer_measure_size.Stop();
                _timer_measure_size.Start();
                return;
            }
            else if (status == G2CLIPCOPY_STATUS.TYPE.PARTIAL_COPIABLE)
            {
                string message = "";

                if (info._info_len > 1)
                {
                    message = "Do you want to copy to {0}?\n(File size was limited to 64GB)";
                    message = string.Format(message, _filedest);
                }
                else
                {
                    ulong free_space = g2foundation.get_disk_free_space(Path.GetDirectoryName(_filename));
                    if (free_space >= uint.MaxValue)
                    {
                        message = "Do you want to copy to {0}?\n(File size was limited to 4GB)";
                        message = string.Format(message, _filedest);
                    }
                    else
                    {
                        message = "Not enough space\nDo you want to continue using the available space?";
                    }
                }

                G2SCOPE scope = info.get_total_scope();
                string message_info = string.Format("{0} ~ {1} {2}", scope._begin._time.to_string_date_time(), scope._end._time.to_string_date_time(), string_from_bytes((long)info.get_total_size()));

                if (info._info_len > 1)
                {
                    message_info += string.Format("\nfile count : {0}", info._info_len);
                }

                if (MessageBox.Show(this, message + "\n" + message_info, "Export Clip", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    post_canceled(false);
                    return;
                }
            }
            else if (status == G2CLIPCOPY_STATUS.TYPE.FULL_COPIABLE) { }
            else if (status == G2CLIPCOPY_STATUS.TYPE.NO_RECORDED_DATA)
            {

                MessageBox.Show(this, "There is no recorded data", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                post_canceled(false);
                return;
            }

            _progress = 0;
            _measured_size = 0UL;

            STC_STATUS.Text = "";

            if (info.empty() != true)
            {
                _file.SetLength((long)info._info[0]._size);
            }

            if (CHK_SAVE_PASSWORD.Checked)
            {
                _adaptor.request_clipcopy_password(channel, EDT_PASSWORD.Text);
            }
            else
            {
                _adaptor.request_clipcopy_data(channel);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////
        #region g2play_saver_listener 멤버

        public void on_g2play_saver_connected(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2play_saver_connected] is not implemented.");
        }

        public void on_g2play_saver_disconnected(int handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            _channel = -1;
            if (reason != G2DISCONNECT_REASON.TYPE.LOGOUT)
            {
                post_canceled(false);
            }
        }

        public void on_g2play_saver_receive_record_channels(int handle, int channel, G2PLAY_CHANNEL_INFO[] channels) 
        {
            Debug.WriteLine("callback [on_g2play_saver_receive_record_channels] is not implemented.");
        }
        public void on_g2play_saver_receive_frame_data(int handle, int channel, ref G2FRAME frame)
        {
            Debug.WriteLine("callback [on_g2play_saver_receive_frame_data] is not implemented."); 
        }
        public void on_g2play_saver_receive_notify_out_of_scope(int handle, int channel, G2PLAYER.OUT_OF_SCOPE status) 
        {
            Debug.WriteLine("callback [on_g2play_saver_receive_notify_out_of_scope] is not implemented."); 
        }
        public void on_g2play_saver_receive_notify_player_error(int handle, int channel, G2PLAYER.PLAYER_ERROR error) 
        {
            Debug.WriteLine("callback [on_g2play_saver_receive_notify_player_error] is not implemented."); 
        }
        public void on_g2play_saver_receive_scope_list(int handle, int channel, G2SCOPE[] scopes)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                imp_clip_remove();
                return;
            }

            this.BeginInvoke((MethodInvoker)delegate() { imp_on_receive_scope_list(channel, scopes); });
        }

        public void on_g2play_saver_receive_no_recorded_data(int handle, int channel) 
        {
            Debug.WriteLine("callback [on_g2play_saver_receive_no_recorded_data] is not implemented.");
        }
        public void on_g2play_saver_receive_clipcopy_size(int handle, int channel, G2CLIPCOPY_STATUS.TYPE status, ref G2CLIPCOPY_SIZE_INFO csi)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor.request_clipcopy_cancel(_channel);
                return;
            }

            G2CLIPCOPY_SIZE_INFO si = new G2CLIPCOPY_SIZE_INFO();
            si = csi;
            this.BeginInvoke((MethodInvoker)delegate() { imp_on_receive_clipcopy_size(channel, status, si); });
        }

        public void on_g2play_saver_receive_clipcopy_data(int handle, int channel, ulong offset, uint size, IntPtr data, uint progress)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor.request_clipcopy_cancel(_channel);
                return;
            }

            if (_progress != progress)
            {
                _progress = progress;

                post_progress(progress);
            }

            if (_buf.Length < size)
            {
                Array.Resize(ref _buf, (int)size);
            }

            System.Runtime.InteropServices.Marshal.Copy(data, _buf, 0, (int)size);

            try
            {
                _file.Seek((long)offset, SeekOrigin.Begin);
                _file.Write(_buf, 0, (int)size);
            }
            catch (Exception e)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor.request_clipcopy_cancel(channel);

                post_message(e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void on_g2play_saver_receive_clipcopy_canceled(int handle, int channel)
        {
            post_canceled(false);
        }

        public void on_g2play_saver_receive_clipcopy_set_password(int handle, int channel, uint result)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor.request_clipcopy_cancel(_channel);
                return;
            }

            _adaptor.request_clipcopy_data(channel);
        }

        public void on_g2play_saver_receive_clipcopy_job_started(int handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor.request_clipcopy_cancel(channel);
                return;
            }

            if (job == G2CLIPCOPY_JOB.TYPE.FORMAT_STORAGE)
            {
                G2CLIPCOPY_SIZE_INFO si;
                if (_adaptor.get_clipcopy_size_info(channel, out si))
                {
                    if (num != 0 &&
                        num < si._info_len)
                    {
                        _filedest = Path.GetDirectoryName(_filename) + Path.GetFileNameWithoutExtension(_filename) + "_" + num.ToString("00") + Path.GetExtension(_filename);

                        if (imp_clip_create(_filedest))
                        {
                            _file.SetLength((long)si._info[num]._size);
                        }
                        else
                        {
                            _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                            _adaptor.request_clipcopy_cancel(channel);

                            post_message("Failed create file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            else if (job == G2CLIPCOPY_JOB.TYPE.MEASURE_SIZE)
            {
                _adaptor.request_clipcopy_size(channel);
            }
        }

        public void on_g2play_saver_receive_clipcopy_job_finished(int handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor.request_clipcopy_cancel(channel);
                return;
            }

            if (job == G2CLIPCOPY_JOB.TYPE.COPY_PLAYER)
            {
                if (num == total - 1)
                {
                    post_completed();
                }
            }
        }

        #endregion
    }
}
