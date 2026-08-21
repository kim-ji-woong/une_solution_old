using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SDMS
{
    public class SendSMSCommand : SensorHistoryCommand
    {
        public class Receiver
        {
            // 1) ID가 양수이고 AllMember가 false
            //    => 정직원
            // 2) ID가 양수이고 AllMember가 true
            //    => ID는 ExternalTeam Table의 ID, 특정 협력업체 전체를 의미
            // 3) ID가 음수이고 AllMember가 false
            //    => 협력업체 직원, ID는 ExternalCompanyMember Table의 ID
            // 4) ID가 음수이고 AllMember가 true
            //    => 정직원 전체
            private int m_nID = -1;
            private bool m_isAllMember = false;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public bool AllMember
            {
                get { return m_isAllMember; }
                set { m_isAllMember = value; }
            }

            public bool IsSame(int nID, bool allMember)
            {
                return m_nID == nID && m_isAllMember == allMember;
            }
        }

        private string m_strMessage = "";
        // 정직원들은 양의 정수값(CompanyMember Table의 ID), 협력업체 직원들은 음의 정수값(ExternalCompanyMember Table의 ID)
        // 전체 정직원은 ALL, 전체 협력업체 직원들은 all(ExternalTeam Table의 ID)
        private ArrayList m_arrReceivers = new ArrayList(); 

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public SendSMSCommand()
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.SEND_SMS;
            Time = DateTime.Now;
        }

        public SendSMSCommand(int nSensorHistoryID)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.SEND_SMS;
            Time = DateTime.Now;
            SensorHistoryID = nSensorHistoryID;
        }

        public SendSMSCommand(int nSensorHistoryID, DateTime time)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.SEND_SMS;
            Time = time;
            SensorHistoryID = nSensorHistoryID;
        }

        public SendSMSCommand(int nSensorHistoryID, DateTime time, string strMessage)
        {
            ReactionType = (int)SensorHistoryCommand._ReactionType.SEND_SMS;
            Time = time;
            SensorHistoryID = nSensorHistoryID;
            m_strMessage = strMessage;
        }

        // 1) ID가 양수이고 AllMember가 false
        //    => 정직원
        // 2) ID가 양수이고 AllMember가 true
        //    => ID는 ExternalTeam Table의 ID, 특정 협력업체 전체를 의미
        // 3) ID가 음수이고 AllMember가 false
        //    => 협력업체 직원, ID는 ExternalCompanyMember Table의 ID
        // 4) ID가 음수이고 AllMember가 true
        //    => 정직원 전체
        public void AddReceiver(int nID, bool allMember)
        {
            foreach (Receiver receiver in m_arrReceivers)
            {
                if (receiver.IsSame(nID, allMember))
                    return;
            }

            Receiver _receiver = new Receiver();
            _receiver.ID = nID;
            _receiver.AllMember = allMember;

            m_arrReceivers.Add(_receiver);
        }

        public override bool MakeInsertQuery(int nID, out string strQuery)
        {
            strQuery = "";

            if (SensorHistoryID < 0)
                return false;

            if (m_strMessage.Length == 0)
                return false;

            if (m_arrReceivers.Count == 0)
                return false;

            strQuery = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', NULL)",
                nID, SensorHistoryID, ReactionType, DBUtility.WebDBManager.MakeDateTimeString(Time),
                m_strMessage, MakeReceiverString());

            return true;
        }

        private string MakeReceiverString()
        {
            string strReceivers = "", strReceiver = "";

            foreach (Receiver receiver in m_arrReceivers)
            {
                if (receiver.ID > 0 && receiver.AllMember)
                    strReceiver = string.Format("all({0})", receiver.ID);
                else if (receiver.ID > 0 && !receiver.AllMember)
                    strReceiver = receiver.ID.ToString();
                else if (receiver.ID < 0 && receiver.AllMember)
                    strReceiver = "ALL";
                else// if (receiver.ID < 0 && !receiver.AllMember)
                    strReceiver = receiver.ID.ToString();

                if (strReceivers.Length == 0)
                    strReceivers = strReceiver;
                else
                    strReceivers += ", " + strReceiver;
            }

            return strReceivers;
        }
    }
}
