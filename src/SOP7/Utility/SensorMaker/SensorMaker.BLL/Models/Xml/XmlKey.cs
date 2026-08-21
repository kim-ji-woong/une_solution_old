using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL.Models.Xml
{
    public class XmlKey
    {
        public const string XName_Spaces = "Spaces";
        public const string XName_Sensors = "Sensors";
        public const string XName_SensorTypes = "SensorTypes";
        public const string XName_SensorType = "SensorType";
        public const string XName_FireSensorSubTypes = "FireSensorSubTypes";
        public const string XName_FireSensorSubType = "FireSensorSubType";
        public const string XName_BuildingGroups = "BuildingGroups";
        public const string XName_BuildingGroup = "BuildingGroup";
        public const string XName_Buildings = "Buildings";
        public const string XName_Building = "Building";
        public const string XName_Zones= "Zones";
        public const string XName_Zone = "Zone";
        public const string XName_EquipmentZones = "EquipmentZones";
        public const string XName_EquipmentZone = "EquipmentZone";
        public const string XName_BuildingGroupID = "BuildingGroupID";
        public const string XName_BuildingID = "BuildingID";
        public const string XName_ZoneID = "ZoneID";
        public const string XName_EquipZoneID = "EquipZoneID";
        public const string XName_EquipZoneIDs = "EquipZoneIDs";
        public const string XName_FireSensors = "FireSensors";
        public const string XName_FireSensor = "FireSensor";
        public const string XName_PsmSensors = "PSMSensors";
        public const string XName_EtcSensors = "EtcSensors";
        public const string XName_CCTVs= "CCTVs";
        public const string XName_Fire = "Fire";
        public const string XName_Psm = "PSM";
        public const string XName_Etc = "Etc";
        public const string XName_CCTV = "CCTV";
        public const string XName_Name = "Name";
        public const string XName_Code = "Code";
        public const string XName_Type = "Type";
        public const string XName_UserID = "UserID";
        public const string XName_Password = "Password";
        public const string XName_Url = "URL";
        public const string XName_DisplayText = "DisplayText";
        public const string XName_BroadcastText = "BroadcastText";
        public const string XName_PositionName = "PositionName";
        public const string XName_Position = "Position";
        public const string XName_TextCenter = "TextCenter";
        public const string XName_Boundary = "Boundary";
        public const string XName_LinkedZoneIDList = "LinkedZoneIDList";
        public const string XName_Point3D = "Point3D";
        public const string XName_Point2D = "Point2D";
        public const string XName_Polygon = "Polygon";
        public const string XName_SensorSubType = "SensorSubType";
        public const string XName_TagNo = "TagNo";
        public const string XName_MaterialType = "MaterialType";
        public const string XName_UniqueKey = "UniqueKey";
        public const string XName_UnitName = "UnitName";
        public const string XName_Gltf = "Gltf";
        public const string XName_GltfOption = "GltfOption";
        public const string XName_ModelBaseURL = "ModelBaseURL";
        public const string XName_TextureBaseURL = "TextureBaseURL";
        public const string XName_IndoorModelOnMemory = "IndoorModelOnMemory";
        public const string XName_BackgroundImage = "BackgroundImage";
        public const string XName_GltfModels = "GltfModels";
        public const string XName_GltfModel = "GltfModel";
        public const string XName_ID = "ID";
        public const string XName_ModelName = "ModelName";
        public const string XName_ParentID = "ParentID";
        public const string XName_Site = "Site";
        public const string XName_SiteID = "SiteID";
        public const string XName_Visible = "Visible";
        public const string XName_ChildModels = "ChildModels";
        public const string XName_ChildModel = "ChildModel";
        public const string XName_ModelDatas = "ModelDatas";
        public const string XName_ModelData = "ModelData";
        public const string XName_ModelOrthoDatas = "ModelOrthoDatas";
        public const string XName_ModelOrthoData = "ModelOrthoData";
        public const string XName_ModelID = "ModelID";
        public const string XName_ModelDisplayText = "ModelDisplayText";
        public const string XName_ModelFile = "ModelFile";
        public const string XName_FloorIndex = "FloorIndex";
        public const string XName_MaxFloor = "MaxFloor";
        public const string XName_MinFloor = "MinFloor";
        public const string XName_CameraFar = "CameraFar";
        public const string XName_CameraFov = "CameraFov";
        public const string XName_CameraNear = "CameraNear";
        public const string XName_CameraPosition = "CameraPosition";
        public const string XName_CameraQuaternion = "CameraQuaternion";
        public const string XName_CameraRotation = "CameraRotation";
        public const string XName_OrbitTarget = "OrbitTarget";
        public const string XName_Target = "Target";
        public const string XName_Zoom = "Zoom";

        public enum KeyValue
        {
            BuildingGroup,
            Building,
            Zone,
            EquipmentZone,
            SensorType,
            FireSensorSubType,
            Material,
            FireSensor,
            PsmSensor,
            EtcSensor,
            Cctv
        }

        public static string GetKeyValueSting(KeyValue keyValue)
        {
            switch (keyValue)
            {
                case KeyValue.BuildingGroup: return "BG_";
                case KeyValue.Building: return "B_";
                case KeyValue.Zone: return "Z_";
                case KeyValue.EquipmentZone: return "EZ_";
                case KeyValue.SensorType: return "ST_";
                case KeyValue.FireSensorSubType: return "FST_";
                case KeyValue.Material: return "M_";
                case KeyValue.FireSensor: return "FS_";
                case KeyValue.PsmSensor: return "PS_";
                case KeyValue.EtcSensor: return "ES_";
                case KeyValue.Cctv: return "CC_";
            }

            return "";
        }
    }
}
