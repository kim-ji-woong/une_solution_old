using System.Collections;
using System.Collections.Generic;
using SDMS.Model.Facility;
using SDMS.Model.Spatial;

namespace ExcelWorker.Writer
{
    using Reader;

    public class FacilityInfoWriter : ExcelWriter
    {
        public FacilityInfoWriter(string strFilePath)
            : base(strFilePath)
        {
        }

        protected override string GetSubject()
        {
            return "설비정보";
        }

        protected override ICollection<SheetData> ReadSheetDatas(out string strErrorMessage)
        {
            if (m_dataManager == null)
            {
                strErrorMessage = "DB에 연결할 수 없습니다.";
                return null;
            }

            // Key : Zone ID
            // Value : Sheet Name
            Dictionary<int, string> dicZoneSheetNames = MakeZoneSheetNames(out strErrorMessage);

            List<Info> infos = m_dataManager.GetSelectManager().SelectFacilityInfos(null, null, out strErrorMessage);

            if (infos == null)
                return null;

            ArrayList arrDatas = m_dataManager.GetSelectManager().JoinFacilityInfoFacilityInfoData(null, null, null, out strErrorMessage);

            if (arrDatas == null)
                return null;

            List<InfoData> infoDatas;

            Dictionary<int, Info> dicInfos = new Dictionary<int, Info>();
            Dictionary<Info, List<InfoData>> dicInfoDatas = new Dictionary<Info, List<InfoData>>();
            int nDataCount = arrDatas.Count;

            for (int i=0;i<nDataCount-1;i+=2)
            {
                if (arrDatas[i] is Info && arrDatas[i + 1] is InfoData)
                {
                    Info info = (Info)arrDatas[i];
                    InfoData data = (InfoData)arrDatas[i + 1];

                    Info _info;

                    if (dicInfos.TryGetValue(info.ID, out _info))
                        info = _info;
                    else
                        dicInfos[info.ID] = info;

                    if (dicInfoDatas.TryGetValue(info, out infoDatas) == false)
                    {
                        infoDatas = new List<InfoData>();
                        dicInfoDatas[info] = infoDatas;
                    }

                    infoDatas.Add(data);
                }
            }

            CheckEmptyFacilityInfo(infos, dicInfoDatas);

            // Key : Zone ID
            Dictionary<int, SheetData> dicSheetDatas = new Dictionary<int, SheetData>();
            // Key : Sheet Name
            Dictionary<string, SheetData> dicSheetDatas2 = new Dictionary<string, SheetData>();

            string strSheetName;
            SheetData sheetData;

            foreach (KeyValuePair<Info, List<InfoData>> pair in dicInfoDatas)
            {
                Info info = pair.Key;
                List<InfoData> datas = pair.Value;
                datas.Sort();

                if (dicSheetDatas.TryGetValue(info.ZoneID, out sheetData) == false)
                {
                    if (dicZoneSheetNames.TryGetValue(info.ZoneID, out strSheetName) == false)
                        continue;

                    if (dicSheetDatas2.TryGetValue(strSheetName, out sheetData))
                    {
                        dicSheetDatas[info.ZoneID] = sheetData;
                    }
                    else
                    {
                        sheetData = new SheetData(strSheetName);
                        SetTitles(sheetData);
                        dicSheetDatas[info.ZoneID] = sheetData;
                        dicSheetDatas2[strSheetName] = sheetData;
                    }
                }

                int nCount = datas.Count;

                if (nCount == 0)
                    SetColumnDatas(sheetData, info.ModelName, info.FacilityName, null, null, null);
                else
                {
                    SetColumnDatas(sheetData, info.ModelName, info.FacilityName, datas[0].Value, datas[0].WithDot, datas[0].IndentDepth);

                    for (int i=1;i<nCount;i++)
                    {
                        SetColumnDatas(sheetData, null, null, datas[i].Value, datas[i].WithDot, datas[i].IndentDepth);
                    }
                }
            }

            return dicSheetDatas2.Values;
        }

        private void CheckEmptyFacilityInfo(List<Info> infos, Dictionary<Info, List<InfoData>> dicInfoDatas)
        {
            Dictionary<int, int> dicInfoDataIDs = new Dictionary<int, int>();

            foreach (KeyValuePair<Info, List<InfoData>> pair in dicInfoDatas)
            {
                dicInfoDataIDs[pair.Key.ID] = pair.Key.ID;
            }

            foreach (Info info in infos)
            {
                if (dicInfoDataIDs.ContainsKey(info.ID) == false)
                    dicInfoDatas[info] = new List<InfoData>();
            }
        }

        private void SetColumnDatas(SheetData sheetData, string strFacilityID, string strFacilityName, string strValue, bool? withDot, int? indent)
        {
            List<string> columnDatas;

            if (sheetData.ColumnDatas.TryGetValue(0, out columnDatas))
            {
                if (strFacilityID != null)
                    columnDatas.Add(strFacilityID);
                else
                    columnDatas.Add(null);
            }

            if (sheetData.ColumnDatas.TryGetValue(1, out columnDatas))
            {
                if (strFacilityName != null)
                    columnDatas.Add(strFacilityName);
                else
                    columnDatas.Add(null);
            }

            if (sheetData.ColumnDatas.TryGetValue(2, out columnDatas))
            {
                if (strValue != null)
                    columnDatas.Add(strValue);
                else
                    columnDatas.Add(null);
            }

            if (sheetData.ColumnDatas.TryGetValue(3, out columnDatas))
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

            if (sheetData.ColumnDatas.TryGetValue(4, out columnDatas))
            {
                if (indent != null)
                    columnDatas.Add(((int)indent).ToString());
                else
                    columnDatas.Add(null);
            }
        }

        private void SetTitles(SheetData sheetData)
        {
            sheetData.Titles[0] = FacilityInfoReader.FacilityID;
            sheetData.Titles[1] = FacilityInfoReader.FacilityName;
            sheetData.Titles[2] = FacilityInfoReader.Text;
            sheetData.Titles[3] = FacilityInfoReader.WithDot;
            sheetData.Titles[4] = FacilityInfoReader.Indent;

            foreach (KeyValuePair<int, string> pair in sheetData.Titles)
            {
                sheetData.ColumnDatas[pair.Key] = new List<string>();
            }
        }

        // Key : Zone ID
        // Value : Sheet Name
        private Dictionary<int, string> MakeZoneSheetNames(out string strErrorMessage)
        {
            List<Building> buildings = m_dataManager.GetSelectManager().SelectBuildings(null, null, out strErrorMessage);

            if (buildings == null)
                return null;

            List<Zone> zones = m_dataManager.GetSelectManager().SelectZones(null, null, out strErrorMessage);

            if (zones == null)
                return null;

            Dictionary<int, Building> dicBuildings = ExcelReader.ToDictionary(buildings);
            Dictionary<int, Zone> dicZones = ExcelReader.ToDictionary(zones);

            Building building;
            Dictionary<int, string> dicZoneSheetNames = new Dictionary<int, string>();

            foreach (KeyValuePair<int, Zone> pair in dicZones)
            {
                if (pair.Value.BuildingID == null)
                    continue;

                if (dicBuildings.TryGetValue((int)pair.Value.BuildingID, out building))
                    dicZoneSheetNames[pair.Key] = building.BuildingName;
            }

            return dicZoneSheetNames;
        }
    }
}
