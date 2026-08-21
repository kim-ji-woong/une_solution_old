using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.BLL.Models.Response
{
    public class ResponseUpdateRegularMember : MessageResult
    {
        private int m_nNewID = -1;
        public int NewID
        {
            get { return m_nNewID; }
            set { m_nNewID = value; }
        }
    }

    public class ResponseUpdateTemporaryMember : MessageResult
    {
        private int m_nNewID = -1;
        public int NewID
        {
            get { return m_nNewID; }
            set { m_nNewID = value; }
        }
    }

    public class ResponseUpdateRegularTeam : MessageResult
    {
        private int m_nNewID = -1;
        public int NewID
        {
            get { return m_nNewID; }
            set { m_nNewID = value; }
        }
    }

    public class ResponseUpdateTemporaryTeam : MessageResult
    {
        private int m_nNewID = -1;
        public int NewID
        {
            get { return m_nNewID; }
            set { m_nNewID = value; }
        }
    }
}
