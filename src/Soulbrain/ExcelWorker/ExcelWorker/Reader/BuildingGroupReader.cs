using System.Collections.Generic;
using SDMS.Model.Spatial;
using SDMS.DAL;

namespace ExcelWorker.Reader
{
    public class BuildingGroupReader : ExcelReader
    {
        public const string BuildingGroupName = "공장동";
        public const string Text = "Text";
        public const string WithDot = "WithDot";
        public const string Indent = "들여쓰기";

        private int BuildingGroup_Index = 0;
        private int Text_Index = 1;
        private int WithDot_Index = 2;
        private int Indent_Index = 3;

        public BuildingGroupReader(string strFilePath)
            : base(strFilePath)
        {
        }

        protected override bool UpdateData(List<SheetData> sheetDatas)
        {
            if (m_dataManager == null)
                return false;

            string strErrorMessage;
            Dictionary<int, BuildingGroup> dicBuildingGroups;
            Dictionary<string, BuildingGroup> dicBuildingGroupNames;

            // Key : BuildingGroup Name
            Dictionary<string, List<BuildingGroupData>> dicBuildingGroupDatas = ReadDB(m_dataManager, out dicBuildingGroups, out dicBuildingGroupNames, out strErrorMessage);

            if (dicBuildingGroupDatas == null)
                return false;

            return CheckData(dicBuildingGroupDatas, dicBuildingGroups, dicBuildingGroupNames, sheetDatas, out strErrorMessage);
        }

