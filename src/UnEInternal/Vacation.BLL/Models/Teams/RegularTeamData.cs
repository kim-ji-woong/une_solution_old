using System;
using System.Collections.Generic;
using System.Text;
using Vacation.Model;

namespace Vacation.BLL.Models.Teams
{
    public class RegularTeamData
    {
        private RegularTeam m_regularTeam = null;
        public RegularTeam RegularTeam
        {
            get { return m_regularTeam; }
            set { m_regularTeam = value; }
        }
    }

    public class RegularTeamDataCollect
    {
        public List<RegularTeam> data { get; set; }
    }
}
