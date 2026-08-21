using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using UnE.Sensor;

namespace UnE.SOP
{
    internal class Supervisor : ISupervisor
    {
        protected class ActionStepComparer : IComparer
        {
            private Dictionary<string, int> m_dicActionStepPriority = new Dictionary<string, int>();

            public ActionStepComparer()
            {
                foreach (string strActionStepName in UnE.SOP.Sections.SectionTabControl.StandardActionStepNames)
                {
                    int nPriority = UnE.SOP.Sections.SectionTabControl.GetActionStepPriority(strActionStepName);
                    m_dicActionStepPriority[strActionStepName] = nPriority;
                }
            }

            public void SetActionStepPriority(string strActionStepName, int nPriority)
            {
                m_dicActionStepPriority[strActionStepName] = nPriority;
            }

            private int GetActionStepPriority(string strActionStepName)
            {
                int nPriority;

                if (m_dicActionStepPriority.TryGetValue(strActionStepName, out nPriority))
                    return nPriority;

                return -1;
            }

            public int Compare(object x, object y)
            {
                ActionStepInfo actionStep1 = (ActionStepInfo)x;
                ActionStepInfo actionStep2 = (ActionStepInfo)y;

                int nPriority1 = GetActionStepPriority(actionStep1.ActionStepName);
                int nPriority2 = GetActionStepPriority(actionStep2.ActionStepName);

                if (nPriority1 > nPriority2)
                    return 1;
                else if (nPriority1 < nPriority2)
                    return -1;
                //else
                return 0;
            }
        }

        protected WebDBManager m_dbMgr = null;
        protected DirectDBManager m_directDBManager = null;
        private Thread m_monitoringThread = null;
        private bool m_exitThread = false;

        protected List<SOPCheckData> m_CheckData = new List<SOPCheckData>();
        protected SortedList<int, SOPCheckData> m_CheckList = new SortedList<int, SOPCheckData>();
        protected object m_LockObj = new object();

        protected Dictionary<int, int> m_dicSameSensorGroup = new Dictionary<int, int>();
        protected bool m_bProcessClose = false;

        protected ISOPScenarioManager m_scenarioManager = null;
        protected Control m_ctrlInvoke = null;
        protected ISOPOwner m_sopOwner = null;

        public Supervisor()
        {
        }

        public Supervisor(DirectDBManager dbMgr)
        {
            m_directDBManager = dbMgr;
        }

        public void Start(WebDBManager dbMgr, ISOPScenarioManager scenarioManager, Control invokeCtrl, ISOPOwner sopOwner)
        {
            m_dbMgr = dbMgr;
            m_scenarioManager = scenarioManager;
            m_ctrlInvoke = invokeCtrl;
            m_sopOwner = sopOwner;

            if (m_monitoringThread == null)
            {
                m_exitThread = false;
                m_monitoringThread = new Thread(MonitorSOP);
                m_monitoringThread.Name = "ClsoeSOP Monitor";
                m_monitoringThread.Start();
            }
        }

        public void Stop()
        {
            if (m_monitoringThread != null)
            {
                m_exitThread = true;
                m_monitoringThread.Join();
            }
        }

        public void TouchSOP(int nActionStepHistory)
        {
            lock (m_LockObj)
            {
                if (m_CheckList.ContainsKey(nActionStepHistory) == true)
                {
                    m_CheckList[nActionStepHistory].TouchTime = DateTime.Now;
                }
            }
        }

