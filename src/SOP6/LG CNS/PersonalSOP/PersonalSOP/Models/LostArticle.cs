using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBUtility2;
using System.Collections;

namespace PersonalSOP.Models
{
    public class LostArticle
    {
        private int m_nArticleNo = -1;
        private DateTime m_time;
        private string m_strTitle = "";
        private string m_strMessage = "";

        public int No
        {
            get { return m_nArticleNo; }
            set { m_nArticleNo = value; }
        }

        public DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public LostArticle()
        {
            m_time = DateTime.Now;
        }

        public LostArticle(int no, string strMessage)
        {
            m_nArticleNo = no;
            m_time = DateTime.Now;
            m_strMessage = strMessage;
        }

        public LostArticle(int no, DateTime time, string strMessage)
        {
            m_nArticleNo = no;
            m_time = time;
            m_strMessage = strMessage;
        }

        public static bool ReadNewData(WebDBManager dbMgr, LostStatus status)
        {
            string strSQL = string.Format("Select ID, Title, Message, TimeStamp from LostArticle where LostStatusID = {0} and ID >= {1} and ID > {2}", status.No, status.BeginArticleID, status.GetMaxArticleID());
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nMaxID = status.GetMaxArticleID();
            int nLastArticleNo = nMaxID;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strTitle = WebDBManager.GetStringField(arrResult[i + 1]);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 3].ToString());

                if (id == null || strTitle == null || time == null)
                    continue;

                LostArticle article = new LostArticle();

                article.No = id.Data;
                article.Title = strTitle;
                article.Time = time.Data;

                if (strMessage != null)
                    article.Message = strMessage;

                status.Articles.Add(article);

