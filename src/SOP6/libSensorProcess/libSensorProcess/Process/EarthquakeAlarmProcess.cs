using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using System.Runtime.InteropServices;

namespace libSensorProcess
{
    public class EarthquakeAlarmProcess : IDisposable, ProcessIF
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

        private bool m_bShowOpenSOP = false;
        public bool ShowOpenSOP
        {
            get { return m_bShowOpenSOP; }
            set { m_bShowOpenSOP = value; }
        }

        private ProcessType mType = ProcessType.EarthquakeAlarm;
        public ProcessType ProcessType
        {
            get { return mType; }
        }

        public EarthquakeAlarmProcess()
        {
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return "[지진알람]";
        }

        public void BeginProcess()
        {
            ProcessManager.Instance.ProcessOwner.AddSensorDectectInvoke(this, true, false);
            
            m_SecurityAlarmThread = new Thread(ConfirmEarthquake);
            m_SecurityAlarmThread.Name = "EarthquakeAlarm_ConfirmStauts";
            m_SecurityAlarmThread.Start();
        }

        public void ReadyProcess()
        {
            try
            {
                ProcessManager.Instance.ProcessOwner.ShowSensorAlarmInvoke(this, ReactionType.NOTIFY_SIGNAL);
                //ProcessManager.Instance.ProcessOwner.ShowSensorAlarmInvoke(this, ReactionType.NOTIFY_EARTHQUAKE);
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
            ProcessManager.Instance.ProcessOwner.SelectProcessInvoke(this, false, m_arCCTVs, m_nSensorID);
            
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

        public void ConfirmEarthquake()
        {
            if (m_TargetSensor == null || m_TargetZone == null)
            {
                return;
            }
            m_bProcess = true;
            bConfirmFire = true;

            m_arCCTVs = ProcessManager.Instance.ProcessOwner.ConfirmDisasterInvoke(this, false, m_nSensorID, ReactionType.NOTIFY_SIGNAL, 1);
            //m_arCCTVs = ProcessManager.Instance.ProcessOwner.ConfirmDisasterInvoke(this, false, m_nSensorID, ReactionType.NOTIFY_EARTHQUAKE, 1);
            try
            {
                try
                {
                }
                catch (ThreadInterruptedException e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
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

        // 외부 센서신호를 통하여 생성된 Process일 경우 ProcessIF 객체 생성 이후에 ReactionLog 객체를 이용하여 Process 초기화를 한다.
        public void InitFromSensor(ReactionLog log)
        {
        }

        // 새로운 신호가 탐지되었음을 ProcessOwner에게 알린다.
        public void SetDetectMode(ReactionLog log, IProcessOwner owner)
        {
            if (owner != null)
                owner.SetEarthquakeDetectModeInvoke(log);
        }

        public void SetAlarmLevel(ReactionLog log)
        {
        }
    }
}
