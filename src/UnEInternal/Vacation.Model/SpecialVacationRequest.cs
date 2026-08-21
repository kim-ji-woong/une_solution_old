using System;
using System.Collections.Generic;
using System.Text;

namespace Vacation.Model
{
    public class SpecialVacationRequest
    {
        public enum Fields { ID, Days, RequestTime, RequestManagerID, MemberIDs, ResponseManagerIDs, Response, RequestDescription };

        private int m_nID = -1;
        // 휴가일수
        private float m_fDays = 0;
        // 휴가 요청시간
        private DateTime m_requestTime = new DateTime();
        // 특별휴가를 신청한 담당자
        private int m_nRequestManagerID = -1;
        // 특별휴가를 부여받는 직원들
        private List<int> m_memberIDs = new List<int>();
        // 휴가 승인권자 리스트
        private List<int> m_responseManagerIDs = new List<int>();
        private Model.Response.ResponseType m_response;
        private string m_strRequestDescription = null;

        public SpecialVacationRequest()
        {
            m_response = Model.Response.ResponseType.None;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // 휴가일수
        public float Days
        {
            get { return m_fDays; }
            set { m_fDays = value; }
        }

        // 휴가 요청시간
        public DateTime RequestTime
        {
            get { return m_requestTime; }
            set { m_requestTime = value; }
        }

        // 특별휴가를 신청한 담당자
        public int RequestManagerID
        {
            get { return m_nRequestManagerID; }
            set { m_nRequestManagerID = value; }
        }

        // 특별휴가를 부여받는 직원들
        public List<int> MemberIDs
        {
            get { return m_memberIDs; }
        }

        // 휴가 승인권자 리스트
        public List<int> ResponseManagerIDs
        {
            get { return m_responseManagerIDs; }
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

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Response ||
                field == Fields.RequestDescription)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string GetTableName()
        {
            return "SpecialVacationRequest";
        }
    }
}
