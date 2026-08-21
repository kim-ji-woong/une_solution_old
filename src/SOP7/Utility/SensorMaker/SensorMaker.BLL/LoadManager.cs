using SensorMaker.BLL.Models.Data;
using SensorMaker.BLL.Models.Data.Sensor;
using SensorMaker.BLL.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL
{
    public class LoadManager
    {
        private ProcessManager m_processManager = null;
        //private SDMS.IDAL.IDataManager m_sdmsDataManager = null;
        //private Common.IDAL.IDataManager m_commonDataManager = null;
        private static Models.Data.SpatialManager m_spatialManager = null;

        public LoadManager(ProcessManager processManager)
        {
            m_processManager = processManager;
            //m_sdmsDataManager = sdmsDataManager;
            //m_commonDataManager = commonDataManager;
        }

        public bool LoadSpatial()
        {
            if (m_spatialManager == null)
                m_spatialManager = new SpatialManager();

            return m_spatialManager.LoadSpatial(m_processManager.SdmsDataManager);
        }

        public ResponseBuildingGroupList RequestBuildingGroupList(List<int> siteIDs)
        {
            if (m_spatialManager == null)
                m_spatialManager = new SpatialManager();

            LoadSpatial();

            ResponseBuildingGroupList response = new ResponseBuildingGroupList();
            response.BuildingGroups = new List<BuildingGroupData>();

            foreach (BuildingGroupData bg in m_spatialManager.BuildingGroups)
            {
                if (siteIDs != null)
                {
                    if (siteIDs.Contains(bg.SiteID) == false)
                        continue;
                }

                response.BuildingGroups.Add(bg);
            }

            List<ZoneData> outdoorZones = m_spatialManager.GetOutdoorZones();

            foreach (ZoneData zone in outdoorZones)
            {
                if (siteIDs != null)
                {
                    if (siteIDs.Contains(zone.SiteID) == false)
                        continue;
                }

                response.OutdoorZones.Add(zone);
            }

            response.Success = true;
            return response;
        }

        public List<BuildingGroupData> GetBuildingGroups(List<int> siteIDs)
        {
            if (m_spatialManager == null)
                m_spatialManager = new SpatialManager();

            List<BuildingGroupData> buildingGroups = new List<BuildingGroupData>();
            foreach (BuildingGroupData bg in m_spatialManager.BuildingGroups)
            {
                if (siteIDs != null)
                {
                    if (siteIDs.Contains(bg.SiteID) == false)
                        continue;
                }

                //Models.Request.BuildingGroupVisibleData data = new Models.Request.BuildingGroupVisibleData();
                //data.ID = bg.ID;
                //data.GroupName = bg.GroupName;
                //data.ParentID = bg.ParentID;
                //data.TextCenter = bg.TextCenter;
                //data.DisplayText = bg.DisplayText;
                //data.SiteID = bg.SiteID;
                //data.Visible = bg.Visible;
                //data.building

                buildingGroups.Add(bg);
            }

            return buildingGroups;
        }

        public List<SDMS.Model.Spatial.Building> GetBuildings()
        {
            if (m_spatialManager == null)
                m_spatialManager = new SpatialManager();

            List<SDMS.Model.Spatial.Building> buildings = new List<SDMS.Model.Spatial.Building>();
            foreach (BuildingData b in m_spatialManager.Buildings)
            {
                buildings.Add(b);
            }

            return buildings;
        }

        public List<SDMS.Model.Spatial.Zone> GetZones()
        {
            if (m_spatialManager == null)
                m_spatialManager = new SpatialManager();

            List<SDMS.Model.Spatial.Zone> zones = new List<SDMS.Model.Spatial.Zone>();
            foreach (ZoneData z in m_spatialManager.Zones)
            {
                zones.Add(z);
            }

            return zones;
        }

        public List<SDMS.Model.Spatial.EquipmentZone> GetEquipmentZones()
        {
            if (m_spatialManager == null)
                m_spatialManager = new SpatialManager();

            List<SDMS.Model.Spatial.EquipmentZone> zones = new List<SDMS.Model.Spatial.EquipmentZone>();
            foreach (EquipmentZoneData ez in m_spatialManager.EquipZones)
            {
                zones.Add(ez);
            }

            return zones;
        }

        public List<ZoneData> GetOutdoorZones(List<int> siteIDs)
        {
            List<ZoneData> outdoorZoneList = new List<ZoneData>();

            List<ZoneData> outdoorZones = m_spatialManager.GetOutdoorZones();
            
            foreach (ZoneData zone in outdoorZones)
            {
                if (siteIDs != null)
                {
                    if (siteIDs.Contains(zone.SiteID) == false)
                        continue;
                }

                outdoorZoneList.Add(zone);
            }

            return outdoorZoneList;
        }

        public ResponseSensorList GetSensorList()
        {
            SensorManager sensorManager = new SensorManager();

            if (sensorManager.LoadSensorList(m_processManager.SdmsDataManager, m_spatialManager) == false)
                return MakeResponseSensorList(null, null, null, null, "센서정보를 읽어올수 없습니다.");

            return MakeResponseSensorList(sensorManager.FireSensors, sensorManager.PSMSensors, sensorManager.EtcSensors, sensorManager.CCTVs, "");
        }

        private ResponseSensorList MakeResponseSensorList(ICollection<FireSensor> fireSensors, ICollection<PSMSensor> psmSensors, ICollection<EtcSensor> etcSensors, ICollection<CCTVSensor> cctvs, string strMessage)
        {
            ResponseSensorList response = new ResponseSensorList();

            response.Success = strMessage == null || strMessage.Length == 0;
            response.Message = strMessage;

            //if (request.RequestFireSensors)
                response.FireSensors = MakeList<FireSensor>(fireSensors);

            //if (request.RequestPSMSensors)
                response.PSMSensors = MakeList<PSMSensor>(psmSensors);

            //if (request.RequestEtcSensors)
                response.EtcSensors = MakeList<EtcSensor>(etcSensors);

            //if (request.RequestCCTVs)
                response.Cctvs = MakeList<CCTVSensor>(cctvs);

            return response;
        }

        private List<DataType> MakeList<DataType>(ICollection<DataType> datas)
        {            
            if (datas == null)
                return null;

            List<DataType> dataList = new List<DataType>();

            foreach (DataType data in datas)
            {                
                dataList.Add(data);
            }

            return dataList;
        }

        public List<Models.Basic.SensorType> GetSensorTypes()
        {
            List<Models.Basic.SensorType> sensorTypes = new List<Models.Basic.SensorType>();

            string strErrorMessage;
            List<SDMS.Model.Sensor.FacilityType> facilityTypes = m_processManager.SdmsDataManager.GetSelectManager().SelectFacilityTypes(null, null, out strErrorMessage);
            if (facilityTypes != null)
            {

                foreach (SDMS.Model.Sensor.FacilityType facilityType in facilityTypes)
                {
                    Models.Basic.SensorType sensorType = new Models.Basic.SensorType();
                    sensorType.ID = facilityType.ID;
                    sensorType.Name = facilityType.TypeName;

                    if (!sensorTypes.Contains(sensorType))
                        sensorTypes.Add(sensorType);
                }
            }

            return sensorTypes;
        }

        public ResponseGltfDataList RequestGltfModelList(List<int> siteIDs/*, string strRootPath*/)
        {
            string strErrorMessage;
            ICollection<GltfModel> models = GltfManager.LoadGltfModels(m_processManager.SdmsDataManager, siteIDs, out strErrorMessage);

            if (models == null)
                return MakeResponseGltfDataList(null, null/*, strRootPath*/, strErrorMessage);

            // 계정에 따른 고,저용량 3D 모델 옵션 구하기
            string str3DHighVer = "true";

            //Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicConditions = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            //dicConditions[SOPManager.Model.Sop.Account.Option.Fields.UserID] = nUserID;
            //dicConditions[SOPManager.Model.Sop.Account.Option.Fields.Category] = "SDMS";
            //dicConditions[SOPManager.Model.Sop.Account.Option.Fields.SubCategory] = "3DHighVer";

            //List<SOPManager.Model.Sop.Account.Option> accountOptions = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicConditions, out strErrorMessage);

            //if (accountOptions != null && accountOptions.Count > 0)
            //{
            //    str3DHighVer = accountOptions[0].PropertyValue1;
            //}

            GltfOption option = GltfManager.LoadGltfOption(m_processManager.CommonDataManager, str3DHighVer, out strErrorMessage);

            if (option == null)
                return MakeResponseGltfDataList(models, null/*, strRootPath*/, strErrorMessage);

            return MakeResponseGltfDataList(models, option/*, strRootPath*/, "");
        }

        private ResponseGltfDataList MakeResponseGltfDataList(ICollection<GltfModel> models, GltfOption option/*, string strRootPath*/, string strMessage)
        {
            ResponseGltfDataList response = new ResponseGltfDataList();

            if (models == null || option == null)
            {
                response.Success = false;
            }
            else
            {
                response.Success = true;

                //string strRootResourceFolder = GetRootResourcePath(option, strRootPath);
                response.Models = new List<GltfModel>();

                foreach (GltfModel model in models)
                {
                    if (model.ParentID == null)
                        response.Models.Add(model);

                    /*foreach (Model.GLTF.ModelData modelData in model.ModelDatas)
                    {
                        CheckModelFileName(modelData, strRootResourceFolder);
                    }*/
                }

                response.GltfOption = option;
            }

            response.Message = strMessage;
            return response;
        }
    }
}
