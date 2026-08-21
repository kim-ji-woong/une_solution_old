using System;
using System.Collections.Generic;

namespace Vacation.BLL.Models.Vacation
{
    using Model;
    using Account;

    public class VacationInfo
    {
        // 결재 대기중, 결재완료, 휴가 진행중, 휴가기간 종료
        public enum StatusType { None = -1, Wait = 0, Confirm, InProgress, Finish };

        private int m_nRequestID = -1;
        // 마지막으로 승인한 관리자
        private ApplicationUser m_lastManager = null;
        // 승인일자
        private DateTime? m_dtConfirm = null;
        private List<Date> m_days = new List<Date>();
        private string m_strDaysDescription = "";
        private StatusType m_status = StatusType.None;

        public int RequestID
        {
            get { return m_nRequestID; }
            set { m_nRequestID = value; }
        }

        public ApplicationUser LastManager
        {
            get { return m_lastManager; }
            set { m_lastManager = value; }
        }

        public DateTime? ConfirmTime
        {
            get { return m_dtConfirm; }
            set { m_dtConfirm = value; }
        }

        public List<Date> Days
        {
            get { return m_days; }
            set { m_days = value; }
        }

        public string DaysDescription
        {
            get { return m_strDaysDescription; }
            set { m_strDaysDescription = value; }
        }

        public int Status
        {
            get { return (int)m_status; }
            set { m_status = (StatusType)value; }
        }

        public StatusType ToStatus(Response.ResponseType responseType)
        {
            if (responseType == Response.ResponseType.Permit)
            {
                DateTime dtNow = DateTime.Now;
                int today = dtNow.Year * 10000 + dtNow.Month * 100 + dtNow.Day;

                bool begin = false, end = false;
                int nDateCount = m_days.Count;

                for (int i = 0; i < nDateCount; i++)
                {
                    Date date = m_days[i];
                    int day = date.Year * 10000 + date.Month * 100 + date.Day;

                    if (day < today)
                        begin = true;
                    else if (day == today)
                    {
                        if (Date.IsFullDay(date.DateType) || Date.BeforeNoon(date.DateType))
                        {
                            if (dtNow.Hour > VacationManager.BeginWorkHour || (dtNow.Hour == VacationManager.BeginWorkHour && dtNow.Minute >= VacationManager.BeginWorkMinute))
                                begin = true;
                        }
                        else// if (date.Type == Date.DateType.PM)
                        {
                            if (dtNow.Hour >= 12)
                                begin = true;
                        }
                    }

                    if (i == nDateCount - 1)
                    {
                        if (day < today)
                            end = true;
                        else if (day == today)
                        {
                            if (Date.BeforeNoon(date.DateType))
                            {
                                // 오전업무 시작시간이 지났으면 사용 완료한 것으로 간주한다.
                                if (dtNow.Hour > VacationManager.BeginWorkHour || (dtNow.Hour == VacationManager.BeginWorkHour && dtNow.Minute >= VacationManager.BeginWorkMinute))
                                    end = true;
                                /*if (dtNow.Hour >= 12)
                                    end = true;*/
                            }
                            else// if (date.Type == Date.DateType.Normal || date.Type == Date.DateType.PM)
                            {
                                // 오후업무 시작시간이 지났으면 사용 완료한 것으로 간주한다.
                                if (dtNow.Hour >= 12)
                                    end = true;
                                /*if (dtNow.Hour > VacationManager.EndWorkHour || (dtNow.Hour == VacationManager.EndWorkHour && dtNow.Minute >= VacationManager.EndWorkMinute))
                                    end = true;*/
                            }
                        }
                    }
                }

                if (begin && end)
                    return StatusType.Finish;
                else if (begin)
                    return StatusType.InProgress;
                else
                    return StatusType.Confirm;
            }
            else if (responseType == Response.ResponseType.Processing || responseType == Response.ResponseType.None)
                return StatusType.Wait;

            return StatusType.None;
        }
    }

    public class ResponseVacationList : MessageResult
    {
        private List<VacationInfo> m_vacations = new List<VacationInfo>();

        public List<VacationInfo> Vacations
        {
            get { return m_vacations; }
            set { m_vacations = value; }
        }

        public ResponseVacationList()
        {
        }

        public ResponseVacationList(bool success, string strMessage)
            : base(success, strMessage)
        {
        }
    }
}
