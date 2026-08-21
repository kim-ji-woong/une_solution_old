using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SampleViewer
{
    public partial class FormMain : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32")]
        static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private static int WM_CLOSE = 0x0010;

        private FormCCTVList m_frmCCTVList = null;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        //void Test()
        //{
        //    string strPath = @"C:\UnESolution\trunk\ref\PNU CCTV\ITX NVR\SampleViewer\SampleViewer\bin\Debug\pnu_cctv.txt";
        //    System.IO.StreamReader reader = new System.IO.StreamReader(strPath, System.Text.Encoding.UTF8);
        //    Dictionary<string, string> ips = new Dictionary<string, string>();

        //    while (reader.EndOfStream == false)
        //    {
        //        string strLine = reader.ReadLine().Trim();

        //        if (strLine.Contains("ITX") == false)
        //            continue;

        //        int nIndex = strLine.IndexOf("192.");

        //        if (nIndex < 0)
        //            continue;

        //        int nIndex2 = strLine.IndexOf('\t', nIndex + 1);

        //        if (nIndex2 < 0)
        //            continue;

        //        string strIP = strLine.Substring(nIndex, nIndex2 - nIndex).Trim();
        //        ips[strIP] = strIP;
        //    }

        //    reader.Close();

        //    string strPath2 = @"C:\UnESolution\trunk\ref\PNU CCTV\ITX NVR\SampleViewer\SampleViewer\bin\Debug\pnu_cctv2.txt";
        //    System.IO.StreamWriter writer = new System.IO.StreamWriter(strPath2, false, System.Text.Encoding.UTF8);

        //    foreach (KeyValuePair<string, string> pair in ips)
        //    {
        //        writer.WriteLine(pair.Value);
        //    }

        //    writer.Close();
        //}

        public FormMain()
        {
            //Test();
            m_instance = this;
            InitializeComponent();

            /*axitxview1.SetAccount("ADMIN", "Secom112");
            axitxview1.SetOEMCode("S1", "IPXP3_1643");
            axitxview1.SetMaxLayout(1);
            axitxview1.SetMacAddress("누구 mac address");
            axitxview1.SessionOpen("192.168.122.10", 554, 0, 1, 1, 0, 0);*/

            Connect("192.168.122.101", 8521, 1, "ADMIN", "Secom112", "00115fa407eb");
        }

        private void btnShowCCTVList_Click(object sender, EventArgs e)
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

        public void Connect(string strIP, ushort nPort, int nChannel, string strID, string strPW, string strMacAddr)
        {
            if (nPort == 0 || nChannel <= 0 ||
               (strIP.Length == 0 || strID.Length == 0))
            {
                MessageBox.Show(this, "device information error", "site", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (axitxview1.IsConnected())
            {
                timer1.Stop();
                axitxview1.SessionClose();
            }

            // 접속 성공여부를 감시하기 위한 타이머
            timer1.Start();

            /*string macAddr =
            (
                from nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();*/

            //strMacAddr = "00115fa407eb";
            //nPort = 8521;

            axitxview1.SetAccount(strID, strPW);
            axitxview1.SetOEMCode("S1", "IPX_0412");
            axitxview1.SetMaxLayout(16);
            axitxview1.SetMacAddress(strMacAddr);
            axitxview1.SessionOpen(strIP + "/live", nPort, 1, 1, 1, 0, 0);
            axitxview1.SetSplitMode(0, (short)nChannel);
            axitxview1.SetCovert((short)nChannel, false, true, 255, "", null);
            /*if (nPort == 0 || nChannel <= 0 ||
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
            }*/
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            uint pid = 0;
            uint currentProcessID = 0;

            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

            if (currentProcess != null)
                currentProcessID = (uint)currentProcess.Id;

            IntPtr handle = FindWindow(null, "WEBVIE~1");
            GetWindowThreadProcessId(handle, out pid);

            // 접속 실패하여 실패하였다는 메시지 창이 나타나면, 창을 닫고 타이머를 종료한다.
            if (handle != IntPtr.Zero && handle != this.Handle && pid == currentProcessID)
            {
                SendMessage(handle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                timer1.Stop();
            }

            // 접속이 성공하면 타이머를 종료한다.
            if (axitxview1.IsConnected())
            {
                timer1.Stop();
            }
        }

        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("ResizeEnd");
            //RecreateControl();
        }

        private void RecreateControl()
        {
            Size size = axitxview1.Size;
            this.Controls.Remove(axitxview1);
            axitxview1.Dispose();

            axitxview1 = new AxitxviewLib.Axitxview();
            this.axitxview1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axitxview1.Enabled = true;
            this.axitxview1.Location = new System.Drawing.Point(0, 0);
            this.axitxview1.Name = "axitxview1";
            this.axitxview1.Size = size;
            this.axitxview1.TabIndex = 2;

            this.Controls.Add(axitxview1);
            //Connect("192.168.122.10", 8521, 1, "ADMIN", "Secom112", "00115fa407eb");
        }
    }
}
