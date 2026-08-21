using System;
using System.Collections.Generic;
using SDMS.IDAL;
using SDMS.Model.Spatial;
using SDMS.Model.Sensor;
using SDMS.Model.CCTV;
using UnE.Geometry;

namespace SDMS.BLL
{
    using Models.Request;
    using Models.Response;
    using Models.Data;
    using dnsData.Sensor;

    public class SaveManager
    {
        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        public SaveManager(IDataManager dataManager, ProcessManager processManager)
        {
            this.m_dataManager = dataManager;
            this.m_processManager = processManager;
        }

        public MessageResult SaveViewport(RequestSaveViewport request)
        {
            MessageResult response = new MessageResult();
            string strErrorMessage;

            response.Success = GltfManager.SaveViewport(m_dataManager, request, out strErrorMessage);
            response.Message = strErrorMessage;
            return response;
        }

        public MessageResult MoveBuildingNameText(RequestMoveBuildingNameText request)
        {
            Dictionary<BuildingGroup.Fields, object> dicConditions = new Dictionary<BuildingGroup.Fields, object>();
            dicConditions[BuildingGroup.Fields.GroupName] = request.BuildingGroupName;

            string strErrorMessage;
            List<BuildingGroup> buildingGroups = m_dataManager.GetSelectManager().SelectBuildingGroups(dicConditions, null, out strErrorMessage);

            if (buildingGroups == null)
                return new MessageResult(false, strErrorMessage);

            if (buildingGroups.Count == 0)
            {
                strErrorMessage = string.Format("{0}에 해당하는 건물그룹 정보를 찾을수 없습니다.", request.BuildingGroupName);
                return new MessageResult(false, strErrorMessage);
            }

            Dictionary<Building.Fields, object> dicConditions2 = new Dictionary<Building.Fields, object>();
            dicConditions2[Building.Fields.BuildingGroupID] = buildingGroups[0].ID;
            dicConditions2[Building.Fields.BuildingName] = request.BuildingName;

            List<Building> buildings = m_dataManager.GetSelectManager().SelectBuildings(dicConditions2, null, out strErrorMessage);

            if (buildings == null)
                return new MessageResult(false, strErrorMessage);

            if (buildings.Count == 0)
            {
                strErrorMessage = string.Format("{0}에 해당하는 건물 정보를 찾을수 없습니다.", request.BuildingName);
                return new MessageResult(false, strErrorMessage);
            }

            Building building = buildings[0];
            building.TextCenter.x = request.X;
            building.TextCenter.y = request.Y;
            building.TextCenter.z = request.Z;

            if (m_dataManager.GetUpdateManager().UpdateBuilding(building, out strErrorMessage))
                return new MessageResult(true, "");

            return new MessageResult(false, strErrorMessage);
        }

        public MessageResult MoveEquipZoneNameText(RequestMoveEquipZoneNameText request)
        {
            string strErrorMessage;
            EquipmentZone equipZone = m_dataManager.GetSelectManager().SelectEquipmentZone(request.EquipZoneID, out strErrorMessage);

            if (equipZone == null)
            {
                if (strErrorMessage == null)
                    return new MessageResult(false, string.Format("{0}에 해당하는 설비영역 정보를 찾을수 없습니다.", request.EquipZoneID));
                else
                    return new MessageResult(false, strErrorMessage);
            }

            equipZone.TextCenter = new Vertex3D(request.X, request.Y, request.Z);
            equipZone.DisplayText = request.DisplayText;

            if (m_dataManager.GetUpdateManager().UpdateEquipmentZone(equipZone, out strErrorMessage))
                return new MessageResult(true, "");

            return new MessageResult(false, strErrorMessage);
        }

