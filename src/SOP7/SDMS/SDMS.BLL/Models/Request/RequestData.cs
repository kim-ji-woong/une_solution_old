using UnE.Geometry;
using System.Collections.Generic;

namespace SDMS.BLL.Models.Request
{
    public class RequestData
    {
        private RequestBuildingGroupList m_requestBuildingGroupList = null;
        private RequestGltfDataList m_requestGltfDataList = null;
        private RequestSaveViewport m_requestSaveViewport = null;
        private RequestMoveBuildingNameText m_requestMoveBuildingNameText = null;
        private RequestMoveEquipZoneNameText m_requestMoveEquipZoneNameText = null;
        private RequestSensorList m_requestSensorList = null;
        private RequestMoveSensor m_requestMoveSensor = null;
        private RequestMalfunction m_requestMalfunction = null;
        private RequestSituationNotice m_requestSituationNotice = null;
        private RequestEquipZoneCCTV m_requestEquipZoneCCTV = null;
        private RequestEquipZoneCCTVFromSensor m_requestEquipZoneCCTVFromSensor = null;
        private RequestEquipZoneSensorList m_requestEquipZoneSensorList = null;
        private RequestUpdateEquipZoneCCTVs m_requestUpdateEquipZoneCCTVs = null;
        private RequestGetOrgSensorID m_requestGetOrgSensorID = null;
        private bool? m_requestSensorCount = null;
        private bool? m_requestStreamServerURL = null;
        private RequestFacilityType m_requestFacilityType = null;
        private bool? m_requestAllFacilityInfo = null;
        private RequestUpdatePOIPosition m_requestUpdatePOIPosition = null;
        private RequestUpdatePOIPositions m_requestUpdatePOIPositions = null;
        private RequestUpdateCCTVs m_requestUpdateCCTVs = null;
        private RequestFacilityInfoData m_requestFacilityInfoData = null;
        private RequestBuildingData m_requestBuildingData = null;
        private RequestBuildingGroupData m_requestBuildingGroupData = null;
        private bool? m_requestOuterDatas = null;
        private RequestIndoorData m_requestIndoorDatas = null;
        private RequestSaveIndoorModelViewport m_requestSaveIndoorModelViewport = null;
        private RequestSaveOrthoModelViewport m_requestSaveOrthoModelViewport = null;
        private RequestFakeWalls m_requestFakeWalls = null;
        private RequestUpdateFakeWall m_requestUpdateFakeWall = null;
        private RequestUpdateFakeWalls m_requestUpdateFakeWalls = null;
        private RequestManualReport m_requestManualReport = null;
        private RequestClearManualReport m_requestClearManualReport = null;
        private bool? m_requestNewCCTVList = null;
        private bool? m_requestTodayAlarmData = null;
        private bool? m_requestGetSiteID = null;
        private bool? m_requestGetSpreadMessage = null;
        private RequestSetSpreadMessage m_requestSetSpreadMessage = null;
        private bool? m_requestMaterials = null;

        public RequestBuildingGroupList RequestBuildingGroupList
        {
            get { return m_requestBuildingGroupList; }
            set { m_requestBuildingGroupList = value; }
        }

        public RequestGltfDataList RequestGltfDataList
        {
            get { return m_requestGltfDataList; }
            set { m_requestGltfDataList = value; }
        }

        public RequestSaveViewport RequestSaveViewport
        {
            get { return m_requestSaveViewport; }
            set { m_requestSaveViewport = value; }
        }

        public RequestMoveBuildingNameText RequestMoveBuildingNameText
        {
            get { return m_requestMoveBuildingNameText; }
            set { m_requestMoveBuildingNameText = value; }
        }

        public RequestMoveEquipZoneNameText RequestMoveEquipZoneNameText
        {
            get { return m_requestMoveEquipZoneNameText; }
            set { m_requestMoveEquipZoneNameText = value; }
        }