                if (nLastArticleNo < article.No)
                    nLastArticleNo = article.No;
            }

            bool result = nMaxID != nLastArticleNo;
            //nLastArticleNo = nMaxID;
            return result;
        }
    }

    public class LostStatus
    {
        public enum State { NoChanged = 0, StatusChanged, ArticleChanged, Both, Initialized };

        private int m_no = -1;
        private int m_nBeginArticleID = -1;
        private int m_nBeginHistoryMessageID = -1;
        private int m_nDeadCount = -1;
        private int m_nInjuryCount = -1;
        private int m_nLostCount = -1;
        private VariousData<float> m_tankTemperature = null;
        private string m_strTemperature = "-";
        private List<LostArticle> m_articles = new List<LostArticle>();

        public int No
        {
            get { return m_no; }
            set { m_no = value; }
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
            set
            {
                m_tankTemperature = value;

                if (m_tankTemperature != null)
                    m_strTemperature = string.Format("{0:F1}", m_tankTemperature.Data);
                else
                    m_strTemperature = "-";
            }
        }

        public string Temperature
        {
            get { return m_strTemperature; }
        }

        public List<LostArticle> Articles
        {
            get { return m_articles; }
        }

        public int GetMaxArticleID()
        {
            int nCount = m_articles.Count;

            if (nCount == 0)
                return 0;

            return m_articles[nCount-1].No;
        }

        public static State ReadNewData(WebDBManager dbMgr, ref LostStatus lastStatus, out string strCurrentStatus)
        {
            strCurrentStatus = "";

            if (dbMgr == null)
                return State.NoChanged;

            State result = State.NoChanged;

            if (ReadCurrentStatus(dbMgr, ref lastStatus, ref result, out strCurrentStatus) == false)
                return result;

            if (result == State.Initialized)
                return result;

            if (LostArticle.ReadNewData(dbMgr, lastStatus))
            {
                if (result == State.StatusChanged)
                    result = State.Both;
                else
                    result = State.ArticleChanged;
            }

            return result;
        }

        private static bool ReadCurrentStatus(WebDBManager dbMgr, ref LostStatus lastStatus, ref State state, out string strCurrentStatus)
        {
            strCurrentStatus = "";

            string strSQL = "Select ID, BeginArticleID, BeginHistoryMessageID, DeadCount, InjuryCount, LostCount, TankTemperature from LostStatus ";
            strSQL += "where ID = (Select max(ID) from LostStatus)";

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            if (nResultCount < 7)
                return false;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> beginArticleID = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<int> beginHistoryMessageID = WebDBManager.GetIntField(arrResult[2].ToString());
            VariousData<int> deadCount = WebDBManager.GetIntField(arrResult[3].ToString());
            VariousData<int> injuryCount = WebDBManager.GetIntField(arrResult[4].ToString());
            VariousData<int> lostCount = WebDBManager.GetIntField(arrResult[5].ToString());
            VariousData<float> tankTemperature = WebDBManager.GetFloatField(arrResult[6].ToString());

            if (id == null || beginArticleID == null || beginHistoryMessageID == null ||
                deadCount == null || injuryCount == null || lostCount == null)
                return false;

            LostStatus status = new LostStatus();

            status.No = id.Data;
            status.BeginArticleID = beginArticleID.Data;
            status.BeginHistoryMessageID = beginHistoryMessageID.Data;
            status.DeadCount = deadCount.Data;
            status.InjuryCount = injuryCount.Data;
            status.LostCount = lostCount.Data;
            status.TankTemperature = tankTemperature;

            strCurrentStatus = string.Format("{0}_{1}_{2}_",
                status.DeadCount,
                status.InjuryCount,
                status.LostCount);

            if (status.TankTemperature == null)
                strCurrentStatus += "-";
            else
                strCurrentStatus += string.Format("{0:F1}", status.TankTemperature.Data);

            if (lastStatus == null || lastStatus.No != status.No)
            {
                lastStatus = status;
                state = State.StatusChanged;
                ReadNewArticles(dbMgr, lastStatus);
                //nLastArticleNo = beginHistoryMessageID.Data - 1;
            }
            else
            {
                if (status.DeadCount != lastStatus.DeadCount)
                {
                    lastStatus.DeadCount = status.DeadCount;
                    state = State.StatusChanged;
                }

                if (status.InjuryCount != lastStatus.InjuryCount)
                {
                    lastStatus.InjuryCount = status.InjuryCount;
                    state = State.StatusChanged;
                }

                if (status.LostCount != lastStatus.LostCount)
                {
                    lastStatus.LostCount = status.LostCount;
                    state = State.StatusChanged;
                }

                if (IsSameData<float>(status.TankTemperature, lastStatus.TankTemperature) == false)
                {
                    lastStatus.TankTemperature = status.TankTemperature;
                    state = State.StatusChanged;
                }

                if (status.BeginHistoryMessageID > lastStatus.BeginHistoryMessageID)
                {
                    lastStatus.BeginHistoryMessageID = status.BeginHistoryMessageID;
                    state = State.Initialized;
                }

                if (status.BeginArticleID > lastStatus.BeginArticleID)
                {
                    lastStatus.BeginArticleID = status.BeginArticleID;
                    state = State.Initialized;
                }
            }

            return true;
        }

        private static bool ReadNewArticles(WebDBManager dbMgr, LostStatus status)
        {
            string strSQL = string.Format("Select ID, Title, Message, TimeStamp from LostArticle where LostStatusID = {0} and ID >= {1} and ID > {2} Order by ID Desc", status.No, status.BeginArticleID, status.GetMaxArticleID());
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strTitle = WebDBManager.GetStringField(arrResult[i + 1]);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 3]);

                if (id == null || strTitle == null || time == null)
                    continue;

                LostArticle article = new LostArticle();

                article.No = id.Data;
                article.Time = time.Data;
                article.Title = strTitle;

                if (strMessage != null)
                    article.Message = strMessage;

                status.Articles.Add(article);
            }

            return true;
        }

        private static bool IsSameData<DataType>(VariousData<DataType> data1, VariousData<DataType> data2)
        {
            if (data1 == null && data2 == null)
                return true;
            else if (data1 == null && data2 != null)
                return false;
            else if (data1 != null && data2 == null)
                return false;

            return data1.Data.Equals(data2.Data);
        }
    }
}
