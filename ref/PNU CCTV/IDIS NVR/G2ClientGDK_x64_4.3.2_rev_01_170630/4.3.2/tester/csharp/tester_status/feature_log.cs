using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GDK;

namespace GDK_tester
{
    public partial class form_status : Form
    {
        public class feature_log
        {
            public feature_log()
            {
                this.options = new G2STATUS_LOG_SEARCH_OPTIONS();
            }

            public G2STATUS_LOG_SEARCH_OPTIONS options;
            public bool support_system;
            public bool support_event;
            public bool support_debug;
            public bool more;
            public bool idr;
        }

        public void init_log()
        {
            this._feature_log = new feature_log();
            this.LOG_CHK_FIRST.Checked = true;
            this.LOG_CHK_LAST.Checked = true;
            this.LOG_BTN_SYSTEM.Tag = G2STATUS_LOG_SEARCH_OPTIONS.TYPE.SYSTEM;
            this.LOG_BTN_EVENT.Tag = G2STATUS_LOG_SEARCH_OPTIONS.TYPE.EVENT;
            this.LOG_BTN_DEBUG.Tag = G2STATUS_LOG_SEARCH_OPTIONS.TYPE.DEBUG;
            this.LOG_BTN_SYSTEM_MORE.Tag = G2STATUS_LOG_SEARCH_OPTIONS.TYPE.SYSTEM;
            this.LOG_BTN_EVENT_MORE.Tag = G2STATUS_LOG_SEARCH_OPTIONS.TYPE.EVENT;
            this.LOG_BTN_DEBUG_MORE.Tag = G2STATUS_LOG_SEARCH_OPTIONS.TYPE.DEBUG;

            feature_log_set_enable(false);
        }
        public void feature_log_reset()
        {
            LSV_LOG_SYSTEM.Items.Clear();
            LSV_LOG_EVENT.Items.Clear();
        }
        public void feature_log_set_product_info(ref G2_PRODUCT_INFO pi)
        {
            int channel = _channel;
            _feature_log.options._type = G2STATUS_LOG_SEARCH_OPTIONS.TYPE.UNDEFINED;
            _feature_log.support_system = _adaptor.is_support(channel, G2STATUS_SUPPORT.QUERY.SUPPORT_LOG_SYSTEM);
            _feature_log.support_event = _adaptor.is_support(channel, G2STATUS_SUPPORT.QUERY.SUPPORT_LOG_EVENT);
            _feature_log.support_debug = _adaptor.is_support(channel, G2STATUS_SUPPORT.QUERY.SUPPORT_LOG_DEBUG);
            _feature_log.more = false;
            _feature_log.idr = _adaptor.is_IDR(channel);
        }
        public void feature_log_set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_log_set_enable(enable); });
                return;
            }

            if (enable)
            {
                LOG_BTN_SYSTEM.Enabled = _feature_log.support_system;
                LOG_BTN_EVENT.Enabled = _feature_log.support_event;
                LOG_BTN_DEBUG.Enabled = _feature_log.support_debug;

                if (_feature_log.idr)
                {
                    LOG_CHK_FIRST.Enabled =
                    LOG_CHK_LAST.Enabled =
                    LOG_DTP_FROM.Enabled =
                    LOG_DTP_TO.Enabled =
                    LOG_BTN_SYSTEM_MORE.Enabled =
                    LOG_BTN_EVENT_MORE.Enabled =
                    LOG_BTN_DEBUG_MORE.Enabled = false;
                }
                else
                {
                    LOG_STC_FROM.Enabled =
                    LOG_STC_TO.Enabled =
                    LOG_CHK_FIRST.Enabled =
                    LOG_CHK_LAST.Enabled = true;
                    LOG_DTP_FROM.Enabled = (LOG_CHK_FIRST.Checked != true);
                    LOG_DTP_TO.Enabled = (LOG_CHK_LAST.Checked != true);
                    LOG_BTN_SYSTEM_MORE.Enabled = _feature_log.support_system && _feature_log.more && (_feature_log.options._type == G2STATUS_LOG_SEARCH_OPTIONS.TYPE.SYSTEM);
                    LOG_BTN_EVENT_MORE.Enabled = _feature_log.support_event && _feature_log.more && (_feature_log.options._type == G2STATUS_LOG_SEARCH_OPTIONS.TYPE.EVENT);
                    LOG_BTN_DEBUG_MORE.Enabled = _feature_log.support_debug && _feature_log.more && (_feature_log.options._type == G2STATUS_LOG_SEARCH_OPTIONS.TYPE.DEBUG);
                }

                if (_feature_log.options._type == G2STATUS_LOG_SEARCH_OPTIONS.TYPE.EVENT)
                {
                    LSV_LOG_EVENT.Visible = true;
                    LSV_LOG_SYSTEM.Visible = false;
                }
                else
                {
                    LSV_LOG_SYSTEM.Visible = true;
                    LSV_LOG_EVENT.Visible = false;
                }

                CHK_INSTANT_RECORD_STATUS.Checked = false;
            }
            else
            {
                foreach (Control c in GRP_LOG.Controls)
                {
                    c.Enabled = enable;
                }
            }
        }

        private void on_post_feature_log_receive_system(int channel, G2STATUS_LOG_SYSTEM[] logs)
        {
            int select = LSV_LOG_SYSTEM.Items.Count;

            LSV_LOG_SYSTEM.BeginUpdate();

            foreach (G2STATUS_LOG_SYSTEM log in logs)
            {
                ListViewItem lvi = new ListViewItem(log._label);
                lvi.Tag = log;
                lvi.SubItems.Add(log._time.to_string_date_time());
                LSV_LOG_SYSTEM.Items.Add(lvi);
            }

            if (LSV_LOG_SYSTEM.Items.Count > select)
            {
                LSV_LOG_SYSTEM.EnsureVisible(select);
                LSV_LOG_SYSTEM.Items[select].Selected = true;
            }

            LSV_LOG_SYSTEM.EndUpdate();

            _feature_log.more = (logs.Length > 0);
            _message.hide(STRING.NIS_SEARCHING);

            feature_log_set_enable(true);
        }
        private void on_post_feature_log_receive_event(int channel, G2STATUS_LOG_EVENT[] logs)
        {
            int select = LSV_LOG_EVENT.Items.Count;

            LSV_LOG_EVENT.BeginUpdate();

            foreach (G2STATUS_LOG_EVENT log in logs)
            {
                ListViewItem lvi = new ListViewItem(log._event);
                lvi.Tag = log;
                lvi.SubItems.Add(log._label);
                lvi.SubItems.Add(log._time.to_string_date_time());
                LSV_LOG_EVENT.Items.Add(lvi);
            }

            if (LSV_LOG_EVENT.Items.Count > select)
            {
                LSV_LOG_EVENT.EnsureVisible(select);
                LSV_LOG_EVENT.Items[select].Selected = true;
            }

            LSV_LOG_EVENT.EndUpdate();

            _feature_log.more = (logs.Length > 0);
            _message.hide(STRING.NIS_SEARCHING);

            feature_log_set_enable(true);
        }
        private void on_post_feature_log_receive_debug(int channel, G2STATUS_LOG_SYSTEM[] logs)
        {
            int select = LSV_LOG_SYSTEM.Items.Count;

            LSV_LOG_SYSTEM.BeginUpdate();

            foreach (G2STATUS_LOG_SYSTEM log in logs)
            {
                ListViewItem lvi = new ListViewItem(log._label);
                lvi.Tag = log;
                lvi.SubItems.Add(log._time.to_string_date_time());
                LSV_LOG_SYSTEM.Items.Add(lvi);
            }

            if (LSV_LOG_SYSTEM.Items.Count > select)
            {
                LSV_LOG_SYSTEM.EnsureVisible(select);
                LSV_LOG_SYSTEM.Items[select].Selected = true;
            }

            LSV_LOG_SYSTEM.EndUpdate();

            _feature_log.more = (logs.Length > 0);
            _message.hide(STRING.NIS_SEARCHING);

            feature_log_set_enable(true);
        }

        protected bool imp_log_get_time_range(out G2SCOPE scope)
        {
            scope = new G2SCOPE();

            if (LOG_CHK_FIRST.Checked) scope._begin._time.reset();
            else scope._begin._time = LOG_DTP_FROM.Value;
            if (LOG_CHK_LAST.Checked) scope._end._time.reset();
            else scope._end._time = LOG_DTP_TO.Value;

            return (scope._begin._time.valid && scope._end._time.valid &&
                   (scope._begin._time >= scope._end._time)) ? false : true;
        }

        private void on_log_chk_first(object sender, EventArgs e)
        {
            LOG_DTP_FROM.Enabled = (LOG_CHK_FIRST.Checked != true);
        }
        private void on_log_chk_last(object sender, EventArgs e)
        {
            LOG_DTP_TO.Enabled = (LOG_CHK_LAST.Checked != true);
        }
        private void on_log_btn_request(object sender, EventArgs e)
        {
            G2SCOPE scope;
            if (imp_log_get_time_range(out scope) != true)
            {
                MessageBox.Show(this, "Invalid recording range", "Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Button control = sender as Button;
            LSV_LOG_SYSTEM.Items.Clear();
            LSV_LOG_EVENT.Items.Clear();

            G2STATUS_LOG.RESULT res = G2STATUS_LOG.RESULT.FAIL;
            G2STATUS_LOG_SEARCH_OPTIONS options = new G2STATUS_LOG_SEARCH_OPTIONS();
            options._type = (G2STATUS_LOG_SEARCH_OPTIONS.TYPE)control.Tag;
            options._from = scope._begin._time;
            options._to = scope._end._time;
            options._reload = true;

            if (_adaptor.request_log(_channel, ref options, out res))
            {
                feature_log_set_enable(false);

                _feature_log.options = options;
                _message.disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 0, true, 500);
            }
        }
        private void on_log_btn_request_more(object sender, EventArgs e)
        {
            G2STATUS_LOG.RESULT res = G2STATUS_LOG.RESULT.FAIL;
            G2STATUS_LOG_SEARCH_OPTIONS options = _feature_log.options;
            options._reload = false;

            Button control = sender as Button;

            if (options._type != (G2STATUS_LOG_SEARCH_OPTIONS.TYPE)control.Tag) return;
            if (_adaptor.request_log(_channel, ref options, out res))
            {
                if (res == G2STATUS_LOG.RESULT.NO_MORE)
                {
                    switch (options._type)
                    {
                        case G2STATUS_LOG_SEARCH_OPTIONS.TYPE.SYSTEM: LOG_BTN_SYSTEM_MORE.Enabled = false; break;
                        case G2STATUS_LOG_SEARCH_OPTIONS.TYPE.EVENT: LOG_BTN_EVENT_MORE.Enabled = false; break;
                        case G2STATUS_LOG_SEARCH_OPTIONS.TYPE.DEBUG: LOG_BTN_DEBUG_MORE.Enabled = false; break;
                    }
                }
                else
                {
                    feature_log_set_enable(false);

                    _message.disp(STRING.NIS_SEARCHING, STRING.get(STRING.NIS_SEARCHING), 0, true, 500);
                }
            }
        }

        private feature_log _feature_log;
    }
}