        public RequestSensorList RequestSensorList
        {
            get { return m_requestSensorList; }
            set { m_requestSensorList = value; }
        }

        public RequestMoveSensor RequestMoveSensor
        {
            get { return m_requestMoveSensor; }
            set { m_requestMoveSensor = value; }
        }

        public RequestMalfunction RequestMalfunction
        {
            get { return m_requestMalfunction; }
            set { m_requestMalfunction = value; }
        }

        public RequestSituationNotice RequestSituationNotice
        {
            get { return m_requestSituationNotice; }
            set { m_requestSituationNotice = value; }
        }

        public RequestEquipZoneCCTV RequestEquipZoneCCTV
        {
            get { return m_requestEquipZoneCCTV; }
            set { m_requestEquipZoneCCTV = value; }
        }

        public RequestEquipZoneCCTVFromSensor RequestEquipZoneCCTVFromSensor
        {
            get { return m_requestEquipZoneCCTVFromSensor; }
            set { m_requestEquipZoneCCTVFromSensor = value; }
        }

        public RequestEquipZoneSensorList RequestEquipZoneSensorList
        {
            get { return m_requestEquipZoneSensorList; }
            set { m_requestEquipZoneSensorList = value; }
        }

        public RequestUpdateEquipZoneCCTVs RequestUpdateEquipZoneCCTVs
        {
            get { return m_requestUpdateEquipZoneCCTVs; }
            set { m_requestUpdateEquipZoneCCTVs = value; }
        }

        public RequestGetOrgSensorID RequestGetOrgSensorID
        {
            get { return m_requestGetOrgSensorID; }
            set { m_requestGetOrgSensorID = value; }
        }

        public bool? RequestSensorCount
        {
            get { return m_requestSensorCount; }
            set { m_requestSensorCount = value; }
        }

        public bool? RequestStreamServerURL
        {
            get { return m_requestStreamServerURL; }
            set { m_requestStreamServerURL = value; }
        }

        public RequestFacilityType RequestFacilityType
        {
            get { return m_requestFacilityType; }
            set { m_requestFacilityType = value; }
        }

        public bool? RequestAllFacilityInfo
        {
            get { return m_requestAllFacilityInfo; }
            set { m_requestAllFacilityInfo = value; }
        }

        public RequestUpdatePOIPosition RequestUpdatePOIPosition
        {
            get { return m_requestUpdatePOIPosition; }
            set { m_requestUpdatePOIPosition = value; }
        }

        public RequestUpdatePOIPositions RequestUpdatePOIPositions
        {
            get { return m_requestUpdatePOIPositions; }
            set { m_requestUpdatePOIPositions = value; }
        }

        public RequestUpdateCCTVs RequestUpdateCCTVs
        {
            get { return m_requestUpdateCCTVs; }
            set { m_requestUpdateCCTVs = value; }
        }

        public RequestFacilityInfoData RequestFacilityInfoData
        {
            get { return m_requestFacilityInfoData; }
            set { m_requestFacilityInfoData = value; }
        }

        public RequestBuildingData RequestBuildingData
        {
            get { return m_requestBuildingData; }
            set { m_requestBuildingData = value; }
        }

        public RequestBuildingGroupData RequestBuildingGroupData
        {
            get { return m_requestBuildingGroupData; }
            set { m_requestBuildingGroupData = value; }
        }

        public bool? RequestOuterDatas
        {
            get { return m_requestOuterDatas; }
            set { m_requestOuterDatas = value; }
        }

        public RequestIndoorData RequestIndoorDatas
        {
            get { return m_requestIndoorDatas; }
            set { m_requestIndoorDatas = value; }
        }

        public RequestSaveIndoorModelViewport RequestSaveIndoorModelViewport
        {
            get { return m_requestSaveIndoorModelViewport; }
            set { m_requestSaveIndoorModelViewport = value; }
        }

