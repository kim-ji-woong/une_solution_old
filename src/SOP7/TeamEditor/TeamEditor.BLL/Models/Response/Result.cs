using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.BLL.Models.Response
{
    [Serializable]
    public class Result
    {
        private bool m_result = false;

        public bool Success
        {
            get { return m_result; }
            set { m_result = value; }
        }
    }

    [Serializable]
    public class MessageResult : Result
    {
        private string m_strMessage = "";

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }
    }
}
