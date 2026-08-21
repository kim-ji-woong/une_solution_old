using System.Collections.Generic;

namespace SensorMaker.BLL.Excel.Writer
{
    using Reader;
    using Models.Data.Sensor;

    public class FireSensorWriter : ExcelWriter
    {
        private List<FireSensor> m_sensors = null;

        public FireSensorWriter(List<FireSensor> sensorList)
        {
            m_sensors = sensorList;
        }

        protected override string GetSubject()
        {
            return "화재센서";
        }

        protected override ICollection<SheetData> ReadSheetDatas(out string strErrorMessage)
        {
            strErrorMessage = null;

            SheetData sheetData = new SheetData(GetSubject());
            SetTitles(sheetData);

            foreach (FireSensor sensor in m_sensors)
            {
                SetColumnDatas(sheetData, sensor);
            }

            List<SheetData> sheetDatas = new List<SheetData>();

            if (sheetData != null)
                sheetDatas.Add(sheetData);

            return sheetDatas;
        }

        private void SetColumnDatas(SheetData sheetData, FireSensor sensor)
        {
            if (sensor.TagNo == null || sensor.Name == null)
                return;

            List<string> columnDatas;

            int? receiverID, loopID, relayID;
            int tagID;

            FireSensorReader.ParseSensorID((int)sensor.TagNo, out receiverID, out loopID, out relayID, out tagID);

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.ID_Index, out columnDatas))
            {
                //if (sensor.OrgSensorID == null)
                //    columnDatas.Add(null);
                //else
                    columnDatas.Add(sensor.ID.ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.BuildingID_Index, out columnDatas))
            {
                if (sensor.BuildingID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)sensor.BuildingID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.ZoneID_Index, out columnDatas))
            {
                if (sensor.ZoneID < 0)
                    columnDatas.Add(null);
                else
                    columnDatas.Add((sensor.ZoneID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.EquipZoneID_Index, out columnDatas))
            {
                if (sensor.EquipZoneID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)sensor.EquipZoneID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.ReceiverID_Index, out columnDatas))
            {
                if (receiverID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)receiverID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.LoopID_Index, out columnDatas))
            {
                if (loopID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)loopID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.RelayID_Index, out columnDatas))
            {
                if (relayID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)relayID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.TagID_Index, out columnDatas))
            {
                columnDatas.Add(tagID.ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.Location_Index, out columnDatas))
            {
                columnDatas.Add(sensor.PositionName);
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.SensorName_Index, out columnDatas))
            {
                columnDatas.Add(sensor.Name);
            }

            if (sheetData.ColumnDatas.TryGetValue(FireSensorReader.SensorType_Index, out columnDatas))
            {
                if (sensor.SensorSubType == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)sensor.SensorSubType).ToString());
            }
        }

        private void SetTitles(SheetData sheetData)
        {
            sheetData.Titles[FireSensorReader.ID_Index] = FireSensorReader.ID_Name;
            sheetData.Titles[FireSensorReader.BuildingID_Index] = FireSensorReader.BuildingID_Name;
            sheetData.Titles[FireSensorReader.ZoneID_Index] = FireSensorReader.ZoneID_Name;
            sheetData.Titles[FireSensorReader.EquipZoneID_Index] = FireSensorReader.EquipZoneID_Name;
            sheetData.Titles[FireSensorReader.ReceiverID_Index] = FireSensorReader.ReceiverID_Name;
            sheetData.Titles[FireSensorReader.LoopID_Index] = FireSensorReader.LoopID_Name;
            sheetData.Titles[FireSensorReader.RelayID_Index] = FireSensorReader.RelayID_Name;
            sheetData.Titles[FireSensorReader.TagID_Index] = FireSensorReader.TagID_Name;
            sheetData.Titles[FireSensorReader.Location_Index] = FireSensorReader.Location_Name;
            sheetData.Titles[FireSensorReader.SensorName_Index] = FireSensorReader.SensorName_Name;
            sheetData.Titles[FireSensorReader.SensorType_Index] = FireSensorReader.SensorType_Name;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                sheetData.ColumnDatas[pair.Key] = new List<string>();
            }
        }
    }
}
