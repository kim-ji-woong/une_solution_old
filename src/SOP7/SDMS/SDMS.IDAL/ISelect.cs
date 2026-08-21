using System.Collections;
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
    using System;

    public interface ISelect
    {
        Zone SelectZone(int id, out string strErrorMessage);
        List<Zone> SelectZones(Dictionary<Zone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Zone> SelectZones(Dictionary<Zone.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        EquipmentZone SelectEquipmentZone(int id, out string strErrorMessage);
        List<EquipmentZone> SelectEquipmentZones(Dictionary<EquipmentZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<EquipmentZone> SelectEquipmentZones(Dictionary<EquipmentZone.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        BuildingGroup SelectBuildingGroup(int id, out string strErrorMessage);
        List<BuildingGroup> SelectBuildingGroups(Dictionary<BuildingGroup.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<BuildingGroup> SelectBuildingGroups(Dictionary<BuildingGroup.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Building SelectBuilding(int id, out string strErrorMessage);
        List<Building> SelectBuildings(Dictionary<Building.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Building> SelectBuildings(Dictionary<Building.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        SensorZone SelectSensorZone(int id, out string strErrorMessage);
        List<SensorZone> SelectSensorZones(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<SensorZone> SelectSensorZones(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        PSM SelectPSMSensor(int id, out string strErrorMessage);
        List<PSM> SelectPSMSensors(Dictionary<PSM.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<PSM> SelectPSMSensors(Dictionary<PSM.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Material SelectMaterial(int id, out string strErrorMessage);
        List<Material> SelectMaterials(Dictionary<Material.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Material> SelectMaterials(Dictionary<Material.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Fire SelectFireSensor(int id, out string strErrorMessage);
        List<Fire> SelectFireSensors(Dictionary<Fire.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Fire> SelectFireSensors(Dictionary<Fire.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ETC SelectETCSensor(int id, out string strErrorMessage);
        List<ETC> SelectETCSensors(Dictionary<ETC.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<ETC> SelectETCSensors(Dictionary<ETC.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        FacilityType SelectFacilityType(int id, out string strErrorMessage);
        List<FacilityType> SelectFacilityTypes(Dictionary<FacilityType.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<FacilityType> SelectFacilityTypes(Dictionary<FacilityType.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        SensorZoneHistory SelectSensorZoneHistory(int id, out string strErrorMessage);
        List<SensorZoneHistory> SelectSensorZoneHistories(Dictionary<SensorZoneHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<SensorZoneHistory> SelectSensorZoneHistories(Dictionary<SensorZoneHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        SensorReactionHistory SelectSensorReactionHistory(int id, out string strErrorMessage);
        List<SensorReactionHistory> SelectSensorReactionHistories(Dictionary<SensorReactionHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<SensorReactionHistory> SelectSensorReactionHistories(Dictionary<SensorReactionHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        SensorReactionHistoryDescription SelectSensorReactionHistoryDescription(int id, out string strErrorMessage);
        List<SensorReactionHistoryDescription> SelectSensorReactionHistoryDescriptions(Dictionary<SensorReactionHistoryDescription.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<SensorReactionHistoryDescription> SelectSensorReactionHistoryDescriptions(Dictionary<SensorReactionHistoryDescription.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        SensorReactionHistoryDescriptionText SelectSensorReactionHistoryDescriptionText(int id, out string strErrorMessage);
        List<SensorReactionHistoryDescriptionText> SelectSensorReactionHistoryDescriptionTexts(Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<SensorReactionHistoryDescriptionText> SelectSensorReactionHistoryDescriptionTexts(Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ServerInfo SelectSensorServerInfo(int id, out string strErrorMessage);
        List<ServerInfo> SelectSensorServerInfo(Dictionary<ServerInfo.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<ServerInfo> SelectSensorServerInfo(Dictionary<ServerInfo.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        TagInfo SelectSensorTagInfo(int id, out string strErrorMessage);
        List<TagInfo> SelectSensorTagInfo(Dictionary<TagInfo.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<TagInfo> SelectSensorTagInfo(Dictionary<TagInfo.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        List<FireSensorZone> SelectFireSensorZone(out string strErrorMessage);
        List<FireSensorZone> SelectFireSensorZone(int? topNCount, out string strErrorMessage);
        Broadcast SelectBroadcast(int id, out string strErrorMessage);
        List<Broadcast> SelectBroadcasts(Dictionary<Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Broadcast> SelectBroadcasts(Dictionary<Broadcast.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.Broadcast.History SelectBroadcastHistory(int id, out string strErrorMessage);
        List<Model.Broadcast.History> SelectBroadcastHistories(Dictionary<Model.Broadcast.History.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.Broadcast.History> SelectBroadcastHistories(Dictionary<Model.Broadcast.History.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.Broadcast.State SelectBroadcastState(int id, out string strErrorMessage);
        List<Model.Broadcast.State> SelectBroadcastStates(Dictionary<Model.Broadcast.State.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.Broadcast.State> SelectBroadcastStates(Dictionary<Model.Broadcast.State.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        SMSHistory SelectSMSHistory(int id, out string strErrorMessage);
        List<SMSHistory> SelectSMSHistories(Dictionary<SMSHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<SMSHistory> SelectSMSHistories(Dictionary<SMSHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.Config.Broadcast SelectBroadcastConfig(int id, out string strErrorMessage);
        List<Model.Config.Broadcast> SelectBroadcastConfigs(Dictionary<Model.Config.Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.Config.Broadcast> SelectBroadcastConfigs(Dictionary<Model.Config.Broadcast.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.Config.SMS SelectSMSConfig(int id, out string strErrorMessage);
        List<Model.Config.SMS> SelectSMSConfigs(Dictionary<Model.Config.SMS.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.Config.SMS> SelectSMSConfigs(Dictionary<Model.Config.SMS.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        List<CurrentAlarm> SelectCurrentAlarms(Dictionary<CurrentAlarm.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<CurrentAlarm> SelectCurrentAlarms(Dictionary<CurrentAlarm.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        FacilityManager SelectFacilityManager(int id, out string strErrorMessage);
        List<FacilityManager> SelectFacilityManagers(Dictionary<FacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<FacilityManager> SelectFacilityManagers(Dictionary<FacilityManager.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        BuildingFacilityManager SelectBuildingFacilityManager(int id, out string strErrorMessage);
        List<BuildingFacilityManager> SelectBuildingFacilityManagers(Dictionary<BuildingFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<BuildingFacilityManager> SelectBuildingFacilityManagers(Dictionary<BuildingFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        EquipZoneFacilityManager SelectEquipZoneFacilityManager(int id, out string strErrorMessage);
        List<EquipZoneFacilityManager> SelectEquipZoneFacilityManagers(Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<EquipZoneFacilityManager> SelectEquipZoneFacilityManagers(Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        CCTV SelectCCTV(int id, out string strErrorMessage);
        List<CCTV> SelectCCTVs(Dictionary<CCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<CCTV> SelectCCTVs(Dictionary<CCTV.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        EquipZoneCCTV SelectEquipZoneCCTV(int id, out string strErrorMessage);
        List<EquipZoneCCTV> SelectEquipZoneCCTVs(Dictionary<EquipZoneCCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<EquipZoneCCTV> SelectEquipZoneCCTVs(Dictionary<EquipZoneCCTV.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.GLTF.Model SelectGltfModel(int id, out string strErrorMessage);
        List<Model.GLTF.Model> SelectGltfModels(Dictionary<Model.GLTF.Model.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.GLTF.Model> SelectGltfModels(Dictionary<Model.GLTF.Model.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.GLTF.ModelData SelectGltfModelData(int id, out string strErrorMessage);
        List<Model.GLTF.ModelData> SelectGltfModelDatas(Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.GLTF.ModelData> SelectGltfModelDatas(Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.GLTF.ModelOrthoData SelectGltfModelOrthoData(int id, out string strErrorMessage);
        List<Model.GLTF.ModelOrthoData> SelectGltfModelOrthoDatas(Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.GLTF.ModelOrthoData> SelectGltfModelOrthoDatas(Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.Sensor.Option.Etc SelectOptionEtcSensor(int sensorType, out string strErrorMessage);
        List<Model.Sensor.Option.Etc> SelectOptionEtcSensors(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.Sensor.Option.Etc> SelectOptionEtcSensors(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.Sensor.Option.EtcData SelectOptionEtcSensorData(int sensorType, int alarmDepth, out string strErrorMessage);
        List<Model.Sensor.Option.EtcData> SelectOptionEtcSensorDatas(Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.Sensor.Option.EtcData> SelectOptionEtcSensorDatas(Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Info SelectFacilityInfo(string strModelName, out string strErrorMessage);
        List<Info> SelectFacilityInfos(Dictionary<Info.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Info> SelectFacilityInfos(Dictionary<Info.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        InfoData SelectFacilityInfoData(int nFacilityInfoID, int nOrderIndex, out string strErrorMessage);
        List<InfoData> SelectFacilityInfoDatas(Dictionary<InfoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<InfoData> SelectFacilityInfoDatas(Dictionary<InfoData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        BuildingData SelectBuildingData(int nBuildingID, int nOrderIndex, out string strErrorMessage);
        List<BuildingData> SelectBuildingDatas(Dictionary<BuildingData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<BuildingData> SelectBuildingDatas(Dictionary<BuildingData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        BuildingGroupData SelectBuildingGroupData(int nBuildingGroupID, int nOrderIndex, out string strErrorMessage);
        List<BuildingGroupData> SelectBuildingGroupDatas(Dictionary<BuildingGroupData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<BuildingGroupData> SelectBuildingGroupDatas(Dictionary<BuildingGroupData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        FakeWall SelectFakeWall(int id, out string strErrorMessage);
        List<FakeWall> SelectFakeWalls(Dictionary<FakeWall.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<FakeWall> SelectFakeWalls(Dictionary<FakeWall.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Model.Config.SpreadMessage SelectSpreadMessage(int id, out string strErrorMessage);
        List<Model.Config.SpreadMessage> SelectSpreadMessages(Dictionary<Model.Config.SpreadMessage.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Model.Config.SpreadMessage> SelectSpreadMessages(Dictionary<Model.Config.SpreadMessage.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        // 현재 진행중인 알람에 대한 정보들을 얻어온다.
        // ArrayList에는 EquipmentZone, SensorReactionHistory, SensorZone, SensorZoneHistory 순서대로 객체들이 담겨진다.
        ArrayList SelectCurrentAlarmHistories(string strAlarmOnReactionTypes, string strAlarmOffReactionTypes, out string strErrorMessage);
        // 현재 진행중인 알람에 대한 정보들을 얻어온다.
        // ArrayList에는 EquipmentZone, SensorReactionHistory, SensorZone, SensorZoneHistory 순서대로 객체들이 담겨진다.
        ArrayList SelectCurrentAlarmHistories(string strAlarmOnReactionTypes, string strAlarmOffReactionTypes, int? topNCount, out string strErrorMessage);
        ZoneData SelectZoneData(int zoneID, out string strErrorMessage);
        List<ZoneData> SelectZoneDatas(Dictionary<ZoneData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<ZoneData> SelectZoneDatas(Dictionary<ZoneData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);

        /// <summary>
        /// EquipmentZone, SensorReactionHistory, SensorZone, SensorZoneHistory, Building, Zone
        /// </summary>
        /// <param name="dicConditions1"></param>
        /// <param name="dicConditions2"></param>
        /// <param name="dicConditions3"></param>
        /// <param name="dicConditions4"></param>
        /// <param name="strAdditionalConditions"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        ArrayList JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory(Dictionary<EquipmentZone.Fields, object> dicConditions1, Dictionary<SensorReactionHistory.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, Dictionary<SensorZoneHistory.Fields, object> dicConditions4, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory(Dictionary<EquipmentZone.Fields, object> dicConditions1, Dictionary<SensorReactionHistory.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, Dictionary<SensorZoneHistory.Fields, object> dicConditions4, string strAdditionalConditions, int? topNCount, out string strErrorMessage);

        /// <summary>
        /// EquipmentZone, SensorReactionHistory, SensorZone, SensorZoneHistory, Zone
        /// </summary>
        /// <param name="dicConditions1"></param>
        /// <param name="dicConditions2"></param>
        /// <param name="dicConditions3"></param>
        /// <param name="dicConditions4"></param>
        /// <param name="strAdditionalConditions"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        ArrayList JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory2(Dictionary<EquipmentZone.Fields, object> dicConditions1, Dictionary<SensorReactionHistory.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, Dictionary<SensorZoneHistory.Fields, object> dicConditions4, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory2(Dictionary<EquipmentZone.Fields, object> dicConditions1, Dictionary<SensorReactionHistory.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, Dictionary<SensorZoneHistory.Fields, object> dicConditions4, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList GetMinMaxIndexSensorReactionHistory(string strAdditionalConditions, out string strErrorMessage);

        ArrayList JoinHistroysensorreactionSpatialequipmentzoneSensorZone(Dictionary<SensorReactionHistory.Fields, object> dicConditions1, Dictionary<EquipmentZone.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinHistroysensorreactionSpatialequipmentzoneSensorZone(Dictionary<SensorReactionHistory.Fields, object> dicConditions1, Dictionary<EquipmentZone.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinOptionEtcSensorOptionEtcSensorData(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions1, Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinOptionEtcSensorOptionEtcSensorData(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions1, Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinSensorZoneTagInfo(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<TagInfo.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneTagInfo(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<TagInfo.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinFacilityInfoFacilityInfoData(Dictionary<Info.Fields, object> dicConditions1, Dictionary<InfoData.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinFacilityInfoFacilityInfoData(Dictionary<Info.Fields, object> dicConditions1, Dictionary<InfoData.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinBuildingGroupBuildingZone(Dictionary<BuildingGroup.Fields, object> dicConditions1, Dictionary<Building.Fields, object> dicConditions2, Dictionary<Zone.Fields, object> dicConditions3, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinBuildingGroupBuildingZone(Dictionary<BuildingGroup.Fields, object> dicConditions1, Dictionary<Building.Fields, object> dicConditions2, Dictionary<Zone.Fields, object> dicConditions3, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinSensorZoneFireSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<Fire.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneFireSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<Fire.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinSensorZonePSMSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<PSM.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZonePSMSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<PSM.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinSensorZoneETCSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<ETC.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneETCSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<ETC.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        /// <summary>
        /// SensorType에 따라 OrgSensorTable과 조인하여 센서명을 가져온다
        /// </summary>
        /// <param name="dicConditions"></param>
        /// <param name="strAdditionalConditions"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns>SensorZoneID, SensorName, SensorType</returns>
        ArrayList JoinSensorZoneSensors(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneSensors(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinEquipmentZoneEquipZoneCCTV(int equipZoneID, string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinEquipmentZoneEquipZoneCCTV(int equipZoneID, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinSensorZoneSensorZoneHistory(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneSensorZoneHistory(string strAdditionalConditions, int? topNCount, out string strErrorMessage);

        /// <summary>
        /// SensorZoneHistory, SensorReactionHistory
        /// </summary>
        /// <param name="strAdditionalConditions"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        ArrayList JoinSensorZoneHistorySensorReactionHistory(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneHistorySensorReactionHistory(string strAdditionalConditions, int? topNCount, out string strErrorMessage);

        ArrayList JoinSensorZoneEquipmentZoneZoneBuildingBuildingGroup(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneEquipmentZoneZoneBuildingBuildingGroup(string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinSensorZoneHistoryZoneBuildingBuildingGroup(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneHistoryZoneBuildingBuildingGroup(string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinSensorZoneHistorySensorZoneZoneBuildingBuildingGroup(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneHistorySensorZoneZoneBuildingBuildingGroup(string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        /// <summary>
        /// 현재 발생중인 알람에 대한 정보
        /// </summary>
        /// <param name="strAdditionalConditions"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        ArrayList JoinCurrentAlarmHistory(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinCurrentAlarmHistory(string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinSensorZoneTagInfoETCMaterial(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneTagInfoETCMaterial(string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinSensorZoneTagInfoPSMMaterial(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinSensorZoneTagInfoPSMMaterial(string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinPSMSensorMaterial(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinPSMSensorMaterial(string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinETCSensorMaterial(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinETCSensorMaterial(string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        ArrayList JoinCurrentAlarmSensorZoneHistorySensorZoneTagInfo(string strAdditionalConditions, out string strErrorMessage);
        ArrayList JoinCurrentAlarmSensorZoneHistorySensorZoneTagInfo(string strAdditionalConditions, int? topNCount, out string strErrorMessage);
    }
}
