using System;

namespace SDMS.Model.Broadcast
{
    public class State : IIDObject
    {
        public enum Fields { ID, HeartBeat, BState, SiteID };

        public enum SpeechState
        {
            NONE = 0,
            STANDBY = 1,
            PLAY = 2,
            STOP = 3,
            PAUSE = 4,
            REPEAT = 5
        }

        private int m_nID = -1;
        private DateTime m_heartBeat = new DateTime();
        // SpeechState
        private int m_nBState = (int)SpeechState.NONE;
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public DateTime HeartBeat
        {
            get { return m_heartBeat; }
            set { m_heartBeat = value; }
        }

        // SpeechState
        public int BState
        {
            get { return m_nBState; }
            set { m_nBState = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public static string TableName
        {
            get { return "SdmsBroadcastState"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }
    }
}
