using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    using G2HSEARCH_G2 = System.Int32;

    public partial class form_export_clip : g2search_g2_listener_saver
    {
        public void on_g2search_g2_saver_connected(G2HSEARCH_G2 handle, int channel) { }
        public void on_g2search_g2_saver_disconnected(G2HSEARCH_G2 handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            post_canceled(false);
        }
        public void on_g2search_g2_saver_receive_frame_data(G2HSEARCH_G2 handle, int channel, ref G2FRAME frame) { }
        public void on_g2search_g2_saver_receive_notify_out_of_scope(G2HSEARCH_G2 handle, int channel, G2PLAYER.OUT_OF_SCOPE status) { }
        public void on_g2search_g2_saver_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, int channel, ref G2ROLLBACK_INFO rbi) { }
        public void on_g2search_g2_saver_receive_notify_player_error(G2HSEARCH_G2 handle, int channel, G2PLAYER.PLAYER_ERROR error) { }
        public void on_g2search_g2_saver_receive_scope_list(G2HSEARCH_G2 handle, int channel, G2SCOPE[] scopes, int type)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                imp_clip_remove();
                return;
            }

            this.BeginInvoke((MethodInvoker)delegate() { imp_g2_on_receive_scope_list(channel, scopes); });
        }
        public void on_g2search_g2_saver_receive_no_recorded_data(G2HSEARCH_G2 handle, int channel) { }
        public void on_g2search_g2_saver_receive_clipcopy_size(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_STATUS.TYPE status, ref G2CLIPCOPY_SIZE_INFO csi, uint progress)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor_g2.request_clipcopy_cancel(_channel);
                return;
            }

            G2CLIPCOPY_SIZE_INFO si = new G2CLIPCOPY_SIZE_INFO();
            si = csi;
            this.BeginInvoke((MethodInvoker)delegate() { imp_g2_on_receive_clipcopy_size(channel, status, si, progress); });
        }
        public void on_g2search_g2_saver_receive_clipcopy_data(G2HSEARCH_G2 handle, int channel, ulong offset, uint size, IntPtr data, uint progress)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor_g2.request_clipcopy_cancel(_channel);
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

            Marshal.Copy(data, _buf, 0, (int)size);

            try
            {
                _file.Seek((long)offset, SeekOrigin.Begin);
                _file.Write(_buf, 0, (int)size);
            }
            catch (Exception e)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor_g2.request_clipcopy_cancel(channel);

                post_message(e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void on_g2search_g2_saver_receive_clipcopy_set_password(G2HSEARCH_G2 handle, int channel, uint result)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor_g2.request_clipcopy_cancel(_channel);
                return;
            }

            _adaptor_g2.request_clipcopy_data(channel);
        }
        public void on_g2search_g2_saver_receive_clipcopy_canceled(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_ERROR.TYPE error)
        {
            post_canceled(false);
        }
        public void on_g2search_g2_saver_receive_clipcopy_job_started(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor_g2.request_clipcopy_cancel(_channel);
                return;
            }

            if (job == G2CLIPCOPY_JOB.TYPE.FORMAT_STORAGE)
            {
                G2CLIPCOPY_SIZE_INFO si;
                if (_adaptor_g2.get_clipcopy_size_info(channel, out si))
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
                            _adaptor_g2.request_clipcopy_cancel(channel);

                            post_message("Failed create file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            else if (job == G2CLIPCOPY_JOB.TYPE.MEASURE_SIZE)
            {
                _adaptor_g2.request_clipcopy_size(channel);
            }
        }
        public void on_g2search_g2_saver_receive_clipcopy_job_finished(G2HSEARCH_G2 handle, int channel, G2CLIPCOPY_JOB.TYPE job, uint num, uint total)
        {
            if (_cancel_status == CANCEL_STATUS.SEND_CANCELED) return;
            if (_cancel_status == CANCEL_STATUS.CANCELED)
            {
                _cancel_status = CANCEL_STATUS.SEND_CANCELED;
                _adaptor_g2.request_clipcopy_cancel(_channel);
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
        public void on_g2search_g2_saver_receive_clipcopy_section_begin(G2HSEARCH_G2 handle, int channel, uint num, uint total) { }
        public void on_g2search_g2_saver_receive_clipcopy_section_end(G2HSEARCH_G2 handle, int channel, uint num, uint total) { }

        protected bool imp_start_g2(g2channel_set channel_set, List<int> ordered_set, G2SCOPE scope)
        {
            this._channel_set = channel_set;
            this._ordered_set = ordered_set;
            this._cancel_status = CANCEL_STATUS.NOT_CANCELED;

            if (imp_clip_create(_filedest) != true)
            {
                on_post_canceled(false);
                MessageBox.Show(this, "Failed create file", "Export Clip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            _adaptor_g2.request_scope_list(_channel, scope._begin._time, scope._end._time, channel_set, (int)G2PLAY_SCOPE_TYPE.TYPE.CLIP_COPY);

            return true;
        }

        protected void imp_g2_on_receive_scope_list(int channel, G2SCOPE[] scopes)
        {
            G2SCOPE scope = G2SCOPE.INVALID_SCOPE;

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
            bool player_exclude = CHK_EXCLUDE_PLAYER.Checked;

            if (CHK_INCLUDE_TEXT_IN.Checked)
            {
                _adaptor_g2.request_clipcopy_text_in(channel, true);
            }
            if (CHK_INCLUDE_GPS.Checked)
            {
                _adaptor_g2.request_clipcopy_gps_data(channel, true);
            }
            if (CHK_INCLUDE_EVENT.Checked)
            {
                _adaptor_g2.request_clipcopy_event(channel, true);
            }

            _adaptor_g2.request_clipcopy_measure_size(channel, _channel_set, scope, total, _ordered_set.ToArray(), true, slice_size, player_exclude);
            _adaptor_g2.request_clipcopy_info(channel, _channel_set);
        }

        protected void imp_g2_on_receive_clipcopy_size(int channel, G2CLIPCOPY_STATUS.TYPE status, G2CLIPCOPY_SIZE_INFO info, uint progress)
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
                    message = "Do you want to copy to %s?\n(File size was limited to 64GB)";
                }
                else
                {
                    ulong free_space = g2foundation.get_disk_free_space(Path.GetDirectoryName(_filename));
                    if (free_space >= uint.MaxValue)
                    {
                        message = "Do you want to copy to %s?\n(File size was limited to 4GB)";
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
                _adaptor_g2.request_clipcopy_password(channel, EDT_PASSWORD.Text);
            }
            else
            {
                _adaptor_g2.request_clipcopy_data(channel);
            }
        }

        private void on_timer_g2(object sender, EventArgs e)
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

                _adaptor_g2.request_clipcopy_size(_channel);
            }
            else if (sender == _timer_cancel)
            {
                _timer_cancel.Stop();
                post_canceled(false);
            }
        }
    }
}
