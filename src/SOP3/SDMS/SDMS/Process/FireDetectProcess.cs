using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
namespace SDMS
{

	public class FireDetectProcess : IDisposable, ProcessIF
	{
		private static SoundPlayerEx m_player = new SoundPlayerEx();
		public static SoundPlayerEx SoundPlayer
		{
			get { return m_player; }
		}

		private int m_nSensorID = -1;
		public int DetectSensorID
		{
			get { return m_nSensorID; }
			set { m_nSensorID = value; }
		}

		private Thread m_FireAlarmThread = null;

		private SensorZone m_TargetSensor = null;
		public SDMS.SensorZone TargetSensor
		{
			get { return m_TargetSensor; }
			set { m_TargetSensor = value; }
		}

		private EquipmentZone m_TargetZone = null;
		public SDMS.EquipmentZone TargetZone
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
		public SDMS.ReactionLog LastLog
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
		
		public FireDetectProcess()
		{
		}

		public void Dispose()
		{
		}

		public override string ToString()
		{
			if (TargetZone != null && TargetZone.LinkedZone != null)
			{
				string szZoneName = TargetZone.LinkedZone.BroadcastName;
				return szZoneName + "/" + TargetZone.ZoneName;
			}
			return base.ToString();
		}

		public void BeginProcess()
		{
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.AddFireDectect(this);
            });

