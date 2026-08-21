using System;
using System.Collections.Generic;
using Vacation.Model;

namespace Vacation.BLL.Models.Vacation
{
    using Account;

    public class History : MessageResult
    {
        private AnnualVacation m_annualVacation = null;
        private List<VacationDetail> m_usedVacations = new List<VacationDetail>();
        // 통계 기준일
        private int m_nYear = 0;
        private int m_nMonth = 0;
        private int m_nDay = 0;

        private float m_fTotalDays = 0;
        private float m_fSpecialVacationDays = 0;
        private float m_fUsedDays = 0;
        private float m_fWaitingDays = 0;
        private float m_fReservations = 0;
        // 작년에 초과해서 사용한 연차일수
        private float m_fMinusDays = 0;
        private bool m_isCalced = false;

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

        public AnnualVacation AnnualVacation
        {
            get { return m_annualVacation; }
            set { m_annualVacation = value; }
        }

        public List<VacationDetail> UsedVacations
        {
            get { return m_usedVacations; }
        }

        // 부여된 총 휴가일수(특별휴가 포함)
        public float TotalDays
        {
            get { return m_fTotalDays; }
        }

        // 부여된 특별휴가일수
        public float SpecialVacationDays
        {
            get { return m_fSpecialVacationDays; }
        }

        // 사용된 휴가일수(승인 대기중인것 포함)
        public float UsedDays
        {
            get { return m_fUsedDays; }
        }

        // 승인 대기중인 휴가일수
        public float WaitingDays
        {
            get { return m_fWaitingDays; }
        }

        // 승인되었으나 아직 휴가일이 지나지 않은 휴가일수
        public float Reservations
        {
            get { return m_fReservations; }
        }

        public void Calc()
        {
            m_fTotalDays = CalcTotalDays() - m_fMinusDays;
            m_fSpecialVacationDays = CalcSpecialVacationDays();
            m_fUsedDays = CalcUsedDays(m_nYear);
            m_fWaitingDays = CalcWaitingDays(m_nYear);
            m_fReservations = CalcReservations();
            m_isCalced = true;
        }

        public void SetMinusDays(float fMinusDays)
        {
            m_fMinusDays = fMinusDays;

            if (m_isCalced)
            {
                if (m_fMinusDays > 0)
                {
                    m_fTotalDays -= m_fMinusDays;
                }
            }
        }

        private float CalcTotalDays()
        {
            if (m_annualVacation == null)
                return 0;

            return m_annualVacation.BaseDays + m_annualVacation.SpecialVacationDays;
        }

        private float CalcSpecialVacationDays()
        {
            if (m_annualVacation == null)
                return 0;

            return m_annualVacation.SpecialVacationDays;
        }

        private float CalcUsedDays(int year)
        {
            float fDays = 0;

            foreach (VacationDetail detail in m_usedVacations)
            {
                fDays += detail.GetYearTotalDays(year);
                //fDays += detail.TotalDays;
            }

            return fDays;
        }

        private float CalcWaitingDays(int year)
        {
            float fDays = 0;

            foreach (VacationDetail detail in m_usedVacations)
            {
                if (detail.IsPermitted == false)
                {
                    fDays += detail.GetYearTotalDays(year);
                    //fDays += detail.TotalDays;
                }
            }

            return fDays;
        }

        private float CalcReservations()
        {
            DateTime dtNow = DateTime.Now;
            int today = dtNow.Year * 10000 + dtNow.Month * 100 + dtNow.Day;

            float fDays = 0;

            foreach (VacationDetail detail in m_usedVacations)
            {
                if (detail.IsPermitted)
                {
                    foreach (Date date in detail.Dates)
                    {
                        if (date.Year != m_nYear)
                            continue;

                        int _date = date.Year * 10000 + date.Month * 100 + date.Day;

                        if (_date == today)
                        {
                            if (dtNow.Hour < 12)
                            {
                                if (date.DateType == (int)Date.DateTypes.PM || date.DateType == (int)Date.DateTypes.Normal)
                                    fDays += 0.5f;
                                else
                                {
                                    if ((date.DateType & Date.Quater3rd) == Date.Quater3rd)
                                        fDays += 0.25f;
                                    if ((date.DateType & Date.Quater4th) == Date.Quater4th)
                                        fDays += 0.25f;
                                }
                            }
                        }
                        else if (_date > today)
                        {
                            if (date.DateType == (int)Date.DateTypes.Normal)
                                fDays += 1;
                            else
                            {
                                if (date.DateType == (int)Date.DateTypes.AM || date.DateType == (int)Date.DateTypes.PM)
                                    fDays += 0.5f;
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
                            }
                        }
                    }
                }
            }

            return fDays;
        }
    }

