using System;
using System.Collections.Generic;
using System.Linq;
using DBUtility2;
using System.Collections;
using System.Collections.Concurrent;
using System.ServiceModel;
using AgentFactory;
using libSOPPolicy.Common;

namespace ServerProcess.Client
{
    using ServerProcess.Data;
    using UnE.SOP;
    using Data.SOP;

    public class SOPSimulatorServer2 : BaseClient, ISOPSimulatorServer
    {
        private class ActionStepHistory
        {
            private int m_nID = -1;
            private int m_nSensorZoneHistoryID = -1;
            private int m_nLastAccessedUserID = -1;
            // 제어권 소유자로부터의 마지막 제어권 소유확인 시간
            private DateTime m_dtLastControlCheck = new DateTime();
            private bool m_isRealMode = false;
            private int m_nActionStepID = -1;
            // 시작시간
            private DateTime m_dtBegin = new DateTime();
            // 종료시간
            private VariousData<DateTime> m_endTime = null;
            // 취소시간
            private VariousData<DateTime> m_cancelTime = null;
            // 센서탐지시간
            private VariousData<DateTime> m_detectTime = null;
            private int m_nMaxComponentHistoryID = -1;
            private List<ComponentHistory> m_componentHistories = new List<ComponentHistory>();
            // nullable
            private string m_strPosition = null;
            private VariousData<int> m_selectedComponentID = null;
            private VariousData<int> m_selectedComponentType = null;
            private VariousData<int> m_startOption = null;
            private string m_strDisasterOption = null;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public int SensorZoneHistoryID
            {
                get { return m_nSensorZoneHistoryID; }
                set { m_nSensorZoneHistoryID = value; }
            }

            public int LastAccessedUserID
            {
                get { return m_nLastAccessedUserID; }
                set { m_nLastAccessedUserID = value; }
            }

            // 제어권 소유자로부터의 마지막 제어권 소유확인 시간
            public DateTime LastControlCheck
            {
                get { return m_dtLastControlCheck; }
                set { m_dtLastControlCheck = value; }
            }

            public bool IsRealMode
            {
                get { return m_isRealMode; }
                set { m_isRealMode = value; }
            }

            public int ActionStepID
            {
                get { return m_nActionStepID; }
                set { m_nActionStepID = value; }
            }

            // 시작시간
            public DateTime BeginTime
            {
                get { return m_dtBegin; }
                set { m_dtBegin = value; }
            }

            // 종료시간
            public VariousData<DateTime> EndTime
            {
                get { return m_endTime; }
                set { m_endTime = value; }
            }

            // 취소시간
            public VariousData<DateTime> CancelTime
            {
                get { return m_cancelTime; }
                set { m_cancelTime = value; }
            }

            // 센서탐지시간
            public VariousData<DateTime> DetectTime
            {
                get { return m_detectTime; }
                set { m_detectTime = value; }
            }

            public int MaxComponentHistoryID
            {
                get { return m_nMaxComponentHistoryID; }
                set { m_nMaxComponentHistoryID = value; }
            }

            public List<ComponentHistory> ComponentHistories
            {
                get { return m_componentHistories; }
            }

            public string Position
            {
                get { return m_strPosition; }
                set { m_strPosition = value; }
            }

            public VariousData<int> SelectedComponentID
            {
                get { return m_selectedComponentID; }
                set { m_selectedComponentID = value; }
            }

            public VariousData<int> SelectedComponentType
            {
                get { return m_selectedComponentType; }
                set { m_selectedComponentType = value; }
            }

            public VariousData<int> StartOption
            {
                get { return m_startOption; }
                set { m_startOption = value; }
            }

            public string DisasterOption
            {
                get { return m_strDisasterOption; }
                set { m_strDisasterOption = value; }
            }
        }

        private class ComponentHistory
        {
            private int m_nID = -1;
            private int m_nComponentID = -1;
            private int m_nComponentType = -1;
            private DateTime m_timeStamp;
            private int m_nStatus = -1;
            // Nullable
            private string m_strTask = null;
            private VariousData<int> m_completeCount = null;
            private VariousData<int> m_showBoard = null;
            private int m_nAccessedUserID = -1;
            private VariousData<int> m_checkedNotify1 = null;
            private VariousData<int> m_checkedNotify2 = null;
            private VariousData<int> m_checkedRun = null;
            private VariousData<int> m_checkedComplete = null;
            private List<ComponentHistoryDetail> m_detailDatas = new List<ComponentHistoryDetail>();

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public int ComponentID
            {
                get { return m_nComponentID; }
                set { m_nComponentID = value; }
            }

            public int ComponentType
            {
                get { return m_nComponentType; }
                set { m_nComponentType = value; }
            }

            public DateTime TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }

            public int Status
            {
                get { return m_nStatus; }
                set { m_nStatus = value; }
            }

            // Nullable
            public string Task
            {
                get { return m_strTask; }
                set { m_strTask = value; }
            }

            public VariousData<int> CompleteCount
            {
                get { return m_completeCount; }
                set { m_completeCount = value; }
            }

            public VariousData<int> ShowBoard
            {
                get { return m_showBoard; }
                set { m_showBoard = value; }
            }

            public int AccessedUserID
            {
                get { return m_nAccessedUserID; }
                set { m_nAccessedUserID = value; }
            }

            public VariousData<int> CheckedNotify1
            {
                get { return m_checkedNotify1; }
                set { m_checkedNotify1 = value; }
            }

            public VariousData<int> CheckedNotify2
            {
                get { return m_checkedNotify2; }
                set { m_checkedNotify2 = value; }
            }

            public VariousData<int> CheckedRun
            {
                get { return m_checkedRun; }
                set { m_checkedRun = value; }
            }

            public VariousData<int> CheckedComplete
            {
                get { return m_checkedComplete; }
                set { m_checkedComplete = value; }
            }

            public List<ComponentHistoryDetail> DetailDatas
            {
                get { return m_detailDatas; }
            }

            public void AddSendData(ArrayList arrDatas, int nCountIndex)
            {
                int nComponentHistoryCount = (int)arrDatas[nCountIndex];
                arrDatas[nCountIndex] = nComponentHistoryCount + 1;

                arrDatas.Add(m_nID);
                arrDatas.Add(m_nComponentID);
                arrDatas.Add(m_nComponentType);
                arrDatas.Add(m_timeStamp.ToBinary());
                arrDatas.Add(m_nStatus);
                AddStringData(arrDatas, m_strTask);
                AddVariousData<int>(arrDatas, m_completeCount);
                AddVariousData<int>(arrDatas, m_showBoard);
                arrDatas.Add(m_nAccessedUserID);
                AddVariousData<int>(arrDatas, m_checkedNotify1);
                AddVariousData<int>(arrDatas, m_checkedNotify2);
                AddVariousData<int>(arrDatas, m_checkedRun);
                AddVariousData<int>(arrDatas, m_checkedComplete);

                int nDetailCount = m_detailDatas.Count;
                arrDatas.Add(nDetailCount);

                for (int i = 0; i < nDetailCount; i++)
                {
                    ComponentHistoryDetail detail = m_detailDatas[i];
                    detail.AddSendData(arrDatas);
                }
            }

            public static void AddStringData(ArrayList arrDatas, string data)
            {
                if (data == null)
                    arrDatas.Add(-1);
                else
                    arrDatas.Add(data);
            }

            public static void AddVariousData<T>(ArrayList arrDatas, VariousData<T> data)
            {
                if (data == null)
                    arrDatas.Add(-1);
                else
                    arrDatas.Add(data.Data);
            }
        }

        private class ComponentHistoryDetail
        {
            private int m_nDataIndex = -1;
            private VariousData<int> m_datai = null;
            private VariousData<float> m_dataf = null;
            private string m_datas = null;
            private VariousData<DateTime> m_timeStamp = null;

            public int DataIndex
            {
                get { return m_nDataIndex; }
                set { m_nDataIndex = value; }
            }

            public VariousData<int> Datai
            {
                get { return m_datai; }
                set { m_datai = value; }
            }

            public VariousData<float> Dataf
            {
                get { return m_dataf; }
                set { m_dataf = value; }
            }

            public string Datas
            {
                get { return m_datas; }
                set { m_datas = value; }
            }

            public VariousData<DateTime> TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }

