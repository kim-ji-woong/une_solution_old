using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertManager.Data
{
    public class DataManager
    {
        WebDBManager m_dbMgr = null;

        private Dictionary<int, FireSensor> m_dicFireSensors = new Dictionary<int, FireSensor>();
        private Dictionary<int, HeatSensor> m_dicHeatSensors = new Dictionary<int, HeatSensor>();
        private Dictionary<int, FloodSensor> m_dicFloodSensors = new Dictionary<int, FloodSensor>();
        private Dictionary<int, CollapseSensor> m_dicCollapseSensors = new Dictionary<int, CollapseSensor>();

        Dictionary<FacilityType, Dictionary<int, DataReport>> m_dicFacilityDataReports = new Dictionary<FacilityType, Dictionary<int, DataReport>>();
        Dictionary<FacilityType, Dictionary<int, AlertReport>> m_dicFacilityAlertReports = new Dictionary<FacilityType, Dictionary<int, AlertReport>>();
        Dictionary<FacilityType, Dictionary<int, SMSReport>> m_dicFacilitySMSReports = new Dictionary<FacilityType, Dictionary<int, SMSReport>>();

        private Dictionary<int, DataTeam> m_dicRegularTeams = new Dictionary<int, DataTeam>();
        private Dictionary<int, DataTeam> m_dicSubTeams = new Dictionary<int, DataTeam>();

        private Dictionary<int, JobLevel> m_dicJobLevels = new Dictionary<int, JobLevel>();

        private Dictionary<int, DataCompanyMember> m_dicCompanyMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<int, FacilityManager> m_dicFacilityManagers = new Dictionary<int, FacilityManager>();
        private Dictionary<int, FacilityMessage> m_dicFacilityMessages = new Dictionary<int, FacilityMessage>();

        private Dictionary<int, FacilityManual> m_dicFacilityManual = new Dictionary<int, FacilityManual>(); 
        //private Dictionary<int, AlarmData> m_dicAlarms = new Dictionary<int, AlarmData>();
        Dictionary<FacilityType, Dictionary<int, AlarmData>> m_dicFacilityAlarms = new Dictionary<FacilityType, Dictionary<int, AlarmData>>();

        public Dictionary<int, FireSensor> DicFireSensors
        {
            get { return m_dicFireSensors; }
            set { m_dicFireSensors = value; }
        }

        public Dictionary<int, HeatSensor> DicHeatSensors
        {
            get { return m_dicHeatSensors; }
            set { m_dicHeatSensors = value; }
        }

        public Dictionary<int, FloodSensor> DicFloodSensors
        {
            get { return m_dicFloodSensors; }
            set { m_dicFloodSensors = value; }
        }

        public Dictionary<int, CollapseSensor> DicCollapseSensors
        {
            get { return m_dicCollapseSensors; }
            set { m_dicCollapseSensors = value; }
        }

        public Dictionary<int, DataTeam> RegularTeams
        {
            get { return m_dicRegularTeams; }
            set { m_dicRegularTeams = value; }
        }

        public Dictionary<int, DataCompanyMember> CompanyMembers
        {
            get { return m_dicCompanyMembers; }
            set { m_dicCompanyMembers = value; }
        }

        public Dictionary<int, JobLevel> JobLevels
        {
            get { return m_dicJobLevels; }
            set { m_dicJobLevels = value; }
        }

        public Dictionary<int, FacilityManager> FacilityManagers
        {
            get { return m_dicFacilityManagers; }
            set { m_dicFacilityManagers = value; }
        }

        public DataManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            LoadSensors();
            LoadTeam();
            LoadReports();
            LoadManual();
            LoadAlarm();
        }

        public bool LoadSensors()
        {
            LoadFireSensors(m_dbMgr, m_dicFireSensors);
            LoadHeatSensors(m_dbMgr, m_dicHeatSensors);
            LoadFloodSensors(m_dbMgr, m_dicFloodSensors);
            LoadCollapseSensors(m_dbMgr, m_dicCollapseSensors);

            return true;
        }

        public bool LoadReports()
        {
            LoadDataReport(m_dbMgr, m_dicFacilityDataReports);
            LoadAlertRecord(m_dbMgr, m_dicFacilityAlertReports);
            LoadSMSRecord(m_dbMgr, m_dicFacilitySMSReports);

            return true;
        }

        public bool LoadTeam()
        {
            LoadRegularTeam(m_dbMgr, m_dicRegularTeams);
            LoadJobLevels(m_dbMgr, m_dicJobLevels);
            LoadCompanyMembers(m_dbMgr, m_dicCompanyMembers);
            LoadFacilityManager(m_dbMgr, m_dicFacilityManagers);
            LoadFacilityMessage(m_dbMgr, m_dicFacilityMessages);

            return true;
        }

        public bool LoadManual()
        {
            LoadFacilityManual(m_dbMgr, m_dicFacilityManual);

            return true;
        }

        public bool LoadAlarm()
        {
            LoadAlarms(m_dbMgr, m_dicFacilityAlarms);

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
                collapseSensor.MeasureTime = dtMeasureTime;;
                collapseSensor.Message = strMessage;
                collapseSensor.UserModifity = nUserModifity;

                dicCollapseSensors[nID] = collapseSensor;
            }

            return true;
        }

        public bool UpdateFireSensorState(FireSensor fireSensor, string strSensorState)
        {
            int nID = fireSensor.ID;
            string strSQL = string.Format("UPDATE FireSensor SET STATE = '" + strSensorState + "', IsUserModifity = 1  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool UpdateFloodSensorState(FloodSensor floodSensor, string strSensorState)
        {
            int nID = floodSensor.ID;
            string strSQL = string.Format("UPDATE FloodSensor SET STATE = '" + strSensorState + "', IsUserModifity = 1  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool UpdateHeatSensorState(HeatSensor heatSensor, string strSensorState)
        {
            int nID = heatSensor.ID;
            string strSQL = string.Format("UPDATE HeatSensor SET STATE = '" + strSensorState + "', IsUserModifity = 1  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool UpdateCollapseSensorState(CollapseSensor collapseSensor, string strSensorState)
        {
            int nID = collapseSensor.ID;
            string strSQL = string.Format("UPDATE CollapseSensor SET STATE = '" + strSensorState + "', IsUserModifity = 1  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool ResetFireSensor(FireSensor fireSensor)
        {
            int nID = fireSensor.ID;
            string strSQL = string.Format("UPDATE FireSensor SET STATE = '" + CommonString.RiskLevel_Normal + "', OccurTime = Null, CloseTime = Null, IsAfterFire = 0, IsInitReact = 0,Demander = 0, DeathToll = 0, IsUserModifity = 0  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool ResetFloodSensor(FloodSensor floodSensor)
        {
            int nID = floodSensor.ID;
            string strSQL = string.Format("UPDATE FloodSensor SET STATE = '" + CommonString.RiskLevel_Normal + "', MeasureTime = Null, Depth = 0, Flow = 0, IsUserModifity = 0  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool ResetHeatSensor(HeatSensor heatSensor)
        {
            int nID = heatSensor.ID;
            string strSQL = string.Format("UPDATE HeatSensor SET STATE = '" + CommonString.RiskLevel_Normal + "', OccurTime = Null, MeasPeriodStart = Null, MeasPeriodEnd = Null, PreliminaryDate = Null, AdvisoryDate = Null, AlertDate = Null, DeathToll = 0, IsUserModifity = 0  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool ResetCollapseSensor(CollapseSensor collapseSensor)
        {
            int nID = collapseSensor.ID;
            string strSQL = string.Format("UPDATE CollapseSensor SET STATE = '" + CommonString.RiskLevel_Normal + "', MeasureTime = Null, IsUserModifity = 0  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool UpdateFireSensorInfo(FireSensor fireSensor)
        {
            DateTime dtDefault = new DateTime();

            int nID = fireSensor.ID;
            string strAddr = fireSensor.Addr;
            string strState = fireSensor.State;
            
            DateTime dtOccurTime = fireSensor.OccurTime;
            string strOccurTime = "Null";
            if (dtOccurTime != dtDefault)
                strOccurTime = "'" + dtOccurTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";

            DateTime dtCloseTime = fireSensor.CloseTime;
            string strCloseTime = "Null";
            if (dtCloseTime != dtDefault)
                strCloseTime = "'" + dtCloseTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";

            bool bAfterFire = fireSensor.AfterFire;
            int nAfterFire = 0;

            if (bAfterFire == true)
                nAfterFire = 1;

            DateTime dtAlarmPeriodStart = fireSensor.AlarmPeriodStart;
            string strAlarmPeriodStart = "Null";
            if (dtAlarmPeriodStart != dtDefault)
                strAlarmPeriodStart = "'" + dtAlarmPeriodStart.ToString("yyyy-MM-dd") + "'";

            DateTime dtAlarmPeriodEnd = fireSensor.AlarmPeriodEnd;
            string strAlarmPeriodEnd = "Null";
            if (dtAlarmPeriodEnd != dtDefault)
                strAlarmPeriodEnd = "'" + dtAlarmPeriodEnd.ToString("yyyy-MM-dd") + "'";

            DateTime dtWeakStart = fireSensor.WeakStart;
            string strWeakStart = "Null";
            if (dtWeakStart != dtDefault)
                strWeakStart = "'" + dtWeakStart.ToString("yyyy-MM-dd") + "'";

            DateTime dtWeakEnd = fireSensor.WeakEnd;
            string strWeakEnd = "Null";
            if (dtWeakEnd != dtDefault)
                strWeakEnd = "'" + dtWeakEnd.ToString("yyyy-MM-dd") + "'";

            int nInitReact = fireSensor.InitReact;
            int nDemander = fireSensor.Demander;
            int nDeathToll = fireSensor.DeathToll;
            int nUserModifity = fireSensor.UserModifity;

            string strSQL = string.Format("UPDATE FireSensor SET " +
                "OccurTime = " + strOccurTime + ", CloseTime = " + strCloseTime + ", " +
                "IsAfterFire = " + nAfterFire + ", AlarmPeriodStart = " + strAlarmPeriodStart + ", " +
                "AlarmPeriodEnd = " + strAlarmPeriodEnd  + ", WeakStart = " + strWeakStart + ", " +
                "WeakEnd = " + strWeakEnd + ", IsInitReact = " + nInitReact +", " +
                "Demander = " + nDemander + ", DeathToll = " + nDeathToll + ", " +
                "State = '" + strState  + "' " +
                "Where ID = " + nID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool UpdateHeatSensorInfo(HeatSensor heatSensor)
        {
            DateTime dtDefault = new DateTime();

            int nID = heatSensor.ID;
            string strAddr = heatSensor.Addr;
            string strState = heatSensor.State;

            DateTime dtOccurTime = heatSensor.OccurTime;
            string strOccurTime = "Null";
            if (dtOccurTime != dtDefault)
                strOccurTime = "'" + dtOccurTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";

            float fTemperature = heatSensor.Temperature;
            float fHumidity = heatSensor.Humidity;
            float fDirection = heatSensor.Direction;
            float fSpeed = heatSensor.Speed;

            DateTime dtMeasPeriodStart = heatSensor.MeasPeriodStart;
            string strMeasPeriodStart = "Null";
            if (dtMeasPeriodStart != dtDefault)
                strMeasPeriodStart = "'" + dtMeasPeriodStart.ToString("yyyy-MM-dd") + "'";

            DateTime dtMeasPeriodEnd = heatSensor.MeasPeriodEnd;
            string strMeasPeriodEnd = "Null";
            if (dtMeasPeriodEnd != dtDefault)
                strMeasPeriodEnd = "'" + dtMeasPeriodEnd.ToString("yyyy-MM-dd") + "'";

            DateTime dtPreliminaryDate = heatSensor.PreliminaryDate;
            string strPreliminaryDate = "Null";
            if (dtPreliminaryDate != dtDefault)
                strPreliminaryDate = "'" + dtPreliminaryDate.ToString("yyyy-MM-dd") + "'";

            DateTime dtAdvisoryDate = heatSensor.AdvisoryDate;
            string strAdvisoryDate = "Null";
            if (dtAdvisoryDate != dtDefault)
                strAdvisoryDate = "'" + dtAdvisoryDate.ToString("yyyy-MM-dd") + "'";

            DateTime dtAlertDate = heatSensor.AlertDate;
            string strAlertDate = "Null";
            if (dtAlertDate != dtDefault)
                strAlertDate = "'" + dtAlertDate.ToString("yyyy-MM-dd") + "'";

            int nDeathToll = heatSensor.DeathToll;

            string strSQL = string.Format("UPDATE HeatSensor SET " +
                "OccurTime = " + strOccurTime + ", Temperature = " + fTemperature + ", " +
                "Humidity = " + fHumidity + ", Direction = " + fDirection + ", " +
                "Speed = " + fSpeed + ", MeasPeriodStart = " + strMeasPeriodStart + ", " +
                "MeasPeriodEnd = " + strMeasPeriodEnd + ", PreliminaryDate = " + strPreliminaryDate + ", " +
                "AdvisoryDate = " + strAdvisoryDate + ", AlertDate = " + strAlertDate + ", " +
                "DeathToll = " + nDeathToll + ", State = '" + strState + "' " +
                "Where ID = " + nID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
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

        public bool InsertAlertReport(FacilityType facilityType, int nID, string strDataName, string strOriginData, string strNewData)
        {
            int nFacilityType = (int)facilityType;
            string strSQL = string.Format("Insert into AlertRecord (FacilityType, SensorID, DataName, OriginData, NewData) " +
                "Values (" + nFacilityType + ", " + nID + ", '" + strDataName + "', '" + strOriginData + "', '" + strNewData + "')");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool LoadDataReport(WebDBManager dbMgr, Dictionary<FacilityType, Dictionary<int, DataReport>> dicFacilityDataReports)
        {
            dicFacilityDataReports.Clear();

            string strSQL = string.Format("SELECT ID, FacilityType, SensorID, OccurTime, DataName, OriginData, NewData FROM DataRecord");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DateTime dtDefault = new DateTime();
            DataReport data;

            for (int i = 0; i < nCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nSensorID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                DateTime dtOccurTime = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strDataName = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strOriginData = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strNewData = WebDBManager.GetStringField(arrResult[i + 6], "");

                data = new DataReport();
                data.ID = nID;
                data.FacilityType = (FacilityType)nFacilityType;
                data.SensorID = nSensorID;
                data.OccurTime = dtOccurTime;
                data.DataName = strDataName;
                data.OriginData = strOriginData;
                data.NewData = strNewData;


                // 타입에 따른 리포트 데이터 분류 저장
                if (dicFacilityDataReports.ContainsKey((FacilityType)nFacilityType))
                {
                    Dictionary<int, DataReport> dicDataReports = dicFacilityDataReports[(FacilityType)nFacilityType];
                    dicDataReports[nID] = data;
                }
                else
                {
                    dicFacilityDataReports[(FacilityType)nFacilityType] = new Dictionary<int, DataReport>();
                    Dictionary<int, DataReport> dicDataReports = dicFacilityDataReports[(FacilityType)nFacilityType];
                    dicDataReports[nID] = data;
                }
            }

            return true;
        }

        public bool LoadAlertRecord(WebDBManager dbMgr, Dictionary<FacilityType, Dictionary<int, AlertReport>> dicFacilityAlertReports)
        {
            dicFacilityAlertReports.Clear();

            string strSQL = string.Format("SELECT ID, FacilityType, SensorID, OccurTime, DataName, OriginData, NewData FROM AlertRecord");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DateTime dtDefault = new DateTime();
            AlertReport data;

            for (int i = 0; i < nCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nSensorID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                DateTime dtOccurTime = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strDataName = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strOriginData = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strNewData = WebDBManager.GetStringField(arrResult[i + 6], "");

                data = new AlertReport();
                data.ID = nID;
                data.FacilityType = (FacilityType)nFacilityType;
                data.SensorID = nSensorID;
                data.OccurTime = dtOccurTime;
                data.DataName = strDataName;
                data.OriginData = strOriginData;
                data.NewData = strNewData;

                // 타입에 따른 리포트 데이터 분류 저장
                if (dicFacilityAlertReports.ContainsKey((FacilityType)nFacilityType))
                {
                    Dictionary<int, AlertReport> dicDataReports = dicFacilityAlertReports[(FacilityType)nFacilityType];
                    dicDataReports[nID] = data;
                }
                else
                {
                    dicFacilityAlertReports[(FacilityType)nFacilityType] = new Dictionary<int, AlertReport>();
                    Dictionary<int, AlertReport> dicAlertReports = dicFacilityAlertReports[(FacilityType)nFacilityType];
                    dicAlertReports[nID] = data;
                }
            }

            return true;
        }

        public bool LoadSMSRecord(WebDBManager dbMgr, Dictionary<FacilityType, Dictionary<int, SMSReport>> dicFacilitySMSReports)
        {
            dicFacilitySMSReports.Clear();

            string strSQL = string.Format("SELECT ID, FacilityType, SensorID, OccurTime, Message, Managers FROM SMSRecord");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DateTime dtDefault = new DateTime();
            SMSReport data;

            for (int i = 0; i < nCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nSensorID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                DateTime dtOccurTime = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strManagers = WebDBManager.GetStringField(arrResult[i + 5], "");

                data = new SMSReport();
                data.ID = nID;
                data.FacilityType = (FacilityType)nFacilityType;
                data.SensorID = nSensorID;
                data.OccurTime = dtOccurTime;
                data.Message = strMessage;
                data.Managers = strManagers;


                // 타입에 따른 리포트 데이터 분류 저장
                if (dicFacilitySMSReports.ContainsKey((FacilityType)nFacilityType))
                {
                    Dictionary<int, SMSReport> dicSMSReports = dicFacilitySMSReports[(FacilityType)nFacilityType];
                    dicSMSReports[nID] = data;
                }
                else
                {
                    dicFacilitySMSReports[(FacilityType)nFacilityType] = new Dictionary<int, SMSReport>();
                    Dictionary<int, SMSReport> dicSMSReports = dicFacilitySMSReports[(FacilityType)nFacilityType];
                    dicSMSReports[nID] = data;
                }
            }

            return true;
        }

        public Dictionary<int, DataReport> LoadFacilityDataReports(FacilityType facilityType)
        {
            Dictionary<int, DataReport> dicFacilityDataReports = new Dictionary<int, DataReport>();

            if (m_dicFacilityDataReports.ContainsKey(facilityType))
                dicFacilityDataReports = m_dicFacilityDataReports[facilityType];

            return dicFacilityDataReports;
        }

        public Dictionary<int, AlertReport> LoadFacilityAlertReports(FacilityType facilityType)
        {
            Dictionary<int, AlertReport> dicFacilityAlertReports = new Dictionary<int, AlertReport>();

            if (m_dicFacilityAlertReports.ContainsKey(facilityType))
                dicFacilityAlertReports = m_dicFacilityAlertReports[facilityType];

            return dicFacilityAlertReports;
        }

        public Dictionary<int, SMSReport> LoadFacilitySMSReports(FacilityType facilityType)
        {
            Dictionary<int, SMSReport> dicFacilitySMSReports = new Dictionary<int, SMSReport>();

            if (m_dicFacilitySMSReports.ContainsKey(facilityType))
                dicFacilitySMSReports = m_dicFacilitySMSReports[facilityType];

            return dicFacilitySMSReports;
        }

        public bool LoadRegularTeam(WebDBManager dbMgr, Dictionary<int, DataTeam> dicRegularTeam)
        {
            dicRegularTeam.Clear();

            string strSQL = string.Format("SELECT ID, TeamName, ParentTeamID FROM RegularTeam");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DataTeam dataTeam;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                dataTeam = new DataTeam();
                dataTeam.ID = nID;
                dataTeam.TeamName = strTeamName;

                if (nParentTeamID != -1)
                {
                    if (dicRegularTeam.ContainsKey(nParentTeamID))
                        dataTeam.ParentTeam = dicRegularTeam[nParentTeamID];
                }


                dicRegularTeam[nID] = dataTeam;
            }

            return true;
        }

        public Dictionary<int, DataTeam> GetSubTeams(int nParentID)
        {
            Dictionary<int, DataTeam> dicSubTeams = new Dictionary<int, DataTeam>();
            DataTeam dataTeam = new DataTeam();

            if (m_dicRegularTeams.ContainsKey(nParentID))
                dataTeam = m_dicRegularTeams[nParentID];
            else
                return dicSubTeams;

            ArrayList arrChildTeams = new ArrayList();
            arrChildTeams = dataTeam.ChildTeams;

            for (int i = 0; i < arrChildTeams.Count; i++)
            {
                DataTeam data = (DataTeam)arrChildTeams[i];

                dicSubTeams[data.ID] = data;
            }

            return dicSubTeams;
        }

        public Dictionary<int, DataTeam> GetMainTeams()
        {
            Dictionary<int, DataTeam> dicMainTeams = new Dictionary<int, DataTeam>();

            foreach (KeyValuePair<int, DataTeam> item in m_dicRegularTeams)
            {
                DataTeam data = item.Value;

                if (data.ParentTeam == null)
                {
                    dicMainTeams[data.ID] = data;
                }
            }

            return dicMainTeams;
        }

        public bool LoadJobLevels(WebDBManager dbMgr, Dictionary<int, JobLevel> dicJobLevels)
        {
            dicJobLevels.Clear();

            string strSQL = string.Format("SELECT ID, LevelName, LevelNo FROM JobLevel");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            JobLevel jobLevel;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strLevelName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nLevelNo = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                jobLevel = new JobLevel();
                jobLevel.ID = nID;
                jobLevel.LevelName = strLevelName;
                jobLevel.LevelNo = nLevelNo;

                dicJobLevels[nID] = jobLevel;
            }

            return true;
        }

        public bool LoadCompanyMembers(WebDBManager dbMgr, Dictionary<int, DataCompanyMember> dicCompanyMember)
        {
            dicCompanyMember.Clear();

            string strSQL = string.Format("SELECT ID, MemberName, RegularTeamID, LevelID, PhoneNumber, FacilityTypes FROM CompanyMember");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DataCompanyMember companyMember;

            for (int i = 0; i < nCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nRegularTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strFacilityTypes = WebDBManager.GetStringField(arrResult[i + 5], "");

                companyMember = new DataCompanyMember();
                companyMember.ID = nID;
                companyMember.MemberName = strMemberName;

                if (nRegularTeamID != -1)
                {
                    if (m_dicRegularTeams.ContainsKey(nRegularTeamID))
                        companyMember.Team = m_dicRegularTeams[nRegularTeamID];
                }

                if (nLevelID != -1)
                {
                    if(m_dicJobLevels.ContainsKey(nLevelID))
                        companyMember.Level = m_dicJobLevels[nLevelID];
                }
            
                companyMember.PhoneNumber = strPhoneNumber;
                companyMember.FacilityTypes = strFacilityTypes;

                dicCompanyMember[nID] = companyMember;
            }

            return true;
        }

        public Dictionary<int, DataCompanyMember> GetCompanyMembers(int nTeamID)
        {
            Dictionary<int, DataCompanyMember> dicCompanyMembers = new Dictionary<int, DataCompanyMember>();

            foreach (KeyValuePair<int, DataCompanyMember> item in m_dicCompanyMembers)
            {
                DataCompanyMember companyMember = item.Value;

                if (companyMember.Team.ID == nTeamID)
                    dicCompanyMembers[companyMember.ID] = companyMember;
            }

            return dicCompanyMembers;
        }

        public bool InsertFacilityManager(FacilityManager manager)
        {
            bool bRet = true;

            int nMemberID = -1;

            if (manager.CompanyMember != null)
                nMemberID = manager.CompanyMember.ID;

            FacilityType facilityType = manager.FacilityType;
            int nSensorID = manager.SensorID;
            string strDescription = manager.Description;
            string strDepartment = manager.Department;
            string strName = manager.Name;
            string strPhoneNumber = manager.PhoneNumber;

            string szText = "INSERT INTO FacilityManager (MemberID, FacilityType, SensorID, Description, Department, Name, PhoneNumber) VALUES({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}') ";
            string szSQL = string.Format(szText, nMemberID, (int)facilityType, nSensorID, strDescription, strDepartment, strName, strPhoneNumber);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool DeleteFacilityManager(FacilityManager manager)
        {
            bool bRet = true;

            int nID = manager.ID;

            string szText = "DELETE FROM FacilityManager WHERE ID = {0}";
            string szSQL = string.Format(szText, nID);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        private bool LoadFacilityManager(WebDBManager dbMgr, Dictionary<int, FacilityManager> dicFacilityManagers)
        {
            dicFacilityManagers.Clear();

            string strSQL = string.Format("SELECT ID, MemberID, FacilityType, SensorID, Description, Department, Name, PhoneNumber FROM FacilityManager");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            FacilityManager facilityManager;

            for (int i = 0; i < nCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nSensorID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strDepartment = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strName = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 7], "");

                facilityManager = new FacilityManager();
                facilityManager.ID = nID;

                if (nMemberID != -1 && m_dicCompanyMembers.ContainsKey(nMemberID))
                    facilityManager.CompanyMember = m_dicCompanyMembers[nMemberID];

                facilityManager.FacilityType = (FacilityType)nFacilityType;
                facilityManager.SensorID = nSensorID;
                facilityManager.Description = strDescription;
                facilityManager.Department = strDepartment;
                facilityManager.Name = strName;
                facilityManager.PhoneNumber = strPhoneNumber;

                dicFacilityManagers[nID] = facilityManager;
            }

            return true;
        }

        public void ReloadFacilityManager()
        {
            LoadFacilityManager(m_dbMgr, m_dicFacilityManagers);
            LoadFacilityMessage(m_dbMgr, m_dicFacilityMessages);
        }

        //public List<FacilityManager> GetFacilityManager(FacilityType type, int nSensorID)
        public List<FacilityManager> GetFacilityManager(FacilityType type)
        {
            List<FacilityManager> listFacilityManager = new List<FacilityManager>();

            foreach (KeyValuePair<int, FacilityManager> item in m_dicFacilityManagers)
            {
                FacilityManager facilityManager = item.Value;

                //if (facilityManager.FacilityType == type && facilityManager.SensorID == nSensorID)
                if (facilityManager.FacilityType == type)
                {
                    listFacilityManager.Add(facilityManager);
                }
            }

            return listFacilityManager;
        }

        public FacilityManager SearchFacilityManager(FacilityType type, int nID)
        {
            FacilityManager manager = null;

            foreach (KeyValuePair<int, FacilityManager> item in m_dicFacilityManagers)
            {
                FacilityManager facilityManager = item.Value;

                if (facilityManager.CompanyMember != null && facilityManager.CompanyMember.ID == nID && facilityManager.FacilityType == type)
                    manager = facilityManager;
            }

            return manager;
        }

        public List<FacilityManager> GetFacilityManagerID(int nID)
        {
            List<FacilityManager> listFacilityManager = new List<FacilityManager>();

            foreach (KeyValuePair<int, FacilityManager> item in m_dicFacilityManagers)
            {
                FacilityManager facilityManager = item.Value;

                if (facilityManager.CompanyMember != null && facilityManager.CompanyMember.ID == nID)
                {
                    listFacilityManager.Add(facilityManager);
                }
            }

            return listFacilityManager;
        }

        private bool LoadFacilityMessage(WebDBManager dbMgr, Dictionary<int, FacilityMessage> dicFacilityMessages)
        {
            dicFacilityMessages.Clear();

            string strSQL = string.Format("SELECT ID, FacilityType, SensorID, MessageType, Message FROM FacilityMessage");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            FacilityMessage facilityMessage;

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nSensorID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nMessageType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");

                facilityMessage = new FacilityMessage();
                facilityMessage.ID = nID;
                facilityMessage.FacilityType = (FacilityType)nFacilityType;
                facilityMessage.SensorID = nSensorID;
                facilityMessage.MessageType = (MessageType)nMessageType;
                facilityMessage.Message = strMessage;

                dicFacilityMessages[nID] = facilityMessage;
            }

            return true;
        }

        //public FacilityMessage GetFacilityMessage(FacilityType type, int nSensorID)
        public FacilityMessage GetFacilityMessage(FacilityType type)
        {
            FacilityMessage facilityMessage = new FacilityMessage();

            foreach (KeyValuePair<int, FacilityMessage> item in m_dicFacilityMessages)
            {
                FacilityMessage message = item.Value;

                //if (message.FacilityType == type && message.SensorID == nSensorID)
                if (message.FacilityType == type)
                {
                    facilityMessage = message;
                }
            }

            return facilityMessage;
        }

        public bool InsertFacilityMessage(FacilityMessage message)
        {
            bool bRet = true;

            FacilityType facilityType = message.FacilityType;
            int nSensorID = message.SensorID;
            MessageType messageType = message.MessageType;
            string strMessage = message.Message;

            string szText = "INSERT INTO FacilityMessage (FacilityType, SensorID, MessageType, Message) VALUES({0}, {1}, {2}, '{3}') ";
            string szSQL = string.Format(szText, (int)facilityType, nSensorID, (int)messageType, strMessage);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool UpdateFacilityMessage(FacilityMessage message)
        {
            bool bRet = true;

            int nID = message.ID;
            FacilityType facilityType = message.FacilityType;
            int nSensorID = message.SensorID;
            MessageType messageType = message.MessageType;
            string strMessage = message.Message;

            string strSQL = "UPDATE FacilityMessage SET FacilityType = " + ((int)facilityType).ToString() + ", SensorID = " + nSensorID.ToString() + 
                ", MessageType = " + ((int)messageType).ToString() + ", Message = '" + strMessage + "' WHERE ID = " + nID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
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

        public DataBeforMaxTemp GetBeforMaxTemp(string strSensorID)
        {
            DataBeforMaxTemp beforMaxTemp = new DataBeforMaxTemp();

            string strSQL = string.Format("SELECT MAX(Temperature) FROM HeatData WHERE date(OccurTime) = date(subdate(now(), INTERVAL 1 DAY))" +
                " AND SensorID = '" + strSensorID + "'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                beforMaxTemp.BeforeOneDay = "-";
            else
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    float fTemperature = WebDBManager.GetFloatField(arrResult[i].ToString(), 0);
                    if (fTemperature == 0)
                        beforMaxTemp.BeforeOneDay = "-";
                    else
                        beforMaxTemp.BeforeOneDay = fTemperature.ToString();
                }
            }

            strSQL = string.Format("SELECT MAX(Temperature) FROM HeatData WHERE date(OccurTime) = date(subdate(now(), INTERVAL 2 DAY))" +
                " AND SensorID = '" + strSensorID + "'");
            arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                beforMaxTemp.BeforeTwoDay = "-";
            else
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    float fTemperature = WebDBManager.GetFloatField(arrResult[i].ToString(), 0);
                    if (fTemperature == 0)
                        beforMaxTemp.BeforeTwoDay = "-";
                    else
                        beforMaxTemp.BeforeTwoDay = fTemperature.ToString();
                }
            }

            strSQL = string.Format("SELECT MAX(Temperature) FROM HeatData WHERE date(OccurTime) = date(subdate(now(), INTERVAL 3 DAY))" +
                " AND SensorID = '" + strSensorID + "'");
            arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                beforMaxTemp.BeforeThreeDay = "-";
            else
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    float fTemperature = WebDBManager.GetFloatField(arrResult[i].ToString(), 0);
                    if (fTemperature == 0)
                        beforMaxTemp.BeforeThreeDay = "-";
                    else
                        beforMaxTemp.BeforeThreeDay = fTemperature.ToString();
                }
            }

            strSQL = string.Format("SELECT MAX(Temperature) FROM HeatData WHERE date(OccurTime) = date(subdate(now(), INTERVAL 4 DAY))" +
                " AND SensorID = '" + strSensorID + "'");
            arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                beforMaxTemp.BeforeFourDay = "-";
            else
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    float fTemperature = WebDBManager.GetFloatField(arrResult[i].ToString(), 0);
                    if (fTemperature == 0)
                        beforMaxTemp.BeforeFourDay = "-";
                    else
                        beforMaxTemp.BeforeFourDay = fTemperature.ToString();
                }
            }


            return beforMaxTemp;
        }

        public bool CheckTempName(string strTeamName)
        {
            int nChk = 0;
            bool bRet = false;

            string strSQL = string.Format("SELECT ID, TeamName FROM RegularTeam WHERE TeamName = '" + strTeamName + "'");
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

        public bool InsertRegularteam(DataTeam team)
        {
            bool bRet = true;

            int nID = team.ID;
            string strTeamName = team.TeamName;
            string strParentTeamID = "";

            if (team.ParentTeam != null)
                strParentTeamID = team.ParentTeam.ID.ToString();
            else
                strParentTeamID = "NULL";


            string szText = "INSERT INTO RegularTeam (ID, TeamName, ParentTeamID) VALUES({0}, '{1}', {2})";
            string szSQL = string.Format(szText, nID, strTeamName, strParentTeamID);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool DeleteRegularteam(DataTeam team)
        {
            bool bRet = true;

            int nID = team.ID;

            string szText = "DELETE FROM RegularTeam WHERE ID = {0}";
            string szSQL = string.Format(szText, nID);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool UpdateRegularteam(DataTeam team)
        {
            int nID = team.ID;
            string strTeamName = team.TeamName;
            string strParentTeamID = "";

            if (team.ParentTeam != null)
                strParentTeamID = team.ParentTeam.ID.ToString();
            else
                strParentTeamID = "NULL";

            string strSQL = string.Format("UPDATE RegularTeam SET TeamName = '" + strTeamName + "', ParentTeamID = " + strParentTeamID + "  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool InsertCompanyMember(DataCompanyMember member)
        {
            bool bRet = true;

            int nID = member.ID;
            string strMemberName = member.MemberName;
            int nRegularTeamID = member.Team.ID;

            //int nLevelID = member.Level.ID;
            string strLevelID = "";

            if (member.Level != null)
                strLevelID = member.Level.ID.ToString();
            else
                strLevelID = "-1";

            string strPhoneNumber = member.PhoneNumber;
            //string strFacilityTypes = member.FacilityTypes;

            string szText = "INSERT INTO CompanyMember (ID, MemberName, RegularTeamID, LevelID, PhoneNumber) VALUES({0}, '{1}', {2}, {3}, '{4}')";
            string szSQL = string.Format(szText, nID, strMemberName, nRegularTeamID, strLevelID, strPhoneNumber);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool DeleteCompanyMember(DataCompanyMember member)
        {
            bool bRet = true;

            int nID = member.ID;

            string szText = "DELETE FROM CompanyMember WHERE ID = {0}";
            string szSQL = string.Format(szText, nID);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool UpdateCompanyMember(DataCompanyMember member)
        {
            int nID = member.ID;
            string strMemberName = member.MemberName;
            int nRegularTeamID = member.Team.ID;

            int nLevelID = -1;
            if (member.Level != null)
                nLevelID = member.Level.ID;

            string strPhoneNumber = member.PhoneNumber;
            //string strFacilityTypes = member.FacilityTypes;

            string strSQL = string.Format("UPDATE CompanyMember SET MemberName = '" + strMemberName + "', RegularTeamID = " + nRegularTeamID +
                ", LevelID = " + nLevelID + ", PhoneNumber = '" + strPhoneNumber + "'" +
                "  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        public bool InsertFacilityManual(FacilityType facilityType, string strManualType, string strManualTitle, string strManualMembers, int nNumber, string strManual)
        {
            bool bRet = true;

            int nFacilityType = (int)facilityType;

            string szText = "INSERT INTO FacilityManual (FacilityType, ManualType, ManualTitle, ManualMembers, Number, Manual) VALUES({0}, '{1}', '{2}', '{3}', {4}, '{5}')";
            string szSQL = string.Format(szText, nFacilityType, strManualType, strManualTitle, strManualMembers, nNumber, strManual);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool DeleteFacilityManual(int nID)
        {
            bool bRet = true;

            string szText = "DELETE FROM FacilityManual WHERE ID = {0}";
            string szSQL = string.Format(szText, nID);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool UpdateFacilityManual(int nID, string strManualTitle, string strManualMembers, int nNumber, string strManual, string strRiskLevel)
        {
            string strSQL = string.Format("UPDATE FacilityManual SET ManualTitle = '" + strManualTitle + "', ManualMembers = '" + strManualMembers +
                "', Number = " + nNumber + ", Manual = '" + strManual + "', ManualType = '" + strRiskLevel + "'" +
                "  Where ID = " + nID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            return true;
        }

        private bool LoadFacilityManual(WebDBManager dbMgr, Dictionary<int, FacilityManual> dicFacilityManual)
        {
            dicFacilityManual.Clear();

            string strSQL = string.Format("SELECT ID, FacilityType, ManualType, ManualTitle, ManualMembers, Number, Manual FROM FacilityManual ORDER BY Number ASC");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            FacilityManual facilityManual;

            for (int i = 0; i < nCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                string strManualType = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strManualTitle = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strManualMembers = WebDBManager.GetStringField(arrResult[i + 4], "");
                int nNumber = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                string strManual = WebDBManager.GetStringField(arrResult[i + 6], "");

                facilityManual = new FacilityManual();
                facilityManual.ID = nID;
                facilityManual.FacilityType = (FacilityType)nFacilityType;
                facilityManual.RiskLevel = strManualType;
                facilityManual.Title = strManualTitle;
                facilityManual.Members = strManualMembers;
                facilityManual.Number = nNumber;
                facilityManual.Manual = strManual;

                dicFacilityManual[nID] = facilityManual;
            }

            return true;
        }

        public Dictionary<int, FacilityManual> LoadFacilityRiskLevelManuals(FacilityType facilityType, string strRiskLevel)
        {
            Dictionary<int, FacilityManual> dicFacilityManuals = new Dictionary<int, FacilityManual>();

            foreach (KeyValuePair<int, FacilityManual> pair in m_dicFacilityManual)
            {
                FacilityManual manual = pair.Value;

                if (manual.FacilityType == facilityType && manual.RiskLevel == strRiskLevel)
                    dicFacilityManuals[manual.ID] = manual;
            }

            return dicFacilityManuals;
        }

        public Dictionary<int, FacilityManual> LoadNumberManuals(FacilityType facilityType, string strRiskLevel, int nNumber)
        {
            Dictionary<int, FacilityManual> dicFacilityManuals = new Dictionary<int, FacilityManual>();

            foreach (KeyValuePair<int, FacilityManual> pair in m_dicFacilityManual)
            {
                FacilityManual manual = pair.Value;

                if (manual.FacilityType == facilityType && manual.RiskLevel == strRiskLevel && manual.Number >= nNumber)
                    dicFacilityManuals[manual.ID] = manual;
            }

            return dicFacilityManuals;
        }

        public FacilityManual CheckNumberManuals(FacilityType facilityType, string strRiskLevel, int nNumber, int nID = -1)
        {
            foreach (KeyValuePair<int, FacilityManual> pair in m_dicFacilityManual)
            {
                FacilityManual manual = pair.Value;

                if (nID != -1 && manual.ID != nID && manual.FacilityType == facilityType && manual.RiskLevel == strRiskLevel && manual.Number == nNumber)
                    return manual;
                else if (nID == -1 && manual.FacilityType == facilityType && manual.RiskLevel == strRiskLevel && manual.Number == nNumber)
                    return manual;
            }

            return null;
        }

        public bool InsertSMSSendMessage(List<string> listNumber, string strMessage, FacilityType type)
        {
            bool bRet = true;

            string strNumberList = "";

            foreach (string strNumber in listNumber)
            {
                if (strNumberList == "")
                    strNumberList = strNumber;
                else
                    strNumberList += ", " + strNumber;
            }

            string szText = "INSERT INTO SMSSendMessage (NumberList, Message, FacilityType) VALUES('{0}', '{1}', {2})";
            string szSQL = string.Format(szText, strNumberList, strMessage, (int)type);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool InsertSMSRecord(string strNameList, string strMessage, FacilityType type)
        {
            bool bRet = true;

            string szText = "INSERT INTO SMSRecord (Managers, Message, FacilityType) VALUES('{0}', '{1}', {2})";
            string szSQL = string.Format(szText, strNameList, strMessage, (int)type);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
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

        public AlarmData CheckAlarmData()
        {
            AlarmData alarm = null;

            string strSQL = string.Format("SELECT ID, FacilityType, SensorID, RiskLevel, Address, CreateTime FROM AlertAlarm WHERE IsCheck = 0 ORDER BY CreateTime DESC LIMIT 1");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return null;

            int nCount = arrResult.Count;
            if (nCount == 0) return null;

            DateTime dtDefault = new DateTime();

            int nID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            int nFacilityType = WebDBManager.GetIntField(arrResult[1].ToString(), 0);
            int nSensorID = WebDBManager.GetIntField(arrResult[2].ToString(), 0);
            string strRiskLevel = WebDBManager.GetStringField(arrResult[3], "");
            string strAddress = WebDBManager.GetStringField(arrResult[4], "");
            DateTime dtCreateTime = WebDBManager.GetDateTimeField(arrResult[5], dtDefault);

            alarm = new AlarmData();
            alarm.ID = nID;
            alarm.FacilityType = (FacilityType)nFacilityType;
            alarm.SersorID = nSensorID;
            alarm.RiskLevel = strRiskLevel;
            alarm.Address = strAddress;
            alarm.CreateTime = dtCreateTime;

            return alarm;
        }

        public bool ConfirmAlertAarm(int nSensorID, int nFacilityType)
        {
            bool bRet = true;

            string szText = "UPDATE AlertAlarm SET IsCheck = 1 WHERE SensorID = " + nSensorID + " AND IsCheck = 0 AND FacilityType = " + nFacilityType;
            string szSQL = string.Format(szText, nSensorID);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        public bool LoadAlarms(WebDBManager dbMgr, Dictionary<FacilityType, Dictionary<int, AlarmData>> dicFacilityAlarms)
        {
            dicFacilityAlarms.Clear();

            string strSQL = string.Format("SELECT ID, FacilityType, SensorID, RiskLevel, Address, IsCheck, CreateTime FROM AlertAlarm Where IsCheck = 0");
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DateTime dtDefault = new DateTime();
            AlarmData data;

            for (int i = 0; i < nCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nSensorID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                string strRiskLevel = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strAddress = WebDBManager.GetStringField(arrResult[i + 4], "");
                int nIsCheck = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                DateTime dtCreateTime = WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);

                data = new AlarmData();
                data.ID = nID;
                data.FacilityType = (FacilityType)nFacilityType;
                data.SersorID = nSensorID;
                data.RiskLevel = strRiskLevel;
                data.Address = strAddress;

                if (nIsCheck == 1) data.Check = true;
                else data.Check = false;

                data.CreateTime = dtCreateTime;

                // 타입에 따른 알람 데이터 분류 저장
                if (dicFacilityAlarms.ContainsKey((FacilityType)nFacilityType))
                {
                    Dictionary<int, AlarmData> dicAlarms = dicFacilityAlarms[(FacilityType)nFacilityType];
                    dicAlarms[nID] = data;
                }
                else
                {
                    dicFacilityAlarms[(FacilityType)nFacilityType] = new Dictionary<int, AlarmData>();
                    Dictionary<int, AlarmData> dicAlarms = dicFacilityAlarms[(FacilityType)nFacilityType];
                    dicAlarms[nID] = data;
                }



            }

            return true;
        }

        public Dictionary<int, AlarmData> LoadFacilityAlarms(FacilityType facilityType)
        {
            Dictionary<int, AlarmData> dicFacilityAlarms = new Dictionary<int, AlarmData>();

            if (m_dicFacilityAlarms.ContainsKey(facilityType))
                dicFacilityAlarms = m_dicFacilityAlarms[facilityType];

            return dicFacilityAlarms;
        }

        public int GetAlarmCount()
        {
            int nAlarmCount = 0;

            string strSQL = string.Format("SELECT Count(*) FROM alertalarm Where IsCheck = 0");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return -1;

            int nCount = arrResult.Count;
            if (nCount == 0) return 0;

            for (int i = 0; i < nCount; i++)
            {
                nAlarmCount = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
            }

            return nAlarmCount;
        }

    }

    
}
