using System;
using System.Threading;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;


namespace libSensorProcess
{
	public class ConnectNotifyProcess : IDisposable, ProcessIF
	{
        public bool ShowOpenSOP
        {
            get { return false; }
            set { }
        }

        private DateTime m_DetectTime;
        public DateTime DetectTime
        {
            get { return m_DetectTime; }
            set { m_DetectTime = value; }
        }

		private int m_nSensorID = -1;

		public int DetectSensorID
		{
			get { return m_nSensorID; }
			set { m_nSensorID = value; }
		}

		private Thread m_ConnectAlarmThread = null;

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

        private ProcessType mType = ProcessType.ConnectSensor;
        public ProcessType ProcessType
        {
            get { return mType; }
        }

        private ReactionLog mLastLog = null;
        public ReactionLog LastLog
        {
            get
            {
                return mLastLog;
            }
            set
            {
                mLastLog = value;
            }
        }

        public bool Select()
        {
            return false;
        }

        public void HideCCTV()
        {

        }

		private bool m_bProcess = false;

		public ConnectNotifyProcess()
		{
		}

		public void Dispose()
		{

		}

		public void BeginProcess()
		{
			m_ConnectAlarmThread = new Thread(ConfirmConnectSensor);
            m_ConnectAlarmThread.Name = "Sensor_Connect";
			m_ConnectAlarmThread.Start();
		}

        public void  ReadyProcess()
        {

        }

		public void AbortProcess()
		{
			try
			{
				if (m_ConnectAlarmThread != null && m_bProcess == true)
				{
					m_bProcess = false;
					m_ConnectAlarmThread.Interrupt();
					m_ConnectAlarmThread.Abort();
				}
			}
			catch (System.Exception)
			{
			}
		}

		public void ConfirmConnectSensor()
		{
			if (m_TargetSensor == null || m_TargetZone == null)
			{
				return;
			}

			m_bProcess = true;

			/*FormMain.Instance.Invoke((MethodInvoker)delegate
			{
                if (m_TargetSensor.POI != null)
                {
                    if (m_TargetSensor.POI.ViewType == 1)
                    {

                        //UnE.Util.Unity.Panel4Unity view = (UnE.Util.Unity.Panel4Unity)m_TargetSensor.POI.ParentView;
                        //if (view != null)
                        //{
                            //view.UpdateIcon(m_TargetSensor.POI.ID, m_TargetSensor.POI.Facility.IconPath);
                            //view.UpdateWindow();
                        //}
                       
                    }
                }
                
				
			});*/

			m_bProcess = false;
		}

        private int m_nAlarmLevel = 0;
        public int AlarmLevel
        {
            get { return m_nAlarmLevel; }
            set { m_nAlarmLevel = value; }
        }

        // 외부 센서신호를 통하여 생성된 Process일 경우 ProcessIF 객체 생성 이후에 ReactionLog 객체를 이용하여 Process 초기화를 한다.
        public void InitFromSensor(ReactionLog log)
        {
        }

        public void SetDetectMode(ReactionLog log, IProcessOwner owner)
        {
        }

        public void SetAlarmLevel(ReactionLog log)
        {
        }
	}
}