using System;
using System.Collections.Generic;
using UnE.Geometry;

namespace SDMS.IDAL
{
    using Model.History;
    using Model.Sensor;
    using Model.Spatial;
    using Model.Alarm;
    using Model.CCTV;
    using Model.Facility;

    public interface ICreate
    {
        string GetErrorMessage();

        Zone CreateZone(string strZoneName, int? nBuildingID, int? nFloorIndex, float? fAddFloor, Polygon boundary, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText, int nSiteID);
        EquipmentZone CreateEquipmentZone(string strZoneName, Polygon boundary, List<int> linkedZoneIDList, int? nType, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText, int nSiteID);
        BuildingGroup CreateBuildingGroup(string strGroupName, int? nParentID, Vertex3D vTextCenter, string strDisplayText, int nSiteID);
        Building CreateBuilding(string strBuildingCode, string strBuildingName, int nBuildingGroupID, int nMaxFloor, int nMinFloor, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText);
        SensorZone CreateSensorZone(int nType, int nOrgSensorID, int nEquipZoneID, bool isAlarmStatus, int? data);
        PSM CreatePSMSensor(string strName, string strPositionName, float? x, float? y, float? z, float? fCurrentData, float? fLimitLevel1, float? fLimitLevel2, float? fLimitLevel3, int nZoneID, int nEquipZoneID, bool useLimitLevel1, bool useLimitLevel2, bool useLimitLevel3, string strDepartment, string strDepartmentPhoneNumber, string strStatus, string strUniqueKey, int? materialType);
        Material CreateMaterial(string strMaterialName, string strUOM, int nSiteID, string strDescription);        
        Fire CreateFireSensor(string strName, string strPositionName, float? x, float? y, float? z, int nZoneID, string strDepartment, string strDepartmentPhoneNumber);
        FacilityType CreateFacilityType(string strTypeName, string strLinkedTableName, int nSiteID, string strDescription, int? nDisasterCategoryID, int? nSubDisasterCategoryID);
        SensorZoneHistory CreateSensorZoneHistory(int nSensorZoneID, string strData, DateTime time, int nZoneID, int nSensorType, int? nDetectionStatus, int nSiteID, string strMemo, List<int> allSensorZoneIDs = null);
        SensorReactionHistory CreateSensorReactionHistory(int nSensorZoneHistoryID, int nReactionType, DateTime time, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5);
        SensorReactionHistoryDescription CreateSensorReactionHistoryDescription(int nSensorReactionHistoryID, int nDescriptionID, int? nSensorZoneHistoryID);
        SensorReactionHistoryDescriptionText CreateSensorReactionHistoryDescriptionText(int nRefCount, string strDescription);
        Model.Broadcast.Broadcast CreateBroadcast(string strText, bool useSiren, int nPlayOption, int nRepeatCount, DateTime requestTime, int nSiteID);
        Model.Broadcast.History CreateBroadcastHistory(string strText, bool useSiren, int nPlayOption, int nRepeatCount, DateTime requestTime, DateTime executeTime, int nSiteID);
        Model.Broadcast.State CreateBroadcastState(DateTime heartBeat, int nBState, int nSiteID);
        SMSHistory CreateSMSHistory(int nSensorZoneHistoryID, int nSensorReactionHistoryID, string strSMSMessage, bool sendType, List<int> regularMemberIDList = null);
        Model.Config.Broadcast CreateBroadcastConfig(int nSituationType, bool useBroadcast, string strMessage, bool useSiren, int nRepeatCount, string strDescription, int nSiteID);
        Model.Config.SMS CreateSMSConfig(int nMessageType, bool useSMS, string strDescription, int nSiteID);
        CurrentAlarm CreateCurrentAlarm(int nSensorZoneHistoryID, int nSensorType, int nAlarmType, DateTime timeStamp, int nSopStatus, int nAlarmDepth, List<int> alarmSensorZoneIDs);
        FacilityManager CreateFacilityManager(int nMemberID, int nMemberType, int nFacilityType, int nDetectType, string strDescription, int nSiteID);
        BuildingFacilityManager CreateBuildingFacilityManager(int nMemberID, int nMemberType, int nFacilityType, int nDetectType, int nBuildingID, string strDescription, int nSiteID);
        EquipZoneFacilityManager CreateEquipZoneFacilityManager(int nMemberID, int nMemberType, int nFacilityType, int nDetectType, int nEquipZoneID, string strDescription, int nSiteID);
        CCTV CreateCCTV(string strCameraName, string strPositionName, string strUniqueKey, float? x, float? y, float? z, int? nZoneID, bool isIndoor, string strType, int? nChannel, string strUserID, string strPassword, string strURL, string strBigURL, string strSmallURL, bool? enabled, string strCameraIP, string strCameraCompanyName, string strCameraModelName, string strDescription);
        EquipZoneCCTV CreateEquipZoneCCTV(int nEquipZoneID, int? nCCTV1, int? nCCTV2, int? nCCTV3, int? nCCTV4, int? nCCTV5, int? nCCTV6, string strPreset1, string strPreset2, string strPreset3, string strPreset4, string strPreset5, string strPreset6, string strDescription);
        Model.GLTF.Model CreateGltfModel(int? nParentID, string strModelName, int nSiteID);
        Model.GLTF.ModelData CreateGltfModelData(int nModelID, string strModelFile, string strModelDisplayText, Vertex3D vCameraPosition, Quaternion qCameraQuaternion, Vertex3D vCameraRotation, int nFov, float fNear, float fFar, Vertex3D vOrbitTarget, float? fFloorIndex, int? nBuildingGroupID, int? nBuildingID, int? nZoneID);
        Model.GLTF.ModelOrthoData CreateGltfModelOrthoData(int nModelID, string strModelFile, Vertex3D vCameraPosition, Quaternion qCameraQuaternion, Vertex3D vCameraRotation, Vertex3D vTarget, float fZoom, int? nZoneID);
        Model.Sensor.Option.Etc CreateOptionEtcSensor(int nSensorType, int nDataType, int? nCloseAlarmSeconds, int? nDelaySeconds, int nSiteID);
        Model.Sensor.Option.EtcData CreateOptionEtcSensorData(int nSensorType, int nAlarmDepth, int? nDataMin, float? fDataMin, string strDataMin, int? nDataMax, float? fDataMax, string strDataMax, List<int> linkedBuildingIDs, List<int> linkedZoneIDs, bool sendSDMS);
        Info CreateFacilityInfo(string strModelName, string strFacilityName, int nZoneID);
        InfoData CreateFacilityInfoData(int nFacilityInfoID, int nOrderIndex, string strValue, bool withDot, int? indentDepth);
        BuildingData CreateBuildingData(int nBuildingID, int nOrderIndex, string strValue, bool withDot, int? indentDepth);
        BuildingGroupData CreateBuildingGroupData(int nBuildingGroupID, int nOrderIndex, string strValue, bool withDot, int? indentDepth);
        // fRotate : Radian
        FakeWall CreateFakeWall(int nZoneID, float x, float y, float z, float fRotate, float fScale);
        Model.Config.SpreadMessage CreateSpreadMessage(int nFacilityType, int? nBuilidingGroupID, int? nBuilidingID, string strRegularID, string strRegularMemberID, int nMessageType, string strMessage);
        ZoneData CreateZoneData(int nZoneID, float? fakeWallElevation, float? poiElevation);
    }
}
