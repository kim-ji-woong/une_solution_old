using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;

namespace ControlMonitoring
{
    class DBManager
    {
        private ControlMonitoring m_Main = null;
        private SqlConnection m_dbConnection;
        private Utility m_ini = new Utility();

        private string m_strConnection;
        private bool m_isConnection = false;

        // Server Connection Info
        private string m_strServerIP = "192.168.0.207";//"127.0.0.1";
        private string m_strServerPort = "9433";
        private string m_strServerDB = "SOP3";
        private string m_strServerID = "sa";
        private string m_strServerPW = "9449966Ab";

        private ArrayList m_arrUserInfo = new ArrayList();
        //private ArrayList m_arrCompanyMember = new ArrayList();
        //private ArrayList m_arrGenLevel = new ArrayList();
        //private ArrayList m_arrGenUser = new ArrayList();

        public DBManager(ControlMonitoring main)
        {
            m_Main = main;

            // DB 열기
            Load_ConnectionInfo();
            m_strConnection = GetConnectionInfo();
            m_dbConnection = new SqlConnection(m_strConnection);

            m_isConnection = OpenConnection();
        }

        private void Load_ConnectionInfo()
        {
            string strSection = "Server Connection Info";

            m_strServerIP = m_ini.getinivalue(strSection, "server_ip");
            m_strServerPort = m_ini.getinivalue(strSection, "server_port");
            m_strServerDB = m_ini.getinivalue(strSection, "server_db");
            //m_strServerID = m_ini.getinivalue(strSection, "server_id");
            //m_strServerPW = m_ini.getinivalue(strSection, "server_pw");
        }

        private string GetConnectionInfo()
        {
            string strConnection = "";

            strConnection = "server=" + m_strServerIP + ";" +
                            "database=" + m_strServerDB + ";" +
                            "uid=" + m_strServerID + ";" +
                            "password=" + m_strServerPW + ";";

            return strConnection;
        }

        private bool OpenConnection()
        {
            try
            {
                m_dbConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                m_dbConnection.Close();
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                return false;
            }
        }

        private void ReadDB(string strSQL, out SqlDataReader reader)
        {
            SqlCommand cmd = new SqlCommand(strSQL, m_dbConnection);
            reader = cmd.ExecuteReader();
        }

        public void TakeControl()
        {
            int nControllerID = Load_Controller();
            if(nControllerID < 0)
            {
                Load_UserInfo(ref m_arrUserInfo); //현재 로그인한 사용자 중
                if (m_arrUserInfo.Count == 0) return;

                ArrayList arrRequestControl = new ArrayList();
                arrRequestControl = FindRequestContol(m_arrUserInfo); //제어권 요청자를 찾음

                if (arrRequestControl.Count == 0) //제어권 요청자가 없는 경우
                {
                    ArrayList arrUser = new ArrayList();
                    arrUser = FindUser(m_arrUserInfo);

                    if (arrUser.Count == 0) return;
                    else //모니터링 사용자를 찾아 제어권을 강제로 넘겨줌
                    {
                        foreach (ControllerInfo data in arrUser)
                        {
                            Update_Controller(data.UserID);
                            break;
                        }
                    }
                }
                else //flag = 1인 사용자가 있는 경우
                {
                    foreach (ControllerInfo data in arrRequestControl)
                    {
                        Update_Controller(data.UserID);
                        Update_Reset();

                        break;
                    }
                }
            }
        }

        public void CheckLogin(int nContollerID)
        {

        }

        // ControlCheck가 1인 UserID를 찾아 레벨이 높은자를 기준으로 정렬
        private ArrayList FindRequestContol(ArrayList arrUserInfo)
        {
            string strSQL = "SELECT ControlCheck.ID, ControlCheck.UserID, ControlCheck.Time, ControlCheck.ControlCheck, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel, SOPGenLevel.LevelName " +
                            "FROM ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
                            "where ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID and SOPGenUser.UserLevel = SOPGenLevel.ID and  ControlCheck.ControlCheck = 1 " +
                            "order by SOPGenUser.UserLevel DESC";

            SqlDataReader reader;
            ReadDB(strSQL, out reader);

            DateTime dt = new DateTime();
            ArrayList arrController = new ArrayList();

            while (reader.Read())
            {
                ControllerInfo data = new ControllerInfo();
                data.ID = GetField<int>(reader[0], -1);
                data.UserID = GetField<int>(reader[1], -1);
                data.Time = GetField<DateTime>(reader[2], dt);
                
                TimeSpan ts = DateTime.Now - data.Time;
                if (ts.TotalMilliseconds > 15000)
                {
                    continue;
                }

                data.ControlCheck = GetField<int>(reader[3], -1);
                data.MemberName = GetField<string>(reader[3], "");
                data.MemberID = GetField<string>(reader[4], "");

                foreach (ControllerInfo info in arrUserInfo)
                {
                    if(info.UserID == data.UserID)
                    {
                        arrController.Add(data);
                        break;
                    }
                }
            }

            reader.Close();
            return arrController;
        }

