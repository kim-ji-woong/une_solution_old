using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDMS
{
    public partial class BigCCTVCtrl : Form
    {
        private CCTV m_cctv = null;
        private bool m_isConnected = false;
        private int m_nBtnLeftPos = 0;
        private int m_nLeftSpace = 0;
        private bool m_isSelected = false;
        private Form4CCTV m_frmParent = null;
        private CCTV m_cctvQueue = null;

        private bool m_isValidCamera = false;
        private bool m_isClosing = false;
        //private System.IO.StreamWriter m_logger = null;

        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public SDMS.CCTV CCTV
        {
            get { return m_cctv; }
            set
            {
                if (m_cctv != value)
                {
                    m_isValidCamera = false;
                    
                    if (m_cctv != null)
                    {
                        m_cctvQueue = value;
                        CloseCamera(true);
                    }
                    else
                    {
                        m_cctv = value;
                        LoadCamera();
                    }

                    EnableControl(value != null);
                }
            }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;

                if (m_isSelected)
                    this.BackColor = Color.FromArgb(109, 155, 206);
                else
                    this.BackColor = Control.DefaultBackColor;
            }
        }

        private static int m_nCCTVCount = 0;
        protected int m_nID = -1;

        private static int m_isFakeMode = -1;
        protected static string m_strFakeCCTVFolderPath = "";

        private static int m_nInitWidth = 0, m_nInitHeight = 0;

        protected AxxpressStrmLib.AxxpressStrm CCTVCtrl
        {
            get { return axxpressStrm1; }
        }

        protected Button LeftButton
        {
            get { return btnLeft; }
        }

        protected Button RightButton
        {
            get { return btnRight; }
        }

        public static BigCCTVCtrl MakeInstance(CCTV cctv, Form4CCTV frmParent)
        {
            if (m_isFakeMode < 0)
            {
                m_strFakeCCTVFolderPath = FormMain.Instance.DBManager.LoadIni("시연용CCTV동영상", "SDMS");

                if (m_strFakeCCTVFolderPath.Length == 0)
                    m_isFakeMode = 0;
                else
                    m_isFakeMode = 1;
            }

            if (m_isFakeMode == 1)
                return new BigCCTVCtrl_Fake(cctv, frmParent);

            return new BigCCTVCtrl(cctv, frmParent);
        }

        /*private void WriteLog(string strLog)
        {
            DateTime dtNow = DateTime.Now;
            string strNow = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            m_logger.WriteLine(strNow + " : " + strLog);
            m_logger.Flush();
        }*/

        public BigCCTVCtrl(CCTV cctv, Form4CCTV frmParent)
        {
            m_nID = ++m_nCCTVCount;

            //string strLogFilePath = string.Format("BigCCTV_{0}.log", m_nID);
            //m_logger = new System.IO.StreamWriter(strLogFilePath, true, Encoding.UTF8);
            //WriteLog("LogFile Create");

            InitializeComponent();

            if (m_nInitWidth == 0)
            {
                m_nInitWidth = this.Size.Width;
                m_nInitHeight = this.Size.Height;
            }

            m_nBtnLeftPos = this.Size.Width - btnLeft.Location.X;
            m_nLeftSpace = btnLeft.Location.X - axxpressStrm1.Size.Width;

            CCTV = cctv;

            this.TopLevel = false;
            frmParent.Controls.Add(this);
            m_frmParent = frmParent;
        }

        protected virtual void BigCCTVCtrl_Load(object sender, EventArgs e)
        {
            if (m_cctv != null)
                LoadCamera();
        }

        protected virtual void LoadCamera()
        {
			lock (m_cctv)
			{
				if (m_cctv == null)
					return;

				axxpressStrm1.PlaybackMode = m_cctv.PlayBackMode;
				if (m_cctv == null)
					return;
				axxpressStrm1.UseRepository = m_cctv.UseRepository;
				if (m_cctv == null)
					return;
				axxpressStrm1.AccessKey = m_cctv.AccessKey;
				if (m_cctv == null)
					return;
				axxpressStrm1.IP = m_cctv.IPAddress;
				if (m_cctv == null)
					return;
				axxpressStrm1.Port = m_cctv.PortNo;

				axxpressStrm1.Connect();
			}
            
        }

        protected virtual void LoadCameraThread()
        {
            /*if (m_cctv == null)
                WriteLog("재접속 시도, m_cctv is null");
            else
                WriteLog("재접속 시도, Current Camera Name : " + m_cctv.AccessKey);*/

            LoadCamera();
        }

        protected virtual void CloseCamera(bool useThread = true)
        {
            if (useThread)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(CloseCameraThread));
                t.Start();
				t.Join();
            }
            else
            {
                axxpressStrm1.LiveAudioOutput(0);
                axxpressStrm1.LiveVideo(0);
                axxpressStrm1.LiveAudio(0);
                axxpressStrm1.Disconnect();
                axxpressStrm1.RepositoryDisconnect();
                m_isConnected = false;
            }
        }

        protected virtual void CloseCameraThread()
        {
            axxpressStrm1.LiveAudioOutput(0);
            axxpressStrm1.LiveVideo(0);
            axxpressStrm1.LiveAudio(0);
            axxpressStrm1.Disconnect();
            axxpressStrm1.RepositoryDisconnect();
            m_isConnected = false;

            if (m_cctvQueue != null)
            {
                m_cctv = m_cctvQueue;
                m_cctvQueue = null;

                LoadCamera();
            }
            else
                m_cctv = m_cctvQueue;
        }

        private void axxpressStrm1_Notify(object sender, AxxpressStrmLib._DxpressStrmEvents_NotifyEvent e)
        {
            if (e.code == 1)
            {
                m_isConnected = false;

                // 재접속 시도
                if (m_isValidCamera && !m_isClosing)
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(LoadCameraThread));
                    t.Start();
                }
            }
            else if (e.code == 2)
            {
                axxpressStrm1.LiveVideo(1);
                m_isConnected = true;
                m_isValidCamera = true;
            }
        }

        private void BigCCTVCtrl_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_isClosing = true;

            if (m_cctv != null)
                CloseCamera();
        }

        private void BigCCTVCtrl_Resize(object sender, EventArgs e)
        {
            if (m_nBtnLeftPos > 0)
            {
                int nLeftPos = this.Size.Width - m_nBtnLeftPos;

                MoveControl(btnUp, nLeftPos);
                MoveControl(btnRight, nLeftPos);
                MoveControl(btnDown, nLeftPos);
                MoveControl(pictureBoxCross, nLeftPos);
                MoveControl(btnZoomIn, nLeftPos);
                MoveControl(btnZoomOut, nLeftPos);

                MoveControl(btnLeft, nLeftPos);

                ChangeCCTVSize(nLeftPos - m_nLeftSpace, axxpressStrm1.Size.Height);
                //axxpressStrm1.Size = new Size(nLeftPos - m_nLeftSpace, axxpressStrm1.Size.Height);
            }
        }

        protected virtual void ChangeCCTVSize(int nWidth, int nHeight)
        {
            axxpressStrm1.Size = new Size(nWidth, nHeight);
        }

        private void MoveControl(Control ctrl, int nLeftPos)
        {
            ctrl.Location = new Point(nLeftPos + ctrl.Location.X - btnLeft.Location.X, ctrl.Location.Y);
        }

        protected virtual void OnCommandButtonDown(object sender, EventArgs e)
        {
            if (axxpressStrm1.PTZLockStatus == 0)
            {
                short nCommand = -1;

                if (sender == btnZoomIn)
                    nCommand = 4;
                else if (sender == btnZoomOut)
                    nCommand = 6;
                else if (sender == btnUp)
                    nCommand = 2;
                else if (sender == btnDown)
                    nCommand = 3;
                else if (sender == btnRight)
                    nCommand = 1;
                else if (sender == btnLeft)
                    nCommand = 0;
                else
                    return;

                axxpressStrm1.PresetNumber = 1;
                axxpressStrm1.PTZSpeed = 50;
                axxpressStrm1.CustomPTZControl(nCommand);
            }
        }

        protected virtual void OnCommandButtonUp(object sender, MouseEventArgs e)
        {
            if (axxpressStrm1.PTZLockStatus == 0)
            {
                axxpressStrm1.CustomPTZControl(9);
            }
        }

        private void BigCCTVCtrl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_frmParent.OnSelectCCTV(this);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                if (m_cctv == null)
                    MessageBox.Show("m_cctv is null");
                else if (m_isConnected)
                    MessageBox.Show("CCTV is connected");
                else
                    MessageBox.Show("CCTV is disconnected");
            }
        }

        private void EnableControl(bool enabled)
        {            
            btnLeft.Enabled = btnRight.Enabled = btnUp.Enabled = btnDown.Enabled = enabled;
            btnZoomIn.Enabled = btnZoomOut.Enabled = enabled;
        }

        private void BigCCTVCtrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                m_frmParent.RemoveCCTV();
            }
        }

        protected virtual void BigCCTVCtrl_Shown(object sender, EventArgs e)
        {

        }

        public void SetCCTVMode(CCTVMode mode)
        {
            if (mode == CCTVMode.CCTV_ONLY)
            {
                btnUp.Visible = false;
                btnDown.Visible = false;
                btnLeft.Visible = false;
                btnRight.Visible = false;
                pictureBoxCross.Visible = false;
                btnZoomIn.Visible = false;
                btnZoomOut.Visible = false;

                CCTVCtrl.Dock = DockStyle.Fill;
            }
            else
            {
                btnUp.Visible = true;
                btnDown.Visible = true;
                btnLeft.Visible = true;
                btnRight.Visible = true;
                pictureBoxCross.Visible = true;
                btnZoomIn.Visible = true;
                btnZoomOut.Visible = true;

                CCTVCtrl.Dock = DockStyle.None;
            }
        }
    }

    public class BigCCTVCtrl_Fake : BigCCTVCtrl
    {
        private DirectShowLib.IGraphBuilder m_graphBuilder = null;
        private DirectShowLib.IMediaControl m_mediaControl = null;
        private DirectShowLib.IVideoWindow m_videoWindow = null;

        private Panel m_cctvPanel = new Panel();

        public BigCCTVCtrl_Fake(CCTV cctv, Form4CCTV frmParent)
            : base(cctv, frmParent)
        {
            Point pt = CCTVCtrl.Location;
            Size sz = CCTVCtrl.Size;

            this.Controls.Add(m_cctvPanel);

            m_cctvPanel.Location = pt;
            m_cctvPanel.Size = sz;
        }

        protected override void BigCCTVCtrl_Shown(object sender, EventArgs e)
        {
            string strURL = "";

            switch (m_nID % 4)
            {
                case 0:
                    strURL = m_strFakeCCTVFolderPath + "\\cctv_video01.wmv";
                    break;

                case 1:
                    strURL = m_strFakeCCTVFolderPath + "\\cctv_video02.wmv";
                    break;

                case 2:
                    strURL = m_strFakeCCTVFolderPath + "\\cctv_video03.wmv";
                    break;

                case 3:
                    strURL = m_strFakeCCTVFolderPath + "\\cctv_video04.wmv";
                    break;

                default:
                    return;
            }

            if (System.IO.File.Exists(strURL))
            {
                int nRightSpace = this.Size.Width - (RightButton.Location.X + RightButton.Size.Width);
                int nPanelRightPos = LeftButton.Location.X - nRightSpace;
                m_cctvPanel.Size = new System.Drawing.Size(nPanelRightPos - m_cctvPanel.Location.X, m_cctvPanel.Size.Height);

                m_graphBuilder = new DirectShowLib.FilterGraph() as DirectShowLib.IGraphBuilder;
                m_mediaControl = m_graphBuilder as DirectShowLib.IMediaControl;

                m_videoWindow = m_graphBuilder as DirectShowLib.IVideoWindow;

                m_graphBuilder.RenderFile(strURL, null);

                m_videoWindow.put_Owner(m_cctvPanel.Handle);
                m_videoWindow.put_WindowStyle(DirectShowLib.WindowStyle.Child | DirectShowLib.WindowStyle.ClipSiblings);
                m_videoWindow.SetWindowPosition(0, 0, m_cctvPanel.Width, m_cctvPanel.Height);
                m_videoWindow.put_MessageDrain(m_cctvPanel.Handle);
                m_videoWindow.put_Visible(DirectShowLib.OABool.True);

                if (m_mediaControl == null)
                {
                    return;
                }

                m_mediaControl.Run();
            }

            CCTVCtrl.Visible = false;
        }

        protected override void LoadCamera()
        {
            if (m_mediaControl != null)
            {
                m_mediaControl.Run();
            }
        }

        protected override void CloseCamera(bool useThread = true)
        {
            if (m_mediaControl != null)
            {
                m_mediaControl.Stop();
            }
        }

        protected override void CloseCameraThread()
        {
            if (m_mediaControl != null)
            {
                m_mediaControl.Stop();
            }
        }

        protected override void ChangeCCTVSize(int nWidth, int nHeight)
        {
            if (m_videoWindow != null)
            {
                m_videoWindow.put_Width(this.Width);
                m_videoWindow.put_Height(this.Height);
            }
        }

        protected override void OnCommandButtonDown(object sender, EventArgs e)
        {
        }

        protected override void OnCommandButtonUp(object sender, MouseEventArgs e)
        {
        }
    }

    //public class BigCCTVCtrl_Fake : BigCCTVCtrl
    //{
    //    //private AxWMPLib.AxWindowsMediaPlayer m_mediaPlayer = null;
    //    private PanelMediaPlayer m_mediaPlayer = null;

    //    public BigCCTVCtrl_Fake(CCTV cctv, Form4CCTV frmParent)
    //        : base(cctv, frmParent)
    //    {
    //    }

    //    protected override void BigCCTVCtrl_Shown(object sender, EventArgs e)
    //    {
    //        string strURL = "";

    //        switch (m_nID % 4)
    //        {
    //            case 0:
    //                strURL = m_strFakeCCTVFolderPath + "\\cctv_video01.wmv";
    //                break;

    //            case 1:
    //                strURL = m_strFakeCCTVFolderPath + "\\cctv_video02.wmv";
    //                break;

    //            case 2:
    //                strURL = m_strFakeCCTVFolderPath + "\\cctv_video03.wmv";
    //                break;

    //            case 3:
    //                strURL = m_strFakeCCTVFolderPath + "\\cctv_video04.wmv";
    //                break;

    //            default:
    //                return;
    //        }

    //        //m_mediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
    //        m_mediaPlayer = new PanelMediaPlayer(strURL);
    //        m_mediaPlayer.Size = CCTVCtrl.Size;
    //        m_mediaPlayer.Location = CCTVCtrl.Location;
            
    //        try
    //        {
    //            this.Controls.Add(m_mediaPlayer);
    //        }
    //        catch (System.ArgumentException)
    //        {}

    //        m_mediaPlayer.Play();
    //        CCTVCtrl.Visible = false;

    //        //base.BigCCTVCtrl_Load(sender, e);
    //    }

    //    protected override void LoadCamera()
    //    {
    //    }

    //    protected override void CloseCamera(bool useThread = true)
    //    {
    //        m_mediaPlayer.Stop();
    //    }

    //    protected override void CloseCameraThread()
    //    {
    //        m_mediaPlayer.Stop();
    //    }

    //    protected override void ChangeCCTVSize(int nWidth, int nHeight)
    //    {
    //        if (m_mediaPlayer!= null)
    //            m_mediaPlayer.Size = new Size(nWidth, nHeight);
    //    }

    //    protected override void OnCommandButtonDown(object sender, EventArgs e)
    //    {
    //    }

    //    protected override void OnCommandButtonUp(object sender, MouseEventArgs e)
    //    {
    //    }
    //}
}
