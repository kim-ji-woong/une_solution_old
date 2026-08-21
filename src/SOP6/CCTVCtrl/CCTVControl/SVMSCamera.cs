using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace UnE.Control.CCTVControl
{
#if _SVMS_
    public class SVMSCamera
    {
        public enum Direction { LEFT = 0, RIGHT, UP, DOWN };

        private AxRTSPLiveScreenLib.AxRTSPLiveScreen axRTSPLiveScreen = null;
        private int m_nCameraIndex = -1;
        private string m_strURL = "";
        private int m_nPort = -1;
        private string m_strGUID = "";
        private CCTVCtrl m_parent = null;

        public Size Size
        {
            get { return axRTSPLiveScreen.Size; }
            set { axRTSPLiveScreen.Size = value; }
        }

        public System.Windows.Forms.Control Control
        {
            get { return axRTSPLiveScreen; }
        }

        public string GUID
        {
            get { return m_strGUID; }
            set { m_strGUID = value; }
        }

        public SVMSCamera(System.ComponentModel.ComponentResourceManager resources, CCTVCtrl parent)
        {
            m_parent = parent;
            
            axRTSPLiveScreen = new AxRTSPLiveScreenLib.AxRTSPLiveScreen();
            ((System.ComponentModel.ISupportInitialize)axRTSPLiveScreen).BeginInit();

            // axRTSPLiveScreen
            axRTSPLiveScreen.Enabled = true;
            axRTSPLiveScreen.Location = new Point(0, 0);
            axRTSPLiveScreen.Name = "axRTSPLiveScreen";
            axRTSPLiveScreen.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axRTSPLiveScreen.OcxState")));
            axRTSPLiveScreen.Size = m_parent.Size;
            m_parent.Controls.Add(axRTSPLiveScreen);

            ((System.ComponentModel.ISupportInitialize)axRTSPLiveScreen).EndInit();
        }

        public int Connect(string strURL, int nPort)
        {
            if (axRTSPLiveScreen == null)
                return -1;

            if (m_strURL == strURL && m_nPort == nPort)
                return m_nCameraIndex;

            m_nCameraIndex = axRTSPLiveScreen.OpenRTSPLiveScreen(strURL, (short)nPort, "", "", 1);

            if (m_nCameraIndex > 0)
            {
                m_strURL = strURL;
                m_nPort = nPort;
            }

            return m_nCameraIndex;
        }

        public void Close()
        {
            if (axRTSPLiveScreen == null)
                return;

            if (m_nCameraIndex > 0)
            {
                axRTSPLiveScreen.CloseRTSPLiveScreen(m_nCameraIndex);
                axRTSPLiveScreen.Refresh();
            }

            m_nCameraIndex = -1;
            m_strURL = "";
            m_nPort = -1;

            if (m_parent != null)
                m_parent.Controls.Remove(axRTSPLiveScreen);

            axRTSPLiveScreen.Dispose();
            axRTSPLiveScreen = null;
        }

        public void ZoomIn()
        {
        }

        public void ZoomOut()
        {
        }

        public void Move(Direction dir)
        {
        }

        public void Pause()
        {
            if (axRTSPLiveScreen != null && m_nCameraIndex > 0)
                axRTSPLiveScreen.CloseRTSPLiveScreen(m_nCameraIndex);
        }

        public void Resume()
        {
            if (axRTSPLiveScreen != null && m_nCameraIndex > 0)
                m_nCameraIndex = axRTSPLiveScreen.OpenRTSPLiveScreen(m_strURL, (short)m_nPort, "", "", 1);
        }

        public void OnMouseUp(System.Windows.Forms.MouseButtons button)
        {

        }
    }
#endif
}
