using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using DBUtility;

namespace JubixNetwork
{
    public enum PipeStatus
    {
        NoSignal = 0,
        Working = 1,
        Stop = 2,
        Leak = 3,
        Overload = 4,
        None
    }

    public class PipeSensor
    {        
        private int m_nPipeID;

        public int PipeID
        {
            get { return m_nPipeID; }
            set { m_nPipeID = value; }
        }

        private float m_fNoramlValueUnder = 0.0f;
        public float NoramlValueUnder
        {
            get { return m_fNoramlValueUnder; }
            set { m_fNoramlValueUnder = value; }
        }

        private float m_fNormalValueUpper = 0.0f;
        public float NormalValueUpper
        {
            get { return m_fNormalValueUpper; }
            set { m_fNormalValueUpper = value; }
        }
        
        private float m_nVariationValue = 1000f;

        private float m_fPrevValue = 0.0f;

        public float PrevValue
        {
            get { return m_fPrevValue; }
            set { m_fPrevValue = value; }
        }

        private Queue<float> variationValues = new Queue<float>();
        private Queue<bool> variationCheckValues = new Queue<bool>();
        private Queue<float> pressureValues = new Queue<float>();


        private int m_nStatus = -1;
        public int Status
        {
            get { return m_nStatus; }
            set { m_nStatus = value; }
        }

        private string m_szPipeName = "";
        public string PipeName
        {
            get { return m_szPipeName; }
            set { m_szPipeName = value; }
        }

        private bool m_bAutoWorkStart = false;
        public bool AutoWorkStart
        {
            get { return m_bAutoWorkStart; }
            set { m_bAutoWorkStart = value; }
        }
        
        /// <summary>
        /// 작업중 인지 여부
        /// </summary>
        private bool m_bIsWorking = false;
        public bool Working
        {
            get { return m_bIsWorking; }
            set { m_bIsWorking = value; }
        }
        /// <summary>
        /// 현재 센서 입력값
        /// </summary>
        private float m_fCurrentValue = 0.0f;
        public float CurrentValue
        {
            get { return m_fCurrentValue; }
        }
        
        private int m_nRecentWorkID = -1;
        private DateTime m_dtRecentBeginTime;
        private int nRecentAvgCnt = 0;
        private float fRecentMaxPressure;
        private float fRecentAvgPressure;
        private float fRecentMinPressure;
        private float fRecentMaxPressureGap;
        private float fSumPressure;

        /// <summary>
        /// 작업이 시작되었는지 여부
        /// </summary>
        private bool m_bStartWorking = false;

        /// <summary>
        /// 변화량 한계값
        /// </summary>
        private float m_fBiasValue = 1.0f;

        public float BiasValue
        {
          get { return m_fBiasValue; }
          set { m_fBiasValue = value; }
        }
        private float m_fBiasValue2 = 1.0f;
        public float BiasValue2
        {
          get { return m_fBiasValue2; }
          set { m_fBiasValue2 = value; }
        }

        private int m_nBiasTime = 14;
        public int BiasTime 
        {
            get { return m_nBiasTime; }
            set { m_nBiasTime = value; } 
        }

        private int m_nCheckTime = 5;
        public int CheckTime
        {
            get { return m_nCheckTime; }
            set { m_nCheckTime = value; }
        }

        // 현재 발생한 알람을 포함하여 가장 마지막에 발생한 알람
        private PipeAlarm m_lastAlarm = null;
        public PipeAlarm LastAlarm
        {
            get { return m_lastAlarm; }
            set { m_lastAlarm = value; }
        }

        public void ReadHistory()
        {
            m_nWorkHistoyrID = ReadWorkHistory();
            if( m_nWorkHistoyrID > 0)
            {
                nWorkDataCount = 0;


                m_fPrevValue = 0.0f;
                variationValues.Clear();

              

                ReadRecentData();

                m_bIsWorking = true;
                m_bStartWorking = false;
            }
        }

        // bias 값 이내인지 검사
        private bool CheckVariation()
        {
            bool bResult = false;
            int hitCount = 0;

            foreach (bool fValue in variationCheckValues)
            {                
                if (fValue == false)
                {
                    hitCount = 0;
                    bResult = false;
                    break;
                }
                else
                {
                    hitCount++;
                    if (hitCount >= m_nCheckTime)
                    {
                        bResult = true;
                      
                    }
                }
            }
            return bResult;
        }