        public RequestSaveOrthoModelViewport RequestSaveOrthoModelViewport
        {
            get { return m_requestSaveOrthoModelViewport; }
            set { m_requestSaveOrthoModelViewport = value; }
        }

        public RequestFakeWalls RequestFakeWalls
        {
            get { return m_requestFakeWalls; }
            set { m_requestFakeWalls = value; }
        }

        public RequestUpdateFakeWall RequestUpdateFakeWall
        {
            get { return m_requestUpdateFakeWall; }
            set { m_requestUpdateFakeWall = value; }
        }

        public RequestUpdateFakeWalls RequestUpdateFakeWalls
        {
            get { return m_requestUpdateFakeWalls; }
            set { m_requestUpdateFakeWalls = value; }
        }

        public RequestManualReport RequestManualReport
        {
            get { return m_requestManualReport; }
            set { m_requestManualReport = value; }
        }

        public RequestClearManualReport RequestClearManualReport
        {
            get { return m_requestClearManualReport; }
            set { m_requestClearManualReport = value; }
        }

        public bool? RequestNewCCTVList
        {
            get { return m_requestNewCCTVList; }
            set { m_requestNewCCTVList = value; }
        }

        public bool? RequestTodayAlarmData
        {
            get { return m_requestTodayAlarmData; }
            set { m_requestTodayAlarmData = value; }
        }

        public bool? RequestGetSiteID
        {
            get { return m_requestGetSiteID; }
            set { m_requestGetSiteID = value; }
        }

        public bool? RequestGetSpreadMessage
        {
            get { return m_requestGetSpreadMessage; }
            set { m_requestGetSpreadMessage = value; }
        }

        public RequestSetSpreadMessage RequestSetSpreadMessage
        {
            get { return m_requestSetSpreadMessage; }
            set { m_requestSetSpreadMessage = value; }
        }

        public bool? RequestMaterials
        {
            get { return m_requestMaterials; }
            set { m_requestMaterials = value; }
        }
    }

