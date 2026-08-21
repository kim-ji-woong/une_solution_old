using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;

namespace MetaData.Models
{
    public class SensorRepository
    {
        private static SqlConnection m_connection = null;
        private static Dictionary<int, Sensor> m_dicSensors = new Dictionary<int, Sensor>();
        private static Dictionary<Sensor, List<SensorValue>> m_dicSensorVaues = new Dictionary<Sensor, List<SensorValue>>();
        private static Dictionary<int, SensorValue> m_dicSensorValue2 = new Dictionary<int, SensorValue>();
        private static Dictionary<int, Region> m_dicRegions = new Dictionary<int, Region>();
        
        /*public static Dictionary<int, Sensor> Sensors
        {
            get { return m_dicSensors; }
        }

        public static Dictionary<int, Region> Regions
        {
            get { return m_dicRegions; }
        }*/

        public static int LastSensorID
        {
            get { return GetMaxID("Sensor"); }
        }

        public static int LastRegionID
        {
            get { return GetMaxID("Region"); }
        }

        public static int LastIntegerValueID
        {
            get { return GetMaxID("SensorValue"); }
        }

        public static int LastFloatValueID
        {
            get { return GetMaxID("SensorValue"); }
        }

        public static int LastStringValueID
        {
            get { return GetMaxID("SensorValue"); }
        }

        private static int GetMaxID(string strTableName)
        {
            if (m_connection == null)
                return -1;

            try
            {
                string strSQL = "Select max(ID) from " + strTableName;
                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int nLastImageID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    reader.Close();
                    return nLastImageID;
                }

                reader.Close();
                return 0;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return -1;
        }

