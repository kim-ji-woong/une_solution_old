using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace HSMS
{
    class EditAlarmDistance : ChangedData
    {
        public const int WorkerToCarDistanceBoth = 1;
        public const int WorkerToCarDistanceOneSide = 2;
        public const int WorkerToZoneDistance = 3;
        public const int WorkerToEquipDistance = 4;

        public EditAlarmDistance()
        {            
        }

        private int m_nSqlType = 0;
        public new int SQLType
        {
            get { return m_nSqlType; }
            set { m_nSqlType = value; }
        }

        private float m_fItemValue;
        public float ItemValue
        {
            get { return m_fItemValue; }
            set { m_fItemValue = value; }
        }

        public override bool Update(DBConn conn)
        {
            /*int nSiteID = FormMain.Instance.SiteID;
            DataManager dataMgr = FormMain.Instance.DataMgr;


            SqlConnection connection = conn.Connect();
            string SQLUpdateQuery = "";
            if (m_nSqlType == EditAlarmDistance.WorkerToCarDistanceOneSide)
            {
                float itemValue = m_fItemValue; 
                dataMgr.WorkerToCarDistanceOneSide = itemValue;

                itemValue *= 1000.0f;

                SQLUpdateQuery = "Update Options Set ItemValue = " + itemValue.ToString() + " where ItemName = 'WorkerToCarDistanceOneSide' And SiteID = " + nSiteID;
                conn.ExecuteSQL(SQLUpdateQuery, connection);
            }
            else if (m_nSqlType == EditAlarmDistance.WorkerToZoneDistance)
            {
                float itemValue = m_fItemValue;
                dataMgr.WorkerToZoneDistance = itemValue;

                itemValue *= 1000.0f;

                SQLUpdateQuery = "Update Options Set ItemValue = " + itemValue.ToString() + " where ItemName = 'WorkerToZoneDistance' And SiteID = " + nSiteID;
                conn.ExecuteSQL(SQLUpdateQuery, connection);
            }
            else if (m_nSqlType == EditAlarmDistance.WorkerToEquipDistance)
            {
                float itemValue = m_fItemValue;
                dataMgr.WorkerToEquipDistance = itemValue;

                itemValue *= 1000.0f;

                SQLUpdateQuery = "Update Options Set ItemValue = " + itemValue.ToString() + " where ItemName = 'WorkerToEquipDistance' And SiteID = " + nSiteID;
                conn.ExecuteSQL(SQLUpdateQuery, connection);
            }
            else if (m_nSqlType == EditAlarmDistance.WorkerToCarDistanceBoth)
            {
                float itemValue = m_fItemValue;
                dataMgr.WorkerToCarDistanceBoth = itemValue;

                itemValue *= 1000.0f;

                SQLUpdateQuery = "Update Options Set ItemValue = " + itemValue.ToString() + " where ItemName = 'WorkerToCarDistanceBoth' And SiteID = " + nSiteID;
                conn.ExecuteSQL(SQLUpdateQuery, connection);
            }
            else
            {
                connection.Close();
                return false;
            }

            connection.Close();*/

            return true;
        }
        public override void AddToManager(IChangedDataManager mgr)
        {
            throw new NotImplementedException();
        }
    }
}
