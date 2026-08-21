using System;
using System.Collections.Generic;
using System.Text;

namespace Common.BLL.Models.Response
{
    public class ResponseSettings : MessageResult
    {
        private ShortcutKey m_shortcutKey = null;
        private string m_strIdleTime = null;
        private string m_strReAlarm = null;
        private string m_strUseReceiveFire = null;
        private string m_strUseReceivePSM = null;
        private string m_strUseReceiveETC = null;
        private string m_strUseReceiveSVMS = null;
        private string m_strEventInfoDisplayTerm = null;
        private string m_strUseScreenMove = null;
        // 센서로부터 탐지신호를 받으면 자동으로 방송을 내보낼 것인가?
        private string m_strUseAlarmBroadcast = null;
        private string m_strExeCautionSOP = null;
        private string m_strExeAlartSOP = null;
        private string m_strExeSeriousSOP = null;
        private string m_strUseTrainingMode = null;
        private string m_strUseWaterMark = null;
        private string m_strUseHeadMessage = null;
        private string m_strUseAutoMoveSOPScreen = null;
        private string m_strUseBroadcast = null;
        private string m_strUseSMS = null;
        private string m_strUseEmail = null;
        private string m_strUseConfirm = null;
        private string m_strWorkingBeginHour = null;
        private string m_strWorkingEndHour = null;
        private string m_strUseResultSummary = null;
        private string m_strDashboardBegin = null;
        private string m_strDashboardEnd = null;
        private string m_strFireSOPWaitEndTime = null;
        private string m_strPSMSOPWaitEndTime = null;
        private string m_strETCSOPWaitEndTime = null;
        private string m_strFireSOPRecoverEndTime = null;
        private string m_strPSMSOPRecoverEndTime = null;
        private string m_strETCSOPRecoverEndTime = null;
        private string m_strMoveDisplayAlarm = null;
        private string m_strUsePoiFocus = null;
        private string m_strUsePoiHighlight = null;
        private string m_strTurnStart = null;
        private string m_strUseAlarmTurn = null;

        public ShortcutKey ShortcutKey
        {
            get { return m_shortcutKey; }
            set { m_shortcutKey = value; }
        }

        public string IdleTime
        {
            get { return m_strIdleTime; }
            set { m_strIdleTime = value; }
        }

        public string ReAlarm
        {
            get { return m_strReAlarm; }
            set { m_strReAlarm = value; }
        }

        public string UseReceiveFire
        {
            get { return m_strUseReceiveFire; }
            set { m_strUseReceiveFire = value; }
        }

        public string UseReceivePSM
        {
            get { return m_strUseReceivePSM; }
            set { m_strUseReceivePSM = value; }
        }

        public string UseReceiveETC
        {
            get { return m_strUseReceiveETC; }
            set { m_strUseReceiveETC = value; }
        }


        public string UseReceiveSVMS
        {
            get { return m_strUseReceiveSVMS; }
            set { m_strUseReceiveSVMS = value; }
        }

        public string EventInfoDisplayTerm
        {
            get { return m_strEventInfoDisplayTerm; }
            set { m_strEventInfoDisplayTerm = value; }
        }

        public string UseScreenMove
        {
            get { return m_strUseScreenMove; }
            set { m_strUseScreenMove = value; }
        }

        // 센서로부터 탐지신호를 받으면 자동으로 방송을 내보낼 것인가?
        public string UseAlarmBroadcast
        {
            get { return m_strUseAlarmBroadcast; }
            set { m_strUseAlarmBroadcast = value; }
        }

        public string ExeCautionSOP
        {
            get { return m_strExeCautionSOP; }
            set { m_strExeCautionSOP = value; }
        }

        public string ExeAlartSOP
        {
            get { return m_strExeAlartSOP; }
            set { m_strExeAlartSOP = value; }
        }

        public string ExeSeriousSOP
        {
            get { return m_strExeSeriousSOP; }
            set { m_strExeSeriousSOP = value; }
        }

        public string UseTrainingMode
        {
            get { return m_strUseTrainingMode; }
            set { m_strUseTrainingMode = value; }
        }

        public string UseWaterMark
        {
            get { return m_strUseWaterMark; }
            set { m_strUseWaterMark = value; }
        }

        public string UseHeadMessage
        {
            get { return m_strUseHeadMessage; }
            set { m_strUseHeadMessage = value; }
        }

        public string UseAutoMoveSOPScreen
        {
            get { return m_strUseAutoMoveSOPScreen; }
            set { m_strUseAutoMoveSOPScreen = value; }
        }

        public string UseBroadcast
        {
            get { return m_strUseBroadcast; }
            set { m_strUseBroadcast = value; }
        }

