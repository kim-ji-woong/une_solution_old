using System;
using System.Collections.Generic;

namespace Vacation.Model
{
    public class Request
    {
        public enum Fields { ID, RequestTime, MemberID, Days, ManagerIDs, Response, RequestDescription, Year, Year2, MailSendTime };
        
        private int m_nID = -1;
        // 휴가 요청시간
        private DateTime m_requestTime = new DateTime();
        private int m_nMemberID = -1;
        // 휴가 요청일
        private List<Date> m_days = new List<Date>();
        // 휴가 승인권자 리스트
        private List<int> m_managerIDs = new List<int>();
        private Model.Response.ResponseType m_response;
        private string m_strRequestDescription = null;
        private int m_nYear = -1;
        // 휴가 요청일이 두 해에 걸쳐 있을경우 두번째 해의 년도
        private int? m_nYear2 = null;
        // 메일공지한 시간
        private DateTime? m_mailSendTime = null;

        public Request()
        {
            m_response = Model.Response.ResponseType.None;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // 휴가 요청시간
        public DateTime RequestTime
        {
            get { return m_requestTime; }
            set { m_requestTime = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        // 휴가 요청일
        public List<Date> Days
        {
            get { return m_days; }
        }

        // 휴가 승인권자 리스트
        public List<int> ManagerIDs
        {
            get { return m_managerIDs; }
        }

        public Model.Response.ResponseType Response
        {
            get { return m_response; }
            set { m_response = value; }
        }

        public string RequestDescription
        {
            get { return m_strRequestDescription; }
            set { m_strRequestDescription = value; }
        }

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        // 휴가 요청일이 두 해에 걸쳐 있을경우 두번째 해의 년도
        public int? Year2
        {
            get { return m_nYear2; }
            set { m_nYear2 = value; }
        }

        // 메일공지한 시간
        public DateTime? MailSendTime
        {
            get { return m_mailSendTime; }
            set { m_mailSendTime = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Response ||
                field == Fields.RequestDescription ||
                field == Fields.Year2 ||
                field == Fields.MailSendTime)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string GetTableName()
        {
            return "Request";
        }
    }
}
