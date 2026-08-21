using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;
using DBUtility2;

namespace SDMS.Report
{
    class ReportData
    {
        //버튼클릭여부
        private bool btnSelect = false;
        //모든데이터 보여줄지 여부
        private bool AllBuildingGroup = false;
        private bool AllBuilding = false;
        private bool AllFloor = false;

        private string strStartDate = "";
        private string strEndDate = "";

        private BuildingGroup group = new BuildingGroup();
        private Building building = new Building();
        private Zone zone = new Zone();

        private WebDBManager webDB = FormMain.Instance.DBManager;


        private Dictionary<int, SensorReactionHistory> m_dicReactionHistory = new Dictionary<int, SensorReactionHistory>();
        public Dictionary<int, SensorReactionHistory> DicReactionHistory
        {
            get { return m_dicReactionHistory; }
            set { m_dicReactionHistory = value; }
        }

      

        public void ComboTxtDate(string strStrat, string strEnd)
        {
            strStartDate = strStrat;
            strEndDate = strEnd;
        }



        public void AllSubmit(bool allBuildingGroup, bool allBuilding, bool allFloor)
        {
            this.AllBuildingGroup = allBuildingGroup;
            this.AllBuilding = allBuilding;
            this.AllFloor = allFloor;
        }

        public void ComboSubmit(BuildingGroup group, Building building, Zone zone, bool btnSelect)
        {
            this.group = group;
            this.building = building;
            this.zone = zone;
            this.btnSelect = btnSelect;
        }

        private int m_nSiteID = 1;
        public ReportData()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            LoadReactionHistory();
        }

        private ArrayList GetSensorZoneResult(WebDBManager dbMgr, string strSQL, bool option = false)
        {
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                return null;

            ArrayList arrResultData = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nSensorZoneType = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                if (equipZone == null)
                    continue;

                if (equipZone.LinkedZoneList.Count == 0)
                    continue;

                Zone firstZone = (Zone)equipZone.LinkedZoneList[0];

                if (firstZone == null)
                    continue;

                //bool notOutdoor = false;

                if (firstZone.Building != null)
                {
                    if (option)
                    {
                        if (AllBuildingGroup == true)
                        {
                        }
                        else if (AllBuilding == true)
                        {
                            if (firstZone.Building.BuildingGroup.GroupID != group.GroupID)
                                continue;
                        }
                        else if (AllFloor == true)
                        {
                            if (firstZone.Building.ID != building.ID)
                                continue;
                        }
                        else
                        {
                            if (firstZone.ID != zone.ID)
                                continue;
                        }
                        //notOutdoor = true;
                    }
                }
                else
                {
                    //if(group.BuildingList == null)

                    if (group.GroupID != ZoneManager.Instance.OutdoorBuildingGroup.GroupID)
                        continue;

                    if (AllBuilding == true)
                    {
 
                    }
                    else if (AllBuilding == true)
                    {
                        if (group != ZoneManager.Instance.OutdoorBuildingGroup)
                            continue;

                    }
                    else if (AllFloor == true)
                    {
                        if (firstZone.ID != zone.ID)
                            continue;
                    }
                    else
                    {
                        if (firstZone.ID != zone.ID)
                            continue;
                    }



                    //notOutdoor = false;
                }

                
                

                arrResultData.Add(arrResult[i]);
                arrResultData.Add(firstZone.ID);
                if (firstZone.Building == null)
                {
                    arrResultData.Add("외부 영역");
                    arrResultData.Add(firstZone.ZoneName);
                }
                else
                {
                    arrResultData.Add(firstZone.Building.BuildingGroup.BuildingGroupName);
                    arrResultData.Add(firstZone.Building.BuildingName);
                }
                
                arrResultData.Add(firstZone.ZoneName);
                arrResultData.Add(firstZone.AddFloor);
                arrResultData.Add(firstZone.FloorIndex);
                arrResultData.Add(arrResult[i + 2]);
                arrResultData.Add(arrResult[i + 3]);
                arrResultData.Add(arrResult[i + 4]);

                if(firstZone.Building == null)
                    arrResultData.Add(-1);
                else
                    arrResultData.Add(firstZone.Building.ID);
				arrResultData.Add(equipZone.ID);
            }

