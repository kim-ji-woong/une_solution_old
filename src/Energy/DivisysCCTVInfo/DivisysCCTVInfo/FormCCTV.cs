using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivisysCCTVInfo
{
    public partial class FormCCTV : Form
    {
        private const int EDNETP_EVENT_STATE = 0;
        private const int EDNETP_EVENT_ATTR = 1;
        private const int EDNETP_EVENT_LOGIN = 2;
        private const int EDNETP_EVENT_CONNECT = 3;
        private const int EDNETP_EVENT_SHUTDOWN = 4;
        private const int EDNETP_EVENT_HOST_EVENT = 5;
        private const int EDNETP_EVENT_CHANNEL_STATE = 9;

        private IEventOwner m_eventOwner = null;
        private FormMain m_frmMain = null;

        // [Sample Data]
        // Host : demo.nvrsw.com
        // Port : 8081
        // ID : guest
        // PW : guest
        public FormCCTV(string strHost, string strPort, string strID, string strPW, IEventOwner eventOwner, FormMain frmMain)
        {
            InitializeComponent();

            m_eventOwner = eventOwner;
            m_frmMain = frmMain;

            string strLoginData = string.Format("{0}:{1}:{2}:{3}", strHost, strPort, strID, strPW);

            try
            {
                // Login
                axEDNetPluginOCX1.sendEvent(EDNETP_EVENT_LOGIN, strLoginData);
                axEDNetPluginOCX1.sendEvent(EDNETP_EVENT_CONNECT, "");
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Connection Fail : " + e.Message);
            }

            // Logout
            //axEDNetPluginOCX1.sendEvent(EDNETP_EVENT_SHUTDOWN, " ");
            }

        private void axEDNetPluginOCX1_onEvent(object sender, AxednetpluginocxLib._IEDNetPluginOCXEvents_onEventEvent e)
        {
            if (e.@event == EDNETP_EVENT_STATE)
            {
                int data;

                if (int.TryParse(e.data, out data))
                {
                    if (m_frmMain != null)
                        m_frmMain.SetStatus(EDNETP_ATTR_STATE_DATA.GetStatusString(data));
                }
            }

            System.Diagnostics.Trace.WriteLine(string.Format("onEvent({0}) : {1}", e.@event, e.data));

            if (e.@event == EDNETP_EVENT_HOST_EVENT ||
                e.@event == EDNETP_EVENT_CHANNEL_STATE)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            {
                if (m_eventOwner != null)
                    m_eventOwner.AddEvent(e.@event, e.data);
                //m_frmEventViewer.AddEvent(false, e.@event, e.data);
                //System.Diagnostics.Trace.WriteLine(string.Format("onEvent({0}) : {1}", e.@event, e.data));
            }
            else
            {
                //m_frmEventViewer.AddEvent(true, e.@event, e.data);
            }
        }

        private void tsMenuChannel_Click(object sender, EventArgs e)
        {
            int nChannel = -1;

            if (sender == tsMenuChannel1)
                nChannel = 0;
            else if (sender == tsMenuChannel2)
                nChannel = 1;
            else if (sender == tsMenuChannel3)
                nChannel = 2;
            else if (sender == tsMenuChannel4)
                nChannel = 3;
            else if (sender == tsMenuChannel5)
                nChannel = 4;
            else if (sender == tsMenuChannel6)
                nChannel = 5;
            else if (sender == tsMenuChannel7)
                nChannel = 6;
            else if (sender == tsMenuChannel8)
                nChannel = 7;
            else if (sender == tsMenuChannel9)
                nChannel = 8;
            else if (sender == tsMenuChannel10)
                nChannel = 9;
            else if (sender == tsMenuChannel11)
                nChannel = 10;
            else if (sender == tsMenuChannel12)
                nChannel = 11;
            else if (sender == tsMenuChannel13)
                nChannel = 12;
            else if (sender == tsMenuChannel14)
                nChannel = 13;
            else if (sender == tsMenuChannel15)
                nChannel = 14;
            else if (sender == tsMenuChannel16)
                nChannel = 15;
            else
                return;

            string strData = string.Format("{0}:{1}", EDNETP_EVENT_ATTR_DATA.EDNETP_ATTR_CAMERA, nChannel);
            axEDNetPluginOCX1.sendEvent(EDNETP_EVENT_ATTR, strData);
        }

        private void tsMenuDisconnect_Click(object sender, EventArgs e)
        {
            // Logout
            axEDNetPluginOCX1.sendEvent(EDNETP_EVENT_SHUTDOWN, " ");
        }
    }

    public interface IEventOwner
    {
        void AddEvent(int nEventCode, string strData);
    }
}
