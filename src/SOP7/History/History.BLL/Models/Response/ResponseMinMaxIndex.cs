using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Response
{
    public class ResponseMinMaxIndex
    {
        private int m_nMinReactionHistoryID = -1;
        private int m_nMaxReactionHistoryID = -1;

        public int MinReactionHistoryID
        {
            get { return m_nMinReactionHistoryID; }
            set { m_nMinReactionHistoryID = value; }
        }

        public int MaxReactionHistoryID
        {
            get { return m_nMaxReactionHistoryID; }
            set { m_nMaxReactionHistoryID = value; }
        }
    }
}