        // 새로운 SOP 시작
        public void AddSOP(int nActionStepHistoryID, int nSensorZoneID, int nSensorZoneHistoryID)
        {
            if (nActionStepHistoryID < 0)
                throw new ArgumentException("ActionStepHistoryID 가 비정상적입니다.");

            UnE.SOP.Workstate.SOPScenario flow = m_scenarioManager.GetSOPScenario(nActionStepHistoryID);
            if (flow == null)
                throw new ArgumentException("ActionStepHistoryID 가 비정상적입니다.");

            string szCategoryName = flow.CategoryName;
            if (szCategoryName == null || szCategoryName == "")
                throw new ArgumentException("ActionStepPath 가 비정상적입니다.");

            UnE.SOP.SOPCloseOption option = null;
            try
            {
                option = UnE.SOP.ProxySOP.Instance.OptionSOPAutoCloseSet[szCategoryName];
            }
            catch (Exception)
            { }

            if (option == null)
                throw new ArgumentException("자동종료 옵션 설정이 비정상적입니다.");

            SOPCheckData data = new SOPCheckData();
            data.CloseNoInput = option.UseCloseSOPWaitInputTime;
            data.CloseSensorClose = (option.UseCloseSOPSensorReset || option.UseCloseSOPSensorResetWaitTime);

            // 체크할 것이 없으므로 더할 필요가 없다
            if (data.CloseNoInput == false && data.CloseSensorClose == false)
            {
                return;
            }

            data.ReciveSensorClsseTime = option.UseCloseSOPSensorResetWaitTime;

            data.CloseNoInputTime = option.CloseSOPWaitInputTime * 60;
            data.CloseSensorWaitTime = option.CloseSOPSensorResetWaitTime * 60;

            DateTime dtNow = DateTime.Now;
            data.CheckTime = dtNow;
            data.TouchTime = dtNow;

            data.ReciveSensorClose = false;
            data.ActionStepHistoryID = nActionStepHistoryID;
            data.SensorZoneID = nSensorZoneID;
            data.SensorZoneHistoryID = nSensorZoneHistoryID;
            data.SensorType = ReadSensorType(nSensorZoneID);
            data.MaxActionStepIndex = GetActionStepIndex(nActionStepHistoryID);

            if (!m_CheckList.ContainsKey(nActionStepHistoryID))
            {
                lock (m_LockObj)
                {
                    m_CheckData.Add(data);
                    m_CheckList.Add(nActionStepHistoryID, data);

                    System.Diagnostics.Trace.WriteLine("Add SOP Check : " + nActionStepHistoryID + ", " + nSensorZoneID + ", " + nSensorZoneHistoryID);
                }

                OnAddSOP(data);

                try
                {
                    SaveData(data);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Duplicated SOP Check Data");
            }
        }

        // 외부에서 사용자가 종료하는 ActionStepHistory
        public void RemoveSOP(int nActionStepHistoryID)
        {
            if (nActionStepHistoryID < 0)
                return;

            SOPCheckData data = null;
            lock (m_LockObj)
            {
                if (m_CheckList.ContainsKey(nActionStepHistoryID) == true)
                {
                    data = m_CheckList[nActionStepHistoryID];
                    m_CheckList.Remove(nActionStepHistoryID);

                    if (m_CheckData.Contains(data))
                    {
                        m_CheckData.Remove(data);
                        OnRemoveSOP(data);
                    }

                    System.Diagnostics.Trace.WriteLine("Close SOP Data  : " + data.ActionStepHistoryID);
                }
            }
            RemoveData(data);
        }

        // 센서 종료 신호
        public void SensorClose(int nSensorZoneID, int nSensorZoneHistoryID)
        {
            if (nSensorZoneID < 0)
                return;

            List<SOPCheckData> checkItems = new List<SOPCheckData>();
            lock (m_LockObj)
            {
                checkItems.AddRange(m_CheckData);
            }

            foreach (SOPCheckData data in checkItems)
            {
                // 제어권이 없는경우 처리하지 않음
                if (m_sopOwner.HasSOPControl(data.ActionStepHistoryID) == false)
                    continue;

                if (data.SensorZoneID == nSensorZoneID)
                {
                    data.ReciveSensorClose = true;
                    data.CheckTime = DateTime.Now;

                    System.Diagnostics.Trace.WriteLine("Check SensorClose  Sensor: " + data.SensorZoneID);
                }
                else if (data.SensorZoneHistoryID == nSensorZoneHistoryID)
                {
                    data.ReciveSensorClose = true;
                    data.CheckTime = DateTime.Now;

                    System.Diagnostics.Trace.WriteLine("Check SensorClose History : " + data.ActionStepHistoryID);
                }
            }
        }

        //먼저 돌고 있는 sop에 같은 sensor group이 있을 때 해당 로직처리를 위한 dictionary
        //1개 이상의 sensorgroup이 있을 경우를 위한 처리. 
        public void RegisterSameSensorGroupRunning(int sensorHistoryID, int activeSensorHistoryID)
        {
            m_dicSameSensorGroup.Add(sensorHistoryID, activeSensorHistoryID);
        }

        // 이미 strSOPFullPath에 해당하는 SOP가 실행중이다.
        // 이 상태에서 새로운 알람 신호가 들어왔는데, 위험단계를 바꿔가며 또다른 SOP를 로딩해야 하는지를 확인한다.
        // strSOPPath : 마지막 ActionStep을 제외한 [대분류/중분류/소분류] 3단계로만 되어있다.
        // Return 값 : strSOPPath가 바뀌었는가?
        public virtual bool CheckOpenSOP(List<UnE.SOP.Workstate.SOPScenario> currentScenarios, ref string strSOPFullPath, int nSensorZoneID, int nSensorZoneHistoryID, int nSensorType)
        {
            return false;
        }

        // strSOPPath : 마지막 ActionStep을 제외한 [대분류/중분류/소분류] 3단계로만 되어있다.
        // 실제 SOP를 불러오기 위해서는 마지막 ActionStepName이 필요한데, nSensorType에 따라 적당한 ActionStepName을 추천해준다.
        public string GetActionStepName(string strSOPPath, int nSensorType)
        {
            object dbMgr = GetDBManager();

            // 우선순위에 따라 위기경보단계 이름들을 정렬한다.
            Dictionary<int, string> dicActionSteps = SortStandardActionStepNames(nSensorType);

            string[] tokens = strSOPPath.Split('/');

            if (tokens.Count() < 3)
                return "";

            string strSQL = "Select d.ID, v.LastAccessTime ";
            strSQL += "from ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc, Version as v ";
            strSQL += "where step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and d.VersionID = v.ID and ";
            strSQL += string.Format("dc.CategoryName = '{0}' and sdc.SubCategoryName = '{1}' and d.DisasterName = '{2}' ", tokens[0].Trim(), tokens[1].Trim(), tokens[2].Trim());
            strSQL += "group by d.ID, v.LastAccessTime";

            ArrayList arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return "";

            int nResultCount = arrResult.Count;
            DateTime maxTime = new DateTime();
            int nDisasterID = -1;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 1]);

