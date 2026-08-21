using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace FireSignalSender
{
    public class DataManager
    {
        private static DataManager m_Instance = null;
        public static DataManager Instance
        {
            get 
            {
                if (m_Instance == null)
                {
                    m_Instance = new DataManager();                                        
                }
                return m_Instance;
            }
        }


        private DataManager()
        {
            m_dtLastRead = LoadLastReadTime();
#if !DEBUG
            m_dtLastRead = DateTime.Now;
#endif

            m_szPathDB = LoadFilePath();
        }

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
        }

        private string m_szPathDB = "";
        public string PathDB
        {
            get { return m_szPathDB; }
        }
        
        private DateTime m_dtLastRead;
        public DateTime LastSignalReadTime
        {
            get { return m_dtLastRead; }
            set { m_dtLastRead = value; }
        }

        private ArrayList m_arData = new ArrayList();
        private Dictionary<string, FireSignalInfo> m_SignalList = new Dictionary<string, FireSignalInfo>();


        public List<FireSignalInfo> GetSignalList()
        {
            List<FireSignalInfo> arSignals = new List<FireSignalInfo>(m_SignalList.Values);
            return arSignals;
        }

        public string LoadFilePath()
        {
            string szFilePath = RegUtil.ReadRegValue("FireSignalInfo", "FilePath", m_nSiteID);
            return szFilePath;
        }

        protected DateTime LoadLastReadTime()
        {
            DateTime dtNow = DateTime.Now;
            string szReadTime = RegUtil.ReadRegValue("FireSignalInfo", "LastReadTime", m_nSiteID);
            if (szReadTime == null || szReadTime == "")
            {
                RegUtil.WriteRegValue("FireSignalInfo", "LastReadTime", Utility.MakeDateTimeString(dtNow), m_nSiteID);
                return dtNow;
            }

            try
            {
                dtNow = Convert.ToDateTime(szReadTime);
            }
            catch (Exception)
            {
            }
            return dtNow;
        }

        private void SaveLastReadTime(DateTime dt)
        {
#if !DEBUG
            RegUtil.WriteRegValue("FireSignalInfo", "LastReadTime", MakeDateTimeString(dt), m_nSiteID);
#endif
        }

        public bool ReadFireSignalList()
        {
            bool bResult = false;
            m_arData.Clear();
            m_SignalList.Clear();

            string strConn = string.Format(@"Data Source={0};", m_szPathDB);

            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(strConn);

                conn.Open();
                //conn.ChangePassword("1234");
                //DateTime dt = Convert.ToDateTime("2016-09-07 15:34:06");
                //string sql = GetTableSQL(dt);
                string sql = GetSensorSignalSQL(m_dtLastRead);
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader rs = cmd.ExecuteReader();
                while (rs.Read())
                {
                    string szDate = rs.GetString(0);

                    DateTime dtRead = Convert.ToDateTime(szDate);
                    if (dtRead > m_dtLastRead)
                    {
                        m_dtLastRead = dtRead;
                    }

                    string szReciver = rs.GetValue(1).ToString();
                    string szCode = rs.GetString(4);
                    string szCodeExp = rs.GetString(5);
                    string szCircuit = rs.GetString(6);
                    string szName = rs.GetString(7);

                    FireSignalInfo info = new FireSignalInfo();
                    info.Time = dtRead;
                    info.Code = szCode;
                    info.Circuit = szCircuit;
                    info.ReciverNo = (int)rs.GetValue(1);
                    info.Name = szName;

                    string szKey = info.FuncionCode + "_" + szCircuit;

                    m_arData.Add(info);

                    //if (IsFireSignal(szCodeExp))
                    {
                        if (info.IsOff == true)
                        {
                            if (m_SignalList.ContainsKey(szKey))
                            {
                                m_SignalList.Remove(szKey);
                            }
                            else
                            {
                                m_SignalList.Add(szKey, info);
                            }
                        }
                        else
                        {
                            if (m_SignalList.ContainsKey(szKey))
                            {
                                m_SignalList.Remove(szKey);
                                m_SignalList.Add(szKey, info);
                            }
                            else
                                m_SignalList.Add(szKey, info);
                        }
                    }
                    //System.Diagnostics.Trace.WriteLine("Data : " + szDate + " , " + szReciver + "," + szCode + ", " + szCircuit + ", " + szName);
                }

                conn.Close();
                SaveLastReadTime(m_dtLastRead);
                bResult = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception: " + e.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return bResult;
        }


        private ArrayList m_arReciverState = new ArrayList();

        public ArrayList ReciverState
        {
            get { return m_arReciverState; }
            set { m_arReciverState = value; }
        }
        public bool ReadReciverState()
        {
            bool bResult = false;
            m_arReciverState.Clear();
            string strConn = string.Format(@"Data Source={0};", m_szPathDB);
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(strConn);

                conn.Open();

                string sql = GetReciverStateSQL();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                SQLiteDataReader rs = cmd.ExecuteReader();
                while (rs.Read())
                {                    
                    int szReciver = (int)rs.GetValue(0);
                    string szCode = rs.GetString(1);
                    string szCodeExp = rs.GetString(2);
                    string szName = rs.GetString(3);

                    m_arReciverState.Add(szReciver);

                    if (szCode == "POL")
                        m_arReciverState.Add(11);
                    else
                        m_arReciverState.Add(0);
                }
                conn.Close();
                bResult = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception: " + e.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return bResult;
        }

        private static string GetReciverResetSQL(DateTime dt)
        {
            string szSQL = "SELECT act_day, r_no, port, type, code, code_exp, circuit, circuit_exp " +
                " FROM t_log where code_exp like '수신기복구 On' " +
                " AND act_day > '" + Utility.MakeDateTimeString(dt) + "'" +
                " order by act_day ASC Limit 0,40";

            return szSQL;
        }

        private static string GetReciverStateSQL()
        {
            string szSQL = "SELECT r_no, code, code_exp, circuit_exp FROM t_log where code_exp = '통신이상' OR code_exp = 'POL' group by r_no order by r_no";
            return szSQL;
        }


        private static string GetSensorSignalSQL(DateTime dt)
        {
            return @"SELECT act_day, r_no, port, type, code, code_exp  , circuit, circuit_exp " +
                    " FROM t_log WHERE circuit_exp <> '미등록설비' AND  ( code_exp = '수신기복구 On' OR code_exp ='감시 On' OR code_exp='화재 On' OR code_exp ='감시 Off' OR code_exp='화재 Off') " +
                    //" AND act_day > '" + Utility.MakeDateTimeString(dt) + "'" +
                    " AND ( act_day BETWEEN '" + Utility.MakeDateTimeString(dt.AddSeconds(1.0)) +"' AND '" +  Utility.MakeDateTimeString(DateTime.Now.AddDays(100)) + "') "+
                    " order by act_day ASC Limit 0, 40";
        }       

    }
}
