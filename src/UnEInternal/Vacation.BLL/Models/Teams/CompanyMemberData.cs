using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vacation.Model;

namespace Vacation.BLL.Models.Teams
{
    public class CompanyMemberData
    {
        private RegularTeam m_regularTeam = null;
        public RegularTeam RegularTeam
        {
            get { return m_regularTeam; }
            set { m_regularTeam = value; }
        }

        private CompanyMember m_companyMember = null;
        public CompanyMember CompanyMember
        {
            get { return m_companyMember; }
            set { m_companyMember = value; }
        }

        private JobLevel m_jobLevel = null;
        public JobLevel JobLevel
        {
            get { return m_jobLevel; }
            set { m_jobLevel = value; }
        }

        private string m_strStartDate = "";
        public string StartDate
        {
            get { return m_strStartDate; }
            set { m_strStartDate = value; }
        }
    }

    public class CompanyMemberDataCollect
    {
        public List<CompanyMemberData> data { get; set; }
    }
}
