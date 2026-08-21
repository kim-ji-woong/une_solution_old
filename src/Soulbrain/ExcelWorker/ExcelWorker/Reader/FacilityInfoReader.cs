using System.Collections;
using System.Collections.Generic;
using SDMS.Model.Facility;
using SDMS.Model.Spatial;

namespace ExcelWorker.Reader
{
    public class FacilityInfoReader : ExcelReader
    {
        private class FacilityInfoData
        {
            private int m_nFacilityDBID = -1;
            private int m_nOrderIndex = -1;
            private string m_strFacilityID = "";
            private string m_strFacilityName = "";
            private string m_strText = "";
            private bool m_withDot = true;
            private int? m_nIndent = null;

            public int FacilityDBID
            {
                get { return m_nFacilityDBID; }
                set { m_nFacilityDBID = value; }
            }

            public int OrderIndex
            {
                get { return m_nOrderIndex; }
                set { m_nOrderIndex = value; }
            }

            public string FacilityID
            {
                get { return m_strFacilityID; }
                set { m_strFacilityID = value; }
            }

            public string FacilityName
            {
                get { return m_strFacilityName; }
                set { m_strFacilityName = value; }
            }

            public string Text
            {
                get { return m_strText; }
                set { m_strText = value; }
            }

            public bool WithDot
            {
                get { return m_withDot; }
                set { m_withDot = value; }
            }

            public int? Indent
            {
                get { return m_nIndent; }
                set { m_nIndent = value; }
            }
        }

        public const string FacilityID = "설비 ID";
        public const string FacilityName = "설비이름";
        public const string Text = "Text";
        public const string WithDot = "WithDot";
        public const string Indent = "들여쓰기";

        private int FacilityID_Index = 0;
        private int FacilityName_Index = 1;
        private int Text_Index = 2;
        private int WithDot_Index = 3;
        private int Indent_Index = 4;

        public FacilityInfoReader(string strFilePath)
            : base(strFilePath)
        {
        }

        protected override bool UpdateData(List<SheetData> sheetDatas)
        {
            if (m_dataManager == null)
                return false;

            string strErrorMessage;
            Dictionary<int, Info> dicFacilityInfo;

            // Key : Building Name
            // Value : Key(Zone ID), Value(Zone별 FacilityInfoData List)
            Dictionary<string, Dictionary<int, List<InfoData>>> dicBuildingFacilityDatas = ReadDB(out dicFacilityInfo, out strErrorMessage);

            if (dicBuildingFacilityDatas == null)
                return false;

            return CheckData(dicBuildingFacilityDatas, dicFacilityInfo, sheetDatas, out strErrorMessage);
        }

        private bool CheckData(Dictionary<string, Dictionary<int, List<InfoData>>> dicBuildingFacilityDatas, Dictionary<int, Info> dicFacilityInfo, List<SheetData> sheetDatas, out string strErrorMessage)
        {
            strErrorMessage = null;
            Dictionary<int, List<InfoData>> dicInfoDatas;

            foreach (SheetData sheet in sheetDatas)
            {
                if (dicBuildingFacilityDatas.TryGetValue(sheet.SheetName, out dicInfoDatas))
                {
                    if (CheckData(dicInfoDatas, dicFacilityInfo, sheet, out strErrorMessage) == false)
                        return false;
                }
            }

            return true;
        }

