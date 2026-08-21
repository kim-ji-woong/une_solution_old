using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class FloodSensor
    {
        public enum Fields { ID, SensorID, State, Addr, MeasureTime, Depth, Flow, Message, IsUserModifity };

        private int m_nID = -1;
        private string m_strSensorID = "";
        private string m_strState = "";
        private string m_strAddr = "";
        private DateTime? m_dtMeasureTime = null;
        private float m_fDepth = 0;
        private float m_fFlow = 0;
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

        public float Depth
        {
            get { return m_fDepth; }
            set { m_fDepth = value; }
        }

        public float Flow
        {
            get { return m_fFlow; }
            set { m_fFlow = value; }
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
            get { return "FloodSensor"; }
        }
    }
}