        // ControlCheck가 -1인 UserID를 찾아 레벨이 높은자를 기준으로 정렬
        private ArrayList FindUser(ArrayList arrUserInfo)
        {
            string strSQL = "SELECT ControlCheck.ID, ControlCheck.UserID, ControlCheck.Time, ControlCheck.ControlCheck, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel, SOPGenLevel.LevelName " +
                            "FROM ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
                            "where ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID and SOPGenUser.UserLevel = SOPGenLevel.ID and  ControlCheck.ControlCheck = -1 " +
                            "order by SOPGenUser.UserLevel DESC";

            SqlDataReader reader;
            ReadDB(strSQL, out reader);

            DateTime dt = new DateTime();
            ArrayList arrController = new ArrayList();

            while (reader.Read())
            {
                ControllerInfo data = new ControllerInfo();
                data.ID = GetField<int>(reader[0], -1);
                data.UserID = GetField<int>(reader[1], -1);
                data.Time = GetField<DateTime>(reader[2], dt);

                TimeSpan ts = DateTime.Now - data.Time;
                if (ts.TotalMilliseconds > 15000)
                {
                    continue;
                }
                data.ControlCheck = GetField<int>(reader[3], -1);
                data.MemberName = GetField<string>(reader[3], "");
                data.MemberID = GetField<string>(reader[4], "");

                foreach (ControllerInfo info in arrUserInfo)
                {
                    if (info.UserID == data.UserID)
                    {
                        arrController.Add(data);
                        break;
                    }
                }
            }

            reader.Close();
            return arrController;
        }

        public void ReturnCheck(ref int nUserID)
        {
            string strSQL = "SELECT ControlCheck.UserID " +
                            "from ControlUser, ControlCheck " +
                            "where ControlCheck.ControlCheck = 0";

            SqlDataReader reader;
            ReadDB(strSQL, out reader);

            while (reader.Read())
            {
                nUserID = GetField<int>(reader[0], -1);
            }

            reader.Close();
        }

        public int Load_Controller()
        {
            string strSQL = "SELECT ControlUser.UserID, ControlCheck.Time FROM ControlUser, ControlCheck WHERE ControlUser.UserID = ControlCheck.UserID and ControlCheck.ControlCheck <> 0";

            SqlDataReader reader;
            ReadDB(strSQL, out reader);

            DateTime dt = new DateTime();
            while (reader.Read())
            {
                int nControlUserID = GetField<int>(reader[0], -1);
                dt = GetField<DateTime>(reader[1], dt);
                TimeSpan ts = DateTime.Now - dt;
                if(ts.TotalMilliseconds > 15000) // 현재 로그인되지 않음
                {
                    break;
                }
                else
                {
                    reader.Close();
                    return nControlUserID;
                }
            }

            reader.Close();
            return -1;
        }

        //접속중인 사용자를 검색
        public void Load_UserInfo(ref ArrayList arrUserInfo)
        {
            arrUserInfo.Clear();

            string strSQL = "SELECT ControlCheck.ID, ControlCheck.UserID, ControlCheck.Time, ControlCheck.ControlCheck, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel, SOPGenLevel.LevelName " +
                            "FROM ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
                            "where ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID and SOPGenUser.UserLevel = SOPGenLevel.ID";

            SqlDataReader reader;
            ReadDB(strSQL, out reader);

            DateTime dateTime = new DateTime();
            while (reader.Read())
            {
                ControllerInfo dataNew = new ControllerInfo();
                dataNew.ID = GetField<int>(reader[0], -1);
                dataNew.UserID = GetField<int>(reader[1], -1);
                dataNew.Time = GetField<DateTime>(reader[2], dateTime);
                dataNew.ControlCheck = GetField<int>(reader[3], -1);
                dataNew.MemberName = GetField<string>(reader[4], "");
                dataNew.MemberID = GetField<string>(reader[5], "");
                dataNew.UserLevel = GetField<int>(reader[6], -1);
                dataNew.LevelName = GetField<string>(reader[7], "");

                arrUserInfo.Add(dataNew);
            }

            reader.Close();
        }

