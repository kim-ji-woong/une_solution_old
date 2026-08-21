using System;
using dnsDBUtil;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace WifiSensorService.Data
{
    using Request;
    using Response;

    public class SensorManager
    {
        //private static int m_nYear = 0, m_nMonth = 0, m_nDay = 0;

        // nLifeTime : 데이터 보존기한(개월)
        public static MessageResult InsertSensorData(SensorData data, WebDBManager dbMgr, int nLifeTime, int nRebootMinutes, int nWarmingupMinutes)
        {
            string strErrorMessage;

            if (data.IsValid(out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            int year, month, day;

            if (ReadDateTime(data.Regdate, out year, out month, out day, out strErrorMessage) == false)
            {
                return new MessageResult(false, strErrorMessage);
            }

            DateTime dtNow = DateTime.Now;
            DateTime dtYesterday = dtNow.AddDays(-1);
            DateTime dtTomorrow = dtNow.AddDays(1);

            int yesterday = dtYesterday.Year * 10000 + dtYesterday.Month * 100 + dtYesterday.Day;
            int tomorrow = dtTomorrow.Year * 10000 + dtTomorrow.Month * 100 + dtTomorrow.Day;
            int sensorDay = year * 10000 + month * 100 + day;

            if (sensorDay < yesterday || sensorDay > tomorrow)
                return new MessageResult(false, string.Format("잘못된 데이터 날짜입니다.\r\nregdate : " + data.Regdate.ToString()));

            // Warmingup이 끝난 센서인가?
            // 센서가 켜지고 Warmingup 시간이 지나지 않은 값은 무시한다.(안정화 필요)
            if (CheckSensorWarmingup(data, dbMgr, nRebootMinutes, nWarmingupMinutes) == false)
                return new MessageResult(true, "");

            // 새로운 데이터를 삽입한다.
            string strFormat = "Insert into SensorData (year, month, day, serno, pm2_5, no2, o3, temp, humidity, regdate, lat, lon) values (";
            strFormat += "{0}, {1}, {2}, '{3}', {4}, {5}, {6}, {7}, {8}, '{9}', {10}, {11})";

            string strSQL = string.Format(strFormat,
                year, month, day,
                data.Serno,
                data.Pm2_5,
                data.No2,
                data.O3,
                data.Temp,
                data.Humi,
                data.Regdate,
                data.Lat,
                data.Lon);

            if (dbMgr.GetResultData(strSQL) == null)
            {
                return new MessageResult(false, dbMgr.LastErrorMessage);
            }

            AddStatistics(year, month, day, data, dbMgr);

            return new MessageResult(true, "");
        }

        private static bool CheckSensorWarmingup(SensorData data, WebDBManager dbMgr, int nRebootMinutes, int nWarmingupMinutes)
        {
            string strTableName = "SensorStatus";
            string strSQL = "Select id, bootTime, lastReadTime from " + strTableName + " where serno = '" + data.Serno + "'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            if (nResultCount < 3)
            {
                string strFormat = "Insert into {0} (id, serno, bootTime, lastReadTime) values (";
                strFormat += "IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, '{1}', '{2}', '{2}')";

                strSQL = string.Format(strFormat, strTableName, data.Serno, data.Regdate);
                dbMgr.GetResultData(strSQL);

                // Query의 성공여부와 상관없이 센서가 이제 막 켜졌기 때문에 워밍업이 끝나지 않았다.
                return false;
            }

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<DateTime> bootTime = WebDBManager.GetDateTimeField(arrResult[1]);
            VariousData<DateTime> lastReadTime = WebDBManager.GetDateTimeField(arrResult[2]);

            if (id == null || bootTime == null || lastReadTime == null)
                return false;

            DateTime dtSensor = GetSensorTime(data);

            TimeSpan spanLast = dtSensor - lastReadTime.Data;

            if (spanLast.TotalMinutes >= nRebootMinutes)
            {
                // 센서가 새로 켜졌다.
                strSQL = string.Format("Update {0} set bootTime = '{1}', lastReadTime = '{1}' where ID = {2}", strTableName, data.Regdate, id.Data);
                dbMgr.GetResultData(strSQL);

                // Query의 성공여부와 상관없이 센서가 이제 막 켜졌기 때문에 워밍업이 끝나지 않았다.
                return false;
            }

            TimeSpan spanBoot = dtSensor - bootTime.Data;

            if (spanBoot.TotalMinutes < nWarmingupMinutes)
            {
                // 아직 워밍업이 끝나지 않았다.
                strSQL = string.Format("Update {0} set lastReadTime = '{1}' where ID = {2}", strTableName, data.Regdate, id.Data);
                dbMgr.GetResultData(strSQL);

                // Query의 성공여부와 상관없이 워밍업이 끝나지 않았다.
                return false;
            }
            else
            {
                strSQL = string.Format("Update {0} set lastReadTime = '{1}' where ID = {2}", strTableName, data.Regdate, id.Data);

                if (dbMgr.GetResultData(strSQL) == null)
                    return false;
            }

            return true;
        }

        private static DateTime GetSensorTime(SensorData data)
        {
            try
            {
                DateTime time = Convert.ToDateTime(data.Regdate);
                return time;
            }
            catch (Exception)
            {
            }

            // 센서시간이 잘못되어 있으면 현재 시간을 사용한다.
            return DateTime.Now;
        }

        private static bool AddStatistics(int year, int month, int day, SensorData data, WebDBManager dbMgr)
        {
            if (AddStatisticsYear(year, data, dbMgr) == false)
                return false;

            if (AddStatisticsMonth(year, month, data, dbMgr) == false)
                return false;

            if (AddStatisticsWeek(year, month, day, data, dbMgr) == false)
                return false;

            if (AddStatisticsDay(year, month, day, data, dbMgr) == false)
                return false;

            return true;
        }

        private static bool AddStatisticsDay(int year, int month, int day, SensorData data, WebDBManager dbMgr)
        {
            string strTableName = "SensorStatisticsDay";
            string strSQL = string.Format("Select id, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount, pm2_5Max, pm2_5Min, no2Max, no2Min, o3Max, o3Min, tempMax, tempMin, humidityMax, humidityMin from {0} where year = {1} and month = {2} and day = {3} and serno = '{4}'", strTableName, year, month, day, data.Serno);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                System.Diagnostics.Trace.WriteLine("SensorStatisticsDay Fail : " + dbMgr.LastErrorMessage);
                return false;
            }

            if (arrResult.Count == 0)
            {
                string strFormat = "Insert into {0} (id, year, month, day, serno, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount, pm2_5Max, pm2_5Min, no2Max, no2Min, o3Max, o3Min, tempMax, tempMin, humidityMax, humidityMin) values (";
                strFormat += "IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {1}, {2}, {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, {5}, {5}, {6}, {6}, {7}, {7}, {8}, {8}, {9}, {9})";
                strSQL = string.Format(strFormat, strTableName, year, month, day, data.Serno, data.Pm2_5, data.No2, data.O3, data.Temp, data.Humi, 1);
            }
            else
            {
                if (arrResult.Count < 17)
                    return false;

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<float> pm2_5 = WebDBManager.GetFloatField(arrResult[1].ToString());
                VariousData<float> no2 = WebDBManager.GetFloatField(arrResult[2].ToString());
                VariousData<float> o3 = WebDBManager.GetFloatField(arrResult[3].ToString());
                VariousData<float> temp = WebDBManager.GetFloatField(arrResult[4].ToString());
                VariousData<float> humidity = WebDBManager.GetFloatField(arrResult[5].ToString());
                VariousData<int> dataCount = WebDBManager.GetIntField(arrResult[6].ToString());
                VariousData<float> pm2_5Max = WebDBManager.GetFloatField(arrResult[7].ToString());
                VariousData<float> pm2_5Min = WebDBManager.GetFloatField(arrResult[8].ToString());
                VariousData<float> no2Max = WebDBManager.GetFloatField(arrResult[9].ToString());
                VariousData<float> no2Min = WebDBManager.GetFloatField(arrResult[10].ToString());
                VariousData<float> o3Max = WebDBManager.GetFloatField(arrResult[11].ToString());
                VariousData<float> o3Min = WebDBManager.GetFloatField(arrResult[12].ToString());
                VariousData<float> tempMax = WebDBManager.GetFloatField(arrResult[13].ToString());
                VariousData<float> tempMin = WebDBManager.GetFloatField(arrResult[14].ToString());
                VariousData<float> humidityMax = WebDBManager.GetFloatField(arrResult[15].ToString());
                VariousData<float> humidityMin = WebDBManager.GetFloatField(arrResult[16].ToString());

                if (id == null || pm2_5 == null || no2 == null ||
                    o3 == null || temp == null || humidity == null || dataCount == null ||
                    pm2_5Max == null || pm2_5Min == null || no2Max == null || no2Min == null ||
                    o3Max == null || o3Min == null || tempMax == null ||
                    tempMin == null || humidityMax == null || humidityMin == null)
                    return false;

                float fPm2_5Max = pm2_5Max.Data > data.Pm2_5 ? pm2_5Max.Data : data.Pm2_5;
                float fPm2_5Min = pm2_5Min.Data < data.Pm2_5 ? pm2_5Min.Data : data.Pm2_5;
                float fNo2Max = no2Max.Data > data.No2 ? no2Max.Data : data.No2;
                float fNo2Min = no2Min.Data < data.No2 ? no2Min.Data : data.No2;
                float fO3Max = o3Max.Data > data.O3 ? o3Max.Data : data.O3;
                float fO3Min = o3Min.Data < data.O3 ? o3Min.Data : data.O3;
                float fTempMax = tempMax.Data > data.Temp ? tempMax.Data : data.Temp;
                float fTempMin = tempMin.Data < data.Temp ? tempMin.Data : data.Temp;
                float fHumidityMax = humidityMax.Data > data.Humi ? humidityMax.Data : data.Humi;
                float fHumidityMin = humidityMin.Data < data.Humi ? humidityMin.Data : data.Humi;

                strSQL = string.Format("Update {0} set pm2_5Total = {1}, no2Total = {2}, o3Total = {3}, tempTotal = {4}, humidityTotal = {5}, dataCount = {6}, pm2_5Max = {8}, pm2_5Min = {9}, no2Max = {10}, no2Min = {11}, o3Max = {12}, o3Min = {13}, tempMax = {14}, tempMin = {15}, humidityMax = {16}, humidityMin = {17} where ID = {7}",
                    strTableName,
                    pm2_5.Data + data.Pm2_5,
                    no2.Data + data.No2,
                    o3.Data + data.O3,
                    temp.Data + data.Temp,
                    humidity.Data + data.Humi,
                    dataCount.Data + 1,
                    id.Data,
                    fPm2_5Max,
                    fPm2_5Min,
                    fNo2Max,
                    fNo2Min,
                    fO3Max,
                    fO3Min,
                    fTempMax,
                    fTempMin,
                    fHumidityMax,
                    fHumidityMin);
            }

            return dbMgr.GetResultData(strSQL) != null;
        }

        private static int GetMonthWeek(int year, int month, int day)
        {
            DateTime dtDay = new DateTime(year, month, 1);

            int lastDay;

            if (dtDay.DayOfWeek == DayOfWeek.Sunday)
                lastDay = 7;
            else if (dtDay.DayOfWeek == DayOfWeek.Monday)
                lastDay = 6;
            else if (dtDay.DayOfWeek == DayOfWeek.Tuesday)
                lastDay = 5;
            else if (dtDay.DayOfWeek == DayOfWeek.Wednesday)
                lastDay = 4;
            else if (dtDay.DayOfWeek == DayOfWeek.Thursday)
                lastDay = 3;
            else if (dtDay.DayOfWeek == DayOfWeek.Friday)
                lastDay = 2;
            else//if (dtDay.DayOfWeek == DayOfWeek.Saturday)
                lastDay = 1;

            if (day <= lastDay)
                return 1;

            for (int i=2;i<=5;i++)
            {
                lastDay += 7;

                if (day <= lastDay)
                    return i;
            }

            return 6;
        }

        private static bool AddStatisticsWeek(int year, int month, int day, SensorData data, WebDBManager dbMgr)
        {
            string strTableName = "SensorStatisticsWeek";
            int week = GetMonthWeek(year, month, day);

            string strSQL = string.Format("Select id, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount, pm2_5Max, pm2_5Min, no2Max, no2Min, o3Max, o3Min, tempMax, tempMin, humidityMax, humidityMin from {0} where year = {1} and month = {2} and week = {3} and serno = '{4}'", strTableName, year, month, week, data.Serno);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                System.Diagnostics.Trace.WriteLine("SensorStatisticsWeek Fail : " + dbMgr.LastErrorMessage);
                return false;
            }

            if (arrResult.Count == 0)
            {
                string strFormat = "Insert into {0} (id, year, month, week, serno, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount, pm2_5Max, pm2_5Min, no2Max, no2Min, o3Max, o3Min, tempMax, tempMin, humidityMax, humidityMin) values (";
                strFormat += "IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {1}, {2}, {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, {5}, {5}, {6}, {6}, {7}, {7}, {8}, {8}, {9}, {9})";
                strSQL = string.Format(strFormat, strTableName, year, month, week, data.Serno, data.Pm2_5, data.No2, data.O3, data.Temp, data.Humi, 1);
            }
            else
            {
                if (arrResult.Count < 17)
                    return false;

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<float> pm2_5 = WebDBManager.GetFloatField(arrResult[1].ToString());
                VariousData<float> no2 = WebDBManager.GetFloatField(arrResult[2].ToString());
                VariousData<float> o3 = WebDBManager.GetFloatField(arrResult[3].ToString());
                VariousData<float> temp = WebDBManager.GetFloatField(arrResult[4].ToString());
                VariousData<float> humidity = WebDBManager.GetFloatField(arrResult[5].ToString());
                VariousData<int> dataCount = WebDBManager.GetIntField(arrResult[6].ToString());
                VariousData<float> pm2_5Max = WebDBManager.GetFloatField(arrResult[7].ToString());
                VariousData<float> pm2_5Min = WebDBManager.GetFloatField(arrResult[8].ToString());
                VariousData<float> no2Max = WebDBManager.GetFloatField(arrResult[9].ToString());
                VariousData<float> no2Min = WebDBManager.GetFloatField(arrResult[10].ToString());
                VariousData<float> o3Max = WebDBManager.GetFloatField(arrResult[11].ToString());
                VariousData<float> o3Min = WebDBManager.GetFloatField(arrResult[12].ToString());
                VariousData<float> tempMax = WebDBManager.GetFloatField(arrResult[13].ToString());
                VariousData<float> tempMin = WebDBManager.GetFloatField(arrResult[14].ToString());
                VariousData<float> humidityMax = WebDBManager.GetFloatField(arrResult[15].ToString());
                VariousData<float> humidityMin = WebDBManager.GetFloatField(arrResult[16].ToString());

                if (id == null || pm2_5 == null || no2 == null ||
                    o3 == null || temp == null || humidity == null || dataCount == null ||
                    pm2_5Max == null || pm2_5Min == null || no2Max == null || no2Min == null ||
                    o3Max == null || o3Min == null || tempMax == null ||
                    tempMin == null || humidityMax == null || humidityMin == null)
                    return false;

                float fPm2_5Max = pm2_5Max.Data > data.Pm2_5 ? pm2_5Max.Data : data.Pm2_5;
                float fPm2_5Min = pm2_5Min.Data < data.Pm2_5 ? pm2_5Min.Data : data.Pm2_5;
                float fNo2Max = no2Max.Data > data.No2 ? no2Max.Data : data.No2;
                float fNo2Min = no2Min.Data < data.No2 ? no2Min.Data : data.No2;
                float fO3Max = o3Max.Data > data.O3 ? o3Max.Data : data.O3;
                float fO3Min = o3Min.Data < data.O3 ? o3Min.Data : data.O3;
                float fTempMax = tempMax.Data > data.Temp ? tempMax.Data : data.Temp;
                float fTempMin = tempMin.Data < data.Temp ? tempMin.Data : data.Temp;
                float fHumidityMax = humidityMax.Data > data.Humi ? humidityMax.Data : data.Humi;
                float fHumidityMin = humidityMin.Data < data.Humi ? humidityMin.Data : data.Humi;

                strSQL = string.Format("Update {0} set pm2_5Total = {1}, no2Total = {2}, o3Total = {3}, tempTotal = {4}, humidityTotal = {5}, dataCount = {6}, pm2_5Max = {8}, pm2_5Min = {9}, no2Max = {10}, no2Min = {11}, o3Max = {12}, o3Min = {13}, tempMax = {14}, tempMin = {15}, humidityMax = {16}, humidityMin = {17} where ID = {7}",
                    strTableName,
                    pm2_5.Data + data.Pm2_5,
                    no2.Data + data.No2,
                    o3.Data + data.O3,
                    temp.Data + data.Temp,
                    humidity.Data + data.Humi,
                    dataCount.Data + 1,
                    id.Data,
                    fPm2_5Max,
                    fPm2_5Min,
                    fNo2Max,
                    fNo2Min,
                    fO3Max,
                    fO3Min,
                    fTempMax,
                    fTempMin,
                    fHumidityMax,
                    fHumidityMin);
            }

            return dbMgr.GetResultData(strSQL) != null;
        }

        private static bool AddStatisticsMonth(int year, int month, SensorData data, WebDBManager dbMgr)
        {
            string strTableName = "SensorStatisticsMonth";
            string strSQL = string.Format("Select id, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount, pm2_5Max, pm2_5Min, no2Max, no2Min, o3Max, o3Min, tempMax, tempMin, humidityMax, humidityMin from {0} where year = {1} and month = {2} and serno = '{3}'", strTableName, year, month, data.Serno);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                System.Diagnostics.Trace.WriteLine("AddStatisticsMonth Fail : " + dbMgr.LastErrorMessage);
                return false;
            }

            if (arrResult.Count == 0)
            {
                string strFormat = "Insert into {0} (id, year, month, serno, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount, pm2_5Max, pm2_5Min, no2Max, no2Min, o3Max, o3Min, tempMax, tempMin, humidityMax, humidityMin) values (";
                strFormat += "IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {1}, {2}, '{3}', {4}, {5}, {6}, {7}, {8}, {9}, {4}, {4}, {5}, {5}, {6}, {6}, {7}, {7}, {8}, {8})";
                strSQL = string.Format(strFormat, strTableName, year, month, data.Serno, data.Pm2_5, data.No2, data.O3, data.Temp, data.Humi, 1);
            }
            else
            {
                if (arrResult.Count < 17)
                    return false;

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<float> pm2_5 = WebDBManager.GetFloatField(arrResult[1].ToString());
                VariousData<float> no2 = WebDBManager.GetFloatField(arrResult[2].ToString());
                VariousData<float> o3 = WebDBManager.GetFloatField(arrResult[3].ToString());
                VariousData<float> temp = WebDBManager.GetFloatField(arrResult[4].ToString());
                VariousData<float> humidity = WebDBManager.GetFloatField(arrResult[5].ToString());
                VariousData<int> dataCount = WebDBManager.GetIntField(arrResult[6].ToString());
                VariousData<float> pm2_5Max = WebDBManager.GetFloatField(arrResult[7].ToString());
                VariousData<float> pm2_5Min = WebDBManager.GetFloatField(arrResult[8].ToString());
                VariousData<float> no2Max = WebDBManager.GetFloatField(arrResult[9].ToString());
                VariousData<float> no2Min = WebDBManager.GetFloatField(arrResult[10].ToString());
                VariousData<float> o3Max = WebDBManager.GetFloatField(arrResult[11].ToString());
                VariousData<float> o3Min = WebDBManager.GetFloatField(arrResult[12].ToString());
                VariousData<float> tempMax = WebDBManager.GetFloatField(arrResult[13].ToString());
                VariousData<float> tempMin = WebDBManager.GetFloatField(arrResult[14].ToString());
                VariousData<float> humidityMax = WebDBManager.GetFloatField(arrResult[15].ToString());
                VariousData<float> humidityMin = WebDBManager.GetFloatField(arrResult[16].ToString());

                if (id == null || pm2_5 == null || no2 == null ||
                    o3 == null || temp == null || humidity == null || dataCount == null ||
                    pm2_5Max == null || pm2_5Min == null || no2Max == null || no2Min == null ||
                    o3Max == null || o3Min == null || tempMax == null ||
                    tempMin == null || humidityMax == null || humidityMin == null)
                    return false;

                float fPm2_5Max = pm2_5Max.Data > data.Pm2_5 ? pm2_5Max.Data : data.Pm2_5;
                float fPm2_5Min = pm2_5Min.Data < data.Pm2_5 ? pm2_5Min.Data : data.Pm2_5;
                float fNo2Max = no2Max.Data > data.No2 ? no2Max.Data : data.No2;
                float fNo2Min = no2Min.Data < data.No2 ? no2Min.Data : data.No2;
                float fO3Max = o3Max.Data > data.O3 ? o3Max.Data : data.O3;
                float fO3Min = o3Min.Data < data.O3 ? o3Min.Data : data.O3;
                float fTempMax = tempMax.Data > data.Temp ? tempMax.Data : data.Temp;
                float fTempMin = tempMin.Data < data.Temp ? tempMin.Data : data.Temp;
                float fHumidityMax = humidityMax.Data > data.Humi ? humidityMax.Data : data.Humi;
                float fHumidityMin = humidityMin.Data < data.Humi ? humidityMin.Data : data.Humi;

                strSQL = string.Format("Update {0} set pm2_5Total = {1}, no2Total = {2}, o3Total = {3}, tempTotal = {4}, humidityTotal = {5}, dataCount = {6}, pm2_5Max = {8}, pm2_5Min = {9}, no2Max = {10}, no2Min = {11}, o3Max = {12}, o3Min = {13}, tempMax = {14}, tempMin = {15}, humidityMax = {16}, humidityMin = {17} where ID = {7}",
                    strTableName,
                    pm2_5.Data + data.Pm2_5,
                    no2.Data + data.No2,
                    o3.Data + data.O3,
                    temp.Data + data.Temp,
                    humidity.Data + data.Humi,
                    dataCount.Data + 1,
                    id.Data,
                    fPm2_5Max,
                    fPm2_5Min,
                    fNo2Max,
                    fNo2Min,
                    fO3Max,
                    fO3Min,
                    fTempMax,
                    fTempMin,
                    fHumidityMax,
                    fHumidityMin);
            }

            return dbMgr.GetResultData(strSQL) != null;
        }

        private static bool AddStatisticsYear(int year, SensorData data, WebDBManager dbMgr)
        {
            string strTableName = "SensorStatisticsYear";
            string strSQL = string.Format("Select id, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount, pm2_5Max, pm2_5Min, no2Max, no2Min, o3Max, o3Min, tempMax, tempMin, humidityMax, humidityMin from {0} where year = {1} and serno = '{2}'", strTableName, year, data.Serno);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                System.Diagnostics.Trace.WriteLine("AddStatisticsYear Fail : " + dbMgr.LastErrorMessage);
                return false;
            }

            if (arrResult.Count == 0)
            {
                string strFormat = "Insert into {0} (id, year, serno, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount, pm2_5Max, pm2_5Min, no2Max, no2Min, o3Max, o3Min, tempMax, tempMin, humidityMax, humidityMin) values (";
                strFormat += "IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {1}, '{2}', {3}, {4}, {5}, {6}, {7}, {8}, {3}, {3}, {4}, {4}, {5}, {5}, {6}, {6}, {7}, {7})";
                strSQL = string.Format(strFormat, strTableName, year, data.Serno, data.Pm2_5, data.No2, data.O3, data.Temp, data.Humi, 1);
            }
            else
            {
                if (arrResult.Count < 17)
                    return false;

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<float> pm2_5 = WebDBManager.GetFloatField(arrResult[1].ToString());
                VariousData<float> no2 = WebDBManager.GetFloatField(arrResult[2].ToString());
                VariousData<float> o3 = WebDBManager.GetFloatField(arrResult[3].ToString());
                VariousData<float> temp = WebDBManager.GetFloatField(arrResult[4].ToString());
                VariousData<float> humidity = WebDBManager.GetFloatField(arrResult[5].ToString());
                VariousData<int> dataCount = WebDBManager.GetIntField(arrResult[6].ToString());
                VariousData<float> pm2_5Max = WebDBManager.GetFloatField(arrResult[7].ToString());
                VariousData<float> pm2_5Min = WebDBManager.GetFloatField(arrResult[8].ToString());
                VariousData<float> no2Max = WebDBManager.GetFloatField(arrResult[9].ToString());
                VariousData<float> no2Min = WebDBManager.GetFloatField(arrResult[10].ToString());
                VariousData<float> o3Max = WebDBManager.GetFloatField(arrResult[11].ToString());
                VariousData<float> o3Min = WebDBManager.GetFloatField(arrResult[12].ToString());
                VariousData<float> tempMax = WebDBManager.GetFloatField(arrResult[13].ToString());
                VariousData<float> tempMin = WebDBManager.GetFloatField(arrResult[14].ToString());
                VariousData<float> humidityMax = WebDBManager.GetFloatField(arrResult[15].ToString());
                VariousData<float> humidityMin = WebDBManager.GetFloatField(arrResult[16].ToString());

                if (id == null || pm2_5 == null || no2 == null ||
                    o3 == null || temp == null || humidity == null || dataCount == null ||
                    pm2_5Max == null || pm2_5Min == null || no2Max == null || no2Min == null ||
                    o3Max == null || o3Min == null || tempMax == null ||
                    tempMin == null || humidityMax == null || humidityMin == null)
                    return false;

                float fPm2_5Max = pm2_5Max.Data > data.Pm2_5 ? pm2_5Max.Data : data.Pm2_5;
                float fPm2_5Min = pm2_5Min.Data < data.Pm2_5 ? pm2_5Min.Data : data.Pm2_5;
                float fNo2Max = no2Max.Data > data.No2 ? no2Max.Data : data.No2;
                float fNo2Min = no2Min.Data < data.No2 ? no2Min.Data : data.No2;
                float fO3Max = o3Max.Data > data.O3 ? o3Max.Data : data.O3;
                float fO3Min = o3Min.Data < data.O3 ? o3Min.Data : data.O3;
                float fTempMax = tempMax.Data > data.Temp ? tempMax.Data : data.Temp;
                float fTempMin = tempMin.Data < data.Temp ? tempMin.Data : data.Temp;
                float fHumidityMax = humidityMax.Data > data.Humi ? humidityMax.Data : data.Humi;
                float fHumidityMin = humidityMin.Data < data.Humi ? humidityMin.Data : data.Humi;

                strSQL = string.Format("Update {0} set pm2_5Total = {1}, no2Total = {2}, o3Total = {3}, tempTotal = {4}, humidityTotal = {5}, dataCount = {6}, pm2_5Max = {8}, pm2_5Min = {9}, no2Max = {10}, no2Min = {11}, o3Max = {12}, o3Min = {13}, tempMax = {14}, tempMin = {15}, humidityMax = {16}, humidityMin = {17} where ID = {7}",
                    strTableName,
                    pm2_5.Data + data.Pm2_5,
                    no2.Data + data.No2,
                    o3.Data + data.O3,
                    temp.Data + data.Temp,
                    humidity.Data + data.Humi,
                    dataCount.Data + 1,
                    id.Data,
                    fPm2_5Max,
                    fPm2_5Min,
                    fNo2Max,
                    fNo2Min,
                    fO3Max,
                    fO3Min,
                    fTempMax,
                    fTempMin,
                    fHumidityMax,
                    fHumidityMin);
            }

            return dbMgr.GetResultData(strSQL) != null;
        }

        private static bool ReadDateTime(string strDate, out int year, out int month, out int day, out string strErrorMessage)
        {
            year = month = day = 0;
            strErrorMessage = null;

            string[] tokens = strDate.Split(' ');

            if (tokens.Length != 2)
            {
                strErrorMessage = string.Format("regdate {0}는 형식('0000-00-00 00:00:00')에 맞지않습니다.", strDate);
                return false;
            }

            string[] dates = tokens[0].Trim().Split('-');

            if (dates.Length != 3)
            {
                strErrorMessage = string.Format("regdate {0}는 형식('0000-00-00 00:00:00')에 맞지않습니다.", strDate);
                return false;
            }

            if (!int.TryParse(dates[0].Trim(), out year) || !int.TryParse(dates[1].Trim(), out month) || !int.TryParse(dates[2].Trim(), out day))
            {
                strErrorMessage = string.Format("regdate {0}는 형식('0000-00-00 00:00:00')에 맞지않습니다.", strDate);
                return false;
            }

            string[] times = tokens[1].Trim().Split(':');

            if (times.Length < 3)
            {
                strErrorMessage = string.Format("regdate {0}는 형식('0000-00-00 00:00:00')에 맞지않습니다.", strDate);
                return false;
            }

            return true;
        }

        public static ResponseSensorAvgData GetSensorAvgData(RequestSensorAvgData request, WebDBManager dbMgr)
        {
            if (request.AvgDate == null)
                return new ResponseSensorAvgData(false, "avgDate가 null입니다.");

            string strDateType = request.AvgDate.ToLower().Trim();

            if (strDateType == "day")
                return GetAllSensorAvgDay(request, dbMgr);
            else if (strDateType == "week")
                return GetAllSensorAvgWeek(request, dbMgr);
            else if (strDateType == "month")
                return GetAllSensorAvgMonth(request, dbMgr);

            return new ResponseSensorAvgData(false, "알수없는 타입입니다. avgDate : " + request.AvgDate);
        }

        private static ResponseSensorAvgData GetAllSensorAvgMonth(RequestSensorAvgData request, WebDBManager dbMgr)
        {
            string strErrorMessage;
            string strCondition = GetAvgMonthCondition(request, out strErrorMessage);

            if (strCondition == null)
                return new ResponseSensorAvgData(false, strErrorMessage);

            if (request.Serno != null)
                strCondition += " and serno = '" + request.Serno + "'";

            string strSensorType = request.SensorType.ToLower().Trim();

            if (strSensorType != "pm2_5" && strSensorType != "no2" && strSensorType != "o3" &&
                strSensorType != "temp" && strSensorType != "humi" && strSensorType != "all")
                return new ResponseSensorAvgData(false, "알수없는 센서타입입니다. sensorType : " + request.SensorType);

            string strSQL = "Select year, month, serno, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount from SensorStatisticsDay where " + strCondition;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return new ResponseSensorAvgData(false, dbMgr.LastErrorMessage);

            int nResultCount = arrResult.Count;

            SensorAvgData sensorData;
            int nDataCount = 0;

            // Key : serno + _ + year * 10000 + month * 100 + day;
            Dictionary<string, SensorAvgData> dicSensorDatas = new Dictionary<string, SensorAvgData>();
            Dictionary<string, int> dicSensorDataCount = new Dictionary<string, int>();

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                VariousData<int> year = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> month = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strSerno = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<float> pm2_5 = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                VariousData<float> no2 = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> o3 = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<float> temp = WebDBManager.GetFloatField(arrResult[i + 6].ToString());
                VariousData<float> humidity = WebDBManager.GetFloatField(arrResult[i + 7].ToString());
                VariousData<int> dataCount = WebDBManager.GetIntField(arrResult[i + 8].ToString());

                if (year == null || month == null || pm2_5 == null ||
                    no2 == null || o3 == null || temp == null ||
                    humidity == null || dataCount == null || strSerno == null)
                    continue;

                int date = year.Data * 10000 + month.Data * 100;
                string strDate = request.Serno == null ? date.ToString() : request.Serno + "_" + date.ToString();

                if (dicSensorDatas.TryGetValue(strDate, out sensorData) == false)
                {
                    sensorData = new SensorAvgData();
                    sensorData.Date = string.Format("{0}-{1:00}", year.Data, month.Data);
                    dicSensorDatas[strDate] = sensorData;

                    if (request.Serno != null)
                        sensorData.Serno = strSerno;
                }

                if (strSensorType == "all" || strSensorType == "pm2_5")
                {
                    if (sensorData.Pm2_5 == null)
                        sensorData.Pm2_5 = pm2_5.Data;
                    else
                        sensorData.Pm2_5 += pm2_5.Data;
                }

                if (strSensorType == "all" || strSensorType == "no2")
                {
                    if (sensorData.No2 == null)
                        sensorData.No2 = no2.Data;
                    else
                        sensorData.No2 += no2.Data;
                }

                if (strSensorType == "all" || strSensorType == "o3")
                {
                    if (sensorData.O3 == null)
                        sensorData.O3 = o3.Data;
                    else
                        sensorData.O3 += o3.Data;
                }

                if (strSensorType == "all" || strSensorType == "temp")
                {
                    if (sensorData.Temp == null)
                        sensorData.Temp = temp.Data;
                    else
                        sensorData.Temp += temp.Data;
                }

                if (strSensorType == "all" || strSensorType == "humi")
                {
                    if (sensorData.Humi == null)
                        sensorData.Humi = humidity.Data;
                    else
                        sensorData.Humi += humidity.Data;
                }

                if (dicSensorDataCount.TryGetValue(strDate, out nDataCount) == false)
                {
                    dicSensorDataCount[strDate] = dataCount.Data;
                }
                else
                    dicSensorDataCount[strDate] = nDataCount + dataCount.Data;
            }

            foreach (KeyValuePair<string, SensorAvgData> pair in dicSensorDatas)
            {
                if (dicSensorDataCount.TryGetValue(pair.Key, out nDataCount) == false)
                    return new ResponseSensorAvgData(false, "알수없는 에러입니다.");

                if (pair.Value.Pm2_5 != null)
                    pair.Value.Pm2_5 /= nDataCount;

                if (pair.Value.No2 != null)
                    pair.Value.No2 /= nDataCount;

                if (pair.Value.O3 != null)
                    pair.Value.O3 /= nDataCount;

                if (pair.Value.Temp != null)
                    pair.Value.Temp /= nDataCount;

                if (pair.Value.Humi != null)
                    pair.Value.Humi /= nDataCount;
            }

            List<SensorAvgData> sensorDatas = new List<SensorAvgData>();
            sensorDatas.AddRange(dicSensorDatas.Values);
            sensorDatas.Sort();

            ResponseSensorAvgData response = new ResponseSensorAvgData(true, "");

            foreach (SensorAvgData data in sensorDatas)
            {
                object avg = data.ToSensorAvgObject();

                if (avg != null)
                    response.SensorAvgDatas.Add(avg);
            }

            return response;
        }

        private static ResponseSensorAvgData GetAllSensorAvgWeek(RequestSensorAvgData request, WebDBManager dbMgr)
        {
            string strErrorMessage;
            string strCondition = GetAvgDayCondition(request, out strErrorMessage);

            if (strCondition == null)
                return new ResponseSensorAvgData(false, strErrorMessage);

            if (request.Serno != null)
                strCondition += " and serno = '" + request.Serno + "'";

            string strSensorType = request.SensorType.ToLower().Trim();

            if (strSensorType != "pm2_5" && strSensorType != "no2" && strSensorType != "o3" &&
                strSensorType != "temp" && strSensorType != "humi" && strSensorType != "all")
                return new ResponseSensorAvgData(false, "알수없는 센서타입입니다. sensorType : " + request.SensorType);

            string strSQL = "Select year, month, day, serno, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount from SensorStatisticsDay where " + strCondition;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return new ResponseSensorAvgData(false, dbMgr.LastErrorMessage);

            int nResultCount = arrResult.Count;

            SensorAvgData sensorData;
            int nDataCount = 0;

            // Key : serno + _ + year * 10000 + month * 100 + day;
            Dictionary<string, SensorAvgData> dicSensorDatas = new Dictionary<string, SensorAvgData>();
            Dictionary<string, int> dicSensorDataCount = new Dictionary<string, int>();

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                VariousData<int> year = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> month = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> day = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strSerno = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<float> pm2_5 = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> no2 = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<float> o3 = WebDBManager.GetFloatField(arrResult[i + 6].ToString());
                VariousData<float> temp = WebDBManager.GetFloatField(arrResult[i + 7].ToString());
                VariousData<float> humidity = WebDBManager.GetFloatField(arrResult[i + 8].ToString());
                VariousData<int> dataCount = WebDBManager.GetIntField(arrResult[i + 9].ToString());

                if (year == null || month == null || day == null ||
                    pm2_5 == null || no2 == null || o3 == null ||
                    temp == null || humidity == null || dataCount == null || strSerno == null)
                    continue;

                int week = GetMonthWeek(year.Data, month.Data, day.Data);
                int date = year.Data * 10000 + month.Data * 100 + week;
                string strDate = request.Serno == null ? date.ToString() : request.Serno + "_" + date.ToString();

                if (dicSensorDatas.TryGetValue(strDate, out sensorData) == false)
                {
                    sensorData = new SensorAvgData();
                    sensorData.Date = string.Format("{0}-{1:00}-{2}", year.Data, month.Data, week);
                    dicSensorDatas[strDate] = sensorData;

                    if (request.Serno != null)
                        sensorData.Serno = strSerno;
                }

                if (strSensorType == "all" || strSensorType == "pm2_5")
                {
                    if (sensorData.Pm2_5 == null)
                        sensorData.Pm2_5 = pm2_5.Data;
                    else
                        sensorData.Pm2_5 += pm2_5.Data;
                }

                if (strSensorType == "all" || strSensorType == "no2")
                {
                    if (sensorData.No2 == null)
                        sensorData.No2 = no2.Data;
                    else
                        sensorData.No2 += no2.Data;
                }

                if (strSensorType == "all" || strSensorType == "o3")
                {
                    if (sensorData.O3 == null)
                        sensorData.O3 = o3.Data;
                    else
                        sensorData.O3 += o3.Data;
                }

                if (strSensorType == "all" || strSensorType == "temp")
                {
                    if (sensorData.Temp == null)
                        sensorData.Temp = temp.Data;
                    else
                        sensorData.Temp += temp.Data;
                }

                if (strSensorType == "all" || strSensorType == "humi")
                {
                    if (sensorData.Humi == null)
                        sensorData.Humi = humidity.Data;
                    else
                        sensorData.Humi += humidity.Data;
                }

                if (dicSensorDataCount.TryGetValue(strDate, out nDataCount) == false)
                {
                    dicSensorDataCount[strDate] = dataCount.Data;
                }
                else
                    dicSensorDataCount[strDate] = nDataCount + dataCount.Data;
            }

            foreach (KeyValuePair<string, SensorAvgData> pair in dicSensorDatas)
            {
                if (dicSensorDataCount.TryGetValue(pair.Key, out nDataCount) == false)
                    return new ResponseSensorAvgData(false, "알수없는 에러입니다.");

                if (pair.Value.Pm2_5 != null)
                    pair.Value.Pm2_5 /= nDataCount;

                if (pair.Value.No2 != null)
                    pair.Value.No2 /= nDataCount;

                if (pair.Value.O3 != null)
                    pair.Value.O3 /= nDataCount;

                if (pair.Value.Temp != null)
                    pair.Value.Temp /= nDataCount;

                if (pair.Value.Humi != null)
                    pair.Value.Humi /= nDataCount;
            }

            List<SensorAvgData> sensorDatas = new List<SensorAvgData>();
            sensorDatas.AddRange(dicSensorDatas.Values);
            sensorDatas.Sort();

            ResponseSensorAvgData response = new ResponseSensorAvgData(true, "");

            foreach (SensorAvgData data in sensorDatas)
            {
                object avg = data.ToSensorAvgObject();

                if (avg != null)
                    response.SensorAvgDatas.Add(avg);
            }

            return response;
        }

        private static ResponseSensorAvgData GetAllSensorAvgDay(RequestSensorAvgData request, WebDBManager dbMgr)
        {
            string strErrorMessage;
            string strCondition = GetAvgDayCondition(request, out strErrorMessage);

            if (strCondition == null)
                return new ResponseSensorAvgData(false, strErrorMessage);

            if (request.Serno != null)
                strCondition += " and serno = '" + request.Serno + "'";

            string strSensorType = request.SensorType.ToLower().Trim();

            if (strSensorType != "pm2_5" && strSensorType != "no2" && strSensorType != "o3" &&
                strSensorType != "temp" && strSensorType != "humi" && strSensorType != "all")
                return new ResponseSensorAvgData(false, "알수없는 센서타입입니다. sensorType : " + request.SensorType);

            string strSQL = "Select year, month, day, serno, pm2_5Total, no2Total, o3Total, tempTotal, humidityTotal, dataCount from SensorStatisticsDay where " + strCondition;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return new ResponseSensorAvgData(false, dbMgr.LastErrorMessage);

            int nResultCount = arrResult.Count;

            SensorAvgData sensorData;
            int nDataCount = 0;

            // Key : serno + _ + year * 10000 + month * 100 + day;
            Dictionary<string, SensorAvgData> dicSensorDatas = new Dictionary<string, SensorAvgData>();
            Dictionary<string, int> dicSensorDataCount = new Dictionary<string, int>();

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                VariousData<int> year = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> month = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> day = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strSerno = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<float> pm2_5 = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> no2 = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<float> o3 = WebDBManager.GetFloatField(arrResult[i + 6].ToString());
                VariousData<float> temp = WebDBManager.GetFloatField(arrResult[i + 7].ToString());
                VariousData<float> humidity = WebDBManager.GetFloatField(arrResult[i + 8].ToString());
                VariousData<int> dataCount = WebDBManager.GetIntField(arrResult[i + 9].ToString());

                if (year == null || month == null || day == null ||
                    pm2_5 == null || no2 == null || o3 == null ||
                    temp == null || humidity == null || dataCount == null || strSerno == null)
                    continue;

                int date = year.Data * 10000 + month.Data * 100 + day.Data;
                string strDate = request.Serno == null ? date.ToString() : request.Serno + "_" + date.ToString();

                if (dicSensorDatas.TryGetValue(strDate, out sensorData) == false)
                {
                    sensorData = new SensorAvgData();
                    sensorData.Date = string.Format("{0}-{1:00}-{2:00}", year.Data, month.Data, day.Data);
                    dicSensorDatas[strDate] = sensorData;

                    if (request.Serno != null)
                        sensorData.Serno = strSerno;
                }

                if (strSensorType == "all" || strSensorType == "pm2_5")
                {
                    if (sensorData.Pm2_5 == null)
                        sensorData.Pm2_5 = pm2_5.Data;
                    else
                        sensorData.Pm2_5 += pm2_5.Data;
                }

                if (strSensorType == "all" || strSensorType == "no2")
                {
                    if (sensorData.No2 == null)
                        sensorData.No2 = no2.Data;
                    else
                        sensorData.No2 += no2.Data;
                }

                if (strSensorType == "all" || strSensorType == "o3")
                {
                    if (sensorData.O3 == null)
                        sensorData.O3 = o3.Data;
                    else
                        sensorData.O3 += o3.Data;
                }

                if (strSensorType == "all" || strSensorType == "temp")
                {
                    if (sensorData.Temp == null)
                        sensorData.Temp = temp.Data;
                    else
                        sensorData.Temp += temp.Data;
                }

                if (strSensorType == "all" || strSensorType == "humi")
                {
                    if (sensorData.Humi == null)
                        sensorData.Humi = humidity.Data;
                    else
                        sensorData.Humi += humidity.Data;
                }

                if (dicSensorDataCount.TryGetValue(strDate, out nDataCount) == false)
                {
                    dicSensorDataCount[strDate] = dataCount.Data;
                }
                else
                    dicSensorDataCount[strDate] = nDataCount + dataCount.Data;
            }

            foreach (KeyValuePair<string, SensorAvgData> pair in dicSensorDatas)
            {
                if (dicSensorDataCount.TryGetValue(pair.Key, out nDataCount) == false)
                    return new ResponseSensorAvgData(false, "알수없는 에러입니다.");

                if (pair.Value.Pm2_5 != null)
                    pair.Value.Pm2_5 /= nDataCount;

                if (pair.Value.No2 != null)
                    pair.Value.No2 /= nDataCount;

                if (pair.Value.O3 != null)
                    pair.Value.O3 /= nDataCount;

                if (pair.Value.Temp != null)
                    pair.Value.Temp /= nDataCount;

                if (pair.Value.Humi != null)
                    pair.Value.Humi /= nDataCount;
            }

            List<SensorAvgData> sensorDatas = new List<SensorAvgData>();
            sensorDatas.AddRange(dicSensorDatas.Values);
            sensorDatas.Sort();

            ResponseSensorAvgData response = new ResponseSensorAvgData(true, "");

            foreach (SensorAvgData data in sensorDatas)
            {
                object avg = data.ToSensorAvgObject();

                if (avg != null)
                    response.SensorAvgDatas.Add(avg);
            }

            return response;
        }

        private static string GetAvgMonthCondition(RequestSensorAvgData request, out string strErrorMessage)
        {
            if (request.BeginMonth == null)
            {
                strErrorMessage = "beginMonth가 null입니다.";
                return null;
            }

            if (request.EndMonth == null)
            {
                strErrorMessage = "endMonth가 null입니다.";
                return null;
            }

            string strBeginDate = string.Format("{0}-{1:00}", request.BeginYear, (int)request.BeginMonth);
            string strEndDate = string.Format("{0}-{1:00}", request.EndYear, (int)request.EndMonth);

            if (string.Compare(strBeginDate, strEndDate) > 0)
            {
                strErrorMessage = string.Format("시작일자가 종료일자보다 더 나중입니다.(BeginYear : {0}, BeginMonth : {1}, EndYear : {2}, EndMonth : {3})", request.BeginYear, (int)request.BeginMonth, request.EndYear, (int)request.EndMonth);
                return null;
            }

            strErrorMessage = null;
            string strCondition = "";

            if (request.BeginYear == request.EndYear)
            {
                if (request.BeginMonth == request.EndMonth)
                    strCondition = string.Format("year = {0} and month = {1}", request.BeginYear, request.BeginMonth);
                else
                    strCondition = string.Format("year = {0} and month >= {1} and month <= {2}", request.BeginYear, request.BeginMonth, request.EndMonth);
            }
            else
            {
                strCondition = string.Format("year >= {0} and year <= {1} and ((year > {0} and year < {1}) or (year = {0} and month >= {2}) or (year = {1} and month <= {3}))",
                    request.BeginYear, request.EndYear, request.BeginMonth, request.EndMonth);
            }

            return strCondition;
        }

        private static string GetAvgDayCondition(RequestSensorAvgData request, out string strErrorMessage)
        {
            if (request.BeginMonth == null)
            {
                strErrorMessage = "beginMonth가 null입니다.";
                return null;
            }

            if (request.EndMonth == null)
            {
                strErrorMessage = "endMonth가 null입니다.";
                return null;
            }

            if (request.BeginDay == null)
            {
                strErrorMessage = "beginDay가 null입니다.";
                return null;
            }

            if (request.EndDay == null)
            {
                strErrorMessage = "endDay가 null입니다.";
                return null;
            }

            string strBeginDate = string.Format("{0}-{1:00}-{2:00}", request.BeginYear, (int)request.BeginMonth, (int)request.BeginDay);
            string strEndDate = string.Format("{0}-{1:00}-{2:00}", request.EndYear, (int)request.EndMonth, (int)request.EndDay);

            if (string.Compare(strBeginDate, strEndDate) > 0)
            {
                strErrorMessage = string.Format("시작일자가 종료일자보다 더 나중입니다.(BeginYear : {0}, BeginMonth : {1}, BeginDay : {2}, EndYear : {3}, EndMonth : {4}, EndDay : {5})", request.BeginYear, (int)request.BeginMonth, (int)request.BeginDay, request.EndYear, (int)request.EndMonth, (int)request.EndDay);
                return null;
            }

            strErrorMessage = null;
            string strCondition = "";

            if (request.BeginYear == request.EndYear)
            {
                if (request.BeginMonth == request.EndMonth)
                    strCondition = string.Format("year = {0} and month = {1} and day >= {2} and day <= {3}", request.BeginYear, request.BeginMonth, request.BeginDay, request.EndDay);
                else
                    strCondition = string.Format("year = {0} and (month >= {1} and month <= {2} and ((month = {1} and day >= {3}) or (month = {2} and day <= {4}))", request.BeginYear, request.BeginMonth, request.EndMonth, request.BeginDay, request.EndDay);
            }
            else
            {
                strCondition = string.Format("year >= {0} and year <= {1} and ((year > {0} and year < {1}) or (year = {0} and month > {2}) or (year = {0} and month = {2} and day >= {4}) or (year = {1} and month < {3}) or (year = {1} and month = {3} and day <= {5}))",
                    request.BeginYear, request.EndYear, request.BeginMonth, request.EndMonth, request.BeginDay, request.EndDay);
            }

            return strCondition;
        }

        public static ResponseSensorData GetSensorData(RequestSensorData request, WebDBManager dbMgr)
        {
            string strBeginDate = request.BeginYear.ToString();
            string strEndDate = request.EndYear.ToString();

            if (request.BeginMonth != null && request.EndMonth != null)
            {
                strBeginDate += string.Format("-{0:00}", (int)request.BeginMonth);
                strEndDate += string.Format("-{0:00}", (int)request.EndMonth);
            }
            else
            {
                if (request.BeginYear > request.EndYear)
                    return new ResponseSensorData(false, string.Format("시작년도가 종료년도보다 더 나중입니다.(BeginYear : {0}, EndYear : {1})", request.BeginYear, request.EndYear));

                string strCondition = string.Format("year >= {0} and year <= {1}", request.BeginYear, request.EndYear);
                return GetSensorData(strCondition, request.Serno, dbMgr);
            }

            if (request.BeginDay != null && request.EndDay != null)
            {
                strBeginDate += string.Format("-{0:00}", (int)request.BeginDay);
                strEndDate += string.Format("-{0:00}", (int)request.EndDay);
            }
            else
            {
                if (string.Compare(strBeginDate, strEndDate) > 0)
                {
                    return new ResponseSensorData(false, string.Format("시작일자가 종료일자보다 더 나중입니다.(BeginYear : {0}, BeginMonth : {1}, EndYear : {2}, EndMonth : {3})", request.BeginYear, (int)request.BeginMonth, request.EndYear, (int)request.EndMonth));
                }

                string strCondition = string.Format("(year = {0} and month >= {1}) or (year = {2} and month <= {3}) or (year > {0} and year < {2})", request.BeginYear, (int)request.BeginMonth, request.EndYear, (int)request.EndMonth);
                return GetSensorData(strCondition, request.Serno, dbMgr);
            }

            if (request.BeginHour != null && request.EndHour != null)
            {
                strBeginDate += string.Format(" {0:00}", (int)request.BeginHour);
                strEndDate += string.Format(" {0:00}", (int)request.EndHour);
            }
            else
            {
                if (string.Compare(strBeginDate, strEndDate) > 0)
                {
                    return new ResponseSensorData(false, string.Format("시작일자가 종료일자보다 더 나중입니다.(BeginYear : {0}, BeginMonth : {1}, BeginDay : {2}, EndYear : {3}, EndMonth : {4}, EndDay : {5})",
                        request.BeginYear, (int)request.BeginMonth, (int)request.BeginDay, request.EndYear, (int)request.EndMonth, (int)request.EndDay));
                }

                string strCondition = "";

                if (request.BeginYear == request.EndYear)
                {
                    if (request.BeginMonth == request.EndMonth)
                        strCondition = string.Format("year = {0} and month = {1} and day >= {2} and day <= {3}", request.BeginYear, request.BeginMonth, request.BeginDay, request.EndDay);
                    else
                        strCondition = string.Format("year = {0} and (month >= {1} and month <= {2} and ((month = {1} and day >= {3}) or (month = {2} and day <= {4}))", request.BeginYear, request.BeginMonth, request.EndMonth, request.BeginDay, request.EndDay);
                }
                else
                {
                    strCondition = string.Format("year >= {0} and year <= {1} and ((year > {0} and year < {1}) or (year = {0} and month > {2}) or (year = {0} and month = {2} and day >= {4}) or (year = {1} and month < {3}) or (year = {1} and month = {3} and day <= {5}))",
                        request.BeginYear, request.EndYear, request.BeginMonth, request.EndMonth, request.BeginDay, request.EndDay);
                }

                return GetSensorData(strCondition, request.Serno, dbMgr);
            }

            if (request.BeginMinute != null && request.EndMinute != null)
            {
                strBeginDate += string.Format(":{0:00}", (int)request.BeginMinute);
                strEndDate += string.Format(":{0:00}", (int)request.EndMinute);
            }
            else
            {
                if (string.Compare(strBeginDate, strEndDate) > 0)
                {
                    return new ResponseSensorData(false, string.Format("시작일자가 종료일자보다 더 나중입니다.(BeginYear : {0}, BeginMonth : {1}, BeginDay : {2}, BeginHour : {3}, EndYear : {4}, EndMonth : {5}, EndDay : {6}, EndHour : {7})",
                        request.BeginYear, (int)request.BeginMonth, (int)request.BeginDay, (int)request.BeginHour, request.EndYear, (int)request.EndMonth, (int)request.EndDay, (int)request.EndHour));
                }

                string strCondition = string.Format("regdate >= '{0}:00:00' and regdate <= '{1}:59:59.999'", strBeginDate, strEndDate);
                return GetSensorData(strCondition, request.Serno, dbMgr);
            }

            if (request.BeginSecond != null && request.EndSecond != null)
            {
                strBeginDate += string.Format(":{0:00}", (int)request.BeginSecond);
                strEndDate += string.Format(":{0:00}.999", (int)request.EndSecond);
            }
            else
            {
                if (string.Compare(strBeginDate, strEndDate) > 0)
                {
                    return new ResponseSensorData(false, string.Format("시작일자가 종료일자보다 더 나중입니다.(BeginYear : {0}, BeginMonth : {1}, BeginDay : {2}, BeginHour : {3}, BeginMinute : {4}, EndYear : {5}, EndMonth : {6}, EndDay : {7}, EndHour : {8}, EndMinute : {9})",
                        request.BeginYear, (int)request.BeginMonth, (int)request.BeginDay, (int)request.BeginHour, (int)request.BeginMinute, request.EndYear, (int)request.EndMonth, (int)request.EndDay, (int)request.EndHour, (int)request.EndMinute));
                }

                string strCondition = string.Format("regdate >= '{0}:00' and regdate <= '{1}:59.999'", strBeginDate, strEndDate);
                return GetSensorData(strCondition, request.Serno, dbMgr);
            }

            if (string.Compare(strBeginDate, strEndDate) > 0)
            {
                return new ResponseSensorData(false, string.Format("시작일자가 종료일자보다 더 나중입니다.(BeginYear : {0}, BeginMonth : {1}, BeginDay : {2}, BeginHour : {3}, BeginMinute : {4}, BeginSecond : {5}, EndYear : {6}, EndMonth : {7}, EndDay : {8}, EndHour : {9}, EndMinute : {10}, EndSecond : {11})",
                    request.BeginYear, (int)request.BeginMonth, (int)request.BeginDay, (int)request.BeginHour, (int)request.BeginMinute, (int)request.BeginSecond, request.EndYear, (int)request.EndMonth, (int)request.EndDay, (int)request.EndHour, (int)request.EndMinute, (int)request.EndSecond));
            }

            return GetSensorData(string.Format("regdate >= '{0}' and regdate <= '{1}'", strBeginDate, strEndDate), request.Serno, dbMgr);
        }

        private static ResponseSensorData GetSensorData(string strCondition, string serno, WebDBManager dbMgr)
        {
            if (serno != null)
                strCondition = string.Format("({0}) and serno = '{1}'", strCondition, serno);

            string strSQL = "Select id, year, month, day, serno, pm2_5, no2, o3, temp, humidity, regdate, lat, lon from SensorData where " + strCondition;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return new ResponseSensorData(false, dbMgr.LastErrorMessage);

            List<SensorData> sensorDatas = new List<SensorData>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 12; i += 13)
            {
                VariousData<int> year = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> month = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> day = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strSerno = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<float> pm2_5 = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                VariousData<float> no2 = WebDBManager.GetFloatField(arrResult[i + 6].ToString());
                VariousData<float> o3 = WebDBManager.GetFloatField(arrResult[i + 7].ToString());
                VariousData<float> temp = WebDBManager.GetFloatField(arrResult[i + 8].ToString());
                VariousData<float> humidity = WebDBManager.GetFloatField(arrResult[i + 9].ToString());
                VariousData<DateTime> regdate = WebDBManager.GetDateTimeField(arrResult[i + 10]);
                VariousData<float> lat = WebDBManager.GetFloatField(arrResult[i + 11].ToString());
                VariousData<float> lon = WebDBManager.GetFloatField(arrResult[i + 12].ToString());

                if (year == null || month == null || day == null ||
                    strSerno == null || pm2_5 == null || no2 == null ||
                    o3 == null || temp == null || humidity == null ||
                    regdate == null || lat == null || lon == null)
                    continue;

                SensorData sensorData = new SensorData();

                sensorData.Serno = strSerno;
                sensorData.Pm2_5 = pm2_5.Data;
                sensorData.No2 = no2.Data;
                sensorData.O3 = o3.Data;
                sensorData.Temp = temp.Data;
                sensorData.Humi = humidity.Data;
                sensorData.Regdate = GetDateTimeString(regdate.Data);
                sensorData.Lat = lat.Data;
                sensorData.Lon = lon.Data;

                sensorDatas.Add(sensorData);
            }

            ResponseSensorData response = new ResponseSensorData(true, "");
            response.SensorDatas = sensorDatas;

            return response;
        }

        private static string GetDateTimeString(DateTime time)
        {
            return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        public static ResponseSensorAlarmList GetAlarmList(WebDBManager dbMgr, bool activeOnly)
        {
            string strSQL = "Select id, regdate, stype, active, atype, value from SensorAlarm where ID in (Select max(ID) from SensorAlarm group by stype)";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return new ResponseSensorAlarmList(false, dbMgr.LastErrorMessage);

            int nResultCount = arrResult.Count;
            ResponseSensorAlarmList response = new ResponseSensorAlarmList(true, "");

            for (int i=0;i<nResultCount-5;i+=6)
            {
                string strID = WebDBManager.GetStringField(arrResult[i]);
                VariousData<DateTime> regdate = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                string strSensorType = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> active = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strAlarmType = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<float> value = WebDBManager.GetFloatField(arrResult[i + 5].ToString());

                if (strID == null || regdate == null || strSensorType == null ||
                    active == null || strAlarmType == null)
                    continue;

                bool isActive = active.Data == 1;

                if (activeOnly && isActive == false)
                    continue;

                AlarmData alarm = new AlarmData();
                alarm.Id = strID;
                alarm.Active = isActive;
                alarm.Regdate = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", regdate.Data.Year, regdate.Data.Month, regdate.Data.Day, regdate.Data.Hour, regdate.Data.Minute, regdate.Data.Second);
                alarm.Stype = strSensorType;
                alarm.Atype = strAlarmType;

                if (strSensorType == "temp")
                {
                    if (isActive)
                    {
                        if (value == null)
                            continue;

                        if (value.Data < 10)
                            alarm.Stype = "cold";
                        else
                            alarm.Stype = "heat";
                    }
                    else
                    {
                        if (regdate.Data.Month >= 10 || regdate.Data.Month <= 4)
                            alarm.Stype = "cold";
                        else
                            alarm.Stype = "heat";
                    }
                }

                response.SensorAlarmList.Add(alarm);
            }

            return response;
        }
    }
}
