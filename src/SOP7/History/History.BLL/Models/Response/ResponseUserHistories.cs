using History.BLL.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Response
{
    public class ResponseUserHistories
    {
        private List<UserHistoryData> m_userHistoryDatas = new List<UserHistoryData>();
        public List<UserHistoryData> UserHistoryDatas
        {
            get { return m_userHistoryDatas; }
            set { m_userHistoryDatas = value; }
        }
    }
}
