using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TankServer
{
    public class ModbusCommand
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private int m_nCommand = -1;
        public int Command
        {
            get { return m_nCommand; }
            set { m_nCommand = value; }
        }

        private DateTime time;
        public DateTime CreateTime
        {
            get { return time; }
            set { time = value; }
        }


        private DateTime execTime;
        public DateTime ExecTime
        {
            get { return execTime; }
            set { execTime = value; }
        }

        private int m_nCmdHistoryID = -1;
        public int HistoryID
        {
            get { return m_nCmdHistoryID; }
            set { m_nCmdHistoryID = value; }
        }

        private int m_nUserID = -1;
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        private int m_nPipeID = -1;
        public int PipeID
        {
            get { return m_nPipeID; }
            set { m_nPipeID = value; }
        }

        private int m_nTankID = -1;
        public int TankID
        {
            get { return m_nTankID; }
            set { m_nTankID = value; }
        }

        private int m_nAlarmHistoryID = -1;
        public int AlarmHistoryID
        {
            get { return m_nAlarmHistoryID; }
            set { m_nAlarmHistoryID = value; }
        }

        private int m_nAlarmOccurType = -1;
        public int AlarmOccurType
        {
            get { return m_nAlarmOccurType; }
            set { m_nAlarmOccurType = value; }
        }

        private string m_szAlarmComment = "";
        public string AlarmComment
        {
            get { return m_szAlarmComment; }
            set { m_szAlarmComment = value; }
        }

        private string m_szCommandValue = "";
        public string CommandValue
        {
            get { return m_szCommandValue; }
            set { m_szCommandValue = value; }
        }


        
    }
}
