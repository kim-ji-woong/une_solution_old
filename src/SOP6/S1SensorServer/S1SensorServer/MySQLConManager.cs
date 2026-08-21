using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using DBUtility2;

namespace S1SensorServer
{
    class MySQLConManager
    {
        private MySqlConnection m_dbConnection;
        public MySqlConnection Connection
        {
            get { return m_dbConnection; }
        }

        private string m_strConnection;

        // Server Connection Info
        private string m_strServerIP = "";//"127.0.0.1";
        private string m_strServerPort = "";
        private string m_strServerDB = "";

        /// <summary>
        /// UNE
        /// </summary>
        private string m_strServerID = "";
        private string m_strServerPW = "";

        /// <summary>
        /// 삼천포 DB
        /// </summary>
        //private string m_strServerID = "sa";
        //private string m_strServerPW = "sa1234";

        private Utility m_ini = new Utility();
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public MySQLConManager()
        {

            string strSection = "Server Connection Info";
            m_strServerIP = m_ini.getinivalue(strSection, "server_ip");
            m_strServerPort = m_ini.getinivalue(strSection, "server_port");
            m_strServerDB = m_ini.getinivalue(strSection, "server_db");
            //PSMSensorServer.WriteLine("READ IP : " + m_strServerIP);

            try
            {
                string idpass = m_ini.getinivalue(strSection, "dbCon");
                //PSMSensorServer.WriteLine("READ ENC : " + idpass);
                string strDec = AES256Cipher.AES_decrypt(idpass, key);
                //PSMSensorServer.WriteLine("READ DEC : " + strDec);
                m_strServerID = strDec.Substring(0, strDec.IndexOf('|'));
                m_strServerPW = strDec.Substring(strDec.IndexOf('|') + 1);

                //PSMSensorServer.WriteLine("READ ID : " + m_strServerID);
                //PSMSensorServer.WriteLine("READ PASS : " + m_strServerPW);
            }
            catch (System.Exception e)
            {
                S1SensorServer.WriteLine(e.Message);
            }

            m_strConnection = GetConnectionInfo();
            //PSMSensorServer.WriteLine(m_strConnection);
            m_dbConnection = new MySqlConnection(m_strConnection);
        }

        public string GetConnectionInfo()
        {
            string strConnection = "";

            strConnection = "server=" + m_strServerIP + ";" +
                            "database=" + m_strServerDB + ";" +
                            "uid=" + m_strServerID + ";" +
                            "password=" + m_strServerPW + ";";

            return strConnection;
        }

        public bool OpenConnection()
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
                S1SensorServer.WriteLine(e.Message);
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                m_dbConnection.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
