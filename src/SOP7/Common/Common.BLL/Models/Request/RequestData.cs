using Common.BLL.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.BLL.Models.Request
{
    public class RequestData
    {
        private RequestSettings m_requestSettings = null;
        private RequestSetAccoutPopup m_requestSetAccoutPopup = null;
        private RequestResetAccoutPopup m_requestResetAccoutPopup = null;
        private bool? m_requestSdmsCommonSettings = null;
        private bool? m_requestSopCommonSettings = null;
        private RequestSaveSettings m_requestSaveSettings = null;
        private RequestUpdateSettings m_requestUpdateSettings = null;
        private RequestResetPopup m_requestResetPopup = null;
        private bool? m_requestDownloadBuilding = null;
        private bool? m_requestDownloadBuildingGroup = null;
        private bool? m_requestDownloadFacility = null;
        private bool? m_requestDownloadRegularTeam = null;
        private bool? m_requestLinkedSOPs = null;
        private RequestUpdateLinkedSOPs m_requestUpdateLinkedSOPs = null;
        private RequestAccountSettings m_requestAccountSettings = null;
        private RequestOnOffBroadcast m_requestOnOffBroadcast = null;

        public RequestSettings RequestSettings
        {
            get { return m_requestSettings; }
            set { m_requestSettings = value; }
        }

        public RequestSetAccoutPopup RequestSetAccoutPopup
        {
            get { return m_requestSetAccoutPopup; }
            set { m_requestSetAccoutPopup = value; }
        }

        public RequestResetAccoutPopup RequestResetAccoutPopup
        {
            get { return m_requestResetAccoutPopup; }
            set { m_requestResetAccoutPopup = value; }
        }

        public bool? RequestSdmsCommonSettings
        {
            get { return m_requestSdmsCommonSettings; }
            set { m_requestSdmsCommonSettings = value; }
        }

        public bool? RequestSopCommonSettings
        {
            get { return m_requestSopCommonSettings; }
            set { m_requestSopCommonSettings = value; }
        }

        public RequestSaveSettings RequestSaveSettings
        {
            get { return m_requestSaveSettings; }
            set { m_requestSaveSettings = value; }
        }

        public RequestUpdateSettings RequestUpdateSettings
        {
            get { return m_requestUpdateSettings; }
            set { m_requestUpdateSettings = value; }
        }

        public RequestResetPopup RequestResetPopup
        {
            get { return m_requestResetPopup; }
            set { m_requestResetPopup = value; }
        }

        public bool? RequestDownloadBuilding
        {
            get { return m_requestDownloadBuilding; }
            set { m_requestDownloadBuilding = value; }
        }

        public bool? RequestDownloadBuildingGroup
        {
            get { return m_requestDownloadBuildingGroup; }
            set { m_requestDownloadBuildingGroup = value; }
        }

        public bool? RequestDownloadFacility
        {
            get { return m_requestDownloadFacility; }
            set { m_requestDownloadFacility = value; }
        }

        public bool? RequestDownloadRegularTeam
        {
            get { return m_requestDownloadRegularTeam; }
            set { m_requestDownloadRegularTeam = value; }
        }

        public bool? RequestLinkedSOPs
        {
            get { return m_requestLinkedSOPs; }
            set { m_requestLinkedSOPs = value; }
        }

        public RequestUpdateLinkedSOPs RequestUpdateLinkedSOPs
        {
            get { return m_requestUpdateLinkedSOPs; }
            set { m_requestUpdateLinkedSOPs = value; }
        }

        public RequestAccountSettings RequestAccountSettings 
        {
            get { return m_requestAccountSettings; }
            set { m_requestAccountSettings = value; }
        }

        public RequestOnOffBroadcast RequestOnOffBroadcast
        {
            get { return m_requestOnOffBroadcast; }
            set { m_requestOnOffBroadcast = value; }
        }
    }

    public class RequestSettings
    {
        private int m_nUserID = -1;
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }
    }

    public class RequestAccountSettings
    {
        private int m_nUserID = -1;
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }
    }

    public class RequestSaveSetting
    {
        private string m_strPropertyName = "";
        private string m_strPropertyValue = "";

        public string PropertyName
        {
            get { return m_strPropertyName; }
            set { m_strPropertyName = value; }
        }
        public string PropertyValue
        {
            get { return m_strPropertyValue; }
            set { m_strPropertyValue = value; }
        }
    }

    public class RequestSetAccoutPopup
    {
        private int m_nUserID = -1;
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }
    }

    public class RequestResetAccoutPopup
    {
        private int m_nUserID = -1;
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }
    }

    public class RequestSaveSettings
    {
        private int m_nUserID = -1;
        private ShortcutKey m_shortcutKey = null;
        private string m_strIdleTime = null;
        private string m_strReAlarm = null;
        private string m_strUseReceiveFire = null;
        private string m_strUseReceivePSM = null;
        private string m_strUseReceiveETC = null;
        private string m_strUseReceiveSVMS = null;
        private string m_strEventInfoDisplayTerm = null;
        private string m_strUseScreenMove = null;
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
        private string m_strUseAlarmBroadcast = null;
        private string m_strUsePoiFocus = null;
        private string m_strUsePoiHighlight = null;
        private string m_strTurnStart = null;
        private string m_strUseAlarmTurn = null;

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

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

    public class RequestUpdateSettings
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
        private int m_nOptionTarget = (int)Common.Model.Option.Options.OptionTarget.NOT_DEFINED;

        public List<PropertyData> Properties
        {
            get { return m_properties; }
            set { m_properties = value; }
        }

        public int OptionTarget
        {
            get { return m_nOptionTarget; }
            set { m_nOptionTarget = value; }
        }
    }

    public class RequestResetPopup
    {
        private int m_nUserID = -1;
        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        private PopupState m_popupState = null;
        public PopupState PopupState
        {
            get { return m_popupState; }
            set { m_popupState = value; }
        }
    }

    public class PopupState
    {
        private PopupLocation m_weatherInfo = null;
        public PopupLocation WeatherInfo
        {
            get { return m_weatherInfo; }
            set { m_weatherInfo = value; }
        }

        private PopupLocation m_statusInfo = null;
        public PopupLocation StatusInfo
        {
            get { return m_statusInfo; }
            set { m_statusInfo = value; }
        }

        private PopupLocation m_buildingInfo = null;
        public PopupLocation BuildingInfo
        {
            get { return m_buildingInfo; }
            set { m_buildingInfo = value; }
        }

        private PopupLocation m_dashboardInfo = null;
        public PopupLocation DashboardInfo
        {
            get { return m_dashboardInfo; }
            set { m_dashboardInfo = value; }
        }

        private PopupLocation m_miniMap = null;
        public PopupLocation MiniMap
        {
            get { return m_miniMap; }
            set { m_miniMap = value; }
        }

        private PopupLocation m_event = null;
        public PopupLocation Event
        {
            get { return m_event; }
            set { m_event = value; }
        }

        private PopupLocation m_cctvInfo = null;
        public PopupLocation CctvInfo
        {
            get { return m_cctvInfo; }
            set { m_cctvInfo = value; }
        }

        private PopupLocation m_cctvInfo_1 = null;
        public PopupLocation CctvInfo_1
        {
            get { return m_cctvInfo_1; }
            set { m_cctvInfo_1 = value; }
        }

        private PopupLocation m_cctvInfo_2 = null;
        public PopupLocation CctvInfo_2
        {
            get { return m_cctvInfo_2; }
            set { m_cctvInfo_2 = value; }
        }

        private PopupLocation m_cctvInfo_3 = null;
        public PopupLocation CctvInfo_3
        {
            get { return m_cctvInfo_3; }
            set { m_cctvInfo_3 = value; }
        }
    }

    public class PopupLocation
    {
        private string m_strX = "";
        private string m_strY = "";
        private string m_strHeight = "";
        private string m_strWidth = "";

        public string X
        {
            get { return m_strX; }
            set { m_strX = value; }
        }

        public string Y
        {
            get { return m_strY; }
            set { m_strY = value; }
        }

        public string Height
        {
            get { return m_strHeight; }
            set { m_strHeight = value; }
        }

        public string Width
        {
            get { return m_strWidth; }
            set { m_strWidth = value; }
        }
    }

    public class RequestUpdateLinkedSOPs
    {
        private List<SOPManager.Model.Sop.Config.LinkedSop> m_addLinkedSops = null;
        private List<SOPManager.Model.Sop.Config.LinkedSop> m_updateLinkedSops = null;
        private List<SOPManager.Model.Sop.Config.LinkedSop> m_removeLinkedSops = null;

        public List<SOPManager.Model.Sop.Config.LinkedSop> AddLinkedSops
        {
            get { return m_addLinkedSops; }
            set { m_addLinkedSops = value; }
        }

        public List<SOPManager.Model.Sop.Config.LinkedSop> UpdateLinkedSops
        {
            get { return m_updateLinkedSops; }
            set { m_updateLinkedSops = value; }
        }

        public List<SOPManager.Model.Sop.Config.LinkedSop> RemoveLinkedSops
        {
            get { return m_removeLinkedSops; }
            set { m_removeLinkedSops = value; }
        }
    }

    public class RequestOnOffBroadcast
    {
        string m_strOnOff = null;
        string m_strBuildingID = null;

        public string OnOff
        {
            get { return m_strOnOff; }
            set { m_strOnOff = value; }
        }

        public string BuildingID
        {
            get { return m_strBuildingID; }
            set { m_strBuildingID = value; }
        }
    }
}