        public MessageResult UpdatePOIPosition(RequestUpdatePOIPosition request, bool saveUserHistory = true, List<int> poiCount = null, List<int> textPoiCount = null)
        {
            if (request.SensorID <= 0)
            {
                string strErrorMessage = string.Format("Sensor ID는 0보다 커야만 합니다.");
                return new MessageResult(false, strErrorMessage);
            }

            if (request.Position == null)
            {
                string strErrorMessage = string.Format("Sensor의 위치정보가 존재하지 않습니다.");
                return new MessageResult(false, strErrorMessage);
            }

            if (SensorManager.IsFireSensor(request.SensorType))
            {
                if (poiCount != null)
                    poiCount[0] = poiCount[0] + 1;

                return UpdateFirePOIPosition(request.UserID, request.ZoneID, request.SensorID, request.Position, saveUserHistory);
            }
            else if (SensorManager.IsCCTVType(request.SensorType))
            {
                if (poiCount != null)
                    poiCount[0] = poiCount[0] + 1;

                return UpdateCCTVPOIPosition(request.UserID, request.ZoneID, request.SensorID, request.Position, saveUserHistory);
            }
            else if (SensorManager.IsPSMSensor(request.SensorType))
            {
                if (poiCount != null)
                    poiCount[0] = poiCount[0] + 1;

                return UpdatePSMPOIPosition(request.UserID, request.ZoneID, request.SensorID, request.Position, saveUserHistory);
            }
            else if (SensorManager.IsEtcSensor(request.SensorType))
            {
                if (poiCount != null)
                    poiCount[0] = poiCount[0] + 1;

                return UpdateEtcPOIPosition(request.UserID, request.ZoneID, request.SensorID, request.Position, saveUserHistory);
            }
            else if (string.Compare(request.SensorType, "textBuildingGroupName", true) == 0)
            {
                if (textPoiCount != null)
                    textPoiCount[0] = textPoiCount[0] + 1;

                return UpdateBuildingGroupTextPosition(request.UserID, request.ZoneID, request.SensorID, request.Position, request.Text, saveUserHistory);
            }
            else if (string.Compare(request.SensorType, "textBuildingName", true) == 0)
            {
                if (textPoiCount != null)
                    textPoiCount[1] = textPoiCount[1] + 1;

                return UpdateBuildingTextPosition(request.UserID, request.ZoneID, request.SensorID, request.Position, request.Text, saveUserHistory);
            }
            else if (string.Compare(request.SensorType, "textEquipZoneName", true) == 0)
            {
                if (textPoiCount != null)
                    textPoiCount[2] = textPoiCount[2] + 1;

                return UpdateEquipZoneTextPosition(request.UserID, request.ZoneID, request.SensorID, request.Position, request.Text, saveUserHistory);
            }

            return new MessageResult(false, string.Format("알려지지 않은 센서타입니다.({0})", request.SensorType));
        }

        public MessageResult UpdatePOIPositions(RequestUpdatePOIPositions request)
        {
            List<int> poiCount = new List<int>();
            List<int> textPoiCount = new List<int>();
            poiCount.Add(0);
            textPoiCount.Add(0);
            textPoiCount.Add(0);
            textPoiCount.Add(0);

            foreach (RequestUpdatePOIPosition req in request.Datas)
            {
                MessageResult result = UpdatePOIPosition(req, false, poiCount, textPoiCount);

                if (result.Success == false)
                    return result;
            }

            Common.BLL.ProcessManager commonProcessManager =
                    new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_dataManager);

            Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();

            if (textPoiCount[0] > 0)
            {
                RequestUpdatePOIPosition req = GetBuildingGroupNameRequest(request);

                if (req != null)
                {
                    commonSaveManager.SaveUserHistory_BuildingGroupName(req.UserID, req.SensorID);
                }
            }
            else if (textPoiCount[1] > 0)
            {
                RequestUpdatePOIPosition req = GetBuildingNameRequest(request);

                if (req != null)
                {
                    commonSaveManager.SaveUserHistory_BuildingName(req.UserID, req.SensorID);
                }
            }
            else if (textPoiCount[2] > 0)
            {
                RequestUpdatePOIPosition req = GetEquipZoneNameRequest(request);

                if (req != null)
                {
                    commonSaveManager.SaveUserHistory_EquipzoneName(req.UserID, req.ZoneID, req.SensorID);
                }
            }
            else if (poiCount[0] > 0)
            {
                dnsData.Sensor.Facility.FacilityType sensorType;
                RequestUpdatePOIPosition req = GetPOIRequest(request, out sensorType);

                if (req != null)
                {
                    commonSaveManager.SaveUserHistory_ModifyPOI(req.UserID, req.ZoneID, (int)sensorType, req.SensorID);
                }
            }

