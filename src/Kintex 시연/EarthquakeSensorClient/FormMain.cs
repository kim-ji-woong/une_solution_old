using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace EarthquakeSensorClient
{
    public partial class FormMain : Form
    {
        private string m_strSendMessage = "";
        private int m_nPortNo = 0;

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            int nPortNo = 0;

            /*if (!int.TryParse(textBoxPortNo.Text, out nPortNo))
            {
                textBoxPortNo.Focus();
                MessageBox.Show("Port 번호를 입력하세요.");
                return;
            }

            DateTime time = DateTime.Now;
            string strTime = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
            string strStation = textBoxStation.Text;
            string strLevel = textBoxAlarmLevel.Text;
            string strHPGA = textBoxHPGA.Text;
            string strTPGA = textBoxTPGA.Text;


            // 00000127Type=SEISMIC&DateTime=20110225081025&Station=192.168.10.101 SS_ES1&Level=1&Source=1&HPGA =0000.000123&TPGA =0000.000248&MMI=01
            string strMessage = "00000127Type=SEISMIC&DateTime=" + strTime + "&Station=" + strStation + "&Level=" + strLevel + "&Source=1&HPGA=" + strHPGA + "&TPGA=" + strTPGA + "&MMI=02";
            //string strMessage = strTime + "\t" + strStation + "\t" + strLevel + "\t" + "1" + "\t" + strHPGA + "\t" + strTPGA;*/
            string strMessage = GetMessage(ref nPortNo);

            /*Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress broadcast = IPAddress.Parse("127.0.0.1");

            byte[] sendbuf = Encoding.UTF8.GetBytes(strMessage);
            IPEndPoint ep = new IPEndPoint(broadcast, nPortNo);

            s.SendTo(sendbuf, ep);*/
            SendMessage(strMessage, nPortNo);
        }

        private void SendMessage(string strMessage, int nPortNo)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress broadcast = IPAddress.Parse("127.0.0.1");

            byte[] sendbuf = Encoding.UTF8.GetBytes(strMessage);
            IPEndPoint ep = new IPEndPoint(broadcast, nPortNo);

            s.SendTo(sendbuf, ep);
        }

        private string GetMessage(ref int nPortNo)
        {
            if (!int.TryParse(textBoxPortNo.Text, out nPortNo))
            {
                textBoxPortNo.Focus();
                MessageBox.Show("Port 번호를 입력하세요.");
                return "";
            }

            DateTime time = DateTime.Now;
            string strTime = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
            string strStation = textBoxStation.Text;
            string strLevel = textBoxAlarmLevel.Text;
            string strHPGA = textBoxHPGA.Text;
            string strTPGA = textBoxTPGA.Text;

            string strPGA = "1";
            string strGal = strPGA == "1" ? strTPGA : strHPGA;

            int nIntensity = GetIntensity(strGal);
            string strMMI = string.Format("&MMI={0:00}", nIntensity);

            // 00000127Type=SEISMIC&DateTime=20110225081025&Station=192.168.10.101 SS_ES1&Level=1&Source=1&HPGA =0000.000123&TPGA =0000.000248&MMI=01
            string strMessage = "00000127Type=SEISMIC&DateTime=" + strTime + "&Station=" + strStation + "&Level=" + strLevel + "&Source=" + strPGA + "&HPGA=" + strHPGA + "&TPGA=" + strTPGA + strMMI;
            //string strMessage = strTime + "\t" + strStation + "\t" + strLevel + "\t" + "1" + "\t" + strHPGA + "\t" + strTPGA;
            return strMessage;
        }

        private int GetIntensity(string strGal)
        {
            float fGal = 0.0f;

            if (float.TryParse(strGal, out fGal))
            {
                if (fGal < 1.0f)
                    return 1;
                else if (fGal >= 1.0f && fGal < 2.5f)
                    return 2;
                else if (fGal >= 2.5f && fGal < 5.0f)
                    return 3;
                else if (fGal >= 5.0f && fGal < 10.0f)
                    return 4;
                else if (fGal >= 10.0f && fGal < 25.0f)
                    return 5;
                else if (fGal >= 25.0f && fGal < 50.0f)
                    return 6;
                else if (fGal >= 50.0f && fGal < 100.0f)
                    return 7;
                else if (fGal >= 100.0f && fGal < 250.0f)
                    return 8;
                else if (fGal >= 250.0f && fGal < 500.0f)
                    return 9;
                else if (fGal >= 500.0f && fGal < 750.0f)
                    return 10;
                else if (fGal >= 750.0f && fGal < 980.0f)
                    return 11;
                else if (fGal >= 980.0f)
                    return 12;
            }

            return -1;
        }

        private void btnKeepGoing_Click(object sender, EventArgs e)
        {
            btnKeepGoing.Enabled = false;

            m_strSendMessage = GetMessage(ref m_nPortNo);
            timerSend.Start();

            btnStop.Enabled = true;
        }

        private void timerSend_Tick(object sender, EventArgs e)
        {
            SendMessage(m_strSendMessage, m_nPortNo);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            timerSend.Stop();
            btnKeepGoing.Enabled = true;
        }
    }
}
