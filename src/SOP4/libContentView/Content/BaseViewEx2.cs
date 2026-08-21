using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.AccessControl;
using System.Windows.Forms;
using Core;
using Microsoft.Win32;
using UnE.Sensor;
using UnE.Spatial;
using UnE.View.Content;
using System.Reflection;
using System.Diagnostics;
using SDMS;

namespace UnE.View.Content
{

    public class BaseViewEx2 : BaseView, ISensorTooltipOwner, IBaseView
	{

        // 외부에서 Tooltip용 Control을 Add할때 사용 (ISensorTooltipOwner)
        public void AddToolTipControl(System.Windows.Forms.Control c)
        {
            this.Controls.Add(c);
        }


        public ILayerManager LayerManager { get; set; }

        public System.Drawing.Point GetPosition2D(int nPOIID, float x, float y, float z)
        {
            Position3D pos = new Position3D(x, y, z);
            return (System.Drawing.Point)Get2DPoint(pos);
        }

        public new bool Visible
        {
            get { return mMainContainer.Visible; }
            set { mMainContainer.Visible = value; }
        }

        private ToolStripContainer mMainContainer = new ToolStripContainer();

        public ToolStripContainer ToolStripContainer
        {
            get { return mMainContainer; }
            set { mMainContainer = value; }
        }

        private string m_szToolKey = @"SDMS\Unity\Toolstrip";
        private string m_szPosSubKeyName = "MainToolStripPos";
        private string m_szToolStripName = "ToolboxStrip";
        public void AddMainToolStrip(ToolStrip strip)
        {
            // read toolstrip position
            int nPos = ReadToolStripConfig();

            // Set StripName for using Key
            strip.Name = m_szToolStripName;

            // Add StripMenu
            SetToolStripMenu(strip, nPos);
        }

        public void RemoveMainToolStrip(ToolStrip strip)
        {
            mMainContainer.RightToolStripPanel.Controls.Remove(strip);
            mMainContainer.LeftToolStripPanel.Controls.Remove(strip);
            mMainContainer.BottomToolStripPanel.Controls.Remove(strip);
            mMainContainer.TopToolStripPanel.Controls.Remove(strip);
        }
        
        private void SetToolStripMenu(ToolStrip strip, int nPos)
        {
            if (nPos == 1)
                mMainContainer.RightToolStripPanel.Controls.Add(strip);
            else if (nPos == 2)
                mMainContainer.LeftToolStripPanel.Controls.Add(strip);
            else if (nPos == 3)
                mMainContainer.BottomToolStripPanel.Controls.Add(strip);
            else
                mMainContainer.TopToolStripPanel.Controls.Add(strip);
        }

        private int ReadToolStripConfig()
        {
            int nResult = 0;
            try
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(m_szToolKey);
                if (rkey == null)
                {
                    return 0;
                }
                else
                {
                    nResult = (int)rkey.GetValue(m_szPosSubKeyName, 0);
                }
                if (rkey != null)
                    rkey.Close();
            }
            catch (System.Exception)
            {
            }
            return nResult;
        }

        private int GetToolStringPos()
        {
            if (mMainContainer.TopToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 0;
            }
            else if (mMainContainer.RightToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 1;
            }
            else if (mMainContainer.LeftToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 2;
            }
            else if (mMainContainer.BottomToolStripPanel.Controls.ContainsKey(m_szToolStripName))
            {
                return 3;
            }
            return 0;
        }

        private void WriteToolStripConfig(int nPos)
        {
            try
            {
                string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

                RegistrySecurity rs = new RegistrySecurity();

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ChangePermissions,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Deny));

                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(m_szToolKey, true);
                if (rkey == null)
                {
                    try
                    {
                        rkey = Registry.CurrentUser.CreateSubKey(m_szToolKey, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (rkey != null)
                {
                    rkey.SetValue(m_szPosSubKeyName, nPos);
                    rkey.Close();
                }
            }
            catch (System.Exception)
            {
            }
        }

        public void ShowIconPOI(int nID, string szType, bool bVisible)
        {           
        }

        private IPopupFactory m_Factory = null;

		private MouseWorkMode m_currentMode = MouseWorkMode.NONE;

		public MouseWorkMode CurrentMouseWorkMode
		{
			get { return m_currentMode; }
			set { m_currentMode = value; }
		}

        private FormContent2D m_frmParent = null;
		private Zone m_currentIndoorZone = null;

		// key : POI id
		// value : POI 객체
		private Dictionary<int, POI> m_dicPOIs = new Dictionary<int, POI>();

		// Zone별 POI 리스트
		// Indoor View에서만 사용됨
		private Dictionary<Zone, ArrayList> m_dicZonePOIs = new Dictionary<Zone, ArrayList>();

		private bool m_isIndoor = false;

		// Panning 또는 Orbit, Zoom In/Out 등의 동작을 위하여 임시로 숨겨놓은 POI Popup 창 리스트
		private ArrayList m_arrTemporaryHiddenPOIs = new ArrayList();

		// POI Move등 수정이 가능한 모드
		private bool m_bEditMode = false;

		public bool EditMode
		{
			get { return m_bEditMode; }
			set
			{
				m_bEditMode = value;
			}
		}

		private ArrayList m_arSelectedPoi = new ArrayList();

		public ArrayList SelectedPOIList
		{
			get { return m_arSelectedPoi; }
		}

		// POI Drag 모드
		private bool m_bDragPoi = false;

		// POI Drag 시작점
		private Point m_ptPrev = new Point();

		private UnE.Geometry.Vertex3F m_vDragOrigin = new UnE.Geometry.Vertex3F();

		// POI Drag Target
		private POI m_DragCurrent = null;

		// 실내뷰에서만 사용
		private bool m_meshOpened = false;

		public bool MeshOpened
		{
			get { return m_meshOpened; }
		}

		private ArrayList m_arrLODShowingPOIs = new ArrayList();
		private Timer timer1;
		private System.ComponentModel.IContainer components;

		// Important 등급의 CCTV가 화면에 보여지게될 카메라와의 최소 거리
		private static float m_fImportanceDistance = 350.0f;
        
        public BaseViewEx2(FormContent2D frmParent, bool isIndoor = false)
        {
            m_frmParent = frmParent;
            m_isIndoor = isIndoor;

            this.MouseDown -= new MouseEventHandler(base.OnMouseDown);
            this.MouseDown += new MouseEventHandler(this.OnMouseDown);
            this.MouseMove -= new System.Windows.Forms.MouseEventHandler(base.OnMouseMove);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.MouseUp -= new MouseEventHandler(base.OnMouseUp);
            this.MouseUp += new MouseEventHandler(this.OnMouseUp);

            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.BaseViewEx_PreviewKeyDown);

            m_TooltipTimer.Tick += new EventHandler(OnShowTooltip);

            mTarget.MouseLeave += new EventHandler(OnMouseLeave);
            mTarget.MouseEnter += mTarget_MouseEnter;

            this.BackColor = Color.FromArgb(227, 226, 226);
            mMainContainer.TopToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainContainer.LeftToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainContainer.RightToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);
            mMainContainer.BottomToolStripPanel.BackColor = Color.FromArgb(227, 226, 226);

            mMainContainer.ContentPanel.Controls.Add(this);
            mMainContainer.Size = new System.Drawing.Size(1900, 1040);
            mMainContainer.Dock = DockStyle.Fill;

        }

        void mTarget_MouseEnter(object sender, EventArgs e)
        {
            int i = 0;
            i++;
        }


		public new void OnMouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			DoMouseWork(sender, e, base.OnMouseDown, MouseEvent.MOUSE_DOWN);

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            owner.EnableFireReportBtn(false);
		}

		public new void OnMouseUp(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			DoMouseWork(sender, e, base.OnMouseUp, MouseEvent.MOUSE_UP);

			if (e.Button == MouseButtons.Left)
			{
				if (m_currentMode == MouseWorkMode.PICK)
				{
					// IF NOT POI MOVE MODE
					if (m_bDragPoi == false)
						PickPOI(e.X, e.Y);
					else
					{
						if (m_DragCurrent != null)
							OnPostMovePOI(m_DragCurrent, e);

						TurnOnTemporaryList();
					}
					m_DragCurrent = null;
					m_bDragPoi = false;
				}
				else if (m_currentMode == MouseWorkMode.NEW_FIRE_SENSOR)
				{
					CreateFireSensor(e, null);
				}
				else if (m_currentMode == MouseWorkMode.NEW_COOLER_SENSOR)
				{
					CreateSpringCooler(e, null);
				}
				else if (m_currentMode == MouseWorkMode.NEW_PRESSURE_SENSOR)
				{
					CreatePumpPressure(e, null);
				}
				else if (m_currentMode == MouseWorkMode.DEL_FACILITY)
				{
					DeletePOI(e.X, e.Y);
				}
				else if (m_currentMode == MouseWorkMode.NEW_CCTV)
				{
					CreateCCTVPOI(e, null);
				}
			}
			//Invalidate(true);
		}

		public new void OnMouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			DoMouseWork(sender, e, base.OnMouseMove, MouseEvent.MOUSE_MOVE);

