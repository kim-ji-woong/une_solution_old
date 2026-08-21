using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.Sensor
{
    /// <summary>
    /// SdmsSensorServerInfo
    /// </summary>
    public class ServerInfo : IIDObject
    {
        public enum Fields { ID, Place, IP };

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_strPlace = "";
        public string Place
        {
            get { return m_strPlace; }
            set { m_strPlace = value; }
        }

        private string m_strIP = "";
        public string IP
        {
            get { return m_strIP; }
            set { m_strIP = value; }
        }

        public static string TableName
        {
            get { return "SdmsSensorServerInfo"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }
    }
}
