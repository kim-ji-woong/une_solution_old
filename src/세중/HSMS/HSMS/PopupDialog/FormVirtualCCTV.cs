using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace HSMS
{
    public partial class FormVirtualCCTV : Form
    {
        private IGraphBuilder m_graphBuilder = null;
        private IMediaControl m_mediaControl = null;
        private IVideoWindow m_videoWindow = null;
        private IFormVirtualCCTVOwner m_cctvOwner = null;

        public CCTVViewer.CCTV m_cctv = null;

        public string CCTVName
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public IFormVirtualCCTVOwner CCTVOwner
        {
            get { return m_cctvOwner; }
            set { m_cctvOwner = value; }
        }

        public FormVirtualCCTV(CCTVViewer.CCTV cctv)
        {
            InitializeComponent();

            m_cctv = cctv;
            CCTVName = cctv.CameraName;
        }

        private void FormVirtualCCTV_Load(object sender, EventArgs e)
        {
            if (m_cctv == null) return;

            CCTVManager.Instance.ListLiveCCTV.Add(CCTVCrtl);

            // CCTV Info 세팅
            CCTVCrtl.SetCCTVInfo(m_cctv);

            timer.Start();
        }

        private void FormVirtualCCTV_FormClosing(object sender, FormClosingEventArgs e)
        {
            CCTVManager.Instance.ListLiveCCTV.Remove(CCTVCrtl);
        }

        private void FormVirtualCCTV_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CCTVOwner != null)
                CCTVOwner.SetLocation(this.Location.X, this.Location.Y);

            if (m_graphBuilder != null)
            {
                Marshal.ReleaseComObject(m_graphBuilder);
            }

            if (m_mediaControl != null)
            {
                Marshal.ReleaseComObject(m_mediaControl);
            }

            base.OnClosed(e);
        }

        private void FormVirtualCCTV_Resize(object sender, EventArgs e)
        {
            if (m_videoWindow != null)
            {
                m_videoWindow.put_Width(this.Width);
                m_videoWindow.put_Height(this.Height);
            }
        }

        // Return 값 : 생성된 이미지 파일의 전체 경로
        //             파일 생성에 실패하면 null을 리턴한다.
        public string CaptureCCTVImage(string strFileName, bool overwrite = false)
        {
            return this.CCTVCrtl.CaptureScreen(CCTVManager.Instance.CapturePath, strFileName, overwrite);
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            if (CCTVCrtl.ConnectStatus == NVS4Viewer2Lib._WinsockStatus_Type.wDisconnected)
            {
                CCTVCrtl.Refresh();
                CCTVCrtl.ConnectCCTV();
            }

            switch (CCTVCrtl.ConnectStatus)
            {
                case NVS4Viewer2Lib._WinsockStatus_Type.wConnected:
                    lblStatus.Visible = false;
                    CCTVName = m_cctv.CameraName;
                    break;
                case NVS4Viewer2Lib._WinsockStatus_Type.wConnecting:
                case NVS4Viewer2Lib._WinsockStatus_Type.wRetryConnecting:
                    lblStatus.Visible = true;
                    lblStatus.Text = "Connecting...";
                    break;
                case NVS4Viewer2Lib._WinsockStatus_Type.wDisconnected:
                    lblStatus.Visible = true;
                    lblStatus.Text = "Disconnection";
                    break;
            }

        }

    }

    public interface IFormVirtualCCTVOwner
    {
        void SetLocation(int x, int y);
    }
}