        // dicInfoDatas.Key : Zone ID
        private bool CheckData(Dictionary<int, List<InfoData>> dicInfoDatas, Dictionary<int, Info> dicFacilityInfo, SheetData sheetData, out string strErrorMessage)
        {
            // Key : FacilityInfo ID(string)
            Dictionary<string, List<FacilityInfoData>> dicSheetInfoDatas = MakeSheetFacilityInfoDatas(sheetData, dicFacilityInfo);
            Dictionary<string, List<InfoData>> dicDBInfoDatas = MakeDBFacilityInfoDatas(dicInfoDatas, dicFacilityInfo);

            List<InfoData> removeDatas = new List<InfoData>();
            List<FacilityInfoData> newDatas = new List<FacilityInfoData>();
            List<InfoData> changedDatas = new List<InfoData>();
            Dictionary<int, Info> dicChangedInfos = new Dictionary<int, Info>();

            Dictionary<int, List<int>> dicAliveDataID = new Dictionary<int, List<int>>();

            List<InfoData> datas;
            Info info;

            foreach (KeyValuePair<string, List<FacilityInfoData>> pair in dicSheetInfoDatas)
            {
                if (dicDBInfoDatas.TryGetValue(pair.Key, out datas) == false)
                {
                    foreach (FacilityInfoData facilityData in pair.Value)
                    {
                        if (facilityData.FacilityDBID > 0)
                        {
                            newDatas.Add(facilityData);
                        }
                    }
                }
                else
                {
                    int nOrderIndex = 1;

                    foreach (FacilityInfoData facilityData in pair.Value)
                    {
                        InfoData data = FindData(datas, facilityData, nOrderIndex++);

                        if (data == null)
                        {
                            if (facilityData.FacilityDBID > 0)
                            {
                                facilityData.OrderIndex = nOrderIndex - 1;
                                newDatas.Add(facilityData);
                                SetAliveData(dicAliveDataID, facilityData, nOrderIndex - 1);
                            }
                        }
                        else
                        {
                            dicFacilityInfo.TryGetValue(data.FacilityInfoID, out info);

                            int result = CompareData(data, info, facilityData);

                            if ((result & 1) == 1)
                                changedDatas.Add(data);
                            if ((result & 2) == 2)
                                dicChangedInfos[info.ID] = info;

                            SetAliveData(dicAliveDataID, facilityData, nOrderIndex - 1);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, List<InfoData>> pair in dicDBInfoDatas)
            {
                foreach (InfoData data in pair.Value)
                {
                    if (IsAliveData(data, dicAliveDataID) == false)
                        removeDatas.Add(data);
                }
            }

            if (ChangeInfo(dicChangedInfos, out strErrorMessage) == false)
            {
                strErrorMessage = "CheckData, ChangeInfo Error : " + strErrorMessage;
                return false;
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

        private bool IsAliveData(InfoData data, Dictionary<int, List<int>> dicAliveDataID)
        {
            List<int> orderIndices;

            if (dicAliveDataID.TryGetValue(data.FacilityInfoID, out orderIndices))
            {
                foreach (int order in orderIndices)
                {
                    if (data.OrderIndex == order)
                        return true;
                }
            }

            return false;
        }

        private void SetAliveData(Dictionary<int, List<int>> dicAliveDataID, FacilityInfoData facilityData, int nOrderIndex)
        {
            List<int> orderIndices;

            if (dicAliveDataID.TryGetValue(facilityData.FacilityDBID, out orderIndices) == false)
            {
                orderIndices = new List<int>();
                dicAliveDataID[facilityData.FacilityDBID] = orderIndices;
            }

            orderIndices.Add(nOrderIndex);
        }

        private bool AddData(List<FacilityInfoData> newDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (FacilityInfoData data in newDatas)
            {
                InfoData infoData = m_dataManager.GetCreateManager().CreateFacilityInfoData(data.FacilityDBID, data.OrderIndex, data.Text, data.WithDot, data.Indent);

                if (infoData == null)
                {
                    strErrorMessage = m_dataManager.GetCreateManager().GetErrorMessage();
                    return false;
                }
            }

            return true;
        }

        private bool ChangeData(List<InfoData> changedDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (InfoData data in changedDatas)
            {
                if (m_dataManager.GetUpdateManager().UpdateFacilityInfoData(data, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private bool RemoveData(List<InfoData> removeDatas, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (InfoData data in removeDatas)
            {
                if (m_dataManager.GetDeleteManager().DeleteFacilityInfoData(data.FacilityInfoID, data.OrderIndex, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private bool ChangeInfo(Dictionary<int, Info> dicChangedInfos, out string strErrorMessage)
        {
            strErrorMessage = null;

            foreach (KeyValuePair<int, Info> pair in dicChangedInfos)
            {
                if (m_dataManager.GetUpdateManager().UpdateFacilityInfo(pair.Value, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        // Return 값 :
        //              0 => 모두 동일하다.
        //              1 => InfoData가 다르다.
        //              2 => info가 다르다.
        //              3 => 둘다 다르다.
        private int CompareData(InfoData data, Info info, FacilityInfoData facilityData)
        {
            int sameInfo = 2;
            
            if (info != null && info.FacilityName == facilityData.FacilityName)
                sameInfo = 0;

            if (sameInfo > 0)
                info.FacilityName = facilityData.FacilityName;

            if (data.Value != facilityData.Text)
                return SetData(data, facilityData, (1 | sameInfo));
            if (data.WithDot != facilityData.WithDot)
                return SetData(data, facilityData, (1 | sameInfo));
            if (data.IndentDepth != facilityData.Indent)
                return SetData(data, facilityData, (1 | sameInfo));

            return sameInfo;
        }

        private int SetData(InfoData data, FacilityInfoData facilityData, int result)
        {
            data.Value = facilityData.Text;
            data.WithDot = facilityData.WithDot;
            data.IndentDepth = facilityData.Indent;
            return result;
        }

        private InfoData FindData(List<InfoData> datas, FacilityInfoData facilityData, int nOrderIndex)
        {
            foreach (InfoData data in datas)
            {
                if (data.FacilityInfoID == facilityData.FacilityDBID && data.OrderIndex == nOrderIndex)
                {
                    datas.Remove(data);
                    return data;
                }
            }

            return null;
        }

        // Key : FacilityInfo ID(string)
        private Dictionary<string, List<InfoData>> MakeDBFacilityInfoDatas(Dictionary<int, List<InfoData>> dicInfoDatas, Dictionary<int, Info> dicFacilityInfo)
        {
            // Key : FacilityInfo ID(string)
            Dictionary<string, List<InfoData>> _dicInfoDatas = new Dictionary<string, List<InfoData>>();

            Info info;
            List<InfoData> infoDatas;

            foreach (KeyValuePair<int, List<InfoData>> pair in dicInfoDatas)
            {
                foreach (InfoData data in pair.Value)
                {
                    if (dicFacilityInfo.TryGetValue(data.FacilityInfoID, out info))
                    {
                        if (_dicInfoDatas.TryGetValue(info.ModelName, out infoDatas) == false)
                        {
                            infoDatas = new List<InfoData>();
                            _dicInfoDatas[info.ModelName] = infoDatas;
                        }

                        infoDatas.Add(data);
                    }
                }
            }

            return _dicInfoDatas;
        }

        // Key : FacilityInfo ID(string)
        private Dictionary<string, List<FacilityInfoData>> MakeSheetFacilityInfoDatas(SheetData sheetData, Dictionary<int, Info> dicFacilityInfo)
        {
            int min = 1, max = 0;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                if (pair.Value == FacilityID)
                    FacilityID_Index = pair.Key;
                else if (pair.Value == FacilityName)
                    FacilityName_Index = pair.Key;
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

            // Key : Facility Model Name
            Dictionary<string, Info> dicFacilityNameInfo = new Dictionary<string, Info>();

            foreach (KeyValuePair<int, Info> pair in dicFacilityInfo)
            {
                dicFacilityNameInfo[pair.Value.ModelName] = pair.Value;
            }

            List<string> datas;
            Dictionary<string, List<FacilityInfoData>> dicSheetInfoDatas = new Dictionary<string, List<FacilityInfoData>>();

            int maxColumnCount = 0;
            int[] arrColumnCount = GetColumnCounts(sheetData, min, max, out maxColumnCount);

            if (arrColumnCount == null)
                return dicSheetInfoDatas;

            string strPrevFacilityID = null;
            string strPrevFacilityName = null;
            bool withDot;
            int indent = 0;

            for (int j = 0; j < maxColumnCount; j++)
            {
                string strFacilityID = null;
                string strFacilityName = null;
                string strText = null;
                string strWithDot = null;
                string strIndent = null;

                for (int i = min; i <= max; i++)
                {
                    if (sheetData.ColumnDatas.TryGetValue(i, out datas))
                    {
                        if (arrColumnCount[i] > j)
                        {
                            if (i == FacilityID_Index)
                                strFacilityID = datas[j];
                            else if (i == FacilityName_Index)
                                strFacilityName = datas[j];
                            else if (i == Text_Index)
                                strText = datas[j];
                            else if (i == WithDot_Index)
                                strWithDot = datas[j];
                            else if (i == Indent_Index)
                                strIndent = datas[j];
                        }
                    }
                }

                if (strFacilityID == null)
                    strFacilityID = strPrevFacilityID;

                if (strFacilityName == null)
                    strFacilityName = strPrevFacilityName;

                if (strFacilityID == null || strFacilityName == null)
                    continue;

                if (strText == null)
                    continue;

                if (strWithDot == null)
                    continue;

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

                FacilityInfoData data = new FacilityInfoData();
                data.FacilityID = strFacilityID;
                data.FacilityName = strFacilityName;
                data.Text = strText;
                data.WithDot = withDot;

                Info dbData;

                if (dicFacilityNameInfo.TryGetValue(data.FacilityID, out dbData))
                {
                    data.FacilityDBID = dbData.ID;
                }

                if (strIndent != null)
                    data.Indent = indent;

                List<FacilityInfoData> infoDatas;

                if (dicSheetInfoDatas.TryGetValue(strFacilityID, out infoDatas) == false)
                {
                    infoDatas = new List<FacilityInfoData>();
                    dicSheetInfoDatas[strFacilityID] = infoDatas;
                }

                infoDatas.Add(data);

                strPrevFacilityID = strFacilityID;
                strPrevFacilityName = strFacilityName;
            }

            return dicSheetInfoDatas;
        }

        // Key : Building Name
        // Value : Key(Zone ID), Value(Zone별 FacilityInfoData List)
        private Dictionary<string, Dictionary<int, List<InfoData>>> ReadDB(out Dictionary<int, Info> dicFacilityInfos, out string strErrorMessage)
        {
            dicFacilityInfos = null;
            List<Building> buildings = m_dataManager.GetSelectManager().SelectBuildings(null, null, out strErrorMessage);

            if (buildings == null || strErrorMessage != null)
                return null;

            List<Zone> zones = m_dataManager.GetSelectManager().SelectZones(null, null, out strErrorMessage);

            if (zones == null || strErrorMessage != null)
                return null;

            Dictionary<int, Building> dicBuildings = ToDictionary(buildings);
            Dictionary<int, Zone> dicZones = ToDictionary(zones);

            List<Info> facilityInfos = new List<Info>();
            List<InfoData> facilityInfoDatas = new List<InfoData>();
            ArrayList arrDatas = m_dataManager.GetSelectManager().JoinFacilityInfoFacilityInfoData(null, null, null, out strErrorMessage);

            if (arrDatas == null || strErrorMessage != null)
                return null;

            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] != null && arrDatas[i + 1] != null &&
                    arrDatas[i] is Info && arrDatas[i + 1] is InfoData)
                {
                    Info facilityInfo = (Info)arrDatas[i];
                    InfoData data = (InfoData)arrDatas[i + 1];

                    facilityInfos.Add(facilityInfo);
                    facilityInfoDatas.Add(data);
                }
            }

            dicFacilityInfos = ToDictionary(facilityInfos);
            return MakeBuildingFacilityInfoDatas(dicBuildings, dicZones, dicFacilityInfos, facilityInfoDatas);
        }

        // Key : Building Name
        // Value : Key(Zone ID), Value(Zone별 FacilityInfoData List)
        private Dictionary<string, Dictionary<int, List<InfoData>>> MakeBuildingFacilityInfoDatas(Dictionary<int, Building> dicBuildings, Dictionary<int, Zone> dicZones, Dictionary<int, Info> dicFacilityInfos, List<InfoData> facilityInfoDatas)
        {
            Info info;
            Zone zone;
            Building building;

            Dictionary<string, Dictionary<int, List<InfoData>>> dicBuildingFacilityDatas = new Dictionary<string, Dictionary<int, List<InfoData>>>();

            foreach (InfoData data in facilityInfoDatas)
            {
                if (dicFacilityInfos.TryGetValue(data.FacilityInfoID, out info))
                {
                    if (dicZones.TryGetValue(info.ZoneID, out zone))
                    {
                        if (zone.BuildingID != null && dicBuildings.TryGetValue((int)zone.BuildingID, out building))
                        {
                            Dictionary<int, List<InfoData>> dicZoneFacilityDatas;

                            if (dicBuildingFacilityDatas.TryGetValue(building.BuildingName, out dicZoneFacilityDatas) == false)
                            {
                                dicZoneFacilityDatas = new Dictionary<int, List<InfoData>>();
                                dicBuildingFacilityDatas[building.BuildingName] = dicZoneFacilityDatas;
                            }

                            List<InfoData> datas;

                            if (dicZoneFacilityDatas.TryGetValue(zone.ID, out datas) == false)
                            {
                                datas = new List<InfoData>();
                                dicZoneFacilityDatas[zone.ID] = datas;
                            }

                            datas.Add(data);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, Dictionary<int, List<InfoData>>> pair1 in dicBuildingFacilityDatas)
            {
                foreach (KeyValuePair<int, List<InfoData>> pair2 in pair1.Value)
                {
                    pair2.Value.Sort();
                }
            }

            return dicBuildingFacilityDatas;
        }
    }
}
