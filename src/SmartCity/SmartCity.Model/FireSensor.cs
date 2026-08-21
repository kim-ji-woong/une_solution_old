using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class FireSensor
    {
        public enum Fields { ID, SensorID, State, Addr, OccurTime, CloseTime, IsAfterFire, AlarmPeriodStart, AlarmPeriodEnd, WeakStart, WeakEnd, IsInitReact, Demander, DeathToll, Message, IsUserModifity };

        private int m_nID = -1;
        private string m_strSensorID = "";
        private string m_strState = "";
        private string m_strAddr = "";
        private DateTime? m_dtOccurTime = null;
        private DateTime? m_dtCloseTime = null;
        private int m_nIsAfterFire = 0;
        private DateTime? m_dtAlarmPeriodStart = null;
        private DateTime? m_dtAlarmPeriodEnd = null;
        private DateTime? m_dtWeakStart = null;
        private DateTime? m_dtWeakEnd = null;
        private int m_nIsInitReact = 0;
        private int m_nDemander = 0;
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

        public DateTime? CloseTime
        {
            get { return m_dtCloseTime; }
            set { m_dtCloseTime = value; }
        }

        public int IsAfterFire
        {
            get { return m_nIsAfterFire; }
            set { m_nIsAfterFire = value; }
        }

        public DateTime? AlarmPeriodStart
        {
            get { return m_dtAlarmPeriodStart; }
            set { m_dtAlarmPeriodStart = value; }
        }

        public DateTime? AlarmPeriodEnd
        {
            get { return m_dtAlarmPeriodEnd; }
            set { m_dtAlarmPeriodEnd = value; }
        }

        public DateTime? WeakStart
        {
            get { return m_dtWeakStart; }
            set { m_dtWeakStart = value; }
        }

        public DateTime? WeakEnd
        {
            get { return m_dtWeakEnd; }
            set { m_dtWeakEnd = value; }
        }

        public int IsInitReact
        {
            get { return m_nIsInitReact; }
            set { m_nIsInitReact = value; }
        }

        public int Demander
        {
            get { return m_nDemander; }
            set { m_nDemander = value; }
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
                field == Fields.CloseTime ||
                field == Fields.AlarmPeriodStart ||
                field == Fields.AlarmPeriodEnd ||
                field == Fields.WeakStart ||
                field == Fields.WeakEnd ||
                field == Fields.Message)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "FireSensor"; }
        }
    }
}
