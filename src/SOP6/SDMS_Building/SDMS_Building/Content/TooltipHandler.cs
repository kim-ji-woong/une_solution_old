using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libSensorProcess;
using UnE.Sensor;

namespace SDMS_Building.Content
{
    public class TooltipHandler : IPOIPopup
    {
        private ISensor m_sensor = null;
        private bool m_bLayerVisible = true;
        private string m_strCurrentPOIType = "";
        public string CurrentPOIType
        {
            get { return m_strCurrentPOIType; }
            set { m_strCurrentPOIType = value; }
        }
        private POI m_poi = new POI();

        public TooltipHandler(IFacility sensor)
        {
            if (sensor is ISensor)
            {
                m_sensor = (ISensor)sensor;
                
                if (m_sensor is FireSensor)
                    m_strCurrentPOIType = Data.CommonString.POI_Fire;
                else if (m_sensor is PSMSensorForPOI)
                    m_strCurrentPOIType = Data.CommonString.POI_Gas;
                else if (m_sensor is EtcSensor)
                {
                    if (m_sensor.Type.ToString().ToUpper() == Data.CommonString.POI_Door.ToUpper())
                        m_strCurrentPOIType = Data.CommonString.POI_Door;
                    else if (m_sensor.SensorName.Contains(Data.CommonString.POI_FireWall))
                        m_strCurrentPOIType = Data.CommonString.POI_FireWall;
                }

                bool isAlarm = IsAlarm();
                if (isAlarm)
                {
                    m_strCurrentPOIType += Data.CommonString.AlarmTag;
                }

                m_sensor.IconPath = m_strCurrentPOIType;
            }
        }

        private bool IsAlarm()
        {
            foreach (ProcessIF proc in FormMain.Instance.SensorDectects)
            {
                if ((proc.ProcessType == ProcessType.FireAlarm && m_sensor.Type == IFacility.FacilityType.FIRE_SENSOR) ||
                    (proc.ProcessType == ProcessType.PSMAlarm && m_sensor.Type == IFacility.FacilityType.PSM_SENSOR) ||
                    (proc.ProcessType == ProcessType.DoorAlarm && m_sensor.Type == IFacility.FacilityType.DOOR) ||
                    (proc.ProcessType == ProcessType.EarthquakeAlarm && m_sensor.Type == IFacility.FacilityType.Earthquake) ||
                    (proc.ProcessType == ProcessType.BlackoutAlarm && m_sensor.Type == IFacility.FacilityType.BLACKOUT) ||
                    (proc.ProcessType == ProcessType.FirewallAlarm && m_sensor.Type == IFacility.FacilityType.FIREWALL) ||
                    (proc.ProcessType == ProcessType.StrongWindAlarm && m_sensor.Type == IFacility.FacilityType.STRONG_WIND))
                {
                    if (proc.TargetSensor.OrgSensorID == m_sensor.ID)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Show(int xTarget, int yTarget)
        {
            if (m_sensor == null)
                return;

            if (m_strCurrentPOIType.Length == 0)
                return;
            else
            {
                //m_poi.ID = m_sensor.ID;
                //m_poi.Facility = m_sensor;

                //int nIndex = m_strCurrentPOIType.IndexOf("AlarmOn");

                //if (nIndex > 0)
                //    m_strCurrentPOIType = m_strCurrentPOIType.Substring(0, nIndex);
                //else
                //    m_strCurrentPOIType += "AlarmOn";

                //FormMain.Instance.DataManager.ChangePOIIcon(m_poi, m_strCurrentPOIType);

                FormMain.Instance.SelectedPOI(m_sensor.Type, m_sensor.ID);
            }
        }

        public void Hide(bool absolutely)
        {
        }

        public void Hide()
        {
        }

        public void MoveTarget(int xTarget, int yTarget)
        {
        }

        public bool IsVisible()
        {
            return false;
        }

        public void Close()
        {
        }

        public bool LayerVisible
        {
            get { return m_bLayerVisible; }
            set { m_bLayerVisible = value; }
        }

        public IntPtr Handle
        {
            get { return FormMain.Instance.Handle; }
        }

        public ISensor Sensor
        {
            get { return m_sensor; }
            set { m_sensor = value; }
        }
    }
}
