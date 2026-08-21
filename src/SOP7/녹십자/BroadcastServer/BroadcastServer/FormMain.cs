using System;
using System.Configuration;
using System.Windows.Forms;

namespace BroadcastServer
{
    using Common.Model.Option;
    using dnsDBUtil;
    using Network;
    using SDMS.DAL;
    using SDMS.Model.Alarm;
    using SDMS.Model.Spatial;
    using System.Collections;
    using System.Collections.Generic;
    using System.Timers;

    public partial class FormMain : Form, IServiceOwner
    {
        private enum UpdateOptionType { Add = 0, Remove }

        private const string RunBroadcastTag = "RunAlarmBroadcast";
        private const string CloseBroadcastTag = "CloseAlarmBroadcast";

        private System.Timers.Timer m_timer = null;
        private BroadcastManager m_manager = null;
        private DataManager m_dataManager = null;
        private Common.DAL.DataManager m_commonDataManager = null;
        private DateTime m_dtLast;
        private string m_strLastTime = "";
        private string m_strSensorTypes = "";

        private bool m_processing = false;

        public FormMain()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            // BroadcastServer가 시작전에 발생한 알람은 방송에 내보내지 않는다.
            // 한번도 방송하지 않은 알람이라 하더라도...
            m_dtLast = DateTime.Now;
            m_strLastTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", m_dtLast.Year, m_dtLast.Month, m_dtLast.Day, m_dtLast.Hour, m_dtLast.Minute, m_dtLast.Second);

            // 빌딩 초기화
            Dictionary<string, BuildingConfigData> dicBuilding;

            if (InitBuildingConfig(out dicBuilding) == false)
                Environment.Exit(0);

            // DB 설정 초기화
            if (SetDataManager())
                m_manager = new BroadcastManager(this, dicBuilding);
            else
                Environment.Exit(0);


            string strSensorTypes = ConfigurationManager.AppSettings.Get("sensorTypes").Trim();

            if (strSensorTypes != null && strSensorTypes.Length > 0)
                m_strSensorTypes = strSensorTypes;
            else
            {
                Logger.Instance.Write("sensorTypes 정보가 잘못되었습니다.");
                System.Diagnostics.Trace.WriteLine("sensorTypes 정보가 잘못되었습니다.");
                Environment.Exit(0);
            }

        }

        private bool InitBuildingConfig(out Dictionary<string, BuildingConfigData> dicBuilding)
        {
            dicBuilding = new Dictionary<string, BuildingConfigData>();

            // 빌딩 Port 초기화
            string strBuildingPort = ConfigurationManager.AppSettings.Get("buildingPort").Trim();

            if (strBuildingPort != null && strBuildingPort.Length > 0)
            {
                string[] tokens = strBuildingPort.Split(';');

                foreach (string strToken in tokens)
                {
                    string[] datas = strToken.Split('=');

                    if (datas.Length == 2)
                    {
                        string strName = datas[0].Trim();
                        //string[] values = datas[1].Split(',');
                        string value = datas[1];
                        int nPort;

                        if (int.TryParse(value.Trim(), out nPort))
                        {
                            BuildingConfigData data = new BuildingConfigData();
                            //data.ID = nID;
                            data.Port = nPort;
                            data.Name = strName;

                            dicBuilding[strName] = data;
                        }
                    }
                }
            } 
            else
            {
                Logger.Instance.Write("buildingPort 정보가 잘못되었습니다.");
                System.Diagnostics.Trace.WriteLine("buildingPort 정보가 잘못되었습니다.");
                return false;
            }

            // 빌딩 ID 초기화
            string strBuildingID = ConfigurationManager.AppSettings.Get("buildingID").Trim();

            if (strBuildingID != null && strBuildingID.Length > 0)
            {
                string[] tokens = strBuildingID.Split(';');

                foreach (string strToken in tokens)
                {
                    string[] datas = strToken.Split('=');

                    if (datas.Length == 2)
                    {
                        string strName = datas[0].Trim();
                        string[] values = datas[1].Split(',');

                        foreach (string value in values)
                        {
                            int nID;

                            if (int.TryParse(value.Trim(), out nID) && dicBuilding.ContainsKey(strName))
                            {
                                List<int> ids = dicBuilding[strName].IDs;

                                if (ids.Contains(nID) == false)
                                {
                                    ids.Add(nID);
                                }
                            }
                        }
                    }
                }
            } 
            else
            {
                Logger.Instance.Write("buildingID 정보가 잘못되었습니다.");
                System.Diagnostics.Trace.WriteLine("buildingID 정보가 잘못되었습니다.");
                return false;
            }

            return true;
        }

