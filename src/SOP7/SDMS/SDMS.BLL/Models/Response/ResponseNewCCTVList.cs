using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.BLL.Models.Response
{
    using Model.CCTV;

    public class ResponseNewCCTVList : MessageResult
    {
        private List<CCTV> m_cctvs = new List<CCTV>();

        public List<CCTV> CCTVs
        {
            get { return m_cctvs; }
            set { m_cctvs = value; }
        }

        public ResponseNewCCTVList()
        {
        }

        public ResponseNewCCTVList(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
