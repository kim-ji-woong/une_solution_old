using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.ServiceProcess;
using System.Configuration;
using SDMS.DAL;
using SDMS.Model.Alarm;
using SDMS.Model.Spatial;
using Common.Model.Option;
using dnsDBUtil;

namespace BroadcastServer
{
    using Network;

    public class ServiceTemp
    {
        private const string RunBroadcastTag = "RunAlarmBroadcast";
        private const string CloseBroadcastTag = "CloseAlarmBroadcast";

        private Timer m_timer = null;
        private BroadcastManager m_manager = null;
        private DataManager m_dataManager = null;
        private Common.DAL.DataManager m_commonDataManager = null;
        private DateTime m_dtLast;
        private string m_strLastTime = "";
        private string m_strSensorTypes = "";
        private Dictionary<string, KeyValuePair<int, int>> m_dicBuildingGroupParams = new Dictionary<string, KeyValuePair<int, int>>();

        private bool m_processing = false;

        public ServiceTemp(IServiceOwner owner)
        {
            InitData(owner);

            // 1초에 한번씩 동작
            m_timer = new Timer(1000);
            m_timer.Elapsed += OnTimer;
            m_timer.Start();
        }

        private void InitData(IServiceOwner owner)
        {
            // BroadcastServer가 시작전에 발생한 알람은 방송에 내보내지 않는다.
            // 한번도 방송하지 않은 알람이라 하더라도...
            m_dtLast = DateTime.Now;
            m_strLastTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", m_dtLast.Year, m_dtLast.Month, m_dtLast.Day, m_dtLast.Hour, m_dtLast.Minute, m_dtLast.Second);

            int? port = GetServerPort();

            if (port != null)
            {
                if (SetDataManager())
                {
                    m_manager = new BroadcastManager(owner, (int)port);
                }
            }

            string strSensorTypes = ConfigurationManager.AppSettings.Get("sensorTypes").Trim();

            if (strSensorTypes != null && strSensorTypes.Length > 0)
                m_strSensorTypes = strSensorTypes;

            int param, level;
            string strBuildingGroupParams = ConfigurationManager.AppSettings.Get("buildingGroupParams").Trim();

            if (strBuildingGroupParams != null && strBuildingGroupParams.Length > 0)
            {
                string[] tokens = strBuildingGroupParams.Split(';');

                foreach (string strToken in tokens)
                {
                    string[] datas = strToken.Split('=');

                    if (datas.Length == 2)
                    {
                        string strName = datas[0].Trim();
                        string[] values = datas[1].Split(',');

                        if (values.Length == 2 && int.TryParse(values[0].Trim(), out param) && int.TryParse(values[1].Trim(), out level))
                        {
                            m_dicBuildingGroupParams[strName] = new KeyValuePair<int, int>(param, level);
                        }
                    }
                }
            }
        }

        private int? GetServerPort()
        {
            string strPort = ConfigurationManager.AppSettings.Get("port");

            if (strPort == null || strPort.Length == 0)
                return null;

            int nPort;
            if (int.TryParse(strPort.Trim(), out nPort) == false)
                return null;

            return nPort;
        }

        private bool SetDataManager()
        {
            string strSite = ConfigurationManager.AppSettings.Get("siteid");

            if (strSite == null || strSite.Length == 0)
                return false;

            int nSiteID, nDBType;

            if (int.TryParse(strSite, out nSiteID) == false)
                return false;

            string strWebServerURL = ConfigurationManager.AppSettings.Get("webserverURL");
            string strDBName = ConfigurationManager.AppSettings.Get("dbName");
            string strDBType = ConfigurationManager.AppSettings.Get("dbType");

            if (strWebServerURL == null || strWebServerURL.Length == 0 ||
                strDBName == null || strDBName.Length == 0 ||
                strDBType == null || strDBType.Length == 0)
                return false;

            if (int.TryParse(strDBType, out nDBType) == false)
                return false;

            m_dataManager = new DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_commonDataManager = new Common.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            return true;
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            if (m_dataManager != null)
            {
                if (m_processing == false)
                {
                    m_processing = true;
                    CheckAlarms();
                    m_processing = false;
                }
            }
        }

        private void CheckAlarms()
        {
            bool isNullable;
            string strCondition = string.Format("{0} > {1}",
                CurrentAlarm.GetFieldName(CurrentAlarm.Fields.TimeStamp, out isNullable),
                m_strLastTime);

            if (m_strSensorTypes.Length > 0)
            {
                strCondition += " and " + m_strSensorTypes.Replace("type", CurrentAlarm.GetFieldName(CurrentAlarm.Fields.SensorType, out isNullable));
            }

            string strErrorMessage;
            List<CurrentAlarm> alarms = m_dataManager.GetSelectManager().SelectCurrentAlarms(null, strCondition, out strErrorMessage);

            if (alarms == null)
            {
                if (strErrorMessage != null)
                    System.Diagnostics.Trace.WriteLine(strErrorMessage);

                return;
            }

            CurrentAlarm selectedAlarm = null;

            foreach (CurrentAlarm alarm in alarms)
            {
                if (m_dtLast < alarm.TimeStamp)
                {
                    m_dtLast = alarm.TimeStamp;
                    selectedAlarm = alarm;

                    m_strLastTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", m_dtLast.Year, m_dtLast.Month, m_dtLast.Day, m_dtLast.Hour, m_dtLast.Minute, m_dtLast.Second);
                }
            }

            if (selectedAlarm != null)
            {
                if (UseAlarmBroadcast())
                {
                    int param;
                    BroadcastManager.AlarmLevel level;
                    BuildingGroup buildingGroup = GetAlarmBuildingGroup(selectedAlarm);

                    if (GetBroadcastParameter(buildingGroup, out param, out level))
                    {
                        // 기존에 실행중인 방송이 있으면 종료시킨다.
                        CloseBroadcast();

                        if (m_manager.SendMessage(BroadcastManager.CommandType.CMD_PSM, param, level))
                        {
                            System.Diagnostics.Trace.WriteLine("RunBroadcast : " + buildingGroup.GroupName + "(" + param + ", " + (int)level + ")");
                            // 방송이 실행되었다.
                            // 언젠가는 꺼야한다.
                            OnBroadcast(param);
                            return;
                        }
                    }
                }
            }

            // 방송종료 명령이 있는지 확인한다.
            CheckClosingCommand();
        }

