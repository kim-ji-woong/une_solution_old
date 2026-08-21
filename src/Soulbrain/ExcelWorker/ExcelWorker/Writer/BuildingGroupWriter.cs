using System.Collections.Generic;
using SDMS.Model.Spatial;

namespace ExcelWorker.Writer
{
    using Reader;

    public class BuildingGroupWriter : ExcelWriter
    {
        public BuildingGroupWriter(string strFilePath)
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

            Dictionary<int, BuildingGroup> dicBuildingGroups;
            Dictionary<string, BuildingGroup> dicBuildingGroupNames;

            // Key : BuildingGroup Name
            Dictionary<string, List<BuildingGroupData>> dicBuildingGroupDatas = BuildingGroupReader.ReadDB(m_dataManager, out dicBuildingGroups, out dicBuildingGroupNames, out strErrorMessage);

            if (dicBuildingGroupDatas == null)
                return null;

            CheckEmptyBuildingGroupData(dicBuildingGroups.Values, dicBuildingGroupDatas);

            BuildingGroup buildingGroup;
            SheetData sheetData = null;

            foreach (KeyValuePair<string, List<BuildingGroupData>> pair in dicBuildingGroupDatas)
            {
                string strBuildingGroupName = pair.Key;
                List<BuildingGroupData> datas = pair.Value;
                datas.Sort();

                if (sheetData == null)
                {
                    sheetData = new SheetData("data");
                    SetTitles(sheetData);
                }

                if (dicBuildingGroupNames.TryGetValue(strBuildingGroupName, out buildingGroup) == false)
                    continue;

                int nCount = datas.Count;

                if (nCount == 0)
                    SetColumnDatas(sheetData, buildingGroup.GroupName, null, null, null);
                else
                {
                    SetColumnDatas(sheetData, buildingGroup.GroupName, datas[0].Value, datas[0].WithDot, datas[0].IndentDepth);

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

        private void CheckEmptyBuildingGroupData(ICollection<BuildingGroup> buildingGroups, Dictionary<string, List<BuildingGroupData>> dicBuildingGroupDatas)
        {
            List<BuildingGroupData> buildingGroupDatas;

            foreach (BuildingGroup buildingGroup in buildingGroups)
            {
                if (dicBuildingGroupDatas.TryGetValue(buildingGroup.GroupName, out buildingGroupDatas) == false)
                    dicBuildingGroupDatas[buildingGroup.GroupName] = new List<BuildingGroupData>();
            }
        }

        private void SetColumnDatas(SheetData sheetData, string strBuildingGroupName, string strValue, bool? withDot, int? indent)
        {
            List<string> columnDatas;

            if (sheetData.ColumnDatas.TryGetValue(0, out columnDatas))
            {
                if (strBuildingGroupName != null)
                    columnDatas.Add(strBuildingGroupName);
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
            sheetData.Titles[0] = BuildingGroupReader.BuildingGroupName;
            sheetData.Titles[1] = BuildingGroupReader.Text;
            sheetData.Titles[2] = BuildingGroupReader.WithDot;
            sheetData.Titles[3] = BuildingGroupReader.Indent;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                sheetData.ColumnDatas[pair.Key] = new List<string>();
            }
        }
    }
}
