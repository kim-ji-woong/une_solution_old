using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Data;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models.Response
{
    public class ResponseRegulars : MessageResult
    {
        private List<Regular> m_regulars = null;

        public List<Regular> Regulars
        {
            get { return m_regulars; }
            set { m_regulars = value; }
        }
    }
}
