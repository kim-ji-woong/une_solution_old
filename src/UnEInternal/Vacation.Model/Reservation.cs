using System.Collections.Generic;

namespace Vacation.Model
{
    public class Reservation
    {
        public enum Fields { ID, MemberID, RequestID, Days, Year, Year2 };

        private int m_nID = -1;
        private int m_nMemberID = -1;
        private int m_nRequestID = -1;
        // 휴가 예정일
        private List<Date> m_days = new List<Date>();
        private int m_nYear = -1;
        // 휴가 예정일이 두 해에 걸쳐 있을경우 두번째 해의 년도
        private int? m_nYear2 = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        public int RequestID
        {
            get { return m_nRequestID; }
            set { m_nRequestID = value; }
        }

        // 휴가 예정일
        public List<Date> Days
        {
            get { return m_days; }
        }

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        // 휴가 예정일이 두 해에 걸쳐 있을경우 두번째 해의 년도
        public int? Year2
        {
            get { return m_nYear2; }
            set { m_nYear2 = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Year2)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string GetTableName()
        {
            return "Reservation";
        }
    }
}
