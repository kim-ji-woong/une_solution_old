using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDMS
{
    public class SetResultCommand : SensorHistoryCommand
    {
        private bool m_isFire = false;

        public bool IsFire
        {
            get { return m_isFire; }
            set { m_isFire = value; }
        }

        public SetResultCommand(bool isFire)
        {
            m_isFire = isFire;
            this.ReactionType = isFire ? (int)SensorHistoryCommand._ReactionType.SET_FIRE : (int)SensorHistoryCommand._ReactionType.SET_MALFUNCTION;
            Time = DateTime.Now;
        }

        public SetResultCommand(bool isFire, int nSensorHistoryID)
        {
            m_isFire = isFire;
            this.ReactionType = isFire ? (int)SensorHistoryCommand._ReactionType.SET_FIRE : (int)SensorHistoryCommand._ReactionType.SET_MALFUNCTION;
            Time = DateTime.Now;
            SensorHistoryID = nSensorHistoryID;
        }

        public SetResultCommand(bool isFire, int nSensorHistoryID, DateTime time)
        {
            m_isFire = isFire;
            this.ReactionType = isFire ? (int)SensorHistoryCommand._ReactionType.SET_FIRE : (int)SensorHistoryCommand._ReactionType.SET_MALFUNCTION;
            Time = time;
            SensorHistoryID = nSensorHistoryID;
        }

        public override bool MakeInsertQuery(int nID, out string strQuery)
        {
            strQuery = "";

            if (SensorHistoryID < 0)
                return false;

            strQuery = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', NULL, NULL, NULL)",
                nID, SensorHistoryID, ReactionType, DBUtility.WebDBManager.MakeDateTimeString(Time));

            return true;
        }
    }
}
