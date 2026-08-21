using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DigitalTwin
{
    public class Sensor
    {
        private int m_nID = 0;
        private string m_strSensorName = "";
        private string m_strUnit = "";
        private string m_strValue = "";
        private DataGridViewRow m_row = null;
        private float m_fWarning = 0.0f;
        private float m_fMinor = 0.0f;
        private float m_fMajor = 0.0f;
        private float m_fCritical = 0.0f;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public string Unit
        {
            get { return m_strUnit; }
            set { m_strUnit = value; }
        }

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public float Warning
        {
            get { return m_fWarning; }
            set { m_fWarning = value; }
        }

        public float Minor
        {
            get { return m_fMinor; }
            set { m_fMinor = value; }
        }

        public float Major
        {
            get { return m_fMajor; }
            set { m_fMajor = value; }
        }

        public float Critical
        {
            get { return m_fCritical; }
            set { m_fCritical = value; }
        }

        public DataGridViewRow DataRow
        {
            get { return m_row; }
            set { m_row = value; }
        }

        public string GetStatus()
        {
            if (m_strValue.Length == 0)
                return "";

            float fValue;

            if (float.TryParse(m_strValue, out fValue) == false)
                return "";

            if (fValue >= m_fCritical)
                return "Critical";
            else if (fValue >= m_fMajor)
                return "Major";
            else if (fValue >= m_fMinor)
                return "Minor";
            else if (fValue >= m_fWarning)
                return "Warning";

            return "";

        }
    }
}