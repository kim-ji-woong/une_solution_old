using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCTV_WESP_Sample
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            labelStatus.Text = "";
            labelServerTime.Text = "";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string strIP = textBoxIP.Text.Trim();
            string strPort = textBoxPort.Text.Trim();
            string strUserID = textBoxUserID.Text.Trim();
            string strPW = textBoxPW.Text.Trim();

            if (strIP.Length == 0)
            {
                MessageBox.Show("IP를 입력하세요.");
                return;
            }

            if (strPort.Length == 0)
            {
                MessageBox.Show("Port를 입력하세요.");
                return;
            }

            if (strUserID.Length == 0)
            {
                MessageBox.Show("사용자 ID를 입력하세요.");
                return;
            }

            if (strPW.Length == 0)
            {
                MessageBox.Show("비밀번호를 입력하세요.");
                return;
            }

            short nPort;

            if (short.TryParse(strPort, out nPort) == false || nPort <= 0)
            {
                MessageBox.Show("Port는 0보다 큰 정수를 입력해야만 합니다.");
                return;
            }

            axWESPMonitorCtrl1.Connect(strIP, nPort, strUserID, strPW);
        }

        private void axWESPMonitorCtrl1_AckReceived(object sender, AxWESPMONITORLib._IWESPMonitorCtrlEvents_AckReceivedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("AckReceived : " + e.ackCode);

            switch (e.ackCode)
            {
                case (int)WESPMONITORLib.EnumAckCode.ACK_CONNECT:
                    labelStatus.Text = "Received ACK : Connect OK";
                    //bMonitorSizeChanged = false;
                    break;
                case (int)WESPMONITORLib.EnumAckCode.ACK_CONTENT_INFO:
                    labelStatus.Text = "Received ACK : Content Info";
                    //bMonitorSizeChanged = false;
                    break;
                case (int)WESPMONITORLib.EnumAckCode.ACK_DISCONNECT:
                    labelStatus.Text = "Received ACK : Disconnect";
                    //bMonitorSizeChanged = false;
                    break;
                case (int)WESPMONITORLib.EnumAckCode.ACK_PLAYVIDEO:
                    labelStatus.Text = "Received ACK : Play Video";
                    //bMonitorSizeChanged = false;
                    break;
                case (int)WESPMONITORLib.EnumAckCode.ACK_STOPVIDEO:
                    labelStatus.Text = "Received ACK : Stop Video";
                    //bMonitorSizeChanged = false;
                    break;
                default:
                    labelStatus.Text = "Received ACK : Others";
                    break;
            }
        }

        private void axWESPMonitorCtrl1_ErrorReceived(object sender, AxWESPMONITORLib._IWESPMonitorCtrlEvents_ErrorReceivedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("ErrorReceived : " + e.errorCode);

            switch (e.errorCode)
            {
                case (int)WESPMONITORLib.EnumErrorCodeMonitor.MON_ERR_CONNECT_FAIL:
                    labelStatus.Text = "Received Error : Connect Fail";
                    break;
                case (int)WESPMONITORLib.EnumErrorCodeMonitor.MON_ERR_UNAUTH_USER:
                    labelStatus.Text = "Received Error : Unauthorized User";
                    break;
                default:
                    labelStatus.Text = "Received Error : Others";
                    break;
            }
        }

        private void axWESPMonitorCtrl1_ServerTimeReceived(object sender, AxWESPMONITORLib._IWESPMonitorCtrlEvents_ServerTimeReceivedEvent e)
        {
            string msg;

            msg = string.Format("received time : {0}", e.pIST.nTimeSec);

            labelServerTime.Text = msg;
        }

        private void axWESPMonitorCtrl1_SizeChanged(object sender, EventArgs e)
        {

        }

        private void btnPlayVideo_Click(object sender, EventArgs e)
        {
            short sChannel = 0, sFrameRate = 0, sResolution = 0;
            switch (cboFR.SelectedItem.ToString())
            {
                case "Snapshot":
                    sFrameRate = (short)WESPMONITORLib.EnumFrameRate.FRAMERATE_SNAPSHOT;
                    break;
                case "1":
                    sFrameRate = (short)WESPMONITORLib.EnumFrameRate.FRAMERATE1;
                    break;
                case "5":
                    sFrameRate = (short)WESPMONITORLib.EnumFrameRate.FRAMERATE5;
                    break;
                case "10":
                    sFrameRate = (short)WESPMONITORLib.EnumFrameRate.FRAMERATE10;
                    break;
                case "15":
                    sFrameRate = (short)WESPMONITORLib.EnumFrameRate.FRAMERATE15;
                    break;
                case "30":
                    sFrameRate = (short)WESPMONITORLib.EnumFrameRate.FRAMERATE30;
                    break;
                default:
                    MessageBox.Show("Invalid FrameRate");
                    break;
            }
            switch (cboResolution.SelectedItem.ToString())
            {
                case "Lowest":
                    sResolution = (short)WESPMONITORLib.EnumResolution.RESOLUTION_LOWEST;
                    break;
                case "Low":
                    sResolution = (short)WESPMONITORLib.EnumResolution.RESOLUTION_LOW;
                    break;
                case "Normal":
                    sResolution = (short)WESPMONITORLib.EnumResolution.RESOLUTION_NORMAL;
                    break;
                case "High":
                    sResolution = (short)WESPMONITORLib.EnumResolution.RESOLUTION_HIGH;
                    break;
                case "Highest":
                    sResolution = (short)WESPMONITORLib.EnumResolution.RESOLUTION_HIGHEST;
                    break;
                default:
                    MessageBox.Show("Invalid Resolution");
                    return;
            }
            sChannel = short.Parse(cboChannels.SelectedItem.ToString());

            axWESPMonitorCtrl1.PlayVideo(
                sChannel,
                sResolution,
                sFrameRate
            );
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            cboChannels.SelectedItem = "1";
            cboFR.SelectedItem = "10";
            cboResolution.SelectedItem = "Normal";
        }
    }
}
