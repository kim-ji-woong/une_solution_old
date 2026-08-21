using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCTVViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.axNVS4Viewer.ServerIP = "192.168.0.90";
            this.axNVS4Viewer.ServerControlPort = 7777;
            this.axNVS4Viewer.ServerVideoPort = 7778;
            this.axNVS4Viewer.ServerAudioTransmitPort = 7779;
            this.axNVS4Viewer.ServerAudioReceivePort = 7780;
            this.axNVS4Viewer.UserID = "admin";
            this.axNVS4Viewer.UserPassword = "admin";
            this.axNVS4Viewer.ChipVersion = 3002;

            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.axNVS4Viewer.ConnectStatus == NVS4Viewer2Lib._WinsockStatus_Type.wConnected)
                this.axNVS4Viewer.ServerDisconnect();
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            if (this.axNVS4Viewer.ConnectStatus == NVS4Viewer2Lib._WinsockStatus_Type.wDisconnected)
            {
                if (this.axNVS4Viewer.ServerConnect())
                {
                    this.axNVS4Viewer.VideoChannel = 0;
                    this.axNVS4Viewer.ChannelView = false;
                    this.axNVS4Viewer.ImageStretch = true;
                    this.axNVS4Viewer.ImageZoomIn = false;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return false;
        }

    }
}