        private bool CheckWorkingEnd()
        {
            if (m_bAutoWorkStart == false)
                return false;

            if (m_bIsWorking == false)
                return false;

            int nCount = m_nBiasTime / 2 - 1;
            if (pressureValues.Count < nCount)
            {
                return false;
            }

            float fMin = 99999.0f;
            float fMax = -99999.0f;
            float fMaxLast = -99999.0f;
            float fMinLast = 99999.0f;

            int nIdxMin = -1 ;
            int nIdxMinLast = -1;
            int nIdxMax = -1;
            int nIdxMaxLast = -1;

            int nIdx = 0;
            foreach (float fValue in pressureValues)
            {
                if (fMin >= fValue)
                {
                    fMinLast = fMin;
                    fMin = fValue;

                    nIdxMinLast = nIdxMin;
                    nIdxMin = nIdx;
                }

                if (fMax <= fValue)
                {
                    fMaxLast = fMax;
                    fMax = fValue;

                    nIdxMaxLast = nIdxMax;
                    nIdxMax = nIdx;
                }

                nIdx++;
            }

            if (fMinLast == 99999.0f)
            {
                fMinLast = 0.0f;
            }

            if (fMaxLast == -99999.0f)
            {
                fMaxLast = 0.0f;
            }

            float fValue2 = fMaxLast - fMinLast;

            if (fValue2 > m_fBiasValue2)
            {
                if (nIdxMinLast > nIdxMaxLast)
                    return true;
            }
            return false;
        }

        private bool CheckWorkingStart()
        {
            if (m_bAutoWorkStart == false)
                return false;

            m_bStartWorking = false;

            int nCount = m_nBiasTime / 2 - 1;
            if (pressureValues.Count < nCount)
            {
                  return m_bStartWorking;
            }

            float fMin = 99999.0f;
            float fMax = -99999.0f;
            float fMaxLast = -99999.0f;
            float fMinLast = 99999.0f;

            int nIdxMin = -1;
            int nIdxMinLast = -1;
            int nIdxMax = -1;
            int nIdxMaxLast = -1;

            int nIdx = 0;
            
            foreach (float fValue in pressureValues)
            {
                if (fMin >= fValue)
                {
                    fMinLast = fMin;
                    fMin = fValue;

                    nIdxMinLast = nIdxMin;
                    nIdxMin = nIdx;
                }

                if (fMax <= fValue)
                {
                    fMaxLast = fMax;
                    fMax = fValue;

                    nIdxMaxLast = nIdxMax;
                    nIdxMax = nIdx;
                }
            }

            if (fMinLast == 99999.0f)
            {
                fMinLast = 0.0f;
            }

            if (fMaxLast == -99999.0f)
            {
                fMaxLast = 0.0f;
            }

            float fValue2 = fMaxLast - fMinLast;
            
            if (fValue2 > m_fBiasValue2)
            {
                if(nIdxMaxLast > nIdxMinLast)
                    m_bStartWorking = true;
            }

            return m_bStartWorking;
        }
        
        public void SetSensorValue(float fValue)
        {
            m_fPrevValue = m_fCurrentValue;
            m_fCurrentValue = fValue / 100.0f;
            
            System.Diagnostics.Trace.WriteLine("Set Pressure " + m_fCurrentValue);

            pressureValues.Enqueue(m_fCurrentValue);
            int nCount = m_nBiasTime / 2;
            if (pressureValues.Count > nCount)
            {
                pressureValues.Dequeue();
            }

            bool bCheckVariation = false;
            if (m_fPrevValue == 0.0f)
            {
                m_nVariationValue = 0.0f;
            }
            else
            {
                float fCheckValue = m_fPrevValue * m_fBiasValue / 100;
                // calc variation
                m_nVariationValue = m_fCurrentValue - m_fPrevValue;

                if (m_fCurrentValue > (fCheckValue + m_fPrevValue))
                {
                    bCheckVariation = true;
                }
                else
                {
                    bCheckVariation = false;
                }
            }
        }
        
