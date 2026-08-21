using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string [] values = szIDList.Split(',');
            foreach( string szID in values)
            {               
                if(int.TryParse(szID, out nTemp))
                {
                    arResult.Add(nTemp);
                }
            }
            return arResult;
        }

        public void ZoneSubmit(ArrayList arEquipZoneList,ArrayList arZoneList, DateTime dtStart, DateTime dtEnd)
        {
            m_arHistoryData.Clear();

            string strNowDate = "";
            string strBeforeDate = string.Format("{0} {1}:{2}:{3}", dtStart.ToShortDateString(), "00", "00", "00");

            //검색에 오늘날짜가 들어가면 현재 시간까지만 검사
            if (dtEnd.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            }
            else//아니면 23시 59분59분까지 검사
            {
                strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), 23, 59, 59);
            }

            string szEquipZoneList = MakeEquipZoneList(arEquipZoneList);
            string szZoneList = MakeZoneList(arZoneList);

            string szSQL =  "SELECT HS.ID, HS.SensorHistoryID, HS.ReactionHistoryID, HS.CompanyMemberIDList,HS.ExternalCompanyMemberIDList" +
                ", HS.SMSMessage,HS.SendType, SR.Time, SR.param1, SR.param2 FROM SDMSSMSHistory as HS, SensorReactionHistory as SR WHERE SR.ID = HS.ReactionHistoryID AND (" +
                // 자동 탐지인경우 Equipzone list
                "SR.ID IN (SELECT ID FROM SensorReactionHistory WHERE param1 in ({0}) and param2 <> 0 and ( ReactionType = 11 ) and Time Between '{1}' and '{2}') OR " +
                // 수동 신고인 경우 ZoneList
                "SR.ID IN (SELECT ID FROM SensorReactionHistory WHERE param1 in ({3}) and param2 = 0 and ( ReactionType = 11 ) and Time Between '{1}' and '{2}')" +
                ") order by HS.ID desc";

            string szSQL1 = string.Format(szSQL, szEquipZoneList, strBeforeDate, strNowDate, szZoneList);

            ArrayList arResult = m_DBMgr.GetResultData(szSQL1, 0);
            if (arResult == null)
                return;

            for(int i = 0 ; i < arResult.Count - 9 ; i += 10)
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
                if( nSensorZoneID == 0) // 수동신고인경우 param1 이 zoneid, 자동인경우 equipzoneid
                {
                    data.Zone = ZoneManager.Instance.GetZone(nEquipZoneID);
                }
                else
                {
                    data.EquipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                    data.Zone = data.EquipZone.LinkedZone;
                }
                m_arHistoryData.Add(data);
            }
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