            return arrResultData;
        }

        public void LoadReactionHistory()
        {
            m_dicReactionHistory = new Dictionary<int, SensorReactionHistory>();

            WebDBManager webDB = FormMain.Instance.DBManager;

            string strNowDate = "";
            string strBeforeDate = "";

            strNowDate = string.Format("{0} {1}:{2}:{3}", strEndDate.ToString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", strStartDate.ToString(), 00, 00, 00);

            //string strSQL = "select SensorZone.Type, EquipmentZone.ID, "
             //        + "CASE WHEN ReactionType = 0 THEN count(*)END, "
             //        + "CASE WHEN ReactionType = 21 THEN count(*)END, "
            //         + "CASE WHEN ReactionType = 22 THEN count(*)END ";
            //strSQL += "From EquipmentZone as ez, SensorZone as , SensorZoneHistory, SensorReactionHistory ";
            //strSQL += "where EquipmentZone.ID = SensorZone.EquipZoneID And SensorZone.ID = SensorZoneHistory.SensorID and Eq";
            //strSQL += "And SensorZoneHistory.ID = SensorReactionHistory.SensorHistoryID And ReactionType in ( 0, 21, 22 ) And EquipmentZone.ID <> 0  "; //


            string strSQL = "select sz.Type, ez.ID, " +
                            " CASE WHEN ReactionType = 0 THEN count(*)END, " +
                            " CASE WHEN ReactionType = 21 THEN count(*)END, " +
                            " CASE WHEN ReactionType = 22 THEN count(*)END ";
            strSQL += " From EquipmentZone as ez, SensorZone as sz, SensorZoneHistory as szh, SensorReactionHistory as srh ";
            strSQL += (" where ez.ID = sz.EquipZoneID And sz.ID = szh.SensorID and ez.SiteID = " + m_nSiteID.ToString());
            strSQL += " And szh.ID = srh.SensorHistoryID And ReactionType in ( 0, 21, 22 ) And ez.ID <> 0 ";


            ArrayList arrResult = null;
            if (btnSelect == false)
            {
                DateTime dtNowDate = DateTime.Now;
                DateTime dtBeforeDate = DateTime.Now;
                dtBeforeDate = DateTime.Now.AddMonths(-6);

                strNowDate = string.Format("{0} {1}:{2}:{3}", dtNowDate.ToShortDateString(), dtNowDate.Hour, dtNowDate.Minute, dtNowDate.Second);
                strBeforeDate = string.Format("{0} {1}:{2}:{3}", dtBeforeDate.ToShortDateString(), dtBeforeDate.Hour, dtBeforeDate.Minute, dtBeforeDate.Second);

                strSQL += "And szh.Time Between'" + strBeforeDate + "'And '" + strNowDate + "'";
                strSQL += "group by sz.Type, ez.ID, ReactionType";

                arrResult = GetSensorZoneResult(webDB, strSQL);

            }
            //버튼이 눌리면(날짜가들어오면)
            else
            {
                strSQL += "And szh.Time Between'" + strBeforeDate + "'And '" + strNowDate + "'";
                strSQL += "group by sz.Type, ez.ID, ReactionType";

                arrResult = GetSensorZoneResult(webDB, strSQL, true);               
            }

			if (arrResult == null)
				return;

   
			int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 11; i += 12)
            {
				
                int nType = WebDBManager.GetIntField(arrResult[i].ToString(), - 1);
                int nZoneID = WebDBManager.GetIntField(arrResult[i+1].ToString(), -1);
                string strBuildingGroupName = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strBuildingName = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strAddFloor = WebDBManager.GetStringField(arrResult[i + 5], "0.0");
                float nFloorIndex = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nReactionCount = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nMulFunctionCount = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);
                int nFireCount = WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
				int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1); 

                //Null값은 0으로 처리
                nReactionCount = (nReactionCount == -1) ? nReactionCount = 0 : nReactionCount;
                nMulFunctionCount = (nMulFunctionCount == -1) ? nMulFunctionCount = 0 : nMulFunctionCount;
                nFireCount = (nFireCount == -1) ? nFireCount = 0 : nFireCount;



                SensorReactionHistory reactionHistory = new SensorReactionHistory();
                if (m_dicReactionHistory.ContainsKey(nZoneID))
                {
                    reactionHistory = m_dicReactionHistory[nZoneID];
                    reactionHistory.ReactionCount += nReactionCount;
                    reactionHistory.MulFunctionCount += nMulFunctionCount;
                    reactionHistory.FireCount += nFireCount;

                    m_dicReactionHistory[nZoneID] = reactionHistory;
                }
                else
                {
                    reactionHistory.Type = nType;
                    reactionHistory.ZoneID = nZoneID;
                    reactionHistory.BuildingGroupName = strBuildingGroupName;
                    reactionHistory.BuildingName = strBuildingName;
                    reactionHistory.ZoneName = strZoneName;
                    reactionHistory.FloorIndex = nFloorIndex;
                    reactionHistory.AddFloor = strAddFloor;
                    reactionHistory.ReactionCount = nReactionCount;
                    reactionHistory.MulFunctionCount = nMulFunctionCount;
                    reactionHistory.FireCount = nFireCount;
                    reactionHistory.BuildingID = nBuildingID;
                    reactionHistory.EquipID = nEquipZoneID;

                    m_dicReactionHistory[nZoneID] = reactionHistory;
                }
            }
        }
    }
}
