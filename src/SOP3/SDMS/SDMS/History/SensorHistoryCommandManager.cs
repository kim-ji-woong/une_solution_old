using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SDMS
{
    public class SensorHistoryCommandManager
    {
        private static SensorHistoryCommandManager m_instance = null;
        public static SensorHistoryCommandManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new SensorHistoryCommandManager();

                return m_instance;
            }
        }

        public bool AddHistory(SensorHistoryCommand cmd, int nID = -1)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            if (nID < 0)
            {
                string strSQL = "select max(id) from SensorReactionHistory";
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return false;

                if (arrResult.Count == 0)
                    nID = 1;
                else
                    nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
            }

            string strSQL2;
            if (!cmd.MakeInsertQuery(nID, out strSQL2))
                return false;

            return dbMgr.GetResultData(strSQL2, 0) != null;
        }
    }
}
