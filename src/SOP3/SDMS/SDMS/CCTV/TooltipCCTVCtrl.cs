using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core;

namespace SDMS
{
    public partial class TooltipCCTVCtrl : Form, IPOIPopup
    {
        // Target과의 거리
        static private int m_nTargetSpaceX = 30;
        static private int m_nTargetSpaceY = 50;

        private int m_nOwnTargetSpaceX = -1;
        private int m_nOwnTargetSpaceY = -1;
        private int m_nTargetPOIX = 0;
        private int m_nTargetPOIY = 0;
        private Point m_ptOrigin = new Point();

        private BaseViewEx m_viewOwner = null;

        private bool m_bVisible = false;
        private bool m_isConnected = false;

        public bool Connected
        {
            get { return m_isConnected; }
        }

        private CCTV m_cctv = null;

        public SDMS.CCTV CCTV
        {
            get { return m_cctv; }
            set { m_cctv = value; }
        }

        private bool m_bLayerVisible = true;
        public bool LayerVisible
        {
            get { return m_bLayerVisible; }
            set 
            { 
                m_bLayerVisible = value;
                if (m_bLayerVisible == false)
                {
                    Visible = false;
                }
                else
                {
                    if (m_bVisible == true)
                    {
                        //base.Show();
                    }
                }
            }
        }

        private static int m_nCCTVCount = 0;
        protected int m_nID = -1;

        private static int m_isFakeMode = -1;
        protected static string m_strFakeCCTVFolderPath = "";

        protected AxxpressStrmLib.AxxpressStrm CCTVCtrl
        {
            get { return axxpressStrm1; }
        }

        public static TooltipCCTVCtrl MakeInstance(BaseViewEx view, CCTV cctv)
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
                return new TooltipCCTVCtrl_Fake(view, cctv);

            return new TooltipCCTVCtrl(view, cctv);
        }

        public TooltipCCTVCtrl(BaseViewEx view, CCTV cctv)
        {
            m_nID = ++m_nCCTVCount;

            InitializeComponent();

            m_nOwnTargetSpaceX = m_nTargetSpaceX;
            m_nOwnTargetSpaceY = m_nTargetSpaceY;

            this.TopLevel = false;
            view.Controls.Add(this);
            this.BringToFront();
            //this.TransparencyKey = this.BackColor;
            m_viewOwner = view;
            m_bVisible = false;
            m_cctv = cctv;
            base.Hide();
        }

        private string GetManager()
        {
            if (m_cctv == null || m_cctv.POI == null)
                return "";

            POI poi = m_cctv.POI;
            FacilityManagerGroup group = null;

			if (poi.Facility != null)
			{
				if (poi.Facility.GetType() == typeof(SensorZone))
				{
					EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(poi.Zone, poi.X, poi.Y);
					if (equipZone != null)
						group = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(Facility.FacilityType.CCTV, equipZone);
				}
			}

			if (group == null)
			{
				if (poi.Zone == null)
				{
					group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(Facility.FacilityType.CCTV);
				}
				else if (poi.Zone.Building == null)
				{
					group = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(Facility.FacilityType.CCTV, poi.Zone);
					if (group == null || group.IsEmpty())
						group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(Facility.FacilityType.CCTV);
				}
				else
				{
					group = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(Facility.FacilityType.CCTV, poi.Zone.Building);
					if (group == null || group.IsEmpty())
						group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(Facility.FacilityType.CCTV);
				}
			}

			string strPhoneNumber = "";
            return FormMain.Instance.DataManager.GetFacilityManagerName(group, ref strPhoneNumber);
        }

