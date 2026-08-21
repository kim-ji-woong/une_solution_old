using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GDK;

namespace GDK_tester
{
    public partial class form_status : Form
    {
        public form_status()
        {
            InitializeComponent();

            screen_options.color.message.back = Color.FromArgb(240, 240, 240);
            screen_options.color.message.border = Color.FromArgb(170, 170, 170);
            screen_options.color.message.text = Color.FromArgb(50, 50, 50);
            screen_options.color.message.spin = Color.FromArgb(90, 90, 90);

            this._message = new screen_message(this);
            this._message.Owner = this;
            this._message.Opacity = 0.9;
            this._ni = new G2NETWORK_INFO();
            this._finalize = false;

            init_connective_status();
            init_connective_device_info();
            init_status();
            init_log();
            init_health();
            init_instant_record();
            init_record_panic();
            init_record_status();
            init_callback();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            BTN_CONNECT.Focus();
        }

        private void on_form_load(object sender, EventArgs e)
        {

        }
        private void on_form_closing(object sender, FormClosingEventArgs e)
        {
            _finalize = true;

            int channel = _channel;
            if (_adaptor.is_disconnectable(channel))
            {
                _adaptor.disconnect(channel);

                while (valid_channel(_channel))
                {
                    Thread.Sleep(10);
                }
            }

            _adaptor.cleanup();
        }
        private void on_btn_connect(object sender, EventArgs e)
        {
            _message.hide();

            form_connect form = new form_connect(G2NETWORK_INFO.PORT_TYPE.ADMIN_PORT, 8200);
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            BTN_CONNECT.Enabled = false;

            G2NETWORK_INFO ni = form.internal_inf.ni;
            int channel_pre = _channel;

            if (_adaptor != null)
            {
                if (_adaptor.is_disconnectable(_channel))
                {
                    _adaptor.disconnect(_channel);
                }

                while (valid_channel(_channel))
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
                        g2fen.get().startup(2, 2, 2, 2, "tester_fen_history_status.dat");
                    }

                    if (g2fen.get().is_startup())
                    {
                        using (scoped_screen_message message = new scoped_screen_message(_message, 0, "FEN preparing...", 0, true))
                        {
                            this.Enabled = false;
                            g2fen.get().set_gateway(ni._extra_server_address, ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
                            g2fen.get().join_service((uint)TimeSpan.FromMinutes(2).TotalMilliseconds, true);
                            this.Enabled = true;
                        }
                    }
                }

                if (g2fen.get().is_activate() != true)
                {
                    g2main.dvrns_setup(ni._extra_server_address, ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
                }

                if (_finalize) return;
            }

            _ni = ni;
            _message.disp(STRING.NIS_CONNECTING, ni._address + " : connecting...", 0, true, 1000);

            G2CONNECT_RES res;
            if (form.internal_inf.port_check_use &&
                form.internal_inf.port_check != 0)
            {
                lock (_adaptor_di)
                {
                    if (ni._address_type != G2NETWORK_INFO.ADDRESS_TYPE.DVRNS)
                    {
                        ni.set_port(G2NETWORK_INFO.PORT_TYPE.WATCH_PORT, form.internal_inf.port_check);
                    }

                    int channel = _adaptor_di.connect_ras(ref ni, out res);
                    if (valid_channel(channel))
                    {
                        BTN_DISCONNECT.Enabled = true;

                        _channel_di = channel;
                    }
                    else
                    {
                        _channel_di = -1;

                        if (res._err_dvrns != 0)
                        {
                            string error = g2foundation.get_string_dvrns_error(res._err_dvrns);
                            _message.disp(error, 10 * 1000, false);
                        }

                        BTN_CONNECT.Enabled = true;
                    }
                }
            }
            else
            {
                on_post_connect_imp(ref ni, form.internal_inf.IDR);
            }
        }
        private void on_btn_disconnect(object sender, EventArgs e)
        {
            int channel_di = _channel_di;
            int channel = _channel;

            if (_adaptor_di.is_disconnectable(channel_di))
            {
                _adaptor_di.disconnect(channel_di);
            }

            if (valid_channel(channel))
            {
                _adaptor.disconnect(channel);
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
                if (_adaptor.get_remote_setup_info_file(channel, path, "", this.Handle))
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

        protected screen_message _message;
        private G2NETWORK_INFO _ni;
        private bool _finalize;
    }

    public class STRING
    {
        public const int NIS_CONNECTING = 1;
        public const int NIS_SEARCHING = 2;
        public const int NIS_NO_RESULT = 3;
        public static string get(int id)
        {
            if (id == NIS_CONNECTING) return "connecting...";
            if (id == NIS_SEARCHING) return "searching...";
            if (id == NIS_NO_RESULT) return "no result";
            return "";
        }
    }
}
