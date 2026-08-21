using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.Sql;
using System.Data.SqlClient;

namespace DigitalTwin
{
    public partial class FormMain : Form
    {
        private class DBData
        {
            private string m_strIP = "";
            private string m_strID = "";
            private string m_strPW = "";

            public string IP
            {
                get { return m_strIP; }
                set { m_strIP = value; }
            }

            public string ID
            {
                get { return m_strID; }
                set { m_strID = value; }
            }

            public string PW
            {
                get { return m_strPW; }
                set { m_strPW = value; }
            }
        }

        private List<Sensor> m_sensors = new List<Sensor>();
        private DBData m_db = null;

        public FormMain()
        {
            InitializeComponent();

            ReadDBInfo();

            if (m_db != null)
                ReadDB(true);
            else
                MakeDefaultSensors();

            InitGrid();
        }

        private void InitGrid()
        {
            foreach (Sensor sensor in m_sensors)
            {
                int nRowIndex = gridSensors.Rows.Add();
                DataGridViewRow row = gridSensors.Rows[nRowIndex];

                row.Cells[0].Value = sensor.ID;
                row.Cells[1].Value = sensor.Name;
                row.Cells[2].Value = sensor.Unit;
                row.Cells[3].Value = sensor.Value;
                row.Cells[4].Value = sensor.GetStatus();
                row.Tag = sensor;

                sensor.DataRow = row;
            }
        }

        private void ReadDB(bool first = false)
        {
            string strConnection = GetConnectionString();

            try
            {
                using (SqlConnection connection = new SqlConnection(strConnection))
                {
                    connection.Open();

                    string strSQL = "Select Sensor.ID, SensorName, SensorUnit, SensorValue, Warning, Minor, Major, Critical ";
                    strSQL += "from Sensor, SensorType where Sensor.SensorType = SensorType.ID";
                    SqlCommand cmd = new SqlCommand(strSQL, connection);

                    SqlDataReader reader = cmd.ExecuteReader();
                    int nColumnCount = reader.FieldCount;

                    while (reader.Read())
                    {
                        Sensor sensor = new Sensor();

                        for (int i = 0; i < nColumnCount; i++)
                        {
                            if (reader.IsDBNull(i))
                                continue;

                            if (i == 0)
                                sensor.ID = reader.GetInt32(i);
                            else
                            {
                                string strValue = reader.GetValue(i).ToString();

                                if (i == 1)
                                    sensor.Name = strValue;
                                else if (i == 2)
                                    sensor.Unit = strValue;
                                else if (i == 3)
                                    sensor.Value = strValue;
                                else if (i == 4)
                                    sensor.Warning = float.Parse(strValue);
                                else if (i == 5)
                                    sensor.Minor = float.Parse(strValue);
                                else if (i == 6)
                                    sensor.Major = float.Parse(strValue);
                                else if (i == 7)
                                    sensor.Critical = float.Parse(strValue);
                            }
                        }

                        if (first)
                            m_sensors.Add(sensor);
                        else
                        {
                            Sensor sensor2 = GetSensor(sensor.ID);

                            if (sensor2 != null)
                            {
                                sensor2.DataRow.Cells[3].Value = sensor.Value;
                                sensor2.DataRow.Cells[4].Value = sensor.GetStatus();
                            }
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        private Sensor GetSensor(int nID)
        {
            foreach (Sensor sensor in m_sensors)
            {
                if (sensor.ID == nID)
                    return sensor;
            }

            return null;
        }

        private bool ReadDBInfo()
        {
            string strIP = ConfigurationManager.AppSettings.Get("Server");

            if (strIP == null)
                return false;

            string strID = ConfigurationManager.AppSettings.Get("ID");

            if (strID == null)
                return false;

            string strPW = ConfigurationManager.AppSettings.Get("PW");

            if (strPW == null)
                return false;

            m_db = new DBData();
            m_db.IP = strIP;
            m_db.ID = strID;
            m_db.PW = strPW;

            string strConnection = GetConnectionString();

            try
            {
                SqlConnection connection = new SqlConnection(strConnection);
                connection.Open();
                connection.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                m_db = null;
            }

            return true;
        }

        private string GetConnectionString()
        {
            return string.Format("Data Source={0};Initial Catalog=UnE_Sensor;User ID={1};Password={2};", m_db.IP, m_db.ID, m_db.PW);
        }

        private void MakeDefaultSensors()
        {
            Sensor sensor = new Sensor();
            sensor.ID = 1;
            sensor.Name = "온도";
            sensor.Unit = "℃";
            sensor.Value = "25";
            sensor.Warning = 30;
            sensor.Minor = 40;
            sensor.Major = 45;
            sensor.Critical = 50;
            m_sensors.Add(sensor);

            sensor = new Sensor();
            sensor.ID = 2;
            sensor.Name = "가속도";
            sensor.Unit = "G";
            sensor.Value = "0.0";
            sensor.Warning = 0.02f;
            sensor.Minor = 0.05f;
            sensor.Major = 0.07f;
            sensor.Critical = 0.1f;
            m_sensors.Add(sensor);

            sensor = new Sensor();
            sensor.ID = 3;
            sensor.Name = "균열";
            sensor.Unit = "mm";
            sensor.Value = "0.21";
            sensor.Warning = 0.1f;
            sensor.Minor = 0.2f;
            sensor.Major = 0.3f;
            sensor.Critical = 0.5f;
            m_sensors.Add(sensor);

            sensor = new Sensor();
            sensor.ID = 4;
            sensor.Name = "변형";
            sensor.Unit = "με";
            sensor.Value = "1200";
            sensor.Warning = 1000;
            sensor.Minor = 2000;
            sensor.Major = 3000;
            sensor.Critical = 4000;
            m_sensors.Add(sensor);

            sensor = new Sensor();
            sensor.ID = 5;
            sensor.Name = "기울기";
            sensor.Unit = "Degree";
            sensor.Value = "0.25";
            sensor.Warning = 0.08f;
            sensor.Minor = 0.12f;
            sensor.Major = 0.23f;
            sensor.Critical = 0.38f;
            m_sensors.Add(sensor);

            sensor = new Sensor();
            sensor.ID = 6;
            sensor.Name = "수위";
            sensor.Unit = "m";
            sensor.Value = "0.7";
            sensor.Warning = 0.15f;
            sensor.Minor = 0.3f;
            sensor.Major = 0.4f;
            sensor.Critical = 0.5f;
            m_sensors.Add(sensor);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (m_db != null)
                timer1.Start();
        }

        private void OnTimer(object sender, EventArgs e)
        {
            ReadDB();
        }
    }
}