#if DEBUG
			//Position3D pos = GetCameraPosition();
			//Quaternion3D ori = GetCameraOrientaion();
			//Position3D dir = GetCameraDirection();

			//if (pos != null)
			//{
			//    Debug.WriteLine("POSITION : " + pos.X + "," + pos.Y + "," + pos.Z);
			//    Debug.WriteLine("DIRECTION : " + dir.X + "," + dir.Y + "," + dir.Z);
			//    Debug.WriteLine("ORIENTATION : " + ori.X + "," + ori.Y + "," + ori.Z + ","+ ori.W );
			//}
#endif
		}

		private bool m_bSaveHomeView = false;
		private Position3D m_CamPos;
		private Quaternion3D m_Quater;
		private Position3D m_CamDir;

        public void CreateCustomView()
        {
            //"POSITION"="Core.Position3D"
            //"POSITIONX"="1193.122"
            //"POSITIONY"="174.526"
            //"POSITIONZ"="-372.1257"
            //"QUATERNION"="Core.Quaternion3D"
            //"QUATERNIONX"="0.2665218"
            //"QUATERNIONY"="0.6525591"
            //"QUATERNIONZ"="0.2642059"
            //"QUATERNIONW"="-0.6582787"
            //"DIRECTION"="Core.Position3D"
            //"DIRECTIONX"="0.7182983"
            //"DIRECTIONY"="-0.6957111"
            //"DIRECTIONZ"="-0.006265521"

            m_CamPos = new Position3D(1212.822f, 193.8039f, -336.5724f);
            m_Quater = new Quaternion3D(-0.3309973f, -0.605935f, -0.3059723f, 0.6554944f);
            m_CamDir = new Position3D(0.591822f, -0.8047324f, -0.04656696f);
            WriteHomeView("Custom1");

            m_CamPos = new Position3D(1300.395f, 166.8353f, -472.8186f);
            m_Quater = new Quaternion3D(-0.2609243f, -0.6596199f, -0.2632194f, 0.6538696f);
            m_CamDir = new Position3D(0.72525f, -0.6884704f,0.006359816f);
            WriteHomeView("Custom2");
        }

		public void ReadHomeView(string szName)
		{
            string szKeyName = @"SDMS\Homview" + szName;
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKeyName);
			if (rkey == null)
			{
				m_bSaveHomeView = false;
			}
			else
			{
				float x, y, z, w;
				string pX = (string)rkey.GetValue("POSITIONX");
				string pY = (string)rkey.GetValue("POSITIONY");
				string pZ = (string)rkey.GetValue("POSITIONZ");

				if (pX == null || pY == null || pZ == null)
					return;
				if (float.TryParse(pX, out x))
				{
					if (float.TryParse(pY, out y))
					{
						if (float.TryParse(pZ, out z))
						{
							m_CamPos = new Position3D(x, y, z);
						}
						else
						{
							return;
						}
					}
					else
					{
						return;
					}
				}
				else
				{
					return;
				}

				pX = (string)rkey.GetValue("QUATERNIONX");
				pY = (string)rkey.GetValue("QUATERNIONY");
				pZ = (string)rkey.GetValue("QUATERNIONZ");
				string pW = (string)rkey.GetValue("QUATERNIONW");

				if (pX == null || pY == null || pZ == null || pW == null)
					return;

				if (float.TryParse(pX, out x))
				{
					if (float.TryParse(pY, out y))
					{
						if (float.TryParse(pZ, out z))
						{
							if (float.TryParse(pW, out w))
							{
								m_Quater = new Quaternion3D(x, y, z, w);
							}
							else
							{
								return;
							}
						}
						else
						{
							return;
						}
					}
					else
					{
						return;
					}
				}
				else
				{
					return;
				}

				pX = (string)rkey.GetValue("DIRECTIONX");
				pY = (string)rkey.GetValue("DIRECTIONY");
				pZ = (string)rkey.GetValue("DIRECTIONZ");

				if (pX == null || pY == null || pZ == null)
					return;

				if (float.TryParse(pX, out x))
				{
					if (float.TryParse(pY, out y))
					{
						if (float.TryParse(pZ, out z))
						{
							m_CamDir = new Position3D(x, y, z);
						}
						else
						{
							return;
						}
					}
					else
					{
						return;
					}
				}
				else
				{
					return;
				}

				m_bSaveHomeView = true;
			}

			if (rkey != null)
				rkey.Close();
		}

		public void WriteHomeView(string szName)
		{

            string szKeyName = @"SDMS\Homview" + szName;
			string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

			RegistrySecurity rs = new RegistrySecurity();

			rs.AddAccessRule(new RegistryAccessRule(szUserName,
				RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
				InheritanceFlags.None,
				PropagationFlags.None,
				AccessControlType.Allow));

			rs.AddAccessRule(new RegistryAccessRule(szUserName,
				RegistryRights.ChangePermissions,
				InheritanceFlags.None,
				PropagationFlags.None,
				AccessControlType.Deny));

            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKeyName, true);
			if (rkey == null)
			{
				try
				{
                    rkey = Registry.CurrentUser.CreateSubKey(szKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
				}
				catch (Exception)
				{
				}
			}

			if (rkey != null)
			{
				rkey.SetValue("POSITION", m_CamPos);
				rkey.SetValue("POSITIONX", m_CamPos.X);
				rkey.SetValue("POSITIONY", m_CamPos.Y);
				rkey.SetValue("POSITIONZ", m_CamPos.Z);

				rkey.SetValue("QUATERNION", m_Quater);
				rkey.SetValue("QUATERNIONX", m_Quater.X);
				rkey.SetValue("QUATERNIONY", m_Quater.Y);
				rkey.SetValue("QUATERNIONZ", m_Quater.Z);
				rkey.SetValue("QUATERNIONW", m_Quater.W);

				rkey.SetValue("DIRECTION", m_CamDir);
				rkey.SetValue("DIRECTIONX", m_CamDir.X);
				rkey.SetValue("DIRECTIONY", m_CamDir.Y);
				rkey.SetValue("DIRECTIONZ", m_CamDir.Z);

				rkey.Close();
			}
		}

        public void SaveViewState(string szName)
        {
            m_CamPos = GetCameraPosition();
            m_Quater = GetCameraOrientaion();
            m_CamDir = GetCameraDirection();

            WriteHomeView(szName);
        }
        public void LoadViewState(string szName)
        {
            ReadViewState(szName);

            CameraMovingAnimation(m_CamPos, m_CamDir, m_Quater);

            //OnViewFront();
			//SetCameraPosition(m_CamPos);
			//SetCameraOrientaion(m_Quater);
			//SetCameraDirection(m_CamDir);
			///base.UpdateWindow();
        }

        public void ReadViewState(string szName)
        {
            string szKeyName = @"SDMS\Homview" + szName;
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKeyName);
            if (rkey == null)
            {
                return;
            }
            else
            {
                float x, y, z, w;
                string pX = (string)rkey.GetValue("POSITIONX");
                string pY = (string)rkey.GetValue("POSITIONY");
                string pZ = (string)rkey.GetValue("POSITIONZ");

                if (pX == null || pY == null || pZ == null)
                    return;
                if (float.TryParse(pX, out x))
                {
                    if (float.TryParse(pY, out y))
                    {
                        if (float.TryParse(pZ, out z))
                        {
                            m_CamPos = new Position3D(x, y, z);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                pX = (string)rkey.GetValue("QUATERNIONX");
                pY = (string)rkey.GetValue("QUATERNIONY");
                pZ = (string)rkey.GetValue("QUATERNIONZ");
                string pW = (string)rkey.GetValue("QUATERNIONW");

                if (pX == null || pY == null || pZ == null || pW == null)
                    return;

                if (float.TryParse(pX, out x))
                {
                    if (float.TryParse(pY, out y))
                    {
                        if (float.TryParse(pZ, out z))
                        {
                            if (float.TryParse(pW, out w))
                            {
                                m_Quater = new Quaternion3D(x, y, z, w);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                pX = (string)rkey.GetValue("DIRECTIONX");
                pY = (string)rkey.GetValue("DIRECTIONY");
                pZ = (string)rkey.GetValue("DIRECTIONZ");

                if (pX == null || pY == null || pZ == null)
                    return;

                if (float.TryParse(pX, out x))
                {
                    if (float.TryParse(pY, out y))
                    {
                        if (float.TryParse(pZ, out z))
                        {
                            m_CamDir = new Position3D(x, y, z);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            if (rkey != null)
                rkey.Close();
        }

		public void SaveHomeView(string szName)
		{
			m_CamPos = GetCameraPosition();
			m_Quater = GetCameraOrientaion();
			m_CamDir = GetCameraDirection();

            WriteHomeView(szName);

			m_bSaveHomeView = true;
		}

        public void LoadHomeView(string szName)
        {
            ReadHomeView(szName);

            if (m_bSaveHomeView == true)
            {
                //SetCameraPosition();
               // SetCameraOrientaion();
               // SetCameraDirection();
            }
        }

        public void OnViewFix2(string szName)
        {
            ReadHomeView(szName);
            if (szName == "Custom1")
            {
                //OnViewFront();
                SetCameraPosition(m_CamPos);
                SetCameraOrientaion(m_Quater);
                SetCameraDirection(m_CamDir);
                //base.UpdateWindow();
                base.UpdateWindow();
            }
        }

		public void OnViewFix(string szName)
		{
			if (m_bSaveHomeView == false)
				base.OnViewFix();
			else
			{
                ReadHomeView(szName);

#if SAFE_KOREA_YH_2017
                if( szName == "Custom1" || szName == "Custom2")
                {                  
                    CameraMovingAnimation(m_CamPos, m_CamDir, m_Quater);
                }
                else
#endif
                {
                    OnViewFront();
                    SetCameraPosition(m_CamPos);
                    SetCameraOrientaion(m_Quater);
                    SetCameraDirection(m_CamDir);
                    base.UpdateWindow();
                }
                

				
			}
		}

		private void DeletePOI(int x, int y)
		{
			int nPOIID = base.OnSelectPOI(x, y);

			if (nPOIID > 0)
				DeletePOI(nPOIID);
		}

		public void SelectPOI(int nPOIID, string szType)
		{
			ClearPOISelection();

			m_arSelectedPoi.Add(nPOIID);

			base.SelectPOI(nPOIID, true);

			if (m_dicPOIs.ContainsKey(nPOIID))
			{
				POI poi = m_dicPOIs[nPOIID];

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                owner.SelectedPOI = poi;
			}
		}

		private void PickPOI(int x, int y)
		{
			int nPOIID = base.OnSelectPOI(x, y);
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			// 이미 선택된 POI인지?
			bool bSelected = base.IsPOISelected(nPOIID);

			if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (bSelected == false)
				{
					m_arSelectedPoi.Add(nPOIID);
				}
				else
				{
					m_arSelectedPoi.Remove(nPOIID);
				}
			}
			else
			{
				// Control키가 눌러지지 않는 경우 모두 클리어
				ClearPOISelection();
				bSelected = false;
			}

			// 현재 뷰에 포함된 POI인지 확인- add by skkim 2014-03-03
			if (nPOIID != -1 && m_dicPOIs.ContainsKey(nPOIID))
			{
				POI poi = m_dicPOIs[nPOIID];
				if (poi.IsIndoor == this.m_isIndoor)
					base.SelectPOI(nPOIID, !bSelected);

				if (!bSelected)
				{
					//if (m_dicPOIs.ContainsKey(nPOIID))
					{
						//POI poi = m_dicPOIs[nPOIID];
                        
                        owner.SelectedPOI = poi;
						bSelected = true;

						if (poi.Popup != null)
						{
							Point pt = (Point)this.Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
							poi.Popup.Show(pt.X, pt.Y);
						}
					}
				}
			}
        
			if (!bSelected)
            {                
                owner.SelectedPOI = null;
            }


            OnPostPick(owner.SelectedPOI);
		}

		private void DoMouseWork(Object sender, MouseEventArgs e, MouseEventHandler baseHandler, MouseEvent mouseEvent)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (m_currentMode == MouseWorkMode.PICK)
				{
					if (this.Focused == false)
						Focus();

					if (mouseEvent == MouseEvent.MOUSE_DOWN)
					{
						if (m_bEditMode == true)
						{
							// SET POI MOVE MODE
							m_ptPrev.X = e.X;
							m_ptPrev.Y = e.Y;
							int nPOIID = base.OnSelectPOI(e.X, e.Y);
                            
                            Position3D pos3d = base.Get3DPoint(e.Location);
                            System.Diagnostics.Trace.WriteLine("3D: " + pos3d.X + "," + pos3d.Z);
							if (nPOIID != -1)
							{
								if (m_dicPOIs.ContainsKey(nPOIID))
								{
									m_DragCurrent = m_dicPOIs[nPOIID];
									if (m_DragCurrent != null)
									{
										m_vDragOrigin.SetVertex(m_DragCurrent.X, m_DragCurrent.Y, m_DragCurrent.Z);
										OnPostPick(null, m_arrTemporaryHiddenPOIs, false);
									}
								}
							}
						}
					}
					else if (mouseEvent == MouseEvent.MOUSE_MOVE)
					{
						if (m_bEditMode == true)
						{
							if (e.Button == MouseButtons.Left)
							{
								int dx = e.X - m_ptPrev.X;
								int dy = e.Y - m_ptPrev.Y;
								// POI MOVE
								if (m_DragCurrent != null && dx != 0 && dy != 0)
								{
									POI poi = m_DragCurrent;
									Point pt = (Point)this.Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
									pt.X = pt.X + dx;
									pt.Y = pt.Y + dy;
									Position3D pos = Get3DPoint(pt);
									m_ptPrev.X = e.X;
									m_ptPrev.Y = e.Y;

									if (base.MovePOI(poi.ID, pos.X, pos.Y, pos.Z))
									{
										poi.X = pos.X;
										poi.Y = pos.Y;
										poi.Z = pos.Z;
										RedrawScene();

										m_bDragPoi = true;
									}
								}
							}
							else
							{
								m_bDragPoi = false;
								m_DragCurrent = null;
							}
						}
					}
				}
				else if (m_currentMode == MouseWorkMode.PANNING)
				{
					OnPrevPanning(mouseEvent);

					MouseEventArgs arg = new MouseEventArgs(MouseButtons.Middle, e.Clicks, e.X, e.Y, e.Delta);
					baseHandler(sender, arg);

					OnPostPanning(mouseEvent);
				}
				else if (m_currentMode == MouseWorkMode.NEW_FIRE_SENSOR)
				{ }
				else if (m_currentMode == MouseWorkMode.NEW_COOLER_SENSOR)
				{ }
				else if (m_currentMode == MouseWorkMode.NEW_PRESSURE_SENSOR)
				{ }
				else if (m_currentMode == MouseWorkMode.DEL_FACILITY)
				{ }
				else if (m_currentMode == MouseWorkMode.NEW_CCTV)
				{ }
				else
				{
					OnPrevOrbit(mouseEvent);
					baseHandler(sender, e);
					OnPostOrbit(mouseEvent);
				}
			}
			else
			{
				if (e.Button == System.Windows.Forms.MouseButtons.Right)
				{
                    UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                    IFormContent formContent = owner.ContentForm;
                    ToolStripMenuItem menuIndoor = formContent.GetMenu("Indoor");
                    ToolStripMenuItem menuManualReport = formContent.GetMenu("ManualReport");
                    ToolStripMenuItem menuManualCCTV = formContent.GetMenu("ManualCCTV");
					//Point pt = PointToScreen(new Point(e.X, e.Y));

					if (mouseEvent == MouseEvent.MOUSE_UP)
					{
						Point pt = PointToScreen(new Point(e.X, e.Y));
						if (this.Popup != null && this.Popup.Enabled == true)
						{
							this.Popup.Show(pt.X, pt.Y);
						}
					}
					if (mouseEvent == MouseEvent.MOUSE_DOWN)
					{
						base.OnSavePt(e);

						Position3D pos = Get3DPoint(new Point(e.X, e.Y));

                        menuIndoor.Enabled = true;

                        ToolStripItemCollection c = menuIndoor.DropDownItems;
						c.Clear();

						ToolStripItemCollection r = menuManualReport.DropDownItems;
						r.Clear();

						ToolStripItemCollection v = menuManualCCTV.DropDownItems;
						v.Clear();

						Building building = null;
						if (m_isIndoor)
						{
							if (m_currentIndoorZone != null)
							{
								//EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(m_currentIndoorZone, pos.X, pos.Z);
								//if (equipZone == null)
								{
									building = m_currentIndoorZone.Building;
									menuIndoor.Enabled = false;
									menuManualReport.Tag = m_currentIndoorZone;
									menuManualCCTV.Tag = m_currentIndoorZone;
								}
							}
						}
						else
						{
							string szBuildingName = OnPickName();
                            Position3D posCurrent = Get3DPoint(e.Location);

                            
							building = ZoneManager.Instance.GetBuilding(szBuildingName);

							if (building != null)
							{
                                
                                if( szBuildingName == "yhz1" || szBuildingName == "yhz2") 
                                {
                                    int nResult = CheckScenePosition(szBuildingName, 0, posCurrent.X);
                                    if (nResult > 0)
                                        building = ZoneManager.Instance.GetBuilding(szBuildingName+"_1");
                                }
                                else if(szBuildingName == "yhz3")
                                {
                                    int nResult = CheckScenePosition(szBuildingName, 0, posCurrent.X);
                                    if (nResult < 0)
                                        building = ZoneManager.Instance.GetBuilding(szBuildingName + "_1");
                                }


								foreach (Zone zone in building.FloorList)
								{
									ToolStripMenuItem item = new ToolStripMenuItem();
									item.Tag = zone;
                                    item.Click += formContent.IndoorMenuClick;
                                    item.Text = zone.DisplayText;
									c.Add(item);

									ToolStripMenuItem item2 = new ToolStripMenuItem();
									item2.Tag = zone;
                                    item2.Click += formContent.ManualReportClick;
                                    item2.Text = zone.DisplayText;
									r.Add(item2);

									ToolStripMenuItem item3 = new ToolStripMenuItem();
									item3.Tag = zone;
                                    item3.Click += formContent.ManualCCTVClick;
                                    item3.Text = zone.DisplayText;
									v.Add(item3);
								}

								menuManualReport.Tag = building;
								menuManualCCTV.Tag = building;
							}
							if (c.Count == 0)
								menuIndoor.Enabled = false;

							if (building == null)
							{
								Position3D pos3d = Get3DPoint(new Point(e.X, e.Y));
								Zone zone = ZoneManager.Instance.GetOutsideZone(pos3d.X, pos3d.Z);
								if (zone != null)
								{
									menuManualReport.Tag = null;
									menuManualCCTV.Tag = null;

									ToolStripMenuItem item2 = new ToolStripMenuItem();
									item2.Tag = zone;
                                    item2.Click += formContent.ManualReportClick;
                                    item2.Text = zone.DisplayText;
									r.Add(item2);

									ToolStripMenuItem item3 = new ToolStripMenuItem();
									item3.Tag = zone;
                                    item3.Click += formContent.ManualCCTVClick;
                                    item3.Text = zone.DisplayText;
									v.Add(item3);
								}
							}
						}

						if (this.m_isIndoor == false)
						{
							if (c.Count == 0)
								menuIndoor.Enabled = false;
							else
								menuIndoor.Enabled = true;
							if (v.Count == 0)
								menuManualCCTV.Enabled = false;
							else
								menuManualCCTV.Enabled = true;
							if (r.Count == 0)
								menuManualReport.Enabled = false;
							else
								menuManualReport.Enabled = true;

							if (c.Count == 0 && v.Count == 0 && r.Count == 0)
							{
                                ToolStrip ts = menuManualReport.Owner;
								ts.Enabled = false;
							}
							else
							{
								ToolStrip ts = menuManualReport.Owner;
								ts.Enabled = true;
							}
						}
					}

					return;
				}

				if (e.Button == System.Windows.Forms.MouseButtons.Middle)
					OnPrevPanning(mouseEvent);

				baseHandler(sender, e);

				if (e.Button == System.Windows.Forms.MouseButtons.Middle)
					OnPostPanning(mouseEvent);
			}

			if (e.Button == MouseButtons.None && mouseEvent == MouseEvent.MOUSE_MOVE)
			{
				ShowTooltip(e);
			}
			else
			{
				OnMouseLeave(this, new EventArgs());
			}
		}

		private int m_nShowTooltipX = 0;
		private int m_nShowTooltipY = 0;
		//private bool m_bShowTooltip = false;

		private Form m_formTooltip = null;
		private Timer m_TooltipTimer = new Timer();

		private void OnMouseLeave(object sender, EventArgs e)
		{
			m_TooltipTimer.Stop();
			m_TooltipTimer.Enabled = false;

			if (m_formTooltip != null)
				m_formTooltip.Visible = false;

			m_formTooltip = null;
		}

		private void ShowTooltip(MouseEventArgs e)
		{
			if (m_nShowTooltipX != e.X || m_nShowTooltipY != e.Y)
			{
				m_TooltipTimer.Stop();
				m_TooltipTimer.Enabled = false;

				//m_bShowTooltip = false;
				if (m_formTooltip != null)
				{
					m_formTooltip.Visible = false;
					m_formTooltip = null;
				}
			}

			if (m_formTooltip == null)
			{
				m_nShowTooltipX = e.X;
				m_nShowTooltipY = e.Y;
				m_TooltipTimer.Enabled = true;
				m_TooltipTimer.Interval = 800;
				m_TooltipTimer.Start();
				//Debug.WriteLine("X={0}, Y={1}", m_nShowTooltipX, m_nShowTooltipY);
				//Debug.WriteLine(e.ToString());
			}
		}

		private void OnShowTooltip(object sender, EventArgs e)
		{
			//m_bShowTooltip = false;

			m_TooltipTimer.Stop();
			m_TooltipTimer.Enabled = false;

			int nPoiID = OnSelectPOI(m_nShowTooltipX, m_nShowTooltipY);
			if (nPoiID != -1)
			{
				POI poi = null;
				if (m_dicPOIs.TryGetValue(nPoiID, out poi))
				{
                    if (poi.Zone == null)
                        return;
					if (poi == null || poi.Facility == null)
						return;

					if (poi.Facility.Type != IFacility.FacilityType.CCTV)
						return;

					if (m_isIndoor != poi.IsIndoor)
						return;

					Point pt = (Point)Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
					CCTV cctv = (CCTV)poi.Facility;
					m_formTooltip = new Form();

					string szName = "CCTV : " + cctv.AccessKey;
                    string szZone = "위치 : " + poi.Zone.DisplayText;
					Label lb = new Label();
					lb.AutoSize = true;
					lb.Text = szName;
					lb.Location = new Point(10, 10);

					int width1 = TextRenderer.MeasureText(lb.Text, new Font(lb.Font.FontFamily, lb.Font.Size, lb.Font.Style)).Width;

					m_formTooltip.Controls.Add(lb);

					Label lb2 = new Label();
					lb2.AutoSize = true;
					lb2.Text = szZone;
					lb2.Location = new Point(10, 28);

					int width2 = TextRenderer.MeasureText(lb2.Text, new Font(lb2.Font.FontFamily, lb2.Font.Size, lb2.Font.Style)).Width;

					int maxWidth = width1 > width2 ? width1 : width2;
					if (maxWidth < 130)
					{
						maxWidth = 130 + 20;
					}
					else
					{
						maxWidth = maxWidth + 20;
					}
					m_formTooltip.Controls.Add(lb2);

					int nTooltipHeight = 50;
					m_formTooltip.ShowInTaskbar = false;
					m_formTooltip.Size = new Size(maxWidth, nTooltipHeight);
					m_formTooltip.FormBorderStyle = FormBorderStyle.None;
					m_formTooltip.StartPosition = FormStartPosition.Manual;
					m_formTooltip.Opacity = 0.8f;
					m_formTooltip.Location = PointToScreen(new Point(pt.X - (maxWidth / 2), pt.Y - nTooltipHeight - 50));
					m_formTooltip.Show();

					//m_bShowTooltip = true;
					return;
				}
			}
		}

		private void OnPrevPanning(MouseEvent e)
		{
			if (e != MouseEvent.MOUSE_DOWN)
				return;

			OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
		}

		private void OnPrevOrbit(MouseEvent e)
		{
			if (e != MouseEvent.MOUSE_DOWN)
				return;

			OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
		}

		private void OnPostOrbit(MouseEvent e)
		{
			if (e == MouseEvent.MOUSE_UP)
				TurnOnTemporaryList();
			else
				OnScreenMove();
		}

		private void OnPostPanning(MouseEvent e)
		{
			if (e == MouseEvent.MOUSE_UP)
			{
				TurnOnTemporaryList();

				// LOD에 따라 CCTV POI들을 가시화한다.
				ProcessCCTVLOD();
			}
			else
				OnScreenMove();
		}

		public void ProcessCCTVLOD()
		{
			Type type = typeof(CCTV);
			m_arrLODShowingPOIs.Clear();

			foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
			{
				POI poi = pair.Value;

				if (poi.Popup == null || poi.Facility == null || poi.Facility.GetType() != type)
					continue;

				CCTV cctv = (CCTV)poi.Facility;

				if (cctv.LODType == CCTV.LOD.VERY_IMPORTANT)
				{
					if (IsInCamera(poi.X, poi.Y, poi.Z))
					{
						if (!poi.Popup.IsVisible())
						{
							Point pt = (Point)Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
							poi.Popup.Show(pt.X, pt.Y);
						}

						m_arrLODShowingPOIs.Add(poi);
					}
					else
					{
						IPOIPopup ctrl = poi.Popup;
						ctrl.Hide();
					}
				}
				else if (cctv.LODType == CCTV.LOD.IMPORTANT)
				{
					if (IsInCamera(poi.X, poi.Y, poi.Z) && GetPOIDistance(poi.ID) <= m_fImportanceDistance)
					{
						if (!poi.Popup.IsVisible())
						{
							Point pt = (Point)Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
							poi.Popup.Show(pt.X, pt.Y);
						}

						m_arrLODShowingPOIs.Add(poi);
					}
					else
					{
                        IPOIPopup ctrl = poi.Popup;
						ctrl.Hide();
					}
				}
			}
		}

		private void TurnOnTemporaryList()
		{
			foreach (POI poi in m_arrTemporaryHiddenPOIs)
			{
				Point pt = (Point)Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
				poi.Popup.Show(pt.X, pt.Y);
			}

			m_arrTemporaryHiddenPOIs.Clear();
		}

		private void OnPostMovePOI(POI poi, MouseEventArgs e)
		{
			float fDistance = m_vDragOrigin.GetDistance(new UnE.Geometry.Vertex3F(poi.X, poi.Y, poi.Z));
			if (fDistance <= UnE.Geometry.Math.HALF_TOLERANCE())
				return;

			if (poi.Type == IFacility.FacilityType.CCTV)
			{
				AddCCTVEditData(poi, e);
			}
		}

		private void AddCCTVEditData(POI poi, MouseEventArgs e)
		{
			CCTV cctv = (CCTV)poi.Facility;
			if (cctv == null)
				return;

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();


			EditCCTV editCCTV = new EditCCTV(cctv);
			editCCTV.Position = new UnE.Geometry.Vertex3F(poi.X , poi.Y, poi.Z);
			editCCTV.Zone = GetPOIZone(e, poi.X, poi.Y, poi.Z);
			editCCTV.AddToManager(owner.IChangedDataManager);

			poi.Zone = editCCTV.Zone;
		}

		private void TurnOnWheelTemporaryList()
		{
			foreach (POI poi in m_arrTemporaryHiddenPOIsForWheel)
			{
				Point pt = (Point)Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
				poi.Popup.Show(pt.X, pt.Y);
			}

			m_arrTemporaryHiddenPOIsForWheel.Clear();
		}

		public void HideAllPOIPopup()
		{
			OnPostPick(null, null, true);
		}

		private void OnPostPick(POI poi, ArrayList arrHidden = null, bool absolutely = false)
		{
			bool refresh = false;

			if (arrHidden != null)
				arrHidden.Clear();

			if (m_isIndoor)
			{
				if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
				{
					ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

					foreach (POI _poi in arrPOIs)
					{
						if (_poi == poi || _poi.Popup == null || !_poi.Popup.IsVisible())
							continue;

						if (arrHidden != null)// && IsLODShowingPOI(_poi))
							arrHidden.Add(_poi);

						_poi.Popup.Hide(absolutely);
						refresh = true;
					}
				}
			}
			else
			{
				foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
				{
					if (pair.Value == poi || pair.Value.Popup == null || !pair.Value.Popup.IsVisible())
						continue;

					if (arrHidden != null)// && IsLODShowingPOI(pair.Value))
						arrHidden.Add(pair.Value);

					pair.Value.Popup.Hide(absolutely);
					refresh = true;
				}
			}

			if (poi != null)
            {
                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                owner.OnPostPickPOI(poi);
            }
            

			if (refresh)
			{
				Update();
			}
		}

		private bool IsLODShowingPOI(POI poi)
		{
			return m_arrLODShowingPOIs.Contains(poi);
		}

		private void OnScreenMove()
		{
			bool refresh = false;

			if (m_isIndoor)
			{
				if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
				{
					ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

					foreach (POI poi in arrPOIs)
					{
						if (OnMovePOI(poi))
							refresh = true;
					}
				}
			}
			else
			{
				foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
				{
					if (OnMovePOI(pair.Value))
						refresh = true;
				}
			}

			if (refresh)
			{
				Update();
			}
		}

		private bool OnMovePOI(POI poi)
		{
			IPOIPopup popup = poi.Popup;

			if (popup != null && popup.IsVisible())
			{
				Point pt = (Point)Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
				popup.Show(pt.X, pt.Y);
				return true;
			}

			return false;
		}

		public POI CreateFireSensor(MouseEventArgs e, Zone zone)
		{
			Position3D pos = Get3DPoint(new Point(e.X, e.Y));

			FireSensor sensor = new FireSensor();
			POI poi = new POI();

			poi.X = pos.X;
			poi.Y = pos.Y;
			poi.Z = pos.Z;
			poi.Facility = sensor;
			//poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
			poi.IsIndoor = m_isIndoor;
            
            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }
			poi.Popup = sensor.CreatePopup(this, m_Factory);

			if (m_isIndoor)
			{
				poi.Zone = zone == null ? m_currentIndoorZone : zone;
			}
			else
			{
				poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
			}

			EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(poi.Zone, pos.X, pos.Z);
			if (equipZone == null)
				return null;
			sensor.EquipZoneID = equipZone.ID;

			string strPath = GetIconPath(sensor.IconPath);
			int nID = AddPOI(strPath, pos.X, pos.Y, pos.Z);
			poi.ID = nID;
			// set pick size;
			base.SetPickSize(nID, 55, 55);

			m_dicPOIs[nID] = poi;

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			owner.SelectedPOI = poi;

			m_frmParent.Layers.GetLayer(ID.ID_LAYER_DETECTOR).Add(nID);

			if (m_isIndoor && poi.Zone != null)
			{
				ArrayList arrPOIs = null;

				if (m_dicZonePOIs.ContainsKey(poi.Zone))
					arrPOIs = m_dicZonePOIs[poi.Zone];
				else
				{
					arrPOIs = new ArrayList();
					m_dicZonePOIs[poi.Zone] = arrPOIs;
				}

				if (!arrPOIs.Contains(poi))
					arrPOIs.Add(poi);
			}

			EditFireSensor editFireSensor = new EditFireSensor(sensor);
			editFireSensor.AddToManager(owner.IChangedDataManager);

			return poi;
		}

		public POI CreateSpringCooler(MouseEventArgs e, Zone zone)
		{
			Position3D pos = Get3DPoint(new Point(e.X, e.Y));

			SpringCooler sensor = new SpringCooler();
			POI poi = new POI();
			poi.X = pos.X;
			poi.Y = pos.Y;
			poi.Z = pos.Z;
			poi.Facility = sensor;
			//poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
			poi.IsIndoor = m_isIndoor;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }

            poi.Popup = sensor.CreatePopup(this, m_Factory);

			if (m_isIndoor)
			{
				poi.Zone = zone == null ? m_currentIndoorZone : zone;
			}
			else
			{
				poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
			}
			EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(poi.Zone, pos.X, pos.Z);
			if (equipZone == null)
				return null;
			sensor.EquipZoneID = equipZone.ID;
			string strPath = GetIconPath(sensor.IconPath);
			int nID = AddPOI(strPath, pos.X, pos.Y, pos.Z);
			poi.ID = nID;
			// set pick size;
			base.SetPickSize(nID, 55, 55);

			m_dicPOIs[nID] = poi;

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			owner.SelectedPOI = poi;

			m_frmParent.Layers.GetLayer(ID.ID_LAYER_COOLER).Add(nID);

			if (m_isIndoor && poi.Zone != null)
			{
				ArrayList arrPOIs = null;

				if (m_dicZonePOIs.ContainsKey(poi.Zone))
					arrPOIs = m_dicZonePOIs[poi.Zone];
				else
				{
					arrPOIs = new ArrayList();
					m_dicZonePOIs[poi.Zone] = arrPOIs;
				}

				if (!arrPOIs.Contains(poi))
					arrPOIs.Add(poi);
			}

			EditSpringCooler editSpringCooler = new EditSpringCooler(sensor);
			editSpringCooler.AddToManager(owner.IChangedDataManager);

			return poi;
		}

		public POI CreatePumpPressure(MouseEventArgs e, Zone zone)
		{
			Position3D pos = Get3DPoint(new Point(e.X, e.Y));

			PumpPressureSensor sensor = new PumpPressureSensor();

			POI poi = new POI();
			poi.X = pos.X;
			poi.Y = pos.Y;
			poi.Z = pos.Z;
			poi.Facility = sensor;
			//poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
			poi.IsIndoor = m_isIndoor;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }

            poi.Popup = sensor.CreatePopup(this, m_Factory);

			if (m_isIndoor)
			{
				poi.Zone = zone == null ? m_currentIndoorZone : zone;
			}
			else
			{
				poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
			}
			EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(poi.Zone, pos.X, pos.Z);
			if (equipZone == null)
			{
				sensor.EquipZoneID = 0;
			}
			else
				sensor.EquipZoneID = equipZone.ID;
			string strPath = GetIconPath(sensor.IconPath);
			int nID = AddPOI(strPath, pos.X, pos.Y, pos.Z);
			poi.ID = nID;
			// set pick size;
			base.SetPickSize(nID, 55, 55);

			m_dicPOIs[nID] = poi;

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			owner.SelectedPOI = poi;

			m_frmParent.Layers.GetLayer(ID.ID_LAYER_PERSURE).Add(nID);

			if (m_isIndoor && poi.Zone != null)
			{
				ArrayList arrPOIs = null;

				if (m_dicZonePOIs.ContainsKey(poi.Zone))
					arrPOIs = m_dicZonePOIs[poi.Zone];
				else
				{
					arrPOIs = new ArrayList();
					m_dicZonePOIs[poi.Zone] = arrPOIs;
				}

				if (!arrPOIs.Contains(poi))
					arrPOIs.Add(poi);
			}

			EditPumpPressuerSensor editPump = new EditPumpPressuerSensor(sensor);
			editPump.AddToManager(owner.IChangedDataManager);

			return poi;
		}

		public POI CreateCCTVPOI(MouseEventArgs e, Zone zone)
		{
			Position3D pos = Get3DPoint(new Point(e.X, e.Y));

			base.OnSavePt(e);
			string szName = base.OnPickName();
			Building building = ZoneManager.Instance.GetBuilding(szName);

			CCTV cctv = new CCTV();

			string strPath = GetIconPath(cctv.IconPath);// Application.StartupPath + "\\Media\\icons\\비산먼지.ico";
			int nID = AddPOI(strPath, pos.X, pos.Y, pos.Z);

			// set pick size;
			base.SetPickSize(nID, 55, 55);

			POI poi = new POI();
			poi.ID = nID;
            poi.X = pos.X;
            poi.Y = pos.Y;
            poi.Z = pos.Z;
			poi.Facility = cctv;
			//poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
			poi.IsIndoor = m_isIndoor;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }

            poi.Popup = cctv.CreatePopup(this, m_Factory);

			if (m_isIndoor)
			{
				poi.Zone = zone == null ? m_currentIndoorZone : zone;
			}
			else
			{
				poi.Zone = zone == null ? GetPOIZone(e, pos.X, pos.Y, pos.Z) : zone;
			}

			m_dicPOIs[nID] = poi;

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			owner.SelectedPOI = poi;

			m_frmParent.Layers.GetLayer(ID.ID_LAYER_CCTV).Add(nID);

			if (m_isIndoor && poi.Zone != null)
			{
				ArrayList arrPOIs = null;

				if (m_dicZonePOIs.ContainsKey(poi.Zone))
					arrPOIs = m_dicZonePOIs[poi.Zone];
				else
				{
					arrPOIs = new ArrayList();
					m_dicZonePOIs[poi.Zone] = arrPOIs;
				}

				if (!arrPOIs.Contains(poi))
					arrPOIs.Add(poi);
			}

			EditCCTV editCCTV = new EditCCTV(cctv);
			if (building != null)
			{
				editCCTV.Description = building.BroadcastName;
			}
			editCCTV.AddToManager(owner.IChangedDataManager);

			return poi;
		}

		private ArrayList m_arFireEquips = new ArrayList();

		public POI CreateFireEquipmentPOI(float x, float y, float z, FireEquipment equip, Zone zone)
		{
			string strPath = GetIconPath(equip.IconPath);
			int nID = AddPOI(strPath, x, y, z);

			// set pick size;
			base.SetPickSize(nID, 55, 55);

			POI poi = new POI();
			poi.ID = nID;
			poi.X = x;
			poi.Y = y;
			poi.Z = z;
			poi.Facility = equip;
			poi.IsIndoor = m_isIndoor;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }

            poi.Popup = equip.CreatePopup(this, m_Factory);

			if (zone == null)
			{
				if (m_isIndoor)
					poi.Zone = m_currentIndoorZone;
				else
				{
					Position3D pos = new Position3D(x, y, z);
					Point pt = (Point)Get2DPoint(pos);

					MouseEventArgs e = new MouseEventArgs(MouseButtons.Right, 0, pt.X, pt.Y, 0);
					poi.Zone = GetPOIZone(e, pos.X, pos.Y, pos.Z);
				}
			}
			else
				poi.Zone = zone;

			m_dicPOIs[nID] = poi;

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			owner.SelectedPOI = poi;
			m_frmParent.Layers.GetLayer(equip.GetLayerID()).Add(nID);

			if (m_isIndoor && poi.Zone != null)
			{
				ArrayList arrPOIs = null;

				if (m_dicZonePOIs.ContainsKey(poi.Zone))
					arrPOIs = m_dicZonePOIs[poi.Zone];
				else
				{
					arrPOIs = new ArrayList();
					m_dicZonePOIs[poi.Zone] = arrPOIs;
				}

				if (!arrPOIs.Contains(poi))
					arrPOIs.Add(poi);

				m_arFireEquips.Add(poi);
			}
			return poi;
		}

		private Zone GetPOIZone(MouseEventArgs e, float x, float y, float z)
		{
			if (m_isIndoor)
			{
				float nCurrentFloorIndex = -1.0f;
				Building building = m_frmParent.GetCurrentBuilding(ref nCurrentFloorIndex);

				if (building == null)
					return null;

				return ZoneManager.Instance.GetZone(building.BuildingID, nCurrentFloorIndex);
			}

			MouseEventArgs arg = new MouseEventArgs(MouseButtons.Right, e.Clicks, e.X, e.Y, e.Delta);
			//base.OnMouseUp(this, arg);
			base.OnSavePt(arg);
			string strBuildingID = OnSelect();
			if (strBuildingID == "")
			{
                ClearSelect();
				return ZoneManager.Instance.GetOutsideZone(x, z);
			}
			else
			{
                ClearSelect();
				Building building = ZoneManager.Instance.GetBuilding(strBuildingID);
				if (building != null)
				{
					Zone zone = ZoneManager.Instance.GetZone(strBuildingID, building.MaxFloorIndex - 1);
					if (zone == null)
					{
						return ZoneManager.Instance.GetOutsideZone(x, z);
					}
					return zone;
				}
			}
			return ZoneManager.Instance.GetOutsideZone(x, z);
		}

		public POI FindPOI(int nID)
		{
			if (m_dicPOIs.ContainsKey(nID))
				return m_dicPOIs[nID];

			return null;
		}

        public POI FindPOI(string s )
        {
            return null;
        }

        public POI FindPOI(int nID, string szType)
        {
            if (m_dicPOIs.ContainsKey(nID))
                return m_dicPOIs[nID];

            return null;
        }


		public bool DeletePOI(int nID)
		{
			if (!m_dicPOIs.ContainsKey(nID))
				return false;

			POI poi = m_dicPOIs[nID];

			if (m_dicPOIs.Remove(nID))
			{

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();

				POI poiSelected = owner.SelectedPOI;
				if (poiSelected != null && poiSelected.ID == nID)
					owner.SelectedPOI = null;

				if (poi.Facility != null)
					m_frmParent.Layers.GetLayer(poi.Facility.GetLayerID()).Remove(nID);
				RemovePOI(nID);

				if (poi.Zone != null)
				{
					if (m_dicZonePOIs.ContainsKey(poi.Zone))
					{
						ArrayList arrPOIs = m_dicZonePOIs[poi.Zone];
						arrPOIs.Remove(poi);
					}
				}

				OnPostDeletePOI(poi);
				return true;
			}

			return false;
		}

		private void OnPostDeletePOI(POI poi)
		{
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			switch (poi.Type)
			{
				case IFacility.FacilityType.CCTV:
					EditCCTV cctv = new EditCCTV((CCTV)poi.Facility);
					cctv.IsDeleting = true;
					cctv.AddToManager(owner.IChangedDataManager);
					break;

                case IFacility.FacilityType.FIRE_SENSOR:
					EditFireSensor fireSensor = new EditFireSensor((FireSensor)poi.Facility);
					fireSensor.IsDeleting = true;
                    fireSensor.AddToManager(owner.IChangedDataManager);
					break;

                case IFacility.FacilityType.COOLER_SENSOR:
					EditSpringCooler coolingSensor = new EditSpringCooler((SpringCooler)poi.Facility);
					coolingSensor.IsDeleting = true;
                    coolingSensor.AddToManager(owner.IChangedDataManager);
					break;

                case IFacility.FacilityType.PRESSURE_SENSOR:
					EditPumpPressuerSensor pressureSensor = new EditPumpPressuerSensor((PumpPressureSensor)poi.Facility);
					pressureSensor.IsDeleting = true;
                    pressureSensor.AddToManager(owner.IChangedDataManager);
					break;
			}
		}

		public void DeleteAllPOIs()
		{
			foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
			{
				m_frmParent.Layers.GetLayer(ID.ID_LAYER_CCTV).Remove(pair.Key);
				m_frmParent.Layers.GetLayer(ID.ID_LAYER_CCTVLOW).Remove(pair.Key);
                m_frmParent.Layers.GetLayer(ID.ID_LAYER_CCTV_DISCONNECTED).Remove(pair.Key);

				if (pair.Value.Popup != null)
					pair.Value.Popup.Close();
				RemovePOI(pair.Key);
			}

			if (m_isIndoor && m_currentIndoorZone != null)
			{
				if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
				{
					ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];
					arrPOIs.Clear();
				}
			}

			m_dicPOIs.Clear();

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			owner.SelectedPOI = null;
		}

        private string GetIconPath(string szPath)
        {
            if (szPath.IndexOf("\\Media\\icons\\") != -1)
                return szPath;
            string szType = szPath.ToLower();

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            string resultPath = owner.ResourcePath + string.Format("\\Media\\icons\\{0}.ico", szType);
            return resultPath;
        }

		public void AddPOI(POI poi)
		{
			poi.ParentView = this;

			if (poi.Facility == null)
				return;

            if (m_Factory == null)
            {
                m_Factory = PopupFactoryHelper.GetFactory();
            }

			if (poi.Popup == null)
                poi.Popup = poi.Facility.CreatePopup(this, m_Factory);

			if (!m_isIndoor || (m_isIndoor && poi.Zone == m_currentIndoorZone))
			{
                string strIconPath = GetIconPath(poi.Facility.IconPath);
				int nID = base.AddPOI(strIconPath, poi.X, poi.Y, poi.Z);
				poi.ID = nID;
				m_dicPOIs[nID] = poi;
			}
			else if (poi.ID > 0)
				m_dicPOIs[poi.ID] = poi;

			if (m_isIndoor && poi.Zone != null)
			{
				if (m_dicZonePOIs.ContainsKey(poi.Zone))
				{
					ArrayList arrPOIs = m_dicZonePOIs[poi.Zone];
					arrPOIs.Add(poi);
				}
				else
				{
					ArrayList arrPOIs = new ArrayList();
					m_dicZonePOIs[poi.Zone] = arrPOIs;
					arrPOIs.Add(poi);
				}
			}

			int nLayerID = poi.Facility.GetLayerID();
			m_frmParent.Layers.GetLayer(nLayerID).Add(poi.ID);
		}

		// 실내뷰에서만 사용
		public void OpenMesh(string strPath, Zone zone)
		{
			if (!m_isIndoor)
				return;

			ArrayList arrPrevPOIs = null;
			ArrayList arrNextPOIs = null;

			if (m_currentIndoorZone != null)
			{
				if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
				{
					ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];
					arrPrevPOIs = arrPOIs;

					foreach (POI poi in arrPOIs)
					{
						// 뷰가 바뀌어서 없애는 것이므로 3d 뷰에서만 삭제하고 dictionary에는 남겨둔다.
						if (poi.Facility != null)
						{
							int nLayerID = poi.Facility.GetLayerID();
							m_frmParent.Layers.GetLayer(nLayerID).Remove(poi.ID);
						}

						base.RemovePOI(poi.ID);

						if (poi.Popup != null)
						{
							poi.Popup.Close();
							poi.Popup = null;
						}
					}
				}
			}

			ClearFireEquipments();
			base.OpenMesh(strPath);
			m_meshOpened = true;

			m_currentIndoorZone = zone;

			if (m_currentIndoorZone != null)
			{
				if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
				{
					ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];
					arrNextPOIs = arrPOIs;

					foreach (POI poi in arrPOIs)
					{
						if (poi.Facility == null)
							continue;

						string strIconPath = GetIconPath(poi.Facility.IconPath);

						if (poi.Facility.Connected == false)
						{
                            strIconPath = GetIconPath(poi.Facility.DisconnectIconPath);
						}

						int nID = base.AddPOI(strIconPath, poi.X, poi.Y, poi.Z);

						poi.ID = nID;
						m_dicPOIs[nID] = poi;


                        if (poi.Popup == null && poi.Facility != null)
                        {
                            if (m_Factory == null)
                            {
                                m_Factory = PopupFactoryHelper.GetFactory();
                            }
                            poi.Popup = poi.Facility.CreatePopup(this, m_Factory);
                        }

						int nLayerID = poi.Facility.GetLayerID();
						m_frmParent.Layers.GetLayer(nLayerID).Add(poi.ID);
					}
				}

				LoadFireEquipments();

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
				owner.ChangeZoneComboBox(m_currentIndoorZone);
			}

			ProcessCCTVLOD();
		}

		private Timer m_WheelTimer = null;
		private static bool m_bWheelProcess = false;
		private ArrayList m_arrTemporaryHiddenPOIsForWheel = new ArrayList();

		public new void OnMouseWheel(int x, int y, int delta)
		{
			m_bWheelProcess = true;

			if (m_WheelTimer == null || m_WheelTimer.Enabled == false)
			{
				if (m_WheelTimer == null)
				{
					m_WheelTimer = new System.Windows.Forms.Timer();
					m_WheelTimer.Interval = 600;
					m_WheelTimer.Tick += new System.EventHandler(OnWheelTimerTick);
				}

				m_WheelTimer.Enabled = true;
				m_WheelTimer.Start();

				OnPostPick(null, m_arrTemporaryHiddenPOIsForWheel, true);
			}

			base.OnMouseWheel(x, y, delta);
			OnScreenMove();

			m_bWheelProcess = false;
		}

		private void OnWheelTimerTick(object sender, EventArgs e)
		{
			if (m_bWheelProcess == false)
			{
				TurnOnWheelTemporaryList();
				ProcessCCTVLOD();
			}
			m_WheelTimer.Enabled = false;
			m_WheelTimer.Stop();
		}

		public void ShowLayer(int nLayer, bool bShow)
		{
			foreach (KeyValuePair<int, POI> kv in m_dicPOIs)
			{
				POI poi = kv.Value;
				if (poi.Popup != null)
				{
					if (poi.Facility.GetLayerID() == nLayer)
					{
						poi.Popup.LayerVisible = bShow;
					}
				}
			}
		}

		public void UpdatePOI()
		{
			OnScreenMove();
		}

		public bool IsTemporaryHiddenPOI(POI poi)
		{
			bool bResult = m_arrTemporaryHiddenPOIs.Contains(poi);
			bool bResult2 = m_arrTemporaryHiddenPOIsForWheel.Contains(poi);
			return (bResult || bResult2);
		}

		public const int WM_PAINT = 0x000F;
		public const int WM_ERASEBKGND = 0x0014;

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_ERASEBKGND)
			{
				m.Result = new IntPtr(0);
				return;
			}

			base.WndProc(ref m);
		}

		private void LoadFireEquipments()
		{
			if (m_isIndoor)
			{

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();

				ArrayList arrEquipments = owner.GetFireEquipments(this.m_currentIndoorZone);

				if (arrEquipments != null)
				{
					foreach (FireEquipment equip in arrEquipments)
					{
						if (equip.GroupID != -1)
							CreateFireEquipmentPOI(equip.X, equip.Y, equip.Z, equip, equip.Zone);
					}
				}
			}
		}

		private void ClearFireEquipments()
		{
			m_frmParent.Layers.GetLayer(ID.ID_LAYER_FIREEXT).Objects.Clear();
			m_frmParent.Layers.GetLayer(ID.ID_LAYER_FIREHYD).Objects.Clear();
			m_frmParent.Layers.GetLayer(ID.ID_LAYER_ALARMSTA).Objects.Clear();

			foreach (POI poi in m_arFireEquips)
			{
				ArrayList arrPOIs = null;
				if (m_dicZonePOIs.ContainsKey(poi.Zone))
				{
					arrPOIs = m_dicZonePOIs[poi.Zone];

					if (arrPOIs.Contains(poi))
						arrPOIs.Remove(poi);
				}
			}
			m_arFireEquips.Clear();
		}

		public new void ClearAllData()
		{
			if (m_currentIndoorZone != null)
			{
				if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
				{
					ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

					foreach (POI poi in arrPOIs)
					{
						// 뷰가 바뀌어서 없애는 것이므로 3d 뷰에서만 삭제하고 dictionary에는 남겨둔다.
						base.RemovePOI(poi.ID);

						if (poi.Popup != null)
						{
							poi.Popup.Close();
							poi.Popup = null;
						}
					}
				}
			}

			base.ClearAllData();
		}

		public void ClearPOISelection()
		{
			base.ClearAllSelectedPOI();
			m_arSelectedPoi.Clear();
		}

		public new void OnViewHome()
		{
			OnPostPick(null, m_arrTemporaryHiddenPOIs, true);

			base.OnViewHome();

			TurnOnTemporaryList();
		}

		public new void OnViewTop()
		{
			OnPostPick(null, m_arrTemporaryHiddenPOIs, true);

			base.OnViewTop();

			TurnOnTemporaryList();
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 600;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // mTarget
            // 
            this.Name = "BaseViewExe";
            this.VisibleChanged += new System.EventHandler(this.mTarget_VisibleChanged);
            this.ParentChanged += new System.EventHandler(this.mTarget_ParentChanged);
            this.ResumeLayout(false);

		}

		private void BaseViewEx_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			owner.EnableFireReportBtn(false);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			//this.UpdateWindow();
		}

        private void mTarget_VisibleChanged(object sender, EventArgs e)
        {
            if( this.m_isIndoor == true)
            {
                
                if( this.Visible == false)
                {
                    int i = 0;
                    i++;
                }

            }
        }

        private void mTarget_ParentChanged(object sender, EventArgs e)
        {
            if (this.m_isIndoor == true)
            {

                if (this.Visible == false)
                {
                    int i = 0;
                    i++;
                }

            }
        }

        public void EnablePOI(int nID, string szType, bool bEnable)
        {

        }

        System.Timers.Timer m_timerEarthquake;
        System.Timers.Timer m_timerEarthquake2;
        int m_timeDue = 5000;
        int m_nMoveScale = 10;
        Stopwatch sw = new Stopwatch();
        private bool m_timerEarthquakeIsFinished = false, m_timerEarthquake2IsFinished = false;

        public bool EarthquakeMotionIsFinished()
        {
            return m_timerEarthquakeIsFinished && m_timerEarthquake2IsFinished;
        }

        public void InitEarthquakeMotion()
        {
            m_timerEarthquakeIsFinished = false;
            m_timerEarthquake2IsFinished = false;
        }

        public void SetEarthquakeMotion(int nMilliSec, int scale, int colorTime)
        {
            m_timeDue = nMilliSec;
            m_nMoveScale = scale;
            m_timerEarthquake = new System.Timers.Timer();
            m_timerEarthquake.Interval = 50;
            m_timerEarthquake.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            m_timerEarthquake2 = new System.Timers.Timer();
            m_timerEarthquake2.Interval = colorTime;
            m_timerEarthquake2.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed2);

            sw.Reset();
            sw.Start();
            m_timerEarthquake.Start();
            m_timerEarthquake2.Start();
            //base.EarthquakeMotion(true);
        }

        bool bFirst = true;
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Form formInvoke = UnE.View.Content.ViewUtils.InvokeForm;
            formInvoke.Invoke((MethodInvoker)delegate
            {
                if (sw.ElapsedMilliseconds > m_timeDue)
                {
                    m_timerEarthquake.Stop();
                    m_timerEarthquake.Enabled = false;
                    //base.EarthquakeMotion(false);
                    this.RedrawScene();
                    m_timerEarthquakeIsFinished = true;

#if SAFE_KOREA_YH_2017
                    if (IsAfterQuake() == false)
                        SDMS.ScriptProxy.Instance.UserObject.SDMSShowBuildingCollapsed.Invoke("yhz85", "1~4호기 기계공작실");
#endif

                    return;
                }

                Position3D pos = GetCameraPosition();
                if (bFirst)
                    SetCameraPosition(new Position3D(pos.X + m_nMoveScale, pos.Y, pos.Z));
                else
                    SetCameraPosition(new Position3D(pos.X - m_nMoveScale, pos.Y, pos.Z));

                this.RedrawScene();
            });

            bFirst = !bFirst;
        }

#if SAFE_KOREA_YH_2017
        // 여진인가?
        private bool IsAfterQuake()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'IsAfterQuake' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return false;

            if (strValue == "1")
                return true;

            return false;
        }
#endif

        bool bFirst2 = true;
        void timer_Elapsed2(object sender, System.Timers.ElapsedEventArgs e)
        {
            Form formInvoke = UnE.View.Content.ViewUtils.InvokeForm;
            formInvoke.Invoke((MethodInvoker)delegate
            {
                if (sw.ElapsedMilliseconds > m_timeDue)
                {
                    m_timerEarthquake2.Stop();
                    m_timerEarthquake2.Enabled = false;
                    base.EarthquakeMotion(false);
                    this.RedrawScene();
                    m_timerEarthquake2IsFinished = true;
                    return;
                }

                Position3D pos = GetCameraPosition();
                if (bFirst2)
                    base.EarthquakeMotion(true);
                else
                    base.EarthquakeMotion(false);

                this.RedrawScene();
            });

            bFirst2 = !bFirst2;
        }

        public int AddPOI(string szPaht)
        {
            return -1;
        }

    
	}
}