using System;
using System.Collections.Generic;
using System.Text;

namespace Vacation.Model
{
    public class Response : IComparable
    {
        public enum Fields { ID, RequestID, ManagerID, Response, ResponseTime, ResponseDescription, PrevResponseID };
        // 승인, 거절, 처리중, 시간경과로 자동거절, 승인후 취소
        public enum ResponseType { Permit = 0, Deny, Processing, Timeout, Cancel, None };

        private int m_nID = -1;
        private int m_nRequestID = -1;
        private int m_nManagerID = -1;
        private ResponseType m_response = ResponseType.None;
        private DateTime? m_responseTime = null;
        private string m_strDescription = null;
        private int? m_nPrevResponseID = null;
        
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int RequestID
        {
            get { return m_nRequestID; }
            set { m_nRequestID = value; }
        }

        public int ManagerID
        {
            get { return m_nManagerID; }
            set { m_nManagerID = value; }
        }

        public ResponseType Result
        {
            get { return m_response; }
            set { m_response = value; }
        }

        public DateTime? ResponseTime
        {
            get { return m_responseTime; }
            set { m_responseTime = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public int? PrevResponseID
        {
            get { return m_nPrevResponseID; }
            set { m_nPrevResponseID = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Response ||
                field == Fields.ResponseDescription ||
                field == Fields.ResponseTime ||
                field == Fields.PrevResponseID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string GetTableName()
        {
            return "Response";
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 0;

            if (obj is Response)
            {
                Response response1 = this;
                Response response2 = (Response)obj;

                if (response1.ID > response2.ID)
                    return 1;
                else if (response1.ID < response2.ID)
                    return -1;
            }

            return 0;
        }
    }
}
