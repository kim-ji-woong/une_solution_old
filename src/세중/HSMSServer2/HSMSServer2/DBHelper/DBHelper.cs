using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using HSMS;

namespace HSMSServer2
{
    public class DBHelper
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private static string m_szMaxID = "_HSMS_TABLE_MAXID_DEFINE_";
        public static string MaxID
        {
            get { return m_szMaxID; }
            set { m_szMaxID = value; }
        }

        /// <summary>
        /// 최대 id_key값 찾기
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="szTableName">테이블명</param>
        /// <returns></returns>
        public static int FindMaxID(DBConn conn, string szTableName , string szIDFieldName = "ID")
        {
            SqlConnection connection = null;
            try
            {
                connection = conn.Connect();
            }
            catch (System.Exception)
            {
                return -1;
            }
            
            int nCount = 0;
            string strCount = "";

            string SQLMaxID = string.Format("select max({0}) from {1}", szIDFieldName, szTableName);

            try
            {
                SqlDataReader rd = conn.ExecuteReader(SQLMaxID, connection);
                if (rd.Read())
                {
                    if (rd.IsDBNull(0))
                    {
                        nCount = 1;
                    }
                    else
                    {
                        strCount = rd[0].ToString();
                        nCount = Convert.ToInt32(strCount);
                        nCount++;
                    }
                }
                rd.Close();
                connection.Close();
            }
            catch (System.Exception)
            {
            	
            }
            finally
            {
                try
                {
                    if (connection != null)
                        connection.Close();
                }
                catch (System.Exception)
                {
                }
            }
            return nCount;
        }

