using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace LostArticle
{
    public class Article
    {
        private int m_nID = -1;
        private string m_strTitle = "";
        private string m_strMessage = "";
        private DateTime m_timeStamp;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
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

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public static List<Article> ReadData(WebDBManager dbMgr, Status status)
        {
            if (dbMgr == null || status == null)
                return null;

            string strSQL = string.Format("Select ID, Title, Message, TimeStamp from LostArticle where LostStatusID = {0} and ID >= {1}", status.ID, status.BeginArticleID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<Article> articles = new List<Article>();

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strTitle = WebDBManager.GetStringField(arrResult[i + 1]);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 3]);

                if (id == null || strTitle == null || timeStamp == null)
                    continue;

                Article article = new Article();

                article.ID = id.Data;
                article.Title = strTitle;
                article.TimeStamp = timeStamp.Data;

                if (strMessage != null)
                    article.Message = strMessage;

                articles.Add(article);
            }

            return articles;
        }

        public static bool SaveDB(WebDBManager dbMgr, Status status, string strTitle, string strMessage)
        {
            if (dbMgr == null)
                return false;

            int nArticleID = Status.GetMaxID(dbMgr, "LostArticle");

            if (nArticleID < 0)
                return false;
            else
                nArticleID++;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = "Insert into LostArticle (ID, LostStatusID, Title, Message, TimeStamp) values (";
            strSQL += string.Format("{0}, {1}, '{2}', '{3}', '{4}')",
                nArticleID, status.ID, strTitle, strMessage, strTime);

            if (dbMgr.GetResultData(strSQL) == null)
                return false;

            Article article = new Article();

            article.ID = nArticleID;
            article.Title = strTitle;
            article.Message = strMessage;
            article.TimeStamp = dtNow;

            status.Articles.Add(article);
            return true;
        }
    }
}
