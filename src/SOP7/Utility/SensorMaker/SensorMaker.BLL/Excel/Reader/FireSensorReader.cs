using System.Collections.Generic;

namespace SensorMaker.BLL.Excel.Reader
{
    using Models.Data.Sensor;

    public class FireSensorReader : ExcelReader
    {
        public const string ID_Name = "ID";
        public const string BuildingID_Name = "BuildingID (생략가능)";
        public const string ZoneID_Name = "ZoneID (생략가능)";
        public const string EquipZoneID_Name = "EquipZoneID (생략가능)";
        public const string ReceiverID_Name = "수신반 ID(생략가능)";
        public const string LoopID_Name = "회로 ID(생략가능)";
        public const string RelayID_Name = "중계기 ID(생략가능)";
        public const string TagID_Name = "Tag ID";
        public const string Location_Name = "구역명(생략가능)";
        public const string SensorName_Name = "센서이름";
        public const string SensorType_Name = "감지기 타입(일반 : 생략, 열 : 0, 연기 : 1, 불꽃 : 2)";

        public const int ID_Index = 0;
        public const int BuildingID_Index = 1;
        public const int ZoneID_Index = 2;
        public const int EquipZoneID_Index = 3;
        public const int ReceiverID_Index = 4;
        public const int LoopID_Index = 5;
        public const int RelayID_Index = 6;
        public const int TagID_Index = 7;
        public const int Location_Index = 8;
        public const int SensorName_Index = 9;
        public const int SensorType_Index = 10;

        private const int ColumnCount = 11;

        private List<FireSensor> m_sensors = null;

        public override object Result
        {
            get { return m_sensors; }
        }

        public FireSensorReader(string strFilePath)
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

            Dictionary<int, int> dicSensorIDs = new Dictionary<int, int>();
            List<FireSensor> sensors = new List<FireSensor>();

            for (int i=0;i<nRowCount;i++)
            {
                int? receiverID = null, loopID = null, relayID = null, buildingID = null, zoneID = null, equipZoneID = null;
                int id = 0, tagID = 0;
                string locationName = null, sensorName = "";
                int? sensorType = null;

                if (GetNotNullInt(firstSheet.ColumnDatas[ID_Index][i], i, ID_Index, out id, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[BuildingID_Index][i], i, BuildingID_Index, out buildingID, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[ZoneID_Index][i], i, ZoneID_Index, out zoneID, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[EquipZoneID_Index][i], i, EquipZoneID_Index, out equipZoneID, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[ReceiverID_Index][i], i, ReceiverID_Index, out receiverID, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[LoopID_Index][i], i, LoopID_Index, out loopID, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[RelayID_Index][i], i, RelayID_Index, out relayID, out strErrorMessage) == false)
                    return false;

                if (GetNotNullInt(firstSheet.ColumnDatas[TagID_Index][i], i, TagID_Index, out tagID, out strErrorMessage) == false)
                    return false;

                if (GetNullableString(firstSheet.ColumnDatas[Location_Index][i], i, Location_Index, out locationName, out strErrorMessage) == false)
                    return false;

                if (GetNotNullString(firstSheet.ColumnDatas[SensorName_Index][i], i, SensorName_Index, out sensorName, out strErrorMessage) == false)
                    return false;

                if (GetNullableInt(firstSheet.ColumnDatas[SensorType_Index][i], i, SensorType_Index, out sensorType, out strErrorMessage) == false)
                    return false;

                string strSensorID;
                int nSensorID = MakeSensorID(receiverID, loopID, relayID, tagID, out strSensorID);

                if (dicSensorIDs.ContainsKey(nSensorID))
                {
                    strErrorMessage = string.Format("{0}번째 행에 이전값과 동일한 센서 키값이 존재합니다.({1})", i + 1, strSensorID);
                    return false;
                }
                else
                    dicSensorIDs[nSensorID] = nSensorID;

                FireSensor fire = new FireSensor();
                fire.ID = id;
                fire.TagNo = nSensorID;
                fire.PositionName = locationName;
                fire.Name = sensorName;
                fire.SensorSubType = sensorType;                
                fire.BuildingID = buildingID;
                fire.ZoneID = zoneID == null ? -1 : (int)zoneID;
                fire.EquipZoneID = equipZoneID;

                sensors.Add(fire);
            }

            m_sensors = sensors;
            return true;
        }

        private int MakeSensorID(int? receiverID, int? loopID, int? relayID, int tagID, out string strSensorID)
        {
            int nReceiverID = receiverID == null ? 0 : (int)receiverID;
            int nLoopID = loopID == null ? 0 : (int)loopID;
            int nRelayID = relayID == null ? 0 : (int)relayID;

            strSensorID = receiverID == null ? "null " : ((int)receiverID).ToString() + " ";
            strSensorID += loopID == null ? "null " : ((int)loopID).ToString() + " ";
            strSensorID += relayID == null ? "null " : ((int)relayID).ToString() + " ";
            strSensorID += tagID.ToString();

            return nReceiverID * 10000000 + nLoopID * 100000 + nRelayID * 100 + tagID;
        }

        public static void ParseSensorID(int tagNo, out int? receiverID, out int? loopID, out int? relayID, out int tagID)
        {
            int nReceiverID = tagNo / 10000000;
            int nLoopID = (tagNo % 10000000) / 100000;
            int nRelayID = (tagNo % 100000) / 100;
            tagID = tagNo % 100;

            if (nReceiverID == 0)
                receiverID = null;
            else
                receiverID = nReceiverID;

            if (nLoopID == 0)
                loopID = null;
            else
                loopID = nLoopID;

            if (nRelayID == 0)
                relayID = null;
            else
                relayID = nRelayID;
        }
    }
}
