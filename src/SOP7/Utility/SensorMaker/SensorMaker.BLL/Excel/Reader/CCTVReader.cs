using System.Collections.Generic;

namespace SensorMaker.BLL.Excel.Reader
{
    using Models.Data.Sensor;

    public class CCTVReader : ExcelReader
    {
        public const string ID_Name = "ID";
        public const string BuildingID_Name = "BuildingID (생략가능)";
        public const string ZoneID_Name = "ZoneID (생략가능)";
        public const string EquipZoneID_Name = "EquipZoneID (생략가능)";
        public const string EquipZoneIDs_Name = "EquipZoneIDs (생략가능, 쉼표로 구분)";
        public const string UniqueKey_Name = "고유키";
        public const string Location_Name = "구역명(생략가능)";
        public const string CameraName_Name = "카메라 이름";
        public const string ConnectionType_Name = "연결방식(생략가능)";
        public const string Channel_Name = "채널(생략가능)";
        public const string UserID_Name = "UserID(생략가능)";
        public const string Password_Name = "Password(생략가능)";
        public const string URL_Name = "URL";

        public const int ID_Index = 0;
        public const int BuildingID_Index = 1;
        public const int ZoneID_Index = 2;
        public const int EquipZoneID_Index = 3;
        public const int EquipZoneIDs_Index = 4;
        public const int UniqueKey_Index = 5;
        public const int Location_Index = 6;
        public const int CameraName_Index = 7;
        public const int ConnectionType_Index = 8;
        public const int Channel_Index = 9;
        public const int UserID_Index = 10;
        public const int Password_Index = 11;
        public const int URL_Index = 12;

        private const int ColumnCount = 13;

        private List<CCTVSensor> m_sensors = null;

        public override object Result
        {
            get { return m_sensors; }
        }

        public CCTVReader(string strFilePath)
            : base(strFilePath)
        {
        }

        protected override bool UpdateData(List<SheetData> sheetDatas, out string strErrorMessage)
        {
            if (sheetDatas.Count == 0)
            {
                strErrorMessage = "비어있는 엑셀파일입니다.";
                return false;
            }

            int nRowCount = -1;
            SheetData firstSheet = sheetDatas[0];

            if (CheckDataFile(firstSheet, ColumnCount, out nRowCount, out strErrorMessage) == false)
                return false;

            Dictionary<string, string> dicUniqueKeys = new Dictionary<string, string>();
            List<CCTVSensor> sensors = new List<CCTVSensor>();

            for (int i = 0; i < nRowCount; i++)
            {
                int id = 0;
                string uniqueKey = "", locationName = null, cameraName = "", connectionType = "", userID = null, password = null, url = "";
                int? buildingID = null, zoneID = null, equipZoneID = null, channel = null;
                string strEquipZoneIDs = null;

                if (GetNotNullInt(firstSheet.ColumnDatas[ID_Index][i], i, ID_Index, out id, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[BuildingID_Index][i], i, BuildingID_Index, out buildingID, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[ZoneID_Index][i], i, ZoneID_Index, out zoneID, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[EquipZoneID_Index][i], i, EquipZoneID_Index, out equipZoneID, out strErrorMessage) == false)
                    return false;

                if (GetNullableString(firstSheet.ColumnDatas[EquipZoneIDs_Index][i], i, EquipZoneIDs_Index, out strEquipZoneIDs, out strErrorMessage) == false)
                    return false;

                if (GetNotNullString(firstSheet.ColumnDatas[UniqueKey_Index][i], i, UniqueKey_Index, out uniqueKey, out strErrorMessage) == false)
                    return false;

                if (GetNullableString(firstSheet.ColumnDatas[Location_Index][i], i, Location_Index, out locationName, out strErrorMessage) == false)
                    return false;

                if (GetNotNullString(firstSheet.ColumnDatas[CameraName_Index][i], i, CameraName_Index, out cameraName, out strErrorMessage) == false)
                    return false;

                if (GetEmptyString(firstSheet.ColumnDatas[ConnectionType_Index][i], i, ConnectionType_Index, out connectionType, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[Channel_Index][i], i, Channel_Index, out channel, out strErrorMessage) == false)
                    return false;

                if (GetNullableString(firstSheet.ColumnDatas[UserID_Index][i], i, UserID_Index, out userID, out strErrorMessage) == false)
                    return false;

                if (GetNullableString(firstSheet.ColumnDatas[Password_Index][i], i, Password_Index, out password, out strErrorMessage) == false)
                    return false;

                if (GetEmptyString(firstSheet.ColumnDatas[URL_Index][i], i, URL_Index, out url, out strErrorMessage) == false)
                    return false;

                if (dicUniqueKeys.ContainsKey(uniqueKey))
                {
                    strErrorMessage = string.Format("{0}번째 행에 이전값과 동일한 센서 키값이 존재합니다.({1})", i + 1, uniqueKey);
                    return false;
                }
                else
                    dicUniqueKeys[uniqueKey] = uniqueKey;

                CCTVSensor sensor = new CCTVSensor();
                sensor.ID = id;
                sensor.BuildingID = buildingID;
                sensor.ZoneID = zoneID;
                sensor.EquipZoneID = equipZoneID;
                sensor.UniqueKey = uniqueKey;
                sensor.PositionName = locationName;
                sensor.CameraName = cameraName;
                sensor.Type = connectionType;
                sensor.Channel = channel;
                sensor.UserID = userID;
                sensor.Password = password;
                sensor.URL = url;
                StringToIntList(strEquipZoneIDs, sensor.EquipZoneIDs);

                sensors.Add(sensor);
            }

            m_sensors = sensors;
            return true;
        }
    }
}
