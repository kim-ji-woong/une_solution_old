using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDMS
{
    public class BeginAlarmCommand : SensorHistoryCommand
    {
        public BeginAlarmCommand()
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.BEGIN_ALARAM;
            Time = DateTime.Now;
        }

        public BeginAlarmCommand(int nSensorHistoryID)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.BEGIN_ALARAM;
            Time = DateTime.Now;
            SensorHistoryID = nSensorHistoryID;
        }

        public BeginAlarmCommand(int nSensorHistoryID, DateTime time)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.BEGIN_ALARAM;
            SensorHistoryID = nSensorHistoryID;
            Time = time;
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
