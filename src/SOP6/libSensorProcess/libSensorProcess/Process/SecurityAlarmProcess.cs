using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using System.Runtime.InteropServices;

namespace libSensorProcess
{
	public class SecurityAlarmProcess : IDisposable, ProcessIF
	{
		private static SoundPlayerEx m_player = new SoundPlayerEx();
		public static SoundPlayerEx SoundPlayer
		{
			get { return m_player; }
		}

        private static BeepPlayer m_beep = new BeepPlayer();
        public static BeepPlayer Beep
        {
            get { return m_beep; }
        }
    

        private int m_nAlarmLevel = 0;
        public int AlarmLevel
        {
            get { return m_nAlarmLevel; }
            set { m_nAlarmLevel = value; }
        }

		private int m_nSensorID = -1;

		public int DetectSensorID
		{
			get { return m_nSensorID; }
			set { m_nSensorID = value; }
		}

        private DateTime m_DetectTime;
        public DateTime DetectTime
        {
            get { return m_DetectTime; }
            set { m_DetectTime = value; }
        }

		private Thread m_SecurityAlarmThread = null;

		private ISensor m_TargetSensor = null;

		public ISensor TargetSensor
		{
			get { return m_TargetSensor; }
			set { m_TargetSensor = value; }
		}

		private EquipmentZone m_TargetZone = null;

		public EquipmentZone TargetZone
		{
			get { return m_TargetZone; }
			set { m_TargetZone = value; }
		}

		private int m_nSensorHistoryID = -1;

		public int SensorHistoryID
		{
			get { return m_nSensorHistoryID; }
			set { m_nSensorHistoryID = value; }
		}

		private ReactionLog m_LastLog = null;

		public ReactionLog LastLog
		{
			get { return m_LastLog; }
			set { m_LastLog = value; }
		}

		private bool m_bProcess = false;

		private static bool m_isShowFireDetectTooltipCCTV = false;

		public static bool ShowFireDetectTooltipCCTV
		{
			get { return m_isShowFireDetectTooltipCCTV; }
			set { m_isShowFireDetectTooltipCCTV = value; }
		}

        private bool m_bShowOpenSOP = false;
        public bool ShowOpenSOP
        {
            get { return m_bShowOpenSOP; }
            set { m_bShowOpenSOP = value; }
        }

        private ProcessType mType = ProcessType.SecurityAlarm;
        public ProcessType ProcessType
        {
            get { return mType; }
        }

        public SecurityAlarmProcess()
		{
		}

		public void Dispose()
		{
		}

		public override string ToString()
		{
			if (TargetZone != null && TargetZone.LinkedZone != null)
			{
                string szZoneName = TargetZone.LinkedZone.DisplayText;

                if (szZoneName == TargetZone.ZoneName)
                    return "[방범]" + TargetZone.ZoneName;

				return "[방범]" + szZoneName + "/" + TargetZone.ZoneName;
			}
			return base.ToString();
		}

		public void BeginProcess()
		{
            ProcessManager.Instance.ProcessOwner.AddSensorDectectInvoke(this, true, false);
            /*FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.AddSensorDectect(this, true, false);
            });*/

			m_SecurityAlarmThread = new Thread(ConfirmFire);
            m_SecurityAlarmThread.Name = "SecurityAlarm_ConfirmStauts"; 
			m_SecurityAlarmThread.Start();
		}

        public void ReadyProcess()
        {
			try
			{
                ProcessManager.Instance.ProcessOwner.ShowSensorAlarmInvoke(this, ReactionType.NOTIFY_SIGNAL);
                //ProcessManager.Instance.ProcessOwner.ShowSensorAlarmInvoke(this, ReactionType.NOTIFY_SECURITY);
                /*FormMain.Instance.Invoke((MethodInvoker)delegate
				{
                    ISensorTooltipOwner view = null;
					PageBackstageHome.Instance.FireDetect(m_TargetSensor, m_TargetZone, m_nSensorHistoryID);

					try
					{
						if (FormFrame.Instance.WindowState != FormWindowState.Maximized)
						{
                            FormFrame.Instance.WindowState = FormWindowState.Maximized;
                            FormMain.Instance.Activate();
                            FormMain.Instance.Focus();
						}
					}
					catch (System.Exception)
					{
					}
                                        
					SeletCaseData form = new SeletCaseData(this.ProcessType, view, m_TargetSensor, m_nSensorHistoryID, this.ShowOpenSOP, DetectTime);
					ConfirmDialogManager.Instance.AddDialogFirst(form);

                    if (m_LastLog != null &&
                            m_LastLog.ReactionType != (int)ReactionType.NOTIFY_FIRE &&
                            m_LastLog.ReactionType != (int)ReactionType.NOTIFY_SECURITY
                            )
                        ConfirmDialogManager.Instance.ShowDialogNext();

                    FormMain.Instance.Update3DView();
                });*/
            }
			catch (ThreadInterruptedException e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
			}
        }

