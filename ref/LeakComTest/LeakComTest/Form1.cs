using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LeakComTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // reset
            if (m_nSelectedUnit >= 0)
            {                
                if (provider.IsConnected == false)
                {
                    byte[] datas = new byte[12];
                    datas[0] = 0x02;
                    datas[1] = 0x00;
                    datas[2] = 0x00;
                    datas[3] = 0x00;
                    datas[4] = 0x00;
                    datas[5] = 0x06;
                    datas[6] = (byte)m_nSelectedUnit;
                    datas[7] = 0x06;
                    datas[8] = 0x00;
                    datas[9] = 0x00;
                    datas[10] = 0x00;
                    datas[11] = 0x00;

                    // send 0x0001 input register query
                    provider.Send(datas, 0, 12);
                }
            }            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int nIdx = comboBox1.SelectedIndex;
            if( nIdx >= 0)
            {
                int nUnitID = nIdx + 1;


                if (provider.IsConnected == false)
                {
                    byte[] datas = new byte[12];
                    datas[0] = 0x01;
                    datas[1] = 0x00;
                    datas[2] = 0x00;
                    datas[3] = 0x00;
                    datas[4] = 0x00;
                    datas[5] = 0x06;
                    datas[6] = (byte)nUnitID;
                    datas[7] = 0x04;
                    datas[8] = 0x00;
                    datas[9] = 0x01;
                    datas[10] = 0x00;
                    datas[11] = 0x01;

                    // send 0x0001 input register query
                    provider.Send(datas, 0, 12);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 부저
            if (m_nSelectedUnit >= 0)
            {
                if (provider.IsConnected == false)
                {
                    byte[] datas = new byte[12];
                    datas[0] = 0x02;
                    datas[1] = 0x00;
                    datas[2] = 0x00;
                    datas[3] = 0x00;
                    datas[4] = 0x00;
                    datas[5] = 0x06;
                    datas[6] = (byte)m_nSelectedUnit;
                    datas[7] = 0x06;
                    datas[8] = 0x00;
                    datas[9] = 0x08;
                    datas[10] = 0x00;
                    datas[11] = 0x00;

                    // send 0x0001 input register query
                    provider.Send(datas, 0, 12);
                }
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 부저
            if (m_nSelectedUnit >= 0)
            {
                if (provider.IsConnected == false)
                {
                    byte[] datas = new byte[12];
                    datas[0] = 0x02;
                    datas[1] = 0x00;
                    datas[2] = 0x00;
                    datas[3] = 0x00;
                    datas[4] = 0x00;
                    datas[5] = 0x06;
                    datas[6] = (byte)m_nSelectedUnit;
                    datas[7] = 0x06;
                    datas[8] = 0x00;
                    datas[9] = 0x08;
                    datas[10] = 0x00;
                    datas[11] = 0x01;

                    // send 0x0001 input register query
                    provider.Send(datas, 0, 12);
                }
            } 
        }

        private int m_nSelectedUnit = -1;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            int nIdx = comboBox1.SelectedIndex;
            if (nIdx >= 0)
            {
                m_nSelectedUnit = nIdx + 1;
            }

        }

        LeakSensorClientProvider provider = new LeakSensorClientProvider();

        private void button4_Click(object sender, EventArgs e)
        {
            string szIP = textBox1.Text;
            if (provider.IsConnected == false)
            {
                button4.Text = "끊기";
                provider.IPAddress = szIP;
                provider.BeginServer();
            }
            else
            {
                button4.Text = "접속";
                provider.StopServer();
            }
        }

    }
}