        private void CheckClosingCommand()
        {
            string strErrorMessage;
            List<Options> options = m_commonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, CloseBroadcastTag, out strErrorMessage);

            if (options == null || options.Count == 0)
                return;

            Options option = options[0];

            if (option.PropertyValue != null)
            {
                string strValue = option.PropertyValue.ToLower();

                if (strValue == "1" || strValue == "true")
                {
                    List<Options> _options = m_commonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, RunBroadcastTag, out strErrorMessage);

                    if (_options != null && _options.Count > 0)
                    {
                        Options runOption = _options[0];

                        if (runOption.PropertyValue != null)
                        {
                            int param;

                            if (int.TryParse(runOption.PropertyValue, out param))
                            {
                                m_manager.SendMessage(BroadcastManager.CommandType.CMD_PSM, param, BroadcastManager.AlarmLevel.ClearAlarm);
                                System.Diagnostics.Trace.WriteLine("CloseBroadcast : (" + param + ")");

                                runOption.PropertyValue = "0";
                                m_commonDataManager.GetUpdateManager().UpdateOption(Options.OptionTarget.SDMS, runOption);
                            }
                        }
                    }

                    option.PropertyValue = "0";
                    m_commonDataManager.GetUpdateManager().UpdateOption(Options.OptionTarget.SDMS, option);
                }
            }
        }

        private void OnBroadcast(int param)
        {
            string strErrorMessage;
            List<Options> options = m_commonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, RunBroadcastTag, out strErrorMessage);

            if (options == null)
                return;

            if (options.Count == 0)
            {
                m_commonDataManager.GetCreateManager().CreateOption(Options.OptionTarget.SDMS, RunBroadcastTag, param.ToString(), m_commonDataManager.SiteID);
            }
            else
            {
                Options option = options[0];
                option.PropertyValue = param.ToString();
                m_commonDataManager.GetUpdateManager().UpdateOption(Options.OptionTarget.SDMS, option);
            }
        }

        // 기존에 실행중인 방송이 있으면 종료시킨다.
        private void CloseBroadcast()
        {
            string strErrorMessage;
            List<Options> options = m_commonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, RunBroadcastTag, out strErrorMessage);

            if (options == null || options.Count == 0)
                return;

            int param;
            Options option = options[0];

            if (option.PropertyValue != null)
            {
                if (int.TryParse(option.PropertyValue.Trim(), out param))
                {
                    if (param > 0)
                    {
                        m_manager.SendMessage(BroadcastManager.CommandType.CMD_PSM, param, BroadcastManager.AlarmLevel.ClearAlarm);
                        System.Diagnostics.Trace.WriteLine("CloseBroadcast : (" + param + ")");

                        option.PropertyValue = "0";
                        m_commonDataManager.GetUpdateManager().UpdateOption(Options.OptionTarget.SDMS, option);
                        System.Threading.Thread.Sleep(2000);

                        List<Options> _options = m_commonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, CloseBroadcastTag, out strErrorMessage);

                        if (_options != null && _options.Count > 0)
                        {
                            Options closeOption = _options[0];

                            if (closeOption.PropertyValue != null)
                            {
                                string strClose = closeOption.PropertyValue.ToLower();

                                if (strClose == "1" || strClose == "true")
                                {
                                    closeOption.PropertyValue = "0";
                                    m_commonDataManager.GetUpdateManager().UpdateOption(Options.OptionTarget.SDMS, closeOption);
                                }
                            }
                        }
                    }
                }
            }
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

        private BuildingGroup GetAlarmBuildingGroup(CurrentAlarm alarm)
        {
            string strSQL = "Select bg.ID, bg.GroupName ";
            strSQL += "from SdmsSpatialZone as zone, SdmsSpatialBuilding as building, SdmsSpatialBuildingGroup as bg, SdmsHistorySensorZone as szh ";
            strSQL += "where zone.BuildingID = building.ID and building.BuildingGroupID = bg.ID and szh.ZoneID = zone.ID and szh.ID = " + alarm.SensorZoneHistoryID.ToString();

            WebDBManager dbMgr = (WebDBManager)m_dataManager.GetDBManager();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 2)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            string strBuildingGroupName = WebDBManager.GetStringField(arrResult[1]);

            if (id == null || strBuildingGroupName == null)
                return null;

            BuildingGroup buildingGroup = new BuildingGroup();
            buildingGroup.ID = id.Data;
            buildingGroup.GroupName = strBuildingGroupName;
            return buildingGroup;
        }

        private bool GetBroadcastParameter(BuildingGroup buildingGroup, out int param, out BroadcastManager.AlarmLevel level)
        {
            param = 0;
            level = BroadcastManager.AlarmLevel.ClearAlarm;

            if (buildingGroup == null)
                return false;

            KeyValuePair<int, int> value;

            if (m_dicBuildingGroupParams.TryGetValue(buildingGroup.GroupName, out value))
            {
                param = value.Key;
                level = (BroadcastManager.AlarmLevel)value.Value;
                return true;
            }

            return false;
        }
    }
}
