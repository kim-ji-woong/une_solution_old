using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
//using System.Data.SqlClient;
using System.Windows.Forms;
using MySql.Data.MySqlClient;



namespace TTSServerDotNetCmd
{
    
    class MySQLDBManager
    {
        private bool bCheckHeartBeat = false;
        private MySqlConnection m_dbConnection;
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
        private string m_strServerPort = "3306";
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

        public MySQLDBManager()
        { 
            // DB 열기
            Load_ConnectionInfo();
            //m_strConnection = GetConnectionInfo();
            //m_dbConnection = new MySqlConnection(m_strConnection);

            CheckHeartBeat();
        }
		private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private int m_nSiteID = 100;
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

        private string GetConnectionInfo()
        {
            string strConnection = "";
            if( bLocal == true)
            {                
                strConnection = "Data Source=(localdb)\\V11.0;Initial Catalog=SOP_1;Integrated Security=True;Pooling=False;AttachDbFileName=C:\\ProgramData\\SOP\\SOP_1.mdf";
                return strConnection;
            }

            strConnection = "Server=" + m_strServerIP + ";" +
                            "Database=" + m_strServerDB + ";" +
                            "Uid=" + m_strServerID + ";" +
                            "Pwd=" + m_strServerPW + ";CharSet=utf8;";
            return strConnection;
        }

        private bool OpenConnection()
        {
            try
            {
                m_strConnection = GetConnectionInfo();
                m_dbConnection = new MySqlConnection(m_strConnection);               
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
              try
                {
                    string szSQL = "SELECT  Text, UseSiren, PlayOption, RepeatCount from Broadcast";
                    m_arBroadcastMsg.Clear();
                    MySqlDataReader reader = null;
                    if (OpenConnection() == false)
                    {
                        reader = null;
                        return;
                    }

                    MySqlCommand cmd = new MySqlCommand(szSQL, m_dbConnection);
                    reader = cmd.ExecuteReader();

                    bool bNewBroadCast = false;
                    if (reader != null && reader.HasRows == true)
                    {

                        while (reader.Read())
                        {
                            BroadcastMessage data = new BroadcastMessage();
                            string szTempMsg = reader.GetString(0);

                            szTempMsg = szTempMsg.Replace((char)6, '\n');
                            szTempMsg = szTempMsg.Replace((char)7, '\r');
                            data.Message = szTempMsg;

                            data.UseSiren = reader.GetBoolean(1);
                            data.PlayOption = reader.GetInt32(2);
                            data.RepeatCount = reader.GetInt32(3);
                           // data.AddTime = reader.GetDateTime(4);

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
                        mLastMsg = (BroadcastMessage)m_arBroadcastMsg[nMsg - 1];
                        RecvMsg = true;
                        //if (nMsg > 10)
                        {
                            ClearMessage();
                        }
                    }            
                }
            catch(Exception ex)
              {
                  System.Diagnostics.Trace.WriteLine(ex.Message);
                  System.Diagnostics.Trace.WriteLine(ex.Source);
                  System.Diagnostics.Trace.WriteLine(ex.StackTrace);
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
                MySqlCommand cmd = new MySqlCommand(szSQL, m_dbConnection);
                MySqlDataReader reader = cmd.ExecuteReader();
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

            MySqlCommand cmd = new MySqlCommand(szSQL, m_dbConnection);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Close();

            MySqlCommand cmd2 = new MySqlCommand(szSQL2, m_dbConnection);
            MySqlDataReader reader2 = cmd2.ExecuteReader();
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
                MySqlCommand cmd = new MySqlCommand(szSQL, m_dbConnection);
                MySqlDataReader reader = cmd.ExecuteReader();               
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

            string szSQL = string.Format("INSERT INTO BroadcastState (HOSTADDRESS, HEARTBEAT, BSTATE, BDescription, SiteID) VALUES ('', '{0} {1:00}:{2:00}:{3:00}', 0, '', {4})"
                , nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, m_nSiteID);

           
            if (OpenConnection() == false)
            {             
                return;
            }

            MySqlCommand cmd = new MySqlCommand(szSQL, m_dbConnection);
            MySqlDataReader reader = cmd.ExecuteReader();
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
                MySqlCommand cmd = new MySqlCommand(szSQL, m_dbConnection);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            } 
            CloseConnection();
        }

    }

    
}