        int nCalcCount = 0;
        int nWorkDataCount = 0;
        public void SetCurrentPressure(float value)
        {
            m_fCurrentValue = value / 100.0f;

            pressureValues.Enqueue(m_fCurrentValue);
            int nCount = m_nBiasTime / 2;
            if (pressureValues.Count > nCount)
            {
                pressureValues.Dequeue();
            }

            if (m_fCurrentValue > 0.0f)
            {
                System.Diagnostics.Trace.WriteLine("SensorValue : " + value.ToString());
                bool bCheckVariation = true;

                float fCheckValue = 0.0f;

                if (m_fPrevValue == 0.0f)
                {
                    m_nVariationValue = 0.0f;
                    bCheckVariation = true;
                    fCheckValue = m_fCurrentValue * m_fBiasValue / 100;
                }
                else
                {

                    fCheckValue = m_fPrevValue * m_fBiasValue / 100;
                    // calc variation
                    m_nVariationValue = m_fCurrentValue - m_fPrevValue;

                    if (m_fCurrentValue > (fCheckValue + m_fPrevValue))
                    {
                        bCheckVariation = true;
                    }
                    else
                    {
                        bCheckVariation = false;
                    }
                }


                variationCheckValues.Enqueue(bCheckVariation);
                variationValues.Enqueue(m_nVariationValue);
                
                if (variationValues.Count > nCount)
                {
                    variationValues.Dequeue();
                    variationCheckValues.Dequeue();
                } 
               
                nCalcCount++;

                if (Math.Abs(m_nVariationValue) <= fCheckValue)
                {

                    if (m_bIsWorking == false)
                    {
                        if (m_bStartWorking == true)
                        {
                            if (CheckVariation() == true)
                            {
                                if (m_bAutoWorkStart == true)
                                {
                                    
                                    BeginWork(-1);
                                    UpdateRecentData();
                                }
                               
                            }
                        }
                        else
                        {
                            CheckWorkingStart();
                        }
                    }
                    else
                    {
                        UpdateRecentData();
                        // save workhistory                        
                    }
                }
                else
                {                   
                    if (m_bIsWorking == false)
                    {
                        // check work start
                        CheckWorkingStart();
                    }
                    else
                    {

                        UpdateRecentData();

                        // 알람이 이미 발생한 상태일 경우 새로운 알람을 발생시키지 않는다.
                        if (m_lastAlarm == null || m_lastAlarm.EndTime != null)
                        {

                            if(m_nVariationValue > 0)
                            {
                                m_nStatus = 4;
                            }
                            else
                            {
                                m_nStatus = 3;
                            }

                            // Create alarm
                            bool bAlarmIgnore = CheckAlarmIgnore();
                            SaveAlarmHistory(bAlarmIgnore);

                            if (bAlarmIgnore == false)
                            {
                                SaveSirenOnCommand();
                                SendSMS();
                            }                            
                        }
                    }               
                }

                if(m_bIsWorking == true)
                {
                    DateTime dtNow = DateTime.Now;
                    TimeSpan span = dtNow - m_dtRecentBeginTime;
                    if( span.TotalHours > 48)
                    {
                        if( m_bAutoWorkStart == true)
                        {
                            nWorkDataCount = 0;
                            DoneWork(-1);
                        }
                       
                    }

                    nWorkDataCount++;
                    if (CheckWorkingEnd() && nWorkDataCount > 14)
                    {
                        if (m_lastAlarm == null || m_lastAlarm.EndTime != null)
                        {
                            m_nStatus = 3;

                            UpdateRecentData();
                            bool bAlarmIgnore = CheckAlarmIgnore();
                            SaveAlarmHistory(bAlarmIgnore);

                            if (bAlarmIgnore == false)
                            {
                                SaveSirenOnCommand();
                                SendSMS();
                            }
                        }

                        if (m_bAutoWorkStart == true)
                        {
                            nWorkDataCount = 0;
                            DoneWork(-1);

                        }
                    }
                }
              
                // save current value to mesure value
                SavePipePressure();
                SavePipePressureHistory();
            }
            else if (m_fCurrentValue <= 0.0f)
            {
                m_nStatus = (int)PipeStatus.Stop;

                m_bStartWorking = false;

                SavePipePressure();
                SavePipePressureHistory();

                if( m_bIsWorking == true)
                {
                    if( m_bAutoWorkStart == true)
                    {
                        //SavePipePressure();
                        nWorkDataCount = 0;
                        DoneWork(-1);
                    }                   
                }
            }
            m_fPrevValue = m_fCurrentValue;
        }

