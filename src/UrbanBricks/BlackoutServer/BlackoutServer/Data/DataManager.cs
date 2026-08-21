using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackoutServer.Data
{
    public class DataManager
    {
        private static DataManager m_instance = null;
        public static DataManager Instance
        {
            get
            {
                if (m_instance == null)
                    new DataManager();

                return m_instance;
            }
        }

        private static List<Sensor> m_sensors = new List<Sensor>();
        public static List<Sensor> Sensors
        {
            get { return m_sensors; }
            set { m_sensors = value; }
        }

        public DataManager()
        {
            m_instance = this;
        }

        public void DisplaySensor(WebDBManager dbManager)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select bs.ID as ID, sz.ID as SensorZoneID, Name, sti.ID as SensorTagInfoID ");
            sb.Append("  From BlackoutSensor as bs, SensorZone as sz, SensorTagInfo as sti ");
            sb.Append(" Where bs.ID = sz.OrgSensorID ");
            sb.Append("   And sz.ID = sti.SensorZoneID ");
            sb.Append("   And SensorType = 17 ");

            ArrayList arrResult = dbManager.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i+=4)
            {
                int nOrgSensorID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nSensorZoneID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strSensorName = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2]);
                int nSensorTagInfoID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                Sensor sensor = new Sensor();
                sensor.OrgSensorID = nOrgSensorID;
                sensor.SensorZoneID = nSensorZoneID;
                sensor.SensorName = strSensorName;
                sensor.SensorTagInfoID = nSensorTagInfoID;

                m_sensors.Add(sensor);
            }
        }
    }

    public class Sensor
    {
        private int m_nOrgSensorID = -1;
        public int OrgSensorID
        {
            get { return m_nOrgSensorID; }
            set { m_nOrgSensorID = value; }
        }

        private int m_nSensorZoneID = -1;
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        private int m_nSensorTagInfoID = -1;
        public int SensorTagInfoID
        {
            get { return m_nSensorTagInfoID; }
            set { m_nSensorTagInfoID = value; }
        }

        private string m_strSensorName = "";
        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        private int m_nData = 0;
        public int Data
        {
            get { return m_nData; }
            set { m_nData = value; }
        }
    }
}
