using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using JubixNetwork;
using DBUtility;

namespace PSensorServer
{
    public class KPXAlarmManager : IDisposable
    {
        private List<AlarmInfo> m_AlarmList = new List<AlarmInfo>();

        // TankID, AlarmInfo
        private SortedList<long, AlarmInfo> m_dicAlarms = new SortedList<long, AlarmInfo>();
        // AlarmHistoryID, AlarmInfo
        private SortedList<int, AlarmInfo> m_dicHistoryAlarms = new SortedList<int, AlarmInfo>();
        
        private WebDBManager m_dbMgr = null;
        
        // <<TankID, PipeID>/ClearTime> : 마지막 알람 시간을 기록해 둔다.
        private SortedList<long, DateTime> m_AlarmClearTime = new SortedList<long, DateTime>();
               
        private bool m_bExitThred = false;
        private Thread m_AlarmIgnoreCheckThred = null;
        
        // Alarm Ignore를 TankID별로 둔다.
        private ConcurrentDictionary<int, AlarmIgnore> m_Ignores = new ConcurrentDictionary<int, AlarmIgnore>();

        DateTime m_dtBegin;

        public KPXAlarmManager()
        {
            m_dtBegin = DateTime.Now;
            m_dbMgr = KPXServerManager.Instance.DBManager;
            System.Diagnostics.Trace.WriteLine("Create KPXAlarmManager");
        }

        public void BeginThread()
        {
            if (m_AlarmIgnoreCheckThred == null)
            {
                m_bExitThred = false;
                m_AlarmIgnoreCheckThred = new Thread(CheckAlarmIgnore);
                m_AlarmIgnoreCheckThred.Name = "AlarmIgonreCheckThread";
                m_AlarmIgnoreCheckThred.Start();
            }

            ReadAllLastAlarm();
        }

        public void ReleaseThread()
        {
            m_bExitThred = true;

            if (m_AlarmIgnoreCheckThred != null)
            {
                try
                {                    
                    m_AlarmIgnoreCheckThred.Join();
                    m_AlarmIgnoreCheckThred = null;
                }
                catch (Exception)
                {
                }
            }
        }

        public void Dispose()
        {
            ReleaseThread();
        }


        internal bool AddAlarm(AlarmInfo alarmInfo)
        {
            long key = DBUtil.ToLong(alarmInfo.TankID, alarmInfo.PipeID, alarmInfo.AlarmType);
            if (m_dicAlarms.ContainsKey(key))
                return false;

            // 서버 시작 100초 이내의 알람은 처리하지 않는다.
            DateTime dtNow = DateTime.Now;
            TimeSpan span = dtNow - m_dtBegin;
            if( span.TotalSeconds < 100)
            {
                return false;
            }

            if (alarmInfo.AlarmType == (int)AlarmType.압력상승 || alarmInfo.AlarmType == (int)AlarmType.압력하강
                         || alarmInfo.AlarmType == (int)AlarmType.탱크배관유량감소 || alarmInfo.AlarmType == (int)AlarmType.탱크배관유량증가)
            {
                // 기준값이 0인경우는 처리하지 않느다.
                if (alarmInfo.StandardValue == 0.0f)
                {
                    return false;
                }


                // 범위값이 0인경우에는 처리하지 않는다.
                if (alarmInfo.StandardRange == 0.0f)
                {
                    return false;
                }
            }
            

            m_dicAlarms.Add(key, alarmInfo);

            m_AlarmList.Add(alarmInfo);
           
            SaveAlarmHistory(alarmInfo);

            m_dicHistoryAlarms.Add(alarmInfo.AlarmHistoryID, alarmInfo);

            SaveAlarmRecentHistory(alarmInfo);
            
            //if (alarmInfo.AlarmType == (int)AlarmType.압력상승 || alarmInfo.AlarmType == (int)AlarmType.압력하강
            //             || alarmInfo.AlarmType == (int)AlarmType.탱크배관유량감소 || alarmInfo.AlarmType == (int)AlarmType.탱크배관유량증가)
            {
                SaveSirenOnCommand(alarmInfo.TankID, alarmInfo.PipeID, alarmInfo.AlarmHistoryID);
            }            

            return true;
        }

