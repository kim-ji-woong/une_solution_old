using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDMSAgent
{
    public enum CommandType { NONE = -1, AGENT_UPDATE = 0, GET_SERVICE_LIST, GET_FILE_LIST, UPDATE, GET_PROC_LIST, GET_ALL_PROC_LIST, DOWNLOAD, SDMS_UPDATE, SOP_SERVER_RESTART, SERVER_STATUS, FILE_COPY }

    public class CommandItem
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private CommandType m_cmdType = CommandType.NONE;
        public CommandType CmdType
        {
            get { return m_cmdType; }
            set { m_cmdType = value; }
        }

        private DateTime m_dtTimeStamp = new DateTime();
        public DateTime TimeStamp
        {
            get { return m_dtTimeStamp; }
            set { m_dtTimeStamp = value; }
        }

        // CommandType이 GET_FILE_LIST(2)일 때 사용
        private string m_strSearchPath = "";
        public string SearchPath
        {
            get { return m_strSearchPath; }
            set { m_strSearchPath = value; }
        } 

        private bool m_IsStop = false;
        public bool IsStop
        {
            get { return m_IsStop; }
            set { m_IsStop = value; }
        }

        private bool m_IsStopService = false;
        public bool IsStopService
        {
            get { return m_IsStopService; }
            set { m_IsStopService = value; }
        }

        private string m_strStopName = "";
        public string StopName
        {
            get { return m_strStopName; }
            set { m_strStopName = value; }
        }

        private bool m_IsUpdate = false;
        public bool IsUpdate
        {
            get { return m_IsUpdate; }
            set { m_IsUpdate = value; }
        }

        private string m_strUpdateName = "";
        public string UpdateName
        {
            get { return m_strUpdateName; }
            set { m_strUpdateName = value; }
        }

        private bool m_IsStart = false;
        public bool IsStart
        {
            get { return m_IsStart; }
            set { m_IsStart = value; }
        }

        private bool m_IsStartService = false;
        public bool IsStartService
        {
            get { return m_IsStartService; }
            set { m_IsStartService = value; }
        }

        private string m_strStartName = "";
        public string StartName
        {
            get { return m_strStartName; }
            set { m_strStartName = value; }
        }

        // 0:실패, 1:성공
        private int m_Result = 0;
        public int Result
        {
            get { return m_Result; }
            set { m_Result = value; }
        }
    }
}
