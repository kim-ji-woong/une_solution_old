using System;
using System.Collections.Generic;

namespace Vacation.Model
{
    // 연간 휴가 사용내역 및 남은 일수
    public class History
    {
        public enum Fields { MemberID, Year, TotalDays, UsedDays, WaitingDays, RequestIDs, NextVacationDay };

        private int m_nMemberID = -1;
        private int m_nYear = -1;
        // int가 아닌 이유는 반차 때문이다.
        // 총 휴가일수
        private float m_fTotalDays = 0;
        // 사용한 휴가일수
        private float m_fUsedDays = 0;
        private float m_fWaitingDays = 0;
        private List<int> m_requestIDs = new List<int>();
        // 다음 휴가 발생일자
        private DateTime m_dtNextVacation = new DateTime();

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        // 총 휴가일수
        public float TotalDays
        {
            get { return m_fTotalDays; }
            set { m_fTotalDays = value; }
        }

        // 사용한 휴가일수
        public float UsedDays
        {
            get { return m_fUsedDays; }
            set { m_fUsedDays = value; }
        }

        // 승인 대기중인 휴가일수
        public float WaitingDays
        {
            get { return m_fWaitingDays; }
            set { m_fWaitingDays = value; }
        }

        public List<int> RequestIDs
        {
            get { return m_requestIDs; }
        }

        // 다음 휴가 발생일자
        public DateTime NextVacationDay
        {
            get { return m_dtNextVacation; }
            set { m_dtNextVacation = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public static string GetTableName()
        {
            return "History";
        }
    }
}
