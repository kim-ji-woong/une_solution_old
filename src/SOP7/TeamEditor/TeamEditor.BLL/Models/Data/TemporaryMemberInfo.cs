using System.Collections.Generic;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models.Data
{
    public class TemporaryMemberInfo
    {
        private int m_nID = -1;
        private string m_strDisplaySOPName = "";
        private Temporary m_temporary = null;
        private Regular m_regular = null;
        private RegularMember m_regularMember = null;
        private int m_nIsNormal = -1;
        private int? m_nRole = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string DisplaySOPName
        {
            get { return m_strDisplaySOPName; }
            set { m_strDisplaySOPName = value; }
        }

        public Temporary Temporary
        {
            get { return m_temporary; }
            set { m_temporary = value; }
        }

        public Regular Regular
        {
            get { return m_regular; }
            set { m_regular = value; }
        }

        public RegularMember RegularMember
        {
            get { return m_regularMember; }
            set { m_regularMember = value; }
        }

        public int IsNormal
        {
            get { return m_nIsNormal; }
            set { m_nIsNormal = value; }
        }

        public int? Role
        {
            get { return m_nRole; }
            set { m_nRole = value; }
        }
    }
}
