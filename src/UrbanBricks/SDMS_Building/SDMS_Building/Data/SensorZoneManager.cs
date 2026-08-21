using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS_Building.Data
{
    public class SensorZoneManager
    {
        private static SensorZoneManager m_Instance = null;
        public static SensorZoneManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SensorZoneManager();
                return m_Instance;
            }
        }

        private List<ISensor> m_sensors = null;

        /// <summary>
        /// 정전, 지진 등 SensorTagInfo 가 없는 센서들 조회
        /// </summary>
        public void LoadETCSensorZone()
        {
            List<int> types = new List<int>();
            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
                types.Add((int)IFacility.FacilityType.Earthquake);
            if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
                types.Add((int)IFacility.FacilityType.STRONG_WIND);

            if (types.Count == 0)
                return;
                        
            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, Type, Zone, EquipZoneID ");
            sb.Append("  From SensorZone ");
            sb.AppendFormat("Where Type In ({0})", string.Join(",", types));
            
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());

            if (arrResult == null)
                return;

            m_sensors = new List<ISensor>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                if (nID >= SOPWebServer.Header.ManualReportDefaultID)
                    continue;

                if (nType < 0)
                    continue;
                
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                string szEquipZoneName = (equipZone != null ? equipZone.DisplayText : "");

                EtcSensor sensor = new EtcSensor();
                sensor.ID = nID;                
                sensor.SetSensorType(IFacility.ToFacilityType(nType));
                sensor.ZoneID = nZoneID;
                sensor.EquipZoneID = nEquipZoneID;
                
                m_sensors.Add(sensor);                
            }
        }

        public List<ISensor> FindZoneInSensor(int equipzoneID, IFacility.FacilityType sensortype)
        {
            List<ISensor> sensors = null;

            foreach (EtcSensor item in m_sensors)
            {
                if ((sensortype == IFacility.FacilityType.Earthquake || sensortype == IFacility.FacilityType.STRONG_WIND) && item.Type == sensortype)
                {
                    if (sensors == null)
                        sensors = new List<ISensor>();

                    sensors.Add(item);
                }
                else if (item.EquipZoneID == equipzoneID && item.Type == sensortype)
                {
                    if (sensors == null)
                        sensors = new List<ISensor>();

                    sensors.Add(item);
                }
            }

            return sensors;
        }
    }
}
