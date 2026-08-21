using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Core;
using Microsoft.Win32;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;
using System.Threading;

using System.Diagnostics;
using System.Security.Policy;

namespace HSMS
{
    public class BaseViewEx : BaseView
    {
        public enum MouseWorkMode { NONE = 0, PICK, PANNING, ORBIT, NEW_FIRE_SENSOR, NEW_COOLER_SENSOR, NEW_PRESSURE_SENSOR, NEW_CCTV, DEL_FACILITY };
        public enum MouseEvent { MOUSE_DOWN = 0, MOUSE_UP, MOUSE_MOVE };

        private MouseWorkMode m_currentMode = MouseWorkMode.NONE;
        public MouseWorkMode CurrentMouseWorkMode
        {
            get { return m_currentMode; }
            set { m_currentMode = value; }
        }

        private FormContent m_frmParent = null;
      
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

        Dictionary<string,ZoneVolume> m_arZoneVolumes = new Dictionary<string,ZoneVolume>();
        public Dictionary<string, ZoneVolume> ZoneVolumes
        {
            get { return m_arZoneVolumes; }
            set { m_arZoneVolumes = value; }
        }

        ArrayList m_arSelectedPoi = new ArrayList();
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
        //private POI m_DragCurrent = null;

        // 실내뷰에서만 사용
        private bool m_meshOpened = false;

        public bool MeshOpened
        {
            get { return m_meshOpened; }
        }

        private ArrayList m_arrLODShowingPOIs = new ArrayList();
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

        // Important 등급의 CCTV가 화면에 보여지게될 카메라와의 최소 거리
        //private static float m_fImportanceDistance = 350.0f;

	    public BaseViewEx(FormContent frmParent, bool isIndoor = false)
        {
            m_frmParent = frmParent;
            m_isIndoor = isIndoor;

			mTarget.MouseDown -= new MouseEventHandler(base.OnMouseDown);
			mTarget.MouseDown += new MouseEventHandler(this.OnMouseDown);
			mTarget.MouseMove -= new System.Windows.Forms.MouseEventHandler(base.OnMouseMove);
			mTarget.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
			mTarget.MouseUp -= new MouseEventHandler(base.OnMouseUp);
			mTarget.MouseUp += new MouseEventHandler(this.OnMouseUp);

			mTarget.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.BaseViewEx_PreviewKeyDown);

			m_TooltipTimer.Tick += new EventHandler(OnShowTooltip);

			mTarget.MouseLeave += new EventHandler(OnMouseLeave);
        }