            return new MessageResult(true, "");
        }

        private RequestUpdatePOIPosition GetPOIRequest(RequestUpdatePOIPositions request, out dnsData.Sensor.Facility.FacilityType sensorType)
        {
            foreach (RequestUpdatePOIPosition req in request.Datas)
            {
                if (SensorManager.IsFireSensor(req.SensorType))
                {
                    sensorType = dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR;
                    return req;
                }
                else if (SensorManager.IsCCTVType(req.SensorType))
                {
                    sensorType = dnsData.Sensor.Facility.FacilityType.CCTV;
                    return req;
                }
                else if (SensorManager.IsPSMSensor(req.SensorType))
                {
                    sensorType = dnsData.Sensor.Facility.FacilityType.PSM_SENSOR;
                    return req;
                }
                else if (SensorManager.IsEtcSensor(req.SensorType))
                {
                    sensorType = dnsData.Sensor.Facility.FacilityType.ETC;
                    return req;
                }
            }

            sensorType = dnsData.Sensor.Facility.FacilityType.NONE;
            return null;
        }

        private RequestUpdatePOIPosition GetBuildingGroupNameRequest(RequestUpdatePOIPositions request)
        {
            foreach (RequestUpdatePOIPosition req in request.Datas)
            {
                if (string.Compare(req.SensorType, "textBuildingGroupName", true) == 0)
                {
                    return req;
                }
            }

            return null;
        }

        private RequestUpdatePOIPosition GetBuildingNameRequest(RequestUpdatePOIPositions request)
        {
            foreach (RequestUpdatePOIPosition req in request.Datas)
            {
                if (string.Compare(req.SensorType, "textBuildingName", true) == 0)
                {
                    return req;
                }
            }

            return null;
        }

        private RequestUpdatePOIPosition GetEquipZoneNameRequest(RequestUpdatePOIPositions request)
        {
            foreach (RequestUpdatePOIPosition req in request.Datas)
            {
                if (string.Compare(req.SensorType, "textEquipZoneName", true) == 0)
                {
                    return req;
                }
            }

            return null;
        }