        public static void AddSensor(Sensor sensor)
        {
            if (m_connection == null || sensor.ID < 0)
                return;

            try
            {
                string strSQL = "Insert into Sensor (ID, SensorName, SensorType, DataType, Coverage, Latitude, Longitude, Description)";
                strSQL += string.Format(" values ({0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7})",
                    sensor.ID,
                    sensor.Name == null || sensor.Name.Length == 0 ? "NULL" : "'" + sensor.Name + "'",
                    sensor.SensorType,
                    (int)sensor.SensorDataType,
                    sensor.Coverage < 0 ? "NULL" : sensor.Coverage.ToString(),
                    sensor.IsValidLatitude() ? sensor.Latitude.ToString() : "NULL",
                    sensor.IsValidLongitude() ? sensor.Longitude.ToString() : "NULL",
                    sensor.Description.Length == 0 ? "NULL" : "'" + sensor.Description + "'");

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                cmd.ExecuteNonQuery();

                m_dicSensors[sensor.ID] = sensor;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        public static void AddSensorValue(Sensor sensor, SensorValue value)
        {
            List<SensorValue> values;

            if (!m_dicSensorVaues.TryGetValue(sensor, out values))
            {
                values = new List<SensorValue>();
                m_dicSensorVaues[sensor] = values;
            }

            if (m_connection == null || sensor.ID < 0)
                return;

            string strInt = "NULL", strFloat = "NULL", strString = "NULL";

            if (sensor.SensorDataType == Sensor.DataType.INTEGER)
                strInt = value.GetValueString();
            else if (sensor.SensorDataType == Sensor.DataType.FLOAT)
                strFloat = value.GetValueString();
            else if (sensor.SensorDataType == Sensor.DataType.STRING)
                strString = "'" + value.GetValueString() + "'";
            else
                return;

            try
            {
                string strSQL = "Insert into SensorValue (ID, SensorID, ValueInt, ValueFloat, ValueString, Time, Latitude, Longitude, Description)";
                strSQL += string.Format(" values ({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7}, {8})",
                    value.ID,
                    sensor.ID,
                    strInt,
                    strFloat,
                    strString,
                    string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", value.Time.Year, value.Time.Month, value.Time.Day, value.Time.Hour, value.Time.Minute, value.Time.Second),
                    value.IsValidLatitude() ? value.Latitude.ToString() : "NULL",
                    value.IsValidLongitude() ? value.Longitude.ToString() : "NULL",
                    value.Description.Length == 0 ? "NULL" : "'" + value.Description + "'");

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                cmd.ExecuteNonQuery();

                values.Add(value);
                m_dicSensorValue2[value.ID] = value;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        public static void AddRegion(Region region)
        {
            if (m_connection == null || region.ID < 0)
                return;

            try
            {
                string strSQL = "Insert into Region (ID, RegionName, Boundary, Description)";
                strSQL += string.Format(" values ({0}, '{1}', '{2}', {3})",
                    region.ID,
                    region.Name,
                    region.Boundary,
                    region.Description.Length == 0 ? "NULL" : "'" + region.Description + "'");

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                cmd.ExecuteNonQuery();

                m_dicRegions[region.ID] = region;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        public static void RemoveSensor(int nID)
        {
            if (m_connection == null || nID < 0)
                return;

            Sensor sensor;

            if (!m_dicSensors.TryGetValue(nID, out sensor))
                return;

            try
            {
                string strSQL = "Delete from SensorValue where SensorID = " + sensor.ID;
                
                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                cmd.ExecuteNonQuery();

                List<SensorValue> values;
                
                if (m_dicSensorVaues.TryGetValue(sensor, out values))
                {
                    foreach (SensorValue value in values)
                    {
                        m_dicSensorValue2.Remove(value.ID);
                    }
                }

                m_dicSensorVaues.Remove(sensor);

                strSQL = "Delete from Sensor where ID = " + sensor.ID;

                cmd = new SqlCommand(strSQL, m_connection);
                cmd.ExecuteNonQuery();

                m_dicSensors.Remove(sensor.ID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        public static void RemoveRegion(int nID)
        {
            if (m_connection == null || nID < 0)
                return;

            Region region;

            if (!m_dicRegions.TryGetValue(nID, out region))
                return;

            try
            {
                string strSQL = "Delete from Region where ID = " + nID;

                SqlCommand cmd = new SqlCommand(strSQL, m_connection);
                cmd.ExecuteNonQuery();

                m_dicRegions.Remove(nID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        public static List<SensorValue> GetSensorValues(Sensor sensor)
        {
            List<SensorValue> values;

            if (m_dicSensorVaues.TryGetValue(sensor, out values))
                return values;

            return null;
        }

        public static SensorValue GetSensorValue(int nSensorValueID)
        {
            SensorValue value;

            if (m_dicSensorValue2.TryGetValue(nSensorValueID, out value))
                return value;

            return null;
        }

        public static List<SensorData2> GetSensorDataList_region_time(int nRegionID, DateTime dtBegin, DateTime dtEnd)
        {
            Region region;

            if (!m_dicRegions.TryGetValue(nRegionID, out region))
                return null;

            List<SensorData2> values = new List<SensorData2>();
            float x = 0.0f, y = 0.0f;

            foreach (KeyValuePair<Sensor, List<SensorValue>> pair in m_dicSensorVaues)
            {
                foreach (SensorValue value in pair.Value)
                {
                    if (value.IsValidLatitude() && value.IsValidLongitude())
                    {
                        y = value.Latitude;
                        x = value.Longitude;
                    }
                    else if (value.Sensor != null && value.Sensor.IsValidLatitude() && value.Sensor.IsValidLongitude())
                    {
                        y = value.Sensor.Latitude;
                        x = value.Sensor.Longitude;
                    }
                    else
                        continue;

                    if (region.IsInclude(x, y, value.Sensor == null ? 0 : value.Sensor.Coverage))
                    {
                        if (value.Time >= dtBegin && value.Time <= dtEnd)
                        {
                            values.Add(new SensorData2(value.ID, value.Sensor == null ? 0 : value.Sensor.ID, value.Time, value.GetValueString()));
                        }
                    }
                }
            }

            return values;
        }

        public static List<SensorData2> GetSensorDataList_rect_time(float fTLx, float fTLy, float fBLx, float fBLy, float fBRx, float fBRy, DateTime dtBegin, DateTime dtEnd)
        {
            List<SensorData2> values = new List<SensorData2>();
            float x = 0.0f, y = 0.0f;

            foreach (KeyValuePair<Sensor, List<SensorValue>> pair in m_dicSensorVaues)
            {
                foreach (SensorValue value in pair.Value)
                {
                    if (value.IsValidLatitude() && value.IsValidLongitude())
                    {
                        y = value.Latitude;
                        x = value.Longitude;
                    }
                    else if (value.Sensor != null && value.Sensor.IsValidLatitude() && value.Sensor.IsValidLongitude())
                    {
                        y = value.Sensor.Latitude;
                        x = value.Sensor.Longitude;
                    }
                    else
                        continue;

                    if (IsInclude(fTLx, fTLy, fBLx, fBLy, fBRx, fBRy, x, y, value.Sensor == null ? 0 : value.Sensor.Coverage))
                    {
                        if (value.Time >= dtBegin && value.Time <= dtEnd)
                        {
                            values.Add(new SensorData2(value.ID, value.Sensor == null ? 0 : value.Sensor.ID, value.Time, value.GetValueString()));
                        }
                    }
                }
            }

            return values;
        }

        public static Sensor GetSensor(int nID)
        {
            Sensor sensor;

            if (m_dicSensors.TryGetValue(nID, out sensor))
                return sensor;

            return null;
        }

        public static Region GetRegion(int nID)
        {
            Region region;

            if (m_dicRegions.TryGetValue(nID, out region))
                return region;

            return null;
        }

        private static bool IsInclude(float fTLx, float fTLy, float fBLx, float fBLy, float fBRx, float fBRy, float x, float y, int nCoverage)
        {
            if (nCoverage < 0)
                nCoverage = 0;

            if (x >= fTLx - nCoverage && x <= fBRx + nCoverage &&
                y >= fBRy - nCoverage && y <= fTLy + nCoverage)
                return true;

            return false;
        }

        public static bool ConnectDB(string strDbPath)
        {
            if (m_connection != null)
                return true;

            try
            {
                string strConnection = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=" + strDbPath + ";Integrated Security=True;Connect Timeout=30";
                m_connection = new SqlConnection(strConnection);
                m_connection.Open();

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                m_connection = null;
            }

            return false;
        }

        public static void CloseDB()
        {
            if (m_connection != null)
            {
                m_connection.Close();
                m_connection = null;
            }
        }
    }
}
