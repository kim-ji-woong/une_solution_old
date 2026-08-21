using System.Collections.Generic;
using SDMS.Model.Spatial;
using SDMS.DAL;

namespace ExcelWorker.Reader
{
    public class BuildingReader : ExcelReader
    {
        public const string BuildingName = "건물";
        public const string Text = "Text";
        public const string WithDot = "WithDot";
        public const string Indent = "들여쓰기";

        private int Building_Index = 0;
        private int Text_Index = 1;
        private int WithDot_Index = 2;
        private int Indent_Index = 3;

        public BuildingReader(string strFilePath)
            : base(strFilePath)
        {
        }

        protected override bool UpdateData(List<SheetData> sheetDatas)
        {
            if (m_dataManager == null)
                return false;

            string strErrorMessage;
            Dictionary<int, Building> dicBuildings;
            Dictionary<string, Building> dicBuildingNames;

            // Key : Building Name
            Dictionary<string, List<BuildingData>> dicBuildingDatas = ReadDB(m_dataManager, out dicBuildings, out dicBuildingNames, out strErrorMessage);

            if (dicBuildingDatas == null)
                return false;

            return CheckData(dicBuildingDatas, dicBuildings, dicBuildingNames, sheetDatas, out strErrorMessage);
        }

        private bool CheckData(Dictionary<string, List<BuildingData>> dicBuildingDatas, Dictionary<int, Building> dicBuildings, Dictionary<string, Building> dicBuildingNames, List<SheetData> sheetDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (SheetData sheet in sheetDatas)
            {
                Dictionary<string, List<BuildingData>> dicSheetBuildingDatas = MakeSheetBuildingDatas(sheet, dicBuildings, dicBuildingNames);

                if (CheckData(dicBuildingDatas, dicSheetBuildingDatas, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private bool CheckData(Dictionary<string, List<BuildingData>> dicDBBuildingDatas, Dictionary<string, List<BuildingData>> dicSheetBuildingDatas, out string strErrorMessage)
        {
            List<BuildingData> removeDatas = new List<BuildingData>();
            List<BuildingData> newDatas = new List<BuildingData>();
            List<BuildingData> changedDatas = new List<BuildingData>();

            List<BuildingData> buildingDatas;
            Dictionary<int, List<int>> dicAliveDataID = new Dictionary<int, List<int>>();

            foreach (KeyValuePair<string, List<BuildingData>> pair in dicSheetBuildingDatas)
            {
                if (dicDBBuildingDatas.TryGetValue(pair.Key, out buildingDatas) == false)
                {
                    continue;
                }
                else
                {
                    int nOrderIndex = 1;

                    foreach (BuildingData buildingData in pair.Value)
                    {
                        BuildingData data = FindData(buildingDatas, buildingData, nOrderIndex++);

                        if (data == null)
                        {
                            buildingData.OrderIndex = nOrderIndex - 1;
                            newDatas.Add(buildingData);
                            SetAliveData(dicAliveDataID, buildingData, nOrderIndex - 1);
                        }
                        else
                        {
                            int result = CompareData(data, buildingData);

                            if (result != 0)
                                changedDatas.Add(data);

                            SetAliveData(dicAliveDataID, buildingData, nOrderIndex - 1);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, List<BuildingData>> pair in dicDBBuildingDatas)
            {
                foreach (BuildingData data in pair.Value)
                {
                    if (IsAliveData(data, dicAliveDataID) == false)
                        removeDatas.Add(data);
                }
            }

            if (RemoveData(removeDatas, out strErrorMessage) == false)
            {
                strErrorMessage = "CheckData, RemoveData Error : " + strErrorMessage;
                return false;
            }

            if (ChangeData(changedDatas, out strErrorMessage) == false)
            {
                strErrorMessage = "CheckData, ChangeData Error : " + strErrorMessage;
                return false;
            }

            if (AddData(newDatas, out strErrorMessage) == false)
            {
                strErrorMessage = "CheckData, AddData Error : " + strErrorMessage;
                return false;
            }

            return true;
        }

        private bool AddData(List<BuildingData> newDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (BuildingData data in newDatas)
            {
                BuildingData _data = m_dataManager.GetCreateManager().CreateBuildingData(data.BuildingID, data.OrderIndex, data.Value, data.WithDot, data.IndentDepth);

                if (_data == null)
                {
                    strErrorMessage = m_dataManager.GetCreateManager().GetErrorMessage();
                    return false;
                }
            }

            return true;
        }

        private bool ChangeData(List<BuildingData> changedDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (BuildingData data in changedDatas)
            {
                if (m_dataManager.GetUpdateManager().UpdateBuildingData(data, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private bool RemoveData(List<BuildingData> removeDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (BuildingData data in removeDatas)
            {
                if (m_dataManager.GetDeleteManager().DeleteBuildingData(data.BuildingID, data.OrderIndex, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        // Return 값 :
        //              0 => 동일하다.
        //              1 => 다르다.
        private int CompareData(BuildingData dbData, BuildingData sheetData)
        {
            if (dbData.Value != sheetData.Value)
                return SetData(dbData, sheetData, 1);
            if (dbData.WithDot != sheetData.WithDot)
                return SetData(dbData, sheetData, 1);
            if (dbData.IndentDepth != sheetData.IndentDepth)
                return SetData(dbData, sheetData, 1);

            return 0;
        }

        private int SetData(BuildingData trg, BuildingData src, int result)
        {
            trg.Value = src.Value;
            trg.WithDot = src.WithDot;
            trg.IndentDepth = src.IndentDepth;
            return result;
        }

        private bool IsAliveData(BuildingData data, Dictionary<int, List<int>> dicAliveDataID)
        {
            List<int> orderIndices;

            if (dicAliveDataID.TryGetValue(data.BuildingID, out orderIndices))
            {
                foreach (int order in orderIndices)
                {
                    if (data.OrderIndex == order)
                        return true;
                }
            }

            return false;
        }

        private void SetAliveData(Dictionary<int, List<int>> dicAliveDataID, BuildingData buildingData, int nOrderIndex)
        {
            List<int> orderIndices;

            if (dicAliveDataID.TryGetValue(buildingData.BuildingID, out orderIndices) == false)
            {
                orderIndices = new List<int>();
                dicAliveDataID[buildingData.BuildingID] = orderIndices;
            }

            orderIndices.Add(nOrderIndex);
        }

        private BuildingData FindData(List<BuildingData> datas, BuildingData buildingData, int nOrderIndex)
        {
            foreach (BuildingData data in datas)
            {
                if (data.BuildingID == buildingData.BuildingID && data.OrderIndex == nOrderIndex)
                {
                    datas.Remove(data);
                    return data;
                }
            }

            return null;
        }

        // Key : Building Name
        private Dictionary<string, List<BuildingData>> MakeSheetBuildingDatas(SheetData sheetData, Dictionary<int, Building> dicBuildings, Dictionary<string, Building> dicBuildingNames)
        {
            // Key : Building Name
            Dictionary<string, List<BuildingData>> dicSheetBuildingDatas = new Dictionary<string, List<BuildingData>>();

            int min = 1, max = 0;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                if (pair.Value == BuildingName)
                    Building_Index = pair.Key;
                else if (pair.Value == Text)
                    Text_Index = pair.Key;
                else if (pair.Value == WithDot)
                    WithDot_Index = pair.Key;
                else if (pair.Value == Indent)
                    Indent_Index = pair.Key;

                if (min > max)
                    min = max = pair.Key;

                if (min > pair.Key)
                    min = pair.Key;

                if (max < pair.Key)
                    max = pair.Key;
            }

            if (min > max)
                return dicSheetBuildingDatas;

            int maxColumnCount = 0;
            int[] arrColumnCount = GetColumnCounts(sheetData, min, max, out maxColumnCount);

            if (arrColumnCount == null)
                return dicSheetBuildingDatas;

            List<string> datas;
            string strPrevBuildingName = null;
            bool withDot;
            int indent = 0;

            for (int j = 0; j < maxColumnCount; j++)
            {
                string strBuildingName = null;
                string strText = null;
                string strWithDot = null;
                string strIndent = null;

                for (int i = min; i <= max; i++)
                {
                    if (sheetData.ColumnDatas.TryGetValue(i, out datas))
                    {
                        if (arrColumnCount[i] > j)
                        {
                            if (i == Building_Index)
                                strBuildingName = datas[j];
                            else if (i == Text_Index)
                                strText = datas[j];
                            else if (i == WithDot_Index)
                                strWithDot = datas[j];
                            else if (i == Indent_Index)
                                strIndent = datas[j];
                        }
                    }
                }

                if (strBuildingName == null)
                    strBuildingName = strPrevBuildingName;

                if (strBuildingName == null)
                    continue;

                if (strText == null)
                    continue;

                if (strWithDot == null)
                    continue;

                strPrevBuildingName = strBuildingName;

                string strLower = strWithDot.ToLower();

                if (strWithDot == "1" || strLower == "true")
                    withDot = true;
                else if (strWithDot == "0" || strLower == "false")
                    withDot = false;
                else
                    continue;

                if (strIndent != null)
                {
                    if (int.TryParse(strIndent, out indent) == false)
                        continue;
                }

                Building building;

                if (dicBuildingNames.TryGetValue(strBuildingName, out building) == false)
                    continue;

                BuildingData data = new BuildingData();
                data.BuildingID = building.ID;
                data.Value = strText;
                data.WithDot = withDot;

                if (strIndent != null)
                    data.IndentDepth = indent;

                List<BuildingData> buildingDatas;

                if (dicSheetBuildingDatas.TryGetValue(strBuildingName, out buildingDatas) == false)
                {
                    buildingDatas = new List<BuildingData>();
                    dicSheetBuildingDatas[strBuildingName] = buildingDatas;
                }

                buildingDatas.Add(data);
            }

            return dicSheetBuildingDatas;
        }

        // Key : Building Name

        public static Dictionary<string, List<BuildingData>> ReadDB(DataManager dataManager, out Dictionary<int, Building> dicBuildings, out Dictionary<string, Building> dicBuildingNames, out string strErrorMessage)
        {
            dicBuildings = null;
            dicBuildingNames = new Dictionary<string, Building>();

            List<Building> buildings = dataManager.GetSelectManager().SelectBuildings(null, null, out strErrorMessage);

            if (buildings == null || strErrorMessage != null)
                return null;

            List<BuildingData> datas = dataManager.GetSelectManager().SelectBuildingDatas(null, null, out strErrorMessage);

            if (datas == null || strErrorMessage != null)
                return null;

            Building building;
            dicBuildings = ToDictionary(buildings);

            foreach (KeyValuePair<int, Building> pair in dicBuildings)
            {
                dicBuildingNames[pair.Value.BuildingName] = pair.Value;
            }

            List<BuildingData> buildingDatas;
            Dictionary<string, List<BuildingData>> dicBuildingDatas = new Dictionary<string, List<BuildingData>>();

            foreach (BuildingData data in datas)
            {
                if (dicBuildings.TryGetValue(data.BuildingID, out building))
                {
                    if (dicBuildingDatas.TryGetValue(building.BuildingName, out buildingDatas) == false)
                    {
                        buildingDatas = new List<BuildingData>();
                        dicBuildingDatas[building.BuildingName] = buildingDatas;
                    }

                    buildingDatas.Add(data);
                }
            }

            foreach (KeyValuePair<string, List<BuildingData>> pair in dicBuildingDatas)
            {
                pair.Value.Sort();
            }

            return dicBuildingDatas;
        }
    }
}
