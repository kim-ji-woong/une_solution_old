using System;
using System.Timers;
using System.ServiceProcess;
using System.Data.SqlClient;
using System.Configuration;

namespace LogRemover
{
    public partial class ServiceMain : ServiceBase
    {
        private Timer m_timer = null;
        private int m_nPrevYear = 0, m_nPrevMonth = 0, m_nPrevDay = 0;
        private string m_strDBName = "", m_strPW;

        public ServiceMain()
        {
            InitializeComponent();
            m_strDBName = ConfigurationManager.AppSettings.Get("dbName");
            m_strPW = ConfigurationManager.AppSettings.Get("pw");
        }

        protected override void OnStart(string[] args)
        {
            // 10분에 한번씩 동작
            m_timer = new Timer(600000);
            m_timer.Elapsed += OnTimer;
            m_timer.Start();

            OnTimer(null, null);
        }

        protected override void OnStop()
        {
            m_timer.Stop();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            DateTime dtNow = DateTime.Now;

            if (dtNow.Year != m_nPrevYear || dtNow.Month != m_nPrevMonth || dtNow.Day != m_nPrevDay)
            {
                m_nPrevYear = dtNow.Year;
                m_nPrevMonth = dtNow.Month;
                m_nPrevDay = dtNow.Day;
                RemoveOldLogs(m_strDBName, m_strPW, m_nPrevYear, m_nPrevMonth, m_nPrevDay);
            }
        }

        public static void RemoveOldLogs(string strDBName, string strPW, int year, int month, int day)
        {
            SqlConnection connection = Connect(strDBName, strPW);

            if (connection == null)
                return;

            if (RemoveStatisticsYear(connection, year))
            {
                if (RemoveStatisticsMonth(connection, year, month))
                {
                    if (RemoveStatisticsWeek(connection, year, month))
                    {
                        if (RemoveStatisticsDay(connection, year, month, day))
                        {
                            RemoveSensorData(connection, year, month, day);
                        }
                    }
                }
            }

            connection.Close();
        }

        private static bool RemoveSensorData(SqlConnection connection, int year, int month, int day)
        {
            // 오래된 로그를 삭제한다.
            DateTime temp = new DateTime(year, month, day);
            DateTime dtOld = temp.AddMonths(-12);

            string strSQL = string.Format("Delete from SensorData where year < {0} or (year = {0} and month < {1})", dtOld.Year, dtOld.Month);

            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private static bool RemoveStatisticsDay(SqlConnection connection, int year, int month, int day)
        {
            string strSQL = string.Format("Delete from SensorStatisticsDay where year < {0} or (year = {0} and month < {1}) or (year = {0} and month = {1} and day < {2})", year - 1, month, day);

            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private static bool RemoveStatisticsWeek(SqlConnection connection, int year, int month)
        {
            string strSQL = string.Format("Delete from SensorStatisticsWeek where year < {0} or (year = {0} and month < {1})", year - 1, month);

            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private static bool RemoveStatisticsMonth(SqlConnection connection, int year, int month)
        {
            string strSQL = string.Format("Delete from SensorStatisticsMonth where year < {0} or (year = {0} and month < {1})", year - 1, month);

            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private static bool RemoveStatisticsYear(SqlConnection connection, int year)
        {
            string strSQL = "Delete from SensorStatisticsYear where year <= " + (year - 2).ToString();

            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private static SqlConnection Connect(string strDBName, string strPW)
        {
            string strConnection = GetConnectionString(strDBName, strPW);

            try
            {
                SqlConnection connection = new SqlConnection(strConnection);
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                    return connection;
                else
                {
                    System.Diagnostics.Trace.WriteLine("DB 접속에 실패하였습니다. : " + strConnection);
                    return null;
                }
            }
            catch (Exception e)
            {
            }

            return null;
        }

        private static string GetConnectionString(string strDBName, string strPW)
        {
            return string.Format("Data Source=127.0.0.1;Initial Catalog={0};User ID=sa;Password={1};", strDBName, strPW);
        }
    }
}