			m_FireAlarmThread = new Thread(ConfirmFire);
			m_FireAlarmThread.Start();			
		}

		public void AbortProcess()
		{
			try
			{               
				if (m_FireAlarmThread != null && m_bProcess == true)
				{
					m_bProcess = false;

					if (m_FireAlarmThread.IsAlive)
					{
						m_FireAlarmThread.Interrupt();
						m_FireAlarmThread.Abort();                       
					}                   
				}
			}
			catch (System.Exception)
			{            	
			}
		}

		private ArrayList m_arCCTVs = null;
		public bool Select()
		{
			// 화재상황이 진행중이면 자동 전환하지 않는다.
			if (bConfirmFire == true)
				return false;

			FormMain.Instance.Invoke((MethodInvoker)delegate
			{

                PageBackstageHome.TranslucentForm.CloseExternal();

				FormMain.Instance.PageHome.ContentForm.HideZoneVolume();

				if (m_TargetZone.Building == null)
				{
					FormMain.Instance.PageHome.ContentForm.LayoutOutside();
				}
				else
				{
					BuildingGroup grp = m_TargetZone.Building.BuildingGroup;
					Building building = m_TargetZone.Building;
                    FormMain.Instance.PageHome.ContentForm.LayoutBothside();
                    PageBackstageHome.Instance.SetCheckBothSide();
					FormMain.Instance.SetFloorStatus(grp, building, (m_TargetZone.LinkedZone));
					
					FormMain.Instance.EnableChangeViewBtn();
				}

				if (m_arCCTVs != null)
				{
					foreach (CCTV cctv in m_arCCTVs)
					{
						if (cctv.POI.Zone == m_TargetZone.LinkedZone && m_TargetZone.IsOutdoor == false)
						{
							BaseViewEx view = (BaseViewEx)cctv.POI.ParentView;
							Core.Position3D pos = new Core.Position3D(cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
							System.Drawing.Point p = (System.Drawing.Point)view.Get2DPoint(pos);
                            if (cctv.POI.Popup != null)
                            {
                                if (ShowFireDetectTooltipCCTV)
                                    cctv.POI.Popup.Show(p.X, p.Y);
                            }
						}
						else
						{
							if (cctv.POI.IsIndoor == false)
							{
								BaseViewEx view = (BaseViewEx)cctv.POI.ParentView;
								Core.Position3D pos = new Core.Position3D(cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
								System.Drawing.Point p = (System.Drawing.Point)view.Get2DPoint(pos);
                                if (cctv.POI.Popup != null)
                                {
                                    if (ShowFireDetectTooltipCCTV)
                                        cctv.POI.Popup.Show(p.X, p.Y);
                                }
							}
						}
					}
				}			
				
				if (m_TargetZone.Building != null)
				{
					string szName = m_TargetZone.Building.BuildingID;
					Core.SceneManager scManager = FormMain.Instance.PageHome.ContentForm.SceneManager;
					foreach (Core.Scene scene in scManager.Childs)
					{
						if (scene.AliasName == szName)
						{
							scene.Zoom(true);
							m_OutVolume = PageBackstageHome.Instance.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, true, true);
							m_InVolume = PageBackstageHome.Instance.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, false, true);
							break;
						}
					}
				}
				else
				{
					UnE.Geometry.Vertex2D pos = m_TargetZone.Polygon.CalcWeightCenter();
					float dx = ZoneManager.Instance.Dx;
					float dy = ZoneManager.Instance.Dy;

					float x = (float)pos.x - dx;
					float y = 0.0f;
					float z = dy - (float)pos.y;
					FormMain.Instance.PageHome.ContentForm.ZoomTarget(x, y, z, false);
					m_OutVolume = FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID,m_TargetZone.ID, true, true);
				}      
				
			});

			return true;
		}

		private Core.ZoneVolume m_OutVolume = null;
		private Core.ZoneVolume m_InVolume = null;
		private static bool bConfirmFire = false;
		////////////////////////////////////////////////////////////////////////   

		static public void PlaySound()
		{
			string szWavPath = FormMain.EnginPath() + "\\Media\\Sound\\FireSignalAlarm.WAV";
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

            try
            {
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    PlaySound();

                    FormMain.Instance.PageHome.ContentForm.HideZoneVolume();
                    FormMain.Instance.SelectMonitoringTab();

                    FormMain.Instance.DetectFireSensor = true;

                    if (m_TargetZone.Building == null)
                    {
                        FormMain.Instance.PageHome.ContentForm.LayoutOutside();
                    }
                    else
                    {
                        BuildingGroup grp = m_TargetZone.Building.BuildingGroup;
                        Building building = m_TargetZone.Building;
                        PageBackstageHome.Instance.ContentForm.LayoutBothside();
                        PageBackstageHome.Instance.SetCheckBothSide();
                        FormMain.Instance.SetFloorStatus(grp, building, m_TargetZone.LinkedZone);

                        FormMain.Instance.EnableChangeViewBtn();
                    }

                    FormMain.Instance.PageHome.HideAllPOIPopup();

                    m_arCCTVs = CCTVManager.Instance.AutoPopupCCTV(m_TargetZone.LinkedZone);
                    foreach (CCTV cctv in m_arCCTVs)
                    {
                        if (cctv.POI.Zone == m_TargetZone.LinkedZone && m_TargetZone.IsOutdoor == false)
                        {
                            BaseViewEx view = (BaseViewEx)cctv.POI.ParentView;
                            Core.Position3D pos = new Core.Position3D(cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                            System.Drawing.Point p = (System.Drawing.Point)view.Get2DPoint(pos);
                            if (cctv.POI != null && cctv.POI.Popup != null)
                            {
                                if (ShowFireDetectTooltipCCTV)
                                    cctv.POI.Popup.Show(p.X, p.Y);
                            }
                        }
                        else
                        {
                            if (cctv.POI.IsIndoor == false)
                            {
                                BaseViewEx view = (BaseViewEx)cctv.POI.ParentView;
                                Core.Position3D pos = new Core.Position3D(cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                                System.Drawing.Point p = (System.Drawing.Point)view.Get2DPoint(pos);
                                if (cctv.POI != null && cctv.POI.Popup != null)
                                {
                                    if (ShowFireDetectTooltipCCTV)
                                        cctv.POI.Popup.Show(p.X, p.Y);
                                }
                            }
                        }
                    }

                    Core.SceneManager scManager = FormMain.Instance.PageHome.ContentForm.SceneManager;
                    if (m_TargetZone.Building != null)
                    {
                        string szName = m_TargetZone.Building.BuildingID;
                        foreach (Core.Scene scene in scManager.Childs)
                        {
                            if (scene.AliasName == szName)
                            {
                                scene.Zoom(true);
                                m_OutVolume = FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.LinkedZone.ID, true, true);
                                m_InVolume = FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, false, true);
                                break;
                            }
                        }
                    }
                    else
                    {
                        UnE.Geometry.Vertex2D pos = m_TargetZone.Polygon.CalcWeightCenter();
                        float dx = ZoneManager.Instance.Dx;
                        float dy = ZoneManager.Instance.Dy;

                        float x = (float)pos.x - dx;
                        float y = 0.0f;
                        float z = dy - (float)pos.y;
                        FormMain.Instance.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                        m_OutVolume = FormMain.Instance.PageHome.ContentForm.ShowZoneVolume(m_TargetZone.LinkedZone.ID, m_TargetZone.ID, true, true);

                    }
                });

                DialogResult result = DialogResult.Cancel;

                try
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        BaseViewEx view = null;

                        if (m_TargetSensor.POI == null)
                        {
                            view = PageBackstageHome.Instance.ContentForm.IndoorView;
                        }
                        else
                        {
                            if (m_TargetSensor.POI.IsIndoor == true)
                            {
                                view = PageBackstageHome.Instance.ContentForm.IndoorView;
                            }
                            else
                            {
                                view = PageBackstageHome.Instance.ContentForm.OutdoorView;
                            }
                        }                        

                        PageBackstageHome.Instance.FireDetect(m_TargetSensor, m_TargetZone, m_nSensorHistoryID);

                        try
                        {
                            if (FormMain.Instance.WindowState != FormWindowState.Maximized)
                            {
                                FormMain.Instance.WindowState = FormWindowState.Maximized;
                                FormMain.Instance.Activate();
                                FormMain.Instance.Focus();
                            }
                        }
                        catch (System.Exception)
                        {
                        }


                        SeletCaseData form = new SeletCaseData(view, m_TargetSensor, m_nSensorHistoryID);
                        ConfirmDialogManager.Instance.AddDialogFirst(form);

                        if (!FormMain.Instance.ShowEquipZoneCCTV)
                        {
                            ConfirmDialogManager.Instance.ShowDialogNext();
                        }
                        else
                        {
                            Form4CCTV cctvForm = PageBackstageHome.Instance.CCTVForm;
                            if (cctvForm.ZoneTarget == null)
                            {
                                PageBackstageHome.TranslucentForm.CloseExternal();
                                ConfirmDialogManager.Instance.ShowDialogNext();
                            }
                            else
                            {
                                PageBackstageHome.Instance.ShowBigCCTV(m_TargetZone);
                            }
                        }
                    });
                }
                catch (ThreadInterruptedException e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                if (result == DialogResult.Cancel)
                {
                    // 데이터 갱신을 한번 기다린후에 제거 한다.
                    //Thread.Sleep(1500);
                    //ProcessManager.Instance.EndProcess(this);
                }			
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
	}

	public class SoundPlayerEx : System.Media.SoundPlayer
	{
		private bool m_isPlaying = false;

		public new void Play()
		{
			if (m_isPlaying)
				Stop();

			m_isPlaying = true;
			base.PlayLooping();
		}

		public new void Stop()
		{
			base.Stop();
			m_isPlaying = false;
		}

		protected override void Dispose(bool disposing)
		{
			Stop();
			base.Dispose(disposing);
		}
	}
}
