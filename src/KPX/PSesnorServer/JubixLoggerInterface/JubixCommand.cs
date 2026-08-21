using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JubixNetwork
{
    public class JubixCommand
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

        private string m_szCommandName = "";
        public string CommandName
        {
            get { return m_szCommandName; }
            set { m_szCommandName = value; }
        }

        private string m_szCommandValue = "";
        public string CommandValue
        {
            get { return m_szCommandValue; }
            set { m_szCommandValue = value; }
        }

        private int m_nOccurrenceType = 0;
        public int OccurrenceType
        {
            get { return m_nOccurrenceType; }
            set { m_nOccurrenceType = value; }
        }

        private string m_strComment = "작업종료로 인한 알람 해제";
        public string Comment
        {
            get { return m_strComment; }
            set { m_strComment = value; }
        }
        
    }
}