		public void AbortProcess()
		{
			try
			{
				if (m_SecurityAlarmThread != null && m_bProcess == true)
				{
					m_bProcess = false;

					if (m_SecurityAlarmThread.IsAlive)
					{
						m_SecurityAlarmThread.Interrupt();
						m_SecurityAlarmThread.Abort();
					}
				}
			}
			catch (System.Exception)
			{
			}
		}

		private ArrayList m_arCCTVs = null;
        private bool m_bSelectProcess = false;
		public bool Select()
		{
			// 화재상황이 진행중이면 자동 전환하지 않는다.
			if (bConfirmFire == true)
				return false;

            if (m_bSelectProcess == true)
                return false;

            m_bSelectProcess = true;
            ProcessManager.Instance.ProcessOwner.SelectProcessInvoke(this, ShowFireDetectTooltipCCTV, m_arCCTVs, m_nSensorID);
            /*FormMain.Instance.Invoke((MethodInvoker)delegate
			{
                FormMain.Instance.PageHome.ContentForm.PushViewState(true);

				FormMain.Instance.PageHome.ContentForm.HideZoneVolume();
                FormMain.Instance.PageHome.ContentForm.HideEvacCircle();

                if (m_TargetZone == null || m_TargetZone.Building == null)
				{
					//FormMain.Instance.PageHome.ContentForm.LayoutOutside();
				}
				else
				{
					BuildingGroup grp = m_TargetZone.Building.BuildingGroup;
					Building building = m_TargetZone.Building;

					//FormMain.Instance.PageHome.ContentForm.LayoutBothside();
					//PageBackstageHome.Instance.SetCheckBothSide();
					FormMain.Instance.SetFloorStatus(grp, building, (m_TargetZone.LinkedZone));

                    if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                    {
                        FormMain.Instance.ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
                    }
                    

					FormMain.Instance.EnableChangeViewBtn();
				}

                if (m_TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell)
                {
                    FormMain.Instance.PageHome.ContentForm.ShowEmPoll(m_nSensorID);
                    FormMain.Instance.PageHome.ContentForm.ZoomBuilding("EMPOLL_" + m_nSensorID);
                }
                else
                {
                    if (m_TargetZone.Building != null && m_TargetZone.Building.BuildingID != "yhNONE")
                    {
                        string szName = m_TargetZone.Building.BuildingID;

                        FormMain.Instance.PageHome.ContentForm.ZoomBuilding(szName);

                        FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.LinkedZone.ID, true, true);
                        FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, false, true);

                    }
                    else
                    {
                        if (m_TargetZone.Polygon != null)
                        {
                            UnE.Geometry.Vertex2D pos = m_TargetZone.Polygon.CalcWeightCenter();
                            float dx = ZoneManager.Instance.Dx;
                            float dy = ZoneManager.Instance.Dy;

                            float x = (float)pos.x - dx;
                            float y = 1.0f;
                            float z = dy - (float)pos.y;
                            x /= 1000;
                            z /= 1000;

                            FormMain.Instance.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                            FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, true, true);
                        }
                    }
                }
				

                if (m_arCCTVs != null)
                {
                    foreach (CCTV cctv in m_arCCTVs)
                    {
                        if (cctv.POI.Zone == m_TargetZone.LinkedZone && m_TargetZone.IsOutdoor == false)
                        {
                            if (cctv.POI.ViewType == 1)
                            {
                                UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                if (view != null)
                                {
                                    System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                                    if (cctv.POI.Popup != null)
                                    {
                                        if (ShowFireDetectTooltipCCTV)
                                            cctv.POI.Popup.Show(p.X, p.Y);
                                    }
                                }

                            }
                            else
                            {
                                UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                if (view != null)
                                {
                                    System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                                    if (cctv.POI.Popup != null)
                                    {
                                        if (ShowFireDetectTooltipCCTV)
                                            cctv.POI.Popup.Show(p.X, p.Y);
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (cctv.POI.IsIndoor == false)
                            {
                                UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                if (view != null)
                                {
                                    System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                                    if (cctv.POI.Popup != null)
                                    {
                                        if (ShowFireDetectTooltipCCTV)
                                            cctv.POI.Popup.Show(p.X, p.Y);
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    if (m_TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell)
                    {
                        FormMain.Instance.PageHome.ContentForm.ShowEmPoll(m_nSensorID);
                        FormMain.Instance.PageHome.ContentForm.ZoomBuilding("EMPOLL_" + m_nSensorID);
                    }

                    if (m_TargetZone != null && m_TargetZone.LinkedZone != null)
                    {

                        //FormMain.Instance.PageHome.ShowSituationCCTV(true);
                        FormMain.Instance.PageHome.ShowBigCCTV(m_TargetZone.LinkedZone, 1, true);
                        FormMain.Instance.SelectCCTVTab(false);
                    }
                }

                if(UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                {
                    if (m_TargetZone != null && m_TargetZone.LinkedZone != null)
                    {
                        FormMain.Instance.CCTVPipe.Send("SetHistoryID(" + m_nSensorHistoryID + ")");
                        //FormMain.Instance.PageHome.ShowSituationCCTV(true);
                        FormMain.Instance.PageHome.ShowBigCCTV(m_TargetZone.LinkedZone, 1, true);
                        FormMain.Instance.SelectCCTVTab(false);
                    }
                }

                FormMain.Instance.Update3DView();
			});*/


            m_bSelectProcess = false;
			return true;
		}

