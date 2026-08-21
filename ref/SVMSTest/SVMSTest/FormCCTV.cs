using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using S1SVMSSDKv2.Model.Device;

namespace SVMSTest
{
    public partial class FormCCTV : Form
    {
        private DeviceCamera m_camera = null;

        public FormCCTV(DeviceCamera camera)
        {
            InitializeComponent();
            m_camera = camera;

            if (m_camera != null)
                this.Text = m_camera.CameraName;
        }

        private void FormCCTV_Load(object sender, EventArgs e)
        {
            ConnectCamera();
        }

        private void ConnectCamera()
        {
            if (m_camera == null)
                return;

            string strCCTVIP = m_camera.CameraIPAddress;
            string strRTSP = "rtsp://";
            string strLower = m_camera.ConnectURL.ToLower();

            int nIndex1 = strLower.IndexOf(strRTSP);
            int nIndex2 = strLower.IndexOf(strCCTVIP);
            string strConnection = m_camera.ConnectURL;

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                string strServer = strConnection.Substring(strRTSP.Length, nIndex2 - strRTSP.Length);

                if (strServer.Contains(':') == false)
                {
                    if (strServer.EndsWith("/"))
                        strConnection = strRTSP + strServer.Substring(0, strServer.Length - 1) + ":" + m_camera.CameraRTSPPort.ToString() + "/" + strCCTVIP;
                    else
                        strConnection = strRTSP + strServer + ":" + m_camera.CameraRTSPPort.ToString() + "/" + strCCTVIP;
                }
            }
            else
                return;

            axRTSPLiveScreen1.OpenRTSPLiveScreen(strConnection, (short)m_camera.CameraRTSPPort, "", "", 1);
        }
    }
}
