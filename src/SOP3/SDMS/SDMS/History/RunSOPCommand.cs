using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDMS
{
    public class RunSOPCommand : SensorHistoryCommand
    {
        private int m_nActionStepHistoryID = -1;

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public RunSOPCommand()
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.RUN_SOP;
            Time = DateTime.Now;
        }

        public RunSOPCommand(int nActionStepHistoryID)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.RUN_SOP;
            Time = DateTime.Now;
            m_nActionStepHistoryID = nActionStepHistoryID;
        }

        public RunSOPCommand(int nActionStepHistoryID, int nSensorHistoryID)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.RUN_SOP;
            Time = DateTime.Now;
            m_nActionStepHistoryID = nActionStepHistoryID;
            SensorHistoryID = nSensorHistoryID;
        }

        public RunSOPCommand(int nActionStepHistoryID, int nSensorHistoryID, DateTime time)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.RUN_SOP;
            Time = time;
            m_nActionStepHistoryID = nActionStepHistoryID;
            SensorHistoryID = nSensorHistoryID;
        }

        public override bool MakeInsertQuery(int nID, out string strQuery)
        {
            strQuery = "";

            if (SensorHistoryID < 0)
                return false;

            if (m_nActionStepHistoryID < 0)
                return false;

            strQuery = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', NULL, '{4}', NULL)",
                nID, SensorHistoryID, ReactionType, DBUtility.WebDBManager.MakeDateTimeString(Time),
                m_nActionStepHistoryID.ToString());

            return true;
        }
    }
}
