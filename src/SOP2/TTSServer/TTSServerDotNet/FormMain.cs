using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;



namespace TTSServerDotNet
{

    public partial class FormMain : Form
    {

        private DBManager mDBManager = null;
        private bool bProcess = false;
        
        public FormMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mDBManager = new DBManager();
            timer1.Interval = 2000;
            timer1.Start();
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BroadcastMessage message = new BroadcastMessage();
            message.Message = textBox1.Text;
            message.RepeatCount = nRepeat;
            message.UseSiren = bUseSiren;
            message.PlayOption = 1;
            
            mDBManager.AddMessage(message);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BroadcastMessage message = new BroadcastMessage();
            message.Message = "방송메니저-끝";
            message.RepeatCount = nRepeat;
            message.UseSiren = bUseSiren;
            message.PlayOption = 0;

            mDBManager.AddMessage(message);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BroadcastMessage message = new BroadcastMessage();
            message.Message = "방송메니저-일시중지";
            message.RepeatCount = nRepeat;
            message.UseSiren = bUseSiren;
            message.PlayOption = 3;
            mDBManager.AddMessage(message);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BroadcastMessage message = new BroadcastMessage();
            message.Message = "방송메니저-계속";
            message.RepeatCount = nRepeat;
            message.UseSiren = bUseSiren;
            message.PlayOption = 2;
            mDBManager.AddMessage(message);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int nState = mDBManager.ReadHeartBeat();
            if (nState == -1)
            {
                label1.Text = "현재상태 : 사용불가";
            }
            if (nState == 1)
            {
                label1.Text = "현재상태 : 대기";
            }
            if (nState == 2)
            {
                label1.Text = "현재상태 : 방송중";
            }
            if (nState == 3)
            {
                label1.Text = "현재상태 : 정지";
            }
            if (nState == 4)
            {
                label1.Text = "현재상태 : 일시 정지";
            }

            if (nState == 5)
            {
                label1.Text = "현재상태 : 무한 반복";
            }        
        }

        private bool bUseSiren = false;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bUseSiren = checkBox1.Checked;
        }
        private int nRepeat = 1;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIdx = comboBox1.SelectedIndex;
            if (nIdx >= 0)
            {
                nRepeat = nIdx + 1;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
