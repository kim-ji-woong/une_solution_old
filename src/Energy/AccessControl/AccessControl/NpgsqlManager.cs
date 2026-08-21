using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace AccessControl
{
    public class NpgsqlManager
    {        
        private NpgsqlConnection m_connection = null;
        private string m_strConnstring = "";
        public string ConnString
        {
            get { return m_strConnstring; }
            set { m_strConnstring = value; }
        }

        private bool m_bconnect = false;
        public bool Connect
        {
            get { return m_bconnect; }
            set { m_bconnect = value; }
        }

        public bool Open()
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(m_strConnstring);
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    m_connection = connection;
                    
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                m_connection = null;
                m_bconnect = false;
                return false;
            }

            m_bconnect = true;
            return true;
        }

        public ArrayList GetResultData(string strSQL)
        {
            return RunQuery(strSQL);
        }

        private ArrayList RunQuery(string strSQL)
        {
            if (m_connection == null || m_connection.State != System.Data.ConnectionState.Open)
            {
                return null;
            }
            
            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(strSQL, m_connection);
                NpgsqlDataReader reader = cmd.ExecuteReader();

                ArrayList datas = new ArrayList();

                int nColumnCount = reader.FieldCount;

                while (reader.Read())
                {
                    for (int i = 0; i < nColumnCount; i++)
                    {
                        if (reader.IsDBNull(i))
                            AddNullData(datas);
                        else
                            AddData(datas, reader.GetValue(i));
                    }
                }

                reader.Close();
                return datas;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void AddNullData(ArrayList datas)
        {
            datas.Add("~");
        }

        private void AddData(ArrayList datas, object data)
        {
            datas.Add(data.ToString());
        }
    }
}
