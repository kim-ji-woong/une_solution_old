using System.Collections.Generic;
using SDMS.Model.Spatial;

namespace ExcelWorker.Writer
{
    using Reader;

    public class BuildingWriter : ExcelWriter
    {
        public BuildingWriter(string strFilePath)
            : base(strFilePath)
        {
        }

        protected override string GetSubject()
        {
            return "공장동 정보";
        }

        protected override ICollection<SheetData> ReadSheetDatas(out string strErrorMessage)
        {
            if (m_dataManager == null)
            {
                strErrorMessage = "DB에 연결할 수 없습니다.";
                return null;
            }

            Dictionary<int, Building> dicBuildings;
            Dictionary<string, Building> dicBuildingNames;

            // Key : Building Name
            Dictionary<string, List<BuildingData>> dicBuildingDatas = BuildingReader.ReadDB(m_dataManager, out dicBuildings, out dicBuildingNames, out strErrorMessage);

            if (dicBuildingDatas == null)
                return null;

            CheckEmptyBuildingData(dicBuildings.Values, dicBuildingDatas);

            Building building;
            SheetData sheetData = null;

            foreach (KeyValuePair<string, List<BuildingData>> pair in dicBuildingDatas)
            {
                string strBuildingName = pair.Key;
                List<BuildingData> datas = pair.Value;
                datas.Sort();

                if (sheetData == null)
                {
                    sheetData = new SheetData("data");
                    SetTitles(sheetData);
                }

                if (dicBuildingNames.TryGetValue(strBuildingName, out building) == false)
                    continue;

                int nCount = datas.Count;

                if (nCount == 0)
                    SetColumnDatas(sheetData, building.BuildingName, null, null, null);
                else
                {
                    SetColumnDatas(sheetData, building.BuildingName, datas[0].Value, datas[0].WithDot, datas[0].IndentDepth);

                    for (int i = 1; i < nCount; i++)
                    {
                        SetColumnDatas(sheetData, null, datas[i].Value, datas[i].WithDot, datas[i].IndentDepth);
                    }
                }
            }

            List<SheetData> sheetDatas = new List<SheetData>();

            if (sheetData != null)
                sheetDatas.Add(sheetData);

            return sheetDatas;
        }

        private void CheckEmptyBuildingData(ICollection<Building> buildings, Dictionary<string, List<BuildingData>> dicBuildingDatas)
        {
            List<BuildingData> buildingDatas;

            foreach (Building building in buildings)
            {
                if (dicBuildingDatas.TryGetValue(building.BuildingName, out buildingDatas) == false)
                    dicBuildingDatas[building.BuildingName] = new List<BuildingData>();
            }
        }

        private void SetColumnDatas(SheetData sheetData, string strBuildingName, string strValue, bool? withDot, int? indent)
        {
            List<string> columnDatas;

            if (sheetData.ColumnDatas.TryGetValue(0, out columnDatas))
            {
                if (strBuildingName != null)
                    columnDatas.Add(strBuildingName);
                else
                    columnDatas.Add(null);
            }

            if (sheetData.ColumnDatas.TryGetValue(1, out columnDatas))
            {
                if (strValue != null)
                    columnDatas.Add(strValue);
                else
                    columnDatas.Add(null);
            }

            if (sheetData.ColumnDatas.TryGetValue(2, out columnDatas))
            {
                if (withDot != null)
                {
                    if ((bool)withDot)
                        columnDatas.Add("1");
                    else
                        columnDatas.Add("0");
                }
                else
                    columnDatas.Add(null);
            }

            if (sheetData.ColumnDatas.TryGetValue(3, out columnDatas))
            {
                if (indent != null)
                    columnDatas.Add(((int)indent).ToString());
                else
                    columnDatas.Add(null);
            }
        }

        private void SetTitles(SheetData sheetData)
        {
            sheetData.Titles[0] = BuildingReader.BuildingName;
            sheetData.Titles[1] = BuildingReader.Text;
            sheetData.Titles[2] = BuildingReader.WithDot;
            sheetData.Titles[3] = BuildingReader.Indent;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                sheetData.ColumnDatas[pair.Key] = new List<string>();
            }
        }
    }
}
