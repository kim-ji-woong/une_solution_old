using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    public partial class form_log : Form
    {
        G2GUID _guid;
        public bool _is_admin = false;
        int _channel;
        int num = 0;
        long _s_row_id;
        long _d_row_id;
        long _dev_row_id;
        public string[] str = new string[] { "Loading...", "Loading...", "Loading..." };
        List<G2SYSTEM_LOG> _syslog_list = new List<G2SYSTEM_LOG>();
        List<G2DEBUG_LOG> _debuglog_list = new List<G2DEBUG_LOG>();
        List<G2SYSTEM_LOG> _devicelog_list = new List<G2SYSTEM_LOG>();
        G2SERVICE_SEARCH_OPTION_SYSTEM_LOG soption = new G2SERVICE_SEARCH_OPTION_SYSTEM_LOG();
        G2SERVICE_SEARCH_OPTION_DEBUG_LOG doption = new G2SERVICE_SEARCH_OPTION_DEBUG_LOG();
        G2SERVICE_SEARCH_OPTION_SYSTEM_LOG dev_option = new G2SERVICE_SEARCH_OPTION_SYSTEM_LOG();

        public G2GUID GUID { get { return _guid; } }
        public long S_ROW_ID { set { _s_row_id = value; } }
        public long D_ROW_ID { set { _d_row_id = value; } }

        public form_log(ref G2GUID guid)
        {
            InitializeComponent();
            CMB_LOG.SelectedIndex = 0;
            _guid = guid;
            soption._GUIDs.Clear();
            soption._GUIDs_reserved.Clear();
            soption._IDs.clear();
            soption._request_count = 100;
            soption._row_id = -1;
            doption._request_count = 100;
            doption._row_id = -1;
            dev_option._GUIDs.Clear();
            dev_option._GUIDs_reserved.Clear();
            dev_option._IDs.clear();
            dev_option._request_count = 100;
            dev_option._row_id = -1;
            _s_row_id = soption._row_id;
            _d_row_id = doption._row_id;
            _dev_row_id = dev_option._row_id;
        }

        public form_log(ref G2GUID guid, bool is_admin) : this(ref guid)
        {
            _is_admin = is_admin;
        }

        private void on_form_shown(object sender, EventArgs e)
        {
            main(ref _guid);
        }

        public void begin_update_log_list(object sender, LayoutEventArgs e)
        {
            LSV_LOG.Items.Clear();
            LSV_LOG.BeginUpdate();
            LABEL_LOG.Text = service_name(_guid);
            LABEL_STATUS.Text = str[0];
            foreach (G2SYSTEM_LOG log in _syslog_list)
            {
                G2SYSTEM_LOG input_log = log;
                string[] arr = new string[2];
                arr[0] = log._time.ToString();
                arr[1] = g2foundation.get_string_service_log(ref _guid, ref input_log);
                ListViewItem lvi = new ListViewItem(arr);
                LSV_LOG.Items.Add(lvi);
            }
            LSV_LOG.EndUpdate();
        }
  
        public void update_log_list(object sender, EventArgs e)
        {
            num = CMB_LOG.SelectedIndex;
            BTN_MORE.Enabled = true;
            LSV_LOG.Items.Clear();
            if (num == 0)
            {
                LSV_LOG.BeginUpdate();
                LABEL_LOG.Text = service_name(_guid);
                LABEL_STATUS.Text = str[0];
                foreach (G2SYSTEM_LOG log in _syslog_list)
                {
                    G2SYSTEM_LOG input_log = log;
                    string[] arr = new string[2];
                    arr[0] = log._time.ToString();
                    arr[1] = g2foundation.get_string_service_log(ref _guid, ref input_log);
                    ListViewItem lvi = new ListViewItem(arr);
                    LSV_LOG.Items.Add(lvi);
                }
                LSV_LOG.EndUpdate();
            }
            else if (num == 1)
            {
                LSV_LOG.BeginUpdate();
                LABEL_LOG.Text = service_name(_guid);
                LABEL_STATUS.Text = str[1];
                foreach (G2DEBUG_LOG log in _debuglog_list)
                {
                    string[] arr = new string[2];
                    arr[0] = log._time.ToString();
                    arr[1] = log._data._string.ToString();
                    ListViewItem lvi = new ListViewItem(arr);
                    LSV_LOG.Items.Add(lvi);
                }
                LSV_LOG.EndUpdate();

            }
            else if (num == 2)
            {
                LSV_LOG.BeginUpdate();
                LABEL_LOG.Text = service_name(_guid);
                LABEL_STATUS.Text = str[2];
                foreach (G2SYSTEM_LOG log in _devicelog_list)
                {
                    string[] arr = new string[2];
                    arr[0] = log._time.ToString();
                    arr[1] = log._data._string.ToString();
                    ListViewItem lvi = new ListViewItem(arr);
                    LSV_LOG.Items.Add(lvi);
                }
                LSV_LOG.EndUpdate();
            }
        }

        private void on_btn_more(object sender, EventArgs e)
        {
            BTN_MORE.Enabled = false;
            if (num == 0)
            {
                str[0] = "Loading...";
                LABEL_STATUS.Text = str[0];
                soption._row_id = _s_row_id;
                if (_guid.is_service_recording || _guid.is_service_recording_failover)
                {
                    _adaptor_play.request_system_log_search(_channel, soption);
                }
                else if (_guid.is_service_streaming)
                {
                    _adaptor_live.request_system_log_search(_channel, soption);
                }
                else if (_is_admin)
                {
                    g2admin admin = g2admin.get();
                    admin.request_system_log_search(soption);
                }
                else if (_guid.is_service_monitoring)
                {
                    g2monitor monitor = g2monitor.get();
                    monitor.request_system_log_search(soption);
                }
                else if (_guid.is_service_backup)
                {
                    _adaptor_backup.request_system_log_search(_channel, soption);
                }
            }
            else if (num == 1)
            {
                str[1] = "Loading...";
                LABEL_STATUS.Text = str[1];
                doption._row_id = _d_row_id;
                if (_guid.is_service_recording || _guid.is_service_recording_failover)
                {
                    _adaptor_play.request_debug_log_search(_channel, doption);
                }
                else if (_guid.is_service_streaming)
                {
                    _adaptor_live.request_debug_log_search(_channel, doption);
                }
                else if (_is_admin)
                {
                    g2admin admin = g2admin.get();
                    admin.request_debug_log_search(doption);
                }
                else if (_guid.is_service_monitoring)
                {
                    g2monitor monitor = g2monitor.get();
                    monitor.request_debug_log_search(doption);
                }
                else if (_guid.is_service_backup)
                {
                    _adaptor_backup.request_debug_log_search(_channel, doption);
                }
            }
            else if (num == 2)
            {
                str[2] = "Loading...";
                LABEL_STATUS.Text = str[2];
                dev_option._row_id = _dev_row_id;
                if (_guid.is_service_recording || _guid.is_service_recording_failover)
                {
                    _adaptor_play.request_device_log_search(_channel, dev_option);
                }              
            }
        }

        private void on_form_closing(object sender, FormClosingEventArgs e)
        {
            if (_guid.is_service_recording || _guid.is_service_recording_failover)
            {
                _adaptor_play.disconnect(_channel);
                while (_channel >= 0)
                {
                    Thread.Sleep(10);
                }
                _adaptor_play.cleanup();
            }
            else if (_guid.is_service_streaming)
            {
                _adaptor_live.disconnect(_channel);
                while (_channel >= 0)
                {
                    Thread.Sleep(10);
                }
                _adaptor_live.cleanup();
            }
            else if (_guid.is_service_backup)
            {
                _adaptor_backup.disconnect(_channel);
                while (_channel >= 0)
                {
                    Thread.Sleep(10);
                }
                _adaptor_backup.cleanup();
            }
        }
    }
}