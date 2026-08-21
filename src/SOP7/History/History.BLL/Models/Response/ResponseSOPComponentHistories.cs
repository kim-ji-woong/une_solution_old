using History.BLL.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace History.BLL.Models.Response
{
    public class ResponseSOPComponentHistories
    {
        private List<SopHistoryComponentData> m_sopComponentHistoryDatas = new List<SopHistoryComponentData>();
        public List<SopHistoryComponentData> SOPComponentHistoryDatas
        {
            get { return m_sopComponentHistoryDatas; }
            set { m_sopComponentHistoryDatas = value; }
        }
    }
}