        private static int FindMaxID(SqlConnection connection, string szTableName, SqlTransaction tranc, string szIDFieldName = "ID")
        { 
            int nCount = 0;
            string strCount = "";

            string SQLMaxID = string.Format("select max({0}) from {1}", szIDFieldName, szTableName);

            try
            {
                if (connection == null)
                    return -1;

                using(SqlCommand cmd = new SqlCommand(SQLMaxID, connection))
                {
                    if (tranc != null)
                        cmd.Transaction = tranc;

                    SqlDataReader rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        if (rd.IsDBNull(0))
                        {
                            nCount = 1;
                        }
                        else
                        {
                            strCount = rd[0].ToString();
                            nCount = Convert.ToInt32(strCount);
                            nCount++;
                        }
                    }
                    rd.Close();  
                } 
            }
            catch (System.Exception ex)
            {
                //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine("DB 에러 : ", ex);
            }
            finally
            {                
            }
            return nCount;
        }

        /// <summary>
        /// 대상테이블의 MaxID를 구하여 sql문중에 MaxID를 치환하여 실행해주는 함수 
        /// MaxID는 하나의 테이블만을 대상이며 arStrQuery에는 반드시 이점을 준수해야함
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="arStrQuery"></param>
        /// <param name="szTable">MaxID 대상 테이블</param>
        /// <param name="nMaxID"></param>
        /// <param name="szIDFieldName">ID인 필드 이름 기본값 "ID"</param>
        /// <returns></returns>
        public static bool ExecuteSQL(DBConn conn, ArrayList arStrQuery, string szTable, ref int nMaxID, string szIDFieldName = "ID")
        {
            nMaxID = -1;

            SqlConnection connection = null;
            try
            {
                connection = conn.Connect();
            }
            catch (System.Exception ex)
            {
                /*if (NetworkServer.Instance.ServiceProvider.ConnectionLog.IsOpened)
                {
                    NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine(ex);
                }*/
                return false;
            }
            
            SqlTransaction tranc = null;
            try
            {
                tranc = connection.BeginTransaction();
            }
            catch (System.Exception ex)
            {
                /*if (NetworkServer.Instance.ServiceProvider.ConnectionLog.IsOpened)
                {
                    NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine(ex);
                }*/
                return false;
            }
            int nSiteID = NetworkServer.Instance.SiteID;
            bool bResult = false;

            try
            {
                foreach (string sql in arStrQuery)
                {
                    string szSql = sql;
                    if (sql.Contains(m_szMaxID))
                    {
                        nMaxID = FindMaxID(connection, szTable, tranc, szIDFieldName);
                        szSql = sql.Replace(m_szMaxID, nMaxID.ToString());
                    }
                    conn.ExecuteSQL(szSql, connection, tranc);
                }

                if (tranc != null)
                    tranc.Commit();
                bResult = true;
            }
            catch (System.Exception ex)
            {
                if (tranc != null)
                    tranc.Rollback();
                bResult = false;
               
                logger.Debug("SQL 실행에러 : " ,ex);
            }
            finally
            {
                try
                {
                    if (connection != null)
                        connection.Close();
                }
                catch (System.Exception)
                {
                }
            }
            return bResult;
        }

        /// <summary>
        /// 여러개의 쿼리를 같은 트랜잭션하에서 실행하는 함수
        /// </summary>
        /// <param name="conn">DBConn</param>
        /// <param name="arStrQuery">ArrayList, string query</param>
        /// <returns>true/false</returns>
        public static bool ExecuteSQL(DBConn conn, ArrayList arStrQuery)
        {
            SqlConnection connection = null;
            try
            {
                connection = conn.Connect();
            }
            catch (System.Exception ex)
            {
                logger.Debug("SQL 실행에러 : ", ex);
                return false;
            }

            SqlTransaction tranc = null;
            try
            {
                tranc = connection.BeginTransaction();
            }
            catch (System.Exception ex)
            {
                logger.Debug("SQL 실행에러 : ", ex);
                return false;
            }
            int nSiteID = NetworkServer.Instance.SiteID;
            bool bResult = false;

            try
            {
                foreach(string sql in arStrQuery)
                {                   
                    conn.ExecuteSQL(sql, connection, tranc);
                }
                
                if (tranc != null)
                    tranc.Commit();
                bResult = true;
            }
            catch (System.Exception ex)
            {
                if (tranc != null)
                    tranc.Rollback();
                bResult = false;
                logger.Debug("SQL 실행에러 : ", ex);
            }
            finally
            {
                try
                {
                    if (connection != null)
                        connection.Close();
                }
                catch (System.Exception)
                {
                }
            }
            return bResult;
        }

        /// <summary>
        /// 대상테이블의 MaxID를 구하여 sql문중에 MaxID를 치환하여 실행해주는 함수
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="szTable">Maxid 대상 테이블</param>
        /// <param name="nMaxID">Maxid리턴값</param>
        /// <param name="szIDFieldName">ID인 필드 이름 기본값 "ID"</param>
        /// <returns>Commit되었으면 true, Rollback되었으면 false</returns>
        public static bool ExecuteSQL(DBConn conn, string sql, string szTable, ref int nMaxID, string szIDFieldName = "ID")
        {
            nMaxID = -1;
            SqlConnection connection = null;
            try
            {
                connection = conn.Connect();
            }
            catch (System.Exception ex)
            {
                logger.Debug("SQL 실행에러 : ", ex);
                return false;
            }

            SqlTransaction tranc = null;
            try
            {
                tranc = connection.BeginTransaction();
            }
            catch (System.Exception ex)
            {
                logger.Debug("SQL 실행에러 : ", ex);
                return false;
            }
            
            int nSiteID = NetworkServer.Instance.SiteID;
            bool bResult = false;

            try
            {
                if (sql.Contains(m_szMaxID))
                {
                    nMaxID = FindMaxID(connection, szTable, tranc, szIDFieldName);
                    sql = sql.Replace(m_szMaxID, nMaxID.ToString());
                }
                conn.ExecuteSQL(sql, connection, tranc);
                if (tranc != null)
                    tranc.Commit();
                bResult = true;

            }
            catch (System.Exception ex)
            {
                if (tranc != null)
                    tranc.Rollback();
                bResult = false;
                logger.Debug("SQL 실행에러 : ", ex);
            }
            finally
            {
                try
                {
                    if (connection != null)
                        connection.Close();
                }
                catch (System.Exception)
                {
                }
            }
            return bResult;
        }

        /// <summary>
        /// 하나의 쿼리를 트랜잭션하에서 실행하는 함수
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool ExecuteSQL(DBConn conn, string sql)
        {
            SqlConnection connection = null;
            try
            {
                connection = conn.Connect();
            }
            catch (System.Exception ex)
            {
                logger.Debug("SQL 실행에러 : ", ex);
                return false;
            }

            SqlTransaction tranc = null;
            try
            {
                tranc = connection.BeginTransaction();
            }
            catch (System.Exception ex)
            {
                logger.Debug("SQL 실행에러 : ", ex);
                return false;
            }
            int nSiteID = NetworkServer.Instance.SiteID;
            bool bResult = false;

            try
            {                
                conn.ExecuteSQL(sql, connection, tranc);
                if (tranc != null)
                    tranc.Commit();
                bResult = true;

            }
            catch (System.Exception ex)
            {
                if (tranc != null)
                    tranc.Rollback();
                bResult = false;

                logger.Debug("SQL 실행에러 : ", ex);
            }
            finally
            {
                try
                {
                    if (connection != null)
                        connection.Close();
                }
                catch (System.Exception)
                {
                }
            }
            return bResult;
        }

        private static object m_lock = new object();
        public static ArrayList GetResultData(DBConn conn, string sql)
        {
            lock (m_lock)
            {
                ArrayList arList = new ArrayList();
                SqlDataReaderInfo info = ExecuteReader(conn, sql);
                if (info == null)
                    return null;

                if (info.IsSelect == false)
                    return arList;

                try
                {
                    SqlDataReader rd = info.DataReader;
                    while(rd.Read())
                    {
                        Object[] values = new Object[rd.FieldCount];
                        int fieldCount = rd.GetValues(values);

                        ICollection objs = values.ToList();
                        arList.AddRange(objs);                        
                    }

                    info.Close();
                    return arList;
                }
                catch(Exception ex)
                {
                    logger.Debug("SQL 에러 : ", ex);
                } 
            }
            return null;
        }


        public static SqlDataReaderInfo ExecuteReader(DBConn conn, string sql)
        {
            if (conn == null)
                return null;

            SqlConnection connection = null;
            try
            {
                connection = conn.Connect();
            }
            catch (System.Exception ex)
            {
                logger.Debug("SQL 실행에러 : ", ex);
                return null;
            }

            SqlTransaction tranc = null;
            try
            {
                tranc = connection.BeginTransaction();
            }
            catch (System.Exception ex)
            {
                logger.Debug("SQL 실행에러 : ", ex);
                return null;
            }

            try
            {

                SqlCommand cmd = new SqlCommand(sql, connection);
                if (tranc != null)
                    cmd.Transaction = tranc;
                try
                {

                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    SqlDataReaderInfo info = new SqlDataReaderInfo(connection, tranc, cmd, reader);
                    return info;
                }
                catch (Exception e)
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                    logger.Debug("SQL 실행에러 : ", e);
                    logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());
                    return null;
                }             
            }
            catch (System.Exception ex)
            {
                if (tranc != null)
                    tranc.Rollback();               

                logger.Debug("SQL 실행에러 : ", ex);

                try
                {
                    if (connection != null)
                        connection.Close();
                }
                catch (System.Exception)
                {
                }
            }            
            return null;
        }
    }

    public class SqlDataReaderInfo
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SqlTransaction m_Tranc = null;
        private SqlCommand m_SqlCmd = null;
        private SqlConnection m_Connection = null;
        private SqlDataReader m_DataReader = null;


        public SqlDataReader DataReader
        {
            get { return m_DataReader; }
        }

        public bool IsSelect
        {
            get 
            {
                if (m_DataReader == null)
                    return false;
                return true;
            }
        }

        public bool IsClosed
        {
            get
            {
                if (m_Connection == null)
                    return false;
                return true;
            }
        }

        private void SetValue()
        {
            m_Connection = null;
            m_Tranc = null;
            m_SqlCmd = null;
            m_DataReader = null;
        }

        public SqlDataReaderInfo(SqlConnection con, SqlTransaction tranc = null, SqlCommand cmd = null, SqlDataReader reader = null)
        {
            m_Connection = con;
            m_Tranc = tranc;
            m_SqlCmd = cmd;
            m_DataReader = reader;
        }     
  
        public bool Commit()
        {
            bool bResult = false;
            try
            {
                if (m_Tranc != null)
                    m_Tranc.Commit();               

                bResult = true;

            }
            catch (System.Exception ex)
            {
                if (m_Tranc != null)
                    m_Tranc.Rollback();
                bResult = false;

                logger.Debug("SQL 실행에러 : ", ex);
            }           
            return bResult;
        }

        public bool RollBack()
        {
            bool bResult = false;
            try
            {
                if (m_Tranc != null)
                    m_Tranc.Rollback();

                bResult = true;

            }
            catch (System.Exception ex)
            {
                if (m_Tranc != null)
                    m_Tranc.Rollback();
                bResult = false;

                logger.Debug("SQL 실행에러 : ", ex);
            }           
            return bResult;
        }

        public bool Close()
        {
            bool bResult = false;
            try
            {
                if( m_DataReader != null && m_DataReader.IsClosed == false)
                    m_DataReader.Close();

                if (m_SqlCmd != null)
                    m_SqlCmd.Dispose();

                if (m_Tranc != null)
                    m_Tranc.Commit();

                bResult = true;

            }
            catch (System.Exception ex)
            {
                if (m_Tranc != null)
                    m_Tranc.Rollback();
                bResult = false;

                logger.Debug("SQL 실행에러 : ", ex);
            }
            finally
            {
                try
                {
                    if (m_Connection != null)
                        m_Connection.Close();
                }
                catch (System.Exception)
                {
                }
            }
            SetValue();
            return bResult;
        }

    }
}
