using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SDMS
{

    public class DisconnectNotifyProcess : IDisposable, ProcessIF
    {
        private int m_nSensorID = -1;
        public int DetectSensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }

        private Thread m_DisconnectAlarmThread = null;

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

        /*private ArrayList m_arAbnormalSensor = null;
        public System.Collections.ArrayList AbnormalSensorList
        {
            set { m_arAbnormalSensor = value; }
        }*/

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
            m_DisconnectAlarmThread.Start();
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


            FormMain.Instance.Invoke((MethodInvoker)delegate  
			{
				Core.BaseView view = m_TargetSensor.POI.ParentView;
				view.UpdateIcon(m_TargetSensor.POI.ID, m_TargetSensor.POI.Facility.DisconnectIconPath);
				view.UpdateWindow();
           });
            m_bProcess = false;
        }

    }
}
