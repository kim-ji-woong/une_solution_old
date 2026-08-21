using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSMSensorSimulator2
{
    public class SensorReactionLog
    {
        public enum ReactionType
        {
            BEGIN_STATUS = 0,
            RUN_BROADCAST = 10,
            SEND_SMS = 11,
            MALFUNCTION = 21,
            NOTIFY_FIRE = 22,
            IGNORE_FIRE = 23,
            TRAINNING_FIRE = 24,
            RUN_SOP = 30,
            RUN_N_CANCEL_SOP = 31,
            FINISH_SOP = 32,
            IGNORE_SOP = 33,
            END_STATUS = 50,
            BEGIN_PSM_STATUS = 60,
            IGNORE_PSM_DETECT = 61,
            CHANGE_PSM_ALARM_DEPTH = 62,
            END_PSM_STATUS = 70,
            ETC = 100
        }

        private static Dictionary<int, ReactionType> m_dicReactionType = null;

        private int m_nID = -1;
        private int m_nSensorHistoryID = -1;
        private ReactionType m_type = ReactionType.ETC;
        private DateTime m_time = new DateTime();
        private string m_strMessage = "";
        private string m_strParam1 = "";
        private string m_strParam2 = "";
        private string m_strParam3 = "";
        private string m_strParam4 = "";
        private string m_strParam5 = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int SensorHistoryID
        {
            get { return m_nSensorHistoryID; }
            set { m_nSensorHistoryID = value; }
        }

        public ReactionType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public DateTime LogTime
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string Param1
        {
            get { return m_strParam1; }
            set { m_strParam1 = value; }
        }

        public string Param2
        {
            get { return m_strParam2; }
            set { m_strParam2 = value; }
        }

        public string Param3
        {
            get { return m_strParam3; }
            set { m_strParam3 = value; }
        }
        public string Param4
        {
            get { return m_strParam4; }
            set { m_strParam4 = value; }
        }

        public string Param5
        {
            get { return m_strParam5; }
            set { m_strParam5 = value; }
        }

        public int GetBytesCount()
        {
            int nBytesCount = sizeof(int) * 3;  // ID, SensorHistoryID, Type
            nBytesCount += sizeof(long);        // LogTime
            // Message, Param1, Param2
            nBytesCount += (m_strMessage.Length + m_strParam1.Length + m_strParam2.Length) * sizeof(char);
            nBytesCount += (m_strParam3.Length + m_strParam4.Length + m_strParam5.Length) * sizeof(char);
            // FieldCount : 7
            return nBytesCount + 5 * 7 + 2;
        }

        public static void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
        {
            int nLength = bytesSrc.Length;

            //TcpLib2.ConnectionLog.Instance.WriteLine(string.Format("bytesSrc length : {0}, bytesDest length : {1}, nDestOffset : {2}, nLength : {3}",
            //	bytesSrc.Length, bytesDest.Length, nDestOffset, nLength));

            System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
            nDestOffset += nLength;
        }

        public static ReactionType ToReactionType(int nType, out bool isSuccess)
        {
            isSuccess = true;

            if (m_dicReactionType == null)
            {
                m_dicReactionType = new Dictionary<int, ReactionType>();

                foreach (ReactionType type in Enum.GetValues(typeof(ReactionType)))
                {
                    m_dicReactionType[(int)type] = type;
                }
            }

            ReactionType fType;

            if (m_dicReactionType.TryGetValue(nType, out fType))
                return fType;

            isSuccess = false;
            return ReactionType.ETC;
        }

        public SensorReactionLog Clone()
        {
            SensorReactionLog log = new SensorReactionLog();

            log.m_nID = m_nID;
            log.m_nSensorHistoryID = m_nSensorHistoryID;
            log.m_type = m_type;
            log.m_time = m_time;
            log.m_strMessage = m_strMessage;
            log.m_strParam1 = m_strParam1;
            log.m_strParam2 = m_strParam2;
            log.m_strParam3 = m_strParam3;
            log.m_strParam4 = m_strParam4;
            log.m_strParam5 = m_strParam5;
            return log;
        }
    }
}