        private void SaveSirenOffCommand()
        {
            DateTime dtnow = DateTime.Now;
            string szDT = WebDBManager.MakeDateTimeString(dtnow);

            int nCmdID = DBUtil.GetMaxID("command", m_dbMgr) + 1;
            string szTemp2 = "INSERT INTO command (ID,CommandType,TimeStamp,PipeID,TankID,UserID,CommandName,CommandValue) " +
                    " VALUES ({0} ,0, '{1}', {2}, {3}, 1, NULL, NULL)";

            //string szTemp2 = "INSERT INTO command (ID, CommandType, TimeStamp, PipeID, UserID) VALUES ({0}, 1, '{1}',{2}, 1) ";
            string szSQL1 = string.Format(szTemp2, nCmdID, szDT, -1, -1);
            m_dbMgr.GetResultData(szSQL1, 0);

            int nMaxID = DBUtil.GetMaxID("commandhistory", m_dbMgr) + 1;

            string szTemp1 = "INSERT INTO commandhistory (ID,CommandType,CommandMakeTime,CommandExecuteTime,UserID,CmdID, PipeID, TankID,CommandName,CommandValue,AlarmHistoryID) " +
                             " VALUES ( {0}, 0, '{1}', NULL, 1, {2}, {3}, {4}, NULL, NULL, NULL )";

            string szSQL2 = string.Format(szTemp1, nMaxID, szDT, nCmdID, -1, -1);
            m_dbMgr.GetResultData(szSQL2, 0);
        }

        private void SaveSirenOnCommand(int nTankID, int nPipeID, int nAlarmHistoryID)
        {           
            DateTime dtnow = DateTime.Now;
            string szDT = WebDBManager.MakeDateTimeString(dtnow);

            int nCmdID = DBUtil.GetMaxID("command", m_dbMgr) + 1;
            string szTemp2 = "INSERT INTO command (ID,CommandType,TimeStamp,PipeID,TankID,UserID,CommandName,CommandValue) " +
                    " VALUES ({0} ,1, '{1}', {2}, {3}, 1, NULL, NULL)";

            //string szTemp2 = "INSERT INTO command (ID, CommandType, TimeStamp, PipeID, UserID) VALUES ({0}, 1, '{1}',{2}, 1) ";
            string szSQL1 = string.Format(szTemp2, nCmdID, szDT, nPipeID, nTankID);
            m_dbMgr.GetResultData(szSQL1, 0);

            int nMaxID = DBUtil.GetMaxID("commandhistory", m_dbMgr) + 1;

            string szTemp1 = "INSERT INTO commandhistory (ID,CommandType,CommandMakeTime,CommandExecuteTime,UserID,CmdID, PipeID, TankID,CommandName,CommandValue,AlarmHistoryID) "+
                             " VALUES ( {0}, 1, '{1}', NULL, 1, {2}, {3}, {4}, NULL, NULL, {5} )";

            string szSQL2 = string.Format(szTemp1, nMaxID, szDT, nCmdID, nPipeID, nTankID, nAlarmHistoryID);
            m_dbMgr.GetResultData(szSQL2, 0);
        }