        // xTarget, yTarget : Target POI의 좌표
        public void Show(int xTarget, int yTarget)
        {
            try
            {
                if (FormMain.Instance.ThumbnailMode)
                    return;

                labelManager.Text = "담당자 : " + GetManager();

                /*Point ptTriangle = pictureBox1.Location;
                Rectangle rect =  m_viewOwner.RectangleToScreen(m_viewOwner.ClientRectangle);

                int x = xTarget + rect.Left - m_viewOwner.ClientRectangle.Left + m_nTargetSpace - ptTriangle.X;
                int y = yTarget + rect.Top - m_viewOwner.ClientRectangle.Top - ptTriangle.Y;*/

                m_nTargetPOIX = xTarget;
                m_nTargetPOIY = yTarget;
                m_ptOrigin = this.Location;

                int x = xTarget + m_nOwnTargetSpaceX;
                int y = yTarget - m_nOwnTargetSpaceY;

                if (!m_isConnected)
                    LoadCamera();

                this.Location = new Point(x, y);
                m_bVisible = true;
                this.Show();
            }
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine("TooltipCCTVCtrl.Show() Error", e);
            }
        }

        protected virtual void LoadCamera()
        {
            axxpressStrm1.PlaybackMode = CCTV.PlayBackMode;
            axxpressStrm1.UseRepository = CCTV.UseRepository;
            axxpressStrm1.AccessKey = CCTV.AccessKey;
            axxpressStrm1.IP = CCTV.IPAddress;
            axxpressStrm1.Port = CCTV.PortNo;
            axxpressStrm1.Connect();
        }

        protected virtual void CloseCamera(bool useThread = true)
        {
            if (useThread)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(CloseCameraThread));
                t.Start();
            }
            else
            {
                axxpressStrm1.LiveAudioOutput(0);
                axxpressStrm1.LiveVideo(0);
                axxpressStrm1.LiveAudio(0);
                axxpressStrm1.Disconnect();
                axxpressStrm1.RepositoryDisconnect();
                m_isConnected = false;
				if (m_cctv != null)
					m_cctv.Connected = false;
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
			if (m_cctv != null)
				m_cctv.Connected = false;
        }

        // Panning이나 Orbit같은 동작을 위하여 잠시동안 임시로 꺼두는 것인가?
        private bool IsTemporaryHidden()
        {
            if (m_viewOwner == null)
                return false;

            if (m_cctv == null)
                return false;

            if (m_cctv.POI == null)
                return false;

            return m_viewOwner.IsTemporaryHiddenPOI(m_cctv.POI);
        }

        public void Hide(bool absolutely)
        {
            if (!checkBoxFix.Checked || absolutely)
            {
                if (!IsTemporaryHidden())
                    CloseCamera();

                base.Hide();
                m_bVisible = false;
            }
        }

        public void MoveTarget(int xTarget, int yTarget)
        {
            m_nTargetPOIX = xTarget;
            m_nTargetPOIY = yTarget;
            m_ptOrigin = this.Location;

            int x = xTarget + m_nOwnTargetSpaceX;
            int y = yTarget - m_nOwnTargetSpaceY;

            this.Location = new Point(x, y);
        }

        public bool IsVisible()
        {
            if (m_bLayerVisible == true && m_bVisible == true)
                return true;
            return Visible;
        }

        public new void Close()
        {
            m_bLayerVisible = false;
            m_bVisible = false;
            Visible = false;
            base.Close();
        }

        private void axxpressStrm1_Notify(object sender, AxxpressStrmLib._DxpressStrmEvents_NotifyEvent e)
        {
            if (e.code == 1)
            {
                m_isConnected = false;
				if (m_cctv != null)
					m_cctv.Connected = false;
            }
            else if (e.code == 2)
            {
                axxpressStrm1.LiveVideo(1);
                m_isConnected = true;
				if (m_cctv != null)
					m_cctv.Connected = false;
            }
        }

        protected virtual void TooltipCCTVCtrl_Resize(object sender, EventArgs e)
        {
            Point pt = this.Location;

            m_nOwnTargetSpaceX = pt.X - m_nTargetPOIX;
            m_nOwnTargetSpaceY = m_nTargetPOIY - pt.Y;

            checkBoxFix.Refresh();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112)    // WM_SYSCOMMAND
            {
                if (m.WParam == (IntPtr)0xF030) // SC_MAXIMIZE
                {
                    button1_Click(null, null);
                    return;
                }
            }

            base.WndProc(ref m);
        }

        private void TooltipCCTVCtrl_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide(true);
        }

        private void TooltipCCTVCtrl_Move(object sender, EventArgs e)
        {
            Point pt = this.Location;

            m_nOwnTargetSpaceX = pt.X - m_nTargetPOIX;
            m_nOwnTargetSpaceY = m_nTargetPOIY - pt.Y;

            checkBoxFix.Refresh();
        }

		private void TooltipCCTVCtrl_Load(object sender, EventArgs e)
		{

		}

		private void TooltipCCTVCtrl_SizeChanged(object sender, EventArgs e)
		{

			
		}

		private bool m_bMaxSize = false;
		private void button1_Click(object sender, EventArgs e)
		{
			if (m_bMaxSize == false)
			{
				Point pt = this.Location;

				m_nOwnTargetSpaceX = pt.X - m_nTargetPOIX;
				m_nOwnTargetSpaceY = m_nTargetPOIY - pt.Y;
				this.Size = this.MaximumSize;
				//button1.Text = "작은화면";
			}
			else
			{
				Point pt = this.Location;

				m_nOwnTargetSpaceX = pt.X - m_nTargetPOIX;
				m_nOwnTargetSpaceY = m_nTargetPOIY - pt.Y;
				this.Size = this.MinimumSize;
				//button1.Text = "큰화면";
			}
			m_bMaxSize = !m_bMaxSize;
		}

        protected virtual void TooltipCCTVCtrl_Shown(object sender, EventArgs e)
        {
        }
    }

    public class TooltipCCTVCtrl_Fake : TooltipCCTVCtrl
    {
        private DirectShowLib.IGraphBuilder m_graphBuilder = null;
        private DirectShowLib.IMediaControl m_mediaControl = null;
        private DirectShowLib.IVideoWindow m_videoWindow = null;

        private bool m_isInit = false;
        private string m_strURL = "";

        public TooltipCCTVCtrl_Fake(BaseViewEx view, CCTV cctv)
            : base(view, cctv)
        {
        }

        protected override void LoadCamera()
        {
            if (m_isInit && m_mediaControl != null)
            {
                m_mediaControl.Run();
            }
        }

        protected override void CloseCamera(bool useThread = true)
        {
            if (m_isInit && m_mediaControl != null)
            {
                m_mediaControl.Stop();
            }
        }

        protected override void CloseCameraThread()
        {
            if (m_isInit && m_mediaControl != null)
            {
                m_mediaControl.Stop();
            }
        }

        protected override void TooltipCCTVCtrl_Resize(object sender, EventArgs e)
        {
            if (m_videoWindow != null)
            {
                m_videoWindow.put_Width(this.Width);
                m_videoWindow.put_Height(this.Height);
            }
        }

        protected override void TooltipCCTVCtrl_Shown(object sender, EventArgs e)
        {
            switch (m_nID % 4)
            {
                case 0:
                    m_strURL = m_strFakeCCTVFolderPath + "\\cctv_video01.wmv";
                    break;

                case 1:
                    m_strURL = m_strFakeCCTVFolderPath + "\\cctv_video02.wmv";
                    break;

                case 2:
                    m_strURL = m_strFakeCCTVFolderPath + "\\cctv_video03.wmv";
                    break;

                case 3:
                    m_strURL = m_strFakeCCTVFolderPath + "\\cctv_video04.wmv";
                    break;

                default:
                    return;
            }

            if (System.IO.File.Exists(m_strURL))
            {
                m_graphBuilder = new DirectShowLib.FilterGraph() as DirectShowLib.IGraphBuilder;
                m_mediaControl = m_graphBuilder as DirectShowLib.IMediaControl;

                m_videoWindow = m_graphBuilder as DirectShowLib.IVideoWindow;

                m_graphBuilder.RenderFile(m_strURL, null);

                m_videoWindow.put_Owner(this.Handle);
                m_videoWindow.put_WindowStyle(DirectShowLib.WindowStyle.Child | DirectShowLib.WindowStyle.ClipSiblings);
                m_videoWindow.SetWindowPosition(0, 0, this.Width, this.Height);
                m_videoWindow.put_MessageDrain(this.Handle);
                m_videoWindow.put_Visible(DirectShowLib.OABool.True);

                if (m_mediaControl == null)
                {
                    return;
                }

                m_mediaControl.Run();
            }

            CCTVCtrl.Visible = false;
        }
    }

    //public class TooltipCCTVCtrl_Fake : TooltipCCTVCtrl
    //{
    //    //private AxWMPLib.AxWindowsMediaPlayer m_mediaPlayer = null;
    //    private PanelMediaPlayer m_mediaPlayer = null;
    //    private bool m_isInit = false;
    //    private string m_strURL = "";

    //    public TooltipCCTVCtrl_Fake(BaseViewEx view, CCTV cctv)
    //        : base(view, cctv)
    //    {
    //    }

    //    protected override void LoadCamera()
    //    {
    //        if (m_isInit)
    //        {
    //            m_mediaPlayer.URL = m_strURL;
    //        }
    //    }

    //    protected override void CloseCamera(bool useThread = true)
    //    {
    //        if (m_mediaPlayer != null)
    //            m_mediaPlayer.Stop();
    //    }

    //    protected override void CloseCameraThread()
    //    {
    //        if (m_mediaPlayer != null)
    //            m_mediaPlayer.Stop();
    //    }

    //    protected override void TooltipCCTVCtrl_Resize(object sender, EventArgs e)
    //    {
    //        /*Point pt = this.Location;

    //        m_nOwnTargetSpaceX = pt.X - m_nTargetPOIX;
    //        m_nOwnTargetSpaceY = m_nTargetPOIY - pt.Y;

    //        checkBoxFix.Refresh();*/
    //    }

    //    protected override void TooltipCCTVCtrl_Shown(object sender, EventArgs e)
    //    {
    //        switch (m_nID % 4)
    //        {
    //            case 0:
    //                m_strURL = m_strFakeCCTVFolderPath + "\\cctv_video01.wmv";
    //                break;

    //            case 1:
    //                m_strURL = m_strFakeCCTVFolderPath + "\\cctv_video02.wmv";
    //                break;

    //            case 2:
    //                m_strURL = m_strFakeCCTVFolderPath + "\\cctv_video03.wmv";
    //                break;

    //            case 3:
    //                m_strURL = m_strFakeCCTVFolderPath + "\\cctv_video04.wmv";
    //                break;

    //            default:
    //                return;
    //        }

    //        //m_mediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
    //        m_mediaPlayer = new PanelMediaPlayer(m_strURL);
    //        m_mediaPlayer.Location = CCTVCtrl.Location;
    //        m_mediaPlayer.Size = CCTVCtrl.Size;

    //        m_isInit = true;

    //        try
    //        {
    //            this.Controls.Add(m_mediaPlayer);
    //        }
    //        catch (System.ArgumentException)
    //        {
    //        }

    //        /*m_mediaPlayer.enableContextMenu = false;
    //        m_mediaPlayer.uiMode = "none";
    //        m_mediaPlayer.Location = CCTVCtrl.Location;
    //        m_mediaPlayer.Size = CCTVCtrl.Size;
    //        m_mediaPlayer.settings.autoStart = true;
    //        m_mediaPlayer.settings.setMode("loop", true);

    //        m_mediaPlayer.URL = m_strURL;*/
    //        m_mediaPlayer.Play();
    //        CCTVCtrl.Visible = false;
    //    }
    //}

    public partial class CCTV : Facility
    {
        public override IPOIPopup CreatePopup(BaseViewEx view)
        {
            //return new TooltipCCTVCtrl(view, this);
            return TooltipCCTVCtrl.MakeInstance(view, this);
        }
    }

    /*public class TooltipCCTV
    {
        private TooltipCCTVCtrl m_ctrl = new TooltipCCTVCtrl();
        private int m_nBeginPos = 30;
        private Position3D m_ptTarget = new Position3D(0.0f, 0.0f, 0.0f);

        public TooltipCCTV()
        {
        }
    }*/
}
