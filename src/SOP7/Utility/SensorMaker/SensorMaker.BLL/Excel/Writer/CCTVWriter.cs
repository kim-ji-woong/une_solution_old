using System.Collections.Generic;

namespace SensorMaker.BLL.Excel.Writer
{
    using Reader;
    using Models.Data.Sensor;

    public class CCTVWriter : ExcelWriter
    {
        private List<CCTVSensor> m_sensors = null;

        public CCTVWriter(List<CCTVSensor> sensorList)
        {
            m_sensors = sensorList;
        }

        protected override string GetSubject()
        {
            return "CCTV";
        }

        protected override ICollection<SheetData> ReadSheetDatas(out string strErrorMessage)
        {
            strErrorMessage = null;

            SheetData sheetData = new SheetData(GetSubject());
            SetTitles(sheetData);

            foreach (CCTVSensor sensor in m_sensors)
            {
                SCCTVolumnDatas(sheetData, sensor);
            }

            List<SheetData> sheetDatas = new List<SheetData>();

            if (sheetData != null)
                sheetDatas.Add(sheetData);

            return sheetDatas;
        }

        private void SCCTVolumnDatas(SheetData sheetData, CCTVSensor sensor)
        {
            if (sensor.UniqueKey == null || sensor.Name == null || sensor.URL == null)
                return;

            List<string> columnDatas;

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.ID_Index, out columnDatas))
            {
                //if (sensor.OrgSensorID == null)
                //    columnDatas.Add(null);
                //else
                    columnDatas.Add(sensor.ID.ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.BuildingID_Index, out columnDatas))
            {
                if (sensor.BuildingID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)sensor.BuildingID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.ZoneID_Index, out columnDatas))
            {
                if (sensor.ZoneID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)sensor.ZoneID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.EquipZoneID_Index, out columnDatas))
            {
                if (sensor.EquipZoneID == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)sensor.EquipZoneID).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.EquipZoneIDs_Index, out columnDatas))
            {
                columnDatas.Add(ListToString(sensor.EquipZoneIDs));
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.UniqueKey_Index, out columnDatas))
            {
                columnDatas.Add(sensor.UniqueKey);
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.Location_Index, out columnDatas))
            {
                columnDatas.Add(sensor.PositionName);
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.CameraName_Index, out columnDatas))
            {
                columnDatas.Add(sensor.Name);
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.ConnectionType_Index, out columnDatas))
            {
                columnDatas.Add(sensor.Type);
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.Channel_Index, out columnDatas))
            {
                if (sensor.Channel == null)
                    columnDatas.Add(null);
                else
                    columnDatas.Add(((int)sensor.Channel).ToString());
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.UserID_Index, out columnDatas))
            {
                columnDatas.Add(sensor.UserID);
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.Password_Index, out columnDatas))
            {
                columnDatas.Add(sensor.Password);
            }

            if (sheetData.ColumnDatas.TryGetValue(CCTVReader.URL_Index, out columnDatas))
            {
                columnDatas.Add(sensor.URL);
            }
        }

        private void SetTitles(SheetData sheetData)
        {
            sheetData.Titles[CCTVReader.ID_Index] = CCTVReader.ID_Name;
            sheetData.Titles[CCTVReader.BuildingID_Index] = CCTVReader.BuildingID_Name;
            sheetData.Titles[CCTVReader.ZoneID_Index] = CCTVReader.ZoneID_Name;
            sheetData.Titles[CCTVReader.EquipZoneID_Index] = CCTVReader.EquipZoneID_Name;
            sheetData.Titles[CCTVReader.EquipZoneIDs_Index] = CCTVReader.EquipZoneIDs_Name;

            sheetData.Titles[CCTVReader.UniqueKey_Index] = CCTVReader.UniqueKey_Name;
            sheetData.Titles[CCTVReader.Location_Index] = CCTVReader.Location_Name;
            sheetData.Titles[CCTVReader.CameraName_Index] = CCTVReader.CameraName_Name;
            sheetData.Titles[CCTVReader.ConnectionType_Index] = CCTVReader.ConnectionType_Name;
            sheetData.Titles[CCTVReader.Channel_Index] = CCTVReader.Channel_Name;
            sheetData.Titles[CCTVReader.UserID_Index] = CCTVReader.UserID_Name;
            sheetData.Titles[CCTVReader.Password_Index] = CCTVReader.Password_Name;
            sheetData.Titles[CCTVReader.URL_Index] = CCTVReader.URL_Name;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                sheetData.ColumnDatas[pair.Key] = new List<string>();
            }
        }
    }
}
