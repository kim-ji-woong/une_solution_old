using System;
using System.Collections.Generic;
using System.Text;

namespace SOPManager.Model.Sop.Component
{
    public class Receiver
    {
        public enum TeamDataType { TemporaryNormalTeam = 0, TemporaryEmergencyTeam, RegularTeam, None };

        private int m_nTeamType = (int)TeamDataType.None;
        private int m_nTeamID = -1;

        public int TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public Receiver()
        {
        }

        public Receiver(int nTeamType, int nTeamID)
        {
            if (nTeamType >= (int)TeamDataType.TemporaryNormalTeam && nTeamType <= (int)TeamDataType.RegularTeam)
                m_nTeamType = nTeamType;
            else
                m_nTeamType = (int)TeamDataType.None;

            m_nTeamID = nTeamID;
        }
    }
}
