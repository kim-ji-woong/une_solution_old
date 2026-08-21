using History.BLL.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Response
{
    public class ResponseSOPHistories
    {
        private List<SOPHistoryData> m_sopHistoryDatas = new List<SOPHistoryData>();
        public List<SOPHistoryData> SOPHistoryDatas
        {
            get { return m_sopHistoryDatas; }
            set { m_sopHistoryDatas = value; }
        }
    }
}
