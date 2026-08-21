using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
//using UnE.Util.Unity;

namespace libSensorProcess
{
    /// <summary>
    /// 유해화학물질 센서
    /// </summary>
	public class EtcProcess : IDisposable, ProcessIF
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

        private int m_nAlarmLevel = 0;
        public int AlarmLevel
        {
            get { return m_nAlarmLevel; }
            set 
            { 
                m_nAlarmLevel = value;
                OnChangeAlarmLevel(m_nAlarmLevel);
            }
        }

        private DateTime m_DetectTime;
        public DateTime DetectTime
        {
            get { return m_DetectTime; }
            set { m_DetectTime = value; }
        }

		private Thread m_EtcAlarmThread = null;

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

        private ProcessType mType = ProcessType.TerrorAlarm;
        public ProcessType ProcessType
        {
            get { return mType; }
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

        public EtcProcess(ProcessType processType)
		{
            mType = processType;
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
                    return "[ETC]" + TargetZone.ZoneName;

				return "[ETC]" + szZoneName + "/" + TargetZone.ZoneName;
			}
			return base.ToString();
		}

        private bool m_bBeginProcess = false;
		public void BeginProcess()
		{
            m_bBeginProcess = true;

            ProcessManager.Instance.ProcessOwner.AddSensorDectectInvoke(this, true, false);
            
			m_EtcAlarmThread = new Thread(ConfirmSpill);
            m_EtcAlarmThread.Name = "ETCAlarm_ConfirmETC";
			m_EtcAlarmThread.Start();
		}

        public void ReadyProcess()
        {
            try
            {
                ProcessManager.Instance.ProcessOwner.ShowSensorAlarmInvoke(this, ReactionType.NOTIFY_SIGNAL);                
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
				if (m_EtcAlarmThread != null && m_bProcess == true)
				{
					m_bProcess = false;

					if (m_EtcAlarmThread.IsAlive)
					{
						m_EtcAlarmThread.Interrupt();
						m_EtcAlarmThread.Abort();
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
            m_bSelectProcess = false;
			return true;
		}

        protected void OnChangeAlarmLevel(int nLevel)
        {
            // 시작된 경우에만 AlarmLevel을 변경한다.
            if (m_bBeginProcess == true)
            {
                ProcessManager.Instance.ProcessOwner.ShowEvacCircleInvoke(nLevel);                
            }            
        }
        
		private static bool bConfirmFire = false;
		
		static public void PlaySound()
		{
			string szWavPath = ProcessManager.EnginPath() + "\\Media\\Sound\\FireSignalAlarm.WAV";
			if (System.IO.File.Exists(szWavPath))
			{
				m_player.SoundLocation = szWavPath;
				m_player.Play();
			}
		}

		public void ConfirmSpill()
		{
			if (m_TargetSensor == null || m_TargetZone == null)
			{
				return;
			}
			m_bProcess = true;
			bConfirmFire = true;

            m_arCCTVs = ProcessManager.Instance.ProcessOwner.ConfirmDisasterInvoke(this, ShowFireDetectTooltipCCTV, m_nSensorID, ReactionType.NOTIFY_SIGNAL, m_nAlarmLevel);            
           
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
            if (log == null)
                return;

            try
            {
                int nLevel = Convert.ToInt32(log.Parameter5);
                this.AlarmLevel = nLevel;
            }
            catch (Exception)
            {
            }
        }

        // 새로운 신호가 탐지되었음을 ProcessOwner에게 알린다.
        public void SetDetectMode(ReactionLog log, IProcessOwner owner)
        {
            if (owner != null)
                owner.SetPSMDetectModeInvoke(log);
        }

        public void SetAlarmLevel(ReactionLog log)
        {
            try
            {
                int nLevel = Convert.ToInt32(log.Parameter5);
                this.AlarmLevel = nLevel;
            }
            catch (Exception)
            { }
        }
	}
}