        private bool SetDataManager()
        {
            string strError = "DB 설정 정보가 잘못되었습니다.";
            string strSite = ConfigurationManager.AppSettings.Get("siteid");

            if (strSite == null || strSite.Length == 0)
                return false;

            int nSiteID, nDBType;

            if (int.TryParse(strSite, out nSiteID) == false)
            {
                Logger.Instance.Write(strError);
                System.Diagnostics.Trace.WriteLine(strError);
                return false;
            }

            string strWebServerURL = ConfigurationManager.AppSettings.Get("webserverURL");
            string strDBName = ConfigurationManager.AppSettings.Get("dbName");
            string strDBType = ConfigurationManager.AppSettings.Get("dbType");

            if (strWebServerURL == null || strWebServerURL.Length == 0 ||
                strDBName == null || strDBName.Length == 0 ||
                strDBType == null || strDBType.Length == 0)
            {
                Logger.Instance.Write(strError);
                System.Diagnostics.Trace.WriteLine(strError);
                return false;
            }
                

            if (int.TryParse(strDBType, out nDBType) == false)
            {
                Logger.Instance.Write(strError);
                System.Diagnostics.Trace.WriteLine(strError);
                return false;
            }

            m_dataManager = new DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_commonDataManager = new Common.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            return true;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            cboCommandType.SelectedIndex = 0;
            cboMaterialType.SelectedIndex = 0;
            cboAlarmLevel.SelectedIndex = 0;

            // 알람 리셋 >> 전체 건물 방송 OFF 한다.
            InitBroadcast();

            // 1초에 한번씩 동작
            m_timer = new System.Timers.Timer();
            m_timer.Interval = 1000;
            m_timer.Elapsed += new ElapsedEventHandler(OnTimer);
            m_timer.Start();
        }

        private bool InitBroadcast()
        {
            string strErrorMessage;

            foreach (KeyValuePair<string, ServiceProvider> pair in m_manager.DicProviders)
            {
                ServiceProvider provider = pair.Value;

                //if (provider.SendBroadcast(false) == false)
                //    return false;
                //else
                //{
                //    if (UpdateBroadcastOption(provider.BuildingConfigData.IDs, RunBroadcastTag, UpdateOptionType.Remove, out strErrorMessage) == false ||
                //        UpdateBroadcastOption(provider.BuildingConfigData.IDs, CloseBroadcastTag, UpdateOptionType.Remove, out strErrorMessage) == false)
                //        return false;
                //}
                // 방송 중지
                provider.SendBroadcast(false);

                // 방송 관련 옵션 초기화
                UpdateBroadcastOption(provider.BuildingConfigData.IDs, RunBroadcastTag, UpdateOptionType.Remove, out strErrorMessage);
                UpdateBroadcastOption(provider.BuildingConfigData.IDs, CloseBroadcastTag, UpdateOptionType.Remove, out strErrorMessage);
            }

            return true;
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            if (m_dataManager != null)
            {
                if (m_processing == false)
                {
                    DateTime dtCurrent = DateTime.Now;
                    bool removeLogs = false;

                    if (dtCurrent.Year != m_dtLast.Year ||
                        dtCurrent.Month != m_dtLast.Month ||
                        dtCurrent.Day != m_dtLast.Day)
                        removeLogs = true;

                    m_processing = true;
                    CheckAlarms();

                    // CheckAlarms가 RemoveOldLogs() 보다 중요하니까
                    // 먼저 처리하고 로그는 나중에 지운다.
                    if (removeLogs)
                    {
                        Logger.Instance.RemoveOldLogs();
                        m_dtLast = dtCurrent;
                    }
                        
                    m_processing = false;
                }
            }
        }

