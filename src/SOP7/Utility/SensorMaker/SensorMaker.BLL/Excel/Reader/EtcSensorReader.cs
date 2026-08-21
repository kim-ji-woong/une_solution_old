using System.Collections.Generic;

namespace SensorMaker.BLL.Excel.Reader
{
    using Models.Data.Sensor;

    public class EtcSensorReader : ExcelReader
    {
        public const string ID_Name = "ID";
        public const string BuildingID_Name = "BuildingID (생략가능)";
        public const string ZoneID_Name = "ZoneID (생략가능)";
        public const string EquipZoneID_Name = "EquipZoneID (생략가능)";
        public const string UniqueKey_Name = "고유키";
        public const string Location_Name = "구역명(생략가능)";
        public const string SensorName_Name = "센서이름";
        public const string SensorType_Name = "신호 타입";
        public const string NameOfUnit_Name = "단위(생략가능)";

        public const int ID_Index = 0;
        public const int BuildingID_Index = 1;
        public const int ZoneID_Index = 2;
        public const int EquipZoneID_Index = 3;
        public const int UniqueKey_Index = 4;
        public const int Location_Index = 5;
        public const int SensorName_Index = 6;
        public const int SensorType_Index = 7;
        public const int NameOfUnit_Index = 8;

        private const int ColumnCount = 9;

        private List<EtcSensor> m_sensors = null;

        public override object Result
        {
            get { return m_sensors; }
        }

        public EtcSensorReader(string strFilePath)
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
            List<EtcSensor> sensors = new List<EtcSensor>();

            for (int i = 0; i < nRowCount; i++)
            {
                int id = 0;
                int? buildingID = null, zoneID = null, equipZoneID = null;
                string uniqueKey = "", locationName = null, sensorName = "", sensorType = "", nameOfUnit = null;

                if (GetNotNullInt(firstSheet.ColumnDatas[ID_Index][i], i, ID_Index, out id, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[BuildingID_Index][i], i, BuildingID_Index, out buildingID, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[ZoneID_Index][i], i, ZoneID_Index, out zoneID, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[EquipZoneID_Index][i], i, EquipZoneID_Index, out equipZoneID, out strErrorMessage) == false)
                    return false;

                if (GetNotNullString(firstSheet.ColumnDatas[UniqueKey_Index][i], i, UniqueKey_Index, out uniqueKey, out strErrorMessage) == false)
                    return false;

                if (GetNullableString(firstSheet.ColumnDatas[Location_Index][i], i, Location_Index, out locationName, out strErrorMessage) == false)
                    return false;

                if (GetNotNullString(firstSheet.ColumnDatas[SensorName_Index][i], i, SensorName_Index, out sensorName, out strErrorMessage) == false)
                    return false;

                if (GetNotNullString(firstSheet.ColumnDatas[SensorType_Index][i], i, SensorType_Index, out sensorType, out strErrorMessage) == false)
                    return false;

                if (GetNullableString(firstSheet.ColumnDatas[NameOfUnit_Index][i], i, NameOfUnit_Index, out nameOfUnit, out strErrorMessage) == false)
                    return false;

                if (dicUniqueKeys.ContainsKey(uniqueKey))
                {
                    strErrorMessage = string.Format("{0}번째 행에 이전값과 동일한 센서 키값이 존재합니다.({1})", i + 1, uniqueKey);
                    return false;
                }
                else
                    dicUniqueKeys[uniqueKey] = uniqueKey;

                EtcSensor sensor = new EtcSensor();
                sensor.ID = id;
                sensor.BuildingID = buildingID;
                sensor.ZoneID = zoneID == null ? -1 : (int)zoneID;
                sensor.EquipZoneID = equipZoneID;
                sensor.UniqueKey = uniqueKey;
                sensor.PositionName = locationName;
                sensor.Name = sensorName;
                sensor.MaterialName = sensorType;
                sensor.UnitName = nameOfUnit;

                sensors.Add(sensor);
            }

            m_sensors = sensors;
            return true;
        }
    }
}
