using System;
using System.Collections.Generic;
using Vacation.Model;

namespace Vacation.BLL.Models.Vacation
{
    public class ManagerRequestData : MessageResult
    {
        private List<WaitingRequest> m_waitingRequests = new List<WaitingRequest>();
        private List<CompletedRequest> m_completedRequests = new List<CompletedRequest>();
        private List<WaitingRequestSpecialVacation> m_waitingRequestSpecialVacations = new List<WaitingRequestSpecialVacation>();
        private List<CompletedRequestSpecialVacation> m_completedRequestSpecialVacations = new List<CompletedRequestSpecialVacation>();

        public List<WaitingRequest> WaitingRequests
        {
            get { return m_waitingRequests; }
            set { m_waitingRequests = value; }
        }

        public List<CompletedRequest> CompletedRequests
        {
            get { return m_completedRequests; }
            set { m_completedRequests = value; }
        }

        public List<WaitingRequestSpecialVacation> WaitingRequestSpecialVacations
        {
            get { return m_waitingRequestSpecialVacations; }
            set { m_waitingRequestSpecialVacations = value; }
        }

        public List<CompletedRequestSpecialVacation> CompletedRequestSpecialVacations
        {
            get { return m_completedRequestSpecialVacations; }
            set { m_completedRequestSpecialVacations = value; }
        }
    }

    public class WaitingRequestSpecialVacation
    {
        private int m_nRequestID = -1;
        private DateTime m_requestTime = new DateTime();
        private int m_nRequestYear = 0, m_nRequestMonth = 0, m_nRequestDay = 0;
        private int m_nRequestHour = 0, m_nRequestMinute = 0, m_nRequestSecond = 0;
        private float m_fDays = 0;
        private Models.Account.ApplicationUser m_requestManager = null;
        private List<Models.Account.ApplicationUser> m_targetMembers = new List<Account.ApplicationUser>();
        private string m_strRequestDescription = "";

        public int RequestID
        {
            get { return m_nRequestID; }
            set { m_nRequestID = value; }
        }

        public DateTime RequestTime
        {
            get { return m_requestTime; }
            set
            {
                m_requestTime = value;
                m_nRequestYear = m_requestTime.Year;
                m_nRequestMonth = m_requestTime.Month;
                m_nRequestDay = m_requestTime.Day;
                m_nRequestHour = m_requestTime.Hour;
                m_nRequestMinute = m_requestTime.Minute;
                m_nRequestSecond = m_requestTime.Second;
            }
        }

        public int RequestYear
        {
            get { return m_nRequestYear; }
        }

        public int RequestMonth
        {
            get { return m_nRequestMonth; }
        }

        public int RequestDay
        {
            get { return m_nRequestDay; }
        }

        public int RequestHour
        {
            get { return m_nRequestHour; }
        }

        public int RequestMinute
        {
            get { return m_nRequestMinute; }
        }

        public int RequestSecond
        {
            get { return m_nRequestSecond; }
        }

        public float Days
        {
            get { return m_fDays; }
            set { m_fDays = value; }
        }

        public Models.Account.ApplicationUser RequestManager
        {
            get { return m_requestManager; }
            set { m_requestManager = value; }
        }

        public List<Models.Account.ApplicationUser> TargetMembers
        {
            get { return m_targetMembers; }
            set { m_targetMembers = value; }
        }

        public string RequestDescription
        {
            get { return m_strRequestDescription; }
            set { m_strRequestDescription = value; }
        }
    }

    public class CompletedRequestSpecialVacation : WaitingRequestSpecialVacation
    {
        private Model.Response.ResponseType m_response = Model.Response.ResponseType.None;
        private DateTime m_responseTime = new DateTime();
        private Comment m_myComment = null;
        private int m_nResponseYear = 0, m_nResponseMonth = 0, m_nResponseDay = 0;
        private int m_nResponseHour = 0, m_nResponseMinute = 0, m_nResponseSecond = 0;

        public Model.Response.ResponseType Response
        {
            get { return m_response; }
            set { m_response = value; }
        }

        public DateTime ResponseTime
        {
            get { return m_responseTime; }
            set
            {
                m_responseTime = value;
                m_nResponseYear = m_responseTime.Year;
                m_nResponseMonth = m_responseTime.Month;
                m_nResponseDay = m_responseTime.Day;
                m_nResponseHour = m_responseTime.Hour;
                m_nResponseMinute = m_responseTime.Minute;
                m_nResponseSecond = m_responseTime.Second;
            }
        }

        public int ResponseYear
        {
            get { return m_nResponseYear; }
        }

        public int ResponseMonth
        {
            get { return m_nResponseMonth; }
        }

        public int ResponseDay
        {
            get { return m_nResponseDay; }
        }

        public int ResponseHour
        {
            get { return m_nResponseHour; }
        }

        public int ResponseMinute
        {
            get { return m_nResponseMinute; }
        }

        public int ResponseSecond
        {
            get { return m_nResponseSecond; }
        }

        public Comment MyComment
        {
            get { return m_myComment; }
            set { m_myComment = value; }
        }
    }