                if (disasterID == null || time == null)
                    continue;

                if (time.Data > maxTime)
                {
                    maxTime = time.Data;
                    nDisasterID = disasterID.Data;
                }
            }

            if (nDisasterID < 0)
                return "";

            strSQL = "Select ID, StepName from ActionStep where DisasterID = " + nDisasterID.ToString();
            arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return "";

            nResultCount = arrResult.Count;
            List<string> actionStepNames = new List<string>();

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strActionStepName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strActionStepName == null)
                    continue;

                actionStepNames.Add(strActionStepName.Trim());
            }

            int nStepCount = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames.Count();

            for (int i=0;i<nStepCount;i++)
            {
                string strActionStepName;

                if (dicActionSteps.TryGetValue(i, out strActionStepName))
                {
                    // DB에 존재하는 ActionStep 가운데 우선순위가 가장 높은것을 리턴한다.
                    if (actionStepNames.Contains(strActionStepName))
                        return strActionStepName;
                }
            }

            return "";
        }

        // strSOPPath : 마지막 ActionStep을 제외한 [대분류/중분류/소분류] 3단계로만 되어있다.
        // 실제 SOP를 불러오기 위해서는 마지막 ActionStepName이 필요한데, nAlarmDepth에 맞는 ActionStepName을 리턴한다.
        // 만일, 알람 단계에 해당하는 ActionStep이 존재하지 않을경우 그보다 하위 단계의 ActionStep을 리턴한다.
        // 그마저도 없을 경우 한단계씩 상위 단계의 ActionStep을 찾아 리턴한다.
        public string GetActionStepNameFromAlarmDepth(string strSOPPath, int nAlarmDepth)
        {
            object dbMgr = GetDBManager();

            string[] tokens = strSOPPath.Split('/');

            if (tokens.Count() < 3)
                return "";

            string strSQL = "Select d.ID, v.LastAccessTime ";
            strSQL += "from ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc, Version as v ";
            strSQL += "where step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and d.VersionID = v.ID and ";
            strSQL += string.Format("dc.CategoryName = '{0}' and sdc.SubCategoryName = '{1}' and d.DisasterName = '{2}' ", tokens[0].Trim(), tokens[1].Trim(), tokens[2].Trim());
            strSQL += "group by d.ID, v.LastAccessTime";

            ArrayList arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return "";

            int nResultCount = arrResult.Count;
            DateTime maxTime = new DateTime();
            int nDisasterID = -1;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 1]);

                if (disasterID == null || time == null)
                    continue;

                if (time.Data > maxTime)
                {
                    maxTime = time.Data;
                    nDisasterID = disasterID.Data;
                }
            }

            if (nDisasterID < 0)
                return "";

            strSQL = "Select ID, StepName from ActionStep where DisasterID = " + nDisasterID.ToString();
            arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return "";

            nResultCount = arrResult.Count;
            List<string> actionStepNames = new List<string>();

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strActionStepName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strActionStepName == null)
                    continue;

                actionStepNames.Add(strActionStepName.Trim());
            }

            int nStepCount = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames.Count();

            if ((nAlarmDepth - 1) <= nStepCount)
            {
                string strActionStepName = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[nAlarmDepth - 1];

                // nAlarmDepth에 딱맞는 ActionStep이 존재하면 그걸 리턴한다.
                if (actionStepNames.Contains(strActionStepName))
                    return strActionStepName;
                else
                {
                    // 그렇지 않다면 그보다 하위 단계의 ActionStep을 리턴한다.
                    for (int i=nAlarmDepth-2;i>=0;i--)
                    {
                        string strActionStepName2 = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[i];

                        if (actionStepNames.Contains(strActionStepName2))
                            return strActionStepName2;
                    }

                    // 그마저도 없으면 그보다 상위 단계의 ActionStep을 리턴한다.
                    for (int i=nAlarmDepth;i<nStepCount;i++)
                    {
                        string strActionStepName3 = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[i];

                        if (actionStepNames.Contains(strActionStepName3))
                            return strActionStepName3;
                    }
                }
            }

            return "";
        }

        protected object GetDBManager()
        {
            if (m_dbMgr != null)
                return m_dbMgr;
            else if (m_directDBManager != null)
                return m_directDBManager;

            return null;
        }

        protected int GetSiteID()
        {
            if (m_dbMgr != null)
                return m_dbMgr.SiteID;
            else if (m_directDBManager != null)
                return m_directDBManager.SiteID;

            return -1;
        }

        protected ArrayList GetResultData(string strSQL, object dbMgr)
        {
            if (dbMgr is WebDBManager)
                return ((WebDBManager)dbMgr).GetResultData(strSQL);
            else if (dbMgr is DirectDBManager)
                return ((DirectDBManager)dbMgr).GetResultData(strSQL);

            return null;
        }

        // 우선순위에 따라 위기경보단계 이름들을 정렬한다.
        protected virtual Dictionary<int, string> SortStandardActionStepNames(int nSensorType)
        {
            int nStepCount = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames.Count();
            Dictionary<int, string> dicActionSteps = new Dictionary<int, string>();

            // 우선순위에 따라 위기경보단계 이름들을 정렬한다.
            for (int i = 0; i < nStepCount; i++)
            {
                string strActionStepName = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[i];
                int nPriority = UnE.SOP.Sections.SectionTabControl.GetActionStepPriority(strActionStepName);
                dicActionSteps[nPriority] = strActionStepName;
            }

            return dicActionSteps;
        }

        protected virtual void OnAddSOP(SOPCheckData data)
        {
        }

        protected virtual void OnRemoveSOP(SOPCheckData data)
        {
        }

        // 센서값 변경으로 인하여 SOP 단계가 변경되지 않는지 확인한다.
        protected virtual void CheckSensorReactionHistory(SOPCheckData data)
        {
        }

        public virtual void SortDisasterActionSteps(DisasterInfo disaster)
        {
            if (disaster.ActionSteps.Count <= 1)
                return;

            disaster.ActionSteps.Sort(new ActionStepComparer());
        }

        private IFacility.FacilityType ReadSensorType(int nSensorZoneID)
        {
            if (nSensorZoneID < 0)
                return IFacility.FacilityType.NONE;

            object dbMgr = GetDBManager();

            string strSQL = "Select Type from SensorZone where ID = " + nSensorZoneID.ToString();
            ArrayList arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count == 0)
                return IFacility.FacilityType.NONE;

            int nSensorType = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return IFacility.ToFacilityType(nSensorType);
        }

        private int GetMaxID(string strTableName, object dbMgr)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private bool ExistCheckDataInDB(SOPCheckData data)
        {
            if (data == null)
                return true;

            object dbMgr = GetDBManager();

            string szTmp = "SELECT ID FROM ActionStepAutoClose WHERE ActionStepHistoryID = {0}";
            string szSQL = string.Format(szTmp, data.ActionStepHistoryID);

            ArrayList arResult = GetResultData(szSQL, dbMgr);
            if (arResult == null || arResult.Count == 0)
                return false;
            return true;
        }

        private void SaveData(SOPCheckData data)
        {
            object dbMgr = GetDBManager();

            if (!ExistCheckDataInDB(data))
            {
                int nMaxID = GetMaxID("ActionStepAutoClose", dbMgr) + 1;
                string szTemp = "INSERT INTO ActionStepAutoClose (ID, ActionStepHistoryID,ActionStepID, UseCloseNoInput, " +
                               " UseCloseSensorReset, UseCloseSensorResetWaitTime,InputWaitTime, SensorResetWaitTime, " +
                               " BeginTime, SensorZoneID, SensorZoneHistoryID ) " +
                               " VALUES  ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', {9}, {10}) ";
                string szSQL = string.Format(szTemp, nMaxID, data.ActionStepHistoryID, "NULL",
                             (data.CloseNoInput ? 1 : 0), //3 
                             (data.CloseSensorClose ? 1 : 0), // 4
                             (data.ReciveSensorClsseTime ? 1 : 0), // 5
                             data.CloseNoInputTime,//6
                             data.CloseSensorWaitTime,//7
                             WebDBManager.MakeDateTimeString(data.CheckTime), //8
                             data.SensorZoneID < 0 ? "NULL" : data.SensorZoneID.ToString(),
                             data.SensorZoneHistoryID < 0 ? "NULL" : data.SensorZoneHistoryID.ToString());//9,10

                GetResultData(szSQL, dbMgr);

                System.Diagnostics.Trace.WriteLine("Save DB Check Data  : " + data.ActionStepHistoryID);
            }
        }

        private void MonitorSOP()
        {
            List<SOPCheckData> deleteItems = new List<SOPCheckData>();
            List<SOPCheckData> checkItems = new List<SOPCheckData>();

            lock (m_LockObj)
            {
                m_CheckData.Clear();
                m_CheckList.Clear();
            }

            // Read All History
            LoadAllData();

            while (m_exitThread == false)
            {
                deleteItems.Clear();
                checkItems.Clear();

                DateTime dtNow = DateTime.Now;

                lock (m_LockObj)
                {
                    checkItems.AddRange(m_CheckData);
                }

                foreach (SOPCheckData data in checkItems)
                {
                    if (m_sopOwner.HasSOPControl(data.ActionStepHistoryID) == false)
                        continue;

                    if (data.CheckedSensorClose == true && data.CheckedTimeClose == true)
                    {
                        deleteItems.Add(data);
                        continue;
                    }

                    if (m_exitThread == true)
                        break;

                    // 센서값 변경으로 인하여 SOP 단계가 변경되지 않는지 확인한다.
                    CheckSensorReactionHistory(data);

                    // Check Touch Time if CloseNoInput is on
                    if (data.CloseNoInput == true)
                    {
                        TimeSpan span = dtNow - data.TouchTime;
                        if (span.TotalSeconds > data.CloseNoInputTime)
                        {
                            data.CheckedTimeClose2 = true;
                            deleteItems.Add(data);
                        }
                    }

                    if (m_exitThread == true)
                        break;

                    // Check Sensor Close Recived if CloseSensorClose is on
                    if (data.CloseSensorClose == true)
                    {
                        if (data.ReciveSensorClose == false)
                        {
                            if (CheckSensorClose(data.SensorZoneHistoryID))
                            {
                                data.ReciveSensorClose = true;
                                data.CheckTime = DateTime.Now;
                            }
                        }

                        if (data.ReciveSensorClsseTime == true)
                        {
                            // check reicve span time 
                            if (data.ReciveSensorClose == true)
                            {
                                TimeSpan span = dtNow - data.CheckTime;
                                if (span.TotalSeconds > data.CloseSensorWaitTime)
                                {
                                    data.CheckedSensorClose2 = true;
                                    deleteItems.Add(data);
                                }
                            }
                        }
                        else
                        {
                            if (data.ReciveSensorClose == true)
                            {
                                data.CheckedSensorClose2 = true;
                                deleteItems.Add(data);
                            }
                        }
                    }

                    if (m_exitThread == true)
                        break;

                    List<int> deleteTemps = new List<int>();

                    if (m_dicSameSensorGroup.Count > 0)
                    {
                        foreach (KeyValuePair<int, int> pair in m_dicSameSensorGroup)
                        {
                            if (CheckSensorClose(pair.Key))      //같은 센서존에 있는 센서가 Close 되었는지 체크.(해당 센서는 history만 있고, SDMS,SOP에 수신 처리 되지 않음) {
                            {
                                if (CheckSensorClose(data.SensorZoneHistoryID) && (data.SensorZoneHistoryID == pair.Value))
                                {
                                    //data.CheckedSensorClose = true;
                                    //data.CheckedTimeClose = true;
                                    deleteItems.Add(data);
                                    deleteTemps.Add(pair.Key);
                                }

                            }
                            //else if (CheckSensorClose(pair.Value))    //같은 센서 존에 있는 그룹에 활성화된 센서가 종료된 경우 해제하기 위함.
                            //{
                            //    deleteTemps.Add(pair.Key);
                            //}
                        }
                        foreach (int beDeleteID in deleteTemps)
                        {
                            m_dicSameSensorGroup.Remove(beDeleteID);
                        }
                    }
                    else
                    {
                        /*if (CheckSensorClose(data.SensorZoneHistoryID))
                        {
                            //data.CheckedSensorClose = true;
                            //data.CheckedTimeClose = true;
                            deleteItems.Add(data);
                        }*/
                    }


                }

                if (m_exitThread == true)
                    break;

                // Remove close sop
                foreach (SOPCheckData data in deleteItems)
                {
                    if (data.CheckedSensorClose == true && data.CheckedTimeClose == true)
                    {
                        lock (m_LockObj)
                        {
                            m_CheckData.Remove(data);
                            m_CheckList.Remove(data.ActionStepHistoryID);
                        }

                        OnRemoveSOP(data);
                    }
                    else
                        CloseSOP(data);
                }

                for (int i = 0; i < 20; i++)
                {
                    if (m_exitThread == true)
                        break;
                    Thread.Sleep(100);
                }
            }
        }

        // SOP 종료 처리
        private void CloseSOP(SOPCheckData data)
        {
            if (m_bProcessClose == true)
                return;
            if (data.Form != null && data.Form.IsDisposed == false && data.Form.Visible == true)
                return;
            
            try
            {
                Thread t = new Thread(CloseThread);
                t.Start(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }
        }

        private void CloseThread(object param)
        {
            m_bProcessClose = true;
            SOPCheckData data = (SOPCheckData)param;

            bool bDelete = false;
            int nActionStepHistoryID = data.ActionStepHistoryID;
            // ActionStep종료 처리

            if (data.Form != null && data.Form.IsDisposed == false && data.Form.Visible == true)
            {
                m_bProcessClose = false;
                return;
            }

            if (data.Form != null && data.Form.IsDisposed == true)
            {
                data.Form = new PopupSOPClose();
                data.Form.TopMost = true;
            }

            PopupSOPClose form = data.Form;

            UnE.SOP.Workstate.SOPScenario sco = null;
            // SOP가 화면에 나타날때까지 최대 30초까지만 기다린다.
            int nLimit = 30;

            if (m_scenarioManager != null)
            {
                for (int i = 0; i < nLimit && sco == null; i++)
                {
                    sco = m_scenarioManager.GetSOPScenario(nActionStepHistoryID);

                    if (sco != null)
                        form.SetSOPName(sco.ActionStepFullPath);
                    else
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    //if( form.ShowDialog() == DialogResult.OK)
                    {
                        m_ctrlInvoke.Invoke((MethodInvoker)delegate
                        {
                            UnE.SOP.Workstate.SOPScenario scenario = m_sopOwner.GetSOPScenario(nActionStepHistoryID);

                            if (scenario != null)
                            {
                                m_sopOwner.StopWorkflow(DateTime.Now, false, scenario.ActionStepID, scenario.RealMode);
                            }
                            /*UnE.SOP.Sections.SectionTabPage page = m_sopOwner.GetTabPage(nActionStepHistoryID);
                            if (page != null)
                            {
                                m_sopOwner.StopWorkflow(DateTime.Now, false, page.ActionStepID, !page.VirtualMode);
                            }*/
                        });

                        bDelete = true;
                    }

                    if ((data.CheckedSensorClose == true && data.CheckedTimeClose == true) || bDelete == true)
                    {

                        data.CheckedSensorClose = true;
                        data.CheckedTimeClose = true;

                        RemoveData(data);
                    }
                    else
                    {
                        data.CheckedSensorClose = data.CheckedSensorClose2;
                        data.CheckedTimeClose = data.CheckedTimeClose2;
                    }
                }
            }

            m_bProcessClose = false;
        }

        private void RemoveData(SOPCheckData data)
        {
            if (data == null)
                return;

            object dbMgr = GetDBManager();

            string szSQL = "DELETE FROM ActionStepAutoClose WHERE ActionStepHistoryID=" + data.ActionStepHistoryID;
            GetResultData(szSQL, dbMgr);
        }

        private void LoadAllData()
        {
            string szSQL = "SELECT asac.ID, asac.ActionStepHistoryID, asac.ActionStepID, asac.UseCloseNoInput, asac.UseCloseSensorReset, asac.UseCloseSensorResetWaitTime, " +
                            " asac.InputWaitTime, asac.SensorResetWaitTime, asac.BeginTime, asac.SensorZoneID, asac.SensorZoneHistoryID, asac.Description, sz.Type " +
                            " FROM ActionStepAutoClose as asac, ActionStepHistory as ash, SensorZoneHistory as szh, SensorZone as sz " +
                            " where asac.ActionStepHistoryID = ash.ID and ash.SensorZoneHistoryID = szh.id and szh.SensorID = sz.ID and ash.EndTime is NULL and ash.CancelTime is NULL and asac.SensorZoneHistoryID is not null";

            object dbMgr = GetDBManager();

            ArrayList arResult = GetResultData(szSQL, dbMgr);
            if (arResult == null || arResult.Count == 0)
                return;

            string strActionStepHistoryIDs = "";
            Dictionary<int, SOPCheckData> dicActionStepHistoryCheckDatas = new Dictionary<int, SOPCheckData>();

            for (int i = 0; i < arResult.Count; i += 13)
            {
                int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                int nActionStepHistoryID = WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);
                int nActionStepID = WebDBManager.GetIntField(arResult[i + 2].ToString(), -1);
                int nUseCloseNoInput = WebDBManager.GetIntField(arResult[i + 3].ToString(), -1);
                int nUseCloseSensorReset = WebDBManager.GetIntField(arResult[i + 4].ToString(), -1);
                int nUseCloseSensorResetWaitTime = WebDBManager.GetIntField(arResult[i + 5].ToString(), -1);
                int nInputWaitTime = WebDBManager.GetIntField(arResult[i + 6].ToString(), -1);
                int nSensorResetWaitTime = WebDBManager.GetIntField(arResult[i + 7].ToString(), -1);
                DateTime dtBeginTime = WebDBManager.GetDateTimeField(arResult[i + 8].ToString(), DateTime.Now);
                int nSensorZoneID = WebDBManager.GetIntField(arResult[i + 9].ToString(), -1);

                int nSensorZoneHistoryID = WebDBManager.GetIntField(arResult[i + 10].ToString(), -1);
                string szDescription = WebDBManager.GetStringField(arResult[i + 11].ToString());
                int nFacilityType = WebDBManager.GetIntField(arResult[i + 12].ToString(), -1);

                if (nSensorZoneID < 0 || nSensorZoneHistoryID < 0)
                    continue;

                SOPCheckData data = new SOPCheckData();
                data.CloseNoInput = nUseCloseNoInput == 1 ? true : false;
                data.CloseSensorClose = nUseCloseSensorReset == 1 ? true : false;
                data.ReciveSensorClsseTime = nUseCloseSensorResetWaitTime == 1 ? true : false;

                data.CloseNoInputTime = nInputWaitTime;
                data.CloseSensorWaitTime = nSensorResetWaitTime;

                DateTime dtNow = DateTime.Now;
                data.CheckTime = dtBeginTime;
                data.TouchTime = dtNow;
                data.ReciveSensorClose = false;
                data.ActionStepHistoryID = nActionStepHistoryID;
                data.SensorZoneID = nSensorZoneID;
                data.SensorZoneHistoryID = nSensorZoneHistoryID;
                data.SensorType = Sensor.IFacility.ToFacilityType(nFacilityType);

                if (!m_CheckList.ContainsKey(nActionStepHistoryID))
                {
                    lock (m_LockObj)
                    {
                        m_CheckData.Add(data);
                        m_CheckList.Add(nActionStepHistoryID, data);
                    }
                }

                dicActionStepHistoryCheckDatas[nActionStepHistoryID] = data;

                if (strActionStepHistoryIDs.Length == 0)
                    strActionStepHistoryIDs = nActionStepHistoryID.ToString();
                else
                    strActionStepHistoryIDs += ", " + nActionStepHistoryID.ToString();
            }

            if (strActionStepHistoryIDs.Length == 0)
                return;

            string strSQL = "Select ash.ID, step.StepName from ActionStepHistory as ash, ActionStep as step ";
            strSQL += "where ash.ActionStepID = step.ID and ash.ID in (" + strActionStepHistoryIDs + ")";

            ArrayList arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strStepName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strStepName == null)
                    continue;

                int nActionStepIndex = GetActionStepIndex(strStepName);

                if (nActionStepIndex > 0)
                {
                    SOPCheckData data;

                    if (dicActionStepHistoryCheckDatas.TryGetValue(id.Data, out data))
                        data.MaxActionStepIndex = nActionStepIndex;
                }
            }
        }

        protected int GetActionStepIndex(string strActionStepName)
        {
            int nCount = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames.Count();

            for (int i=0;i<nCount;i++)
            {
                if (UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[i] == strActionStepName)
                    return i + 1;
            }

            return -1;
        }

        private int GetActionStepIndex(int nActionStepHistoryID)
        {
            string strSQL = "Select step.StepName from ActionStepHistory as ash, ActionStep as step ";
            strSQL += "where ash.ActionStepID = step.ID and ash.ID = " + nActionStepHistoryID.ToString();

            object dbMgr = GetDBManager();

            ArrayList arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            string strStepName = WebDBManager.GetStringField(arrResult[0]);

            if (strStepName == null)
                return -1;

            return GetActionStepIndex(strStepName);
        }

        private bool CheckSensorClose(int nSensorHistoryID)
        {
            if (nSensorHistoryID <= 0)
                return false;

            string strEndStatus = GetEndStatusString();
            object dbMgr = GetDBManager();

            string strSQL = "Select ID from SensorReactionHistory where ReactionType in " + strEndStatus + " and SensorHistoryID = " + nSensorHistoryID.ToString();
            ArrayList arrResult = GetResultData(strSQL, dbMgr);

            if (arrResult != null && arrResult.Count > 0)
                return true;

            return false;
        }

        private string GetEndStatusString()
        {
            string strEndStatusString = "(";

            List<libSensorProcess.ReactionType> types = new List<libSensorProcess.ReactionType>();
            types.Add(libSensorProcess.ReactionType.MALFUNCTION);
            types.Add(libSensorProcess.ReactionType.USER_RESET);
            types.Add(libSensorProcess.ReactionType.IGNORE_SIGNAL);
            types.Add(libSensorProcess.ReactionType.IGNORE_SOP);
            types.Add(libSensorProcess.ReactionType.END_STATUS);
            types.Add(libSensorProcess.ReactionType.TIME_OUT);

            foreach (libSensorProcess.ReactionType type in types)
            {
                if (strEndStatusString.Length == 1)
                    strEndStatusString += ((int)type).ToString();
                else
                    strEndStatusString += "," + ((int)type).ToString();
            }

            strEndStatusString += ")";
            return strEndStatusString;
        }

        protected static bool ReadLastDouble(string str, out double num, out string strNum)
        {
            int len = str.Length;
            num = 0;
            strNum = "";

            bool begin = false, readDot = false;
            int count = 0;
            int nEndIndex = -1, nBeginIndex = -1;

            for (int i = len - 1; i >= 0; i--)
            {
                char ch = str.ElementAt(i);

                if (begin == false)
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        num = ch - '0';
                        count = 1;
                        begin = true;
                        nEndIndex = nBeginIndex = i;
                    }
                    else if (ch == '.')
                    {
                        readDot = true;
                        begin = true;
                        nEndIndex = nBeginIndex = i;
                    }
                }
                else
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        num = num + (ch - '0') * System.Math.Pow(10, count);
                        count++;
                        nBeginIndex = i;
                    }
                    else if (ch == '.')
                    {
                        if (readDot)
                            break;
                        else
                        {
                            num = num * System.Math.Pow(10, -count);
                            readDot = true;
                            count = 0;
                            nBeginIndex = i;
                        }
                    }
                    else
                        break;
                }
            }

            if (nBeginIndex <= nEndIndex && nBeginIndex >= 0)
                strNum = str.Substring(nBeginIndex, nEndIndex - nBeginIndex + 1);
            else
                return false;

            return count > 0 || readDot;
        }
    }
}