        public void Load_ControlCheck(ref ArrayList arrControlCheck)
        {
            arrControlCheck.Clear();

            string strSQL = "select ID, UserID, Time, ControlCheck from ControlCheck";

            SqlDataReader reader;
            ReadDB(strSQL, out reader);

            DateTime dateTime = new DateTime();
            while (reader.Read())
            {
                ControlCheckData dataNew = new ControlCheckData();
                dataNew.ID = GetField<int>(reader[0], -1);
                dataNew.UserID = GetField<int>(reader[1], -1);
                dataNew.Time = GetField<DateTime>(reader[2], dateTime);
                dataNew.ControlCheck = GetField<bool>(reader[3], false);

                arrControlCheck.Add(dataNew);
            }

            reader.Close();
        }

        private void Update_Controller(int nUserID)
        {
            string strSQL = string.Format("update ControlUser set UserID = {0}", nUserID);

            SqlCommand cmd = new SqlCommand(strSQL, m_dbConnection);
            cmd.ExecuteNonQuery();
        }

        private void Update_Reset()
        {
            string strSQL = "update ControlCheck set ControlCheck = -1 where ControlCheck = 1";

            SqlCommand cmd = new SqlCommand(strSQL, m_dbConnection);
            cmd.ExecuteNonQuery();
        }

        public void Update_ControlCheck(ControlCheckData data)
        {
            string strSQL = string.Format("update ControlCheck set ControlCheck = {0} where id = {1}", data.ControlCheck ? 1 : 0, data.ID);

            SqlCommand cmd = new SqlCommand(strSQL, m_dbConnection);
            cmd.ExecuteNonQuery();
        }

        private T GetField<T>(object dataSrc, T dataDefault)
        {
            T result;

            try
            {
                result = (T)dataSrc;
            }
            catch (Exception)
            {
                result = dataDefault;
            }

            return result;
        }

    }

    public class ControllerInfo
    {
        private int m_nID;
        private int m_nUserID;
        private DateTime m_time;
        private int m_nControlCheck;
        private string m_strMemberName;
        private string m_strMemberID;
        private int m_nUserLevel;
        private string m_strLevelName;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }
        public System.DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }
        public int ControlCheck
        {
            get { return m_nControlCheck; }
            set { m_nControlCheck = value; }
        }
        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }
        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }
        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }
        public string LevelName
        {
            get { return m_strLevelName; }
            set { m_strLevelName = value; }
        }
    }

    public class ControlCheckData
    {
        private int m_nID;
        private int m_nUserID;
        private DateTime m_time;
        private bool m_isControlCheck;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }
        public System.DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }
        public bool ControlCheck
        {
            get { return m_isControlCheck; }
            set { m_isControlCheck = value; }
        }
    }

    public class CompanymemberData
    {
        private int m_nID;
        private string m_strMemberName;
        private int m_nRegularTeamID;
        private int m_nLevelID;
        private int m_nPositionID;
        private string m_strMemberID;
        private int m_nSecondRegularTeamID;
        private int m_nSecondPositionID;
        private string m_strPhoneNumber;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }
        public int RegularTeamID
        {
            get { return m_nRegularTeamID; }
            set { m_nRegularTeamID = value; }
        }
        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }
        public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }
        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }
        public int SecondRegularTeamID
        {
            get { return m_nSecondRegularTeamID; }
            set { m_nSecondRegularTeamID = value; }
        }
        public int SecondPositionID
        {
            get { return m_nSecondPositionID; }
            set { m_nSecondPositionID = value; }
        }
        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }
    }

    public class SOPGenLevelData
    {
        private int m_nID;
        private string m_strLevelName;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string LevelName
        {
            get { return m_strLevelName; }
            set { m_strLevelName = value; }
        }
    }

    public class SOPGenUserData
    {
        private int m_nID;
        private int m_nMemberID;
        private int m_nUserLevel;
        private string m_strPassword;
        private string m_strUserID;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }
        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }
        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }
        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }
    }
}