        private void SaveSirenOnCommand()
        {
            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
            DateTime dtnow = DateTime.Now;
            string szDT = WebDBManager.MakeDateTimeString(dtnow);

            int nCmdID = GetMaxID("command", dbManager) + 1;
            string szTemp2 = "INSERT INTO command (ID, CommandType, TimeStamp, PipeID, UserID) VALUES ({0}, 1, '{1}',{2}, 1) ";
            string szSQL1 = string.Format(szTemp2, nCmdID, szDT, m_nPipeID);
            dbManager.GetResultData(szSQL1, 0);

            int nMaxID = GetMaxID("commandhistory", dbManager) + 1;
            string szTemp1 = "INSERT INTO commandhistory (ID, CommandType,CommandMakeTime, UserID, CmdID) VALUES ( {0}, 1, '{1}', 1, {2} )";
            string szSQL2 = string.Format(szTemp1, nMaxID, szDT, nCmdID);
            dbManager.GetResultData(szSQL2, 0);
        }

        private void EndCollback(IAsyncResult ar)
        {
            try
            {
                ((Action)ar.AsyncState).EndInvoke(ar);
            }
            catch (Exception)
            {
                // 종료되었는지 검사
            }
        }

        private string MakeMessage(int nStatus)
        {
            string szMsg = "배관[{0}] 에서 {1}이 감지 되었습니다.";
            string szResult = string.Format(szMsg, m_szPipeName, nStatus == (int)PipeStatus.Leak ? "압력 하강" : "압력 상승");
            return szResult;
        }

        private void SendSMS()
        {
            if (UseSMS() == false)
                return;

            DataManager mgr = new DataManager(JubixSensorManager.Instance.DBManager, JubixSensorManager.Instance.SiteID);
            List<string> phoneNumbers = mgr.GetFacilityManagerPhoneNumberList();

            if (phoneNumbers == null || phoneNumbers.Count == 0)
                return;

            int status = m_nStatus;
            string strMsg = MakeMessage(status);

            string szSendPhoneNumber = GetSendPhoneNumber();
            foreach (string strPhoneNumber in phoneNumbers)
            {
                libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(500, "127.0.0.1");
                client.SendSMS(szSendPhoneNumber, strPhoneNumber, strMsg);             
            }
        }

        private string szCaller = "0522676652";
        private string GetSendPhoneNumber()
        {
            string strSQL = "Select PropertyValue from Options where PropertyName = 'SmsCaller' and SiteID = " + JubixSensorManager.Instance.SiteID.ToString();
            ArrayList arrResult = JubixSensorManager.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return szCaller;

            string strValue = WebDBManager.GetStringField(arrResult[0]).Trim();
            if (strValue == null || strValue == "")
                return szCaller;

            return strValue;
        }

        private bool UseSMS()
        {
            string strSQL = "Select PropertyValue from Options where PropertyName = 'UseSMS' and SiteID = " + JubixSensorManager.Instance.SiteID.ToString();
            ArrayList arrResult = JubixSensorManager.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strValue = WebDBManager.GetStringField(arrResult[0]).Trim();

            if (strValue == "0")
                return false;
            else if (strValue == "1")
                return true;
            else if (string.Compare(strValue, "true", true) == 0)
                return true;
            else if (string.Compare(strValue, "false", true) == 0)
                return false;

            return false;
        }

