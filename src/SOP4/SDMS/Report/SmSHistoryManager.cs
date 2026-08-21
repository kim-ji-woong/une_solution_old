using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
{
    internal class SmSHistoryManager
    {
        private DBUtility.WebDBManager m_DBMgr = null;

        private ArrayList m_arHistoryData = new ArrayList();
        public ArrayList HistoryData
        {
            get { return m_arHistoryData; }
        }

        internal SmSHistoryManager()
        {
            m_DBMgr = FormMain.Instance.DBManager;
        }

        private string MakeEquipZoneList(ArrayList arZoneList)
        {
            string szZoneList = "";
            int nCount = 1;
            foreach (EquipmentZone zone in arZoneList)
            {
                szZoneList += zone.ID.ToString();
                if (nCount != arZoneList.Count)
                    szZoneList += ",";
                nCount++;
            }
            return szZoneList;
        }

        private string MakeZoneList(ArrayList arZoneList)
        {
            string szZoneList = "";
            int nCount = 1;
            foreach (Zone zone in arZoneList)
            {
                szZoneList += zone.ID.ToString();
                if (nCount != arZoneList.Count)
                    szZoneList += ",";
                nCount++;
            }
            return szZoneList;
        }

        private ArrayList MakeArrayList(string szIDList)
        {
            ArrayList arResult = new ArrayList();

            int nTemp = -1;
            string[] values = szIDList.Split(',');
            foreach (string szID in values)
            {
                if (int.TryParse(szID, out nTemp))
                {
                    arResult.Add(nTemp);
                }
            }
            return arResult;
        }

        public void ZoneSubmit(ArrayList arEquipZoneList, ArrayList arZoneList, DateTime dtStart, DateTime dtEnd, UnE.Sensor.IFacility.FacilityType type)
        {
            m_arHistoryData.Clear();
            bool isPSM = false;
            bool isSecurity = false;
            if( type == UnE.Sensor.IFacility.FacilityType.Security_Sensor)
            {
                isSecurity = true;
                isPSM = false;
            }

            if (type == UnE.Sensor.IFacility.FacilityType.PSM_SENSOR)
            {
                isPSM = true;
                isSecurity = false;
            }

            bool allZones = arZoneList.Count == ZoneManager.Instance.DicZones.Count;

            string strNowDate = "";
            string strBeforeDate = dtStart.ToShortDateString();
            //string strBeforeDate = string.Format("{0} {1}:{2}:{3}", dtStart.ToShortDateString(), "00", "00", "00");

            //검색에 오늘날짜가 들어가면 현재 시간까지만 검사
            strNowDate = dtEnd.AddDays(1).ToShortDateString();
            //if (dtEnd.ToShortDateString() == DateTime.Now.ToShortDateString())
            //{
            //    strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            //}
            //else//아니면 23시 59분59분까지 검사
            //{
            //    strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), 23, 59, 59);
            //}

            string szEquipZoneList = MakeEquipZoneList(arEquipZoneList);
            string szZoneList = MakeZoneList(arZoneList);

            string szSQL = String.Empty;

            string strCondition1 = "param1 in ({0})", strCondition2 = "param1 in ({3})", strCondition3 = "'{1}' and '{2}'";

            if (allZones)
            {
                strCondition1 = "param1 <> ''";
                strCondition2 = "param1 <> ''";
                strCondition3 = "'{0}' and '{1}'";
            }

            if (isPSM == true)
            {
                //szSQL = "  SELECT";
                //szSQL += " HS.ID,";
                //szSQL += " HS.SensorHistoryID,";
                //szSQL += " HS.ReactionHistoryID,";
                //szSQL += " HS.CompanyMemberIDList,";
                //szSQL += " HS.ExternalCompanyMemberIDList,";
                //szSQL += " HS.SMSMessage,";
                //szSQL += " HS.SendType,";
                //szSQL += " SR.Time,";
                //szSQL += " SR.param1,";
                //szSQL += " SR.param2";
                //szSQL += " FROM SDMSSMSHistory AS HS";
                //szSQL += " INNER JOIN SensorZoneHistory AS SZH ON HS.SensorHistoryID = SZH.id";
                //szSQL += " INNER JOIN SensorZone AS SZ ON SZH.SensorID = SZ.ID AND SZ.Type = 11";
                //szSQL += " INNER JOIN SensorReactionHistory AS SR ON HS.ReactionHistoryID = SR.ID";
                //szSQL += " WHERE";
                //// 자동 탐지인경우 Equipzone list
                //szSQL += " SR.ID IN (";
                //szSQL += " SELECT ID";
                //szSQL += " FROM SensorReactionHistory";
                //szSQL += " WHERE param1 in ({0})";
                //szSQL += " AND param2 <> 0";
                //szSQL += " AND ( ReactionType = 11 )";
                //szSQL += " AND Time Between '{1}' and '{2}'";
                //szSQL += " )";
                //// 수동 신고인 경우 ZoneList
                //szSQL += " OR";
                //szSQL += " SR.ID IN (";
                //szSQL += " SELECT ID";
                //szSQL += " FROM SensorReactionHistory";
                //szSQL += " WHERE param1 in ({3})";
                //szSQL += " AND param2 = 0";
                //szSQL += " AND ( ReactionType = 11 )";
                //szSQL += " AND Time Between '{1}' and '{2}'";
                //szSQL += " )";
                //szSQL += " ORDER BY HS.ID DESC";

                szSQL = "SELECT HS.ID, HS.SensorHistoryID, HS.ReactionHistoryID, HS.CompanyMemberIDList,HS.ExternalCompanyMemberIDList" +
                ", HS.SMSMessage,HS.SendType, SR.Time, SR.param1, SR.param2 FROM SDMSSMSHistory as HS, SensorReactionHistory as SR WHERE SR.ID = HS.ReactionHistoryID AND (" +
                    // 자동 탐지인경우 Equipzone list
                "SR.ID IN (SELECT ID FROM SensorReactionHistory WHERE " + strCondition1 + " and param2 <> '0' and ( ReactionType = 11 ) and Time Between " + strCondition3 + ") OR " +
                    // 수동 신고인 경우 ZoneList
                "SR.ID IN (SELECT ID FROM SensorReactionHistory WHERE " + strCondition2 + " and param2 = '0' and ( ReactionType = 11 ) and Time Between " + strCondition3 + ")" +
                ") order by HS.ID desc";
            }
            else
            {    
                szSQL = "SELECT HS.ID, HS.SensorHistoryID, HS.ReactionHistoryID, HS.CompanyMemberIDList,HS.ExternalCompanyMemberIDList" +
                ", HS.SMSMessage,HS.SendType, SR.Time, SR.param1, SR.param2 FROM SDMSSMSHistory as HS, SensorReactionHistory as SR WHERE SR.ID = HS.ReactionHistoryID AND (" +
                    // 자동 탐지인경우 Equipzone list
                "SR.ID IN (SELECT ID FROM SensorReactionHistory WHERE " + strCondition1 + " and param2 <> '0' and ( ReactionType = 11 ) and Time Between " + strCondition3 + ") OR " +
                    // 수동 신고인 경우 ZoneList
                "SR.ID IN (SELECT ID FROM SensorReactionHistory WHERE " + strCondition2 + " and param2 = '0' and ( ReactionType = 11 ) and Time Between " + strCondition3 + ")" +
                ") order by HS.ID desc";
            }
          

            string szSQL1 = allZones ? string.Format(szSQL, strBeforeDate, strNowDate) : string.Format(szSQL, szEquipZoneList, strBeforeDate, strNowDate, szZoneList);

            //szSQL1 = "  SELECT HS.ID, HS.SensorHistoryID, HS.ReactionHistoryID, HS.CompanyMemberIDList, HS.ExternalCompanyMemberIDList, HS.SMSMessage, HS.SendType, SR.Time, SR.param1, SR.param2 FROM SDMSSMSHistory AS HS INNER JOIN SensorZoneHistory AS SZH ON HS.SensorHistoryID = SZH.id INNER JOIN SensorZone AS SZ ON SZH.SensorID = SZ.ID AND SZ.Type <> 11 INNER JOIN SensorReactionHistory AS SR ON HS.ReactionHistoryID = SR.ID WHERE SR.ID IN ( SELECT ID FROM SensorReactionHistory WHERE  param2 <> 0 AND ( ReactionType = 11 ) AND Time Between '2015-01-01 00:00:00' and '2016-03-10 13:7:56' ) OR SR.ID IN ( SELECT ID FROM SensorReactionHistory WHERE param1 in (120,121,122,123,110,111,112,113,124,125,126,127,128,129,130,131,132,444,445,133,134,135,136,137,138,139,140,141,446,447,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,301,300,20,22,30,377,378) AND param2 = 0 AND ( ReactionType = 11 ) AND Time Between '2015-01-01 00:00:00' and '2016-03-10 13:7:56' ) ORDER BY HS.ID DESC";

            ArrayList arResult = m_DBMgr.GetResultData(szSQL1, 0);
            if (arResult == null)
                return;

            ArrayList arTempSmsHistory = new ArrayList();
            ArrayList arTempValidSensorZoneHistoryID = new ArrayList();
            List<string> liSensorHistoryIDs = new List<string>();

            for (int i = 0; i < arResult.Count - 9; i += 10)
            {
                SmsHistory data = new SmsHistory();

                data.ID = DBUtility.WebDBManager.GetIntField(arResult[i].ToString(), -1);
                data.SensorHistoryID = DBUtility.WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);
                data.ReactionHistoryID = DBUtility.WebDBManager.GetIntField(arResult[i + 2].ToString(), -1);

                data.CompanyMemberList = MakeArrayList(arResult[i + 3].ToString());
                data.ExteanlMemberList = MakeArrayList(arResult[i + 4].ToString());
                data.Message = arResult[i + 5].ToString();

                data.IsAuto = (DBUtility.WebDBManager.GetIntField(arResult[i + 6].ToString(), -1) == 1);
                data.Time = DBUtility.WebDBManager.GetDateTimeField(arResult[i + 7].ToString(), DateTime.Now);
                int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arResult[i + 8].ToString(), -1);
                int nSensorZoneID = DBUtility.WebDBManager.GetIntField(arResult[i + 9].ToString(), -1);
                if (nSensorZoneID == 0) // 수동신고인경우 param1 이 zoneid, 자동인경우 equipzoneid
                {
                    data.Zone = ZoneManager.Instance.GetZone(nEquipZoneID);
                }
                else
                {
                    data.EquipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                    data.Zone = data.EquipZone.LinkedZone;
                }

                if (liSensorHistoryIDs.Contains(data.SensorHistoryID.ToString()) == false)
                    liSensorHistoryIDs.Add(data.SensorHistoryID.ToString());

                arTempSmsHistory.Add(data);                
            }

            string strSQL = "";
            string strSecurityTypes = "900,901,902,903,904,905,907,1001,1002,1003,1004,2100,2110,2200,2300,3000,3004,3008,4000";
            if( isSecurity == true)
            {               
                strSQL = "select SensorZoneHistory.id from SensorZoneHistory, SensorZone where SensorZoneHistory.SensorID = SensorZone.ID AND SensorZone.Type in (" + strSecurityTypes + " ) AND SensorZoneHistory.id in (" + String.Join(",", liSensorHistoryIDs.ToArray()) + ")";
            }
            else
            {

                if (isPSM== true)
                    strSQL = "select SensorZoneHistory.id from SensorZoneHistory, SensorZone where SensorZoneHistory.SensorID = SensorZone.ID AND SensorZone.Type " + (isPSM ? "=" : "<>") + " 11 AND SensorZoneHistory.id in (" + String.Join(",", liSensorHistoryIDs.ToArray()) + ")";
                else
                {
                    strSQL = "select SensorZoneHistory.id from SensorZoneHistory, SensorZone where SensorZoneHistory.SensorID = SensorZone.ID AND SensorZone.Type not in (11,"+strSecurityTypes+") AND SensorZoneHistory.id in (" + String.Join(",", liSensorHistoryIDs.ToArray()) + ")";
                }
            }

            arResult = m_DBMgr.GetResultData(strSQL, 0);
            if (arResult == null)
                return;

            for (int i = 0; i < arResult.Count; i++)
            {
                int nSensorZoneHistoryID = DBUtility.WebDBManager.GetIntField(arResult[i].ToString(), -1);

                if (arTempValidSensorZoneHistoryID.Contains(arTempValidSensorZoneHistoryID) == false)
                    arTempValidSensorZoneHistoryID.Add(nSensorZoneHistoryID);
            }

            foreach (SmsHistory item in arTempSmsHistory)
            {
                if (arTempValidSensorZoneHistoryID.Contains(item.SensorHistoryID))
                {
                    m_arHistoryData.Add(item);
                }
            }

            //m_arHistoryData.Add(data);

        }
        public void ZoneSubmitIntrusion(ArrayList arEquipZoneList, ArrayList arZoneList, DateTime dtStart, DateTime dtEnd)
        {
            m_arHistoryData.Clear();

            bool allZones = arZoneList.Count == ZoneManager.Instance.DicZones.Count;

            string strNowDate = "";
            string strBeforeDate = dtStart.ToShortDateString();
            //string strBeforeDate = string.Format("{0} {1}:{2}:{3}", dtStart.ToShortDateString(), "00", "00", "00");

            //검색에 오늘날짜가 들어가면 현재 시간까지만 검사
            strNowDate = dtEnd.AddDays(1).ToShortDateString();
            //if (dtEnd.ToShortDateString() == DateTime.Now.ToShortDateString())
            //{
            //    strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            //}
            //else//아니면 23시 59분59분까지 검사
            //{
            //    strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), 23, 59, 59);
            //}

            string szEquipZoneList = MakeEquipZoneList(arEquipZoneList);
            string szZoneList = MakeZoneList(arZoneList);

            string szSQL = String.Empty;

            string strCondition1 = "param1 in ({0})", strCondition2 = "param1 in ({3})", strCondition3 = "'{1}' and '{2}'";

            if (allZones)
            {
                strCondition1 = "param1 <> ''";
                strCondition2 = "param1 <> ''";
                strCondition3 = "'{0}' and '{1}'";
            } 

            szSQL = "SELECT HS.ID, HS.SensorHistoryID, HS.ReactionHistoryID, HS.CompanyMemberIDList,HS.ExternalCompanyMemberIDList" +
            ", HS.SMSMessage,HS.SendType, SR.Time, SR.param1, SR.param2 FROM SDMSSMSHistory as HS, SensorReactionHistory as SR WHERE SR.ID = HS.ReactionHistoryID AND (" +
                // 자동 탐지인경우 Equipzone list
            "SR.ID IN (SELECT ID FROM SensorReactionHistory WHERE " + strCondition1 + " and param2 <> 0 and ( ReactionType = 11 ) and Time Between " + strCondition3 + ") OR " +
                // 수동 신고인 경우 ZoneList
            "SR.ID IN (SELECT ID FROM SensorReactionHistory WHERE " + strCondition2 + " and param2 = 0 and ( ReactionType = 11 ) and Time Between " + strCondition3 + ")" +
            ") order by HS.ID desc"; 
            
            string szSQL1 = allZones ? string.Format(szSQL, strBeforeDate, strNowDate) : string.Format(szSQL, szEquipZoneList, strBeforeDate, strNowDate, szZoneList);
             
            ArrayList arResult = m_DBMgr.GetResultData(szSQL1, 0);
            if (arResult == null)
                return;

            ArrayList arTempSmsHistory = new ArrayList();
            ArrayList arTempValidSensorZoneHistoryID = new ArrayList();
            List<string> liSensorHistoryIDs = new List<string>();

            for (int i = 0; i < arResult.Count - 9; i += 10)
            {
                SmsHistory data = new SmsHistory();

                data.ID = DBUtility.WebDBManager.GetIntField(arResult[i].ToString(), -1);
                data.SensorHistoryID = DBUtility.WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);
                data.ReactionHistoryID = DBUtility.WebDBManager.GetIntField(arResult[i + 2].ToString(), -1);

                data.CompanyMemberList = MakeArrayList(arResult[i + 3].ToString());
                data.ExteanlMemberList = MakeArrayList(arResult[i + 4].ToString());
                data.Message = arResult[i + 5].ToString();

                data.IsAuto = (DBUtility.WebDBManager.GetIntField(arResult[i + 6].ToString(), -1) == 1);
                data.Time = DBUtility.WebDBManager.GetDateTimeField(arResult[i + 7].ToString(), DateTime.Now);
                int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arResult[i + 8].ToString(), -1);
                int nSensorZoneID = DBUtility.WebDBManager.GetIntField(arResult[i + 9].ToString(), -1);
                if (nSensorZoneID == 0) // 수동신고인경우 param1 이 zoneid, 자동인경우 equipzoneid
                {
                    data.Zone = ZoneManager.Instance.GetZone(nEquipZoneID);
                }
                else
                {
                    data.EquipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                    data.Zone = data.EquipZone.LinkedZone;
                }

                if (liSensorHistoryIDs.Contains(data.SensorHistoryID.ToString()) == false)
                    liSensorHistoryIDs.Add(data.SensorHistoryID.ToString());

                arTempSmsHistory.Add(data);
            }

            string strTypeIDs = (int)IFacility.FacilityType.Intrusion_S1 + "," + (int)IFacility.FacilityType.Loiter_S1 + ","
                    + (int)IFacility.FacilityType.Collapse_S1 + "," + (int)IFacility.FacilityType.Theft_S1 + "," + (int)IFacility.FacilityType.Neglect_S1 + ","
                    + (int)IFacility.FacilityType.VirtualFence_S1 + "," + (int)IFacility.FacilityType.EmergencyBell_S1 + "," + (int)IFacility.FacilityType.GeneralIntrusionT1_S1 + ","
                    + (int)IFacility.FacilityType.GeneralIntrusionT2_S1 + "," + (int)IFacility.FacilityType.InternalIntrusionT3_S1 + "," + (int)IFacility.FacilityType.VaultIntrusionT4_S1 + ","
                    + (int)IFacility.FacilityType.CustomerEmergencyC1_S1 + "," + (int)IFacility.FacilityType.CustomerEmergencyC2_S1 + "," + (int)IFacility.FacilityType.RescueQQ_S1 + ","
                    + (int)IFacility.FacilityType.GasG1_S1 + "," + (int)IFacility.FacilityType.BlackoutAbnormalityU1_S1 + "," + (int)IFacility.FacilityType.LeakAbnormalityU4_S1 + ","
                    + (int)IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1 + "," + (int)IFacility.FacilityType.ExternalAlarmBell; 
            string strSQL = "select SensorZoneHistory.id from SensorZoneHistory, SensorZone where SensorZoneHistory.SensorID = SensorZone.ID AND SensorZone.Type IN (" + strTypeIDs + ") AND SensorZoneHistory.id in (" + String.Join(",", liSensorHistoryIDs.ToArray()) + ")";

            arResult = m_DBMgr.GetResultData(strSQL, 0);
            if (arResult == null)
                return;

            for (int i = 0; i < arResult.Count; i++)
            {
                int nSensorZoneHistoryID = DBUtility.WebDBManager.GetIntField(arResult[i].ToString(), -1);

                if (arTempValidSensorZoneHistoryID.Contains(arTempValidSensorZoneHistoryID) == false)
                    arTempValidSensorZoneHistoryID.Add(nSensorZoneHistoryID);
            }

            foreach (SmsHistory item in arTempSmsHistory)
            {
                if (arTempValidSensorZoneHistoryID.Contains(item.SensorHistoryID))
                {
                    m_arHistoryData.Add(item);
                }
            }

            //m_arHistoryData.Add(data);

        }
    }

    internal class SmsHistory
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private int m_nSensorHistoryID = -1;
        public int SensorHistoryID
        {
            get { return m_nSensorHistoryID; }
            set { m_nSensorHistoryID = value; }
        }

        private int m_nReactionHistoryID = -1;
        public int ReactionHistoryID
        {
            get { return m_nReactionHistoryID; }
            set { m_nReactionHistoryID = value; }
        }

        private ArrayList m_arCompanyMemberList = new ArrayList();
        public ArrayList CompanyMemberList
        {
            get { return m_arCompanyMemberList; }
            set { m_arCompanyMemberList = value; }
        }

        private ArrayList m_arExteanlMemberList = new ArrayList();
        public ArrayList ExteanlMemberList
        {
            get { return m_arExteanlMemberList; }
            set { m_arExteanlMemberList = value; }
        }

        private DateTime m_dtTime;
        public DateTime Time
        {
            get { return m_dtTime; }
            set { m_dtTime = value; }
        }

        private string m_szMsg = "";
        public string Message
        {
            get { return m_szMsg; }
            set { m_szMsg = value; }
        }

        private bool m_bAuto = false;
        public bool IsAuto
        {
            get { return m_bAuto; }
            set { m_bAuto = value; }
        }

        private Zone m_Zone = null;
        public Zone Zone
        {
            get { return m_Zone; }
            set { m_Zone = value; }
        }

        private EquipmentZone m_EquipZone = null;
        public EquipmentZone EquipZone
        {
            get { return m_EquipZone; }
            set { m_EquipZone = value; }
        }


    }
}
