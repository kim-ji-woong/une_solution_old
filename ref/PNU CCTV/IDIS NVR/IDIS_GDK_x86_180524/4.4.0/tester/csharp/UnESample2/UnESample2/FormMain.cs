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

namespace UnESample2
{
    public partial class FormMain : Form
    {
        private GDK_tester.screen_pane SCREEN_PANE;
        private g2app_frame_relay_server _server;
        private GDK_tester.screen_pane _screen;
        private G2APP_FRAME_RELAY_SERVER_OPTION _option;
        private string m_strOptionFileName = "device1_sample_option.dat";

        //private bool _probe_perf;
        private bool _finalize = false;

        private FormCCTVList m_frmCCTVList = null;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        //void Test()
        //{
        //    string strPath = @"C:\Users\dev\Documents\script.sql";
        //    string strPath2 = @"C:\Users\dev\Documents\script2.sql";
        //    System.IO.StreamReader reader = new System.IO.StreamReader(strPath, System.Text.Encoding.UTF8);
        //    System.IO.StreamWriter writer = new System.IO.StreamWriter(strPath2, false, System.Text.Encoding.UTF8);

        //    while (reader.EndOfStream == false)
        //    {
        //        string strLine = reader.ReadLine().Trim();

        //        if (strLine.StartsWith("INSERT") == false)
        //        {
        //            writer.WriteLine(strLine);
        //            continue;
        //        }

        //        strLine = strLine.Replace("INSERT [dbo].[Secom_Alarm] (", "INSERT [dbo].[Secom_Alarm_History] ([TimeStamp], ");

        //        int nIndex = strLine.IndexOf("N'");
        //        int nIndex2 = strLine.IndexOf("N'", nIndex + 1);

        //        if (nIndex < 0 || nIndex2 < 0)
        //        {
        //            writer.WriteLine(strLine);
        //            continue;
        //        }

        //        string strDate = strLine.Substring(nIndex + 2, 8);
        //        string strTime = strLine.Substring(nIndex2 + 2, 6);
        //        string strTime2 = strDate + strTime + ".000";

        //        strLine = strLine.Replace("VALUES (", "VALUES (N'" + strTime2 + "', ");
        //        writer.WriteLine(strLine);
        //    }

        //    reader.Close();
        //    writer.Close();
        //}

        public FormMain()
        {
            //Test();
            m_instance = this;

            SCREEN_PANE = new GDK_tester.screen_pane();
            SCREEN_PANE.Name = "SCREEN_PANE";
            SCREEN_PANE.TabIndex = 0;
            _screen = SCREEN_PANE;

            InitializeComponent();

            //this.panelCCTV.Controls.Add(_screen);
            //SCREEN_PANE.Dock = DockStyle.Fill;
            _screen.Location = this.panelCCTV.Location;
            _screen.Size = this.panelCCTV.Size;
            this.Controls.Add(_screen);

            init_connective_server();
            init_connective_screen();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server.cleanup();
            _screen.cleanup();
        }

        public void Connect(string strIP, ushort nPort, int nChannel, string strID, string strPW)
        {
            if (nPort == 0 || nChannel <= 0 ||
               (strIP.Length == 0 || strID.Length == 0))
            {
                MessageBox.Show(this, "device information error", "site", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_server.is_disconnectable(0))
            {
                _server.disconnect(0);

                while (_server.is_disconnected(0) != true)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }

            G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION option = new G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION();
            option._server_ni._address = "127.0.0.1";
            option._server_ni.set_port(G2NETWORK_INFO.PORT_TYPE.SERVICE_PORT, _option._port);
            option._device_ni._address = strIP;
            option._device_ni._user_id = strID;
            option._device_ni._password = strPW;
            option._device_ni.set_port(G2NETWORK_INFO.PORT_TYPE.SEARCH_PORT, nPort);
            option._channels = new g2channel_set(nChannel - 1);
            option._site = "SITE_NVR_TEST";
            option._title = "G2Search Controller";
            option._no_port_unity = false;
            option._message_duration = 0;
            //option._message_duration = 10 * 1000;

            option._startup_pos_use = true;
            option._startup_pos.x = 100000;
            option._startup_pos.y = 100000;

            if (g2app_frame_relay_server.search_controller_option_make_file(m_strOptionFileName, ref option))
            {
                g2app_frame_relay_server.search_controller_run("tester_cpp_app_g2_search_controller.exe", m_strOptionFileName);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connect(textBoxIP.Text.Trim(), 8016, int.Parse(textBoxChannel.Text.Trim()), "admin", "Secom112");
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
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

        private void FormMain_Resize(object sender, EventArgs e)
        {

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