        private void AddAlarmRecentHistory(AlarmInfo alarmInfo)
        {
            int nMaxID = DBUtil.GetMaxID("alarmrecenthistory", m_dbMgr) + 1;
            string szTemp = "INSERT INTO alarmrecenthistory (ID,TankID,PipeID,AlarmHistoryID1,AlarmHistoryID2,AlarmHistoryID3,AlarmHistoryID4) VALUES " +
                " ( {0}, {1} , {2}, NULL, NULL, NULL, NULL) ";
            string szSQL = string.Format(szTemp, nMaxID, alarmInfo.TankID, alarmInfo.PipeID > 0 ? alarmInfo.PipeID.ToString() : "NULL");
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private void SaveAlarmRecentHistory(AlarmInfo alarmInfo)
        {
            int nAlarmID = alarmInfo.AlarmHistoryID;
            string szPipeID = alarmInfo.PipeID > 0 ? "= " + alarmInfo.PipeID.ToString() : "is NULL";
            int nTankID = alarmInfo.TankID;
            
            string szTemp1 = "SELECT ID,AlarmHistoryID1,AlarmHistoryID2,AlarmHistoryID3,AlarmHistoryID4 FROM alarmrecenthistory WHERE TankID = {0} and PipeID {1}";
            string szSQL1 = string.Format(szTemp1, nTankID, szPipeID);
            ArrayList arResult = m_dbMgr.GetResultData(szSQL1, 0);
            if( arResult == null || arResult.Count == 0)
            {
                AddAlarmRecentHistory(alarmInfo);
            }
               
            if(alarmInfo.AlarmType == (int)AlarmType.최고레벨)               
            {
                string szTemp = "UPDATE alarmrecenthistory SET AlarmHistoryID2 = {2} WHERE TankID = {0} and PipeID {1}";
                string szSQL = string.Format(szTemp, nTankID, szPipeID, nAlarmID);
                m_dbMgr.GetResultData(szSQL, 0);
            }
            else if(alarmInfo.AlarmType == (int)AlarmType.온도상승 || alarmInfo.AlarmType == (int)AlarmType.온도하강)
            {                 
                string szTemp = "UPDATE alarmrecenthistory SET AlarmHistoryID3 = {2} WHERE TankID = {0} and PipeID {1}";
                string szSQL = string.Format(szTemp, nTankID, szPipeID, nAlarmID);
                m_dbMgr.GetResultData(szSQL, 0);
            }
            else
            {
                string szTemp = "UPDATE alarmrecenthistory SET AlarmHistoryID1 = {2} WHERE TankID = {0} and PipeID {1}";
                string szSQL = string.Format(szTemp, nTankID, szPipeID, nAlarmID);
                m_dbMgr.GetResultData(szSQL, 0);
            }
                        
        }        

        private void SaveAlarmHistory(AlarmInfo info)
        {
            int nMaxID = DBUtil.GetMaxID("alarmhistory", m_dbMgr) + 1;


            string szPipeID = info.PipeID > 0 ? info.PipeID.ToString() : "NULL";


            string szTemp = "INSERT INTO alarmhistory (ID,TankID,PipeID,BeginTime,AlarmType,StandardValue,StandardRange,RealValue) "+
                            " VALUES ( {0}, {1}, {2}, '{3}', {4}, {5}, {6}, {7}) ";

            string szTime = DBUtility.WebDBManager.MakeDateTimeString(info.BeginTime);
            string szSQL = string.Format(szTemp, nMaxID, info.TankID, szPipeID, szTime, info.AlarmType, info.StandardValue, info.StandardRange, info.RealValue);
            m_dbMgr.GetResultData(szSQL, 0);
            info.AlarmHistoryID = nMaxID;
        }      

        /// <summary>
        /// 시작시 이전에 남아 있는 알람을 로드한다.
        /// </summary>
        public void ReadAllLastAlarm()
        {
            //string strSQL = "SELECT ID, TankID, PipeID, BeginTime, AlarmType FROM alarmhistory where EndTime is NULL";

            string szSQL = "SELECT ah.ID, ah.TankID, ah.PipeID, ah.BeginTime, ah.AlarmType from alarmhistory as ah " +
                           " INNER JOIN alarmrecenthistory as arh on ah.TankID = arh.TankID and (( ah.PipeID = arh.PipeID ) OR (ah.PipeID is NULL and arh.PipeID is NULL)) " +
                           " WHERE EndTime is NULL AND (arh.AlarmHistoryID1 = ah.ID OR arh.AlarmHistoryID2 = ah.ID OR  " +
                           " arh.AlarmHistoryID3 = ah.ID OR arh.AlarmHistoryID4 = ah.ID)";

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                VariousData<int> historyID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> tankID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                int nPipeID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                VariousData<DateTime> beginTime = WebDBManager.GetDateTimeField(arrResult[i + 3]);                
                VariousData<int> status = WebDBManager.GetIntField(arrResult[i + 4].ToString());
              
                if (historyID == null || tankID == null || beginTime == null || status == null)
                    continue;

                AlarmInfo alarmInfo = null;

                long key = DBUtil.ToLong(tankID.Data, nPipeID, status.Data);
                if (m_dicAlarms.TryGetValue(key, out alarmInfo) == true)
                    continue;

                alarmInfo = new AlarmInfo();
                alarmInfo.AlarmHistoryID = historyID.Data;
                alarmInfo.TankID = tankID.Data;
                alarmInfo.PipeID = nPipeID;
                alarmInfo.BeginTime = beginTime.Data;
                alarmInfo.AlarmType = status.Data;
                
                m_AlarmList.Add(alarmInfo);
                m_dicAlarms.Add(key, alarmInfo);
                m_dicHistoryAlarms.Add(historyID.Data, alarmInfo);
            }


            ReadAllAlarmIgnore();
        }


