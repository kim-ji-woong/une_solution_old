using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;

namespace AgentCommander
{
    public class Command
    {
        public const int AGENT_UPDATE = 0;
        public const int SCREEN_CAPTURE = 1;
        public const int CLIENT_UPDATE = 2;
        public const int SERVER_UPDATE = 3;
        public const int TANK_SERVER_UPDATE = 4;
        public const int PUSH_SERVER_UPDATE = 5;
        public const int USER_ACCEPTANCE_UPDATE = 6;
        public const int JSP_UPDATE = 7;
        public const int CHECK_STATUS = 8;
        public const int SERVER_DLL_UPDATE = 9;
        public const int ZIP_FILE_UPDATE = 10;
        public const int NORMAL_FILE_UPDATE = 11;
        public const int SEARCH_FOLDER = 12;

        public const int PROCESS_KILL = 1000;
        public const int PROCESS_START = 2000;
        public const int SERVICE_STOP = 3000;
        public const int SERVICE_START = 4000;

        public const int FILE_UPDATE = 100;
        public const int SERVICE_FILE_UPDATE = 200;         

        public static bool ProcessUpdate(WebDBManager dbMgr, int command, bool isAreaType = false)
        {
            int nID = 0;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = string.Empty;
            if (isAreaType)
            {
                //휴게실도 update 해야하는가
                nID = GetMaxID(dbMgr) + 1;
                strSQL = string.Format("Insert into AgentCommand (ID, Command, TimeStamp, AreaType) values ({0}, {1}, '{2}', 1)", nID, command, strTime);
                dbMgr.GetResultData(strSQL, 0);
            }

            nID = GetMaxID(dbMgr) + 1;
            strSQL = string.Format("Insert into AgentCommand (ID, Command, TimeStamp, AreaType) values ({0}, {1}, '{2}', 0)", nID, command, strTime); 
            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        public static bool FileUpdate(WebDBManager dbMgr, int command, string fileName, bool isAreaType = false)
        {
            int nID = 0;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = string.Empty;
            if (isAreaType)
            {
                //휴게실도 update 해야하는가
                nID = GetMaxID(dbMgr) + 1;
                strSQL = string.Format("Insert into AgentCommand (ID, Command, TimeStamp, AreaType, FileName) values ({0}, {1}, '{2}', 1, '{3}')", nID, command, strTime, fileName);
                dbMgr.GetResultData(strSQL, 0);
            }

            nID = GetMaxID(dbMgr) + 1;
            strSQL = string.Format("Insert into AgentCommand (ID, Command, TimeStamp, AreaType, FileName) values ({0}, {1}, '{2}', 0, '{3}')", nID, command, strTime, fileName);
            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        public static bool ProcessScreenCapture(WebDBManager dbMgr)
        {
            int nID = GetMaxID(dbMgr) + 1;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = string.Format("Insert into AgentCommand (ID, Command, TimeStamp, AreaType) values ({0}, {1}, '{2}', 0)", nID, SCREEN_CAPTURE, strTime);
            dbMgr.GetResultData(strSQL, 0);

            nID = GetMaxID(dbMgr) + 1;
            strSQL = string.Format("Insert into AgentCommand (ID, Command, TimeStamp, AreaType) values ({0}, {1}, '{2}', 1)", nID, SCREEN_CAPTURE, strTime);

            return dbMgr.GetResultData(strSQL, 0) != null;
        } 

        private static int GetMaxID(WebDBManager dbMgr)
        {
            string strSQL = "Select max(ID) from AgentCommand";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            return id == null ? 0 : id.Data;
        }
    }
}
