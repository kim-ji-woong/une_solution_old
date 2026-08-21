using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace UnE.Control.CCTVControl
{
#if _Divisys_
    public class DivisysCamera
    {
        private class EDNETP_EVENT_ATTR_DATA
        {
            public const int EDNETP_ATTR_CAMERA = 5;
        }

        private class EDNETP_ATTR_STATE_DATA
        {
            public const int EDNETP_STATE_OFFLINE = 0;
            public const int EDNETP_STATE_RESOLVE = 1;
            // 연결중인 상태
            public const int EDNETP_STATE_CONNECT = 2;
            public const int EDNETP_STATE_INIT = 3;
            // 로그인 시도중인 상태
            public const int EDNETP_STATE_LOGIN = 4;
            public const int EDNETP_STATE_PREPARE = 5;
            // 로그인 성공한 상태
            public const int EDNETP_STATE_ONLINE = 6;
        }

        private const int EDNETP_EVENT_STATE = 0;
        private const int EDNETP_EVENT_ATTR = 1;
        private const int EDNETP_EVENT_LOGIN = 2;
        private const int EDNETP_EVENT_CONNECT = 3;
        private const int EDNETP_EVENT_SHUTDOWN = 4;
        private const int EDNETP_EVENT_HOST_EVENT = 5;
        private const int EDNETP_EVENT_CHANNEL_STATE = 9;
        private const int EDNETP_EVENT_LOG_MESSAGE = 99;

        private AxednetpluginocxLib.AxEDNetPluginOCX axEDNetPluginOCX1 = null;
        private string m_strHost = "";
        private int m_nPort = 0;
        private int m_nChannel = 0;
        private string m_strID = "";
        private string m_strPW = "";
        private CCTVCtrl m_parent = null;

        public Size Size
        {
            get { return axEDNetPluginOCX1.Size; }
            set { axEDNetPluginOCX1.Size = value; }
        }

        public System.Windows.Forms.Control Control
        {
            get { return axEDNetPluginOCX1; }
        }

        public DivisysCamera(System.ComponentModel.ComponentResourceManager resources, CCTVCtrl parent)
        {
            m_parent = parent;

            this.axEDNetPluginOCX1 = new AxednetpluginocxLib.AxEDNetPluginOCX();
            ((System.ComponentModel.ISupportInitialize)(this.axEDNetPluginOCX1)).BeginInit();

            // 
            // axEDNetPluginOCX1
            // 
            this.axEDNetPluginOCX1.Enabled = true;
            this.axEDNetPluginOCX1.Location = new System.Drawing.Point(0, 0);
            this.axEDNetPluginOCX1.Name = "axEDNetPluginOCX1";
            this.axEDNetPluginOCX1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axEDNetPluginOCX1.OcxState")));
            this.axEDNetPluginOCX1.Size = new System.Drawing.Size(parent.Size.Width, parent.Size.Height);
            this.axEDNetPluginOCX1.onEvent += new AxednetpluginocxLib._IEDNetPluginOCXEvents_onEventEventHandler(this.axEDNetPluginOCX1_onEvent);
            m_parent.Controls.Add(axEDNetPluginOCX1);

            ((System.ComponentModel.ISupportInitialize)axEDNetPluginOCX1).EndInit();
        }

        public int Connect(string strURL, int nPort, int nChannel, string strID, string strPW)
        {
            if (axEDNetPluginOCX1 == null)
                return -1;

            if (m_strHost == strURL && m_nPort == nPort && m_nChannel == nChannel)
                return m_nChannel;

            m_strHost = strURL;
            m_nPort = nPort;
            m_nChannel = nChannel;
            m_strID = strID;
            m_strPW = strPW;

            if (m_strID.Length == 0 || m_strPW.Length == 0)
                return 0;

            string strLoginData = string.Format("{0}:{1}:{2}:{3}", m_strHost, m_nPort, strID, strPW);

            axEDNetPluginOCX1.sendEvent(EDNETP_EVENT_LOGIN, strLoginData);
            axEDNetPluginOCX1.sendEvent(EDNETP_EVENT_CONNECT, "");

            return m_nChannel;
        }

        public void Close()
        {
            if (axEDNetPluginOCX1 == null)
                return;

            axEDNetPluginOCX1.sendEvent(EDNETP_EVENT_SHUTDOWN, " ");
        }

        private void axEDNetPluginOCX1_onEvent(object sender, AxednetpluginocxLib._IEDNetPluginOCXEvents_onEventEvent e)
        {
            if (e.@event == EDNETP_EVENT_STATE)
            {
                int data;

                if (int.TryParse(e.data, out data))
                {
                    if (data == EDNETP_ATTR_STATE_DATA.EDNETP_STATE_ONLINE)
                    {
                        string strData = string.Format("{0}:{1}", EDNETP_EVENT_ATTR_DATA.EDNETP_ATTR_CAMERA, m_nChannel - 1);
                        axEDNetPluginOCX1.sendEvent(EDNETP_EVENT_ATTR, strData);

                        m_parent.OnConnected(this);
                    }
                }
            }
            else if (e.@event == EDNETP_EVENT_LOG_MESSAGE)
            {
                string strMessage = e.data.ToLower();

                if (strMessage.Contains("fail"))
                    m_parent.OnDisconnected(this);
            }
        }
    }
#endif
}