        private void RemoveAlarmForTank(int nTankID, int nPipeID, int nAlarmType)
        {
            AlarmInfo alarmInfo = null;
            long key = DBUtil.ToLong(nTankID, nPipeID, nAlarmType);
            if (m_dicAlarms.TryGetValue(key, out alarmInfo) == false)
                return;

            m_AlarmList.Remove(alarmInfo);
            m_dicAlarms.Remove(key);

            try
            {
                m_dicHistoryAlarms.Remove(alarmInfo.AlarmHistoryID);
            }
            catch (Exception)
            {
            }

            //if (nAlarmType == (int)AlarmType.압력상승 || nAlarmType == (int)AlarmType.압력하강 ||
             //    nAlarmType == (int)AlarmType.탱크배관유량감소 ||  nAlarmType == (int)AlarmType.탱크배관유량증가)
                //RemoveAlarmIgnore(alarmInfo.TankID);

        }
        private AlarmInfo GetAlarmForTank(int nTankID, int nPipeID, int nAlarmType)
        {
            AlarmInfo alarmInfo = null;
            long key = DBUtil.ToLong(nTankID, nPipeID, nAlarmType);
            m_dicAlarms.TryGetValue(key, out alarmInfo);
            return alarmInfo;
        }

        private AlarmInfo GetAlarm(int nAlarmHistoryID)
        {
            AlarmInfo info = null;
            m_dicHistoryAlarms.TryGetValue(nAlarmHistoryID, out info);
            return info;
        }

