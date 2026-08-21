using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.BLL.Models.Response
{
    public class ResponseCommandChangeRegularMemberInfo : ResponseCommand
    {
        private int m_nNewID = -1;
        public int nNewID
        {
            get { return m_nNewID; }
            set { m_nNewID = value; }
        } 
    }
}
