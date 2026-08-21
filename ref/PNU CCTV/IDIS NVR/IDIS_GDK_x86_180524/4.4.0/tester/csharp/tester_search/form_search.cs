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
    public partial class form_search : Form
    {
        public form_search()
        {
            this.SCREEN_PANE = new GDK_tester.screen_pane();
            this.SCREEN_PANE.Name = "SCREEN_PANE";
            this.SCREEN_PANE.TabIndex = 0;
            this.Controls.Add(this.SCREEN_PANE);

            InitializeComponent();

            this.SCREEN_PANE.Location = STC_SCREEN.Location;
            this.SCREEN_PANE.Size = STC_SCREEN.Size;
            this.LSV_EVENT.Location = STC_TIME_TABLE.Location;
            this.LSV_EVENT.Size = STC_TIME_TABLE.Size;
            this.STC_PROBE_PERF.Text = "";

            this._adaptor = null;
            this._ni = new G2NETWORK_INFO();
            this._channel = -1;
            this._count_camera = 0;
            this._finalize = false;

            init_connective_search_g1();
            init_connective_search_g2();
            init_connective_device_info();
            init_connective_screen();
            init_connective_controller();
            init_connective_time_table();
            init_connective_fen();
        }

        public void set_camera_list(int channel)
        {
            g2channel_set channelset_interest = new g2channel_set();
            g2channel_set channelset = new g2channel_set();

            for (int i = 0; i < _count_camera; ++i)
            {
                channelset_interest.insert(i);

                if (_screen.is_visible(i))
                {
                    channelset.insert(i);
                }
            }

            if (_adaptor is g2search_g2)
            {
                imp_set_camera_list_g2(channel, channelset_interest, channelset);
            }
            else
            {
                imp_set_camera_list_g1(channel, channelset_interest, channelset);
            }
        }

        public bool valid_channel(int channel)
        {
            return channel >= 0;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            BTN_CONNECT.Focus();
        }

        private void on_form_closing(object sender, FormClosingEventArgs e)
        {
            _finalize = true;

            if (_adaptor != null)
            {
                int channel = _channel;
                if (_adaptor.is_disconnectable(channel))
                {
                    _screen.search_shutdown(channel);
                    _adaptor.disconnect(channel);
                }

                while (valid_channel(_channel))
                {
                    Thread.Sleep(10);
                }
            }

            _adaptor_g2.cleanup();
            _adaptor_g1.cleanup();
            _screen.cleanup();
        }
        private void on_event_list_selected_changed(object sender, EventArgs e)
        {
            if (LSV_EVENT.SelectedItems.Count != 0)
            {
                object data = LSV_EVENT.SelectedItems[0].Tag;

                if (data is G2EVENT)
                {
                    _controller_g2.on_event_list_selected((G2EVENT)data);
                }
                else if (data is G2SEARCH_LOG_INFO)
                {
                    _controller_g1.on_event_list_selected((G2SEARCH_LOG_INFO)data);
                }
            }
        }
        private void on_btn_connect(object sender, EventArgs e)
        {
            _screen.message().hide();

            form_connect form = new form_connect(G2NETWORK_INFO.PORT_TYPE.SEARCH_PORT, 8016);
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
                        g2fen.get().startup(2, 2, 2, 2, "tester_fen_history_search.dat");
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

            _ni = ni;
            _screen.message().disp(STRING.NIS_CONNECTING, ni._address + " : connecting...", 0, true, 1000);

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
                            _screen.message().disp(error, 10 * 1000, false);
                        }

                        BTN_CONNECT.Enabled = true;
                    }
                }
            }
            else
            {
                if (form.internal_inf.search_G2)
                {
                    _adaptor = _adaptor_g2;
                }
                else
                {
                    _adaptor = _adaptor_g1;
                }

                on_post_connect_imp(ref ni, form.internal_inf.port_unity);
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
                _adaptor_g1.disconnect(channel);
                _adaptor_g2.disconnect(channel);
                _screen.search_shutdown(channel);
            }
        }
        private void on_chk_probe_perf(object sender, EventArgs e)
        {
            _probe_perf = CHK_PROBE_PERF.Checked;
            _adaptor_g1.set_probe_session_profile(_probe_perf);
            _adaptor_g2.set_probe_session_profile(_probe_perf);
            _screen.option_use_probe_performance = _probe_perf;

            if (_probe_perf != true)
            {
                STC_PROBE_PERF.Text = "";
            }
        }

        private adaptor_playable _adaptor;
        private search_data _data;
        private G2NETWORK_INFO _ni;
        private int _channel;
        private int _count_camera;
        private bool _probe_perf;
        private bool _finalize;
    }

    public class STRING
    {
        public const int NIS_CONNECTING = 1;
        public const int NIS_LOADING_RECORD_TIME = 2;
        public const int NIS_NO_RECORDED_IMAGE = 3;
        public const int NIS_NO_RECORDED_DATA = 4;
        public const int NIS_SEARCHING = 5;
        public const int NIS_NO_RESULT = 6;
        public const int NIS_NO_ASSEMBLY_DSOUND = 7;
        public static string get(int id)
        {
            if (id == NIS_CONNECTING) return "connecting...";
            if (id == NIS_LOADING_RECORD_TIME) return "loading record time information...";
            if (id == NIS_NO_RECORDED_IMAGE) return "there is no recorded image.";
            if (id == NIS_NO_RECORDED_DATA) return "there is no recorded data.";
            if (id == NIS_SEARCHING) return "searching...";
            if (id == NIS_NO_RESULT) return "no result";
            if (id == NIS_NO_ASSEMBLY_DSOUND) return "cannot load assembly \"Microsoft.DirectX.DirectSound\"";
            return "";
        }
    }
}
