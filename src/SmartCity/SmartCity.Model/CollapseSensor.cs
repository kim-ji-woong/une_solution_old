using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class CollapseSensor
    {
        public enum Fields { ID, SensorID, State, Addr, MeasureTime, Message, IsUserModifity };

        private int m_nID = -1;
        private string m_strSensorID = "";
        private string m_strState = "";
        private string m_strAddr = "";
        private DateTime? m_dtMeasureTime = null;
        private string m_strMessage = "";
        private int m_nIsUserModifity = 0;


        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorID
        {
            get { return m_strSensorID; }
            set { m_strSensorID = value; }
        }

        public string State
        {
            get { return m_strState; }
            set { m_strState = value; }
        }

        public string Addr
        {
            get { return m_strAddr; }
            set { m_strAddr = value; }
        }

        public DateTime? MeasureTime
        {
            get { return m_dtMeasureTime; }
            set { m_dtMeasureTime = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public int IsUserModifity
        {
            get { return m_nIsUserModifity; }
            set { m_nIsUserModifity = value; }
        }


        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.MeasureTime ||
                field == Fields.Message)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "CollapseSensor"; }
        }
    }
}
