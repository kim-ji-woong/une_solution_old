using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace AgentFactory.Agent
{
    public class ProcessAgent : BaseProcessAgent
    {
        public override List<ClientMessage> PrevNewAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            string msg = GetMessage(dbMgr, alarm.Message, alarm.ReactionHistoryParam1);
            alarm.Message = msg;

            return base.PrevNewAlarm(dbMgr, alarm, alarmManager);
        }
        public override List<ClientMessage> PrevClearAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            string msg = GetMessage(dbMgr, alarm.Message, alarm.ReactionHistoryParam1);
            alarm.Message = msg;

            return base.PrevClearAlarm(dbMgr, alarm, alarmManager);
        }

        public string GetMessage(DirectDBManager dbMgr, string message, string equipzoneID)
        {
            int nIndex = message.LastIndexOf("]");
            if (nIndex < 0)
                return message;

            string lastMsg = message.Substring(nIndex + 1);
            
            string frontMsg = message.Substring(0, nIndex + 1);

            string strTag = "";
            bool isReal = true;
            int nIndex2 = message.LastIndexOf("[");
            if (nIndex2 > 0)
            {
                strTag = message.Substring(0, nIndex2);
                if (strTag.Replace("[", "").Replace("]", "") == "테스트")
                    isReal = false;
            }

            //DirectDBManager dbMgr = m_dbMgr.Clone();

            if (isReal)
            {
                //if (dbMgr.Connect())
                {
                    string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='TranningMode' and SiteID = " + dbMgr.SiteID.ToString();
                    ArrayList arrResult = dbMgr.GetResultData(strSQL);

                    if (arrResult == null || arrResult.Count == 0)
                        strTag = "";

                    VariousData<int> value = WebDBManager.GetIntField(arrResult[0].ToString());

                    if (value != null && value.Data == 1)
                    {
                        strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='HeaderMsg' and SiteID = " + dbMgr.SiteID.ToString();
                        arrResult = dbMgr.GetResultData(strSQL);

                        if (arrResult == null || arrResult.Count == 0)
                            strTag = "[훈련상황]";

                        string strTag2 = WebDBManager.GetStringField(arrResult[0]);

                        if (strTag2 == null)
                            strTag = "[훈련상황]";

                        strTag = "[" + strTag + "]";
                    }
                }
            }

            string strEqName = "";
            string strFloorIndex = "";

            
            //if (dbMgr.Connect())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT eq.DisplayText, z.DisplayText ");
                sb.Append("  FROM EquipmentZone as eq, Zone as z ");
                sb.Append(" WHERE eq.LinkedZoneIDList = Convert(nvarchar(10), z.ID) ");
                sb.AppendFormat(" AND eq.ID = {0} ", equipzoneID);

                ArrayList arrResult = dbMgr.GetResultData(sb.ToString());
                
                if (arrResult == null || arrResult.Count != 2)
                    return message;

                strEqName = DBUtility2.WebDBManager.GetStringField(arrResult[0]);
                strFloorIndex = DBUtility2.WebDBManager.GetStringField(arrResult[1]);

            }

            string displayName = frontMsg.Substring(nIndex2).Replace("[", "").Replace("]", "");
            if (displayName == strEqName)
            {
                displayName = "[" + strFloorIndex + " " + displayName + "]";
            }

            return strTag + displayName + lastMsg;
        }
    }
}
