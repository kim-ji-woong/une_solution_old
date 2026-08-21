using System.Collections.Generic;

namespace SDMS.IDAL
{
    using Model.Broadcast;
    using Model.History;
    using Model.Sensor;
    using Model.Spatial;
    using Model.Alarm;
    using Model.CCTV;
    using Model.Facility;

    public interface IDelete
    {
        bool DeleteZone(int id, out string strErrorMessage);
        bool DeleteZone(Dictionary<Zone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteEquipmentZone(int id, out string strErrorMessage);
        bool DeleteEquipmentZone(Dictionary<EquipmentZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteBuildingGroup(int id, out string strErrorMessage);
        bool DeleteBuildingGroup(Dictionary<BuildingGroup.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteBuilding(int id, out string strErrorMessage);
        bool DeleteBuilding(Dictionary<Building.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteSensorZone(int id, out string strErrorMessage);
        bool DeleteSensorZone(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeletePSMSensor(int id, out string strErrorMessage);
        bool DeletePSMSensor(Dictionary<PSM.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeletePSMMaterial(int id, out string strErrorMessage);
        bool DeletePSMMaterial(Dictionary<Material.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteFireSensor(int id, out string strErrorMessage);
        bool DeleteFireSensor(Dictionary<Fire.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteFacilityType(int id, out string strErrorMessage);
        bool DeleteFacilityType(Dictionary<FacilityType.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteSensorZoneHistory(int id, out string strErrorMessage);
        bool DeleteSensorZoneHistory(Dictionary<SensorZoneHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteSensorReactionHistory(int id, out string strErrorMessage);
        bool DeleteSensorReactionHistory(Dictionary<SensorReactionHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteSensorReactionHistoryDescription(int id, out string strErrorMessage);
        bool DeleteSensorReactionHistoryDescription(Dictionary<SensorReactionHistoryDescription.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteSensorReactionHistoryDescriptionText(int id, out string strErrorMessage);
        bool DeleteSensorReactionHistoryDescriptionText(Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteBroadcast(int id, out string strErrorMessage);
        bool DeleteBroadcast(Dictionary<Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteBroadcastHistory(int id, out string strErrorMessage);
        bool DeleteBroadcastHistory(Dictionary<Model.Broadcast.History.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteBroadcastState(int id, out string strErrorMessage);
        bool DeleteBroadcastState(Dictionary<Model.Broadcast.State.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteSMSHistory(int id, out string strErrorMessage);
        bool DeleteSMSHistory(Dictionary<SMSHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteBroadcastConfig(int id, out string strErrorMessage);
        bool DeleteBroadcastConfig(Dictionary<Model.Config.Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteSMSConfig(int id, out string strErrorMessage);
        bool DeleteSMSConfig(Dictionary<Model.Config.SMS.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteCurrentAlarm(Dictionary<CurrentAlarm.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteFacilityManager(int id, out string strErrorMessage);
        bool DeleteFacilityManager(Dictionary<FacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteBuildingFacilityManager(int id, out string strErrorMessage);
        bool DeleteBuildingFacilityManager(Dictionary<BuildingFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteEquipZoneFacilityManager(int id, out string strErrorMessage);
        bool DeleteEquipZoneFacilityManager(Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteCCTV(int id, out string strErrorMessage);
        bool DeleteCCTV(Dictionary<CCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteEquipZoneCCTV(int id, out string strErrorMessage);
        bool DeleteEquipZoneCCTV(Dictionary<EquipZoneCCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteGltfModel(int id, out string strErrorMessage);
        bool DeleteGltfModel(Dictionary<Model.GLTF.Model.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteGltfModelData(int id, out string strErrorMessage);
        bool DeleteGltfModelData(Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteGltfModelOrthoData(int id, out string strErrorMessage);
        bool DeleteGltfModelOrthoData(Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteOptionEtcSensor(int sensorType, out string strErrorMessage);
        bool DeleteEtcSensor(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteOptionEtcSensorData(int sensorType, int alarmDepth, out string strErrorMessage);
        bool DeleteEtcSensorData(Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteFacilityInfo(int id, out string strErrorMessage);
        bool DeleteFacilityInfo(Dictionary<Info.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteFacilityInfoData(int nFacilityInfoID, int nOrderIndex, out string strErrorMessage);
        bool DeleteFacilityInfoData(Dictionary<InfoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteBuildingData(int nBuildingID, int nOrderIndex, out string strErrorMessage);
        bool DeleteBuildingData(Dictionary<BuildingData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteBuildingGroupData(int nBuildingGroupID, int nOrderIndex, out string strErrorMessage);
        bool DeleteBuildingGroupData(Dictionary<BuildingGroupData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteFakeWall(int id, out string strErrorMessage);
        bool DeleteFakeWall(Dictionary<FakeWall.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteSpreadMessage(int id, out string strErrorMessage);
        bool DeleteSpreadMessage(Dictionary<Model.Config.SpreadMessage.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteZoneData(int zoneID, out string strErrorMessage);
        bool DeleteZoneData(Dictionary<ZoneData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
    }
}
