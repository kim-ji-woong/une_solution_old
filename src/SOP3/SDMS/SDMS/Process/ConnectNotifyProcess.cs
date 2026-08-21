using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SDMS
{
    public class ConnectNotifyProcess : IDisposable, ProcessIF
    {
        private int m_nSensorID = -1;
        public int DetectSensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }

        private Thread m_ConnectAlarmThread = null;

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
			m_ConnectAlarmThread.Start();
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

            FormMain.Instance.Invoke((MethodInvoker)delegate  
			{
				Core.BaseView view = m_TargetSensor.POI.ParentView;
				view.UpdateIcon(m_TargetSensor.POI.ID, m_TargetSensor.POI.Facility.IconPath);
				view.UpdateWindow();
			});
           
			m_bProcess = false;
		}
    }
}
