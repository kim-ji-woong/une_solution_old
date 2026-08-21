using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDK;

namespace UnESample
{
    public partial class FormMain : Form, g2user_listener
    {
        private GDK_tester.screen_pane SCREEN_PANE;
        private GDK_tester.screen_pane _screen = null;

        //private bool _probe_perf;
        private bool _finalize = false;

        private int m_nChannel = -1;

        private FormCCTVList m_frmCCTVList = null;
        private static FormMain m_instance = null;

        public static FormMain Instace
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;

            SCREEN_PANE = new GDK_tester.screen_pane();
            SCREEN_PANE.Name = "SCREEN_PANE";
            SCREEN_PANE.TabIndex = 0;
            SCREEN_PANE.Dock = DockStyle.Fill;
            _screen = SCREEN_PANE;

            InitializeComponent();

            this.panelCCTV.Controls.Add(this.SCREEN_PANE);

            init_connective_watch();
            init_connective_screen();
            //init_connective_fen();
            init_control_ptz();
            init_control_color();
            //init_control_alarm_out();

            _screen.set_pane_select(0);
            _screen.set_format2(GDK_tester.screen_format.FORMAT.LAYOUT1X1, true, 0);

            Connect("192.168.122.179", 8016, 1, "admin", "Secom112");
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _finalize = true;

            if (_adaptor.is_disconnectable(_channel))
            {
                _adaptor.disconnect(_channel);

                while (valid_channel(_channel))
                {
                    System.Threading.Thread.Sleep(10);
                }
            }

            _adaptor.cleanup();
            _screen.cleanup();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //Connect("", 0, 1, "", "");
            //_screen.message().hide();

            ///*form_connect form = new form_connect(G2NETWORK_INFO.PORT_TYPE.WATCH_PORT, 8016);
            //if (form.ShowDialog(this) != DialogResult.OK)
            //{
            //    return;
            //}*/

            //btnConnect.Enabled = false;

            //G2NETWORK_INFO ni = new G2NETWORK_INFO();
            //ni._address = "192.168.0.55";
            //ni.set_port(G2NETWORK_INFO.PORT_TYPE.WATCH_PORT, 8016);
            //ni._user_id = "admin";
            //ni._password = "Secom112";
            ////G2NETWORK_INFO ni = form.internal_inf.ni;
            //int channel_pre = _channel;
            //if (_adaptor.is_disconnectable(channel_pre))
            //{
            //    _adaptor.disconnect(channel_pre);

            //    while (_adaptor.is_disconnected(channel_pre) != true)
            //    {
            //        System.Threading.Thread.Sleep(10);
            //    }
            //}

            //if (ni._address_type == G2NETWORK_INFO.ADDRESS_TYPE.DVRNS)
            //{
            //    //if (form.internal_inf.FEN_query)
            //    {
            //        if (g2fen.get().is_startup() != true)
            //        {
            //            g2fen.get().startup(2, 2, 2, 2, "tester_fen_history_watch.dat");
            //        }

            //        if (g2fen.get().is_startup())
            //        {
            //            using (GDK_tester.scoped_screen_message message = new GDK_tester.scoped_screen_message(_screen.message(), 0, "FEN preparing...", 0, true))
            //            {
            //                this.Enabled = false;
            //                g2fen.get().set_gateway(ni._extra_server_address, ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
            //                g2fen.get().join_service((uint)TimeSpan.FromMinutes(2).TotalMilliseconds, true);
            //                this.Enabled = true;
            //            }
            //        }

            //        if (g2fen.get().is_activate() != true)
            //        {
            //            g2main.dvrns_setup(ni._extra_server_address, ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
            //        }
            //    }

            //    if (_finalize) return;
            //}

            //lock (_adaptor)
            //{
            //    _screen.message().disp(STRING.NIS_CONNECTING, ni._address + " : connecting...", 0, true, 1000);

            //    G2CONNECT_RES res;
            //    int channel = _adaptor.connect_ras(ref ni, out res);
            //    if (valid_channel(channel))
            //    {
            //        btnDisconnect.Enabled = true;

            //        _channel = channel;
            //        _screen.buf_setup(_channel, 64, GDK_tester.frame_buf.TYPE.LIVE);
            //    }
            //    else
            //    {
            //        _channel = -1;

            //        if (res._err_dvrns != 0)
            //        {
            //            string error = g2foundation.get_string_dvrns_error(res._err_dvrns);
            //            _screen.message().disp(error, 10 * 1000, false);
            //        }

