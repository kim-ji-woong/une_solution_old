using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using Model.Sop.Config;

    public class ResponseSpecialMessageList : MessageResult
    {
        private List<SpecialMessage> m_specialMessages = new List<SpecialMessage>();

        public List<SpecialMessage> SpecialMessages
        {
            get { return m_specialMessages; }
        }

        public ResponseSpecialMessageList()
        {
        }

        public ResponseSpecialMessageList(bool success, List<SpecialMessage> spcecialMessages, string strErrorMessage)
        {
            Success = success;
            Message = strErrorMessage;

            if (spcecialMessages != null)
            {
                m_specialMessages.AddRange(spcecialMessages);
            }
        }
    }
}