    public class VacationDetail
    {
        // 총 휴가일수
        private float m_fTotalDays = 0;
        // 연도별 총 휴가일수
        private Dictionary<int, float> m_dicTotalDays = new Dictionary<int, float>();
        private List<Date> m_dates = new List<Date>();
        private string m_strDays = "";
        private string m_strDetailDays = "";
        // 승인되었는가?
        private bool m_isPermitted = false;
        private List<KeyValuePair<ApplicationUser, Comment>> m_managers = new List<KeyValuePair<ApplicationUser, Comment>>();

        // 총 휴가일수
        public float TotalDays
        {
            get { return m_fTotalDays; }
        }

        public List<Date> Dates
        {
            get { return m_dates; }
        }

        public string Days
        {
            get { return m_strDays; }
        }

        public string DetailDays
        {
            get { return m_strDetailDays; }
        }

        public bool IsPermitted
        {
            get { return m_isPermitted; }
        }

        public List<KeyValuePair<ApplicationUser, Comment>> Managers
        {
            get { return m_managers; }
        }

        public float GetYearTotalDays(int year)
        {
            float fDays;

            if (m_dicTotalDays.TryGetValue(year, out fDays))
                return fDays;

            return 0;
        }

        public void Calc()
        {
            m_dates.Sort();

            // 전체 휴가일수
            float fDays = 0;
            // 연도별 휴가일수
            Dictionary<int, float> dicYearDays = new Dictionary<int, float>();
            string strBeginDate = "", strEndDate = "", strDate = "";
            m_strDetailDays = "";

            foreach (Date date in m_dates)
            {
                float yearDays = 0;
                bool contains = dicYearDays.TryGetValue(date.Year, out yearDays);

                if (contains == false)
                    yearDays = 0;

                float fCount = Date.GetDateCount(date.DateType);
                fDays += fCount;
                dicYearDays[date.Year] = yearDays + fCount;

                strDate = string.Format("{0}월 {1}일", date.Month, date.Day);
                strDate += Date.GetDateTypeString(date.DateType, "반차");

                if (strBeginDate.Length == 0)
                    strBeginDate = strDate;

                strEndDate = strDate;

                if (m_strDetailDays.Length == 0)
                    m_strDetailDays = strDate;
                else
                    m_strDetailDays += ", " + strDate;
            }

            m_fTotalDays = fDays;
            m_dicTotalDays = dicYearDays;

            if (strBeginDate == strEndDate)
                m_strDays = strBeginDate;
            else
                m_strDays = strBeginDate + " ~ " + strEndDate;

            bool isPermitted = true;

            foreach (KeyValuePair<ApplicationUser, Comment> pair in m_managers)
            {
                if (pair.Value == null)
                {
                    isPermitted = false;
                    break;
                }
                else if (pair.Value.ResponseType != (int)Response.ResponseType.Permit)
                {
                    isPermitted = false;
                    break;
                }
            }

            m_isPermitted = isPermitted;
        }
    }

    public class AnnualVacation
    {
        // 기본 제공연차 혹은 월차
        private float m_fBaseDays = 0;
        // 특별 휴가
        private List<SpecialVacationData> m_specialVacations = new List<SpecialVacationData>();

        public float BaseDays
        {
            get { return m_fBaseDays; }
            set { m_fBaseDays = value; }
        }

        public List<SpecialVacationData> SpecialVacations
        {
            get { return m_specialVacations; }
        }

        public float SpecialVacationDays
        {
            get
            {
                float fDays = 0;

                foreach (SpecialVacationData vacation in m_specialVacations)
                {
                    fDays += vacation.Days;
                }

                return fDays;
            }
        }
    }

    public class SpecialVacationData
    {
        // 휴가일수
        private float m_fDays = 0;
        // 휴가 발생시간
        private DateTime m_dtCreate = new DateTime();
        // 승인한 관리자들 이력
        private List<KeyValuePair<ApplicationUser, Comment>> m_managerHistories = new List<KeyValuePair<ApplicationUser, Comment>>();
        private string m_strDescription = "";
        
        public float Days
        {
            get { return m_fDays; }
            set { m_fDays = value; }
        }

        public DateTime CreateTime
        {
            get { return m_dtCreate; }
            set { m_dtCreate = value; }
        }

        public List<KeyValuePair<ApplicationUser, Comment>> ManagerHistories
        {
            get { return m_managerHistories; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }

    public class Comment
    {
        private int m_responseType = (int)Response.ResponseType.None;
        private string m_strDescription = "";
        private DateTime m_timeStamp = new DateTime();

        public int ResponseType
        {
            get { return m_responseType; }
            set { m_responseType = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public Comment()
        {
        }

        public Comment(string strDescription, DateTime time)
        {
            m_strDescription = strDescription;
            m_timeStamp = time;
        }
    }
}