            public void AddSendData(ArrayList arrDatas)
            {
                arrDatas.Add(m_nDataIndex);
                ComponentHistory.AddVariousData<int>(arrDatas, m_datai);
                ComponentHistory.AddVariousData<float>(arrDatas, m_dataf);
                ComponentHistory.AddStringData(arrDatas, m_datas);

                if (m_timeStamp == null)
                    arrDatas.Add(-1);
                else
                    arrDatas.Add(m_timeStamp.Data.ToBinary());
            }
        }

        // 현재 제어권 소유자
        // Key : ActionStepHistory ID
        private ConcurrentDictionary<int, Data.SOP.SOPClientData> m_dicControlClient = new ConcurrentDictionary<int, Data.SOP.SOPClientData>();
        // 현재 실행중인 SOP 정보
        // Key : ActionStepHistory ID
        private ConcurrentDictionary<int, ActionStepHistory> m_dicActionStepHistory = new ConcurrentDictionary<int, ActionStepHistory>();

        // 제어권 대기시간
        private int m_nControlWaitSeconds = 0;
        private int m_nMaxActionStepHistoryID = 0;

        private bool m_initialized = true;

        // 제어권이 없는 사용자도 다른 사용자가 제어중인 SOP 화면을 모니터링 할수 있는가?
        private bool m_useSOPMonitoring = true;

        // 센서신호로 인한 SOP 실행권한을 요청한 Client들의 리스트
        // Value에 List를 사용하는 이유는 다수의 Client들의 요청을 처리하기 위하여 lock을 사용하지 않기 위해서다.
        // 서버는 List에 처음으로 담긴 Client에게 실행권한을 부여한다.
        // Key : SensorZoneHistoryID
        // Value : SOPGenUserID List
        private ConcurrentDictionary<int, List<int>> m_dicSensorSOPRequestList = new ConcurrentDictionary<int, List<int>>();

        private int m_nSOPonSensorDetect = SOPonSensorDetect.NotConcern;
        private Dictionary<string, SOPCloseOption> m_dicSOPAutoCloseOptions = new Dictionary<string, SOPCloseOption>();
        private AlarmSOPManager m_alarmSOPManager = new AlarmSOPManager();

        // SOP 실행요청 리스트
        private ConcurrentQueue<SOPRequest> m_sopRequests = new ConcurrentQueue<SOPRequest>();

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.SOP_SIMULATOR; }
        }

        public bool Initialized
        {
            get { return m_initialized; }
        }

        public SOPSimulatorServer2()
            : base()
        {
        }

        public SOPSimulatorServer2(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.SOPSimulator);
            SetControlWaitTime();
        }

        private void ReadUseSOPMonitoringOption()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'UseSOPMonitoring' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strValue = WebDBManager.GetStringField(arrResult[0], "");
            int nValue;

            if (!int.TryParse(strValue, out nValue))
                m_useSOPMonitoring = false;

            m_useSOPMonitoring = nValue == 0 ? false : true;
        }

        private void ReadStandardActionStepNames()
        {
            string strPropertyName = "StandardActionStepNames";

            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            if (arrResult.Count == 0 || arrResult[0] == null)
                return;

            string strValue = WebDBManager.GetStringField(arrResult[0], "");

            string[] stepNames = strValue.Split(',');
            List<string> actionStepNames = new List<string>();

            foreach (string strStepName in stepNames)
            {
                actionStepNames.Add(strStepName.Trim());
            }

            UnE.SOP.Sections.SectionTabControl.SetStandardActionStepNames(actionStepNames);
        }

        private void SetControlWaitTime()
        {
            if (m_agent == null)
                return;

            object time = m_agent.RunMethod(BaseAgent.MethodType.Etc, "ControlWaitSeconds");

            if (time != null && time is int)
            {
                m_nControlWaitSeconds = (int)time;
            }
        }

        protected override void OnLoadEvent()
        {
            ReadUseSOPMonitoringOption();
            ReadStandardActionStepNames();
            // 서버가 재가동되면 그 이전에 가동중이던 SOP를 확인한다.
            CheckPrevControlClient();
            m_initialized = true;
        }

        // 서버가 재가동되면 그 이전에 가동중이던 SOP를 확인한다.
        private void CheckPrevControlClient()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            int nMaxActionStepHistoryID = GetMaxActionHistoryID(dbMgr);

            DateTime dtNow = DateTime.Now;
            DateTime dtOneMonthAgo = dtNow.AddMonths(-1);
            string strTimeLimit = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtOneMonthAgo.Year, dtOneMonthAgo.Month, dtOneMonthAgo.Day, dtOneMonthAgo.Hour, dtOneMonthAgo.Minute, dtOneMonthAgo.Second);

            string strSQL = "Select ash.ID, ash.ActionStepID, ash.BeginTime, ash.DetectTime, ash.RealMode, ash.LastAccessedUserID, ash.SensorZoneHistoryID, ash.Position, ash.SelectedComponentID, ash.SelectedComponentType, ash.StartOption, ash.DisasterOption, dc.CategoryName, sdc.SubCategoryName, d.DisasterName ";
            strSQL += "from ActionStepHistory as ash, ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += " where ash.ActionStepID = step.ID and step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and ash.EndTime is null AND ash.CancelTime is NULL and ash.BeginTime > '" + strTimeLimit + "' ";
            strSQL += " ORDER BY ash.ID DESC";

            int nMaxID = 0;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult != null)
            {
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 14; i += 15)
                {
                    VariousData<int> actionStepHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                    VariousData<DateTime> beginTime = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                    VariousData<DateTime> detectTime = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                    VariousData<int> realMode = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                    VariousData<int> userID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                    VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                    string strPosition = WebDBManager.GetStringField(arrResult[i + 7]);
                    VariousData<int> selectedComponentID = WebDBManager.GetIntField(arrResult[i + 8].ToString());
                    VariousData<int> selectedComponentType = WebDBManager.GetIntField(arrResult[i + 9].ToString());
                    VariousData<int> startOption = WebDBManager.GetIntField(arrResult[i + 10].ToString());
                    string strDisasterOption = WebDBManager.GetStringField(arrResult[i + 11]);

                    string strDisasterCategoryName = WebDBManager.GetStringField(arrResult[i + 12]);
                    string strSubDisasterCategoryName = WebDBManager.GetStringField(arrResult[i + 13]);
                    string strDisasterName = WebDBManager.GetStringField(arrResult[i + 14]);

                    if (actionStepID == null || beginTime == null || realMode == null ||
                        strDisasterCategoryName == null || strSubDisasterCategoryName == null ||
                        strDisasterName == null)
                        continue;

                    if (actionStepHistoryID != null && userID != null)
                    {
                        ActionStepHistory actionStepHistory = new ActionStepHistory();

                        actionStepHistory.ID = actionStepHistoryID.Data;
                        actionStepHistory.ActionStepID = actionStepID.Data;
                        actionStepHistory.BeginTime = beginTime.Data;
                        actionStepHistory.DetectTime = detectTime;
                        actionStepHistory.IsRealMode = realMode.Data != 0;
                        actionStepHistory.LastAccessedUserID = userID.Data;
                        actionStepHistory.LastControlCheck = dtNow;

                        if (sensorZoneHistoryID != null)
                        {
                            actionStepHistory.SensorZoneHistoryID = sensorZoneHistoryID.Data;

                            AlarmData alarm = AlarmManager.Instance.GetAlarm(actionStepHistory.SensorZoneHistoryID);

                            if (alarm != null)
                            {
                                alarm.SOPProcess = AlarmData.SOPProcessType.Run;
                            }
                        }

                        actionStepHistory.Position = strPosition;
                        actionStepHistory.SelectedComponentID = selectedComponentID;
                        actionStepHistory.SelectedComponentType = selectedComponentType;
                        actionStepHistory.StartOption = startOption;
                        actionStepHistory.DisasterOption = strDisasterOption;

                        AddActionStepHistory(actionStepHistory);
                        //m_dicActionStepHistory[actionStepHistoryID.Data] = actionStepHistory;

                        if (IsEarthquakeSOP(strDisasterCategoryName, strSubDisasterCategoryName))
                            EarthquakeSensorServer.Instance.OnBeginSOP(strDisasterCategoryName, strSubDisasterCategoryName, strDisasterName);

                        if (nMaxID < actionStepHistoryID.Data)
                            nMaxID = actionStepHistoryID.Data;
                    }
                }
            }

            dbMgr.Close();

            if (nMaxActionStepHistoryID > nMaxID)
            {
                SetMaxActionStepHistoryID(nMaxActionStepHistoryID);
                //m_nMaxActionStepHistoryID = nMaxActionStepHistoryID;
            }
            else
            {
                SetMaxActionStepHistoryID(nMaxID);
                //m_nMaxActionStepHistoryID = nMaxID;
            }
        }

        private int GetMaxActionHistoryID(DirectDBManager dbMgr)
        {
            string strSQL = "Select max(ID) from ActionStepHistory";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> maxID = WebDBManager.GetIntField(arrResult[0].ToString());
            return maxID == null ? 0 : maxID.Data;
        }

        protected override ServerProcess.Client.ClientData MakeClientData(int nClientType, int nClientSubType, OperationContext ctx, string strIP, int nPort)
        {
            if (m_postOffice != null)
            {
                IPostMan postMan = m_postOffice.GetPostMan(ctx);
                Data.SOP.SOPClientData data = new Data.SOP.SOPClientData(ctx.SessionId, postMan, nClientType, nClientSubType, m_dbMgr);
                data.IP = strIP;
                data.Port = nPort;
                postMan.ClientData = data;

                SendClientData(SOPWebServer.Header.REQUEST_CLIENT_INFO, null, data);
                //SendDataToSOPSimulator(SOPWebServer.Header.REQUEST_CLIENT_INFO, null);
                return data;
            }

            return null;
        }

        protected override int OnReceiveEvent(ServerProcess.Client.ClientData data, OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.REPLY_CLIENT_INFO)
                return ProcessClientInfo((Data.SOP.SOPClientData)data, arrDatas);
            /*else if (header == SOPWebServer.Header.CONFIRM_SOP_CONTROL)
                return ProcessConfirmSOPControl((ClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.SEND_NEW_SOP)
                return ProcessNewSOP((ClientData)data, arrDatas);*/
            else if (header == SOPWebServer.Header.REQUEST_CONTROL)
                return ProcessRequestControl((Data.SOP.SOPClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.CANCEL_REQUEST_CONTROL)
                return ProcessCancelRequestControl((Data.SOP.SOPClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.GIVE_CONTROL)
                return ProcessGiveControl((Data.SOP.SOPClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.REJECT_REQUEST_CONTROL)
                return ProcessRejectRequestControl((Data.SOP.SOPClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.STEAL_CONTROL)
                return ProcessStealControl((Data.SOP.SOPClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.RETURN_CONTROL)
                return ProcessReturnControl((Data.SOP.SOPClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.SOP_SELECT_MISSION)
                return ProcessRelay(header, messages);
            else if (header == SOPWebServer.Header.CHANGE_CONFIG)
            {
                int nResult = ProcessRelay(header, messages, data);
                ProcessChangeConfig(arrDatas);
                return nResult;
            }
            else if (header == SOPWebServer.Header.CHANGE_WORK_MEMBER)
                return ProcessRelay(header, messages, data);
            else if (header == SOPWebServer.Header.SOP_SIMULATOR_COMMAND)
                return ProcessSOPSimulatorCommand(data, header, messages, arrDatas);
            else if (header == SOPWebServer.Header.ALARM_SOP_RESULT)
                return ProcessAlarmSOPResult(arrDatas);
            else if (header == SOPWebServer.Header.SOP_CURRENT_SELECT_MISSION)
                return ProcessRequestComponentHistoryList((Data.SOP.SOPClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.REQUEST_SENSOR_SOP_PERMIT)
                return ProcessRequestSensorSOPPermit((Data.SOP.SOPClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.RUN_SOP)
                return ProcessRunSOP(arrDatas);
            else if (header == SOPWebServer.Header.SELECT_SOP_COMPONENT)
                return ProcessSelectSOPComponent(header, messages);
            else if (header == SOPWebServer.Header.RESET_MAX_ACTIONSTEP_HISTORY_ID)
                return ResetMaxActionStepHistoryID();

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private int ResetMaxActionStepHistoryID()
        {
            m_nMaxActionStepHistoryID = 0;
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessSelectSOPComponent(int header, byte[] bytes)
        {
            SendMessageToClient(SOPWebServer.ClientType.SOP_SIMULATOR, -1, header, bytes, null);
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessRunSOP(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount > 1 && arrDatas[0] is bool && arrDatas[1] is string)
            {
                bool isRealMode = (bool)arrDatas[0];
                string strSOPFullPath = (string)arrDatas[1];
                SOPRequest sop = new SOPRequest(isRealMode, strSOPFullPath);

                for (int i=2;i<nDataCount;i++)
                {
                    if (arrDatas[i] is string)
                    {
                        string strParam = (string)arrDatas[i];
                        sop.AddParameter(strParam);
                    }
                }

                m_sopRequests.Enqueue(sop);
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private void ProcessChangeConfig(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            for (int i=0;i<nDataCount;i+=3)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is string && arrDatas[i + 2] is string)
                {
                    int nClientType = (int)arrDatas[i];
                    string strPropertyName = (string)arrDatas[i + 1];
                    string strPropertyValue = (string)arrDatas[i + 2];

                    if (nClientType == SOPWebServer.ClientType.SOP_SIMULATOR)
                    {
                        if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.RUN_SOP_ON_LOADED))
                        {
                            int nMode;

                            if (int.TryParse(strPropertyValue, out nMode))
                                m_nSOPonSensorDetect = SOPonSensorDetect.GetOption(nMode);
                            else
                                m_nSOPonSensorDetect = SOPonSensorDetect.NotConcern;
                        }
                        else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SOP_AUTO_CLOSE))
                        {
                            ProcessSOPAutoClose(strPropertyValue);
                        }
                    }
                }
            }
        }

        private void ProcessSOPAutoClose(string strValue)
        {
            string[] tokens = strValue.Split(';');

            if (tokens.Count() != 7)
                return;

            int nCategoryID;

            if (int.TryParse(tokens[0].Trim(), out nCategoryID) == false)
                return;

            string strCategoryName = tokens[1].Trim();
            SetSOPAutoClose(nCategoryID, strCategoryName, tokens[2].Trim(), tokens[3].Trim(), tokens[4].Trim(), tokens[5].Trim(), tokens[6].Trim());
        }

        private void SetSOPAutoClose(int nCategoryID, string strCategoryName, string strCloseSOPWaitInputTime, string strUseCloseSOPWaitInputTime, string strUseCloseSOPSensorReset, string strCloseSOPSensorResetWaitTime, string strUseCloseSOPSensorResetWaitTime)
        {
            SOPCloseOption option = null;

            if (m_dicSOPAutoCloseOptions.TryGetValue(strCategoryName, out option) == false)
                option = null;

            bool addOption = false;

            if (option == null)
            {
                option = new SOPCloseOption();
                option.CategroyName = strCategoryName;
                addOption = true;
            }

            int nCloseSOPWaitInputTime, nCloseSOPSensorResetWaitTime;
            int nUseCloseSOPWaitInputTime, nUseCloseSOPSensorReset, nUseCloseSOPSensorResetWaitTime;

            if (!int.TryParse(strCloseSOPWaitInputTime, out nCloseSOPWaitInputTime) || !int.TryParse(strCloseSOPSensorResetWaitTime, out nCloseSOPSensorResetWaitTime))
                return;

            if (!int.TryParse(strUseCloseSOPWaitInputTime, out nUseCloseSOPWaitInputTime) ||
                !int.TryParse(strUseCloseSOPSensorReset, out nUseCloseSOPSensorReset) ||
                !int.TryParse(strUseCloseSOPSensorResetWaitTime, out nUseCloseSOPSensorResetWaitTime))
                return;

            if (addOption)
                m_dicSOPAutoCloseOptions[strCategoryName] = option;
        }

        private int ProcessRequestSensorSOPPermit(Data.SOP.SOPClientData data, ArrayList arrDatas)
        {
            if (arrDatas == null || arrDatas.Count != 1)
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            if (arrDatas[0] is int)
            {
                // SOPSimulator로부터 센서신호에 의한 SOP 실행권한 요청이 왔다.
                // 일단, m_dicSensorSOPRequestList에 넣어놓고 Timer에서 처리하도록 한다.
                int nSensorZoneHistoryID = (int)arrDatas[0];

                List<int> sopGenUserIDs;

                if (m_dicSensorSOPRequestList.TryGetValue(nSensorZoneHistoryID, out sopGenUserIDs))
                {
                    sopGenUserIDs.Add(data.ID);
                }

                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessRequestComponentHistoryList(Data.SOP.SOPClientData data, ArrayList arrDatas)
        {
            if (arrDatas == null)
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            int nDataCount = arrDatas.Count;

            if (nDataCount % 2 > 0)
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            ArrayList arrResult = new ArrayList();

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int)
                {
                    int nActionStepHistoryID = (int)arrDatas[i];
                    int nMaxComponentHistoryID = (int)arrDatas[i + 1];
                    GetComponentHistory(arrResult, nActionStepHistoryID, nMaxComponentHistoryID);
                }
                else
                    return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
            }

            if (arrResult.Count > 0)
            {
                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrResult);
                SendClientData(SOPWebServer.Header.RESPONSE_COMPONENT_HISTORY_LIST, bytes, data);
            }

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private bool GetComponentHistory(ArrayList arrDatas, int nActionStepHistoryID, int nMaxComponentHistoryID)
        {
            bool hasData = false;
            ActionStepHistory actionStepHistory;

            if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
            {
                arrDatas.Add(nActionStepHistoryID);
                arrDatas.Add(0);
                int nComponentHistoryCountIndex = arrDatas.Count - 1;

                int nCount = actionStepHistory.ComponentHistories.Count;

                for (int i = 0; i < nCount; i++)
                {
                    ComponentHistory history = actionStepHistory.ComponentHistories[i];

                    if (history.ID <= nMaxComponentHistoryID)
                        continue;

                    history.AddSendData(arrDatas, nComponentHistoryCountIndex);
                }

                int nComponentHistoryCount = (int)arrDatas[nComponentHistoryCountIndex];

                if (nComponentHistoryCount == 0)
                {
                    arrDatas.RemoveAt(nComponentHistoryCountIndex);
                    arrDatas.RemoveAt(nComponentHistoryCountIndex - 1);
                    hasData = false;
                }
                else
                    hasData = true;
            }

            return hasData;
        }

        private int ProcessAlarmSOPResult(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int)
                {
                    int nSensorZoneHistoryID = (int)arrDatas[i];
                    int nResultType = (int)arrDatas[i + 1];

                    AlarmData alarm = AlarmManager.Instance.GetAlarm(nSensorZoneHistoryID);

                    if (alarm != null)
                        alarm.SOPProcess = (AlarmData.SOPProcessType)nResultType;
                }
                else
                    return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
            }

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessSOPSimulatorCommand(Client.ClientData data, int header, byte[] messages, ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count > 0 && arrDatas[0] is byte)
            {
                byte command = (byte)arrDatas[0];

                if (command == SOPWebServer.SOPSimulatorCommandType.RESET_USER_DEFINED_TEAM_NAMES)
                {
                    return ProcessRelay(header, messages, data);
                }

                return SOPWebServer.ErrorMessageType.UNKNOWN_COMMAND;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        // Client들에게 그대로 전달
        private int ProcessRelay(int header, byte[] messages, Client.ClientData exceptClient = null)
        {
            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
            // Timer를 사용하도록 한다.
            SendClientData(header, messages, SOPWebServer.ClientType.SOP_SIMULATOR, -1, exceptClient);
            /*List<Client.ClientData> clients = GetClientDatas();
            List<Client.ClientData> removeClients = new List<Client.ClientData>();

            foreach (Client.ClientData client in clients)
            {
                if (client == exceptClient)
                    continue;

                if (client.PostMan.ClientChannel.State == CommunicationState.Opened)
                    client.PostMan.OnRing(header, messages);
                else
                    removeClients.Add(client);
            }

            foreach (Client.ClientData client in removeClients)
            {
                RemoveClient(client);
            }

            removeClients.Clear();
            clients.Clear();*/

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        // 제어권 반납
        // 제어권은 반납한 Client를 제외한 다른 Client 가운데 하나에게 전달된다.
        // 만일, 다른 Client가 없다면 반납되지 않는다.
        private int ProcessReturnControl(Data.SOP.SOPClientData data, ArrayList arrDatas)
        {
            if (arrDatas.Count > 0 && arrDatas[0] is int)
            {
                Data.SOP.SOPClientData controlClient;
                int nActionStepHistoryID = (int)arrDatas[0];

                if (m_dicControlClient.TryGetValue(nActionStepHistoryID, out controlClient) == false || controlClient.ID != data.ID)
                    return SOPWebServer.ErrorMessageType.NO_PERMISSION;

                List<Client.ClientData> clients = GetClientDatas();

                List<Data.SOP.SOPClientData> clientList = new List<Data.SOP.SOPClientData>();

                foreach (Client.ClientData client in clients)
                {
                    if (client != data)
                        clientList.Add((Data.SOP.SOPClientData)client);
                }

                if (clients.Count == 0)
                    return SOPWebServer.ErrorMessageType.NO_OTHER_CLIENTS;

                controlClient = GetOptimalControlClient(nActionStepHistoryID, clientList);

                if (controlClient != null)
                {
                    SetControlClient(nActionStepHistoryID, controlClient);
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                    return SOPWebServer.ErrorMessageType.NO_OTHER_CLIENTS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        // 제어권 강제로 뺏기
        // 현재 제어권을 가진 Client로부터 제어권을 빼앗아 data에게 전달한다.
        private int ProcessStealControl(Data.SOP.SOPClientData data, ArrayList arrDatas)
        {
            if (arrDatas.Count > 0 && arrDatas[0] is int)
            {
                Data.SOP.SOPClientData controlClient;
                int nActionStepHistoryID = (int)arrDatas[0];

                if (m_dicControlClient.TryGetValue(nActionStepHistoryID, out controlClient))
                {
                    if (m_agent != null)
                    {
                        object result = m_agent.RunMethod(BaseAgent.MethodType.Etc, "StealSOPControl", m_dbMgr, nActionStepHistoryID, controlClient.ID, data.ID);

                        if (result != null && result is int)
                        {
                            int nResult = (int)result;

                            if (nResult == 1)
                                SetControlClient(nActionStepHistoryID, data);

                            return SOPWebServer.ErrorMessageType.SUCCESS;
                        }
                    }

                    SetControlClient(nActionStepHistoryID, data);
                }

                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessRejectRequestControl(Data.SOP.SOPClientData data, ArrayList arrDatas)
        {
            if (arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nActionStepHistoryID = (int)arrDatas[0];

                List<Client.ClientData> clients = GetClientDatas();

                for (int i = 1; i < arrDatas.Count; i++)
                {
                    if (arrDatas[i] is string)
                    {
                        string strUserID = (string)arrDatas[1];
                        Data.SOP.SOPClientData client = GetClientData(clients, strUserID);

                        if (client != null)
                            SendNoSOPControl(nActionStepHistoryID, client);
                    }
                }

                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessGiveControl(Data.SOP.SOPClientData data, ArrayList arrDatas)
        {
            if (arrDatas.Count >= 2 && arrDatas[0] is int && arrDatas[1] is int)
            {
                int nActionStepHistoryID = (int)arrDatas[0];
                int nUserIDCount = (int)arrDatas[1];

                List<Client.ClientData> clients = GetClientDatas();

                if (nUserIDCount > 0 && arrDatas.Count >= 3 && arrDatas[2] is string)
                {
                    string strControlUserID = (string)arrDatas[2];
                    Data.SOP.SOPClientData controlClient = GetClientData(clients, strControlUserID);

                    if (controlClient != null)
                    {
                        SetControlClient(nActionStepHistoryID, controlClient);
                    }

                    for (int i = 1; i < nUserIDCount; i++)
                    {
                        int nIndex = 2 + i;

                        if (nIndex >= arrDatas.Count)
                            break;

                        if (arrDatas[nIndex] is string)
                        {
                            string strUserID = (string)arrDatas[nIndex];
                            Data.SOP.SOPClientData client = GetClientData(clients, strControlUserID);

                            if (client != null)
                                SendNoSOPControl(nActionStepHistoryID, client);
                        }
                    }
                }

                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessCancelRequestControl(Data.SOP.SOPClientData data, ArrayList arrDatas)
        {
            if (arrDatas.Count > 0 && arrDatas[0] is int)
            {
                int nActionStepHistoryID = (int)arrDatas[0];
                Data.SOP.SOPClientData controlClient;

                if (m_dicControlClient.TryGetValue(nActionStepHistoryID, out controlClient))
                {
                    arrDatas = new ArrayList();
                    arrDatas.Add(nActionStepHistoryID);
                    arrDatas.Add(data.UserID);

                    byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                    SendClientData(SOPWebServer.Header.CANCEL_REQUEST_CONTROL, bytes, controlClient);
                }

                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessRequestControl(Data.SOP.SOPClientData data, ArrayList arrDatas)
        {
            if (data.ID < 0)
                return SOPWebServer.ErrorMessageType.NO_PERMISSION;

            if (arrDatas.Count > 0 && arrDatas[0] is int)
            {
                Data.SOP.SOPClientData controlClient;
                int nControlClientLevel = -1;
                int nControlClientID = -1;
                int nActionStepHistoryID = (int)arrDatas[0];

                if (m_dicControlClient.TryGetValue(nActionStepHistoryID, out controlClient))
                {
                    nControlClientLevel = controlClient.UserLevel;
                    nControlClientID = controlClient.ID;
                }

                if (m_agent != null)
                {
                    object result = m_agent.RunMethod(BaseAgent.MethodType.Etc, "RequestSOPControl", m_dbMgr, nActionStepHistoryID, nControlClientID, nControlClientLevel, data.ID, data.UserLevel);

                    if (result != null && result is int)
                    {
                        int nResult = (int)result;

                        if (nResult == 1)
                        {
                            // 제어권 전송
                            SetControlClient(nActionStepHistoryID, data);
                        }
                        else if (nResult == 0)
                        {
                            // 제어권 허가 못함
                            SendNoSOPControl(nActionStepHistoryID, data);
                        }
                        else// if (nResult < 0)
                        {
                            if (controlClient != null)
                            {
                                // 제어권을 가진 User에게 맡김
                                SendRequestSOPControl(nActionStepHistoryID, controlClient, data);
                            }
                        }

                        return SOPWebServer.ErrorMessageType.SUCCESS;
                    }
                }

                // Defulat
                if (nControlClientID < 0)
                {
                    // 제어권 전송
                    SetControlClient(nActionStepHistoryID, data);
                }
                else if (controlClient != null)
                {
                    // 제어권을 가진 User에게 맡김
                    SendRequestSOPControl(nActionStepHistoryID, controlClient, data);
                }

                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        /*private int ProcessNewSOP(ClientData data, ArrayList arrDatas)
        {
            if (arrDatas.Count == 4 && arrDatas[0] is int && arrDatas[1] is bool && arrDatas[2] is int && arrDatas[3] is int)
            {
                // SOPSimulator는 서버에 NewSOP 신호를 보낸 이후에 ActionStepHistory를 DB에 기록한다.
                // 즉, 서버에 보낼 당시에는 아직 ActionStepHistoryID가 생성되기 전이다.
                int nActionStepID = (int)arrDatas[0];
                bool isRealMode = (bool)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                int nMaxActionStepHistoryID = (int)arrDatas[3];

                ArrayList arr = new ArrayList();
                arr.Add(nActionStepID);
                arr.Add(isRealMode ? 1 : 0);
                arr.Add(nSOPGenUserID);
                arr.Add(nMaxActionStepHistoryID);
                arr.Add(data);

                // 쓰레드에서 ActionStepHistory가 생성될때까지 감시한다.
                Thread t = new Thread(new ParameterizedThreadStart(NewSOPThread));
                t.Start(arr);
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private void NewSOPThread(object arg)
        {
            ArrayList arrDatas = (ArrayList)arg;

            int nActionStepID = (int)arrDatas[0];
            int nRealMode = (int)arrDatas[1];
            int nSOPGenUserID = (int)arrDatas[2];
            int nMaxActionStepHistoryID = (int)arrDatas[3];
            ClientData data = (ClientData)arrDatas[4];

            // 5초 이내에 ActionStepHistory가 생성되지 않으면 무시한다.
            int nLimit = 50;

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            for (int i=0;i<nLimit;i++)
            {
                string strSQL = string.Format("Select ID from ActionStepHistory where ID > {0} and ActionStepID = {1} and RealMode = {2} and LastAccessedUserID = {3}", nMaxActionStepHistoryID, nActionStepID, nRealMode, nSOPGenUserID);
                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                {
                    break;
                }

                if (arrResult.Count > 0)
                {
                    VariousData<int> actionStepHistoryID = WebDBManager.GetIntField(arrResult[0].ToString());

                    if (actionStepHistoryID != null)
                    {
                        ActionStepHistory actionStepHistory = new ActionStepHistory();
                        actionStepHistory.ID = actionStepHistoryID.Data;
                        actionStepHistory.LastControlCheck = DateTime.Now;
                        m_dicActionStepHistory[actionStepHistoryID.Data] = actionStepHistory;

                        arrDatas.Clear();
                        arrDatas.Add(nActionStepID);
                        arrDatas.Add(nRealMode == 1);
                        arrDatas.Add(nSOPGenUserID);
                        arrDatas.Add(actionStepHistoryID.Data);

                        byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                        SendClientData(SOPWebServer.Header.CONFIRM_NEW_SOP, bytes, data);
                        break;
                    }
                }

                Thread.Sleep(100);
            }

            dbMgr.Close();
        }

        private int ProcessConfirmSOPControl(ClientData data, ArrayList arrDatas)
        {
            if (arrDatas.Count > 0)
            {
                for (int i=0;i<arrDatas.Count;i++)
                {
                    if (arrDatas[i] is int)
                    {
                        int nActionStepHistoryID = (int)arrDatas[i];
                        SetControlClient(nActionStepHistoryID, data);
                    }
                }

                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }*/

        private int ProcessClientInfo(Data.SOP.SOPClientData data, ArrayList arrDatas)
        {
            if (data != null && arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string && arrDatas[3] is int)
            {
                int nSOPGenUserID = (int)arrDatas[0];
                string strUserID = (string)arrDatas[1];
                string strNickName = (string)arrDatas[2];
                int nUserLevel = (int)arrDatas[3];

                data.ID = nSOPGenUserID;
                data.UserID = strUserID;
                data.NickName = strNickName;
                data.UserLevel = nUserLevel;

                /*int nControlUserID;
                List<int> actionStepHistoryIDs = m_dicLastControlUserID.Keys.ToList();
                List<int> controlActionStepHistoryIDs = new List<int>();

                foreach (int nActionStepHistoryID in actionStepHistoryIDs)
                {
                    if (m_dicLastControlUserID.TryGetValue(nActionStepHistoryID, out nControlUserID) && nControlUserID == nSOPGenUserID)
                    {
                        if (m_dicControlClient.ContainsKey(nActionStepHistoryID) == false)
                        {
                            controlActionStepHistoryIDs.Add(nActionStepHistoryID);
                        }
                    }
                }

                SetControlClient(controlActionStepHistoryIDs, data);*/
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private Data.SOP.SOPClientData GetOptimalControlClient(int nActionStepHistoryID, List<Data.SOP.SOPClientData> clientList)
        {
            if (m_agent != null)
            {
                List<int> sopGenUserIDs = new List<int>();

                foreach (Data.SOP.SOPClientData data in clientList)
                {
                    sopGenUserIDs.Add(data.ID);
                }

                object result = m_agent.RunMethod(BaseAgent.MethodType.Etc, "GetOptimalControlClient", nActionStepHistoryID, sopGenUserIDs);

                if (result != null && result is int)
                {
                    int nIndex = (int)result;

                    if (nIndex < 0)
                        return null;
                    else
                        return clientList[nIndex];
                }
            }

            // agent로부터 응답이 없으면 첫번째 클라이언트가 제어권을 가진다.
            return clientList[0];
        }

        private Data.SOP.SOPClientData GetClientData(List<Client.ClientData> clients, int nSOPGenUserID)
        {
            foreach (Client.ClientData data in clients)
            {
                if (data is Data.SOP.SOPClientData)
                {
                    Data.SOP.SOPClientData client = (Data.SOP.SOPClientData)data;

                    if (client.ID == nSOPGenUserID)
                        return client;
                }
            }

            return null;
        }

        private Data.SOP.SOPClientData GetClientData(List<Client.ClientData> clients, string strUserID)
        {
            foreach (Client.ClientData data in clients)
            {
                if (data is Data.SOP.SOPClientData)
                {
                    Data.SOP.SOPClientData client = (Data.SOP.SOPClientData)data;

                    if (client.UserID == strUserID)
                        return client;
                }
            }

            return null;
        }

        private void SendNoSOPControl(int nActionStepHistoryID, Data.SOP.SOPClientData data)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nActionStepHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.REJECT_REQUEST_CONTROL, bytes, data);
        }

        private void SendRequestSOPControl(int nActionStepHistoryID, Data.SOP.SOPClientData controlClient, Data.SOP.SOPClientData requestClient)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nActionStepHistoryID);
            arrDatas.Add(requestClient.UserID);
            arrDatas.Add(requestClient.NickName);
            arrDatas.Add(requestClient.IP);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.REQUEST_CONTROL, bytes, controlClient);
        }

        private bool SetControlClient(int nActionStepHistoryID, Data.SOP.SOPClientData data)
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return false;

            string strSQL = string.Format("Update ActionStepHistory set LastAccessedUserID = {0} where ID = {1}", data.ID, nActionStepHistoryID);
            bool result = false;

            if (dbMgr.GetResultData(strSQL) != null)
            {
                ActionStepHistory actionStepHistory;

                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                {
                    actionStepHistory.LastAccessedUserID = data.ID;
                    actionStepHistory.LastControlCheck = DateTime.Now;
                    m_dicControlClient[nActionStepHistoryID] = data;
                    result = true;
                }
            }

            dbMgr.Close();
            return result;
        }

        /*private void RemoveControl(List<int> actionStepHistoryIDs)
        {
            ClientData data;

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                m_dicControlClient.TryRemove(nActionStepHistoryID, out data);
            }
        }*/

        /*private void SetControlClient(ClientData data)
        {
            if (data == null)
            {
                if (m_nLastControlUserID > 0)
                {
                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                        return;

                    string strSQL = "Update ControlUser set UserID = NULL where SiteID = " + dbMgr.SiteID.ToString();
                    dbMgr.GetResultData(strSQL);
                    dbMgr.Close();
                }

                m_nLastControlUserID = -1;
                m_controlClient = data;
            }
            else
            {
                if (m_nLastControlUserID != data.ID)
                {
                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                        return;

                    string strSQL = string.Format("Update ControlUser set UserID = {0} where SiteID = {1}", data.ID, dbMgr.SiteID);

                    if (dbMgr.GetResultData(strSQL) == null)
                    {
                        // 데이터가 존재하지 않는 경우
                        strSQL = string.Format("Insert into ControlUser(ID, UserID, SiteID) values((select isnull(max(id), 0) + 1 from ControlUser), {0}, {1})", data.ID, dbMgr.SiteID);

                        if (dbMgr.GetResultData(strSQL) == null)
                        {
                            m_nLastControlUserID = -1;
                            m_controlClient = null;
                        }
                        else
                        {
                            m_nLastControlUserID = data.ID;
                            m_controlClient = data;
                            m_dtLastControlCheck = DateTime.Now;
                        }
                    }
                    else
                    {
                        m_nLastControlUserID = data.ID;
                        m_controlClient = data;
                        m_dtLastControlCheck = DateTime.Now;
                    }

                    dbMgr.Close();
                }
                else
                {
                    m_controlClient = data;
                    m_dtLastControlCheck = DateTime.Now;
                }
            }
        }*/

        private int SendDataToSOPSimulator(int header, ArrayList arrDatas)
        {
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(header, bytes, SOPWebServer.ClientType.SOP_SIMULATOR, SOPWebServer.ClientSubType.SOP_SIMULATOR);
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        protected override void OnTimerEvent()
        {
            base.OnTimerEvent();

            // 센서신호로 실행된 ActionStepHistory에 변동사항이 있는지 확인한다.
            CheckSensorActionStepHistory();
            // 종료된 ActionStepHistory가 있는지 확인한다.
            CheckClosedActionStepHistory();
            // 종료된 후 1분이 지난 ActionStepHistory를 삭제한다.
            CheckOldActionStepHistory();
            // 새로운 ActionStepHistory가 생겼는지 확인한다.
            CheckNewActionStepHistory();
            // 새로운 ComponentHistory가 생겼는지 확인한다.
            CheckNewComponentHistory();

            List<Client.ClientData> clients = GetClientDatas();

            if (clients.Count == 0)
            {
                //SetControlClient(null, null);
                return;
            }

            // 센서신호에 의한 SOP 실행권한 요청이 있는지 확인한다.
            //byte[] sensorSOPBytes = CheckSensorSOPRequest();

            List<AlarmData> alarms = AlarmManager.Instance.CurrentAlarms;
            // SOP 처리가 되지 않은 알람들에 대한 byte Array를 생성한다.
            byte[] sopBytes = m_alarmSOPManager.MakeAlarmDataList(alarms, clients, m_dbMgr);
            //byte[] sopBytes = MakeAlarmDataList(alarms);

            // SOP 실행요청
            byte[] sopRequestBytes = m_alarmSOPManager.MakeSOPDataList(m_sopRequests, clients, m_dbMgr);

            ArrayList arrDatas = CheckControlClient(clients);
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            List<Client.ClientData> removeClients = new List<Client.ClientData>();

            foreach (Client.ClientData client in clients)
            {
                IClientChannel channel = client.PostMan.ClientChannel;

                if (channel.State == CommunicationState.Opened)
                {
                    if (((Data.SOP.SOPClientData)client).ID < 0)
                    {
                        SendClientData(SOPWebServer.Header.REQUEST_CLIENT_INFO, null, client);
                    }

                    client.PostMan.OnRing(SOPWebServer.Header.CONTROL_CLIENT, bytes);

                    // 아직 SOP 처리가 되지 않은 알람들에 대한 정보를 SOP Simulator에게 알려준다.
                    if (sopBytes != null)
                        client.PostMan.OnRing(SOPWebServer.Header.ALARM_DATA_LIST2, sopBytes);

                    // SOP 실행요청
                    if (sopRequestBytes != null)
                        client.PostMan.OnRing(SOPWebServer.Header.RUN_SOP, sopRequestBytes);

                    /*if (sensorSOPBytes != null)
                        client.PostMan.OnRing(SOPWebServer.Header.RESPONSE_SENSOR_SOP_PERMIT, sensorSOPBytes);*/
                }
                else
                {
                    removeClients.Add(client);
                }
            }

            // 제어권 소유자 가운데 통신이 끊어진 클라이언트가 있는지 확인한다.
            Data.SOP.SOPClientData data;
            List<int> actionStepHistoryIDs = m_dicControlClient.Keys.ToList();

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicControlClient.TryGetValue(nActionStepHistoryID, out data) && removeClients.Contains(data))
                {
                    RemoveControlClient(nActionStepHistoryID);
                }
            }

            foreach (Client.ClientData client in removeClients)
            {
                RemoveClient(client);
            }

            removeClients.Clear();
            clients.Clear();

            // 종료된 SOP가 있는지 확인한다.
            //CheckCloseSOP(actionStepHistoryIDs);
        }

        // 센서신호로 실행된 ActionStepHistory에 변동사항이 있는지 확인한다.
        private void CheckSensorActionStepHistory()
        {
            ActionStepHistory actionStepHistory;
            List<int> actionStepHistoryIDs = m_dicActionStepHistory.Keys.ToList();

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                {
                    if (actionStepHistory.EndTime == null && actionStepHistory.CancelTime == null && actionStepHistory.SensorZoneHistoryID > 0)
                    {
                        if (AlarmManager.Instance.GetAlarm(actionStepHistory.SensorZoneHistoryID) == null)
                        {
                            // 이미 종료된 알람
                            ActionStepHistory temp;

                            if (m_dicActionStepHistory.TryRemove(nActionStepHistoryID, out temp))
                            {
                                CancelActionStepHistory(nActionStepHistoryID);
                            }
                        }
                    }
                }
            }
        }

        private void CancelActionStepHistory(int nActionStepHistoryID)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strSQL = "Update ActionStepHistory set CancelTime = '" + strTime + "' where ID = " + nActionStepHistoryID;

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            dbMgr.GetResultData(strSQL);
            dbMgr.Close();
        }

        // 센서신호에 의한 SOP 실행권한 요청이 있는지 확인한다.
        /*private byte[] CheckSensorSOPRequest()
        {
            List<int> sensorZoneHistoryIDs = m_dicSensorSOPRequestList.Keys.ToList();
            List<int> sopGenUserIDs;

            ArrayList arrDatas = new ArrayList();

            foreach (int nSensorZoneHistoryID in sensorZoneHistoryIDs)
            {
                if (m_dicSensorSOPRequestList.TryGetValue(nSensorZoneHistoryID, out sopGenUserIDs))
                {
                    if (sopGenUserIDs.Count > 0 && sopGenUserIDs[0] > 0)
                    {
                        arrDatas.Add(nSensorZoneHistoryID);
                        arrDatas.Add(sopGenUserIDs[0]);

                        // nSensorZoneHistoryID에 대한 처리는 끝났음을 표시한다.
                        sopGenUserIDs[0] = -sopGenUserIDs[0];
                    }
                }
            }

            if (arrDatas.Count > 0)
            {
                return SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            }

            return null;
        }*/

        // 종료된 ActionStepHistory가 있는지 확인한다.
        private void CheckClosedActionStepHistory()
        {
            ActionStepHistory actionStepHistory;
            List<int> actionStepHistoryIDs = m_dicActionStepHistory.Keys.ToList();

            string strIDs = "";

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                {
                    if (actionStepHistory.EndTime == null && actionStepHistory.CancelTime == null)
                    {
                        if (strIDs.Length == 0)
                            strIDs = nActionStepHistoryID.ToString();
                        else
                            strIDs += ", " + nActionStepHistoryID.ToString();
                    }
                }
            }

            if (strIDs.Length == 0)
                return;

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            string strSQL = "Select ash.ID, ash.EndTime, ash.CancelTime, dc.CategoryName, sdc.SubCategoryName, d.DisasterName ";
            strSQL += "FROM ActionStepHistory as ash, ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += "where ash.ActionStepID = step.ID and step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and (ash.EndTime is not null or ash.CancelTime is not null) and ash.ID in (" + strIDs + ")";
            //string strSQL = "Select ID, EndTime, CancelTime from ActionStepHistory where (EndTime is not null or CancelTime is not null) and ID in (" + strIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            dbMgr.Close();

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> endTime = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                VariousData<DateTime> cancelTime = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                string strDisasterCategoryName = WebDBManager.GetStringField(arrResult[i + 3]);
                string strSubDisasterCategoryName = WebDBManager.GetStringField(arrResult[i + 4]);
                string strDisasterName = WebDBManager.GetStringField(arrResult[i + 5]);

                if (id == null || strDisasterCategoryName == null || strSubDisasterCategoryName == null || strDisasterName == null)
                    continue;

                if (m_dicActionStepHistory.TryGetValue(id.Data, out actionStepHistory))
                {
                    actionStepHistory.EndTime = endTime;
                    actionStepHistory.CancelTime = cancelTime;

                    if (IsEarthquakeSOP(strDisasterCategoryName, strSubDisasterCategoryName))
                        EarthquakeSensorServer.Instance.OnFinishSOP(strDisasterCategoryName, strSubDisasterCategoryName, strDisasterName);

                    RemoveActionStepHistory(id.Data);
                }
            }
        }

        // 종료된 후 1분이 지난 ActionStepHistory를 삭제한다.
        private void CheckOldActionStepHistory()
        {
            ActionStepHistory actionStepHistory;
            List<int> temp;
            List<int> actionStepHistoryIDs = m_dicActionStepHistory.Keys.ToList();

            Data.SOP.SOPClientData client;
            DateTime dtNow = DateTime.Now;

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                {
                    if (OneMinutePass(actionStepHistory.EndTime, dtNow) || OneMinutePass(actionStepHistory.CancelTime, dtNow))
                    {
                        RemoveActionStepHistory(nActionStepHistoryID);
                        //m_dicActionStepHistory.TryRemove(nActionStepHistoryID, out actionStepHistory);
                        // ActionStepHistory가 삭제되므로 관련된 제어권도 같이 삭제한다.
                        m_dicControlClient.TryRemove(nActionStepHistoryID, out client);

                        if (actionStepHistory.SensorZoneHistoryID > 0)
                            m_dicSensorSOPRequestList.TryRemove(actionStepHistory.SensorZoneHistoryID, out temp);
                    }
                }
            }
        }

        // 1분이 경과했는가?
        private bool OneMinutePass(VariousData<DateTime> data, DateTime timeStamp)
        {
            if (data == null)
                return false;

            TimeSpan span = timeStamp - data.Data;
            return span.TotalSeconds >= 60;
        }

        // ComponentHistory가 생성된후 ComponentHistoryDetail 생성까지 완료된 상태인가?
        private bool IsCompleteStatus(int nStatus)
        {
            if ((nStatus & 0x100) == 0x100)
                return false;

            return true;
        }

        private void CheckNewComponentHistory()
        {
            ActionStepHistory actionStepHistory;
            List<int> actionStepHistoryIDs = m_dicActionStepHistory.Keys.ToList();

            if (actionStepHistoryIDs.Count == 0)
                return;

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            Dictionary<int, ComponentHistory> dicComponentHistories = new Dictionary<int, ComponentHistory>();
            string strComponentHistoryIDs = "";

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                {
                    string strSQL = "Select ID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, ShowBoard, AccessedUserID, ";
                    strSQL += string.Format("CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete from ComponentHistory where ActionStepHistoryID = {0} and ID > {1}",
                        nActionStepHistoryID, actionStepHistory.MaxComponentHistoryID.ToString());

                    ArrayList arrResult = dbMgr.GetResultData(strSQL);

                    if (arrResult == null)
                        break;

                    int nResultCount = arrResult.Count;

                    for (int i = 0; i < nResultCount - 12; i += 13)
                    {
                        VariousData<int> componentHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());
                        VariousData<int> componentID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                        VariousData<int> componentType = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                        VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                        VariousData<int> status = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                        string strTask = WebDBManager.GetStringField(arrResult[i + 5]);
                        VariousData<int> completeCount = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                        VariousData<int> showBoard = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                        VariousData<int> accessedUserID = WebDBManager.GetIntField(arrResult[i + 8].ToString());
                        VariousData<int> checkedNotify1 = WebDBManager.GetIntField(arrResult[i + 9].ToString());
                        VariousData<int> checkedNotify2 = WebDBManager.GetIntField(arrResult[i + 10].ToString());
                        VariousData<int> checkedRun = WebDBManager.GetIntField(arrResult[i + 11].ToString());
                        VariousData<int> checkedComplete = WebDBManager.GetIntField(arrResult[i + 12].ToString());

                        if (componentHistoryID == null || componentID == null || componentType == null || timeStamp == null ||
                            status == null || accessedUserID == null)
                            continue;

                        if (IsCompleteStatus(status.Data) == false)
                        {
                            // ComponentHistory가 아직 완성되지 않은 상태라면 다음에 다시 읽도록 한다.
                            break;
                        }

                        ComponentHistory componentHistory = new ComponentHistory();

                        componentHistory.ID = componentHistoryID.Data;
                        componentHistory.ComponentID = componentID.Data;
                        componentHistory.ComponentType = componentType.Data;
                        componentHistory.TimeStamp = timeStamp.Data;
                        componentHistory.Status = status.Data;
                        componentHistory.Task = strTask;
                        componentHistory.CompleteCount = completeCount;
                        componentHistory.ShowBoard = showBoard;
                        componentHistory.AccessedUserID = accessedUserID.Data;
                        componentHistory.CheckedNotify1 = checkedNotify1;
                        componentHistory.CheckedNotify2 = checkedNotify2;
                        componentHistory.CheckedRun = checkedRun;
                        componentHistory.CheckedComplete = checkedComplete;

                        actionStepHistory.ComponentHistories.Add(componentHistory);
                        dicComponentHistories[componentHistoryID.Data] = componentHistory;

                        if (strComponentHistoryIDs.Length == 0)
                            strComponentHistoryIDs = componentHistoryID.Data.ToString();
                        else
                            strComponentHistoryIDs += ", " + componentHistoryID.Data.ToString();

                        if (actionStepHistory.MaxComponentHistoryID < componentHistoryID.Data)
                            actionStepHistory.MaxComponentHistoryID = componentHistoryID.Data;
                    }
                }
            }

            if (strComponentHistoryIDs.Length > 0)
            {
                string strSQL = "Select ComponentHistoryID, DataIndex, Datai, Dataf, Datas, Time from ComponentHistoryDetail where ComponentHistoryID in (" + strComponentHistoryIDs + ")";
                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                ComponentHistory componentHistory;

                if (arrResult != null)
                {
                    int nResultCount = arrResult.Count;

                    for (int i = 0; i < nResultCount - 5; i += 6)
                    {
                        VariousData<int> componentHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());
                        VariousData<int> dataIndex = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                        VariousData<int> datai = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                        VariousData<float> dataf = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                        string datas = WebDBManager.GetStringField(arrResult[i + 4]);
                        VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 5]);

                        if (componentHistoryID == null || dataIndex == null)
                            continue;

                        if (dicComponentHistories.TryGetValue(componentHistoryID.Data, out componentHistory) == false)
                            continue;

                        ComponentHistoryDetail detail = new ComponentHistoryDetail();

                        detail.DataIndex = dataIndex.Data;
                        detail.Datai = datai;
                        detail.Dataf = dataf;
                        detail.Datas = datas;
                        detail.TimeStamp = timeStamp;

                        componentHistory.DetailDatas.Add(detail);
                    }
                }
            }

            dbMgr.Close();
        }

        private void CheckNewActionStepHistory()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            string strSQL = "SELECT ash.id, ash.ActionStepID, ash.BeginTime, ash.DetectTime, ash.RealMode, ash.LastAccessedUserID, ash.SensorZoneHistoryID, ash.Position, ash.SelectedComponentID, ash.SelectedComponentType, ash.StartOption, ash.DisasterOption, dc.CategoryName, sdc.SubCategoryName, d.DisasterName ";
            strSQL += "FROM ActionStepHistory as ash, ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += string.Format(" where ash.ActionStepID = step.ID and step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and ash.EndTime is null AND ash.CancelTime is NULL and ash.ID > {0}", m_nMaxActionStepHistoryID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            dbMgr.Close();

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 14; i += 15)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<DateTime> dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                VariousData<DateTime> dtDetect = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                VariousData<int> realMode = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> userID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                int nSensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                string strPosition = WebDBManager.GetStringField(arrResult[i + 7]);
                VariousData<int> selectedComponentID = WebDBManager.GetIntField(arrResult[i + 8].ToString());
                VariousData<int> selectedComponentType = WebDBManager.GetIntField(arrResult[i + 9].ToString());
                VariousData<int> startOption = WebDBManager.GetIntField(arrResult[i + 10].ToString());
                string strDisasterOption = WebDBManager.GetStringField(arrResult[i + 11]);
                string strDisasterCategoryName = WebDBManager.GetStringField(arrResult[i + 12]);
                string strSubDisasterCategoryName = WebDBManager.GetStringField(arrResult[i + 13]);
                string strDisasterName = WebDBManager.GetStringField(arrResult[i + 14]);

                if (id == null || actionStepID == null || dtBegin == null || realMode == null ||
                    strDisasterCategoryName == null || strSubDisasterCategoryName == null ||
                    strDisasterName == null)
                    continue;

                ActionStepHistory actionStepHistory = new ActionStepHistory();

                actionStepHistory.ID = id.Data;
                actionStepHistory.ActionStepID = actionStepID.Data;
                actionStepHistory.BeginTime = dtBegin.Data;
                actionStepHistory.DetectTime = dtDetect;
                actionStepHistory.IsRealMode = realMode.Data != 0;
                actionStepHistory.SensorZoneHistoryID = nSensorZoneHistoryID;
                actionStepHistory.Position = strPosition;
                actionStepHistory.SelectedComponentID = selectedComponentID;
                actionStepHistory.SelectedComponentType = selectedComponentType;
                actionStepHistory.StartOption = startOption;
                actionStepHistory.DisasterOption = strDisasterOption;
                actionStepHistory.LastControlCheck = DateTime.Now;

                if (userID != null)
                    actionStepHistory.LastAccessedUserID = userID.Data;

                if (nSensorZoneHistoryID > 0)
                {
                    AlarmData alarm = AlarmManager.Instance.GetAlarm(nSensorZoneHistoryID);

                    if (alarm != null)
                    {
                        alarm.SOPProcess = AlarmData.SOPProcessType.Run;
                    }
                }

                AddActionStepHistory(actionStepHistory);
                //m_dicActionStepHistory[actionStepHistory.ID] = actionStepHistory;

                if (IsEarthquakeSOP(strDisasterCategoryName, strSubDisasterCategoryName))
                    EarthquakeSensorServer.Instance.OnBeginSOP(strDisasterCategoryName, strSubDisasterCategoryName, strDisasterName);

                if (m_nMaxActionStepHistoryID < id.Data)
                {
                    SetMaxActionStepHistoryID(id.Data);
                    //m_nMaxActionStepHistoryID = id.Data;
                }
            }
        }

        private bool IsEarthquakeSOP(string strDisasterCategoryName, string strSubDisasterCategoryName)
        {
            return strDisasterCategoryName.Contains("지진") || strSubDisasterCategoryName.Contains("지진");
        }

        private void AddActionStepHistory(ActionStepHistory actionStepHistory)
        {
            m_dicActionStepHistory[actionStepHistory.ID] = actionStepHistory;
        }

        private void RemoveActionStepHistory(int nActionStepHistoryID)
        {
            ActionStepHistory temp;
            m_dicActionStepHistory.TryRemove(nActionStepHistoryID, out temp);
        }

        private void SetMaxActionStepHistoryID(int nID)
        {
            bool noActionStepHistory;
            bool isClosed = IsCloseSOP(nID, out noActionStepHistory);
            bool exist = m_dicActionStepHistory.ContainsKey(nID);

            if (noActionStepHistory)
                System.Diagnostics.Trace.WriteLine("no ActionStepHistory : " + nID);

            if (isClosed == false)
            {
                if (exist == false)
                    System.Diagnostics.Trace.WriteLine("not Closed & not Exist : " + nID);
            }

            m_nMaxActionStepHistoryID = nID;
        }

        private bool IsCloseSOP(int nActionStepHistoryID, out bool noExist)
        {
            noExist = false;
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return false;

            string strSQL = "Select EndTime, CancelTime from ActionStepHistory where ID = " + nActionStepHistoryID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            dbMgr.Close();

            if (arrResult == null)
                return false;
            else if (arrResult.Count < 2)
            {
                noExist = true;
                return false;
            }

            VariousData<DateTime> endTime = WebDBManager.GetDateTimeField(arrResult[0]);
            VariousData<DateTime> cancelTime = WebDBManager.GetDateTimeField(arrResult[1]);

            if (endTime != null || cancelTime != null)
                return true;

            return false;
        }

        // 종료된 SOP가 있는지 확인한다.
        /*private void CheckCloseSOP(List<int> actionStepHistoryIDs)
        {
            string strIDs = "";

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (strIDs.Length == 0)
                    strIDs = nActionStepHistoryID.ToString();
                else
                    strIDs += ", " + nActionStepHistoryID.ToString();
            }

            if (strIDs.Length == 0)
                return;

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            string strSQL = "Select ID from ActionStepHistory where (EndTime is not NULL or CancelTime is not NULL) and ID in (" + strIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            dbMgr.Close();

            if (arrResult == null)
                return;

            ActionStepHistory actionStepHistory;
            ClientData client;
            int nResultCount = arrResult.Count;

            foreach (object data in arrResult)
            {
                VariousData<int> id = WebDBManager.GetIntField(data.ToString());

                if (id == null)
                    continue;

                m_dicActionStepHistory.TryRemove(id.Data, out actionStepHistory);
                m_dicControlClient.TryRemove(id.Data, out client);
            }
        }*/

        // SOP 제어권에 대한 소유자를 삭제하고, 마지막 소유했던 유저 정보를 기록한다.
        private void RemoveControlClient(int nActionStepHistoryID)
        {
            Data.SOP.SOPClientData data;

            if (m_dicControlClient.TryRemove(nActionStepHistoryID, out data))
            {
                ActionStepHistory actionStepHistory;

                if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                {
                    actionStepHistory.LastAccessedUserID = data.ID;
                }
            }
        }

        // SOP 처리가 되지 않은 알람들에 대한 byte Array를 생성한다.
        private byte[] MakeAlarmDataList(List<AlarmData> alarms)
        {
            ArrayList arrDatas = null;
            List<int> sopGenUserIDs;

            foreach (AlarmData alarm in alarms)
            {
                if (alarm.SOPProcess != AlarmData.SOPProcessType.None)
                    continue;

                if (alarm.SensorZoneHistoryID > 0)
                {
                    if (m_dicSensorSOPRequestList.ContainsKey(alarm.SensorZoneHistoryID) == false)
                    {
                        m_dicSensorSOPRequestList[alarm.SensorZoneHistoryID] = new List<int>();
                    }
                }

                if (arrDatas == null)
                    arrDatas = new ArrayList();

                int nZoneID, nEquipZoneID;
                GetZoneNEquipZoneIDFromAlarm(alarm, out nZoneID, out nEquipZoneID);

                arrDatas.Add((int)alarm.SensorType);
                arrDatas.Add(nEquipZoneID);
                arrDatas.Add(nZoneID);
                arrDatas.Add(alarm.TimeStamp.ToBinary());
                arrDatas.Add(alarm.SensorZoneID);
                arrDatas.Add(alarm.SensorZoneHistoryID);

                // SOP를 실행시킬 User ID
                if (alarm.SensorZoneHistoryID > 0 && m_dicSensorSOPRequestList.TryGetValue(alarm.SensorZoneHistoryID, out sopGenUserIDs) && sopGenUserIDs.Count > 0)
                {
                    arrDatas.Add(sopGenUserIDs[0]);
                }
                else
                    arrDatas.Add(-1);
            }

            if (arrDatas == null)
                return null;

            return SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
        }

        private void GetZoneNEquipZoneIDFromAlarm(AlarmData alarm, out int nZoneID, out int nEquipZoneID)
        {
            nZoneID = nEquipZoneID = -1;

            BaseSMSManager.SMSMessageType messageType = ServerProcess.Data.SMSManager.ReactionTypeToMessageType(alarm.Status, alarm.SensorType);

            if (messageType == BaseSMSManager.SMSMessageType.REPORT_FIRE && alarm.SensorZoneID >= SOPWebServer.Header.ManualReportDefaultID)
            {
                // 수동화재신고
                nZoneID = -1;
                int.TryParse(alarm.ReactionHistoryParam1, out nZoneID);
            }
            else
            {
                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(alarm.SensorZoneID);

                if (group != null && group.EquipmentZone != null)
                {
                    nEquipZoneID = group.EquipmentZone.ID;

                    if (group.EquipmentZone.LinkedZoneList.Count > 0)
                    {
                        UnE.Spatial.Zone zone = (UnE.Spatial.Zone)group.EquipmentZone.LinkedZoneList[0];
                        nZoneID = zone.ID;
                    }
                }
            }
        }

        protected override void RemoveNotConnectedClients()
        {
            // 다른 서버들과 달리 SOPSimulatorServer는 OnTimerEvent에서 직접 Client들의 접속여부를 확인한다.
        }

        // SOP 제어권의 원래 소유자가 접속중인지 검사한다.
        private void CheckOriginalSOPControl(List<int> actionStepHistoryIDs, List<Client.ClientData> clients)
        {
            Data.SOP.SOPClientData temp;
            ActionStepHistory actionStepHistory;

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicControlClient.TryGetValue(nActionStepHistoryID, out temp) == false)
                {
                    if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                    {
                        foreach (Client.ClientData client in clients)
                        {
                            Data.SOP.SOPClientData _client = (Data.SOP.SOPClientData)client;

                            // SOP 제어권의 원래 소유자가 접속중이면 즉시 제어권을 부여한다.
                            if (_client.ID == actionStepHistory.LastAccessedUserID)
                            {
                                m_dicControlClient[nActionStepHistoryID] = _client;
                                actionStepHistory.LastControlCheck = DateTime.Now;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // SOP 제어권의 원래 소유자가 접속을 끊은후 일정 시간이 지났으면 가장 우선권이 높은 클라이언트에게 제어권을 인계한다.
        private void CheckOptimalSOPControl(List<int> actionStepHistoryIDs, List<Client.ClientData> clients)
        {
            List<Data.SOP.SOPClientData> clientList = new List<Data.SOP.SOPClientData>();

            foreach (Client.ClientData client in clients)
            {
                clientList.Add((Data.SOP.SOPClientData)client);
            }

            if (clientList.Count == 0)
                return;

            Data.SOP.SOPClientData temp;
            ActionStepHistory actionStepHistory;
            DateTime dtNow = DateTime.Now;

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                if (m_dicControlClient.TryGetValue(nActionStepHistoryID, out temp) == false)
                {
                    if (m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                    {
                        TimeSpan span = dtNow - actionStepHistory.LastControlCheck;

                        if (span.TotalSeconds >= m_nControlWaitSeconds)
                        {
                            if (m_agent != null)
                            {
                                Data.SOP.SOPClientData controlClient = GetOptimalControlClient(nActionStepHistoryID, clientList);

                                if (controlClient != null)
                                {
                                    SetControlClient(nActionStepHistoryID, controlClient);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private ArrayList CheckControlClient(List<Client.ClientData> clients)
        {
            List<int> actionStepHistoryIDs = m_dicActionStepHistory.Keys.ToList();

            // SOP 제어권의 원래 소유자가 접속중인지 검사한다.
            CheckOriginalSOPControl(actionStepHistoryIDs, clients);

            // 모니터링을 사용하지 않을 경우 SOP를 시작한 사용자가 계속해서 제어권을 갖도록 한다.
            // 해당 사용자가 접속을 끊으면 그 시간만큼 SOP는 실행되지 않는다.
            if (m_useSOPMonitoring)
            {
                // SOP 제어권의 원래 소유자가 접속을 끊은후 일정 시간이 지났으면 가장 우선권이 높은 클라이언트에게 제어권을 인계한다.
                CheckOptimalSOPControl(actionStepHistoryIDs, clients);
            }

            Data.SOP.SOPClientData data;
            ActionStepHistory actionStepHistory;
            ArrayList arrDatas = new ArrayList();

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                // 제어권이 아직 생성되지 않은 ActionStepHistory 정보는 클라이언트에게 보내지 않는다..
                if (m_dicControlClient.TryGetValue(nActionStepHistoryID, out data) && m_dicActionStepHistory.TryGetValue(nActionStepHistoryID, out actionStepHistory))
                {
                    arrDatas.Add(nActionStepHistoryID);
                    arrDatas.Add(data.ID);
                    arrDatas.Add(actionStepHistory.ActionStepID);
                    arrDatas.Add(actionStepHistory.IsRealMode);
                    arrDatas.Add(actionStepHistory.BeginTime.ToBinary());

                    if (actionStepHistory.EndTime == null)
                        arrDatas.Add(-1);
                    else
                        arrDatas.Add(actionStepHistory.EndTime.Data.ToBinary());

                    if (actionStepHistory.CancelTime == null)
                        arrDatas.Add(-1);
                    else
                        arrDatas.Add(actionStepHistory.CancelTime.Data.ToBinary());

                    if (actionStepHistory.DetectTime == null)
                        arrDatas.Add(-1);
                    else
                        arrDatas.Add(actionStepHistory.DetectTime.Data.ToBinary());

                    arrDatas.Add(actionStepHistory.SensorZoneHistoryID);

                    if (actionStepHistory.Position == null)
                        arrDatas.Add(-1);
                    else
                        arrDatas.Add(actionStepHistory.Position);

                    if (actionStepHistory.SelectedComponentID == null)
                        arrDatas.Add(-1);
                    else
                        arrDatas.Add(actionStepHistory.SelectedComponentID.Data);

                    if (actionStepHistory.SelectedComponentType == null)
                        arrDatas.Add(-1);
                    else
                        arrDatas.Add(actionStepHistory.SelectedComponentType.Data);

                    if (actionStepHistory.StartOption == null)
                        arrDatas.Add(-1);
                    else
                        arrDatas.Add(actionStepHistory.StartOption.Data);

                    if (actionStepHistory.DisasterOption == null)
                        arrDatas.Add(-1);
                    else
                        arrDatas.Add(actionStepHistory.DisasterOption);
                }
            }

            return arrDatas;
        }

        public void SendSensorSignal(AlarmData alarm, int nZoneID, int nOriginSensorID, float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            bool isRealMode = GetTrainingMode(dbMgr);
            dbMgr.Close();

            if (nZoneID < 0)
            {
                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(alarm.SensorZoneID);

                if (sensorZone != null && sensorZone.EquipZone != null)
                    nZoneID = sensorZone.EquipZone.ID;
            }

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nOriginSensorID);
            arrDatas.Add(alarm.SensorZoneHistoryID);
            arrDatas.Add(nZoneID);
            arrDatas.Add(alarm.TimeStamp.ToBinary());
            arrDatas.Add(x);
            arrDatas.Add(y);
            arrDatas.Add(z);
            arrDatas.Add(isRealMode ? 0 : 1);
            arrDatas.Add(alarm.SensorZoneID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
            // Timer를 사용하도록 한다.
            SendClientData(SOPWebServer.Header.SENSOR_SIGNAL_FOR_SOP, bytes, SOPWebServer.ClientType.SOP_SIMULATOR, -1);
            //SendMessageToClient(-1, -1, SOPWebServer.Header.SENSOR_SIGNAL_FOR_SOP, bytes, null);
        }

        public static bool GetTrainingMode(DirectDBManager dbMgr)
        {
            string szSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='TranningMode' and SiteID = " + dbMgr.SiteID;
            ArrayList arrResult = dbMgr.GetResultData(szSQL);

            if (arrResult == null || arrResult.Count == 0)
            {
                return false;
            }
            else
            {
                int value = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

                if (value == 1)
                    return true;
            }

            return false;
        }

        public void SendClearAlarm(AlarmData alarm)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(alarm.SensorZoneHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
            // Timer를 사용하도록 한다.
            SendClientData(SOPWebServer.Header.CLEAR_DETECT_REPORT, bytes, SOPWebServer.ClientType.SOP_SIMULATOR, -1);
            //SendMessageToClient(-1, -1, SOPWebServer.Header.CLEAR_DETECT_REPORT, bytes, null);
        }

        public void SendChangedConfig(int nConfigData)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SOPWebServer.ClientType.SDMS);
            arrDatas.Add(SOP.SDMSConfig.PropertyName);
            arrDatas.Add(nConfigData.ToString());

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.CHANGE_CONFIG, bytes, SOPWebServer.ClientType.SOP_SIMULATOR, -1);
        }

        public void DeleteActionStepHistory(List<int> actionStepHistoryIDs, List<int> actionStepIDs)
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            // m_nMaxActionStepID가 삭제되었으므로 DB로부터 새로 읽어온다.
            string strSQL = "Select max(ID) from ActionStepHistory";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            dbMgr.Close();

            if (arrResult == null)
                return;

            if (arrResult.Count == 0)
                m_nMaxActionStepHistoryID = 0;
            else
                m_nMaxActionStepHistoryID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }
    }
}