        private MessageResult UpdateBuildingGroupTextPosition(int nUserID, int nZoneID, int nBuildingGroupID, Vertex3D vPos, string strText, bool saveUserHistory)
        {
            Dictionary<BuildingGroup.Fields, object> dicSets = new Dictionary<BuildingGroup.Fields, object>();
            Dictionary<BuildingGroup.Fields, object> dicConditions = new Dictionary<BuildingGroup.Fields, object>();

            dicConditions[BuildingGroup.Fields.ID] = nBuildingGroupID;
            dicSets[BuildingGroup.Fields.TextCenter] = VertexToString(vPos);

            if (strText != null)
                dicSets[BuildingGroup.Fields.DisplayText] = strText;
            
            string strErrorMessage;

            if (m_dataManager.GetUpdateManager().UpdateBuildingGroup(dicSets, dicConditions, null, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            if (saveUserHistory)
            {
                Common.BLL.ProcessManager commonProcessManager =
                    new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_dataManager);

                Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();
                commonSaveManager.SaveUserHistory_BuildingGroupName(nUserID, nBuildingGroupID);
            }

            return new MessageResult(true, "");
        }

        private MessageResult UpdateBuildingTextPosition(int nUserID, int nZoneID, int nBuildingID, Vertex3D vPos, string strText, bool saveUserHistory)
        {
            Dictionary<Building.Fields, object> dicSets = new Dictionary<Building.Fields, object>();
            Dictionary<Building.Fields, object> dicConditions = new Dictionary<Building.Fields, object>();

            dicConditions[Building.Fields.ID] = nBuildingID;
            dicSets[Building.Fields.TextCenter] = VertexToString(vPos);

            if (strText != null)
                dicSets[Building.Fields.DisplayText] = strText;

            string strErrorMessage;

            if (m_dataManager.GetUpdateManager().UpdateBuilding(dicSets, dicConditions, null, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            if (saveUserHistory)
            {
                Common.BLL.ProcessManager commonProcessManager =
                    new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_dataManager);

                Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();
                commonSaveManager.SaveUserHistory_BuildingName(nUserID, nBuildingID);
            }

            return new MessageResult(true, "");
        }

        private MessageResult UpdateEquipZoneTextPosition(int nUserID, int nZoneID, int nEquipZoneID, Vertex3D vPos, string strText, bool saveUserHistory)
        {
            Dictionary<EquipmentZone.Fields, object> dicSets = new Dictionary<EquipmentZone.Fields, object>();
            Dictionary<EquipmentZone.Fields, object> dicConditions = new Dictionary<EquipmentZone.Fields, object>();

            dicConditions[EquipmentZone.Fields.ID] = nEquipZoneID;
            dicSets[EquipmentZone.Fields.TextCenter] = VertexToString(vPos);

            if (strText != null)
                dicSets[EquipmentZone.Fields.ZoneName] = strText;

            string strErrorMessage;

            if (m_dataManager.GetUpdateManager().UpdateEquipmentZone(dicSets, dicConditions, null, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            if (saveUserHistory)
            {
                Common.BLL.ProcessManager commonProcessManager =
                        new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_dataManager);

                Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();
                commonSaveManager.SaveUserHistory_EquipzoneName(nUserID, nZoneID, nEquipZoneID);
            }

            return new MessageResult(true, "");
        }

        private MessageResult UpdateFirePOIPosition(int nUserID, int nZoneID, int nSensorID, Vertex3D vPos, bool saveUserHistory)
        {
            Dictionary<Fire.Fields, object> dicSets = new Dictionary<Fire.Fields, object>();
            Dictionary<Fire.Fields, object> dicConditions = new Dictionary<Fire.Fields, object>();

            dicConditions[Fire.Fields.ID] = nSensorID;
            dicSets[Fire.Fields.X] = vPos.x;
            dicSets[Fire.Fields.Y] = vPos.y;
            dicSets[Fire.Fields.Z] = vPos.z;

            string strErrorMessage;
            
            if (m_dataManager.GetUpdateManager().UpdateFireSensor(dicSets, dicConditions, null, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            if (saveUserHistory)
            {
                Common.BLL.ProcessManager commonProcessManager =
                        new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_dataManager);

                Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();
                commonSaveManager.SaveUserHistory_ModifyPOI(nUserID, nZoneID, (int)dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR, nSensorID);
            }

            return new MessageResult(true, "");
        }

        private MessageResult UpdateCCTVPOIPosition(int nUserID, int nZoneID, int nSensorID, Vertex3D vPos, bool saveUserHistory)
        {
            Dictionary<CCTV.Fields, object> dicSets = new Dictionary<CCTV.Fields, object>();
            Dictionary<CCTV.Fields, object> dicConditions = new Dictionary<CCTV.Fields, object>();

            dicConditions[CCTV.Fields.ID] = nSensorID;
            dicSets[CCTV.Fields.X] = vPos.x;
            dicSets[CCTV.Fields.Y] = vPos.y;
            dicSets[CCTV.Fields.Z] = vPos.z;

            string strErrorMessage;

            if (m_dataManager.GetUpdateManager().UpdateCCTV(dicSets, dicConditions, null, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            if (saveUserHistory)
            {
                Common.BLL.ProcessManager commonProcessManager =
                        new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_dataManager);

                Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();
                commonSaveManager.SaveUserHistory_ModifyPOI(nUserID, nZoneID, (int)dnsData.Sensor.Facility.FacilityType.CCTV, nSensorID);
            }

            return new MessageResult(true, "");
        }

        private MessageResult UpdatePSMPOIPosition(int nUserID, int nZoneID, int nSensorID, Vertex3D vPos, bool saveUserHistory)
        {
            Dictionary<PSM.Fields, object> dicSets = new Dictionary<PSM.Fields, object>();
            Dictionary<PSM.Fields, object> dicConditions = new Dictionary<PSM.Fields, object>();

            dicConditions[PSM.Fields.ID] = nSensorID;
            dicSets[PSM.Fields.X] = vPos.x;
            dicSets[PSM.Fields.Y] = vPos.y;
            dicSets[PSM.Fields.Z] = vPos.z;

            string strErrorMessage;

            if (m_dataManager.GetUpdateManager().UpdatePSMSensor(dicSets, dicConditions, null, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            if (saveUserHistory)
            {
                Common.BLL.ProcessManager commonProcessManager =
                        new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_dataManager);

                Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();
                commonSaveManager.SaveUserHistory_ModifyPOI(nUserID, nZoneID, (int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR, nSensorID);
            }

            return new MessageResult(true, "");
        }

        private MessageResult UpdateEtcPOIPosition(int nUserID, int nZoneID, int nSensorID, Vertex3D vPos, bool saveUserHistory)
        {
            Dictionary<ETC.Fields, object> dicSets = new Dictionary<ETC.Fields, object>();
            Dictionary<ETC.Fields, object> dicConditions = new Dictionary<ETC.Fields, object>();

            dicConditions[ETC.Fields.ID] = nSensorID;
            dicSets[ETC.Fields.X] = vPos.x;
            dicSets[ETC.Fields.Y] = vPos.y;
            dicSets[ETC.Fields.Z] = vPos.z;

            string strErrorMessage;

            if (m_dataManager.GetUpdateManager().UpdateETCSensors(dicSets, dicConditions, null, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            if (saveUserHistory)
            {
                Common.BLL.ProcessManager commonProcessManager =
                        new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_dataManager);

                Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();
                commonSaveManager.SaveUserHistory_ModifyPOI(nUserID, nZoneID, (int)dnsData.Sensor.Facility.FacilityType.ETC, nSensorID);
            }

            return new MessageResult(true, "");
        }

        public MessageResult UpdateIndoorModelViewport(RequestSaveIndoorModelViewport data)
        {
            if (data.ModelName == null || data.ModelName.Trim().Length == 0)
                return new MessageResult(false, "ModelName이 입력되지 않았습니다.");

            if (data.CameraPosition == null || data.CameraQuaternion == null || data.CameraRotation == null)
                return new MessageResult(false, "Camera Data가 null입니다.");

            if (data.OrbitTarget == null)
                return new MessageResult(false, "회전 중심점이 설정되지 않았습니다.");

            Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions = new Dictionary<Model.GLTF.ModelData.Fields, object>();
            Dictionary<Model.GLTF.ModelData.Fields, object> dicSets = new Dictionary<Model.GLTF.ModelData.Fields, object>();

            dicConditions[Model.GLTF.ModelData.Fields.ModelFile] = data.ModelName;

            dicSets[Model.GLTF.ModelData.Fields.CameraPositionX] = data.CameraPosition.x;
            dicSets[Model.GLTF.ModelData.Fields.CameraPositionY] = data.CameraPosition.y;
            dicSets[Model.GLTF.ModelData.Fields.CameraPositionZ] = data.CameraPosition.z;
            dicSets[Model.GLTF.ModelData.Fields.CameraQuaternionX] = data.CameraQuaternion.x;
            dicSets[Model.GLTF.ModelData.Fields.CameraQuaternionY] = data.CameraQuaternion.y;
            dicSets[Model.GLTF.ModelData.Fields.CameraQuaternionZ] = data.CameraQuaternion.z;
            dicSets[Model.GLTF.ModelData.Fields.CameraQuaternionW] = data.CameraQuaternion.w;
            dicSets[Model.GLTF.ModelData.Fields.CameraRotationX] = data.CameraRotation.x;
            dicSets[Model.GLTF.ModelData.Fields.CameraRotationY] = data.CameraRotation.y;
            dicSets[Model.GLTF.ModelData.Fields.CameraRotationZ] = data.CameraRotation.z;
            dicSets[Model.GLTF.ModelData.Fields.OrbitTargetX] = data.OrbitTarget.x;
            dicSets[Model.GLTF.ModelData.Fields.OrbitTargetY] = data.OrbitTarget.y;
            dicSets[Model.GLTF.ModelData.Fields.OrbitTargetZ] = data.OrbitTarget.z;

            string strErrorMessage;

            if (m_dataManager.GetUpdateManager().UpdateGltfModelData(dicSets, dicConditions, null, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            return new MessageResult(true, "");
        }

        public MessageResult UpdateOrthoModelViewport(RequestSaveOrthoModelViewport data)
        {
            if (data.ModelName == null || data.ModelName.Trim().Length == 0)
                return new MessageResult(false, "ModelName이 입력되지 않았습니다.");

            if (data.CameraPosition == null || data.CameraQuaternion == null || data.CameraRotation == null)
                return new MessageResult(false, "Camera Data가 null입니다.");

            if (data.Target == null)
                return new MessageResult(false, "카메라 중심점이 설정되지 않았습니다.");

            string strErrorMessage;
            Model.GLTF.ModelOrthoData orthoData = SelectModelOrthoData(data.ZoneID, out strErrorMessage);

            if (strErrorMessage != null)
            {
                return new MessageResult(false, strErrorMessage);
            }

            if (orthoData != null)
            {
                Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions = new Dictionary<Model.GLTF.ModelOrthoData.Fields, object>();
                Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicSets = new Dictionary<Model.GLTF.ModelOrthoData.Fields, object>();

                dicConditions[Model.GLTF.ModelOrthoData.Fields.ID] = orthoData.ID;

                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraPositionX] = data.CameraPosition.x;
                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraPositionY] = data.CameraPosition.y;
                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraPositionZ] = data.CameraPosition.z;
                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionX] = data.CameraQuaternion.x;
                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionY] = data.CameraQuaternion.y;
                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionZ] = data.CameraQuaternion.z;
                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionW] = data.CameraQuaternion.w;
                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraRotationX] = data.CameraRotation.x;
                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraRotationY] = data.CameraRotation.y;
                dicSets[Model.GLTF.ModelOrthoData.Fields.CameraRotationZ] = data.CameraRotation.z;
                dicSets[Model.GLTF.ModelOrthoData.Fields.TargetX] = data.Target.x;
                dicSets[Model.GLTF.ModelOrthoData.Fields.TargetY] = data.Target.y;
                dicSets[Model.GLTF.ModelOrthoData.Fields.TargetZ] = data.Target.z;
                dicSets[Model.GLTF.ModelOrthoData.Fields.Zoom] = data.Zoom;

                if (m_dataManager.GetUpdateManager().UpdateGltfModelOrthoData(dicSets, dicConditions, null, out strErrorMessage) == false)
                    return new MessageResult(false, strErrorMessage);
            }
            else
            {
                Model.GLTF.ModelData modelData = SelectModelData(data.ZoneID, out strErrorMessage);

                if (strErrorMessage != null)
                {
                    return new MessageResult(false, strErrorMessage);
                }
                else if (modelData == null)
                {
                    return new MessageResult(false, "Viewport를 저장할 3D 모델을 찾을수 없습니다.");
                }

                orthoData = m_dataManager.GetCreateManager().CreateGltfModelOrthoData(modelData.ModelID, data.ModelName, data.CameraPosition, data.CameraQuaternion, data.CameraRotation, data.Target, data.Zoom, data.ZoneID);

                if (orthoData == null)
                {
                    return new MessageResult(false, m_dataManager.GetCreateManager().GetErrorMessage());
                }
            }

            return new MessageResult(true, "");
        }

        private Model.GLTF.ModelOrthoData SelectModelOrthoData(int? zoneID, out string strErrorMessage)
        {
            Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions = new Dictionary<Model.GLTF.ModelOrthoData.Fields, object>();
            dicConditions[Model.GLTF.ModelOrthoData.Fields.ZoneID] = zoneID;

            List<Model.GLTF.ModelOrthoData> datas = m_dataManager.GetSelectManager().SelectGltfModelOrthoDatas(dicConditions, null, out strErrorMessage);

            if (datas == null || datas.Count == 0)
                return null;

            return datas[0];
        }

        private Model.GLTF.ModelData SelectModelData(int? zoneID, out string strErrorMessage)
        {
            Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions = new Dictionary<Model.GLTF.ModelData.Fields, object>();
            dicConditions[Model.GLTF.ModelData.Fields.ZoneID] = zoneID;

            if (zoneID == null)
            {
                dicConditions[Model.GLTF.ModelData.Fields.FloorIndex] = null;
                dicConditions[Model.GLTF.ModelData.Fields.BuildingGroupID] = null;
                dicConditions[Model.GLTF.ModelData.Fields.BuildingID] = null;
            }

            List<Model.GLTF.ModelData> datas = m_dataManager.GetSelectManager().SelectGltfModelDatas(dicConditions, null, out strErrorMessage);

            if (datas == null || datas.Count == 0)
                return null;

            return datas[0];
        }

        public static string VertexToString(Vertex3D vertex)
        {
            if (vertex == null)
                return null;

            string strVertex = string.Format("{0:F1}, {1:F1}, {2:F1}", vertex.x, vertex.y, vertex.z);
            return strVertex;
        }

        public MessageResult UpdateEquipZoneCCTVs(RequestUpdateEquipZoneCCTVs request)
        {
            string strErrorMessage;
            Dictionary<EquipZoneCCTV.Fields, object> dicConditions = new Dictionary<EquipZoneCCTV.Fields, object>();
            Dictionary<EquipZoneCCTV.Fields, object> dicSets = new Dictionary<EquipZoneCCTV.Fields, object>();

            foreach (EquipZoneCCTV equipZoneCCTV in request.EquipZoneCCTVs)
            {
                dicConditions.Remove(EquipZoneCCTV.Fields.ID);
                dicConditions[EquipZoneCCTV.Fields.EquipZoneID] = equipZoneCCTV.EquipZoneID;

                List<EquipZoneCCTV> equipZoneCCTVs = m_dataManager.GetSelectManager().SelectEquipZoneCCTVs(dicConditions, null, out strErrorMessage);

                if (equipZoneCCTVs == null)
                {
                    return new MessageResult(false, strErrorMessage);
                }
                else if (equipZoneCCTVs.Count == 0)
                {
                    EquipZoneCCTV data = m_dataManager.GetCreateManager().CreateEquipZoneCCTV(equipZoneCCTV.EquipZoneID, equipZoneCCTV.CCTV1, equipZoneCCTV.CCTV2, equipZoneCCTV.CCTV3, equipZoneCCTV.CCTV4, equipZoneCCTV.CCTV5, equipZoneCCTV.CCTV6, null, null, null, null, null, null, null);

                    if (data == null)
                    {
                        return new MessageResult(false, m_dataManager.GetCreateManager().GetErrorMessage());
                    }
                }
                else
                {
                    dicConditions[EquipZoneCCTV.Fields.ID] = equipZoneCCTVs[0].ID;
                    dicConditions.Remove(EquipZoneCCTV.Fields.EquipZoneID);

                    dicSets[EquipZoneCCTV.Fields.CCTV1] = equipZoneCCTV.CCTV1;
                    dicSets[EquipZoneCCTV.Fields.CCTV2] = equipZoneCCTV.CCTV2;
                    dicSets[EquipZoneCCTV.Fields.CCTV3] = equipZoneCCTV.CCTV3;
                    dicSets[EquipZoneCCTV.Fields.CCTV4] = equipZoneCCTV.CCTV4;
                    dicSets[EquipZoneCCTV.Fields.CCTV5] = equipZoneCCTV.CCTV5;
                    dicSets[EquipZoneCCTV.Fields.CCTV6] = equipZoneCCTV.CCTV6;

                    if (m_dataManager.GetUpdateManager().UpdateEquipZoneCCTV(dicSets, dicConditions, null, out strErrorMessage) == false)
                    {
                        return new MessageResult(false, strErrorMessage);
                    }
                }
            }

            return new MessageResult(true, "");
        }

        public MessageResult UpdateCCTVs(RequestUpdateCCTVs request)
        {
            Dictionary<CCTV.Fields, object> dicConditions = new Dictionary<CCTV.Fields, object>();
            Dictionary<CCTV.Fields, object> dicSets = new Dictionary<CCTV.Fields, object>();
            string strErrorMessage;

            foreach (UpdateCCTV data in request.UpdateCCTVs)
            {
                dicConditions[CCTV.Fields.ID] = data.ID;
                //dicSets[CCTV.Fields.ZoneID] = data.ZoneID;
                dicSets[CCTV.Fields.X] = data.X;
                dicSets[CCTV.Fields.Y] = data.Y;
                dicSets[CCTV.Fields.Z] = data.Z;

                if (data.ZoneID != null && data.ZoneID < 0)
                    dicSets[CCTV.Fields.ZoneID] = null;
                else
                    dicSets[CCTV.Fields.ZoneID] = data.ZoneID;

                if (m_dataManager.GetUpdateManager().UpdateCCTV(dicSets, dicConditions, null, out strErrorMessage) == false)
                {
                    return new MessageResult(false, strErrorMessage);
                }
            }

            if (request.UpdateCCTVs.Count > 0)
            {
                Common.BLL.ProcessManager commonProcessManager =
                            new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_dataManager);

                Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();

                int? zoneID = request.UpdateCCTVs[0].ZoneID;

                if (zoneID != null && zoneID > 0)
                    commonSaveManager.SaveUserHistory_AddPOI(request.UserID, (int)zoneID, (int)Facility.FacilityType.CCTV, request.UpdateCCTVs[0].ID);
                else if (zoneID != null && zoneID < 0)
                    commonSaveManager.SaveUserHistory_DeletePOI(request.UserID, -(int)zoneID, (int)Facility.FacilityType.CCTV, request.UpdateCCTVs[0].ID);
            }

            return new MessageResult(true, "");
        }

