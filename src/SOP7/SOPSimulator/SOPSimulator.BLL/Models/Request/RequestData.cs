using System;
using System.Collections.Generic;

namespace SOPSimulator.BLL.Models.Request
{
    public class RequestData
    {
    }

    /// <summary>
    /// SOP 실행할 때 사용
    /// </summary>
    public class RequestExcuteSOP
    {
        private SOPManager.BLL.Models.SOP.ActionStepData m_actionStepData = null;
        public SOPManager.BLL.Models.SOP.ActionStepData ActionStepData
        {
            get { return m_actionStepData; }
            set { m_actionStepData = value; }
        }

        private string m_beginTime = "";
        public string BeginTime
        {
            get { return m_beginTime; }
            set { m_beginTime = value; }
        }
        
        private int m_nActionStepID = -1;
        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        private string m_strPosition = "";
        public string Position
        {
            get { return m_strPosition; }
            set { m_strPosition = value; }
        }

        private int? m_nLastAccessedUserID = null;
        public int? LastAccessedUserID
        {
            get { return m_nLastAccessedUserID; }
            set { m_nLastAccessedUserID = value; }
        }

        private int? m_nSensorZoneHistoryID = -1;
        public int? SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        private int m_realMode = 1;
        public int RealMode
        {
            get { return m_realMode; }
            set { m_realMode = value; }
        }
    }

    /// <summary>
    /// SOP 종료할 때 사용
    /// </summary>
    public class RequestCloseSOP
    {

        private int m_nActionStepHistoryID = -1;
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        private string m_strEndTime = "";
        public string EndTime
        {
            get { return m_strEndTime; }
            set { m_strEndTime = value; }
        }

        private int? m_nLastAccessedUserID = null;
        public int? LastAccessedUserID
        {
            get { return m_nLastAccessedUserID; }
            set { m_nLastAccessedUserID = value; }
        }
    }

    /// <summary>
    /// SOP 진행할 때 사용
    /// </summary>
    public class RequestProgressSOP
    {
        private string m_strSopKey = "";
        public string SopKey
        {
            get { return m_strSopKey; }
            set { m_strSopKey = value; }
        }

        private int m_nActionStepID = -1;
        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        private int m_nActionStepHistoryID = -1;
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        private int m_nComponentID = -1;
        public int ComponentID
        {
            get { return m_nComponentID; }
            set { m_nComponentID = value; }
        }

        private int m_nComponentType = -1;
        public int ComponentType
        {
            get { return m_nComponentType; }
            set { m_nComponentType = value; }
        }

        private int m_nStatus = -1;
        public int Status
        {
            get { return m_nStatus; }
            set { m_nStatus = value; }
        }

        private int? m_nAccessedUserID = null;
        public int? AccessedUserID
        {
            get { return m_nAccessedUserID; }
            set { m_nAccessedUserID = value; }
        }

        private string m_strText = "";
        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        private string m_strDecisionValue = "";
        public string DecisionValue
        {
            get { return m_strDecisionValue; }
            set { m_strDecisionValue = value; }
        }

        private bool m_bSkip = false;
        /// <summary>
        /// 현재 임무가 아닌 다른 임무의 다음버튼을 눌렀을 때 true
        /// </summary>
        public bool Skip
        {
            get { return m_bSkip; }
            set { m_bSkip = value; }
        }
    }

    /// <summary>
    /// 임무 체크할 때 사용
    /// </summary>
    public class RequestProgressMission
    {
        private string m_strSopKey = "";
        public string SopKey
        {
            get { return m_strSopKey; }
            set { m_strSopKey = value; }
        }

        //private int m_nActionStepID = -1;
        //public int ActionStepID
        //{
        //    get { return m_nActionStepID; }
        //    set { m_nActionStepID = value; }
        //}

        private int m_nActionStepHistoryID = -1;
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        private int m_nComponentID = -1;
        public int ComponentID
        {
            get { return m_nComponentID; }
            set { m_nComponentID = value; }
        }

        private int m_nComponentType = -1;
        public int ComponentType
        {
            get { return m_nComponentType; }
            set { m_nComponentType = value; }
        }

        private int m_nDataIndex = -1;
        public int DataIndex
        {
            get { return m_nDataIndex; }
            set { m_nDataIndex = value; }
        }

        private int m_nComponentStatus = -1;
        public int ComponentStatus
        {
            get { return m_nComponentStatus; }
            set { m_nComponentStatus = value; }
        }

        private int? m_nAccessedUserID = null;
        public int? AccessedUserID
        {
            get { return m_nAccessedUserID; }
            set { m_nAccessedUserID = value; }
        }

        private bool m_bChecked = false;
        public bool Checked
        {
            get { return m_bChecked; }
            set { m_bChecked = value; }
        }
    }

    /// <summary>
    /// 내부 상황 전파(문자, 메일, 방송), 프로세스 임무 개별 전파 (문자, 메일)
    /// </summary>
    public class RequestProgressInternalSpread
    {
        private string m_strSopKey = "";
        public string SopKey
        {
            get { return m_strSopKey; }
            set { m_strSopKey = value; }
        }

        //private int m_nActionStepID = -1;
        //public int ActionStepID
        //{
        //    get { return m_nActionStepID; }
        //    set { m_nActionStepID = value; }
        //}

        private int m_nActionStepHistoryID = -1;
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        private int m_nComponentID = -1;
        public int ComponentID
        {
            get { return m_nComponentID; }
            set { m_nComponentID = value; }
        }

        private int m_nComponentType = -1;
        public int ComponentType
        {
            get { return m_nComponentType; }
            set { m_nComponentType = value; }
        }

        private int m_nDataIndex = -1;
        public int DataIndex
        {
            get { return m_nDataIndex; }
            set { m_nDataIndex = value; }
        }

        private int m_nComponentStatus = -1;
        public int ComponentStatus
        {
            get { return m_nComponentStatus; }
            set { m_nComponentStatus = value; }
        }

        private int? m_nAccessedUserID = null;
        public int? AccessedUserID
        {
            get { return m_nAccessedUserID; }
            set { m_nAccessedUserID = value; }
        }               

        private bool m_isSMS = false;
        public bool IsSMS
        {
            get { return m_isSMS; }
            set { m_isSMS = value; }
        }

        private bool m_isEmail = false;
        public bool IsEmail
        {
            get { return m_isEmail; }
            set { m_isEmail = value; }
        }

        private bool m_isBroadcast = false;
        public bool IsBroadcast
        {
            get { return m_isBroadcast; }
            set { m_isBroadcast = value; }
        }

        private bool m_isSiren = false;
        public bool IsSiren
        {
            get { return m_isSiren; }
            set { m_isSiren = value; }
        }

        private string m_strMessage = "";
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }
    }

    public class RequestSensorName
    {
        private int m_nSensorZoneHistoryID = -1;
        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }
    }
}
