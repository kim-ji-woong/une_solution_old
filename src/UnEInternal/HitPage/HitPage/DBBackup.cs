using dnsDBUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static dnsDBUtil.WebDBManager;

namespace HitPage
{
    public class DBBackup
    {
        private DateTime m_dtLastDateTime = new DateTime();
        private WebDBManager m_dbMgr = null;
        private string m_backupFilePath = "";

        public static StreamWriter swLog = new StreamWriter(Application.StartupPath + "\\dbbackup.log", true);

        public DBBackup()
        {
            m_dtLastDateTime = DateTime.Now;

            string strWebserverURL = System.Configuration.ConfigurationManager.AppSettings.Get("webServerUrl");
            string strDBName = System.Configuration.ConfigurationManager.AppSettings.Get("dbName");
            string strDBType = System.Configuration.ConfigurationManager.AppSettings.Get("dbType");
            m_backupFilePath = System.Configuration.ConfigurationManager.AppSettings.Get("backupFilePath");

            m_dbMgr = new WebDBManager();
            m_dbMgr.WebServerURL = strWebserverURL;
            m_dbMgr.DatabaseName = strDBName;
            m_dbMgr.DatabaseTypeName = (strDBType == "1") ? "mysql" : "sqlserver";

            DBExport();
        }

        public void Run()
        {
            DateTime dtNow = DateTime.Now;
            TimeSpan ts = dtNow - m_dtLastDateTime;
            if (ts.TotalHours < 24)
                return;

            m_dtLastDateTime = dtNow;

            DBExport();
            FileDelete();
        }

        /// <summary>
        /// 한달 지난 파일 지우기
        /// </summary>
        private void FileDelete()
        {
            try
            {
                if (Directory.Exists(m_backupFilePath))
                {
                    DateTime dtNow = DateTime.Now;

                    List<string> delFiles = new List<string>();
                    DirectoryInfo dir = new DirectoryInfo(m_backupFilePath);
                    foreach (FileInfo item in dir.GetFiles())
                    {
                        DateTime dtFile = new DateTime();
                        if (DateTime.TryParse(item.Name.Replace(".sql", ""), out dtFile) && item.Extension == ".sql")
                        {
                            //DateTime dtFile = DateTime.Parse(item.Name.Replace(".sql", ""));
                            if ((dtNow - dtFile).TotalDays > 30)
                                delFiles.Add(item.FullName);
                        }
                    }

                    foreach (string item in delFiles)
                    {
                        File.Delete(item);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("[ERROR] FileDelete() : " + ex.Message);
            }
        }

        private void DBExport()
        {
            LoadTables();
            MakeSql();
        }

        private List<string> m_tables = new List<string>();
        private void LoadTables()
        {
            try
            {
                m_tables.Clear();
                ArrayList arrResult = m_dbMgr.GetResultData("SELECT table_name FROM INFORMATION_SCHEMA.TABLES");
                if (arrResult == null) return;

                for (int i = 0; i < arrResult.Count; i++)
                {
                    string tableName = WebDBManager.GetStringField(arrResult[i]);
                    if (tableName == "sysdiagrams")
                        continue;

                    m_tables.Add(tableName);
                }
            }
            catch (Exception ex)
            {
                WriteLog("[ERROR] LoadTables() : " + ex.Message);
            }
        }

        private void MakeSql()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'");
                foreach (string strTableName in m_tables)
                {
                    sb.AppendLine("DELETE FROM " + strTableName + ";");

                    ArrayList arrColumnInfo = m_dbMgr.GetResultData(
                        "SELECT column_name, data_type FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME ='" + strTableName + "'");

                    if (arrColumnInfo == null || arrColumnInfo.Count == 0)
                        continue;

                    Dictionary<string, string> dicTableInfo = new Dictionary<string, string>();

                    for (int i = 0; i < arrColumnInfo.Count; i += 2)
                    {
                        string strColumnName = WebDBManager.GetStringField(arrColumnInfo[i]);
                        string strColumnType = WebDBManager.GetStringField(arrColumnInfo[i + 1]);

                        dicTableInfo.Add(strColumnName, strColumnType);
                    }

                    int nColumnCount = dicTableInfo.Count;
                    string columns = string.Join(", ", dicTableInfo.Keys);

                    ArrayList arrResult = m_dbMgr.GetResultData("SELECT " + columns + " FROM " + strTableName, 0);
                    if (arrResult == null || arrResult.Count == 0)
                        continue;

                    for (int i = 0; i < arrResult.Count; i += nColumnCount)
                    {
                        int iCnt = 0;
                        
                        sb.Append("INSERT INTO " + strTableName + " VALUES ");

                        sb.Append("(");
                        for (int j = 0; j < nColumnCount; j++)
                        {
                            if (j > 0)
                                sb.Append(",");
                            string dataKey = dicTableInfo.Keys.ElementAt(iCnt);
                            string dataType = dicTableInfo.Values.ElementAt(iCnt);
                            object value = null;
                            if (dataType.ToUpper() == "INT")
                            {
                                VariousData<int> data = WebDBManager.GetIntField(arrResult[i + iCnt].ToString());
                                if (data != null)
                                {
                                    value = data.Data;

                                    // ParentID가 null 이면 최상위인데 int 타입의 기본값이 -1로 들어오다 보니 데이터가 잘못입력됨
                                    if (dataKey.ToUpper() == "PARENTID" && data.Data == -1)
                                    {
                                        value = null;
                                    }
                                }
                            }
                            else if (dataType.ToUpper() == "DATETIME")
                            {
                                VariousData<DateTime> date = WebDBManager.GetDateTimeField(arrResult[i + iCnt]);
                                if (date != null)
                                {
                                    value = Convert.ToDateTime(date.Data).ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }
                            else if (dataType.ToUpper() == "BIT")
                                value = WebDBManager.GetIntField(arrResult[i + iCnt].ToString(), -1);
                            else
                                value = WebDBManager.GetStringField(arrResult[i + iCnt]);

                            if (value == null)
                                sb.Append("NULL");
                            else
                            {
                                if (dataType.ToUpper() == "INT" || dataType.ToUpper() == "FLOAT")
                                    sb.Append(value);
                                else
                                    sb.Append("'" + value + "'");
                            }
                            iCnt++;
                        }
                        sb.AppendLine(");");
                    }
                }

                sb.AppendLine("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all'");

                if (sb.Length > 0)
                {
                    FileStream fs = new FileStream(m_backupFilePath + DateTime.Now.ToString("yyyy-MM-dd") + ".sql", FileMode.Create, FileAccess.Write);
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(sb.ToString());

                    }
                    fs.Close();

                    WriteLog("backup 완료");
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        public static void WriteLog(string content)
        {
            string strNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            swLog.WriteLine("[" + strNow + "] " + content);
            swLog.Flush();
        }
    }    
}