        public MessageResult SetSpreadMessage(RequestSetSpreadMessage data)
        {
            MessageResult result = new MessageResult();
            string strErrorMessage = "";
            string strAdditionalConditions = null;

            List<SDMS.Model.Config.SpreadMessage> addSpreadMessage = data.AddSpreadMessage;
            List<SDMS.Model.Config.SpreadMessage> updateSpreadMessage = data.UpdateSpreadMessage;
            List<SDMS.Model.Config.SpreadMessage> removeSpreadMessage = data.RemoveSpreadMessage;

            foreach (SDMS.Model.Config.SpreadMessage spread in addSpreadMessage)
            {
                SDMS.Model.Config.SpreadMessage spreadData = m_processManager.SdmsDataManager.GetCreateManager().CreateSpreadMessage(spread.FacilityType, spread.BuildingGroupID, spread.BuildingID, spread.RegularID, spread.RegularMemberID, spread.MessageType, spread.Message);

                if (spreadData == null)
                {
                    result.Success = false;
                    result.Message = "CreateSpreadMessage 실패";
                    return result;
                }
            }

            foreach (SDMS.Model.Config.SpreadMessage spread in updateSpreadMessage)
            {
                if (m_processManager.SdmsDataManager.GetUpdateManager().UpdateSpreadMessage(spread, out strErrorMessage) == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            foreach (SDMS.Model.Config.SpreadMessage spread in removeSpreadMessage)
            {
                if (m_processManager.SdmsDataManager.GetDeleteManager().DeleteSpreadMessage(spread.ID, out strErrorMessage) == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            result.Success = true;
            return result;
        }
    }
}
