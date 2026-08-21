using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL.Models.Response
{
    public class ResponseExcelInfo : MessageResult
    {
        private byte[] m_bytes = null;

        public byte[] Bytes
        {
            get { return m_bytes; }
            set { m_bytes = value; }
        }

        public ResponseExcelInfo(bool success, string strMessage)
            : base(success, strMessage)
        {
        }
    }
}
