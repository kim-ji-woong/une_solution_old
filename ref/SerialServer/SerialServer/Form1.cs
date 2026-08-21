using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialServer
{
    public partial class Form1 : Form
    {
        private SerialManager sm = new SerialManager();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnComConnect_Click(object sender, EventArgs e)
        {
            sm.Port = txtComPort.Text;
            sm.Connect();
        }

        private void btnBeginServer_Click(object sender, EventArgs e)
        {
            sm.BeginServer();
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            sm.StopServer();
        }


        private int m_nStartTagNo = 0;
        private int m_nEndTagNo = 0;
        private int GetTagCount(string szBegin, string szEnd)
        {
            m_nStartTagNo = Convert.ToInt32(szBegin);
            m_nEndTagNo = Convert.ToInt32(szEnd);


            int nResult = (m_nEndTagNo - m_nStartTagNo) + 1;
            return nResult;

        }
        private void btnTagValue_Click(object sender, EventArgs e)
        {
            // 02 30 30 2D 31 30 2D 31 34 46 34 03

            int nCount = GetTagCount(txtBeginTagNo.Text, txtEndTagNo.Text);

            for( int i = 0; i < nCount ; i++)
            {
                int nTag = m_nStartTagNo + i;
                string szTag = string.Format("{0:D3}", nTag);
                byte [] buf = new byte[12];
                buf[0] = 0x02;
                buf[1] = 0x30;
                buf[2] = 0x30;

                buf[3] = 0x2D;
                // value
                buf[4] = 0x31;

                buf[5] = (byte)szTag[0];

                buf[6] = 0x2D;
                buf[7] = (byte)szTag[1];
                buf[8] = (byte)szTag[2];

                if(txtTagValue.Text == "0")
                    buf[9] = (byte)('F');
                else
                    buf[9] = (byte)('N');

                buf[11] = 0x03;
                sm.SendBytes(buf);

                System.Threading.Thread.Sleep(2000);
            } 
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] buf = new byte[12];
            buf[0] = 0x02;
            buf[1] = 0x30;
            buf[2] = 0x30;

            buf[3] = 0x2D;
            // value
            buf[4] = 0x31;

            buf[5] = 0x30;

            buf[6] = 0x2D;
            buf[7] = 0x30;
            buf[8] = 0x30;

            buf[9] = (byte)('R');

            buf[11] = 0x03;
            sm.SendBytes(buf);
        }
    }
}