        private int GetMaxID(string strTableName, WebDBManager dbMgr)
        {
            string strSQL = "select MAX(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }
        public string GetKeyTimeString(DateTime dt)
        {
            string szResult = string.Format("{0}{1:D2}{2:D2}{3:D2}", dt.Day, dt.Hour, dt.Minute, dt.Second);
            return szResult;
        }
        public void SavePipePressureHistory()
        {
#if !DB_LOG
            float value = m_fCurrentValue;
            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;

            DateTime dtNow = DateTime.Now;
            string szTableName = TablePartition.GetTableNames(m_nPipeID, dtNow);
            int nMaxID = GetMaxID(szTableName, dbManager) + 1;
            string szDate = WebDBManager.MakeDateTimeString(dtNow);
            
            string szKeyTime = GetKeyTimeString(dtNow);
            string szTemp2 = "INSERT "+ szTableName+ " ( ID, TimeStamp, Pressure, KeyTime) VALUES ({0},'{1}',{2},'{3}')";
            string szSQL2 = string.Format(szTemp2, nMaxID, szDate, value, szKeyTime);
            dbManager.GetResultData(szSQL2, 0);

            // 기존 테이블에도 넣어준다. 
            SaveOrgPipePressureHistory();
#endif
        }

        public void SaveOrgPipePressureHistory()
        {
            float value = m_fCurrentValue;
            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
          
            DateTime dtNow = DateTime.Now;           
            int nMaxID = GetMaxID("PipeHistory", dbManager) + 1;
            string szDate = WebDBManager.MakeDateTimeString(dtNow);
            string szKeyTime = GetKeyTimeString(dtNow);
            string szTemp2 = "INSERT PipeHistory ( ID, PipeID, TimeStamp, Pressure, KeyTime) VALUES ({0},{1},'{2}',{3}, '{4}')";
            string szSQL2 = string.Format(szTemp2, nMaxID, m_nPipeID, szDate, value, szKeyTime);

            dbManager.GetResultData(szSQL2, 0);
        }

        public void SavePipePressure()
        {
#if !DB_LOG
            float value = m_fCurrentValue;

            // save current value
            string szTemp = "UPDATE Pipe SET Pressure = {0},PrevPressure = {1}, Status = {2} WHERE ID = {3}";
            string szSQL = string.Format(szTemp, value,m_fPrevValue, (int)m_nStatus, m_nPipeID);

            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
            dbManager.GetResultData(szSQL, 0);
#endif
        }

        private void SaveAlarmHistory(bool bAlarmIgnore)
        {

            if (bAlarmIgnore == false && m_bIsWorking == true)
            {
                WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
                int nMaxID = GetMaxID("PipeAlarmHistory", dbManager) + 1;

                DateTime dtNow = DateTime.Now;
                string szDate = WebDBManager.MakeDateTimeString(dtNow);
                // Save  alarm history
                string szTemp = "INSERT INTO PipeAlarmHistory (ID, PipeID , BeginTime , EndTime , AlarmPressure , Status, AlarmTerminator, NormalRange, StandardPressure ) " +
                " VALUES ( {0}, {1}, '{2}', NULL, {3}, {4}, NULL, {5}, {6})";

                string szSQL = string.Format(szTemp, nMaxID, m_nPipeID, szDate, m_fCurrentValue, (int)m_nStatus, m_fBiasValue, m_fPrevValue);
                dbManager.GetResultData(szSQL, 0);

                //m//_lastAlarm = new PipeAlarm();
                //m_lastAlarm.BeginTime = dtNow;
                //m_lastAlarm.Pressure = m_fCurrentValue;
                //m_lastAlarm.Status = (PipeStatus)m_nStatus;
            
                szTemp = "UPDATE RecentAlarmHistory SET AlarmHistoryID = {0} WHERE PipeID = {1}";
                szSQL = string.Format(szTemp, nMaxID, m_nPipeID);
                dbManager.GetResultData(szSQL, 0);
            }
        }

        private VariousData<DateTime> m_LastAlarmTime = new VariousData<DateTime>();
        
        private int nAlarmBetween = 30;

        public int AlarmBetween
        {
          get { return nAlarmBetween; }
          set { nAlarmBetween = value; }
        }

        private bool CheckAlarmIgnore()
        {
            DateTime dt = DateTime.Now;
            if( m_LastAlarmTime.Data != null)
            {                
                TimeSpan sp2 = dt - m_LastAlarmTime.Data;
                if( sp2.TotalSeconds <= nAlarmBetween)
                {
                    return true;
                }
            }

            // TargetID == ID 레코드가 있는경우
            // 검색조건 ignoreBeginTime + IgnoreTime 이 현재보다 이후

            string szSQL = "SELECT ID, IgnoreBeginTime, IgnoreTime FROM alarmignore where TargetType = 0 and TargetID = " + m_nPipeID.ToString();
            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
            ArrayList arResult = dbManager.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count < 3)
                return false;

            int nID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
            VariousData<DateTime> dtData = WebDBManager.GetDateTimeField(arResult[1]);
            int nMin = WebDBManager.GetIntField(arResult[2].ToString(), 0);

            if( dtData == null || dtData.Data == null)
            {
                return false;
            }

            TimeSpan sp = dt - dtData.Data;
            if( sp.TotalMinutes <= nMin)
            {
                return true;
            }
            return false;
        }    
 
