using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace WindowsFormsApplication14
{
    public class BatchProcess : IDisposable
    {
        private char[] m_Buffer = null;
        
        private string m_szConURL = "Data Source={0};Initial Catalog={1};User ID={2}; Password={3}";
        
        private int m_nBlockSize = 1024 * 1024 * 10;
        public int BlockSize
        {
            get { return m_nBlockSize; }
            set { m_nBlockSize = value; }
        }
        
        private string m_szServerIP = "";
        public string ServerIP
        {
            get { return m_szServerIP; }
            set { m_szServerIP = value; }
        }

        private string m_szTargetDB = "";
        public string TargetDB
        {
            set { m_szTargetDB = value; }
        }

        private string m_szUserID = "";
        public string UserID
        {
            set { m_szUserID = value; }
        }

        private string m_szPassword = "";
        public string Password
        {
            set { m_szPassword = value; }
        }

        private bool m_bSavedFile = false;
        public bool SavedFile
        {
            get { return m_bSavedFile; }
            set { m_bSavedFile = value; }
        }

        private string m_szSQL = null;
        private int nCount = 1;

        private string m_szTempFileName = "";
        
        private string m_szSavedPath = "";
        public string SavedPath
        {
            get { return m_szSavedPath; }
            set { m_szSavedPath = value; }
        }

        private bool m_bMySQL = false;
        public bool UseMySQL
        {
            get { return m_bMySQL; }
            set { m_bMySQL = value; }
        }

        private ArrayList m_arCreateFileList = new ArrayList();

        public BatchProcess(bool bSaveSeperateFile)
        {
            m_Buffer = new char[m_nBlockSize + 1];
        }      
 
        public void Dispose()
        {
            m_Buffer = null;
        }

        private string MakeTempFileName(string szPath)
        {
            string szTemp1 = Path.GetFileName(szPath);
            string szTemp2 = Path.GetExtension(szTemp1);

            szTemp1 = szTemp1.Replace(szTemp2, "");
            szTemp1 = szTemp1.Replace(".", "");

            if (m_szSavedPath == "")
            {
                m_szSavedPath = Path.GetDirectoryName(szPath);
            }

            return szTemp1;
        }

        public bool Run(string szFileName)
        {
            nCount = 1;

            if(!File.Exists(szFileName))
            {
                return false;
            }

            m_szTempFileName = MakeTempFileName(szFileName);

            DbConnection con = Connect();
            DbTransaction tx = con.BeginTransaction();

            bool bResult = true;
            m_arCreateFileList.Clear();
            try
            {
                StreamReader reader = File.OpenText(szFileName);

                int nToRead = m_nBlockSize;
                int nStartRead = 0;

                while (!reader.EndOfStream)
                {
                    int nRead = reader.ReadBlock(m_Buffer, nStartRead, (nToRead - nStartRead));

                    int nExtra = ProcessSQL(con, tx, m_Buffer, nRead + nStartRead);

                    nStartRead = nExtra;
                }
                reader.Close();
            }
            catch(Exception)
            {
                DeleteTempFile();
                tx.Rollback();
                bResult = false;
            }

            if (bResult == true)
                tx.Commit();

            con.Close();

            return bResult;
        }

        private void DeleteTempFile()
        {
            foreach(string szName in m_arCreateFileList)
            {
                FileInfo info = new FileInfo(szName);
                if( info.Exists )
                {
                    info.Delete();
                }
            }
        }

        private int FindEndLine(char[] buf, int nRead)
        {
            int nResult = -1;
            for (int i = nRead; i >= 0; i--)
            {
                if (buf[i] == '\n')
                {
                    nResult = i;
                    return nResult;
                }
            }
            return nResult;
        }              

        private int ProcessSQL(DbConnection con, DbTransaction tx, char[] buf, int nRead)
        {
            int nIdx = FindEndLine(buf, nRead);
            string szSQL = new string(buf, 0, nIdx);
           
            int nExtraCount = nRead - (nIdx + 1);
            Array.Copy(buf, nIdx + 1, buf, 0, nExtraCount);
            
            if( m_bSavedFile == true)
            {
                string szWriteFileName = m_szSavedPath + "\\" + m_szTempFileName + "_sq" + nCount + ".sql";                
                System.IO.StreamWriter w = new StreamWriter(szWriteFileName);
                w.WriteLine("USE " + m_szTargetDB);
                w.WriteLine("GO");
                w.Write(szSQL);
                w.WriteLine("GO");
                w.Close();
                m_arCreateFileList.Add(szWriteFileName);

                nCount++;
            }      

            string temp1 = szSQL.Replace("GO", "");
            ExecuteSQL(temp1, con, tx);
                        
            szSQL = null;
            temp1 = null; 

            try
            {
                GC.Collect();
            }
            catch(Exception)
            {

            }
            return nExtraCount;
        }

        public static bool ConnectionTest(string szServerIP, string szTargetDB, string szUserID, string szPassword, bool bMysql = false)
        {
            string szConURL = "Data Source={0};Initial Catalog={1};User ID={2}; Password={3}";

            string strConnection = string.Format(szConURL, szServerIP, szTargetDB, szUserID, szPassword);
            strConnection = @"" + strConnection;

            if(bMysql == true)
            {
                MySqlConnection conn = new MySqlConnection(strConnection);
                try
                {
                    conn.Open();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                    return false;
                }
            }
            else
            {
                SqlConnection conn = new SqlConnection(strConnection);
                try
                {
                    conn.Open();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                    return false;
                }
                
            }
            return true;
        }

        public DbConnection Connect()
        {
            DbConnection conn = null;
            if( m_bMySQL == true)
            {
                string strConnection = string.Format(m_szConURL, m_szServerIP, m_szTargetDB, m_szUserID, m_szPassword);
                strConnection = @"" + strConnection;
                conn = new MySqlConnection(strConnection);
                try
                {                    conn.Open();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                    return null;
                }
            }
            else
            {
                string strConnection = string.Format(m_szConURL, m_szServerIP, m_szTargetDB, m_szUserID, m_szPassword);
                strConnection = @"" + strConnection;
                conn = new SqlConnection(strConnection);
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                    return null;
                }
            }            
            return conn;
        }
        
        public void ExecuteSQL(string sql, DbConnection conn, DbTransaction tranc = null)
        {
            if (conn == null)
                return;

            if (m_bMySQL == true)
            {
                using(MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)conn))
                {
                    if (tranc != null)
                        cmd.Transaction = (MySqlTransaction)tranc;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);                        
                    }
                }                
            }
            else 
            {
                using(SqlCommand cmd = new SqlCommand(sql, (SqlConnection)conn))
                {
                    if (tranc != null)
                        cmd.Transaction = (SqlTransaction)tranc;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        return;
                    }
                }
                
            }

            
        }
    }
}
