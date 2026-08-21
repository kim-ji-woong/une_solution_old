using System.Collections.Generic;

namespace SensorMaker.BLL.Models.Response
{
    using Models.Data.Sensor;
    using SDMS.Model.Spatial;

    public class ResponseSensorList : MessageResult
    {
        private List<FireSensor> m_fireSensors = null;
        private List<PSMSensor> m_psmSensors = null;
        private List<EtcSensor> m_etcSensors = null;
        private List<CCTVSensor> m_cctvs = null;

        public List<FireSensor> FireSensors
        {
            get { return m_fireSensors; }
            set { m_fireSensors = value; }
        }

        public List<PSMSensor> PSMSensors
        {
            get { return m_psmSensors; }
            set { m_psmSensors = value; }
        }

        public List<EtcSensor> EtcSensors
        {
            get { return m_etcSensors; }
            set { m_etcSensors = value; }
        }

        public List<CCTVSensor> Cctvs
        {
            get { return m_cctvs; }
            set { m_cctvs = value; }
        }
    }

    public class ResponseIndoorDatas : ResponseSensorList
    {
        private int m_nZoneID = -1;
        private List<EquipmentZone> m_equipZones = new List<EquipmentZone>();

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public List<EquipmentZone> EquipZones
        {
            get { return m_equipZones; }
            set { m_equipZones = value; }
        }
    }
}
