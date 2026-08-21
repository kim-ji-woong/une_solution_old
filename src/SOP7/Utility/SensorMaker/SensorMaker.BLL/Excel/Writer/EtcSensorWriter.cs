using System.Collections.Generic;

namespace SensorMaker.BLL.Excel.Writer
{
    using Reader;
    using Models.Data.Sensor;

    public class EtcSensorWriter : ExcelWriter
    {
        private List<EtcSensor> m_sensors = null;

        public EtcSensorWriter(List<EtcSensor> sensorList)
        {
            m_sensors = sensorList;
        }

        protected override string GetSubject()
        {
            return "기타센서";
        }

        protected override ICollection<SheetData> ReadSheetDatas(out string strErrorMessage)
        {
            strErrorMessage = null;

            SheetData sheetData = new SheetData(GetSubject());
            SetTitles(sheetData);

            foreach (EtcSensor sensor in m_sensors)
            {
                SetColumnDatas(sheetData, sensor);
            }

            List<SheetData> sheetDatas = new List<SheetData>();

            if (sheetData != null)
                sheetDatas.Add(sheetData);

            return sheetDatas;
        }

        private void SetColumnDatas(SheetData sheetData, EtcSensor sensor)
        {
            if (sensor.UniqueKey == null || sensor.Name == null || sensor.MaterialName == null)
                return;

            List<string> columnDatas;

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

            if (sheetData.ColumnDatas.TryGetValue(EtcSensorReader.UniqueKey_Index, out columnDatas))
            {
                columnDatas.Add(sensor.UniqueKey);
            }

            if (sheetData.ColumnDatas.TryGetValue(EtcSensorReader.Location_Index, out columnDatas))
            {
                columnDatas.Add(sensor.PositionName);
            }

            if (sheetData.ColumnDatas.TryGetValue(EtcSensorReader.SensorName_Index, out columnDatas))
            {
                columnDatas.Add(sensor.Name);
            }

            if (sheetData.ColumnDatas.TryGetValue(EtcSensorReader.SensorType_Index, out columnDatas))
            {
                columnDatas.Add(sensor.MaterialName);
            }

            if (sheetData.ColumnDatas.TryGetValue(EtcSensorReader.NameOfUnit_Index, out columnDatas))
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
            sheetData.Titles[EtcSensorReader.UniqueKey_Index] = EtcSensorReader.UniqueKey_Name;
            sheetData.Titles[EtcSensorReader.Location_Index] = EtcSensorReader.Location_Name;
            sheetData.Titles[EtcSensorReader.SensorName_Index] = EtcSensorReader.SensorName_Name;
            sheetData.Titles[EtcSensorReader.SensorType_Index] = EtcSensorReader.SensorType_Name;
            sheetData.Titles[EtcSensorReader.NameOfUnit_Index] = EtcSensorReader.NameOfUnit_Name;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                sheetData.ColumnDatas[pair.Key] = new List<string>();
            }
        }
    }
}
