using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class HeatSensor
    {
        public enum Fields { ID, SensorID, State, Addr, OccurTime, Temperature, Humidity, Direction, Speed, MeasPeriodStart, MeasPeriodEnd, PreliminaryDate, AdvisoryDate, AlertDate, DeathToll, Message, IsUserModifity };

        private int m_nID = -1;
        private string m_strSensorID = "";
        private string m_strState = "";
        private string m_strAddr = "";
        private DateTime? m_dtOccurTime = null;
        private float m_fTemperature = 0;
        private float m_fHumidity = 0;
        private float m_fDirection = 0;
        private float m_fSpeed = 0;
        private DateTime? m_dtMeasPeriodStart = null;
        private DateTime? m_dtMeasPeriodEnd = null;
        private DateTime? m_dtPreliminaryDate = null;
        private DateTime? m_dtAdvisoryDate = null;
        private DateTime? m_dtAlertDate = null;
        private int m_nDeathToll = 0;
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

        public DateTime? OccurTime
        {
            get { return m_dtOccurTime; }
            set { m_dtOccurTime = value; }
        }

        public float Temperature
        {
            get { return m_fTemperature; }
            set { m_fTemperature = value; }
        }

        public float Humidity
        {
            get { return m_fHumidity; }
            set { m_fHumidity = value; }
        }

        public float Direction
        {
            get { return m_fDirection; }
            set { m_fDirection = value; }
        }

        public float Speed
        {
            get { return m_fSpeed; }
            set { m_fSpeed = value; }
        }

        public DateTime? MeasPeriodStart
        {
            get { return m_dtMeasPeriodStart; }
            set { m_dtMeasPeriodStart = value; }
        }

        public DateTime? MeasPeriodEnd
        {
            get { return m_dtMeasPeriodEnd; }
            set { m_dtMeasPeriodEnd = value; }
        }

        public DateTime? PreliminaryDate
        {
            get { return m_dtPreliminaryDate; }
            set { m_dtPreliminaryDate = value; }
        }

        public DateTime? AdvisoryDate
        {
            get { return m_dtAdvisoryDate; }
            set { m_dtAdvisoryDate = value; }
        }

        public DateTime? AlertDate
        {
            get { return m_dtAlertDate; }
            set { m_dtAlertDate = value; }
        }

        public int DeathToll
        {
            get { return m_nDeathToll; }
            set { m_nDeathToll = value; }
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
            if (field == Fields.OccurTime ||
                field == Fields.MeasPeriodStart ||
                field == Fields.MeasPeriodEnd ||
                field == Fields.PreliminaryDate ||
                field == Fields.AdvisoryDate ||
                field == Fields.AlertDate ||
                field == Fields.Message)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "HeatSensor"; }
        }
    }
}