        //private Core.ZoneVolume m_OutVolume = null;
        //private Core.ZoneVolume m_InVolume = null;
		private static bool bConfirmFire = false;
		////////////////////////////////////////////////////////////////////////

		static public void PlaySound()
		{
			string szWavPath = ProcessManager.EnginPath() + "\\Media\\Sound\\FireSignalAlarm.WAV";
			if (System.IO.File.Exists(szWavPath))
			{
				m_player.SoundLocation = szWavPath;
				m_player.Play();
			}
		}

		public void ConfirmFire()
		{
			if (m_TargetSensor == null || m_TargetZone == null)
			{
				return;
			}
			m_bProcess = true;
			bConfirmFire = true;

            m_arCCTVs = ProcessManager.Instance.ProcessOwner.ConfirmDisasterInvoke(this, ShowFireDetectTooltipCCTV, m_nSensorID, ReactionType.NOTIFY_SIGNAL, 1);
            //m_arCCTVs = ProcessManager.Instance.ProcessOwner.ConfirmDisasterInvoke(this, ShowFireDetectTooltipCCTV, m_nSensorID, ReactionType.NOTIFY_SECURITY, 1);
            try
			{
				/*FormMain.Instance.Invoke((MethodInvoker)delegate
				{

                    FormMain.Instance.PageHome.ContentForm.PushViewState(true);


					PlaySound();

					FormMain.Instance.PageHome.ContentForm.HideZoneVolume();


                    //if (!FormMain.Instance.ShowEquipZoneCCTV)
                    {
                        FormMain.Instance.SelectMonitoringTab();
                    }

					FormMain.Instance.DetectFireSensor = true;

					if (m_TargetZone.Building == null)
					{
						//FormMain.Instance.PageHome.ContentForm.LayoutOutside();
					}
					else
					{
						BuildingGroup grp = m_TargetZone.Building.BuildingGroup;
						Building building = m_TargetZone.Building;

						//PageBackstageHome.Instance.ContentForm.LayoutBothside();
						//PageBackstageHome.Instance.SetCheckBothSide();
						FormMain.Instance.SetFloorStatus(grp, building, m_TargetZone.LinkedZone);
                        
                        if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                        {
                            FormMain.Instance.ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
                        }
						
                        FormMain.Instance.EnableChangeViewBtn();
					}

					FormMain.Instance.PageHome.HideAllPOIPopup();

					m_arCCTVs = CCTVManager.Instance.AutoPopupCCTV(m_TargetZone.LinkedZone);
					foreach (CCTV cctv in m_arCCTVs)
					{
						if (cctv.POI.Zone == m_TargetZone.LinkedZone && m_TargetZone.IsOutdoor == false)
						{
                            if (cctv.POI != null && cctv.POI.Popup != null)
                            {
                                UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);

                                if (ShowFireDetectTooltipCCTV)
                                    cctv.POI.Popup.Show(p.X, p.Y);
                            }
						}
						else
						{
							if (cctv.POI.IsIndoor == false)
							{
                                UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                if( view != null && cctv.POI != null)
                                {                                    
                                    System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                                    if (cctv.POI.Popup != null)
                                    {
                                        if (ShowFireDetectTooltipCCTV)
                                            cctv.POI.Popup.Show(p.X, p.Y);
                                    }
                                }								
							}
						}
					}

                    if (m_TargetZone.Building != null && m_TargetZone.Building.BuildingID != "yhNONE")
					{
                        if (m_TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell)
                        {
                            FormMain.Instance.PageHome.ContentForm.ShowEmPoll(m_nSensorID);
                            FormMain.Instance.PageHome.ContentForm.ZoomBuilding("EMPOLL_" + m_nSensorID);
                        }
                        else
                        {
                            string szName = m_TargetZone.Building.BuildingID;

                            FormMain.Instance.PageHome.ContentForm.ZoomBuilding(szName);

                            FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.LinkedZone.ID, true, true);
                            FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, false, true);
                        }
						

					}
					else
					{
                        if (m_TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell)
                        {
                            FormMain.Instance.PageHome.ContentForm.ShowEmPoll(m_nSensorID);
                            FormMain.Instance.PageHome.ContentForm.ZoomBuilding("EMPOLL_" + m_nSensorID);
                        }
                        else if(m_TargetZone.Polygon != null)
                        {
                            UnE.Geometry.Vertex2D pos = m_TargetZone.Polygon.CalcWeightCenter();
                            float dx = ZoneManager.Instance.Dx;
                            float dy = ZoneManager.Instance.Dy;


                            if( UnE.SOP.ProxySOP.Instance.SiteID == 2)
                            {
                                float x = (float)pos.x - dx;
                                float y = 0.0f;
                                float z = dy - (float)pos.y;

                                x /= 1000;
                                z /= 1000;
                                FormMain.Instance.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                                FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, true, true);
                            }
                            else if(UnE.SOP.ProxySOP.Instance.SiteID == 100)
                            {                                
                                float x = (float)pos.x;
                                float y = 2.0f;
                                float z = (float)pos.y;
                                FormMain.Instance.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                                FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, true, true);                                
                            }
                            else
                            {
                                float x = (float)pos.x - dx;
                                float y = 0.0f;
                                float z = dy - (float)pos.y;
                                FormMain.Instance.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                                FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, true, true);
                            }
                        }						
					}
				});*/

				//DialogResult result = DialogResult.Cancel;

				try
				{
					/*FormMain.Instance.Invoke((MethodInvoker)delegate
					{
                        ISensorTooltipOwner view = null;

						if (m_TargetSensor.POI == null)
						{
							//view = PageBackstageHome.Instance.ContentForm.IndoorView;
						}
						else
						{
							if (m_TargetSensor.POI.IsIndoor == true)
							{
								//view = PageBackstageHome.Instance.ContentForm.IndoorView;
							}
							else
							{
								//view = PageBackstageHome.Instance.ContentForm.OutdoorView;
							}
						}

						PageBackstageHome.Instance.FireDetect(m_TargetSensor, m_TargetZone, m_nSensorHistoryID);

						try
						{
							if (FormFrame.Instance.WindowState != FormWindowState.Maximized)
							{
                                FormFrame.Instance.WindowState = FormWindowState.Maximized;
                                FormFrame.Instance.Activate();
                                FormMain.Instance.Focus();
							}
						}
						catch (System.Exception)
						{
						}

						SeletCaseData form = new SeletCaseData(this.ProcessType, view, m_TargetSensor, m_nSensorHistoryID, this.ShowOpenSOP, DetectTime);
						ConfirmDialogManager.Instance.AddDialogFirst(form);

                        PageBackstageHome.Instance.ShowBigCCTV(m_TargetZone, 1);						

                        if (m_LastLog != null && m_LastLog.ReactionType != (int)ReactionType.NOTIFY_SECURITY)
					        ConfirmDialogManager.Instance.ShowDialogNext();
                       
						
                        FormMain.Instance.Update3DView();
					});*/
				}
				catch (ThreadInterruptedException e)
				{
					System.Diagnostics.Trace.WriteLine(e.Message);
				}

				/*if (result == DialogResult.Cancel)
				{
					// 데이터 갱신을 한번 기다린후에 제거 한다.
					//Thread.Sleep(1500);
					//ProcessManager.Instance.EndProcess(this);
				}*/
			}
			catch (Exception)
			{
			}

  

			m_bProcess = false;
			bConfirmFire = false;
		}

		public void HideCCTV()
		{
			if (m_arCCTVs != null)
			{
				foreach (CCTV cctv in m_arCCTVs)
				{
					if (cctv.POI != null && cctv.POI.Popup != null)
						cctv.POI.Popup.Close();
				}
			}
		}

        // 외부 센서신호를 통하여 생성된 Process일 경우 ProcessIF 객체 생성 이후에 ReactionLog 객체를 이용하여 Process 초기화를 한다.
        public void InitFromSensor(ReactionLog log)
        {
        }

        // 새로운 신호가 탐지되었음을 ProcessOwner에게 알린다.
        public void SetDetectMode(ReactionLog log, IProcessOwner owner)
        {
            if (owner != null)
                owner.SetSecurityDetectModeInvoke(log);
        }

        public void SetAlarmLevel(ReactionLog log)
        {
        }
	}

}