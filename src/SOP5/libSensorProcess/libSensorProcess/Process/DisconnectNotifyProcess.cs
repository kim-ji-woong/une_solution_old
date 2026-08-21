using System;
using System.Threading;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;


namespace libSensorProcess
{
	public class DisconnectNotifyProcess : IDisposable, ProcessIF
	{
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
            set { m_nAlarmLevel = value; }
        }

        private DateTime m_DetectTime;
        public DateTime DetectTime
        {
            get { return m_DetectTime; }
            set { m_DetectTime = value; }
        }

		private Thread m_DisconnectAlarmThread = null;

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

		/*private ArrayList m_arAbnormalSensor = null;
		public System.Collections.ArrayList AbnormalSensorList
		{
			set { m_arAbnormalSensor = value; }
		}*/

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

        private ProcessType mType = ProcessType.DisconnectSensor;
        public ProcessType ProcessType
        {
            get { return mType; }
        }


        public bool Select()
        {
            return false;
        }
        public void HideCCTV()
        {

        }
		private bool m_bProcess = false;

		public DisconnectNotifyProcess()
		{
		}

		public void Dispose()
		{
		}

		public void BeginProcess()
		{
			m_DisconnectAlarmThread = new Thread(ConfirmDisconnectSensor);
            m_DisconnectAlarmThread.Name = "Sensor_Disconnect";
			m_DisconnectAlarmThread.Start();
		}

        public void ReadyProcess()
        {

        }

		public void AbortProcess()
		{
			try
			{
				if (m_DisconnectAlarmThread != null && m_bProcess == true)
				{
					m_bProcess = false;
					m_DisconnectAlarmThread.Interrupt();
					m_DisconnectAlarmThread.Abort();
				}
			}
			catch (System.Exception)
			{
			}
		}

		public void ConfirmDisconnectSensor()
		{
			if (m_TargetSensor == null || m_TargetZone == null)
			{
				return;
			}

			m_bProcess = true;

			//FormMain.Instance.Invoke((MethodInvoker)delegate
			//{
			//    if (m_TargetZone.Building == null)
			//    {
			//        FormMain.Instance.PageHome.ContentForm.LayoutOutside();
			//    }
			//    else
			//    {
			//        BuildingGroup grp = m_TargetZone.Building.BuildingGroup;
			//        Building building = m_TargetZone.Building;
			//        FormMain.Instance.SetFloorStatus(grp, building, m_TargetZone);
			//        FormMain.Instance.PageHome.ContentForm.LayoutBothside();
			//    }
			//});

			/*FormMain.Instance.Invoke((MethodInvoker)delegate
			{
                if( m_TargetSensor.POI.ViewType == 1)
                {
                    //UnE.Util.Unity.Panel4Unity view = (UnE.Util.Unity.Panel4Unity)m_TargetSensor.POI.ParentView;
                    //view.UpdateIcon(m_TargetSensor.POI.ID, m_TargetSensor.POI.Facility.DisconnectIconPath);
                    //view.UpdateWindow();
                }
				
			});*/
			m_bProcess = false;
		}

        public bool ShowOpenSOP
        {
            get { return false; }
            set {}
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