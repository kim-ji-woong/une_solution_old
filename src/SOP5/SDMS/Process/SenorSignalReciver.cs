using System;

namespace SDMS
{
	internal class SensorSignalReciver : IDisposable
	{
		private static SensorSignalReciver m_instance = null;

		public static SensorSignalReciver Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new SensorSignalReciver();
				return m_instance;
			}
		}

		/*private Thread m_PollingThread = null;

		private int m_PollingTime = 300;
		public int PollingTime
		{
			get { return m_PollingTime; }
			set { m_PollingTime = value; }
		}

		private bool m_bProcess = true;
		public bool Progress
		{
			get { return m_bProcess; }
			set
			{
				m_bProcess = value;
			}
		}*/

		public SensorSignalReciver()
		{
		}

		public void Dispose()
		{
			//StopPolling();
		}

		/*public bool StartPolling()
		{
			m_bProcess = true;

			m_PollingThread = new Thread(Polling);
			m_PollingThread.Start();

			return true;
		}*/

		/*public bool StopPolling()
		{
			if (m_bProcess == false)
				return true;

			m_bProcess = false;
			m_PollingThread.Join();
			return true;
		}*/

		/*private void Polling()
		{
			while (m_bProcess == true)
			{
				SensorManager.Instance.ReadSensorZone();
				ArrayList m_arAbnormal = (ArrayList)SensorManager.Instance.GetAbnormalSensorList().Clone();

				if (m_arAbnormal != null && m_arAbnormal.Count > 0)
				{
					foreach ( Sensor sensor in m_arAbnormal )
					{
						if (sensor.SensorData == 1)
						{
							if (sensor.Type == Facility.FacilityType.FIRE_SENSOR)
							{
								//BeginAlarmCommand cmd = new BeginAlarmCommand(sensor.ID);
								//SensorHistoryCommandManager.Instance.AddHistory(cmd);

								ProcessManager.Instance.BeginProcess(sensor, (ArrayList)m_arAbnormal.Clone(), ProcessType.FireAlarm);
							}

							if (sensor.Type == Facility.FacilityType.PRESSURE_SENSOR)
							{
							}
						}

						if (sensor.Connected == false && sensor.InitSensor == true)
						{
							if (sensor.Type == Facility.FacilityType.FIRE_SENSOR)
							{
								ProcessManager.Instance.BeginProcess(sensor, (ArrayList)m_arAbnormal.Clone(), ProcessType.DisconnectSensor);
							}
						}

						Thread.Sleep(20);
					}
				}
				Thread.Sleep(m_PollingTime);
			}
		}*/
	}
}