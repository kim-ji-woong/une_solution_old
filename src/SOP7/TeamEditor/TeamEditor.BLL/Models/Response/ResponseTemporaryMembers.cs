using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Data;

namespace TeamEditor.BLL.Models.Response
{
    public class ResponseTemporaryMembers : MessageResult
    {
        private List<TemporaryMemberInfo> m_temporaryMemberInfos = null;

        public List<TemporaryMemberInfo> TemporaryMemberInfos
        {
            get { return m_temporaryMemberInfos; }
            set { m_temporaryMemberInfos = value; }
        }
    }
}