        private void CheckAlarms()
        {
            bool isNullable;
            string strCondition = "";

            if (m_strSensorTypes.Length > 0)
            {
                strCondition += m_strSensorTypes.Replace("type", CurrentAlarm.GetFieldName(CurrentAlarm.Fields.SensorType, out isNullable));
            }

            string strErrorMessage;
            List<CurrentAlarm> alarms = m_dataManager.GetSelectManager().SelectCurrentAlarms(null, strCondition, out strErrorMessage);

            if (alarms == null)
            {
                if (strErrorMessage != null)
                    System.Diagnostics.Trace.WriteLine(strErrorMessage);

                return;
            }

            // 기존 알람인지, 새로운 알람인지 판단

            // 1) 새로운 알람이라면 알람 등록

            // 2) 해당 건물에 방송 ON, OFF 판단

            // - 방송 OFF 이라면,
            // 2-1) .TODO: 사용자 방송 ON 확인 (RunAlarmBroadcast 옵션 확인)
            // 2-1) and 사용자 방송 중지 확인 (CloseAlarmBroadcast 옵션 확인)
            // 사용자 방송 ON 그리고 사용자 방송 중지가 아니라면 방송 실행 

            // - 방송 ON 이라면,
            // 2-2) 사용자 방송 중지 확인 (CloseAlarmBroadcast 옵션 확인)
            // 방송 중지 이라면 방송 중지

            foreach (CurrentAlarm alarm in alarms)
            {
                // 방송 설정 사용 여부 확인
                if (UseAlarmBroadcast())
                {
                    // 알람에 대한 건물 방송 서버를 불러온다.
                    Building building = GetAlarmBuilding(alarm);
                    ServiceProvider provider = m_manager.GetBroadcastProvider(building.ID);

                    if (provider == null)
                        continue;
                    
                    // 기존 알람인지, 새로운 알람인지 판단
                    if (provider.BuildingConfigData.Alarms.ContainsKey(alarm.SensorZoneHistoryID) == false)
                    {   // 1) 새로운 알람이라면 알람 등록
                        Dictionary<int, CurrentAlarm> dicAlarms = provider.BuildingConfigData.Alarms;
                        dicAlarms[alarm.SensorZoneHistoryID] = alarm;
                    }

                    // 2) 해당 건물에 방송 ON, OFF 판단
                    if (provider.BuildingConfigData.RunBroadcast == false)
                    {   // 방송 OFF 이라면

                        // 2-1) .TODO: 사용자 방송 ON 확인 (RunAlarmBroadcast 옵션 확인)
                        //if (CheckBroadcastOption(provider.BuildingConfigData.IDs, RunBroadcastTag) == true && CheckBroadcastOption(provider.BuildingConfigData.IDs, CloseBroadcastTag) == false)
                        // 2-1) 사용자 방송 중지 확인 (CloseAlarmBroadcast 옵션 확인)
                        if (CheckCloseBroadcastOption(provider.BuildingConfigData.IDs) == false)
                        {   // 사용자 방송 ON 그리고 사용자 방송 중지가 아니라면 방송 실행 

                            // 방송 실행 
                            provider.SendBroadcast(true);
                            // 방송 여부 업데이트
                            provider.BuildingConfigData.RunBroadcast = true;
                            // 사용자 방송 ON 옵션 업데이트 (RunAlarmBroadcast 옵션 업데이트)
                            UpdateBroadcastOption(provider.BuildingConfigData.IDs, RunBroadcastTag, UpdateOptionType.Add, out strErrorMessage);
                            // 사용자 방송 중지 옵션 업데이트 (CloseAlarmBroadcast 옵션 업데이트)
                            UpdateBroadcastOption(provider.BuildingConfigData.IDs, CloseBroadcastTag, UpdateOptionType.Remove, out strErrorMessage);
                        }
                    }
                    else if (provider.BuildingConfigData.RunBroadcast == true)
                    {   // 방송 ON 이라면

                        // 2-2) 사용자 방송 중지 확인 (CloseAlarmBroadcast 옵션 확인)
                        if (CheckBroadcastOption(provider.BuildingConfigData.IDs, CloseBroadcastTag) == true)
                        //if (CheckCloseBroadcastOption(provider.BuildingConfigData.IDs) == true)
                        {   // 사용자 방송 중지 이라면 방송 중지

                            // 방송 중지
                            provider.SendBroadcast(false);
                            // 방송 여부 업데이트
                            provider.BuildingConfigData.RunBroadcast = false;
                            // RunAlarmBroadcast 옵션 제거
                            //UpdateBroadcastOption(provider.BuildingConfigData.IDs, RunBroadcastTag, UpdateOptionType.Remove, out strErrorMessage);
                            
                            // 사용자 방송 중지 옵션 업데이트 (CloseAlarmBroadcast 옵션 업데이트)
                            UpdateBroadcastOption(provider.BuildingConfigData.IDs, CloseBroadcastTag, UpdateOptionType.Add, out strErrorMessage);
                        }
                    }
                }
            }

            // 제거된 알람 여부 조회

            // 1) 있다면 해당 알람 제거

            // 1-1) 해당 건물 알림 갯수 확인
            // 알람이 없다면
            // 방송이 ON 이라면 방송 OFF
            // RunAlarmBroadcast, CloseAlarmBroadcast 옵션 제거

            // 제거된 알람 여부 조회
            Dictionary<string, ServiceProvider> dicProviders = m_manager.DicProviders;

            if (dicProviders != null)
            {
                foreach (KeyValuePair<string, ServiceProvider> pair in dicProviders)
                {
                    ServiceProvider provider = pair.Value;
                    Dictionary<int, CurrentAlarm> dicAlarms = provider.BuildingConfigData.Alarms;


                    var alarmDatas = new Dictionary<int, CurrentAlarm>(dicAlarms);

                    foreach (KeyValuePair<int, CurrentAlarm> keyValue in alarmDatas)
                    {
                        CurrentAlarm value = keyValue.Value;

                        bool bChk = false;

                        foreach (CurrentAlarm alarmData in alarms) {
                            if (alarmData.SensorZoneHistoryID == value.SensorZoneHistoryID)
                                bChk = true;
                        }

                        if (bChk == false)    
                        {   // 1) 있다면 해당 알람 제거
                            //alarms.Remove(value);
                            dicAlarms.Remove(value.SensorZoneHistoryID);

                            // 1-1) 해당 건물 알림 갯수 확인
                            if (dicAlarms.Count == 0)
                            {   // 알람이 없다면
                                if (provider.BuildingConfigData.RunBroadcast == true)
                                {   // 방송이 ON 이라면 방송 OFF
                                    // 방송 중지
                                    provider.SendBroadcast(false);
                                    // 방송 여부 업데이트
                                    provider.BuildingConfigData.RunBroadcast = false;
                                }

                                // RunAlarmBroadcast, CloseAlarmBroadcast 옵션 제거
                                UpdateBroadcastOption(provider.BuildingConfigData.IDs, RunBroadcastTag, UpdateOptionType.Remove, out strErrorMessage);
                                UpdateBroadcastOption(provider.BuildingConfigData.IDs, CloseBroadcastTag, UpdateOptionType.Remove, out strErrorMessage);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 해당 빌딩에 대해 사용자가 방송 시작 및 중지 여부를 체크 
        /// </summary>
        /// <returns></returns>
        private bool CheckBroadcastOption(List<int> BuildingID, string RunCloseBroadcastTag)
        {
            string strErrorMessage;

            List<Options> options = m_commonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, RunCloseBroadcastTag, out strErrorMessage);

            if (options != null && options.Count > 0)
            {
                Options closeOption = options[0];

                if (closeOption.PropertyValue != null)
                {
                    string strClose = closeOption.PropertyValue;
                    string[] values = strClose.Split(',');

                    foreach (string value in values)
                    {
                        int nID;

                        if (int.TryParse(value.Trim(), out nID))
                        {
                            if (BuildingID.Contains(nID))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool CheckCloseBroadcastOption(List<int> BuildingID)
        {
            string strErrorMessage;

            List<Options> options = m_commonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, CloseBroadcastTag, out strErrorMessage);

            if (options != null && options.Count > 0)
            {
                Options closeOption = options[0];

                if (closeOption.PropertyValue != null)
                {
                    string strClose = closeOption.PropertyValue;
                    string[] values = strClose.Split(',');
                    

                    foreach (int nData in BuildingID)
                    {
                        bool bChk = false;

                        foreach (string value in values)
                        {
                            int nID;

                            if (int.TryParse(value.Trim(), out nID))
                            {
                                if (nData == nID)
                                    bChk = true;
                            }
                        }

                        if (bChk == false)
                            return false;
                    }
                }
            }

            return true;
        }

        private bool UpdateBroadcastOption(List<int> BuildingID, string RunCloseBroadcastTag, UpdateOptionType type, out string strErrorMessage)
        {
            strErrorMessage = "";

            List<Options> options = m_commonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, RunCloseBroadcastTag, out strErrorMessage);

            if (options != null && options.Count > 0)
            {
                Options option = options[0];

                if (option.PropertyValue != null)
                {
                    string strClose = option.PropertyValue;
                    string[] values = strClose.Split(',');
                    List<int> IDs = new List<int>();
                    bool bChkUpdate = false;

                    foreach (string value in values)
                    {
                        int nID;

                        if (int.TryParse(value.Trim(), out nID))
                            IDs.Add(nID);
                    }

                    string strBuildingIDs = "";

                    if (type == UpdateOptionType.Add)
                    {
                        foreach (int nID in BuildingID)
                        {
                            if (IDs.Contains(nID) == false)
                            {
                                bChkUpdate = true;
                                IDs.Add(nID);
                            }

                        }
                    }
                    else if (type == UpdateOptionType.Remove)
                    {
                        foreach (int nID in BuildingID)
                        {
                            if (IDs.Contains(nID) == true)
                            {
                                bChkUpdate = true;
                                IDs.Remove(nID);
                            }

                        }
                    }

                    // 수정된 내용이 있는지 체크
                    if (bChkUpdate == true)
                    {
                        foreach (int nID in IDs)
                        {
                            if (strBuildingIDs == "")
                                strBuildingIDs = nID.ToString();
                            else
                                strBuildingIDs += "," + nID.ToString();
                        }

                        option.PropertyValue = strBuildingIDs.ToString();
                        m_commonDataManager.GetUpdateManager().UpdateOption(Options.OptionTarget.SDMS, option);
                        return true;
                    }
                }
            }
            else if (options.Count == 0)
            {
                string strBuildingIDs = "";

                if (type == UpdateOptionType.Add)
                {
                    foreach (int nID in BuildingID)
                    {
                        if (strBuildingIDs == "")
                            strBuildingIDs = nID.ToString();
                        else
                            strBuildingIDs += "," + nID.ToString();
                    }

                    m_commonDataManager.GetCreateManager().CreateOption(Options.OptionTarget.SDMS, RunBroadcastTag, strBuildingIDs, m_commonDataManager.SiteID);
                    return true;
                }
                else if (type == UpdateOptionType.Remove)
                {
                    m_commonDataManager.GetCreateManager().CreateOption(Options.OptionTarget.SDMS, RunBroadcastTag, strBuildingIDs, m_commonDataManager.SiteID);
                    return true;
                }
            }

            return false;
        }

        private bool UseAlarmBroadcast()
        {
            string strErrorMessage;
            List<Options> options = m_commonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, "UseAlarmBroadcast", out strErrorMessage);

            if (options == null || options.Count == 0)
                return false;

            Options option = options[0];
            string strOption = option.PropertyValue.ToLower().Trim();

            if (strOption == "1" || strOption == "true")
                return true;

            return false;
        }

        private Building GetAlarmBuilding(CurrentAlarm alarm)
        {
            string strSQL = "Select building.ID, building.BuildingName ";
            strSQL += "from SdmsSpatialZone as zone, SdmsSpatialBuilding as building, SdmsSpatialBuildingGroup as bg, SdmsHistorySensorZone as szh ";
            strSQL += "where zone.BuildingID = building.ID and building.BuildingGroupID = bg.ID and szh.ZoneID = zone.ID and szh.ID = " + alarm.SensorZoneHistoryID.ToString();

            WebDBManager dbMgr = (WebDBManager)m_dataManager.GetDBManager();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 2)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            string strBuildingName = WebDBManager.GetStringField(arrResult[1]);

            if (id == null || strBuildingName == null)
                return null;

            Building building = new Building();
            building.ID = id.Data;
            building.BuildingName = strBuildingName;
            return building;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (m_manager != null)
            {
                BroadcastManager.CommandType cmd = (BroadcastManager.CommandType)(cboCommandType.SelectedIndex + 1);
                //BroadcastManager.MaterialType material = (BroadcastManager.MaterialType)(cboMaterialType.SelectedIndex + 1);
                BroadcastManager.AlarmLevel alarmLevel;

                if (cboAlarmLevel.SelectedIndex == 0)
                    alarmLevel = BroadcastManager.AlarmLevel.ClearAlarm;
                else
                    alarmLevel = (BroadcastManager.AlarmLevel)(cboAlarmLevel.SelectedIndex + 1);

                //if (alarmLevel != BroadcastManager.AlarmLevel.ClearAlarm)
                //{
                //    // 모든 타입에 대하여 알람해제를 시킨다.
                //    /*foreach (BroadcastManager.MaterialType materialType in Enum.GetValues(typeof(BroadcastManager.MaterialType)))
                //    {
                //        m_manager.SendMessage(cmd, materialType, BroadcastManager.AlarmLevel.ClearAlarm);
                //        System.Threading.Thread.Sleep(500);
                //    }*/

                //    /*m_manager.SendMessage(cmd, material, BroadcastManager.AlarmLevel.ClearAlarm);
                //    System.Threading.Thread.Sleep(500);*/
                //    m_manager.SendMessage(cmd, material, alarmLevel);
                //}
                //else
                {
                    if (cmd == BroadcastManager.CommandType.CMD_PSM)
                    {
                        BroadcastManager.MaterialType material = (BroadcastManager.MaterialType)(cboMaterialType.SelectedIndex + 1);
                        m_manager.SendMessage(cmd, (int)material, alarmLevel);
                    }
                    else
                    {
                        BroadcastManager.FireType fire = (BroadcastManager.FireType)(cboMaterialType.SelectedIndex + 1);
                        m_manager.SendMessage(cmd, (int)fire, alarmLevel);
                    }
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //m_manager.Close();
        }

        private void cboCommandType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BroadcastManager.CommandType cmd = (BroadcastManager.CommandType)(cboCommandType.SelectedIndex + 1);

            if (cmd == BroadcastManager.CommandType.CMD_PSM)
            {
                cboMaterialType.Items.Clear();

                cboMaterialType.Items.Add("불산");
                cboMaterialType.Items.Add("염산");
                cboMaterialType.Items.Add("Co");
                cboMaterialType.Items.Add("Co2");
                cboMaterialType.Items.Add("Tvoc");
                cboMaterialType.Items.Add("O2");
            }
            else
            {
                cboMaterialType.Items.Clear();

                cboMaterialType.Items.Add("화재");
            }

            cboMaterialType.SelectedIndex = 0;
        }

        public void OnAccept(string strConnectionInfo)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.labelConnection.Text = "클라이언트 접속중 : " + strConnectionInfo;
            });

            Logger.Instance.Write("클라이언트 접속(" + strConnectionInfo + ")");
        }

        public void OnDropConnection(string strConnectionInfo)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.labelConnection.Text = "접속된 클라이언트 없음";
            });

            Logger.Instance.Write("클라이언트 접속종료(" + strConnectionInfo + ")");
        }
    }

    
}
