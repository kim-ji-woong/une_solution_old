using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDMS
{
    public abstract class SensorHistoryCommand
    {
        public enum _ReactionType { BEGIN_ALARAM = 0, RUN_BROADCAST = 10, SEND_SMS = 11, SET_MALFUNCTION = 21, SET_FIRE = 22, RUN_SOP = 30, ETC = 100 };
        private int m_nSensorHistoryID = -1;
        private int m_nReactionType = -1;
        private DateTime m_time;

        public int SensorHistoryID
        {
            get { return m_nSensorHistoryID; }
            set { m_nSensorHistoryID = value; }
        }

        public int ReactionType
        {
            get { return m_nReactionType; }
            set { m_nReactionType = value; }
        }

        public DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public abstract bool MakeInsertQuery(int nID, out string strQuery);
    }
}
