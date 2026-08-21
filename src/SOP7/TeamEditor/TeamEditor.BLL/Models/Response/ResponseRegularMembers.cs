using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Data;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models.Response
{
    public class ResponseRegularMembers : MessageResult
    {
        private List<RegularMember> m_regularMembers = null;

        public List<RegularMember> RegularMembers
        {
            get { return m_regularMembers; }
            set { m_regularMembers = value; }
        }
    }
}