    public class WaitingRequest
    {
        private int m_nRequestID = -1;
        private DateTime m_requestTime = new DateTime();
        private int m_nRequestYear = 0, m_nRequestMonth = 0, m_nRequestDay = 0;
        private int m_nRequestHour = 0, m_nRequestMinute = 0, m_nRequestSecond = 0;
        private List<Date> m_requestDays = new List<Date>();
        private string m_strPeriod = "";
        private float m_fDays = 0;
        private Models.Account.ApplicationUser m_requestMember = null;
        private List<KeyValuePair<Account.ApplicationUser, Comment>> m_prevHistories = new List<KeyValuePair<Account.ApplicationUser, Comment>>();
        private string m_strRequestDescription = "";

        public int RequestID
        {
            get { return m_nRequestID; }
            set { m_nRequestID = value; }
        }

        public DateTime RequestTime
        {
            get { return m_requestTime; }
            set
            {
                m_requestTime = value;
                m_nRequestYear = m_requestTime.Year;
                m_nRequestMonth = m_requestTime.Month;
                m_nRequestDay = m_requestTime.Day;
                m_nRequestHour = m_requestTime.Hour;
                m_nRequestMinute = m_requestTime.Minute;
                m_nRequestSecond = m_requestTime.Second;
            }
        }

        public int RequestYear
        {
            get { return m_nRequestYear; }
        }

        public int RequestMonth
        {
            get { return m_nRequestMonth; }
        }

        public int RequestDay
        {
            get { return m_nRequestDay; }
        }

        public int RequestHour
        {
            get { return m_nRequestHour; }
        }

        public int RequestMinute
        {
            get { return m_nRequestMinute; }
        }

        public int RequestSecond
        {
            get { return m_nRequestSecond; }
        }

        public List<Date> RequestDays
        {
            get { return m_requestDays; }
        }

        public string Period
        {
            get { return m_strPeriod; }
        }

        public float Days
        {
            get { return m_fDays; }
        }

        public Models.Account.ApplicationUser RequestMember
        {
            get { return m_requestMember; }
            set { m_requestMember = value; }
        }

        public List<KeyValuePair<Account.ApplicationUser, Comment>> PrevHistories
        {
            get { return m_prevHistories; }
        }

        public string RequestDescription
        {
            get { return m_strRequestDescription; }
            set { m_strRequestDescription = value; }
        }

        public void Calc()
        {
            m_requestDays.Sort();

            float fDays = 0;
            string strBeginDate = "", strEndDate = "", strDate = "";

            foreach (Date date in m_requestDays)
            {
                if (date.DateType == (int)Date.DateTypes.AM || date.DateType == (int)Date.DateTypes.PM)
                    fDays += 0.5f;
                else if (date.DateType == (int)Date.DateTypes.Normal)
                    fDays += 1;
                else
                {
                    if ((date.DateType & Date.Quater1st) == Date.Quater1st)
                        fDays += 0.25f;
                    if ((date.DateType & Date.Quater2nd) == Date.Quater2nd)
                        fDays += 0.25f;
                    if ((date.DateType & Date.Quater3rd) == Date.Quater3rd)
                        fDays += 0.25f;
                    if ((date.DateType & Date.Quater4th) == Date.Quater4th)
                        fDays += 0.25f;
                }

                strDate = string.Format("{0}월 {1}일", date.Month, date.Day);
                strDate += Date.GetDateTypeString(date.DateType, "반차");

                if (strBeginDate.Length == 0)
                    strBeginDate = strDate;

                strEndDate = strDate;
            }

            m_fDays = fDays;

            if (strBeginDate == strEndDate)
                m_strPeriod = strBeginDate;
            else
                m_strPeriod = strBeginDate + " ~ " + strEndDate;
        }

        
    }

    public class CompletedRequest : WaitingRequest
    {
        private Model.Response.ResponseType m_response = Model.Response.ResponseType.None;
        private DateTime m_responseTime = new DateTime();
        private Comment m_myComment = null;
        private int m_nResponseYear = 0, m_nResponseMonth = 0, m_nResponseDay = 0;
        private int m_nResponseHour = 0, m_nResponseMinute = 0, m_nResponseSecond = 0;

        public Model.Response.ResponseType Response
        {
            get { return m_response; }
            set { m_response = value; }
        }

        public DateTime ResponseTime
        {
            get { return m_responseTime; }
            set
            {
                m_responseTime = value;
                m_nResponseYear = m_responseTime.Year;
                m_nResponseMonth = m_responseTime.Month;
                m_nResponseDay = m_responseTime.Day;
                m_nResponseHour = m_responseTime.Hour;
                m_nResponseMinute = m_responseTime.Minute;
                m_nResponseSecond = m_responseTime.Second;
            }
        }

        public int ResponseYear
        {
            get { return m_nResponseYear; }
        }

        public int ResponseMonth
        {
            get { return m_nResponseMonth; }
        }

        public int ResponseDay
        {
            get { return m_nResponseDay; }
        }

        public int ResponseHour
        {
            get { return m_nResponseHour; }
        }

        public int ResponseMinute
        {
            get { return m_nResponseMinute; }
        }

        public int ResponseSecond
        {
            get { return m_nResponseSecond; }
        }

        public Comment MyComment
        {
            get { return m_myComment; }
            set { m_myComment = value; }
        }
    }
}
