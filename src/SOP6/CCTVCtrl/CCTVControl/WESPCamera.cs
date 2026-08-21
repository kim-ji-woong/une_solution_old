using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace UnE.Control.CCTVControl
{
#if _WESP_
    public class WESPCamera
    {
        private AxWESPMONITORLib.AxWESPMonitorCtrl axWESPMonitorCtrl1 = null;
        private string m_strIP = "";
        private short m_nPortNo = 0;
        private string m_strUserID = "";
        private string m_strPW = "";
        private short m_nChannel = 0;
        private short m_nResolution = (short)WESPMONITORLib.EnumResolution.RESOLUTION_NORMAL;
        private short m_nFrameRate = (short)WESPMONITORLib.EnumFrameRate.FRAMERATE10;
        private CCTVCtrl m_parent = null;
        private bool m_isConnected = false;

        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public Size Size
        {
            get { return axWESPMonitorCtrl1.Size; }
            set { axWESPMonitorCtrl1.Size = value; }
        }

        public WESPCamera(System.ComponentModel.ComponentResourceManager resources, CCTVCtrl parent)
        {
            m_parent = parent;

            this.axWESPMonitorCtrl1 = new AxWESPMONITORLib.AxWESPMonitorCtrl();
            ((System.ComponentModel.ISupportInitialize)(this.axWESPMonitorCtrl1)).BeginInit();

            // 
            // axWESPMonitorCtrl1
            // 
            this.axWESPMonitorCtrl1.Enabled = true;
            this.axWESPMonitorCtrl1.Location = new System.Drawing.Point(0, 0);
            this.axWESPMonitorCtrl1.Name = "axWESPMonitorCtrl1";
            this.axWESPMonitorCtrl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWESPMonitorCtrl1.OcxState")));
            this.axWESPMonitorCtrl1.Size = new System.Drawing.Size(parent.Size.Width, parent.Size.Height);
            this.axWESPMonitorCtrl1.TabIndex = 0;
            this.axWESPMonitorCtrl1.AckReceived += new AxWESPMONITORLib._IWESPMonitorCtrlEvents_AckReceivedEventHandler(this.axWESPMonitorCtrl1_AckReceived);
            this.axWESPMonitorCtrl1.ErrorReceived += new AxWESPMONITORLib._IWESPMonitorCtrlEvents_ErrorReceivedEventHandler(this.axWESPMonitorCtrl1_ErrorReceived);
            //this.axWESPMonitorCtrl1.ServerTimeReceived += new AxWESPMONITORLib._IWESPMonitorCtrlEvents_ServerTimeReceivedEventHandler(this.axWESPMonitorCtrl1_ServerTimeReceived);
            m_parent.Controls.Add(axWESPMonitorCtrl1);

            ((System.ComponentModel.ISupportInitialize)axWESPMonitorCtrl1).EndInit();
        }

        private void axWESPMonitorCtrl1_AckReceived(object sender, AxWESPMONITORLib._IWESPMonitorCtrlEvents_AckReceivedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("AckReceived : " + e.ackCode);

            switch (e.ackCode)
            {
                case (int)WESPMONITORLib.EnumAckCode.ACK_CONNECT:
                    System.Diagnostics.Trace.WriteLine("Received ACK : Connect OK");
                    PlayVideo();
                    break;
                case (int)WESPMONITORLib.EnumAckCode.ACK_CONTENT_INFO:
                    System.Diagnostics.Trace.WriteLine("Received ACK : Content Info");
                    break;
                case (int)WESPMONITORLib.EnumAckCode.ACK_DISCONNECT:
                    System.Diagnostics.Trace.WriteLine("Received ACK : Disconnect");
                    SetDisconnect();
                    break;
                case (int)WESPMONITORLib.EnumAckCode.ACK_PLAYVIDEO:
                    System.Diagnostics.Trace.WriteLine("Received ACK : Play Video");
                    SetConnect();
                    break;
                case (int)WESPMONITORLib.EnumAckCode.ACK_STOPVIDEO:
                    System.Diagnostics.Trace.WriteLine("Received ACK : Stop Video");
                    SetDisconnect();
                    break;
                default:
                    System.Diagnostics.Trace.WriteLine("Received ACK : Others");
                    break;
            }
        }

        private void axWESPMonitorCtrl1_ErrorReceived(object sender, AxWESPMONITORLib._IWESPMonitorCtrlEvents_ErrorReceivedEvent e)
        {
            System.Diagnostics.Trace.WriteLine("ErrorReceived : " + e.errorCode);
            SetDisconnect();

            switch (e.errorCode)
            {
                case (int)WESPMONITORLib.EnumErrorCodeMonitor.MON_ERR_CONNECT_FAIL:
                    System.Diagnostics.Trace.WriteLine("Received Error : Connect Fail");
                    break;
                case (int)WESPMONITORLib.EnumErrorCodeMonitor.MON_ERR_UNAUTH_USER:
                    System.Diagnostics.Trace.WriteLine("Received Error : Unauthorized User");
                    break;
                default:
                    System.Diagnostics.Trace.WriteLine("Received Error : Others");
                    break;
            }
        }

        private void PlayVideo()
        {
            axWESPMonitorCtrl1.PlayVideo(m_nChannel, m_nResolution, m_nFrameRate);
        }

        public void Connect(string strIP, short nPort, short nChannel, string strID, string strPW)
        {
            if (axWESPMonitorCtrl1 == null)
                return;

            m_strIP = strIP;
            m_nPortNo = nPort;
            m_nChannel = nChannel;
            m_strUserID = strID;
            m_strPW = strPW;

            axWESPMonitorCtrl1.Connect(m_strIP, m_nPortNo, m_strUserID, m_strPW);
        }

        public void Close()
        {
            if (axWESPMonitorCtrl1 == null)
                return;

            try
            {
                axWESPMonitorCtrl1.Disconnect();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            SetDisconnect();
        }

        private void SetConnect()
        {
            if (m_parent != null)
            {
                m_parent.OnConnected(this);
                this.Size = m_parent.Size;
            }

            m_isConnected = true;
        }

        private void SetDisconnect()
        {
            if (m_parent != null)
                m_parent.OnDisconnected(this);

            m_isConnected = false;
        }
    }
#endif
}