            //        btnConnect.Enabled = true;
            //    }
            //}
        }

        public void Connect(string strIP, ushort nPort, int nChannel, string strID, string strPW)
        {
            _screen.message().hide();

            /*form_connect form = new form_connect(G2NETWORK_INFO.PORT_TYPE.WATCH_PORT, 8016);
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }*/

            m_nChannel = nChannel;
            btnConnect.Enabled = false;

            G2NETWORK_INFO ni = new G2NETWORK_INFO();
            ni._address = strIP;
            ni.set_port(G2NETWORK_INFO.PORT_TYPE.WATCH_PORT, nPort);
            ni._user_id = strID;
            ni._password = strPW;
            //G2NETWORK_INFO ni = form.internal_inf.ni;
            int channel_pre = _channel;
            if (_adaptor.is_disconnectable(channel_pre))
            {
                _adaptor.disconnect(channel_pre);

                while (_adaptor.is_disconnected(channel_pre) != true)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }

            if (ni._address_type == G2NETWORK_INFO.ADDRESS_TYPE.DVRNS)
            {
                //if (form.internal_inf.FEN_query)
                {
                    if (g2fen.get().is_startup() != true)
                    {
                        g2fen.get().startup(2, 2, 2, 2, "tester_fen_history_watch.dat");
                    }

                    if (g2fen.get().is_startup())
                    {
                        using (GDK_tester.scoped_screen_message message = new GDK_tester.scoped_screen_message(_screen.message(), 0, "FEN preparing...", 0, true))
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
                //if (valid_channel(nChannel))
                int channel = _adaptor.connect_ras(ref ni, out res);
                if (valid_channel(channel))
                {
                    btnDisconnect.Enabled = true;

                    _channel = channel;
                    _screen.buf_setup(_channel, 64, GDK_tester.frame_buf.TYPE.LIVE);
                }
                else
                {
                    _channel = -1;

                    if (res._err_dvrns != 0)
                    {
                        string error = g2foundation.get_string_dvrns_error(res._err_dvrns);
                        _screen.message().disp(error, 10 * 1000, false);
                    }

                    btnConnect.Enabled = true;
                }
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (valid_channel(_channel))
            {
                _adaptor.disconnect(_channel);
            }
        }

        private void btnShowList_Click(object sender, EventArgs e)
        {
            if (m_frmCCTVList == null || m_frmCCTVList.IsDisposed)
            {
                m_frmCCTVList = new FormCCTVList();
                m_frmCCTVList.Show(this);
            }
            else
            {
                if (m_frmCCTVList.Visible)
                    m_frmCCTVList.Focus();
                else
                    m_frmCCTVList.Show(this);
            }
        }

        public void on_g2user_connected()
        {
            System.Diagnostics.Trace.WriteLine("on_g2user_connected");
        }

        public void on_g2user_disconnect_entered(G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user)
        {
            System.Diagnostics.Trace.WriteLine("on_g2user_disconnect_entered");
        }

        public void on_g2user_login()
        {
            System.Diagnostics.Trace.WriteLine("on_g2user_login");
        }

        public void on_g2user_login_cancelled()
        {
            System.Diagnostics.Trace.WriteLine("on_g2user_login_cancelled");
        }

        public void on_g2user_login_failed(ref G2NETWORK_INFO ni, G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user)
        {
            System.Diagnostics.Trace.WriteLine("on_g2user_login_failed");
        }

        public void on_g2user_login_failed_from_dvrns(ref G2NETWORK_INFO ni, G2FEN_RESULT.TYPE reason)
        {
            System.Diagnostics.Trace.WriteLine("on_g2user_login_failed_from_dvrns");
        }

        public void on_g2user_logout(G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user)
        {
            System.Diagnostics.Trace.WriteLine("on_g2user_logout");
        }

        public void on_g2user_modify()
        {
            System.Diagnostics.Trace.WriteLine("on_g2user_modify");
        }

        public void on_g2user_set_authority()
        {
            System.Diagnostics.Trace.WriteLine("on_g2user_set_authority");
        }

        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            int nPaneIndex = m_nChannel <= 0 ? 0 : m_nChannel - 1;

            //if (m_nChannel > 0)
            {
                GDK_tester.camera_pane cp = _screen.get_pane(nPaneIndex);

                if (cp != null)
                {
                    Rectangle rect = new Rectangle(new Point(0, 0), this.panelCCTV.Size);
                    cp.set_rect(ref rect);
                    _screen.reset(true);
                    _screen.Refresh();
                }
            }
        }

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