        public void SetClearTime(DateTime dt)
        {
            m_LastAlarmTime.Data = dt;
        }
       
        private void ClearAlarm()
        {
            SetClearTime(DateTime.Now);

            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
            string szTemp = "UPDATE recentalarmhistory SET AlarmHistoryID = NULL WHERE PipeID = {0}";
            string szSQL = string.Format(szTemp, m_nPipeID);
            dbManager.GetResultData(szSQL, 0);

            string szEndDate = WebDBManager.MakeDateTimeString(DateTime.Now);
            szTemp = "UPDATE pipealarmhistory SET EndTime= '{0}', AlarmTerminator={1} WHERE PipeID = {2} AND EndTime is NULL";
            szSQL = string.Format(szTemp, szEndDate, 1, m_nPipeID);
            dbManager.GetResultData(szSQL, 0);

        }

        private int GetRecentWorkDataID()
        {            
            string szSQL = "SELECT ID FROM RecentPipeHistory WHERE PipeID = " + m_nPipeID;

            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
            ArrayList arResult = dbManager.GetResultData(szSQL, 0);

            int nRecentID = -1;
            if (arResult != null && arResult.Count > 0)
            {
                nRecentID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
                m_nRecentWorkID = nRecentID;
            }
            return nRecentID;
        }
        
        private int SaveWorkHistory(int nCmdHistoryID)
        {

            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
            int nMaxID = GetMaxID("PipeWorkHistory", dbManager) + 1;
          
            string szBeginDate = WebDBManager.MakeDateTimeString(m_dtRecentBeginTime); 

            // Save Pipeworkhistory
            string szTemp = "INSERT INTO PipeWorkHistory (ID,PipeID,BeginTime,EndTime,AvgPressure,MinPressure,MaxPressure,AvgCnt, BeginCmdHistoryID, EndCmdHistoryID) " +
                            " VALUES ( {0}, {1}, '{2}', NULL, {3}, {4}, {5}, {6}, {7}, -2)";

            string szSQL = string.Format(szTemp, nMaxID, m_nPipeID, szBeginDate, fRecentAvgPressure, fRecentMinPressure, fRecentMaxPressure, nRecentAvgCnt, nCmdHistoryID);
            dbManager.GetResultData(szSQL, 0);

            return nMaxID;

        }

        private int ReadWorkHistory()
        {
            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
            
            string szTemp = "SELECT ID FROM kpx.pipeworkhistory where EndTime is NULL and PipeID = {0} order by BeginTime DESC limit 1";

            string szSQL = string.Format(szTemp, m_nPipeID);
         
            ArrayList arResult = dbManager.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count == 0)
                return -1;

            int nHistoryID = WebDBManager.GetIntField(arResult[0].ToString(), -1);

            return nHistoryID;
        }

        public void UpdateWorkHistory(int nCmdHistoryID)
        {
            //if(m_nWorkHistoyrID > 0)
            {
                string szEndDate = WebDBManager.MakeDateTimeString(DateTime.Now);
                string szTemp = "UPDATE PipeWorkHistory SET EndTime = '{0}', AvgPressure = {1}, MinPressure = {2}, MaxPressure = {3}, AvgCnt = {4}, EndCmdHistoryID={5} WHERE EndTime is NULL and PipeID = {6}";
                string szSQL = string.Format(szTemp, szEndDate, fRecentAvgPressure, fRecentMinPressure, fRecentMaxPressure, nRecentAvgCnt,nCmdHistoryID, m_nPipeID);

                WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
                dbManager.GetResultData(szSQL, 0);
            }  
        }