        public new void OnMouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            DoMouseWork(sender, e, base.OnMouseDown, MouseEvent.MOUSE_DOWN);
			
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
                        TurnOnTemporaryList();
                    }
                    m_bDragPoi = false;
                }
                else if (m_currentMode == MouseWorkMode.NEW_FIRE_SENSOR)
                {
                }
                else if (m_currentMode == MouseWorkMode.NEW_COOLER_SENSOR)
                {
                }
                else if (m_currentMode == MouseWorkMode.NEW_PRESSURE_SENSOR)
                {
                }
                else if (m_currentMode == MouseWorkMode.DEL_FACILITY)
                {
                    DeletePOI(e.X, e.Y);
                }
                else if (m_currentMode == MouseWorkMode.NEW_CCTV)
                {
                }
            }
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

		public void ReadHomeView()
		{
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"Software\UNE\HSMS\Homview");
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


		public void WriteHomeView()
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


            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(@"Software\UNE\HSMS\Homview", true);
			if (rkey == null)
			{
				try
				{
                    rkey = Registry.CurrentUser.CreateSubKey(@"Software\UNE\HSMS\Homview", RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
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

		public void SaveHomeView()
		{
			m_CamPos = GetCameraPosition();
			m_Quater = GetCameraOrientaion();
			m_CamDir = GetCameraDirection();

			WriteHomeView();

			m_bSaveHomeView = true;
		}

		public override void OnViewFix()
		{
			if (m_bSaveHomeView == false)
				base.OnViewFix();
			else
			{
				OnViewFront();
				SetCameraPosition(m_CamPos);
				SetCameraOrientaion(m_Quater);
				SetCameraDirection(m_CamDir);
				base.UpdateWindow();
			}
		}

        private void DeletePOI(int x, int y)
        {
            int nPOIID = base.OnSelectPOI(x, y);
        }

        public void SelectPOI(int nPOIID)
        {
            ClearPOISelection();

            m_arSelectedPoi.Add(nPOIID);

            base.SelectPOI(nPOIID, true);
        }

        private void PickPOI(int x, int y)
        {
            int nPOIID = base.OnSelectPOI(x, y);         

            // 이미 선택된 POI인지?
            bool bSelected = base.IsPOISelected(nPOIID);

            if (!bSelected)
            {
                base.SelectPOI(nPOIID, true);
                m_frmParent.OnSelectedPOI(nPOIID);
                base.RedrawScene();
            }
            else
            {
                
            }
            
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
                            if (nPOIID != -1)
                            {                              
                                
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
							}
							else
							{
								m_bDragPoi = false;								
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
				//ShowTooltip(e);

                ShowZoneVolume(e);
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
        private System.Windows.Forms.Timer m_TooltipTimer = new System.Windows.Forms.Timer();

		private void OnMouseLeave(object sender, EventArgs e)
		{
			m_TooltipTimer.Stop();
			m_TooltipTimer.Enabled = false;

			if (m_formTooltip != null)
				m_formTooltip.Visible = false;

			m_formTooltip = null;
		}

        private void ShowZoneVolumeThread(object param)
        {
            MouseEventArgs e = (MouseEventArgs)param;

            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                base.OnSavePt(e);

                Position3D pos = Get3DPoint(new Point(e.X, e.Y));

                DataManager dataManger = FormMain.Instance.DataMgr;
                int nZoneGroupCount = dataManger.GetZoneGroupCount();

                for (int i = 0; i < nZoneGroupCount; i++)
                {
                    ZoneGroup group = dataManger.GetZoneGroup(i);
                    int nZoneCount = group.GetZoneCount();

                    for (int j = 0; j < nZoneCount; j++)
                    //ArrayList arZone = dataManger.DataZones;
                    //foreach (DataZone zone in arZone)
                    {
                        DataZone zone = group.GetZone(j);

                        if (zone.ZoneName == "PLAN")
                            continue;

                        if (zone != null)
                        {
                            if (ZoneVolumes.Count > 0)
                            {
                                UnE.Geometry.Vertex2D pos2 = new UnE.Geometry.Vertex2D(pos.X, -pos.Z);
                                if (zone.Boundary.HitTest(pos2) != 0)
                                {
                                    ZoneVolumes[zone.ZoneName].SetVisible(true);
                                }
                                else
                                {
                                    ZoneVolumes[zone.ZoneName].SetVisible(false);
                                }
                            }

                        }
                    }
                }
            });
        }
		private void ShowZoneVolume(MouseEventArgs e)
        {
            Thread t = new Thread(ShowZoneVolumeThread);
            t.Start(e);

            //base.OnSavePt(e);          

            //Position3D pos = Get3DPoint(new Point(e.X, e.Y));
            
            //DataManager dbManger = FormMain.Instance.DataMgr;
            //ArrayList arZone = dbManger.DataZones;
            //foreach (DataZone zone in arZone)
            //{
            //    if( zone.ZoneName == "PLAN")
            //        continue;
               
            //    if (zone != null)
            //    {
            //        UnE.Geometry.Vertex2D pos2 = new UnE.Geometry.Vertex2D(pos.X, -pos.Z);
            //        if (zone.Boundary.HitTest(pos2) != 0)
            //        {
            //            ZoneVolumes[zone.ZoneName].SetVisible(true);
            //        }
            //        else
            //        {
            //            ZoneVolumes[zone.ZoneName].SetVisible(false);
            //        }
            //    }
            //}
            
            //string szLine = string.Format("Location {0} , {1} , {2}", pos.X, pos.Y, pos.Z);
            //Debug.WriteLine(szLine);
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
	
			}
			
		}

		private void OnShowTooltip(object sender , EventArgs e)
		{
			m_TooltipTimer.Stop();
			m_TooltipTimer.Enabled = false;
			
			int nPoiID = OnSelectPOI(m_nShowTooltipX, m_nShowTooltipY);
			if (nPoiID != -1)
			{			
              
			}
			
		}

        private void OnPrevPanning(MouseEvent e)
        {
            if (e != MouseEvent.MOUSE_DOWN)
                return;

            //OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
        }

        private void OnPrevOrbit(MouseEvent e)
        {
            if (e != MouseEvent.MOUSE_DOWN)
                return;

            //OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
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
                //ProcessCCTVLOD();
            }
            else
                OnScreenMove();
        }

      

        private void TurnOnTemporaryList()
        {
          
        }

      

        private void TurnOnWheelTemporaryList()
        {
            //foreach (POI poi in m_arrTemporaryHiddenPOIsForWheel)
            //{
            //    Point pt = (Point)Get2DPoint(new Position3D(poi.X, poi.Y, poi.Z));
            //    poi.Popup.Show(pt.X, pt.Y);
            //}

            //m_arrTemporaryHiddenPOIsForWheel.Clear();
        }

        public void HideAllPOIPopup()
        {
            //OnPostPick(null, null, true);
        }

        //private void OnPostPick(POI poi, ArrayList arrHidden = null, bool absolutely = false)
        //{
        //    bool refresh = false;

        //    if (arrHidden != null)
        //        arrHidden.Clear();
            
            //if (m_isIndoor)
            //{
            //    if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
            //    {
            //        ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

            //        foreach (POI _poi in arrPOIs)
            //        {
            //            if (_poi == poi || _poi.Popup == null || !_poi.Popup.IsVisible())
            //                continue;

            //            if (arrHidden != null)// && IsLODShowingPOI(_poi))
            //                arrHidden.Add(_poi);

            //            _poi.Popup.Hide(absolutely);
            //            refresh = true;
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
            //    {
            //        if (pair.Value == poi || pair.Value.Popup == null || !pair.Value.Popup.IsVisible())
            //            continue;

            //        if (arrHidden != null)// && IsLODShowingPOI(pair.Value))
            //            arrHidden.Add(pair.Value);

            //        pair.Value.Popup.Hide(absolutely);
            //        refresh = true;
            //    }
            //}

            //if (poi != null)
            //    FormMain.Instance.PageHome.OnPostPickPOI(poi);

        //    if (refresh)
        //    {                
        //        Update();
        //    }
        //}

        //private bool IsLODShowingPOI(POI poi)
        //{
        //    return m_arrLODShowingPOIs.Contains(poi);
        //}
        
        private void OnScreenMove()
        {
            bool refresh = false;

            //if (m_isIndoor)
            //{
            //    if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
            //    {
            //        ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

            //        foreach (POI poi in arrPOIs)
            //        {
            //            if (OnMovePOI(poi))
            //                refresh = true;
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
            //    {
            //        if (OnMovePOI(pair.Value))
            //            refresh = true;
            //    }
            //}

            if (refresh)
            {
                Update();
            }
        }


        private System.Windows.Forms.Timer m_WheelTimer = null;
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

                //OnPostPick(null, m_arrTemporaryHiddenPOIsForWheel, true);     
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
                //ProcessCCTVLOD();
            }
            m_WheelTimer.Enabled = false;
            m_WheelTimer.Stop();
        }


        public void ShowLayer(int nLayer, bool bShow)
        {            
            //foreach (KeyValuePair<int, POI> kv in m_dicPOIs)
            //{
            //    POI poi = kv.Value;
            //    if (poi.Popup != null)
            //    {
            //        if (poi.Facility.GetLayerID() == nLayer)
            //        {
            //            poi.Popup.LayerVisible = bShow;
            //        }
            //    }                                   
            //}  
            
        }

        public void UpdatePOI()
        {
            OnScreenMove();
        }

        //public bool IsTemporaryHiddenPOI(POI poi)
        //{
        //    bool bResult = m_arrTemporaryHiddenPOIs.Contains(poi);
        //    bool bResult2 = m_arrTemporaryHiddenPOIsForWheel.Contains(poi);
        //    return (bResult || bResult2);
        //}

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
      
        public new void ClearAllData()
        {
            base.ClearAllData();
        }

        public void ClearPOISelection()
        {
            base.ClearAllSelectedPOI();
            m_arSelectedPoi.Clear();
        }

        public new void OnViewHome()
        {
            //OnPostPick(null, m_arrTemporaryHiddenPOIs, true);

            base.OnViewHome();

            TurnOnTemporaryList();
        }

        public new void OnViewTop()
        {
            //OnPostPick(null, m_arrTemporaryHiddenPOIs, true);

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
			this.ResumeLayout(false);
			timer1.Enabled = false;

		}

		private void BaseViewEx_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			//FormMain.Instance.EnableFireReportBtn(false);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			
			//this.UpdateWindow();
		}

        public int CreatePOI(string strIconPath, float x, float y, float z)
        {
            //Position3D pos = Get3DPoint(new Point(x, y));
            int nID = AddPOI(strIconPath, x, y, z);

            return nID;
        }
    }
}
