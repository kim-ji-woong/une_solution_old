using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH_Temp_Humidity_Server.Data
{
    public class Sensor
    {
        private int m_nID = -1;
        private string m_strSensorName = "";
        // m_strSensorName에 빈칸을 없앤 버전
        private string m_strSensorName2 = "";
        private string m_strNickName = null;
        // m_strNickName 빈칸을 없앤 버전
        private string m_strNickName2 = null;
        private int m_nFloorIndex = 0;
        private string m_strMeshName = "";
        private int m_nSensorTagInfoID = -1;
        private int m_nSensorZoneID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { SetSensorName(value); }
        }

        public string ShortSensorName
        {
            get { return m_strSensorName2; }
        }

        public string NickName
        {
            get { return m_strNickName; }
            set { SetNickName(value); }
        }

        public string ShortNickName
        {
            get { return m_strNickName2; }
        }

        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public string MeshName
        {
            get { return m_strMeshName; }
            set { m_strMeshName = value; }
        }

        public int SensorTagInfoID
        {
            get { return m_nSensorTagInfoID; }
            set { m_nSensorTagInfoID = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        private void SetSensorName(string strSensorName)
        {
            m_strSensorName = strSensorName;

            if (m_strSensorName != null)
            {
                m_strSensorName2 = RemoveEmpty(m_strSensorName);
            }
        }

        private void SetNickName(string strNickName)
        {
            m_strNickName = strNickName;

            if (m_strNickName != null)
            {
                m_strNickName2 = RemoveEmpty(m_strNickName);
            }
        }

        private string RemoveEmpty(string str)
        {
            string[] tokens = str.Split(new char[] { ' ', '\t' });
            string strResult = "";

            foreach (string strToken in tokens)
            {
                strResult += strToken;
            }

            return strResult;
        }
    }

    public class AlarmType : IComparable
    {
        private int m_nID = -1;
        private string m_strTypeName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string TypeName
        {
            get { return m_strTypeName; }
            set { m_strTypeName = value; }
        }

        // 문자열 길이의 역순으로 정렬한다.
        public int CompareTo(object obj)
        {
            AlarmType alarmType = (AlarmType)obj;

            if (obj == null)
                return -1;

            int len1 = m_strTypeName.Length;
            int len2 = alarmType.m_strTypeName.Length;

            return -len1.CompareTo(len2);
        }
    }

    public class AlarmData
    {
        private int m_nID = -1;
        private string m_strAlarmCode = "";
        private string m_strMessage = "";
        private AlarmType m_alarmType = null;
        private Sensor m_sensor = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string AlarmCode
        {
            get { return m_strAlarmCode; }
            set { m_strAlarmCode = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public AlarmType AlarmType
        {
            get { return m_alarmType; }
            set { m_alarmType = value; }
        }

        public Sensor Sensor
        {
            get { return m_sensor; }
            set { m_sensor = value; }
        }
    }
}