        public void ReadRecentData()
        {
            WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
            string szTemp = "SELECT ID, RecentBeginTime, RecentMaxPressure, RecentAvgPressure, RecentAvgCount, RecentMaxPressureGap WHERE PipeID = {0}";
            string szSQL = string.Format(szTemp, m_nPipeID);
            ArrayList arResult = dbManager.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                m_nRecentWorkID = WebDBManager.GetIntField(arResult[0].ToString() , -1);
                m_dtRecentBeginTime = WebDBManager.GetDateTimeField(arResult[1], DateTime.Now);

                fRecentMaxPressure = WebDBManager.GetFloatField(arResult[2].ToString(), -99999.0f);
                fRecentAvgPressure = WebDBManager.GetFloatField(arResult[3].ToString(), 0.0f);
                fRecentMinPressure = WebDBManager.GetFloatField(arResult[4].ToString(), 99999.0f);
                fRecentMaxPressureGap = WebDBManager.GetFloatField(arResult[5].ToString(), 0.0f);
                nRecentAvgCnt = WebDBManager.GetIntField(arResult[6].ToString(), 0);
            }
        }

        public void BeginRecentData()
        {
            if (m_nRecentWorkID > 0)
            {
                string szBeginDate = WebDBManager.MakeDateTimeString(m_dtRecentBeginTime);

                string szTemp = "UPDATE RecentPipeHistory SET RecentBeginTime = '{0}', RecentMaxPressure = 0, RecentAvgPressure = 0, RecentAvgCount = 0, RecentMaxPressureGap = 0 WHERE ID = {2}";
                string szSQL = string.Format(szTemp, szBeginDate, (int)m_nStatus, m_nRecentWorkID);

                WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
                dbManager.GetResultData(szSQL, 0);
            }
        }

        public void UpdateRecentData()
        {
            if( m_nRecentWorkID > 0)
            {                
                // calc recent data
                nRecentAvgCnt++;

                fSumPressure += m_fCurrentValue;

                fRecentAvgPressure = fSumPressure / nRecentAvgCnt;

                if (m_fCurrentValue > fRecentMaxPressure)
                    fRecentMaxPressure = m_fCurrentValue;

                if (m_fCurrentValue < fRecentMinPressure)
                    fRecentMinPressure = m_fCurrentValue;

                fRecentMaxPressureGap = fRecentMaxPressure - fRecentMinPressure;

                // save db
                string szBeginDate = WebDBManager.MakeDateTimeString(m_dtRecentBeginTime);

                string szTemp = "UPDATE RecentPipeHistory SET RecentMaxPressure = {0}, RecentAvgPressure = {1}, RecentAvgCount = {2}, RecentMaxPressureGap = {3} WHERE ID = {4}";
                string szSQL = string.Format(szTemp, fRecentMaxPressure, fRecentAvgPressure, nRecentAvgCnt, fRecentMaxPressureGap, m_nRecentWorkID);

                WebDBManager dbManager = JubixSensorManager.Instance.DBManager;
                dbManager.GetResultData(szSQL, 0);
               
            }
        }
        
        public void BeginWork(int nCmdHistoryID)
        {
            nWorkDataCount = 0;

        
            m_fPrevValue = 0.0f;
            variationValues.Clear();
            variationCheckValues.Clear();
            pressureValues.Clear();
            

            nRecentAvgCnt = 0;
            fRecentMaxPressure = -99999.0f;
            fRecentAvgPressure = 0.0f;
            fRecentMinPressure = 99999.0f;
            fRecentMaxPressureGap = 0.0f;
            fSumPressure = 0.0f;

            m_dtRecentBeginTime = DateTime.Now;
            m_nRecentWorkID = GetRecentWorkDataID();

            BeginRecentData();

            m_nWorkHistoyrID = SaveWorkHistory(nCmdHistoryID);

            m_bIsWorking = true;
            m_bStartWorking = false;
        }

        private int m_nWorkHistoyrID = -1;

        public void DoneWork(int nCmdHistoryID)
        {
            m_bIsWorking = false;
            m_bStartWorking = false;

            m_nRecentWorkID = -1;

            m_fPrevValue = 0.0f;
            variationValues.Clear();
            variationCheckValues.Clear();
            pressureValues.Clear();

            //if (m_nWorkHistoyrID > 0)
            UpdateWorkHistory(nCmdHistoryID);

            if( m_lastAlarm != null)
            {
                if(m_bAutoWorkStart == false)
                {
                    ClearAlarm();
                }
            }
            m_lastAlarm = null;


            m_nWorkHistoyrID = -1;
        }
    }

    public class PipeAlarm
    {
        private DateTime m_dtBegin = new DateTime();
        private VariousData<DateTime> m_dtEnd = null;
        private PipeStatus m_status = PipeStatus.None;
        private double m_pressure = -1;

        public DateTime BeginTime
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        public VariousData<DateTime> EndTime
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
        }

        public PipeStatus Status
        {
            get { return m_status; }
            set { m_status = value; }
        }

        public double Pressure
        {
            get { return m_pressure; }
            set { m_pressure = value; }
        }
    }
}
