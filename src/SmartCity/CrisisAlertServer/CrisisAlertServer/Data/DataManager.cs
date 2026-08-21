using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertServer.Data
{
    public class DataManager
    {
        WebDBManager m_dbMgr = null;
        Dictionary<int, DataMidTemp> m_dicMidTemp = new Dictionary<int, DataMidTemp>();
        Dictionary<int, DataLongTemp> m_dicLongTemp = new Dictionary<int, DataLongTemp>();

        Dictionary<int, FloodSensor> m_dicFloodSensors = new Dictionary<int, FloodSensor>();
        public Dictionary<int, FloodSensor> DicFloodSensors
        {
            get { return m_dicFloodSensors; }
            set { m_dicFloodSensors = value; }
        }

        Dictionary<string, FloodSensorFallData> m_dicFloodSensorFalls = new Dictionary<string, FloodSensorFallData>();
        public Dictionary<string, FloodSensorFallData> DicFloodSensorFalls
        {
            get { return m_dicFloodSensorFalls; }
            set { m_dicFloodSensorFalls = value; }
        }

        Dictionary<string, FloodSensorLevelData> m_dicFloodSensorLevels = new Dictionary<string, FloodSensorLevelData>();
        public Dictionary<string, FloodSensorLevelData> DicFloodSensorLevels
        {
            get { return m_dicFloodSensorLevels; }
            set { m_dicFloodSensorLevels = value; }
        }

        public DataManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;

            LoadFloodSensors(m_dbMgr, m_dicFloodSensors);
            LoadFloodTimeFall(m_dbMgr, m_dicFloodSensorFalls);
            LoadFloodFallLevel(m_dbMgr, m_dicFloodSensorLevels);

            LoadMidTemp(m_dbMgr, m_dicMidTemp);
            LoadLongTemp(m_dbMgr, m_dicLongTemp);
        }

        private bool LoadMidTemp(WebDBManager dbMgr, Dictionary<int, DataMidTemp> dicMidTemp)
        {
            dicMidTemp.Clear();

            string strSQL = string.Format("SELECT ID, AnnounceTime, TempAfterOneDay, TempAfterOneDay FROM MidTemp");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DataMidTemp dataMid;

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strAnnounceTime = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strTempAfterOneDay = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strTempAfterTwoDay = WebDBManager.GetStringField(arrResult[i + 3], "");

                dataMid = new DataMidTemp();
                dataMid.ID = nID;
                dataMid.AnnounceTime = strAnnounceTime;
                dataMid.AfterOneDay = strTempAfterOneDay;
                dataMid.AfterTwoDay = strTempAfterTwoDay;

                dicMidTemp[nID] = dataMid;
            }

            return true;
        }

        private bool LoadLongTemp(WebDBManager dbMgr, Dictionary<int, DataLongTemp> dicLongTemp)
        {
            dicLongTemp.Clear();

            string strSQL = string.Format("SELECT ID, AnnounceTime, TempAfterThreeDay, TempAfterFourDay, TempAfterFiveDay, TempAfterSixDay FROM LongTemp");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DataLongTemp dataLong;

            for (int i = 0; i < nCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strAnnounceTime = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strTempAfterThreeDay = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strTempAfterFourDay = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strTempAfterFiveDay = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strTempAfterSixDay = WebDBManager.GetStringField(arrResult[i + 5], "");

                dataLong = new DataLongTemp();
                dataLong.ID = nID;
                dataLong.AnnounceTime = strAnnounceTime;
                dataLong.AfterThreeDay = strTempAfterThreeDay;
                dataLong.AfterFourDay = strTempAfterFourDay;
                dataLong.AfterFiveDay = strTempAfterFiveDay;
                dataLong.AfterSixDay = strTempAfterSixDay;

                dicLongTemp[nID] = dataLong;
            }

            return true;
        }

        private bool LoadFloodTimeFall(WebDBManager dbMgr, Dictionary<string, FloodSensorFallData> dicFloodSensorFalls)
        {
            dicFloodSensorFalls.Clear();

            string strSQL = string.Format("SELECT ID, SensorID, Fall, Time, Depth FROM FloodTimeFall");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            FloodSensorFallData sensorFallData;
            TimeFallData timeFallData;


            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSensorID = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nFall = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nTime = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                float nDepth = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0);

                if (dicFloodSensorFalls.ContainsKey(strSensorID))
                    sensorFallData = dicFloodSensorFalls[strSensorID];
                else
                {
                    sensorFallData = new FloodSensorFallData();
                    sensorFallData.SensorID = strSensorID;

                    dicFloodSensorFalls[strSensorID] = sensorFallData;
                }

                if (sensorFallData.DicTimeFalls.ContainsKey(nFall))
                    timeFallData = sensorFallData.DicTimeFalls[nFall];
                else
                {
                    timeFallData = new TimeFallData();
                    timeFallData.Fall = nFall;

                    sensorFallData.DicTimeFalls[nFall] = timeFallData;
                }

                timeFallData.DicFalls[nTime] = nDepth;
            }

                return true;
        }

        private bool LoadFloodFallLevel(WebDBManager dbMgr, Dictionary<string, FloodSensorLevelData> dicFloodSensorLevels)
        {
            dicFloodSensorLevels.Clear();

            string strSQL = string.Format("SELECT ID, SensorID, OverValue, LowerValue, RiskLevel FROM FloodFallLevel");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            FloodSensorLevelData sensorLevelData;
            FallLevelData fallLevelData;


            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSensorID = WebDBManager.GetStringField(arrResult[i + 1], "");
                float fOverValue = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0);
                float fLowerValue = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0);
                int nRiskLevel = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                if (dicFloodSensorLevels.ContainsKey(strSensorID))
                    sensorLevelData = dicFloodSensorLevels[strSensorID];
                else
                {
                    sensorLevelData = new FloodSensorLevelData();
                    sensorLevelData.SensorID = strSensorID;

                    dicFloodSensorLevels[strSensorID] = sensorLevelData;
                }

                fallLevelData = new FallLevelData();
                fallLevelData.OverValue = fOverValue;
                fallLevelData.LowerValue = fLowerValue;
                fallLevelData.Level = (RiskLevel)nRiskLevel;

                sensorLevelData.FallLevels.Add(fallLevelData);
            }

            return true;
        }

        private bool LoadFloodSensors(WebDBManager dbMgr, Dictionary<int, FloodSensor> dicFloodSensors)
        {
            dicFloodSensors.Clear();

            string strSQL = string.Format("SELECT ID, SensorID, State, Addr, MeasureTime, Depth, Flow, Message, IsUserModifity  FROM FloodSensor");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DateTime dtDefault = new DateTime();
            FloodSensor floodSensor;

            for (int i = 0; i < nCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSensorID = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strState = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strAddr = WebDBManager.GetStringField(arrResult[i + 3], "");
                DateTime dtMeasureTime = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
                float fDepth = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0);
                float fFlow = WebDBManager.GetFloatField(arrResult[i + 6].ToString(), 0);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 7], "");
                int nUserModifity = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);


                floodSensor = new FloodSensor();
                floodSensor.ID = nID;
                floodSensor.SensorID = strSensorID;
                floodSensor.State = strState;
                floodSensor.Addr = strAddr;
                floodSensor.MeasureTime = dtMeasureTime;
                floodSensor.Depth = fDepth;
                floodSensor.Flow = fFlow;
                floodSensor.Message = strMessage;
                floodSensor.UserModifity = nUserModifity;

                dicFloodSensors[nID] = floodSensor;
            }

            return true;
        }

        public bool InsertMidTemp(DataMidTemp midTemp)
        {
            string strAnnounceTime = midTemp.AnnounceTime;
            string strTempAfterOneDay = midTemp.AfterOneDay;
            string strTempAfterTwoDay = midTemp.AfterTwoDay;

            string szText = "INSERT INTO MidTemp (AnnounceTime, TempAfterOneDay, TempAfterTwoDay) VALUES('{0}', '{1}', '{2}')";
            string szSQL = string.Format(szText, strAnnounceTime, strTempAfterOneDay, strTempAfterTwoDay);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            return true;
        }

        public bool InsertLongTemp(DataLongTemp longTemp)
        {
            string strAnnounceTime = longTemp.AnnounceTime;
            string strTempAfterThreeDay = longTemp.AfterThreeDay;
            string strTempAfterFourDay = longTemp.AfterFourDay;
            string strTempAfterFiveDay = longTemp.AfterFiveDay;
            string strTempAfterSixDay = longTemp.AfterSixDay;

            string strTempAfterSevenDay = longTemp.AfterSevenDay;
            string strTempAfterEightDay = longTemp.AfterEightDay;
            string strTempAfterNineDay = longTemp.AfterNineDay;
            string strTempAfterTenDay = longTemp.AfterTenDay;

            string szText = "INSERT INTO LongTemp (AnnounceTime, TempAfterThreeDay, TempAfterFourDay, TempAfterFiveDay, TempAfterSixDay, TempAfterSevenDay, TempAfterEightDay, TempAfterNineDay, TempAfterTenDay) " +
                "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')";
            string szSQL = string.Format(szText, strAnnounceTime, strTempAfterThreeDay, strTempAfterFourDay, strTempAfterFiveDay, strTempAfterSixDay, strTempAfterSevenDay, strTempAfterEightDay, strTempAfterNineDay, strTempAfterTenDay);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            return true;
        }

        public DataMidTemp GetMidTemp(string strDate)
        {
            string strSQL = string.Format("SELECT ID, AnnounceTime, TempAfterOneDay, TempAfterTwoDay FROM MidTemp WHERE AnnounceTime Like '" + strDate + "%'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return null;

            int nCount = arrResult.Count;
            if (nCount == 0) return null;

            DataMidTemp dataMid = new DataMidTemp();

            for (int i = 0; i < nCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strAnnounceTime = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strTempAfterOneDay = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strTempAfterTwoDay = WebDBManager.GetStringField(arrResult[i + 3], "");

                dataMid.ID = nID;
                dataMid.AnnounceTime = strAnnounceTime;
                dataMid.AfterOneDay = strTempAfterOneDay;
                dataMid.AfterTwoDay = strTempAfterTwoDay;
            }

            return dataMid;
        }

        public DataLongTemp GetLongTemp(string strDate)
        {
            string strSQL = string.Format("SELECT ID, AnnounceTime, TempAfterThreeDay, TempAfterFourDay, TempAfterFiveDay, TempAfterSixDay, TempAfterSevenDay, TempAfterEightDay, TempAfterNineDay, TempAfterTenDay " +
                "FROM LongTemp WHERE AnnounceTime Like '" + strDate + "%'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return null;

            int nCount = arrResult.Count;
            if (nCount == 0) return null;

            DataLongTemp dataLong = new DataLongTemp();

            for (int i = 0; i < nCount - 9; i += 10)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strAnnounceTime = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strTempAfterThreeDay = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strTempAfterFourDay = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strTempAfterFiveDay = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strTempAfterSixDay = WebDBManager.GetStringField(arrResult[i + 5], "");

                string strTempAfterSevenDay = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strTempAfterEightDay = WebDBManager.GetStringField(arrResult[i + 7], "");
                string strTempAfterNineDay = WebDBManager.GetStringField(arrResult[i + 8], "");
                string strTempAfterTenDay = WebDBManager.GetStringField(arrResult[i + 9], "");

                dataLong.ID = nID;
                dataLong.AnnounceTime = strAnnounceTime;
                dataLong.AfterThreeDay = strTempAfterThreeDay;
                dataLong.AfterFourDay = strTempAfterFourDay;
                dataLong.AfterFiveDay = strTempAfterFiveDay;
                dataLong.AfterSixDay = strTempAfterSixDay;

                dataLong.AfterSevenDay = strTempAfterSevenDay;
                dataLong.AfterEightDay = strTempAfterEightDay;
                dataLong.AfterNineDay = strTempAfterNineDay;
                dataLong.AfterTenDay = strTempAfterTenDay;
            }

            return dataLong;
        }

        public bool InsertExpectTemp(DataExpectTemp expectTemp)
        {
            string strAnnounceTime = expectTemp.AnnounceTime;
            string strTempAfterOneDay = expectTemp.AfterOneDay;
            string strTempAfterTwoDay = expectTemp.AfterTwoDay;
            string strTempAfterThreeDay = expectTemp.AfterThreeDay;
            string strTempAfterFourDay = expectTemp.AfterFourDay;
            string strTempAfterFiveDay = expectTemp.AfterFiveDay;
            string strTempAfterSixDay = expectTemp.AfterSixDay;


            string szText = "INSERT INTO ExpectTemp (AnnounceTime, TempAfterOneDay, TempAfterTwoDay, TempAfterThreeDay, TempAfterFourDay, TempAfterFiveDay, TempAfterSixDay) " +
                "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')";
            string szSQL = string.Format(szText, strAnnounceTime, strTempAfterOneDay, strTempAfterTwoDay, strTempAfterThreeDay, strTempAfterFourDay, strTempAfterFiveDay, strTempAfterSixDay);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            return true;
        }

        public DataExpectTemp GetExpectTemp(string strDate)
        {
            string strSQL = string.Format("SELECT ID, AnnounceTime, TempAfterOneDay, TempAfterTwoDay, TempAfterThreeDay, TempAfterFourDay, TempAfterFiveDay, TempAfterSixDay FROM ExpectTemp WHERE AnnounceTime Like '" + strDate + "%'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return null;

            int nCount = arrResult.Count;
            if (nCount == 0) return null;

            DataExpectTemp dataExpect = new DataExpectTemp();

            for (int i = 0; i < nCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strAnnounceTime = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strTempAfterOneDay = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strTempAfterTwoDay = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strTempAfterThreeDay = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strTempAfterFourDay = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strTempAfterFiveDay = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strTempAfterSixDay = WebDBManager.GetStringField(arrResult[i + 7], "");

                dataExpect.ID = nID;
                dataExpect.AnnounceTime = strAnnounceTime;
                dataExpect.AfterOneDay = strTempAfterOneDay;
                dataExpect.AfterTwoDay = strTempAfterTwoDay;
                dataExpect.AfterThreeDay = strTempAfterThreeDay;
                dataExpect.AfterFourDay = strTempAfterFourDay;
                dataExpect.AfterFiveDay = strTempAfterFiveDay;
                dataExpect.AfterSixDay = strTempAfterSixDay;
            }

            return dataExpect;
        }

        public bool InsertFloodData(FloodNewData flood)
        {
            string strObserveTime = flood.ObserveTime.ToString("yyyy-MM-dd HH:mm");
            string strDistrictCode = flood.DistrictCode;
            string strFall = flood.Fall;

            string strSQL = string.Format("INSERT INTO FloodData (ObserveTime, DistrictCode, Fall) VALUES ('" + strObserveTime + "', '" + strDistrictCode + "', '" + strFall + "')");

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                return false;

            return true;
        }

        public bool CheckFloodSensorID(string strID)
        {
            int nChk = 0;
            bool bRet = false;

            string strSQL = string.Format("SELECT ID, SensorID FROM FloodSensor WHERE SensorID = '" + strID + "'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return bRet;

            int nCount = arrResult.Count;
            if (nCount == 0) return bRet;

            for (int i = 0; i < nCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");

                nChk++;
            }

            if (nChk > 0)
                bRet = true;

            return bRet;
        }

        public bool InsertFloodSensor(FloodSensor flood)
        {
            string strSensorID = flood.SensorID;
            string strMeasureTime = flood.MeasureTime.ToString("yyyy-MM-dd HH:mm");

            string szText = "INSERT INTO FloodSensor (SensorID, MeasureTime) " +
                "VALUES('{0}', '{1}')";
            string szSQL = string.Format(szText, strSensorID, strMeasureTime);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            return true;
        }

        //public bool UpdateFloodSensorData(FloodData data)
        //{
        //    string strSensorID = data.SensorID;
        //    string strMeasureTime = data.MeasureTime.ToString("yyyy-MM-dd HH:mm");

        //    string strSQL = string.Format("UPDATE FloodSensor SET MeasureTime = '" + strMeasureTime + "'  Where SensorID = '" + strSensorID + "'");
        //    ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
        //    if (arrResult == null) return false;

        //    return true;
        //}

        public bool UpdateFloodSensorData(FloodSensor sensor)
        {
            int nID = sensor.ID;
            string strState = sensor.State;
            float fDepth = sensor.Depth;
            string strMeasureTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            string strSQL = string.Format("UPDATE FloodSensor SET State = '" + strState + "', Depth = " + fDepth + ", MeasureTime = '" + strMeasureTime + "'  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public Dictionary<int, SMSData> LoadSMSMessage()
        {
            Dictionary<int, SMSData> dicSMSData = new Dictionary<int, SMSData>();

            string strSQL = string.Format("SELECT ID, NumberList, Message, FacilityType FROM SMSSendMessage");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return null;

            int nCount = arrResult.Count;
            if (nCount == 0) return dicSMSData;

            for (int i = 0; i < nCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strNumberList = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strMessage = WebDBManager.GetStringField(arrResult[i + 2], "");
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);

                List<string> listNumber = new List<string>();

                SMSData data = new SMSData();
                data.ID = nID;
                data.Message = strMessage;

                string[] arrNumberList = strNumberList.Split(',');
                int nListCount = arrNumberList.Length;

                for (int j = 0; j < nListCount; j++)
                {
                    string strNumber = arrNumberList[j].Trim();
                    listNumber.Add(strNumber);
                }

                data.NumberList = listNumber;

                dicSMSData[nID] = data;
            }

            return dicSMSData;

        }

        public bool DeleteSMSMessage(int nID)
        {
            bool bRet = true;

            string szText = "DELETE FROM SMSSendMessage WHERE ID = {0}";
            string szSQL = string.Format(szText, nID);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool InsertCollapseData(CollapseData collapse)
        {
            string strSensorID = collapse.SensorID;
            short nSlopeID = collapse.SlopeID;
            string strEvelDate = collapse.EvelDate.ToString("yyyy-MM-dd HH:mm");
            string strLevel = SensorData.ChangeLevelNumToType(collapse.Level.ToString());

            string szText = "INSERT INTO CollapseData (SensorID, SlopeID, EvelDate, Level) " +
                "VALUES('{0}', {1}, '{2}', {3})";
            string szSQL = string.Format(szText, strSensorID, nSlopeID, strEvelDate, strLevel);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            return true;
        }

        public bool UpdateCollapseSensorData(CollapseData data)
        {
            string strSensorID = data.SensorID;
            string strMeasureTime = data.EvelDate.ToString("yyyy-MM-dd HH:mm");

            string strLevel = SensorData.ChangeLevelNumToType(data.Level);

            string strSQL = string.Format("UPDATE CollapseSensor SET MeasureTime = '" + strMeasureTime + "', State = '" + strLevel + "' Where SensorID = '" + strSensorID + "'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public CollapseSensor GetCollapseSensor(string SensorID)
        {
            CollapseSensor sensor = null;

            string strSQL = string.Format("SELECT ID, SensorID, State, Addr, MeasureTime, Message, IsUserModifity FROM CollapseSensor WHERE SensorID = '" + SensorID + "'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return sensor;

            int nCount = arrResult.Count;
            if (nCount == 0) return sensor;

            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSensorID = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strState = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strAddr = WebDBManager.GetStringField(arrResult[i + 3], "");
                DateTime dtMeasureTime = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 5], "");
                int nUserModifity = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);

                sensor.ID = nID;
                sensor.SensorID = strSensorID;
                sensor.State = strState;
                sensor.Addr = strAddr;
                sensor.MeasureTime = dtMeasureTime; ;
                sensor.Message = strMessage;
                sensor.UserModifity = nUserModifity;
            }

            return sensor;

        }

        public bool InsertAlertAarm(FacilityType type, int nSensorID, string strAddress, string strRiskLevel)
        {
            bool bRet = true;

            string szText = "INSERT INTO AlertAlarm (FacilityType, SensorID, RiskLevel, Address) VALUES({0}, {1}, '{2}', '{3}')";
            string szSQL = string.Format(szText, (int)type, nSensorID, strRiskLevel, strAddress);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool InsertAlertReport(FacilityType type, int nID, string oldData, string newData)
        {
            bool bRet = true;

            int nFacilityType = (int)type;
            string strSQL = string.Format("Insert into AlertRecord (FacilityType, SensorID, DataName, OriginData, NewData) " +
                "Values (" + nFacilityType + ", " + nID + ", '" + CommonString.RiskLevel_Kor + "', '" + oldData + "', '" + newData + "')");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null) bRet = false;

            return bRet;
        }

        public bool InsertDataReport(FacilityType facilityType, int nID, string strDataName, string strOriginData, string strNewData)
        {
            int nFacilityType = (int)facilityType;
            string strSQL = string.Format("Insert into DataRecord (FacilityType, SensorID, DataName, OriginData, NewData) " +
                "Values (" + nFacilityType + ", " + nID + ", '" + strDataName + "', '" + strOriginData + "', '" + strNewData + "')");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool InsertFireData(FireData fire)
        {
            int nEventID = fire.EventID;
            string strOccurType = fire.OccurType;
            string strOccurTime = fire.OccurTime.ToString("yyyy-MM-dd HH:mm");
            float fLatitude = fire.Latitude;
            float fLongitude = fire.Longitude;
            int nDangerRange = fire.DangerRange;
            string strDangerStep = fire.DangerStep;
            int nBuildingId = fire.BuildingId;
            short nEventFinishYn = fire.EventFinishYn;

            string szText = "INSERT INTO FireData (EventID, OccurType, OccurTime, Latitude, Longitude, DangerRange, DangerStep, BuildingId, EventFinishYn) " +
                "VALUES({0}, '{1}', '{2}', {3}, {4}, {5}, '{6}', {7}, {8})";
            string szSQL = string.Format(szText, nEventID, strOccurType, strOccurTime, fLatitude, fLongitude, nDangerRange, strDangerStep, nBuildingId, nEventFinishYn);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            return true;
        }

        public FireSensor GetFireSensor(string SensorID)
        {
            FireSensor fireSensor = null;

            string strSQL = string.Format("SELECT ID, SensorID, State, Addr, OccurTime, CloseTime, IsAfterFire, AlarmPeriodStart, AlarmPeriodEnd, WeakStart, WeakEnd, IsInitReact," +
                " Demander, DeathToll, Message, IsUserModifity FROM FireSensor WHERE SensorID = '" + SensorID + "'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return fireSensor;

            int nCount = arrResult.Count;
            if (nCount == 0) return fireSensor;

            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nCount - 15; i += 16)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSensorID = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strState = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strAddr = WebDBManager.GetStringField(arrResult[i + 3], "");
                DateTime dtOccurTime = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
                DateTime dtCloseTime = WebDBManager.GetDateTimeField(arrResult[i + 5], dtDefault);
                int nAfterFire = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                DateTime dtAlarmPeriodStart = WebDBManager.GetDateTimeField(arrResult[i + 7], dtDefault);
                DateTime dtAlarmPeriodEnd = WebDBManager.GetDateTimeField(arrResult[i + 8], dtDefault);
                DateTime dtWeakStart = WebDBManager.GetDateTimeField(arrResult[i + 9], dtDefault);
                DateTime dtWeakEnd = WebDBManager.GetDateTimeField(arrResult[i + 10], dtDefault);
                int nInitReact = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0);
                int nDemander = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 0);
                int nDeathToll = WebDBManager.GetIntField(arrResult[i + 13].ToString(), 0);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 14], "");
                int nUserModifity = WebDBManager.GetIntField(arrResult[i + 15].ToString(), 0);


                fireSensor = new FireSensor();
                fireSensor.ID = nID;
                fireSensor.SensorID = strSensorID;
                fireSensor.State = strState;
                fireSensor.Addr = strAddr;
                fireSensor.OccurTime = dtOccurTime;
                fireSensor.CloseTime = dtCloseTime;

                if (nAfterFire == 0) fireSensor.AfterFire = false;
                else fireSensor.AfterFire = true;

                fireSensor.AlarmPeriodStart = dtAlarmPeriodStart;
                fireSensor.AlarmPeriodEnd = dtAlarmPeriodEnd;
                fireSensor.WeakStart = dtWeakStart;
                fireSensor.WeakEnd = dtWeakEnd;
                fireSensor.InitReact = nInitReact;
                fireSensor.Demander = nDemander;
                fireSensor.DeathToll = nDeathToll;
                fireSensor.Message = strMessage;
                fireSensor.UserModifity = nUserModifity;
            }

            return fireSensor;

        }

        public bool UpdateFireSensorData(FireData data)
        {
            if (data == null)
                return false;

            int nBuildingId = data.BuildingId;
            short sEventFinishYn = data.EventFinishYn;
            string strOccurTime = data.OccurTime.ToString("yyyy-MM-dd HH:mm");
            string strDangerStep = SensorData.ChangeLevelNumToType(data.DangerStep);

            string strSQL = string.Format("UPDATE FireSensor SET OccurTime = '" + strOccurTime + "', State = '" + strDangerStep + "' Where SensorID = '" + nBuildingId + "'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool InsertHeatData(HeatData heat)
        {
            int nEventID = heat.EventID;
            int nGroupID = heat.GroupID;
            short nUniqueID = heat.UniqueID;
            double dLatitude = heat.Latitude;
            double dLongitude = heat.Longitude;
            DateTime dtMeasureTime = heat.MeasureTime;
            string strTemperature = heat.Temperature;
            string strHumidity = heat.Humidity;
            //string strDust = heat.Dust;
            //int nDirection = heat.Direction;
            //int nVelocity = heat.Velocity;
            int nGrade = heat.Grade;
            int nWorkStatus = heat.WorkStatus;
            double dPrevTemperature = heat.PrevTemperature;
            DateTime dtRegDate = heat.RegDate;

            string strMeasureTime = dtMeasureTime.ToString("yyyy-MM-dd HH:mm");
            string strRegDate = dtRegDate.ToString("yyyy-MM-dd HH:mm");

            //string szText = "INSERT INTO HeatData (GroupID, UniqueID, MeasureTime, Temperature, Humidity, Dust, Direction, Velocity) " +
            //    "VALUES({0}, {1}, '{2}', '{3}', '{4}', '{5}', {6}, {7})";
            string szText = "INSERT INTO HeatData (EventID, GroupID, UniqueID, Latitude, Longitude, MeasureTime, Temperature, Humidity, Grade, WorkStatus, PrevTemperature, RegDate) " +
                "VALUES({0}, {1}, '{2}', '{3}', '{4}', '{5}', {6}, {7})";
            //string szSQL = string.Format(szText, nGroupID, nUniqueID, strMeasureTime, strTemperature, strHumidity, strDust, nDirection, nVelocity);
            string szSQL = string.Format(szText, nEventID, nGroupID, nUniqueID, dLatitude, dLongitude, strMeasureTime, strTemperature, strHumidity, nGrade, nWorkStatus, dPrevTemperature, strRegDate);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            return true;
        }

        public HeatSensor GetHeatSensor(int nSensorGroupID, short nSensorUniqueID)
        {
            HeatSensor sensor = null;

            string strSQL = string.Format("SELECT ID, SensorID, GroupID, UniqueID, State, Addr, OccurTime, Temperature, Humidity, Direction, Speed, MeasPeriodStart, MeasPeriodEnd, PreliminaryDate," +
                " AdvisoryDate, AlertDate, DeathToll, Message, IsUserModifity FROM HeatSensor WHERE GroupID = " + nSensorGroupID + " AND UniqueID = " + nSensorUniqueID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return sensor;

            int nCount = arrResult.Count;
            if (nCount == 0) return sensor;

            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nCount - 18; i += 19)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSensorID = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nGroupID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nUniqueID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                string strState = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strAddr = WebDBManager.GetStringField(arrResult[i + 5], "");
                DateTime dtOccurTime = WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);

                float fTemperature = WebDBManager.GetFloatField(arrResult[i + 7].ToString(), 0);
                float fHumidity = WebDBManager.GetFloatField(arrResult[i + 8].ToString(), 0);
                float fDirection = WebDBManager.GetFloatField(arrResult[i + 9].ToString(), 0);
                float fSpeed = WebDBManager.GetFloatField(arrResult[i + 10].ToString(), 0);
                DateTime dtMeasPeriodStart = WebDBManager.GetDateTimeField(arrResult[i + 11], dtDefault);
                DateTime dtMeasPeriodEnd = WebDBManager.GetDateTimeField(arrResult[i + 12], dtDefault);
                DateTime dtPreliminaryDate = WebDBManager.GetDateTimeField(arrResult[i + 13], dtDefault);
                DateTime dtAdvisoryDate = WebDBManager.GetDateTimeField(arrResult[i + 14], dtDefault);
                DateTime dtAlertDate = WebDBManager.GetDateTimeField(arrResult[i + 15], dtDefault);
                int nDeathToll = WebDBManager.GetIntField(arrResult[i + 16].ToString(), 0);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 17], "");
                int nUserModifity = WebDBManager.GetIntField(arrResult[i + 18].ToString(), 0);

                sensor = new HeatSensor();
                sensor.ID = nID;
                sensor.SensorID = strSensorID;
                sensor.GroupID = nGroupID;
                sensor.UniqueID = nUniqueID;
                sensor.State = strState;
                sensor.Addr = strAddr;
                sensor.OccurTime = dtOccurTime;
                sensor.Temperature = fTemperature;
                sensor.Humidity = fHumidity;
                sensor.Direction = fDirection;
                sensor.Speed = fSpeed;
                sensor.MeasPeriodStart = dtMeasPeriodStart;
                sensor.MeasPeriodEnd = dtMeasPeriodEnd;
                sensor.PreliminaryDate = dtPreliminaryDate;
                sensor.AdvisoryDate = dtAdvisoryDate;
                sensor.AlertDate = dtAlertDate;
                sensor.DeathToll = nDeathToll;
                sensor.Message = strMessage;
                sensor.UserModifity = nUserModifity;

                break;
            }

            return sensor;
        }

        public bool UpdateHeatSensorData(HeatData data)
        {
            if (data == null)
                return false;

            int nGroupID = data.GroupID;
            short nUniqueID = data.UniqueID;
            string strMeasureTime = data.MeasureTime.ToString("yyyy-MM-dd HH:mm");
            float fTemperature = float.Parse(data.Temperature);
            float fHumidity = float.Parse(data.Humidity);
            //int nDirection = data.Direction;
            //int nVelocity = data.Velocity;

            string strLevel = SensorData.ChangeLevelNumToType(data.Grade.ToString());

            string strSQL = string.Format("UPDATE HeatSensor SET State = '" + strLevel + "' OccurTime = '" + strMeasureTime + "', Temperature = "+ fTemperature + ", Humidity = " + fHumidity + " Where GroupID = " + nGroupID + " AND UniqueID = " + nUniqueID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }
    }
}