        private bool CheckData(Dictionary<string, List<BuildingGroupData>> dicBuildingGroupDatas, Dictionary<int, BuildingGroup> dicBuildingGroups, Dictionary<string, BuildingGroup> dicBuildingGroupNames, List<SheetData> sheetDatas, out string strErrorMessage)
        {
            strErrorMessage = null;
            
            foreach (SheetData sheet in sheetDatas)
            {
                Dictionary<string, List<BuildingGroupData>> dicSheetBuildingGroupDatas = MakeSheetBuildingGroupDatas(sheet, dicBuildingGroups, dicBuildingGroupNames);
                    
                if (CheckData(dicBuildingGroupDatas, dicSheetBuildingGroupDatas, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private bool CheckData(Dictionary<string, List<BuildingGroupData>> dicDBBuildingGroupDatas, Dictionary<string, List<BuildingGroupData>> dicSheetBuildingGroupDatas, out string strErrorMessage)
        {
            List<BuildingGroupData> removeDatas = new List<BuildingGroupData>();
            List<BuildingGroupData> newDatas = new List<BuildingGroupData>();
            List<BuildingGroupData> changedDatas = new List<BuildingGroupData>();

            List<BuildingGroupData> buildingGroupDatas;
            Dictionary<int, List<int>> dicAliveDataID = new Dictionary<int, List<int>>();

            foreach (KeyValuePair<string, List<BuildingGroupData>> pair in dicSheetBuildingGroupDatas)
            {
                if (dicDBBuildingGroupDatas.TryGetValue(pair.Key, out buildingGroupDatas) == false)
                {
                    continue;
                }
                else
                {
                    int nOrderIndex = 1;

                    foreach (BuildingGroupData buildingGroupData in pair.Value)
                    {
                        BuildingGroupData data = FindData(buildingGroupDatas, buildingGroupData, nOrderIndex++);

                        if (data == null)
                        {
                            buildingGroupData.OrderIndex = nOrderIndex - 1;
                            newDatas.Add(buildingGroupData);
                            SetAliveData(dicAliveDataID, buildingGroupData, nOrderIndex - 1);
                        }
                        else
                        {
                            int result = CompareData(data, buildingGroupData);

                            if (result != 0)
                                changedDatas.Add(data);

                            SetAliveData(dicAliveDataID, buildingGroupData, nOrderIndex - 1);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, List<BuildingGroupData>> pair in dicDBBuildingGroupDatas)
            {
                foreach (BuildingGroupData data in pair.Value)
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

        private bool AddData(List<BuildingGroupData> newDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (BuildingGroupData data in newDatas)
            {
                BuildingGroupData _data = m_dataManager.GetCreateManager().CreateBuildingGroupData(data.BuildingGroupID, data.OrderIndex, data.Value, data.WithDot, data.IndentDepth);

                if (_data == null)
                {
                    strErrorMessage = m_dataManager.GetCreateManager().GetErrorMessage();
                    return false;
                }
            }

            return true;
        }

        private bool ChangeData(List<BuildingGroupData> changedDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (BuildingGroupData data in changedDatas)
            {
                if (m_dataManager.GetUpdateManager().UpdateBuildingGroupData(data, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private bool RemoveData(List<BuildingGroupData> removeDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (BuildingGroupData data in removeDatas)
            {
                if (m_dataManager.GetDeleteManager().DeleteBuildingGroupData(data.BuildingGroupID, data.OrderIndex, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        // Return 값 :
        //              0 => 동일하다.
        //              1 => 다르다.
        private int CompareData(BuildingGroupData dbData, BuildingGroupData sheetData)
        {
            if (dbData.Value != sheetData.Value)
                return SetData(dbData, sheetData, 1);
            if (dbData.WithDot != sheetData.WithDot)
                return SetData(dbData, sheetData, 1);
            if (dbData.IndentDepth != sheetData.IndentDepth)
                return SetData(dbData, sheetData, 1);

            return 0;
        }

        private int SetData(BuildingGroupData trg, BuildingGroupData src, int result)
        {
            trg.Value = src.Value;
            trg.WithDot = src.WithDot;
            trg.IndentDepth = src.IndentDepth;
            return result;
        }

        private bool IsAliveData(BuildingGroupData data, Dictionary<int, List<int>> dicAliveDataID)
        {
            List<int> orderIndices;

            if (dicAliveDataID.TryGetValue(data.BuildingGroupID, out orderIndices))
            {
                foreach (int order in orderIndices)
                {
                    if (data.OrderIndex == order)
                        return true;
                }
            }

            return false;
        }

        private void SetAliveData(Dictionary<int, List<int>> dicAliveDataID, BuildingGroupData buildingGroupData, int nOrderIndex)
        {
            List<int> orderIndices;

            if (dicAliveDataID.TryGetValue(buildingGroupData.BuildingGroupID, out orderIndices) == false)
            {
                orderIndices = new List<int>();
                dicAliveDataID[buildingGroupData.BuildingGroupID] = orderIndices;
            }

            orderIndices.Add(nOrderIndex);
        }

        private BuildingGroupData FindData(List<BuildingGroupData> datas, BuildingGroupData buildingGroupData, int nOrderIndex)
        {
            foreach (BuildingGroupData data in datas)
            {
                if (data.BuildingGroupID == buildingGroupData.BuildingGroupID && data.OrderIndex == nOrderIndex)
                {
                    datas.Remove(data);
                    return data;
                }
            }

            return null;
        }

        // Key : BuildingGroup Name
        private Dictionary<string, List<BuildingGroupData>> MakeSheetBuildingGroupDatas(SheetData sheetData, Dictionary<int, BuildingGroup> dicBuildingGroups, Dictionary<string, BuildingGroup> dicBuildingGroupNames)
        {
            // Key : BuildingGroup Name
            Dictionary<string, List<BuildingGroupData>> dicSheetBuildingGroupDatas = new Dictionary<string, List<BuildingGroupData>>();

            int min = 1, max = 0;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                if (pair.Value == BuildingGroupName)
                    BuildingGroup_Index = pair.Key;
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
                return dicSheetBuildingGroupDatas;

            int maxColumnCount = 0;
            int[] arrColumnCount = GetColumnCounts(sheetData, min, max, out maxColumnCount);

            if (arrColumnCount == null)
                return dicSheetBuildingGroupDatas;

            List<string> datas;
            string strPrevBuildingGroupName = null;
            bool withDot;
            int indent = 0;

            for (int j = 0; j < maxColumnCount; j++)
            {
                string strBuildingGroupName = null;
                string strText = null;
                string strWithDot = null;
                string strIndent = null;

                for (int i = min; i <= max; i++)
                {
                    if (sheetData.ColumnDatas.TryGetValue(i, out datas))
                    {
                        if (arrColumnCount[i] > j)
                        {
                            if (i == BuildingGroup_Index)
                                strBuildingGroupName = datas[j];
                            else if (i == Text_Index)
                                strText = datas[j];
                            else if (i == WithDot_Index)
                                strWithDot = datas[j];
                            else if (i == Indent_Index)
                                strIndent = datas[j];
                        }
                    }
                }

                if (strBuildingGroupName == null)
                    strBuildingGroupName = strPrevBuildingGroupName;

                if (strBuildingGroupName == null)
                    continue;

                if (strText == null)
                    continue;

                if (strWithDot == null)
                    continue;

                strPrevBuildingGroupName = strBuildingGroupName;

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

                BuildingGroup buildingGroup;

                if (dicBuildingGroupNames.TryGetValue(strBuildingGroupName, out buildingGroup) == false)
                    continue;

                BuildingGroupData data = new BuildingGroupData();
                data.BuildingGroupID = buildingGroup.ID;
                data.Value = strText;
                data.WithDot = withDot;

                if (strIndent != null)
                    data.IndentDepth = indent;

                List<BuildingGroupData> buildingGroupDatas;
                
                if (dicSheetBuildingGroupDatas.TryGetValue(strBuildingGroupName, out buildingGroupDatas) == false)
                {
                    buildingGroupDatas = new List<BuildingGroupData>();
                    dicSheetBuildingGroupDatas[strBuildingGroupName] = buildingGroupDatas;
                }

                buildingGroupDatas.Add(data);
            }

            return dicSheetBuildingGroupDatas;
        }

        // Key : BuildingGroup Name

        public static Dictionary<string, List<BuildingGroupData>> ReadDB(DataManager dataManager, out Dictionary<int, BuildingGroup> dicBuildingGroups, out Dictionary<string, BuildingGroup> dicBuildingGroupNames, out string strErrorMessage)
        {
            dicBuildingGroups = null;
            dicBuildingGroupNames = new Dictionary<string, BuildingGroup>();

            List<BuildingGroup> buildingGroups = dataManager.GetSelectManager().SelectBuildingGroups(null, null, out strErrorMessage);

            if (buildingGroups == null || strErrorMessage != null)
                return null;

            List<BuildingGroupData> datas = dataManager.GetSelectManager().SelectBuildingGroupDatas(null, null, out strErrorMessage);

            if (datas == null || strErrorMessage != null)
                return null;

            BuildingGroup buildingGroup;
            dicBuildingGroups = ToDictionary(buildingGroups);

            foreach (KeyValuePair<int, BuildingGroup> pair in dicBuildingGroups)
            {
                dicBuildingGroupNames[pair.Value.GroupName] = pair.Value;
            }

            List<BuildingGroupData> buildingGroupDatas;
            Dictionary<string, List<BuildingGroupData>> dicBuildingGroupDatas = new Dictionary<string, List<BuildingGroupData>>();

            foreach (BuildingGroupData data in datas)
            {
                if (dicBuildingGroups.TryGetValue(data.BuildingGroupID, out buildingGroup))
                {
                    if (dicBuildingGroupDatas.TryGetValue(buildingGroup.GroupName, out buildingGroupDatas) == false)
                    {
                        buildingGroupDatas = new List<BuildingGroupData>();
                        dicBuildingGroupDatas[buildingGroup.GroupName] = buildingGroupDatas;
                    }

                    buildingGroupDatas.Add(data);
                }
            }

            foreach (KeyValuePair<string, List<BuildingGroupData>> pair in dicBuildingGroupDatas)
            {
                pair.Value.Sort();
            }

            return dicBuildingGroupDatas;
        }
    }
}
