using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;


namespace ConsoleApplication2
{

    class ConManager
    {
        private SqlConnection m_dbConnection;
        public System.Data.SqlClient.SqlConnection Connection
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

       
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public ConManager()
        {
            m_strServerIP = "192.168.0.210";
            m_strServerPort = "1433";
            m_strServerDB = "SOP3";

            m_strServerID = "sa";
            m_strServerPW = "9449966Ab";

            //m_strConnection = GetConnectionInfo();            
            //m_dbConnection = new SqlConnection(m_strConnection);
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

        public bool OpenConnection()
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
            catch (Exception e)
            {
                return false;
            }
        }
    }
}

