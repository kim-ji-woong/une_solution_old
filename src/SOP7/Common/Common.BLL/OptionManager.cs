using Common.BLL.Models.Request;
using Common.BLL.Models.Response;
using Common.Model.Option;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Common.BLL
{
    public class OptionManager
    {
        private ProcessManager m_processManager = null;

        private static bool m_bTimerRunning = false;     // 셋팅값 조회중인가 ?
        private static Timer m_timerReadSettings = null; // 셋팅값 불러오기 타이머

        private static List<Common.Model.Option.Options> m_sdmsOptions = new List<Options>();
        /// <summary>
        /// SDMS 옵션
        /// </summary>
        public List<Options> SDMSOptions { get { return m_sdmsOptions; } }

        private static List<Common.Model.Option.Options> m_sopOptions = new List<Options>();
        /// <summary>
        /// SOP 옵션
        /// </summary>
        public List<Options> SOPOptions { get { return m_sopOptions; } }

        public OptionManager(ProcessManager processManager)
        {
            this.m_processManager = processManager;

            InitTimerReadSettings();
        }

        private void InitTimerReadSettings()
        {
            if (m_timerReadSettings == null)
            {
                m_timerReadSettings = new Timer();
                m_timerReadSettings.Interval = 1000 * 1.5;
                m_timerReadSettings.Elapsed += new ElapsedEventHandler(timerReadSettings_Elapsed);

                m_timerReadSettings.Start();
            }
        }

        /// <summary>
        /// 셋팅 값 불러오기
        /// </summary>
        private void timerReadSettings_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_bTimerRunning == true)
                return;

            m_bTimerRunning = true;

            string strErrorMessage;

            List<Common.Model.Option.Options> sdmsOptions = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, null, out strErrorMessage);
            if (sdmsOptions == null)
            {
                m_bTimerRunning = false;
                return;
            }

            m_sdmsOptions = sdmsOptions;

            List<Common.Model.Option.Options> sopOptions = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, null, out strErrorMessage);
            if (sopOptions == null)
            {
                m_bTimerRunning = false;
                return;
            }

            m_sopOptions = sopOptions;

            m_bTimerRunning = false;
        }

        public ResponseSettings GetSettings(RequestSettings data)
        {
            ResponseSettings result = new ResponseSettings();
            
            // 유저 설정 정보 불러오기
            Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicCondition = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.UserID, data.UserID);
            //dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.Category, strShortcutKey);

            string strErrorMessage = null;
            List<SOPManager.Model.Sop.Account.Option> options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            if (options == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            ShortcutKey key = new ShortcutKey();
            string strShortcutKey = "ShortcutKey";

            // 기본값
            result.IdleTime = "10;1";
            result.TurnStart = "1";
            result.UseAlarmTurn = "false";

            foreach (SOPManager.Model.Sop.Account.Option option in options)
            {
                if (option.Category == strShortcutKey && option.SubCategory == "SDMS")
                    key.SDMS = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "SOP")
                    key.SOP = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "SOPMgr")
                    key.SOPMgr = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "TeamEdit")
                    key.TeamEdit = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "History")
                    key.History = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "Settings")
                    key.Settings = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "Dashboard")
                    key.Dashboard = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "Home")
                    key.Home = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "Rotation")
                    key.Rotation = option.PropertyValue1;
                else if (option.Category == "IdleTime")
                    result.IdleTime = option.PropertyValue1;
                else if (option.Category == "TurnStart")
                    result.TurnStart = option.PropertyValue1;
                else if (option.Category == "UseAlarmTurn")
                    result.UseAlarmTurn = option.PropertyValue1;
            }

            result.ShortcutKey = key;

            //// 카메라 회전시간 설정 정보 불러오기
            //dicCondition = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            //dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.UserID, data.UserID);
            //dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.Category, "IdleTime");

            //options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            //if (options == null)
            //{
            //    result.Success = false;
            //    result.Message = strErrorMessage;
            //    return result;
            //}

            //string strIdleTime = "10;1";

            //if (options.Count > 0)
            //{
            //    SOPManager.Model.Sop.Account.Option temp = options[0];
            //    strIdleTime = temp.PropertyValue1;
            //}

            //result.IdleTime = strIdleTime;

            Dictionary<string, string> dicSDMSDefaultOptions = new Dictionary<string, string>();
            dicSDMSDefaultOptions["ReAlarm"] = "";
            dicSDMSDefaultOptions["UseReceiveFire"] = "true";
            dicSDMSDefaultOptions["UseReceivePSM"] = "true";
            dicSDMSDefaultOptions["UseReceiveETC"] = "true";
            dicSDMSDefaultOptions["UseReceiveSVMS"] = "true";
            dicSDMSDefaultOptions["EventInfoDisplayTerm"] = "0";
            dicSDMSDefaultOptions["UseScreenMove"] = "true";
            dicSDMSDefaultOptions["UseAlarmBroadcast"] = "0";
            dicSDMSDefaultOptions["MoveDisplayAlarm"] = "3";
            dicSDMSDefaultOptions["UsePoiFocus"] = "false";
            dicSDMSDefaultOptions["UsePoiHighlight"] = "false";

            if (!GetSDMSOptions(dicSDMSDefaultOptions, result, out strErrorMessage))
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            Dictionary<string, string> dicSopDefaultOptions = new Dictionary<string, string>();
            dicSopDefaultOptions["ExeCautionSOP"] = "1";
            dicSopDefaultOptions["ExeAlartSOP"] = "1";
            dicSopDefaultOptions["ExeSeriousSOP"] = "1";
            dicSopDefaultOptions["UseTrainingMode"] = "false";
            dicSopDefaultOptions["UseWaterMark"] = "false";
            dicSopDefaultOptions["UseHeadMessage"] = "false";
            dicSopDefaultOptions["UseAutoMoveSOPScreen"] = "true";
            dicSopDefaultOptions["UseBroadcast"] = "false";
            dicSopDefaultOptions["UseSMS"] = "false";
            dicSopDefaultOptions["UseEmail"] = "false";
            dicSopDefaultOptions["UseConfirm"] = "false";
            dicSopDefaultOptions["WorkingBeginHour"] = "9:0";
            dicSopDefaultOptions["WorkingEndHour"] = "18:0";
            dicSopDefaultOptions["UseResultSummary"] = "false";
            dicSopDefaultOptions["DashboardBegin"] = "today";
            dicSopDefaultOptions["DashboardEnd"] = "today";
            dicSopDefaultOptions["FireSOPWaitEndTime"] = "10;1;2";
            dicSopDefaultOptions["PSMSOPWaitEndTime"] = "10;1;2";
            dicSopDefaultOptions["ETCSOPWaitEndTime"] = "10;1;2";
            dicSopDefaultOptions["FireSOPRecoverEndTime"] = "10;1;2";
            dicSopDefaultOptions["PSMSOPRecoverEndTime"] = "10;1;2";
            dicSopDefaultOptions["ETCSOPRecoverEndTime"] = "10;1;2";

            if (!GetSopOptions(dicSopDefaultOptions, result, out strErrorMessage))
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            result.Success = true;
            return result;
        }

        private bool GetSDMSOptions(Dictionary<string, string> dicDefaultOptions, ResponseSettings result, out string strErrorMessage)
        {
            List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, null, out strErrorMessage);

            if (options == null)
                return false;

            foreach (Common.Model.Option.Options option in options)
            {
                if (dicDefaultOptions.ContainsKey(option.PropertyName))
                    dicDefaultOptions[option.PropertyName] = option.PropertyValue;
            }

            foreach (KeyValuePair<string, string> pair in dicDefaultOptions)
            {
                if (pair.Key == "ReAlarm")
                    result.ReAlarm = pair.Value;
                else if (pair.Key == "UseReceiveFire")
                    result.UseReceiveFire = pair.Value;
                else if (pair.Key == "UseReceivePSM")
                    result.UseReceivePSM = pair.Value;
                else if (pair.Key == "UseReceiveETC")
                    result.UseReceiveETC = pair.Value;
                else if (pair.Key == "UseReceiveSVMS")
                    result.UseReceiveSVMS = pair.Value;
                else if (pair.Key == "EventInfoDisplayTerm")
                    result.EventInfoDisplayTerm = pair.Value;
                else if (pair.Key == "UseScreenMove")
                    result.UseScreenMove = pair.Value;
                else if (pair.Key == "UseAlarmBroadcast")
                    result.UseAlarmBroadcast = pair.Value;
                else if (pair.Key == "MoveDisplayAlarm")
                    result.MoveDisplayAlarm = pair.Value;
                else if (pair.Key == "UsePoiFocus")
                    result.UsePoiFocus = pair.Value;
                else if (pair.Key == "UsePoiHighlight")
                    result.UsePoiHighlight = pair.Value;

            }

            return true;
        }

        private bool GetSopOptions(Dictionary<string, string> dicDefaultOptions, ResponseSettings result, out string strErrorMessage)
        {
            strErrorMessage = "";

            List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, null, out strErrorMessage);

            if (options == null)
                return false;

            foreach (Common.Model.Option.Options option in options)
            {
                if (dicDefaultOptions.ContainsKey(option.PropertyName))
                    dicDefaultOptions[option.PropertyName] = option.PropertyValue;
            }

            foreach (KeyValuePair<string, string> pair in dicDefaultOptions)
            {
                if (pair.Key == "ExeCautionSOP")
                    result.ExeCautionSOP = pair.Value;
                else if (pair.Key == "ExeAlartSOP")
                    result.ExeAlartSOP = pair.Value;
                else if (pair.Key == "ExeSeriousSOP")
                    result.ExeSeriousSOP = pair.Value;
                else if (pair.Key == "UseTrainingMode")
                    result.UseTrainingMode = pair.Value;
                else if (pair.Key == "UseWaterMark")
                    result.UseWaterMark = pair.Value;
                else if (pair.Key == "UseHeadMessage")
                    result.UseHeadMessage = pair.Value;
                else if (pair.Key == "UseAutoMoveSOPScreen")
                    result.UseAutoMoveSOPScreen = pair.Value;
                else if (pair.Key == "UseBroadcast")
                    result.UseBroadcast = pair.Value;
                else if (pair.Key == "UseSMS")
                    result.UseSMS = pair.Value;
                else if (pair.Key == "UseEmail")
                    result.UseEmail = pair.Value;
                else if (pair.Key == "UseConfirm")
                    result.UseConfirm = pair.Value;
                else if (pair.Key == "WorkingBeginHour")
                    result.WorkingBeginHour = pair.Value;
                else if (pair.Key == "WorkingEndHour")
                    result.WorkingEndHour = pair.Value;
                else if (pair.Key == "UseResultSummary")
                    result.UseResultSummary = pair.Value;
                else if (pair.Key == "DashboardBegin")
                    result.DashboardBegin = pair.Value;
                else if (pair.Key == "DashboardEnd")
                    result.DashboardEnd = pair.Value;
                else if (pair.Key == "FireSOPWaitEndTime")
                    result.FireSOPWaitEndTime = pair.Value;
                else if (pair.Key == "PSMSOPWaitEndTime")
                    result.PSMSOPWaitEndTime = pair.Value;
                else if (pair.Key == "ETCSOPWaitEndTime")
                    result.ETCSOPWaitEndTime = pair.Value;
                else if (pair.Key == "FireSOPRecoverEndTime")
                    result.FireSOPRecoverEndTime = pair.Value;
                else if (pair.Key == "PSMSOPRecoverEndTime")
                    result.PSMSOPRecoverEndTime = pair.Value;
                else if (pair.Key == "ETCSOPRecoverEndTime")
                    result.ETCSOPRecoverEndTime = pair.Value;
            }

            return true;
        }

        public ResponseCommonSettings GetSdmsCommonSettings()
        {
            ResponseCommonSettings result = new ResponseCommonSettings();

            //string strErrorMessage;
            //List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, null, out strErrorMessage);
            List<Common.Model.Option.Options> options = m_sdmsOptions;

            if (options == null)
            {
                result.Success = false;
                result.Message = "timerReadSettings_Elapsed에서 SdmsCommonSettings 조회를 실패하였습니다.";
            }
            else
            {
                foreach (Common.Model.Option.Options option in options)
                {
                    result.AddProperty(option.PropertyName, option.PropertyValue);
                }

                result.Success = true;
            }

            return result;
        }

        public ResponseCommonSettings GetSopCommonSettings()
        {
            ResponseCommonSettings result = new ResponseCommonSettings();

            //string strErrorMessage;
            //List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, null, out strErrorMessage);
            List<Common.Model.Option.Options> options = m_sopOptions;

            if (options == null)
            {
                result.Success = false;
                result.Message = "timerReadSettings_Elapsed에서 SopCommonSettings 조회를 실패하였습니다.";
            }
            else
            {
                foreach (Common.Model.Option.Options option in options)
                {
                    result.AddProperty(option.PropertyName, option.PropertyValue);
                }

                result.Success = true;
            }

            return result;
        }

        public ResponseAccountSettings GetAccountSettings(RequestAccountSettings data)
        {
            ResponseAccountSettings result = new ResponseAccountSettings();
            
            // 유저 설정 정보 불러오기
            Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicCondition = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.UserID, data.UserID);

            string strErrorMessage = null;
            List<SOPManager.Model.Sop.Account.Option> options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            if (options == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            ShortcutKey key = new ShortcutKey();
            PopupState popupState = new PopupState();

            string strShortcutKey = "ShortcutKey";
            string strPopup = "popup";

            // 기본값
            result.IdleTime = "10;1";
            result.TurnStart = "1";
            result.UseAlarmTurn = "false";

            foreach (SOPManager.Model.Sop.Account.Option option in options)
            {
                if (option.Category == strShortcutKey && option.SubCategory == "SDMS")
                    key.SDMS = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "SOP")
                    key.SOP = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "SOPMgr")
                    key.SOPMgr = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "TeamEdit")
                    key.TeamEdit = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "History")
                    key.History = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "Settings")
                    key.Settings = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "Dashboard")
                    key.Dashboard = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "Home")
                    key.Home = option.PropertyValue1;
                else if (option.Category == strShortcutKey && option.SubCategory == "Rotation")
                    key.Rotation = option.PropertyValue1;
                else if (option.Category == "IdleTime")
                    result.IdleTime = option.PropertyValue1;
                else if (option.Category == "TurnStart")
                    result.TurnStart = option.PropertyValue1;
                else if (option.Category == "UseAlarmTurn")
                    result.UseAlarmTurn = option.PropertyValue1;
                else if (option.Category == strPopup)
                {
                    PopupLocation popupLocation = new PopupLocation();
                    popupLocation.X = option.PropertyValue1;
                    popupLocation.Y = option.PropertyValue2;
                    popupLocation.Height = option.PropertyValue3;
                    popupLocation.Width = option.PropertyValue4;

                    if (option.SubCategory == "weatherInfo")
                        popupState.WeatherInfo = popupLocation;
                    else if (option.SubCategory == "statusInfo")
                        popupState.StatusInfo = popupLocation;
                    else if (option.SubCategory == "buildingInfo")
                        popupState.BuildingInfo = popupLocation;
                    else if (option.SubCategory == "dashboardInfo")
                        popupState.DashboardInfo = popupLocation;
                    else if (option.SubCategory == "miniMap")
                        popupState.MiniMap = popupLocation;
                    else if (option.SubCategory == "event")
                        popupState.Event = popupLocation;
                    else if (option.SubCategory == "cctvInfo")
                        popupState.CctvInfo = popupLocation;
                    else if (option.SubCategory == "cctvInfo_1")
                        popupState.CctvInfo_1 = popupLocation;
                    else if (option.SubCategory == "cctvInfo_2")
                        popupState.CctvInfo_2 = popupLocation;
                    else if (option.SubCategory == "cctvInfo_3")
                        popupState.CctvInfo_3 = popupLocation;
                }
            }

            result.ShortcutKey = key;
            result.PopupState = popupState;





            // 단축키 설정 정보 불러오기
            //Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicCondition = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            //dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.UserID, data.UserID);
            //dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.Category, strShortcutKey);

            //string strErrorMessage = null;
            //List<SOPManager.Model.Sop.Account.Option> options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            //if (options == null)
            //{
            //    result.Success = false;
            //    result.Message = strErrorMessage;
            //    return result;
            //}

            //ShortcutKey key = new ShortcutKey();

            //foreach (SOPManager.Model.Sop.Account.Option option in options)
            //{
            //    if (option.SubCategory == "SDMS")
            //    {
            //        key.SDMS = option.PropertyValue1;
            //    }
            //    else if (option.SubCategory == "SOP")
            //    {
            //        key.SOP = option.PropertyValue1;
            //    }
            //    else if (option.SubCategory == "SOPMgr")
            //    {
            //        key.SOPMgr = option.PropertyValue1;
            //    }
            //    else if (option.SubCategory == "TeamEdit")
            //    {
            //        key.TeamEdit = option.PropertyValue1;
            //    }
            //    else if (option.SubCategory == "History")
            //    {
            //        key.History = option.PropertyValue1;
            //    }
            //    else if (option.SubCategory == "Settings")
            //    {
            //        key.Settings = option.PropertyValue1;
            //    }
            //    else if (option.SubCategory == "Dashboard")
            //    {
            //        key.Dashboard = option.PropertyValue1;
            //    }
            //    else if (option.SubCategory == "Home")
            //    {
            //        key.Home = option.PropertyValue1;
            //    }
            //    else if (option.SubCategory == "Rotation")
            //    {
            //        key.Rotation = option.PropertyValue1;
            //    }
            //}

            //result.ShortcutKey = key;

            // 카메라 회전시간 설정 정보 불러오기
            //dicCondition = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            //dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.UserID, data.UserID);
            //dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.Category, "IdleTime");

            //options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            //if (options == null)
            //{
            //    result.Success = false;
            //    result.Message = strErrorMessage;
            //    return result;
            //}

            //string strIdleTime = "10;1";

            //if (options.Count > 0)
            //{
            //    SOPManager.Model.Sop.Account.Option temp = options[0];
            //    strIdleTime = temp.PropertyValue1;
            //}

            //result.IdleTime = strIdleTime;

            // 팝업 위치 불러오기
            //dicCondition = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            //dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.UserID, data.UserID);
            //dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.Category, "popup");

            //options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            //if (options == null)
            //{
            //    result.Success = false;
            //    result.Message = strErrorMessage;
            //    return result;
            //}

            //PopupState popupState = new PopupState();

            //foreach (SOPManager.Model.Sop.Account.Option option in options)
            //{
            //    PopupLocation popupLocation = new PopupLocation();
            //    popupLocation.X = option.PropertyValue1;
            //    popupLocation.Y = option.PropertyValue2;
            //    popupLocation.Height = option.PropertyValue3;
            //    popupLocation.Width = option.PropertyValue4;

            //    if (option.SubCategory == "weatherInfo")
            //        popupState.WeatherInfo = popupLocation;
            //    else if (option.SubCategory == "statusInfo")
            //        popupState.StatusInfo = popupLocation;
            //    else if (option.SubCategory == "buildingInfo")
            //        popupState.BuildingInfo = popupLocation;
            //    else if (option.SubCategory == "dashboardInfo")
            //        popupState.DashboardInfo = popupLocation;
            //    else if (option.SubCategory == "miniMap")
            //        popupState.MiniMap = popupLocation;
            //    else if (option.SubCategory == "event")
            //        popupState.Event = popupLocation;
            //    else if (option.SubCategory == "cctvInfo")
            //        popupState.CctvInfo = popupLocation;
            //    else if (option.SubCategory == "cctvInfo_1")
            //        popupState.CctvInfo_1 = popupLocation;
            //    else if (option.SubCategory == "cctvInfo_2")
            //        popupState.CctvInfo_2 = popupLocation;
            //    else if (option.SubCategory == "cctvInfo_3")
            //        popupState.CctvInfo_3 = popupLocation;
                
            //}

            //result.PopupState = popupState;

            result.Success = true;
            return result;
        }

        public MessageResult SaveSettings(Models.Request.RequestSaveSettings data)
        {
            MessageResult result = new MessageResult();

            string strErrorMessage = null;
            int nSiteID = m_processManager.SopDataManager.SiteID;

            if (data.ShortcutKey != null)
            {   // 단축키 설정 정보 저장
                if (!UpdateAccountOption(data.UserID, "ShortcutKey", "SDMS", data.ShortcutKey.SDMS, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (!UpdateAccountOption(data.UserID, "ShortcutKey", "SOP", data.ShortcutKey.SOP, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (!UpdateAccountOption(data.UserID, "ShortcutKey", "SOPMgr", data.ShortcutKey.SOPMgr, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (!UpdateAccountOption(data.UserID, "ShortcutKey", "TeamEdit", data.ShortcutKey.TeamEdit, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (!UpdateAccountOption(data.UserID, "ShortcutKey", "History", data.ShortcutKey.History, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (!UpdateAccountOption(data.UserID, "ShortcutKey", "Settings", data.ShortcutKey.Settings, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (!UpdateAccountOption(data.UserID, "ShortcutKey", "Dashboard", data.ShortcutKey.Dashboard, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (!UpdateAccountOption(data.UserID, "ShortcutKey", "Home", data.ShortcutKey.Home, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (!UpdateAccountOption(data.UserID, "ShortcutKey", "Rotation", data.ShortcutKey.Rotation, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.IdleTime != null)
            {   // 회전대기 시간 설정
                if (!UpdateAccountOption(data.UserID, "IdleTime", "", data.IdleTime, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.TurnStart != null)
            {   // 자동회전 시작점 설정
                if (!UpdateAccountOption(data.UserID, "TurnStart", "", data.TurnStart, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseAlarmTurn != null)
            {   // 알람시 회전기능  설정
                if (!UpdateAccountOption(data.UserID, "UseAlarmTurn", "", data.UseAlarmTurn, out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.ReAlarm != null)
            {
                if (!UpdateOption(Common.Model.Option.Options.OptionTarget.SDMS, "ReAlarm", data.ReAlarm, nSiteID, "오작동 처리 센서의 재알람 기준 [ 모드(0:모두 재알람, 1:*후 재알람, 2:계속 미알람) / 시간(숫자) / 시간 단위(0:초, 1:분, 2:시간) ]", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseReceiveFire != null)
            {
                if (!UpdateOption(Common.Model.Option.Options.OptionTarget.SDMS, "UseReceiveFire", data.UseReceiveFire, nSiteID, "화재 알람 신호 수신 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseReceivePSM != null)
            {
                if (!UpdateOption(Common.Model.Option.Options.OptionTarget.SDMS, "UseReceivePSM", data.UseReceivePSM, nSiteID, "누출 알람 신호 수신 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseReceiveETC != null)
            {
                if (!UpdateOption(Common.Model.Option.Options.OptionTarget.SDMS, "UseReceiveETC", data.UseReceiveETC, nSiteID, "기타 알람 신호 수신 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseReceiveSVMS != null)
            {
                if (!UpdateOption(Common.Model.Option.Options.OptionTarget.SDMS, "UseReceiveSVMS", data.UseReceiveSVMS, nSiteID, "SVMS 알람 신호 수신 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.EventInfoDisplayTerm != null)
            {
                if (!UpdateOption(Options.OptionTarget.SDMS, "EventInfoDisplayTerm", data.EventInfoDisplayTerm, nSiteID, "이벤트 정보창 표출 리스트 기간 설정", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseScreenMove != null)
            {
                if (!UpdateOption(Options.OptionTarget.SDMS, "UseScreenMove", data.UseScreenMove, nSiteID, "종료/오탐지시 화면 이동 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseAlarmBroadcast != null)
            {
                if (!UpdateOption(Options.OptionTarget.SDMS, "UseAlarmBroadcast", data.UseAlarmBroadcast, nSiteID, "센서로부터 탐지신호를 받으면 방송을 내보낼 것인가?", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.ExeCautionSOP != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "ExeCautionSOP", data.ExeCautionSOP, nSiteID, "주의 알람 감지시 SOP 실행 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.ExeAlartSOP != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "ExeAlartSOP", data.ExeAlartSOP, nSiteID, "경계 알람 감지시 SOP 실행 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.ExeSeriousSOP != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "ExeSeriousSOP", data.ExeSeriousSOP, nSiteID, "심각 알람 감지시 SOP 실행 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseTrainingMode != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "UseTrainingMode", data.UseTrainingMode, nSiteID, "모든 센서 신호 수신시 훈련모드로 사용 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseWaterMark != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "UseWaterMark", data.UseWaterMark, nSiteID, "훈련모드 워터마크 사용 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseHeadMessage != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "UseHeadMessage", data.UseHeadMessage, nSiteID, "전파 메시지 앞머리 문구 지정", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.MoveDisplayAlarm != null)
            {
                if (!UpdateOption(Options.OptionTarget.SDMS, "MoveDisplayAlarm", data.MoveDisplayAlarm, nSiteID, "알람시 자동 화면 전환 기능 설정", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseAutoMoveSOPScreen != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "UseAutoMoveSOPScreen", data.UseAutoMoveSOPScreen, nSiteID, "실행중인 컴포넌트로 자동 화면 이동 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseBroadcast != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "UseBroadcast", data.UseBroadcast, nSiteID, "방송 전파 사용 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseSMS != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "UseSMS", data.UseSMS, nSiteID, "문자 전파 사용 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseEmail != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "UseEmail", data.UseEmail, nSiteID, "이메일 전파 사용 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseConfirm != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "UseConfirm", data.UseConfirm, nSiteID, "상황 전파시 확인단계 거치기 설정", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.WorkingBeginHour != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "WorkingBeginHour", data.WorkingBeginHour, nSiteID, "평일 주간 시작 시간", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.WorkingEndHour != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "WorkingEndHour", data.WorkingEndHour, nSiteID, "평일 주간 종료 시간", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UseResultSummary != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "UseResultSummary", data.UseResultSummary, nSiteID, "SOP 결과 요약창 설정 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.DashboardBegin != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "DashboardBegin", data.DashboardBegin, nSiteID, "대시보드 시작일", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.DashboardEnd != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "DashboardEnd", data.DashboardEnd, nSiteID, "대시보드 종료일", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.FireSOPWaitEndTime != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "FireSOPWaitEndTime", data.FireSOPWaitEndTime, nSiteID, "화재 SOP 대기 자동 종료 설정 [ 시간 / 시간단위(0:초,1:분,2:시간) / 종료모드(0:자동종료, 1:확인 후 종료, 2:종료안함) ]", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PSMSOPWaitEndTime != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "PSMSOPWaitEndTime", data.PSMSOPWaitEndTime, nSiteID, "누출 SOP 대기 자동 종료 설정 [ 시간 / 시간단위(0:초,1:분,2:시간) / 종료모드(0:자동종료, 1:확인 후 종료, 2:종료안함) ]", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.ETCSOPWaitEndTime != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "ETCSOPWaitEndTime", data.ETCSOPWaitEndTime, nSiteID, "기타 SOP 대기 자동 종료 설정 [ 시간 / 시간단위(0:초,1:분,2:시간) / 종료모드(0:자동종료, 1:확인 후 종료, 2:종료안함) ]", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.FireSOPRecoverEndTime != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "FireSOPRecoverEndTime", data.FireSOPRecoverEndTime, nSiteID, "화재 SOP 복구 자동 종료 설정 [ 시간 / 시간단위(0:초,1:분,2:시간) / 종료모드(0:자동종료, 1:확인 후 종료, 2:종료안함) ]", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PSMSOPRecoverEndTime != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "PSMSOPRecoverEndTime", data.PSMSOPRecoverEndTime, nSiteID, "누출 SOP 복구 자동 종료 설정 [ 시간 / 시간단위(0:초,1:분,2:시간) / 종료모드(0:자동종료, 1:확인 후 종료, 2:종료안함) ]", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.ETCSOPRecoverEndTime != null)
            {
                if (!UpdateOption(Options.OptionTarget.SOPSimulator, "ETCSOPRecoverEndTime", data.ETCSOPRecoverEndTime, nSiteID, "기타 SOP 복구 자동 종료 설정 [ 시간 / 시간단위(0:초,1:분,2:시간) / 종료모드(0:자동종료, 1:확인 후 종료, 2:종료안함) ]", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UsePoiFocus != null)
            {
                if (!UpdateOption(Options.OptionTarget.SDMS, "UsePoiFocus", data.UsePoiFocus, nSiteID, "이벤트 관련 POI에 카메라 포커싱 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.UsePoiHighlight != null)
            {
                if (!UpdateOption(Options.OptionTarget.SDMS, "UsePoiHighlight", data.UsePoiHighlight, nSiteID, "POI 선택시 선택된 POI 및 같은 공간의 POI 확대 여부", out strErrorMessage))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            result.Success = true;
            return result;
        }

        public MessageResult UpdateSettings(RequestUpdateSettings settings)
        {
            Dictionary<Options.Fields, object> dicSets = new Dictionary<Options.Fields, object>();
            Dictionary<Options.Fields, object> dicConditions = new Dictionary<Options.Fields, object>();
            string strErrorMessage;

            MessageResult result = new MessageResult();

            foreach (RequestUpdateSettings.PropertyData prop in settings.Properties)
            {
                dicSets[Options.Fields.PropertyValue] = prop.Value;
                dicConditions[Options.Fields.PropertyName] = prop.Name;

                List<Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption((Options.OptionTarget)settings.OptionTarget, prop.Name, out strErrorMessage);

                if (options == null)
                {
                    System.Diagnostics.Trace.WriteLine("UpdateSdmsSettings Error : " + strErrorMessage);
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
                else if (options.Count == 0)
                {
                    if (m_processManager.CommonDataManager.GetCreateManager().CreateOption((Options.OptionTarget)settings.OptionTarget, prop.Name, prop.Value, m_processManager.CommonDataManager.SiteID) == null)
                    {
                        System.Diagnostics.Trace.WriteLine("UpdateSdmsSettings Error : " + m_processManager.CommonDataManager.GetCreateManager().GetErrorMessage());
                        result.Success = false;
                        result.Message = m_processManager.CommonDataManager.GetCreateManager().GetErrorMessage();
                        return result;
                    }
                }
                else
                {
                    if (m_processManager.CommonDataManager.GetUpdateManager().UpdateOption((Options.OptionTarget)settings.OptionTarget, dicSets, dicConditions, null, out strErrorMessage) == false)
                    {
                        System.Diagnostics.Trace.WriteLine("UpdateSdmsSettings Error : " + strErrorMessage);
                        result.Success = false;
                        result.Message = strErrorMessage;
                        return result;
                    }
                }
            }

            result.Success = true;
            return result;
        }

        private bool UpdateOption(Common.Model.Option.Options.OptionTarget type, string strPropertyName, string strPropertyValue, int nSiteID, string strDescription, out string strErrorMessage)
        {
            List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(type, strPropertyName, out strErrorMessage);
            if (options == null)
            {
                return false;
            }

            if (options.Count == 0)
            {
                Common.Model.Option.Options option = m_processManager.CommonDataManager.GetCreateManager().CreateOption(type, strPropertyName, strPropertyValue, nSiteID, strDescription);

                if (option == null)
                {
                    strErrorMessage = strPropertyName + " CreateOption 실패.";
                    return false;
                }
            }
            else if (options.Count > 0)
            {
                Common.Model.Option.Options optionData = options[0];
                optionData.PropertyValue = strPropertyValue;
                string strCondition = "ID = " + optionData.ID.ToString();

                if (!m_processManager.CommonDataManager.GetUpdateManager().UpdateOption(type, optionData, strCondition))
                {
                    strErrorMessage = strPropertyName + " UpdateOption 실패.";
                    return false;
                }
            }

            return true;
        }

        public MessageResult ResetPopup(Models.Request.RequestResetPopup data)
        {
            MessageResult result = new MessageResult();

            string strCategory = "popup";
            string strErrorMessage = null;

            if (data.PopupState.WeatherInfo != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "weatherInfo", data.PopupState.WeatherInfo.X, out strErrorMessage, data.PopupState.WeatherInfo.Y, data.PopupState.WeatherInfo.Height, data.PopupState.WeatherInfo.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PopupState.StatusInfo != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "statusInfo", data.PopupState.StatusInfo.X, out strErrorMessage, data.PopupState.StatusInfo.Y, data.PopupState.StatusInfo.Height, data.PopupState.StatusInfo.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PopupState.BuildingInfo != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "buildingInfo", data.PopupState.BuildingInfo.X, out strErrorMessage, data.PopupState.BuildingInfo.Y, data.PopupState.BuildingInfo.Height, data.PopupState.BuildingInfo.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PopupState.DashboardInfo != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "dashboardInfo", data.PopupState.DashboardInfo.X, out strErrorMessage, data.PopupState.DashboardInfo.Y, data.PopupState.DashboardInfo.Height, data.PopupState.DashboardInfo.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PopupState.MiniMap != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "miniMap", data.PopupState.MiniMap.X, out strErrorMessage, data.PopupState.MiniMap.Y, data.PopupState.MiniMap.Height, data.PopupState.MiniMap.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PopupState.Event != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "event", data.PopupState.Event.X, out strErrorMessage, data.PopupState.Event.Y, data.PopupState.Event.Height, data.PopupState.Event.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PopupState.CctvInfo != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "cctvInfo", data.PopupState.CctvInfo.X, out strErrorMessage, data.PopupState.CctvInfo.Y, data.PopupState.CctvInfo.Height, data.PopupState.CctvInfo.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PopupState.CctvInfo_1 != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "cctvInfo_1", data.PopupState.CctvInfo_1.X, out strErrorMessage, data.PopupState.CctvInfo_1.Y, data.PopupState.CctvInfo_1.Height, data.PopupState.CctvInfo_1.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PopupState.CctvInfo_2 != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "cctvInfo_2", data.PopupState.CctvInfo_2.X, out strErrorMessage, data.PopupState.CctvInfo_2.Y, data.PopupState.CctvInfo_2.Height, data.PopupState.CctvInfo_2.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (data.PopupState.CctvInfo_3 != null)
            {
                if (!UpdateAccountOption(data.UserID, strCategory, "cctvInfo_3", data.PopupState.CctvInfo_3.X, out strErrorMessage, data.PopupState.CctvInfo_3.Y, data.PopupState.CctvInfo_3.Height, data.PopupState.CctvInfo_3.Width))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            result.Success = true;
            return result;
        }

        public MessageResult SetAccoutPopup(Models.Request.RequestSetAccoutPopup data)
        {
            MessageResult result = new MessageResult();

            string strCategory = "popup";
            string strErrorMessage = null;

            // 팝업창 설정 정보 불러오기
            Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicCondition = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.UserID, data.UserID);
            dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.Category, strCategory);

            List<SOPManager.Model.Sop.Account.Option> options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            if (options == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            string strAccountCategory = "accountPopup";

            // 현재 팝업창 설정을 계정 팝업창 설정으로 저장
            foreach (SOPManager.Model.Sop.Account.Option option in options)
            {
                if (!UpdateAccountOption(data.UserID, strAccountCategory, option.SubCategory, option.PropertyValue1, out strErrorMessage, option.PropertyValue2, option.PropertyValue3, option.PropertyValue4))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

            }

            result.Success = true;
            return result;
        }

        public ResponseAccountPopup ResetAccoutPopup(Models.Request.RequestResetAccoutPopup data)
        {
            ResponseAccountPopup result = new ResponseAccountPopup();

            string strCategory = "accountPopup";
            string strErrorMessage = null;

            // 사용자 팝업창 설정 정보 불러오기
            Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicCondition = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.UserID, data.UserID);
            dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.Category, strCategory);

            List<SOPManager.Model.Sop.Account.Option> options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            if (options == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            string strCategory2 = "popup";

            // 팝업창 설정으로 저장
            foreach (SOPManager.Model.Sop.Account.Option option in options)
            {
                if (!UpdateAccountOption(data.UserID, strCategory2, option.SubCategory, option.PropertyValue1, out strErrorMessage, option.PropertyValue2, option.PropertyValue3, option.PropertyValue4))
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

            }

            result.AccountPopups = options;
            result.Success = true;
            return result;
        }

        private bool UpdateAccountOption(int nUserID, string strCategory, string strSubCategory, string strPropertyValue1, out string strErrorMessage, string strPropertyValue2 = "", string strPropertyValue3 = "", string strPropertyValue4 = "")
        {
            strErrorMessage = "";
            //string strCategory = "ShortcutKey";

            Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object> dicCondition = new Dictionary<SOPManager.Model.Sop.Account.Option.Fields, object>();
            dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.UserID, nUserID);
            dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.Category, strCategory);
            dicCondition.Add(SOPManager.Model.Sop.Account.Option.Fields.SubCategory, strSubCategory);

            List<SOPManager.Model.Sop.Account.Option> options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            if (options == null)
            {
                return false;
            }

            if (options.Count == 0)
            {   // 새로 생성
                SOPManager.Model.Sop.Account.Option retOption = m_processManager.SopDataManager.GetCreateManager().CreateOption(nUserID, strCategory, strSubCategory, strPropertyValue1, strPropertyValue2, strPropertyValue3, strPropertyValue4);

                if (retOption == null)
                {
                    strErrorMessage = strCategory + " " + strSubCategory + " CreateOption 실패.";
                    return false;
                }
            }
            else if (options.Count > 0)
            {   // 업데이트
                SOPManager.Model.Sop.Account.Option optionData = options[0];
                optionData.PropertyValue1 = strPropertyValue1;
                optionData.PropertyValue2 = strPropertyValue2;
                optionData.PropertyValue3 = strPropertyValue3;
                optionData.PropertyValue4 = strPropertyValue4;

                if (!m_processManager.SopDataManager.GetUpdateManager().UpdateOption(optionData))
                {
                    strErrorMessage = strCategory + " " + strSubCategory + " UpdateOption 실패.";
                    return false;
                }
            }

            return true;
        }

        public MessageResult UpdateLinkedSOPs(RequestUpdateLinkedSOPs data)
        {
            MessageResult result = new MessageResult();
            string strErrorMessage = "";
            string strAdditionalConditions = null;

            List<SOPManager.Model.Sop.Config.LinkedSop> addLinkedSops = data.AddLinkedSops;
            List<SOPManager.Model.Sop.Config.LinkedSop> updateLinkedSops = data.UpdateLinkedSops;
            List<SOPManager.Model.Sop.Config.LinkedSop> removeLinkedSops = data.RemoveLinkedSops;

            foreach (SOPManager.Model.Sop.Config.LinkedSop linkedSop in addLinkedSops)
            {
                SOPManager.Model.Sop.Config.LinkedSop linkedSopData = m_processManager.SopDataManager.GetCreateManager().CreateLinkedSop(linkedSop.FacilityTypeID, linkedSop.DisasterCategoryID, linkedSop.SubDisasterCategoryID, linkedSop.DisasterName, linkedSop.LinkedBuildingID, linkedSop.LinkedZoneID, linkedSop.Description);

                if (linkedSopData == null)
                {
                    result.Success = false;
                    result.Message = "CreateLinkedSop 실패";
                    return result;
                }
            }

            foreach (SOPManager.Model.Sop.Config.LinkedSop linkedSop in updateLinkedSops)
            {
                if (m_processManager.SopDataManager.GetUpdateManager().UpdateLinkedSop(linkedSop) == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            foreach (SOPManager.Model.Sop.Config.LinkedSop linkedSop in removeLinkedSops)
            {
                if (m_processManager.SopDataManager.GetDeleteManager().DeleteLinkedSop(linkedSop.ID) == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            result.Success = true;
            return result;
        }

        public MessageResult OnOffBroadcast(RequestOnOffBroadcast data)
        {
            MessageResult result = new MessageResult();
            string strErrorMessage = "";
            string CloseBroadcastTag = "CloseAlarmBroadcast";

            List<Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Options.OptionTarget.SDMS, CloseBroadcastTag, out strErrorMessage);

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

                    if (data.OnOff == "true")
                    {   // 방송 시작
                        string strBuildingID = data.BuildingID;
                        int nID;

                        if (strBuildingID != null && int.TryParse(strBuildingID, out nID) &&
                            IDs.Contains(nID) == true)
                        {
                            bChkUpdate = true;
                            IDs.Remove(nID);
                        }
                    }
                    else if (data.OnOff == "false")
                    {   // 방송 중지
                        string strBuildingID = data.BuildingID;
                        int nID;

                        if (strBuildingID != null && int.TryParse(strBuildingID, out nID) &&
                            IDs.Contains(nID) == false)
                        {
                            bChkUpdate = true;
                            IDs.Add(nID);
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
                        m_processManager.CommonDataManager.GetUpdateManager().UpdateOption(Options.OptionTarget.SDMS, option);
                    }
                }
            }
            else if (options.Count == 0)
            {
                string strBuildingIDs = "";

                if (data.OnOff == "false")
                {
                    string strBuildingID = data.BuildingID;
                    int nID;

                    if (strBuildingID != null && int.TryParse(strBuildingID, out nID))
                    {
                        strBuildingIDs = nID.ToString();
                    }

                    m_processManager.CommonDataManager.GetCreateManager().CreateOption(Options.OptionTarget.SDMS, CloseBroadcastTag, strBuildingIDs, m_processManager.SopDataManager.SiteID);
                }
                else if (data.OnOff == "true")
                {
                    m_processManager.CommonDataManager.GetCreateManager().CreateOption(Options.OptionTarget.SDMS, CloseBroadcastTag, strBuildingIDs, m_processManager.SopDataManager.SiteID);
                }
            }

            result.Success = true;
            return result;
        }
    }
}