    public class RequestIndoorData
    {
        private int m_nZoneID = -1;

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }
    }

    public class RequestFacilityInfoData
    {
        private string m_strModelName = "";

        public string ModelName
        {
            get { return m_strModelName; }
            set { m_strModelName = value; }
        }
    }

    public class RequestBuildingData
    {
        private string m_strBuildingName = "";

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }
    }

    public class RequestBuildingGroupData
    {
        private int m_nBuildingGroupID = -1;

        public int BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }
    }

    public class RequestMoveSensor
    {
        private string m_strSensorType = "";
        private int m_nSensorID = -1;
        private float x = 0;
        private float z = 0;

        public string SensorType
        {
            get { return m_strSensorType; }
            set { m_strSensorType = value; }
        }

        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }
    }

    public class RequestSensorList
    {
        private bool m_requestFireSensors = false;
        private bool m_requestPSMSensors = false;
        private bool m_requestEtcSensors = false;
        private bool m_requestCCTVs = false;

        public bool RequestFireSensors
        {
            get { return m_requestFireSensors; }
            set { m_requestFireSensors = value; }
        }

        public bool RequestPSMSensors
        {
            get { return m_requestPSMSensors; }
            set { m_requestPSMSensors = value; }
        }

        public bool RequestEtcSensors
        {
            get { return m_requestEtcSensors; }
            set { m_requestEtcSensors = value; }
        }

        public bool RequestCCTVs
        {
            get { return m_requestCCTVs; }
            set { m_requestCCTVs = value; }
        }
    }

    public class RequestMoveBuildingNameText
    {
        private string m_strBuildingGroupName = "";
        private string m_strBuildingName = "";
        private float x = 0;
        private float y = 0;
        private float z = 0;

        public string BuildingGroupName
        {
            get { return m_strBuildingGroupName; }
            set { m_strBuildingGroupName = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }
    }

    public class RequestMoveEquipZoneNameText
    {
        private int m_nEquipZoneID = 0;
        private string m_strDisplayText = "";
        private float x = 0;
        private float y = 0;
        private float z = 0;

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }
    }

    public class RequestSaveViewport
    {
        private string m_strModelName = "";
        private string m_strModelFile = "";
        private string m_strModelDisplayText = null;
        private Vertex3D m_vCameraPosition = null;
        private Quaternion m_qCameraQuaternion = null;
        private Vertex3D m_vCameraRotation = null;
        private Vertex3D m_vOrbitTarget = null;
        private int m_nCameraFov = 0;
        private float m_fCameraFar = 0;
        private float m_fCameraNear = 0;
        private int? m_nFloorIndex = null;
        private int? m_nBuildingGroupID = null;
        private int? m_nBuildingID = null;
        private int? m_nZoneID = null;

        public string ModelName
        {
            get { return m_strModelName; }
            set { m_strModelName = value; }
        }

        public string ModelFile
        {
            get { return m_strModelFile; }
            set { m_strModelFile = value; }
        }

        public string ModelDisplayText
        {
            get { return m_strModelDisplayText; }
            set { m_strModelDisplayText = value; }
        }

        public Vertex3D CameraPosition
        {
            get { return m_vCameraPosition; }
            set { m_vCameraPosition = value; }
        }

        public Quaternion CameraQuaternion
        {
            get { return m_qCameraQuaternion; }
            set { m_qCameraQuaternion = value; }
        }

        public Vertex3D CameraRotation
        {
            get { return m_vCameraRotation; }
            set { m_vCameraRotation = value; }
        }

        public Vertex3D OrbitTarget
        {
            get { return m_vOrbitTarget; }
            set { m_vOrbitTarget = value; }
        }

        public int Fov
        {
            get { return m_nCameraFov; }
            set { m_nCameraFov = value; }
        }

        public float Near
        {
            get { return m_fCameraNear; }
            set { m_fCameraNear = value; }
        }

        public float Far
        {
            get { return m_fCameraFar; }
            set { m_fCameraFar = value; }
        }

        public int? FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public int? BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }

        public int? BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public int? ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }
    }

    public class RequestMalfunction
    {
        private int m_nSensorType = -1;
        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        private int m_nSensorZoneID = -1;
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        private int m_nAccessedUserID = -1;
        public int AccessedUserID
        {
            get { return m_nAccessedUserID; }
            set { m_nAccessedUserID = value; }
        }

        private bool m_isMalfunction = true;
        public bool IsMalfunction
        {
            get { return m_isMalfunction; }
            set { m_isMalfunction = value; }
        }
    }

    /// <summary>
    /// 상황 전파
    /// </summary>
    public class RequestSituationNotice
    {
        private int m_nSensorType = -1;
        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        private int m_nSensorZoneID = -1;
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }
    }

    public class RequestEquipZoneCCTV
    {
        private int m_nEquipZoneID = -1;

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }
    }

    public class RequestEquipZoneCCTVFromSensor
    {
        private string m_strSensorType = "";
        private int m_nSensorID = -1;

        public string SensorType
        {
            get { return m_strSensorType; }
            set { m_strSensorType = value; }
        }

        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }
    }

    // 같은 EquipZone 내에 존재하는 같은 SensorType의 Sensor들을 요청한다.
    public class RequestEquipZoneSensorList
    {
        private string m_strSensorType = "";
        private int m_nSensorID = -1;

        public string SensorType
        {
            get { return m_strSensorType; }
            set { m_strSensorType = value; }
        }

        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }
    }

    public class RequestUpdateEquipZoneCCTVs
    {
        private List<Model.CCTV.EquipZoneCCTV> m_equipZoneCCTVs = new List<Model.CCTV.EquipZoneCCTV>();

        public List<Model.CCTV.EquipZoneCCTV> EquipZoneCCTVs
        {
            get { return m_equipZoneCCTVs; }
            set { m_equipZoneCCTVs = value; }
        }
    }

    public class RequestGetOrgSensorID
    {
        private int m_nSensorZoneID = -1;

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }
    }

    public class RequestFacilityType
    {
        private int m_nFacilityTypeID = -1;

        public int FacilityTypeID
        {
            get { return m_nFacilityTypeID; }
            set { m_nFacilityTypeID = value; }
        }
    }

    public class RequestUpdatePOIPosition
    {
        private int m_nUserID = -1;
        private string m_strSensorType = "";
        private int m_nSensorID = -1;
        private int m_nZoneID = -1;
        private Vertex3D m_vPos = null;
        private string m_strText = null;

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }
        
        public string SensorType
        {
            get { return m_strSensorType; }
            set { m_strSensorType = value; }
        }

        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public Vertex3D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }
    }

    public class RequestUpdatePOIPositions
    {
        private List<RequestUpdatePOIPosition> m_datas = new List<RequestUpdatePOIPosition>();

        public List<RequestUpdatePOIPosition> Datas
        {
            get { return m_datas; }
            set { m_datas = value; }
        }
    }

    public class UpdateCCTV
    {
        private int m_nID = -1;
        private int? m_nZoneID = -1;
        private float? x = 0;
        private float? y = 0;
        private float? z = 0;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int? ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public float? X
        {
            get { return x; }
            set { x = value; }
        }

        public float? Y
        {
            get { return y; }
            set { y = value; }
        }

        public float? Z
        {
            get { return z; }
            set { z = value; }
        }
    }

    public class RequestUpdateCCTVs
    {
        private int m_nUserID = -1;
        private List<UpdateCCTV> m_updateCCTVs = new List<UpdateCCTV>();

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public List<UpdateCCTV> UpdateCCTVs
        {
            get { return m_updateCCTVs; }
            set { m_updateCCTVs = value; }
        }
    }

    public class RequestSaveIndoorModelViewport
    {
        private string m_strModelName = "";
        private Vertex3D m_vCameraPos = null;
        private Quaternion m_qCameraQuaternion = null;
        private Vertex3D m_vCameraRotation = null;
        private Vertex3D m_vOrbitTarget = null;

        public string ModelName
        {
            get { return m_strModelName; }
            set { m_strModelName = value; }
        }

        public Vertex3D CameraPosition
        {
            get { return m_vCameraPos; }
            set { m_vCameraPos = value; }
        }

        public Quaternion CameraQuaternion
        {
            get { return m_qCameraQuaternion; }
            set { m_qCameraQuaternion = value; }
        }

        public Vertex3D CameraRotation
        {
            get { return m_vCameraRotation; }
            set { m_vCameraRotation = value; }
        }

        public Vertex3D OrbitTarget
        {
            get { return m_vOrbitTarget; }
            set { m_vOrbitTarget = value; }
        }
    }

    public class RequestSaveOrthoModelViewport
    {
        private string m_strModelName = "";
        private Vertex3D m_vCameraPos = null;
        private Quaternion m_qCameraQuaternion = null;
        private Vertex3D m_vCameraRotation = null;
        private Vertex3D m_vTarget = null;
        private float m_fZoom = 1.0f;
        private int? m_nZoneID = null;

        public string ModelName
        {
            get { return m_strModelName; }
            set { m_strModelName = value; }
        }

        public Vertex3D CameraPosition
        {
            get { return m_vCameraPos; }
            set { m_vCameraPos = value; }
        }

        public Quaternion CameraQuaternion
        {
            get { return m_qCameraQuaternion; }
            set { m_qCameraQuaternion = value; }
        }

        public Vertex3D CameraRotation
        {
            get { return m_vCameraRotation; }
            set { m_vCameraRotation = value; }
        }

        public Vertex3D Target
        {
            get { return m_vTarget; }
            set { m_vTarget = value; }
        }

        public float Zoom
        {
            get { return m_fZoom; }
            set { m_fZoom = value; }
        }

        public int? ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }
    }

    public class RequestFakeWalls
    {
        private int m_nZoneID = -1;

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }
    }

    public class RequestUpdateFakeWall
    {
        public enum UpdateMode { None = 0, Add, Move, Rotate, Resize, Delete };

        private int m_nUserID = -1;
        private int m_nFakeWallID = -1;
        private int m_nZoneID = -1;
        private float x = 0;
        private float y = 0;
        private float z = 0;
        private float m_fRotate = 0;
        private float m_fScale = 0;
        private int m_nMode = (int)UpdateMode.None;

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public int FakeWallID
        {
            get { return m_nFakeWallID; }
            set { m_nFakeWallID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        // Radian
        public float Rotate
        {
            get { return m_fRotate; }
            set { m_fRotate = value; }
        }

        public float Scale
        {
            get { return m_fScale; }
            set { m_fScale = value; }
        }

        // UpdateMode
        public int Mode
        {
            get { return m_nMode; }
            set { m_nMode = value; }
        }
    }

    public class RequestManualReport
    {
        private string m_strDateTime = "";
        private int m_nSensorType = -1;
        private int m_nSensorZoneID = -1;
        private int m_nZoneID = -1;
        private int m_nAlarmDepth = -1;
        private string m_strReportPerson = "";
        private string m_strMemo = "";

        public string DateTime
        {
            get { return m_strDateTime; }
            set { m_strDateTime = value; }
        }

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int AlarmDepth
        {
            get { return m_nAlarmDepth; }
            set { m_nAlarmDepth = value; }
        }

        public string ReportPerson
        {
            get { return m_strReportPerson; }
            set { m_strReportPerson = value; }
        }

        public string Memo
        {
            get { return m_strMemo; }
            set { m_strMemo = value; }
        }
    }

    public class RequestClearManualReport
    {        
        private int m_nSensorType = -1;
        private int m_nSensorZoneID = -1;
        private int m_nSensorZoneHistoryID = -1;
        private int m_nAccessedUserID = -1;

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public int AccessedUserID
        {
            get { return m_nAccessedUserID; }
            set { m_nAccessedUserID = value; }
        }
    }

    public class RequestUpdateFakeWalls
    {
        private List<RequestUpdateFakeWall> m_updateDatas = new List<RequestUpdateFakeWall>();

        public List<RequestUpdateFakeWall> UpdateDatas
        {
            get { return m_updateDatas; }
            set { m_updateDatas = value; }
        }
    }

    public class RequestSetSpreadMessage
    {
        private List<SDMS.Model.Config.SpreadMessage> m_addSpreadMessage = null;
        private List<SDMS.Model.Config.SpreadMessage> m_updateSpreadMessage = null;
        private List<SDMS.Model.Config.SpreadMessage> m_removeSpreadMessage = null;

        public List<SDMS.Model.Config.SpreadMessage> AddSpreadMessage
        {
            get { return m_addSpreadMessage; }
            set { m_addSpreadMessage = value; }
        }

        public List<SDMS.Model.Config.SpreadMessage> UpdateSpreadMessage
        {
            get { return m_updateSpreadMessage; }
            set { m_updateSpreadMessage = value; }
        }

        public List<SDMS.Model.Config.SpreadMessage> RemoveSpreadMessage
        {
            get { return m_removeSpreadMessage; }
            set { m_removeSpreadMessage = value; }
        }
    }

    public class RequestGltfDataList
    {
        private int m_nUserID = -1;
        private List<int> m_siteIDs = null;

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public List<int> SiteIDs
        {
            get { return m_siteIDs; }
            set { m_siteIDs = value; }
        }
    }

    public class RequestBuildingGroupList
    {
        private List<int> m_siteIDs = null;

        public List<int> SiteIDs
        {
            get { return m_siteIDs; }
            set { m_siteIDs = value; }
        }
    }
}
