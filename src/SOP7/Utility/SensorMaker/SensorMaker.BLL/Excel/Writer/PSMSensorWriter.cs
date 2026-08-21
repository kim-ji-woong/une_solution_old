using System.Collections.Generic;

namespace SensorMaker.BLL.Excel.Writer
{
    using Reader;
    using Models.Data.Sensor;

    public class PSMSensorWriter : ExcelWriter
    {
        private List<PSMSensor> m_sensors = null;

        public PSMSensorWriter(List<PSMSensor> sensorList)
        {
            m_sensors = sensorList;
        }

        protected override string GetSubject()
        {
            return "누출센서";
        }

        protected override ICollection<SheetData> ReadSheetDatas(out string strErrorMessage)
        {
            strErrorMessage = null;

            SheetData sheetData = new SheetData(GetSubject());
            SetTitles(sheetData);

            foreach (PSMSensor sensor in m_sensors)
            {
                SetColumnDatas(sheetData, sensor);
            }

            List<SheetData> sheetDatas = new List<SheetData>();

            if (sheetData != null)
                sheetDatas.Add(sheetData);

            return sheetDatas;
        }

        private void SetColumnDatas(SheetData sheetData, PSMSensor sensor)
        {
            if (sensor.UniqueKey == null || sensor.Name == null || sensor.MaterialName == null)
                return;

            int tagNo;

            if (int.TryParse(sensor.UniqueKey.Trim(), out tagNo) == false)
                return;

            List<string> columnDatas;

            int? receiverID, loopID, relayID;
            int tagID;

            PSMSensorReader.ParseSensorID(tagNo, out receiverID, out loopID, out relayID, out tagID);

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
                //if (sensor.EquipZoneID == null)
                //    columnDatas.Add(null);
                //else
                    columnDatas.Add(sensor.EquipZoneID.ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(PSMSensorReader.ReceiverID_Index, out columnDatas))
            {
                if (receiverID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)receiverID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(PSMSensorReader.LoopID_Index, out columnDatas))
            {
                if (loopID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)loopID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(PSMSensorReader.RelayID_Index, out columnDatas))
            {
                if (relayID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)relayID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(PSMSensorReader.TagID_Index, out columnDatas))
            {
                columnDatas.Add(tagID.ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(PSMSensorReader.Location_Index, out columnDatas))
            {
                columnDatas.Add(sensor.PositionName);
            }

            if (sheetData.ColumnDatas.TryGetValue(PSMSensorReader.SensorName_Index, out columnDatas))
            {
                columnDatas.Add(sensor.Name);
            }

            if (sheetData.ColumnDatas.TryGetValue(PSMSensorReader.MaterialName_Index, out columnDatas))
            {
                columnDatas.Add(sensor.MaterialName);
            }

            if (sheetData.ColumnDatas.TryGetValue(PSMSensorReader.NameOfUnit_Index, out columnDatas))
            {
                columnDatas.Add(sensor.UnitName);
            }
        }

        private void SetTitles(SheetData sheetData)
        {
            sheetData.Titles[FireSensorReader.ID_Index] = FireSensorReader.ID_Name;
            sheetData.Titles[FireSensorReader.BuildingID_Index] = FireSensorReader.BuildingID_Name;
            sheetData.Titles[FireSensorReader.ZoneID_Index] = FireSensorReader.ZoneID_Name;
            sheetData.Titles[FireSensorReader.EquipZoneID_Index] = FireSensorReader.EquipZoneID_Name;
            sheetData.Titles[PSMSensorReader.ReceiverID_Index] = PSMSensorReader.ReceiverID_Name;
            sheetData.Titles[PSMSensorReader.LoopID_Index] = PSMSensorReader.LoopID_Name;
            sheetData.Titles[PSMSensorReader.RelayID_Index] = PSMSensorReader.RelayID_Name;
            sheetData.Titles[PSMSensorReader.TagID_Index] = PSMSensorReader.TagID_Name;
            sheetData.Titles[PSMSensorReader.Location_Index] = PSMSensorReader.Location_Name;
            sheetData.Titles[PSMSensorReader.SensorName_Index] = PSMSensorReader.SensorName_Name;
            sheetData.Titles[PSMSensorReader.MaterialName_Index] = PSMSensorReader.MaterialName_Name;
            sheetData.Titles[PSMSensorReader.NameOfUnit_Index] = PSMSensorReader.NameOfUnit_Name;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                sheetData.ColumnDatas[pair.Key] = new List<string>();
            }
        }
    }
}
