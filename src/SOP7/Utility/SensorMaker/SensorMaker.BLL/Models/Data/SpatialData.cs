using SDMS.Model.Spatial;
using System.Collections.Generic;
using SDMS.IDAL;
using SensorMaker.BLL.Models.Data.Sensor;

namespace SensorMaker.BLL.Models.Data
{
    public class BuildingGroupData : BuildingGroup
    {
        private List<BuildingData> m_buildingDatas = new List<BuildingData>();
        private BuildingGroupData m_parent = null;
        private bool m_bVisible = true;
        
        public BuildingGroupData Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public List<BuildingData> BuildingDatas
        {
            get { return m_buildingDatas; }
            set { m_buildingDatas = value; }
        }

        public bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }
    }

    public class BuildingData : Building
    {
        private List<ZoneData> m_zoneDatas = new List<ZoneData>();

        public List<ZoneData> ZoneDatas
        {
            get { return m_zoneDatas; }
            set { m_zoneDatas = value; }
        }
    }

    public class ZoneData : Zone
    {
        private List<EquipmentZoneData> m_equipmentZoneDatas = new List<EquipmentZoneData>();
        private ZoneSensors m_sensors = null;
        private SDMS.Model.Spatial.ZoneData m_zoneData = new SDMS.Model.Spatial.ZoneData();

        public List<EquipmentZoneData> EquipmentZoneDatas
        {
            get { return m_equipmentZoneDatas; }
            set { m_equipmentZoneDatas = value; }
        }

        public ZoneSensors Sensors
        {
            get { return m_sensors; }
            set { m_sensors = value; }
        }

        public SDMS.Model.Spatial.ZoneData Datas
        {
            get { return m_zoneData; }
            set { m_zoneData = value; }
        }
    }

    public class EquipmentZoneData : EquipmentZone
    {
        // 하나의 EquipmentZone이 여러개의 Zone에 걸쳐 있을수 있다.
        private List<Zone> m_linkedZoneDatas = new List<Zone>();

        public List<Zone> LinkedZoneDatas
        {
            get { return m_linkedZoneDatas; }
            set { m_linkedZoneDatas = value; }
        }
    }

    public class ZoneSensors
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

        public List<PSMSensor> PsmSensors
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

        public ZoneSensors()
        {
        }

        public ZoneSensors(List<FireSensor> fireSensors, List<PSMSensor> psmSensors, List<EtcSensor> etcSensors, List<CCTVSensor> cctvs)
        {
            m_fireSensors = fireSensors;
            m_psmSensors = psmSensors;
            m_etcSensors = etcSensors;
            m_cctvs = cctvs;
        }
    }
}
