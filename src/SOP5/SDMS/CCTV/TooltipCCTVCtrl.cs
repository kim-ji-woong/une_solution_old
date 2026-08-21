using System;
using System.Drawing;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;

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

        private ISensorTooltipOwner m_viewOwner = null;

		private bool m_bVisible = false;
		private bool m_isConnected = false;

		public bool Connected
		{
			get { return m_isConnected; }
		}

		private CCTV m_cctv = null;

		public CCTV CCTV
		{
			get { return m_cctv; }
			set { m_cctv = value; }
		}

        public ISensor Sensor
        {
            get;
            set;
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

		protected UnE.Control.CCTVCtrl CCTVCtrl
		{
			get { return cctvCtrl1; }
		}

        public static TooltipCCTVCtrl MakeInstance(ISensorTooltipOwner view, CCTV cctv)
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

        public TooltipCCTVCtrl(ISensorTooltipOwner view, CCTV cctv)
		{
			m_nID = ++m_nCCTVCount;

            this.cctvCtrl1 = new UnE.Control.CCTVCtrl((UnE.Control.CCTVTypes)cctv.CCTVType);

           
			InitializeComponent();

            this.checkBoxLOD.Checked = (cctv.LODType != CCTV.LOD.DISCONNECTED);

			m_nOwnTargetSpaceX = m_nTargetSpaceX;
			m_nOwnTargetSpaceY = m_nTargetSpaceY;

			this.TopLevel = false;
			view.AddToolTipControl(this);
			this.BringToFront();
			//this.TransparencyKey = this.BackColor;
			m_viewOwner = view;
			m_bVisible = false;
			m_cctv = cctv;

            if (m_cctv != null && m_cctv.ReversePTZ < 0)
                btnControl.Visible = false;

			base.Hide();

            this.checkBoxLOD.CheckedChanged += checkBoxLOD_CheckedChanged;
		}

        private void checkBoxLOD_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLOD.Checked)
                m_cctv.LODType = CCTV.LOD.DEFAULT;
            else
                m_cctv.LODType = CCTV.LOD.DISCONNECTED;
            
            EditCCTV editCCTV = new EditCCTV(m_cctv);
            editCCTV.LOD = (checkBoxLOD.Checked ? 1 : -1);

            if (editCCTV.Update(FormMain.Instance.DBManager))
            {
                m_cctv.UpdateDBData();

                //PageBackstageHome.Instance.ContentForm.OutdoorView.UpdateIcon(m_cctv.POI.ID, m_cctv.IconPath);
                //PageBackstageHome.Instance.ContentForm.IndoorView.UpdateIcon(m_cctv.POI.ID, m_cctv.IconPath);
            }
        }

		/*private string GetManager()
		{
			if (m_cctv == null || m_cctv.POI == null)
				return "";

			POI poi = m_cctv.POI;
			FacilityManagerGroup group = null;

			if (poi.Facility != null)
			{
				if (poi.Facility.GetType() == typeof(ISensor))
				{
					EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(poi.Zone, poi.X, poi.Y);
					if (equipZone != null)
                        group = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(IFacility.FacilityType.CCTV, equipZone);
				}
			}

			if (group == null)
			{
				if (poi.Zone == null)
				{
                    group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.CCTV);
				}
				else if (poi.Zone.Building == null)
				{
                    group = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(IFacility.FacilityType.CCTV, poi.Zone);
					if (group == null || group.IsEmpty())
                        group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.CCTV);
				}
				else
				{
                    group = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(IFacility.FacilityType.CCTV, poi.Zone.Building);
					if (group == null || group.IsEmpty())
						group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.CCTV);
				}
			}

			string strPhoneNumber = "";
			return FormMain.Instance.DataManager.GetFacilityManagerName(group, ref strPhoneNumber);
		}*/


        protected virtual void OnCommandButtonDown(object sender, EventArgs e)
        {
            if (sender == btnZoomIn)
                cctvCtrl1.ZoomIn();
            else if (sender == btnZoomOut)
                cctvCtrl1.ZoomOut();
            else if (sender == btnUp)
                cctvCtrl1.MoveUp();
            else if (sender == btnDown)
                cctvCtrl1.MoveDown();
            else if (sender == btnRight)
                cctvCtrl1.MoveRight();
            else if (sender == btnLeft)
                cctvCtrl1.MoveLeft();
            else if (sender == btnStop)
                cctvCtrl1.TestStop(9);
        }

        protected virtual void OnCommandButtonUp(object sender, MouseEventArgs e)
        {
            if (cctvCtrl1 != null && cctvCtrl1.CCTVType == UnE.Control.CCTVTypes.XpressStrm)
            {
                cctvCtrl1.TestStop(9);
            }
        }

		// xTarget, yTarget : Target POI의 좌표
		public void Show(int xTarget, int yTarget)
		{
			try
			{
				if (FormMain.Instance.ThumbnailMode)
					return;

                if (m_cctv != null)
                    this.Text = String.Format("{0} - {1}", m_cctv.ID, m_cctv.AccessKey);


                panelPTZ.Visible = false;
                //labelManager.Text = "담당자 : " + GetManager();

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

                this.Location = new Point(xTarget, yTarget);
				m_bVisible = true;
				this.Show();

                
                this.ShowInTaskbar = true;
                this.BringToFront();
                //this.UpdateZOrder();
			}
			catch (Exception e)
			{
				ConnectionLogEx.Instance.WriteLine("TooltipCCTVCtrl.Show() Error", e);
			}
		}
        
        private UnE.Control.CCTVTypes GetCCTVType(int nType)
        {
            switch(nType)
            {
                case 1:
                    return UnE.Control.CCTVTypes.Axis;
                case 2:
                    return UnE.Control.CCTVTypes.NVS;
                case 3:
                    return UnE.Control.CCTVTypes.XpressStrm;
                case 4:
                    return UnE.Control.CCTVTypes.UDP;
                case 5:
                    return UnE.Control.CCTVTypes.Panasonic;
                case 6:
                    return UnE.Control.CCTVTypes.TechWin;
                case 7:
                    return UnE.Control.CCTVTypes.IPVideo;
                case 8:
                    return UnE.Control.CCTVTypes.HIK;
                case 9:
                    return UnE.Control.CCTVTypes.NVT;
                case 10:
                    return UnE.Control.CCTVTypes.MediaPlayer;
                case 11:
                    return UnE.Control.CCTVTypes.IDIS;
                case 12:
                    return UnE.Control.CCTVTypes.RTSP;
                case 13:
                    return UnE.Control.CCTVTypes.IDIS_NVR;
                case 14:
                    return UnE.Control.CCTVTypes.ITX_NVR;
                case 15:
                    return UnE.Control.CCTVTypes.RTSPONVIF;

            }
            return UnE.Control.CCTVTypes.NotSet;
        }

		protected virtual void LoadCamera()
		{
            //cctvCtrl1.ChangeType(GetCCTVType(CCTV.CCTVType));
            cctvCtrl1.AddProperty("MediaType", "rtp-tcp");
            cctvCtrl1.AddProperty("Channel", CCTV.Channel.ToString());
            cctvCtrl1.AddProperty("Stream", CCTV.Stream.ToString());
            cctvCtrl1.AddProperty("HttpPort", CCTV.HttpPort.ToString());
            cctvCtrl1.AddProperty("IPAddress", CCTV.IPAddress);
            cctvCtrl1.AddProperty("Port", CCTV.PortNo.ToString());
            cctvCtrl1.AddProperty("UserName", CCTV.UserName);
            cctvCtrl1.AddProperty("Password", CCTV.Password);
            //cctvCtrl1.AddProperty("ReversePTZ", CCTV.ReversePTZ.ToString());
            cctvCtrl1.AddProperty("AccessKey", CCTV.AccessKey.ToString());
            cctvCtrl1.AddProperty("URL", CCTV.URL.ToString());

            cctvCtrl1.Connect();
		}

		protected virtual void CloseCamera(bool useThread = true)
		{
			if (useThread)
			{
				System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(CloseCameraThread));
                t.Name = "CCTV Close Thread";
                t.Start();
			}
			else
			{
                cctvCtrl1.Disconnect();

				m_isConnected = false;
				if (m_cctv != null)
					m_cctv.Connected = false;
			}
		}

		protected virtual void CloseCameraThread()
		{
            cctvCtrl1.Disconnect();

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
        private bool bCloseOnHide = false;
		public void Hide(bool absolutely)
		{
            if (IsDisposed == true)
                return;

			if (!checkBoxFix.Checked || absolutely)
			{
                bCloseOnHide = true;
                //this.Close();
                if (!IsTemporaryHidden() || m_cctv.CCTVType == (int)UnE.Control.CCTVTypes.IDIS)
					CloseCamera(false);

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
            if (m_cctv != null && m_cctv.Connected && cctvCtrl1.IsDisposed == false)
            {
                CloseCamera();
            }

			m_bLayerVisible = false;
			m_bVisible = false;
			Visible = false;
			base.Close();
		}

        //private void axxpressStrm1_Notify(object sender, AxxpressStrmLib._DxpressStrmEvents_NotifyEvent e)
        //{
        //    if (e.code == 1)
        //    {
        //        m_isConnected = false;
        //        if (m_cctv != null)
        //            m_cctv.Connected = false;
        //    }
        //    else if (e.code == 2)
        //    {
        //        axxpressStrm1.LiveVideo(1);
        //        m_isConnected = true;
        //        if (m_cctv != null)
        //            m_cctv.Connected = false;
        //    }
        //}

		protected virtual void TooltipCCTVCtrl_Resize(object sender, EventArgs e)
		{
            try
            {
			    Point pt = this.Location;

			    m_nOwnTargetSpaceX = pt.X - m_nTargetPOIX;
			    m_nOwnTargetSpaceY = m_nTargetPOIY - pt.Y;

			    checkBoxFix.Refresh();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }        
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
            //if (bCloseOnHide == true)
            //{
            //
            //}
            //else
            {
                e.Cancel = true;
                Hide(true);
                checkBoxFix.Checked = false;
            }			
		}

        private bool bToggle = false;
		private void TooltipCCTVCtrl_Move(object sender, EventArgs e)
		{
            try
            {
                Point pt = this.Location;

                m_nOwnTargetSpaceX = pt.X - m_nTargetPOIX;
                m_nOwnTargetSpaceY = m_nTargetPOIY - pt.Y;

                if (bToggle == false)
                {
                    this.Size = new Size(this.Size.Width - 1, this.Size.Height);
                }
                else
                {
                    this.Size = new Size(this.Size.Width + 1, this.Size.Height);
                }

                bToggle = !bToggle;

                checkBoxFix.Refresh();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
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


        private System.Windows.Forms.Timer mRefreshTimer = new Timer();
        private void TooltipCCTVCtrl_LocationChanged(object sender, EventArgs e)
        {
            if(mRefreshTimer.Enabled == true)
                mRefreshTimer.Enabled = false;
            
            mRefreshTimer = new Timer();
            mRefreshTimer.Interval = 1000;
            mRefreshTimer.Tick += mRefreshTimer_Tick;
            mRefreshTimer.Enabled = true;
        }

        void mRefreshTimer_Tick(object sender, EventArgs e)
        {
            mRefreshTimer.Enabled = false;

            this.Size = new Size(this.Width + 1, this.Height);
            this.Refresh();

            this.Size = new Size(this.Width - 1, this.Height);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panelPTZ.BringToFront();
            panelPTZ.Visible = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            btnControl.BringToFront();
            panelPTZ.Visible = false;            
        }

       
	}

	public class TooltipCCTVCtrl_Fake : TooltipCCTVCtrl
	{
		private DirectShowLib.IGraphBuilder m_graphBuilder = null;
		private DirectShowLib.IMediaControl m_mediaControl = null;
		private DirectShowLib.IVideoWindow m_videoWindow = null;

		private bool m_isInit = false;
		private string m_strURL = "";

        public TooltipCCTVCtrl_Fake(ISensorTooltipOwner view, CCTV cctv)
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

	
}