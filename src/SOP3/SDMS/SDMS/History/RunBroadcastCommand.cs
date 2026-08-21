using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDMS
{
    public class RunBroadcastCommand : SensorHistoryCommand
    {
        private string m_strMessage = "";

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public RunBroadcastCommand()
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.RUN_BROADCAST;
            Time = DateTime.Now;
        }

        public RunBroadcastCommand(int nSensorHistoryID)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.RUN_BROADCAST;
            Time = DateTime.Now;
            SensorHistoryID = nSensorHistoryID;
        }

        public RunBroadcastCommand(int nSensorHistoryID, DateTime time)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.RUN_BROADCAST;
            Time = time;
            SensorHistoryID = nSensorHistoryID;
        }

        public RunBroadcastCommand(int nSensorHistoryID, DateTime time, string strMessage)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.RUN_BROADCAST;
            Time = time;
            SensorHistoryID = nSensorHistoryID;
            m_strMessage = strMessage;
        }

        public override bool MakeInsertQuery(int nID, out string strQuery)
        {
            strQuery = "";

            if (SensorHistoryID < 0)
                return false;

            if (m_strMessage.Length == 0)
                return false;

            strQuery = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', '{4}', NULL, NULL)",
                nID, SensorHistoryID, ReactionType, DBUtility.WebDBManager.MakeDateTimeString(Time),
                m_strMessage);

            return true;
        }
    }
}
