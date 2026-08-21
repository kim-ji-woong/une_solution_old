using SensorMaker.BLL.Models.Data;
using SensorMaker.BLL.Models.Data.Sensor;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL.Models.Response
{
    public class ResponseOpenXML : MessageResult
    {
        private string m_strSiteName = "";

        // SensorType
        private List<Basic.SensorType> m_sensorTypes = null;

        // Spatial
        private List<BuildingGroupData> m_buildingGroups = null;
        private List<ZoneData> m_outdoorZones = new List<ZoneData>();

        // Sensors
        private List<FireSensor> m_fireSensors = null;
        private List<PSMSensor> m_psmSensors = null;
        private List<EtcSensor> m_etcSensors = null;
        private List<CCTVSensor> m_cctvs = null;

        // GltfModel
        private List<GltfModel> m_gltfModels = null;
        private GltfOption m_gltfOption = null;

        public string SiteName
        {
            get { return m_strSiteName; }
            set { m_strSiteName = value; }
        }

        public List<Basic.SensorType> SensorTypes
        {
            get { return m_sensorTypes; }
            set { m_sensorTypes = value; }
        }

        public List<BuildingGroupData> BuildingGroups
        {
            get { return m_buildingGroups; }
            set { m_buildingGroups = value; }
        }

        public List<ZoneData> OutdoorZones
        {
            get { return m_outdoorZones; }
            set { m_outdoorZones = value; }
        }

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

        public List<GltfModel> Models
        {
            get { return m_gltfModels; }
            set { m_gltfModels = value; }
        }

        public GltfOption GltfOption
        {
            get { return m_gltfOption; }
            set { m_gltfOption = value; }
        }
    }
}
