using System;
using System.Configuration;
using System.Collections;

namespace AlarmWatcher
{
    public class SensorManager
    {
        private int m_nPrevReadID = -1;
        private DBManager m_dbMgr = null;
        private Alarm.AlarmManager m_alarmManager = null;

        public SensorManager(int nPrevReadID = -1)
        {
            m_nPrevReadID = nPrevReadID;

            string strDBUrl = ConfigurationManager.AppSettings.Get("dbUrl");
            string strDBName = ConfigurationManager.AppSettings.Get("dbName");
            string strPW = ConfigurationManager.AppSettings.Get("pw");

            m_dbMgr = new DBManager(strDBUrl, "sa", strPW, strDBName);
            m_alarmManager = new Alarm.AlarmManager(m_dbMgr);

            if (m_nPrevReadID < 0)
                ReadLastReadID();
        }

        private void ReadLastReadID()
        {
            string strSQL = "Select LastReadID from TempData4SensorAlarm";
            ArrayList arrResult = m_dbMgr.RunQuery(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                VariousData<int> id = DBManager.GetIntField(arrResult[0].ToString());

                if (id != null)
                    m_nPrevReadID = id.Data;
            }
        }

        private void UpdatePrevReadID()
        {
            string strSQL = "Update TempData4SensorAlarm set LastReadID = " + m_nPrevReadID.ToString();
            m_dbMgr.RunQuery(strSQL);
        }

        public bool ReadSensorData(int nReadCount = -1)
        {
            string strCondition = GetConditionString();
            string strSQL = "Select ";

            if (nReadCount > 0)
                strSQL += string.Format("TOP ({0}) ", nReadCount);

            strSQL += "id, year, month, day, serno, pm2_5, no2, o3, temp, humidity, regdate from SensorData";

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbMgr.RunQuery(strSQL);

            if (arrResult == null)
            {
                System.Diagnostics.Trace.WriteLine("Query Error : " + strSQL);
                System.Diagnostics.Trace.WriteLine("Error Message : " + m_dbMgr.ErrorMessage);
                return false;
            }

            int nPrevID = m_nPrevReadID;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-10;i+=11)
            {
                VariousData<int> id = DBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> year = DBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> month = DBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> day = DBManager.GetIntField(arrResult[i + 3].ToString());
                string strSerno = DBManager.GetStringField(arrResult[i + 4]);
                VariousData<float> pm2_5 = DBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<float> no2 = DBManager.GetFloatField(arrResult[i + 6].ToString());
                VariousData<float> o3 = DBManager.GetFloatField(arrResult[i + 7].ToString());
                VariousData<float> temp = DBManager.GetFloatField(arrResult[i + 8].ToString());
                VariousData<float> humidity = DBManager.GetFloatField(arrResult[i + 9].ToString());
                VariousData<DateTime> regdate = DBManager.GetDateTimeField(arrResult[i + 10]);

                if (id == null || year == null || month == null || day == null || strSerno == null ||
                    pm2_5 == null || no2 == null || o3 == null || temp == null || humidity == null || regdate == null)
                    continue;

                if (m_nPrevReadID < id.Data)
                    m_nPrevReadID = id.Data;

                m_alarmManager.ProcessSensorData(year.Data, month.Data, day.Data, pm2_5.Data, no2.Data, o3.Data, temp.Data, humidity.Data, regdate.Data);
            }

            if (m_nPrevReadID != nPrevID)
            {
                UpdatePrevReadID();
                //System.Diagnostics.Trace.WriteLine("ReadID : " + m_nPrevReadID);
            }

            return true;
        }

        private string GetConditionString()
        {
            string strCondition = "";

            if (m_nPrevReadID > 0)
            {
                strCondition = "id > " + m_nPrevReadID.ToString();
            }
            else
            {
                // 기존에 읽었던 데이터 정보가 없을 경우
                // 최근 5분전의 데이터부터 5분후의 데이터까지 읽어온다.
                DateTime timeBefore = DateTime.Now.AddMinutes(-5);
                DateTime timeAfter = DateTime.Now.AddMinutes(5);
                strCondition = string.Format("regdate > '{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}' and regdate < '{6}-{7:00}-{8:00} {9:00}:{10:00}:{11:00}'",
                    timeBefore.Year, timeBefore.Month, timeBefore.Day, timeBefore.Hour, timeBefore.Minute, timeBefore.Second,
                    timeAfter.Year, timeAfter.Month, timeAfter.Day, timeAfter.Hour, timeAfter.Minute, timeAfter.Second);
            }

            return strCondition;
        }
    }
}
