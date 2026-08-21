using System.Collections.Generic;

namespace Vacation.Model
{
    // 특별휴가
    // 특별휴가가 발생하면 History의 TotalDays에 더해진다.
    public class SpecialVacation
    {
        public enum Fields { ID, MemberID, Days, CreateTime, ManagerIDs, RequestID, Description };

        private int m_nID = -1;
        private int m_nMemberID = -1;
        // 특별휴가가 총 몇일인가?
        private float m_days = 0;
        // 특별휴가 발생일자
        private System.DateTime m_dtCreate = new System.DateTime();
        // 특별휴가 승인권자
        // 승인한 순서대로 List에 담긴다.
        private List<int> m_managerIDs = new List<int>();
        private int m_nRequestID = 0;
        private string m_strDescription = "";
        
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

        public float Days
        {
            get { return m_days; }
            set { m_days = value; }
        }

        // 특별휴가 발생일자
        public System.DateTime CreateTime
        {
            get { return m_dtCreate; }
            set { m_dtCreate = value; }
        }

        // 특별휴가 승인권자
        // 승인한 순서대로 List에 담긴다.
        public List<int> ManagerIDs
        {
            get { return m_managerIDs; }
        }

        public int RequestID
        {
            get { return m_nRequestID; }
            set { m_nRequestID = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public static string GetTableName()
        {
            return "SpecialVacation";
        }
    }
}
