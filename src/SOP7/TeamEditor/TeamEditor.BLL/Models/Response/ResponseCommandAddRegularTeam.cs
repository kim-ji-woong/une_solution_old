using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.BLL.Models.Response
{
    public class ResponseCommandAddRegularTeam : ResponseCommand
    {
        private int m_nNewID = -1;
        public int nNewID
        {
            get { return m_nNewID; }
            set { m_nNewID = value; }
        }

        private int m_nOrgID = -1;
        public int nOrgID
        {
            get { return m_nOrgID; }
            set { m_nOrgID = value; }
        }        
    }
}