        private void ResetAlarmRecent(int nTankID, int nPipeID, int nAlarmHistoryID)
        {
            int nAlarmID = nAlarmHistoryID;
            string szPipeID = nPipeID > 0 ? "= " + nPipeID.ToString() : "is NULL";

            // 알람 해제는 네개의 필드 중 하나이므로 모두 검색하여 설정한다.
            // Recent 테이블에 참조 제거
            string szTemp = "UPDATE alarmrecenthistory SET AlarmHistoryID1 = NULL WHERE TankID = {0} and PipeID {1} and AlarmHistoryID1 = {2}";
            string szSQL = string.Format(szTemp, nTankID, szPipeID, nAlarmID);
            m_dbMgr.GetResultData(szSQL, 0);

            szTemp = "UPDATE alarmrecenthistory SET AlarmHistoryID2 = NULL WHERE TankID = {0} and PipeID {1} and AlarmHistoryID2 = {2}";
            szSQL = string.Format(szTemp, nTankID, szPipeID, nAlarmID);
            m_dbMgr.GetResultData(szSQL, 0);

            szTemp = "UPDATE alarmrecenthistory SET AlarmHistoryID3 = NULL WHERE TankID = {0} and PipeID {1} and AlarmHistoryID3 = {2}";
            szSQL = string.Format(szTemp, nTankID, szPipeID, nAlarmID);
            m_dbMgr.GetResultData(szSQL, 0);

            szTemp = "UPDATE alarmrecenthistory SET AlarmHistoryID4 = NULL WHERE TankID = {0} and PipeID {1} and AlarmHistoryID4 = {2}";
            szSQL = string.Format(szTemp, nTankID, szPipeID, nAlarmID);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private void CloseAlarmHistory(int nTankID, int nPipeID, int nAlarmHistoryID, int nAlarmTerminator, DateTime dtEnd, int nOccurType, string alarmComment)
        {
            int nAlarmID = nAlarmHistoryID;

            // AlarmHistory에 EndTime을 세팅한다.
            string szEndDate = WebDBManager.MakeDateTimeString(dtEnd);

            string szPipeID = nPipeID > 0 ?"="+ nPipeID.ToString() : "IS NULL";

            string szTemp = "UPDATE alarmhistory SET EndTime= '{0}', AlarmTerminator={1}, alarmOccurType={4}, alarmComment='{5}' WHERE TankID = {2} and PipeID {3} AND EndTime is NULL";
            string szSQL = string.Format(szTemp, szEndDate, (nAlarmTerminator > 0 ? nAlarmTerminator.ToString() : "NULL"), nTankID, szPipeID, nOccurType, alarmComment);
            m_dbMgr.GetResultData(szSQL, 0);    
        }

        public void ClearAlarm(int nTankID, int nPipeID, int nAlarmHistoryID, int nAlarmTerminator, int occurrence, string comment)
        {
            DateTime dtEnd = DateTime.Now;

            // AlarmHistory테이블에 Alarm EndTime 설정
            CloseAlarmHistory(nTankID, nPipeID, nAlarmHistoryID, nAlarmTerminator, dtEnd, occurrence, comment);
            
            // AlarmRecent테이블에서 알람History ID 삭제
            ResetAlarmRecent(nTankID, nPipeID, nAlarmHistoryID);

            AlarmInfo info = GetAlarm(nAlarmHistoryID);
            if( info != null)
            {
                RemoveAlarmForTank(nTankID, nPipeID, info.AlarmType);

                // Alarm 종료 시간을 저장
                UpdateLastAlarmClearTime(nTankID, nPipeID, dtEnd, (int)info.AlarmType);
            }
                
        }

        AlarmType[] alrmTypes = {  
                AlarmType.온도상승, AlarmType.온도하강, AlarmType.최고레벨,        
                AlarmType.탱크유량증가, AlarmType.탱크유량감소,  AlarmType.압력상승, 
                AlarmType.압력하강, AlarmType.탱크배관유량증가, AlarmType.탱크배관유량감소
        };

        internal void ClearAlarm(int nTankID, int nPipeID, int nUserID)
        {

            DateTime dtEnd = DateTime.Now;

            int[] result = Array.ConvertAll<AlarmType, int>(alrmTypes, delegate(AlarmType value) {return (int) value;});
            for(int i = 0; i < result.Length ; i++)
            {
                int nAlarmType = result[i];

                AlarmInfo info = GetAlarmForTank(nTankID, nPipeID, nAlarmType);
                if (info != null)
                {

                    //if (nAlarmType == (int)AlarmType.압력상승 || nAlarmType == (int)AlarmType.압력하강
                    //     || nAlarmType == (int)AlarmType.탱크배관유량감소 || nAlarmType == (int)AlarmType.탱크배관유량증가)
                    {
                        if(!CheckSirenAlarm(info))
                            SaveSirenOffCommand();
                    }

                    // AlarmHistory테이블에 Alarm EndTime 설정
                    CloseAlarmHistory(nTankID, nPipeID, info.AlarmHistoryID, nUserID, dtEnd, info.OccurrenceType, info.Comment);

                    // AlarmRecent테이블에서 알람History ID 삭제
                    ResetAlarmRecent(nTankID, nPipeID, info.AlarmHistoryID);

                    // 알람 목록에서 제거한다.
                    RemoveAlarmForTank(nTankID, nPipeID, nAlarmType);
                    if (info != null)
                        // Alarm 종료 시간을 저장
                        UpdateLastAlarmClearTime(nTankID, nPipeID, dtEnd, (int)info.AlarmType);  
                }
            }            
        }

        private bool CheckSirenAlarm(AlarmInfo info)
        {
            foreach(AlarmInfo a in m_AlarmList)
            {
                if(a.AlarmHistoryID != info.AlarmHistoryID)
                {
                    //if (a.AlarmType == (int)AlarmType.압력상승 || a.AlarmType == (int)AlarmType.압력하강
                    //     || a.AlarmType == (int)AlarmType.탱크배관유량감소 || a.AlarmType == (int)AlarmType.탱크배관유량증가)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public bool CheckAlarmClearTime(int nTankID, int nPipeID, int nType, int nCheckTime, DateTime dtNow)
        {
            long key = FindAlarmKey(nTankID, nPipeID, nType);
            if (key == -1L)
            {
                return false;
            }

            DateTime dt = m_AlarmClearTime[key];
            TimeSpan sp = dtNow - dt;
            if( sp.TotalSeconds > nCheckTime)
            {
                if (m_AlarmClearTime.ContainsKey(key))
                {
                    m_AlarmClearTime.Remove(key);
                }               
                return false;
            }
            return true;
        }

        private object m_LockObject = new object();
        private void UpdateLastAlarmClearTime(int nTankID, int nPipeID, DateTime dtEnd,int nAlarmType )
        {
            lock(m_LockObject)
            {
                long key = FindAlarmKey(nTankID, nPipeID, nAlarmType);
                if (key >= 0)
                {
                    m_AlarmClearTime.Remove(key);
                    m_AlarmClearTime.Add(key, dtEnd);
                }
                else
                {
                    long a = DBUtil.ToLong(nTankID, nPipeID, nAlarmType);
                    if (!m_AlarmClearTime.ContainsKey(a))
                        m_AlarmClearTime.Add(a, dtEnd);
                }    
            }            
        }

        private long FindAlarmKey(int nTankID, int nPipeID, int nAlarmType)
        {
            long nKey = DBUtil.ToLong(nTankID, nPipeID, nAlarmType);
            if(m_AlarmClearTime.ContainsKey(nKey))
            {
                return nKey;
            }
            return -1L;
        }

        public void RemoveAlarm(int nAlarmHistoryID)
        {
            AlarmInfo alarmInfo = null;
            if (m_dicHistoryAlarms.TryGetValue(nAlarmHistoryID, out alarmInfo) == false)
                return;

            m_AlarmList.Remove(alarmInfo);
            m_dicHistoryAlarms.Remove(nAlarmHistoryID);
            try
            {
                long key = DBUtil.ToLong(alarmInfo.TankID, alarmInfo.PipeID, alarmInfo.AlarmType);
                m_dicAlarms.Remove(key);
            }
            catch(Exception)
            { }

            //RemoveAlarmIgnore(alarmInfo.TankID);
        }


        // AlarmIgonre Check Thread        
        private void CheckAlarmIgnore()
        {
            List<AlarmIgnore> deleteList = new List<AlarmIgnore>();
            while(!m_bExitThred)
            {

                // Read all Igonre
                List<AlarmIgnore> tempList = m_Ignores.Values.ToList();
                
                // clear delete list
                deleteList.Clear();

                // Check Ignore
                foreach (AlarmIgnore ignore in tempList)
                {
                    // check ignore time
                    TimeSpan span = DateTime.Now - ignore.Begin;
                    if( span.TotalSeconds > (ignore.IgnreTime * 60))
                    {
                        // add delete list
                        deleteList.Add(ignore);
                    }
                }
                
                // remove Igonre
                foreach(AlarmIgnore ig in deleteList)
                {
                    AlarmIgnore data = null;
                    if( m_Ignores.TryRemove(ig.TargetID, out data))
                    {

                        KPXAlarmChecker.Instance.SetStableValue(ig.WorkHistoryID);
                        RemoveAlarmIgnore(data);

                    }                   
                }

                for( int i = 0 ; i < 20 ; i++)
                {
                    if(m_bExitThred == true)
                    {
                        break;
                    }
                    Thread.Sleep(50);
                }
            }
        }

        


        internal void CreateAlarmIgnore(int nTankID, int nPipeID, int nUserID, int nIgnoreTime, int nWorkHistoryID)
        {
            DateTime dtBegin = DateTime.Now;
            AlarmIgnore ignore = new AlarmIgnore();
            ignore.TargetType = 1;
            ignore.TargetID = nTankID;
            ignore.Begin = dtBegin;
            ignore.IgnreTime = nIgnoreTime;
            ignore.BeginUser = nUserID;
            ignore.WorkHistoryID = nWorkHistoryID;

            if( m_Ignores.TryAdd(nTankID, ignore))
            {
                SaveAlarmIgnore(ignore);
            }   
            else
            {
                AlarmIgnore ig2 = null;
                if(m_Ignores.TryGetValue(nTankID, out ig2))
                {
                    TimeSpan span1 = DateTime.Now - ignore.Begin;
                    int nTotalTime1 = (ignore.IgnreTime * 60) - (int)span1.TotalSeconds;

                    TimeSpan span2 = DateTime.Now - ig2.Begin;
                    int nTotalTime2 = (ig2.IgnreTime * 60) - (int)span2.TotalSeconds;

                    if( nTotalTime1 > nTotalTime2)
                    {                        
                        m_Ignores.TryRemove(nTankID, out ig2);                       

                        if (m_Ignores.TryAdd(nTankID, ignore))
                        {
                            SaveAlarmIgnore(ignore);
                        };

                        // Add close alarm ignore after remove
                        RemoveAlarmIgnore(ig2);
                    }
                }                
            }
        }

        public AlarmIgnore FindAlarmIgnore(int nTankID)
        {
            AlarmIgnore igonre = null;
            if(m_Ignores.TryGetValue(nTankID, out igonre))
                return igonre;

            return null;
        }


        public void RemoveAlarmIgnore(int nTankID, int nUserID)
        {
            AlarmIgnore ignore = FindAlarmIgnore(nTankID);
            if( ignore != null)
            {
                CloseAlarmIgnore(ignore.ID, nUserID);
                m_Ignores.TryRemove(nTankID, out ignore);
            }           
        }

        private void RemoveAlarmIgnore(AlarmIgnore data)
        {
            CloseAlarmIgnore(data.ID, -1); 
        }

        private void CloseAlarmIgnore(int nID, int nEndUser)
        {          
            string szDtEnd = WebDBManager.MakeDateTimeString(DateTime.Now);
            string szTemp = "UPDATE alarmignorehistory SET IgnoreEndTime = '{0}' , EndUserId = {1} WHERE ID = {2}";
            
            string szSQL = string.Format(szTemp, szDtEnd, nEndUser, nID);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private void SaveAlarmIgnore(AlarmIgnore ignore)
        {
            int nMaxID = DBUtil.GetMaxID("alarmignorehistory", m_dbMgr) + 1;

            string szTemp = "INSERT INTO alarmignorehistory (ID,TargetType,TargetID, IgnoreBeginTime, IgnoreTime, BeginUserId, WorkHistoryID) " +
                            " VALUES ( {0}, {1}, {2}, '{3}', '{4}', {5}, {6})";

            string szDtBegin = WebDBManager.MakeDateTimeString(ignore.Begin);
            string szSQL = string.Format(szTemp, nMaxID, ignore.TargetType, ignore.TargetID, szDtBegin, ignore.IgnreTime, ignore.BeginUser, ignore.WorkHistoryID);
            m_dbMgr.GetResultData(szSQL, 0);
            ignore.ID = nMaxID;    
        }
        
        internal void OnChangedOption(int nTankID, AlarmOption option)
        {
            AlarmIgnore ignore = FindAlarmIgnore(nTankID);
            if (ignore != null)
            {
                if(ignore.IgnreTime != option.StableBeginWorkM)
                {
                    ignore.IgnreTime = option.StableBeginWorkM;
                }
            }
        }
    
        public void ReadAllAlarmIgnore()
        {
            string szSQL = "SELECT ID,TargetType,TargetID,IgnoreBeginTime,IgnoreTime,BeginUserId,WorkHistoryID FROM alarmignorehistory WHERE IgnoreEndTime is NULL";

            ArrayList arResult = m_dbMgr.GetResultData(szSQL, 0);
            DateTime now = DateTime.Now;
            if( arResult != null && arResult.Count > 0)
            {
                for( int i = 0 ; i < arResult.Count - 6; i += 7)
                {
                    int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                    int nType = WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);
                    int nTankID = WebDBManager.GetIntField(arResult[i + 2].ToString(), -1);
                    VariousData<DateTime> dtIgnoreBeginTime = WebDBManager.GetDateTimeField(arResult[i + 3]);
                    int nTime = WebDBManager.GetIntField(arResult[i + 4].ToString(), 0);
                    int nUserID = WebDBManager.GetIntField(arResult[i + 5].ToString(),-1);
                    int nWorkHistoryID = WebDBManager.GetIntField(arResult[i + 6].ToString(), -1);

                    if (dtIgnoreBeginTime == null)
                        continue;

                    AlarmIgnore ignore = new AlarmIgnore();
                    ignore.ID = nID;
                    ignore.TargetType = nType;
                    ignore.TargetID = nTankID;
                    ignore.Begin = dtIgnoreBeginTime.Data;
                    ignore.IgnreTime = nTime;
                    ignore.BeginUser = nUserID;
                    ignore.WorkHistoryID = nWorkHistoryID;

                    DateTime dt = dtIgnoreBeginTime.Data.AddMinutes(nTime);
                    if(dt > now)
                    {
                        if (!m_Ignores.ContainsKey(nTankID))
                        {
                            m_Ignores.TryAdd(nTankID, ignore);
                        }
                        else
                        {
                            CloseAlarmIgnore(nID, -1);
                        }  
                    }
                    else
                    {
                        CloseAlarmIgnore(nID, -1);
                    }                                     
                }
            }
        }
    
    }
}
