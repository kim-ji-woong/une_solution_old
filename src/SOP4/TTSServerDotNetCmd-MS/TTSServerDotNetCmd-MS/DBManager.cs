using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TTSServerDotNetCmd
{
    
    class DBManager
    {
        private bool bCheckHeartBeat = false;
        private SqlConnection m_dbConnection;
        private Utility m_ini = new Utility();
        public TTSServerDotNetCmd.Utility Ini
        {
            get { return m_ini; }
            set { m_ini = value; }
        }
        private string m_strConnection;
        private bool m_isConnection = false;

        // Server Connection Info
        private string m_strServerIP = "127.0.0.1";//"127.0.0.1";
        private string m_strServerPort = "3389";
        private string m_strServerDB = "EDU_100";
        
        /// <summary>
        /// UNE
        /// </summary>
        private string m_strServerID = "sa";
        private string m_strServerPW = "9449966Ab";

        /// <summary>
        /// 삼천포 DB
        /// </summary>
        //private string m_strServerID = "sa";
        //private string m_strServerPW = "sa1234";

        private ArrayList m_arBroadcastMsg = new ArrayList();
                
        private string tMsg = "";        
        public string TMsg
        {
            get { return tMsg; }
            set { tMsg = value; }
        }

        private int mMode = 1;
        public int Mode
        {
            get { return mMode; }
            set { mMode = value; }
        }

        private BroadcastMessage mLastMsg = null;
        public BroadcastMessage LastMessege
        {
            get { return mLastMsg; }
            set { mLastMsg = value; }
        }

        private bool bRecvMsg = false;
        public bool RecvMsg
        {
            get { return bRecvMsg; }
            set { bRecvMsg = value; }
        }
        
        public DBManager()
        { 
            // DB 열기
           // Load_ConnectionInfo();
            m_strConnection = GetConnectionInfo();
            m_dbConnection = new SqlConnection(m_strConnection);

            CheckHeartBeat();
        }
		private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private void Load_ConnectionInfo()
        {
            string strSection = "Server Connection Info";
            
            m_strServerIP = m_ini.getinivalue(strSection, "server_ip");
            m_strServerPort = m_ini.getinivalue(strSection, "server_port");
            m_strServerDB = m_ini.getinivalue(strSection, "server_db");

            string szSiteID = m_ini.getinivalue(strSection, "siteid");
            int.TryParse(szSiteID, out m_nSiteID);
            try
            {
                mMode = int.Parse(m_ini.getinivalue(strSection, "mode"));
                tMsg = m_ini.getinivalue(strSection, "msg");
            }
            catch (System.Exception ex)
            {
                mMode = 0;
                tMsg = "";
            }

			try
			{

				string idpass = m_ini.getinivalue(strSection, "dbCon");
				string strDec = DBUtility.AES256Cipher.AES_decrypt(idpass, key);

				m_strServerID = strDec.Substring(0, strDec.IndexOf('|'));
				m_strServerPW = strDec.Substring(strDec.IndexOf('|') + 1);

			}
			catch (System.Exception ex)
			{

			}
        }

        // LocalDB 사용인지 여부
        private bool bLocal = false;
        private int m_nSiteID = 100;
        private string GetConnectionInfo()
        {
            string strConnection = "";
            if( bLocal == true)
            {
                strConnection = "Data Source=(localdb)\\V11.0;Initial Catalog=SOP_1;Integrated Security=True;Pooling=False;AttachDbFileName=C:\\ProgramData\\SOP\\SOP_1.mdf";
                return strConnection;
            }
            else
            {
                Load_ConnectionInfo();
            }
      
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
                m_strConnection = GetConnectionInfo();
                m_dbConnection = new SqlConnection(m_strConnection);               
                m_dbConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
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
                MessageBox.Show(e.StackTrace);
                return false;
            }
        }
               
        public void ReadMessage()
        {
            string szSQL = "SELECT Text,UseSiren,PlayOption,RepeatCount,AddTime from Broadcast";
            m_arBroadcastMsg.Clear();
            SqlDataReader reader = null;
            if (OpenConnection() == false)
            {
                reader = null;
                return;
            }

            SqlCommand cmd = new SqlCommand(szSQL, m_dbConnection);
            reader = cmd.ExecuteReader();

            bool bNewBroadCast = false;
            if (reader != null && reader.HasRows == true )
            {
                
                while (reader.Read())
                {
                    BroadcastMessage data = new BroadcastMessage();
                    string szTempMsg = reader.GetString(0);                 

                    szTempMsg = szTempMsg.Replace( (char)6, '\n');
                    szTempMsg = szTempMsg.Replace( (char)7, '\r');
                    data.Message = szTempMsg;

                    data.UseSiren = reader.GetBoolean(1);
                    data.PlayOption = reader.GetInt32(2);
                    data.RepeatCount = reader.GetInt32(3);
                    data.AddTime = reader.GetDateTime(4);
                    
                    if (data.PlayOption != -1 && data.ID != -1)
                    {
                        if (data.PlayOption == 1)
                        {
                            bNewBroadCast = true;  
                        }
                        m_arBroadcastMsg.Add(data);                        
                    }   
                }            
            }
            CloseConnection();
            int nMsg = m_arBroadcastMsg.Count;
            if (bNewBroadCast == false)
            {
                if (nMsg == 0)
                {
                    RecvMsg = false;
                    mLastMsg = null;
                }
                else
                {
                    RecvMsg = true;
                    mLastMsg = (BroadcastMessage)m_arBroadcastMsg[nMsg - 1];
                    //if (nMsg > 10)
                    {
                        ClearMessage();
                    }
                }
            }
            else
            {
                mLastMsg = (BroadcastMessage)m_arBroadcastMsg[nMsg-1];
                RecvMsg = true;
                //if (nMsg > 10)
                {                    
                    ClearMessage();
                }
            }            
        }

        public void ClearMessage()
        {
            string szSQL = " DELETE from Broadcast";

            if (OpenConnection() == false)
            {
                return;
            }

            try
            {
                SqlCommand cmd = new SqlCommand(szSQL, m_dbConnection);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader != null && reader.HasRows == true)
                {
                    bCheckHeartBeat = true;
                }
            }
            catch (System.Exception e)
            {
                bCheckHeartBeat = false;
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
            CloseConnection();
        }

        public void AddMessage(BroadcastMessage msg)
        {
            if( msg == null)
                return;
            
            DateTime nDate = DateTime.Now;
                       
            string szSQL = string.Format("INSERT INTO Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime) VALUES('{0}','{1}','{2}','{3}','{4} {5:00}:{6:00}:{7:00}')",
                msg.Message,msg.UseSiren, msg.PlayOption, msg.RepeatCount, nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second);

            string szSQL2 = string.Format("INSERT INTO BroadcastHistory (Text, UseSiren, PlayOption, RepeatCount, HostInfo, AddTime) VALUES('{0}','{1}','{2}','{3}','{4}', '{5} {6:00}:{7:00}:{8:00}')",
               msg.Message, msg.UseSiren, msg.PlayOption, msg.RepeatCount, "", nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second);
            

            if (OpenConnection() == false)
            {
                return;
            }

            SqlCommand cmd = new SqlCommand(szSQL, m_dbConnection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Close();

            SqlCommand cmd2 = new SqlCommand(szSQL2, m_dbConnection);
            SqlDataReader reader2 = cmd2.ExecuteReader();
            reader2.Close();


            CloseConnection();
        }
        
        private void CheckHeartBeat()
        {
            string szSQL = " SELECT BroadcastState.HOSTADDRESS, BroadcastState.HEARTBEAT, BroadcastState.BSTATE, BroadcastState.BDescription from BroadcastState";

            if (OpenConnection() == false)
            {
                return;
            }

            try
            {
                SqlCommand cmd = new SqlCommand(szSQL, m_dbConnection);
                SqlDataReader reader = cmd.ExecuteReader();               
                if (reader != null && reader.HasRows == true)
                {
                    bCheckHeartBeat = true;
                }
            }
            catch (System.Exception e)
            {
                bCheckHeartBeat = false;
                MessageBox.Show(e.Message + "\n" + e.StackTrace);

            }            
            CloseConnection();
        }


        private void InsertHeartBeat()
        {
            DateTime nDate = DateTime.Now;

            //string szSQL = string.Format("INSERT INTO BroadcastState (HOSTADDRESS, HEARTBEAT, BSTATE, BDescription) VALUES ('', '{0} {1:00}:{2:00}:{3:00}', 0, '')"
            //    , nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second);
            string szSQL = string.Format("INSERT INTO BroadcastState (HOSTADDRESS, HEARTBEAT, BSTATE, BDescription, SiteID) VALUES ('', '{0} {1:00}:{2:00}:{3:00}', 0, '', {4})"
                , nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, m_nSiteID);

           
            if (OpenConnection() == false)
            {             
                return;
            }

            SqlCommand cmd = new SqlCommand(szSQL, m_dbConnection);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Close();
                       
            CloseConnection();       
        }
            
        public void HeartBeat(int nState)
        {
            if( bCheckHeartBeat == false)
            {
                try
                {
                    InsertHeartBeat();
                    bCheckHeartBeat = true;
                }
                catch (System.Exception)
                {                
                }               
            }
            
            DateTime nDate = DateTime.Now;
            string szSQL = string.Format("UPDATE BroadcastState SET HEARTBEAT= '{0} {1:00}:{2:00}:{3:00}', BSTATE ={4} WHERE ID = 1"
                , nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, nState);
            
            if (OpenConnection() == false)
            {               
                return;
            }

            try
            {
                SqlCommand cmd = new SqlCommand(szSQL, m_dbConnection);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            } 
            CloseConnection();
        }

    }

    class BroadcastMessage
    {
        protected int mID;
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        protected string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        protected bool bUseSiren;
        public bool UseSiren
        {
            get { return bUseSiren; }
            set { bUseSiren = value; }
        }
        protected int mplayOption;
        public int PlayOption
        {
            get { return mplayOption; }
            set { mplayOption = value; }
        }
        protected int mRepeatCount;
        public int RepeatCount
        {
            get { return mRepeatCount; }
            set { mRepeatCount = value; }
        }

        protected DateTime mAddedTime;
        public System.DateTime AddTime
        {
            get { return mAddedTime; }
            set { mAddedTime = value; }
        }
    }    
}