        public string UseSMS
        {
            get { return m_strUseSMS; }
            set { m_strUseSMS = value; }
        }

        public string UseEmail
        {
            get { return m_strUseEmail; }
            set { m_strUseEmail = value; }
        }

        public string UseConfirm
        {
            get { return m_strUseConfirm; }
            set { m_strUseConfirm = value; }
        }

        public string WorkingBeginHour
        {
            get { return m_strWorkingBeginHour; }
            set { m_strWorkingBeginHour = value; }
        }

        public string WorkingEndHour
        {
            get { return m_strWorkingEndHour; }
            set { m_strWorkingEndHour = value; }
        }

        public string UseResultSummary
        {
            get { return m_strUseResultSummary; }
            set { m_strUseResultSummary = value; }
        }

        public string DashboardBegin
        {
            get { return m_strDashboardBegin; }
            set { m_strDashboardBegin = value; }
        }

        public string DashboardEnd
        {
            get { return m_strDashboardEnd; }
            set { m_strDashboardEnd = value; }
        }

        public string FireSOPWaitEndTime
        {
            get { return m_strFireSOPWaitEndTime; }
            set { m_strFireSOPWaitEndTime = value; }
        }
        public string PSMSOPWaitEndTime
        {
            get { return m_strPSMSOPWaitEndTime; }
            set { m_strPSMSOPWaitEndTime = value; }
        }

        public string ETCSOPWaitEndTime
        {
            get { return m_strETCSOPWaitEndTime; }
            set { m_strETCSOPWaitEndTime = value; }
        }

        public string FireSOPRecoverEndTime
        {
            get { return m_strFireSOPRecoverEndTime; }
            set { m_strFireSOPRecoverEndTime = value; }
        }

        public string PSMSOPRecoverEndTime
        {
            get { return m_strPSMSOPRecoverEndTime; }
            set { m_strPSMSOPRecoverEndTime = value; }
        }

        public string ETCSOPRecoverEndTime
        {
            get { return m_strETCSOPRecoverEndTime; }
            set { m_strETCSOPRecoverEndTime = value; }
        }

        public string MoveDisplayAlarm
        {
            get { return m_strMoveDisplayAlarm; }
            set { m_strMoveDisplayAlarm = value; }
        }

        public string UsePoiFocus
        {
            get { return m_strUsePoiFocus; }
            set { m_strUsePoiFocus = value; }
        }

        public string UsePoiHighlight
        {
            get { return m_strUsePoiHighlight; }
            set { m_strUsePoiHighlight = value; }
        }

        public string TurnStart
        {
            get { return m_strTurnStart; }
            set { m_strTurnStart = value; }
        }

        public string UseAlarmTurn
        {
            get { return m_strUseAlarmTurn; }
            set { m_strUseAlarmTurn = value; }
        }
    }

    public class ShortcutKey
    {
        private string m_strSDMS = "";
        private string m_strSOP = "";
        private string m_strSOPMgr = "";
        private string m_strTeamEdit = "";
        private string m_strHistory = "";
        private string m_strSettings = "";
        private string m_strDashboard = "";
        private string m_strHome = "";
        private string m_strRotation = "";

        public string SDMS
        {
            get { return m_strSDMS; }
            set { m_strSDMS = value; }
        }

        public string SOP
        {
            get { return m_strSOP; }
            set { m_strSOP = value; }
        }

        public string SOPMgr
        {
            get { return m_strSOPMgr; }
            set { m_strSOPMgr = value; }
        }

        public string TeamEdit
        {
            get { return m_strTeamEdit; }
            set { m_strTeamEdit = value; }
        }

        public string History
        {
            get { return m_strHistory; }
            set { m_strHistory = value; }
        }

        public string Settings
        {
            get { return m_strSettings; }
            set { m_strSettings = value; }
        }

        public string Dashboard
        {
            get { return m_strDashboard; }
            set { m_strDashboard = value; }
        }

        public string Home
        {
            get { return m_strHome; }
            set { m_strHome = value; }
        }

        public string Rotation
        {
            get { return m_strRotation; }
            set { m_strRotation = value; }
        }
    }

    public class ResponseCommonSettings : MessageResult
    {
        public class PropertyData
        {
            public string Name
            {
                get; set;
            }

            public string Value
            {
                get; set;
            }

            public string Description
            {
                get; set;
            }
        }

        private List<PropertyData> m_properties = new List<PropertyData>();

        public List<PropertyData> Properties
        {
            get { return m_properties; }
            set { m_properties = value; }
        }

        public void AddProperty(string strName, string strValue, string strDescription = null)
        {
            PropertyData data = new PropertyData();

            data.Name = strName;
            data.Value = strValue;
            data.Description = strDescription;

            m_properties.Add(data);
        }
    }
}
