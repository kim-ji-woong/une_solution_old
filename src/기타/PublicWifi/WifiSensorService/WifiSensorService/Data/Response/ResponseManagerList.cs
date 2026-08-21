using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WifiSensorService.Data.Response
{
    using Request;

    public class ResponseManagerList : MessageResult
    {
        private List<RequestCreateManager> m_managerList = new List<RequestCreateManager>();

        public List<RequestCreateManager> ManagerList
        {
            get { return m_managerList; }
            set { m_managerList = value; }
        }

        public ResponseManagerList()
            : base()
        {
        }

        public ResponseManagerList(bool success, string strMessage)
            : base(success, strMessage)
        {
        }
    }
}
