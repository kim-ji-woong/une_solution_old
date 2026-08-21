using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace LostArticle
{
    public class Status
    {
        private int m_nID = -1;
        private int m_nBeginArticleID = -1;
        private int m_nBeginHistoryMessageID = -1;
        private int m_nDeadCount = -1;
        private int m_nInjuryCount = -1;
        private int m_nLostCount = -1;
        private VariousData<float> m_tankTemperature = null;
        private List<Article> m_articles = new List<Article>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int BeginArticleID
        {
            get { return m_nBeginArticleID; }
            set { m_nBeginArticleID = value; }
        }

        public int BeginHistoryMessageID
        {
            get { return m_nBeginHistoryMessageID; }
            set { m_nBeginHistoryMessageID = value; }
        }

        public int DeadCount
        {
            get { return m_nDeadCount; }
            set { m_nDeadCount = value; }
        }

        public int InjuryCount
        {
            get { return m_nInjuryCount; }
            set { m_nInjuryCount = value; }
        }

        public int LostCount
        {
            get { return m_nLostCount; }
            set { m_nLostCount = value; }
        }

        public VariousData<float> TankTemperature
        {
            get { return m_tankTemperature; }
            set { m_tankTemperature = value; }
        }

        public List<Article> Articles
        {
            get { return m_articles; }
            set { m_articles = value; }
        }

        public static Status ReadData(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, BeginArticleID, BeginHistoryMessageID, DeadCount, InjuryCount, LostCount, TankTemperature from LostStatus where ID = (Select max(ID) from LostStatus)";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 7)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> beginArticleID = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<int> beginHistoryMessageID = WebDBManager.GetIntField(arrResult[2].ToString());
            VariousData<int> deadCount = WebDBManager.GetIntField(arrResult[3].ToString());
            VariousData<int> injuryCount = WebDBManager.GetIntField(arrResult[4].ToString());
            VariousData<int> lostCount = WebDBManager.GetIntField(arrResult[5].ToString());
            VariousData<float> tankTemperature = WebDBManager.GetFloatField(arrResult[6].ToString());

            if (id == null || beginArticleID == null || beginHistoryMessageID == null || deadCount == null || injuryCount == null || lostCount == null)
                return null;

            Status status = new Status();

            status.ID = id.Data;
            status.BeginArticleID = beginArticleID.Data;
            status.BeginHistoryMessageID = beginHistoryMessageID.Data;
            status.DeadCount = deadCount.Data;
            status.InjuryCount = injuryCount.Data;
            status.LostCount = lostCount.Data;
            status.TankTemperature = tankTemperature;

            return status;
        }

        public static bool SaveDB(WebDBManager dbMgr, ref Status status, int nDeadCount, int nInjuryCount, int nLostCount, VariousData<float> tankTemperature)
        {
            if (dbMgr == null)
                return false;

            if (status == null)
            {
                status = InsertDB(dbMgr, nDeadCount, nInjuryCount, nLostCount, tankTemperature, 1);
                return status != null;
            }

            string strTemp = "NULL";

            if (tankTemperature != null)
                strTemp = string.Format("{0:F1}", tankTemperature.Data);

            string strSQL = string.Format("Update LostStatus set DeadCount = {0}, InjuryCount = {1}, LostCount = {2}, TankTemperature = {3} where ID = {4}",
                 nDeadCount, nInjuryCount, nLostCount, strTemp, status.ID);

            if (dbMgr.GetResultData(strSQL) == null)
                return false;

            status.DeadCount = nDeadCount;
            status.InjuryCount = nInjuryCount;
            status.LostCount = nLostCount;
            status.TankTemperature = tankTemperature;
            return true;
        }

        private static Status InsertDB(WebDBManager dbMgr, int nDeadCount, int nInjuryCount, int nLostCount, VariousData<float> tankTemperature, int nBeginArticleID)
        {
            int nBeginHistoryMessageID = GetMaxID(dbMgr, "ActionStepHistoryMessage");

            if (nBeginHistoryMessageID < 0)
                return null;
            else
                nBeginHistoryMessageID++;

            int nStatusID = GetMaxID(dbMgr, "LostStatus");

            if (nStatusID < 0)
                return null;
            else
                nStatusID++;

            string strTemp = "NULL";

            if (tankTemperature != null)
                strTemp = string.Format("{0:F1}", tankTemperature.Data);

            string strSQL = "Insert into LostStatus (ID, BeginArticleID, BeginHistoryMessageID, DeadCount, InjuryCount, LostCount, TankTemperature) values (";
            strSQL += string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6})",
                nStatusID, nBeginArticleID, nBeginHistoryMessageID, nDeadCount, nInjuryCount, nLostCount, strTemp);

            if (dbMgr.GetResultData(strSQL) == null)
                return null;

            Status status = new Status();

            status.ID = nStatusID;
            status.BeginArticleID = nBeginArticleID;
            status.BeginHistoryMessageID = nBeginHistoryMessageID;
            status.DeadCount = nDeadCount;
            status.InjuryCount = nInjuryCount;
            status.LostCount = nLostCount;
            status.TankTemperature = tankTemperature;

            return status;
        }

        public static int GetMaxID(WebDBManager dbMgr, string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count > 0)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id != null)
                    return id.Data;
            }

            return 0;
        }

        public static bool Initialize(WebDBManager dbMgr, Status status)
        {
            if (dbMgr == null)
                return false;

            int nBeginHistoryMessageID = GetMaxID(dbMgr, "ActionStepHistoryMessage");

            if (nBeginHistoryMessageID < 0)
                return false;
            else
                nBeginHistoryMessageID++;

            int nBeginArticleID = GetMaxID(dbMgr, "LostArticle");

            if (nBeginArticleID < 0)
                return false;
            else
                nBeginArticleID++;

            string strSQL = string.Format("Update LostStatus set BeginArticleID = {0}, BeginHistoryMessageID = {1}, DeadCount = 0, InjuryCount = 0, LostCount = 0, TankTemperature = NULL where ID = {2}",
                nBeginArticleID, nBeginHistoryMessageID, status.ID);
            bool result = dbMgr.GetResultData(strSQL) != null;

            if (result)
            {
                status.BeginArticleID = nBeginArticleID;
                status.BeginHistoryMessageID = nBeginHistoryMessageID;
                status.DeadCount = status.InjuryCount = status.LostCount = 0;
                status.TankTemperature = null;
            }

            return result;
        }
    }
}
