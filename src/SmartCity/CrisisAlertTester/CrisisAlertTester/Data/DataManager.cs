using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertTester.Data
{
    public class DataManager
    {
        WebDBManager m_dbMgr = null;

        private Dictionary<int, FireSensor> m_dicFireSensors = new Dictionary<int, FireSensor>();
        public Dictionary<int, FireSensor> DicFireSensors
        {
            get { return m_dicFireSensors; }
            set { m_dicFireSensors = value; }
        }

        private Dictionary<int, HeatSensor> m_dicHeatSensors = new Dictionary<int, HeatSensor>();
        public Dictionary<int, HeatSensor> DicHeatSensors
        {
            get { return m_dicHeatSensors; }
            set { m_dicHeatSensors = value; }
        }

        private Dictionary<int, FloodSensor> m_dicFloodSensors = new Dictionary<int, FloodSensor>();
        public Dictionary<int, FloodSensor> DicFloodSensors
        {
            get { return m_dicFloodSensors; }
            set { m_dicFloodSensors = value; }
        }

        private Dictionary<int, CollapseSensor> m_dicCollapseSensors = new Dictionary<int, CollapseSensor>();
        public Dictionary<int, CollapseSensor> DicCollapseSensors
        {
            get { return m_dicCollapseSensors; }
            set { m_dicCollapseSensors = value; }
        }

        public DataManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            LoadSensors();
        }

        public bool LoadSensors()
        {
            LoadFireSensors(m_dbMgr, m_dicFireSensors);
            LoadHeatSensors(m_dbMgr, m_dicHeatSensors);
            LoadFloodSensors(m_dbMgr, m_dicFloodSensors);
            LoadCollapseSensors(m_dbMgr, m_dicCollapseSensors);

            return true;
        }

        public bool LoadFireSensors(WebDBManager dbMgr, Dictionary<int, FireSensor> dicFireSensors)
        {
            dicFireSensors.Clear();

            string strSQL = string.Format("SELECT ID, SensorID, State, Addr, OccurTime, CloseTime, IsAfterFire, AlarmPeriodStart, AlarmPeriodEnd, WeakStart, WeakEnd, IsInitReact," +
                " Demander, DeathToll, Message, IsUserModifity FROM FireSensor");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DateTime dtDefault = new DateTime();
            FireSensor fireSensor;

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

                dicFireSensors[nID] = fireSensor;
            }

            return true;
        }

        public bool LoadHeatSensors(WebDBManager dbMgr, Dictionary<int, HeatSensor> dicHeatSensors)
        {
            dicHeatSensors.Clear();

            string strSQL = string.Format("SELECT ID, SensorID, State, Addr, OccurTime, Temperature, Humidity, Direction, Speed, MeasPeriodStart, MeasPeriodEnd, PreliminaryDate," +
                " AdvisoryDate, AlertDate, DeathToll, Message, IsUserModifity FROM HeatSensor");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DateTime dtDefault = new DateTime();
            HeatSensor heatSensor;

            for (int i = 0; i < nCount - 16; i += 17)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSensorID = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strState = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strAddr = WebDBManager.GetStringField(arrResult[i + 3], "");
                DateTime dtOccurTime = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);

                float fTemperature = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0);
                float fHumidity = WebDBManager.GetFloatField(arrResult[i + 6].ToString(), 0);
                float fDirection = WebDBManager.GetFloatField(arrResult[i + 7].ToString(), 0);
                float fSpeed = WebDBManager.GetFloatField(arrResult[i + 8].ToString(), 0);
                DateTime dtMeasPeriodStart = WebDBManager.GetDateTimeField(arrResult[i + 9], dtDefault);
                DateTime dtMeasPeriodEnd = WebDBManager.GetDateTimeField(arrResult[i + 10], dtDefault);
                DateTime dtPreliminaryDate = WebDBManager.GetDateTimeField(arrResult[i + 11], dtDefault);
                DateTime dtAdvisoryDate = WebDBManager.GetDateTimeField(arrResult[i + 12], dtDefault);
                DateTime dtAlertDate = WebDBManager.GetDateTimeField(arrResult[i + 13], dtDefault);
                int nDeathToll = WebDBManager.GetIntField(arrResult[i + 14].ToString(), 0);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 15], "");
                int nUserModifity = WebDBManager.GetIntField(arrResult[i + 16].ToString(), 0);


                heatSensor = new HeatSensor();
                heatSensor.ID = nID;
                heatSensor.SensorID = strSensorID;
                heatSensor.State = strState;
                heatSensor.Addr = strAddr;
                heatSensor.OccurTime = dtOccurTime;
                heatSensor.Temperature = fTemperature;
                heatSensor.Humidity = fHumidity;
                heatSensor.Direction = fDirection;
                heatSensor.Speed = fSpeed;
                heatSensor.MeasPeriodStart = dtMeasPeriodStart;
                heatSensor.MeasPeriodEnd = dtMeasPeriodEnd;
                heatSensor.PreliminaryDate = dtPreliminaryDate;
                heatSensor.AdvisoryDate = dtAdvisoryDate;
                heatSensor.AlertDate = dtAlertDate;
                heatSensor.DeathToll = nDeathToll;
                heatSensor.Message = strMessage;
                heatSensor.UserModifity = nUserModifity;

                dicHeatSensors[nID] = heatSensor;
            }

            return true;
        }

        public bool LoadFloodSensors(WebDBManager dbMgr, Dictionary<int, FloodSensor> dicFloodSensors)
        {
            dicFloodSensors.Clear();

            string strSQL = string.Format("SELECT ID, SensorID, State, Addr, MeasureTime, Depth, Flow, Message, IsUserModifity FROM FloodSensor");
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

        public bool LoadCollapseSensors(WebDBManager dbMgr, Dictionary<int, CollapseSensor> dicCollapseSensors)
        {
            dicCollapseSensors.Clear();

            string strSQL = string.Format("SELECT ID, SensorID, State, Addr, MeasureTime, Message, IsUserModifity FROM CollapseSensor");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DateTime dtDefault = new DateTime();
            CollapseSensor collapseSensor;

            for (int i = 0; i < nCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strSensorID = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strState = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strAddr = WebDBManager.GetStringField(arrResult[i + 3], "");
                DateTime dtMeasureTime = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 5], "");
                int nUserModifity = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);

                collapseSensor = new CollapseSensor();
                collapseSensor.ID = nID;
                collapseSensor.SensorID = strSensorID;
                collapseSensor.State = strState;
                collapseSensor.Addr = strAddr;
                collapseSensor.MeasureTime = dtMeasureTime; ;
                collapseSensor.Message = strMessage;
                collapseSensor.UserModifity = nUserModifity;

                dicCollapseSensors[nID] = collapseSensor;
            }

            return true;
        }

        public bool InsertAlertReport(FacilityType facilityType, int nID, string strDataName, string strOriginData, string strNewData)
        {
            int nFacilityType = (int)facilityType;
            string strSQL = string.Format("Insert into AlertRecord (FacilityType, SensorID, DataName, OriginData, NewData) " +
                "Values (" + nFacilityType + ", " + nID + ", '" + strDataName + "', '" + strOriginData + "', '" + strNewData + "')");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
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

        public bool UpdateFireSensorState(int nID, string strSensorState)
        {
            string strSQL = string.Format("UPDATE FireSensor SET STATE = '" + strSensorState + "', IsUserModifity = 1  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool UpdateFloodSensorState(int nID, string strSensorState)
        {
            string strSQL = string.Format("UPDATE FloodSensor SET STATE = '" + strSensorState + "', IsUserModifity = 1  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool UpdateHeatSensorState(int nID, string strSensorState)
        {
            string strSQL = string.Format("UPDATE HeatSensor SET STATE = '" + strSensorState + "', IsUserModifity = 1  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool UpdateCollapseSensorState(int nID, string strSensorState)
        {
            string strSQL = string.Format("UPDATE CollapseSensor SET STATE = '" + strSensorState + "', IsUserModifity = 1  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }
    }

}
