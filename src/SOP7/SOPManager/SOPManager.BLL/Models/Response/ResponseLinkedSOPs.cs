using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using Model.Sop.Config;

    public class ResponseLinkedSOPs : MessageResult
    {
        private List<LinkedSop> m_linkedSops = null;

        public List<LinkedSop> LinkedSops
        {
            get { return m_linkedSops; }
            set { m_linkedSops = value; }
        }
    }


}
