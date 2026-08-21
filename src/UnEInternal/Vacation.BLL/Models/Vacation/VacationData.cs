using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vacation.Model;

namespace Vacation.BLL.Models.Vacation
{
    public class VacationData
    {
        private _RequestManager m_requestManager = null;
        private _RequestManager m_requestSpecialVacationManager = null;
        private _RequestHistory m_requestHistory = null;
        private _RequestVacation m_requestVacation = null;
        private _RequestSpecialVacation m_requestSpecialVacation = null;
        private _RequestManagerData m_requestManagerData = null;
        private _ProcessRequest m_processRequest = null;
        private _RequestMemberHistory m_requestMemberHistory = null;
        private RequestVacationList m_requestVacationList = null;
        private RequestCancelVacations m_requestCancelVacations = null;
        private RequestHolidays m_requestHolidays = null;

        public RequestCancelVacations RequestCancelVacations
        {
            get { return m_requestCancelVacations; }
            set { m_requestCancelVacations = value; }
        }

        public RequestVacationList RequestVacationList
        {
            get { return m_requestVacationList; }
            set { m_requestVacationList = value; }
        }

        public _RequestManager RequestManager
        {
            get { return m_requestManager; }
            set { m_requestManager = value; }
        }

        public _RequestManager RequestSpecialVacationManager
        {
            get { return m_requestSpecialVacationManager; }
            set { m_requestSpecialVacationManager = value; }
        }

        public _RequestHistory RequestHistory
        {
            get { return m_requestHistory; }
            set { m_requestHistory = value; }
        }

        public _RequestVacation RequestVacation
        {
            get { return m_requestVacation; }
            set { m_requestVacation = value; }
        }

        public _RequestSpecialVacation RequestSpecialVacation
        {
            get { return m_requestSpecialVacation; }
            set { m_requestSpecialVacation = value; }
        }

        public _RequestManagerData RequestManagerData
        {
            get { return m_requestManagerData; }
            set { m_requestManagerData = value; }
        }

        public _ProcessRequest ProcessRequest
        {
            get { return m_processRequest; }
            set { m_processRequest = value; }
        }

        public _RequestMemberHistory RequestMemberHistory
        {
            get { return m_requestMemberHistory; }
            set { m_requestMemberHistory = value; }
        }

        public RequestHolidays RequestHolidays
        {
            get { return m_requestHolidays; }
            set { m_requestHolidays = value; }
        }
    }

    public class _RequestHistory
    {
        private string m_strUserID = "";
        private int m_nYear = 0;
        private int m_nMonth = 0;
        private int m_nDay = 0;

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        public int Month
        {
            get { return m_nMonth; }
            set { m_nMonth = value; }
        }

        public int Day
        {
            get { return m_nDay; }
            set { m_nDay = value; }
        }
    }

    public class _RequestManager
    {
        private string m_strUserID = "";
        private string m_strRequestDays = "";

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        // YYYYMMDD:Type
        // Type은 하루종일(0), 오전반차(1), 오후반차(2)로 구분
        // 하루종일일 경우 Type은 생략 가능
        // 여러날짜일 경우 각 날짜들의 구분은 세미콜론(;)으로 한다.
        public string RequestDays
        {
            get { return m_strRequestDays; }
            set { m_strRequestDays = value; }
        }

        public List<Date> GetDateList()
        {
            List<Date> dates = new List<Date>();

            if (m_strRequestDays.Length == 0)
                return dates;

            string[] tokens = m_strRequestDays.Split(';');

            foreach (string strToken in tokens)
            {
                string strDate = strToken.Trim();
                int nIndex = strDate.LastIndexOf(':');

                if (strDate.Length < 8)
                    continue;

                string _strDate = strDate.Substring(0, 8);

                int year, month, day;

                if (!int.TryParse(_strDate.Substring(0, 4), out year) ||
                    !int.TryParse(_strDate.Substring(4, 2), out month) ||
                    !int.TryParse(_strDate.Substring(6, 2), out day))
                    continue;

                Date date = new Date();
                date.Year = year;
                date.Month = month;
                date.Day = day;

                if (nIndex > 0)
                {
                    string strType = strDate.Substring(nIndex + 1).Trim();
                    int nType;

                    if (int.TryParse(strType, out nType))
                        date.DateType = nType;
                    else
                        continue;
                }

                dates.Add(date);
            }

            dates.Sort();
            return dates;
        }
    }

    public class _RequestVacation : _RequestManager
    {
        private string m_strDescription = null;

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }

    public class _RequestSpecialVacation
    {
        private string m_strRequestManagerID = "";
        private float m_fDays = 0.0f;
        private List<string> m_userIDs = new List<string>();
        private string m_strReason = "";

        public string RequestManagerID
        {
            get { return m_strRequestManagerID; }
            set { m_strRequestManagerID = value; }
        }

        public float Days
        {
            get { return m_fDays; }
            set { m_fDays = value; }
        }

        public List<string> UserIDs
        {
            get { return m_userIDs; }
            set { m_userIDs = value; }
        }

        public string Reason
        {
            get { return m_strReason; }
            set { m_strReason = value; }
        }
    }

    public class _RequestManagerData
    {
        private string m_strManagerUserID = "";
        private int m_nYear = 0;

        public string ManagerUserID
        {
            get { return m_strManagerUserID; }
            set { m_strManagerUserID = value; }
        }

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }
    }

    public class _ProcessRequest
    {
        private int m_nRequestID = -1;
        private bool m_isPermit = false;
        private string m_strManagerUserID = "";
        private string m_strManagerDescription = null;
        // true면 일반휴가, false면 특별휴가
        private bool m_isNormal = true;

        public int RequestID
        {
            get { return m_nRequestID; }
            set { m_nRequestID = value; }
        }

        public bool IsPermit
        {
            get { return m_isPermit; }
            set { m_isPermit = value; }
        }

        public string ManagerUserID
        {
            get { return m_strManagerUserID; }
            set { m_strManagerUserID = value; }
        }

        public string ManagerDescription
        {
            get { return m_strManagerDescription; }
            set { m_strManagerDescription = value; }
        }

        // true면 일반휴가, false면 특별휴가
        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }
    }

    public class _RequestMemberHistory
    {
        private string m_strManagerUserID = "";

        public string ManagerUserID
        {
            get { return m_strManagerUserID; }
            set { m_strManagerUserID = value; }
        }
    }

    // 결재 대기중인것 포함하여 아직 휴가가 완료되지 않은 모든 휴가정보를 얻어온다.
    public class RequestVacationList
    {
        private string m_strUserID = "";
        private int m_nYear = 0;

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }
    }

    public class RequestCancelVacations
    {
        private List<int> m_requestIDs = new List<int>();

        public List<int> RequestIDs
        {
            get { return m_requestIDs; }
            set { m_requestIDs = value; }
        }
    }

    public class RequestHolidays
    {
        private int m_nYear = -1;
        private int? m_month = null;

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        public int? Month
        {
            get { return m_month; }
            set { m_month = value; }
        }
    }
}
