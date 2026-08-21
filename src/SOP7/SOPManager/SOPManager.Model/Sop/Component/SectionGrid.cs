namespace SOPManager.Model.Sop.Component
{
    public class SectionGrid
    {
        public enum Fields { ID, StepMemberID };

        private int m_nID = -1;
        private int m_nStepMemberID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int StepMemberID
        {
            get { return m_nStepMemberID; }
            set { m_nStepMemberID = value; }
        }

        public static string TableName
        {
            get { return "SopComponentSectionGrid"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }
    }
}
