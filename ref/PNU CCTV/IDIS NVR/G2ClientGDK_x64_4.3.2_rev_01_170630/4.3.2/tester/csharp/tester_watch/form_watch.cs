using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using GDK;

namespace GDK_tester
{
    public partial class form_watch : Form
    {
        public form_watch()
        {
            this.SCREEN_PANE = new GDK_tester.screen_pane();
            this.SCREEN_PANE.Name = "SCREEN_PANE";
            this.SCREEN_PANE.TabIndex = 0;
            this.Controls.Add(this.SCREEN_PANE);

            InitializeComponent();

            this.SCREEN_PANE.Location = STC_SCREEN.Location;
            this.SCREEN_PANE.Size = STC_SCREEN.Size;
            this.STC_PROBE_PERF.Text = "";

            init_connective_watch();
            init_connective_screen();
            init_connective_fen();
            init_control_ptz();
            init_control_color();
            init_control_alarm_out();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            BTN_CONNECT.Focus();
        }

        private void on_form_closing(object sender, FormClosingEventArgs e)
        {
            _finalize = true;

            if (_adaptor.is_disconnectable(_channel))
            {
                _adaptor.disconnect(_channel);

                while (valid_channel(_channel))
                {
                    Thread.Sleep(10);
                }
            }

            _adaptor.cleanup();
            _screen.cleanup();
        }
        private void on_btn_connect(object sender, EventArgs e)
        {
            _screen.message().hide();

            form_connect form = new form_connect(G2NETWORK_INFO.PORT_TYPE.WATCH_PORT, 8016);
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            BTN_CONNECT.Enabled = false;

            G2NETWORK_INFO ni = form.internal_inf.ni;
            int channel_pre = _channel;
            if (_adaptor.is_disconnectable(channel_pre))
            {
                _adaptor.disconnect(channel_pre);

                while (_adaptor.is_disconnected(channel_pre) != true)
                {
                    Thread.Sleep(10);
                }
            }

            if (ni._address_type == G2NETWORK_INFO.ADDRESS_TYPE.DVRNS)
            {
                if (form.internal_inf.FEN_query)
                {
                    if (g2fen.get().is_startup() != true)
                    {
                        g2fen.get().startup(2, 2, 2, 2, "tester_fen_history_watch.dat");
                    }

                    if (g2fen.get().is_startup())
                    {
                        using (scoped_screen_message message = new scoped_screen_message(_screen.message(), 0, "FEN preparing...", 0, true))
                        {
                            this.Enabled = false;
                            g2fen.get().set_gateway(ni._extra_server_address, ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
                            g2fen.get().join_service((uint)TimeSpan.FromMinutes(2).TotalMilliseconds, true);
                            this.Enabled = true;
                        }
                    }

                    if (g2fen.get().is_activate() != true)
                    {
                        g2main.dvrns_setup(ni._extra_server_address, ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
                    }
                }

                if (_finalize) return;
            }

            lock (_adaptor)
            {
                _screen.message().disp(STRING.NIS_CONNECTING, ni._address + " : connecting...", 0, true, 1000);

                G2CONNECT_RES res;
                int channel = _adaptor.connect_ras(ref ni, out res);
                if (valid_channel(channel))
                {
                    BTN_DISCONNECT.Enabled = true;

                    _channel = channel;
                    _screen.buf_setup(_channel, 64, frame_buf.TYPE.LIVE);
                }
                else
                {
                    _channel = -1;

                    if (res._err_dvrns != 0)
                    {
                        string error = g2foundation.get_string_dvrns_error(res._err_dvrns);
                        _screen.message().disp(error, 10 * 1000, false);
                    }

                    BTN_CONNECT.Enabled = true;
                }
            }
        }
        private void on_btn_disconnect(object sender, EventArgs e)
        {
            if (valid_channel(_channel))
            {
                _adaptor.disconnect(_channel);
            }
        }
        private void on_btn_request_device_status(object sender, EventArgs e)
        {
            if (_adaptor.is_connected(_channel))
            {
                _adaptor.request_device_status(_channel);
            }
        }
        private void on_btn_remote_setup(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_authority(channel, G2RAS_AUTHORITY.TYPE.AUTHORITY_SETUP))
            {
                if (File.Exists(@".\remote_setup\G2RemoteConf.exe") != true)
                {
                    MessageBox.Show(this, "There is no G2RemoteSetup program.", "Remote Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string path = Path.GetFullPath(@".\remote_setup\");
                path += Path.GetRandomFileName();
                if (_adaptor.get_remote_setup_info_file(channel, path, _sitename, this.Handle))
                {
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = @".\remote_setup\G2RemoteConf.exe";
                    si.Arguments = path;
                    Process.Start(si);
                }
                else
                {
                    MessageBox.Show(this, "Failed to configure remote setup information.", "Remote Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void on_chk_probe_perf(object sender, EventArgs e)
        {
            _probe_perf = CHK_PROBE_PERF.Checked;
            _adaptor.set_probe_session_profile(_probe_perf);
            _screen.option_use_probe_performance = _probe_perf;

            if (_probe_perf != true)
            {
                STC_PROBE_PERF.Text = "";
            }
        }
        private void on_btn_network_alarm(object sender, EventArgs e)
        {
            form_network_alarm form = new form_network_alarm(_adaptor, _channel);
            form.ShowDialog(this);
        }

        private bool _probe_perf;
        private bool _finalize;
    }

    public class STRING
    {
        public const int NIS_CONNECTING = 1;
        public static string get(int id)
        {
            if (id == NIS_CONNECTING) return "connecting...";
            return "";
        }
    }
}
